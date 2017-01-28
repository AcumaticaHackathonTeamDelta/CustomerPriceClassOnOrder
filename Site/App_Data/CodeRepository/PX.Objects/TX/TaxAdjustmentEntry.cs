using System;
using System.Collections.Generic;
using System.Collections;

using PX.Data;

using PX.Objects.Common;
using PX.Objects.CM;
using PX.Objects.AP;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.GL;

namespace PX.Objects.TX
{
	public class TaxAdjustmentEntry : PXGraph<TaxAdjustmentEntry, TaxAdjustment>
	{
		public PXSelect<TaxAdjustment, Where<TaxAdjustment.docType, Equal<Optional<TaxAdjustment.docType>>>> Document;
		public PXSelect<TaxAdjustment, Where<TaxAdjustment.docType, Equal<Current<TaxAdjustment.docType>>, And<TaxAdjustment.refNbr, Equal<Current<TaxAdjustment.refNbr>>>>> CurrentDocument;
		public PXSelect<TaxTran, Where<TaxTran.tranType, Equal<Current<TaxAdjustment.docType>>, And<TaxTran.refNbr, Equal<Current<TaxAdjustment.refNbr>>>>> Transactions;
		public PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<TaxAdjustment.curyInfoID>>>> currencyinfo;

		public PXSetup<Vendor, Where<Vendor.bAccountID, Equal<Optional<TaxAdjustment.vendorID>>>> vendor;
		public PXSetup<Location, Where<Location.bAccountID, Equal<Current<TaxAdjustment.vendorID>>, And<Location.locationID, Equal<Optional<TaxAdjustment.vendorLocationID>>>>> location;
		
		public PXSelect<Tax, Where<Tax.taxID, Equal<Required<Tax.taxID>>>> SalesTax_Select;
		public PXSelect<TaxRev, Where<TaxRev.taxID, Equal<Required<TaxRev.taxID>>, And<TaxRev.taxType, Equal<Required<TaxRev.taxType>>, And<TaxRev.outdated, Equal<boolFalse>, And<Required<TaxRev.startDate>, Between<TaxRev.startDate, TaxRev.endDate>>>>>> SalesTaxRev_Select;

		public ToggleCurrency<TaxAdjustment> CurrencyView;

