using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.RQ;

namespace PX.Objects.IN
{

	public class INSiteSummaryEnq : PXGraph<INSiteSummaryEnq>
	{
		#region Filter
		[Serializable]
		public partial class Filter : PX.Data.IBqlTable
		{

			#region SiteID
			public abstract class siteID : PX.Data.IBqlField
			{
			}
			protected Int32? _SiteID;
			[Site(DescriptionField = typeof(INSite.descr), DisplayName = "Warehouse")]
			public virtual Int32? SiteID
			{
				get
				{
					return this._SiteID;
				}
				set
				{
					this._SiteID = value;
				}
			}
			#endregion

			#region LocationID
			public abstract class locationID : PX.Data.IBqlField
			{
			}
			protected Int32? _LocationID;
			[Location(typeof(Filter.siteID), Visibility = PXUIVisibility.Visible, KeepEntry = false, DescriptionField = typeof(INLocation.descr), DisplayName = "Location")]
			public virtual Int32? LocationID
			{
				get
				{
					return this._LocationID;
				}
				set
				{
					this._LocationID = value;
				}
			}
			#endregion

			#region InventoryID
			public abstract class inventoryID : PX.Data.IBqlField
			{
			}
			protected Int32? _InventoryID;
			[Inventory(typeof(Search<InventoryItem.inventoryID, Where<InventoryItem.stkItem, NotEqual<boolFalse>, And<Where<Match<Current<AccessInfo.userName>>>>>>), typeof(InventoryItem.inventoryCD), typeof(InventoryItem.descr))]
			public virtual Int32? InventoryID
			{
				get
				{
					return this._InventoryID;
				}
				set
				{
					this._InventoryID = value;
				}
			}
			#endregion

			#region SubItemCD
			public abstract class subItemCD : PX.Data.IBqlField
			{
			}
			protected String _SubItemCD;
			[SubItemRawExt(typeof(Filter.inventoryID), DisplayName = "Subitem")]
			//[SubItemRaw(DisplayName = "Subitem")]
			public virtual String SubItemCD
			{
				get
				{
					return this._SubItemCD;
				}
				set
				{
					this._SubItemCD = value;
				}
			}
			#endregion

			#region SubItemCD Wildcard
			public abstract class subItemCDWildcard : PX.Data.IBqlField { };
			[PXDBString(30, IsUnicode = true)]
			public virtual String SubItemCDWildcard
			{
				get
				{
					//return SubItemCDUtils.CreateSubItemCDWildcard(this._SubItemCD);
					return SubCDUtils.CreateSubCDWildcard(this._SubItemCD, SubItemAttribute.DimensionName);
				}
			}
			#endregion

			#region OnlyAvailable
			public abstract class onlyAvailable : PX.Data.IBqlField
			{
			}
			protected bool? _OnlyAvailable;
			[PXBool]
			[PXDefault(true)]
			[PXUIField(DisplayName = "Show Available Items Only")]
			public virtual bool? OnlyAvailable
			{
				get
				{
					return _OnlyAvailable;
				}
				set
				{
					_OnlyAvailable = value;
				}
			}
			#endregion
		}
		#endregion

		public PXCancel<Filter> Cancel;

		public PXFilter<Filter> CurrentFilter;
		public PXSelectJoin<INSiteStatus,
			InnerJoin<InventoryItem, 
			       On<InventoryItem.inventoryID, Equal<INSiteStatus.inventoryID>>>,
			Where<CurrentValue<Filter.onlyAvailable>, Equal<CS.boolFalse>,
				 Or<INSiteStatus.qtyOnHand, Greater<CS.decimal0>>>> Records;

		public INSiteSummaryEnq()
		{
			Records.View.WhereAndCurrent<Filter>();
		}
	}
}
