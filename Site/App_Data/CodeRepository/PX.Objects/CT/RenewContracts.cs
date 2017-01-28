using System;
using PX.Data;
using System.Collections;
using PX.Objects.AR;
using PX.Objects.GL;

namespace PX.Objects.CT
{	
	[TableAndChartDashboardType]
	public class RenewContracts : PXGraph<RenewContracts>
	{
		public PXCancel<RenewalContractFilter> Cancel;
		public PXAction<RenewalContractFilter> EditDetail;
		public PXFilter<RenewalContractFilter> Filter;
		public PXFilteredProcessing<ContractsList, RenewalContractFilter> Items;

		public RenewContracts()
		{
			Items.SetSelected<ContractsList.selected>();
		}

		protected virtual IEnumerable items()
		{
			RenewalContractFilter filter = Filter.Current;
			if (filter == null)
			{
				yield break;
			}
			bool found = false;
			foreach (ContractsList item in Items.Cache.Inserted)
			{
				found = true;
				yield return item;
			}
			if (found)
				yield break;


			PXSelectBase<Contract> select = new PXSelectJoin<Contract, 
				InnerJoin<ContractBillingSchedule, On<Contract.contractID, Equal<ContractBillingSchedule.contractID>>, 
				InnerJoin<Customer, On<Customer.bAccountID, Equal<Contract.customerID>>>>,
				Where<Contract.isTemplate, NotEqual<True>,
					And<Contract.baseType, Equal<Contract.ContractBaseType>,
					And<Contract.type, NotEqual<Contract.type.unlimited>,
					And<Contract.autoRenew, Equal<True>,
					And<Where<Contract.status, Equal<Contract.status.active>, 
						Or<Contract.status, Equal<Contract.status.expired>>>>>>>>>(this);

			
			if (!string.IsNullOrEmpty(filter.CustomerClassID))
			{
				select.WhereAnd<Where<Customer.customerClassID, Equal<Current<RenewalContractFilter.customerClassID>>>>();
			}

			if (filter.TemplateID != null)
			{
				select.WhereAnd<Where<Contract.templateID, Equal<Current<RenewalContractFilter.templateID>>>>();
			}

			/*
			 * Expiring Contracts has a hierarchical structure and we need to show only the latest expiring node hidding
			 * all of its original contracts
			*/
			foreach (PXResult<Contract, ContractBillingSchedule, Customer> resultSet in select.Select())
			{
				Contract contract = resultSet;
				ContractBillingSchedule schedule = resultSet;
				Customer customer = resultSet;

				bool skipItem = false;
				if (contract.Type == Contract.type.Expiring || contract.Type == Contract.type.Renewable)
				{
					Contract child = PXSelect<Contract, Where<Contract.originalContractID, Equal<Required<Contract.originalContractID>>>>.Select(this, contract.ContractID);
					skipItem = child != null;
				}

				if (!skipItem && contract.ExpireDate != null && 
					((DateTime)contract.ExpireDate).AddDays(-(contract.AutoRenewDays ?? 0)) <= filter.RenewalDate)
				{
					yield return Items.Insert(new ContractsList
					{
						ContractID = contract.ContractID,
						Description = contract.Description,
						Type = contract.Type,
						ExpireDate = contract.ExpireDate,
						CustomerID = contract.CustomerID,
						CustomerName = customer.AcctName,
						LastDate = schedule.LastDate,
						NextDate = schedule.NextDate,
						TemplateID = contract.TemplateID,
						Status = contract.Status,
						StartDate = contract.StartDate
					});
				}
			}

			Items.Cache.IsDirty = false;
		}