		public PXAction<TaxAdjustment> newVendor;
		[PXUIField(DisplayName = Messages.NewVendor, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable NewVendor(PXAdapter adapter)
		{
			VendorMaint graph = PXGraph.CreateInstance<VendorMaint>();
			throw new PXRedirectRequiredException(graph, Messages.NewVendor);
		}

		public PXAction<TaxAdjustment> editVendor;
		[PXUIField(DisplayName = Messages.EditVendor, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable EditVendor(PXAdapter adapter)
		{
			if (vendor.Current != null)
			{
				VendorMaint graph = PXGraph.CreateInstance<VendorMaint>();
				graph.BAccount.Current = (VendorR)vendor.Current;
				throw new PXRedirectRequiredException(graph, Messages.EditVendor);
			}
			return adapter.Get();
		}

		public PXAction<TaxAdjustment> viewBatch;
		[PXUIField(DisplayName = Messages.ReviewBatch, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable ViewBatch(PXAdapter adapter)
		{
			if (Document.Current != null && !String.IsNullOrEmpty(Document.Current.BatchNbr))
			{
				GL.JournalEntry graph = PXGraph.CreateInstance<GL.JournalEntry>();
				graph.BatchModule.Current = graph.BatchModule.Search<GL.Batch.batchNbr>(Document.Current.BatchNbr, "AP");
				throw new PXRedirectRequiredException(graph, Messages.CurrentBatchRecord);
			}
			return adapter.Get();
		}

		public PXAction<TaxAdjustment> release;
		[PXUIField(DisplayName = Messages.Release, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable Release(PXAdapter adapter)
		{
			PXCache cache = Document.Cache;
			List<TaxAdjustment> list = new List<TaxAdjustment>();
			foreach (TaxAdjustment doc in adapter.Get())
			{
				if (doc.Hold != true && doc.Released != true)
				{
					cache.Update(doc);
					list.Add(doc);
				}
			}
			if (list.Count == 0)
			{
				throw new PXException(Messages.Document_Status_Invalid);
			}
			Save.Press();
			if (list.Count > 0)
			{
				PXLongOperation.StartOperation(this, () => ReportTaxReview.ReleaseDoc(list));
			}
			return list;
		}		

		public PXSetup<APSetup> APSetup;

		public TaxAdjustmentEntry()
		{
			APSetup setup = APSetup.Current;
			PXUIFieldAttribute.SetEnabled<TaxAdjustment.curyID>(Document.Cache, null, false);
			PXUIFieldAttribute.SetVisible<TaxAdjustment.vendorLocationID>(Document.Cache, null, false);
			FieldDefaulting.AddHandler<BAccountR.type>((sender, e) => { if (e.Row != null) e.NewValue = BAccountType.VendorType; });
		}


		#region CurrencyInfo Events
		protected virtual void CurrencyInfo_CuryID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				if (vendor.Current != null && !string.IsNullOrEmpty(vendor.Current.CuryID))
				{
					e.NewValue = vendor.Current.CuryID;
					e.Cancel = true;
				}
			}
		}

		protected virtual void CurrencyInfo_CuryRateTypeID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				if (vendor.Current != null && !string.IsNullOrEmpty(vendor.Current.CuryRateTypeID))
				{
					e.NewValue = vendor.Current.CuryRateTypeID;
					e.Cancel = true;
				}
			}
		}

		protected virtual void CurrencyInfo_CuryEffDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (Document.Cache.Current != null)
			{
				e.NewValue = ((TaxAdjustment)Document.Cache.Current).DocDate;
				e.Cancel = true;
			}
		}
		protected virtual void CurrencyInfo_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			CurrencyInfo info = e.Row as CurrencyInfo;
			if (info != null)
			{
				bool curyenabled = info.AllowUpdate(this.Transactions.Cache);

				if (vendor.Current != null && vendor.Current.AllowOverrideRate != true)
				{
					curyenabled = false;
				}

				PXUIFieldAttribute.SetEnabled<CurrencyInfo.curyID>(sender, null, false);			
				PXUIFieldAttribute.SetEnabled<CurrencyInfo.curyRateTypeID>(sender, info, curyenabled);
				PXUIFieldAttribute.SetEnabled<CurrencyInfo.curyEffDate>(sender, info, curyenabled);
				PXUIFieldAttribute.SetEnabled<CurrencyInfo.sampleCuryRate>(sender, info, curyenabled);
				PXUIFieldAttribute.SetEnabled<CurrencyInfo.sampleRecipRate>(sender, info, curyenabled);
			}
		}
		#endregion

		protected virtual void TaxAdjustment_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			TaxAdjustment doc = e.Row as TaxAdjustment;

			if (doc == null)
			{
				return;
			}		

			if (doc.Released == true)
			{
				PXUIFieldAttribute.SetEnabled(cache, doc, false);
				cache.AllowDelete = false;
				cache.AllowUpdate = false;
				Transactions.Cache.AllowDelete = false;
				Transactions.Cache.AllowUpdate = false;
				Transactions.Cache.AllowInsert = false;

				release.SetEnabled(false);
			}
			else
			{
				PXDBCurrencyAttribute.SetBaseCalc<TaxAdjustment.curyDocBal>(cache, null, true);

				PXUIFieldAttribute.SetEnabled(cache, doc, true);
				PXUIFieldAttribute.SetEnabled<TaxAdjustment.status>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<TaxAdjustment.curyDocBal>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<TaxAdjustment.batchNbr>(cache, doc, false);

				cache.AllowDelete = true;
				cache.AllowUpdate = true;
				Transactions.Cache.AllowDelete = true;
				Transactions.Cache.AllowUpdate = true;
				Transactions.Cache.AllowInsert = true;

				release.SetEnabled(doc.Hold == false);

				ValidateDocDate(cache, doc);
			}
			PXUIFieldAttribute.SetEnabled<TaxAdjustment.docType>(cache, doc);
			PXUIFieldAttribute.SetEnabled<TaxAdjustment.refNbr>(cache, doc);
			PXUIFieldAttribute.SetEnabled<TaxAdjustment.curyID>(cache, doc, false);
			PXUIFieldAttribute.SetVisible<TaxAdjustment.curyID>(cache, doc, PXAccess.FeatureInstalled<FeaturesSet.multicurrency>());

			editVendor.SetEnabled(vendor.Current != null);
			bool tranExist = (TaxTran)this.Transactions.SelectWindowed(0, 1) != null;
			PXUIFieldAttribute.SetEnabled<TaxAdjustment.vendorID>(cache, doc, !tranExist);
			PXUIFieldAttribute.SetEnabled<TaxAdjustment.taxPeriod>(cache, doc, !tranExist);
		}

		protected virtual void TaxAdjustment_VendorID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			TaxAdjustment doc = (TaxAdjustment)e.Row;
			if (doc == null) return;

			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				if (e.ExternalCall || sender.GetValuePending<TaxAdjustment.curyID>(doc) == null)
				{
					CurrencyInfo info = CurrencyInfoAttribute.SetDefaults<TaxAdjustment.curyInfoID>(sender, doc);

					string message = PXUIFieldAttribute.GetError<CurrencyInfo.curyEffDate>(currencyinfo.Cache, info);
					if (string.IsNullOrEmpty(message) == false)
					{
						sender.RaiseExceptionHandling<TaxAdjustment.docDate>(doc, doc.DocDate, new PXSetPropertyException(message, PXErrorLevel.Warning));
					}

					if (info != null)
					{
						doc.CuryID = info.CuryID;
					}
				}
			}

