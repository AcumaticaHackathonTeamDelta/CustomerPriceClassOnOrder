using PX.Data;
using PX.Data.EP;
using PX.Objects.AP;
using PX.Objects.AP.Standalone;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;
using System;
using APPayment = PX.Objects.AP.APPayment;
using ARCashSale = PX.Objects.AR.Standalone.ARCashSale;

namespace PX.Objects.CA
{	
	[System.SerializableAttribute()]
	[PXPrimaryGraph(new Type[] {
						typeof(CATranEntry),
						typeof(CashTransferEntry),
						typeof(CADepositEntry),
						typeof(AR.ARCashSaleEntry),
						typeof(AR.ARPaymentEntry),
						typeof(AP.APQuickCheckEntry),
						typeof(AP.APPaymentEntry),
						typeof(CABatchEntry),
						typeof(JournalEntry)
						},
					new Type[] {
						typeof(Select<CAAdj, Where<CAAdj.tranID, Equal<Current<CATran.tranID>>>>),
						typeof(Select<CATransfer, Where<CATransfer.tranIDIn, Equal<Current<CATran.tranID>>,
								Or<CATransfer.tranIDOut, Equal<Current<CATran.tranID>>>>>),
						typeof(Select<CADeposit, Where<CADeposit.tranType, Equal<Current<CATran.origTranType>>,
								And<CADeposit.refNbr, Equal<Current<CATran.origRefNbr>>>>>),
						typeof(Select<ARCashSale, Where<ARCashSale.docType, Equal<Current<CATran.origTranType>>,
							And<ARCashSale.refNbr,Equal<Current<CATran.origRefNbr>>>>>),
						typeof(Select<ARPayment,Where<ARPayment.docType, Equal<Current<CATran.origTranType>>,
							And<ARPayment.refNbr, Equal<Current<CATran.origRefNbr>>,
							And<Current<CATran.origModule>, Equal<BatchModule.moduleAR>>>>>),
						typeof(Select<APQuickCheck, Where<APQuickCheck.docType, Equal<Current<CATran.origTranType>>,
							And<APQuickCheck.refNbr, Equal<Current<CATran.origRefNbr>>>>>),
						typeof(Select<APPayment, Where<APPayment.docType, Equal<Current<CATran.origTranType>>,
							And<APPayment.refNbr, Equal<Current<CATran.origRefNbr>>,
							And<Current<CATran.origModule>, Equal<BatchModule.moduleAP>>>>>),
						typeof(Select<CABatch, Where<CABatch.batchNbr, Equal<Current<CATran.origRefNbr>>,
							And<Current<CATran.origModule>, Equal<BatchModule.moduleAP>, And<Current<CATran.origTranType>, Equal<CATranType.cABatch>>>>>),
						typeof(Select<Batch, Where<Batch.module, Equal<Current<CATran.origModule>>,
							And<Batch.batchNbr, Equal<Current<CATran.origRefNbr>>>>>)
					})]
	[PXCacheName(Messages.CATran)]
	public partial class CATran : PX.Data.IBqlTable
	{
		#region Selected
		public abstract class selected : PX.Data.IBqlField
		{
		}
		protected Boolean? _Selected;
		[PXDBBool()]
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

