using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.IN;

namespace PX.Objects.CS
{
	[PXCacheName(Messages.FeaturesSet)]
	[PXPrimaryGraph(typeof(FeaturesMaint))]
    [Serializable]
	public class FeaturesSet : IBqlTable
	{
		#region LicenseID
		public abstract class licenseID : PX.Data.IBqlField
		{
		}
		protected String _LicenseID;
		[PXString(64, IsUnicode = true, InputMask = "")]
		[PXUIField(DisplayName = "License ID", Visible = false)]
		public virtual String LicenseID
		{
			get
			{
				return this._LicenseID;
			}
			set
			{
				this._LicenseID = value;
			}
		}
		#endregion
		#region Status
		public abstract class status : IBqlField
		{
		}
		[PXDBInt(IsKey = true)]
		[PXDefault(3)]
		[PXIntList(
			new int[] { 0, 1, 2, 3 },
			new string[] { "Validated", "Failed Validation", "Pending Validation", "Pending Activation" }
		)]
		[PXUIField(DisplayName = "Status", Enabled = false)]
		public int? Status
		{
			get;
			set;
		}
		#endregion
		#region ValidUntill
		public abstract class validUntill : PX.Data.IBqlField
		{
		}
		protected DateTime? _ValidUntill;
		[PXDBDate()]
		[PXUIField(DisplayName = "Next Validation Date", Enabled = false, Visible = false)]
		public virtual DateTime? ValidUntill
		{
			get
			{
				return this._ValidUntill;
			}
			set
			{
				this._ValidUntill = value;
			}
		}
		#endregion		
		#region ValidationCode
		public abstract class validationCode : PX.Data.IBqlField
		{
		}
		protected String _ValidationCode;
		[PXString(500, IsUnicode = true, InputMask = "")]
		public virtual String ValidationCode
		{
			get
			{
				return this._ValidationCode;
			}
			set
			{
				this._ValidationCode = value;
			}
		}
		#endregion

