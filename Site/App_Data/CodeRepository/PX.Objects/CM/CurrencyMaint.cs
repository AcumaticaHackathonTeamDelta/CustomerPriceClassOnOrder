using System;
using System.Collections;
using PX.Data;
using PX.Objects.GL;

namespace PX.Objects.CM
{
	public class CurrencyMaint : PXGraph<CurrencyMaint, CurrencyList>
	{
		[PXCopyPasteHiddenView]
		public PXSelect<CurrencyList, Where<CurrencyList.curyID, NotEqual<Current<Company.baseCuryID>>>> CuryListRecords;
		public PXSelect<Currency, Where<Currency.curyID, NotEqual<Current<Company.baseCuryID>>, And<Currency.curyID, Equal<Current<CurrencyList.curyID>>>>> CuryRecords;

		public virtual IEnumerable curyRecords()
		{
			PXResultset<Currency> res = PXSelect<Currency,Where<Currency.curyID, NotEqual<Current<Company.baseCuryID>>,And<Currency.curyID, Equal<Current<CurrencyList.curyID>>>>>.Select(this);
			
			if (res.Count == 0 && CuryListRecords.Current != null)
			{
				Currency newrow = new Currency();
				newrow.CuryID = CuryListRecords.Current.CuryID;
				res.Add(new PXResult<Currency>(newrow));
			}
			return res;
		}

		public PXSetup<Company> company;
				
		public CurrencyMaint()
		{
			Company setup = company.Current;
			if (string.IsNullOrEmpty(setup.BaseCuryID))
			{
                throw new PXSetupNotEnteredException(ErrorMessages.SetupNotEntered, typeof(Company), CS.Messages.BranchMaint);
			}
			PXUIFieldAttribute.SetVisible(CuryRecords.Cache, null, PXAccess.FeatureInstalled<CS.FeaturesSet.multicurrency>());
			PXUIFieldAttribute.SetVisible<CurrencyList.isFinancial>(CuryListRecords.Cache, null, PXAccess.FeatureInstalled<CS.FeaturesSet.multicurrency>());

		}
		
		protected virtual void Currency_DecimalPlaces_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
		{
			cache.SetValue(e.Row, typeof(Currency.decimalPlaces).Name, (short)2);
		}

        protected virtual void CurrencyList_RowUpdated(PXCache cache, PXRowUpdatedEventArgs e)
        {
            var row = e.Row as CurrencyList;
            if (row == null)
                return;

            if (CuryRecords.Cache.Current != null && row.IsFinancial != true)
            {
                CuryRecords.Cache.Delete(CuryRecords.Cache.Current);
            }
            if (row.IsFinancial == true && CuryRecords.Cache.Current != null && CuryRecords.Current.tstamp == null)
            {
                CuryRecords.Cache.Insert(CuryRecords.Cache.Current);
            }
        }

		protected void Currency_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			CurrencyList currencyListRow = CuryListRecords.Current;
			Currency currencyRow = (Currency)e.Row;
			PXUIFieldAttribute.SetEnabled(cache, currencyRow, currencyListRow != null && currencyListRow.IsFinancial == true);
		}

		protected void Currency_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
		{
			CurrencyList currencyListRow = CuryListRecords.Current;
			if (currencyListRow != null && currencyListRow.IsFinancial == false && e.Operation == PXDBOperation.Insert)
			{
				e.Cancel = true;
			}
		}

		[PXDBString(5, IsUnicode = true, IsKey = true, InputMask = ">LLLLL")]
		[PXDefault()]
		[PXUIField(DisplayName = "Currency ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof (Search<CurrencyList.curyID, Where<CurrencyList.curyID, NotEqual<Current<Company.baseCuryID>>>>))]
		[PX.Data.EP.PXFieldDescription]
		protected virtual void CurrencyList_CuryID_CacheAttached(PXCache cache)
		{
		}


	}
}
