namespace PX.Objects.RQ
{
	using System;
	using PX.Data;
	using PX.Objects.GL;
	using PX.Objects.AP;
	using PX.Objects.IN;
	using PX.Objects.CM;
	using PX.Objects.TX;
	using PX.Objects.CR;
	using PX.Objects.CS;
using PX.Objects.EP;

	[System.SerializableAttribute()]
    [PXCacheName(Messages.RQRequestLine)]
	public partial class RQRequestLine : PX.Data.IBqlTable
	{
		#region Selected
		public abstract class selected : PX.Data.IBqlField
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
		[Branch(typeof(RQRequest.branchID))]
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
		#region OrderNbr
		public abstract class orderNbr : PX.Data.IBqlField
		{
		}
		protected String _OrderNbr;

		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "")]
		[PXDBDefault(typeof(RQRequest.orderNbr), DefaultForUpdate=false)]
		[PXParent(typeof(Select<RQRequest, Where<RQRequest.orderNbr, Equal<Current<RQRequestLine.orderNbr>>>>))]
		[PXUIField(DisplayName = "Ref. Nbr.", Visibility = PXUIVisibility.Invisible, Visible = false)]
		public virtual String OrderNbr
		{
			get
			{
				return this._OrderNbr;
			}
			set
			{
				this._OrderNbr = value;
			}
		}
		#endregion
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected int? _LineNbr;
		[PXDBInt(IsKey = true)]
		[PXUIField(DisplayName = "Line Nbr.", Visible = false)]
		[PXLineNbr(typeof(RQRequest.lineCntr))]
		public virtual int? LineNbr
		{
			get
			{
				return this._LineNbr;
			}
			set
			{
				this._LineNbr = value;
			}
		}
		#endregion		
		#region DepartmentID
		public abstract class departmentID : PX.Data.IBqlField
		{
		}
		protected String _DepartmentID;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault(typeof(RQRequest.departmentID), PersistingCheck = PXPersistingCheck.Nothing)]
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

		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[RQRequestInventoryItem(typeof(RQRequest.reqClassID), Filterable = true)]
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
		#region SubItemID
		public abstract class subItemID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubItemID;
		[PXDefault(typeof(Search<InventoryItem.defaultSubItemID,
			Where<InventoryItem.inventoryID, Equal<Current<RQRequestLine.inventoryID>>,
			And<InventoryItem.defaultSubItemOnEntry, Equal<boolTrue>>>>),
			PersistingCheck = PXPersistingCheck.Nothing)]
		[SubItem(typeof(RQRequestLine.inventoryID))]
		public virtual Int32? SubItemID
		{
			get
			{
				return this._SubItemID;
			}
			set
			{
				this._SubItemID = value;
			}
		}
		#endregion
		
		
		#region Description
		public abstract class description : PX.Data.IBqlField
		{
		}
		protected String _Description;
		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(typeof(Search<InventoryItem.descr, Where<InventoryItem.inventoryID, Equal<Current<RQRequestLine.inventoryID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
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

		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[VendorNonEmployeeActive(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true)]
		[PXDefault(
			typeof(
			Search2<Vendor.bAccountID,				
				LeftJoin<InventoryItem, 
				      On<InventoryItem.preferredVendorID, Equal<Vendor.bAccountID>, 
						And<InventoryItem.inventoryID, Equal<Current<RQRequestLine.inventoryID>>>>>,
			Where2<
					Where<Current<RQRequest.vendorID>, IsNotNull, And<Vendor.bAccountID, Equal<Current<RQRequest.vendorID>>>>,
			Or<
					Where<Current<RQRequest.vendorID>, IsNull, And<InventoryItem.preferredVendorID, IsNotNull>>>>>), 
			PersistingCheck=PXPersistingCheck.Nothing)]
		public virtual Int32? VendorID
		{
			get
			{
				return this._VendorID;
			}
			set
			{
				this._VendorID = value;
			}
		}
		#endregion
		#region VendorLocationID
		public abstract class vendorLocationID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorLocationID;

		[LocationID(typeof(Where<Location.bAccountID, Equal<Current<RQRequestLine.vendorID>>>), DescriptionField = typeof(Location.descr), Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(typeof(Search<Vendor.defLocationID, Where<Vendor.bAccountID, Equal<Current<RQRequestLine.vendorID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Int32? VendorLocationID
		{
			get
			{
				return this._VendorLocationID;
			}
			set
			{
				this._VendorLocationID = value;
			}
		}
		#endregion
		#region VendorName
		public abstract class vendorName : PX.Data.IBqlField
		{
		}
		protected String _VendorName;
		[PXString(50, IsUnicode = true)]
		[PXDBScalar(typeof(Search<Vendor.acctName, Where<Vendor.bAccountID, Equal<RQRequestLine.vendorID>>>))]
		[PXDefault(typeof(Search<Vendor.acctName, Where<Vendor.bAccountID, Equal<Current<RQRequestLine.vendorID>>>>), PersistingCheck=PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Vendor Name", Enabled=false)]
		public virtual String VendorName
		{
			get
			{
				return this._VendorName;
			}
			set
			{
				this._VendorName = value;
			}
		}
		#endregion
		#region VendorRefNbr
		public abstract class vendorRefNbr : PX.Data.IBqlField
		{
		}
		protected String _VendorRefNbr;
		[PXDBString(40, IsUnicode = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Vendor Ref.")]
		public virtual String VendorRefNbr
		{
			get
			{
				return this._VendorRefNbr;
			}
			set
			{
				this._VendorRefNbr = value;
			}
		}
		#endregion
		#region VendorDescription
		public abstract class vendorDescription : PX.Data.IBqlField
		{
		}
		protected String _VendorDescription;
		[PXDBString(100, IsUnicode = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Vendor Description")]
		public virtual String VendorDescription
		{
			get
			{
				return this._VendorDescription;
			}
			set
			{
				this._VendorDescription = value;
			}
		}
		#endregion
		#region AlternateID
		public abstract class alternateID : PX.Data.IBqlField
		{
		}
		protected String _AlternateID;
		[AlternativeItem(INPrimaryAlternateType.VPN, typeof(RQRequestLine.vendorID), typeof(RQRequestLine.inventoryID), typeof(RQRequestLine.subItemID))]
		public virtual String AlternateID
		{
			get
			{
				return this._AlternateID;
			}
			set
			{
				this._AlternateID = value;
			}
		}
		#endregion
		
		#region UOM
		public abstract class uOM : PX.Data.IBqlField
		{
		}
		protected String _UOM;
		[PXDefault(typeof(Search<InventoryItem.purchaseUnit, Where<InventoryItem.inventoryID, Equal<Current<RQRequestLine.inventoryID>>>>))]
		[INUnit(typeof(RQRequestLine.inventoryID), DisplayName = "UOM")]
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
		#region OrderQty
		public abstract class orderQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _OrderQty;
		[PXDBQuantity(typeof(RQRequestLine.uOM), typeof(RQRequestLine.baseOrderQty), HandleEmptyKey = true)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Order Qty.", Visibility = PXUIVisibility.Visible)]
		[PXFormula(null, typeof(AddCalc<RQRequestLine.openQty>))]
		public virtual Decimal? OrderQty
		{
			get
			{
				return this._OrderQty;
			}
			set
			{
				this._OrderQty = value;
			}
		}
		#endregion
		#region BaseOrderQty
		public abstract class baseOrderQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseOrderQty;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? BaseOrderQty
		{
			get
			{
				return this._BaseOrderQty;
			}
			set
			{
				this._BaseOrderQty = value;
			}
		}
		#endregion
		#region OriginQty
		public abstract class originQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _OriginQty;
		[PXDBQuantity(typeof(RQRequestLine.uOM), typeof(RQRequestLine.baseOriginQty), HandleEmptyKey = true)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Original Qty.", Enabled = false)]
		public virtual Decimal? OriginQty
		{
			get
			{
				return this._OriginQty;
			}
			set
			{
				this._OriginQty = value;
			}
		}
		#endregion
		#region BaseOriginQty
		public abstract class baseOriginQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseOriginQty;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? BaseOriginQty
		{
			get
			{
				return this._BaseOriginQty;
			}
			set
			{
				this._BaseOriginQty = value;
			}
		}
		#endregion

		#region IssuedQty
		public abstract class issuedQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _IssuedQty;
		[PXDBQuantity(typeof(RQRequestLine.uOM), typeof(RQRequestLine.baseIssuedQty), HandleEmptyKey = true)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Issued Qty.", Enabled = false)]
		[PXFormula(null, typeof(SubCalc<RQRequestLine.openQty>))]
		public virtual Decimal? IssuedQty
		{
			get
			{
				return this._IssuedQty;
			}
			set
			{
				this._IssuedQty = value;
			}
		}
		#endregion
		#region BaseIssuedQty
		public abstract class baseIssuedQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseIssuedQty;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? BaseIssuedQty
		{
			get
			{
				return this._BaseIssuedQty;
			}
			set
			{
				this._BaseIssuedQty = value;
			}
		}
		#endregion
		#region ReqQty
		public abstract class reqQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _ReqQty;
		[PXDBQuantity(typeof(RQRequestLine.uOM), typeof(RQRequestLine.baseReqQty), HandleEmptyKey = true)]		
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Requisition Qty.", Enabled = false)]
		public virtual Decimal? ReqQty
		{
			get
			{
				return this._ReqQty;
			}
			set
			{
				this._ReqQty = value;
			}
		}
		#endregion
		#region BaseReqQty
		public abstract class baseReqQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseReqQty;
		[PXDBDecimal(6)]
		public virtual Decimal? BaseReqQty
		{
			get
			{
				return this._BaseReqQty;
			}
			set
			{
				this._BaseReqQty = value;
			}
		}
		#endregion	
		#region OpenQty
		public abstract class openQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _OpenQty;
		[PXDBQuantity(typeof(RQRequestLine.uOM), typeof(RQRequestLine.baseOpenQty), HandleEmptyKey = true)]		
		[PXFormula(typeof(RQRequestLine.orderQty), typeof(SumCalc<RQRequest.openOrderQty>))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Open Qty.", Enabled = false)]
		public virtual Decimal? OpenQty
		{
			get
			{
				return this._OpenQty;
			}
			set
			{
				this._OpenQty = value;
			}
		}
		#endregion
		#region BaseOpenQty
		public abstract class baseOpenQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseOpenQty;
		[PXDBDecimal(6)]
		public virtual Decimal? BaseOpenQty
		{
			get
			{
				return this._BaseOpenQty;
			}
			set
			{
				this._BaseOpenQty = value;
			}
		}
		#endregion	

		#region IssueStatus
		public abstract class issueStatus : IBqlField
		{
		}
		protected string _IssueStatus;
		[PXString]
		[PXUIField(DisplayName = "Issue Status", Visibility = PXUIVisibility.Visible, Enabled=false)]
		[RQRequestIssueType.ListAttribute]
		public string IssueStatus
		{
			get
			{
				return _IssueStatus;
			}
			set
			{
				_IssueStatus = value; 
			}
		}
		#endregion

		#region CuryInfoID
		public abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _CuryInfoID;

		[PXDBLong()]
		[CurrencyInfo(typeof(RQRequest.curyInfoID))]
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
		#region CuryEstUnitCost
		public abstract class curyEstUnitCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryEstUnitCost;

		[PXDBCurrency(typeof(Search<CommonSetup.decPlPrcCst>), typeof(RQRequestLine.curyInfoID), typeof(RQRequestLine.estUnitCost))]
		[PXUIField(DisplayName = "Est. Unit Cost", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(TypeCode.Decimal, "0.0")]		
		public virtual Decimal? CuryEstUnitCost
		{
			get
			{
				return this._CuryEstUnitCost;
			}
			set
			{
				this._CuryEstUnitCost = value;
			}
		}
		#endregion
		#region EstUnitCost
		public abstract class estUnitCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _EstUnitCost;

		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0", typeof(Search<INItemCost.lastCost, Where<INItemCost.inventoryID, Equal<Current<RQRequestLine.inventoryID>>>>))]
		[PXUIField(DisplayName = "Est. Unit Cost")]
		public virtual Decimal? EstUnitCost
		{
			get
			{
				return this._EstUnitCost;
			}
			set
			{
				this._EstUnitCost = value;
			}
		}
		#endregion

		#region CuryEstExtCost
		public abstract class curyEstExtCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryEstExtCost;
		[PXDBCurrency(typeof(RQRequestLine.curyInfoID), typeof(RQRequestLine.estExtCost))]
		[PXUIField(DisplayName = "Est. Ext. Cost", Visibility = PXUIVisibility.SelectorVisible)]
		[PXFormula(typeof(Mult<RQRequestLine.orderQty, RQRequestLine.curyEstUnitCost>))]
		[PXFormula(null, typeof(SumCalc<RQRequest.curyEstExtCostTotal>))]		
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryEstExtCost
		{
			get
			{
				return this._CuryEstExtCost;
			}
			set
			{
				this._CuryEstExtCost = value;
			}
		}
		#endregion
		#region EstExtCost
		public abstract class estExtCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _EstExtCost;
		[PXUIField(DisplayName = "Est. Ext. Cost")]
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? EstExtCost
		{
			get
			{
				return this._EstExtCost;
			}
			set
			{
				this._EstExtCost = value;
			}
		}
		#endregion
		#region ExpenseAcctID
		public abstract class expenseAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _ExpenseAcctID;
		[Account(DisplayName = "Account", Visibility = PXUIVisibility.Visible, Filterable = false, DescriptionField=typeof(Account.description), Required=false)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
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

		[SubAccount(typeof(RQRequestLine.expenseAcctID), DisplayName = "Sub.", Visibility = PXUIVisibility.Visible, Filterable = true, DescriptionField = typeof(Sub.description), Required = false)]		
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
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

		#region RequestedDate
		public abstract class requestedDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _RequestedDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Required Date")]
		public virtual DateTime? RequestedDate
		{
			get
			{
				return this._RequestedDate;
			}
			set
			{
				this._RequestedDate = value;
			}
		}
		#endregion
		#region PromisedDate
		public abstract class promisedDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _PromisedDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Promised Date")]
		public virtual DateTime? PromisedDate
		{
			get
			{
				return this._PromisedDate;
			}
			set
			{
				this._PromisedDate = value;
			}
		}
		#endregion
		#region Approved
		public abstract class approved : PX.Data.IBqlField
		{
		}
		protected Boolean? _Approved;
		[PXDBBool()]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Approved", Visibility = PXUIVisibility.Visible)]
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
		#region Cancelled
		public abstract class cancelled : PX.Data.IBqlField
		{
		}
		protected Boolean? _Cancelled;
		[PXDBBool()]
		[PXUIField(DisplayName = "Canceled", Visibility = PXUIVisibility.Visible)]
		[PXDefault(false)]
		public virtual Boolean? Cancelled
		{
			get
			{
				return this._Cancelled;
			}
			set
			{
				this._Cancelled = value;
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
		#region Updatable
		public abstract class updatable : PX.Data.IBqlField
		{
		}
		protected bool? _Updatable = false;
		[PXBool]
		[PXDefault(false)]		
		public virtual bool? Updatable
		{
			get
			{
				return _Updatable;
			}
			set
			{
				_Updatable = value;
			}
		}
		#endregion
	}
		
	public static class RQRequestIssueType
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Open, PartiallyIssued, Closed, Canceled, Ordered, Requseted, Received },
				new string[] { Messages.Open, Messages.PartiallyIssued, Messages.Closed, Messages.Canceled, Messages.Ordered, Messages.Requested, Messages.Received}) { }
		}

		public const string Open = "O";
		public const string PartiallyIssued = "P";
		public const string Closed = "I";
		public const string Canceled = "C";		
		public const string Ordered = "B";
		public const string Requseted = "R";
		public const string Received = "E";
	}	
}
