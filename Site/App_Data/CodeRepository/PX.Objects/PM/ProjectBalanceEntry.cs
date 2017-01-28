using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.GL;
using PX.Objects.IN;
using System.Collections;
using PX.Objects.CM;

namespace PX.Objects.PM
{
	[GL.TableDashboardType]
    [Serializable]
	public class ProjectBalanceEntry : PXGraph<ProjectBalanceEntry>
	{
		#region DAC Attributes Override

		[PXDefault(typeof(ProjectBalanceFilter.projectID))]
		[PXDBInt()]
		protected virtual void PMProjectStatusEx_ProjectID_CacheAttached(PXCache sender)
		{
		}

		[PXDefault]
        [ProjectTask(typeof(PMProjectStatusEx.projectID))]
		protected virtual void PMProjectStatusEx_ProjectTaskID_CacheAttached(PXCache sender)
		{
		}

		[PXDefault(typeof(ProjectBalanceFilter.accountGroupID))]
        [PXDBInt()]
		protected virtual void PMProjectStatusEx_AccountGroupID_CacheAttached(PXCache sender)
		{
		}

        [PXDBInt()]
		[PXUIField(DisplayName = "Inventory ID", Visibility = PXUIVisibility.Visible)]
		[PMInventorySelector(typeof(Search2<InventoryItem.inventoryID,
        InnerJoin<PMAccountGroup, On<PMAccountGroup.groupID, Equal<Current<PMProjectStatusEx.accountGroupID>>>,
        LeftJoin<PMInventorySelectorAttribute.Cogs, On<PMInventorySelectorAttribute.Cogs.accountID, Equal<InventoryItem.cOGSAcctID>>,
				LeftJoin<PMInventorySelectorAttribute.Exp, On<PMInventorySelectorAttribute.Exp.accountID, Equal<InventoryItem.cOGSAcctID>>,
        LeftJoin<PMInventorySelectorAttribute.Sale, On<PMInventorySelectorAttribute.Sale.accountID, Equal<InventoryItem.salesAcctID>>>>>>,
		Where<PMAccountGroup.type, Equal<AccountType.expense>,
			And<PMAccountGroup.groupID, Equal<PMInventorySelectorAttribute.Cogs.accountGroupID>,
			And<InventoryItem.stkItem, Equal<True>,
			Or<PMAccountGroup.type, Equal<AccountType.expense>,
				And<PMAccountGroup.groupID, Equal<PMInventorySelectorAttribute.Exp.accountGroupID>,
			And<InventoryItem.stkItem, Equal<False>,
			Or<PMAccountGroup.type, Equal<AccountType.income>,
			And<PMAccountGroup.groupID, Equal<PMInventorySelectorAttribute.Sale.accountGroupID>,
			Or<PMAccountGroup.type, Equal<AccountType.liability>,
			Or<PMAccountGroup.type, Equal<AccountType.asset>,
			Or<PMAccountGroup.type, Equal<PMAccountType.offBalance>>>>>>>>>>>>>), SubstituteKey = typeof(InventoryItem.inventoryCD), Filterable = true)]
		protected virtual void PMProjectStatusEx_InventoryID_CacheAttached(PXCache sender)
		{
		}

		[PMUnit(typeof(PMProjectStatusEx.inventoryID))]
		protected virtual void PMProjectStatusEx_UOM_CacheAttached(PXCache sender)
		{
		}


		#endregion


		#region Views/Selects
		
		public PXFilter<ProjectBalanceFilter> Filter;
		[PXVirtualDAC]
		public ProjectStatusSelect<PMProjectStatusEx, Where<PMProjectStatusEx.accountGroupID, IsNotNull>, OrderBy<Asc<PMProjectStatusEx.sortOrder>>> Items;
		
		public PXSelect<PMTask, Where<PMTask.projectID, Equal<Current<PMProjectStatusEx.projectID>>, And<PMTask.taskID, Equal<Current<PMProjectStatusEx.projectTaskID>>>>> Task;
		
