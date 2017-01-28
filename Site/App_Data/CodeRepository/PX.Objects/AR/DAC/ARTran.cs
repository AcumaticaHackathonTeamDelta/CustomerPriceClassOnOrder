using System;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.IN;
using PX.Objects.CS;
using PX.Objects.TX;
using System.Diagnostics;
using PX.Objects.DR;
using PX.Objects.PM;
using PX.Objects.CR;

namespace PX.Objects.AR
{
	
	[System.SerializableAttribute()]
	[DebuggerDisplay("LineType={LineType} TranAmt={CuryTranAmt}")]
	[PXCacheName(Messages.ARTran)]
	public partial class ARTran : IBqlTable, SO.IDiscountable, DR.Descriptor.IDocumentLine
	{
        #region Selected
        public abstract class selected : IBqlField
        {
        }
        protected bool? _Selected = false;
        [PXBool()]
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
		[Branch(typeof(ARRegister.branchID))]
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
		#region TranType
		public abstract class tranType : PX.Data.IBqlField
		{
		}
		protected String _TranType;
		[PXDBString(3, IsKey = true, IsFixed = true)]
		[PXDBDefault(typeof(ARRegister.docType))]
		[PXUIField(DisplayName = "Tran. Type", Visibility = PXUIVisibility.Visible, Visible = false, Enabled = false)]
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
		[PXDBDefault(typeof(ARRegister.refNbr))]
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.Visible, Visible = false, Enabled = false)]
		[PXParent(typeof(Select<ARRegister, Where<ARRegister.docType, Equal<Current<ARTran.tranType>>, And<ARRegister.refNbr, Equal<Current<ARTran.refNbr>>>>>))]
		[PXParent(typeof(Select<SO.SOInvoice, Where<SO.SOInvoice.docType, Equal<Current<ARTran.tranType>>, And<SO.SOInvoice.refNbr, Equal<Current<ARTran.refNbr>>>>>))]
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
		[PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false)]
		[PXLineNbr(typeof(ARRegister.lineCntr))]
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
		#region SOOrderType
		public abstract class sOOrderType : PX.Data.IBqlField
		{
		}
		protected String _SOOrderType;
		[PXDBString(2, IsFixed = true)]
		[PXUIField(DisplayName = "Order Type", Enabled = false)]
		[PXSelector(typeof(Search<SO.SOOrderType.orderType>), CacheGlobal = true)]
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
		[PXUIField(DisplayName = "Order Nbr.", Enabled = false)]
		[PXSelector(typeof(Search<SO.SOOrder.orderNbr, Where<SO.SOOrder.orderType, Equal<Current<ARTran.sOOrderType>>>>))]
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
		#region CustomerID
		public abstract class customerID : PX.Data.IBqlField
		{
		}
		protected Int32? _CustomerID;
		[Customer()]
		[PXDBDefault(typeof(ARRegister.customerID))]
		public virtual Int32? CustomerID
		{
			get
			{
				return this._CustomerID;
			}
			set
			{
				this._CustomerID = value;
			}
		}
		#endregion
		#region BatchNbr
		public abstract class batchNbr : PX.Data.IBqlField
		{
		}
		protected String _BatchNbr;
		[PXDBString(15, IsUnicode = true)]
		[PXDefault("")]
		public virtual String BatchNbr
		{
			get
			{
				return this._BatchNbr;
			}
			set
			{
				this._BatchNbr = value;
			}
		}
		#endregion
		#region LineType
		public abstract class lineType : PX.Data.IBqlField
		{
		}
		protected String _LineType;
		[PXDBString(2, IsFixed = true)]
		[PXUIField(DisplayName="Line Type", Visible=false, Enabled=false)]
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
        #region IsFree
        public abstract class isFree : PX.Data.IBqlField
        {
        }
        protected Boolean? _IsFree;
        [PXBool()]
        public virtual Boolean? IsFree
        {
            get
            {
                return this._IsFree ?? false;
            }
            set
            {
                this._IsFree = value;
            }
        }
        #endregion
        #region ProjectID
        public abstract class projectID : PX.Data.IBqlField
		{
		}
		protected Int32? _ProjectID;
		[PXDBInt()]
		[PXDefault(typeof(ARInvoice.projectID), PersistingCheck=PXPersistingCheck.Nothing)]
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
		#region PMTranID
		public abstract class pMTranID : PX.Data.IBqlField
		{
		}
		protected Int64? _PMTranID;
		[PXDBLong]
		public virtual Int64? PMTranID
		{
			get
			{
				return this._PMTranID;
			}
			set
			{
				this._PMTranID = value;
			}
		}
		#endregion
		#region PMDeltaOption
		public abstract class pMDeltaOption : PX.Data.IBqlField
		{
			public const string CompleteLine = "C";
		}
		protected string _PMDeltaOption;
		[PXDBString()]
		[PXStringList(new string[] { pMDeltaOption.CompleteLine, "U" }, new string[] { Messages.CompleteLine, Messages.IncompleteLine })]
		[PXUIField(DisplayName="Billing Option")]
		public virtual string PMDeltaOption
		{
			get
			{
				return this._PMDeltaOption;
			}
			set
			{
				this._PMDeltaOption = value;
			}
		}
		#endregion
		#region ExpenseDate
		public abstract class expenseDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ExpenseDate;
		[PXDBDate()]
		public virtual DateTime? ExpenseDate
		{
			get
			{
				return this._ExpenseDate;
			}
			set
			{
				this._ExpenseDate = value;
			}
		}
		#endregion
		#region CuryInfoID
		public abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _CuryInfoID;
		[PXDBLong()]
		[CurrencyInfo(typeof(ARRegister.curyInfoID))]
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
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[NonStockNonKitItem(SO.Messages.CannotAddNonStockKitDirectly, typeof(ARTran.sOOrderNbr), Filterable=true)]
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
		#region DeferredCode
		public abstract class deferredCode : PX.Data.IBqlField
		{
		}
		protected String _DeferredCode;
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXUIField(DisplayName = "Deferral Code")]
		[PXSelector(typeof(Search<DRDeferredCode.deferredCodeID, Where<DRDeferredCode.accountType, Equal<DeferredAccountType.income>>>))]
		[PXDefault(typeof(Search2<InventoryItem.deferredCode, InnerJoin<DRDeferredCode, On<DRDeferredCode.deferredCodeID, Equal<InventoryItem.deferredCode>>>, Where<DRDeferredCode.accountType, Equal<DeferredAccountType.income>, And<InventoryItem.inventoryID, Equal<Current<ARTran.inventoryID>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
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
		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SiteID;
		[PXDBInt()]
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
		#region UOM
		public abstract class uOM : PX.Data.IBqlField
		{
		}
		protected String _UOM;
		[PXDefault(typeof(Search<InventoryItem.salesUnit, Where<InventoryItem.inventoryID, Equal<Current<ARTran.inventoryID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[INUnit(typeof(ARTran.inventoryID))]
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
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Quantity", Visibility = PXUIVisibility.Visible)]
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
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXFormula(null, typeof(AddCalc<SO.SOLine2.baseBilledQty>))]
		[PXFormula(null, typeof(SubCalc<SO.SOLine2.baseUnbilledQty>))]
		[PXFormula(null, typeof(AddCalc<SO.SOMiscLine2.baseBilledQty>))]
		[PXFormula(null, typeof(SubCalc<SO.SOMiscLine2.baseUnbilledQty>))]
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
		#region UnitCost
		public abstract class unitCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnitCost;
		[PXPriceCost()]
		[PXDBCalced(typeof(Switch<Case<Where<ARTran.qty, NotEqual<decimal0>>, Div<ARTran.tranCost, ARTran.qty>>, decimal0>), typeof(Decimal))]
		[PXDefault(TypeCode.Decimal, "0.0")]
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
		#region TranCost
		public abstract class tranCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranCost;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Ext. Cost")]
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
		#region TranCostOrig
		public abstract class tranCostOrig : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranCostOrig;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Orig. Ext. Cost")]
		public virtual Decimal? TranCostOrig
		{
			get
			{
				return this._TranCostOrig;
			}
			set
			{
				this._TranCostOrig = value;
			}
		}
		#endregion
		#region IsTranCostFinal
		public abstract class isTranCostFinal : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsTranCostFinal;
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? IsTranCostFinal
		{
			get
			{
				return this._IsTranCostFinal;
			}
			set
			{
				this._IsTranCostFinal = value;
			}
		}
		#endregion
		#region CuryUnitPrice
		public abstract class curyUnitPrice : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryUnitPrice;
		[PXDBCurrency(typeof(Search<CommonSetup.decPlPrcCst>), typeof(ARTran.curyInfoID), typeof(ARTran.unitPrice))]
		[PXUIField(DisplayName = "Unit Price", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryUnitPrice
		{
			get
			{
				return this._CuryUnitPrice;
			}
			set
			{
				this._CuryUnitPrice = value;
			}
		}
		#endregion
		#region UnitPrice
		public abstract class unitPrice : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnitPrice;
		[PXDBPriceCost()]
		//[PXDefault(typeof(Search<InventoryItem.basePrice, Where<InventoryItem.inventoryID, Equal<Current<ARTran.inventoryID>>>>))]
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
		#region CuryExtPrice
		public abstract class curyExtPrice : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryExtPrice;
		[PXDBCurrency(typeof(ARTran.curyInfoID), typeof(ARTran.extPrice))]
		[PXUIField(DisplayName = "Ext. Price")]
		[PXFormula(typeof(Mult<ARTran.qty, ARTran.curyUnitPrice>))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryExtPrice
		{
			get
			{
				return this._CuryExtPrice;
			}
			set
			{
				this._CuryExtPrice = value;
			}
		}
		#endregion
		#region ExtPrice
		public abstract class extPrice : PX.Data.IBqlField
		{
		}
		protected Decimal? _ExtPrice;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? ExtPrice
		{
			get
			{
				return this._ExtPrice;
			}
			set
			{
				this._ExtPrice = value;
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
        [PXDBCurrency(typeof(ARTran.curyInfoID), typeof(ARTran.discAmt))]
        [PXUIField(DisplayName = "Discount Amount")]
        //[PXFormula(typeof(Div<Mult<Mult<ARTran.qty, ARTran.curyUnitPrice>, ARTran.discPct>, decimal100>))]
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
		#region ManualDisc
		public abstract class manualDisc : PX.Data.IBqlField
		{
		}
		protected Boolean? _ManualDisc;
        [ManualDiscountMode(typeof(ARTran.curyDiscAmt), typeof(ARTran.curyTranAmt), typeof(ARTran.discPct), typeof(ARTran.freezeManualDisc))]
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
        #region ManualPrice
        public abstract class manualPrice : PX.Data.IBqlField
        {
        }
        protected Boolean? _ManualPrice;
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Manual Price", Visible=false)]
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
		#region CuryTranAmt
		public abstract class curyTranAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryTranAmt;
		[PXDBCurrency(typeof(ARTran.curyInfoID), typeof(ARTran.tranAmt))]
		[PXUIField(DisplayName = "Amount", Visibility = PXUIVisibility.SelectorVisible, Enabled=false)]
		[PXFormula(typeof(Sub<ARTran.curyExtPrice, ARTran.curyDiscAmt>))]
		[PXFormula(null, typeof(CountCalc<ARSalesPerTran.refCntr>))]
		[PXFormula(null, typeof(AddCalc<SO.SOMiscLine2.curyBilledAmt>))]
		[PXFormula(null, typeof(SubCalc<SO.SOMiscLine2.curyUnbilledAmt>))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryTranAmt
		{
			get
			{
				return this._CuryTranAmt;
			}
			set
			{
				this._CuryTranAmt = value;
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
		#region CuryTaxableAmt
		public abstract class curyTaxableAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryTaxableAmt;
		[PXDBCurrency(typeof(ARTran.curyInfoID), typeof(ARTran.taxableAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Net Amount", Enabled = false)]
		public virtual Decimal? CuryTaxableAmt
		{
			get
			{
				return this._CuryTaxableAmt;
			}
			set
			{
				this._CuryTaxableAmt = value;
			}
		}
		#endregion
		#region TaxableAmt
		public abstract class taxableAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _TaxableAmt;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TaxableAmt
		{
			get
			{
				return this._TaxableAmt;
			}
			set
			{
				this._TaxableAmt = value;
			}
		}
		#endregion
		#region CuryTaxAmt
		public abstract class curyTaxAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryTaxAmt;
		[PXDBCurrency(typeof(ARTran.curyInfoID), typeof(ARTran.taxAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "VAT", Enabled = false)]
		public virtual Decimal? CuryTaxAmt
		{
			get
			{
				return this._CuryTaxAmt;
			}
			set
			{
				this._CuryTaxAmt = value;
			}
		}
		#endregion
		#region TaxAmt
		public abstract class taxAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _TaxAmt;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TaxAmt
		{
			get
			{
				return this._TaxAmt;
			}
			set
			{
				this._TaxAmt = value;
			}
		}
		#endregion
		#region TranClass
		public abstract class tranClass : PX.Data.IBqlField
		{
		}
		protected String _TranClass;
		[PXDBString(1, IsFixed = true)]
		[PXDefault("")]
		public virtual String TranClass
		{
			get
			{
				return this._TranClass;
			}
			set
			{
				this._TranClass = value;
			}
		}
		#endregion
		#region DrCr
		public abstract class drCr : PX.Data.IBqlField
		{
		}
		protected String _DrCr;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(typeof(ARInvoice.drCr))]
		public virtual String DrCr
		{
			get
			{
				return this._DrCr;
			}
			set
			{
				this._DrCr = value;
			}
		}
		#endregion
		#region TranDate
		public abstract class tranDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _TranDate;
		[PXDBDate()]
		[PXDBDefault(typeof(ARRegister.docDate))]
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
		#region OrigInvoiceDate
		public abstract class origInvoiceDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _OrigInvoiceDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Original Invoice date")]
		public virtual DateTime? OrigInvoiceDate
		{
			get
			{
				return this._OrigInvoiceDate;
			}
			set
			{
				this._OrigInvoiceDate = value;
			}
		}
		#endregion
		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected String _FinPeriodID;
		[GL.FinPeriodID()]
		[PXDBDefault(typeof(ARRegister.finPeriodID))]
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
		#region TranDesc
		public abstract class tranDesc : PX.Data.IBqlField
		{
		}
		protected String _TranDesc;
		[PXDBString(256, IsUnicode = true)]
		[PXUIField(DisplayName = "Transaction Descr.", Visibility = PXUIVisibility.Visible)]
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
		#region Released
		public abstract class released : PX.Data.IBqlField
		{
		}
		protected Boolean? _Released;
		[PXDBBool()]
		[PXDefault(false)]
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
		#region SOOrderLineNbr
		public abstract class sOOrderLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _SOOrderLineNbr;
		[PXDBInt()]
		[PXParent(typeof(Select<SO.SOLine2, Where<SO.SOLine2.orderType, Equal<Current<ARTran.sOOrderType>>, And<SO.SOLine2.orderNbr, Equal<Current<ARTran.sOOrderNbr>>, And<SO.SOLine2.lineNbr, Equal<Current<ARTran.sOOrderLineNbr>>>>>>), LeaveChildren = true)]
		[PXParent(typeof(Select<SO.SOMiscLine2, Where<SO.SOMiscLine2.orderType, Equal<Current<ARTran.sOOrderType>>, And<SO.SOMiscLine2.orderNbr, Equal<Current<ARTran.sOOrderNbr>>, And<SO.SOMiscLine2.lineNbr, Equal<Current<ARTran.sOOrderLineNbr>>>>>>), LeaveChildren = true)]
		[PXUIField(DisplayName = "Order Line Nbr", Visible = false, Enabled = false)]
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
		[PXUIField(DisplayName = "Shipment Nbr.", Enabled = false)]
		[PXSelector(typeof(Search<SO.SOOrderShipment.shipmentNbr, Where<SO.SOOrderShipment.orderType, Equal<Current<ARTran.sOOrderType>>, And<SO.SOOrderShipment.orderNbr, Equal<Current<ARTran.sOOrderNbr>>, And<SO.SOOrderShipment.shipmentType, Equal<Current<ARTran.sOShipmentType>>>>>>), DirtyRead = true)]
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
		#region SalesPersonID
		public abstract class salesPersonID : PX.Data.IBqlField
		{
		}
		protected Int32? _SalesPersonID;
		[SalesPerson()]
		[PXParent(typeof(Select<ARSalesPerTran, Where<ARSalesPerTran.docType, Equal<Current<ARTran.tranType>>, And<ARSalesPerTran.refNbr, Equal<Current<ARTran.refNbr>>, And<ARSalesPerTran.salespersonID, Equal<Current2<ARTran.salesPersonID>>, And<Current<ARTran.commissionable>, Equal<True>>>>>>), LeaveChildren = true, ParentCreate = true)]
		[PXDefault(typeof(ARRegister.salesPersonID), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXFormula(typeof(Switch<Case<Where<ARTran.lineType, Equal<SO.SOLineType.freight>>, Null>, ARTran.salesPersonID>))]
		public virtual Int32? SalesPersonID
		{
			get
			{
				return this._SalesPersonID;
			}
			set
			{
				this._SalesPersonID = value;
			}
		}
		#endregion
        #region EmployeeID
        public abstract class employeeID : PX.Data.IBqlField
        {
        }
        protected Int32? _EmployeeID;
        [PXDBInt]
        [PXDefault(typeof(Search<EP.EPEmployee.bAccountID, Where<EP.EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>), PersistingCheck=PXPersistingCheck.Nothing)]
        public virtual Int32? EmployeeID
        {
            get
            {
                return this._EmployeeID;
            }
            set
            {
                this._EmployeeID = value;
            }
        }
        #endregion
		#region CommnPct
		public abstract class commnPct : PX.Data.IBqlField
		{
		}
		protected Decimal? _CommnPct;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CommnPct
		{
			get
			{
				return this._CommnPct;
			}
			set
			{
				this._CommnPct = value;
			}
		}
		#endregion
		#region CuryCommnAmt
		public abstract class curyCommnAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryCommnAmt;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryCommnAmt
		{
			get
			{
				return this._CuryCommnAmt;
			}
			set
			{
				this._CuryCommnAmt = value;
			}
		}
		#endregion
		#region CommnAmt
		public abstract class commnAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CommnAmt;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CommnAmt
		{
			get
			{
				return this._CommnAmt;
			}
			set
			{
				this._CommnAmt = value;
			}
		}
		#endregion
		#region DefScheduleID
		public abstract class defScheduleID : IBqlField
		{
		}
		protected int? _DefScheduleID;
		[PXDBInt]
		[PXUIField(DisplayName = "Original Deferral Schedule")]
		[PXSelector(typeof(Search<DR.DRSchedule.scheduleID, Where<DR.DRSchedule.bAccountID, Equal<Current<ARInvoice.customerID>>,
		And<DR.DRSchedule.docType, NotEqual<Current<ARTran.tranType>>>>>))]
		public virtual int? DefScheduleID
		{
			get
			{
				return this._DefScheduleID;
			}
			set
			{
				this._DefScheduleID = value;
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
		[PXDefault(typeof(Search<InventoryItem.taxCategoryID,
			Where<InventoryItem.inventoryID, Equal<Current<ARTran.inventoryID>>>>),
			PersistingCheck = PXPersistingCheck.Nothing, SearchOnDefault = false)]
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
		#region ReasonCode
		public abstract class reasonCode : PX.Data.IBqlField
		{
		}
		protected String _ReasonCode;
		[PXDBString(CS.ReasonCode.reasonCodeID.Length, IsUnicode = true)]
		[PXSelector(typeof(Search<ReasonCode.reasonCodeID,
			Where<ReasonCode.usage, Equal<ReasonCodeUsages.sales>, Or<ReasonCode.usage, Equal<ReasonCodeUsages.issue>>>>), DescriptionField = typeof(ReasonCode.descr))]
		[PXUIField(DisplayName = "Reason Code")]
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
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;
		[Account(typeof(ARTran.branchID), DisplayName = "Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		[PXDefault(typeof(Search<InventoryItem.salesAcctID, Where<InventoryItem.inventoryID, Equal<Current<ARTran.inventoryID>>>>))]
		public virtual Int32? AccountID
		{
			get
			{
				return this._AccountID;
			}
			set
			{
				this._AccountID = value;
			}
		}
		#endregion
		#region SubID
		public abstract class subID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubID;
		[SubAccount(typeof(ARTran.accountID), typeof(ARTran.branchID), true, DisplayName = "Subaccount", Visibility = PXUIVisibility.Visible)]
		[PXDefault(typeof(Search<InventoryItem.salesSubID, Where<InventoryItem.inventoryID, Equal<Current<ARTran.inventoryID>>>>))]
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
		#region TaskID
		public abstract class taskID : PX.Data.IBqlField
		{
		}
		protected Int32? _TaskID;
		[PXDefault(typeof(Search<PMAccountTask.taskID, Where<PMAccountTask.projectID, Equal<Current<ARTran.projectID>>, And<PMAccountTask.accountID, Equal<Current<ARTran.accountID>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[ActiveProjectTask(typeof(ARTran.projectID), BatchModule.AR, DisplayName = "Project Task")]
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
		#region Commissionable
		public abstract class commissionable : IBqlField
		{
		}
		protected bool? _Commissionable;
		[PXDBBool()]
		[PXFormula(typeof(Switch<Case<Where<ARTran.inventoryID, IsNotNull>, Selector<ARTran.inventoryID, InventoryItem.commisionable>>, True>))]
		[PXUIField(DisplayName = "Commissionable")]
		public bool? Commissionable
		{
			get
			{
				return _Commissionable;
			}
			set
			{
				_Commissionable = value;
			}
		}
		#endregion
		#region Date
		public abstract class date : PX.Data.IBqlField
		{
		}
		protected DateTime? _Date;
		/// <summary>
		/// Reference Date. May be an original expense date that is billed to the customer.
		/// </summary>
		[PXDBDate()]
		[PXUIField(DisplayName = "Date", Visible=false)]
		public virtual DateTime? Date
		{
			get
			{
				return this._Date;
			}
			set
			{
				this._Date = value;
			}
		}
		#endregion
		#region CaseID
		public abstract class caseID : PX.Data.IBqlField
		{
		}
		protected Int32? _CaseID;
		[PXSelector(typeof(Search<CRCase.caseID>), SubstituteKey = typeof(CRCase.caseCD))]
		[PXDBInt]
		[PXUIField(DisplayName = "Case ID", Visible = false, Enabled=false)]
		public virtual Int32? CaseID
		{
			get
			{
				return this._CaseID;
			}
			set
			{
				this._CaseID = value;
			}
		}
		#endregion

		#region FreezeManualDisc
		public abstract class freezeManualDisc : PX.Data.IBqlField
		{
		}
		protected Boolean? _FreezeManualDisc;
		[PXBool()]
		public virtual Boolean? FreezeManualDisc
		{
			get
			{
				return this._FreezeManualDisc;
			}
			set
			{
				this._FreezeManualDisc = value;
			}
		}
		#endregion
		#region DetDiscIDC1
		public abstract class detDiscIDC1 : PX.Data.IBqlField
		{
		}
		protected String _DetDiscIDC1;
		[PXDBString(10, IsUnicode = true)]
		public virtual String DetDiscIDC1
		{
			get
			{
				return this._DetDiscIDC1;
			}
			set
			{
				this._DetDiscIDC1 = value;
			}
		}
		#endregion
		#region DetDiscSeqIDC1
		public abstract class detDiscSeqIDC1 : PX.Data.IBqlField
		{
		}
		protected String _DetDiscSeqIDC1;
		[PXDBString(10, IsUnicode = true)]
		public virtual String DetDiscSeqIDC1
		{
			get
			{
				return this._DetDiscSeqIDC1;
			}
			set
			{
				this._DetDiscSeqIDC1 = value;
			}
		}
		#endregion
		#region DetDiscIDC2
		public abstract class detDiscIDC2 : PX.Data.IBqlField
		{
		}
		protected String _DetDiscIDC2;
		[PXDBString(10, IsUnicode = true)]
		public virtual String DetDiscIDC2
		{
			get
			{
				return this._DetDiscIDC2;
			}
			set
			{
				this._DetDiscIDC2 = value;
			}
		}
		#endregion
		#region DetDiscSeqIDC2
		public abstract class detDiscSeqIDC2 : PX.Data.IBqlField
		{
		}
		protected String _DetDiscSeqIDC2;
		[PXDBString(10, IsUnicode = true)]
		public virtual String DetDiscSeqIDC2
		{
			get
			{
				return this._DetDiscSeqIDC2;
			}
			set
			{
				this._DetDiscSeqIDC2 = value;
			}
		}
		#endregion
		#region DetDiscApp
		public abstract class detDiscApp : PX.Data.IBqlField
		{
		}
		protected Boolean? _DetDiscApp;
		[PXDBBool()]
		[PXDefault(false, PersistingCheck=PXPersistingCheck.Nothing)]
		public virtual Boolean? DetDiscApp
		{
			get
			{
				return this._DetDiscApp;
			}
			set
			{
				this._DetDiscApp = value;
			}
		}
		#endregion
		#region PromoDiscID
		public virtual string PromoDiscID
		{
			get
			{
				return null;
			}
			set
			{ ;}
		}
		#endregion
		#region DocDiscIDC1
		public abstract class docDiscIDC1 : PX.Data.IBqlField
		{
		}
		protected String _DocDiscIDC1;
		[PXDBString(10, IsUnicode = true)]
		public virtual String DocDiscIDC1
		{
			get
			{
				return this._DocDiscIDC1;
			}
			set
			{
				this._DocDiscIDC1 = value;
			}
		}
		#endregion
		#region DocDiscSeqIDC1
		public abstract class docDiscSeqIDC1 : PX.Data.IBqlField
		{
		}
		protected String _DocDiscSeqIDC1;
		[PXDBString(10, IsUnicode = true)]
		public virtual String DocDiscSeqIDC1
		{
			get
			{
				return this._DocDiscSeqIDC1;
			}
			set
			{
				this._DocDiscSeqIDC1 = value;
			}
		}
		#endregion
		#region DocDiscIDC2
		public abstract class docDiscIDC2 : PX.Data.IBqlField
		{
		}
		protected String _DocDiscIDC2;
		[PXDBString(10, IsUnicode = true)]
		public virtual String DocDiscIDC2
		{
			get
			{
				return this._DocDiscIDC2;
			}
			set
			{
				this._DocDiscIDC2 = value;
			}
		}
		#endregion
		#region DocDiscSeqIDC2
		public abstract class docDiscSeqIDC2 : PX.Data.IBqlField
		{
		}
		protected String _DocDiscSeqIDC2;
		[PXDBString(10, IsUnicode = true)]
		public virtual String DocDiscSeqIDC2
		{
			get
			{
				return this._DocDiscSeqIDC2;
			}
			set
			{
				this._DocDiscSeqIDC2 = value;
			}
		}
		#endregion
		

		#region IDiscountable Members

		public decimal? CuryLineAmt
		{
			get
			{
				return this.CuryTranAmt;
			}
			set
			{
				this.CuryTranAmt = value;
			}
		}

        #endregion
        #region DiscountID
        public abstract class discountID : PX.Data.IBqlField
        {
        }
        protected String _DiscountID;
        [PXDBString(10, IsUnicode = true)]
        [PXSelector(typeof(Search<ARDiscount.discountID, Where<ARDiscount.type, Equal<DiscountType.LineDiscount>, And<ARDiscount.applicableTo, NotEqual<DiscountTarget.warehouse>, And<ARDiscount.applicableTo, NotEqual<DiscountTarget.warehouseAndCustomer>, 
              And<ARDiscount.applicableTo, NotEqual<DiscountTarget.warehouseAndCustomerPrice>, And<ARDiscount.applicableTo, NotEqual<DiscountTarget.warehouseAndInventory>, And<ARDiscount.applicableTo, NotEqual<DiscountTarget.warehouseAndInventoryPrice>>>>>>>>))]
        [PXUIField(DisplayName = "Discount Code", Visible = true, Enabled = true)]
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


		#region DRTermStartDate
		public abstract class dRTermStartDate : IBqlField { }

		protected DateTime? _DRTermStartDate;

		[PXDBDate]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Term Start Date")]
		public DateTime? DRTermStartDate
		{
			get { return _DRTermStartDate; }
			set { _DRTermStartDate = value; }
		}
		#endregion
		#region DRTermEndDate
		public abstract class dRTermEndDate : IBqlField { }

		protected DateTime? _DRTermEndDate;

		[PXDBDate]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Term End Date")]
		public DateTime? DRTermEndDate
		{
			get { return _DRTermEndDate; }
			set { _DRTermEndDate = value; }
		}
		#endregion
		#region RequiresTerms
		public abstract class requiresTerms : IBqlField { }

		/// <summary>
		/// When set to <c>true</c>, indicates that the <see cref="DRTermStartDate"/> and <see cref="DRTermEndDate"/>
		/// fields are enabled and should be filled for the line.
		/// </summary>
		/// <value>
		/// The value of this field is set by the <see cref="ARInvoiceEntry"/> and <see cref="ARCashSaleEntry"/> graphs
		/// based on the settings of the <see cref="InventoryID">item</see> and the <see cref="DeferredCode">Deferral Code</see> selected
		/// for the line. In other contexts it is not populated.
		/// See the attribute on the <see cref="ARInvoiceEntry.ARTran_RequiresTerms_CacheAttached"/> handler for details.
		/// </value>
		[PXBool]
		public virtual bool? RequiresTerms
		{
			get;
			set;
		}
		#endregion
		#region CuryUnitPriceDR
		public abstract class curyUnitPriceDR : IBqlField { }

		protected decimal? _CuryUnitPriceDR;

		[PXUIField(DisplayName = "Unit Price for DR", Visible = false)]
		[PXDBDecimal(typeof(Search<CommonSetup.decPlPrcCst>))]
		public virtual decimal? CuryUnitPriceDR
		{
			get { return _CuryUnitPriceDR; }
			set { _CuryUnitPriceDR = value; }
		}
		#endregion
		#region DiscPctDR
		public abstract class discPctDR : IBqlField { }

		protected decimal? _DiscPctDR;

		[PXUIField(DisplayName = "Discount Percent for DR", Visible = false)]
		[PXDBDecimal(6, MinValue = -100, MaxValue = 100)]
		public virtual decimal? DiscPctDR
		{
			get { return _DiscPctDR; }
			set { _DiscPctDR = value; }
		}
		#endregion
		#region ItemHasResidual
		public abstract class itemHasResidual : IBqlField { }

		[PXBool]
		[DR.DRTerms.VerifyResidual(typeof(inventoryID), typeof(deferredCode), typeof(curyUnitPriceDR), typeof(curyExtPrice))]
		public virtual bool? ItemHasResidual
		{
			get;
			set;
		}
		#endregion

		#region Sales Reporting
		#region GroupDiscountAmount
		public abstract class groupDiscountAmount : IBqlField { }
		protected decimal? _GroupDiscountAmount;
		[PXUIField(DisplayName = "Group Discount Amount", Enabled = false)]
		[PXDecimal]
		[PXDBCalced(typeof(Mult<ARTran.tranAmt, Sub<decimal1, ARTran.groupDiscountRate>>), typeof(Decimal))]
		public virtual decimal? GroupDiscountAmount
		{
			get
			{
				return this._GroupDiscountAmount;
			}
			set
			{
				this._GroupDiscountAmount = value;
			}
		}
		#endregion
		#region DocumentDiscountAmount
		public abstract class documentDiscountAmount : IBqlField { }
		[PXUIField(DisplayName = "Document Discount Amount", Enabled = false)]
		[PXDecimal]
		[PXDBCalced(typeof(Mult<ARTran.tranAmt, Sub<decimal1, ARTran.documentDiscountRate>>), typeof(Decimal))]
		public virtual decimal? DocumentDiscountAmount
		{
			get;
			set;
		}
		#endregion
		#region GrossSalesAmount
		public abstract class grossSalesAmount : IBqlField { }
		protected decimal? _GrossSalesAmount;
		[PXUIField(DisplayName = "Gross Sales Amount", Enabled = false)]
		[PXDecimal]
		[PXDBCalced(typeof(Add<ARTran.discAmt, Switch<Case<Where<ARTran.taxableAmt, Equal<decimal0>>, ARTran.tranAmt>, ARTran.taxableAmt>>), typeof(Decimal))]
		public virtual decimal? GrossSalesAmount
		{
			get
			{
				return this._GrossSalesAmount;
			}
			set
			{
				this._GrossSalesAmount = value;
			}
		}
		#endregion
		#region Cost
		public abstract class cost : IBqlField { }
		[PXUIField(DisplayName = "Cost", Enabled = false)]
		[PXDecimal]
		[PXDBCalced(typeof(Mult<Switch<Case<Where<ARTran.drCr, Equal<DrCr.debit>>, Minus<decimal1>>, decimal1>, Switch<Case<Where<ARTran.isTranCostFinal, Equal<False>>, ARTran.tranCostOrig>, ARTran.tranCost>>), typeof(Decimal))]
		public virtual decimal? Cost
		{
			get;
			set;
		}
		#endregion
		#region NetSalesAmount
		public abstract class netSalesAmount : IBqlField { }
		protected decimal? _NetSalesAmount;
		[PXUIField(DisplayName = "Net Sales Amount", Enabled = false)]
		[PXDecimal]
		[PXDBCalced(typeof(Mult<Switch<Case<Where<ARTran.drCr, Equal<DrCr.debit>>, Minus<decimal1>>, decimal1>, Sub<Sub<Sub<ARTran.grossSalesAmount, ARTran.discAmt>, ARTran.groupDiscountAmount>, ARTran.documentDiscountAmount>>), typeof(Decimal))]
		public virtual decimal? NetSalesAmount
		{
			get
			{
				return this._NetSalesAmount;
			}
			set
			{
				this._NetSalesAmount = value;
			}
		}
		#endregion
		#region Margin
		public abstract class margin : IBqlField { }
		[PXUIField(DisplayName = "Margin", Enabled = false)]
		[PXDecimal]
		[PXDBCalced(typeof(Sub<ARTran.netSalesAmount, ARTran.cost>), typeof(Decimal))]
		public virtual decimal? Margin
		{
			get;
			set;
		}
		#endregion
		#region MarginPercent
		public abstract class marginPercent : IBqlField { }
		[PXUIField(DisplayName = "Margin Percent", Enabled = false)]
		[PXDecimal]
		[PXDBCalced(typeof(Mult<Switch<Case<Where<ARTran.drCr, Equal<DrCr.debit>>, Minus<decimal1>>, decimal1>, Switch<Case<Where<ARTran.netSalesAmount, NotEqual<decimal0>>, Mult<Div<ARTran.margin, ARTran.netSalesAmount>, decimal100>>, decimal0>>), typeof(Decimal))]
		public virtual decimal? MarginPercent
		{
			get;
			set;
		}
		#endregion
		#endregion

		#region DR Interface Implementation
		string DR.Descriptor.IDocumentLine.Module => BatchModule.AR;
		#endregion
	}
	public class decimalPct : Constant<decimal>
    {
        public decimalPct() : base(0.01m) { }
	}
}