        #region FinancialModule
        public abstract class financialModule : PX.Data.IBqlField
		{
		}
        protected bool? _FinancialModule;
        [Feature(true, null, typeof(Select<GL.GLSetup>), DisplayName = "Finance", Enabled = false)]
        public virtual bool? FinancialModule
		{
			get
			{
                return this._FinancialModule;
			}
			set
			{
                this._FinancialModule = value;
			}
		}
		#endregion
        #region FinancialStandard
        public abstract class financialStandard : PX.Data.IBqlField
		{
		}
        protected bool? _FinancialStandard;
        [Feature(true, typeof(FeaturesSet.financialModule), null, DisplayName = "Standard Financials", SyncToParent = true)]
        public virtual bool? FinancialStandard
		{
			get
			{
                return this._FinancialStandard;
			}
			set
			{
                this._FinancialStandard = value;
			}
		}
		#endregion
		#region Branch
		public abstract class branch : PX.Data.IBqlField
		{
		}
		protected bool? _Branch;
        [Feature(typeof(FeaturesSet.financialStandard), typeof(Select2<CS.BranchMaint.BranchBAccount,
			InnerJoin<GL.Branch, On<GL.Branch.bAccountID, NotEqual<CS.BranchMaint.BranchBAccount.bAccountID>>>>), 
			DisplayName = "Multi-Branch Support")]
		public virtual bool? Branch
		{
			get
			{
				return this._Branch;
			}
			set
			{
				this._Branch = value;
			}
		}
		#endregion
        #region AccountLocations
        public abstract class accountLocations : PX.Data.IBqlField
        {
        }
        protected bool? _AccountLocations;
        [Feature(false, typeof(FeaturesSet.financialStandard), DisplayName = "Business Account Locations")]
        public virtual bool? AccountLocations
        {
            get
            {
                return this._AccountLocations;
            }
            set
		{
                this._AccountLocations = value;
            }
		}
		#endregion
		#region Multicurrency
		public abstract class multicurrency : PX.Data.IBqlField
		{
		}
		protected bool? _Multicurrency;
        [Feature(typeof(FeaturesSet.financialStandard), typeof(Select<CM.CMSetup>), DisplayName = "Multi-Currency Accounting")]
		public virtual bool? Multicurrency
		{
			get
			{
				return this._Multicurrency;
			}
			set
			{
				this._Multicurrency = value;
			}
		}
		#endregion
        #region SupportBreakQty
        public abstract class supportBreakQty : PX.Data.IBqlField { }
        protected bool? _SupportBreakQty;
        [Feature(false, typeof(FeaturesSet.financialStandard), DisplayName = "Volume Pricing")]
        public virtual bool? SupportBreakQty
        {
            get
            {
                return this._SupportBreakQty;
            }
            set
            {
                this._SupportBreakQty = value;
            }
        }
        #endregion
        #region Prebooking
        public abstract class prebooking : PX.Data.IBqlField
        {
        }
        protected bool? _Prebooking;
        [Feature(typeof(FeaturesSet.financialStandard), typeof(Select<AP.APRegister, Where<AP.APRegister.prebookBatchNbr, IsNotNull>>), DisplayName = "Expense Reclassification")]
        public virtual bool? Prebooking
        {
            get;
            set;
        }
        #endregion
        #region TaxEntryFromGL
        public abstract class taxEntryFromGL : PX.Data.IBqlField
		{
		}
        protected bool? _TaxEntryFromGL;
        [Feature(false, typeof(FeaturesSet.financialStandard), typeof(Select<GL.GLTran, Where<GL.GLTran.taxID, IsNotNull>>), DisplayName = "Tax Entry from GL Module")]
        public virtual bool? TaxEntryFromGL
		{
			get
			{
                return this._TaxEntryFromGL;
			}
			set
			{
                this._TaxEntryFromGL = value;
			}
		}
		#endregion
        #region VATReporting
        public abstract class vATReporting : PX.Data.IBqlField
		{
		}
        protected bool? _VATReporting;
        [Feature(typeof(FeaturesSet.financialStandard), typeof(Select<TX.Tax, Where<TX.Tax.taxType, Equal<TX.CSTaxType.vat>>>), DisplayName = "VAT Reporting")]
        public virtual bool? VATReporting
		{
			get
			{
                return this._VATReporting;
			}
			set
			{
                this._VATReporting = value;
			}
		}
		#endregion
		#region NetGrossEntryMode
		public abstract class netGrossEntryMode : PX.Data.IBqlField
		{
		}
		protected bool? _NetGrossEntryMode;
		[Feature(typeof(FeaturesSet.vATReporting), DisplayName = "Net/Gross Entry Mode")]
		public virtual bool? NetGrossEntryMode
		{
			get
			{
				return this._NetGrossEntryMode;
			}
			set
			{
				this._NetGrossEntryMode = value;
			}
		}
		#endregion
		#region ManualVATEntryMode
		public abstract class manualVATEntryMode : PX.Data.IBqlField
		{
		}
		protected bool? _ManualVATEntryMode;
		[Feature(typeof(FeaturesSet.netGrossEntryMode), typeof(Select<TX.TaxZone,Where<TX.TaxZone.isManualVATZone, Equal<True>>>), DisplayName = "Manual VAT Entry Mode", Visible = false)]
		public virtual bool? ManualVATEntryMode
		{
			get
			{
				return this._ManualVATEntryMode;
			}
			set
			{
				this._ManualVATEntryMode = value;
			}
		}
		#endregion
        #region InvoiceRounding
        public abstract class invoiceRounding : PX.Data.IBqlField
        {
        }
        protected bool? _InvoiceRounding;
        [Feature(typeof(FeaturesSet.financialStandard), DisplayName = "Invoice Rounding")]
        public virtual bool? InvoiceRounding
        {
            get;
            set;
        }
        #endregion                
        #region RutRotDeduction
        public abstract class rutRotDeduction : PX.Data.IBqlField { }
        protected bool? _RutRotDeduction;
        [Feature(false, typeof(FeaturesSet.financialStandard), typeof(Select<GL.Branch,Where<RUTROT.BranchRUTROT.allowsRUTROT,Equal<boolTrue>>>), DisplayName = "ROT & RUT Deduction")]
        public virtual bool? RutRotDeduction
        {
            get
            {
                return this._RutRotDeduction;
            }
            set
            {
                this._RutRotDeduction = value;
            }
        }
        #endregion		
		#region GLWorkBooks
		public abstract class gLWorkBooks : PX.Data.IBqlField { }
		protected bool? _GLWorkBooks;
		[Feature(false, typeof(FeaturesSet.financialStandard), typeof(Select<GL.GLWorkBook>), DisplayName = "GL Workbooks", Visible =false)]
		public virtual bool? GLWorkBooks
		{
			get
			{
				return this._GLWorkBooks;
			}
			set
			{
				this._GLWorkBooks = value;
			}
		}
		#endregion		
        

