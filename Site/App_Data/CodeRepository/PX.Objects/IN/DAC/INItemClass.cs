namespace PX.Objects.IN
{
	using System;
	using PX.Data;
	using PX.Objects.TX;
	using PX.Objects.CS;
	using PX.Objects.DR;
	using PX.TM;

	[System.SerializableAttribute()]
	[PXPrimaryGraph(typeof(INItemClassMaint))]
	[PXCacheName(Messages.ItemClass)]
	public partial class INItemClass : PX.Data.IBqlTable, PX.SM.IIncludable
	{
		#region ItemClassID
		public abstract class itemClassID : PX.Data.IBqlField
		{
		}
		protected String _ItemClassID;
		[PXDefault()]
		[PXDBString(10, IsUnicode = true, IsKey = true, InputMask = ">aaaaaaaaaa")]
		[PXUIField(DisplayName = "Class ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<INItemClass.itemClassID, Where<INItemClass.stkItem, Equal<False>, Or<Where<INItemClass.stkItem, Equal<True>, And<FeatureInstalled<FeaturesSet.distributionModule>>>>>>))]
		[PX.Data.EP.PXFieldDescription]
		public virtual String ItemClassID
		{
			get
			{
				return this._ItemClassID;
			}
			set
			{
				this._ItemClassID = value;
			}
		}
		#endregion
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;
		[PXDBLocalizableString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		[PX.Data.EP.PXFieldDescription]
		public virtual String Descr
		{
			get
			{
				return this._Descr;
			}
			set
			{
				this._Descr = value;
			}
		}
		#endregion
		#region StkItem
		public abstract class stkItem : PX.Data.IBqlField
		{
		}
		protected Boolean? _StkItem;
		[PXDBBool()]
		[PXDefault(true, typeof(Search2<INItemClass.stkItem, InnerJoin<INSetup, On<INItemClass.itemClassID, Equal<INSetup.dfltItemClassID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Stock Item")]
		public virtual Boolean? StkItem
		{
			get
			{
				return this._StkItem;
			}
			set
			{
				this._StkItem = value;
			}
		}
		#endregion
		#region NegQty
		public abstract class negQty : PX.Data.IBqlField
		{
		}
		protected Boolean? _NegQty;
		[PXDBBool()]
		[PXDefault(false, typeof(Search2<INItemClass.negQty, InnerJoin<INSetup, On<INItemClass.itemClassID, Equal<INSetup.dfltItemClassID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Allow Negative Quantity")]
		public virtual Boolean? NegQty
		{
			get
			{
				return this._NegQty;
			}
			set
			{
				this._NegQty = value;
			}
		}
		#endregion
		#region InclQtySOReverse
		public abstract class inclQtySOReverse : PX.Data.IBqlField
		{
		}
		protected Boolean? _InclQtySOReverse;
		[PXDBBool()]
		[PXDefault(false, typeof(Search2<INItemClass.inclQtySOReverse, InnerJoin<INSetup, On<INItemClass.itemClassID, Equal<INSetup.dfltItemClassID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Include Qty. on Returns")]
		public virtual Boolean? InclQtySOReverse
		{
			get
			{
				return this._InclQtySOReverse;
			}
			set
			{
				this._InclQtySOReverse = value;
			}
		}
		#endregion
		#region InclQtySOBackOrdered
		public abstract class inclQtySOBackOrdered : PX.Data.IBqlField
		{
		}
		protected Boolean? _InclQtySOBackOrdered;
		[PXDBBool()]
		[PXDefault(true, typeof(Search2<INItemClass.inclQtySOBackOrdered, InnerJoin<INSetup, On<INItemClass.itemClassID, Equal<INSetup.dfltItemClassID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Deduct Qty. on Back Orders")]
		public virtual Boolean? InclQtySOBackOrdered
		{
			get
			{
				return this._InclQtySOBackOrdered;
			}
			set
			{
				this._InclQtySOBackOrdered = value;
			}
		}
		#endregion
		#region InclQtySOPrepared
		public abstract class inclQtySOPrepared : PX.Data.IBqlField
		{
		}
		protected Boolean? _InclQtySOPrepared;
		[PXDBBool()]
		[PXDefault(false, typeof(Search2<INItemClass.inclQtySOPrepared, InnerJoin<INSetup, On<INItemClass.itemClassID, Equal<INSetup.dfltItemClassID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Deduct Qty. on Sales Prepared")]
		public virtual Boolean? InclQtySOPrepared
		{
			get
			{
				return this._InclQtySOPrepared;
			}
			set
			{
				this._InclQtySOPrepared = value;
			}
		}
		#endregion
		#region InclQtySOBooked
		public abstract class inclQtySOBooked : PX.Data.IBqlField
		{
		}
		protected Boolean? _InclQtySOBooked;
		[PXDBBool()]
		[PXDefault(true, typeof(Search2<INItemClass.inclQtySOBooked, InnerJoin<INSetup, On<INItemClass.itemClassID, Equal<INSetup.dfltItemClassID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Deduct Qty. on Sales Orders")]
		public virtual Boolean? InclQtySOBooked
		{
			get
			{
				return this._InclQtySOBooked;
			}
			set
			{
				this._InclQtySOBooked = value;
			}
		}
		#endregion
		#region InclQtySOShipped
		public abstract class inclQtySOShipped : PX.Data.IBqlField
		{
		}
		protected Boolean? _InclQtySOShipped;
		[PXDBBool()]
		[PXDefault(true, typeof(Search2<INItemClass.inclQtySOShipped, InnerJoin<INSetup, On<INItemClass.itemClassID, Equal<INSetup.dfltItemClassID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Deduct Qty. Shipped")]
		public virtual Boolean? InclQtySOShipped
		{
			get
			{
				return this._InclQtySOShipped;
			}
			set
			{
				this._InclQtySOShipped = value;
			}
		}
		#endregion
		#region InclQtySOShipping
		public abstract class inclQtySOShipping : PX.Data.IBqlField
		{
		}
		protected Boolean? _InclQtySOShipping;
		[PXDBBool()]
		[PXDefault(true, typeof(Search2<INItemClass.inclQtySOShipping, InnerJoin<INSetup, On<INItemClass.itemClassID, Equal<INSetup.dfltItemClassID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Deduct Qty. Allocated")]
		public virtual Boolean? InclQtySOShipping
		{
			get
			{
				return this._InclQtySOShipping;
			}
			set
			{
				this._InclQtySOShipping = value;
			}
		}
		#endregion
		#region InclQtyInTransit
		public abstract class inclQtyInTransit : PX.Data.IBqlField
		{
		}
		protected Boolean? _InclQtyInTransit;
		[PXDBBool()]
		[PXDefault(false, typeof(Search2<INItemClass.inclQtyInTransit, InnerJoin<INSetup, On<INItemClass.itemClassID, Equal<INSetup.dfltItemClassID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Include Qty. in Transit")]
		public virtual Boolean? InclQtyInTransit
		{
			get
			{
				return this._InclQtyInTransit;
			}
			set
			{
				this._InclQtyInTransit = value;
			}
		}
		#endregion
		#region InclQtyPOReceipts
		public abstract class inclQtyPOReceipts : PX.Data.IBqlField
		{
		}
		protected Boolean? _InclQtyPOReceipts;
		[PXDBBool()]
		[PXDefault(false, typeof(Search2<INItemClass.inclQtyPOReceipts, InnerJoin<INSetup, On<INItemClass.itemClassID, Equal<INSetup.dfltItemClassID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Include Qty. on PO Receipts")]
		public virtual Boolean? InclQtyPOReceipts
		{
			get
			{
				return this._InclQtyPOReceipts;
			}
			set
			{
				this._InclQtyPOReceipts = value;
			}
		}
		#endregion
		#region InclQtyPOPrepared
		public abstract class inclQtyPOPrepared : PX.Data.IBqlField
		{
		}
		protected Boolean? _InclQtyPOPrepared;
		[PXDBBool()]
		[PXDefault(false, typeof(Search2<INItemClass.inclQtyPOPrepared, InnerJoin<INSetup, On<INItemClass.itemClassID, Equal<INSetup.dfltItemClassID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Include Qty. on Purchase Prepared")]
		public virtual Boolean? InclQtyPOPrepared
		{
			get
			{
				return this._InclQtyPOPrepared;
			}
			set
			{
				this._InclQtyPOPrepared = value;
			}
		}
		#endregion
		#region InclQtyPOOrders
		public abstract class inclQtyPOOrders : PX.Data.IBqlField
		{
		}
		protected Boolean? _InclQtyPOOrders;
		[PXDBBool()]
		[PXDefault(false, typeof(Search2<INItemClass.inclQtyPOOrders, InnerJoin<INSetup, On<INItemClass.itemClassID, Equal<INSetup.dfltItemClassID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Include Qty. on Purchase Orders")]
		public virtual Boolean? InclQtyPOOrders
		{
			get
			{
				return this._InclQtyPOOrders;
			}
			set
			{
				this._InclQtyPOOrders = value;
			}
		}
		#endregion
		#region InclQtyINIssues
		public abstract class inclQtyINIssues : PX.Data.IBqlField
		{
		}
		protected Boolean? _InclQtyINIssues;
		[PXDBBool()]
		[PXDefault(true, typeof(Search2<INItemClass.inclQtyINIssues, InnerJoin<INSetup, On<INItemClass.itemClassID, Equal<INSetup.dfltItemClassID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Deduct Qty. on Issues")]
		public virtual Boolean? InclQtyINIssues
		{
			get
			{
				return this._InclQtyINIssues;
			}
			set
			{
				this._InclQtyINIssues = value;
			}
		}
		#endregion
		#region InclQtyINReceipts
		public abstract class inclQtyINReceipts : PX.Data.IBqlField
		{
		}
		protected Boolean? _InclQtyINReceipts;
		[PXDBBool()]
		[PXDefault(false, typeof(Search2<INItemClass.inclQtyINReceipts, InnerJoin<INSetup, On<INItemClass.itemClassID, Equal<INSetup.dfltItemClassID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Include Qty. on Receipts")]
		public virtual Boolean? InclQtyINReceipts
		{
			get
			{
				return this._InclQtyINReceipts;
			}
			set
			{
				this._InclQtyINReceipts = value;
			}
		}
		#endregion
		#region InclQtyINAssemblyDemand
		public abstract class inclQtyINAssemblyDemand : PX.Data.IBqlField
		{
		}
		protected Boolean? _InclQtyINAssemblyDemand;
		[PXDBBool()]
		[PXDefault(true, typeof(Search2<INItemClass.inclQtyINAssemblyDemand, InnerJoin<INSetup, On<INItemClass.itemClassID, Equal<INSetup.dfltItemClassID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Deduct Qty. of Kit Assembly Demand")]
		public virtual Boolean? InclQtyINAssemblyDemand
		{
			get
			{
				return this._InclQtyINAssemblyDemand;
			}
			set
			{
				this._InclQtyINAssemblyDemand = value;
			}
		}
		#endregion
		#region InclQtyINAssemblySupply
		public abstract class inclQtyINAssemblySupply : PX.Data.IBqlField
		{
		}
		protected Boolean? _InclQtyINAssemblySupply;
		[PXDBBool()]
		[PXDefault(false, typeof(Search2<INItemClass.inclQtyINAssemblySupply, InnerJoin<INSetup, On<INItemClass.itemClassID, Equal<INSetup.dfltItemClassID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Include Qty. of Kit Assembly Supply")]
		public virtual Boolean? InclQtyINAssemblySupply
		{
			get
			{
				return this._InclQtyINAssemblySupply;
			}
			set
			{
				this._InclQtyINAssemblySupply = value;
			}
		}
		#endregion
		#region ValMethod
		public abstract class valMethod : PX.Data.IBqlField
		{
		}
		protected String _ValMethod;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(INValMethod.Average, typeof(Search2<INItemClass.valMethod, InnerJoin<INSetup, On<INItemClass.itemClassID, Equal<INSetup.dfltItemClassID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[INValMethod.List()]
		[PXUIField(DisplayName = "Valuation Method")]
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
		#region BaseUnit
		public abstract class baseUnit : PX.Data.IBqlField
		{
		}
		protected String _BaseUnit;
		[PXDefault(typeof(Search2<INItemClass.baseUnit, InnerJoin<INSetup, On<INItemClass.itemClassID, Equal<INSetup.dfltItemClassID>>>>))]
		[INUnit(DisplayName = "Base Unit")]
		public virtual String BaseUnit
		{
			get
			{
				return this._BaseUnit;
			}
			set
			{
				this._BaseUnit = value;
			}
		}
		#endregion
		#region SalesUnit
		public abstract class salesUnit : PX.Data.IBqlField
		{
		}
		protected String _SalesUnit;
		[PXDefault(typeof(Search2<INItemClass.salesUnit, InnerJoin<INSetup, On<INItemClass.itemClassID, Equal<INSetup.dfltItemClassID>>>>))]
		[PXFormula(typeof(Switch<Case<Where<FeatureInstalled<FeaturesSet.multipleUnitMeasure>>, Current<salesUnit>>, baseUnit>))]
		[INUnit(null, typeof(INItemClass.baseUnit), DisplayName = "Sales Unit", Visibility = PXUIVisibility.Visible)]
		public virtual String SalesUnit
		{
			get
			{
				return this._SalesUnit;
			}
			set
			{
				this._SalesUnit = value;
			}
		}
		#endregion
		#region PurchaseUnit
		public abstract class purchaseUnit : PX.Data.IBqlField
		{
		}
		protected String _PurchaseUnit;
		[PXDefault(typeof(Search2<INItemClass.purchaseUnit, InnerJoin<INSetup, On<INItemClass.itemClassID, Equal<INSetup.dfltItemClassID>>>>))]
		[PXFormula(typeof(Switch<Case<Where<FeatureInstalled<FeaturesSet.multipleUnitMeasure>>, Current<purchaseUnit>>, baseUnit>))]
		[INUnit(null, typeof(INItemClass.baseUnit), DisplayName = "Purchase Unit", Visibility = PXUIVisibility.Visible)]
		public virtual String PurchaseUnit
		{
			get
			{
				return this._PurchaseUnit;
			}
			set
			{
				this._PurchaseUnit = value;
			}
		}
		#endregion
		#region PostClassID
		public abstract class postClassID : PX.Data.IBqlField
		{
		}
		protected String _PostClassID;
		[PXDBString(10, IsUnicode = true)]
		[PXSelector(typeof(Search<INPostClass.postClassID>), DescriptionField = typeof(INPostClass.descr))]
		[PXUIField(DisplayName = "Posting Class")]
		[PXDefault(typeof(Search2<INItemClass.postClassID, InnerJoin<INSetup, On<INItemClass.itemClassID, Equal<INSetup.dfltItemClassID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual String PostClassID
		{
			get
			{
				return this._PostClassID;
			}
			set
			{
				this._PostClassID = value;
			}
		}
		#endregion
		#region LotSerClassID
		public abstract class lotSerClassID : PX.Data.IBqlField
		{
		}
		protected String _LotSerClassID;
		[PXDBString(10, IsUnicode = true)]
		[PXSelector(typeof(Search<INLotSerClass.lotSerClassID>), DescriptionField = typeof(INLotSerClass.descr))]
		[PXUIField(DisplayName = "Lot/Serial Class")]
		[PXDefault(typeof(Search2<INItemClass.lotSerClassID, InnerJoin<INSetup, On<INItemClass.itemClassID, Equal<INSetup.dfltItemClassID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual String LotSerClassID
		{
			get
			{
				return this._LotSerClassID;
			}
			set
			{
				this._LotSerClassID = value;
			}
		}
		#endregion
		#region TaxCategoryID
		public abstract class taxCategoryID : PX.Data.IBqlField
		{
		}
		protected String _TaxCategoryID;
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Tax Category")]
        [PXSelector(typeof(TaxCategory.taxCategoryID), DescriptionField = typeof(TaxCategory.descr))]
        [PXRestrictor(typeof(Where<TaxCategory.active, Equal<True>>), TX.Messages.InactiveTaxCategory, typeof(TaxCategory.taxCategoryID))]
        [PXDefault(typeof(Search2<INItemClass.taxCategoryID, InnerJoin<INSetup, On<INItemClass.itemClassID, Equal<INSetup.dfltItemClassID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual String TaxCategoryID
		{
			get
			{
				return this._TaxCategoryID;
			}
			set
			{
				this._TaxCategoryID = value;
			}
		}
		#endregion
		#region DeferredCode
		public abstract class deferredCode : PX.Data.IBqlField
		{
		}
		protected string _DeferredCode;
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXUIField(DisplayName = "Deferral Code")]
		[PXSelector(typeof(Search<DRDeferredCode.deferredCodeID>))]
		[PXDefault(typeof(Search2<INItemClass.deferredCode, InnerJoin<INSetup, On<INItemClass.itemClassID, Equal<INSetup.dfltItemClassID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual String DeferredCode
		{
			get
			{
				return this._DeferredCode;
			}
			set
			{
				this._DeferredCode = value;
			}
		}
		#endregion
		#region ItemType
		public abstract class itemType : PX.Data.IBqlField
		{
		}
		protected String _ItemType;
		[PXDBString(1, IsFixed = true)]
		[INItemTypes.List()]
		[PXDefault(typeof(Search2<INItemClass.itemType, InnerJoin<INSetup, On<INItemClass.itemClassID, Equal<INSetup.dfltItemClassID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Item Type")]
		public virtual String ItemType
		{
			get
			{
				return this._ItemType;
			}
			set
			{
				this._ItemType = value;
			}
		}
		#endregion
		#region PriceClassID
		public abstract class priceClassID : PX.Data.IBqlField
		{
		}
		protected String _PriceClassID;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault(typeof(Search2<INItemClass.priceClassID, InnerJoin<INSetup, On<INItemClass.itemClassID, Equal<INSetup.dfltItemClassID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXSelector(typeof(INPriceClass.priceClassID), DescriptionField = typeof(INPriceClass.description))]
		[PXUIField(DisplayName = "Price Class", Visibility = PXUIVisibility.Visible)]
		public virtual String PriceClassID
		{
			get
			{
				return this._PriceClassID;
			}
			set
			{
				this._PriceClassID = value;
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
		[PXOwnerSelector(typeof(InventoryItem.priceWorkgroupID))]
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
		#region DfltSiteID
		public abstract class dfltSiteID : PX.Data.IBqlField
		{
		}
		protected Int32? _DfltSiteID;
		[IN.Site(DisplayName = "Default Warehouse", DescriptionField = typeof(INSite.descr))]
		public virtual Int32? DfltSiteID
		{
			get
			{
				return this._DfltSiteID;
			}
			set
			{
				this._DfltSiteID = value;
			}
		}
		#endregion
		#region MinGrossProfitPct
		public abstract class minGrossProfitPct : PX.Data.IBqlField
		{
		}
		protected Decimal? _MinGrossProfitPct;
		[PXDefault(TypeCode.Decimal, "0.0", typeof(Search2<INItemClass.minGrossProfitPct, InnerJoin<INSetup, On<INItemClass.itemClassID, Equal<INSetup.dfltItemClassID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXDBDecimal(2, MinValue = 0)]
		[PXUIField(DisplayName = "Min. Markup %", Visibility = PXUIVisibility.Visible)]
		public virtual Decimal? MinGrossProfitPct
		{
			get
			{
				return this._MinGrossProfitPct;
			}
			set
			{
				this._MinGrossProfitPct = value;
			}
		}
		#endregion
		#region MarkupPct
		public abstract class markupPct : PX.Data.IBqlField
		{
		}
		protected Decimal? _MarkupPct;
		[PXDBDecimal(6, MinValue = 0, MaxValue = 1000)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Markup %")]
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
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXNote(DescriptionField = typeof(INItemClass.itemClassID),
			Selector = typeof(INItemClass.itemClassID))]
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
		#region DemandCalculation
		public abstract class demandCalculation : PX.Data.IBqlField
		{
		}
		protected string _DemandCalculation;
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Demand Calculation")]
		[PXDefault(INDemandCalculation.ItemClassSettings)]
		[INDemandCalculation.List]
		public virtual string DemandCalculation
		{
			get
			{
				return this._DemandCalculation;
			}
			set
			{
				this._DemandCalculation = value;
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
		#region GroupMask
		public abstract class groupMask : IBqlField
		{
		}
		protected Byte[] _GroupMask;
		[PXDBGroupMask()]
		public virtual Byte[] GroupMask
		{
			get
			{
				return this._GroupMask;
			}
			set
			{
				this._GroupMask = value;
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

		#region Included
		public abstract class included : PX.Data.IBqlField
		{
		}
		protected bool? _Included;
		[PXBool]
		[PXUIField(DisplayName = "Included")]
		[PXUnboundDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual bool? Included
		{
			get
			{
				return this._Included;
			}
			set
			{
				this._Included = value;
			}
		}
		#endregion
	}

	public class INDemandCalculation
	{
		public const string ItemClassSettings = "I";
		public const string HardDemand = "H";
		public class List : PXStringListAttribute
		{
			public List()
				: base(
				new string[] { ItemClassSettings, HardDemand },
				new string[] { Messages.ItemClassSettings, Messages.HardDemand })
			{
			}
		}

		public class itemClassSettings : Constant<string>
		{
			public itemClassSettings() : base(ItemClassSettings) { }
		}

		public class hardDemand : Constant<string>
		{
			public hardDemand() : base(HardDemand) { }
		}
	}

	// Used in AR674000.rpx
	public class FilterItemByClass : IBqlTable
	{
		#region ItemClassID
		public abstract class itemClassID : PX.Data.IBqlField
		{
		}
		protected String _ItemClassID;
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Item Class", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<INItemClass.itemClassID>), DescriptionField = typeof(INItemClass.descr))]
		public virtual String ItemClassID
		{
			get
			{
				return this._ItemClassID;
			}
			set
			{
				this._ItemClassID = value;
			}
		}
		#endregion

		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[Inventory(typeof(Search<InventoryItem.inventoryCD, Where<InventoryItem.itemClassID, Equal<Optional<FilterItemByClass.itemClassID>>, Or<Optional<FilterItemByClass.itemClassID>, IsNull>>>), typeof(InventoryItem.inventoryCD), typeof(InventoryItem.descr))]
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

	}
}
