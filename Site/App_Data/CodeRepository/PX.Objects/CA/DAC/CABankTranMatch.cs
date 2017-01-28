using System;
using PX.Data;
using PX.Objects.CA.BankStatementHelpers;
using PX.Objects.CA.BankStatementProtoHelpers;
using PX.Objects.CR;

namespace PX.Objects.CA
{
	[System.SerializableAttribute()]
    [PXCacheName(Messages.BankTranMatch)]
	public partial class CABankTranMatch : PX.Data.IBqlTable
	{
		#region TranID
		public abstract class tranID : PX.Data.IBqlField
		{
		}
		protected Int32? _TranID;
		[PXDBInt(IsKey = true)]
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
		#region TranType
		public abstract class tranType : PX.Data.IBqlField
		{
		}
		protected String _TranType;
		[PXDBString(1, IsFixed = true)]
		[PXDefault()]
		[CABankTranType.List()]
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
		#region CATranID
		public abstract class cATranID : PX.Data.IBqlField
		{
		}
		protected Int64? _CATranID;
		[PXDBLong()]
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
		[PXStringList(new string[] { GL.BatchModule.AP, GL.BatchModule.AR }, new string[] { GL.Messages.ModuleAP, GL.Messages.ModuleAR })]
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
		#region CuryAmt
		public abstract class curyAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmt;
		[PXDBDecimal()]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Decimal? CuryAmt
		{
			get
			{
				return this._CuryAmt;
			}
			set
			{
				this._CuryAmt = value;
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
		public static void Redirect(PXGraph graph, CABankTranMatch match)
		{
			if (match.DocModule == GL.BatchModule.AP && match.DocType == CATranType.CABatch && match.DocRefNbr != null)
			{
				CABatchEntry docGraph = PXGraph.CreateInstance<CABatchEntry>();
				docGraph.Clear();
				docGraph.Document.Current = PXSelect<CABatch, Where<CABatch.batchNbr, Equal<Required<CATran.origRefNbr>>>>.Select(docGraph, match.DocRefNbr);
				throw new PXRedirectRequiredException(docGraph, true, "Document") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
			}
			else if (match.CATranID != null)
			{
				CATran catran = PXSelect<CATran, Where<CATran.tranID, Equal<Required<CABankTranMatch.cATranID>>>>.Select(graph, match.CATranID);
				CATran.Redirect(null, catran);
			}
			else if (match.DocModule != null && match.DocType != null && match.DocRefNbr != null)
			{
				if (match.DocModule == GL.BatchModule.AP)
				{
					AP.APInvoiceEntry docGraph = PXGraph.CreateInstance<AP.APInvoiceEntry>();
					docGraph.Clear();
					AP.APInvoice invoice = PXSelect<AP.APInvoice, Where<AP.APInvoice.refNbr, Equal<Required<CABankTranMatch.docRefNbr>>, And<AP.APInvoice.docType, Equal<Required<CABankTranMatch.docType>>>>>.Select(docGraph, match.DocRefNbr, match.DocType);
					docGraph.Document.Current = invoice;
					throw new PXRedirectRequiredException(docGraph, true, "Document") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
				}
				else if (match.DocModule == GL.BatchModule.AR)
				{
					AR.ARInvoiceEntry docGraph = PXGraph.CreateInstance<AR.ARInvoiceEntry>();
					docGraph.Clear();
					AR.ARInvoice invoice = PXSelect<AR.ARInvoice, Where<AR.ARInvoice.refNbr, Equal<Required<CABankTranMatch.docRefNbr>>, And<AR.ARInvoice.docType, Equal<Required<CABankTranMatch.docType>>>>>.Select(docGraph, match.DocRefNbr, match.DocType);
					docGraph.Document.Current = invoice;
					throw new PXRedirectRequiredException(docGraph, true, "Document") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
				}
			}
		}
		public void Copy(CABankTranDocRef docRef)
		{
			CATranID = docRef.CATranID;
			DocModule = docRef.DocModule;
			DocType = docRef.DocType;
			DocRefNbr = docRef.DocRefNbr;
			ReferenceID = docRef.ReferenceID;
		}
	}
	public partial class CABankTranMatch2 : CABankTranMatch
	{
		#region TranID
		public new abstract class tranID : PX.Data.IBqlField
		{
		}
		#endregion
		#region TranType
		public new abstract class tranType : PX.Data.IBqlField
		{
		}
		#endregion
		#region CATranID
		public new abstract class cATranID : PX.Data.IBqlField
		{
		}
		#endregion
		#region DocModule
		public new abstract class docModule : PX.Data.IBqlField
		{
		}
		#endregion
		#region DocType
		public new abstract class docType : PX.Data.IBqlField
		{
		}
		#endregion
		#region DocRefNbr
		public new abstract class docRefNbr : PX.Data.IBqlField
		{
		}
		#endregion
	}
}