        #region FinancialAdvanced
        public abstract class financialAdvanced : PX.Data.IBqlField
		{
		}
        protected bool? _FinancialAdvanced;
        [Feature(typeof(FeaturesSet.financialModule), null, DisplayName = "Advanced Financials")]
        public virtual bool? FinancialAdvanced
		{
			get
			{
                return this._FinancialAdvanced;
			}
			set
			{
                this._FinancialAdvanced = value;
			}
		}
		#endregion
        #region SubAccount
        public abstract class subAccount : PX.Data.IBqlField
		{
		}
        protected bool? _SubAccount;
        [Feature(typeof(FeaturesSet.financialAdvanced), typeof(Select<GL.Sub, Where<GL.Sub.subCD, NotEqual<IN.INSubItem.Zero>>>), DisplayName = "Subaccounts")]
        public virtual bool? SubAccount
		{
			get
			{
                return this._SubAccount;
			}
			set
			{
                this._SubAccount = value;
			}
		}
		#endregion
        #region AllocationTemplates
        public abstract class allocationTemplates : PX.Data.IBqlField
        {
        }
        protected bool? _AllocationTemplates;
        [Feature(false, typeof(FeaturesSet.financialAdvanced), DisplayName = "General Ledger Allocation Templates")]
        public virtual bool? AllocationTemplates
        {
            get
		{
                return this._AllocationTemplates;
		}
            set
		{
                this._AllocationTemplates = value;
            }
		}
		#endregion
        #region Inter-Branch Accounting
        public abstract class interBranch : PX.Data.IBqlField
		{

		}
        //[FeatureRestrictor(typeof(Select2<GL.Ledger, InnerJoin<GL.Branch, On<GL.Branch.branchID, Equal<GL.Ledger.defBranchID>>>>))]
        //[FeatureRestrictor(typeof(Select2<GL.Ledger, InnerJoin<GL.GLHistory, On<GL.GLHistory.ledgerID, Equal<GL.Ledger.ledgerID>>>, 
        //	Where<GL.Ledger.balanceType,Equal<GL.LedgerBalanceType.actual>, And<GL.Ledger.postInterCompany, Equal<True>>>>))]
        [FeatureRestrictor(typeof(Select2<GL.BranchAcctMap, InnerJoin<GL.Branch, On<GL.Branch.branchID, Equal<GL.BranchAcctMap.branchID>, And<GL.Branch.active, Equal<True>>>>>))]
        [FeatureDependency(typeof(FeaturesSet.branch))]
        [Feature(typeof(FeaturesSet.financialAdvanced), null, DisplayName = "Inter-Branch Transactions")]        
        public virtual bool? InterBranch { get; set; }
        #endregion
        #region GLConsolidation
        public abstract class gLConsolidation : PX.Data.IBqlField
		{
		}
        protected bool? _GLConsolidation;
        [Feature(false, typeof(FeaturesSet.financialAdvanced), DisplayName = "General Ledger Consolidation")]
        public virtual bool? GLConsolidation
        {
            get
        {
                return this._GLConsolidation;
        }
            set
        {
                this._GLConsolidation = value;
            }
        }
        #endregion
        #region FinStatementCurTranslation
        public abstract class finStatementCurTranslation : PX.Data.IBqlField
        {
        }
        protected bool? _FinStatementCurTranslation;
        [Feature(false, typeof(FeaturesSet.financialAdvanced), DisplayName = "Translation of Financial Statements")]
        [FeatureDependency(typeof(FeaturesSet.multicurrency))]
        public virtual bool? FinStatementCurTranslation
        {
            get
		{
                return this._FinStatementCurTranslation;
		}
            set
            {
                this._FinStatementCurTranslation = value;
            }
        }
        #endregion
        #region VendorDiscounts
        public abstract class vendorDiscounts : PX.Data.IBqlField { }
        protected bool? _VendorDiscounts;
        [Feature(false, typeof(FeaturesSet.financialAdvanced), DisplayName = "Customer & Vendor Discounts")]
        public virtual bool? VendorDiscounts
		{
			get
			{
                return this._VendorDiscounts;
			}
			set
			{
                this._VendorDiscounts = value;
			}
		}
		#endregion
        #region Commissions
        public abstract class commissions : PX.Data.IBqlField
		{
		}
        protected bool? _Commissions;
        [Feature(false, typeof(FeaturesSet.financialAdvanced), DisplayName = "Commissions")]
        public virtual bool? Commissions
		{
			get
			{
                return this._Commissions;
			}
			set
			{
                this._Commissions = value;
			}
		}
		#endregion
        #region OverdueFinCharges
        public abstract class overdueFinCharges : PX.Data.IBqlField
		{
		}
        protected bool? _OverdueFinCharges;
        [Feature(false, typeof(FeaturesSet.financialAdvanced), DisplayName = "Overdue Charges")]
        public virtual bool? OverdueFinCharges
		{
			get
			{
                return this._OverdueFinCharges;
			}
			set
			{
                this._OverdueFinCharges = value;
			}
		}
		#endregion
        #region DunningLetter
        public abstract class dunningLetter : PX.Data.IBqlField
		{
		}
        protected bool? _DunningLetter;
        [Feature(typeof(FeaturesSet.financialAdvanced), typeof(Select<AR.ARDunningSetup>), DisplayName = "Dunning Letter Management")]
        public virtual bool? DunningLetter
		{
			get
			{
                return this._DunningLetter;
			}
			set
			{
                this._DunningLetter = value;
			}
		}
		#endregion
        #region DefferedRevenue
        public abstract class defferedRevenue : PX.Data.IBqlField
        {
        }
        protected bool? _DefferedRevenue;
        [Feature(typeof(FeaturesSet.financialAdvanced), typeof(Select<DR.DRSchedule>), DisplayName = "Deferred Revenue Management")]
        public virtual bool? DefferedRevenue
        {
            get
            {
                return this._DefferedRevenue;
            }
            set
		{
                this._DefferedRevenue = value;
            }
		}
		#endregion
		#region ConsolidatedPosting
		public abstract class consolidatedPosting : PX.Data.IBqlField { }
		protected bool? _ConsolidatedPosting;

