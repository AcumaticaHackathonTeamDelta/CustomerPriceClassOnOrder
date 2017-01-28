using System;
using System.Linq;
using PX.Data;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using PX.Objects.AP;
using PX.Objects.CR;

namespace PX.Objects.TX
{
	public class SalesTaxMaint : PXGraph<SalesTaxMaint>
	{		
		public PXSave<Tax> Save;
		public PXCancel<Tax> Cancel;
		public PXInsert<Tax> Insert;
		public PXDelete<Tax> Delete;
		public PXFirst<Tax> First;
		public PXPrevious<Tax> Previous;
		public PXNext<Tax> Next;
		public PXLast<Tax> Last;
        
		public PXSelect<Tax> Tax;
		public PXSelect<TaxRev, Where<TaxRev.taxID,Equal<Current<Tax.taxID>>>,OrderBy<Asc<TaxRev.taxType, Desc<TaxRev.startDate>>>> TaxRevisions;
		public PXSelectJoin<TaxCategoryDet, LeftJoin<TaxCategory, On<TaxCategory.taxCategoryID, Equal<TaxCategoryDet.taxCategoryID>>>, Where<TaxCategoryDet.taxID, Equal<Current<Tax.taxID>>>> Categories;
		public PXSelectJoin<TaxZoneDet, LeftJoin<TaxZone, On<TaxZone.taxZoneID, Equal<TaxZoneDet.taxZoneID>>>, Where<TaxZoneDet.taxID, Equal<Current<Tax.taxID>>>> Zones;
		public PXSelectReadonly<TaxCategory> Category;
		public PXSelectReadonly<TaxZone> Zone;

		private void PopulateBucketList(object Row)
		{
			List<int> AllowedValues = new List<int>();
			List<string> AllowedLabels = new List<string>();

            List<int> DefaultAllowedValues = new List<int>(new int[] {0});
            List<string> DefaultAllowedLabels = new List<string>(new string[] { "undefined" });

			string BucketType = null;

			switch (((Tax)Row).TaxType)
			{
				case CSTaxType.VAT:
					BucketType = "%";
					break;
				case CSTaxType.Sales:
				case CSTaxType.Use:
				case CSTaxType.Withholding:
					BucketType = CSTaxBucketType.Sales;
					break;
			}

			foreach (TaxBucket bucket in PXSelectReadonly<TaxBucket, Where<TaxBucket.vendorID, Equal<Required<TaxBucket.vendorID>>, And<TaxBucket.bucketType,Like<Required<TaxBucket.bucketType>>>>>.Select(this, ((Tax)Row).TaxVendorID, BucketType))
			{
				AllowedValues.Add((int)bucket.BucketID);
				AllowedLabels.Add(bucket.Name);
			}

			if (((Tax)Row).TaxVendorID == null)
			{
				if (BucketType == "%")
				{
					AllowedValues.Add((int)-1);
					AllowedLabels.Add(Messages.DefaultInputGroup);
				}

				AllowedValues.Add((int)-2);
				AllowedLabels.Add(Messages.DefaultOutputGroup);
			}

            if (AllowedValues.Count > 0)
            {
                PXIntListAttribute.SetList<TaxRev.taxBucketID>(TaxRevisions.Cache, null, AllowedValues.ToArray(), AllowedLabels.ToArray());
            }
            else
            {
                PXIntListAttribute.SetList<TaxRev.taxBucketID>(TaxRevisions.Cache, null, DefaultAllowedValues.ToArray(), DefaultAllowedLabels.ToArray());
            }
		}

		protected virtual void Tax_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			Tax tax = (Tax)e.Row;
			bool AlreadyWarned = false;
			bool isDeductibleSet = ((Tax)e.Row).DeductibleVAT == true && ((Tax)e.Row).DeductibleVAT != ((Tax)e.OldRow).DeductibleVAT;
			bool isDeductibleRemoved = ((Tax)e.Row).DeductibleVAT == false && ((Tax)e.Row).DeductibleVAT != ((Tax)e.OldRow).DeductibleVAT;
			
			sender.RaiseExceptionHandling<Tax.taxType>(e.Row, tax.TaxType, null);
            sender.RaiseExceptionHandling<Tax.taxCalcRule>(e.Row, tax.TaxCalcRule, null);
            sender.RaiseExceptionHandling<Tax.taxCalcLevel2Exclude>(e.Row, tax.TaxCalcLevel2Exclude, null);
            sender.RaiseExceptionHandling<Tax.reverseTax>(e.Row, tax.ReverseTax, null);
            sender.RaiseExceptionHandling<Tax.pendingTax>(e.Row, tax.PendingTax, null);
            sender.RaiseExceptionHandling<Tax.exemptTax>(e.Row, tax.ExemptTax, null);
            sender.RaiseExceptionHandling<Tax.statisticalTax>(e.Row, tax.StatisticalTax, null);
			sender.RaiseExceptionHandling<Tax.directTax>(e.Row, tax.DirectTax, null);
			sender.RaiseExceptionHandling<Tax.includeInTaxable>(e.Row, tax.IncludeInTaxable, null);

            CheckAndFixTaxRates(e);

			if (AlreadyWarned == false && tax.TaxType != CSTaxType.VAT && tax.DirectTax == true)
			{
                sender.RaiseExceptionHandling<Tax.taxType>(e.Row, tax.TaxType, new PXSetPropertyException(Messages.ThisOptionCanOnlyBeUsedWithTaxTypeVAT));
                sender.RaiseExceptionHandling<Tax.directTax>(e.Row, tax.DirectTax, new PXSetPropertyException(Messages.ThisOptionCanOnlyBeUsedWithTaxTypeVAT));
				AlreadyWarned = true;

			}