		#region BegBal
		public abstract class begBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _BegBal;
		[PXCury(typeof(CATran.curyID))]
		[PXUIField(Visible = false)]
		public virtual Decimal? BegBal
		{
			get
			{
				return this._BegBal;
			}
			set
			{
				this._BegBal = value;
			}
		}
		#endregion
		#region EndBal
		public abstract class endBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _EndBal;
		[PXCury(typeof(CATran.curyID))]
		[PXUIField(DisplayName = "Ending Balance", Enabled = false)]
		public virtual Decimal? EndBal
		{
			get
			{
				return this._EndBal;
			}
			set
			{
				this._EndBal = value;
			}
		}
		#endregion
		#region DayDesc
		public abstract class dayDesc : PX.Data.IBqlField
		{
		}
		protected String _DayDesc;
		[PXString(20, IsUnicode = true)]
		[PXUIField(DisplayName = "Day of Week", Enabled = false)]
		public virtual String DayDesc
		{
			get
			{
				return this._DayDesc;
			}
			set
			{
				this._DayDesc = value;
			}
		}
		#endregion
		#region OrigModule
		public abstract class origModule : PX.Data.IBqlField
		{
		}
		protected String _OrigModule;
		[PXDBString(2, IsFixed = true)]
		[PXDefault()]
        [BatchModule.List()]
		[PXUIField(DisplayName = "Module")]
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
		[PXDBString(3, IsFixed = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Tran. Type")]
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
		#region OrigRefNbr
		public abstract class origRefNbr : PX.Data.IBqlField
		{
		}
		protected String _OrigRefNbr;
		[PXDBString(15, IsUnicode = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Orig. Doc. Number")]
		public virtual String OrigRefNbr
		{
			get
			{
				return this._OrigRefNbr;
			}
			set
			{
				this._OrigRefNbr = value;
			}
		}
		#endregion
		#region ExtRefNbr
		public abstract class extRefNbr : PX.Data.IBqlField
		{
		}
		protected String _ExtRefNbr;
		[PXDBString(40, IsUnicode = true)]
		[PXUIField(DisplayName="Document Ref.", Visibility=PXUIVisibility.Visible)]
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
		#region TranID
		public abstract class tranID : PX.Data.IBqlField
		{
		}
		protected Int64? _TranID;
		[PXDBLongIdentity(IsKey = true)]
		[PXUIField(DisplayName = "Document Number")]
		[PXVerifySelector(typeof(Search<CATran.tranID, Where<CATran.cashAccountID, Equal<Current<CARecon.cashAccountID>>, And<Where<CATran.reconNbr, IsNull, Or<CATran.reconNbr, Equal<Current<CARecon.reconNbr>>>>>>>),
			typeof(CATran.extRefNbr),
			typeof(CATran.tranDate),
			typeof(CATran.origModule),
			typeof(CATran.origTranType),
			typeof(CATran.origRefNbr),
			typeof(CATran.status),
			typeof(CATran.curyDebitAmt),
			typeof(CATran.curyCreditAmt),
			typeof(CATran.tranDesc),
			typeof(CATran.cleared),
			typeof(CATran.clearDate), 
			VerifyField = false, DescriptionField = typeof(CATran.extRefNbr))]
		public virtual Int64? TranID
		{
			get
			{
				return this._TranID;
			}
			set
			{
				this._TranID = value;
			}
		}
		#endregion
		#region TranDate
		public abstract class tranDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _TranDate;
		[PXDBDate()]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Doc. Date")]
		[CADailyAccumulator]
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
		#region DrCr
		public abstract class drCr : PX.Data.IBqlField
		{
		}
		protected String _DrCr;
		[PXDefault]
		[PXDBString(1, IsFixed = true)]
		[CADrCr.List()]
		[PXUIField(DisplayName = "Disb. / Receipt")]
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
		[PXDBInt()]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXSelector(typeof(BAccountR.bAccountID),
						SubstituteKey = typeof(BAccountR.acctCD),
					 DescriptionField = typeof(BAccountR.acctName))]
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
		#region ReferenceName
		public abstract class referenceName : PX.Data.IBqlField
		{
		}
		protected String _ReferenceName;
		[PXUIField(DisplayName = "Business Name", Visibility = PXUIVisibility.Visible)]
		[PXString(60, IsUnicode = true)]
		public virtual String ReferenceName
		{
			get
			{
				return this._ReferenceName;
			}
			set
			{
				this._ReferenceName = value;
			}
		}
		#endregion
		#region TranDesc
		public abstract class tranDesc : PX.Data.IBqlField
		{
		}
		protected String _TranDesc;
		[PXDBString(256, IsUnicode = true)]
		[PXUIField(DisplayName="Description", Visibility=PXUIVisibility.Visible)]
		[PXFieldDescription]
		public virtual String TranDesc
		{
			get
			{
				return this._TranDesc;
			}
			set
			{
				this._TranDesc = value;
			}
		}
		#endregion
		#region TranPeriodID
		public abstract class tranPeriodID : PX.Data.IBqlField
		{
		}
		protected String _TranPeriodID;
		[TranPeriodID(typeof(CATran.tranDate))]
		public virtual String TranPeriodID
		{
			get
			{
				return this._TranPeriodID;
			}
			set
			{
				this._TranPeriodID = value;
			}
		}
		#endregion
		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected String _FinPeriodID;
		[FinPeriodSelector(typeof(CATran.tranDate))]
		[PXDefault]
		[PXUIField(DisplayName = "Post Period")]
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
		#region CuryInfoID
		public abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _CuryInfoID;
		[PXDBLong()]
		[CurrencyInfo()]
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
		#region Hold
		public abstract class hold : PX.Data.IBqlField
		{
		}
		protected Boolean? _Hold;
		[PXDBBool()]
		[PXDefault(typeof(Search<CASetup.holdEntry>))]
		public virtual Boolean? Hold
		{
			get
			{
				return this._Hold;
			}
			set
			{
				this._Hold = value;
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
		public virtual Boolean? Released
		{
			get
			{
				return this._Released;
			}
			set
			{
				this._Released = value;
			}
		}
		#endregion
		#region Posted
		public abstract class posted : PX.Data.IBqlField
		{
		}
		protected Boolean? _Posted;
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? Posted
		{
			get
			{
				return this._Posted;
			}
			set
			{
				this._Posted = value;
			}
		}
		#endregion
		#region Status
		public abstract class status : PX.Data.IBqlField
		{ 
		}
		[PXString(1,IsFixed=true)]
		[PXUIField(DisplayName="Status", Enabled=false)]
		[GL.BatchStatus.List()]
		public virtual string Status
		{
			[PXDependsOnFields(typeof(posted),typeof(released),typeof(hold))]
			get
			{
				if (this._Posted == true)
				{
					if (this._Released == true)
						return GL.BatchStatus.Posted;
					else
						return GL.BatchStatus.Unposted;
				}
				else if (this._Released == true && this._Posted != true)
				{
					return GL.BatchStatus.Released;
				}
				else if (this._Hold == true)
				{
					return GL.BatchStatus.Hold;
				}
				else
				{
					return GL.BatchStatus.Balanced;
				}
			}
			set
			{ 
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
		#region ReconDate
		public abstract class reconDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ReconDate;
		[PXDBDate()]
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
		#region ReconNbr
		public abstract class reconNbr : PX.Data.IBqlField
		{
		}
		protected String _ReconNbr;
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Reconciled Number", Enabled = false)]
		[PXParent(typeof(Select<CARecon, Where<CARecon.reconNbr,Equal<Current<CATran.reconNbr>>>>), UseCurrent = true, LeaveChildren=true)]
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
		#region CuryTranAmt
		public abstract class curyTranAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryTranAmt;
		[PXDBCurrency(typeof(CATran.curyInfoID), typeof(CATran.tranAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName="Amount", Visibility=PXUIVisibility.Visible)]
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
		#region TranAmt
		public abstract class tranAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranAmt;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Tran. Amount")]
		public virtual Decimal? TranAmt
		{
			get
			{
				return this._TranAmt;
			}
			set
			{
				this._TranAmt = value;
			}
		}
		#endregion
		#region BatchNbr
		public abstract class batchNbr : PX.Data.IBqlField
		{
		}
		protected String _BatchNbr;
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Batch Number")]
		public virtual String BatchNbr
		{
			get
			{
				return this._BatchNbr;
			}
			set
			{
				this._BatchNbr = value;
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
		[PXDefault(typeof(Search<CashAccount.curyID, Where<CashAccount.cashAccountID, Equal<Current<CATran.cashAccountID>>>>))]
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
		#region Cleared
		public abstract class cleared : PX.Data.IBqlField
		{
		}
		protected Boolean? _Cleared;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName="Cleared")]
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
		#region ClearDate
		public abstract class clearDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ClearDate;
		[PXDBDate]
		[PXUIField(DisplayName = "Clear Date")]
		public virtual DateTime? ClearDate
		{
			get
			{
				return this._ClearDate;
			}
			set
			{
				this._ClearDate = value;
			}
		}
		#endregion
		#region CuryDebitAmt
		public abstract class curyDebitAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryDebitAmt;
		[PXDecimal()]
		[PXUIField(DisplayName = "Receipt")]
		public virtual Decimal? CuryDebitAmt
		{
			[PXDependsOnFields(typeof(drCr),typeof(curyTranAmt))]
			get
			{
				if (this._DrCr == null)
				{
					return this._CuryDebitAmt;
				}
				else
				{
					return (this._DrCr == CADrCr.CADebit) ? this._CuryTranAmt : 0m;
				}
			}
			set
			{
				this._CuryDebitAmt = value;
			}
		}
		#endregion
		#region CuryCreditAmt
		public abstract class curyCreditAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryCreditAmt;
		[PXDecimal()]
		[PXUIField(DisplayName = "Disbursement")]
		public virtual Decimal? CuryCreditAmt
		{
			[PXDependsOnFields(typeof(drCr),typeof(curyTranAmt))]
			get
			{
				if (this._DrCr == null)
				{
					return this._CuryCreditAmt;
				}
				else
				{
					return (this._DrCr == CADrCr.CACredit) ? -this._CuryTranAmt : 0m;
				}
			}
			set
			{
				this._CuryCreditAmt = value;
			}
		}
		#endregion
		#region CuryClearedDebitAmt
		public abstract class curyClearedDebitAmt : PX.Data.IBqlField
		{
		}
		[PXDecimal()]
		[PXUIField(DisplayName = "Receipt")]
		public virtual Decimal? CuryClearedDebitAmt
		{
			[PXDependsOnFields(typeof(cleared),typeof(drCr),typeof(curyTranAmt))]
			get
			{
				return (this._Cleared == true && this._DrCr == CADrCr.CADebit) ? this._CuryTranAmt : 0m;
			}
			set
			{
			}
		}
		#endregion
		#region CuryClearedCreditAmt
		public abstract class curyClearedCreditAmt : PX.Data.IBqlField
		{
		}
		[PXDecimal()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Disbursement")]
		public virtual Decimal? CuryClearedCreditAmt
		{
			[PXDependsOnFields(typeof(cleared),typeof(drCr),typeof(curyTranAmt))]
			get
			{
				return (this._Cleared == true && this._DrCr == CADrCr.CACredit) ? - this._CuryTranAmt : 0m;
			}
			set
			{
			}
		}
		#endregion
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXNote(DescriptionField = typeof(CATran.tranID))]
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
		#region RefTranAccountID
		public abstract class refTranAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _RefTranAccountID;	
		[GL.CashAccount(DisplayName = "ChildTran Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(CashAccount.descr))]
		public virtual Int32? RefTranAccountID
		{
			get
			{
				return this._RefTranAccountID;
			}
			set
			{
				this._RefTranAccountID = value;
			}
		}
		#endregion
		#region RefTranID
		public abstract class refTranID : PX.Data.IBqlField
		{
		}
		protected Int64? _RefTranID;
		[PXDBLong()]		
		public virtual Int64? RefTranID
		{
			get
			{
				return this._RefTranID;
			}
			set
			{
				this._RefTranID = value;
			}
		}
		#endregion
        #region RefSplitLineNbr
        public abstract class refSplitLineNbr : PX.Data.IBqlField
        {
        }
        protected Int32? _RefSplitLineNbr;
        [PXDBInt()]
        public virtual Int32? RefSplitLineNbr
        {
            get
            {
                return this._RefSplitLineNbr;
            }
            set
            {
                this._RefSplitLineNbr = value;
            }
        }
        #endregion
        #region VoidedTranID
        public abstract class voidedTranID : PX.Data.IBqlField
        {
        }
        protected Int64? _VoidedTranID;
        [PXDBLong()]
        public virtual Int64? VoidedTranID
        {
            get
            {
                return this._VoidedTranID;
            }
            set
            {
                this._VoidedTranID = value;
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
        #region Methods

        public static void Redirect(PXCache sender, CATran catran)
		{
			if (catran == null)
			{
				return;
			}
			if (catran.OrigTranType == CAAPARTranType.GLEntry)
			{
				JournalEntry graph = PXGraph.CreateInstance<JournalEntry>();
				graph.Clear();
				if (sender != null)
					sender.IsDirty = false;
				graph.BatchModule.Current = PXSelect<Batch, Where<Batch.module, Equal<Required<Batch.module>>, And<Batch.batchNbr, Equal<Required<Batch.batchNbr>>>>>.Select(graph,	catran.OrigModule, catran.OrigRefNbr);
				throw new PXRedirectRequiredException(graph, true, "Document") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
			}
			else if (catran.OrigModule == GL.BatchModule.AP && (catran.OrigTranType == AP.APDocType.QuickCheck || catran.OrigTranType == AP.APDocType.VoidQuickCheck))
			{
				APQuickCheckEntry graph = PXGraph.CreateInstance<APQuickCheckEntry>();
				graph.Clear();
				if (sender != null)
					sender.IsDirty = false;
				graph.Document.Current = PXSelect<APQuickCheck, Where<APQuickCheck.refNbr, Equal<Required<APQuickCheck.refNbr>>,
					And<APQuickCheck.docType, Equal<Required<APQuickCheck.docType>>>>>.Select(graph, catran.OrigRefNbr, catran.OrigTranType);
				throw new PXRedirectRequiredException(graph, true, "Document") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
			}
			else if (catran.OrigModule == GL.BatchModule.AP && catran.OrigTranType == CATranType.CABatch)
			{
				CABatchEntry graph = PXGraph.CreateInstance<CABatchEntry>();
				graph.Clear();
				if (sender != null)
					sender.IsDirty = false;
				graph.Document.Current = PXSelect<CABatch, Where<CABatch.batchNbr, Equal<Required<CATran.origRefNbr>>>>.Select(graph, catran.OrigRefNbr);
				throw new PXRedirectRequiredException(graph, true, "Document") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
			}
			else if (catran.OrigModule == GL.BatchModule.AP)
			{
				APPaymentEntry graph = PXGraph.CreateInstance<APPaymentEntry>();
				graph.Clear();
				if (sender != null)
					sender.IsDirty = false;
				graph.Document.Current = (APPayment)PXSelect<APPayment, Where<APPayment.refNbr, Equal<Required<APPayment.refNbr>>,
					And<APPayment.docType, Equal<Required<APPayment.docType>>>>>.Select(graph, catran.OrigRefNbr, catran.OrigTranType);
				throw new PXRedirectRequiredException(graph, true, "Document") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
			}
            else if (catran.OrigModule == GL.BatchModule.AR && (catran.OrigTranType == ARDocType.CashSale || catran.OrigTranType == ARDocType.CashReturn))
            {
				ARCashSale document=(ARCashSale)PXSelect<ARCashSale, Where<ARCashSale.refNbr, Equal<Required<ARCashSale.refNbr>>,
				   And<ARCashSale.docType, Equal<Required<ARCashSale.docType>>>>>.Select(sender.Graph, catran.OrigRefNbr, catran.OrigTranType);
				if (document.OrigModule == BatchModule.SO && document.Released == false)
				{
					SO.SOInvoiceEntry graph = PXGraph.CreateInstance<SO.SOInvoiceEntry>();
					graph.Clear();
					sender.IsDirty = false;

					graph.Document.Current = (ARInvoice)PXSelect<ARInvoice, Where<ARInvoice.refNbr, Equal<Required<ARInvoice.refNbr>>,
				   And<ARInvoice.docType, Equal<Required<ARInvoice.docType>>>>>.Select(sender.Graph, catran.OrigRefNbr, catran.OrigTranType);
					
					throw new PXRedirectRequiredException(graph, true, "Document") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
				}
				else
				{
                ARCashSaleEntry graph = PXGraph.CreateInstance<ARCashSaleEntry>();
                graph.Clear();
				if (sender != null)
					sender.IsDirty = false;

					graph.Document.Current = document;
                throw new PXRedirectRequiredException(graph, true, "Document") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
            }
			}
			else if (catran.OrigModule == GL.BatchModule.AR)
			{
				ARPaymentEntry graph = PXGraph.CreateInstance<ARPaymentEntry>();
				graph.Clear();
				if (sender != null)
					sender.IsDirty = false;
				graph.Document.Current = (ARPayment)PXSelect<ARPayment, Where<ARPayment.refNbr, Equal<Required<ARPayment.refNbr>>,
					And<ARPayment.docType, Equal<Required<ARPayment.docType>>>>>.Select(graph, catran.OrigRefNbr, catran.OrigTranType);
				throw new PXRedirectRequiredException(graph, true, "Document") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
			}
			else if (catran.OrigModule == GL.BatchModule.CA && catran.OrigTranType == CATranType.CAAdjustment)
			{
				CATranEntry graph = PXGraph.CreateInstance<CATranEntry>();
				graph.Clear();
				if (sender != null)
					sender.IsDirty = false;
				graph.CAAdjRecords.Current = PXSelect<CAAdj, Where<CAAdj.tranID, Equal<Required<CAAdj.tranID>>>>.Select(graph, catran.TranID);
				throw new PXRedirectRequiredException(graph, true, "Document") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
			}
			else if (catran.OrigModule == GL.BatchModule.CA && catran.OrigTranType == CATranType.CATransferExp)
			{
				CATranEntry graph = PXGraph.CreateInstance<CATranEntry>();
				graph.Clear();
				if (sender != null)
					sender.IsDirty = false;
				graph.CAAdjRecords.Current = PXSelect<CAAdj, Where<CAAdj.adjTranType, Equal<CAAPARTranType.cATransferExp>, And<CAAdj.transferNbr, Equal<Required<CATran.origRefNbr>>>>>.Select(graph, catran.OrigRefNbr);
				throw new PXRedirectRequiredException(graph, true, "Document") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
			}
			else if (catran.OrigModule == GL.BatchModule.CA)
			{
				if (catran.OrigTranType == CATranType.CADeposit || catran.OrigTranType == CATranType.CAVoidDeposit)
				{
					CADepositEntry graph = PXGraph.CreateInstance<CADepositEntry>();
					graph.Clear();
					if (sender != null)
						sender.IsDirty = false;
					graph.Document.Current = PXSelect<CADeposit, Where<CADeposit.tranType, Equal<Required<CADeposit.tranType>>,
											And<CADeposit.refNbr, Equal<Required<CADeposit.refNbr>>>>>
														.Select(graph, catran.OrigTranType, catran.OrigRefNbr);
					throw new PXRedirectRequiredException(graph, true, "Document") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
				}
				else
				{
					CashTransferEntry graph = PXGraph.CreateInstance<CashTransferEntry>();
					graph.Clear();
					if (sender != null)
						sender.IsDirty = false;
					graph.Transfer.Current = PXSelect<CATransfer, Where<CATransfer.tranIDOut, Equal<Required<CATransfer.tranIDOut>>, Or<CATransfer.tranIDIn, Equal<Required<CATransfer.tranIDIn>>>>>
														.Select(graph, catran.TranID, catran.TranID);
					throw new PXRedirectRequiredException(graph, true, "Document") { Mode = PXBaseRedirectException.WindowMode.NewWindow };

				}
			}
		}
		#endregion
	}
}
