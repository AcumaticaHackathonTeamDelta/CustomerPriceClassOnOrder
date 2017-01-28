using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PX.Data;
using PX.Objects.GL;

namespace PX.Objects.AR
{
    [Serializable]
	public partial class ARIntegrityCheckFilter : PX.Data.IBqlTable
	{
		#region CustomerClassID
		public abstract class customerClassID : PX.Data.IBqlField
		{
		}
		protected String _CustomerClassID;
		[PXDBString(10, IsUnicode = true)]
		[PXSelector(typeof(CustomerClass.customerClassID), DescriptionField = typeof(CustomerClass.descr), CacheGlobal = true)]
		[PXUIField(DisplayName = "Customer Class")]
		public virtual String CustomerClassID
		{
			get
			{
				return this._CustomerClassID;
			}
			set
			{
				this._CustomerClassID = value;
			}
		}
		#endregion
		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected String _FinPeriodID;
		[FinPeriodSelector()]
		[PXDefault(typeof(Search<FinPeriod.finPeriodID, Where<FinPeriod.aRClosed, Equal<False>>, OrderBy<Asc<FinPeriod.finPeriodID>>>))]
		[PXUIField(DisplayName = "Fin. Period")]
		public virtual String FinPeriodID
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

	[TableAndChartDashboardType]
	public class ARIntegrityCheck : PXGraph<ARIntegrityCheck>
	{
		public PXCancel<ARIntegrityCheckFilter> Cancel;
		public PXFilter<ARIntegrityCheckFilter> Filter;
		[PXFilterable]
        [PX.SM.PXViewDetailsButton(typeof(Customer.acctCD), WindowMode = PXRedirectHelper.WindowMode.NewWindow)]
		public PXFilteredProcessing<Customer, ARIntegrityCheckFilter,
			Where<Match<Current<AccessInfo.userName>>>> ARCustomerList;
		public PXSelect<Customer, 
			Where<Customer.customerClassID, Equal<Current<ARIntegrityCheckFilter.customerClassID>>,
			And<Match<Current<AccessInfo.userName>>>>> Customer_ClassID;

		protected virtual IEnumerable arcustomerlist()
		{
			if (Filter.Current != null && Filter.Current.CustomerClassID != null)
			{
				return Customer_ClassID.Select();
			}
			return null;
		}

		public ARIntegrityCheck()
		{
			ARSetup setup = ARSetup.Current;
		}

		protected virtual void ARIntegrityCheckFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			ARIntegrityCheckFilter filter = Filter.Current;
            ARCustomerList.SetProcessDelegate<ARReleaseProcess>(
				delegate(ARReleaseProcess re, Customer cust)
				{
					re.Clear(PXClearOption.PreserveTimeStamp);
					re.IntegrityCheckProc(cust, filter.FinPeriodID);
				}
			); 
		}

		public PXSetup<ARSetup> ARSetup;
	}
}
