using PX.Data;
using PX.Objects.CS;
using System.Collections;
using PX.Objects.GL;
using PX.Objects.AR;
using PX.Objects.EP;
using PX.Objects.CT;
using PX.Objects.IN;

namespace PX.Objects.PM
{
    public class TemplateMaint : PXGraph<TemplateMaint, PMProject>
    {
        #region DAC Attributes Override

        #region PMProject
        [PXDimensionSelector(ProjectAttribute.DimensionName,
            typeof(Search<PMProject.contractCD, Where<PMProject.baseType, Equal<PMProject.ProjectBaseType>, And<PMProject.isTemplate, Equal<True>>>>),
            typeof(PMProject.contractCD),
            typeof(PMProject.contractCD), typeof(PMProject.customerID), typeof(PMProject.description), typeof(PMProject.status), DescriptionField = typeof(PMProject.description))]
		[PXDBString(30, IsUnicode = true, IsKey = true, InputMask = "")]
        [PXDefault]
        [PXUIField(DisplayName = "Template ID", Visibility = PXUIVisibility.SelectorVisible)]
        protected virtual void PMProject_ContractCD_CacheAttached(PXCache sender)
        {
        }

        [Project(Visibility = PXUIVisibility.Invisible, Visible = false)]
        protected virtual void PMProject_TemplateID_CacheAttached(PXCache sender)
        {
        }

        [PXDBBool]
        [PXDefault(true)]
        protected virtual void PMProject_IsTemplate_CacheAttached(PXCache sender)
        {
        }

        [PXDBBool]
        [PXDefault(false)]
        [PXUIField(Visibility = PXUIVisibility.Invisible, Visible = false)]
        protected virtual void PMProject_NonProject_CacheAttached(PXCache sender)
        {
        }

        [PXDBString(1, IsFixed = true)]
        [ProjectStatus.TemplStatusList]
        [PXDefault(ProjectStatus.OnHold)]
        [PXUIField(DisplayName = "Status", Required = true, Visibility = PXUIVisibility.SelectorVisible)]
        protected virtual void PMProject_Status_CacheAttached(PXCache sender)
        {
        }

        [PXDBDate]
        protected virtual void PMProject_StartDate_CacheAttached(PXCache sender)
        {
        }

        #endregion

        #region PMTask
        [Project(typeof(Where<PMProject.isTemplate, Equal<True>>), DisplayName = "Project ID", IsKey = true, DirtyRead=true)]
        [PXParent(typeof(Select<PMProject, Where<PMProject.contractID, Equal<Current<PMTask.projectID>>>>))]
        [PXDBLiteDefault(typeof(PMProject.contractID))]
        protected virtual void PMTask_ProjectID_CacheAttached(PXCache sender)
        {
        }

        [PXDBString(1, IsFixed = true)]
        [PXDefault(ProjectTaskStatus.Active)]
        [PXUIField(Visibility = PXUIVisibility.Invisible, Visible = false)]
        protected virtual void PMTask_Status_CacheAttached(PXCache sender)
        {
        }

        [Customer(DescriptionField = typeof(Customer.acctName), Visibility = PXUIVisibility.Invisible, Visible = false)]
        protected virtual void PMTask_CustomerID_CacheAttached(PXCache sender)
        {
        }

        [PXDBBool]
        [PXDefault(true)]
        [PXUIField(DisplayName = "Automatically Include in Project")]
        protected virtual void PMTask_AutoIncludeInPrj_CacheAttached(PXCache sender)
        {
        }

        #endregion

        #region EPEquipmentRate

        [PXDBInt(IsKey = true)]
        [PXDefault]
        [PXUIField(DisplayName = "Equipment ID")]
        [PXSelector(typeof(EPEquipment.equipmentID), DescriptionField = typeof(EPEquipment.description))]
        protected virtual void EPEquipmentRate_EquipmentID_CacheAttached(PXCache sender)
        {
        }


        [PXParent(typeof(Select<PMProject, Where<PMProject.contractID, Equal<Current<EPEquipmentRate.projectID>>>>))]
        [PXDBLiteDefault(typeof(PMProject.contractID))]
        [PXDBInt(IsKey = true)]
        protected virtual void EPEquipmentRate_ProjectID_CacheAttached(PXCache sender)
        {
        }

        [PXDBInt]
		[PXUIField(DisplayName = "Run Rate for Project")]
        [PXDimensionSelector(InventoryAttribute.DimensionName, typeof(Search<InventoryItem.inventoryID, Where<InventoryItem.itemType, Equal<INItemTypes.nonStockItem>, And<Match<Current<AccessInfo.userName>>>>>), typeof(InventoryItem.inventoryCD))]
        protected virtual void EPEquipmentRate_RunRateItemID_CacheAttached(PXCache sender)
        {
        }

        [PXDBInt]
		[PXUIField(DisplayName = "Setup Rate for Project")]
        [PXDimensionSelector(InventoryAttribute.DimensionName, typeof(Search<InventoryItem.inventoryID, Where<InventoryItem.itemType, Equal<INItemTypes.nonStockItem>, And<Match<Current<AccessInfo.userName>>>>>), typeof(InventoryItem.inventoryCD))]
        protected virtual void EPEquipmentRate_SetupRateItemID_CacheAttached(PXCache sender)
        {
        }

