using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Common;

namespace PX.Objects.PM
{
	[PXLocalizable(Messages.Prefix)]
	public static class Messages
	{
		#region Validation and Processing Messages
		public const string Prefix = "PM Error";
		public const string AccountGroupAttributeNotValid = "One or more Attributes are not valid.";
		public const string Account_FK = "Account Group cannot be deleted. One or more Accounts are mapped to this Account Group.";
		public const string AccountDiactivate_FK = "Account Group cannot be deactivated. One or more Accounts are mapped to this Account Group.";
		public const string ProjectStatus_FK = "Account Group cannot be deleted. Project Status table contains one or more references to the given Account Group.";
        public const string OnlyPlannedCanbeDeleted = "Once Activated the record cannot be deleted. Only Tasks that are in 'Planning' or 'Cancelled' can be deleted.";
		public const string StartEndDateInvalid = "Planned Start Date for the given Task should be before the Planned End Date.";
		public const string UncompletedTasksExist = "Project can only be Completed if all Tasks are completed. {0} Task(s) are still incomplete.";
		public const string ProjectTaskIsCompleted = "Task is Completed and cannot be used for data entry.";
		public const string ProjectTaskIsCanceled = "Task is Canceled and cannot be used for data entry.";
		public const string HasRollupData = "Cannot delete Balance record since it already has rollup data associated with it.";
		public const string HasTranData = "Cannot delete Task since it already has at least one Transaction associated with it.";
		public const string HasActivityData = "Cannot delete Task since it already has at least on Activity associated with it.";
        public const string HasTimeCardItemData = "Cannot delete Task since it already has at least one Time Card Item Record associated with it.";
		public const string DuplicateProjectStatus = "Inventory ID must be unique within an AccountGroup.";
		public const string DuplicateProjectStatus_Task = "Inventory ID must be unique within Project Task.";
		public const string ValidationFailed = "One or more rows failed to validate. Please correct and try again.";
		public const string NoAccountGroup = "Record is associated with Project whereas Account '{0}' is not associated with any Account Group";
        public const string InactiveProjectsCannotBeBilled = "Inactive Project cannot be billed.";
        public const string CancelledProjectsCannotBeBilled = "Cancelled Project cannot be billed.";
        public const string NoNextBillDateProjectCannotBeBilled = "Project can not be Billed if Next Billing Date is empty.";
        public const string NoCustomer = "This Project has no Customer associated with it and thus cannot be billed.";
        public const string BillingIDIsNotDefined = "Billing Rule must be defined for the task for Auto Budget to work.";
		public const string FailedToEmulateExpenses = "Failed to emulate Expenses when running Auto Budget for Task {0}. Probably there is no Expense Account Group in the Budget.";
		public const string FailedToCalcDescFormula = "Failed to calculate Description formula in the allocation rule:{0}, Step{1}. Formula:{2} Error:{3}";
		public const string FailedToCalcAmtFormula = "Failed to calculate Amount formula in the allocation rule:{0}, Step{1}. Formula:{2} Error:{3}";
		public const string FailedToCalcQtyFormula = "Failed to calculate Quantity formula in the allocation rule:{0}, Step{1}. Formula:{2} Error:{3}";
		public const string FailedToCalcBillQtyFormula = "Failed to calculate Billable Quantity formula in the allocation rule:{0}, Step{1}. Formula:{2} Error:{3}";
		public const string PeriodsOverlap = "Overlapping time intervals are not allowed";
		public const string Activities = "Activities";
		public const string RangeOverlapItself = "Range for the summary step should not refer to itself.";
		public const string RangeOverlapFuture = "Range for the summary step should not refer future steps.";
		public const string ReversalExists = "Reversal for the given allocation already exist. Allocation can be reversed only once. RefNbr of the reversal document is {0}.";
		public const string TaskAlreadyExists = "Task with this ID already exists in the Project.";
		public const string AllocationStepFailed = "Failed to Process Step: {0} during Allocation for Task:{1}";
		public const string DebitProjectNotFound = "Step '{0}': Debit Project was not found in the system.";
		public const string CreditProjectNotFound = "Step '{0}': Credit Project was not found in the system.";
		public const string DebitTaskNotFound = "Step '{0}': Failed to assign Debit Task. Task '{1}' was not found for the given Project '{2}'";
		public const string CreditTaskNotFound = "Step '{0}': Failed to assign Credi Task. Task '{1}' was not found for the given Project '{2}'";
		public const string AccountGroupInBillingRuleNotFound = "Billing Rule {0} has invalid Account Group. Account Group with the the given ID '{1}' was not found in the system.";
		public const string AccountGroupInAllocationStepFromNotFound = "Billing Rule {0} / Allocation Step {1} / From has invalid Account Group. Account Group with the the given ID '{2}' was not found in the system.";
		public const string AccountGroupInAllocationStepToNotFound = "Billing Rule {0} / Allocation Step {1} / To has invalid Account Group. Account Group with the the given ID '{2}' was not found in the system.";
		public const string ProjectInTaskNotFound = "Task '{0}' has invalid Project associated with it. Project with the ID '{1}' was not found in the system.";
		public const string TaskNotFound = "Task with the given id was not found in the system. ProjectID='{0}' TaskID='{1}'";
		public const string ProjectNotFound = "Project with the given id was not found in the system. ProjectID='{0}'";
		public const string AccountNotFound = "Account with the given id was not found in the system. AccountID='{0}'";
		public const string AutoAllocationFailed = "Auto-allocation of Project Transactions failed.";
		public const string AutoReleaseFailed = "Auto-release of allocated Project Transactions failed. Please try to release this document manually.";
		public const string AutoReleaseARFailed = "Auto-release of ARInvoice document created during billing failed. Please try to release this document manually.";
		public const string AutoReleaseOfReversalFailed = "During Billing ARInvoice was created successfully. PM Reversal document was created successfully. Auto-release of PM Reversal document failed. Please try to release this document manually.";
		public const string BillingRuleAccountIsNotConfiguredForBilling = "Billing Rule '{0}' is configured to get its Account from the Billing Rule but Account is not configured for the Billing Rule '{0}'.";
		public const string BillingRuleAccountIsNotConfiguredForBillingRecurent = "Recurent Billing '{0}' is configured to get its Account from the Recurent Billing Item but Account is not configured for the Billing Rule '{0}'.";
		public const string ProjectAccountIsNotConfiguredForBilling = "Billing Rule '{0}' is configured to get its Account from the Project but Default Account is not configured for Project '{1}'.";
		public const string ProjectAccountIsNotConfiguredForBillingRecurent = "Recurent Billing '{0}' is configured to get its Account from the Project but Default Account is not configured for Project '{1}'.";
		public const string TaskAccountIsNotConfiguredForBilling = "Billing Rule '{0}' is configured to get its Account from the Task but Default Account is not configured for Project '{1}' Task '{2}'.";
		public const string TaskAccountIsNotConfiguredForBillingRecurent = "Recurent Billing '{0}' is configured to get its Account from the Task but Default Account is not configured for Project '{1}' Task '{2}'.";
		public const string InventoryAccountIsNotConfiguredForBilling = "Billing Rule '{0}' is configured to get its Account from the Inventory Item but Sales Account is not configured for Inventory Item '{1}'";
		public const string InventoryAccountIsNotConfiguredForBillingRecurent = "Recurent Billing '{0}' is configured to get its Account from the Inventory Item but Sales Account is not configured for Inventory Item";
		public const string CustomerAccountIsNotConfiguredForBilling = "Billing Rule '{0}' is configured to get its Account from the Customer but Sales Account is not configured for Customer '{1}'";
		public const string CustomerAccountIsNotConfiguredForBillingRecurent = "Recurent Billing '{0}' is configured to get its Account from the Customer but Sales Account is not configured for Customer '{1}'";
		public const string EmployeeAccountIsNotConfiguredForBilling = "Billing Rule '{0}' is configured to get its Account from the Employee but Sales Account is not configured for Employee '{1}'";
		public const string SubAccountCannotBeComposed = "Billing Rule '{0}' will not be able the compose the subaccount since account was not determined.";
		public const string SubAccountCannotBeComposedRecurent = "Recurent Billing '{0}' will not be able the compose the subaccount since account was not determined.";
	    public const string EmployeeNotInProjectList = "Project is configured to restrict employees that are not in the Project's Employee list. Given Employee is not assigned to the Project."; 
		public const string RateTypeNotDefinedForStep = "Rate Type is not defined for step {0}";
		public const string RateNotDefinedForStep = "@Rate is not defined on step {0}. Check Trace for details.";
        public const string InactiveTask = "Project Task '{0}' is inactive.";
        public const string CompletedTask = "Project Task '{0}' is completed.";
        public const string TaskInvisibleInModule = "Project Task '{0}' is invisible in {1} module.";
        public const string InvisibleTask = "Project Task '{0}' is invisible.";
		public const string InactiveContract = "Given Project/Contract '{0}' is inactive";
		public const string CompleteContract = "Given Project/Contract '{0}' is completed";
		public const string TemplateContract = "Given Project/Contract '{0}' is a template";
		public const string DebitAccountGroupIsRequired = "Allocation Rule Step {0} is not defined correctly. Debit Account Group is required.";
		public const string AtleastOneAccountGroupIsRequired = "Allocation Rule Step {0} is not defined correctly. At least either Debit or Credit Account Group is required.";
		public const string DebitAccountEqualCreditAccount = "Debit Account matches Credit Account.";
		public const string DebitAccountGroupEqualCreditAccountGroup = "Debit Account Group matches Credit Account Group.";
		public const string AccountGroupIsRequired = "Failed to Release PM Transaction '{0}': Account Group is required.";
		public const string InvalidAllocationRule = "Allocation Step '{0}' is not valid. When applied to transactions in Task '{1}' failed to set Account Group. Please correct your Allocation rules and try again.";
		public const string PostToGLFailed = "Failed to Automatically Post GLBatch created during release of PM document.";
		public const string UnitConversionNotDefinedForItem = "Failed to Convert from {0} to {1}. Unit conversion is not defined for {2}";
		public const string UnitConversionNotDefinedForItemOnBudgetUpdate = "Failed to Convert from {0} to {1} when updating the Budget for the Project. Unit conversion is not defined for {2}";
	    public const string SourceSubNotSpecified = "Allocation rule is configured to use the source subaccount of transaction that is being allocated but the Subaccount is not set for the original transaction. Please correct your allocation step. Allocation Rule:{0} Step:{1}";
        public const string StepSubNotSpecified = "Allocation rule is configured to use the subaccount of allocation step but the subaccount is not set up. Please correct your allocation step. Allocation Rule:{0} Step:{1}";
        public const string OtherSourceIsEmpty = "Allocation rule is configured to take Debit Account from the source transaction and use it as a Credit Account of allocated transaction but the Debit Account is not set for the source transaction. Rule:{0} Step:{1} Transaction Description:{2}";
		public const string ProjectIsNullAfterAllocation = "In Step {0} Transaction that is processed has a null ProjectID. Please check the allocation rules in the preceding steps.";
		public const string TaskIsNullAfterAllocation = "In Step {0} Transaction that is processed has a null TaskID. Please check the allocation rules in the preceding steps.";
		public const string AccountGroupIsNullAfterAllocation = "In Step {0} Transaction that is processed has a null AllocationID. Please check the allocation rules in the preceding steps.";
		public const string StepSubMaskSpecified = "Subaccount Mask is not set in allocation step. Please correct your allocation step. Allocation Rule:{0} Step:{1}";
		public const string RateTableIsInvalid = "One or more validations failed for the given Rate Table sequence. Combinations of entities within sequence must be unique. The following combinations are not unique:";
		public const string ProjectIdIsNotSpecifiedForActivity = "ProjectID is not specified for the activity {0}:{1}";
		public const string ProjectRefError = "This record cannot be deleted. One or more projects are referencing this document.";
		public const string ValueMustBeGreaterThanZero = "The value must be greater than zero";
		public const string LocationNotFound = "Failed to create allocation transaction. The Location specified on the Task level is not valid. Please check and try again.";
		public const string GenericFieldErrorOnAllocation = "Failed to create allocation transaction.";
        #endregion

