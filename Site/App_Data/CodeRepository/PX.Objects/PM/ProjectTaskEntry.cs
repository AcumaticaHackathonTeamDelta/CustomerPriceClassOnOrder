using System;
using System.Collections.Generic;
using PX.Data;
using System.Collections;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.CT;

namespace PX.Objects.PM
{
	public class ProjectTaskEntry : PXGraph<ProjectTaskEntry, PMTask>
	{
		#region DAC Attributes Override

		#region PMTask
		
		[Project(typeof(Where<PMProject.nonProject, NotEqual<True>, And<PMProject.isTemplate, NotEqual<True>>>), DisplayName = "Project ID", IsKey = true)]
		[PXParent(typeof(Select<PMProject, Where<PMProject.contractID, Equal<Current<PMTask.projectID>>>>))]
		[PXDefault]
		protected virtual void PMTask_ProjectID_CacheAttached(PXCache sender) { }


		[PXDimensionSelector(ProjectTaskAttribute.DimensionName,
			typeof(Search<PMTask.taskCD, Where<PMTask.projectID, Equal<Current<PMTask.projectID>>>>),
			typeof(PMTask.taskCD),
			typeof(PMTask.taskCD), typeof(PMTask.locationID), typeof(PMTask.description), typeof(PMTask.status), DescriptionField = typeof(PMTask.description))]
		[PXDBString(IsUnicode = true, IsKey = true, InputMask = "")]
		[PXDefault()]
		[PXUIField(DisplayName = "Task ID", Visibility = PXUIVisibility.SelectorVisible)]
		protected virtual void PMTask_TaskCD_CacheAttached(PXCache sender) { }
		
		#endregion
        
		#region PMProjectStatusEx
        [PXDefault(typeof(PMTask.projectID))]
        [PXDBInt()]
        protected virtual void PMProjectStatusEx_ProjectID_CacheAttached(PXCache sender)
        {
        }

        [PXDefault(typeof(PMTask.taskID))]
        [PXDBInt()]
		[PXSelector(typeof(Search<PMTask.taskID>))]
        protected virtual void PMProjectStatusEx_ProjectTaskID_CacheAttached(PXCache sender)
        {
        }

        [PXDefault]
        [AccountGroupAttribute()]
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

        #region PMDetail
        
        [PXDBInt()]
        [PXDBDefault(typeof(PMTask.projectID))]
        protected virtual void PMDetail_ContractID_CacheAttached(PXCache sender) { }

        [PXDBInt()]
        [PXDBDefault(typeof(PMTask.taskID))]
        [PXParent(typeof(Select<PMTask, Where<PMTask.projectID, Equal<Current<PMDetail.contractID>>, And<PMTask.taskID, Equal<Current<PMDetail.taskID>>>>>))]
        protected virtual void PMDetail_TaskID_CacheAttached(PXCache sender) { }

