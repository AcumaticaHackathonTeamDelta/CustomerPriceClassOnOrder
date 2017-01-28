namespace PX.Objects.CA
{
	using System;
	using PX.Data;
	using PX.Objects.GL;
	using PX.Objects.CM;
#if PTInstStatement
	[System.SerializableAttribute()]
	public partial class PTInstStatement : PX.Data.IBqlTable
	{
		#region CashAccountID
		public abstract class cashAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _CashAccountID;
		[PXDefault()]
		[GL.CashAccountAttribute(DisplayName = "Cash Account", Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(CashAccount.descr), Enabled = true, IsKey = true)]
		public virtual Int32? CashAccountID
		{
			get
			{
				return this._CashAccountID;
			}
			set
			{
				this._CashAccountID = value;
			}
		}
		#endregion
		#region StatementID
		public abstract class statementID : PX.Data.IBqlField
		{
		}
		protected String _StatementID;
		[PXDBString(15, IsUnicode = true, IsKey = true)]
		
		[PXDefault()]
		[PXUIField(DisplayName = "Statement ID",Visibility= PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<PTInstStatement.statementID,Where<PTInstStatement.cashAccountID,Equal<Current<PTInstStatement.cashAccountID>>>>))]
		[CS.AutoNumber(typeof(CASetup.pTInstStmtNumbering), typeof(PTInstStatement.statementDate))]
		public virtual String StatementID
		{
			get
			{
				return this._StatementID;
			}
			set
			{
				this._StatementID = value;
			}
		}
		#endregion
	#region StatementDate
		public abstract class statementDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _StatementDate;
		[PXDBDate()]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Statement Date")]
		public virtual DateTime? StatementDate
		{
			get
			{
				return this._StatementDate;
			}
			set
			{
				this._StatementDate = value;
			}
		}
	#endregion

	#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXDBString(5, IsUnicode = true)]
		[PXDefault(typeof(CashAccount.curyID))]
		[PXUIField(DisplayName = "Currency", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[PXSelector(typeof(Currency.curyID))]
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
	#region CuryInfoID
		public abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _CuryInfoID;
		[PX.Objects.CM.CurrencyInfo()]
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

	#region Status
		public abstract class status : PX.Data.IBqlField
		{
		}
		protected String _Status;
		[PXDBString(1, IsFixed = true)]
		[PXDefault("O")]
		//[CADocStatus.List()]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual String Status
		{
			get
			{
				return this._Status;
			}
			set
			{
				this._Status = value;
			}
		}
		#endregion
	#region StmtRefNbr
		public abstract class stmtRefNbr : PX.Data.IBqlField
		{
		}
		protected String _StmtRefNbr;
		[PXDBString(20, IsUnicode = true)]
		[PXUIField(DisplayName = "Ext. Ref. Nbr.")]
		public virtual String StmtRefNbr
		{
			get
			{
				return this._StmtRefNbr;
			}
			set
			{
				this._StmtRefNbr = value;
			}
		}
	#endregion
	#region CuryBegBalance
		public abstract class curyBegBalance : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryBegBalance;
		[PXDBCurrency(typeof(PTInstStatement.curyInfoID), typeof(PTInstStatement.begBalance))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Beginning Balance", Enabled = false)]
		public virtual Decimal? CuryBegBalance
		{
			get
			{
				return this._CuryBegBalance;
			}
			set
			{
				this._CuryBegBalance = value;
			}
		}
	#endregion
	#region CuryEndBalance
		public abstract class curyEndBalance : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryEndBalance;
		[PXDBCurrency(typeof(PTInstStatement.curyInfoID), typeof(PTInstStatement.endBalance))]
		[PXDefault(TypeCode.Decimal,"0.0")]
		public virtual Decimal? CuryEndBalance
		{
			get
			{
				return this._CuryEndBalance;
			}
			set
			{
				this._CuryEndBalance = value;
			}
		}
	#endregion

	#region CuryPayments
		public abstract class curyPayments : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryPayments;
		[PXDBCurrency(typeof(PTInstStatement.curyInfoID), typeof(PTInstStatement.payments))]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Payments")]
		public virtual Decimal? CuryPayments
		{
			get
			{
				return this._CuryPayments;
			}
			set
			{
				this._CuryPayments = value;
			}
		}
		#endregion
	#region CuryExpenses
		public abstract class curyExpenses : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryExpenses;
		[PXDBCurrency(typeof(PTInstStatement.curyInfoID), typeof(PTInstStatement.expenses))]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Expenses")]
		public virtual Decimal? CuryExpenses
		{
			get
			{
				return this._CuryExpenses;
			}
			set
			{
				this._CuryExpenses = value;
			}
		}
	#endregion
	#region CuryBookedExps
		public abstract class curyBookedExps : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryBookedExps;
		[PXDBCurrency(typeof(PTInstStatement.curyInfoID), typeof(PTInstStatement.bookedExps))]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Booked Expenses")]
		public virtual Decimal? CuryBookedExps
		{
			get
			{
				return this._CuryBookedExps;
			}
			set
			{
				this._CuryBookedExps = value;
			}
		}
	#endregion
	#region CuryCharges
		public abstract class curyCharges : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryCharges;
		[PXDBCurrency(typeof(PTInstStatement.curyInfoID), typeof(PTInstStatement.charges))]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Charges")]
		public virtual Decimal? CuryCharges
		{
			get
			{
				return this._CuryCharges;
			}
			set
			{
				this._CuryCharges = value;
			}
		}
	#endregion

	#region BegBalance
		public abstract class begBalance : PX.Data.IBqlField
		{
		}
		protected Decimal? _BegBalance;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal,"0.0")]
		public virtual Decimal? BegBalance
		{
			get
			{
				return this._BegBalance;
			}
			set
			{
				this._BegBalance = value;
			}
		}
		#endregion
	#region EndBalance
		public abstract class endBalance : PX.Data.IBqlField
		{
		}
		protected Decimal? _EndBalance;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal,"0.0")]
		public virtual Decimal? EndBalance
		{
			get
			{
				return this._EndBalance;
			}
			set
			{
				this._EndBalance = value;
			}
		}
	#endregion

	#region Payments
		public abstract class payments : PX.Data.IBqlField
		{
		}
		protected Decimal? _Payments;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal,"0.0")]
		public virtual Decimal? Payments
		{
			get
			{
				return this._Payments;
			}
			set
			{
				this._Payments = value;
			}
		}
		#endregion
	#region Expenses
		public abstract class expenses : PX.Data.IBqlField
		{
		}
		protected Decimal? _Expenses;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal,"0.0")]
		public virtual Decimal? Expenses
		{
			get
			{
				return this._Expenses;
			}
			set
			{
				this._Expenses = value;
			}
		}
	#endregion
	#region BookedExps
		public abstract class bookedExps : PX.Data.IBqlField
		{
		}
		protected Decimal? _BookedExps;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal,"0.0")]
		public virtual Decimal? BookedExps
		{
			get
			{
				return this._BookedExps;
			}
			set
			{
				this._BookedExps = value;
			}
		}
	#endregion
	#region Charges
		public abstract class charges : PX.Data.IBqlField
		{
		}
		protected Decimal? _Charges;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal,"0.0")]
		public virtual Decimal? Charges
		{
			get
			{
				return this._Charges;
			}
			set
			{
				this._Charges = value;
			}
		}
	#endregion
	#region PrevStatementID
		public abstract class prevStatementID : PX.Data.IBqlField
		{
		}
		protected String _PrevStatementID;
		[PXDBString(15, IsUnicode = true)]
		public virtual String PrevStatementID
		{
			get
			{
				return this._PrevStatementID;
			}
			set
			{
				this._PrevStatementID = value;
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

	}
#endif
}
