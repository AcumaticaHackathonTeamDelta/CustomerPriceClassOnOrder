using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PX.Data;
using System.Collections;
using PX.Objects.CS;
using PX.Objects.GL;
using LoadChildDocumentsOptions = PX.Objects.AR.ARPaymentEntry.LoadOptions.loadChildDocuments;

namespace PX.Objects.AR
{
	[System.SerializableAttribute()]
	public partial class ARAutoApplyParameters: IBqlTable
	{
		#region ApplyCreditMemos
		public abstract class applyCreditMemos : PX.Data.IBqlField
		{
		}
		protected bool? _ApplyCreditMemos;
		[PXBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Apply Credit Memos", Visibility = PXUIVisibility.Visible)]
		public virtual bool? ApplyCreditMemos
		{
			get
			{
				return this._ApplyCreditMemos;
			}
			set
			{
				this._ApplyCreditMemos = value;
			}
		}
		#endregion
		#region ReleaseBatchWhenFinished
		public abstract class releaseBatchWhenFinished : PX.Data.IBqlField
		{
		}
		protected bool? _ReleaseBatchWhenFinished;
		[PXBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Release Batch When Finished", Visibility = PXUIVisibility.Visible)]
		public virtual bool? ReleaseBatchWhenFinished
		{
			get
			{
				return this._ReleaseBatchWhenFinished;
			}
			set
			{
				this._ReleaseBatchWhenFinished = value;
			}
		}
		#endregion
		#region LoadChildDocuments
		public abstract class loadChildDocuments : IBqlField { }

		[PXDBString(5, IsFixed = true)]
		[PXUIField(DisplayName = "Include Child Documents")]
		[LoadChildDocumentsOptions.List]
		[PXDefault(LoadChildDocumentsOptions.None)]
		public virtual string LoadChildDocuments { get; set; }
		#endregion
		#region ApplicationDate
		public abstract class applicationDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ApplicationDate;
		[PXDate()]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Application Date", Visibility = PXUIVisibility.Visible)]
		public virtual DateTime? ApplicationDate
		{
			get
			{
				return this._ApplicationDate;
			}
			set
			{
				this._ApplicationDate = value;
			}
		}
		#endregion
		#region FinPeriod
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected String _FinPeriodID;
		[AROpenPeriod(typeof(ARAutoApplyParameters.applicationDate))]
		[PXUIField(DisplayName = "Application Period", Visibility = PXUIVisibility.Visible)]
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
	}

	[TableAndChartDashboardType]
	public class ARAutoApplyPayments : PXGraph<ARAutoApplyPayments>
	{
		public PXCancel<ARAutoApplyParameters> Cancel;
		public PXFilter<ARAutoApplyParameters> Filter;
		[PXFilterable]
		public PXFilteredProcessing<ARStatementCycle, ARAutoApplyParameters> ARStatementCycleList;
		

		public ARAutoApplyPayments()
		{
			ARSetup setup = arsetup.Current;
		}

		public PXSetup<ARSetup> arsetup;

		protected virtual void ARAutoApplyParameters_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
            if (e.Row == null) return;

            ARAutoApplyParameters filter = Filter.Current;
			ARStatementCycleList.SetProcessDelegate<ARPaymentEntry>(
				delegate(ARPaymentEntry graph, ARStatementCycle cycle)
				{
					ProcessDoc(graph, cycle, filter);
				}
			);

			PXStringListAttribute.SetList<ARAutoApplyParameters.loadChildDocuments>(Filter.Cache, filter,
				filter.ApplyCreditMemos == true ?
				(PXStringListAttribute)new LoadChildDocumentsOptions.ListAttribute() :
				(PXStringListAttribute)new LoadChildDocumentsOptions.NoCRMListAttribute());
		}

		protected virtual void ARAutoApplyParameters_ApplyCreditMemos_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			var row = e.Row as ARAutoApplyParameters;
			if(row.ApplyCreditMemos == false && row.LoadChildDocuments == LoadChildDocumentsOptions.IncludeCRM)
			{
				row.LoadChildDocuments = LoadChildDocumentsOptions.ExcludeCRM;
			}
		}

