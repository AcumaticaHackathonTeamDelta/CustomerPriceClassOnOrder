namespace PX.Objects.PO
{
	using System;
	using PX.Data;
	using PX.Objects.CM;
	using PX.Objects.AP;
	using PX.Objects.CS;
	using PX.Objects.CR;
	using PX.Objects.TX;
	using PX.Objects.GL;
    using CRLocation = PX.Objects.CR.Standalone.Location;


    [System.SerializableAttribute()]
    [PXCacheName(Messages.LandedCostTran)]
	public partial class LandedCostTran : PX.Data.IBqlTable
	{
		#region LCTranID
		public abstract class lCTranID : PX.Data.IBqlField
		{
		}
		protected Int32? _LCTranID;
		[PXDBIdentity(IsKey = true)]
		[PXUIField(Enabled = false,Visible=false, DisplayName = "Landed Cost Tran. ID")]
		public virtual Int32? LCTranID
		{
			get
			{
				return this._LCTranID;
			}
			set
			{
				this._LCTranID = value;
			}
		}
		#endregion
		#region Source
		public abstract class source : PX.Data.IBqlField
		{
		}
		protected String _Source;
		[PXDBString(2, IsFixed = true)]
		[PXDefault(LandedCostTranSource.FromPO)]
		[PXUIField(DisplayName = "PO Receipt Type")]
		public virtual String Source
		{
			get
			{
				return this._Source;
			}
			set
			{
				this._Source = value;
			}
		}
		#endregion
		#region LandedCostCodeID  
		public abstract class landedCostCodeID   : PX.Data.IBqlField
		{
		}
		protected String _LandedCostCodeID  ;
		[PXDBString(15, IsUnicode = true, IsFixed = false)]
		[PXDefault()]
		[PXUIField(DisplayName = "Landed Cost Code")]
		[PXSelector(typeof(Search<LandedCostCode.landedCostCodeID,Where<LandedCostCode.applicationMethod,Equal<LandedCostApplicationMethod.fromPO>,
												Or<LandedCostCode.applicationMethod, Equal<LandedCostApplicationMethod.fromBoth>>>>))]
		public virtual String LandedCostCodeID  
		{
			get
			{
				return this._LandedCostCodeID  ;
			}
			set
			{
				this._LandedCostCodeID   = value;
			}
		}
		#endregion
		#region POReceiptType
		public abstract class pOReceiptType : PX.Data.IBqlField
		{
		}
		protected String _POReceiptType;
		[PXDBString(3, IsFixed = true)]
		[PXDBDefault(typeof(POReceipt.receiptType), DefaultForUpdate = false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "PO Receipt Type")]
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
		[PXDBDefault(typeof(POReceipt.receiptNbr), DefaultForUpdate = false)]
		[PXUIField(DisplayName = "PO Receipt Nbr.", Visibility=PXUIVisibility.Invisible)]
		[PXParent(typeof(Select<POReceipt,Where<POReceipt.receiptNbr,Equal<Current<LandedCostTran.pOReceiptNbr>>>>))]
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
		[PXUIField(DisplayName = "PO Receipt Line Nbr.")]
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

        #region LineNbr
        public abstract class lineNbr : PX.Data.IBqlField
        {
        }
        protected Int32? _LineNbr;
        [PXDBInt()]
        [PXLineNbr(typeof(POReceipt.lineCntr))]
        [PXUIField(DisplayName = "Line Nbr.", Enabled = false, Visible = false)]
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

        #region APDocType
        public abstract class aPDocType : PX.Data.IBqlField
		{
		}
		protected String _APDocType;
		[PXDBString(3, IsFixed = true)]
		[PXUIField(DisplayName = "AP Doc. Type",Enabled=false)]
		[APInvoiceType.List()]	
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
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "AP Ref. Nbr.",Enabled=false)]
		[APInvoiceType.RefNbr(typeof(Search2<AP.Standalone.APRegisterAlias.refNbr,
			InnerJoinSingleTable<APInvoice, On<APInvoice.docType, Equal<AP.Standalone.APRegisterAlias.docType>,
				And<APInvoice.refNbr, Equal<AP.Standalone.APRegisterAlias.refNbr>>>,
			InnerJoinSingleTable<Vendor, On<AP.Standalone.APRegisterAlias.vendorID, Equal<Vendor.bAccountID>>>>,
			Where<AP.Standalone.APRegisterAlias.docType, Equal<Current<LandedCostTran.aPDocType>>,
				And<Match<Vendor, Current<AccessInfo.userName>>>>, 
			OrderBy<Desc<AP.Standalone.APRegisterAlias.refNbr>>>))]
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
        #region INDocType
        public abstract class iNDocType : PX.Data.IBqlField
        {
        }
        protected String _INDocType;
        [PXDBString(3, IsFixed = true)]
        [PXUIField(DisplayName = "IN Doc. Type",Enabled = false)]
		[IN.INDocType.List()]
        public virtual String INDocType
        {
            get
            {
                return this._INDocType;
            }
            set
            {
                this._INDocType = value;
            }
        }
        #endregion
        #region INRefNbr
        public abstract class iNRefNbr : PX.Data.IBqlField
        {
        }
        protected String _INRefNbr;
		[PXDBString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "IN Ref. Nbr.",Enabled = false)]
		[PXSelector(typeof(Search<IN.INRegister.refNbr, Where<IN.INRegister.docType, Equal<Optional<LandedCostTran.iNDocType>>>, OrderBy<Desc<IN.INRegister.refNbr>>>), Filterable = true)]
        public virtual String INRefNbr
        {
            get
            {
                return this._INRefNbr;
            }
            set
            {
                this._INRefNbr = value;
            }
        }
        #endregion
	
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;

		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description")]
		[PXDefault(typeof(Search<LandedCostCode.descr,Where<LandedCostCode.landedCostCodeID,Equal<Current<LandedCostTran.landedCostCodeID>>>>),PersistingCheck =PXPersistingCheck.Nothing)]
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
		#region InvoiceNbr
		public abstract class invoiceNbr : PX.Data.IBqlField
		{
		}
		protected String _InvoiceNbr;
		[PXDBString(40, IsUnicode = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Vendor Ref.")]
		[LCTranVendorRefNbr]
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
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[LandedCostVendorActive(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true)]
		[PXDefault(typeof(Search<LandedCostCode.vendorID, Where<LandedCostCode.landedCostCodeID, Equal<Current<LandedCostTran.landedCostCodeID>>>>))]
		[PXUIField(DisplayName = "Vendor")]
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
		[LocationID(typeof(Where<Location.bAccountID, Equal<Current<LandedCostTran.vendorID>>>), DescriptionField = typeof(Location.descr), Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(typeof(Search<LandedCostCode.vendorLocationID, Where<LandedCostCode.landedCostCodeID, Equal<Current<LandedCostTran.landedCostCodeID>>>>))]
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
		#region InvoiceDate
		public abstract class invoiceDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _InvoiceDate;
		[PXDBDate()]
		[PXDefault(typeof(POReceipt.receiptDate),PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "AP Bill Date")]
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
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL")]
		[PXUIField(DisplayName = "CurrencySimpleSelector", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(typeof(Coalesce<Search<AP.Vendor.curyID,Where<AP.Vendor.bAccountID,Equal<Current<LandedCostTran.vendorID>>>>,
									Search<POReceipt.curyID, Where<POReceipt.receiptNbr, Equal<Current<LandedCostTran.pOReceiptNbr>>>>>))]
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
		[PXUIField(DisplayName = "Currency")]
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
		#region CuryLCAmount
		public abstract class curyLCAmount : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryLCAmount;

		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(LandedCostTran.curyInfoID), typeof(LandedCostTran.lCAmount), MinValue=0)]
		[PXUIVerify(typeof(Where<curyLCAmount, NotEqual<decimal0>>), PXErrorLevel.Error, EP.Messages.ValueMustBeGreaterThanZero, CheckOnInserted = false)]
		[PXUIField(DisplayName = "Amount")]
		public virtual Decimal? CuryLCAmount
		{
			get
			{
				return this._CuryLCAmount;
			}
			set
			{
				this._CuryLCAmount = value;
			}
		}
		#endregion
		#region LCAmount
		public abstract class lCAmount : PX.Data.IBqlField
		{
		}
		protected Decimal? _LCAmount;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]		
		public virtual Decimal? LCAmount
		{
			get
			{
				return this._LCAmount;
			}
			set
			{
				this._LCAmount = value;
			}
		}
		#endregion
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[IN.Inventory(typeof(Search2<IN.InventoryItem.inventoryID, 
			InnerJoin<POReceiptLine, On<POReceiptLine.inventoryID, Equal<IN.InventoryItem.inventoryID>>>,
			Where2<Match<Current<AccessInfo.userName>>, And<POReceiptLine.receiptNbr, Equal<Current<LandedCostTran.pOReceiptNbr>>>>>), typeof(IN.InventoryItem.inventoryCD), typeof(IN.InventoryItem.descr), DisplayName = "Inventory ID")]
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
		#region TaxCategoryID
		public abstract class taxCategoryID : PX.Data.IBqlField
		{
		}
		protected String _TaxCategoryID;
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Tax Category", Visibility = PXUIVisibility.Visible)]
		[PXDefault(typeof(Coalesce<Search<LandedCostCode.taxCategoryID, Where<LandedCostCode.landedCostCodeID, Equal<Optional<LandedCostTran.landedCostCodeID>>>>,
						Search2<TaxZone.dfltTaxCategoryID, InnerJoin<CRLocation, On<CRLocation.vTaxZoneID, Equal<TaxZone.taxZoneID>>>,
							Where<CRLocation.bAccountID, Equal<Current2<LandedCostTran.vendorID>>,
								And<CRLocation.locationID,Equal<Current2<LandedCostTran.vendorLocationID>>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [PXSelector(typeof(TaxCategory.taxCategoryID), DescriptionField = typeof(TaxCategory.descr))]
        [PXRestrictor(typeof(Where<TaxCategory.active, Equal<True>>), TX.Messages.InactiveTaxCategory, typeof(TaxCategory.taxCategoryID))]
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
		
		#region TermsID
		public abstract class termsID : PX.Data.IBqlField
		{
		}
		protected String _TermsID;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault(typeof(Search<LandedCostCode.termsID, Where<LandedCostCode.landedCostCodeID, Equal<Current<LandedCostTran.landedCostCodeID>>>>),PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Terms", Visibility = PXUIVisibility.Visible)]
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
		#region Processed
		public abstract class processed : PX.Data.IBqlField
		{
		}
		protected Boolean? _Processed;
		[PXDBBool()]
		[PXUIField(DisplayName = "Processed", Visibility = PXUIVisibility.Visible)]
		[PXDefault(false)]
		public virtual Boolean? Processed
		{
			get
			{
				return this._Processed;
			}
			set
			{
				this._Processed = value;
			}
		}
		#endregion
		#region PostponeAP
		public abstract class postponeAP : PX.Data.IBqlField
		{
		}
		protected Boolean? _PostponeAP;
		[PXDBBool()]
		[PXUIField(DisplayName = "Postpone AP Bill Creation", Visibility = PXUIVisibility.Visible)]
		[PXDefault(false)]
		public virtual Boolean? PostponeAP
		{
			get
			{
				return this._PostponeAP;
			}
			set
			{
				this._PostponeAP = value;
			}
		}
		#endregion
		#region LCAccrualAcct
		public abstract class lCAccrualAcct : PX.Data.IBqlField
		{
		}
		protected Int32? _LCAccrualAcct;

		[PXDBInt]
		[PXDefault(typeof(Search<LandedCostCode.lCAccrualAcct,Where<LandedCostCode.landedCostCodeID,Equal<Current<LandedCostTran.landedCostCodeID>>>>),PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Int32? LCAccrualAcct
		{
			get
			{
				return this._LCAccrualAcct;
			}
			set
			{
				this._LCAccrualAcct = value;
			}
		}
		#endregion
		#region LCAccrualSub
		public abstract class lCAccrualSub : PX.Data.IBqlField
		{
		}
		protected Int32? _LCAccrualSub;

		[PXDBInt()]
		[PXDefault(typeof(Search<LandedCostCode.lCAccrualSub, Where<LandedCostCode.landedCostCodeID, Equal<Current<LandedCostTran.landedCostCodeID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Int32? LCAccrualSub
		{
			get
			{
				return this._LCAccrualSub;
			}
			set
			{
				this._LCAccrualSub = value;
			}
		}
		#endregion
		#region APCuryInfoID
		public abstract class aPCuryInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _APCuryInfoID;
		[PXDBLong()]				
		[PXUIField(DisplayName = "Currency")]
		public virtual Int64? APCuryInfoID
		{
			get
			{
				return this._APCuryInfoID;
			}
			set
			{
				this._APCuryInfoID = value;
			}
		}
		#endregion
		#region CuryLCAPAmount
		public abstract class curyLCAPAmount : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryLCAPAmount;


		[PXDBCurrency(typeof(LandedCostTran.curyInfoID), typeof(LandedCostTran.lCAPAmount), MinValue = 0)]
		[PXFormula(typeof(LandedCostTran.curyLCAmount))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount")]
		public virtual Decimal? CuryLCAPAmount
		{
			get
			{
				return this._CuryLCAPAmount;
			}
			set
			{
				this._CuryLCAPAmount = value;
			}
		}
		#endregion
		#region LCAPAmount
		public abstract class lCAPAmount : PX.Data.IBqlField
		{
		}
		protected Decimal? _LCAPAmount;
		[PXDBDecimal(4)]		
		//[PXFormula(typeof(Row<LandedCostTran.lCAmount>))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? LCAPAmount
		{
			get
			{
				return this._LCAPAmount;
			}
			set
			{
				this._LCAPAmount = value;
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

		#region JustInserted
		public abstract class justInserted : PX.Data.IBqlField
		{
		}
		protected bool? _JustInserted = false;
		[PXBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Selected")]
		public virtual bool? JustInserted
		{
			get
			{
				return _JustInserted;
			}
			set
			{
				_JustInserted = value;
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

		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SiteID;
		[IN.Site()]
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
		[IN.Location(typeof(LandedCostTran.siteID), KeepEntry = false)]
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
        #region INDocCreated
        public abstract class iNDocCreated : PX.Data.IBqlField
        {
        }
        [PXBool()]
        [PXUIField(DisplayName = "IN Doc Created", Visibility = PXUIVisibility.Visible)]
        public virtual Boolean? INDocCreated
        {
            [PXDependsOnFields(typeof(iNRefNbr))]
            get
            {
                return String.IsNullOrEmpty(this.INRefNbr) == false;
            }
            set
            {

            }
        }
        #endregion
        #region CuryLCAPEffAmount
        public abstract class curyLCAPEffAmount : PX.Data.IBqlField
        {
        }

        protected Decimal? _CuryLCAPEffAmount;
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXCury(typeof(LandedCostTran.curyID),MinValue = 0)]
		[PXDBCalced(typeof(Switch<Case<Where<LandedCostTran.aPDocType,Equal<APDocType.debitAdj>>,Minus<LandedCostTran.curyLCAPAmount>>,LandedCostTran.curyLCAPAmount>),typeof(Decimal))]        
        [PXUIField(DisplayName = "Amount")]
        public virtual Decimal? CuryLCAPEffAmount
        {
            get
            {
                return this._CuryLCAPEffAmount;                
            }
            set
            {                
                this._CuryLCAPEffAmount = value;
            }
        }

        #endregion
        

        #region AmountSign
        public abstract class amountSign : PX.Data.IBqlField
        {
        }
        [PXDecimal(0)]
        public virtual Decimal? AmountSign
        {            
            get
            {
                return IsReversed() ? Decimal.MinusOne: Decimal.One;
            }
            set
            {
                
            }
        }
        #endregion

		#region GroupTranID
		public abstract class groupTranID : PX.Data.IBqlField
		{
		}
		[PXLong()]
		[PXUIField(FieldName = "Group Tran ID", Visible = false, Enabled = false)]
		public virtual Int64? GroupTranID
		{			
			[PXDependsOnFields(typeof(lCTranID))]
			get
			{
				Int64? id = (this.LCTranID.HasValue && this.LCTranID.Value <0 ) ? ((Int64)Int32.MaxValue + (Int64)(this.LCTranID.Value -Int32.MinValue)):(Int64?)this.LCTranID;
				return id;
			}
			set
			{

			}
		}
		#endregion

        protected bool IsReversed() 
        {
            return String.IsNullOrEmpty(this._APDocType) == false && (this._APDocType == AP.APDocType.DebitAdj);
        }
	}

	public static class LandedCostTranSource
	{
		public const string FromAP = "AP";
		public const string FromPO = "PO";


		public class fromAP : Constant<string>
		{
			public fromAP() : base(FromAP) { ;}
		}

		public class fromPO : Constant<string>
		{
			public fromPO() : base(FromPO) { ;}
		}
	}
}
