using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Common;
using PX.Data;
using PX.Objects.CA;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.AR;
using PX.Objects.GL;
using PX.Objects.CT;
using PX.Objects.PM;

namespace PX.Objects.EP
{
	[TableDashboardType]
	public class EPCustomerBilling : PXGraph<EPCustomerBilling>
	{
		public PXCancel<BillingFilter> Cancel;
		public PXFilter<BillingFilter> Filter;
		[PXFilterable]
		public PXFilteredProcessing<CustomersList, BillingFilter> Customers;


		public EPCustomerBilling()
		{
			Customers.SetProcessCaption(Messages.Process);
			Customers.SetProcessAllCaption(Messages.ProcessAll);
			Customers.SetSelected<CustomersList.selected>();
		}

		protected virtual IEnumerable customers()
		{
			BillingFilter filter = Filter.Current;
			if (filter == null)
			{
				yield break;
			}
			bool found = false;
			foreach (CustomersList item in Customers.Cache.Inserted)
			{
				found = true;
				yield return item;
			}
			if (found)
			{
				yield break;
			}

			PXSelectBase<EPExpenseClaimDetails> sel = new PXSelectJoinGroupBy<EPExpenseClaimDetails, InnerJoin<Customer, On<EPExpenseClaimDetails.customerID, Equal<Customer.bAccountID>>,
																									 LeftJoin<Contract, On<EPExpenseClaimDetails.contractID, Equal<Contract.contractID>, And<Where<Contract.baseType, Equal<Contract.ContractBaseType>, Or<Contract.nonProject, Equal<True>>>>>>>,
																									 Where<EPExpenseClaimDetails.released, Equal<boolTrue>,
																									 And<EPExpenseClaimDetails.billable, Equal<boolTrue>,
																									 And<EPExpenseClaimDetails.billed, Equal<boolFalse>,
																									 And<EPExpenseClaimDetails.expenseDate, LessEqual<Current<BillingFilter.endDate>>,
																									 And<Where<EPExpenseClaimDetails.contractID, Equal<Contract.contractID>, Or<EPExpenseClaimDetails.contractID, IsNull>>>
																									 >>>>,
																									 Aggregate<GroupBy<EPExpenseClaimDetails.customerID,
																											   GroupBy<EPExpenseClaimDetails.customerLocationID>>>>(this);
			if (filter.CustomerClassID != null)
			{
				sel.WhereAnd<Where<Customer.customerClassID, Equal<Current<BillingFilter.customerClassID>>>>();
			}
			if (filter.CustomerID != null)
			{
				sel.WhereAnd<Where<Customer.bAccountID, Equal<Current<BillingFilter.customerID>>>>();
			}

			foreach (PXResult<EPExpenseClaimDetails, Customer, Contract> res in sel.Select())
			{
				CustomersList retitem = new CustomersList();
				Customer customer = res;
				EPExpenseClaimDetails claimdetaisl = res;
				retitem.CustomerID = customer.BAccountID;
				retitem.LocationID = claimdetaisl.CustomerLocationID;
				retitem.CustomerClassID = customer.CustomerClassID;

				retitem.Selected = false;

				yield return Customers.Insert(retitem);

			}
		}

		public static void Bill(EPCustomerBillingProcess docgraph, CustomersList customer, BillingFilter filter)
		{
			docgraph.Bill(customer, filter);
		}

		#region EventHandlers
		protected virtual void BillingFilter_RowUpdated(PXCache cache, PXRowUpdatedEventArgs e)
		{
			Customers.Cache.Clear();
		}

		protected virtual void BillingFilter_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			BillingFilter filter = Filter.Current;
			Customers.SetProcessDelegate<EPCustomerBillingProcess>((docgraph, customer) => docgraph.Bill(customer, filter));
		}
		#endregion