		private static IEnumerable<Customer> GetCustomersForAutoApplication(PXGraph graph, ARStatementCycle cycle, ARAutoApplyParameters filter)
		{
			var result = new List<Customer> { };

			if (filter.LoadChildDocuments != LoadChildDocumentsOptions.None)
			{

				var childrenOfParentsWithCycle = PXSelectJoin<Customer,
					InnerJoin<CustomerMaster,
						On<Customer.parentBAccountID, Equal<CustomerMaster.bAccountID>,
						And<Customer.consolidateToParent, Equal<True>>>>,
					Where<CustomerMaster.statementCycleId, Equal<Required<Customer.statementCycleId>>,
						And<Match<Required<AccessInfo.userName>>>>>
						.Select(graph, cycle.StatementCycleId, graph.Accessinfo.UserName);

				result.AddRange(childrenOfParentsWithCycle.RowCast<Customer>());
			}

			var nonChildrenWithCycle = PXSelectJoin<Customer,
				LeftJoin<CustomerMaster,
					On<Customer.parentBAccountID, Equal<CustomerMaster.bAccountID>,
					And<Customer.consolidateToParent, Equal<True>>>>,
				Where<Customer.statementCycleId, Equal<Required<Customer.statementCycleId>>,
					And2<Match<Required<AccessInfo.userName>>,
					And<Where<CustomerMaster.bAccountID, IsNull,
						Or<CustomerMaster.statementCycleId, NotEqual<Required<CustomerMaster.statementCycleId>>>>>>>>
					.Select(graph, cycle.StatementCycleId, graph.Accessinfo.UserName, cycle.StatementCycleId);

			var toAdd = nonChildrenWithCycle.RowCast<Customer>()
				.Where(c => result.FirstOrDefault(alreadyPresent => alreadyPresent.BAccountID == c.BAccountID) == null).ToList();
			result.AddRange(toAdd);
			return result;
		}
		
		public static void ProcessDoc(ARPaymentEntry graph, ARStatementCycle cycle, ARAutoApplyParameters filter)
		{
			List<ARRegister> toRelease = new List<ARRegister>();

			foreach (Customer customer in GetCustomersForAutoApplication(graph, cycle, filter))
			{
				List<ARInvoice> arInvoiceList = new List<ARInvoice>();

				var invoiceQuery = new PXSelectJoin<ARInvoice,
					InnerJoin<Customer, On<ARInvoice.customerID, Equal<Customer.bAccountID>>>,
					Where<ARInvoice.released, Equal<True>,
						And<ARInvoice.openDoc, Equal<True>,
						And<ARInvoice.pendingPPD, NotEqual<True>, 
						And<Where<ARInvoice.docType, Equal<ARDocType.invoice>,
							Or<ARInvoice.docType, Equal<ARDocType.finCharge>,
							Or<ARInvoice.docType, Equal<ARDocType.debitMemo>>>>>>>>,
					OrderBy<Asc<ARInvoice.dueDate>>>(graph);

				if(filter.LoadChildDocuments == LoadChildDocumentsOptions.None || customer.ParentBAccountID != null)
				{
					invoiceQuery.WhereAnd<Where<Customer.bAccountID, Equal<Required<ARInvoice.customerID>>>>();
				}
				else
				{
					invoiceQuery.WhereAnd<Where<Customer.consolidatingBAccountID, Equal<Required<ARInvoice.customerID>>>>();
				}

				foreach (ARInvoice invoice in invoiceQuery.Select(
					customer.BAccountID))
				{
					arInvoiceList.Add(invoice);
				}

				arInvoiceList.Sort(new Comparison<ARInvoice>(delegate(ARInvoice a, ARInvoice b)
					{
						if ((bool)graph.arsetup.Current.FinChargeFirst)
						{
							int aSortOrder = (a.DocType == ARDocType.FinCharge ? 0 : 1);
							int bSortOrder = (b.DocType == ARDocType.FinCharge ? 0 : 1);
							int ret = ((IComparable)aSortOrder).CompareTo(bSortOrder);
							if (ret != 0)
							{
								return ret;
							}
						}

						{
							object aDueDate = a.DueDate;
							object bDueDate = b.DueDate;
							int ret = ((IComparable)aDueDate).CompareTo(bDueDate);

							return ret;
						}
					}
				));


				if (arInvoiceList.Count > 0)
				{
					foreach (ARPayment payment in PXSelect<ARPayment,
								Where<ARPayment.released, Equal<True>,
									And<ARPayment.openDoc, Equal<True>,
									And<ARPayment.customerID, Equal<Required<ARPayment.customerID>>,
									And<Where<ARPayment.docType, Equal<ARDocType.payment>,
										Or<ARPayment.docType, Equal<ARDocType.prepayment>,
										Or<ARPayment.docType, Equal<Required<ARPayment.docType>>>>>>>>>,
								OrderBy<Asc<ARPayment.docDate>>>.Select(graph,
									customer.BAccountID,
									filter.ApplyCreditMemos == true ? ARDocType.CreditMemo : ARDocType.Payment))
					{
						ApplyPayment(graph, filter, payment, arInvoiceList, toRelease);
					}
				}

				var remainingParentInvoices = arInvoiceList.Where(inv => inv.CustomerID == customer.BAccountID).ToList();

				if(remainingParentInvoices.Count > 0
					&& filter.ApplyCreditMemos == true && filter.LoadChildDocuments == LoadChildDocumentsOptions.IncludeCRM)
				{
					foreach (ARPayment payment in PXSelectJoin<ARPayment,
								InnerJoin<Customer, On<ARPayment.customerID, Equal<Customer.bAccountID>>>,
								Where<ARPayment.released, Equal<True>,
									And<ARPayment.openDoc, Equal<True>,
									And<Customer.consolidatingBAccountID, Equal<Required<Customer.consolidatingBAccountID>>,
									And<ARPayment.docType, Equal<ARDocType.creditMemo>>>>>,
								OrderBy<Asc<ARPayment.docDate>>>.Select(graph, customer.BAccountID))
					{
						ApplyPayment(graph, filter, payment, remainingParentInvoices, toRelease);
					}
				}
			}

			if (toRelease.Count > 0)
			{
				ARDocumentRelease.ReleaseDoc(toRelease, false);
			}
		}

