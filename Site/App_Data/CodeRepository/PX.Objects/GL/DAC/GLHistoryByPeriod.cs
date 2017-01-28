using PX.Objects.AP;
using PX.Objects.CS;

namespace PX.Objects.GL
{
	using System;
	using PX.Data;
	
    /// <summary>
    /// The DAC used to simplify selection and aggregation of proper <see cref="GLHistory"/> records
    /// on various inquiry and processing screens of the General Ledger module.
    /// </summary>
	[System.SerializableAttribute()]
	[PXProjection(typeof(Select5<GLHistory,
		InnerJoin<FinPeriod, On<FinPeriod.finPeriodID, GreaterEqual<GLHistory.finPeriodID>>>,
	   Aggregate<
	   GroupBy<GLHistory.branchID,
	   GroupBy<GLHistory.ledgerID,
	   GroupBy<GLHistory.accountID,
	   GroupBy<GLHistory.subID,
	   Max<GLHistory.finPeriodID,
		GroupBy<FinPeriod.finPeriodID
        >>>>>>>>))]
	[PXPrimaryGraph(typeof(AccountByPeriodEnq), Filter = typeof(AccountByPeriodFilter))]
    [PXCacheName(Messages.GLHistoryByPeriod)]
	public partial class GLHistoryByPeriod : PX.Data.IBqlTable
	{
		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
        protected Int32? _BranchID;

        /// <summary>
        /// Identifier of the <see cref="Branch"/>, which the history record belongs.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="Branch.BAccountID"/> field.
        /// </value>
		[PXDBInt(IsKey = true, BqlField = typeof(GLHistory.branchID))]
        [PXUIField(DisplayName="Branch")]
		[PXSelector(typeof(Branch.branchID), SubstituteKey = typeof(Branch.branchCD))]
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
		#region LedgerID
		public abstract class ledgerID : PX.Data.IBqlField
		{
		}
        protected Int32? _LedgerID;

        /// <summary>
        /// Identifier of the <see cref="Ledger"/>, which the history record belongs.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="Ledger.LedgerID"/> field.
        /// </value>
		[PXDBInt(IsKey = true, BqlField = typeof(GLHistory.ledgerID))]
        [PXUIField(DisplayName = "Ledger")]
		[PXSelector(typeof(Ledger.ledgerID), SubstituteKey = typeof(Ledger.ledgerCD))]
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
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;

        /// <summary>
        /// Identifier of the <see cref="Account"/> associated with the history record.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="Account.AccountID"/> field.
        /// </value>
		[Account(IsKey = true, BqlField = typeof(GLHistory.accountID))]
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
		public abstract class subID : PX.Data.IBqlField
		{
		}
        protected Int32? _SubID;

        /// <summary>
        /// Identifier of the <see cref="Sub">Subaccount</see> associated with the history record.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="Sub.SubID"/> field.
        /// </value>
		[SubAccount(IsKey = true, BqlField = typeof(GLHistory.subID))]
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
		#region LastActivityPeriod
		public abstract class lastActivityPeriod : PX.Data.IBqlField
		{
		}
		protected String _LastActivityPeriod;

        /// <summary>
        /// Identifier of the <see cref="FinPeriod">Financial Period</see> of the last activity on the Account and Subaccount associated with the history record,
        /// with regards to Ledger and Branch.
        /// </summary>
		[GL.FinPeriodID(BqlField = typeof(GLHistory.finPeriodID))]
        [PXUIField(DisplayName = "Last Activity Period", Visibility = PXUIVisibility.Invisible)]
		public virtual String LastActivityPeriod
		{
			get
			{
				return this._LastActivityPeriod;
			}
			set
			{
				this._LastActivityPeriod = value;
			}
		}
		#endregion
		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected String _FinPeriodID;

        /// <summary>
        /// Identifier of the <see cref="FinPeriod">Financial Period</see>, for which the history data is given.
        /// </summary>
        [PXUIField(DisplayName = "Financial Period", Visibility = PXUIVisibility.Invisible)]
		[GL.FinPeriodID(IsKey = true, BqlField = typeof(FinPeriod.finPeriodID))]
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
	}

	[Serializable]
	[PXProjection(typeof(Select4<GLHistory,
		Where<GLHistory.finPtdRevalued, NotEqual<decimal0>>,
		Aggregate<
			GroupBy<GLHistory.branchID,
			GroupBy<GLHistory.ledgerID,
			GroupBy<GLHistory.accountID,
			GroupBy<GLHistory.subID,
			Max<GLHistory.finPeriodID>>>>>>>))]
	public partial class GLHistoryLastRevaluation : PX.Data.IBqlTable
	{
		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		protected Int32? _BranchID;

		/// <summary>
		/// Identifier of the <see cref="Branch"/>, which the history record belongs.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="Branch.BAccountID"/> field.
		/// </value>
		[PXDBInt(IsKey = true, BqlField = typeof(GLHistory.branchID))]
		[PXUIField(DisplayName = "Branch")]
		[PXSelector(typeof(Branch.branchID), SubstituteKey = typeof(Branch.branchCD))]
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
		#region LedgerID
		public abstract class ledgerID : PX.Data.IBqlField
		{
		}
		protected Int32? _LedgerID;

		/// <summary>
		/// Identifier of the <see cref="Ledger"/>, which the history record belongs.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="Ledger.LedgerID"/> field.
		/// </value>
		[PXDBInt(IsKey = true, BqlField = typeof(GLHistory.ledgerID))]
		[PXUIField(DisplayName = "Ledger")]
		[PXSelector(typeof(Ledger.ledgerID), SubstituteKey = typeof(Ledger.ledgerCD))]
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
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;

		/// <summary>
		/// Identifier of the <see cref="Account"/> associated with the history record.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="Account.AccountID"/> field.
		/// </value>
		[Account(IsKey = true, BqlField = typeof(GLHistory.accountID))]
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
		public abstract class subID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubID;

		/// <summary>
		/// Identifier of the <see cref="Sub">Subaccount</see> associated with the history record.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="Sub.SubID"/> field.
		/// </value>
		[SubAccount(IsKey = true, BqlField = typeof(GLHistory.subID))]
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
		#region LastActivityPeriod
		public abstract class lastActivityPeriod : PX.Data.IBqlField
		{
		}
		protected String _LastActivityPeriod;

		/// <summary>
		/// Identifier of the <see cref="FinPeriod">Financial Period</see> of the last activity on the Account and Subaccount associated with the history record,
		/// with regards to Ledger and Branch.
		/// </summary>
		[GL.FinPeriodID(BqlField = typeof(GLHistory.finPeriodID))]
		[PXUIField(DisplayName = "Last Activity Period", Visibility = PXUIVisibility.Invisible)]
		public virtual String LastActivityPeriod
		{
			get
			{
				return this._LastActivityPeriod;
			}
			set
			{
				this._LastActivityPeriod = value;
			}
		}
		#endregion
	}
}
