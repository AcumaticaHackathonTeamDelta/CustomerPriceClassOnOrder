namespace PX.Objects.CA
{
	using System;
	using PX.Data;
	using PX.Objects.CM;
#if PTInstStatement
	[System.SerializableAttribute()]
	public partial class PTInstStmtDetail : PX.Data.IBqlTable
	{
		#region CashAccountID
		public abstract class cashAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _CashAccountID;
		[PXDBInt(IsKey = true)]
		[PXDefault(typeof(PTInstStatement.cashAccountID))]
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
		[PXDBDefault(typeof(PTInstStatement.statementID))]
		[PXParent(typeof(Select<PTInstStatement,
						Where<PTInstStatement.statementID,Equal<Current<PTInstStmtDetail.statementID>>,
							And<PTInstStatement.cashAccountID,Equal<Current<PTInstStmtDetail.cashAccountID>>
							>>>))]
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
	#region PTInstanceID
		public abstract class pTInstanceID : PX.Data.IBqlField
		{
		}
		protected Int32? _PTInstanceID;
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Card" )]
		[PXSelector(typeof(Search<PaymentTypeInstance.pTInstanceID,
					Where<PaymentTypeInstance.cashAccountID,
					Equal<Current<PTInstStmtDetail.cashAccountID>>>>),DescriptionField = typeof(PaymentTypeInstance.descr))]
		public virtual Int32? PTInstanceID
		{
			get
			{
				return this._PTInstanceID;
			}
			set
			{
				this._PTInstanceID = value;
			}
		}
	#endregion
	#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXDBString(5, IsUnicode = true)]
		[PXDefault(typeof(PTInstStatement.curyID))]
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
		[PXDBLong]
		[PXDBDefault(typeof(PTInstStatement.curyInfoID))]
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
	#region CuryBegBalance
		public abstract class curyBegBalance : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryBegBalance;
		[PXDBCurrency(typeof(PTInstStmtDetail.curyInfoID), typeof(PTInstStmtDetail.begBalance))]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Beg. Balance")]
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
		[PXDBCurrency(typeof(PTInstStmtDetail.curyInfoID), typeof(PTInstStmtDetail.endBalance))]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Ending Balance")]
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
		[PXDBCurrency(typeof(PTInstStmtDetail.curyInfoID), typeof(PTInstStmtDetail.payments))]
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
		[PXDBCurrency(typeof(PTInstStmtDetail.curyInfoID), typeof(PTInstStmtDetail.expenses))]
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
		[PXDBCurrency(typeof(PTInstStmtDetail.curyInfoID), typeof(PTInstStmtDetail.curyBookedExps))]
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
	#region CuryRefunds
		public abstract class curyRefunds : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryRefunds;
		[PXDBCurrency(typeof(PTInstStmtDetail.curyInfoID), typeof(PTInstStmtDetail.refunds))]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Refunds")]
		public virtual Decimal? CuryRefunds
		{
			get
			{
				return this._CuryRefunds;
			}
			set
			{
				this._CuryRefunds = value;
			}
		}
	#endregion
	#region CuryChardes
		public abstract class curyChardes : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryChardes;
		[PXDBCurrency(typeof(PTInstStmtDetail.curyInfoID), typeof(PTInstStmtDetail.charges))]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Late Charges")]
		public virtual Decimal? CuryChardes
		{
			get
			{
				return this._CuryChardes;
			}
			set
			{
				this._CuryChardes = value;
			}
		}
	#endregion
	#region CuryMaintFees
		public abstract class curyMaintFees : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryMaintFees;
		[PXDBCurrency(typeof(PTInstStmtDetail.curyInfoID), typeof(PTInstStmtDetail.maintFees))]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Maint. Fees")]
		public virtual Decimal? CuryMaintFees
		{
			get
			{
				return this._CuryMaintFees;
			}
			set
			{
				this._CuryMaintFees = value;
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
	#region Refunds
		public abstract class refunds : PX.Data.IBqlField
		{
		}
		protected Decimal? _Refunds;
		[PXDBDecimal(19)]
		[PXDefault(TypeCode.Decimal,"0.0")]
		public virtual Decimal? Refunds
		{
			get
			{
				return this._Refunds;
			}
			set
			{
				this._Refunds = value;
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
	#region MaintFees
		public abstract class maintFees : PX.Data.IBqlField
		{
		}
		protected Decimal? _MaintFees;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal,"0.0")]
		public virtual Decimal? MaintFees
		{
			get
			{
				return this._MaintFees;
			}
			set
			{
				this._MaintFees = value;
			}
		}
	#endregion
	#region DocType
		public abstract class docType : PX.Data.IBqlField
		{
		}
		protected String _DocType;
		[PXDBString(3, IsFixed = true)]
		public virtual String DocType
		{
			get
			{
				return this._DocType;
			}
			set
			{
				this._DocType = value;
			}
		}
	#endregion
	#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
		}
		protected String _RefNbr;
		[PXDBString(10, IsUnicode = true)]
		public virtual String RefNbr
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
	#region Cleared
		public abstract class cleared : PX.Data.IBqlField
		{
		}
		protected Boolean? _Cleared;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Cleared")]
		public virtual Boolean? Cleared
		{
			get
			{
				return this._Cleared;
			}
			set
			{
				this._Cleared = value;
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
