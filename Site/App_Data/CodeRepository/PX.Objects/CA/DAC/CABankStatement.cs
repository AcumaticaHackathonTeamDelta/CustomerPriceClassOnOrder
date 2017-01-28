namespace PX.Objects.CA
{
	using System;
	using PX.Data;
	using PX.Objects.GL;
	using PX.Objects.CM;
	
	[System.SerializableAttribute()]
	[PXPrimaryGraph(typeof(CABankStatementEntry))]
	public partial class CABankStatement : PX.Data.IBqlTable
	{
		#region CashAccountID
		public abstract class cashAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _CashAccountID;

		[PXDefault()]
        [GL.CashAccount(null, typeof(Search<CashAccount.cashAccountID,
						Where2<Match<Current<AccessInfo.userName>>,
						And<CashAccount.clearingAccount, Equal<CS.boolFalse>>>>), IsKey = true, DisplayName = "Cash Account", 
						Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(CashAccount.descr), Required = true)]
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
		#region CashAccountID1
		public abstract class cashAccountID1 : PX.Data.IBqlField
		{
		}


        [PXSelector(typeof(Search<CashAccount.cashAccountID,
						Where2<Match<Current<AccessInfo.userName>>,
						And<CashAccount.clearingAccount, Equal<CS.boolFalse>>>>),
						SubstituteKey = typeof(CashAccount.extRefNbr),
						 DescriptionField = typeof(CashAccount.descr))]		
		[PXUIField(DisplayName = "Cash Account By Ext. Ref. Nbr")]
		public virtual Int32? CashAccountID1
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
		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
		}
		protected String _RefNbr;
		[PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXDefault()]
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible, Required = true)]
		[CABankStatementType.Numbering()]
		[CABankStatementType.RefNbr(typeof(Search<CABankStatement.refNbr, Where<CABankStatement.cashAccountID, Equal<Optional<CABankStatement.cashAccountID>>>>))]
		
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
		#region ExtRefNbr
		public abstract class extRefNbr : PX.Data.IBqlField
		{
		}
		protected String _ExtRefNbr;
		[PXDBString(40, IsUnicode = true)]		
		[PXUIField(DisplayName = "Ext. Ref. Nbr.")]
		public virtual String ExtRefNbr
		{
			get
			{
				return this._ExtRefNbr;
			}
			set
			{
				this._ExtRefNbr = value;
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
		[PXDBString(5, InputMask = ">LLLLL", IsUnicode = true)]
		[PXSelector(typeof(Currency.curyID))]
		[PXDefault(typeof(Search<CashAccount.curyID, Where<CashAccount.cashAccountID, Equal<Current<CABankStatement.cashAccountID>>>>))]
		[PXUIField(DisplayName = "Currency")]
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
#if UseCuryInfo

		#region CuryInfoID
		public abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _CuryInfoID;

		[PXDBLong()]
		[CurrencyInfo(ModuleCode = BatchModule.CA)]
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
		
#endif
		#region StartBalanceDate
		public abstract class startBalanceDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _StartBalanceDate;
		[PXDBDate()]
		[PXDefault(typeof(Search<CABankStatement.endBalanceDate,
					 Where<CABankStatement.cashAccountID,Equal<Current<CABankStatement.cashAccountID>>,
                        And<CABankStatement.endBalanceDate,LessEqual<Current<CABankStatement.statementDate>>,
                        And<Where<Current<CABankStatement.refNbr>,IsNull, Or<CABankStatement.refNbr,NotEqual<Current<CABankStatement.refNbr>>>>>>>,
                        OrderBy<Asc<CABankStatement.startBalanceDate>>>),
						PersistingCheck = PXPersistingCheck.NullOrBlank)] //Normally it should be desc on order by, but PXDefault takes the last record
		[PXUIField(DisplayName = "Start Balance Date")]
		public virtual DateTime? StartBalanceDate
		{
			get
			{
				return this._StartBalanceDate;
			}
			set
			{
				this._StartBalanceDate = value;
			}
		}
		#endregion
		#region EndBalanceDate
		public abstract class endBalanceDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _EndBalanceDate;
		[PXDBDate()]
		[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
		[PXUIField(DisplayName = "End Balance Date")]
		public virtual DateTime? EndBalanceDate
		{
			get
			{
				return this._EndBalanceDate;
			}
			set
			{
				this._EndBalanceDate = value;
			}
		}
		#endregion
		#region CuryBegBalance
		public abstract class curyBegBalance : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryBegBalance;

#if UseCuryInfo
		[PXDBCurrency(typeof(CABankStatement.curyInfoID), typeof(CABankStatement.begBalance))]
#else
		[PXDBCury(typeof(CABankStatement.curyID))]
		  
#endif
		[PXDefault(TypeCode.Decimal, "0.0", 
			typeof(Search<CABankStatement.curyEndBalance,
				 Where<CABankStatement.cashAccountID,Equal<Current<CABankStatement.cashAccountID>>,
                    And<CABankStatement.endBalanceDate,LessEqual<Current<CABankStatement.statementDate>>,
                    And<Where<Current<CABankStatement.refNbr>,IsNull, Or<CABankStatement.refNbr,NotEqual<Current<CABankStatement.refNbr>>>>>>>,
                    OrderBy<Asc<CABankStatement.startBalanceDate>>>))] //Normally it shold be desc on order by, but PXDefault takes the last record
		[PXUIField(DisplayName = "Beginning Balance")]
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
		[PXDefault(TypeCode.Decimal, "0.0")]
#if UseCuryInfo
		[PXDBCurrency(typeof(CABankStatement.curyInfoID), typeof(CABankStatement.endBalance))]
#else
		[PXDBCury(typeof(CABankStatement.curyID))]
#endif	
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
#if UseCuryInfo
		#region BegBalance
		public abstract class begBalance : PX.Data.IBqlField
		{
		}
		protected Decimal? _BegBalance;
		[PXDBDecimal(19)]
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
		[PXDBDecimal(19)]
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
		
#endif
		#region CuryDebitsTotal
		public abstract class curyDebitsTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryDebitsTotal;
		[PXDefault(TypeCode.Decimal, "0.0")]
		//[PXDBCurrency(typeof(CABankStatement.curyInfoID), typeof(CABankStatement.debitsTotal))]
		[PXDBCury(typeof(CABankStatement.curyID))]
		[PXUIField(DisplayName = "Total Receipts", Enabled = false)]
		public virtual Decimal? CuryDebitsTotal
		{
			get
			{
				return this._CuryDebitsTotal;
			}
			set
			{
				this._CuryDebitsTotal = value;
			}
		}
		#endregion
		#region CuryCreditsTotal
		public abstract class curyCreditsTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryCreditsTotal;
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCury(typeof(CABankStatement.curyID))]
		//[PXDBCurrency(typeof(CABankStatement.curyInfoID), typeof(CABankStatement.creditsTotal))]
		[PXUIField(DisplayName = "Total Disbursements", Enabled = false)]
		public virtual Decimal? CuryCreditsTotal
		{
			get
			{
				return this._CuryCreditsTotal;
			}
			set
			{
				this._CuryCreditsTotal = value;
			}
		}
		#endregion
#if UseCuryInfo
		#region DebitsTotal
		public abstract class debitsTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _DebitsTotal;
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(CABankStatement.curyInfoID), typeof(CABankStatement.debitsTotal))]
		public virtual Decimal? DebitsTotal
		{
			get
			{
				return this._DebitsTotal;
			}
			set
			{
				this._DebitsTotal = value;
			}
		}
		#endregion
		#region CreditsTotal
		public abstract class creditsTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CreditsTotal;
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(CABankStatement.curyInfoID), typeof(CABankStatement.creditsTotal))]
		public virtual Decimal? CreditsTotal
		{
			get
			{
				return this._CreditsTotal;
			}
			set
			{
				this._CreditsTotal = value;
			}
		}
		#endregion
