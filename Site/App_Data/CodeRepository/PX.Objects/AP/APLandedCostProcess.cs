using PX.Objects.GL;
using PX.SM;
using System;
using System.Collections;
using System.Collections.Generic;
using PX.Data;
using PX.Objects.PO;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.CM;
using PX.Objects.IN;

namespace PX.Objects.AP
{
	[PXProjection(typeof(Select5<APRegister,
						InnerJoin<LandedCostTran, On<LandedCostTran.aPDocType, Equal<APRegister.docType>,
						And<LandedCostTran.aPRefNbr, Equal<APRegister.refNbr>>>>,
						Where<LandedCostTran.source, Equal<LandedCostTranSource.fromAP>,
							And<APRegister.released, Equal<True>,
							And<LandedCostTran.processed, Equal<False>>>>,
							Aggregate<GroupBy<APRegister.docType,
									  GroupBy<APRegister.refNbr,
									  Sum<LandedCostTran.lCAmount,
									  Sum<LandedCostTran.curyLCAmount,
									  Count<LandedCostTran.lCTranID>>>>>>>), Persistent = false)]
	[Serializable]
	public partial class APInvoiceLCInfo : IBqlTable
	{
		#region Selected
		public abstract class selected : PX.Data.IBqlField
		{
		}
		protected bool? _Selected = false;
		[PXBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Selected")]
		public virtual bool? Selected
		{
			get
			{
				return _Selected;
			}
			set
			{
				_Selected = value;
			}
		}
		#endregion
		#region DocType
		public abstract class docType : PX.Data.IBqlField
		{
		}
		protected String _DocType;
		[PXDBString(3, IsKey = true, IsFixed = true, InputMask = "", BqlField = typeof(APRegister.docType))]
		[APDocType.List()]
		[PXUIField(DisplayName = "Type")]
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
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "", BqlField = typeof(APRegister.refNbr))]
		[PXUIField(DisplayName = "AP Document Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[VendorNonEmployeeActive(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true, BqlField = typeof(APRegister.vendorID))]
		[PXDefault()]
		public virtual Int32? VendorID
		{
			get
			{
				return this._VendorID;
			}
			set
			{
				this._VendorID = value;
			}
		}
		#endregion
		#region VendorLocationID
		public abstract class vendorLocationID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorLocationID;

		[LocationID(typeof(Where<Location.bAccountID, Equal<Current<APInvoiceLCInfo.vendorID>>>), DescriptionField = typeof(Location.descr), Visibility = PXUIVisibility.SelectorVisible, BqlField = typeof(APRegister.vendorLocationID))]
		public virtual Int32? VendorLocationID
		{
			get
			{
				return this._VendorLocationID;
			}
			set
			{
				this._VendorLocationID = value;
			}
		}
		#endregion
		#region DocDate
		public abstract class docDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _DocDate;
		[PXDBDate(BqlField = typeof(APRegister.docDate))]
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
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL", BqlField = typeof(APRegister.curyID))]
		[PXUIField(DisplayName = "Currency", Visibility = PXUIVisibility.SelectorVisible)]
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
		[PXDBLong(BqlField = typeof(APRegister.curyInfoID))]
		[CM.CurrencyInfo(ModuleCode = "PO")]
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

		#region CuryDocAmount
		public abstract class curyDocAmount : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryDocAmount;
		[PXDBCuryAttribute(typeof(APInvoiceLCInfo.curyID), BqlField = typeof(APRegister.curyOrigDocAmt))]
		[PXUIField(DisplayName = "Receipt Total Amt.", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual Decimal? CuryDocAmount
		{
			get
			{
				return this._CuryDocAmount;
			}
			set
			{
				this._CuryDocAmount = value;
			}
		}
		#endregion
		#region DocAmount
		public abstract class docAmount : PX.Data.IBqlField
		{
		}
		protected Decimal? _DocAmount;

		[PXDBBaseCury(BqlField = typeof(APRegister.origDocAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? DocAmount
		{
			get
			{
				return this._DocAmount;
			}
			set
			{
				this._DocAmount = value;
			}
		}
		#endregion
		#region CuryLCTranTotal
		public abstract class curyLCTranTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryLCTranTotal;
		[PXDBCuryAttribute(typeof(APInvoiceLCInfo.curyID), BqlField = typeof(LandedCostTran.curyLCAmount))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Landed Cost Total")]
		public virtual Decimal? CuryLCTranTotal
		{
			get
			{
				return this._CuryLCTranTotal;
			}
			set
			{
				this._CuryLCTranTotal = value;
			}
		}
		#endregion
		#region LCTranTotal
		public abstract class lCTranTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _LCTranTotal;

		[PXDBBaseCury(BqlField = typeof(LandedCostTran.lCAmount))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Landed Cost Total (Base Currency)", Visible = false)]
		public virtual Decimal? LCTranTotal
		{
			get
			{
				return this._LCTranTotal;
			}
			set
			{
				this._LCTranTotal = value;
			}
		}
		#endregion
		#region LCTranCount
		public abstract class lCTranCount : PX.Data.IBqlField
		{
		}
		protected Int32? _LCTranCount;
		[PXDBInt(BqlField = typeof(LandedCostTran.lCTranID))]
		[PXUIField(DisplayName = "Landed Cost Records", Visible = false)]
		public virtual Int32? LCTranCount
		{
			get
			{
				return this._LCTranCount;
			}
			set
			{
				this._LCTranCount = value;
			}
		}
		#endregion
	}
	[TableAndChartDashboardType]
    [Serializable]
	public class APLandedCostProcess : PXGraph<APLandedCostProcess>
	{

		public PXCancel<APInvoiceLCInfo> Cancel;
		[PXFilterable]
		public PXProcessing<APInvoiceLCInfo> documents;

		public PXAction<APInvoiceLCInfo> ViewDocument;
		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable viewDocument(PXAdapter adapter)
		{
			if (this.documents.Current != null)
			{
				APInvoiceEntry graph = PXGraph.CreateInstance<APInvoiceEntry>();
				APInvoice poDoc = graph.Document.Search<APInvoice.refNbr>(this.documents.Current.RefNbr, this.documents.Current.DocType);
				if (poDoc != null)
				{
					graph.Document.Current = poDoc;
					throw new PXRedirectRequiredException(graph, true, "Document") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
				}
			}
			return adapter.Get();
		}

		public APLandedCostProcess()
		{
			APSetup apSetup = APSetup.Current;
			POSetup poSetup = POSetup.Current;			
			documents.SetSelected<APInvoiceLCInfo.selected>();
			documents.SetProcessCaption(ActionsMessages.Process);
			documents.SetProcessAllCaption(ActionsMessages.ProcessAll);
			documents.SetProcessDelegate(delegate(List<APInvoiceLCInfo> list)
			{
				ReleaseDoc(list);
			});
		}

		public PXSetup<APSetup> APSetup;
		public PXSetup<POSetup> POSetup;

		public static void ReleaseDoc(List<APInvoiceLCInfo> list)
		{
			LandedCostProcess lcGraph = PXGraph.CreateInstance<LandedCostProcess>();
			PXSelectBase<LandedCostTran> select = new PXSelect<LandedCostTran, Where<LandedCostTran.source, Equal<LandedCostTranSource.fromAP>,
														And<LandedCostTran.processed, Equal<False>,
														And<LandedCostTran.aPRefNbr, Equal<Required<LandedCostTran.aPRefNbr>>,
														And<LandedCostTran.aPDocType, Equal<Required<LandedCostTran.aPDocType>>>>>>>(lcGraph);
			bool failed = false;
			int index = 0;
			foreach (APInvoiceLCInfo iRct in list)
			{
				if ((iRct.Selected ?? false))
				{
					List<LandedCostTran> trans = new List<LandedCostTran>();
					foreach (LandedCostTran itr in select.Select(iRct.RefNbr,iRct.DocType))
					{
						trans.Add(itr);
					}
					try
					{
						lcGraph.Clear();
						lcGraph.ReleaseLCTrans(trans, null, null);
						PXProcessing<APRegister>.SetInfo(index, ActionsMessages.RecordProcessed);
					}
					catch (Exception e)
					{
						PXProcessing<APInvoiceLCInfo>.SetError(index, e);
						failed = true;
					}
				}
				index++;
			}
			if (failed)
			{
				throw new PXException(Messages.ProcessingOfLandedCostTransForAPDocFailed);
			}

		}


	}
}