		#region Internal Types
		[Serializable]
		public partial class BillingFilter : PX.Data.IBqlTable
		{
			#region InvoiceDate
			public abstract class invoiceDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _InvoiceDate;
			[PXDBDate()]
			[PXDefault(typeof(AccessInfo.businessDate))]
			[PXUIField(DisplayName = "Invoice Date", Visibility = PXUIVisibility.Visible)]
			public virtual DateTime? InvoiceDate
			{
				get
				{
					return this._InvoiceDate;
				}
				set
				{
					this._InvoiceDate = value;
				}
			}
			#endregion
			#region InvFinPeriodID
			public abstract class invFinPeriodID : PX.Data.IBqlField
			{
			}
			protected string _InvFinPeriodID;
			[AROpenPeriod(typeof(BillingFilter.invoiceDate))]
			[PXUIField(DisplayName = "Post Period", Visibility = PXUIVisibility.Visible)]
			public virtual String InvFinPeriodID
			{
				get
				{
					return this._InvFinPeriodID;
				}
				set
				{
					this._InvFinPeriodID = value;
				}
			}
			#endregion
			#region CustomerClassID
			public abstract class customerClassID : PX.Data.IBqlField
			{
			}
			protected String _CustomerClassID;
			[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
			[PXSelector(typeof(CustomerClass.customerClassID), DescriptionField = typeof(CustomerClass.descr), CacheGlobal = true)]
			[PXUIField(DisplayName = "Customer Class")]
			public virtual String CustomerClassID
			{
				get
				{
					return this._CustomerClassID;
				}
				set
				{
					this._CustomerClassID = value;
				}
			}
			#endregion
			#region CustomerID
			public abstract class customerID : PX.Data.IBqlField
			{
			}
			protected Int32? _CustomerID;
			[Customer(DescriptionField = typeof(Customer.acctName))]
			public virtual Int32? CustomerID
			{
				get
				{
					return this._CustomerID;
				}
				set
				{
					this._CustomerID = value;
				}
			}
			#endregion
			#region EndDate
			public abstract class endDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _EndDate;
			[PXDBDate()]
			[PXDefault(typeof(AccessInfo.businessDate))]
			[PXUIField(DisplayName = "Load Claims Up to", Visibility = PXUIVisibility.Visible)]
			public virtual DateTime? EndDate
			{
				get
				{
					return this._EndDate;
				}
				set
				{
					this._EndDate = value;
				}
			}
			#endregion
		}
		

		#endregion
	}

	[Serializable]
	public partial class CustomersList : PX.Data.IBqlTable
	{
		#region Selected
		public abstract class selected : IBqlField
		{
		}
		protected bool? _Selected = false;
		[PXBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Selected")]
		public bool? Selected
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
		#region CustomerClassID
		public abstract class customerClassID : PX.Data.IBqlField
		{
		}
		protected String _CustomerClassID;
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXSelector(typeof(CustomerClass.customerClassID), DescriptionField = typeof(CustomerClass.descr), CacheGlobal = true)]
		[PXUIField(DisplayName = "Customer Class")]
		public virtual String CustomerClassID
		{
			get
			{
				return this._CustomerClassID;
			}
			set
			{
				this._CustomerClassID = value;
			}
		}
		#endregion
		#region CustomerID
		public abstract class customerID : PX.Data.IBqlField
		{
		}
		protected Int32? _CustomerID;
		[Customer(DescriptionField = typeof(Customer.acctName), IsKey = true)]
		public virtual Int32? CustomerID
		{
			get
			{
				return this._CustomerID;
			}
			set
			{
				this._CustomerID = value;
			}
		}
		#endregion
		#region LocationID
		public abstract class locationID : PX.Data.IBqlField
		{
		}
		protected Int32? _LocationID;
		[LocationID(typeof(Where<Location.bAccountID, Equal<Current<CustomersList.customerID>>>), DescriptionField = typeof(Location.descr), Visibility = PXUIVisibility.SelectorVisible, IsKey = true)]
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
		
	}

