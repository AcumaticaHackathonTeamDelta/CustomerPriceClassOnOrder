using System;
using PX.Data;
using PX.Objects.TX;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.CS;

namespace PX.Objects.AR
{
    [PXProjection(typeof(Select<TaxTran, Where<TaxTran.module, Equal<BatchModule.moduleAR>>>), Persistent = true)]
    [PXCacheName(Messages.ARTaxTran)]
    [Serializable]
    public partial class ARTaxTran : TaxTran
    {
        #region Module
        public new abstract class module : PX.Data.IBqlField
        {
        }
        [PXDBString(2, IsKey = true, IsFixed = true)]
        [PXDefault((string)BatchModule.AR)]
        [PXUIField(DisplayName = "Module", Enabled = false, Visible = false)]
        public override String Module
        {
            get
            {
                return this._Module;
            }
            set
            {
                this._Module = value;
            }
        }
        #endregion
        #region TranType
        public new abstract class tranType : PX.Data.IBqlField
        {
        }
        [PXDBString(3, IsKey = true, IsFixed = true)]
        [PXDBDefault(typeof(ARRegister.docType))]
        [PXParent(typeof(Select<ARRegister, Where<ARRegister.docType, Equal<Current<TaxTran.tranType>>, And<ARRegister.refNbr, Equal<Current<TaxTran.refNbr>>>>>))]
        [PXUIField(DisplayName = "Tran. Type", Enabled = false, Visible = false)]
        public override String TranType
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
        public new abstract class refNbr : PX.Data.IBqlField
        {
        }
        [PXDBString(15, IsKey = true, IsUnicode = true)]
        [PXDBDefault(typeof(ARRegister.refNbr))]
        [PXUIField(DisplayName = "Ref. Nbr.", Enabled = false, Visible = false)]
        public override String RefNbr
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
        #region BranchID
        public new abstract class branchID : PX.Data.IBqlField
        {
        }
        [Branch(typeof(ARRegister.branchID), Enabled = false)]
        public override Int32? BranchID
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
        #region Released
        public new abstract class released : PX.Data.IBqlField
        {
        }
        #endregion
        #region Voided
        public new abstract class voided : PX.Data.IBqlField
        {
        }
        #endregion
        #region TaxPeriodID
        public new abstract class taxPeriodID : PX.Data.IBqlField
        {
        }
        [GL.FinPeriodID()]
        public override String TaxPeriodID
        {
            get
            {
                return this._TaxPeriodID;
            }
            set
            {
                this._TaxPeriodID = value;
            }
        }
        #endregion
        #region FinPeriodID
        public new abstract class finPeriodID : PX.Data.IBqlField
        {
        }
        [GL.FinPeriodID()]
        [PXDBDefault(typeof(ARRegister.finPeriodID))]
        public override String FinPeriodID
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
        #region TaxID
        public new abstract class taxID : PX.Data.IBqlField
        {
        }
        [PXDBString(Tax.taxID.Length, IsUnicode = true, IsKey = true)]
        [PXDefault()]
        [PXUIField(DisplayName = "Tax ID", Visibility = PXUIVisibility.Visible)]
        [PXSelector(typeof(Tax.taxID), DescriptionField = typeof(Tax.descr), DirtyRead = true)]
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
        #region VendorID
        public new abstract class vendorID : PX.Data.IBqlField
        {
        }
        [PXDBInt()]
        [PXDefault(typeof(Search<Tax.taxVendorID, Where<Tax.taxID, Equal<Current<ARTaxTran.taxID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        public override Int32? VendorID
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
        #region BAccountID
        public new abstract class bAccountID : PX.Data.IBqlField
        {
        }
        [PXDBInt]
        [PXDefault(typeof(ARRegister.customerID))]
        public override Int32? BAccountID
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
        #region TaxZoneID
        public new abstract class taxZoneID : PX.Data.IBqlField
        {
        }
        [PXDBString(10, IsUnicode = true)]
        [PXDefault()]
        public override String TaxZoneID
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
        #region AccountID
        public new abstract class accountID : PX.Data.IBqlField
        {
        }
        [Account()]
        [PXDefault(typeof(Search<SalesTax.histTaxAcctID, Where<SalesTax.taxID, Equal<Current<ARTaxTran.taxID>>>>))]
        public override Int32? AccountID
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
        public new abstract class subID : PX.Data.IBqlField
        {
        }
        [SubAccount()]
        [PXDefault(typeof(Search<SalesTax.histTaxSubID, Where<SalesTax.taxID, Equal<Current<ARTaxTran.taxID>>>>))]
        public override Int32? SubID
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
        #region TranDate
        public new abstract class tranDate : PX.Data.IBqlField
        {
        }
        [PXDBDate()]
        [PXDBDefault(typeof(ARRegister.docDate))]
        public override DateTime? TranDate
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
        #region TaxType
        public new abstract class taxType : PX.Data.IBqlField
        {
        }
        [PXDBString(1, IsFixed = true)]
        [PXDefault(typeof(Search<SalesTax.tranTaxType, Where<SalesTax.taxID, Equal<Current<ARTaxTran.taxID>>>>))]
        public override String TaxType
        {
            get
            {
                return this._TaxType;
            }
            set
            {
                this._TaxType = value;
            }
        }
        #endregion
        #region TaxBucketID
        public new abstract class taxBucketID : PX.Data.IBqlField
        {
        }
        [PXDBInt()]
        [PXDefault(typeof(Search<TaxRev.taxBucketID, Where<TaxRev.taxID, Equal<Current<ARTaxTran.taxID>>, And<Current<ARTaxTran.tranDate>, Between<TaxRev.startDate, TaxRev.endDate>, And2<Where<TaxRev.taxType, Equal<Current<ARTaxTran.taxType>>, Or<TaxRev.taxType, Equal<TaxType.sales>, And<Current<ARTaxTran.taxType>, Equal<TaxType.pendingSales>, Or<TaxRev.taxType, Equal<TaxType.purchase>, And<Current<ARTaxTran.taxType>, Equal<TaxType.pendingPurchase>>>>>>, And<TaxRev.outdated, Equal<False>>>>>>))]
        public override Int32? TaxBucketID
        {
            get
            {
                return this._TaxBucketID;
            }
            set
            {
                this._TaxBucketID = value;
            }
        }
        #endregion
        #region CuryInfoID
        public new abstract class curyInfoID : PX.Data.IBqlField
        {
        }
        [PXDBLong()]
        [CurrencyInfo(typeof(ARRegister.curyInfoID))]
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
        public new abstract class curyTaxableAmt : PX.Data.IBqlField
        {
        }
		[PXDBCurrency(typeof(ARTaxTran.curyInfoID), typeof(ARTaxTran.taxableAmt))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Taxable Amount", Visibility = PXUIVisibility.Visible)]
        [PXUnboundFormula(typeof(Switch<Case<WhereExempt<ARTaxTran.taxID>, ARTaxTran.curyTaxableAmt>, decimal0>), typeof(SumCalc<ARInvoice.curyVatExemptTotal>))]
        [PXUnboundFormula(typeof(Switch<Case<WhereTaxable<ARTaxTran.taxID>, ARTaxTran.curyTaxableAmt>, decimal0>), typeof(SumCalc<ARInvoice.curyVatTaxableTotal>))]
        [PXUnboundFormula(typeof(Switch<Case<WhereExempt<ARTaxTran.taxID>, ARTaxTran.curyTaxableAmt>, decimal0>), typeof(SumCalc<AR.Standalone.ARCashSale.curyVatExemptTotal>))]
        [PXUnboundFormula(typeof(Switch<Case<WhereTaxable<ARTaxTran.taxID>, ARTaxTran.curyTaxableAmt>, decimal0>), typeof(SumCalc<AR.Standalone.ARCashSale.curyVatTaxableTotal>))]
        public override Decimal? CuryTaxableAmt
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
        public new abstract class taxableAmt : PX.Data.IBqlField
        {
        }
        #endregion
        #region CuryTaxAmt
        public new abstract class curyTaxAmt : PX.Data.IBqlField
        {
        }
        [PXDBCurrency(typeof(ARTaxTran.curyInfoID), typeof(ARTaxTran.taxAmt))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Tax Amount", Visibility = PXUIVisibility.Visible)]
        public override Decimal? CuryTaxAmt
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
        public new abstract class taxAmt : PX.Data.IBqlField
        {
        }
		#endregion
		#region CuryExpenseAmt
		public new abstract class curyExpenseAmt : PX.Data.IBqlField
		{
		}
		[PXDBCurrency(typeof(ARTaxTran.curyInfoID), typeof(ARTaxTran.expenseAmt))]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Expense Amount", Visibility = PXUIVisibility.Visible)]
		public override Decimal? CuryExpenseAmt
		{
			get; set;
		}
		#endregion
		#region ExpenseAmt
		public new abstract class expenseAmt : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryDiscountedTaxableAmt
		public abstract class curyDiscountedTaxableAmt : PX.Data.IBqlField
		{
		}
		protected decimal? _CuryDiscountedTaxableAmt;

		/// <summary>
		/// The taxable amount reduced on early payment, according to cash discount.
		/// Given in the <see cref="CuryID"> currency of the document</see>.
		/// </summary>
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXCurrency(typeof(ARTaxTran.curyInfoID), typeof(ARTaxTran.discountedTaxableAmt))]
		[PXUIField(DisplayName = "Discounted Taxable Amount", Visible = false, Enabled = false)]
		public virtual decimal? CuryDiscountedTaxableAmt
		{
			get
			{
				return _CuryDiscountedTaxableAmt;
			}
			set
			{
				_CuryDiscountedTaxableAmt = value;
			}
		}
		#endregion
		#region DiscountedTaxableAmt
		public abstract class discountedTaxableAmt : PX.Data.IBqlField
		{
		}
		protected decimal? _DiscountedTaxableAmt;

		/// <summary>
		/// The taxable amount reduced on early payment, according to cash discount.
		/// Given in the <see cref="Company.BaseCuryID"> base currency of the company</see>.
		/// </summary>
		[PXBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual decimal? DiscountedTaxableAmt
		{
			get
			{
				return _DiscountedTaxableAmt;
			}
			set
			{
				_DiscountedTaxableAmt = value;
			}
		}
		#endregion
		#region CuryDiscountedPrice
		public abstract class curyDiscountedPrice : PX.Data.IBqlField
		{
		}
		protected decimal? _CuryDiscountedPrice;

		/// <summary>
		/// The tax amount reduced on early payment, according to cash discount.
		/// Given in the <see cref="CuryID"> currency of the document</see>.
		/// </summary>
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXCurrency(typeof(ARTaxTran.curyInfoID), typeof(ARTaxTran.discountedPrice))]
		[PXUIField(DisplayName = "Tax on Discounted Price", Visible = false, Enabled = false)]
		public virtual decimal? CuryDiscountedPrice
		{
			get
			{
				return _CuryDiscountedPrice;
			}
			set
			{
				_CuryDiscountedPrice = value;
			}
		}
		#endregion
		#region DiscountedPrice
		public abstract class discountedPrice : PX.Data.IBqlField
		{
		}
		protected decimal? _DiscountedPrice;

		/// <summary>
		/// The tax amount reduced on early payment, according to cash discount.
		/// Given in the <see cref="Company.BaseCuryID"> base currency of the company</see>.
		/// </summary>
		[PXBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual decimal? DiscountedPrice
		{
			get
			{
				return _DiscountedPrice;
			}
			set
			{
				_DiscountedPrice = value;
			}
		}
		#endregion
    }

    [Serializable]
    [PXHidden]
    public partial class SalesTax : Tax
    {
        #region TaxID
        public new abstract class taxID : PX.Data.IBqlField
        {
        }
        #endregion
        #region TaxType
        public new abstract class taxType : PX.Data.IBqlField
        {
        }
        #endregion
        #region TaxVendorID
        public new abstract class taxVendorID : PX.Data.IBqlField
        {
        }
        #endregion
        #region SalesTaxAcctID
        public new abstract class salesTaxAcctID : PX.Data.IBqlField
        {
        }
        #endregion
        #region SalesTaxSubID
        public new abstract class salesTaxSubID : PX.Data.IBqlField
        {
        }
        #endregion
        #region PurchTaxAcctID
        public new abstract class purchTaxAcctID : PX.Data.IBqlField
        {
        }
        #endregion
        #region PurchTaxSubID
        public new abstract class purchTaxSubID : PX.Data.IBqlField
        {
        }
        #endregion
        #region PendingTax
        public new abstract class pendingTax : PX.Data.IBqlField
        {
        }
        #endregion
        #region PendingSalesTaxAcctID
        public new abstract class pendingSalesTaxAcctID : PX.Data.IBqlField
        {
        }
        #endregion
        #region PendingSalesTaxSubID
        public new abstract class pendingSalesTaxSubID : PX.Data.IBqlField
        {
        }
        #endregion
        #region PendingPurchTaxAcctID
        public new abstract class pendingPurchTaxAcctID : PX.Data.IBqlField
        {
        }
        #endregion
        #region PendingPurchTaxSubID
        public new abstract class pendingPurchTaxSubID : PX.Data.IBqlField
        {
        }
        #endregion
        #region HistTaxAcctID
        public abstract class histTaxAcctID : PX.Data.IBqlField
        {
        }
        protected Int32? _HistTaxAcctID;
        [PXDBCalcedAttribute(typeof(Switch<Case<Where<SalesTax.pendingTax, Equal<True>>, SalesTax.pendingSalesTaxAcctID>, SalesTax.salesTaxAcctID>), typeof(Int32))]
        public virtual Int32? HistTaxAcctID
        {
            get
            {
                return this._HistTaxAcctID;
            }
            set
            {
                this._HistTaxAcctID = value;
            }
        }
        #endregion
        #region HistTaxSubID
        public abstract class histTaxSubID : PX.Data.IBqlField
        {
        }
        protected Int32? _HistTaxSubID;
        [PXDBCalcedAttribute(typeof(Switch<Case<Where<SalesTax.pendingTax, Equal<True>>, SalesTax.pendingSalesTaxSubID>, SalesTax.salesTaxSubID>), typeof(Int32))]
        public virtual Int32? HistTaxSubID
        {
            get
            {
                return this._HistTaxSubID;
            }
            set
            {
                this._HistTaxSubID = value;
            }
        }
        #endregion
        #region TranTaxType
        public abstract class tranTaxType : PX.Data.IBqlField
        {
        }
        protected string _TranTaxType;
        [PXDBCalcedAttribute(typeof(Switch<Case<Where<SalesTax.pendingTax, Equal<True>>, TaxType.pendingSales>, TaxType.sales>), typeof(string))]
        public virtual String TranTaxType
        {
            get
            {
                return this._TranTaxType;
            }
            set
            {
                this._TranTaxType = value;
            }
        }
        #endregion

    }
}
