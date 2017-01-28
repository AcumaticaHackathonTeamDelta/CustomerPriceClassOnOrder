
using System;
using PX.Data;
using PX.Data.EP;
using PX.Objects.CM;
using PX.Objects.GL;
using PX.TM;

namespace PX.Objects.CA
{
	[PXCacheName(Messages.CADeposit)]
	[System.SerializableAttribute()]
	public partial class CADeposit : PX.Data.IBqlTable, ICADocument 
	{
		public string DocType
		{
			get
			{
				return this._TranType;
			}
		}
		#region TranType
		public abstract class tranType : PX.Data.IBqlField
		{
		}
		protected String _TranType;

		[PXDBString(3, IsFixed = true,IsKey = true)]
		[CATranType.DepositList()]
		[PXDefault(CATranType.CADeposit)]
		[PXUIField(DisplayName = "Tran. Type", Enabled = true, Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String TranType
		{
			get
			{
				return this._TranType;
			}
			set
			{
				this._TranType = value;
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
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		[CADepositType.Numbering()]
		[CADepositType.RefNbr(typeof(Search<CADeposit.refNbr,Where<CADeposit.tranType,Equal<Current<CADeposit.tranType>>>>))]
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
		[PXDefault()]
		[PXUIField(DisplayName="Document Ref.", Visibility=PXUIVisibility.SelectorVisible)]
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
		[PXDefault]
		[GL.CashAccount(null, typeof(Search<CashAccount.cashAccountID, Where2<Match<Current<AccessInfo.userName>>, And<CashAccount.clearingAccount, Equal<CS.boolFalse>>>>), DisplayName = "Cash Account", Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(CashAccount.descr))]
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
		#region TranDate
		public abstract class tranDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _TranDate;
		[PXDBDate()]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Deposit Date", Visibility = PXUIVisibility.SelectorVisible)]
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
		[PXDefault(CADrCr.CADebit)]
		[PXDBString(1, IsFixed = true)]
		[CADrCr.List()]
		//[PXUIField(DisplayName = "Disb. / Receipt", Enabled = false)]
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
		#region TranDesc
		public abstract class tranDesc : PX.Data.IBqlField
		{
		}
		protected String _TranDesc;
		[PXDBString(256, IsUnicode = true)]
		[PXUIField(DisplayName="Description", Visibility=PXUIVisibility.SelectorVisible)]
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
		[OpenPeriod(typeof(CADeposit.tranDate))]
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
		[OpenPeriod(typeof(CADeposit.tranDate))]
		[PXUIField(DisplayName = "Fin. Period")]
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
		
		#region Hold
		public abstract class hold : PX.Data.IBqlField
		{
		}
		protected Boolean? _Hold;
		[PXDBBool()]
		[PXDefault(typeof(Search<CASetup.holdEntry>))]
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
		#region Voided
		public abstract class voided : PX.Data.IBqlField
		{
		}

		protected Boolean? _Voided;
		[PXDBBool()]
		[PXUIField(DisplayName = "Voided", Visibility = PXUIVisibility.Visible)]
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
			}
		}
		#endregion
		#region Status
		protected String _Status;
		[PXString(1, IsFixed = true)]
		[PXDefault(CADepositStatus.Balanced, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[CADepositStatus.List()]
		public virtual String Status
		{
			[PXDependsOnFields(typeof(hold),typeof(released),typeof(voided))]
			get
			{
				if (Hold.HasValue && Hold == true)
				{
					_Status = CADepositStatus.Hold;
				}
				else
				{
					if (Released.HasValue && Released == true)
					{
						_Status = (this._Voided?? false)? CADepositStatus.Voided: CADepositStatus.Released;
					}
					else
					{
						_Status = CADepositStatus.Balanced;
					}
				}
				return this._Status;
			}
			set
			{
				
			}
		}
		#endregion
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXDBString(5, InputMask = ">LLLLL", IsUnicode = true)]
		[PXUIField(DisplayName = "Currency", Enabled = false)]
		[PXDefault(typeof(Search<CashAccount.curyID, Where<CashAccount.cashAccountID, Equal<Current<CADeposit.cashAccountID>>>>))]
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
		[CurrencyInfo(ModuleCode = "CA")]
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
		#region CuryTranAmt
		public abstract class curyTranAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryTranAmt;
		[PXDBCurrency(typeof(CADeposit.curyInfoID), typeof(CADeposit.tranAmt))]
		[PXFormula(typeof(Add<CADeposit.curyDetailTotal,Add<CADeposit.curyExtraCashTotal,Mult<CADeposit.curyChargeTotal,CADeposit.chargeMult>>>))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Total Amount", Enabled = false)]
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
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Tran Amount", Enabled = false)]
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
		#region CuryDetailTotal
		public abstract class curyDetailTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryDetailTotal;
		[PXDBCurrency(typeof(CADeposit.curyInfoID), typeof(CADeposit.detailTotal))]
		[PXUIField(DisplayName = "Deposits Total", Visibility = PXUIVisibility.Visible, Enabled = false)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryDetailTotal
		{
			get
			{
				return this._CuryDetailTotal;
			}
			set
			{
				this._CuryDetailTotal = value;
			}
		}
		#endregion
		#region DetailTotal
		public abstract class detailTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _DetailTotal;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? DetailTotal
		{
			get
			{
				return this._DetailTotal;
			}
			set
			{
				this._DetailTotal = value;
			}
		}
		#endregion
		#region EntryTypeID
		public abstract class entryTypeID : PX.Data.IBqlField
		{
		}
		protected String _EntryTypeID;
		[PXDBString(10, IsUnicode = true)]
		[PXSelector(typeof(Search2<CAEntryType.entryTypeId, InnerJoin<CashAccountETDetail, On<CashAccountETDetail.entryTypeID, Equal<CAEntryType.entryTypeId>>>,
									Where<CashAccountETDetail.accountID, Equal<Current<CADeposit.cashAccountID>>, And<CAEntryType.module, Equal<BatchModule.moduleCA>>>>), DescriptionField = typeof(CAEntryType.descr))]
		[PXUIField(DisplayName = "Entry Type", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region CuryChargeTotal
		public abstract class curyChargeTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryChargeTotal;
		[PXDBCurrency(typeof(CADeposit.curyInfoID), typeof(CADeposit.chargeTotal))]
		[PXUIField(DisplayName = "Charge Total", Visibility = PXUIVisibility.Visible, Enabled = false)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryChargeTotal
		{
			get
			{
				return this._CuryChargeTotal;
			}
			set
			{
				this._CuryChargeTotal = value;
			}
		}
		#endregion
		#region ChargeTotal
		public abstract class chargeTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _ChargeTotal;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? ChargeTotal
		{
			get
			{
				return this._ChargeTotal;
			}
			set
			{
				this._ChargeTotal = value;
			}
		}
		#endregion
		#region CuryExtraCashTotal
		public abstract class curyExtraCashTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryExtraCashTotal;
		[PXDBCurrency(typeof(CADeposit.curyInfoID), typeof(CADeposit.extraCashTotal))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Cash Drop Amount", Visibility = PXUIVisibility.Visible, Enabled = true)]
		
		public virtual Decimal? CuryExtraCashTotal
		{
			get
			{
				return this._CuryExtraCashTotal;
			}
			set
			{
				this._CuryExtraCashTotal = value;
			}
		}
		#endregion
		#region ExtraCashTotal
		public abstract class extraCashTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _ExtraCashTotal;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? ExtraCashTotal
		{
			get
			{
				return this._ExtraCashTotal;
			}
			set
			{
				this._ExtraCashTotal = value;
			}
		}
		#endregion
		#region ExtraCashAccountID
		public abstract class extraCashAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _ExtraCashAccountID;
		[GL.CashAccount(null, typeof(Search<CashAccount.cashAccountID, Where<CashAccount.curyID,Equal<Current<CADeposit.curyID>>,
															And<CashAccount.cashAccountID,NotEqual<Current<CADeposit.cashAccountID>>>>>), DisplayName = "Cash Drop Account", 
															Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(CashAccount.descr))]
		public virtual Int32? ExtraCashAccountID
		{
			get
			{
				return this._ExtraCashAccountID;
			}
			set
			{
				this._ExtraCashAccountID = value;
			}
		}
		#endregion
		#region CuryControlAmt
		public abstract class curyControlAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryControlAmt;
		[PXDBCurrency(typeof(CADeposit.curyInfoID), typeof(CADeposit.controlAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Control Total")]
		public virtual Decimal? CuryControlAmt
		{
			get
			{
				return this._CuryControlAmt;
			}
			set
			{
				this._CuryControlAmt = value;
			}
		}
		#endregion
		#region ControlAmt
		public abstract class controlAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _ControlAmt;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? ControlAmt
		{
			get
			{
				return this._ControlAmt;
			}
			set
			{
				this._ControlAmt = value;
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
		#region TranID
		public abstract class tranID : PX.Data.IBqlField
		{
		}
		protected Int64? _TranID;
		[PXDBLong()]
		[DepositTranID()]
		[PXSelector(typeof(Search<CATran.tranID>), DescriptionField = typeof(CATran.batchNbr))]
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
		#region CashTranID
		public abstract class cashTranID : PX.Data.IBqlField
		{
		}
		protected Int64? _CashTranID;
		[PXDBLong()]
		[DepositCashTranID()]
		[PXSelector(typeof(Search<CATran.tranID>), DescriptionField = typeof(CATran.batchNbr))]
		public virtual Int64? CashTranID
		{
			get
			{
				return this._CashTranID;
			}
			set
			{
				this._CashTranID = value;
			}
		}
		#endregion		
		#region ChargeTranID
		public abstract class chargeTranID : PX.Data.IBqlField
		{
		}
		protected Int64? _ChargeTranID;
		[PXDBLong()]
		[DepositChargeTranID()]
		[PXSelector(typeof(Search<CATran.tranID>), DescriptionField = typeof(CATran.batchNbr))]
		public virtual Int64? ChargeTranID
		{
			get
			{
				return this._ChargeTranID;
			}
			set
			{
				this._ChargeTranID = value;
			}
		}
		#endregion		
		#region ChargesSeparate
		public abstract class chargesSeparate : PX.Data.IBqlField
		{
		}
		protected Boolean? _ChargesSeparate;
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Separate Charges")]
		public virtual Boolean? ChargesSeparate
		{
			get
			{
				return this._ChargesSeparate;
			}
			set
			{
				this._ChargesSeparate = value;
			}
		}
		#endregion
		#region Cleared
		public abstract class cleared : PX.Data.IBqlField
		{
		}
		protected Boolean? _Cleared;
		[PXDBBool]
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
		#region TranID_CATran_batchNbr
		public abstract class tranID_CATran_batchNbr : PX.Data.IBqlField
		{
		}
		#endregion
		#region WorkgroupID
		public abstract class workgroupID : PX.Data.IBqlField
		{
		}
		protected int? _WorkgroupID;
		[PXDBInt]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXCompanyTreeSelector]
		[PXUIField(DisplayName = "Workgroup", Visibility = PXUIVisibility.Visible)]
		public virtual int? WorkgroupID
		{
			get
			{
				return this._WorkgroupID;
			}
			set
			{
				this._WorkgroupID = value;
			}
		}
		#endregion
		#region OwnerID
		public abstract class ownerID : IBqlField { }
		protected Guid? _OwnerID;
		[PXDBGuid()]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXOwnerSelector(typeof(CADeposit.workgroupID))]
		[PXUIField(DisplayName = "Owner", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Guid? OwnerID
		{
			get
			{
				return this._OwnerID;
			}
			set
			{
				this._OwnerID = value;
			}
		}
		#endregion		
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXSearchable(SM.SearchCategory.CA, "{0} {1}", new Type[] { typeof(CADeposit.tranType), typeof(CADeposit.refNbr) },
			new Type[] { typeof(CADeposit.tranDesc), typeof(CADeposit.extRefNbr) },
			NumberFields = new Type[] { typeof(CADeposit.refNbr) },
			Line1Format = "{0}{1:d}{3}", Line1Fields = new Type[] { typeof(CADeposit.extRefNbr), typeof(CADeposit.tranDate), typeof(CADeposit.cashAccountID), typeof(Account.accountCD) },
			Line2Format = "{0}", Line2Fields = new Type[] { typeof(CADeposit.tranDesc) }
		)]
		[PXNote(DescriptionField = typeof(CADeposit.refNbr))]
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
		#region ChargeMult
		public abstract class chargeMult : PX.Data.IBqlField
		{
		}
		
		[PXDecimal(2)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Control Total")]
		public virtual Decimal? ChargeMult
		{
			[PXDependsOnFields(typeof(chargesSeparate))]
			get
			{
				return (this.ChargesSeparate == true? Decimal.Zero: Decimal.MinusOne);
			}
			
		}
		#endregion
	}

	public class CADepositType
	{
        /// <summary>
        /// Specialized selector for CABatch RefNbr.<br/>
        /// By default, defines the following set of columns for the selector:<br/>
        /// CADeposit.refNbr, CADeposit.tranDate, CADeposit.finPeriodID,
		/// CADeposit.cashAccountID, CADeposit.curyID, CADeposit.curyTranAmt,
		/// CADeposit.extRefNbr
        /// <example>
        /// [CADepositType.RefNbr(typeof(Search<CADeposit.refNbr,Where<CADeposit.tranType,Equal<Current<CADeposit.tranType>>>>))]
        /// </example>
        /// </summary>
		public class RefNbrAttribute : PXSelectorAttribute
		{
			public RefNbrAttribute(Type SearchType)
				: base(SearchType,
				typeof(CADeposit.refNbr),
				typeof(CADeposit.tranDate),
				typeof(CADeposit.finPeriodID),
				typeof(CADeposit.cashAccountID),
				typeof(CADeposit.curyID),
				typeof(CADeposit.curyTranAmt),
				typeof(CADeposit.extRefNbr))
			{
			}
		}

        /// <summary>
        /// Specialized for CADeposit version of the <see cref="AutoNumberAttribute"/><br/>
        /// It defines how the new numbers are generated for the AR Invoice. <br/>
        /// References CADeposit.tranType and CADeposit.tranDate fields of the document,<br/>
        /// and also define a link between  numbering ID's defined in CASetup (namely CASetup.registerNumberingID)<br/>
        /// and CADeposit: <br/>
        /// </summary>		
		public class NumberingAttribute : CS.AutoNumberAttribute
		{
			public NumberingAttribute()
				: base(typeof(CADeposit.tranType), typeof(CADeposit.tranDate),
					new string[] { CATranType.CADeposit, CATranType.CAVoidDeposit},
					new Type[] { typeof(CASetup.registerNumberingID), null }) { ; }
		}

	}

	public class CADepositStatus
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Balanced, Hold, Released, Voided },
				new string[] { Messages.Balanced, Messages.Hold, Messages.Released, Messages.Voided }) { ; }
		}

		public const string Balanced = "B";
		public const string Hold = "H";
		public const string Released = "R";
		public const string Voided = "V";

		public class balanced : Constant<string>
		{
			public balanced() : base(Balanced) { ;}
		}

		public class hold : Constant<string>
		{
			public hold() : base(Hold) { ;}
		}

		public class released : Constant<string>
		{
			public released() : base(Released) { ;}
		}

		public class voided : Constant<string>
		{
			public voided() : base(Voided) { ;}
		}

	}
	
}

