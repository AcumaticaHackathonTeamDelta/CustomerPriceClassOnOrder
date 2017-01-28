using System;
using System.Collections.Generic;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.GL;
using PX.Objects.CS;
using PX.Objects.CA;
using System.Collections;
using PX.Data.EP;
using PX.Objects.CR;
using System.Linq;
using APQuickCheck = PX.Objects.AP.Standalone.APQuickCheck;
using ARCashSale = PX.Objects.AR.Standalone.ARCashSale;

namespace PX.Objects.TX
{
	[TableAndChartDashboardType]
	public class ProcessSVATBase : PXGraph<ProcessSVATBase>
	{
		public PXCancel<SVATTaxFilter> Cancel;
		public PXFilter<SVATTaxFilter> Filter;
		public PXAction<SVATTaxFilter> viewDocument;

		[PXFilterable]
		public PXFilteredProcessingOrderBy<SVATConversionHistExt, SVATTaxFilter,
			OrderBy<Asc<SVATConversionHistExt.module,
				Asc<SVATConversionHistExt.adjdRefNbr,
				Asc<SVATConversionHistExt.adjdDocType,
				Asc<SVATConversionHistExt.adjgRefNbr,
				Asc<SVATConversionHistExt.adjgDocType>>>>>>> SVATDocuments;

		Dictionary<object, object> _copies = new Dictionary<object, object>();

		public override bool IsDirty
		{
			get
			{
				return false;
			}
		}

		public override void Clear()
		{
			Filter.Current.TotalTaxAmount = 0m;
			base.Clear();
		}

		public ProcessSVATBase()
		{
			PXUIFieldAttribute.SetEnabled<SVATConversionHistExt.taxInvoiceDate>(SVATDocuments.Cache, null, true);
			PXUIFieldAttribute.SetEnabled<SVATConversionHistExt.taxInvoiceNbr>(SVATDocuments.Cache, null, true);
		}

		[PXUIField(MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXLookupButton]
		public virtual IEnumerable ViewDocument(PXAdapter adapter)
		{
			if (!string.IsNullOrEmpty(SVATDocuments.Current?.AdjdDocType) &&
				!string.IsNullOrEmpty(SVATDocuments.Current?.AdjdRefNbr))
			{
				PXRedirectHelper.TryRedirect(SVATDocuments.Cache, SVATDocuments.Current, Messages.Document, PXRedirectHelper.WindowMode.NewWindow);
			}

			return adapter.Get();
		}

		public virtual IEnumerable sVATDocuments()
		{
			SVATTaxFilter filter = Filter.Current;
			if (filter != null)
			{
				PXSelectBase<SVATConversionHistExt> sel = new PXSelect<SVATConversionHistExt,
					Where<SVATConversionHistExt.processed, NotEqual<True>>>(this);

				if (filter.ReversalMethod != null)
				{
					switch (filter.ReversalMethod)
					{
						case SVATTaxReversalMethods.OnPayments:
							sel.WhereAnd<Where<SVATConversionHistExt.module, Equal<BatchModule.moduleCA>,
								Or<SVATConversionHistExt.adjdDocType, Equal<ARDocType.cashSale>,
								Or<SVATConversionHistExt.adjdDocType, Equal<ARDocType.cashReturn>,
								Or<SVATConversionHistExt.adjdDocType, Equal<APDocType.quickCheck>,
								Or<SVATConversionHistExt.adjdDocType, Equal<APDocType.voidQuickCheck>,
								Or<Where<SVATConversionHistExt.reversalMethod, Equal<SVATTaxReversalMethods.onPayments>,
									And<Where<SVATConversionHistExt.adjdDocType, NotEqual<SVATConversionHistExt.adjgDocType>,
										Or<SVATConversionHistExt.adjdRefNbr, NotEqual<SVATConversionHistExt.adjgRefNbr>>>>>>>>>>>>();
							break;

						case SVATTaxReversalMethods.OnDocuments:
							sel.WhereAnd<Where<SVATConversionHistExt.reversalMethod, Equal<SVATTaxReversalMethods.onDocuments>,
								And<Where<SVATConversionHistExt.adjdDocType, Equal<SVATConversionHistExt.adjgDocType>,
									And<SVATConversionHistExt.adjdRefNbr, Equal<SVATConversionHistExt.adjgRefNbr>>>>>>();
							break;
					}
				}

				if (filter.TaxAgencyID != null)
				{
					sel.WhereAnd<Where<SVATConversionHistExt.vendorID, Equal<Current<SVATTaxFilter.taxAgencyID>>>>();
				}
				else
				{
					sel.WhereAnd<Where<SVATConversionHistExt.vendorID, IsNull>>();
				}

				if (filter.Date != null)
				{
					sel.WhereAnd<Where<SVATConversionHistExt.adjdDocDate, LessEqual<Current<SVATTaxFilter.date>>>>();
				}

				if (filter.BranchID != null)
				{
					sel.WhereAnd<Where<SVATConversionHistExt.adjdBranchID, Equal<Current<SVATTaxFilter.branchID>>>>();
				}

				if (filter.TaxID != null)
				{
					sel.WhereAnd<Where<SVATConversionHistExt.taxID, Equal<Current<SVATTaxFilter.taxID>>>>();
				}

				FillSVATDocumentsQuery(sel);

				foreach (SVATConversionHistExt hist in sel.Select())
		{
					yield return hist;

					if (_copies.ContainsKey(hist))
					{
						_copies.Remove(hist);
					}
					_copies.Add(hist, PXCache<SVATConversionHistExt>.CreateCopy(hist));
				}

				SVATDocuments.Cache.IsDirty = false;
			}
		}

		public virtual void FillSVATDocumentsQuery(PXSelectBase<SVATConversionHistExt> sel)
		{
		}

		protected virtual void SVATTaxFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			SVATTaxFilter filter = (SVATTaxFilter)e.Row;
			if (filter == null) return;

			SVATDocuments.SetProcessDelegate(list => ProcessPendingVATProc(list, filter));
		}