		[PXDBInt()]
		[PXDBDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void PMDetail_ContractItemID_CacheAttached(PXCache sender) { }

		[PXDefault(ResetUsageOption.Never)]
		[PXUIField(DisplayName = "Reset Usage")]
		[PXDBString(1, IsFixed = true)]
		[ResetUsageOption.ListForProjectAttribute()]
        protected virtual void PMDetail_ResetUsage_CacheAttached(PXCache sender) { }

		[PXDBString(1, IsFixed = true)]
		[PMAccountSource.RecurentList()]
		[PXDefault(PMAccountSource.None)]
		[PXUIField(DisplayName = "Account Source", Required = true)]
		protected virtual void PMDetail_AccountSource_CacheAttached(PXCache sender)
		{
		}
		[Account(DisplayName = "Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		protected virtual void PMDetail_AccountID_CacheAttached(PXCache sender)
		{
		}
		[PMRecurentBillSubAccountMask]
		protected virtual void PMDetail_SubMask_CacheAttached(PXCache sender)
		{
		}
		[SubAccount(typeof(PMDetail.accountID), DisplayName = "Subaccount", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		protected virtual void PMDetail_SubID_CacheAttached(PXCache sender)
		{
		}

		[PXDBInt(MinValue = 1)]
		[PXDefault(1, PersistingCheck = PXPersistingCheck.Null)]
		protected virtual void PMDetail_RevID_CacheAttached(PXCache sender) { }

		[PXDBInt(IsKey = true)]
		[PXLineNbr(typeof(PMTask.lineCtr))]
		[PXUIField(DisplayName = "Line Nbr.", Visible = false)]
		protected virtual void PMDetail_LineNbr_CacheAttached(PXCache sender) { }
        #endregion
		
        #endregion

		#region Views/Selects

		public PXSelectJoin<PMTask, LeftJoin<PMProject, On<PMTask.projectID, Equal<PMProject.contractID>>>, Where<PMProject.nonProject, Equal<False>, And<PMProject.isTemplate, Equal<False>>>> Task;

		[PXViewName(Messages.ProjectTask)]
		public PXSelect<PMTask, Where<PMTask.taskID, Equal<Current<PMTask.taskID>>>> TaskProperties;
        public PXSelect<PMDetail,
            Where<PMDetail.contractID, Equal<Current<PMTask.projectID>>,
            And<PMDetail.taskID, Equal<Current<PMTask.taskID>>>>> BillingItems;
       	[PXVirtualDAC]
        public ProjectStatusSelect<PMProjectStatusEx, Where<PMProjectStatusEx.accountGroupID, IsNotNull>, OrderBy<Asc<PMProjectStatusEx.sortOrder>>> ProjectStatus;
		
		[PXViewName(Messages.TaskAnswers)]
		public CRAttributeList<PMTask> Answers;

		[PXFilterable]
		[PXViewName(Messages.Activities)]
		[CRReference(typeof(Select<Customer, Where<Customer.bAccountID, Equal<Current<PMTask.customerID>>>>))]
		public ProjectTaskActivities Activities;

       	public PXSetup<PMSetup> Setup;
		public PXSetup<Company> CompanySetup;
		[PXViewName(Messages.Project)]
		public PXSelect<PMProject, Where<PMProject.contractID, Equal<Current<PMTask.projectID>>>> Project;

		public PXSelect<PMHistory2Accum> History2;
		
        public virtual IEnumerable projectStatus()
        {
            if (Task.Current == null)
            {
                yield break;
            }

            Dictionary<string, PMProjectStatusEx> cachedItems = new Dictionary<string, PMProjectStatusEx>();

            bool isDirty = false;

            int cxMax = 0;
            foreach (PMProjectStatusEx item in ProjectStatus.Cache.Cached)
            {
                cxMax = Math.Max(cxMax, item.LineNbr.Value);
                string key = string.Format("{0}.{1}.{2}.{3}", item.AccountGroupID, item.ProjectID, item.ProjectTaskID, item.InventoryID.GetValueOrDefault());

                if (!cachedItems.ContainsKey(key))
                    cachedItems.Add(key, item);

                if (ProjectStatus.Cache.GetStatus(item) == PXEntryStatus.Inserted ||
                    ProjectStatus.Cache.GetStatus(item) == PXEntryStatus.Updated ||
                    ProjectStatus.Cache.GetStatus(item) == PXEntryStatus.Notchanged ||
                    ProjectStatus.Cache.GetStatus(item) == PXEntryStatus.Held)
                {

                    if (ProjectStatus.Cache.GetStatus(item) == PXEntryStatus.Inserted ||
                    ProjectStatus.Cache.GetStatus(item) == PXEntryStatus.Updated)
                    {
                        isDirty = true;
                    }

                    yield return item;
                }

            }

            PXSelectBase<PMProjectStatus> select = new PXSelectJoinGroupBy<PMProjectStatus,
                    InnerJoin<PMTask, On<PMTask.projectID, Equal<PMProjectStatus.projectID>, And<PMTask.taskID, Equal<PMProjectStatus.projectTaskID>>>,
                    InnerJoin<PMAccountGroup, On<PMProjectStatus.accountGroupID, Equal<PMAccountGroup.groupID>>>>,
                    Where<PMProjectStatus.projectID, Equal<Current<PMTask.projectID>>,
                    And<PMProjectStatus.projectTaskID, Equal<Current<PMTask.taskID>>>>,
                    Aggregate<GroupBy<PMProjectStatus.accountGroupID,
                    GroupBy<PMProjectStatus.projectID,
                    GroupBy<PMProjectStatus.projectTaskID,
                    GroupBy<PMProjectStatus.inventoryID,
                    Sum<PMProjectStatus.amount,
                    Sum<PMProjectStatus.qty,
                    Sum<PMProjectStatus.revisedAmount,
                    Sum<PMProjectStatus.revisedQty,
                    Sum<PMProjectStatus.actualAmount,
                    Sum<PMProjectStatus.actualQty>>>>>>>>>>>, OrderBy<Asc<PMAccountGroup.sortOrder>>>(this);

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
                    item = (PMProjectStatusEx)ProjectStatus.Cache.Insert(item);
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
	                PMProjectStatus rowDetail = (PMProjectStatus)PXSelect<
		                PMProjectStatus
		                , Where<
			                PMProjectStatus.isProduction, Equal<True>
							, And<PMProjectStatus.projectID, Equal<Required<PMProjectStatus.projectID>>
								, And<PMProjectStatus.projectTaskID, Equal<Required<PMProjectStatus.projectTaskID>>
									, And<PMProjectStatus.inventoryID, Equal<Required<PMProjectStatus.inventoryID>>
										, And<PMProjectStatus.accountGroupID, Equal<Required<PMProjectStatus.accountGroupID>>>
										>
									>
								>
							>
						>.Select(this, row.ProjectID, row.ProjectTaskID, row.InventoryID, row.AccountGroupID);

					if (rowDetail != null)
						item.IsProduction = true;
					item.TaskStatus = task.Status;
                    item.Type = ag.Type;
                    switch (ag.Type)
                    {
                        case AccountType.Asset:
                            item.SortOrder = 1;
                            break;
                        case AccountType.Liability:
                            item.SortOrder = 2;
                            break;
                        case AccountType.Income:
                            item.SortOrder = 3;
                            break;
                        case AccountType.Expense:
                            item.SortOrder = 4;
                            break;
                    }
                    ProjectStatus.Cache.SetStatus(item, PXEntryStatus.Held);

                    yield return item;
                }
            }

            ProjectStatus.Cache.IsDirty = isDirty;
        }

		#endregion
				
		#region	Actions/Buttons
		
		public PXAction<PMTask> viewBalance;
		[PXUIField(DisplayName = Messages.ViewBalance, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntry)]
		public IEnumerable ViewBalance(PXAdapter adapter)
		{
			if (ProjectStatus.Current != null)
			{
				ProjectBalanceByPeriodEntry graph = PXGraph.CreateInstance<ProjectBalanceByPeriodEntry>();
				ProjectBalanceByPeriodEntry.ProjectBalanceFilter filter = new ProjectBalanceByPeriodEntry.ProjectBalanceFilter();
				filter.AccountGroupID = ProjectStatus.Current.AccountGroupID;
				filter.ProjectID = ProjectStatus.Current.ProjectID;
				filter.ProjectTaskID = ProjectStatus.Current.ProjectTaskID;
				filter.InventoryID = ProjectStatus.Current.InventoryID;
                
				graph.Filter.Insert(filter);

                throw new PXPopupRedirectException(graph, Messages.ProjectBalanceEntry + " - " + Messages.ViewBalance, true);
			}
			return adapter.Get();
		}

		public PXAction<PMTask> viewTransactions;
		[PXUIField(DisplayName = Messages.ViewTransactions, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.Inquiry)]
		public IEnumerable ViewTransactions(PXAdapter adapter)
		{
			if (ProjectStatus.Current != null)
			{
				TransactionInquiry target = PXGraph.CreateInstance<TransactionInquiry>();
			    target.Filter.Insert(new TransactionInquiry.TranFilter());
				target.Filter.Current.AccountGroupID = ProjectStatus.Current.AccountGroupID;
				target.Filter.Current.ProjectID = ProjectStatus.Current.ProjectID;
				target.Filter.Current.ProjectTaskID = ProjectStatus.Current.ProjectTaskID;
				if (ProjectStatus.Current.InventoryID != null && !ProjectDefaultAttribute.IsNonProject(this, ProjectStatus.Current.InventoryID))
					target.Filter.Current.InventoryID = ProjectStatus.Current.InventoryID;

				throw new PXPopupRedirectException(target, Messages.TransactionInquiry + " - " + Messages.ViewTransactions, false);
			}
			return adapter.Get();
		}

		public PXAction<PMTask> autoBudget;
		[PXUIField(DisplayName = Messages.AutoBudget, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable AutoBudget(PXAdapter adapter)
		{
			if (Task.Current != null)
			{
				this.Save.Press();

				PXLongOperation.StartOperation(this, delegate()
				{
					RunAutoBudget();

				});
			}

			return adapter.Get();

		}

		public virtual void RunAutoBudget()
		{
			AutoBudgetWorkerProcess wp = PXGraph.CreateInstance<AutoBudgetWorkerProcess>();
			List<AutoBudgetWorkerProcess.Balance> list = wp.Run(Task.Current);

			foreach (AutoBudgetWorkerProcess.Balance b in list)
			{
				PMProjectStatusEx ps = new PMProjectStatusEx();
				ps.AccountGroupID = b.AccountGroupID;
				ps.InventoryID = b.InventoryID;
				ps.ProjectID = Task.Current.ProjectID;
				ps.ProjectTaskID = Task.Current.TaskID;
				ps.Amount = b.Amount;
				ps.RevisedAmount = b.Amount;
				ps.Qty = b.Quantity;
				ps.RevisedQty = b.Quantity;
				if ( b.Quantity != 0 )
					ps.Rate = b.Amount / b.Quantity;

				bool found = false;
				foreach (PMProjectStatusEx item in ProjectStatus.Select())
				{
					if (item.ProjectID == ps.ProjectID &&
						item.ProjectTaskID == ps.ProjectTaskID &&
						item.AccountGroupID == ps.AccountGroupID &&
						item.InventoryID == ps.InventoryID)
					{
						if (Task.Current.IsActive != true)
						{
							item.Amount = b.Amount;
							item.Qty = b.Quantity;
						}
						
						item.RevisedAmount = b.Amount;
						item.RevisedQty = b.Quantity;
						if (b.Quantity != 0)
							item.Rate = b.Amount / b.Quantity;
						ProjectStatus.Update(item);

						found = true;
					}
				}

				if (!found)
				{
					PMAccountGroup ag = PXSelect<PMAccountGroup, Where<PMAccountGroup.groupID, Equal<Required<PMAccountGroup.groupID>>>>.Select(this, ps.AccountGroupID);
					ps.Type = ag.Type;
					switch (ag.Type)
					{
						case AccountType.Asset:
							ps.SortOrder = 1;
							break;
						case AccountType.Liability:
							ps.SortOrder = 2;
							break;
						case AccountType.Income:
							ps.SortOrder = 3;
							break;
						case AccountType.Expense:
							ps.SortOrder = 4;
							break;
					}
					this.FieldVerifying.AddHandler<PMProjectStatusEx.inventoryID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });
					ProjectStatus.Insert(ps);
				}
			}

			this.Save.Press();
		}
		#endregion		 

		public ProjectTaskEntry()
		{
			if (Setup.Current == null)
			{
				throw new PXException(Messages.SetupNotConfigured);
			}

			Activities.GetNewEmailAddress =
					() =>
						{
							PMProject current = Project.Select();
							if (current != null)
							{
								Contact customerContact = PXSelectJoin<Contact, InnerJoin<BAccount, On<BAccount.defContactID, Equal<Contact.contactID>>>, Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>.Select(this, current.CustomerID);

								if (customerContact != null && !string.IsNullOrWhiteSpace(customerContact.EMail))
									return new Email(customerContact.DisplayName, customerContact.EMail);
							}
							return Email.Empty;
						};
		}
               
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
        
        protected virtual void PMProjectStatusEx_AccountGroupID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            PMProjectStatusEx row = e.Row as PMProjectStatusEx;
            if (row != null)
            {
                if (string.IsNullOrEmpty(row.Type))
                {
                    PMAccountGroup ag = PXSelect<PMAccountGroup, Where<PMAccountGroup.groupID, Equal<Required<PMAccountGroup.groupID>>>>.Select(this, row.AccountGroupID);
                    if (ag != null)
                    {
                        row.Type = ag.Type;
                    }
                }
            }
        }

		protected virtual void PMProjectStatusEx_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			PMProjectStatusEx row = e.Row as PMProjectStatusEx;
			if (row == null) return;

			PXUIFieldAttribute.SetEnabled<PMProjectStatusEx.uOM>(sender, e.Row, row.ActualQty == 0);
		}


	
		protected virtual void PMTask_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			PMTask row = e.Row as PMTask;
			if (row != null)
			{
				viewTransactions.SetEnabled(row.IsActive == true || row.IsCompleted == true );

				PXUIFieldAttribute.SetEnabled<PMTask.visibleInGL>(sender, e.Row, Setup.Current.VisibleInGL == true);
				PXUIFieldAttribute.SetEnabled<PMTask.visibleInAP>(sender, e.Row, Setup.Current.VisibleInAP == true);
				PXUIFieldAttribute.SetEnabled<PMTask.visibleInAR>(sender, e.Row, Setup.Current.VisibleInAR == true);
				PXUIFieldAttribute.SetEnabled<PMTask.visibleInSO>(sender, e.Row, Setup.Current.VisibleInSO == true);
				PXUIFieldAttribute.SetEnabled<PMTask.visibleInPO>(sender, e.Row, Setup.Current.VisibleInPO == true);
				PXUIFieldAttribute.SetEnabled<PMTask.visibleInEP>(sender, e.Row, Setup.Current.VisibleInEP == true);
				PXUIFieldAttribute.SetEnabled<PMTask.visibleInIN>(sender, e.Row, Setup.Current.VisibleInIN == true);
				PXUIFieldAttribute.SetEnabled<PMTask.visibleInCA>(sender, e.Row, Setup.Current.VisibleInCA == true);

				PXUIFieldAttribute.SetVisible<PMProjectStatusEx.isProduction>(ProjectStatus.Cache, null, row.CompletedPctMethod != PMCompletedPctMethod.Manual);

				ProjectStatus.Cache.AllowInsert = row.IsCompleted == false && sender.GetStatus(e.Row) != PXEntryStatus.Inserted;
				ProjectStatus.Cache.AllowDelete = row.IsCompleted == false;
				ProjectStatus.Cache.AllowUpdate = row.IsCompleted == false;
			}
		}

