using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Linq;
using PX.Common;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.GL;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.PM;
using PX.Objects.TX;
using PX.Objects.AP.Overrides.APDocumentRelease;
using PX.Objects.CA;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.DR;
using Avalara.AvaTax.Adapter;
using Avalara.AvaTax.Adapter.TaxService;
using Amount = PX.Objects.AR.ARReleaseProcess.Amount;

namespace PX.Objects.AP
{
	[System.SerializableAttribute()]
	public partial class BalancedAPDocument : APRegister
	{
		#region Selected
		public new abstract class selected : PX.Data.IBqlField
		{
		}
		#endregion
		#region DocType
		public new abstract class docType : PX.Data.IBqlField
		{
		}
		#endregion
		#region RefNbr
		public new abstract class refNbr : PX.Data.IBqlField
		{
		}
		#endregion
        #region OrigModule
        public new abstract class origModule : PX.Data.IBqlField
        {
        }
        #endregion
        #region OpenDoc
        public new abstract class openDoc : PX.Data.IBqlField
		{
		}
		#endregion
		#region Released
		public new abstract class released : PX.Data.IBqlField
		{
		}
		#endregion
		#region Hold
		public new abstract class hold : PX.Data.IBqlField
		{
		}
		#endregion
		#region Scheduled
		public new abstract class scheduled : PX.Data.IBqlField
		{
		}
		#endregion
		#region Voided
		public new abstract class voided : PX.Data.IBqlField
		{
		}
		#endregion
		#region Printed
		public new abstract class printed : PX.Data.IBqlField
		{
		}
		#endregion
		#region Prebooked
		public new abstract class prebooked : PX.Data.IBqlField
		{
		}
		#endregion
		#region Approved
		public new abstract class approved : IBqlField { }
		#endregion
		#region CreatedByID
		public new abstract class createdByID : PX.Data.IBqlField
		{
		}
		#endregion
		#region LastModifiedByID
		public new abstract class lastModifiedByID : PX.Data.IBqlField
		{
		}
		#endregion
		#region VendorRefNbr
		public abstract class vendorRefNbr : IBqlField
		{
		}
		protected String _VendorRefNbr;
		[PXString(40, IsUnicode = true)]
		[PXUIField(DisplayName = "Vendor Ref.")]
		public String VendorRefNbr
		{
			get
			{
				return _VendorRefNbr;
			}
			set
			{
				_VendorRefNbr = value;
			}
		}
		#endregion
		#region NoteID
		public new abstract class noteID : PX.Data.IBqlField
		{
		}
		#endregion
	}

	public class PXMassProcessException : PXException
	{
		protected Exception _InnerException;
		protected int _ListIndex;

		public int ListIndex
		{
			get
			{
				return this._ListIndex;
			}
		}

		public PXMassProcessException(int ListIndex, Exception InnerException)
			: base(InnerException is PXOuterException ? InnerException.Message + "\r\n" + String.Join("\r\n", ((PXOuterException)InnerException).InnerMessages) : InnerException.Message, InnerException)
		{
			this._ListIndex = ListIndex;
		}

		public PXMassProcessException(SerializationInfo info, StreamingContext context)
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

	[PX.Objects.GL.TableAndChartDashboardType]
	public class APDocumentRelease : PXGraph<APDocumentRelease>
	{
		public PXCancel<BalancedAPDocument> Cancel;
		[PXFilterable]
		public PXProcessingJoin<
			BalancedAPDocument,
				LeftJoin<APInvoice, 
					On<APInvoice.docType, Equal<BalancedAPDocument.docType>,
					And<APInvoice.refNbr, Equal<BalancedAPDocument.refNbr>>>,
				LeftJoin<APPayment, 
					On<APPayment.docType, Equal<BalancedAPDocument.docType>,
					And<APPayment.refNbr, Equal<BalancedAPDocument.refNbr>>>,
				InnerJoinSingleTable<Vendor, 
					On<Vendor.bAccountID, Equal<BalancedAPDocument.vendorID>>,
				LeftJoin<
					APAdjust, On<APAdjust.adjgDocType, Equal<BalancedAPDocument.docType>,
					And<APAdjust.adjgRefNbr, Equal<BalancedAPDocument.refNbr>,
					And<APAdjust.adjNbr, Equal<BalancedAPDocument.lineCntr>,
					And<APAdjust.hold, Equal<boolFalse>>>>>>>>>,
				Where2<
					Match<Vendor, Current<AccessInfo.userName>>,
					And<APRegister.hold, Equal<boolFalse>,
					And<APRegister.voided, Equal<boolFalse>,
					And<APRegister.scheduled, Equal<boolFalse>,
					And<APRegister.approved, Equal<boolTrue>,
					And<APRegister.docType, NotEqual<APDocType.check>,
					And<APRegister.docType, NotEqual<APDocType.quickCheck>,
					And<Where<
						BalancedAPDocument.released, Equal<boolFalse>,
						Or<BalancedAPDocument.openDoc, Equal<boolTrue>,
						And<APAdjust.adjdRefNbr, IsNotNull>>>>>>>>>>>> 
			APDocumentList;

		public static string[] TransClassesWithoutZeroPost = {GLTran.tranClass.Discount, GLTran.tranClass.RealizedAndRoundingGOL};

		public APDocumentRelease()
		{
			APSetup setup = APSetup.Current;
			APDocumentList.SetProcessDelegate(
				delegate(List<BalancedAPDocument> list)
				{
					List<APRegister> newlist = new List<APRegister>(list.Count);
					foreach (BalancedAPDocument doc in list)
					{
						newlist.Add(doc);
					}
					ReleaseDoc(newlist, true);
				}
			);
			APDocumentList.SetProcessCaption(ActionsMessages.Release);
			APDocumentList.SetProcessAllCaption(ActionsMessages.ReleaseAll);
			//APDocumentList.SetProcessAllVisible(false);
			PXNoteAttribute.ForcePassThrow<BalancedAPDocument.noteID>(APDocumentList.Cache);
		}

		public PXAction<BalancedAPDocument> ViewDocument;
		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable viewDocument(PXAdapter adapter)
		{
			if (this.APDocumentList.Current != null)
			{
				PXRedirectHelper.TryRedirect(APDocumentList.Cache, APDocumentList.Current, "Document", PXRedirectHelper.WindowMode.NewWindow);
			}
			return adapter.Get();
		}

		public static void ReleaseDoc(List<APRegister> list, bool isMassProcess) 
		{
			ReleaseDoc(list, isMassProcess, false);
		}

        public static void ReleaseDoc(List<APRegister> list, bool isMassProcess, List<Batch> externalPostList)
        {
            ReleaseDoc(list, isMassProcess, false, externalPostList);
        }
		public static void ReleaseDoc(List<APRegister> list, bool isMassProcess, bool isPrebooking) 
		{
			ReleaseDoc(list, isMassProcess, isPrebooking, null);
		}