		[Obsolete("ConsolidatedPosting setting was moved to GLSetup. FeatureSet.ConsolidatedPosting will be fully eliminated in the future version.")]
		[Feature(false, typeof(FeaturesSet.financialAdvanced), DisplayName = "Consolidated Posting to GL", Visible = false)]
		public virtual bool? ConsolidatedPosting
		{
			get
			{
				return this._ConsolidatedPosting;
			}
			set
			{
				this._ConsolidatedPosting = value;
			}
		}
		#endregion
		#region ParentChildAccount
		public abstract class parentChildAccount : IBqlField { }
		protected bool? _ParentChildAccount;

		[Feature(false, typeof(FeaturesSet.financialAdvanced), typeof(Select<AR.Customer, 
			Where<AR.Customer.consolidatingBAccountID, NotEqual<AR.Customer.bAccountID>, 
				Or<AR.Customer.statementCustomerID, NotEqual<AR.Customer.bAccountID>,
				Or<AR.Customer.sharedCreditCustomerID, NotEqual<AR.Customer.bAccountID>>>>>), DisplayName = "Parent-Child Customer Relationship")]
		public virtual bool? ParentChildAccount
		{
			get { return _ParentChildAccount; }
			set { _ParentChildAccount = value; }
		}
		#endregion
        #region Contract
        public abstract class contractManagement : PX.Data.IBqlField
        {
        }
        protected bool? _ContractManagement;
        [Feature(typeof(FeaturesSet.financialModule), typeof(Select<CT.Contract, Where<CT.Contract.nonProject, Equal<False>, And<CT.Contract.baseType, Equal<CT.Contract.ContractBaseType>>>>), DisplayName = "Contract Management")]
        public virtual bool? ContractManagement
        {
            get;
            set;
        }
        #endregion
		#region FixedAsset
		public abstract class fixedAsset : PX.Data.IBqlField
		{
		}
		protected bool? _FixedAsset;
		[Feature(typeof(FeaturesSet.financialModule), typeof(Select<FA.FASetup>), DisplayName = "Fixed Asset Management")]
		public virtual bool? FixedAsset
		{
			get
			{
				return this._FixedAsset;
			}
			set
			{
				this._FixedAsset = value;
			}
		}
		#endregion
        
        #region Payroll
        public abstract class payroll : PX.Data.IBqlField
		{
		}
        protected bool? _Payroll;
        [Feature(false, typeof(FeaturesSet.financialModule), DisplayName = "Payroll")]
        [FeatureDependency(typeof(FeaturesSet.financialAdvanced))]
        public virtual bool? Payroll
		{
			get
			{
                return this._Payroll;
			}
			set
			{
                this._Payroll = value;
			}
		}
		#endregion
        #region MultipleWorkShifts
        public abstract class multipleWorkShifts : PX.Data.IBqlField
		{
		}
        protected bool? _MultipleWorkShifts;
        [Feature(false, typeof(FeaturesSet.payroll), DisplayName = "Multiple Work Shifts")]
        public virtual bool? MultipleWorkShifts
		{
			get
			{
                return this._MultipleWorkShifts;
			}
			set
			{
                this._MultipleWorkShifts = value;
			}
		}
		#endregion
        
        
        

