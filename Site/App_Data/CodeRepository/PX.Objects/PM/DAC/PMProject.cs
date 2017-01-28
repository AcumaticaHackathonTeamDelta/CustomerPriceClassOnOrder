using System;
using PX.Data;
using PX.Objects.CT;
using PX.Data.EP;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.CS;
using PX.Objects.CM;
using System.Collections.Generic;
using System.Text;

namespace PX.Objects.PM
{
	/// <summary>
	/// Planned set of interrelated tasks to be executed over a fixed period and within certain cost and other limitations. 
	/// Each project consists of tasks that need to be completed to complete the project. 
	/// Project budget, profitability and balances are monitored in scope of account groups.
	/// </summary>
	[PXCacheName(Messages.Project)]
    [Serializable]
	[PXEMailSource]
	public partial class PMProject : Contract, IAssign, PX.SM.IIncludable
	{
		public class ProjectBaseType : Constant<string>
		{
			public const string Project = "P";
			public ProjectBaseType() : base(Project) { ;}
		}
		
		#region ContractID
		public new abstract class contractID : PX.Data.IBqlField
		{
		}

		/// <summary>
		/// Get or sets ProjectID
		/// </summary>
		[PXDBIdentity()]
		[PXSelector(typeof(Search<PMProject.contractID, Where<PMProject.baseType, Equal<PMProject.ProjectBaseType>, And<PMProject.isTemplate, NotEqual<True>>>>))]
		[PXUIField(DisplayName = "Project ID")]
		public override Int32? ContractID
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
		#region ContractCD
		public new abstract class contractCD : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// Gets or sets ProjectCD. 
		/// This is a segmented key and format is configured under segmented key maintenance screen in CS module.
		/// </summary>
		[PXDimensionSelector(ProjectAttribute.DimensionName,
			typeof(Search<PMProject.contractCD, Where<PMProject.baseType, Equal<PMProject.ProjectBaseType>, And<PMProject.nonProject, Equal<False>, And<PMProject.isTemplate, NotEqual<True>, And<Match<Current<AccessInfo.userName>>>>>>>),
			typeof(PMProject.contractCD),
			typeof(PMProject.contractCD), typeof(PMProject.customerID), typeof(PMProject.description), typeof(PMProject.status), Filterable = true)]
		[PXDBString(IsUnicode = true, IsKey = true, InputMask = "")]
		[PXDefault()]
		[PXUIField(DisplayName = "Project ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXFieldDescription]
		public override String ContractCD
		{
			get
			{
				return this._ContractCD;
			}
			set
			{
				this._ContractCD = value;
			}
		}
		#endregion
		#region Description
		public new abstract class description : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// Gets or sets Project description
		/// </summary>
		[PXDBString(60, IsUnicode = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		[PXFieldDescription]
		public override String Description
		{
			get
			{
				return _Description;
			}
			set
			{
				_Description = value;
			}
		}
		#endregion
		#region OriginalContractID
		public new abstract class originalContractID : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// This field in not used with Projects. 
		/// </summary>
		[PXDBInt()]
		public override Int32? OriginalContractID
		{
			get
			{
				return this._OriginalContractID;
			}
			set
			{
				this._OriginalContractID = value;
			}
		}
		#endregion
		#region MasterContractID
		public new abstract class masterContractID : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// This field in not used with Projects. 
		/// </summary>
		[PXDBInt()]
		public override Int32? MasterContractID
		{
			get
			{
				return this._MasterContractID;
			}
			set
			{
				this._MasterContractID = value;
			}
		}
		#endregion
		#region CaseItemID
		public new abstract class caseItemID : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// This field in not used with Projects. 
		/// </summary>
		[PXDBInt()]
		public override Int32? CaseItemID
		{
			get
			{
				return this._CaseItemID;
			}
			set
			{
				this._CaseItemID = value;
			}
		}
		#endregion
		#region BaseType
		public new abstract class baseType : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// Gets or sets the type of the record. It can be either Contract or Project.
		/// The default value is Project.
		/// </summary>
		[PXDBString(1, IsFixed = true)]
		[PXDefault(ProjectBaseType.Project)]
        [PXUIField(DisplayName = "Base Type")]
        public override String BaseType
		{
			get
			{
				return this._BaseType;
			}
			set
			{
				this._BaseType = value;
			}
		}
		#endregion
		#region Type
		public new abstract class type : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// This field in not used with Projects. 
		/// </summary>
		[PXDBString(1, IsFixed = true)]
		[Contract.type.List]
		[PXDefault(Contract.type.Renewable)]
		public override String Type
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
		#region CustomerID
		public new abstract class customerID : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// Gets or sets Customer for a Project. Project can be oof 2 types - internal and external.
		/// Internal projects are those that have this value = NULL and hense are not billable.
		/// </summary>
		[CustomerActive(DescriptionField = typeof(Customer.acctName))]
		public override Int32? CustomerID
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
		public new abstract class locationID : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// Gets or sets the Customer Location.
		/// </summary>
        [LocationID(typeof(Where<Location.bAccountID, Equal<Current<customerID>>>), DisplayName = "Customer Location", DescriptionField = typeof(Location.descr))]
		[PXDefault(typeof(Search<Customer.defLocationID, Where<Customer.bAccountID, Equal<Current<customerID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public override Int32? LocationID
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
		#region DefaultAccountID
		public new abstract class defaultAccountID : PX.Data.IBqlField
		{
		}
		#endregion
		#region DefaultSubID
		public new abstract class defaultSubID : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// Default Sub Account Associated with the project. This Subaccount can be later used in allocation and billing rules.
		/// </summary>
		[PXDefault(typeof(Search<Location.cSalesSubID, Where<Location.bAccountID, Equal<Current<PMProject.customerID>>, And<Location.locationID, Equal<Current<PMProject.locationID>>>>>))]
		[SubAccount(DisplayName = "Default Subaccount", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public override Int32? DefaultSubID
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
        public new abstract class defaultAccrualAccountID : PX.Data.IBqlField
        {
        }
        #endregion
        #region DefaultAccrualSubID
        public new abstract class defaultAccrualSubID : PX.Data.IBqlField
        {
        }
		/// <summary>
        /// Default Project Accrual Subaccount. Is used depending on the <see cref="PMSetup.ExpenseAccrualSubMask"/> mask setting.
		/// </summary>
        [SubAccount(DisplayName = "Default Accrual Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
        public override Int32? DefaultAccrualSubID
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
        #region BillingID
		public new abstract class billingID : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// Gets or sets the Billing Rule <see cref="PMBilling"/> for the Project. 
		/// Billing rule is set at the <see cref="PMTask"/> level. Here its just a default value for the Tasks created under the given project.
		/// </summary>
        [PXSelector(typeof(Search<PMBilling.billingID, Where<PMBilling.isActive, Equal<True>>>))]
		[PXUIField(DisplayName="Billing Rule")]
		[PXDBString(PMBilling.billingID.Length, IsUnicode = true)]
		public override String BillingID
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
		public new abstract class allocationID : PX.Data.IBqlField
		{
		}

		/// <summary>
		/// Gets or sets the Allocation Rule <see cref="PMAllocation"/> for the Project. 
		/// Allocation rule is set at the <see cref="PMTask"/> level. Here its just a default value for the Tasks created under the given project. 
		/// </summary>
		[PXSelector(typeof(Search<PMAllocation.allocationID, Where<PMAllocation.isActive, Equal<True>>>))]
		[PXUIField(DisplayName = "Allocation Rule")]
		[PXDBString(PMAllocation.allocationID.Length, IsUnicode = true)]
		public override String AllocationID
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
		#region ApproverID
		public new abstract class approverID : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// Gets or Sets Project manager for the Project. Project manager can Approve and Reject activities that require approval. 
		/// Activity required approval if and only if the <see cref="PMTask.ApproverID"/> is set for a given <see cref="PMTask"/> 
		/// </summary>
		[PXDBInt]
		[PXEPEmployeeSelector]
		[PXUIField(DisplayName = "Project Manager", Visibility = PXUIVisibility.SelectorVisible)]
		public override Int32? ApproverID
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
		#region WorkgroupID
		public new abstract class workgroupID : PX.Data.IBqlField
		{
		}
		#endregion
		#region OwnerID
		public new abstract class ownerID : IBqlField
		{
		}
		#endregion
        #region RateTableID
        public new abstract class rateTableID : PX.Data.IBqlField
        {}

		/// <summary>
		/// Gets or sets the RateTable <see cref="PMRateTable"/> for the project. 
		/// </summary>
        [PXDBString(PMRateTable.rateTableID.Length, IsUnicode = true)]
        [PXUIField(DisplayName = "Rate Table")]
        [PXSelector(typeof(PMRateTable.rateTableID))]
        public override String RateTableID
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
		#region TemplateID
		public new abstract class templateID : PX.Data.IBqlField
		{
		}

		/// <summary>
		/// Gets or sets Template for the project.
		/// </summary>
        [Project(typeof(Where<PMProject.isTemplate, Equal<True>, And<PMProject.isActive, Equal<True>>>), DisplayName = "Template ID")]
        public override Int32? TemplateID
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
		public new abstract class status : PX.Data.IBqlField
		{
		}

		/// <summary>
		/// Gets or sets status of the Project. <see cref="ProjectStatus"/>
		/// </summary>
		[PXDBString(1, IsFixed = true)]
		[ProjectStatus.List()]
		[PXDefault(ProjectStatus.Planned)]
		[PXUIField(DisplayName = "Status", Required = true, Visibility = PXUIVisibility.SelectorVisible)]
		public override String Status
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
		#region Duration
		public new abstract class duration : PX.Data.IBqlField
		{
		}

		/// <summary>
		/// This field in not used with Projects. 
		/// </summary>
		[PXDBInt()]
		public override Int32? Duration
		{
			get
			{
				return this._Duration;
			}
			set
			{
				this._Duration = value;
			}
		}
		#endregion
		#region DurationType
		public new abstract class durationType : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// This field in not used with Projects. 
		/// </summary>
		[PXDBString(1, IsFixed = true)]
		public override string DurationType
		{
			get
			{
				return this._DurationType;
			}
			set
			{
				this._DurationType = value;
			}
		}
		#endregion
		#region StartDate
		public new abstract class startDate : PX.Data.IBqlField
		{
		}

		/// <summary>
		/// Gets or sets Start date of a project.
		/// </summary>
		[PXDefault]
		[PXDBDate()]
		[PXUIField(DisplayName = "Start Date")]
		public override DateTime? StartDate
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
		#region ExpireDate
		public new abstract class expireDate : PX.Data.IBqlField
		{
		}

		/// <summary>
		/// Gets or sets End date of a project.
		/// </summary>
		[PXDBDate()]
		[PXUIField(DisplayName = "End Date", Visibility = PXUIVisibility.SelectorVisible)]
		public override DateTime? ExpireDate
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
		#region GracePeriod
		public new abstract class gracePeriod : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// This field in not used with Projects. 
		/// </summary>
		[PXDBInt(MinValue = 0, MaxValue = 365)]
		[PXDefault(0)]
		public override Int32? GracePeriod
		{
			get
			{
				return this._GracePeriod;
			}
			set
			{
				this._GracePeriod = value;
			}
		}
		#endregion
		#region AutoRenew
		public new abstract class autoRenew : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// This field in not used with Projects. 
		/// </summary>
		[PXDBBool()]
		[PXDefault(false)]
		public override Boolean? AutoRenew
		{
			get
			{
				return this._AutoRenew;
			}
			set
			{
				this._AutoRenew = value;
			}
		}
		#endregion
		#region AutoRenewDays
		public new abstract class autoRenewDays : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// This field in not used with Projects. 
		/// </summary>
		[PXDBInt(MinValue = 0, MaxValue = 365)]
		[PXDefault(0)]
		public override Int32? AutoRenewDays
		{
			get
			{
				return this._AutoRenewDays;
			}
			set
			{
				this._AutoRenewDays = value;
			}
		}
		#endregion
		#region IsTemplate
		public new abstract class isTemplate : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// Gets or sets whether current record is a Template or a Project.
		/// </summary>
		[PXDBBool()]
		[PXDefault(false)]
		public override Boolean? IsTemplate
		{
			get
			{
				return this._IsTemplate;
			}
			set
			{
				this._IsTemplate = value;
			}
		}
		#endregion

		#region RestrictToEmployeeList
		public new abstract class restrictToEmployeeList : PX.Data.IBqlField
		{
		}
		#endregion
		#region RestrictToResourceList
		public new abstract class restrictToResourceList : PX.Data.IBqlField
		{
		}
		#endregion

		#region DetailedBilling
		public new abstract class detailedBilling : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// This field in not used with Projects. 
		/// </summary>
		[PXDBInt()]
		[PXDefault(Contract.detailedBilling.Summary)]
		public override Int32? DetailedBilling
		{
			get
			{
				return this._DetailedBilling;
			}
			set
			{
				this._DetailedBilling = value;
			}
		}
		#endregion
		#region AllowOverride
		public new abstract class allowOverride : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// This field in not used with Projects. 
		/// </summary>
		[PXDBBool()]
		[PXDefault(false)]
		public override Boolean? AllowOverride
		{
			get
			{
				return this._AllowOverride;
			}
			set
			{
				this._AllowOverride = value;
			}
		}
		#endregion
		#region RefreshOnRenewal
		public new abstract class refreshOnRenewal : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// This field in not used with Projects. 
		/// </summary>
		[PXDBBool()]
		[PXDefault(false)]
		public override Boolean? RefreshOnRenewal
		{
			get
			{
				return this._RefreshOnRenewal;
			}
			set
			{
				this._RefreshOnRenewal = value;
			}
		}
		#endregion
		#region IsContinuous
		public new abstract class isContinuous : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// This field in not used with Projects. 
		/// </summary>
		[PXDBBool()]
		[PXDefault(false)]
		public override Boolean? IsContinuous
		{
			get
			{
				return this._IsContinuous;
			}
			set
			{
				this._IsContinuous = value;
			}
		}
		#endregion
        
        #region Asset
        public abstract class asset : PX.Data.IBqlField
        {
        }
		/// <summary>
		/// Gets or sets Total Asset balance for the project.
		/// This value is calculated on the fly only in the <see cref="ProjectEntry"/> graph and is not stored in the DB. 
		/// </summary>
        protected Decimal? _Asset;
        [PXBaseCury()]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck=PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Assets", Enabled = false)]
        public virtual Decimal? Asset
        {
            get
            {
                return this._Asset;
            }
            set
            {
                this._Asset = value;
            }
        }
        #endregion
        #region Liability
        public abstract class liability : PX.Data.IBqlField
        {
        }
        protected Decimal? _Liability;
		/// <summary>
		/// Gets or sets Total Liability balance for the project.
		/// This value is calculated on the fly only in the <see cref="ProjectEntry"/> graph and is not stored in the DB. 
		/// </summary>
        [PXBaseCury()]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Liabilities", Enabled = false)]
        public virtual Decimal? Liability
        {
            get
            {
                return this._Liability;
            }
            set
            {
                this._Liability = value;
            }
        }
        #endregion
        #region Income
        public abstract class income : PX.Data.IBqlField
        {
        }
        protected Decimal? _Income;
		/// <summary>
		/// Gets or sets Total Income balance for the project.
		/// This value is calculated on the fly only in the <see cref="ProjectEntry"/> graph and is not stored in the DB. 
		/// </summary>
        [PXBaseCury()]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Income", Enabled = false)]
        public virtual Decimal? Income
        {
            get
            {
                return this._Income;
            }
            set
            {
                this._Income = value;
            }
        }
        #endregion
        #region Expense
        public abstract class expense : PX.Data.IBqlField
        {
        }
        protected Decimal? _Expense;
		/// <summary>
		/// Gets or sets Total Expense balance for the project.
		/// This value is calculated on the fly only in the <see cref="ProjectEntry"/> graph and is not stored in the DB. 
		/// </summary>
        [PXBaseCury()]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Expenses", Enabled = false)]
        public virtual Decimal? Expense
        {
            get
            {
                return this._Expense;
            }
            set
            {
                this._Expense = value;
            }
        }
        #endregion

		#region IsActive
		public new abstract class isActive : PX.Data.IBqlField
		{
		}

		/// <summary>
		/// Gets or sets whether Project is active or not. Transactions can be added only to the active projects.
		/// </summary>
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Active", Enabled=false, Visible=false, Visibility=PXUIVisibility.Visible)]
		public override Boolean? IsActive
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
		public new abstract class isCompleted : PX.Data.IBqlField
		{
		}

		/// <summary>
		/// Gets or sets whether project is Completed or not.
		/// </summary>
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Completed", Enabled = false, Visible = false, Visibility = PXUIVisibility.Visible)]
		public override Boolean? IsCompleted
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
        #region AutoAllocate
        public new abstract class autoAllocate : PX.Data.IBqlField
        {
        }

		/// <summary>
		/// Gets or sets whether to run allocation everytime a <see cref="PMTran"/> is released.
		/// </summary>
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Run Allocation on PM Transaction Release")]
        public override Boolean? AutoAllocate
        {
            get
            {
                return this._AutoAllocate;
            }
            set
            {
                this._AutoAllocate = value;
            }
        }
        #endregion

		#region VisibleInGL
		public new abstract class visibleInGL : PX.Data.IBqlField
		{
		}

		/// <summary>
		/// Gets or sets whether the Project is visible in the GL Module.
		/// If Project is set as invisible - it will not show up in the field selectors in the given module.
		/// </summary>
		[PXDBBool()]
		[PXDefault(typeof(Search<PMSetup.visibleInGL>))]
		[PXUIField(DisplayName = "GL")]
		public override Boolean? VisibleInGL
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
		public new abstract class visibleInAP : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// Gets or sets whether the Project is visible in the AP Module.
		/// If Project is set as invisible - it will not show up in the field selectors in the given module.
		/// </summary>
		[PXDBBool()]
		[PXDefault(typeof(Search<PMSetup.visibleInAP>))]
		[PXUIField(DisplayName = "AP")]
		public override Boolean? VisibleInAP
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
		public new abstract class visibleInAR : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// Gets or sets whether the Project is visible in the AR Module.
		/// If Project is set as invisible - it will not show up in the field selectors in the given module.
		/// </summary>
		[PXDBBool()]
		[PXDefault(typeof(Search<PMSetup.visibleInAR>))]
		[PXUIField(DisplayName = "AR")]
		public override Boolean? VisibleInAR
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
		public new abstract class visibleInSO : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// Gets or sets whether the Project is visible in the SO Module.
		/// If Project is set as invisible - it will not show up in the field selectors in the given module.
		/// </summary>
		[PXDBBool()]
		[PXDefault(typeof(Search<PMSetup.visibleInSO>))]
		[PXUIField(DisplayName = "SO")]
		public override Boolean? VisibleInSO
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
		public new abstract class visibleInPO : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// Gets or sets whether the Project is visible in the PO Module.
		/// If Project is set as invisible - it will not show up in the field selectors in the given module.
		/// </summary>
		[PXDBBool()]
		[PXDefault(typeof(Search<PMSetup.visibleInPO>))]
		[PXUIField(DisplayName = "PO")]
		public override Boolean? VisibleInPO
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
		public new abstract class visibleInEP : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// Gets or sets whether the Project is visible in the EP Module.
		/// If Project is set as invisible - it will not show up in the field selectors in the given module.
		/// </summary>
		[PXDBBool()]
		[PXDefault(typeof(Search<PMSetup.visibleInEP>))]
		[PXUIField(DisplayName = "TE")]
		public override Boolean? VisibleInEP
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
		public new abstract class visibleInIN : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// Gets or sets whether the Project is visible in the IN Module.
		/// If Project is set as invisible - it will not show up in the field selectors in the given module.
		/// </summary>
		[PXDBBool()]
		[PXDefault(typeof(Search<PMSetup.visibleInIN>))]
		[PXUIField(DisplayName = "IN")]
		public override Boolean? VisibleInIN
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
		public new abstract class visibleInCA : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// Gets or sets whether the Project is visible in the CA Module.
		/// If Project is set as invisible - it will not show up in the field selectors in the given module.
		/// </summary>
		[PXDBBool()]
		[PXDefault(typeof(Search<PMSetup.visibleInCA>))]
		[PXUIField(DisplayName = "CA")]
		public override Boolean? VisibleInCA
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
		public new abstract class visibleInCR : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// Gets or sets whether the Project is visible in the CR Module.
		/// If Project is set as invisible - it will not show up in the field selectors in the given module.
		/// </summary>
		[PXDBBool()]
		[PXDefault(typeof(Search<PMSetup.visibleInCR>))]
		[PXUIField(DisplayName = "CR")]
		public override Boolean? VisibleInCR
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
		#region NonProject
		public new abstract class nonProject : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// Only one project in the system is a Non-Project. A non-project is used whenever you have a transaction that is not applicable to any other project.
		/// </summary>
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Is Global", Visibility = PXUIVisibility.Visible, Visible = false)]
        public override Boolean? NonProject
        {
            get
            {
                return this._NonProject;
            }
            set
            {
                this._NonProject = value;
            }
        }
        #endregion
		#region NoteID
		public new abstract class noteID : IBqlField{}

		[PXSearchable(SM.SearchCategory.PM, Messages.ProjectSearchTitle, new Type[] { typeof(PMProject.contractCD), typeof(PMProject.customerID), typeof(BAccount.acctName) },
		   new Type[] { typeof(PMProject.description) },
		   NumberFields = new Type[] { typeof(PMProject.contractCD) },
		   Line1Format = "{0}{1}{2:d}", Line1Fields = new Type[] { typeof(PMProject.templateID), typeof(PMProject.status), typeof(PMProject.startDate) },
		   Line2Format = "{0}", Line2Fields = new Type[] { typeof(PMProject.description) },
		   WhereConstraint = typeof(Where<Current<PMProject.baseType>, Equal<ProjectBaseType>, And<Current<PMProject.isTemplate>, NotEqual<True>, And<Current<PMProject.nonProject>, NotEqual<True>>>>),
		   MatchWithJoin = typeof(LeftJoin<Customer, On<Customer.bAccountID, Equal<PMProject.customerID>>>)
		)]
		[PXNote(DescriptionField = typeof(PMProject.contractCD))]
		public override Guid? NoteID
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


		#region ServiceActivate
		public new abstract class serviceActivate : PX.Data.IBqlField
		{
		}
		/// <summary>
		/// This field in not used with Projects. 
		/// </summary>
		[PXDBBool()]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		public override Boolean? ServiceActivate
		{
			get
			{
				return this._ServiceActivate;
			}
			set
			{
				this._ServiceActivate = value;
			}
		}
		#endregion

		#region Attributes
		public new abstract class attributes : IBqlField
		{
		}
		/// <summary>
		/// Gets or sets entity attributes.
		/// </summary>
		[CRAttributesField(typeof(PMProject.classID), typeof(Contract.noteID))]
		public override string[] Attributes { get; set; }

		#region ClassID
		public new abstract class classID : IBqlField
		{
		}
		/// <summary>
		/// Gets ClassID for the attributes. Always returns current <see cref="GroupTypes.Project"/>
		/// </summary>
		[PXString(20)]
		public override string ClassID
		{
			get { return GroupTypes.Project; }
		}

		#endregion
	
		#endregion

		#region GroupMask
		public new abstract class groupMask : IBqlField { }
		#endregion
		#region Included
		public abstract class included : PX.Data.IBqlField
		{
		}
		protected bool? _Included;
		[PXBool]
		[PXUIField(DisplayName = "Included")]
		[PXUnboundDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual bool? Included
		{
			get
			{
				return this._Included;
			}
			set
			{
				this._Included = value;
			}
		}
		#endregion
	}

	public static class ProjectStatus
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Planned, Active, Completed, Cancelled, OnHold, PendingApproval, Contract.status.InUpgrade },
				new string[] { Messages.InPlanning, Messages.Active, Messages.Completed, Messages.Canceled, Messages.Suspend, Messages.PendingApproval, CT.Messages.InUpgrade }) { }
		}

        public class TemplStatusListAttribute : PXStringListAttribute
        {
            public TemplStatusListAttribute()
                : base(
				new string[] { Active, OnHold },
                new string[] { Messages.Active, Messages.OnHold }) {}
        }

		public const string Planned = Contract.status.Draft;
		public const string Active = Contract.status.Active;
		public const string Completed = Contract.status.Completed;
        public const string OnHold = Contract.status.Expired;
		public const string Cancelled = Contract.status.Canceled;
		public const string PendingApproval = Contract.status.InApproval;

		public class active : Constant<string>
		{
			public active() : base(Active) { ;}
		}

		public class completed : Constant<string>
		{
			public completed() : base(Completed) { ;}
		}

	}



	public sealed class NonProject : IBqlCreator, IBqlOperand
	{
		public void Parse(PXGraph graph, List<IBqlParameter> pars, List<Type> tables, List<Type> fields, List<IBqlSortColumn> sortColumns, StringBuilder text, BqlCommand.Selection selection)
		{
		}

		public void Verify(PXCache cache, object item, List<object> pars, ref bool? result, ref object value)
		{
			value = ID;
		}
		
		public static int ID
		{
			get
			{
				ProjectDefinition def = PXDatabase.GetSlot<ProjectDefinition>(typeof (NonProject).FullName);
				return def.ID;
			}
		}

		private class ProjectDefinition : IPrefetchable 
		{
			public int ID;

			public void Prefetch()
			{
			 using (PXConnectionScope s = new PXConnectionScope())
				{
					using (PXDataRecord record = PXDatabase.SelectSingle<Contract>(
						new PXDataField<Contract.contractID>(),
						new PXDataFieldValue<Contract.nonProject>(1)))
					{
						ID = record.GetInt32(0) ?? 0;
					}					
				}
			}

		}
	}

}
