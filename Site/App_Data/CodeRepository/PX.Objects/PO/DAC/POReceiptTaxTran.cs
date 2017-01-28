namespace PX.Objects.PO
{
    using System;
    using PX.Data;
    using PX.Objects.TX;
    using PX.Objects.CM;
    using PX.Objects.CS;

    [System.SerializableAttribute()]
    public partial class POReceiptTaxTran : TaxDetail, PX.Data.IBqlTable
    {
        #region RecordID
        public abstract class recordID : PX.Data.IBqlField
        {
        }
        protected Int32? _RecordID;
        [PXDBIdentity(IsKey = true)]
        public virtual Int32? RecordID
        {
            get
            {
                return this._RecordID;
            }
            set
            {
                this._RecordID = value;
            }
        }
        #endregion
        #region ReceiptNbr
        public abstract class receiptNbr : PX.Data.IBqlField
        {
        }
        protected String _ReceiptNbr;
        [PXDBString(15, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXDBDefault(typeof(POReceipt.receiptNbr))]
        [PXUIField(DisplayName = "Receipt Nbr.", Enabled = false, Visible = false)]
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
        [PXDefault(POReceiptTaxTran.LineNbrValue)]
        [PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false)]
		[PXParent(typeof(Select<POReceipt, Where<POReceipt.receiptNbr, Equal<Current<POReceiptTaxTran.receiptNbr>>>>))]
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
        #region TaxID
        public abstract class taxID : PX.Data.IBqlField
        {
        }
        [PXDBString(Tax.taxID.Length, IsUnicode = true, IsKey = true)]
        [PXDefault()]
        [PXUIField(DisplayName = "Tax ID", Visibility = PXUIVisibility.Visible)]
		[PXSelector(typeof(Tax.taxID), DescriptionField = typeof(Tax.descr))]
        public override String TaxID
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
        #region JurisType
        public abstract class jurisType : PX.Data.IBqlField
        {
        }
        protected String _JurisType;
        [PXDBString(9, IsUnicode = true)]
        [PXUIField(DisplayName = "Tax Jurisdiction Type")]
        public virtual String JurisType
        {
            get
            {
                return this._JurisType;
            }
            set
            {
                this._JurisType = value;
            }
        }
        #endregion
        #region JurisName
        public abstract class jurisName : PX.Data.IBqlField
        {
        }
        protected String _JurisName;
        [PXDBString(200, IsUnicode = true)]
        [PXUIField(DisplayName = "Tax Jurisdiction Name")]
        public virtual String JurisName
        {
            get
            {
                return this._JurisName;
            }
            set
            {
                this._JurisName = value;
            }
        }
        #endregion
        #region TaxRate
        public abstract class taxRate : PX.Data.IBqlField
        {
        }
        #endregion
        #region CuryInfoID
        public abstract class curyInfoID : PX.Data.IBqlField
        {
        }
        [PXDBLong()]
        [CurrencyInfo(typeof(POReceipt.curyInfoID))]
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
        #region CuryTaxableAmt
        public abstract class curyTaxableAmt : PX.Data.IBqlField
        {
        }
		protected decimal? _CuryTaxableAmt;
        [PXDBCurrency(typeof(POReceiptTaxTran.curyInfoID), typeof(POReceiptTaxTran.taxableAmt))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Taxable Amount", Visibility = PXUIVisibility.Visible)]
        [PXUnboundFormula(typeof(Switch<Case<Where<WhereExempt<POReceiptTaxTran.taxID>>, POReceiptTaxTran.curyTaxableAmt>, decimal0>), typeof(SumCalc<POReceipt.curyVatExemptTotal>))]
        [PXUnboundFormula(typeof(Switch<Case<Where<WhereTaxable<POReceiptTaxTran.taxID>>, POReceiptTaxTran.curyTaxableAmt>, decimal0>), typeof(SumCalc<POReceipt.curyVatTaxableTotal>))]
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
		#region CuryUnbilledTaxableAmt
		public abstract class curyUnbilledTaxableAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryUnbilledTaxableAmt;
		[PXDBCurrency(typeof(POTaxTran.curyInfoID), typeof(POReceiptTaxTran.unbilledTaxableAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Unbilled Taxable Amount", Visibility = PXUIVisibility.Visible)]
		public virtual Decimal? CuryUnbilledTaxableAmt
		{
			get
			{
				return this._CuryUnbilledTaxableAmt;
			}
			set
			{
				this._CuryUnbilledTaxableAmt = value;
			}
		}
		#endregion
        #region TaxableAmt
        public abstract class taxableAmt : PX.Data.IBqlField
        {
        }
		protected Decimal? _TaxableAmt;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Taxable Amount", Visibility = PXUIVisibility.Visible)]
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
		#region UnbilledTaxableAmt
		public abstract class unbilledTaxableAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnbilledTaxableAmt;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? UnbilledTaxableAmt
		{
			get
			{
				return this._UnbilledTaxableAmt;
			}
			set
			{
				this._UnbilledTaxableAmt = value;
			}
		}
		#endregion
        #region CuryTaxAmt
        public abstract class curyTaxAmt : PX.Data.IBqlField
        {
        }
		protected decimal? _CuryTaxAmt;
        [PXDBCurrency(typeof(POReceiptTaxTran.curyInfoID), typeof(POReceiptTaxTran.taxAmt))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Tax Amount", Visibility = PXUIVisibility.Visible)]
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
		#region CuryUnbilledTaxAmt
		public abstract class curyUnbilledTaxAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryUnbilledTaxAmt;
		[PXDBCurrency(typeof(POTaxTran.curyInfoID), typeof(POReceiptTaxTran.unbilledTaxAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Unbilled Tax Amount", Visibility = PXUIVisibility.Visible)]
		public virtual Decimal? CuryUnbilledTaxAmt
		{
			get
			{
				return this._CuryUnbilledTaxAmt;
			}
			set
			{
				this._CuryUnbilledTaxAmt = value;
			}
		}
		#endregion
        #region TaxAmt
        public abstract class taxAmt : PX.Data.IBqlField
        {
        }
		protected Decimal? _TaxAmt;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Tax Amount", Visibility = PXUIVisibility.Visible)]
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
		#region CuryExpenseAmt
		public abstract class curyExpenseAmt : PX.Data.IBqlField
		{
		}
		#endregion
		#region UnbilledTaxAmt
		public abstract class unbilledTaxAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnbilledTaxAmt;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? UnbilledTaxAmt
		{
			get
			{
				return this._UnbilledTaxAmt;
			}
			set
			{
				this._UnbilledTaxAmt = value;
			}
		}
		#endregion
		public class lineNbrValue : Constant<int>
        {
            public lineNbrValue() : base(LineNbrValue) { ;}
        }

        public const int LineNbrValue = Int32.MaxValue;
    }
}