		#region Translatable Strings used in the code
		public const string ViewTask = "View Task";
        public const string ViewProject = "View Project";
        public const string NewTask = "New Task";
		public const string OffBalance = "Off-Balance";
		public const string SetupNotConfigured = "Project Management Setup is not configured.";
		public const string ViewBalance = "View Details";
		public const string ViewRates = "View Rates";
		public const string NonProjectDescription = "Non-Project Code.";
		public const string FullDetail = "Full Detail";
		public const string ProcAllocate = "Allocate";
		public const string ProcAllocateAll = "Allocate All";
		public const string ProcBill = "Bill";
		public const string ProcBillAll = "Bill All";
		public const string Release = "Release";
		public const string ReleaseAll = "Release All";
		public const string Approve = "Approve";
		public const string ApproveAll = "Approve All";
		public const string EstimateBudget = "Estimate Budget";
		public const string ViewTransactions = "View Transactions";
        public const string PrjDescription = "Project Description";
        public const string Bill = "Run Project Billing";
		public const string BillTip = "Runs billing for the Next Billing Date";
        public const string Allocate = "Allocate";
	    public const string AddTasks = "Add Tasks";
        public const string AddCommonTasks = "Add Common Tasks";
        public const string CreateTemplate = "Create Template";
		public const string CreateTemplateTip = "Creates Template from the current Project.";
		public const string AutoBudget = "Auto-Budget";
		public const string AutoBudgetTip = "Creates projected budget based on the expenses and Allocation Rules";
		public const string Actions = "Actions";
		public const string Filter = "Filter";
		public const string Reject = "Reject";
		public const string Assign = "Assign";
		public const string ApprovalDate = "Approval Date";
		public const string ReverseAllocation = "Reverse Allocation";
		public const string ReverseAllocationTip = "Reverses Released Allocation";
		public const string ViewAllocationSource = "View Allocation Source";
		public const string ViewBatch = "View Batch";
		public const string ProjectSearchTitle = "Project: {0} - {2}";
		public const string AccountGroup = "Account Group";
		public const string RateCode = "Rate Code";
		public const string Task = "Task";
		public const string Item = "Item";
		public const string FailedEmulateBilling = "Failed to emulate billing. The Sales Account in the Invoice is not mapped to any Account Group.";
		public const string InvalidScheduleType = "Invalid Schedule type";
		public const string ArgumentNullEmptyError ="Argument is null or an empty string";
		public const string AllocationReversalBilling = "Allocation Reversal on Billing";
		public const string AllocationReversalNonBilling = "Allocation Reversal to Non-Billable";
		public const string ProjectAttributeNotSupport = "ProjectAttribute doesnot support the given module";
		public const string ProjectTaskAttributeNotSupport = "ProjectTaskAttribute doesnot support the given module";
		#endregion