	public class EPCustomerBillingProcess : PXGraph<EPCustomerBillingProcess>
	{

		[PXDBString(15, IsUnicode = true, IsKey = true)]
		protected virtual void EPExpenseClaimDetails_RefNbr_CacheAttached(PXCache sender)
		{ }

		public PXSelect<EPExpenseClaimDetails> Transactions;
		public PXSetup<EPSetup> Setup;

		public virtual void Bill(CustomersList customer, PX.Objects.EP.EPCustomerBilling.BillingFilter filter)
		{
			ARInvoiceEntry arGraph = PXGraph.CreateInstance<ARInvoiceEntry>();
			RegisterEntry pmGraph = PXGraph.CreateInstance<RegisterEntry>();

			arGraph.Clear();
			pmGraph.Clear();

			PMRegister pmDoc = null;
			ARInvoice arDoc = null;

			List<ARRegister> doclist = new List<ARRegister>();
			List<EPExpenseClaimDetails> listOfDirectBilledClaims = new List<EPExpenseClaimDetails>();

			PXSelectBase<EPExpenseClaimDetails> select = new PXSelectJoin<EPExpenseClaimDetails,
															LeftJoin<Contract, On<EPExpenseClaimDetails.contractID, Equal<Contract.contractID>, And<Where<Contract.baseType, Equal<Contract.ContractBaseType>, Or<Contract.nonProject, Equal<True>>>>>
															, LeftJoin<Account, On<EPExpenseClaimDetails.expenseAccountID, Equal<Account.accountID>>>
															>,
															Where<EPExpenseClaimDetails.released, Equal<boolTrue>,
															And<EPExpenseClaimDetails.billable, Equal<boolTrue>,
															And<EPExpenseClaimDetails.billed, Equal<boolFalse>,
															And<EPExpenseClaimDetails.customerID, Equal<Required<EPExpenseClaimDetails.customerID>>,
															And<EPExpenseClaimDetails.customerLocationID, Equal<Required<EPExpenseClaimDetails.customerLocationID>>,
															And<EPExpenseClaimDetails.expenseDate, LessEqual<Required<EPExpenseClaimDetails.expenseDate>>,
															And<Where<EPExpenseClaimDetails.contractID, Equal<Contract.contractID>, Or<EPExpenseClaimDetails.contractID, IsNull>>>>>>>>>,
															OrderBy<Asc<EPExpenseClaimDetails.branchID>>>(this);

			arGraph.RowPersisted.AddHandler<ARInvoice>(
			delegate(PXCache sender, PXRowPersistedEventArgs e)
			{
				if (e.TranStatus == PXTranStatus.Open)
				{
					foreach (EPExpenseClaimDetails row in listOfDirectBilledClaims.Select(claimdetail => Transactions.Locate(claimdetail)))
					{
						row.ARDocType = ((ARInvoice)e.Row).DocType;
						row.ARRefNbr = ((ARInvoice)e.Row).RefNbr;
					}
				}
			});


			decimal signOperation = 1;

			foreach (PXResult<EPExpenseClaimDetails, Contract, Account> res in select.Select(customer.CustomerID, customer.LocationID, filter.EndDate))
			{
				EPExpenseClaimDetails row = (EPExpenseClaimDetails)res;

				if (row.ContractID != null && !ProjectDefaultAttribute.IsNonProject(this, row.ContractID))
				{
					if (pmDoc == null)
					{
						pmDoc = (PMRegister)pmGraph.Document.Cache.Insert();
						pmDoc.OrigDocType = PMOrigDocType.ExpenseClaim;
						pmDoc.OrigDocNbr = row.RefNbr;

					}

					PMTran usage = InsertPMTran(pmGraph, res);
					if (usage.Released == true) //contract trans are created as released
					{
						UsageMaint.AddUsage(pmGraph.Transactions.Cache, usage.ProjectID, usage.InventoryID, usage.BillableQty ?? 0m, usage.UOM);
					}
				}
				else
				{
					if (arDoc == null || arDoc.BranchID != row.BranchID)
					{
						if (arDoc != null)
						{
							arDoc.CuryOrigDocAmt = arDoc.CuryDocBal;
							arGraph.Document.Update(arDoc);
							arGraph.Save.Press();
							listOfDirectBilledClaims.Clear();
						}
						EPExpenseClaimDetails summDetail = PXSelectJoinGroupBy<EPExpenseClaimDetails,
																		LeftJoin<Contract, On<EPExpenseClaimDetails.contractID, Equal<Contract.contractID>, And<Where<Contract.baseType, Equal<Contract.ContractBaseType>, Or<Contract.nonProject, Equal<True>>>>>
																		>,
																		Where<EPExpenseClaimDetails.released, Equal<boolTrue>,
																		And<EPExpenseClaimDetails.billable, Equal<boolTrue>,
																		And<EPExpenseClaimDetails.billed, Equal<boolFalse>,
																		And<EPExpenseClaimDetails.customerID, Equal<Required<EPExpenseClaimDetails.customerID>>,
																		And<EPExpenseClaimDetails.customerLocationID, Equal<Required<EPExpenseClaimDetails.customerLocationID>>,
																		And<EPExpenseClaimDetails.expenseDate, LessEqual<Required<EPExpenseClaimDetails.expenseDate>>,
																		And<EPExpenseClaimDetails.branchID, Equal<Required<EPExpenseClaimDetails.branchID>>,
																		And<Where<Contract.nonProject, Equal<True>, Or<EPExpenseClaimDetails.contractID, IsNull>>>>>>>>>>
																		, Aggregate<Sum<EPExpenseClaimDetails.curyTranAmt>>
																		>.Select(this, customer.CustomerID, customer.LocationID, filter.EndDate, row.BranchID);

						if (summDetail.CuryTranAmt < 0)
						{
							signOperation = -1;
						}
						else
						{
							signOperation = 1;
						}

						arDoc = (ARInvoice)arGraph.Document.Cache.Insert();
						//arDocList.Add(arDoc);
						if (signOperation < 0)
							arGraph.Document.Cache.SetValueExt<ARInvoice.docType>(arDoc, AR.ARDocType.CreditMemo);
						else
							arGraph.Document.Cache.SetValueExt<ARInvoice.docType>(arDoc, AR.ARDocType.Invoice);
						arGraph.Document.Cache.SetValueExt<ARInvoice.customerID>(arDoc, row.CustomerID);
						arGraph.Document.Cache.SetValueExt<ARInvoice.customerLocationID>(arDoc, row.CustomerLocationID);
						arGraph.Document.Cache.SetValueExt<ARInvoice.docDate>(arDoc, filter.InvoiceDate);
						arGraph.Document.Cache.SetValueExt<ARInvoice.branchID>(arDoc, row.BranchID);
						arDoc = arGraph.Document.Update(arDoc);
						arDoc.FinPeriodID = filter.InvFinPeriodID;
						if (Setup.Current.AutomaticReleaseAR == true)
							arDoc.Hold = false;
						doclist.Add(arDoc);
					}

					//insert ARTran
					InsertARTran(arGraph, row, signOperation);
					listOfDirectBilledClaims.Add(row);
				}

				row.Billed = true;
				Transactions.Update(row);
			}

			if (arDoc != null)
			{
				arDoc.CuryOrigDocAmt = arDoc.CuryDocBal;
				arGraph.Document.Update(arDoc);
				arGraph.Save.Press();
			}

			if (pmDoc != null)
				pmGraph.Save.Press();

			this.Persist(typeof(EPExpenseClaimDetails), PXDBOperation.Update);

			if (Setup.Current.AutomaticReleaseAR == true)
			{
				ARDocumentRelease.ReleaseDoc(doclist, false);
			}
		}

