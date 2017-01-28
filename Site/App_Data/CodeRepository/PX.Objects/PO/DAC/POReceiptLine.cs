namespace PX.Objects.PO
{
	using System;
	using PX.Data;
	using PX.Objects.TX;
	using PX.Objects.GL;
	using PX.Objects.IN;
	using PX.Objects.AP;
	using PX.Objects.CM;
	using PX.Objects.CS;
	using PX.Objects.CR;
	using PX.Objects.PM;
	
	[System.SerializableAttribute()]
	[PXCacheName(Messages.POReceiptLine)]
	public partial class POReceiptLine : PX.Data.IBqlTable, ILSPrimary
	{
		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		protected Int32? _BranchID;
		[Branch(typeof(POReceipt.branchID))]
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
		#region ReceiptType
		public abstract class receiptType : PX.Data.IBqlField
		{
			public const int Length = 2;
		}
		protected String _ReceiptType;
		[PXUIField(DisplayName = "Type")]
		[PXDBString(receiptType.Length, IsKey = true, IsFixed = true)]
		[PXDBDefault(typeof(POReceipt.receiptType))]
		public virtual String ReceiptType
		{
			get
			{
				return this._ReceiptType;
			}
			set
			{
				this._ReceiptType = value;
			}
		}
		#endregion
		#region ReceiptNbr
		public abstract class receiptNbr : PX.Data.IBqlField
		{
		}
		protected String _ReceiptNbr;
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "")]
		[PXDBDefault(typeof(POReceipt.receiptNbr))]
		[PXParent(typeof(Select<POReceipt, Where<POReceipt.receiptNbr, Equal<Current<POReceiptLine.receiptNbr>>>>))]
		[PXUIField(DisplayName = "Order Nbr.", Visibility = PXUIVisibility.Invisible, Visible = false)]
		public virtual String ReceiptNbr
		{
			get
			{
				return this._ReceiptNbr;
			}
			set
			{
				this._ReceiptNbr = value;
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
		[PXLineNbr(typeof(POReceipt.lineCntr))]
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
		#region InvtMult
		public abstract class invtMult : PX.Data.IBqlField
		{
		}
		protected Int16? _InvtMult;
		[PXDBShort()]
		[PXDefault()]
		[PXUIField(DisplayName = "Inventory Multiplier")]
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
		[POLineInventoryItem(Filterable = true)]
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
		#region LineType
		public abstract class lineType : PX.Data.IBqlField
		{
			public const int Length = 2;
		}
		protected String _LineType;
		[PXDBString(lineType.Length, IsFixed = true)]
		[PXDefault(POLineType.Service)]
		[POReceiptLineTypeList(typeof(POReceiptLine.inventoryID))]
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
		#region TranType

		public abstract class tranType : PX.Data.IBqlField
		{
		}

		public string TranType
		{
			[PXDependsOnFields(typeof(receiptType))]
			get
			{
				return POReceiptType.GetINTranType(this._ReceiptType);
			}
		}
		#endregion
		#region TranDate
		public virtual DateTime? TranDate
		{
			get { return this._ReceiptDate; }
		}
		#endregion
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[Vendor(typeof(Search<BAccountR.bAccountID,
			Where<BAccountR.type, Equal<BAccountType.companyType>, Or<Vendor.type, NotEqual<BAccountType.employeeType>>>>),
			CacheGlobal = true, Filterable = true)]
		[PXDBDefault(typeof(POReceipt.vendorID))]
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
		#region ReceiptDate
		public abstract class receiptDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ReceiptDate;

		[PXDBDate()]
		[PXDBDefault(typeof(POReceipt.receiptDate))]
		public virtual DateTime? ReceiptDate
		{
			get
			{
				return this._ReceiptDate;
			}
			set
			{
				this._ReceiptDate = value;
			}
		}
		#endregion
		#region SubItemID
		public abstract class subItemID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubItemID;
		[SubItem(typeof(POReceiptLine.inventoryID))]
		[PXDefault(typeof(Search<InventoryItem.defaultSubItemID,
			Where<InventoryItem.inventoryID, Equal<Current2<POReceiptLine.inventoryID>>,
			And<InventoryItem.defaultSubItemOnEntry, Equal<boolTrue>>>>),
			PersistingCheck = PXPersistingCheck.Nothing)]
		[PXFormula(typeof(Default<POReceiptLine.inventoryID>))]
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
		#region UOM
		public abstract class uOM : PX.Data.IBqlField
		{
		}
		protected String _UOM;

		[INUnit(typeof(POReceiptLine.inventoryID))]
        [UOMDefault(INUOMDefaulType.Purchase, typeof(POReceiptLine.inventoryID), typeof(POReceiptLine.siteID), PersistingCheck = PXPersistingCheck.Nothing)]
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
		#region POType
		public abstract class pOType : PX.Data.IBqlField
		{
		}
		protected String _POType;
		[PXDBString(2, IsFixed = true)]
		[POOrderType.List()]
		[PXUIField(DisplayName = "PO Order Type")]
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
			public const int Length = 15;
		}
		protected String _PONbr;
		[PXDBString(pONbr.Length, IsUnicode = true)]
		[PXUIField(DisplayName = "PO Order Nbr.")]
		[PO.RefNbr(typeof(Search2<POOrder.orderNbr,
			LeftJoinSingleTable<Vendor, On<POOrder.vendorID, Equal<Vendor.bAccountID>,
			And<Match<Vendor, Current<AccessInfo.userName>>>>>,
			Where<POOrder.orderType, Equal<Optional<POReceiptLine.pOType>>,
			And<Vendor.bAccountID, IsNotNull>>,
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
		[PXParent(typeof(Select<POLineR, Where<POLineR.orderType, Equal<Current<POReceiptLine.pOType>>,
										And<POLineR.orderNbr, Equal<Current<POReceiptLine.pONbr>>,
										And<POLineR.lineNbr, Equal<Current<POReceiptLine.pOLineNbr>>>>>>))]
		[PXUIField(DisplayName = "PO Line Nbr.")]
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
		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SiteID;
		[IN.POSiteAvail(typeof(POReceiptLine.inventoryID), typeof(POReceiptLine.subItemID))]
		[PXDefault(typeof(Coalesce<Search<CR.Location.vSiteID,
			Where<CR.Location.locationID, Equal<Current2<POReceipt.vendorLocationID>>,
				And<CR.Location.bAccountID, Equal<Current2<POReceipt.vendorID>>>>>,
					Search<InventoryItem.dfltSiteID, Where<InventoryItem.inventoryID, Equal<Current2<POReceiptLine.inventoryID>>>>>))]
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
		[POLocationAvail(typeof(POReceiptLine.inventoryID), typeof(POReceiptLine.subItemID), typeof(POReceiptLine.siteID), typeof(POReceiptLine.tranType), typeof(POReceiptLine.invtMult), KeepEntry = false)]
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
		#region LotSerialNbr
		public abstract class lotSerialNbr : PX.Data.IBqlField
		{
		}
		protected String _LotSerialNbr;
		[POLotSerialNbrAttribute(typeof(POReceiptLine.inventoryID), typeof(POReceiptLine.subItemID), typeof(POReceiptLine.locationID), PersistingCheck = PXPersistingCheck.Nothing)]
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
		#region AllowComplete
		public abstract class allowComplete : PX.Data.IBqlField
		{
		}
		protected Boolean? _AllowComplete;
		[PXBool()]
		[PXUIField(DisplayName = "Complete PO Line", Visibility = PXUIVisibility.Service, Visible = true)]		
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
		#region AllowOpen
		public abstract class allowOpen : PX.Data.IBqlField
		{
		}
		protected Boolean? _AllowOpen;
		[PXBool()]
		[PXUIField(DisplayName = "Open PO Line", Visibility = PXUIVisibility.Service, Visible = true)]
		public virtual Boolean? AllowOpen
		{
			get
			{
				return this._AllowOpen;
			}
			set
			{
				this._AllowOpen = value;

			}
		}
		#endregion
				
		#region ReceiptQty
		public abstract class receiptQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _ReceiptQty;

		[PXDBQuantity(typeof(POReceiptLine.uOM), typeof(POReceiptLine.baseReceiptQty),HandleEmptyKey = true, MinValue = 0)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXFormula(null, typeof(SumCalc<POReceipt.orderQty>))]
		[PXUIField(DisplayName = "Receipt Qty.", Visibility = PXUIVisibility.Visible)]
		public virtual Decimal? ReceiptQty
		{
			get
			{
				return this._ReceiptQty;
			}
			set
			{
				this._ReceiptQty = value;
			}
		}

		public virtual Decimal? Qty
		{
			get
			{
				return this._ReceiptQty;
			}
			set
			{
				this._ReceiptQty = value;
			}
		}
		#endregion
		#region BaseReceiptQty
		public abstract class baseReceiptQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseReceiptQty;

		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? BaseReceiptQty
		{
			get
			{
				return this._BaseReceiptQty;
			}
			set
			{
				this._BaseReceiptQty = value;
			}
		}
		public virtual Decimal? BaseQty
		{
			get
			{
				return this._BaseReceiptQty;
			}
			set
			{
				this._BaseReceiptQty = value;
			}
		}
		#endregion
		#region BaseMultReceiptQty
		public abstract class baseMultReceiptQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseMiltReceiptQty ;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXFormula(typeof(Mult<POReceiptLine.baseReceiptQty, POReceiptLine.invtMult>))]
		[PXFormula(null, typeof(SumCalc<POLineR.baseReceivedQty>))]
		public virtual Decimal? BaseMultReceiptQty
		{
			get
			{
				return this._BaseMiltReceiptQty;
			}
			set
			{
				this._BaseMiltReceiptQty = value;
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
		#region CuryInfoID
		public abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _CuryInfoID;
		[PXDBLong()]
		[CurrencyInfo(typeof(POReceipt.curyInfoID))]
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

		[PXDBCurrency(typeof(Search<CommonSetup.decPlPrcCst>), typeof(POReceiptLine.curyInfoID), typeof(POReceiptLine.unitCost))]
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

		[PXUIField(DisplayName = "Unit Cost")]
		[PXDBPriceCost()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		//[PXDefault(typeof(Search<InventoryItem.lastCost, Where<InventoryItem.inventoryID, Equal<Current<POReceiptLine.inventoryID>>>>))]
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
        [PXDBCurrency(typeof(POReceiptLine.curyInfoID), typeof(POReceiptLine.discAmt))]
        [PXUIField(DisplayName = "Discount Amount")]
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
        [AR.ManualDiscountMode(typeof(POReceiptLine.curyDiscAmt), typeof(POReceiptLine.curyExtCost), typeof(POReceiptLine.discPct), typeof(POReceiptLine.freezeManualDisc))]
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
        [PXDBCurrency(typeof(POReceiptLine.curyInfoID), typeof(POReceiptLine.lineAmt))]
        [PXUIField(DisplayName = "Ext. Cost")]
        [PXFormula(typeof(Mult<POReceiptLine.receiptQty, POReceiptLine.curyUnitCost>))]
        [PXFormula(null, typeof(SumCalc<POLineR.receivedCost>))]
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
        [PXDBPriceCostCalced(typeof(Switch<Case<Where<POReceiptLine.receiptQty, Equal<decimal0>>, decimal0>, Div<Sub<POReceiptLine.curyExtCost, POReceiptLine.curyDiscAmt>, POReceiptLine.receiptQty>>), typeof(Decimal))]
        [PXFormula(typeof(Div<Sub<POReceiptLine.curyExtCost, POReceiptLine.curyDiscAmt>, NullIf<POReceiptLine.receiptQty, decimal0>>))]
        [PXCurrency(typeof(Search<CommonSetup.decPlPrcCst>), typeof(POReceiptLine.curyInfoID), typeof(POReceiptLine.discCost))]
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
        [PXDBPriceCostCalced(typeof(Switch<Case<Where<POReceiptLine.receiptQty, Equal<decimal0>>, decimal0>, Div<POReceiptLine.lineAmt, POReceiptLine.receiptQty>>), typeof(Decimal))]
        [PXFormula(typeof(Div<Row<POReceiptLine.lineAmt, POReceiptLine.curyLineAmt>, NullIf<POReceiptLine.receiptQty, decimal0>>))]
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

		[PXDBCurrency(typeof(POReceiptLine.curyInfoID), typeof(POReceiptLine.extCost), MinValue=0.0)]
		[PXUIField(DisplayName = "Amount", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
        [PXFormula(typeof(Sub<POReceiptLine.curyLineAmt, POReceiptLine.curyDiscAmt>))]
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
        #region CuryMultMultExtCost
        public abstract class curyMultExtCost : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryMultExtCost;
        [PXDBCurrency(typeof(POReceiptLine.curyInfoID), typeof(POReceiptLine.multExtCost))]
        [PXFormula(typeof(Mult<Sub<Mult<POReceiptLine.receiptQty, POReceiptLine.curyUnitCost>, POReceiptLine.curyDiscAmt>, POReceiptLine.invtMult>))]
        [PXFormula(null, typeof(SumCalc<POLineR.curyReceivedCost>))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? CuryMultExtCost
        {
            get
            {
                return this._CuryMultExtCost;
            }
            set
            {
                this._CuryMultExtCost = value;
            }
        }
        #endregion
        #region MultExtCost
        public abstract class multExtCost : PX.Data.IBqlField
        {
        }
        protected Decimal? _MultExtCost;

        [PXDBBaseCury()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? MultExtCost
        {
            get
            {
                return this._MultExtCost;
            }
            set
            {
                this._MultExtCost = value;
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
		[POReceiptTax(typeof(POReceipt), typeof(POReceiptTax), typeof(POReceiptTaxTran))]
		[POReceiptUnbilledTax(typeof(POReceipt), typeof(POReceiptTax), typeof(POReceiptTaxTran))]
        [PXSelector(typeof(TaxCategory.taxCategoryID), DescriptionField = typeof(TaxCategory.descr))]
        [PXRestrictor(typeof(Where<TaxCategory.active, Equal<True>>), TX.Messages.InactiveTaxCategory, typeof(TaxCategory.taxCategoryID))]
        [PXDefault(typeof(Search<InventoryItem.taxCategoryID,
			Where<InventoryItem.inventoryID, Equal<Current<POReceiptLine.inventoryID>>>>),
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
		#region ReasonCode
		public abstract class reasonCode : PX.Data.IBqlField
		{
		}
		protected String _ReasonCode;
		[PXDBString(CS.ReasonCode.reasonCodeID.Length, IsUnicode = true)]
		[PXSelector(typeof(Search<ReasonCode.reasonCodeID, Where<ReasonCode.usage, Equal<ReasonCodeUsages.issue>>>), DescriptionField = typeof(ReasonCode.descr))]
		[PXUIField(DisplayName = "Reason Code", Visible = false)]
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
		#region ExpenseAcctID
		public abstract class expenseAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _ExpenseAcctID;
		[Account(typeof(POReceiptLine.branchID),DisplayName = "Account", Visibility = PXUIVisibility.Visible, Filterable = false, DescriptionField = typeof(Account.description))]
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
        [SubAccount(typeof(POReceiptLine.branchID), typeof(POReceiptLine.expenseAcctID), DisplayName = "Sub.", Visibility = PXUIVisibility.Visible, Filterable = true)]
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
		public abstract class pOAccrualAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _POAccrualAcctID;
		[Account(typeof(POReceiptLine.branchID), DisplayName = "Accrual Account", Filterable = false, DescriptionField = typeof(Account.description))]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Int32? POAccrualAcctID
		{
			get
			{
				return this._POAccrualAcctID;
			}
			set
			{
				this._POAccrualAcctID = value;
			}
		}
		#endregion
		#region POAccrualSubID
		public abstract class pOAccrualSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _POAccrualSubID;
		[SubAccount(typeof(POReceiptLine.branchID), typeof(POReceiptLine.pOAccrualAcctID), DisplayName = "Accrual Sub.", Filterable = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Int32? POAccrualSubID
		{
			get
			{
				return this._POAccrualSubID;
			}
			set
			{
				this._POAccrualSubID = value;
			}
		}
		#endregion
		#region AlternateID
		public abstract class alternateID : PX.Data.IBqlField
		{
		}
		protected String _AlternateID;
		[PXDBString(50, IsUnicode = true, InputMask = "")]
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
		[PXUIField(DisplayName = "Transaction Descr.", Visibility = PXUIVisibility.Visible)]
		[PXDefault(typeof(Search<InventoryItem.descr, Where<InventoryItem.inventoryID, Equal<Current<POReceiptLine.inventoryID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
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
		[PXDefault(TypeCode.Decimal, "0.0", typeof(Search<InventoryItem.baseWeight, Where<InventoryItem.inventoryID, Equal<Current<POReceiptLine.inventoryID>>, And<InventoryItem.baseWeight, IsNotNull>>>))]
		[PXUIField(DisplayName = "Unit Weight")]
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
		[PXDefault(TypeCode.Decimal, "0.0", typeof(Search<InventoryItem.baseVolume, Where<InventoryItem.inventoryID, Equal<Current<POReceiptLine.inventoryID>>, And<InventoryItem.baseVolume, IsNotNull>>>))]
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
		[PXFormula(typeof(Mult<Row<POReceiptLine.baseReceiptQty,POReceiptLine.receiptQty>, POReceiptLine.unitWeight>), typeof(SumCalc<POReceipt.receiptWeight>))]
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
		[PXUIField(DisplayName = "Volume", Visibility = PXUIVisibility.SelectorVisible)]
		[PXFormula(typeof(Mult<Row<POReceiptLine.baseReceiptQty, POReceiptLine.receiptQty>, POReceiptLine.unitVolume>), typeof(SumCalc<POReceipt.receiptVolume>))]
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
		[POProjectDefault(typeof(POReceiptLine.lineType))]
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
		[ActiveProjectTask(typeof(POReceiptLine.projectID), BatchModule.PO, DisplayName = "Project Task")]
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
		#region VoucheredQty
		public abstract class voucheredQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _VoucheredQty;
		[PXDBQuantity(typeof(POReceiptLine.uOM), typeof(POReceiptLine.baseVoucheredQty),HandleEmptyKey = true)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Order Qty.", Visibility = PXUIVisibility.Visible)]
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

		[PXDBCurrency(typeof(POReceiptLine.curyInfoID), typeof(POReceiptLine.voucheredCost))]
		[PXUIField(DisplayName = "Vouchered Cost", Visibility = PXUIVisibility.SelectorVisible)]
		[PXFormula(typeof(Mult<POReceiptLine.voucheredQty, POReceiptLine.curyUnitCost>))]
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

		[PXDBBaseCury()]
		[PXDefault(typeof(POReceiptLine.unitCost))]
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
		#region BillPPVAmt
		public abstract class billPPVAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _BillPPVAmt;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? BillPPVAmt
		{
			get
			{
				return this._BillPPVAmt;
			}
			set
			{
				this._BillPPVAmt = value;
			}
		}
		#endregion
		#region ReversedBillPPVAmt
		public abstract class reversedBillPPVAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _ReversedBillPPVAmt;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? ReversedBillPPVAmt
		{
			get
			{
				return this._ReversedBillPPVAmt;
			}
			set
			{
				this._ReversedBillPPVAmt = value;
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
        #region SOOrderType
        public abstract class sOOrderType : PX.Data.IBqlField
        {
        }
        protected String _SOOrderType;
        [PXDBString(2, IsFixed = true)]
        [PXUIField(DisplayName = "Transfer Order Type", Enabled = false)]
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
        [PXUIField(DisplayName = "Transfer Order Nbr.", Enabled = false)]
        [PXSelector(typeof(Search<SO.SOOrder.orderNbr, Where<SO.SOOrder.orderType, Equal<Current<POReceiptLine.sOOrderType>>>>))]
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
        [PXUIField(DisplayName = "Transfer Line Nbr.", Enabled = false)]
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
		[PXUIField(DisplayName = "Transfer Shipment Nbr.", Enabled = false)]
		[PXSelector(typeof(Search<SO.SOShipment.shipmentNbr, Where<SO.SOShipment.shipmentType, Equal<Current<POReceiptLine.sOShipmentType>>>>))]
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

        #region OrigPlanType
        public abstract class origPlanType : PX.Data.IBqlField
        {
        }
        protected String _OrigPlanType;
        [PXDBString(2, IsFixed = true)]
        [PXSelector(typeof(Search<INPlanType.planType>), CacheGlobal = true)]
        public virtual String OrigPlanType
        {
            get
            {
                return this._OrigPlanType;
            }
            set
            {
                this._OrigPlanType = value;
            }
        }
        #endregion
		#region Released
		public abstract class released : PX.Data.IBqlField
		{
		}
		protected Boolean? _Released;
		[PXDBBool()]
		[PXUIField(DisplayName = "Released")]
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
		
		#region ExpireDate
		public abstract class expireDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ExpireDate;
		[POExpireDateAttribute(typeof(POReceiptLine.inventoryID), PersistingCheck = PXPersistingCheck.Nothing)]
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

		#region UnbilledQty
		public abstract class unbilledQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnbilledQty;
		[PXDBQuantity(typeof(POReceiptLine.uOM), typeof(POReceiptLine.baseUnbilledQty), HandleEmptyKey = true)]
		[PXFormula(typeof(POReceiptLine.receiptQty), typeof(SumCalc<POReceipt.unbilledQty>))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Unbilled Qty.", Enabled = false)]
		public virtual Decimal? UnbilledQty
		{
			get
			{
				return this._UnbilledQty;
			}
			set
			{
				this._UnbilledQty = value;
			}
		}
		#endregion
		#region BaseUnbilledQty
		public abstract class baseUnbilledQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseUnbilledQty;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? BaseUnbilledQty
		{
			get
			{
				return this._BaseUnbilledQty;
			}
			set
			{
				this._BaseUnbilledQty = value;
			}
		}
		#endregion
		#region CuryUnbilledAmt
		public abstract class curyUnbilledAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryUnbilledAmt;
		[PXUIField(DisplayName = "Unbilled Amount")]
		[PXDBCurrency(typeof(POReceiptLine.curyInfoID), typeof(POReceiptLine.unbilledAmt))]
		[PXFormula(typeof(POReceiptLine.curyLineAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryUnbilledAmt
		{
			get
			{
				return this._CuryUnbilledAmt;
			}
			set
			{
				this._CuryUnbilledAmt = value;
			}
		}
		#endregion
		#region UnbilledAmt
		public abstract class unbilledAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnbilledAmt;
		[PXUIField(DisplayName = "Unbilled Amount")]
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? UnbilledAmt
		{
			get
			{
				return this._UnbilledAmt;
			}
			set
			{
				this._UnbilledAmt = value;
			}
		}
        #endregion
        #region CuryUnbilledDiscountAmt
        public abstract class curyUnbilledDiscountAmt : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryUnbilledDiscountAmt;
        [PXDBCurrency(typeof(POReceiptLine.curyInfoID), typeof(POReceiptLine.unbilledDiscountAmt))]
        [PXFormula(typeof(POReceiptLine.curyDiscAmt))]

        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? CuryUnbilledDiscountAmt
        {
            get
            {
                return this._CuryUnbilledDiscountAmt;
            }
            set
            {
                this._CuryUnbilledDiscountAmt = value;
            }
        }
        #endregion
        #region UnbilledDiscountAmt
        public abstract class unbilledDiscountAmt : PX.Data.IBqlField
        {
        }
        protected Decimal? _UnbilledDiscountAmt;
        [PXDBBaseCury()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? UnbilledDiscountAmt
        {
            get
            {
                return this._UnbilledDiscountAmt;
            }
            set
            {
                this._UnbilledDiscountAmt = value;
            }
        }
		#endregion

		#region CuryUnbilledExtCost
		public abstract class curyUnbilledExtCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryUnbilledExtCost;
		[PXCurrency(typeof(POReceiptLine.curyInfoID), typeof(POReceiptLine.unbilledExtCost))]
		[PXFormula(typeof(Sub<POReceiptLine.curyUnbilledAmt, POReceiptLine.curyUnbilledDiscountAmt>))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryUnbilledExtCost
		{
			get
			{
				return this._CuryUnbilledExtCost;
			}
			set
			{
				this._CuryUnbilledExtCost = value;
			}
		}
		#endregion
		#region UnbilledExtCost
		public abstract class unbilledExtCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnbilledExtCost;
		[PXBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Decimal? UnbilledExtCost
		{
			get
			{
				return this._UnbilledExtCost;
			}
			set
			{
				this._UnbilledExtCost = value;
			}
		}
		#endregion

		#region IsSkItem
		public abstract class isStkItem : PX.Data.IBqlField
		{
		}
		[PXBool]
		[PXUIField(DisplayName = "Is stock", Visibility = PXUIVisibility.Invisible, Visible = false, Enabled = false)]
		[PXFormula(typeof(Selector<POReceiptLine.inventoryID, InventoryItem.stkItem>))]
		public virtual Boolean? IsStkItem
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
		[PXFormula(typeof(Selector<POReceiptLine.inventoryID, InventoryItem.kitItem>))]
		public virtual Boolean? IsKit
		{
			get;
			set;
		}
		#endregion

        #region LastBaseReceivedQty

        protected Decimal? _LastBaseReceivedQty;

		public virtual Decimal? LastBaseReceivedQty
		{
			get
			{
				return this._LastBaseReceivedQty;
			}
			set
			{
				this._LastBaseReceivedQty = value;
			}
		}
		#endregion

        #region DiscountID
        public abstract class discountID : PX.Data.IBqlField
        {
        }
        protected String _DiscountID;
        [PXDBString(10, IsUnicode = true)]
        [PXSelector(typeof(Search<APDiscount.discountID, Where<APDiscount.bAccountID, Equal<Current<POReceiptLine.vendorID>>, And<APDiscount.type, Equal<AR.DiscountType.LineDiscount>>>>))]
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
		#region IsLSEntryBlocked
		public abstract class isLSEntryBlocked : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsLSEntryBlocked;
		[PXBool()]
		public virtual Boolean? IsLSEntryBlocked
		{
			get
			{
				return this._IsLSEntryBlocked;
			}
			set
			{
				this._IsLSEntryBlocked = value;

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

		#region Methods
		public virtual bool IsStockItem() 
		{
			return (this.LineType == POLineType.GoodsForInventory || 
				this.LineType == POLineType.GoodsForSalesOrder ||
				this.LineType == POLineType.GoodsForReplenishment);
		}
		public virtual bool IsNonStockItem()
		{
			return (this.LineType == POLineType.NonStock ||
				this.LineType == POLineType.NonStockForSalesOrder);
		} 
		public static implicit operator POReceiptLineSplit(POReceiptLine item)
		{
			POReceiptLineSplit ret = new POReceiptLineSplit();
			ret.ReceiptType = item.ReceiptType;
			ret.ReceiptNbr = item.ReceiptNbr;
			ret.LineType = item.LineType;
			ret.LineNbr = item.LineNbr;
			ret.SplitLineNbr = (short)1;
			ret.InventoryID = item.InventoryID;
			ret.SiteID = item.SiteID;
			ret.SubItemID = item.SubItemID;
			ret.LocationID = item.LocationID;
			ret.LotSerialNbr = item.LotSerialNbr;
			ret.ExpireDate = item.ExpireDate;
			ret.Qty = item.Qty;
			ret.UOM = item.UOM;
			ret.ExpireDate= item.ExpireDate;
			ret.BaseQty = item.BaseQty;
			ret.InvtMult = item.InvtMult;
            ret.OrigPlanType = item.OrigPlanType;

			return ret;
		}
		public static implicit operator POReceiptLine(POReceiptLineSplit item)
		{
			POReceiptLine ret = new POReceiptLine();
			ret.ReceiptNbr = item.ReceiptNbr;
			ret.LineNbr = item.LineNbr;
			ret.InventoryID = item.InventoryID;
			ret.SiteID = item.SiteID;
			ret.SubItemID = item.SubItemID;
			ret.LocationID = item.LocationID;
			ret.LotSerialNbr = item.LotSerialNbr;
			ret.Qty = item.Qty;
			ret.UOM = item.UOM;
			ret.ExpireDate = item.ExpireDate;
			ret.BaseQty = item.BaseQty;
			ret.InvtMult = item.InvtMult;
            ret.OrigPlanType = item.OrigPlanType;
			return ret;
		}
		
		#endregion
	}
}
