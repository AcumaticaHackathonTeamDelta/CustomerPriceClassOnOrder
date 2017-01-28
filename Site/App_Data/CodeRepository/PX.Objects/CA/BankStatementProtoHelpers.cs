using System;
using System.Collections;
using System.Collections.Generic;
using PX.Data;
using PX.Common;
using PX.Data.EP;
using PX.Objects.GL;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.CM;
using PX.Objects.CA.BankStatementHelpers;


namespace PX.Objects.CA.BankStatementProtoHelpers
{
	[Serializable]
	[PXHidden]
	public partial class CABankTranDocRef : PX.Data.IBqlTable, IBankMatchRelevance
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
		#region TranID
		public abstract class refNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _TranID;
		public virtual Int32? TranID
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
		#region DocModule
		public abstract class docModule : PX.Data.IBqlField
		{
		}
		protected String _DocModule;
		[PXDBString(2, IsFixed = true)]
		[PXStringList(new string[] { GL.BatchModule.AP, GL.BatchModule.AR }, new string[] { "AP", "AR" })]
		public virtual String DocModule
		{
			get
			{
				return this._DocModule;
			}
			set
			{
				this._DocModule = value;
			}
		}
		#endregion
		#region DocType
		public abstract class docType : PX.Data.IBqlField
		{
		}
		protected String _DocType;
		[PXDBString(3, IsFixed = true, InputMask = "")]
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
		#region DocRefNbr
		public abstract class docRefNbr : PX.Data.IBqlField
		{
		}
		protected String _DocRefNbr;
		[PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		public virtual String DocRefNbr
		{
			get
			{
				return this._DocRefNbr;
			}
			set
			{
				this._DocRefNbr = value;
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

		public void Copy(CABankTranInvoiceMatch aSrc)
		{
			DocModule = aSrc.OrigModule;
			DocType = aSrc.OrigTranType;
			DocRefNbr = aSrc.OrigRefNbr;
			ReferenceID = aSrc.ReferenceID;
		}

		public void Copy(CATran aSrc)
		{
			this.CashAccountID = aSrc.CashAccountID;
			this.CATranID = aSrc.TranID;
			this.ReferenceID = aSrc.ReferenceID;
		}
		public void Copy(CABankTran aSrc)
		{
			this.TranID = aSrc.TranID;
			this.CashAccountID = aSrc.CashAccountID;
		}

	}

	[Serializable]
	public partial class CABankTranInvoiceMatch : PX.Data.IBqlTable
	{
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
		[PXDecimal(6)]
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
		#region OrigModule
		public abstract class origModule : PX.Data.IBqlField
		{
		}
		protected String _OrigModule;
		[PXDBString(2, IsFixed = true, IsKey = true)]
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
		[PXDBString(3, IsFixed = true, IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Type")]
		[CA.CAAPARTranType.ListByModuleRestricted(typeof(CABankTranInvoiceMatch.origModule))]
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
		[PXDBString(15, IsUnicode = true, IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Reference Nbr.")]
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
		[PXUIField(DisplayName = "Ext. Ref. Nbr.", Visibility = PXUIVisibility.Visible)]
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
		#region TranDate
		public abstract class tranDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _TranDate;
		[PXDBDate()]
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
		[PXUIField(DisplayName = CR.Messages.BAccountName, Visibility = PXUIVisibility.Visible)]
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
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.Visible)]
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
		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected String _FinPeriodID;
		[FinPeriodSelector(typeof(CATran.tranDate))]
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
		#region Released
		public abstract class released : PX.Data.IBqlField
		{
		}
		protected Boolean? _Released;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Released", Enabled = false)]
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
		#region CuryTranAmt
		public abstract class curyTranAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryTranAmt;
		[PXDBCurrency(typeof(CATran.curyInfoID), typeof(CATran.tranAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount", Visibility = PXUIVisibility.Visible)]
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
		#region DueDate
		public abstract class dueDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _DueDate;
		[PXDBDate()]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Doc. Date")]
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
		public void Copy(ARInvoice aSrc)
		{
			this.CashAccountID = null;
			this.TranDate = aSrc.DocDate;
			this.TranDesc = aSrc.DocDesc;
			this.ReferenceID = aSrc.CustomerID;
			this.DrCr = CADrCr.CADebit;
			this.FinPeriodID = aSrc.FinPeriodID;
			this.ExtRefNbr = aSrc.InvoiceNbr;
			this.CuryInfoID = aSrc.CuryInfoID;
			this.CuryID = aSrc.CuryID;
			this.CuryTranAmt = aSrc.CuryDocBal;
			this.TranAmt = aSrc.DocBal;
			this.Released = aSrc.Released;
			this.OrigTranType = aSrc.DocType;
			this.OrigRefNbr = aSrc.RefNbr;
			this.OrigModule = GL.BatchModule.AR;
		}
		public void Copy(APInvoice aSrc)
		{
			this.CashAccountID = null;
			this.TranDate = aSrc.DocDate;
			this.TranDesc = aSrc.DocDesc;
			this.ReferenceID = aSrc.VendorID;
			this.DrCr = CADrCr.CADebit;
			this.FinPeriodID = aSrc.FinPeriodID;
			this.ExtRefNbr = aSrc.InvoiceNbr;
			this.CuryInfoID = aSrc.CuryInfoID;
			this.CuryID = aSrc.CuryID;
			this.CuryTranAmt = aSrc.CuryDocBal;
			this.TranAmt = aSrc.DocBal;
			this.Released = aSrc.Released;
			this.OrigTranType = aSrc.DocType;
			this.OrigRefNbr = aSrc.RefNbr;
			this.OrigModule = GL.BatchModule.AP;
		}
		public void Copy(Light.ARInvoice aSrc)
		{
			this.CashAccountID = null;
			this.TranDate = aSrc.DocDate;
			this.TranDesc = aSrc.DocDesc;
			this.ReferenceID = aSrc.CustomerID;
			this.DrCr = CADrCr.CADebit;
			this.FinPeriodID = aSrc.FinPeriodID;
			this.ExtRefNbr = aSrc.InvoiceNbr;
			this.CuryInfoID = aSrc.CuryInfoID;
			this.CuryID = aSrc.CuryID;
			this.CuryTranAmt = aSrc.CuryDocBal;
			this.TranAmt = aSrc.DocBal;
			this.Released = aSrc.Released;
			this.OrigTranType = aSrc.DocType;
			this.OrigRefNbr = aSrc.RefNbr;
			this.OrigModule = GL.BatchModule.AR;
		}
		public void Copy(Light.APInvoice aSrc)
		{
			this.CashAccountID = null;
			this.TranDate = aSrc.DocDate;
			this.TranDesc = aSrc.DocDesc;
			this.ReferenceID = aSrc.VendorID;
			this.DrCr = CADrCr.CADebit;
			this.FinPeriodID = aSrc.FinPeriodID;
			this.ExtRefNbr = aSrc.InvoiceNbr;
			this.CuryInfoID = aSrc.CuryInfoID;
			this.CuryID = aSrc.CuryID;
			this.CuryTranAmt = aSrc.CuryDocBal;
			this.TranAmt = aSrc.DocBal;
			this.Released = aSrc.Released;
			this.OrigTranType = aSrc.DocType;
			this.OrigRefNbr = aSrc.RefNbr;
			this.OrigModule = GL.BatchModule.AP;
		}
		public void Copy(CATran aSrc)
		{

		}
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
			   typeof(GeneralInvoice.dueDate))
		{
			this._BatchModule = BatchModule;
		}

		protected virtual IEnumerable GetRecords()
		{
			PXCache cache = this._Graph.Caches[BqlCommand.GetItemType(this._BatchModule)];
			PXCache adjustments = this._Graph.Caches[typeof(CABankTranAdjustment)];
			PXCache bankTrans = this._Graph.Caches[typeof(CABankTran)];
			CABankTran currentBankTran = (CABankTran)bankTrans.Current;
			object current = null;
			foreach (object item in PXView.Currents)
			{
				if (item != null && (item.GetType() == typeof(CABankTranAdjustment) || item.GetType().IsSubclassOf(typeof(CABankTranAdjustment))))
				{
					current = item;
					break;
				}
			}
			if (current == null)
			{
				current = adjustments.Current;
			}

			CABankTranAdjustment currentAdj = current as CABankTranAdjustment;
			if (cache.Current == null) yield break;
			string tranModule = (string)cache.GetValue(cache.Current, this._BatchModule.Name);
			switch (tranModule)
			{
				case GL.BatchModule.AP:
					foreach (APAdjust.APInvoice apInvoice in GetRecordsAP(currentAdj, currentBankTran, adjustments, this._Graph))
					{
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
					foreach (ARAdjust.ARInvoice arInvoice in GetRecordsAR(currentAdj, currentBankTran, adjustments, this._Graph))
					{
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

		public static IEnumerable<ARAdjust.ARInvoice> GetRecordsAR(CABankTranAdjustment currentAdj, CABankTran currentBankTran, PXCache adjustments, PXGraph graph)
		{
			foreach (ARAdjust.ARInvoice result in GetRecordsAR(currentAdj.AdjdDocType, currentAdj.TranID, currentAdj.AdjNbr, currentBankTran,adjustments,graph))
			{
				yield return result;
			}
		}
		public static IEnumerable<APAdjust.APInvoice> GetRecordsAP(CABankTranAdjustment currentAdj, CABankTran currentBankTran, PXCache adjustments, PXGraph graph)
		{
			foreach (APAdjust.APInvoice result in GetRecordsAP(currentAdj.AdjdDocType, currentAdj.TranID, currentAdj.AdjNbr, currentBankTran, adjustments, graph))
			{
				yield return result;
			}
		}
		public static IEnumerable<ARAdjust.ARInvoice> GetRecordsAR(string AdjdDocType, int? TranID, int? AdjNbr, CABankTran currentBankTran, PXCache adjustments, PXGraph graph)
		{
			foreach (ARAdjust.ARInvoice result in PXSelectJoin<ARAdjust.ARInvoice,
									LeftJoin<ARAdjust, On<ARAdjust.adjdDocType, Equal<ARAdjust.ARInvoice.docType>, And<ARAdjust.adjdRefNbr, Equal<ARAdjust.ARInvoice.refNbr>,
										And<ARAdjust.released, Equal<boolFalse>, And<ARAdjust.voided, Equal<boolFalse>, And<Where<ARAdjust.adjgDocType, NotEqual<Required<CABankTranAdjustment.adjdDocType>>>>>>>>,
									LeftJoin<CABankTranAdjustment, On<CABankTranAdjustment.adjdModule, Equal<BatchModule.moduleAR>, 
									And<CABankTranAdjustment.adjdDocType, Equal<ARAdjust.ARInvoice.docType>,
										And<CABankTranAdjustment.adjdRefNbr, Equal<ARAdjust.ARInvoice.refNbr>,
										And<CABankTranAdjustment.released, Equal<boolFalse>,
										And<Where<CABankTranAdjustment.tranID,
											NotEqual<Required<CABankTranAdjustment.tranID>>,
											Or<Required<CABankTranAdjustment.adjNbr>, IsNull, 
											Or<CABankTranAdjustment.adjNbr, NotEqual<Required<CABankTranAdjustment.adjNbr>>>>>>>>>>,
									LeftJoin<CABankTran, On<CABankTran.tranID, Equal<CABankTranAdjustment.tranID>>>>>,
									Where<ARAdjust.ARInvoice.customerID, Equal<Required<CABankTran.payeeBAccountID>>,
									And<ARAdjust.ARInvoice.docType, Equal<Required<CABankTranAdjustment.adjdDocType>>,
									And<ARAdjust.ARInvoice.released, Equal<boolTrue>,
									And<ARAdjust.ARInvoice.openDoc, Equal<boolTrue>,
									And<ARAdjust.adjgRefNbr, IsNull,
									And<ARAdjust.ARInvoice.pendingPPD, NotEqual<True>,
									And<Where<CABankTranAdjustment.adjdRefNbr, IsNull, Or<CABankTran.origModule, NotEqual<BatchModule.moduleAR>>>>>>>>>>>
									.Select(graph, AdjdDocType, TranID, AdjNbr, AdjNbr, currentBankTran.PayeeBAccountID, AdjdDocType))
			{
				if (ShouldSkipRecord(result.DocType, result.RefNbr, TranID, AdjNbr, currentBankTran, adjustments, graph)) continue;
				yield return result;
			}
		}
		public static IEnumerable<APAdjust.APInvoice> GetRecordsAP(string AdjdDocType, int? TranID, int? AdjNbr, CABankTran currentBankTran, PXCache adjustments, PXGraph graph)
		{
			foreach (APAdjust.APInvoice result in PXSelectJoin<APAdjust.APInvoice,
									   LeftJoin<APAdjust, On<APAdjust.adjdDocType, Equal<APAdjust.APInvoice.docType>,
										   And<APAdjust.adjdRefNbr, Equal<APAdjust.APInvoice.refNbr>, And<APAdjust.released, Equal<boolFalse>>>>,
									   LeftJoin<CABankTranAdjustment, On<CABankTranAdjustment.adjdModule, Equal<BatchModule.moduleAP>, 
										And<CABankTranAdjustment.adjdDocType, Equal<APAdjust.APInvoice.docType>,
										And<CABankTranAdjustment.adjdRefNbr, Equal<APAdjust.APInvoice.refNbr>, And<CABankTranAdjustment.released, Equal<boolFalse>,
											   And<Where<CABankTranAdjustment.tranID,
													NotEqual<Required<CABankTranAdjustment.tranID>>,
													Or<Required<CABankTranAdjustment.adjNbr>, IsNull, 
													Or<CABankTranAdjustment.adjNbr, NotEqual<Required<CABankTranAdjustment.adjNbr>>>>>>>>>>,
									   LeftJoin<AP.Standalone.APPayment, On<AP.Standalone.APPayment.docType, Equal<APAdjust.APInvoice.docType>,
										   And<AP.Standalone.APPayment.refNbr, Equal<APAdjust.APInvoice.refNbr>, And<
										   Where<AP.Standalone.APPayment.docType, Equal<APDocType.prepayment>, Or<AP.Standalone.APPayment.docType, Equal<APDocType.debitAdj>>>>>>,
									   LeftJoin<CABankTran, On<CABankTran.tranID, Equal<CABankTranAdjustment.tranID>>>>>>,
									   Where<APAdjust.APInvoice.vendorID, Equal<Optional<CABankTran.payeeBAccountID>>, And<APAdjust.APInvoice.docType, Equal<Optional<CABankTranAdjustment.adjdDocType>>,
									   And2<Where<APAdjust.APInvoice.released, Equal<True>, Or<APAdjust.APInvoice.prebooked, Equal<True>>>, And<APAdjust.APInvoice.openDoc, Equal<boolTrue>,
									   And2<Where<CABankTranAdjustment.adjdRefNbr, IsNull, Or<CABankTran.origModule, NotEqual<BatchModule.moduleAP>>>, And<APAdjust.adjgRefNbr, IsNull,
										  And2<Where<AP.Standalone.APPayment.refNbr, IsNull, And<Required<CABankTran.docType>, NotEqual<APDocType.refund>,
										   Or<AP.Standalone.APPayment.refNbr, IsNotNull, And<Required<CABankTran.docType>, Equal<APDocType.refund>,
										   Or<AP.Standalone.APPayment.docType, Equal<APDocType.debitAdj>, And<Required<CABankTran.docType>, Equal<APDocType.check>,
										   Or<AP.Standalone.APPayment.docType, Equal<APDocType.debitAdj>, And<Required<CABankTran.docType>, Equal<APDocType.voidCheck>>>>>>>>>,
										And<Where<APAdjust.APInvoice.docDate, LessEqual<Required<CABankTran.tranDate>>, Or<Current<APSetup.earlyChecks>, Equal<True>, And<Required<CABankTran.docType>, NotEqual<APDocType.refund>>>>>>>>>>>>>
								 .Select(graph, TranID, AdjNbr, AdjNbr, currentBankTran.PayeeBAccountID, AdjdDocType, currentBankTran.DocType, currentBankTran.DocType, currentBankTran.DocType, currentBankTran.DocType, currentBankTran.TranDate, currentBankTran.DocType))
			{
				if (ShouldSkipRecord(result.DocType, result.RefNbr, TranID, AdjNbr, currentBankTran, adjustments, graph)) continue;
				yield return result;
			}
		}
		protected static bool ShouldSkipRecord(string docType, string refNbr,int? TranID, int? AdjNbr, CABankTran currentBankTran, PXCache adjustments, PXGraph graph)
		{
			foreach (CABankTranAdjustment adj in adjustments.Inserted)
			{
				if ((adj.AdjdDocType == docType && adj.AdjdRefNbr == refNbr && (adj.TranID != TranID || adj.AdjNbr != AdjNbr)
				|| (adj.AdjdDocType == docType && adj.AdjdRefNbr == refNbr)))
				{
					return true;
				}
			}
			CABankTranMatch match = PXSelect<CABankTranMatch, Where<CABankTranMatch.docModule, Equal<GL.BatchModule.moduleAR>,
				And<CABankTranMatch.docType, Equal<Required<CABankTranMatch.docType>>,
				And<CABankTranMatch.docRefNbr, Equal<Required<CABankTranMatch.docRefNbr>>,
				And<CABankTranMatch.tranID, NotEqual<Required<CABankTran.tranID>>>>>>>.Select(graph, docType, refNbr, currentBankTran.TranID);
			if (match != null) return true;
			return false;
		}
	}

	public static class StatementsMatchingProto
	{
		public static IEnumerable FindDetailMatches(CABankTransactionsMaint graph, PXCache DetailsCache, CABankTran aDetail, IMatchSettings aSettings, decimal aRelevanceTreshold, CATranExt[] aBestMatches)
		{
			List<CATranExt> matchList = new List<CATranExt>();
			bool isPayeeMatched = (aDetail.PayeeMatched == true);
			bool hasBaccount = aDetail.PayeeBAccountID.HasValue;
			bool hasLocation = aDetail.PayeeLocationID.HasValue;
			if (!aDetail.TranEntryDate.HasValue && !aDetail.TranDate.HasValue) return matchList;
			Pair<DateTime, DateTime> tranDateRange = GetDateRangeForMatch(aDetail, aSettings);
			CashAccount cashAcct = PXSelect<CashAccount, Where<CashAccount.cashAccountID, Equal<Required<CashAccount.cashAccountID>>>>.Select(graph, aDetail.CashAccountID);
			CASetup setup = PXSelect<CASetup>.Select(graph);
			string curyID = aDetail.CuryID; //Need to reconsider. 
			CATranExt bestMatch = null;
			int bestMatchesNumber = aBestMatches != null ? aBestMatches.Length : 0;

			if (cashAcct.MatchToBatch == true)
			{
				bestMatch = MatchToBatch(graph, aDetail, aSettings, aRelevanceTreshold, aBestMatches, matchList, tranDateRange, curyID, bestMatch, bestMatchesNumber, setup.AllowMatchingToUnreleasedBatch ?? false);
			}
			var cmd = new PXSelectReadonly2<CATranExt,
					LeftJoin<Light.BAccount, On<Light.BAccount.bAccountID, Equal<CATranExt.referenceID>>,
					LeftJoin<CATran2, On<CATran2.cashAccountID, Equal<CATranExt.cashAccountID>,
						And<CATran2.voidedTranID, Equal<CATranExt.tranID>,
						And<True, Equal<Required<CASetup.skipVoided>>>>>,
					LeftJoin<CABankTranMatch2, On<CABankTranMatch2.cATranID, Equal<CATranExt.tranID>,
						And<CABankTranMatch2.tranType, Equal<Required<CABankTran.tranType>>>>,
					LeftJoin<CABatchDetail, On<CABatchDetail.origModule, Equal<CATranExt.origModule>,
						And<CABatchDetail.origDocType, Equal<CATranExt.origTranType>,
						And<CABatchDetail.origRefNbr, Equal<CATranExt.origRefNbr>>>>,
					LeftJoin<CABankTranMatch, On<CABankTranMatch.docModule, Equal<BatchModule.moduleAP>,
						And<CABankTranMatch.docType, Equal<CATranType.cABatch>,
						And<CABankTranMatch.docRefNbr, Equal<CABatchDetail.batchNbr>,
						And<CABankTranMatch.tranType, Equal<Required<CABankTran.tranType>>>>>>>>>>>,
					Where<CATranExt.cashAccountID, Equal<Required<CABankTran.cashAccountID>>,
						And<Where<CATranExt.tranDate, Between<Required<CATranExt.tranDate>, Required<CATranExt.tranDate>>,
						And<CATranExt.curyID, Equal<Required<CATranExt.curyID>>,
						And<CATranExt.curyTranAmt, Equal<Required<CATranExt.curyTranAmt>>>>>>>>(graph);
			if (aSettings.SkipVoided == true)
			{
				cmd.WhereAnd<Where<CATranExt.voidedTranID, IsNull, And<CATran2.tranID, IsNull>>>();
			}
			foreach (PXResult<CATranExt, Light.BAccount, CATran2, CABankTranMatch2, CABatchDetail> iRes in
				cmd.Select(aSettings.SkipVoided, aDetail.TranType, aDetail.TranType, aDetail.CashAccountID, tranDateRange.first, tranDateRange.second,
								curyID, aDetail.CuryTranAmt.Value))
			{
				CABatchDetail batchDetail = iRes;
				if (cashAcct.MatchToBatch == true && batchDetail != null && batchDetail.BatchNbr != null)//exclude transaction included to the batch
				{
					PXResultset<CABatchDetail> matches = PXSelectJoin<CABatchDetail,
					InnerJoin<CATran, On<CATran.origModule, Equal<CABatchDetail.origModule>,
						And<CATran.origTranType, Equal<CABatchDetail.origDocType>,
						And<CATran.origRefNbr, Equal<CABatchDetail.origRefNbr>>>>,
					InnerJoin<CABankTranMatch, On<CABankTranMatch.cATranID, Equal<CATran.tranID>,
						And<CABankTranMatch.tranType, Equal<Required<CABankTran.tranType>>>>>>,
					Where<CABatchDetail.batchNbr, Equal<Required<CABatch.batchNbr>>>>.Select(graph, aDetail.TranType, batchDetail.BatchNbr);
					if (matches == null || matches.Count < 1)//if there is no already matched transaction in that batch
					{
						continue;
					}
				}
				CATranExt iTran = iRes;
				Light.BAccount iPayee = iRes;
				//is this thing needed in new design?
				if (isPayeeMatched && hasBaccount && iTran.ReferenceID != aDetail.PayeeBAccountID) continue;
				iTran.ReferenceName = iPayee.AcctName;
				//Check updated in cache
				bool matched = false;

				CABankTranMatch match = (CABankTranMatch2)iRes;//existing match to CATran
				match = CheckMatchInCache(graph.TranMatch.Cache, iTran, match);
				if (match == null || match.TranID == null)
				{
					match = (CABankTranMatch)iRes;//existing match to batch
					match = CheckBatchMatchInCache(graph.TranMatch.Cache, batchDetail, match);
				}
				if (match != null && match.TranID != null)
				{
					if (match.TranID != aDetail.TranID)
					{
						continue;
					}
					matched = true;
				}
				iTran.MatchRelevance = graph.EvaluateMatching(aDetail, iTran, aSettings);
				iTran.IsMatched = matched;
				if (iTran.IsMatched == false && iTran.MatchRelevance < aRelevanceTreshold)
					continue;

				if (bestMatchesNumber > 0)
				{
					for (int i = 0; i < bestMatchesNumber; i++)
					{
						if ((aBestMatches[i] == null || aBestMatches[i].MatchRelevance < iTran.MatchRelevance))
						{
							for (int j = bestMatchesNumber - 1; j > i; j--)
							{
								aBestMatches[j] = aBestMatches[j - 1];
							}
							aBestMatches[i] = iTran;
							break;
						}
					}
				}
				else
				{
					if (bestMatch == null || bestMatch.MatchRelevance < iTran.MatchRelevance)
					{
						bestMatch = iTran;
					}
				}

				iTran.IsBestMatch = false;
				matchList.Add(iTran);
			}

			//adding matched transactions if they fell out of filters
			foreach (PXResult<CABankTranMatch, CATranExt> matches in PXSelectJoin<CABankTranMatch,
				LeftJoin<CATranExt, On<CABankTranMatch.cATranID, Equal<CATranExt.tranID>>>,
				Where<CABankTranMatch.tranID, Equal<Required<CABankTranMatch.tranID>>>>.Select(graph, aDetail.TranID))
			{
				CATranExt matchedTran = matches;
				CABankTranMatch match = matches;
				if (matchedTran != null && matchedTran.TranID != null)
				{
					if (matchList.Find((CATranExt tran) => { return tran.TranID == matchedTran.TranID; }) == null)
					{
						matchedTran.MatchRelevance = graph.EvaluateMatching(aDetail, matchedTran, aSettings);
						matchedTran.IsMatched = true;
						matchList.Add(matchedTran);
					}
				}
				else if (match.DocModule == BatchModule.AP && match.DocType == CATranType.CABatch)
				{
					CABatch batch = PXSelect<CABatch, Where<CABatch.batchNbr, Equal<Required<CABatch.batchNbr>>>>.Select(graph, match.DocRefNbr);
					if (batch != null && batch.BatchNbr != null)
					{
						if (matchList.Find((CATranExt tran) => { return tran.OrigModule == BatchModule.AP && tran.OrigRefNbr == batch.BatchNbr && tran.OrigTranType == CATranType.CABatch; }) == null)
						{
							matchedTran = new CATranExt();
							batch.CopyTo(matchedTran);
							matchedTran.MatchRelevance = graph.EvaluateMatching(aDetail, matchedTran, aSettings);
							matchedTran.IsMatched = true;
							matchList.Add(matchedTran);
						}
					}
				}
			}

			if (bestMatchesNumber > 0)
				bestMatch = aBestMatches[0];
			if (bestMatch != null)
			{
				bestMatch.IsBestMatch = true;
			}
			aDetail.CountMatches = matchList.Count;
			return matchList;
		}

		private static CABankTranMatch CheckBatchMatchInCache(PXCache cache, CABatchDetail detail, CABankTranMatch match)
		{
			if (match != null && match.TranID != null)
			{
				var status = cache.GetStatus(match);
				if (status == PXEntryStatus.Deleted) 
					match = null;
				else if (status == PXEntryStatus.Updated)
				{
					CABankTranMatch updatedMatch = (CABankTranMatch)cache.Locate(match);
					if(updatedMatch.DocRefNbr!=match.DocRefNbr || updatedMatch.DocType!=match.DocType || updatedMatch.DocModule!=match.DocModule)
					match = null;
				}
			}
			if ((match == null || match.TranID == null) && detail != null && detail.BatchNbr != null)
			{
				foreach (CABankTranMatch insertedMatch in cache.Inserted)
				{
					if (insertedMatch.DocRefNbr == detail.BatchNbr && insertedMatch.DocType == CATranType.CABatch && insertedMatch.DocModule == BatchModule.AP)
					{
						return insertedMatch;
					}
				}
				foreach (CABankTranMatch insertedMatch in cache.Updated)
				{
					if (insertedMatch.DocRefNbr == detail.BatchNbr && insertedMatch.DocType == CATranType.CABatch && insertedMatch.DocModule == BatchModule.AP)
					{
						return insertedMatch;
					}
				}
			}
			return match;
		}

		private static CABankTranMatch CheckMatchInCache(PXCache cache, CATranExt catran, CABankTranMatch match)
		{
			if (match != null && match.TranID != null)
			{
				var status=cache.GetStatus(match);
				if (status == PXEntryStatus.Deleted || (status == PXEntryStatus.Updated && ((CABankTranMatch)cache.Locate(match)).CATranID != catran.TranID))
					match = null;
			}
			if(match==null || match.TranID==null)
			{
				foreach (CABankTranMatch insertedMatch in cache.Inserted)
				{
					if (insertedMatch.CATranID == catran.TranID)
					{
						return insertedMatch;
					}
				}
				foreach (CABankTranMatch insertedMatch in cache.Updated)
				{
					if (insertedMatch.CATranID == catran.TranID)
					{
						return insertedMatch;
					}
				}
			}
			return match;
		}

		class CABatchWithBaccount
		{
			public CABatch Batch;
			public Int32? BaccountID;
		}
		private static CATranExt MatchToBatch(CABankTransactionsMaint graph, CABankTran aDetail, IMatchSettings aSettings, decimal aRelevanceTreshold, CATranExt[] aBestMatches, List<CATranExt> matchList, Pair<DateTime, DateTime> tranDateRange, string curyID, CATranExt bestMatch, int bestMatchesNumber, bool allowUnreleased)
		{
			List<CABatchWithBaccount> batches = new List<CABatchWithBaccount>();
			bool matchFound = false;
			bool referenceNotEqual = false;
			foreach (PXResult<CABatch, CABatchDetail, Light.APPayment, CABankTranMatch> iRes in
						 PXSelectJoin<CABatch,
							LeftJoin<CABatchDetail, On<CABatchDetail.batchNbr, Equal<CABatch.batchNbr>,
								And<CABatchDetail.origModule, Equal<BatchModule.moduleAP>>>,
							LeftJoin<Light.APPayment, On<Light.APPayment.docType, Equal<CABatchDetail.origDocType>,
								And<Light.APPayment.refNbr, Equal<CABatchDetail.origRefNbr>>>,
							LeftJoin<CABankTranMatch, On<CABankTranMatch.cATranID, Equal<Light.APPayment.cATranID>,
								And<CABankTranMatch.tranType, Equal<Required<CABankTran.tranType>>>>>>>,
							 Where<CABatch.cashAccountID, Equal<Required<CABatch.cashAccountID>>,
								And2<Where<CABatch.released, Equal<True>,Or<Required<CASetup.allowMatchingToUnreleasedBatch>, Equal<True>>>,
								And<Where<CABatch.tranDate, Between<Required<CABatch.tranDate>, Required<CABatch.tranDate>>,
								And<CABatch.curyID, Equal<Required<CABatch.curyID>>,
								And<CABatch.curyDetailTotal, Equal<Required<CABatch.curyDetailTotal>>>>>>>>>.
							Select(graph, aDetail.TranType, aDetail.CashAccountID, allowUnreleased, tranDateRange.first, tranDateRange.second,
								 curyID, -1 * aDetail.CuryTranAmt.Value))
			{
				CABankTranMatch existingMatch = iRes;
				CABatch batch = iRes;
				Light.APPayment payment = iRes;
				if (batches.Count == 0 || batches[batches.Count - 1].Batch.BatchNbr != batch.BatchNbr)
				{
					if (batches.Count > 0)
					{
						if (matchFound)
						{
							batches.RemoveAt(batches.Count - 1);
						}
						if (referenceNotEqual)
						{
							batches[batches.Count - 1].BaccountID = null;
						}
					}
					matchFound = false;
					referenceNotEqual = false;
					batches.Add(new CABatchWithBaccount() { Batch = batch, BaccountID = payment.VendorID });

				}
				if (existingMatch != null && existingMatch.TranID.HasValue)
				{
					matchFound = true;
				}
				if (batches.Count == 0 || batches[batches.Count - 1].BaccountID != payment.VendorID)
				{
					referenceNotEqual = true;
				}
			}
			if (batches.Count > 0)
			{
				if (referenceNotEqual)
				{
					batches[batches.Count - 1].BaccountID = null;
				}
				if (matchFound)
				{
					batches.RemoveAt(batches.Count - 1);
				}
			}
			foreach (CABatchWithBaccount batch in batches)
			{
				CATranExt iTran = new CATranExt();
				batch.Batch.CopyTo(iTran);
				iTran.ReferenceID = batch.BaccountID;
				//Check updated in cache
				bool matched = false;
				var matchedRows = PXSelect<CABankTranMatch, Where<CABankTranMatch.docRefNbr, Equal<Required<CABankTranMatch.docRefNbr>>,
					And<CABankTranMatch.docType, Equal<CATranType.cABatch>,
					And<CABankTranMatch.docModule, Equal<BatchModule.moduleAP>>>>>.Select(graph, batch.Batch.BatchNbr);
				if (matchedRows.Count != 0)
				{
					CABankTranMatch match = matchedRows;
					if (match.TranID != aDetail.TranID)
					{
						continue;
					}
					matched = true;
				}
				iTran.MatchRelevance = graph.EvaluateMatching(aDetail, iTran, aSettings);
				iTran.IsMatched = matched;
				if (iTran.IsMatched == false && iTran.MatchRelevance < aRelevanceTreshold)
					continue;

				if (bestMatchesNumber > 0)
				{
					for (int i = 0; i < bestMatchesNumber; i++)
					{
						if ((aBestMatches[i] == null || aBestMatches[i].MatchRelevance < iTran.MatchRelevance))
						{
							for (int j = bestMatchesNumber - 1; j > i; j--)
							{
								aBestMatches[j] = aBestMatches[j - 1];
							}
							aBestMatches[i] = iTran;
							break;
						}
					}
				}
				else
				{
					if (bestMatch == null || bestMatch.MatchRelevance < iTran.MatchRelevance)
					{
						bestMatch = iTran;
					}
				}

				iTran.IsBestMatch = false;
				matchList.Add(iTran);
			}
			return bestMatch;
		}
		public static decimal EvaluateMatching(CABankTransactionsMaint graph, CABankTran aDetail, CATran aTran, IMatchSettings aSettings)
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

		public static decimal CompareDate(CABankTran aDetail, CATran aTran, Double meanValue, Double sigma)
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

		public static decimal CompareRefNbr(CABankTransactionsMaint graph, CABankTran aDetail, CATran aTran, bool looseCompare)
		{
			if (looseCompare)
				return graph.EvaluateMatching(aDetail.ExtRefNbr, aTran.ExtRefNbr, false);
			else
				return graph.EvaluateTideMatching(aDetail.ExtRefNbr, aTran.ExtRefNbr, false);
		}

		public static decimal ComparePayee(CABankTransactionsMaint graph, CABankTran aDetail, CATran aTran)
		{
			return graph.EvaluateMatching(aDetail.PayeeName, aTran.ReferenceName, false);
		}

		public static Pair<DateTime, DateTime> GetDateRangeForMatch(CABankTran aDetail, IMatchSettings aSettings)
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

		public static void SetDocTypeList(PXCache cache, CABankTran Row)
		{
			CABankTran detail = Row;

			List<string> AllowedValues = new List<string>();
			List<string> AllowedLabels = new List<string>();

			if (detail.OrigModule == GL.BatchModule.AP)
			{
				if (detail.DocType == APDocType.Refund)
				{
					PXDefaultAttribute.SetDefault<CABankTranAdjustment.adjdDocType>(cache, APDocType.DebitAdj);
					PXStringListAttribute.SetList<CABankTranAdjustment.adjdDocType>(cache, null, new string[] { APDocType.DebitAdj, APDocType.Prepayment }, new string[] { AP.Messages.DebitAdj, AP.Messages.Prepayment });
				}
				else if (detail.DocType == APDocType.Prepayment)
				{
					PXDefaultAttribute.SetDefault<CABankTranAdjustment.adjdDocType>(cache, APDocType.Invoice);
					PXStringListAttribute.SetList<CABankTranAdjustment.adjdDocType>(cache, null, new string[] { APDocType.Invoice, APDocType.CreditAdj }, new string[] { AP.Messages.Invoice, AP.Messages.CreditAdj });
				}
				else if (detail.DocType == APDocType.Check)
				{
					PXDefaultAttribute.SetDefault<CABankTranAdjustment.adjdDocType>(cache, APDocType.Invoice);
					PXStringListAttribute.SetList<CABankTranAdjustment.adjdDocType>(cache, null, new string[] { APDocType.Invoice, APDocType.DebitAdj, APDocType.CreditAdj, APDocType.Prepayment }, new string[] { AP.Messages.Invoice, AP.Messages.DebitAdj, AP.Messages.CreditAdj, AP.Messages.Prepayment });
				}
				else
				{
					PXDefaultAttribute.SetDefault<CABankTranAdjustment.adjdDocType>(cache, APDocType.Invoice);
					PXStringListAttribute.SetList<CABankTranAdjustment.adjdDocType>(cache, null, new string[] { APDocType.Invoice, APDocType.CreditAdj, APDocType.Prepayment }, new string[] { AP.Messages.Invoice, AP.Messages.CreditAdj, AP.Messages.Prepayment });
				}
			}
			else if (detail.OrigModule == GL.BatchModule.AR)
			{

				if (detail.DocType == ARDocType.Refund)
				{
					PXDefaultAttribute.SetDefault<CABankTranAdjustment.adjdDocType>(cache, ARDocType.CreditMemo);
					PXStringListAttribute.SetList<CABankTranAdjustment.adjdDocType>(cache, null, new string[] { ARDocType.CreditMemo, ARDocType.Payment, ARDocType.Prepayment }, new string[] { AR.Messages.CreditMemo, AR.Messages.Payment, AR.Messages.Prepayment });
				}
				else if (detail.DocType == ARDocType.Payment || detail.DocType == ARDocType.VoidPayment)
				{
					PXDefaultAttribute.SetDefault<CABankTranAdjustment.adjdDocType>(cache, ARDocType.Invoice);
					PXStringListAttribute.SetList<CABankTranAdjustment.adjdDocType>(cache, null,
						new string[] { ARDocType.Invoice, ARDocType.DebitMemo, ARDocType.CreditMemo, ARDocType.FinCharge },
						new string[] { AR.Messages.Invoice, AR.Messages.DebitMemo, AR.Messages.CreditMemo, AR.Messages.FinCharge });
				}
				else
				{
					PXDefaultAttribute.SetDefault<CABankTranAdjustment.adjdDocType>(cache, ARDocType.Invoice);
					PXStringListAttribute.SetList<CABankTranAdjustment.adjdDocType>(cache, null,
						new string[] { ARDocType.Invoice, ARDocType.DebitMemo, ARDocType.FinCharge },
						new string[] { AR.Messages.Invoice, AR.Messages.DebitMemo, AR.Messages.FinCharge });
				}
			}
		}

		public static decimal EvaluateMatching(CABankTransactionsMaint graph, CABankTran aDetail, CABankTranInvoiceMatch aTran, IMatchSettings aSettings)
		{
			decimal relevance = Decimal.Zero;
			decimal[] weights = { 0.1m, 0.7m, 0.2m };
			double sigma = 50.0;
			double meanValue = -7.0;
			if (aSettings != null)
			{
				if (aSettings.DateCompareWeight.HasValue && aSettings.RefNbrCompareWeight.HasValue && aSettings.PayeeCompareWeight.HasValue)
				{
					Decimal totalWeight = (//aSettings.DateCompareWeight.Value + 
										aSettings.RefNbrCompareWeight.Value + aSettings.PayeeCompareWeight.Value);
					if (totalWeight != Decimal.Zero)
					{
						//weights[0] = aSettings.DateCompareWeight.Value / totalWeight;
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
			//relevance += CompareDate(aDetail, aTran, meanValue, sigma) * weights[0];
			relevance += graph.CompareRefNbr(aDetail, aTran, looseCompare) * weights[1];
			relevance += graph.ComparePayee(aDetail, aTran) * weights[2];
			return relevance;
		}

		public static decimal CompareRefNbr(CABankTransactionsMaint graph, CABankTran aDetail, CABankTranInvoiceMatch aTran, bool looseCompare)
		{
			decimal relevance1 = Decimal.Zero;
			decimal relevance2 = Decimal.Zero;
			if (looseCompare)
			{
				relevance1 = String.IsNullOrEmpty(aDetail.InvoiceInfo) && String.IsNullOrEmpty(aTran.ExtRefNbr) ? Decimal.Zero : graph.EvaluateMatching(aDetail.InvoiceInfo, aTran.ExtRefNbr, false);
				relevance2 = String.IsNullOrEmpty(aDetail.InvoiceInfo) && String.IsNullOrEmpty(aTran.OrigRefNbr) ? Decimal.Zero : graph.EvaluateMatching(aDetail.InvoiceInfo, aTran.OrigRefNbr, false);
			}
			else
			{
				relevance1 = String.IsNullOrEmpty(aDetail.InvoiceInfo) && String.IsNullOrEmpty(aTran.ExtRefNbr) ? Decimal.Zero : graph.EvaluateTideMatching(aDetail.InvoiceInfo, aTran.ExtRefNbr, false);
				relevance2 = String.IsNullOrEmpty(aDetail.InvoiceInfo) && String.IsNullOrEmpty(aTran.OrigRefNbr) ? Decimal.Zero : graph.EvaluateTideMatching(aDetail.InvoiceInfo, aTran.OrigRefNbr, false);
			}
			return (relevance1 > relevance2 ? relevance1 : relevance2);
		}

		public static decimal ComparePayee(CABankTransactionsMaint graph, CABankTran aDetail, CABankTranInvoiceMatch aTran)
		{
			return graph.EvaluateMatching(aDetail.PayeeName, aTran.ReferenceName, false);
		}
	}

	public static class StatementApplicationBalancesProto
	{
		public static void UpdateBalance(PXGraph graph, PXSelectBase<CurrencyInfo> curyInfoSelect, CABankTran currentDetail, CABankTranAdjustment adj, bool isCalcRGOL)
		{
			if (currentDetail.OrigModule == GL.BatchModule.AP)
			{
				foreach (PXResult<APInvoice, CurrencyInfo> res in PXSelectJoin<APInvoice, InnerJoin<CurrencyInfo,
					On<CurrencyInfo.curyInfoID, Equal<APInvoice.curyInfoID>>>,
					Where<APInvoice.docType, Equal<Required<APInvoice.docType>>,
						And<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>>.Select(graph, adj.AdjdDocType, adj.AdjdRefNbr))
				{
					UpdateBalanceFromAPDocument(res, curyInfoSelect, adj, isCalcRGOL);
					return;
				}

				foreach (PXResult<APPayment, CurrencyInfo> res in PXSelectJoin<APPayment, InnerJoin<CurrencyInfo,
					On<CurrencyInfo.curyInfoID, Equal<APPayment.curyInfoID>>>,
					Where<APPayment.docType, Equal<Required<APPayment.docType>>,
						And<APPayment.refNbr, Equal<Required<APPayment.refNbr>>>>>.Select(graph, adj.AdjdDocType, adj.AdjdRefNbr))
				{
					UpdateBalanceFromAPDocument(res, curyInfoSelect, adj, isCalcRGOL);
				}
			}
			else if (currentDetail.OrigModule == GL.BatchModule.AR)
			{
				foreach (ARInvoice invoice in PXSelect<ARInvoice, Where<ARInvoice.customerID, Equal<Required<ARInvoice.customerID>>,
					And<ARInvoice.docType, Equal<Required<ARInvoice.docType>>,
					And<ARInvoice.refNbr, Equal<Required<ARInvoice.refNbr>>>>>>.Select(graph, currentDetail.PayeeBAccountID, adj.AdjdDocType, adj.AdjdRefNbr))
				{
					UpdateBalanceFromARDocument(graph, curyInfoSelect, adj, invoice, isCalcRGOL);
					return;
				}

				foreach (ARPayment invoice in PXSelect<ARPayment, Where<ARPayment.customerID, Equal<Required<ARPayment.customerID>>,
					And<ARPayment.docType, Equal<Required<ARPayment.docType>>,
					And<ARPayment.refNbr, Equal<Required<ARPayment.refNbr>>>>>>.Select(graph, currentDetail.PayeeBAccountID, adj.AdjdDocType, adj.AdjdRefNbr))
				{
					UpdateBalanceFromARDocument(graph, curyInfoSelect, adj, invoice, isCalcRGOL);
				}
			}
		}

		private static void UpdateBalanceFromAPDocument<T>(PXResult<T, CurrencyInfo> res, PXSelectBase<CurrencyInfo> curyInfoSelect, CABankTranAdjustment adj, bool isCalcRGOL)
			where T : APRegister, IInvoice, new()
		{
			T invoice = (T)res;

			APAdjust adjustment = new APAdjust();
			adjustment.AdjdRefNbr = adj.AdjdRefNbr;
			adjustment.AdjdDocType = adj.AdjdDocType;
			CopyToAdjust(adjustment, adj);

			APPaymentEntry.CalcBalances<T>(curyInfoSelect, adjustment, invoice, isCalcRGOL, true);

			CopyToAdjust(adj, adjustment);
			adj.AdjdCuryRate = adjustment.AdjdCuryRate;
		}

		private static void UpdateBalanceFromARDocument<TInvoice>(PXGraph graph, PXSelectBase<CurrencyInfo> curyInfoSelect, CABankTranAdjustment adj, TInvoice invoice, bool isCalcRGOL)
			where TInvoice : IInvoice
		{
			ARAdjust adjustment = new ARAdjust();
			CopyToAdjust(adjustment, adj);
			adjustment.AdjdRefNbr = adj.AdjdRefNbr;
			adjustment.AdjdDocType = adj.AdjdDocType;

			StatementApplicationBalances.CalculateBalancesAR<TInvoice>(graph, curyInfoSelect, adjustment, invoice, isCalcRGOL, false);

			CopyToAdjust(adj, adjustment);
			adj.AdjdCuryRate = adjustment.AdjdCuryRate;
		}

		public static void PopulateAdjustmentFieldsAP(PXGraph graph, PXSelectBase<CurrencyInfo> curyInfoSelect, CABankTran currentDetail, CABankTranAdjustment adj)
		{
			foreach (PXResult<APInvoice, CurrencyInfo> res in PXSelectJoin<APInvoice, InnerJoin<CurrencyInfo,
				On<CurrencyInfo.curyInfoID, Equal<APInvoice.curyInfoID>>>,
				Where<APInvoice.docType, Equal<Required<APInvoice.docType>>,
					And<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>>.Select(graph, adj.AdjdDocType, adj.AdjdRefNbr))
			{
				PopulateAP(res, curyInfoSelect, currentDetail, adj);
				return;
			}

			foreach (PXResult<APPayment, CurrencyInfo> res in PXSelectJoin<APPayment, InnerJoin<CurrencyInfo,
				On<CurrencyInfo.curyInfoID, Equal<APPayment.curyInfoID>>>,
				Where<APPayment.docType, Equal<Required<APPayment.docType>>,
					And<APPayment.refNbr, Equal<Required<APPayment.refNbr>>>>>.Select(graph, adj.AdjdDocType, adj.AdjdRefNbr))
			{
				PopulateAP(res, curyInfoSelect, currentDetail, adj);
			}
		}

		private static void PopulateAP<T>(PXResult<T, CurrencyInfo> res, PXSelectBase<CurrencyInfo> curyInfoSelect, CABankTran currentDetail, CABankTranAdjustment adj)
			where T : APRegister, IInvoice, new()
		{
			CurrencyInfo info = (CurrencyInfo)res;
			CurrencyInfo info_copy = null;
			T invoice = (T)res;

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
				info_copy.CuryInfoID = adj.AdjdCuryInfoID;
				info_copy = (CurrencyInfo)curyInfoSelect.Cache.Update(info_copy);
				info_copy.SetCuryEffDate(curyInfoSelect.Cache, currentDetail.TranDate);
			}

			adj.AdjdBranchID = invoice.BranchID;
			adj.AdjdDocDate = invoice.DocDate;
			adj.AdjdFinPeriodID = invoice.FinPeriodID;
			//				adj.AdjgCuryInfoID = currentDetail.CuryInfoID;
			adj.AdjdCuryInfoID = info_copy.CuryInfoID;
			adj.AdjdOrigCuryInfoID = info.CuryInfoID;
			adj.AdjgDocDate = currentDetail.TranDate;
			adj.AdjdAPAcct = invoice.APAccountID;
			adj.AdjdAPSub = invoice.APSubID;

			APAdjust adjustment = new APAdjust();
			adjustment.AdjdRefNbr = adj.AdjdRefNbr;
			adjustment.AdjdDocType = adj.AdjdDocType;
			adjustment.AdjdAPAcct = invoice.APAccountID;
			adjustment.AdjdAPSub = invoice.APSubID;
			CopyToAdjust(adjustment, adj);

			if (currentDetail.DrCr == CADrCr.CACredit)
			{
				adjustment.AdjgDocType = APDocType.Check;
			}
			else
			{
				adjustment.AdjgDocType = APDocType.Refund;
			}

			adj.AdjgBalSign = adjustment.AdjgBalSign;

			APPaymentEntry.CalcBalances<T>(curyInfoSelect, adjustment, invoice, false, true);

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
			else if (currentDetail != null && CuryUnappliedBal > 0m && adjustment.AdjgBalSign > 0m)
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

			APPaymentEntry.CalcBalances<T>(curyInfoSelect, adjustment, invoice, true, true);

			CopyToAdjust(adj, adjustment);
			adj.AdjdCuryRate = adjustment.AdjdCuryRate;
		}

		public static void PopulateAdjustmentFieldsAR(PXGraph graph, PXSelectBase<CurrencyInfo> curyInfoSelect, CABankTran currentDetail, CABankTranAdjustment adj)
		{
			foreach (PXResult<ARInvoice, CurrencyInfo> res in PXSelectJoin<ARInvoice, InnerJoin<CurrencyInfo,
				On<CurrencyInfo.curyInfoID, Equal<ARInvoice.curyInfoID>>>,
				Where<ARInvoice.docType, Equal<Required<ARInvoice.docType>>,
					And<ARInvoice.refNbr, Equal<Required<ARInvoice.refNbr>>>>>.Select(graph, adj.AdjdDocType, adj.AdjdRefNbr))
			{
				PopulateAR(res, graph, curyInfoSelect, currentDetail, adj);
				return;
			}


			foreach (PXResult<ARPayment, CurrencyInfo> res in PXSelectJoin<ARPayment, InnerJoin<CurrencyInfo,
				On<CurrencyInfo.curyInfoID, Equal<ARPayment.curyInfoID>>>,
				Where<ARPayment.docType, Equal<Required<ARPayment.docType>>,
					And<ARPayment.refNbr, Equal<Required<ARPayment.refNbr>>>>>.Select(graph, adj.AdjdDocType, adj.AdjdRefNbr))
			{
				PopulateAR(res, graph, curyInfoSelect, currentDetail, adj);
			}
		}

		private static void PopulateAR<TInvoice>(PXResult<TInvoice, CurrencyInfo> res, PXGraph graph, PXSelectBase<CurrencyInfo> curyInfoSelect, CABankTran currentDetail, CABankTranAdjustment adj)
			where TInvoice : ARRegister, IInvoice, new()
		{
			CurrencyInfo info_copy = PXCache<CurrencyInfo>.CreateCopy((CurrencyInfo)res);
			info_copy.CuryInfoID = adj.AdjdCuryInfoID;
			info_copy = (CurrencyInfo)curyInfoSelect.Cache.Update(info_copy);
			TInvoice invoice = (TInvoice)res;
			info_copy.SetCuryEffDate(curyInfoSelect.Cache, currentDetail.TranDate);

			//				adj.AdjgCuryInfoID = currentDetail.CuryInfoID;
			adj.AdjdCuryInfoID = info_copy.CuryInfoID;
			adj.AdjdOrigCuryInfoID = invoice.CuryInfoID;
			adj.AdjdBranchID = invoice.BranchID;
			adj.AdjdDocDate = invoice.DocDate;
			adj.AdjdFinPeriodID = invoice.FinPeriodID;
			adj.AdjdARAcct = invoice.ARAccountID;
			adj.AdjdARSub = invoice.ARSubID;
			adj.AdjgBalSign = -ARDocType.SignBalance(currentDetail.DocType) * ARDocType.SignBalance(adj.AdjdDocType);

			ARAdjust adjustment = new ARAdjust();
			adjustment.AdjdRefNbr = adj.AdjdRefNbr;
			adjustment.AdjdDocType = adj.AdjdDocType;
			adjustment.AdjdARAcct = invoice.ARAccountID;
			adjustment.AdjdARSub = invoice.ARSubID;
			CopyToAdjust(adjustment, adj);

			StatementApplicationBalances.CalculateBalancesAR(graph, curyInfoSelect, adjustment, invoice, false, true);

			decimal? CuryApplAmt = adjustment.CuryDocBal - adjustment.CuryDiscBal;
			decimal? CuryApplDiscAmt = adjustment.CuryDiscBal;
			decimal? CuryUnappliedBal = currentDetail.CuryUnappliedBal;


			if (currentDetail != null && adj.AdjgBalSign < 0m)
			{
				if (CuryUnappliedBal < 0m)
				{
					CuryApplAmt = Math.Min((decimal)CuryApplAmt, Math.Abs((decimal)CuryUnappliedBal));
				}
			}
			else if (currentDetail != null && CuryUnappliedBal > 0m && adj.AdjgBalSign > 0m)
			{
				CuryApplAmt = Math.Min((decimal)CuryApplAmt, (decimal)CuryUnappliedBal);

				if (CuryApplAmt + CuryApplDiscAmt < adjustment.CuryDocBal)
				{
					CuryApplDiscAmt = 0m;
				}
			}
			else if (currentDetail != null && CuryUnappliedBal <= 0m && ((CABankTran)currentDetail).CuryOrigDocAmt > 0)
			{
				CuryApplAmt = 0m;
				CuryApplDiscAmt = 0m;
			}

			adjustment.CuryAdjgAmt = CuryApplAmt;
			adjustment.CuryAdjgDiscAmt = CuryApplDiscAmt;
			adjustment.CuryAdjgWOAmt = 0m;

			StatementApplicationBalances.CalculateBalancesAR(graph, curyInfoSelect, adjustment, invoice, true, true);

			CopyToAdjust(adj, adjustment);
			adj.AdjdCuryRate = adjustment.AdjdCuryRate;
		}

		public static CABankTranAdjustment CopyToAdjust(CABankTranAdjustment bankAdj, IAdjustment iAdjust)
		{
			bankAdj.AdjgCuryInfoID = iAdjust.AdjgCuryInfoID;
			bankAdj.AdjdCuryInfoID = iAdjust.AdjdCuryInfoID;
			bankAdj.AdjgDocDate = iAdjust.AdjgDocDate;
			bankAdj.DocBal = iAdjust.DocBal;
			bankAdj.CuryDocBal = iAdjust.CuryDocBal;
			bankAdj.CuryDiscBal = iAdjust.CuryDiscBal;
			bankAdj.CuryWhTaxBal = iAdjust.CuryWhTaxBal;
			bankAdj.CuryAdjgAmt = iAdjust.CuryAdjgAmt;
			bankAdj.CuryAdjdAmt = iAdjust.CuryAdjdAmt;
			bankAdj.CuryAdjgDiscAmt = iAdjust.CuryAdjgDiscAmt;
			bankAdj.CuryAdjdDiscAmt = iAdjust.CuryAdjdDiscAmt;
			bankAdj.CuryAdjgWhTaxAmt = iAdjust.CuryAdjgWhTaxAmt;
			bankAdj.AdjdOrigCuryInfoID = iAdjust.AdjdOrigCuryInfoID;
			return bankAdj;
		}

		public static IAdjustment CopyToAdjust(IAdjustment iAdjust, CABankTranAdjustment bankAdj)
		{
			iAdjust.AdjgCuryInfoID = bankAdj.AdjgCuryInfoID;
			iAdjust.AdjdCuryInfoID = bankAdj.AdjdCuryInfoID;
			iAdjust.AdjgDocDate = bankAdj.AdjgDocDate;
			iAdjust.DocBal = bankAdj.DocBal;
			iAdjust.CuryDocBal = bankAdj.CuryDocBal;
			iAdjust.CuryDiscBal = bankAdj.CuryDiscBal;
			iAdjust.CuryWhTaxBal = bankAdj.CuryWhTaxBal;
			iAdjust.CuryAdjgAmt = bankAdj.CuryAdjgAmt;
			iAdjust.CuryAdjdAmt = bankAdj.CuryAdjdAmt;
			iAdjust.CuryAdjgDiscAmt = bankAdj.CuryAdjgDiscAmt;
			iAdjust.CuryAdjdDiscAmt = bankAdj.CuryAdjdDiscAmt;
			iAdjust.CuryAdjgWhTaxAmt = bankAdj.CuryAdjgWhTaxAmt;
			iAdjust.AdjdOrigCuryInfoID = bankAdj.AdjdOrigCuryInfoID;
			return iAdjust;
		}
	}
}
