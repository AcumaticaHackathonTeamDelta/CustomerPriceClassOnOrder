using PX.SM;
using System;
using System.Collections;
using System.Collections.Generic;	
using PX.Data;
using PX.Objects.PO;
using PX.Objects.AP;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.CM;
using PX.Objects.IN;

namespace PX.Objects.PO
{
	[PXProjection(typeof(Select5<POReceipt,
						   InnerJoin<LandedCostTran, On<LandedCostTran.pOReceiptNbr, Equal<POReceipt.receiptNbr>>>,
						   Where<LandedCostTran.source, Equal<LandedCostTranSource.fromPO>,
							   And<POReceipt.released, Equal<True>,
							   And<LandedCostTran.processed, Equal<False>>>>,
							   Aggregate<GroupBy<POReceipt.receiptNbr,
										   Sum<LandedCostTran.lCAmount,
										   Count<LandedCostTran.lCTranID>>>>>), Persistent = false)]
	[Serializable]
	public partial class POReceiptLCInfo : IBqlTable
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
		#region ReceiptType
		public abstract class receiptType : PX.Data.IBqlField
		{
		}
		protected String _ReceiptType;
		[PXDBString(2, IsFixed = true, InputMask = "", BqlField = typeof(POReceipt.receiptType))]
		[POReceiptType.List()]
		[PXUIField(DisplayName = "Type")]
		public virtual String ReceiptType
		{
			get
			{
				return this._ReceiptType;
			}
			set
			{
				this._ReceiptType = value;
			}
		}
		#endregion
		#region ReceiptNbr
		public abstract class receiptNbr : PX.Data.IBqlField
		{
		}
		protected String _ReceiptNbr;
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "", BqlField = typeof(POReceipt.receiptNbr))]
		[PXUIField(DisplayName = "Receipt Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String ReceiptNbr
		{
			get
			{
				return this._ReceiptNbr;
			}
			set
			{
				this._ReceiptNbr = value;
			}
		}
		#endregion

		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[VendorNonEmployeeActive(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true, BqlField = typeof(POReceipt.vendorID))]
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

		[LocationID(typeof(Where<Location.bAccountID, Equal<Current<POReceiptLCInfo.vendorID>>>), DescriptionField = typeof(Location.descr), Visibility = PXUIVisibility.SelectorVisible, BqlField = typeof(POReceipt.vendorLocationID))]
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
		#region ReceiptDate
		public abstract class receiptDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ReceiptDate;
		[PXDBDate(BqlField = typeof(POReceipt.receiptDate))]
		[PXUIField(DisplayName = "Date", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual DateTime? ReceiptDate
		{
			get
			{
				return this._ReceiptDate;
			}
			set
			{
				this._ReceiptDate = value;
			}
		}
		#endregion
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL", BqlField = typeof(POReceipt.curyID))]
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
		[PXDBLong(BqlField = typeof(POReceipt.curyInfoID))]
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

		#region CuryOrderTotal
		public abstract class curyOrderTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryOrderTotal;

		[PXDBCurrency(typeof(POReceiptLCInfo.curyInfoID), typeof(POReceiptLCInfo.orderTotal), BqlField = typeof(POReceipt.curyOrderTotal))]
		[PXUIField(DisplayName = "Receipt Total Amt.", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual Decimal? CuryOrderTotal
		{
			get
			{
				return this._CuryOrderTotal;
			}
			set
			{
				this._CuryOrderTotal = value;
			}
		}
		#endregion
		#region OrderTotal
		public abstract class orderTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _OrderTotal;

		[PXDBBaseCury(BqlField = typeof(POReceipt.orderTotal))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? OrderTotal
		{
			get
			{
				return this._OrderTotal;
			}
			set
			{
				this._OrderTotal = value;
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
		[PXUIField(DisplayName = "Landed Cost Total (Base Currency)")]
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


	[PX.Objects.GL.TableAndChartDashboardType]
    [Serializable]
	public class POLandedCostProcess : PXGraph<POLandedCostProcess>
	{

		public PXCancel<POReceiptLCInfo> Cancel;
		public PXAction<POReceiptLCInfo> ViewDocument;
		[PXFilterable]
		public PXProcessing<POReceiptLCInfo> receiptsList;

		
		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXEditDetailButton()]
		public virtual IEnumerable viewDocument(PXAdapter adapter)
		{
			if (this.receiptsList.Current != null)
			{
				POReceiptEntry graph = PXGraph.CreateInstance<POReceiptEntry>();
				POReceipt poDoc = graph.Document.Search<POReceipt.receiptNbr>(this.receiptsList.Current.ReceiptNbr, this.receiptsList.Current.ReceiptType);
				if (poDoc != null)
				{
					graph.Document.Current = poDoc;
					throw new PXRedirectRequiredException(graph, true, "Document") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
				}				
			}
			return adapter.Get();
		}

		public PXSetup<POSetup> POSetup;
		public POLandedCostProcess()
		{
			POSetup setup = POSetup.Current;
			receiptsList.SetSelected<POReceiptLCInfo.selected>();
			receiptsList.SetProcessCaption(Messages.Process);
			receiptsList.SetProcessAllCaption(Messages.ProcessAll);
			receiptsList.SetProcessDelegate(delegate(List<POReceiptLCInfo> list)
			{
				ReleaseDoc(list);
			});
		}

		public static void ReleaseDoc(List<POReceiptLCInfo> list)
		{
			LandedCostProcess lcGraph = PXGraph.CreateInstance<LandedCostProcess>();		
			PXSelectBase<LandedCostTran> select = new PXSelect<LandedCostTran, Where<LandedCostTran.source, Equal<LandedCostTranSource.fromPO>,
														And<LandedCostTran.processed, Equal<False>,
														And<LandedCostTran.pOReceiptNbr, Equal<Required<LandedCostTran.pOReceiptNbr>>>>>>(lcGraph);
			DocumentList<INRegister> inList = new DocumentList<INRegister>(lcGraph);
			DocumentList<APInvoice> apList = new DocumentList<APInvoice>(lcGraph);
			bool failed = false;
			int index = 0;
			foreach (POReceiptLCInfo iRct in list)
			{
				if ((iRct.Selected ?? false))
				{
					List<LandedCostTran> trans = new List<LandedCostTran>();
					foreach (LandedCostTran itr in select.Select(iRct.ReceiptNbr))
					{
						trans.Add(itr);
					}
					try
					{
						lcGraph.Clear();
						lcGraph.ReleaseLCTrans(trans, inList, apList);
						PXProcessing<APRegister>.SetInfo(index, ActionsMessages.RecordProcessed);
					}
					catch (LandedCostProcess.NoApplicableSourceException) 
					{
						PXProcessing<APRegister>.SetInfo(index, Messages.NoApplicableLinesForLCTransOnlyAPDocumentIsCreated);
					}
					catch (Exception e)
					{
						PXProcessing<POReceiptLCInfo>.SetError(index, e);
						failed = true;
					}
				}
				index++;
			}
			if (failed)
			{
				throw new PXException(Messages.LandedCostProcessingForOneOrMorePOReceiptsFailed);
			}

		}

		
	}
}
