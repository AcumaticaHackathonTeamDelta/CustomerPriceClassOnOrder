namespace PX.Objects.CA
{
	using System;
	using PX.Data;

#if false
    [System.SerializableAttribute()]
    public partial class CashAccountDetail : PX.Data.IBqlTable
    {
        #region AccountID
        public abstract class accountID : PX.Data.IBqlField
        {
        }
        protected Int32? _AccountID;
        [PXDBInt(IsKey = true)]
        [PXDBLiteDefault(typeof(CashAccount.cashAccountID))]
        [PXUIField(DisplayName = "AccountID", Visible = false)]
        [PXParent(typeof(Select<CashAccount, Where<CashAccount.cashAccountID, Equal<Current<CashAccountDetail.accountID>>>>))]
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
        #region PaymentMethodID
        public abstract class paymentTypeID : PX.Data.IBqlField
        {
        }
        protected String _PaymentTypeID;
        [PXDBString(10, IsUnicode = true, IsKey = true)]
        [PXDefault()]
        [PXUIField(DisplayName = "Payment Type")]
        [PXSelector(typeof(PaymentType.paymentTypeID))]
        public virtual String PaymentTypeID
        {
            get
            {
                return this._PaymentTypeID;
            }
            set
            {
                this._PaymentTypeID = value;
            }
        }
        #endregion
        #region IsDefault
        public abstract class isDefault : PX.Data.IBqlField
        {
        }
        protected Boolean? _IsDefault;
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Default")]
        public virtual Boolean? IsDefault
        {
            get
            {
                return this._IsDefault;
            }
            set
            {
                this._IsDefault = value;
            }
        }
        #endregion
        #region AutoNextNbr
        public abstract class autoNextNbr : PX.Data.IBqlField
        {
        }
        protected Boolean? _AutoNextNbr;
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Auto-Number")]
        public virtual Boolean? AutoNextNbr
        {
            get
            {
                return this._AutoNextNbr;
            }
            set
            {
                this._AutoNextNbr = value;
            }
        }
        #endregion
        #region LastRefNbr
        public abstract class lastRefNbr : PX.Data.IBqlField
        {
        }
        protected String _LastRefNbr;
        [PXDBString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "Last Reference Number")]
        public virtual String LastRefNbr
        {
            get
            {
                return this._LastRefNbr;
            }
            set
            {
                this._LastRefNbr = value;
            }
        }
        #endregion
        #region APLastRefNbrIsNull
        protected Boolean? _LastRefNbrIsNull = false;
        public virtual Boolean? LastRefNbrIsNull
        {
            get
            {
                return this._LastRefNbrIsNull;
            }
            set
            {
                this._LastRefNbrIsNull = value;
            }
        }
        #endregion
        #region BatchLastRefNbr
        public abstract class batchLastRefNbr : PX.Data.IBqlField
        {
        }
        protected String _BatchLastRefNbr;
        [PXDBString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "Batch Last Reference Number")]
        public virtual String BatchLastRefNbr
        {
            get
            {
                return this._BatchLastRefNbr;
            }
            set
            {
                this._BatchLastRefNbr = value;
            }
        }
        #endregion
        #region MinFee
        public abstract class minFee : PX.Data.IBqlField
        {
        }
        protected Decimal? _MinFee;
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "Min. Fee")]
        public virtual Decimal? MinFee
        {
            get
            {
                return this._MinFee;
            }
            set
            {
                this._MinFee = value;
            }
        }
        #endregion
        #region MaxFee
        public abstract class maxFee : PX.Data.IBqlField
        {
        }
        protected Decimal? _MaxFee;
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "Max. Fee")]
        public virtual Decimal? MaxFee
        {
            get
            {
                return this._MaxFee;
            }
            set
            {
                this._MaxFee = value;
            }
        }
        #endregion
        #region PercentFee
        public abstract class percentFee : PX.Data.IBqlField
        {
        }
        protected Decimal? _PercentFee;
        [PXDBDecimal(6)]
        [PXUIField(DisplayName = "Fee Percent")]
        public virtual Decimal? PercentFee
        {
            get
            {
                return this._PercentFee;
            }
            set
            {
                this._PercentFee = value;
            }
        }
        #endregion
    } 
#endif
}