		/// <summary>
		/// Static function for release of AP documents and posting of the released batch.
		/// Released batches will be posted if the corresponded flag in APSetup is set to true.
		/// SkipPost parameter is used to override this flag. 
		/// This function can not be called from inside of the covering DB transaction scope, unless skipPost is set to true.     
		/// </summary>
		/// <param name="list">List of the documents to be released</param>
		/// <param name="isMassProcess">Flag specifing if the function is called from mass process - affects error handling</param>
		/// <param name="skipPost"> Prevent Posting of the released batch(es). This parameter must be set to true if this function is called from "covering" DB transaction</param>
		public static void ReleaseDoc(List<APRegister> list, bool isMassProcess, bool isPrebooking, List<Batch> externalPostList)
		{
			bool failed = false;
			bool skipPost = (externalPostList != null);
			APReleaseProcess rg = PXGraph.CreateInstance<APReleaseProcess>();
			JournalEntry je = PXGraph.CreateInstance<JournalEntry>();
			//Field Verification can fail if GL module is not "Visible";therfore suppress it:
			je.FieldVerifying.AddHandler<GLTran.projectID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });
			je.FieldVerifying.AddHandler<GLTran.taskID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });
			je.RowInserting.AddHandler<GLTran>((sender, e) => { je.SetZeroPostIfUndefined((GLTran) e.Row, TransClassesWithoutZeroPost); });
			
			PostGraph pg = PXGraph.CreateInstance<PostGraph>();
			Dictionary<int,int> batchbind = new Dictionary<int,int>();
			bool lcProcessFailed = false;
			bool ppvProcessFailed = false;
			LandedCostProcess lcProcess = null; 

			for (int i = 0; i < list.Count; i++)
			{
				APRegister doc = list[i];
				List<INRegister> inDocs = null;
				List<INRegister> pPVAdjustments = new List<INRegister>();
				bool allocatePPVToInventory = rg.posetup != null && rg.posetup.PPVAllocationMode == PPVMode.Inventory;

				if (doc == null)
				{
					continue;
				}

				try
				{
					rg.Clear();

					rg.VerifyStockItemLineHasReceipt(doc);

					if (doc.Passed == true)
					{
						rg.TimeStamp = doc.tstamp;
					}

					List<APRegister> childs = rg.ReleaseDocProc(je, doc, isPrebooking, out inDocs);
					if (allocatePPVToInventory && inDocs != null)
						pPVAdjustments.Add(inDocs);

					int k;
					if ((k = je.created.IndexOf(je.BatchModule.Current)) >= 0 && batchbind.ContainsKey(k) == false)
					{
						batchbind.Add(k, i);
					}

					if (childs != null)
					{
						for (int j = 0; j < childs.Count; j++)
						{
							doc = childs[j];
							rg.Clear();
							rg.ReleaseDocProc(je, doc, isPrebooking, out inDocs);
							if (allocatePPVToInventory && inDocs != null)
								pPVAdjustments.Add(inDocs);
						}
					}

					if (string.IsNullOrEmpty(doc.WarningMessage))
						PXProcessing<APRegister>.SetInfo(i, ActionsMessages.RecordProcessed);
					else
					{
						PXProcessing<APRegister>.SetWarning(i, doc.WarningMessage);
					}
				}
				catch (Exception e)
				{
					je.CleanupCreated(batchbind.Keys);
					je.Clear();

					if (isMassProcess)
					{
						PXProcessing<APRegister>.SetError(i, e);
						failed = true;
					}
					else
					{
						throw new PXMassProcessException(i, e);
					}
				}

				try
				{
					if (allocatePPVToInventory && pPVAdjustments.Count > 0)
					{
						INDocumentRelease.ReleaseDoc(pPVAdjustments, false);
					}
				}
				catch (Exception e)
				{
					PXProcessing<APRegister>.SetError(i, e);
					ppvProcessFailed = true;
				}

				try
				{
					if (!isPrebooking)
					{
						List<LandedCostTran> lcList = new List<LandedCostTran>();
						foreach (LandedCostTran iTran in PXSelectReadonly<LandedCostTran, Where<LandedCostTran.aPDocType, Equal<Required<LandedCostTran.aPDocType>>,
																	And<LandedCostTran.aPRefNbr, Equal<Required<LandedCostTran.aPRefNbr>>,
																		And<LandedCostTran.source, Equal<LandedCostTranSource.fromAP>,
																		And<LandedCostTran.processed, Equal<False>>>>>>.Select(rg, doc.DocType, doc.RefNbr))
						{

							lcList.Add(iTran);

						}
						if (lcList.Count > 0)
						{
							if (lcProcess == null)
								lcProcess = PXGraph.CreateInstance<LandedCostProcess>();
							lcProcess.ReleaseLCTrans(lcList, null, null);
						}
					}
				}
				catch (Exception e)
				{
					PXProcessing<APRegister>.SetError(i, e);
					lcProcessFailed = true;
				}
			}

			if (skipPost) 
			{
				if (rg.AutoPost)
					externalPostList.AddRange(je.created);
			}
			else
			{
				for (int i = 0; i < je.created.Count; i++)
				{
					Batch batch = je.created[i];
					try
					{
						if (rg.AutoPost)
						{
							pg.Clear();
							pg.PostBatchProc(batch);
						}
					}
					catch (Exception e)
					{
						if (isMassProcess)
						{
							failed = true;
							PXProcessing<APRegister>.SetError(batchbind[i], e);
						}
						else
						{
							throw new PXMassProcessException(batchbind[i], e);
						}
					}
				}
			}
			if (failed)
			{
				throw new PXException(GL.Messages.DocumentsNotReleased);
			}
			if (ppvProcessFailed)
			{
				throw new PXException(Messages.ProcessingOfPPVTransactionForAPDocFailed);
			}
			if (lcProcessFailed) 
			{
				throw new PXException(Messages.ProcessingOfLandedCostTransForAPDocFailed);
			}
		}

		public static void VoidDoc(List<APRegister> list)
		{
			bool failed = false;
			APReleaseProcess rg = PXGraph.CreateInstance<APReleaseProcess>();
			JournalEntry je = PXGraph.CreateInstance<JournalEntry>();
			//Field Verification can fail if GL module is not "Visible";therfore suppress it:
			je.FieldVerifying.AddHandler<GLTran.projectID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });
			je.FieldVerifying.AddHandler<GLTran.taskID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });
			je.RowInserting.AddHandler<GLTran>((sender, e) => { je.SetZeroPostIfUndefined((GLTran) e.Row, TransClassesWithoutZeroPost); });

			PostGraph pg = PXGraph.CreateInstance<PostGraph>();
			Dictionary<int, int> batchbind = new Dictionary<int, int>();
			for (int i = 0; i < list.Count; i++)
			{
				APRegister doc = list[i];
				if (doc == null)
				{
					continue;
				}
				try
				{
					rg.Clear();
					if (doc.Passed == true)
					{
						rg.TimeStamp = doc.tstamp;
					}
					rg.VoidDocProc(je, doc);
					PXProcessing<APRegister>.SetInfo(i, ActionsMessages.RecordProcessed);
					int k;
					if ((k = je.created.IndexOf(je.BatchModule.Current)) >= 0 && batchbind.ContainsKey(k) == false)
					{
						batchbind.Add(k, i);
					}
				}
				catch (Exception e)
				{
					throw new PXMassProcessException(i, e);
				}
			}

			for (int i = 0; i < je.created.Count; i++)
			{
				Batch batch = je.created[i];
				try
				{
					if (rg.AutoPost)
					{
						pg.Clear();
						pg.PostBatchProc(batch);
					}
				}
				catch (Exception e)
				{
					throw new PXMassProcessException(batchbind[i], e);
				}
			}
			if (failed)
			{
				throw new PXException(GL.Messages.DocumentsNotReleased);
			}
		}

		protected virtual IEnumerable apdocumentlist()
		{
			PXResultset<BalancedAPDocument, APInvoice, APPayment, Vendor, APAdjust> ret = new PXResultset<BalancedAPDocument, APInvoice, APPayment, Vendor, APAdjust>();

			foreach (PXResult<BalancedAPDocument, APInvoice, APPayment, Vendor, APAdjust> res in 
				PXSelectJoinGroupBy<
					BalancedAPDocument,
					LeftJoin<APInvoice, 
						On<APInvoice.docType, Equal<BalancedAPDocument.docType>,
						And<APInvoice.refNbr, Equal<BalancedAPDocument.refNbr>>>,
					LeftJoin<APPayment, 
						On<APPayment.docType, Equal<BalancedAPDocument.docType>,
						And<APPayment.refNbr, Equal<BalancedAPDocument.refNbr>>>,
					InnerJoinSingleTable<Vendor, 
						On<Vendor.bAccountID, Equal<BalancedAPDocument.vendorID>>,
					LeftJoin<APAdjust, 
						On<APAdjust.adjgDocType, Equal<BalancedAPDocument.docType>, 
						And<APAdjust.adjgRefNbr, Equal<BalancedAPDocument.refNbr>, 
						And<APAdjust.adjNbr, Equal<BalancedAPDocument.lineCntr>,
						And<APAdjust.hold, Equal<boolFalse>>>>>>>>>,
					Where2<
						Match<Vendor, Current<AccessInfo.userName>>, 
						And<APRegister.hold, Equal<boolFalse>, 
						And<APRegister.voided, Equal<boolFalse>, 
						And<APRegister.scheduled, Equal<boolFalse>,
						And<APRegister.approved, Equal<boolTrue>,
						And<APRegister.docType, NotEqual<APDocType.check>, 
						And<APRegister.docType, NotEqual<APDocType.quickCheck>,
                        And2<Where<APInvoice.refNbr,IsNotNull,Or<APPayment.refNbr, IsNotNull>>,
						And<Where<
							BalancedAPDocument.released, Equal<boolFalse>, 
							Or<BalancedAPDocument.openDoc,Equal<boolTrue>, 
							And<APAdjust.adjdRefNbr, IsNotNull>>>>>>>>>>>>,
					Aggregate<
						GroupBy<BalancedAPDocument.docType, 
						GroupBy<BalancedAPDocument.refNbr,
						GroupBy<BalancedAPDocument.released, 
						GroupBy<BalancedAPDocument.prebooked,
						GroupBy<BalancedAPDocument.openDoc, 
						GroupBy<BalancedAPDocument.hold, 
						GroupBy<BalancedAPDocument.scheduled, 
						GroupBy<BalancedAPDocument.voided,
						GroupBy<BalancedAPDocument.printed,
						GroupBy<BalancedAPDocument.approved,
						GroupBy<BalancedAPDocument.noteID, 
						GroupBy<BalancedAPDocument.createdByID, 
						GroupBy<BalancedAPDocument.lastModifiedByID>>>>>>>>>>>>>>>
					.Select(this))
			{
				BalancedAPDocument apdoc = (BalancedAPDocument)res;
				apdoc = APDocumentList.Locate(apdoc) ?? apdoc;
				APInvoice invoice = (APInvoice)res;
				APPayment payment = (APPayment)res; 
               
				if (invoice != null && string.IsNullOrEmpty(invoice.InvoiceNbr) == false)
				{
					apdoc.VendorRefNbr = invoice.InvoiceNbr;
				}
				else if (payment != null && string.IsNullOrEmpty(payment.ExtRefNbr) == false)
				{
					apdoc.VendorRefNbr = payment.ExtRefNbr;
				}
				APAdjust adj = (APAdjust)res;
				if (true)
				{
					if (adj.AdjdRefNbr != null)
					{
						apdoc.DocDate = adj.AdjgDocDate;
						apdoc.TranPeriodID = adj.AdjgTranPeriodID;
						apdoc.FinPeriodID = adj.AdjgFinPeriodID;
					}
					ret.Add(new PXResult<BalancedAPDocument, APInvoice, APPayment, Vendor, APAdjust>(apdoc, res, res, res, res));
				}
			}
			return ret;
		}

		public PXSetup<APSetup> APSetup;
	}

	public class APPayment_CurrencyInfo_Currency_Vendor : PXSelectJoin<APPayment, InnerJoin<CurrencyInfo, On<CurrencyInfo.curyInfoID, Equal<APPayment.curyInfoID>>, InnerJoin<Currency, On<Currency.curyID,Equal<CurrencyInfo.curyID>>, LeftJoin<Vendor, On<Vendor.bAccountID, Equal<APPayment.vendorID>>, LeftJoin<CashAccount, On<CashAccount.cashAccountID, Equal<APPayment.cashAccountID>>>>>>, Where<APPayment.docType, Equal<Required<APPayment.docType>>, And<APPayment.refNbr, Equal<Required<APPayment.refNbr>>>>>
	{
		public APPayment_CurrencyInfo_Currency_Vendor(PXGraph graph)
			: base(graph)
		{
		}
	}

	public class APInvoice_CurrencyInfo_Terms_Vendor : PXSelectJoin<APInvoice, InnerJoin<CurrencyInfo, On<CurrencyInfo.curyInfoID, Equal<APInvoice.curyInfoID>>, LeftJoin<Terms, On<Terms.termsID, Equal<APInvoice.termsID>>, LeftJoin<Vendor, On<Vendor.bAccountID, Equal<APInvoice.vendorID>>>>>, Where<APInvoice.docType, Equal<Required<APInvoice.docType>>, And<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>>
	{
		public APInvoice_CurrencyInfo_Terms_Vendor(PXGraph graph)
			: base(graph)
		{
		}
	}

	[PXHidden]
	public class APReleaseProcess : PXGraph<APReleaseProcess>
	{
		public PXSelect<APRegister> APDocument;

		public PXSelectJoin<
			APTran, 
				LeftJoin<APTax, 
					On<APTax.tranType, Equal<APTran.tranType>, 
					And<APTax.refNbr, Equal<APTran.refNbr>, 
					And<APTax.lineNbr, Equal<APTran.lineNbr>>>>, 
				LeftJoin<Tax, 
					On<Tax.taxID, Equal<APTax.taxID>>, 
				LeftJoin<DRDeferredCode, 
					On<DRDeferredCode.deferredCodeID, Equal<APTran.deferredCode>>, 
				LeftJoin<LandedCostTran,
					On<LandedCostTran.lCTranID,Equal<APTran.lCTranID>,
					And<LandedCostTran.iNDocType,IsNotNull>>, 
				LeftJoin<LandedCostCode,
					On<LandedCostCode.landedCostCodeID, Equal<LandedCostTran.landedCostCodeID>>,
				LeftJoin<InventoryItem, 
					On<InventoryItem.inventoryID, Equal<APTran.inventoryID>>>>>>>>, 
			Where<
				APTran.tranType, Equal<Required<APTran.tranType>>, 
				And<APTran.refNbr, Equal<Required<APTran.refNbr>>>>, 
			OrderBy<
				Asc<APTran.lineNbr, 
				Asc<Tax.taxCalcLevel, 
				Asc<Tax.taxType>>>>> 
			APTran_TranType_RefNbr;

		public PXSelectJoin<APTaxTran, InnerJoin<Tax, On<Tax.taxID, Equal<APTaxTran.taxID>>, LeftJoin<APInvoice, On<APInvoice.docType, Equal<APTaxTran.origTranType>, And<APInvoice.refNbr, Equal<APTaxTran.origRefNbr>>>>>, Where<APTaxTran.module, Equal<BatchModule.moduleAP>, And<APTaxTran.tranType, Equal<Required<APTaxTran.tranType>>, And<APTaxTran.refNbr, Equal<Required<APTaxTran.refNbr>>>>>, OrderBy<Asc<Tax.taxCalcLevel>>> APTaxTran_TranType_RefNbr;
		public PXSelect<SVATConversionHist> SVATConversionHistory;
		public PXSelect<Batch> Batch;

		public APInvoice_CurrencyInfo_Terms_Vendor APInvoice_DocType_RefNbr;
		public APPayment_CurrencyInfo_Currency_Vendor APPayment_DocType_RefNbr;

		public PXSelectJoin<
			APAdjust, 
				InnerJoin<CurrencyInfo, 
					On<CurrencyInfo.curyInfoID, Equal<APAdjust.adjdCuryInfoID>>, 
				InnerJoin<Currency, 
					On<Currency.curyID, Equal<CurrencyInfo.curyID>>, 
				LeftJoinSingleTable<APInvoice, 
					On<APInvoice.docType, Equal<APAdjust.adjdDocType>, 
					And<APInvoice.refNbr, Equal<APAdjust.adjdRefNbr>>>, 
				LeftJoinSingleTable<APPayment, 
					On<APPayment.docType, Equal<APAdjust.adjdDocType>, 
					And<APPayment.refNbr, Equal<APAdjust.adjdRefNbr>>>,
				InnerJoin<Standalone.APRegisterAlias,
					On<Standalone.APRegisterAlias.docType, Equal<APAdjust.adjdDocType>,
					And<Standalone.APRegisterAlias.refNbr, Equal<APAdjust.adjdRefNbr>>>>>>>>, 
			Where<
				APAdjust.adjgDocType, Equal<Required<APAdjust.adjgDocType>>, 
				And<APAdjust.adjgRefNbr, Equal<Required<APAdjust.adjgRefNbr>>, 
				And<APAdjust.adjNbr, Equal<Required<APAdjust.adjNbr>>>>>> 
			APAdjust_AdjgDocType_RefNbr_VendorID;

        public PXSelect<APPaymentChargeTran, Where<APPaymentChargeTran.docType, Equal<Required<APPaymentChargeTran.docType>>, And<APPaymentChargeTran.refNbr, Equal<Required<APPaymentChargeTran.refNbr>>>>> APPaymentChargeTran_DocType_RefNbr;

		public PXSelect<APTran, Where<APTran.tranType, Equal<Required<APTran.tranType>>, And<APTran.refNbr, Equal<Required<APTran.refNbr>>, And<APTran.box1099, IsNotNull>>>> AP1099Tran_Select;
		public PXSelect<AP1099Hist> AP1099History_Select;
		public PXSelect<AP1099Yr> AP1099Year_Select;

		public PXSelectJoin<APTaxTran, InnerJoin<Tax, On<Tax.taxID, Equal<APTaxTran.taxID>>>, Where<APTaxTran.module, Equal<BatchModule.moduleAP>, And<APTaxTran.tranType, Equal<Required<APTaxTran.tranType>>, And<APTaxTran.refNbr, Equal<Required<APTaxTran.refNbr>>, And<Tax.taxType,Equal<CSTaxType.withholding>>>>>> WHTax_TranType_RefNbr;

		public PXSelect<CATran> CashTran;
		public PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Required<CurrencyInfo.curyInfoID>>>> CurrencyInfo_CuryInfoID;
		public PXSelect<PTInstTran, Where<PTInstTran.origTranType, Equal<Required<APPayment.docType>>,
									And<PTInstTran.origRefNbr, Equal<Required<APPayment.refNbr>>>>> ptInstanceTrans;
		public PXSelect<PO.POReceiptLineR1, Where<PO.POReceiptLineR1.receiptNbr, Equal<Required<PO.POReceiptLineR1.receiptNbr>>,
															And<PO.POReceiptLineR1.lineNbr,Equal<Required<PO.POReceiptLineR1.lineNbr>>>>> poReceiptLineUPD;
		public PXSelect<PO.POReceipt> poReceiptUPD;
		public PXSelectJoin<POLineUOpen,
						LeftJoin<POLine, On<POLine.orderType, Equal<POLineUOpen.orderType>, 
							And<POLine.orderNbr, Equal<POLineUOpen.orderNbr>, 
							And<POLine.lineNbr, Equal<POLineUOpen.lineNbr>>>>>,
						Where<POLineUOpen.orderType, Equal<Required<POLineUOpen.orderType>>, 
							And<POLineUOpen.orderNbr,Equal<Required<POLineUOpen.orderNbr>>, 
							And<POLineUOpen.lineNbr, Equal<Required<POLineUOpen.lineNbr>>>>>> poOrderLineUPD;
		public PXSelect<PO.POOrder, Where<PO.POOrder.orderType, Equal<Required<PO.POOrder.orderType>>,
								And<PO.POOrder.orderNbr, Equal<Required<PO.POOrder.orderNbr>>>>> poOrderUPD;
		public PXSelect<POItemCostManager.POVendorInventoryPriceUpdate> poVendorInventoryPriceUpdate;
		public PXSetup<GLSetup> glsetup;
		public PXSelect<Tax> taxes;

		private APSetup _apsetup;

		public APSetup apsetup
		{
			get { return _apsetup ?? (_apsetup = PXSelect<APSetup>.Select(this)); }
				}
		private POSetup _posetup;

		public POSetup posetup
		{
			get { return _posetup ?? (_posetup = PXSelect<POSetup>.Select(this)); }
				}

		public bool AutoPost
		{
			get
			{
				return (bool)apsetup.AutoPost;
			}
		}

		public bool SummPost
		{
			get
			{
				return (apsetup.TransactionPosting == "S");
			}
		}

        public string InvoiceRounding
        {
            get
            {
				return apsetup.InvoiceRounding;
            }
        }

        public decimal? InvoicePrecision
        {
            get
            {
                return apsetup.InvoicePrecision;
            }
        }

		public bool? RequireControlTaxTotal
		{
			get { return apsetup.RequireControlTaxTotal == true && PXAccess.FeatureInstalled<FeaturesSet.netGrossEntryMode>(); }
		}

		public decimal? RoundingLimit
		{
			get { return glsetup.Current.RoundingLimit; }
		}

		protected APInvoiceEntry _ie;
		public APInvoiceEntry ie
		{
			get { return _ie ?? (_ie = CreateInstance<APInvoiceEntry>()); }
		}

		[PXDBString(6, IsFixed = true)]
		[PXDefault()]
		protected virtual void APPayment_AdjFinPeriodID_CacheAttached(PXCache sender)
		{ }

		[PXDBString(6, IsFixed = true)]
		[PXDefault()]
		protected virtual void APPayment_AdjTranPeriodID_CacheAttached(PXCache sender)
		{ }

		[VendorActive(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true, DisplayName = "Vendor ID")]
		[PXDefault(typeof(Vendor.bAccountID))]
		public virtual void POVendorInventoryPriceUpdate_VendorID_CacheAttached(PXCache sender) { }

		[PXDBString(1, IsFixed = true)]
		public virtual void Tax_TaxType_CacheAttached(PXCache sender) { }

		[PXDBString(1, IsFixed = true)]
		public virtual void Tax_TaxCalcLevel_CacheAttached(PXCache sender) { }

		public APReleaseProcess()
		{
			//APDocument.Cache = new PXCache<APRegister>(this);
			OpenPeriodAttribute.SetValidatePeriod<APRegister.finPeriodID>(APDocument.Cache, null, PeriodValidation.Nothing);
			OpenPeriodAttribute.SetValidatePeriod<APPayment.adjFinPeriodID>(APPayment_DocType_RefNbr.Cache, null, PeriodValidation.Nothing);

			PXDBDefaultAttribute.SetDefaultForUpdate<APAdjust.vendorID>(Caches[typeof(APAdjust)], null, false);
			PXDBDefaultAttribute.SetDefaultForUpdate<APAdjust.adjgDocType>(Caches[typeof(APAdjust)], null, false);
			PXDBDefaultAttribute.SetDefaultForUpdate<APAdjust.adjgRefNbr>(Caches[typeof(APAdjust)], null, false);
			PXDBLiteDefaultAttribute.SetDefaultForUpdate<APAdjust.adjgCuryInfoID>(Caches[typeof(APAdjust)], null, false);
			PXDBDefaultAttribute.SetDefaultForUpdate<APAdjust.adjgDocDate>(Caches[typeof(APAdjust)], null, false);
			PXDBDefaultAttribute.SetDefaultForUpdate<APAdjust.adjgFinPeriodID>(Caches[typeof(APAdjust)], null, false);
			PXDBDefaultAttribute.SetDefaultForUpdate<APAdjust.adjgTranPeriodID>(Caches[typeof(APAdjust)], null, false);

			PXDBDefaultAttribute.SetDefaultForUpdate<APTran.tranType>(Caches[typeof(APTran)], null, false);
			PXDBDefaultAttribute.SetDefaultForUpdate<APTran.refNbr>(Caches[typeof(APTran)], null, false);
			PXDBLiteDefaultAttribute.SetDefaultForUpdate<APTran.curyInfoID>(Caches[typeof(APTran)], null, false);
			PXDBDefaultAttribute.SetDefaultForUpdate<APTran.tranDate>(Caches[typeof(APTran)], null, false);
			PXDBDefaultAttribute.SetDefaultForUpdate<APTran.finPeriodID>(Caches[typeof(APTran)], null, false);
			PXDBDefaultAttribute.SetDefaultForUpdate<APTran.vendorID>(Caches[typeof(APTran)], null, false);

			PXDBDefaultAttribute.SetDefaultForInsert<APTaxTran.tranType>(Caches[typeof(APTaxTran)], null, false);
			PXDBDefaultAttribute.SetDefaultForInsert<APTaxTran.refNbr>(Caches[typeof(APTaxTran)], null, false);
			PXDBLiteDefaultAttribute.SetDefaultForInsert<APTaxTran.curyInfoID>(Caches[typeof(APTaxTran)], null, false);
			PXDBDefaultAttribute.SetDefaultForInsert<APTaxTran.tranDate>(Caches[typeof(APTaxTran)], null, false);
			PXDBDefaultAttribute.SetDefaultForInsert<APTaxTran.taxZoneID>(Caches[typeof(APTaxTran)], null, false);

			PXDBDefaultAttribute.SetDefaultForUpdate<APTaxTran.tranType>(Caches[typeof(APTaxTran)], null, false);
			PXDBDefaultAttribute.SetDefaultForUpdate<APTaxTran.refNbr>(Caches[typeof(APTaxTran)], null, false);
			PXDBLiteDefaultAttribute.SetDefaultForUpdate<APTaxTran.curyInfoID>(Caches[typeof(APTaxTran)], null, false);
			PXDBDefaultAttribute.SetDefaultForUpdate<APTaxTran.tranDate>(Caches[typeof(APTaxTran)], null, false);
			PXDBDefaultAttribute.SetDefaultForUpdate<APTaxTran.taxZoneID>(Caches[typeof(APTaxTran)], null, false);
			PXDBDefaultAttribute.SetDefaultForUpdate<POReceiptTax.receiptNbr>(Caches[typeof(POReceiptTax)], null, false);
			PXDBDefaultAttribute.SetDefaultForUpdate<POReceiptTaxTran.receiptNbr>(Caches[typeof(POReceiptTaxTran)], null, false);
		}

        protected virtual void APPayment_CashAccountID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            e.Cancel = true;
        }

        protected virtual void APPayment_PaymentMethodID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            e.Cancel = true;
        }

        protected virtual void APPayment_ExtRefNbr_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            e.Cancel = true;
        }
        
		protected virtual void APPayment_FinPeriodID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.Cancel = true;
		}

        protected virtual void APRegister_FinPeriodID_CommandPreparing(PXCache sender, PXCommandPreparingEventArgs e)
		{
			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Update)
			{
				e.FieldName = string.Empty;
				e.Cancel = true;
			}
		}

		protected virtual void APRegister_TranPeriodID_CommandPreparing(PXCache sender, PXCommandPreparingEventArgs e)
		{
			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Update)
			{
				e.FieldName = string.Empty;
				e.Cancel = true;
			}
		}

		protected virtual void APRegister_DocDate_CommandPreparing(PXCache sender, PXCommandPreparingEventArgs e)
		{
			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Update)
			{
				e.FieldName = string.Empty;
				e.Cancel = true;
			}
		}

		protected virtual void APPayment_FinPeriodID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			e.Cancel = true;
		}

		protected virtual void CATran_ReferenceID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			e.Cancel = true;
		}

		protected virtual void APAdjust_AdjdRefNbr_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			e.Cancel = true;
		}

		protected virtual void APTran_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
		{
			e.Cancel = _IsIntegrityCheck;
		}

		private APHist CreateHistory(int? BranchID, int? AccountID, int? SubID, int? VendorID, string PeriodID)
		{
			APHist accthist = new APHist();
			accthist.BranchID = BranchID;
			accthist.AccountID = AccountID;
			accthist.SubID = SubID;
			accthist.VendorID = VendorID;
			accthist.FinPeriodID = PeriodID;
			return (APHist)Caches[typeof(APHist)].Insert(accthist);
		}

		private CuryAPHist CreateHistory(int? BranchID, int? AccountID, int? SubID, int? VendorID, string CuryID, string PeriodID)
		{
			CuryAPHist accthist = new CuryAPHist();
			accthist.BranchID = BranchID;
			accthist.AccountID = AccountID;
			accthist.SubID = SubID;
			accthist.VendorID = VendorID;
			accthist.CuryID = CuryID;
			accthist.FinPeriodID = PeriodID;
			return (CuryAPHist)Caches[typeof(CuryAPHist)].Insert(accthist);
		}

		private class APHistBucket
		{
			public int? apAccountID = null;
			public int? apSubID = null;
			public decimal SignPayments = 0m;
			public decimal SignDeposits = 0m;
			public decimal SignPurchases = 0m;
			public decimal SignDrAdjustments = 0m;
			public decimal SignCrAdjustments = 0m;
			public decimal SignDiscTaken = 0m;
			public decimal SignWhTax = 0m;
			public decimal SignRGOL = 0m;
			public decimal SignPtd = 0m;

			public APHistBucket(GLTran tran, string TranType)
			{
				apAccountID = tran.AccountID;
				apSubID = tran.SubID;

				switch (TranType + tran.TranClass)
				{
					case "QCKN":
						SignPurchases = 1m;
						SignPayments = 1m;
						SignPtd = 0m;
						break;
					case "VQCN":
						SignPurchases = 1m;
						SignPayments = 1m;
						SignPtd = 0m;
						break;
					case "INVN":
						SignPurchases = -1m;
						SignPtd = -1m;
						break;
					case "ACRN":
						SignCrAdjustments = -1m;
						SignPtd = -1m;
						break;
					case "ADRP":
					case "ADRN":
						SignDrAdjustments = 1m;
						SignPtd = -1m;
						break;
					case "ADRR":
						apAccountID = tran.OrigAccountID;
						apSubID = tran.OrigSubID;
						SignDrAdjustments = 1m;
						SignRGOL = -1m;
						break;
					case "ADRD":
						apAccountID = tran.OrigAccountID;
						apSubID = tran.OrigSubID;
						SignDrAdjustments = 1m;
						SignDiscTaken = -1m;
						break;
					case "VCKP":
					case "VCKN":
					case "CHKP":
					case "CHKN":
					case "PPMN":
					case "REFP":
					case "REFN":
						SignPayments = 1m;
						SignPtd = -1m;
						break;
					case "VCKR":
					case "CHKR":
					case "PPMR":
					case "REFR":
					case "QCKR":
					case "VQCR":
						apAccountID = tran.OrigAccountID;
						apSubID = tran.OrigSubID;
						SignPayments = 1m;
						SignRGOL = -1m;
						break;
					case "VCKD":
					case "CHKD":
					case "PPMD":
					case "REFD": //not really happens
					case "QCKD":
					case "VQCD":
						apAccountID = tran.OrigAccountID;
						apSubID = tran.OrigSubID;
						SignPayments = 1m;
						SignDiscTaken = -1m;
						break;
					case "PPMP":
						SignDeposits = 1m;
						break;
					case "PPMU":
						SignDeposits = 1m;
						break;
					case "CHKU":
					case "VCKU":
						SignDeposits = 1m;
						break;
					case "REFU":
						SignDeposits = 1m;
						break;
					case "VCKW":
					case "PPMW":
					case "CHKW":
					case "QCKW":
					case "VQCW":
						apAccountID = tran.OrigAccountID;
						apSubID = tran.OrigSubID;
						SignPayments = 1m;
						SignWhTax = -1m;
						break;

				}
			}
		}

		private void UpdateHist<History>(History accthist, APHistBucket bucket, bool FinFlag, GLTran tran)
			where History: class, IBaseAPHist
		{
			if (_IsIntegrityCheck == false || accthist.DetDeleted == false)
			{
				accthist.FinFlag = FinFlag;
				accthist.PtdPayments += bucket.SignPayments * (tran.DebitAmt - tran.CreditAmt);
				accthist.PtdPurchases += bucket.SignPurchases * (tran.DebitAmt - tran.CreditAmt);
				accthist.PtdCrAdjustments += bucket.SignCrAdjustments * (tran.DebitAmt - tran.CreditAmt);
				accthist.PtdDrAdjustments += bucket.SignDrAdjustments * (tran.DebitAmt - tran.CreditAmt);
				accthist.PtdDiscTaken += bucket.SignDiscTaken * (tran.DebitAmt - tran.CreditAmt);
				accthist.PtdWhTax += bucket.SignWhTax * (tran.DebitAmt - tran.CreditAmt);
				accthist.PtdRGOL += bucket.SignRGOL * (tran.DebitAmt - tran.CreditAmt);
				accthist.YtdBalance += bucket.SignPtd * (tran.DebitAmt - tran.CreditAmt);
				accthist.PtdDeposits += bucket.SignDeposits * (tran.DebitAmt - tran.CreditAmt);
				accthist.YtdDeposits += bucket.SignDeposits * (tran.DebitAmt - tran.CreditAmt);
			}
		}

		private void UpdateFinHist<History>(History accthist, APHistBucket bucket, GLTran tran)
			where History : class, IBaseAPHist
		{
			UpdateHist<History>(accthist, bucket, true, tran);
		}

		private void UpdateTranHist<History>(History accthist, APHistBucket bucket, GLTran tran)
			where History : class, IBaseAPHist
		{
			UpdateHist<History>(accthist, bucket, false, tran);
		}

		private void CuryUpdateHist<History>(History accthist, APHistBucket bucket, bool FinFlag, GLTran tran)
			where History : class, ICuryAPHist, IBaseAPHist
		{
			if (_IsIntegrityCheck == false || accthist.DetDeleted == false)
			{
				UpdateHist<History>(accthist, bucket, FinFlag, tran);

				accthist.FinFlag = FinFlag;

				accthist.CuryPtdPayments += bucket.SignPayments * (tran.CuryDebitAmt - tran.CuryCreditAmt);
				accthist.CuryPtdPurchases += bucket.SignPurchases * (tran.CuryDebitAmt - tran.CuryCreditAmt);
				accthist.CuryPtdCrAdjustments += bucket.SignCrAdjustments * (tran.CuryDebitAmt - tran.CuryCreditAmt);
				accthist.CuryPtdDrAdjustments += bucket.SignDrAdjustments * (tran.CuryDebitAmt - tran.CuryCreditAmt);
				accthist.CuryPtdDiscTaken += bucket.SignDiscTaken * (tran.CuryDebitAmt - tran.CuryCreditAmt);
				accthist.CuryPtdWhTax += bucket.SignWhTax * (tran.CuryDebitAmt - tran.CuryCreditAmt);
				accthist.CuryYtdBalance += bucket.SignPtd * (tran.CuryDebitAmt - tran.CuryCreditAmt);
				accthist.CuryPtdDeposits += bucket.SignDeposits * (tran.CuryDebitAmt - tran.CuryCreditAmt);
				accthist.CuryYtdDeposits += bucket.SignDeposits * (tran.CuryDebitAmt - tran.CuryCreditAmt);
			}
		}

		private void CuryUpdateFinHist<History>(History accthist, APHistBucket bucket, GLTran tran)
			where History : class, ICuryAPHist, IBaseAPHist
		{
			CuryUpdateHist<History>(accthist, bucket, true, tran);
		}

		private void CuryUpdateTranHist<History>(History accthist, APHistBucket bucket, GLTran tran)
			where History : class, ICuryAPHist, IBaseAPHist
		{
			CuryUpdateHist<History>(accthist, bucket, false, tran);
		}

		private void UpdateHistory(GLTran tran, Vendor vend)
		{
			UpdateHistory(tran, vend.BAccountID);
		}
		private void UpdateHistory(GLTran tran, int? vendorID) 
		{
			string HistTranType = tran.TranType;
			if (tran.TranType == APDocType.VoidCheck)
			{
				APRegister doc = PXSelect<APRegister, Where<APRegister.vendorID, Equal<Required<APRegister.vendorID>>, And<APRegister.refNbr, Equal<Required<APRegister.refNbr>>, And<Where<APRegister.docType, Equal<APDocType.check>, Or<APRegister.docType, Equal<APDocType.prepayment>>>>>>, OrderBy<Asc<Switch<Case<Where<APRegister.docType, Equal<APDocType.check>>, int0>, int1>, Asc<APRegister.docType, Asc<APRegister.refNbr>>>>>.Select(this, vendorID, tran.RefNbr);
				if (doc != null)
				{
					HistTranType = doc.DocType;
				}
			}
			APHistBucket bucket = new APHistBucket(tran, HistTranType);
			{
				APHist accthist = CreateHistory(tran.BranchID, bucket.apAccountID, bucket.apSubID, vendorID, tran.FinPeriodID);
				if (accthist != null)
				{
					UpdateFinHist<APHist>(accthist, bucket, tran);
				}
			}

			{
				APHist accthist = CreateHistory(tran.BranchID, bucket.apAccountID, bucket.apSubID, vendorID, tran.TranPeriodID);
				if (accthist != null)
				{
					UpdateTranHist<APHist>(accthist, bucket, tran);
				}
			}
		}

		private void UpdateHistory(GLTran tran, Vendor vend, CurrencyInfo info)
		{
			UpdateHistory(tran, vend.BAccountID, info.CuryID);
		}
		private void UpdateHistory(GLTran tran, int? vendorID, string aCuryID)
		{
			string HistTranType = tran.TranType;
			if (tran.TranType == APDocType.VoidCheck)
			{
				APRegister doc = PXSelect<APRegister, Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>, And<Where<APRegister.docType, Equal<APDocType.check>, Or<APRegister.docType, Equal<APDocType.prepayment>>>>>, OrderBy<Asc<Switch<Case<Where<APRegister.docType, Equal<APDocType.check>>, int0>, int1>, Asc<APRegister.docType, Asc<APRegister.refNbr>>>>>.Select(this, tran.RefNbr);
				if (doc != null)
				{
					HistTranType = doc.DocType;
				}
			}

			APHistBucket bucket = new APHistBucket(tran, HistTranType);
			{
				CuryAPHist accthist = CreateHistory(tran.BranchID, bucket.apAccountID, bucket.apSubID, vendorID, aCuryID, tran.FinPeriodID);
				if (accthist != null)
				{
					CuryUpdateFinHist<CuryAPHist>(accthist, bucket, tran);
				}
			}

			{
				CuryAPHist accthist = CreateHistory(tran.BranchID, bucket.apAccountID, bucket.apSubID, vendorID, aCuryID, tran.TranPeriodID);
				if (accthist != null)
				{
					CuryUpdateTranHist<CuryAPHist>(accthist, bucket, tran);
				}
			}
		}

		private List<APRegister> CreateInstallments(PXResult<APInvoice, CurrencyInfo, Terms, Vendor> res)
		{
			APInvoice apdoc = (APInvoice)res;
			CurrencyInfo info = (CurrencyInfo)res;
			Terms terms = (Terms)res;
			Vendor vendor = (Vendor)res;
			List<APRegister> ret = new List<APRegister>();

			decimal CuryTotalInstallments = 0m;

			APInvoiceEntry docgraph = PXGraph.CreateInstance<APInvoiceEntry>();

			PXResultset<TermsInstallments> installments = TermsAttribute.SelectInstallments(this, terms, (DateTime) apdoc.DueDate);
			foreach (TermsInstallments inst in installments)
			{
				docgraph.vendor.Current = vendor;
				PXCache sender = APInvoice_DocType_RefNbr.Cache;
				//force precision population
				object CuryOrigDocAmt = sender.GetValueExt(apdoc, "CuryOrigDocAmt");

				CurrencyInfo new_info = PXCache<CurrencyInfo>.CreateCopy(info);
				new_info.CuryInfoID = null;
				new_info = docgraph.currencyinfo.Insert(new_info);

				APInvoice new_apdoc = PXCache<APInvoice>.CreateCopy(apdoc);
				new_apdoc.CuryInfoID = new_info.CuryInfoID;
				new_apdoc.DueDate = ((DateTime)new_apdoc.DueDate).AddDays((double)inst.InstDays);
				new_apdoc.DiscDate = new_apdoc.DueDate;
				new_apdoc.InstallmentNbr = inst.InstallmentNbr;
				new_apdoc.MasterRefNbr = new_apdoc.RefNbr;
				new_apdoc.RefNbr = null;
				new_apdoc.PayDate = null;
				TaxAttribute.SetTaxCalc<APTran.taxCategoryID>(docgraph.Transactions.Cache, null, TaxCalc.NoCalc);

				if (inst.InstallmentNbr == installments.Count)
				{
					new_apdoc.CuryOrigDocAmt = new_apdoc.CuryOrigDocAmt - CuryTotalInstallments;
				}
				else
				{
					if (terms.InstallmentMthd == TermsInstallmentMethod.AllTaxInFirst)
					{
						new_apdoc.CuryOrigDocAmt = PXDBCurrencyAttribute.Round(sender, apdoc, (decimal)((apdoc.CuryOrigDocAmt - apdoc.CuryTaxTotal) * inst.InstPercent / 100m), CMPrecision.TRANCURY);
						if (inst.InstallmentNbr == 1)
						{
							new_apdoc.CuryOrigDocAmt += (decimal)apdoc.CuryTaxTotal;
						}
					}
					else
					{
						new_apdoc.CuryOrigDocAmt = PXDBCurrencyAttribute.Round(sender, apdoc, (decimal)(apdoc.CuryOrigDocAmt * inst.InstPercent / 100m), CMPrecision.TRANCURY);
					}
				}
				new_apdoc.CuryDocBal = new_apdoc.CuryOrigDocAmt;
				new_apdoc.CuryLineTotal = new_apdoc.CuryOrigDocAmt;
				new_apdoc.CuryTaxTotal = 0m;
				new_apdoc.CuryOrigDiscAmt = 0m;
				new_apdoc.CuryVatTaxableTotal = 0m;
				new_apdoc = docgraph.Document.Insert(new_apdoc);
				CuryTotalInstallments += (decimal)new_apdoc.CuryOrigDocAmt;
				TaxAttribute.SetTaxCalc<APTran.taxCategoryID>(docgraph.Transactions.Cache, null, TaxCalc.NoCalc);

				APTran new_aptran = new APTran();
				new_aptran.AccountID = new_apdoc.APAccountID;
				new_aptran.SubID = new_apdoc.APSubID;
				new_aptran.CuryTranAmt = new_apdoc.CuryOrigDocAmt;
				using (new PXLocaleScope(vendor.LocaleName))
				{
					new_aptran.TranDesc =PXMessages.LocalizeNoPrefix(Messages.MultiplyInstallmentsTranDesc);
				}

				docgraph.Transactions.Insert(new_aptran);

				docgraph.Save.Press();

				ret.Add((APRegister)docgraph.Document.Current);

				docgraph.Clear();
			}


			if (installments.Count > 0)
			{
				docgraph.Document.Search<APInvoice.refNbr>(apdoc.RefNbr, apdoc.DocType);
				docgraph.Document.Current.InstallmentCntr = Convert.ToInt16(installments.Count);
				docgraph.Document.Cache.SetStatus(docgraph.Document.Current, PXEntryStatus.Updated);

				docgraph.Save.Press();
				docgraph.Clear();
			}

			return ret;
		}

        public static decimal? RoundAmount(decimal? amount, string RoundType, decimal? precision)
        {
            decimal? toround = amount / precision;

            switch (RoundType)
            {
                case "F":
                    return Math.Floor((decimal)toround) * precision;
                case "C":
                    return Math.Ceiling((decimal)toround) * precision;
                case "R":
                    return Math.Round((decimal)toround, 0, MidpointRounding.AwayFromZero) * precision;
                default:
                    return amount;
            }
        }

        public virtual decimal? RoundAmount(decimal? amount)
        {
            return RoundAmount(amount, this.InvoiceRounding, this.InvoicePrecision);
        }

		public virtual void VerifyStockItemLineHasReceipt(APRegister doc)
		{
			APTran tran = PXSelectJoin<APTran, InnerJoin<InventoryItem, On<APTran.inventoryID, Equal<InventoryItem.inventoryID>>>
									, Where<APTran.refNbr, Equal<Required<APInvoice.refNbr>>, And<APTran.tranType, Equal<Required<APInvoice.docType>>, And<APTran.receiptNbr, IsNull, And<InventoryItem.stkItem, Equal<True>, And<APTran.tranType, NotEqual<APDocType.prepayment>>>>>>
									>.SelectSingleBound(this, new object[] { }, doc.RefNbr, doc.DocType);
			if (tran != null)
				throw new PXException(Messages.HasNoLinkedtoReceipt);
		}

		public virtual List<APRegister> ReleaseDocProc(JournalEntry je, ref APRegister doc, PXResult<APInvoice, CurrencyInfo, Terms, Vendor> res, bool isPrebooking)
		{
			List<INRegister> inDocs;
			return ReleaseDocProc(je, ref doc, res, isPrebooking, out inDocs);
		}

		public virtual List<APRegister> ReleaseDocProc(JournalEntry je, ref APRegister doc, PXResult<APInvoice, CurrencyInfo, Terms, Vendor> res, bool isPrebooking, out List<INRegister> inDocs)
		{
			APInvoice apdoc = res;
			CurrencyInfo info = res;
			Terms terms = res;
			Vendor vend = res;

			List<APRegister> ret = null;
			inDocs = new List<INRegister>();

			if (doc.Released != true && (!isPrebooking || doc.Prebooked != true))
			{
				string _InstallmentType = terms.InstallmentType;
				if (_IsIntegrityCheck && apdoc.InstallmentNbr == null)
				{
					_InstallmentType = apdoc.InstallmentCntr != null ? TermsInstallmentType.Multiple : TermsInstallmentType.Single;
				}

				bool isPrebookCompletion = (doc.Prebooked == true);
				bool isPrebookVoiding = doc.DocType == APDocType.VoidQuickCheck && !string.IsNullOrEmpty(apdoc.PrebookBatchNbr);
				if (isPrebookCompletion && string.IsNullOrEmpty(apdoc.PrebookBatchNbr)) 
				{
					throw new PXException(Messages.LinkToThePrebookingBatchIsMissing, doc.DocType, doc.RefNbr);
				} 

				if (_InstallmentType == TermsInstallmentType.Multiple && isPrebooking) 
				{
					throw new PXException(Messages.InvoicesWithMultipleInstallmentTermsMayNotBePrebooked);
				}

				if (_InstallmentType == TermsInstallmentType.Multiple && (apdoc.DocType == APDocType.QuickCheck || apdoc.DocType == APDocType.VoidQuickCheck))
				{
					throw new PXException(Messages.Quick_Check_Cannot_Have_Multiply_Installments);
				}

				if (_InstallmentType == TermsInstallmentType.Multiple && apdoc.InstallmentNbr == null)
				{
					if (_IsIntegrityCheck == false)
					{
						ret = CreateInstallments(res);
					}
					doc.CuryDocBal = 0m;
					doc.DocBal = 0m;
					doc.CuryDiscBal = 0m;
					doc.DiscBal = 0m;
					doc.CuryDiscTaken = 0m;
					doc.DiscTaken = 0m;
					doc.CuryWhTaxBal = 0m;
					doc.WhTaxBal = 0m;
					doc.CuryTaxWheld = 0m;
					doc.TaxWheld = 0m;

					doc.OpenDoc = false;
					doc.ClosedFinPeriodID = doc.FinPeriodID;
					doc.ClosedTranPeriodID = doc.TranPeriodID;
				}
				else
				{
					if (isPrebookCompletion == false)
					{
						doc.CuryDocBal = doc.CuryOrigDocAmt;
						doc.DocBal = doc.OrigDocAmt;
						doc.CuryDiscBal = doc.CuryOrigDiscAmt;
						doc.DiscBal = doc.OrigDiscAmt;
						doc.CuryWhTaxBal = doc.CuryOrigWhTaxAmt;
						doc.WhTaxBal = doc.OrigWhTaxAmt;
						doc.CuryDiscTaken = 0m;
						doc.DiscTaken = 0m;
						doc.CuryTaxWheld = 0m;
						doc.TaxWheld = 0m;
						doc.RGOLAmt = 0m;

						doc.OpenDoc = true;
						doc.ClosedFinPeriodID = null;
						doc.ClosedTranPeriodID = null;
					}
				}
				
				//should always restore APRegister to APInvoice after above assignments
				PXCache<APRegister>.RestoreCopy(apdoc, doc);

				CurrencyInfo new_info = PXCache<CurrencyInfo>.CreateCopy(info);
				new_info.CuryInfoID = null;
				new_info.ModuleCode = "GL";
				new_info = je.currencyinfo.Insert(new_info) ?? new_info;

				if (isPrebookCompletion == false)
				{
					if (apdoc.DocType == APDocType.QuickCheck || apdoc.DocType == APDocType.VoidQuickCheck)
					{
						GLTran tran = new GLTran();
						tran.SummPost = true;
						tran.ZeroPost = false;
						tran.BranchID = apdoc.BranchID;

						tran.AccountID = apdoc.APAccountID;
						tran.SubID = apdoc.APSubID;
						tran.ReclassificationProhibited = true;
						tran.CuryDebitAmt = (apdoc.DrCr == DrCr.Debit) ? 0m : apdoc.CuryOrigDocAmt + apdoc.CuryOrigDiscAmt + apdoc.CuryOrigWhTaxAmt;
						tran.DebitAmt = (apdoc.DrCr == DrCr.Debit) ? 0m : apdoc.OrigDocAmt + apdoc.OrigDiscAmt + apdoc.OrigWhTaxAmt;
						tran.CuryCreditAmt = (apdoc.DrCr == DrCr.Debit) ? apdoc.CuryOrigDocAmt + apdoc.CuryOrigDiscAmt + apdoc.CuryOrigWhTaxAmt : 0m;
						tran.CreditAmt = (apdoc.DrCr == DrCr.Debit) ? apdoc.OrigDocAmt + apdoc.OrigDiscAmt + apdoc.OrigWhTaxAmt : 0m;

						tran.TranType = apdoc.DocType;
						tran.TranClass = apdoc.DocClass;
						tran.RefNbr = apdoc.RefNbr;
						tran.TranDesc = apdoc.DocDesc;
						tran.TranPeriodID = apdoc.TranPeriodID;
						tran.FinPeriodID = apdoc.FinPeriodID;
						tran.TranDate = apdoc.DocDate;
						tran.CuryInfoID = new_info.CuryInfoID;
						tran.Released = true;
						tran.ReferenceID = apdoc.VendorID;
                        
						//no history update should take place
						je.GLTranModuleBatNbr.Insert(tran);
					}
					else if (apdoc.DocType != APDocType.Prepayment)
					{
						GLTran tran = new GLTran();
						tran.SummPost = true;
						tran.BranchID = apdoc.BranchID;

						tran.AccountID = apdoc.APAccountID;
						tran.SubID = apdoc.APSubID;
						tran.ReclassificationProhibited = true;
						tran.CuryDebitAmt = (apdoc.DrCr == DrCr.Debit) ? 0m : apdoc.CuryOrigDocAmt;
						tran.DebitAmt = (apdoc.DrCr == DrCr.Debit) ? 0m : apdoc.OrigDocAmt - apdoc.RGOLAmt;
						tran.CuryCreditAmt = (apdoc.DrCr == DrCr.Debit) ? apdoc.CuryOrigDocAmt : 0m;
						tran.CreditAmt = (apdoc.DrCr == DrCr.Debit) ? apdoc.OrigDocAmt - apdoc.RGOLAmt : 0m;

						tran.TranType = apdoc.DocType;
						tran.TranClass = apdoc.DocClass;
						tran.RefNbr = apdoc.RefNbr;
						tran.TranDesc = apdoc.DocDesc;
						tran.TranPeriodID = apdoc.TranPeriodID;
						tran.FinPeriodID = apdoc.FinPeriodID;
						tran.TranDate = apdoc.DocDate;
						tran.CuryInfoID = new_info.CuryInfoID;
						tran.Released = true;
						tran.ReferenceID = apdoc.VendorID;
                        tran.ProjectID = PM.ProjectDefaultAttribute.NonProject(this);
						tran.NonBillable = true;
						//if (apdoc.InstallmentNbr == null || apdoc.InstallmentNbr == 0)
						if ((bool)doc.OpenDoc)
						{
							UpdateHistory(tran, vend);
							UpdateHistory(tran, vend, info);
						}
						je.GLTranModuleBatNbr.Insert(tran);
					}
				}


				if (apdoc.DocType != APDocType.Prepayment)
				{
					GLTran summaryTran = null;
					if (isPrebooking || isPrebookCompletion || isPrebookVoiding)
					{
						summaryTran = new GLTran();
						summaryTran.SummPost = true;
						summaryTran.ZeroPost = false;
						summaryTran.CuryCreditAmt = summaryTran.CuryDebitAmt = summaryTran.CreditAmt = summaryTran.DebitAmt = Decimal.Zero;
						summaryTran.BranchID = apdoc.BranchID;
						summaryTran.AccountID = apdoc.PrebookAcctID;
						summaryTran.SubID = apdoc.PrebookSubID;
						summaryTran.ReclassificationProhibited = true;
						summaryTran.TranType = apdoc.DocType;
						summaryTran.TranClass = apdoc.DocClass;
						summaryTran.RefNbr = apdoc.RefNbr;
                        summaryTran.TranDesc = isPrebookCompletion ? PXMessages.LocalizeFormatNoPrefix(Messages.PreliminaryAPExpenceBookingAdjustment) : PXMessages.LocalizeFormatNoPrefix(Messages.PreliminaryAPExpenceBooking);
						summaryTran.TranPeriodID = apdoc.TranPeriodID;
						summaryTran.FinPeriodID = apdoc.FinPeriodID;
						summaryTran.TranDate = apdoc.DocDate;
						summaryTran.CuryInfoID = new_info.CuryInfoID;
						summaryTran.Released = true;
						summaryTran.ReferenceID = apdoc.VendorID;						
					}
					bool updateDeferred = !isPrebooking;
					bool updatePOReceipt = !isPrebooking;
					bool updatePOOrder = !isPrebooking;
					bool updateVendorPrice = !isPrebooking;

					APTran prev_n = new APTran();
					Dictionary<string, string> orderCheckClosed1 = new Dictionary<string, string>();
					List<APTran> pPVLines = new List<APTran>();
					List<LandedCostHelper.POReceiptLineAdjustment> pPVTransactions = new List<LandedCostHelper.POReceiptLineAdjustment>();
                    Dictionary<KeyValuePair<string, string>, bool> orderCheckClosed = new Dictionary<KeyValuePair<string, string>, bool>();
                    PXResultset<APTran> apTranTaxes = APTran_TranType_RefNbr.Select((object) apdoc.DocType, apdoc.RefNbr);
					//sorting on joined tables' fields does not work!
					apTranTaxes.Sort((PXResult<APTran> x, PXResult<APTran> y) =>
					{
						APTran tranX = (APTran) x;
						APTran tranY = (APTran) y;
						Tax taxX = x.GetItem<Tax>();
						Tax taxY = y.GetItem<Tax>();
						if (tranX.LineNbr == tranY.LineNbr)
						{
							if (taxX.TaxCalcLevel == taxY.TaxCalcLevel)
							{
								if (taxX.TaxType == taxY.TaxType)
								{
									return 0;
								}
								else
								{
									return String.Compare(taxX.TaxType, taxY.TaxType);
								}
							}
							else
							{
								return Int32.Parse(taxX.TaxCalcLevel) - Int32.Parse(taxY.TaxCalcLevel);
							}
						}
						else
						{
							return tranX.LineNbr.Value - tranY.LineNbr.Value;
						}

					});
					foreach (PXResult<APTran, APTax, Tax, DRDeferredCode, LandedCostTran, LandedCostCode, InventoryItem> r in apTranTaxes)
					{
						APTran n = (APTran)r;
						APTax x = (APTax)r;
						Tax salestax = (Tax)r;
						DRDeferredCode defcode = (DRDeferredCode)r;
						LandedCostTran lcTran = (LandedCostTran)r;
					    InventoryItem inventoryItem = (InventoryItem) r;

						if (!object.Equals(prev_n, n) && _IsIntegrityCheck == false && n.Released == true)
						{
							throw new PXException(Messages.Document_Status_Invalid);
						}

						if (object.Equals(prev_n, n) == false)
						{
							n.TranDate = apdoc.DocDate;
							n.FinPeriodID = apdoc.FinPeriodID;

							GLTran tran = new GLTran();
							GLTran corrTran1 = null;
							GLTran corrTran2 = null;
							tran.SummPost = this.SummPost;
							tran.BranchID = n.BranchID;
							tran.CuryInfoID = new_info.CuryInfoID;
							tran.TranType = n.TranType;
							tran.TranClass = apdoc.DocClass;
							tran.InventoryID = n.InventoryID;
							tran.UOM = n.UOM;
							tran.Qty = (n.DrCr == DrCr.Debit) ? n.Qty : -1 * n.Qty;
							tran.RefNbr = n.RefNbr;
							tran.TranDate = n.TranDate;
							tran.ProjectID = n.ProjectID;
							tran.TaskID = n.TaskID;
							tran.AccountID = n.AccountID;
							tran.SubID = n.SubID;
							tran.TranDesc = n.TranDesc;
							tran.Released = true;
							tran.ReferenceID = apdoc.VendorID;
							tran.TranLineNbr = (tran.SummPost == true) ? null : n.LineNbr;
						    tran.NonBillable = n.NonBillable;

							if (x != null && x.TaxID != null && salestax != null && salestax.TaxCalcType == CSTaxCalcType.Item && PXAccess.FeatureInstalled<FeaturesSet.netGrossEntryMode>())
							{
								string TaxCalcMode = apdoc.TaxCalcMode;
								switch (TaxCalcMode)
								{
									case VendorClass.taxCalcMode.Gross:
										salestax.TaxCalcLevel = CSTaxCalcLevel.Inclusive;
										break;
									case VendorClass.taxCalcMode.Net:
										salestax.TaxCalcLevel = CSTaxCalcLevel.CalcOnItemAmt;
										break;
									case VendorClass.taxCalcMode.TaxSetting:
										break;
								}
							}

							var postedAmount = GetExpensePostingAmount(n, x, salestax,
								amount => PXDBCurrencyAttribute.Round(je.GLTranModuleBatNbr.Cache, tran, amount, CMPrecision.TRANCURY));

							tran.CuryDebitAmt = (n.DrCr == DrCr.Debit) ? postedAmount.Cury : 0m;
							tran.DebitAmt = (n.DrCr == DrCr.Debit) ? postedAmount.Base : 0m;
							tran.CuryCreditAmt = (n.DrCr == DrCr.Debit) ? 0m : postedAmount.Cury;
							tran.CreditAmt = (n.DrCr == DrCr.Debit) ? 0m : postedAmount.Base;

							if (lcTran.LCTranID != null)
							{
								if (n.TranAmt != lcTran.LCAmount)
								{
									decimal LCdelta = (lcTran.AmountSign.Value * lcTran.LCAmount.Value) - n.TranAmt.Value; //For Debit Adjustment LCAmount and AmountSign are negative
									decimal curyLCdelta;
									LandedCostCode lcCode = (LandedCostCode)r;
									PXCurrencyAttribute.CuryConvCury(je.currencyinfo.Cache, new_info, LCdelta, out curyLCdelta);
									if (LCdelta != Decimal.Zero || curyLCdelta != decimal.Zero)
									{
										corrTran1 = (GLTran)je.GLTranModuleBatNbr.Cache.CreateCopy(tran);
										corrTran2 = (GLTran)je.GLTranModuleBatNbr.Cache.CreateCopy(tran);
										corrTran1.TranDesc = Messages.LandedCostAccrualCorrection;

										corrTran1.CuryDebitAmt = (n.DrCr == DrCr.Debit) ? curyLCdelta : 0m;
										corrTran1.DebitAmt = (n.DrCr == DrCr.Debit) ? LCdelta : 0m;
										corrTran1.CuryCreditAmt = (n.DrCr == DrCr.Debit) ? 0m : curyLCdelta;
										corrTran1.CreditAmt = (n.DrCr == DrCr.Debit) ? 0m : LCdelta;

										corrTran2.TranDesc = Messages.LandedCostVariance;
										corrTran2.CuryDebitAmt = (n.DrCr == DrCr.Debit) ? 0m : curyLCdelta;
										corrTran2.DebitAmt = (n.DrCr == DrCr.Debit) ? 0m : LCdelta;
										corrTran2.CuryCreditAmt = (n.DrCr == DrCr.Debit) ? curyLCdelta : 0m;
										corrTran2.CreditAmt = (n.DrCr == DrCr.Debit) ? LCdelta : 0m;
										corrTran2.AccountID = lcCode.LCVarianceAcct;
										corrTran2.SubID = lcCode.LCVarianceSub;
									}
								}
							}

							if (_IsIntegrityCheck == false && !string.IsNullOrEmpty(n.ReceiptNbr) && n.ReceiptLineNbr != null && updatePOReceipt)
							{
								PO.POReceiptLineR1 rctLine = poReceiptLineUPD.Select(n.ReceiptNbr, n.ReceiptLineNbr);
								if (rctLine != null)
								{
									PO.POReceiptLineR1 rctLine1 = (PO.POReceiptLineR1)this.Caches[typeof(PO.POReceiptLineR1)].CreateCopy(rctLine);

									decimal sign = (n.DrCr == DrCr.Debit) ? Decimal.One : Decimal.MinusOne; //This is needed for the correct application of the APDebitAdjustment to POReceipt
									sign *= rctLine1.APSign; //This is needed for the correct application of the APDebitAdjustment to POReturn 
									decimal effQty = n.Qty.Value * sign;
									decimal effAmt = n.TranAmt.Value * sign;
									decimal effCuryAmt = n.CuryTranAmt.Value * sign;
                                    decimal effCuryDisc = n.CuryDiscAmt.Value * sign;

									rctLine1.UnbilledQty -= effQty;
									//Need to recalc price - CuryExtPrice may be different from the multiplication of Qty * CuryUnitPrice
									if (rctLine1.UnbilledQty == Decimal.Zero)
									{
										rctLine1.CuryUnbilledAmt = Decimal.Zero;
                                        rctLine1.CuryUnbilledDiscountAmt = Decimal.Zero;
									}
									else
									{
										if (rctLine.ReceiptQty != decimal.Zero)
										{
											decimal billCuryAmt = (effQty * rctLine.CuryExtCost.Value / rctLine.Qty.Value); //Count proportion of the original CuryExtCost for this line
											rctLine1.CuryUnbilledAmt -= PXCurrencyAttribute.Round(this.Caches[typeof(PO.POReceiptLineR1)], rctLine1, billCuryAmt, CMPrecision.TRANCURY);
                                            
										}
										else
										{
											rctLine1.CuryUnbilledAmt -= effCuryAmt;
										}
                                        rctLine1.CuryUnbilledDiscountAmt -= effCuryDisc;
									}

									if (IsPPVCalcNeeded(rctLine, n))
									{
										decimal unbilledAmt1 = Decimal.Zero;
                                        decimal billedEffDisc = Decimal.Zero;
										PXCurrencyAttribute.CuryConvBase(this.Caches[typeof(PO.POReceiptLineR1)], rctLine1, (decimal)rctLine1.CuryUnbilledAmt, out unbilledAmt1);
                                        PXCurrencyAttribute.CuryConvBase(this.Caches[typeof(PO.POReceiptLineR1)], rctLine1, (decimal)effCuryDisc, out billedEffDisc);
										decimal billedLineAmt = (decimal)rctLine.UnbilledAmt - unbilledAmt1;
                                        decimal amount = (effQty == rctLine.UnbilledQty) ? (decimal)(rctLine.UnbilledAmt - rctLine.UnbilledDiscountAmt - effAmt) : (decimal)(billedLineAmt - billedEffDisc - effAmt);
										decimal curyAmt;
										PXCurrencyAttribute.CuryConvCury(je.currencyinfo.Cache, new_info, amount, out curyAmt);

										bool isInventoryPPV = false;
										if (posetup.PPVAllocationMode == PPVMode.Inventory)
										{
											InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, rctLine.InventoryID);
											isInventoryPPV = (item == null ? false : item.ValMethod != INValMethod.Standard) && !PO.POLineType.IsNonStock(rctLine.LineType);
										}

										if (rctLine.ReceiptType == POReceiptType.POReceipt && isInventoryPPV)
										{
											bool isReverseTran = apdoc.OrigDocType != null && apdoc.OrigRefNbr != null && apdoc.OrigDocType != apdoc.DocType; 
											//Keep BillPPVAmt on POReceiptLine to calculate difference later (reverse only)
											if (isReverseTran && sign != Decimal.One)
											{
												rctLine1.ReversedBillPPVAmt += amount;
												amount = 0m;
											}
											if (!isReverseTran && rctLine1.ReversedBillPPVAmt != 0m)
											{
												n.POPPVAmt = rctLine1.ReversedBillPPVAmt + amount;
												rctLine1.BillPPVAmt += (rctLine1.ReversedBillPPVAmt + amount);
												rctLine1.ReversedBillPPVAmt = 0m;
											}
											else
											{
												n.POPPVAmt = amount;
												rctLine1.BillPPVAmt += amount;
											}
										}
										else
										{
											n.POPPVAmt = amount;
											rctLine1.BillPPVAmt += amount;
										}

										if ((amount != Decimal.Zero || curyAmt != decimal.Zero) && !isInventoryPPV)
										{
											GLTran poAccrualTran = new GLTran();
											GLTran ppVTran = new GLTran();

											poAccrualTran = (GLTran)je.GLTranModuleBatNbr.Cache.CreateCopy(tran);
											ppVTran = (GLTran)je.GLTranModuleBatNbr.Cache.CreateCopy(tran);
											int? accountID = null;
											int? subID = null;
											if (posetup.PPVAllocationMode == PPVMode.Inventory && PO.POLineType.IsNonStock(rctLine.LineType))
											{
												POReceiptLine poreceiptline = PXSelect<POReceiptLine, Where<POReceiptLine.receiptType, Equal<Required<POReceiptLine.receiptType>>,
													And<POReceiptLine.receiptNbr, Equal<Required<POReceiptLine.receiptNbr>>,
													And<POReceiptLine.lineNbr, Equal<Required<POReceiptLine.lineNbr>>>>>>.Select(this, n.ReceiptType, n.ReceiptNbr, n.ReceiptLineNbr);
												if (poreceiptline == null) throw new PXException(Messages.CannotFindPOReceipt, n.ReceiptNbr);
												accountID = poreceiptline.ExpenseAcctID;
												subID = poreceiptline.ExpenseSubID;
											}
											else
												RetrievePPVAccount(je, rctLine, ref accountID, ref subID);

											ppVTran.AccountID = accountID;
											ppVTran.SubID = subID;
											//Type of transaction is aleady counted in the sign
											poAccrualTran.CuryDebitAmt = curyAmt * (rctLine.ReceiptType == POReceiptType.POReturn ? -1 : 1);
											poAccrualTran.CuryCreditAmt = Decimal.Zero;
											poAccrualTran.DebitAmt = amount * (rctLine.ReceiptType == POReceiptType.POReturn ? -1 : 1);
											poAccrualTran.CreditAmt = Decimal.Zero;

											ppVTran.CuryDebitAmt = Decimal.Zero;
											ppVTran.CuryCreditAmt = curyAmt * (rctLine.ReceiptType == POReceiptType.POReturn ? -1 : 1);
											ppVTran.DebitAmt = Decimal.Zero;
											ppVTran.CreditAmt = amount * (rctLine.ReceiptType == POReceiptType.POReturn ? -1 : 1);

											poAccrualTran = je.GLTranModuleBatNbr.Insert(poAccrualTran);
											ppVTran = je.GLTranModuleBatNbr.Insert(ppVTran);
										}
										else if (n.POPPVAmt != null && n.POPPVAmt != 0m && posetup.PPVAllocationMode == PPVMode.Inventory)
										{
											POReceiptLine poreceiptline = PXSelect<POReceiptLine, Where<POReceiptLine.receiptType, Equal<Required<POReceiptLine.receiptType>>,
												And<POReceiptLine.receiptNbr, Equal<Required<POReceiptLine.receiptNbr>>,
												And<POReceiptLine.lineNbr, Equal<Required<POReceiptLine.lineNbr>>>>>>.Select(this, n.ReceiptType, n.ReceiptNbr, n.ReceiptLineNbr);
											if (poreceiptline == null) throw new PXException(Messages.CannotFindPOReceipt, n.ReceiptNbr);

                                            var totalToDistribute = (decimal)-n.POPPVAmt;

                                            var lch = new LandedCostHelper(this, true);
                                            totalToDistribute -= lch.AllocateOverRCTLine(pPVTransactions, poreceiptline, totalToDistribute, n.BranchID);
                                            lch.AllocateRestOverRCTLines(pPVTransactions, totalToDistribute);

											pPVLines.Add(n);
										}
									}
									rctLine = UpdatePOReceiptLine(rctLine1, n);

									if (((n.PONbr==null&&n.ReceiptNbr==null)||posetup.VendorPriceUpdate == POVendorPriceUpdateType.ReleaseAPBill) &&
										n.InventoryID != null && n.CuryUnitCost != null)
									{
										POItemCostManager.Update(this,
													doc.VendorID,
													doc.VendorLocationID,
													doc.CuryID,
													n.InventoryID,
													rctLine.SubItemID,
													n.UOM,
													n.CuryUnitCost.Value);
									}
								}
							}

							if (_IsIntegrityCheck == false && (!PXAccess.FeatureInstalled<FeaturesSet.distributionModule>()
                                    || ((posetup != null && posetup.VendorPriceUpdate == POVendorPriceUpdateType.ReleaseAPBill) || (n.PONbr == null && n.ReceiptNbr == null))) &&
								n.InventoryID != null && n.CuryUnitCost != null && updateVendorPrice)
							{
								POItemCostManager.Update(this,
											doc.VendorID,
											doc.VendorLocationID,
											doc.CuryID,
											n.InventoryID,
											null,
											n.UOM,
											n.CuryUnitCost.Value);
							}

							if (_IsIntegrityCheck == false && !string.IsNullOrEmpty(n.PONbr) 
								&& !string.IsNullOrEmpty(n.POOrderType) && n.POLineNbr != null && String.IsNullOrEmpty(n.ReceiptNbr) 
								&& updatePOOrder)
							{
								PXResult<PO.POLineUOpen, PO.POLine> poRes = (PXResult<PO.POLineUOpen, PO.POLine>)this.poOrderLineUPD.Select(n.POOrderType, n.PONbr, n.POLineNbr);
								if (poRes != null)
								{
									PO.POLine srcLine = (PO.POLine)poRes;
									PO.POLineUOpen orgLine = (PO.POLineUOpen)poRes;
									PO.POLineUOpen updLine = (PO.POLineUOpen)this.poOrderLineUPD.Cache.CreateCopy(orgLine);
									decimal sign = (n.DrCr == DrCr.Debit) ? Decimal.One : Decimal.MinusOne;

									updLine.OpenQty -= sign * n.Qty;
									if (updLine.OpenQty < 0)
										updLine.OpenQty = 0;

									if ((updLine.CuryUnitCost ?? Decimal.Zero) != Decimal.Zero)
									{
										updLine.CuryOpenAmt -= sign * n.Qty * updLine.CuryUnitCost;
									}
									else
									{
										updLine.CuryOpenAmt -= sign * n.CuryTranAmt;
									}
									if (updLine.CuryOpenAmt < 0)
										updLine.CuryOpenAmt = 0;

									bool doClose = false;
									if (inventoryItem.CompletePOLine == CompletePOLineTypes.Quantity)
									{
										doClose = ((srcLine.OrderQty * srcLine.RcptQtyThreshold) / 100.0m) <= srcLine.VoucheredQty;
									}
									else
									{
										doClose = ((srcLine.CuryExtCost * srcLine.RcptQtyThreshold) / 100.0m) <= srcLine.CuryVoucheredCost;
									}

									if (doClose || (!doClose && updLine.Completed == true))
									{
										KeyValuePair<string, string> orderKey = new KeyValuePair<string, string>(updLine.OrderType, updLine.OrderNbr);
										updLine.Completed = doClose;
										if (updLine.Completed == true)
										{
											updLine.OpenQty = 0;
											updLine.CuryOpenAmt = 0;
										}
										orderCheckClosed[orderKey] = doClose;
									}

									updLine = this.poOrderLineUPD.Update(updLine);
								}
							}

							IEnumerable<GLTran> transactions = null;
							if (_IsIntegrityCheck == false && defcode != null && defcode.DeferredCodeID != null && updateDeferred)
							{
								DRProcess dr = PXGraph.CreateInstance<DRProcess>();
								dr.CreateSchedule(n, defcode, apdoc, postedAmount.Base.Value, isDraft: false);
								dr.Actions.PressSave();

								transactions = je.CreateTransBySchedule(dr, tran);
								je.CorrectCuryAmountsDueToRounding(transactions, tran, postedAmount.Cury.Value);
							}

							if (transactions == null || transactions.Any() == false)
							{
								transactions = new GLTran[] { tran };
							}

							if (isPrebooking == true || isPrebookVoiding == true)
							{
								foreach(var item in transactions)
							{
									Append(summaryTran, item);
								}
							}
							else
							{
								foreach(var item in transactions)
								{
									je.GLTranModuleBatNbr.Insert(item);
								if (isPrebookCompletion)
								{
										Append(summaryTran, item);
									}
								}

								if (corrTran1 != null)
									je.GLTranModuleBatNbr.Insert(corrTran1);
								if (corrTran2 != null)
									je.GLTranModuleBatNbr.Insert(corrTran2);
								n.Released = true;
							}
							APTran_TranType_RefNbr.Update(n);
						}
						prev_n = n;
					}

					//create PPV adjustments
					if (pPVTransactions.Count() != 0)
					{
						INRegister pPVAdjustment = CreatePPVAdjustment(apdoc, pPVTransactions, pPVLines);
						inDocs.Add(pPVAdjustment);
					}


					if (orderCheckClosed.Count > 0)
					{
						foreach (KeyValuePair<KeyValuePair<string, string>, bool> orderNbr in orderCheckClosed)
						{
							POLineUOpen line =
								PXSelect<POLineUOpen,
									Where<POLineUOpen.orderType, Equal<Required<POLineUOpen.orderType>>,
									And<POLineUOpen.orderNbr, Equal<Required<POLineUOpen.orderNbr>>,
									And<POLineUOpen.lineType, NotEqual<POLineType.description>,
									And<POLineUOpen.completed, Equal<False>>>>>>.Select(this, orderNbr.Key.Key, orderNbr.Key.Value);

							//close order
							if (line == null && orderNbr.Value == true)
							{
							POLineUOpen cancelled =
								PXSelect<POLineUOpen,
								Where<POLineUOpen.orderType, Equal<Required<POLineUOpen.orderType>>,
									And<POLineUOpen.orderNbr, Equal<Required<POLineUOpen.orderNbr>>,
									And<POLineUOpen.lineType, NotEqual<POLineType.description>,
									And<POLineUOpen.cancelled, Equal<True>>>>>>.Select(this, orderNbr.Key.Key, orderNbr.Key.Value);
								PO.POOrder order = this.poOrderUPD.Select(orderNbr.Key.Key, orderNbr.Key.Value);
								if (order != null && order.Status != POOrderStatus.Closed && order.Hold != true)
								{
									POOrder upd = poOrderUPD.Cache.CreateCopy(order) as POOrder;
									if (upd != null)
									{
										upd.Receipt = cancelled == null;
										upd.Status = POOrderStatus.Closed;
										poOrderUPD.Update(upd);
									}
								}
							}
							//re-open closed order
							else if (orderNbr.Value != true)
							{
								PO.POOrder order = this.poOrderUPD.Select(orderNbr.Key.Key, orderNbr.Key.Value);
								if (order != null && order.Status == POOrderStatus.Closed && order.Hold != true)
								{
									POOrder upd = poOrderUPD.Cache.CreateCopy(order) as POOrder;
									if (upd != null)
									{
										upd.Receipt = false;
										upd.Status = POOrderStatus.Open;
										poOrderUPD.Update(upd);
						}
					}
							}
						}
					}

					if (isPrebookCompletion)
					{
						foreach (PXResult<APTaxTran, Tax, APInvoice> r in APTaxTran_TranType_RefNbr.Select(apdoc.DocType, apdoc.RefNbr))
						{
							APTaxTran x = r;
							Tax salestax = r;
							APInvoice orig_doc = r;
							if (salestax.DeductibleVAT == true && salestax.ReportExpenseToSingleAccount != true && salestax.TaxCalcType == CSTaxCalcType.Item)
							{
								IEnumerable<GLTran> newTrans = PostTaxExpenseToItemAccounts(je, apdoc, new_info, x, salestax, true);
								foreach (var item in newTrans)
								{
									Append(summaryTran, item);
								}
							}
						}
						Invert(summaryTran);
						je.GLTranModuleBatNbr.Insert(summaryTran);
					}
					else
					{
						foreach (PXResult<APTaxTran, Tax, APInvoice> r in APTaxTran_TranType_RefNbr.Select(apdoc.DocType, apdoc.RefNbr))
						{
							APTaxTran x = r;
							Tax salestax = r;
							APInvoice orig_doc = r;

							if (salestax.TaxType == CSTaxType.Withholding)
							{
								continue;
							}

							if (salestax.DirectTax == true && string.IsNullOrEmpty(x.OrigRefNbr) == false)
							{
								if (_IsIntegrityCheck)
								{
									continue;
								}

								if (orig_doc.CuryInfoID == null)
								{
									throw new PXException(ErrorMessages.ElementDoesntExist, x.OrigRefNbr);
								}

								PostDirectTax(info, x, orig_doc);
							}

							if (salestax.ReverseTax == false)
							{
								PostGeneralTax(je, apdoc, new_info, x, salestax);
							}

							if (salestax.TaxType == CSTaxType.Use || salestax.ReverseTax == true)
							{
								PostReverseTax(je, apdoc, new_info, x, salestax);
							}

							if (salestax.DeductibleVAT == true)
							{
								if (salestax.ReportExpenseToSingleAccount == true)
								{
									PostTaxExpenseToSingleAccount(je, apdoc, new_info, x, salestax);
								}
								else if (salestax.TaxCalcType == CSTaxCalcType.Item)
								{
									IEnumerable<GLTran> newTrans = PostTaxExpenseToItemAccounts(je, apdoc, new_info, x, salestax, !isPrebooking && !isPrebookVoiding);
									if (isPrebooking || isPrebookVoiding)
									{
										foreach (var item in newTrans)
										{
											Append(summaryTran, item);
										}
									}
								}
							}

							x.Released = true;
							x = APTaxTran_TranType_RefNbr.Update(x);

							if (PXAccess.FeatureInstalled<FeaturesSet.vATReporting>() && _IsIntegrityCheck == false &&
								(x.TaxType == TX.TaxType.PendingPurchase || x.TaxType == TX.TaxType.PendingSales))
							{
								Vendor vendor = PXSelect<Vendor, Where<Vendor.bAccountID,
									Equal<Required<Vendor.bAccountID>>>>.SelectSingleBound(this, null, x.VendorID);

								decimal mult = ReportTaxProcess.GetMultByTranType(BatchModule.AP, x.TranType);
								string reversalMethod = x.TranType == APDocType.QuickCheck || x.TranType == APDocType.VoidQuickCheck
									? SVATTaxReversalMethods.OnDocuments
									: vendor?.SVATReversalMethod;

								SVATConversionHist histSVAT = new SVATConversionHist
								{
									Module = BatchModule.AP,
									AdjdBranchID = x.BranchID,
									AdjdDocType = x.TranType,
									AdjdRefNbr = x.RefNbr,
									AdjgDocType = x.TranType,
									AdjgRefNbr = x.RefNbr,
									AdjdDocDate = doc.DocDate,
									AdjdFinPeriodID = doc.FinPeriodID,

									TaxID = x.TaxID,
									TaxType = x.TaxType,
									TaxRate = x.TaxRate,
									VendorID = x.VendorID,
									ReversalMethod = reversalMethod,

									CuryInfoID = x.CuryInfoID,
									CuryTaxableAmt = x.CuryTaxableAmt * mult,
									CuryTaxAmt = x.CuryTaxAmt * mult,
									CuryUnrecognizedTaxAmt = x.CuryTaxAmt * mult
								};

								histSVAT.FillBaseAmounts(SVATConversionHistory.Cache);
								SVATConversionHistory.Cache.Insert(histSVAT);
							}
						}

						if (isPrebooking || isPrebookVoiding)
							je.GLTranModuleBatNbr.Insert(summaryTran);
					}
				}

				if (_IsIntegrityCheck == false)
				{
					foreach (PXResult<APAdjust, APPayment> appres in PXSelectJoin<APAdjust, InnerJoin<APPayment, On<APPayment.docType, Equal<APAdjust.adjgDocType>, And<APPayment.refNbr, Equal<APAdjust.adjgRefNbr>>>>, Where<APAdjust.adjdDocType, Equal<Required<APAdjust.adjdDocType>>, And<APAdjust.adjdRefNbr, Equal<Required<APAdjust.adjdRefNbr>>, And<APAdjust.released, Equal<False>, And<APPayment.released, Equal<True>>>>>>.Select(this, doc.DocType, doc.RefNbr))
					{
						APAdjust adj = (APAdjust)appres;
						APPayment payment = (APPayment)appres;

						if (((APAdjust)appres).CuryAdjdAmt > 0m)
						{
							if (_InstallmentType != TermsInstallmentType.Single)
							{
								throw new PXException(Messages.PrepaymentAppliedToMultiplyInstallments);
							}

							if (ret == null)
							{
								ret = new List<APRegister>();
							}

							//are always greater then payments period
							payment.AdjDate = adj.AdjdDocDate;
							payment.AdjFinPeriodID = adj.AdjdFinPeriodID;
							payment.AdjTranPeriodID = adj.AdjdTranPeriodID;

							ret.Add(payment);

							APPayment_DocType_RefNbr.Cache.Update(payment);

							adj.AdjAmt += adj.RGOLAmt;
							adj.RGOLAmt = -adj.RGOLAmt;
							adj.Hold = false;

							APAdjust_AdjgDocType_RefNbr_VendorID.Cache.SetStatus(adj, PXEntryStatus.Updated);
						}
					}
				}

				Batch apbatch = je.BatchModule.Current;

                if (Math.Abs(Math.Round((decimal)(apbatch.CuryDebitTotal - apbatch.CuryCreditTotal), 4)) >= 0.00005m)
                {
                    VerifyRoundingAllowed(apdoc, apbatch,je.currencyInfo.BaseCuryID);

                    GLTran tran = new GLTran();
                    tran.SummPost = true;
                    tran.BranchID = apdoc.BranchID;
                    Currency c = PXSelect<Currency, Where<Currency.curyID, Equal<Required<CurrencyInfo.curyID>>>>.Select(this, doc.CuryID);

	                if (c.RoundingGainAcctID == null || c.RoundingGainSubID == null)
	                {
						throw new PXException(Messages.NoRoundingGainLossAccSub, c.CuryID);
	                }

                    if (Math.Sign((decimal)(apbatch.CuryDebitTotal - apbatch.CuryCreditTotal)) == 1)
                    {
                        tran.AccountID = c.RoundingGainAcctID;
                        tran.SubID = GainLossSubAccountMaskAttribute.GetSubID<Currency.roundingGainSubID>(je, tran.BranchID, c);
                        tran.CuryCreditAmt = Math.Round((decimal)(apbatch.CuryDebitTotal - apbatch.CuryCreditTotal), 4);
                        tran.CuryDebitAmt = 0m;
                    }
                    else
                    {
                        tran.AccountID = c.RoundingLossAcctID;
                        tran.SubID = GainLossSubAccountMaskAttribute.GetSubID<Currency.roundingLossSubID>(je, tran.BranchID, c);
                        tran.CuryCreditAmt = 0m;
                        tran.CuryDebitAmt = Math.Round((decimal)(apbatch.CuryCreditTotal - apbatch.CuryDebitTotal), 4);
                    }
                    tran.CreditAmt = 0m;
                    tran.DebitAmt = 0m;
                    tran.TranType = doc.DocType;
                    tran.RefNbr = doc.RefNbr;
                    tran.TranClass = GLTran.tranClass.Normal;
                    tran.TranDesc = GL.Messages.RoundingDiff;
                    tran.LedgerID = apbatch.LedgerID;
                    tran.FinPeriodID = apbatch.FinPeriodID;
                    tran.TranDate = apbatch.DateEntered;
                    tran.Released = true;
                    tran.ReferenceID = apdoc.VendorID;

                    CurrencyInfo infocopy = new CurrencyInfo();
                    infocopy = je.currencyinfo.Insert(infocopy) ?? infocopy;

                    tran.CuryInfoID = infocopy.CuryInfoID;
                    je.GLTranModuleBatNbr.Insert(tran);
                }


				if (Math.Abs(Math.Round((decimal)(apbatch.DebitTotal - apbatch.CreditTotal), 4, MidpointRounding.AwayFromZero)) >= 0.00005m)
				{
					GLTran tran = new GLTran();
					tran.SummPost = true;
					tran.BranchID = apdoc.BranchID;
					Currency c = PXSelect<Currency, Where<Currency.curyID, Equal<Required<CurrencyInfo.curyID>>>>.Select(this, doc.CuryID);

					if (c.RoundingGainAcctID == null || c.RoundingGainSubID == null)
					{
						throw new PXException(Messages.NoRoundingGainLossAccSub, c.CuryID);
					}

					if (Math.Sign((decimal)(apbatch.DebitTotal - apbatch.CreditTotal)) == 1)
					{
						tran.AccountID = c.RoundingGainAcctID;
						tran.SubID = GainLossSubAccountMaskAttribute.GetSubID<Currency.roundingGainSubID>(je, tran.BranchID, c);
						tran.CreditAmt = Math.Round((decimal)(apbatch.DebitTotal - apbatch.CreditTotal), 4, MidpointRounding.AwayFromZero);
						tran.DebitAmt = 0m;
					}
					else
					{
						tran.AccountID = c.RoundingLossAcctID;
                        tran.SubID = GainLossSubAccountMaskAttribute.GetSubID<Currency.roundingLossSubID>(je, tran.BranchID, c);
                        tran.CreditAmt = 0m;
						tran.DebitAmt = Math.Round((decimal)(apbatch.CreditTotal - apbatch.DebitTotal), 4, MidpointRounding.AwayFromZero);
					}
					tran.CuryCreditAmt = 0m;
					tran.CuryDebitAmt = 0m;
					tran.TranType = doc.DocType;
					tran.RefNbr = doc.RefNbr;
					tran.TranClass = GLTran.tranClass.Normal;
					tran.TranDesc = GL.Messages.RoundingDiff;
					tran.LedgerID = apbatch.LedgerID;
					tran.FinPeriodID = apbatch.FinPeriodID;
					tran.TranDate = apbatch.DateEntered;
					tran.Released = true;
					tran.ReferenceID = apdoc.VendorID;

					CurrencyInfo infocopy = new CurrencyInfo();
					infocopy = je.currencyinfo.Insert(infocopy) ?? infocopy;

					tran.CuryInfoID = infocopy.CuryInfoID;
					je.GLTranModuleBatNbr.Insert(tran);
				}

				if (doc.CuryDocBal == 0m)
				{
					doc.DocBal = 0m;
					doc.CuryDiscBal = 0m;
					doc.DiscBal = 0m;

					doc.OpenDoc = false;
					doc.ClosedFinPeriodID = doc.FinPeriodID;
					doc.ClosedTranPeriodID = doc.TranPeriodID;
				}
			}
			return ret;
		}

		protected virtual IEnumerable<GLTran> PostTaxExpenseToItemAccounts(JournalEntry je, APInvoice apdoc, CurrencyInfo new_info, APTaxTran x, Tax salestax, bool doInsert)
		{
			PXResultset<APTax> deductibleLines = PXSelectJoin<APTax, InnerJoin<APTran,
													On<APTax.tranType, Equal<APTran.tranType>,
														And<APTax.refNbr, Equal<APTran.refNbr>, And<APTax.lineNbr, Equal<APTran.lineNbr>>>>>,
													Where<APTax.taxID, Equal<Required<APTax.taxID>>, And<APTran.tranType, Equal<Required<APTran.tranType>>,
														And<APTran.refNbr, Equal<Required<APTran.refNbr>>>>>,
													OrderBy<Desc<APTax.curyTaxAmt>>>.Select(this, salestax.TaxID, x.TranType, x.RefNbr);

			APTaxAttribute apTaxAttr = new APTaxAttribute(typeof(APRegister), typeof(APTax), typeof(APTaxTran));
			apTaxAttr.DistributeExpenseDiscrepancy(this, deductibleLines.FirstTableItems, x.CuryExpenseAmt.Value);

			List<GLTran> newTrans = new List<GLTran>();

			foreach (PXResult<APTax, APTran> item in deductibleLines)
			{
				APTax taxLine = (APTax)item;
				APTran apTran = (APTran)item;

				GLTran tran = new GLTran();
				tran.SummPost = this.SummPost;
				tran.BranchID = apTran.BranchID;
				tran.CuryInfoID = new_info.CuryInfoID;
				tran.TranType = x.TranType;
				tran.TranClass = GLTran.tranClass.Tax;
				tran.RefNbr = x.RefNbr;
				tran.TranDate = x.TranDate;
				tran.AccountID = apTran.AccountID;
				tran.SubID = apTran.SubID;
				tran.TranDesc = salestax.TaxID;
				tran.TranLineNbr = apTran.LineNbr;
				tran.CuryDebitAmt = (APInvoiceType.DrCr(x.OrigTranType != null && x.OrigTranType.Trim() != string.Empty ? x.OrigTranType : x.TranType) == DrCr.Debit) ? taxLine.CuryExpenseAmt : 0m;
				tran.DebitAmt = (APInvoiceType.DrCr(x.OrigTranType != null && x.OrigTranType.Trim() != string.Empty ? x.OrigTranType : x.TranType) == DrCr.Debit) ? taxLine.ExpenseAmt : 0m;
				tran.CuryCreditAmt = (APInvoiceType.DrCr(x.OrigTranType != null && x.OrigTranType.Trim() != string.Empty ? x.OrigTranType : x.TranType) == DrCr.Debit) ? 0m : taxLine.CuryExpenseAmt;
				tran.CreditAmt = (APInvoiceType.DrCr(x.OrigTranType != null && x.OrigTranType.Trim() != string.Empty ? x.OrigTranType : x.TranType) == DrCr.Debit) ? 0m : taxLine.ExpenseAmt;
				tran.Released = true;
				tran.ReferenceID = apdoc.VendorID;
				tran.ProjectID = apTran.ProjectID;
				tran.TaskID = apTran.TaskID;

				newTrans.Add(tran);
				if (doInsert)
				{
					je.GLTranModuleBatNbr.Insert(tran);

					//SelectJoin doesn't merge caches for joined tables
					APTran locatedTran = (APTran)this.Caches[typeof(APTran)].Locate(apTran);

					decimal? expenseAmt = taxLine.ExpenseAmt + (locatedTran != null ? locatedTran.ExpenseAmt : 0);
					decimal? curyExpenseAmt = taxLine.CuryExpenseAmt + (locatedTran != null ? locatedTran.CuryExpenseAmt : 0);

					apTran.ExpenseAmt += expenseAmt;
					apTran.CuryExpenseAmt += curyExpenseAmt;

					APTran_TranType_RefNbr.Update(apTran);
				}
			}

			return newTrans;
        }

		protected virtual void PostTaxExpenseToSingleAccount(JournalEntry je, APInvoice apdoc, CurrencyInfo new_info, APTaxTran x, Tax salestax)
		{
			GLTran tran = new GLTran();
			tran.SummPost = this.SummPost;
			tran.BranchID = x.BranchID;
			tran.CuryInfoID = new_info.CuryInfoID;
			tran.TranType = x.TranType;
			tran.TranClass = GLTran.tranClass.Tax;
			tran.RefNbr = x.RefNbr;
			tran.TranDate = x.TranDate;
			tran.AccountID = salestax.ExpenseAccountID;
			tran.SubID = salestax.ExpenseSubID;
			tran.TranDesc = salestax.TaxID;
			tran.CuryDebitAmt = (APInvoiceType.DrCr(x.OrigTranType != null && x.OrigTranType.Trim() != string.Empty ? x.OrigTranType : x.TranType) == DrCr.Debit) ? x.CuryExpenseAmt : 0m;
			tran.DebitAmt = (APInvoiceType.DrCr(x.OrigTranType != null && x.OrigTranType.Trim() != string.Empty ? x.OrigTranType : x.TranType) == DrCr.Debit) ? x.ExpenseAmt : 0m;
			tran.CuryCreditAmt = (APInvoiceType.DrCr(x.OrigTranType != null && x.OrigTranType.Trim() != string.Empty ? x.OrigTranType : x.TranType) == DrCr.Debit) ? 0m : x.CuryExpenseAmt;
			tran.CreditAmt = (APInvoiceType.DrCr(x.OrigTranType != null && x.OrigTranType.Trim() != string.Empty ? x.OrigTranType : x.TranType) == DrCr.Debit) ? 0m : x.ExpenseAmt;
			tran.Released = true;
			tran.ReferenceID = apdoc.VendorID;
			tran.ProjectID = ProjectDefaultAttribute.NonProject(this);

			je.GLTranModuleBatNbr.Insert(tran);
		}

		protected virtual void PostReverseTax(JournalEntry je, APInvoice apdoc, CurrencyInfo new_info, APTaxTran x, Tax salestax)
		{
			GLTran tran = new GLTran();
			tran.SummPost = this.SummPost;
			tran.BranchID = x.BranchID;
			tran.CuryInfoID = new_info.CuryInfoID;
			tran.TranType = x.TranType;
			tran.TranClass = GLTran.tranClass.Tax;
			tran.RefNbr = x.RefNbr;
			tran.TranDate = x.TranDate;
			tran.AccountID = x.AccountID;
			tran.SubID = x.SubID;
			tran.TranDesc = salestax.TaxID;
			tran.CuryDebitAmt = (APInvoiceType.DrCr(x.OrigTranType != null && x.OrigTranType.Trim() != string.Empty ? x.OrigTranType : x.TranType) == DrCr.Debit) ? 0m : x.CuryTaxAmt;
			tran.DebitAmt = (APInvoiceType.DrCr(x.OrigTranType != null && x.OrigTranType.Trim() != string.Empty ? x.OrigTranType : x.TranType) == DrCr.Debit) ? 0m : x.TaxAmt;
			tran.CuryCreditAmt = (APInvoiceType.DrCr(x.OrigTranType != null && x.OrigTranType.Trim() != string.Empty ? x.OrigTranType : x.TranType) == DrCr.Debit) ? x.CuryTaxAmt : 0m;
			tran.CreditAmt = (APInvoiceType.DrCr(x.OrigTranType != null && x.OrigTranType.Trim() != string.Empty ? x.OrigTranType : x.TranType) == DrCr.Debit) ? x.TaxAmt : 0m;
			tran.Released = true;
			tran.ReferenceID = apdoc.VendorID;
			tran.ProjectID = ProjectDefaultAttribute.NonProject(this);

			je.GLTranModuleBatNbr.Insert(tran);
		}

		protected virtual void PostGeneralTax(JournalEntry je, APInvoice apdoc, CurrencyInfo new_info, APTaxTran x, Tax salestax)
		{
			GLTran tran = new GLTran();
			tran.SummPost = this.SummPost;
			tran.BranchID = x.BranchID;
			tran.CuryInfoID = new_info.CuryInfoID;
			tran.TranType = x.TranType;
			tran.TranClass = GLTran.tranClass.Tax;
			tran.RefNbr = x.RefNbr;
			tran.TranDate = x.TranDate;
			tran.AccountID = (salestax.TaxType == CSTaxType.Use) ? salestax.ExpenseAccountID : x.AccountID;
			tran.SubID = (salestax.TaxType == CSTaxType.Use) ? salestax.ExpenseSubID : x.SubID;
			tran.TranDesc = salestax.TaxID;
			tran.CuryDebitAmt = (APInvoiceType.DrCr(x.OrigTranType != null && x.OrigTranType.Trim() != string.Empty ? x.OrigTranType : x.TranType) == DrCr.Debit) ? x.CuryTaxAmt : 0m;
			tran.DebitAmt = (APInvoiceType.DrCr(x.OrigTranType != null && x.OrigTranType.Trim() != string.Empty ? x.OrigTranType : x.TranType) == DrCr.Debit) ? x.TaxAmt : 0m;
			tran.CuryCreditAmt = (APInvoiceType.DrCr(x.OrigTranType != null && x.OrigTranType.Trim() != string.Empty ? x.OrigTranType : x.TranType) == DrCr.Debit) ? 0m : x.CuryTaxAmt;
			tran.CreditAmt = (APInvoiceType.DrCr(x.OrigTranType != null && x.OrigTranType.Trim() != string.Empty ? x.OrigTranType : x.TranType) == DrCr.Debit) ? 0m : x.TaxAmt;
			tran.Released = true;
			tran.ReferenceID = apdoc.VendorID;
			tran.ProjectID = ProjectDefaultAttribute.NonProject(this);
			je.GLTranModuleBatNbr.Insert(tran);
		}

		protected virtual void PostDirectTax(CurrencyInfo info, APTaxTran x, APInvoice orig_doc)
		{
			APTaxTran tran = PXSelect<APTaxTran, Where<APTaxTran.tranType, Equal<Required<APTaxTran.tranType>>, And<APTaxTran.refNbr, Equal<Required<APTaxTran.refNbr>>, And<APTaxTran.taxID, Equal<Required<APTaxTran.taxID>>, And<APTaxTran.module, Equal<BatchModule.moduleAP>>>>>>.Select(this, x.OrigTranType, x.OrigRefNbr, x.TaxID);

			if (tran == null)
			{
				tran = PXCache<APTaxTran>.CreateCopy(x);
				tran.TranType = x.OrigTranType;
				tran.RefNbr = x.OrigRefNbr;
				tran.OrigTranType = null;
				tran.OrigRefNbr = null;
				tran.CuryInfoID = orig_doc.CuryInfoID;
				tran.CuryTaxableAmt = 0m;
				tran.CuryTaxAmt = 0m;
				tran.Released = true;

				tran = PXCache<APTaxTran>.CreateCopy(APTaxTran_TranType_RefNbr.Insert(tran));
			}

			if (string.IsNullOrEmpty(tran.TaxPeriodID) == false)
			{
				throw new PXException(TX.Messages.CannotAdjustTaxForClosedOrPreparedPeriod, APTaxTran_TranType_RefNbr.Cache.GetValueExt<APTaxTran.taxPeriodID>(tran));
			}

			if (tran.TranType == APDocType.DebitAdj && x.TranType != APDocType.DebitAdj || tran.TranType != APDocType.DebitAdj && x.TranType == APDocType.DebitAdj)
			{
				tran.TaxZoneID = x.TaxZoneID;
				tran.CuryTaxableAmt -= x.CuryTaxableAmt;
				tran.CuryTaxAmt -= x.CuryTaxAmt;
				tran.TaxableAmt -= x.TaxableAmt;
				tran.TaxAmt -= x.TaxAmt;
			}
			else
			{
				tran.TaxZoneID = x.TaxZoneID;
				tran.CuryTaxableAmt += x.CuryTaxableAmt;
				tran.CuryTaxAmt += x.CuryTaxAmt;
				tran.TaxableAmt += x.TaxableAmt;
				tran.TaxAmt += x.TaxAmt;
			}

			CurrencyInfo orig_info = CurrencyInfo_CuryInfoID.Select(tran.CuryInfoID);

			if (orig_info != null && string.Equals(orig_info.CuryID, info.CuryID) == false)
			{
				PXCurrencyAttribute.CuryConvCury<APTaxTran.curyTaxableAmt>(APTaxTran_TranType_RefNbr.Cache, tran);
				PXCurrencyAttribute.CuryConvCury<APTaxTran.curyTaxAmt>(APTaxTran_TranType_RefNbr.Cache, tran);
			}

			APTaxTran_TranType_RefNbr.Update(tran);
		}

		public virtual INRegister CreatePPVAdjustment(APInvoice apdoc, List<LandedCostHelper.POReceiptLineAdjustment> pPVTransactions, List<APTran> pPVLines)
		{
			INAdjustmentEntry inGraph = PXGraph.CreateInstance<INAdjustmentEntry>();
            inGraph.Clear();

            inGraph.FieldVerifying.AddHandler<INTran.inventoryID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });
            inGraph.FieldVerifying.AddHandler<INTran.origRefNbr>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });

            inGraph.insetup.Current.RequireControlTotal = false;
			inGraph.insetup.Current.HoldEntry = false;

			INRegister newdoc = new INRegister();
			newdoc.DocType = INDocType.Adjustment;
			newdoc.OrigModule = BatchModule.AP;
			newdoc.OrigRefNbr = apdoc.RefNbr;
			newdoc.SiteID = null;
			newdoc.TranDate = apdoc.DocDate;
            newdoc.BranchID = apdoc.BranchID;
			newdoc.IsPPVTran = true;
			inGraph.adjustment.Insert(newdoc);

			LandedCostProcess lcGraph = PXGraph.CreateInstance<LandedCostProcess>();
			lcGraph.CreateAdjustmentTran(inGraph, pPVTransactions, null, null, posetup.PPVReasonCodeID);
			inGraph.Save.Press();

			foreach (APTran n in pPVLines)
			{
				n.PPVDocType = inGraph.adjustment.Current.DocType;
				n.PPVRefNbr = inGraph.adjustment.Current.RefNbr;
				APTran_TranType_RefNbr.Update(n);
			}

			return inGraph.adjustment.Current;
		}

		public static void GetPPVAccountSub(ref int? aAccountID, ref int? aSubID, PXGraph aGraph, POReceiptLine aRow, ReasonCode reasonCode, bool getPOAccrual = false)
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
					if (postClass == null)
						throw new PXException(PO.Messages.PostingClassIsNotDefinedForTheItemInReceiptRow, invItem.InventoryCD, aRow.ReceiptNbr, aRow.LineNbr);
					INSite invSite = PXSelect<INSite, Where<INSite.siteID, Equal<Required<POReceiptLine.siteID>>>>.Select(aGraph, aRow.SiteID);

					if (getPOAccrual)
					{
						aAccountID = INReleaseProcess.GetAcctID<INPostClass.pOAccrualAcctID>(aGraph, postClass.POAccrualAcctDefault, invItem, invSite, postClass);
						try
						{
							aSubID = INReleaseProcess.GetSubID<INPostClass.pOAccrualSubID>(aGraph, postClass.POAccrualAcctDefault, postClass.POAccrualSubMask, invItem, invSite, postClass);
						}
						catch (PXException ex)
						{
							if (postClass.POAccrualSubID == null
								|| string.IsNullOrEmpty(postClass.POAccrualSubMask)
									|| invItem.POAccrualSubID == null || invSite == null || invSite.POAccrualSubID == null)
								throw new PXException(PO.Messages.POAccrualSubAccountCanNotBeFoundForItemInReceiptRow, invItem.InventoryCD, aRow.ReceiptNbr, aRow.LineNbr, postClass.PostClassID, invSite != null ? invSite.SiteCD : String.Empty);
							else
								throw ex;
						}
					return;
					}

					if ((bool)invItem.StkItem)
					{
						if (aRow.LineType == POLineType.GoodsForDropShip)
						{
							aAccountID = INReleaseProcess.GetAcctID<INPostClass.cOGSAcctID>(aGraph, postClass.COGSAcctDefault, invItem, invSite, postClass);
							if (aAccountID == null)
								throw new PXException(PO.Messages.COGSAccountCanNotBeFoundForItemInReceiptRow, invItem.InventoryCD, aRow.ReceiptNbr, aRow.LineNbr, postClass.PostClassID, invSite != null ? invSite.SiteCD : String.Empty);
							try
							{
								aSubID = INReleaseProcess.GetSubID<INPostClass.cOGSSubID>(aGraph, postClass.COGSAcctDefault, postClass.COGSSubMask, invItem, invSite, postClass);
							}
							catch (PXException ex)
							{
								if (postClass.COGSSubID == null
									|| string.IsNullOrEmpty(postClass.COGSSubMask)
										|| invItem.COGSSubID == null || invSite == null || invSite.COGSSubID == null)
									throw new PXException(PO.Messages.COGSSubAccountCanNotBeFoundForItemInReceiptRow, invItem.InventoryCD, aRow.ReceiptNbr, aRow.LineNbr, postClass.PostClassID, invSite != null ? invSite.SiteCD : String.Empty);
								else
									throw ex;
							}
							if (aSubID == null)
								throw new PXException(PO.Messages.COGSSubAccountCanNotBeFoundForItemInReceiptRow, invItem.InventoryCD, aRow.ReceiptNbr, aRow.LineNbr, postClass.PostClassID, invSite != null ? invSite.SiteCD : String.Empty);
						}
						else
						{
							aAccountID = reasonCode.AccountID;
							if (aAccountID == null)
							{
								throw new PXException(PO.Messages.PPVAccountCanNotBeFoundForItemInReceiptRow, invItem.InventoryCD, aRow.ReceiptNbr, aRow.LineNbr, reasonCode.ReasonCodeID);
							}
							try
							{
								aSubID = INReleaseProcess.GetReasonCodeSubID<INPostClass.reasonCodeSubID>(aGraph, reasonCode, invItem, invSite, postClass);
							}
							catch (PXException ex)
							{
								if (reasonCode.SubID == null
									|| string.IsNullOrEmpty(reasonCode.SubMaskInventory)
										|| invItem == null || invSite == null)
								{
									throw new PXException(PO.Messages.PPVSubAccountCanNotBeFoundForItemInReceiptRow, invItem.InventoryCD, aRow.ReceiptNbr, aRow.LineNbr, reasonCode.ReasonCodeID, invSite != null ? invSite.SiteCD : String.Empty);
								}
								else
								{
									throw ex;
								}
							}
							if (aSubID == null)
								throw new PXException(PO.Messages.PPVSubAccountCanNotBeFoundForItemInReceiptRow, invItem.InventoryCD, aRow.ReceiptNbr, aRow.LineNbr, reasonCode.ReasonCodeID, invSite != null ? invSite.SiteCD : String.Empty);
						}

					}
					else
					{
						aAccountID = INReleaseProcess.GetAcctID<INPostClass.cOGSAcctID>(aGraph, postClass.COGSAcctDefault, invItem, invSite, postClass);
						try
						{
							aSubID = INReleaseProcess.GetSubID<INPostClass.cOGSSubID>(aGraph, postClass.COGSAcctDefault, postClass.COGSSubMask, invItem, invSite, postClass);
						}
						catch (PXException)
						{
							throw new PXException(Messages.ExpSubAccountCanNotBeAssembled);
						}
					}
				}
				else
				{
					throw new PXException(PO.Messages.PPVInventoryItemInReceiptRowIsNotFound, aRow.InventoryID, aRow.ReceiptNbr, aRow.LineNbr);
				}
			}
			else
			{
				aAccountID = aRow.ExpenseAcctID;
				aSubID = aRow.ExpenseSubID;
			}
		}

		public virtual POReceiptLineR1 UpdatePOReceiptLine(POReceiptLineR1 rctLine, APTran n)
		{
			return poReceiptLineUPD.Update(rctLine);
		}

		public virtual bool IsPPVCalcNeeded(POReceiptLineR1 rctLine, APTran tran)
		{
			return rctLine.LineType == PO.POLineType.GoodsForInventory ||
			       rctLine.LineType == PO.POLineType.GoodsForDropShip ||
			       rctLine.LineType == PO.POLineType.NonStockForSalesOrder ||
			       rctLine.LineType == PO.POLineType.GoodsForSalesOrder ||
			       rctLine.LineType == PO.POLineType.NonStockForDropShip ||
			       rctLine.LineType == PO.POLineType.GoodsForReplenishment ||
			       rctLine.LineType == PO.POLineType.NonStock;
		}

		/// <summary>
		/// Gets the amount to be posted to the expense account
		/// for the given document line.
		/// </summary>
		public static Amount GetExpensePostingAmount(PXGraph graph, APTran documentLine)
		{
			var documentLineWithTaxes = new PXSelectJoin<
				APTran,
					LeftJoin<APTax, 
						On<APTax.tranType, Equal<APTran.tranType>,
						And<APTax.refNbr, Equal<APTran.refNbr>,
						And<APTax.lineNbr, Equal<APTran.lineNbr>>>>, 
					LeftJoin<Tax,
						On<Tax.taxID, Equal<APTax.taxID>>>>,
				Where<
					APTran.tranType, Equal<Required<APTran.tranType>>,
					And<APTran.refNbr, Equal<Required<APTran.refNbr>>,
					And<APTran.lineNbr, Equal<Required<APTran.lineNbr>>>>>>
				(graph);

			PXResult<APTran, APTax, Tax> queryResult =
				documentLineWithTaxes
					.Select(documentLine.TranType, documentLine.RefNbr, documentLine.LineNbr)
					.Cast<PXResult<APTran, APTax, Tax>>()
					.First();

			Func<decimal, decimal> roundingFunction = amount => 
				PXDBCurrencyAttribute.Round(
					graph.Caches[typeof(APTran)],
					documentLine,
					amount,
					CMPrecision.TRANCURY);

			return GetExpensePostingAmount(documentLine, queryResult, queryResult, roundingFunction);
		}

		/// <summary>
		/// Gets the amount to be posted to the expense account for 
		/// the given document line.
		/// </summary>
		public static Amount GetExpensePostingAmount(APTran documentLine, APTax lineTax, Tax salesTax, Func<decimal, decimal> round)
		{
			if (lineTax?.TaxID != null &&
				salesTax != null &&
				salesTax.TaxCalcLevel == "0" &&
				salesTax.TaxType != CSTaxType.Withholding &&
				salesTax.ReverseTax != true)
			{
				decimal? curyAddUp = documentLine.CuryTranAmt
					- round((decimal)(documentLine.CuryTranAmt * documentLine.GroupDiscountRate * documentLine.DocumentDiscountRate));

				decimal? addUp = documentLine.TranAmt
					- round((decimal)(documentLine.TranAmt * documentLine.GroupDiscountRate * documentLine.DocumentDiscountRate));

				return new Amount(
					lineTax.CuryTaxableAmt + curyAddUp,
					lineTax.TaxableAmt + addUp);
			}
			else
			{
				return new Amount(
					documentLine.CuryTranAmt,
					documentLine.TranAmt);
			}
		}

		protected virtual void VerifyRoundingAllowed(APInvoice document, Batch batch, string baseCuryID)
		{
			//p. "document" is used in Visma customization (see AC-59109)

			if (this.InvoiceRounding == RoundingType.Currency && this.RequireControlTaxTotal != true)
			{
				throw new PXException(Messages.DocumentOutOfBalance);
			}
			decimal roundDiff = Math.Abs(Math.Round((decimal)(batch.DebitTotal - batch.CreditTotal), 4));
			if (roundDiff > this.RoundingLimit)
			{
				throw new PXException(Messages.RoundingAmountTooBig, baseCuryID, roundDiff,
					PXDBQuantityAttribute.Round(this.RoundingLimit));
			}
		}

		private static void Append(GLTran aDest, GLTran aSrc) 
		{			
			aDest.CuryCreditAmt += aSrc.CuryCreditAmt ?? Decimal.Zero;
			aDest.CreditAmt += aSrc.CreditAmt ?? Decimal.Zero;
			aDest.CuryDebitAmt += aSrc.CuryDebitAmt ?? Decimal.Zero;
			aDest.DebitAmt += aSrc.DebitAmt ?? Decimal.Zero;
		}

		private static void Invert(GLTran aRow)
		{
			Decimal? swap1 = aRow.CuryDebitAmt;
			Decimal? swap2 = aRow.DebitAmt;
			aRow.CuryDebitAmt = aRow.CuryCreditAmt;
			aRow.DebitAmt = aRow.CreditAmt;
			aRow.CuryCreditAmt = swap1;
			aRow.CreditAmt = swap2;
		}		

		private void UpdateWithholding(JournalEntry je, APAdjust adj, APRegister adjddoc, APPayment adjgdoc, Vendor vend, CurrencyInfo vouch_info)
		{
			APRegister apdoc = (APRegister)adjddoc;
			APRegister cached = (APRegister)APDocument.Cache.Locate(apdoc);
			if (cached != null)
			{
				apdoc = cached;
			}

			if (adjgdoc.DocType == APDocType.DebitAdj)
			{
				return;
			}

			if (PXCurrencyAttribute.IsNullOrEmpty(apdoc.CuryOrigWhTaxAmt))
			{
				return;
			}

			if (je.currencyinfo.Current == null)
			{
				throw new PXException();
			}

			PXResultset<APTaxTran> whtaxtrans = (PXResultset<APTaxTran>)WHTax_TranType_RefNbr.Select(apdoc.DocType, apdoc.RefNbr);

			int i = 0;
			decimal CuryAdjgWhTaxAmt = (decimal)adj.CuryAdjgWhTaxAmt;
			decimal AdjWhTaxAmt = (decimal)adj.AdjWhTaxAmt;

			foreach (PXResult<APTaxTran, Tax> whres in whtaxtrans)
			{
				Tax salesTax = (Tax)whres;
				APTaxTran taxtran = (APTaxTran)whres;

				if (apdoc.DocType == APDocType.QuickCheck || apdoc.DocType == APDocType.VoidQuickCheck)
				{
					taxtran.Released = true;
					WHTax_TranType_RefNbr.Update(taxtran);
					CreateGLTranForWhTaxTran(je, adj, adjgdoc, taxtran, vend, vouch_info, i == whtaxtrans.Count - 1);
				}
				else
				{
					APTaxTran whtran = new APTaxTran();
					whtran.Module = taxtran.Module;
					whtran.BranchID = adj.AdjgBranchID;
					whtran.TranType = adj.AdjgDocType;
					whtran.RefNbr = adj.AdjgRefNbr;
					whtran.AdjdDocType = adj.AdjdDocType;
					whtran.AdjdRefNbr = adj.AdjdRefNbr;
					whtran.AdjNbr = adj.AdjNbr;
					whtran.VendorID = taxtran.VendorID;
					whtran.TaxZoneID = taxtran.TaxZoneID;
					whtran.TaxID = taxtran.TaxID;
					whtran.TaxRate = taxtran.TaxRate;
					whtran.AccountID = taxtran.AccountID;
					whtran.SubID = taxtran.SubID;
					whtran.TaxType = taxtran.TaxType;
					whtran.TaxBucketID = taxtran.TaxBucketID;
					whtran.TranDate = adj.AdjgDocDate;
					whtran.CuryInfoID = adj.AdjgCuryInfoID;
					whtran.Released = true;
					whtran.CuryTaxableAmt = PXCurrencyAttribute.Round<APAdjust.adjgCuryInfoID>(Caches[typeof (APAdjust)], adj, ((decimal) adj.CuryAdjgAmt + (decimal) adj.CuryAdjgWhTaxAmt)*(decimal) taxtran.CuryTaxableAmt/(decimal) apdoc.CuryOrigDocAmt, CMPrecision.TRANCURY);

					if (i < whtaxtrans.Count - 1)
					{
						whtran.CuryTaxAmt = PXCurrencyAttribute.Round<APAdjust.adjgCuryInfoID>(Caches[typeof (APAdjust)], adj, (decimal) adj.CuryAdjgWhTaxAmt*(decimal) taxtran.CuryTaxAmt/(decimal) apdoc.CuryOrigWhTaxAmt, CMPrecision.TRANCURY);
						//insert, get back with base currency
						if (APTaxTran_TranType_RefNbr.Cache.ObjectsEqual(whtran, taxtran))
						{
							whtran.CreatedByID = taxtran.CreatedByID;
							whtran.CreatedByScreenID = taxtran.CreatedByScreenID;
							whtran.CreatedDateTime = taxtran.CreatedDateTime;
							whtran = (APTaxTran) APTaxTran_TranType_RefNbr.Cache.Update(whtran);
						}
						else
						{
							whtran = (APTaxTran) APTaxTran_TranType_RefNbr.Cache.Insert(whtran);
						}

						CuryAdjgWhTaxAmt -= (decimal) whtran.CuryTaxAmt;
						AdjWhTaxAmt -= (decimal) whtran.TaxAmt;
					}
					else
					{
						whtran.CuryTaxAmt = CuryAdjgWhTaxAmt;
						whtran.TaxAmt = AdjWhTaxAmt;

						//insert, do not get back not to recalc base cury
						if (APTaxTran_TranType_RefNbr.Cache.ObjectsEqual(whtran, taxtran))
						{
							whtran.CreatedByID = taxtran.CreatedByID;
							whtran.CreatedByScreenID = taxtran.CreatedByScreenID;
							whtran.CreatedDateTime = taxtran.CreatedDateTime;
							APTaxTran_TranType_RefNbr.Cache.Update(whtran);
						}
						else
						{
							APTaxTran_TranType_RefNbr.Cache.Insert(whtran);
						}

						CuryAdjgWhTaxAmt = 0m;
						AdjWhTaxAmt = 0m;
					}

					CreateGLTranForWhTaxTran(je, adj, adjgdoc, whtran, vend, vouch_info, i == whtaxtrans.Count - 1);
				}

				i++;
			}
		}

		protected virtual void CreateGLTranForWhTaxTran(JournalEntry je, APAdjust adj, APPayment adjgdoc, APTaxTran whtran, Vendor vend, CurrencyInfo vouch_info, bool updateHistory)
		{
			GLTran tran = new GLTran();
			tran.SummPost = this.SummPost;
			tran.BranchID = whtran.BranchID;
			tran.AccountID = whtran.AccountID;
			tran.SubID = whtran.SubID;
			tran.OrigAccountID = adj.AdjdAPAcct;
			tran.OrigSubID = adj.AdjdAPSub;
			tran.DebitAmt = (adjgdoc.DrCr == DrCr.Debit) ? whtran.TaxAmt : 0m;
			tran.CuryDebitAmt = (adjgdoc.DrCr == DrCr.Debit) ? whtran.CuryTaxAmt : 0m;
			tran.CreditAmt = (adjgdoc.DrCr == DrCr.Debit) ? 0m : whtran.TaxAmt;
			tran.CuryCreditAmt = (adjgdoc.DrCr == DrCr.Debit) ? 0m : whtran.CuryTaxAmt;
			tran.TranType = adj.AdjgDocType;
			tran.TranClass = GLTran.tranClass.WithholdingTax;
			tran.RefNbr = adj.AdjgRefNbr;
			tran.TranDesc = whtran.TaxID;
			tran.TranDate = adj.AdjgDocDate;
			tran.TranPeriodID = adj.AdjgTranPeriodID;
			tran.FinPeriodID = adj.AdjgFinPeriodID;
			tran.CuryInfoID = je.currencyinfo.Current.CuryInfoID;
			tran.Released = true;
			tran.ReferenceID = adjgdoc.VendorID;
			je.GLTranModuleBatNbr.Insert(tran);

			if (updateHistory)
			{
				tran.DebitAmt = (adjgdoc.DrCr == DrCr.Debit) ? adj.AdjWhTaxAmt : 0m;
				tran.CreditAmt = (adjgdoc.DrCr == DrCr.Debit) ? 0m : adj.AdjWhTaxAmt;
				UpdateHistory(tran, vend);

				tran.CuryDebitAmt = (adjgdoc.DrCr == DrCr.Debit) ? adj.CuryAdjdWhTaxAmt : 0m;
				tran.CuryCreditAmt = (adjgdoc.DrCr == DrCr.Debit) ? 0m : adj.CuryAdjdWhTaxAmt;
				UpdateHistory(tran, vend, vouch_info);
			}
		}

		protected virtual void AP1099Hist_FinYear_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (_IsIntegrityCheck)
			{
				e.Cancel = true;
			}
		}

		protected virtual void AP1099Hist_BoxNbr_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (_IsIntegrityCheck)
			{
				e.Cancel = true;
			}
		}

		protected virtual void AP1099Hist_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			if (((AP1099Hist)e.Row).BoxNbr == null)
			{
				e.Cancel = true;
			}
		}

		public static void Update1099Hist(PXGraph graph, decimal histMult, APAdjust adj, APTran tran, APRegister apdoc)
		{
            if (adj.AdjdDocType == APDocType.Prepayment || adj.AdjgDocType == APDocType.DebitAdj)
            {
                return;
            }

			if (adj.AdjgDocType == APDocType.VoidQuickCheck || adj.AdjgDocType == APDocType.Refund || adj.AdjdDocType == APDocType.DebitAdj)
            {
                histMult = -histMult;
            }
            
            PXCache cache = graph.Caches[typeof(AP1099Hist)];
			string Year1099 = ((DateTime)adj.AdjgDocDate).Year.ToString();

			if (apdoc != null && apdoc.OrigDocAmt != 0m)
			{
				AP1099Yr year = new AP1099Yr();
				year.FinYear = Year1099;

				year = (AP1099Yr)graph.Caches[typeof(AP1099Yr)].Insert(year);

				AP1099Hist hist = new AP1099Hist();
				hist.BranchID = adj.AdjgBranchID;
				hist.VendorID = apdoc.VendorID;
				hist.FinYear = Year1099;
				hist.BoxNbr = tran.Box1099;

				hist = (AP1099Hist)cache.Insert(hist);

				if (hist != null)
				{
					hist.HistAmt += PXCurrencyAttribute.BaseRound(graph, histMult * (decimal)tran.TranAmt * (decimal)adj.AdjAmt / (decimal)apdoc.OrigDocAmt);
				}
			}
		}

		private void Update1099(APAdjust adj, APRegister apdoc)
		{
			string Year1099 = ((DateTime)adj.AdjgDocDate).Year.ToString();

			foreach(APTran tran in AP1099Tran_Select.Select(apdoc.DocType, apdoc.RefNbr))
			{
				AP1099Year year = PXSelect<AP1099Year, Where<AP1099Year.finYear, Equal<Required<AP1099Year.finYear>>>>.Select(this, Year1099);
				if (year == null)
				{
					year = new AP1099Yr();
					year.FinYear = Year1099;
					year.Status = "N";
					year = (AP1099Year) AP1099Year_Select.Cache.Insert(year);
				}
				else if (_IsIntegrityCheck == false && year.Status != "N")
				{
					throw new PXException(Messages.AP1099_PaymentDate_NotIn_OpenYear, PXUIFieldAttribute.GetDisplayName<APPayment.adjDate>(APPayment_DocType_RefNbr.Cache));
				}
				Update1099Hist(this, 1, adj, tran, apdoc);
			}
		}

		public virtual void UpdateBalances(APAdjust adj, APRegister adjddoc, Vendor vendor)
		{
			APRegister apdoc = (APRegister)adjddoc;
			APRegister cached = (APRegister)APDocument.Cache.Locate(apdoc);
			if (cached != null)
			{
				apdoc = cached;
			}
			else if (_IsIntegrityCheck == true)
			{
				return;
			}

			if (_IsIntegrityCheck == false && adj.VoidAdjNbr != null)
			{
				APAdjust voidadj = PXSelect<APAdjust, Where<APAdjust.adjgDocType, Equal<Required<APAdjust.adjgDocType>>, And<APAdjust.adjgRefNbr, Equal<Required<APAdjust.adjgRefNbr>>, And<APAdjust.adjdDocType, Equal<Required<APAdjust.adjdDocType>>, And<APAdjust.adjdRefNbr, Equal<Required<APAdjust.adjdRefNbr>>, And<APAdjust.adjNbr, Equal<Required<APAdjust.adjNbr>>>>>>>>.Select(this, (adj.AdjgDocType == APDocType.VoidCheck ? APDocType.Check : adj.AdjgDocType), adj.AdjgRefNbr, adj.AdjdDocType, adj.AdjdRefNbr, adj.VoidAdjNbr);

				if (voidadj != null)
				{
					if ((bool)voidadj.Voided)
					{
						throw new PXException(Messages.DocumentApplicationAlreadyVoided);
					}

					voidadj.Voided = true;
					Caches[typeof(APAdjust)].Update(voidadj);

					adj.AdjAmt = -voidadj.AdjAmt;
					adj.RGOLAmt = -voidadj.RGOLAmt;

					Caches[typeof(APAdjust)].Update(adj);
				}
			}

			if (apdoc.DocType == APDocType.Prepayment)
			{
				if (adj.AdjgDocType == APDocType.VoidCheck || adj.VoidAdjNbr != null)
				{
					if (Math.Abs((decimal)(apdoc.CuryOrigDocAmt - apdoc.CuryDocBal)) > 0m)
					{
						throw new PXException(Messages.PrepaymentCheckCannotBeVoided, adj.AdjgRefNbr, apdoc.RefNbr);
					}
					else
					{
						foreach (APAdjust oldadj in APAdjust_AdjgDocType_RefNbr_VendorID.Select(apdoc.DocType, apdoc.RefNbr, apdoc.LineCntr))
						{
							throw new PXException(Messages.PrepaymentCheckCannotBeVoided, adj.AdjgRefNbr, apdoc.RefNbr);
						}
					}

					apdoc.OpenDoc = false;
					apdoc.Voided = true;
					apdoc.ClosedFinPeriodID = adj.AdjgFinPeriodID;
					apdoc.ClosedTranPeriodID = adj.AdjgTranPeriodID;
					apdoc.CuryDocBal = 0m;
					apdoc.DocBal = 0m;

					apdoc = (APRegister)APDocument.Cache.Update(apdoc);

                    if (_IsIntegrityCheck == false)
                    {
                        PXDatabase.Update<POOrder>(
                       new PXDataFieldAssign<POOrder.prepaymentDocType>(null),
                       new PXDataFieldAssign<POOrder.prepaymentRefNbr>(null),
                       new PXDataFieldRestrict<POOrder.prepaymentDocType>(PXDbType.VarChar, 3, apdoc.DocType, PXComp.EQ),
                       new PXDataFieldRestrict<POOrder.prepaymentRefNbr>(PXDbType.NVarChar, 15, apdoc.RefNbr, PXComp.EQ)
                       );
                    }

					return;
				}
				else if (adj.AdjgDocType == APDocType.Check)
				{
					if (_IsIntegrityCheck)
					{
						//check for prepayment request will be processed last
						apdoc.CuryDocBal += adj.CuryAdjgAmt;
						apdoc.DocBal += adj.AdjAmt;

						if (apdoc.CuryDocBal == 0m)
						{
							apdoc.DocBal = 0m;
							apdoc.OpenDoc = false;

							foreach (APAdjust adjmax in PXSelectGroupBy<APAdjust, Where<APAdjust.adjgDocType, Equal<Required<APAdjust.adjgDocType>>, And<APAdjust.adjgRefNbr, Equal<Required<APAdjust.adjgRefNbr>>, And<Where<APAdjust.released, Equal<True>, Or<APAdjust.adjNbr, Equal<Required<APAdjust.adjNbr>>>>>>>, Aggregate<GroupBy<APAdjust.adjgDocType, GroupBy<APAdjust.adjgRefNbr, Max<APAdjust.adjgFinPeriodID, Max<APAdjust.adjgTranPeriodID>>>>>>.Select(this, apdoc.DocType, apdoc.RefNbr, short.MaxValue))
							{
								apdoc.ClosedFinPeriodID = adjmax.AdjgFinPeriodID;
								apdoc.ClosedTranPeriodID = adjmax.AdjgTranPeriodID;
							}
						}
						else
						{
							apdoc.OpenDoc = true;
							apdoc.ClosedFinPeriodID = null;
							apdoc.ClosedTranPeriodID = null;
						}

						apdoc = (APRegister)APDocument.Cache.Update(apdoc);
						return;
					}

					if (Math.Abs((decimal)apdoc.CuryOrigDocAmt - (decimal)adj.CuryAdjgAmt) != 0m)
					{
						throw new PXException(Messages.PrepaymentNotPayedFull, apdoc.RefNbr);
					}

					APPayment prepayment = (APPayment)APPayment_DocType_RefNbr.Cache.Extend<APRegister>(apdoc);
					prepayment.CreatedByID = apdoc.CreatedByID;
					prepayment.CreatedByScreenID = apdoc.CreatedByScreenID;
					prepayment.CreatedDateTime = apdoc.CreatedDateTime;
					prepayment.CashAccountID = null;
					prepayment.PaymentMethodID = null;
					prepayment.ExtRefNbr = null;

					prepayment.DocDate = adj.AdjgDocDate;
					prepayment.TranPeriodID = adj.AdjgTranPeriodID;
					prepayment.FinPeriodID = adj.AdjgFinPeriodID;

					prepayment.AdjDate = prepayment.DocDate;
					prepayment.AdjTranPeriodID = prepayment.TranPeriodID;
					prepayment.AdjFinPeriodID = prepayment.FinPeriodID;
					prepayment.Printed = true;

					APAddressAttribute.DefaultRecord<APPayment.remitAddressID>(APPayment_DocType_RefNbr.Cache, prepayment);
					APContactAttribute.DefaultRecord<APPayment.remitContactID>(APPayment_DocType_RefNbr.Cache, prepayment);

					APPayment_DocType_RefNbr.Cache.Update(prepayment);

					TaxAttribute.SetTaxCalc<APTran.taxCategoryID>(APTran_TranType_RefNbr.Cache, null, TaxCalc.NoCalc);
					PXDBCurrencyAttribute.SetBaseCalc<APPayment.curyDocBal>(APPayment_DocType_RefNbr.Cache, prepayment, true);

					CurrencyInfo ppinfo = CurrencyInfo_CuryInfoID.Select(prepayment.CuryInfoID);
					CurrencyInfo info = CurrencyInfo_CuryInfoID.Select(adj.AdjgCuryInfoID);

					PXCache<CurrencyInfo>.RestoreCopy(ppinfo, info);
					ppinfo.CuryInfoID = prepayment.CuryInfoID;

					CurrencyInfo_CuryInfoID.Cache.SetStatus(ppinfo, PXEntryStatus.Updated);

					APDocument.Cache.SetStatus(apdoc, PXEntryStatus.Notchanged);

					return;
				}
			}

			apdoc.CuryDocBal -= (adj.CuryAdjdAmt + adj.CuryAdjdDiscAmt + adj.CuryAdjdWhTaxAmt );
			apdoc.DocBal -= (adj.AdjAmt + adj.AdjDiscAmt + adj.AdjWhTaxAmt + (adj.ReverseGainLoss == false ? adj.RGOLAmt : -adj.RGOLAmt));
			apdoc.CuryDiscBal -= adj.CuryAdjdDiscAmt;
			apdoc.DiscBal -= adj.AdjDiscAmt;
			apdoc.CuryWhTaxBal -= adj.CuryAdjdWhTaxAmt;
			apdoc.WhTaxBal -= adj.AdjWhTaxAmt;
			apdoc.CuryDiscTaken += adj.CuryAdjdDiscAmt;
			apdoc.DiscTaken += adj.AdjDiscAmt;
			apdoc.CuryTaxWheld += adj.CuryAdjdWhTaxAmt;
			apdoc.TaxWheld += adj.AdjWhTaxAmt;

			apdoc.RGOLAmt += adj.RGOLAmt;

			if (apdoc.CuryDiscBal == 0m)
			{
				apdoc.DiscBal = 0m;
			}

			if (apdoc.CuryWhTaxBal == 0m)
			{
				apdoc.WhTaxBal = 0m;
			}

			if (_IsIntegrityCheck == false && apdoc.CuryDocBal < 0m)
			{
				throw new PXException(Messages.DocumentBalanceNegative);
			}

			if (_IsIntegrityCheck == false && adj.AdjgDocDate < adjddoc.DocDate)
			{
				throw new PXException(Messages.ApplDate_Less_DocDate, PXUIFieldAttribute.GetDisplayName<APPayment.adjDate>(APPayment_DocType_RefNbr.Cache));
			}

			if (_IsIntegrityCheck == false && string.Compare(adj.AdjgFinPeriodID, adjddoc.FinPeriodID) < 0)
			{
				throw new PXException(Messages.ApplPeriod_Less_DocPeriod, PXUIFieldAttribute.GetDisplayName<APPayment.adjFinPeriodID>(APPayment_DocType_RefNbr.Cache));
			}

			if (apdoc.CuryDocBal == 0m)
			{
				apdoc.CuryDiscBal = 0m;
				apdoc.DiscBal = 0m;
				apdoc.CuryWhTaxBal = 0m;
				apdoc.WhTaxBal = 0m;
				apdoc.DocBal = 0m;
				apdoc.OpenDoc = false;
				string closedFinPeriodID = string.Empty;
				string closedTranPeriodID = string.Empty;

				foreach (APAdjust adjmax in PXSelectGroupBy<APAdjust, Where<APAdjust.adjdDocType, Equal<Required<APAdjust.adjdDocType>>, And<APAdjust.adjdRefNbr, Equal<Required<APAdjust.adjdRefNbr>>, And<Where<APAdjust.released, Equal<True>, Or<APAdjust.adjNbr, Equal<Required<APAdjust.adjNbr>>>>>>>, Aggregate<GroupBy<APAdjust.adjdDocType, GroupBy<APAdjust.adjdRefNbr, Max<APAdjust.adjgFinPeriodID, Max<APAdjust.adjgTranPeriodID>>>>>>.Select(this, apdoc.DocType, apdoc.RefNbr, adj.AdjNbr))
				{
					apdoc.ClosedFinPeriodID = closedFinPeriodID = adjmax.AdjgFinPeriodID;
					apdoc.ClosedTranPeriodID = closedTranPeriodID = adjmax.AdjgTranPeriodID;
				}

				foreach (APAdjust adjmax in PXSelectGroupBy<APAdjust, Where<APAdjust.adjgDocType, Equal<Required<APAdjust.adjgDocType>>, And<APAdjust.adjgRefNbr, Equal<Required<APAdjust.adjgRefNbr>>, And<Where<APAdjust.released, Equal<True>, Or<APAdjust.adjNbr, Equal<Required<APAdjust.adjNbr>>>>>>>, Aggregate<GroupBy<APAdjust.adjgDocType, GroupBy<APAdjust.adjgRefNbr, Max<APAdjust.adjgFinPeriodID, Max<APAdjust.adjgTranPeriodID>>>>>>.Select(this, apdoc.DocType, apdoc.RefNbr, adj.AdjNbr))
				{
					apdoc.ClosedFinPeriodID = string.Compare(closedFinPeriodID, adjmax.AdjgFinPeriodID) < 0 ? adjmax.AdjgFinPeriodID : closedFinPeriodID;
					apdoc.ClosedTranPeriodID = string.Compare(closedTranPeriodID, adjmax.AdjgTranPeriodID) < 0 ? adjmax.AdjgTranPeriodID : closedTranPeriodID;
				}
			}
			else
			{
				if (apdoc.CuryDocBal == apdoc.CuryOrigDocAmt)
				{
					apdoc.CuryDiscBal = apdoc.CuryOrigDiscAmt;
					apdoc.DiscBal = apdoc.OrigDiscAmt;
					apdoc.CuryWhTaxBal = apdoc.CuryOrigWhTaxAmt;
					apdoc.WhTaxBal = apdoc.OrigWhTaxAmt;
					apdoc.CuryDiscTaken = 0m;
					apdoc.DiscTaken = 0m;
					apdoc.CuryTaxWheld = 0m;
					apdoc.TaxWheld = 0m;
				}

				apdoc.OpenDoc = true;
				apdoc.ClosedTranPeriodID = null;
				apdoc.ClosedFinPeriodID = null;
			}

			apdoc = (APRegister) APDocument.Cache.Update(apdoc);
		}


		private void UpdateVoidedCheck(APPayment voidcheck, string OrigDocType)
		{
			foreach (PXResult<APPayment, CurrencyInfo, Currency, Vendor> res in APPayment_DocType_RefNbr.Search<APPayment.vendorID>(voidcheck.VendorID, OrigDocType, voidcheck.RefNbr))
			{
				APPayment payment = (APPayment)res;
				if (_IsIntegrityCheck == false && string.Equals(voidcheck.ExtRefNbr, payment.ExtRefNbr, StringComparison.OrdinalIgnoreCase) == false)
				{
					throw new PXException(Messages.VoidAppl_CheckNbr_NotMatchOrigPayment);
				}

				APRegister apdoc = (APRegister)payment;
				APRegister cached = (APRegister)APDocument.Cache.Locate(apdoc);
				if (cached != null)
				{
					apdoc = cached;
				}

				apdoc.Voided = true;
				apdoc.OpenDoc = false;
				apdoc.CuryDocBal = 0m;
				apdoc.DocBal = 0m;
				apdoc.ClosedTranPeriodID = voidcheck.TranPeriodID;
				apdoc.ClosedFinPeriodID = voidcheck.FinPeriodID;
				APDocument.Cache.Update(apdoc);
			}
		}

		protected void DeactivateOneTimeVendorIfAllDocsIsClosed(Vendor vendor)
		{
			if (vendor.Status != BAccount.status.OneTime)
				return;

			APRegister apRegister = PXSelect<APRegister,
												Where<APRegister.vendorID, Equal<Required<APRegister.vendorID>>,
														And<APRegister.released, Equal<True>,
														And<APRegister.openDoc, Equal<True>>>>>
												.SelectWindowed(this, 0, 1, vendor.BAccountID);

			if (apRegister != null)
				return;

			vendor.Status = BAccount.status.Inactive;
			Caches[typeof(Vendor)].Update(vendor);
			Caches[typeof(Vendor)].Persist(PXDBOperation.Update);
			Caches[typeof(Vendor)].Persisted(false);
		}

		public virtual void ReleaseDocProc(JournalEntry je, ref APRegister doc, PXResult<APPayment, CurrencyInfo, Currency, Vendor, CashAccount> res)
		{
			ReleaseDocProc(je, ref doc, res, doc.LineCntr);

			var payment = (APPayment) res;
			var vendor = (Vendor)res;

			ClosePayment(doc, payment, vendor);

			//increment default for AdjNbr
			doc.LineCntr++;
		}

		public virtual void ReleaseDocProc(JournalEntry je, ref APRegister doc, PXResult<APPayment, CurrencyInfo, Currency, Vendor, CashAccount> res, int? AdjNbr)
		{
			APPayment apdoc = (APPayment)res;
			CurrencyInfo info = (CurrencyInfo)res;
			Currency paycury = (Currency)res;
			Vendor vend = (Vendor)res;
			CashAccount cashacct = (CashAccount)res;

			VendorClass vendclass = (VendorClass)PXSelectJoin<VendorClass, InnerJoin<APSetup, On<APSetup.dfltVendorClassID, Equal<VendorClass.vendorClassID>>>>.Select(this, null);

			CurrencyInfo new_info = PXCache<CurrencyInfo>.CreateCopy(info);
			new_info.CuryInfoID = null;
			new_info.ModuleCode = "GL";
			new_info = je.currencyinfo.Insert(new_info) ?? new_info;

			if (doc.Released == false)
			{
                //should always restore ARRegister to ARPayment after invoice part release of cash sale
                PXCache<APRegister>.RestoreCopy(apdoc, doc);

				GLTran tran = new GLTran();
				tran.SummPost = this.SummPost;
				tran.BranchID = cashacct.BranchID;
                tran.AccountID = cashacct.AccountID;
                tran.SubID = cashacct.SubID;
				tran.CuryDebitAmt = (apdoc.DrCr == DrCr.Debit) ? apdoc.CuryOrigDocAmt : 0m;
				tran.DebitAmt = (apdoc.DrCr == DrCr.Debit) ? apdoc.OrigDocAmt : 0m;
				tran.CuryCreditAmt = (apdoc.DrCr == DrCr.Debit) ? 0m : apdoc.CuryOrigDocAmt;
				tran.CreditAmt = (apdoc.DrCr == DrCr.Debit) ? 0m : apdoc.OrigDocAmt;
				tran.TranType = apdoc.DocType;
				tran.TranClass = apdoc.DocClass;
				tran.RefNbr = apdoc.RefNbr;
				tran.TranDesc = apdoc.DocDesc;
				tran.TranDate = apdoc.DocDate;
				tran.TranPeriodID = apdoc.TranPeriodID;
				tran.FinPeriodID = apdoc.FinPeriodID;
				tran.CuryInfoID = new_info.CuryInfoID;
				tran.Released = true;
				tran.CATranID = apdoc.CATranID;
				tran.ReferenceID = apdoc.VendorID;
                tran.ProjectID = PM.ProjectDefaultAttribute.NonProject(this);
				tran.NonBillable = true;

				je.GLTranModuleBatNbr.Insert(tran);

				/*Debit Payment AP Account*/
				tran = new GLTran();
				tran.SummPost = true;
                //if (apdoc.DocType == APDocType.QuickCheck || apdoc.DocType == APDocType.VoidQuickCheck || apdoc.DocType == APDocType.Check || apdoc.DocType == APDocType.VoidCheck)
                if (!APPaymentType.CanHaveBalance(apdoc.DocType))
				{
					tran.ZeroPost = false;
				}
				tran.BranchID = apdoc.BranchID;
				tran.AccountID = apdoc.APAccountID;
				tran.ReclassificationProhibited = true;
				tran.SubID = apdoc.APSubID;
				tran.CuryDebitAmt = (apdoc.DrCr == DrCr.Debit) ? 0m : apdoc.CuryOrigDocAmt;
				tran.DebitAmt = (apdoc.DrCr == DrCr.Debit) ? 0m : apdoc.OrigDocAmt;
				tran.CuryCreditAmt = (apdoc.DrCr == DrCr.Debit) ? apdoc.CuryOrigDocAmt : 0m;
				tran.CreditAmt = (apdoc.DrCr == DrCr.Debit) ? apdoc.OrigDocAmt : 0m;
				tran.TranType = apdoc.DocType;
				tran.TranClass = GLTran.tranClass.Payment;
				tran.RefNbr = apdoc.RefNbr;
				tran.TranDesc = apdoc.DocDesc;
				tran.TranDate = apdoc.DocDate;
				tran.TranPeriodID = apdoc.TranPeriodID;
				tran.FinPeriodID = apdoc.FinPeriodID;
				tran.CuryInfoID = new_info.CuryInfoID;
				tran.Released = true;
				tran.ReferenceID = apdoc.VendorID;
                tran.ProjectID = PM.ProjectDefaultAttribute.NonProject(this);
				tran.NonBillable = true;

				UpdateHistory(tran, vend);
				UpdateHistory(tran, vend, new_info);

				je.GLTranModuleBatNbr.Insert(tran);


                foreach (APPaymentChargeTran charge in APPaymentChargeTran_DocType_RefNbr.Select(doc.DocType, doc.RefNbr))
                {
					bool isCADebit = charge.DrCr == DrCr.Debit || doc.DocType == APDocType.VoidQuickCheck;
					
					tran = new GLTran();
					tran.SummPost = this.SummPost;
					tran.BranchID = cashacct.BranchID;
                    tran.AccountID = cashacct.AccountID;
                    tran.SubID = cashacct.SubID;
					tran.CuryDebitAmt = isCADebit ? charge.CuryTranAmt : 0m;
					tran.DebitAmt = isCADebit ? charge.TranAmt : 0m;
					tran.CuryCreditAmt = isCADebit ? 0m : charge.CuryTranAmt;
					tran.CreditAmt = isCADebit ? 0m : charge.TranAmt;
                    tran.TranType = charge.DocType;
                    tran.TranClass = apdoc.DocClass;
                    tran.RefNbr = charge.RefNbr;
                    tran.TranDesc = charge.TranDesc;
                    tran.TranDate = charge.TranDate;
                    tran.TranPeriodID = charge.TranPeriodID;
                    tran.FinPeriodID = charge.FinPeriodID;
                    tran.CuryInfoID = new_info.CuryInfoID;
                    tran.Released = true;
                    tran.CATranID = charge.CashTranID;
                    tran.ReferenceID = apdoc.VendorID;

                    je.GLTranModuleBatNbr.Insert(tran);

                    tran = new GLTran();
                    tran.SummPost = true;
                    tran.ZeroPost = false;
                    tran.BranchID = apdoc.BranchID;
                    tran.AccountID = charge.AccountID;
                    tran.SubID = charge.SubID;
					tran.CuryDebitAmt = isCADebit ? 0m : charge.CuryTranAmt;
					tran.DebitAmt = isCADebit ? 0m : charge.TranAmt;
					tran.CuryCreditAmt = isCADebit ? charge.CuryTranAmt : 0m;
					tran.CreditAmt = isCADebit ? charge.TranAmt : 0m;
                    tran.TranType = charge.DocType;
                    tran.TranClass = GLTran.tranClass.Charge;
                    tran.RefNbr = charge.RefNbr;
                    tran.TranDesc = charge.TranDesc;
                    tran.TranDate = charge.TranDate;
                    tran.TranPeriodID = charge.TranPeriodID;
                    tran.FinPeriodID = charge.FinPeriodID;
                    tran.CuryInfoID = new_info.CuryInfoID;
                    tran.Released = true;
                    tran.ReferenceID = apdoc.VendorID;

                    je.GLTranModuleBatNbr.Insert(tran);
                }


				doc.CuryDocBal = doc.CuryOrigDocAmt;
				doc.DocBal = doc.OrigDocAmt;

				doc.Voided = false;
				doc.OpenDoc = true;
				doc.ClosedFinPeriodID = null;
				doc.ClosedTranPeriodID = null;

				if ((bool)apdoc.VoidAppl)
				{
					UpdateVoidedCheck(apdoc, APDocType.Check);
					UpdateVoidedCheck(apdoc, APDocType.Prepayment);
				}
				else
				{
                    PaymentMethod paytype = PXSelect<PaymentMethod, Where<PaymentMethod.paymentMethodID, Equal<Required<PaymentMethod.paymentMethodID>>>>.Select(this, apdoc.PaymentMethodID);
					if (_IsIntegrityCheck == false && paytype != null && (bool)paytype.PrintOrExport && (bool)apdoc.Printed == false)
					{
						throw new PXException(Messages.Check_NotPrinted_CannotRelease);
					}
				}
			}
			else if (_IsIntegrityCheck && doc.DocType == APDocType.Prepayment && doc.LineCntr == 0)
			{ 
				//this is the only good place to reset prepayment request balance
				doc.CuryDocBal = 0m;
				doc.DocBal = 0m;
			}

			if (doc.DocType == APDocType.QuickCheck || doc.DocType == APDocType.VoidQuickCheck)
			{
				if (_IsIntegrityCheck == false)
				{
					PXDatabase.Delete<APAdjust>(
						new PXDataFieldRestrict("AdjgDocType", PXDbType.Char, 3, doc.DocType, PXComp.EQ),
						new PXDataFieldRestrict("AdjgRefNbr", PXDbType.VarChar, 15, doc.RefNbr, PXComp.EQ)
						);

					APAdjust adj = new APAdjust();
					adj.AdjdDocType = doc.DocType;
					adj.AdjdRefNbr = doc.RefNbr;
					adj.AdjdBranchID = doc.BranchID;
					adj.AdjgDocType = doc.DocType;
					adj.AdjgRefNbr = doc.RefNbr;
					adj.AdjgBranchID = doc.BranchID;
					adj.AdjdAPAcct = doc.APAccountID;
					adj.AdjdAPSub = doc.APSubID;
					adj.AdjdCuryInfoID = doc.CuryInfoID;
					adj.AdjdDocDate = doc.DocDate;
					adj.AdjdFinPeriodID = doc.FinPeriodID;
					adj.AdjdTranPeriodID = doc.TranPeriodID;
					adj.AdjdOrigCuryInfoID = doc.CuryInfoID;
					adj.AdjgCuryInfoID = doc.CuryInfoID;
					adj.AdjgDocDate = doc.DocDate;
					adj.AdjgFinPeriodID = doc.FinPeriodID;
					adj.AdjgTranPeriodID = doc.TranPeriodID;
					adj.AdjNbr = doc.LineCntr;
					adj.AdjAmt = doc.OrigDocAmt;
					adj.AdjDiscAmt = doc.OrigDiscAmt;
					adj.AdjWhTaxAmt = doc.OrigWhTaxAmt;
					adj.RGOLAmt = 0m;
					adj.CuryAdjdAmt = doc.CuryOrigDocAmt;
					adj.CuryAdjdDiscAmt = doc.CuryOrigDiscAmt;
					adj.CuryAdjdWhTaxAmt = doc.CuryOrigWhTaxAmt;
					adj.CuryAdjgAmt = doc.CuryOrigDocAmt;
					adj.CuryAdjgDiscAmt = doc.CuryOrigDiscAmt;
					adj.CuryAdjgWhTaxAmt = doc.CuryOrigWhTaxAmt;
					adj.Released = false;
					adj.VendorID = doc.VendorID;
					APAdjust_AdjgDocType_RefNbr_VendorID.Cache.Insert(adj);
				}

				if (doc.DocType == APDocType.VoidQuickCheck)
				{
					UpdateVoidedCheck(apdoc, APDocType.QuickCheck);
				}

				if (!_IsIntegrityCheck                     // always during classical release
					|| (_IsIntegrityCheck && AdjNbr == 0)) // only one time during balance validation
				{
				doc.CuryDocBal += doc.CuryOrigDiscAmt + doc.CuryOrigWhTaxAmt;
				doc.DocBal += doc.OrigDiscAmt + doc.OrigWhTaxAmt;
				doc.ClosedFinPeriodID = doc.FinPeriodID;
				doc.ClosedTranPeriodID = doc.TranPeriodID;
			}
			}

			if (_IsIntegrityCheck == false) 
			{
				foreach (PTInstTran iTran in this.ptInstanceTrans.Select(doc.DocType, doc.RefNbr)) 
				{
					iTran.Released = true;
					this.ptInstanceTrans.Update(iTran);						
				}
			}

			doc.Released = true;

			APAdjust prev_adj = new APAdjust();
			CurrencyInfo prev_info = new CurrencyInfo();

			foreach (PXResult<APAdjust, CurrencyInfo, Currency, APInvoice, APPayment, Standalone.APRegisterAlias> adjres in APAdjust_AdjgDocType_RefNbr_VendorID.Select(doc.DocType, doc.RefNbr, AdjNbr))
			{
				APAdjust adj = (APAdjust)adjres;
				CurrencyInfo vouch_info = (CurrencyInfo)adjres;
				prev_info = (CurrencyInfo)adjres;
				Currency cury = (Currency)adjres;
				APInvoice adjddoc = (APInvoice)adjres;
				APPayment adjgdoc = (APPayment)adjres;

				// Restore full invoice / payment from the "single table" stripped version.
				// -
				if (adjddoc?.RefNbr != null)
				{
					PXCache<APRegister>.RestoreCopy(adjddoc, (Standalone.APRegisterAlias)adjres);
				}
				else if (adjgdoc?.RefNbr != null)
				{
					PXCache<APRegister>.RestoreCopy(adjgdoc, (Standalone.APRegisterAlias)adjres);
				}

				if (adj.CuryAdjgAmt == 0m && adj.CuryAdjgDiscAmt == 0m && adj.CuryAdjgWhTaxAmt == 0m)
				{
					APAdjust_AdjgDocType_RefNbr_VendorID.Cache.Delete(adj);
					continue;
				}

				if (adj.Hold == true)
				{
					throw new PXException(Messages.Document_OnHold_CannotRelease);
				}

				if (adjddoc.RefNbr != null)
				{

					UpdateBalances(adj, adjddoc, vend);
					if (_IsIntegrityCheck == false && vend.Vendor1099 == true)
					{
						Update1099(adj, adjddoc);
					}

					if (_IsIntegrityCheck == false)
					{
						UpdateWithholding(je, adj, adjddoc, apdoc, vend, vouch_info);
					}
				}
				else
				{
					UpdateBalances(adj, adjgdoc, vend);
				}

				/*Credit Payment AP Account*/
				GLTran tran = new GLTran();
				tran.SummPost = true;
				tran.ZeroPost = false;
				tran.BranchID = adj.AdjgBranchID;
				tran.AccountID = apdoc.APAccountID;
				tran.ReclassificationProhibited = true;
				tran.SubID = apdoc.APSubID;
                tran.DebitAmt = (adj.AdjgGLSign == 1m) ? 0m : adj.AdjAmt;
                tran.CuryDebitAmt = (adj.AdjgGLSign == 1m) ? 0m : adj.CuryAdjgAmt;
                tran.CreditAmt = (adj.AdjgGLSign == 1m) ? adj.AdjAmt : 0m;
                tran.CuryCreditAmt = (adj.AdjgGLSign == 1m) ? adj.CuryAdjgAmt : 0m;
                tran.TranType = adj.AdjgDocType;
				tran.TranClass = GLTran.tranClass.Payment;
				tran.RefNbr = adj.AdjgRefNbr;
				tran.TranDesc = apdoc.DocDesc;
				tran.TranDate = adj.AdjgDocDate;
				tran.TranPeriodID = adj.AdjgTranPeriodID;
				tran.FinPeriodID = adj.AdjgFinPeriodID;
				tran.CuryInfoID = new_info.CuryInfoID;
				tran.Released = true;
				tran.ReferenceID = apdoc.VendorID;
				tran.ProjectID = PM.ProjectDefaultAttribute.NonProject(this);

				UpdateHistory(tran, vend);
				UpdateHistory(tran, vend, new_info);

				je.GLTranModuleBatNbr.Insert(tran);

				/*Debit Voucher AP Account/minus RGOL for refund*/
				tran = new GLTran();
				tran.SummPost = true;
				tran.ZeroPost = false;
				tran.BranchID = adj.AdjdBranchID;
				tran.AccountID = adj.AdjdAPAcct;
				tran.ReclassificationProhibited = true;
				tran.SubID = adj.AdjdAPSub;
                tran.CreditAmt = (adj.AdjgGLSign == 1m) ? 0m : adj.AdjAmt + adj.AdjDiscAmt + adj.AdjWhTaxAmt - adj.RGOLAmt;
                tran.CuryCreditAmt = (adj.AdjgGLSign == 1m) ? 0m : (object.Equals(new_info.CuryID, new_info.BaseCuryID) ? tran.CreditAmt : adj.CuryAdjgAmt + adj.CuryAdjgDiscAmt + adj.CuryAdjgWhTaxAmt);
                tran.DebitAmt = (adj.AdjgGLSign == 1m) ? adj.AdjAmt + adj.AdjDiscAmt + adj.AdjWhTaxAmt + adj.RGOLAmt : 0m;
                tran.CuryDebitAmt = (adj.AdjgGLSign == 1m) ? (object.Equals(new_info.CuryID, new_info.BaseCuryID) ? tran.DebitAmt : adj.CuryAdjgAmt + adj.CuryAdjgDiscAmt + adj.CuryAdjgWhTaxAmt) : 0m;
				tran.TranType = adj.AdjgDocType;
				//always N for AdjdDocs except Prepayment
				tran.TranClass = APDocType.DocClass(adj.AdjdDocType);
				tran.RefNbr = adj.AdjgRefNbr;
				tran.TranDesc = apdoc.DocDesc;
				tran.TranDate = adj.AdjgDocDate;
				tran.TranPeriodID = adj.AdjgTranPeriodID;
				tran.FinPeriodID = adj.AdjgFinPeriodID;
				tran.CuryInfoID = new_info.CuryInfoID;
				tran.Released = true;
				tran.ReferenceID = apdoc.VendorID;
				tran.ProjectID = PM.ProjectDefaultAttribute.NonProject(this);

				UpdateHistory(tran, vend);

				je.GLTranModuleBatNbr.Insert(tran);

				/*Update CuryHistory in Voucher currency*/
                tran.CuryCreditAmt = (adj.AdjgGLSign == 1m) ? 0m : (object.Equals(vouch_info.CuryID, vouch_info.BaseCuryID) ? tran.CreditAmt : adj.CuryAdjdAmt + adj.CuryAdjdDiscAmt + adj.CuryAdjdWhTaxAmt);
                tran.CuryDebitAmt = (adj.AdjgGLSign == 1m) ? (object.Equals(vouch_info.CuryID, vouch_info.BaseCuryID) ? tran.DebitAmt : adj.CuryAdjdAmt + adj.CuryAdjdDiscAmt + adj.CuryAdjdWhTaxAmt) : 0m;
				UpdateHistory(tran, vend, vouch_info);

				/*Credit Discount Taken/does not apply to refund, since no disc in AD*/
				tran = new GLTran();
				tran.SummPost = this.SummPost;
				tran.BranchID = adj.AdjdBranchID;
				tran.AccountID = vend.DiscTakenAcctID;
				tran.SubID = vend.DiscTakenSubID;
				tran.OrigAccountID = adj.AdjdAPAcct;
				tran.OrigSubID = adj.AdjdAPSub;
                tran.DebitAmt = (adj.AdjgGLSign == 1m) ? 0m : adj.AdjDiscAmt;
                tran.CuryDebitAmt = (adj.AdjgGLSign == 1m) ? 0m : adj.CuryAdjgDiscAmt;
                tran.CreditAmt = (adj.AdjgGLSign == 1m) ? adj.AdjDiscAmt : 0m;
                tran.CuryCreditAmt = (adj.AdjgGLSign == 1m) ? adj.CuryAdjgDiscAmt : 0m;
				tran.TranType = adj.AdjgDocType;
				tran.TranClass = GLTran.tranClass.Discount;
				tran.RefNbr = adj.AdjgRefNbr;
				tran.TranDesc = apdoc.DocDesc;
				tran.TranDate = adj.AdjgDocDate;
				tran.TranPeriodID = adj.AdjgTranPeriodID;
				tran.FinPeriodID = adj.AdjgFinPeriodID;
				tran.CuryInfoID = new_info.CuryInfoID;
				tran.Released = true;
				tran.ReferenceID = apdoc.VendorID;
				tran.ProjectID = PM.ProjectDefaultAttribute.NonProject(this);

				UpdateHistory(tran, vend);

				je.GLTranModuleBatNbr.Insert(tran);

				/*Update CuryHistory in Voucher currency*/
                tran.CuryDebitAmt = (adj.AdjgGLSign == 1m) ? 0m : adj.CuryAdjdDiscAmt;
                tran.CuryCreditAmt = (adj.AdjgGLSign == 1m) ? adj.CuryAdjdDiscAmt : 0m;
				UpdateHistory(tran, vend, vouch_info);

				/*Debit/Credit RGOL Account*/
				if (cury.RealGainAcctID != null && cury.RealLossAcctID != null)
				{
					tran = new GLTran();
					tran.SummPost = this.SummPost;
					tran.BranchID = adj.AdjdBranchID;
					tran.AccountID = (adj.RGOLAmt > 0m && !(bool)adj.VoidAppl || adj.RGOLAmt < 0m && (bool)adj.VoidAppl) 
                        ? cury.RealGainAcctID 
                        : cury.RealLossAcctID;
					tran.SubID = (adj.RGOLAmt > 0m && !(bool)adj.VoidAppl || adj.RGOLAmt < 0m && (bool)adj.VoidAppl)
                        ? GainLossSubAccountMaskAttribute.GetSubID<Currency.realGainSubID>(je, tran.BranchID, cury)
                        : GainLossSubAccountMaskAttribute.GetSubID<Currency.realLossSubID>(je, tran.BranchID, cury);
					tran.OrigAccountID = adj.AdjdAPAcct;
					tran.OrigSubID = adj.AdjdAPSub;
					tran.DebitAmt = (adj.RGOLAmt < 0m) ? -1m * adj.RGOLAmt : 0m;
					//!object.Equals(vouch_info.CuryID, vouch_info.BaseCuryID) === precision alteration before payment application
					tran.CuryDebitAmt = object.Equals(new_info.CuryID, new_info.BaseCuryID) && !object.Equals(vouch_info.CuryID, vouch_info.BaseCuryID) ? tran.DebitAmt : 0m;
					tran.CreditAmt = (adj.RGOLAmt > 0m) ? adj.RGOLAmt : 0m;
					tran.CuryCreditAmt = object.Equals(new_info.CuryID, new_info.BaseCuryID) && !object.Equals(vouch_info.CuryID, vouch_info.BaseCuryID) ? tran.CreditAmt : 0m;
					tran.TranType = adj.AdjgDocType;
					tran.TranClass = GLTran.tranClass.RealizedAndRoundingGOL;
					tran.RefNbr = adj.AdjgRefNbr;
					tran.TranDesc = apdoc.DocDesc;
					tran.TranDate = adj.AdjgDocDate;
					tran.TranPeriodID = adj.AdjgTranPeriodID;
					tran.FinPeriodID = adj.AdjgFinPeriodID;
					tran.CuryInfoID = new_info.CuryInfoID;
					tran.Released = true;
					tran.ReferenceID = apdoc.VendorID;
					tran.ProjectID = PM.ProjectDefaultAttribute.NonProject(this);

					UpdateHistory(tran, vend);

					je.GLTranModuleBatNbr.Insert(tran);

					/*Update CuryHistory in Voucher currency*/
					tran.CuryDebitAmt = 0m;
					tran.CuryCreditAmt = 0m;
					UpdateHistory(tran, vend, vouch_info);
				}
				//Debit/Credit Rounding Gain-Loss Account
				else if (paycury.RoundingGainAcctID != null && paycury.RoundingLossAcctID != null)
				{
					tran = new GLTran();
					tran.SummPost = this.SummPost;
					tran.BranchID = adj.AdjdBranchID;
					tran.AccountID = (adj.RGOLAmt > 0m && !(bool)adj.VoidAppl || adj.RGOLAmt < 0m && (bool)adj.VoidAppl) 
                        ? paycury.RoundingGainAcctID 
                        : paycury.RoundingLossAcctID;
					tran.SubID = (adj.RGOLAmt > 0m && !(bool)adj.VoidAppl || adj.RGOLAmt < 0m && (bool)adj.VoidAppl)
                        ? GainLossSubAccountMaskAttribute.GetSubID<Currency.roundingGainSubID>(je, tran.BranchID, paycury)
                        : GainLossSubAccountMaskAttribute.GetSubID<Currency.roundingLossSubID>(je, tran.BranchID, paycury); 
                    tran.OrigAccountID = adj.AdjdAPAcct;
					tran.OrigSubID = adj.AdjdAPSub;
					tran.DebitAmt = (adj.RGOLAmt < 0m) ? -1m * adj.RGOLAmt : 0m;
					//!object.Equals(vouch_info.CuryID, vouch_info.BaseCuryID) === precision alteration before payment application
					tran.CuryDebitAmt = object.Equals(new_info.CuryID, new_info.BaseCuryID) && !object.Equals(vouch_info.CuryID, vouch_info.BaseCuryID) ? tran.DebitAmt : 0m;
					tran.CreditAmt = (adj.RGOLAmt > 0m) ? adj.RGOLAmt : 0m;
					tran.CuryCreditAmt = object.Equals(new_info.CuryID, new_info.BaseCuryID) && !object.Equals(vouch_info.CuryID, vouch_info.BaseCuryID) ? tran.CreditAmt : 0m;
					tran.TranType = adj.AdjgDocType;
					tran.TranClass = GLTran.tranClass.RealizedAndRoundingGOL;
					tran.RefNbr = adj.AdjgRefNbr;
					tran.TranDesc = apdoc.DocDesc;
					tran.TranDate = adj.AdjgDocDate;
					tran.TranPeriodID = adj.AdjgTranPeriodID;
					tran.FinPeriodID = adj.AdjgFinPeriodID;
					tran.CuryInfoID = new_info.CuryInfoID;
					tran.Released = true;
					tran.ReferenceID = apdoc.VendorID;
					tran.ProjectID = PM.ProjectDefaultAttribute.NonProject(this);

					UpdateHistory(tran, vend);

					je.GLTranModuleBatNbr.Insert(tran);

					/*Update CuryHistory in Voucher currency*/
					tran.CuryDebitAmt = 0m;
					tran.CuryCreditAmt = 0m;
					UpdateHistory(tran, vend, vouch_info);
				}

				//true for Cash Sale and Reverse Cash Sale
				if (adj.AdjgDocType != adj.AdjdDocType || adj.AdjgRefNbr != adj.AdjdRefNbr)
				{
                    doc.CuryDocBal -= adj.AdjgBalSign * adj.CuryAdjgAmt;
                    doc.DocBal -= adj.AdjgBalSign * adj.AdjAmt;
				}

				if (_IsIntegrityCheck == false)
				{
					if (je.GLTranModuleBatNbr.Cache.IsInsertedUpdatedDeleted)
					{
						je.Save.Press();
					}

					if (!je.BatchModule.Cache.IsDirty)
					{
						adj.AdjBatchNbr = ((Batch)je.BatchModule.Current).BatchNbr;
					}
					adj.Released = true;
					prev_adj = (APAdjust)Caches[typeof(APAdjust)].Update(adj);
				}

				ProcessSVATAdjustments(adj, adjddoc, doc);
			}

            if (_IsIntegrityCheck == false && ((bool)apdoc.VoidAppl == false && doc.CuryDocBal < 0m || (bool)apdoc.VoidAppl && doc.CuryDocBal > 0m))
            {
                throw new PXException(Messages.DocumentBalanceNegative);
            }

			if (doc.CuryDocBal == 0m && doc.DocBal != 0m && prev_adj.AdjdRefNbr != null)
			{
				if ((bool)prev_adj.VoidAppl || object.Equals(new_info.CuryID, new_info.BaseCuryID))
				{
					throw new PXException();
				}

				//BaseCalc should be false
				prev_adj.AdjAmt += doc.DocBal;

				decimal? roundingLoss = prev_adj.ReverseGainLoss == false ? doc.DocBal : -doc.DocBal;
				prev_adj.RGOLAmt -= roundingLoss;

				prev_adj = (APAdjust)Caches[typeof(APAdjust)].Update(prev_adj);

				//signs are reversed to RGOL
				GLTran tran = new GLTran();
				tran.SummPost = this.SummPost;
				tran.BranchID = apdoc.BranchID; 
				tran.AccountID = (roundingLoss < 0m) 
                    ? paycury.RoundingGainAcctID 
                    : paycury.RoundingLossAcctID;
				tran.SubID = (roundingLoss < 0m)
                    ? GainLossSubAccountMaskAttribute.GetSubID<Currency.roundingGainSubID>(je, tran.BranchID, paycury)
                    : GainLossSubAccountMaskAttribute.GetSubID<Currency.roundingLossSubID>(je, tran.BranchID, paycury); 
                tran.OrigAccountID = prev_adj.AdjdAPAcct;
				tran.OrigSubID = prev_adj.AdjdAPSub;
				tran.DebitAmt = (roundingLoss > 0m) ? roundingLoss : 0m;
				tran.CuryDebitAmt = 0m;
				tran.CreditAmt = (roundingLoss < 0m) ? -roundingLoss : 0m;
				tran.CuryCreditAmt = 0m;
				tran.TranType = prev_adj.AdjgDocType;
				tran.TranClass = GLTran.tranClass.RealizedAndRoundingGOL;
				tran.RefNbr = prev_adj.AdjgRefNbr;
				tran.TranDesc = apdoc.DocDesc;
				tran.TranDate = prev_adj.AdjgDocDate;
				tran.TranPeriodID = prev_adj.AdjgTranPeriodID;
				tran.FinPeriodID = prev_adj.AdjgFinPeriodID;
				tran.CuryInfoID = new_info.CuryInfoID;
				tran.Released = true;
				tran.ReferenceID = apdoc.VendorID;
				tran.ProjectID = PM.ProjectDefaultAttribute.NonProject(this);

				UpdateHistory(tran, vend);
				//Update CuryHistory in Voucher currency
				UpdateHistory(tran, vend, prev_info);

				var adjdDoc = (APRegister)APDocument.Cache.Locate(new APRegister { DocType = prev_adj.AdjdDocType, RefNbr = prev_adj.AdjdRefNbr });
				if (adjdDoc != null)
				{
					adjdDoc.RGOLAmt -= roundingLoss;
					APDocument.Cache.Update(adjdDoc);
				}

				je.GLTranModuleBatNbr.Insert(tran);

				//Credit Payment AR Account
				tran = new GLTran();
				tran.SummPost = true;
				tran.ZeroPost = false;
				tran.BranchID = apdoc.BranchID; 
				tran.AccountID = apdoc.APAccountID;
				tran.ReclassificationProhibited = true;
				tran.SubID = apdoc.APSubID;
				tran.CreditAmt = (roundingLoss > 0m) ? roundingLoss : 0m;
				tran.CuryCreditAmt = 0m;
				tran.DebitAmt = (roundingLoss < 0m) ? -roundingLoss : 0m;
				tran.CuryDebitAmt = 0m;
				tran.TranType = prev_adj.AdjgDocType;
				tran.TranClass = GLTran.tranClass.Payment;
				tran.RefNbr = prev_adj.AdjgRefNbr;
				tran.TranDesc = apdoc.DocDesc;
				tran.TranDate = prev_adj.AdjgDocDate;
				tran.TranPeriodID = prev_adj.AdjgTranPeriodID;
				tran.FinPeriodID = prev_adj.AdjgFinPeriodID;
				tran.CuryInfoID = new_info.CuryInfoID;
				tran.Released = true;
				tran.ReferenceID = apdoc.VendorID;
				tran.OrigAccountID = prev_adj.AdjdAPAcct;
				tran.OrigSubID = prev_adj.AdjdAPSub;
				tran.ProjectID = PM.ProjectDefaultAttribute.NonProject(this);

				UpdateHistory(tran, vend);
				//Update CuryHistory in Payment currency
				UpdateHistory(tran, vend, new_info);

				je.GLTranModuleBatNbr.Insert(tran);
			}
		}

		protected virtual void ProcessSVATAdjustments(APAdjust adj, APRegister adjddoc, APRegister adjgdoc)
		{
			if (PXAccess.FeatureInstalled<FeaturesSet.vATReporting>() && _IsIntegrityCheck == false)
			{
				foreach (SVATConversionHist docSVAT in PXSelect<SVATConversionHist, Where<
					SVATConversionHist.module, Equal<BatchModule.moduleAP>,
					And2<Where<SVATConversionHist.adjdDocType, Equal<Current<APAdjust.adjdDocType>>,
						And<SVATConversionHist.adjdRefNbr, Equal<Current<APAdjust.adjdRefNbr>>,
						Or<SVATConversionHist.adjdDocType, Equal<Current<APAdjust.adjgDocType>>,
						And<SVATConversionHist.adjdRefNbr, Equal<Current<APAdjust.adjgRefNbr>>>>>>,
					And<SVATConversionHist.reversalMethod, Equal<SVATTaxReversalMethods.onPayments>,
					And<Where<SVATConversionHist.adjdDocType, Equal<SVATConversionHist.adjgDocType>,
						And<SVATConversionHist.adjdRefNbr, Equal<SVATConversionHist.adjgRefNbr>>>>>>>>
					.SelectMultiBound(this, new object[] { adj }))
				{
					bool isPayment = adj.AdjgDocType == docSVAT.AdjdDocType && adj.AdjgRefNbr == docSVAT.AdjdRefNbr;
					decimal percent = isPayment
						? ((adj.CuryAdjgAmt ?? 0m) + (adj.CuryAdjgDiscAmt ?? 0m)) / (adjgdoc.CuryOrigDocAmt ?? 0m)
						: ((adj.CuryAdjdAmt ?? 0m) + (adj.CuryAdjdDiscAmt ?? 0m)) / (adjddoc.CuryOrigDocAmt ?? 0m);
					decimal curyTaxableAmt = PXDBCurrencyAttribute.RoundCury(SVATConversionHistory.Cache, docSVAT, (docSVAT.CuryTaxableAmt ?? 0m) * percent);
					decimal curyTaxAmt = PXDBCurrencyAttribute.RoundCury(SVATConversionHistory.Cache, docSVAT, (docSVAT.CuryTaxAmt ?? 0m) * percent);

					SVATConversionHist adjSVAT = new SVATConversionHist
					{
						Module = BatchModule.AP,
						AdjdBranchID = adj.AdjdBranchID,
						AdjdDocType = isPayment ? adj.AdjgDocType : adj.AdjdDocType,
						AdjdRefNbr = isPayment ? adj.AdjgRefNbr : adj.AdjdRefNbr,
						AdjgDocType = isPayment ? adj.AdjdDocType : adj.AdjgDocType,
						AdjgRefNbr = isPayment ? adj.AdjdRefNbr : adj.AdjgRefNbr,
						AdjNbr = adj.AdjNbr,
						AdjdDocDate = adj.AdjgDocDate,
						AdjdFinPeriodID = adj.AdjdFinPeriodID,

						TaxID = docSVAT.TaxID,
						TaxType = docSVAT.TaxType,
						TaxRate = docSVAT.TaxRate,
						VendorID = docSVAT.VendorID,
						ReversalMethod = SVATTaxReversalMethods.OnPayments,

						CuryInfoID = docSVAT.CuryInfoID,
						CuryTaxableAmt = curyTaxableAmt,
						CuryTaxAmt = curyTaxAmt,
						CuryUnrecognizedTaxAmt = curyTaxAmt
					};

					adjSVAT.FillBaseAmounts(SVATConversionHistory.Cache);

					if (adjSVAT.CuryTaxAmt != docSVAT.CuryTaxAmt &&
						(isPayment ? adjgdoc.CuryDocBal : adjddoc.CuryDocBal) == 0m)
					{
						PXResultset<SVATConversionHist> rows = PXSelect<SVATConversionHist, Where<
							SVATConversionHist.module, Equal<BatchModule.moduleAP>,
							And<SVATConversionHist.adjdDocType, Equal<Current<SVATConversionHist.adjdDocType>>,
							And<SVATConversionHist.adjdRefNbr, Equal<Current<SVATConversionHist.adjdRefNbr>>,
							And<SVATConversionHist.taxID, Equal<Current<SVATConversionHist.taxID>>,
							And<Where<SVATConversionHist.adjdDocType, NotEqual<SVATConversionHist.adjgDocType>,
								Or<SVATConversionHist.adjdRefNbr, NotEqual<SVATConversionHist.adjgRefNbr>>>>>>>>>
							.SelectMultiBound(this, new object[] { docSVAT });
						if (rows.Any())
						{
							adjSVAT.CuryTaxableAmt = docSVAT.CuryTaxableAmt;
							adjSVAT.TaxableAmt = docSVAT.TaxableAmt;
							adjSVAT.CuryTaxAmt = docSVAT.CuryTaxAmt;
							adjSVAT.TaxAmt = docSVAT.TaxAmt;

							foreach (SVATConversionHist row in rows)
							{
								adjSVAT.CuryTaxableAmt -= (row.CuryTaxableAmt ?? 0m);
								adjSVAT.TaxableAmt -= (row.TaxableAmt ?? 0m);
								adjSVAT.CuryTaxAmt -= (row.CuryTaxAmt ?? 0m);
								adjSVAT.TaxAmt -= (row.TaxAmt ?? 0m);
							}

							adjSVAT.CuryUnrecognizedTaxAmt = adjSVAT.CuryTaxAmt;
							adjSVAT.UnrecognizedTaxAmt = adjSVAT.TaxAmt;
						}
					}

					adjSVAT = (SVATConversionHist)SVATConversionHistory.Cache.Insert(adjSVAT);

					docSVAT.Processed = false;
					docSVAT.AdjgFinPeriodID = null;

					PXTimeStampScope.PutPersisted(SVATConversionHistory.Cache, docSVAT, PXDatabase.SelectTimeStamp());
					SVATConversionHistory.Cache.Update(docSVAT);
				}
			}
		}

        private void SegregateBatch(JournalEntry je, int? branchID, string curyID, DateTime? docDate, string finPeriodID, string description, CurrencyInfo curyInfo)
		{
            JournalEntry.SegregateBatch(je, BatchModule.AP, branchID, curyID, docDate, finPeriodID, description, curyInfo, null);
		}

		public virtual List<APRegister> ReleaseDocProc(JournalEntry je, APRegister doc, bool isPrebooking, out List<INRegister> inDocs)
		{
			List<APRegister> ret = null;
			inDocs = null;

			if ((bool)doc.Hold)
			{
				throw new PXException(Messages.Document_OnHold_CannotRelease);
			}

			//using (new PXConnectionScope())
			{
				using (PXTransactionScope ts = new PXTransactionScope())
				{
					//mark as updated so that doc will not expire from cache and update with Released = 1 will not override balances/amount in document
					APDocument.Cache.SetStatus(doc, PXEntryStatus.Updated);

					foreach (PXResult<APInvoice, CurrencyInfo, Terms, Vendor> res in APInvoice_DocType_RefNbr.Select((object)doc.DocType, doc.RefNbr))
					{
					    Vendor v = res;
					    switch (v.Status)
					    {
					        case Vendor.status.Inactive:   
					        case Vendor.status.Hold:  
                                throw new PXSetPropertyException(Messages.VendorIsInStatus, new Vendor.status.ListAttribute().ValueLabelDic[v.Status]);
					    }

						//must check for AD application in different period
						if ((bool)doc.Released == false)
						{
                            SegregateBatch(je, doc.BranchID, doc.CuryID, doc.DocDate, doc.FinPeriodID, doc.DocDesc, (CurrencyInfo)res);
						}
						ret = ReleaseDocProc(je, ref doc, res, isPrebooking, out inDocs);
						//ensure correct PXDBDefault behaviour on APTran persisting
						APInvoice_DocType_RefNbr.Current = (APInvoice)res;
					}

					foreach (PXResult<APPayment, CurrencyInfo, Currency, Vendor, CashAccount> res in APPayment_DocType_RefNbr.Select((object)doc.DocType, doc.RefNbr))
					{
                        Vendor v = res;
                        switch (v.Status)
                        {
                            case Vendor.status.Inactive:
                            case Vendor.status.Hold:
                            case Vendor.status.HoldPayments:
                                throw new PXSetPropertyException(Messages.VendorIsInStatus, new Vendor.status.ListAttribute().ValueLabelDic[v.Status]);
                        }
                        
						if (doc.DocType == APDocType.QuickCheck || doc.DocType == APDocType.VoidQuickCheck || doc.DocType == APDocType.DebitAdj)
						{
							if (doc.Prebooked == true && doc.Released != true) continue; //We don't need payment part processing on release;
						}

						if ((doc.DocType == APDocType.Check || doc.DocType == APDocType.VoidCheck || doc.DocType == APDocType.Prepayment) && doc.Released == false)
                        {
                            SegregateBatch(je, doc.BranchID, doc.CuryID, ((APPayment)res).DocDate, ((APPayment)res).FinPeriodID, doc.DocDesc, (CurrencyInfo)res);
							ReleaseDocProc(je, ref doc, res, -1);

							if (je.GLTranModuleBatNbr.Cache.IsInsertedUpdatedDeleted)
							{
								je.Save.Press();
							}

							if (!je.BatchModule.Cache.IsDirty && string.IsNullOrEmpty(doc.BatchNbr))
							{
								doc.BatchNbr = je.BatchModule.Current.BatchNbr;
							}

							Dictionary<string, List<object>> appsbyperiod = new Dictionary<string, List<object>>();
							Dictionary<string, DateTime?> datesbyperiod = new Dictionary<string, DateTime?>();
							bool IsFutureAppl = false;

							foreach(PXResult<APAdjust> adjres in APAdjust_AdjgDocType_RefNbr_VendorID.Select(doc.DocType, doc.RefNbr, doc.LineCntr))
							{
								APAdjust adj = adjres;
								IsFutureAppl |= string.Compare(adj.AdjdFinPeriodID, adj.AdjgFinPeriodID) > 0;
								IsFutureAppl |= DateTime.Compare((DateTime)adj.AdjdDocDate, (DateTime)adj.AdjgDocDate) > 0;

								adj.AdjgFinPeriodID = string.Compare(adj.AdjdFinPeriodID, adj.AdjgFinPeriodID) > 0 ? adj.AdjdFinPeriodID : adj.AdjgFinPeriodID;
								adj.AdjgDocDate = DateTime.Compare((DateTime)adj.AdjdDocDate, (DateTime)adj.AdjgDocDate) > 0 ? adj.AdjdDocDate : adj.AdjgDocDate;

								List<object> apps;
								if (!appsbyperiod.TryGetValue(adj.AdjgFinPeriodID, out apps))
								{
									appsbyperiod[adj.AdjgFinPeriodID] = apps = new List<object>();
								}
								apps.Add(adjres);

								DateTime? maxdate;
								if (!datesbyperiod.TryGetValue(adj.AdjgFinPeriodID, out maxdate))
								{
									datesbyperiod[adj.AdjgFinPeriodID] = maxdate = adj.AdjgDocDate;
								}
								
								if (DateTime.Compare((DateTime)adj.AdjgDocDate, (DateTime)maxdate) > 0)
								{
									datesbyperiod[adj.AdjgFinPeriodID] = adj.AdjgDocDate;
								}

								if (doc.OpenDoc == false)
								{
									//this is true for VoidCheck
									doc.OpenDoc = true;
									doc.CuryDocBal = doc.CuryOrigDocAmt;
									doc.DocBal = doc.OrigDocAmt;
								}
							}

							int i = -2;
							foreach(KeyValuePair<string, List<object>> pair in appsbyperiod)
							{
								APAdjust_AdjgDocType_RefNbr_VendorID.StoreCached(new PXCommandKey(new object[] { doc.DocType, doc.RefNbr, i }), pair.Value);
								if (IsFutureAppl)
								{
									SegregateBatch(je, doc.BranchID, doc.CuryID, datesbyperiod[pair.Key], pair.Key, doc.DocDesc, (CurrencyInfo)res);
								}
								ReleaseDocProc(je, ref doc, res, i);
								i--;
							}

							var payment = (APPayment)res;
							var vendor = (Vendor)res;

							ClosePayment(doc, payment, vendor);

							//increment default for AdjNbr
							doc.LineCntr++;
						}
						else
						{
							if (doc.DocType != APDocType.QuickCheck && doc.DocType != APDocType.VoidQuickCheck)
                            {
                                SegregateBatch(je, doc.BranchID, doc.CuryID, ((APPayment)res).AdjDate, ((APPayment)res).AdjFinPeriodID, doc.DocDesc, (CurrencyInfo)res);
							}

							ReleaseDocProc(je, ref doc, res);
						}
						//ensure correct PXDBDefault behaviour on APAdjust persisting
						APPayment_DocType_RefNbr.Current = (APPayment)res;
					}

                    if (_IsIntegrityCheck == false)
                    {
                        if (je.GLTranModuleBatNbr.Cache.IsInsertedUpdatedDeleted)
                        {
                            je.Save.Press();
                        }

                        //leave BatchNbr empty for Prepayment Requests
                        if (!je.BatchModule.Cache.IsDirty && string.IsNullOrEmpty(doc.BatchNbr) && (APInvoice_DocType_RefNbr.Current == null || APInvoice_DocType_RefNbr.Current.DocType != APDocType.Prepayment))
                        {
							if (!isPrebooking)
							{
								doc.BatchNbr = ((Batch)je.BatchModule.Current).BatchNbr;
								if (doc.DocType == APDocType.VoidQuickCheck)
								{
									doc.PrebookBatchNbr = null; //Void Quick check is not prebooked by itself, but may contain a reference on the prebook batch of the original Quick Check. 
								}
							}
							else
							{
								doc.PrebookBatchNbr = ((Batch)je.BatchModule.Current).BatchNbr;
							}
                        }
                    }

					//bool Released = (bool)doc.Released;
					//doc.Released = true;

					#region Auto Commit/Post document to avalara.

					APInvoice apDoc = doc as APInvoice;
					if (apDoc != null && doc.IsTaxValid == true && AvalaraMaint.IsExternalTax(this, apDoc.TaxZoneID) && apDoc.InstallmentNbr == null)
					{
						TXAvalaraSetup avalaraSetup = PXSelect<TXAvalaraSetup>.Select(this);
						if (avalaraSetup != null && avalaraSetup.IsActive == true)
						{
							TaxSvc service = new TaxSvc();
							AvalaraMaint.SetupService(this, service);

							CommitTaxRequest request = new CommitTaxRequest();
							request.CompanyCode = AvalaraMaint.CompanyCodeFromBranch(this, doc.BranchID);
							request.DocCode = string.Format("AP.{0}.{1}", doc.DocType, doc.RefNbr);

							if (doc.DocType == APDocType.Refund)
								request.DocType = DocumentType.ReturnInvoice;
							else
								request.DocType = DocumentType.PurchaseInvoice;


							CommitTaxResult result = service.CommitTax(request);
							if (result.ResultCode == SeverityLevel.Success)
							{
								doc.IsTaxPosted = true;
							}
							else
							{
								//Avalara retuned an error - The given document is already marked as posted on the avalara side.
								if (result.ResultCode == SeverityLevel.Error && result.Messages.Count == 1 &&
									result.Messages[0].Details == "Expected Posted")
								{
									//ignore this error - everything is cool
								}
								else
								{
									//show as warning.
									StringBuilder sb = new StringBuilder();
									foreach (Avalara.AvaTax.Adapter.Message msg in result.Messages)
									{
										sb.AppendLine(msg.Name + ": " + msg.Details);
									}

									if (sb.Length > 0)
									{
										apDoc.WarningMessage = PXMessages.LocalizeFormatNoPrefixNLA(AR.Messages.PostingToAvalaraFailed, sb.ToString());
									}
								}
							}
						}
					}
					#endregion

					bool Released = (bool)doc.Released && isPrebooking==false;
					doc.Released = !isPrebooking;
					if (doc.Prebooked == false)
						doc.Prebooked = isPrebooking;
					
					doc = (APRegister)APDocument.Cache.Update(doc);

					PXTimeStampScope.DuplicatePersisted(APDocument.Cache, doc, typeof(APInvoice));
					PXTimeStampScope.DuplicatePersisted(APDocument.Cache, doc, typeof(APPayment));

					if (doc.DocType == APDocType.DebitAdj)
					{
						if (Released)
						{
							APPayment_DocType_RefNbr.Cache.SetStatus(APPayment_DocType_RefNbr.Current, PXEntryStatus.Notchanged);
						}
						else
						{
							APPayment debitadj = (APPayment)APPayment_DocType_RefNbr.Cache.Extend<APRegister>(doc);
							debitadj.CreatedByID = doc.CreatedByID;
							debitadj.CreatedByScreenID = doc.CreatedByScreenID;
							debitadj.CreatedDateTime = doc.CreatedDateTime;
							debitadj.CashAccountID = null;
							debitadj.PaymentMethodID = null;
                            debitadj.DepositAsBatch = false;
							debitadj.ExtRefNbr = null;
							debitadj.AdjDate = debitadj.DocDate;
							debitadj.AdjTranPeriodID = debitadj.TranPeriodID;
							debitadj.AdjFinPeriodID = debitadj.FinPeriodID;
							debitadj.Printed = true;
                            APAddressAttribute.DefaultRecord<APPayment.remitAddressID>(APPayment_DocType_RefNbr.Cache, debitadj);
                            APContactAttribute.DefaultRecord<APPayment.remitContactID>(APPayment_DocType_RefNbr.Cache, debitadj);                            
							APPayment_DocType_RefNbr.Cache.Update(debitadj);
							PXTimeStampScope.DuplicatePersisted(APPayment_DocType_RefNbr.Cache, debitadj, typeof(APInvoice));
							APDocument.Cache.SetStatus(doc, PXEntryStatus.Notchanged);
						}
					}
					else
					{
						if (APDocument.Cache.ObjectsEqual(doc, APPayment_DocType_RefNbr.Current))
						{
							APPayment_DocType_RefNbr.Cache.SetStatus(APPayment_DocType_RefNbr.Current, PXEntryStatus.Notchanged);
						}
					}

					this.Actions.PressSave();
                    ts.Complete(this);
				}

			}
			return ret;
		}

		private void ClosePayment(APRegister doc, APPayment apdoc, Vendor vendor)
		{
			if ((bool)apdoc.VoidAppl || doc.CuryDocBal == 0m)
			{
				doc.CuryDocBal = 0m;
				doc.DocBal = 0m;
				doc.OpenDoc = false;
				doc.ClosedFinPeriodID = doc.FinPeriodID;
				doc.ClosedTranPeriodID = doc.TranPeriodID;
				foreach (APAdjust adjmax in PXSelectGroupBy<APAdjust,
					Where<APAdjust.adjgDocType, Equal<Required<APAdjust.adjgDocType>>,
						And<APAdjust.adjgRefNbr, Equal<Required<APAdjust.adjgRefNbr>>,
						And<Where<APAdjust.released, Equal<True>,
						Or<APAdjust.adjNbr, Equal<Required<APAdjust.adjNbr>>>>>>>,
					Aggregate<GroupBy<APAdjust.adjgDocType, GroupBy<APAdjust.adjgRefNbr,
						Max<APAdjust.adjgFinPeriodID, Max<APAdjust.adjgTranPeriodID>>>>>>.Select(this, doc.DocType, doc.RefNbr, doc.LineCntr))
				{
					doc.ClosedFinPeriodID = adjmax.AdjgFinPeriodID;
					doc.ClosedTranPeriodID = adjmax.AdjgTranPeriodID;
				}

				DeactivateOneTimeVendorIfAllDocsIsClosed(vendor);
			}
			else if (apdoc.VoidAppl == false)
			{
				//do not reset ClosedPeriod for VoidCheck.
				doc.OpenDoc = true;
				doc.ClosedTranPeriodID = null;
				doc.ClosedFinPeriodID = null;
			}
		}

        public override void Persist()
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                APPayment_DocType_RefNbr.Cache.Persist(PXDBOperation.Insert);
                APPayment_DocType_RefNbr.Cache.Persist(PXDBOperation.Update);

                APDocument.Cache.Persist(PXDBOperation.Update);

                APTran_TranType_RefNbr.Cache.Persist(PXDBOperation.Update);

                APTaxTran_TranType_RefNbr.Cache.Persist(PXDBOperation.Insert);
                APTaxTran_TranType_RefNbr.Cache.Persist(PXDBOperation.Update);
				SVATConversionHistory.Cache.Persist(PXDBOperation.Insert);
				SVATConversionHistory.Cache.Persist(PXDBOperation.Update);

				APAdjust_AdjgDocType_RefNbr_VendorID.Cache.Persist(PXDBOperation.Insert);
                APAdjust_AdjgDocType_RefNbr_VendorID.Cache.Persist(PXDBOperation.Update);
                APAdjust_AdjgDocType_RefNbr_VendorID.Cache.Persist(PXDBOperation.Delete);

                Caches[typeof(APHist)].Persist(PXDBOperation.Insert);
                Caches[typeof(CuryAPHist)].Persist(PXDBOperation.Insert);

                AP1099Year_Select.Cache.Persist(PXDBOperation.Insert);
                AP1099History_Select.Cache.Persist(PXDBOperation.Insert);
                ptInstanceTrans.Cache.Persist(PXDBOperation.Update);

                CurrencyInfo_CuryInfoID.Cache.Persist(PXDBOperation.Update);

                this.poReceiptLineUPD.Cache.Persist(PXDBOperation.Update);
                this.poReceiptUPD.Cache.Persist(PXDBOperation.Update);
                this.Caches[typeof(PO.POReceiptTax)].Persist(PXDBOperation.Update);
                this.Caches[typeof(PO.POReceiptTaxTran)].Persist(PXDBOperation.Update);
                this.poOrderLineUPD.Cache.Persist(PXDBOperation.Update);
                this.poOrderUPD.Cache.Persist(PXDBOperation.Update);
                this.poVendorInventoryPriceUpdate.Cache.Persist(PXDBOperation.Insert);
                this.poVendorInventoryPriceUpdate.Cache.Persist(PXDBOperation.Update);
				this.Caches[typeof(CADailySummary)].Persist(PXDBOperation.Insert);
                
                ts.Complete(this);
            }

            APPayment_DocType_RefNbr.Cache.Persisted(false);
            APDocument.Cache.Persisted(false);
            APTran_TranType_RefNbr.Cache.Persisted(false);
            APTaxTran_TranType_RefNbr.Cache.Persisted(false);
            APAdjust_AdjgDocType_RefNbr_VendorID.Cache.Persisted(false);

            Caches[typeof(APHist)].Persisted(false);
            Caches[typeof(CuryAPHist)].Persisted(false);

            AP1099Year_Select.Cache.Persisted(false);
            AP1099History_Select.Cache.Persisted(false);

            CurrencyInfo_CuryInfoID.Cache.Persisted(false);
            ptInstanceTrans.Cache.Persisted(false);
            this.poReceiptLineUPD.Cache.Persisted(false);
            this.poReceiptUPD.Cache.Persisted(false);
            this.Caches[typeof(PO.POReceiptTax)].Persisted(false);
            this.Caches[typeof(PO.POReceiptTaxTran)].Persisted(false);
            this.poOrderLineUPD.Cache.Persisted(false);
            this.poOrderUPD.Cache.Persisted(false);
            this.poVendorInventoryPriceUpdate.Cache.Persisted(false);
			this.Caches[typeof(CADailySummary)].Persisted(false);
        }

        protected bool _IsIntegrityCheck = false;

		protected virtual int SortVendDocs(PXResult<APRegister> a, PXResult<APRegister> b)
		{
			return ((IComparable)((APRegister)a).SortOrder).CompareTo(((APRegister)b).SortOrder);
		}

		public virtual void IntegrityCheckProc(Vendor vend, string startPeriod)
		{
			_IsIntegrityCheck = true;
			JournalEntry je = PXGraph.CreateInstance<JournalEntry>();
			je.SetOffline();
			DocumentList<Batch> created = new DocumentList<Batch>(je);

			Caches[typeof(Vendor)].Current = vend;

			using (new PXConnectionScope())
			{
				using (PXTransactionScope ts = new PXTransactionScope())
				{
					string minPeriod = "190001";

					APHistory maxHist = (APHistory)PXSelectGroupBy<APHistory, Where<APHistory.vendorID, Equal<Current<Vendor.bAccountID>>, And<APHistory.detDeleted, Equal<True>>>, Aggregate<Max<APHistory.finPeriodID>>>.Select(this);

					if (maxHist != null && maxHist.FinPeriodID != null)
					{
						minPeriod = FinPeriodIDAttribute.PeriodPlusPeriod(this, maxHist.FinPeriodID, 1);
					}

					if (string.IsNullOrEmpty(startPeriod) == false && string.Compare(startPeriod, minPeriod) > 0)
					{
						minPeriod = startPeriod;
					}

					foreach (CuryAPHist old_hist in PXSelectReadonly<CuryAPHist, Where<CuryAPHist.vendorID, Equal<Required<CuryAPHist.vendorID>>, And<CuryAPHist.finPeriodID, GreaterEqual<Required<CuryAPHist.finPeriodID>>>>>.Select(this, vend.BAccountID, minPeriod))
					{
						CuryAPHist hist = new CuryAPHist();
						hist.BranchID = old_hist.BranchID;
						hist.AccountID = old_hist.AccountID;
						hist.SubID = old_hist.SubID;
						hist.VendorID = old_hist.VendorID;
						hist.FinPeriodID = old_hist.FinPeriodID;
						hist.CuryID = old_hist.CuryID;

						hist = (CuryAPHist)Caches[typeof(CuryAPHist)].Insert(hist);

						hist.FinPtdRevalued += old_hist.FinPtdRevalued;

						APHist basehist = new APHist();
						basehist.BranchID = old_hist.BranchID;
						basehist.AccountID = old_hist.AccountID;
						basehist.SubID = old_hist.SubID;
						basehist.VendorID = old_hist.VendorID;
						basehist.FinPeriodID = old_hist.FinPeriodID;

						basehist = (APHist)Caches[typeof(APHist)].Insert(basehist);

						basehist.FinPtdRevalued += old_hist.FinPtdRevalued;
					}

					PXDatabase.Delete<APHistory>(
						new PXDataFieldRestrict("VendorID", PXDbType.Int, 4, vend.BAccountID, PXComp.EQ),
						new PXDataFieldRestrict("FinPeriodID", PXDbType.Char, 6, minPeriod, PXComp.GE)
						);

					PXDatabase.Delete<CuryAPHistory>(
						new PXDataFieldRestrict("VendorID", PXDbType.Int, 4, vend.BAccountID, PXComp.EQ),
						new PXDataFieldRestrict("FinPeriodID", PXDbType.Char, 6, minPeriod, PXComp.GE)
						);

                    PXDatabase.Delete<AP1099History>(
                        new PXDataFieldRestrict("VendorID", PXDbType.Int, 4, vend.BAccountID, PXComp.EQ),
                        new PXDataFieldRestrict("FinYear", PXDbType.Char, 4, minPeriod.Substring(0, 4), PXComp.GE)
                        );

					PXRowInserting APHist_RowInserting = delegate(PXCache sender, PXRowInsertingEventArgs e)
					{
						if (string.Compare(((APHist)e.Row).FinPeriodID, minPeriod) < 0)
						{
							e.Cancel = true;
						}
					};

					PXRowInserting CuryAPHist_RowInserting = delegate(PXCache sender, PXRowInsertingEventArgs e)
					{
						if (string.Compare(((CuryAPHist)e.Row).FinPeriodID, minPeriod) < 0)
						{
							e.Cancel = true;
						}
					};

					this.RowInserting.AddHandler<APHist>(APHist_RowInserting);
					this.RowInserting.AddHandler<CuryAPHist>(CuryAPHist_RowInserting);

					PXResultset<APRegister> venddocs = PXSelect<APRegister, Where<APRegister.vendorID, Equal<Current<Vendor.bAccountID>>, And2<Where<APRegister.released, Equal<True>, Or<APRegister.prebooked, Equal<True>>>, And<Where<APRegister.finPeriodID, GreaterEqual<Required<APRegister.finPeriodID>>, Or<APRegister.closedFinPeriodID, GreaterEqual<Required<APRegister.finPeriodID>>>>>>>>.Select(this, minPeriod, minPeriod);
					PXResultset<APRegister> other = PXSelectJoinGroupBy<APRegister, 
						InnerJoin<APAdjust, On<APAdjust.adjgDocType, Equal<APRegister.docType>, And<APAdjust.adjgRefNbr, Equal<APRegister.refNbr>>>,
						InnerJoin<Standalone.APRegister2, On<Standalone.APRegister2.docType, Equal<APAdjust.adjdDocType>,
							And<Standalone.APRegister2.refNbr, Equal<APAdjust.adjdRefNbr>>>>>, 
						Where<APRegister.vendorID, Equal<Current<Vendor.bAccountID>>, 
							And2<Where<Standalone.APRegister2.closedFinPeriodID, GreaterEqual<Required<Standalone.APRegister2.closedFinPeriodID>>, 
							Or<APAdjust.adjdFinPeriodID, GreaterEqual<Required<APAdjust.adjdFinPeriodID>>,
							Or<APAdjust.adjdFinPeriodID, Less<Required<APAdjust.adjdFinPeriodID>>,
								And<Standalone.APRegister2.closedFinPeriodID, IsNull>>>>,
							And<APAdjust.released, Equal<True>, 
							And<APRegister.finPeriodID, Less<Required<APRegister.finPeriodID>>,
							And2<Where<APRegister.released, Equal<True>,
									Or<APRegister.prebooked, Equal<True>>>,
							And<Where<APRegister.closedFinPeriodID, Less<Required<APRegister.closedFinPeriodID>>, Or<APRegister.closedFinPeriodID, IsNull>>>>>>>>, 
						Aggregate<GroupBy<APRegister.docType, 
							GroupBy<APRegister.refNbr, 
							GroupBy<APRegister.createdByID, 
							GroupBy<APRegister.lastModifiedByID, 
							GroupBy<APRegister.released,
 							GroupBy<APRegister.prebooked, 
							GroupBy<APRegister.openDoc, 
							GroupBy<APRegister.hold, 
							GroupBy<APRegister.scheduled, 
							GroupBy<APRegister.voided, 
							GroupBy<APRegister.printed,
							GroupBy<APRegister.isTaxPosted,
							GroupBy<APRegister.isTaxSaved,
							GroupBy<APRegister.isTaxValid>>>>
							>>>>>>>>>>>>.Select(this, minPeriod, minPeriod, minPeriod, minPeriod, minPeriod);

					venddocs.AddRange(other);
					venddocs.Sort(SortVendDocs);

					foreach (APRegister venddoc in venddocs)
					{
						je.Clear();

						APRegister doc = venddoc;

						//mark as updated so that doc will not expire from cache and update with Released = 1 will not override balances/amount in document
						APDocument.Cache.SetStatus(doc, PXEntryStatus.Updated);

						bool prebooked = (doc.Prebooked == true);
						bool released = (doc.Released == true); //Save state of the document - prebooked & released flags will be altered during release process
						if (prebooked)
						{
							doc.Prebooked = false;
						}

						doc.Released = false;
						
						foreach (PXResult<APInvoice, CurrencyInfo, Terms, Vendor> res in APInvoice_DocType_RefNbr.Select((object)doc.DocType, doc.RefNbr))
						{
							APTran_TranType_RefNbr.StoreCached(new PXCommandKey(new object[] { doc.DocType, doc.RefNbr }), new List<object>());

							//must check for AD application in different period
							if (doc.Voided == false || doc.DocType == APDocType.QuickCheck)
							{

								if ((bool)doc.Released == false || (bool)doc.Prebooked == false)
                                {
                                    SegregateBatch(je, doc.BranchID, doc.CuryID, doc.DocDate, doc.FinPeriodID, doc.DocDesc, (CurrencyInfo)res);
								}
								ReleaseDocProc(je, ref doc, res, prebooked);								
							}
							doc.Released = released; //Restore flag
							doc.Prebooked = prebooked;
						}

						foreach (PXResult<APPayment, CurrencyInfo, Currency, Vendor, CashAccount> res in APPayment_DocType_RefNbr.Select((object)doc.DocType, doc.RefNbr, doc.VendorID))
						{
							if (doc.DocType == APDocType.DebitAdj || doc.DocType == APDocType.CreditAdj)
							{
								if (doc.Prebooked == true) continue; //We don't need payment part processing on release;
							}

                            SegregateBatch(je, doc.BranchID, doc.CuryID, ((APPayment)res).AdjDate, ((APPayment)res).AdjFinPeriodID, doc.DocDesc, (CurrencyInfo)res);

							int OrigLineCntr = (int)doc.LineCntr;
							doc.LineCntr = 0;

							while (doc.LineCntr < OrigLineCntr)
							{
								ReleaseDocProc(je, ref doc, res);
								doc.Prebooked = prebooked;
								doc.Released = released;
							}
							APAdjust reversal = APAdjust_AdjgDocType_RefNbr_VendorID.Select(doc.DocType, doc.RefNbr, OrigLineCntr);
							if (reversal != null)
							{
								doc.OpenDoc = true;
							}
						}

						APDocument.Cache.Update(doc);
					}

                    Caches[typeof(AP1099Hist)].Clear();

                    foreach (PXResult<APAdjust, APTran, APInvoice> res in PXSelectReadonly2<APAdjust, 
                        InnerJoin<APTran, On<APTran.tranType, Equal<APAdjust.adjdDocType>, And<APTran.refNbr, Equal<APAdjust.adjdRefNbr>>>,
                        InnerJoin<APInvoice, On<APInvoice.docType, Equal<APAdjust.adjdDocType>, And<APInvoice.refNbr, Equal<APAdjust.adjdRefNbr>>>>>,
					Where<APAdjust.vendorID, Equal<Required<APAdjust.vendorID>>, And<APAdjust.adjgDocDate, GreaterEqual<Required<APAdjust.adjgDocDate>>, And<APAdjust.released, Equal<True>, And<APAdjust.voided, Equal<False>, And<APTran.box1099, IsNotNull>>>>>>.Select(this, vend.BAccountID, new DateTime(Convert.ToInt32(minPeriod.Substring(0, 4)), 1, 1)))
                    {
                        APAdjust adj = res;
                        APTran tran = res;
                        APInvoice doc = res;

                        Update1099Hist(this, 1, adj, tran, doc);
                    }

					foreach (APRegister apdoc in APDocument.Cache.Updated)
					{
						if (apdoc.CuryDocBal < 0m)
						{
							//throw new PXException(Messages.DocumentBalanceNegative);
						}

						APDocument.Cache.PersistUpdated(apdoc);
					}

					this.RowInserting.RemoveHandler<APHist>(APHist_RowInserting);
					this.RowInserting.RemoveHandler<CuryAPHist>(CuryAPHist_RowInserting);

					Caches[typeof(APHist)].Persist(PXDBOperation.Insert);

					Caches[typeof(CuryAPHist)].Persist(PXDBOperation.Insert);

                    Caches[typeof(AP1099Hist)].Persist(PXDBOperation.Insert);

					ts.Complete(this);
				}
				APDocument.Cache.Persisted(false);

				Caches[typeof(APHist)].Persisted(false);

				Caches[typeof(CuryAPHist)].Persisted(false);

                Caches[typeof(AP1099Hist)].Persisted(false);
			}
		}

		protected virtual void RetrievePPVAccount(PXGraph aOpGraph, PO.POReceiptLineR1 aLine, ref int? aPPVAcctID, ref int? aPPVSubID) 
		{
			aPPVAcctID = null;
			aPPVSubID = null;
			PXResult<PO.POReceiptLine, IN.InventoryItem, IN.INPostClass, INSite> res = (PXResult<PO.POReceiptLine, IN.InventoryItem, IN.INPostClass, INSite>)
							PXSelectJoin<PO.POReceiptLine, InnerJoin<InventoryItem, On<PO.POReceiptLine.inventoryID,Equal<InventoryItem.inventoryID>>,
													InnerJoin<IN.INPostClass, On<IN.INPostClass.postClassID, Equal<IN.InventoryItem.postClassID>>,
													InnerJoin<IN.INSite, On<IN.INSite.siteID,Equal<PO.POReceiptLine.siteID>>>>>,
														Where<PO.POReceiptLine.receiptNbr,Equal<Required<PO.POReceiptLine.receiptNbr>>, 
													And<PO.POReceiptLine.lineNbr,Equal<Required<PO.POReceiptLine.lineNbr>>>>>.Select(this,aLine.ReceiptNbr,aLine.LineNbr);
			if (res != null )
			{
				InventoryItem  invItem = (InventoryItem)res;
				INPostClass postClass = (INPostClass)res;
				INSite invSite = (INSite)res;
				aPPVAcctID = INReleaseProcess.GetAcctID<INPostClass.pPVAcctID>(aOpGraph, postClass.PPVAcctDefault, invItem, invSite, postClass);
				try
				{
					aPPVSubID = INReleaseProcess.GetSubID<INPostClass.pPVSubID>(aOpGraph, postClass.PPVAcctDefault, postClass.PPVSubMask, invItem, invSite, postClass);
				}
				catch (PXException) 
				{
					throw new PXException(Messages.PPVSubAccountMaskCanNotBeAssembled);
				}
			}
		}

		protected virtual void POReceipt_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			POReceipt row = e.Row as POReceipt;
			POReceipt oldRow = e.OldRow as POReceipt;
			if (row != null && oldRow != null && row.UnbilledQty != oldRow.UnbilledQty)
			{
				row.IsUnbilledTaxValid = false;
			}
		}

		public virtual void VoidDocProc(JournalEntry je, APRegister doc)
		{
			if (doc.Released == true && doc.Prebooked == true)
			{
				throw new PXException(Messages.PrebookedDocumentsMayNotBeVoidedAfterTheyAreReleased);
			}

			if (doc.Prebooked == true && string.IsNullOrEmpty(doc.PrebookBatchNbr))
			{
				throw new PXException(Messages.LinkToThePrebookingBatchIsMissingVoidImpossible,doc.DocType, doc.RefNbr);
			}

			APAdjust adjustment = PXSelectReadonly<APAdjust, Where<APAdjust.adjdDocType, Equal<Required<APAdjust.adjdDocType>>, 
								And<APAdjust.adjdRefNbr,Equal<Required<APAdjust.adjdRefNbr>>>>>.Select(this, doc.DocType, doc.RefNbr);
			if (adjustment != null && string.IsNullOrEmpty(adjustment.AdjgRefNbr) == false)
			{
				throw new PXException(Messages.PrebookedDocumentMayNotBeVoidedIfPaymentsWereAppliedToIt);
			}


			APTran tran = PXSelectReadonly<APTran, Where<APTran.tranType, Equal<Required<APTran.tranType>>, And<APTran.refNbr, Equal<Required<APTran.refNbr>>,
											And<Where<APTran.lCTranID, IsNotNull, Or<APTran.pONbr, IsNotNull, Or<APTran.receiptNbr, IsNotNull>>>>>>>.Select(this, doc.DocType, doc.RefNbr);
			if (tran != null && !string.IsNullOrEmpty(tran.RefNbr)) 
			{
				throw new PXException(Messages.ThisDocumentConatinsTransactionsLinkToPOVoidIsNotPossible);
			}

			APTaxTran reportedTaxTran = PXSelectReadonly<APTaxTran, Where<APTaxTran.tranType, Equal<Required<APTaxTran.tranType>>, And<APTaxTran.refNbr, Equal<Required<APTaxTran.refNbr>>, And<APTaxTran.taxPeriodID, IsNotNull>>>>.Select(this, doc.DocType,doc.RefNbr);
			if (reportedTaxTran != null && string.IsNullOrEmpty(reportedTaxTran.TaxID) == false) 
			{
				throw new PXException(Messages.TaxesForThisDocumentHaveBeenReportedVoidIsNotPossible);
			}
			//using (new PXConnectionScope())
			{
				using (PXTransactionScope ts = new PXTransactionScope())
				{
					//mark as updated so that doc will not expire from cache and update with Released = 1 will not override balances/amount in document
					APDocument.Cache.SetStatus(doc, PXEntryStatus.Updated);
					string batchNbr = doc.Prebooked == true ? doc.PrebookBatchNbr : doc.BatchNbr;
					GL.Batch batch = PXSelectReadonly<GL.Batch,
								Where<GL.Batch.module, Equal<GL.BatchModule.moduleAP>, And<GL.Batch.batchNbr, Equal<Required<GL.Batch.batchNbr>>>>>.Select(this, batchNbr);
					if (batch == null && string.IsNullOrEmpty(batch.BatchNbr))
					{
						throw new PXException(Messages.PrebookingBatchDoesNotExistsInTheSystemVoidImpossible, GL.BatchModule.AP, doc.PrebookBatchNbr);
					}

					je.ReverseDocumentBatch(batch);
					if (doc.OpenDoc == true)
					{
						GLTran apTran= CreateGLTranAP(doc, true);
						GLTran apTranActual = null;
						foreach (GLTran iTran in je.GLTranModuleBatNbr.Select())
						{
							if (apTranActual == null 
									&& iTran.AccountID == apTran.AccountID && iTran.SubID == apTran.SubID && iTran.ReferenceID == apTran.ReferenceID
									&& iTran.TranType == apTran.TranType && iTran.RefNbr == apTran.RefNbr 
									&& iTran.ReferenceID == apTran.ReferenceID
									&& iTran.BranchID == apTran.BranchID && iTran.CuryCreditAmt == iTran.CuryCreditAmt && iTran.CuryDebitAmt == iTran.CuryDebitAmt ) //Detect AP Tran
							{
								apTranActual = iTran;								
							}
							iTran.Released = true;
						}
						if (apTranActual != null)
						{
							UpdateHistory(apTranActual, doc.VendorID);
							UpdateHistory(apTranActual, doc.VendorID, doc.CuryID);
						}
						else 
						{
							throw new PXException(Messages.APTransactionIsNotFoundInTheReversingBatch);
						}
					}

					foreach (APTaxTran iTaxTran in this.APTaxTran_TranType_RefNbr.Select(doc.DocType, doc.RefNbr))
					{
						PXCache taxCache = this.APTaxTran_TranType_RefNbr.Cache;
						APTaxTran copy = (APTaxTran) taxCache.CreateCopy(iTaxTran);
						copy.Voided = true;
						this.APTaxTran_TranType_RefNbr.Update(copy);
					}

					if (je.GLTranModuleBatNbr.Cache.IsInsertedUpdatedDeleted)
					{
						je.Persist();
					}

					//leave BatchNbr empty for Prepayment Requests
					if (!je.BatchModule.Cache.IsDirty && string.IsNullOrEmpty(doc.VoidBatchNbr) && (APInvoice_DocType_RefNbr.Current == null || APInvoice_DocType_RefNbr.Current.DocType != APDocType.Prepayment))
					{
						doc.VoidBatchNbr = ((Batch)je.BatchModule.Current).BatchNbr;
					}
					doc.OpenDoc = false;
					doc.Voided = true;
					doc.CuryDocBal = Decimal.Zero;
					doc.DocBal = Decimal.Zero;
					doc = (APRegister)APDocument.Cache.Update(doc);

					PXTimeStampScope.DuplicatePersisted(APDocument.Cache, doc, typeof(APInvoice));
					PXTimeStampScope.DuplicatePersisted(APDocument.Cache, doc, typeof(APPayment));

					if (doc.DocType != APDocType.DebitAdj)
					{
						if (APDocument.Cache.ObjectsEqual(doc, APPayment_DocType_RefNbr.Current))
						{
							APPayment_DocType_RefNbr.Cache.SetStatus(APPayment_DocType_RefNbr.Current, PXEntryStatus.Notchanged);
						}
					}
					//this.Persist();
					//APPayment_DocType_RefNbr.Cache.Persist(PXDBOperation.Insert);
					//APPayment_DocType_RefNbr.Cache.Persist(PXDBOperation.Update); 
					APDocument.Cache.Persist(PXDBOperation.Update);
					APTran_TranType_RefNbr.Cache.Persist(PXDBOperation.Update);
					APTaxTran_TranType_RefNbr.Cache.Persist(PXDBOperation.Update);

					Caches[typeof(APHist)].Persist(PXDBOperation.Insert);
					Caches[typeof(CuryAPHist)].Persist(PXDBOperation.Insert);

					AP1099Year_Select.Cache.Persist(PXDBOperation.Insert);
					AP1099History_Select.Cache.Persist(PXDBOperation.Insert);
					ptInstanceTrans.Cache.Persist(PXDBOperation.Update);

					CurrencyInfo_CuryInfoID.Cache.Persist(PXDBOperation.Update);					
					ts.Complete(this);
				}

				APPayment_DocType_RefNbr.Cache.Persisted(false);
				APDocument.Cache.Persisted(false);
				APTran_TranType_RefNbr.Cache.Persisted(false);
				APTaxTran_TranType_RefNbr.Cache.Persisted(false);
				APAdjust_AdjgDocType_RefNbr_VendorID.Cache.Persisted(false);

				Caches[typeof(APHist)].Persisted(false);
				Caches[typeof(CuryAPHist)].Persisted(false);

				AP1099Year_Select.Cache.Persisted(false);
				AP1099History_Select.Cache.Persisted(false);

				CurrencyInfo_CuryInfoID.Cache.Persisted(false);				
			}
		}

		protected static GLTran CreateGLTranAP(APRegister doc, bool aReversed)
		{
			GLTran tran = new GLTran();
			tran.SummPost = true;
			tran.BranchID = doc.BranchID;
			tran.AccountID = doc.APAccountID;
			tran.SubID = doc.APSubID;
			tran.ReclassificationProhibited = true;
			bool isDebit = APInvoiceType.DrCr(doc.DocType) == DrCr.Debit;			
			tran.CuryDebitAmt = (isDebit && !aReversed) ? 0m : doc.CuryOrigDocAmt;
			tran.DebitAmt = (isDebit && !aReversed) ? 0m : doc.OrigDocAmt - doc.RGOLAmt;
			tran.CuryCreditAmt = (isDebit && !aReversed) ? doc.CuryOrigDocAmt : 0m;
			tran.CreditAmt = (isDebit && !aReversed) ? doc.OrigDocAmt - doc.RGOLAmt : 0m;

			tran.TranType = doc.DocType;
			tran.TranClass = doc.DocClass;
			tran.RefNbr = doc.RefNbr;
			tran.TranDesc = doc.DocDesc;
			tran.TranPeriodID = doc.TranPeriodID;
			tran.FinPeriodID = doc.FinPeriodID;
			tran.TranDate = doc.DocDate;			
			tran.ReferenceID = doc.VendorID;
			return tran;
		}



	}
}