		#region MiscModule
		public abstract class miscModule : PX.Data.IBqlField
		{
		}
		protected bool? _MiscModule;
        [Feature(true, DisplayName = "Monitoring & Automation", Enabled = false)]
		public virtual bool? MiscModule
		{
			get
			{
				return this._MiscModule;
			}
			set
			{
				this._MiscModule = value;
			}
		}
		#endregion
        #region TimeReportingModule
        public abstract class timeReportingModule : PX.Data.IBqlField
		{
		}
        protected bool? _TimeReportingModule;
        [FeatureRestrictor(typeof(Select<CR.PMTimeActivity, Where<CR.PMTimeActivity.trackTime, Equal<boolTrue>>>))]
        [Feature(false, typeof(FeaturesSet.miscModule), DisplayName = "Time Reporting on Activity")]
        public virtual bool? TimeReportingModule
		{
			get
			{
                return this._TimeReportingModule;
			}
			set
			{
                this._TimeReportingModule = value;
			}
		}
		#endregion
		#region ApprovalWorkflow
        public abstract class approvalWorkflow : PX.Data.IBqlField
		{
		}
        protected bool? _ApprovalWorkflow;
        [Feature(typeof(FeaturesSet.miscModule), DisplayName = "Approval Workflow")]
        public virtual bool? ApprovalWorkflow
		{
			get
			{
                return this._ApprovalWorkflow;
			}
			set
			{
                this._ApprovalWorkflow = value;
			}
		}
		#endregion
        #region FieldLevelLogging
        public abstract class fieldLevelLogging : PX.Data.IBqlField
		{
		}
        protected bool? _FieldLevelLogging;
        [Feature(typeof(FeaturesSet.miscModule), DisplayName = "Field-Level Audit")]
        public virtual bool? FieldLevelLogging
		{
			get
			{
                return this._FieldLevelLogging;
			}
			set
			{
                this._FieldLevelLogging = value;
			}
		}
		#endregion
		#region RowLevelSecurity
		public abstract class rowLevelSecurity : PX.Data.IBqlField
		{
		}
		protected bool? _RowLevelSecurity;
		[Feature(typeof(FeaturesSet.miscModule), typeof(Select<PX.SM.RelationGroup>), DisplayName = "Row-Level Security")]
		public virtual bool? RowLevelSecurity
		{
			get
			{
				return this._RowLevelSecurity;
			}
			set
			{
				this._RowLevelSecurity = value;
			}
		}
		#endregion
        #region ScheduleModule
        public abstract class scheduleModule : PX.Data.IBqlField
		{
		}
        protected bool? _ScheduleModule;
        [Feature(true, typeof(FeaturesSet.miscModule), typeof(Select<PX.SM.AUSchedule, Where<PX.SM.AUSchedule.isActive, Equal<True>>>), DisplayName = "Scheduled Processing")]
        public virtual bool? ScheduleModule
		{
			get
			{
                return this._ScheduleModule;
			}
			set
			{
                this._ScheduleModule = value;
			}
		}
		#endregion
        #region NotificationModule
        public abstract class notificationModule : PX.Data.IBqlField
		{
		}
        protected bool? _NotificationModule;
        [Feature(false, typeof(FeaturesSet.miscModule), typeof(Select<PX.SM.AUNotification, Where<PX.SM.AUNotification.isActive, Equal<True>>>), DisplayName = "Change Notifications")]
        public virtual bool? NotificationModule
		{
			get
			{
                return this._NotificationModule;
			}
			set
			{
                this._NotificationModule = value;
			}
		}
		#endregion
        #region AutomationModule
        public abstract class automationModule : PX.Data.IBqlField
		{
		}
        protected bool? _AutomationModule;
        [Feature(true, typeof(FeaturesSet.miscModule), DisplayName = "Workflow Automation")]
        public virtual bool? AutomationModule
		{
			get
			{
                return this._AutomationModule;
			}
			set
			{
                this._AutomationModule = value;
			}
		}
		#endregion

     

        #region DistributionModule
        public abstract class distributionModule : PX.Data.IBqlField
		{
		}
        protected bool? _DistributionModule;
        [Feature(typeof(FeaturesSet.financialModule), typeof(Select<IN.INSetup>), Top = true, DisplayName = "Distribution")]
        public virtual bool? DistributionModule
		{
			get
			{
                return this._DistributionModule;
			}
			set
			{
                this._DistributionModule = value;
			}
		}
		#endregion
        #region DistributionStandard
        public abstract class distributionStandard : PX.Data.IBqlField
		{
		}
        protected bool? _DistributionStandard;
        [Feature(typeof(FeaturesSet.distributionModule), null, DisplayName = "Standard Distribution", SyncToParent = true)]
        public virtual bool? DistributionStandard
		{
			get
			{
                return this._DistributionStandard;
			}
			set
			{
                this._DistributionStandard = value;
			}
		}
		#endregion
		#region MultipleUnitMeasure
		public abstract class multipleUnitMeasure : PX.Data.IBqlField
		{
		}
		protected bool? _MultipleUnitMeasure;
		[FeatureRestrictor(typeof(Select<IN.InventoryItem, Where<IN.InventoryItem.baseUnit, NotEqual<IN.InventoryItem.salesUnit>, Or<IN.InventoryItem.baseUnit, NotEqual<IN.InventoryItem.purchaseUnit>>>>))]
		[FeatureRestrictor(typeof(Select<IN.INItemClass, Where<IN.INItemClass.baseUnit, NotEqual<IN.INItemClass.salesUnit>, Or<IN.INItemClass.baseUnit, NotEqual<IN.INItemClass.purchaseUnit>>>>))]
		[FeatureRestrictor(typeof(
			Select2<
				IN.InventoryItem
				, InnerJoin<IN.INItemSite, On<IN.InventoryItem.inventoryID, Equal<IN.INItemSite.inventoryID>>>
				, Where<IN.InventoryItem.baseUnit, NotEqual<IN.INItemSite.dfltSalesUnit>, Or<IN.InventoryItem.baseUnit, NotEqual<IN.INItemSite.dfltPurchaseUnit>>>
				>
			))]