		#region Actions

		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Enabled = true)]
		[PXEditDetailButton]
		public virtual IEnumerable editDetail(PXAdapter adapter)
		{
			if (Items.Current != null)
			{
				ContractMaint target = PXGraph.CreateInstance<ContractMaint>();
				target.Clear();
				target.Contracts.Current = PXSelect<CT.Contract, Where<CT.Contract.contractID, Equal<Current<ContractsList.contractID>>>>.Select(this);
				throw new PXRedirectRequiredException(target, true, "ViewContract"){Mode = PXBaseRedirectException.WindowMode.NewWindow};
			}

			return adapter.Get();
		}

		#endregion
			
		#region EventHandlers
		protected virtual void RenewalContractFilter_RowUpdated(PXCache cache, PXRowUpdatedEventArgs e)
		{
			Items.Cache.Clear();
		}

		protected virtual void RenewalContractFilter_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			RenewalContractFilter filter = Filter.Current;
			Items.SetProcessDelegate<ContractMaint>(delegate(ContractMaint graph, ContractsList item)
			{
				RenewContract(graph, item, filter);
			});
		}

		[PXDBDate]
		[PXUIField(DisplayName = "Setup Date", Visibility = PXUIVisibility.Visible)]
		protected virtual void ContractsList_StartDate_CacheAttached(PXCache sender) {}

		protected virtual void ContractsList_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			ContractsList contract = e.Row as ContractsList;
			if (contract == null) return;

			if (contract.NextDate < contract.ExpireDate)
			{
				PXUIFieldAttribute.SetEnabled<ContractsList.selected>(sender, contract, false);
				sender.RaiseExceptionHandling<ContractsList.selected>(contract, null, new PXSetPropertyException(Messages.BillContractBeforeRenewal, PXErrorLevel.RowWarning));
			}
		}

		#endregion

		public static void RenewContract(ContractMaint docgraph, ContractsList item, RenewalContractFilter filter)
		{
			docgraph.Contracts.Current = PXSelect<Contract, Where<Contract.contractID, Equal<Required<Contract.contractID>>>>.Select(docgraph, item.ContractID);
			docgraph.Billing.Current = docgraph.Billing.Select();
			docgraph.RenewContract(filter.RenewalDate.Value);
		}

		#region Local Types

		[Serializable]
		public partial class RenewalContractFilter : IBqlTable
		{
			#region CustomerClassID
			public abstract class customerClassID : PX.Data.IBqlField
			{
			}
			protected String _CustomerClassID;
			[PXDBString(10, IsUnicode = true)]
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
			#region TemplateID
			public abstract class templateID : PX.Data.IBqlField
			{
			}
			protected Int32? _TemplateID;
			[ContractTemplate]
			public virtual Int32? TemplateID
			{
				get
				{
					return this._TemplateID;
				}
				set
				{
					this._TemplateID = value;
				}
			}
			#endregion
			#region RenewalDate
			public abstract class renewalDate : IBqlField {}
			[PXDBDate]
			[PXDefault(typeof(AccessInfo.businessDate))]
			[PXUIField(DisplayName = "Renewal Date")]
			public virtual DateTime? RenewalDate { get; set; }
			#endregion
		}
				
		[Serializable]
		public partial class ContractsList : IBqlTable
		{
			#region Selected
			public abstract class selected : IBqlField
			{
			}
			protected bool? _Selected = false;
			[PXBool]
			[PXDefault(false)]
			[PXUIField(DisplayName = "Selected", Visibility = PXUIVisibility.Visible)]
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

			#region ContractID
			public abstract class contractID : PX.Data.IBqlField
			{
			}
			protected Int32? _ContractID;
			[Contract(IsKey = true)]
			public virtual Int32? ContractID
			{
				get
				{
					return this._ContractID;
				}
				set
				{
					this._ContractID = value;
				}
			}
			#endregion

			#region CustomerID
			public abstract class customerID : PX.Data.IBqlField
			{
			}
			protected Int32? _CustomerID;
			[PXDBInt()]
			[PXUIField(DisplayName = "Customer ID", Visibility = PXUIVisibility.Visible)]
			[PXSelector(typeof(Customer.bAccountID), SubstituteKey = typeof(Customer.acctCD))]
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

			#region CustomerName
			public abstract class customerName : PX.Data.IBqlField
			{
			}
			protected string _CustomerName;
			[PXDBString(60, IsUnicode = true)]
			[PXUIField(DisplayName = "Customer Name", Visibility = PXUIVisibility.Visible)]
			public virtual string CustomerName
			{
				get
				{
					return this._CustomerName;
				}
				set
				{
					this._CustomerName = value;
				}
			}
			#endregion

			#region Description
			public abstract class description : PX.Data.IBqlField
			{
			}
			protected String _Description;
			[PXDBString(60, IsUnicode = true)]
			[PXDefault()]
			[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.Visible)]
			public virtual String Description
			{
				get
				{
					return this._Description;
				}
				set
				{
					this._Description = value;
				}
			}
			#endregion

			#region LastDate
			public abstract class lastDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _LastDate;
			[PXDBDate()]
			[PXUIField(DisplayName = "Last Billing Date", Visibility = PXUIVisibility.Visible)]
			public virtual DateTime? LastDate
			{
				get
				{
					return this._LastDate;
				}
				set
				{
					this._LastDate = value;
				}
			}
			#endregion

			#region NextDate
			public abstract class nextDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _NextDate;
			[PXDBDate()]
			[PXUIField(DisplayName = "Next Billing Date", Visibility = PXUIVisibility.Visible)]
			public virtual DateTime? NextDate
			{
				get
				{
					return this._NextDate;
				}
				set
				{
					this._NextDate = value;
				}
			}
			#endregion

			#region ExpireDate
			public abstract class expireDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _ExpireDate;
			[PXDBDate()]
			[PXUIField(DisplayName = "Expiration Date", Visibility = PXUIVisibility.Visible)]
			public virtual DateTime? ExpireDate
			{
				get
				{
					return this._ExpireDate;
				}
				set
				{
					this._ExpireDate = value;
				}
			}
			#endregion

			#region Type
			public abstract class type : PX.Data.IBqlField
			{
			}
			protected String _Type;
			[PXDBString(1, IsFixed = true)]
			[PXUIField(DisplayName = "Contract Type", Visibility = PXUIVisibility.Visible)]
			[Contract.type.List]
			[PXDefault(Contract.type.Renewable)]
			public virtual String Type
			{
				get
				{
					return this._Type;
				}
				set
				{
					this._Type = value;
				}
			}
			#endregion

			#region TemplateID
			public abstract class templateID : PX.Data.IBqlField
			{
			}
			protected Int32? _TemplateID;
			[ContractTemplate]
			public virtual Int32? TemplateID
			{
				get
				{
					return this._TemplateID;
				}
				set
				{
					this._TemplateID = value;
				}
			}
			#endregion

			#region Status
			public abstract class status : PX.Data.IBqlField
			{
			}
			protected String _Status;
			[PXDBString(1, IsFixed = true)]
			[Contract.status.List]
			[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.Visible)]
			public virtual String Status
			{
				get
				{
					return this._Status;
				}
				set
				{
					this._Status = value;
				}
			}
			#endregion

			#region StartDate
			public abstract class startDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _StartDate;
			[PXDBDate()]
			[PXUIField(DisplayName = "Start Date", Visibility = PXUIVisibility.Visible)]
			public virtual DateTime? StartDate
			{
				get
				{
					return this._StartDate;
				}
				set
				{
					this._StartDate = value;
				}
			}
			#endregion

		}

		#endregion
	}
}