namespace PX.Objects.AP.Overrides.APDocumentRelease
{
	[PXAccumulator(SingleRecord = true)]
    [Serializable]
	public partial class AP1099Yr : AP1099Year
	{
		#region FinYear
		public new abstract class finYear : PX.Data.IBqlField
		{
		}
		[PXDBString(4, IsKey = true, IsFixed = true)]
		[PXDefault()]
		public override String FinYear
		{
			get
			{
				return this._FinYear;
			}
			set
			{
				this._FinYear = value;
			}
		}
		#endregion
	}

	[PXAccumulator(SingleRecord = true)]
    [Serializable]
	public partial class AP1099Hist : AP1099History
	{
		#region BranchID
		public new abstract class branchID : PX.Data.IBqlField
		{
		}
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public override Int32? BranchID
		{
			get
			{
				return this._BranchID;
			}
			set
			{
				this._BranchID = value;
			}
		}
		#endregion
		#region VendorID
		public new abstract class vendorID : PX.Data.IBqlField
		{
		}
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public override Int32? VendorID
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
		#region FinYear
		public new abstract class finYear : PX.Data.IBqlField
		{
		}
		[PXDBString(4, IsKey = true, IsFixed = true)]
		[PXSelector(typeof(Search<AP1099Year.finYear, Where<AP1099Year.status, Equal<AP1099Year.status.open>>>), DirtyRead = true)]
		[PXDefault()]
		public override String FinYear
		{
			get
			{
				return this._FinYear;
			}
			set
			{
				this._FinYear = value;
			}
		}
		#endregion
		#region BoxNbr
		public new abstract class boxNbr : PX.Data.IBqlField
		{
		}
		[PXDBShort(IsKey = true)]
		[PXSelector(typeof(Search<AP1099Box.boxNbr>))]
		[PXDefault()]
		public override Int16? BoxNbr
		{
			get
			{
				return this._BoxNbr;
			}
			set
			{
				this._BoxNbr = value;
			}
		}
		#endregion
	}