        [Feature(false, typeof(FeaturesSet.distributionStandard), DisplayName = "Multiple Units of Measure")]
		public virtual bool? MultipleUnitMeasure
		{
			get
			{
				return this._MultipleUnitMeasure;
			}
			set
			{
				this._MultipleUnitMeasure = value;
			}
		}
		#endregion
		#region LotSerialTracking
		public abstract class lotSerialTracking : PX.Data.IBqlField
		{
		}
		protected bool? _LotSerialTracking;
		[Feature(false, typeof(FeaturesSet.distributionAdvanced), DisplayName = "Lot and Serial Tracking")]
		public virtual bool? LotSerialTracking
		{
			get
			{
				return this._LotSerialTracking;
			}
			set
			{
				this._LotSerialTracking = value;
			}
		}
		#endregion
        #region BlanketPO
        public abstract class blanketPO : PX.Data.IBqlField
		{
		}
        protected bool? _BlanketPO;
        [Feature(false, typeof(FeaturesSet.distributionStandard), typeof(Select<PO.POOrder, Where<PO.POOrder.orderType, Equal<PO.POOrderType.blanket>, Or<PO.POOrder.orderType, Equal<PO.POOrderType.standardBlanket>>>>), DisplayName = "Blanket and Standard Purchase Orders")]
        public virtual bool? BlanketPO
		{
			get
			{
                return this._BlanketPO;
			}
			set
			{
                this._BlanketPO = value;
			}
		}
		#endregion		
        #region DropShipments
        public abstract class dropShipments : PX.Data.IBqlField
		{
		}
        protected bool? _DropShipments;
        [Feature(false, typeof(FeaturesSet.distributionStandard), typeof(Select<PO.POOrder, Where<PO.POOrder.orderType, Equal<PO.POOrderType.dropShip>>>), DisplayName = "Drop Shipments")]
        public virtual bool? DropShipments
		{
			get
			{
                return this._DropShipments;
			}
			set
			{
                this._DropShipments = value;
			}
		}
		#endregion