			sender.SetDefaultExt<TaxAdjustment.vendorLocationID>(e.Row);
			sender.SetDefaultExt<TaxAdjustment.taxPeriod>(e.Row);
			sender.SetDefaultExt<TaxAdjustment.adjAccountID>(e.Row);
			sender.SetDefaultExt<TaxAdjustment.adjSubID>(e.Row);
		}

		protected virtual void TaxAdjustment_DocDate_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			CurrencyInfoAttribute.SetEffectiveDate<TaxAdjustment.docDate>(sender, e);
		}

		protected virtual void TaxAdjustment_BranchID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			foreach (TaxTran taxTran in Transactions.Select())
			{
				Transactions.Cache.SmartSetStatus(taxTran, PXEntryStatus.Updated);
			}
		}

		protected virtual void TaxAdjustment_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			var doc = e.Row as TaxAdjustment;

			if (doc == null)
				return;

			if (doc.Hold != true && doc.Released != true)
			{
				sender.RaiseExceptionHandling<TaxAdjustment.curyOrigDocAmt>(e.Row, ((TaxAdjustment) e.Row).CuryOrigDocAmt,
					((TaxAdjustment) e.Row).CuryDocBal != ((TaxAdjustment) e.Row).CuryOrigDocAmt
						? new PXSetPropertyException(Messages.DocumentOutOfBalance)
						: null);
			}
		}

		protected virtual void TaxAdjustment_TaxPeriod_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			var document = (TaxAdjustment)e.Row;

			SetDocDateByPeriods(cache, document);
			ValidateDocDate(cache, document);
		}

		private void SetDocDateByPeriods(PXCache cache, TaxAdjustment document)
		{
			if (document.TaxPeriod == null)
				return;

			var taxPeriod = ReportTaxProcess.FindTaxPeriodByKey(this, document.VendorID, document.TaxPeriod);

			if (taxPeriod == null)
				return;

			DateTime? docDate;

			if (vendor.Current.TaxReportFinPeriod == true)
			{
				var finPeriod = FinPeriodIDAttribute.FindMaxFinPeriodWithEndDataBelongToInterval(this, taxPeriod.StartDate,
					taxPeriod.EndDate);

				docDate = finPeriod != null
					? finPeriod.FinDate
					: Accessinfo.BusinessDate;
			}
			else
			{
				docDate = taxPeriod.EndDateUI;
			}

			cache.SetValueExt<TaxAdjustment.docDate>(document, docDate);
		}

		private void ValidateDocDate(PXCache cache, TaxAdjustment doc)
		{
			if (doc.DocDate == null || doc.TaxPeriod == null)
				return;
			
			var taxPeriod = ReportTaxProcess.FindTaxPeriodByKey(this, doc.VendorID, doc.TaxPeriod);

			if(taxPeriod == null)
				return;

			string errorMessage = null;

			if (vendor.Current.TaxReportFinPeriod == true)
			{
				var finPeriod = FinPeriodIDAttribute.GetFinPeriodByID(this, doc.FinPeriodID);

				if (finPeriod.FinDate >= taxPeriod.EndDate)
				{
					errorMessage = Messages.SelectedDateBelongsToTheTaxPeriodThatIsGreaterThanTheSpecifiedOne;
				}
			}
			else if (doc.DocDate >= taxPeriod.EndDate)
			{
				errorMessage = Messages.SelectedDateBelongsToTheTaxPeriodThatIsGreaterThanTheSpecifiedOne;
			}

			var ex = errorMessage != null
				? new PXSetPropertyException(errorMessage, PXErrorLevel.Warning)
				: null;

			cache.RaiseExceptionHandling<TaxAdjustment.docDate>(doc, doc.DocDate, ex);
		}

		protected Tax _Tax;
		protected TaxRev _TaxRev;

        protected virtual void TaxTranDefaulting(PXCache sender, TaxTran tran)
        {
            _Tax = SalesTax_Select.Select(tran.TaxID);

            sender.SetDefaultExt<TaxTran.accountID>(tran);
            sender.SetDefaultExt<TaxTran.subID>(tran);
            sender.SetDefaultExt<TaxTran.taxType>(tran);
            sender.SetDefaultExt<TaxTran.tranDate>(tran);

            _TaxRev = SalesTaxRev_Select.Select(tran.TaxID, tran.TaxType, tran.TranDate);

            sender.SetDefaultExt<TaxTran.taxBucketID>(tran);
            sender.SetDefaultExt<TaxTran.taxRate>(tran);

            _Tax = null;
            _TaxRev = null;
        }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXDBDefault(typeof(TaxAdjustment.branchID))]
		protected virtual void TaxTran_BranchID_CacheAttached(PXCache sender)
		{
		}

		protected virtual void TaxTran_TaxID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if (object.Equals(e.OldValue, ((TaxTran)e.Row).TaxID) == false)
			{
                TaxTranDefaulting(sender, (TaxTran)e.Row);
			}
		}

        protected virtual void TaxTran_TaxID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            TaxTran tran = (TaxTran) e.Row;
            if (tran == null) return;

            TaxTran copy = (TaxTran)sender.CreateCopy(tran);
            copy.TaxID = (string)e.NewValue;
            TaxTranDefaulting(sender, copy);

            if (copy.TaxBucketID == null)
            {
                throw new PXSetPropertyException(
					Messages.EffectiveTaxNotFound, 
					copy.TaxID, 
					GetLabel.For<TaxType>(copy.TaxType));
            }
        }

	    protected virtual void TaxTran_AccountID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (_Tax != null && Document.Current != null)
			{
				switch (Document.Current.DocType)
				{
					case TaxAdjustmentType.AdjustOutput:
						e.NewValue = SalesTax_Select.GetValueExt<Tax.salesTaxAcctID>(_Tax);
						break;
					case TaxAdjustmentType.AdjustInput:
						e.NewValue = SalesTax_Select.GetValueExt<Tax.purchTaxAcctID>(_Tax);
						break;
				}
			}
		}
		protected virtual void TaxTran_RevisionID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			TaxHistory max = PXSelect<TaxHistory, Where
																<TaxHistory.vendorID, Equal<Current<TaxAdjustment.vendorID>>,
																		And<TaxHistory.taxPeriodID, Equal<Current<TaxAdjustment.taxPeriod>>>>, 
																		OrderBy<Desc<TaxHistory.revisionID>>>
																		.Select(this);
			e.NewValue = max != null ? max.RevisionID : 1;

		}

		protected virtual void TaxTran_SubID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (_Tax != null && Document.Current != null)
			{
				switch (Document.Current.DocType)
				{
					case TaxAdjustmentType.AdjustOutput:
						e.NewValue = SalesTax_Select.GetValueExt<Tax.salesTaxSubID>(_Tax);
						break;
					case TaxAdjustmentType.AdjustInput:
						e.NewValue = SalesTax_Select.GetValueExt<Tax.purchTaxSubID>(_Tax);
						break;
				}
			}
		}
		protected virtual void TaxTran_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			TaxTran row = e.Row as TaxTran;
			if (row == null) return;

			row.ReportTaxAmt = row.CuryTaxAmt;
			row.ReportTaxableAmt = row.CuryTaxableAmt;
		}
		protected virtual void TaxTran_TaxType_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (Document.Current != null)
			{
				switch (Document.Current.DocType)
				{
					case TaxAdjustmentType.AdjustOutput:
						e.NewValue = TaxType.Sales;
						break;
					case TaxAdjustmentType.AdjustInput:
						e.NewValue = TaxType.Purchase;
						break;
				}
			}
		}

		protected virtual void TaxTran_TaxBucketID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
		    TaxTran tran = (TaxTran) e.Row;
			if (_TaxRev != null)
			{
				e.NewValue = _TaxRev.TaxBucketID;
				sender.SetValue<TaxTran.taxRate>(e.Row, _TaxRev.TaxRate);
			}
		}

		protected virtual void TaxTran_TaxRate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (_TaxRev != null)
			{
				e.NewValue = _TaxRev.TaxRate;
				e.Cancel = true;
			}
		}
	}
}