		#region Graph Names
		public const string RegisterEntry = "Register Entry";
		public const string ProjectEntry = "Project Entry";
		public const string ProjectTaskEntry = "Project Task Entry";
		public const string ProjectBalanceEntry = "Project Balance Entry";
		public const string ProjectBalanceByPeriodEntry = "Project Balance By Period Entry";
		public const string ProjectAttributeGroupMaint = "Project Attribute Maintenance";
		public const string AccountGroupMaint = "Account Group Maintenance";
		public const string EquipmentMaint = "Equipment Maintenance";
		public const string RateCodeMaint = "Rate Code Maintenance";
		public const string RateDefinitionMaint = "Rate Definition Maintenance";
		public const string RateMaint = "Rate Maintenance";
		public const string BillingMaint = "Billing Maintenance";
		public const string RegisterRelease = "Register Release";
		public const string AllocationProcess = "Allocation Process By Task";
		public const string AllocationProcessByProject = "Allocation Process By Project";
		public const string BillingProcess = "Billing Process";
		public const string ReverseUnbilledProcess = "Reverse Unbilled Process";
		public const string TransactionInquiry = "Transactions Inquiry";
		public const string PMSetup = "Project Management Setup";
        public const string PMSetupMaint = "Project Preferences";
        public const string TaskInquiry = "Tasks Inquiry";
	    public const string TemplateMaint = "Project Template Maintenance";
        public const string TemplateTaskMaint = "Project Task Template Maintenance";
		public const string TemplateGlobalTaskListMaint = "Common Task List Template Maintenance";
		public const string TemplateGlobalTaskMaint = "Project Task Template Maintenance";
		public const string PMAllocator = "Project Allocator";
		#endregion

