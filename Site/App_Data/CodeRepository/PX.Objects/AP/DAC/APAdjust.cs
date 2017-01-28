using System;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.TX;
	
namespace PX.Objects.AP
{
    /// <summary>
    /// Represents the adjustments to Accounts Payable documents and serves to link
    /// adjusted and adjusting documents together.
    /// </summary>
	[System.SerializableAttribute()]
	[PXPrimaryGraph(
		new Type[] { typeof(APPaymentEntry)},
		new Type[] { typeof(Select<APPayment, 
			Where<APPayment.docType, Equal<Current<APAdjust.adjgDocType>>,
			And<APPayment.refNbr, Equal<Current<APAdjust.adjgRefNbr>>>>>)
		})]
	[PXCacheName(Messages.APAdjust)]    
	public partial class APAdjust : PX.Data.IBqlTable, IAdjustment
	{
		#region Selected
		public abstract class selected : PX.Data.IBqlField
		{
		}
		protected bool? _Selected = false;

        /// <summary>
        /// Indicates whether the record is selected for mass processing or not.
        /// </summary>
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
		#region SeparateCheck
		public abstract class separateCheck : PX.Data.IBqlField
		{
		}
		protected Boolean? _SeparateCheck;

        /// <summary>
        /// When set to <c>true</c> indicates that the adjusted document should be paid for by a separate check.
        /// (See <see cref="APInvoice.SeparateCheck" />)
        /// </summary>
		[PXBool()]
		[PXUIField(DisplayName = "Pay Separately", Visibility = PXUIVisibility.Visible)]
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
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;

        /// <summary>
        /// Identifier of the <see cref="Vendor"/>, whom the related documents belongs.
        /// </summary>
		[Vendor(Visibility = PXUIVisibility.Visible, Visible = false)]
		[PXDBDefault(typeof(APPayment.vendorID))]
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
		#region AdjgDocType
		public abstract class adjgDocType : PX.Data.IBqlField
		{
		}
		protected String _AdjgDocType;

        /// <summary>
        /// [key] The type of the adjusting document.
        /// </summary>
		[PXDBString(3, IsKey = true, IsFixed = true, InputMask = "")]
		[PXDBDefault(typeof(APPayment.docType))]
		[PXUIField(DisplayName = "AdjgDocType", Visibility = PXUIVisibility.Visible, Visible = false)]
		public virtual String AdjgDocType
		{
			get
			{
				return this._AdjgDocType;
			}
			set
			{
				this._AdjgDocType = value;
			}
		}
		#endregion
		#region PrintAdjgDocType
		public abstract class printAdjgDocType : PX.Data.IBqlField
		{
		}

        /// <summary>
        /// The type of the adjusting document for printing. Internal representation is the same as for <see cref="AdjgDocType"/>,
        /// but the user-friendly values of these two fields differ.
        /// </summary>
		[PXString(3, IsFixed = true)]
		[APDocType.PrintList()]
		[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.Visible, Enabled = true)]
		public virtual String PrintAdjgDocType
		{
			get
			{
				return this._AdjgDocType;
			}
			set
			{
			}
		}
		#endregion
		#region AdjgRefNbr
		public abstract class adjgRefNbr : PX.Data.IBqlField
		{
		}
		protected String _AdjgRefNbr;
        
