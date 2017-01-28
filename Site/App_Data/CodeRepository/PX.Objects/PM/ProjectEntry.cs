using System;
using System.Collections.Generic;
using PX.Common;
using PX.Data;
using PX.Objects.CS;
using System.Collections;
using PX.Objects.GL;
using PX.Objects.AR;
using PX.Objects.IN;
using PX.Objects.CM;
using PX.Objects.EP;
using PX.Objects.CR;
using PX.Objects.CT;

namespace PX.Objects.PM
{   
    [Serializable]
	public class ProjectEntry : PXGraph<ProjectEntry, PMProject>
	{
        #region Inner DACs
        [PXCacheName(Messages.SelectedTask)]
        [Serializable]
        public partial class SelectedTask: PMTask
        {
            #region ProjectID
			public new abstract class projectID : PX.Data.IBqlField
			{
			}

            [PXDBInt(IsKey = true)]
            public override Int32? ProjectID
            {
                get
                {
                    return _ProjectID;
                }
                set
                {
                    _ProjectID = value;
                }
            }
            #endregion
            #region TaskCD

			public new abstract class taskCD : PX.Data.IBqlField
			{
			}
			[PXDimension(ProjectTaskAttribute.DimensionName)]
            [PXDBString(30, IsUnicode = true, IsKey = true)]
            [PXUIField(DisplayName = "Task ID")]
            public override String TaskCD
            {
                get
                {
                    return _TaskCD;
                }
                set
                {
                    _TaskCD = value;
                }
            }
            #endregion

			public new abstract class taskID : PX.Data.IBqlField
			{
			}

			public new abstract class description : PX.Data.IBqlField
			{
			}


        }

        #endregion

		#region DAC Attributes Override
        #region PMTask
		
        [PXDefault(typeof(PMProject.defaultSubID), PersistingCheck = PXPersistingCheck.Nothing)]
        [SubAccount(DisplayName = "Default Subaccount", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description), Visible = false)]
        protected virtual void PMTask_DefaultSubID_CacheAttached(PXCache sender)
        {
        }

        #endregion

        #region PMProjectStatusEx

		[PXDefault]
        [PXDBInt]
        protected virtual void PMProjectStatusEx_ProjectID_CacheAttached(PXCache sender)
        {
        }

		[PXDefault]
		[PXDBInt]
        protected virtual void PMProjectStatusEx_ProjectTaskID_CacheAttached(PXCache sender)
        {
        }
        #endregion

        #region EPEquipmentRate

        [PXDBInt(IsKey = true)]
		[PXDefault]
		[PXUIField(DisplayName = "Equipment ID")]
		[PXSelector(typeof(EPEquipment.equipmentID), DescriptionField = typeof(EPEquipment.description), SubstituteKey=typeof(EPEquipment.equipmentCD))]
		protected virtual void EPEquipmentRate_EquipmentID_CacheAttached(PXCache sender)
		{
		}


		[PXParent(typeof(Select<PMProject, Where<PMProject.contractID, Equal<Current<EPEquipmentRate.projectID>>>>))]
		[PXDBLiteDefault(typeof(PMProject.contractID))]
		[PXDBInt(IsKey = true)]
		protected virtual void EPEquipmentRate_ProjectID_CacheAttached(PXCache sender)
		{
		}

		[PXDBInt()]
		[PXUIField(DisplayName = "Run Rate for Project")]
		[PXDimensionSelector(InventoryAttribute.DimensionName, typeof(Search<InventoryItem.inventoryID, Where<InventoryItem.itemType, Equal<INItemTypes.nonStockItem>, And<Match<Current<AccessInfo.userName>>>>>), typeof(InventoryItem.inventoryCD))]
		protected virtual void EPEquipmentRate_RunRateItemID_CacheAttached(PXCache sender)
		{
		}

		[PXDBInt()]
		[PXUIField(DisplayName = "Setup Rate for Project")]
		[PXDimensionSelector(InventoryAttribute.DimensionName, typeof(Search<InventoryItem.inventoryID, Where<InventoryItem.itemType, Equal<INItemTypes.nonStockItem>, And<Match<Current<AccessInfo.userName>>>>>), typeof(InventoryItem.inventoryCD))]
		protected virtual void EPEquipmentRate_SetupRateItemID_CacheAttached(PXCache sender)
		{
		}

		[PXDBInt()]
		[PXUIField(DisplayName = "Suspend Rate for Project")]
		[PXDimensionSelector(InventoryAttribute.DimensionName, typeof(Search<InventoryItem.inventoryID, Where<InventoryItem.itemType, Equal<INItemTypes.nonStockItem>, And<Match<Current<AccessInfo.userName>>>>>), typeof(InventoryItem.inventoryCD))]
		protected virtual void EPEquipmentRate_SuspendRateItemID_CacheAttached(PXCache sender)
		{
		}

