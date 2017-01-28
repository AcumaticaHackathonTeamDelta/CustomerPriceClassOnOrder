namespace PX.Objects.PO
{
	using System;
	using PX.Data;
	using PX.Objects.AP;
	using PX.Objects.CS;
	using PX.Objects.CR;
	using PX.Objects.CM;
	using PX.Objects.IN;
	using PX.Objects.GL;
	using CRLocation = PX.Objects.CR.Standalone.Location;
	
	[System.SerializableAttribute()]
	[PXPrimaryGraph(typeof(POReceiptEntry))]
	[PXEMailSource]
	[PXCacheName(Messages.POReceipt)]
	public partial class POReceipt : PX.Data.IBqlTable, PX.Data.EP.IAssign
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
		#region ReceiptType
		public abstract class receiptType : PX.Data.IBqlField
		{
		}
		protected String _ReceiptType;
		[PXDBString(2, IsFixed = true, IsKey = true,InputMask="")]
		[PXDefault(POReceiptType.POReceipt)]
		[POReceiptType.List()]
		[PXUIField(DisplayName = "Type")]
        [PX.Data.EP.PXFieldDescription]
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
			public const string DisplayName = "Receipt Nbr.";
		}
		protected String _ReceiptNbr;
		[PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXDefault()]
		[POReceiptType.Numbering()]
		[POReceiptType.RefNbr(typeof(Search2<POReceipt.receiptNbr,
			LeftJoinSingleTable<Vendor, On<Vendor.bAccountID, Equal<POReceipt.vendorID>>>,
			Where<POReceipt.receiptType, Equal<Optional<POReceipt.receiptType>>,
			  And<Where<Vendor.bAccountID, IsNull,
				 Or<Match<Vendor, Current<AccessInfo.userName>>>>>>,
			OrderBy<Desc<POReceipt.receiptNbr>>>), Filterable = true)]
		[PXUIField(DisplayName = receiptNbr.DisplayName, Visibility = PXUIVisibility.SelectorVisible)]
		[PX.Data.EP.PXFieldDescription]
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
        #region SiteID
        public abstract class siteID : IBqlField { }
        [IN.Site(DisplayName = "Warehouse")]
        public int? SiteID
        {
            get;
            set;
        }
        #endregion
        #region VendorID
        public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[Vendor(typeof(Search<BAccountR.bAccountID,
			Where<BAccountR.type, Equal<BAccountType.companyType>, Or<Vendor.type, NotEqual<BAccountType.employeeType>>>>), Visibility = PXUIVisibility.SelectorVisible, CacheGlobal = true, Filterable = true)]
		[PXRestrictor(typeof(Where<Vendor.status, IsNull, 
								Or<Vendor.status, Equal<BAccount.status.active>, 
				  		        Or<Vendor.status, Equal<BAccount.status.oneTime>>>>), AP.Messages.VendorIsInStatus, typeof(Vendor.status))]
		[PXDefault()]	
        [PXFormula(typeof(Switch<Case<Where<POReceipt.receiptType, Equal<POReceiptType.transferreceipt>>, Selector<POReceipt.siteID, Selector<INSite.branchID, Branch.branchCD>>>, POReceipt.vendorID>))]
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
		[LocationID(typeof(Where<Location.bAccountID, Equal<Current<POReceipt.vendorID>>,
			And<Location.isActive, Equal<True>,
			And<MatchWithBranch<Location.vBranchID>>>>), DescriptionField = typeof(Location.descr), Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(typeof(Coalesce<Search2<BAccountR.defLocationID,
			InnerJoin<CRLocation, On<CRLocation.bAccountID, Equal<BAccountR.bAccountID>, And<CRLocation.locationID, Equal<BAccountR.defLocationID>>>>,
			Where<BAccountR.bAccountID, Equal<Current<POReceipt.vendorID>>,
				And<CRLocation.isActive, Equal<True>,
				And<MatchWithBranch<CRLocation.vBranchID>>>>>,
			Search<CRLocation.locationID,
			Where<CRLocation.bAccountID, Equal<Current<POReceipt.vendorID>>,
			And<CRLocation.isActive, Equal<True>, And<MatchWithBranch<CRLocation.vBranchID>>>>>>))]
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
        #region BranchID
        public abstract class branchID : PX.Data.IBqlField
        {
        }
        protected Int32? _BranchID;
        [GL.Branch(typeof(Search<Branch.branchID, Where<Branch.branchID, Equal<Current<AccessInfo.branchID>>>>), IsDetail = false)]
        [PXFormula(typeof(Switch<Case<Where<POReceipt.receiptType, Equal<POReceiptType.transferreceipt>>, Selector<POReceipt.siteID, Selector<INSite.branchID, Branch.branchCD>>, Case<Where<Selector<POReceipt.vendorLocationID, Location.vBranchID>, IsNotNull>, Selector<POReceipt.vendorLocationID, Selector<Location.vBranchID, Branch.branchCD>>>>, POReceipt.branchID>))]
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
		#region ReceiptDate
		public abstract class receiptDate : PX.Data.IBqlField
		{
			public const string DisplayName = "Date";
		}
		protected DateTime? _ReceiptDate;
		[PXDBDate()]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = receiptDate.DisplayName, Visibility = PXUIVisibility.SelectorVisible)]
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
		#region InvoiceDate
		public abstract class invoiceDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _InvoiceDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Bill Date", Visibility = PXUIVisibility.SelectorVisible)]
		[PXFormula(typeof(POReceipt.receiptDate))]
		[PXDefault(typeof(POReceipt.receiptDate),PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual DateTime? InvoiceDate
		{
			get
			{
				return this._InvoiceDate;
			}
			set
			{
				this._InvoiceDate = value;
			}
		}
		#endregion
		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected String _FinPeriodID;
		[POOpenPeriod(typeof(POReceipt.receiptDate))]
		[PXUIField(DisplayName = "Post Period", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region Hold
		public abstract class hold : PX.Data.IBqlField
		{
		}
		protected Boolean? _Hold;
		[PXDBBool()]
		[PXUIField(DisplayName = "Hold", Visibility = PXUIVisibility.Visible)]
		[PXDefault(true, typeof(Search<POSetup.holdReceipts>))]		
		public virtual Boolean? Hold
		{
			get
			{
				return this._Hold;
			}
			set
			{
				this._Hold = value;
				this.SetStatus();
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
		[PXDefault(false,PersistingCheck =PXPersistingCheck.Nothing)]
		public virtual Boolean? Released
		{
			get
			{
				return this._Released;
			}
			set
			{
				this._Released = value;
				this.SetStatus();
			}
		}
		#endregion
		#region IsTaxValid
		public abstract class isTaxValid : PX.Data.IBqlField
		{
		}
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Tax is up to date", Enabled = false)]
		public virtual Boolean? IsTaxValid
		{
			get;
			set;
		}
		#endregion
		#region IsUnbilledTaxValid
		public abstract class isUnbilledTaxValid : PX.Data.IBqlField
		{
		}
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? IsUnbilledTaxValid
		{
			get;
			set;
		}
		#endregion
		#region Status
		public abstract class status : PX.Data.IBqlField
		{
		}
		protected String _Status;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(POReceiptStatus.Hold)]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[POReceiptStatus.List()]
		public virtual String Status
		{
			[PXDependsOnFields(typeof(released), typeof(hold))]
			get
			{
				return this._Status;
			}
			set
			{
				//this._Status = value;
			}
		}
		#endregion
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXSearchable(SM.SearchCategory.PO, "{0}: {1} - {3}", new Type[] { typeof(POReceipt.pOType), typeof(POReceipt.receiptNbr), typeof(POReceipt.vendorID), typeof(Vendor.acctName) },
		   new Type[] { typeof(POReceipt.invoiceNbr) },
		   NumberFields = new Type[] { typeof(POReceipt.receiptNbr) },
		   Line1Format = "{0:d}{1}{2}", Line1Fields = new Type[] { typeof(POReceipt.receiptDate), typeof(POReceipt.status), typeof(POReceipt.invoiceNbr) },
		   Line2Format = "{0}", Line2Fields = new Type[] { typeof(POReceipt.orderQty), typeof(POReceipt.orderTotal) },
		   MatchWithJoin = typeof(InnerJoin<Vendor, On<Vendor.bAccountID, Equal<POReceipt.vendorID>>>),
		   SelectForFastIndexing = typeof(Select2<POReceipt, InnerJoin<Vendor, On<POReceipt.vendorID, Equal<Vendor.bAccountID>>>>)
		)]
		[PXNote(new Type[0], ShowInReferenceSelector = true)]
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
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL")]
		[PXUIField(DisplayName = "Currency", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(typeof(Search<GL.Company.baseCuryID>))]
		[PXSelector(typeof(Currency.curyID))]		
		public virtual String CuryID
		{
			get
			{
				return this._CuryID;
			}
			set
			{
				this._CuryID = value;
			}
		}
		#endregion
		#region CuryInfoID
		public abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _CuryInfoID;
		[PXDBLong()]
		[CurrencyInfo(ModuleCode = "PO")]		
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
		#region LineCntr
		public abstract class lineCntr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineCntr;
		[PXDBInt()]
		[PXDefault(0)]		
		public virtual Int32? LineCntr
		{
			get
			{
				return this._LineCntr;
			}
			set
			{
				this._LineCntr = value;
			}
		}
		#endregion
        #region CuryLineTotal
        public abstract class curyLineTotal : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryLineTotal;

        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXDBCurrency(typeof(POReceipt.curyInfoID), typeof(POReceipt.lineTotal))]
        [PXUIField(DisplayName = "Lines Total", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
        public virtual Decimal? CuryLineTotal
        {
            get
            {
                return this._CuryLineTotal;
            }
            set
            {
                this._CuryLineTotal = value;
            }
        }
        #endregion
        #region LineTotal
        public abstract class lineTotal : PX.Data.IBqlField
        {
        }
        protected Decimal? _LineTotal;
        [PXDBBaseCury()]
        [PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Line Total")]
		public virtual Decimal? LineTotal
        {
            get
            {
                return this._LineTotal;
            }
            set
            {
                this._LineTotal = value;
            }
        }
        #endregion

        #region CuryDiscTot
        public abstract class curyDiscTot : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryDiscTot;
        [PXDBCurrency(typeof(POReceipt.curyInfoID), typeof(POReceipt.discTot))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Discount Total", Enabled = true)]
        public virtual Decimal? CuryDiscTot
        {
            get
            {
                return this._CuryDiscTot;
            }
            set
            {
                this._CuryDiscTot = value;
            }
        }
		#endregion
		#region DiscTot
		public abstract class discTot : PX.Data.IBqlField
		{
		}
		protected Decimal? _DiscTot;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? DiscTot
		{
			get
			{
				return this._DiscTot;
			}
			set
			{
				this._DiscTot = value;
			}
		}
		#endregion
		#region CuryUnbilledDiscTot
		public abstract class curyUnbilledDiscTot : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryUnbilledDiscTot;
		[PXCurrency(typeof(POReceipt.curyInfoID), typeof(POReceipt.unbilledDiscTot))]
		[PXFormula(typeof(Mult<Switch<Case<Where<POReceipt.orderQty, Greater<decimal0>>, Div<POReceipt.unbilledQty, POReceipt.orderQty>>, decimal1>, POReceipt.curyDiscTot>))]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Decimal? CuryUnbilledDiscTot
		{
			get
			{
				return this._CuryUnbilledDiscTot;
			}
			set
			{
				this._CuryUnbilledDiscTot = value;
			}
		}
		#endregion
		#region UnbilledDiscTot
		public abstract class unbilledDiscTot : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnbilledDiscTot;
		[PXBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Decimal? UnbilledDiscTot
		{
			get
			{
				return this._UnbilledDiscTot;
			}
			set
			{
				this._UnbilledDiscTot = value;
			}
		}
		#endregion
		#region DocDisc
		public abstract class docDisc : PX.Data.IBqlField
        {
        }
        protected Decimal? _DocDisc;
        [PXBaseCury()]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? DocDisc
        {
            get
            {
                return this._DocDisc;
            }
            set
            {
                this._DocDisc = value;
            }
        }
        #endregion
        #region CuryDocDisc
        public abstract class curyDocDisc : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryDocDisc;
        [PXCurrency(typeof(POReceipt.curyInfoID), typeof(POReceipt.docDisc))]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Document Discount", Enabled = true)]
        public virtual Decimal? CuryDocDisc
        {
            get
            {
                return this._CuryDocDisc;
            }
            set
            {
                this._CuryDocDisc = value;
            }
        }
        #endregion

		#region CuryOrderTotal
		public abstract class curyOrderTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryOrderTotal;

		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(POReceipt.curyInfoID), typeof(POReceipt.orderTotal))]
		[PXUIField(DisplayName = "Total Amt.", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual Decimal? CuryOrderTotal
		{
			get
			{
				return this._CuryOrderTotal;
			}
			set
			{
				this._CuryOrderTotal = value;
			}
		}
		#endregion
		#region OrderTotal
		public abstract class orderTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _OrderTotal;
		
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? OrderTotal
		{
			get
			{
				return this._OrderTotal;
			}
			set
			{
				this._OrderTotal = value;
			}
		}
		#endregion
		#region OrderQty
		public abstract class orderQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _OrderQty;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Total Qty.")]
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
		#region CuryTaxTotal
		public abstract class curyTaxTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryTaxTotal;
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(POReceipt.curyInfoID), typeof(POReceipt.taxTotal))]
		[PXUIField(DisplayName = "Tax Total", Enabled = false)]
		public virtual Decimal? CuryTaxTotal
		{
			get
			{
				return this._CuryTaxTotal;
			}
			set
			{
				this._CuryTaxTotal = value;
			}
		}
		#endregion
		#region TaxTotal
		public abstract class taxTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _TaxTotal;

		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Tax Total")]
		public virtual Decimal? TaxTotal
		{
			get
			{
				return this._TaxTotal;
			}
			set
			{
				this._TaxTotal = value;
			}
		}
		#endregion
        
        #region CuryVatExemptTotal
        public abstract class curyVatExemptTotal : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryVatExemptTotal;
        [PXDBCurrency(typeof(POReceipt.curyInfoID), typeof(POReceipt.vatExemptTotal))]
        [PXUIField(DisplayName = "VAT Exempt Total", Visibility = PXUIVisibility.Visible, Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? CuryVatExemptTotal
        {
            get
            {
                return this._CuryVatExemptTotal;
            }
            set
            {
                this._CuryVatExemptTotal = value;
            }
        }
        #endregion

        #region VatExemptTaxTotal
        public abstract class vatExemptTotal : PX.Data.IBqlField
        {
        }
        protected Decimal? _VatExemptTotal;
        [PXDBDecimal(4)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? VatExemptTotal
        {
            get
            {
                return this._VatExemptTotal;
            }
            set
            {
                this._VatExemptTotal = value;
            }
        }
        #endregion
                
        #region CuryVatTaxableTotal
        public abstract class curyVatTaxableTotal : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryVatTaxableTotal;
        [PXDBCurrency(typeof(POReceipt.curyInfoID), typeof(POReceipt.vatTaxableTotal))]
        [PXUIField(DisplayName = "VAT Taxable Total", Visibility = PXUIVisibility.Visible, Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? CuryVatTaxableTotal
        {
            get
            {
                return this._CuryVatTaxableTotal;
            }
            set
            {
                this._CuryVatTaxableTotal = value;
            }
        }
        #endregion

        #region VatTaxableTotal
        public abstract class vatTaxableTotal : PX.Data.IBqlField
        {
        }
        protected Decimal? _VatTaxableTotal;
        [PXDBDecimal(4)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? VatTaxableTotal
        {
            get
            {
                return this._VatTaxableTotal;
            }
            set
            {
                this._VatTaxableTotal = value;
            }
        }
        #endregion

		#region TaxZoneID
		public abstract class taxZoneID : PX.Data.IBqlField
		{
		}
		protected String _TaxZoneID;

		[PXDBString(10, IsUnicode = true)]
		[PXDefault(typeof(Search<Location.vTaxZoneID, Where<Location.bAccountID, Equal<Current<POReceipt.vendorID>>, And<Location.locationID, Equal<Current<POReceipt.vendorLocationID>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Vendor Tax Zone", Visibility = PXUIVisibility.Visible)]
		[PXSelector(typeof(TX.TaxZone.taxZoneID), DescriptionField = typeof(TX.TaxZone.descr), Filterable = true)]
		[PXRestrictor(typeof(Where<TX.TaxZone.isManualVATZone, Equal<False>>), TX.Messages.CantUseManualVAT)]
		public virtual String TaxZoneID
		{
			get
			{
				return this._TaxZoneID;
			}
			set
			{
				this._TaxZoneID = value;
			}
		}
		#endregion
		#region TermsID
		public abstract class termsID : PX.Data.IBqlField
		{
		}
		protected String _TermsID;

		[PXDBString(10, IsUnicode = true)]
		[PXDefault(typeof(Search<Vendor.termsID, Where<Vendor.bAccountID, Equal<Current<POReceipt.vendorID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Terms", Visibility = PXUIVisibility.Visible)]
		[Terms(typeof(POReceipt.invoiceDate), typeof(POReceipt.dueDate), typeof(POReceipt.discDate), typeof(POReceipt.curyOrderTotal), typeof(POReceipt.curyDiscAmt))]
		[PXSelector(typeof(Search<Terms.termsID, Where<Terms.visibleTo, Equal<TermsVisibleTo.all>, Or<Terms.visibleTo, Equal<TermsVisibleTo.vendor>>>>), DescriptionField = typeof(Terms.descr), Filterable = true)]
		public virtual String TermsID
		{
			get
			{
				return this._TermsID;
			}
			set
			{
				this._TermsID = value;
			}
		}
		#endregion
		#region CuryControlTotal
		public abstract class curyControlTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryControlTotal;
		[PXDBCurrency(typeof(POReceipt.curyInfoID), typeof(POReceipt.controlTotal))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Control Amt.")]
		public virtual Decimal? CuryControlTotal
		{
			get
			{
				return this._CuryControlTotal;
			}
			set
			{
				this._CuryControlTotal = value;
			}
		}
		#endregion
		#region ControlTotal
		public abstract class controlTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _ControlTotal;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? ControlTotal
		{
			get
			{
				return this._ControlTotal;
			}
			set
			{
				this._ControlTotal = value;
			}
		}
		#endregion
		#region ControlQty
		public abstract class controlQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _ControlQty;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName="Control Qty.")]
		public virtual Decimal? ControlQty
		{
			get
			{
				return this._ControlQty;
			}
			set
			{
				this._ControlQty = value;
			}
		}
		#endregion
		#region APDocType
		public abstract class aPDocType : PX.Data.IBqlField
		{
		}
		protected String _APDocType;
		[PXDBString(3,IsFixed = true)]
		[PXDefault(PersistingCheck=PXPersistingCheck.Nothing)]
		[APDocType.List()]
		[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = true)]		
		public virtual String APDocType
		{
			get
			{
				return this._APDocType;
			}
			set
			{
				this._APDocType = value;
			}
		}
		#endregion
		#region APRefNbr
		public abstract class aPRefNbr : PX.Data.IBqlField
		{
		}
		protected String _APRefNbr;
		[PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible, TabOrder = 1)]
		[APInvoiceType.RefNbr(typeof(Search2<AP.Standalone.APRegisterAlias.refNbr,
			InnerJoinSingleTable<APInvoice, On<APInvoice.docType, Equal<AP.Standalone.APRegisterAlias.docType>,
				And<APInvoice.refNbr, Equal<AP.Standalone.APRegisterAlias.refNbr>>>,
			InnerJoin<Vendor, On<AP.Standalone.APRegisterAlias.vendorID, Equal<Vendor.bAccountID>>>>,
			Where<AP.Standalone.APRegisterAlias.docType, Equal<Current<POReceipt.aPDocType>>,
			And<Vendor.bAccountID,Equal<Current<POReceipt.vendorID>>>>>), Filterable = true)]
		
		public virtual String APRefNbr
		{
			get
			{
				return this._APRefNbr;
			}
			set
			{
				this._APRefNbr = value;
			}
		}
		#endregion
		#region CuryDiscAmt
		public abstract class curyDiscAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryDiscAmt;

		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(POReceipt.curyInfoID), typeof(POReceipt.discAmt))]
		[PXUIField(DisplayName = "Cash Discount", Visibility = PXUIVisibility.SelectorVisible)]
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
		[PXDBBaseCury()]
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
		#region DueDate
		public abstract class dueDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _DueDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Due Date", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual DateTime? DueDate
		{
			get
			{
				return this._DueDate;
			}
			set
			{
				this._DueDate = value;
			}
		}
		#endregion
		#region DiscDate
		public abstract class discDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _DiscDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Cash Discount Date", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual DateTime? DiscDate
		{
			get
			{
				return this._DiscDate;
			}
			set
			{
				this._DiscDate = value;
			}
		}
		#endregion
		#region InvoiceNbr
		public abstract class invoiceNbr : PX.Data.IBqlField
		{
		}
		protected String _InvoiceNbr;
		[PXDBString(40, IsUnicode = true)]
		[PXDefault(PersistingCheck=PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Vendor Ref.", Visibility = PXUIVisibility.SelectorVisible)]
		[POVendorRefNbr()]
		public virtual String InvoiceNbr
		{
			get
			{
				return this._InvoiceNbr;
			}
			set
			{
				this._InvoiceNbr = value;
			}
		}
		#endregion
		#region AutoCreateInvoice
		public abstract class autoCreateInvoice : PX.Data.IBqlField
		{
		}
		protected Boolean? _AutoCreateInvoice;
		[PXDBBool()]
		[PXUIField(DisplayName = "Create Bill", Visibility = PXUIVisibility.Visible)]
        [PXFormula(typeof(Switch<Case<Where<POReceipt.receiptType, Equal<POReceiptType.transferreceipt>>, False>, Current<POSetup.autoCreateInvoiceOnReceipt>>))]
        [PXDefault()]
		public virtual Boolean? AutoCreateInvoice
		{
			get
			{
				return this._AutoCreateInvoice;
			}
			set
			{
				this._AutoCreateInvoice = value;
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
		#region VendorID_Vendor_acctName
		public abstract class vendorID_Vendor_acctName : PX.Data.IBqlField
		{
		}
		#endregion
		#region WorkgroupID
		public abstract class workgroupID : PX.Data.IBqlField
		{
		}
		protected int? _WorkgroupID;
		[PXDBInt]
		[PXDefault(typeof(Vendor.workgroupID), PersistingCheck = PXPersistingCheck.Nothing)]
		[PX.TM.PXCompanyTreeSelector]
		[PXUIField(DisplayName = "Workgroup", Visibility = PXUIVisibility.Visible)]
		public virtual int? WorkgroupID
		{
			get
			{
				return this._WorkgroupID;
			}
			set
			{
				this._WorkgroupID = value;
			}
		}
		#endregion
		#region OwnerID
		public abstract class ownerID : IBqlField
		{
		}
		protected Guid? _OwnerID;
		[PXDBGuid()]
		[PXDefault(typeof(Vendor.ownerID), PersistingCheck = PXPersistingCheck.Nothing)]
		[PX.TM.PXOwnerSelector(typeof(POReceipt.workgroupID))]
		[PXUIField(DisplayName = "Owner", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Guid? OwnerID
		{
			get
			{
				return this._OwnerID;
			}
			set
			{
				this._OwnerID = value;
			}
		}
		#endregion

		#region CuryUnbilledOrderTotal
		public abstract class curyUnbilledTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryUnbilledTotal;
		[PXDBCurrency(typeof(POReceipt.curyInfoID), typeof(POReceipt.unbilledTotal))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Unbilled Balance", Enabled = false)]
		public virtual Decimal? CuryUnbilledTotal
		{
			get
			{
				return this._CuryUnbilledTotal;
			}
			set
			{
				this._CuryUnbilledTotal = value;
			}
		}
		#endregion
		#region UnbilledTotal
		public abstract class unbilledTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnbilledTotal;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? UnbilledTotal
		{
			get
			{
				return this._UnbilledTotal;
			}
			set
			{
				this._UnbilledTotal = value;
			}
		}
		#endregion
		#region CuryUnbilledLineTotal
		public abstract class curyUnbilledLineTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryUnbilledLineTotal;
		[PXDBCurrency(typeof(POReceipt.curyInfoID), typeof(POReceipt.unbilledLineTotal))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Unbilled Line Total", Enabled = false)]
		public virtual Decimal? CuryUnbilledLineTotal
		{
			get
			{
				return this._CuryUnbilledLineTotal;
			}
			set
			{
				this._CuryUnbilledLineTotal = value;
			}
		}
		#endregion
		#region UnbilledLineTotal
		public abstract class unbilledLineTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnbilledLineTotal;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? UnbilledLineTotal
		{
			get
			{
				return this._UnbilledLineTotal;
			}
			set
			{
				this._UnbilledLineTotal = value;
			}
		}
		#endregion
		#region CuryUnbilledTaxTotal
		public abstract class curyUnbilledTaxTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryUnbilledTaxTotal;
		[PXDBCurrency(typeof(POReceipt.curyInfoID), typeof(POReceipt.unbilledTaxTotal))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Unbilled Tax Total", Enabled = false)]
		public virtual Decimal? CuryUnbilledTaxTotal
		{
			get
			{
				return this._CuryUnbilledTaxTotal;
			}
			set
			{
				this._CuryUnbilledTaxTotal = value;
			}
		}
		#endregion
		#region UnbilledTaxTotal
		public abstract class unbilledTaxTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnbilledTaxTotal;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? UnbilledTaxTotal
		{
			get
			{
				return this._UnbilledTaxTotal;
			}
			set
			{
				this._UnbilledTaxTotal = value;
			}
		}
		#endregion
		#region UnbilledOrderQty
		public abstract class unbilledQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnbilledQty;
		[PXDBQuantity()]
		[PXUIField(DisplayName = "Unbilled Quantity")]
		[PXDefault(TypeCode.Decimal, "0.0")]
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
		#region ReceiptWeight
		public abstract class receiptWeight : PX.Data.IBqlField
		{
		}
		protected Decimal? _ReceiptWeight;
		[PXDBDecimal(6)]
		[PXUIField(DisplayName = "Weight", Visible = false)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? ReceiptWeight
		{
			get
			{
				return this._ReceiptWeight;
			}
			set
			{
				this._ReceiptWeight = value;
			}
		}
		#endregion
		#region ReceiptVolume
		public abstract class receiptVolume : PX.Data.IBqlField
		{
		}
		protected Decimal? _ReceiptVolume;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Volume", Visible = false)]
		public virtual Decimal? ReceiptVolume
		{
			get
			{
				return this._ReceiptVolume;
			}
			set
			{
				this._ReceiptVolume = value;
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
		[PXUIField(DisplayName = "Order Type")]
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
		#region ShipToBAccountID
		public abstract class shipToBAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _ShipToBAccountID;
		[PXDBInt()]
		[PXUIField(DisplayName = "Ship To")]		
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
		public abstract class shipToLocationID : PX.Data.IBqlField
		{
		}
		protected Int32? _ShipToLocationID;
		[LocationID(typeof(Where<Location.bAccountID, Equal<Current<POReceipt.shipToBAccountID>>>), DescriptionField = typeof(Location.descr))]
		
		[PXUIField(DisplayName = "Shipping Location")]
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

		#region Methods
		protected virtual void SetStatus()
		{
			if (this._Released != null && (bool)this._Released)
			{
				this._Status = POReceiptStatus.Released;
			}
			else if (this._Hold != null && (bool)this._Hold)
			{
				this._Status = POReceiptStatus.Hold;
			}
			else
			{
				this._Status = POReceiptStatus.Balanced;
			}
		}

		public virtual string GetAPDocType() 
		{
			return (this.ReceiptType == POReceiptType.POReceipt ? AP.APDocType.Invoice : AP.APDocType.DebitAdj);
		}
		#endregion

		#region IAssign Members

		int? PX.Data.EP.IAssign.WorkgroupID
		{
			get
			{
				return WorkgroupID; 
			}
			set
			{
				WorkgroupID = value; 
			}
		}

		Guid? PX.Data.EP.IAssign.OwnerID
		{
			get
			{
				return OwnerID; 
			}
			set
			{
				OwnerID = value; 
			}
		}

		#endregion
	}


	public static class POReceiptType
	{
        /// <summary>
        /// Specialized selector for POReceipt ReceiptNbr.<br/>
        /// By default, defines the following set of columns for the selector:<br/>
        /// POReceipt.receiptNbr, receiptDate,<br/>
        /// status, vendorID, vendorID_Vendor_acctName,<br/>
        /// vendorLocationID, curyID, curyOrderTotal,<br/>
        /// <example>
        /// [POReceiptType.RefNbr(typeof(Search2<POReceipt.receiptNbr,
		/// 	LeftJoin<Vendor, On<Vendor.bAccountID, Equal<POReceipt.vendorID>>>,
		/// 	Where<POReceipt.receiptType, Equal<Optional<POReceipt.receiptType>>,
		///       And<Where<Vendor.bAccountID, IsNull,
		/// 	  Or<Match<Vendor, Current<AccessInfo.userName>>>>>>>), Filterable = true)]
        /// </example>
        /// </summary>            
		public class RefNbrAttribute : PXSelectorAttribute
		{
            /// <summary>
            /// Default Ctor
            /// </summary>
            /// <param name="SearchType"> Must be IBqlSearch type, pointing to POReceipt.refNbr</param>
            public RefNbrAttribute(Type SearchType)
				: base(SearchType,
				typeof(POReceipt.receiptNbr),
                typeof(POReceipt.invoiceNbr),
                typeof(POReceipt.receiptDate),
				typeof(POReceipt.vendorID),
				typeof(POReceipt.vendorID_Vendor_acctName),
				typeof(POReceipt.vendorLocationID),
				typeof(POReceipt.status),
				typeof(POReceipt.curyID),
				typeof(POReceipt.curyOrderTotal))
			{
			}
		}

        /// <summary>
        /// Specialized version of the AutoNumber attribute for POReceipts<br/>
        /// It defines how the new numbers are generated for the PO Receipt. <br/>
        /// References POReceipt.receiptDate fields of the document,<br/>
        /// and also define a link between  numbering ID's defined in PO Setup:<br/>
        /// namely POSetup.receiptNumberingID for any receipt types<br/>        
        /// </summary>		
		public class NumberingAttribute: AutoNumberAttribute
		{
			public NumberingAttribute()
				: base(typeof(POSetup.receiptNumberingID), typeof(POReceipt.receiptDate)) { ; }
		}

		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { POReceipt, POReturn, TransferReceipt },
				new string[] { "Receipt", "Return", "Transfer Receipt" }) { }
		}

        public const string TransferReceipt = "RX";
		public const string POReceipt = "RT";
		public const string POReturn = "RN";
		
		public class poreceipt : Constant<string>
		{
			public poreceipt() : base(POReceipt) { ;}
		}

		public class poreturn: Constant<string>
		{
			public poreturn() : base(POReturn) { ;}
		}

        public class transferreceipt : Constant<string>
        {
            public transferreceipt() : base(TransferReceipt) { ;}
        }

		public static string GetINTranType(string aReceiptType)
		{
			string planType = string.Empty;
			switch (aReceiptType)
			{
				case POReceipt:
                case TransferReceipt:
                    return INTranType.Receipt;
				case POReturn: 
                    return INTranType.Issue;
			}
			return planType;

		}

        public static string GetINDocType(string aReceiptType)
        {
            string planType = string.Empty;
            switch (aReceiptType)
            {
                case POReceipt:
                case TransferReceipt:
                    return INDocType.Receipt;
                case POReturn:
                    return INDocType.Issue;
            }
            return planType;

        }

		public static Int16? InvtMult(string aReceiptType) 
		{
            return (aReceiptType == POReceiptType.POReceipt || aReceiptType == POReceiptType.TransferReceipt ? (Int16?)1 : (aReceiptType == POReceiptType.POReturn ? (Int16?)-1 : null));
		}


	}

	public class POReceiptStatus
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Hold,Balanced ,Released},
				new string[] { Messages.Hold, Messages.Balanced, Messages.Released}) { ; }
		}

		public const string Hold = "H";
		public const string Balanced = "B";		
		public const string Released = "R";
		


		public class hold : Constant<string>
		{
			public hold() : base(Hold) { ;}
		}

		public class balanced : Constant<string>
		{
			public balanced() : base(Balanced) { ;}
		}

		public class released : Constant<string>
		{
			public released() : base(Released) { ;}
		}

		
	}


    public partial class POReceiptBillingReport : PX.Data.IBqlTable
    {
        #region ReceiptNbr
        public abstract class receiptNbr : PX.Data.IBqlField
        {
        }
        protected String _ReceiptNbr;
        [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXDefault()]
        [POReceiptType.Numbering()]
        [POReceiptType.RefNbr(typeof(Search2<POReceipt.receiptNbr,
			LeftJoinSingleTable<Vendor, On<Vendor.bAccountID, Equal<POReceipt.vendorID>>>,
            Where<POReceipt.receiptType, Equal<Optional<POReceipt.receiptType>>, And<POReceipt.released, Equal<True>,
              And<Where<Vendor.bAccountID, IsNull,
                 Or<Match<Vendor, Current<AccessInfo.userName>>>>>>>,
            OrderBy<Desc<POReceipt.receiptNbr>>>), Filterable = true)]
        [PXUIField(DisplayName = "Receipt Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
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
    }
}
