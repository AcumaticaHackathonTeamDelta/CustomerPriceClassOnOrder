namespace PX.Objects.IN
{
    using System;
    using PX.Data;
    using PX.Objects.CS;
    using PX.Objects.GL;
    using PX.Objects.CM;
    using PX.Objects.PO;
    using PX.Objects.SO;
    using PX.Objects.PM;
    using System.Collections.Generic;
    [System.SerializableAttribute()]
    [PXCacheName(Messages.INTran)]
	public partial class INTran : PX.Data.IBqlTable, ILSPrimary 
	{
        #region Selected
        public abstract class selected : PX.Data.IBqlField
        {
        }
        protected bool? _Selected;
        [PXBool]
        [PXFormula(typeof(False))]
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
		[Branch(typeof(INRegister.branchID))]
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
		#region DocType
		public abstract class docType : PX.Data.IBqlField
		{
		}
		protected String _DocType;
		[PXUIField(DisplayName = INRegister.docType.DisplayName)]
		[PXDBString(1, IsFixed = true, IsKey = true)]
		[PXDefault(typeof(INRegister.docType))]
		public virtual String DocType
		{
			get
			{
				return this._DocType;
			}
			set
			{
				this._DocType = value;
			}
		}
		#endregion
        #region OrigModule
        public abstract class origModule : PX.Data.IBqlField
        {
        }
        protected String _OrigModule;
        [PXString(2)]       
        public virtual String OrigModule
        {
            get
            {
                return this._OrigModule;
            }
            set
            {
                this._OrigModule = value;
            }
        }
        #endregion
		#region TranType
		public abstract class tranType : PX.Data.IBqlField
		{
		}
		protected String _TranType;
		[PXDBString(3, IsFixed = true)]
		[PXDefault()]
		[INTranType.List()]
		[PXUIField(DisplayName="Tran. Type")]
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
		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
		}
		protected String _RefNbr;
		[PXDBString(15, IsUnicode = true, IsKey = true)]
		[PXDBDefault(typeof(INRegister.refNbr))]
		[PXParent(typeof(Select<INRegister, Where<INRegister.docType, Equal<Current<INTran.docType>>, And<INRegister.refNbr,Equal<Current<INTran.refNbr>>>>>))]
		[PXUIField(DisplayName = INRegister.refNbr.DisplayName)]
		public virtual String RefNbr
		{
			get
			{
				return this._RefNbr;
			}
			set
			{
				this._RefNbr = value;
			}
		}
		#endregion
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineNbr;
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		[PXLineNbr(typeof(INRegister.lineCntr))]
		[PXUIField(DisplayName = "Line Number")]
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
		#region TranDate
		public abstract class tranDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _TranDate;
		[PXDBDate()]
		[PXDBDefault(typeof(INRegister.tranDate))]
		public virtual DateTime? TranDate
		{
			get
			{
				return this._TranDate;
			}
			set
			{
				this._TranDate = value;
			}
		}
		#endregion
		#region InvtMult
		public abstract class invtMult : PX.Data.IBqlField
		{
		}
		protected Int16? _InvtMult;
		[PXDBShort()]
		[PXDefault()]
		[PXUIField(DisplayName = "Multiplier")]
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
		[PXDefault()]
		[StockItem(DisplayName="Inventory ID")]
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
		[PXDefault(typeof(Search<InventoryItem.defaultSubItemID,
			Where<InventoryItem.inventoryID, Equal<Current<INTran.inventoryID>>,
			And<InventoryItem.defaultSubItemOnEntry, Equal<boolTrue>>>>),
			PersistingCheck = PXPersistingCheck.Nothing)]
		[IN.SubItem(
			typeof(INTran.inventoryID),
			typeof(LeftJoin<INSiteStatus,
				On<INSiteStatus.subItemID, Equal<INSubItem.subItemID>,
				And<INSiteStatus.inventoryID, Equal<Optional<INTran.inventoryID>>,
				And<INSiteStatus.siteID, Equal<Optional<INTran.siteID>>>>>>))]
		[PXFormula(typeof(Default<INTran.inventoryID>))]
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
		[IN.SiteAvail(typeof(INTran.inventoryID), typeof(INTran.subItemID))]
		[PXDefault(typeof(INRegister.siteID))]
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
		[IN.LocationAvail(typeof(INTran.inventoryID), typeof(INTran.subItemID), typeof(INTran.siteID), typeof(INTran.tranType), typeof(INTran.invtMult))]
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
		#region BAccountID
		public abstract class bAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _BAccountID;
		[PXDBInt]
		public virtual Int32? BAccountID
		{
			get
			{
				return this._BAccountID;
			}
			set
			{
				this._BAccountID = value;
			}
		}
		#endregion
		#region OrigBranchID
		public abstract class origBranchID : PX.Data.IBqlField
		{
		}
		protected Int32? _OrigBranchID;
		[PXDBInt()]
		public virtual Int32? OrigBranchID
		{
			get
			{
				return this._OrigBranchID;
			}
			set
			{
				this._OrigBranchID = value;
			}
		}
		#endregion
		#region OrigTranType
		public abstract class origTranType : PX.Data.IBqlField
		{
		}
		protected String _OrigTranType;
		[PXDBString(3, IsFixed = true)]
		public virtual String OrigTranType
		{
			get
			{
				return this._OrigTranType;
			}
			set
			{
				this._OrigTranType = value;
			}
		}
		#endregion
		#region OrigRefNbr
		public abstract class origRefNbr : PX.Data.IBqlField
		{
		}
		protected String _OrigRefNbr;
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Receipt Nbr.")]
		[PXVerifySelector(typeof(Search2<INCostStatus.receiptNbr,
			InnerJoin<INCostSubItemXRef, On<INCostSubItemXRef.costSubItemID, Equal<INCostStatus.costSubItemID>>,
			InnerJoin<INLocation, On<INLocation.locationID, Equal<Optional<INTran.locationID>>>>>,
			Where<INCostStatus.inventoryID, Equal<Optional<INTran.inventoryID>>,
			And<INCostSubItemXRef.subItemID, Equal<Optional<INTran.subItemID>>,
			And<
			Where<INCostStatus.costSiteID, Equal<Optional<INTran.siteID>>, And<INLocation.isCosted, Equal<boolFalse>,
			Or<INCostStatus.costSiteID, Equal<Optional<INTran.locationID>>>>>>>>>), VerifyField = false)]
		public virtual String OrigRefNbr
		{
			get
			{
				return this._OrigRefNbr;
			}
			set
			{
				this._OrigRefNbr = value;
			}
		}
		#endregion
		#region OrigLineNbr
		public abstract class origLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _OrigLineNbr;
		[PXDBInt()]
		public virtual Int32? OrigLineNbr
		{
			get
			{
				return this._OrigLineNbr;
			}
			set
			{
				this._OrigLineNbr = value;
			}
		}
		#endregion
		#region AcctID
		public abstract class acctID : PX.Data.IBqlField
		{
		}
		protected Int32? _AcctID;
		[Account()]
		public virtual Int32? AcctID
		{
			get
			{
				return this._AcctID;
			}
			set
			{
				this._AcctID = value;
			}
		}
		#endregion
		#region SubID
		public abstract class subID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubID;
		[SubAccount(typeof(INTran.acctID))]
		public virtual Int32? SubID
		{
			get
			{
				return this._SubID;
			}
			set
			{
				this._SubID = value;
			}
		}
		#endregion
		#region ReclassificationProhibited
		public abstract class reclassificationProhibited : PX.Data.IBqlField
		{
		}
		protected Boolean? _ReclassificationProhibited;

		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? ReclassificationProhibited
		{
			get
			{
				return this._ReclassificationProhibited;
			}
			set
			{
				this._ReclassificationProhibited = value;
			}
		}
		#endregion
		#region InvtAcctID
		public abstract class invtAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _InvtAcctID;
		[Account()]
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
		[SubAccount(typeof(INTran.invtAcctID))]
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
		#region COGSAcctID
		public abstract class cOGSAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _COGSAcctID;
		[Account()]
		public virtual Int32? COGSAcctID
		{
			get
			{
				return this._COGSAcctID;
			}
			set
			{
				this._COGSAcctID = value;
			}
		}
		#endregion
		#region COGSSubID
		public abstract class cOGSSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _COGSSubID;
		[SubAccount(typeof(INTran.cOGSAcctID))]
		public virtual Int32? COGSSubID
		{
			get
			{
				return this._COGSSubID;
			}
			set
			{
				this._COGSSubID = value;
			}
		}
		#endregion
		#region ToSiteID
		public abstract class toSiteID : PX.Data.IBqlField
		{
		}
		protected Int32? _ToSiteID;
		[IN.ToSite()]
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
		#region ToLocationID
		public abstract class toLocationID : PX.Data.IBqlField
		{
		}
		protected Int32? _ToLocationID;
		[IN.LocationAvail(typeof(INTran.inventoryID), typeof(INTran.subItemID), typeof(INTran.toSiteID), false, false, true, DisplayName = "To Location ID")]
		public virtual Int32? ToLocationID
		{
			get
			{
				return this._ToLocationID;
			}
			set
			{
				this._ToLocationID = value;
			}
		}
		#endregion
		#region LotSerialNbr
		public abstract class lotSerialNbr : PX.Data.IBqlField
		{
		}
		protected String _LotSerialNbr;
		[INLotSerialNbr(typeof(INTran.inventoryID), typeof(INTran.subItemID), typeof(INTran.locationID), PersistingCheck = PXPersistingCheck.Nothing, FieldClass = "LotSerial")]
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
		#region ExpireDate
		public abstract class expireDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ExpireDate;
		[INExpireDate(typeof(INTran.inventoryID), PersistingCheck=PXPersistingCheck.Nothing, FieldClass="LotSerial")]
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
		[PXDefault()]
		[INUnit(typeof(INTran.inventoryID))]
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
		[PXDBQuantity(typeof(INTran.uOM), typeof(INTran.baseQty))]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName="Quantity")]
		[PXFormula(null, typeof(SumCalc<INRegister.totalQty>))]
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
		#region Released
		public abstract class released : PX.Data.IBqlField
		{
		}
		protected Boolean? _Released;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Released")]
		public virtual Boolean? Released
		{
			get
			{
				return this._Released;
			}
			set
			{
				this._Released = value;
			}
		}
		#endregion
		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected String _FinPeriodID;
		[FinPeriodID()]
		[PXDBDefault(typeof(INRegister.finPeriodID))]
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
		#region TranPeriodID
		public abstract class tranPeriodID : PX.Data.IBqlField
		{
		}
		protected String _TranPeriodID;
		[FinPeriodID()]
		[PXDBDefault(typeof(INRegister.tranPeriodID))]
		public virtual String TranPeriodID
		{
			get
			{
				return this._TranPeriodID;
			}
			set
			{
				this._TranPeriodID = value;
			}
		}
		#endregion
		#region UnitPrice
		public abstract class unitPrice : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnitPrice;
		[PXDBPriceCost()]
		[PXDefault(TypeCode.Decimal, "0.0", typeof(Search<INItemSite.basePrice, Where<INItemSite.inventoryID, Equal<Current<INTran.inventoryID>>, And<INItemSite.siteID, Equal<Current<INTran.siteID>>>>>))]
		[PXUIField(DisplayName = "Unit Price")]
		public virtual Decimal? UnitPrice
		{
			get
			{
				return this._UnitPrice;
			}
			set
			{
				this._UnitPrice = value;
			}
		}
		#endregion
		#region TranAmt
		public abstract class tranAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranAmt;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Ext. Price")]
		[PXFormula(typeof(Mult<INTran.qty, INTran.unitPrice>), typeof(SumCalc<INRegister.totalAmount>))]
		public virtual Decimal? TranAmt
		{
			get
			{
				return this._TranAmt;
			}
			set
			{
				this._TranAmt = value;
			}
		}
        #endregion

       

        #region UnitCost
        public abstract class unitCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnitCost;
		[PXDBPriceCost(MinValue = 0)]
        [PXDefault(TypeCode.Decimal, "0.0", typeof(Coalesce<
            Search<INItemSite.tranUnitCost, Where<INItemSite.inventoryID, Equal<Current<INTran.inventoryID>>, And<INItemSite.siteID, Equal<Current<INTran.siteID>>>>>,
            Search<INItemCost.tranUnitCost, Where<INItemCost.inventoryID, Equal<Current<INTran.inventoryID>>>>>))]
        [PXUIField(DisplayName="Unit Cost")]
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
		#region AvgCost
		public abstract class avgCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _AvgCost;
		[PXPriceCost()]
		[PXDefault(TypeCode.Decimal, "0.0", typeof(Search<INItemSite.avgCost, Where<INItemSite.inventoryID, Equal<Current<INTran.inventoryID>>, And<INItemSite.siteID, Equal<Current<INTran.siteID>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
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
		#region TranCost
		public abstract class tranCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranCost;
		[PXDBBaseCury(MinValue = 0)]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName="Ext. Cost")]
		[PXFormula(typeof(Mult<INTran.qty, INTran.unitCost>), typeof(SumCalc<INRegister.totalCost>))]
		public virtual Decimal? TranCost
		{
			get
			{
				return this._TranCost;
			}
			set
			{
				this._TranCost = value;
			}
		}
		#endregion
		#region TranDesc
		public abstract class tranDesc : PX.Data.IBqlField
		{
		}
		protected String _TranDesc;
		[PXDBString(256, IsUnicode = true)]
		[PXUIField(DisplayName="Description")]
		[PXDefault(typeof(Search<InventoryItem.descr, Where<InventoryItem.inventoryID, Equal<Current<INTran.inventoryID>>>>),PersistingCheck=PXPersistingCheck.Nothing)]
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
		#region ReasonCode
		public abstract class reasonCode : PX.Data.IBqlField
		{
		}
		protected String _ReasonCode;
		[PXDBString(CS.ReasonCode.reasonCodeID.Length, IsUnicode = true)]
		[PXSelector(typeof(Search<ReasonCode.reasonCodeID, Where<ReasonCode.usage, Equal<Optional<INTran.docType>>>>))]
		[PXUIField(DisplayName="Reason Code")]
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
		#region OrigQty
		public abstract class origQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _OrigQty;
		[PXDBCalced(typeof(Minus<INTran.qty>), typeof(decimal))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? OrigQty
		{
			get
			{
				return this._OrigQty;
			}
			set
			{
				this._OrigQty = value;
			}
		}
		#endregion
		#region BaseQty
		public abstract class baseQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseQty;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
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
		#region UnassignedQty
		public abstract class unassignedQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnassignedQty;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? UnassignedQty
		{
			get
			{
				return this._UnassignedQty;
			}
			set
			{
				this._UnassignedQty = value;
			}
		}
		#endregion
		#region CostedQty
		public abstract class costedQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _CostedQty;
		[PXDecimal(6)]
		[PXFormula(typeof(decimal0))]
		public virtual Decimal? CostedQty
		{
			get
			{
				return this._CostedQty;
			}
			set
			{
				this._CostedQty = value;
			}
		}
		#endregion
		#region OrigTranCost
		public abstract class origTranCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _OrigTranCost;
		[PXBaseCury()]
		public virtual Decimal? OrigTranCost
		{
			get
			{
				return this._OrigTranCost;
			}
			set
			{
				this._OrigTranCost = value;
			}
		}
		#endregion
		#region OrigTranAmt
		public abstract class origTranAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _OrigTranAmt;
		[PXBaseCury()]
		public virtual Decimal? OrigTranAmt
		{
			get
			{
				return this._OrigTranAmt;
			}
			set
			{
				this._OrigTranAmt = value;
			}
		}
		#endregion
      
		#region ARDocType
		public abstract class aRDocType : PX.Data.IBqlField
		{
		}
		protected String _ARDocType;
		[PXDBString(3)]
		public virtual String ARDocType
		{
			get
			{
				return this._ARDocType;
			}
			set
			{
				this._ARDocType = value;
			}
		}
		#endregion
		#region ARRefNbr
		public abstract class aRRefNbr : PX.Data.IBqlField
		{
		}
		protected String _ARRefNbr;
		[PXDBString(15)]
		public virtual String ARRefNbr
		{
			get
			{
				return this._ARRefNbr;
			}
			set
			{
				this._ARRefNbr = value;
			}
		}
		#endregion
		#region ARLineNbr
		public abstract class aRLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _ARLineNbr;
		[PXDBInt()]
		public virtual Int32? ARLineNbr
		{
			get
			{
				return this._ARLineNbr;
			}
			set
			{
				this._ARLineNbr = value;
			}
		}
		#endregion
		#region SOOrderType
		public abstract class sOOrderType : PX.Data.IBqlField
		{
		}
		protected String _SOOrderType;
		[PXDBString(2, IsFixed = true)]
		[PXUIField(DisplayName="SO Order Type",  Visible = false)]
		[PXSelector(typeof(Search<SOOrderType.orderType>))]
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
		[PXUIField(DisplayName="SO Order Nbr.",  Visible = false, Enabled = false)]
		[PXSelector(typeof(Search<SOOrder.orderNbr, Where<SOOrder.orderType, Equal<Current<INTran.sOOrderType>>>>))]
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
		#region SOOrderLineNbr
		public abstract class sOOrderLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _SOOrderLineNbr;
		[PXDBInt()]
		public virtual Int32? SOOrderLineNbr
		{
			get
			{
				return this._SOOrderLineNbr;
			}
			set
			{
				this._SOOrderLineNbr = value;
			}
		}
		#endregion
		#region SOShipmentType
		public abstract class sOShipmentType : PX.Data.IBqlField
		{
		}
		protected String _SOShipmentType;
		[PXDBString(1, IsFixed = true)]
		public virtual String SOShipmentType
		{
			get
			{
				return this._SOShipmentType;
			}
			set
			{
				this._SOShipmentType = value;
			}
		}
		#endregion
		#region SOShipmentNbr
		public abstract class sOShipmentNbr : PX.Data.IBqlField
		{
		}
		protected String _SOShipmentNbr;
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName="SO Shipment Nbr.",  Visible = false, Enabled = false)]
		[PXSelector(typeof(Search<SOShipment.shipmentNbr>))]
		public virtual String SOShipmentNbr
		{
			get
			{
				return this._SOShipmentNbr;
			}
			set
			{
				this._SOShipmentNbr = value;
			}
		}
		#endregion
		#region SOShipmentLineNbr
		public abstract class sOShipmentLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _SOShipmentLineNbr;
		[PXDBInt()]
		public virtual Int32? SOShipmentLineNbr
		{
			get
			{
				return this._SOShipmentLineNbr;
			}
			set
			{
				this._SOShipmentLineNbr = value;
			}
		}
		#endregion
        #region UpdateShippedNotInvoiced
        public abstract class updateShippedNotInvoiced : PX.Data.IBqlField
        {
        }
        protected Boolean? _UpdateShippedNotInvoiced;
        [PXDBBool()]
        [PXDefault(false)]
        public virtual Boolean? UpdateShippedNotInvoiced
        {
            get
            {
                return this._UpdateShippedNotInvoiced;
            }
            set
            {
                this._UpdateShippedNotInvoiced = value;
            }
        }
        #endregion
        #region POReceiptType
        public abstract class pOReceiptType : PX.Data.IBqlField
        {
        }
        protected String _POReceiptType;
        [PXDBString(2, IsFixed = true)]
        [PXUIField(DisplayName = "PO Receipt Type", Visible = false, Enabled = false)]
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
		[PXUIField(DisplayName = "PO Receipt Nbr.",  Visible = false, Enabled = false)]
		[PXSelector(typeof(Search<POReceipt.receiptNbr>))]
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
		#region POReceiptLineNbr
		public abstract class pOReceiptLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _POReceiptLineNbr;
		[PXDBInt()]
		public virtual Int32? POReceiptLineNbr
		{
			get
			{
				return this._POReceiptLineNbr;
			}
			set
			{
				this._POReceiptLineNbr = value;
			}
		}
		#endregion
		#region POLineType
		public abstract class pOLineType : PX.Data.IBqlField
		{
		}
		protected String _POLineType;
		[PXDBString(2, IsFixed = true)]
		public virtual String POLineType
		{
			get
			{
				return this._POLineType;
			}
			set
			{
				this._POLineType = value;
			}
		}
		#endregion
		#region AssyType
		public abstract class assyType : PX.Data.IBqlField
		{
		}
		protected String _AssyType;
		[PXDBString(1, IsFixed = true)]
		public virtual String AssyType
		{
			get
			{
				return this._AssyType;
			}
			set
			{
				this._AssyType = value;
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
		#region ProjectID
		public abstract class projectID : PX.Data.IBqlField
		{
		}
		protected Int32? _ProjectID;
        [ProjectDefault(BatchModule.IN)]
        [ActiveProjectForModule(PX.Objects.GL.BatchModule.IN)]
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
		[ActiveProjectTask(typeof(INTran.projectID), BatchModule.IN, DisplayName = "Project Task")]
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

		#region HasMixedProjectTasks
		public abstract class hasMixedProjectTasks : PX.Data.IBqlField
		{
		}
		protected bool? _HasMixedProjectTasks;
		/// <summary>
		/// Returns true if the splits associated with the line has mixed ProjectTask values.
		/// This field is used to validate the record on persist. 
		/// </summary>
		[PXBool]
		[PXFormula(typeof(False))]
		public virtual bool? HasMixedProjectTasks
		{
			get
			{
				return _HasMixedProjectTasks;
			}
			set
			{
				_HasMixedProjectTasks = value;
			}
		}
		#endregion

		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXNote()]
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
		#region Unbound Properties
		#region SalesMult
		public abstract class salesMult : PX.Data.IBqlField
		{
		}
		[PXShort()]
		public virtual Int16? SalesMult
		{
			[PXDependsOnFields(typeof(tranType))]
			get
			{
				return INTranType.SalesMult(this._TranType);
			}
			set
			{
			}
		}
		#endregion
		#endregion
		#region Methods
		public static implicit operator INTranSplit(INTran item)
		{
			INTranSplit ret = new INTranSplit();
			ret.DocType = item.DocType;
			ret.TranType = item.TranType;
			ret.RefNbr = item.RefNbr;
			ret.LineNbr = item.LineNbr;
			ret.SplitLineNbr = 1;
			ret.InventoryID = item.InventoryID;
			ret.SiteID = item.SiteID;
			ret.SubItemID = item.SubItemID;
			ret.LocationID = item.LocationID;
			ret.LotSerialNbr = item.LotSerialNbr;
			ret.ExpireDate = item.ExpireDate;
			ret.Qty = item.Qty;
			ret.UOM = item.UOM;
			ret.TranDate = item.TranDate;
			ret.BaseQty = item.BaseQty;
			ret.InvtMult = item.InvtMult;
			ret.Released = item.Released;
            ret.POLineType = item.POLineType;
            ret.ToSiteID = item.ToSiteID;
            ret.ToLocationID = item.ToLocationID;

			return ret;
		}
		public static implicit operator INTran(INTranSplit item)
		{
			INTran ret = new INTran();
			ret.DocType = item.DocType;
			ret.TranType = item.TranType;
			ret.RefNbr = item.RefNbr;
			ret.LineNbr = item.LineNbr;
			ret.InventoryID = item.InventoryID;
			ret.SiteID = item.SiteID;
			ret.SubItemID = item.SubItemID;
			ret.LocationID = item.LocationID;
			ret.LotSerialNbr = item.LotSerialNbr;
			ret.Qty = item.Qty;
			ret.UOM = item.UOM;
			ret.TranDate = item.TranDate;
			ret.BaseQty = item.BaseQty;
			ret.InvtMult = item.InvtMult;
			ret.Released = item.Released;
            ret.POLineType = item.POLineType;
            ret.ToSiteID = item.ToSiteID;
            ret.ToLocationID = item.ToLocationID;

			return ret;
		}
		#endregion
	}

	public class INTranType
	{
		public class CustomListAttribute : PXStringListAttribute
		{
			public string[] AllowedValues
			{
				get
				{
					return _AllowedValues;
				}
			}

			public string[] AllowedLabels
			{
				get
				{
					return _AllowedLabels;
				}
			}

			public CustomListAttribute(string[] AllowedValues, string[] AllowedLabels)
				:base(AllowedValues, AllowedLabels)
			{ 
			}
		}

		public class ListAttribute : CustomListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Receipt, Issue, Return, Adjustment, Transfer, Invoice, DebitMemo, CreditMemo, Assembly, Disassembly, StandardCostAdjustment, NegativeCostAdjustment, ReceiptCostAdjustment },
				new string[] { Messages.Receipt, Messages.Issue, Messages.Return, Messages.Adjustment, Messages.Transfer, Messages.Invoice, Messages.DebitMemo, Messages.CreditMemo, Messages.Assembly, Messages.Disassembly, Messages.StandardCostAdjustment, Messages.NegativeCostAdjustment, Messages.ReceiptCostAdjustment }) { }
		}

		public class IssueListAttribute : CustomListAttribute
		{
			public IssueListAttribute()
				: base(
				new string[] { Issue, Return, Invoice, DebitMemo, CreditMemo },
				new string[] { Messages.Issue, Messages.Return, Messages.Invoice, Messages.DebitMemo, Messages.CreditMemo }) { }
		}

		public class SOListAttribute : CustomListAttribute
		{
			public SOListAttribute()
				: base(
				new string[] { Issue, Return, Transfer, Invoice, DebitMemo, CreditMemo, NoUpdate },
				new string[] { Messages.Issue, Messages.Return, Messages.Transfer, Messages.Invoice, Messages.DebitMemo, Messages.CreditMemo, Messages.NoUpdate }) { }
		}

		public class SONonARListAttribute : CustomListAttribute
		{
			public SONonARListAttribute()
				: base(
				new string[] { Issue, Return, Transfer, NoUpdate },
				new string[] { Messages.Issue, Messages.Return, Messages.Transfer, Messages.NoUpdate }) { }
		}

		public const string Assembly = "ASY";
		public const string Disassembly = "DSY";
		public const string Receipt = "RCP";
		public const string Issue = "III";
		public const string Return = "RET";
		public const string Adjustment = "ADJ";
		public const string Transfer = "TRX";
		public const string Invoice = "INV";
		public const string DebitMemo = "DRM";
		public const string CreditMemo = "CRM";
		public const string StandardCostAdjustment = "ASC";
		public const string NegativeCostAdjustment = "NSC";
		public const string ReceiptCostAdjustment = "RCA";

		public const string NoUpdate = "UND";

		public class adjustment : Constant<string>
		{
			public adjustment() : base(Adjustment) { ;}
		}

		public class receipt : Constant<string>
		{
			public receipt() : base(Receipt) { ;}
		}

		public class issue : Constant<string>
		{
			public issue() : base(Issue) { ;}
		}

		public class transfer : Constant<string>
		{
			public transfer() : base(Transfer) { ;}
		}

		public class return_ : Constant<string>
		{
			public return_() : base(Return) { ;}
		}

		public class invoice : Constant<string>
		{
			public invoice() : base(Invoice) { ;}
		}

		public class creditMemo : Constant<string>
		{
			public creditMemo() : base(CreditMemo) { ;}
		}

		public class debitMemo : Constant<string>
		{
			public debitMemo() : base(DebitMemo) { ;}
		}

		public class assembly : Constant<string>
		{
			public assembly() : base(Assembly) { ;}
		}

		public class disassembly : Constant<string>
		{
			public disassembly() : base(Disassembly) { ;}
		}

		public class standardCostAdjustment : Constant<string>
		{
			public standardCostAdjustment() : base(StandardCostAdjustment) { ;}
		}

		public class negativeCostAdjustment : Constant<string>
		{
			public negativeCostAdjustment() : base(NegativeCostAdjustment) { ;}
		}

		public class receiptCostAdjustment : Constant<string>
		{
			public receiptCostAdjustment() : base(ReceiptCostAdjustment) { ;}
		}

		public class noUpdate : Constant<string>
		{
			public noUpdate() : base(NoUpdate) { ; }
		}

		public static string DocType(string TranType)
		{
			switch (TranType)
			{
				case Issue:
				case Invoice:
				case DebitMemo:
				case CreditMemo:
				case Return:
					return INDocType.Issue;
				case Transfer:
					return INDocType.Transfer;
				case Receipt:
					return INDocType.Receipt;
				default:
					return INDocType.Undefined;
			}
		}

		public static Int16? InvtMult(string TranType)
		{
			switch (TranType)
			{
				case Issue:
				case Invoice:
				case DebitMemo:
				case Transfer:
				case Assembly:
				case Disassembly:
					return -1;
				case Receipt:
				case Return:
				case CreditMemo:
				case Adjustment:
				case StandardCostAdjustment:
				case NegativeCostAdjustment:
					return 1;
				case ReceiptCostAdjustment:
				case NoUpdate:
					return 0;
				default:
					return null;
			}
		}

		public static Int16? SalesMult(string TranType)
		{
			switch (TranType)
			{
				case Invoice:
				case DebitMemo:
					return 1;
				case CreditMemo:
					return -1;
				default:
					return null;
			}
		}
	}

	public class INAssyType
	{

		public const string KitTran = "K";
		public const string CompTran = "C";
		public const string OverheadTran = "O";


		public class kitTran : Constant<string>
		{
			public kitTran() : base(KitTran) { ;}
		}

		public class compTran : Constant<string>
		{
			public compTran() : base(CompTran) { ;}
		}

		public class overheadTran : Constant<string>
		{
			public overheadTran() : base(OverheadTran) { ;}
		}
	}

    


}
