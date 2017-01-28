namespace PX.Objects.CA
{
	using System;
	using PX.Data;
	using PX.Objects.CS;
	using PX.Objects.GL;
	using PX.Objects.CM;
	using PX.Objects.EP;
	
	[System.SerializableAttribute()]
    [PXPrimaryGraph(typeof(CASetupMaint))]
    [PXCacheName(Messages.CASetup)]
    public partial class CASetup : PX.Data.IBqlTable
	{
		#region BatchNumberingID
		public abstract class batchNumberingID : PX.Data.IBqlField
		{
		}
		protected String _BatchNumberingID;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault("BATCH")]
		[PXSelector(typeof(Numbering.numberingID),
					 DescriptionField = typeof(Numbering.descr))]
		[PXUIField(DisplayName = "Batch Numbering Sequence")]
		public virtual String BatchNumberingID
		{
			get
			{
				return this._BatchNumberingID;
			}
			set
			{
				this._BatchNumberingID = value;
			}
		}
		#endregion
		#region RegisterNumberingID
		public abstract class registerNumberingID : PX.Data.IBqlField
		{
		}
		protected String _RegisterNumberingID;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault("CATRAN")]
		[PXSelector(typeof(Numbering.numberingID),
					 DescriptionField = typeof(Numbering.descr))]
		[PXUIField(DisplayName = "Transaction Numbering Sequence")]
		public virtual String RegisterNumberingID
		{
			get
			{
				return this._RegisterNumberingID;
			}
			set
			{
				this._RegisterNumberingID = value;
			}
		}
		#endregion
		#region TransferNumberingID
		public abstract class transferNumberingID : PX.Data.IBqlField
		{
		}
		protected String _TransferNumberingID;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault("CATRANSFER")]
		[PXSelector(typeof(Numbering.numberingID),
					 DescriptionField = typeof(Numbering.descr))]
		[PXUIField(DisplayName = "Transfer Numbering Sequence")]
		public virtual String TransferNumberingID
		{
			get
			{
				return this._TransferNumberingID;
			}
			set
			{
				this._TransferNumberingID = value;
			}
		}
		#endregion
		#region CABatchNumberingID
		public abstract class cABatchNumberingID : PX.Data.IBqlField
		{
		}
		protected String _CABatchNumberingID;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault("CABATCH")]
		[PXSelector(typeof(Numbering.numberingID),
					 DescriptionField = typeof(Numbering.descr))]
		[PXUIField(DisplayName = "Payment Batch Numbering Sequence")]
		public virtual String CABatchNumberingID
		{
			get
			{
				return this._CABatchNumberingID;
			}
			set
			{
				this._CABatchNumberingID = value;
			}
		}
		#endregion
		#region CAStatementNumberingID
		public abstract class cAStatementNumberingID : PX.Data.IBqlField
		{
		}
		protected String _CAStatementNumberingID;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault("CABANKSTMT")]
		[PXSelector(typeof(Numbering.numberingID),
					 DescriptionField = typeof(Numbering.descr))]
		[PXUIField(DisplayName = "Bank Statement Numbering Sequence")]
		public virtual String CAStatementNumberingID
		{
			get
			{
				return this._CAStatementNumberingID;
			}
			set
			{
				this._CAStatementNumberingID = value;
			}
		}
		#endregion
		#region CAImportPaymentsNumberingID
		public abstract class cAImportPaymentsNumberingID : PX.Data.IBqlField
		{
		}
		protected String _CAImportPaymentsNumberingID;
		[PXDBString(10, IsUnicode = true)]
		[PXSelector(typeof(Numbering.numberingID),
					 DescriptionField = typeof(Numbering.descr))]
		[PXUIField(DisplayName = "Payments Import Numbering Sequence")]
		public virtual String CAImportPaymentsNumberingID
		{
			get
			{
				return this._CAImportPaymentsNumberingID;
			}
			set
			{
				this._CAImportPaymentsNumberingID = value;
			}
		}
		#endregion
