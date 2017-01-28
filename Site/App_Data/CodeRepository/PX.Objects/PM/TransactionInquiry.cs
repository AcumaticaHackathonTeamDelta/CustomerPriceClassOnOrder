using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.IN;
using System.Collections;
using PX.Objects.GL;
using PX.Web.UI;
using PX.Objects.EP;

namespace PX.Objects.PM
{
	[GL.TableDashboardType]
    [Serializable]
	public class TransactionInquiry : PXGraph<TransactionInquiry>
	{
		#region DAC Attributes Override

		#region PMTran

		[AccountGroup(DisplayName="Debit Account Group")]
		protected virtual void PMTran_AccountGroupID_CacheAttached(PXCache sender)
		{
		}

		#endregion


		#region ARTran
		
		[PXDBString(15, IsUnicode = true, IsKey = true)]
		[PXUIField(DisplayName = "Invoice Ref. Nbr.", Visibility = PXUIVisibility.Visible)]
		protected virtual void ARTran_RefNbr_CacheAttached(PXCache sender)
		{
		}

		#endregion
		#endregion

		public PXFilter<TranFilter> Filter;
		public PXCancel<TranFilter> Cancel;

	    public PXSelect<ARTran> Dummy;
			
		[PXFilterable]
		public PXSelectJoin<PMTran,
				LeftJoin<Account, On<Account.accountID, Equal<PMTran.offsetAccountID>>,
				LeftJoin<PMRegister, On<PMTran.tranType, Equal<PMRegister.module>, And<PMTran.refNbr, Equal<PMRegister.refNbr>>>,
				LeftJoin<ARTran, On<ARTran.pMTranID, Equal<PMTran.tranID>>,
				LeftJoin<PMAllocationAuditTran, On<PMAllocationAuditTran.sourceTranID, Equal<PMTran.tranID>>,
				LeftJoin<PMTranEx, On<PMTranEx.tranID, Equal<PMAllocationAuditTran.tranID>>>>>>>,
				Where<PMTran.projectID, Equal<Current<TranFilter.projectID>>>> Transactions;


		public virtual IEnumerable transactions()
		{
            PXSelectBase<PMTran> select = new PXSelectJoinGroupBy<PMTran,
                LeftJoin<Account, On<Account.accountID, Equal<PMTran.offsetAccountID>>,
                LeftJoin<PMRegister, On<PMTran.tranType, Equal<PMRegister.module>, And<PMTran.refNbr, Equal<PMRegister.refNbr>>>,
				LeftJoin<ARTran, On<ARTran.pMTranID, Equal<PMTran.tranID>>,
				LeftJoin<PMAllocationAuditTran, On<PMAllocationAuditTran.sourceTranID, Equal<PMTran.tranID>>,
				LeftJoin<PMTranEx, On<PMTranEx.tranID, Equal<PMAllocationAuditTran.tranID>>>>>>>,
                Where<PMTran.projectID, Equal<Current<TranFilter.projectID>>>,
				Aggregate<GroupBy<PMTran.tranID, GroupBy<PMTran.billable, GroupBy<PMTran.useBillableQty, GroupBy<PMTran.allocated,
				GroupBy<PMTran.released, GroupBy<PMTran.billed, GroupBy<PMTran.reversed, GroupBy<PMTran.isNonGL, GroupBy<PMTran.isQtyOnly>>>>>>>>>>>(this);

			TranFilter filter = this.Filter.Current;
			if (filter != null)
			{
                if(filter.AccountGroupID != null)
                {
                    select.WhereAnd<Where<PMTran.accountGroupID, Equal<Current<TranFilter.accountGroupID>>, Or<Account.accountGroupID, Equal<Current<TranFilter.accountGroupID>>>>>();
                }
                if(filter.ProjectTaskID != null)
                {
                    select.WhereAnd<Where<PMTran.taskID, Equal<Current<TranFilter.projectTaskID>>>>();
                }
				if (filter.InventoryID != null)
				{
					select.WhereAnd<Where<PMTran.inventoryID, Equal<Current<TranFilter.inventoryID>>>>();
				}
				if (filter.ResourceID != null)
				{
					select.WhereAnd<Where<PMTran.resourceID, Equal<Current<TranFilter.resourceID>>>>();
				}
				if (filter.OnlyAllocation == true)
				{
					select.WhereAnd<Where<PMRegister.isAllocation, Equal<True>>>();
				}
				if (filter.DateFrom != null && filter.DateTo != null)
				{
					if (filter.DateFrom == filter.DateTo)
					{
						select.WhereAnd<Where<PMTran.date, Equal<Current<TranFilter.dateFrom>>>>();
					}
					else
					{
						select.WhereAnd<Where<PMTran.date, Between<Current<TranFilter.dateFrom>, Current<TranFilter.dateTo>>>>();
					}
				}
				else if (filter.DateFrom != null)
				{
					select.WhereAnd<Where<PMTran.date, GreaterEqual<Current<TranFilter.dateFrom>>>>();
				}
				else if (filter.DateTo != null)
				{
					select.WhereAnd<Where<PMTran.date, LessEqual<Current<TranFilter.dateTo>>>>();
				}
			}

			return select.Select();
		}