		protected virtual void PMTask_CompletedPct_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			PMTask row = e.Row as PMTask;
			if (row != null && row.CompletedPctMethod != PMCompletedPctMethod.Manual)
			{
				PXSelectBase<PMProjectStatus> select = new PXSelectGroupBy<PMProjectStatus,
						   Where<PMProjectStatus.projectID, Equal<Required<PMTask.projectID>>,
						   And<PMProjectStatus.projectTaskID, Equal<Required<PMTask.taskID>>,
						   And<PMProjectStatus.isProduction, Equal<True>>>>,
						   Aggregate<
						   GroupBy<PMProjectStatus.accountGroupID,
						   GroupBy<PMProjectStatus.inventoryID,
						   GroupBy<PMProjectStatus.uOM,
						   Sum<PMProjectStatus.amount,
						   Sum<PMProjectStatus.qty,
						   Sum<PMProjectStatus.revisedAmount,
						   Sum<PMProjectStatus.revisedQty,
						   Sum<PMProjectStatus.actualAmount,
						   Sum<PMProjectStatus.actualQty>>>>>>>>>>>(this);

				PXResultset<PMProjectStatus> ps = select.Select(row.ProjectID, row.TaskID);


				if (ps != null)
				{
					double percentSum = 0;
					Int32 recordCount = 0;
					decimal actualAmount = 0;
					decimal revisedAmount = 0;
					foreach (PMProjectStatus item in ps)
					{

						if (row.CompletedPctMethod == PMCompletedPctMethod.ByQuantity && item.RevisedQty > 0)
						{
							recordCount ++;
							percentSum += Convert.ToDouble(100 * item.ActualQty / item.RevisedQty);
						}
						else if (row.CompletedPctMethod == PMCompletedPctMethod.ByAmount)
						{
							recordCount++;
							actualAmount += item.ActualAmount.GetValueOrDefault(0);
							revisedAmount += item.RevisedAmount.GetValueOrDefault(0);
						}
					}
					if (row.CompletedPctMethod == PMCompletedPctMethod.ByAmount)
						e.ReturnValue = revisedAmount == 0 ? 0 : Convert.ToDouble(100 * actualAmount / revisedAmount);
					else
						e.ReturnValue = Convert.ToInt32(percentSum) == 0 ? 0 : Convert.ToInt32(percentSum / recordCount);
					e.ReturnState = PXFieldState.CreateInstance(e.ReturnValue, typeof(decimal?), false, false, 0, 0, 0, 0, "CompletedPct", null, null, null, PXErrorLevel.Undefined, false, true, true, PXUIVisibility.Visible, null, null, null);
				}
			}
		}

