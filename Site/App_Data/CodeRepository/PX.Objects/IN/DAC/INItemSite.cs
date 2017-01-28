namespace PX.Objects.IN
{
	using System;
	using PX.Data;
	using PX.Objects.GL;
	using PX.Objects.AP;
	using PX.Objects.EP;
	using PX.Objects.CM;
	using PX.Objects.CS;
	using PX.TM;
	using PX.Objects.CR;
	public class DemandForecastModelType
	{
		public const string None = "NNN";
		public const string MovingAverage = "CMA";
		public const string ExponentialSmoothing = "ESC";
		public const string ExponentialSmoothingTrend = "EST";
		public const string ExponentialSmoothingSeasons = "ESS";
		public const string ExponentialSmoothingTrendAndSeasons = "ETS";
		public const string LinearRegression = "LRT";

		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { None, MovingAverage},
				new string[] { Messages.DFM_None, Messages.DFM_MovingAverage} 				
			) { }
		}

		public class none : Constant<String> 
		{
			public none() : base(None) { }
		}
	}

	public class DemandPeriodType
	{
		public const string Month = "MT";
		public const string Week = "WK";		
		public const string Quarter= "QT";
		public const string Day = "DY";
		

		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Quarter, Month, Week, Day, },
				new string[] { Messages.Quarter, Messages.Month, Messages.Week, Messages.Day }) { }
		}
	}

	[System.SerializableAttribute()]
	[PXPrimaryGraph(typeof(INItemSiteMaint))]
	[PXCacheName(Messages.ItemWarehouseSettings)]
    [PXProjection(typeof(Select2<INItemSite, InnerJoin<INSite, On<INSite.siteID, Equal<INItemSite.siteID>>, LeftJoin<INItemStats, On<INItemStats.inventoryID, Equal<INItemSite.inventoryID>, And<INItemStats.siteID, Equal<INItemSite.siteID>>>>>>), new Type[] { typeof(INItemSite) })]
	public partial class INItemSite : PX.Data.IBqlTable
	{
		#region Selected
		public abstract class selected : PX.Data.IBqlField
		{
		}
		protected Boolean? _Selected = false;
		[PXBool()]
		[PXUIField(DisplayName = "Selected")]
		public virtual Boolean? Selected
		{
			get
			{
				return this._Selected;
			}
			set
			{
				this._Selected = value;
			}
		}
		#endregion
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[StockItem(IsKey = true, DirtyRead = true, DisplayName="Inventory ID", TabOrder = 1)]		
		[PXParent(typeof(Select<InventoryItem, Where<InventoryItem.inventoryID, Equal<Current<INItemSite.inventoryID>>>>))]
		[PXDefault()]
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
		[IN.Site(IsKey = true, TabOrder = 2)]
		[PXDefault()]
		[PXParent(typeof(Select<INSite, Where<INSite.siteID, Equal<Current<INItemSite.siteID>>>>))]
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
		#region SiteStatus
		public abstract class siteStatus : PX.Data.IBqlField
		{
		}
		protected String _SiteStatus;
		[PXDBString(2, IsFixed = true)]
		[PXDefault("AC")]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible)]
		[PXStringList(new string[] { "AC", "IN" }, new string[] { "Active", "Inactive" })]
		public virtual String SiteStatus
		{
			get
			{
				return this._SiteStatus;
			}
			set
			{
				this._SiteStatus = value;
			}
		}
		#endregion
		#region OverrideInvtAcctSub
		public abstract class overrideInvtAcctSub : IBqlField
		{
		}
		protected bool? _OverrideInvtAcctSub;
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = Messages.OverrideInventoryAcctSub)]
		public virtual bool? OverrideInvtAcctSub
		{
			get
			{
				return _OverrideInvtAcctSub;
			}
			set
			{
				_OverrideInvtAcctSub = value;
			}
		}
		#endregion
		#region InvtAcctID
		public abstract class invtAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _InvtAcctID;
		[Account(DisplayName = "Inventory Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		[PXUIRequired(typeof(Where<INItemSite.overrideInvtAcctSub, Equal<True>>))]
		[PXUIEnabled(typeof(Where<INItemSite.overrideInvtAcctSub, Equal<True>>))]
		[PXDefault()]
		public virtual Int32? InvtAcctID
		{
			get
			{
				return this._InvtAcctID;
			}
			set
			{
				this._InvtAcctID = value;
			}
		}
		#endregion
		#region InvtSubID
		public abstract class invtSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _InvtSubID;
		[SubAccount(typeof(INItemSite.invtAcctID), DisplayName = "Inventory Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		[PXUIRequired(typeof(Where<INItemSite.overrideInvtAcctSub, Equal<True>>))]
		[PXUIEnabled(typeof(Where<INItemSite.overrideInvtAcctSub, Equal<True>>))]
		[PXDefault()]
		public virtual Int32? InvtSubID
		{
			get
			{
				return this._InvtSubID;
			}
			set
			{
				this._InvtSubID = value;
			}
		}
		#endregion
		#region ValMethod
		public abstract class valMethod : PX.Data.IBqlField
		{
		}
		protected String _ValMethod;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(typeof(Search<InventoryItem.valMethod, Where<InventoryItem.inventoryID, Equal<Current<INItemSite.inventoryID>>>>))]
		public virtual String ValMethod
		{
			get
			{
				return this._ValMethod;
			}
			set
			{
				this._ValMethod = value;
			}
		}
		#endregion
		#region DfltShipLocationID
		public abstract class dfltShipLocationID : PX.Data.IBqlField
		{
		}
		protected Int32? _DfltShipLocationID;
		[IN.Location(typeof(INItemSite.siteID), DisplayName = "Default Issue From", DescriptionField = typeof(INLocation.descr))]
		public virtual Int32? DfltShipLocationID
		{
			get
			{
				return this._DfltShipLocationID;
			}
			set
			{
				this._DfltShipLocationID = value;
			}
		}
		#endregion
		#region DfltReceiptLocationID
		public abstract class dfltReceiptLocationID : PX.Data.IBqlField
		{
		}
		protected Int32? _DfltReceiptLocationID;
		[IN.Location(typeof(INItemSite.siteID), DisplayName = "Default Receipt To", DescriptionField = typeof(INLocation.descr))]
		public virtual Int32? DfltReceiptLocationID
		{
			get
			{
				return this._DfltReceiptLocationID;
			}
			set
			{
				this._DfltReceiptLocationID = value;
			}
		}
        #endregion
        #region DfltUnitOverride
        public abstract class dfltUnitOverride : IBqlField
        {
        }
        [PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Override Unit of Measure")]
        public virtual bool? DfltUnitOverride { get; set; }
        #endregion
        #region DfltSalesUnit
        public abstract class dfltSalesUnit : PX.Data.IBqlField
		{
		}
		protected String _DfltSalesUnit;
		[INUnit(null, typeof(InventoryItem.baseUnit), DisplayName = "Sales Unit")]
		[PXDefault(typeof(Search<InventoryItem.salesUnit, Where<InventoryItem.inventoryID, Equal<Current<INItemSite.inventoryID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual String DfltSalesUnit
		{
			get
			{
				return this._DfltSalesUnit;
			}
			set
			{
				this._DfltSalesUnit = value;
			}
		}
		#endregion
		#region DfltPurchaseUnit
		public abstract class dfltPurchaseUnit : PX.Data.IBqlField
		{
		}
		protected String _DfltPurchaseUnit;
		[INUnit(null, typeof(InventoryItem.baseUnit), DisplayName = "Purchase Unit")]
		[PXDefault(typeof(Search<InventoryItem.purchaseUnit, Where<InventoryItem.inventoryID, Equal<Current<INItemSite.inventoryID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual String DfltPurchaseUnit
		{
			get
			{
				return this._DfltPurchaseUnit;
			}
			set
			{
				this._DfltPurchaseUnit = value;
			}
		}
		#endregion
		#region LastPurchaseDate
		public abstract class lastPurchaseDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _LastPurchaseDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Last Purchase Date", Enabled = false)]
		public virtual DateTime? LastPurchaseDate
		{
			get
			{
				return this._LastPurchaseDate;
			}
			set
			{
				this._LastPurchaseDate = value;
			}
		}
		#endregion
		#region LastPurchasePrice
		public abstract class lastPurchasePrice : PX.Data.IBqlField
		{
		}
		protected Decimal? _LastPurchasePrice;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Last Purchase Price", Enabled = false)]
		public virtual Decimal? LastPurchasePrice
		{
			get
			{
				return this._LastPurchasePrice;
			}
			set
			{
				this._LastPurchasePrice = value;
			}
		}
		#endregion
		#region LastVendorID
		public abstract class lastVendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _LastVendorID;
		[Vendor(DescriptionField = typeof(Vendor.acctName), Enabled = false)]
		public virtual Int32? LastVendorID
		{
			get
			{
				return this._LastVendorID;
			}
			set
			{
				this._LastVendorID = value;
			}
		}
		#endregion
		#region LastStdCost
		public abstract class lastStdCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _LastStdCost;
		[PXDBPriceCost()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Last Cost", Enabled = false)]
		public virtual Decimal? LastStdCost
		{
			get
			{
				return this._LastStdCost;
			}
			set
			{
				this._LastStdCost = value;
			}
		}
		#endregion
		#region PendingStdCost
		public abstract class pendingStdCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _PendingStdCost;
		[PXDBPriceCost()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Pending Cost")]
		public virtual Decimal? PendingStdCost
		{
			get
			{
				return this._PendingStdCost;
			}
			set
			{
				this._PendingStdCost = value;
			}
		}
		#endregion
		#region PendingStdCostDate
		public abstract class pendingStdCostDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _PendingStdCostDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Pending Cost Date")]
		public virtual DateTime? PendingStdCostDate
		{
			get
			{
				return this._PendingStdCostDate;
			}
			set
			{
				this._PendingStdCostDate = value;
			}
		}
		#endregion
		#region StdCost
		public abstract class stdCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _StdCost;
		[PXDBPriceCost()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Current Cost", Enabled = false)]        
		public virtual Decimal? StdCost
		{
			get
			{
				return this._StdCost;
			}
			set
			{
				this._StdCost = value;
			}
		}
		#endregion
		#region StdCostDate
		public abstract class stdCostDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _StdCostDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Effective Date", Enabled = false)]
        [PXFormula(typeof(Switch<Case<Where<INItemSite.lastStdCost, NotEqual<CS.decimal0>>, Current<AccessInfo.businessDate>>, INItemSite.stdCostDate>))]
		public virtual DateTime? StdCostDate
		{
			get
			{
				return this._StdCostDate;
			}
			set
			{
				this._StdCostDate = value;
			}
		}
		#endregion
		#region LastBasePrice
		public abstract class lastBasePrice : PX.Data.IBqlField
		{
		}
		protected Decimal? _LastBasePrice;
		[PXDBPriceCost()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Last Price", Enabled = false)]
		public virtual Decimal? LastBasePrice
		{
			get
			{
				return this._LastBasePrice;
			}
			set
			{
				this._LastBasePrice = value;
			}
		}
		#endregion
		#region PendingBasePrice
		public abstract class pendingBasePrice : PX.Data.IBqlField
		{
		}
		protected Decimal? _PendingBasePrice;
		[PXDBPriceCost()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Pending Price")]
		public virtual Decimal? PendingBasePrice
		{
			get
			{
				return this._PendingBasePrice;
			}
			set
			{
				this._PendingBasePrice = value;
			}
		}
		#endregion
		#region PendingBasePriceDate
		public abstract class pendingBasePriceDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _PendingBasePriceDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Pending Price Date", Enabled = false)]
		public virtual DateTime? PendingBasePriceDate
		{
			get
			{
				return this._PendingBasePriceDate;
			}
			set
			{
				this._PendingBasePriceDate = value;
			}
		}
		#endregion
		#region BasePrice
		public abstract class basePrice : PX.Data.IBqlField
		{
		}
		protected Decimal? _BasePrice;
		[PXDBPriceCost()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Current Price", Enabled = false)]
		public virtual Decimal? BasePrice
		{
			get
			{
				return this._BasePrice;
			}
			set
			{
				this._BasePrice = value;
			}
		}
		#endregion
		#region BasePriceDate
		public abstract class basePriceDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _BasePriceDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Effective Date", Enabled = false)]
		public virtual DateTime? BasePriceDate
		{
			get
			{
				return this._BasePriceDate;
			}
			set
			{
				this._BasePriceDate = value;
			}
		}
		#endregion
		#region LastCostDate
		public abstract class lastCostDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _LastCostDate;
        [PXDate]
        public virtual DateTime? LastCostDate
		{
			get
			{
				return this._LastCostDate;
			}
			set
			{
				this._LastCostDate = value;
			}
		}
		#endregion
		#region LastCost
		public abstract class lastCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _LastCost;
		[PXDBPriceCost(BqlField = typeof(INItemStats.lastCost))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Last Cost", Enabled = true)]
		public virtual Decimal? LastCost
		{
			get
			{
				return this._LastCost;
			}
			set
			{
				this._LastCost = value;
			}
		}
		#endregion
		#region AvgCost
		public abstract class avgCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _AvgCost;
		[PXPriceCost()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Average Cost", Enabled = false)]
		[PXDBPriceCostCalced(typeof(Switch<Case<Where<INItemStats.qtyOnHand, Equal<decimal0>>, decimal0>, Div<INItemStats.totalCost, INItemStats.qtyOnHand>>), typeof(Decimal))]
		public virtual Decimal? AvgCost
		{
			get
			{
				return this._AvgCost;
			}
			set
			{
				this._AvgCost = value;
			}
		}
		#endregion
		#region MinCost
		public abstract class minCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _MinCost;
		[PXDBPriceCost(BqlField = typeof(INItemStats.minCost))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Min. Cost", Enabled = false)]
		public virtual Decimal? MinCost
		{
			get
			{
				return this._MinCost;
			}
			set
			{
				this._MinCost = value;
			}
		}
		#endregion
		#region MaxCost
		public abstract class maxCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _MaxCost;
        [PXDBPriceCost(BqlField = typeof(INItemStats.maxCost))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Max. Cost", Enabled = false)]
		public virtual Decimal? MaxCost
		{
			get
			{
				return this._MaxCost;
			}
			set
			{
				this._MaxCost = value;
			}
		}
		#endregion
		#region TranUnitCost
		public abstract class tranUnitCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranUnitCost;
		[PXDBCalced(typeof(Switch<Case<Where<INItemSite.valMethod, Equal<INValMethod.standard>>, INItemSite.stdCost,
								Case<Where<INItemSite.valMethod, Equal<INValMethod.average>, And<INSite.avgDefaultCost, Equal<INSite.avgDefaultCost.averageCost>,
										Or<INItemSite.valMethod, Equal<INValMethod.fIFO>, And<INSite.fIFODefaultCost, Equal<INSite.avgDefaultCost.averageCost>>>>>,
                                        Switch<Case<Where<INItemStats.qtyOnHand, Equal<decimal0>>, decimal0,
                                            Case<Where<Div<INItemStats.totalCost, INItemStats.qtyOnHand>,Less<decimal0>>,INItemStats.lastCost>>, 
                                            Div<INItemStats.totalCost, INItemStats.qtyOnHand>>>>,
								INItemStats.lastCost>), typeof(Decimal))]
		public virtual Decimal? TranUnitCost
		{
			get
			{
				return this._TranUnitCost;
			}
			set
			{
				this._TranUnitCost = value;
			}
		}
		#endregion
		#region NegativeCost
		public abstract class negativeCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _NegativeCost;
		[PXDBCalced(typeof(Switch<Case<Where<INItemSite.valMethod, Equal<INValMethod.standard>>, INItemSite.stdCost, Case<Where<INItemSite.valMethod, Equal<INValMethod.average>, And<INItemStats.qtyOnHand, NotEqual<decimal0>, And<Div<INItemStats.totalCost, INItemStats.qtyOnHand>, GreaterEqual<decimal0>>>>, IsNull<NullIf<Switch<Case<Where<INItemStats.qtyOnHand, Equal<decimal0>>, decimal0>, Div<INItemStats.totalCost, INItemStats.qtyOnHand>>, decimal0>, INItemStats.lastCost>>>, INItemStats.lastCost>), typeof(Decimal))]
		public virtual Decimal? NegativeCost
		{
			get
			{
				return this._NegativeCost;
			}
			set
			{
				this._NegativeCost = value;
			}
		}
		#endregion
		#region PreferredVendorOverride
		public abstract class preferredVendorOverride : PX.Data.IBqlField
		{
		}
		protected Boolean? _PreferredVendorOverride;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Override Preferred Vendor")]
		public virtual Boolean? PreferredVendorOverride
		{
			get
			{
				return this._PreferredVendorOverride;
			}
			set
			{
				this._PreferredVendorOverride = value;
			}
		}
		#endregion
		#region PreferredVendorID
		public abstract class preferredVendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _PreferredVendorID;
		[AP.VendorActive(DisplayName = "Preferred Vendor", Required = false, DescriptionField = typeof(Vendor.acctName))]
		public virtual Int32? PreferredVendorID
		{
			get
			{
				return this._PreferredVendorID;
			}
			set
			{
				this._PreferredVendorID = value;
			}
		}
		#endregion
		#region PreferredVendorLocationID
		public abstract class preferredVendorLocationID : PX.Data.IBqlField
		{
		}
		protected Int32? _PreferredVendorLocationID;
		[LocationID(typeof(Where<Location.bAccountID, Equal<Current<INItemSite.preferredVendorID>>>),
			DescriptionField = typeof(Location.descr), Visibility = PXUIVisibility.SelectorVisible, DisplayName = "Preferred Location")]
		[PXDefault(typeof(Search<Vendor.defLocationID, Where<Vendor.bAccountID, Equal<Current<INItemSite.preferredVendorID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXFormula(typeof(Default<INItemSite.preferredVendorID>))]
		public virtual Int32? PreferredVendorLocationID
		{
			get
			{
				return this._PreferredVendorLocationID;
			}
			set
			{
				this._PreferredVendorLocationID = value;
			}
		}
		#endregion

		#region ProductWorkgroupID
		public abstract class productWorkgroupID : PX.Data.IBqlField
		{
		}
		protected Int32? _ProductWorkgroupID;
		[PXDBInt()]
		[PXCompanyTreeSelector]
		[PXUIField(DisplayName = "Product Workgroup")]
		public virtual Int32? ProductWorkgroupID
		{
			get
			{
				return this._ProductWorkgroupID;
			}
			set
			{
				this._ProductWorkgroupID = value;
			}
		}
		#endregion
		#region ProductManagerID
		public abstract class productManagerID : PX.Data.IBqlField
		{
		}
		protected Guid? _ProductManagerID;
		[PXDBGuid()]
		[PXOwnerSelector(typeof(INItemSite.productWorkgroupID))]
		[PXUIField(DisplayName = "Product Manager")]
		public virtual Guid? ProductManagerID
		{
			get
			{
				return this._ProductManagerID;
			}
			set
			{
				this._ProductManagerID = value;
			}
		}
		#endregion
		#region PriceWorkgroupID
		public abstract class priceWorkgroupID : PX.Data.IBqlField
		{
		}
		protected Int32? _PriceWorkgroupID;
		[PXDBInt()]
		[PXCompanyTreeSelector]
		[PXUIField(DisplayName = "Price Workgroup")]
		public virtual Int32? PriceWorkgroupID
		{
			get
			{
				return this._PriceWorkgroupID;
			}
			set
			{
				this._PriceWorkgroupID = value;
			}
		}
		#endregion
		#region PriceManagerID
		public abstract class priceManagerID : PX.Data.IBqlField
		{
		}
		protected Guid? _PriceManagerID;
		[PXDBGuid()]
		[PXOwnerSelector(typeof(INItemSite.priceWorkgroupID))]
		[PXUIField(DisplayName = "Price Manager")]
		public virtual Guid? PriceManagerID
		{
			get
			{
				return this._PriceManagerID;
			}
			set
			{
				this._PriceManagerID = value;
			}
		}
		#endregion
		#region IsDefault
		public abstract class isDefault : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsDefault;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Default")]
		public virtual Boolean? IsDefault
		{
			get
			{
				return this._IsDefault;
			}
			set
			{
				this._IsDefault = value;
			}
		}
		#endregion
		#region StdCostOverride
		public abstract class stdCostOverride : PX.Data.IBqlField
		{
		}
		protected Boolean? _StdCostOverride;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Override Std. Cost")]
		public virtual Boolean? StdCostOverride
		{
			get
			{
				return this._StdCostOverride;
			}
			set
			{
				this._StdCostOverride = value;
			}
		}
		#endregion
		#region BasePriceOverride
		public abstract class basePriceOverride : PX.Data.IBqlField
		{
		}
		protected Boolean? _BasePriceOverride;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Price Override")]
		public virtual Boolean? BasePriceOverride
		{
			get
			{
				return this._BasePriceOverride;
			}
			set
			{
				this._BasePriceOverride = value;
			}
		}
		#endregion
		#region Commissionable
		public abstract class commissionable : PX.Data.IBqlField
		{
		}
		protected Boolean? _Commissionable;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Subject to Commission", Visibility = PXUIVisibility.Visible)]
		public virtual Boolean? Commissionable
		{
			get
			{
				return this._Commissionable;
			}
			set
			{
				this._Commissionable = value;
			}
		}
		#endregion
		#region ABCCodeOverride
		public abstract class aBCCodeOverride : PX.Data.IBqlField
		{
		}
		protected Boolean? _ABCCodeOverride;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "ABC Code Override")]
		public virtual Boolean? ABCCodeOverride
		{
			get
			{
				return this._ABCCodeOverride;
			}
			set
			{
				this._ABCCodeOverride = value;
			}
		}
		#endregion
		#region ABCCodeID
		public abstract class aBCCodeID : PX.Data.IBqlField
		{
		}
		protected String _ABCCodeID;
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "ABC Code", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(INABCCode.aBCCodeID), DescriptionField = typeof(INABCCode.descr))]
		public virtual String ABCCodeID
		{
			get
			{
				return this._ABCCodeID;
			}
			set
			{
				this._ABCCodeID = value;
			}
		}
		#endregion
		#region ABCCodeIsFixed
		public abstract class aBCCodeIsFixed : PX.Data.IBqlField
		{
		}
		protected Boolean? _ABCCodeIsFixed;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Fixed ABC Code")]
		public virtual Boolean? ABCCodeIsFixed
		{
			get
			{
				return this._ABCCodeIsFixed;
			}
			set
			{
				this._ABCCodeIsFixed = value;
			}
		}
		#endregion
		#region MovementClassOverride
		public abstract class movementClassOverride : PX.Data.IBqlField
		{
		}
		protected Boolean? _MovementClassOverride;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Movement Class Override")]
		public virtual Boolean? MovementClassOverride
		{
			get
			{
				return this._MovementClassOverride;
			}
			set
			{
				this._MovementClassOverride = value;
			}
		}
		#endregion
		#region MovementClassID
		public abstract class movementClassID : PX.Data.IBqlField
		{
		}
		protected String _MovementClassID;
		[PXDBString(1)]
		[PXUIField(DisplayName = "Movement Class", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(INMovementClass.movementClassID), DescriptionField = typeof(INMovementClass.descr))]
		public virtual String MovementClassID
		{
			get
			{
				return this._MovementClassID;
			}
			set
			{
				this._MovementClassID = value;
			}
		}
		#endregion
		#region MovementClassIsFixed
		public abstract class movementClassIsFixed : PX.Data.IBqlField
		{
		}
		protected Boolean? _MovementClassIsFixed;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Fixed Movement Class")]
		public virtual Boolean? MovementClassIsFixed
		{
			get
			{
				return this._MovementClassIsFixed;
			}
			set
			{
				this._MovementClassIsFixed = value;
			}
		}
		#endregion
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXNote(DescriptionField = typeof(INItemSite.siteID),
			Selector = typeof(INItemSite.siteID))] //TODO: need calculate description from inventoryID and siteID
		public virtual Guid? NoteID
		{
			get
			{
				return this._NoteID;
			}
			set
			{
				this._NoteID = value;
			}
		}
		#endregion
		
		#region POCreate
		public abstract class pOCreate : PX.Data.IBqlField
		{
		}
		protected Boolean? _POCreate;		
		[PXDBCalced(
			typeof(Switch<Case<Where<INItemSite.replenishmentSource, Equal<INReplenishmentSource.purchaseToOrder>,
														Or<INItemSite.replenishmentSource, Equal<INReplenishmentSource.dropShipToOrder>>>, boolTrue>, boolFalse>), typeof(bool))]
		[PXUIField(DisplayName = "Mark fo PO",  Enabled = false)]
		public virtual Boolean? POCreate
		{
			get
			{
				return this._POCreate;
			}
			set
			{
				this._POCreate = value ?? false;
			}
		}
		#endregion
		#region POSource
		public abstract class pOSource : PX.Data.IBqlField
		{
		}
		protected string _POSource;
		[PXDBCalced(
			typeof(Switch<Case<Where<INItemSite.replenishmentSource, Equal<INReplenishmentSource.dropShipToOrder>>, INReplenishmentSource.dropShipToOrder,
										Case<Where<INItemSite.replenishmentSource, Equal<INReplenishmentSource.purchaseToOrder>>, INReplenishmentSource.purchaseToOrder>>,
										INReplenishmentSource.none>), typeof(string))]
		public virtual string POSource
		{
			get
			{
				return this._POSource;
			}
			set
			{
				this._POSource = value ?? INReplenishmentSource.None;
			}
		}
		#endregion

		#region MarkupPct
		public abstract class markupPct : PX.Data.IBqlField
		{
		}
		protected Decimal? _MarkupPct;
		[PXDBPriceCost()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Markup %", Enabled = false)]
		public virtual Decimal? MarkupPct
		{
			get
			{
				return this._MarkupPct;
			}
			set
			{
				this._MarkupPct = value;
			}
		}
		#endregion
		#region MarkupPctOverride
		public abstract class markupPctOverride : PX.Data.IBqlField
		{
		}
		protected Boolean? _MarkupPctOverride;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Override Markup %")]
		public virtual Boolean? MarkupPctOverride
		{
			get
			{
				return this._MarkupPctOverride;
			}
			set
			{
				this._MarkupPctOverride = value;
			}
		}
		#endregion

		#region RecPrice
		public abstract class recPrice : PX.Data.IBqlField
		{
		}
		protected Decimal? _RecPrice;
		[PXDBPriceCost()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "MSRP", Enabled = false)]
		public virtual Decimal? RecPrice
		{
			get
			{
				return this._RecPrice;
			}
			set
			{
				this._RecPrice = value;
			}
		}
		#endregion
		#region RecPriceOverride
		public abstract class recPriceOverride : PX.Data.IBqlField
		{
		}
		protected Boolean? _RecPriceOverride;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Override Price")]
		public virtual Boolean? RecPriceOverride
		{
			get
			{
				return this._RecPriceOverride;
			}
			set
			{
				this._RecPriceOverride = value;
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
		
		#region ReplenishmentClassID
		public abstract class replenishmentClassID : PX.Data.IBqlField
		{
		}
		protected String _ReplenishmentClassID;
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXUIField(DisplayName = "Replenishment Class", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<INReplenishmentClass.replenishmentClassID>), DescriptionField = typeof(INReplenishmentClass.descr))]		
		public virtual String ReplenishmentClassID
		{
			get
			{
				return this._ReplenishmentClassID;
			}
			set
			{
				this._ReplenishmentClassID = value;
			}
		}
		#endregion
		#region ReplenishmentPolicyOverride
		public abstract class replenishmentPolicyOverride : PX.Data.IBqlField
		{
		}
		protected Boolean? _ReplenishmentPolicyOverride;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Override Replenishment Settings")]
		public virtual Boolean? ReplenishmentPolicyOverride
		{
			get
			{
				return this._ReplenishmentPolicyOverride;
			}
			set
			{
				this._ReplenishmentPolicyOverride = value;
			}
		}
		#endregion
		#region ReplenishmentPolicyID
		public abstract class replenishmentPolicyID : PX.Data.IBqlField
		{
		}
		protected String _ReplenishmentPolicyID;		
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXUIField(DisplayName = "Seasonality")]
		[PXSelector(typeof(Search<INReplenishmentPolicy.replenishmentPolicyID>), DescriptionField = typeof(INReplenishmentPolicy.descr))]
		public virtual String ReplenishmentPolicyID
		{
			get
			{
				return this._ReplenishmentPolicyID;
			}
			set
			{
				this._ReplenishmentPolicyID = value;
			}
		}
		#endregion		
		#region ReplenishmentSource
		public abstract class replenishmentSource : PX.Data.IBqlField
		{
		}
		protected string _ReplenishmentSource;
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Replenishment Source")]
		[INReplenishmentSource.List]
		public virtual string ReplenishmentSource
		{
			get
			{
				return this._ReplenishmentSource;
			}
			set
			{
				this._ReplenishmentSource = value;
			}
		}
		#endregion
		#region ReplenishmentMethod
		public abstract class replenishmentMethod : PX.Data.IBqlField
		{
		}
		protected String _ReplenishmentMethod;
		[PXDBString(1, IsFixed = true)]		
		[PXUIField(DisplayName = "Replenishment Method", Enabled = false)]
		[INReplenishmentMethod.List]		
		public virtual String ReplenishmentMethod
		{
			get
			{
				return this._ReplenishmentMethod;
			}
			set
			{
				this._ReplenishmentMethod = value;
			}
		}
		#endregion
		#region ReplenishmentSourceSiteID
		public abstract class replenishmentSourceSiteID : PX.Data.IBqlField
		{
		}
		protected Int32? _ReplenishmentSourceSiteID;
		[IN.Site(DisplayName = "Replenishment Warehouse", DescriptionField = typeof(INSite.descr))]		
		public virtual Int32? ReplenishmentSourceSiteID
		{
			get
			{
				return this._ReplenishmentSourceSiteID;
			}
			set
			{
				this._ReplenishmentSourceSiteID = value;
			}
		}
		#endregion
		#region MaxShelfLifeOverride
		public abstract class maxShelfLifeOverride : PX.Data.IBqlField
		{
		}
		protected Boolean? _MaxShelfLifeOverride;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Override")]
		public virtual Boolean? MaxShelfLifeOverride
		{
			get
			{
				return this._MaxShelfLifeOverride;
			}
			set
			{
				this._MaxShelfLifeOverride = value;
			}
		}
		#endregion
		#region MaxShelfLife
		public abstract class maxShelfLife : PX.Data.IBqlField
		{
		}
		protected Int32? _MaxShelfLife;
		[PXDBInt()]
		[PXUIField(DisplayName = "Max. Shelf Life (Days)")]		
		public virtual Int32? MaxShelfLife
		{
			get
			{
				return this._MaxShelfLife;
			}
			set
			{
				this._MaxShelfLife = value;
			}
		}
		#endregion
		#region LaunchDateOverride
		public abstract class launchDateOverride : PX.Data.IBqlField
		{
		}
		protected Boolean? _LaunchDateOverride;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Override")]
		public virtual Boolean? LaunchDateOverride
		{
			get
			{
				return this._LaunchDateOverride;
			}
			set
			{
				this._LaunchDateOverride = value;
			}
		}
		#endregion
		#region LaunchDate
		public abstract class launchDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _LaunchDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Launch Date")]
		public virtual DateTime? LaunchDate
		{
			get
			{
				return this._LaunchDate;
			}
			set
			{
				this._LaunchDate = value;
			}
		}
		#endregion
		#region TerminationDateOverride
		public abstract class terminationDateOverride : PX.Data.IBqlField
		{
		}
		protected Boolean? _TerminationDateOverride;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Override")]
		public virtual Boolean? TerminationDateOverride
		{
			get
			{
				return this._TerminationDateOverride;
			}
			set
			{
				this._TerminationDateOverride = value;
			}
		}
		#endregion
		#region TerminationDate
		public abstract class terminationDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _TerminationDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Termination Date")]
		public virtual DateTime? TerminationDate
		{
			get
			{
				return this._TerminationDate;
			}
			set
			{
				this._TerminationDate = value;
			}
		}
		#endregion
		#region ServiceLevelOverride
		public abstract class serviceLevelOverride : PX.Data.IBqlField
		{
		}
		protected Boolean? _ServiceLevelOverride;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Override")]
		public virtual Boolean? ServiceLevelOverride
		{
			get
			{
				return this._ServiceLevelOverride;
			}
			set
			{
				this._ServiceLevelOverride = value;
			}
		}
		#endregion
		#region ServiceLevel
		public abstract class serviceLevel : PX.Data.IBqlField
		{
		}
		protected decimal? _ServiceLevel;
		[PXDBDecimal(6, MinValue=0.500001, MaxValue=0.999999)]
		[PXUIField(DisplayName = "Service Level", Visible =true)]		
		public virtual decimal? ServiceLevel
		{
			get
			{
				return this._ServiceLevel;
			}
			set
			{
				this._ServiceLevel = value;
			}
		}
		#endregion
		#region ServiceLevelPct
		public abstract class serviceLevelPct : PX.Data.IBqlField
		{
		}
		
		[PXDecimal(4, MinValue = 50.0001, MaxValue = 99.9999)]
		[PXUIField(DisplayName = "Service Level (%)", Visible = true)]
		public virtual decimal? ServiceLevelPct
		{
			[PXDependsOnFields(typeof(serviceLevel))]
			get
			{
				return this._ServiceLevel * 100.0m;
			}
			set
			{
				this._ServiceLevel = value/100.0m;
			}
		}
		#endregion
		#region SafetyStockOverride
		public abstract class safetyStockOverride : PX.Data.IBqlField
		{
		}
		protected Boolean? _SafetyStockOverride;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Override")]
		public virtual Boolean? SafetyStockOverride
		{
			get
			{
				return this._SafetyStockOverride;
			}
			set
			{
				this._SafetyStockOverride = value;
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
		#region MinQtyOverride
		public abstract class minQtyOverride : PX.Data.IBqlField
		{
		}
		protected Boolean? _MinQtyOverride;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Override")]
		public virtual Boolean? MinQtyOverride
		{
			get
			{
				return this._MinQtyOverride;
			}
			set
			{
				this._MinQtyOverride = value;
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
		#region MaxQtyOverride
		public abstract class maxQtyOverride : PX.Data.IBqlField
		{
		}
		protected Boolean? _MaxQtyOverride;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Override")]
		public virtual Boolean? MaxQtyOverride
		{
			get
			{
				return this._MaxQtyOverride;
			}
			set
			{
				this._MaxQtyOverride = value;
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
		#region TransferERQOverride
		public abstract class transferERQOverride : PX.Data.IBqlField
		{
		}
		protected Boolean? _TransferERQOverride;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Override")]
		public virtual Boolean? TransferERQOverride
		{
			get
			{
				return this._TransferERQOverride;
			}
			set
			{
				this._TransferERQOverride = value;
			}
		}
		#endregion
		#region TransferERQ
		public abstract class transferERQ : PX.Data.IBqlField
		{
		}
		protected Decimal? _TransferERQ;
		[PXDBQuantity]		
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

		#region SubItemOverride
		public abstract class subItemOverride : PX.Data.IBqlField
		{
		}
		protected Boolean? _SubItemOverride;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Override", FieldClass = SubItemAttribute.DimensionName)]
		public virtual Boolean? SubItemOverride
		{
			get
			{
				return this._SubItemOverride;
			}
			set
			{
				this._SubItemOverride = value;
			}
		}
		#endregion		

		#region SafetyStockSuggested
		public abstract class safetyStockSuggested : PX.Data.IBqlField
		{
		}
		protected Decimal? _SafetyStockSuggested;
		[PXDBQuantity]
		[PXUIField(DisplayName = "Safety Stock Suggested", Enabled=false)]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
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
		public abstract class minQtySuggested : PX.Data.IBqlField
		{
		}
		protected Decimal? _MinQtySuggested;
		[PXDBQuantity]
		[PXUIField(DisplayName = "Reorder Point Suggested", Enabled = false)]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
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
		[PXUIField(DisplayName = "Max Qty Suggested")]
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

		#region ESSmoothingConstantL
		public abstract class eSSmoothingConstantL : PX.Data.IBqlField
		{
		}
		protected Decimal? _ESSmoothingConstantL;
		[PXDBDecimal(9, MinValue = 0.0, MaxValue = 1.0)]
		[PXUIField(DisplayName = "Level Smoothing Constant")]
		//[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck =PXPersistingCheck.Nothing)]
		public virtual Decimal? ESSmoothingConstantL
		{
			get
			{
				return this._ESSmoothingConstantL;
			}
			set
			{
				this._ESSmoothingConstantL = value;
			}
		}
		#endregion
		#region ESSmoothingConstantLOverride
		public abstract class eSSmoothingConstantLOverride : PX.Data.IBqlField
		{
		}
		protected Boolean? _ESSmoothingConstantLOverride;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Override")]
		public virtual Boolean? ESSmoothingConstantLOverride
		{
			get
			{
				return this._ESSmoothingConstantLOverride;
			}
			set
			{
				this._ESSmoothingConstantLOverride = value;
			}
		}
		#endregion
		#region ESSmoothingConstantT
		public abstract class eSSmoothingConstantT : PX.Data.IBqlField
		{
		}
		protected Decimal? _ESSmoothingConstantT;
		[PXDBDecimal(9)]
		[PXUIField(DisplayName = "Trend Smoothing Constant")]
		
		public virtual Decimal? ESSmoothingConstantT
		{
			get
			{
				return this._ESSmoothingConstantT;
			}
			set
			{
				this._ESSmoothingConstantT = value;
			}
		}
		#endregion
		#region ESSmoothingConstantTOverride
		public abstract class eSSmoothingConstantTOverride : PX.Data.IBqlField
		{
		}
		protected Boolean? _ESSmoothingConstantTOverride;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Override")]
		public virtual Boolean? ESSmoothingConstantTOverride
		{
			get
			{
				return this._ESSmoothingConstantTOverride;
			}
			set
			{
				this._ESSmoothingConstantTOverride = value;
			}
		}
		#endregion
		#region ESSmoothingConstantS
		public abstract class eSSmoothingConstantS : PX.Data.IBqlField
		{
		}
		protected Decimal? _ESSmoothingConstantS;
		[PXDBDecimal(9)]
		[PXUIField(DisplayName = "Seasonality Smoothing Constant")]		
		public virtual Decimal? ESSmoothingConstantS
		{
			get
			{
				return this._ESSmoothingConstantS;
			}
			set
			{
				this._ESSmoothingConstantS = value;
			}
		}
		#endregion
		#region ESSmoothingConstantSOverride
		public abstract class eSSmoothingConstantSOverride : PX.Data.IBqlField
		{
		}
		protected Boolean? _ESSmoothingConstantSOverride;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Override")]
		public virtual Boolean? ESSmoothingConstantSOverride
		{
			get
			{
				return this._ESSmoothingConstantSOverride;
			}
			set
			{
				this._ESSmoothingConstantSOverride = value;
			}
		}
		#endregion
		#region AutoFitModel
		public abstract class autoFitModel : PX.Data.IBqlField
		{
		}
		protected Boolean? _AutoFitModel;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Auto Fit Model", Visibility = PXUIVisibility.Invisible)]
		public virtual Boolean? AutoFitModel
		{
			get
			{
				return this._AutoFitModel;
			}
			set
			{
				this._AutoFitModel = value;
			}
		}
		#endregion

		#region DemandPerDayAverage
		public abstract class demandPerDayAverage : PX.Data.IBqlField
		{
		}
		protected Decimal? _DemandPerDayAverage;
		[PXDBQuantity]
		[PXUIField(DisplayName = "Daily Demand Forecast", Enabled=false)]
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
		[PXUIField(DisplayName = "Daily Demand Forecast Error(MSE)", Enabled=false)]
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
		[PXUIField(DisplayName = "Daily Forecast Error(MAD)",Enabled = false)]
	//	[PXDefault(TypeCode.Decimal, "0.0")]
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
		[PXUIField(DisplayName = "Daily Demand Forecast Error(STDEV)",Enabled = false)]		
		public virtual Decimal? DemandPerDaySTDEV
		{
			[PXDependsOnFields(typeof(demandPerDayMSE))]
			get
			{
				return this._DemandPerDayMSE.HasValue? (Decimal)Math.Sqrt((double)this._DemandPerDayMSE.Value): this._DemandPerDayMSE;				
			}
			set
			{
				
			}
		}
		#endregion

		#region LeadTimeAverage
		public abstract class leadTimeAverage : PX.Data.IBqlField
		{
		}
		protected Decimal? _LeadTimeAverage;
		[PXDBQuantity]
		[PXUIField(DisplayName = "Lead Time Average")]
		//[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Decimal? LeadTimeAverage
		{
			get
			{
				return this._LeadTimeAverage;
			}
			set
			{
				this._LeadTimeAverage = value;
			}
		}
		#endregion
		#region LeadTimeMSE
		public abstract class leadTimeMSE : PX.Data.IBqlField
		{
		}
		protected Decimal? _LeadTimeMSE;
		[PXDBQuantity]
		[PXUIField(DisplayName = "Lead Time Deviation")]
		//[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Decimal? LeadTimeMSE
		{
			get
			{
				return this._LeadTimeMSE;
			}
			set
			{
				this._LeadTimeMSE = value;
			}
		}
		#endregion
		#region LeadTimeSTDEV
		public abstract class leadTimeSTDEV : PX.Data.IBqlField
		{
		}
		
		[PXQuantity]
		[PXUIField(DisplayName = "Lead Time STDEV")]	
		public virtual Decimal? LeadTimeSTDEV
		{
			[PXDependsOnFields(typeof(leadTimeMSE))]
			get
			{

				return this._LeadTimeMSE.HasValue ? (Decimal)Math.Sqrt((double)this._LeadTimeMSE.Value) : this._LeadTimeMSE;
			}
			set
			{
		
			}
		}
		#endregion

		#region LastForecastDate
		public abstract class lastForecastDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _LastForecastDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Last Forecast Date", Enabled = false)]
		public virtual DateTime? LastForecastDate
		{
			get
			{
				return this._LastForecastDate;
			}
			set
			{
				this._LastForecastDate = value;
			}
		}
		#endregion

		#region ForecastModelType
		public abstract class forecastModelType : PX.Data.IBqlField
		{
		}
		protected String _ForecastModelType;
		[PXDBString(3, IsFixed = true)]
		[DemandForecastModelType.List()]
		[PXUIField(DisplayName = "Demand Forecast Model Used")]
		[PXFormula(typeof(Default<INItemRep.forecastModelType>))]
		public virtual String ForecastModelType
		{
			get
			{
				return this._ForecastModelType;
			}
			set
			{
				this._ForecastModelType = value;
			}
		}
		#endregion
		#region ForecastPeriodType
		public abstract class forecastPeriodType : PX.Data.IBqlField
		{
		}
		protected String _ForecastPeriodType;
		[PXDBString(2, IsFixed = true)]
		[DemandPeriodType.List()]
		[PXUIField(DisplayName = "Forecast Period Type Used")]
		public virtual String ForecastPeriodType
		{
			get
			{
				return this._ForecastPeriodType;
			}
			set
			{
				this._ForecastPeriodType = value;
			}
		}
		#endregion
		#region LastForecastDate
		public abstract class lastFCApplicationDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _LastFCApplicationDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Last Forecast Results Application Date", Enabled = false)]
		public virtual DateTime? LastFCApplicationDate
		{
			get
			{
				return this._LastFCApplicationDate;
			}
			set
			{
				this._LastFCApplicationDate = value;
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
    [Serializable]
    [PXHidden]
	public partial class INItemStatsA: INItemStats
 {  
  #region InventoryID
  public new abstract class inventoryID : IBqlField {}
  #endregion
  #region SiteID
  public new abstract class siteID : IBqlField {}
  #endregion
  #region QtyOnHand
  public new abstract class qtyOnHand : IBqlField {}
  #endregion
  #region TotalCost
  public new abstract class totalCost : IBqlField {}
  #endregion
  #region MinCost
  public new abstract class minCost : IBqlField {}
  #endregion
  #region MaxCost
  public new abstract class maxCost : IBqlField {}
  #endregion
  #region LastCost
  public new abstract class lastCost : IBqlField {}
  #endregion
  #region LastCostDate
  public new abstract class lastCostDate : IBqlField {}
  #endregion
  #region LastPurchasePrice
  public new abstract class lastPurchasePrice : IBqlField {}
  #endregion
  #region LastPurchaseDate
  public new abstract class lastPurchaseDate : IBqlField {}
  #endregion
  #region LastVendorID
  public new abstract class lastVendorID : IBqlField {}
  #endregion
  #region ValMethod
  public new abstract class valMethod : IBqlField {}
  #endregion
  #region tstamp
  public new abstract class tstamp : IBqlField {}
  #endregion
 }

	[System.SerializableAttribute()]
	public partial class INItemStats : PX.Data.IBqlTable
	{
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[PXDBInt(IsKey = true)]
		[PXDefault()]
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
		[PXDBInt(IsKey = true)]
		[PXDefault()]
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
		#region ValMethod
		public abstract class valMethod : PX.Data.IBqlField
		{
		}
		protected String _ValMethod;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(typeof(Search<InventoryItem.valMethod, Where<InventoryItem.inventoryID, Equal<Current<INItemStats.inventoryID>>>>))]
		public virtual String ValMethod
		{
			get
			{
				return this._ValMethod;
			}
			set
			{
				this._ValMethod = value;
			}
		}
		#endregion
		#region LastPurchaseDate
		public abstract class lastPurchaseDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _LastPurchaseDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Last Purchase Date", Enabled = false)]
		public virtual DateTime? LastPurchaseDate
		{
			get
			{
				return this._LastPurchaseDate;
			}
			set
			{
				this._LastPurchaseDate = value;
			}
		}
		#endregion
		#region LastPurchasePrice
		public abstract class lastPurchasePrice : PX.Data.IBqlField
		{
		}
		protected Decimal? _LastPurchasePrice;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Last Purchase Price", Enabled = false)]
		public virtual Decimal? LastPurchasePrice
		{
			get
			{
				return this._LastPurchasePrice;
			}
			set
			{
				this._LastPurchasePrice = value;
			}
		}
		#endregion
		#region LastVendorID
		public abstract class lastVendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _LastVendorID;
		[Vendor(DescriptionField = typeof(Vendor.acctName), Enabled = false)]
		public virtual Int32? LastVendorID
		{
			get
			{
				return this._LastVendorID;
			}
			set
			{
				this._LastVendorID = value;
			}
		}
		#endregion
		#region LastCost
		public abstract class lastCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _LastCost;
		[PXDBPriceCost()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Last Cost", Enabled = false)]
		public virtual Decimal? LastCost
		{
			get
			{
				return this._LastCost;
			}
			set
			{
				this._LastCost = value;
			}
		}
		#endregion
		#region LastCostDate
		public abstract class lastCostDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _LastCostDate;
        [PXDBLastChangeDateTime(typeof(INItemStats.lastCost))]
		public virtual DateTime? LastCostDate
		{
			get
			{
				return this._LastCostDate;
			}
			set
			{
				this._LastCostDate = value;
			}
		}
		#endregion
		#region AvgCost
		public abstract class avgCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _AvgCost;
		[PXDBPriceCostCalced(typeof(Switch<Case<Where<INItemStats.qtyOnHand, Equal<decimal0>>, decimal0>, Div<INItemStats.totalCost, INItemStats.qtyOnHand>>), typeof(Decimal))]
		[PXPriceCost()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Average Cost", Enabled = false)]
		public virtual Decimal? AvgCost
		{
			get
			{
				return this._AvgCost;
			}
			set
			{
				this._AvgCost = value;
			}
		}
		#endregion
		#region MinCost
		public abstract class minCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _MinCost;
		[PXDBPriceCost()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Minimal Cost", Enabled = false)]
		public virtual Decimal? MinCost
		{
			get
			{
				return this._MinCost;
			}
			set
			{
				this._MinCost = value;
			}
		}
		#endregion
		#region MaxCost
		public abstract class maxCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _MaxCost;
		[PXDBPriceCost()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Max. Cost", Enabled = false)]
		public virtual Decimal? MaxCost
		{
			get
			{
				return this._MaxCost;
			}
			set
			{
				this._MaxCost = value;
			}
		}
		#endregion
		#region QtyOnHand
		public abstract class qtyOnHand : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyOnHand;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? QtyOnHand
		{
			get
			{
				return this._QtyOnHand;
			}
			set
			{
				this._QtyOnHand = value;
			}
		}
		#endregion
		#region TotalCost
		public abstract class totalCost : PX.Data.IBqlField
		{
		}
		protected decimal? _TotalCost;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TotalCost
		{
			get
			{
				return this._TotalCost;
			}
			set
			{
				this._TotalCost = value;
			}
		}
		#endregion
		#region QtyReceived
		public abstract class qtyReceived : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyReceived;
		[PXDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Decimal? QtyReceived
		{
			get
			{
				return this._QtyReceived;
			}
			set
			{
				this._QtyReceived = value;
			}
		}
		#endregion
		#region CostReceived
		public abstract class costReceived : PX.Data.IBqlField
		{
		}
		protected decimal? _CostReceived;
		[PXDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Decimal? CostReceived
		{
			get
			{
				return this._CostReceived;
			}
			set
			{
				this._CostReceived = value;
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
	
	[PXBreakInheritance]
    [Serializable]
    [PXHidden]
	public partial class INItemStatsTotal : INItemStats
	{
		#region InventoryID
		public new abstract class inventoryID : PX.Data.IBqlField
		{
		}		
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public override Int32? InventoryID
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
		#region LastCost
		public new abstract class lastCost : PX.Data.IBqlField
		{
		}		
		[PXDBPriceCost()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Last Cost", Enabled = false)]
		public override Decimal? LastCost
		{
			get
			{
				return this._LastCost;
			}
			set
			{
				this._LastCost = value;
			}
		}
		#endregion
		#region LastCostDate
		public new abstract class lastCostDate : PX.Data.IBqlField
		{
		}
        [PXDBLastChangeDateTime(typeof(INItemStatsTotal.lastCost))]
        public override DateTime? LastCostDate
		{
			get
			{
				return this._LastCostDate;
			}
			set
			{
				this._LastCostDate = value;
			}
		}
		#endregion
		#region NextCostDate
		public abstract class nextCostDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _NextCostDate;
		[PXDate()]
		[PXDBScalar(typeof(Search<INItemStatsA.lastCostDate,
			Where<INItemStatsA.inventoryID, Equal<INItemStatsTotal.inventoryID>,
				And<Where<INItemStatsA.lastCostDate, Greater<INItemStatsTotal.lastCostDate>,
							 Or<Where<INItemStatsA.lastCostDate, Equal<INItemStatsTotal.lastCostDate>,
										 And<INItemStatsA.siteID, Greater<INItemStatsTotal.siteID>>>>>>>>))]
		public virtual DateTime? NextCostDate
		{
			get
			{
				return this._NextCostDate;
			}
			set
			{
				this._NextCostDate = value;
			}
		}
		#endregion
		#region TotalCost
		public new abstract class totalCost : PX.Data.IBqlField
		{
		}
		[PXDBScalar(typeof(Search4<INItemStatsA.totalCost, Where<INItemStatsA.inventoryID, Equal<INItemStatsTotal.inventoryID>>, Aggregate<Sum<INItemStatsA.totalCost>>>))]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public override Decimal? TotalCost
		{
			get
			{
				return this._TotalCost;
			}
			set
			{
				this._TotalCost = value;
			}
		}
		#endregion
		#region QtyOnHand
		public new abstract class qtyOnHand : PX.Data.IBqlField
		{
		}
		[PXDBScalar(typeof(Search4<INItemStatsA.qtyOnHand, Where<INItemStatsA.inventoryID, Equal<INItemStatsTotal.inventoryID>>, Aggregate<Sum<INItemStatsA.qtyOnHand>>>))]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public override Decimal? QtyOnHand
		{
			get
			{
				return this._QtyOnHand;
			}
			set
			{
				this._QtyOnHand = value;
			}
		}
		#endregion
		#region AvgCost
		public new abstract class avgCost : PX.Data.IBqlField
		{
		}
		[PXPriceCost()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Average Cost", Enabled = false)]
		[PXDBPriceCostCalced(typeof(Switch<Case<Where<INItemStatsTotal.qtyOnHand, Equal<decimal0>>, decimal0>, Div<INItemStatsTotal.totalCost, INItemStatsTotal.qtyOnHand>>), typeof(Decimal))]
		public override Decimal? AvgCost
		{
			get
			{
				return this._AvgCost;
			}
			set
			{
				this._AvgCost = value;
			}
		}
		#endregion
		#region MinCost
		public new abstract class minCost : PX.Data.IBqlField
		{
		}
		[PXPriceCost()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Min. Cost", Enabled = false)]
		[PXDBCostScalar(typeof(Search4<INItemStatsA.minCost, Where<INItemStatsA.inventoryID, Equal<INItemStatsTotal.inventoryID>>, Aggregate<Min<INItemStatsA.minCost>>>))]
		public override Decimal? MinCost
		{
			get
			{
				return this._MinCost;
			}
			set
			{
				this._MinCost = value;
			}
		}
		#endregion
		#region MaxCost
		public new abstract class maxCost : PX.Data.IBqlField
		{
		}
		[PXPriceCost()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Max. Cost", Enabled = false)]
		[PXDBCostScalar(typeof(Search4<INItemStatsA.maxCost, Where<INItemStatsA.inventoryID, Equal<INItemStatsTotal.inventoryID>>, Aggregate<Max<INItemStatsA.maxCost>>>))]		
		public override Decimal? MaxCost
		{
			get
			{
				return this._MaxCost;
			}
			set
			{
				this._MaxCost = value;
			}
		}
		#endregion
	}

	[PXProjection(typeof(Select2<InventoryItem, LeftJoin<INItemStatsTotal, On<INItemStatsTotal.inventoryID, Equal<InventoryItem.inventoryID>>, LeftJoin<INSite, On<INSite.siteID, Equal<InventoryItem.dfltSiteID>>>>, Where<INItemStatsTotal.nextCostDate, IsNull>>))]
    [PXCacheName(Messages.ItemCostStatistics)]
    [Serializable]
    public partial class INItemCost : IBqlTable
	{
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected int? _InventoryID;
		[PXDBInt(IsKey = true, BqlField = typeof(InventoryItem.inventoryID))]
		[PXDefault(typeof(InventoryItem.inventoryID))]
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
		#region LastCost
		public abstract class lastCost : PX.Data.IBqlField
		{
		}

		protected decimal? _LastCost;
		[PXDBPriceCost(BqlField = typeof(INItemStatsTotal.lastCost))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Last Cost", Enabled = false)]
		public virtual Decimal? LastCost
		{
			get
			{
				return this._LastCost;
			}
			set
			{
				this._LastCost = value;
			}
		}
		#endregion
		#region LastCostDate
		public abstract class lastCostDate : PX.Data.IBqlField
		{
		}

		protected DateTime? _LastCostDate;
		[PXDBDate(BqlField = typeof(INItemStatsTotal.lastCostDate))]
		public virtual DateTime? LastCostDate
		{
			get
			{
				return this._LastCostDate;
			}
			set
			{
				this._LastCostDate = value;
			}
		}
		#endregion
		#region NextCostDate
		public abstract class nextCostDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _NextCostDate;
		[PXDate()]
		[PXDBScalar(typeof(Search<INItemStatsA.lastCostDate,
			Where<INItemStatsA.inventoryID, Equal<INItemStatsTotal.inventoryID>,
				And<INItemStatsA.lastCostDate, Greater<INItemStatsTotal.lastCostDate>>>>))]
		public virtual DateTime? NextCostDate
		{
			get
			{
				return this._NextCostDate;
			}
			set
			{
				this._NextCostDate = value;
			}
		}
		#endregion
		#region TotalCost
		public abstract class totalCost : PX.Data.IBqlField
		{
		}
		protected decimal? _TotalCost;
		[PXDBScalar(typeof(Search4<INItemStatsA.totalCost, Where<INItemStatsA.inventoryID, Equal<INItemStatsTotal.inventoryID>>, Aggregate<Sum<INItemStatsA.totalCost>>>))]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Decimal? TotalCost
		{
			get
			{
				return this._TotalCost;
			}
			set
			{
				this._TotalCost = value;
			}
		}
		#endregion
		#region QtyOnHand
		public  abstract class qtyOnHand : PX.Data.IBqlField
		{
		}
		protected decimal? _QtyOnHand;
		[PXDBScalar(typeof(Search4<INItemStatsA.qtyOnHand, Where<INItemStatsA.inventoryID, Equal<INItemStatsTotal.inventoryID>>, Aggregate<Sum<INItemStatsA.qtyOnHand>>>))]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Decimal? QtyOnHand
		{
			get
			{
				return this._QtyOnHand;
			}
			set
			{
				this._QtyOnHand = value;
			}
		}
		#endregion
		#region AvgCost
		public abstract class avgCost : PX.Data.IBqlField
		{
		}

		protected decimal? _AvgCost;
		[PXPriceCost()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Average Cost", Enabled = false)]
		[PXDBPriceCostCalced(typeof(Switch<Case<Where<INItemStatsTotal.qtyOnHand, Equal<decimal0>>, decimal0>, Div<INItemStatsTotal.totalCost, INItemStatsTotal.qtyOnHand>>), typeof(Decimal))]
		public virtual Decimal? AvgCost
		{
			get
			{
				return this._AvgCost;
			}
			set
			{
				this._AvgCost = value;
			}
		}
		#endregion
		#region MinCost
		public  abstract class minCost : PX.Data.IBqlField
		{
		}
		protected decimal? _MinCost;
		[PXPriceCost()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Min. Cost", Enabled = false)]
		[PXDBCostScalar(typeof(Search4<INItemStatsA.minCost, Where<INItemStatsA.inventoryID, Equal<INItemStatsTotal.inventoryID>>, Aggregate<Min<INItemStatsA.minCost>>>))]
		public virtual Decimal? MinCost
		{
			get
			{
				return this._MinCost;
			}
			set
			{
				this._MinCost = value;
			}
		}
		#endregion
		#region MaxCost
		public abstract class maxCost : PX.Data.IBqlField
		{
		}
		protected decimal? _MaxCost;
		[PXPriceCost()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Max. Cost", Enabled = false)]
		[PXDBCostScalar(typeof(Search4<INItemStatsA.maxCost, Where<INItemStatsA.inventoryID, Equal<INItemStatsTotal.inventoryID>>, Aggregate<Max<INItemStatsA.maxCost>>>))]		
		public virtual Decimal? MaxCost
		{
			get
			{
				return this._MaxCost;
			}
			set
			{
				this._MaxCost = value;
			}
		}
		#endregion		
		#region TranUnitCost
		public abstract class tranUnitCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranUnitCost;
		[PXDBCalced(typeof(Switch<Case<Where<InventoryItem.valMethod, Equal<INValMethod.standard>>, InventoryItem.stdCost,
								Case<Where<InventoryItem.valMethod, Equal<INValMethod.average>, And<INSite.avgDefaultCost, Equal<INSite.avgDefaultCost.lastCost>,
										Or<InventoryItem.valMethod, Equal<INValMethod.fIFO>, And<INSite.fIFODefaultCost, Equal<INSite.avgDefaultCost.lastCost>,
										Or<InventoryItem.valMethod, Equal<INValMethod.specific>>>>>>,
										INItemStatsTotal.lastCost>>,
								Switch<Case<Where<INItemStatsTotal.qtyOnHand, Equal<decimal0>>, decimal0,
                                    Case<Where<Div<INItemStatsTotal.totalCost, INItemStatsTotal.qtyOnHand>,Less<decimal0>>,INItemStatsTotal.lastCost>>, 
                                    Div<INItemStatsTotal.totalCost, INItemStatsTotal.qtyOnHand>>>), typeof(Decimal))]
		public virtual Decimal? TranUnitCost
		{
			get
			{
				return this._TranUnitCost;
			}
			set
			{
				this._TranUnitCost = value;
			}
		}
		#endregion
	}

	[PXProjection(typeof(Select2<InventoryItem,
	CrossJoin<INSite,
	LeftJoin<INItemRep, On<INItemRep.inventoryID, Equal<InventoryItem.inventoryID>, And<INItemRep.replenishmentClassID, Equal<INSite.replenishmentClassID>>>,
	LeftJoinSingleTable<INItemSite, On<INItemSite.inventoryID, Equal<InventoryItem.inventoryID>, And<INItemSite.siteID, Equal<INSite.siteID>>>>>>>))]
    [Serializable]
    [PXHidden]
	public partial class INItemSiteSettings : IBqlTable
	{
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[PXDBInt(IsKey = true, BqlField = typeof(InventoryItem.inventoryID))]
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
		#region DefaultSubItemID
		public abstract class defaultSubItemID : PX.Data.IBqlField
		{
		}
		protected Int32? _DefaultSubItemID;
		[PXDBInt(BqlField=typeof(InventoryItem.defaultSubItemID))]
		public virtual Int32? DefaultSubItemID
		{
			get;
			set;
		}
		#endregion
		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SiteID;
		[PXDBInt(IsKey = true, BqlField = typeof(INSite.siteID))]
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
		#region PreferredVendorID
		public abstract class preferredVendorID : IBqlField { }
		[PXDBCalced(typeof(IsNull<InventoryItem.preferredVendorID, INItemSite.preferredVendorID>), typeof(Int32))]
		public virtual Int32? PreferredVendorID
		{
			get;
			set;
		}
		#endregion
		#region PreferredVendorLocationID
		public abstract class preferredVendorLocationID : IBqlField { }
		[PXDBCalced(typeof(IsNull<InventoryItem.preferredVendorLocationID, INItemSite.preferredVendorLocationID>), typeof(Int32))]
		public virtual Int32? PreferredVendorLocationID
		{
			get;
			set;
		}
		#endregion
		#region ReplenishmentSource
		public abstract class replenishmentSource : PX.Data.IBqlField
		{
		}
		[PXDBCalced(typeof(IsNull<INItemSite.replenishmentSource, INItemRep.replenishmentSource>), typeof(string))]
		public virtual string ReplenishmentSource
		{
			get;
			set;
		}
		#endregion
		#region ReplenishmentSourceSiteID
		public abstract class replenishmentSourceSiteID : PX.Data.IBqlField
		{
		}
		[PXDBCalced(typeof(IsNull<INItemSite.replenishmentSourceSiteID, INItemRep.replenishmentSourceSiteID>), typeof(Int32))]
		public virtual Int32? ReplenishmentSourceSiteID
		{
			get;
			set;
		}
		#endregion
		#region POCreate
		public abstract class pOCreate : PX.Data.IBqlField
		{
		}
		[PXDBCalced(
			typeof(Switch<Case<Where<IsNull<INItemSite.replenishmentSource,INItemRep.replenishmentSource>, Equal<INReplenishmentSource.purchaseToOrder>,
					Or<IsNull<INItemSite.replenishmentSource,INItemRep.replenishmentSource>, Equal<INReplenishmentSource.dropShipToOrder>>>, boolTrue>, boolFalse>), typeof(Boolean))]
		public virtual Boolean? POCreate
		{
			get;
			set;
		}
		#endregion
		#region POSource
		public abstract class pOSource : PX.Data.IBqlField
		{
		}
		[PXDBCalced(
			typeof(Switch<Case<Where<IsNull<INItemSite.replenishmentSource,INItemRep.replenishmentSource>, Equal<INReplenishmentSource.dropShipToOrder>>, INReplenishmentSource.dropShipToOrder,
						Case<Where<IsNull<INItemSite.replenishmentSource,INItemRep.replenishmentSource>, Equal<INReplenishmentSource.purchaseToOrder>>, INReplenishmentSource.purchaseToOrder>>,
						INReplenishmentSource.none>), typeof(string))]
		public virtual string POSource
		{
			get;
			set;
		}
		#endregion
	}
}
