using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.IN;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.AR;
using System.Diagnostics;

namespace PX.Objects.PM
{
	public class AutoBudgetWorkerProcess : PXGraph<AutoBudgetWorkerProcess>
	{
		#region DAC Attributes Override
		
		[PXDefault]
		[PXDBInt()]
		protected virtual void PMTran_ProjectID_CacheAttached(PXCache sender) { }

		[PXDefault]
		[PXDBInt()]
		protected virtual void PMTran_TaskID_CacheAttached(PXCache sender) { }

		[PXDefault]
		[PXDBInt()]
		protected virtual void PMTran_InventoryID_CacheAttached(PXCache sender) { }

		#endregion
		
		public PXSelect<PMTran> Transactions;
		
		public virtual List<Balance> Run(PMTask task)
		{
			if (task.BillingID == null)
				throw new PXException(Messages.BillingIDIsNotDefined);

			Dictionary<string, Balance> balances = new Dictionary<string, Balance>();

			DateTime? lastDate;
			List<PMTran> expenseTrans = CreateExpenseTransactions(task, out lastDate);
			List<long> expenseTranIds = new List<long>();
			Debug.Print("Created Expense Transactions:");
			Debug.Indent();
			foreach (PMTran tran in expenseTrans)
			{
				expenseTranIds.Add(tran.TranID.Value);
				Debug.Print("TranID:{0} AccountGroup:{1}, InventoryID={2}, Qty={3}, Amt={4}, Allocated={5}, Released={6}, Billed={7}, Date={8}", tran.TranID, AccountGroupFromID(tran.AccountGroupID), InventoryFromID(tran.InventoryID), tran.Qty, tran.Amount, tran.Allocated, tran.Released, tran.Billed, tran.Date);
			}
			Debug.Unindent();

			if (expenseTrans.Count == 0)
			{
				PXTrace.WriteError(Messages.FailedToEmulateExpenses, task.TaskCD);
				return new List<Balance>();
			}

			PMAllocator ae = PXGraph.CreateInstance<PMAllocator>();
			ae.SourceTransactions = expenseTrans;
			foreach (PMTran tran in expenseTrans)
			{
				ae.Transactions.Insert(tran);
			}
			ae.OverrideAllocationDate = lastDate;
			ae.Execute(task);
			Debug.Print("After Allocation:");
			Debug.Indent();
			foreach (PMTran tran in ae.Transactions.Cache.Inserted)
			{
				tran.Released = true;
				Transactions.Cache.Update(tran);

				if (expenseTranIds.Contains(tran.TranID.Value))
					continue;

				int inventoryID = tran.InventoryID ?? PMInventorySelectorAttribute.EmptyInventoryID;
				string key = string.Format("{0}.{1}", tran.AccountGroupID, inventoryID);

				if (!balances.ContainsKey(key))
				{
					Balance b = new Balance();
					b.AccountGroupID = tran.AccountGroupID.Value;
					b.InventoryID = inventoryID;

					balances.Add(key, b);
				}

				Debug.Print("TranID:{0} AccountGroup:{1}, InventoryID={2}, Qty={3}, Amt={4}, Allocated={5}, Released={6}, Billed={7}, Date={8}", tran.TranID, AccountGroupFromID(tran.AccountGroupID), InventoryFromID(tran.InventoryID), tran.Qty, tran.Amount, tran.Allocated, tran.Released, tran.Billed, tran.Date);
			}
			Debug.Unindent();

			DateTime billingDate = lastDate.Value.AddDays(1);

			//Get ARTrans for Bill:
			Debug.Print("Bill using the following Billing date={0}", billingDate);

			PMBillEngine engine = PXGraph.CreateInstance<PMBillEngine>();
			engine.IsEmulation = true;
			engine.FieldVerifying.AddHandler<PMTran.projectID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });
			engine.FieldVerifying.AddHandler<PMTran.taskID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });
			Debug.Print("Transactions passed to BillTask:");
			Debug.Indent();
			foreach (PMTran tran in Transactions.Cache.Cached)
			{
				if (expenseTranIds.Contains(tran.TranID.Value))
					continue;
				engine.Transactions.Insert(tran);
				Debug.Print("TranID:{0} AccountGroup:{1}, InventoryID={2}, Qty={3}, Amt={4}, Allocated={5}, Released={6}, Billed={7}, Date={8}", tran.TranID, AccountGroupFromID(tran.AccountGroupID), InventoryFromID(tran.InventoryID), tran.Qty, tran.Amount, tran.Allocated, tran.Released, tran.Billed, tran.Date);
			}
			Debug.Unindent();

			PMProject project = PXSelect<PMProject, Where<PMProject.contractID, Equal<Required<PMProject.contractID>>>>.Select(this, task.ProjectID);
			Customer customer = PXSelect<Customer, Where<Customer.bAccountID, Equal<Required<Customer.bAccountID>>>>.Select(this, project.CustomerID);

			PXSelectBase<PMBillingRule> billingRuleSelect = new PXSelect<PMBillingRule, Where<PMBillingRule.billingID, Equal<Required<PMBillingRule.billingID>>>>(this);
			List<PMBillingRule> rulesList = new List<PMBillingRule>();
			foreach (PMBillingRule rule in billingRuleSelect.Select(task.BillingID))
			{
				rulesList.Add(rule);
			}
			engine.billingRules.Add(task.BillingID, rulesList);

			List<PMBillEngine.BillingData> arTrans = engine.BillTask(project, customer, task, billingDate);
			
			//Create ARInvoice to DEFAULT Accounts and SubAccounts.
			ARInvoiceEntry invoiceEntry = PXGraph.CreateInstance<ARInvoiceEntry>();
			//Project and Task is in Planning State - thus suppress verification:
			invoiceEntry.FieldVerifying.AddHandler<ARInvoice.projectID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });
			invoiceEntry.FieldVerifying.AddHandler<ARTran.projectID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });
			invoiceEntry.FieldVerifying.AddHandler<ARTran.taskID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });
			ARInvoice invoice = new ARInvoice();
			invoice.DocType = ARDocType.Invoice;

			invoice = PXCache<ARInvoice>.CreateCopy(invoiceEntry.Document.Insert(invoice));
			invoice.DocDate = billingDate;
			invoice.CustomerID = task.CustomerID;
			invoice.CustomerLocationID = task.LocationID;
			invoice.ProjectID = task.ProjectID;
			invoice = (ARInvoice)invoiceEntry.Document.Update(invoice);

			CurrencyInfo curyinfo = (CurrencyInfo)invoiceEntry.Caches[typeof(CurrencyInfo)].Current;
			foreach (PMBillEngine.BillingData data in arTrans)
			{
				decimal curyamount;
				PXDBCurrencyAttribute.CuryConvCury(invoiceEntry.Caches[typeof(CurrencyInfo)], curyinfo, (data.Tran.UnitPrice ?? 0), out curyamount);
				data.Tran.CuryUnitPrice = curyamount;
				PXDBCurrencyAttribute.CuryConvCury(invoiceEntry.Caches[typeof(CurrencyInfo)], curyinfo, (data.Tran.ExtPrice ?? 0), out curyamount);
				data.Tran.CuryExtPrice = curyamount;
				data.Tran.CuryTranAmt = data.Tran.CuryExtPrice;
				data.Tran.FreezeManualDisc = true;
				ARTran newTran = (ARTran)invoiceEntry.Caches[typeof(ARTran)].Insert(data.Tran);
			}

			ARInvoice oldInvoice = (ARInvoice)invoiceEntry.Caches[typeof(ARInvoice)].CreateCopy(invoice);

			invoice.CuryOrigDocAmt = invoice.CuryDocBal;
			invoiceEntry.Caches[typeof(ARInvoice)].RaiseRowUpdated(invoice, oldInvoice);

			Debug.Print("AR Trans:");
			Debug.Indent();
			
			foreach (ARTran tran in invoiceEntry.Transactions.Select())
			{
				if (tran.TaskID == task.TaskID)
				{
					Debug.Print("InventoryID={0}, Qty={1}, Amt={2}", InventoryFromID(tran.InventoryID), tran.Qty, tran.TranAmt);

					Account acct = PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(invoiceEntry, tran.AccountID);
					
					if (acct.AccountGroupID == null)
						throw new PXException(Messages.FailedEmulateBilling);
					
					string key = string.Format("{0}.{1}", acct.AccountGroupID, tran.InventoryID ?? PMInventorySelectorAttribute.EmptyInventoryID);

					if (balances.ContainsKey(key))
					{
						balances[key].Amount += tran.TranAmt ?? 0;
						balances[key].Quantity += tran.Qty ?? 0;
					}
					else
					{
						Balance b = new Balance();
						b.AccountGroupID = acct.AccountGroupID.Value;
						b.InventoryID = tran.InventoryID ?? PMInventorySelectorAttribute.EmptyInventoryID;
						b.Amount = tran.TranAmt ?? 0;
						b.Quantity = tran.Qty ?? 0;
						
						balances.Add(key, b);
					}
				}
			}

			return new List<Balance>(balances.Values);
		}
		
		protected virtual List<PMTran> CreateExpenseTransactions(PMTask task, out DateTime? lastDate)
		{
			lastDate = null;

			PXSelectBase<PMProjectStatus> select = new PXSelectJoin<PMProjectStatus,
					InnerJoin<PMTask, On<PMTask.projectID, Equal<PMProjectStatus.projectID>, And<PMTask.taskID, Equal<PMProjectStatus.projectTaskID>>>,
					InnerJoin<PMAccountGroup, On<PMProjectStatus.accountGroupID, Equal<PMAccountGroup.groupID>>,
					LeftJoin<InventoryItem, On<PMProjectStatus.inventoryID, Equal<InventoryItem.inventoryID>>>>>,
					Where<PMProjectStatus.projectID, Equal<Required<PMTask.projectID>>,
					And<PMProjectStatus.projectTaskID, Equal<Required<PMTask.taskID>>,
					And<PMAccountGroup.type, Equal<AccountType.expense>>>>>(this);

			List<PMTran> trans = new List<PMTran>();

			foreach (PXResult<PMProjectStatus, PMTask, PMAccountGroup, InventoryItem> res in select.Select(task.ProjectID, task.TaskID))
			{
				PMProjectStatus ps = (PMProjectStatus)res;
				InventoryItem item = (InventoryItem)res;

				PMTran tran = new PMTran();
				tran.AccountGroupID = ps.AccountGroupID;
				tran.ProjectID = ps.ProjectID;
				tran.TaskID = ps.ProjectTaskID;
				tran.InventoryID = ps.InventoryID;
				tran.AccountID = item.InventoryID != null ? item.COGSAcctID : null;
				tran.SubID = item.InventoryID != null ? item.COGSSubID : null;
				tran.Amount = ps.Amount;
				tran.Qty = ps.Qty;
				tran.UOM = ps.UOM;
				tran.BAccountID = task.CustomerID;
				tran.LocationID = task.LocationID;
				tran.Billable = true;
				tran.UseBillableQty = true;
				tran.BillableQty = ps.RevisedQty;
				tran.Date = FinPeriodIDAttribute.PeriodEndDate(this, ps.PeriodID);
				tran.StartDate = tran.Date;
				tran.EndDate = tran.Date;
				tran.FinPeriodID = ps.PeriodID;
				tran.TranPeriodID = ps.PeriodID;
				tran.Released = true;
				
				if (lastDate != null)
				{
					if (lastDate < tran.Date)
						lastDate = tran.Date;
				}
				else
				{
					lastDate = tran.Date;
				}
				trans.Add(Transactions.Insert(tran));
			}

			return trans;
		}
		
		public override void Persist()
		{
			//this graph should not be persisted. its an emulation.
		}

		public string AccountGroupFromID(int? accountGroupID)
		{
			PMAccountGroup ag = PXSelect<PMAccountGroup, Where<PMAccountGroup.groupID, Equal<Required<PMAccountGroup.groupID>>>>.Select(this, accountGroupID);
			return ag.GroupCD;
		}

		public string InventoryFromID(int? inventoryID)
		{
			if (inventoryID == null || inventoryID == 0)
				return "<OTHER>";

			InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, inventoryID);
			return item.InventoryCD;
		}

		public class Balance
		{
			public int AccountGroupID { get; set; }
			public int InventoryID { get; set; }
			public decimal Amount { get; set; }
			public decimal Quantity { get; set; }
		
		}
		
	}

}
