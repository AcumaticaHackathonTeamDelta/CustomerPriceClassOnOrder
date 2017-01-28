using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using PX.Objects.CM;


namespace PX.Objects.AR
{
	public class ARDunningLetterMaint : PXGraph<ARDunningLetterMaint>
	{
		#region Selects + Ctor
		public PXSelect<ARDunningLetter> Document;
		public PXSelect<Customer, Where<Customer.bAccountID, Equal<Current<ARDunningLetter.bAccountID>>>> CurrentCustomer;
		public PXSelectJoin<ARDunningLetterDetail, LeftJoin<ARDunningLetter, On<ARDunningLetter.dunningLetterID, Equal<ARDunningLetterDetail.dunningLetterID>>>,
			Where<ARDunningLetterDetail.dunningLetterID, Equal<Current<ARDunningLetter.dunningLetterID>>>> Details;

		public PXSave<ARDunningLetter> Save;
		public PXCancel<ARDunningLetter> Cancel;
		public PXDelete<ARDunningLetter> Delete;

		public ARDunningLetterMaint()
		{
			Details.AllowUpdate = false;
			Details.AllowInsert = false;
			CurrentCustomer.AllowUpdate = false;
			foreach (ARDunningLetterDetail detail in Details.Select())
			{
				ARDunningLetterProcess.ARInvoiceWithDL invoice = PXSelect<ARDunningLetterProcess.ARInvoiceWithDL, Where<ARDunningLetterProcess.ARInvoiceWithDL.refNbr, Equal<Required<ARInvoice.refNbr>>, And<ARDunningLetterProcess.ARInvoiceWithDL.docType, Equal<Required<ARInvoice.docType>>>>>.Select(this, detail.RefNbr, detail.DocType);
				if (invoice != null && invoice.DunningLetterLevel > detail.DunningLetterLevel)
				{
					VoidLetter.SetEnabled(false);
					break;
				}
			}
		}
		#endregion
		#region Events
		[PXCustomizeBaseAttribute(typeof(CustomerRawAttribute), "DisplayName", "Customer")]
		protected virtual void Customer_AcctCD_CacheAttached(PXCache sender)
		{
		}
		protected virtual void ARDunningLetter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			ARDunningLetter row = (ARDunningLetter)e.Row;
			bool released = row.Released == true;
			bool voided = row.Voided == true;
			sender.AllowDelete = !released;
			Details.AllowDelete = !released;
			VoidLetter.SetEnabled(released && !voided);
			PrintLetter.SetEnabled(released && !voided);
			Revoke.SetEnabled(!released);
			Release.SetEnabled(!released);
		}
		protected virtual void ARDunningLetterDetail_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			int maxLevel = 0;
			foreach (ARDunningLetterDetail detail in Details.Select())
			{
				maxLevel = Math.Max(detail.DunningLetterLevel ?? 0, maxLevel);
			}
			Document.Current.DunningLetterLevel = maxLevel;
		}
		#endregion
		#region Actions
		public PXAction<ARDunningLetter> ViewDocument;
		[PXUIField(DisplayName = "View Invoice", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable viewDocument(PXAdapter adapter)
		{
			if (Details.Current != null)
			{
				ARInvoice doc = PXSelect<ARInvoice, Where<ARInvoice.refNbr, Equal<Required<ARDunningLetterDetail.refNbr>>, And<ARInvoice.docType, Equal<Required<ARDunningLetterDetail.docType>>>>>.Select(this, Details.Current.RefNbr, Details.Current.DocType);
				if (doc != null)
				{
					ARInvoiceEntry graph = PXGraph.CreateInstance<ARInvoiceEntry>();
					graph.Document.Current = doc;
					PXRedirectHelper.TryRedirect(graph, PXRedirectHelper.WindowMode.NewWindow);
				}
			}
			return adapter.Get();
		}
		public PXAction<ARDunningLetter> Release;
		[PXUIField(DisplayName = "Release", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXProcessButton]
		public virtual IEnumerable release(PXAdapter adapter)
		{
			if (Document.Current != null && Document.Current.Released == false && Document.Current.Voided == false)
			{
				Save.Press();
				PXLongOperation.StartOperation(this, delegate() { ReleaseProcess(Document.Current); });
			}
			return adapter.Get();
		}
		public PXAction<ARDunningLetter> VoidLetter;
		[PXUIField(DisplayName = "Void", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXProcessButton]
		public virtual IEnumerable voidLetter(PXAdapter adapter)
		{
			if (Document.Current != null && Document.Current.Released == true && Document.Current.Voided == false)
			{
				Save.Press();
				PXLongOperation.StartOperation(this, delegate() { VoidProcess(Document.Current); });
			}
			return adapter.Get();
		}

		public PXAction<ARDunningLetter> PrintLetter;
		[PXUIField(DisplayName = "Print", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable printLetter(PXAdapter adapter)
		{
			if (Document.Current != null && Document.Current.Released == true && Document.Current.Voided == false)
			{
				Document.Current.Printed = true;
				Save.Press(); 
				
				int? L = Document.Current.DunningLetterID;
				Dictionary<string, string> d = new Dictionary<string, string>();
				d["ARDunningLetter.DunningLetterID"] = L.ToString();
				PXReportRequiredException ex = null;
				ex = PXReportRequiredException.CombineReport(ex, ARDunningLetterPrint.GetCustomerReportID(this, "AR661000", Document.Current.BAccountID, Document.Current.BranchID), d);
				throw ex;
			}
			return adapter.Get();
		}
		public PXAction<ARDunningLetter> Revoke;
		[PXUIField(DisplayName = "Revoke", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable revoke(PXAdapter adapter)
		{
			if (Details.Current != null && Document.Current != null && Document.Current.Released == false && Document.Current.Voided == false)
			{
				ARInvoice doc = PXSelect<ARInvoice, Where<ARInvoice.refNbr, Equal<Required<ARDunningLetterDetail.refNbr>>, And<ARInvoice.docType, Equal<Required<ARDunningLetterDetail.docType>>>>>.Select(this, Details.Current.RefNbr, Details.Current.DocType);
				if (doc != null)
				{
					ARInvoiceEntry graph = PXGraph.CreateInstance<ARInvoiceEntry>();
					graph.Document.Current = doc;
					doc.Revoked = true;
					graph.Document.Update(doc);
					graph.Save.Press();
					Details.Delete(Details.Current);
					Save.Press();
				}
			}
			return adapter.Get();
		}
		#endregion
		#region Processing
		private static void VoidProcess(ARDunningLetter doc)
		{
			ARDunningLetterMaint graph = PXGraph.CreateInstance<ARDunningLetterMaint>();
			graph.Document.Current = doc;
			graph.Details.AllowUpdate = true;
			foreach (ARDunningLetterDetail detail in graph.Details.Select())
			{
				ARDunningLetterProcess.ARInvoiceWithDL invoice = PXSelect<ARDunningLetterProcess.ARInvoiceWithDL, Where<ARDunningLetterProcess.ARInvoiceWithDL.refNbr, Equal<Required<ARInvoice.refNbr>>, And<ARDunningLetterProcess.ARInvoiceWithDL.docType, Equal<Required<ARInvoice.docType>>>>>.Select(graph, detail.RefNbr, detail.DocType);
				if (invoice != null && invoice.DunningLetterLevel > detail.DunningLetterLevel)
				{
					throw new PXException(Messages.DunningLetterHigherLevelExists);
				}
			}
			ARInvoice feeInvoice = PXSelect<ARInvoice, Where<ARInvoice.refNbr, Equal<Required<ARInvoice.refNbr>>, And<ARInvoice.docType, Equal<Required<ARInvoice.docType>>>>>.Select(graph, doc.FeeRefNbr, doc.FeeDocType);
			if (feeInvoice != null && feeInvoice.Voided == false)
			{
				ARInvoiceEntry invoiceGraph = PXGraph.CreateInstance<ARInvoiceEntry>();
				invoiceGraph.Document.Current = feeInvoice;
				if (feeInvoice.Released == false)
				{
					invoiceGraph.Delete.Press();
				}
				else if (feeInvoice.Released == true && feeInvoice.OpenDoc == true)
				{
					if (feeInvoice.CuryOrigDocAmt != feeInvoice.CuryDocBal)
					{
						throw new PXException(Messages.DunningLetterHavePaidFee);
					}
					invoiceGraph.reverseInvoice.Press();
					invoiceGraph.release.Press();
					ARPaymentEntry paymentGraph = PXGraph.CreateInstance<ARPaymentEntry>();
					paymentGraph.Document.Current = PXSelect<ARPayment, Where<ARPayment.refNbr, Equal<Required<ARPayment.refNbr>>, And<ARPayment.docType, Equal<Required<ARPayment.docType>>>>>.Select(paymentGraph, invoiceGraph.Document.Current.RefNbr, invoiceGraph.Document.Current.DocType);
					ARAdjust adj = new ARAdjust();
					paymentGraph.Adjustments.Insert(adj);
					adj.AdjdRefNbr = feeInvoice.RefNbr;
					adj.AdjdDocType = feeInvoice.DocType;
					paymentGraph.Adjustments.Update(adj);
					paymentGraph.release.Press();
				}
				else
				{
					throw new PXException(Messages.DunningLetterHavePaidFee);
				}
			}
			foreach (ARDunningLetterDetail detail in graph.Details.Select())
			{
				detail.Voided = true;
				graph.Details.Update(detail);
			}

			doc.Voided = true;
			graph.Document.Update(doc);
			graph.Save.Press();
		}
		public static void ReleaseProcess(ARDunningLetter doc)
		{
			ARDunningLetterMaint graph = PXGraph.CreateInstance<ARDunningLetterMaint>();
			ReleaseProcess(graph, doc);
		}
		public static void ReleaseProcess(ARDunningLetterMaint graph, ARDunningLetter doc)
		{
			if (doc != null && doc.Released != true && doc.Voided != true)
			{
				graph.Document.Current = doc;
				doc.DunningLetterLevel = 0;
				foreach (ARDunningLetterDetail detail in graph.Details.Select())
				{
					doc.DunningLetterLevel = Math.Max(doc.DunningLetterLevel ?? 0, detail.DunningLetterLevel ?? 0);
				}

				if (doc.DunningLetterLevel == 0)
				{
					throw new PXException(Messages.DunningLetterZeroLevel);
				}

				ARSetup setup = PXSelect<ARSetup>.Select(graph);
				ARDunningSetup dunningSetup = PXSelect<ARDunningSetup, Where<ARDunningSetup.dunningLetterLevel, Equal<Required<ARDunningLetter.dunningLetterLevel>>>>.Select(graph, doc.DunningLetterLevel);
				ARInvoiceEntry invGraph = PXGraph.CreateInstance<ARInvoiceEntry>();
				if (dunningSetup.DunningFee.HasValue && dunningSetup.DunningFee != 0m)
				{
					if (setup.DunningFeeInventoryID == null)
					{
						throw new PXException(Messages.DunningLetterEmptyInventory);
					}

					Customer customer = graph.CurrentCustomer.Select();
					ARInvoice feeInvoice = new ARInvoice
					{
						Released = false,
						Hold = false,
						Voided = false,
						BranchID = doc.BranchID,
						DocDate = doc.DunningLetterDate,
						CustomerID = doc.BAccountID
					};

					invGraph.Document.Current = feeInvoice;
					feeInvoice.CustomerLocationID = customer.DefLocationID;
					invGraph.Document.Update(feeInvoice);
					
					feeInvoice.CuryID = customer.AllowOverrideCury == false && customer.CuryID != null
						? customer.CuryID
						: ((GL.Company)PXSelect<GL.Company>.Select(invGraph)).BaseCuryID;
					invGraph.Document.SetValueExt<ARInvoice.curyID>(invGraph.Document.Current, feeInvoice.CuryID);
					invGraph.Document.Update(feeInvoice);
					
					feeInvoice.DocDesc = Messages.DunningLetterFee;
					feeInvoice.Hold = false;
					invGraph.Document.Update(feeInvoice);

					decimal curyVal;
					var curyInfo = invGraph.currencyinfo.Select();
					PXCurrencyAttribute.PXCurrencyHelper.CuryConvCury(invGraph.Caches[typeof(CurrencyInfo)], curyInfo, dunningSetup.DunningFee ?? 0m, out curyVal);

					IN.InventoryItem item = PXSelect<IN.InventoryItem, 
						Where<IN.InventoryItem.inventoryID, Equal<Required<ARSetup.dunningFeeInventoryID>>>>.Select(graph, setup.DunningFeeInventoryID);
					if (item == null)
					{
						throw new PXException(Messages.DunningLetterEmptyInventory);
					}

					if (item.SalesAcctID == null)
					{
						throw new PXException(Messages.DunningProcessFeeEmptySalesAccount);
					}

					ARTran detail = new ARTran
					{
						BranchID = doc.BranchID,
						Qty = 1,
						CuryUnitPrice = curyVal,
						InventoryID = setup.DunningFeeInventoryID,
						AccountID = item.SalesAcctID,
						SubID = item.SalesSubID
					};
					invGraph.Transactions.Insert(detail);

					feeInvoice = PXCache<ARInvoice>.CreateCopy(invGraph.Document.Current);
					feeInvoice.OrigDocAmt = feeInvoice.DocBal;
					feeInvoice.CuryOrigDocAmt = feeInvoice.CuryDocBal;
					invGraph.Document.Update(feeInvoice);
					invGraph.Save.Press();
					
					if (setup.AutoReleaseDunningFee == true)
					{
						invGraph.release.Press();
					}
					doc.FeeRefNbr = invGraph.Document.Current.RefNbr;
					doc.FeeDocType = invGraph.Document.Current.DocType;
				}

				graph.Details.AllowUpdate = true;
				foreach (ARDunningLetterDetail detail in graph.Details.Select())
				{
					detail.Released = true;
					graph.Details.Update(detail);
				}

				doc.Released = true;
				graph.Document.Update(doc);
				graph.Save.Press();
			}
		}
		#endregion


	}
}