		protected virtual void InsertARTran(ARInvoiceEntry arGraph, EPExpenseClaimDetails row, decimal signOperation)
		{
			CurrencyInfo curyInfo = PXSelect<CurrencyInfo>.Search<CurrencyInfo.curyInfoID>(arGraph, row.CuryInfoID);

			decimal curyamount;
			if (arGraph.currencyinfo.Current != null && curyInfo != null && arGraph.currencyinfo.Current.CuryID == curyInfo.CuryID)
				curyamount = row.CuryTranAmt.GetValueOrDefault(0);
			else
				CM.PXCurrencyAttribute.CuryConvCury(arGraph.Document.Cache, arGraph.currencyinfo, row.TranAmt.GetValueOrDefault(), out curyamount);

			ARTran tran = arGraph.Transactions.Insert();
			tran.InventoryID = row.InventoryID;
			tran.TranDesc = row.TranDesc;
			tran.Qty = row.Qty * signOperation;
			tran.UOM = row.UOM;
			tran.AccountID = row.SalesAccountID;
			tran.SubID = row.SalesSubID;
			tran.Date = row.ExpenseDate;
			tran.CuryTranAmt = curyamount * signOperation;
			tran.CuryUnitPrice = (curyamount / (row.Qty.GetValueOrDefault(1m) != 0m ? row.Qty.GetValueOrDefault(1m) : 1m)) * signOperation;
			tran.CuryExtPrice = curyamount * signOperation;
			tran = arGraph.Transactions.Update(tran);

			PXNoteAttribute.CopyNoteAndFiles(Caches[typeof(EPExpenseClaimDetails)], row, arGraph.Transactions.Cache, tran, Setup.Current.GetCopyNoteSettings<PXModule.ar>());

		}