		#region View Names
        public const string Selection = "Selection";
        public const string ProjectAnswers = "Project Answers";
		public const string TaskAnswers = "Task Answers";
		public const string AccountGroupAnswers = "Account Group Answers";
		public const string EquipmentAnswers = "Equipment Answers";
        public const string PMTasks = "Tasks";
		public const string Approval = "Approval";
		#endregion

		#region DAC Names
		public const string Project = "Project";
		public const string ProjectTask = "Project Task";
	    public const string PMProjectTemplate = "Project Template";
        public const string SelectedTask = "Tasks for Addition";
        public const string PMAccountGroupName = "Project Account Group Name";
        public const string PMTran = "Project Transaction";
        public const string PMHistory = "Project History";
        #endregion

        #region Combo Values
        public const string NotStarted = "NotStarted";
		public const string Active = "Active";
		public const string Canceled = "Canceled";
		public const string Completed = "Completed";
		public const string InPlanning = "In Planning";
        public const string OnHold = "On Hold";
		public const string Suspend = "Suspended";
		public const string PendingApproval = "Pending Approval";

        public const string Hold = "Hold";
        public const string Balanced = "Balanced";
        public const string Released = "Released";


		public const string GroupTypes_Project = "Project";
		public const string GroupTypes_Task = "Task";
		public const string GroupTypes_AccountGroup = "Account Group";
		public const string GroupTypes_Transaction = "Transaction";
		public const string GroupTypes_Equipment = "Equipment";


