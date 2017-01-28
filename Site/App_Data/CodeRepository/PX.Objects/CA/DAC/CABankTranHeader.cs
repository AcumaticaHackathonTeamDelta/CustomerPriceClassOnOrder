using System;
using PX.Data;
using PX.Objects.CM;

namespace PX.Objects.CA
{
	[System.SerializableAttribute()]
    [PXCacheName(Messages.BankTranHeader)]
	public partial class CABankTranHeader : PX.Data.IBqlTable
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
		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
		}
		protected string _RefNbr;
		[PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible, Required = true)]
		[PXSelector(typeof(Search<CABankTranHeader.refNbr,
                                    Where<CABankTranHeader.cashAccountID, Equal<Optional<CABankTranHeader.cashAccountID>>,
                                      And<CABankTranHeader.tranType, Equal<Optional<CABankTranHeader.tranType>>>>,
                                    OrderBy<Desc<CABankTranHeader.refNbr>>>),
                    typeof(CABankTranHeader.refNbr),
                    typeof(CABankTranHeader.cashAccountID),
                    typeof(CABankTranHeader.curyID),
                    typeof(CABankTranHeader.docDate),
                    typeof(CABankTranHeader.endBalanceDate),
                    typeof(CABankTranHeader.curyEndBalance))]
		[CS.AutoNumber(typeof(CABankTranHeader.tranType), typeof(CABankTranHeader.docDate), new string[] {CABankTranType.Statement, CABankTranType.PaymentImport},
			new Type[] { typeof(CASetup.cAStatementNumberingID), typeof(CASetup.cAImportPaymentsNumberingID) })]
		public virtual string RefNbr
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
		#region TranType
		public abstract class tranType : PX.Data.IBqlField
		{
		}
		protected String _TranType;
		[PXDBString(1, IsFixed = true, IsKey = true)]
		[PXDefault(typeof(CABankTranType.statement))]
		[CABankTranType.List()]
		[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = false, Visible = false, TabOrder = 0)]
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
		#region DocDate
		public abstract class docDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _DocDate;
		[PXDBDate()]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Statement Date")]
		public virtual DateTime? DocDate
		{
			get
			{
				return this._DocDate;
			}
			set
			{
				this._DocDate = value;
			}
		}
		#endregion
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXDBString(5, InputMask = ">LLLLL", IsUnicode = true)]
		[PXDefault(typeof(Search<CashAccount.curyID, Where<CashAccount.cashAccountID, Equal<Current<CABankTranHeader.cashAccountID>>>>))]
		[PXUIField(DisplayName = "Currency", Enabled = false)]
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
		#region StartBalanceDate
		public abstract class startBalanceDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _StartBalanceDate;
		[PXDBDate()]
		[PXDefault(typeof(Search<CABankTranHeader.endBalanceDate,
					 Where<CABankTranHeader.cashAccountID, Equal<Current<CABankTranHeader.cashAccountID>>,
						And<CABankTranHeader.tranType, Equal<Current<CABankTranHeader.tranType>>,
						And<CABankTranHeader.endBalanceDate, LessEqual<Current<CABankTranHeader.docDate>>,
						And<Where<Current<CABankTranHeader.refNbr>, IsNull, Or<CABankTranHeader.refNbr, NotEqual<Current<CABankTranHeader.refNbr>>>>>>>>,
						OrderBy<Desc<CABankTranHeader.startBalanceDate>>>),
						PersistingCheck = PXPersistingCheck.NullOrBlank)]
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
		[PXDBCury(typeof(CABankTranHeader.curyID))]
		[PXDefault(TypeCode.Decimal, "0.0",
			typeof(Search<CABankTranHeader.curyEndBalance,
				 Where<CABankTranHeader.cashAccountID, Equal<Current<CABankTranHeader.cashAccountID>>,
					And<CABankTranHeader.tranType, Equal<Current<CABankTranHeader.tranType>>,
					And<CABankTranHeader.endBalanceDate, LessEqual<Current<CABankTranHeader.docDate>>,
					And<Where<Current<CABankTranHeader.refNbr>, IsNull, Or<CABankTranHeader.refNbr, NotEqual<Current<CABankTranHeader.refNbr>>>>>>>>,
					OrderBy<Desc<CABankTranHeader.startBalanceDate>>>))]
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
		[PXDBCury(typeof(CABankTranHeader.curyID))]
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
		#region CuryDebitsTotal
		public abstract class curyDebitsTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryDebitsTotal;
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCury(typeof(CABankTranHeader.curyID))]
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
		[PXDBCury(typeof(CABankTranHeader.curyID))]
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
		#region CuryDetailsEndBalance
		public abstract class curyDetailsEndBalance : PX.Data.IBqlField
		{
		}
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Calculated Balance", Enabled = false)]
		[PXCury(typeof(CABankTranHeader.curyID))]
		public virtual Decimal? CuryDetailsEndBalance
		{
			[PXDependsOnFields(typeof(curyBegBalance), typeof(curyDebitsTotal), typeof(curyCreditsTotal))]
			get
			{
				return this._CuryBegBalance + (this.CuryDebitsTotal - this.CuryCreditsTotal);
			}
			set
			{

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
}
