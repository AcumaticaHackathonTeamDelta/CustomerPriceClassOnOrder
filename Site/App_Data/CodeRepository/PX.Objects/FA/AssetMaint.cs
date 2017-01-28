using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Common;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.CM;
using PX.Objects.GL;
using PX.Objects.AP;
using PX.Objects.EP;
using PX.Objects.IN;

namespace PX.Objects.FA
{
	public class OperableTran
	{
		public FATran Tran { get; }
		public PXDBOperation Op { get; set; }

		public OperableTran(FATran tran, PXDBOperation op)
		{
			Tran = tran;
			Op = op;
		}
	}

	public class AssetMaint : PXGraph<AssetMaint, FixedAsset>
	{
		#region Selects Declaration

		public PXSelect<FixedAsset, Where<FixedAsset.recordType, Equal<FARecordType.assetType>>> Asset;
		public PXSelect<FAClass> Class;
		public PXSelect<BAccount> Baccount;
		public PXSelect<Vendor> Vendor;
		public PXSelect<EPEmployee> Employee;
		public PXSelect<FixedAsset, Where<FixedAsset.assetCD, Equal<Current<FixedAsset.assetCD>>>> CurrentAsset;
		public PXSelect<FADetails, Where<FADetails.assetID, Equal<Optional<FixedAsset.assetID>>>> AssetDetails;
		public PXSelect<FABookBalance, Where<FABookBalance.assetID, Equal<Current<FixedAsset.assetID>>>> AssetBalance;
		public PXSelect<FABookSettings, Where<FABookSettings.assetID, Equal<Current<FixedAsset.classID>>>> DepreciationSettings;
		public PXSelect<FABookHistory, Where<FABookHistory.assetID, Equal<Current<FixedAsset.assetID>>,
										 And<FABookHistory.bookID, Equal<Current<FABookSettings.bookID>>>>> BookHistory;

		public PXSelectJoin<FAComponent, InnerJoin<FADetails, On<FAComponent.assetID, Equal<FADetails.assetID>>>, Where<FAComponent.parentAssetID, Equal<Current<FixedAsset.assetID>>>> AssetElements;

		public PXSelect<FARegister> Register;
		public PXSelect<FATran> FATransactions;
		public PXSelect<FALocationHistory,
						Where<FALocationHistory.assetID, Equal<Current<FixedAsset.assetID>>>,
						OrderBy<Desc<FALocationHistory.revisionID>>> LocationHistory;

		public PXSelect<FAUsage, Where<FAUsage.assetID, Equal<Current<FixedAsset.assetID>>>> AssetUsage;

		public PXSelect<FAServiceSchedule, Where<FAServiceSchedule.scheduleID, Equal<Current<FixedAsset.serviceScheduleID>>>> ServiceSchedule;

		public PXSelect<FAService, Where<FAService.assetID, Equal<Current<FixedAsset.assetID>>>> AssetService;

		public PXSelect<FALocationHistory, Where<FALocationHistory.assetID, Equal<Optional<FADetails.assetID>>,
			And<FALocationHistory.revisionID, Equal<Optional<FADetails.locationRevID>>>>> AssetLocation;
		[PXCopyPasteHiddenView]
		public PXSelectOrderBy<FAHistory, OrderBy<Asc<FAHistory.finPeriodID>>> AssetHistory;

		[PXCopyPasteHiddenView]
		public PXSelectOrderBy<FASheetHistory, OrderBy<Asc<FASheetHistory.periodNbr>>> BookSheetHistory;

		public PXSetup<FASetup> fasetup;
		[PXCopyPasteHiddenView]
		public PXFilter<DeprBookFilter> deprbookfilter;
		[PXCopyPasteHiddenView]
		public PXFilter<TranBookFilter> bookfilter;

		public PXFilter<DisposeParams> DispParams;
		public PXFilter<SuspendParams> SuspendParams;
		public PXFilter<ReverseDisposalInfo> RevDispInfo;

		public PXFilter<GLTranFilter> GLTrnFilter;
		public PXSelect<FAAccrualTran> Additions;

		[PXFilterable]
		[PXCopyPasteHiddenView]
		public PXSelectJoin<DsplFAATran, LeftJoin<FAAccrualTran, On<DsplFAATran.tranID, Equal<FAAccrualTran.tranID>>>> DsplAdditions;

		public PXSetup<GLSetup> glsetup;
		public PXSetup<Company> company;
		#endregion

		#region Ctor

		public Dictionary<int?, int> _Books = new Dictionary<int?, int>();
		public List<string> _FieldNames = new List<string>();
		public List<string> _SheetFieldNames = new List<string>();
		private FixedAsset _PersistedAsset;

		public AssetMaint()
		{
			PXCache glCache = Caches[typeof(GLTran)];
			PXCache detCache = AssetDetails.Cache;
			PXCache cache = Asset.Cache;

			object setup = fasetup.Current;
			setup = glsetup.Current;

			PXUIFieldAttribute.SetRequired<FADetails.propertyType>(detCache, true);
			PXUIFieldAttribute.SetRequired<FixedAsset.assetTypeID>(cache, true);
			PXUIFieldAttribute.SetRequired<FixedAsset.classID>(cache, true);

			PXUIFieldAttribute.SetVisible<GLTran.module>(glCache, null, false);
			PXUIFieldAttribute.SetVisible<GLTran.batchNbr>(glCache, null, false);
			PXUIFieldAttribute.SetVisible<GLTran.refNbr>(glCache, null, false);

			int i = 0;
			foreach (FABook book in PXSelect<FABook>.Select(this))
			{
				int j = i++;
				string fname;
				_Books.Add(book.BookID, j);
				fname = string.Format("{0} Calculated", book.BookCode);
				_FieldNames.Add(fname);
				AssetHistory.Cache.Fields.Add(fname);
				FieldSelecting.AddHandler(typeof(FAHistory), fname, (sender, e) => PtdCalculatedFieldSelecting(sender, e, j, true));
				fname = string.Format("{0} Depreciated", book.BookCode);
				_FieldNames.Add(fname);
				AssetHistory.Cache.Fields.Add(fname);
				FieldSelecting.AddHandler(typeof(FAHistory), fname, (sender, e) => PtdCalculatedFieldSelecting(sender, e, j, false));
			}

			int maxyears = 0;
			foreach (FABookBalance2 bal in PXSelectGroupBy<FABookBalance2, Aggregate<Max<FABookBalance2.exactLife>>>.Select(this))
			{
				maxyears = bal.ExactLife ?? 0;
			}

			for (i = 0; i < maxyears; i++)
			{
				int j = i;
				string fldname = string.Format("Year_{0}", j);
				_SheetFieldNames.Add(fldname);
				BookSheetHistory.Cache.Fields.Add(fldname);
				FieldSelecting.AddHandler(typeof(FASheetHistory), fldname,
					delegate (PXCache sender, PXFieldSelectingEventArgs e)
					{
						BookSheetPeriodDepreciatedFieldSelecting(sender, e, j);
					});
				fldname = string.Format("Calc_{0}", j);
				_SheetFieldNames.Add(fldname);
				BookSheetHistory.Cache.Fields.Add(fldname);
				FieldSelecting.AddHandler(typeof(FASheetHistory), fldname,
					delegate (PXCache sender, PXFieldSelectingEventArgs e)
					{
						BookSheetPtdCalcFieldSelecting(sender, e, j, true);
					});
				fldname = string.Format("Depr_{0}", j);
				_SheetFieldNames.Add(fldname);
				BookSheetHistory.Cache.Fields.Add(fldname);
				FieldSelecting.AddHandler(typeof(FASheetHistory), fldname,
					delegate (PXCache sender, PXFieldSelectingEventArgs e)
					{
						BookSheetPtdCalcFieldSelecting(sender, e, j, false);
					});
			}


			FATransactions.Cache.AllowInsert = false;
			FATransactions.Cache.AllowUpdate = false;
			FATransactions.Cache.AllowDelete = true;

			PXUIFieldAttribute.SetEnabled<FABookBalance.lastDeprPeriod>(AssetBalance.Cache, null, true);
			PXUIFieldAttribute.SetEnabled<FABookBalance.ytdDepreciated>(AssetBalance.Cache, null, true);

			FieldDefaulting.AddHandler<FixedAsset.fASubID>(FASubIDFieldDefaulting<FixedAsset.fASubMask, FixedAsset.fASubID>);
			FieldDefaulting.AddHandler<FixedAsset.accumulatedDepreciationSubID>(FASubIDFieldDefaulting<FixedAsset.accumDeprSubMask, FixedAsset.accumulatedDepreciationSubID>);
			FieldDefaulting.AddHandler<FixedAsset.depreciatedExpenseSubID>(FASubIDFieldDefaulting<FixedAsset.deprExpenceSubMask, FixedAsset.depreciatedExpenseSubID>);
			FieldDefaulting.AddHandler<FixedAsset.disposalSubID>(FASubIDFieldDefaulting<FixedAsset.proceedsSubMask, FixedAsset.disposalSubID>);
			FieldDefaulting.AddHandler<FixedAsset.gainSubID>(FASubIDFieldDefaulting<FixedAsset.gainLossSubMask, FixedAsset.gainSubID>);
			FieldDefaulting.AddHandler<FixedAsset.lossSubID>(FASubIDFieldDefaulting<FixedAsset.gainLossSubMask, FixedAsset.lossSubID>);

			GLTrnFilter.Cache.SetDefaultExt<GLTranFilter.currentCost>(GLTrnFilter.Current);
			GLTrnFilter.Cache.SetDefaultExt<GLTranFilter.accrualBalance>(GLTrnFilter.Current);
			GLTrnFilter.Cache.SetDefaultExt<GLTranFilter.unreconciledAmt>(GLTrnFilter.Current);
			GLTrnFilter.Cache.SetDefaultExt<GLTranFilter.selectionAmt>(GLTrnFilter.Current);

			FieldUpdated.AddHandler<FixedAsset.fAAccountID>(SetFABookBalanceUpdated);
			FieldUpdated.AddHandler<FixedAsset.fASubID>(SetFABookBalanceUpdated);
			FieldUpdated.AddHandler<FixedAsset.accumulatedDepreciationAccountID>(SetFABookBalanceUpdated);
			FieldUpdated.AddHandler<FixedAsset.accumulatedDepreciationSubID>(SetFABookBalanceUpdated);
			FieldUpdated.AddHandler<FixedAsset.fAAccrualAcctID>(SetFABookBalanceUpdated);
			FieldUpdated.AddHandler<FixedAsset.fAAccrualSubID>(SetFABookBalanceUpdated);
			FieldUpdated.AddHandler<FixedAsset.depreciatedExpenseAccountID>(SetFABookBalanceUpdated);
			FieldUpdated.AddHandler<FixedAsset.depreciatedExpenseSubID>(SetFABookBalanceUpdated);

			DsplAdditions.Cache.AllowInsert = false;
		}

		public override int ExecuteUpdate(string viewName, IDictionary keys, IDictionary values, params object[] parameters)
		{
			if (IsImport && viewName == "AssetLocation")
			{
				if (AssetDetails.Current == null)
				{
					AssetDetails.Current = AssetDetails.Select();
				}
				if (keys.Count == 0)
				{
					keys.Add(typeof(FALocationHistory.assetID).Name, AssetDetails.Current.AssetID);
					keys.Add(typeof(FALocationHistory.revisionID).Name, AssetDetails.Current.LocationRevID);
				}
			}
			return base.ExecuteUpdate(viewName, keys, values, parameters);
		}

		protected virtual void SetFABookBalanceUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if (fasetup.Current.UpdateGL != true)
			{
				AssetBalance.Select().RowCast<FABookBalance>().ForEach(balance => sender.Graph.Caches<FABookBalance>().SmartSetStatus(balance, PXEntryStatus.Updated));
			}
		}

		public PXAction<FixedAsset> viewDocument;
		[PXUIField(DisplayName = Messages.ViewDocument, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Enabled = true)]
		[PXButton(OnClosingPopup = PXSpecialButtonType.Cancel)]
		public virtual IEnumerable ViewDocument(PXAdapter adapter)
		{
			TransactionEntry graph = CreateInstance<TransactionEntry>();
			graph.Document.Current = graph.Document.Search<FARegister.refNbr>(FATransactions.Current.RefNbr);
			throw new PXRedirectRequiredException(graph, "FATransactions") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
		}

		public PXAction<FixedAsset> viewBatch;
		[PXUIField(DisplayName = Messages.ViewBatch, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Enabled = true)]
		[PXButton(OnClosingPopup = PXSpecialButtonType.Refresh)]
		public virtual IEnumerable ViewBatch(PXAdapter adapter)
		{
			JournalEntry graph = CreateInstance<JournalEntry>();
			graph.BatchModule.Current = graph.BatchModule.Search<Batch.batchNbr>(FATransactions.Current.BatchNbr, GL.BatchModule.FA);
			throw new PXRedirectRequiredException(graph, "Batch") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
		}

		protected virtual void GLTranFilter_UnreconciledAmt_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			GLTranFilter filter = (GLTranFilter)e.Row;
			if (filter == null || filter.CurrentCost == null || filter.AccrualBalance == null) return;