		public const string Action_Hold = "Hold";
		public const string Action_Post = "Post";
		public const string Action_SumAndPost = "Sum & Post";
				
		public const string None = "None";
		
		public const string Origin_Source = "Use Source";
		public const string Origin_Change = "Replace";
		public const string Origin_FromAccount = "From Account";
		public const string Origin_None = "None";
		public const string Origin_DebitSource = "Debit Source";
		public const string Origin_CreditSource = "Credit Source";

        public const string PMMethod_Transaction = "Allocate Transactions";
        public const string PMMethod_Budget = "Allocate Budget";
		
		public const string PMSelectOption_Transaction = "Not Allocated Transactions";
		public const string PMSelectOption_Step = "From Previous Allocation Steps";
		
		public const string MaskSource = "Source";
		public const string AllocationStep = "Allocation Step";
		public const string ProjectDefault = "Project Default";
		public const string TaskDefault = "Task Default";

		public const string OnBilling = "By Billing Period";
        public const string OnTaskCompletion = "On Task Completion";
        public const string OnProjectCompetion = "On Project Completion";
		
		public const string AccountSource_None = "None";
		public const string AccountSource_BillingRule = "Billing Rule";
		public const string AccountSource_Project = "Project";
        public const string AccountSource_ProjectAccrual = "Project Accrual";
		public const string AccountSource_Task = "Task";
        public const string AccountSource_Task_Accrual = "Task Accrual";
		public const string AccountSource_InventoryItem = "Inventory Item";
		public const string AccountSource_LaborItem = "Labor Item";
        public const string AccountSource_LaborItem_Accrual = "Labor Item Accrual";
		public const string AccountSource_Customer = "Customer";
		public const string AccountSource_Resource = "Resource";
		public const string AccountSource_RecurentBillingItem = "Recurring Item";