        public TransactionInquiry()
        {
            Transactions.Cache.AllowInsert = false;
            Transactions.Cache.AllowUpdate = false;
            Transactions.Cache.AllowDelete = false;

            PXUIFieldAttribute.SetDisplayName<Account.accountGroupID>(Caches[typeof(Account)], Messages.CreditAccountGroup);
        }

        public PXAction<TranFilter> viewDocument;
        [PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select, Enabled = false)]
        [PXButton]
        public virtual IEnumerable ViewDocument(PXAdapter adapter)
        {
            RegisterEntry graph = CreateInstance<RegisterEntry>();
            graph.Document.Current = graph.Document.Search<PMRegister.refNbr>(Transactions.Current.RefNbr, Transactions.Current.TranType);
            throw new PXRedirectRequiredException(graph, "PMTransactiosn"){Mode = PXBaseRedirectException.WindowMode.NewWindow};
        }

        public PXAction<TranFilter> viewInventory;
		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select, Enabled = false)]
        [PXButton]
        public virtual IEnumerable ViewInventory(PXAdapter adapter)
        {
            InventoryItem inv = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Current<PMTran.inventoryID>>>>.SelectSingleBound(this, new object[] { Transactions.Current });
            if(inv != null && inv.StkItem == true)
            {
                InventoryItemMaint graph = CreateInstance<InventoryItemMaint>();
                graph.Item.Current = inv;
                throw new PXRedirectRequiredException(graph, "Inventory Item") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
            }
            else if(inv != null)
            {
                NonStockItemMaint graph = CreateInstance<NonStockItemMaint>();
                graph.Item.Current = graph.Item.Search<InventoryItem.inventoryID>(inv.InventoryID);
                throw new PXRedirectRequiredException(graph, "Inventory Item") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
            }
            return adapter.Get();
        }


		public PXAction<TranFilter> viewInvoice;
		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select, Enabled = false)]
		[PXButton]
		public virtual IEnumerable ViewInvoice(PXAdapter adapter)
		{
			ARTran arTran = PXSelect<ARTran, Where<ARTran.pMTranID, Equal<Current<PMTran.tranID>>>>.SelectSingleBound(this, new object[] { Transactions.Current });
			if (arTran != null)
			{
				ARInvoiceEntry target = PXGraph.CreateInstance<ARInvoiceEntry>();
				target.Clear();
				target.Document.Current = target.Document.Search<ARInvoice.refNbr>(arTran.RefNbr);
				throw new PXRedirectRequiredException(target, "Invoice") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
			}
			
			return adapter.Get();
		}

		public PXAction<TranFilter> viewAllocation;
		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select, Enabled = false)]
		[PXButton]
		public virtual IEnumerable ViewAllocation(PXAdapter adapter)
		{
			PMAllocationAuditTran audit = PXSelect<PMAllocationAuditTran, Where<PMAllocationAuditTran.sourceTranID, Equal<Current<PMTran.tranID>>>>.SelectSingleBound(this, new object[] { Transactions.Current });
			if (audit != null)
			{
				PMTran allocation = PXSelect<PMTran, Where<PMTran.tranID, Equal<Required<PMTran.tranID>>>>.Select(this, audit.TranID);

				if (allocation != null)
				{
					RegisterEntry graph = CreateInstance<RegisterEntry>();
					graph.Document.Current = graph.Document.Search<PMRegister.refNbr>(allocation.RefNbr, allocation.TranType);
					throw new PXRedirectRequiredException(graph, "PMTransactiosn") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
				}
			}

			return adapter.Get();
		}


	    #region Local Types
        [Serializable]
		public partial class TranFilter : IBqlTable
		{
			#region AccountGroupID
			public abstract class accountGroupID : PX.Data.IBqlField
			{
			}
			protected Int32? _AccountGroupID;
			[AccountGroupAttribute()]
			public virtual Int32? AccountGroupID
			{
				get
				{
					return this._AccountGroupID;
				}
				set
				{
					this._AccountGroupID = value;
				}
			}
			#endregion
			#region ProjectID
			public abstract class projectID : PX.Data.IBqlField
			{
			}
			protected Int32? _ProjectID;
			[PXDefault]
			[Project(typeof(Where<PMProject.isTemplate, NotEqual<True>>), WarnIfCompleted = false)]
			public virtual Int32? ProjectID
			{
				get
				{
					return this._ProjectID;
				}
				set
				{
					this._ProjectID = value;
				}
			}
			#endregion
			#region ProjectTaskID
			public abstract class projectTaskID : PX.Data.IBqlField
			{
			}
			protected Int32? _ProjectTaskID;
			[ProjectTask(typeof(TranFilter.projectID))]
			public virtual Int32? ProjectTaskID
			{
				get
				{
					return this._ProjectTaskID;
				}
				set
				{
					this._ProjectTaskID = value;
				}
			}
			#endregion
			#region InventoryID
			public abstract class inventoryID : PX.Data.IBqlField
			{
			}
			protected Int32? _InventoryID;
			[Inventory]
			public virtual Int32? InventoryID
			{
				get
				{
					return this._InventoryID;
				}
				set
				{
					this._InventoryID = value;
				}
			}
			#endregion
			#region ResourceID
			public abstract class resourceID : PX.Data.IBqlField
			{
			}
			protected Int32? _ResourceID;
			[PXEPEmployeeSelector]
			[PXDBInt()]
			[PXUIField(DisplayName = "Employee")]
			public virtual Int32? ResourceID
			{
				get
				{
					return this._ResourceID;
				}
				set
				{
					this._ResourceID = value;
				}
			}
			#endregion
			#region DateFrom
			public abstract class dateFrom : PX.Data.IBqlField
			{
			}
			protected DateTime? _DateFrom;
			[PXDBDate()]
			[PXUIField(DisplayName="From", Visibility=PXUIVisibility.Visible)]
			public virtual DateTime? DateFrom
			{
				get
				{
					return this._DateFrom;
				}
				set
				{
					this._DateFrom = value;
				}
			}
			#endregion
			#region DateTo
			public abstract class dateTo : PX.Data.IBqlField
			{
			}
			protected DateTime? _DateTo;
			[PXDBDate()]
			[PXUIField(DisplayName = "To", Visibility = PXUIVisibility.Visible)]
			public virtual DateTime? DateTo
			{
				get
				{
					return this._DateTo;
				}
				set
				{
					this._DateTo = value;
				}
			}
			#endregion
			#region OnlyAllocation
			public abstract class onlyAllocation : PX.Data.IBqlField
			{
			}
			protected Boolean? _OnlyAllocation;
			[PXDBBool()]
			[PXDefault(false)]
			[PXUIField(DisplayName="Show only Allocation")]
			public virtual Boolean? OnlyAllocation
			{
				get
				{
					return this._OnlyAllocation;
				}
				set
				{
					_OnlyAllocation = value;
				}
			}
			#endregion
		}

		public partial class PMTranEx : PMTran
		{
			public new abstract class tranID : PX.Data.IBqlField
			{
			}

			public new abstract class tranType : PX.Data.IBqlField
			{
			}

			#region RefNbr
			public new abstract class refNbr : PX.Data.IBqlField
			{
			}
			[PXUIField(DisplayName = "Allocation Ref. Number")]
			[PXDBString(PMRegister.refNbr.Length, IsUnicode = true)]
			public override String RefNbr
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
			
		}
		
		#endregion
	}
}
