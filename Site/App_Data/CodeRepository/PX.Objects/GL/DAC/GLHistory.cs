namespace PX.Objects.GL
{
	using System;
	using PX.Data;
	using PX.Objects.CM;

    /// <summary>
    /// General Ledger history record.
    /// An instance of this class represents a history record for a particular <see cref="BaseGLHistory.LedgerID">ledger</see>,
    /// <see cref="BaseGLHistory.BranchID">branch</see>, <see cref="BaseGLHistory.AccountID">account</see>, <see cref="BaseGLHistory.SubID">subaccount</see>
    /// and <see cref="BaseGLHistory.FinPeriodID">financial period</see>.
    /// </summary>
	[System.SerializableAttribute()]
	[PXCacheName(Messages.GLHistory)]
	public partial class GLHistory : BaseGLHistory, PX.Data.IBqlTable
	{
		#region LedgerID
		public abstract class ledgerID : PX.Data.IBqlField
		{
		}
		#endregion
		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		#endregion
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		#endregion
		#region SubID
		public abstract class subID : PX.Data.IBqlField
		{
		}
		#endregion
		#region FinPeriod
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		#endregion
        #region FinYear
        public abstract class finYear : PX.Data.IBqlField
        { 
        }

        /// <summary>
        /// Financial year.
        /// </summary>
        /// <value>
        /// Determined from the <see cref="FinPeriodID"/> field.
        /// </value>
        [PXDBCalced(typeof(Substring<finPeriodID, CS.int1, CS.int4>), typeof(string))]
        public virtual string FinYear
        {
            get;
            set;
        }
        #endregion
        #region BalanceType
        public abstract class balanceType : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		#endregion
		#region DetDeleted
		public abstract class detDeleted : PX.Data.IBqlField
		{
		}
		protected Boolean? _DetDeleted;

        /// <summary>
        /// !REV!
        /// </summary>
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? DetDeleted
		{
			get
			{
				return this._DetDeleted;
			}
			set
			{
				this._DetDeleted = value;
			}
		}
		#endregion
		#region YearClosed
		public abstract class yearClosed : PX.Data.IBqlField { }

        /// <summary>
        /// Indicates whether the year, which the history record belongs, is closed.
        /// </summary>
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? YearClosed
		{
			get;
			set;
		}
		#endregion
		#region FinPtdCredit
		public abstract class finPtdCredit : PX.Data.IBqlField
		{
		}
		#endregion
		#region FinPtdDebit
		public abstract class finPtdDebit : PX.Data.IBqlField
		{
		}
		#endregion
		#region FinYtdBalance
		public abstract class finYtdBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region FinBegBalance
		public abstract class finBegBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region FinPtdRevalued
		public abstract class finPtdRevalued : PX.Data.IBqlField
		{
		}
		#endregion
		#region TranPtdCredit
		public abstract class tranPtdCredit : PX.Data.IBqlField
		{
		}
		#endregion
		#region TranPtdDebit
		public abstract class tranPtdDebit : PX.Data.IBqlField
		{
		}
		#endregion
		#region TranYtdBalance
		public abstract class tranYtdBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region TranBegBalance
		public abstract class tranBegBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryFinPtdCredit
		public abstract class curyFinPtdCredit : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryFinPtdDebit
		public abstract class curyFinPtdDebit : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryFinYtdBalance
		public abstract class curyFinYtdBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryFinBegBalance
		public abstract class curyFinBegBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryTranPtdCredit
		public abstract class curyTranPtdCredit : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryTranPtdDebit
		public abstract class curyTranPtdDebit : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryTranYtdBalance
		public abstract class curyTranYtdBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryTranBegBalance
		public abstract class curyTranBegBalance : PX.Data.IBqlField
		{
		}
		#endregion 
		#region AllocPtdBalance
		public abstract class allocPtdBalance : PX.Data.IBqlField
		{
		}
		protected Decimal? _AllocPtdBalance;

        /// <summary>
        /// !REV!
        /// Period-to-Date allocation balance - the amount allocated since the beginning of the period of the history record.
        /// </summary>
		[PXDBBaseCury(typeof(GLHistory.ledgerID))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? AllocPtdBalance
		{
			get
			{
				return this._AllocPtdBalance;
			}
			set
			{
				this._AllocPtdBalance = value;
			}
		}
	#endregion
		#region AllocBegBalance
		public abstract class allocBegBalance : PX.Data.IBqlField
		{
		}
		protected Decimal? _AllocBegBalance;

        /// <summary>
        /// !REV!
        /// Beginning allocation balance - the amount allocated for periods preceding the period of the history record.
        /// </summary>
		[PXDBBaseCury(typeof(GLHistory.ledgerID))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? AllocBegBalance
		{
			get
			{
				return this._AllocBegBalance;
			}
			set
			{
				this._AllocBegBalance = value;
			}
		}
		#endregion
		#region tstamp
		public abstract class Tstamp : PX.Data.IBqlField
		{
		}
		#endregion
	}	

	[System.SerializableAttribute()]
	[PXPrimaryGraph(typeof(AccountByPeriodEnq), Filter = typeof(AccountByPeriodFilter))]
	public class BaseGLHistory 
	{
		#region LedgerID
		protected Int32? _LedgerID;

        /// <summary>
        /// Identifier of the <see cref="Ledger"/>, which the history record belongs.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="Ledger.LedgerID"/> field.
        /// </value>
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Ledger", Visibility = PXUIVisibility.Invisible)]
		[PXSelector(typeof(Ledger.ledgerID),
			typeof(Ledger.ledgerCD), typeof(Ledger.baseCuryID), typeof(Ledger.descr), typeof(Ledger.balanceType),
			DescriptionField = typeof(Ledger.descr), SubstituteKey = typeof(Ledger.ledgerCD))]
		public virtual Int32? LedgerID
		{
			get
			{
				return this._LedgerID;
			}
			set
			{
				this._LedgerID = value;
			}
		}
		#endregion
		#region BranchID
		protected Int32? _BranchID;

        /// <summary>
        /// Identifier of the <see cref="Branch"/>, which the history record belongs.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="Branch.BAccountID"/> field.
        /// </value>
		[Branch(IsKey = true)]
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
		#region AccountID
		protected Int32? _AccountID;

		/*[PXDBInt(IsKey = true)]
		[PXUIField(DisplayName = "Account", Visibility = PXUIVisibility.Invisible)]
		[PXSelector(typeof(Account.accountID), DescriptionField = typeof(Account.description), SubstituteKey = typeof(Account.accountCD))]*/
        /// <summary>
        /// Identifier of the <see cref="Account"/> associated with the history record.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="Account.AccountID"/> field.
        /// </value>
		[Account(Visibility = PXUIVisibility.Invisible,IsKey = true, DescriptionField = typeof(Account.description))]
		[PXDefault()]
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
		#region SubID
		protected Int32? _SubID;
		/*[PXDBInt(IsKey = true)]
		[PXUIField(DisplayName = "Subaccount", Visibility = PXUIVisibility.Invisible)]
		[PXSelector(typeof(Sub.subID), DescriptionField = typeof(Sub.description), SubstituteKey = typeof(Sub.subCD))]*/
        /// <summary>
        /// Identifier of the <see cref="Sub">Subaccount</see> associated with the history record.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="Sub.SubID"/> field.
        /// </value>
		[SubAccount(IsKey = true, Visibility = PXUIVisibility.Invisible, DescriptionField = typeof(Sub.description))]
		[PXDefault()]
		public virtual Int32? SubID
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
		#region FinPeriod
		protected String _FinPeriodID;

        /// <summary>
        /// Identifier of the <see cref="FinPeriod">Financial Period</see> of the history record.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="FinPeriod.FinPeriodID"/> field.
        /// </value>
		[GL.FinPeriodID(IsKey=true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Financial Period", Visibility = PXUIVisibility.Invisible)]
		[PXSelector(typeof(FinPeriod.finPeriodID), DescriptionField = typeof(FinPeriod.descr),SelectorMode=PXSelectorMode.NoAutocomplete)]
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
		#region BalanceType
		protected String _BalanceType;

        /// <summary>
        /// The type of the balance.
        /// </summary>
        /// <value>
        /// Allowed values are:
        /// <c>"A"</c> - Actual,
        /// <c>"R"</c> - Reporting,
        /// <c>"S"</c> - Statistical,
        /// <c>"B"</c> - Budget.
        /// See <see cref="Ledger.BalanceType"/>.
        /// </value>
		[PXDBString(1, IsFixed = true)]
		[PXDefault(LedgerBalanceType.Actual)]
		[LedgerBalanceType.List()]
		[PXUIField(DisplayName = "Balance Type")]
		public virtual String BalanceType
		{
			get
			{
				return this._BalanceType;
			}
			set
			{
				this._BalanceType = value;
			}
		}
		#endregion
		#region CuryID
		protected String _CuryID;

        /// <summary>
        /// Identifier of the <see cref="Currency"/> of the history record.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="Currency.CuryID"/> field.
        /// </value>
		[PXDBString(5, IsUnicode = true)]
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
		#region FinPtdCredit
		protected Decimal? _FinPtdCredit;

        /// <summary>
        /// Period-to-Date credit of the account in the <see cref="Company.baseCuryID">base currency</see> of the company.
        /// </summary>
        /// <value>
        /// The value of this field is based on the <see cref="GLTran.FinPeriodID">GLTran.FinPeriodID</see> field, which can be overriden by user.
        /// See also <see cref="BaseGLHistory.TranPtdCredit"/>.
        /// </value>
		[PXDBBaseCury(typeof(GLHistory.ledgerID))]
		[PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName="Fin. PTD Credit")]
		public virtual Decimal? FinPtdCredit
		{
			get
			{
				return this._FinPtdCredit;
			}
			set
			{
				this._FinPtdCredit = value;
			}
		}
		#endregion
		#region FinPtdDebit
		protected Decimal? _FinPtdDebit;

        /// <summary>
        /// Period-to-Date debit of the account in the <see cref="Company.baseCuryID">base currency</see> of the company.
        /// </summary>
        /// <value>
        /// The value of this field is based on the <see cref="GLTran.FinPeriodID">GLTran.FinPeriodID</see> field, which can be overriden by user.
        /// See also <see cref="BaseGLHistory.TranPtdDebit"/>.
        /// </value>
		[PXDBBaseCury(typeof(GLHistory.ledgerID))]
		[PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Fin. PTD Debit")]
		public virtual Decimal? FinPtdDebit
		{
			get
			{
				return this._FinPtdDebit;
			}
			set
			{
				this._FinPtdDebit = value;
			}
		}
		#endregion
		#region FinYtdBalance
		protected Decimal? _FinYtdBalance;

        /// <summary>
        /// Year-to-Date balance of the account in the <see cref="Company.baseCuryID">base currency</see> of the company.
        /// </summary>
        /// <value>
        /// The value of this field is based on the <see cref="GLTran.FinPeriodID">GLTran.FinPeriodID</see> field, which can be overriden by user.
        /// See also <see cref="BaseGLHistory.TranYtdBalance"/>.
        /// </value>
		[PXDBBaseCury(typeof(GLHistory.ledgerID))]
		[PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Fin. YTD Balance")]
		public virtual Decimal? FinYtdBalance
		{
			get
			{
				return this._FinYtdBalance;
			}
			set
			{
				this._FinYtdBalance = value;
			}
		}
		#endregion
		#region FinBegBalance
        protected Decimal? _FinBegBalance;

        /// <summary>
        /// Beginning balance of the account in the <see cref="Company.baseCuryID">base currency</see> of the company.
        /// </summary>
        /// <value>
        /// The value of this field is based on the <see cref="GLTran.FinPeriodID">GLTran.FinPeriodID</see> field, which can be overriden by user.
        /// See also <see cref="BaseGLHistory.TranBegBalance"/>.
        /// </value>
		[PXDBBaseCury(typeof(GLHistory.ledgerID))]
		[PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Fin. Begining Balance")]
		public virtual Decimal? FinBegBalance
		{
			get
			{
				return this._FinBegBalance;
			}
			set
			{
				this._FinBegBalance = value;
			}
		}
		#endregion
		#region FinPtdRevalued
        protected Decimal? _FinPtdRevalued;

        /// <summary>
        /// Period-to-Date revalued balance of the account in the <see cref="Company.baseCuryID">base currency</see> of the company.
        /// </summary>
        /// <value>
        /// The value of this field is based on the <see cref="GLTran.FinPeriodID">GLTran.FinPeriodID</see> field, which can be overriden by user.
        /// See also <see cref="BaseGLHistory.TranPtdRevalued"/>.
        /// </value>
		[PXDBBaseCury(typeof(GLHistory.ledgerID))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? FinPtdRevalued
		{
			get
			{
				return this._FinPtdRevalued;
			}
			set
			{
				this._FinPtdRevalued = value;
			}
		}
		#endregion
		#region TranPtdCredit
		protected Decimal? _TranPtdCredit;

        /// <summary>
        /// Period-to-Date credit of the account in the <see cref="Company.baseCuryID">base currency</see> of the company.
        /// </summary>
        /// <value>
        /// The value of this field is based on the <see cref="GLTran.TranPeriodID">GLTran.TranPeriodID</see> field,
        /// which can not be overriden by user and is determined by the date of the transactions.
        /// See also <see cref="BaseGLHistory.FinPtdCredit"/>.
        /// </value>
        [PXDBBaseCury(typeof(GLHistory.ledgerID))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TranPtdCredit
		{
			get
			{
				return this._TranPtdCredit;
			}
			set
			{
				this._TranPtdCredit = value;
			}
		}
		#endregion
		#region TranPtdDebit
        protected Decimal? _TranPtdDebit;

        /// <summary>
        /// Period-to-Date debit of the account in the <see cref="Company.baseCuryID">base currency</see> of the company.
        /// </summary>
        /// <value>
        /// The value of this field is based on the <see cref="GLTran.TranPeriodID">GLTran.TranPeriodID</see> field,
        /// which can not be overriden by user and is determined by the date of the transactions.
        /// See also <see cref="BaseGLHistory.FinPtdDebit"/>.
        /// </value>
		[PXDBBaseCury(typeof(GLHistory.ledgerID))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TranPtdDebit
		{
			get
			{
				return this._TranPtdDebit;
			}
			set
			{
				this._TranPtdDebit = value;
			}
		}
		#endregion
		#region TranYtdBalance
        protected Decimal? _TranYtdBalance;

        /// <summary>
        /// Year-to-Date balance of the account in the <see cref="Company.baseCuryID">base currency</see> of the company.
        /// </summary>
        /// <value>
        /// The value of this field is based on the <see cref="GLTran.TranPeriodID">GLTran.TranPeriodID</see> field,
        /// which can not be overriden by user and is determined by the date of the transactions.
        /// See also <see cref="BaseGLHistory.FinYtdBalance"/>.
        /// </value>
		[PXDBBaseCury(typeof(GLHistory.ledgerID))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TranYtdBalance
		{
			get
			{
				return this._TranYtdBalance;
			}
			set
			{
				this._TranYtdBalance = value;
			}
		}
		#endregion
		#region TranBegBalance
        protected Decimal? _TranBegBalance;

        /// <summary>
        /// Beginning balance of the account in the <see cref="Company.baseCuryID">base currency</see> of the company.
        /// </summary>
        /// <value>
        /// The value of this field is based on the <see cref="GLTran.TranPeriodID">GLTran.TranPeriodID</see> field,
        /// which can not be overriden by user and is determined by the date of the transactions.
        /// See also <see cref="BaseGLHistory.FinBegBalance"/>.
        /// </value>
		[PXDBBaseCury(typeof(GLHistory.ledgerID))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TranBegBalance
		{
			get
			{
				return this._TranBegBalance;
			}
			set
			{
				this._TranBegBalance = value;
			}
		}
		#endregion
		#region CuryFinPtdCredit
        protected Decimal? _CuryFinPtdCredit;

        /// <summary>
        /// Period-to-Date credit of the account in the <see cref="BaseGLHistory.curyID">currency</see> of the account.
        /// </summary>
        /// <value>
        /// The value of this field is based on the <see cref="GLTran.FinPeriodID">GLTran.FinPeriodID</see> field, which can be overriden by user.
        /// See also <see cref="BaseGLHistory.CuryTranPtdCredit"/>.
        /// </value>
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal,"0.0")]
		public virtual Decimal? CuryFinPtdCredit
		{
			get
			{
				return this._CuryFinPtdCredit;
			}
			set
			{
				this._CuryFinPtdCredit = value;
			}
		}
		#endregion
		#region CuryFinPtdDebit
        protected Decimal? _CuryFinPtdDebit;

        /// <summary>
        /// Period-to-Date debit of the account in the <see cref="BaseGLHistory.curyID">currency</see> of the account.
        /// </summary>
        /// <value>
        /// The value of this field is based on the <see cref="GLTran.FinPeriodID">GLTran.FinPeriodID</see> field, which can be overriden by user.
        /// See also <see cref="BaseGLHistory.CuryTranPtdDebit"/>.
        /// </value>
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal,"0.0")]
		public virtual Decimal? CuryFinPtdDebit
		{
			get
			{
				return this._CuryFinPtdDebit;
			}
			set
			{
				this._CuryFinPtdDebit = value;
			}
		}
		#endregion
		#region CuryFinYtdBalance
        protected Decimal? _CuryFinYtdBalance;

        /// <summary>
        /// Year-to-Date balance of the account in the <see cref="BaseGLHistory.curyID">currency</see> of the account.
        /// </summary>
        /// <value>
        /// The value of this field is based on the <see cref="GLTran.FinPeriodID">GLTran.FinPeriodID</see> field, which can be overriden by user.
        /// See also <see cref="BaseGLHistory.CuryTranYtdBalance"/>.
        /// </value>
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "CuryFinYtdBalance")]
		public virtual Decimal? CuryFinYtdBalance
		{
			get
			{
				return this._CuryFinYtdBalance;
			}
			set
			{
				this._CuryFinYtdBalance = value;
			}
		}
		#endregion
		#region CuryFinBegBalance
        protected Decimal? _CuryFinBegBalance;

        /// <summary>
        /// Beginning balance of the account in the <see cref="BaseGLHistory.curyID">currency</see> of the account.
        /// </summary>
        /// <value>
        /// The value of this field is based on the <see cref="GLTran.FinPeriodID">GLTran.FinPeriodID</see> field, which can be overriden by user.
        /// See also <see cref="BaseGLHistory.CuryTranBegBalance"/>.
        /// </value>
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal,"0.0")]
		public virtual Decimal? CuryFinBegBalance
		{
			get
			{
				return this._CuryFinBegBalance;
			}
			set
			{
				this._CuryFinBegBalance = value;
			}
		}
		#endregion
		#region CuryTranPtdCredit
        protected Decimal? _CuryTranPtdCredit;

        /// <summary>
        /// Period-to-Date credit of the account in the <see cref="BaseGLHistory.curyID">currency</see> of the account.
        /// </summary>
        /// <value>
        /// The value of this field is based on the <see cref="GLTran.TranPeriodID">GLTran.TranPeriodID</see> field,
        /// which can not be overriden by user and is determined by the date of the transactions.
        /// See also <see cref="BaseGLHistory.CuryFinPtdCredit"/>.
        /// </value>
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal,"0.0")]
		public virtual Decimal? CuryTranPtdCredit
		{
			get
			{
				return this._CuryTranPtdCredit;
			}
			set
			{
				this._CuryTranPtdCredit = value;
			}
		}
		#endregion
		#region CuryTranPtdDebit
        protected Decimal? _CuryTranPtdDebit;

        /// <summary>
        /// Period-to-Date debit of the account in the <see cref="BaseGLHistory.curyID">currency</see> of the account.
        /// </summary>
        /// <value>
        /// The value of this field is based on the <see cref="GLTran.TranPeriodID">GLTran.TranPeriodID</see> field,
        /// which can not be overriden by user and is determined by the date of the transactions.
        /// See also <see cref="BaseGLHistory.CuryFinPtdDebit"/>.
        /// </value>
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal,"0.0")]
		public virtual Decimal? CuryTranPtdDebit
		{
			get
			{
				return this._CuryTranPtdDebit;
			}
			set
			{
				this._CuryTranPtdDebit = value;
			}
		}
		#endregion
		#region CuryTranYtdBalance
        protected Decimal? _CuryTranYtdBalance;

        /// <summary>
        /// Year-to-Date balance of the account in the <see cref="BaseGLHistory.curyID">currency</see> of the account.
        /// </summary>
        /// <value>
        /// The value of this field is based on the <see cref="GLTran.TranPeriodID">GLTran.TranPeriodID</see> field,
        /// which can not be overriden by user and is determined by the date of the transactions.
        /// See also <see cref="BaseGLHistory.CuryFinYtdBalance"/>.
        /// </value>
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal,"0.0")]
		public virtual Decimal? CuryTranYtdBalance
		{
			get
			{
				return this._CuryTranYtdBalance;
			}
			set
			{
				this._CuryTranYtdBalance = value;
			}
		}
		#endregion
		#region CuryTranBegBalance
        protected Decimal? _CuryTranBegBalance;

        /// <summary>
        /// Beginning balance of the account in the <see cref="BaseGLHistory.curyID">currency</see> of the account.
        /// </summary>
        /// <value>
        /// The value of this field is based on the <see cref="GLTran.TranPeriodID">GLTran.TranPeriodID</see> field,
        /// which can not be overriden by user and is determined by the date of the transactions.
        /// See also <see cref="BaseGLHistory.CuryFinBegBalance"/>.
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal,"0.0")]
		public virtual Decimal? CuryTranBegBalance
		{
			get
			{
				return this._CuryTranBegBalance;
			}
			set
			{
				this._CuryTranBegBalance = value;
			}
		}
		#endregion
		#region FinFlag
		protected bool? _FinFlag = true;

        /// <summary>
        /// The flag determining the balance fields, to which the <see cref="PtdCredit"/>, <see cref="PtdDebit"/>,
        /// <see cref="YtdBalance"/>, <see cref="BegBalance"/>, <see cref="PtdRevalued"/> and their Cury* counterparts are mapped.
        /// </summary>
        /// <value>
        /// When set to <c>true</c>, the above fields are mapped to their Fin* analogs (e.g. <see cref="PtdDebit"/> will represent - get and set - <see cref="FinPtdDebit"/>),
        /// otherwise they are mapped to their Tran* analogs (e.g. <see cref="PtdDebit"/> corresponds to <see cref="TranPtdDebit"/>
        /// </value>
		[PXBool()]
		public virtual bool? FinFlag
		{
			get
			{
				return this._FinFlag;
			}
			set
			{
				this._FinFlag = value;
			}
		}
		#endregion
		#region REFlag
		public abstract class rEFlag : PX.Data.IBqlField { }

        /// <summary>
        /// !REV!
        /// </summary>
        [Obsolete("The field is not used and will be eleminated in the later version.")]
		[PXBool()]
		public virtual bool? REFlag
		{
			get;
			set;
		}
		#endregion
		#region PtdCredit

        /// <summary>
        /// Period-to-Date credit of the account in the <see cref="Company.baseCuryID">base currency</see> of the company.
        /// </summary>
        /// <value>
        /// Corresponds either to <see cref="FinPtdCredit"/> or to <see cref="TranPtdCredit"/> field, depending on the <see cref="FinFlag"/>.
		[PXDecimal(4)]
		public virtual Decimal? PtdCredit
		{
			get
			{
				return ((bool)_FinFlag) ? this._FinPtdCredit : this._TranPtdCredit;
			}
			set
			{
				if ((bool)_FinFlag)
				{
					this._FinPtdCredit = value;
				}
				else
				{
					this._TranPtdCredit = value;
				}
			}
		}
		#endregion
        #region PtdDebit

        /// <summary>
        /// Period-to-Date debit of the account in the <see cref="Company.baseCuryID">base currency</see> of the company.
        /// </summary>
        /// <value>
        /// Corresponds either to <see cref="FinPtdDebit"/> or to <see cref="TranPtdDebit"/> field, depending on the <see cref="FinFlag"/>.
		[PXDecimal(4)]
		public virtual Decimal? PtdDebit
		{
			get
			{
				return ((bool)_FinFlag) ? this._FinPtdDebit : this._TranPtdDebit;
			}
			set
			{
				if ((bool)_FinFlag)
				{
					this._FinPtdDebit = value;
				}
				else
				{
					this._TranPtdDebit = value;
				}
			}
		}
		#endregion
        #region YtdBalance

        /// <summary>
        /// Year-to-Date balance of the account in the <see cref="Company.baseCuryID">base currency</see> of the company.
        /// </summary>
        /// <value>
        /// Corresponds either to <see cref="FinYtdBalance"/> or to <see cref="TranYtdBalance"/> field, depending on the <see cref="FinFlag"/>.
		[PXDecimal(4)]
		public virtual Decimal? YtdBalance
		{
			get
			{
				return ((bool)_FinFlag) ? this._FinYtdBalance : this._TranYtdBalance;
			}
			set
			{
				if ((bool)_FinFlag)
				{
					this._FinYtdBalance = value;
				}
				else
				{
					this._TranYtdBalance = value;
				}
			}
		}
		#endregion
        #region BegBalance

        /// <summary>
        /// Beginning balance of the account in the <see cref="Company.baseCuryID">base currency</see> of the company.
        /// </summary>
        /// <value>
        /// Corresponds either to <see cref="FinBegBalance"/> or to <see cref="TranBegBalance"/> field, depending on the <see cref="FinFlag"/>.
		[PXDecimal(4)]
		public virtual Decimal? BegBalance
		{
			get
			{
				return ((bool)_FinFlag) ? this._FinBegBalance : this._TranBegBalance;
			}
			set
			{
				if ((bool)_FinFlag)
				{
					this._FinBegBalance = value;
				}
				else
				{
					this._TranBegBalance = value;
				}
			}
		}
		#endregion
        #region PtdRevalued

        /// <summary>
        /// Period-to-Date revalued balance of the account in the <see cref="Company.baseCuryID">base currency</see> of the company.
        /// </summary>
        /// <value>
        /// Corresponds either to <see cref="FinPtdRevalued"/> or to <see cref="TranPtdRevalued"/> field, depending on the <see cref="FinFlag"/>.
		[PXDecimal(4)]
		public virtual Decimal? PtdRevalued
		{
			get
			{
				return ((bool)_FinFlag) ? this._FinPtdRevalued : null;
			}
			set
			{
				if ((bool)_FinFlag)
				{
					this._FinPtdRevalued = value;
				}
			}
		}
		#endregion
        #region CuryPtdCredit

        /// <summary>
        /// Period-to-Date credit of the account in the <see cref="BaseGLHistory.curyID">currency</see> of the account.
        /// </summary>
        /// <value>
        /// Corresponds either to <see cref="CuryFinPtdCredit"/> or to <see cref="CuryTranPtdCredit"/> field, depending on the <see cref="FinFlag"/>.
		[PXDecimal(4)]
		public virtual Decimal? CuryPtdCredit
		{
			get
			{
				return ((bool)_FinFlag) ? this._CuryFinPtdCredit : this._CuryTranPtdCredit;
			}
			set
			{
				if ((bool)_FinFlag)
				{
					this._CuryFinPtdCredit = value;
				}
				else
				{
					this._CuryTranPtdCredit = value;
				}
			}
		}
		#endregion
        #region CuryPtdDebit

        /// <summary>
        /// Period-to-Date debit of the account in the <see cref="BaseGLHistory.curyID">currency</see> of the account.
        /// </summary>
        /// <value>
        /// Corresponds either to <see cref="CuryFinPtdDebit"/> or to <see cref="CuryTranPtdDebit"/> field, depending on the <see cref="FinFlag"/>.
		[PXDecimal(4)]
		public virtual Decimal? CuryPtdDebit
		{
			get
			{
				return ((bool)_FinFlag) ? this._CuryFinPtdDebit : this._CuryTranPtdDebit;
			}
			set
			{
				if ((bool)_FinFlag)
				{
					this._CuryFinPtdDebit = value;
				}
				else
				{
					this._CuryTranPtdDebit = value;
				}
			}
		}
		#endregion
        #region CuryYtdBalance

        /// <summary>
        /// Year-to-Date balance of the account in the <see cref="BaseGLHistory.curyID">currency</see> of the account.
        /// </summary>
        /// <value>
        /// Corresponds either to <see cref="CuryFinYtdBalance"/> or to <see cref="CuryTranYtdBalance"/> field, depending on the <see cref="FinFlag"/>.
		[PXDecimal(4)]
		public virtual Decimal? CuryYtdBalance
		{
			get
			{
				return ((bool)_FinFlag) ? this._CuryFinYtdBalance : this._CuryTranYtdBalance;
			}
			set
			{
				if ((bool)_FinFlag)
				{
					this._CuryFinYtdBalance = value;
				}
				else
				{
					this._CuryTranYtdBalance = value;
				}
			}
		}
		#endregion
        #region CuryBegBalance

        /// <summary>
        /// Beginning balance of the account in the <see cref="BaseGLHistory.curyID">currency</see> of the account.
        /// </summary>
        /// <value>
        /// Corresponds either to <see cref="CuryFinBegBalance"/> or to <see cref="CuryTranBegBalance"/> field, depending on the <see cref="FinFlag"/>.
		[PXDecimal(4)]
		public virtual Decimal? CuryBegBalance
		{
			get
			{
				return ((bool)_FinFlag) ? this._CuryFinBegBalance : this._CuryTranBegBalance;
			}
			set
			{
				if ((bool)_FinFlag)
				{
					this._CuryFinBegBalance = value;
				}
				else
				{
					this._CuryTranBegBalance = value;
				}
			}
		}
		#endregion
		#region tstamp
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
	}
}