		public const string Allocation = "Allocation";
		public const string Timecard = "Time Card";
		public const string Case = "Case";
		public const string ExpenseClaim = "Expense Claim";
		public const string EquipmentTimecard = "Equipment Time Card";
		public const string AllocationReversal = "Allocation Reversal";
		public const string Reversal = "Reversal";
		public const string CreditMemo = "Credit Memo";
		public const string UnbilledRemainder = "Unbilled Remainder";


		public const string PMReverse_OnInvoice = "On Invoice Release";
		public const string PMReverse_OnBilling = "On Project Billing";
		public const string PMReverse_Never = "Never";
		
		public const string PMNoRateOption_SetOne = "Set @Rate to 1";
		public const string PMNoRateOption_SetZero = "Set @Rate to 0";
		public const string PMNoRateOption_RaiseError = "Raise Error";
		public const string PMNoRateOption_NoAllocate = "Do not allocate";

		public const string PMDateSource_Transaction = "Original Transaction";
		public const string PMDateSource_Allocation = "Allocation Date";

		public const string Included = "Include Trans. created on billing date";
		public const string Excluded = "Exclude Trans. created on billing date";

		public const string Manual = "Manual";
		public const string ByQuantity = "Budgeted Quantity";
		public const string ByAmount = "Budgeted Amount";


		#endregion


        #region Field Display Names

        public const string CreditAccountGroup = "Credit Account Group";
        public const string FinPTDAmount = "Financial PTD Amount";
        public const string FinPTDQuantity = "Financial PTD Quantity";
        public const string BudgetPTDAmount = "Budget PTD Amount";
        public const string BudgetPTDQuantity = "Budget PTD Quantity";
        public const string RevisedPTDAmount = "Revised PTD Amount";
        public const string RevisedPTDQuantity = "Revised PTD Quantity";
		#endregion

		public static string GetLocal(string message)
		{
			return PXLocalizer.Localize(message, typeof(Message).FullName);
		}
	}

	[PXLocalizable(Warnings.Prefix)]
    public static class Warnings
    {
        public const string Prefix = "PM Warning";

        public const string AccountIsUsed = "This account is already mapped to '{0}' Account Group. By clicking 'Save,' you will remap the account to the current Acccount Group.";
        public const string StartDateOverlow = "Start Date for the given Task falls outside the Project Start and End date range.";
        public const string EndDateOverlow = "End Date for the given Task falls outside the Project Start and End date range.";
        public const string ProjectIsCompleted = "Project is Completed. It will not be available for data entry.";
        public const string ProjectIsNotActive = "Project is Not Active. Please Activate Project.";
        public const string HasAllocatedTrans = "Allocated Transaction(s) associated with this Task was created using different Billing Rule ('{0}')";
        public const string NothingToAllocate = "Transactions were not created during the allocation.";
        public const string NothingToBill = "Invoice was not created during the billing. Nothing to bill.";
		public const string ProjectCustomerDontMatchTheDocument = "Customer on the Document doesn't match the Customer on the Project or Contract.";
    }
}