	public interface IBaseAPHist
	{
		Boolean? DetDeleted
		{
			get;
			set;
		}
        Boolean? FinFlag
		{
			get;
			set;
		}
		Decimal? PtdCrAdjustments
		{
			get;
			set;
		}
		Decimal? PtdDrAdjustments
		{
			get;
			set;
		}
		Decimal? PtdPurchases
		{
			get;
			set;
		}
		Decimal? PtdPayments
		{
			get;
			set;
		}
		Decimal? PtdDiscTaken
		{
			get;
			set;
		}
		Decimal? PtdWhTax
		{
			get;
			set;
		}
		Decimal? PtdRGOL
		{
			get;
			set;
		}
		Decimal? YtdBalance
		{
			get;
			set;
		}
		Decimal? BegBalance
		{
			get;
			set;
		}
		Decimal? PtdDeposits
		{
			get;
			set;
		}
		Decimal? YtdDeposits
		{
			get;
			set;
		}
	}

	public interface ICuryAPHist
	{
		Decimal? CuryPtdCrAdjustments
		{
			get;
			set;
		}
		Decimal? CuryPtdDrAdjustments
		{
			get;
			set;
		}
		Decimal? CuryPtdPurchases
		{
			get;
			set;
		}
		Decimal? CuryPtdPayments
		{
			get;
			set;
		}
		Decimal? CuryPtdDiscTaken
		{
			get;
			set;
		}
		Decimal? CuryPtdWhTax
		{
			get;
			set;
		}
		Decimal? CuryYtdBalance
		{
			get;
			set;
		}
		Decimal? CuryBegBalance
		{
			get;
			set;
		}
		Decimal? CuryPtdDeposits
		{
			get;
			set;
		}
		Decimal? CuryYtdDeposits
		{
			get;
			set;
		}
	}