#if PTInstStatement
		#region PTInstStmtNumbering
		public abstract class pTInstStmtNumbering : PX.Data.IBqlField
		{
		}
		protected String _PTInstStmtNumbering;
		[PXDBString(10)]
		[PXDefault("PTINSTSTMT")]
		[PXSelector(typeof(Numbering.numberingID),
					 DescriptionField = typeof(Numbering.descr))]
		[PXUIField(DisplayName = "Corp. Card Statements Sequence")]
		public virtual String PTInstStmtNumbering
		{
			get
			{
				return this._PTInstStmtNumbering;
			}
			set
			{
				this._PTInstStmtNumbering = value;
			}
		}
		#endregion
#endif

		#region TransitAcctId
		public abstract class transitAcctId : PX.Data.IBqlField
		{
		}
		protected Int32? _TransitAcctId;
		[PXDefault()]
		[PXNonCashAccount(DisplayName = "Cash-In-Transit Account",
		   DescriptionField = typeof(Account.description))]
		public virtual Int32? TransitAcctId
		{
			get
			{
				return this._TransitAcctId;
			}
			set
			{
				this._TransitAcctId = value;
			}
		}
		#endregion
		#region TransitSubID
		public abstract class transitSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _TransitSubID;
		[PXDefault()]
		[SubAccount(typeof(CASetup.transitAcctId), DisplayName = "Cash-In-Transit Subaccount", DescriptionField = typeof(Sub.description))]
		public virtual Int32? TransitSubID
		{
			get
			{
				return this._TransitSubID;
			}
			set
			{
				this._TransitSubID = value;
			}
		}
		#endregion
		#region RequireControlTotal
		public abstract class requireControlTotal : PX.Data.IBqlField
		{
		}
		protected Boolean? _RequireControlTotal;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Validate Control Totals on Entry")]
		public virtual Boolean? RequireControlTotal
		{
			get
			{
				return this._RequireControlTotal;
			}
			set
			{
				this._RequireControlTotal = value;
			}
		}
		#endregion
		#region RequireControlTaxTotal
		public abstract class requireControlTaxTotal : IBqlField { }
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Validate Tax Totals on Entry")]
		public virtual bool? RequireControlTaxTotal { get; set; }
		#endregion
		#region HoldEntry
		public abstract class holdEntry : PX.Data.IBqlField
		{
		}
		protected Boolean? _HoldEntry;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Hold Transactions on Entry")]
		public virtual Boolean? HoldEntry
		{
			get
			{
				return this._HoldEntry;
			}
			set
			{
				this._HoldEntry = value;
			}
		}
		#endregion
		#region CuryRateTypeID
		public abstract class curyRateTypeID : PX.Data.IBqlField
		{
		}
		protected String _CuryRateTypeID;
		[PXDBString(6, IsUnicode = true)]
		[PXSelector(typeof(CurrencyRateType.curyRateTypeID), DescriptionField = typeof(CurrencyRateType.descr))]
		[PXUIField(DisplayName = "Default Rate Type")]
		public virtual String CuryRateTypeID
		{
			get
			{
				return this._CuryRateTypeID;
			}
			set
			{
				this._CuryRateTypeID = value;
			}
		}
		#endregion
		#region ReleaseAP
		public abstract class releaseAP : PX.Data.IBqlField
		{
		}
		protected Boolean? _ReleaseAP;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Release AP Documents from CA Module")]
		public virtual Boolean? ReleaseAP
		{
			get
			{
				return this._ReleaseAP;
			}
			set
			{
				this._ReleaseAP = value;
			}
		}
		#endregion
		#region ReleaseAR
		public abstract class releaseAR : PX.Data.IBqlField
		{
		}
		protected Boolean? _ReleaseAR;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Release AR Documents from CA Module")]
		public virtual Boolean? ReleaseAR
		{
			get
			{
				return this._ReleaseAR;
			}
			set
			{
				this._ReleaseAR = value;
			}
		}
		#endregion
		#region CalcBalDebitUnclearedUnreleased
		public abstract class calcBalDebitUnclearedUnreleased : PX.Data.IBqlField
		{
		}
		protected Boolean? _CalcBalDebitUnclearedUnreleased;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Unreleased Uncleared")]
		public virtual Boolean? CalcBalDebitUnclearedUnreleased
		{
			get
			{
				return this._CalcBalDebitUnclearedUnreleased;
			}
			set
			{
				this._CalcBalDebitUnclearedUnreleased = value;
			}
		}
		#endregion
		#region CalcBalDebitClearedUnreleased
		public abstract class calcBalDebitClearedUnreleased : PX.Data.IBqlField
		{
		}
		protected Boolean? _CalcBalDebitClearedUnreleased;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Unreleased Cleared")]
		public virtual Boolean? CalcBalDebitClearedUnreleased
		{
			get
			{
				return this._CalcBalDebitClearedUnreleased;
			}
			set
			{
				this._CalcBalDebitClearedUnreleased = value;
			}
		}
		#endregion
		#region CalcBalDebitUnclearedReleased
		public abstract class calcBalDebitUnclearedReleased : PX.Data.IBqlField
		{
		}
		protected Boolean? _CalcBalDebitUnclearedReleased;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Released Uncleared")]
		public virtual Boolean? CalcBalDebitUnclearedReleased
		{
			get
			{
				return this._CalcBalDebitUnclearedReleased;
			}
			set
			{
				this._CalcBalDebitUnclearedReleased = value;
			}
		}
		#endregion
		#region CalcBalCreditUnclearedUnreleased
		public abstract class calcBalCreditUnclearedUnreleased : PX.Data.IBqlField
		{
		}
		protected Boolean? _CalcBalCreditUnclearedUnreleased;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Unreleased Uncleared")]
		public virtual Boolean? CalcBalCreditUnclearedUnreleased
		{
			get
			{
				return this._CalcBalCreditUnclearedUnreleased;
			}
			set
			{
				this._CalcBalCreditUnclearedUnreleased = value;
			}
		}
		#endregion
		#region CalcBalCreditClearedUnreleased
		public abstract class calcBalCreditClearedUnreleased : PX.Data.IBqlField
		{
		}
		protected Boolean? _CalcBalCreditClearedUnreleased;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Unreleased Cleared")]
		public virtual Boolean? CalcBalCreditClearedUnreleased
		{
			get
			{
				return this._CalcBalCreditClearedUnreleased;
			}
			set
			{
				this._CalcBalCreditClearedUnreleased = value;
			}
		}
		#endregion
		#region CalcBalCreditUnclearedReleased
		public abstract class calcBalCreditUnclearedReleased : PX.Data.IBqlField
		{
		}
		protected Boolean? _CalcBalCreditUnclearedReleased;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Released Uncleared")]
		public virtual Boolean? CalcBalCreditUnclearedReleased
		{
			get
			{
				return this._CalcBalCreditUnclearedReleased;
			}
			set
			{
				this._CalcBalCreditUnclearedReleased = value;
			}
		}
		#endregion
		#region AutoPostOption
		public abstract class autoPostOption : PX.Data.IBqlField
		{
		}
		protected Boolean? _AutoPostOption;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Automatically Post to GL on Release", Visibility = PXUIVisibility.Visible)]
		public virtual Boolean? AutoPostOption
		{
			get
			{
				return this._AutoPostOption;
			}
			set
			{
				this._AutoPostOption = value;
			}
		}
		#endregion
		#region DateRangeDefault
		public abstract class dateRangeDefault : PX.Data.IBqlField
		{
		}
		protected String _DateRangeDefault;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(CADateRange.Week)]
		[PXUIField(DisplayName = "Default Date Range")]
		[CADateRange.List()]
		public virtual String DateRangeDefault
		{
			get
			{
				return this._DateRangeDefault;
			}
			set
			{
				this._DateRangeDefault = value;
			}
		}
		#endregion		
		#region RequestApproval
		public abstract class requestApproval : PX.Data.IBqlField
		{
		}
		protected Boolean? _RequestApproval;
		[EPRequireApproval]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Require Approval")]
		public virtual Boolean? RequestApproval
		{
			get
			{
				return this._RequestApproval;
			}
			set
			{
				this._RequestApproval = value;
			}
		}
		#endregion

		#region ReceiptTranDaysBefore
		public abstract class receiptTranDaysBefore : PX.Data.IBqlField
		{
		}
		protected Int32? _ReceiptTranDaysBefore;
		[PXDBInt(MinValue = 0, MaxValue = 365)]
		[PXDefault(5, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Days before bank transaction date")]
		public virtual Int32? ReceiptTranDaysBefore
		{
			get
			{
				return this._ReceiptTranDaysBefore;
			}
			set
			{
				this._ReceiptTranDaysBefore = value;
			}
		}
		#endregion
		#region ReceiptTranDaysAfter
		public abstract class receiptTranDaysAfter : PX.Data.IBqlField
		{
		}
		protected Int32? _ReceiptTranDaysAfter;
		[PXDBInt(MinValue = 0, MaxValue = 365)]
		[PXDefault(5, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Days after bank transaction date")]
		public virtual Int32? ReceiptTranDaysAfter
		{
			get
			{
				return this._ReceiptTranDaysAfter;
			}
			set
			{
				this._ReceiptTranDaysAfter = value;
			}
		}
		#endregion
		#region DisbursementTranDaysBefore
		public abstract class disbursementTranDaysBefore : PX.Data.IBqlField
		{
		}
		protected Int32? _DisbursementTranDaysBefore;
		[PXDBInt(MinValue = 0, MaxValue = 365)]
		[PXDefault(5, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Days before bank transaction date")]
		public virtual Int32? DisbursementTranDaysBefore
		{
			get
			{
				return this._DisbursementTranDaysBefore;
			}
			set
			{
				this._DisbursementTranDaysBefore = value;
			}
		}
		#endregion
		#region DisbursementTranDaysAfter
		public abstract class disbursementTranDaysAfter : PX.Data.IBqlField
		{
		}
		protected Int32? _DisbursementTranDaysAfter;
		[PXDBInt(MinValue = 0, MaxValue = 365)]
		[PXDefault(5, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Days after bank transaction date")]
		public virtual Int32? DisbursementTranDaysAfter
		{
			get
			{
				return this._DisbursementTranDaysAfter;
			}
			set
			{
				this._DisbursementTranDaysAfter = value;
			}
		}
		#endregion
		#region RefNbrCompareWeight
		public abstract class refNbrCompareWeight : PX.Data.IBqlField
		{
		}
		protected Decimal? _RefNbrCompareWeight;
		[PXDBDecimal(MinValue = 0, MaxValue = 100.0)]
		[PXDefault(TypeCode.Decimal, "70.0",	
			PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Ref. Nbr. Weight")]
		public virtual Decimal? RefNbrCompareWeight
		{
			get
			{
				return this._RefNbrCompareWeight;
			}
			set
			{
				this._RefNbrCompareWeight = value;
			}
		}
		#endregion
		#region DateCompareWeight
		public abstract class dateCompareWeight : PX.Data.IBqlField
		{
		}
		protected Decimal? _DateCompareWeight;
		[PXDBDecimal(MinValue = 0, MaxValue = 100)]
		[PXDefault(TypeCode.Decimal, "20.0",			
			PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Doc. Date Weight")]
		public virtual Decimal? DateCompareWeight
		{
			get
			{
				return this._DateCompareWeight;
			}
			set
			{
				this._DateCompareWeight = value;
			}
		}
		#endregion
		#region PayeeCompareWeight
		public abstract class payeeCompareWeight : PX.Data.IBqlField
		{
		}
		protected Decimal? _PayeeCompareWeight;
		[PXDBDecimal(MinValue = 0, MaxValue = 100)]
		[PXDefault(TypeCode.Decimal,"10.0",PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Doc. Payee Weight")]
		public virtual Decimal? PayeeCompareWeight
		{
			get
			{
				return this._PayeeCompareWeight;
			}
			set
			{
				this._PayeeCompareWeight = value;
			}
		}
		#endregion
		#region DateMeanOffset
		public abstract class dateMeanOffset : PX.Data.IBqlField
		{
		}
		protected Decimal? _DateMeanOffset;
		[PXDBDecimal(MinValue = -365, MaxValue = 365)]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Payment Clearing Average Delay")]
		public virtual Decimal? DateMeanOffset
		{
			get
			{
				return this._DateMeanOffset;
			}
			set
			{
				this._DateMeanOffset = value;
			}
		}
		#endregion
		#region DateSigma
		public abstract class dateSigma : PX.Data.IBqlField
		{
		}
		protected Decimal? _DateSigma;
		[PXDBDecimal(MinValue = 0, MaxValue = 365)]
		[PXDefault(TypeCode.Decimal, "5.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Estimated Deviation (days)")]
		public virtual Decimal? DateSigma
		{
			get
			{
				return this._DateSigma;
			}
			set
			{
				this._DateSigma = value;
			}
		}
		#endregion

		protected Decimal TotalWeight
		{
			get
			{
				decimal total = (this._DateCompareWeight ?? Decimal.Zero)
								+ (this.RefNbrCompareWeight ?? Decimal.Zero)
								+ (this.PayeeCompareWeight ?? Decimal.Zero);
				return total;
			}

		}
		#region RefNbrComparePercent
		public abstract class refNbrComparePercent : PX.Data.IBqlField
		{
		}

		[PXDecimal()]
		[PXUIField(DisplayName = "%", Enabled = false)]
		public virtual Decimal? RefNbrComparePercent
		{
			[PXDependsOnFields(typeof(refNbrCompareWeight), typeof(dateCompareWeight), typeof(payeeCompareWeight))] 
			get
			{
				Decimal total = this.TotalWeight;
				return ((total != Decimal.Zero ? (this.RefNbrCompareWeight / total) : Decimal.Zero) * 100.0m);
			}
			set
			{

			}
		}
		#endregion
		#region DateComparePercent
		public abstract class dateComparePercent : PX.Data.IBqlField
		{
		}
		[PXDecimal()]
		[PXUIField(DisplayName = "%", Enabled = false)]
		public virtual Decimal? DateComparePercent
		{
			[PXDependsOnFields(typeof(refNbrCompareWeight), typeof(dateCompareWeight), typeof(payeeCompareWeight))] 
			get
			{
				Decimal total = this.TotalWeight;
				return ((total != Decimal.Zero ? (this.DateCompareWeight / total) : Decimal.Zero) * 100.0m);
			}
			set
			{

			}
		}
		#endregion
		#region PayeeComparePercent
		public abstract class payeeComparePercent : PX.Data.IBqlField
		{
		}

		[PXDecimal()]
		[PXUIField(DisplayName = "%", Enabled = false)]
		public virtual Decimal? PayeeComparePercent
		{
			[PXDependsOnFields(typeof(refNbrCompareWeight), typeof(dateCompareWeight), typeof(payeeCompareWeight))] 
			get
			{
				Decimal total = this.TotalWeight;
				return ((total != Decimal.Zero ? (this.PayeeCompareWeight / total) : Decimal.Zero) * 100.0m);
			}
			set
			{

			}
		}
		#endregion
		#region MatchInSelection
		public abstract class matchInSelection : PX.Data.IBqlField
		{
		}
		protected Boolean? _MatchInSelection;
		[PXDBBool()]
		[PXDefault(false,
				PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Match in the selection only", Visible = false)]
		public virtual Boolean? MatchInSelection
		{
			get
			{
				return this._MatchInSelection;
			}
			set
			{
				this._MatchInSelection = value;
			}
		}
		#endregion
		#region IgnoreCuryCheckOnImport
		public abstract class ignoreCuryCheckOnImport : PX.Data.IBqlField
		{
		}
		protected Boolean? _IgnoreCuryCheckOnImport;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Ignore Currency Check on Bank Statement Import")]
		public virtual Boolean? IgnoreCuryCheckOnImport
		{
			get
			{
				return this._IgnoreCuryCheckOnImport;
			}
			set
			{
				this._IgnoreCuryCheckOnImport = value;
			}
		}
		#endregion
		#region ImportToSingleAccount
		public abstract class importToSingleAccount : PX.Data.IBqlField
		{
		}
		protected Boolean? _ImportToSingleAccount;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Import Bank Statement to single Cash Account")]
		public virtual Boolean? ImportToSingleAccount
		{
			get
			{
				return this._ImportToSingleAccount;
			}
			set
			{
				this._ImportToSingleAccount = value;
			}
		}
		#endregion
		#region AllowEmptyFITID
		public abstract class allowEmptyFITID : PX.Data.IBqlField
		{
		}
		protected Boolean? _AllowEmptyFITID;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Allow Empty FITID")]
		public virtual Boolean? AllowEmptyFITID
		{
			get
			{
				return this._AllowEmptyFITID;
			}
			set
			{
				this._AllowEmptyFITID = value;
			}
		}
		#endregion
		#region AllowMatchingToUnreleasedBatch
		public abstract class allowMatchingToUnreleasedBatch : PX.Data.IBqlField
		{
		}
		protected Boolean? _AllowMatchingToUnreleasedBatch;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Allow Matching to Unreleased Batch Payments")]
		public virtual Boolean? AllowMatchingToUnreleasedBatch
		{
			get
			{
				return this._AllowMatchingToUnreleasedBatch;
			}
			set
			{
				this._AllowMatchingToUnreleasedBatch = value;
			}
		}
		#endregion
		#region UnknownPaymentEntryTypeID
		public abstract class unknownPaymentEntryTypeID : PX.Data.IBqlField
		{
		}
		protected String _UnknownPaymentEntryTypeID;
		[PXDBString(10, IsUnicode = true)]
		[PXSelector(typeof(Search<CAEntryType.entryTypeId,
									Where<CAEntryType.module, Equal<BatchModule.moduleCA>,
									And<CAEntryType.useToReclassifyPayments,Equal<True>>>>), DescriptionField = typeof(CAEntryType.descr))]
		[PXUIField(DisplayName = "Unrecognized Receipts Type", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String UnknownPaymentEntryTypeID
		{
			get
			{
				return this._UnknownPaymentEntryTypeID;
			}
			set
			{
				this._UnknownPaymentEntryTypeID = value;
			}
		}
		#endregion
		#region StatementImportTypeName
		public abstract class statementImportTypeName : PX.Data.IBqlField
		{
		}
		protected String _StatementImportTypeName;
		[PXDBString(255)]
		[PXUIField(DisplayName = "Statement Import Service")]
		[CA.PXProviderTypeSelector(typeof(IStatementReader))]
		public virtual String StatementImportTypeName
		{
			get
			{
				return this._StatementImportTypeName;
			}
			set
			{
				this._StatementImportTypeName = value;
			}
		}
		#endregion
        #region SkipVoided
        public abstract class skipVoided : PX.Data.IBqlField
        {
        }
        protected Boolean? _SkipVoided;
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Skip Voided Transactions")]
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
        #region RequireExtRefNbr
        public abstract class requireExtRefNbr : PX.Data.IBqlField
        {
        }
        protected Boolean? _RequireExtRefNbr;
        [PXDBBool]
        [PXDefault(true)]
        [PXUIField(DisplayName = "Require Document Ref. Nbr. on Entry")]
        public virtual Boolean? RequireExtRefNbr
        {
            get
            {
                return this._RequireExtRefNbr;
            }
            set
            {
                this._RequireExtRefNbr = value;
            }
        }
		#endregion
		#region ValidateDataConsistencyOnRelease
		public abstract class validateDataConsistencyOnRelease : PX.Data.IBqlField
		{
		}
		[PXDBBool]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Validate data consistency on Release", Visible = false)]
		public virtual Boolean ValidateDataConsistencyOnRelease { get; set; }
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
	}

	public class CADateRange
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Day, Week, Month, Period},
				new string[] { Messages.Day, Messages.Week, Messages.Month, Messages.Period }) { }
		}

		public const string Day = "D";
		public const string Week = "W";
		public const string Month = "M";
		public const string Period = "P";
	}

}