			if (AlreadyWarned == false && tax.ExemptTax == true && tax.DirectTax == true)
			{
                sender.RaiseExceptionHandling<Tax.exemptTax>(e.Row, tax.ExemptTax, new PXSetPropertyException(Messages.TheseTwoOptionsCantBeCombined));
                sender.RaiseExceptionHandling<Tax.directTax>(e.Row, tax.DirectTax, new PXSetPropertyException(Messages.TheseTwoOptionsCantBeCombined));
				AlreadyWarned = true;
			}

			if (AlreadyWarned == false && tax.StatisticalTax == true && tax.DirectTax == true)
			{
				sender.RaiseExceptionHandling<Tax.statisticalTax>(e.Row, tax.StatisticalTax, new PXSetPropertyException(Messages.TheseTwoOptionsCantBeCombined));
				sender.RaiseExceptionHandling<Tax.directTax>(e.Row, tax.DirectTax, new PXSetPropertyException(Messages.TheseTwoOptionsCantBeCombined));
				AlreadyWarned = true;
			}


			if (AlreadyWarned == false && tax.PendingTax == true && tax.DirectTax == true)
			{
				sender.RaiseExceptionHandling<Tax.pendingTax>(e.Row, tax.PendingTax, new PXSetPropertyException(Messages.TheseTwoOptionsCantBeCombined));
				sender.RaiseExceptionHandling<Tax.directTax>(e.Row, tax.DirectTax, new PXSetPropertyException(Messages.TheseTwoOptionsCantBeCombined));
				AlreadyWarned = true;
			}

            if (AlreadyWarned == false && tax.TaxType != CSTaxType.VAT && tax.PendingTax == true)
            {
                sender.RaiseExceptionHandling<Tax.taxType>(e.Row, tax.TaxType, new PXSetPropertyException(Messages.ThisOptionCanOnlyBeUsedWithTaxTypeVAT));
                sender.RaiseExceptionHandling<Tax.pendingTax>(e.Row, tax.PendingTax, new PXSetPropertyException(Messages.ThisOptionCanOnlyBeUsedWithTaxTypeVAT));
                AlreadyWarned = true;
            }

            if (AlreadyWarned == false && tax.ReverseTax == true && tax.ExemptTax == true)
            {
                sender.RaiseExceptionHandling<Tax.reverseTax>(e.Row, tax.ReverseTax, new PXSetPropertyException(Messages.TheseTwoOptionsCantBeCombined));
                sender.RaiseExceptionHandling<Tax.exemptTax>(e.Row, tax.ExemptTax, new PXSetPropertyException(Messages.TheseTwoOptionsCantBeCombined));
                AlreadyWarned = true;
            }

            if (AlreadyWarned == false && tax.ReverseTax == true && tax.StatisticalTax == true)
            {
                sender.RaiseExceptionHandling<Tax.reverseTax>(e.Row, tax.ReverseTax, new PXSetPropertyException(Messages.TheseTwoOptionsCantBeCombined));
                sender.RaiseExceptionHandling<Tax.statisticalTax>(e.Row, tax.StatisticalTax, new PXSetPropertyException(Messages.TheseTwoOptionsCantBeCombined));
                AlreadyWarned = true;
            }

            if (AlreadyWarned == false && tax.PendingTax == true && tax.StatisticalTax == true)
            {
                sender.RaiseExceptionHandling<Tax.pendingTax>(e.Row, tax.PendingTax, new PXSetPropertyException(Messages.TheseTwoOptionsCantBeCombined));
                sender.RaiseExceptionHandling<Tax.statisticalTax>(e.Row, tax.StatisticalTax, new PXSetPropertyException(Messages.TheseTwoOptionsCantBeCombined));
                AlreadyWarned = true;
            }

            if (AlreadyWarned == false && tax.PendingTax == true && tax.ExemptTax == true)
            {
                sender.RaiseExceptionHandling<Tax.pendingTax>(e.Row, tax.PendingTax, new PXSetPropertyException(Messages.TheseTwoOptionsCantBeCombined));
                sender.RaiseExceptionHandling<Tax.exemptTax>(e.Row, tax.ExemptTax, new PXSetPropertyException(Messages.TheseTwoOptionsCantBeCombined));
                AlreadyWarned = true;
            }

            if (AlreadyWarned == false && tax.StatisticalTax == true && tax.ExemptTax == true)
            {
                sender.RaiseExceptionHandling<Tax.statisticalTax>(e.Row, tax.StatisticalTax, new PXSetPropertyException(Messages.TheseTwoOptionsCantBeCombined));
                sender.RaiseExceptionHandling<Tax.exemptTax>(e.Row, tax.ExemptTax, new PXSetPropertyException(Messages.TheseTwoOptionsCantBeCombined));
                AlreadyWarned = true;
            }

            if (AlreadyWarned == false && tax.TaxCalcLevel == "0" && tax.TaxCalcLevel2Exclude == true)
            {
                sender.RaiseExceptionHandling<Tax.taxCalcRule>(e.Row, tax.TaxCalcRule, new PXSetPropertyException(Messages.TheseTwoOptionsCantBeCombined));
                sender.RaiseExceptionHandling<Tax.taxCalcLevel2Exclude>(e.Row, tax.TaxCalcLevel2Exclude, new PXSetPropertyException(Messages.TheseTwoOptionsCantBeCombined));
                AlreadyWarned = true;
            }