		public virtual IEnumerable items()
        {
            if (Filter.Current == null)
            {
                yield break;
            }

            Dictionary<string, PMProjectStatusEx> cachedItems = new Dictionary<string, PMProjectStatusEx>();

            bool isDirty = false;

            int cxMax = 0;
            foreach (PMProjectStatusEx item in Items.Cache.Cached)
            {
                cxMax = Math.Max(cxMax, item.LineNbr.Value);
                string key = string.Format("{0}.{1}.{2}.{3}", item.AccountGroupID, item.ProjectID, item.ProjectTaskID, item.InventoryID.GetValueOrDefault());

                if (!cachedItems.ContainsKey(key))
                    cachedItems.Add(key, item);

                if (Items.Cache.GetStatus(item) == PXEntryStatus.Inserted ||
                    Items.Cache.GetStatus(item) == PXEntryStatus.Updated ||
                    Items.Cache.GetStatus(item) == PXEntryStatus.Notchanged ||
                    Items.Cache.GetStatus(item) == PXEntryStatus.Held)
                {

                    if (Items.Cache.GetStatus(item) == PXEntryStatus.Inserted ||
                    Items.Cache.GetStatus(item) == PXEntryStatus.Updated)
                    {
                        isDirty = true;
                    }

                    yield return item;
                }

            }

            PXSelectBase<PMProjectStatus> select = new PXSelectJoinGroupBy<PMProjectStatus,
                    InnerJoin<PMTask, On<PMTask.projectID, Equal<PMProjectStatus.projectID>, And<PMTask.taskID, Equal<PMProjectStatus.projectTaskID>>>,
                    InnerJoin<PMAccountGroup, On<PMProjectStatus.accountGroupID, Equal<PMAccountGroup.groupID>>>>,
                    Where<PMProjectStatus.projectID, Equal<Current<ProjectBalanceFilter.projectID>>,
                    And<PMProjectStatus.accountGroupID, Equal<Current<ProjectBalanceFilter.accountGroupID>>>>,
                    Aggregate<GroupBy<PMProjectStatus.accountGroupID,
                    GroupBy<PMProjectStatus.projectID,
                    GroupBy<PMProjectStatus.projectTaskID,
                    GroupBy<PMProjectStatus.inventoryID,
                    Sum<PMProjectStatus.amount,
                    Sum<PMProjectStatus.qty,
                    Sum<PMProjectStatus.revisedAmount,
                    Sum<PMProjectStatus.revisedQty,
                    Sum<PMProjectStatus.actualAmount,
                    Sum<PMProjectStatus.actualQty,
                    GroupBy<PMProjectStatus.isProduction>>>>>>>>>>>>, OrderBy<Asc<PMAccountGroup.sortOrder>>>(this);

            int cx = cxMax + 1;
            foreach (PXResult<PMProjectStatus, PMTask, PMAccountGroup> res in select.Select())
            {
                PMProjectStatus row = (PMProjectStatus)res;
                PMTask task = (PMTask)res;
                PMAccountGroup ag = (PMAccountGroup)res;

                string key = string.Format("{0}.{1}.{2}.{3}", row.AccountGroupID, row.ProjectID, row.ProjectTaskID, row.InventoryID.GetValueOrDefault());

                if (!cachedItems.ContainsKey(key))
                {
                    PMProjectStatusEx item = new PMProjectStatusEx();
                    item.LineNbr = cx++;
                    item = (PMProjectStatusEx)Items.Cache.Insert(item);
                    item.ProjectID = row.ProjectID;
                    item.ProjectTaskID = row.ProjectTaskID;
                    item.AccountGroupID = row.AccountGroupID;
                    item.InventoryID = row.InventoryID;
                    item.Description = row.Description;
                    item.UOM = row.UOM;
                    item.Rate = row.Rate;
                    item.Qty = row.Qty;
                    item.Amount = row.Amount;
                    item.RevisedQty = row.RevisedQty;
                    item.RevisedAmount = row.RevisedAmount;
                    item.ActualQty = row.ActualQty;
                    item.ActualAmount = row.ActualAmount;
                    item.IsProduction = row.IsProduction;
                    item.TaskStatus = task.Status;
                   
                    Items.Cache.SetStatus(item, PXEntryStatus.Held);

                    yield return item;
                }
            }

            Items.Cache.IsDirty = isDirty;
        }

		public PXSelect<PMHistory2Accum> History2; //used by ProjectStatusSelect to accumulate Budget/Revised in PMHistory
				
		#endregion

		#region Buttons
		
		public PXSave<ProjectBalanceFilter> Save;
		public PXCancel<ProjectBalanceFilter> Cancel;

		public PXAction<ProjectBalanceFilter> viewBalance;
		[PXUIField(DisplayName = Messages.ViewBalance, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntry)]
		public IEnumerable ViewBalance(PXAdapter adapter)
		{
			if (Items.Current != null)
			{
				ProjectBalanceByPeriodEntry graph = PXGraph.CreateInstance<ProjectBalanceByPeriodEntry>();
				ProjectBalanceByPeriodEntry.ProjectBalanceFilter filter = new ProjectBalanceByPeriodEntry.ProjectBalanceFilter();
				filter.AccountGroupID = Items.Current.AccountGroupID;
				filter.ProjectID = Items.Current.ProjectID;
				filter.ProjectTaskID = Items.Current.ProjectTaskID;
				filter.InventoryID = Items.Current.InventoryID;

				graph.Filter.Insert(filter);

				throw new PXPopupRedirectException(graph, Messages.ProjectBalanceEntry + " - " + Messages.ViewBalance, true);
			}
			return adapter.Get();
		}