	[PXAccumulator(new Type[] {
				typeof(CuryAPHistory.finYtdBalance),
				typeof(CuryAPHistory.tranYtdBalance),
				typeof(CuryAPHistory.curyFinYtdBalance),
				typeof(CuryAPHistory.curyTranYtdBalance),
				typeof(CuryAPHistory.finYtdBalance),
				typeof(CuryAPHistory.tranYtdBalance),
				typeof(CuryAPHistory.curyFinYtdBalance),
				typeof(CuryAPHistory.curyTranYtdBalance),
				typeof(CuryAPHistory.finYtdDeposits),
				typeof(CuryAPHistory.tranYtdDeposits),
				typeof(CuryAPHistory.curyFinYtdDeposits),
				typeof(CuryAPHistory.curyTranYtdDeposits)
				},
					new Type[] {
				typeof(CuryAPHistory.finBegBalance),
				typeof(CuryAPHistory.tranBegBalance),
				typeof(CuryAPHistory.curyFinBegBalance),
				typeof(CuryAPHistory.curyTranBegBalance),
				typeof(CuryAPHistory.finYtdBalance),
				typeof(CuryAPHistory.tranYtdBalance),
				typeof(CuryAPHistory.curyFinYtdBalance),
				typeof(CuryAPHistory.curyTranYtdBalance),
				typeof(CuryAPHistory.finYtdDeposits),
				typeof(CuryAPHistory.tranYtdDeposits),
				typeof(CuryAPHistory.curyFinYtdDeposits),
				typeof(CuryAPHistory.curyTranYtdDeposits)
				}
			)]
    [Serializable]
	public partial class CuryAPHist : CuryAPHistory, ICuryAPHist, IBaseAPHist
	{
		#region BranchID
		public new abstract class branchID : PX.Data.IBqlField
		{
		}
		[PXDBInt(IsKey = true)]
		public override Int32? BranchID
		{
			get
			{
				return this._BranchID;
			}
			set
			{
				this._BranchID = value;
			}
		}
		#endregion
		#region AccountID
		public new abstract class accountID : PX.Data.IBqlField
		{
		}
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public override Int32? AccountID
		{
			get
			{
				return this._AccountID;
			}
			set
			{
				this._AccountID = value;
			}
		}
		#endregion
		#region SubID
		public new abstract class subID : PX.Data.IBqlField
		{
		}
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public override Int32? SubID
		{
			get
			{
				return this._SubID;
			}
			set
			{
				this._SubID = value;
			}
		}
		#endregion
		#region VendorID
		public new abstract class vendorID : PX.Data.IBqlField
		{
		}
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public override Int32? VendorID
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
		#region CuryID
		public new abstract class curyID : PX.Data.IBqlField
		{
		}
		[PXDBString(5, IsUnicode = true, IsKey=true, InputMask = ">LLLLL")]
		[PXDefault()]
		public override String CuryID
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
		#region FinPeriodID
		public new abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		[PXDBString(6, IsKey = true, IsFixed = true)]
		[PXDefault()]
		public override String FinPeriodID
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
	}