            if (AlreadyWarned == false && tax.TaxType == CSTaxType.Use && tax.TaxCalcLevel2Exclude == false)
            {
                sender.RaiseExceptionHandling<Tax.taxType>(e.Row, tax.TaxType, new PXSetPropertyException(Messages.TheseTwoOptionsShouldBeCombined));
                sender.RaiseExceptionHandling<Tax.taxCalcLevel2Exclude>(e.Row, tax.TaxCalcLevel2Exclude, new PXSetPropertyException(Messages.TheseTwoOptionsShouldBeCombined));
                AlreadyWarned = true;
            }

            if (AlreadyWarned == false && tax.TaxType == CSTaxType.Use && tax.TaxCalcLevel == "0")
            {
                sender.RaiseExceptionHandling<Tax.taxType>(e.Row, tax.TaxType, new PXSetPropertyException(Messages.TheseTwoOptionsCantBeCombined));
                sender.RaiseExceptionHandling<Tax.taxCalcRule>(e.Row, tax.TaxCalcRule, new PXSetPropertyException(Messages.TheseTwoOptionsCantBeCombined));
                AlreadyWarned = true;
            }

            if (AlreadyWarned == false && tax.TaxType == CSTaxType.Withholding && tax.TaxCalcLevel != "0")
            {
                sender.RaiseExceptionHandling<Tax.taxType>(e.Row, tax.TaxType, new PXSetPropertyException(Messages.TheseTwoOptionsCantBeCombined));
                sender.RaiseExceptionHandling<Tax.taxCalcRule>(e.Row, tax.TaxCalcRule, new PXSetPropertyException(Messages.TheseTwoOptionsCantBeCombined));
                AlreadyWarned = true;
            }

            if (AlreadyWarned == false && tax.ReverseTax == true && tax.TaxCalcLevel == "0")
            {
                sender.RaiseExceptionHandling<Tax.reverseTax>(e.Row, tax.ReverseTax, new PXSetPropertyException(Messages.TheseTwoOptionsCantBeCombined));
                sender.RaiseExceptionHandling<Tax.taxCalcRule>(e.Row, tax.TaxCalcRule, new PXSetPropertyException(Messages.TheseTwoOptionsCantBeCombined));
                AlreadyWarned = true;
            }

			if (AlreadyWarned == false && tax.ExemptTax == true && tax.IncludeInTaxable == true)
			{
				sender.RaiseExceptionHandling<Tax.exemptTax>(e.Row, tax.ExemptTax, new PXSetPropertyException(Messages.TheseTwoOptionsCantBeCombined));
				sender.RaiseExceptionHandling<Tax.includeInTaxable>(e.Row, tax.IncludeInTaxable, new PXSetPropertyException(Messages.TheseTwoOptionsCantBeCombined));
				AlreadyWarned = true;
			}

			if (AlreadyWarned == false && tax.StatisticalTax == true && tax.IncludeInTaxable == true)
			{
				sender.RaiseExceptionHandling<Tax.statisticalTax>(e.Row, tax.StatisticalTax, new PXSetPropertyException(Messages.TheseTwoOptionsCantBeCombined));
				sender.RaiseExceptionHandling<Tax.includeInTaxable>(e.Row, tax.IncludeInTaxable, new PXSetPropertyException(Messages.TheseTwoOptionsCantBeCombined));
				AlreadyWarned = true;
			}

			if (AlreadyWarned == false && tax.DeductibleVAT == true && tax.ReportExpenseToSingleAccount == false && tax.TaxCalcType != CSTaxCalcType.Item)
			{
				sender.RaiseExceptionHandling<Tax.taxCalcRule>(e.Row, tax.TaxCalcRule, new PXSetPropertyException(Messages.TheseTwoOptionsCantBeCombined));
				sender.RaiseExceptionHandling<Tax.reportExpenseToSingleAccount>(e.Row, tax.ReportExpenseToSingleAccount, 
					new PXSetPropertyException(Messages.TheseTwoOptionsCantBeCombined));
				AlreadyWarned = true;
			}

			if (AlreadyWarned == false)
			{
				if (((Tax)e.Row).TaxType != CSTaxType.VAT)
				{
					sender.SetValue<Tax.purchTaxAcctID>(e.Row, null);
					sender.SetValue<Tax.purchTaxSubID>(e.Row, null);
					sender.SetValue<Tax.reverseTax>(e.Row, false);
					sender.SetValue<Tax.pendingTax>(e.Row, false);
                    sender.SetValue<Tax.exemptTax>(e.Row, false);
                    sender.SetValue<Tax.statisticalTax>(e.Row, false);
					sender.SetValue<Tax.deductibleVAT>(e.Row, false);
				}

				if (((Tax)e.Row).PendingTax != true)
				{
					sender.SetValue<Tax.pendingSalesTaxAcctID>(e.Row, null);
					sender.SetValue<Tax.pendingSalesTaxSubID>(e.Row, null);
					sender.SetValue<Tax.pendingPurchTaxAcctID>(e.Row, null);
					sender.SetValue<Tax.pendingPurchTaxSubID>(e.Row, null);
				}

				if (((Tax)e.Row).TaxType != CSTaxType.Use && isDeductibleRemoved)
				{
					sender.SetValue<Tax.expenseAccountID>(e.Row, null);
					sender.SetValue<Tax.expenseSubID>(e.Row, null);
				}

				if (isDeductibleRemoved)
				{
					sender.SetValue<Tax.reportExpenseToSingleAccount>(e.Row, true);
				}

				if (isDeductibleSet)
				{
					sender.SetDefaultExt<Tax.expenseAccountID>(e.Row);
					sender.SetDefaultExt<Tax.expenseSubID>(e.Row);
				}
            }

