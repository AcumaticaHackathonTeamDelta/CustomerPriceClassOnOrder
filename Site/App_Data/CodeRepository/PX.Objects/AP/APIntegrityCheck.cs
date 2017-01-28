using System;
using System.Collections;
using PX.Data;
using PX.Objects.GL;

namespace PX.Objects.AP
{
    [Serializable]
	public partial class APIntegrityCheckFilter : PX.Data.IBqlTable
	{
		#region VendorClassID
		public abstract class vendorClassID : PX.Data.IBqlField
		{
		}
		protected String _VendorClassID;
		[PXDBString(10, IsUnicode = true)]
		[PXSelector(typeof(VendorClass.vendorClassID), DescriptionField = typeof(VendorClass.descr), CacheGlobal = true)]
		[PXUIField(DisplayName = "Vendor Class")]
		public virtual String VendorClassID
		{
			get
			{
				return this._VendorClassID;
			}
			set
			{
				this._VendorClassID = value;
			}
		}
		#endregion
		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected String _FinPeriodID;
		[FinPeriodSelector()]
		[PXDefault(typeof(Search<FinPeriod.finPeriodID, Where<FinPeriod.aPClosed, Equal<False>>, OrderBy<Asc<FinPeriod.finPeriodID>>>))]
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
	public class APIntegrityCheck : PXGraph<APIntegrityCheck>
	{
		public PXFilter<APIntegrityCheckFilter> Filter;
		public PXCancel<APIntegrityCheckFilter> Cancel;
		[PXFilterable]
		public PXFilteredProcessing<Vendor, APIntegrityCheckFilter,
			Where<Match<Current<AccessInfo.userName>>>> APVendorList;
		public PXSelect<Vendor,
			Where<Vendor.vendorClassID, Equal<Current<APIntegrityCheckFilter.vendorClassID>>,
			And<Match<Current<AccessInfo.userName>>>>>
			APVendorList_ByVendorClassID;

		public APIntegrityCheck()
		{
			APSetup setup = APSetup.Current;
		}

		protected virtual void APIntegrityCheckFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			APIntegrityCheckFilter filter = Filter.Current;

			APVendorList.SetProcessDelegate<APReleaseProcess>(
				delegate(APReleaseProcess re, Vendor vend)
				{
					re.Clear(PXClearOption.PreserveTimeStamp);
					re.IntegrityCheckProc(vend, filter.FinPeriodID);
				}
			);
		}

		public virtual IEnumerable apvendorlist()
		{
			if (Filter.Current != null && Filter.Current.VendorClassID != null)
			{
				return APVendorList_ByVendorClassID.Select();
			}
			return null;
		}

		public PXSetup<APSetup> APSetup;

		public PXAction<APIntegrityCheckFilter> viewVendor;
		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable ViewVendor(PXAdapter adapter)
		{
			VendorMaint graph = CreateInstance<VendorMaint>();
			graph.BAccount.Current = PXSelect<VendorR, Where<VendorR.bAccountID, Equal<Current<Vendor.bAccountID>>>>.Select(this);
			throw new PXRedirectRequiredException(graph, true, "View Vendor") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
		}

	}
}