#endif
		#region CountDebit
		public abstract class countDebit : PX.Data.IBqlField
		{
		}
		protected Int32? _CountDebit;
		[PXDBInt()]
		[PXDefault(0)]
		[PXUIField(DisplayName = "Count Debit", Enabled = false)]
		public virtual Int32? CountDebit
		{
			get
			{
				return this._CountDebit;
			}
			set
			{
				this._CountDebit = value;
			}
		}
		#endregion
		#region CountCredit
		public abstract class countCredit : PX.Data.IBqlField
		{
		}
		protected Int32? _CountCredit;
		[PXDBInt()]
		[PXDefault(0)]
		[PXUIField(DisplayName = "Count Credit", Enabled = false)]
		public virtual Int32? CountCredit
		{
			get
			{
				return this._CountCredit;
			}
			set
			{
				this._CountCredit = value;
			}
		}
		#endregion
		#region CuryDetailsEndBalance
		public abstract class curyDetailsEndBalance : PX.Data.IBqlField
		{
		}

		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "End Balance", Enabled = false)]
		[PXCury(typeof(CABankStatement.curyID))]
		public virtual Decimal? CuryDetailsEndBalance
		{
			[PXDependsOnFields(typeof(curyBegBalance),typeof(curyDebitsTotal),typeof(curyCreditsTotal))]
			get
			{
				return this._CuryBegBalance + (this.CuryDebitsTotal - this.CuryCreditsTotal);
			}
			set
			{

			}
		}
		#endregion
		#region CuryDetailsBalanceDiff
		public abstract class curyDetailsBalanceDiff : PX.Data.IBqlField
		{
		}

		[PXCury(typeof(CABankStatement.curyID))]
		[PXUIField(DisplayName = "Difference", Enabled = false)]
		public virtual Decimal? CuryDetailsBalanceDiff
		{
			[PXDependsOnFields(typeof(curyEndBalance),typeof(curyDetailsEndBalance))]
			get
			{
				return this.CuryEndBalance - this.CuryDetailsEndBalance;
			}
			set
			{

			}
		}
		#endregion

		#region CuryReconciledDebits
		public abstract class curyReconciledDebits : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryReconciledDebits;
		[PXDefault(TypeCode.Decimal, "0.0")]

