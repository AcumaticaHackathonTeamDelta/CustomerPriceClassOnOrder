namespace PX.Objects.PO
{
	using System;
	using PX.Data;
	using PX.Objects.GL;
	using PX.Objects.AP;
	using PX.Objects.IN;
	using PX.Objects.CM;
	using PX.Objects.TX;
using PX.Objects.CR;
	using PX.Objects.CS;
	using PX.Objects.PM;
	
	[System.SerializableAttribute()]
    [PXCacheName(Messages.POLineShort)]
	public partial class POLine : PX.Data.IBqlTable, IAPTranSource, IItemPlanMaster
	{
		#region Selected
		public abstract class selected : PX.Data.IBqlField
		{
		}
		protected bool? _Selected = false;
		[PXBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Selected")]
		public virtual bool? Selected
		{
			get
			{
				return _Selected;
			}
			set
			{
				_Selected = value;
			}
		}
		#endregion
		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		protected Int32? _BranchID;
		[Branch(typeof(POOrder.branchID))]
		public virtual Int32? BranchID
		{
			get
			{
				return this._BranchID;
			}
			set
			{
				this._BranchID = value;
			}
		}
		#endregion
		#region OrderType
		public abstract class orderType : PX.Data.IBqlField
		{
		}
		protected String _OrderType;
		[PXDBString(2, IsKey = true, IsFixed = true)]
		[PXDBDefault(typeof(POOrder.orderType))]
		[PXUIField(DisplayName = "Order Type", Visibility = PXUIVisibility.Visible, Visible = false)]
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
		[PXDBDefault(typeof(POOrder.orderNbr))]
		[PXParent(typeof(Select<POOrder, Where<POOrder.orderType, Equal<Current<POLine.orderType>>, And<POOrder.orderNbr, Equal<Current<POLine.orderNbr>>>>>))]
		[PXUIField(DisplayName = "Order Nbr.", Visibility = PXUIVisibility.Invisible, Visible = false)]
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
		[PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false)]
		[PXLineNbr(typeof(POOrder.lineCntr))]
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
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[POLineInventoryItem(Filterable = true)]
        [PXRestrictor(typeof(
        Where<Current<POLine.orderType>, NotEqual<POOrderType.dropShip>,
        Or<InventoryItem.stkItem, Equal<True>,
            Or<InventoryItem.nonStockReceipt, Equal<True>>>>), IN.Messages.DropshipItemMustRequireReceipt)]
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
		[PXDefault(POLineType.Service)]
		[PXDBString(2, IsFixed = true)]
		[POLineTypeList2(typeof(POLine.orderType), typeof(POLine.inventoryID))]
		[PXUIField(DisplayName = "Line Type")]
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

		#region PlanID
		public abstract class planID : PX.Data.IBqlField
		{
		}
		protected Int64? _PlanID;
		[PXDBLong()]
		[POLinePlanID(typeof(POOrder.noteID),typeof(POOrder.hold))]
		[PXUIField(DisplayName = "Plan ID", Visible = false, Enabled = false)]
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

		#region POType
		public abstract class pOType : PX.Data.IBqlField
		{
		}
		protected String _POType;
		[PXDBString(2, IsFixed = true)]
		[POOrderType.List()]
		[PXUIField(DisplayName = "Order Type", Enabled = false)]
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
		[PXUIField(DisplayName = "Order Nbr.", Enabled = false)]
		[PO.RefNbr(typeof(Search2<POOrder.orderNbr,
			InnerJoinSingleTable<Vendor, On<POOrder.vendorID, Equal<Vendor.bAccountID>>>,
			 Where<POOrder.orderType, Equal<Current<POLine.pOType>>,
			 And<Match<Vendor, Current<AccessInfo.userName>>>>,
			OrderBy<Desc<POOrder.orderNbr>>>), Filterable = true)]
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
		[PXParent(typeof(Select<POLineR, 
			Where<POLineR.orderType, Equal<Current<POLine.pOType>>,
										And<POLineR.orderType,Equal<POOrderType.blanket>,
										And<POLineR.orderNbr, Equal<Current<POLine.pONbr>>,
										And<POLineR.lineNbr, Equal<Current<POLine.pOLineNbr>>>>>>>))]
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
		

		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[POVendor()]
		[PXDBDefault(typeof(POOrder.vendorID))]
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
		#region VendorLocationID
		public abstract class vendorLocationID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorLocationID;
		[PXInt()]
		public virtual Int32? VendorLocationID
		{
			get
			{
				return this._VendorLocationID;
			}
			set
			{
				this._VendorLocationID = value;
			}
		}
		#endregion
		#region ShipToBAccountID
		public abstract class shiptToBAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _ShipToBAccountID;
		[PXInt()]
		public virtual Int32? ShipToBAccountID
		{
			get
			{
				return this._ShipToBAccountID;
			}
			set
			{
				this._ShipToBAccountID = value;
			}
		}
		#endregion
		#region ShipToLocationID
		public abstract class shiptToLocationID : PX.Data.IBqlField
		{
		}
		protected Int32? _ShipToLocationID;
		[PXInt()]
		public virtual Int32? ShipToLocationID
		{
			get
			{
				return this._ShipToLocationID;
			}
			set
			{
				this._ShipToLocationID = value;
			}
		}
		#endregion
		#region OrderDate
		public abstract class orderDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _OrderDate;
		[PXDBDate()]
		[PXDBDefault(typeof(POOrder.orderDate))]
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
		#region SubItemID
		public abstract class subItemID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubItemID;
		[PXDefault(typeof(Search<InventoryItem.defaultSubItemID,
			Where<InventoryItem.inventoryID, Equal<Current<POLine.inventoryID>>,
			And<InventoryItem.defaultSubItemOnEntry, Equal<boolTrue>>>>),
			PersistingCheck = PXPersistingCheck.Nothing)]
		[SubItem(typeof(POLine.inventoryID))]
		//[SubItemStatusVeryfier(typeof(POLine.inventoryID), typeof(POLine.siteID), InventoryItemStatus.Inactive, InventoryItemStatus.NoPurchases)]
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
		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SiteID;

		[POSiteAvail(typeof(POLine.inventoryID), typeof(POLine.subItemID))]
		[PXDefault(typeof(Coalesce<Search<Location.vSiteID,
			Where<Location.locationID, Equal<Current<POOrder.vendorLocationID>>,
				And<Location.bAccountID, Equal<Current<POOrder.vendorID>>>>>,
					Search<InventoryItem.dfltSiteID,Where<InventoryItem.inventoryID,Equal<Current<POLine.inventoryID>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
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
		#region LotSerialNbr
		public abstract class lotSerialNbr : PX.Data.IBqlField
		{
		}
		protected String _LotSerialNbr;
		[PXDBString(100, IsUnicode = true)]
		[PXUIField(DisplayName = "Lot Serial Number",Visible=false )]
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

		#region BLType
		public abstract class bLType : PX.Data.IBqlField
		{
		}
		protected String _BLType;
		[PXDBString(2, IsFixed = true)]
		public virtual String BLType
		{
			get
			{
				return this._BLType;
			}
			set
			{
				this._BLType = value;
			}
		}
		#endregion
		#region BLOrderNbr
		public abstract class bLOrderNbr : PX.Data.IBqlField
		{
		}
		protected String _BLOrderNbr;
		[PXDBString(15, IsUnicode = true)]
		public virtual String BLOrderNbr
		{
			get
			{
				return this._BLOrderNbr;
			}
			set
			{
				this._BLOrderNbr = value;
			}
		}
		#endregion
		#region BLLineNbr
		public abstract class bLLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _BLLineNbr;
		[PXDBInt()]
		public virtual Int32? BLLineNbr
		{
			get
			{
				return this._BLLineNbr;
			}
			set
			{
				this._BLLineNbr = value;
			}
		}
		#endregion

		#region RQReqNbr
		public abstract class rQReqNbr : PX.Data.IBqlField
		{
		}
		protected String _RQReqNbr;
		[PXDBString(15, IsUnicode = true)]
		public virtual String RQReqNbr
		{
			get
			{
				return this._RQReqNbr;
			}
			set
			{
				this._RQReqNbr = value;
			}
		}
		#endregion
		#region RQReqLineNbr
		public abstract class rQReqLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _RQReqLineNbr;
		[PXDBInt()]
		public virtual Int32? RQReqLineNbr
		{
			get
			{
				return this._RQReqLineNbr;
			}
			set
			{
				this._RQReqLineNbr = value;
			}
		}
		#endregion
		#region UOM
		public abstract class uOM : PX.Data.IBqlField
		{
		}
		protected String _UOM;

		[INUnit(typeof(POLine.inventoryID),DisplayName="UOM")]
        [UOMDefault(INUOMDefaulType.Purchase, typeof(POLine.inventoryID), typeof(POLine.siteID), PersistingCheck = PXPersistingCheck.Nothing)]
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
		#region ClosedQty
		public abstract class closedQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _ClosedQty;
		[PXDBCalced(typeof(Sub<POLine.orderQty, POLine.openQty>), typeof(decimal))]
		[PXQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? ClosedQty
		{
			get
			{
				return this._ClosedQty;
			}
			set
			{
				this._ClosedQty = value;
			}
		}
		#endregion
		#region OrderQty
		public abstract class orderQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _OrderQty;
		[PXDBQuantity(typeof(POLine.uOM), typeof(POLine.baseOrderQty),HandleEmptyKey = true,MinValue = 0)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXFormula(null, typeof(SumCalc<POOrder.orderQty>))] //		
		[PXUIField(DisplayName = "Order Qty.", Visibility = PXUIVisibility.Visible)]
		public virtual Decimal? OrderQty
		{
			get
			{
				return this._OrderQty;
			}
			set
			{
				this._OrderQty = value;
			}
		}
		#endregion
		#region BaseOrderQty
		public abstract class baseOrderQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseOrderQty;
		[PXUIField(DisplayName = "Base Order Qty.")]
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXFormula(null, typeof(SumCalc<POLineR.baseReceivedQty>))] //
		public virtual Decimal? BaseOrderQty
		{
			get
			{
				return this._BaseOrderQty;
			}
			set
			{
				this._BaseOrderQty = value;
			}
		}
		#endregion
		#region ReceivedQty
		public abstract class receivedQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _ReceivedQty;
		[PXDBQuantity(typeof(POLine.uOM), typeof(POLine.baseReceivedQty),HandleEmptyKey = true)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. On Receipts", Visibility = PXUIVisibility.Visible, Enabled = false)]
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
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Base Received Qty.", Visibility = PXUIVisibility.Visible)]
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
		#region CuryClosedAmt
		public abstract class curyClosedAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryClosedAmt;
		[PXDBCalced(typeof(Sub<POLine.curyExtCost, POLine.curyOpenAmt>), typeof(decimal))]
		[PXQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryClosedAmt
		{
			get
			{
				return this._CuryClosedAmt;
			}
			set
			{
				this._CuryClosedAmt = value;
			}
		}
		#endregion
		#region CuryInfoID
		public abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _CuryInfoID;
		
		[PXDBLong()]
		[CurrencyInfo(typeof(POOrder.curyInfoID))]
		public virtual Int64? CuryInfoID
		{
			get
			{
				return this._CuryInfoID;
			}
			set
			{
				this._CuryInfoID = value;
			}
		}
		#endregion
		#region CuryUnitCost
		public abstract class curyUnitCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryUnitCost;

		[PXDBCurrency(typeof(Search<CommonSetup.decPlPrcCst>), typeof(POLine.curyInfoID), typeof(POLine.unitCost))]
		[PXUIField(DisplayName = "Unit Cost", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryUnitCost
		{
			get
			{
				return this._CuryUnitCost;
			}
			set
			{
				this._CuryUnitCost = value;
			}
		}
		#endregion
		#region UnitCost
		public abstract class unitCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnitCost;

		[PXDBPriceCost()]
		[PXDefault(typeof(Search<INItemCost.lastCost, Where<INItemCost.inventoryID, Equal<Current<POLine.inventoryID>>>>))]
		public virtual Decimal? UnitCost
		{
			get
			{
				return this._UnitCost;
			}
			set
			{
				this._UnitCost = value;
			}
		}
		#endregion

        #region DiscPct
        public abstract class discPct : PX.Data.IBqlField
        {
        }
        protected Decimal? _DiscPct;
        [PXDBDecimal(6, MinValue = -100, MaxValue = 100)]
        [PXUIField(DisplayName = "Discount Percent")]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? DiscPct
        {
            get
            {
                return this._DiscPct;
            }
            set
            {
                this._DiscPct = value;
            }
        }
        #endregion
        #region CuryDiscAmt
        public abstract class curyDiscAmt : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryDiscAmt;
        [PXDBCurrency(typeof(POLine.curyInfoID), typeof(POLine.discAmt))]
        [PXUIField(DisplayName = "Discount Amount")]
        //[PXFormula(typeof(Div<Mult<Mult<SOLine.orderQty, SOLine.curyUnitPrice>, SOLine.discPct>, decimal100>))]->Causes SetValueExt for CuryDiscAmt 
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? CuryDiscAmt
        {
            get
            {
                return this._CuryDiscAmt;
            }
            set
            {
                this._CuryDiscAmt = value;
            }
        }
        #endregion
        #region DiscAmt
        public abstract class discAmt : PX.Data.IBqlField
        {
        }
        protected Decimal? _DiscAmt;
        [PXDBDecimal(4)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? DiscAmt
        {
            get
            {
                return this._DiscAmt;
            }
            set
            {
                this._DiscAmt = value;
            }
        }
        #endregion
        #region ManualPrice
        public abstract class manualPrice : PX.Data.IBqlField
        {
        }
        protected Boolean? _ManualPrice;
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Manual Price", Visibility = PXUIVisibility.Visible)]
        public virtual Boolean? ManualPrice
        {
            get
            {
                return this._ManualPrice;
            }
            set
            {
                this._ManualPrice = value;
            }
        }
        #endregion
        #region ManualDisc
        public abstract class manualDisc : PX.Data.IBqlField
        {
        }
        protected Boolean? _ManualDisc;
        [AR.ManualDiscountMode(typeof(POLine.curyDiscAmt), typeof(POLine.curyExtCost), typeof(POLine.discPct))]
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Manual Discount", Visibility = PXUIVisibility.Visible)]
        public virtual Boolean? ManualDisc
        {
            get
            {
                return this._ManualDisc;
            }
            set
            {
                this._ManualDisc = value;
            }
        }
        #endregion
        #region CuryLineAmt
        public abstract class curyLineAmt : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryLineAmt;
        [PXDBCurrency(typeof(POLine.curyInfoID), typeof(POLine.lineAmt))]
        [PXUIField(DisplayName = "Ext. Cost")]
        [PXFormula(typeof(Mult<POLine.orderQty, POLine.curyUnitCost>))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? CuryLineAmt
        {
            get
            {
                return this._CuryLineAmt;
            }
            set
            {
                this._CuryLineAmt = value;
            }
        }
        #endregion
        #region LineAmt
        public abstract class lineAmt : PX.Data.IBqlField
        {
        }
        protected Decimal? _LineAmt;
        [PXDBDecimal(4)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? LineAmt
        {
            get
            {
                return this._LineAmt;
            }
            set
            {
                this._LineAmt = value;
            }
        }
        #endregion
        #region CuryDiscCost
        public abstract class curyDiscCost : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryDiscCost;
        [PXDBPriceCostCalced(typeof(Switch<Case<Where<POLine.orderQty, Equal<decimal0>>, decimal0>, Div<Sub<POLine.curyLineAmt, POLine.curyDiscAmt>, POLine.orderQty>>), typeof(Decimal))]
        [PXFormula(typeof(Div<Sub<POLine.curyLineAmt, POLine.curyDiscAmt>, NullIf<POLine.orderQty, decimal0>>))]
		[PXCurrency(typeof(Search<CommonSetup.decPlPrcCst>), typeof(POLine.curyInfoID), typeof(POLine.discCost))]
        [PXUIField(DisplayName = "Disc. Unit Cost", Enabled = false, Visible = false)]
        public virtual Decimal? CuryDiscCost
        {
            get
            {
                return this._CuryDiscCost;
            }
            set
            {
                this._CuryDiscCost = value;
            }
        }
        #endregion
        #region DiscCost
        public abstract class discCost : PX.Data.IBqlField
        {
        }
        protected Decimal? _DiscCost;
		[PXDBPriceCostCalced(typeof(Switch<Case<Where<POLine.orderQty, Equal<decimal0>>, decimal0>, Div<POLine.lineAmt, POLine.orderQty>>), typeof(Decimal), CastToScale = 9, CastToPrecision = 25)]
        [PXFormula(typeof(Div<Row<POLine.lineAmt, POLine.curyLineAmt>, NullIf<POLine.orderQty, decimal0>>))]
        public virtual Decimal? DiscCost
        {
            get
            {
                return this._DiscCost;
            }
            set
            {
                this._DiscCost = value;
            }
        }
        #endregion

		#region CuryExtCost
		public abstract class curyExtCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryExtCost;
		[PXDBCurrency(typeof(POLine.curyInfoID), typeof(POLine.extCost),MinValue=0.0)]
		[PXUIField(DisplayName = "Amount", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
        [PXFormula(typeof(Sub<POLine.curyLineAmt, POLine.curyDiscAmt>))]
		[PXFormula(null, typeof(SumCalc<POLineR.curyReceivedCost>))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryExtCost
		{
			get
			{
				return this._CuryExtCost;
			}
			set
			{
				this._CuryExtCost = value;
			}
		}
		#endregion
		#region ExtCost
		public abstract class extCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _ExtCost;

		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount")]
		public virtual Decimal? ExtCost
		{
			get
			{
				return this._ExtCost;
			}
			set
			{
				this._ExtCost = value;
			}
		}
		#endregion
        #region GroupDiscountRate
        public abstract class groupDiscountRate : PX.Data.IBqlField
        {
        }
        protected Decimal? _GroupDiscountRate;
        [PXDBDecimal(6)]
        [PXDefault(TypeCode.Decimal, "1.0")]
        public virtual Decimal? GroupDiscountRate
        {
            get
            {
                return this._GroupDiscountRate;
            }
            set
            {
                this._GroupDiscountRate = value;
            }
        }
        #endregion
        #region DocumentDiscountRate
        public abstract class documentDiscountRate : PX.Data.IBqlField
        {
        }
        protected Decimal? _DocumentDiscountRate;
        [PXDBDecimal(6)]
        [PXDefault(TypeCode.Decimal, "1.0")]
        public virtual Decimal? DocumentDiscountRate
        {
            get
            {
                return this._DocumentDiscountRate;
            }
            set
            {
                this._DocumentDiscountRate = value;
            }
        }
        #endregion
		#region TaxCategoryID
		public abstract class taxCategoryID : PX.Data.IBqlField
		{
		}
		protected String _TaxCategoryID;
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Tax Category", Visibility = PXUIVisibility.Visible)]
		[POTax(typeof(POOrder), typeof(POTax), typeof(POTaxTran))]
		[POOpenTax(typeof(POOrder), typeof(POTax), typeof(POTaxTran))]
        [PXSelector(typeof(TaxCategory.taxCategoryID), DescriptionField = typeof(TaxCategory.descr))]
        [PXRestrictor(typeof(Where<TaxCategory.active, Equal<True>>), TX.Messages.InactiveTaxCategory, typeof(TaxCategory.taxCategoryID))]
        [PXDefault(typeof(Search<InventoryItem.taxCategoryID,
			Where<InventoryItem.inventoryID, Equal<Current<POLine.inventoryID>>>>),
			PersistingCheck = PXPersistingCheck.Nothing)]
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
		#region TaxID
		public abstract class taxID : PX.Data.IBqlField
		{
		}
		protected String _TaxID;
		[PXDBString(Tax.taxID.Length, IsUnicode = true)]
		[PXUIField(DisplayName = "Tax ID", Visible = false)]
		[PXSelector(typeof(Tax.taxID), DescriptionField = typeof(Tax.descr))]
		public virtual String TaxID
		{
			get
			{
				return this._TaxID;
			}
			set
			{
				this._TaxID = value;
			}
		}
		#endregion		
		#region ExpenseAcctID
		public abstract class expenseAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _ExpenseAcctID;
		[Account(typeof(POLine.branchID),DisplayName = "Account", Visibility = PXUIVisibility.Visible, Filterable = false, DescriptionField = typeof(Account.description))]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Int32? ExpenseAcctID
		{
			get
			{
				return this._ExpenseAcctID;
			}
			set
			{
				this._ExpenseAcctID = value;
			}
		}
		#endregion
		#region ExpenseSubID
		public abstract class expenseSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _ExpenseSubID;
		[SubAccount(typeof(POLine.expenseAcctID), typeof(POLine.branchID), DisplayName = "Sub.", Visibility = PXUIVisibility.Visible, Filterable = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Int32? ExpenseSubID
		{
			get
			{
				return this._ExpenseSubID;
			}
			set
			{
				this._ExpenseSubID = value;
			}
		}
		#endregion
		#region POAccrualAcctID
		int? IAPTranSource.POAccrualAcctID
		{
			get
			{
				return null;
			}
		}
		#endregion
		#region POAccrualSubID
		int? IAPTranSource.POAccrualSubID
		{
			get
			{
				return null;
			}
		}
		#endregion
		#region AlternateID
		public abstract class alternateID : PX.Data.IBqlField
		{
		}
		protected String _AlternateID;		
		[AlternativeItem(INPrimaryAlternateType.VPN, typeof(POLine.vendorID), typeof(POLine.inventoryID), typeof(POLine.subItemID))]
		public virtual String AlternateID
		{
			get
			{
				return this._AlternateID;
			}
			set
			{
				this._AlternateID = value;
			}
		}
		#endregion
		#region TranDesc
		public abstract class tranDesc : PX.Data.IBqlField
		{
		}
		protected String _TranDesc;
		[PXDBString(256, IsUnicode = true)]
		[PXUIField(DisplayName = "Line Description", Visibility = PXUIVisibility.Visible)]
		public virtual String TranDesc
		{
			get
			{
				return this._TranDesc;
			}
			set
			{
				this._TranDesc = value;
			}
		}
		#endregion
		#region UnitWeight
		public abstract class unitWeight : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnitWeight;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0", typeof(Search<InventoryItem.baseWeight, Where<InventoryItem.inventoryID, Equal<Current<POLine.inventoryID>>, And<InventoryItem.baseWeight, IsNotNull>>>))]
		[PXUIField(DisplayName="Unit Weight")]
		public virtual Decimal? UnitWeight
		{
			get
			{
				return this._UnitWeight;
			}
			set
			{
				this._UnitWeight = value;
			}
		}
		#endregion
		#region UnitVolume
		public abstract class unitVolume : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnitVolume;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0", typeof(Search<InventoryItem.baseVolume, Where<InventoryItem.inventoryID, Equal<Current<POLine.inventoryID>>, And<InventoryItem.baseVolume, IsNotNull>>>))]
		[PXUIField(DisplayName = "Unit Volume")]
		public virtual Decimal? UnitVolume
		{
			get
			{
				return this._UnitVolume;
			}
			set
			{
				this._UnitVolume = value;
			}
		}
		#endregion
		#region ExtWeight
		public abstract class extWeight : PX.Data.IBqlField
		{
		}
		protected Decimal? _ExtWeight;
		[PXDBDecimal(6)]
		[PXUIField(DisplayName = "Weight", Visibility = PXUIVisibility.SelectorVisible)]
		[PXFormula(typeof(Mult<Row<POLine.baseOrderQty, POLine.orderQty>, POLine.unitWeight>), typeof(SumCalc<POOrder.orderWeight>))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? ExtWeight
		{
			get
			{
				return this._ExtWeight;
			}
			set
			{
				this._ExtWeight = value;
			}
		}
		#endregion
		#region ExtVolume
		public abstract class extVolume : PX.Data.IBqlField
		{
		}
		protected Decimal? _ExtVolume;
		[PXDBDecimal(6)]
		[PXUIField(DisplayName = "Weight", Visibility = PXUIVisibility.SelectorVisible)]
		[PXFormula(typeof(Mult<Row<POLine.baseOrderQty, POLine.orderQty>, POLine.unitVolume>), typeof(SumCalc<POOrder.orderVolume>))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? ExtVolume
		{
			get
			{
				return this._ExtVolume;
			}
			set
			{
				this._ExtVolume = value;
			}
		}
		#endregion
		#region ProjectID
		public abstract class projectID : PX.Data.IBqlField
		{
		}
		protected Int32? _ProjectID;
		[POProjectDefault(typeof(POLine.lineType), AccountType=typeof(POLine.expenseAcctID))]
		[ActiveProjectForModule(BatchModule.PO)]
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
		[ActiveProjectTask(typeof(POLine.projectID), BatchModule.PO, DisplayName = "Project Task")]
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
		#region ReasonCode
		public abstract class reasonCode : PX.Data.IBqlField
		{
		}
		protected String _ReasonCode;
		[PXDBString(CS.ReasonCode.reasonCodeID.Length, IsUnicode = true)]
		public virtual String ReasonCode
		{
			get
			{
				return this._ReasonCode;
			}
			set
			{
				this._ReasonCode = value;
			}
		}
		#endregion
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;		
		[PXNote]
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
		#region RcptQtyMin
		public abstract class rcptQtyMin : PX.Data.IBqlField
		{
		}
		protected Decimal? _RcptQtyMin;
		[PXDBDecimal(2, MinValue = 0.0, MaxValue = 999.0)]
		[PXDefault(typeof(Search<Location.vRcptQtyMin, 
			Where<Location.locationID,Equal<Current<POOrder.vendorLocationID>>, 
			  And<Location.bAccountID, Equal<Current<POOrder.vendorID>>>>>))]
		[PXUIField(DisplayName = "Min. Receipt (%)")]
		public virtual Decimal? RcptQtyMin
		{
			get
			{
				return this._RcptQtyMin;
			}
			set
			{
				this._RcptQtyMin = value;
			}
		}
		#endregion
		#region RcptQtyMax
		public abstract class rcptQtyMax : PX.Data.IBqlField
		{
		}
		protected Decimal? _RcptQtyMax;
		[PXDBDecimal(2, MinValue = 0.0, MaxValue = 999.0)]
		[PXDefault(typeof(Search<Location.vRcptQtyMax, 
			Where<Location.locationID,Equal<Current<POOrder.vendorLocationID>>, 
			  And<Location.bAccountID, Equal<Current<POOrder.vendorID>>>>>))]
		[PXUIField(DisplayName = "Max. Receipt (%)")]
		public virtual Decimal? RcptQtyMax
		{
			get
			{
				return this._RcptQtyMax;
			}
			set
			{
				this._RcptQtyMax = value;
			}
		}
		#endregion
		#region RcptQtyThreshold
		public abstract class rcptQtyThreshold : PX.Data.IBqlField
		{
		}
		protected Decimal? _RcptQtyThreshold;
		[PXDBDecimal(2, MinValue = 0.0, MaxValue = 999.0)]
		[PXDefault(typeof(Search<Location.vRcptQtyThreshold,
			Where<Location.locationID, Equal<Current<POOrder.vendorLocationID>>,
				And<Location.bAccountID, Equal<Current<POOrder.vendorID>>>>>))]
		[PXUIField(DisplayName = "Complete On (%)")]
		public virtual Decimal? RcptQtyThreshold
		{
			get
			{
				return this._RcptQtyThreshold;
			}
			set
			{
				this._RcptQtyThreshold = value;
			}
		}
		#endregion
		#region RcptQtyAction
		public abstract class rcptQtyAction : PX.Data.IBqlField
		{
		}
		protected String _RcptQtyAction;
		[PXDBString(1, IsFixed = true)]		
		[POReceiptQtyAction.List()]
		[PXDefault(typeof(Search<Location.vRcptQtyAction, 
			Where<Location.locationID,Equal<Current<POOrder.vendorLocationID>>, 
			  And<Location.bAccountID, Equal<Current<POOrder.vendorID>>>>>))]
		[PXUIField(DisplayName="Receipt Action")]
		public virtual String RcptQtyAction
		{
			get
			{
				return this._RcptQtyAction;
			}
			set
			{
				this._RcptQtyAction = value;
			}
		}
		#endregion
		#region VoucheredQty
		public abstract class voucheredQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _VoucheredQty;
		[PXDBQuantity(typeof(POLine.uOM), typeof(POLine.baseVoucheredQty),HandleEmptyKey = true)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Billed Qty.", Visibility = PXUIVisibility.Visible)]
		public virtual Decimal? VoucheredQty
		{
			get
			{
				return this._VoucheredQty;
			}
			set
			{
				this._VoucheredQty = value;
			}
		}
		#endregion
		#region BaseVoucheredQty
		public abstract class baseVoucheredQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseVoucheredQty;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]		
		public virtual Decimal? BaseVoucheredQty
		{
			get
			{
				return this._BaseVoucheredQty;
			}
			set
			{
				this._BaseVoucheredQty = value;
			}
		}
		#endregion
		#region CuryVoucheredCost
		public abstract class curyVoucheredCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryVoucheredCost;

		[PXDBCurrency(typeof(POLine.curyInfoID), typeof(POLine.voucheredCost))]
		[PXUIField(DisplayName = "Billed Cost", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryVoucheredCost
		{
			get
			{
				return this._CuryVoucheredCost;
			}
			set
			{
				this._CuryVoucheredCost = value;
			}
		}
		#endregion
		#region VoucheredCost
		public abstract class voucheredCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _VoucheredCost;
		[PXDBDecimal(6)]
		[PXDefault(typeof(POLine.unitCost))]
		public virtual Decimal? VoucheredCost
		{
			get
			{
				return this._VoucheredCost;
			}
			set
			{
				this._VoucheredCost = value;
			}
		}
		#endregion
		#region CuryReceivedCost
		public abstract class curyReceivedCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryReceivedCost;

		[PXDBCurrency(typeof(POLine.curyInfoID), typeof(POLine.receivedCost))]
		[PXUIField(DisplayName = "Received Amt.", Visibility = PXUIVisibility.SelectorVisible,Enabled=false)]
		[PXDefault(TypeCode.Decimal, "0.0")]

		public virtual Decimal? CuryReceivedCost
		{
			get
			{
				return this._CuryReceivedCost;
			}
			set
			{
				this._CuryReceivedCost = value;
			}
		}
		#endregion
		#region ReceivedCost
		public abstract class receivedCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _ReceivedCost;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]		
		[PXUIField(DisplayName = "Received Cost")]
		public virtual Decimal? ReceivedCost
		{
			get
			{
				return this._ReceivedCost;
			}
			set
			{
				this._ReceivedCost = value;
			}
		}
		#endregion
		#region ReceiptStatus
		public abstract class receiptStatus : PX.Data.IBqlField
		{
		}
		protected String _ReceiptStatus;
		[PXDBString(1, IsFixed = true)]
		[PXDefault("")]
		public virtual String ReceiptStatus
		{
			get
			{
				return this._ReceiptStatus;
			}
			set
			{
				this._ReceiptStatus = value;
			}
		}
		#endregion
		#region VoucherStatus
		public abstract class voucherStatus : PX.Data.IBqlField
		{
		}
		protected String _VoucherStatus;
		[PXDBString(1, IsFixed = true)]
		[PXDefault("")]
		public virtual String VoucherStatus
		{
			get
			{
				return this._VoucherStatus;
			}
			set
			{
				this._VoucherStatus = value;
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
	
		#region OpenQty
		public abstract class openQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _OpenQty;
		[PXDBQuantity(typeof(POLine.uOM), typeof(POLine.baseOpenQty),HandleEmptyKey = true)]
		[PXFormula(typeof(Switch<Case<Where<POLine.completed, Equal<True>, Or<POLine.cancelled, Equal<True>>>, decimal0>, Sub<POLine.orderQty, POLine.closedQty>>), typeof(SumCalc<POOrder.openOrderQty>))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Open Qty.", Enabled = false)]
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
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
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
		#region CuryOpenAmt
		public abstract class curyOpenAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryOpenAmt;
		[PXDBCurrency(typeof(POLine.curyInfoID), typeof(POLine.openAmt))]
		[PXFormula(typeof(Switch<Case<Where<POLine.completed, Equal<True>, Or<POLine.cancelled, Equal<True>>>, decimal0>, Sub<POLine.curyExtCost, POLine.curyClosedAmt>>))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Open Amount")]
		public virtual Decimal? CuryOpenAmt
		{
			get
			{
				return this._CuryOpenAmt;
			}
			set
			{
				this._CuryOpenAmt = value;
			}
		}
		#endregion
		#region OpenAmt
		public abstract class openAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _OpenAmt;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Open Amount")]
		public virtual Decimal? OpenAmt
		{
			get
			{
				return this._OpenAmt;
			}
			set
			{
				this._OpenAmt = value;
			}
		}
		#endregion

		#region LeftToReceiveQty
		public abstract class leftToReceiveQty : PX.Data.IBqlField
		{
		}
		[PXQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Open Qty.", Visibility = PXUIVisibility.Invisible)]
		public virtual Decimal? LeftToReceiveQty
		{
			[PXDependsOnFields(typeof(orderQty),typeof(receivedQty))]
			get
			{
				return (this._OrderQty - this._ReceivedQty);
			}
		}
#endregion
		#region LeftToReceiveBaseQty
		public abstract class leftToReceiveBaseQty : PX.Data.IBqlField
		{
		}

		[PXDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? LeftToReceiveBaseQty
		{
			[PXDependsOnFields(typeof(baseOrderQty),typeof(baseReceivedQty))]
			get
			{
				return (this._BaseOrderQty - this._BaseReceivedQty);
			}
		}
		#endregion

		#region RequestedDate
		public abstract class requestedDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _RequestedDate;
		[PXDBDate()]
		[PXDefault(typeof(POOrder.orderDate), PersistingCheck= PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Requested")]
		public virtual DateTime? RequestedDate
		{
			get
			{
				return this._RequestedDate;
			}
			set
			{
				this._RequestedDate = value;
			}
		}
			#endregion
		#region PromisedDate
		public abstract class promisedDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _PromisedDate;
		[PXDBDate()]
		[PXDefault(typeof(POOrder.expectedDate), PersistingCheck= PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Promised")]
		public virtual DateTime? PromisedDate
		{
			get
			{
				return this._PromisedDate;
			}
			set
			{
				this._PromisedDate = value;
			}
		}
		#endregion
		#region Cancelled
		public abstract class cancelled : PX.Data.IBqlField
		{
		}
		protected Boolean? _Cancelled;
		[PXDBBool()]
		[PXUIField(DisplayName = "Cancelled", Visibility = PXUIVisibility.Visible)]
		[PXDefault(false)]
		public virtual Boolean? Cancelled
		{
			get
			{
				return this._Cancelled;
			}
			set
			{
				this._Cancelled = value;				
			}
		}
		#endregion
		#region Completed
		public abstract class completed : PX.Data.IBqlField
		{
		}
		protected Boolean? _Completed;
		[PXDBBool()]
		[PXUIField(DisplayName = "Completed", Visibility = PXUIVisibility.Visible)]
		[PXDefault(false)]
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

		#region AllowComplete
		public abstract class allowComplete: PX.Data.IBqlField
		{
		}
		protected Boolean? _AllowComplete;
		[PXDBBool()]
		[PXUIField(DisplayName = "Allow Complete", Visibility = PXUIVisibility.Visible)]
		[PXDefault(false)]
		public virtual Boolean? AllowComplete
		{
			get
			{
				return this._AllowComplete;
			}
			set
			{
				this._AllowComplete = value;
			}
		}
		#endregion
		#region IsStockItem
		public abstract class isStockItem : PX.Data.IBqlField
		{
		}
		[PXBool]
		[PXUIField(DisplayName = "Is stock", Visibility = PXUIVisibility.Invisible, Visible = false, Enabled = false)]
		[PXFormula(typeof(Selector<POLine.inventoryID, InventoryItem.stkItem>))]
		public virtual Boolean? IsStockItem
		{
			get;
			set;
		}
		#endregion
		#region IsKit
		public abstract class isKit : PX.Data.IBqlField
		{
		}
		[PXBool]
		[PXUIField(DisplayName = "Is a Kit", Visibility = PXUIVisibility.Invisible, Visible = false, Enabled = false)]
		[PXFormula(typeof(Selector<POLine.inventoryID, InventoryItem.kitItem>))]
		public virtual Boolean? IsKit
		{
			get;
			set;
		}
		#endregion

        #region DiscountID
        public abstract class discountID : PX.Data.IBqlField
        {
        }
        protected String _DiscountID;
        [PXDBString(10, IsUnicode = true)]
        [PXSelector(typeof(Search<APDiscount.discountID, Where<APDiscount.bAccountID, Equal<Current<POLine.vendorID>>, And<APDiscount.type, Equal<AR.DiscountType.LineDiscount>>>>))]
        [PXUIField(DisplayName = "Discount Code", Visible = true, Enabled = false)]
        public virtual String DiscountID
        {
            get
            {
                return this._DiscountID;
            }
            set
            {
                this._DiscountID = value;
            }
        }
        #endregion
        #region DiscountSequenceID
        public abstract class discountSequenceID : PX.Data.IBqlField
        {
        }
        protected String _DiscountSequenceID;
        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "Discount Sequence", Visible = false, Enabled = false)]
        public virtual String DiscountSequenceID
        {
            get
            {
                return this._DiscountSequenceID;
            }
            set
            {
                this._DiscountSequenceID = value;
            }
        }
        #endregion

		#region OrderedQtyAltered
		public abstract class orderedQtyAltered : PX.Data.IBqlField
		{
		}
		[PXBool]
		[PXFormula(typeof(False))]
		public virtual bool? OrderedQtyAltered
		{
			get; set;
		}
		#endregion

		#region IAPTranSource Members

		Decimal? IAPTranSource.BillQty
		{
			[PXDependsOnFields(typeof(orderQty),typeof(voucheredQty))]
			get
			{
				return (this._OrderQty - this.VoucheredQty);
			}
		}
		Decimal? IAPTranSource.CuryLineAmt
		{
			[PXDependsOnFields(typeof(curyLineAmt),typeof(curyVoucheredCost))]
			get
			{
				return (this._CuryLineAmt - this._CuryVoucheredCost);
			}
		}

		public virtual bool CompareReferenceKey(APTran aTran)
		{
			return (aTran.PONbr == this.OrderNbr && aTran.POOrderType == this.OrderType && aTran.POLineNbr == this.LineNbr);
		}

		public virtual void SetReferenceKeyTo(APTran aTran)
		{
			aTran.PONbr = this.OrderNbr;
			aTran.POOrderType = this.OrderType;
			aTran.POLineNbr = this.LineNbr;
		}

		public virtual bool IsReturn
		{
			get { return false; }
		}

		#endregion
	}

	
	public static class POLineType
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { GoodsForInventory, GoodsForSalesOrder, GoodsForReplenishment, GoodsForDropShip, NonStockForDropShip, NonStockForSalesOrder, NonStock, Service, Freight, Description },
				new string[] { Messages.GoodsForInventory, Messages.GoodsForSalesOrder, Messages.GoodsForReplenishment, Messages.GoodsForDropShip, Messages.NonStockForDropShip, Messages.NonStockForSalesOrder, Messages.NonStockItem, Messages.Service, Messages.Freight, Messages.Description }) { }
		}

        /// <summary>
        /// Selector. Provides a Default List of PO Line Types <br/>
        /// i.e. GoodsForInventory, NonStock, Service 
        /// </summary>
		public class DefaultListAttribute : PXStringListAttribute
		{
			public DefaultListAttribute()
				: base(
				new string[] { GoodsForInventory, NonStock, Service },
				new string[] { Messages.GoodsForInventory, Messages.NonStockItem, Messages.Service }) { }
		}		
		
		public const string GoodsForInventory = "GI";
		public const string GoodsForSalesOrder = "GS";
		public const string GoodsForReplenishment = "GR";
		public const string GoodsForDropShip = "GP";
		public const string NonStockForDropShip = "NP";
		public const string NonStockForSalesOrder = "NO";
		public const string NonStock = "NS";
		public const string Service = "SV";
		public const string Freight = "FT";
		public const string MiscCharges = "MC";
		public const string Description = "DN";
		


		public class goodsForInventory : Constant<string>
		{
			public goodsForInventory() : base(GoodsForInventory) { ;}
		}

		public class goodsForSalesOrder : Constant<string>
		{
			public goodsForSalesOrder() : base(GoodsForSalesOrder) { ;}
		}
		public class goodsForReplenishment : Constant<string>
		{
			public goodsForReplenishment() : base(GoodsForReplenishment) { ;}
		}

		public class goodsForDropShip : Constant<string>
		{
			public goodsForDropShip() : base(GoodsForDropShip) { ;}
		}

		public class nonStockForDropShip : Constant<string>
		{
			public nonStockForDropShip() : base(NonStockForDropShip) { ;}
		}
		public class nonStockForSalesOrder : Constant<string>
		{
			public nonStockForSalesOrder() : base(NonStockForSalesOrder) { ;}
		}

		public class nonStock: Constant<string>
		{
			public nonStock() : base(NonStock) { ;}
		}

		public class service : Constant<string>
		{
			public service() : base(Service) { ;}
		}

		public class freight : Constant<string>
		{
			public freight() : base(Freight) { ;}
		}

		public class miscCharges: Constant<string>
		{
			public miscCharges() : base(MiscCharges) { ;}
		}

		public class description : Constant<string>
		{
			public description() : base(Description) { ;}
		}

		public static bool IsStock(string lineType)
		{
			return
				lineType == POLineType.GoodsForInventory ||
				lineType == POLineType.GoodsForDropShip ||
				lineType == POLineType.GoodsForSalesOrder ||
				lineType == POLineType.GoodsForReplenishment;
		}
		public static bool IsNonStock(string lineType)
		{
			return
				lineType == POLineType.NonStock ||
				lineType == POLineType.NonStockForDropShip ||
				lineType == POLineType.NonStockForSalesOrder ||
				lineType == POLineType.Service;
		}
		public static bool IsService(string lineType)
		{
			return
				lineType == POLineType.Service;
		}
		public static bool IsDefault(string lineType)
		{
			return
				lineType == POLineType.GoodsForInventory ||
				lineType == POLineType.NonStock ||
				lineType == POLineType.Service ||
				lineType == POLineType.Freight ||
				lineType == POLineType.Description;
		}

	}

	public class POReceiptQtyAction
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Reject, AcceptButWarn, Accept },
				new string[] { Messages.Reject, Messages.AcceptButWarn, Messages.Accept }) { }
		}

		
		public const string Accept = "A";
		public const string AcceptButWarn = "W";
		public const string Reject = "R";
	}

	//This class is used for reports
    [System.SerializableAttribute()]
    [PXHidden]
	public partial class POLineFilter : PX.Data.IBqlTable 
	{
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[Inventory(Filterable = true)]
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
		#region SubItemID
		public abstract class subItemID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubItemID;
		[SubItem(typeof(POLineFilter.inventoryID))]
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
		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SiteID;
		[Site()]
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
	}


	[PXProjection(typeof(Select<APTran,Where<APTran.receiptNbr,IsNull,And<APTran.pONbr,IsNotNull>>>), Persistent = false)]
    [Serializable]
	[PXCacheName(AP.Messages.APTran)]
	public partial class APTranPOLinked : APTran
	{
	}

}