		protected virtual PMTran InsertPMTran(RegisterEntry pmGraph, PXResult<EPExpenseClaimDetails, Contract, Account> res)
		{
			EPExpenseClaimDetails detail = res;
			Contract contract = res;
			Account account = res;

			if (account.AccountGroupID == null && contract.BaseType == PMProject.ProjectBaseType.Project)
				throw new PXException(Messages.AccountGroupIsNotAssignedForAccount, account.AccountCD);

			bool released = contract.BaseType == Contract.ContractBaseType.Contract; //contract trans are created as released

			PMTran tran = (PMTran)pmGraph.Transactions.Cache.Insert();
			tran.AccountGroupID = account.AccountGroupID;
			tran.BAccountID = detail.CustomerID;
			tran.LocationID = detail.CustomerLocationID;
			tran.ProjectID = detail.ContractID;
			tran.TaskID = detail.TaskID;
			tran.InventoryID = detail.InventoryID;
			tran.Qty = detail.Qty;
			tran.Billable = true;
			tran.BillableQty = detail.Qty;
			tran.UOM = detail.UOM;
			if (detail.CuryInfoID == detail.ClaimCuryInfoID && detail.EmployeePart == 0m)
				tran.UnitRate = detail.UnitCost;
			else
				tran.UnitRate = detail.ClaimTranAmt/detail.Qty;
			tran.Amount = detail.ClaimTranAmt;
			tran.AccountID = detail.ExpenseAccountID;
			tran.SubID = detail.ExpenseSubID;
			tran.StartDate = detail.ExpenseDate;
			tran.EndDate = detail.ExpenseDate;
			tran.Date = detail.ExpenseDate;
			tran.ResourceID = detail.EmployeeID;
			tran.Released = released;

			tran = pmGraph.Transactions.Update(tran);

			pmGraph.Document.Current.Released = released;

			PXNoteAttribute.CopyNoteAndFiles(Caches[typeof(EPExpenseClaimDetails)], detail, pmGraph.Transactions.Cache, tran, Setup.Current.GetCopyNoteSettings<PXModule.pm>());

			return tran;
		}
	}
}