#if UseCuryInfo
		[PXDBCurrency(typeof(CABankStatement.curyInfoID), typeof(CABankStatement.reconciledDebits))]
		
#else	
		[PXDBCury(typeof(CABankStatement.curyID))] 
#endif
		[PXUIField(DisplayName = "Reconciled", Enabled = false)]
		public virtual Decimal? CuryReconciledDebits
		{
			get
			{
				return this._CuryReconciledDebits;
			}
			set
			{
				this._CuryReconciledDebits = value;
			}
		}
		#endregion
		#region CuryReconciledCredits
		public abstract class curyReconciledCredits : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryReconciledCredits;
		[PXDefault(TypeCode.Decimal, "0.0")]
#if UseCuryInfo
		[PXDBCurrency(typeof(CABankStatement.curyInfoID), typeof(CABankStatement.reconciledCredits))]
#else
		[PXDBCury(typeof(CABankStatement.curyID))]
#endif
		[PXUIField(DisplayName = "Reconciled Credits", Enabled = false)]
		public virtual Decimal? CuryReconciledCredits
		{
			get
			{
				return this._CuryReconciledCredits;
			}
			set
			{
				this._CuryReconciledCredits = value;
			}
		}
		#endregion
#if UseCuryInfo
		
		#region ReconciledDebits
		public abstract class reconciledDebits : PX.Data.IBqlField
		{
		}
		protected Decimal? _ReconciledDebits;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? ReconciledDebits
		{
			get
			{
				return this._ReconciledDebits;
			}
			set
			{
				this._ReconciledDebits = value;
			}
		}
		#endregion		
		#region ReconciledCredits
		public abstract class reconciledCredits : PX.Data.IBqlField
		{
		}
		protected Decimal? _ReconciledCredits;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]		
		public virtual Decimal? ReconciledCredits
		{
			get
			{
				return this._ReconciledCredits;
			}
			set
			{
				this._ReconciledCredits = value;
			}
		}
		#endregion

  
