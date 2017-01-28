using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PX.Common;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.CM;
using PX.Objects.CR;

namespace PX.Objects.CA
{
	public interface IStatementReader
	{
		void Read(byte[] aInput);
		bool IsValidInput(byte[] aInput);
		bool AllowsMultipleAccounts();
		void ExportTo(PX.SM.FileInfo aFileInfo, CABankStatementEntry current, out List<CABankStatement> aExported);
		void ExportToNew<T>(PX.SM.FileInfo aFileInfo, T current, out List<CABankTranHeader> aExported)
			where T : CABankTransactionsImport, new();
	}
}

namespace PX.Objects.CA.BankStatementHelpers
{
	public interface IBankMatchRelevance
	{
		Decimal? MatchRelevance { get; set; }
	}

	[Serializable]
	[PXHidden]
	public partial class CABankStmtDetailDocRef : PX.Data.IBqlTable, IBankMatchRelevance
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
		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
		}
		protected String _RefNbr;
		[PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXDBDefault(typeof(CABankStatement.refNbr))]
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
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineNbr;
		[PXDBInt(IsKey = true)]
		[PXParent(typeof(Select<CABankStatementDetail, Where<CABankStatementDetail.refNbr, Equal<Current<CABankStmtDetailDocRef.refNbr>>,
									And<CABankStatementDetail.lineNbr, Equal<Current<CABankStmtDetailDocRef.lineNbr>>>>>))]
		public virtual Int32? LineNbr
		{
			get
			{
				return this._LineNbr;
			}
			set
			{
				this._LineNbr = value;
			}
		}
		#endregion
		#region CATranID
		public abstract class cATranID : PX.Data.IBqlField
		{
		}
		protected Int64? _CATranID;

		[PXDBLong(IsKey = true)]
		public virtual Int64? CATranID
		{
			get
			{
				return this._CATranID;
			}
			set
			{
				this._CATranID = value;
			}
		}
		#endregion
		#region CashAccountID
		public abstract class cashAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _CashAccountID;
		[PXDBInt()]
		[PXUIField(DisplayName = "Cash Account ID", Visible = false)]
		[PXDefault()]
		public virtual int? CashAccountID
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
		#region MatchRelevance
		public abstract class matchRelevance : PX.Data.IBqlField
		{
		}
		protected Decimal? _MatchRelevance;
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBDecimal(6)]
		[PXUIField(DisplayName = "Match Relevance", Enabled = false)]
		public virtual Decimal? MatchRelevance
		{
			get
			{
				return this._MatchRelevance;
			}
			set
			{
				this._MatchRelevance = value;
			}
		}
		#endregion

		public void Copy(CATran aSrc)
		{
			this.CashAccountID = aSrc.CashAccountID;
			this.CATranID = aSrc.TranID;
		}
		public void Copy(CABankStatementDetail aSrc)
		{
			this.RefNbr = aSrc.RefNbr;
			this.LineNbr = aSrc.LineNbr;
			this.CashAccountID = aSrc.CashAccountID;
		}

	}
	
	[Serializable]
	public partial class CATranExt : CATran, IBankMatchRelevance
	{
		#region TranID
		public new abstract class tranID : PX.Data.IBqlField
		{
		}
		#endregion
		#region IsMatched
		public abstract class isMatched : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsMatched;
		[PXBool()]
		[PXUIField(DisplayName = "Matched")]
		public virtual Boolean? IsMatched
		{
			get
			{
				return this._IsMatched;
			}
			set
			{
				this._IsMatched = value;
			}
		}
		#endregion
		#region MatchRelevance
		public abstract class matchRelevance : PX.Data.IBqlField
		{
		}
		protected Decimal? _MatchRelevance;
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXDecimal(3)]
		[PXUIField(DisplayName = "Match Relevance", Enabled = false)]
		public virtual Decimal? MatchRelevance
		{
			get
			{
				return this._MatchRelevance;
			}
			set
			{
				this._MatchRelevance = value;
			}
		}
		#endregion
		#region Released
		public new abstract class released : PX.Data.IBqlField
		{
		}

		[PXDBBool()]
		[PXUIField(DisplayName = "Released", Enabled = false)]
		[PXDefault(false)]
		public override Boolean? Released
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
		#region CuryTranAbsAmt
		public abstract class curyTranAbsAmt : PX.Data.IBqlField
		{
		}
		[PXCurrency(typeof(CATran.curyInfoID), typeof(CATran.tranAmt))]
		[PXUIField(DisplayName = "Amount", Visibility = PXUIVisibility.Visible)]
		public virtual Decimal? CuryTranAbsAmt
		{
			get
			{
				return Math.Abs(this._CuryTranAmt??0m);
			}
			set
			{
			}
		}
		#endregion
		#region TranAbsAmt
		public abstract class tranAbsAmt : PX.Data.IBqlField
		{
		}
		[PXDecimal(4)]
		[PXUIField(DisplayName = "Tran. Amount")]
		public virtual Decimal? TranAbsAmt
		{
			get
			{
				return  Math.Abs(this._TranAmt??0m);
			}
			set
			{
			}
		}
		#endregion
        #region IsBestMatch
        public abstract class isBestMatch : PX.Data.IBqlField
        {
        }
        protected Boolean? _IsBestMatch;
        [PXBool()]
        [PXUIField(DisplayName = "Best Match")]
        public virtual Boolean? IsBestMatch
        {
            get
            {
                return this._IsBestMatch;
            }
            set
            {
                this._IsBestMatch = value;
            }
        }
        #endregion
	}

	[Serializable]
	[PXHidden]
	public partial class CATran2 : CATran
	{
		#region TranID
		public new abstract class tranID : PX.Data.IBqlField
		{
		}
		#endregion
		#region CashAccountID
		public new abstract class cashAccountID : PX.Data.IBqlField
		{
		}
		#endregion
		#region VoidedTranID
		public new abstract class voidedTranID : PX.Data.IBqlField
		{
		}
		#endregion
	}

	public partial class AdjustInfo : CA.ICADocAdjust
	{
		private string _AdjdDocType;
		private string _AdjdRefNbr;
		private Decimal? _CuryAdjgAmount;
		private Decimal? _CuryAdjgWhTaxAmt;
		private Decimal? _CuryAdjgDiscAmt;
		private Decimal? _AdjdCuryRate;


		#region ICADocAdjust Members

		public string AdjdDocType
		{
			get
			{
				return this._AdjdDocType;
			}
			set
			{
				this._AdjdDocType = value;
			}
		}

		public string AdjdRefNbr
		{
			get
			{
				return this._AdjdRefNbr;
			}
			set
			{
				this._AdjdRefNbr = value;
			}
		}

		public Decimal? CuryAdjgAmount
		{
			get
			{
				return this._CuryAdjgAmount;
			}
			set
			{
				this._CuryAdjgAmount = value;
			}
		}
		public Decimal? CuryAdjgDiscAmt
		{
			get
			{
				return this._CuryAdjgDiscAmt;
			}
			set
			{
				this._CuryAdjgDiscAmt = value;
			}
		}
		public Decimal? CuryAdjgWhTaxAmt
		{
			get
			{
				return this._CuryAdjgWhTaxAmt;
			}
			set
			{
				this._CuryAdjgWhTaxAmt = value;
			}
		}
		public Decimal? AdjdCuryRate
		{
			get
			{
				return this._AdjdCuryRate;
			}
			set
			{
				this._AdjdCuryRate = value;
			}
		}

		#endregion

		public void Copy(APRegister src)
		{
			this._AdjdDocType = src.DocType;
			this._AdjdRefNbr = src.RefNbr;
		}

		public void Copy(ARRegister src)
		{
			this._AdjdDocType = src.DocType;
			this._AdjdRefNbr = src.RefNbr;
		}
	}

	public interface IMatchSettings
	{
		int? DisbursementTranDaysBefore { get; set; }
		int? DisbursementTranDaysAfter { get; set; }

		int? ReceiptTranDaysBefore { get; set; }
		int? ReceiptTranDaysAfter { get; set; }

		Decimal? RefNbrCompareWeight { get; set; }
		Decimal? DateCompareWeight { get; set; }
		Decimal? PayeeCompareWeight { get; set; }

		Decimal? DateMeanOffset { get; set; }
		Decimal? DateSigma { get; set; }
		Boolean? MatchInSelection { get; set; }
		Boolean? SkipVoided { get; set; }
	}

	[Serializable]
	public partial class MatchSettings : IBqlTable, IMatchSettings
	{
		#region ReceiptTranDaysBefore
		public abstract class receiptTranDaysBefore : PX.Data.IBqlField
		{
		}
		protected Int32? _ReceiptTranDaysBefore;
		[PXInt(MinValue = 0, MaxValue = 365)]
		[PXDefault(5, typeof(CASetup.receiptTranDaysBefore), PersistingCheck = PXPersistingCheck.Nothing)]
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
		[PXInt(MinValue = 0, MaxValue = 365)]
		[PXDefault(2, typeof(CASetup.receiptTranDaysAfter), PersistingCheck = PXPersistingCheck.Nothing)]
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
		[PXInt(MinValue = 0, MaxValue = 365)]
		[PXDefault(5, typeof(CASetup.disbursementTranDaysBefore), PersistingCheck = PXPersistingCheck.Nothing)]
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
		[PXInt(MinValue = 0, MaxValue = 365)]
		[PXDefault(2, typeof(CASetup.disbursementTranDaysAfter), PersistingCheck = PXPersistingCheck.Nothing)]
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
		[PXDecimal(MinValue = 0, MaxValue = 100.0)]
		[PXDefault(TypeCode.Decimal, "70.0", typeof(CASetup.refNbrCompareWeight),
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
		[PXDecimal(MinValue = 0, MaxValue = 100)]
		[PXDefault(TypeCode.Decimal, "20.0",
			typeof(CASetup.dateCompareWeight),
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
		[PXDecimal(MinValue = 0, MaxValue = 100)]
		[PXDefault(TypeCode.Decimal, "10.0",
				typeof(CASetup.payeeCompareWeight),
				PersistingCheck = PXPersistingCheck.Nothing)]
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
		#region DateMeanOffset
		public abstract class dateMeanOffset : PX.Data.IBqlField
		{
		}
		protected Decimal? _DateMeanOffset;
		[PXDecimal(MinValue = -365, MaxValue = 365)]
		[PXDefault(TypeCode.Decimal, "10.0",
			typeof(CASetup.dateMeanOffset),
				PersistingCheck = PXPersistingCheck.Nothing)]
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
		[PXDecimal(MinValue = 0, MaxValue = 365)]
		[PXDefault(TypeCode.Decimal, "5.0",
			typeof(CASetup.dateSigma),
				PersistingCheck = PXPersistingCheck.Nothing)]
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
		#region MatchInSelection
		public abstract class matchInSelection : PX.Data.IBqlField
		{
		}
		protected Boolean? _MatchInSelection;
		[PXBool()]
		[PXDefault(false,
				typeof(CASetup.matchInSelection),
				PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Match in the selection only")]
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

		#region SkipVoided
		public abstract class skipVoided : PX.Data.IBqlField
		{
		}
		protected Boolean? _SkipVoided;
		[PXBool()]
		[PXDefault(false, typeof(CASetup.skipVoided),
				PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Skip Voided transactions during matching")]
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

	}

	public static class ImportProviderParams
	{
		public const string FileName = "FileName";
		public const string StatementNbr = "RefNbr";
		public const string BatchSequenceStartingNbr = "BatchStartNumber";
	}

	[PXHidden]
	public partial class GeneralInvoice : IBqlTable
	{
		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
		}
		private String _RefNbr;
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		[PXString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		public String RefNbr
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
		#region OrigModule
		public abstract class origModule : PX.Data.IBqlField
		{
		}
		protected String _OrigModule;
		[PXString(2, IsFixed = true)]
		[PXUIField(DisplayName = "Source", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[GL.BatchModule.FullList()]
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
		#region DocType
		public abstract class docType : PX.Data.IBqlField
		{
		}
		protected String _DocType;
		[PXString(3, IsKey = true, IsFixed = true)]
		[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = true, TabOrder = 0)]
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
		#region DocDate
		public abstract class docDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _DocDate;
		[PXDate()]
		[PXUIField(DisplayName = "Date", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected String _FinPeriodID;
		[APOpenPeriod(typeof(APRegister.docDate))]
		[PXDefault()]
		[PXUIField(DisplayName = "Post Period", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region BAccountID
		public abstract class bAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _bAccountID;
		[PXDefault()]
		public virtual Int32? BAccountID
		{
			get
			{
				return this._bAccountID;
			}
			set
			{
				this._bAccountID = value;
			}
		}
		#endregion
		#region LocationID
		public abstract class locationID : PX.Data.IBqlField
		{
		}
		protected Int32? _LocationID;
		[LocationID(
			typeof(Where<Location.bAccountID, Equal<Optional<GeneralInvoice.bAccountID>>,
				And<Location.isActive, Equal<boolTrue>,
				And<MatchWithBranch<Location.vBranchID>>>>),
			DescriptionField = typeof(Location.descr),
			Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Int32? LocationID
		{
			get
			{
				return this._LocationID;
			}
			set
			{
				this._LocationID = value;
			}
		}
		#endregion
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXString(5, IsUnicode = true, InputMask = ">LLLLL")]
		[PXUIField(DisplayName = "Currency", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(typeof(Search<Company.baseCuryID>))]
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
		#region CuryOrigDocAmt
		public abstract class curyOrigDocAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryOrigDocAmt;
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXCury(typeof(GeneralInvoice.curyID))]
		[PXUIField(DisplayName = "Amount", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? CuryOrigDocAmt
		{
			get
			{
				return this._CuryOrigDocAmt;
			}
			set
			{
				this._CuryOrigDocAmt = value;
			}
		}
		#endregion
		#region CuryDocBal
		public abstract class curyDocBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryDocBal;
		[PXCury(typeof(GeneralInvoice.curyID))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Balance", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual Decimal? CuryDocBal
		{
			get
			{
				return this._CuryDocBal;
			}
			set
			{
				this._CuryDocBal = value;
			}
		}
		#endregion
		#region Status
		public abstract class status : PX.Data.IBqlField
		{
		}
		protected String _Status;
		[PXString(1, IsFixed = true)]
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
		#region DueDate
		public abstract class dueDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _DueDate;
		[PXDate()]
		[PXUIField(DisplayName = "Due Date", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual DateTime? DueDate
		{
			get
			{
				return this._DueDate;
			}
			set
			{
				this._DueDate = value;
			}
		}
		#endregion
		#endregion
	}

	public class PXInvoiceSelectorAttribute : PXCustomSelectorAttribute
	{
		protected Type _BatchModule;

		public PXInvoiceSelectorAttribute(Type BatchModule)
			: base(typeof(GeneralInvoice.refNbr),
			   typeof(GeneralInvoice.refNbr),
			   typeof(GeneralInvoice.docDate),
			   typeof(GeneralInvoice.finPeriodID),
			   typeof(GeneralInvoice.locationID),
			   typeof(GeneralInvoice.curyID),
			   typeof(GeneralInvoice.curyOrigDocAmt),
			   typeof(GeneralInvoice.curyDocBal),
			   typeof(GeneralInvoice.status),
			   typeof(GeneralInvoice.dueDate))
		{
			this._BatchModule = BatchModule;
		}

		protected virtual IEnumerable GetRecords()
		{
			PXCache cache = this._Graph.Caches[BqlCommand.GetItemType(this._BatchModule)];
			PXCache adjustments = this._Graph.Caches[typeof(CABankStatementAdjustment)];
			object current = null;
			foreach (object item in PXView.Currents)
			{
				if (item != null && (item.GetType() == typeof(CABankStatementAdjustment) || item.GetType().IsSubclassOf(typeof(CABankStatementAdjustment))))
				{
					current = item;
					break;
				}
			}
			if (current == null)
			{
				current = adjustments.Current;
			}

			if (cache.Current == null) yield break;
			CABankStatementAdjustment row = current as CABankStatementAdjustment;
			string tranModule = (string)cache.GetValue(cache.Current, this._BatchModule.Name);
			switch (tranModule)
			{
				case GL.BatchModule.AP:
					foreach (PX.Objects.AP.APAdjust.APInvoice apInvoice in PXSelectJoin<PX.Objects.AP.APAdjust.APInvoice,
						   LeftJoin<APAdjust, On<APAdjust.adjdDocType, Equal<PX.Objects.AP.APAdjust.APInvoice.docType>,
							   And<APAdjust.adjdRefNbr, Equal<PX.Objects.AP.APAdjust.APInvoice.refNbr>, And<APAdjust.released, Equal<boolFalse>>>>,
						   LeftJoin<CABankStatementAdjustment, On<CABankStatementAdjustment.adjdDocType, Equal<PX.Objects.AP.APAdjust.APInvoice.docType>,
							And<CABankStatementAdjustment.adjdRefNbr, Equal<PX.Objects.AP.APAdjust.APInvoice.refNbr>, And<CABankStatementAdjustment.released, Equal<boolFalse>,
								   And<Where<CABankStatementAdjustment.refNbr,
					   NotEqual<Current<CABankStatementAdjustment.refNbr>>, Or<CABankStatementAdjustment.lineNbr, NotEqual<Current<CABankStatementAdjustment.lineNbr>>>>>
								>>>,
						   LeftJoin<APPayment, On<APPayment.docType, Equal<PX.Objects.AP.APAdjust.APInvoice.docType>,
							   And<APPayment.refNbr, Equal<PX.Objects.AP.APAdjust.APInvoice.refNbr>, And<
							   Where<APPayment.docType, Equal<APDocType.prepayment>, Or<APPayment.docType, Equal<APDocType.debitAdj>>>>>>>>>,
						   Where<PX.Objects.AP.APAdjust.APInvoice.vendorID, Equal<Optional<CAApplicationStatementDetail.payeeBAccountID>>, And<PX.Objects.AP.APAdjust.APInvoice.docType, Equal<Optional<CABankStatementAdjustment.adjdDocType>>,
						   And2<Where<PX.Objects.AP.APAdjust.APInvoice.released, Equal<True>, Or<PX.Objects.AP.APAdjust.APInvoice.prebooked, Equal<True>>>, And<PX.Objects.AP.APAdjust.APInvoice.openDoc, Equal<boolTrue>, And<CABankStatementAdjustment.adjdRefNbr, IsNull, And<APAdjust.adjgRefNbr, IsNull,
							  And2<Where<APPayment.refNbr, IsNull, And<Current<CAApplicationStatementDetail.docType>, NotEqual<APDocType.refund>,
							   Or<APPayment.refNbr, IsNotNull, And<Current<CAApplicationStatementDetail.docType>, Equal<APDocType.refund>,
							   Or<APPayment.docType, Equal<APDocType.debitAdj>, And<Current<CAApplicationStatementDetail.docType>, Equal<APDocType.check>,
							   Or<APPayment.docType, Equal<APDocType.debitAdj>, And<Current<CAApplicationStatementDetail.docType>, Equal<APDocType.voidCheck>>>>>>>>>,
						  And<PX.Objects.AP.APAdjust.APInvoice.docDate, LessEqual<Current<CAApplicationStatementDetail.tranDate>>>>>>>>>>>
						  .SelectMultiBound(this._Graph, new object[] { current }))
					{
						CABankStatementAdjustment adjustment = null;
						foreach (CABankStatementAdjustment adj in adjustments.Inserted)
						{
							if ((adj.AdjdDocType == apInvoice.DocType && adj.AdjdRefNbr == apInvoice.RefNbr && row != null && (adj.RefNbr != row.RefNbr || adj.LineNbr != row.LineNbr))
								|| (row == null && adj.AdjdDocType == apInvoice.DocType && adj.AdjdRefNbr == apInvoice.RefNbr))
							{
								adjustment = adj;
								break;
							}
						}
						if (adjustment != null) continue;
						GeneralInvoice gInvoice = new GeneralInvoice();
						gInvoice.RefNbr = apInvoice.RefNbr;
						gInvoice.OrigModule = apInvoice.OrigModule;
						gInvoice.DocType = apInvoice.DocType;
						gInvoice.DocDate = apInvoice.DocDate;
						gInvoice.FinPeriodID = apInvoice.FinPeriodID;
						gInvoice.LocationID = apInvoice.VendorLocationID;
						gInvoice.CuryID = apInvoice.CuryID;
						gInvoice.CuryOrigDocAmt = apInvoice.CuryOrigDocAmt;
						gInvoice.CuryDocBal = apInvoice.CuryDocBal;
						gInvoice.Status = apInvoice.Status;
						gInvoice.DueDate = apInvoice.DueDate;
						yield return gInvoice;
					}

					break;
				case GL.BatchModule.AR:
					foreach (ARInvoice arInvoice in PXSelectJoin<ARInvoice,
						LeftJoin<ARAdjust, On<ARAdjust.adjdDocType, Equal<ARInvoice.docType>, And<ARAdjust.adjdRefNbr, Equal<ARInvoice.refNbr>,
						And<ARAdjust.released, Equal<boolFalse>, And<ARAdjust.voided, Equal<boolFalse>, And<Where<ARAdjust.adjgDocType, NotEqual<Current<CABankStatementAdjustment.adjdDocType>>>>>>>>,
						LeftJoin<CABankStatementAdjustment, On<CABankStatementAdjustment.adjdDocType, Equal<ARInvoice.docType>,
						And<CABankStatementAdjustment.adjdRefNbr, Equal<ARInvoice.refNbr>,
						And<CABankStatementAdjustment.released, Equal<boolFalse>,
								And<Where<CABankStatementAdjustment.refNbr,
						NotEqual<Current<CABankStatementAdjustment.refNbr>>, Or<CABankStatementAdjustment.lineNbr, NotEqual<Current<CABankStatementAdjustment.lineNbr>>>>>
						>>>>>,
						Where<ARInvoice.customerID, Equal<Current<CAApplicationStatementDetail.payeeBAccountID>>,
						And<ARInvoice.docType, Equal<Current<CABankStatementAdjustment.adjdDocType>>,
						And<ARInvoice.released, Equal<boolTrue>,
						And<ARInvoice.openDoc, Equal<boolTrue>,
						And<ARAdjust.adjgRefNbr, IsNull,
						And<CABankStatementAdjustment.adjdRefNbr, IsNull,
						And<ARInvoice.docDate, LessEqual<Current<CAApplicationStatementDetail.tranDate>>>
						>>>>>>>.SelectMultiBound(this._Graph, new object[] { current }))
					{
						CABankStatementAdjustment adjustment = null;
						foreach (CABankStatementAdjustment adj in adjustments.Inserted)
						{
							if (adj.AdjdDocType == arInvoice.DocType && adj.AdjdRefNbr == arInvoice.RefNbr && row != null && (adj.RefNbr != row.RefNbr || adj.LineNbr != row.LineNbr)
							|| (row == null && adj.AdjdDocType == arInvoice.DocType && adj.AdjdRefNbr == arInvoice.RefNbr)
								)
							{
								adjustment = adj;
								break;
							}
						}
						if (adjustment != null) continue;
						GeneralInvoice gInvoice = new GeneralInvoice();
						gInvoice.RefNbr = arInvoice.RefNbr;
						gInvoice.OrigModule = arInvoice.OrigModule;
						gInvoice.DocType = arInvoice.DocType;
						gInvoice.DocDate = arInvoice.DocDate;
						gInvoice.FinPeriodID = arInvoice.FinPeriodID;
						gInvoice.LocationID = arInvoice.CustomerLocationID;
						gInvoice.CuryID = arInvoice.CuryID;
						gInvoice.CuryOrigDocAmt = arInvoice.CuryOrigDocAmt;
						gInvoice.CuryDocBal = arInvoice.CuryDocBal;
						gInvoice.Status = arInvoice.Status;
						gInvoice.DueDate = arInvoice.DueDate;
						yield return gInvoice;
					}

					break;
			}
		}
	}

	public class CurrencyInfoConditional : CurrencyInfoAttribute
	{
		Type _conditionField = null;
		public CurrencyInfoConditional(Type conditionField)
			: base()
		{
			this._conditionField = conditionField;
		}

		public override void RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			if (this._conditionField != null)
			{
				Boolean? condition = sender.GetValue(e.Row, this._conditionField.Name) as Boolean?;
				if (condition ?? false)
					base.RowInserting(sender, e);
			}
			else
				base.RowInserting(sender, e);
		}
		public override void RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
		{
			if (this._conditionField != null)
			{
				Boolean? newCondition = sender.GetValue(e.NewRow, this._conditionField.Name) as Boolean?;
				Boolean? condition = sender.GetValue(e.Row, this._conditionField.Name) as Boolean?;
				object value = sender.GetValue(e.Row, this._FieldName);
				if ((newCondition ?? false) || ((newCondition ?? false) && (condition ?? false) && value == null))
					base.RowUpdating(sender, e);
			}
			else
				base.RowUpdating(sender, e);
		}
	}
	
	public static class StatementMatching
	{
		public static IEnumerable FindDetailMatches(CABankStatementEntry graph, PXCache DetailsCache, CABankStatementDetail aDetail, IMatchSettings aSettings, bool skipLowRelevance)
		{
			List<CATranExt> matchList = new List<CATranExt>();
			bool hasBaccount = aDetail.PayeeBAccountID.HasValue;
			bool hasLocation = aDetail.PayeeLocationID.HasValue;
			if (!aDetail.TranEntryDate.HasValue && !aDetail.TranDate.HasValue) return matchList;
			Pair<DateTime, DateTime> tranDateRange = GetDateRangeForMatch(aDetail, aSettings);
			const decimal relevanceTreshhold = 0.2m;
			string CuryID = aDetail.CuryID; //Need to reconsider.            
			foreach (PXResult<CATranExt, BAccountR, CABankStatementDetail, CATran2> iRes in
				PXSelectReadonly2<CATranExt,
					LeftJoin<BAccountR, On<BAccountR.bAccountID, Equal<CATran.referenceID>>,
					LeftJoin<CABankStatementDetail, On<CABankStatementDetail.cATranID, Equal<CATran.tranID>>,
					LeftJoin<CATran2, On<CATran2.cashAccountID, Equal<CATran.cashAccountID>,
						And<CATran2.voidedTranID, Equal<CATran.tranID>>>>>>,
					Where<CATran.cashAccountID, Equal<Current<CABankStatement.cashAccountID>>,
						And2<Where<CABankStatementDetail.refNbr, IsNull, Or<CABankStatementDetail.refNbr, Equal<Current<CABankStatement.refNbr>>>>,
						And<CATran.tranDate, Between<Required<CATran.tranDate>, Required<CATran.tranDate>>, And<CATran.curyID, Equal<Required<CATran.curyID>>,
						And<CATran.curyTranAmt, Equal<Required<CATran.curyTranAmt>>,
						And<CATran.drCr, Equal<Required<CATran.drCr>>>>>>>>>.Select(graph, tranDateRange.first, tranDateRange.second,
					CuryID, aDetail.CuryTranAmt.Value, aDetail.DrCr))
			{
				CATranExt iTran = iRes;
				BAccount iPayee = iRes;
				CATran2 iVoidTran = iRes;

				if (aSettings.SkipVoided == true && (iTran.VoidedTranID.HasValue
													 || (iVoidTran != null && iVoidTran.TranID.HasValue)))
				{
					continue;
				}

				if (hasBaccount && iTran.ReferenceID != aDetail.PayeeBAccountID) continue;
				iTran.ReferenceName = iPayee.AcctName;
				CABankStatementDetail iLinkedDetail = iRes;
				//Check updated in cache
				CABankStatementDetail sourceRow = null;
				bool hasLinkedDetail = (iLinkedDetail.LineNbr != null
										&& (iLinkedDetail.RefNbr != aDetail.RefNbr || iLinkedDetail.LineNbr != aDetail.LineNbr));
				bool linkCleared = !hasLinkedDetail;
				if (hasLinkedDetail)
				{
					foreach (CABankStatementDetail iDetail in DetailsCache.Deleted)
					{
						if (iDetail.CashAccountID == iLinkedDetail.CashAccountID && iDetail.RefNbr == iLinkedDetail.RefNbr &&
							iDetail.LineNbr == iLinkedDetail.LineNbr)
						{
							linkCleared = true;
							break;
						}
					}
				}

				foreach (CABankStatementDetail iDetail in DetailsCache.Inserted)
				{
					if (iDetail.CATranID == iTran.TranID && (iDetail.RefNbr != aDetail.RefNbr || iDetail.LineNbr != aDetail.LineNbr))
					{
						sourceRow = iDetail;
						break;
					}
				}

				if (sourceRow != null) continue;
				foreach (CABankStatementDetail iDetail in DetailsCache.Updated)
				{
					if (iDetail.CATranID == iTran.TranID && (iDetail.RefNbr != aDetail.RefNbr || iDetail.LineNbr != aDetail.LineNbr))
					{
						sourceRow = iDetail;
						if (hasLinkedDetail == false || linkCleared == true)
							break;
					}
					if (hasLinkedDetail && !linkCleared)
					{
						if (iDetail.RefNbr == iLinkedDetail.RefNbr &&
							iDetail.LineNbr == iLinkedDetail.LineNbr)
						{
							if (iDetail.CATranID == null || iDetail.CATranID != iTran.TranID)
							{
								linkCleared = true;
								if (sourceRow != null)
									break;
							}
						}
					}
				}
				if (sourceRow != null || (hasLinkedDetail && !linkCleared)) continue;
				iTran.MatchRelevance = graph.EvaluateMatching(aDetail, iTran, aSettings);
				iTran.IsMatched = (iTran.TranID == aDetail.CATranID);
				if (skipLowRelevance && (iTran.IsMatched == false && iTran.MatchRelevance < relevanceTreshhold))
					continue;
				matchList.Add(iTran);
			}
			return matchList;
		}

		public static decimal EvaluateMatching(CABankStatementEntry graph, CABankStatementDetail aDetail, CATran aTran, IMatchSettings aSettings)
		{
			decimal relevance = Decimal.Zero;
			decimal[] weights = { 0.1m, 0.7m, 0.2m };
			double sigma = 50.0;
			double meanValue = -7.0;
			if (aSettings != null)
			{
				if (aSettings.DateCompareWeight.HasValue && aSettings.RefNbrCompareWeight.HasValue && aSettings.PayeeCompareWeight.HasValue)
				{
					Decimal totalWeight = (aSettings.DateCompareWeight.Value + aSettings.RefNbrCompareWeight.Value + aSettings.PayeeCompareWeight.Value);
					if (totalWeight != Decimal.Zero)
					{
						weights[0] = aSettings.DateCompareWeight.Value / totalWeight;
						weights[1] = aSettings.RefNbrCompareWeight.Value / totalWeight;
						weights[2] = aSettings.PayeeCompareWeight.Value / totalWeight;
					}
				}
				if (aSettings.DateMeanOffset.HasValue)
					meanValue = (double)aSettings.DateMeanOffset.Value;
				if (aSettings.DateSigma.HasValue)
					sigma = (double)aSettings.DateSigma.Value;
			}
			bool looseCompare = false;
			relevance += graph.CompareDate(aDetail, aTran, meanValue, sigma) * weights[0];
			relevance += graph.CompareRefNbr(aDetail, aTran, looseCompare) * weights[1];
			relevance += graph.ComparePayee(aDetail, aTran) * weights[2];
			return relevance;
		}

		public static decimal CompareDate(CABankStatementDetail aDetail, CATran aTran, Double meanValue, Double sigma)
		{
			TimeSpan diff1 = (aDetail.TranDate.Value - aTran.TranDate.Value);
			TimeSpan diff2 = aDetail.TranEntryDate.HasValue ? (aDetail.TranEntryDate.Value - aTran.TranDate.Value) : diff1;
			TimeSpan diff = diff1.Duration() < diff2.Duration() ? diff1 : diff2;
			Double sigma2 = (sigma * sigma);
			if (sigma2 < 1.0)
			{
				sigma2 = 0.25; //Corresponds to 0.5 day
			}
			decimal res = (decimal)Math.Exp(-(Math.Pow(diff.TotalDays - meanValue, 2.0) / (2 * sigma2))); //Normal Distribution 
			return res > 0 ? res : 0.0m;
		}

		public static decimal CompareRefNbr(CABankStatementEntry graph, CABankStatementDetail aDetail, CATran aTran, bool looseCompare)
		{
			if (looseCompare)
				return graph.EvaluateMatching(aDetail.ExtRefNbr, aTran.ExtRefNbr, false);
			else
				return graph.EvaluateTideMatching(aDetail.ExtRefNbr, aTran.ExtRefNbr, false);
		}

		public static decimal ComparePayee(CABankStatementEntry graph, CABankStatementDetail aDetail, CATran aTran)
		{
			if (aDetail.PayeeBAccountID.HasValue)
			{
				return aDetail.PayeeBAccountID == aTran.ReferenceID ? Decimal.One : Decimal.Zero;
			}
			if (String.IsNullOrEmpty(aDetail.PayeeName) || String.IsNullOrEmpty(aTran.ReferenceName))
				return Decimal.Zero;
			return graph.EvaluateMatching(aDetail.PayeeName, aTran.ReferenceName, false);
		}

		public static decimal EvaluateMatching(string aStr1, string aStr2, bool aCaseSensitive)
		{
			decimal result = Decimal.Zero;
			if (String.IsNullOrEmpty(aStr1) || String.IsNullOrEmpty(aStr2))
			{
				return (String.IsNullOrEmpty(aStr1) && String.IsNullOrEmpty(aStr2)) ? Decimal.One : Decimal.Zero;
			}
			string str1 = aStr1.Trim();
			string str2 = aStr2.Trim();
			int length = str1.Length > str2.Length ? str1.Length : str2.Length;
			if (length == 0) return Decimal.One;
			Decimal charWeight = Decimal.One / (decimal)length;
			Decimal total = Decimal.Zero;
			for (int i = 0; i < length; i++)
			{
				if (i < str1.Length && i < str2.Length)
				{
					bool match = (aCaseSensitive) ? (str2[i].CompareTo(str2[i]) == 0) : (Char.ToLower(str2[i]).CompareTo(Char.ToLower(str1[i])) == 0);
					if (match)
						result += charWeight;
				}
				total += charWeight;
			}
			//Compencate rounding
			if (result > Decimal.Zero && total != Decimal.One)
			{
				result += (Decimal.One - total);
			}
			return result;
		}

		public static decimal EvaluateTideMatching(string aStr1, string aStr2, bool aCaseSensitive)
		{
			decimal result = Decimal.One;
			const int maxDiffCount = 3;
			decimal[] distr = { Decimal.One, 0.5m, 0.25m, 0.05m };
			if (String.IsNullOrEmpty(aStr1) || String.IsNullOrEmpty(aStr2))
			{
				return (String.IsNullOrEmpty(aStr1) && String.IsNullOrEmpty(aStr2)) ? Decimal.One : Decimal.Zero;
			}

			string str1 = aStr1.Trim();
			string str2 = aStr2.Trim();

			long strAsInt1, strAsInt2;
			if (Int64.TryParse(str1, out strAsInt1) && Int64.TryParse(str2, out strAsInt2))
			{
				return (strAsInt1 == strAsInt2 ? Decimal.One : Decimal.Zero);
			}

			int length = Math.Max(str1.Length, str2.Length);
			if (length == 0) return Decimal.One;
			int diff = Math.Abs(str1.Length - str2.Length);
			if (diff > maxDiffCount) return Decimal.Zero;
			int differentCount = 0;
			for (int i = 0; i < length; i++)
			{
				if (i < str1.Length && i < str2.Length)
				{
					bool match = (aCaseSensitive) ? (str2[i].CompareTo(str2[i]) == 0) : (Char.ToLower(str2[i]).CompareTo(Char.ToLower(str1[i])) == 0);
					if (!match)
						differentCount++;
				}
				else
				{
					differentCount++;
				}
				if (differentCount > maxDiffCount) return Decimal.Zero;
			}
			//Compencate rounding

			result = distr[differentCount];
			return result;
		}

		public static Pair<DateTime, DateTime> GetDateRangeForMatch(CABankStatementDetail aDetail, IMatchSettings aSettings)
		{
			DateTime tranDateStart = aDetail.TranEntryDate ?? aDetail.TranDate.Value;
			DateTime tranDateEnd = aDetail.TranEntryDate ?? aDetail.TranDate.Value;
			bool isReceipt = (aDetail.DrCr == CADrCr.CADebit);
			tranDateStart = tranDateStart.AddDays(-(isReceipt ? aSettings.ReceiptTranDaysBefore.Value : aSettings.DisbursementTranDaysBefore.Value));
			tranDateEnd = tranDateEnd.AddDays((isReceipt ? aSettings.ReceiptTranDaysAfter.Value : aSettings.DisbursementTranDaysAfter.Value));
			if (tranDateEnd < tranDateStart)
			{
				DateTime swap = tranDateStart;
				tranDateStart = tranDateEnd;
				tranDateEnd = swap;
			}
			return new Pair<DateTime, DateTime>(tranDateStart, tranDateEnd);
		}
	}

	public static class StatementApplicationBalances
	{

        public static void CalculateBalancesAR<TInvoice>(PXGraph graph, PXSelectBase<CurrencyInfo> curyInfoSelect, ARAdjust adj, TInvoice invoice, bool isCalcRGOL, bool DiscOnDiscDate)
            where TInvoice : IInvoice
        {
            var customer = new PXSetup<Customer, Where<Customer.bAccountID, Equal<Optional<CAApplicationStatementDetail.payeeBAccountID>>>>(graph);
			Customer currentCustomer = customer.Current;
			if (currentCustomer == null)
			{
				currentCustomer = PXSelect<Customer, Where<Customer.bAccountID, Equal<Optional<CABankTran.payeeBAccountID>>>>.Select(graph);
			}
            PaymentEntry.CalcBalances<TInvoice, ARAdjust>(curyInfoSelect, adj.AdjgCuryInfoID, adj.AdjdCuryInfoID, invoice, adj);
            if (DiscOnDiscDate)
            {
                PaymentEntry.CalcDiscount<TInvoice, ARAdjust>(adj.AdjgDocDate, invoice, adj);
            }
            PaymentEntry.WarnDiscount<TInvoice, ARAdjust>(graph, adj.AdjgDocDate, invoice, adj);

			CurrencyInfo pay_info = curyInfoSelect.Select(adj.AdjgCuryInfoID);
			CurrencyInfo vouch_info = curyInfoSelect.Select(adj.AdjdCuryInfoID);

			if (vouch_info != null && string.Equals(pay_info.CuryID, vouch_info.CuryID) == false)
			{
				adj.AdjdCuryRate = Math.Round((vouch_info.CuryMultDiv == "M" ? (decimal)vouch_info.CuryRate : 1 / (decimal)vouch_info.CuryRate) * (pay_info.CuryMultDiv == "M" ? 1 / (decimal)pay_info.CuryRate : (decimal)pay_info.CuryRate), 8, MidpointRounding.AwayFromZero);
			}
			else
			{
				adj.AdjdCuryRate = 1m;
			}

			if (currentCustomer != null && currentCustomer.SmallBalanceAllow == true && adj.AdjgDocType != ARDocType.Refund && adj.AdjdDocType != ARDocType.CreditMemo)
            {
                decimal payment_smallbalancelimit;
                CurrencyInfo payment_info = curyInfoSelect.Select(adj.AdjgCuryInfoID);
				PXDBCurrencyAttribute.CuryConvCury(curyInfoSelect.Cache, payment_info, currentCustomer.SmallBalanceLimit ?? 0m, out payment_smallbalancelimit);
                adj.CuryWOBal = payment_smallbalancelimit;
				adj.WOBal = currentCustomer.SmallBalanceLimit;
            }
            else
            {
                adj.CuryWOBal = 0m;
                adj.WOBal = 0m;
            }

            PaymentEntry.AdjustBalance<ARAdjust>(curyInfoSelect, adj);
            if (isCalcRGOL && (adj.Voided != true))
            {
                PaymentEntry.CalcRGOL<TInvoice, ARAdjust>(curyInfoSelect, invoice, adj);
                adj.RGOLAmt = (bool)adj.ReverseGainLoss ? -1.0m * adj.RGOLAmt : adj.RGOLAmt;
            }
        }

		public static void UpdateBalance(PXGraph graph, PXSelectBase<CurrencyInfo> curyInfoSelect, CABankStatementDetail currentDetail, CABankStatementAdjustment adj, bool isCalcRGOL)
		{
			if (currentDetail.OrigModule == GL.BatchModule.AP)
			{
				foreach (PXResult<APInvoice, CurrencyInfo> res in PXSelectJoin<APInvoice, InnerJoin<CurrencyInfo,
					On<CurrencyInfo.curyInfoID, Equal<APInvoice.curyInfoID>>>,
					Where<APInvoice.docType, Equal<Required<APInvoice.docType>>,
						And<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>>.Select(graph, adj.AdjdDocType, adj.AdjdRefNbr))
				{
					APInvoice invoice = (APInvoice)res;

					APAdjust adjustment = new APAdjust();
					adjustment.AdjdRefNbr = adj.AdjdRefNbr;
					adjustment.AdjdDocType = adj.AdjdDocType;
					CopyToAdjust(adjustment, adj);

					APPaymentEntry.CalcBalances<APInvoice>(curyInfoSelect, adjustment, invoice, isCalcRGOL, true);

					CopyToBankAdjustment(adj, adjustment);
					adj.AdjdCuryRate = adjustment.AdjdCuryRate;

				}
			}
			else if (currentDetail.OrigModule == GL.BatchModule.AR)
			{

				foreach (ARInvoice invoice in PXSelect<ARInvoice, Where<ARInvoice.customerID, Equal<Required<ARInvoice.customerID>>,
					And<ARInvoice.docType, Equal<Required<ARInvoice.docType>>,
					And<ARInvoice.refNbr, Equal<Required<ARInvoice.refNbr>>>>>>.Select(graph, adj.VendorID, adj.AdjdDocType, adj.AdjdRefNbr))
				{
					ARAdjust adjustment = new ARAdjust();
					CopyToAdjust(adjustment, adj);
					adjustment.AdjdRefNbr = adj.AdjdRefNbr;
					adjustment.AdjdDocType = adj.AdjdDocType;

					CalculateBalancesAR(graph, curyInfoSelect, adjustment, invoice, isCalcRGOL, false);

					CopyToBankAdjustment(adj, adjustment);
				}
			}
		}

		public static void PopulateAdjustmentFieldsAP(PXGraph graph, PXSelectBase<CurrencyInfo> curyInfoSelect, CABankStatementDetail currentDetail, CABankStatementAdjustment adj)
		{
			foreach (PXResult<APInvoice, CurrencyInfo> res in PXSelectJoin<APInvoice, InnerJoin<CurrencyInfo,
				On<CurrencyInfo.curyInfoID, Equal<APInvoice.curyInfoID>>>,
				Where<APInvoice.docType, Equal<Required<APInvoice.docType>>,
					And<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>>.Select(graph, adj.AdjdDocType, adj.AdjdRefNbr))
			{
				CurrencyInfo info = (CurrencyInfo)res;
				CurrencyInfo info_copy = null;
				APInvoice invoice = (APInvoice)res;

				if (adj.AdjdDocType == APDocType.Prepayment)
				{
					//Prepayment cannot have RGOL
					info = new CurrencyInfo();
					info.CuryInfoID = currentDetail.CuryInfoID;
					info_copy = info;
				}
				else
				{
					info_copy = PXCache<CurrencyInfo>.CreateCopy((CurrencyInfo)res);
					info_copy.CuryInfoID = null;
					info_copy = (CurrencyInfo)curyInfoSelect.Cache.Insert(info_copy);
					info_copy.SetCuryEffDate(curyInfoSelect.Cache, currentDetail.TranDate);
				}

				adj.VendorID = invoice.VendorID;
				adj.AdjdBranchID = invoice.BranchID;
				adj.AdjdDocDate = invoice.DocDate;
				adj.AdjdFinPeriodID = invoice.FinPeriodID;
				adj.AdjgCuryInfoID = currentDetail.CuryInfoID;
				adj.AdjdCuryInfoID = info_copy.CuryInfoID;
				adj.AdjdOrigCuryInfoID = info.CuryInfoID;
				adj.AdjgDocDate = currentDetail.TranDate;
				adj.AdjdAPAcct = invoice.APAccountID;
				adj.AdjdAPSub = invoice.APSubID;
				adj.AdjgBalSign = (currentDetail.DocType == APDocType.Check && currentDetail.DocType == APDocType.DebitAdj
					|| currentDetail.DocType == APDocType.VoidCheck && currentDetail.DocType == APDocType.DebitAdj ? -1m : 1m);

				APAdjust adjustment = new APAdjust();
				adjustment.AdjdRefNbr = adj.AdjdRefNbr;
				adjustment.AdjdDocType = adj.AdjdDocType;
				adjustment.AdjdAPAcct = invoice.APAccountID;
				adjustment.AdjdAPSub = invoice.APSubID;
				StatementApplicationBalances.CopyToAdjust(adjustment, adj);

				if (currentDetail.DrCr == CADrCr.CACredit)
				{
					adjustment.AdjgDocType = APDocType.Prepayment;
				}
				else
				{
					adjustment.AdjgDocType = APDocType.Refund;
				}

				APPaymentEntry.CalcBalances<APInvoice>(curyInfoSelect, adjustment, invoice, false, true);

				decimal? CuryApplDiscAmt = (adjustment.AdjgDocType == APDocType.DebitAdj) ? 0m : adjustment.CuryDiscBal;
				decimal? CuryApplAmt = adjustment.CuryDocBal - adjustment.CuryWhTaxBal - CuryApplDiscAmt;
				decimal? CuryUnappliedBal = currentDetail.CuryUnappliedBal;

				if (currentDetail != null && adjustment.AdjgBalSign < 0m)
				{
					if (CuryUnappliedBal < 0m)
					{
						CuryApplAmt = Math.Min((decimal)CuryApplAmt, Math.Abs((decimal)CuryUnappliedBal));
					}
				}
				else if (currentDetail != null && CuryUnappliedBal > 0m && adjustment.AdjgBalSign > 0m && CuryUnappliedBal < CuryApplDiscAmt)
				{
					CuryApplAmt = CuryUnappliedBal;
					CuryApplDiscAmt = 0m;
				}
				else if (currentDetail != null && CuryUnappliedBal > 0m && adj.AdjgBalSign > 0m)
				{
					CuryApplAmt = Math.Min((decimal)CuryApplAmt, (decimal)CuryUnappliedBal);
				}
				else if (currentDetail != null && CuryUnappliedBal <= 0m && currentDetail.CuryOrigDocAmt > 0)
				{
					CuryApplAmt = 0m;
				}

				adjustment.CuryAdjgAmt = CuryApplAmt;
				adjustment.CuryAdjgDiscAmt = CuryApplDiscAmt;
				adjustment.CuryAdjgWhTaxAmt = adjustment.CuryWhTaxBal;

				APPaymentEntry.CalcBalances<APInvoice>(curyInfoSelect, adjustment, invoice, true, true);

				StatementApplicationBalances.CopyToBankAdjustment(adj, adjustment);
				adj.AdjdCuryRate = adjustment.AdjdCuryRate;
			}
		}

		public static void PopulateAdjustmentFieldsAR(PXGraph graph, PXSelectBase<CurrencyInfo> curyInfoSelect, CABankStatementDetail currentDetail, CABankStatementAdjustment adj)
		{
			foreach (PXResult<ARInvoice, CurrencyInfo> res in PXSelectJoin<ARInvoice, InnerJoin<CurrencyInfo,
				On<CurrencyInfo.curyInfoID, Equal<ARInvoice.curyInfoID>>>,
				Where<ARInvoice.docType, Equal<Required<ARInvoice.docType>>,
					And<ARInvoice.refNbr, Equal<Required<ARInvoice.refNbr>>>>>.Select(graph, adj.AdjdDocType, adj.AdjdRefNbr))
			{
				CurrencyInfo info_copy = PXCache<CurrencyInfo>.CreateCopy((CurrencyInfo)res);
				info_copy.CuryInfoID = null;
				info_copy = (CurrencyInfo)curyInfoSelect.Cache.Insert(info_copy);
				ARInvoice invoice = (ARInvoice)res;
				info_copy.SetCuryEffDate(curyInfoSelect.Cache, currentDetail.TranDate);

				adj.VendorID = invoice.CustomerID;
				adj.AdjgCuryInfoID = currentDetail.CuryInfoID;
				adj.AdjdCuryInfoID = info_copy.CuryInfoID;
				adj.AdjdOrigCuryInfoID = invoice.CuryInfoID;
				adj.AdjdBranchID = invoice.BranchID;
				adj.AdjdDocDate = invoice.DocDate;
				adj.AdjdFinPeriodID = invoice.FinPeriodID;
				adj.AdjdARAcct = invoice.ARAccountID;
				adj.AdjdARSub = invoice.ARSubID;
				adj.AdjgBalSign = (currentDetail.DocType == ARDocType.Payment && currentDetail.DocType == ARDocType.CreditMemo
					|| currentDetail.DocType == ARDocType.VoidPayment && currentDetail.DocType == ARDocType.CreditMemo ? -1m : 1m);

				ARAdjust adjustment = new ARAdjust();
				adjustment.AdjdRefNbr = adj.AdjdRefNbr;
				adjustment.AdjdDocType = adj.AdjdDocType;
				adjustment.AdjdARAcct = invoice.ARAccountID;
				adjustment.AdjdARSub = invoice.ARSubID;
				StatementApplicationBalances.CopyToAdjust(adjustment, adj);

				StatementApplicationBalances.CalculateBalancesAR(graph, curyInfoSelect, adjustment, invoice, false, true);

				decimal? CuryApplAmt = adjustment.CuryDocBal - adjustment.CuryDiscBal;
				decimal? CuryApplDiscAmt = adjustment.CuryDiscBal;
				decimal? CuryUnappliedBal = currentDetail.CuryUnappliedBal;


				if (currentDetail != null && adjustment.AdjgBalSign < 0m)
				{
					if (CuryUnappliedBal < 0m)
					{
						CuryApplAmt = Math.Min((decimal)CuryApplAmt, Math.Abs((decimal)CuryUnappliedBal));
					}
				}
				else if (currentDetail != null && CuryUnappliedBal > 0m && adjustment.AdjgBalSign > 0m)
				{
					CuryApplAmt = Math.Min((decimal)CuryApplAmt, (decimal)CuryUnappliedBal);

					if (CuryApplAmt + CuryApplDiscAmt < adjustment.CuryDocBal)
					{
						CuryApplDiscAmt = 0m;
					}
				}
				else if (currentDetail != null && CuryUnappliedBal <= 0m && ((CABankStatementDetail)currentDetail).CuryOrigDocAmt > 0)
				{
					CuryApplAmt = 0m;
					CuryApplDiscAmt = 0m;
				}

				adjustment.CuryAdjgAmt = CuryApplAmt;
				adjustment.CuryAdjgDiscAmt = CuryApplDiscAmt;
				adjustment.CuryAdjgWOAmt = 0m;

				StatementApplicationBalances.CalculateBalancesAR(graph, curyInfoSelect, adjustment, invoice, true, true);

				StatementApplicationBalances.CopyToBankAdjustment(adj, adjustment);
			}
		}

		public static CABankStatementAdjustment CopyToBankAdjustment(CABankStatementAdjustment bankAdj, IAdjustment iAdjust)
		{
			bankAdj.AdjgCuryInfoID = iAdjust.AdjgCuryInfoID;
			bankAdj.AdjdCuryInfoID = iAdjust.AdjdCuryInfoID;
			bankAdj.AdjgDocDate = iAdjust.AdjgDocDate;
			bankAdj.DocBal = iAdjust.DocBal;
			bankAdj.CuryDocBal = iAdjust.CuryDocBal;
			bankAdj.CuryDiscBal = iAdjust.CuryDiscBal;
			bankAdj.CuryWhTaxBal = iAdjust.CuryWhTaxBal;
			bankAdj.CuryAdjgAmt = iAdjust.CuryAdjgAmt;
			bankAdj.CuryAdjgDiscAmt = iAdjust.CuryAdjgDiscAmt;
			bankAdj.CuryAdjgWhTaxAmt = iAdjust.CuryAdjgWhTaxAmt;
			bankAdj.AdjdOrigCuryInfoID = iAdjust.AdjdOrigCuryInfoID;
			return bankAdj;
		}

		public static IAdjustment CopyToAdjust(IAdjustment iAdjust, CABankStatementAdjustment bankAdj)
		{
			iAdjust.AdjgCuryInfoID = bankAdj.AdjgCuryInfoID;
			iAdjust.AdjdCuryInfoID = bankAdj.AdjdCuryInfoID;
			iAdjust.AdjgDocDate = bankAdj.AdjgDocDate;
			iAdjust.DocBal = bankAdj.DocBal;
			iAdjust.CuryDocBal = bankAdj.CuryDocBal;
			iAdjust.CuryDiscBal = bankAdj.CuryDiscBal;
			iAdjust.CuryWhTaxBal = bankAdj.CuryWhTaxBal;
			iAdjust.CuryAdjgAmt = bankAdj.CuryAdjgAmt;
			iAdjust.CuryAdjgDiscAmt = bankAdj.CuryAdjgDiscAmt;
			iAdjust.CuryAdjgWhTaxAmt = bankAdj.CuryAdjgWhTaxAmt;
			iAdjust.AdjdOrigCuryInfoID = bankAdj.AdjdOrigCuryInfoID;
			return iAdjust;
		}

	}
}