		protected virtual void PMTask_IsActive_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			PMTask row = e.Row as PMTask;
			if (row != null && e.NewValue != null && ((bool)e.NewValue) == true)
			{
				PMProject project = Project.Select();
				if (project != null)
				{
					if (project.IsActive == false)
					{
						sender.RaiseExceptionHandling<PMTask.status>(e.Row, e.NewValue, new PXSetPropertyException(Warnings.ProjectIsNotActive, PXErrorLevel.Warning));
					}
				}
			}
		}
		
		protected virtual void PMTask_BillingID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			PMTask row = e.Row as PMTask;
			if (row != null)
			{
				PMTran allocatedTran = PXSelect<PMTran, Where<PMTran.billingID, NotEqual<Required<PMTran.billingID>>, 
					And<PMTran.allocated, Equal<True>,
					And<PMTran.projectID, Equal<Current<PMTask.projectID>>,
					And<PMTran.taskID, Equal<Current<PMTask.taskID>>>>>>>.SelectWindowed(this, 0, 1, e.NewValue);
				if (allocatedTran != null)
				{
                    sender.RaiseExceptionHandling<PMTask.billingID>(e.Row, e.NewValue, new PXSetPropertyException(Warnings.HasAllocatedTrans, PXErrorLevel.Warning, allocatedTran.BillingID));
				}
			}
		}

        
		protected virtual void PMTask_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			PMTask row = e.Row as PMTask;
			if (row == null)
				return;

			if (row.IsActive == true && row.IsCancelled == false)
			{
				throw new PXException(Messages.OnlyPlannedCanbeDeleted);
			}

			//validate that all child records can be deleted:

			PMTran tran = PXSelect<PMTran, Where<PMTran.projectID, Equal<Required<PMTask.projectID>>, And<PMTran.taskID, Equal<Required<PMTask.taskID>>>>>.SelectWindowed(this, 0, 1, row.ProjectID, row.TaskID);
			if ( tran != null )
			{
				throw new PXException(Messages.HasTranData);
			}

			PMTimeActivity activity = PXSelect<PMTimeActivity, Where<PMTimeActivity.projectID, Equal<Required<PMTask.projectID>>, And<PMTimeActivity.projectTaskID, Equal<Required<PMTask.taskID>>>>>.SelectWindowed(this, 0, 1, row.ProjectID, row.TaskID);
			if (activity != null)
			{
				throw new PXException(Messages.HasActivityData);
			}

            EP.EPTimeCardItem timeCardItem = PXSelect<EP.EPTimeCardItem, Where<EP.EPTimeCardItem.projectID, Equal<Required<PMTask.projectID>>, And<EP.EPTimeCardItem.taskID, Equal<Required<PMTask.taskID>>>>>.SelectWindowed(this, 0, 1, row.ProjectID, row.TaskID);
            if (timeCardItem != null)
            {
                throw new PXException(Messages.HasTimeCardItemData);
            }
		}

		protected virtual void PMTask_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			//PXParent dosnt work for VirtualDAC - thus delete manualy:

			foreach (PMProjectStatusEx child in ProjectStatus.Select())
			{
				ProjectStatus.Delete(child);
			}
		}

		protected virtual void PMTask_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			PMTask row = e.Row as PMTask;
			PMProject project = PXSelect<PMProject, Where<PMProject.contractID, Equal<Current<PMTask.projectID>>>>.Select(this);
			if (row != null && project != null)
			{
				row.CustomerID = project.CustomerID;
				row.BillingID = project.BillingID;
				row.AllocationID = project.AllocationID;
				row.DefaultAccountID = project.DefaultAccountID;
				row.DefaultSubID = project.DefaultSubID;
			}
		}

		protected virtual void PMTask_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			PMTask row = e.Row as PMTask;
			PMTask oldRow = e.OldRow as PMTask;
			if (row == null || oldRow == null) return;

			if (row.IsActive == true && oldRow.IsActive != true)
			{
				ActivateTask(row);
			}

			if (row.IsCompleted == true && oldRow.IsCompleted != true)
			{
				CompleteTask(row);
			}
		}
	

        protected virtual void PMDetail_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
        {
            PMDetail row = e.Row as PMDetail;
            if (row != null)
            {
                row.ContractID = Task.Current.ProjectID;

                #region Check for Uniqueness
                if (row.InventoryID.HasValue)
                {
                    PMDetail item = PXSelect<PMDetail,
                        Where<PMDetail.inventoryID, Equal<Required<InventoryItem.inventoryID>>,
                        And<PMDetail.contractID, Equal<Current<PMTask.projectID>>,
                        And<PMDetail.taskID, Equal<Current<PMTask.taskID>>>>>>.SelectWindowed(this, 0, 1, row.InventoryID);

                    if (item != null && item.PMDetailID != row.PMDetailID)
                    {
                        sender.RaiseExceptionHandling<PMDetail.inventoryID>(row, row.InventoryID, new PXException(CT.Messages.ItemNotUnique));
                        e.Cancel = true;
                    }
                }
                #endregion
            }
        }

        protected virtual void PMDetail_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
        {
            PMDetail row = e.Row as PMDetail;
            if (row != null)
            {
                #region Check for Uniqueness
                if (row.InventoryID.HasValue)
                {
                    PMDetail item = PXSelect<PMDetail,
                        Where<PMDetail.inventoryID, Equal<Required<InventoryItem.inventoryID>>,
                        And<PMDetail.contractID, Equal<Current<PMTask.projectID>>,
                        And<PMDetail.taskID, Equal<Current<PMTask.taskID>>>>>>.SelectWindowed(this, 0, 1, row.InventoryID);

                    if (item != null && item.PMDetailID != row.PMDetailID)
                    {
                        sender.RaiseExceptionHandling<PMDetail.inventoryID>(row, row.InventoryID, new PXException(CT.Messages.ItemNotUnique));
                        e.Cancel = true;
                    }
                }
                #endregion
            }
        }

        protected virtual void PMTask_ProjectID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            PMTask row = e.Row as PMTask;
            if (row != null)
            {
                sender.SetDefaultExt<PMTask.visibleInAP>(row);
                sender.SetDefaultExt<PMTask.visibleInAR>(row);
                sender.SetDefaultExt<PMTask.visibleInCA>(row);
                sender.SetDefaultExt<PMTask.visibleInCR>(row);
                sender.SetDefaultExt<PMTask.visibleInEP>(row);
                sender.SetDefaultExt<PMTask.visibleInGL>(row);
                sender.SetDefaultExt<PMTask.visibleInIN>(row);
                sender.SetDefaultExt<PMTask.visibleInPO>(row);
                sender.SetDefaultExt<PMTask.visibleInSO>(row);
                sender.SetDefaultExt<PMTask.customerID>(row);
                sender.SetDefaultExt<PMTask.locationID>(row);
                sender.SetDefaultExt<PMTask.rateTableID>(row);
            }
        }

        protected virtual void PMDetail_InventoryID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            OnPMDetailInventoryIDFieldUpdated(sender, e);
        }

        protected virtual void OnPMDetailInventoryIDFieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            PMDetail row = e.Row as PMDetail;

            InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, row.InventoryID);
            if (item != null && row != null)
            {
                row.UOM = item.SalesUnit;
                row.ItemFee = item.BasePrice;
                row.Description = item.Descr;

				if ( Task.Current != null && Task.Current.CustomerID != null)
				{
					Customer customer = PXSelect<Customer, Where<Customer.bAccountID, Equal<Current<PMTask.customerID>>>>.Select(this);
					if ( customer != null && !string.IsNullOrEmpty(customer.LocaleName) )
					{
						row.Description = PXDBLocalizableStringAttribute.GetTranslation(Caches[typeof(InventoryItem)], item, nameof(InventoryItem.Descr), customer.LocaleName);
					}
				}
				
				sender.SetDefaultExt<PMDetail.curyItemFee>(e.Row);
            }
        }

        protected virtual void PMDetail_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            PMDetail row = e.Row as PMDetail;
            if (row != null && Task.Current != null)
            {
                PXUIFieldAttribute.SetEnabled<PMDetail.included>(sender, row, Task.Current.IsActive != true);
				PXUIFieldAttribute.SetEnabled<PMDetail.accountID>(sender, e.Row, row.AccountSource != PMAccountSource.None);
				PXUIFieldAttribute.SetEnabled<PMDetail.subID>(sender, e.Row, row.AccountSource != PMAccountSource.None);
				PXUIFieldAttribute.SetEnabled<PMDetail.subMask>(sender, e.Row, row.AccountSource != PMAccountSource.None);
            }
        }

		#endregion

		public override void Persist()
		{
			ValidateUniqueFields();

			using (PXTransactionScope ts = new PXTransactionScope())
			{
				base.Persist();
				this.Persist(typeof (PMHistory2Accum), PXDBOperation.Insert);
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

			foreach (PMProjectStatusEx item in ProjectStatus.Select())
			{
				if (ProjectStatus.Cache.GetStatus(item) == PXEntryStatus.Notchanged)
				{
					keys.Add(string.Format("{0}.{1}", item.AccountGroupID, item.InventoryID));
				}
			}

			foreach (PMProjectStatusEx item in ProjectStatus.Cache.Updated)
			{
				string key = string.Format("{0}.{1}", item.AccountGroupID, item.InventoryID);

				if (keys.Contains(key))
				{
					validationFailed = true;
					ProjectStatus.Cache.RaiseExceptionHandling<PMProjectStatusEx.inventoryID>(
									item, ProjectStatus.Cache.GetValueExt<PMProjectStatusEx.inventoryID>(item), new PXSetPropertyException(Messages.DuplicateProjectStatus));
				}
				else
				{
					keys.Add(key);
				}
			}

			foreach (PMProjectStatusEx item in ProjectStatus.Cache.Inserted)
			{
				string key = string.Format("{0}.{1}", item.AccountGroupID, item.InventoryID);

				if (keys.Contains(key))
				{
					validationFailed = true;
					ProjectStatus.Cache.RaiseExceptionHandling<PMProjectStatusEx.inventoryID>(
									item, ProjectStatus.Cache.GetValueExt<PMProjectStatusEx.inventoryID>(item), new PXSetPropertyException(Messages.DuplicateProjectStatus));
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
						
		public virtual void ActivateTask(PMTask task)
		{
			if (task.StartDate == null)
				task.StartDate = Accessinfo.BusinessDate;
		}
				
		public virtual void CompleteTask(PMTask task)
		{
			task.EndDate = Accessinfo.BusinessDate;
			task.CompletedPct = Math.Max(100, task.CompletedPct.GetValueOrDefault());
		}

		protected virtual void RunAutoBudget(PMTask task)
		{
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
				tran.AccountID = item.InventoryID != null ? item.SalesAcctID : null;
				tran.Amount = ps.RevisedAmount;
				tran.Qty = ps.RevisedQty;
				tran.UOM = ps.UOM;
				tran.BAccountID = task.CustomerID;
				tran.LocationID = task.LocationID;
				tran.Billable = true;
				tran.BillableQty = ps.RevisedQty;
				tran.Date = FinPeriodIDAttribute.PeriodEndDate(this, ps.PeriodID);
				tran.StartDate = tran.Date;
				tran.EndDate = tran.Date;
				tran.FinPeriodID = ps.PeriodID;
				tran.TranPeriodID = ps.PeriodID;

				trans.Add(tran);			
			}


		}
	}
}
