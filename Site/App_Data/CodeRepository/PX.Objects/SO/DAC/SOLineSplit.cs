namespace PX.Objects.SO
{
	using System;
    using System.Text;
	using PX.Data;
	using PX.Objects.IN;
	using PX.Objects.CS;
	using PX.Objects.GL;
	using PX.Objects.PO;
	
	[System.SerializableAttribute()]
	[PXCacheName(Messages.SOLineSplit)]
	public partial class SOLineSplit : PX.Data.IBqlTable, ILSDetail
	{
		#region OrderType
		public abstract class orderType : PX.Data.IBqlField
		{
		}
		protected String _OrderType;
		[PXDBString(2, IsKey = true, IsFixed = true)]
		[PXDefault(typeof(SOOrder.orderType))]
        [PXSelector(typeof(Search<SOOrderType.orderType>), CacheGlobal = true)]
		public virtual String OrderType
		{
			get
			{
				return this._OrderType;
			}
			set
			{
				this._OrderType = value;
			}
		}
		#endregion
		#region OrderNbr
		public abstract class orderNbr : PX.Data.IBqlField
		{
		}
		protected String _OrderNbr;
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "")]
		[PXDBDefault(typeof(SOOrder.orderNbr))]
		[PXParent(typeof(Select<SOOrder, Where<SOOrder.orderType, Equal<Current<SOLineSplit.orderType>>, And<SOOrder.orderNbr, Equal<Current<SOLineSplit.orderNbr>>>>>))]
		[PXParent(typeof(Select<SOLine, Where<SOLine.orderType, Equal<Current<SOLineSplit.orderType>>, And<SOLine.orderNbr, Equal<Current<SOLineSplit.orderNbr>>, And<SOLine.lineNbr, Equal<Current<SOLineSplit.lineNbr>>>>>>))]
		public virtual String OrderNbr
		{
			get
			{
				return this._OrderNbr;
			}
			set
			{
				this._OrderNbr = value;
			}
		}
		#endregion
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineNbr;
		[PXDBInt(IsKey = true)]
		[PXDefault(typeof(SOLine.lineNbr))]
		public virtual Int32? LineNbr
		{
			get
			{
				return this._LineNbr;
			}
			set
			{
				this._LineNbr = value;
			}
		}
		#endregion
		#region SplitLineNbr
		public abstract class splitLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _SplitLineNbr;
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		[PXLineNbr(typeof(SOOrder.lineCntr))]
		[PXUIField(DisplayName = "Allocation ID", Visible = false, IsReadOnly = true)]
		public virtual Int32? SplitLineNbr
		{
			get
			{
				return this._SplitLineNbr;
			}
			set
			{
				this._SplitLineNbr = value;
			}
		}
		#endregion
		#region ParentSplitLineNbr
		public abstract class parentSplitLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _ParentSplitLineNbr;
		[PXDBInt()]
		[PXUIField(DisplayName = "Parent Allocation ID", Visible = false, IsReadOnly = true)]
		public virtual Int32? ParentSplitLineNbr
		{
			get
			{
				return this._ParentSplitLineNbr;
			}
			set
			{
				this._ParentSplitLineNbr = value;
			}
		}
		#endregion
		#region Operation
		public abstract class operation : PX.Data.IBqlField
		{
		}
		protected String _Operation;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(typeof(SOLine.operation))]
		[PXSelectorMarker(typeof(Search<SOOrderTypeOperation.operation, Where<SOOrderTypeOperation.orderType, Equal<Current<SOLineSplit.orderType>>>>))]
		public virtual String Operation
		{
			get
			{
				return this._Operation;
			}
			set
			{
				this._Operation = value;
			}
		}
		#endregion
		#region InvtMult
		public abstract class invtMult : PX.Data.IBqlField
		{
		}
		protected Int16? _InvtMult;
		[PXDBShort()]
		[PXDefault(typeof(INTran.invtMult))]
		public virtual Int16? InvtMult
		{
			get
			{
				return this._InvtMult;
			}
			set
			{
				this._InvtMult = value;
			}
		}
		#endregion
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[Inventory(Enabled = false, Visible = true)]
		[PXDefault(typeof(SOLine.inventoryID))]
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
		#region LineType
		public abstract class lineType : PX.Data.IBqlField
		{
		}
		protected String _LineType;
		[PXDBString(2, IsFixed = true)]
		[PXDefault(typeof(SOLine.lineType))]
		public virtual String LineType
		{
			get
			{
				return this._LineType;
			}
			set
			{
				this._LineType = value;
			}
		}
		#endregion
		#region IsStockItem
		public abstract class isStockItem : IBqlField { }
		[PXDBBool()]
		[PXFormula(typeof(Selector<SOLineSplit.inventoryID, InventoryItem.stkItem>))]
		public bool? IsStockItem
		{
			get;
			set;
		}
		#endregion
        #region IsAllocated
        public abstract class isAllocated : PX.Data.IBqlField
        {
        }
        protected Boolean? _IsAllocated;
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Allocated")]
        public virtual Boolean? IsAllocated
        {
            get
            {
                return this._IsAllocated;
            }
            set
            {
                this._IsAllocated = value;
            }
        }
        #endregion
        #region IsMergeable
        public abstract class isMergeable : PX.Data.IBqlField
        {
        }
        protected Boolean? _IsMergeable;
        [PXBool()]
        [PXFormula(typeof(True))]
        public virtual Boolean? IsMergeable
        {
            get
            {
                return this._IsMergeable;
            }
            set
            {
                this._IsMergeable = value;
            }
        }
        #endregion
		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SiteID;
        [SiteAvail(typeof(SOLineSplit.inventoryID), typeof(SOLineSplit.subItemID), new Type[] { typeof(INSite.siteCD), typeof(INSiteStatus.qtyOnHand), typeof(INSiteStatus.qtyAvail), typeof(INSiteStatus.active), typeof(INSite.descr) }, DisplayName = "Alloc. Warehouse")]
		//[PXDefault(typeof(SOLine.siteID))]
        [PXFormula(typeof(Switch<Case<Where<SOLineSplit.isAllocated, Equal<False>>, Current<SOLine.siteID>>, SOLineSplit.siteID>))]
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
		[SOLocationAvail(typeof(SOLineSplit.inventoryID), typeof(SOLineSplit.subItemID), typeof(SOLineSplit.siteID), typeof(SOLineSplit.tranType), typeof(SOLineSplit.invtMult))]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
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
		#region ToSiteID
		public abstract class toSiteID : PX.Data.IBqlField
		{
		}
		protected Int32? _ToSiteID;
		[IN.Site(DisplayName = "Orig. Warehouse")]
		[PXDefault(typeof(SOLine.siteID))]
		public virtual Int32? ToSiteID
		{
			get
			{
				return this._ToSiteID;
			}
			set
			{
				this._ToSiteID = value;
			}
		}
		#endregion
		#region SubItemID
		public abstract class subItemID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubItemID;
		[IN.SubItem(typeof(SOLineSplit.inventoryID))]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[SubItemStatusVeryfier(typeof(SOLineSplit.inventoryID), typeof(SOLineSplit.siteID), InventoryItemStatus.Inactive, InventoryItemStatus.NoSales)]
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
		#region ShipDate
		public abstract class shipDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ShipDate;
		[PXDBDate()]
		[PXDefault(typeof(SOLine.shipDate), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Ship On", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual DateTime? ShipDate
		{
			get
			{
				return this._ShipDate;
			}
			set
			{
				this._ShipDate = value;
			}
		}
		#endregion
        #region ShipComplete
        public abstract class shipComplete : PX.Data.IBqlField
        {
        }
        protected String _ShipComplete;
        [PXDBString(1, IsFixed = true)]
        [PXDefault(typeof(SOLine.shipComplete), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual String ShipComplete
        {
            get
            {
                return this._ShipComplete;
            }
            set
            {
                this._ShipComplete = value;
            }
        }
        #endregion
		#region Completed
		public abstract class completed : PX.Data.IBqlField
		{
		}
		protected Boolean? _Completed;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Completed", Enabled = false)]
		public virtual Boolean? Completed
		{
			get
			{
				return this._Completed;
			}
			set
			{
				this._Completed = value;
			}
		}
		#endregion
		#region ShipmentNbr
		public abstract class shipmentNbr : PX.Data.IBqlField
		{
		}
		protected string _ShipmentNbr;
		[PXDBString(IsUnicode = true)]
		[PXUIFieldAttribute(DisplayName="Shipment Nbr.", Enabled = false)]
		public virtual string ShipmentNbr
		{
			get
			{
				return this._ShipmentNbr;
			}
			set
			{
				this._ShipmentNbr = value;
			}
		}
		#endregion
		#region LotSerialNbr
		public abstract class lotSerialNbr : PX.Data.IBqlField
		{
		}
		protected String _LotSerialNbr;
        [SOLotSerialNbr(typeof(SOLineSplit.inventoryID), typeof(SOLineSplit.subItemID), typeof(SOLineSplit.locationID), typeof(SOLine.lotSerialNbr), FieldClass = "LotSerial")]
		public virtual String LotSerialNbr
		{
			get
			{
				return this._LotSerialNbr;
			}
			set
			{
				this._LotSerialNbr = value;
			}
		}
		#endregion
		#region LotSerClassID
		public abstract class lotSerClassID : PX.Data.IBqlField
		{
		}
		protected String _LotSerClassID;
		[PXString(10, IsUnicode = true)]
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
		#region AssignedNbr
		public abstract class assignedNbr : PX.Data.IBqlField
		{
		}
		protected String _AssignedNbr;
		[PXString(30, IsUnicode = true)]
		public virtual String AssignedNbr
		{
			get
			{
				return this._AssignedNbr;
			}
			set
			{
				this._AssignedNbr = value;
			}
		}
		#endregion
		#region ExpireDate
		public abstract class expireDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ExpireDate;
		[INExpireDate(typeof(SOLineSplit.inventoryID))]
		public virtual DateTime? ExpireDate
		{
			get
			{
				return this._ExpireDate;
			}
			set
			{
				this._ExpireDate = value;
			}
		}
		#endregion
		#region UOM
		public abstract class uOM : PX.Data.IBqlField
		{
		}
		protected String _UOM;
		[INUnit(typeof(SOLineSplit.inventoryID), DisplayName = "UOM", Enabled = false)]
		[PXDefault(typeof(SOLine.uOM))]
		public virtual String UOM
		{
			get
			{
				return this._UOM;
			}
			set
			{
				this._UOM = value;
			}
		}
		#endregion
		#region Qty
		public abstract class qty : PX.Data.IBqlField
		{
		}
		protected Decimal? _Qty;
		[PXDBQuantity(typeof(SOLineSplit.uOM), typeof(SOLineSplit.baseQty))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Quantity")]
		public virtual Decimal? Qty
		{
			get
			{
				return this._Qty;
			}
			set
			{
				this._Qty = value;
			}
		}
		#endregion
		#region BaseQty
		public abstract class baseQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseQty;
		[PXDBDecimal(6)]
		public virtual Decimal? BaseQty
		{
			get
			{
				return this._BaseQty;
			}
			set
			{
				this._BaseQty = value;
			}
		}
		#endregion
		#region ShippedQty
		public abstract class shippedQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _ShippedQty;
		[PXDBQuantity(typeof(SOLineSplit.uOM), typeof(SOLineSplit.baseShippedQty))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. On Shipments", Enabled = false)]
		public virtual Decimal? ShippedQty
		{
			get
			{
				return this._ShippedQty;
			}
			set
			{
				this._ShippedQty = value;
			}
		}
		#endregion
		#region BaseShippedQty
		public abstract class baseShippedQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseShippedQty;
		[PXDBDecimal(6, MinValue = 0)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? BaseShippedQty
		{
			get
			{
				return this._BaseShippedQty;
			}
			set
			{
				this._BaseShippedQty = value;
			}
		}
		#endregion
		#region ReceivedQty
		public abstract class receivedQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _ReceivedQty;
		[PXDBQuantity(typeof(SOLineSplit.uOM), typeof(SOLineSplit.baseReceivedQty), MinValue = 0)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. Received", Enabled = false)]
		public virtual Decimal? ReceivedQty
		{
			get
			{
				return this._ReceivedQty;
			}
			set
			{
				this._ReceivedQty = value;
			}
		}
		#endregion
		#region BaseReceivedQty
		public abstract class baseReceivedQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseReceivedQty;
		[PXDBDecimal(6, MinValue = 0)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? BaseReceivedQty
		{
			get
			{
				return this._BaseReceivedQty;
			}
			set
			{
				this._BaseReceivedQty = value;
			}
		}
		#endregion
		#region UnreceivedQty
		public abstract class unreceivedQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnreceivedQty;
		[PXQuantity(typeof(SOLineSplit.uOM), typeof(SOLineSplit.baseUnreceivedQty), MinValue = 0)]
		[PXFormula(typeof(Sub<SOLineSplit.qty, SOLineSplit.receivedQty>))]
		public virtual Decimal? UnreceivedQty
		{
			get
			{
				return this._UnreceivedQty;
			}
			set
			{
				this._UnreceivedQty = value;
			}
		}
		#endregion
		#region BaseUnreceivedQty
		public abstract class baseUnreceivedQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseUnreceivedQty;
		[PXDecimal(6, MinValue = 0)]
		[PXFormula(typeof(Sub<SOLineSplit.baseQty, SOLineSplit.baseReceivedQty>))]
		public virtual Decimal? BaseUnreceivedQty
		{
			get
			{
				return this._BaseUnreceivedQty;
			}
			set
			{
				this._BaseUnreceivedQty = value;
			}
		}
		#endregion
		#region OpenQty
		public abstract class openQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _OpenQty;
		[PXQuantity(typeof(SOLineSplit.uOM), typeof(SOLineSplit.baseOpenQty), MinValue = 0)]
		[PXFormula(typeof(Sub<SOLineSplit.qty, SOLineSplit.shippedQty>))]
		public virtual Decimal? OpenQty
		{
			get
			{
				return this._OpenQty;
			}
			set
			{
				this._OpenQty = value;
			}
		}
		#endregion
		#region BaseOpenQty
		public abstract class baseOpenQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseOpenQty;
		[PXDecimal(6, MinValue = 0)]
		[PXFormula(typeof(Sub<SOLineSplit.baseQty, SOLineSplit.baseShippedQty>))]
		public virtual Decimal? BaseOpenQty
		{
			get
			{
				return this._BaseOpenQty;
			}
			set
			{
				this._BaseOpenQty = value;
			}
		}
		#endregion
		#region OrderDate
		public abstract class orderDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _OrderDate;
		[PXDBDate()]
		[PXDBDefault(typeof(SOOrder.orderDate))]
		public virtual DateTime? OrderDate
		{
			get
			{
				return this._OrderDate;
			}
			set
			{
				this._OrderDate = value;
			}
		}
		#endregion
		#region TranType
		public abstract class tranType : PX.Data.IBqlField
		{
		}
		protected String _TranType;
		[PXFormula(typeof(Selector<SOLineSplit.operation, SOOrderTypeOperation.iNDocType>))]
		[PXString(SOOrderTypeOperation.iNDocType.Length, IsFixed = true)]
		public virtual String TranType
		{
			get
			{
				return this._TranType;
			}
			set
			{
				this._TranType = value;
			}
		}
		#endregion
		#region TranDate
		public virtual DateTime? TranDate
		{
			get { return this._OrderDate; }
		}
		#endregion
		#region PlanType
		public abstract class planType : PX.Data.IBqlField
		{
		}
		protected String _PlanType;
		[PXFormula(typeof(Selector<SOLineSplit.operation, SOOrderTypeOperation.orderPlanType>))]
		[PXString(SOOrderTypeOperation.orderPlanType.Length, IsFixed = true)]
		public virtual String PlanType
		{
			get
			{
				return this._PlanType;
			}
			set
			{
				this._PlanType = value;
			}
		}
		#endregion
		#region AllocatedPlanType
		public abstract class allocatedPlanType : PX.Data.IBqlField
		{
		}
		[PXDBScalar(typeof(Search<INPlanType.planType, Where<INPlanType.inclQtySOShipping, Equal<True>>>))]
		[PXDefault(typeof(Search<INPlanType.planType, Where<INPlanType.inclQtySOShipping, Equal<True>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual String AllocatedPlanType
		{
			get;
			set;
		}
		#endregion
		#region BackOrderPlanType
		public abstract class backOrderPlanType : PX.Data.IBqlField
		{
		}
		protected String _BackOrderPlanType;
		[PXDBScalar(typeof(Search<INPlanType.planType, Where<INPlanType.inclQtySOBackOrdered, Equal<True>>>))]
		[PXDefault(typeof(Search<INPlanType.planType,	Where<INPlanType.inclQtySOBackOrdered, Equal<True>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual String BackOrderPlanType
		{
			get
			{
				return this._BackOrderPlanType;
			}
			set
			{
				this._BackOrderPlanType = value;
			}
		}
		#endregion
		#region OrigPlanType
		public abstract class origPlanType : PX.Data.IBqlField
		{
		}
		[PXDBString(2, IsFixed = true)]
		[PXSelector(typeof(Search<INPlanType.planType>), CacheGlobal = true)]
		public virtual String OrigPlanType
		{
			get;
			set;
		}
		#endregion   
		#region RequireShipping
		public abstract class requireShipping : PX.Data.IBqlField
		{
		}
		protected bool? _RequireShipping;
        [PXBool()]
        [PXFormula(typeof(Selector<SOLineSplit.orderType, SOOrderType.requireShipping>))]
		public virtual bool? RequireShipping
		{
			get
			{
				return this._RequireShipping;
			}
			set
			{
				this._RequireShipping = value;
			}
		}
		#endregion
		#region RequireAllocation
		public abstract class requireAllocation : PX.Data.IBqlField
		{
		}
		protected bool? _RequireAllocation;
        [PXBool()]
        [PXFormula(typeof(Selector<SOLineSplit.orderType, SOOrderType.requireAllocation>))]
		public virtual bool? RequireAllocation
		{
			get
			{
				return this._RequireAllocation;
			}
			set
			{
				this._RequireAllocation = value;
			}
		}
		#endregion
		#region RequireLocation
		public abstract class requireLocation : PX.Data.IBqlField
		{
		}
		protected bool? _RequireLocation;
        [PXBool()]
        [PXFormula(typeof(Selector<SOLineSplit.orderType, SOOrderType.requireLocation>))]
		public virtual bool? RequireLocation
		{
			get
			{
				return this._RequireLocation;
			}
			set
			{
				this._RequireLocation = value;
			}
		}
		#endregion

		#region POCreate
		public abstract class pOCreate : PX.Data.IBqlField
		{
		}
		protected Boolean? _POCreate;
		[PXDBBool()]
		[PXDefault()]
        [PXFormula(typeof(Switch<Case<Where<SOLineSplit.isAllocated, Equal<False>, And<SOLineSplit.pOReceiptNbr, IsNull>>, Current<SOLine.pOCreate>>, False>))]
		[PXUIField(DisplayName = "Mark for PO", Visible = true, Enabled = false)]
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
		#region POCompleted
		public abstract class pOCompleted : PX.Data.IBqlField
		{
		}
		protected Boolean? _POCompleted;
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? POCompleted
		{
			get
			{
				return this._POCompleted;
			}
			set
			{
				this._POCompleted = value;
			}
		}
		#endregion
		#region POCancelled
		public abstract class pOCancelled : PX.Data.IBqlField
		{
		}
		protected Boolean? _POCancelled;
		[PXDBBool()]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Boolean? POCancelled
		{
			get
			{
				return this._POCancelled;
			}
			set
			{
				this._POCancelled = value;
			}
		}
		#endregion
		#region POSource
		public abstract class pOSource : PX.Data.IBqlField
		{
		}
		protected string _POSource;
		[PXDBString()]
        [PXFormula(typeof(Switch<Case<Where<SOLineSplit.isAllocated, Equal<False>>, Current<SOLine.pOSource>>, Null>))]
		public virtual string POSource
		{
			get
			{
				return this._POSource;
			}
			set
			{
				this._POSource = value;
			}
		}
		#endregion
        #region FixedSource
        public abstract class fixedSource : PX.Data.IBqlField
        {
        }
        protected String _FixedSource;
        [PXString(1, IsFixed = true)]
        [PXDBCalced(typeof(Switch<Case<Where<SOLineSplit.pOCreate, Equal<True>>, INReplenishmentSource.purchased, Case<Where<SOLineSplit.siteID, NotEqual<SOLineSplit.toSiteID>>, INReplenishmentSource.transfer>>, INReplenishmentSource.none>), typeof(string))]
        public virtual String FixedSource
        {
            get
            {
                return this._FixedSource;
            }
            set
            {
                this._FixedSource = value;
            }
        }
        #endregion
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[PXDBInt()]
        [PXFormula(typeof(Switch<Case<Where<SOLineSplit.isAllocated, Equal<False>>, Current<SOLine.vendorID>>, Null>))]
        public virtual Int32? VendorID
		{
			get
			{
				return this._VendorID;
			}
			set
			{
				this._VendorID = value;
			}
		}
		#endregion
        #region POSiteID
        public abstract class pOSiteID : PX.Data.IBqlField
        {
        }
        protected Int32? _POSiteID;
        [PXDBInt()]
        [PXFormula(typeof(Switch<Case<Where<SOLineSplit.isAllocated, Equal<False>>, Current<SOLine.pOSiteID>>, Null>))]
        public virtual Int32? POSiteID
        {
            get
            {
                return this._POSiteID;
            }
            set
            {
                this._POSiteID = value;
            }
        }
        #endregion
		#region POType
		public abstract class pOType : PX.Data.IBqlField
		{
		}
		protected String _POType;
		[PXDBString(2, IsFixed = true)]
		[PXUIField(DisplayName = "PO Type", Enabled = false)]
		[POOrderType.RBDList]
		public virtual String POType
		{
			get
			{
				return this._POType;
			}
			set
			{
				this._POType = value;
			}
		}
		#endregion
		#region PONbr
		public abstract class pONbr : PX.Data.IBqlField
		{
		}
		protected String _PONbr;
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "PO Nbr.", Enabled = false)]
		[PXSelector(typeof(Search<POOrder.orderNbr, Where<POOrder.orderType, Equal<Current<SOLineSplit.pOType>>>>), DescriptionField = typeof(POOrder.orderDesc))]
		public virtual String PONbr
		{
			get
			{
				return this._PONbr;
			}
			set
			{
				this._PONbr = value;
			}
		}
		#endregion
		#region POLineNbr
		public abstract class pOLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _POLineNbr;
		[PXDBInt()]
		[PXUIField(DisplayName = "PO Line Nbr.", Enabled = false)]
		public virtual Int32? POLineNbr
		{
			get
			{
				return this._POLineNbr;
			}
			set
			{
				this._POLineNbr = value;
			}
		}
		#endregion
        #region POReceiptType
        public abstract class pOReceiptType : PX.Data.IBqlField
        {
        }
        protected String _POReceiptType;
        [PXDBString(2, IsFixed = true)]
        [PXUIField(DisplayName = "PO Receipt Type", Enabled = false)]
        public virtual String POReceiptType
        {
            get
            {
                return this._POReceiptType;
            }
            set
            {
                this._POReceiptType = value;
            }
        }
        #endregion
		#region POReceiptNbr
		public abstract class pOReceiptNbr : PX.Data.IBqlField
		{
		}
		protected String _POReceiptNbr;
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "PO Receipt Nbr.", Enabled = false)]
		[PXSelector(typeof(Search<POReceipt.receiptNbr, Where<POReceipt.receiptType, Equal<Current<SOLineSplit.pOReceiptType>>>>), DescriptionField = typeof(POReceipt.invoiceNbr))]
		public virtual String POReceiptNbr
		{
			get
			{
				return this._POReceiptNbr;
			}
			set
			{
				this._POReceiptNbr = value;
			}
		}
		#endregion
		#region SOOrderType
		public abstract class sOOrderType : PX.Data.IBqlField
		{
		}
		protected String _SOOrderType;
		[PXDBString(2, IsFixed = true)]
		public virtual String SOOrderType
		{
			get
			{
				return this._SOOrderType;
			}
			set
			{
				this._SOOrderType = value;
			}
		}
		#endregion
		#region SOOrderNbr
		public abstract class sOOrderNbr : PX.Data.IBqlField
		{
		}
		protected String _SOOrderNbr;
		[PXDBString(15, IsUnicode = true)]
		public virtual String SOOrderNbr
		{
			get
			{
				return this._SOOrderNbr;
			}
			set
			{
				this._SOOrderNbr = value;
			}
		}
		#endregion
		#region SOLineNbr
		public abstract class sOLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _SOLineNbr;
		[PXDBInt()]
		public virtual Int32? SOLineNbr
		{
			get
			{
				return this._SOLineNbr;
			}
			set
			{
				this._SOLineNbr = value;
			}
		}
		#endregion
		#region SOSplitLineNbr
		public abstract class sOSplitLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _SOSplitLineNbr;
		[PXDBInt()]
		public virtual Int32? SOSplitLineNbr
		{
			get
			{
				return this._SOSplitLineNbr;
			}
			set
			{
				this._SOSplitLineNbr = value;
			}
		}
		#endregion
        #region RefNoteID
        public abstract class refNoteID : PX.Data.IBqlField
        {
        }
        protected Guid? _RefNoteID;
        [PXUIField(DisplayName = "Related Document", Enabled = false)]
        [PXRefNote()]
        public virtual Guid? RefNoteID
        {
            get
            {
                return this._RefNoteID;
            }
            set
            {
                this._RefNoteID = value;
            }
        }
        public class PXRefNoteAttribute : PX.Data.PXRefNoteAttribute
        {
            public PXRefNoteAttribute()
                :base()
            { 
            }

            public class PXLinkState : PXStringState
            {
                protected object[] _keys;
                protected Type _target;

                public object[] keys
                {
                    get { return _keys; }
                }

                public Type target
                {
                    get { return _target; }
                }

                public PXLinkState(object value)
                    :base(value)
                { 
                }

                public static PXFieldState CreateInstance(object value, Type target, object[] keys)
                {
                    PXLinkState state = value as PXLinkState;
                    if (state == null)
                    {
                        PXFieldState field = value as PXFieldState;
                        if (field != null && field.DataType != typeof(object) && field.DataType != typeof(string))
                        {
                            return field;
                        }
                        state = new PXLinkState(value);
                    }
                    if (target != null)
                    {
                        state._target = target;
                    }
                    if (keys != null)
                    {
                        state._keys = keys;
                    }

                    return state;
                }
            }

            public override void CacheAttached(PXCache sender)
            {
                base.CacheAttached(sender);

                PXButtonDelegate del = delegate(PXAdapter adapter)
                {
                    PXCache cache = adapter.View.Graph.Caches[typeof(SOLineSplit)];
                    if (cache.Current != null)
                    {
						object val = cache.GetValueExt(cache.Current, _FieldName);

                        PXLinkState state = val as PXLinkState;
                        if (state != null)
                        {
                            helper.NavigateToRow(state.target.FullName, state.keys, PXRedirectHelper.WindowMode.NewWindow);
                        }
                        else
                        {
                            helper.NavigateToRow((Guid?)cache.GetValue(cache.Current, _FieldName), PXRedirectHelper.WindowMode.NewWindow);
                        }
                    }

                    return adapter.Get();
                };

                string ActionName = sender.GetItemType().Name + "$" + _FieldName + "$Link";
                sender.Graph.Actions[ActionName] = (PXAction)Activator.CreateInstance(typeof(PXNamedAction<>).MakeGenericType(typeof(SOOrder)), new object[] { sender.Graph, ActionName, del, new PXEventSubscriberAttribute[] { new PXUIFieldAttribute { MapEnableRights = PXCacheRights.Select } }});
            }

            public virtual object GetEntityRowID(PXCache cache, object[] keys)
            {
                return GetEntityRowID(cache, keys, ", ");
            }

            public static object GetEntityRowID(PXCache cache, object[] keys, string separator)
            {
                StringBuilder result = new StringBuilder();
                int i = 0;
                foreach (string key in cache.Keys)
                {
                    if (i >= keys.Length) break;
                    object val = keys[i++];
                    cache.RaiseFieldSelecting(key, null, ref val, true);

                    if (val != null)
                    {
                        if (result.Length != 0) result.Append(separator);
                        result.Append(val.ToString().TrimEnd());
                    }
                }
                return result.ToString();
            }

            public override void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
            {
                SOLineSplit row = e.Row as SOLineSplit;

                if (row != null && !string.IsNullOrEmpty(row.PONbr))
                {
                    e.ReturnValue = GetEntityRowID(sender.Graph.Caches[typeof(POOrder)], new object[] { row.POType, row.PONbr });
                    e.ReturnState = PXLinkState.CreateInstance(e.ReturnState, typeof(POOrder), new object[] { row.POType, row.PONbr });
                }
                else if (row != null && !string.IsNullOrEmpty(row.ShipmentNbr))
                {
                    e.ReturnValue = GetEntityRowID(sender.Graph.Caches[typeof(SOShipment)], new object[] { row.ShipmentNbr });
                    e.ReturnState = PXLinkState.CreateInstance(e.ReturnState, typeof(SOShipment), new object[] { row.ShipmentNbr });
                }
                else if (row != null && !string.IsNullOrEmpty(row.SOOrderNbr))
                {
                    e.ReturnValue = GetEntityRowID(sender.Graph.Caches[typeof(SOOrder)], new object[] { row.SOOrderType, row.SOOrderNbr });
                    e.ReturnState = PXLinkState.CreateInstance(e.ReturnState, typeof(SOOrder), new object[] { row.SOOrderType, row.SOOrderNbr });
                }
                else if (row != null && !string.IsNullOrEmpty(row.POReceiptNbr))
                {
                    e.ReturnValue = GetEntityRowID(sender.Graph.Caches[typeof(POReceipt)], new object[] { row.POReceiptType, row.POReceiptNbr });
                    e.ReturnState = PXLinkState.CreateInstance(e.ReturnState, typeof(POReceipt), new object[] { row.POReceiptType, row.POReceiptNbr });
                }
                else
                {
                    base.FieldSelecting(sender, e);
                }
            }
        }

        #endregion
		#region PlanID
		public abstract class planID : PX.Data.IBqlField
		{
		}
		protected Int64? _PlanID;
		[PXDBLong()]
		[SOLineSplitPlanID(typeof(SOOrder.noteID), typeof(SOOrder.hold), typeof(SOOrder.orderDate))]
		public virtual Int64? PlanID
		{
			get
			{
				return this._PlanID;
			}
			set
			{
				this._PlanID = value;
			}
		}
		#endregion
		#region ProjectID
		public abstract class projectID : PX.Data.IBqlField
		{
		}
		protected Int32? _ProjectID;
		[PXFormula(typeof(Selector<SOLineSplit.locationID, INLocation.projectID>))]
		[PXInt]
		public virtual Int32? ProjectID
		{
			get
			{
				return this._ProjectID;
			}
			set
			{
				this._ProjectID = value;
			}
		}
		#endregion
		#region TaskID
		public abstract class taskID : PX.Data.IBqlField
		{
		}
		protected Int32? _TaskID;
		[PXFormula(typeof(Selector<SOLineSplit.locationID, INLocation.taskID>))]
		[PXInt]
		public virtual Int32? TaskID
		{
			get
			{
				return this._TaskID;
			}
			set
			{
				this._TaskID = value;
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