		protected virtual void SVATTaxFilter_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			Filter.Current.TotalTaxAmount = 0m;
			SVATDocuments.Cache.Clear();
			SVATDocuments.Cache.ClearQueryCache();
		}

		protected virtual void SVATConversionHistExt_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			SVATTaxFilter filter = Filter.Current;
			SVATConversionHistExt hist = e.Row as SVATConversionHistExt;

			if (filter == null || hist == null)
				return;

			if (string.IsNullOrEmpty(hist.TaxInvoiceNbr))
			{
				switch (hist.DisplayTaxEntryRefNbr)
				{
					case VendorSVATTaxEntryRefNbr.DocumentRefNbr:
						hist.TaxInvoiceNbr = hist.AdjdRefNbr;
						break;

					case VendorSVATTaxEntryRefNbr.PaymentRefNbr:
						hist.TaxInvoiceNbr = hist.AdjgRefNbr;
						break;
				}
		}

			if (hist.TaxInvoiceDate == null)
		{
				hist.TaxInvoiceDate = filter.ReversalMethod == SVATTaxReversalMethods.OnDocuments ? filter.Date : hist.AdjdDocDate;
			}

			PXUIFieldAttribute.SetEnabled<SVATConversionHistExt.taxInvoiceNbr>(sender, null, hist.DisplayTaxEntryRefNbr != VendorSVATTaxEntryRefNbr.TaxInvoiceNbr);
		}

		protected virtual void SVATConversionHistExt_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			SVATTaxFilter filter = Filter.Current;
			if (filter != null)
			{
				object OldRow = e.OldRow;
				if (object.ReferenceEquals(e.Row, e.OldRow) && !_copies.TryGetValue(e.Row, out OldRow))
				{
					decimal? val = 0m;
					foreach (SVATConversionHistExt res in SVATDocuments.Select())
					{
						if (res.Selected == true)
						{
							val += res.TaxAmt ?? 0m;
						}
					}

					filter.TotalTaxAmount = val;
				}
				else
				{
					SVATConversionHistExt old_row = OldRow as SVATConversionHistExt;
					SVATConversionHistExt new_row = e.Row as SVATConversionHistExt;

					filter.TotalTaxAmount -= old_row?.Selected == true ? old_row?.TaxAmt : 0m;
					filter.TotalTaxAmount += new_row?.Selected == true ? new_row?.TaxAmt : 0m;
				}
			}
		}

		public static void ProcessPendingVATProc(List<SVATConversionHistExt> list, SVATTaxFilter filter)
		{
			ProcessSVATBase svat = PXGraph.CreateInstance<ProcessSVATBase>();
			JournalEntry je = PXGraph.CreateInstance<JournalEntry>();

			PXCache dummycache = je.Caches[typeof(TaxTran)];
			dummycache = je.Caches[typeof(SVATTaxTran)];
			je.Views.Caches.Add(typeof(SVATTaxTran));

			int index = 0;
			bool failed = false;
			Dictionary<AdjKey, string> taxInvoiceNbrs = new Dictionary<AdjKey, string>();
			Dictionary<string, Tuple<Batch, int>> batches = new Dictionary<string, Tuple<Batch, int>>();

			foreach (SVATConversionHistExt histSVAT in list)
			{
				AdjKey adjKey = new AdjKey
				{
					Module = histSVAT.Module,
					AdjdDocType = histSVAT.AdjdDocType,
					AdjdRefNbr = histSVAT.AdjdRefNbr,
					AdjgDocType = histSVAT.AdjgDocType,
					AdjgRefNbr = histSVAT.AdjgRefNbr,
					AdjNbr = histSVAT.AdjNbr ?? 0
				};

				try
				{
					je.Clear(PXClearOption.ClearAll);

					using (PXTransactionScope ts = new PXTransactionScope())
					{
						Vendor vendor = PXSelect<Vendor, Where<Vendor.bAccountID,
							Equal<Required<Vendor.bAccountID>>>>.SelectSingleBound(svat, null, histSVAT.VendorID);

						string TaxInvoiceNbr;
						if (taxInvoiceNbrs.TryGetValue(adjKey, out TaxInvoiceNbr))
						{
							histSVAT.TaxInvoiceNbr = TaxInvoiceNbr;
						}
						else
						{
							if (histSVAT.TaxType == TaxType.PendingSales &&
								vendor?.SVATOutputTaxEntryRefNbr == VendorSVATTaxEntryRefNbr.TaxInvoiceNbr)
							{
								histSVAT.TaxInvoiceNbr = AutoNumberAttribute.GetNextNumber(svat.SVATDocuments.Cache, null,
									vendor.SVATTaxInvoiceNumberingID, svat.Accessinfo.BusinessDate);
							}
							taxInvoiceNbrs.Add(adjKey, histSVAT.TaxInvoiceNbr);
						}

						if (string.IsNullOrEmpty(histSVAT.TaxInvoiceNbr) == true || histSVAT.TaxInvoiceDate == null)
						{
							PXProcessing<SVATConversionHistExt>.SetError(Messages.CannotProcessW);
							failed = true;
							continue;
						}

						TaxTran prev_taxtran = null;
						string strFinPeriodID = FinPeriodSelectorAttribute.PeriodFromDate(histSVAT.TaxInvoiceDate);

						foreach (PXResult<SVATTaxTran, CurrencyInfo, Tax> res in PXSelectJoin<SVATTaxTran,
							InnerJoin<CurrencyInfo, On<CurrencyInfo.curyInfoID, Equal<SVATTaxTran.curyInfoID>>,
							InnerJoin<Tax, On<Tax.taxID, Equal<SVATTaxTran.taxID>>>>,
							Where<SVATTaxTran.module, Equal<Current<SVATConversionHistExt.module>>,
								And2<Where<SVATTaxTran.vendorID, Equal<Current<SVATConversionHistExt.vendorID>>,
									Or<SVATTaxTran.vendorID, IsNull,
									And<Current<SVATConversionHistExt.vendorID>, IsNull>>>,
								And<SVATTaxTran.tranType, Equal<Current<SVATConversionHistExt.adjdDocType>>,
								And<SVATTaxTran.refNbr, Equal<Current<SVATConversionHistExt.adjdRefNbr>>,
								And<SVATTaxTran.taxID, Equal<Current<SVATConversionHistExt.taxID>>>>>>>>
							.SelectSingleBound(je, new object[] { histSVAT }))
						{
							SVATTaxTran taxtran = res;
							CurrencyInfo info = res;
							Tax tax = res;

							je.created.Consolidate = je.glsetup.Current.ConsolidatedPosting ?? false;
							je.Segregate(BatchModule.GL, taxtran.BranchID, info.CuryID, info.CuryEffDate, 
								histSVAT.TaxInvoiceDate, strFinPeriodID, null, info.CuryRate, info.CuryRateTypeID, null);

							taxtran.TaxInvoiceNbr = histSVAT.TaxInvoiceNbr;
							taxtran.TaxInvoiceDate = histSVAT.TaxInvoiceDate;

							PXCache taxtranCache = je.Caches[typeof(SVATTaxTran)];
							taxtranCache.Update(taxtran);

							CurrencyInfo new_info = PXCache<CurrencyInfo>.CreateCopy(info);
							new_info.CuryInfoID = null;
							new_info.ModuleCode = BatchModule.GL;
							new_info.BaseCalc = false;
							new_info = je.currencyinfo.Insert(new_info) ?? new_info;

							bool drCr = (ReportTaxProcess.GetMult(taxtran) == 1m);
							decimal tranMult = ReportTaxProcess.GetMultByTranType(taxtran.Module, taxtran.TranType);

							decimal curyTaxableAmt = (histSVAT.CuryTaxableAmt ?? 0m) * tranMult;
							decimal taxableAmt = (histSVAT.TaxableAmt ?? 0m) * tranMult;
							decimal curyTaxAmt = (histSVAT.CuryTaxAmt ?? 0m) * tranMult;
							decimal taxAmt = (histSVAT.TaxAmt ?? 0m) * tranMult;

							#region reverse original transaction
							{
								GLTran tran = new GLTran();
								tran.BranchID = taxtran.BranchID;
								tran.AccountID = taxtran.AccountID;
								tran.SubID = taxtran.SubID;
								tran.CuryDebitAmt = drCr ? curyTaxAmt : 0m;
								tran.DebitAmt = drCr ? taxAmt : 0m;
								tran.CuryCreditAmt = drCr ? 0m : curyTaxAmt;
								tran.CreditAmt = drCr ? 0m : taxAmt;
								tran.TranType = taxtran.TranType;
								tran.TranClass = GLTran.tranClass.Normal;
								tran.RefNbr = taxtran.RefNbr;
								tran.TranDesc = taxtran.TaxInvoiceNbr;
								tran.TranPeriodID = strFinPeriodID;
								tran.FinPeriodID = strFinPeriodID;
								tran.TranDate = taxtran.TaxInvoiceDate;
								tran.CuryInfoID = new_info.CuryInfoID;
								tran.Released = true;

								je.GLTranModuleBatNbr.Insert(tran);

								SVATTaxTran newtaxtran = PXCache<SVATTaxTran>.CreateCopy(taxtran);
								newtaxtran.RecordID = null;
								newtaxtran.Module = BatchModule.GL;
								newtaxtran.TranType = (taxtran.TaxType == TaxType.PendingSales) ? TaxAdjustmentType.ReverseOutputVAT : TaxAdjustmentType.ReverseInputVAT;
								newtaxtran.RefNbr = newtaxtran.TaxInvoiceNbr;
								newtaxtran.TranDate = newtaxtran.TaxInvoiceDate;

								decimal tranSign = (-1m) * ReportTaxProcess.GetMult(taxtran) * ReportTaxProcess.GetMult(newtaxtran);
								newtaxtran.CuryTaxableAmt = tranSign * curyTaxableAmt;
								newtaxtran.TaxableAmt = tranSign * taxableAmt;
								newtaxtran.CuryTaxAmt = tranSign * curyTaxAmt;
								newtaxtran.TaxAmt = tranSign * taxAmt;

								taxtranCache.Insert(newtaxtran);
							}
							#endregion

							#region reclassify to VAT account
							{
								GLTran tran = new GLTran();
								tran.BranchID = taxtran.BranchID;
								tran.AccountID = (taxtran.TaxType == TaxType.PendingSales) ? tax.SalesTaxAcctID : tax.PurchTaxAcctID;
								tran.SubID = (taxtran.TaxType == TaxType.PendingSales) ? tax.SalesTaxSubID : tax.PurchTaxSubID;
								tran.CuryDebitAmt = drCr ? 0m : curyTaxAmt;
								tran.DebitAmt = drCr ? 0m : taxAmt;
								tran.CuryCreditAmt = drCr ? curyTaxAmt : 0m;
								tran.CreditAmt = drCr ? taxAmt : 0m;
								tran.TranType = taxtran.TranType;
								tran.TranClass = GLTran.tranClass.Normal;
								tran.RefNbr = taxtran.RefNbr;
								tran.TranDesc = taxtran.TaxInvoiceNbr;
								tran.TranPeriodID = strFinPeriodID;
								tran.FinPeriodID = strFinPeriodID;
								tran.TranDate = taxtran.TaxInvoiceDate;
								tran.CuryInfoID = new_info.CuryInfoID;
								tran.Released = true;

								je.GLTranModuleBatNbr.Insert(tran);

								SVATTaxTran newtaxtran = PXCache<SVATTaxTran>.CreateCopy(taxtran);
								newtaxtran.RecordID = null;
								newtaxtran.Module = BatchModule.GL;
								newtaxtran.TranType = (taxtran.TaxType == TaxType.PendingSales) ? TaxAdjustmentType.OutputVAT : TaxAdjustmentType.InputVAT;
								newtaxtran.TaxType = (taxtran.TaxType == TaxType.PendingSales) ? TaxType.Sales : TaxType.Purchase;
								newtaxtran.AccountID = (taxtran.TaxType == TaxType.PendingSales) ? tax.SalesTaxAcctID : tax.PurchTaxAcctID;
								newtaxtran.SubID = (taxtran.TaxType == TaxType.PendingSales) ? tax.SalesTaxSubID : tax.PurchTaxSubID;
								newtaxtran.RefNbr = newtaxtran.TaxInvoiceNbr;
								newtaxtran.TranDate = newtaxtran.TaxInvoiceDate;

								decimal tranSign = ReportTaxProcess.GetMult(taxtran) * ReportTaxProcess.GetMult(newtaxtran);
								newtaxtran.CuryTaxableAmt = tranSign * curyTaxableAmt;
								newtaxtran.TaxableAmt = tranSign * taxableAmt;
								newtaxtran.CuryTaxAmt = tranSign * curyTaxAmt;
								newtaxtran.TaxAmt = tranSign * taxAmt;

								prev_taxtran = (TaxTran)taxtranCache.Insert(newtaxtran);
							}
							#endregion
						}

						if (histSVAT.ReversalMethod == SVATTaxReversalMethods.OnPayments)
						{
							SVATConversionHist docSVAT = PXSelect<SVATConversionHist,
								Where<SVATConversionHist.module, Equal<Current<SVATConversionHist.module>>,
									And<SVATConversionHist.adjdDocType, Equal<Current<SVATConversionHist.adjdDocType>>,
									And<SVATConversionHist.adjdRefNbr, Equal<Current<SVATConversionHist.adjdRefNbr>>,
									And<SVATConversionHist.adjgDocType, Equal<SVATConversionHist.adjdDocType>,
									And<SVATConversionHist.adjgRefNbr, Equal<SVATConversionHist.adjdRefNbr>,
									And<SVATConversionHist.adjNbr, Equal<decimal_1>,
									And<SVATConversionHist.taxID, Equal<Current<SVATConversionHist.taxID>>>>>>>>>>
								.SelectSingleBound(svat, new object[] { histSVAT });

							docSVAT.CuryUnrecognizedTaxAmt -= histSVAT.CuryTaxAmt;
							docSVAT.UnrecognizedTaxAmt -= histSVAT.TaxAmt;

							if (docSVAT.CuryUnrecognizedTaxAmt == 0m)
							{
								docSVAT.Processed = true;
								docSVAT.AdjgFinPeriodID = strFinPeriodID;
							}

							svat.SVATDocuments.Cache.Update(docSVAT);
						}

						je.Save.Press();

						histSVAT.Processed = true;
						histSVAT.CuryUnrecognizedTaxAmt = 0m;
						histSVAT.UnrecognizedTaxAmt = 0m;
						histSVAT.TaxRecordID = prev_taxtran?.RecordID;
						histSVAT.AdjBatchNbr = je.BatchModule.Current?.BatchNbr;
						histSVAT.AdjgFinPeriodID = strFinPeriodID;
						svat.SVATDocuments.Cache.Update(histSVAT);

						svat.Persist();
						ts.Complete();
						
						if (je.BatchModule.Current != null && !batches.ContainsKey(histSVAT.AdjBatchNbr))
						{
							batches.Add(histSVAT.AdjBatchNbr, new Tuple<Batch, int>(je.BatchModule.Current, index));
						}
					}

					PXProcessing<SVATConversionHistExt>.SetInfo(index, ActionsMessages.RecordProcessed);
				}
				catch (Exception e)
				{
					PXProcessing<SVATConversionHistExt>.SetError(index, e);
					failed = true;
					taxInvoiceNbrs.Remove(adjKey);
				}

				index++;
			}

			if (je.glsetup.Current.AutoPostOption == true)
			{
				PostGraph pg = PXGraph.CreateInstance<PostGraph>();
				foreach (Tuple<Batch, int> tuple in batches.Values)
				{
					try
					{
						pg.Clear();
						pg.PostBatchProc(tuple.Item1);
					}	
					catch (Exception e)
					{
						PXProcessing<SVATConversionHistExt>.SetError(tuple.Item2, e);
						failed = true;
					}
				}
			}

			if (filter.ReversalMethod == SVATTaxReversalMethods.OnPayments)
			{
				foreach (AdjKey key in taxInvoiceNbrs.Keys)
				{
					string taxInvoiceNbr = taxInvoiceNbrs[key];
					switch (key.Module)
					{
						case BatchModule.AP:
							PXUpdate<Set<APAdjust.taxInvoiceNbr, Required<APAdjust.taxInvoiceNbr>>, APAdjust,
							Where<APAdjust.adjdDocType, Equal<Required<APAdjust.adjdDocType>>,
								And<APAdjust.adjdRefNbr, Equal<Required<APAdjust.adjdRefNbr>>,
								And<APAdjust.adjgDocType, Equal<Required<APAdjust.adjgDocType>>,
								And<APAdjust.adjgRefNbr, Equal<Required<APAdjust.adjgRefNbr>>,
								And<APAdjust.adjNbr, Equal<Required<APAdjust.adjNbr>>>>>>>>
								.Update(svat, taxInvoiceNbr, key.AdjdDocType, key.AdjdRefNbr, key.AdjgDocType, key.AdjgRefNbr, key.AdjNbr);
							break;

						case BatchModule.AR:
							PXUpdate<Set<ARAdjust.taxInvoiceNbr, Required<ARAdjust.taxInvoiceNbr>>, ARAdjust,
							Where<ARAdjust.adjdDocType, Equal<Required<ARAdjust.adjdDocType>>,
								And<ARAdjust.adjdRefNbr, Equal<Required<ARAdjust.adjdRefNbr>>,
								And<ARAdjust.adjgDocType, Equal<Required<ARAdjust.adjgDocType>>,
								And<ARAdjust.adjgRefNbr, Equal<Required<ARAdjust.adjgRefNbr>>,
								And<ARAdjust.adjNbr, Equal<Required<ARAdjust.adjNbr>>>>>>>>
								.Update(svat, taxInvoiceNbr, key.AdjdDocType, key.AdjdRefNbr, key.AdjgDocType, key.AdjgRefNbr, key.AdjNbr);
							break;
					}
				}
			}

			if (failed)
			{
				throw new PXException(GL.Messages.DocumentsNotReleased);
			}
		}
	}

	public class ProcessOutputSVAT : ProcessSVATBase
	{
		public ProcessOutputSVAT()
		{
		}

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXUIField(DisplayName = "Customer", Visible = false)]
		protected virtual void SVATConversionHistExt_DisplayCounterPartyID_CacheAttached(PXCache sender)
		{
		}

		public override void FillSVATDocumentsQuery(PXSelectBase<SVATConversionHistExt> sel)
		{
			sel.WhereAnd<Where<SVATConversionHistExt.taxType, Equal<TaxType.pendingSales>>>();
		}
	}

	public class ProcessInputSVAT : ProcessSVATBase
	{
		public ProcessInputSVAT()
		{
		}

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXUIField(DisplayName = "Vendor", Visible = false)]
		protected virtual void SVATConversionHistExt_DisplayCounterPartyID_CacheAttached(PXCache sender)
		{
		}

		public override void FillSVATDocumentsQuery(PXSelectBase<SVATConversionHistExt> sel)
		{
			sel.WhereAnd<Where<SVATConversionHistExt.taxType, Equal<TaxType.pendingPurchase>>>();
		}
	}

	public struct AdjKey
	{
		public string Module;
		public string AdjdDocType;
		public string AdjdRefNbr;
		public string AdjgDocType;
		public string AdjgRefNbr;
		public int AdjNbr;
	}

	[Serializable]
	public partial class SVATTaxFilter : IBqlTable
	{
		#region Date
		public abstract class date : IBqlField
	{
		}
		[PXDBDate]
		[PXDefault(typeof(AccessInfo.businessDate), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Date")]
		public virtual DateTime? Date
		{
			get;
			set;
		}
		#endregion
		#region BranchID
		public abstract class branchID : IBqlField
		{
		}
		[Branch(Required = false)]
		public virtual int? BranchID
		{
			get;
			set;
		}
		#endregion
		#region TaxAgencyID
		public abstract class taxAgencyID : IBqlField
		{
		}
		[Vendor(typeof(Search<Vendor.bAccountID, Where<Vendor.taxAgency, Equal<True>>>),
			DisplayName = "Tax Agency", DescriptionField = typeof(Vendor.acctName), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual int? TaxAgencyID
			{
			get;
			set;
			}
		#endregion
		#region TaxID
		public abstract class taxID : IBqlField
			{
			}
		[PXDBString(Tax.taxID.Length, IsUnicode = true)]
		[PXUIField(DisplayName = "Tax ID")]
		[PXSelector(typeof(Search<Tax.taxID, Where<Tax.isExternal, NotEqual<True>>>), DescriptionField = typeof(Tax.descr))]
		public virtual string TaxID
		{
			get;
			set;
		}
		#endregion
		#region ReversalMethod
		public abstract class reversalMethod : IBqlField
		{
		}
		[PXDBString(1, IsFixed = true)]
		[PXDefault(SVATTaxReversalMethods.OnDocuments)]
		[PXFormula(typeof(IsNull<Selector<SVATTaxFilter.taxAgencyID, Vendor.sVATReversalMethod>, SVATTaxReversalMethods.onDocuments>))]
		[SVATTaxReversalMethods.List]
		[PXUIField(DisplayName = "VAT Recognition Method")]
		public virtual string ReversalMethod
		{
			get;
			set;
		}
		#endregion
		#region TotalTaxAmount
		public abstract class totalTaxAmount : IBqlField
			{
			}
		[PXDBBaseCury]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Total Tax Amount", Enabled = false)]
		public virtual decimal? TotalTaxAmount
			{
			get;
			set;
			}
		#endregion
	}

	[Serializable]
	[PXHidden]
	public partial class SVATTaxTran : TaxTran
	{
		#region Selected
		public new abstract class selected : IBqlField
		{
		}
		#endregion
		#region Module
		public new abstract class module : IBqlField
		{
		}
		[PXDBString(2, IsKey = true, IsFixed = true)]
		[PXDefault]
		public override string Module
		{
			get;
			set;
		}
		#endregion
		#region TranType
		public new abstract class tranType : IBqlField
		{
		}
		[PXDBString(3, IsKey = true, IsFixed = true)]
		[PXDefault]
		public override string TranType
			{
			get;
			set;
			}
		#endregion
		#region RefNbr
		public new abstract class refNbr : IBqlField
			{
			}
		[PXDBString(15, IsUnicode = true, IsKey = true)]
		[PXDefault]
		public override string RefNbr
		{
			get;
			set;
		}
		#endregion
		#region Released
		public new abstract class released : IBqlField
		{
		}
		#endregion
		#region Voided
		public new abstract class voided : IBqlField
		{
		}
		#endregion
		#region TaxPeriodID
		public new abstract class taxPeriodID : IBqlField
		{
		}
		[FinPeriodID]
		public override string TaxPeriodID
			{
			get;
			set;
		}
		#endregion
		#region FinPeriodID
		public new abstract class finPeriodID : IBqlField
		{
			}
		[FinPeriodID]
		[PXDefault]
		public override string FinPeriodID
			{
			get;
			set;
		}
		#endregion
		#region TaxID
		public new abstract class taxID : IBqlField
		{
		}
		[PXDBString(Tax.taxID.Length, IsUnicode = true, IsKey = true)]
		[PXDefault]
		public override string TaxID
			{
			get;
			set;
		}
		#endregion
		#region VendorID
		public new abstract class vendorID : IBqlField
		{
			}
		[PXDBInt]
		public override int? VendorID
			{
			get;
			set;
		}
		#endregion
		#region TaxZoneID
		public new abstract class taxZoneID : IBqlField
		{
		}
		[PXDBString(10, IsUnicode = true)]
		[PXDefault]
		public override string TaxZoneID
			{
			get;
			set;
		}
		#endregion
		#region AccountID
		public new abstract class accountID : IBqlField
			{
			}
		[PXDBInt]
		[PXDefault]
		public override int? AccountID
			{
			get;
			set;
		}
		#endregion
		#region SubID
		public new abstract class subID : IBqlField
		{
		}
		[PXDBInt]
		[PXDefault]
		public override int? SubID
			{
			get;
			set;
		}
		#endregion
		#region TranDate
		public new abstract class tranDate : IBqlField
		{
		}
		[PXDBDate]
		[PXDefault]
		public override DateTime? TranDate
		{
			get;
			set;
		}
		#endregion
		#region TaxType
		public new abstract class taxType : IBqlField
		{
		}
		[PXDBString(1, IsFixed = true)]
		[PXDefault]
		public override string TaxType
		{
			get;
			set;
		}
		#endregion
		#region TaxBucketID
		public new abstract class taxBucketID : IBqlField
		{
		}
		[PXDBInt]
		[PXDefault]
		public override int? TaxBucketID
		{
			get;
			set;
		}
		#endregion
		#region TaxInvoiceNbr
		public new abstract class taxInvoiceNbr : IBqlField
		{
		}
		#endregion
		#region TaxInvoiceDate
		public new abstract class taxInvoiceDate : IBqlField
		{
		}
		#endregion
		#region CuryInfoID
		public new abstract class curyInfoID : IBqlField
		{
			}
		[PXDBLong]
		[PXDefault]
		public override long? CuryInfoID
			{
			get;
			set;
		}
		#endregion
		#region CuryTaxableAmt
		public new abstract class curyTaxableAmt : IBqlField
		{
		}
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public override decimal? CuryTaxableAmt
			{
			get;
			set;
		}
		#endregion
		#region TaxableAmt
		public new abstract class taxableAmt : IBqlField
		{
		}
		#endregion
		#region CuryTaxAmt
		public new abstract class curyTaxAmt : IBqlField
		{
		}
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public override decimal? CuryTaxAmt
		{
			get;
			set;
		}
		#endregion
		#region TaxAmt
		public new abstract class taxAmt : IBqlField
		{
		}
		#endregion
	}

	[Serializable]
	[PXPrimaryGraph(new Type[]
	{
		typeof(CATranEntry),
		typeof(APQuickCheckEntry),
		typeof(APInvoiceEntry),
		typeof(ARCashSaleEntry),
		typeof(ARInvoiceEntry)
	},
	new Type[]
	{
		typeof(Select<CAAdj, Where<Current<SVATConversionHistExt.module>, Equal<BatchModule.moduleCA>,
			And<CAAdj.adjTranType, Equal<Current<SVATConversionHistExt.adjdDocType>>,
			And<CAAdj.adjRefNbr, Equal<Current<SVATConversionHistExt.adjdRefNbr>>>>>>),
		typeof(Select<APQuickCheck, Where<Current<SVATConversionHistExt.module>, Equal<BatchModule.moduleAP>,
			And<APQuickCheck.docType, Equal<Current<SVATConversionHistExt.adjdDocType>>,
			And<APQuickCheck.refNbr, Equal<Current<SVATConversionHistExt.adjdRefNbr>>>>>>),
		typeof(Select<APInvoice, Where<Current<SVATConversionHistExt.module>, Equal<BatchModule.moduleAP>,
			And<APInvoice.docType, Equal<Current<SVATConversionHistExt.adjdDocType>>,
			And<APInvoice.refNbr, Equal<Current<SVATConversionHistExt.adjdRefNbr>>>>>>),
		typeof(Select<ARCashSale, Where<Current<SVATConversionHistExt.module>, Equal<BatchModule.moduleAR>,
			And<ARCashSale.docType, Equal<Current<SVATConversionHistExt.adjdDocType>>,
			And<ARCashSale.refNbr, Equal<Current<SVATConversionHistExt.adjdRefNbr>>>>>>),
		typeof(Select<ARInvoice, Where<Current<SVATConversionHistExt.module>, Equal<BatchModule.moduleAR>,
			And<ARInvoice.docType, Equal<Current<SVATConversionHistExt.adjdDocType>>,
			And<ARInvoice.refNbr, Equal<Current<SVATConversionHistExt.adjdRefNbr>>>>>>)
	})]
	[PXProjection(typeof(Select2<SVATConversionHist, 
		LeftJoin<Vendor, On<Vendor.bAccountID, Equal<SVATConversionHist.vendorID>>,
		LeftJoin<APInvoice, On<SVATConversionHist.module, Equal<BatchModule.moduleAP>,
			And<SVATConversionHist.adjdDocType, Equal<APInvoice.docType>,
			And<SVATConversionHist.adjdRefNbr, Equal<APInvoice.refNbr>>>>,
		LeftJoin<ARInvoice, On<SVATConversionHist.module, Equal<BatchModule.moduleAR>, 
			And<SVATConversionHist.adjdDocType, Equal<ARInvoice.docType>,
			And<SVATConversionHist.adjdRefNbr, Equal<ARInvoice.refNbr>>>>,
		LeftJoin<CAAdj, On<SVATConversionHist.module, Equal<BatchModule.moduleCA>, 
			And<SVATConversionHist.adjdDocType, Equal<CAAdj.adjTranType>,
			And<SVATConversionHist.adjdRefNbr, Equal<CAAdj.adjRefNbr>>>>>>>>>), Persistent = true)]
	public partial class SVATConversionHistExt : SVATConversionHist
	{
		#region SVATConversionHist keys

		#region Module
		public new abstract class module : IBqlField
		{
		}
		[PXDBString(2, IsKey = true, IsFixed = true, BqlField = typeof(SVATConversionHist.module))]
		[PXDefault]
		[PXUIField(DisplayName = "Module")]
		[BatchModule.List]
		[PXFieldDescription]
		public override string Module
		{
			get;
			set;
		}
		#endregion
		#region AdjdDocType
		public new abstract class adjdDocType : IBqlField
		{
		}
		[PXDBString(3, IsKey = true, IsFixed = true, BqlField = typeof(SVATConversionHist.adjdDocType))]
		[PXDefault]
		[PXUIField(DisplayName = "Type")]
		public override string AdjdDocType
		{
			get;
			set;
		}
		#endregion
		#region AdjdRefNbr
		public new abstract class adjdRefNbr : IBqlField
			{
			}
		[PXDBString(15, IsUnicode = true, IsKey = true, BqlField = typeof(SVATConversionHist.adjdRefNbr))]
		[PXDefault]
		[PXUIField(DisplayName = "Reference Nbr.")]
		public override string AdjdRefNbr
			{
			get;
			set;
		}
		#endregion
		#region AdjgDocType
		public new abstract class adjgDocType : IBqlField
		{
		}
		[PXDBString(3, IsKey = true, IsFixed = true, BqlField = typeof(SVATConversionHist.adjgDocType))]
		[PXDefault(typeof(SVATConversionHist.adjdDocType))]
		[PXUIField(DisplayName = "AdjgDocType")]
		public override string AdjgDocType
		{
			get;
			set;
		}
		#endregion
		#region AdjgRefNbr
		public new abstract class adjgRefNbr : IBqlField
			{
			}
		[PXDBString(15, IsUnicode = true, IsKey = true, BqlField = typeof(SVATConversionHist.adjgRefNbr))]
		[PXDefault(typeof(SVATConversionHist.adjdRefNbr))]
		[PXUIField(DisplayName = "AdjgRefNbr")]
		public override string AdjgRefNbr
			{
			get;
			set;
		}
		#endregion
		#region AdjNbr
		public new abstract class adjNbr : IBqlField
		{
		}
		[PXDBInt(IsKey = true, BqlField = typeof(SVATConversionHist.adjNbr))]
		[PXDefault(-1)]
		[PXUIField(DisplayName = "Adjustment Nbr.")]
		public override int? AdjNbr
		{
			get;
			set;
		}
		#endregion
		#region TaxID
		public new abstract class taxID : IBqlField
		{
		}
		[PXDBString(Tax.taxID.Length, IsUnicode = true, IsKey = true, BqlField = typeof(SVATConversionHist.taxID))]
		[PXUIField(DisplayName = "Tax ID")]
		public override string TaxID
		{
			get;
			set;
		}
		#endregion

		#endregion

		#region Status
		public abstract class status : IBqlField
		{
		}
		[PXString(1, IsFixed = true)]
		[PXDBCalced(typeof(Switch<Case<Where<SVATConversionHist.module, Equal<BatchModule.moduleCA>>, CAAdj.status,
			Case<Where<SVATConversionHist.module, Equal<BatchModule.moduleAP>>, APInvoice.status,
			Case<Where<SVATConversionHist.module, Equal<BatchModule.moduleAR>>, ARInvoice.status>>>>), typeof(string))]
		public virtual string Status
		{
			get;
			set;
		}
		#endregion
		#region DisplayStatus
		public abstract class displayStatus : IBqlField
		{
		}
		[PXString(3, IsFixed = true)]
		[PXUIField(DisplayName = "Status")]
		[SVATHistStatus.List]
		public virtual string DisplayStatus
		{
			[PXDependsOnFields(typeof(SVATConversionHistExt.module),
				typeof(SVATConversionHistExt.status))]
			get
			{
				return this.Module + this.Status;
			}
			set
			{
			}
		}
		#endregion
		#region DisplayDocType
		public abstract class displayDocType : IBqlField
		{
		}
		[PXString(5, IsFixed = true)]
		[PXUIField(DisplayName = "Type")]
		[SVATHistDocType.List]
		public virtual string DisplayDocType
		{
			[PXDependsOnFields(typeof(SVATConversionHistExt.module), 
				typeof(SVATConversionHistExt.adjdDocType))]
			get
		{
				return this.Module + this.AdjdDocType;
		}
			set
		{
			}
		}
		#endregion
		#region DisplayCounterPartyID
		public abstract class displayCounterPartyID : IBqlField
		{
		}
		[PXInt]
		[PXUIField(DisplayName = "Customer/Vendor", Visible = false)]
		[PXDBCalced(typeof(Switch<Case<Where<SVATConversionHist.module, Equal<BatchModule.moduleAP>>, APInvoice.vendorID,
			Case<Where<SVATConversionHist.module, Equal<BatchModule.moduleAR>>, ARInvoice.customerID>>>), typeof(int))]
		[PXSelector(typeof(Search<BAccountR.bAccountID, Where<BAccountR.bAccountID, Equal<Current<SVATConversionHistExt.displayCounterPartyID>>>>),
			DescriptionField = typeof(BAccountR.acctName), SubstituteKey = typeof(BAccountR.acctCD))]
		public virtual int? DisplayCounterPartyID
		{
			get;
			set;
		}
		#endregion
		#region DisplayDescription
		public abstract class displayDescription : IBqlField
		{
		}
		[PXString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visible = false)]
		[PXDBCalced(typeof(Switch<Case<Where<SVATConversionHist.module, Equal<BatchModule.moduleCA>>, CAAdj.tranDesc,
			Case<Where<SVATConversionHist.module, Equal<BatchModule.moduleAP>>, APInvoice.docDesc,
			Case<Where<SVATConversionHist.module, Equal<BatchModule.moduleAR>>, ARInvoice.docDesc>>>>), typeof(string))]
		public virtual string DisplayDescription
		{
			get;
			set;
		}
		#endregion
		#region DisplayDocRef
		public abstract class displayDocRef : IBqlField
		{
		}
		[PXString(40, IsUnicode = true)]
		[PXUIField(DisplayName = "Document Ref. / Customer Order Nbr.", Visible = false)]
		[PXDBCalced(typeof(Switch<Case<Where<SVATConversionHist.module, Equal<BatchModule.moduleCA>>, CAAdj.extRefNbr,
			Case<Where<SVATConversionHist.module, Equal<BatchModule.moduleAP>>, APInvoice.invoiceNbr,
			Case<Where<SVATConversionHist.module, Equal<BatchModule.moduleAR>>, ARInvoice.invoiceNbr>>>>), typeof(string))]
		public virtual string DisplayDocRef
		{
			get;
			set;
		}
		#endregion
		#region DisplayTaxEntryRefNbr
		public abstract class displayTaxEntryRefNbr : IBqlField
		{
		}
		[PXString(1, IsFixed = true)]
		[PXDBCalced(typeof(IsNull<
			Switch<Case<Where<SVATConversionHist.taxType, Equal<TaxType.pendingPurchase>>, Vendor.sVATInputTaxEntryRefNbr,
				Case<Where<SVATConversionHist.taxType, Equal<TaxType.pendingSales>>, Vendor.sVATOutputTaxEntryRefNbr>>>, 
			VendorSVATTaxEntryRefNbr.manuallyEntered>), typeof(string))]
		public virtual string DisplayTaxEntryRefNbr
		{
			get;
			set;
		}
		#endregion
		#region DisplayOrigDocAmt
		public abstract class displayOrigDocAmt : IBqlField
		{
		}
		[PXBaseCury]
		[PXUIField(DisplayName = "Amount")]
		[PXDBCalced(typeof(Switch<Case<Where<SVATConversionHist.module, Equal<BatchModule.moduleCA>>, CAAdj.tranAmt,
			Case<Where<SVATConversionHist.module, Equal<BatchModule.moduleAP>>, APInvoice.origDocAmt,
			Case<Where<SVATConversionHist.module, Equal<BatchModule.moduleAR>>, ARInvoice.origDocAmt>>>>), typeof(decimal))]
		public virtual decimal? DisplayOrigDocAmt
		{
			get;
			set;
		}
		#endregion
		#region DisplayDocBal
		public abstract class displayDocBal : IBqlField
		{
		}
		[PXBaseCury]
		[PXUIField(DisplayName = "Balance")]
		[PXDBCalced(typeof(Switch<Case<Where<SVATConversionHist.module, Equal<BatchModule.moduleCA>>, decimal0,
			Case<Where<SVATConversionHist.module, Equal<BatchModule.moduleAP>>, APInvoice.docBal,
			Case<Where<SVATConversionHist.module, Equal<BatchModule.moduleAR>>, ARInvoice.docBal>>>>), typeof(decimal))]
		public virtual decimal? DisplayDocBal
		{
			get;
			set;
		}
		#endregion
	}

	public class SVATHistStatus
	{
		public static readonly string[] Values = ARDocStatus.Values.Select(value => BatchModule.AR + value)
				.Concat(APDocStatus.Values.Select(value => BatchModule.AP + value))
				.Concat(CATransferStatus.Values.Select(value => BatchModule.CA + value)).ToArray();
		public static readonly string[] Labels = ARDocStatus.Labels.Concat(APDocStatus.Labels).Concat(CATransferStatus.Labels).ToArray();

		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(Values, Labels)
			{
			}
		}
	}

	public class SVATHistDocType
	{
		public static readonly string[] Values = ARDocType.Values.Select(value => BatchModule.AR + value)
			.Concat(APDocType.Values.Select(value => BatchModule.AP + value))
			.Concat(CATranType.Values.Select(value => BatchModule.CA + value)).ToArray();
		public static readonly string[] Labels = ARDocType.Labels.Concat(APDocType.Labels).Concat(CATranType.Labels).ToArray();

		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(Values, Labels)
			{
			}
		}
	}

	public class SVATTaxReversalMethods
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { OnPayments, OnDocuments },
				new string[] { Messages.OnPayments, Messages.OnDocuments })
			{
			}
		}

		public const string OnPayments = "P";
		public const string OnDocuments = "D";

		public class onPayments : Constant<string>
		{
			public onPayments() : base(OnPayments) { }
		}

		public class onDocuments : Constant<string>
		{
			public onDocuments() : base(OnDocuments) { }
		}
	}
}