        #region DistributionAdvanced
        public abstract class distributionAdvanced : PX.Data.IBqlField
        {
        }
        protected bool? _DistributionAdvanced;
        [Feature(typeof(FeaturesSet.distributionModule), null, DisplayName = "Advanced Distribution")]
        public virtual bool? DistributionAdvanced
        {
            get
            {
                return this._DistributionAdvanced;
            }
            set
            {
                this._DistributionAdvanced = value;
            }
        }
        #endregion
        #region Warehouse
        public abstract class warehouse : PX.Data.IBqlField
		{
		}
        protected bool? _Warehouse;
		[FeatureRestrictor(typeof(Select<INItemSite, Where<INItemSite.replenishmentSourceSiteID, IsNotNull>>))]
		[FeatureRestrictor(typeof(Select<INItemRep, Where<INItemRep.replenishmentSourceSiteID, IsNotNull>>))]
		[FeatureRestrictor(typeof(Select<INItemClassRep, Where<INItemClassRep.replenishmentSourceSiteID, IsNotNull>>))]
		[Feature(true, typeof(FeaturesSet.distributionAdvanced), typeof(Select<IN.INSite, Where<IN.INSite.siteCD, NotEqual<IN.INSite.main>>>), DisplayName = "Multiple Warehouses")]
        public virtual bool? Warehouse
		{
			get
			{
                return this._Warehouse;
			}
			set
			{
                this._Warehouse = value;
			}
		}
		#endregion
        #region WarehouseLocation
        public abstract class warehouseLocation : PX.Data.IBqlField
		{
		}
        protected bool? _WarehouseLocation;
        [Feature(true, typeof(FeaturesSet.distributionAdvanced), typeof(Select<IN.INLocation, Where<IN.INLocation.locationCD, NotEqual<IN.INLocation.main>>>), DisplayName = "Multiple Warehouse Locations")]
        public virtual bool? WarehouseLocation
		{
			get
			{
                return this._WarehouseLocation;
			}
			set
			{
                this._WarehouseLocation = value;
			}
		}
		#endregion
        #region Replenishment
        public abstract class replenishment : PX.Data.IBqlField
		{
		}
        protected bool? _Replenishment;
        [Feature(false, typeof(FeaturesSet.distributionAdvanced), DisplayName = "Inventory Replenishment")]
        [FeatureDependency(typeof(FeaturesSet.warehouse), typeof(FeaturesSet.warehouseLocation))]
        public virtual bool? Replenishment
		{
			get
			{
                return this._Replenishment;
			}
			set
			{
                this._Replenishment = value;
			}
		}
		#endregion
        #region SubItem
        public abstract class subItem : PX.Data.IBqlField
		{
		}
        protected bool? _SubItem;
        [Feature(typeof(FeaturesSet.distributionAdvanced), typeof(Select<IN.INSubItem, Where<IN.INSubItem.subItemCD, NotEqual<IN.INSubItem.Zero>>>), DisplayName = "Inventory Subitems")]
        public virtual bool? SubItem
		{
			get
			{
                return this._SubItem;
			}
			set
			{
                this._SubItem = value;
			}
		}
		#endregion		
        #region AutoPackaging
        public abstract class autoPackaging : PX.Data.IBqlField
		{
		}
        protected bool? _AutoPackaging;
        [Feature(typeof(FeaturesSet.distributionAdvanced), DisplayName = "Automatic Packaging")]
        public virtual bool? AutoPackaging
		{
			get
			{
                return this._AutoPackaging;
			}
			set
			{
                this._AutoPackaging = value;
			}
		}
		#endregion
        #region KitAssemblies
        public abstract class kitAssemblies : PX.Data.IBqlField
		{
		}
        protected bool? _KitAssemblies;
        [Feature(false, typeof(FeaturesSet.distributionAdvanced), typeof(Select<IN.InventoryItem, Where<IN.InventoryItem.kitItem, Equal<True>, And<IN.InventoryItem.itemStatus, NotEqual<IN.InventoryItemStatus.inactive>, And<IN.InventoryItem.itemStatus, NotEqual<IN.InventoryItemStatus.markedForDeletion>>>>>), DisplayName = "Kit Assembly")]
        public virtual bool? KitAssemblies
		{
			get
			{
                return this._KitAssemblies;
			}
			set
			{
                this._KitAssemblies = value;
			}
		}
		#endregion
        #region AdvancedPhysicalCounts
        public abstract class advancedPhysicalCounts : PX.Data.IBqlField
        {
        }
        protected bool? _AdvancedPhysicalCounts;
        [Feature(false, typeof(FeaturesSet.distributionAdvanced), DisplayName = "Advanced Physical Count")]
        public virtual bool? AdvancedPhysicalCounts
        {
            get
            {
                return this._AdvancedPhysicalCounts;
            }
            set
            {
                this._AdvancedPhysicalCounts = value;
            }
        }
        #endregion		
		#region SOToPOLink
		public abstract class sOToPOLink : PX.Data.IBqlField
		{
		}
		protected bool? _SOToPOLink;
		[Feature(false, typeof(FeaturesSet.distributionAdvanced), DisplayName = "Sales Order to Purchase Order Link")]
		public virtual bool? SOToPOLink
		{
			get
			{
				return this._SOToPOLink;
			}
			set
			{
				this._SOToPOLink = value;
			}
		}
		#endregion
        #region UserDefinedOrderTypes
        public abstract class userDefinedOrderTypes : PX.Data.IBqlField
		{
		}
        protected bool? _UserDefinedOrderTypes;
		[Feature(false, typeof(FeaturesSet.distributionAdvanced), DisplayName = "Custom Order Types")]
        public virtual bool? UserDefinedOrderTypes
        {
            get
            {
                return this._UserDefinedOrderTypes;
            }
            set
            {
                this._UserDefinedOrderTypes = value;
            }
        }
        #endregion		
        #region PurchaseRequisitions
        public abstract class purchaseRequisitions : PX.Data.IBqlField
        {
        }
        protected bool? _PurchaseRequisitions;
        [Feature(false, typeof(FeaturesSet.distributionAdvanced), DisplayName = "Purchase Requisitions")]
        public virtual bool? PurchaseRequisitions
        {
            get
            {
                return this._PurchaseRequisitions;
            }
            set
            {
                this._PurchaseRequisitions = value;
            }
        }
        #endregion		


        #region OrganizationModule
        public abstract class organizationModule : PX.Data.IBqlField
        {
            }
        protected bool? _OrganizationModule;
        [Feature(true, DisplayName = "Organization", Enabled = false, Visible=false)]
        public virtual bool? OrganizationModule
        {
            get
            {
                return this._OrganizationModule;
            }
            set
            {
                this._OrganizationModule = value;
            }
        }
        #endregion
        #region CustomerModule
        public abstract class customerModule : PX.Data.IBqlField
        {
            public const string FieldClass = "CRM";
            }
        protected bool? _CustomerModule;
        [Feature(false, null, typeof(Select<CR.CRSetup>), DisplayName = "Customer Management")]
        public virtual bool? CustomerModule
		{
			get
			{
                return this._CustomerModule;
			}
			set
			{
                this._CustomerModule = value;
			}
		}
		#endregion		
        #region ContactDuplicate
        public abstract class contactDuplicate : PX.Data.IBqlField
		{
            public const string FieldClass = "DUPLICATE";
		}
        protected bool? _ContactDuplicate;
        [Feature(typeof(FeaturesSet.customerModule), DisplayName = "Lead/Contact Duplicate Validation")]
        public virtual bool? ContactDuplicate
		{
			get
			{
                return this._ContactDuplicate;
			}
			set
			{
                this._ContactDuplicate = value;
			}
		}
		#endregion
        #region ProjectModule
        public abstract class projectModule : PX.Data.IBqlField
		{
		}
        protected bool? _ProjectModule;
        [Feature(false, null, typeof(Select<PM.PMProject, Where<PM.PMProject.baseType, Equal<PM.PMProject.ProjectBaseType>, And<PM.PMProject.nonProject, Equal<False>>>>), DisplayName = "Project Management")]
        public virtual bool? ProjectModule
		{
			get
			{
                return this._ProjectModule;
			}
			set
			{
                this._ProjectModule = value;
			}
		}
		#endregion

