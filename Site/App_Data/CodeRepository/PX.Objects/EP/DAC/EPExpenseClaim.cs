using System;
using System.Collections.Generic;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.CS;
using PX.Objects.AR;
using PX.Objects.AP;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.SM;
using PX.TM;
using PX.Objects.TX;
using PX.Objects.Common;

namespace PX.Objects.EP
{
    public class EPExpenseClaimStatus : ILabelProvider
    {
        public const string ApprovedStatus = "A";
        public const string HoldStatus = "H";
        public const string ReleasedStatus = "R";
        public const string OpenStatus = "O";
        public const string RejectedStatus = "C";

        private static readonly IEnumerable<ValueLabelPair> _valueLabelPairs = new ValueLabelList
    {
        { HoldStatus, Messages.HoldStatus },
        { OpenStatus, Messages.OpenStatus },
        { ApprovedStatus, Messages.Approved },
        { RejectedStatus, Messages.RejectedStatus },
        { ReleasedStatus, Messages.ReleasedStatus  },
    };
        public IEnumerable<ValueLabelPair> ValueLabelPairs => _valueLabelPairs;

        public class ListAttribute : LabelListAttribute
        {
            public ListAttribute() : base(_valueLabelPairs)
            { }
        }

    }
    [System.SerializableAttribute()]
	[PXPrimaryGraph(typeof(ExpenseClaimEntry))]
	[PXCacheName(Messages.ExpenseClaim)]
	[PXEMailSource]
	public partial class EPExpenseClaim : PX.Data.IBqlTable, PX.Data.EP.IAssign
	{
		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		protected Int32? _BranchID;
		[GL.Branch()]
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
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "")]
		[PXDefault()]
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible, TabOrder = 1)]
		[EPExpenceClaimSelector]
		[AutoNumber(typeof(EPSetup.claimNumberingID), typeof(EPExpenseClaim.docDate))]
		[PX.Data.EP.PXFieldDescription]
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
		#region EmployeeID
		public abstract class employeeID : PX.Data.IBqlField
		{
		}
		protected Int32? _EmployeeID;
		[PXDBInt()]
		[PXDefault(typeof(Search<EPEmployee.bAccountID, Where<EPEmployee.userID,Equal<Current<AccessInfo.userID>>>>))]
		[PXSubordinateAndWingmenSelector]
		[PXUIField(DisplayName = "Claimed by", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region WorkgroupID
		public abstract class workgroupID : PX.Data.IBqlField
		{
		}
		protected int? _WorkgroupID;
		[PXDBInt]
		[PXUIField(DisplayName = "Approval Workgroup")]
        [PXSelector(typeof(EPCompanyTreeOwner.workGroupID), SubstituteKey = typeof(EPCompanyTreeOwner.description))]
		public virtual int? WorkgroupID
		{
			get
			{
				return this._WorkgroupID;
			}
			set
			{
				this._WorkgroupID = value;
			}
		}
		#endregion
		#region ApproverID
		public abstract class approverID : IBqlField { }
		protected Guid? _ApproverID;
		[PXDBGuid()]
		[PXOwnerSelector(typeof(EPExpenseClaim.workgroupID))]
		[PXDefault(typeof(Search<EPCompanyTreeMember.userID,
		 Where<EPCompanyTreeMember.workGroupID, Equal<Current<EPExpenseClaim.workgroupID>>,
			And<EPCompanyTreeMember.isOwner, Equal<boolTrue>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Approver", Visible = false)]
		public virtual Guid? ApproverID
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
		#region ApprovedByID
		public abstract class approvedByID : IBqlField { }
		protected Guid? _ApprovedByID;
		[PXDBGuid()]
		[PX.TM.PXOwnerSelector]
		[PXUIField(DisplayName = "Approved By", Visibility = PXUIVisibility.Visible, Enabled=false)]
		public virtual Guid? ApprovedByID
		{
			get
			{
				return this._ApprovedByID;
			}
			set
			{
				this._ApprovedByID = value;
			}
		}
		#endregion
		#region DepartmentID
		public abstract class departmentID : PX.Data.IBqlField
		{
		}
		protected String _DepartmentID;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault(typeof(Search<EPEmployee.departmentID, Where<EPEmployee.bAccountID, Equal<Current<EPExpenseClaim.employeeID>>>>))]
		[PXSelector(typeof(EPDepartment.departmentID), DescriptionField = typeof(EPDepartment.description))]
		[PXUIField(DisplayName = "Department ID", Enabled = false, Visibility = PXUIVisibility.Visible)]
		public virtual String DepartmentID
		{
			get
			{
				return this._DepartmentID;
			}
			set
			{
				this._DepartmentID = value;
			}
		}
		#endregion
		#region LocationID
		public abstract class locationID : PX.Data.IBqlField
		{
		}
		protected Int32? _LocationID;
		[PXDefault(typeof(Search<EPEmployee.defLocationID, Where<EPEmployee.bAccountID, Equal<Current<EPExpenseClaim.employeeID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[LocationID(typeof(Where<Location.bAccountID, Equal<Current<EPExpenseClaim.employeeID>>>), Visibility = PXUIVisibility.SelectorVisible)]
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
		#region DocDate
		public abstract class docDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _DocDate;
		[PXDBDate()]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Date", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual DateTime? DocDate
		{
			get
			{
				return this._DocDate;
			}
			set
			{
				this._DocDate = value;
			}
		}
		#endregion
		#region ApproveDate
		public abstract class approveDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ApproveDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Approval Date", Enabled = false)]

		public virtual DateTime? ApproveDate
		{
			get
			{
				return this._ApproveDate;
			}
			set
			{
				this._ApproveDate = value;
			}
		}
		#endregion
		#region TranPeriodID
		public abstract class tranPeriodID : PX.Data.IBqlField
		{
		}
		protected String _TranPeriodID;
		[TranPeriodID(typeof(EPExpenseClaim.docDate))]
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
		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
        protected String _FinPeriodID;
        [APOpenPeriod(ValidatePeriod = PeriodValidation.Nothing)]
        [PXFormula(typeof(Switch<Case<Where<EPExpenseClaim.hold, Equal<True>>, Null>, finPeriodID>))]
        [PXUIField(DisplayName = "Post to Period")]
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
		#region DocDesc
		public abstract class docDesc : PX.Data.IBqlField
		{
		}
		protected String _DocDesc;
		[PXDBString(60, IsUnicode = true)]
		[PXDefault]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String DocDesc
		{
			get
			{
				return this._DocDesc;
			}
			set
			{
				this._DocDesc = value;
			}
		}
		#endregion
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL")]
		[PXUIField(DisplayName = "Currency", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[PXDefault(typeof(Search<Company.baseCuryID>))]
		[PXSelector(typeof(Currency.curyID))]
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
		#region CuryInfoID
		public abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _CuryInfoID;
		[PXDBLong()]
		[CurrencyInfo(ModuleCode = "EP")]
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
		#region CuryDocBal
		public abstract class curyDocBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryDocBal;
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(EPExpenseClaim.curyInfoID), typeof(EPExpenseClaim.docBal))]
		[PXUIField(DisplayName = "Claim Total", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? CuryDocBal
		{
			get
			{
				return this._CuryDocBal;
			}
			set
			{
				this._CuryDocBal = value;
			}
		}
		#endregion
		#region DocBal
		public abstract class docBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _DocBal;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? DocBal
		{
			get
			{
				return this._DocBal;
			}
			set
			{
				this._DocBal = value;
			}
		}
		#endregion
		#region Approved
		public abstract class approved : PX.Data.IBqlField
		{
		}
		protected Boolean? _Approved;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Approve", Visibility = PXUIVisibility.Visible)]
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
		#region Released
		public abstract class released : PX.Data.IBqlField
		{
		}
		protected Boolean? _Released;
		[PXDBBool()]
		[PXDefault(false)]
        [PXUIField(DisplayName =  "Released")]
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
		#region Hold
		public abstract class hold : PX.Data.IBqlField
		{
		}
		protected Boolean? _Hold;
		[PXDBBool()]
		[PXDefault(true)]
		public virtual Boolean? Hold
		{
			get
			{
				return this._Hold;
			}
			set
			{
				this._Hold = value;
			}
		}
		#endregion
		#region Rejected
		public abstract class rejected : PX.Data.IBqlField
		{
		}
		protected Boolean? _Rejected;
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? Rejected
		{
			get
			{
				return this._Rejected;
			}
			set
			{
				this._Rejected = value;
			}
		}
		#endregion
		#region Status
		public abstract class status : PX.Data.IBqlField
		{
		}
		[PXDBString(1)]
		[PXDefault(EPExpenseClaimStatus.HoldStatus)]
		[EPExpenseClaimStatus.List()]
		[PXUIField(DisplayName = "Status", Enabled = false)]
		public virtual String Status
		{
		    get;
            set;
        }
		#endregion
		#region APDocType
		public abstract class aPDocType : PX.Data.IBqlField
		{
		}
		protected String _APDocType;
		[PXDBString(3, IsFixed = true)]
		[APDocType.List()]
        [PXUIField(DisplayName = "AP Document Type", Visibility = PXUIVisibility.Visible, Enabled = false, TabOrder = -1)]
		public virtual String APDocType
		{
			get
			{
				return this._APDocType;
			}
			set
			{
				this._APDocType = value;
			}
		}
		#endregion
		#region APRefNbr
		public abstract class aPRefNbr : PX.Data.IBqlField
		{
		}
		protected String _APRefNbr;
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "AP Reference Nbr.")]
		[PXSelector(typeof(Search<APInvoice.refNbr, Where<APInvoice.docType, Equal<Optional<EPExpenseClaim.aPDocType>>>>))]
		public virtual String APRefNbr
		{
			get
			{
				return this._APRefNbr;
			}
			set
			{
				this._APRefNbr = value;
			}
		}
		#endregion
        #region APStatus
        public abstract class aPStatus : PX.Data.IBqlField
        {
        }
        protected String _APStatus;
        [PXString(1, IsFixed = true)]
        [PXUIField(DisplayName = "AP Document Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
        [APDocStatus.List()]
        public virtual String APStatus
        {
            get
            {
                return this._APStatus;
            }
            set
            {
                this._APStatus = value;
            }
        }
        #endregion
		#region TaxZoneID
		public abstract class taxZoneID : PX.Data.IBqlField
		{
		}
		protected String _TaxZoneID;
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Tax Zone", Visibility = PXUIVisibility.Visible)]
		[PXSelector(typeof(TaxZone.taxZoneID), DescriptionField = typeof(TaxZone.descr), Filterable = true)]
		[PXRestrictor(typeof(Where<TaxZone.isManualVATZone, Equal<False>>), TX.Messages.CantUseManualVAT)]
		[PXDefault(typeof(Search<Location.vTaxZoneID,
						Where<Location.bAccountID, Equal<Current<EPExpenseClaim.employeeID>>, And<Location.locationID, Equal<Current<EPExpenseClaim.locationID>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual String TaxZoneID
		{
			get
			{
				return this._TaxZoneID;
			}
			set
			{
				this._TaxZoneID = value;
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
		[PXSearchable(SM.SearchCategory.TM, Messages.SearchableTitleExpenseClaim, new Type[] { typeof(EPExpenseClaim.refNbr), typeof(EPExpenseClaim.employeeID), typeof(EPEmployee.acctName) },
			  new Type[] { typeof(EPExpenseClaim.docDesc) },
			  NumberFields = new Type[] { typeof(EPExpenseClaim.refNbr) },
			  Line1Format = "{0:d}{1}{2}", Line1Fields = new Type[] { typeof(EPExpenseClaim.docDate), typeof(EPExpenseClaim.status), typeof(EPExpenseClaim.refNbr) },
			  Line2Format = "{0}", Line2Fields = new Type[] { typeof(EPExpenseClaim.docDesc) },
			  SelectForFastIndexing = typeof(Select2<EPExpenseClaim, InnerJoin<EPEmployee, On<EPExpenseClaim.employeeID, Equal<EPEmployee.bAccountID>>>>),
			  SelectDocumentUser = typeof(Select2<Users, 
				InnerJoin<EPEmployee, On<Users.pKID, Equal<EPEmployee.userID>>>,
			  Where<EPEmployee.bAccountID, Equal<Current<EPExpenseClaim.employeeID>>>>)
		  )]
		[PXNote(DescriptionField = typeof(EPExpenseClaim.refNbr), 
			Selector = typeof(EPExpenseClaim.refNbr))]
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
		[PXUIField]
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
		#region CuryTaxTotal
		public abstract class curyTaxTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryTaxTotal;
		[PXCurrency(typeof(EPExpenseClaim.curyInfoID), typeof(EPExpenseClaim.taxTotal))]
		[PXUIField(DisplayName = "Tax Total", Visibility = PXUIVisibility.Visible, Enabled = false)]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck=PXPersistingCheck.Nothing)]
		public virtual Decimal? CuryTaxTotal
		{
			get
			{
				return this._CuryTaxTotal;
			}
			set
			{
				this._CuryTaxTotal = value;
			}
		}
		#endregion
		#region TaxTotal
		public abstract class taxTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _TaxTotal;
		[PXDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Decimal? TaxTotal
		{
			get
			{
				return this._TaxTotal;
			}
			set
			{
				this._TaxTotal = value;
			}
		}
		#endregion
		#region CuryLineTotal
		public abstract class curyLineTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryLineTotal;
		[PXCurrency(typeof(EPExpenseClaim.curyInfoID), typeof(EPExpenseClaim.lineTotal))]
		[PXUIField(DisplayName = "Detail Total", Visibility = PXUIVisibility.Visible, Enabled = false)]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Decimal? CuryLineTotal
		{
			get
			{
				return this._CuryLineTotal;
			}
			set
			{
				this._CuryLineTotal = value;
			}
		}
		#endregion
		#region LineTotal
		public abstract class lineTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _LineTotal;
		[PXDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Decimal? LineTotal
		{
			get
			{
				return this._LineTotal;
			}
			set
			{
				this._LineTotal = value;
			}
		}
		#endregion                        
		#region CustomerID
		public abstract class customerID : PX.Data.IBqlField
		{
		}
		protected Int32? _CustomerID;
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[CustomerActive(DescriptionField = typeof(Customer.acctName))]
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
		[PXUIEnabled(typeof(Where<Current2<customerID>, IsNotNull>))]
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
              
        #region CuryVatExemptTotal
        public abstract class curyVatExemptTotal : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryVatExemptTotal;
        [PXDBCurrency(typeof(EPExpenseClaim.curyInfoID), typeof(EPExpenseClaim.vatExemptTotal))]
        [PXUIField(DisplayName = "VAT Exempt Total", Visibility = PXUIVisibility.Visible, Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? CuryVatExemptTotal
        {
            get
            {
                return this._CuryVatExemptTotal;
            }
            set
            {
                this._CuryVatExemptTotal = value;
            }
        }
        #endregion

        #region VatExemptTaxTotal
        public abstract class vatExemptTotal : PX.Data.IBqlField
        {
        }
        protected Decimal? _VatExemptTotal;
        [PXDBDecimal(4)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? VatExemptTotal
        {
            get
            {
                return this._VatExemptTotal;
            }
            set
            {
                this._VatExemptTotal = value;
            }
        }
        #endregion

        #region CuryVatTaxableTotal
        public abstract class curyVatTaxableTotal : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryVatTaxableTotal;
        [PXDBCurrency(typeof(EPExpenseClaim.curyInfoID), typeof(EPExpenseClaim.vatTaxableTotal))]
        [PXUIField(DisplayName = "VAT Taxable Total", Visibility = PXUIVisibility.Visible, Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? CuryVatTaxableTotal
        {
            get
            {
                return this._CuryVatTaxableTotal;
            }
            set
            {
                this._CuryVatTaxableTotal = value;
            }
        }
        #endregion


        #region VatTaxableTotal
        public abstract class vatTaxableTotal : PX.Data.IBqlField
        {
        }
        protected Decimal? _VatTaxableTotal;
        [PXDBDecimal(4)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? VatTaxableTotal
        {
            get
            {
                return this._VatTaxableTotal;
            }
            set
            {
                this._VatTaxableTotal = value;
            }
        }
        #endregion
       
		#region IAssign Members

		public int? ID
		{
			get { return null; }
		}


		public string EntityType
		{
			get { return null; }
		}
	  		
		public Guid? OwnerID
		{
			get { return ApproverID; }
			set { ApproverID = value; }
		}
		#endregion
	}
}