			e.NewValue = filter.CurrentCost - filter.AccrualBalance;
		}

		protected virtual void GLTranFilter_TranDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			FinPeriod currentPeriod = PXSelectJoin<FinPeriod,
				InnerJoin<FABookBalance, On<FinPeriod.finPeriodID, Equal<FABookBalance.currDeprPeriod>>>,
				Where<FABookBalance.assetID, Equal<Current<FixedAsset.assetID>>,
					And<FABookBalance.updateGL, Equal<True>,
					And<FinPeriod.endDate, Greater<FinPeriod.startDate>>>>>.Select(this);

			if (currentPeriod == null) return;

			if (Accessinfo.BusinessDate >= currentPeriod.StartDate && Accessinfo.BusinessDate <= currentPeriod.EndDate)
			{
				e.NewValue = Accessinfo.BusinessDate;
			}
			else
			{
				e.NewValue = currentPeriod.StartDate;
			}
		}

		public override void Persist()
		{
			//get nonpurchasing/nonreconcilliation first, then reconcilliation, purchasing last
			//purchasing plus first
			//filled batchnbr first
			//add primary key to retain sorting by expression
			PXSelectBase<FATran> cmdsel = new PXSelect<FATran,
				Where<FATran.assetID, Equal<Current<FABookBalance.assetID>>,
					And<FATran.bookID, Equal<Current<FABookBalance.bookID>>>>,
				OrderBy<Asc<Switch<Case<Where<FATran.origin, Equal<FARegister.origin.purchasing>>, int4, Case<Where<FATran.origin, Equal<FARegister.origin.reconcilliation>>, int1>>, int0>,
					Asc<Switch<Case<Where<FATran.tranType, Equal<FATran.tranType.purchasingPlus>>, int0>, int1>,
					Desc<FATran.batchNbr,
					Asc<FATran.refNbr,
					Asc<FATran.lineNbr>>>>>>>(this);

			Dictionary<FABookBalance, OperableTran> booktrn = new Dictionary<FABookBalance, OperableTran>();

			List<FARegister> created;

			FARegister register = PXSelectJoin<FARegister,
				InnerJoin<FATran, On<FARegister.refNbr, Equal<FATran.refNbr>>>,
				Where<FARegister.origin, Equal<FARegister.origin.purchasing>,
					And<FARegister.released, Equal<False>,
					And<FATran.tranType, Equal<FATran.tranType.purchasingPlus>,
					And<FATran.assetID, Equal<Current<FixedAsset.assetID>>>>>>>.Select(this);

			foreach (FABookBalance bookbal in AssetBalance.Cache.Deleted)
			{
				FATran res = (FATran)cmdsel.View.SelectSingleBound(new object[] { bookbal });
				if (res != null && (res.Origin == FARegister.origin.Purchasing || res.Origin == FARegister.origin.Reconcilliation) && res.Released == false)
				{
					booktrn.Add(bookbal, new OperableTran(PXCache<FATran>.CreateCopy(res), PXDBOperation.Delete));
				}
			}

			foreach (FABookBalance bookbal in AssetBalance.Cache.Updated)
			{
				FATran res = (FATran)cmdsel.View.SelectSingleBound(new object[] { bookbal });
				if (res != null &&
					(res.Origin == FARegister.origin.Purchasing || res.Origin == FARegister.origin.Reconcilliation) &&
					(res.Released == false || fasetup.Current.UpdateGL != true && string.IsNullOrEmpty(res.BatchNbr)))
				{
					booktrn.Add(bookbal, new OperableTran(PXCache<FATran>.CreateCopy(res), PXDBOperation.Update));
				}

				if (res == null)
				{
					booktrn.Add(bookbal, new OperableTran(null, PXDBOperation.Insert));
				}
			}

			foreach (FABookBalance bookbal in AssetBalance.Cache.Inserted)
			{
				booktrn.Add(bookbal, new OperableTran(null, PXDBOperation.Insert));
			}

			FARegister transferreg = null;
			FixedAsset asset = CurrentAsset.Current;
			FADetails det = AssetDetails.Select();

			if (asset != null && det.ReceiptDate != null)
			{
				FALocationHistory currentLocation = this.GetCurrentLocation(det);
				FALocationHistory previousLocation = this.GetPrevLocation(currentLocation);
				FABookBalanceTransfer trbal = PXSelect<FABookBalanceTransfer, Where<FABookBalanceTransfer.assetID, Equal<Current<FixedAsset.assetID>>>, OrderBy<Desc<FABookBalance.updateGL>>>.SelectSingleBound(this, new object[] { asset });
				FABookBalance glbal = PXSelect<FABookBalance, Where<FABookBalance.assetID, Equal<Current<FixedAsset.assetID>>>, OrderBy<Desc<FABookBalance.updateGL>>>.SelectSingleBound(this, new object[] { asset });

				//first location or newly created asset
				if (glbal == null)
				{
					currentLocation.PeriodID = FinPeriodIDAttribute.PeriodFromDate(det.ReceiptDate);
					currentLocation.TransactionDate = det.ReceiptDate;
				}
				else if ((previousLocation == null && string.IsNullOrEmpty(currentLocation.PeriodID)) || trbal == null)
				{
					currentLocation.PeriodID = glbal.DeprFromPeriod;
					currentLocation.TransactionDate = det.ReceiptDate;
				}
				else
				{
					currentLocation.PeriodID = trbal.FinPeriodID;
				}

				AssetProcess.TransferAsset(this, asset, currentLocation, ref transferreg);
			}
			FARegister reconreg = null;
			using (PXTransactionScope ts = new PXTransactionScope())
			{
				FARegister r = register;
				foreach (FATran tran in FATransactions.Cache.Inserted.Cast<FATran>().Where(tran => r != null))
				{
					tran.RefNbr = register.RefNbr;
					tran.LineNbr = (int?)PXLineNbrAttribute.NewLineNbr<FATran.lineNbr>(FATransactions.Cache, register);

					FATransactions.Cache.Normalize();
					PXParentAttribute.SetLeaveChildren<FATran.refNbr>(FATransactions.Cache, null, true);
					foreach (FARegister doc in Register.Cache.Inserted.Cast<FARegister>().Where(doc => doc.Origin == FARegister.origin.Purchasing))
					{
						Register.Cache.Delete(doc);
					}
					Register.Cache.Current = register;
				}

				foreach (FARegister item in Register.Cache.Inserted)
				{
					if (item.Origin == FARegister.origin.Purchasing)
					{
						register = item;
					}
					if (item.Origin == FARegister.origin.Reconcilliation)
					{
						reconreg = item;
					}
				}

				base.Persist();

				TransactionEntry docgraph = CreateInstance<TransactionEntry>();
				if (asset != null)
				{
					AssetProcess.AcquireAsset(docgraph, (int)asset.BranchID, booktrn, register);
				}
				created = docgraph.created;
				SelectTimeStamp();

				ts.Complete();
			}
			FATransactions.Cache.Clear();

			PXHashSet<FARegister> toRelease = new PXHashSet<FARegister>(this);
			if (fasetup.Current.AutoReleaseAsset == true && created != null && created.Count > 0)
			{
				toRelease.Add(created[0]);
			}
			if (fasetup.Current.AutoReleaseAsset == true && register != null)
			{
				toRelease.Add(register);
			}
			if (fasetup.Current.AutoReleaseAsset == true && reconreg != null)
			{
				toRelease.Add(reconreg);
			}
			if (fasetup.Current.AutoReleaseTransfer == true && transferreg != null)
			{
				toRelease.Add(transferreg);
			}

			if (det != null && det.Hold == false && toRelease.Count > 0)
			{
				SelectTimeStamp();
				PXLongOperation.StartOperation(this, delegate () { AssetTranRelease.ReleaseDoc(toRelease.ToList(), false); });
			}
		}

		protected int? AssetBookID(int idx)
		{
			return AssetBalance.Select()
				.RowCast<FABookBalance>()
				.FirstOrDefault(book => book != null && book.BookID != null && _Books[book.BookID] == idx && book.Depreciate == true)
				.With(bal => bal.BookID);
		}

		protected virtual void PtdCalculatedFieldSelecting(PXCache sender, PXFieldSelectingEventArgs e, int fieldNbr, bool calc)
		{
			e.ReturnState = PXDecimalState.CreateInstance(e.ReturnState, 2, _FieldNames[2 * fieldNbr + (calc ? 0 : 1)], null, null, null, null);
			((PXDecimalState)e.ReturnState).Visibility = PXUIVisibility.Visible;
			int? bookID = AssetBookID(fieldNbr);
			((PXDecimalState)e.ReturnState).Visible = bookID != null;

			if (e.Row == null || !((PXDecimalState)e.ReturnState).Visible) return;

			FABookPeriod exist_period = PXSelect<FABookPeriod,
												Where<FABookPeriod.bookID, Equal<Required<FABookPeriod.bookID>>,
													And<FABookPeriod.finPeriodID, Equal<Required<FABookPeriod.finPeriodID>>>>>.Select(this, bookID, ((FAHistory)e.Row).FinPeriodID);
			if (exist_period != null)
				e.ReturnValue = ((FAHistory)e.Row).PtdDepreciated[2 * fieldNbr + (calc ? 0 : 1)] ?? 0m;
		}

		protected virtual void BookSheetPeriodDepreciatedFieldSelecting(PXCache sender, PXFieldSelectingEventArgs e, int fieldNbr)
		{
			e.ReturnState = PXStringState.CreateInstance(e.ReturnState, 7, true, _SheetFieldNames[3 * fieldNbr], false, null, "##-####", null, null, null, null);
			((PXStringState)e.ReturnState).Visibility = PXUIVisibility.Visible;

			FABookBalance bal = PXSelect<FABookBalance, Where<FABookBalance.assetID, Equal<Current<FixedAsset.assetID>>,
				And<FABookBalance.bookID, Equal<Current<DeprBookFilter.bookID>>>>>.Select(this);

			if (bal == null)
			{
				((PXStringState)e.ReturnState).Visible = false;
			}
			else
			{
				int yearFrom = Convert.ToInt32(bal.DeprFromYear);
				int yearTo = Math.Max(Convert.ToInt32(bal.DeprToYear), Convert.ToInt32((bal.LastDeprPeriod ?? "1900").Substring(0, 4)));

				((PXStringState)e.ReturnState).Visible = yearFrom + fieldNbr <= yearTo;
			}
			((PXStringState)e.ReturnState).DisplayName = PXMessages.LocalizeNoPrefix(Messages.PeriodFieldName);

			FASheetHistory hist = (FASheetHistory)e.Row;
			if (hist == null || hist.PtdValues == null || hist.PtdValues[fieldNbr] == null) return;

			e.ReturnValue = FABookPeriodIDAttribute.FormatPeriod(hist.PtdValues[fieldNbr].PeriodID);
		}

		protected virtual void BookSheetPtdCalcFieldSelecting(PXCache sender, PXFieldSelectingEventArgs e, int fieldNbr, bool calc)
		{
			e.ReturnState = PXDecimalState.CreateInstance(e.ReturnState, 2, _SheetFieldNames[3 * fieldNbr + (calc ? 1 : 2)], null, null, null, null);
			((PXDecimalState)e.ReturnState).Visibility = PXUIVisibility.Visible;

			FABookBalance bal = PXSelect<FABookBalance, Where<FABookBalance.assetID, Equal<Current<FixedAsset.assetID>>,
				And<FABookBalance.bookID, Equal<Current<DeprBookFilter.bookID>>>>>.Select(this);
			if (bal == null)
			{
				((PXDecimalState)e.ReturnState).Visible = false;
			}
			else
			{
				int yearFrom = Convert.ToInt32(bal.DeprFromYear);
				int yearTo = Math.Max(Convert.ToInt32(bal.DeprToYear), Convert.ToInt32((bal.LastDeprPeriod ?? "1900").Substring(0, 4)));

				((PXDecimalState)e.ReturnState).Visible = yearFrom + fieldNbr <= yearTo;
			}
			((PXDecimalState)e.ReturnState).DisplayName = calc ?
				PXMessages.LocalizeNoPrefix(Messages.CalcValueFieldName) :
				PXMessages.LocalizeNoPrefix(Messages.DeprValueFieldName);

			FASheetHistory hist = (FASheetHistory)e.Row;
			if (hist == null || hist.PtdValues == null || hist.PtdValues[fieldNbr] == null) return;

			e.ReturnValue = calc ? hist.PtdValues[fieldNbr].PtdCalculated : hist.PtdValues[fieldNbr].PtdDepreciated;
		}

		public virtual IEnumerable booksheethistory()
		{
			FixedAsset asset = CurrentAsset.Current;
			DeprBookFilter bookfltr = deprbookfilter.Current;
			if (bookfltr == null || bookfltr.BookID == null || asset == null || asset.AssetID == null) yield break;

			FASheetHistory dyn = null;
			FABookBalance bal = PXSelect<FABookBalance, Where<FABookBalance.assetID, Equal<Current<FixedAsset.assetID>>, And<FABookBalance.bookID, Equal<Current<DeprBookFilter.bookID>>>>>.Select(this);

			if (bal == null) yield break;

			string toYear = bal.DeprToYear;
			string lastDeprYear = string.IsNullOrEmpty(bal.LastDeprPeriod) ? toYear : bal.LastDeprPeriod.Substring(0, 4);
			if (string.CompareOrdinal(lastDeprYear, toYear) > 0)
			{
				toYear = lastDeprYear;
			}

			int fromYearInt;
			int.TryParse(bal.DeprFromYear, out fromYearInt);

			foreach (PXResult<FABookPeriod, FABookHistory> res in PXSelectJoin<FABookPeriod,
				LeftJoin<FABookHistory, On<FABookPeriod.finPeriodID, Equal<FABookHistory.finPeriodID>,
					And<FABookPeriod.bookID, Equal<FABookHistory.bookID>,
					And<FABookHistory.assetID, Equal<Current<FixedAsset.assetID>>>>>>,
				Where<FABookPeriod.bookID, Equal<Current<DeprBookFilter.bookID>>,
					And<FABookPeriod.finYear, GreaterEqual<Required<FABookPeriod.finYear>>,
					And<FABookPeriod.finYear, LessEqual<Required<FABookPeriod.finYear>>,
					And<FABookPeriod.startDate, NotEqual<FABookPeriod.endDate>>>>>,
				OrderBy<Asc<FABookPeriod.periodNbr, Asc<FABookPeriod.finYear>>>>.Select(this, bal.DeprFromYear, toYear))
			{
				FABookHistory hist = res;
				FABookPeriod period = res;

				if (dyn != null && dyn.PeriodNbr != period.PeriodNbr)
				{
					yield return dyn;
				}
				if (dyn == null || dyn.PeriodNbr != period.PeriodNbr)
				{
					dyn = new FASheetHistory
					{
						AssetID = bal.AssetID,
						PeriodNbr = period.PeriodNbr,
						PtdValues = new FASheetHistory.PeriodDeprPair[_SheetFieldNames.Count / 3],
					};
				}
				decimal? calculated;
				if (fasetup.Current.AccurateDepreciation == true)
				{
					if (bal.CurrDeprPeriod == null || string.Compare(hist.FinPeriodID, bal.CurrDeprPeriod) < 0)
					{
						calculated = hist.PtdDepreciated + hist.PtdAdjusted + hist.PtdDeprDisposed;
					}
					else if (string.CompareOrdinal(hist.FinPeriodID, bal.CurrDeprPeriod) > 0)
					{
						calculated = hist.PtdCalculated;
					}
					else //if (string.Compare(hist.FinPeriodID, bal.CurrDeprPeriod) == 0)
					{
						calculated = hist.YtdCalculated - hist.YtdDepreciated;
					}
				}
				else
				{
					calculated = hist.PtdCalculated;
				}

				int curYearInt;
				int.TryParse(period.FinYear, out curYearInt);

				dyn.PtdValues[curYearInt - fromYearInt] = new FASheetHistory.PeriodDeprPair(period.FinPeriodID, hist.PtdDepreciated + hist.PtdAdjusted + hist.PtdDeprDisposed, calculated);
			}
			yield return dyn;
		}

		public virtual IEnumerable assethistory()
		{
			string history_from = "207699";
			string history_to = "190001";

			AssetDetails.Current = AssetDetails.Select();
			if (AssetDetails.Current == null || AssetDetails.Current.AssetID < 0) yield break;

			Dictionary<int?, FABookBalance> balances = new Dictionary<int?, FABookBalance>();

			foreach (FABookBalance bookbal in AssetBalance.Select())
			{
				if (bookbal.Depreciate == true && (bookbal.DeprToDate != null || AssetDetails.Current.DepreciateFromDate != null))
				{
					string DeprFromPeriod = !string.IsNullOrEmpty(bookbal.DeprFromPeriod) ? bookbal.DeprFromPeriod : FABookPeriodIDAttribute.PeriodFromDate(this, bookbal.DeprToDate ?? AssetDetails.Current.DepreciateFromDate, bookbal.BookID);
					string DeprToPeriod = !string.IsNullOrEmpty(bookbal.DeprToPeriod) ? bookbal.DeprToPeriod : DeprFromPeriod;
					string LastDeprPeriod = !string.IsNullOrEmpty(bookbal.LastDeprPeriod) ? bookbal.LastDeprPeriod : DeprToPeriod;
					if (string.CompareOrdinal(DeprFromPeriod, history_from) < 0)
					{
						history_from = DeprFromPeriod;
					}
					if (string.CompareOrdinal(LastDeprPeriod, DeprToPeriod) > 0)
					{
						DeprToPeriod = LastDeprPeriod;
					}
					if (string.CompareOrdinal(DeprToPeriod, history_to) > 0)
					{
						history_to = DeprToPeriod;
					}
				}
				balances[bookbal.BookID] = bookbal;
			}

			if (string.CompareOrdinal(history_from, history_to) > 0)
			{
				yield break;
			}

			FAHistory dyn = null;
			foreach (PXResult<FABookPeriod, FABookHistory> res in PXSelectReadonly2<FABookPeriod,
												LeftJoin<FABookHistory,
													On<FABookHistory.assetID, Equal<Current<FixedAsset.assetID>>,
													And<FABookHistory.bookID, Equal<FABookPeriod.bookID>,
													And<FABookHistory.finPeriodID, Equal<FABookPeriod.finPeriodID>>>>>,
											Where<FABookPeriod.finPeriodID, GreaterEqual<Required<FABookPeriod.finPeriodID>>,
												And<FABookPeriod.finPeriodID, LessEqual<Required<FABookPeriod.finPeriodID>>,
												And<FABookPeriod.startDate, NotEqual<FABookPeriod.endDate>>>>,
											OrderBy<Asc<FABookPeriod.finPeriodID>>>.Select(this, history_from, history_to))
			{
				FABookPeriod period = res;
				FABookHistory hist = res;

				FABookBalance balance;
				if (!balances.TryGetValue(period.BookID, out balance) || balance.Depreciate != true)
					continue;

				if (dyn != null && dyn.FinPeriodID != period.FinPeriodID)
				{
					yield return dyn;
				}
				if (dyn == null || dyn.FinPeriodID != period.FinPeriodID)
				{
					dyn = new FAHistory
					{
						AssetID = Asset.Current.AssetID,
						FinPeriodID = period.FinPeriodID,
						PtdDepreciated = new decimal?[_Books.Count * 2]
					};
				}
				if (fasetup.Current.AccurateDepreciation == true)
				{
					if (balance.CurrDeprPeriod == null || string.Compare(hist.FinPeriodID, balance.CurrDeprPeriod) < 0)
					{
						dyn.PtdDepreciated[_Books[period.BookID] * 2] = hist.PtdDepreciated + hist.PtdAdjusted + hist.PtdDeprDisposed;
					}
					else if (string.Compare(hist.FinPeriodID, balance.CurrDeprPeriod) > 0)
					{
						dyn.PtdDepreciated[_Books[period.BookID] * 2] = hist.PtdCalculated;
					}
					else //if(string.Compare(hist.FinPeriodID, balance.CurrDeprPeriod) == 0)
					{
						dyn.PtdDepreciated[_Books[period.BookID] * 2] = hist.YtdCalculated - hist.YtdDepreciated;
					}
				}
				else
				{
					dyn.PtdDepreciated[_Books[period.BookID] * 2] = hist.PtdCalculated;
				}
				dyn.PtdDepreciated[_Books[period.BookID] * 2 + 1] = hist.PtdDepreciated + hist.PtdAdjusted + hist.PtdDeprDisposed;
			}
			yield return dyn;
		}
		#endregion

		#region View Delegates
		protected virtual IEnumerable fatransactions()
		{
			TranBookFilter filter = bookfilter.Current;

			PXSelectBase<FATran> cmd = new PXSelect<FATran, Where<FATran.assetID, Equal<Current<FixedAsset.assetID>>>,
								OrderBy<Asc<FATran.bookID, Asc<FATran.finPeriodID>>>>(this);

			if (filter.BookID != null)
			{
				cmd.WhereAnd<Where<FATran.bookID, Equal<Current<TranBookFilter.bookID>>>>();
			}
			return cmd.Select();
		}

		public virtual IEnumerable dspladditions()
		{
			Additions.View.Clear();

			int startRow = PXView.StartRow;
			int totalRows = 0;
			foreach (FAAccrualTran ext in Additions.View.Select(PXView.Currents, null, PXView.Searches, null, PXView.Descendings, PXView.Filters, ref startRow, PXView.MaximumRows, ref totalRows))
			{
				DsplFAATran dspl = new DsplFAATran
				{
					TranID = ext.TranID,
					GLTranQty = ext.GLTranQty,
					GLTranAmt = ext.GLTranAmt,
					ClosedQty = ext.ClosedQty,
					ClosedAmt = ext.ClosedAmt,
				};

				DsplAdditions.Cache.IsDirty = (DsplAdditions.Cache.GetStatus(dspl) != PXEntryStatus.Notchanged || DsplAdditions.Insert(dspl) == null) && DsplAdditions.Cache.IsDirty;

				dspl = DsplAdditions.Cache.Locate(dspl) as DsplFAATran;
				yield return new PXResult<DsplFAATran, FAAccrualTran>(dspl, ext);
			}
			PXView.StartRow = 0;
		}

		public override IEnumerable<PXDataRecord> ProviderSelect(BqlCommand command, int topCount, params PXDataValue[] pars)
		{
			Type[] tables = command.GetTables();
			if (tables != null && tables.Length > 1 && tables[0] == typeof(FAAccrualTran))
				command = command.OrderByNew<OrderBy<Asc<FAAccrualTran.gLTranID>>>();
			return base.ProviderSelect(command, topCount, pars);
		}

		public virtual IEnumerable additions()
		{
			return AssetGLTransactions.additions(this, GLTrnFilter.Current, Additions.Cache);
		}

		#endregion

		#region Buttons

		public PXAction<FixedAsset> runReversal;
		[PXUIField(DisplayName = Messages.Reverse, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXProcessButton]
		// TODO: Split parameters between appropriate actions (waiting for Andrew Boulanov)
		public virtual IEnumerable RunReversal(PXAdapter adapter,
			[PXDate]
			DateTime? disposalDate,
			[PXString]
			string disposalPeriodID,
			[PXBaseCury]
			decimal? disposalCost,
			[PXInt]
			int? disposalMethID,
			[PXInt]
			int? disposalAcctID,
			[PXInt]
			int? disposalSubID,
			[PXString]
			string dispAmtMode,
			[PXBool]
			bool? deprBeforeDisposal,
			[PXString]
			string reason,
			[PXInt]
			int? assetID
			)
		{
			if (adapter.MassProcess)
			{
				throw new NotImplementedException();
			}
			else
			{
				Save.Press();
				AssetProcess.CheckUnreleasedTransactions(this, (FixedAsset)Caches[typeof(FixedAsset)].Current);
				PXLongOperation.StartOperation(this, delegate { AssetProcess.ReverseAsset(CurrentAsset.Current, fasetup.Current); });
				return adapter.Get();
			}
		}

		public PXAction<FixedAsset> runDispReversal;
		[PXUIField(DisplayName = Messages.DispReverse, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXProcessButton]
		// TODO: Split parameters between appropriate actions (waiting for Andrew Boulanov)
		public virtual IEnumerable RunDispReversal(PXAdapter adapter,
			[PXDate]
			DateTime? disposalDate,
			[PXString]
			string disposalPeriodID,
			[PXBaseCury]
			decimal? disposalCost,
			[PXInt]
			int? disposalMethID,
			[PXInt]
			int? disposalAcctID,
			[PXInt]
			int? disposalSubID,
			[PXString]
			string dispAmtMode,
			[PXBool]
			bool? deprBeforeDisposal,
			[PXString]
			string reason,
			[PXInt]
			int? assetID
			)
		{
			if (!adapter.MassProcess)
			{
				if (RevDispInfo.View.Answer == WebDialogResult.None)
				{
					AssetDetails.Current = AssetDetails.Select();
					RevDispInfo.Cache.Remove(RevDispInfo.Current);
					RevDispInfo.Insert();

					if (AssetDetails.Current.DisposalDate != null && DateTime.Compare((DateTime)RevDispInfo.Current.ReverseDisposalDate, (DateTime)AssetDetails.Current.DisposalDate) < 0)
					{
						RevDispInfo.Current.ReverseDisposalDate = AssetDetails.Current.DisposalDate;
					}

					string PeriodID = FinPeriodIDAttribute.PeriodFromDate(RevDispInfo.Current.ReverseDisposalDate);
					if (PXSelectorAttribute.Select<ReverseDisposalInfo.reverseDisposalPeriodID>(RevDispInfo.Cache, RevDispInfo.Cache.Current, PeriodID) == null)
					{
						FinPeriod period = (FinPeriod)PXSelectorAttribute.SelectFirst<ReverseDisposalInfo.reverseDisposalPeriodID>(RevDispInfo.Cache, RevDispInfo.Cache.Current);
						if (period == null)
						{
							throw new PXException(Messages.ReverseDispPeriodNotFound, FinPeriodIDAttribute.FormatForError(RevDispInfo.Current.DisposalPeriodID));
						}
						PeriodID = period.FinPeriodID;
					}

					RevDispInfo.Current.ReverseDisposalPeriodID = PeriodID;

					RevDispInfo.AskExt();
				}
				else if (RevDispInfo.View.Answer != WebDialogResult.OK || !RevDispInfo.VerifyRequired())
				{
					return adapter.Get();
				}
			}
			else
			{
				throw new NotImplementedException();
			}
			Save.Press();
			PXLongOperation.StartOperation(this, delegate { AssetProcess.ReverseDisposal(CurrentAsset.Current, RevDispInfo.Current.ReverseDisposalDate, RevDispInfo.Current.ReverseDisposalPeriodID); });
			return adapter.Get();
		}

		public PXAction<FixedAsset> disposalOK;
		[PXUIField(DisplayName = "OK", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXProcessButton]
		public virtual IEnumerable DisposalOK(PXAdapter adapter)
		{
			DispParams.VerifyRequired();
			DisposeParams prm = (DisposeParams)DispParams.Cache.Current;
			if (DispParams.Cache.Fields.Select(field => PXUIFieldAttribute.GetError(DispParams.Cache, DispParams.Cache.Current, field)).Any(err => !string.IsNullOrEmpty(err)))
			{
				return adapter.Get();
			}

			FixedAsset asset = CurrentAsset.Current;
			FADetails det = PXSelect<FADetails, Where<FADetails.assetID, Equal<Current<FixedAsset.assetID>>>>.SelectSingleBound(this, new object[] { asset });
			PXResultset<FixedAsset, FADetails> list = new PXResultset<FixedAsset, FADetails>();
			list.Add(new PXResult<FixedAsset, FADetails>(asset, det));
			Dispose(list, det.CurrentCost ?? 0m, prm.DisposalDate, prm.DisposalPeriodID, prm.DisposalAmt, prm.DisposalMethodID,
				prm.DisposalAccountID, prm.DisposalSubID, null, prm.DeprBeforeDisposal, prm.Reason, false);
			return adapter.Get();
		}

		protected virtual void Dispose(
			PXResultset<FixedAsset, FADetails> list,
			decimal cost,
			DateTime? disposalDate,
			string disposalPeriodID,
			decimal? disposalCost,
			int? disposalMethID,
			int? disposalAcctID,
			int? disposalSubID,
			string dispAmtMode,
			bool? deprBeforeDisposal,
			string reason,
			bool isMassProcess)
		{
			foreach (PXResult<FixedAsset, FADetails> res in list)
			{
				FixedAsset asset = res;
				FADetails det = res;
				decimal assetDisposalAmt = cost == 0m
									   ? (disposalCost ?? 0m) / list.Count
									   : (disposalCost ?? 0m) * (det.CurrentCost ?? 0m) / cost;
				if (dispAmtMode == DisposalProcess.DisposalFilter.disposalAmtMode.Manual)
				{
					if (asset.DisposalAmt == null)
					{
						throw new PXException(Messages.DispAmtIsEmpty);
					}
					assetDisposalAmt = (decimal)asset.DisposalAmt;
				}

				asset.DisposalAccountID = disposalAcctID;
				asset.DisposalSubID = disposalSubID;
				det.DisposalDate = disposalDate;
				det.DisposalMethodID = disposalMethID;
				det.SaleAmount = assetDisposalAmt;
				det.DisposalPeriodID = disposalPeriodID;

				Asset.Update(asset);
				AssetDetails.Update(det);
				Save.Press();
			}
			PXLongOperation.StartOperation((PXGraph)this, delegate ()
			{
				DocumentList<FARegister> created = AssetProcess.DisposeAsset(list, fasetup.Current, false, deprBeforeDisposal == true, reason);

				if (!(fasetup.Current.AutoReleaseDisp ?? false) && isMassProcess)
				{
					AssetTranRelease graph = CreateInstance<AssetTranRelease>();
					AssetTranRelease.ReleaseFilter filter = (AssetTranRelease.ReleaseFilter)graph.Filter.Cache.CreateCopy(graph.Filter.Current);
					filter.Origin = FARegister.origin.Disposal;
					graph.Filter.Update(filter);
					graph.SelectTimeStamp();
					int i = 0;
					Dictionary<string, string> parameters = new Dictionary<string, string>();
					foreach (FARegister register in created)
					{
						register.Selected = true;
						graph.FADocumentList.Update(register);
						graph.FADocumentList.Cache.SetStatus(register, PXEntryStatus.Updated);
						graph.FADocumentList.Cache.IsDirty = false;
						parameters["FARegister.RefNbr" + i] = register.RefNbr;
						i++;
					}

					parameters["DateFrom"] = null;
					parameters["DateTo"] = null;

					PXReportRequiredException reportex = new PXReportRequiredException(parameters, "FA680010", "Preview");
					throw new PXRedirectWithReportException(graph, reportex, "Release FA Transaction");
				}
			});
		}

		public PXAction<FixedAsset> runDisposal;
		[PXUIField(DisplayName = Messages.Dispose, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXProcessButton]
		// TODO: Split parameters between appropriate actions (waiting for Andrew Boulanov)
		public virtual IEnumerable RunDisposal(PXAdapter adapter,
			[PXDate]
			DateTime? disposalDate,
			[PXString]
			string disposalPeriodID,
			[PXBaseCury]
			decimal? disposalCost,
			[PXInt]
			int? disposalMethID,
			[PXInt]
			int? disposalAcctID,
			[PXInt]
			int? disposalSubID,
			[PXString]
			string dispAmtMode,
			[PXBool]
			bool? deprBeforeDisposal,
			[PXString]
			string reason,
			[PXInt]
			int? assetID
			)
		{
			PXResultset<FixedAsset, FADetails> list = new PXResultset<FixedAsset, FADetails>();
			decimal sum = 0m;
			foreach (FixedAsset asset in adapter.Get())
			{
				FADetails det = PXSelect<FADetails, Where<FADetails.assetID, Equal<Current<FixedAsset.assetID>>>>.SelectSingleBound(this, new object[] { asset });
				list.Add(new PXResult<FixedAsset, FADetails>(asset, det));
				sum += det.CurrentCost ?? 0m;
			}
			if (list.Count == 0)
				return list;

			if (!adapter.MassProcess)
			{
				PXView.InitializePanel disposalInit = delegate (PXGraph graph, string viewName)
				{
					DisposeParams param = DispParams.Current;
					FADetails assetdet = list[0].GetItem<FADetails>();

					if (param.DisposalMethodID == null)
					{
						param.DisposalMethodID = assetdet.DisposalMethodID;
						param.DisposalDate = assetdet.DisposalDate;
						param.DisposalAmt = assetdet.SaleAmount ?? 0m;
						DispParams.Cache.SetDefaultExt<DisposeParams.disposalPeriodID>(param);
					}

					if (param.DisposalDate == null)
					{
						param.DisposalDate = Accessinfo.BusinessDate;
						DispParams.Cache.SetDefaultExt<DisposeParams.disposalPeriodID>(param);
					}

					if (param.DisposalAccountID == null)
					{
						DispParams.Cache.SetDefaultExt<DisposeParams.disposalAccountID>(param);
						DispParams.Cache.SetDefaultExt<DisposeParams.disposalSubID>(param);
					}
				};

				DispParams.AskExt(disposalInit);
				return list;
			}

			Dispose(list, sum, disposalDate, disposalPeriodID, disposalCost, disposalMethID, disposalAcctID,
						disposalSubID, dispAmtMode, deprBeforeDisposal, reason, adapter.MassProcess);
			return list;
		}

		public void GLTranFilterPeriodIDFieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			e.Cancel = true;
		}

		public PXAction<FixedAsset> runSplit;
		[PXUIField(DisplayName = Messages.Split, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXProcessButton]
		// TODO: Split parameters between appropriate actions (waiting for Andrew Boulanov)
		public virtual IEnumerable RunSplit(PXAdapter adapter,
			[PXDate]
			DateTime? disposalDate,
			[PXString]
			string disposalPeriodID,
			[PXBaseCury]
			decimal? disposalCost,
			[PXInt]
			int? disposalMethID,
			[PXInt]
			int? disposalAcctID,
			[PXInt]
			int? disposalSubID,
			[PXString]
			string dispAmtMode,
			[PXBool]
			bool? deprBeforeDisposal,
			[PXString]
			string reason,
			[PXInt]
			int? assetID
			)
		{
			List<SplitParams> list = new List<SplitParams>();
			if (adapter.MassProcess)
			{
				FixedAsset asset = PXSelect<FixedAsset, Where<FixedAsset.assetID, Equal<Required<FixedAsset.assetID>>>>.Select(this, assetID);
				AssetProcess.CheckUnreleasedTransactions(this, asset);
				FADetails det = PXSelect<FADetails, Where<FADetails.assetID, Equal<Required<FixedAsset.assetID>>>>.Select(this, assetID);
				FALocationHistory lochist = PXSelect<FALocationHistory, Where<FALocationHistory.assetID, Equal<Current<FADetails.assetID>>, And<FALocationHistory.revisionID, Equal<Current<FADetails.locationRevID>>>>>.SelectSingleBound(this, new object[] { det });
				FABookBalance bal = PXSelect<FABookBalance, Where<FABookBalance.assetID, Equal<Required<FixedAsset.assetID>>>>.Select(this, assetID);

				decimal tcost = 0m;
				decimal tqty = 0m;
				foreach (SplitParams parms in adapter.Get())
				{
					tcost += parms.Cost ?? 0m;
					if (tcost > bal.YtdDeprBase)
						throw new PXSetPropertyException(Messages.SplittedCostGreatherOrigin);
					tqty += parms.SplittedQty ?? 0m;
					list.Add(parms);
				}

				Dictionary<FixedAsset, decimal> ratio = new Dictionary<FixedAsset, decimal>();
				foreach (SplitParams split in list)
				{
					FixedAsset split_asset = (FixedAsset)Asset.Cache.CreateCopy(asset);
					split_asset.AssetID = null;
					split_asset.AssetCD = split.SplittedAssetCD;
					split_asset.ClassID = null;
					split_asset.Description = string.Format(string.IsNullOrEmpty(asset.Description) ? "{0} {1}" : "{2} - {0} {1}", Messages.SplitAssetDesc, asset.AssetCD, asset.Description);
					split_asset.Qty = split.SplittedQty;
					string assetCD = AssetGLTransactions.GetTempKey<FixedAsset.assetCD>(Asset.Cache);
					split_asset = Asset.Insert(split_asset);
					split_asset.AssetCD = split.SplittedAssetCD ?? assetCD;
					Asset.Cache.Normalize();
					split_asset.ClassID = asset.ClassID;
					split_asset.FAAccountID = asset.FAAccountID;
					split_asset.FASubID = asset.FASubID;
					split_asset.AccumulatedDepreciationAccountID = asset.AccumulatedDepreciationAccountID;
					split_asset.AccumulatedDepreciationSubID = asset.AccumulatedDepreciationSubID;
					split_asset.DepreciatedExpenseAccountID = asset.DepreciatedExpenseAccountID;
					split_asset.DepreciatedExpenseSubID = asset.DepreciatedExpenseSubID;
					split_asset.FAAccrualAcctID = asset.FAAccrualAcctID;
					split_asset.FAAccrualSubID = asset.FAAccrualSubID;
					split_asset.SplittedFrom = asset.AssetID;

					// To prevent verifying of GLTranFilter.PeriodID default value, AC-64486
					FieldVerifying.AddHandler<GLTranFilter.periodID>(GLTranFilterPeriodIDFieldVerifying);

					FADetails split_det = (FADetails)AssetDetails.Cache.CreateCopy(det);
					split_det.AssetID = split_asset.AssetID;
					split_det.AcquisitionCost = split.Cost;
					split_det.SalvageAmount = PXDBCurrencyAttribute.BaseRound(this, (decimal)(det.SalvageAmount * split.Ratio / 100m));
					split_det.LocationRevID = AssetDetails.Current.LocationRevID;
					split_det = AssetDetails.Update(split_det);

					FieldVerifying.RemoveHandler<GLTranFilter.periodID>(GLTranFilterPeriodIDFieldVerifying);

					FALocationHistory split_hist = (FALocationHistory)AssetLocation.Cache.CreateCopy(lochist);
					split_hist.AssetID = split_det.AssetID;
					split_hist.RevisionID = split_det.LocationRevID;
					split_hist.Department = lochist.Department;
					split_hist.PeriodID = lochist.PeriodID;
					split_hist = AssetLocation.Update(split_hist);

					foreach (FABookBalance bookbal in PXSelect<FABookBalance, Where<FABookBalance.assetID, Equal<Current<FixedAsset.assetID>>>>.SelectMultiBound(this, new object[] { asset }))
					{
						FABookBalance split_bal = (FABookBalance)AssetBalance.Cache.CreateCopy(bookbal);
						split_bal.AssetID = split_asset.AssetID;
						split_bal = AssetBalance.Insert(split_bal);
						split_bal.AcquisitionCost = split.Cost;
					}
					det.SalvageAmount -= split_det.SalvageAmount;
					det = AssetDetails.Update(det);

					Save.Press();
					split.SplittedAssetCD = split_asset.AssetCD;
					ratio.Add(split_asset, (decimal)split.Ratio);
				}
				asset.Qty -= tqty;
				if (asset.Qty <= 0m) asset.Qty = 1m;
				Asset.Update(asset);
				Save.Press();

				PXLongOperation.StartOperation(this, delegate
				{
					AssetProcess.SplitAsset(asset, disposalDate, disposalPeriodID, deprBeforeDisposal == true, ratio);
				});
				return list;
			}
			else
			{
				Save.Press();
				PXLongOperation.StartOperation(this, delegate ()
				{
					SplitProcess graph = CreateInstance<SplitProcess>();
					graph.Filter.Insert(new SplitProcess.SplitFilter() { AssetID = CurrentAsset.Current.AssetID });
					graph.Filter.Cache.IsDirty = false;
					throw new PXRedirectRequiredException(graph, true, "SplitProcess") { Mode = PXBaseRedirectException.WindowMode.Same };
				});

				return adapter.Get();
			}
		}

		public PXAction<FixedAsset> action;
		[PXUIField(DisplayName = "Actions", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		// TODO: Split parameters between appropriate actions (waiting for Andrew Boulanov)
		protected virtual IEnumerable Action(PXAdapter adapter,
			[PXDate]
			DateTime? disposalDate,
			[PXString]
			string disposalPeriodID,
			[PXBaseCury]
			decimal? disposalCost,
			[PXInt]
			int? disposalMethID,
			[PXInt]
			int? disposalAcctID,
			[PXInt]
			int? disposalSubID,
			[PXString]
			string dispAmtMode,
			[PXBool]
			bool? deprBeforeDisposal,
			[PXString]
			string reason,
			[PXInt]
			int? assetID
			)
		{
			return adapter.Get();
		}

		#region Button Create Lines
		public PXAction<FixedAsset> CalculateDepreciation;
		[PXUIField(DisplayName = Messages.CalculateDepreciation, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXProcessButton]
		public virtual IEnumerable calculateDepreciation(PXAdapter adapter)
		{
			FixedAsset fixedAsset = Asset.Current;
			FADetails assetDetails = AssetDetails.Select();

			if (fixedAsset != null && fixedAsset.ClassID != null && assetDetails != null && assetDetails.DepreciateFromDate != null)
			{
				Save.Press();
				DepreciationCalculation(fixedAsset, assetDetails);
			}
			Caches[typeof(FABookPeriod)].ClearQueryCache();
			Caches[typeof(FABookHistory)].ClearQueryCache();
			return adapter.Get();
		}
		#endregion
		#region Button Suspend
		public PXAction<FixedAsset> Suspend;
		[PXUIField(DisplayName = Messages.Suspend, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXProcessButton]
		public virtual IEnumerable suspend(PXAdapter adapter,
			[PXString(6)]
			string CurrentPeriodID)
		{
			List<FixedAsset> list = adapter.Get().Cast<FixedAsset>().ToList();

			if (adapter.MassProcess == false && list.Count > 0 && list[0].Suspended == true)
			{
				if (SuspendParams.View.Answer == WebDialogResult.None)
				{
					SuspendParams.Cache.SetDefaultExt<SuspendParams.currentPeriodID>(SuspendParams.Current);
				}

				if (SuspendParams.AskExt() != WebDialogResult.OK)
				{
					return adapter.Get();
				}

				CurrentPeriodID = SuspendParams.Current.CurrentPeriodID;
			}

			PXLongOperation.StartOperation(this, delegate ()
			{
				foreach (FixedAsset asset in list)
				{
					if (asset.Suspended == false)
					{
						AssetProcess.SuspendAsset(asset);
					}
					else
					{
						AssetProcess.UnsuspendAsset(asset, CurrentPeriodID);
					}
				}
			});

			return adapter.Get();
		}
		#endregion
		#region Button Dispose
		#endregion

		#region Button ProcessAdditions
		public PXAction<FixedAsset> ProcessAdditions;
		[PXUIField(DisplayName = Messages.ProcessAdditions, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXProcessButton]
		public virtual IEnumerable processAdditions(PXAdapter adapter)
		{
			FixedAsset asset = CurrentAsset.Current;
			FADetails det = AssetDetails.Current ?? AssetDetails.Select();
			GLTranFilter filter = GLTrnFilter.Current;

			decimal accrualBal = filter.AccrualBalance ?? 0m;
			decimal currCost = filter.CurrentCost ?? 0m;
			foreach (DsplFAATran dspl in DsplAdditions.Cache.Inserted)
			{
				if (dspl.Selected == true)
				{
					AssetGLTransactions.GLTran gltran = PXSelect<AssetGLTransactions.GLTran, Where<AssetGLTransactions.GLTran.tranID, Equal<Current<DsplFAATran.tranID>>>>.SelectSingleBound(this, new[] { dspl });
					FinPeriod glperiod = PXSelect<FinPeriod, Where<FinPeriod.fAClosed, NotEqual<True>,
																And<FinPeriod.startDate, NotEqual<FinPeriod.endDate>,
																And<FinPeriod.finPeriodID, GreaterEqual<Current<AssetGLTransactions.GLTran.finPeriodID>>>>>,
															 OrderBy<Asc<FinPeriod.finPeriodID>>>.SelectSingleBound(this, new object[] { gltran });
					if (dspl.Component == true)
					{
						FALocationHistory hist = AssetLocation.Select(asset.AssetID, det.LocationRevID);
						if (AssetLocation.Cache.GetStatus(hist) != PXEntryStatus.Notchanged)
						{
							throw new PXException(Messages.AssetIsNotSaved);
						}

						FixedAsset cls = PXSelect<FixedAsset, Where<FixedAsset.assetID, Equal<Current<DsplFAATran.classID>>>>.SelectSingleBound(this, new[] { dspl });
						if (cls == null)
						{
							throw new PXException(ErrorMessages.FieldIsEmpty, typeof(DsplFAATran.classID).Name);
						}

						AdditionsFATran procgraph = CreateInstance<AdditionsFATran>();
						procgraph.SelectTimeStamp();

						decimal unitCost = PXDBCurrencyAttribute.BaseRound(this, (dspl.SelectedAmt / dspl.SelectedQty) ?? 0m);
						for (int i = 0; i < dspl.SelectedQty; i++)
						{
							procgraph.InsertNewComponent(asset, cls, filter.TranDate, unitCost, 1m, hist, gltran);
						}
						procgraph.Actions.PressSave();

					}
					else
					{
						if (Register.Current == null || (Register.Current.Released ?? false))
						{
							AssetGLTransactions.SetCurrentRegister(Register, (int)asset.BranchID);
						}
						FATran tran = new FATran();
						foreach (FABookBalance bal in PXSelect<FABookBalance, Where<FABookBalance.assetID, Equal<Current<FixedAsset.assetID>>>>.Select(this))
						{
							FADepreciationMethod method = PXSelect<FADepreciationMethod, Where<FADepreciationMethod.methodID, Equal<Required<FADepreciationMethod.methodID>>>>.Select(this, bal.DepreciationMethodID);
							if (bal.Depreciate == true && method == null)
							{
								throw new PXException(Messages.DepreciationMethodDoesNotExist);
							}

							if (filter.TranDate == null)
							{
								throw new PXException(ErrorMessages.FieldIsEmpty, PXUIFieldAttribute.GetDisplayName<GLTranFilter.tranDate>(GLTrnFilter.Cache));
							}
							if (bal.UpdateGL == true && string.IsNullOrEmpty(filter.PeriodID))
							{
								throw new PXException(ErrorMessages.FieldIsEmpty, PXUIFieldAttribute.GetDisplayName<GLTranFilter.periodID>(GLTrnFilter.Cache));
							}
							if (bal.UpdateGL == true && glperiod == null)
							{
								throw new PXException(GL.Messages.NoOpenPeriod);
							}

							if (filter.ReconType == GLTranFilter.reconType.Addition)
							{
								tran = new FATran
								{
									AssetID = asset.AssetID,
									BookID = bal.BookID,
									TranAmt = dspl.SelectedAmt,
									Qty = dspl.SelectedQty,
									TranDate = gltran.TranDate,
									FinPeriodID = glperiod != null ? glperiod.FinPeriodID : gltran.FinPeriodID,
									GLTranID = bal.UpdateGL == true ? gltran.TranID : null,
									TranType = FATran.tranType.ReconciliationPlus,
									CreditAccountID = gltran.AccountID,
									CreditSubID = gltran.SubID,
									DebitAccountID = asset.FAAccrualAcctID,
									DebitSubID = asset.FAAccrualSubID,
									TranDesc = gltran.TranDesc,
								};
								tran = FATransactions.Insert(tran);

								if (tran.TranAmt + accrualBal > currCost)
								{
									tran = new FATran
									{
										AssetID = asset.AssetID,
										BookID = bal.BookID,
										TranAmt = tran.TranAmt + accrualBal - currCost,
										TranDate = filter.TranDate,
										FinPeriodID = bal.UpdateGL == true ? filter.PeriodID : null,
										TranType = FATran.tranType.PurchasingPlus,
										CreditAccountID = asset.FAAccrualAcctID,
										CreditSubID = asset.FAAccrualSubID,
										DebitAccountID = asset.FAAccountID,
										DebitSubID = asset.FASubID,
										TranDesc = gltran.TranDesc,
									};
									tran = FATransactions.Insert(tran);

									if (bal.Status == FixedAssetStatus.FullyDepreciated && bal.Depreciate == true && method.IsPureStraightLine)
									{
										tran = new FATran
										{
											AssetID = asset.AssetID,
											BookID = bal.BookID,
											TranAmt = tran.TranAmt,
											TranDate = filter.TranDate,
											FinPeriodID = filter.PeriodID,
											TranType = FATran.tranType.CalculatedPlus,
											CreditAccountID = asset.AccumulatedDepreciationAccountID,
											CreditSubID = asset.AccumulatedDepreciationSubID,
											DebitAccountID = asset.DepreciatedExpenseAccountID,
											DebitSubID = asset.DepreciatedExpenseSubID,
											TranDesc = gltran.TranDesc,
										};
										tran = FATransactions.Insert(tran);
									}
								}
							}
							else
							{
								tran = new FATran
								{
									AssetID = asset.AssetID,
									BookID = bal.BookID,
									TranAmt = dspl.SelectedAmt,
									Qty = dspl.SelectedQty,
									TranDate = gltran.TranDate,
									FinPeriodID = glperiod != null ? glperiod.FinPeriodID : gltran.FinPeriodID,
									GLTranID = bal.UpdateGL == true ? gltran.TranID : null,
									TranType = FATran.tranType.ReconciliationMinus,
									DebitAccountID = gltran.AccountID,
									DebitSubID = gltran.SubID,
									CreditAccountID = asset.FAAccrualAcctID,
									CreditSubID = asset.FAAccrualSubID,
									TranDesc = gltran.TranDesc,
								};
								FATransactions.Insert(tran);

								tran = new FATran
								{
									AssetID = asset.AssetID,
									BookID = bal.BookID,
									TranAmt = dspl.SelectedAmt,
									TranDate = filter.TranDate,
									FinPeriodID = bal.UpdateGL == true ? filter.PeriodID : null,
									TranType = FATran.tranType.PurchasingMinus,
									DebitAccountID = asset.FAAccrualAcctID,
									DebitSubID = asset.FAAccrualSubID,
									CreditAccountID = asset.FAAccountID,
									CreditSubID = asset.FASubID,
									TranDesc = gltran.TranDesc,
								};
								tran = FATransactions.Insert(tran);

								if (bal.Status == FixedAssetStatus.FullyDepreciated && bal.Depreciate == true && method.IsPureStraightLine)
								{
									tran = new FATran
									{
										AssetID = asset.AssetID,
										BookID = bal.BookID,
										TranAmt = tran.TranAmt,
										TranDate = filter.TranDate,
										FinPeriodID = filter.PeriodID,
										TranType = FATran.tranType.CalculatedMinus,
										CreditAccountID = asset.DepreciatedExpenseAccountID,
										CreditSubID = asset.DepreciatedExpenseSubID,
										DebitAccountID = asset.AccumulatedDepreciationAccountID,
										DebitSubID = asset.AccumulatedDepreciationSubID,
										TranDesc = gltran.TranDesc,
									};
									tran = FATransactions.Insert(tran);
								}
							}
						}
						accrualBal += dspl.SelectedAmt ?? 0m;
						currCost += tran.TranType == FATran.tranType.PurchasingPlus ? (tran.TranAmt ?? 0m) : 0m;
					}
				}
			}
			DsplAdditions.Cache.Clear();
			return adapter.Get();
		}
		#endregion

		public PXAction<FixedAsset> ReduceUnreconCost;
		[PXUIField(DisplayName = Messages.ReduceUnreconCost, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXProcessButton]
		public virtual IEnumerable reduceUnreconCost(PXAdapter adapter)
		{
			FixedAsset asset = CurrentAsset.Current;
			GLTranFilter filter = GLTrnFilter.Current;

			if (Register.Current == null || (Register.Current.Released ?? false))
			{
				AssetGLTransactions.SetCurrentRegister(Register, (int)asset.BranchID);
			}
			foreach (FABookBalance bal in PXSelect<FABookBalance, Where<FABookBalance.assetID, Equal<Current<FixedAsset.assetID>>>>.Select(this))
			{
				FATran tran = new FATran
				{
					AssetID = asset.AssetID,
					BookID = bal.BookID,
					TranAmt = filter.UnreconciledAmt,
					TranDate = filter.TranDate,
					TranType = FATran.tranType.PurchasingMinus,
					CreditAccountID = asset.FAAccountID,
					CreditSubID = asset.FASubID,
					DebitAccountID = asset.FAAccrualAcctID,
					DebitSubID = asset.FAAccrualSubID,
					TranDesc = PXMessages.LocalizeFormatNoPrefix(Messages.ReduceUnreconciledCost, asset.AssetCD),
				};
				FATransactions.Insert(tran);
			}

			return adapter.Get();
		}


		#endregion

		#region Functions
		private void DepreciationCalculation(FixedAsset fixedAsset, FADetails assetDetails)
		{
			if (fixedAsset.Depreciable != true) return;

			foreach (FABookBalance bookBalance in PXSelect<FABookBalance, Where<FABookBalance.assetID, Equal<Required<FABookBalance.assetID>>,
				And<FABookBalance.depreciationMethodID, IsNotNull>>>.Select(this, fixedAsset.AssetID))
			{
				DepreciationCalculation graph = CreateInstance<DepreciationCalculation>();
				graph.CalculateDepreciation(bookBalance);
			}
		}

		private static void CopyLocation(FALocationHistory from, FALocationHistory to)
		{
			to.LocationID = from.LocationID;
			to.BuildingID = from.BuildingID;
			to.Floor = from.Floor;
			to.Room = from.Room;
			to.EmployeeID = from.EmployeeID;
			to.Department = from.Department;
			to.Reason = from.Reason;
		}
		#endregion

		#region Header Events

		protected virtual void FixedAsset_RecordType_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = FARecordType.AssetType;
			e.Cancel = true;
		}

		protected virtual void FAClass_RecordType_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = FARecordType.ClassType;
			e.Cancel = true;
		}

		protected virtual void FADetails_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			FADetails det = (FADetails)e.Row;
			if (det == null) return;

			FATran tran = PXSelect<FATran, Where<FATran.assetID, Equal<Required<FADetails.assetID>>>, 
				OrderBy<Desc<Switch<Case<Where<FATran.origin, Equal<FARegister.origin.depreciation>>, intMax,
					Case<Where<FATran.origin, Equal<FARegister.origin.purchasing>>, int2,
					Case<Where<FATran.origin, Equal<FARegister.origin.reconcilliation>>, int1>>>, int0>,
					Desc<FATran.released, Asc<FATran.refNbr, Asc<FATran.lineNbr>>>>>>.SelectSingleBound(this, null, det.AssetID);

			bool depreciated = tran?.Origin == FARegister.origin.Depreciation;
			bool purchased = depreciated || ((tran?.Origin == FARegister.origin.Purchasing || tran?.Origin == FARegister.origin.Reconcilliation) && tran?.Released == true);

			PXUIFieldAttribute.SetEnabled<FADetails.receiptDate>(sender, det, !purchased);
			PXUIFieldAttribute.SetEnabled<FADetails.depreciateFromDate>(sender, det, !depreciated);
			PXUIFieldAttribute.SetEnabled<FADetails.tagNbr>(sender, det, fasetup.Current.TagNumberingID == null && fasetup.Current.CopyTagFromAssetID != true);
		}

		protected virtual void FADetails_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			FADetails assetdet = (FADetails)e.Row;

			if (!sender.ObjectsEqual<FADetails.receiptDate>(e.Row, e.OldRow))
			{
				PXResultset<FALocationHistory> locations = LocationHistory.SelectWindowed(0, 2);

				if (locations.Count == 1)
				{
					AssetLocation.Current = locations[0];
					AssetLocation.Current.TransactionDate = assetdet.ReceiptDate;
					AssetLocation.Update(AssetLocation.Current);
				}
			}
		}

		protected virtual void FixedAsset_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			FixedAsset asset = (FixedAsset)e.Row;
			if (asset == null) return;

			FADetails det = AssetDetails.SelectSingle() ?? AssetDetails.Insert();
			//Sync status for automation, GetStep-ApplyStep are called after RowSelected
			asset.Status = det.Status;
			Suspend.SetCaption(asset.Suspended == true ? Messages.Unsuspend : Messages.Suspend);
			Suspend.SetEnabled(asset.Depreciable == true);

			bool enabled = (det.Status != FixedAssetStatus.Disposed) && (det.Status != FixedAssetStatus.Reversed);
			sender.AllowUpdate = enabled;
			sender.AllowDelete = enabled;

			AssetBalance.Cache.AllowInsert = enabled;
			AssetBalance.Cache.AllowUpdate = enabled;
			AssetBalance.Cache.AllowDelete = enabled;
			AssetUsage.Cache.AllowInsert = asset.UsageScheduleID != null && enabled;
			AssetUsage.Cache.AllowUpdate = enabled;
			AssetUsage.Cache.AllowDelete = enabled;
			FATransactions.Cache.AllowDelete = enabled;

			bool isInserted = sender.GetStatus(asset) == PXEntryStatus.Inserted;
			AssetHistory.Cache.AllowInsert = !isInserted;
			BookSheetHistory.Cache.AllowInsert = !isInserted;

			CalculateDepreciation.SetEnabled(det.Status == FixedAssetStatus.Active && asset.Depreciable == true);
			runReversal.SetEnabled(det.Status == FixedAssetStatus.Active || det.Status == FixedAssetStatus.FullyDepreciated);

			bool updGL = fasetup.Current.UpdateGL ?? false;
			FABookBalance bal = PXSelect<FABookBalance, Where<FABookBalance.updateGL, Equal<True>, And<FABookBalance.assetID, Equal<Current<FixedAsset.assetID>>>>>.SelectSingleBound(this, new object[] { asset });
			bool faAccDisabled = bal?.InitPeriod != null;
			bool deprAccDisabled = bal?.LastDeprPeriod != null;
			FATran pplustran = PXSelect<FATran, Where<FATran.tranType, Equal<FATran.tranType.purchasingPlus>, And<FATran.assetID, Equal<Current<FixedAsset.assetID>>>>>.SelectSingleBound(this, new object[] { asset });

			PXUIFieldAttribute.SetEnabled<FixedAsset.fAAccountID>(sender, asset, !(faAccDisabled && updGL));
			PXUIFieldAttribute.SetEnabled<FixedAsset.fASubID>(sender, asset, !(faAccDisabled && updGL));
			PXUIFieldAttribute.SetEnabled<FixedAsset.fAAccrualAcctID>(sender, asset, !(pplustran != null && updGL));
			PXUIFieldAttribute.SetEnabled<FixedAsset.fAAccrualSubID>(sender, asset, !(pplustran != null && updGL));
			PXUIFieldAttribute.SetEnabled<FixedAsset.accumulatedDepreciationAccountID>(sender, asset, !(deprAccDisabled && updGL) && asset.Depreciable == true);
			PXUIFieldAttribute.SetEnabled<FixedAsset.accumulatedDepreciationSubID>(sender, asset, !(deprAccDisabled && updGL) && asset.Depreciable == true);

			FADepreciationMethod ulMeth = PXSelectJoin<FADepreciationMethod, InnerJoin<FABookBalance, On<FADepreciationMethod.methodID, Equal<FABookBalance.depreciationMethodID>>>, Where<FADepreciationMethod.usefulLife, IsNotNull, And<FABookBalance.assetID, Equal<Current<FixedAsset.assetID>>>>>.SelectSingleBound(this, new object[] { asset });
			bool inactive = ((FABookBalance)PXSelect<FABookBalance, Where<FABookBalance.assetID, Equal<Current<FixedAsset.assetID>>, And<FABookBalance.status, NotEqual<FixedAssetStatus.active>>>>.SelectSingleBound(this, new object[] { asset })) != null;
			PXUIFieldAttribute.SetEnabled<FixedAsset.usefulLife>(sender, asset, ulMeth == null && !inactive);

			bool depreciate = asset.Depreciable == true;

			PXUIFieldAttribute.SetVisible<DisposeParams.deprBeforeDisposal>(DispParams.Cache, null, depreciate);

			PXUIFieldAttribute.SetDisplayName<FABookBalance.deprFromDate>(AssetBalance.Cache, depreciate ? Messages.DeprFromDate : Messages.PlacedInServiceDate);
			PXUIFieldAttribute.SetDisplayName<FABookBalance.deprFromPeriod>(AssetBalance.Cache, depreciate ? Messages.DeprFromPeriod : Messages.PlacedInServicePeriod);
			PXUIFieldAttribute.SetVisible<FABookBalance.deprToDate>(AssetBalance.Cache, null, depreciate);
			PXUIFieldAttribute.SetVisible<FABookBalance.deprToPeriod>(AssetBalance.Cache, null, depreciate);
			PXUIFieldAttribute.SetVisible<FABookBalance.lastDeprPeriod>(AssetBalance.Cache, null, depreciate);
			PXUIFieldAttribute.SetVisible<FABookBalance.salvageAmount>(AssetBalance.Cache, null, depreciate);
			PXUIFieldAttribute.SetVisible<FABookBalance.ytdDepreciated>(AssetBalance.Cache, null, depreciate);
			PXUIFieldAttribute.SetVisible<FABookBalance.averagingConvention>(AssetBalance.Cache, null, depreciate);
			PXUIFieldAttribute.SetVisible<FABookBalance.tax179Amount>(AssetBalance.Cache, null, depreciate);
			PXUIFieldAttribute.SetVisible<FABookBalance.ytdTax179Recap>(AssetBalance.Cache, null, depreciate);
			PXUIFieldAttribute.SetVisible<FABookBalance.bonusID>(AssetBalance.Cache, null, depreciate);
			PXUIFieldAttribute.SetVisible<FABookBalance.bonusRate>(AssetBalance.Cache, null, depreciate);
			PXUIFieldAttribute.SetVisible<FABookBalance.bonusAmount>(AssetBalance.Cache, null, depreciate);
			PXUIFieldAttribute.SetVisible<FABookBalance.ytdBonusRecap>(AssetBalance.Cache, null, depreciate);
			PXUIFieldAttribute.SetVisible<FABookBalance.depreciationMethodID>(AssetBalance.Cache, null, depreciate);
			PXUIFieldAttribute.SetVisible<FABookBalance.midMonthType>(AssetBalance.Cache, null, depreciate);
			PXUIFieldAttribute.SetVisible<FABookBalance.midMonthDay>(AssetBalance.Cache, null, depreciate);

			AssetHistory.AllowSelect = asset.Depreciable == true && fasetup.Current.DeprHistoryView == FASetup.deprHistoryView.SideBySide;
			BookSheetHistory.AllowSelect = deprbookfilter.AllowSelect = asset.Depreciable == true && fasetup.Current.DeprHistoryView == FASetup.deprHistoryView.BookSheet;
		}

		protected virtual void FixedAsset_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			AssetDetails.Cache.Insert();
			AssetLocation.Cache.Insert();
			AssetLocation.Cache.IsDirty = false;
			AssetDetails.Cache.IsDirty = false;
		}
		protected virtual void FixedAsset_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			FixedAsset header = (FixedAsset)e.Row;
			if (header.ClassID == null)
			{
				sender.RaiseExceptionHandling<FixedAsset.classID>(header, header.ClassID, new PXSetPropertyException(Messages.ValueCanNotBeEmpty));
			}
			_PersistedAsset = header;
		}

		protected virtual void FixedAsset_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			FixedAsset asset = (FixedAsset)e.Row;
			if (asset == null) return;

			if (null != (FATran)PXSelect<FATran, Where<FATran.assetID, Equal<Current<FixedAsset.assetID>>, And<FATran.batchNbr, IsNotNull>>>.SelectSingleBound(this, new object[] { asset }))
			{
				throw new PXSetPropertyException(Messages.BalanceRecordCannotBeDeleted);
			}

			this.EnsureCachePersistence(typeof(FARegister));
			this.EnsureCachePersistence(typeof(FABookHistory));

			foreach (FARegister reg in PXSelectJoinGroupBy<FARegister,
				LeftJoin<FATran, On<FARegister.refNbr, Equal<FATran.refNbr>>>,
				Where<FATran.assetID, Equal<Required<FixedAsset.assetID>>>,
				Aggregate<GroupBy<FARegister.refNbr>>>.Select(this, asset.AssetID))
			{
				this.Caches<FARegister>().Delete(reg);
			}

			foreach (FABookHistory hist in PXSelect<FABookHistory, Where<FABookHistory.assetID, Equal<Required<FixedAsset.assetID>>>>.Select(this, asset.AssetID))
			{
				this.Caches<FABookHistory>().Delete(hist);
			}
		}

		protected virtual void CheckNewUsefulLife(PXCache sender, FABookBalance bal, decimal usefulLife)
		{
			FABookBalance copy = (FABookBalance)sender.CreateCopy(bal);
			copy.UsefulLife = usefulLife;
			sender.SetDefaultExt<FABookBalance.recoveryPeriod>(copy);
			sender.SetDefaultExt<FABookBalance.deprToPeriod>(copy);
			if (copy.LastDeprPeriod != null && copy.DeprToPeriod != null && string.CompareOrdinal(copy.LastDeprPeriod, copy.DeprToPeriod) >= 0)
			{
				throw new PXSetPropertyException(Messages.CannotChangeUsefulLife, FinPeriodIDFormattingAttribute.FormatForError(copy.DeprToPeriod), FinPeriodIDFormattingAttribute.FormatForError(copy.LastDeprPeriod));
			}
		}

		protected virtual void FixedAsset_UsefulLife_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			FixedAsset asset = (FixedAsset)e.Row;
			if (asset.Depreciable == true && e.NewValue == null)
			{
				throw new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(FixedAsset.usefulLife).Name);
			}
			if (asset.Depreciable == true && (decimal)e.NewValue == 0m)
			{
				throw new PXSetPropertyException(CS.Messages.Entry_GT, "0");
			}
			foreach (FABookBalance bal in PXSelect<FABookBalance, Where<FABookBalance.assetID, Equal<Current<FixedAsset.assetID>>>>.SelectMultiBound(this, new object[] { asset }))
			{
				CheckNewUsefulLife(AssetBalance.Cache, bal, (decimal)e.NewValue);
			}
		}

		protected virtual void FABookBalance_UsefulLife_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			FABookBalance bal = e.Row as FABookBalance;
			if (bal == null) return;
			if (bal.Depreciate == true)
			{
				if (e.NewValue == null)
				{
					throw new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(FABookBalance.usefulLife).Name);
				}
				if ((decimal)e.NewValue == 0m)
				{
					throw new PXSetPropertyException(CS.Messages.Entry_GT, "0");
				}
			}
			CheckNewUsefulLife(sender, (FABookBalance)e.Row, (decimal)e.NewValue);
		}

		protected virtual void FixedAsset_UsefulLife_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			UpdateBalances<FABookBalance.usefulLife, FixedAsset.usefulLife>(sender, e);
		}

		protected virtual void FixedAsset_Depreciable_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			UpdateBalances<FABookBalance.depreciate, FixedAsset.depreciable>(sender, e);
		}

		protected virtual void FixedAsset_ParentAssetID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			FixedAsset asset = (FixedAsset)e.Row;
			if (asset == null) return;

			PXSelectBase<FixedAsset> cmd = new PXSelect<FixedAsset, Where<FixedAsset.assetID, Equal<Required<FixedAsset.parentAssetID>>>>(this);
			int? parentID = (int?)e.NewValue;
			string parentCD = null;
			while (parentID != null)
			{
				FixedAsset parent = cmd.Select(parentID);
				parentCD = parentCD ?? parent.AssetCD;
				if (parent.ParentAssetID == asset.AssetID)
				{
					e.NewValue = asset.ParentAssetID != null ? ((FixedAsset)cmd.Select(asset.ParentAssetID)).AssetCD : null;
					throw new PXSetPropertyException(Messages.CyclicParentRef, parentCD);
				}
				parentID = parent.ParentAssetID;
			}
		}

		public static int? MakeSubID<MaskField, SubIDField>(PXCache sender, FixedAsset asset)
			where MaskField : IBqlField
			where SubIDField : IBqlField
		{
			FAClass cls = PXSelect<FAClass, Where<FAClass.assetID, Equal<Current<FixedAsset.classID>>>>.SelectSingleBound(sender.Graph, new object[] { asset });
			if (cls == null) return null;

			FADetails det = PXSelect<FADetails, Where<FADetails.assetID, Equal<Current<FixedAsset.assetID>>>>.SelectSingleBound(sender.Graph, new object[] { asset });
			FALocationHistory hist = PXSelect<FALocationHistory, Where<FALocationHistory.assetID, Equal<Current<FADetails.assetID>>,
											And<FALocationHistory.revisionID, Equal<Current<FADetails.locationRevID>>>>>.SelectSingleBound(sender.Graph, new object[] { asset, det });

			Location loc = PXSelectJoin<Location, LeftJoin<BAccount, On<Location.bAccountID, Equal<BAccount.bAccountID>, And<Location.locationID, Equal<BAccount.defLocationID>>>, LeftJoin<Branch, On<Branch.bAccountID, Equal<BAccount.bAccountID>>>>, Where<Branch.branchID, Equal<Current<FALocationHistory.locationID>>>>.SelectSingleBound(sender.Graph, new object[] { hist });
			EPDepartment dep = PXSelect<EPDepartment, Where<EPDepartment.departmentID, Equal<Current<FALocationHistory.department>>>>.SelectSingleBound(sender.Graph, new object[] { hist });

			string masksub = (string)sender.Graph.Caches[typeof(FAClass)].GetValue<MaskField>(cls);
			int? asset_subID = (int?)sender.Graph.Caches[typeof(FixedAsset)].GetValue<SubIDField>(asset);
			int? loc_subID = (int?)sender.Graph.Caches[typeof(Location)].GetValue<Location.cMPExpenseSubID>(loc);
			int? dep_subID = (int?)sender.Graph.Caches[typeof(EPDepartment)].GetValue<EPDepartment.expenseSubID>(dep);
			int? cls_subID = (int?)sender.Graph.Caches[typeof(FixedAsset)].GetValue<SubIDField>(cls);


			object value = SubAccountMaskAttribute.MakeSub<MaskField>(sender.Graph, masksub,
																	  new object[] { asset_subID, loc_subID, dep_subID, cls_subID },
																	  new[] { typeof(SubIDField), typeof(Location.cMPExpenseSubID), typeof(EPDepartment.expenseSubID), typeof(SubIDField) });

			sender.RaiseFieldUpdating<SubIDField>(asset, ref value);

			return (int?)value;
		}

		protected static void FASubIDFieldDefaulting<MaskField, SubIDField>(PXCache sender, PXFieldDefaultingEventArgs e)
			where MaskField : IBqlField
			where SubIDField : IBqlField
		{
			FixedAsset asset = (FixedAsset)e.Row;
			e.NewValue = MakeSubID<MaskField, SubIDField>(sender, asset);
			e.Cancel = true;
		}

		protected virtual void FixedAsset_ClassID_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
		{
			FixedAsset asset = (FixedAsset)e.Row;
			if (asset?.ClassID == null) return;

			FixedAsset cls = PXSelect<FixedAsset, Where<FixedAsset.assetID, Equal<Current<FixedAsset.classID>>>>.SelectSingleBound(this, new object[] { asset });
			if (cls != null && string.CompareOrdinal((string)e.NewValue, cls.AssetCD) != 0)
			{
				WebDialogResult answer = CurrentAsset.Ask(asset, GL.Messages.ImportantConfirmation, Messages.FAClassChangeConfirmation, MessageButtons.YesNo, MessageIcon.Question);
				if (answer == WebDialogResult.No)
				{
					e.NewValue = cls.AssetCD;
				}
			}
		}

		protected virtual void FixedAsset_ClassID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			FixedAsset currAsset = (FixedAsset)e.Row;

			if (e.OldValue != null) return;

			sender.SetDefaultExt<FixedAsset.isTangible>(e.Row);
			sender.SetDefaultExt<FixedAsset.assetTypeID>(e.Row);
			sender.SetDefaultExt<FixedAsset.usefulLife>(e.Row);
			sender.SetDefaultExt<FixedAsset.serviceScheduleID>(e.Row);
			sender.SetDefaultExt<FixedAsset.usageScheduleID>(e.Row);
			sender.SetDefaultExt<FixedAsset.constructionAccountID>(e.Row);
			sender.SetDefaultExt<FixedAsset.constructionSubID>(e.Row);
			sender.SetDefaultExt<FixedAsset.fAAccountID>(e.Row);
			sender.SetDefaultExt<FixedAsset.fASubID>(e.Row);
			sender.SetDefaultExt<FixedAsset.accumulatedDepreciationAccountID>(e.Row);
			sender.SetDefaultExt<FixedAsset.accumulatedDepreciationSubID>(e.Row);
			sender.SetDefaultExt<FixedAsset.depreciatedExpenseAccountID>(e.Row);
			sender.SetDefaultExt<FixedAsset.depreciatedExpenseSubID>(e.Row);
			sender.SetDefaultExt<FixedAsset.disposalAccountID>(e.Row);
			sender.SetDefaultExt<FixedAsset.disposalSubID>(e.Row);
			sender.SetDefaultExt<FixedAsset.gainAcctID>(e.Row);
			sender.SetDefaultExt<FixedAsset.gainSubID>(e.Row);
			sender.SetDefaultExt<FixedAsset.lossAcctID>(e.Row);
			sender.SetDefaultExt<FixedAsset.lossSubID>(e.Row);

			bool updateGL = (PXSelect<FABookSettings, Where<FABookSettings.assetID, Equal<Current<FixedAsset.classID>>>>
				.SelectMultiBound(this, new[] { e.Row })
				.RowCast<FABookSettings>()
				.Select(sett => AssetBalance.Insert(new FABookBalance
				{
					AssetID = currAsset.AssetID,
					ClassID = currAsset.ClassID,
					BookID = sett.BookID,
					AveragingConvention = sett.AveragingConvention,
				})))
				.Aggregate(false, (current, bal) => current || (bal.UpdateGL == true));

			AssetBalance.Cache.IsDirty = false;

			FAClass cls = PXSelect<FAClass, Where<FAClass.assetID, Equal<Required<FixedAsset.classID>>>>.SelectSingleBound(sender.Graph, null, currAsset.ClassID);
			if (cls?.HoldEntry == true)
			{
				FADetails det = AssetDetails.Select();
				det.Hold = true;
				det.Status = FixedAssetStatus.Hold;
				AssetDetails.Cache.SmartSetStatus(det, PXEntryStatus.Updated);
			}
		}

		protected virtual void FADetails_Hold_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if ((bool?)e.NewValue == false)
			{
				FABookBalance balance = AssetBalance.Select()
					.RowCast<FABookBalance>()
					.Where(bookbal => fasetup.Current.UpdateGL == true && bookbal.UpdateGL == true && string.IsNullOrEmpty(bookbal.InitPeriod))
					.Select(bookbal => new
					{
						bookbal,
						AcquiredCost = PXSelect<FATran, Where<FATran.assetID, Equal<Current<FABookBalance.assetID>>,
							And<FATran.bookID, Equal<Current<FABookBalance.bookID>>,
								And<FATran.tranType, Equal<FATran.tranType.purchasingPlus>>>>>.SelectMultiBound(this, new object[] { bookbal })
								.RowCast<FATran>()
								.Aggregate<FATran, decimal?>(0m, (current, tran) => current + tran.TranAmt)
					})
					.Where(t => t.bookbal.AcquisitionCost - t.AcquiredCost >= 0.00005m).Select(t => t.bookbal).FirstOrDefault();
				if (balance != null)
				{
					throw new PXSetPropertyException(Messages.UnholdAssetOutOfBalance, PXCurrencyAttribute.BaseRound(this, balance.AcquisitionCost));
				}
			}
			else
			{
				switch (((FADetails)e.Row).Status)
				{
					case FixedAssetStatus.FullyDepreciated:
					case FixedAssetStatus.Disposed:
						e.NewValue = false;
						break;
				}
			}
		}

		protected virtual void FADetails_Hold_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if (((FADetails)e.Row).Hold == true)
			{
				sender.SetValue<FADetails.status>(e.Row, FixedAssetStatus.Hold);
			}
			else if (CurrentAsset.Current.Suspended == true)
			{
				sender.SetValue<FADetails.status>(e.Row, FixedAssetStatus.Suspended);
			}
			else
			{
				sender.SetValue<FADetails.status>(e.Row, FixedAssetStatus.Active);
			}
		}
		#endregion

		#region Detail Events
		public static void UpdateBalances<Field, ParentField>(PXCache sender, PXFieldUpdatedEventArgs e)
			where Field : IBqlField
			where ParentField : IBqlField
		{
			Type balType = typeof(Field).DeclaringType;
			PXCache cache = sender.Graph.Caches[balType];

			BqlCommand cmd = BqlCommand.CreateInstance(BqlCommand.Compose(typeof(Select<,>), balType, typeof(Where<,>), cache.GetBqlField<FixedAsset.assetID>(), typeof(Equal<Current<FixedAsset.assetID>>)));

			foreach (object bal in new PXView(sender.Graph, false, cmd).SelectMulti())
			{
				cache.RaiseRowSelected(bal);

				PXFieldState state;
				if ((state = (PXFieldState)cache.GetStateExt<Field>(bal)) != null && state.Enabled)
				{
					object newBal = cache.CreateCopy(bal);
					cache.SetValue<Field>(newBal, sender.GetValue<ParentField>(e.Row));
					try
					{
						FABookBalance newBalance = newBal as FABookBalance;
						if (newBalance != null && typeof(ParentField) == typeof(FADetails.depreciateFromDate) && newBalance.DeprFromDate != null)
						{
							IDepreciationCalculation calc = sender.Graph as IDepreciationCalculation ?? new DepreciationCalc();
							DepreciationCalc.SetParameters(sender.Graph, calc, newBalance);
						}
						cache.Update(newBal);
					}
					catch (PXSetPropertyException ex)
					{
						if (ex.InnerException is TranDateOutOfRangeException)
						{
							throw new PXSetPropertyException(Messages.TranDateOutOfRange, cache.GetValueExt<FABookBalance.bookID>(bal));
						}
						throw;
					}
				}
			}
		}

		protected virtual void FADetails_DepreciateFromDate_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			FADetails det = e.Row as FADetails;

			if (det?.ReceiptDate != null && e.NewValue != null && 
				det.ReceiptDate.Value.CompareTo(e.NewValue) > 0)
			{
				throw new PXSetPropertyException(CS.Messages.Entry_GE, det.ReceiptDate.Value.ToShortDateString());
			}
		}

		protected virtual void FADetails_DepreciateFromDate_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			try
			{
				UpdateBalances<FABookBalance.deprFromDate, FADetails.depreciateFromDate>(sender, e);
				sender.RaiseExceptionHandling<FADetails.depreciateFromDate>(e.Row, ((FADetails)(e.Row)).DepreciateFromDate, null);
			}
			catch (PXException ex)
			{
				sender.SetValue<FADetails.depreciateFromDate>(e.Row, e.OldValue);
				sender.RaiseExceptionHandling<FADetails.depreciateFromDate>(e.Row, e.OldValue, ex);
			}

			GLTrnFilter.Cache.SetDefaultExt<GLTranFilter.tranDate>(GLTrnFilter.Current);
		}

		protected virtual void FADetails_AcquisitionCost_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			UpdateBalances<FABookBalance.acquisitionCost, FADetails.acquisitionCost>(sender, e);

			GLTrnFilter.Cache.SetDefaultExt<GLTranFilter.acquisitionCost>(GLTrnFilter.Current);
		}

		protected virtual void FADetails_SalvageAmount_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			UpdateBalances<FABookBalance.salvageAmount, FADetails.salvageAmount>(sender, e);
		}

		protected virtual void FADetails_SalvageAmount_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			FADetails det = (FADetails)e.Row;

			if ((decimal?) e.NewValue > det.AcquisitionCost)
			{
				throw new PXSetPropertyException(CS.Messages.Entry_LE, det.AcquisitionCost);
			}
		}

		protected virtual void FADetails_TagNbr_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			FixedAsset asset = (FixedAsset)PXParentAttribute.SelectParent(sender, e.Row);
			if (asset?.AssetCD != null && fasetup.Current.CopyTagFromAssetID == true)
			{
				e.NewValue = asset.AssetCD;
				e.Cancel = true;
			}
		}

		protected virtual void FADetails_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			FADetails det = (FADetails)e.Row;
			if (det != null && fasetup.Current.CopyTagFromAssetID == true && (e.Operation & PXDBOperation.Command) == PXDBOperation.Insert)
			{
				det.TagNbr = _PersistedAsset.AssetCD;
			}
		}

		protected virtual void FASetup_RowSelecting(PXCache sender, PXRowSelectingEventArgs e)
		{
			if (e.Row != null)
			{
				FASetup setup = (FASetup)e.Row;
				if (setup.CopyTagFromAssetID == true)
				{
					setup.TagNumberingID = null;
				}
			}
		}

		#endregion
		#region FABookBalance Events
		protected virtual void FABookBalance_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			FABookBalance bookbal = (FABookBalance)e.Row;

			bool acquired = !string.IsNullOrEmpty(bookbal.InitPeriod);

			if (acquired && e.ExternalCall)
			{
				throw new PXSetPropertyException(Messages.BalanceRecordCannotBeDeleted);
			}
		}

		protected virtual void FABookBalance_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			FABookBalance bookbal = (FABookBalance)e.Row;
			if (bookbal == null) return;

			bool depreciated = false;
			bool acquired = !string.IsNullOrEmpty(bookbal.InitPeriod);
			bool newitem = sender.GetStatus(e.Row) == PXEntryStatus.Inserted && fasetup.Current.UpdateGL != true;

			if (!newitem)
			{
				FATran other = PXSelectReadonly<FATran, Where<FATran.assetID, Equal<Required<FABookBalance.assetID>>,
						And<FATran.bookID, Equal<Required<FABookBalance.bookID>>>>,
					OrderBy<Desc<Switch<Case<Where<FATran.origin, Equal<FARegister.origin.depreciation>>, intMax,
						Case<Where<FATran.origin, Equal<FARegister.origin.purchasing>>, int2,
						Case<Where<FATran.origin, Equal<FARegister.origin.reconcilliation>>, int1>>>, int0>,
						Desc<FATran.released, Asc<FATran.refNbr, Asc<FATran.lineNbr>>>>>>.SelectSingleBound(this, null, bookbal.AssetID, bookbal.BookID);

				depreciated = other?.Origin == FARegister.origin.Depreciation;
				bool purchased = depreciated || ((other?.Origin == FARegister.origin.Purchasing || other?.Origin == FARegister.origin.Reconcilliation) && other?.Released == true);
				newitem = !purchased && fasetup.Current.UpdateGL != true;
			}

			PXUIFieldAttribute.SetEnabled<FABookBalance.lastDeprPeriod>(sender, e.Row, newitem && bookbal.Depreciate == true);
			PXUIFieldAttribute.SetEnabled<FABookBalance.ytdDepreciated>(sender, e.Row, newitem && bookbal.Depreciate == true);
			PXUIFieldAttribute.SetEnabled<FABookBalance.deprFromDate>(sender, e.Row, !depreciated);
			PXUIFieldAttribute.SetEnabled<FABookBalance.acquisitionCost>(sender, e.Row, !acquired || newitem);

			PXUIFieldAttribute.SetEnabled<FABookBalance.tax179Amount>(sender, e.Row, !acquired && bookbal.Depreciate == true);
			PXUIFieldAttribute.SetEnabled<FABookBalance.bonusAmount>(sender, e.Row, !acquired && bookbal.Depreciate == true);
			PXUIFieldAttribute.SetEnabled<FABookBalance.bonusRate>(sender, e.Row, !acquired && bookbal.Depreciate == true);
			PXUIFieldAttribute.SetEnabled<FABookBalance.bonusID>(sender, e.Row, !acquired && bookbal.Depreciate == true);

			if (bookbal.Status == FixedAssetStatus.Disposed)
			{
				PXUIFieldAttribute.SetEnabled(sender, bookbal, false);
			}

			IYearSetup yearSetup = FABookPeriodIDAttribute.GetBookCalendar(this, bookbal.BookID);
			FADepreciationMethod meth = PXSelect<FADepreciationMethod, Where<FADepreciationMethod.methodID, Equal<Current<FABookBalance.depreciationMethodID>>>>.SelectSingleBound(this, new object[] { bookbal });

			PXUIFieldAttribute.SetEnabled<FABookBalance.usefulLife>(sender, bookbal, bookbal.Status == FixedAssetStatus.Active && meth != null && meth.UsefulLife == null);
			PXUIFieldAttribute.SetEnabled<FABookBalance.averagingConvention>(sender, bookbal, !DepreciationCalc.IsTableMethod(meth));

			List<KeyValuePair<object, Dictionary<object, string[]>>> parsList = new List<KeyValuePair<object, Dictionary<object, string[]>>>();
			if (meth != null)
			{
				parsList.Add(meth.IsTableMethod == true
					? new KeyValuePair<object, Dictionary<object, string[]>>(meth.RecordType, FAAveragingConvention.RecordTypeDisabledValues)
					: new KeyValuePair<object, Dictionary<object, string[]>>(meth.DepreciationMethod, FAAveragingConvention.DeprMethodDisabledValues));
			}
			if (yearSetup != null)
			{
				parsList.Add(new KeyValuePair<object, Dictionary<object, string[]>>(yearSetup.IsFixedLengthPeriod, FAAveragingConvention.FixedLengthPeriodDisabledValues));
			}

			FAAveragingConvention.SetAveragingConventionsList<FADepreciationMethod.averagingConvention>(sender, bookbal, parsList.ToArray());
			PXUIFieldAttribute.SetRequired<FABookBalance.deprFromDate>(sender, bookbal.Depreciate == true);

			FABookSettings booksettings = PXSelect<FABookSettings, Where<FABookSettings.assetID, Equal<Current<FixedAsset.classID>>,
				And<FABookSettings.bookID, Equal<Required<FABookBalance.bookID>>>>>.Select(this, bookbal.BookID);
			PXUIFieldAttribute.SetEnabled<FABookBalance.tax179Amount>(sender, bookbal, booksettings?.Sect179 == true);
			PXUIFieldAttribute.SetEnabled<FABookBalance.ytdTax179Recap>(sender, bookbal, booksettings?.Sect179 == true);

			bookbal.AllowChangeDeprFromPeriod = !depreciated;
		}

		protected virtual void FABookBalance_DeprFromDate_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			FABookBalance bal = e.Row as FABookBalance;
			if (bal == null)
				return;

			if (AssetDetails.Current?.ReceiptDate != null && e.NewValue != null && 
				AssetDetails.Current.ReceiptDate.Value.CompareTo(e.NewValue) > 0)
			{
				throw new PXSetPropertyException(CS.Messages.Entry_GE, AssetDetails.Current.ReceiptDate.Value.ToShortDateString());
			}
		}

		protected virtual void FABookBalance_DeprFromPeriod_CommandPreparing(PXCache sender, PXCommandPreparingEventArgs e)
		{
			FABookBalance balance = e.Row as FABookBalance;
			if (balance != null && (e.Operation & PXDBOperation.Command) == PXDBOperation.Update && balance.AllowChangeDeprFromPeriod != true)
			{
				e.IsRestriction = true;
			}
		}

		protected virtual void FABookBalance_BookID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{

			if ((FABookSettings)PXSelect<FABookSettings,
						Where<FABookSettings.assetID, Equal<Current<FixedAsset.classID>>,
						And<FABookSettings.bookID, Equal<Required<FABookSettings.bookID>>>>>.Select(this, e.NewValue) == null)
			{
				throw new PXSetPropertyException(ErrorMessages.ElementDoesntExist, "BookID");
			}

		}

		protected virtual void FABookBalance_DepreciationMethodID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			FABookBalance bal = (FABookBalance)e.Row;
			if (bal?.DepreciationMethodID == null) return;

			if (e.ExternalCall)
			{
				FADepreciationMethod meth = PXSelect<FADepreciationMethod,
					Where<FADepreciationMethod.methodID, Equal<Current<FABookBalance.depreciationMethodID>>>>.SelectSingleBound(this, new object[] { bal });
				bal.AveragingConvention = meth?.AveragingConvention ?? bal.AveragingConvention;
			}

			if (IsImport) // #35003
			{
				bool failed = false;
				object methID = bal.DepreciationMethodID;
				try
				{
					sender.RaiseFieldVerifying<FABookBalance.depreciationMethodID>(sender, ref methID);
				}
				catch (PXException)
				{
					failed = true;
				}
				if (!failed)
				{
					sender.RaiseExceptionHandling<FABookBalance.depreciationMethodID>(bal, methID, null);
				}
			}
		}

		protected virtual void FABookBalance_DeprToPeriod_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			FABookBalance bal = (FABookBalance)e.Row;
			if (bal != null && e.OldValue != null)
			{
				bal.LastPeriod = bal.DeprToPeriod;
			}
		}

		protected virtual void FABookBalance_SalvageAmount_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			FABookBalance bal = (FABookBalance)e.Row;

			if ((decimal?) e.NewValue > bal.AcquisitionCost)
			{
				throw new PXSetPropertyException(CS.Messages.Entry_LE, bal.AcquisitionCost);
			}
		}

		protected virtual void FABookBalance_LastDeprPeriod_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			FABookBalance bal = (FABookBalance)e.Row;
			if (e.NewValue == null) return;

			if (string.CompareOrdinal((string)e.NewValue, bal.DeprFromPeriod) < 0)
			{
				throw new PXSetPropertyException(CS.Messages.Entry_GE, FinPeriodIDAttribute.FormatForError(bal.DeprFromPeriod));
			}
			if (string.CompareOrdinal((string)e.NewValue, bal.DeprToPeriod) > 0)
			{
				throw new PXSetPropertyException(CS.Messages.Entry_LE, FinPeriodIDAttribute.FormatForError(bal.DeprToPeriod));
			}
		}

		protected virtual void FABookBalance_DeprToDate_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			FABookBalance bal = (FABookBalance)e.Row;

			if ((DateTime?) e.NewValue < bal.DeprFromDate)
			{
				throw new PXSetPropertyException(CS.Messages.Entry_GT, bal.DeprFromDate);
			}
		}
		#endregion
		#region FAUsage Events
		protected virtual void FAUsage_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			FAUsage usage = (FAUsage)e.Row;
			if (usage == null) return;

			if (usage.Depreciated == true)
			{
				throw new PXSetPropertyException(Messages.BalanceRecordCannotBeDeleted);
			}
		}

		protected virtual void FAUsage_Value_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			FAUsage cur = (FAUsage)e.Row;
			if (cur == null) return;

			if ((decimal)e.NewValue <= 0m)
			{
				throw new PXSetPropertyException(CS.Messages.Entry_GT, 0m);
			}

			FAUsage prev = PXSelect<FAUsage, Where<FAUsage.assetID, Equal<Current<FADetails.assetID>>,
				And<FAUsage.number, Less<Current<FAUsage.number>>>>,
									OrderBy<Desc<FAUsage.number>>>.SelectSingleBound(this, new object[] { cur });

			if (prev != null && (decimal)e.NewValue <= prev.Value)
			{
				throw new PXSetPropertyException(CS.Messages.Entry_GT, prev.Value);
			}

			FADetails det = PXSelect<FADetails, Where<FADetails.assetID, Equal<Required<FADetails.assetID>>>>.Select(this, cur.AssetID);
			if (det != null && (decimal)e.NewValue > det.TotalExpectedUsage)
			{
				throw new PXSetPropertyException(CS.Messages.Entry_LE, det.TotalExpectedUsage);
			}
		}

		protected virtual void FAUsage_MeasurementDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			FAUsage cur = (FAUsage)e.Row;
			e.NewValue = Accessinfo.BusinessDate;
			if (cur == null || e.NewValue == null) return;

			FAUsage prev = PXSelect<FAUsage, Where<FAUsage.assetID, Equal<Current<FADetails.assetID>>,
				And<FAUsage.number, Less<Current<FAUsage.number>>>>,
									OrderBy<Desc<FAUsage.number>>>.SelectSingleBound(this, new object[] { cur });

			if (prev != null && ((DateTime)e.NewValue) <= prev.MeasurementDate)
			{
				sender.RaiseExceptionHandling<FAUsage.measurementDate>(cur, e.NewValue, new PXSetPropertyException(CS.Messages.Entry_GT, prev.MeasurementDate));
			}
		}

		protected virtual void FAUsage_MeasurementDate_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			FAUsage cur = (FAUsage)e.Row;
			if (cur == null) return;

			FAUsage prev = PXSelect<FAUsage, Where<FAUsage.assetID, Equal<Current<FADetails.assetID>>,
				And<FAUsage.number, Less<Current<FAUsage.number>>>>,
									OrderBy<Desc<FAUsage.number>>>.SelectSingleBound(this, new object[] { cur });

			if (prev != null && ((DateTime)e.NewValue) <= prev.MeasurementDate)
			{
				throw new PXSetPropertyException(CS.Messages.Entry_GT, prev.MeasurementDate);
			}
			else
			{
				sender.RaiseExceptionHandling<FAUsage.measurementDate>(cur, e.NewValue, null);
			}
		}

		#endregion
		#region FALocationHistory Events
		protected virtual void FALocationHistory_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
		{
			FALocationHistory location = e.NewRow as FALocationHistory;
			if (location == null) return;

			FATran transfer = PXSelect<FATran, Where<FATran.tranType, Equal<FATran.tranType.transferPurchasing>,
				And<FATran.released, NotEqual<True>,
				And<FATran.assetID, Equal<Current<FALocationHistory.assetID>>>>>>
				.SelectSingleBound(this, new object[] { location });

			if (transfer == null && this.IsLocationChanged(e.Row as FALocationHistory, location))
			{
				FALocationHistory newLocation = PXCache<FALocationHistory>.CreateCopy(location);
				try
				{
					sender.SmartSetStatus(e.Row, PXEntryStatus.Notchanged, PXEntryStatus.Updated);
					newLocation.RevisionID = AssetDetails.Current.LocationRevID;
					newLocation.TransactionDate = Accessinfo.BusinessDate;
					newLocation = (FALocationHistory)sender.Insert(newLocation);
				}
				finally
				{
					if (!(e.Cancel = (newLocation != null)))
					{
						sender.SmartSetStatus(e.Row, PXEntryStatus.Updated);
					}
				}
			}
		}

		public static void LiveUpdateMaskedSubs(PXGraph graph, PXCache facache, FALocationHistory lochist)
		{
			if (lochist == null) return;
			FixedAsset asset = PXSelect<FixedAsset, Where<FixedAsset.assetID, Equal<Current<FALocationHistory.assetID>>>>.SelectSingleBound(graph, new object[] { lochist });
			FABookBalance bal = PXSelect<FABookBalance, Where<FABookBalance.assetID, Equal<Current<FixedAsset.assetID>>>, OrderBy<Desc<FABookBalance.updateGL>>>.SelectSingleBound(graph, new object[] { asset });

			if (bal == null || bal.YtdDeprBase == 0m)
			{
				asset.FASubID = AssetMaint.MakeSubID<FixedAsset.fASubMask, FixedAsset.fASubID>(facache, asset);
				asset.AccumulatedDepreciationSubID = AssetMaint.MakeSubID<FixedAsset.accumDeprSubMask, FixedAsset.accumulatedDepreciationSubID>(facache, asset);
				asset.DepreciatedExpenseSubID = AssetMaint.MakeSubID<FixedAsset.deprExpenceSubMask, FixedAsset.depreciatedExpenseSubID>(facache, asset);
				asset.DisposalSubID = AssetMaint.MakeSubID<FixedAsset.proceedsSubMask, FixedAsset.disposalSubID>(facache, asset);
				asset.GainSubID = AssetMaint.MakeSubID<FixedAsset.gainLossSubMask, FixedAsset.gainSubID>(facache, asset);
				asset.LossSubID = AssetMaint.MakeSubID<FixedAsset.gainLossSubMask, FixedAsset.lossSubID>(facache, asset);

				if (facache.GetStatus(asset) == PXEntryStatus.Notchanged)
				{
					facache.SetStatus(asset, PXEntryStatus.Updated);
				}
			}
		}

		protected virtual void FALocationHistory_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			LiveUpdateMaskedSubs(this, Asset.Cache, (FALocationHistory)e.Row);
		}

		protected virtual void FALocationHistory_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			LiveUpdateMaskedSubs(this, Asset.Cache, (FALocationHistory)e.Row);
		}

		protected virtual void FALocationHistory_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			FALocationHistory newLocation = (FALocationHistory)e.Row;
			if (newLocation == null) return;
			foreach (FALocationHistory row in sender.Cached)
			{
				PXEntryStatus status = sender.GetStatus(row);
				if (status == PXEntryStatus.Inserted || status == PXEntryStatus.Updated)
				{
					CopyLocation(newLocation, row);
					e.Cancel = true;
					break;
				}
			}

			if (!e.Cancel)
			{
				if (AssetDetails.Current == null)
				{
					AssetDetails.Current = AssetDetails.Select();
				}
				AssetDetails.Current.LocationRevID = ++newLocation.RevisionID;
				AssetDetails.Cache.Update(AssetDetails.Current);
			}
		}

		protected virtual void FALocationHistory_EmployeeID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			FALocationHistory hist = (FALocationHistory)e.Row;
			sender.SetDefaultExt<FALocationHistory.locationID>(hist);
			sender.SetDefaultExt<FALocationHistory.department>(hist);
		}

		protected virtual void FALocationHistory_LocationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if (e.ExternalCall)
			{
				sender.SetDefaultExt<FALocationHistory.buildingID>(e.Row);
				sender.SetValuePending<FALocationHistory.buildingID>(e.Row, null);
			}
		}

		protected virtual void FALocationHistory_BuildingID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (!e.ExternalCall) e.Cancel = true;
		}

		#endregion
		#region FAComponent Events
		protected virtual void FAComponent_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			FixedAsset parent = Asset.Current;
			FAComponent child = (FAComponent)e.Row;
			if (parent == null || child == null) return;

			if (e.Operation == PXDBOperation.Delete)
			{
				e.Cancel = true;
			}
		}

		protected virtual void FAComponent_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			FAComponent child = (FAComponent)e.Row;
			FixedAsset parent = Asset.Current;
			if (child == null || parent == null) return;

			if (child.AssetCD != null)
			{
				FAComponent copy = PXSelectReadonly<FAComponent, Where<FAComponent.assetCD, Equal<Required<FAComponent.assetCD>>>>.Select(this, child.AssetCD);
				PXCache<FAComponent>.RestoreCopy((FAComponent)e.Row, copy);
				sender.SetStatus(e.Row, PXEntryStatus.Updated);
				sender.SetValue<FAComponent.parentAssetID>(e.Row, parent.AssetID);
			}
		}
		protected virtual void FAComponent_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			FAComponent child = (FAComponent)e.Row;

			sender.SetValue<FAComponent.parentAssetID>(child, null);
			sender.SetStatus(child, PXEntryStatus.Updated);
			e.Cancel = true;
		}
		#endregion
		#region ReverseDisposalInfo Events

		protected virtual void ReverseDisposalInfo_ReverseDisposalPeriodID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			ReverseDisposalInfo row = (ReverseDisposalInfo)e.Row;
			if (row == null) return;

			FinPeriod period = (FinPeriod)PXSelect<FinPeriod, Where<FinPeriod.finPeriodID, Equal<Required<FinPeriod.finPeriodID>>>>.Select(sender.Graph, (string)e.NewValue);
			if (period != null && period.FAClosed != true && period.Active != true)
			{
				sender.RaiseExceptionHandling<ReverseDisposalInfo.reverseDisposalPeriodID>(e.Row, null, new FiscalPeriodInactiveException(period.FinPeriodID, PXErrorLevel.Warning));
			}
		}

		#endregion
		#region Additions Events
		protected virtual void DsplFAATran_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			DsplFAATran tran = (DsplFAATran)e.Row;
			if (tran == null) return;

			PXUIFieldAttribute.SetEnabled<DsplFAATran.component>(sender, tran, (tran.Selected ?? false) && GLTrnFilter.Current.ReconType == GLTranFilter.reconType.Addition);
			PXUIFieldAttribute.SetEnabled<DsplFAATran.classID>(sender, tran, tran.Component ?? false);
			PXUIFieldAttribute.SetVisible<GLTran.inventoryID>(Caches[typeof(GLTran)], null, true);
		}

		protected virtual void DsplFAATran_ClassID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			DsplFAATran tran = (DsplFAATran)e.Row;
			if (tran == null) return;

			if (tran.Component == true && e.NewValue == null)
			{
				throw new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(DsplFAATran.classID).Name);
			}
		}

		protected virtual void DsplFAATran_SelectedAmt_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			DsplFAATran tran = (DsplFAATran)e.Row;
			if (tran.SelectedAmt > 0m)
			{
				tran.Selected = true;
			}
		}

		protected virtual void DsplFAATran_SelectedQty_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			DsplFAATran tran = (DsplFAATran)e.Row;
			if (tran.SelectedQty > 0m)
			{
				tran.Selected = true;
			}
		}

		protected virtual void DsplFAATran_Reconciled_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			DsplFAATran tran = (DsplFAATran)e.Row;
			if (tran == null) return;

			FAAccrualTran atran = PXSelect<FAAccrualTran, Where<FAAccrualTran.tranID, Equal<Current<DsplFAATran.tranID>>>>.SelectSingleBound(this, new object[] { tran });
			atran.Reconciled = tran.Reconciled;
			if (Additions.Cache.GetStatus(atran) == PXEntryStatus.Notchanged)
			{
				Additions.Cache.SetStatus(atran, PXEntryStatus.Updated);
				Additions.Cache.IsDirty = true;
			}
		}

		protected virtual void DsplFAATran_SelectedAmt_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			DsplFAATran tran = (DsplFAATran)e.Row;
			if (tran == null) return;

			if (tran.Selected == true)
			{
				if (((decimal?)e.NewValue) <= 0m)
					throw new PXSetPropertyException(CS.Messages.Entry_GT, 0m);
				if (((decimal?)e.NewValue) > tran.GLTranAmt - tran.ClosedAmt)
					throw new PXSetPropertyException(CS.Messages.Entry_LE, tran.GLTranAmt - tran.ClosedAmt);
			}
		}

		protected virtual void DsplFAATran_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			DsplFAATran tran = (DsplFAATran)e.Row;
			if (tran == null) return;

			GLTranFilter filter = GLTrnFilter.Current;
			decimal sum = 0m;
			decimal expectedcost = filter.CurrentCost ?? 0m;
			decimal expectedaccr = filter.AccrualBalance ?? 0m;

			foreach (DsplFAATran trn in DsplAdditions.Cache.Inserted)
			{
				sum += trn.SelectedAmt ?? 0m;
			}

			if (filter.ReconType == GLTranFilter.reconType.Addition)
			{
				expectedaccr += sum;
				if (expectedaccr > filter.CurrentCost)
				{
					expectedcost = expectedaccr;
				}
			}
			else
			{
				expectedaccr -= sum;
				expectedcost = expectedaccr;
			}

			filter.SelectionAmt = sum;
			filter.ExpectedAccrualBal = expectedaccr;
			filter.ExpectedCost = expectedcost;

			try
			{
				object classID = tran.ClassID;
				sender.RaiseFieldVerifying<DsplFAATran.classID>(tran, ref classID);
			}
			catch (PXSetPropertyException ex)
			{
				sender.RaiseExceptionHandling<DsplFAATran.classID>(tran, tran.ClassID, ex);
			}

			try
			{
				object selAmt = tran.SelectedAmt;
				sender.RaiseFieldVerifying<DsplFAATran.selectedAmt>(tran, ref selAmt);
			}
			catch (PXSetPropertyException ex)
			{
				sender.RaiseExceptionHandling<DsplFAATran.selectedAmt>(tran, tran.SelectedAmt, ex);
			}
		}

		// TODO: DK - need to write a formula
		protected virtual void DsplFAATran_Component_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			DsplFAATran tran = (DsplFAATran)e.Row;
			if (tran == null) return;

			if (tran.Component != true)
			{
				tran.ClassID = null;
				object old = tran.SelectedQty;
				tran.SelectedQty = tran.Selected == true ? Math.Min((Asset.Current.Qty ?? 0m), (tran.OpenQty ?? 0m)) : 0m;
				if ((decimal?)old != tran.SelectedQty)
				{
					sender.RaiseFieldUpdated<DsplFAATran.selectedQty>(tran, old);
				}
			}
		}

		protected virtual void DsplFAATran_Selected_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			DsplFAATran tran = (DsplFAATran)e.Row;
			if (tran == null) return;

			GLTranFilter filter = GLTrnFilter.Current;
			object old;

			old = tran.SelectedQty;
			tran.SelectedQty = tran.Selected == true ? Math.Min((Asset.Current.Qty ?? 0m), (tran.OpenQty ?? 0m)) : 0m;
			if ((decimal?)old != tran.SelectedQty)
			{
				sender.RaiseFieldUpdated<DsplFAATran.selectedQty>(tran, old);
			}

			old = tran.SelectedAmt;
			tran.SelectedAmt = tran.Selected == true ? Math.Min((filter.ExpectedCost ?? 0m) - (filter.ExpectedAccrualBal ?? 0m), tran.OpenAmt ?? 0m) : 0m;
			if ((decimal?)old != tran.SelectedAmt)
			{
				sender.RaiseFieldUpdated<DsplFAATran.selectedAmt>(tran, old);
			}
		}

		protected virtual void DsplFAATran_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			e.Cancel = true;
		}

		#endregion
		#region FATran
		protected virtual void FATran_AssetID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			e.Cancel = true;
		}
		protected virtual void FATran_BookID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			e.Cancel = true;
		}

		protected virtual void FATran_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			FATran tran = (FATran)e.Row;
			if ((tran.Released == true && e.ExternalCall) || (tran.Released == true && !string.IsNullOrEmpty(tran.BatchNbr)))
			{
				throw new PXSetPropertyException(ErrorMessages.CantDeleteRecord);
			}
		}

		protected virtual void FATran_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			DsplAdditions.Cache.Clear();
		}

		protected virtual void FATran_RefNbr_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			FATran tran = (FATran)e.Row;
			if (tran == null) return;

			if (AssetGLTransactions.IsTempKey(tran.RefNbr))
			{
				e.ReturnValue = null;
			}
		}

		protected virtual void GLTranFilter_AccountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			DsplAdditions.Cache.Clear();
		}

		protected virtual void GLTranFilter_SubID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			DsplAdditions.Cache.Clear();
		}

		protected virtual void GLTranFilter_ReconType_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			DsplAdditions.Cache.Clear();
		}

		protected virtual void GLTranFilter_ShowReconciled_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			DsplAdditions.Cache.Clear();
		}

		protected virtual void GLTranFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			GLTranFilter filter = (GLTranFilter)e.Row;
			FixedAsset asset = CurrentAsset.Current;
			FADetails det = AssetDetails.Current ?? AssetDetails.Select();
			if (filter == null || asset == null || det == null) return;

			FATran unrelp = PXSelect<FATran, Where<FATran.assetID, Equal<Current<FixedAsset.assetID>>, And<FATran.released, Equal<False>, And<Where<FATran.tranType, Equal<FATran.tranType.purchasingPlus>, Or<FATran.tranType, Equal<FATran.tranType.purchasingMinus>>>>>>>.Select(this);
			FATran unrelr = PXSelect<FATran, Where<FATran.assetID, Equal<Current<FixedAsset.assetID>>, And<FATran.released, Equal<False>, And<Where<FATran.tranType, Equal<FATran.tranType.reconcilliationPlus>, Or<FATran.tranType, Equal<FATran.tranType.reconcilliationMinus>>>>>>>.Select(this);
			ProcessAdditions.SetEnabled(CurrentAsset.Cache.GetStatus(asset) != PXEntryStatus.Inserted && unrelp == null && unrelr == null && filter.UnreconciledAmt >= 0);
			PXUIFieldAttribute.SetWarning<GLTranFilter.acquisitionCost>(sender, filter,
																		CurrentAsset.Cache.GetStatus(asset) == PXEntryStatus.Inserted
																			? Messages.FixedAssetNotSaved
																			: null);
			PXUIFieldAttribute.SetWarning<GLTranFilter.currentCost>(sender, filter,
																		unrelp != null
																			? Messages.FixedAssetHasUnreleasedPurchasing
																			: null);
			PXUIFieldAttribute.SetWarning<GLTranFilter.accrualBalance>(sender, filter,
																		unrelr != null
																			? Messages.FixedAssetHasUnreleasedRecon
																			: null);
			PXUIFieldAttribute.SetError<GLTranFilter.unreconciledAmt>(sender, filter,
																		filter.UnreconciledAmt < 0
																			? EP.Messages.ValueMustBeGreaterThanZero
																			: null);

			PXUIFieldAttribute.SetEnabled<GLTranFilter.reconType>(sender, filter, det.IsReconciled ?? false);
			ReduceUnreconCost.SetEnabled(filter.ReconType == GLTranFilter.reconType.Addition && filter.UnreconciledAmt > 0m && filter.AccrualBalance > 0m && unrelp == null && unrelr == null);
		}
		#endregion

		protected virtual void DisposeParams_DisposalMethodID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<DisposeParams.disposalAccountID>(e.Row);
			sender.SetDefaultExt<DisposeParams.disposalSubID>(e.Row);
		}

		protected virtual void DisposeParams_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			DisposeParams param = (DisposeParams)e.Row;
			if (param == null) return;

			param.DisposalAccountID = null;
			param.DisposalSubID = null;
		}
		#region Overrides

		#endregion
	}

	#region DAC Definitions

	#region FABookBalance2
	[PXProjection(typeof(Select2<FABookBalance,
		LeftJoin<FixedAsset, On<FABookBalance.assetID, Equal<FixedAsset.assetID>>,
		InnerJoin<FABookPeriod, On<FABookPeriod.bookID, Equal<FABookBalance.bookID>, And<FABookPeriod.finPeriodID, Equal<FABookBalance.deprFromPeriod>>>,
		InnerJoin<FABookPeriod2, On<FABookPeriod2.bookID, Equal<FABookBalance.bookID>, And<FABookPeriod2.finPeriodID, Equal<FABookBalance.deprToPeriod>>>>>>,
		Where<FixedAsset.depreciable, Equal<True>>>))]
	[Serializable]
	[PXHidden]
	public class FABookBalance2 : IBqlTable
	{
		#region ExactLife
		public abstract class exactLife : IBqlField { }
		[PXDBCalced(typeof(Sub<Add<int1, FABookPeriod2.finYear>, FABookPeriod.finYear>), typeof(int))]
		public virtual int? ExactLife
		{
			get;
			set;
		}
		#endregion
		#region FABookPeriod2
		[Serializable]
		[PXHidden]
		public class FABookPeriod2 : FABookPeriod
		{
			public new abstract class bookID : IBqlField { }
			public new abstract class finPeriodID : IBqlField { }
			public new abstract class finYear : IBqlField { }
		}
		#endregion
	}
	#endregion

	#region Dynamic Representation of Depreciation (Periods Side by Side)
	[Serializable]
	public partial class FAHistory : IBqlTable
	{
		#region AssetID
		public abstract class assetID : IBqlField
		{
		}
		protected Int32? _AssetID;
		[PXDBInt(IsKey = true)]
		[PXDefault]
		public virtual Int32? AssetID
		{
			get
			{
				return _AssetID;
			}
			set
			{
				_AssetID = value;
			}
		}
		#endregion
		#region FinPeriodID
		public abstract class finPeriodID : IBqlField
		{
		}
		protected String _FinPeriodID;
		[FABookPeriodID]
		[PXUIField(DisplayName = "Period")]
		public virtual String FinPeriodID
		{
			get
			{
				return _FinPeriodID;
			}
			set
			{
				_FinPeriodID = value;
			}
		}
		#endregion
		#region PtdDepreciated
		public Decimal?[] PtdDepreciated;
		#endregion
	}
	#endregion

	#region Dynamic Representation of Depreciation (Book Sheet)
	[Serializable]
	public partial class FASheetHistory : IBqlTable
	{
		#region AssetID
		public abstract class assetID : IBqlField
		{
		}
		protected Int32? _AssetID;
		[PXDBInt(IsKey = true)]
		[PXDefault]
		public virtual Int32? AssetID
		{
			get
			{
				return _AssetID;
			}
			set
			{
				_AssetID = value;
			}
		}
		#endregion
		#region PeriodNbr
		public abstract class periodNbr : IBqlField
		{
		}
		protected string _PeriodNbr;
		[PXString(2, IsFixed = true, IsKey = true)]
		public virtual string PeriodNbr
		{
			get
			{
				return _PeriodNbr;
			}
			set
			{
				_PeriodNbr = value;
			}
		}
		#endregion
		#region PtdDepreciated
		public class PeriodDeprPair
		{
			private readonly string _PeriodID;
			private readonly decimal? _PtdDepreciated;
			private readonly decimal? _PtdCalculated;
			public PeriodDeprPair(string periodID, decimal? depr, decimal? calc)
			{
				_PeriodID = periodID;
				_PtdDepreciated = depr;
				_PtdCalculated = calc;
			}

			public string PeriodID
			{
				get { return _PeriodID; }
			}
			public decimal? PtdDepreciated
			{
				get { return _PtdDepreciated; }
			}
			public decimal? PtdCalculated
			{
				get { return _PtdCalculated; }
			}
		}

		public PeriodDeprPair[] PtdValues;

		#endregion
	}
	#endregion

	#region FATransactions History Filter
	[Serializable]
	public partial class TranBookFilter : IBqlTable
	{
		#region BookID
		public abstract class bookID : IBqlField
		{
		}
		protected Int32? _BookID;
		[PXDBInt]
		[PXSelector(typeof(Search2<FABook.bookID,
								InnerJoin<FABookBalance, On<FABookBalance.bookID, Equal<FABook.bookID>>>,
								Where<FABookBalance.assetID, Equal<Current<FixedAsset.assetID>>>>),
					SubstituteKey = typeof(FABook.bookCode),
					DescriptionField = typeof(FABook.description))]
		[PXUIField(DisplayName = "Book")]
		public virtual Int32? BookID
		{
			get
			{
				return _BookID;
			}
			set
			{
				_BookID = value;
			}
		}
		#endregion
	}

	[Serializable]
	public partial class DeprBookFilter : IBqlTable
	{
		#region BookID
		public abstract class bookID : IBqlField
		{
		}
		protected Int32? _BookID;
		[PXDBInt]
		[PXSelector(typeof(Search2<FABook.bookID,
								InnerJoin<FABookBalance, On<FABookBalance.bookID, Equal<FABook.bookID>>>,
								Where<FABookBalance.assetID, Equal<Current<FixedAsset.assetID>>>>),
					SubstituteKey = typeof(FABook.bookCode),
					DescriptionField = typeof(FABook.description))]
		[PXDefault(typeof(Search2<FABookBalance.bookID, LeftJoin<FixedAsset, On<FABookBalance.assetID, Equal<FixedAsset.assetID>>>, Where<FABookBalance.assetID, Equal<Current<FixedAsset.assetID>>, And<FixedAsset.depreciable, Equal<True>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Book")]
		public Int32? BookID
		{
			get
			{
				return _BookID;
			}
			set
			{
				_BookID = value;
			}
		}
		#endregion
	}
	#endregion

	#region Parameters for Dispose
	[Serializable]
	[PXCacheName(Messages.DisposeParams)]
	public partial class DisposeParams : IBqlTable
	{
		#region DisposalDate
		public abstract class disposalDate : IBqlField
		{
		}
		protected DateTime? _DisposalDate;
		[PXDBDate()]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Disposal Date")]
		public virtual DateTime? DisposalDate
		{
			get
			{
				return _DisposalDate;
			}
			set
			{
				_DisposalDate = value;
			}
		}
		#endregion
		#region DisposalPeriodID
		public abstract class disposalPeriodID : IBqlField
		{
		}
		protected string _DisposalPeriodID;
		[PXUIField(DisplayName = "Disposal Period")]
		[FinPeriodSelector(typeof(Search2<FinPeriod.finPeriodID,
			LeftJoin<FABookBalance, On<FABookBalance.updateGL, Equal<True>,
				And<FABookBalance.assetID, Equal<Current<FixedAsset.assetID>>>>>,
			Where<FinPeriod.fAClosed, NotEqual<True>,
				And<FinPeriod.startDate, NotEqual<FinPeriod.endDate>,
				And<Where<FinPeriod.finPeriodID, GreaterEqual<FABookBalance.lastDeprPeriod>,
					Or<FABookBalance.lastDeprPeriod, IsNull>>>>>>),
			typeof(DisposeParams.disposalDate))]
		public virtual string DisposalPeriodID
		{
			get
			{
				return _DisposalPeriodID;
			}
			set
			{
				_DisposalPeriodID = value;
			}
		}
		#endregion
		#region DisposalAmt
		public abstract class disposalAmt : IBqlField
		{
		}
		protected Decimal? _DisposalAmt;
		[PXDBBaseCury]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Proceeds Amount")]
		public virtual Decimal? DisposalAmt
		{
			get
			{
				return _DisposalAmt;
			}
			set
			{
				_DisposalAmt = value;
			}
		}
		#endregion
		#region DisposalMethodID
		public abstract class disposalMethodID : IBqlField
		{
		}
		protected Int32? _DisposalMethodID;
		[PXDBInt()]
		[PXDefault()]
		[PXSelector(typeof(Search<FADisposalMethod.disposalMethodID>),
					SubstituteKey = typeof(FADisposalMethod.disposalMethodCD),
					DescriptionField = typeof(FADisposalMethod.description))]
		[PXUIField(DisplayName = "Disposal Method", Required = true)]
		public virtual Int32? DisposalMethodID
		{
			get
			{
				return _DisposalMethodID;
			}
			set
			{
				_DisposalMethodID = value;
			}
		}
		#endregion
		#region DisposalAccountID
		public abstract class disposalAccountID : IBqlField
		{
		}
		protected Int32? _DisposalAccountID;
		[PXDefault(typeof(Coalesce<Search<FixedAsset.disposalAccountID, Where<FixedAsset.assetID, Equal<Current<FixedAsset.assetID>>>>, Coalesce<Search<FADisposalMethod.proceedsAcctID, Where<FADisposalMethod.disposalMethodID, Equal<Current<DisposeParams.disposalMethodID>>>>, Search<FASetup.proceedsAcctID>>>))]
		[Account(DisplayName = "Proceeds Account", DescriptionField = typeof(Account.description))]
		public virtual Int32? DisposalAccountID
		{
			get
			{
				return _DisposalAccountID;
			}
			set
			{
				_DisposalAccountID = value;
			}
		}
		#endregion
		#region DisposalSubID
		public abstract class disposalSubID : IBqlField
		{
		}
		protected Int32? _DisposalSubID;
		[PXDefault(typeof(Coalesce<Search<FixedAsset.disposalSubID, Where<FixedAsset.assetID, Equal<Current<FixedAsset.assetID>>>>, Coalesce<Search<FADisposalMethod.proceedsSubID, Where<FADisposalMethod.disposalMethodID, Equal<Current<DisposeParams.disposalMethodID>>>>, Search<FASetup.proceedsSubID>>>))]
		[SubAccount(typeof(DisposeParams.disposalAccountID), typeof(FixedAsset.branchID), DisplayName = "Proceeds Sub.", DescriptionField = typeof(Sub.description))]
		public virtual Int32? DisposalSubID
		{
			get
			{
				return _DisposalSubID;
			}
			set
			{
				_DisposalSubID = value;
			}
		}
		#endregion
		#region DeprBeforeDisposal
		public abstract class deprBeforeDisposal : IBqlField
		{
		}
		protected bool? _DeprBeforeDisposal;
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Depreciate before disposal")]
		public virtual bool? DeprBeforeDisposal
		{
			get
			{
				return _DeprBeforeDisposal;
			}
			set
			{
				_DeprBeforeDisposal = value;
			}
		}
		#endregion
		#region Reason
		public abstract class reason : PX.Data.IBqlField
		{
		}
		protected String _Reason;
		[PXDBString(30, IsUnicode = true)]
		[PXUIField(DisplayName = "Reason")]
		public virtual String Reason
		{
			get
			{
				return this._Reason;
			}
			set
			{
				this._Reason = value;
			}
		}
		#endregion

	}
	#endregion

	#region Info for Reverse Disposal
	[Serializable]
	[PXCacheName(Messages.ReverseDisposalInfo)]
	public partial class ReverseDisposalInfo : DisposeParams
	{
		#region DisposalDate
		[PXDBDate]
		[PXDefault(typeof(FADetails.disposalDate), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Disposal Date", Enabled = false)]
		public override DateTime? DisposalDate
		{
			get
			{
				return _DisposalDate;
			}
			set
			{
				_DisposalDate = value;
			}
		}
		#endregion
		#region DisposalPeriodID
		[PXUIField(DisplayName = "Disposal Period", Enabled = false)]
		[PXDBString(6, IsFixed = true)]
		[FinPeriodIDFormatting]
		[PXDefault(typeof(FADetails.disposalPeriodID), PersistingCheck = PXPersistingCheck.Nothing)]
		public override string DisposalPeriodID
		{
			get
			{
				return _DisposalPeriodID;
			}
			set
			{
				_DisposalPeriodID = value;
			}
		}
		#endregion
		#region DisposalAmt
		[PXDBBaseCury]
		[PXDefault(typeof(FADetails.saleAmount), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Proceeds Amount", Enabled = false)]
		public override Decimal? DisposalAmt
		{
			get
			{
				return _DisposalAmt;
			}
			set
			{
				_DisposalAmt = value;
			}
		}
		#endregion
		#region DisposalMethodID
		[PXDBInt]
		[PXDefault(typeof(FADetails.disposalMethodID), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXSelector(typeof(Search<FADisposalMethod.disposalMethodID>),
					SubstituteKey = typeof(FADisposalMethod.disposalMethodCD),
					DescriptionField = typeof(FADisposalMethod.description))]
		[PXUIField(DisplayName = "Disposal Method", Enabled = false)]
		public override Int32? DisposalMethodID
		{
			get
			{
				return _DisposalMethodID;
			}
			set
			{
				_DisposalMethodID = value;
			}
		}
		#endregion
		#region DisposalAccountID
		[PXDefault(typeof(FixedAsset.disposalAccountID), PersistingCheck = PXPersistingCheck.Nothing)]
		[Account(DisplayName = "Proceeds Account", DescriptionField = typeof(Account.description), Enabled = false)]
		public override Int32? DisposalAccountID
		{
			get
			{
				return _DisposalAccountID;
			}
			set
			{
				_DisposalAccountID = value;
			}
		}
		#endregion
		#region DisposalSubID
		[PXDefault(typeof(FixedAsset.disposalSubID), PersistingCheck = PXPersistingCheck.Nothing)]
		[SubAccount(typeof(FixedAsset.disposalAccountID), DisplayName = "Proceeds Sub.", DescriptionField = typeof(Sub.description), Enabled = false)]
		public override Int32? DisposalSubID
		{
			get
			{
				return _DisposalSubID;
			}
			set
			{
				_DisposalSubID = value;
			}
		}
		#endregion
		#region ReverseDisposalDate
		public abstract class reverseDisposalDate : IBqlField
		{
		}
		protected DateTime? _ReverseDisposalDate;
		[PXDBDate]
		[PXDefault(typeof(AccessInfo.businessDate), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Reversal Date")]
		public virtual DateTime? ReverseDisposalDate
		{
			get
			{
				return _ReverseDisposalDate;
			}
			set
			{
				_ReverseDisposalDate = value;
			}
		}
		#endregion
		#region ReverseDisposalPeriodID
		public abstract class reverseDisposalPeriodID : IBqlField { }
		protected string _ReverseDisposalPeriodID;
		[PXUIField(DisplayName = "Reversal Period", Required = true)]
		[PXDBString(6, IsFixed = true, InputMask = "##-####")]
		[FinPeriodSelector(typeof(Search2<FinPeriod.finPeriodID,
							LeftJoin<FABookHistory, On<FinPeriod.finPeriodID, Equal<FABookHistory.finPeriodID>,
								And<FABookHistory.assetID, Equal<Current<FixedAsset.assetID>>>>,
							LeftJoin<FABook, On<FABook.bookID, Equal<FABookHistory.bookID>>>>,
							Where<FinPeriod.fAClosed, NotEqual<True>,
								And<FinPeriod.finPeriodID, GreaterEqual<Current<FADetails.disposalPeriodID>>,
								And<FinPeriod.startDate, NotEqual<FinPeriod.endDate>,
								And<Where<FABook.updateGL, Equal<True>, Or<FABook.updateGL, IsNull>>>>>>>), typeof(ReverseDisposalInfo.reverseDisposalDate))]
		[PXDefault]
		public virtual string ReverseDisposalPeriodID
		{
			get
			{
				return _ReverseDisposalPeriodID;
			}
			set
			{
				_ReverseDisposalPeriodID = value;
			}
		}
		#endregion
	}
	#endregion

	#region Parameters for Suspend
	[Serializable]
	public partial class SuspendParams : IBqlTable
	{
		#region CurrentPeriodID
		public abstract class currentPeriodID : IBqlField
		{
		}
		protected string _CurrentPeriodID;
		[PXUIField(DisplayName = "Current Period")]
		[FAOpenPeriod]
		[PXDefault(typeof(Search2<FABookPeriod.finPeriodID,
			InnerJoin<FABookBalance, On<FABookPeriod.bookID, Equal<FABookBalance.bookID>>>,
			Where<FABookPeriod.closed, NotEqual<True>,
				And<FABookPeriod.startDate, NotEqual<FABookPeriod.endDate>,
				And<FABookBalance.assetID, Equal<Current<FixedAsset.assetID>>,
				And<FABookPeriod.finPeriodID, GreaterEqual<IsNull<FABookBalance.currDeprPeriod, IsNull<FABookBalance.lastDeprPeriod, FABookBalance.deprFromPeriod>>>>>>>,
			OrderBy<Desc<FABookBalance.updateGL, Asc<FABookPeriod.finPeriodID>>>>))]
		public virtual string CurrentPeriodID
		{
			get
			{
				return _CurrentPeriodID;
			}
			set
			{
				_CurrentPeriodID = value;
			}
		}
		#endregion
	}
	#endregion

	#region Parameters for Split
	[Serializable]
	public partial class SplitParams : FixedAsset
	{
		#region SplitID
		public abstract class splitID : IBqlField { }
		protected int? _SplitID;
		[PXInt(IsKey = true)]
		[PXLineNbr(typeof(SplitProcess.SplitFilter))]
		public virtual int? SplitID
		{
			get
			{
				return _SplitID;
			}
			set
			{
				_SplitID = value;
			}
		}
		#endregion
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXUIField(DisplayName = "Asset ID", Visibility = PXUIVisibility.SelectorVisible, TabOrder = 1)]
		public override string AssetCD { get; set; }

		#region SplittedAssetCD
		public abstract class splittedAssetCD : IBqlField { }
		protected String _SplittedAssetCD;
		[PXString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXUIField(DisplayName = "Asset ID")]
		public virtual string SplittedAssetCD
		{
			get
			{
				return _SplittedAssetCD;
			}
			set
			{
				_SplittedAssetCD = value;
			}
		}
		#endregion
		#region Cost
		public abstract class cost : IBqlField
		{
		}
		protected decimal? _Cost;
		[PXBaseCury]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Cost")]
		public virtual decimal? Cost
		{
			get
			{
				return _Cost;
			}
			set
			{
				_Cost = value;
			}
		}
		#endregion
		#region SplittedQty
		public abstract class splittedQty : IBqlField { }
		protected Decimal? _SplittedQty;
		[PXQuantity]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Quantity")]
		public virtual Decimal? SplittedQty
		{
			get
			{
				return _SplittedQty;
			}
			set
			{
				_SplittedQty = value;
			}
		}
		#endregion
		#region Ratio
		public abstract class ratio : IBqlField
		{
		}
		protected decimal? _Ratio;
		[PXDecimal(6, MinValue = 0, MaxValue = 100)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Ratio")]
		public virtual decimal? Ratio
		{
			get
			{
				return this._Ratio;
			}
			set
			{
				this._Ratio = value;
			}
		}
		#endregion
	}
	#endregion


	[Serializable]
	public partial class DsplFAATran : IBqlTable
	{
		#region Selected
		public abstract class selected : IBqlField
		{
		}
		protected bool? _Selected = false;
		[PXBool]
		[PXUIField(DisplayName = "Selected")]
		public bool? Selected
		{
			get
			{
				return _Selected;
			}
			set
			{
				_Selected = value;
			}
		}
		#endregion
		#region TranID
		public abstract class tranID : IBqlField
		{
		}
		protected int? _TranID;
		[PXDBInt(IsKey = true)]
		[PXDefault]
		public virtual int? TranID
		{
			get
			{
				return _TranID;
			}
			set
			{
				_TranID = value;
			}
		}
		#endregion
		#region GLTranQty
		public abstract class gLTranQty : IBqlField
		{
		}
		protected Decimal? _GLTranQty;
		[PXDBQuantity]
		[PXUIField(DisplayName = "Orig. Quantity", Enabled = false)]
		public virtual Decimal? GLTranQty
		{
			get
			{
				return _GLTranQty;
			}
			set
			{
				_GLTranQty = value;
			}
		}
		#endregion
		#region GLTranAmt
		public abstract class gLTranAmt : IBqlField
		{
		}
		protected Decimal? _GLTranAmt;
		[PXDBBaseCury]
		[PXUIField(DisplayName = "Orig. Amount", Enabled = false)]
		public virtual Decimal? GLTranAmt
		{
			get
			{
				return _GLTranAmt;
			}
			set
			{
				_GLTranAmt = value;
			}
		}
		#endregion
		#region SelectedQty
		public abstract class selectedQty : IBqlField
		{
		}
		protected Decimal? _SelectedQty;
		[PXDBQuantity]
		[PXUnboundDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Selected Quantity")]
		public virtual Decimal? SelectedQty
		{
			get
			{
				return _SelectedQty;
			}
			set
			{
				_SelectedQty = value;
			}
		}
		#endregion
		#region SelectedAmt
		public abstract class selectedAmt : IBqlField
		{
		}
		protected Decimal? _SelectedAmt;
		[PXDBBaseCury]
		[PXUnboundDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Selected Amount")]
		public virtual Decimal? SelectedAmt
		{
			get
			{
				return _SelectedAmt;
			}
			set
			{
				_SelectedAmt = value;
			}
		}
		#endregion
		#region OpenQty
		public abstract class openQty : IBqlField
		{
		}
		protected Decimal? _OpenQty;
		[PXQuantity]
		[PXFormula(typeof(Sub<gLTranQty, Add<closedQty, selectedQty>>))]
		[PXUIField(DisplayName = "Open Quantity", Enabled = false)]
		public virtual Decimal? OpenQty
		{
			get
			{
				return _OpenQty;
			}
			set
			{
				_OpenQty = value;
			}
		}
		#endregion
		#region OpenAmt
		public abstract class openAmt : IBqlField
		{
		}
		protected Decimal? _OpenAmt;
		[PXBaseCury]
		[PXFormula(typeof(Sub<gLTranAmt, Add<closedAmt, selectedAmt>>))]
		[PXUIField(DisplayName = "Open Amount", Enabled = false)]
		public virtual Decimal? OpenAmt
		{
			get
			{
				return _OpenAmt;
			}
			set
			{
				_OpenAmt = value;
			}
		}
		#endregion
		#region ClosedAmt
		public abstract class closedAmt : IBqlField
		{
		}
		protected Decimal? _ClosedAmt;
		[PXDBBaseCury]
		public virtual Decimal? ClosedAmt
		{
			get
			{
				return _ClosedAmt;
			}
			set
			{
				_ClosedAmt = value;
			}
		}
		#endregion
		#region ClosedQty
		public abstract class closedQty : IBqlField
		{
		}
		protected Decimal? _ClosedQty;
		[PXDBQuantity]
		public virtual Decimal? ClosedQty
		{
			get
			{
				return _ClosedQty;
			}
			set
			{
				_ClosedQty = value;
			}
		}
		#endregion
		#region UnitCost
		public abstract class unitCost : IBqlField
		{
		}
		protected Decimal? _UnitCost;
		[PXDBBaseCury]
		[PXFormula(typeof(Switch<Case<Where<gLTranQty, LessEqual<decimal0>>, gLTranAmt>, Div<gLTranAmt, gLTranQty>>))]
		[PXUIField(DisplayName = "Unit Cost", Enabled = false)]
		public virtual Decimal? UnitCost
		{
			get
			{
				return _UnitCost;
			}
			set
			{
				_UnitCost = value;
			}
		}
		#endregion
		#region Reconciled
		public abstract class reconciled : IBqlField { }
		[PXDBBool]
		[PXUIField(DisplayName = "Reconciled")]
		[PXDefault(typeof(Search<FAAccrualTran.reconciled, Where<FAAccrualTran.tranID, Equal<Current<tranID>>>>))]
		public bool? Reconciled { get; set; }
		#endregion

		#region Component
		public abstract class component : IBqlField
		{
		}
		protected Boolean? _Component = false;
		[PXBool]
		[PXUIField(DisplayName = "Component")]
		public virtual Boolean? Component
		{
			get
			{
				return _Component;
			}
			set
			{
				_Component = value;
			}
		}
		#endregion
		#region ClassID
		public abstract class classID : IBqlField
		{
		}
		protected Int32? _ClassID;
		[PXInt]
		[PXSelector(typeof(Search2<FixedAsset.assetID,
			LeftJoin<FABookSettings, On<FixedAsset.assetID, Equal<FABookSettings.assetID>>,
			LeftJoin<FABook, On<FABookSettings.bookID, Equal<FABook.bookID>>>>,
			Where<FixedAsset.recordType, Equal<FARecordType.classType>,
			And<FABook.updateGL, Equal<True>>>>),
					SubstituteKey = typeof(FixedAsset.assetCD),
					DescriptionField = typeof(FixedAsset.description))]
		[PXUIField(DisplayName = "Asset Class")]
		public virtual Int32? ClassID
		{
			get
			{
				return _ClassID;
			}
			set
			{
				_ClassID = value;
			}
		}
		#endregion
	}

	#endregion

	#region Formulas
	#region SelectDepreciationMethod
	public class SelectDepreciationMethod<DeprDate, ClassID, BookID> : BqlFormulaEvaluator<DeprDate, ClassID, BookID>
		where DeprDate : IBqlOperand
		where ClassID : IBqlOperand
		where BookID : IBqlOperand
	{

		public override object Evaluate(PXCache cache, object item, Dictionary<Type, object> pars)
		{
			DateTime? deprDate = (DateTime?)pars[typeof(DeprDate)];
			int? bookID = (int?)pars[typeof(BookID)];
			int? classID = (int?)pars[typeof(ClassID)];

			FADepreciationMethod parent = PXSelectJoin<FADepreciationMethod,
								LeftJoin<FABookSettings,
									On<FADepreciationMethod.methodID, Equal<FABookSettings.depreciationMethodID>>>,
								Where<FABookSettings.assetID, Equal<Required<FABookBalance.classID>>,
									And<FABookSettings.bookID, Equal<Required<FABookBalance.bookID>>>>>.Select(cache.Graph, classID, bookID);
			if (parent != null && parent.RecordType == FARecordType.BothType)
			{
				return parent.MethodCD;
			}
			else if (parent != null && parent.RecordType == FARecordType.ClassType && deprDate != null)
			{
				int? num = null;
				if (parent.AveragingConvention == FAAveragingConvention.HalfQuarter)
				{
					num = FABookPeriodIDAttribute.QuarterNumFromDate(cache.Graph, deprDate, bookID);
				}
				if (parent.AveragingConvention == FAAveragingConvention.HalfPeriod)
				{
					num = FABookPeriodIDAttribute.PeriodNumFromDate(cache.Graph, deprDate, bookID);
				}

				FADepreciationMethod meth =
					PXSelect<FADepreciationMethod,
						Where<FADepreciationMethod.parentMethodID, Equal<Required<FADepreciationMethod.parentMethodID>>,
							And<FADepreciationMethod.averagingConvPeriod, Equal<Required<FADepreciationMethod.averagingConvPeriod>>>>>.Select(cache.Graph, parent.MethodID, num);
				if (meth != null)
				{
					return meth.MethodCD;
				}
			}
			return null;
		}
	}
	#endregion
	#region CalcNextMeasurementDate
	public class CalcNextMeasurementDate<LastDate, DateFrom, AssetID> : BqlFormulaEvaluator<LastDate, DateFrom, AssetID>
		where LastDate : IBqlOperand
		where DateFrom : IBqlOperand
		where AssetID : IBqlOperand
	{

		public override object Evaluate(PXCache cache, object item, Dictionary<Type, object> pars)
		{
			DateTime? lastDate = (DateTime?)pars[typeof(LastDate)];
			DateTime? dateFrom = (DateTime?)pars[typeof(DateFrom)];
			int? assetID = (int?)pars[typeof(AssetID)];

			lastDate = lastDate ?? dateFrom;
			if (lastDate == null)
			{
				return null;
			}

			FAUsageSchedule sched = PXSelectJoin<FAUsageSchedule,
				InnerJoin<FixedAsset, On<FixedAsset.usageScheduleID, Equal<FAUsageSchedule.scheduleID>>>,
				Where<FixedAsset.assetID, Equal<Required<FixedAsset.assetID>>>>.Select(cache.Graph, assetID);

			if (sched == null)
			{
				return null;
			}

			switch (sched.ReadUsageEveryPeriod)
			{
				case FAUsageSchedule.readUsageEveryPeriod.Day:
					return lastDate.Value.AddDays(sched.ReadUsageEveryValue ?? 0);
				case FAUsageSchedule.readUsageEveryPeriod.Week:
					return lastDate.Value.AddDays((sched.ReadUsageEveryValue ?? 0) * 7);
				case FAUsageSchedule.readUsageEveryPeriod.Month:
					return lastDate.Value.AddMonths(sched.ReadUsageEveryValue ?? 0);
				case FAUsageSchedule.readUsageEveryPeriod.Year:
					return lastDate.Value.AddYears(sched.ReadUsageEveryValue ?? 0);
				default:
					return null;
			}
		}
	}
	#endregion
	#region DepreciatedPartTotalUsage
	public class DepreciatedPartTotalUsage<Value, AssetID> : BqlFormulaEvaluator<Value, AssetID>
		where Value : IBqlOperand
		where AssetID : IBqlOperand
	{
		public override object Evaluate(PXCache cache, object item, Dictionary<Type, object> pars)
		{
			decimal? val = (decimal?)pars[typeof(Value)];
			int? assetID = (int?)pars[typeof(AssetID)];

			FADetails det = PXSelect<FADetails, Where<FADetails.assetID, Equal<Required<FADetails.assetID>>>>.Select(cache.Graph, assetID);
			return det == null ? (object) null : ((val/det.TotalExpectedUsage) ?? 0m);
		}
	}
	#endregion

	public class IsAcquiredAsset<AssetID> : BqlFormulaEvaluator<AssetID>, IBqlOperand
		where AssetID : IBqlOperand
	{
		public override object Evaluate(PXCache cache, object item, Dictionary<Type, object> pars)
		{
			int? assetID = pars[typeof(AssetID)] as int?;
			if (assetID != null)
			{
				FATran released = PXSelect<FATran, Where<FATran.assetID, Equal<Required<FixedAsset.assetID>>, And<FATran.released, Equal<True>>>>.SelectSingleBound(cache.Graph, new object[0], assetID);
				return released != null;
			}
			return null;
		}
	}
	#endregion
}