#endif		
		#region ReconciledCountDebit
		public abstract class reconciledCountDebit : PX.Data.IBqlField
		{
		}
		protected Int32? _ReconciledCountDebit;
		[PXDBInt()]
		[PXDefault(0)]
		[PXUIField(DisplayName = "Reconciled Debit Count",Enabled=false)]
		public virtual Int32? ReconciledCountDebit
		{
			get
			{
				return this._ReconciledCountDebit;
			}
			set
			{
				this._ReconciledCountDebit = value;
			}
		}
		#endregion
		#region ReconciledCountCredit
		public abstract class reconciledCountCredit : PX.Data.IBqlField
		{
		}
		protected Int32? _ReconciledCountCredit;
		[PXDBInt()]
		[PXDefault(0)]
		[PXUIField(DisplayName = "Reconciled Credit Count",Enabled=false)]
		public virtual Int32? ReconciledCountCredit
		{
			get
			{
				return this._ReconciledCountCredit;
			}
			set
			{
				this._ReconciledCountCredit = value;
			}
		}
		#endregion
		#region CuryReconciledEndBalance
		public abstract class curyReconciledEndBalance : PX.Data.IBqlField
		{
		}

		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Reconciled End Balance", Enabled = false)]
		[PXCury(typeof(CABankStatement.curyID))]
		public virtual Decimal? CuryReconciledEndBalance
		{
			[PXDependsOnFields(typeof(curyBegBalance),typeof(curyReconciledDebits),typeof(curyReconciledCredits))]
			get
			{
				return this._CuryBegBalance + (this.CuryReconciledDebits - this.CuryReconciledCredits);
			}
			set
			{

			}
		}
		#endregion
		#region CuryDiffBalance
		public abstract class curyReconciledBalanceDiff : PX.Data.IBqlField
		{
		}

		[PXCury(typeof(CABankStatement.curyID))]
		[PXUIField(DisplayName = "Difference", Enabled = false)]
		public virtual Decimal? CuryReconciledBalanceDiff
		{
			[PXDependsOnFields(typeof(curyEndBalance),typeof(curyReconciledEndBalance))]
			get
			{
				return this.CuryEndBalance - this.CuryReconciledEndBalance;
			}
			set
			{

			}
		}
		#endregion

		#region Status
		public abstract class status : PX.Data.IBqlField
		{
		}
		protected String _Status;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(CABankStatementStatus.Hold)]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[CABankStatementStatus.List()]
		public virtual String Status
		{
			[PXDependsOnFields(typeof(released), typeof(hold))]
			get
			{
				return this._Status;
			}
			set
			{
				//this._Status = value;
			}
		}
		#endregion
		#region Reconciled
		public abstract class reconciled : PX.Data.IBqlField
		{
		}
		protected Boolean? _Reconciled;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Reconciled")]
		public virtual Boolean? Reconciled
		{
			get
			{
				return this._Reconciled;
			}
			set
			{
				this._Reconciled = value;
			}
		}
		#endregion
		#region Hold
		public abstract class hold : PX.Data.IBqlField
		{
		}
		protected Boolean? _Hold;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Hold")]
		public virtual Boolean? Hold
		{
			get
			{
				return this._Hold;
			}
			set
			{
				this._Hold = value;
				this.SetStatus();
			}
		}
		#endregion
		#region Released
		public abstract class released : PX.Data.IBqlField
		{
		}
		protected Boolean? _Released;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Released",Visible = false)]
		public virtual Boolean? Released
		{
			get
			{
				return this._Released;
			}
			set
			{
				this._Released = value;
				this.SetStatus();
			}
		}
		#endregion
		#region Voided
		public abstract class voided : PX.Data.IBqlField
		{
		}
		protected Boolean? _Voided;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Voided",Visible=false)]
		public virtual Boolean? Voided
		{
			get
			{
				return this._Voided;
			}
			set
			{
				this._Voided = value;
			}
		}
		#endregion
		#region Imported
		public abstract class imported : PX.Data.IBqlField
		{
		}
		protected Boolean? _Imported;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Imported",Visible = false)]
		public virtual Boolean? Imported
		{
			get
			{
				return this._Imported;
			}
			set
			{
				this._Imported = value;
			}
		}
		#endregion

		#region BankStatementFormat
		public abstract class bankStatementFormat : PX.Data.IBqlField
		{
		}
		protected String _BankStatementFormat;
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Bank Statements Format")]
		public virtual String BankStatementFormat
		{
			get
			{
				return this._BankStatementFormat;
			}
			set
			{
				this._BankStatementFormat = value;
			}
		}
		#endregion
		#region FormatVerisionNbr
		public abstract class formatVerisionNbr : PX.Data.IBqlField
		{
		}
		protected String _FormatVerisionNbr;
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Format Verision Nbr")]
		public virtual String FormatVerisionNbr
		{
			get
			{
				return this._FormatVerisionNbr;
			}
			set
			{
				this._FormatVerisionNbr = value;
			}
		}
		#endregion
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXNote(new Type[0])]
		public virtual Guid? NoteID
		{
			get
			{
				return this._NoteID;
			}
			set
			{
				this._NoteID = value;
			}
		}
		#endregion
		#region LineCntr
		public abstract class lineCntr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineCntr;
		[PXDBInt()]
		[PXDefault(0)]
		public virtual Int32? LineCntr
		{
			get
			{
				return this._LineCntr;
			}
			set
			{
				this._LineCntr = value;
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
		#region TranMaxDate
		public abstract class tranMaxDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _TranMaxDate;
		[PXDBDate()]		
		public virtual DateTime? TranMaxDate
		{
			get
			{
				return this._TranMaxDate;
			}
			set
			{
				this._TranMaxDate = value;
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

		#region CuryBegBalanceRO
		public abstract class curyBegBalanceRO : PX.Data.IBqlField
		{
		}

		[PXCury(typeof(CABankStatement.curyID))]
		[PXUIField(DisplayName = "Beginning Balance")]
		public virtual Decimal? CuryBegBalanceRO
		{
			[PXDependsOnFields(typeof(curyBegBalance))]
			get
			{
				return this._CuryBegBalance;
			}
			set{}
		}
		#endregion
		#region CuryEndBalanceRO
		public abstract class curyEndBalanceRO : PX.Data.IBqlField
		{
		}
		
		[PXCury(typeof(CABankStatement.curyID))]
		[PXUIField(DisplayName = "Ending Balance")]
		public virtual Decimal? CuryEndBalanceRO
		{
			[PXDependsOnFields(typeof(curyEndBalance))]
			get
			{
				return this._CuryEndBalance;
			}
			set{}
		}
		#endregion
		#region Methods
		protected virtual void SetStatus()
		{
			if (this._Released != null && (bool)this._Released)
			{
				this._Status = CABankStatementStatus.Released;
			}
			else if (this._Hold != null && (bool)this._Hold)
			{
				this._Status = CABankStatementStatus.Hold;
			}
			else
			{
				this._Status = CABankStatementStatus.Balanced;
			}
		}
		#endregion
	}



	public static class CABankStatementType
	{
        /// <summary>
        /// Specialized selector for CABatch RefNbr.<br/>
        /// By default, defines the following set of columns for the selector:<br/>
        /// CABankStatement.refNbr, CABankStatement.cashAccountID, CABankStatement.curyID,
		/// CABankStatement.statementDate, CABankStatement.endBalanceDate, CABankStatement.curyEndBalance,
		/// CABankStatement.extRefNbr
        /// <example>
        /// [CABankStatementType.RefNbr(typeof(Search<CABankStatement.refNbr, Where<CABankStatement.cashAccountID, Equal<Optional<CABankStatement.cashAccountID>>>>))]
        /// </example>
        /// </summary>
        public class RefNbrAttribute : PXSelectorAttribute
		{
			public RefNbrAttribute(Type SearchType)
				: base(SearchType,
				typeof(CABankStatement.refNbr),
				typeof(CABankStatement.cashAccountID),
				typeof(CABankStatement.curyID),
				typeof(CABankStatement.statementDate),
				typeof(CABankStatement.endBalanceDate),
				typeof(CABankStatement.curyEndBalance),
                typeof(CABankStatement.extRefNbr),
                typeof(CABankStatement.status))
			{
			}
		}

        /// <summary>
        /// Specialized for CABankStatement version of the <see cref="AutoNumberAttribute"/><br/>
        /// It defines how the new numbers are generated for the AR Payment. <br/>
        /// References CABankStatement.statementDate fields of the document,<br/>
        /// and also define a link between  numbering ID's defined in CA Setup 
        /// and CABankStatement: namely CASetup.cAStatementNumberingID. <br/>        
        /// </summary>		
		public class NumberingAttribute : CS.AutoNumberAttribute
		{
			public NumberingAttribute()
				: base(typeof(CASetup.cAStatementNumberingID), typeof(CABankStatement.statementDate))
			{
			}

		}
	}


	public class CABankStatementStatus
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Hold, Balanced, Released },
				new string[] { Messages.Hold, Messages.Balanced, Messages.Released }) { ; }
		}

		public const string Hold = "H";
		public const string Balanced = "B";
		public const string Released = "R";


		public class hold : Constant<string>
		{
			public hold() : base(Hold) { ;}
		}

		public class balanced : Constant<string>
		{
			public balanced() : base(Balanced) { ;}
		}

		public class released : Constant<string>
		{
			public released() : base(Released) { ;}
		}

	}
}