        protected virtual void EPEquipmentRate_EquipmentID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            if (this.IsMobile && e.Row != null)
            {
                EPEquipmentRate row = e.Row as EPEquipmentRate;
                if ((int?)e.OldValue != row.EquipmentID)
                {
                    sender.SetDefaultExt<EPEquipmentRate.runRate>(row);
                    sender.SetDefaultExt<EPEquipmentRate.setupRate>(row);
                    sender.SetDefaultExt<EPEquipmentRate.suspendRate>(row);
                }
            }
        }

        protected virtual void EPEquipmentRate_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
        {
            EPEquipmentRate row = e.Row as EPEquipmentRate;
            if (row != null && this.IsMobile)
            {
                var toDelete = new List<object>();
                foreach (var item in sender.Inserted)
                {
                    if (((EPEquipmentRate)item).EquipmentID == null)
                    {
                        toDelete.Add(item);
                    }
                }

                foreach (var item in toDelete)
                {
                    sender.Delete(item);
                }
            }
        }

        protected virtual void EPEquipmentRate_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
        {
            EPEquipmentRate row = e.Row as EPEquipmentRate;
            if (row != null && this.IsMobile)
            {
                var toDelete = new List<object>();
                foreach (var item in sender.Inserted)
                {
                    if (((EPEquipmentRate)item).EquipmentID == row.EquipmentID)
                    {
                        toDelete.Add(item);
                    }
                }

                foreach (var item in toDelete)
                {
                    sender.Delete(item);
                }
            }
        }

        protected virtual void EPEquipmentRate_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            EPEquipmentRate row = e.Row as EPEquipmentRate;
            if (row != null && this.IsMobile)
            {
                if (row.EquipmentID == null)
                {
                    sender.Delete(row);
                    e.Cancel = true;
                }
            }
        }

        #endregion

        [PXDBString(1, IsFixed = true)]
		[BillingType.ListForProject()]
		[PXUIField(DisplayName = "Billing Period")]
		protected virtual void ContractBillingSchedule_Type_CacheAttached(PXCache sender)
		        {
		        }


		#region EPEmployeeContract
		[PXDBInt(IsKey = true)]
		[PXDefault]
		[PXUIField(DisplayName = "Employee ID", Visibility = PXUIVisibility.Visible)]
		[EP.PXEPEmployeeSelector()]
		[PXCheckUnique(Where = typeof(Where<EPEmployeeContract.contractID, Equal<Current<EPEmployeeContract.contractID>>>))]
		protected virtual void EPEmployeeContract_EmployeeID_CacheAttached(PXCache sender)
	    {
	    }

		[PXDBInt(IsKey = true)]
		[PXDBDefault(typeof(PMProject.contractID))]
		[PXParent(typeof(Select<PMProject, Where<PMProject.contractID, Equal<Current<EPEmployeeContract.contractID>>>>))]
		[PXCheckUnique(Where = typeof(Where<EPEmployeeContract.employeeID, Equal<Current<EPEmployeeContract.employeeID>>>))]
		protected virtual void EPEmployeeContract_ContractID_CacheAttached(PXCache sender)
		{
		}
		#endregion

		#region EPApproval Cache Attached - Approvals Fields
		[PXDBDate()]
		[PXDefault(typeof(PMProject.startDate), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void EPApproval_DocDate_CacheAttached(PXCache sender)
		{
		}

		[PXDBInt()]
		[PXDefault(typeof(PMProject.customerID), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void EPApproval_BAccountID_CacheAttached(PXCache sender)
		{
		}

		[PXDBString(60, IsUnicode = true)]
		[PXDefault(typeof(PMProject.description), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void EPApproval_Descr_CacheAttached(PXCache sender)
		{
		}
		#endregion

		#endregion

		#region Views/Selects

		[PXViewName(Messages.Project)]
		public PXSelect<PMProject, Where<PMProject.baseType, Equal<PMProject.ProjectBaseType>, 
			And<PMProject.nonProject, Equal<False>, And<PMProject.isTemplate, Equal<False>,
			And<Match<Current<AccessInfo.userName>>>>>>> Project;
		public PXSelect<PMProject, Where<PMProject.contractID, Equal<Current<PMProject.contractID>>>> ProjectProperties;

        public PXSelect<PX.Objects.CR.Standalone.EPEmployee> approver;

        [PXCopyPasteHiddenFields(typeof(ContractBillingSchedule.lastDate), typeof(ContractBillingSchedule.nextDate))]
        public PXSelect<CT.ContractBillingSchedule, Where<CT.ContractBillingSchedule.contractID, Equal<Current<PMProject.contractID>>>> Billing;
		[PXFilterable]
		[PXCopyPasteHiddenFields(typeof(PMTask.completedPct), typeof(PMTask.endDate))]
		public PXSelectJoin<PMTask, 
           LeftJoin<PMTaskTotal, On<PMTaskTotal.projectID, Equal<PMTask.projectID>, And<PMTaskTotal.taskID, Equal<PMTask.taskID>>>>,
            Where<PMTask.projectID, Equal<Current<PMProject.contractID>>>> Tasks;
		public PXSelectJoin<EPEmployeeContract,
			InnerJoin<CR.Standalone.EPEmployee, On<CR.Standalone.EPEmployee.bAccountID, Equal<EPEmployeeContract.employeeID>>>,
			Where<EPEmployeeContract.contractID, Equal<Current<PMProject.contractID>>>> EmployeeContract;
		public PXSelectJoin<EPContractRate
				, LeftJoin<IN.InventoryItem, On<IN.InventoryItem.inventoryID, Equal<EPContractRate.labourItemID>>
					, LeftJoin<EPEarningType, On<EPEarningType.typeCD, Equal<EPContractRate.earningType>>>
					>
				, Where<EPContractRate.employeeID, Equal<Optional<EPEmployeeContract.employeeID>>, And<EPContractRate.contractID, Equal<Optional<PMProject.contractID>>>>
				, OrderBy<Asc<EPContractRate.contractID>>
				> ContractRates;
		[PXFilterable]
		public PXSelectJoin<EPEquipmentRate, InnerJoin<EPEquipment, On<EPEquipmentRate.equipmentID, Equal<EPEquipment.equipmentID>>>, Where<EPEquipmentRate.projectID, Equal<Current<PMProject.contractID>>>> EquipmentRates;
		public PXSelect<PMAccountTask, Where<PMAccountTask.projectID, Equal<Current<PMProject.contractID>>>> Accounts;
		[PXCopyPasteHiddenView]
		public PXSelect<ARInvoice> Invoices;
		public PXFilter<TemplateSettingsFilter> TemplateSettings;

		[PXHidden]
		public PXSelect<PMDetail, Where<PMDetail.contractID, Equal<Current<PMTask.projectID>>>> BillingItems;

		public PXSetup<PMSetup> Setup;
		public PXSetup<Company> Company;


		[PXViewName(Messages.ProjectAnswers)]
		public CRAttributeList<PMProject> Answers;


		[PXFilterable]
		[PXViewName(Messages.Activities)]
		[CRReference(typeof(Select<Customer, Where<Customer.bAccountID, Equal<Current<PMProject.customerID>>>>))]
		public ProjectActivities Activities;

        [PXVirtualDAC]
        public PXSelect<PMProjectBalanceRecord, Where<PMProjectBalanceRecord.recordID, IsNotNull>, 
            OrderBy<Asc<PMProjectBalanceRecord.sortOrder>>> BalanceRecords;

        [PXVirtualDAC]
		public ProjectStatusSelect<PMProjectStatusEx, Where<PMProjectStatusEx.accountGroupID, IsNotNull>, OrderBy<Asc<PMProjectStatusEx.sortOrder>>> ProjectStatus;
		[PXHidden]
		[PXCopyPasteHiddenView]
		public PXSelect<PMProjectStatus, Where<PMProjectStatus.projectID, Equal<Current<PMProject.contractID>>>> InternalProjectStatus;

        [PXFilterable]
        public PXSelectJoin<SelectedTask, LeftJoin<PMProject, On<SelectedTask.projectID, Equal<PMProject.contractID>>>, Where<SelectedTask.autoIncludeInPrj, NotEqual<True>, And<SelectedTask.projectID, Equal<Current<PMProject.templateID>>, Or<PMProject.nonProject, Equal<True>>>>> TasksForAddition;

        public PXSelect<CSAnswers, Where<CSAnswers.refNoteID, Equal<Required<PMTask.noteID>>>> Answer;

		protected IEnumerable tasksForAddition()
		{
			PXSelectBase<SelectedTask> select = new PXSelectJoin<SelectedTask, 
				LeftJoin<PMProject, On<SelectedTask.projectID, Equal<PMProject.contractID>>>, 
				Where<SelectedTask.autoIncludeInPrj, NotEqual<True>, 
				And<SelectedTask.projectID, Equal<Current<PMProject.templateID>>, 
				Or<PMProject.nonProject, Equal<True>>>>>(this);

			List<string> existingTasks = new List<string>();
			foreach (PMTask task in Tasks.Select())
			{
				if (string.IsNullOrEmpty(task.TaskCD))
					continue;

				existingTasks.Add(task.TaskCD.ToUpperInvariant().Trim());
			}

			List<SelectedTask> taskToShow = new List<SelectedTask>();
			foreach (SelectedTask task in select.Select())
			{
				if ( !existingTasks.Contains(task.TaskCD.ToUpperInvariant().Trim()))
				{
					taskToShow.Add(task);
				}
			}

			return taskToShow;
		}

		[PXViewName(Messages.Approval)]
		public EPApprovalAutomation<PMProject, PMProject.approved, PMProject.rejected, PMProject.hold, PMSetup> Approval;

		[PXHidden]
		[PXCopyPasteHiddenView]
		public PXSelect<PMTimeActivity> ActivityDummy;

		public PXSelect<PMHistory2Accum> History2;

		#endregion
		
		#region Actions/Buttons
		
		public PXChangeID<PMProject, PMProject.contractCD> ChangeID;
		public PXAction<PMProject> action;
		[PXUIField(DisplayName = "Actions")]
		[PXButton]
		protected virtual IEnumerable Action(PXAdapter adapter,
		[PXInt]
		[PXIntList(new int[] { 1, 2 }, new string[] { "Persist", "Update" })]
		int? actionID,
		[PXBool]
		bool refresh,
		[PXString]
		string actionName
		)
		{
			List<PMProject> result = new List<PMProject>();
			if (actionName != null)
			{
				PXAction a = this.Actions[actionName];
				if (a != null)
					foreach (PXResult<PMProject> e in a.Press(adapter))
						result.Add(e);
			}
			else
				foreach (PMProject e in adapter.Get<PMProject>())
					result.Add(e);

			//if (refresh)
			//{
			//	foreach (PMProject order in result)
			//		Document.Search<POOrder.orderNbr>(order.OrderNbr, order.OrderType);
			//}
			switch (actionID)
			{
				case 1:
					Save.Press();
					break;
				case 2:
					break;
			}
			return result;
		}

		public PXAction<PMProject> viewTask;
		[PXUIField(DisplayName = Messages.ViewTask, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntry)]
		public IEnumerable ViewTask(PXAdapter adapter)
		{
			if (Tasks.Current != null && Project.Cache.GetStatus(this.Project.Current) != PXEntryStatus.Inserted)
			{
				if ( Tasks.Cache.GetStatus(Tasks.Current) == PXEntryStatus.Inserted || Tasks.Cache.GetStatus(Tasks.Current) == PXEntryStatus.Updated)
				{
					this.Save.Press();
				}

				ProjectTaskEntry graph = PXGraph.CreateInstance<ProjectTaskEntry>();
				graph.Task.Current = PXSelect<PMTask, Where<PMTask.projectID, Equal<Required<PMTask.projectID>>, And<PMTask.taskID, Equal<Required<PMTask.taskID>>>>>.Select(this, Tasks.Current.ProjectID, Tasks.Current.TaskID);

				throw new PXPopupRedirectException(graph, Messages.ProjectTaskEntry + " - " + Messages.ViewTask, true);
			}
			return adapter.Get();
		}


		public PXAction<PMProject> viewBalance;
		[PXUIField(DisplayName = Messages.ViewBalance, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntry)]
		public IEnumerable ViewBalance(PXAdapter adapter)
		{
            if (BalanceRecords.Current != null && BalanceRecords.Current.RecordID > 0)
            {
                ProjectBalanceEntry graph = PXGraph.CreateInstance<ProjectBalanceEntry>();
				graph.Filter.Current.AccountGroupID = BalanceRecords.Current.RecordID;
				graph.Filter.Current.ProjectID = Project.Current.ContractID;

            	
                throw new PXPopupRedirectException(graph, Messages.ProjectBalanceEntry + " - " + Messages.ViewBalance, true);
            }
			return adapter.Get();
		}

        public PXAction<PMProject> viewInvoice;
        [PXUIField(DisplayName = CT.Messages.ViewInvoice, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Enabled = true)]
        [PXButton(ImageKey = PX.Web.UI.Sprite.Main.Inquiry)]
        public virtual IEnumerable ViewInvoice(PXAdapter adapter)
        {
            if (Invoices.Current != null)
            {
                ARInvoiceEntry target = PXGraph.CreateInstance<ARInvoiceEntry>();
                target.Clear();
                target.Document.Current = Invoices.Current;
                throw new PXRedirectRequiredException(target, true, "ViewInvoice") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
            }

            return adapter.Get();
        }
		
		public PXAction<PMProject> bill;
        [PXUIField(DisplayName = Messages.Bill, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
		[PXButton(Tooltip = Messages.BillTip)]
        public virtual IEnumerable Bill(PXAdapter adapter)
        {
            if (CanBeBilled())
            {
				this.Save.Press();

                PXLongOperation.StartOperation(this, delegate() {

					PMBillEngine engine = PXGraph.CreateInstance<PMBillEngine>();
                    engine.Bill(Project.Current.ContractID, null, null);
				});
            }

            return adapter.Get();
        }

        public PXAction<PMProject> addTasks;
        [PXUIField(DisplayName = Messages.AddCommonTasks, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select, Enabled = false)]
        [PXButton(ImageKey = PX.Web.UI.Sprite.Main.Process)]
        public virtual IEnumerable AddTasks(PXAdapter adapter)
        {
            foreach (SelectedTask task in TasksForAddition.Cache.Updated)
            {
                if (task.Selected == true)
                {
                    CopyTask(task, (int)ProjectProperties.Current.ContractID);
	                task.Selected = false;
                }
            }
            return adapter.Get();
        }

		public PXAction<PMProject> createTemplate;
		[PXUIField(DisplayName = Messages.CreateTemplate, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select, Enabled = false)]
		[PXButton]
		public virtual IEnumerable CreateTemplate(PXAdapter adapter)
		{
			PMProject templ = new PMProject();

			bool isAutonumbered = true;

			Dimension dm = PXSelect<Dimension, Where<Dimension.dimensionID, Equal<Required<Dimension.dimensionID>>>>.Select(this, ProjectAttribute.DimensionName);
			if (dm.NumberingID == null)
			{
				isAutonumbered = false;
			}
			else
			{
				bool hasAutonumber = false;
				foreach (Segment segment in PXSelect<Segment, Where<Segment.dimensionID, Equal<Required<Segment.dimensionID>>>>.Select(this, ProjectAttribute.DimensionName))
				{
					if (segment.AutoNumber == true)
					{
						hasAutonumber = true;
						break;
					}
				}

				if ( !hasAutonumber )
					isAutonumbered = false;
			}


			if (!isAutonumbered)
			{
				if (TemplateSettings.AskExt() == WebDialogResult.OK && !string.IsNullOrEmpty(TemplateSettings.Current.TemplateID))
				{
					templ.ContractCD = TemplateSettings.Current.TemplateID;
				}
				else
				{
					return adapter.Get();
				}
			}

			this.Save.Press();
			PMProject project = Project.Current;

			TemplateMaint graph = CreateInstance<TemplateMaint>();
			templ = graph.Project.Insert(templ);

			templ.Description = project.Description;
			templ.DefaultAccountID = project.DefaultAccountID;
			templ.DefaultSubID = project.DefaultSubID;
			templ.CalendarID = project.CalendarID;
			templ.RestrictToEmployeeList = project.RestrictToEmployeeList;
			templ.RestrictToResourceList = project.RestrictToResourceList;
			templ.CuryID = project.CuryID;
			templ.RateTypeID = project.RateTypeID;
			templ.AllowOverrideCury = project.AllowOverrideCury;
			templ.AllowOverrideRate = project.AllowOverrideRate;
			templ.AllocationID = project.AllocationID;
			templ.BillingID = project.BillingID;
			templ.ApproverID = project.ApproverID;
			templ.RateTableID = project.RateTableID;
			
			templ.VisibleInAP = project.VisibleInAP;
			templ.VisibleInGL = project.VisibleInGL;
			templ.VisibleInAR = project.VisibleInAR;
			templ.VisibleInSO = project.VisibleInSO;
			templ.VisibleInPO = project.VisibleInPO;
			templ.VisibleInEP = project.VisibleInEP;
			templ.VisibleInIN = project.VisibleInIN;
			templ.VisibleInCA = project.VisibleInCA;
			templ.VisibleInCR = project.VisibleInCR;

			ContractBillingSchedule billing = PXSelect<ContractBillingSchedule, Where<ContractBillingSchedule.contractID, Equal<Current<PMProject.contractID>>>>.SelectSingleBound(this, new object[] { project });
			if (billing != null)
			{
				graph.Billing.Current.Type = billing.Type;
			}

			Dimension skey = PXSelect<Dimension, Where<Dimension.dimensionID, Equal<Required<Dimension.dimensionID>>>>.Select(this, ProjectTaskAttribute.DimensionName);
			bool isAutoNumbered = skey != null && skey.NumberingID != null;
			Dictionary<int, int> taskIDs = new Dictionary<int, int>();
			foreach (PMTask task in PXSelect<PMTask, Where<PMTask.projectID, Equal<Current<PMProject.contractID>>>>.SelectMultiBound(this, new object[] { project }))
			{
				PMTask dst = graph.Tasks.Insert(new PMTask { TaskCD = !isAutoNumbered ? task.TaskCD : null, ProjectID = templ.ContractID });
				dst.BillingID = task.BillingID;
				dst.AllocationID = task.AllocationID;
				dst.Description = task.Description;
				dst.ApproverID = task.ApproverID;
				dst.TaxCategoryID = task.TaxCategoryID;
				dst.BillingOption = task.BillingOption;
				dst.DefaultAccountID = task.DefaultAccountID;
				dst.DefaultSubID = dst.DefaultSubID;
				dst.VisibleInGL = task.VisibleInGL;
				dst.VisibleInAP = task.VisibleInAP;
				dst.VisibleInAR = task.VisibleInAR;
				dst.VisibleInSO = task.VisibleInSO;
				dst.VisibleInPO = task.VisibleInPO;
				dst.VisibleInEP = task.VisibleInEP;
				dst.VisibleInIN = task.VisibleInIN;
				dst.VisibleInCA = task.VisibleInCA;
				dst.VisibleInCR = task.VisibleInCR;
				dst.IsActive = task.IsActive ?? false;

				taskIDs.Add(task.TaskID.Value, dst.TaskID.Value);

				PXSelectBase<PMProjectStatus> select = new PXSelectGroupBy<PMProjectStatus,
					Where<PMProjectStatus.projectID, Equal<Required<PMTask.projectID>>,
					And<PMProjectStatus.projectTaskID, Equal<Required<PMTask.taskID>>>>,
					Aggregate<GroupBy<PMProjectStatus.accountGroupID,
					GroupBy<PMProjectStatus.inventoryID,
					Sum<PMProjectStatus.amount,
					Sum<PMProjectStatus.qty,
					Sum<PMProjectStatus.revisedAmount,
					Sum<PMProjectStatus.revisedQty,
					Sum<PMProjectStatus.actualAmount,
					Sum<PMProjectStatus.actualQty>>>>>>>>>>(this);
				foreach (PMProjectStatus st in select.Select(task.ProjectID, task.TaskID))
				{
					PMProjectStatus dstst = graph.Status.Insert(new PMProjectStatus());
					dstst.ProjectID = dst.ProjectID;
					dstst.ProjectTaskID = dst.TaskID;
					dstst.AccountGroupID = st.AccountGroupID;
					dstst.InventoryID = st.InventoryID;
					dstst.Qty = st.Qty;
					dstst.Rate = st.Rate;
					dstst.Amount = st.Amount;
					dstst.Description = st.Description;
					dstst.UOM = st.UOM;
					dstst.IsProduction = st.IsProduction ?? false;
					dstst.IsTemplate = true;
					dstst.PeriodID = string.Empty;
				}

				foreach (PMDetail detail in PXSelect<PMDetail, Where<PMDetail.contractID, Equal<Required<PMTask.projectID>>, And<PMDetail.taskID, Equal<Required<PMTask.taskID>>>>>.Select(this, task.ProjectID, task.TaskID))
				{
					PMDetail newDetail = new PMDetail();
					newDetail.ContractID = dst.ProjectID;
					newDetail.TaskID = dst.TaskID;
					newDetail.InventoryID = detail.InventoryID;
					newDetail.UOM = detail.UOM;
					newDetail.Description = detail.Description;
					newDetail.ItemFee = detail.ItemFee;
					newDetail.CuryItemFee = detail.CuryItemFee;
					newDetail.CuryInfoID = detail.CuryInfoID;
					newDetail.AccountSource = detail.AccountSource;
					newDetail.SubMask = detail.SubMask;
					newDetail.AccountID = detail.AccountID;
					newDetail.SubID = detail.SubID;
					newDetail.ResetUsage = detail.ResetUsage;
					newDetail.Included = detail.Included;

					graph.BillingItems.Insert(newDetail);
			}
			}

			foreach (EPEmployeeContract rate in PXSelect<EPEmployeeContract, Where<EPEmployeeContract.contractID, Equal<Current<PMProject.contractID>>>>.SelectMultiBound(this, new object[] { project }))
			{
				EPEmployeeContract dst = graph.EmployeeContract.Insert(new EPEmployeeContract());
				dst.EmployeeID = rate.EmployeeID;
			}

			foreach (EPContractRate rate in PXSelect<EPContractRate, Where<EPContractRate.contractID, Equal<Current<PMProject.contractID>>>>.SelectMultiBound(this, new object[] { project }))
			{
				EPContractRate dst = graph.ContractRates.Insert(new EPContractRate());
				dst.IsActive = rate.IsActive;
				dst.EmployeeID = rate.EmployeeID;
				dst.EarningType = rate.EarningType;
				dst.LabourItemID = rate.LabourItemID;
			}

			foreach (EPEquipmentRate equipment in PXSelect<EPEquipmentRate, Where<EPEquipmentRate.projectID, Equal<Current<PMProject.contractID>>>>.SelectMultiBound(this, new object[] { project }))
			{
				EPEquipmentRate dst = graph.EquipmentRates.Insert(new EPEquipmentRate());
				dst.IsActive = equipment.IsActive;
				dst.EquipmentID = equipment.EquipmentID;
				dst.RunRate = equipment.RunRate;
				dst.SuspendRate = equipment.SuspendRate;
				dst.SetupRate = equipment.SetupRate;
			}

			foreach (PMAccountTask acc in PXSelect<PMAccountTask, Where<PMAccountTask.projectID, Equal<Current<PMProject.contractID>>>>.SelectMultiBound(this, new object[] { project }))
			{
				if (taskIDs.ContainsKey(acc.TaskID.GetValueOrDefault()))
				{
					PMAccountTask dst = (PMAccountTask)graph.Accounts.Cache.Insert();
					dst.ProjectID = templ.ContractID;
					dst.AccountID = acc.AccountID;
					dst.TaskID = taskIDs[acc.TaskID.GetValueOrDefault()];
				}
			}

			throw new PXRedirectRequiredException(graph, "ProjectTemplate") { Mode = PXBaseRedirectException.WindowMode.Same };
		}

		public PXAction<PMProject> autoBudget;
		[PXUIField(DisplayName = Messages.AutoBudget, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
		[PXButton(Tooltip=Messages.AutoBudgetTip)]
		public virtual IEnumerable AutoBudget(PXAdapter adapter)
		{
			if (Project.Current != null)
			{
				this.Save.Press();

				PXLongOperation.StartOperation(this, delegate()
				{
					RunAutoBudget();

				});
			}

			return adapter.Get();

		}

		protected virtual void RunAutoBudget()
		{
			ProjectTaskEntry pte = PXGraph.CreateInstance<ProjectTaskEntry>();
			foreach (PMTask task in Tasks.Select())
			{
				pte.Clear();
				pte.Task.Current = PXSelect<PMTask, Where<PMTask.projectID, Equal<Required<PMTask.projectID>>, And<PMTask.taskID, Equal<Required<PMTask.taskID>>>>>.Select(this, task.ProjectID, task.TaskID);
				pte.RunAutoBudget();
			}
		}

		public PXAction<PMProject> hold;
		[PXUIField(DisplayName = "Hold", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		protected virtual IEnumerable Hold(PXAdapter adapter)
		{
			return adapter.Get();
		}

		#endregion
		
		public ProjectEntry()
		{
			if (Setup.Current == null)
			{
				throw new PXException(Messages.SetupNotConfigured);
			}

			action.AddMenuAction(ChangeID);
            BalanceRecords.Cache.AllowInsert = false;
            BalanceRecords.Cache.AllowUpdate = false;
            BalanceRecords.Cache.AllowDelete = false;

            Invoices.Cache.AllowDelete = false;
            Invoices.Cache.AllowInsert = false;
            Invoices.Cache.AllowUpdate = false;

            Activities.GetNewEmailAddress =
					() =>
					{
						PMProject current = Project.Current;
						if (current != null)
						{
							Contact customerContact = PXSelectJoin<Contact, InnerJoin<BAccount, On<BAccount.defContactID, Equal<Contact.contactID>>>, Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>.Select(this, current.CustomerID);

							if (customerContact != null && !string.IsNullOrWhiteSpace(customerContact.EMail))
								return new Email(customerContact.DisplayName, customerContact.EMail);
						}
						return Email.Empty;
					};
		}

		protected virtual IEnumerable invoices()
		{
			var s = new PXSelectGroupBy<ARTran, Where<ARTran.projectID, Equal<Current<Contract.contractID>>>, Aggregate<GroupBy<ARTran.tranType, GroupBy<ARTran.refNbr>>>>(this);

			foreach (ARTran tran in s.Select())
			{
				ARInvoice inv = PXSelect<ARInvoice, Where<ARInvoice.docType, Equal<Required<ARInvoice.docType>>, And<ARInvoice.refNbr, Equal<Required<ARInvoice.refNbr>>>>>.Select(this, tran.TranType, tran.RefNbr);
				yield return inv;
			}
		}
        
        public IEnumerable balanceRecords()
        {
            List<PMProjectBalanceRecord> asset = new List<PMProjectBalanceRecord>();
            List<PMProjectBalanceRecord> liability = new List<PMProjectBalanceRecord>();
            List<PMProjectBalanceRecord> income = new List<PMProjectBalanceRecord>();
            List<PMProjectBalanceRecord> expense = new List<PMProjectBalanceRecord>();
			List<PMProjectBalanceRecord> offbalance = new List<PMProjectBalanceRecord>();

            PXSelectBase<PMProjectStatus> select = new PXSelectJoinGroupBy<PMProjectStatus,
            InnerJoin<PMAccountGroup, On<PMAccountGroup.groupID, Equal<PMProjectStatus.accountGroupID>>>,
            Where<PMProjectStatus.projectID, Equal<Current<PMProject.contractID>>>,
            Aggregate<GroupBy<PMProjectStatus.accountGroupID,
            Sum<PMProjectStatus.amount,
            Sum<PMProjectStatus.revisedAmount,
            Sum<PMProjectStatus.actualAmount>>>>>>(this);

            foreach (PXResult<PMProjectStatus, PMAccountGroup> res in select.Select())
            {
                PMProjectStatus ps = (PMProjectStatus)res;
                PMAccountGroup ag = (PMAccountGroup)res;

                switch (ag.Type)
                {
                    case AccountType.Asset:
                        asset.Add(PMProjectBalanceRecord.FromStatus(ps, ag));
                        break;
                    case AccountType.Liability:
                        liability.Add(PMProjectBalanceRecord.FromStatus(ps, ag));
                        break;
                    case AccountType.Income:
                        income.Add(PMProjectBalanceRecord.FromStatus(ps, ag));
                        break;
                    case AccountType.Expense:
                        expense.Add(PMProjectBalanceRecord.FromStatus(ps, ag));
                        break;
					case PMAccountType.OffBalance:
						offbalance.Add(PMProjectBalanceRecord.FromStatus(ps, ag));
						break;
                        
                }
            }

            asset.Sort();
            liability.Sort();
            income.Sort();
            expense.Sort();
			offbalance.Sort();            

            int cx = 0;

            var cache = BalanceRecords.Cache;
            foreach (PMProjectBalanceRecord line in GetLines(AccountType.Asset, asset))
            {
                line.SortOrder = cx++;
                yield return cache.Locate(line) ?? cache.Insert(line);
            }

            foreach (PMProjectBalanceRecord line in GetLines(AccountType.Liability, liability))
            {
                line.SortOrder = cx++;
                yield return cache.Locate(line) ?? cache.Insert(line);
            }

            foreach (PMProjectBalanceRecord line in GetLines(AccountType.Income, income))
            {
                line.SortOrder = cx++;
                yield return cache.Locate(line) ?? cache.Insert(line);
            }

            foreach (PMProjectBalanceRecord line in GetLines(AccountType.Expense, expense))
            {
                line.SortOrder = cx++;
                yield return cache.Locate(line) ?? cache.Insert(line);
            }

            foreach (PMProjectBalanceRecord line in GetLines(PMAccountType.OffBalance, offbalance))
			{
				line.SortOrder = cx++;
                yield return cache.Locate(line) ?? cache.Insert(line);
            }


            BalanceRecords.Cache.IsDirty = false;
        }
        
        private IEnumerable GetLines(string accountType, List<PMProjectBalanceRecord> records)
        {
            if (records.Count > 0)
            {
                if (!IsMobile)
                {
                    yield return PMProjectBalanceRecord.CreateHeader(accountType);
                }

                decimal totalAmt = 0;
                decimal totalRevAmt = 0;
                decimal totalActAmt = 0;

                foreach (PMProjectBalanceRecord record in records)
                {
                    if (!IsMobile)
                    {
                        totalAmt += record.Amount ?? 0;
                        totalRevAmt += record.RevisedAmount ?? 0;
                        totalActAmt += record.ActualAmount ?? 0;
                    }

                    yield return record;
                }

                if (!IsMobile)
                {
                    yield return PMProjectBalanceRecord.CreateTotal(accountType, totalAmt, totalRevAmt, totalActAmt);
                }
            }
        }
        
		#region Event Handlers

		protected virtual void SelectedTask_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			SelectedTask row = e.Row as SelectedTask;
			if (row != null)
			{
				bool found = false;
				foreach (PMTask task in Tasks.Select())
				{
					if (string.IsNullOrEmpty(task.TaskCD))
						continue;

					if (string.Equals(task.TaskCD.Trim(), row.TaskCD.Trim(), StringComparison.InvariantCultureIgnoreCase))
					{
						found = true;
						break;
					}
				}

				PXUIFieldAttribute.SetWarning<SelectedTask.taskCD>(sender, e.Row, found ? Messages.TaskAlreadyExists : null);
				PXUIFieldAttribute.SetEnabled(sender, e.Row, !found);
			}
		}

		protected virtual void SelectedTask_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			e.Cancel = true;
		}


		protected virtual void PMTask_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			PMTask row = e.Row as PMTask;
			if (row != null)
			{
				PXUIFieldAttribute.SetEnabled<PMTask.locationID>(sender, e.Row, row.Status == ProjectTaskStatus.Planned);
				PXUIFieldAttribute.SetEnabled<PMTask.billingOption>(sender, e.Row, row.Status == ProjectTaskStatus.Planned);
				PXUIFieldAttribute.SetEnabled<PMTask.completedPct>(sender, e.Row, row.Status != ProjectTaskStatus.Planned);
			}
		}

		protected virtual void PMTask_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			PMTask row = e.Row as PMTask;
			if (row != null)
			{
				if (this.IsCopyPasteContext)
				{
					row.Status = PM.ProjectStatus.Planned;
					row.IsActive = false;
					row.IsCompleted = false;
					row.IsCancelled = false;
					row.StartDate = null;
					row.EndDate = null;
					row.CompletedPct = 0;
				}

                if (this.IsMobile && string.IsNullOrEmpty(row.TaskCD))
                {
                    PXUIFieldAttribute.SetError<PMTask.taskCD>(sender, row, String.Format("'{0}' may not be empty.", "Task ID"));
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
			if (tran != null)
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
			PMTask row = e.Row as PMTask;
			if (row == null)
				return;
			foreach (PMProjectStatus status in PXSelect<PMProjectStatus, Where<PMProjectStatus.projectID, Equal<Current<PMTask.projectID>>, And<PMProjectStatus.projectTaskID, Equal<Current<PMTask.taskID>>>>>.SelectMultiBound(this, new object[] { row }))
			{
				InternalProjectStatus.Delete(status);
			}
			foreach (PMProjectStatusEx ps in ProjectStatus.Cache.Cached)
			{
				if ( ps.ProjectID == row.ProjectID && ps.ProjectTaskID == row.TaskID)
					ProjectStatus.Cache.Remove(ps);
			}
		}
		protected virtual void PMTask_RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
		{
			if ((e.Operation & PXDBOperation.Insert) != PXDBOperation.Insert || e.TranStatus != PXTranStatus.Open)
				return;
			PMTask dstTask = (PMTask)e.Row;
			if (dstTask == null)
				return;
			foreach (CSAnswers answer in Answer.Select(dstTask.NoteID))
			{
				CSAnswers dstanswer =
					Answer.Insert(new CSAnswers()
					{
                        RefNoteID = dstTask.NoteID,
						AttributeID = answer.AttributeID
					});
				if (dstanswer != null)
					dstanswer.Value = answer.Value;
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
							recordCount++;
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
		
		protected virtual void PMTask_PlannedStartDate_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			PMTask row = e.Row as PMTask;
			if (row != null)
			{
				if (e.NewValue != null && Project.Current != null && Project.Current.StartDate != null)
				{
					DateTime date = (DateTime)e.NewValue;

					if (date.Date < Project.Current.StartDate.Value.Date)
					{
                        sender.RaiseExceptionHandling<PMTask.plannedStartDate>(e.Row, e.NewValue, new PXSetPropertyException<PMTask.plannedStartDate>(Warnings.StartDateOverlow, PXErrorLevel.Warning));
					}

					if (row.PlannedEndDate != null && date > row.PlannedEndDate)
					{
						sender.RaiseExceptionHandling<PMTask.plannedStartDate>(e.Row, e.NewValue, new PXSetPropertyException<PMTask.plannedStartDate>(Messages.StartEndDateInvalid, PXErrorLevel.Error));
					}
				}
			}
		}

		protected virtual void PMTask_PlannedEndDate_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			PMTask row = e.Row as PMTask;
			if (row != null)
			{
				if (e.NewValue != null && Project.Current != null && Project.Current.ExpireDate != null)
				{
					DateTime date = (DateTime)e.NewValue;

					if (date.Date > Project.Current.ExpireDate.Value.Date)
					{
                        sender.RaiseExceptionHandling<PMTask.plannedEndDate>(e.Row, e.NewValue, new PXSetPropertyException<PMTask.plannedEndDate>(Warnings.EndDateOverlow, PXErrorLevel.Warning));
					}

					if (row.PlannedStartDate != null && date < row.PlannedStartDate)
					{
						sender.RaiseExceptionHandling<PMTask.plannedEndDate>(e.Row, e.NewValue, new PXSetPropertyException<PMTask.plannedEndDate>(Messages.StartEndDateInvalid, PXErrorLevel.Error));
					}
				}
			}
		}
        
		protected virtual void PMTask_Status_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			//Workflow is handled here since AU cannot be used for detail record.

			PMTask row = e.Row as PMTask;
			if (row != null)
			{
				switch (row.Status)
				{
					case ProjectTaskStatus.Active:
						row.IsActive = true;
						row.IsCompleted = false;
						row.IsCancelled = false;
						if (row.StartDate == null)
							row.StartDate = Accessinfo.BusinessDate;
						break;
					case ProjectTaskStatus.Canceled:
						row.IsActive = false;
						row.IsCompleted = false;
						row.IsCancelled = true;
						break;
					case ProjectTaskStatus.Completed:
						row.IsActive = true;
						row.IsCompleted = true;
						row.IsCancelled = false;
						row.EndDate = Accessinfo.BusinessDate;
						row.CompletedPct = Math.Max(100, row.CompletedPct.GetValueOrDefault());
						break;
					case ProjectTaskStatus.Planned:
						row.IsActive = false;
						row.IsCompleted = false;
						row.IsCancelled = false;
						break;

				}
			}
		}

		
        protected virtual void PMProject_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            PMProject row = e.Row as PMProject;
            if (row != null)
            {
                ContractBillingSchedule schedule = new ContractBillingSchedule();
                schedule.ContractID = row.ContractID;
                Billing.Insert(schedule);

                PXUIFieldAttribute.SetRequired<ContractBillingSchedule.nextDate>(sender, true);
                Billing.Cache.IsDirty = false;
				
            }
        }

        protected virtual void EPEmployeeContract_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
        {
            EPEmployeeContract row = e.Row as EPEmployeeContract;
            if (row != null && this.IsMobile)
            {
                var toDelete = new List<object>();
                foreach (var item in sender.Inserted)
                {
                    if (((EPEmployeeContract)item).EmployeeID == null)
                    {
                        toDelete.Add(item);
                    }
                }

                foreach (var item in toDelete)
                {
                    sender.Delete(item);
                }
            }
        }

        protected virtual void EPEmployeeContract_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
        {
            EPEmployeeContract row = e.Row as EPEmployeeContract;
            if (row != null && this.IsMobile)
            {
                var toDelete = new List<object>();
                foreach (var item in sender.Inserted)
                {
                    if (((EPEmployeeContract)item).EmployeeID == row.EmployeeID)
                    {
                        toDelete.Add(item);
                    }
                }

                foreach (var item in toDelete)
                {
                    sender.Delete(item);
                }
            }
        }

        protected virtual void EPEmployeeContract_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            EPEmployeeContract row = e.Row as EPEmployeeContract;
            if (row != null && this.IsMobile)
            {
                if (row.EmployeeID == null)
                {
                    sender.Delete(row);
                    e.Cancel = true;
                }
            }
        }
        
        protected virtual void PMProject_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			PMProject row = e.Row as PMProject;
			if (row != null)
			{
				PXUIFieldAttribute.SetEnabled<PMProject.visibleInGL>(sender, e.Row, Setup.Current.VisibleInGL == true);
				PXUIFieldAttribute.SetEnabled<PMProject.visibleInAP>(sender, e.Row, Setup.Current.VisibleInAP == true);
				PXUIFieldAttribute.SetEnabled<PMProject.visibleInAR>(sender, e.Row, Setup.Current.VisibleInAR == true);
				PXUIFieldAttribute.SetEnabled<PMProject.visibleInSO>(sender, e.Row, Setup.Current.VisibleInSO == true);
				PXUIFieldAttribute.SetEnabled<PMProject.visibleInPO>(sender, e.Row, Setup.Current.VisibleInPO == true);
				PXUIFieldAttribute.SetEnabled<PMProject.visibleInEP>(sender, e.Row, Setup.Current.VisibleInEP == true);
				PXUIFieldAttribute.SetEnabled<PMProject.visibleInIN>(sender, e.Row, Setup.Current.VisibleInIN == true);
				PXUIFieldAttribute.SetEnabled<PMProject.visibleInCA>(sender, e.Row, Setup.Current.VisibleInCA == true);
                PXUIFieldAttribute.SetEnabled<PMProject.visibleInCR>(sender, e.Row, Setup.Current.VisibleInCR == true);
				PXUIFieldAttribute.SetEnabled<PMProject.templateID>(sender, e.Row, row.TemplateID == null && sender.GetStatus(row) == PXEntryStatus.Inserted);
				PXUIFieldAttribute.SetEnabled<PMProject.automaticReleaseAR>(sender, row, Project.Current.CustomerID != null);
				
                PXSelectBase<PMTaskTotal> select = new PXSelectGroupBy<PMTaskTotal, Where<PMTaskTotal.projectID, Equal<Current<PMProject.contractID>>>,
                Aggregate<Sum<PMTaskTotal.asset, Sum<PMTaskTotal.liability, Sum<PMTaskTotal.income, Sum<PMTaskTotal.expense>>>>>>(this);

				Tasks.Cache.AllowInsert = row.IsCompleted != true && row.IsCancelled != true;
				Tasks.Cache.AllowUpdate = row.IsCompleted != true && row.IsCancelled != true;
				Tasks.Cache.AllowDelete = row.IsCompleted != true && row.IsCancelled != true;

				ContractRates.Cache.AllowInsert = row.IsCompleted != true && row.IsCancelled != true;
				ContractRates.Cache.AllowUpdate = row.IsCompleted != true && row.IsCancelled != true;
				ContractRates.Cache.AllowDelete = row.IsCompleted != true && row.IsCancelled != true;

				EmployeeContract.Cache.AllowInsert = row.IsCompleted != true && row.IsCancelled != true;
				EmployeeContract.Cache.AllowUpdate = row.IsCompleted != true && row.IsCancelled != true;
				EmployeeContract.Cache.AllowDelete = row.IsCompleted != true && row.IsCancelled != true;

				EquipmentRates.Cache.AllowInsert = row.IsCompleted != true && row.IsCancelled != true;
				EquipmentRates.Cache.AllowUpdate = row.IsCompleted != true && row.IsCancelled != true;
				EquipmentRates.Cache.AllowDelete = row.IsCompleted != true && row.IsCancelled != true;
				

                PMTaskTotal tt = select.Select();
                if (tt != null)
                {
                    row.Asset = tt.Asset;
                    row.Liability = tt.Liability;
                    row.Income = tt.Income;
                    row.Expense = tt.Expense;
                }
			}
		}

		protected virtual void PMProject_IsActive_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			PMProject row = e.Row as PMProject;
			if (row == null) return;
			
			if (row.IsActive == true && (bool?) e.OldValue != true )
			{
				ActivateProject(row);
			}

		}

		protected virtual void PMProject_IsCompleted_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			PMProject row = e.Row as PMProject;
			if (row == null) return;

			if (row.IsCompleted == true && (bool?)e.OldValue != true)
			{
				CompleteProject(row);
			}
		}

		protected virtual void PMProject_IsCompleted_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			PMProject row = e.Row as PMProject;
			if (row != null && e.NewValue != null && ((bool)e.NewValue) == true)
			{
				//Project can only be completed if all task are completed.
				PXResultset<PMTask> res = PXSelect<PMTask, Where<PMTask.projectID, Equal<Current<PMProject.contractID>>, And<PMTask.isCompleted, Equal<False>, And<PMTask.isCancelled, Equal<False>>>>>.Select(this);

				if (res.Count > 0)
				{
					e.NewValue = false;

					sender.RaiseExceptionHandling<PMProject.status>(row, row.Status, new PXSetPropertyException<PMProject.status>(Messages.UncompletedTasksExist, PXErrorLevel.Error, res.Count));
				}
			}
		}

		protected virtual void PMProject_CustomerID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			PMProject row = e.Row as PMProject;
			if (row != null)
			{
				if (sender.GetStatus(row) == PXEntryStatus.Inserted)
				{
					#region Default CuryID and Rate type from Customer and CustomerClass

					PXSelectBase<Customer> cs = new PXSelect<Customer,
						Where<Customer.bAccountID, Equal<Required<Customer.bAccountID>>>>(this);
					Customer customer = cs.Select(row.CustomerID);

					string defaultCuryID = null;
					string defaultCuryRateType = null;
					if (customer != null)
					{
						if (!string.IsNullOrEmpty(customer.CuryID))
						{
							defaultCuryID = customer.CuryID;
						}

						if (!string.IsNullOrEmpty(customer.CuryRateTypeID))
						{
							defaultCuryRateType = customer.CuryRateTypeID;
						}

						if (string.IsNullOrEmpty(defaultCuryID) || string.IsNullOrEmpty(defaultCuryRateType))
						{
							PXSelectBase<CustomerClass> ccs = new PXSelect<CustomerClass,
								Where<CustomerClass.customerClassID, Equal<Required<CustomerClass.customerClassID>>>>(this);
							CustomerClass customerClass = ccs.Select(customer.CustomerClassID);
							if (customerClass != null)
							{
								if (!string.IsNullOrEmpty(defaultCuryID))
								{
									defaultCuryID = customerClass.CuryID;
								}

								if (!string.IsNullOrEmpty(defaultCuryRateType))
								{
									defaultCuryRateType = customerClass.CuryRateTypeID;
								}
							}

						}
					}

					if (!string.IsNullOrEmpty(defaultCuryID))
					{
						row.CuryID = defaultCuryID;
					}

					if (!string.IsNullOrEmpty(defaultCuryRateType))
					{
						row.RateTypeID = defaultCuryRateType;
					}

					#endregion
				}

				sender.SetDefaultExt<PMProject.locationID>(e.Row);

				foreach (PMTask task in Tasks.Select())
				{
					Tasks.Cache.SetDefaultExt<PMTask.customerID>(task);
				    Tasks.Update(task);
				}

				if ( Billing.Current != null && row.CustomerID == null)
				{
					Billing.Current.Type = null;
					Billing.Current.NextDate = null;

					Billing.Update(Billing.Current);
				}
			}
		}

        protected virtual void PMProject_TemplateID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            PMProject row = e.Row as PMProject;
            if (row == null) return;
            if (ProjectProperties.Cache.GetStatus(row) == PXEntryStatus.Inserted || this.IsMobile)
            {
                DefaultFromTemplate(row, sender);
            }
        }

		protected virtual void PMProject_LocationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			PMProject row = e.Row as PMProject;
			if (row != null)
			{
				sender.SetDefaultExt<PMProject.defaultSubID>(e.Row);
			}
		}
		
		protected virtual void PMProject_CuryID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			PMProject row = e.Row as PMProject;
			if (row != null && Company.Current != null)
			{
				row.CuryID = Company.Current.BaseCuryID;
			}
		}
        
        protected virtual void PMProjectBalanceRecord_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            e.Cancel = true;
        }

		protected virtual void EPEmployeeContract_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			ContractRates.View.Cache.AllowInsert = e.Row != null;
		}

		protected virtual void EPEmployeeContract_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			EPEmployeeContract oldRow = (EPEmployeeContract)e.OldRow;
			EPEmployeeContract newRow = (EPEmployeeContract)e.Row;
			if (oldRow == null)
				return;
			EPContractRate.UpdateKeyFields(this, oldRow.ContractID, oldRow.EmployeeID, newRow.ContractID, newRow.EmployeeID);
		}

		
        #region ContractBillingSchedule Event Handlers

        protected virtual void ContractBillingSchedule_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            ContractBillingSchedule row = e.Row as ContractBillingSchedule;
            if (row != null)
            {
                if (Project.Current != null )
                {
					PXUIFieldAttribute.SetEnabled<ContractBillingSchedule.type>(sender, row, Project.Current.CustomerID != null && Project.Current.IsActive != true);
					PXUIFieldAttribute.SetRequired<ContractBillingSchedule.type>(sender, Project.Current.CustomerID != null);
                    PXUIFieldAttribute.SetEnabled<ContractBillingSchedule.nextDate>(sender, row, Project.Current.IsActive == true && Project.Current.CustomerID != null);
				}

                if (row.Type == BillingType.OnDemand)
                {
                    PXUIFieldAttribute.SetEnabled<ContractBillingSchedule.nextDate>(sender, row, false);
                }
            }
        }

        protected virtual void ContractBillingSchedule_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			ContractBillingSchedule row = e.Row as ContractBillingSchedule;
			if (row == null) return;

			
			
		}


        #endregion	


		#endregion

		protected virtual PMTask CopyTask(PMTask task, int ProjectID)
	    {
            task = PXSelect<PMTask, Where<PMTask.projectID, Equal<Current<PMTask.projectID>>, And<PMTask.taskCD, Equal<Current<PMTask.taskCD>>>>>.SelectSingleBound(this, new object[] { task });
            PMTask dst = Tasks.Insert(new PMTask { TaskCD = task.TaskCD, ProjectID = ProjectID});
			dst.RateTableID = task.RateTableID ?? Project.Current.RateTableID;
			dst.AllocationID = task.AllocationID ?? Project.Current.AllocationID;
			dst.BillingID = task.BillingID ?? Project.Current.BillingID;
            dst.Description = task.Description;
			dst.ApproverID = task.ApproverID;
            dst.TaxCategoryID = task.TaxCategoryID;
            dst.BillingOption = task.BillingOption;
			dst.DefaultAccountID = task.DefaultAccountID;
			dst.DefaultSubID = task.DefaultSubID;
            dst.VisibleInGL = task.VisibleInGL;
            dst.VisibleInAP = task.VisibleInAP;
            dst.VisibleInAR = task.VisibleInAR;
            dst.VisibleInSO = task.VisibleInSO;
            dst.VisibleInPO = task.VisibleInPO;
            dst.VisibleInEP = task.VisibleInEP;
            dst.VisibleInIN = task.VisibleInIN;
			dst.VisibleInCA = task.VisibleInCA;
			dst.VisibleInCR = task.VisibleInCR;
	        dst.IsActive = task.IsActive ?? false;
			dst.TemplateID = task.TaskID;

            int linenbr = 0;
            foreach (PMProjectStatusEx status in ProjectStatus.Cache.Cached)
            {
                linenbr = Math.Max(linenbr, status.LineNbr ?? 0);
            }

            foreach (PMProjectStatus status in PXSelect<PMProjectStatus, Where<PMProjectStatus.projectID, Equal<Current<PMTask.projectID>>, And<PMProjectStatus.projectTaskID, Equal<Current<PMTask.taskID>>>>>.SelectMultiBound(this, new object[] { task }))
            {
                PMProjectStatusEx dststatus = ProjectStatus.Insert(new PMProjectStatusEx() { ProjectID = ProjectID, ProjectTaskID = dst.TaskID });
                dststatus.LineNbr = ++linenbr;
                dststatus.AccountGroupID = status.AccountGroupID;
                dststatus.InventoryID = status.InventoryID;
                dststatus.Description = status.Description;
                dststatus.Qty = status.Qty;
                dststatus.RevisedQty = status.Qty; 
                dststatus.UOM = status.UOM;
                dststatus.Rate = status.Rate;
                dststatus.Amount = status.Amount;
                dststatus.RevisedAmount = status.Amount;
                dststatus.IsProduction = status.IsProduction;
			}

			foreach (PMDetail detail in PXSelect<PMDetail,Where<PMDetail.contractID, Equal<Current<PMTask.projectID>>, And<PMDetail.taskID, Equal<Current<PMTask.taskID>>>>>.SelectMultiBound(this, new object[] {task}))
			{
				PMDetail newDetail = new PMDetail();
				newDetail.ContractID = ProjectID;
				newDetail.TaskID = dst.TaskID;
				newDetail.InventoryID = detail.InventoryID;
				newDetail.UOM = detail.UOM;
				newDetail.Description = detail.Description;
				newDetail.ItemFee = detail.ItemFee;
				newDetail.CuryItemFee = detail.CuryItemFee;
				newDetail.CuryInfoID = detail.CuryInfoID;
				newDetail.AccountSource = detail.AccountSource;
				newDetail.SubMask = detail.SubMask;
				newDetail.AccountID = detail.AccountID;
				newDetail.SubID = detail.SubID;
				newDetail.ResetUsage = detail.ResetUsage;
				newDetail.Included = detail.Included;

				BillingItems.Insert(newDetail);
			}

			return dst;
        }

        protected virtual void DefaultFromTemplate(PMProject prj, PXCache sender)
        {
            PMProject templ = PXSelect<PMProject, Where<PMProject.contractID, Equal<Current<PMProject.templateID>>>>.SelectSingleBound(this, new object[] { prj });
            if (templ == null) return;

            prj.Description = templ.Description;
			prj.DefaultAccountID = templ.DefaultAccountID;
            prj.DefaultSubID = templ.DefaultSubID;
			prj.DefaultAccrualAccountID = templ.DefaultAccrualAccountID;
			prj.DefaultAccrualSubID = templ.DefaultAccrualSubID;
			prj.CalendarID = templ.CalendarID;
            prj.RestrictToEmployeeList = templ.RestrictToEmployeeList;
            prj.RestrictToResourceList = templ.RestrictToResourceList;
            prj.CuryID = templ.CuryID;
            prj.RateTypeID = templ.RateTypeID;
            prj.AllowOverrideCury = templ.AllowOverrideCury;
            prj.AllowOverrideRate = templ.AllowOverrideRate;
	        prj.RateTableID = templ.RateTableID;
	        prj.AllocationID = templ.AllocationID;
			prj.BillingID = templ.BillingID;
			prj.ApproverID = templ.ApproverID;
			prj.AutomaticReleaseAR = templ.AutomaticReleaseAR;

            prj.VisibleInAP = templ.VisibleInAP;
            prj.VisibleInGL = templ.VisibleInGL;
            prj.VisibleInAR = templ.VisibleInAR;
            prj.VisibleInSO = templ.VisibleInSO;
            prj.VisibleInPO = templ.VisibleInPO;
            prj.VisibleInEP = templ.VisibleInEP;
            prj.VisibleInIN = templ.VisibleInIN;
			prj.VisibleInCA = templ.VisibleInCA;
			prj.VisibleInCR = templ.VisibleInCR;

            prj.StartDate = Accessinfo.BusinessDate;

            ContractBillingSchedule billing = PXSelect<ContractBillingSchedule, Where<ContractBillingSchedule.contractID, Equal<Current<PMProject.contractID>>>>.SelectSingleBound(this, new object[]{templ});
            if(billing != null)
            {
                Billing.Current.Type = billing.Type;
            }

			Dictionary<int, int> taskIds = new Dictionary<int, int>();
            foreach (PMTask task in PXSelect<PMTask, Where<PMTask.projectID, Equal<Current<PMProject.contractID>>>>.SelectMultiBound(this, new object[]{templ}))
            {
                if(task.AutoIncludeInPrj == true)
                {
                    PMTask dest = CopyTask(task, (int)prj.ContractID);
					taskIds.Add(task.TaskID.Value, dest.TaskID.Value);
                }
            }

			foreach (EPEmployeeContract rate in PXSelect<EPEmployeeContract, Where<EPEmployeeContract.contractID, Equal<Current<PMProject.contractID>>>>.SelectMultiBound(this, new object[] { templ }))
			{
				EPEmployeeContract dst = EmployeeContract.Insert(new EPEmployeeContract());
				dst.EmployeeID = rate.EmployeeID;
			}

			foreach (EPContractRate rate in PXSelect<EPContractRate, Where<EPContractRate.contractID, Equal<Current<PMProject.contractID>>>>.SelectMultiBound(this, new object[] { templ }))
            {
				EPContractRate dst = ContractRates.Insert(new EPContractRate());
                dst.IsActive = rate.IsActive;
                dst.EmployeeID = rate.EmployeeID;
                dst.LabourItemID = rate.LabourItemID;
				dst.EarningType = rate.EarningType;
            }

            foreach (EPEquipmentRate equipment in PXSelect<EPEquipmentRate, Where<EPEquipmentRate.projectID, Equal<Current<PMProject.contractID>>>>.SelectMultiBound(this, new object[] { templ }))
            {
                EPEquipmentRate dst = EquipmentRates.Insert(new EPEquipmentRate());
                dst.IsActive = equipment.IsActive;
                dst.EquipmentID = equipment.EquipmentID;
                dst.RunRate = equipment.RunRate;
                dst.SuspendRate = equipment.SuspendRate;
                dst.SetupRate = equipment.SetupRate;
            }

			foreach (PMAccountTask acc in PXSelect<PMAccountTask, Where<PMAccountTask.projectID, Equal<Current<PMProject.contractID>>>>.SelectMultiBound(this, new object[] { templ }))
			{
				if (taskIds.ContainsKey(acc.TaskID.GetValueOrDefault()) && !IsImport)
				{
					PMAccountTask dst = (PMAccountTask)Accounts.Cache.Insert();
					dst.AccountID = acc.AccountID;
					dst.TaskID = taskIds[acc.TaskID.GetValueOrDefault()];
				}
			}
			
		}

		protected virtual bool Validate()
		{
			if (Project.Current != null && Billing.Current != null && Project.Current.CustomerID != null && string.IsNullOrEmpty(Billing.Current.Type))
			{
				Billing.Cache.RaiseExceptionHandling<ContractBillingSchedule.type>(Billing.Current, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(ContractBillingSchedule.type).Name));
				return false;
			}

			return true;
		}

		public override void Persist()
	    {
			if (!Validate())
			{
				throw new PXException(Messages.ValidationFailed);
			}

			using (PXTransactionScope ts = new PXTransactionScope())
			{
				base.Persist();
				this.Persist(typeof(PMHistory2Accum), PXDBOperation.Insert);
				ts.Complete();
			}
			Caches[typeof(PMHistory2Accum)].Clear();
            Caches[typeof(PMProjectStatusEx)].Clear();
            Caches[typeof(PMProjectStatus)].Clear();
	    }

		
        public virtual void ActivateProject(PMProject project)
		{
			if ( project.StartDate == null )
				project.StartDate = Accessinfo.BusinessDate;

            if (Billing.Current != null)
            {
                if (!string.IsNullOrEmpty(Billing.Current.Type))
				{
					Billing.Current.NextDate = PMBillEngine.GetNextBillingDate(this, Billing.Current, project.StartDate.Value);
					Billing.Update(Billing.Current);
				}
            }
		}
        		
		public virtual void CompleteProject(PMProject project)
		{
			project.ExpireDate = Accessinfo.BusinessDate;
		}

				
		private PMProject GetProjectByID(int? id)
		{
			if (id == null)
				return null;

			return PXSelect<PMProject, Where<PMProject.contractID, Equal<Required<PMProject.contractID>>>>.Select(this, id);
		}

        private bool CanBeBilled()
        {
            if (Project.Current != null)
            {
                if (Project.Current.IsActive == false)
                {
                    throw new PXException(Messages.InactiveProjectsCannotBeBilled);
                }

                if (Project.Current.IsCancelled == true)
                {
                    throw new PXException(Messages.CancelledProjectsCannotBeBilled);
                }


                if (Project.Cache.GetStatus(Project.Current) == PXEntryStatus.Inserted)
                    return false;

                if (Project.Current.CustomerID == null)
                {
                    throw new PXException(Messages.NoCustomer);
                }
                else
                {
					ContractBillingSchedule billingCurrent = Billing.Select();

					if (billingCurrent != null)
                    {
						if (billingCurrent.NextDate == null && billingCurrent.Type != BillingType.OnDemand)
                            throw new PXException(Messages.NoNextBillDateProjectCannotBeBilled);
                    }
                    else
                    {
                        return false;
                    }

                    return true;
                }
            }

            return false;
        }
		        
		#region Local Types

        [Serializable]
		public partial class PMProjectBalanceRecord : PX.Data.IBqlTable, IComparable<PMProjectBalanceRecord>
		{
			public const int EmptyInventoryID = 0;

			#region RecordID
			public abstract class recordID : PX.Data.IBqlField
			{
			}
			protected Int32? _RecordID;
			[PXInt(IsKey = true)]
			public virtual Int32? RecordID
			{
				get
				{
					return this._RecordID;
				}
				set
				{
					this._RecordID = value;
				}
			}
			#endregion
			#region AccountGroup
			public abstract class accountGroup : PX.Data.IBqlField
			{
			}
			protected string _AccountGroup;
			[PXString]
			[PXUIField(DisplayName = "Account Group")]
			public virtual string AccountGroup
			{
				get
				{
					return this._AccountGroup;
				}
				set
				{
					this._AccountGroup = value;
				}
			}
			#endregion
			#region SortOrder
			public abstract class sortOrder : PX.Data.IBqlField
			{
			}
			protected Int32? _SortOrder;
			[PXInt()]
			public virtual Int32? SortOrder
			{
				get
				{
					return this._SortOrder;
				}
				set
				{
					this._SortOrder = value;
				}
			}
			#endregion

			#region Description
			public abstract class description : PX.Data.IBqlField
			{
			}
			protected String _Description;
			[PXString(255, IsUnicode = true)]
			[PXUIField(DisplayName = "Description")]
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
			#region Amount
			public abstract class amount : PX.Data.IBqlField
			{
			}
			protected Decimal? _Amount;
			[PXBaseCury]
			[PXUIField(DisplayName = "Original Budgeted Amount")]
			public virtual Decimal? Amount
			{
				get
				{
					return this._Amount;
				}
				set
				{
					this._Amount = value;
				}
			}
			#endregion
			#region RevisedAmount
			public abstract class revisedAmount : PX.Data.IBqlField
			{
			}
			protected Decimal? _RevisedAmount;
			[PXBaseCury]
            [PXUIField(DisplayName = "Current Budgeted Amount")]
			public virtual Decimal? RevisedAmount
			{
				get
				{
					return this._RevisedAmount;
				}
				set
				{
					this._RevisedAmount = value;
				}
			}
			#endregion
			#region ActualAmount
			public abstract class actualAmount : PX.Data.IBqlField
			{
			}
			protected Decimal? _ActualAmount;
			[PXBaseCury]
			[PXUIField(DisplayName = "Actual Amount", Enabled = false)]
			public virtual Decimal? ActualAmount
			{
				get
				{
					return this._ActualAmount;
				}
				set
				{
					this._ActualAmount = value;
				}
			}
			#endregion
			#region Performance
			public abstract class performance : PX.Data.IBqlField
			{
			}
			protected Decimal? _Performance;
			[PXDBDecimal(2)]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Performance (%)", Enabled = false)]
			public virtual Decimal? Performance
			{
				get
				{
					if (_RevisedAmount != 0)
						return (_ActualAmount / _RevisedAmount) * 100;
					else
						return 0;
				}
			}
			#endregion

			public static PMProjectBalanceRecord FromStatus(PMProjectStatus ps, PMAccountGroup ag)
			{
				PMProjectBalanceRecord record = new PMProjectBalanceRecord();
				record.RecordID = ps.AccountGroupID;
				record.AccountGroup = ag.GroupCD;
				record.Description = ag.Description;
				record.Amount = ps.Amount;
				record.RevisedAmount = ps.RevisedAmount;
				record.ActualAmount = ps.ActualAmount;

				return record;
			}

			public static PMProjectBalanceRecord CreateHeader(string accountType)
			{
				PMProjectBalanceRecord record = new PMProjectBalanceRecord();

				switch (accountType)
				{
					case AccountType.Asset:
						record.RecordID = -10;
						record.AccountGroup = GL.Messages.Asset;
						break;

					case AccountType.Liability:
						record.RecordID = -20;
						record.AccountGroup = GL.Messages.Liability;
						break;

					case AccountType.Income:
						record.RecordID = -30;
						record.AccountGroup = GL.Messages.Income;
						break;

					case AccountType.Expense:
						record.RecordID = -40;
						record.AccountGroup = GL.Messages.Expense;
						break;

					case PMAccountType.OffBalance:
						record.RecordID = -50;
						record.AccountGroup = PM.Messages.OffBalance;
						break;
				}

				return record;
			}

			public static PMProjectBalanceRecord CreateTotal(string accountType, decimal amount, decimal revisedAmt, decimal actualAmt)
			{
				PMProjectBalanceRecord record = new PMProjectBalanceRecord();

				switch (accountType)
				{
					case AccountType.Asset:
						record.RecordID = -11;
						record.Description = GL.Messages.Asset + " Totals";
						break;

					case AccountType.Liability:
						record.RecordID = -21;
						record.Description = GL.Messages.Liability + " Totals";
						break;

					case AccountType.Income:
						record.RecordID = -31;
						record.Description = GL.Messages.Income + " Totals";
						break;

					case AccountType.Expense:
						record.RecordID = -41;
						record.Description = GL.Messages.Expense + " Totals";
						break;

					case PMAccountType.OffBalance:
						record.RecordID = -51;
						record.Description = Messages.OffBalance + " Totals";
						break;
				}


				record.Amount = amount;
				record.RevisedAmount = revisedAmt;
				record.ActualAmount = actualAmt;

				return record;
			}

			public static PMProjectBalanceRecord CreateFooter(string accountType)
			{
				PMProjectBalanceRecord record = new PMProjectBalanceRecord();

				switch (accountType)
				{
					case AccountType.Asset:
						record.RecordID = -12;
						break;

					case AccountType.Liability:
						record.RecordID = -22;
						break;

					case AccountType.Income:
						record.RecordID = -32;
						break;

					case AccountType.Expense:
						record.RecordID = -42;
						break;
					case PMAccountType.OffBalance:
						record.RecordID = -52;
						break;
				}

				return record;
			}

			#region IComparable<PMProjectBalanceRecord> Members

			public int CompareTo(PMProjectBalanceRecord other)
			{
				return AccountGroup.CompareTo(other.AccountGroup);
			}

			#endregion
		}

		
		[Serializable]
		public partial class TemplateSettingsFilter : IBqlTable
		{
			#region TemplateID
			public abstract class templateID : PX.Data.IBqlField
			{
			}
			protected string _TemplateID;
			[PXString()]
			[PXUIField(DisplayName = "Template ID", Required = true)]
			[PXDimensionAttribute(ProjectAttribute.DimensionName)]
			public virtual string TemplateID
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
		}

		#endregion

		public override int ExecuteUpdate(string viewName, IDictionary keys, IDictionary values, params object[] parameters)
		{
            if (viewName.ToLower() == "project" && values != null)
            {
                if (IsExport || IsMobile || IsContractBasedAPI)
                {
                    Project.Cache.Locate(keys);
                    if (values.Contains("Hold") && values["Hold"] != PXCache.NotSetValue && values["Hold"] != null)
                    {
                        var hold = Project.Current.Hold ?? false;
                        if (Convert.ToBoolean(values["Hold"]) != hold)
                        {
                            ((PXAction<PMProject>)this.Actions["Hold"]).PressImpl(false);
                        }
                    }
                }

                values["Hold"] = PXCache.NotSetValue;
            }

            return base.ExecuteUpdate(viewName, keys, values, parameters);
		}

		public override IEnumerable ExecuteSelect(string viewName, object[] parameters, object[] searches, string[] sortcolumns, bool[] descendings, PXFilterRow[] filters, ref int startRow, int maximumRows, ref int totalRows)
		{
			return base.ExecuteSelect(viewName, parameters, searches, sortcolumns, descendings, filters, ref startRow, maximumRows, ref totalRows);
		}
		
	}
}
