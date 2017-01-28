using PX.SM;
using System;
using PX.Data;
using PX.Objects.CS;

namespace PX.Objects.CA
{

    [Serializable]
	[PXCacheName(Messages.ClearingAccount)]
	public partial class CashAccountDeposit: IBqlTable
	{
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;
		[PXDBInt(IsKey = true)]
        [PXDBDefault(typeof(CashAccount.cashAccountID))]
		[PXUIField(DisplayName = "Cash Account ID", Visible = false)]
        [PXParent(typeof(Select<CashAccount, Where<CashAccount.cashAccountID, Equal<Current<CashAccountDeposit.accountID>>>>))]
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
		#region DepositAcctID
		public abstract class depositAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _DepositAcctID;
		[PXDefault()]
        [PXInt]
        [GL.CashAccount(null, typeof(Search<CashAccount.cashAccountID, Where<CashAccount.curyID, Equal<Current<CashAccount.curyID>>,
                                            And<CashAccount.cashAccountID, NotEqual<Current<CashAccount.cashAccountID>>,
                                            And<Where<CashAccount.clearingAccount, Equal<boolTrue>, Or<CashAccount.cashAccountID, Equal<Current<CashAccountDeposit.depositAcctID>>>>>>>>), IsKey = true, DisplayName = "Clearing Account")]	
		public virtual Int32? DepositAcctID
		{
			get
			{
				return this._DepositAcctID;
			}
			set
			{
				this._DepositAcctID = value;
			}
		}
		#endregion
		#region PaymentMethodID
		public abstract class paymentMethodID : PX.Data.IBqlField
		{
		}
		protected String _PaymentMethodID;
		[PXDBString(10, IsKey = true,IsUnicode = true)]
		[PXDefault("",PersistingCheck = PXPersistingCheck.Null)]
		[PXUIField(DisplayName = "Payment Method", Required = false)]
		[PXSelector(typeof(PaymentMethod.paymentMethodID))]
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
		#region ChargeEntryTypeID
        public abstract class chargeEntryTypeID : PX.Data.IBqlField
		{
		}
		protected String _ChargeEntryTypeID;
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Charges Type")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXSelector(typeof(Search2<CAEntryType.entryTypeId, InnerJoin<CashAccountETDetail,
								On<CashAccountETDetail.entryTypeID, Equal<CAEntryType.entryTypeId>,
								And<CashAccountETDetail.accountID, Equal<Current<CashAccount.cashAccountID>>>>>,
								Where<CAEntryType.module, Equal<GL.BatchModule.moduleCA>,
								And<CAEntryType.useToReclassifyPayments, Equal<False>>>>), ValidateValue=false)]
		public virtual String ChargeEntryTypeID
		{
			get
			{
				return this._ChargeEntryTypeID;
			}
			set
			{
				this._ChargeEntryTypeID = value;
			}
		}
		#endregion
		#region ChargeRate
        public abstract class chargeRate : PX.Data.IBqlField { };
		protected Decimal? _ChargeRate;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Charge Rate", Visibility = PXUIVisibility.Visible, Enabled = true)]
		public virtual Decimal? ChargeRate
		{
			get
			{
				return this._ChargeRate;
			}
			set
			{
				this._ChargeRate = value;
			}
		}
		#endregion
	}
}
