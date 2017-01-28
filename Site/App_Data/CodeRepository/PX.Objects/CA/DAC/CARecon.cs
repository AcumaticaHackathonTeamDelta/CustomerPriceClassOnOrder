namespace PX.Objects.CA
{
	using System;
	using PX.Data;
	using PX.Objects.CS;
    using PX.Objects.CR;
	using PX.Objects.GL;
	using PX.Objects.CM;

	public class CADocStatus
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Balanced, Closed, Hold, Voided },
				new string[] { AP.Messages.Balanced, AP.Messages.Closed, AP.Messages.Hold, AP.Messages.Voided }) { ; }
		}

		public const string Balanced = "B";
		public const string Closed	 = "C";
		public const string Hold	 = "H";
        public const string Voided	 = "V";

		public class balanced : Constant<string>
		{
			public balanced() : base(Balanced) { ;}
		}

		public class closed : Constant<string>
		{
			public closed() : base(Closed) { ;}
		}

		public class hold : Constant<string>
		{
			public hold() : base(Hold) { ;}
		}

        public class voided : Constant<string>
        {
            public voided() : base(Voided) { ;}
        }
	}

	[PXCacheName(Messages.CARecon)]
	[System.SerializableAttribute()]
	[PXPrimaryGraph(typeof(CAReconEntry))]
	public partial class CARecon :  PX.Data.IBqlTable
	{
		#region Selected
		public abstract class selected : PX.Data.IBqlField
		{
		}
		protected Boolean? _Selected;
		[PXBool()]
		[PXUIField(DisplayName = "Selected")]
		public virtual Boolean? Selected
		{
			get
			{
				return this._Selected;
			}
			set
			{
				this._Selected = value;
			}
		}
		#endregion

        #region CashAccountID
        public abstract class cashAccountID : PX.Data.IBqlField
        {
        }
        protected Int32? _CashAccountID;
		[PXDefault()]
        [CashAccount(null, typeof(Search<CashAccount.cashAccountID, Where<CashAccount.reconcile, Equal<boolTrue>, And<Match<Current<AccessInfo.userName>>>>>), IsKey = true, Visibility = PXUIVisibility.SelectorVisible, Enabled = true, Required=true)]
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
        #region ReconNbr
		public abstract class reconNbr : PX.Data.IBqlField
		{
		}
		protected String _ReconNbr;
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXDefault()]
		[AutoNumber(typeof(Search<CashAccount.reconNumberingID, Where<CashAccount.cashAccountID, Equal<Current<CARecon.cashAccountID>>>>), typeof(CARecon.reconDate))]
		[PXUIField(DisplayName = "Ref. Number", Visibility = PXUIVisibility.SelectorVisible, Required=true)]
        [PXSelector(typeof(Search<CARecon.reconNbr, Where<CARecon.cashAccountID, Equal<Optional<CARecon.cashAccountID>>>>))] //,
                    //typeof(CARecon.reconNbr), typeof(CARecon.cashAccountID), typeof(CARecon.reconDate), typeof(CARecon.status))]
		public virtual String ReconNbr
		{
			get
			{
				return this._ReconNbr;
			}
			set
			{
				this._ReconNbr = value;
			}
		}
		#endregion
		#region ReconDate
		public abstract class reconDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ReconDate;
		[PXDBDate()]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Reconciliation Date", Visibility = PXUIVisibility.SelectorVisible, Required = true)]
		public virtual DateTime? ReconDate
		{
			get
			{
				return this._ReconDate;
			}
			set
			{
				this._ReconDate = value;
			}
		}
		#endregion
		#region LastReconDate
		public abstract class lastReconDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _LastReconDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Last Reconciliation Date", Enabled = false)]
		public virtual DateTime? LastReconDate
		{
			get
			{
				return this._LastReconDate;
			}
			set
			{
				this._LastReconDate = value;
			}
		}
		#endregion
        #region LoadDocumentsTill
        public abstract class loadDocumentsTill : PX.Data.IBqlField
        {
        }
        protected DateTime? _LoadDocumentsTill;
        [PXDate()]
		[PXDefault(typeof(AccessInfo.businessDate), PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Load Documents Up To", Visibility = PXUIVisibility.SelectorVisible, Required = false)]
        public virtual DateTime? LoadDocumentsTill
        {
            get
            {
                return this._LoadDocumentsTill;
            }
            set
            {
                this._LoadDocumentsTill = value;
            }
        }
        #endregion
		#region Reconciled
		public abstract class reconciled : PX.Data.IBqlField
		{
		}
		protected Boolean? _Reconciled;
		[PXDBBool()]
		[PXUIField(DisplayName = "Reconciled", Enabled = false)]
		[PXDefault(false)]
		public virtual Boolean? Reconciled
		{
			get
			{
				return this._Reconciled;
			}
			set
			{
				this._Reconciled = value;
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
		[PXUIField(DisplayName = "Voided", Enabled = false)]
		[PXDefault(false)]
		public virtual Boolean? Voided
		{
			get
			{
				return this._Voided;
			}
			set
			{
				this._Voided = value;
				this.SetStatus();
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
        [PXUIField(DisplayName="Hold")]
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
        #region CuryBegBalance
        public abstract class curyBegBalance : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryBegBalance;
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXDBCury(typeof(CARecon.curyID))]
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
        #region CuryBalance
        public abstract class curyBalance : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryBalance;
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXDBCury(typeof(CARecon.curyID))]
        [PXUIField(DisplayName = "Statement Balance")]
        public virtual Decimal? CuryBalance
        {
            get
            {
                return this._CuryBalance;
            }
            set
            {
                this._CuryBalance = value;
            }
        }
        #endregion
        #region CuryReconciledDebits
        public abstract class curyReconciledDebits : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryReconciledDebits;
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXDBCurrency(typeof(CARecon.curyInfoID), typeof(CARecon.reconciledDebits))]
        [PXUIField(DisplayName = "Reconciled Receipts", Enabled = false)]
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
        #region ReconciledDebits
        public abstract class reconciledDebits : PX.Data.IBqlField
        {
        }
        protected Decimal? _ReconciledDebits;
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "Reconciled Receipts", Enabled = false, Required = false)]
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
        #region CuryReconciledCredits
        public abstract class curyReconciledCredits : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryReconciledCredits;
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXDBCurrency(typeof(CARecon.curyInfoID), typeof(CARecon.reconciledCredits))]
        [PXUIField(DisplayName = "Reconciled Disb.", Enabled = false)]
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
        #region ReconciledCredits
        public abstract class reconciledCredits : PX.Data.IBqlField
        {
        }
        protected Decimal? _ReconciledCredits;
        [PXDBDecimal(4)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Reconciled Disb.", Enabled = false, Required = false)]
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
        #region CuryReconciledBalance
        public abstract class curyReconciledBalance : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryReconciledBalance;
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Reconciled Balance", Enabled = false)]
        [PXDBCury(typeof(CARecon.curyID))]
        [PXFormula(typeof(Add<CARecon.curyBegBalance, CARecon.curyReconciledTurnover>))]
        public virtual Decimal? CuryReconciledBalance
        {
            get
            {
                return this._CuryReconciledBalance;
            }
            set
            {
                this._CuryReconciledBalance = value;
            }
        }
        #endregion
        #region CuryReconciledTurnover
        public abstract class curyReconciledTurnover : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryReconciledTurnover;
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Reconciled Turnover", Enabled = false)]
        [PXFormula(typeof(Sub<CARecon.curyReconciledDebits, CARecon.curyReconciledCredits>))]
        public virtual Decimal? CuryReconciledTurnover
        {
            get
            {
                return this._CuryReconciledTurnover;
            }
            set
            {
                this._CuryReconciledTurnover = value;
            }
        }
        #endregion
        #region ReconciledTurnover
        public abstract class reconciledTurnover : PX.Data.IBqlField
        {
        }
        protected Decimal? _ReconciledTurnover;
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Reconciled Turnover", Enabled = false)]
        [PXFormula(typeof(Sub<CARecon.reconciledDebits, CARecon.reconciledCredits>))]
        public virtual Decimal? ReconciledTurnover
        {
            get
            {
                return this._ReconciledTurnover;
            }
            set
            {
                this._ReconciledTurnover = value;
            }
        }
        #endregion
        #region CuryDiffBalance
		public abstract class curyDiffBalance : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryDiffBalance;
		[PXDefault(TypeCode.Decimal, "0.0")]
        [PXDBCury(typeof(CARecon.curyID))]
		[PXUIField(DisplayName = "Difference", Enabled = false)]
		[PXFormula(typeof(Sub<CARecon.curyBalance, CARecon.curyReconciledBalance>))]
        public virtual Decimal? CuryDiffBalance
		{
			get
			{
                return this._CuryDiffBalance;
			}
			set
			{
                this._CuryDiffBalance = value;
			}
		}
		#endregion
        #region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL")]
		[PXUIField(DisplayName = "Currency", Enabled = false)]
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
		[PXDBLong()]
        //[PXDefault(0L)]
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
		[PXString(1, IsFixed = true)]
		[CADocStatus.List()]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual string Status
		{
			[PXDependsOnFields(typeof(reconciled), typeof(voided), typeof(hold))]
			get
			{
				return this._Status;
			}
			set
			{
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
		#region CountDebit
		public abstract class countDebit : PX.Data.IBqlField
		{
		}
		protected Int32? _CountDebit;
		[PXDBInt()]
		[PXDefault(0)]
		[PXUIField(DisplayName = "Receipt Count", Enabled = false)]
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
		[PXUIField(DisplayName = "Disb. Count", Enabled = false)]
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
		#region SkipVoided
		public abstract class skipVoided : PX.Data.IBqlField
		{
		}
		protected Boolean? _SkipVoided;
		[PXDBBool()]
		[PXDefault(typeof(CASetup.skipVoided))]
		[PXUIField(DisplayName = "Voided Transactions Are Skipped", Enabled = false, Visible = false)]
		public virtual Boolean? SkipVoided
		{
			get
			{
				return this._SkipVoided;
			}
			set
			{
				this._SkipVoided = value;
			}
		}
		#endregion
		#region ShowBatchPayments
		public abstract class showBatchPayments : PX.Data.IBqlField
		{
		}
		protected Boolean? _ShowBatchPayments;
		[PXDBBool()]
		[PXDefault(typeof(Search<CashAccount.matchToBatch, Where<CashAccount.cashAccountID, Equal<Current<CARecon.cashAccountID>>>>))]
		[PXUIField(DisplayName = "Bank Transactions Are Matched to Batch Payments", Enabled = false, Visible = false)]
		public virtual Boolean? ShowBatchPayments
		{
			get
			{
				return this._ShowBatchPayments;
			}
			set
			{
				this._ShowBatchPayments = value;
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
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXNote(DescriptionField = typeof(CARecon.reconNbr))]
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
		#region Methods
		private void SetStatus()
		{
			if (this._Reconciled == true)
			{
				this._Status = CADocStatus.Closed;
			}
			else if (this._Voided == true)
			{
				this._Status = CADocStatus.Voided;
			}
			else if (this._Hold == true)
			{
				this._Status = CADocStatus.Hold;
			}
			else
			{
				this._Status = CADocStatus.Balanced;
			}
		}
		#endregion
	}

    [Serializable]
    [PXHidden]
	public partial class AddDetailFilter : PX.Data.IBqlTable
	{
		#region OrigModule
		public abstract class origModule : PX.Data.IBqlField
		{
		}
		protected String _OrigModule;
		[PXString(2, IsFixed = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Orig. Module")]
		public virtual String OrigModule
		{
			get
			{
				return this._OrigModule;
			}
			set
			{
				this._OrigModule = value;
			}
		}
		#endregion
		#region OrigTranType
		public abstract class origTranType : PX.Data.IBqlField
		{
		}
		protected String _OrigTranType;
		[PXString(3)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Transaction Type")]
		[CAAPARTranType.ListByModule(typeof(origModule))]
		public virtual String OrigTranType
		{
			get
			{
				return this._OrigTranType;
			}
			set
			{
				this._OrigTranType = value;
			}
		}
		#endregion
		#region TranDate
		public abstract class tranDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _TranDate;
		[PXDate()]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Doc. Date")]
		public virtual DateTime? TranDate
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
		#region CashAccountID
		public abstract class cashAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _CashAccountID;
		[PXDefault()]
		[GL.CashAccount(DisplayName = "Cash Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(CashAccount.descr))]
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
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL")]
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
		#region ExtRefNbr
		public abstract class extRefNbr : PX.Data.IBqlField
		{
		}
		protected String _ExtRefNbr;
		[PXDBString(40, IsUnicode = true)]
		[PXUIField(DisplayName = "Document Ref.")]
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
		#region EntryTypeID
		public abstract class entryTypeID : PX.Data.IBqlField
		{
		}
		protected String _EntryTypeID;
		[PXString(10, IsUnicode = true)]
		[PXSelector(typeof(Search<CAEntryType.entryTypeId>))]
		[PXUIField(DisplayName = "Entry Type")]
		public virtual String EntryTypeID
		{
			get
			{
				return this._EntryTypeID;
			}
			set
			{
				this._EntryTypeID = value;
			}
		}
		#endregion
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;
		[PXString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description")]
		public virtual String Descr
		{
			get
			{
				return this._Descr;
			}
			set
			{
				this._Descr = value;
			}
		}
		#endregion
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[Account(DisplayName = "Offset Account")]
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
		[SubAccount(typeof(AddDetailFilter.accountID), DisplayName = "Offset Subaccount")]
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
		#region CuryTranAmt
		public abstract class curyTranAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryTranAmt;
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Amount")]
		[PXDecimal(2)]
		public virtual Decimal? CuryTranAmt
		{
			get
			{
				return this._CuryTranAmt;
			}
			set
			{
				this._CuryTranAmt = value;
			}
		}
		#endregion
		#region DrCr
		public abstract class drCr : PX.Data.IBqlField
		{
		}
		protected String _DrCr;
		[PXDefault(GL.DrCr.Debit)]
		[PXString(1, IsFixed = true)]
		[CADrCr.List()]
		[PXUIField(DisplayName = "Debit / Credit")]
		public virtual String DrCr
		{
			get
			{
				return this._DrCr;
			}
			set
			{
				this._DrCr = value;
			}
		}
		#endregion
		#region ReferenceID
		public abstract class referenceID : PX.Data.IBqlField
		{
		}
		protected Int32? _ReferenceID;
		[PXInt()]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXSelector(typeof(BAccount.bAccountID),
						SubstituteKey = typeof(BAccount.acctCD),
					 DescriptionField = typeof(BAccount.acctName))]
		[PXUIField(DisplayName = "Business Account", Visibility = PXUIVisibility.Visible)]
		public virtual Int32? ReferenceID
		{
			get
			{
				return this._ReferenceID;
			}
			set
			{
				this._ReferenceID = value;
			}
		}
		#endregion

	}

}
