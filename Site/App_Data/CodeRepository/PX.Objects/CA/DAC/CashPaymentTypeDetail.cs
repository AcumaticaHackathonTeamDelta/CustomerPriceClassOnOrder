namespace PX.Objects.CA
{
	using System;
	using PX.Data;
	using PX.Objects.CS;
	
	[System.SerializableAttribute()]
	[PXCacheName(Messages.CashAccountPaymentMethodDetail)]
	public partial class CashAccountPaymentMethodDetail : PX.Data.IBqlTable
	{
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;

		[PXDBInt(IsKey = true)]
		[PXDBLiteDefault(typeof(CashAccount.cashAccountID))]
		[PXUIField(DisplayName = "Cash Account", Visible = false, Enabled = false)]
		[PXParent(typeof(Select<PaymentMethodAccount, 
            Where<PaymentMethodAccount.cashAccountID, Equal<Current<CashAccountPaymentMethodDetail.accountID>>,
			And<PaymentMethodAccount.paymentMethodID, Equal<Current<CashAccountPaymentMethodDetail.paymentMethodID>>,
            And<PaymentMethodAccount.useForAP,Equal<True>>>>>))]
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
		public abstract class paymentMethodID : PX.Data.IBqlField
		{
		}
		protected String _PaymentMethodID;
		[PXDBString(10, IsUnicode = true, IsKey = true)]
        [PXDefault]
		[PXUIField(DisplayName = "Payment Method", Visible=false)]
		[PXSelector(typeof(Search<PaymentMethod.paymentMethodID>))]
		public virtual String PaymentMethodID
		{
			get
			{
				return this._PaymentMethodID;
			}
			set
			{
				this._PaymentMethodID = value;
			}
		}
				#endregion
        #region DetailID
        public abstract class detailID : PX.Data.IBqlField
        {
        }
        protected String _DetailID;
        [PXDBString(10, IsUnicode = true, IsKey = true)]
        [PXDefault()]        
        [PXUIField(DisplayName = "ID", Visible = false, Enabled = false)]
        [PXSelector(typeof(Search<PaymentMethodDetail.detailID, Where<PaymentMethodDetail.paymentMethodID,
                    Equal<Current<CashAccountPaymentMethodDetail.paymentMethodID>>,
                        And<Where<PaymentMethodDetail.useFor, Equal<PaymentMethodDetailUsage.useForCashAccount>,
                        Or<PaymentMethodDetail.useFor, Equal<PaymentMethodDetailUsage.useForAll>>>>>>))]		
        public virtual String DetailID
        {
            get
            {
                return this._DetailID;
            }
            set
            {
                this._DetailID = value;
            }
        }
        #endregion
		#region DetailValue
		public abstract class detailValue : PX.Data.IBqlField
		{
		}
		protected String _DetailValue;

		[PXDBStringWithMask(255, typeof(Search<PaymentMethodDetail.entryMask, Where<PaymentMethodDetail.paymentMethodID, Equal<Current<CashAccountPaymentMethodDetail.paymentMethodID>>,
									   And<PaymentMethodDetail.detailID, Equal<Current<CashAccountPaymentMethodDetail.detailID>>,
                                       And<Where<PaymentMethodDetail.useFor, Equal<PaymentMethodDetailUsage.useForCashAccount>,
                                           Or<PaymentMethodDetail.useFor, Equal<PaymentMethodDetailUsage.useForAll>>>>>>>), IsUnicode = true)]
		[PXUIField(DisplayName = "Value")]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[DynamicValueValidation(typeof(Search<PaymentMethodDetail.validRegexp, Where<PaymentMethodDetail.paymentMethodID, Equal<Current<CashAccountPaymentMethodDetail.paymentMethodID>>,
										And<PaymentMethodDetail.detailID,Equal<Current<CashAccountPaymentMethodDetail.detailID>>,
                                        And<Where<PaymentMethodDetail.useFor, Equal<PaymentMethodDetailUsage.useForCashAccount>,
                                            Or<PaymentMethodDetail.useFor, Equal<PaymentMethodDetailUsage.useForAll>>>>>>>))]
		public virtual String DetailValue
		{
			get
			{
				return this._DetailValue;
			}
			set
			{
				this._DetailValue = value;
			}
		}
		#endregion
	}
}
