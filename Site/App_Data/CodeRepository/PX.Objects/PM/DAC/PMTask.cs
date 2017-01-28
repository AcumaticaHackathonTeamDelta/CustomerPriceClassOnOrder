using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Data.EP;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.EP;
using PX.TM;
using PX.Objects.TX;
using PX.Objects.IN;
using PX.Objects.GL;
using PX.Objects.CM;

namespace PX.Objects.PM
{
	/// <summary>
	/// The smallest identifiable and essential piece of a job that serves as a unit of work, and as a means of differentiating between the various components of a project.  
	/// Task is always defined within the scope of a project. Task budget, profitability and balances are monitored in scope of account groups. 
	/// </summary>
	[PXCacheName(Messages.ProjectTask)]
	[PXPrimaryGraph(typeof(ProjectTaskEntry))]
    [Serializable]
	public partial class PMTask : PX.Data.IBqlTable
	{
        #region Selected
        public abstract class selected : IBqlField
        {
        }
        protected bool? _Selected = false;

		/// <summary>
		/// Gets or sets whether the task is selected in the grid.
		/// </summary>
        [PXBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected
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

		#region ProjectID
		public abstract class projectID : PX.Data.IBqlField
		{
		}

		/// <summary>
		/// Gets or sets the parent Project.
		/// </summary>
		protected Int32? _ProjectID;
        [Project(typeof(Where<PMProject.nonProject, NotEqual<True>, And<PMProject.isTemplate, NotEqual<True>>>), DisplayName = "Project ID", IsKey = true, DirtyRead=true)]
		[PXParent(typeof(Select<PMProject, Where<PMProject.contractID, Equal<Current<PMTask.projectID>>>>))]
        [PXDBLiteDefault(typeof(PMProject.contractID))]
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
		#region TaskID
		public abstract class taskID : PX.Data.IBqlField
		{
		}
		protected Int32? _TaskID;
		/// <summary>
		/// Gets or sets unique identifier.
		/// </summary>
		[PXDBIdentity()]
		[PXSelector(typeof(PMTask.taskID))]
		public virtual Int32? TaskID
		{
			get
			{
				return this._TaskID;
			}
			set
			{
				this._TaskID = value;
			}
		}
		#endregion
		#region TaskCD
		public abstract class taskCD : PX.Data.IBqlField
		{
		}
		protected String _TaskCD;
		/// <summary>
		/// Get or sets unique identifier.
		/// This is a segmented key and format is configured under segmented key maintenance screen in CS module.
		/// </summary>
		[PXDimension(ProjectTaskAttribute.DimensionName)]
		[PXDBString(IsUnicode = true, IsKey = true, InputMask = "")]
		[PXDefault()]
		[PXUIField(DisplayName = "Task ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXFieldDescription]
		public virtual String TaskCD
		{
			get
			{
				return this._TaskCD;
			}
			set
			{
				this._TaskCD = value;
			}
		}
		#endregion
		

		#region Description
		public abstract class description : PX.Data.IBqlField
		{
		}
		protected String _Description;
		/// <summary>
		/// Gets or sets description
		/// </summary>
		[PXDBString(250, IsUnicode = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		[PXFieldDescription]
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
		#region CustomerID
		public abstract class customerID : PX.Data.IBqlField
		{
		}
		protected Int32? _CustomerID;
		/// <summary>
		/// Gets or sets customer for the task.
		/// The customer is always set at the project level. Here it is just copied from the project (denormalization). 
		/// </summary>
		[PXDefault(typeof(Search<PMProject.customerID, Where<PMProject.contractID, Equal<Current<PMTask.projectID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[Customer(DescriptionField = typeof(Customer.acctName), Visibility = PXUIVisibility.SelectorVisible, Enabled=false)]
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

		/// <summary>
		/// Gets or sets customer location.
		/// </summary>
		[LocationID(typeof(Where<Location.bAccountID, Equal<Current<PMTask.customerID>>>), Visibility = PXUIVisibility.SelectorVisible, DisplayName = "Location", DescriptionField = typeof(Location.descr))]
		[PXDefault(typeof(Search<PMProject.locationID, Where<PMProject.contractID, Equal<Current<PMTask.projectID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
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
		#region RateTableID
		public abstract class rateTableID : PX.Data.IBqlField
		{
		}
		protected String _RateTableID;

		/// <summary>
		/// Gets or sets Rate table <see cref="PMRateTable"/>
		/// </summary>
		[PXDBString(PMRateTable.rateTableID.Length, IsUnicode = true)]
		[PXDefault(typeof(Search<PMProject.rateTableID, Where<PMProject.contractID, Equal<Current<PMTask.projectID>>, And<PMProject.nonProject, Equal<False>>>>))]
		[PXUIField(DisplayName = "Rate Table")]
        [PXSelector(typeof(PMRateTable.rateTableID))]
        public virtual String RateTableID
		{
			get
			{
                return this._RateTableID;
			}
			set
			{
                this._RateTableID = value;
			}
		}
		#endregion
        #region BillingID
        public abstract class billingID : PX.Data.IBqlField
        {
        }
        protected String _BillingID;

		/// <summary>
		/// Gets or sets Billing rules <see cref="PMBilling"/>
		/// </summary>
        [PXDefault(typeof(PMProject.billingID), PersistingCheck=PXPersistingCheck.Nothing)]
        [PXSelector(typeof(Search<PMBilling.billingID, Where<PMBilling.isActive, Equal<True>>>))]
        [PXUIField(DisplayName = "Billing Rule")]
		[PXDBString(PMBilling.billingID.Length, IsUnicode = true)]
        public virtual String BillingID
        {
            get
            {
                return this._BillingID;
            }
            set
            {
                this._BillingID = value;
            }
        }
        #endregion
		#region AllocationID
		public abstract class allocationID : PX.Data.IBqlField
		{
		}
		protected String _AllocationID;
		/// <summary>
		/// Gets or sets Allocation rules <see cref="PMAllocation"/>
		/// </summary>
		[PXDefault(typeof(PMProject.allocationID), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXSelector(typeof(Search<PMAllocation.allocationID, Where<PMAllocation.isActive, Equal<True>>>))]
		[PXUIField(DisplayName = "Allocation Rule")]
		[PXDBString(PMAllocation.allocationID.Length, IsUnicode = true)]
		public virtual String AllocationID
		{
			get
			{
				return this._AllocationID;
			}
			set
			{
				this._AllocationID = value;
			}
		}
		#endregion
        #region BillingOption
        public abstract class billingOption : PX.Data.IBqlField
        {
        }
        protected String _BillingOption;
		/// <summary>
		/// Gets or sets how the project is billed <see cref="PMBillingOption"/>
		/// </summary>
        [PXDBString(1, IsFixed = true)]
        [PMBillingOption.List()]
        [PXDefault(PMBillingOption.OnBilling)]
        [PXUIField(DisplayName = "Billing Option", Required = true, Visibility = PXUIVisibility.SelectorVisible)]
        public virtual String BillingOption
        {
            get
            {
                return this._BillingOption;
            }
            set
            {
                this._BillingOption = value;
            }
        }
        #endregion
		#region CompletedPctMethod
		public abstract class completedPctMethod : PX.Data.IBqlField
		{
		}
		protected String _CompletedPctMethod;
		/// <summary>
		/// Gets or sets how the calculation method of completion
		/// </summary>
		[PXDBString(1, IsFixed = true)]
		[PMCompletedPctMethod.List()]
		[PXDefault(PMCompletedPctMethod.Manual)]
		[PXUIField(DisplayName = "Completion Calculation Method", Required = true, Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String CompletedPctMethod
		{
			get
			{
				return this._CompletedPctMethod;
			}
			set
			{
				this._CompletedPctMethod = value;
			}
		}
		#endregion
		#region Status
		public abstract class status : PX.Data.IBqlField
		{
		}
		protected String _Status;
		/// <summary>
		/// Gets or sets Task status. <see cref="ProjectTaskStatus"/>
		/// </summary>
		[PXDBString(1, IsFixed = true)]
		[ProjectTaskStatus.List()]
		[PXDefault(ProjectTaskStatus.Planned)]
		[PXUIField(DisplayName = "Status", Required = true, Visibility = PXUIVisibility.SelectorVisible)]
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
		#region PlannedStartDate
		public abstract class plannedStartDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _PlannedStartDate;
		/// <summary>
		/// Gets or sets the date, when the task is suppose to start
		/// </summary>
		[PXDBDate()]
		[PXUIField(DisplayName = "Planned Start")]
		public virtual DateTime? PlannedStartDate
		{
			get
			{
				return this._PlannedStartDate;
			}
			set
			{
				this._PlannedStartDate = value;
			}
		}
		#endregion
		#region PlannedEndDate
		public abstract class plannedEndDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _PlannedEndDate;
		/// <summary>
		///  Gets or sets the date, when the task is suppose to finish
		/// </summary>
		[PXDBDate()]
		[PXVerifyEndDate(typeof(plannedStartDate), AutoChangeWarning = true)]
		[PXUIField(DisplayName = "Planned End")]
		public virtual DateTime? PlannedEndDate
		{
			get
			{
				return this._PlannedEndDate;
			}
			set
			{
				this._PlannedEndDate = value;
			}
		}
		#endregion
		#region StartDate
		public abstract class startDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _StartDate;
		/// <summary>
		///  Gets or sets the actual date, when the task is started
		/// </summary>
		[PXDBDate()]
		[PXUIField(DisplayName = "Start Date")]
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
		#region EndDate
		public abstract class endDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _EndDate;
		/// <summary>
		/// Gets or sets the actual date, when the task is finished.
		/// </summary>
		[PXDBDate()]
		[PXVerifyEndDate(typeof(startDate), AutoChangeWarning = true)]
		[PXUIField(DisplayName = "End Date")]
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
		#region ApproverID
		public abstract class approverID : PX.Data.IBqlField
		{
		}
		protected Int32? _ApproverID;
		/// <summary>
		/// Gets or sets the <see cref="EPEmployee"/> that will approve/reject the activites created under the given task.
		/// If current value is null then appoval is not required; otherwise either <see cref="PMTask.ApproverID"/> or <see cref="PMProject.ApproverID"/> must approve the activity before it can be released to the project.
		/// </summary>
		[PXDBInt]
		[PXEPEmployeeSelector]
		[PXUIField(DisplayName = "Approver", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Int32? ApproverID
		{
			get
			{
				return this._ApproverID;
			}
			set
			{
				this._ApproverID = value;
			}
		}
		#endregion
		#region TaxCategoryID
		public abstract class taxCategoryID : PX.Data.IBqlField
		{
		}
		protected String _TaxCategoryID;
		/// <summary>
		/// Obsolete field. Not used anywere.
		/// </summary>
		[PXDBString(30, IsUnicode = true)]
		[PXUIField(DisplayName = "Tax Category", Visible=false)]//Not used anywere
        [PXSelector(typeof(TaxCategory.taxCategoryID), DescriptionField = typeof(TaxCategory.descr))]
        [PXRestrictor(typeof(Where<TaxCategory.active, Equal<True>>), TX.Messages.InactiveTaxCategory, typeof(TaxCategory.taxCategoryID))]
        public virtual String TaxCategoryID
		{
			get
			{
				return this._TaxCategoryID;
			}
			set
			{
				this._TaxCategoryID = value;
			}
		}
		#endregion
		#region DefaultAccountID
		public abstract class defaultAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _DefaultAccountID;
		/// <summary>
		/// Default Account. Can be used in allocation and/or billing to as a default for the new <see cref="PMTran"/> and <see cref="ARTran"/>.
		/// </summary>
		[PXDefault(typeof(PMProject.defaultAccountID), PersistingCheck = PXPersistingCheck.Nothing)]
		[Account(DisplayName = "Default Account")]
		public virtual Int32? DefaultAccountID
		{
			get
			{
				return this._DefaultAccountID;
			}
			set
			{
				this._DefaultAccountID = value;
			}
		}
		#endregion
		#region DefaultSubID
		public abstract class defaultSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _DefaultSubID;
		/// <summary>
		/// Default Subaccount. Can be used in allocation and/or billing to as a default for the new <see cref="PMTran"/> and <see cref="ARTran"/>.
		/// </summary>
		[PXDefault(typeof(PMProject.defaultSubID), PersistingCheck=PXPersistingCheck.Nothing )]
		[SubAccount(DisplayName = "Default Subaccount", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? DefaultSubID
		{
			get
			{
				return this._DefaultSubID;
			}
			set
			{
				this._DefaultSubID = value;
			}
		}
		#endregion
        #region DefaultAccrualAccountID
        public abstract class defaultAccrualAccountID : PX.Data.IBqlField
        {
        }
        protected Int32? _DefaultAccrualAccountID;
		/// <summary>
		/// Default Accrual Account. Is used depending on the <see cref="PMSetup.ExpenseAccrualSubMask"/> mask setting.
		/// </summary>
        [PXDefault(typeof(PMProject.defaultAccrualAccountID), PersistingCheck = PXPersistingCheck.Nothing)]
        [Account(DisplayName = "Default Accrual Account")]
        public virtual Int32? DefaultAccrualAccountID
        {
            get
            {
                return this._DefaultAccrualAccountID;
            }
            set
            {
                this._DefaultAccrualAccountID = value;
            }
        }
        #endregion
        #region DefaultAccrualSubID
        public abstract class defaultAccrualSubID : PX.Data.IBqlField
        {
        }
        protected Int32? _DefaultAccrualSubID;

		/// <summary>
		/// Default Accrual subaccount. Is used depending on the <see cref="PMSetup.ExpenseAccrualSubMask"/> mask setting.
		/// </summary>
        [PXDefault(typeof(PMProject.defaultAccrualSubID), PersistingCheck = PXPersistingCheck.Nothing)]
        [SubAccount(DisplayName = "Default Accrual Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
        public virtual Int32? DefaultAccrualSubID
        {
            get
            {
                return this._DefaultAccrualSubID;
            }
            set
            {
                this._DefaultAccrualSubID = value;
            }
        }
        #endregion
        #region CompletedPct
		public abstract class completedPct : PX.Data.IBqlField
		{
		}
		protected Int32? _CompletedPct;
		/// <summary>
		/// Gets or sets the Task completion state in %.
		/// Depending on settings this value either maintained manually or can be autocalculated based on the budget ratio of actual vs revised values.
		/// </summary>
		[PXDBInt(MinValue = 0)]
		[PXDefault(0)]
		[PXUIEnabled(typeof(Where<completedPctMethod, Equal<PMCompletedPctMethod.manual>>))]
		[PXUIField(DisplayName = "Completed (%)")]
		public virtual Int32? CompletedPct
		{
			get
			{
				return this._CompletedPct;
			}
			set
			{
				this._CompletedPct = value;
			}
		}
		#endregion
       	#region IsActive
		public abstract class isActive : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsActive;
		/// <summary>
		/// Gets or sets whether the task is active or not. <see cref="PMTran"/> can be created only for active tasks.
		/// </summary>
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Active", Enabled=false, Visibility=PXUIVisibility.Visible, Visible=false)]
		public virtual Boolean? IsActive
		{
			get
			{
				return this._IsActive;
			}
			set
			{
				this._IsActive = value;
			}
		}
		#endregion
		#region IsCompleted
		public abstract class isCompleted : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsCompleted;
		/// <summary>
		/// Gets or sets whether task is completed or not.
		/// </summary>
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Completed", Enabled = false, Visibility = PXUIVisibility.Visible, Visible = false)]
		public virtual Boolean? IsCompleted
		{
			get
			{
				return this._IsCompleted;
			}
			set
			{
				this._IsCompleted = value;
			}
		}
		#endregion
		#region IsCancelled
		public abstract class isCancelled : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsCancelled;
		/// <summary>
		/// Gets or sets whetther the task is cancelled.
		/// </summary>
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Cancelled", Enabled = false, Visibility = PXUIVisibility.Visible, Visible = false)]
		public virtual Boolean? IsCancelled
		{
			get
			{
				return this._IsCancelled;
			}
			set
			{
				this._IsCancelled = value;
			}
		}
		#endregion
		
		#region VisibleInGL
		public abstract class visibleInGL : PX.Data.IBqlField
		{
		}
		protected Boolean? _VisibleInGL;
		/// <summary>
		/// Gets or sets whether the Task is visible in the GL Module.
		/// If Project Task is set as invisible - it will not show up in the field selectors in the given module.
		/// </summary>
		[PXDBBool()]
		[PXDefault(typeof(Search<PMProject.visibleInGL, Where<PMProject.contractID, Equal<Current<PMTask.projectID>>>>))]
		[PXUIField(DisplayName = "GL")]
		public virtual Boolean? VisibleInGL
		{
			get
			{
				return this._VisibleInGL;
			}
			set
			{
				this._VisibleInGL = value;
			}
		}
		#endregion
		#region VisibleInAP
		public abstract class visibleInAP : PX.Data.IBqlField
		{
		}
		protected Boolean? _VisibleInAP;
		/// <summary>
		/// Gets or sets whether the Task is visible in the AP Module.
		/// If Project Task is set as invisible - it will not show up in the field selectors in the given module.
		/// </summary>
		[PXDBBool()]
		[PXDefault(typeof(Search<PMProject.visibleInAP, Where<PMProject.contractID, Equal<Current<PMTask.projectID>>>>))]
		[PXUIField(DisplayName = "AP")]
		public virtual Boolean? VisibleInAP
		{
			get
			{
				return this._VisibleInAP;
			}
			set
			{
				this._VisibleInAP = value;
			}
		}
		#endregion
		#region VisibleInAR
		public abstract class visibleInAR : PX.Data.IBqlField
		{
		}
		protected Boolean? _VisibleInAR;
		/// <summary>
		/// Gets or sets whether the Task is visible in the AR Module.
		/// If Project Task is set as invisible - it will not show up in the field selectors in the given module.
		/// </summary>
		[PXDBBool()]
		[PXDefault(typeof(Search<PMProject.visibleInAR, Where<PMProject.contractID, Equal<Current<PMTask.projectID>>>>))]
		[PXUIField(DisplayName = "AR")]
		public virtual Boolean? VisibleInAR
		{
			get
			{
				return this._VisibleInAR;
			}
			set
			{
				this._VisibleInAR = value;
			}
		}
		#endregion
		#region VisibleInSO
		public abstract class visibleInSO : PX.Data.IBqlField
		{
		}
		protected Boolean? _VisibleInSO;
		/// <summary>
		/// Gets or sets whether the Task is visible in the SO Module.
		/// If Project Task is set as invisible - it will not show up in the field selectors in the given module.
		/// </summary>
		[PXDBBool()]
		[PXDefault(typeof(Search<PMProject.visibleInSO, Where<PMProject.contractID, Equal<Current<PMTask.projectID>>>>))]
		[PXUIField(DisplayName = "SO")]
		public virtual Boolean? VisibleInSO
		{
			get
			{
				return this._VisibleInSO;
			}
			set
			{
				this._VisibleInSO = value;
			}
		}
		#endregion
		#region VisibleInPO
		public abstract class visibleInPO : PX.Data.IBqlField
		{
		}
		protected Boolean? _VisibleInPO;
		/// <summary>
		/// Gets or sets whether the Task is visible in the PO Module.
		/// If Project Task is set as invisible - it will not show up in the field selectors in the given module.
		/// </summary>
		[PXDBBool()]
		[PXDefault(typeof(Search<PMProject.visibleInPO, Where<PMProject.contractID, Equal<Current<PMTask.projectID>>>>))]
		[PXUIField(DisplayName = "PO")]
		public virtual Boolean? VisibleInPO
		{
			get
			{
				return this._VisibleInPO;
			}
			set
			{
				this._VisibleInPO = value;
			}
		}
		#endregion
		#region VisibleInEP
		/// <summary>
		/// Gets or sets whether the Task is visible in the EP Module.
		/// If Project Task is set as invisible - it will not show up in the field selectors in the given module.
		/// </summary>
		public abstract class visibleInEP : PX.Data.IBqlField
		{
		}
		protected Boolean? _VisibleInEP;
		[PXDBBool()]
		[PXDefault(typeof(Search<PMProject.visibleInEP, Where<PMProject.contractID, Equal<Current<PMTask.projectID>>>>))]
		[PXUIField(DisplayName = "TE")]
		public virtual Boolean? VisibleInEP
		{
			get
			{
				return this._VisibleInEP;
			}
			set
			{
				this._VisibleInEP = value;
			}
		}
		#endregion
		#region VisibleInIN
		public abstract class visibleInIN : PX.Data.IBqlField
		{
		}
		protected Boolean? _VisibleInIN;
		/// <summary>
		/// Gets or sets whether the Task is visible in the IN Module.
		/// If Project Task is set as invisible - it will not show up in the field selectors in the given module.
		/// </summary>
		[PXDBBool()]
		[PXDefault(typeof(Search<PMProject.visibleInIN, Where<PMProject.contractID, Equal<Current<PMTask.projectID>>>>))]
		[PXUIField(DisplayName = "IN")]
		public virtual Boolean? VisibleInIN
		{
			get
			{
				return this._VisibleInIN;
			}
			set
			{
				this._VisibleInIN = value;
			}
		}
		#endregion
		#region VisibleInCA
		public abstract class visibleInCA : PX.Data.IBqlField
		{
		}
		protected Boolean? _VisibleInCA;
		/// <summary>
		/// Gets or sets whether the Task is visible in the CA Module.
		/// If Project Task is set as invisible - it will not show up in the field selectors in the given module.
		/// </summary>
		[PXDBBool()]
		[PXDefault(typeof(Search<PMProject.visibleInCA, Where<PMProject.contractID, Equal<Current<PMTask.projectID>>>>))]
		[PXUIField(DisplayName = "CA")]
		public virtual Boolean? VisibleInCA
		{
			get
			{
				return this._VisibleInCA;
			}
			set
			{
				this._VisibleInCA = value;
			}
		}
		#endregion
		#region VisibleInCR
		public abstract class visibleInCR : PX.Data.IBqlField
		{
		}
		protected Boolean? _VisibleInCR;
		/// <summary>
		/// Gets or sets whether the Task is visible in the CR Module.
		/// If Project Task is set as invisible - it will not show up in the field selectors in the given module.
		/// </summary>
		[PXDBBool()]
		[PXDefault(typeof(Search<PMProject.visibleInCR, Where<PMProject.contractID, Equal<Current<PMTask.projectID>>>>))]
		[PXUIField(DisplayName = "CR")]
		public virtual Boolean? VisibleInCR
		{
			get
			{
				return this._VisibleInCR;
			}
			set
			{
				this._VisibleInCR = value;
			}
		}
		#endregion
        #region AutoIncludeInPrj
        public abstract class autoIncludeInPrj : IBqlField
        {
        }
        protected bool? _AutoIncludeInPrj;
		/// <summary>
		/// Gets or sets whether to autocreate this task when a template is assigned to a Project.
		/// This field is for Project Templates.
		/// </summary>
        [PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Automatically Include in Project")]
        public virtual bool? AutoIncludeInPrj
        {
            get
            {
                return _AutoIncludeInPrj;
            }
            set
            {
                _AutoIncludeInPrj = value;
            }
        }
        #endregion
		#region LineCtr
		public abstract class lineCtr : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// Gets or sets linecounter for <see cref="PMDetail"/>
		/// </summary>
		[PXDBInt()]
		[PXDefault(0)]
		public virtual int? LineCtr { get; set; }
		#endregion
		#region ProjectManagerID
		public abstract class projectManagerID : PX.Data.IBqlField
		{
		}
		protected Int32? _ProjectManagerID;

		[PXEPEmployeeSelector]
		[PXFormula(typeof(Selector<projectID, PMProject.approverID>))]
		[PXInt]
		[PXUIField(DisplayName = "Project Manager")]
		public virtual Int32? ProjectManagerID
		{
			get
			{
				return this._ProjectManagerID;
			}
			set
			{
				this._ProjectManagerID = value;
			}
		}
		#endregion
		
		#region Attributes
		public abstract class attributes : IBqlField
		{
		}

		/// <summary>
		/// Gets or sets entity attributes.
		/// </summary>
		[CRAttributesField(typeof(PMTask.classID))]
		public virtual string[] Attributes { get; set; }

		#region ClassID

		public abstract class classID : IBqlField
		{
		}
		/// <summary>
		/// Gets ClassID for the attributes. Always returns <see cref="GroupTypes.Task"/>
		/// </summary>
		[PXString(20)]
		public virtual string ClassID
		{
			get { return GroupTypes.Task; }
		}
		#endregion

		#endregion

		#region templateID
		public abstract class templateID : IBqlField
		{
		}
		[PXInt]
		public virtual int? TemplateID { get; set; }
		#endregion

        #region System Columns
        #region NoteID
        public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXNote(DescriptionField = typeof(PMTask.taskCD))]
		public virtual Guid? NoteID
		{
			get
			{
				return this._NoteID;
			}
			set
			{
				this._NoteID = value;
			}
		}
		#endregion
		#region tstamp
		public abstract class Tstamp : PX.Data.IBqlField
		{
		}
		protected Byte[] _tstamp;
		[PXDBTimestamp()]
		public virtual Byte[] tstamp
		{
			get
			{
				return this._tstamp;
			}
			set
			{
				this._tstamp = value;
			}
		}
		#endregion
		#region CreatedByID
		public abstract class createdByID : PX.Data.IBqlField
		{
		}
		protected Guid? _CreatedByID;
		[PXDBCreatedByID()]
		public virtual Guid? CreatedByID
		{
			get
			{
				return this._CreatedByID;
			}
			set
			{
				this._CreatedByID = value;
			}
		}
		#endregion
		#region CreatedByScreenID
		public abstract class createdByScreenID : PX.Data.IBqlField
		{
		}
		protected String _CreatedByScreenID;
		[PXDBCreatedByScreenID()]
		public virtual String CreatedByScreenID
		{
			get
			{
				return this._CreatedByScreenID;
			}
			set
			{
				this._CreatedByScreenID = value;
			}
		}
		#endregion
		#region CreatedDateTime
		public abstract class createdDateTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _CreatedDateTime;
		[PXDBCreatedDateTime()]
		public virtual DateTime? CreatedDateTime
		{
			get
			{
				return this._CreatedDateTime;
			}
			set
			{
				this._CreatedDateTime = value;
			}
		}
		#endregion
		#region LastModifiedByID
		public abstract class lastModifiedByID : PX.Data.IBqlField
		{
		}
		protected Guid? _LastModifiedByID;
		[PXDBLastModifiedByID()]
		public virtual Guid? LastModifiedByID
		{
			get
			{
				return this._LastModifiedByID;
			}
			set
			{
				this._LastModifiedByID = value;
			}
		}
		#endregion
		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : PX.Data.IBqlField
		{
		}
		protected String _LastModifiedByScreenID;
		[PXDBLastModifiedByScreenID()]
		public virtual String LastModifiedByScreenID
		{
			get
			{
				return this._LastModifiedByScreenID;
			}
			set
			{
				this._LastModifiedByScreenID = value;
			}
		}
		#endregion
		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _LastModifiedDateTime;
		[PXDBLastModifiedDateTime()]
		public virtual DateTime? LastModifiedDateTime
		{
			get
			{
				return this._LastModifiedDateTime;
			}
			set
			{
				this._LastModifiedDateTime = value;
			}
		}
		#endregion
		#endregion

	}

    public static class ProjectTaskStatus
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Planned, Active, Canceled, Completed },
				new string[] { Messages.InPlanning, Messages.Active, Messages.Canceled, Messages.Completed }) { ; }
		}
		public const string Planned = "D";
		public const string Active = "A";
		public const string Canceled = "C";
		public const string Completed = "F";
		
	    public class planned : Constant<String>
	    {
		    public planned() : base(Planned){;}
		}

		public class active : Constant<string>
		{
			public active() : base(Active) { ;}
		}

		public class completed : Constant<string>
		{
			public completed() : base(Completed) { ;}
		}
		
	}

    public static class PMBillingOption
    {
        public class ListAttribute : PXStringListAttribute
        {
            public ListAttribute()
                : base(
                new string[] { OnBilling, OnTaskCompletion, OnProjectCompetion },
                new string[] { Messages.OnBilling, Messages.OnTaskCompletion, Messages.OnProjectCompetion }) { ; }
        }
        public const string OnBilling = "B";
        public const string OnTaskCompletion = "T";
        public const string OnProjectCompetion = "P";

		public class onBilling : Constant<string>
		{
			public onBilling() : base(OnBilling) {; }
		}
		public class onTaskCompletion : Constant<string>
		{
			public onTaskCompletion() : base(OnTaskCompletion) {; }
		}
		public class onProjectCompetion : Constant<string>
		{
			public onProjectCompetion() : base(OnProjectCompetion) {; }
		}
    }

	public static class PMCompletedPctMethod
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Manual, ByQuantity, ByAmount },
				new string[] { Messages.Manual, Messages.ByQuantity, Messages.ByAmount }) { ; }
		}
		public const string Manual = "M";
		public const string ByQuantity = "Q";
		public const string ByAmount = "A";

		public class manual : Constant<string>
		{
			public manual() : base(Manual) { ;}
		}

		public class byQuantity : Constant<string>
		{
			public byQuantity() : base(ByQuantity) { ;}
		}

		public class byAmount : Constant<string>
		{
			public byAmount() : base(ByAmount) { ;}
		}
	}
}
