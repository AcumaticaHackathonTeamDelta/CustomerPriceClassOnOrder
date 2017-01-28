using System;
using System.Collections;
using PX.Data;
using PX.Objects.GL;

namespace PX.Objects.FA
{
	public class FABookYearSetupMaint : YearSetupGraph<FABookYearSetupMaint, FABookYearSetup, FABookPeriodSetup, Where<FABookPeriodSetup.bookID, Equal<Current<FABookYearSetup.bookID>>>>
	{
		protected virtual IEnumerable fiscalYearSetup()
		{
			return PXSelectJoin<FABookYearSetup, LeftJoin<FABook, On<FABookYearSetup.bookID, Equal<FABook.bookID>>>, Where<FABook.updateGL, NotEqual<True>>>.Select(this).RowCast<FABookYearSetup>();
		}

		#region Overrided functions
		protected override bool IsFiscalYearSetupExists()
		{
			return false;
		}

		protected override bool IsFiscalYearExists()
		{
			PXSelectBase select = new PXSelect<FABookYear, Where<FABookYear.bookID, Equal<Current<FABookYearSetup.bookID>>>>(this);
			Object result = select.View.SelectSingle();
			return (result != null);
		}

        protected override bool CheckForPartiallyDefinedYear()
        {
            //Validates if there are FiscalYears with incomplete set of periods - not all the periods are defined;
            FABookYear unclosedYear = null;
            foreach (FABookYear fy in PXSelect<FABookYear, Where<FABookYear.bookID, Equal<Current<FABookYearSetup.bookID>>>>.Select(this))
            {
                int count = 0;
                foreach (FABookPeriod fp in PXSelect<FABookPeriod, Where<FABookPeriod.finYear, Equal<Required<FABookPeriod.finYear>>>>.Select(this, fy.Year))
                {
                    count++;                    
                }                
                if (count < fy.FinPeriods)
                {
                    unclosedYear = fy;
                    break;
                }
            }
            return (unclosedYear != null);
        }

		protected override bool AllowDeleteYearSetup(out string errMsg)
		{
			errMsg = GL.Messages.FABookPeriodsDefined;
			return !IsFiscalYearExists();
		}

		#endregion

		#region FABookYearSetup Events

		protected override bool AllowSetupModification(FABookYearSetup aRow)
		{
			bool isBookPresent = FiscalYearSetup.Current.BookID != null;
			return isBookPresent;
		}
		#endregion

        public PXAction<FABookYearSetup> DeleteGeneratedPeriods;
        [PXUIField(DisplayName = Messages.DeleteGeneratedPeriods, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public virtual IEnumerable deleteGeneratedPeriods(PXAdapter adapter)
        {
            FABookYearSetup year = FiscalYearSetup.Current;
            PXLongOperation.StartOperation(this, delegate
            {
                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    PXDatabase.Delete<FABookYear>(new PXDataFieldRestrict("BookID", PXDbType.Int, 4, year.BookID, PXComp.EQ));
                    PXDatabase.Delete<FABookPeriod>(new PXDataFieldRestrict("BookID", PXDbType.Int, 4, year.BookID, PXComp.EQ));
                    ts.Complete();
                }
            });
            return adapter.Get();
        }

	    protected override void TYearSetupOnRowSelected(PXCache cache, PXRowSelectedEventArgs e)
	    {
		    base.TYearSetupOnRowSelected(cache, e);
	        FABookYearSetup yearSetup = (FABookYearSetup) e.Row;
            FABookBalance bal = PXSelect<FABookBalance, Where<FABookBalance.bookID, Equal<Current<FABookYearSetup.bookID>>>>.SelectSingleBound(this, new object[] { yearSetup });
	        FABookYear year = null;
            if(bal == null)
            {
                year = PXSelect<FABookYear, Where<FABookYear.bookID, Equal<Current<FABookYearSetup.bookID>>>>.SelectSingleBound(this, new object[] { yearSetup });
            }
            DeleteGeneratedPeriods.SetEnabled(bal == null && year != null);
	    }
	}
}
