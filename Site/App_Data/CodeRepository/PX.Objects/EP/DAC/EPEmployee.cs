using PX.TM;
using System;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CA;
using PX.Objects.IN;

namespace PX.Objects.EP
{
	[System.SerializableAttribute()]
	[PXTable(typeof(PX.Objects.CR.BAccount.bAccountID))]
	[PXCacheName(Messages.Employee)]
	[PXPrimaryGraph(typeof(EmployeeMaint))]
	public partial class EPEmployee : Vendor
	{
		#region BAccountID
		public new abstract class bAccountID : IBqlField {}
		#endregion
		#region AcctCD
		public abstract new class acctCD : IBqlField
		{
		}
		[EmployeeRaw]
		[PXDBString(30, IsUnicode = true, IsKey = true, InputMask="")]
		[PXDefault()]
		[PXUIField(DisplayName = "Employee ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PX.Data.EP.PXFieldDescription]
		public override String AcctCD
		{
			get
			{
				return this._AcctCD;
			}
			set
			{
				this._AcctCD = value;
			}
		}
		#endregion
		#region DefContactID
		public abstract new class defContactID : IBqlField { }
		[PXDBInt()]
		[PXDBChildIdentity(typeof(Contact.contactID))]
		[PXUIField(DisplayName = "Default Contact")]
		[PXSelector(typeof(Search<Contact.contactID, Where<Contact.bAccountID, Equal<Current<EPEmployee.parentBAccountID>>>>))]
		public override Int32? DefContactID
		{
			get
			{
				return this._DefContactID;
			}
			set
			{
				this._DefContactID = value;
			}
		}
		#endregion
		#region DefAddressID
		public abstract new class defAddressID : IBqlField { }
		[PXDBInt()]
		[PXDBChildIdentity(typeof(Address.addressID))]
		[PXUIField(DisplayName = "Default Address")]
		[PXSelector(typeof(Search<Address.addressID, Where<Address.bAccountID, Equal<Current<EPEmployee.parentBAccountID>>>>))]
		public override Int32? DefAddressID
		{
			get
			{
				return this._DefAddressID;
			}
			set
			{
				this._DefAddressID = value;
			}
		}

		#endregion
		#region ParentBAccountID
		public abstract new class parentBAccountID : IBqlField { }
		[PXDBInt()]
		[PXDefault(typeof(Search<Branch.bAccountID, Where<Branch.branchID, Equal<Current<AccessInfo.branchID>>>>))]
		[PXUIField(DisplayName = "Branch")]
        [PXDimensionSelector("BIZACCT", typeof(Search2<Branch.bAccountID, LeftJoin<Ledger, On<Ledger.defBranchID, Equal<Branch.branchID>>>, Where2<MatchWithBranch<Branch.branchID>, And<Ledger.ledgerID, IsNull>>>), typeof(Branch.branchCD), DescriptionField = typeof(Branch.acctName))]
		public override Int32? ParentBAccountID
		{
			get
			{
				return this._ParentBAccountID;
			}
			set
			{
				this._ParentBAccountID = value;
			}
		}
		#endregion
		#region Type
		[PXDBString(2, IsFixed = true)]
		[PXDefault(BAccountType.EmployeeType)]
		[PXUIField(DisplayName = "Type")]
		[BAccountType.List()]
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
		#region AcctName
				public new abstract class acctName : PX.Data.IBqlField
				{
				}
		[PXDBString(60, IsUnicode = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Employee Name", Enabled = false, Visibility = PXUIVisibility.SelectorVisible)]
		public override string AcctName
		{
			get
			{
				return base.AcctName;
			}
			set
			{
				base.AcctName = value;
			}
		}
		#endregion
		#region AcctReferenceNbr
		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "Employee Ref. No.", Visibility = PXUIVisibility.Visible)]
		public override string AcctReferenceNbr
		{
			get
			{
				return base.AcctReferenceNbr;
			}
			set
			{
				base.AcctReferenceNbr = value;
			}
		}
		#endregion
		#region VendorClassID
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXDefault]
		[PXUIField(DisplayName = "Employee Class", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(EPEmployeeClass.vendorClassID),DescriptionField = typeof(EPEmployeeClass.descr))]
		public override String VendorClassID
		{
			get
			{
				return this._VendorClassID;
			}
			set
			{
				this._VendorClassID = value;
			}
		}
		#endregion
        #region Attributes

	    [CRAttributesField(typeof (EPEmployee.vendorClassID), typeof (BAccount.noteID))]
	    public override string[] Attributes { get; set; }

	    #endregion
		
		#region DepartmentID
		public abstract class departmentID : PX.Data.IBqlField
		{
		}
		protected String _DepartmentID;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault()]
		[PXSelector(typeof(EPDepartment.departmentID), DescriptionField = typeof(EPDepartment.description))]
		[PXUIField(DisplayName = "Department", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region DefLocationID
		public abstract new class defLocationID : IBqlField {}
		[PXDefault()]
		[PXDBInt()]
		[PXUIField(DisplayName = "Default Location", Visibility = PXUIVisibility.SelectorVisible)]
		[DefLocationID(typeof(Search<Location.locationID, Where<Location.bAccountID, Equal<Current<EPEmployee.bAccountID>>>>), SubstituteKey=typeof(Location.locationCD), DescriptionField = typeof(Location.descr))]
		[PXDBChildIdentity(typeof(Location.locationID))]
		public override int? DefLocationID
		{
			get
			{
				return base.DefLocationID;
			}
			set
			{
				base.DefLocationID = value;
			}
		}
		#endregion
		#region SupervisorID
		public abstract class supervisorID : PX.Data.IBqlField
		{
		}
		protected Int32? _SupervisorID;
		[PXDBInt()]
		[PXEPEmployeeSelector]
		[PXUIField(DisplayName = "Reports to", Visibility = PXUIVisibility.Visible)]
		public virtual Int32? SupervisorID
		{
			get
			{
				return this._SupervisorID;
			}
			set
			{
				this._SupervisorID = value;
			}
		}
		#endregion
		#region SalesPersonID
		public abstract class salesPersonID : PX.Data.IBqlField
		{
		}
		protected Int32? _SalesPersonID;
		[PXDBInt()]
		[PXDimensionSelector("SALESPER", typeof(Search5<SalesPerson.salesPersonID,
												LeftJoin<EPEmployee,On<SalesPerson.salesPersonID,Equal<EPEmployee.salesPersonID>>>,
											   Where<EPEmployee.bAccountID, IsNull,
											   Or<SalesPerson.salesPersonID,Equal<Current<EPEmployee.salesPersonID>>>>, 
											   Aggregate<GroupBy<SalesPerson.salesPersonID>>>), 
											   typeof(SalesPerson.salesPersonCD), 
											   typeof(SalesPerson.salesPersonCD),
											   typeof(SalesPerson.descr))]
		[PXUIField(DisplayName = "Salesperson", Visibility = PXUIVisibility.Visible)]
		public virtual Int32? SalesPersonID
		{
			get
			{
				return this._SalesPersonID;
			}
			set
			{
				this._SalesPersonID = value;
			}
		}
				#endregion
		#region LabourItemID
		public abstract class labourItemID : PX.Data.IBqlField
		{
		}
		protected Int32? _LabourItemID;
		[PXDBInt()]
		[PXUIField(DisplayName = "Labor Item")]
		[PXDimensionSelector(InventoryAttribute.DimensionName, typeof(Search<InventoryItem.inventoryID, Where<InventoryItem.itemType, Equal<INItemTypes.laborItem>, And<Match<Current<AccessInfo.userName>>>>>), typeof(InventoryItem.inventoryCD))]
		public virtual Int32? LabourItemID
		{
			get
			{
				return this._LabourItemID;
			}
			set
			{
				this._LabourItemID = value;
			}
		}
		#endregion

		#region VendorClassID
		public new abstract class vendorClassID : IBqlField
		{
		}
		#endregion
		#region ClassID
		public new abstract class classID : IBqlField
		{
		}
		#endregion

		#region UserID
		public abstract class userID : IBqlField { }
		[PXDBGuid]
		[PXUIField(DisplayName = "Employee Login", Visibility = PXUIVisibility.Visible)]
		public virtual Guid? UserID { get; set; }
		#endregion
		
		#region SalesAcctID
		public abstract class salesAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _SalesAcctID;
		[PXDefault(typeof(Select<EPEmployeeClass, Where<EPEmployeeClass.vendorClassID, Equal<Current<EPEmployee.vendorClassID>>>>), SourceField = typeof(EPEmployeeClass.salesAcctID))]
		[Account(DisplayName = "Sales Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		public virtual Int32? SalesAcctID
		{
			get
			{
				return this._SalesAcctID;
			}
			set
			{
				this._SalesAcctID = value;
			}
		}
		#endregion
		#region SalesSubID
		public abstract class salesSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _SalesSubID;
		[PXDefault(typeof(Select<EPEmployeeClass, Where<EPEmployeeClass.vendorClassID, Equal<Current<EPEmployee.vendorClassID>>>>), SourceField = typeof(EPEmployeeClass.salesSubID))]
		[SubAccount(typeof(EPEmployee.salesAcctID), DisplayName = "Sales Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
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
		#region DiscTakenAcctID
		[Account(DisplayName = "Cash Discount Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		[PXDefault(typeof(Select<EPEmployeeClass, Where<EPEmployeeClass.vendorClassID, Equal<Current<EPEmployee.vendorClassID>>>>), SourceField = typeof(EPEmployeeClass.discTakenAcctID))]
		public override Int32? DiscTakenAcctID
		{
			get
			{
				return this._DiscTakenAcctID;
			}
			set
			{
				this._DiscTakenAcctID = value;
			}
		}
		#endregion
		#region DiscTakenSubID
		[SubAccount(typeof(Vendor.discTakenAcctID), DisplayName = "Cash Discount Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		[PXDefault(typeof(Select<EPEmployeeClass, Where<EPEmployeeClass.vendorClassID, Equal<Current<EPEmployee.vendorClassID>>>>), SourceField = typeof(EPEmployeeClass.discTakenSubID))]
		public override Int32? DiscTakenSubID
		{
			get
			{
				return this._DiscTakenSubID;
			}
			set
			{
				this._DiscTakenSubID = value;
			}
		}
		#endregion				
		#region ExpenseAcctID
		public abstract class expenseAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _ExpenseAcctID;
		[Account(DisplayName = "Expense Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		[PXDefault(typeof(Select<EPEmployeeClass, Where<EPEmployeeClass.vendorClassID, Equal<Current<EPEmployee.vendorClassID>>>>), SourceField = typeof(EPEmployeeClass.expenseAcctID))]
		public virtual Int32? ExpenseAcctID
		{
			get
			{
				return this._ExpenseAcctID;
			}
			set
			{
				this._ExpenseAcctID = value;
			}
		}
		#endregion
		#region ExpenseSubID
		public abstract class expenseSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _ExpenseSubID;
		[SubAccount(typeof(EPEmployee.expenseAcctID), DisplayName = "Expense Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		[PXDefault(typeof(Select<EPEmployeeClass, Where<EPEmployeeClass.vendorClassID, Equal<Current<EPEmployee.vendorClassID>>>>), SourceField = typeof(EPEmployeeClass.expenseSubID))]
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
		#region PrepaymentAcctID
		public new abstract class prepaymentAcctID : PX.Data.IBqlField
		{
		}
		[Account(DisplayName = "Prepayment Account", DescriptionField = typeof(Account.description))]
		[PXDefault(typeof(Select<EPEmployeeClass, Where<EPEmployeeClass.vendorClassID, Equal<Current<EPEmployee.vendorClassID>>>>), SourceField = typeof(EPEmployeeClass.prepaymentAcctID), PersistingCheck = PXPersistingCheck.Nothing)]
		public override Int32? PrepaymentAcctID
		{
			get
			{
				return this._PrepaymentAcctID;
			}
			set
			{
				this._PrepaymentAcctID = value;
			}
		}
		#endregion
		#region PrepaymentSubID
		public new abstract class prepaymentSubID : PX.Data.IBqlField
		{
		}
		[SubAccount(typeof(EPEmployee.prepaymentAcctID), DisplayName = "Prepayment Sub.", DescriptionField = typeof(Sub.description))]
		[PXDefault(typeof(Select<EPEmployeeClass, Where<EPEmployeeClass.vendorClassID, Equal<Current<EPEmployee.vendorClassID>>>>), SourceField = typeof(EPEmployeeClass.prepaymentSubID), PersistingCheck = PXPersistingCheck.Nothing)]
		public override Int32? PrepaymentSubID
		{
			get
			{
				return this._PrepaymentSubID;
			}
			set
			{
				this._PrepaymentSubID = value;
			}
		}
		#endregion

		
		#region CalendarID
		public abstract class calendarID : PX.Data.IBqlField
		{
		}
		protected String _CalendarID;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault(typeof(Select<EPEmployeeClass, Where<EPEmployeeClass.vendorClassID, Equal<Current<EPEmployee.vendorClassID>>>>), SourceField = typeof(EPEmployeeClass.calendarID))]
		[PXUIField(DisplayName = "Calendar", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<CSCalendar.calendarID>), DescriptionField = typeof(CSCalendar.description))]
		public virtual String CalendarID
		{
			get
			{
				return this._CalendarID;
			}
			set
			{
				this._CalendarID = value;
			}
		}
		#endregion
		#region PositionLineCntr
		public abstract class positionLineCntr : PX.Data.IBqlField
		{
		}
		protected int? _PositionLineCntr;
		[PXDBInt()]
		[PXDefault(0)]
		public virtual int? PositionLineCntr
		{
			get
			{
				return this._PositionLineCntr;
			}
			set
			{
				this._PositionLineCntr = value;
			}
		}
		#endregion	
		#region NoteID
		public abstract new class noteID : PX.Data.IBqlField
		{
		}
		[PXSearchable(SM.SearchCategory.OS, Messages.SearchableTitleEmployee, new Type[] { typeof(EPEmployee.acctCD), typeof(EPEmployee.acctName) },
		   new Type[] { typeof(EPEmployee.defContactID), typeof(Contact.eMail) },
		   NumberFields = new Type[] { typeof(EPEmployee.acctCD) },
			 Line1Format = "{1}{2}", Line1Fields = new Type[] { typeof(EPEmployee.defContactID), typeof(Contact.eMail), typeof(Contact.phone1) },
			 Line2Format = "{1}", Line2Fields = new Type[] { typeof(EPEmployee.departmentID), typeof(EPDepartment.description) },
			SelectForFastIndexing = typeof(Select2<EPEmployee, InnerJoin<Contact, On<Contact.contactID, Equal<EPEmployee.defContactID>>>>)
		 )]
		[PXNote(
			DescriptionField = typeof(EPEmployee.acctCD),
			Selector = typeof(EPEmployee.acctCD),
			ShowInReferenceSelector = true)]
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

		#region RouteEmails

		public abstract class routeEmails : IBqlField { }

		[PXDBBool]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Route Emails")]
		public virtual Boolean? RouteEmails { get; set; }

		#endregion
		#region TimeCardRequired

		public abstract class timeCardRequired : IBqlField { }

		[PXDBBool]
		[PXUIField(DisplayName = "Time Card is Required")]
		public virtual Boolean? TimeCardRequired { get; set; }

		#endregion
        #region HoursValidation
        public abstract class hoursValidation : PX.Data.IBqlField
        {
        }
        protected String _HoursValidation;
        [PXDBString(1)]
        [PXUIField(DisplayName = "Regular Hours Validation")]
        [HoursValidationOption.List]
		[PXDefault(typeof(Select<EPEmployeeClass, Where<EPEmployeeClass.vendorClassID, Equal<Current<EPEmployee.vendorClassID>>>>), SourceField = typeof(EPEmployeeClass.hoursValidation), Constant = HoursValidationOption.Validate)]
        public virtual String HoursValidation
        {
            get
            {
                return this._HoursValidation;
            }
            set
            {
                this._HoursValidation = value;
            }
        }
        #endregion
	}
	
	[Serializable]
	[PXCacheName(CR.Messages.Employee)]
    [PXHidden]
	public sealed class EPEmployeeSimple : EPEmployee
	{
		#region BAccountID

		public new abstract class bAccountID : IBqlField { }

		#endregion

		#region DefContactID

		public new abstract class defContactID : IBqlField { }

		#endregion

		#region UserID

		public new abstract class userID : IBqlField { }

		#endregion
	}
    

	#region ApproverEmployee

	[Serializable]
    [PXHidden]
	public partial class ApproverEmployee : EPEmployee
	{
		#region AcctCD

		public new abstract class acctCD : IBqlField { }

		[PXDBString(30, IsUnicode = true, IsKey = true, InputMask = "")]
		[PXUIField(DisplayName = "Approver")]
		public override string AcctCD
		{
			get
			{
				return base.AcctCD;
			}
			set
			{
				base.AcctCD = value;
			}
		}

		#endregion

		#region AcctName

		public new abstract class acctName : IBqlField { }

		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Approver Name")]
		public override string AcctName
		{
			get
			{
				return base.AcctName;
			}
			set
			{
				base.AcctName = value;
			}
		}

		#endregion

		#region UserId

		public new abstract class userID : IBqlField { }

		#endregion
	}

	#endregion

	#region ApprovedByEmployee

	[Serializable]
    [PXHidden]
	public partial class ApprovedByEmployee : EPEmployee
	{
		#region AcctCD

		public new abstract class acctCD : IBqlField { }

		[PXDBString(30, IsUnicode = true, IsKey = true, InputMask = "")]
		[PXUIField(DisplayName = "Approved By")]
		public override string AcctCD
		{
			get
			{
				return base.AcctCD;
			}
			set
			{
				base.AcctCD = value;
			}
		}

		#endregion

		#region AcctName

		public new abstract class acctName : IBqlField { }

		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Approved By Name")]
		public override string AcctName
		{
			get
			{
				return base.AcctName;
			}
			set
			{
				base.AcctName = value;
			}
		}

		#endregion

		#region UserId

		public new abstract class userID : IBqlField { }

		#endregion
	}

	#endregion
}

namespace PX.Objects.EP.Simple
{
    [System.SerializableAttribute()]
    [PXHidden]
    public partial class EPEmployee : IBqlTable
    {
        #region BAccountID
        public abstract class bAccountID : PX.Data.IBqlField
        {
        }
        protected Int32? _BAccountID;
        [PXDBIdentity(IsKey = true)]
        [PXUIField(Visible = false, Visibility = PXUIVisibility.Invisible)]
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

        #region UserID
        public abstract class userID : IBqlField { }
        [PXDBGuid]
        [PXUIField(DisplayName = "Employee Login", Visibility = PXUIVisibility.Visible)]
        public virtual Guid? UserID { get; set; }
        #endregion
    }
	
}
