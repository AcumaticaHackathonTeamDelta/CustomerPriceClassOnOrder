using System.Collections.Generic;
using PX.Data;
using PX.Objects.CA;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.AP;
using PX.Objects.GL;
using PX.Objects.PM;
using PX.Objects.TX;
using PX.Objects.CT;

namespace PX.Objects.EP
{
	[TableDashboardType]
	public class EPDocumentRelease : PXGraph<EPDocumentRelease>
	{
		public PXCancel<EPExpenseClaim> Cancel;
		[PXFilterable]
		public PXProcessing<EPExpenseClaim, Where<EPExpenseClaim.released, Equal<boolFalse>, 
											And<EPExpenseClaim.approved, Equal<boolTrue>>>> EPDocumentList;
	   
		public EPDocumentRelease()
		{
			EPDocumentList.SetProcessDelegate(EPDocumentRelease.ReleaseDoc);
			EPDocumentList.SetProcessCaption(Messages.Release);
			EPDocumentList.SetProcessAllCaption(Messages.ReleaseAll);
			EPDocumentList.SetSelected<EPExpenseClaim.selected>();
		}

		public static void ReleaseDoc(EPExpenseClaim claim)
		{
			APInvoiceEntry docgraph = PXGraph.CreateInstance<APInvoiceEntry>();
			ExpenseClaimEntry expenseclaim = PXGraph.CreateInstance<ExpenseClaimEntry>();
			//if(claim.FinPeriodID == null)
			//	throw new PXException(Messages.ReleaseClaimWithoutFinPeriod);
			//docgraph.FieldVerifying.AddHandler<APInvoice.vendorLocationID>(APInterceptor);

			EPEmployee employee = PXSelect<EPEmployee, Where<EPEmployee.bAccountID, Equal<Required<EPExpenseClaim.employeeID>>>>.Select(docgraph, claim.EmployeeID);
			Location emplocation = PXSelect<Location, Where<Location.bAccountID, Equal<Required<EPExpenseClaim.employeeID>>, And<Location.locationID, Equal<Required<EPExpenseClaim.locationID>>>>>.Select(docgraph, claim.EmployeeID, claim.LocationID);
			EPSetup epsetup = PXSelectReadonly<EPSetup>.Select(docgraph);
			APSetup apsetup = PXSelectReadonly<APSetup>.Select(docgraph);
		   
			docgraph.vendor.Current = employee;
			docgraph.location.Current = emplocation;
			
			CurrencyInfo infoOriginal = PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Required<EPExpenseClaim.curyInfoID>>>>.Select(docgraph, claim.CuryInfoID);
		    CurrencyInfo info = PXCache<CurrencyInfo>.CreateCopy(infoOriginal);

			info.CuryInfoID = null;
			info = docgraph.currencyinfo.Insert(info);
			APInvoice invoice = new APInvoice();
			decimal signOperation;
			if (claim.CuryDocBal < 0)
			{
				invoice.DocType = APInvoiceType.DebitAdj;
				signOperation = -1;
			}
			else
			{
				invoice.DocType = APInvoiceType.Invoice;
				signOperation = 1;
			}

			invoice.CuryInfoID = info.CuryInfoID;
			
			invoice.Hold = true;
			invoice.Released = false;
			invoice.Printed = false;
			invoice.OpenDoc = true;

			invoice.DocDate = claim.DocDate;
			invoice.FinPeriodID = claim.FinPeriodID;
			invoice.InvoiceNbr = claim.RefNbr;
			invoice.DocDesc = claim.DocDesc;
			invoice.VendorID = claim.EmployeeID;
			invoice.CuryID = info.CuryID;
			invoice.VendorLocationID = claim.LocationID;
			invoice.APAccountID = emplocation != null ? emplocation.APAccountID : null;
			invoice.APSubID = emplocation != null ? emplocation.APSubID : null;
			invoice.TaxZoneID = claim.TaxZoneID;
			invoice.TaxCalcMode = AP.VendorClass.taxCalcMode.TaxSetting;
			invoice.BranchID = claim.BranchID;
			invoice = docgraph.Document.Insert(invoice);

			PXCache<CurrencyInfo>.RestoreCopy(info, infoOriginal);
			info.CuryInfoID = invoice.CuryInfoID;

			PXCache claimcache = docgraph.Caches[typeof(EPExpenseClaim)];
			PXCache claimdetailcache = docgraph.Caches[typeof(EPExpenseClaimDetails)];

			PXNoteAttribute.CopyNoteAndFiles(claimcache, claim, docgraph.Document.Cache, invoice, epsetup.GetCopyNoteSettings<PXModule.ap>());

			using (PXConnectionScope cs = new PXConnectionScope())
			{
				using (PXTransactionScope ts = new PXTransactionScope())
				{
					TaxAttribute.SetTaxCalc<APTran.taxCategoryID, APTaxAttribute>(docgraph.Transactions.Cache, null, TaxCalc.ManualCalc);
					
					foreach (PXResult<EPExpenseClaimDetails, Contract> res in PXSelectJoin<EPExpenseClaimDetails, 
						LeftJoin<Contract, On<EPExpenseClaimDetails.contractID, Equal<Contract.contractID>>>,
						Where<EPExpenseClaimDetails.refNbr, Equal<Required<EPExpenseClaim.refNbr>>>>.Select(docgraph, claim.RefNbr))
					{

						EPExpenseClaimDetails claimdetail = (EPExpenseClaimDetails)res;
						Contract contract = (Contract) res;

						if (claimdetail.TaskID != null)
						{
							PMTask task = PXSelect<PMTask, Where<PMTask.taskID, Equal<Required<PMTask.taskID>>>>.Select(expenseclaim, claimdetail.TaskID);
							if (task != null && !(bool)task.VisibleInAP)
								throw new PXException(PM.Messages.TaskInvisibleInModule, task.TaskCD, BatchModule.AP);
						}

						APTran tran = new APTran();
						tran.InventoryID = claimdetail.InventoryID;
						tran.TranDesc = claimdetail.TranDesc;
						if (claimdetail.CuryInfoID == claimdetail.ClaimCuryInfoID)
							tran.CuryUnitCost = claimdetail.CuryUnitCost * signOperation;
						else if(claimdetail.CuryUnitCost == null || claimdetail.CuryUnitCost == 0m)
							tran.CuryUnitCost = 0m;
						else
						{
							decimal newValue;
							CM.PXCurrencyAttribute.CuryConvCury(docgraph.Document.Cache, info, (decimal)claimdetail.UnitCost, out newValue);
							tran.CuryUnitCost = newValue * signOperation;
						}
						tran.Qty = claimdetail.Qty * signOperation;
						tran.UOM = claimdetail.UOM;						
						tran.NonBillable = claimdetail.Billable != true;
						tran.CuryLineAmt = claimdetail.ClaimCuryTranAmt * signOperation;
						tran.Date = claimdetail.ExpenseDate;

						if (contract.BaseType == PM.PMProject.ProjectBaseType.Project)
						{
							tran.ProjectID = claimdetail.ContractID;							
						}
						else
						{
							tran.ProjectID = PM.ProjectDefaultAttribute.NonProject(docgraph);
						}

						tran.TaskID = claimdetail.TaskID;
						tran.AccountID = claimdetail.ExpenseAccountID;
						tran.SubID = claimdetail.ExpenseSubID;
						tran.TaxCategoryID = claimdetail.TaxCategoryID;
						tran.BranchID = claimdetail.BranchID;
						tran = docgraph.Transactions.Insert(tran);

						PXNoteAttribute.CopyNoteAndFiles(claimdetailcache, claimdetail, docgraph.Transactions.Cache, tran, epsetup.GetCopyNoteSettings<PXModule.ap>());
		
						claimdetail.Released = true;
						expenseclaim.ExpenseClaimDetails.Update(claimdetail);
					}

					foreach (EPTaxTran tax in PXSelect<EPTaxTran, Where<EPTaxTran.refNbr, Equal<Required<EPTaxTran.refNbr>>>>.Select(docgraph,  claim.RefNbr))
					{
						APTaxTran new_aptax = new APTaxTran();
						new_aptax.TaxID = tax.TaxID;

						new_aptax = docgraph.Taxes.Insert(new_aptax);

						if (new_aptax != null)
						{
							new_aptax = PXCache<APTaxTran>.CreateCopy(new_aptax);
							new_aptax.TaxRate = tax.TaxRate;
							new_aptax.CuryTaxableAmt = tax.CuryTaxableAmt * signOperation;
							new_aptax.CuryTaxAmt = tax.CuryTaxAmt * signOperation;
							new_aptax = docgraph.Taxes.Update(new_aptax);
						}
					}

					invoice.CuryOrigDocAmt = invoice.CuryDocBal;
					invoice.CuryTaxAmt = invoice.CuryTaxTotal;
					invoice.Hold = false;
					docgraph.Document.Update(invoice);
					docgraph.Save.Press();
					claim.Status = EPExpenseClaimStatus.ReleasedStatus;
					claim.Released = true;
					claim.APRefNbr = invoice.RefNbr;
					claim.APDocType = invoice.DocType;
					expenseclaim.ExpenseClaim.Update(claim);

					#region EP History Update
					EPHistory hist = new EPHistory();
					hist.EmployeeID = invoice.VendorID;
					hist.FinPeriodID = invoice.FinPeriodID;
					hist =(EPHistory)expenseclaim.Caches[typeof(EPHistory)].Insert(hist);

					hist.FinPtdClaimed += invoice.DocBal;
					hist.FinYtdClaimed += invoice.DocBal;
					if (invoice.FinPeriodID == invoice.TranPeriodID)
					{
						hist.TranPtdClaimed += invoice.DocBal;
						hist.TranYtdClaimed += invoice.DocBal;
					}
					else
					{
						EPHistory tranhist = new EPHistory();
						tranhist.EmployeeID = invoice.VendorID;
						tranhist.FinPeriodID = invoice.TranPeriodID;
						tranhist =(EPHistory)expenseclaim.Caches[typeof(EPHistory)].Insert(tranhist);
						tranhist.TranPtdClaimed += invoice.DocBal;
						tranhist.TranYtdClaimed += invoice.DocBal;
					}
					expenseclaim.Views.Caches.Add(typeof(EPHistory));
					#endregion
					
					expenseclaim.Save.Press();

					ts.Complete();
				}
			}
			if ((bool)epsetup.AutomaticReleaseAP == true)
			{
				List<APRegister> doclist = new List<APRegister>();
				doclist.Add(docgraph.Document.Current);
				APDocumentRelease.ReleaseDoc(doclist, false);
			}
		}
	}
}
