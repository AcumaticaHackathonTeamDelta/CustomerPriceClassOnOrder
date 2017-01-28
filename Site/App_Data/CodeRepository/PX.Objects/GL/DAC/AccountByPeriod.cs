namespace PX.Objects.GL
{
	using System;
	using PX.Data;

	
    [Serializable]
    [PXHidden]
	public partial class AccountByPeriod : PX.Data.IBqlTable
	{

		#region FinPeriod
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected String _FinPeriodID;
		[OpenPeriod(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Financial Period", Visibility = PXUIVisibility.Visible)]
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
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;
		[PXUIField(DisplayName = "Account", Visibility = PXUIVisibility.Visible)]
		[PXSelector(typeof(Search<Account.accountID, Where<Match<Current<AccessInfo.userName>>>>), SubstituteKey = typeof(Account.accountCD))]
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
		#region PeriodDate
		public abstract class periodDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _PeriodDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Period Start Date", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public virtual DateTime? PeriodDate
		{
			get
			{
				return this._PeriodDate;
			}
			set
			{
				this._PeriodDate = value;
			}
		}
		#endregion
		#region PerDesc
		public abstract class perDesc : PX.Data.IBqlField
		{
		}
		protected String _PerDesc;
		[PXDBString(60, IsUnicode=true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.Visible)]
		public virtual String PerDesc
		{
			get
			{
				return this._PerDesc;
			}
			set
			{
				this._PerDesc = value;
			}
		}
		#endregion
		#region CreditAmount
		public abstract class creditTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CreditAmount;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Credit Amount", Visibility = PXUIVisibility.Visible)]
		public virtual Decimal? CreditAmount
		{
			get
			{
				return this._CreditAmount;
			}
			set
			{
				this._CreditAmount = value;
			}
		}
		#endregion
		#region DebitAmount
		public abstract class debitTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _DebitAmount;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Debit Amount", Visibility = PXUIVisibility.Visible)]
		public virtual Decimal? DebitAmount
		{
			get
			{
				return this._DebitAmount;
			}
			set
			{
				this._DebitAmount = value;
			}
		}
		#endregion
	}


}