			if (sender.ObjectsEqual<Tax.taxVendorID>(e.Row, e.OldRow) == false)
			{
				foreach (TaxRev rev in TaxRevisions.Select())
				{
					if (((Tax)e.Row).TaxVendorID == null)
					{
						if (rev.TaxType == TaxType.Sales)
						{
							rev.TaxBucketID = -2;
						}
						else
						{
							rev.TaxBucketID = -1;
						}
					}
					else
					{
						rev.TaxBucketID = null;
						TaxRevisions.Cache.RaiseExceptionHandling<TaxRev.taxBucketID>(rev, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty));
					}

					if (TaxRevisions.Cache.GetStatus(rev) == PXEntryStatus.Notchanged)
					{
						TaxRevisions.Cache.SetStatus(rev, PXEntryStatus.Updated);
					}
				}
                PopulateBucketList(e.Row);
			}
            if (!sender.ObjectsEqual<Tax.outDate>(e.Row, e.OldRow))
            {
                PXSelectBase<TaxRev> TaxRevFiltered = new PXSelect<TaxRev, Where<TaxRev.taxID, Equal<Current<Tax.taxID>>, And<TaxRev.startDate, Greater<Required<Tax.outDate>>>>>(this);
                if (tax.OutDate.HasValue && TaxRevFiltered.Select(tax.OutDate).Count > 0)
                {
                    Tax.Cache.RaiseExceptionHandling<Tax.outDate>(tax, tax.OutDate, new PXException(Messages.TaxOutDateTooEarly));
                }
                else
                {
                    sender.SetValue<Tax.outdated>(e.Row, tax.OutDate.HasValue);
                    DateTime newEndDate = tax.OutDate.HasValue ? tax.OutDate.Value : new DateTime(9999, 6, 6);
                    foreach (TaxRev rev in TaxRevisions.Select())
                    {
                        TaxRev revcopy = (TaxRev)TaxRevisions.Cache.CreateCopy(rev);
                        revcopy.EndDate = newEndDate;
                        TaxRevisions.Cache.Update(revcopy);
                    }
                }
            }

			if (!sender.ObjectsEqual<Tax.reportExpenseToSingleAccount>(e.Row, e.OldRow) && tax.ReportExpenseToSingleAccount == false)
			{
				sender.SetValue<Tax.expenseAccountID>(tax, null);
				sender.SetValue<Tax.expenseSubID>(tax, null);
			}

