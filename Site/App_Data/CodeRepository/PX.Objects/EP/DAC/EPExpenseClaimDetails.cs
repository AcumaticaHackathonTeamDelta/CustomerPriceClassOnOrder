using System;
using System.Collections.Generic;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.IN;
using PX.Objects.TX;
using PX.SM;
using PX.Objects.CT;
using PX.Objects.PM;
using PX.Data.EP;
using PX.Objects.Common;

namespace PX.Objects.EP
{
	public class EPExpenseClaimDetailsStatus : ILabelProvider
	{
		public const string ApprovedStatus = "A";
		public const string HoldStatus = "H";
		public const string ReleasedStatus = "R";
		public const string OpenStatus = "O";
		public const string RejectedStatus = "C";

		private static readonly IEnumerable<ValueLabelPair> _valueLabelPairs = new ValueLabelList
	{
		{ HoldStatus, Messages.HoldStatus },
		{ ApprovedStatus, Messages.ApprovedStatus },
		{ OpenStatus, Messages.OpenStatus },
		{ ReleasedStatus, Messages.ReleasedStatus },
		{ RejectedStatus, Messages.RejectedStatus  },
	};
		public IEnumerable<ValueLabelPair> ValueLabelPairs => _valueLabelPairs;

		public class ListAttribute : LabelListAttribute
		{
			public ListAttribute() : base(_valueLabelPairs)
			{ }
		}

	}

	[System.SerializableAttribute()]
	[PXPrimaryGraph(typeof(ExpenseClaimDetailEntry))]
    [PXCacheName(Messages.ExpenseReceipt)]
	[PXEMailSource]
	public partial class EPExpenseClaimDetails : PX.Data.IBqlTable, PX.Data.EP.IAssign
	{

		#region ClaimDetailID
		public abstract class claimDetailID : PX.Data.IBqlField
		{
		}
		protected Int32? _ClaimDetailID;
		[PXDBIdentity(IsKey = true)]
		[PXUIField(DisplayName = "Receipt ID", Visibility = PXUIVisibility.Invisible)]
		[EPExpenceReceiptSelector]
		public virtual Int32? ClaimDetailID
		{
			get
			{
				return this._ClaimDetailID;
			}
			set
			{
				this._ClaimDetailID = value;
			}
		}
		#endregion

		#region ClaimDetailCD
		public abstract class claimDetailCD : PX.Data.IBqlField
		{
		}
		[PXString(10)]
		[PXUIField(DisplayName = "Receipt ID", Visibility = PXUIVisibility.Visible)]
		public virtual String ClaimDetailCD
		{
			get
			{
				return _ClaimDetailID == null || _ClaimDetailID < -1 ? null : (string)_ClaimDetailID.ToString();
			}
		}
		#endregion

		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		protected Int32? _BranchID;
		[GL.Branch(typeof(EPExpenseClaim.branchID))]
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

