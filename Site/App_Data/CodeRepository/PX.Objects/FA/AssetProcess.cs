using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using PX.Common;
using PX.Data;
using PX.Objects.FA.Overrides.AssetProcess;
using PX.Objects.GL;
using PX.Objects.CS;
using PX.Objects.CM;

namespace PX.Objects.FA
{
	public class PXMassProcessException : PXException
	{
		protected Exception _InnerException;
		protected int _ListIndex;

		public int ListIndex
		{
			get
			{
				return this._ListIndex;
			}
		}

		public PXMassProcessException(int ListIndex, Exception InnerException)
			: base(InnerException is PXOuterException ? InnerException.Message + "\r\n" + String.Join("\r\n", ((PXOuterException)InnerException).InnerMessages) : InnerException.Message, InnerException)
		{
			this._ListIndex = ListIndex;
		}

		public PXMassProcessException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			PXReflectionSerializer.RestoreObjectProps(this, info);
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			PXReflectionSerializer.GetObjectData(this, info);
			base.GetObjectData(info, context);
		}

	}

	[Serializable]
	public class AssetTranRelease : PXGraph<AssetTranRelease>
	{
		public PXCancel<ReleaseFilter> Cancel;
		public PXFilter<ReleaseFilter> Filter;
		[PXFilterable]
		[PX.SM.PXViewDetailsButton(typeof(FARegister.refNbr), WindowMode = PXRedirectHelper.WindowMode.NewWindow)]
		public PXFilteredProcessing<FARegister, ReleaseFilter, Where<True, Equal<True>>, OrderBy<Desc<FARegister.selected, Asc<FARegister.finPeriodID>>>> FADocumentList;
		public PXSelect<FATran> Trans;
		public PXSetup<FASetup> fasetup;

		protected virtual void FARegister_RowSelecting(PXCache sender, PXRowSelectingEventArgs e)
		{
			if (e.Row != null)
			{
				using (new PXConnectionScope())
				{
					PXFormulaAttribute.CalcAggregate<FATran.tranAmt>(Trans.Cache, e.Row, true);
				}
			}
		}

		public IEnumerable fADocumentList(PXAdapter adapter)
		{
			ReleaseFilter filter = Filter.Current;

			PXSelectBase<FARegister> cmd = new PXSelect<FARegister, Where<FARegister.released, Equal<False>, And<FARegister.hold, Equal<False>>>>(this);
			if (filter.Origin != null)
			{
				cmd.WhereAnd<Where<FARegister.origin, Equal<Current<ReleaseFilter.origin>>>>();
			}
			return cmd.Select();
		}

		public AssetTranRelease()
		{
			if(fasetup.Current.UpdateGL != true)
			{
				throw new PXSetupNotEnteredException<FASetup>(Messages.OperationNotWorkInInitMode, PXUIFieldAttribute.GetDisplayName<FASetup.updateGL>(fasetup.Cache));
			}

			FADocumentList.SetProcessDelegate(list => ReleaseDoc(list, true));
			FADocumentList.SetProcessCaption(Messages.Release);
			FADocumentList.SetProcessAllCaption(Messages.ReleaseAll);
		}

		public static DocumentList<Batch> ReleaseDoc(List<FARegister> list, bool isMassProcess)
		{
			return ReleaseDoc(list, isMassProcess, true);
		}

		public static DocumentList<Batch> ReleaseDoc(List<FARegister> list, bool isMassProcess, bool AutoPost)
		{
			bool failed = false;

			AssetProcess rg = CreateInstance<AssetProcess>();
			JournalEntry je = CreateInstance<JournalEntry>();
			PostGraph pg = CreateInstance<PostGraph>();

			List<int> batchbind = new List<int>();

			DocumentList<Batch> batchlist = new DocumentList<Batch>(rg);

			//list.Sort((a, b) => string.CompareOrdinal(a.FinPeriodID, b.FinPeriodID));

			for (int i = 0; i < list.Count; i++)
			{
				FARegister doc = list[i];
				try
				{
					rg.Clear();
					
					rg.ProcessAssetTran(je, doc, batchlist);

					if (je.BatchModule.Current != null && batchlist.Find(je.BatchModule.Current) == null)
					{
						batchlist.Add(je.BatchModule.Current);
						batchbind.Add(i);
					}

					if (isMassProcess)
					{
						PXProcessing<FARegister>.SetInfo(i, ActionsMessages.RecordProcessed);
					}
				}
				catch (Exception e)
				{
					batchlist.Remove(je.BatchModule.Current);
					je.Clear();
					if (isMassProcess)
					{
						PXProcessing<FARegister>.SetError(i, e);
						failed = true;
					}
					else
					{
						throw new PXMassProcessException(i, e);
					}
				}
			}

			for (int i = 0; i < batchlist.Count; i++)
			{
				Batch batch = batchlist[i];
				try
				{
					if (rg.AutoPost && AutoPost)
					{
						pg.Clear();
						pg.PostBatchProc(batch);
					}
				}
				catch (Exception e)
				{
					if (isMassProcess)
					{
						failed = true;
						PXProcessing<FARegister>.SetError(batchbind[i], e);
					}
					else
					{
						throw new PXMassProcessException(batchbind[i], e);
					}
				}
			}
			if (failed)
			{
				throw new PXException(GL.Messages.DocumentsNotReleased);
			}
			return rg.AutoPost ? batchlist : new DocumentList<Batch>(rg);
		}

		[Serializable]
		public partial class ReleaseFilter:IBqlTable
		{
			#region Origin
			public abstract class origin : IBqlField
			{
			}
			protected String _Origin;
			[PXDBString(1, IsFixed = true)]
			[FARegister.origin.List]
			[PXUIField(DisplayName = "Origin")]
			public virtual String Origin
			{
				get
				{
					return _Origin;
				}
				set
				{
					_Origin = value;
				}
			}
			#endregion
		}
	}

	public class AssetProcess : PXGraph<AssetProcess>
	{
		public PXSelect<FABookHist> bookhist;
		public PXSelect<FARegister> register;
		public PXSelect<FATran> booktran;
		public PXSelect<FixedAsset> fixedasset;
		public PXSelect<FABookBalance> bookbalance;
		public PXSelect<FADetails> fadetail;
		public PXSelect<FAAccrualTran> accrualtran;
		public PXSetup<FASetup> fasetup;
		public PXSetup<GLSetup> glsetup;

		public JournalEntry je = PXGraph.CreateInstance<JournalEntry>();

		public bool UpdateGL
		{
			get
			{
				return fasetup.Current.UpdateGL == true;
			}
		}

		public bool AutoPost
		{
			get
			{
				return fasetup.Current.AutoPost == true;
			}
		}

		public bool SummPost
		{
			get
			{
				return fasetup.Current.SummPost == true;
			}
		}

		public bool SummPostDepr
		{
			get
			{
				return fasetup.Current.SummPostDepreciation == true;
			}
		}

		public AssetProcess()
		{
			object record = fasetup.Select();
		}

		public override void Clear()
		{
			base.Clear();
			je.Clear();
		}

		public static void SuspendAsset(FixedAsset asset)
		{
			TransactionEntry docgraph = PXGraph.CreateInstance<TransactionEntry>();

			FADetails assetdet = PXSelect<FADetails, Where<FADetails.assetID, Equal<Required<FADetails.assetID>>>>.Select(docgraph, asset.AssetID);
			assetdet.Status = FixedAssetStatus.Suspended;

			docgraph.assetdetails.Update(assetdet);

			asset.Status = FixedAssetStatus.Suspended;
			asset.Suspended = true;

			docgraph.Asset.Update(asset);

			foreach (FABookBalance item in PXSelect<FABookBalance, Where<FABookBalance.assetID, Equal<Current<FixedAsset.assetID>>>>.SelectMultiBound(docgraph, new object[] { asset })
				.RowCast<FABookBalance>()
				.Where(item => item.Status != FixedAssetStatus.FullyDepreciated))
				{
					if (item.CurrDeprPeriod == null)
					{
						throw new PXException(Messages.CurrentDeprPeriodIsNull);
					}

					item.Status = FixedAssetStatus.Suspended;

					docgraph.bookbalances.Update(item);
				}
			docgraph.Actions.PressSave();
		}

		public static void UnsuspendAsset(FixedAsset asset, string CurrentPeriod)
		{
			TransactionEntry docgraph = CreateInstance<TransactionEntry>();

			FADetails assetdet = PXSelect<FADetails, Where<FADetails.assetID, Equal<Required<FADetails.assetID>>>>.Select(docgraph, asset.AssetID);
			assetdet.Status = FixedAssetStatus.Active;

			docgraph.assetdetails.Update(assetdet);

			asset.Status = FixedAssetStatus.Active;
			asset.Suspended = false;

			docgraph.Asset.Update(asset);

			using (PXTransactionScope ts = new PXTransactionScope())
			{
				docgraph.Actions.PressSave();

				FinPeriod currentperiod = PXSelect<FinPeriod, Where<FinPeriod.finPeriodID, Equal<Required<FinPeriod.finPeriodID>>, And<FinPeriod.fAClosed, Equal<False>>>>.SelectWindowed(docgraph, 0, 1, CurrentPeriod);

				if (currentperiod == null)
				{
					throw new PXException(GL.Messages.FiscalPeriodClosed, CurrentPeriod);
				}

				foreach (FABookBalance item in PXSelect<FABookBalance, Where<FABookBalance.assetID, Equal<Current<FixedAsset.assetID>>>>.SelectMultiBound(docgraph, new object[] { asset })
					.RowCast<FABookBalance>()
					.Where(item => item.Status == FixedAssetStatus.Suspended))
					{
						item.Status = FixedAssetStatus.Active;

						if (item.CurrDeprPeriod == null)
						{
							throw new PXException(Messages.CurrentDeprPeriodIsNull);
						}

						string CurrentBookPeriod = item.UpdateGL == true ? CurrentPeriod : FABookPeriodIDAttribute.PeriodFromDate(docgraph, currentperiod.StartDate, item.BookID);

					if (string.Equals(CurrentBookPeriod, item.CurrDeprPeriod))
						{
							docgraph.bookbalances.Update(item);
							docgraph.Actions.PressSave();
						}
						else
						{
							int? periods = FABookPeriodIDAttribute.PeriodMinusPeriod(docgraph, CurrentBookPeriod, item.CurrDeprPeriod, item.BookID);

							if (periods == null)
							{
								throw new PXException(Messages.CannotChangeCurrentPeriod, FABookPeriodIDAttribute.FormatForError(CurrentBookPeriod), FABookPeriodIDAttribute.FormatForError(item.CurrDeprPeriod));
							}

							List<FABookBalance> balances = new List<FABookBalance>();
							balances.Add(item);
							SuspendAssetForPeriods(balances, (int)periods);
						}
					}
				ts.Complete();
			}
		}

		public static void SuspendBalanceForPeriods(TransactionEntry graph, FABookBalance bookbal, int Periods)
		{
			if (Periods < 1)
			{
				return;
			}

			FABookHist hist = graph.bookhistory.Insert(new FABookHist
			{
				AssetID = bookbal.AssetID,
				BookID = bookbal.BookID,
				FinPeriodID = FABookPeriodIDAttribute.PeriodPlusPeriod(graph, bookbal.CurrDeprPeriod, -(int) bookbal.YtdSuspended, bookbal.BookID)
			});
			hist.YtdSuspended = Periods;

			for (int i = 0; i < Periods; i++)
			{
				hist = graph.bookhistory.Insert(new FABookHist
				{
					AssetID = bookbal.AssetID,
					BookID = bookbal.BookID,
					FinPeriodID = FABookPeriodIDAttribute.PeriodPlusPeriod(graph, bookbal.CurrDeprPeriod, i, bookbal.BookID)
				});
				hist.YtdReversed = (i == 0) ? Periods : 0;
				hist.Suspended = true;
				hist.Closed = true;
			}

			graph.bookhistory.Insert(new FABookHist
			{
				AssetID = bookbal.AssetID,
				BookID = bookbal.BookID,
				FinPeriodID = FABookPeriodIDAttribute.PeriodPlusPeriod(graph, bookbal.CurrDeprPeriod, Periods, bookbal.BookID)
			});

			FABookBalance copy = PXCache<FABookBalance>.CreateCopy(bookbal);
			if (!string.IsNullOrEmpty(copy.CurrDeprPeriod))
			{
				copy.CurrDeprPeriod = FABookPeriodIDAttribute.PeriodPlusPeriod(graph, copy.CurrDeprPeriod, Periods, copy.BookID);
			}
			if (!string.IsNullOrEmpty(copy.LastDeprPeriod))
			{
				copy.LastDeprPeriod = FABookPeriodIDAttribute.PeriodPlusPeriod(graph, copy.LastDeprPeriod, Periods, copy.BookID);
			}
			if (!string.IsNullOrEmpty(copy.DeprToPeriod))
			{
				copy.DeprToPeriod = FABookPeriodIDAttribute.PeriodPlusPeriod(graph, copy.DeprToPeriod, Periods, copy.BookID);
				copy.LastPeriod = copy.DeprToPeriod;
			}
			copy.DeprToDate = null;

			graph.bookbalances.Update(copy);
		}

		public static void SuspendAssetForPeriods(IEnumerable<FABookBalance> balances, int Periods)
		{
			TransactionEntry docgraph = CreateInstance<TransactionEntry>();

			foreach (FABookBalance bookbal in balances)
			{
				SuspendBalanceForPeriods(docgraph, bookbal, Periods);
			}

			docgraph.Actions.PressSave();
		}

		public static void SuspendAssetToPeriod(IEnumerable<FABookBalance> balances, DateTime? Date, string PeriodID)
		{
			TransactionEntry docgraph = CreateInstance<TransactionEntry>();
			FinPeriod periodTo = PXSelect<FinPeriod, Where<FinPeriod.finPeriodID, Equal<Required<FinPeriod.finPeriodID>>, And<FinPeriod.fAClosed, Equal<False>>>>.SelectWindowed(docgraph, 0, 1, PeriodID);
			if (periodTo == null)
			{
				throw new PXException(GL.Messages.FiscalPeriodClosed, PeriodID);
			}
			Date = Date ?? periodTo.StartDate;

			foreach (FABookBalance bookbal in balances)
			{
				if (bookbal.LastDeprPeriod != null && string.CompareOrdinal(bookbal.LastDeprPeriod, bookbal.DeprToPeriod) >= 0) continue;

				string BookPeriodID = bookbal.UpdateGL == true ? PeriodID : FABookPeriodIDAttribute.PeriodFromDate(docgraph, Date, bookbal.BookID);
				int Periods = FABookPeriodIDAttribute.PeriodMinusPeriod(docgraph, BookPeriodID, bookbal.CurrDeprPeriod, bookbal.BookID) ?? 0;
				SuspendBalanceForPeriods(docgraph, bookbal, Periods);
			}

			docgraph.Actions.PressSave();
		}

		public abstract class HashView<TView, T> : PXView
			where TView : PXView
		{
			public static TView GetView(PXGraph graph)
			{ 
				PXView view;
				string viewname = "_" + typeof(TView).Name + "_";
				if (!graph.Views.TryGetValue(viewname, out view))
				{
					graph.Views[viewname] = view = (PXView)Activator.CreateInstance(typeof(TView), graph);
				}
				return (TView)view;
			}

			protected HashSet<T> _set;

			protected HashView(PXGraph graph, BqlCommand select)
				: base(graph, true, select)
			{
				object[] list = graph.TypedViews.GetView(this.BqlSelect, true).SelectMulti().ToArray();
				Initialize(list);
			}

			protected abstract void Initialize(object[] list);
 
			public virtual bool Contains(T item)
			{
				return _set.Contains(item);
			}

			public override void Clear()
			{
			}
		}

		public class UnreleasedView : HashView<UnreleasedView, int?>
		{
			public UnreleasedView(PXGraph graph)
				: base(graph, new Select4<FATran, Where<FATran.released, Equal<False>>, Aggregate<GroupBy<FATran.assetID, GroupBy<FATran.bookID>>>>())
			{
			}

			protected override void Initialize(object[] list)
			{
				_set = new HashSet<int?>(Array.ConvertAll(list, a => ((FATran)a).AssetID));
			}
		}

		public static bool UnreleasedTransactionsExistsForAsset(PXGraph graph, Int32? assetID)
		{
			//it is to expensive to search for AssetID,BookID combo
			UnreleasedView unreleased = UnreleasedView.GetView(graph);

			return unreleased.Contains(assetID);
		}

		public static void CheckUnreleasedTransactions(PXGraph graph, Int32? assetID)
		{
			if (UnreleasedTransactionsExistsForAsset(graph, assetID))
			{
				throw new PXException(Messages.AssetHasUnreleasedTran);
			}
		}

		public static void CheckUnreleasedTransactions(PXGraph graph, FixedAsset asset)
		{
			UnreleasedView unreleased = UnreleasedView.GetView(graph);
			if (unreleased.Contains(asset.AssetID))
			{
				throw new PXException(Messages.AssetHasUnreleasedTran);
			}
		}

		public static void ThrowDisabled_Dispose(PXGraph graph, FixedAsset asset, FADetails det, FASetup fasetup, DateTime disposalDate, string disposalPeriodID, bool deprBeforeDisposal)
		{
			if (det.ReceiptDate != null && DateTime.Compare((DateTime)det.ReceiptDate, disposalDate) > 0)
			{
				throw new PXException(Messages.AcquisitionAfterDisposal);
			}

			if (det.IsReconciled == false && fasetup.ReconcileBeforeDisposal == true)
			{
				throw new PXException(Messages.CanNotDisposeUnreconciledAsset);
			}

			foreach (FABookBalance bookbal in PXSelectReadonly<FABookBalance, Where<FABookBalance.assetID, Equal<Required<FABookBalance.assetID>>>>.Select(graph, asset.AssetID))
			{
				string disposalPeriod = bookbal.UpdateGL == true ? disposalPeriodID : FABookPeriodIDAttribute.PeriodFromDate(graph, disposalDate, bookbal.BookID);

				CheckUnreleasedTransactions(graph, bookbal.AssetID);

				if (bookbal.LastDeprPeriod != bookbal.DeprToPeriod && bookbal.Depreciate == true && // live depreciable asset
					string.CompareOrdinal(disposalPeriod, bookbal.CurrDeprPeriod) > 0 && deprBeforeDisposal &&
					fasetup.AutoReleaseDepr != true) 
				{
					throw new PXException(Messages.AssetShouldBeDeprToPeriod, FinPeriodIDAttribute.FormatForError(string.Format("{0}", Math.Min(int.Parse(disposalPeriod), int.Parse(bookbal.DeprToPeriod)))));
				}
				if (string.CompareOrdinal(bookbal.LastDeprPeriod, disposalPeriod) > 0)
				{
					throw new PXException(Messages.AssetDisposedInPastPeriod);
				}
			}
		}

		public static void ThrowDisabled_Transfer(PXGraph graph, FixedAsset asset, FADetails det)
		{
			CheckUnreleasedTransactions(graph, asset);

			if (string.IsNullOrEmpty(det.TransferPeriod))
			{
				FABookBalance bookbal = PXSelectReadonly<FABookBalance, Where<FABookBalance.assetID, Equal<Required<FABookBalance.assetID>>>, OrderBy<Desc<FABookBalance.updateGL>>>.SelectWindowed(graph, 0, 1, asset.AssetID);
				if (bookbal != null && string.IsNullOrEmpty(bookbal.LastDeprPeriod) == false && bookbal.UpdateGL == true)
				{
					throw new PXException(GL.Messages.NoOpenPeriod);
				}
			}

		}

		protected static Dictionary<string, string> reversalTranType = new Dictionary<string, string>() 
		{ 
			{FATran.tranType.PurchasingPlus, FATran.tranType.PurchasingMinus},
			{FATran.tranType.PurchasingMinus, FATran.tranType.PurchasingPlus},
			{FATran.tranType.DepreciationPlus, FATran.tranType.DepreciationMinus},
			{FATran.tranType.DepreciationMinus, FATran.tranType.DepreciationPlus},
			{FATran.tranType.ReconciliationPlus, FATran.tranType.ReconciliationMinus},
			{FATran.tranType.ReconciliationMinus, FATran.tranType.ReconciliationPlus},
			{FATran.tranType.SalePlus, FATran.tranType.SaleMinus},
			{FATran.tranType.SaleMinus, FATran.tranType.SalePlus},
			{FATran.tranType.PurchasingReversal, FATran.tranType.PurchasingDisposal},
			{FATran.tranType.PurchasingDisposal, FATran.tranType.PurchasingReversal},
			{FATran.tranType.AdjustingDeprPlus, FATran.tranType.AdjustingDeprMinus},
			{FATran.tranType.AdjustingDeprMinus, FATran.tranType.AdjustingDeprPlus},
			{FATran.tranType.TransferPurchasing, FATran.tranType.TransferPurchasing},
			{FATran.tranType.TransferDepreciation, FATran.tranType.TransferDepreciation},
		};

		protected static Dictionary<string, string> disposalreversalTranType = new Dictionary<string, string>() 
		{ 
			{FATran.tranType.PurchasingDisposal, FATran.tranType.PurchasingReversal},
			{FATran.tranType.AdjustingDeprMinus, FATran.tranType.AdjustingDeprPlus},
			{FATran.tranType.SalePlus, FATran.tranType.SaleMinus},
			{FATran.tranType.DepreciationPlus, FATran.tranType.DepreciationMinus},
		};

		public static DocumentList<FARegister> ReverseDisposal(FixedAsset asset, DateTime? revDate, string revPeriodID)
		{
			TransactionEntry docgraph = CreateInstance<TransactionEntry>();
			FARegister doc = docgraph.Document.Insert(new FARegister()
														 {
															 BranchID = asset.BranchID,
															 Origin = FARegister.origin.DisposalReversal,
															 DocDesc = PXMessages.LocalizeFormatNoPrefix(Messages.DocDescDispReversal, asset.AssetCD)
														 });

			FARegister dispdoc = PXSelectReadonly2<FARegister, LeftJoin<FATran, On<FARegister.refNbr, Equal<FATran.refNbr>>>, Where<FATran.assetID, Equal<Current<FixedAsset.assetID>>, And<FARegister.origin, Equal<FARegister.origin.disposal>, And<FARegister.released, Equal<True>>>>, OrderBy<Desc<FARegister.docDate>>>.SelectSingleBound(docgraph, new object[] { asset });
			foreach (PXResult<FATran, FABook> res in PXSelectJoin<FATran, LeftJoin<FABook, On<FATran.bookID, Equal<FABook.bookID>>>,  Where<FATran.refNbr, Equal<Current<FARegister.refNbr>>>>.SelectMultiBound(docgraph, new object[]{dispdoc}))
			{
				FATran tran = res;
				FABook book = res;
				revPeriodID = revPeriodID ?? tran.FinPeriodID;
				if (book.UpdateGL != true)
			{
					revPeriodID = FABookPeriodIDAttribute.FABookPeriodFromDate(docgraph, revDate ?? tran.TranDate, tran.BookID).FinPeriodID;
				}
				FATran copy = new FATran
				{
					RefNbr = doc.RefNbr,
					LineNbr = null,
					DebitAccountID = tran.CreditAccountID,
					DebitSubID = tran.CreditSubID,
					CreditAccountID = tran.DebitAccountID,
					CreditSubID = tran.DebitSubID,
					TranDesc = PXMessages.LocalizeFormatNoPrefix(Messages.TranDescDispReversal, tran.TranDesc),
					Released = false,
					TranAmt = tran.TranAmt,
					RGOLAmt = tran.RGOLAmt,
					AssetID = tran.AssetID,
					BookID = tran.BookID,
					TranType = disposalreversalTranType[tran.TranType],
					TranDate = revDate ?? tran.TranDate,
					FinPeriodID = revPeriodID ?? tran.FinPeriodID,
				};
				copy = docgraph.Trans.Insert(copy);
			}
			if (docgraph.Trans.Cache.IsInsertedUpdatedDeleted)
			{
				docgraph.Actions.PressSave();
			}
			if (docgraph.fasetup.Current.AutoReleaseReversal == true)
			{
				PXLongOperation.StartOperation(docgraph, delegate { AssetTranRelease.ReleaseDoc(new List<FARegister> { doc }, false); });
			}
			return docgraph.created;
		}

		public static DocumentList<FARegister> ReverseAsset(FixedAsset asset, FASetup fasetup)
		{
			TransactionEntry docgraph = CreateInstance<TransactionEntry>();
			FARegister doc = docgraph.Document.Insert(new FARegister
															{
																BranchID = asset.BranchID,
																Origin = FARegister.origin.Reversal, 
																DocDesc = PXMessages.LocalizeFormatNoPrefix(Messages.DocDescReversal, asset.AssetCD)
															});
			foreach (FATran copy in PXSelect<FATran, Where<FATran.assetID, Equal<Current<FixedAsset.assetID>>, And<FATran.released, Equal<True>>>>
				.SelectMultiBound(docgraph, new object[]{asset})
				.RowCast<FATran>()
				.Select(tran => new FATran
								  {
									  RefNbr = doc.RefNbr,
									  LineNbr = null,
									  DebitAccountID = tran.CreditAccountID,
									  DebitSubID = tran.CreditSubID,
									  CreditAccountID = tran.DebitAccountID,
									  CreditSubID = tran.DebitSubID,
									  TranDesc = PXMessages.LocalizeFormatNoPrefix(Messages.TranDescReversal, tran.TranDesc),
									  Released = false,
									  TranAmt = tran.TranAmt, 
									  RGOLAmt = tran.TranAmt,
									  AssetID = tran.AssetID,
									  BookID = tran.BookID,
									  FinPeriodID = tran.FinPeriodID,
									  TranType = reversalTranType[tran.TranType],
									  GLTranID = tran.GLTranID,
									  BranchID = tran.IsTransfer ? tran.SrcBranchID : tran.BranchID,
									  SrcBranchID = tran.IsTransfer ? tran.BranchID : tran.SrcBranchID,
								  }))
			{
				docgraph.Trans.Insert(copy);
			}
			if (docgraph.Trans.Cache.IsInsertedUpdatedDeleted)
			{
				docgraph.Actions.PressSave();
			}
			if (docgraph.fasetup.Current.AutoReleaseReversal == true)
			{
				PXLongOperation.StartOperation(docgraph, () => AssetTranRelease.ReleaseDoc(new List<FARegister> {doc}, false));
			}
			return docgraph.created;
		}

		public static List<FABookBalance> PrepareDisposal(TransactionEntry docgraph, FixedAsset asset, FADetails assetdet, bool IsMassProcess, bool deprBeforeDisposal)
		{
			PXSelectBase<FABookBalance> bookbalSelect = new PXSelectReadonly<FABookBalance,
				Where<FABookBalance.assetID, Equal<Required<FixedAsset.assetID>>>>(docgraph);

			List<FABookBalance> books = new List<FABookBalance>();
			string disposalPeriod = assetdet.DisposalPeriodID ?? FinPeriodIDAttribute.PeriodFromDate(assetdet.DisposalDate);
			
			foreach (FABookBalance bookbal in bookbalSelect.Select(assetdet.AssetID))
			{
				if (bookbal.Depreciate != true || !deprBeforeDisposal)
				{
					SuspendAssetToPeriod(new List<FABookBalance>() { bookbal }, assetdet.DisposalDate, disposalPeriod);
				}

				//all previous reads should be readonly!! otherwise SuspendAsset changes would not be read.
				FABookBalance copy = PXSelect<FABookBalance, 
					Where<FABookBalance.assetID, Equal<Current<FABookBalance.assetID>>, 
						And<FABookBalance.bookID, Equal<Current<FABookBalance.bookID>>>>>
					.SelectSingleBound(docgraph, new object[] { bookbal });

				copy.DisposalPeriodID = (bookbal.UpdateGL == true && assetdet.DisposalPeriodID != null) 
					? assetdet.DisposalPeriodID 
					: FABookPeriodIDAttribute.PeriodFromDate(docgraph, assetdet.DisposalDate, copy.BookID);
				copy.OrigDeprToDate = copy.DeprToDate;
				copy.DisposalAmount = assetdet.SaleAmount;

				books.Add(docgraph.bookbalances.Update(copy));
			}

			docgraph.Actions.PressSave();
			docgraph.Clear();

			if (asset.Depreciable == true && deprBeforeDisposal)
			{
				DepreciateAsset(books, assetdet.DisposalDate, disposalPeriod, IsMassProcess, false);

				/// Rows should be refreshed, because some fields 
				/// can be affected by the depreciate process.
				/// Note, that non db fields should be filled again.
				/// 
				books = bookbalSelect.Select(assetdet.AssetID)
					.RowCast<FABookBalance>()
					.ToList();
				books.ForEach(bal => bal.DisposalAmount = assetdet.SaleAmount);
			}

			Dictionary<int?, FABookBalance> origin = SetDeprTo(books, assetdet.DisposalDate);
			CalculateAsset(books, null);
			ResetDeprTo(books, origin);

			return books;
		}

		protected static Dictionary<int?, FABookBalance> SetDeprTo(List<FABookBalance> books, DateTime? deprToDate)
		{
			Dictionary<int?, FABookBalance> origin = new Dictionary<int?, FABookBalance>();
			foreach (FABookBalance balance in books.Where(balance => balance != null && balance.BookID != null && string.CompareOrdinal(balance.DeprToPeriod, balance.DisposalPeriodID) > 0))
			{
				origin.Add(balance.BookID, new FABookBalance{DeprToDate = balance.DeprToDate, DeprToPeriod = balance.DeprToPeriod});
				balance.DeprToDate = deprToDate;
				balance.DeprToPeriod = balance.DisposalPeriodID;
			}
			return origin;
		}

		protected static void ResetDeprTo(List<FABookBalance> books, Dictionary<int?, FABookBalance> origin)
		{
			foreach (FABookBalance balance in books)
			{
				FABookBalance orig;
				if (origin.TryGetValue(balance.BookID, out orig))
				{
					balance.DeprToDate = orig.DeprToDate;
					balance.DeprToPeriod = orig.DeprToPeriod;
				}
			}
		}

		public static DocumentList<FARegister> DisposeAsset(PXResultset<FixedAsset, FADetails> assets, FASetup fasetup, bool IsMassProcess, bool deprBeforeDisposal, string reason)
		{
			TransactionEntry docgraph = CreateInstance<TransactionEntry>();
			foreach (PXResult<FixedAsset, FADetails> res in assets)
			{
				FixedAsset asset = res;
				FADetails assetdet = res;

				PXProcessing<FixedAsset>.SetCurrentItem(asset);
				try
				{
					ThrowDisabled_Dispose(docgraph, asset, assetdet, fasetup, (DateTime)assetdet.DisposalDate, assetdet.DisposalPeriodID, deprBeforeDisposal);

					List<FABookBalance> books = PrepareDisposal(docgraph, asset, assetdet, IsMassProcess, deprBeforeDisposal);
					DisposeAsset(docgraph, asset, books, fasetup, reason);

					PXProcessing<FixedAsset>.SetProcessed();
				}
				catch (Exception ex)
				{
					if (IsMassProcess)
					{
						PXProcessing<FixedAsset>.SetError(ex);
					}
					else
					{
						throw;
					}
				}
			}
			
			if (docgraph.fasetup.Current.AutoReleaseDisp == true && docgraph.created.Count > 0)
			{
				AssetTranRelease.ReleaseDoc(docgraph.created, IsMassProcess);
			}
			return docgraph.created;
		}

		public static void DisposeAsset(TransactionEntry docgraph, FixedAsset asset, IEnumerable<FABookBalance> books, FASetup setup, string reason)
		{
			FADetails det = PXSelect<FADetails, Where<FADetails.assetID, Equal<Current<FixedAsset.assetID>>>>.SelectSingleBound(docgraph, new object[] { asset });
			if(setup.SummPost == true)
			{
				TransactionEntry.SegregateRegister(docgraph, (int)asset.BranchID, FARegister.origin.Disposal, null, det.DisposalDate, "", docgraph.created);
			}
			else
			{
				FARegister doc = docgraph.Document.Insert(new FARegister() 
																{ 
																	Origin = FARegister.origin.Disposal, 
																	DocDate = det.DisposalDate, 
																	BranchID = asset.BranchID
																});
			}

			FARegister currdoc = docgraph.Document.Current;
			if(currdoc != null && string.IsNullOrEmpty(currdoc.Reason))
			{
				currdoc.Reason = reason;
				docgraph.Document.Update(currdoc);
			}
		   
			foreach (FABookBalance bal in books)
			{
				FABookBalance bookbal = PXSelect<FABookBalance, Where<FABookBalance.assetID, Equal<Current<FABookBalance.assetID>>, And<FABookBalance.bookID, Equal<Current<FABookBalance.bookID>>>>>.SelectSingleBound(docgraph, new object[] { bal });
				FABookPeriod bookperiod = PXSelect<FABookPeriod, Where<FABookPeriod.finPeriodID, Equal<Current<FABookBalance.deprToPeriod>>>>.SelectSingleBound(docgraph, new object[] { bookbal });
				if (bookbal.DeprToDate == null && bookperiod != null && bookperiod.EndDate != null)
				{
					bookbal.DeprToDate = bookperiod.EndDate.Value.AddDays(-1);
				}
				bookbal.DisposalAmount = bal.DisposalAmount;

				string period;
				if(bookbal.Depreciate == true)
				{
					period = string.CompareOrdinal(bookbal.DeprToPeriod, bookbal.DisposalPeriodID) > 0
						? bookbal.DisposalPeriodID
						: bookbal.DeprToPeriod;
					if (!string.IsNullOrEmpty(bookbal.LastPeriod) &&
						string.CompareOrdinal(bookbal.LastPeriod, bookbal.DeprToPeriod) > 0 &&
						string.CompareOrdinal(bookbal.LastPeriod, period) > 0)
					{
						period = bookbal.LastPeriod;
					}
				}
				else
				{
					period = bookbal.DisposalPeriodID;
				}
				FABookHistory hist = PXSelect<FABookHistory, Where<FABookHistory.assetID, Equal<Current<FABookBalance.assetID>>, And<FABookHistory.bookID, Equal<Current<FABookBalance.bookID>>, And<FABookHistory.finPeriodID, Equal<Required<FABookBalance.deprToPeriod>>>>>>.SelectSingleBound(docgraph, new object[]{bookbal}, period);

				if (hist == null)
				{
					throw new PXException();
				}
				if (string.CompareOrdinal(bookbal.LastDeprPeriod, bookbal.DisposalPeriodID) > 0)
				{
					throw new PXException(Messages.AssetDisposedInPastPeriod);
				}
				if (hist.Suspended == true)
				{
					throw new PXException(Messages.AssetDisposedInSuspendedPeriod);
				}

				{
					FATran tran = new FATran
					{
						AssetID = bookbal.AssetID,
						BookID = bookbal.BookID,
						TranDate = det.DisposalDate,
						FinPeriodID = bookbal.DisposalPeriodID,
						TranType = "A-",
						TranAmt = (hist.YtdTax179 - hist.YtdTax179Taken),
						MethodDesc = Messages.MethodDescTax179
					};
					tran.TranDesc = PXMessages.LocalizeFormatNoPrefix(Messages.TranDescDepreciationRecap, docgraph.Trans.GetValueExt<FATran.assetID>(tran));
					docgraph.Trans.Insert(tran);

					hist.YtdCalculated -= tran.TranAmt;
					hist.YtdDepreciated -= tran.TranAmt;
				}

				{
					FATran tran = new FATran
					{
						AssetID = bookbal.AssetID,
						BookID = bookbal.BookID,
						TranDate = det.DisposalDate,
						FinPeriodID = bookbal.DisposalPeriodID,
						TranType = "A-",
						TranAmt = (hist.YtdBonus - hist.YtdBonusTaken),
						MethodDesc = Messages.MethodDescBonus
					};
					tran.TranDesc = PXMessages.LocalizeFormatNoPrefix(Messages.TranDescDepreciationRecap, docgraph.Trans.GetValueExt<FATran.assetID>(tran));
					docgraph.Trans.Insert(tran);

					hist.YtdCalculated -= tran.TranAmt;
					hist.YtdDepreciated -= tran.TranAmt;
				}


				int? GainLossAcctID;
				int? GainLossSubID;

				decimal? RGOLAmt;
				if (setup.DepreciateInDisposalPeriod == true && bookbal.Status == FixedAssetStatus.Active)
				{
					RGOLAmt = bookbal.DisposalAmount - (hist.YtdAcquired - (bookbal.Depreciate == true ? hist.YtdCalculated : 0m));
				}
				else
				{
					RGOLAmt = bookbal.DisposalAmount - (hist.YtdAcquired - (bookbal.Depreciate == true ? hist.YtdDepreciated : 0m));
				}
					

				if (RGOLAmt > 0m)
				{
					GainLossAcctID = asset.GainAcctID;
					GainLossSubID = asset.GainSubID;
				}
				else
				{
					GainLossAcctID = asset.LossAcctID;
					GainLossSubID = asset.LossSubID;
				}

				#region Sect 179 Recap


				#endregion

				decimal? tranAmt = hist.YtdCalculated - hist.YtdDepreciated;
				bool isPositive = tranAmt > 0m;
				tranAmt = Math.Abs((decimal)tranAmt);
				if (setup.DepreciateInDisposalPeriod == true && bookbal.Status == FixedAssetStatus.Active && bookbal.Depreciate == true && tranAmt != 0m)
				{
					FATran tran = new FATran
					{
						AssetID = hist.AssetID,
						BookID = hist.BookID,
						TranDate = det.DisposalDate,
						FinPeriodID = bookbal.DisposalPeriodID,
						TranAmt = tranAmt
					};
					tran.TranType = isPositive
										? (bookbal.CurrDeprPeriod == tran.FinPeriodID
											   ? FATran.tranType.CalculatedPlus
											   : FATran.tranType.DepreciationPlus)
										: (bookbal.CurrDeprPeriod == tran.FinPeriodID
											   ? FATran.tranType.CalculatedMinus
											   : FATran.tranType.DepreciationMinus);
					tran.TranDesc = PXMessages.LocalizeFormatNoPrefix(Messages.TranDescDisposalAdj, docgraph.Trans.GetValueExt<FATran.assetID>(tran));

					docgraph.Trans.Insert(tran);
				}

				{
					FATran tran = new FATran
					{
						AssetID = bookbal.AssetID,
						BookID = bookbal.BookID,
						TranDate = det.DisposalDate,
						FinPeriodID = bookbal.DisposalPeriodID,
						TranType = "PD",
						CreditAccountID = asset.FAAccountID,
						CreditSubID = asset.FASubID,
						DebitAccountID = GainLossAcctID,
						DebitSubID = GainLossSubID,
						TranAmt = hist.YtdAcquired
					};
					tran.TranDesc = PXMessages.LocalizeFormatNoPrefix(Messages.TranDescCostDisposal, docgraph.Trans.GetValueExt<FATran.assetID>(tran));

					docgraph.Trans.Insert(tran);
				}

				if(bookbal.Depreciate == true)
				{
					FATran tran = new FATran
					{
						AssetID = bookbal.AssetID,
						BookID = bookbal.BookID,
						TranDate = det.DisposalDate,
						FinPeriodID = bookbal.DisposalPeriodID,
						TranType = "A-",
						DebitAccountID = asset.AccumulatedDepreciationAccountID,
						DebitSubID = asset.AccumulatedDepreciationSubID,
						ReclassificationOnDebitProhibited = true,
						CreditAccountID = GainLossAcctID,
						CreditSubID = GainLossSubID,
						TranAmt = docgraph.fasetup.Current.DepreciateInDisposalPeriod == true && hist.Closed != true? hist.YtdCalculated : hist.YtdDepreciated
					};
					tran.TranDesc = PXMessages.LocalizeFormatNoPrefix(Messages.TranDescDeprDisposal, docgraph.Trans.GetValueExt<FATran.assetID>(tran));

					docgraph.Trans.Insert(tran);
				}

				{
					FATran tran = new FATran
					{
						AssetID = bookbal.AssetID,
						BookID = bookbal.BookID,
						TranDate = det.DisposalDate,
						FinPeriodID = bookbal.DisposalPeriodID,
						TranType = "S+"
					};

					if (bookbal.DisposalAmount >= 0m)
					{
						tran.DebitAccountID = asset.DisposalAccountID;
						tran.DebitSubID = asset.DisposalSubID;
						tran.CreditAccountID = GainLossAcctID;
						tran.CreditSubID = GainLossSubID;
					}
					else
					{
						tran.DebitAccountID = GainLossAcctID;
						tran.DebitSubID = GainLossSubID;
						tran.CreditAccountID = asset.DisposalAccountID;
						tran.CreditSubID = asset.DisposalSubID;
					}

					tran.TranAmt = Math.Abs((decimal)bookbal.DisposalAmount);
					tran.RGOLAmt = RGOLAmt;
					tran.TranDesc = PXMessages.LocalizeFormatNoPrefix(Messages.TranDescSale, docgraph.Trans.GetValueExt<FATran.assetID>(tran));

					docgraph.Trans.Insert(tran);
				}
			}

			if (docgraph.Trans.Cache.IsInsertedUpdatedDeleted)
			{
				docgraph.Actions.PressSave();
			}
		}

		public static DocumentList<FARegister> SplitAsset(FixedAsset asset, DateTime? splitDate, string splitPeriodID, bool deprBeforeSplit, Dictionary<FixedAsset, decimal> splits)
		{
			TransactionEntry docgraph = CreateInstance<TransactionEntry>();
			FinPeriod splitPeriod = PXSelect<FinPeriod, Where<FinPeriod.finPeriodID, Equal<Required<FinPeriod.finPeriodID>>, And<FinPeriod.fAClosed, Equal<False>>>>.SelectWindowed(docgraph, 0, 1, splitPeriodID);
			if (splitPeriod == null)
			{
				throw new PXException(GL.Messages.FiscalPeriodClosed, splitPeriodID);
			}
			splitDate = splitDate ?? splitPeriod.StartDate;

			FARegister doc = docgraph.Document.Insert(new FARegister
															{ 
																BranchID = asset.BranchID,
																Origin = FARegister.origin.Split, 
																DocDesc = PXMessages.LocalizeFormatNoPrefix(Messages.TranDescSplit, asset.AssetCD) 
															});
			string desc = PXMessages.LocalizeFormatNoPrefix(Messages.TranDescSplit, asset.AssetCD);
			foreach (KeyValuePair<FixedAsset, decimal> split in splits)
			{
				foreach (FABookBalance sbal in PXSelect<FABookBalance, Where<FABookBalance.assetID, Equal<Required<FixedAsset.assetID>>>>.Select(docgraph, split.Key.AssetID))
				{
					FABookBalance bal = PXSelect<FABookBalance, Where<FABookBalance.assetID, Equal<Required<FixedAsset.assetID>>, And<FABookBalance.bookID, Equal<Required<FABook.bookID>>>>>.Select(docgraph, asset.AssetID, sbal.BookID);
					string tranPeriod = bal.UpdateGL == true ? splitPeriod.FinPeriodID : FABookPeriodIDAttribute.PeriodFromDate(docgraph, splitDate, bal.BookID);

					if(deprBeforeSplit)
					{
						DepreciateAsset(new List<FABookBalance>() { bal }, splitDate, tranPeriod, false, false);
					}
					else
					{
						SuspendAssetToPeriod(new List<FABookBalance>() { bal }, splitDate, tranPeriod);
						CalculateAsset(new List<FABookBalance>() { bal }, null);
					}

					// reselect after suspend/depreciation
					bal = PXSelectReadonly<FABookBalance, Where<FABookBalance.assetID, Equal<Required<FixedAsset.assetID>>, And<FABookBalance.bookID, Equal<Required<FABook.bookID>>>>>.Select(docgraph, asset.AssetID, sbal.BookID);
					sbal.CurrDeprPeriod = bal.CurrDeprPeriod;
					sbal.LastDeprPeriod = bal.LastDeprPeriod;
					sbal.LastPeriod = bal.LastPeriod;
					sbal.DeprToPeriod = bal.DeprToPeriod;
					docgraph.bookbalances.Update(sbal);

					int? prev_ytdsuspened = 0;
					int? prev_ytdreversed = 0;
					decimal? rounding = sbal.AcquisitionCost;

					FATran prev_tran = null;
					FATran prev_tran2 = null;

					string lastPeriod = null;
					if (string.IsNullOrEmpty(bal.DeprToPeriod))
					{
						lastPeriod = PXSelect<FABookHistory, 
							Where<FABookHistory.assetID, Equal<Required<FABookBalance.assetID>>, 
								And<FABookHistory.bookID, Equal<Required<FABookBalance.bookID>>>>, 
							OrderBy<Desc<FABookHistory.finPeriodID>>>
							.Select(docgraph, bal.AssetID, bal.BookID)
							.RowCast<FABookHistory>()
							.FirstOrDefault()
							.With(h => h.FinPeriodID);
					}
					foreach (FABookHist item in PXSelectReadonly<FABookHist, 
						Where<FABookHist.assetID, Equal<Required<FABookHist.assetID>>, 
							And<FABookHist.bookID, Equal<Required<FABookHist.bookID>>, 
							And<FABookHist.finPeriodID, LessEqual<Required<FABookHist.finPeriodID>>>>>, 
						OrderBy<Asc<FABookHist.finPeriodID>>>.Select(docgraph, bal.AssetID, bal.BookID, lastPeriod ?? bal.DeprToPeriod))
					{
						if (!string.IsNullOrEmpty(bal.LastDeprPeriod) &&
							string.CompareOrdinal(item.FinPeriodID, bal.CurrDeprPeriod) <= 0)
						{
							FABookHist newhist = docgraph.bookhistory.Insert(new FABookHist
							{
								AssetID = sbal.AssetID,
								BookID = sbal.BookID,
								FinPeriodID = item.FinPeriodID
							});

							newhist.Suspended = item.Suspended;
							newhist.Closed = item.Closed;
							newhist.YtdSuspended = item.YtdSuspended;
							newhist.YtdReversed = item.YtdReversed;

							newhist.YtdSuspended -= prev_ytdsuspened;
							newhist.YtdReversed -= prev_ytdreversed;

							prev_ytdsuspened = item.YtdSuspended;
							prev_ytdreversed = item.YtdReversed;
						}

						if (item.PtdDeprBase != 0m)
						{
							FATran tran = new FATran
							{
								AssetID = bal.AssetID,
								BookID = bal.BookID,
								TranType = FATran.tranType.PurchasingMinus,
								TranDate = splitDate,
								FinPeriodID = splitPeriod.FinPeriodID,
								TranPeriodID = item.FinPeriodID,
								TranAmt = PXCurrencyAttribute.BaseRound(docgraph, item.PtdDeprBase * split.Value / 100m)
							};

							prev_tran = docgraph.Trans.Insert(tran);

							tran = new FATran
							{
								AssetID = sbal.AssetID,
								BookID = sbal.BookID,
								TranType = FATran.tranType.PurchasingPlus,
								TranDate = splitDate,
								FinPeriodID = splitPeriod.FinPeriodID,
								TranPeriodID = item.FinPeriodID,
								TranAmt = PXCurrencyAttribute.BaseRound(docgraph, item.PtdDeprBase * split.Value / 100m),
								TranDesc = desc
							};

							prev_tran2 = docgraph.Trans.Insert(tran);

							rounding -= tran.TranAmt;
						}
					}

					if (rounding != 0m && prev_tran != null && prev_tran2 != null)
					{
						prev_tran.TranAmt += rounding;
						prev_tran2.TranAmt += rounding;
					}

					docgraph.Trans.Insert(new FATran
					{
						AssetID = asset.AssetID,
						BookID = bal.BookID,
						TranDate = splitDate,
						FinPeriodID = tranPeriod,
						TranType = FATran.tranType.ReconciliationMinus,
						TranAmt = PXDBCurrencyAttribute.BaseRound(docgraph, (decimal)(bal.YtdReconciled * split.Value / 100m)),
						TranDesc = desc
					});
					docgraph.Trans.Insert(new FATran
					{
						AssetID = asset.AssetID,
						BookID = bal.BookID,
						TranDate = splitDate,
						FinPeriodID = tranPeriod,
						TranType = FATran.tranType.AdjustingDeprMinus,
						TranAmt = PXDBCurrencyAttribute.BaseRound(docgraph, (decimal)(bal.YtdDepreciated * split.Value / 100m)),
						TranDesc = desc,
						CreditAccountID = asset.AccumulatedDepreciationAccountID,
						CreditSubID = asset.AccumulatedDepreciationSubID,
						ReclassificationOnCreditProhibited = true,
						DebitAccountID = asset.AccumulatedDepreciationAccountID,
						DebitSubID = asset.AccumulatedDepreciationSubID,
						ReclassificationOnDebitProhibited = true
					});
					docgraph.Trans.Insert(new FATran
					{
						AssetID = split.Key.AssetID,
						BookID = sbal.BookID,
						TranDate = splitDate,
						FinPeriodID = tranPeriod,
						TranType = FATran.tranType.ReconciliationPlus,
						TranAmt = PXDBCurrencyAttribute.BaseRound(docgraph, (decimal)(bal.YtdReconciled * split.Value / 100m)),
						TranDesc = desc
					});
					docgraph.Trans.Insert(new FATran
					{
						AssetID = split.Key.AssetID,
						BookID = sbal.BookID,
						TranDate = splitDate,
						FinPeriodID = tranPeriod,
						TranType = FATran.tranType.AdjustingDeprPlus,
						TranAmt = PXDBCurrencyAttribute.BaseRound(docgraph, (decimal)(bal.YtdDepreciated * split.Value / 100m)),
						TranDesc = desc,
						CreditAccountID = asset.AccumulatedDepreciationAccountID,
						CreditSubID = asset.AccumulatedDepreciationSubID,
						ReclassificationOnCreditProhibited = true,
						DebitAccountID = split.Key.AccumulatedDepreciationAccountID,
						DebitSubID = split.Key.AccumulatedDepreciationSubID,
						ReclassificationOnDebitProhibited = true
					});
				}
			}

			DocumentList<Batch> batchlist = new DocumentList<Batch>(docgraph);
			using (PXTransactionScope ts = new PXTransactionScope())
			{

				if (docgraph.Trans.Cache.IsInsertedUpdatedDeleted)
				{
					docgraph.Actions.PressSave();
				}
				if (docgraph.fasetup.Current.AutoReleaseSplit == true && docgraph.created.Count > 0)
				{
					batchlist = AssetTranRelease.ReleaseDoc(docgraph.created, false, false);
				}
				ts.Complete(docgraph);
			}

			PostGraph pg = CreateInstance<PostGraph>();
			foreach (Batch batch in batchlist)
			{
				pg.Clear();
				pg.PostBatchProc(batch);
			}

			return docgraph.created;
		}

		public static void AcquireAsset(TransactionEntry docgraph, int BranchID, IDictionary<FABookBalance, OperableTran> booktrn, FARegister register)
		{
			if (register != null)
			{
				docgraph.Document.Current = register;
			}
			else
			{
				TransactionEntry.SegregateRegister(docgraph, BranchID, FARegister.origin.Purchasing, null, null, "", docgraph.created);
			}

			foreach (KeyValuePair<FABookBalance, OperableTran> book in booktrn)
			{
				FixedAsset asset = PXSelect<FixedAsset, Where<FixedAsset.assetID, Equal<Required<FixedAsset.assetID>>>>.Select(docgraph, book.Key.AssetID);
				FADetails details = PXSelect<FADetails, Where<FADetails.assetID, Equal<Required<FixedAsset.assetID>>>>.Select(docgraph, book.Key.AssetID);

				if (book.Value.Op == PXDBOperation.Update && !docgraph.UpdateGL)
				{
					PXDatabase.Delete<FATran>(
						new PXDataFieldRestrict<FATran.assetID>(book.Key.AssetID),
						new PXDataFieldRestrict<FATran.bookID>(book.Key.BookID));
					PXDatabase.Delete<FABookHistory>(
						new PXDataFieldRestrict<FABookHistory.assetID>(book.Key.AssetID), 
						new PXDataFieldRestrict<FABookHistory.bookID>(book.Key.BookID));
					
					book.Value.Op = PXDBOperation.Insert;
					docgraph.Document.Update(docgraph.Document.Current);
				}

				if (string.IsNullOrEmpty(book.Key.LastDeprPeriod))
				{
					if (book.Value.Op == PXDBOperation.Delete || book.Value.Op == PXDBOperation.Update)
					{
						PXDatabase.Delete<FABookHistory>(
							new PXDataFieldRestrict<FABookHistory.assetID>(book.Key.AssetID),
							new PXDataFieldRestrict<FABookHistory.bookID>(book.Key.BookID));
					}
					if ((book.Value.Op == PXDBOperation.Insert && asset.SplittedFrom == null) || book.Value.Op == PXDBOperation.Update)
					{
						FABookHist hist = new FABookHist
						{
							AssetID = book.Key.AssetID,
							BookID = book.Key.BookID,
							FinPeriodID = book.Key.DeprFromPeriod
						};

						hist = docgraph.bookhistory.Insert(hist);

						FABookBalance bookBal = docgraph.bookbalances.SelectSingle(book.Key.AssetID, book.Key.BookID);
						bookBal.CurrDeprPeriod = book.Key.DeprFromPeriod;
						docgraph.bookbalances.Update(bookBal);
					}
				}

				switch (book.Value.Op)
				{
					case PXDBOperation.Insert:
						if(asset.SplittedFrom != null)
						{
							continue;
						}

						bool initMode = !string.IsNullOrEmpty(book.Key.LastDeprPeriod) || (!docgraph.UpdateGL && book.Key.Depreciate != true);
						{
							FATran tran = new FATran
							{
								AssetID = book.Key.AssetID,
								BookID = book.Key.BookID,
								TranDate = details.ReceiptDate,
								TranAmt = book.Key.AcquisitionCost,
								TranType = FATran.tranType.PurchasingPlus,
								Released = initMode
							};
							tran.TranDesc = PXMessages.LocalizeFormatNoPrefix(Messages.TranDescPurchase, docgraph.Trans.GetValueExt<FATran.assetID>(tran));
							docgraph.Trans.Insert(tran);
						}
						if (initMode)
						{
							if (!string.IsNullOrEmpty(book.Key.LastDeprPeriod))
							{
								{
									FATran tran = new FATran
									{
										AssetID = book.Key.AssetID,
										BookID = book.Key.BookID,
										TranDate = book.Key.DeprFromDate,
										FinPeriodID = book.Key.DeprFromPeriod,
										TranAmt = book.Key.Tax179Amount,
										TranType = FATran.tranType.DepreciationPlus,
										MethodDesc = Messages.MethodDescTax179,
										Released = true
									};
									tran.TranDesc = PXMessages.LocalizeFormatNoPrefix(Messages.TranDescDepreciation,
									docgraph.Trans.GetValueExt<FATran.assetID>(tran));
									docgraph.Trans.Insert(tran);
								}

								{
									FATran tran = new FATran
									{
										AssetID = book.Key.AssetID,
										BookID = book.Key.BookID,
										TranDate = book.Key.DeprFromDate,
										FinPeriodID = book.Key.DeprFromPeriod,
										TranAmt = book.Key.BonusAmount,
										TranType = FATran.tranType.DepreciationPlus,
										MethodDesc = Messages.MethodDescBonus,
										Released = true
									};
									tran.TranDesc = PXMessages.LocalizeFormatNoPrefix(Messages.TranDescDepreciation,
									docgraph.Trans.GetValueExt<FATran.assetID>(tran));
									docgraph.Trans.Insert(tran);
								}

								{
									FATran tran = new FATran
									{
										AssetID = book.Key.AssetID,
										BookID = book.Key.BookID,
										TranDate = FABookPeriodIDAttribute.PeriodEndDate(docgraph, book.Key.LastDeprPeriod, book.Key.BookID),
										FinPeriodID = book.Key.LastDeprPeriod,
										TranAmt = book.Key.YtdDepreciated - (book.Key.Tax179Amount + book.Key.BonusAmount),
										TranType = FATran.tranType.DepreciationPlus,
										Released = true
									};
									tran.TranDesc = PXMessages.LocalizeFormatNoPrefix(Messages.TranDescDepreciation,
									docgraph.Trans.GetValueExt<FATran.assetID>(tran));
									docgraph.Trans.Insert(tran);
								}
							}

							if (!docgraph.UpdateGL)
							{
								{
									FATran tran = new FATran
									{
										AssetID = book.Key.AssetID,
										BookID = book.Key.BookID,
										TranDate = details.ReceiptDate,
										TranAmt = book.Key.AcquisitionCost,
										TranType = FATran.tranType.ReconciliationPlus,
										Released = true
									};
									docgraph.Trans.Insert(tran);
								}
							}


							docgraph.Document.Current.Released = true;

							{
								FABookHist hist = new FABookHist
								{
									AssetID = book.Key.AssetID,
									BookID = book.Key.BookID,
									FinPeriodID = book.Key.DeprFromPeriod
								};

								hist = docgraph.bookhistory.Insert(hist);

								hist.PtdAcquired += book.Key.AcquisitionCost;
								hist.YtdAcquired += book.Key.AcquisitionCost;
								hist.PtdDeprBase += book.Key.AcquisitionCost;
								hist.YtdDeprBase += book.Key.AcquisitionCost;
								hist.PtdDepreciated += book.Key.Tax179Amount + book.Key.BonusAmount;
								hist.YtdDepreciated += book.Key.Tax179Amount + book.Key.BonusAmount;

								hist.YtdBal += book.Key.AcquisitionCost - (book.Key.Tax179Amount + book.Key.BonusAmount);

								hist.PtdTax179 += book.Key.Tax179Amount;
								hist.YtdTax179 += book.Key.Tax179Amount;

								hist.PtdBonus += book.Key.BonusAmount;
								hist.YtdBonus += book.Key.BonusAmount;
								hist.Closed = true;

								if (docgraph.fasetup.Current.UpdateGL != true)
								{
									hist.PtdReconciled += book.Key.AcquisitionCost;
									hist.YtdReconciled += book.Key.AcquisitionCost;
								}
							}

							KeyValuePair<FABookBalance, OperableTran> b = book;
							foreach (FABookHist hist in PXSelect<FABookPeriod, Where<FABookPeriod.bookID, Equal<Required<FABookPeriod.bookID>>, 
								And<FABookPeriod.finPeriodID, Greater<Required<FABookPeriod.finPeriodID>>,
								And<FABookPeriod.finPeriodID, Less<Required<FABookPeriod.finPeriodID>>,
								And<FABookPeriod.endDate, Greater<FABookPeriod.startDate>>>>>>.Select(docgraph, book.Key.BookID, book.Key.DeprFromPeriod, book.Key.LastDeprPeriod)
									.RowCast<FABookPeriod>()
									.Select(per => new FABookHist
									{
										AssetID = b.Key.AssetID,
										BookID = b.Key.BookID,
										FinPeriodID = per.FinPeriodID
									}).Select(hist => docgraph.bookhistory.Insert(hist)))
							{
								hist.Closed = true;
							}

							if (!string.IsNullOrEmpty(book.Key.LastDeprPeriod))
							{
								{
									FABookHist hist = new FABookHist
									{
										AssetID = book.Key.AssetID,
										BookID = book.Key.BookID,
										FinPeriodID = book.Key.LastDeprPeriod
									};

									hist = docgraph.bookhistory.Insert(hist);

									hist.PtdDepreciated += book.Key.YtdDepreciated - (book.Key.Tax179Amount + book.Key.BonusAmount);
									hist.YtdDepreciated += book.Key.YtdDepreciated - (book.Key.Tax179Amount + book.Key.BonusAmount);
									hist.YtdBal -= book.Key.YtdDepreciated - (book.Key.Tax179Amount + book.Key.BonusAmount);
									hist.Closed = true;
								}

								if (string.CompareOrdinal(book.Key.LastDeprPeriod, book.Key.DeprToPeriod) < 0)
								{
									FABookHist hist = new FABookHist
									{
										AssetID = book.Key.AssetID,
										BookID = book.Key.BookID,
										FinPeriodID = FABookPeriodIDAttribute.PeriodPlusPeriod(docgraph, book.Key.LastDeprPeriod, 1, book.Key.BookID)
									};

									hist = docgraph.bookhistory.Insert(hist);

									hist.Closed = false;

									FABookBalance bookBal = docgraph.bookbalances.SelectSingle(book.Key.AssetID, book.Key.BookID);
									bookBal.CurrDeprPeriod = FABookPeriodIDAttribute.PeriodPlusPeriod(docgraph, book.Key.LastDeprPeriod, 1, book.Key.BookID);
									bookBal.InitPeriod = book.Key.DeprFromPeriod;
									bookBal.LastPeriod = book.Key.DeprToPeriod;
									docgraph.bookbalances.Update(bookBal);
								}

								if (string.CompareOrdinal(book.Key.LastDeprPeriod, book.Key.DeprToPeriod) == 0)
								{
									FABookBalance bookBal = docgraph.bookbalances.SelectSingle(book.Key.AssetID, book.Key.BookID);
									bookBal.Status = FixedAssetStatus.FullyDepreciated;
									bookBal.CurrDeprPeriod = null;
									bookBal.InitPeriod = book.Key.DeprFromPeriod;
									bookBal.LastPeriod = book.Key.DeprToPeriod;
									docgraph.bookbalances.Update(bookBal);
								}
							}
						}

						break;
					case PXDBOperation.Update:
						FATran copy = PXCache<FATran>.CreateCopy(book.Value.Tran);
						copy.TranDate = details.ReceiptDate;
						docgraph.Trans.Cache.SetDefaultExt<FATran.finPeriodID>(copy);
						copy.TranAmt = book.Key.AcquisitionCost;
						docgraph.Trans.Update(copy);
						break;
					case PXDBOperation.Delete:
						docgraph.Trans.Delete(book.Value.Tran);
						break;
				}
			}

			if (docgraph.Trans.Cache.IsInsertedUpdatedDeleted)
			{
				docgraph.Actions.PressSave();
			}
		}

		public static void CalculateAsset(IEnumerable<FABookBalance> books, string maxPeriodID)
		{
			DepreciationCalculation calc = CreateInstance<DepreciationCalculation>();

			foreach (FABookBalance bookbal in books)
			{
				calc.Clear();
				PXProcessing<FABookBalance>.SetCurrentItem(bookbal);
				try
				{
					calc.CalculateDepreciation(bookbal, maxPeriodID);
					PXProcessing<FABookBalance>.SetProcessed();
				}
				catch (PXException ex)
				{
					PXProcessing<FABookBalance>.SetError(ex);
				}
			}
		}

		public static void DepreciateAsset(IEnumerable<FABookBalance> books, DateTime? DateTo, string PeriodTo, bool IsMassProcess)
		{
			DepreciateAsset(books, DateTo, PeriodTo, IsMassProcess, true);
		}

		public static void DepreciateAsset(IEnumerable<FABookBalance> books, DateTime? DateTo, string PeriodTo, bool IsMassProcess, bool IncludeLastPeriod)
		{
			TransactionEntry docgraph = PXGraph.CreateInstance<TransactionEntry>();
			DepreciationCalculation calc = CreateInstance<DepreciationCalculation>();

			FinPeriod periodTo = (FinPeriod)PXSelect<FinPeriod, Where<FinPeriod.finPeriodID, Equal<Required<FinPeriod.finPeriodID>>, 
				And<FinPeriod.fAClosed, Equal<False>>>>.SelectWindowed(docgraph, 0, 1, PeriodTo);
			if (periodTo == null)
			{
				throw new PXException(GL.Messages.FiscalPeriodClosed, PeriodTo);
			}

			foreach (FABookBalance item in books)
			{
				PXProcessing<FABookBalance>.SetCurrentItem(item);

				try
				{
					using (PXTransactionScope ts = new PXTransactionScope())
					{
						FABookBalance bookbal = PXSelect<FABookBalance, Where<FABookBalance.assetID, Equal<Current<FABookBalance.assetID>>, 
							And<FABookBalance.bookID, Equal<Current<FABookBalance.bookID>>>>>.SelectSingleBound(docgraph, new object[] { item });

						calc.Clear();
						calc.CalculateDepreciation(bookbal, PeriodTo);

						string BookPeriodTo = bookbal.UpdateGL != true && DateTo != null ?
							FABookPeriodIDAttribute.PeriodFromDate(docgraph, DateTo, bookbal.BookID) :
							PeriodTo;

						if (!IncludeLastPeriod)
						{
							BookPeriodTo = FABookPeriodIDAttribute.PeriodPlusPeriod(docgraph, BookPeriodTo, -1, bookbal.BookID);
						}
						foreach (PXResult<FABookHistory, FABookPeriod, FixedAsset> res in PXSelectJoin<FABookHistory, InnerJoin<FABookPeriod,
							On<FABookPeriod.bookID, Equal<FABookHistory.bookID>,
								And<FABookPeriod.finPeriodID, Equal<FABookHistory.finPeriodID>>>,
							LeftJoin<FixedAsset, On<FixedAsset.assetID, Equal<FABookHistory.assetID>>>>,
							Where<FABookHistory.assetID, Equal<Current<FABookBalance.assetID>>,
								And<FABookHistory.bookID, Equal<Current<FABookBalance.bookID>>,
								And<FABookHistory.finPeriodID, GreaterEqual<Required<FABookHistory.finPeriodID>>,
								And<FABookHistory.finPeriodID, LessEqual<Required<FABookHistory.finPeriodID>>,
								And<FABookHistory.closed, NotEqual<True>,
								And<FABookPeriod.startDate, NotEqual<FABookPeriod.endDate>>>>>>>,
								OrderBy<Asc<FABookHistory.finPeriodID>>>.SelectMultiBound(docgraph, new object[] { bookbal }, bookbal.CurrDeprPeriod, BookPeriodTo))
						{
							FABookHistory hist = res;
							FABookPeriod period = res;
							FixedAsset asset = res;

							TransactionEntry.SegregateRegister(docgraph, (int)asset.BranchID, FARegister.origin.Depreciation, period.FinPeriodID, null, null, docgraph.created);

							FATran prev_tran;
							{
								FATran tran = new FATran();
								tran.AssetID = hist.AssetID;
								tran.BookID = hist.BookID;
								tran.TranDate = period.EndDate.Value.AddDays(-1);
								tran.FinPeriodID = period.FinPeriodID;
								tran.TranAmt = (hist.FinPeriodID == bookbal.CurrDeprPeriod) ? hist.YtdCalculated - hist.YtdDepreciated : hist.PtdCalculated + hist.PtdAdjusted + hist.PtdDeprDisposed;
								tran.TranType = "C+";
								tran.TranDesc = PXMessages.LocalizeFormatNoPrefix(Messages.TranDescDepreciation, docgraph.Trans.GetValueExt<FATran.assetID>(tran));

								prev_tran = docgraph.Trans.Insert(tran);
							}

							if (hist.FinPeriodID == bookbal.DeprFromPeriod && (hist.YtdTax179Calculated - hist.YtdTax179) > 0m)
							{
								prev_tran.TranAmt -= (hist.YtdTax179Calculated - hist.YtdTax179);

								FATran tran = new FATran();
								tran.AssetID = hist.AssetID;
								tran.BookID = hist.BookID;
								tran.TranDate = period.EndDate.Value.AddDays(-1);
								tran.FinPeriodID = period.FinPeriodID;
								tran.TranAmt = bookbal.Tax179Amount;
								tran.TranType = "C+";
								tran.MethodDesc = Messages.MethodDescTax179;
								tran.TranDesc = PXMessages.LocalizeFormatNoPrefix(Messages.TranDescDepreciation, docgraph.Trans.GetValueExt<FATran.assetID>(tran));

								docgraph.Trans.Insert(tran);
							}

							if (hist.FinPeriodID == bookbal.DeprFromPeriod && (hist.YtdBonusCalculated - hist.YtdBonus) > 0m)
							{
								prev_tran.TranAmt -= (hist.YtdBonusCalculated - hist.YtdBonus);

								FATran tran = new FATran();
								tran.AssetID = hist.AssetID;
								tran.BookID = hist.BookID;
								tran.TranDate = period.EndDate.Value.AddDays(-1);
								tran.FinPeriodID = period.FinPeriodID;
								tran.TranAmt = bookbal.BonusAmount;
								tran.TranType = "C+";
								tran.MethodDesc = Messages.MethodDescBonus;
								tran.TranDesc = PXMessages.LocalizeFormatNoPrefix(Messages.TranDescDepreciation, docgraph.Trans.GetValueExt<FATran.assetID>(tran));

								docgraph.Trans.Insert(tran);
							}
						}
						if (docgraph.Trans.Cache.IsInsertedUpdatedDeleted)
						{
							docgraph.Actions.PressSave();
							docgraph.Clear(); // prevent OutOfMemoryException, clear cloned attributes
						}
						ts.Complete();
					}
					PXProcessing<FABookBalance>.SetProcessed();
				}
				catch (Exception ex)
				{
					PXProcessing<FABookBalance>.SetError(ex);
				}
			}

			if (docgraph.fasetup.Current.AutoReleaseDepr == true && docgraph.created.Count > 0)
			{
				docgraph.created.Sort((a, b) => string.Compare(a.FinPeriodID, b.FinPeriodID, StringComparison.Ordinal));
				AssetTranRelease.ReleaseDoc(docgraph.created, IsMassProcess); 
			}
		}

		public static void SetLastDeprPeriod(PXSelectBase<FABookBalance> bookBalances, FABookBalance bookBal, string LastDeprPeriod)
		{
			if (LastDeprPeriod == null) return;
			LastDeprPeriod = string.CompareOrdinal(LastDeprPeriod, bookBal.DeprToPeriod) > 0 ? bookBal.DeprToPeriod : LastDeprPeriod;
			bookBal = (FABookBalance)bookBalances.Cache.Locate(bookBal) ?? bookBal;
			if (string.CompareOrdinal(bookBal.LastDeprPeriod, LastDeprPeriod) < 0)
			{
				bookBal.LastDeprPeriod = LastDeprPeriod;
				bookBal.CurrDeprPeriod = string.CompareOrdinal(bookBal.LastDeprPeriod, bookBal.DeprToPeriod) < 0 ? FABookPeriodIDAttribute.PeriodPlusPeriod(bookBalances.Cache.Graph, bookBal.LastDeprPeriod, 1, bookBal.BookID) : null;
				bookBalances.Update(bookBal);
			}
		}

		public virtual void ProcessAssetTran(JournalEntry je, FARegister doc, DocumentList<Batch> created)
		{
			if (doc == null) return;

			if(fasetup.Current.UpdateGL != true && doc.Origin != FARegister.origin.Purchasing)
			{
				throw new PXException(Messages.CannotReleaseInInitializeMode);
			}

			using (PXTransactionScope ts = new PXTransactionScope())
			{
				foreach (PXResult<FATran, FixedAsset, FADetails, FABook, FABookBalance, FABookHist> res in PXSelectJoin<FATran, InnerJoin<FixedAsset, On<FixedAsset.assetID, Equal<FATran.assetID>>, 
																																InnerJoin<FADetails, On<FADetails.assetID, Equal<FixedAsset.assetID>>, 
																																InnerJoin<FABook, On<FABook.bookID, Equal<FATran.bookID>>, 
																																InnerJoin<FABookBalance, On<FABookBalance.assetID, Equal<FATran.assetID>, 
																																	And<FABookBalance.bookID, Equal<FATran.bookID>>>, 
																																LeftJoin<FABookHist, On<FABookHist.assetID, Equal<FATran.assetID>, 
																																	And<FABookHist.bookID, Equal<FATran.bookID>, 
																																	And<FABookHist.finPeriodID, Equal<FATran.finPeriodID>>>>>>>>>, 
																																Where<FATran.refNbr, Equal<Required<FARegister.refNbr>>, 
																																	And<FATran.released, Equal<False>>>>.Select(this, doc.RefNbr))
				{
					FATran fatran = res;
					FixedAsset asset = res;
					FADetails det = res;
					FABook book = res;
					FABookBalance bookbal = res;
					FABookHist posthist = res;

					if (det.Hold == true)
					{
						throw new PXException(Messages.TranPostedOnHold);
					}

					if (det.Status == FixedAssetStatus.Disposed && fatran.Origin == FARegister.origin.Reversal)
					{
						throw new PXException(Messages.CantReverseDisposedAsset, asset.AssetCD);
					}

					if (det.Status != FixedAssetStatus.Disposed && fatran.Origin == FARegister.origin.DisposalReversal)
					{
						throw new PXException(Messages.CantReverseDisposal, asset.AssetCD, new FixedAssetStatus.ListAttribute().ValueLabelDic[det.Status]);
					}
					
					if (posthist.Suspended == true && fatran.TranType != "P+" && fatran.TranType != "P-")
					{
						throw new PXException(Messages.TranPostedToSuspendedPeriod, booktran.Cache.GetValueExt<FATran.bookID>(fatran));
					}

					FADepreciationMethod method = PXSelect<FADepreciationMethod, Where<FADepreciationMethod.methodID, Equal<Required<FADepreciationMethod.methodID>>>>.Select(this, bookbal.DepreciationMethodID);
					if ((fatran.TranType == FATran.tranType.CalculatedPlus || fatran.TranType == FATran.tranType.CalculatedMinus) && method == null)
					{
						throw new PXException(Messages.DepreciationMethodDoesNotExist);
					}

					FixedAsset cls = PXSelect<FixedAsset, Where<FixedAsset.assetID, Equal<Required<FixedAsset.classID>>>>.Select(this, asset.ClassID);

					if (doc.Origin == FARegister.origin.DisposalReversal)
					{
						// DONE: Call CloseFABookHistory(toPeriod)
						this.CloseFABookHistory(bookbal, fatran.FinPeriodID);
					}

					PXResultset<FABookPeriod> closedPeriods = new PXResultset<FABookPeriod>();
					if (doc.Origin == FARegister.origin.Transfer && !string.IsNullOrEmpty(bookbal.CurrDeprPeriod))
					{
						closedPeriods.AddRange(PXSelect<FABookPeriod, Where<FABookPeriod.bookID, Equal<Required<FABookPeriod.bookID>>,
																   And<FABookPeriod.finPeriodID, GreaterEqual<Required<FABookPeriod.finPeriodID>>,
																   And<FABookPeriod.finPeriodID, Less<Required<FABookPeriod.finPeriodID>>,
																   And<FABookPeriod.endDate, Greater<FABookPeriod.startDate>>>>>>.Select(this, fatran.BookID, bookbal.CurrDeprPeriod, fatran.FinPeriodID));
					}
					if (doc.Origin == FARegister.origin.Transfer && string.IsNullOrEmpty(bookbal.CurrDeprPeriod) && !string.IsNullOrEmpty(bookbal.LastDeprPeriod))
					{
						closedPeriods.AddRange(PXSelect<FABookPeriod, Where<FABookPeriod.bookID, Equal<Required<FABookPeriod.bookID>>,
																   And<FABookPeriod.finPeriodID, GreaterEqual<Required<FABookPeriod.finPeriodID>>,
																   And<FABookPeriod.finPeriodID, Less<Required<FABookPeriod.finPeriodID>>,
																   And<FABookPeriod.endDate, Greater<FABookPeriod.startDate>>>>>>.Select(this, fatran.BookID, bookbal.LastDeprPeriod, fatran.FinPeriodID));
					}
					if (doc.Origin == FARegister.origin.Split && fatran.TranType == FATran.tranType.DepreciationPlus)
					{
						closedPeriods.AddRange(PXSelect<FABookPeriod, Where<FABookPeriod.bookID, Equal<Required<FABookPeriod.bookID>>, 
																   And<FABookPeriod.finPeriodID, GreaterEqual<Required<FABookPeriod.finPeriodID>>, 
																   And<FABookPeriod.finPeriodID, Less<Required<FABookPeriod.finPeriodID>>, 
																   And<FABookPeriod.endDate, Greater<FABookPeriod.startDate>>>>>>.Select(this, fatran.BookID, bookbal.DeprFromPeriod, fatran.FinPeriodID));
					}
					if ((fatran.TranType == FATran.tranType.PurchasingDisposal || (doc.Origin == FARegister.origin.Split && (fatran.TranType == FATran.tranType.DepreciationPlus || fatran.TranType == FATran.tranType.DepreciationMinus))) && string.Compare(fatran.FinPeriodID, bookbal.DeprToPeriod) > 0 && bookbal.Status == FixedAssetStatus.FullyDepreciated)
					{
						closedPeriods.AddRange(PXSelect<FABookPeriod, Where<FABookPeriod.bookID, Equal<Required<FABookPeriod.bookID>>,
																   And<FABookPeriod.finPeriodID, Greater<Required<FABookPeriod.finPeriodID>>,
																   And<FABookPeriod.finPeriodID, LessEqual<Required<FABookPeriod.finPeriodID>>,
																   And<FABookPeriod.endDate, Greater<FABookPeriod.startDate>>>>>>.Select(this, fatran.BookID, bookbal.DeprToPeriod, fatran.FinPeriodID));
					}
					if (fatran.TranType == FATran.tranType.PurchasingReversal)
					{
						FATran disptran = PXSelect<FATran, Where<FATran.assetID, Equal<Current<FATran.assetID>>, And<FATran.bookID, Equal<Current<FATran.bookID>>, And<FATran.tranType, Equal<FATran.tranType.purchasingDisposal>, And<FATran.finPeriodID, LessEqual<Current<FATran.finPeriodID>>>>>>, OrderBy<Desc<FATran.finPeriodID>>>.SelectSingleBound(this, new object[] { fatran });
						closedPeriods.AddRange(PXSelect<FABookPeriod, Where<FABookPeriod.bookID, Equal<Required<FABookPeriod.bookID>>,
																   And<FABookPeriod.finPeriodID, Greater<Required<FABookPeriod.finPeriodID>>,
																   And<FABookPeriod.finPeriodID, Less<Required<FABookPeriod.finPeriodID>>,
																   And<FABookPeriod.endDate, Greater<FABookPeriod.startDate>>>>>>.Select(this, fatran.BookID, disptran != null ? disptran.FinPeriodID : fatran.FinPeriodID, fatran.FinPeriodID));
					}
					if ((fatran.TranType == FATran.tranType.CalculatedPlus || fatran.TranType == FATran.tranType.CalculatedMinus) &&
						bookbal.Status == FixedAssetStatus.FullyDepreciated && method.IsPureStraightLine)
					{
						closedPeriods.AddRange(PXSelect<FABookPeriod, Where<FABookPeriod.bookID, Equal<Required<FABookPeriod.bookID>>,
																   And<FABookPeriod.finPeriodID, Greater<Required<FABookPeriod.finPeriodID>>,
																   And<FABookPeriod.finPeriodID, Less<Required<FABookPeriod.finPeriodID>>,
																   And<FABookPeriod.endDate, Greater<FABookPeriod.startDate>>>>>>.Select(this, fatran.BookID, bookbal.DeprToPeriod, fatran.FinPeriodID));
					}

					if (closedPeriods != null)
					{
						// DONE: Call CloseFABookHistory(periods)
						string maxClosedPeriod = this.CloseFABookHistory(bookbal, closedPeriods.RowCast<FABookPeriod>().Select(p => p.FinPeriodID));
						SetLastDeprPeriod(bookbalance, bookbal, maxClosedPeriod);
					}

					if (doc.Origin == FARegister.origin.Reversal)
					{
						if (book.UpdateGL == true)
						{
							FinPeriod glperiod = PXSelect<FinPeriod, Where<FinPeriod.fAClosed, NotEqual<True>,
											And<FinPeriod.startDate, NotEqual<FinPeriod.endDate>,
											And<FinPeriod.finPeriodID, GreaterEqual<Required<GLTran.finPeriodID>>>>>,
										 OrderBy<Asc<FinPeriod.finPeriodID>>>.Select(this, fatran.FinPeriodID);
							if (glperiod == null)
							{
								throw new PXException(GL.Messages.NoOpenPeriod);
							}
							if (string.CompareOrdinal(glperiod.FinPeriodID, fatran.FinPeriodID) > 0)
							{
								fatran.FinPeriodID = glperiod.FinPeriodID;
							}
						}
					}
					else if (fatran.TranType == "P+" || fatran.TranType == "P-" ||
						fatran.TranType == "R+" || fatran.TranType == "R-")
					{
						if (book.UpdateGL == true)
						{
							FinPeriod period = PXSelect<FinPeriod, Where<FinPeriod.fAClosed, NotEqual<True>,
																		And<FinPeriod.startDate, NotEqual<FinPeriod.endDate>,
																		And<FinPeriod.finPeriodID, GreaterEqual<Current<FATran.finPeriodID>>>>>,
																	 OrderBy<Asc<FinPeriod.finPeriodID>>>.SelectSingleBound(this, new object[] { fatran });
							if (period == null)
							{
								throw new PXException(GL.Messages.NoOpenPeriod);
							}
							fatran.FinPeriodID = period.FinPeriodID;
						}
					}

					if (fatran.TranType == "R+" || fatran.TranType == "R-")
					{
						if(string.CompareOrdinal(fatran.TranPeriodID, bookbal.DeprFromPeriod) < 0)
						{
							fatran.TranPeriodID = bookbal.DeprFromPeriod;
						}
						if (!string.IsNullOrEmpty(bookbal.DeprToPeriod) && string.CompareOrdinal(fatran.TranPeriodID, bookbal.DeprToPeriod) > 0)
						{
							fatran.TranPeriodID = bookbal.DeprToPeriod;
						}
					}

					if (fatran.TranType == "P+" || fatran.TranType == "P-")
					{
						if (doc.Origin != FARegister.origin.Split && DepreciationCalculation.UseAcceleratedDepreciation(cls, method))
						{
							if (string.CompareOrdinal(fatran.TranPeriodID, bookbal.CurrDeprPeriod) < 0)
							{
								fatran.TranPeriodID = bookbal.CurrDeprPeriod;
							}
							else if (book.UpdateGL == true)
							{
								FinPeriod period = PXSelect<FinPeriod, Where<FinPeriod.finPeriodID, GreaterEqual<Required<FinPeriod.finPeriodID>>, 
									And<FinPeriod.fAClosed, Equal<False>,
									And<FinPeriod.startDate, NotEqual<FinPeriod.endDate>>>>>.Select(this, fatran.TranPeriodID);
								if (period == null)
								{
									throw new PXException(GL.Messages.NoOpenPeriod);
								}

								if (string.CompareOrdinal(fatran.TranPeriodID, period.FinPeriodID) < 0)
								{
									fatran.TranPeriodID = period.FinPeriodID;
								}
							}
						}

						if (string.CompareOrdinal(fatran.TranPeriodID, bookbal.DeprFromPeriod) < 0)
						{
							fatran.TranPeriodID = bookbal.DeprFromPeriod;
						}

						string maxClosedPeriod = null;
						foreach (PXResult<FABookPeriod, FinPeriod> p in PXSelectJoin<FABookPeriod, 
																		LeftJoin<FinPeriod,
																			On<FABookPeriod.finPeriodID, Equal<FinPeriod.finPeriodID>>>,
																		Where<FABookPeriod.bookID, Equal<Required<FATran.bookID>>,
																			And<FABookPeriod.startDate, NotEqual<FABookPeriod.endDate>,
																			And<FABookPeriod.finPeriodID, GreaterEqual<Required<FABookBalance.deprFromPeriod>>,
																			And<FABookPeriod.finPeriodID, LessEqual<Required<FATran.finPeriodID>>>>>>>.Select(this, fatran.BookID, bookbal.DeprFromPeriod, fatran.FinPeriodID))
						{
							FABookPeriod bookperiod = p;
							FinPeriod finperiod = p;

							FABookHist createdHist = new FABookHist
							{
								AssetID = fatran.AssetID,
								BookID = fatran.BookID,
								FinPeriodID = bookperiod.FinPeriodID
							};
							createdHist = bookhist.Insert(createdHist);
							createdHist.Closed = finperiod.FAClosed == true;

							if ((maxClosedPeriod == null || (string.CompareOrdinal(bookperiod.FinPeriodID, maxClosedPeriod)) > 0 ) && createdHist.Closed == true)
							{
								maxClosedPeriod = bookperiod.FinPeriodID;
							}
						}
						SetLastDeprPeriod(bookbalance, bookbal, maxClosedPeriod);
					}

					switch (fatran.TranType)
					{
						case "P+":
							if (!fatran.IsOriginReversal || fatran.DebitAccountID == null)
							{
								fatran.DebitAccountID = asset.FAAccountID;
								fatran.DebitSubID = asset.FASubID;
							}
							if (!fatran.IsOriginReversal || fatran.CreditAccountID == null)
							{
								fatran.CreditAccountID = asset.FAAccrualAcctID;
								fatran.CreditSubID = asset.FAAccrualSubID;
							}

							break;

						case "PR":
							if (fatran.DebitAccountID == null)
							{
								fatran.DebitAccountID = asset.FAAccountID;
								fatran.DebitSubID = asset.FASubID;
							}

							if (fatran.CreditAccountID == null)
							{
								fatran.CreditAccountID = asset.FAAccrualAcctID;
								fatran.CreditSubID = asset.FAAccrualSubID;
							}
							break;

						case "P-":
							if (!fatran.IsOriginReversal || fatran.CreditAccountID == null)
							{
								fatran.CreditAccountID = asset.FAAccountID;
								fatran.CreditSubID = asset.FASubID;
							}
							if (!fatran.IsOriginReversal || fatran.DebitAccountID == null)
							{
								fatran.DebitAccountID = asset.FAAccrualAcctID;
								fatran.DebitSubID = asset.FAAccrualSubID;
							}
							break;

						case "PD":
							if (fatran.CreditAccountID == null)
							{
								fatran.CreditAccountID = asset.FAAccountID;
								fatran.CreditSubID = asset.FASubID;
							}

							if (fatran.DebitAccountID == null)
							{
								throw new PXException(ErrorMessages.FieldIsEmpty, typeof(FATran.creditAccountID).Name);
							}
							break;

						case "R+":
							if (!fatran.IsOriginReversal || fatran.DebitAccountID == null)
							{
								fatran.DebitAccountID = asset.FAAccrualAcctID;
								fatran.DebitSubID = asset.FAAccrualSubID;
							}
							if (fatran.CreditAccountID == null)
							{
								fatran.CreditAccountID = asset.FAAccrualAcctID;
								fatran.CreditSubID = asset.FAAccrualSubID;
							}

							break;
						case "R-":
							if (!fatran.IsOriginReversal || fatran.CreditAccountID == null)
							{
								fatran.CreditAccountID = asset.FAAccrualAcctID;
								fatran.CreditSubID = asset.FAAccrualSubID;
							}
							if (fatran.DebitAccountID == null)
							{
								fatran.DebitAccountID = asset.FAAccrualAcctID;
								fatran.DebitSubID = asset.FAAccrualSubID;
							}

							break;
						case "C+":
						case "D+":
						case "A+":
							if (fatran.CreditAccountID == null)
							{
								fatran.CreditAccountID = asset.AccumulatedDepreciationAccountID;
								fatran.CreditSubID = asset.AccumulatedDepreciationSubID;
							}
							if (fatran.DebitAccountID == null)
							{
								fatran.DebitAccountID = asset.DepreciatedExpenseAccountID;
								fatran.DebitSubID = asset.DepreciatedExpenseSubID;
							}
							break;
						case "C-":
						case "D-":
						case "A-":
							if (fatran.DebitAccountID == null)
							{
								fatran.DebitAccountID = asset.AccumulatedDepreciationAccountID;
								fatran.DebitSubID = asset.AccumulatedDepreciationSubID;
							}
							if (fatran.CreditAccountID == null)
							{
								fatran.CreditAccountID = asset.DepreciatedExpenseAccountID;
								fatran.CreditSubID = asset.DepreciatedExpenseSubID;
							}
							break;
						case "S+":
							if (fatran.DebitAccountID == null)
							{
								throw new PXException(ErrorMessages.FieldIsEmpty, typeof (FATran.debitAccountID).Name);
							}
							if (fatran.CreditAccountID == null)
							{
								throw new PXException(ErrorMessages.FieldIsEmpty, typeof(FATran.creditAccountID).Name);
							}
							break;
						case "S-":
							if (fatran.DebitAccountID == null)
							{
								throw new PXException(ErrorMessages.FieldIsEmpty, typeof(FATran.debitAccountID).Name);
							}

							if (fatran.CreditAccountID == null)
							{
								throw new PXException(ErrorMessages.FieldIsEmpty, typeof(FATran.creditAccountID).Name);
							}
							break;
						case "TD":
							if(!fatran.IsOriginReversal)
								fatran.TranAmt = bookbal.YtdDepreciated;
							asset = (FixedAsset)fixedasset.Cache.Locate(asset) ?? asset;
							asset.AccumulatedDepreciationAccountID = fatran.CreditAccountID;
							asset.AccumulatedDepreciationSubID = fatran.CreditSubID;
							fixedasset.Update(asset);
							break;
						case "TP":
							if (!fatran.IsOriginReversal)
								fatran.TranAmt = bookbal.YtdAcquired;
							asset = (FixedAsset)fixedasset.Cache.Locate(asset) ?? asset;
							asset.FAAccountID = fatran.DebitAccountID;
							asset.FASubID = fatran.DebitSubID;
							asset.BranchID = fatran.BranchID;
							fixedasset.Update(asset);
							break;
					}

					FABookHist hist = new FABookHist
						{
							AssetID = fatran.AssetID,
							BookID = fatran.BookID,
							FinPeriodID = fatran.FinPeriodID
						};

					hist = bookhist.Insert(hist);

					switch (fatran.TranType)
					{
						case "P+":
							hist.PtdAcquired += fatran.TranAmt;
							hist.YtdAcquired += fatran.TranAmt;
							hist.YtdBal += fatran.TranAmt;
							break;
						case "P-":
							hist.PtdAcquired -= fatran.TranAmt;
							hist.YtdAcquired -= fatran.TranAmt;
							hist.YtdBal -= fatran.TranAmt;
							break;
						case "PR":
							hist.YtdBal += fatran.TranAmt;
							break;
						case "PD":
							hist.YtdBal -= fatran.TranAmt;
							break;
						case "C+":
							if (string.IsNullOrEmpty(bookbal.CurrDeprPeriod) && string.IsNullOrEmpty(bookbal.LastDeprPeriod) || !string.IsNullOrEmpty(bookbal.CurrDeprPeriod) && !string.Equals(bookbal.CurrDeprPeriod, hist.FinPeriodID))
							{
								throw new PXException(Messages.CalculatedDepreciationPostedFuturePeriod);
							}
							fatran.TranType = "D+";
							switch (fatran.MethodDesc)
							{
								case Messages.MethodDescTax179:
									hist.PtdTax179 += fatran.TranAmt;
									hist.YtdTax179 += fatran.TranAmt;
									break;
								case Messages.MethodDescBonus:
									hist.PtdBonus += fatran.TranAmt;
									hist.YtdBonus += fatran.TranAmt;
									break;
							}
							hist.PtdDepreciated += fatran.TranAmt;
							hist.YtdDepreciated += fatran.TranAmt;
							hist.YtdBal -= fatran.TranAmt;
							hist.Closed = true;
							if(bookbal.Status == FixedAssetStatus.FullyDepreciated && method.IsPureStraightLine)
							{
								hist.PtdCalculated += fatran.TranAmt;
								hist.YtdCalculated += fatran.TranAmt;
							}
							break;
						case "A+":
							if (fatran.Origin == FARegister.origin.DisposalReversal)
							{
								hist.PtdDeprDisposed += fatran.TranAmt;
							}
							else
							{
								hist.PtdAdjusted += fatran.TranAmt;
							}
							hist.YtdDepreciated += fatran.TranAmt;
							hist.YtdBal -= fatran.TranAmt;
							break;
						case "D+":
							if (string.IsNullOrEmpty(bookbal.CurrDeprPeriod) && string.IsNullOrEmpty(bookbal.LastDeprPeriod) 
								|| !string.IsNullOrEmpty(bookbal.CurrDeprPeriod) && string.CompareOrdinal(bookbal.CurrDeprPeriod, hist.FinPeriodID) <= 0 && !fatran.IsOriginReversal)
							{
								throw new PXException(Messages.DepreciationAdjustmentPostedOpenPeriod);
							}
							switch (fatran.MethodDesc)
							{
								case Messages.MethodDescTax179:
									hist.PtdTax179Recap -= fatran.TranAmt;
									hist.YtdTax179Recap -= fatran.TranAmt;
									break;
								case Messages.MethodDescBonus:
									hist.PtdBonusRecap -= fatran.TranAmt;
									hist.YtdBonusRecap -= fatran.TranAmt;
									break;
							}
							hist.PtdDepreciated += fatran.TranAmt;
							hist.YtdDepreciated += fatran.TranAmt;
							hist.YtdBal -= fatran.TranAmt;
							break;
						case "C-":
							if (string.IsNullOrEmpty(bookbal.CurrDeprPeriod) && string.IsNullOrEmpty(bookbal.LastDeprPeriod) || !string.IsNullOrEmpty(bookbal.CurrDeprPeriod) && !string.Equals(bookbal.CurrDeprPeriod, hist.FinPeriodID))
							{
								throw new PXException(Messages.CalculatedDepreciationPostedFuturePeriod);
							}
							fatran.TranType = "D-";
							switch (fatran.MethodDesc)
							{
								case Messages.MethodDescTax179:
									hist.PtdTax179 -= fatran.TranAmt;
									hist.YtdTax179 -= fatran.TranAmt;
									break;
								case Messages.MethodDescBonus:
									hist.PtdBonus -= fatran.TranAmt;
									hist.YtdBonus -= fatran.TranAmt;
									break;
							}
							hist.PtdDepreciated -= fatran.TranAmt;
							hist.YtdDepreciated -= fatran.TranAmt;
							hist.YtdBal += fatran.TranAmt;
							hist.Closed = true;
							if (bookbal.Status == FixedAssetStatus.FullyDepreciated && method.IsPureStraightLine)
							{
								hist.PtdCalculated -= fatran.TranAmt;
								hist.YtdCalculated -= fatran.TranAmt;
							}
							break;
						case "A-":
							if (fatran.Origin == FARegister.origin.Disposal)
							{
								hist.PtdDeprDisposed -= fatran.TranAmt;
							}
							else
							{
								hist.PtdAdjusted -= fatran.TranAmt;
							}
							hist.YtdDepreciated -= fatran.TranAmt;
							hist.YtdBal += fatran.TranAmt;
							break;
						case "D-":
							if (string.IsNullOrEmpty(bookbal.CurrDeprPeriod) && string.IsNullOrEmpty(bookbal.LastDeprPeriod)
								|| !string.IsNullOrEmpty(bookbal.CurrDeprPeriod) && string.CompareOrdinal(bookbal.CurrDeprPeriod, hist.FinPeriodID) <= 0 && !fatran.IsOriginReversal)
							{
								throw new PXException(Messages.DepreciationAdjustmentPostedOpenPeriod);
							}
							if (fatran.Origin == FARegister.origin.DisposalReversal)
							{
								FABookHistory h = PXSelectReadonly<FABookHistory,
									Where<FABookHistory.assetID, Equal<Required<FABookHistory.assetID>>,
										And<FABookHistory.bookID, Equal<Required<FABookHistory.bookID>>,
										And<FABookHistory.finPeriodID, Equal<Required<FABookHistory.finPeriodID>>>>>>.Select(this, hist.AssetID, hist.BookID, hist.FinPeriodID);
								if (h.PtdDepreciated == fatran.TranAmt)
								{
									hist.Reopen = true;
									hist.Closed = false;
								}
							}

							switch (fatran.MethodDesc)
							{
								case Messages.MethodDescTax179:
									hist.PtdTax179Recap += fatran.TranAmt;
									hist.YtdTax179Recap += fatran.TranAmt;
									break;
								case Messages.MethodDescBonus:
									hist.PtdBonusRecap += fatran.TranAmt;
									hist.YtdBonusRecap += fatran.TranAmt;
									break;
							}
							hist.PtdDepreciated -= fatran.TranAmt;
							hist.YtdDepreciated -= fatran.TranAmt;
							hist.YtdBal += fatran.TranAmt;
							break;
						case "S+":
							hist.PtdDisposalAmount += fatran.TranAmt;
							hist.YtdDisposalAmount += fatran.TranAmt;

							hist.PtdRGOL += fatran.RGOLAmt;
							hist.YtdRGOL += fatran.RGOLAmt;
							break;
						case "S-":
							hist.PtdDisposalAmount -= fatran.TranAmt;
							hist.YtdDisposalAmount -= fatran.TranAmt;

							hist.PtdRGOL -= fatran.RGOLAmt;
							hist.YtdRGOL -= fatran.RGOLAmt;
							break;
						case "TP":
						case "TD":
							if (string.CompareOrdinal(fatran.FinPeriodID, bookbal.DeprToPeriod) > 0)
							{
								//do not create open history outside depreciation schedule of the asset
								bookhist.Cache.SetStatus(hist, PXEntryStatus.Notchanged);
							}
							break;
					}

					hist = new FABookHist
						{
							AssetID = fatran.AssetID,
							BookID = fatran.BookID,
							FinPeriodID = fatran.TranPeriodID
						};
					hist = bookhist.Insert(hist);

					switch (fatran.TranType)
					{
						case "P+":
							hist.PtdDeprBase += fatran.TranAmt;
							hist.YtdDeprBase += fatran.TranAmt;
							break;
						case "P-":
							hist.PtdDeprBase -= fatran.TranAmt;
							hist.YtdDeprBase -= fatran.TranAmt;
							break;
						case "R+":
							hist.PtdReconciled += fatran.TranAmt;
							hist.YtdReconciled += fatran.TranAmt;
							break;
						case "R-":
							hist.PtdReconciled -= fatran.TranAmt;
							hist.YtdReconciled -= fatran.TranAmt;
							break;
						case "TP":
						case "TD":
							if (!string.IsNullOrEmpty(bookbal.DeprToPeriod) && string.CompareOrdinal(fatran.TranPeriodID, bookbal.DeprToPeriod) > 0)
							{
								if (!string.IsNullOrEmpty(bookbal.CurrDeprPeriod))
								{
									throw new PXException(Messages.ActiveAssetTransferedPastDeprToPeriod, FinPeriodIDAttribute.FormatForError(bookbal.DeprToPeriod));
								}
								//do not create open history outside depreciation schedule of the asset
								bookhist.Cache.SetStatus(hist, PXEntryStatus.Notchanged);
							}
							if (!fatran.IsOriginReversal)
							{
								if (!string.IsNullOrEmpty(bookbal.CurrDeprPeriod) && string.CompareOrdinal(bookbal.CurrDeprPeriod, fatran.TranPeriodID) > 0)
								{
									throw new PXException(Messages.ActiveAssetTransferedBeforePeriod, FinPeriodIDAttribute.FormatForError(bookbal.CurrDeprPeriod));
								}
								if (!string.IsNullOrEmpty(bookbal.LastDeprPeriod) && string.CompareOrdinal(bookbal.LastDeprPeriod, fatran.TranPeriodID) >= 0)
								{
									throw new PXException(Messages.FullyDepreciatedAssetTransferedBeforePeriod, FinPeriodIDAttribute.FormatForError(bookbal.LastDeprPeriod));
								}
							}
							break;
					}

					PXSelectBase<FABookBalance> cmd = new PXSelect<FABookBalance, Where<FABookBalance.assetID, Equal<Required<FABookBalance.assetID>>, 
						And<FABookBalance.status, NotEqual<Required<FABookBalance.status>>>>>(this); 
					switch (doc.Origin)
					{
						case FARegister.origin.Purchasing:
							bookbal = (FABookBalance)bookbalance.Cache.Locate(bookbal) ?? bookbal;
							
							bookbal.InitPeriod = bookbal.DeprFromPeriod;
							bookbal.LastPeriod = bookbal.DeprToPeriod;

							if ((fatran.TranType == FATran.tranType.DepreciationMinus || fatran.TranType == FATran.tranType.DepreciationPlus)
								&& string.CompareOrdinal(fatran.FinPeriodID, bookbal.LastPeriod) > 0)
							{
								bookbal.LastPeriod = fatran.FinPeriodID;
								bookbal.LastDeprPeriod = fatran.FinPeriodID;
							}
							
							bookbalance.Update(bookbal);
							break;
						case FARegister.origin.Disposal:
							bookbal = (FABookBalance) bookbalance.Cache.Locate(bookbal) ?? bookbal;
							bookbal.CurrDeprPeriod = null;
							bookbal.Status = FixedAssetStatus.Disposed;
							if (fatran.TranType == FATran.tranType.DepreciationPlus)
							{
								bookbal.LastDeprPeriod = fatran.FinPeriodID;
							}
							if (string.CompareOrdinal(bookbal.DeprToPeriod, bookbal.DisposalPeriodID) > 0)
							{
								bookbal.DeprToDate = det.DisposalDate;
								bookbal.DeprToPeriod = bookbal.DisposalPeriodID;
							}
							bookbal.LastPeriod = bookbal.DisposalPeriodID;
							bookbalance.Update(bookbal);

							cmd.View.Clear();
							if (cmd.SelectWindowed(0, 1, bookbal.AssetID, FixedAssetStatus.Disposed).Count == 0)
							{
								det = (FADetails)fadetail.Cache.Locate(det) ?? det;
								det.Status = FixedAssetStatus.Disposed;
								fadetail.Update(det);
							}
							break;
						case FARegister.origin.Depreciation:
							bookbal = (FABookBalance)bookbalance.Cache.Locate(bookbal) ?? bookbal;
							if (string.Equals(bookbal.DeprToPeriod, fatran.FinPeriodID))
							{
								bookbal.LastDeprPeriod = fatran.FinPeriodID;
								bookbal.CurrDeprPeriod = null;
								bookbal.Status = FixedAssetStatus.FullyDepreciated;
								bookbalance.Update(bookbal);

								if (IsFullyDepreciatedAsset(this, bookbal.AssetID))
								{
									det.Status = FixedAssetStatus.FullyDepreciated;
									fadetail.Update(det);
								}
							}
							else
							{
								bookbal.LastDeprPeriod = fatran.FinPeriodID;
								bookbal.CurrDeprPeriod = FABookPeriodIDAttribute.PeriodPlusPeriod(this, bookbal.LastDeprPeriod, 1, bookbal.BookID);
								bookbalance.Update(bookbal);

								hist = bookhist.Insert(new FABookHist
								{
									AssetID = bookbal.AssetID,
									BookID = bookbal.BookID,
									FinPeriodID = bookbal.CurrDeprPeriod
								});
							}
							break;
						case FARegister.origin.Reversal:
							bookbal = (FABookBalance)bookbalance.Cache.Locate(bookbal) ?? bookbal;
							bookbal.Status = FixedAssetStatus.Reversed;
							bookbal.CurrDeprPeriod = null;
							bookbalance.Update(bookbal);

							cmd.View.Clear();
							if (cmd.SelectWindowed(0, 1, bookbal.AssetID, FixedAssetStatus.Reversed).Count == 0)
							{
								det = (FADetails)fadetail.Cache.Locate(det) ?? det;
								det.Status = FixedAssetStatus.Reversed;
								fadetail.Update(det);
							}

							FAAccrualTran accrual = PXSelect<FAAccrualTran, Where<FAAccrualTran.tranID, Equal<Current<FATran.gLtranID>>>>.SelectSingleBound(this, new object[] { fatran });
							if (accrual != null)
							{
								FAAccrualTran copy = (FAAccrualTran)accrualtran.Cache.CreateCopy(accrual);
								copy.ClosedAmt -= fatran.TranAmt;
								copy.ClosedQty--;
								copy.OpenAmt += fatran.TranAmt;
								copy.OpenQty++;
								accrualtran.Update(copy);
							}
							break;
						case FARegister.origin.DisposalReversal:
							det.DisposalDate = null;
							det.DisposalMethodID = null;
							det.SaleAmount = null;

							bookbal = (FABookBalance)bookbalance.Cache.Locate(bookbal) ?? bookbal;
							bookbal.DeprToDate = bookbal.OrigDeprToDate;
							bookbalance.Cache.SetDefaultExt<FABookBalance.deprToPeriod>(bookbal);
							FABookHistory lhist = PXSelect<FABookHistory, Where<FABookHistory.assetID, Equal<Current<FABookBalance.assetID>>, And<FABookHistory.bookID, Equal<Current<FABookBalance.bookID>>, And<FABookHistory.ytdReversed, Greater<int0>>>>, OrderBy<Desc<FABookHistory.finPeriodID>>>.SelectSingleBound(this, new object[] { bookbal });
							if(lhist != null)
							{
								bookbal.DeprToPeriod = bookbal.Depreciate == true
									? FABookPeriodIDAttribute.PeriodPlusPeriod(this, bookbal.DeprToPeriod,lhist.YtdReversed ?? 0, bookbal.BookID)
									: null;
							}

							bool isFullyDepreciated = string.CompareOrdinal(bookbal.LastDeprPeriod, bookbal.DeprToPeriod) >= 0;
							bookbal.Status = bookbal.Depreciate == true && isFullyDepreciated ? FixedAssetStatus.FullyDepreciated : FixedAssetStatus.Active;

							bookbal.CurrDeprPeriod = bookbal.Depreciate != true 
														? bookbal.DeprFromPeriod 
														: isFullyDepreciated 
															? null 
															: hist.FinPeriodID;
							bookbal.LastDeprPeriod = bookbal.Depreciate != true 
														? null 
														: isFullyDepreciated 
															? bookbal.DeprToPeriod 
															: FABookPeriodIDAttribute.PeriodPlusPeriod(this, bookbal.CurrDeprPeriod, -1, bookbal.BookID);

							bookbal.LastPeriod = bookbal.DeprToPeriod ?? fatran.FinPeriodID;
							bookbalance.Update(bookbal);

							det = (FADetails)fadetail.Cache.Locate(det) ?? det;
							det.Status = bookbal.Status == FixedAssetStatus.FullyDepreciated && IsFullyDepreciatedAsset(this, bookbal.AssetID)
								? FixedAssetStatus.FullyDepreciated
								: FixedAssetStatus.Active;
							fadetail.Update(det);
							break;
						case FARegister.origin.Split:
							if ((fatran.TranType == FATran.tranType.PurchasingPlus || fatran.TranType == FATran.tranType.PurchasingMinus) && string.CompareOrdinal(fatran.FinPeriodID, bookbal.LastPeriod) > 0)
							{
								bookbal.LastPeriod = fatran.FinPeriodID;
								bookbalance.Update(bookbal);
							}
							break;
						case FARegister.origin.Reconcilliation:
							if ((fatran.TranType == FATran.tranType.DepreciationMinus || fatran.TranType == FATran.tranType.DepreciationPlus)
								&& string.CompareOrdinal(fatran.FinPeriodID, bookbal.LastPeriod) > 0)
							{
								bookbal.LastPeriod = fatran.FinPeriodID;
								bookbal.LastDeprPeriod = fatran.FinPeriodID;
								bookbalance.Update(bookbal);
							}
							break;
					}

					if (UpdateGL && book.UpdateGL == true && doc.Origin != FARegister.origin.Split)
					{
						SetControlAccountFlags(fatran);

						bool summaryPost = SummPost || (SummPostDepr && fatran.Origin == FARegister.origin.Depreciation);
						SegregateBatch(je, asset.BranchID, fatran.TranDate, fatran.FinPeriodID, doc.DocDesc, created);
						{
							GLTran tran = new GLTran();
							tran.SummPost = summaryPost;
							tran.ReclassificationProhibited = fatran.ReclassificationOnDebitProhibited;
							tran.AccountID = fatran.DebitAccountID;
							tran.SubID = fatran.DebitSubID;
							tran.CuryDebitAmt = fatran.TranAmt;
							tran.CuryCreditAmt = 0m;
							tran.DebitAmt = fatran.TranAmt;
							tran.CreditAmt = 0m;
							tran.TranType = fatran.TranType;
							tran.Released = true;
							tran.TranDesc = fatran.TranDesc;
							tran.RefNbr = fatran.RefNbr;
							tran.TranLineNbr = summaryPost ? null : fatran.LineNbr;
							switch (fatran.TranType)
							{
								case FATran.tranType.TransferDepreciation:
									tran.BranchID = fatran.SrcBranchID;
									break;
								default:
									tran.BranchID = fatran.BranchID;
									break;
							}

							tran = je.GLTranModuleBatNbr.Insert(tran);
						}

						{
							GLTran tran = new GLTran();
							tran.SummPost = summaryPost;
							tran.ReclassificationProhibited = fatran.ReclassificationOnCreditProhibited;
							tran.AccountID = fatran.CreditAccountID;
							tran.SubID = fatran.CreditSubID;
							tran.CuryDebitAmt = 0m;
							tran.CuryCreditAmt = fatran.TranAmt;
							tran.DebitAmt = 0m;
							tran.CreditAmt = fatran.TranAmt;
							tran.TranType = fatran.TranType;
							tran.Released = true;
							tran.TranDesc = fatran.TranDesc;
							tran.RefNbr = fatran.RefNbr;
							tran.TranLineNbr = summaryPost ? null : fatran.LineNbr;
							switch (fatran.TranType)
							{
								case FATran.tranType.ReconciliationPlus:
								case FATran.tranType.ReconciliationMinus:
									GLTran orig = PXSelect<GLTran, Where<GLTran.tranID, Equal<Current<FATran.gLtranID>>>>.SelectSingleBound(this, new object[] { fatran });
									if (orig != null)
									{
										tran.BranchID = orig.BranchID;
									}
									else if (doc.Origin == FARegister.origin.Split || doc.Origin == FARegister.origin.Reversal)
									{
										tran.BranchID = fatran.BranchID;
									}
									else
									{
										throw new PXException(Messages.InvalidReconTran);
									}
									break;
								case FATran.tranType.TransferPurchasing:
									tran.BranchID = fatran.SrcBranchID;
									break;
								default:
									tran.BranchID = fatran.BranchID;
									break;
							}

							tran = je.GLTranModuleBatNbr.Insert(tran);
						}

						if (je.GLTranModuleBatNbr.Cache.IsInsertedUpdatedDeleted)
						{
							je.Save.Press();

							if (je.BatchModule.Current != null) 
							{
								fatran.BatchNbr = je.BatchModule.Current.BatchNbr;
								
								if (created.Find(je.BatchModule.Current) == null)
								{
									created.Add(je.BatchModule.Current);
								}
							}
						}

						doc.Posted |= (fatran.BatchNbr != null);
					}

					fatran.Released = true;
					booktran.Update(fatran);

					if(fatran.TranType == FATran.tranType.PurchasingPlus)
					{
						asset.IsAcquired = true;
						fixedasset.Update(asset);
					}
				}

				doc.Released = true;
				register.Update(doc);

				this.Actions.PressSave();
				//CalculateAsset(books);

				ts.Complete();
			}
		}

		private static readonly string[] NonReclassifiableTranTypes =
		{
			FATran.tranType.PurchasingPlus, FATran.tranType.PurchasingMinus,
			FATran.tranType.ReconciliationPlus, FATran.tranType.ReconciliationMinus,
			FATran.tranType.TransferPurchasing, FATran.tranType.TransferDepreciation,
			FATran.tranType.PurchasingReversal
		};

		private static readonly string[] FullyReclassifiableTranTypes =
		{
			FATran.tranType.SalePlus, FATran.tranType.SaleMinus
		};

		private void SetControlAccountFlags(FATran fatran)
		{
			if (NonReclassifiableTranTypes.Contains(fatran.TranType))
			{
				CheckControlAccountFlagsIsNotDefined(fatran);

				fatran.ReclassificationOnDebitProhibited = true;
				fatran.ReclassificationOnCreditProhibited = true;
			}
			else if (FullyReclassifiableTranTypes.Contains(fatran.TranType))
			{
				CheckControlAccountFlagsIsNotDefined(fatran);

				fatran.ReclassificationOnDebitProhibited = false;
				fatran.ReclassificationOnCreditProhibited = false;
			}
			else if (fatran.TranType == FATran.tranType.PurchasingDisposal)
			{
				CheckControlAccountFlagsIsNotDefined(fatran);

				fatran.ReclassificationOnDebitProhibited = true;
				fatran.ReclassificationOnCreditProhibited = false;//GainLoss
			}
			else if (fatran.TranType == FATran.tranType.AdjustingDeprPlus)
			{
				if (fatran.ReclassificationOnDebitProhibited == null)
				{
					fatran.ReclassificationOnDebitProhibited = false;//Depreciated Expense
				}
				if (fatran.ReclassificationOnCreditProhibited == null)
				{
					fatran.ReclassificationOnCreditProhibited = true;//Accumulated Depretiation
				}
			}
			else if (fatran.TranType == FATran.tranType.AdjustingDeprMinus)
			{
				if (fatran.ReclassificationOnDebitProhibited == null)
				{
					fatran.ReclassificationOnDebitProhibited = true;//Accumulated Depretiation
				}
				if (fatran.ReclassificationOnCreditProhibited == null)
				{
					fatran.ReclassificationOnCreditProhibited = false;//Depreciated Expense or GainLoss
				}
			}
			else if (fatran.TranType == FATran.tranType.DepreciationPlus)
			{
				CheckControlAccountFlagsIsNotDefined(fatran);

				fatran.ReclassificationOnDebitProhibited = false;//Depreciated Expense
				fatran.ReclassificationOnCreditProhibited = true;//Accumulated Depretiation
			}
			else if (fatran.TranType == FATran.tranType.DepreciationMinus)
			{
				CheckControlAccountFlagsIsNotDefined(fatran);

				fatran.ReclassificationOnDebitProhibited = true; //Accumulated Depretiation
				fatran.ReclassificationOnCreditProhibited = false; //Depreciated Expense
			}
			else
			{
				throw new PXException(Messages.FlagHasValueOnButItMustBeUndefinedForTransactionType,
					Caches[typeof (FATran)].GetValueExt<FATran.tranType>(fatran), fatran.ToString());
			}
		}

		private void CheckControlAccountFlagsIsNotDefined(FATran fatran)
		{
			if (fatran.ReclassificationOnDebitProhibited != null)
				throw new PXException(Messages.FlagsOfControlAccountsWereNotDefinedForType,
					typeof (FATran.reclassificationOnDebitProhibited).Name, fatran.ToString(),
					Caches[typeof (FATran)].GetValueExt<FATran.tranType>(fatran));

			if (fatran.ReclassificationOnCreditProhibited != null)
				throw new PXException(Messages.FlagsOfControlAccountsWereNotDefinedForType,
					typeof (FATran.reclassificationOnCreditProhibited).Name, fatran.ToString(),
					Caches[typeof (FATran)].GetValueExt<FATran.tranType>(fatran));
		}

		public static bool IsFullyDepreciatedAsset(PXGraph graph, int? AssetID)
		{
			FABookBalance bookbal = PXSelect<FABookBalance,
				Where<FABookBalance.assetID, Equal<Required<FABookBalance.assetID>>,
				And<FABookBalance.status, NotEqual<FixedAssetStatus.fullyDepreciated>>>>.SelectSingleBound(graph, null, AssetID);

			return bookbal == null;
		}

		private static void SegregateBatch(JournalEntry je, int? BranchID, DateTime? DocDate, string FinPeriodID, string descr, DocumentList<Batch> created)
		{
			if (je.GLTranModuleBatNbr.Cache.IsInsertedUpdatedDeleted)
			{
				je.Save.Press();

				if (created.Find(je.BatchModule.Current) == null)
				{
					created.Add(je.BatchModule.Current);
				}
			}

			Batch fabatch = created.Find<Batch.branchID, Batch.finPeriodID>(BranchID, FinPeriodID) ?? new Batch();

			if (fabatch.BatchNbr != null)
			{
				if (!je.BatchModule.Cache.ObjectsEqual(je.BatchModule.Current, fabatch))
				{
					je.Clear();
				}

				Batch newbatch = je.BatchModule.Search<Batch.batchNbr>(fabatch.BatchNbr, fabatch.Module);
				if (newbatch.Description != descr)
				{
					newbatch.Description = "";
					je.BatchModule.Update(newbatch);
				}
				je.BatchModule.Current = newbatch;
			}
			else
			{
				je.Clear();

				fabatch = new Batch();
				fabatch.Module = "FA";
				fabatch.Status = "U";
				fabatch.Released = true;
				fabatch.Hold = false;			
				fabatch.TranPeriodID = FinPeriodID;
				fabatch.BranchID = BranchID;
				fabatch.DebitTotal = 0m;
				fabatch.CreditTotal = 0m;
				fabatch.Description = descr;
				fabatch = je.BatchModule.Insert(fabatch);
				fabatch.DateEntered = DocDate;
				fabatch.FinPeriodID = FinPeriodID;
			}
		}

		public static void TransferAsset(PXGraph graph, FixedAsset asset, FALocationHistory location, ref FARegister register)
		{
			PXCache lochistoryCache = graph.Caches[typeof (FALocationHistory)];
			PXCache assetCache = graph.Caches[typeof (FixedAsset)];
			PXCache registerCache = graph.Caches[typeof(FARegister)];
			PXCache transactCache = graph.Caches[typeof(FATran)];
			FASetup fasetup = (FASetup)graph.Caches[typeof(FASetup)].Current;

			if (!assetCache.IsInsertedUpdatedDeleted)
			{
				assetCache.ClearQueryCache();
				assetCache.Clear();
				graph.Caches<FixedAsset>().Current = 
					asset = PXSelectReadonly<FixedAsset, Where<FixedAsset.assetCD, Equal<Current<FixedAsset.assetCD>>>>.SelectSingleBound(graph, new object[] {asset});
			}

			int? oldFAAccountID = asset.FAAccountID;
			int? oldFASubID = asset.FASubID;
			int? oldAccumulatedDepreciationAccountID = asset.AccumulatedDepreciationAccountID;
			int? oldAccumulatedDepreciationSubID = asset.AccumulatedDepreciationSubID;
			int? oldDepreciatedExpenseAccountID = asset.DepreciatedExpenseAccountID;
			int? oldDepreciatedExpenseSubID = asset.DepreciatedExpenseSubID;
			int? oldDisposalAccountID = asset.DisposalAccountID;
			int? oldDisposalSubID = asset.DisposalSubID;
			int? oldGainAcctID = asset.GainAcctID;
			int? oldGainSubID = asset.GainSubID;
			int? oldLossAcctID = asset.LossAcctID;
			int? oldLossSubID = asset.LossSubID;
			int? oldBranchID = asset.BranchID;

			int? classID = (asset.OldClassID != null && asset.OldClassID != asset.ClassID ? asset.ClassID : asset.AssetID);

			FixedAsset faclass = PXSelect<FixedAsset, Where<FixedAsset.assetID, Equal<Required<FixedAsset.assetID>>>>.Select(graph, classID);
			if (faclass == null)
			{
				throw new PXException(ErrorMessages.ValueDoesntExist, Messages.AssetClass, asset.ClassID);
			}

			int? newFAAccountID = (int?)assetCache.GetValue<FixedAsset.fAAccountID>(faclass);
			int? newFASubID = AssetMaint.MakeSubID<FixedAsset.fASubMask, FixedAsset.fASubID>(assetCache, asset);
			int? newAccumulatedDepreciationAccountID = (int?)assetCache.GetValue<FixedAsset.accumulatedDepreciationAccountID>(faclass);
			int? newAccumulatedDepreciationSubID = AssetMaint.MakeSubID<FixedAsset.accumDeprSubMask, FixedAsset.accumulatedDepreciationSubID>(assetCache, asset);
			int? newDepreciatedExpenseAccountID = (int?)assetCache.GetValue<FixedAsset.depreciatedExpenseAccountID>(faclass);
			int? newDepreciatedExpenseSubID = AssetMaint.MakeSubID<FixedAsset.deprExpenceSubMask, FixedAsset.depreciatedExpenseSubID>(assetCache, asset);
			int? newDisposalAccountID = (int?)assetCache.GetValue<FixedAsset.disposalAccountID>(faclass);
			int? newDisposalSubID = AssetMaint.MakeSubID<FixedAsset.proceedsSubMask, FixedAsset.disposalSubID>(assetCache, asset);
			int? newGainAcctID = (int?)assetCache.GetValue<FixedAsset.gainAcctID>(faclass);
			int? newGainSubID = AssetMaint.MakeSubID<FixedAsset.gainLossSubMask, FixedAsset.gainSubID>(assetCache, asset);
			int? newLossAcctID = (int?)assetCache.GetValue<FixedAsset.lossAcctID>(faclass);
			int? newLossSubID = AssetMaint.MakeSubID<FixedAsset.gainLossSubMask, FixedAsset.lossSubID>(assetCache, asset);
			int? newBranchID = location.BranchID;

			FALocationHistory oldLocation = PXCache<FALocationHistory>.CreateCopy(location);

			location.FAAccountID = newFAAccountID;
			location.FASubID = newFASubID;
			location.AccumulatedDepreciationAccountID = newAccumulatedDepreciationAccountID;
			location.AccumulatedDepreciationSubID = newAccumulatedDepreciationSubID;
			location.DepreciatedExpenseAccountID = newDepreciatedExpenseAccountID;
			location.DepreciatedExpenseSubID = newDepreciatedExpenseSubID;
			location.DisposalAccountID = newDisposalAccountID;
			location.DisposalSubID = newDisposalSubID;
			location.GainAcctID = newGainAcctID;
			location.GainSubID = newGainSubID;
			location.LossAcctID = newLossAcctID;
			location.LossSubID = newLossSubID;

			if (!lochistoryCache.ObjectsEqual<FALocationHistory.fAAccountID, FALocationHistory.fASubID,
					FALocationHistory.accumulatedDepreciationAccountID, FALocationHistory.accumulatedDepreciationSubID,
					FALocationHistory.depreciatedExpenseAccountID, FALocationHistory.depreciatedExpenseSubID>(location, oldLocation)) // really transfer
			{
				lochistoryCache.SmartSetStatus(location, PXEntryStatus.Updated);
			}

			foreach (PXResult<FARegister, FATran> res in PXSelectJoin<FARegister, InnerJoin<FATran,
				On<FARegister.refNbr, Equal<FATran.refNbr>>>,
				Where<FARegister.released, NotEqual<True>,
					And<FARegister.origin, Equal<FARegister.origin.transfer>,
					And<FATran.assetID, Equal<Current<FixedAsset.assetID>>>>>>.SelectMultiBound(graph, new object[] { asset }))
			{
				FARegister openreg = res;
				FATran opentran = res;
				register = register ?? openreg;
				transactCache.Delete(opentran);
			}
			location.RefNbr = null;

			PXResultset<FABookBalance> balset;
			if ((newFAAccountID != oldFAAccountID || newFASubID != oldFASubID ||
				newAccumulatedDepreciationAccountID != oldAccumulatedDepreciationAccountID ||newAccumulatedDepreciationSubID != oldAccumulatedDepreciationSubID || 
				newBranchID != oldBranchID) && fasetup.UpdateGL == true &&
				(balset = PXSelectJoin<FABookBalance, LeftJoin<FABook, On<FABookBalance.bookID, Equal<FABook.bookID>>>,
							Where<FABookBalance.assetID, Equal<Current<FixedAsset.assetID>>,
							And<FABook.updateGL, Equal<True>>>>.SelectMultiBound(graph, new object[]{asset})).Count > 0)
			{
				if (register == null)
				{
					register = (FARegister)registerCache.Insert(new FARegister
					{
						BranchID = newBranchID,
						Origin = FARegister.origin.Transfer,
						DocDate = location.TransactionDate
					});
				}
				else
				{
					registerCache.Current = register;
				}
				register.Reason = location.Reason;

				foreach (FABookBalance bal in balset.RowCast<FABookBalance>().Where(bal => bal.YtdDeprBase != 0m && !string.IsNullOrEmpty(location.PeriodID) && oldBranchID != null))
				{
					if (newFAAccountID != oldFAAccountID || newFASubID != oldFASubID || newBranchID != oldBranchID)
					{
						FATran tp = (FATran)transactCache.Insert(new FATran
						{
							AssetID = asset.AssetID,
							BookID = bal.BookID,
							TranType = FATran.tranType.TransferPurchasing,
							FinPeriodID = location.PeriodID ?? bal.CurrDeprPeriod ?? bal.LastPeriod,
							TranDate = location.TransactionDate,
							CreditAccountID = oldFAAccountID,
							CreditSubID = oldFASubID,
							DebitAccountID = newFAAccountID,
							DebitSubID = newFASubID,
							TranAmt = bal.YtdAcquired,
							TranDesc = PXMessages.LocalizeFormatNoPrefix(Messages.TranDescTransferPurchasing, asset.AssetCD),
							BranchID = newBranchID,
							SrcBranchID = oldBranchID,
							LineNbr = (int?)PXLineNbrAttribute.NewLineNbr<FATran.lineNbr>(transactCache, register),
						});
						location.RefNbr = register.RefNbr;
					}
					if (newAccumulatedDepreciationAccountID != oldAccumulatedDepreciationAccountID || newAccumulatedDepreciationSubID != oldAccumulatedDepreciationSubID || newBranchID != oldBranchID)
					{
						FATran td = (FATran)transactCache.Insert(new FATran
						{
							AssetID = asset.AssetID,
							BookID = bal.BookID,
							TranType = FATran.tranType.TransferDepreciation,
							FinPeriodID = location.PeriodID ?? bal.CurrDeprPeriod ?? bal.LastPeriod,
							TranDate = location.TransactionDate,
							CreditAccountID = newAccumulatedDepreciationAccountID,
							CreditSubID = newAccumulatedDepreciationSubID,
							DebitAccountID = oldAccumulatedDepreciationAccountID,
							DebitSubID = oldAccumulatedDepreciationSubID,
							TranAmt = bal.YtdDepreciated,
							TranDesc = PXMessages.LocalizeFormatNoPrefix(Messages.TranDescTransferDepreciation, asset.AssetCD),
							BranchID = newBranchID,
							SrcBranchID = oldBranchID,
							LineNbr = (int?)PXLineNbrAttribute.NewLineNbr<FATran.lineNbr>(transactCache, register)
						});
					}
				}

				FixedAsset copy = (FixedAsset)assetCache.CreateCopy(asset);
				copy.DepreciatedExpenseAccountID = newDepreciatedExpenseAccountID;
				copy.DepreciatedExpenseSubID = newDepreciatedExpenseSubID;
				copy.DisposalAccountID = newDisposalAccountID;
				copy.DisposalSubID = newDisposalSubID;
				copy.GainAcctID = newGainAcctID;
				copy.GainSubID = newGainSubID;
				copy.LossAcctID = newLossAcctID;
				copy.LossSubID = newLossSubID;
				if ((newDepreciatedExpenseAccountID != oldDepreciatedExpenseAccountID || 
					newDepreciatedExpenseSubID != oldDepreciatedExpenseSubID ||
					newDisposalAccountID != oldDisposalAccountID ||
					newDisposalSubID != oldDisposalSubID ||
					newGainAcctID != oldGainAcctID ||
					newGainSubID != oldGainSubID ||
					newLossAcctID != oldLossAcctID ||
					newLossSubID != oldLossSubID) 
					)
				{
					assetCache.Update(copy);
				}
			}

			#region garbage collection

			FALocationHistory prevLocation = graph.GetPrevLocation(location);
			if (!graph.IsLocationChanged(location, prevLocation))
			{
				graph.Caches<FALocationHistory>().Delete(location);
				FALocationHistory updated = PXCache<FALocationHistory>.CreateCopy(location);
				updated.RevisionID = prevLocation.RevisionID;
				graph.Caches<FALocationHistory>().Update(updated);

				FADetails details = PXSelect<FADetails, Where<FADetails.assetID, Equal<Current<FALocationHistory.assetID>>>>.SelectSingleBound(graph, new object[] {location});
				details.LocationRevID = updated.RevisionID;
				graph.Caches<FADetails>().Update(details);
			}

			// Clear empty unreleased transfer
			FATran existing = PXSelect<FATran, Where<FATran.refNbr, Equal<Current<FARegister.refNbr>>>>.SelectSingleBound(graph, new object[] {registerCache.Current});
			if (existing == null)
			{
				registerCache.Delete(registerCache.Current);
			}

			#endregion
		}
	}

	public static class FAHelper
	{
		public static string CloseFABookHistory(this PXGraph graph, FABookBalance bal, IEnumerable<string> periodIDs)
		{
			string maxPeriodID = null;
			foreach (string id in periodIDs.Where(id => !string.IsNullOrEmpty(id)))
			{
				if (maxPeriodID == null)
				{
					graph.EnsureCachePersistence<FABookHist>();
				}

				FABookHist closeHist = new FABookHist
				{
					AssetID = bal.AssetID,
					BookID = bal.BookID,
					FinPeriodID = id,
					Closed = true
				};

				closeHist = (FABookHist)graph.Caches<FABookHist>().Insert(closeHist);

				if (maxPeriodID == null || string.CompareOrdinal(id, maxPeriodID) > 0)
				{
					maxPeriodID = id;
				}
			}
			return maxPeriodID;
		}

		public static string CloseFABookHistory(this PXGraph graph, FABookBalance bal, string toPeriod, bool force = false)
		{
			FABookHistory hist = PXSelect<FABookHistory, 
				Where<FABookHistory.assetID, Equal<Required<FABookBalance.assetID>>, 
					And<FABookHistory.bookID, Equal<Required<FABookBalance.bookID>>,
					And<FABookHistory.closed, NotEqual<True>>>>,
				OrderBy<Asc<FABookHistory.finPeriodID>>>.Select(graph, bal.AssetID, bal.BookID);

			string periodID = force
				? bal.DeprFromPeriod
				: (hist != null && string.CompareOrdinal(hist.FinPeriodID, bal.HistPeriod) > 0
					? hist.FinPeriodID
					: bal.HistPeriod);

			List<string> periodIDs = new List<string>();
			while (!string.IsNullOrEmpty(periodID) && string.CompareOrdinal(periodID, toPeriod) < 0)
			{
				periodIDs.Add(periodID);
				periodID = FABookPeriodIDAttribute.NextPeriod(graph, periodID, bal.BookID);
			}
			return graph.CloseFABookHistory(bal, periodIDs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FALocationHistory GetCurrentLocation(this PXGraph graph, FADetails details)
		{
			FALocationHistory current = PXSelect<FALocationHistory, 
				Where<FALocationHistory.assetID, Equal<Current<FADetails.assetID>>, 
					And<FALocationHistory.revisionID, Equal<Current<FADetails.locationRevID>>>>>
				.SelectSingleBound(graph, new object[] {details});
			return current;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FALocationHistory GetPrevLocation(this PXGraph graph, FALocationHistory current)
		{
			FALocationHistory prev = PXSelect<FALocationHistory,
				Where<FALocationHistory.assetID, Equal<Current<FALocationHistory.assetID>>,
					And<FALocationHistory.revisionID, Less<Current<FALocationHistory.revisionID>>>>,
				OrderBy<Desc<FALocationHistory.revisionID>>>
				.SelectSingleBound(graph, new object[] { current });
			return prev;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsLocationChanged(this PXGraph graph, FALocationHistory current, FALocationHistory prev=null)
		{
			return !graph.Caches<FALocationHistory>().ObjectsEqual<
				FALocationHistory.locationID, 
				FALocationHistory.buildingID,
				FALocationHistory.floor, 
				FALocationHistory.room,
				FALocationHistory.employeeID,
				FALocationHistory.department>(prev ?? graph.GetPrevLocation(current), current);
		}
	}
}

namespace PX.Objects.FA.Overrides.AssetProcess
{
	public class FABookHistAccumAttribute : PXAccumulatorAttribute
	{
		public FABookHistAccumAttribute()
			: base( 
			new Type[] {
				typeof(FABookHist.ytdCalculated),
				typeof(FABookHist.ytdBal),
				typeof(FABookHist.ytdBal),
				typeof(FABookHist.ytdDeprBase),
				typeof(FABookHist.ytdDeprBase),
				typeof(FABookHist.ytdBonus),
				typeof(FABookHist.ytdBonusTaken),
				typeof(FABookHist.ytdBonusCalculated),
				typeof(FABookHist.ytdBonusRecap),
				typeof(FABookHist.ytdTax179),
				typeof(FABookHist.ytdTax179Taken),
				typeof(FABookHist.ytdTax179Calculated),
				typeof(FABookHist.ytdTax179Recap),
				typeof(FABookHist.ytdAcquired),
				typeof(FABookHist.ytdDepreciated),
				typeof(FABookHist.ytdDisposalAmount),
				typeof(FABookHist.ytdRGOL),
				typeof(FABookHist.ytdSuspended),
				typeof(FABookHist.ytdReversed),
				typeof(FABookHist.ytdReconciled),
			}, 
			new Type[] {
				typeof(FABookHist.ytdCalculated),
				typeof(FABookHist.begBal),
				typeof(FABookHist.ytdBal),
				typeof(FABookHist.begDeprBase),
				typeof(FABookHist.ytdDeprBase),
				typeof(FABookHist.ytdBonus),
				typeof(FABookHist.ytdBonusTaken),
				typeof(FABookHist.ytdBonusCalculated),
				typeof(FABookHist.ytdBonusRecap),
				typeof(FABookHist.ytdTax179),
				typeof(FABookHist.ytdTax179Taken),
				typeof(FABookHist.ytdTax179Calculated),
				typeof(FABookHist.ytdTax179Recap),
				typeof(FABookHist.ytdAcquired),
				typeof(FABookHist.ytdDepreciated),
				typeof(FABookHist.ytdDisposalAmount),
				typeof(FABookHist.ytdRGOL),
				typeof(FABookHist.ytdSuspended),
				typeof(FABookHist.ytdReversed),
				typeof(FABookHist.ytdReconciled),
			})
		{ 
		}

		protected override bool PrepareInsert(PXCache sender, object row, PXAccumulatorCollection columns)
		{
			if (!base.PrepareInsert(sender, row, columns))
			{
				return false;
			}

			FABookHist hist = (FABookHist)row;

			columns.Update<FABookHist.closed>(hist.Closed,
											  hist.Closed == true || hist.Reopen == true
												  ? PXDataFieldAssign.AssignBehavior.Replace
												  : PXDataFieldAssign.AssignBehavior.Initialize);

			columns.Update<FABookHist.suspended>(hist.Suspended,
												 hist.Suspended == true
													 ? PXDataFieldAssign.AssignBehavior.Replace
													 : PXDataFieldAssign.AssignBehavior.Initialize);

			columns.Update<FABookHist.ytdSuspended>(hist.YtdSuspended, PXDataFieldAssign.AssignBehavior.Summarize);
			columns.Update<FABookHist.ytdReversed>(hist.YtdReversed, PXDataFieldAssign.AssignBehavior.Summarize);

			columns.Update<FABookHist.createdByID>(hist.CreatedByID, PXDataFieldAssign.AssignBehavior.Initialize);
			columns.Update<FABookHist.createdDateTime>(hist.CreatedDateTime, PXDataFieldAssign.AssignBehavior.Initialize);
			columns.Update<FABookHist.createdByScreenID>(hist.CreatedByScreenID, PXDataFieldAssign.AssignBehavior.Initialize);

			columns.Update<FABookHist.lastModifiedByID>(hist.LastModifiedByID, PXDataFieldAssign.AssignBehavior.Replace);
			columns.Update<FABookHist.lastModifiedDateTime>(hist.LastModifiedDateTime, PXDataFieldAssign.AssignBehavior.Replace);
			columns.Update<FABookHist.lastModifiedByScreenID>(hist.LastModifiedByScreenID, PXDataFieldAssign.AssignBehavior.Replace);

			return true;
		}
	}

	[Serializable()]
	[FABookHistAccum()]
	public partial class FABookHist : FABookHistory
	{
		#region AssetID
		public new abstract class assetID : PX.Data.IBqlField
		{
		}
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public override Int32? AssetID
		{
			get
			{
				return this._AssetID;
			}
			set
			{
				this._AssetID = value;
			}
		}
		#endregion
		#region BookID
		public new abstract class bookID : PX.Data.IBqlField
		{
		}
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public override Int32? BookID
		{
			get
			{
				return this._BookID;
			}
			set
			{
				this._BookID = value;
			}
		}
		#endregion
		#region FinPeriodID
		public new abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		[PXDBString(6, IsKey = true, IsFixed = true)]
		[PXDefault()]
		public override String FinPeriodID
		{
			get
			{
				return this._FinPeriodID;
			}
			set
			{
				this._FinPeriodID = value;
			}
		}
		#endregion
	}
}
