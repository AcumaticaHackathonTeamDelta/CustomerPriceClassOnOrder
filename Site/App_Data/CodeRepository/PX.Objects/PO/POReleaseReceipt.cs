using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using PX.Common;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.AP;
using PX.Objects.PO;
using PX.Objects.GL;
using PX.Objects.CM;

namespace PX.Objects.PO
{
	[TableAndChartDashboardType]
	public class POReleaseReceipt : PXGraph<POReleaseReceipt>
	{
		public PXCancel<POReceipt> Cancel;
		public PXAction<POReceipt> ViewDocument;
		[PXFilterable]
		public PXProcessing<POReceipt> Orders;

		
		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXEditDetailButton()]
		public virtual IEnumerable viewDocument(PXAdapter adapter)
		{
			if (this.Orders.Current != null)
			{
				if (this.Orders.Current.Released == false)
				{
					POReceiptEntry graph = PXGraph.CreateInstance<POReceiptEntry>();
					POReceipt poDoc = graph.Document.Search<POReceipt.receiptNbr>(this.Orders.Current.ReceiptNbr, this.Orders.Current.ReceiptType);
					if( poDoc != null)
					{
						graph.Document.Current = poDoc;
						throw new PXRedirectRequiredException(graph, true, "Document"){Mode = PXBaseRedirectException.WindowMode.NewWindow};
					}
				}				
			}
			return adapter.Get();
		}


		public POReleaseReceipt()
		{
			Orders.SetSelected<POReceipt.selected>();
			Orders.SetProcessCaption(Messages.Process);
			Orders.SetProcessAllCaption(Messages.ProcessAll);
			Orders.SetProcessDelegate(delegate(List<POReceipt> list)
			{
				ReleaseDoc(list,true);
			});
		}


		public virtual IEnumerable orders()
		{
			foreach (PXResult<POReceipt> res in PXSelectJoinGroupBy<POReceipt,
			LeftJoinSingleTable<Vendor, On<Vendor.bAccountID, Equal<POReceipt.vendorID>>,
			InnerJoin<POReceiptLine, On<POReceiptLine.receiptNbr, Equal<POReceipt.receiptNbr>>,			
			LeftJoin<APTran, On<APTran.receiptNbr, Equal<POReceiptLine.receiptNbr>, 
				And<APTran.receiptLineNbr, Equal<POReceiptLine.lineNbr>>>>>>,
			Where2<Where<Vendor.bAccountID, IsNull, Or<Match<Vendor, Current<AccessInfo.userName>>>>, 
			And<POReceipt.hold, Equal<boolFalse>,
			And<POReceipt.released, Equal<boolFalse>,			
			And<APTran.refNbr, IsNull>>>>,
			Aggregate<GroupBy<POReceipt.receiptNbr,
			GroupBy<POReceipt.receiptType,
			GroupBy<POReceipt.released,
			GroupBy<POReceipt.hold,
			GroupBy<POReceipt.autoCreateInvoice>>>>>>>
			.Select(this))
			{
				POReceipt sel = res;
				POReceipt order;
				if ((order = (POReceipt)Orders.Cache.Locate(sel)) != null)
				{
					sel.Selected = order.Selected;
				}
				
				yield return sel;
			}
			Orders.Cache.IsDirty = false;
		}