        #region PortalModules
        #region PortalModule
        public abstract class portalModule : PX.Data.IBqlField
        {
        }
        protected bool? _PortalModule;
        [Feature(false, DisplayName = "Customer Portal")]
        public virtual bool? PortalModule
        {
            get
            {
                return this._PortalModule;
            }
            set
            {
                this._PortalModule = value;
            }
        }
        #endregion

        #region B2BOrdering
        public abstract class b2BOrdering : PX.Data.IBqlField
        {
        }
        protected bool? _B2BOrdering;
        [Feature(false, typeof(portalModule), DisplayName = "B2B Ordering")]
        [FeatureDependency(typeof(FeaturesSet.distributionAdvanced))]
        public virtual bool? B2BOrdering
        {
            get
            {
                return this._B2BOrdering;
            }
            set
            {
                this._B2BOrdering = value;
            }
        }
        #endregion

        #region PortalCaseManagement
        public abstract class portalCaseManagement : PX.Data.IBqlField
        {
        }
        protected bool? _PortalCaseManagement;
        [Feature(false, typeof(portalModule), DisplayName = "Case Management on Portal")]
        [FeatureDependency(typeof(FeaturesSet.customerModule))]
        public virtual bool? PortalCaseManagement
        {
            get
            {
                return this._PortalCaseManagement;
            }
            set
            {
                this._PortalCaseManagement = value;
            }
        }
        #endregion

        #region Financials
        public abstract class portalFinancials : PX.Data.IBqlField
        {
        }
        protected bool? _PortalFinancials;
        [Feature(false, typeof(portalModule), DisplayName = "Financials on Portal")]
        [FeatureDependency(typeof(FeaturesSet.financialAdvanced))]
        public virtual bool? PortalFinancials
        {
            get
            {
                return this._PortalFinancials;
            }
            set
            {
                this._PortalFinancials = value;
            }
        }
        #endregion
        #endregion
        
        #region IntegrationModule
        public abstract class integrationModule : PX.Data.IBqlField
		{
		}
        protected bool? _IntegrationModule;
        [Feature(true, DisplayName = "Third Party Integrations")]
        public virtual bool? IntegrationModule
		{
			get
			{
                return this._IntegrationModule;
			}
			set
			{
                this._IntegrationModule = value;
			}
		}
		#endregion
        #region CarrierIntegration
        public abstract class carrierIntegration : PX.Data.IBqlField
		{
		}
        protected bool? _CarrierIntegration;
        [Feature(false, typeof(FeaturesSet.integrationModule), DisplayName = "Shipping Carrier Integraton")]
        [FeatureDependency(typeof(FeaturesSet.distributionAdvanced))]
        public virtual bool? CarrierIntegration
		{
			get
			{
                return this._CarrierIntegration;
			}
			set
			{
                this._CarrierIntegration = value;
			}
		}
		#endregion
        #region ExchangeIntegration
        public abstract class exchangeIntegration : PX.Data.IBqlField
		{
		}
        protected bool? _ExchangeIntegration;
        [Feature(typeof(FeaturesSet.integrationModule), DisplayName = "Exchange Integration")]
        public virtual bool? ExchangeIntegration
		{
			get
			{
                return this._ExchangeIntegration;
			}
			set
			{
                this._ExchangeIntegration = value;
			}
		}
		#endregion
        #region AvalaraTax
        public abstract class avalaraTax : PX.Data.IBqlField
		{
		}
        protected bool? _AvalaraTax;
        [Feature(typeof(FeaturesSet.integrationModule), DisplayName = "Avalara Tax Integration")]
        public virtual bool? AvalaraTax
		{
			get
			{
                return this._AvalaraTax;
			}
			set
			{
                this._AvalaraTax = value;
			}
		}
		#endregion
        #region AddressValidation
        public abstract class addressValidation : PX.Data.IBqlField
		{
		}
        protected bool? _AddressValidation;
        [Feature(typeof(FeaturesSet.avalaraTax), DisplayName = "Address Validation")]
        public virtual bool? AddressValidation
		{
			get
			{
                return this._AddressValidation;
			}
			set
			{
                this._AddressValidation = value;
			}
		}
		#endregion
    }

}