        #region RefNbr
        public abstract class refNbr : PX.Data.IBqlField
		{
		}
		protected String _RefNbr;
		[PXDBString(15, IsUnicode = true)]
		[PXDBDefault(typeof(EPExpenseClaim.refNbr), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXParent(typeof(Select<EPExpenseClaim,Where<EPExpenseClaim.refNbr,Equal<Current<EPExpenseClaimDetails.refNbr>>>>))]
		[PXUIField(DisplayName = "Expense Claim Ref. Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<EPExpenseClaim.refNbr>), DescriptionField = typeof(EPExpenseClaim.docDesc), ValidateValue = false, DirtyRead = true)]
        [PXFieldDescription]
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

        #region RefNbrNotFiltered
        public abstract class refNbrNotFiltered : PX.Data.IBqlField
        {
        }
        [PXString(15, IsUnicode = true)]
        [PXFormula(typeof(Current<EPExpenseClaimDetails.refNbr>))]
        [PXSelector(typeof(Search<EPExpenseClaim.refNbr>))]
        public virtual String RefNbrNotFiltered
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

        #region EmployeeID
        public abstract class employeeID : PX.Data.IBqlField
		{
		}
		protected Int32? _EmployeeID;
		[PXDBInt()]
		[PXDefault(typeof(EPExpenseClaim.employeeID))]
		[PXSubordinateAndWingmenSelector]
		[PXUIField(DisplayName = "Claimed by", Visibility = PXUIVisibility.SelectorVisible)]
        [PXFieldDescription]		
		public virtual Int32? EmployeeID
		{
			get
			{
				return this._EmployeeID;
			}
			set
			{
				this._EmployeeID = value;
			}
		}
		#endregion

		#region OwnerID
		public abstract class ownerID : IBqlField
		{
		}
		[PXDBGuid]
		[PX.TM.PXOwnerSelector]
		[PXUIField(DisplayName = "Owner")]
		public virtual Guid? OwnerID
		{
			get;
			set;
		}
		#endregion

		#region WorkgroupID
		public abstract class workgroupID : IBqlField
		{
		}
		[PXDBInt]
		[PX.TM.PXCompanyTreeSelector]
		[PXUIField(DisplayName = "Workgroup")]
		public virtual int? WorkgroupID
		{
			get;
			set;
		}
		#endregion

		#region Hold
		public abstract class hold : IBqlField
		{
		}
		[PXDBBool]
        [PXDefault(true)]
        [PXUIField(Visible =false)]
		public virtual bool? Hold
		{
			get;
			set;
		}
		#endregion

		#region Approved
		public abstract class approved : PX.Data.IBqlField
		{
		}
		protected Boolean? _Approved;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName ="Approved", Enabled = false, Visible = false)]
		public virtual Boolean? Approved
		{
			get
			{
				return this._Approved;
			}
			set
			{
				this._Approved = value;
			}
		}
		#endregion

		#region Rejected
		public abstract class rejected : IBqlField
		{
		}
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(Visible = false)]
		public bool? Rejected
		{
			get;
			set;
		}
		#endregion

		#region CuryInfoID
		public abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _CuryInfoID;
		[PXDBLong()]
		[CurrencyInfo(typeof(EPExpenseClaim.curyInfoID), CuryIDField = "CuryID", CuryDisplayName = "Currency")]
		public virtual Int64? CuryInfoID
		{
			get
			{
				return this._CuryInfoID;
			}
			set
			{
				this._CuryInfoID = value;
			}
		}
		#endregion
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXString(5, IsUnicode = true, InputMask = ">LLLLL")]
		[PXSelector(typeof(Search<CurrencyList.curyID, Where<CurrencyList.isActive, Equal<True>>>))]
		[PXUIField(DisplayName = "Currency")]
		public virtual String CuryID
		{
			get
			{
				return this._CuryID;
			}
			set
			{
				this._CuryID = value;
			}
		}
		#endregion