        [PXDBInt]
		[PXUIField(DisplayName = "Suspend Rate for Project")]
        [PXDimensionSelector(InventoryAttribute.DimensionName, typeof(Search<InventoryItem.inventoryID, Where<InventoryItem.itemType, Equal<INItemTypes.nonStockItem>, And<Match<Current<AccessInfo.userName>>>>>), typeof(InventoryItem.inventoryCD))]
        protected virtual void EPEquipmentRate_SuspendRateItemID_CacheAttached(PXCache sender)
        {
        }

        #endregion

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

		[PXDBString(1, IsFixed = true)]
		[BillingType.ListForProject()]
		[PXUIField(DisplayName = "Billing Period")]
		protected virtual void ContractBillingSchedule_Type_CacheAttached(PXCache sender)
		{
		}

		#region PMDetail
		
		[PXDBInt()]
		[PXDBDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void PMDetail_ContractItemID_CacheAttached(PXCache sender) { }

		
		[PXDBInt()]
		[PXDBLiteDefault(typeof(PMTask.taskID))]
		[PXParent(typeof(Select<PMTask, Where<PMTask.projectID, Equal<Current<PMDetail.contractID>>, And<PMTask.taskID, Equal<Current<PMDetail.taskID>>>>>))]
		protected virtual void PMDetail_TaskID_CacheAttached(PXCache sender) { }

		#endregion

        #endregion

        #region Views/Selects

        public PXSelect<PMProject, Where<PMProject.baseType, Equal<PMProject.ProjectBaseType>,
            And<PMProject.isTemplate, Equal<True>>>> Project;
        public PXSelect<PMProject, Where<PMProject.contractID, Equal<Current<PMProject.contractID>>>> ProjectProperties;

        public PXSelect<ContractBillingSchedule, Where<ContractBillingSchedule.contractID, Equal<Current<PMProject.contractID>>>> Billing;
        [PXFilterable]
        public PXSelect<PMTask, Where<PMTask.projectID, Equal<Current<PMProject.contractID>>>> Tasks;
		[PXFilterable]
        public PXSelectJoin<EPEquipmentRate, InnerJoin<EPEquipment, On<EPEquipmentRate.equipmentID, Equal<EPEquipment.equipmentID>>>, Where<EPEquipmentRate.projectID, Equal<Current<PMProject.contractID>>>> EquipmentRates;
		public PXSelect<PMAccountTask, Where<PMAccountTask.projectID, Equal<Current<PMProject.contractID>>>> Accounts;
        public PXSetup<PMSetup> Setup;
        public PXSetup<Company> Company;
        public PXSelect<PMProjectStatus> Status;
		public PXSelectJoin<EPEmployeeContract,
			InnerJoin<EPEmployee, On<EPEmployee.bAccountID, Equal<EPEmployeeContract.employeeID>>>,
			Where<EPEmployeeContract.contractID, Equal<Current<PMProject.contractID>>>> EmployeeContract;
		public PXSelectJoin<EPContractRate
				, LeftJoin<IN.InventoryItem, On<IN.InventoryItem.inventoryID, Equal<EPContractRate.labourItemID>>
					, LeftJoin<EPEarningType, On<EPEarningType.typeCD, Equal<EPContractRate.earningType>>>
					>
				, Where<EPContractRate.employeeID, Equal<Optional<EPEmployeeContract.employeeID>>, And<EPContractRate.contractID, Equal<Optional<PMProject.contractID>>>>
				, OrderBy<Asc<EPContractRate.contractID>>
				> ContractRates;

		[PXHidden]
		public PXSelect<PMDetail, Where<PMDetail.contractID, Equal<Current<PMTask.projectID>>>> BillingItems;
        #endregion

        #region Actions/Buttons

        public PXAction<PMProject> viewTask;
        [PXUIField(DisplayName = Messages.ViewTask, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntry)]
        public IEnumerable ViewTask(PXAdapter adapter)
        {
            if (Tasks.Current != null && Project.Cache.GetStatus(Project.Current) != PXEntryStatus.Inserted)
            {
                TemplateTaskMaint graph = CreateInstance<TemplateTaskMaint>();
                graph.Task.Current = PXSelect<PMTask, Where<PMTask.projectID, Equal<Required<PMTask.projectID>>, And<PMTask.taskID, Equal<Required<PMTask.taskID>>>>>.Select(this, Tasks.Current.ProjectID, Tasks.Current.TaskID);

                throw new PXPopupRedirectException(graph, Messages.ProjectTaskEntry + " - " + Messages.ViewTask, true);
            }
            return adapter.Get();
        }

        #endregion

        public TemplateMaint()
        {
            if (Setup.Current == null)
            {
                throw new PXException(Messages.SetupNotConfigured);
            }
        }

        #region Event Handlers

        
        protected virtual void PMProject_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            PMProject row = e.Row as PMProject;
            if (row == null) return;
            ContractBillingSchedule schedule = new ContractBillingSchedule {ContractID = row.ContractID};
            Billing.Insert(schedule);
            Billing.Cache.IsDirty = false;
        }

        protected virtual void PMProject_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            PMProject row = e.Row as PMProject;
            if (row == null) return;

            PXUIFieldAttribute.SetEnabled<PMProject.contractCD>(sender, row, true);
        }

		protected virtual void PMProject_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			PMProject row = e.Row as PMProject;
			if (row != null)
			{
				PMProject project = PXSelect<PMProject, Where<PMProject.templateID, Equal<Required<PMProject.contractID>>>>.Select(this, row.ContractID);

				if (project != null)
				{
					e.Cancel = true;
					throw new PXException(Messages.ProjectRefError);
				}
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

        #endregion

    }
}
