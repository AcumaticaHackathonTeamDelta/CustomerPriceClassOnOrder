using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.CA;
using PX.Objects.CS;
using PX.Objects.CR;


namespace PX.Objects.AP
{
    [PXCacheName(Messages.APPaySelReport)]
	[PXProjection(typeof(
			Select2<APInvoice, 
			InnerJoin<Vendor, On<Vendor.bAccountID, Equal<APInvoice.vendorID>>,
            InnerJoin<CashAccount, On<CashAccount.cashAccountID, Equal<APInvoice.payAccountID>>, 
			InnerJoin<CurrencyInfo, On<CurrencyInfo.curyInfoID,Equal<APInvoice.curyInfoID>>,
			LeftJoin<APAdjust, On<APAdjust.adjdDocType,Equal<APInvoice.docType>, And<APAdjust.adjdRefNbr,Equal<APInvoice.refNbr>, And<APAdjust.released, Equal<False>>>>,
			CrossJoin<APSetup>>>>>,
		Where<APInvoice.openDoc, Equal<True>,
			And2<Where<APInvoice.released, Equal<True>,Or<APInvoice.prebooked,Equal<True>>>, 
			And2<Where<APInvoice.paySel, Equal<True>,Or<APSetup.requireApprovePayments, Equal<False>>>,
			And<APInvoice.voided,Equal<False>,
			And<APInvoice.dueDate,IsNotNull,
			And<APAdjust.adjgRefNbr,IsNull>>>>>>>))]
    [Serializable]
	public partial class APPaySelReport : PX.Data.IBqlTable
	{
        #region BranchID
        public abstract class branchID : PX.Data.IBqlField
        {
        }
        protected Int32? _BranchID;
        [Branch(BqlField = typeof(APInvoice.branchID))]
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
        #region PayAccountID
		public abstract class payAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _PayAccountID;
		[CashAccount(BqlField = typeof(APInvoice.payAccountID))]
		public virtual Int32? PayAccountID
		{
			get
			{
				return this._PayAccountID;
			}
			set
			{
				this._PayAccountID = value;
			}
		}
		#endregion
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[Vendor(BqlField = typeof(APInvoice.vendorID))]
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
		#region PayTypeID
		public abstract class payTypeID : PX.Data.IBqlField
		{
		}
		protected String _PayTypeID;
        [PXDBString(10, IsUnicode = true, BqlField = typeof(APInvoice.payTypeID))]
        [PXUIField(DisplayName = "Payment Method", Visibility = PXUIVisibility.Visible)]
		public virtual String PayTypeID
		{
			get
			{
				return this._PayTypeID;
			}
			set
			{
				this._PayTypeID = value;
			}
		}
		#endregion
		#region TermsID
		public abstract class termsID : PX.Data.IBqlField
		{
		}
		protected String _TermsID;
        [PXDBString(10, IsUnicode = true, BqlField = typeof(APInvoice.termsID))]
        [PXUIField(DisplayName = "Terms", Visibility = PXUIVisibility.Visible)]
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
		#region DocType
		public abstract class docType : PX.Data.IBqlField
		{
		}
		protected string _DocType;
        [PXDBString(3, IsKey = true, IsFixed = true, BqlField = typeof(APInvoice.docType))]
        [PXUIField(DisplayName = "Document Type", Visibility = PXUIVisibility.SelectorVisible)]
		[APInvoiceType.List()]
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
		#region PrintDocType
		public abstract class printDocType : PX.Data.IBqlField
		{
		}
		[PXString(3, IsFixed = true)]
		[APDocType.PrintList()]
		[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.Visible, Enabled = true)]
		public virtual String PrintDocType
		{
			get
			{
				return this._DocType;
			}
			set
			{
			}
		}
		#endregion
		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
		}
		protected string _RefNbr;
        [PXDBString(15, IsUnicode = true, IsKey = true, BqlField = typeof(APInvoice.refNbr))]
        [PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region PayNbr
		public abstract class payNbr : PX.Data.IBqlField
		{
		}
		protected string _PayNbr;
        [PXDBString(15)]
        [PXUIField(DisplayName = "Payment Nbr.", Visibility = PXUIVisibility.Visible)]
		public virtual String PayNbr
		{
			[PXDependsOnFields(typeof(separateCheck),typeof(refNbr))]
			get
			{
				return (this._SeparateCheck == true ? this._RefNbr: "");
			}
			set
			{
				this._PayNbr = value;
			}
		}
		#endregion
		#region SeparateCheck
		public abstract class separateCheck : PX.Data.IBqlField
		{
		}
		protected Boolean? _SeparateCheck;
        [PXDBBool(BqlField = typeof(APInvoice.separateCheck))]
        [PXUIField(DisplayName = "Separate Check", Visibility = PXUIVisibility.Visible)]
		public virtual Boolean? SeparateCheck
		{
			get
			{
				return this._SeparateCheck;
			}
			set
			{
				this._SeparateCheck = value;
			}
		}
		#endregion

		#region InvoiceNbr
		public abstract class invoiceNbr : PX.Data.IBqlField
		{
		}
		protected String _InvoiceNbr;
        [PXDBString(40, IsUnicode = true, BqlField = typeof(APInvoice.invoiceNbr))]
        [PXUIField(DisplayName = "Vendor Ref.", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region DueDate
		public abstract class dueDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _DueDate;
        [PXDBDate(BqlField = typeof(APInvoice.dueDate))]
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
        [PXDBDate(BqlField = typeof(APInvoice.discDate))]
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
		#region PayDate
		public abstract class payDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _PayDate;
        [PXDBDate(BqlField = typeof(APInvoice.payDate))]
        [PXUIField(DisplayName = "Pay Date", Visibility = PXUIVisibility.Visible)]
		public virtual DateTime? PayDate
		{
			get
			{
				return this._PayDate;
			}
			set
			{
				this._PayDate = value;
			}
		}
		#endregion
		#region PaySel
		public abstract class paySel : PX.Data.IBqlField
		{
		}
		protected Boolean? _PaySel;
        [PXDBBool(BqlField = typeof(APInvoice.paySel))]
        [PXUIField(DisplayName = "Selected")]
		public virtual Boolean? PaySel
		{
			get
			{
				return this._PaySel;
			}
			set
			{
				this._PaySel = value;
			}
		}
		#endregion
		#region CuryInfoID
		public abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _CuryInfoID;
        [PXDBLong(BqlField = typeof(APInvoice.curyInfoID))]
        [PXUIField(DisplayName = "Currency Info")]
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
		#region BaseCuryID
		public abstract class baseCuryID : PX.Data.IBqlField
		{
		}
		protected string _BaseCuryID;
        [PXDBString(5, IsUnicode = true, BqlField = typeof(CurrencyInfo.baseCuryID))]
        [PXUIField(DisplayName = "Base Currency ID")]
		public virtual string BaseCuryID
		{
			get
			{
				return this._BaseCuryID;
			}
			set
			{
				this._BaseCuryID = value;
			}
		}
		#endregion
        #region CuryRateType
        public abstract class curyRateType : PX.Data.IBqlField
        {
        }
        protected string _CuryRateType;
        [PXDBString(6, IsUnicode = true, BqlField = typeof(CurrencyInfo.curyRateTypeID))]
        public virtual string CuryRateType
        {
            get
            {
                return this._CuryRateType;
            }
            set
            {
                this._CuryRateType = value;
            }
        }
        #endregion
		#region CashCuryID
		public abstract class cashCuryID : PX.Data.IBqlField
		{ 
		}
		protected string _CashCuryID;
		[PXDBString(5, IsUnicode = true, BqlField=typeof(CashAccount.curyID))]
        [PXDefault(typeof(Search<Company.baseCuryID>))]
        [PXUIField(DisplayName = "Cash Account Currency")]
		public virtual string CashCuryID
		{
			get
			{
				return this._CashCuryID;
			}
			set
			{
				this._CashCuryID = value;
			}
		}
		#endregion
		#region DocCuryID
		public abstract class docCuryID : PX.Data.IBqlField
		{
		}
		protected string _DocCuryID;
		[PXDBString(5, IsUnicode = true, BqlField = typeof(CurrencyInfo.curyID))]
        [PXDefault(typeof(Search<Company.baseCuryID>))]
        [PXUIField(DisplayName = "Document Currency")]
		public virtual string DocCuryID
		{
			get
			{
				return this._DocCuryID;
			}
			set
			{
				this._DocCuryID = value;
			}
		}
		#endregion
		#region OrigCuryMultDiv
		public abstract class origCuryMultDiv : PX.Data.IBqlField
		{
		}
		protected string _OrigCuryMultDiv;
		[PXDBString(1, IsFixed = true, BqlField = typeof(CurrencyInfo.curyMultDiv))]
		[PXDefault("M")]
		public virtual string OrigCuryMultDiv
		{
			get
			{
				return this._OrigCuryMultDiv;
			}
			set
			{
				this._OrigCuryMultDiv = value;
			}
		}
		#endregion
		#region OrigCuryRate
		public abstract class origCuryRate : PX.Data.IBqlField
		{
		}
		protected Decimal? _OrigCuryRate;
		[PXDBDecimal(6, BqlField = typeof(CurrencyInfo.curyRate))]
		[PXDefault(TypeCode.Decimal, "1.0")]
		public virtual Decimal? OrigCuryRate
		{
			get
			{
				return this._OrigCuryRate;
			}
			set
			{
				this._OrigCuryRate = value;
			}
		}
		#endregion
		#region CuryOrigDocAmt
		public abstract class curyOrigDocAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryOrigDocAmt;
        [PXDBCury(typeof(APPaySelReport.cashCuryID), BqlField = typeof(APInvoice.curyOrigDocAmt))]
        [PXUIField(DisplayName = "Amount", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? CuryOrigDocAmt
		{
			get
			{
				return this._CuryOrigDocAmt;
			}
			set
			{
				this._CuryOrigDocAmt = value;
			}
		}
		#endregion
		#region OrigDocAmt
		public abstract class origDocAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _OrigDocAmt;
		[PXDBBaseCury(BqlField = typeof(APInvoice.origDocAmt))]
		public virtual Decimal? OrigDocAmt
		{
			get
			{
				return this._OrigDocAmt;
			}
			set
			{
				this._OrigDocAmt = value;
			}
		}
		#endregion
		#region CuryDocBal
		public abstract class curyDocBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryDocBal;
        [PXDBCury(typeof(APPaySelReport.cashCuryID), BqlField = typeof(APInvoice.curyDocBal))]
        [PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryDocBal
		{
			get
			{
				return this._CuryDocBal;
			}
			set
			{
				this._CuryDocBal = value;
			}
		}
		#endregion
		#region DocBal
		public abstract class docBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _DocBal;
		[PXDBBaseCury(BqlField=typeof(APInvoice.docBal))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? DocBal
		{
			get
			{
				return this._DocBal;
			}
			set
			{
				this._DocBal = value;
			}
		}
		#endregion
		#region CuryDiscBal
		public abstract class curyDiscBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryDiscBal;
        [PXDBCury(typeof(APPaySelReport.cashCuryID), BqlField = typeof(APInvoice.curyDiscBal))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryDiscBal
		{
			get
			{
				return this._CuryDiscBal;
			}
			set
			{
				this._CuryDiscBal = value;
			}
		}
		#endregion
		#region DiscBal
		public abstract class discBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _DiscBal;
		[PXDBBaseCury(BqlField=typeof(APInvoice.discBal))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? DiscBal
		{
			get
			{
				return this._DiscBal;
			}
			set
			{
				this._DiscBal = value;
			}
		}
		#endregion
		#region CuryPayOrigDocAmt
		public abstract class curyPayOrigDocAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryPayOrigDocAmt;
        [PXCury(typeof(APPaySelReport.cashCuryID))]
		public virtual Decimal? CuryPayOrigDocAmt
		{
            [PXDependsOnFields(typeof(curyOrigDocAmt), typeof(origDocAmt))]
            get
			{
				return this._CuryPayOrigDocAmt;
			}
			set
			{
				this._CuryPayOrigDocAmt = value;
			}
		}
		#endregion
		#region PayOrigDocAmt
		public abstract class payOrigDocAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _PayOrigDocAmt;
		[PXDBBaseCury(BqlField = typeof(APInvoice.origDocAmt))]
		public virtual Decimal? PayOrigDocAmt
		{
			get
			{
				return this._PayOrigDocAmt;
			}
			set
			{
				this._PayOrigDocAmt = value;
			}
		}
		#endregion
		#region CuryPayDocBal
		public abstract class curyPayDocBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryPayDocBal;
        [PXCury(typeof(APPaySelReport.cashCuryID))]
        [PXUIField(DisplayName = "Balance", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? CuryPayDocBal
		{
            [PXDependsOnFields(typeof(curyDocBal), typeof(docBal))]
            get
			{
				return this._CuryPayDocBal;
			}
			set
			{
				this._CuryPayDocBal = value;
			}
		}
		#endregion
		#region PayDocBal
		public abstract class payDocBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _PayDocBal;
		[PXDBBaseCury(BqlField = typeof(APInvoice.docBal))]
		public virtual Decimal? PayDocBal
		{
			get
			{
				return this._PayDocBal;
			}
			set
			{
				this._PayDocBal = value;
			}
		}
		#endregion
		#region CuryPayDiscBal
		public abstract class curyPayDiscBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryPayDiscBal;
        [PXCury(typeof(APPaySelReport.cashCuryID))]
        [PXUIField(DisplayName = "Cash Discount Balance", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? CuryPayDiscBal
		{
            [PXDependsOnFields(typeof(curyDiscBal), typeof(discBal))]
            get
			{
				return this._CuryPayDiscBal;
			}
			set
			{
				this._CuryPayDiscBal = value;
			}
		}
		#endregion
		#region PayDiscBal
		public abstract class payDiscBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _PayDiscBal;
		[PXDBBaseCury(BqlField = typeof(APInvoice.discBal))]
		public virtual Decimal? PayDiscBal
		{
			get
			{
				return this._PayDiscBal;
			}
			set
			{
				this._PayDiscBal = value;
			}
		}
		#endregion
		#region SignBalance
		public abstract class signBalance : PX.Data.IBqlField { }
		[PXDecimal(0)]
		[PXUIField(DisplayName = "SignBalance", Visibility = PXUIVisibility.Invisible)]
		public virtual Decimal? SignBalance
		{
			[PXDependsOnFields(typeof(docType))]
			get
			{
				return APDocType.SignBalance(this._DocType);
			}
			set
			{
			}
		}
		#endregion
	}

    [PXCacheName(Messages.APPayNotSelReport)]
	[PXProjection(typeof(
			Select2<APInvoice,
			InnerJoin<Location, On<Location.bAccountID, Equal<APInvoice.vendorID>, And<Location.locationID, Equal<APInvoice.vendorLocationID>>>,
			InnerJoin<LocationAPPaymentInfo, On<LocationAPPaymentInfo.bAccountID, Equal<Location.bAccountID>, And<LocationAPPaymentInfo.locationID, Equal<Location.vPaymentInfoLocationID>>>,
            LeftJoin<PaymentMethodAccount, On<PaymentMethodAccount.paymentMethodID, Equal<LocationAPPaymentInfo.vPaymentMethodID>, And<PaymentMethodAccount.branchID, Equal<APInvoice.branchID>, And<PaymentMethodAccount.useForAP, Equal<True>, And<PaymentMethodAccount.aPIsDefault, Equal<True>>>>>,
			LeftJoin<CashAccount, 
                On<CashAccount.cashAccountID, Equal<APInvoice.payAccountID>,
                Or<APInvoice.payAccountID, IsNull, And<CashAccount.cashAccountID, Equal<LocationAPPaymentInfo.vCashAccountID>,
                Or<APInvoice.payAccountID, IsNull, And<LocationAPPaymentInfo.vCashAccountID, IsNull, And<CashAccount.cashAccountID, Equal<PaymentMethodAccount.cashAccountID>>>>>>>,
			InnerJoin<CurrencyInfo, On<CurrencyInfo.curyInfoID, Equal<APInvoice.curyInfoID>>,
            LeftJoin<APAdjust, On<APAdjust.adjdDocType, Equal<APInvoice.docType>, And<APAdjust.adjdRefNbr, Equal<APInvoice.refNbr>, And<APAdjust.released, Equal<False>>>>>>>>>>,
		Where<APInvoice.openDoc, Equal<True>,
			And2<Where<APInvoice.released, Equal<True>,Or<APInvoice.prebooked,Equal<True>>>, 
			And<APInvoice.paySel, Equal<False>,
			And<APInvoice.dueDate, IsNotNull,
			And<APAdjust.adjgRefNbr, IsNull,
            And<CashAccount.cashAccountID, IsNotNull>>>>>>>))]
    [Serializable]
	public partial class APPayNotSelReport : PX.Data.IBqlTable
	{
        #region BranchID
        public abstract class branchID : PX.Data.IBqlField
        {
        }
        protected Int32? _BranchID;
        [Branch(BqlField = typeof(APInvoice.branchID))]
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
        #region PayAccountID
		public abstract class payAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _PayAccountID;
		[CashAccount(BqlField = typeof(CashAccount.cashAccountID))]
		public virtual Int32? PayAccountID
		{
			get
			{
				return this._PayAccountID;
			}
			set
			{
				this._PayAccountID = value;
			}
		}
		#endregion
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[Vendor(BqlField = typeof(APInvoice.vendorID))]
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
		#region PayTypeID
		public abstract class payTypeID : PX.Data.IBqlField
		{
		}
		protected String _PayTypeID;
        [PXDBString(10, IsUnicode = true, BqlField = typeof(LocationAPPaymentInfo.vPaymentMethodID))]
        [PXUIField(DisplayName = "Payment Method", Visibility = PXUIVisibility.Visible)]
		public virtual String PayTypeID
		{
			get
			{
				return this._PayTypeID;
			}
			set
			{
				this._PayTypeID = value;
			}
		}
		#endregion
		#region TermsID
		public abstract class termsID : PX.Data.IBqlField
		{
		}
		protected String _TermsID;
		[PXDBString(10, IsUnicode = true, BqlField = typeof(APInvoice.termsID))]
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
		#region DocType
		public abstract class docType : PX.Data.IBqlField
		{
		}
		protected string _DocType;
		[PXDBString(3, IsKey = true, IsFixed = true, BqlField = typeof(APInvoice.docType))]
		[APInvoiceType.List()]
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
		#region PrintDocType
		public abstract class printDocType : PX.Data.IBqlField
		{
		}
		[PXString(3, IsFixed = true)]
		[APDocType.PrintList()]
		[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.Visible, Enabled = true)]
		public virtual String PrintDocType
		{
			get
			{
				return this._DocType;
			}
			set
			{
			}
		}
		#endregion
		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
		}
		protected string _RefNbr;
        [PXDBString(15, IsUnicode = true, IsKey = true, BqlField = typeof(APInvoice.refNbr))]
        [PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region InvoiceNbr
		public abstract class invoiceNbr : PX.Data.IBqlField
		{
		}
		protected String _InvoiceNbr;
        [PXDBString(40, IsUnicode = true, BqlField = typeof(APInvoice.invoiceNbr))]
        [PXUIField(DisplayName = "Vendor Ref.", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region DueDate
		public abstract class dueDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _DueDate;
        [PXDBDate(BqlField = typeof(APInvoice.dueDate))]
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
        [PXDBDate(BqlField = typeof(APInvoice.discDate))]
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
		#region PayDate
		public abstract class payDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _PayDate;
		[PXDBDate(BqlField = typeof(APInvoice.payDate))]
		public virtual DateTime? PayDate
		{
			get
			{
				return this._PayDate;
			}
			set
			{
				this._PayDate = value;
			}
		}
		#endregion
		#region PaySel
		public abstract class paySel : PX.Data.IBqlField
		{
		}
		protected Boolean? _PaySel;
        [PXDBBool(BqlField = typeof(APInvoice.paySel))]
		public virtual Boolean? PaySel
		{
			get
			{
				return this._PaySel;
			}
			set
			{
				this._PaySel = value;
			}
		}
		#endregion
		#region CuryInfoID
		public abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _CuryInfoID;
		[PXDBLong(BqlField = typeof(APInvoice.curyInfoID))]
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
		#region BaseCuryID
		public abstract class baseCuryID : PX.Data.IBqlField
		{
		}
		protected string _BaseCuryID;
		[PXDBString(5, IsUnicode = true, BqlField = typeof(CurrencyInfo.baseCuryID))]
		public virtual string BaseCuryID
		{
			get
			{
				return this._BaseCuryID;
			}
			set
			{
				this._BaseCuryID = value;
			}
		}
		#endregion
        #region CuryRateType
        public abstract class curyRateType : PX.Data.IBqlField
        {
        }
        protected string _CuryRateType;
        [PXDBString(6, IsUnicode = true, BqlField = typeof(CurrencyInfo.curyRateTypeID))]
        public virtual string CuryRateType
        {
            get
            {
                return this._CuryRateType;
            }
            set
            {
                this._CuryRateType = value;
            }
        }
        #endregion
		#region CashCuryID
		public abstract class cashCuryID : PX.Data.IBqlField
		{
		}
		protected string _CashCuryID;
		[PXDBString(5, IsUnicode = true, BqlField = typeof(CashAccount.curyID))]
        [PXDefault(typeof(Search<Company.baseCuryID>))]
        [PXUIField(DisplayName = "Cash Account Currency")]
		public virtual string CashCuryID
		{
			get
			{
				return this._CashCuryID;
			}
			set
			{
				this._CashCuryID = value;
			}
		}
		#endregion
		#region DocCuryID
		public abstract class docCuryID : PX.Data.IBqlField
		{
		}
		protected string _DocCuryID;
		[PXDBString(5, IsUnicode = true, BqlField = typeof(CurrencyInfo.curyID))]
        [PXDefault(typeof(Search<Company.baseCuryID>))]
        [PXUIField(DisplayName = "Document Currency")]
		public virtual string DocCuryID
		{
			get
			{
				return this._DocCuryID;
			}
			set
			{
				this._DocCuryID = value;
			}
		}
		#endregion
		#region OrigCuryMultDiv
		public abstract class origCuryMultDiv : PX.Data.IBqlField
		{
		}
		protected string _OrigCuryMultDiv;
		[PXDBString(1, IsFixed = true, BqlField = typeof(CurrencyInfo.curyMultDiv))]
		[PXDefault("M")]
		public virtual string OrigCuryMultDiv
		{
			get
			{
				return this._OrigCuryMultDiv;
			}
			set
			{
				this._OrigCuryMultDiv = value;
			}
		}
		#endregion
		#region OrigCuryRate
		public abstract class origCuryRate : PX.Data.IBqlField
		{
		}
		protected Decimal? _OrigCuryRate;
		[PXDBDecimal(6, BqlField = typeof(CurrencyInfo.curyRate))]
		[PXDefault(TypeCode.Decimal, "1.0")]
		public virtual Decimal? OrigCuryRate
		{
			get
			{
				return this._OrigCuryRate;
			}
			set
			{
				this._OrigCuryRate = value;
			}
		}
		#endregion
		#region CuryOrigDocAmt
		public abstract class curyOrigDocAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryOrigDocAmt;
        [PXDBCury(typeof(APPaySelReport.cashCuryID), BqlField = typeof(APInvoice.curyOrigDocAmt))]
		public virtual Decimal? CuryOrigDocAmt
		{
			get
			{
				return this._CuryOrigDocAmt;
			}
			set
			{
				this._CuryOrigDocAmt = value;
			}
		}
		#endregion
		#region OrigDocAmt
		public abstract class origDocAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _OrigDocAmt;
		[PXDBBaseCury(BqlField = typeof(APInvoice.origDocAmt))]
		public virtual Decimal? OrigDocAmt
		{
			get
			{
				return this._OrigDocAmt;
			}
			set
			{
				this._OrigDocAmt = value;
			}
		}
		#endregion
		#region CuryDocBal
		public abstract class curyDocBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryDocBal;
        [PXDBCury(typeof(APPaySelReport.cashCuryID), BqlField = typeof(APInvoice.curyDocBal))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryDocBal
		{
			get
			{
				return this._CuryDocBal;
			}
			set
			{
				this._CuryDocBal = value;
			}
		}
		#endregion
		#region DocBal
		public abstract class docBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _DocBal;
		[PXDBBaseCury(BqlField = typeof(APInvoice.docBal))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? DocBal
		{
			get
			{
				return this._DocBal;
			}
			set
			{
				this._DocBal = value;
			}
		}
		#endregion
		#region CuryDiscBal
		public abstract class curyDiscBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryDiscBal;
        [PXDBCury(typeof(APPaySelReport.cashCuryID), BqlField = typeof(APInvoice.curyDiscBal))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryDiscBal
		{
			get
			{
				return this._CuryDiscBal;
			}
			set
			{
				this._CuryDiscBal = value;
			}
		}
		#endregion
		#region DiscBal
		public abstract class discBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _DiscBal;
		[PXDBBaseCury(BqlField = typeof(APInvoice.discBal))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? DiscBal
		{
			get
			{
				return this._DiscBal;
			}
			set
			{
				this._DiscBal = value;
			}
		}
		#endregion
		#region CuryPayOrigDocAmt
		public abstract class curyPayOrigDocAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryPayOrigDocAmt;
        [PXDBCury(typeof(APPaySelReport.cashCuryID), BqlField = typeof(APInvoice.curyOrigDocAmt))]
		public virtual Decimal? CuryPayOrigDocAmt
		{
            [PXDependsOnFields(typeof(curyOrigDocAmt), typeof(origDocAmt))]
            get
			{
				return this._CuryPayOrigDocAmt;
			}
			set
			{
				this._CuryPayOrigDocAmt = value;
			}
		}
		#endregion
		#region PayOrigDocAmt
		public abstract class payOrigDocAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _PayOrigDocAmt;
		[PXDBBaseCury(BqlField = typeof(APInvoice.origDocAmt))]
		public virtual Decimal? PayOrigDocAmt
		{
			get
			{
				return this._PayOrigDocAmt;
			}
			set
			{
				this._PayOrigDocAmt = value;
			}
		}
		#endregion
		#region CuryPayDocBal
		public abstract class curyPayDocBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryPayDocBal;
        [PXCury(typeof(APPaySelReport.cashCuryID))]
        [PXUIField(DisplayName = "Balance", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? CuryPayDocBal
		{
            [PXDependsOnFields(typeof(curyDocBal), typeof(docBal))]
            get
			{
				return this._CuryPayDocBal;
			}
			set
			{
				this._CuryPayDocBal = value;
			}
		}
		#endregion
		#region PayDocBal
		public abstract class payDocBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _PayDocBal;
		[PXDBBaseCury(BqlField = typeof(APInvoice.docBal))]
		public virtual Decimal? PayDocBal
		{
			get
			{
				return this._PayDocBal;
			}
			set
			{
				this._PayDocBal = value;
			}
		}
		#endregion
		#region CuryPayDiscBal
		public abstract class curyPayDiscBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryPayDiscBal;
        [PXCury(typeof(APPaySelReport.cashCuryID))]
        [PXUIField(DisplayName = "Cash Discount Balance", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? CuryPayDiscBal
		{
            [PXDependsOnFields(typeof(curyDiscBal), typeof(discBal))]
            get
			{
				return this._CuryPayDiscBal;
			}
			set
			{
				this._CuryPayDiscBal = value;
			}
		}
		#endregion
		#region PayDiscBal
		public abstract class payDiscBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _PayDiscBal;
		[PXDBBaseCury(BqlField = typeof(APInvoice.discBal))]
		public virtual Decimal? PayDiscBal
		{
			get
			{
				return this._PayDiscBal;
			}
			set
			{
				this._PayDiscBal = value;
			}
		}
		#endregion
		#region SignBalance
		public abstract class signBalance : PX.Data.IBqlField { }
		[PXDecimal(0)]
		[PXUIField(DisplayName = "SignBalance", Visibility = PXUIVisibility.Invisible)]
		public virtual Decimal? SignBalance
		{
			[PXDependsOnFields(typeof(docType))]
			get
			{
				return APDocType.SignBalance(this._DocType);
			}
			set
			{
			}
		}
		#endregion
	}

    [PXCacheName(Messages.APCashRequirementReport)]
	[PXProjection(typeof(
			Select2<APInvoice,
			InnerJoin<Location, On<Location.bAccountID, Equal<APInvoice.vendorID>, And<Location.locationID, Equal<APInvoice.vendorLocationID>>>,
			InnerJoin<LocationAPPaymentInfo, On<LocationAPPaymentInfo.bAccountID, Equal<Location.bAccountID>, And<LocationAPPaymentInfo.locationID, Equal<Location.vPaymentInfoLocationID>>>,
            LeftJoin<PaymentMethodAccount, On<PaymentMethodAccount.paymentMethodID, Equal<LocationAPPaymentInfo.vPaymentMethodID>, And<PaymentMethodAccount.branchID, Equal<APInvoice.branchID>, And<PaymentMethodAccount.useForAP, Equal<True>, And<PaymentMethodAccount.aPIsDefault, Equal<True>>>>>,
			LeftJoin<CashAccount, 
                On<CashAccount.cashAccountID, Equal<APInvoice.payAccountID>,
                Or<APInvoice.payAccountID, IsNull, And<CashAccount.cashAccountID, Equal<LocationAPPaymentInfo.vCashAccountID>,
                Or<APInvoice.payAccountID, IsNull, And<LocationAPPaymentInfo.vCashAccountID, IsNull, And<CashAccount.cashAccountID, Equal<PaymentMethodAccount.cashAccountID>>>>>>>,
			InnerJoin<CurrencyInfo, On<CurrencyInfo.curyInfoID, Equal<APInvoice.curyInfoID>>,
            LeftJoin<APAdjust, On<APAdjust.adjdDocType, Equal<APInvoice.docType>, And<APAdjust.adjdRefNbr, Equal<APInvoice.refNbr>, And<APAdjust.released, Equal<False>>>>>>>>>>,
		Where<APInvoice.openDoc, Equal<True>,
			And2<Where<APInvoice.released, Equal<True>,Or<APInvoice.prebooked,Equal<True>>>, 
			And<APInvoice.dueDate, IsNotNull,
			And<APAdjust.adjgRefNbr, IsNull,
            And<CashAccount.cashAccountID, IsNotNull>>>>>>))]
    [Serializable]
	public partial class APCashRequirementsReport : PX.Data.IBqlTable
	{
        #region BranchID
        public abstract class branchID : PX.Data.IBqlField
        {
        }
        protected Int32? _BranchID;
        [Branch(BqlField = typeof(APInvoice.branchID))]
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
        #region PayAccountID
		public abstract class payAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _PayAccountID;
		[CashAccount(BqlField = typeof(CashAccount.cashAccountID))]
		public virtual Int32? PayAccountID
		{
			get
			{
				return this._PayAccountID;
			}
			set
			{
				this._PayAccountID = value;
			}
		}
		#endregion
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[Vendor(BqlField = typeof(APInvoice.vendorID))]
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
		#region PayTypeID
		public abstract class payTypeID : PX.Data.IBqlField
		{
		}
		protected String _PayTypeID;
        [PXDBString(10, IsUnicode = true, BqlField = typeof(LocationAPPaymentInfo.vPaymentMethodID))]
        [PXUIField(DisplayName = "Payment Method", Visibility = PXUIVisibility.Visible)]
		public virtual String PayTypeID
		{
			get
			{
				return this._PayTypeID;
			}
			set
			{
				this._PayTypeID = value;
			}
		}
		#endregion
		#region TermsID
		public abstract class termsID : PX.Data.IBqlField
		{
		}
		protected String _TermsID;
		[PXDBString(10, IsUnicode = true, BqlField = typeof(APInvoice.termsID))]
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
		#region DocType
		public abstract class docType : PX.Data.IBqlField
		{
		}
		protected string _DocType;
		[PXDBString(3, IsKey = true, IsFixed = true, BqlField = typeof(APInvoice.docType))]
		[APInvoiceType.List()]
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
		#region PrintDocType
		public abstract class printDocType : PX.Data.IBqlField
		{
		}
		[PXString(3, IsFixed = true)]
		[APDocType.PrintList()]
		[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.Visible, Enabled = true)]
		public virtual String PrintDocType
		{
			get
			{
				return this._DocType;
			}
			set
			{
			}
		}
		#endregion
		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
		}
		protected string _RefNbr;
        [PXDBString(15, IsUnicode = true, IsKey = true, BqlField = typeof(APInvoice.refNbr))]
        [PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region InvoiceNbr
		public abstract class invoiceNbr : PX.Data.IBqlField
		{
		}
		protected String _InvoiceNbr;
        [PXDBString(40, IsUnicode = true, BqlField = typeof(APInvoice.invoiceNbr))]
        [PXUIField(DisplayName = "Vendor Ref.", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region DueDate
		public abstract class dueDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _DueDate;
        [PXDBDate(BqlField = typeof(APInvoice.dueDate))]
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
        [PXDBDate(BqlField = typeof(APInvoice.discDate))]
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
		#region PayDate
		public abstract class payDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _PayDate;
		[PXDBDate(BqlField = typeof(APInvoice.payDate))]
		public virtual DateTime? PayDate
		{
			get
			{
				return this._PayDate;
			}
			set
			{
				this._PayDate = value;
			}
		}
		#endregion
		#region PaySel
		public abstract class paySel : PX.Data.IBqlField
		{
		}
		protected Boolean? _PaySel;
        [PXDBBool(BqlField = typeof(APInvoice.paySel))]
        [PXUIField(DisplayName = "Approved For Payment")]
		public virtual Boolean? PaySel
		{
			get
			{
				return this._PaySel;
			}
			set
			{
				this._PaySel = value;
			}
		}
		#endregion
		#region EstPayDate
		public abstract class estPayDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _EstPayDate;
        [PXDBDate(BqlField = typeof(APInvoice.estPayDate))]
        [PXUIField(DisplayName = "Estimated Pay Date", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual DateTime? EstPayDate
		{
			get
			{
				return this._EstPayDate;
			}
			set
			{
				this._EstPayDate = value;
			}
		}
		#endregion
		#region DocDesc
		public abstract class docDesc : PX.Data.IBqlField
		{
		}
		protected string _DocDesc;
		[PXDBString(60, IsUnicode=true, BqlField = typeof(APInvoice.docDesc))]
		public virtual string DocDesc
		{
			get
			{
				return this._DocDesc;
			}
			set
			{
				this._DocDesc = value;
			}
		}
		#endregion
		#region CuryInfoID
		public abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _CuryInfoID;
		[PXDBLong(BqlField = typeof(APInvoice.curyInfoID))]
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
        #region CuryRateType
        public abstract class curyRateType : PX.Data.IBqlField
        {
        }
        protected string _CuryRateType;
        [PXDBString(6, IsUnicode = true, BqlField = typeof(CurrencyInfo.curyRateTypeID))]
        public virtual string CuryRateType
        {
            get
            {
                return this._CuryRateType;
            }
            set
            {
                this._CuryRateType = value;
            }
        }
        #endregion
		#region BaseCuryID
		public abstract class baseCuryID : PX.Data.IBqlField
		{
		}
		protected string _BaseCuryID;
		[PXDBString(5, IsUnicode = true, BqlField = typeof(CurrencyInfo.baseCuryID))]
		public virtual string BaseCuryID
		{
			get
			{
				return this._BaseCuryID;
			}
			set
			{
				this._BaseCuryID = value;
			}
		}
		#endregion
		#region CashCuryID
		public abstract class cashCuryID : PX.Data.IBqlField
		{
		}
		protected string _CashCuryID;
        [PXDBString(5, IsUnicode = true, BqlField = typeof(CashAccount.curyID))]
        [PXUIField(DisplayName = "Cash Account Currency")]
		public virtual string CashCuryID
		{
			get
			{
				return this._CashCuryID;
			}
			set
			{
				this._CashCuryID = value;
			}
		}
		#endregion
		#region DocCuryID
		public abstract class docCuryID : PX.Data.IBqlField
		{
		}
		protected string _DocCuryID;
        [PXDBString(5, IsUnicode = true, BqlField = typeof(CurrencyInfo.curyID))]
        [PXUIField(DisplayName = "Document Currency")]
		public virtual string DocCuryID
		{
			get
			{
				return this._DocCuryID;
			}
			set
			{
				this._DocCuryID = value;
			}
		}
		#endregion
		#region OrigCuryMultDiv
		public abstract class origCuryMultDiv : PX.Data.IBqlField
		{
		}
		protected string _OrigCuryMultDiv;
		[PXDBString(1, IsFixed = true, BqlField = typeof(CurrencyInfo.curyMultDiv))]
		public virtual string OrigCuryMultDiv
		{
			get
			{
				return this._OrigCuryMultDiv;
			}
			set
			{
				this._OrigCuryMultDiv = value;
			}
		}
		#endregion
		#region OrigCuryRate
		public abstract class origCuryRate : PX.Data.IBqlField
		{
		}
		protected Decimal? _OrigCuryRate;
		[PXDBDecimal(6, BqlField = typeof(CurrencyInfo.curyRate))]
		public virtual Decimal? OrigCuryRate
		{
			get
			{
				return this._OrigCuryRate;
			}
			set
			{
				this._OrigCuryRate = value;
			}
		}
		#endregion
		#region CuryOrigDocAmt
		public abstract class curyOrigDocAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryOrigDocAmt;
        [PXDBCury(typeof(APPaySelReport.cashCuryID), BqlField = typeof(APInvoice.curyOrigDocAmt))]
		public virtual Decimal? CuryOrigDocAmt
		{
			get
			{
				return this._CuryOrigDocAmt;
			}
			set
			{
				this._CuryOrigDocAmt = value;
			}
		}
		#endregion
		#region OrigDocAmt
		public abstract class origDocAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _OrigDocAmt;
		[PXDBBaseCury(BqlField = typeof(APInvoice.origDocAmt))]
		public virtual Decimal? OrigDocAmt
		{
			get
			{
				return this._OrigDocAmt;
			}
			set
			{
				this._OrigDocAmt = value;
			}
		}
		#endregion
		#region CuryDocBal
		public abstract class curyDocBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryDocBal;
        [PXDBCury(typeof(APPaySelReport.cashCuryID), BqlField = typeof(APInvoice.curyDocBal))]
		public virtual Decimal? CuryDocBal
		{
			get
			{
				return this._CuryDocBal;
			}
			set
			{
				this._CuryDocBal = value;
			}
		}
		#endregion
		#region DocBal
		public abstract class docBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _DocBal;
		[PXDBBaseCury(BqlField = typeof(APInvoice.docBal))]
		public virtual Decimal? DocBal
		{
			get
			{
				return this._DocBal;
			}
			set
			{
				this._DocBal = value;
			}
		}
		#endregion
		#region CuryDiscBal
		public abstract class curyDiscBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryDiscBal;
        [PXDBCury(typeof(APPaySelReport.cashCuryID), BqlField = typeof(APInvoice.curyDiscBal))]
		public virtual Decimal? CuryDiscBal
		{
			get
			{
				return this._CuryDiscBal;
			}
			set
			{
				this._CuryDiscBal = value;
			}
		}
		#endregion
		#region DiscBal
		public abstract class discBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _DiscBal;
		[PXDBBaseCury(BqlField = typeof(APInvoice.discBal))]
		public virtual Decimal? DiscBal
		{
			get
			{
				return this._DiscBal;
			}
			set
			{
				this._DiscBal = value;
			}
		}
		#endregion
		#region CuryPayOrigDocAmt
		public abstract class curyPayOrigDocAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryPayOrigDocAmt;
        [PXCury(typeof(APPaySelReport.cashCuryID))]
		public virtual Decimal? CuryPayOrigDocAmt
		{
            [PXDependsOnFields(typeof(curyOrigDocAmt), typeof(origDocAmt))]
            get
			{
				return this._CuryPayOrigDocAmt;
			}
			set
			{
				this._CuryPayOrigDocAmt = value;
			}
		}
		#endregion
		#region PayOrigDocAmt
		public abstract class payOrigDocAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _PayOrigDocAmt;
		[PXDBBaseCury(BqlField = typeof(APInvoice.origDocAmt))]
		public virtual Decimal? PayOrigDocAmt
		{
			get
			{
				return this._PayOrigDocAmt;
			}
			set
			{
				this._PayOrigDocAmt = value;
			}
		}
		#endregion
		#region CuryPayDocBal
		public abstract class curyPayDocBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryPayDocBal;
        [PXCury(typeof(APPaySelReport.cashCuryID))]
        [PXUIField(DisplayName = "Balance", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? CuryPayDocBal
		{
            [PXDependsOnFields(typeof(curyDocBal), typeof(docBal))]
            get
			{
				return this._CuryPayDocBal;
			}
			set
			{
				this._CuryPayDocBal = value;
			}
		}
		#endregion
		#region PayDocBal
		public abstract class payDocBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _PayDocBal;
		[PXDBBaseCury(BqlField = typeof(APInvoice.docBal))]
		public virtual Decimal? PayDocBal
		{
			get
			{
				return this._PayDocBal;
			}
			set
			{
				this._PayDocBal = value;
			}
		}
		#endregion
		#region CuryPayDiscBal
		public abstract class curyPayDiscBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryPayDiscBal;
        [PXCury(typeof(APPaySelReport.cashCuryID))]
        [PXUIField(DisplayName = "Cash Discount Balance", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? CuryPayDiscBal
		{
            [PXDependsOnFields(typeof(curyDiscBal), typeof(discBal))]
            get
			{
				return this._CuryPayDiscBal;
			}
			set
			{
				this._CuryPayDiscBal = value;
			}
		}
		#endregion
		#region PayDiscBal
		public abstract class payDiscBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _PayDiscBal;
		[PXDBBaseCury(BqlField = typeof(APInvoice.discBal))]
		public virtual Decimal? PayDiscBal
		{
			get
			{
				return this._PayDiscBal;
			}
			set
			{
				this._PayDiscBal = value;
			}
		}
		#endregion
		#region SignBalance
		public abstract class signBalance : PX.Data.IBqlField { }
		[PXDecimal(0)]
		[PXUIField(DisplayName = "SignBalance", Visibility = PXUIVisibility.Invisible)]
		public virtual Decimal? SignBalance
		{
			[PXDependsOnFields(typeof(docType))]
			get
			{
				return APDocType.SignBalance(this._DocType);
			}
			set
			{
			}
		}
		#endregion
	}
}