			if (!sender.ObjectsEqual<Tax.reportExpenseToSingleAccount>(e.Row, e.OldRow) && tax.ReportExpenseToSingleAccount == true)
			{
				sender.SetDefaultExt<Tax.expenseAccountID>(e.Row);
				sender.SetDefaultExt<Tax.expenseSubID>(e.Row);
			}
		}

       
	    private void CheckAndFixTaxRates(PXRowUpdatedEventArgs e)
        {
            Tax newTax = (Tax)e.Row;

            if (newTax.ExemptTax == true)
            {
                foreach (TaxRev iRev in this.TaxRevisions.Select())
                {
                    if ((iRev.TaxRate == null) || (iRev.TaxRate != 0))
                    {
                        iRev.TaxRate = 0;
                        TaxRevisions.Cache.Update(iRev);
                    }
                }
            }
        }

		/// <summary>
		/// In a given Tax record, sets all VAT related fields to their default values.
		/// </summary>
		protected void DefaultAllVatRelatedFields(PXCache cache, Tax taxRecord)
		{
			cache.SetDefaultExt<Tax.deductibleVAT>(taxRecord);
			cache.SetDefaultExt<Tax.reverseTax>(taxRecord);
			cache.SetDefaultExt<Tax.statisticalTax>(taxRecord);
			cache.SetDefaultExt<Tax.exemptTax>(taxRecord);
			cache.SetDefaultExt<Tax.includeInTaxable>(taxRecord);
			cache.SetDefaultExt<Tax.pendingTax>(taxRecord);
			cache.SetDefaultExt<Tax.directTax>(taxRecord);
		}

		protected virtual void Tax_TaxVendorID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<Tax.salesTaxAcctID>(e.Row);
			sender.SetDefaultExt<Tax.salesTaxSubID>(e.Row);

			if (((Tax)e.Row).TaxType == CSTaxType.VAT)
			{
				sender.SetDefaultExt<Tax.purchTaxAcctID>(e.Row);
				sender.SetDefaultExt<Tax.purchTaxSubID>(e.Row);
			}

			if (((Tax)e.Row).PendingTax == true)
			{
				sender.SetDefaultExt<Tax.pendingSalesTaxAcctID>(e.Row);
				sender.SetDefaultExt<Tax.pendingSalesTaxSubID>(e.Row);
				sender.SetDefaultExt<Tax.pendingPurchTaxAcctID>(e.Row);
				sender.SetDefaultExt<Tax.pendingPurchTaxSubID>(e.Row);
			}

			if (((Tax)e.Row).TaxType == CSTaxType.Use)
			{
				sender.SetDefaultExt<Tax.expenseAccountID>(e.Row);
				sender.SetDefaultExt<Tax.expenseSubID>(e.Row);
			}

			if (((Tax)e.Row).DeductibleVAT == true && ((Tax)e.Row).ReportExpenseToSingleAccount == true)
			{
				sender.SetDefaultExt<Tax.expenseAccountID>(e.Row);
				sender.SetDefaultExt<Tax.expenseSubID>(e.Row);
			}

			foreach (TaxRev taxrev in TaxRevisions.View.SelectMultiBound(new object[]{e.Row}))
			{
				if(TaxRevisions.Cache.GetStatus(taxrev) == PXEntryStatus.Notchanged)
					TaxRevisions.Cache.SetStatus(taxrev, PXEntryStatus.Updated);
			}
		}

		protected virtual void Tax_TaxType_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if (e.Row == null) return;
			Tax modifiedTaxRecord = e.Row as Tax;

			if (modifiedTaxRecord.TaxType != CSTaxType.VAT)
			{
				this.DefaultAllVatRelatedFields(sender, modifiedTaxRecord);
			}
		}

		protected virtual void Tax_PurchTaxAcctID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (e.Row == null) return;
			CheckAcctIDSubID<Tax.purchTaxAcctID>(sender, e.Row, e.NewValue);
		}

		protected virtual void Tax_SalesTaxAcctID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (e.Row == null) return;
			CheckAcctIDSubID<Tax.salesTaxAcctID>(sender, e.Row, e.NewValue);

		}

		protected virtual void Tax_SalesTaxSubID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (e.Row == null) return;
			CheckAcctIDSubID<Tax.salesTaxSubID>(sender, e.Row, e.NewValue);
			
		}

		protected virtual void Tax_PurchTaxSubID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (e.Row == null) return;
			CheckAcctIDSubID<Tax.purchTaxSubID>(sender, e.Row, e.NewValue);
		}

		private void CheckAcctIDSubID<Field>(PXCache sender, object row, object NewValue)
			where Field : IBqlField
		{
			if (PXAccess.FeatureInstalled<CS.FeaturesSet.taxEntryFromGL>())
			{
				Tax taxrow = (Tax)sender.CreateCopy(row as Tax);
				sender.SetValue<Field>(taxrow, NewValue);
				if (taxrow.PurchTaxAcctID == taxrow.SalesTaxAcctID && taxrow.PurchTaxSubID == taxrow.SalesTaxSubID)
				{
					sender.RaiseExceptionHandling<Tax.purchTaxAcctID>(row, null, new PXSetPropertyException(Messages.ClaimableAndPayableAccountsAreTheSame, PXErrorLevel.Warning, taxrow.TaxID));
					sender.RaiseExceptionHandling<Tax.purchTaxSubID>(row, null, new PXSetPropertyException(Messages.ClaimableAndPayableAccountsAreTheSame, PXErrorLevel.Warning, taxrow.TaxID));
					sender.RaiseExceptionHandling<Tax.salesTaxAcctID>(row, null, new PXSetPropertyException(Messages.ClaimableAndPayableAccountsAreTheSame, PXErrorLevel.Warning, taxrow.TaxID));
					sender.RaiseExceptionHandling<Tax.salesTaxSubID>(row, null, new PXSetPropertyException(Messages.ClaimableAndPayableAccountsAreTheSame, PXErrorLevel.Warning, taxrow.TaxID));
				}
			}
		}

		protected virtual void Tax_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
            if (e.Row == null) return;
						
			Tax tax = (Tax)e.Row;

			bool isUsedForManualVAT = PXSelect<TaxZone, Where<TaxZone.taxID, Equal<Required<TaxZone.taxID>>>>.Select(this, tax.TaxID).Count != 0;

			bool isOutdated = tax.OutDate.HasValue && (tax.OutDate.Value.CompareTo(Accessinfo.BusinessDate) < 0);
            if (this.Tax.Cache.GetStatus(tax) != PXEntryStatus.Updated)
            {
                this.TaxRevisions.Cache.AllowInsert = !isOutdated;
                this.TaxRevisions.Cache.AllowUpdate = !isOutdated;
                this.TaxRevisions.Cache.AllowDelete = !isOutdated;
            }

			bool isExternal = tax.IsExternal ?? false;
			if (isExternal)
			{
                this.TaxRevisions.Cache.AllowInsert = false;
				this.TaxRevisions.Cache.AllowUpdate = false;
				this.TaxRevisions.Cache.AllowDelete = false;
				this.Categories.Cache.AllowInsert = false;
				this.Categories.Cache.AllowUpdate = false;
				this.Categories.Cache.AllowDelete = false;
				this.Zones.Cache.AllowInsert = false;
				this.Zones.Cache.AllowUpdate = false;
				this.Zones.Cache.AllowDelete = false;
			}

			bool isVAT = ((Tax) e.Row).TaxType == CSTaxType.VAT;
			bool isPending = ((Tax) e.Row).PendingTax == true;
			bool isUse = ((Tax) e.Row).TaxType == CSTaxType.Use;
			bool isDeductible = ((Tax) e.Row).DeductibleVAT == true;
			bool isReportingExpenseToSingleAcc = isDeductible && ((Tax)e.Row).ReportExpenseToSingleAccount == true;

			PXUIFieldAttribute.SetEnabled<Tax.reverseTax>(cache, e.Row, isVAT && !isExternal & !isUsedForManualVAT);
			PXUIFieldAttribute.SetEnabled<Tax.pendingTax>(cache, e.Row, isVAT && !isExternal);
			PXUIFieldAttribute.SetEnabled<Tax.directTax>(cache, e.Row, isVAT && !isExternal);
			PXUIFieldAttribute.SetEnabled<Tax.statisticalTax>(cache, e.Row, isVAT && !isExternal);
			PXUIFieldAttribute.SetEnabled<Tax.exemptTax>(cache, e.Row, isVAT && !isExternal);
			PXUIFieldAttribute.SetEnabled<Tax.deductibleVAT>(cache, e.Row, isVAT && !isExternal & !isUsedForManualVAT);
			PXUIFieldAttribute.SetEnabled<Tax.includeInTaxable>(cache, e.Row, isVAT && !isExternal);
			PXUIFieldAttribute.SetEnabled<Tax.purchTaxAcctID>(cache, e.Row, isVAT);
			PXUIFieldAttribute.SetEnabled<Tax.purchTaxSubID>(cache, e.Row, isVAT);
			PXUIFieldAttribute.SetRequired<Tax.purchTaxAcctID>(cache, isVAT);
			PXUIFieldAttribute.SetRequired<Tax.purchTaxSubID>(cache, isVAT);

			PXUIFieldAttribute.SetEnabled<Tax.pendingSalesTaxAcctID>(cache, e.Row, isPending);
			PXUIFieldAttribute.SetEnabled<Tax.pendingSalesTaxSubID>(cache, e.Row, isPending);
			PXUIFieldAttribute.SetEnabled<Tax.pendingPurchTaxAcctID>(cache, e.Row, isPending);
			PXUIFieldAttribute.SetEnabled<Tax.pendingPurchTaxSubID>(cache, e.Row, isPending);

			PXUIFieldAttribute.SetVisible<Tax.reportExpenseToSingleAccount>(cache, e.Row, isDeductible);

			PXUIFieldAttribute.SetEnabled<Tax.expenseAccountID>(cache, e.Row, isUse || isReportingExpenseToSingleAcc);
			PXUIFieldAttribute.SetEnabled<Tax.expenseSubID>(cache, e.Row, isUse || isReportingExpenseToSingleAcc);
			PXDefaultAttribute.SetPersistingCheck<Tax.expenseAccountID>(cache, e.Row, isReportingExpenseToSingleAcc ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
			PXDefaultAttribute.SetPersistingCheck<Tax.expenseSubID>(cache, e.Row, isReportingExpenseToSingleAcc ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
			PXUIFieldAttribute.SetRequired<Tax.expenseAccountID>(cache, isUse || isReportingExpenseToSingleAcc);
			PXUIFieldAttribute.SetRequired<Tax.expenseSubID>(cache, isUse || isReportingExpenseToSingleAcc);

			PXUIFieldAttribute.SetEnabled<TaxRev.taxableMin>(TaxRevisions.Cache, null, ((Tax)e.Row).TaxCalcLevel == "1" || ((Tax)e.Row).TaxCalcLevel == "2");
			PXUIFieldAttribute.SetEnabled<TaxRev.taxableMax>(TaxRevisions.Cache, null, ((Tax)e.Row).TaxCalcLevel == "1" || ((Tax)e.Row).TaxCalcLevel == "2");

            PXUIFieldAttribute.SetEnabled<TaxRev.taxRate>(TaxRevisions.Cache, null, ((Tax)e.Row).ExemptTax == false);
			PXUIFieldAttribute.SetEnabled<TaxRev.nonDeductibleTaxRate>(TaxRevisions.Cache, null, isDeductible);
			PXUIFieldAttribute.SetVisible<TaxRev.nonDeductibleTaxRate>(TaxRevisions.Cache, null, isDeductible);

			PXUIFieldAttribute.SetEnabled<Tax.taxType>(cache, e.Row, !isExternal);
			PXUIFieldAttribute.SetEnabled<Tax.taxCalcType>(cache, e.Row, !isExternal);
			PXUIFieldAttribute.SetEnabled<Tax.taxCalcRule>(cache, e.Row, !isExternal);
			PXUIFieldAttribute.SetEnabled<Tax.taxCalcLevel2Exclude>(cache, e.Row, !isExternal);
			PXUIFieldAttribute.SetEnabled<Tax.taxApplyTermsDisc>(cache, e.Row, !isExternal);
			PXUIFieldAttribute.SetEnabled<Tax.outDate>(cache, e.Row, !isExternal);

			PopulateBucketList(e.Row);

			PXDefaultAttribute.SetDefault<Tax.taxApplyTermsDisc>(cache, CSTaxTermsDiscount.ToTaxableAmount);
			if (isVAT)
			{
				PXStringListAttribute.SetList<Tax.taxApplyTermsDisc>(cache, tax,
					new [] { CSTaxTermsDiscount.ToTaxableAmount, CSTaxTermsDiscount.ToPromtPayment, CSTaxTermsDiscount.NoAdjust },
					new [] { Messages.DiscountToTaxableAmount, Messages.DiscountToPromtPayment, Messages.DiscountToTotalAmount });
			}
			else
			{
				PXStringListAttribute.SetList<Tax.taxApplyTermsDisc>(cache, tax,
					new [] { CSTaxTermsDiscount.ToTaxableAmount, CSTaxTermsDiscount.NoAdjust },
					new [] { Messages.DiscountToTaxableAmount, Messages.DiscountToTotalAmount });
			}
		}

		protected void ThrowFieldIsEmpty<Field>(PXCache sender, PXRowPersistingEventArgs e)
			where Field : IBqlField
		{
			sender.RaiseExceptionHandling<Field>(e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, PXUIFieldAttribute.GetDisplayName<Field>(sender)));
		}

		protected void CheckFieldIsEmpty<Field>(PXCache sender, PXRowPersistingEventArgs e)
			where Field : IBqlField
		{
			if (sender.GetValue<Field>(e.Row) == null)
			{
				ThrowFieldIsEmpty<Field>(sender, e);
			}
		}

		protected virtual void Tax_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if ((e.Operation & PXDBOperation.Command) != PXDBOperation.Delete)
			{
				if (((Tax)e.Row).TaxType == CSTaxType.Use)
				{
					CheckFieldIsEmpty<Tax.expenseAccountID>(sender, e);
					CheckFieldIsEmpty<Tax.expenseSubID>(sender, e);
				}

				if (((Tax)e.Row).TaxType == CSTaxType.VAT)
				{
					CheckFieldIsEmpty<Tax.purchTaxAcctID>(sender, e);
					CheckFieldIsEmpty<Tax.purchTaxSubID>(sender, e);
				}

				if (((Tax)e.Row).PendingTax == true)
				{
					CheckFieldIsEmpty<Tax.pendingSalesTaxAcctID>(sender, e);
					CheckFieldIsEmpty<Tax.pendingSalesTaxSubID>(sender, e);
					CheckFieldIsEmpty<Tax.pendingPurchTaxAcctID>(sender, e);
					CheckFieldIsEmpty<Tax.pendingPurchTaxSubID>(sender, e);
				}
			}
		}

		public override void Persist()
		{
			this.PrepareRevisions();
			base.Persist();
		}

		protected virtual void TaxRev_RowInserted(PXCache cache, PXRowInsertedEventArgs e)
		{
			ForcePersisting();
		}

		private bool UpdateRevisions(PXCache sender, TaxRev item, PXResultset<TaxRev> summary, bool PerformUpdate)
		{
			foreach (TaxRev summ_item in summary)
			{
				if (!sender.ObjectsEqual(summ_item, item))
				{
					if (PerformUpdate)
					{
						summ_item.TaxBucketID = item.TaxBucketID;
						summ_item.TaxableMax = item.TaxableMax;
						summ_item.TaxableMin = item.TaxableMin;
						summ_item.TaxRate = item.TaxRate;
						summ_item.Outdated = item.Outdated;
						TaxRevisions.Cache.Update(summ_item);
					}
					return true;
				}
			}
			return false;
		}

		protected virtual void TaxRev_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			if (Tax.Current.TaxVendorID == null)
			{
				switch (((TaxRev)e.Row).TaxBucketID)
				{
					case -1:
						((TaxRev)e.Row).TaxType = CSTaxType.Use;
						break;
					case -2:
						((TaxRev)e.Row).TaxType = CSTaxType.Sales;
						break;
				}
			}
			else if (((TaxRev)e.Row).TaxBucketID != null)
			{
				TaxBucket bucket = (TaxBucket)PXSelect<TaxBucket, Where<TaxBucket.vendorID, Equal<Current<Tax.taxVendorID>>, And<TaxBucket.bucketID, Equal<Required<TaxBucket.bucketID>>>>>.Select(this,((TaxRev)e.Row).TaxBucketID);
                if (bucket != null)
                    ((TaxRev)e.Row).TaxType = bucket.BucketType;
			}

			object StartDate = ((TaxRev)e.Row).StartDate;
			object TaxType = ((TaxRev)e.Row).TaxType;
                        
            e.Cancel = UpdateRevisions(sender, (TaxRev)e.Row, PXSelect<TaxRev, Where<TaxRev.taxID, Equal<Current<Tax.taxID>>, And<TaxRev.taxType, Equal<Required<TaxRev.taxType>>, And<TaxRev.startDate, Equal<Required<TaxRev.startDate>>>>>>.Select(this, TaxType, StartDate), true);
            
		}

		protected virtual void TaxRev_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			TaxRev taxrev = e.Row as TaxRev;
			if (taxrev == null) return;

			PXUIFieldAttribute.SetEnabled<TaxRev.nonDeductibleTaxRate>(sender, taxrev, taxrev.TaxType != TaxType.Sales);
		}

		protected virtual void TaxRev_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
		{
			if (Tax.Current.TaxVendorID == null)
			{
				switch (((TaxRev)e.Row).TaxBucketID)
				{
					case -1:
						((TaxRev)e.Row).TaxType = "P";
						break;
					case -2:
						((TaxRev)e.Row).TaxType = "S";
						break;
				}
			} 
			else if (((TaxRev)e.NewRow).TaxBucketID != null)
			{
				TaxBucket bucket = (TaxBucket)PXSelect<TaxBucket, Where<TaxBucket.vendorID, Equal<Current<Tax.taxVendorID>>, And<TaxBucket.bucketID, Equal<Required<TaxBucket.bucketID>>>>>.Select(this, ((TaxRev)e.NewRow).TaxBucketID);
                if ( bucket != null )
                    ((TaxRev)e.NewRow).TaxType = bucket.BucketType;
			}

			object StartDate = ((TaxRev)e.NewRow).StartDate;
			object TaxType = ((TaxRev)e.NewRow).TaxType;
            
            e.Cancel = UpdateRevisions(sender, (TaxRev)e.NewRow, PXSelect<TaxRev, Where<TaxRev.taxID, Equal<Current<Tax.taxID>>, And<TaxRev.taxType, Equal<Required<TaxRev.taxType>>, And<TaxRev.startDate, Equal<Required<TaxRev.startDate>>>>>>.Select(this, TaxType, StartDate), false);
            
		}

		protected virtual void TaxRev_StartDate_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			/*
			object StartDate = ((TaxRev)e.Row).StartDate;
			if (StartDate != null && e.NewValue != null & ((DateTime)StartDate).CompareTo((DateTime)e.NewValue) != 0)
			{
				throw new PXSetPropertyException("Field cannot be changed");
			}
			*/
		}

		protected virtual void TaxRev_RowUpdated(PXCache cache, PXRowUpdatedEventArgs e)
		{
			if(((TaxRev)e.OldRow).StartDate != ((TaxRev)e.Row).StartDate)
			{
				ForcePersisting();
			}
		}

		protected virtual void TaxRev_RowDeleted(PXCache cache, PXRowDeletedEventArgs e)
		{
			ForcePersisting();
		}

		private void ForcePersisting() 
		{
			if (this.Tax.Cache.GetStatus(this.Tax.Current) == PXEntryStatus.Notchanged) 
			{
				this.Tax.Cache.SetStatus(this.Tax.Current, PXEntryStatus.Updated);
			}
		}

		private void PrepareRevisions() 
		{
			Tax taxrow = (Tax)Tax.Current;

			if (Tax.Current == null)
			{
				return;
			}

			if (taxrow.OutDate != null && !(bool) taxrow.Outdated)
			{
				{
					TaxRev rev = new TaxRev();
					rev.StartDate = taxrow.OutDate;
					rev.TaxType = "S";
					TaxRevisions.Insert(rev);
				}

				if (taxrow.TaxType == "V")
				{
					TaxRev rev = new TaxRev();
					rev.StartDate = taxrow.OutDate;
					rev.TaxType = "P";
					TaxRevisions.Insert(rev);
				}

				taxrow.Outdated = true;
				Tax.Update(taxrow);
			}

			DateTime? lastSalesDate = null;
			DateTime? lastPurchDate = null;
			
			//Assumes that taxRevisions are sorted in the descending order (latest first);
			foreach (TaxRev iRev in this.TaxRevisions.Select())
			{
				if (iRev.TaxType == "S" && lastSalesDate == null ||
					  iRev.TaxType == "P" && lastPurchDate == null)
				{
					if (taxrow.OutDate.HasValue)
					{
						iRev.EndDate = taxrow.OutDate.Value;
						TaxRevisions.Update(iRev);
					}
					else if (iRev.EndDate != iRev.GetDefaultEndDate())
					{
						TaxRevisions.Cache.SetDefaultExt<TaxRev.endDate>(iRev);
						TaxRevisions.Update(iRev);
					}
				}

				if (((Tax)Tax.Current).OutDate != null)
				{
					if (((DateTime)((Tax)Tax.Current).OutDate).CompareTo(iRev.StartDate) <= 0)
					{
						iRev.Outdated = true;
						TaxRevisions.Update(iRev);
					}
				}

				if (iRev.TaxType == "S")
				{
					if (lastSalesDate != null && iRev.EndDate != lastSalesDate)
					{
						iRev.EndDate = lastSalesDate;
						TaxRevisions.Update(iRev);
					}
					lastSalesDate = ((DateTime)iRev.StartDate).AddDays(-1);
				}
				else if (iRev.TaxType == "P")
				{
					if (lastPurchDate != null && iRev.EndDate != lastPurchDate)
					{
						iRev.EndDate = lastPurchDate;
						TaxRevisions.Update(iRev);
					}
					lastPurchDate = ((DateTime)iRev.StartDate).AddDays(-1);
				}
			}
		}

		public SalesTaxMaint()
		{
			APSetup setup = APSetup.Current;
			PXUIFieldAttribute.SetVisible<TaxCategoryDet.taxID>(Categories.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<TaxCategoryDet.taxID>(Categories.Cache, null, false);
			PXUIFieldAttribute.SetVisible<TaxCategoryDet.taxCategoryID>(Categories.Cache, null, true);
			PXUIFieldAttribute.SetEnabled<TaxCategoryDet.taxCategoryID>(Categories.Cache, null, true);
			//SWUIFieldAttribute.SetVisible<TaxCategory.taxCategoryID>(Category.Cache, null, false);

			PXUIFieldAttribute.SetVisible<TaxZoneDet.taxID>(Zones.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<TaxZoneDet.taxID>(Zones.Cache, null, false);
			PXUIFieldAttribute.SetVisible<TaxZoneDet.taxZoneID>(Zones.Cache, null, true);
			PXUIFieldAttribute.SetEnabled<TaxZoneDet.taxZoneID>(Zones.Cache, null, true);
			//SWUIFieldAttribute.SetVisible<TaxZone.taxZoneID>(Zone.Cache, null, false);
			FieldDefaulting.AddHandler<BAccountR.type>((sender, e) => { if (e.Row != null) e.NewValue = BAccountType.VendorType; });
			if(!PXAccess.FeatureInstalled<CS.FeaturesSet.vATReporting>())
				PXStringListAttribute.SetList<TX.Tax.taxType>(Tax.Cache, null, new CSTaxType.ListSimpleAttribute());
		}

		public PXSetup<APSetup> APSetup;

		protected virtual void TaxZoneDet_TaxID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = (Tax.Cache.Current == null) ? null : ((Tax)Tax.Cache.Current).TaxID;
		}

		protected virtual void TaxCategoryDet_TaxID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = (Tax.Cache.Current == null) ? null : ((Tax)Tax.Cache.Current).TaxID;
		}
	}
}