		private static void ApplyPayment(ARPaymentEntry graph, ARAutoApplyParameters filter, ARPayment payment, List<ARInvoice> arInvoiceList, List<ARRegister> toRelease)
		{
			if (arInvoiceList.Any() == false)
				return;

			int invoiceIndex = 0;
			var paymentsViewIntoInvoiceList = new List<ARInvoice>(arInvoiceList);

			graph.Clear();
			graph.Document.Current = payment;

			bool adjustmentAdded = false;
			while (graph.Document.Current.CuryUnappliedBal > 0 && invoiceIndex < paymentsViewIntoInvoiceList.Count)
			{
				if (graph.Document.Current.CuryApplAmt == null)
				{
					object curyapplamt = graph.Document.Cache.GetValueExt<ARPayment.curyApplAmt>(graph.Document.Current);
					if (curyapplamt is PXFieldState)
					{
						curyapplamt = ((PXFieldState)curyapplamt).Value;
					}
					graph.Document.Current.CuryApplAmt = (decimal?)curyapplamt;
				}
				graph.Document.Current.AdjDate = filter.ApplicationDate;
				graph.Document.Current.AdjFinPeriodID = filter.FinPeriodID;
				graph.Document.Cache.Update(graph.Document.Current);

				ARInvoice invoice = paymentsViewIntoInvoiceList[invoiceIndex];

				ARAdjust adj = new ARAdjust();
				adj.AdjdDocType = invoice.DocType;
				adj.AdjdRefNbr = invoice.RefNbr;

				graph.AutoPaymentApp = true;
				adj = graph.Adjustments.Insert(adj);
				if (adj == null)
				{
					invoiceIndex++;
					continue;
				}
				adjustmentAdded = true;
				if (adj.CuryDocBal <= 0m)
				{
					arInvoiceList.Remove(invoice);
				}

				invoiceIndex++;
			}
			if (adjustmentAdded)
			{
				graph.Save.Press();
				if (filter.ReleaseBatchWhenFinished == true)
				{
					toRelease.Add(graph.Document.Current);
				}
			}
		}

		protected virtual void ARStatementCycle_RowSelecting(PXCache sender, PXRowSelectingEventArgs e)
		{
			if (e.Row != null)
			{
				ARStatementCycle row = e.Row as ARStatementCycle;

				row.NextStmtDate = ARStatementProcess.CalcStatementDateBefore(this.Accessinfo.BusinessDate.Value, row.PrepareOn, row.Day00, row.Day01);
				if (row.LastStmtDate.HasValue && row.NextStmtDate <= row.LastStmtDate)
					row.NextStmtDate = ARStatementProcess.CalcNextStatementDate(row.LastStmtDate.Value, row.PrepareOn, row.Day00, row.Day01);
			}
		}
	}
}