	[PXAccumulator(new Type[] {
				typeof(APHistory.finYtdBalance),
				typeof(APHistory.tranYtdBalance),
				typeof(APHistory.finYtdBalance),
				typeof(APHistory.tranYtdBalance),
				typeof(APHistory.finYtdDeposits),
				typeof(APHistory.tranYtdDeposits)
				},
					new Type[] {
				typeof(APHistory.finBegBalance),
				typeof(APHistory.tranBegBalance),
				typeof(APHistory.finYtdBalance),
				typeof(APHistory.tranYtdBalance),
				typeof(APHistory.finYtdDeposits),
				typeof(APHistory.tranYtdDeposits)
				}
			)]
    [Serializable]
    [PXHidden]
	public partial class APHist : APHistory, IBaseAPHist 
	{
		#region BranchID
		public new abstract class branchID : PX.Data.IBqlField
		{
		}
		[PXDBInt(IsKey = true)]
		public override Int32? BranchID
		{
			get
			{
				return this._BranchID;
			}
			set
			{
				this._BranchID = value;
			}
		}
		#endregion
		#region AccountID
		public new abstract class accountID : PX.Data.IBqlField
		{
		}
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public override Int32? AccountID
		{
			get
			{
				return this._AccountID;
			}
			set
			{
				this._AccountID = value;
			}
		}
		#endregion
		#region SubID
		public new abstract class subID : PX.Data.IBqlField
		{
		}
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public override Int32? SubID
		{
			get
			{
				return this._SubID;
			}
			set
			{
				this._SubID = value;
			}
		}
		#endregion
		#region VendorID
		public new abstract class vendorID : PX.Data.IBqlField
		{
		}
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public override Int32? VendorID
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
		#region FinPeriodID
		public new abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		[PXDBString(6, IsKey = true, IsFixed = true)]
		[PXDefault()]
		public override String FinPeriodID
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
	}


}