		public PXAction<ProjectBalanceFilter> viewTransactions;
		[PXUIField(DisplayName = Messages.ViewTransactions, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.Inquiry)]
		public IEnumerable ViewTransactions(PXAdapter adapter)
		{
			if (Items.Current != null)
			{
				TransactionInquiry target = PXGraph.CreateInstance<TransactionInquiry>();
			    target.Filter.Insert(new TransactionInquiry.TranFilter());
				target.Filter.Current.AccountGroupID = Items.Current.AccountGroupID;
				target.Filter.Current.ProjectID = Items.Current.ProjectID;
				target.Filter.Current.ProjectTaskID = Items.Current.ProjectTaskID;
				if ( Items.Current.InventoryID != null && !ProjectDefaultAttribute.IsNonProject(this, Items.Current.InventoryID) )
					target.Filter.Current.InventoryID = Items.Current.InventoryID;

				throw new PXPopupRedirectException(target, Messages.TransactionInquiry + " - " + Messages.ViewTransactions, true);
			}
			return adapter.Get();
		} 

		#endregion

		#region Event Handlers

        protected virtual void PMProjectStatusEx_LineNbr_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            PMProjectStatusEx row = e.Row as PMProjectStatusEx;
            if (row != null)
            {
                if (e.NewValue == null)
                {
                    int minLineNbr = 0;

                    foreach (PMProjectStatusEx ps in sender.Inserted)
                    {
                        minLineNbr = Math.Min(minLineNbr, ps.LineNbr.Value);
                    }

                    e.NewValue = minLineNbr - 1;
                }

            }
        }

		protected virtual void ProjectBalanceFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			ProjectBalanceFilter row = e.Row as ProjectBalanceFilter;
			if (row != null)
			{
				Items.Cache.AllowInsert = row.ProjectID != null && row.AccountGroupID != null;
			}
		}

		protected virtual void ProjectBalanceFilter_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
            this.Caches[typeof(PMProjectStatusEx)].Clear();
            this.Caches[typeof(PMProjectStatus)].Clear();
			this.Caches[typeof(PMHistory2Accum)].Clear();
        }

        #endregion

		public override void Persist()
		{
			ValidateUniqueFields();

			using (PXTransactionScope ts = new PXTransactionScope())
			{
				base.Persist();
				this.Persist(typeof(PMHistory2Accum), PXDBOperation.Insert);
				ts.Complete();
			}
			this.Caches[typeof(PMHistory2Accum)].Clear();
            this.Caches[typeof(PMProjectStatusEx)].Clear();
            this.Caches[typeof(PMProjectStatus)].Clear();
		}

		protected virtual void ValidateUniqueFields()
		{
			bool validationFailed = false;
			List<string> keys = new List<string>();

			foreach (PMProjectStatusEx item in Items.Select())
			{
				if (Items.Cache.GetStatus(item) == PXEntryStatus.Notchanged)
				{
					keys.Add(string.Format("{0}.{1}", item.ProjectTaskID, item.InventoryID));
				}
			}

			foreach (PMProjectStatusEx item in Items.Cache.Updated)
			{
				string key = string.Format("{0}.{1}", item.ProjectTaskID, item.InventoryID);

				if (keys.Contains(key))
				{
					validationFailed = true;
					Items.Cache.RaiseExceptionHandling<PMProjectStatusEx.inventoryID>(
									item, Items.Cache.GetValueExt<PMProjectStatusEx.inventoryID>(item), new PXSetPropertyException(Messages.DuplicateProjectStatus_Task));
				}
				else
				{
					keys.Add(key);
				}
			}

			foreach (PMProjectStatusEx item in Items.Cache.Inserted)
			{
				string key = string.Format("{0}.{1}", item.ProjectTaskID, item.InventoryID);

				if (keys.Contains(key))
				{
					validationFailed = true;
					Items.Cache.RaiseExceptionHandling<PMProjectStatusEx.inventoryID>(
									item, Items.Cache.GetValueExt<PMProjectStatusEx.inventoryID>(item), new PXSetPropertyException(Messages.DuplicateProjectStatus_Task));
				}
				else
				{
					keys.Add(key);
				}
			}

			if (validationFailed)
			{
				throw new PXException(Messages.ValidationFailed);
			}
		}
		
		#region Local Types

        [Serializable]
		public partial class ProjectBalanceFilter : IBqlTable
		{
			#region ProjectID
			public abstract class projectID : PX.Data.IBqlField
			{
			}
			protected Int32? _ProjectID;
			[PXDefault]
            [Project(typeof(Where<PMProject.nonProject, Equal<False>>))]
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
			#region AccountGroupID
			public abstract class accountGroupID : PX.Data.IBqlField
			{
			}
			protected Int32? _AccountGroupID;
			[PXDefault]
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
		}

		#endregion
	}


	
}