        /// <summary>
        /// [key] Reference number of the adjusting document.
        /// </summary>
		[PXDBString(15, IsUnicode = true, IsKey = true)]
		[PXDBDefault(typeof(APPayment.refNbr))]
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.Visible, Visible = false)]
		[PXParent(typeof(Select<APPayment,Where<APPayment.docType,Equal<Current<APAdjust.adjgDocType>>, And<APPayment.refNbr,Equal<Current<APAdjust.adjgRefNbr>>, And<APPayment.lineCntr,Equal<Current<APAdjust.adjNbr>>>>>>))]
		public virtual String AdjgRefNbr
		{
			get
			{
				return this._AdjgRefNbr;
			}
			set
			{
				this._AdjgRefNbr = value;
			}
		}
		#endregion
		#region AdjgBranchID
		public abstract class adjgBranchID : PX.Data.IBqlField
		{
		}
		protected Int32? _AdjgBranchID;

        /// <summary>
        /// Identifier of the <see cref="Branch"/>, to which the adjusting document belongs.
        /// </summary>
		[Branch(typeof(APPayment.branchID))]
		public virtual Int32? AdjgBranchID
		{
			get
			{
				return this._AdjgBranchID;
			}
			set
			{
				this._AdjgBranchID = value;
			}
		}
		#endregion
		#region AdjdCuryInfoID
		public abstract class adjdCuryInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _AdjdCuryInfoID;

        /// <summary>
        /// Identifier of the <see cref="CurrencyInfo">Currency Info</see> record associated with the adjusted document.
        /// </summary>
		[PXDBLong()]
		[PXDefault()]
		[CurrencyInfo(ModuleCode = "AP", CuryIDField = "AdjdCuryID", Enabled = false)]
		public virtual Int64? AdjdCuryInfoID
		{
			get
			{
				return this._AdjdCuryInfoID;
			}
			set
			{
				this._AdjdCuryInfoID = value;
			}
		}
		#endregion
		#region AdjdDocType
		public abstract class adjdDocType : PX.Data.IBqlField
		{
		}
		protected String _AdjdDocType;

        /// <summary>
        /// [key] The type of the adjusted document.
        /// </summary>
		[PXDBString(3, IsKey = true, IsFixed = true, InputMask = "")]
		[PXDefault(APDocType.Invoice)]
		[PXUIField(DisplayName = "Document Type", Visibility=PXUIVisibility.Visible)]
		[APInvoiceType.AdjdList()]
		public virtual String AdjdDocType
		{
			get
			{
				return this._AdjdDocType;
			}
			set
			{
				this._AdjdDocType = value;
			}
		}
		#endregion
		#region PrintAdjdDocType
		public abstract class printAdjdDocType : PX.Data.IBqlField
		{
		}

        /// <summary>
        /// The type of the adjusted document for printing. Internal representation is the same as for <see cref="AdjgDocType"/>,
        /// but the user-friendly values of these two fields differ.
        /// </summary>
		[PXString(3, IsFixed = true)]
		[APDocType.PrintList()]
		[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.Visible, Enabled = true)]
		public virtual String PrintAdjdDocType
		{
			get
			{
				return this._AdjdDocType;
			}
			set
			{
			}
		}
		#endregion
		#region AdjdRefNbr
		[PXHidden()]
		[PXProjection(typeof(Select2<Standalone.APRegister, LeftJoin<Standalone.APInvoice, On<Standalone.APInvoice.docType, Equal<Standalone.APRegister.docType>, And<Standalone.APInvoice.refNbr, Equal<Standalone.APRegister.refNbr>>>>>))]
        [Serializable]
		public partial class APInvoice : Standalone.APRegister
		{
			#region DocType
			public new abstract class docType : PX.Data.IBqlField
			{
			}
			#endregion
			#region RefNbr
			public new abstract class refNbr : PX.Data.IBqlField
			{
			}
			#endregion
			#region CustomerID
			public new abstract class vendorID : PX.Data.IBqlField
			{
			}
			#endregion
			#region Released
			public new abstract class released : PX.Data.IBqlField
			{
			}
			#endregion
			#region OpenDoc
			public new abstract class openDoc : PX.Data.IBqlField
			{
			}
			#endregion
			#region DocDate
			public new abstract class docDate : PX.Data.IBqlField
			{
			}
			#endregion
			#region FinPeriodID
			public new abstract class finPeriodID : PX.Data.IBqlField
			{
			}
			#endregion
			#region DueDate
			public abstract class dueDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _DueDate;
			[PXDBDate(BqlField = typeof(Standalone.APInvoice.dueDate))]
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
			#region InvoiceNbr
			public abstract class invoiceNbr : PX.Data.IBqlField
			{
			}
			protected String _InvoiceNbr;
			[PXDBString(40, IsUnicode = true, BqlField = typeof(Standalone.APInvoice.invoiceNbr))]
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
		}
		public abstract class adjdRefNbr : PX.Data.IBqlField
		{
		}
		protected String _AdjdRefNbr;

        /// <summary>
        /// [key] Reference number of the adjusted document.
        /// </summary>
		[PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXDefault()]
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.Visible)]
		[APInvoiceType.AdjdRefNbr(typeof(Search2<APInvoice.refNbr, 
			LeftJoin<APAdjust, On<APAdjust.adjdDocType, Equal<APInvoice.docType>, And<APAdjust.adjdRefNbr, Equal<APInvoice.refNbr>, And<APAdjust.released, Equal<False>, And<Where<APAdjust.adjgDocType, NotEqual<Current<APPayment.docType>>, Or<APAdjust.adjgRefNbr, NotEqual<Current<APPayment.refNbr>>>>>>>>,
            LeftJoin<APAdjust2, On<APAdjust2.adjgDocType, Equal<APInvoice.docType>, And<APAdjust2.adjgRefNbr, Equal<APInvoice.refNbr>, And<APAdjust2.released, Equal<False>, And<APAdjust2.voided, Equal<False>>>>>,
			LeftJoin<APPayment, On<APPayment.docType, Equal<APInvoice.docType>,
				And<APPayment.refNbr, Equal<APInvoice.refNbr>, And<
				Where<APPayment.docType, Equal<APDocType.prepayment>, Or<APPayment.docType, Equal<APDocType.debitAdj>>>>>>>>>,
			Where<APInvoice.vendorID, Equal<Optional<APPayment.vendorID>>, And<APInvoice.docType, Equal<Optional<APAdjust.adjdDocType>>, And2<Where<APInvoice.released, Equal<True>, Or<APInvoice.prebooked, Equal<True>>>, And<APInvoice.openDoc, Equal<True>, And<APAdjust.adjgRefNbr, IsNull, And<APAdjust2.adjdRefNbr, IsNull,
				And2<Where<APPayment.refNbr, IsNull, And<Current<APPayment.docType>, NotEqual<APDocType.refund>, Or<APPayment.refNbr, IsNotNull, And<Current<APPayment.docType>, Equal<APDocType.refund>, Or<APPayment.docType, Equal<APDocType.debitAdj>, And<Current<APPayment.docType>, Equal<APDocType.check>, Or<APPayment.docType, Equal<APDocType.debitAdj>, And<Current<APPayment.docType>, Equal<APDocType.voidCheck>>>>>>>>>,
				And<Where<APInvoice.docDate, LessEqual<Current<APPayment.adjDate>>, And<APInvoice.finPeriodID, LessEqual<Current<APPayment.adjFinPeriodID>>, Or<Current<APPayment.adjFinPeriodID>, IsNull, Or<Current<APPayment.docType>, Equal<APDocType.check>, And<Current<APSetup.earlyChecks>, Equal<True>, Or<Current<APPayment.docType>, Equal<APDocType.voidCheck>, And<Current<APSetup.earlyChecks>, Equal<True>,
					Or<Current<APPayment.docType>, Equal<APDocType.prepayment>, And<Current<APSetup.earlyChecks>, Equal<True>>>>>>>>>>>>>>>>>>>), Filterable = true)]
		public virtual String AdjdRefNbr
		{
			get
			{
				return this._AdjdRefNbr;
			}
			set
			{
				this._AdjdRefNbr = value;
			}
		}
		#endregion
		#region AdjdBranchID
		public abstract class adjdBranchID : PX.Data.IBqlField
		{
		}
		protected Int32? _AdjdBranchID;

        /// <summary>
        /// Identifier of the <see cref="Branch"/>, to which the adjusted document belongs.
        /// </summary>
		[Branch(null)]
		public virtual Int32? AdjdBranchID
		{
			get
			{
				return this._AdjdBranchID;
			}
			set
			{
				this._AdjdBranchID = value;
			}
		}
		#endregion
		#region AdjNbr
		public abstract class adjNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _AdjNbr;

        /// <summary>
        /// The number of the adjustment.
        /// </summary>
        /// <value>
        /// Defaults to the current <see cref="APPayment.LineCntr">number of lines</see> in the related payment document.
        /// </value>
		[PXDBInt(IsKey = true)]
		[PXUIField(DisplayName = "Adjustment Nbr.", Visibility = PXUIVisibility.Visible, Visible = false, Enabled = false)]
		[PXDefault(typeof(APPayment.lineCntr))]
		public virtual Int32? AdjNbr
		{
			get
			{
				return this._AdjNbr;
			}
			set
			{
				this._AdjNbr = value;
			}
		}
		#endregion
		#region CashAccountID
		public abstract class cashAccountID : IBqlField {}
		[PXDBInt]
		public virtual int? CashAccountID { get; set; }
		#endregion
		#region PaymentMethodID
		public abstract class paymentMethodID : IBqlField {}
		[PXDBString(10, IsUnicode = true)]
		public virtual string PaymentMethodID { get; set; }
		#endregion
		#region StubNbr
		public abstract class stubNbr : IBqlField
		{
		}
		protected String _StubNbr;

        /// <summary>
        /// The number of the payment stub.
        /// </summary>
		[PXDBString(15, IsUnicode = true)]
		public virtual String StubNbr
		{
			get
			{
				return this._StubNbr;
			}
			set
			{
				this._StubNbr = value;
			}
		}
		#endregion
		#region AdjBatchNbr
		public abstract class adjBatchNbr : PX.Data.IBqlField
		{
		}
		protected String _AdjBatchNbr;

        /// <summary>
        /// The number of the <see cref="Batch"/> generated from the adjustment.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="Batch.BatchNbr"/> field.
        /// </value>
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName="Batch Number", Visibility=PXUIVisibility.Visible, Visible=true, Enabled=false)]
		public virtual String AdjBatchNbr
		{
			get
			{
				return this._AdjBatchNbr;
			}
			set
			{
				this._AdjBatchNbr = value;
			}
		}
		#endregion
		#region VoidAdjNbr
		public abstract class voidAdjNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _VoidAdjNbr;

        /// <summary>
        /// The reference number of the voiding adjustment.
        /// </summary>
		[PXDBInt()]
		public virtual Int32? VoidAdjNbr
		{
			get
			{
				return this._VoidAdjNbr;
			}
			set
			{
				this._VoidAdjNbr = value;
			}
		}
		#endregion
		#region AdjdOrigCuryInfoID
		public abstract class adjdOrigCuryInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _AdjdOrigCuryInfoID;

        /// <summary>
        /// Identifier of the original <see cref="CurrencyInfo">Currency Info</see> record associated with the adjusted document.
        /// </summary>
		[PXDBLong()]
		[PXDefault()]
		[CurrencyInfo(ModuleCode = "AP", CuryIDField = "AdjdOrigCuryID")]
		public virtual Int64? AdjdOrigCuryInfoID
		{
			get
			{
				return this._AdjdOrigCuryInfoID;
			}
			set
			{
				this._AdjdOrigCuryInfoID = value;
			}
		}
		#endregion
		#region AdjgCuryInfoID
		public abstract class adjgCuryInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _AdjgCuryInfoID;

        /// <summary>
        /// Identifier of the <see cref="CurrencyInfo">Currency Info</see> record associated with the adjusting document.
        /// </summary>
		[PXDBLong()]
		[CurrencyInfo(typeof(APPayment.curyInfoID), CuryIDField = "AdjgCuryID")]
		public virtual Int64? AdjgCuryInfoID
		{
			get
			{
				return this._AdjgCuryInfoID;
			}
			set
			{
				this._AdjgCuryInfoID = value;
			}
		}
		#endregion
		#region AdjgDocDate
		public abstract class adjgDocDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _AdjgDocDate;

        /// <summary>
        /// The date when the payment is applied.
        /// </summary>
        /// <value>
        /// Defaults to the <see cref="APPayment.AdjDate">application date specified on the adjusting document</see>.
        /// </value>
		[PXDBDate()]
		[PXDBDefault(typeof(APPayment.adjDate))]
        [PXUIField(DisplayName="Transaction Date")]
		public virtual DateTime? AdjgDocDate
		{
			get
			{
				return this._AdjgDocDate;
			}
			set
			{
				this._AdjgDocDate = value;
			}
		}
		#endregion
		#region AdjgFinPeriodID
		public abstract class adjgFinPeriodID : PX.Data.IBqlField
		{
		}
		protected String _AdjgFinPeriodID;

        /// <summary>
        /// Financial period of payment application.
        /// </summary>
        /// <value>
        /// Defaults to the <see cref="APPayment.AdjFinPeriodID">application period specified on the adjusting document</see>.
        /// </value>
		[GL.FinPeriodID()]
		[PXDBDefault(typeof(APPayment.adjFinPeriodID))]
		[PXUIField(DisplayName = "Application Period", Enabled = false)]
		public virtual String AdjgFinPeriodID
		{
			get
			{
				return this._AdjgFinPeriodID;
			}
			set
			{
				this._AdjgFinPeriodID = value;
			}
		}
		#endregion
		#region AdjgTranPeriodID
		public abstract class adjgTranPeriodID : PX.Data.IBqlField
		{
		}
		protected String _AdjgTranPeriodID;

        /// <summary>
        /// Financial period of the adjusting document. 
        /// This field corresponds to the <see cref="APPayment.AdjTranPeriodID"/> (not user-overridable) field of the adjusting document.
        /// </summary>
		[GL.FinPeriodID()]
		[PXDBDefault(typeof(APPayment.adjTranPeriodID))]
        [PXUIField(DisplayName = "Post Period")]
		public virtual String AdjgTranPeriodID
		{
			get
			{
				return this._AdjgTranPeriodID;
			}
			set
			{
				this._AdjgTranPeriodID = value;
			}
		}
		#endregion
		#region AdjdDocDate
		public abstract class adjdDocDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _AdjdDocDate;

        /// <summary>
        /// Either the date when the adjusted document was created or the date of the original vendor’s document.
        /// </summary>
		[PXDBDate()]
		[PXDefault()]
		[PXUIField(DisplayName = "Date", Visibility = PXUIVisibility.Visible, Enabled=false)]
		public virtual DateTime? AdjdDocDate
		{
			get
			{
				return this._AdjdDocDate;
			}
			set
			{
				this._AdjdDocDate = value;
			}
		}
		#endregion
		#region AdjdFinPeriodID
		public abstract class adjdFinPeriodID : PX.Data.IBqlField
		{
		}
		protected String _AdjdFinPeriodID;

        /// <summary>
        /// <see cref="FinPeriod">Financial period</see> of the adjusted document. Corresponds to the <see cref="APRegister.FinPeriodID"/> field.
        /// </summary>
        /// <value>
        /// The value of this field is determined from the <see cref="APAdjust.AdjdDocDate"/> field.
        /// </value>
		[FinPeriodID(typeof(APAdjust.adjdDocDate))]
		[PXUIField(DisplayName = "Post Period", Enabled = false)]
		public virtual String AdjdFinPeriodID
		{
			get
			{
				return this._AdjdFinPeriodID;
			}
			set
			{
				this._AdjdFinPeriodID = value;
			}
		}
		#endregion
		#region AdjdTranPeriodID
		public abstract class adjdTranPeriodID : PX.Data.IBqlField
		{
		}
		protected String _AdjdTranPeriodID;

        /// <summary>
        /// <see cref="FinPeriod">Financial period</see> of the adjusted document. Corresponds to the <see cref="APRegister.TranPeriodID"/> field.
        /// </summary>
		[TranPeriodID(typeof(APAdjust.adjdDocDate))]
		public virtual String AdjdTranPeriodID
		{
			get
			{
				return this._AdjdTranPeriodID;
			}
			set
			{
				this._AdjdTranPeriodID = value;
			}
		}
		#endregion
		#region CuryAdjgDiscAmt
		public abstract class curyAdjgDiscAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAdjgDiscAmt;

        /// <summary>
        /// The amount of the cash discount taken for the adjusting document.
        /// Presented in the currency of the document, see <see cref="APRegister.CuryID"/>.
        /// </summary>
		[PXDBCurrency(typeof(APAdjust.adjgCuryInfoID), typeof(APAdjust.adjDiscAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Cash Discount Taken", Visibility = PXUIVisibility.Visible)]
		public virtual Decimal? CuryAdjgDiscAmt
		{
			get
			{
				return this._CuryAdjgDiscAmt;
			}
			set
			{
				this._CuryAdjgDiscAmt = value;
			}
		}
		#endregion
		#region CuryAdjgWhTaxAmt
		public abstract class curyAdjgWhTaxAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAdjgWhTaxAmt;

        /// <summary>
        /// The amount of withholding tax calculated for the adjusting document, if applicable.
        /// Presented in the currency of the document, see <see cref="APRegister.CuryID"/>.
        /// </summary>
		[PXDBCurrency(typeof(APAdjust.adjgCuryInfoID), typeof(APAdjust.adjWhTaxAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "With. Tax", Visibility = PXUIVisibility.Visible)]
		public virtual Decimal? CuryAdjgWhTaxAmt
		{
			get
			{
				return this._CuryAdjgWhTaxAmt;
			}
			set
			{
				this._CuryAdjgWhTaxAmt = value;
			}
		}
		#endregion
		#region CuryAdjgAmt
		public abstract class curyAdjgAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAdjgAmt;

        /// <summary>
        /// The actual amount paid on the document.
        /// Presented in the currency of the document, see <see cref="APRegister.CuryID"/>.
        /// </summary>
		[PXDBCurrency(typeof(APAdjust.adjgCuryInfoID), typeof(APAdjust.adjAmt), BaseCalc=false)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Paid", Visibility = PXUIVisibility.Visible)]
		//[PXFormula(null, typeof(SumCalc<APPayment.curyApplAmt>))]
        [PXUnboundFormula(typeof(Mult<APAdjust.adjgBalSign, APAdjust.curyAdjgAmt>), typeof(SumCalc<APPayment.curyApplAmt>))]
		public virtual Decimal? CuryAdjgAmt
		{
			get
			{
				return this._CuryAdjgAmt;
			}
			set
			{
				this._CuryAdjgAmt = value;
			}
		}
		#endregion
		#region AdjDiscAmt
		public abstract class adjDiscAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _AdjDiscAmt;

        /// <summary>
        /// The amount of the cash discount for the adjusted document.
        /// Presented in the base currency of the company, see <see cref="Company.BaseCuryID"/>.
        /// </summary>
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Cash Discount Amount")]
		public virtual Decimal? AdjDiscAmt
		{
			get
			{
				return this._AdjDiscAmt;
			}
			set
			{
				this._AdjDiscAmt = value;
			}
		}
		#endregion
		#region CuryAdjdDiscAmt
		public abstract class curyAdjdDiscAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAdjdDiscAmt;

        /// <summary>
        /// The amount of the cash discount for the adjusted document.
        /// Presented in the currency of the document, see <see cref="CuryID"/>.
        /// </summary>
		[PXDBDecimal(4)]
		//[PXDBCurrency(typeof(APAdjust.adjdCuryInfoID), typeof(APAdjust.adjDiscAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryAdjdDiscAmt
		{
			get
			{
				return this._CuryAdjdDiscAmt;
			}
			set
			{
				this._CuryAdjdDiscAmt = value;
			}
		}
		#endregion
		#region AdjWhTaxAmt
		public abstract class adjWhTaxAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _AdjWhTaxAmt;

        /// <summary>
        /// The amount of tax withheld from the payments to the adjusted document.
        /// (Presented in the base currency of the company, see <see cref="Company.BaseCuryID"/>)
        /// </summary>
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName="Withholding Tax Amount")]
		public virtual Decimal? AdjWhTaxAmt
		{
			get
			{
				return this._AdjWhTaxAmt;
			}
			set
			{
				this._AdjWhTaxAmt = value;
			}
		}
		#endregion
		#region CuryAdjdWhTaxAmt
		public abstract class curyAdjdWhTaxAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAdjdWhTaxAmt;

        /// <summary>
        /// The amount of tax withheld from the payments to the adjusted document.
        /// (Presented in the currency of the document, see <see cref="APRegister.CuryID"/>)
        /// </summary>
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryAdjdWhTaxAmt
		{
			get
			{
				return this._CuryAdjdWhTaxAmt;
			}
			set
			{
				this._CuryAdjdWhTaxAmt = value;
			}
		}
		#endregion
		#region AdjAmt
		public abstract class adjAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _AdjAmt;

        /// <summary>
        /// The amount to be paid for the adjusted document. (See <see cref="APRegister.OrigDocAmt"/>)
        /// (Presented in the base currency of the company, see <see cref="Company.BaseCuryID"/>)
        /// </summary>
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName="Amount")]
		public virtual Decimal? AdjAmt
		{
			get
			{
				return this._AdjAmt;
			}
			set
			{
				this._AdjAmt = value;
			}
		}
		#endregion
		#region CuryAdjdAmt
		public abstract class curyAdjdAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAdjdAmt;

        /// <summary>
        /// The amount to be paid for the adjusted document. (See <see cref="APRegister.CuryOrigDocAmt"/>)
        /// (Presented in the currency of the document, see <see cref="APRegister.CuryID"/>)
        /// </summary>
		[PXDBDecimal(4)]
		//[PXDBCurrency(typeof(APAdjust.adjdCuryInfoID), typeof(APAdjust.adjAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryAdjdAmt
		{
			get
			{
				return this._CuryAdjdAmt;
			}
			set
			{
				this._CuryAdjdAmt = value;
			}
		}
		#endregion
		#region RGOLAmt
		public abstract class rGOLAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _RGOLAmt;

        /// <summary>
        /// Realized Gain and Loss amount associated with the adjustment.
        /// </summary>
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Realized Gain/Loss Amount")]
		public virtual Decimal? RGOLAmt
		{
			get
			{
				return this._RGOLAmt;
			}
			set
			{
				this._RGOLAmt = value;
			}
		}
		#endregion
		#region Released
		public abstract class released : PX.Data.IBqlField
		{
		}
		protected Boolean? _Released;

        /// <summary>
        /// When set to <c>true</c> indicates that the adjustment was released.
        /// </summary>
		[PXDBBool()]
		[PXDefault(false)]
        [PXUIField(DisplayName="Released")]
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
		#region Hold
		public abstract class hold : PX.Data.IBqlField
		{
		}
		protected Boolean? _Hold;

        /// <summary>
        /// When set to <c>true</c> indicates that the adjustment is on hold.
        /// </summary>
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? Hold
		{
			get
			{
				return this._Hold;
			}
			set
			{
				this._Hold = value;
			}
		}
		#endregion
		#region Voided
		public abstract class voided : PX.Data.IBqlField
		{
		}
		protected Boolean? _Voided;

        /// <summary>
        /// When set to <c>true</c> indicates that the adjustment was voided.
        /// </summary>
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? Voided
		{
			get
			{
				return this._Voided;
			}
			set
			{
				this._Voided = value;
			}
		}
		#endregion
		#region AdjdAPAcct
		public abstract class adjdAPAcct : PX.Data.IBqlField
		{
		}
		protected Int32? _AdjdAPAcct;

        /// <summary>
        /// Identifier of the AP account, to which the adjusted document belongs.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="Account.AccountID"/> field.
        /// </value>
		[Account(SuppressCurrencyValidation = true)]
		[PXDefault()]
		public virtual Int32? AdjdAPAcct
		{
			get
			{
				return this._AdjdAPAcct;
			}
			set
			{
				this._AdjdAPAcct = value;
			}
		}
		#endregion
		#region AdjdAPSub
		public abstract class adjdAPSub : PX.Data.IBqlField
		{
		}
		protected Int32? _AdjdAPSub;

        /// <summary>
        /// Identifier of the AP subaccount, to which the adjusted document belongs.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="Sub.SubID"/> field.
        /// </value>
		[SubAccount()]
		[PXDefault()]
		public virtual Int32? AdjdAPSub
		{
			get
			{
				return this._AdjdAPSub;
			}
			set
			{
				this._AdjdAPSub = value;
			}
		}
		#endregion
		#region AdjdWhTaxAcctID
		public abstract class adjdWhTaxAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _AdjdWhTaxAcctID;

        /// <summary>
        /// Identifier of the account associated with withholding tax for the adjusted document.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="Account.AccountID"/> field.
        /// </value>
		[Account()]
		[PXDefault(typeof(Search2<APTaxTran.accountID, InnerJoin<Tax, On<Tax.taxID, Equal<APTaxTran.taxID>>>, Where<APTaxTran.tranType, Equal<Current<APAdjust.adjdDocType>>, And<APTaxTran.refNbr, Equal<Current<APAdjust.adjdRefNbr>>, And<Tax.taxType, Equal<CSTaxType.withholding>>>>, OrderBy<Asc<APTaxTran.taxID>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Int32? AdjdWhTaxAcctID
		{
			get
			{
				return this._AdjdWhTaxAcctID;
			}
			set
			{
				this._AdjdWhTaxAcctID = value;
			}
		}
		#endregion
		#region AdjdWhTaxSubID
		public abstract class adjdWhTaxSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _AdjdWhTaxSubID;

        /// <summary>
        /// Identifier of the subaccount associated with withholding tax for the adjusted document.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="Sub.SubID"/> field.
        /// </value>
		[SubAccount()]
		[PXDefault(typeof(Search2<APTaxTran.subID, InnerJoin<Tax, On<Tax.taxID, Equal<APTaxTran.taxID>>>, Where<APTaxTran.tranType, Equal<Current<APAdjust.adjdDocType>>, And<APTaxTran.refNbr, Equal<Current<APAdjust.adjdRefNbr>>, And<Tax.taxType, Equal<CSTaxType.withholding>>>>, OrderBy<Asc<APTaxTran.taxID>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Int32? AdjdWhTaxSubID
		{
			get
			{
				return this._AdjdWhTaxSubID;
			}
			set
			{
				this._AdjdWhTaxSubID = value;
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

        /// <summary>
        /// Identifier of the <see cref="PX.Data.Note">Note</see> object, associated with the adjustment.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Data.Note.NoteID">Note.NoteID</see> field. 
        /// </value>
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
		#region AdjdCuryRate
		public abstract class adjdCuryRate : PX.Data.IBqlField
		{
		}
		protected Decimal? _AdjdCuryRate;

        /// <summary>
        /// An optional cross rate that can be specified between the currency of the payment and currency of the original document.
        /// </summary>
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXDecimal(8)]
		[PXUIField(DisplayName = "Cross Rate", Visibility = PXUIVisibility.Visible)]
		public virtual Decimal? AdjdCuryRate
		{
			get
			{
				return this._AdjdCuryRate;
			}
			set
			{
				this._AdjdCuryRate = value;
			}
		}
		#endregion
		#region CuryDocBal
		public abstract class curyDocBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryDocBal;

        /// <summary>
        /// The amount of the adjustment before the discount is taken.
        /// (Presented in the currency of the document, see <see cref="APRegister.CuryID"/>)
        /// </summary>
		[PXUnboundDefault(TypeCode.Decimal, "0.0")]
		[PXCurrency(typeof(APAdjust.adjgCuryInfoID), typeof(APAdjust.docBal), BaseCalc=false)]
		[PXUIField(DisplayName = "Balance", Visibility = PXUIVisibility.Visible, Enabled = false)]
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

        /// <summary>
        /// The amount of the adjustment before the discount is taken.
        /// (Presented in the base currency of the company, see <see cref="Company.BaseCuryID"/>)
        /// </summary>
		[PXDecimal(4)]
		[PXUnboundDefault(TypeCode.Decimal, "0.0")]
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

        /// <summary>
        /// The difference between the cash discount that was available and the actual amount of cash discount taken.
        /// (Presented in the currency of the document, see <see cref="APRegister.CuryID"/>)
        /// </summary>
		[PXCurrency(typeof(APAdjust.adjgCuryInfoID), typeof(APAdjust.discBal), BaseCalc=false)]
		[PXUnboundDefault()]
		[PXUIField(DisplayName = "Cash Discount Balance", Visibility = PXUIVisibility.Visible, Enabled = false)]
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

        /// <summary>
        /// The difference between the cash discount that was available and the actual amount of cash discount taken.
        /// (Presented in the base currency of the company, see <see cref="Company.BaseCuryID"/>)
        /// </summary>
		[PXDecimal(4)]
		[PXUnboundDefault()]
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
		#region CuryWhTaxBal
		public abstract class curyWhTaxBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryWhTaxBal;

        /// <summary>
        /// The difference between the amount of the tax to be withheld and the actual withheld amount (if any withholding taxes are applicable).
        /// (Presented in the currency of the document, see <see cref="APRegister.CuryID"/>)
        /// </summary>
		[PXCurrency(typeof(APAdjust.adjgCuryInfoID), typeof(APAdjust.whTaxBal), BaseCalc = false)]
		[PXUnboundDefault()]
		[PXUIField(DisplayName = "With. Tax Balance", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public virtual Decimal? CuryWhTaxBal
		{
			get
			{
				return this._CuryWhTaxBal;
			}
			set
			{
				this._CuryWhTaxBal = value;
			}
		}
		#endregion
		#region WhTaxBal
		public abstract class whTaxBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _WhTaxBal;

        /// <summary>
        /// The difference between the amount of the tax to be withheld and the actual withheld amount (if any withholding taxes are applicable).
        /// (Presented in the base currency of the company, see <see cref="Company.BaseCuryID"/>)
        /// </summary>
		[PXDecimal(4)]
		[PXUnboundDefault()]
		public virtual Decimal? WhTaxBal
		{
			get
			{
				return this._WhTaxBal;
			}
			set
			{
				this._WhTaxBal = value;
			}
		}
		#endregion
		#region VoidAppl
		public abstract class voidAppl : PX.Data.IBqlField
		{
		}

        /// <summary>
        /// When equal to <c>true</c>, indicates that the payment was voided or cancelled.
        /// </summary>
        /// <value>
        /// Setting this field to <c>true</c> will change the <see cref="AdjgDocType">type of the adjusting document</see> to Void Check (<c>"VCK"</c>)
        /// and set <see cref="Voided"/> to <c>true</c>.
        /// </value>
		[PXBool()]
		[PXUIField(DisplayName = "Void Application", Visibility = PXUIVisibility.Visible)]
		[PXDefault(false)]
		public virtual Boolean? VoidAppl
		{
			[PXDependsOnFields(typeof(adjgDocType))]
			get
			{
				return APPaymentType.VoidAppl(this._AdjgDocType);
			}
			set
			{
				if ((bool) value)
				{
					this._AdjgDocType = APDocType.VoidCheck;
					this.Voided = true;
				}
			}
		}
		#endregion
		#region ReverseGainLoss
		public abstract class reverseGainLoss : PX.Data.IBqlField
		{
		}

        /// <summary>
        /// A read-only field, which when equal to <c>true</c>, indicates that the sign of the <see cref="RGOLAmt">gain and loss</see> is reversed for the adjustment.
        /// </summary>
		[PXDependsOnFields(typeof(adjgDocType), typeof(adjdDocType))]
		public virtual Boolean? ReverseGainLoss
		{

            get
            {

				return (AdjgTBSign == -1m);
			}
			set
			{
			}
		}
		#endregion
        #region AdjgBalSign
        public abstract class adjgBalSign : PX.Data.IBqlField
        {
        }

        /// <summary>
        /// A read-only field showing the sign of the impact of the adjusting document on the balance.
        /// Depends only on the values of the <see cref="AdjgDocType"/> and <see cref="AdjdDocType"/> fields.
        /// </summary>
        [PXDependsOnFields(typeof(adjgDocType), typeof(adjdDocType))]
        public virtual decimal? AdjgBalSign
        {
            get
            {
				return this.AdjgDocType == APDocType.Check && this.AdjdDocType == APDocType.DebitAdj || this.AdjgDocType == APDocType.VoidCheck && this.AdjdDocType == APDocType.DebitAdj ? -1m : 1m;
            }
            set { }
        }
        #endregion
		#region AdjgGLSign
		public abstract class adjgGLSign : PX.Data.IBqlField
		{
		}

        /// <summary>
        /// !REV! A read-only field showing the sign of impact of the adjusting document on the General Ledger.
        /// Depends only on the values of the <see cref="AdjgDocType"/> and <see cref="AdjdDocType"/> fields.
        /// </summary>
		[PXDependsOnFields(typeof(adjgDocType), typeof(adjdDocType))]
		public virtual decimal? AdjgGLSign
		{
			get
			{
				return this.AdjgDocType == APDocType.Check && this.AdjdDocType == APDocType.Prepayment || this.AdjgDocType == APDocType.VoidCheck && this.AdjdDocType == APDocType.Prepayment ? 1m : APDocType.SignAmount(this.AdjdDocType);
			}
			set { }
		}
		#endregion
        #region AdjgTBSign
        public abstract class adjgTBSign : PX.Data.IBqlField
        {
        }

        /// <summary>
        /// A read-only field showing the sign of impact of the adjusting document on the trial balance.
        /// Depends only on the values of the <see cref="AdjgDocType"/> and <see cref="AdjdDocType"/> fields.
        /// </summary>
        [PXDependsOnFields(typeof(adjgDocType), typeof(adjdDocType))]
        public virtual decimal? AdjgTBSign
        {
            get
            {
                return this.AdjgDocType == APDocType.Check && this.AdjdDocType == APDocType.Prepayment || this.AdjgDocType == APDocType.VoidCheck && this.AdjdDocType == APDocType.Prepayment ? 1m : APDocType.SignBalance(this.AdjdDocType);
            }
            set { }
        }
        #endregion
        #region AdjdTBSign
        public abstract class adjdTBSign : PX.Data.IBqlField
        {
        }

        /// <summary>
        /// A read-only field showing the sign of impact of the adjusted document on the trial balance.
        /// Depends only on the values of the <see cref="AdjgDocType"/> and <see cref="AdjdDocType"/> fields.
        /// </summary>
        [PXDependsOnFields(typeof(adjgDocType), typeof(adjdDocType))]
        public virtual decimal? AdjdTBSign
        {
            get
            {
				return this.AdjgDocType == APDocType.Check && this.AdjdDocType == APDocType.DebitAdj || this.AdjgDocType == APDocType.VoidCheck && this.AdjdDocType == APDocType.DebitAdj ? 1m : APDocType.SignBalance(this.AdjgDocType);
            }
            set { }
        }
        #endregion
		#region SignedRGOLAmt

        /// <summary>
        /// A read-only field showing the Realized Gain and Loss amount associated with the adjustment with proper sign.
        /// Depends on the <see cref="RGOLAmt"/> and <see cref="ReverseGainLoss"/> fields.
        /// </summary>
		public virtual Decimal? SignedRGOLAmt
		{
			[PXDependsOnFields(typeof(reverseGainLoss),typeof(rGOLAmt))]
			get
			{
				return ((bool)this.ReverseGainLoss ? -this._RGOLAmt : this._RGOLAmt);
			}			
		}
		#endregion

        #region AdjdBalSign
        public abstract class adjdBalSign : PX.Data.IBqlField
        {
        }

        /// <summary>
        /// A read-only field showing the sign of the impact of the adjusted document on the balance.
        /// Depends only on the values of the <see cref="AdjgDocType"/> and <see cref="AdjdDocType"/> fields.
        /// </summary>
        [PXDependsOnFields(typeof(adjgDocType), typeof(adjdDocType))]
        public virtual decimal? AdjdBalSign
        {
            get
            {                                
                return this.AdjgDocType == APDocType.Check && this.AdjdDocType == APDocType.DebitAdj || this.AdjgDocType == APDocType.VoidCheck && this.AdjdDocType == APDocType.DebitAdj ? -1m : 1m;
            }
            set { }
        }
        #endregion

		#region TaxInvoiceNbr
		public abstract class taxInvoiceNbr : PX.Data.IBqlField
		{
		}

		/// <summary>
		/// The "Tax Doc. Nbr" of the tax transaction, generated on the "Recognize Output/Input VAT" (TX503000/TX503500) form.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="TaxTran.TaxInvoiceNbr"/> field.
		/// </value>
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Tax Doc. Nbr", Enabled = false, Visible = false)]
		public virtual string TaxInvoiceNbr
		{
			get;
			set;
		}
		#endregion

		#region AdjType
		public abstract class adjType : IBqlField { }

		[PXString(1, IsFixed = true)]
		[ARAdjust.adjType.List]
		public virtual string AdjType { get; set; }
		#endregion

		/// <summary>
		/// These fields are required to display two-way (incoming and outgoing)
		/// applications in a same grid: e.g. displaying the adjusting document 
		/// number in case of an incoming application, and adjusted document number
		/// in case of an outgoing application. The fields are controlled by specific 
		/// BLCs, e.g. filled out in delegates or by formula.
		/// </summary>
		#region Display Fields

		#region DisplayDocType
		public abstract class displayDocType : IBqlField { }
		[PXString(3, IsFixed = true, InputMask = "")]
		[PXUIField(DisplayName = "Doc. Type")]
		[APDocType.List]
		public virtual string DisplayDocType { get; set; }
		#endregion

		#region DisplayRefNbr
		public abstract class displayRefNbr : IBqlField { }
		[PXString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXUIField(DisplayName = "Reference Nbr.")]
		[PXSelector(typeof(Search<Standalone.APRegister.refNbr, Where<Standalone.APRegister.docType, Equal<Current<APAdjust.displayDocType>>>>))]
		public virtual string DisplayRefNbr { get; set; }
		#endregion

		#region DisplayDocDate
		public abstract class displayDocDate : IBqlField { }
		[PXDate]
		[PXUIField(DisplayName = "Date", Enabled = false)]
		public virtual DateTime? DisplayDocDate { get; set; }
		#endregion

		#region DisplayDocDesc
		public abstract class displayDocDesc : IBqlField { }
		[PXString(150, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Enabled = false)]
		public virtual string DisplayDocDesc { get; set; }
		#endregion

		#region DisplayCuryID
		public abstract class displayCuryID : IBqlField { }
		[PXString(5, IsUnicode = true, InputMask = ">LLLLL")]
		[PXUIField(DisplayName = "Currency", Enabled = false)]
		[PXSelector(typeof(Currency.curyID))]
		public virtual string DisplayCuryID { get; set; }
		#endregion

		#region DisplayCuryInfoID
		public abstract class displayCuryInfoID : IBqlField { }
		[PXLong]
	    public virtual long? DisplayCuryInfoID
	    {
		    get;
			set;
	    }
		#endregion

		#region DisplayFinPeriodID
		public abstract class displayFinPeriodID : IBqlField { }
		[PXString(FiscalPeriodUtils.FULL_LENGHT, IsFixed = true)]
		[FinPeriodIDFormatting]
		[PXSelector(typeof(Search<FinPeriod.finPeriodID>))]
		[PXUIField(DisplayName = "Post Period", Enabled = false)]
		public virtual string DisplayFinPeriodID { get; set; }
		#endregion

		#region DisplayStatus
		public abstract class displayStatus : IBqlField { }
		[PXString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Status", Enabled = false)]
		[ARDocStatus.List]
		public virtual string DisplayStatus { get; set; }
		#endregion

		#region DisplayCuryAmt
		public abstract class displayCuryAmt : IBqlField { }
		[PXCurrency(typeof(APAdjust.displayCuryInfoID), typeof(APAdjust.adjAmt), BaseCalc = false)]
		[PXUIField(DisplayName = "Amount Paid", Enabled = false)]
	    public virtual decimal? DisplayCuryAmt
	    {
			get;
			set;
	    }
		#endregion

		#region DisplayCuryDiscAmt
		public abstract class displayCuryDiscAmt : IBqlField { }
		[PXCurrency(typeof(APAdjust.displayCuryInfoID), typeof(APAdjust.adjDiscAmt), BaseCalc = false)]
		[PXUIField(DisplayName = "Cash Discount Taken", Enabled = false)]
		public virtual decimal? DisplayCuryDiscAmt
	    {
		    get;
			set;
	    }
		#endregion

		#region DisplayCuryWhTaxAmt
		public abstract class displayCuryWhTaxAmt : IBqlField { }
		[PXCurrency(typeof(APAdjust.displayCuryInfoID), typeof(APAdjust.adjWhTaxAmt), BaseCalc = false)]
		[PXUIField(DisplayName = "With. Tax", Enabled = false)]
	    public virtual decimal? DisplayCuryWhTaxAmt
	    {
			get;
			set;
	    }
		#endregion

		#endregion
	}

    [Serializable]
    [PXHidden]
    public partial class APAdjust2 : APAdjust
    {
        public new abstract class adjgRefNbr : IBqlField { }
        public new abstract class adjgDocType : IBqlField { }
        public new abstract class released : IBqlField { }
        public new abstract class voided : IBqlField { }
        public new abstract class adjdRefNbr : IBqlField { }
        public new abstract class adjdDocType : IBqlField { }
    }
}

namespace PX.Objects.AP.Standalone
{
	[PXHidden]
	[Serializable]
	public partial class APRegister : AP.APRegister
	{
		#region DocType
		public new abstract class docType : IBqlField { }
		#endregion
		#region RefNbr
		public new abstract class refNbr : IBqlField { }
		#endregion
		#region CuryInfoID
		public new abstract class curyInfoID : IBqlField { }
		[PXDBLong()]
		public override Int64? CuryInfoID
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
		#region ClosedFinPeriodID
		public new abstract class closedFinPeriodID : IBqlField { }
		#endregion

		public new abstract class branchID : IBqlField { }
		public new abstract class docDate : IBqlField { }
		public new abstract class docDesc : IBqlField { }
		public new abstract class curyID : IBqlField { }
		public new abstract class finPeriodID : IBqlField { }
		public new abstract class status : IBqlField { }
	}

	[PXHidden]
	[Serializable]
	public partial class APRegister2 : AP.APRegister
	{
		#region DocType
		public new abstract class docType : IBqlField
		{
		}
		#endregion
		#region RefNbr
		public new abstract class refNbr : IBqlField
		{
		}
		#endregion
		#region CuryInfoID
		public new abstract class curyInfoID : IBqlField
		{
		}
		[PXDBLong]
		public override long? CuryInfoID
		{
			get;
			set;
		}
		#endregion
		#region ClosedFinPeriodID
		public new abstract class closedFinPeriodID : IBqlField
		{
		}
		#endregion
	}
}
