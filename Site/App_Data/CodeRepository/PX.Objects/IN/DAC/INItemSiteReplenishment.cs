using System;
using PX.Data;

namespace PX.Objects.IN
{
    [Serializable]
	[PXCacheName(Messages.INItemSiteReplenishment)]
	public partial class INItemSiteReplenishment : IBqlTable
	{
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[StockItem(IsKey = true, DirtyRead = true, DisplayName = "Inventory ID")]
		[PXParent(typeof(Select<InventoryItem, Where<InventoryItem.inventoryID, Equal<Current<INItemSiteReplenishment.inventoryID>>>>))]
		[PXDefault(typeof(INItemSite.inventoryID))]
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
		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SiteID;
		[IN.Site(IsKey = true)]
		[PXDefault(typeof(INItemSite.siteID))]
		[PXParent(typeof(Select<INSite, Where<INSite.siteID, Equal<Current<INItemSiteReplenishment.siteID>>>>))]
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
		#region SubItemID
		public abstract class subItemID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubItemID;
		[SubItem(typeof(INItemSiteReplenishment.inventoryID), DisplayName = "Subitem", IsKey = true)]		
		[PXDefault]
		public virtual Int32? SubItemID
		{
			get
			{
				return this._SubItemID;
			}
			set
			{
				this._SubItemID = value;
			}
		}
		#endregion
		#region SafetyStock
		public abstract class safetyStock : PX.Data.IBqlField
		{
		}
		protected Decimal? _SafetyStock;
		[PXDBQuantity]
		[PXUIField(DisplayName = "Safety Stock")]
		[PXDefault(TypeCode.Decimal, "0.0",
			typeof(Select<INItemSite,
			Where<INItemSite.inventoryID, Equal<Current<INItemSiteReplenishment.inventoryID>>,
				And<INItemSite.siteID, Equal<Current<INItemSiteReplenishment.siteID>>>>>),
			SourceField = typeof(INItemSite.safetyStock))]
		public virtual Decimal? SafetyStock
		{
			get
			{
				return this._SafetyStock;
			}
			set
			{
				this._SafetyStock = value;
			}
		}
		#endregion
		#region MinQty
		public abstract class minQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _MinQty;
		[PXDBQuantity]
		[PXUIField(DisplayName = "Reorder Point")]
		[PXDefault(TypeCode.Decimal, "0.0",
		typeof(Select<INItemSite,
		Where<INItemSite.inventoryID, Equal<Current<INItemSiteReplenishment.inventoryID>>,
			And<INItemSite.siteID, Equal<Current<INItemSiteReplenishment.siteID>>>>>),
		SourceField = typeof(INItemSite.minQty))]
		public virtual Decimal? MinQty
		{
			get
			{
				return this._MinQty;
			}
			set
			{
				this._MinQty = value;
			}
		}
		#endregion
		#region MaxQty
		public abstract class maxQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _MaxQty;
		[PXDBQuantity]
		[PXUIField(DisplayName = "Max Qty.")]
		[PXDefault(TypeCode.Decimal, "0.0",
		typeof(Select<INItemSite,
		Where<INItemSite.inventoryID, Equal<Current<INItemSiteReplenishment.inventoryID>>,
			And<INItemSite.siteID, Equal<Current<INItemSiteReplenishment.siteID>>>>>),
		SourceField = typeof(INItemSite.maxQty))]
		public virtual Decimal? MaxQty
		{
			get
			{
				return this._MaxQty;
			}
			set
			{
				this._MaxQty = value;
			}
		}
		#endregion
		#region TransferERQ
		public abstract class transferERQ : PX.Data.IBqlField
		{
		}
		protected Decimal? _TransferERQ;
		[PXDBQuantity]
		[PXDefault(TypeCode.Decimal, "0.0",
			typeof(Select<INItemSite,
				Where<INItemSite.inventoryID, Equal<Current<INItemSiteReplenishment.inventoryID>>,
					And<INItemSite.siteID, Equal<Current<INItemSiteReplenishment.siteID>>>>>),
				SourceField = typeof(INItemSite.transferERQ),PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Transfer ERQ")]
		public virtual Decimal? TransferERQ
		{
			get
			{
				return this._TransferERQ;
			}
			set
			{
				this._TransferERQ = value;
			}
		}
		#endregion
		#region ItemStatus
		public abstract class itemStatus : PX.Data.IBqlField
		{
		}
		protected String _ItemStatus;
		[PXDBString(2, IsFixed = true)]
		[PXDefault(InventoryItemStatus.Active)]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible)]
		[InventoryItemStatus.SubItemList]
		public virtual String ItemStatus
		{
			get
			{
				return this._ItemStatus;
			}
			set
			{
				this._ItemStatus = value;
			}
		}
		#endregion