		public static void ReleaseDoc( List<POReceipt> list, bool aIsMassProcess)
		{
			POReceiptEntry docgraph = PXGraph.CreateInstance<POReceiptEntry>();
            DocumentList<INRegister> created = new DocumentList<INRegister>(docgraph);
			DocumentList<APInvoice> invoicesCreated = new DocumentList<APInvoice>(docgraph);
			INReceiptEntry iRe = null;
			INIssueEntry iIe = null;
			AP.APInvoiceEntry apInvoiceGraph = PXGraph.CreateInstance<APInvoiceEntry>();
			int iRow = 0;
			bool failed = false;			
			foreach (POReceipt order in list)
			{
				try
				{
					switch (order.ReceiptType)
					{
						case POReceiptType.POReceipt:
                        case POReceiptType.TransferReceipt:
							if (iRe == null) iRe = docgraph.CreateReceiptEntry();
							docgraph.ReleaseReceipt(iRe, apInvoiceGraph, order, created, invoicesCreated, aIsMassProcess);
							break;
						case POReceiptType.POReturn:
							if (iIe == null) iIe = docgraph.CreateIssueEntry();
							docgraph.ReleaseReturn(iIe, apInvoiceGraph, order, created, invoicesCreated, aIsMassProcess);
							break;
					}
					PXProcessing<POReceipt>.SetInfo(iRow, ActionsMessages.RecordProcessed);
				}
				catch (Exception e) 
				{
					if (aIsMassProcess)
					{
						PXProcessing<POReceipt>.SetError(iRow, e);
						failed = true;
					}
					else
						throw;
				}
				iRow++;
			}
			if (failed)
			{
				throw new PXException(Messages.ReleaseOfOneOrMoreReceiptsHasFailed);
			}
		}
	}
	
	[PXHidden()]
	public class LandedCostProcess : PXGraph<LandedCostProcess>
	{

		#region Type Definition

		public class NoApplicableSourceException : PXException
		{
			public NoApplicableSourceException() : base()
			{
			}

			public NoApplicableSourceException(string message, params object[] aParams)
				: base(message, aParams)
			{

			}

			public NoApplicableSourceException(SerializationInfo info, StreamingContext context)
				: base(info, context)
			{
				PXReflectionSerializer.RestoreObjectProps(this, info);
			}

			public override void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				PXReflectionSerializer.GetObjectData(this, info);
				base.GetObjectData(info, context);
			}

		}

		#endregion
		

		public PXSelect<LandedCostTran, Where<LandedCostTran.lCTranID, Equal<Required<LandedCostTran.lCTranID>>>> landedCostTrans;
		public PXSelect<POReceiptLine, Where<POReceiptLine.receiptNbr, Equal<Required<POReceiptLine.receiptNbr>>>> poLines;
		public PXSetup<POSetup> poSetup;
		public LandedCostProcess()
		{
			POSetup record = poSetup.Current;
		}

		public virtual void ReleaseLCTrans(IEnumerable<LandedCostTran> aTranSet, DocumentList<INRegister> aINCreated, DocumentList<APInvoice> aAPCreated)
		{
			Dictionary<int, APInvoiceEntry> apGraphs = new Dictionary<int, APInvoiceEntry>();
			Dictionary<int, INAdjustmentEntry> inGraphs = new Dictionary<int, INAdjustmentEntry>();			
			Dictionary<int, int> combinations = new Dictionary<int, int>();
			List<APRegister> forReleaseAP = new List<APRegister>();
			List<INRegister> forReleaseIN = new List<INRegister>();
			DocumentList<APInvoice> apDocuments = new DocumentList<APInvoice>(this); 
			POSetup poSetupR = this.poSetup.Select();
			bool autoReleaseIN = poSetupR.AutoReleaseLCIN.Value;
			bool autoReleaseAP = poSetupR.AutoReleaseAP.Value;
			bool noApplicableItems = false;
            bool noApplicableTransfers = false;

			foreach (LandedCostTran iTran in aTranSet)
			{
				LandedCostCode lcCode = PXSelect<LandedCostCode, Where<LandedCostCode.landedCostCodeID, Equal<Required<LandedCostCode.landedCostCodeID>>>>.Select(this, iTran.LandedCostCodeID);
				if ((string.IsNullOrEmpty(iTran.APDocType) || string.IsNullOrEmpty(iTran.APRefNbr))&& iTran.PostponeAP == false)
				{
					APInvoiceEntry apGraph = null;					
					foreach (KeyValuePair<int, APInvoiceEntry> iGraph in apGraphs) 
					{
						APInvoice apDoc = iGraph.Value.Document.Current;
						string terms = String.IsNullOrEmpty(iTran.TermsID) ? lcCode.TermsID : iTran.TermsID;

						if (apDoc.VendorID == iTran.VendorID 
							&& apDoc.VendorLocationID == iTran.VendorLocationID							
							&& apDoc.InvoiceNbr == iTran.InvoiceNbr
							&& apDoc.CuryID == iTran.CuryID
							&& apDoc.DocDate == iTran.InvoiceDate
							&& apDoc.TermsID == terms 
                            && (apDoc.DocType == AP.APDocType.Invoice && iTran.CuryLCAmount > Decimal.Zero)) 
						{
							combinations.Add(iTran.LCTranID.Value, iGraph.Key);
							apGraph = iGraph.Value;
						}
					}
					if (apGraph == null)
					{
						apGraph = PXGraph.CreateInstance<APInvoiceEntry>();
						if (autoReleaseAP)
						{
							apGraph.APSetup.Current.RequireControlTotal = false;
							apGraph.APSetup.Current.RequireControlTaxTotal = false;
							apGraph.APSetup.Current.HoldEntry = false;
						}
						apGraphs[iTran.LCTranID.Value] = apGraph;
					}
					apGraph.InvoiceLandedCost(iTran, null, false);
					apDocuments.Add(apGraph.Document.Current);
				}
				
				if (lcCode.AllocationMethod != LandedCostAllocationMethod.None)
				{
					List<POReceiptLine> receiptLines = new List<POReceiptLine>();
					List<LandedCostTranSplit> lcTranSplits = new List<LandedCostTranSplit>();
					GetReceiptLinesToAllocate(receiptLines, lcTranSplits, iTran);					
                    var lch = new LandedCostHelper(this, false);				
					List<LandedCostHelper.POReceiptLineAdjustment> result = lch.AllocateLCOverRCTLines(receiptLines, lcCode, iTran, lcTranSplits);
                    if (result.Count > 0)
					{
						if (result.Count == 1 && !result[0].Item1.InventoryID.HasValue)
						{
                            noApplicableTransfers = !lch.HasApplicableTransfers;
							noApplicableItems = true;  //Skip Cost adjustment creation;
						}
						else
						{
							INAdjustmentEntry inGraph = PXGraph.CreateInstance<INAdjustmentEntry>();
                            if (autoReleaseIN)
							{
								inGraph.insetup.Current.RequireControlTotal = false;
								inGraph.insetup.Current.HoldEntry = false;
							}
							CreateCostAjustment(inGraph, lcCode, iTran, result);
							inGraphs[iTran.LCTranID.Value] = inGraph;
						}
					}
				}
			}

			using (PXConnectionScope cs = new PXConnectionScope())
			{
				using (PXTransactionScope ts = new PXTransactionScope())
				{
					foreach (LandedCostTran iTran in aTranSet)
					{
						bool needUpdate = false;
						LandedCostTran tran = this.landedCostTrans.Select(iTran.LCTranID);
						
						if (apGraphs.ContainsKey(tran.LCTranID.Value))
						{							
							APInvoiceEntry apGraph = apGraphs[iTran.LCTranID.Value];
							apGraph.Save.Press();
							tran.APDocType = apGraph.Document.Current.DocType;
							tran.APRefNbr = apGraph.Document.Current.RefNbr;
							if(!tran.APCuryInfoID.HasValue)
								tran.APCuryInfoID = apGraph.Document.Current.CuryInfoID;
							tran.Processed = true;
							if (apGraph.Document.Current.Hold != true)
								forReleaseAP.Add(apGraph.Document.Current);
							if (aAPCreated != null)
								aAPCreated.Add(apGraph.Document.Current);
							needUpdate = true;
						}
						else if (combinations.ContainsKey(tran.LCTranID.Value)) 
						{
							//Its already saved at this point
							APInvoiceEntry apGraph = apGraphs[combinations[tran.LCTranID.Value]];
							tran.APDocType = apGraph.Document.Current.DocType;
							tran.APRefNbr = apGraph.Document.Current.RefNbr;
							if (!tran.APCuryInfoID.HasValue)
								tran.APCuryInfoID = apGraph.Document.Current.CuryInfoID;							
							tran.Processed = true;
							needUpdate = true;
						}

						if (inGraphs.ContainsKey(tran.LCTranID.Value))
						{
							INAdjustmentEntry inGraph = inGraphs[iTran.LCTranID.Value];
							inGraph.Save.Press();
							tran.INDocType = inGraph.adjustment.Current.DocType;
							tran.INRefNbr = inGraph.adjustment.Current.RefNbr;
							tran.Processed = true;
							forReleaseIN.Add(inGraph.adjustment.Current); 
							if (aINCreated != null)
								aINCreated.Add(inGraph.adjustment.Current);
							needUpdate = true;
						}
						if (!needUpdate && tran.PostponeAP == true) 
						{
							LandedCostCode lcCode = PXSelect<LandedCostCode, Where<LandedCostCode.landedCostCodeID, Equal<Required<LandedCostCode.landedCostCodeID>>>>.Select(this, iTran.LandedCostCodeID);
							if (lcCode.AllocationMethod == LandedCostAllocationMethod.None)
							{
								tran.Processed = true;
								needUpdate = true; //This combination needs no processing here but must be updated
							}
						}  
						if (needUpdate)
						{
							LandedCostTran copy = (LandedCostTran)this.landedCostTrans.Cache.CreateCopy(tran);
							tran = this.landedCostTrans.Update(copy);
						}
					}
					this.Actions.PressSave();
					ts.Complete();
				}
			}
            if(noApplicableTransfers == true)
            {
                throw new NoApplicableSourceException(Messages.LandedCostCannotBeDistributed);
            }
			if(noApplicableItems == true) 
			{
				throw new NoApplicableSourceException(Messages.LandedCostAmountRemainderCannotBeDistributedMultyLines);
			}

			if (autoReleaseAP)
			{
				if (forReleaseAP.Count > 0)
						APDocumentRelease.ReleaseDoc(forReleaseAP, true);
			}

			if (autoReleaseIN)
			{
				if (forReleaseIN.Count > 0)
					INDocumentRelease.ReleaseDoc(forReleaseIN, false);
			}			
		}

        protected virtual void CreateCostAjustment(INAdjustmentEntry inGraph, LandedCostCode aLCCode, LandedCostTran aTran, List<LandedCostHelper.POReceiptLineAdjustment> aAllocatedLines)
        {
            inGraph.Clear();

            inGraph.FieldVerifying.AddHandler<INTran.inventoryID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });
            inGraph.FieldVerifying.AddHandler<INTran.origRefNbr>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });

            inGraph.insetup.Current.RequireControlTotal = false;
            inGraph.insetup.Current.HoldEntry = false;

            INRegister newdoc = new INRegister();
            newdoc.DocType = INDocType.Adjustment;
            newdoc.OrigModule = BatchModule.PO;
            newdoc.SiteID = null;
            newdoc.TranDate = aTran.InvoiceDate;
            inGraph.adjustment.Insert(newdoc);

			CreateAdjustmentTran(inGraph, aAllocatedLines, aLCCode, aTran, aLCCode.ReasonCode);
        }
        
		public virtual void CreateAdjustmentTran(INAdjustmentEntry inGraph, List<LandedCostHelper.POReceiptLineAdjustment> pOLinesToProcess, LandedCostCode aLCCode, LandedCostTran aTran, string ReasonCode)
		{
			foreach (LandedCostHelper.POReceiptLineAdjustment poreceiptline in pOLinesToProcess)
			{
				INTran intran = new INTran();
				bool isLCTran = aTran != null;

                INTran origtran = LandedCostHelper.GetOriginalInTran(inGraph, poreceiptline.Item1.ReceiptNbr, poreceiptline.Item1.LineNbr);
				bool isDropShip = (poreceiptline.Item1.LineType == POLineType.GoodsForDropShip || poreceiptline.Item1.LineType == POLineType.NonStockForDropShip);

				if (!isDropShip && origtran == null) throw new PXException(AP.Messages.CannotFindINReceipt, poreceiptline.Item1.ReceiptNbr);

				if (!isDropShip && (origtran == null || origtran.Released != true))
				{
					throw new PXException(isLCTran ? AP.Messages.INReceiptMustBeReleasedBeforePPV : AP.Messages.INReceiptMustBeReleasedBeforePPV, origtran.RefNbr, poreceiptline.Item1.ReceiptNbr);
				}

				//Drop-Ships are considered non-stocks
				if (poreceiptline.Item1.IsStockItem())
				{
					intran.TranType = INTranType.ReceiptCostAdjustment;
				}
				else
				{
					//Landed cost and PPV for non-stock items are handled in special way in the inventory.
					//They should create a GL Batch, but for convinience and unforminty this functionality is placed into IN module
					//Review this part when the functionality is implemented in IN module
					intran.TranType = INTranType.Adjustment;
					intran.InvtMult = 0;
                }
				intran.InventoryID = poreceiptline.Item1.InventoryID;
				intran.SubItemID = poreceiptline.Item1.SubItemID;
				intran.SiteID = poreceiptline.Item1.SiteID;
				intran.BAccountID = isLCTran ? aTran.VendorID : poreceiptline.Item1.VendorID;
                intran.BranchID = poreceiptline.BranchID;


				if (isDropShip && intran.SiteID != null)
				{
					INSite invSite = PXSelect<INSite, Where<INSite.siteID, Equal<Required<POReceiptLine.siteID>>>>.Select(inGraph, intran.SiteID);
					if (invSite.DropShipLocationID == null)
					{
						throw new PXException(SO.Messages.NoDropShipLocation, invSite.SiteCD);
					}

					intran.LocationID = invSite.DropShipLocationID;
				}
				else
				{
					intran.LocationID = poreceiptline.Item1.LocationID ?? origtran.LocationID;
				}

				intran.LotSerialNbr = poreceiptline.Item1.LotSerialNbr;
				intran.POReceiptNbr = poreceiptline.Item1.ReceiptNbr;
				intran.POReceiptLineNbr = poreceiptline.Item1.LineNbr;

				//tran.Qty = poreceiptline.ReceiptQty;
				intran.TranDesc = isLCTran ? aTran.Descr : poreceiptline.Item1.TranDesc;
				//tran.UnitCost = PXDBPriceCostAttribute.Round(inGraph.transactions.Cache, (decimal)(poreceiptline.ExtCost / poreceiptline.ReceiptQty));
				intran.TranCost = poreceiptline.Item2;
				intran.ReasonCode = ReasonCode;
				if (origtran != null && origtran.DocType == INDocType.Issue)
				{
					intran.ARDocType = origtran.ARDocType;
					intran.ARRefNbr = origtran.ARRefNbr;
					intran.ARLineNbr = origtran.ARLineNbr;
				}
				if (!isDropShip)
				{
					intran.OrigTranType = origtran.DocType;
					intran.OrigRefNbr = origtran.RefNbr;
				}

				int? acctID = null;
				int? subID = null;
				if (isLCTran)
				{
					intran.AcctID = aLCCode.LCAccrualAcct;
					intran.SubID = aLCCode.LCAccrualSub;
                    GetLCVarianceAccountSub(ref acctID, ref subID, inGraph, poreceiptline.Item1);
				}
				else
				{
					//Set AcctID and SubID = POAccrual Acct/Sub from orig. INTran
					if (origtran != null)
					{
						intran.AcctID = origtran.AcctID;
						intran.SubID = origtran.SubID;
					}
					else if (isDropShip)
					{
						intran.AcctID = poreceiptline.Item1.POAccrualAcctID;
						intran.SubID = poreceiptline.Item1.POAccrualSubID;
					}
					ReasonCode reasonCode = PXSelect<ReasonCode, Where<ReasonCode.reasonCodeID, Equal<Required<ReasonCode.reasonCodeID>>>>.Select(inGraph, ReasonCode);
					if (reasonCode == null)
						throw new PXException(AP.Messages.ReasonCodeCannotNotFound, ReasonCode);
					AP.APReleaseProcess.GetPPVAccountSub(ref acctID, ref subID, inGraph, poreceiptline.Item1, reasonCode);
				}
				intran.COGSAcctID = acctID;
				intran.COGSSubID = subID;

				intran = inGraph.transactions.Insert(intran);
			}
		}

		//Graph passed in function must be the one using newly created sub in details - otherwise save will fail (inserted subs will be created in other graph)
		protected static void GetLCVarianceAccountSub(ref int? aAccountID, ref int? aSubID, PXGraph aGraph, POReceiptLine aRow)
		{
			if (aRow.InventoryID.HasValue)
			{
				PXResult<InventoryItem, INPostClass> res = (PXResult<InventoryItem, INPostClass>)PXSelectJoin<InventoryItem,
									LeftJoin<INPostClass, On<INPostClass.postClassID, Equal<InventoryItem.postClassID>>>,
									Where<InventoryItem.inventoryID, Equal<Required<POLine.inventoryID>>>>.Select(aGraph, aRow.InventoryID);
				if (res != null)
				{
					InventoryItem invItem = (InventoryItem)res;
					INPostClass postClass = (INPostClass)res;
					if ((bool)invItem.StkItem)
					{
						if (postClass == null)
							throw new PXException(Messages.PostingClassIsNotDefinedForTheItemInReceiptRow, invItem.InventoryCD, aRow.ReceiptNbr, aRow.LineNbr);
						INSite invSite = null;
						if (aRow.SiteID != null)
							invSite = PXSelect<INSite, Where<INSite.siteID, Equal<Required<POReceiptLine.siteID>>>>.Select(aGraph, aRow.SiteID);

						aAccountID = INReleaseProcess.GetAcctID<INPostClass.cOGSAcctID>(aGraph, postClass.COGSAcctDefault, invItem, invSite, postClass);
						if (aAccountID == null)
							throw new PXException(Messages.COGSAccountCanNotBeFoundForItemInReceiptRow, invItem.InventoryCD, aRow.ReceiptNbr, aRow.LineNbr, postClass.PostClassID, invSite != null ? invSite.SiteCD : String.Empty);
						try
						{
							aSubID = INReleaseProcess.GetSubID<INPostClass.cOGSSubID>(aGraph, postClass.COGSAcctDefault, postClass.COGSSubMask, invItem, invSite, postClass);
						}
						catch (PXException ex)
						{
							if (postClass.COGSSubID == null
								|| string.IsNullOrEmpty(postClass.COGSSubMask)
									|| invItem.COGSSubID == null || invSite == null || invSite.COGSSubID == null)
								throw new PXException(Messages.COGSSubAccountCanNotBeFoundForItemInReceiptRow, invItem.InventoryCD, aRow.ReceiptNbr, aRow.LineNbr, postClass.PostClassID, invSite != null ? invSite.SiteCD : String.Empty);
							else
								throw ex;
						}
						if (aSubID == null)
							throw new PXException(Messages.COGSSubAccountCanNotBeFoundForItemInReceiptRow, invItem.InventoryCD, aRow.ReceiptNbr, aRow.LineNbr, postClass.PostClassID, invSite != null ? invSite.SiteCD : String.Empty);
					}
					else
					{
						aAccountID = aRow.ExpenseAcctID;
						aSubID = aRow.ExpenseSubID;
					}
				}
				else
				{
					throw
						new PXException(Messages.LCInventoryItemInReceiptRowIsNotFound, aRow.InventoryID, aRow.ReceiptNbr, aRow.LineNbr);
				}
			}
			else
			{
				aAccountID = aRow.ExpenseAcctID;
				aSubID = aRow.ExpenseSubID;				
			}
		}

		protected virtual void GetReceiptLinesToAllocate(List<POReceiptLine> aReceiptLines, List<LandedCostTranSplit> aSplits, LandedCostTran aTran)
		{
			Dictionary<string, int> used = new Dictionary<string,int>();
			foreach (POReceiptLine it in this.poLines.Select(aTran.POReceiptNbr)) 
			{
					aReceiptLines.Add(it);
					used[aTran.POReceiptNbr] = 1;
			}
			foreach (LandedCostTranSplit iSplit in PXSelect<LandedCostTranSplit, Where<LandedCostTranSplit.lCTranID, Equal<Required<LandedCostTranSplit.lCTranID>>>>.Select(this, aTran.LCTranID))
			{
				aSplits.Add(iSplit); 
				if (used.ContainsKey(iSplit.POReceiptNbr))
				{
					used[iSplit.POReceiptNbr] += 1;
					continue;
				}						
				foreach (POReceiptLine it in this.poLines.Select(iSplit.POReceiptNbr))
				{
						aReceiptLines.Add(it);
						used[iSplit.POReceiptNbr] = 1;
				}
			}			
		}
	}


	#region Landed Cost Utility Class
	public class LandedCostHelper
	{
        private PXGraph _graph;

        public class POReceiptLineAdjustment : Tuple<POReceiptLine, decimal, Int32?>
        {
            public POReceiptLine ReceiptLine { get { return Item1; } }
            public decimal QtyToAssign { get { return Item2; } }
            public Int32? BranchID { get { return Item3; } }
            public POReceiptLineAdjustment(POReceiptLine receiptLine, decimal qtyToAssign, int? branchID) : base(receiptLine, qtyToAssign, branchID)
            {
            }
        }
        public bool HasApplicableTransfers { get; set; }
        private bool _isPPV;
        public LandedCostHelper(PXGraph graph, bool isPPV)
	{
            _graph = graph;
            _isPPV = isPPV;
            
            HasApplicableTransfers = false;
        }

		public bool IsApplicable(LandedCostTran aTran, LandedCostCode aCode, POReceiptLine aLine) 
		{
			return IsApplicable(aTran, null, aCode, aLine);
		}

        public static INTran GetOriginalInTran(PXGraph graph, string receiptNbr, int? lineNbr)
        {
            return
                    PXSelect<INTran, Where<INTran.docType, NotEqual<INDocType.adjustment>,
                        And<INTran.docType, NotEqual<INDocType.transfer>,
                        And<INTran.pOReceiptNbr, Equal<Required<INTran.pOReceiptNbr>>,
                        And<INTran.pOReceiptLineNbr, Equal<Required<INTran.pOReceiptLineNbr>>>>>>>.SelectWindowed(graph, 0, 1, receiptNbr, lineNbr);
        }

		public bool IsApplicable(LandedCostTran aTran, IEnumerable<LandedCostTranSplit> aSplits, LandedCostCode aCode, POReceiptLine aLine)
		{
			if (!IsApplicable(aCode, aLine)) return false;
			bool passes = false;
			if (aSplits != null)
			{
				foreach (LandedCostTranSplit it in aSplits)
				{					
					if (it.POReceiptNbr == aLine.ReceiptNbr)
					{
						passes = true;
						if (passes && it.POReceiptLineNbr.HasValue)
							if (it.POReceiptLineNbr != aLine.LineNbr) passes = false;
						if (passes && it.InventoryID.HasValue)
							if (it.InventoryID != aLine.InventoryID) passes = false;						
					}
					if (passes)
						break;
				}
			}

			if (!passes)
			{
				if(String.IsNullOrEmpty(aTran.POReceiptNbr)
					|| (aTran.POReceiptNbr != aLine.ReceiptNbr))
					return false;
				if(aTran.POReceiptLineNbr.HasValue)
					if (aTran.POReceiptLineNbr != aLine.LineNbr) return false;
				if (aTran.InventoryID.HasValue)
					if (aTran.InventoryID != aLine.InventoryID) return false;
				passes = true;
			}
			if (passes)
			{
				if (aTran.SiteID.HasValue)
					if (aTran.SiteID != aLine.SiteID) return false;
				if (aTran.LocationID.HasValue)
					if (aTran.LocationID != aLine.LocationID) return false;
			}
			return passes;
		}

		public static bool NeedsApplicabilityChecking(LandedCostCode aCode) 
		{
			return (aCode.AllocationMethod != LandedCostAllocationMethod.None);
		}

		public bool IsApplicable(LandedCostCode aCode, POReceiptLine aLine)
		{
            bool transferinsidewarehouse = false;
            if (aLine.ReceiptType == POReceiptType.TransferReceipt)
            {
                INTran ortran = PXSelectReadonly<INTran, Where<INTran.tranType, Equal<Required<INTran.origTranType>>,
                                And<INTran.refNbr, Equal<Required<INTran.origRefNbr>>>>>
                                .SelectWindowed(_graph, 0, 1, aLine.OrigTranType, aLine.OrigRefNbr, aLine.OrigLineNbr);
                transferinsidewarehouse = (ortran == null || ortran.SiteID == aLine.SiteID);
            }
            if (transferinsidewarehouse == false)
                HasApplicableTransfers = true;
			//Memo - in this release, non-stock Items are not applicable for the landed cost. Review later.
            return !transferinsidewarehouse && 
                (aCode.AllocationMethod!=LandedCostAllocationMethod.None && (aLine.LineType == POLineType.GoodsForInventory || 
				aLine.LineType == POLineType.GoodsForReplenishment ||
				aLine.LineType == POLineType.GoodsForSalesOrder ||
				aLine.LineType == POLineType.GoodsForDropShip ||
                aLine.LineType == POLineType.NonStock ||
                aLine.LineType == POLineType.NonStockForDropShip ||
                aLine.LineType == POLineType.NonStockForSalesOrder));
        }
		public static Decimal GetBaseValue(LandedCostCode aCode, POReceiptLine aLine)
		{
			Decimal value = Decimal.Zero;
			switch (aCode.AllocationMethod)
			{
				case LandedCostAllocationMethod.ByCost: value = aLine.CuryExtCost ?? Decimal.Zero; break;
				case LandedCostAllocationMethod.ByQuantity: value = aLine.BaseQty ?? Decimal.Zero; break;
				case LandedCostAllocationMethod.ByWeight: value = aLine.ExtWeight ?? Decimal.Zero; break;
				case LandedCostAllocationMethod.ByVolume: value = aLine.ExtVolume ?? Decimal.Zero; break;
				case LandedCostAllocationMethod.None: value = Decimal.One; break; //Line Count
				default:
					throw new PXException(Messages.UnknownLCAllocationMethod, aCode.LandedCostCodeID);
			}
			return value;
		}
        public Decimal CalcAllocationValue(LandedCostCode aCode, POReceiptLine aLine, Decimal aBaseTotal, Decimal aAllocationTotal)
		{
            if (_isPPV)
                return aAllocationTotal;

			Decimal value = Decimal.Zero;
			if (IsApplicable(aCode, aLine))
			{
				Decimal baseShare = LandedCostHelper.GetBaseValue(aCode, aLine);
				value = (baseShare * aAllocationTotal) / aBaseTotal; 
			}
			return value;
		}

        public Decimal CalcAllocationValue(LandedCostCode aCode, POReceiptLineSplit aSplit, POReceiptLine aLine, Decimal aBaseTotal, Decimal aAllocationTotal)
        {
            var res = CalcAllocationValue(aCode, aLine, aBaseTotal, aAllocationTotal);
            return res * ((aSplit.BaseQty / (aLine.BaseQty == 0 ? (decimal?)null : aLine.BaseQty)) ?? 1);
        }

        public decimal AllocateOverRCTLine(List<POReceiptLineAdjustment> result, POReceiptLine aLine, decimal toDistribute, Int32? branchID)
		{
            var aLCCode = new LandedCostCode();
            aLCCode.AllocationMethod = LandedCostAllocationMethod.ByQuantity;
            decimal baseTotal = GetBaseValue(aLCCode, aLine);

            decimal shareAmt = Decimal.Zero;
            decimal allocatedAmt = Decimal.Zero;
            decimal allocatedBase = Decimal.Zero;

            List<Type> bql = new List<Type>
			{
                typeof(Select4<,,>),
                 typeof(POReceiptLineSplit),
                 typeof(Where<POReceiptLineSplit.receiptNbr, Equal<Required<POReceiptLine.receiptNbr>>, And<POReceiptLineSplit.lineNbr, Equal<Required<POReceiptLine.lineNbr>>>>),
                 typeof(Aggregate<>),
                 typeof(GroupBy<,>),
                 typeof(POReceiptLineSplit.locationID),
                 typeof(GroupBy<,>),
                 typeof(POReceiptLineSplit.subItemID),
                 typeof(Sum<,>),
                 typeof(POReceiptLineSplit.baseQty),
                 typeof(GroupBy<>),
                 typeof(POReceiptLineSplit.lotSerialNbr)
              };

            InventoryItem ii = (InventoryItem)PXSelectorAttribute.Select<POReceiptLine.inventoryID>(_graph.Caches[typeof(POReceiptLine)], aLine);
            bool requierelotserial = ii.ValMethod == INValMethod.Specific;

            if (!requierelotserial)
				{
                bql.RemoveAt(bql.Count - 1);
                bql.RemoveAt(bql.Count - 1);
                bql[bql.Count - 2] = typeof(Sum<>);
				}

            PXView splitsView = new PXView(_graph, false, BqlCommand.CreateInstance(bql.ToArray()));

            bool hasSplits = false;
            foreach (POReceiptLineSplit split in splitsView.SelectMulti(aLine.ReceiptNbr, aLine.LineNbr))
				{
                hasSplits = true;
                decimal allocatingBase = split.BaseQty ?? 0m;
                shareAmt = CalcAllocationValue(aLCCode, split, aLine, baseTotal, toDistribute);
                shareAmt = PXDBCurrencyAttribute.BaseRound(_graph, shareAmt);

                //accu rounding
                shareAmt += PXCurrencyAttribute.BaseRound(_graph, (allocatedBase + allocatingBase) * toDistribute / baseTotal - shareAmt - allocatedAmt);

                if (shareAmt != Decimal.Zero)
					{
                    POReceiptLine newPOReceiptLine = (POReceiptLine)_graph.Caches[typeof(POReceiptLine)].CreateCopy(aLine);
                    newPOReceiptLine.LocationID = split.LocationID;
                    newPOReceiptLine.SiteID = split.SiteID;
                    newPOReceiptLine.SubItemID = split.SubItemID;
                    newPOReceiptLine.LotSerialNbr = requierelotserial ? split.LotSerialNbr : null;
                    result.Add(new POReceiptLineAdjustment(newPOReceiptLine, shareAmt, branchID));
                    allocatedAmt += shareAmt;
                    allocatedBase += allocatingBase;
					}
            }
            if (!hasSplits)
					{
						shareAmt = toDistribute;
                shareAmt = PXDBCurrencyAttribute.BaseRound(_graph, shareAmt);
					if (shareAmt != Decimal.Zero)
                    result.Add(new POReceiptLineAdjustment(aLine, shareAmt, branchID));

                allocatedAmt = shareAmt;
				}
            return allocatedAmt;
			}
        public void AllocateRestOverRCTLines(IList<POReceiptLineAdjustment> aLines, decimal rest)
			{
            if (rest != Decimal.Zero)
				{
                if (aLines.Count == 0)
                {
                    aLines.Add(new POReceiptLineAdjustment(new POReceiptLine(), rest, null));
				}
				else
				{
                    aLines[0] = new POReceiptLineAdjustment(aLines[0].Item1, aLines[0].Item2 + rest, aLines[0].BranchID);
                }
            }
        }
        public List<LandedCostHelper.POReceiptLineAdjustment> AllocateLCOverRCTLines(IEnumerable<POReceiptLine> aLines, LandedCostCode aLCCode, LandedCostTran aTran, IEnumerable<LandedCostTranSplit> aSplits)
		{
			Decimal toDistribute = aTran.LCAmount.Value;
			Decimal baseTotal = decimal.Zero;

			foreach (POReceiptLine iDet in aLines)
			{
				if (IsApplicable(aTran, aSplits, aLCCode, iDet))
				{
					baseTotal += GetBaseValue(aLCCode, iDet);
				}
				}

            List<POReceiptLineAdjustment> result = new List<POReceiptLineAdjustment>();
            Decimal leftToDistribute = toDistribute;

            foreach (POReceiptLine iDet in aLines)
            {
                var poreceipt = (POReceipt)PXSelect<POReceipt, Where<POReceipt.receiptType, Equal<Current<POReceiptLine.receiptType>>, And<POReceipt.receiptNbr, Equal<Current<POReceiptLine.receiptNbr>>>>>.SelectSingleBound(this._graph, new object[] { iDet });
                if (!IsApplicable(aTran, aSplits, aLCCode, iDet))
                    continue;
                leftToDistribute -= AllocateOverRCTLine(result, iDet, CalcAllocationValue(aLCCode, iDet, baseTotal, toDistribute), poreceipt.BranchID);
			}
            AllocateRestOverRCTLines(result ,leftToDistribute);

			return result;
		}

	} 
	#endregion
}
