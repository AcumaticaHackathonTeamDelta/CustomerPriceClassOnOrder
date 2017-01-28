using PX.SM;

namespace PX.Objects.RQ
{
	using System;
	using PX.Data;
	using PX.Objects.AP;
	using PX.Objects.CS;
	using PX.Objects.CR;
	using PX.Objects.CM;
	using PX.Objects.IN;
	using PX.Objects.AR;
	using PX.TM;
	using PX.Objects.EP;
	using PX.Objects.PO;
	using PX.Objects.SO;
	using CRLocation = PX.Objects.CR.Standalone.Location;

	[System.SerializableAttribute()]
	[PXPrimaryGraph(typeof(RQRequisitionEntry))]
	[PXCacheName(Messages.RQRequisition)]
	[PXEMailSource]
	public partial class RQRequisition : PX.Data.IBqlTable, PX.Data.EP.IAssign
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
		#region ReqNbr
		public abstract class reqNbr : PX.Data.IBqlField
		{
		}
		protected String _ReqNbr;
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXDefault()]
		[PXUIField(DisplayName = "Ref. Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		[AutoNumber(typeof(RQSetup.requisitionNumberingID), typeof(RQRequisition.orderDate))]
		[PXSelectorAttribute(
			typeof(Search2<RQRequisition.reqNbr,
			LeftJoinSingleTable<Customer, On<Customer.bAccountID, Equal<RQRequisition.customerID>>,
			LeftJoinSingleTable<Vendor, On<Vendor.bAccountID, Equal<RQRequisition.vendorID>>>>,
			Where2<Where<Customer.bAccountID, IsNull,
								Or<Match<Customer, Current<AccessInfo.userName>>>>,
 				 And<Where<Vendor.bAccountID, IsNull,
								Or<Match<Vendor, Current<AccessInfo.userName>>>>>>>),
			typeof(RQRequisition.reqNbr),
			typeof(RQRequisition.status),
			typeof(RQRequisition.employeeID),
			typeof(RQRequisition.vendorID),		
			Filterable = true)]
		[PX.Data.EP.PXFieldDescription]
		public virtual String ReqNbr
		{
			get
			{
				return this._ReqNbr;
			}
			set
			{
				this._ReqNbr = value;
			}
		}
		#endregion		
		#region OrderDate
		public abstract class orderDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _OrderDate;

		[PXDBDate()]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Date", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual DateTime? OrderDate
		{
			get
			{
				return this._OrderDate;
			}
			set
			{
				this._OrderDate = value;
			}
		}
		#endregion				
		#region Priority
		public abstract class priority : PX.Data.IBqlField
		{
		}
		protected Int32? _Priority;
		[PXDBInt]
		[PXUIField]
		[PXDefault(1)]
		[PXIntList(new int[] { 0, 1, 2 },
			new string[] { "Low", "Normal", "High" })]
		public virtual Int32? Priority
		{
			get
			{
				return this._Priority;
			}
			set
			{
				this._Priority = value;
			}
		}
		#endregion
		#region Status
		public abstract class status : PX.Data.IBqlField
		{
		}
		protected String _Status;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(RQRequestStatus.Hold)]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[RQRequisitionStatus.List()]
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
		#region Description
		public abstract class description : PX.Data.IBqlField
		{
		}
		protected String _Description;
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
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

		#region Hold
		public abstract class hold : PX.Data.IBqlField
		{
		}
		protected Boolean? _Hold;
		[PXDBBool()]
		[PXUIField(DisplayName = "Hold", Visibility = PXUIVisibility.Visible)]
		[PXDefault(true)]
		[PXNoUpdate]
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
		#region Approved
		public abstract class approved : PX.Data.IBqlField
		{
		}
		protected Boolean? _Approved;
		[PXDBBool()]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Approved", Visibility = PXUIVisibility.Visible, Enabled = false)]
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
		protected bool? _Rejected = false;
		[PXBool]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Reject", Visibility = PXUIVisibility.Visible)]
		public bool? Rejected
		{
			get
			{
				return _Rejected;
			}
			set
			{
				_Rejected = value;
			}
		}
		#endregion
		#region Cancelled
		public abstract class cancelled : PX.Data.IBqlField
		{
		}
		protected Boolean? _Cancelled;
		[PXDBBool()]
		[PXUIField(DisplayName = "Cancel", Visibility = PXUIVisibility.Visible)]
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
		#region Splittable
		public abstract class splittable : PX.Data.IBqlField
		{
		}
		protected Boolean? _Splittable;
		[PXDBBool()]
		[PXDefault(true, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Splittable", Visibility = PXUIVisibility.Visible)]
		public virtual Boolean? Splittable
		{
			get
			{
				return this._Splittable;
			}
			set
			{
				this._Splittable = value;
			}
		}
		#endregion
		#region Released
		public abstract class released : PX.Data.IBqlField
		{
		}
		protected Boolean? _Released;
		[PXDBBool()]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Released", Visibility = PXUIVisibility.Visible, Enabled = false)]
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
		#region BiddingComplete
		public abstract class biddingComplete : PX.Data.IBqlField
		{
		}
		protected Boolean? _BiddingComplete;
		[PXDBBool()]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Complete Bidding", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public virtual Boolean? BiddingComplete
		{
			get
			{
				return this._BiddingComplete;
			}
			set
			{
				this._BiddingComplete = value;
			}
		}
		#endregion
		#region Quoted
		public abstract class quoted : PX.Data.IBqlField
		{
		}
		protected Boolean? _Quoted;
		[PXDBBool()]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Quoted", Visibility = PXUIVisibility.Visible)]
		public virtual Boolean? Quoted
		{
			get
			{
				return this._Quoted;
			}
			set
			{
				this._Quoted = value;
			}
		}
		#endregion
	

		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXSearchable(SM.SearchCategory.RQ, Messages.SearchableTitleRequisition, new Type[] { typeof(RQRequisition.reqNbr), typeof(RQRequisition.employeeID), typeof(EPEmployee.acctName) },
		   new Type[] { typeof(RQRequisition.vendorRefNbr), typeof(RQRequisition.description) },
		   NumberFields = new Type[] { typeof(RQRequisition.reqNbr) },
		   Line1Format = "{0:d}{1}", Line1Fields = new Type[] { typeof(RQRequisition.orderDate), typeof(RQRequisition.status) },
		   Line2Format = "{0}", Line2Fields = new Type[] { typeof(RQRequisition.description) }
		)]
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
		#region EmployeeID
		public abstract class employeeID : PX.Data.IBqlField
		{
		}
		protected Int32? _EmployeeID;
		[PXDBInt()]
		[PXDefault(typeof(Search<EPEmployee.bAccountID, Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>))]
		[PXSubordinateSelector]
		[PXUIField(DisplayName = "Creator", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region CustomerID
		public abstract class customerID : PX.Data.IBqlField
		{
		}
		protected Int32? _CustomerID;
        [CustomerActive(typeof(Search<BAccountR.bAccountID, Where<Customer.type, IsNotNull,
                                                            Or<Current<SOOrder.aRDocType>, Equal<ARDocType.noUpdate>,
                                                            And<BAccountR.type, Equal<BAccountType.companyType>>>>>), 
                        Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Customer.acctName), Filterable = true)]
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
		[PXDefault(typeof(Search<Customer.defLocationID, Where<Customer.bAccountID, Equal<Current<RQRequisition.customerID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[LocationID(typeof(Where<Location.bAccountID, Equal<Current<RQRequisition.customerID>>>), DescriptionField = typeof(Location.descr))]
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
		#region WorkgroupID
		public abstract class workgroupID : PX.Data.IBqlField
		{
		}
		protected int? _WorkgroupID;
		[PXDBInt]
		[PXUIField(DisplayName = "Workgroup", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSubordinateGroupSelectorAttribute]
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
		#region OwnerID
		public abstract class ownerID : IBqlField
		{
		}
		protected Guid? _OwnerID;
		[PXDBGuid()]
		[PXDefault(typeof(Vendor.ownerID), PersistingCheck = PXPersistingCheck.Nothing)]
		[PX.TM.PXOwnerSelector(typeof(RQRequisition.workgroupID))]
		[PXUIField(DisplayName = "Owner", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Guid? OwnerID
		{
			get
			{
				return this._OwnerID;
			}
			set
			{
				this._OwnerID = value;
			}
		}
        #endregion

        #region ApprovalWorkgroupID
        public abstract class approvalWorkgroupID : PX.Data.IBqlField
        {
        }
        protected int? _ApprovalWorkgroupID;
        [PXInt]
        [PXSelector(typeof(Search<EPCompanyTree.workGroupID>), SubstituteKey = typeof(EPCompanyTree.description))]
        [PXUIField(DisplayName = "Approval Workgroup ID", Enabled = false)]
        public virtual int? ApprovalWorkgroupID
        {
            get
            {
                return this._ApprovalWorkgroupID;
            }
            set
            {
                this._ApprovalWorkgroupID = value;
            }
        }
        #endregion
        #region ApprovalOwnerID
        public abstract class approvalOwnerID : IBqlField
        {
        }
        protected Guid? _ApprovalOwnerID;
        [PXGuid()]
        [PX.TM.PXOwnerSelector]
        [PXUIField(DisplayName = "Approver", Enabled = false)]
        public virtual Guid? ApprovalOwnerID
        {
            get
            {
                return this._ApprovalOwnerID;
            }
            set
            {
                this._ApprovalOwnerID = value;
            }
        }
        #endregion
        #region ShipDestType
        public abstract class shipDestType : PX.Data.IBqlField
		{
		}
		protected String _ShipDestType;
		[PXDBString(1, IsFixed = true)]
		[POShippingDestination.ListSimple()]
		[PXDefault(POShippingDestination.CompanyLocation)]
		[PXUIField(DisplayName = "Shipping Destination Type")]
		public virtual String ShipDestType
		{
			get
			{
				return this._ShipDestType;
			}
			set
			{
				this._ShipDestType = value;
			}
		}
		#endregion
		#region ShipToBAccountID
		public abstract class shipToBAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _ShipToBAccountID;
		[PXDBInt()]
		[PXSelector(typeof(Search2<BAccount2.bAccountID,
			LeftJoin<Vendor,
						On<Vendor.bAccountID, Equal<BAccount2.bAccountID>,
					 And<Vendor.type, NotEqual<BAccountType.employeeType>>>,
			LeftJoin<AR.Customer, On<AR.Customer.bAccountID, Equal<BAccount2.bAccountID>>,
			LeftJoin<GL.Branch, On<GL.Branch.bAccountID, Equal<BAccount2.bAccountID>>>>>,
			 Where<Vendor.bAccountID, IsNotNull, And<Optional<RQRequisition.shipDestType>, Equal<POShippingDestination.vendor>,
			Or<Where<GL.Branch.bAccountID, IsNotNull, And<Optional<RQRequisition.shipDestType>, Equal<POShippingDestination.company>,
			Or<Where<AR.Customer.bAccountID, IsNotNull, And<Optional<RQRequisition.shipDestType>, Equal<POShippingDestination.customer>>>
				>>>>>>>),
				typeof(BAccount.acctCD), typeof(BAccount.acctName), typeof(BAccount.type), typeof(BAccount.acctReferenceNbr), typeof(BAccount.parentBAccountID),
			SubstituteKey = typeof(BAccount.acctCD), DescriptionField = typeof(BAccount.acctName))]
		[PXUIField(DisplayName = "Ship To")]
		[PXDefault((object)null, typeof(Search<GL.Branch.bAccountID, Where<GL.Branch.branchID, Equal<Current<AccessInfo.branchID>>, And<Optional<RQRequisition.shipDestType>, Equal<POShippingDestination.company>>>>))]
		public virtual Int32? ShipToBAccountID
		{
			get
			{
				return this._ShipToBAccountID;
			}
			set
			{
				this._ShipToBAccountID = value;
			}
		}
		#endregion
		#region ShipToLocationID
		public abstract class shipToLocationID : PX.Data.IBqlField
		{
		}
		protected Int32? _ShipToLocationID;

		[LocationID(typeof(Where<Location.bAccountID, Equal<Current<RQRequisition.shipToBAccountID>>>), DescriptionField = typeof(Location.descr))]
		[PXDefault((object)null, typeof(Search<BAccount2.defLocationID, Where<BAccount2.bAccountID, Equal<Current<RQRequisition.shipToBAccountID>>>>))]
		[PXUIField(DisplayName = "Shipping Location")]
		public virtual Int32? ShipToLocationID
		{
			get
			{
				return this._ShipToLocationID;
			}
			set
			{
				this._ShipToLocationID = value;
			}
		}
		#endregion
		#region ShipAddressID
		public abstract class shipAddressID : PX.Data.IBqlField
		{
		}
		protected Int32? _ShipAddressID;
		[PXDBInt()]
		[POShipAddress(typeof(Select2<Address,
					LeftJoin<CRLocation, On<Address.bAccountID, Equal<CRLocation.bAccountID>,
						And<Address.addressID, Equal<CRLocation.defAddressID>,
						And<CRLocation.bAccountID, Equal<Current<RQRequisition.shipToBAccountID>>,
						And<CRLocation.locationID, Equal<Current<RQRequisition.shipToLocationID>>>>>>,
					LeftJoin<POShipAddress, On<POShipAddress.bAccountID, Equal<Address.bAccountID>,
						And<POShipAddress.bAccountAddressID, Equal<Address.addressID>,
						And<POShipAddress.revisionID, Equal<Address.revisionID>,
						And<POShipAddress.isDefaultAddress, Equal<boolTrue>>>>>>>,
					Where<CRLocation.locationCD, IsNotNull>>))]
		[PXUIField()]
		public virtual Int32? ShipAddressID
		{
			get
			{
				return this._ShipAddressID;
			}
			set
			{
				this._ShipAddressID = value;
			}
		}
		#endregion
		#region ShipContactID
		public abstract class shipContactID : PX.Data.IBqlField
		{
		}
		protected Int32? _ShipContactID;
		[PXDBInt()]
		[POShipContactAttribute(typeof(Select2<Contact,
					LeftJoin<CRLocation, On<Contact.bAccountID, Equal<CRLocation.bAccountID>,
						And<Contact.contactID, Equal<CRLocation.defContactID>,
						And<CRLocation.bAccountID, Equal<Current<RQRequisition.shipToBAccountID>>,
						And<CRLocation.locationID, Equal<Current<RQRequisition.shipToLocationID>>>>>>,
					LeftJoin<POShipContact, On<POShipContact.bAccountID, Equal<Contact.bAccountID>,
						And<POShipContact.bAccountContactID, Equal<Contact.contactID>,
						And<POShipContact.revisionID, Equal<Contact.revisionID>,
						And<POShipContact.isDefaultContact, Equal<boolTrue>>>>>>>,
					Where<CRLocation.locationCD, IsNotNull>>))]
		[PXUIField()]
		public virtual Int32? ShipContactID
		{
			get
			{
				return this._ShipContactID;
			}
			set
			{
				this._ShipContactID = value;
			}
		}
		#endregion
		#region FOBPoint
		public abstract class fOBPoint : PX.Data.IBqlField
		{
		}
		protected String _FOBPoint;
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "FOB Point")]
		[PXSelector(typeof(Search<FOBPoint.fOBPointID>), DescriptionField = typeof(FOBPoint.description), CacheGlobal = true)]
		[PXDefault(typeof(Search<Location.vFOBPointID,
								 Where<Location.bAccountID, Equal<Current<RQRequisition.vendorID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual String FOBPoint
		{
			get
			{
				return this._FOBPoint;
			}
			set
			{
				this._FOBPoint = value;
			}
		}
		#endregion
		#region ShipVia
		public abstract class shipVia : PX.Data.IBqlField
		{
		}
		protected String _ShipVia;
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Ship Via")]
		[PXSelector(typeof(Search<Carrier.carrierID>), CacheGlobal = true)]
		[PXDefault(typeof(Search<Location.vCarrierID,
							Where<Location.bAccountID, Equal<Current<RQRequisition.vendorID>>,
										And<Location.locationID, Equal<Current<RQRequisition.vendorLocationID>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual String ShipVia
		{
			get
			{
				return this._ShipVia;
			}
			set
			{
				this._ShipVia = value;
			}
		}
		#endregion

		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[VendorNonEmployeeActive(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true)]
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
		[LocationID(typeof(Where<Location.bAccountID, Equal<Current<RQRequisition.vendorID>>>), DescriptionField = typeof(Location.descr), Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(typeof(Search<Vendor.defLocationID, Where<Vendor.bAccountID, Equal<Current<RQRequisition.vendorID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
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
		#region VendorRequestSent
		public abstract class vendorRequestSent : PX.Data.IBqlField
		{
		}
		protected bool? _VendorRequestSent = false;
		[PXBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Vendor Requests Sent")]
		public virtual bool? VendorRequestSent
		{
			get
			{
				return _VendorRequestSent;
			}
			set
			{
				_VendorRequestSent = value;
			}
		}
		#endregion		

		#region TermsID
		public abstract class termsID : PX.Data.IBqlField
		{
		}
		protected String _TermsID;
		[PXDBString(10, IsUnicode = true, IsFixed = true)]
		[PXDefault(typeof(Search<Vendor.termsID, Where<Vendor.bAccountID, Equal<Current<RQRequisition.vendorID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Terms", Visibility = PXUIVisibility.Visible)]
		[PXSelector(typeof(Search<Terms.termsID, Where<Terms.visibleTo, Equal<TermsVisibleTo.all>, Or<Terms.visibleTo, Equal<TermsVisibleTo.vendor>>>>), DescriptionField = typeof(Terms.descr), Filterable = true)]

		public virtual String TermsID
		{
			get
			{
				return this._TermsID;
			}
			set
			{
				this._TermsID = value;
			}
		}
		#endregion
		#region RemitAddressID
		public abstract class remitAddressID : PX.Data.IBqlField
		{
		}
		protected Int32? _RemitAddressID;
		[PXDBInt()]
		[PORemitAddress(typeof(Select2<BAccount2,
			InnerJoin<Location, On<Location.bAccountID, Equal<BAccount2.bAccountID>>,
			InnerJoin<Address, On<Address.bAccountID, Equal<Location.bAccountID>, And<Address.addressID, Equal<Location.defAddressID>>>,
			LeftJoin<PORemitAddress, On<PORemitAddress.bAccountID, Equal<Address.bAccountID>,
						And<PORemitAddress.bAccountAddressID, Equal<Address.addressID>,
				And<PORemitAddress.revisionID, Equal<Address.revisionID>, And<PORemitAddress.isDefaultAddress, Equal<boolTrue>>>>>>>>,
			Where<Location.bAccountID, Equal<Current<RQRequisition.vendorID>>, And<Location.locationID, Equal<Current<RQRequisition.vendorLocationID>>>>>), Required = false)]
		[PXUIField()]
		public virtual Int32? RemitAddressID
		{
			get
			{
				return this._RemitAddressID;
			}
			set
			{
				this._RemitAddressID = value;
			}
		}
		#endregion
		#region RemitContactID
		public abstract class remitContactID : PX.Data.IBqlField
		{
		}
		protected Int32? _RemitContactID;
		[PXDBInt()]
		[PORemitContactAttribute(typeof(Select2<Location,
				InnerJoin<Contact, On<Contact.bAccountID, Equal<Location.bAccountID>, And<Contact.contactID, Equal<Location.defContactID>>>,
				LeftJoin<PORemitContact, On<PORemitContact.bAccountID, Equal<Contact.bAccountID>,
				And<PORemitContact.bAccountContactID, Equal<Contact.contactID>,
						And<PORemitContact.revisionID, Equal<Contact.revisionID>,
				And<PORemitContact.isDefaultContact, Equal<boolTrue>>>>>>>,
				Where<Location.bAccountID, Equal<Current<RQRequisition.vendorID>>, And<Location.locationID, Equal<Current<RQRequisition.vendorLocationID>>>>>), Required = false)]
		public virtual Int32? RemitContactID
		{
			get
			{
				return this._RemitContactID;
			}
			set
			{
				this._RemitContactID = value;
			}
		}
		#endregion

		#region OpenOrderQty
		public abstract class openOrderQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _OpenOrderQty;
		[PXDBQuantity()]
		[PXUIField(DisplayName = "Open Quantity")]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? OpenOrderQty
		{
			get
			{
				return this._OpenOrderQty;
			}
			set
			{
				this._OpenOrderQty = value;
			}
		}
		#endregion

		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL")]
		[PXUIField(DisplayName = "Currency", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(typeof(Search<GL.Company.baseCuryID>))]
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
		[CurrencyInfo(ModuleCode = "PO")]
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
		
		#region EstExtCostTotal
		public abstract class estExtCostTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _EstExtCostTotal;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? EstExtCostTotal
		{
			get
			{
				return this._EstExtCostTotal;
			}
			set
			{
				this._EstExtCostTotal = value;
			}
		}
		#endregion
		#region CuryEstExtCostTotal
		public abstract class curyEstExtCostTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryEstExtCostTotal;

		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(RQRequisition.curyInfoID), typeof(RQRequisition.estExtCostTotal))]
		[PXUIField(DisplayName = "Est. Ext. Cost", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual Decimal? CuryEstExtCostTotal
		{
			get
			{
				return this._CuryEstExtCostTotal;
			}
			set
			{
				this._CuryEstExtCostTotal = value;
			}
		}
		#endregion
		#region POType
		public abstract class pOType : PX.Data.IBqlField
		{
		}
		protected String _POType;		
		[PXDBString(2, IsFixed = true)]
		[PXDefault(POOrderType.RegularOrder)]		
		[POOrderType.List]
		[PXUIField(DisplayName = "PO Type", Enabled = true)]		 
		public virtual String POType
		{
			get
			{
				return this._POType;
			}
			set
			{
				this._POType = value;
			}
		}
		#endregion
		#region LineCntr
		public abstract class lineCntr : PX.Data.IBqlField
		{
		}
		protected int? _LineCntr;
		[PXDBInt()]
		[PXDefault(0)]
		public virtual int? LineCntr
		{
			get
			{
				return this._LineCntr;
			}
			set
			{
				this._LineCntr = value;
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
        #region IAssign Members
        int? PX.Data.EP.IAssign.WorkgroupID
        {
            get { return ApprovalWorkgroupID; }
            set { ApprovalWorkgroupID = value; }
        }

        Guid? PX.Data.EP.IAssign.OwnerID
        {
            get { return ApprovalOwnerID; }
            set { ApprovalOwnerID = value; }
        }
        #endregion
    }

    public class RQRequisitionStatus
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Hold, Open, PendingApproval, Canceled, Closed, Bidding, PendingQuotation, Rejected, Released },
				new string[] { Messages.Hold, Messages.Open, Messages.PendingApproval, Messages.Canceled, Messages.Closed, Messages.Bidding, Messages.PendingQuotation, Messages.Rejected, Messages.Released }) { ; }
		}

		public const string Hold = "H";
		public const string PendingApproval = "P";
		public const string Rejected = "R";
		public const string Open = "N";
		public const string Closed = "C";
		public const string Issued = "I";
		public const string Canceled = "L";
		public const string Bidding = "B";
		public const string Released = "E";
		public const string PendingQuotation = "Q";


		public class hold : Constant<string>
		{
			public hold() : base(Hold) { ;}
		}
		public class pendingApproval : Constant<string>
		{
			public pendingApproval() : base(PendingApproval) { ;}
		}
		public class rejected : Constant<string>
		{
			public rejected() : base(Rejected) { ;}
		}
		public class released : Constant<string>
		{
			public released() : base(Released) { ;}
		}
		public class open : Constant<string>
		{
			public open() : base(Open) { ;}
		}
		public class closed : Constant<string>
		{
			public closed() : base(Closed) { ;}
		}
		public class canceled : Constant<string>
		{
			public canceled() : base(Canceled) { ;}
		}
		public class bidding : Constant<string>
		{
			public bidding() : base(Bidding) { ;}
		}
		public class pendingQuotation : Constant<string>
		{
			public pendingQuotation() : base(PendingQuotation) { ;}
		}

	}
}