		#region SafetyStockSuggested
		public abstract class safetyStockSuggested : PX.Data.IBqlField
		{
		}
		protected Decimal? _SafetyStockSuggested;
		[PXDBQuantity]
		[PXUIField(DisplayName = "Safety Stock Suggested", Enabled = false)]
		public virtual Decimal? SafetyStockSuggested
		{
			get
			{
				return this._SafetyStockSuggested;
			}
			set
			{
				this._SafetyStockSuggested = value;
			}
		}
		#endregion
		#region MinQtySuggested
		public abstract class minQtySuggested: PX.Data.IBqlField
		{
		}
		protected Decimal? _MinQtySuggested;
		[PXDBQuantity]
		[PXUIField(DisplayName = "Reorder Point Suggested", Enabled = false)]
		public virtual Decimal? MinQtySuggested
		{
			get
			{
				return this._MinQtySuggested;
			}
			set
			{
				this._MinQtySuggested = value;
			}
		}
		#endregion
		#region MaxQtySuggested
		public abstract class maxQtySuggested : PX.Data.IBqlField
		{
		}
		protected Decimal? _MaxQtySuggested;
		[PXDBQuantity]
		[PXUIField(DisplayName = "Max Qty Suggested", Enabled = false)]
		public virtual Decimal? MaxQtySuggested
		{
			get
			{
				return this._MaxQtySuggested;
			}
			set
			{
				this._MaxQtySuggested = value;
			}
		}
		#endregion

		#region DemandPerDayAverage
		public abstract class demandPerDayAverage : PX.Data.IBqlField
		{
		}
		protected Decimal? _DemandPerDayAverage;
		[PXDBQuantity]
		[PXUIField(DisplayName = "Daily Demand Forecast", Enabled = false)]
		//[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? DemandPerDayAverage
		{
			get
			{
				return this._DemandPerDayAverage;
			}
			set
			{
				this._DemandPerDayAverage = value;
			}
		}
		#endregion
		#region DemandPerDayMSE
		public abstract class demandPerDayMSE : PX.Data.IBqlField
		{
		}
		protected Decimal? _DemandPerDayMSE;
		[PXDBQuantity]
		[PXUIField(DisplayName = "Daily Demand Forecast Error(MSE)", Enabled = false)]
		//[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? DemandPerDayMSE
		{
			get
			{
				return this._DemandPerDayMSE;
			}
			set
			{
				this._DemandPerDayMSE = value;
			}
		}
		#endregion
		#region DemandPerDayMAD
		public abstract class demandPerDayMAD : PX.Data.IBqlField
		{
		}
		protected Decimal? _DemandPerDayMAD;
		[PXDBQuantity]
		[PXUIField(DisplayName = "Daily Forecast Error MAD", Enabled = false)]
		//[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? DemandPerDayMAD
		{
			get
			{
				return this._DemandPerDayMAD;
			}
			set
			{
				this._DemandPerDayMAD = value;
			}
		}
		#endregion
		#region DemandPerDaySTDEV
		public abstract class demandPerDaySTDEV : PX.Data.IBqlField
		{
		}
		[PXQuantity]
		[PXUIField(DisplayName = "Daily Demand Forecast Error(STDEV)", Enabled = false)]
		public virtual Decimal? DemandPerDaySTDEV
		{
			[PXDependsOnFields(typeof(demandPerDayMSE))]
			get
			{
				return this._DemandPerDayMSE.HasValue ? (Decimal)Math.Sqrt((double)this._DemandPerDayMSE.Value) : this._DemandPerDayMSE;
			}
			set
			{

			}
		}
		#endregion

		#region CreatedByID
		public abstract class createdByID : PX.Data.IBqlField
		{
		}
		protected Guid? _CreatedByID;
		[PXDBCreatedByID()]
		public virtual Guid? CreatedByID
		{
			get
			{
				return this._CreatedByID;
			}
			set
			{
				this._CreatedByID = value;
			}
		}
		#endregion
		#region CreatedByScreenID
		public abstract class createdByScreenID : PX.Data.IBqlField
		{
		}
		protected String _CreatedByScreenID;
		[PXDBCreatedByScreenID()]
		public virtual String CreatedByScreenID
		{
			get
			{
				return this._CreatedByScreenID;
			}
			set
			{
				this._CreatedByScreenID = value;
			}
		}
		#endregion
		#region CreatedDateTime
		public abstract class createdDateTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _CreatedDateTime;
		[PXDBCreatedDateTime()]
		public virtual DateTime? CreatedDateTime
		{
			get
			{
				return this._CreatedDateTime;
			}
			set
			{
				this._CreatedDateTime = value;
			}
		}
		#endregion
		#region LastModifiedByID
		public abstract class lastModifiedByID : PX.Data.IBqlField
		{
		}
		protected Guid? _LastModifiedByID;
		[PXDBLastModifiedByID()]
		public virtual Guid? LastModifiedByID
		{
			get
			{
				return this._LastModifiedByID;
			}
			set
			{
				this._LastModifiedByID = value;
			}
		}
		#endregion
		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : PX.Data.IBqlField
		{
		}
		protected String _LastModifiedByScreenID;
		[PXDBLastModifiedByScreenID()]
		public virtual String LastModifiedByScreenID
		{
			get
			{
				return this._LastModifiedByScreenID;
			}
			set
			{
				this._LastModifiedByScreenID = value;
			}
		}
		#endregion
		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _LastModifiedDateTime;
		[PXDBLastModifiedDateTime()]
		public virtual DateTime? LastModifiedDateTime
		{
			get
			{
				return this._LastModifiedDateTime;
			}
			set
			{
				this._LastModifiedDateTime = value;
			}
		}
		#endregion
		#region tstamp
		public abstract class Tstamp : PX.Data.IBqlField
		{
		}
		protected Byte[] _tstamp;
		[PXDBTimestamp()]
		public virtual Byte[] tstamp
		{
			get
			{
				return this._tstamp;
			}
			set
			{
				this._tstamp = value;
			}
		}
		#endregion
	}
}