		#region ClaimCuryInfoID
		public abstract class claimCuryInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _ClaimCuryInfoID;
		[PXDBLong()]
		[CurrencyInfo(typeof(EPExpenseClaim.curyInfoID), CuryIDField = "ClaimCuryID", CuryDisplayName = "Claim Currency")]
		public virtual Int64? ClaimCuryInfoID
		{
			get
			{
				return this._ClaimCuryInfoID;
			}
			set 
			{
				_ClaimCuryInfoID = RefNbr != null ? value : null;
			}
		}
		#endregion
		#region ExpenseDate
		public abstract class expenseDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ExpenseDate;
		[PXDBDate()]
		[PXDefault(typeof(Search<EPExpenseClaim.docDate, Where<EPExpenseClaim.refNbr, Equal<Current<EPExpenseClaimDetails.refNbr>>>>))]
		[PXUIField(DisplayName = "Date")]
		public virtual DateTime? ExpenseDate
		{
			get
			{
				return this._ExpenseDate;
			}
			set
			{
				this._ExpenseDate = value;
			}
		}
		#endregion
		#region ExpenseRefNbr
		public abstract class expenseRefNbr : PX.Data.IBqlField
		{
		}
		protected String _ExpenseRefNbr;
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Ref. Nbr.")]
		public virtual String ExpenseRefNbr
		{
			get
			{
				return this._ExpenseRefNbr;
			}
			set
			{
				this._ExpenseRefNbr = value;
			}
		}
		#endregion
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[PXDefault()]
		[Inventory(DisplayName = "Expense Item")]
		[PXRestrictor(typeof(Where<InventoryItem.itemType, Equal<INItemTypes.expenseItem>>), Messages.InventoryItemIsType)]
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
		#region TaxCategoryID
		public abstract class taxCategoryID : PX.Data.IBqlField
		{
		}
		protected String _TaxCategoryID;
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Tax Category", Visibility = PXUIVisibility.Visible)]
		[EPTax(typeof(EPExpenseClaim), typeof(EPTax), typeof(EPTaxTran))]
        [PXSelector(typeof(TaxCategory.taxCategoryID), DescriptionField = typeof(TaxCategory.descr))]
        [PXRestrictor(typeof(Where<TaxCategory.active, Equal<True>>), TX.Messages.InactiveTaxCategory, typeof(TaxCategory.taxCategoryID))]
		[PXFormula(typeof(Selector<inventoryID, InventoryItem.taxCategoryID>))]
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
		#region UOM
		public abstract class uOM : PX.Data.IBqlField
		{
		}
		protected String _UOM;
		[PXDefault]
		[INUnit(typeof(EPExpenseClaimDetails.inventoryID), DisplayName = "UOM")]
		[PXUIEnabled(typeof(Where<inventoryID, IsNotNull, And<FeatureInstalled<FeaturesSet.multipleUnitMeasure>>>))]
		[PXFormula(typeof(Switch<Case<Where<inventoryID, IsNull>, Null>, Selector<inventoryID, InventoryItem.purchaseUnit>>))]
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
		[PXDefault(TypeCode.Decimal, "1.0")]
		[PXUIField(DisplayName = "Quantity", Visibility = PXUIVisibility.Visible)]
		[PXUIVerify(typeof(Where<qty, NotEqual<decimal0>, Or<Selector<contractID, Contract.nonProject>, Equal<True>>>), PXErrorLevel.Error, Messages.ValueShouldBeNonZero)]
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
		#region CuryUnitCost
		public abstract class curyUnitCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryUnitCost;
		[PXDBCurrency(typeof(Search<CommonSetup.decPlPrcCst>), typeof(EPExpenseClaimDetails.curyInfoID), typeof(EPExpenseClaimDetails.unitCost))]
		[PXUIField(DisplayName = "Unit Cost", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryUnitCost
		{
			get
			{
				return this._CuryUnitCost;
			}
			set
			{
				this._CuryUnitCost = value;
			}
		}
		#endregion
		#region UnitCost
		public abstract class unitCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnitCost;
		[PXDBPriceCost()]
		[PXDefault(typeof(Search<INItemCost.lastCost, Where<INItemCost.inventoryID, Equal<Current<EPExpenseClaimDetails.inventoryID>>>>))]
		public virtual Decimal? UnitCost
		{
			get
			{
				return this._UnitCost;
			}
			set
			{
				this._UnitCost = value;
			}
		}
		#endregion
		#region CuryEmployeePart
		public abstract class curyEmployeePart : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryEmployeePart;
		[PXDBCurrency(typeof(EPExpenseClaimDetails.curyInfoID), typeof(EPExpenseClaimDetails.employeePart), MinValue=0)]
		[PXUIField(DisplayName = "Employee Part")]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIEnabled(typeof(Where<curyExtCost, GreaterEqual<decimal0>>))]
		[PXFormula(typeof(Switch<Case<Where<curyExtCost, Less<decimal0>>, decimal0>, curyEmployeePart>))]
		[PXUIVerify(
			typeof(Where<curyEmployeePart, Equal<decimal0>, 
                        Or<curyEmployeePart, Less<decimal0>, 
                        And<curyEmployeePart, GreaterEqual<curyExtCost>, 
                        Or<curyEmployeePart, Greater<decimal0>, 
                        And<curyEmployeePart, LessEqual<curyExtCost>>>>>>), PXErrorLevel.Error, Messages.EmployeePartExceed	)]
		[PXUIVerify(
            typeof(Where<curyEmployeePart, Equal<decimal0>,
                        Or<curyEmployeePart, Greater<decimal0>,
                        And<curyExtCost, Greater<decimal0>,
                        Or<curyEmployeePart, Less<decimal0>,
                        And<curyExtCost, Less<decimal0>>>>>>), PXErrorLevel.Error, Messages.EmployeePartSign)]
        public virtual Decimal? CuryEmployeePart
		{
			get
			{
				return this._CuryEmployeePart;
			}
			set
			{
				this._CuryEmployeePart = value;
			}
		}
		#endregion
		#region EmployeePart
		public abstract class employeePart : PX.Data.IBqlField
		{
		}
		protected Decimal? _EmployeePart;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Original Employee Part")]
        public virtual Decimal? EmployeePart
		{
			get
			{
				return this._EmployeePart;
			}
			set
			{
				this._EmployeePart = value;
			}
		}
		#endregion
		#region CuryExtCost
		public abstract class curyExtCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryExtCost;
		[PXDBCurrency(typeof(EPExpenseClaimDetails.curyInfoID), typeof(EPExpenseClaimDetails.extCost))]
		[PXUIField(DisplayName = "Total Amount")]
		[PXFormula(typeof(Mult<EPExpenseClaimDetails.qty, EPExpenseClaimDetails.curyUnitCost>))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryExtCost
		{
			get
			{
				return this._CuryExtCost;
			}
			set
			{
				this._CuryExtCost = value;
			}
		}
		#endregion
		#region ExtCost
		public abstract class extCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _ExtCost;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Original Total Amount")]
        public virtual Decimal? ExtCost
		{
			get
			{
				return this._ExtCost;
			}
			set
			{
				this._ExtCost = value;
			}
		}
		#endregion		
		#region CuryTranAmt
		public abstract class curyTranAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryTranAmt;
		[PXDBCurrency(typeof(EPExpenseClaimDetails.curyInfoID), typeof(EPExpenseClaimDetails.tranAmt))]
		[PXFormula(typeof(Sub<EPExpenseClaimDetails.curyExtCost, EPExpenseClaimDetails.curyEmployeePart>))]
		[PXUIField(DisplayName = "Claim Amount", Enabled=false)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryTranAmt
		{
			get
			{
				return this._CuryTranAmt;
			}
			set
			{
				this._CuryTranAmt = value;
			}
		}
		#endregion
		#region TranAmt
		public abstract class tranAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranAmt;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Original Claim Amount")]
        public virtual Decimal? TranAmt
		{
			get
			{
				return this._TranAmt;
			}
			set
			{
				this._TranAmt = value;
			}
		}
		#endregion
		#region ClaimCuryTranAmt
		public abstract class claimCuryTranAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _ClaimCuryTranAmt;
		[PXDBCurrency(typeof(EPExpenseClaimDetails.claimCuryInfoID), typeof(EPExpenseClaimDetails.claimTranAmt))]
		[PXUIField(DisplayName = "Amount in Claim Curr.")]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXFormula(typeof(Default<curyTranAmt, curyTranAmt>))]
		[PXUIEnabled(typeof(Where<Current2<claimCuryInfoID>, NotEqual<curyInfoID>>))]
		public virtual Decimal? ClaimCuryTranAmt
		{
			get
			{
				return this._ClaimCuryTranAmt;
			}
			set
			{
				this._ClaimCuryTranAmt = value;
			}
		}
		#endregion
		#region ClaimTranAmt
		public abstract class claimTranAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _ClaimTranAmt;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Amount in Claim Original")]
        public virtual Decimal? ClaimTranAmt
		{
			get
			{
				return this._ClaimTranAmt;
			}
			set
			{
				this._ClaimTranAmt = value;
			}
		}
		#endregion
		#region TranDesc
		public abstract class tranDesc : PX.Data.IBqlField
		{
		}
		protected String _TranDesc;
		[PXDBString(256, IsUnicode = true)]
		[PXDefault]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.Visible)]
		public virtual String TranDesc
		{
			get
			{
				return this._TranDesc;
			}
			set
			{
				this._TranDesc = value;
			}
		}
		#endregion
		#region CustomerID
		public abstract class customerID : PX.Data.IBqlField
		{
		}
		protected Int32? _CustomerID;
		[PXDefault(typeof(EPExpenseClaim.customerID), PersistingCheck = PXPersistingCheck.Nothing)]
		[CustomerActive(DescriptionField = typeof(Customer.acctName))]
		[PXUIRequired(typeof(billable))]
		[PXUIEnabled(typeof(Where<contractID, IsNull, Or<Selector<contractID, Contract.nonProject>, Equal<True>, Or<Selector<contractID, Contract.customerID>, IsNull>>>))]
		[PXUIVerify(typeof(Where<refNbr, IsNull, 
                                Or<Current2<customerID>, IsNull, 
                                Or<Selector<refNbr, EPExpenseClaim.customerID>, IsNull, 
                                Or<Current2<customerID>, Equal<Selector<refNbr, EPExpenseClaim.customerID>>, 
                                Or<billable, NotEqual<True>, 
                                Or<Selector<contractID, Contract.nonProject>, Equal<False>>>>>>>), PXErrorLevel.Warning, Messages.CustomerDoesNotMatch)]
		[PXFormula(typeof(Switch<Case<Where<Selector<contractID, Contract.nonProject>, Equal<False>>, Selector<contractID, Contract.customerID>>, Null>))]
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
		#region CustomerLocationID
		public abstract class customerLocationID : PX.Data.IBqlField
		{
		}
		protected Int32? _CustomerLocationID;
		[PXDefault(typeof(Search<Customer.defLocationID, Where<Customer.bAccountID, Equal<Current<EPExpenseClaimDetails.customerID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[LocationID(typeof(Where<Location.bAccountID, Equal<Current2<customerID>>>), DescriptionField = typeof(Location.descr))]
		[PXUIEnabled(typeof(Where<Current2<customerID>, IsNotNull, And<Where<contractID, IsNull, Or<Selector<contractID, Contract.nonProject>, Equal<True>, Or<Selector<contractID, Contract.customerID>, IsNull>>>>>))]
		[PXFormula(typeof(Switch<Case<Where<Current2<customerID>, IsNull>, Null>, Selector<customerID, Customer.defLocationID>>))]
		public virtual Int32? CustomerLocationID
		{
			get
			{
				return this._CustomerLocationID;
			}
			set
			{
				this._CustomerLocationID = value;
			}
		}
		#endregion
        #region ContractID
        public abstract class contractID : PX.Data.IBqlField
        {
        }
        protected Int32? _ContractID;

        [PXDBInt]
        [PXUIField(DisplayName = "Project/Contract")]
		[PXDimensionSelector(ContractAttribute.DimensionName, 
                            typeof(Search2<Contract.contractID,
			                               LeftJoin<EPEmployeeContract, 
                                                    On<EPEmployeeContract.contractID, Equal<Contract.contractID>, 
                                                    And<EPEmployeeContract.employeeID, Equal<Current2<employeeID>>>>>,
			                               Where<Contract.isTemplate, Equal<False>, 
                                                 And<Contract.isActive, Equal<True>, 
                                                 And<Contract.isCompleted, Equal<False>, 
                                                 And<Where<Contract.nonProject, Equal<True>, 
                                                           Or2<Where<Contract.baseType, Equal<Contract.ContractBaseType>, 
                                                           And<FeatureInstalled<FeaturesSet.contractManagement>>>, 
                                                           Or<Contract.baseType, Equal<PMProject.ProjectBaseType>, 
                                                           And2<Where<Contract.visibleInEP, Equal<True>>, 
                                                           And2<FeatureInstalled<FeaturesSet.projectModule>, 
                                                           And2<Match<Current<AccessInfo.userName>>, 
                                                           And<Where<Contract.restrictToEmployeeList, Equal<False>, 
                                                           Or<EPEmployeeContract.employeeID, IsNotNull>>>>>>>>>>>>>, 
                                           OrderBy<Desc<Contract.contractCD>>>), 
                             typeof(Contract.contractCD), 
                             typeof(Contract.contractCD), 
                             typeof(Contract.description), 
                             typeof(Contract.customerID), 
                             typeof(Contract.status), 
                             Filterable = true, 
                             ValidComboRequired = true, 
                             CacheGlobal = true, 
                             DescriptionField=typeof(Contract.description))]
		[ProjectDefault(BatchModule.EP, AccountType=typeof(expenseAccountID))]
        public virtual Int32? ContractID
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

        #region ProjectDescription
        public abstract class projectDescription : IBqlField { }

        [PXFormula(typeof(Selector<EPExpenseClaimDetails.contractID, Contract.description>))]
        [PXUIField(DisplayName = "Project Description", IsReadOnly = true)]
        [PXString]
        public virtual string ProjectDescription { get; set; }
        #endregion

        #region TaskID
        public abstract class taskID : PX.Data.IBqlField
        {
        }
        protected Int32? _TaskID;
        [ActiveProjectTask(typeof(EPExpenseClaimDetails.contractID), BatchModule.EP, DisplayName = "Project Task")]
		[PXUIEnabled(typeof(Where<contractID, IsNotNull, And<Selector<contractID, Contract.baseType>, Equal<PMProject.ProjectBaseType>>>))]
		[PXFormula(typeof(Switch<Case<Where<contractID, IsNull, Or<Selector<contractID, Contract.baseType>, NotEqual<PMProject.ProjectBaseType>>>, Null>, taskID>))]
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


        #region ProjectTaskDescription
        public abstract class projectTaskDescription : IBqlField { }

        [PXUIField(DisplayName = "Project Task Description", IsReadOnly = true)]
        [PXFormula(typeof(Selector<EPExpenseClaimDetails.taskID, PMTask.description>))]
        [PXString]
        public virtual string ProjectTaskDescription { get; set; }
        #endregion

		#region Billable
		public abstract class billable : PX.Data.IBqlField
		{
		}
		protected Boolean? _Billable;
		[PXDBBool()]
		[PXDefault(false)]
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
		#region Billed
		public abstract class billed : PX.Data.IBqlField
		{
		}
		protected Boolean? _Billed;
		[PXDBBool()]
		[PXDefault(false)]
        [PXUIField(DisplayName = "Billed")]
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

		#region Released
		public abstract class released : PX.Data.IBqlField
		{
		}
		protected Boolean? _Released;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Released",Visible = false)]
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
		#region ExpenseAccountID
		public abstract class expenseAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _ExpenseAccountID;
		[PXDefault]
		[PXFormula(typeof(Selector<inventoryID, InventoryItem.cOGSAcctID>))]
		[Account(DisplayName = "Expense Account", Visibility = PXUIVisibility.Visible)]
		[PXUIVerify(typeof(Where<Current2<contractID>, IsNull, 
                                 Or<Current2<expenseAccountID>, IsNull, 
                                 Or<Selector<contractID, Contract.nonProject>, Equal<True>, 
                                 Or<Selector<contractID, Contract.baseType>, Equal<Contract.ContractBaseType>, 
                                 Or<Selector<expenseAccountID, Account.accountGroupID>, IsNotNull>>>>>), 
                    PXErrorLevel.Error, 
                    Messages.AccountGroupIsNotAssignedForAccount, 
                    typeof(expenseAccountID))]//, account.AccountCD.Trim())]
		public virtual Int32? ExpenseAccountID
		{
			get
			{
				return this._ExpenseAccountID;
			}
			set
			{
				this._ExpenseAccountID = value;
			}
		}
		#endregion
		#region ExpenseSubID
		public abstract class expenseSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _ExpenseSubID;
		[PXDefault]
		[PXFormula(typeof(Default<inventoryID, contractID, taskID, customerLocationID>))]
		[SubAccount(typeof(EPExpenseClaimDetails.expenseAccountID), DisplayName = "Expense Sub.", Visibility = PXUIVisibility.Visible)]
		public virtual Int32? ExpenseSubID
		{
			get
			{
				return this._ExpenseSubID;
			}
			set
			{
				this._ExpenseSubID = value;
			}
		}
		#endregion
		#region SalesAccountID
		public abstract class salesAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _SalesAccountID;
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXFormula(typeof(Switch<Case<Where<billable, Equal<True>>, Selector<inventoryID, InventoryItem.salesAcctID>>, Null>))]
		[PXUIRequired(typeof(billable))]
		[PXUIEnabled(typeof(billable))]
		[Account(DisplayName = "Sales Account", Visibility = PXUIVisibility.Visible)]
	  
		public virtual Int32? SalesAccountID
		{
			get
			{
				return this._SalesAccountID;
			}
			set
			{
				this._SalesAccountID = value;
			}
		}
		#endregion
		#region SalesSubID
		public abstract class salesSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _SalesSubID;
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXFormula(typeof(Default<billable, inventoryID, contractID, taskID>))]
		[SubAccount(typeof(EPExpenseClaimDetails.salesAccountID), DisplayName = "Sales Sub.", Visibility = PXUIVisibility.Visible)]
		[PXUIRequired(typeof(billable))]
		[PXUIEnabled(typeof(billable))]
		public virtual Int32? SalesSubID
		{
			get
			{
				return this._SalesSubID;
			}
			set
			{
				this._SalesSubID = value;
			}
		}
		#endregion
		#region ARDocType
		public abstract class aRDocType : PX.Data.IBqlField
		{
		}
		protected String _ARDocType;
		[PXDBString(3, IsFixed = true)]
		[ARDocType.List()]
		[PXUIField(DisplayName = "AR Doument Type", Visibility = PXUIVisibility.Visible, Enabled = false, TabOrder = -1)]
		public virtual String ARDocType
		{
			get
			{
				return this._ARDocType;
			}
			set
			{
				this._ARDocType = value;
			}
		}
		#endregion
		#region ARRefNbr
		public abstract class aRRefNbr : PX.Data.IBqlField
		{
		}
		protected String _ARRefNbr;
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "AR Reference Nbr.", Enabled=false)]
		[PXSelector(typeof(Search<ARInvoice.refNbr, Where<ARInvoice.docType, Equal<Optional<EPExpenseClaimDetails.aRDocType>>>>))]
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
		#region byCorporateCard
		public abstract class byCorporateCard : PX.Data.IBqlField
		{
		}
		protected Boolean? _byCorporateCard;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Paid by Corporate Card")]
		public virtual Boolean? ByCorporateCard
		{
			get
			{
				return this._byCorporateCard;
			}
			set
			{
				this._byCorporateCard = value;
			}
		}
		#endregion
		#region Status
		public abstract class status : PX.Data.IBqlField
		{
		}

		[PXDBString(1, IsFixed = true)]
		[PXDefault(EPExpenseClaimDetailsStatus.HoldStatus)]
		[PXUIField(DisplayName = "Status", Enabled = false)]
		[EPExpenseClaimDetailsStatus.List()]
		public virtual String Status
		{
		    get;
			set;
		}
		#endregion
		#region Status Claim
		public abstract class statusClaim : PX.Data.IBqlField
		{
		}

		[PXString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Expense Claim Status", Enabled = false)]
        [EPExpenseClaimStatus.List()]
        [PXFormula(typeof(Switch<Case<Where<EPExpenseClaimDetails.refNbr, IsNotNull>,
                            Selector<EPExpenseClaimDetails.refNbrNotFiltered, EPExpenseClaim.status>>,
                            Null>))]
		public virtual String StatusClaim
		{
		    get;
            set;
        }
        #endregion
        #region Hold Claim
        public abstract class holdClaim : PX.Data.IBqlField
		{
		}
        [PXBool()]
        [PXFormula(typeof(Switch<Case<Where<EPExpenseClaimDetails.refNbr, IsNotNull>,
                                    Selector<EPExpenseClaimDetails.refNbrNotFiltered, EPExpenseClaim.hold>>,
                                    True>))]
        [PXDefault(true, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(Visible = false)]
        public virtual Boolean? HoldClaim
        {
            get;
            set;          
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
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
			[PXSearchable(SM.SearchCategory.TM, Messages.SearchableTitleExpenseReceipt, new Type[] { typeof(EPExpenseClaimDetails.refNbr), typeof(EPExpenseClaimDetails.employeeID), typeof(EPEmployee.acctName) },
			  new Type[] { typeof(EPExpenseClaimDetails.tranDesc) },
			  NumberFields = new Type[] { typeof(EPExpenseClaimDetails.refNbr) },
			  Line1Format = "{0:d}{1}{2}", Line1Fields = new Type[] { typeof(EPExpenseClaimDetails.expenseDate), typeof(EPExpenseClaimDetails.status), typeof(EPExpenseClaimDetails.refNbr) },
			  Line2Format = "{0}", Line2Fields = new Type[] { typeof(EPExpenseClaimDetails.tranDesc) },
			  SelectForFastIndexing = typeof(Select2<EPExpenseClaimDetails, InnerJoin<EPEmployee, On<EPExpenseClaimDetails.employeeID, Equal<EPEmployee.bAccountID>>>>),
			  SelectDocumentUser = typeof(Select2<Users,
				InnerJoin<EPEmployee, On<Users.pKID, Equal<EPEmployee.userID>>>,
			  Where<EPEmployee.bAccountID, Equal<Current<EPExpenseClaimDetails.employeeID>>>>)
		  )]
        [PXNote(
            DescriptionField = typeof(EPExpenseClaimDetails.claimDetailID),
            Selector = typeof(EPExpenseClaimDetails.claimDetailID),
            ShowInReferenceSelector = true)]
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
		#region Selected
		public abstract class selected : IBqlField
		{
		}
		protected bool? _Selected = false;
		[PXBool]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Selected")]
		public bool? Selected
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
		#region Submited
		public abstract class submited : IBqlField
		{
		}
		protected bool? _Submited = false;
		[PXDBBool]
		[PXUIField(DisplayName = "Submitted", Enabled = false)]
		public bool? Submited
		{
			get
			{
				return _Submited;
			}
			set
			{
				_Submited = value;
			}
		}
		#endregion

	}
}
