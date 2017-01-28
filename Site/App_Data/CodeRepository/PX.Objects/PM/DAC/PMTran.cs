using PX.Data.EP;

namespace PX.Objects.PM
{
	using System;
	using PX.Data;
	using PX.Objects.CS;
	using PX.Objects.IN;
	using PX.Objects.CM;
    using PX.Objects.GL;
    using PX.Objects.EP;
	using PX.Objects.CR;
	using PX.Objects.CA;

	[PXPrimaryGraph(
		new Type[] { typeof(RegisterEntry) },
		new Type[] { typeof(Select<PMRegister, Where<PMRegister.refNbr, Equal<Current<PMTran.refNbr>>>>)
		})]
	[System.SerializableAttribute()]
    [PXCacheName(Messages.PMTran)]
	public partial class PMTran : PX.Data.IBqlTable
	{
		#region Selected
		public abstract class selected : IBqlField
		{
		}
		protected bool? _Selected = false;
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

		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		protected Int32? _BranchID;
		[Branch()]
		public virtual Int32? BranchID
		{
			get
			{
				return this._BranchID;
			}
			set
			{
				this._BranchID = value;
			}
		}
		#endregion
		#region TranID
		public abstract class tranID : PX.Data.IBqlField
		{
		}
		protected Int64? _TranID;
		[PXDBLongIdentity(IsKey = true)]
		public virtual Int64? TranID
		{
			get
			{
				return this._TranID;
			}
			set
			{
				this._TranID = value;
			}
		}
		#endregion
		#region TranType
		public abstract class tranType : PX.Data.IBqlField
		{
		}
		protected String _TranType;
		[PXDefault(typeof(PMRegister.module))]
		[PXDBString(2, IsFixed = true)]
		public virtual String TranType
		{
			get
			{
				return this._TranType;
			}
			set
			{
				this._TranType = value;
			}
		}
		#endregion
		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
		}
		protected String _RefNbr;
        [PXUIField(DisplayName = "Ref. Number")]
		[PXDBLiteDefault(typeof(PMRegister.refNbr))]
		[PXDBString(PMRegister.refNbr.Length, IsUnicode = true)]
		[PXParent(typeof(Select<PMRegister, Where<PMRegister.module, Equal<Current<PMTran.tranType>>, And<PMRegister.refNbr, Equal<Current<PMTran.refNbr>>>>>))]
		public virtual String RefNbr
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
       	#region Date
		public abstract class date : PX.Data.IBqlField
		{
		}
		protected DateTime? _Date;
		[PXDBDate]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Date", Visibility = PXUIVisibility.SelectorVisible, Required = true)]
		public virtual DateTime? Date
		{
			get
			{
				return this._Date;
			}
			set
			{
				this._Date = value;
			}
		}
		#endregion
		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected String _FinPeriodID;
		[OpenPeriod(typeof(PMTran.date))]
		[PXUIField(DisplayName = "Fin. Period", Visibility = PXUIVisibility.SelectorVisible, Required = true)]
		public virtual String FinPeriodID
		{
			get
			{
				return this._FinPeriodID;
			}
			set
			{
				this._FinPeriodID = value;
			}
		}
		#endregion
		#region TranDate
		public abstract class tranDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _TranDate;
		[PXDBDate()]
		[PXDefault(typeof(AccessInfo.businessDate))]
		public virtual DateTime? TranDate
		{
			get
			{
				return this._TranDate;
			}
			set
			{
				this._TranDate = value;
			}
		}
		#endregion		
		#region TranPeriodID
		public abstract class tranPeriodID : PX.Data.IBqlField
		{
		}
		protected String _TranPeriodID;
		[TranPeriodID(typeof(PMTran.tranDate))]
		[PXDefault(MapErrorTo = typeof(PMTran.date))]
		public virtual String TranPeriodID
		{
			get
			{
				return this._TranPeriodID;
			}
			set
			{
				this._TranPeriodID = value;
			}
		}
		#endregion
		#region ProjectID
		public abstract class projectID : PX.Data.IBqlField
		{
		}
		protected Int32? _ProjectID;
		[PXDefault]
        [ActiveProjectOrContractAttribute(typeof(PMTran.bAccountID))]
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
		[ActiveProjectTask(typeof(PMTran.projectID))]
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
		#region AccountGroupID
		public abstract class accountGroupID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountGroupID;
		[AccountGroup]
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
		#region OffsetAccountGroupID
		public abstract class offsetAccountGroupID : PX.Data.IBqlField
		{
		}
		protected Int32? _OffsetAccountGroupID;
		[PXDBInt]
		public virtual Int32? OffsetAccountGroupID
		{
			get
			{
				return this._OffsetAccountGroupID;
			}
			set
			{
				this._OffsetAccountGroupID = value;
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
		#region BAccountID
		public abstract class bAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _BAccountID;
		[PXDBInt()]
		[PXUIField(DisplayName = "Customer/Vendor")]
		[PXSelector(typeof(Search<BAccountR.bAccountID, Where<BAccountR.type, NotEqual<BAccountType.companyType>,
			And<BAccountR.type, NotEqual<BAccountType.prospectType>>>>), SubstituteKey = typeof(BAccountR.acctCD))]
		public virtual Int32? BAccountID
		{
			get
			{
				return this._BAccountID;
			}
			set
			{
				this._BAccountID = value;
			}
		}
		#endregion
		#region LocationID
		public abstract class locationID : PX.Data.IBqlField
		{
		}
		protected Int32? _LocationID;
		[PXDefault(typeof(Search<BAccount.defLocationID, Where<BAccount.bAccountID, Equal<Current<PMTran.bAccountID>>>>), PersistingCheck=PXPersistingCheck.Nothing)]
		[LocationID(typeof(Where<Location.bAccountID, Equal<Current<PMTran.bAccountID>>>), DisplayName = "Location", DescriptionField = typeof(Location.descr))]
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
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[PXUIField(DisplayName="Inventory ID")]
		[PXDBInt()]
		[PMInventorySelector(typeof(Search<InventoryItem.inventoryID>), SubstituteKey=typeof(InventoryItem.inventoryCD))]//All Items + <None>
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
		#region Description
		public abstract class description : PX.Data.IBqlField
		{
		}
		protected String _Description;
		[PXDBString(255, IsUnicode = true)]
		[PXUIField(DisplayName = "Description")]
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
		#region UOM
		public abstract class uOM : PX.Data.IBqlField
		{
		}
		protected String _UOM;
		[PMUnit(typeof(PMTran.inventoryID))]
		public virtual String UOM
		{
			get
			{
				return this._UOM;
			}
			set
			{
				this._UOM = value;
			}
		}
		#endregion
		#region Qty
		public abstract class qty : PX.Data.IBqlField
		{
		}
		protected Decimal? _Qty;
		[PXDBQuantity]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Qty.")]
		public virtual Decimal? Qty
		{
			get
			{
				return this._Qty;
			}
			set
			{
				this._Qty = value;
			}
		}
		#endregion
		#region Billable
		public abstract class billable : PX.Data.IBqlField
		{
		}
		protected Boolean? _Billable;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Billable")]
		public virtual Boolean? Billable
		{
			get
			{
				return this._Billable;
			}
			set
			{
				this._Billable = value;
			}
		}
		#endregion
		#region UseBillableQty
		public abstract class useBillableQty : PX.Data.IBqlField
		{
		}
		protected Boolean? _UseBillableQty;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Use Billable Qty. in Amount formula")]
		public virtual Boolean? UseBillableQty
		{
			get
			{
				return this._UseBillableQty;
			}
			set
			{
				this._UseBillableQty = value;
			}
		}
		#endregion
		#region BillableQty
		public abstract class billableQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BillableQty;
		[PXDBQuantity]
		[PXDefault(typeof(PMTran.qty))]
		[PXUIField(DisplayName = "Billable Qty.")]
		public virtual Decimal? BillableQty
		{
			get
			{
				return this._BillableQty;
			}
			set
			{
				this._BillableQty = value;
			}
		}
		#endregion
		#region UnitRate
		public abstract class unitRate : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnitRate;
		[PXDBPriceCost]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Unit Rate")]
		public virtual Decimal? UnitRate
		{
			get
			{
				return this._UnitRate;
			}
			set
			{
				this._UnitRate = value;
			}
		}
		#endregion
		#region Amount
		public abstract class amount : PX.Data.IBqlField
		{
		}
		protected Decimal? _Amount;
		[PXDBBaseCury]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount")]
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
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;
		//[PXDefault] can be null for Off-balance account group
		[Account(null, typeof(Search2<Account.accountID, 
            LeftJoin<PMAccountGroup, On<PMAccountGroup.groupID, Equal<Current<PMTran.accountGroupID>>>>,
            Where<PMAccountGroup.type, NotEqual<PMAccountType.offBalance>, And<Account.accountGroupID, Equal<Current<PMTran.accountGroupID>>,
            Or<PMAccountGroup.type, Equal<PMAccountType.offBalance>,
			Or<PMAccountGroup.groupID, IsNull>>>>>), DisplayName="Debit Account")]
		public virtual Int32? AccountID
		{
			get
			{
				return this._AccountID;
			}
			set
			{
				this._AccountID = value;
			}
		}
		#endregion
		#region SubID
		public abstract class subID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubID;
        //[PXDefault] can be null for Off-balance account group
		[SubAccount(typeof(PMTran.accountID), DisplayName="Debit Subaccount")]
		public virtual Int32? SubID
		{
			get
			{
				return this._SubID;
			}
			set
			{
				this._SubID = value;
			}
		}
		#endregion
		#region OffsetAccountID
		public abstract class offsetAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _OffsetAccountID;
		//[PXDefault]
		[Account(DisplayName="Credit Account")]
		public virtual Int32? OffsetAccountID
		{
			get
			{
				return this._OffsetAccountID;
			}
			set
			{
				this._OffsetAccountID = value;
			}
		}
		#endregion
		#region OffsetSubID
		public abstract class offsetSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _OffsetSubID;
		//[PXDefault]
		[SubAccount(typeof(PMTran.offsetAccountID), DisplayName="Credit Subaccount")]
		public virtual Int32? OffsetSubID
		{
			get
			{
				return this._OffsetSubID;
			}
			set
			{
				this._OffsetSubID = value;
			}
		}
		#endregion
		#region Allocated
		public abstract class allocated : PX.Data.IBqlField
		{
		}
		protected Boolean? _Allocated;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Allocated", Enabled=false)]
		public virtual Boolean? Allocated
		{
			get
			{
				return this._Allocated;
			}
			set
			{
				this._Allocated = value;
			}
		}
		#endregion
		#region Released
		public abstract class released : PX.Data.IBqlField
		{
		}
		protected Boolean? _Released;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Released", Enabled=false)]
		public virtual Boolean? Released
		{
			get
			{
				return this._Released;
			}
			set
			{
				this._Released = value;
			}
		}
		#endregion
		#region BatchNbr
		public abstract class batchNbr : PX.Data.IBqlField
		{
		}
		protected String _BatchNbr;
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Batch No.", Enabled=false)]
		[PXSelector(typeof(Search<Batch.batchNbr, Where<Batch.module, Equal<Current<PMTran.tranType>>>>))]
		public virtual String BatchNbr
		{
			get
			{
				return this._BatchNbr;
			}
			set
			{
				this._BatchNbr = value;
			}
		}
		#endregion
		#region OrigModule
		public abstract class origModule : PX.Data.IBqlField
		{
		}
		protected String _OrigModule;
		[PXDBString(2, IsFixed = true)]
		[PXUIField(DisplayName = "OrigModule")]
		public virtual String OrigModule
		{
			get
			{
				return this._OrigModule;
			}
			set
			{
				this._OrigModule = value;
			}
		}
		#endregion
		#region OrigTranType
		public abstract class origTranType : PX.Data.IBqlField
		{
		}
		protected String _OrigTranType;
		[PXDBString(3, IsFixed = true)]
		[PXUIField(DisplayName = "OrigTranType")]
		public virtual String OrigTranType
		{
			get
			{
				return this._OrigTranType;
			}
			set
			{
				this._OrigTranType = value;
			}
		}
		#endregion
		#region OrigRefNbr
		public abstract class origRefNbr : PX.Data.IBqlField
		{
		}
		protected String _OrigRefNbr;
		[PXDBString(PMRegister.refNbr.Length, IsUnicode = true)]
		[PXUIField(DisplayName = "OrigRefNbr")]
		public virtual String OrigRefNbr
		{
			get
			{
				return this._OrigRefNbr;
			}
			set
			{
				this._OrigRefNbr = value;
			}
		}
		#endregion
		#region OrigLineNbr
		public abstract class origLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _OrigLineNbr;
		[PXDBInt()]
		[PXUIField(DisplayName = "OrigLineNbr")]
		public virtual Int32? OrigLineNbr
		{
			get
			{
				return this._OrigLineNbr;
			}
			set
			{
				this._OrigLineNbr = value;
			}
		}
		#endregion
		#region BillingID
		public abstract class billingID : PX.Data.IBqlField
		{
		}
		protected String _BillingID;
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
		[PXDBString(PMAllocation.allocationID.Length, IsUnicode = true)]
		[PXUIField(DisplayName = "AllocationID")]
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
		#region Billed
		public abstract class billed : PX.Data.IBqlField
		{
		}
		protected Boolean? _Billed;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Billed", Enabled = false)]
		public virtual Boolean? Billed
		{
			get
			{
				return this._Billed;
			}
			set
			{
				this._Billed = value;
			}
		}
		#endregion
		#region Reversed 
		public abstract class reversed : PX.Data.IBqlField
		{
		}
		protected Boolean? _Reversed;
		/// <summary>
		/// When Transaction is marked as reverse it means that it should not be billed.
		/// </summary>
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Reversed to Non-Billable", Enabled = false)]
		public virtual Boolean? Reversed
		{
			get
			{
				return this._Reversed;
			}
			set
			{
				this._Reversed = value;
			}
		}
		#endregion
		#region BilledDate
		public abstract class billedDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _BilledDate;
		[PXDBDate(PreserveTime = true)]
		[PXUIField(DisplayName = "Billed Date")]
		public virtual DateTime? BilledDate
		{
			get
			{
				return this._BilledDate;
			}
			set
			{
				this._BilledDate = value;
			}
		}
		#endregion
        #region StartDate
        public abstract class startDate : PX.Data.IBqlField
        {
        }
        protected DateTime? _StartDate;
		[PXDefault(typeof(PMTran.date), PersistingCheck=PXPersistingCheck.Nothing)]
        [PXDBDate()]
        [PXUIField(DisplayName = "Start Date", Visible=false)]
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
		[PXDefault(typeof(PMTran.date), PersistingCheck = PXPersistingCheck.Nothing)]
        [PXDBDate()]
        [PXUIField(DisplayName = "End Date", Visible = false)]
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
        #region OrigRefID
        public abstract class origRefID : PX.Data.IBqlField
        {
        }
		protected Guid? _OrigRefID;
        /// <summary>
        /// Case.NoteID for Contracts and CRActivity.NoteID for Timecards/Timesheets
        /// </summary>
		[PXDBGuid]
		public virtual Guid? OrigRefID
        {
            get
            {
                return this._OrigRefID;
            }
            set
            {
                this._OrigRefID = value;
            }
        }
        #endregion
		#region IsNonGL
		public abstract class isNonGL : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsNonGL;
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? IsNonGL
		{
			get
			{
				return this._IsNonGL;
			}
			set
			{
				this._IsNonGL = value;
			}
		}
		#endregion
		#region IsQtyOnly
		public abstract class isQtyOnly : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsQtyOnly;

		/// <summary>
		/// Returns True if the transaction contains only qty data and no price/amount data.
		/// Ex: CRM records only usage without price information. Price is determined later while the billing process.
		/// </summary>
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? IsQtyOnly
		{
			get
			{
				return this._IsQtyOnly;
			}
			set
			{
				this._IsQtyOnly = value;
			}
		}
		#endregion
		#region Reverse
		public abstract class reverse : PX.Data.IBqlField
		{
		}
		protected String _Reverse;
		[PMReverse.List]
		[PXDefault(PMReverse.Never)]
		[PXDBString(1)]
		[PXUIField(DisplayName = "Reverse")]
		public virtual String Reverse
		{
			get
			{
				return this._Reverse;
			}
			set
			{
				this._Reverse = value;
			}
		}
		#endregion
		#region EarningTypeID
		public abstract class earningType : IBqlField { }
		[PXDBString(2, IsFixed = true, IsUnicode = false, InputMask = ">LL")]
		[PXSelector(typeof(EPEarningType.typeCD), DescriptionField = typeof(EPEarningType.description))]
		[PXUIField(DisplayName = "Earning Type", Enabled = false)]
		public virtual string EarningType { get; set; }
		#endregion
		#region OvertimeMultiplier
		public abstract class overtimeMultiplier : IBqlField
		{
		}
		[PXDBDecimal(2)]
		[PXUIField(DisplayName = "Multiplier", Enabled = false)]
		public virtual Decimal? OvertimeMultiplier { get; set; }
		#endregion
		#region CaseID
		public abstract class caseID : PX.Data.IBqlField
		{
		}
		protected Int32? _CaseID;
		[PXSelector(typeof(Search<CRCase.caseID>), SubstituteKey=typeof(CRCase.caseCD))]
		[PXDBInt]
		[PXUIField(DisplayName = "Case ID", Visible = false, Enabled=false)]
		public virtual Int32? CaseID
		{
			get
			{
				return this._CaseID;
			}
			set
			{
				this._CaseID = value;
			}
		}
		#endregion


        //Reference to ARInvoice: after ContractBilling
        #region ARTranType
        public abstract class aRTranType : PX.Data.IBqlField
        {
        }
        protected String _ARTranType;
        [PXDBString(3, IsFixed = true)]
        public virtual String ARTranType
        {
            get
            {
                return this._ARTranType;
            }
            set
            {
                this._ARTranType = value;
            }
        }
        #endregion
        #region ARRefNbr
        public abstract class aRRefNbr : PX.Data.IBqlField
        {
        }
        protected String _ARRefNbr;
        [PXDBString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "AR Reference Nbr.")]
        [PXSelector(typeof(Search<PX.Objects.AR.ARInvoice.refNbr>))]
        public virtual String ARRefNbr
        {
            get
            {
                return this._ARRefNbr;
            }
            set
            {
                this._ARRefNbr = value;
            }
        }
        #endregion
        #region RefLineNbr
        public abstract class refLineNbr : PX.Data.IBqlField
        {
        }
        protected Int32? _RefLineNbr;
        [PXDBInt()]
        public virtual Int32? RefLineNbr
        {
            get
            {
                return this._RefLineNbr;
            }
            set
            {
                this._RefLineNbr = value;
            }
        }
        #endregion

        //Reference to Original Task for Budget Allocation 
        #region OrigProjectID
        public abstract class origProjectID : PX.Data.IBqlField
        {
        }
        protected Int32? _OrigProjectID;
        [PXDBInt()]
        public virtual Int32? OrigProjectID
        {
            get
            {
                return this._OrigProjectID;
            }
            set
            {
                this._OrigProjectID = value;
            }
        }
        #endregion
        #region OrigTaskID
        public abstract class origTaskID : PX.Data.IBqlField
        {
        }
        protected Int32? _OrigTaskID;
        [PXDBInt()]
        public virtual Int32? OrigTaskID
        {
            get
            {
                return this._OrigTaskID;
            }
            set
            {
                this._OrigTaskID = value;
            }
        }
        #endregion
        #region OrigAccountGroupID
        public abstract class origAccountGroupID : PX.Data.IBqlField
        {
        }
        protected Int32? _OrigAccountGroupID;
        [PXDBInt()]
        public virtual Int32? OrigAccountGroupID
        {
            get
            {
                return this._OrigAccountGroupID;
            }
            set
            {
                this._OrigAccountGroupID = value;
            }
        }
        #endregion

		#region OrigTranID
		public abstract class origTranID : PX.Data.IBqlField
		{
		}
		protected Int64? _OrigTranID;
		[PXDBLong]
		public virtual Int64? OrigTranID
		{
			get
			{
				return this._OrigTranID;
			}
			set
			{
				this._OrigTranID = value;
			}
		}
		#endregion

		#region RemainderOfTranID
		public abstract class remainderOfTranID : PX.Data.IBqlField
		{
		}
		protected Int64? _RemainderOfTranID;

		/// <summary>
		/// When remainder is created it holds the reference to the original transaction.
		/// </summary>
		[PXDBLong]
		public virtual Int64? RemainderOfTranID
		{
			get
			{
				return this._RemainderOfTranID;
			}
			set
			{
				this._RemainderOfTranID = value;
			}
		}
		#endregion

        #region IsFree
        public abstract class isFree : IBqlField
        {
        }
        protected bool? _IsFree = false;
        [PXBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? IsFree
        {
            get
            {
                return _IsFree;
            }
            set
            {
                _IsFree = value;
            }
        }
        #endregion
        #region Proportion
        public abstract class proportion : IBqlField
        {
        }
        protected decimal? _Proportion;
        [PXDecimal]
        public virtual decimal? Proportion
        {
            get
            {
                return _Proportion;
            }
            set
            {
                _Proportion = value;
            }
        }
        #endregion
        #region Skip
        public abstract class skip : IBqlField
        {
        }
        protected bool? _Skip = false;
        [PXBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField]
        public virtual bool? Skip
        {
            get
            {
                return _Skip;
            }
            set
            {
                _Skip = value;
            }
        }
        #endregion
        #region Prefix
        public abstract class prefix : PX.Data.IBqlField
        {
        }
        protected String _Prefix;
        [PXString(255, IsUnicode = true)]
        public virtual String Prefix
        {
            get
            {
                return this._Prefix;
            }
            set
            {
                this._Prefix = value;
            }
        }
        #endregion

		#region System Columns
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
        [PXNote]
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

		[PXBool]
		public bool? IsInverted { get; set; }

		[PXBool]
		public bool? IsCreditPair { get; set; }

		/// <summary>
		/// Gets or sets Rate
		/// </summary>
		public decimal? Rate { get; set; }

		#region CreatedByCurrentAllocation
		public abstract class createdByCurrentAllocation : IBqlField
		{
		}
		protected bool? _CreatedByCurrentAllocation = false;
		[PXBool]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual bool? CreatedByCurrentAllocation
		{
			get
			{
				return _CreatedByCurrentAllocation;
			}
			set
			{
				_CreatedByCurrentAllocation = value;
			}
		}
		#endregion
				
	}
}
