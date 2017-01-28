namespace PX.Objects.SO
{
	using System;
	using PX.Data;
	using PX.Objects.CS;
	using PX.Objects.IN;
	using PX.Objects.AR;
	using PX.Objects.CR;
	using PX.Objects.CM;
	using POReceipt = PX.Objects.PO.POReceipt;
	using CRLocation = PX.Objects.CR.Standalone.Location;
	
	[System.SerializableAttribute()]
	[PXPrimaryGraph(typeof(SOShipmentEntry))]
	[PXCacheName(Messages.SOShipment)]
	[PXEMailSource]
	public partial class SOShipment : PX.Data.IBqlTable, PX.Data.EP.IAssign, IFreightBase 
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
		#region ShipmentNbr
		public abstract class shipmentNbr : PX.Data.IBqlField
		{
			public const string DisplayName = "Shipment Nbr.";
		}
		protected String _ShipmentNbr;
		[PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXDefault()]
		[PXUIField(DisplayName = shipmentNbr.DisplayName, Visibility = PXUIVisibility.SelectorVisible)]
		[AutoNumber(typeof(SOSetup.shipmentNumberingID), typeof(SOShipment.shipDate))]
		[PXSelector(typeof(Search2<SOShipment.shipmentNbr,
			InnerJoin<INSite, On<INSite.siteID, Equal<SOShipment.siteID>>,
			LeftJoinSingleTable<Customer, On<SOShipment.customerID, Equal<Customer.bAccountID>>>>,
			Where2<Match<INSite, Current<AccessInfo.userName>>,
			And<Where<Customer.bAccountID, IsNull, Or<Match<Customer, Current<AccessInfo.userName>>>>>>,
			OrderBy<Desc<SOShipment.shipmentNbr>>>))]
		public virtual String ShipmentNbr
		{
			get
			{
				return this._ShipmentNbr;
			}
			set
			{
				this._ShipmentNbr = value;
			}
		}
		#endregion
		#region ShipDate
		public abstract class shipDate : PX.Data.IBqlField
		{
			public const string DisplayName = "Shipment Date";
		}
		protected DateTime? _ShipDate;
		[PXDBDate()]
		[PXUIField(DisplayName = shipDate.DisplayName, Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(typeof(AccessInfo.businessDate))]
		public virtual DateTime? ShipDate
		{
			get
			{
				return this._ShipDate;
			}
			set
			{
				this._ShipDate = value;
			}
		}
		#endregion
		#region Operation
		public abstract class operation : PX.Data.IBqlField
		{
		}
		protected String _Operation;
		[PXDBString(1, IsFixed = true, InputMask = ">a")]
		[PXUIField(DisplayName = "Operation")]		
		[SOOperation.List]
		public virtual String Operation
		{
			get
			{
				return this._Operation;
			}
			set
			{
				this._Operation = value;
			}
		}
		#endregion
		#region ShipmentType
		public abstract class shipmentType : PX.Data.IBqlField
		{
		}
		protected String _ShipmentType;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(SOShipmentType.Issue)]
		[SOShipmentType.List()]
		[PXUIField(DisplayName = "Type")]
		public virtual String ShipmentType
		{
			get
			{
				return this._ShipmentType;
			}
			set
			{
				this._ShipmentType = value;
			}
		}
		#endregion
		#region CustomerID
		public abstract class customerID : PX.Data.IBqlField
		{
		}
		protected Int32? _CustomerID;
		[Customer(typeof(Search<BAccountR.bAccountID,
						Where<Customer.type, IsNotNull,
						Or<BAccountR.type, Equal<BAccountType.companyType>>>>),
			Visibility = PXUIVisibility.SelectorVisible)]
		[PXRestrictor(typeof(Where<Customer.status, IsNull, 
						Or<Customer.status, Equal<BAccount.status.active>,
						Or<Customer.status, Equal<BAccount.status.oneTime>>>>), AR.Messages.CustomerIsInStatus, typeof(Customer.status))]
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
		[LocationID(typeof(Where<Location.bAccountID, Equal<Current<SOShipment.customerID>>,
			And<Location.isActive, Equal<True>,
			And<MatchWithBranch<Location.cBranchID>>>>), DescriptionField = typeof(Location.descr), Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(typeof(Coalesce<Search2<BAccountR.defLocationID,
			InnerJoin<CRLocation, On<CRLocation.bAccountID, Equal<BAccountR.bAccountID>, And<CRLocation.locationID, Equal<BAccountR.defLocationID>>>>,
			Where<BAccountR.bAccountID, Equal<Current<SOShipment.customerID>>,
				And<CRLocation.isActive, Equal<True>,
				And<MatchWithBranch<CRLocation.cBranchID>>>>>,
			Search<CRLocation.locationID,
			Where<CRLocation.bAccountID, Equal<Current<SOShipment.customerID>>,
			And<CRLocation.isActive, Equal<True>, And<MatchWithBranch<CRLocation.cBranchID>>>>>>))]
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
		#region CustomerOrderNbr
		public abstract class customerOrderNbr : PX.Data.IBqlField
		{
		}
		protected String _CustomerOrderNbr;
		[PXString(40, IsUnicode = true)]
		[PXUIField(DisplayName = "Customer Order", Enabled = false)]
		public virtual String CustomerOrderNbr
		{
			get
			{
				return this._CustomerOrderNbr;
			}
			set
			{
				this._CustomerOrderNbr = value;
			}
		}
		#endregion
		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SiteID;
		[IN.Site(DisplayName = "Warehouse ID", DescriptionField = typeof(INSite.descr), Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(typeof(Search<CRLocation.cSiteID, Where<CRLocation.bAccountID, Equal<Current<SOShipment.customerID>>, And<CRLocation.locationID, Equal<Current<SOShipment.customerLocationID>>>>>))]
		public virtual Int32? SiteID
		{
			get
			{
				return this._SiteID;
			}
			set
			{
				this._SiteID = value;
			}
		}
		#endregion
		#region DestinationSiteID
		public abstract class destinationSiteID : PX.Data.IBqlField
		{
		}
		protected Int32? _DestinationSiteID;
		[IN.ToSite(DisplayName = "To Warehouse", DescriptionField = typeof(INSite.descr))]
		public virtual Int32? DestinationSiteID
		{
			get
			{
				return this._DestinationSiteID;
			}
			set
			{
				this._DestinationSiteID = value;
			}
		}
		#endregion
		#region ShipAddressID
		public abstract class shipAddressID : PX.Data.IBqlField
		{
		}
		protected Int32? _ShipAddressID;
		[PXDBInt()]
		[PXUIField(DisplayName = "Shipping Address")]
		[SOShipmentAddress(typeof(Select2<Address,
			InnerJoin<CRLocation, On<CRLocation.bAccountID, Equal<Address.bAccountID>,
					 And<Address.addressID, Equal<CRLocation.defAddressID>>>,
			LeftJoin<SOShipmentAddress, On<SOShipmentAddress.customerID, Equal<Address.bAccountID>, 
                And<SOShipmentAddress.customerAddressID, Equal<Address.addressID>, 
                And<SOShipmentAddress.revisionID, Equal<Address.revisionID>, 
                And<SOShipmentAddress.isDefaultAddress, Equal<True>>>>>>>,
			Where<CRLocation.bAccountID, Equal<Current<SOShipment.customerID>>,
					 And<CRLocation.locationID, Equal<Current<SOShipment.customerLocationID>>>>>))]
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
		[SOShipmentContact(typeof(Select2<Contact, 			
			InnerJoin<CRLocation, 
			      On<CRLocation.bAccountID, Equal<Contact.bAccountID>, 
			     And<Contact.contactID,Equal<CRLocation.defContactID>>>,			
			LeftJoin<SOShipmentContact, On<SOShipmentContact.customerID, Equal<Contact.bAccountID>, 
			     And<SOShipmentContact.customerContactID, Equal<Contact.contactID>, 
					 And<SOShipmentContact.revisionID, Equal<Contact.revisionID>, 
					 And<SOShipmentContact.isDefaultContact, Equal<True>>>>>>>, 
			Where<CRLocation.bAccountID, Equal<Current<SOShipment.customerID>>,
			    And<CRLocation.locationID, Equal<Current<SOShipment.customerLocationID>>>>>))]
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
		[PXSelector(typeof(Search<Carrier.carrierID>), DescriptionField = typeof(Carrier.description), CacheGlobal = true)]
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
		#region UseCustomerAccount
		public abstract class useCustomerAccount : PX.Data.IBqlField
		{
		}
		protected Boolean? _UseCustomerAccount;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Use Customer's Account")]
		public virtual Boolean? UseCustomerAccount
		{
			get
			{
				return this._UseCustomerAccount;
			}
			set
			{
				this._UseCustomerAccount = value;
			}
		}
		#endregion
		#region Resedential
		public abstract class resedential : PX.Data.IBqlField
		{
		}
		protected Boolean? _Resedential;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Residential Delivery")]
		public virtual Boolean? Resedential
		{
			get
			{
				return this._Resedential;
			}
			set
			{
				this._Resedential = value;
			}
		}
		#endregion
		#region SaturdayDelivery
		public abstract class saturdayDelivery : PX.Data.IBqlField
		{
		}
		protected Boolean? _SaturdayDelivery;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Saturday Delivery")]
		public virtual Boolean? SaturdayDelivery
		{
			get
			{
				return this._SaturdayDelivery;
			}
			set
			{
				this._SaturdayDelivery = value;
			}
		}
		#endregion
		#region Insurance
		public abstract class insurance : PX.Data.IBqlField
		{
		}
		protected Boolean? _Insurance;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Insurance")]
		public virtual Boolean? Insurance
		{
			get
			{
				return this._Insurance;
			}
			set
			{
				this._Insurance = value;
			}
		}
		#endregion
		#region GroundCollect
		public abstract class groundCollect : PX.Data.IBqlField
		{
		}
		protected Boolean? _GroundCollect;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Ground Collect")]
		public virtual Boolean? GroundCollect
		{
			get
			{
				return this._GroundCollect;
			}
			set
			{
				this._GroundCollect = value;
			}
		}
		#endregion
		#region LabelsPrinted
		public abstract class labelsPrinted : PX.Data.IBqlField
		{
		}
		protected Boolean? _LabelsPrinted;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName="Labels Printed")]
		public virtual Boolean? LabelsPrinted
		{
			get
			{
				return this._LabelsPrinted;
			}
			set
			{
				this._LabelsPrinted = value;
			}
		}
		#endregion
		#region PickListPrinted
		public abstract class pickListPrinted : PX.Data.IBqlField
		{
		}
		protected Boolean? _PickListPrinted;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Pick List Printed")]
		public virtual Boolean? PickListPrinted
		{
			get
			{
				return this._PickListPrinted;
			}
			set
			{
				this._PickListPrinted = value;
			}
		}
		#endregion
		#region ShippedViaCarrier
		public abstract class shippedViaCarrier : PX.Data.IBqlField
		{
		}
		protected Boolean? _ShippedViaCarrier;
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? ShippedViaCarrier
		{
			get
			{
				return this._ShippedViaCarrier;
			}
			set
			{
				this._ShippedViaCarrier = value;
			}
		}
		#endregion
		#region ShipTermsID
		public abstract class shipTermsID : PX.Data.IBqlField
		{
		}
		protected String _ShipTermsID;
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXUIField(DisplayName = "Shipping Terms")]
		[PXSelector(typeof(ShipTerms.shipTermsID), DescriptionField = typeof(ShipTerms.description), CacheGlobal = true)]
		public virtual String ShipTermsID
		{
			get
			{
				return this._ShipTermsID;
			}
			set
			{
				this._ShipTermsID = value;
			}
		}
		#endregion
		#region ShipZoneID
		public abstract class shipZoneID : PX.Data.IBqlField
		{
		}
		protected String _ShipZoneID;
		[PXDBString(15, IsUnicode = true, InputMask = ">aaaaaaaaaaaaaaa")]
		[PXUIField(DisplayName = "Shipping Zone ID")]
		[PXSelector(typeof(ShippingZone.zoneID), DescriptionField = typeof(ShippingZone.description), CacheGlobal = true)]
		public virtual String ShipZoneID
		{
			get
			{
				return this._ShipZoneID;
			}
			set
			{
				this._ShipZoneID = value;
			}
		}
		#endregion
		#region LineTotal
		public abstract class lineTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _LineTotal;
		[PXDBBaseCury()]
		[PXUIField(DisplayName = "Line Total")]
		[PXDefault(TypeCode.Decimal, "0.0")]
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
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL")]
		[PXUIField(DisplayName = "Freight Currency", Visibility = PXUIVisibility.SelectorVisible)]
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
		[CurrencyInfo()]
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
		#region CuryFreightCost
		public abstract class curyFreightCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryFreightCost;
		[PXDBCurrency(typeof(SOShipment.curyInfoID), typeof(SOShipment.freightCost))]
		[PXUIField(DisplayName = "Freight Cost", Enabled = false)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryFreightCost
		{
			get
			{
				return this._CuryFreightCost;
			}
			set
			{
				this._CuryFreightCost = value;
			}
		}
		#endregion
		#region FreightCost
		public abstract class freightCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _FreightCost;
		[PXDBDecimal(4)]
		public virtual Decimal? FreightCost
		{
			get
			{
				return this._FreightCost;
			}
			set
			{
				this._FreightCost = value;
			}
		}
		#endregion
		#region CuryFreightAmt
		public abstract class curyFreightAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryFreightAmt;
		[PXDBCurrency(typeof(SOShipment.curyInfoID), typeof(SOShipment.freightAmt))]
		[PXUIField(DisplayName = "Freight Amt.")]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryFreightAmt
		{
			get
			{
				return this._CuryFreightAmt;
			}
			set
			{
				this._CuryFreightAmt = value;
			}
		}
		#endregion
		#region FreightAmt
		public abstract class freightAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _FreightAmt;
		[PXDBDecimal(4)]
		public virtual Decimal? FreightAmt
		{
			get
			{
				return this._FreightAmt;
			}
			set
			{
				this._FreightAmt = value;
			}
		}
		#endregion
		#region CuryPremiumFreightAmt
		public abstract class curyPremiumFreightAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryPremiumFreightAmt;
		[PXDBCurrency(typeof(SOShipment.curyInfoID), typeof(SOShipment.premiumFreightAmt))]
		[PXUIField(DisplayName = "Premium Freight Amt.")]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryPremiumFreightAmt
		{
			get
			{
				return this._CuryPremiumFreightAmt;
			}
			set
			{
				this._CuryPremiumFreightAmt = value;
			}
		}
		#endregion
		#region PremiumFreightAmt
		public abstract class premiumFreightAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _PremiumFreightAmt;
		[PXDBDecimal(4)]
		public virtual Decimal? PremiumFreightAmt
		{
			get
			{
				return this._PremiumFreightAmt;
			}
			set
			{
				this._PremiumFreightAmt = value;
			}
		}
		#endregion
		#region CuryTotalFreightAmt
		public abstract class curyTotalFreightAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryTotalFreightAmt;
		[PXDBCurrency(typeof(SOShipment.curyInfoID), typeof(SOShipment.totalFreightAmt))]
		[PXFormula(typeof(Add<SOShipment.curyPremiumFreightAmt, SOShipment.curyFreightAmt>))]
		[PXUIField(DisplayName = "Total Freight Amt.", Enabled = false)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryTotalFreightAmt
		{
			get
			{
				return this._CuryTotalFreightAmt;
			}
			set
			{
				this._CuryTotalFreightAmt = value;
			}
		}
		#endregion
		#region TotalFreightAmt
		public abstract class totalFreightAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _TotalFreightAmt;
		[PXDBDecimal(4)]
		[PXUIField(DisplayName = "Freight Total")]
		public virtual Decimal? TotalFreightAmt
		{
			get
			{
				return this._TotalFreightAmt;
			}
			set
			{
				this._TotalFreightAmt = value;
			}
		}
		#endregion
		#region TaxCategoryID
		public abstract class taxCategoryID : PX.Data.IBqlField
		{
		}
		protected String _TaxCategoryID;
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXUIField(DisplayName = "Tax Category")]
		[PXSelector(typeof(TX.TaxCategory.taxCategoryID), DescriptionField = typeof(TX.TaxCategory.descr))]
		[PXDefault(typeof(Search<Carrier.taxCategoryID, Where<Carrier.carrierID, Equal<Current<SOShipment.shipVia>>>>), SearchOnDefault = false, PersistingCheck = PXPersistingCheck.Nothing)]
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
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXSearchable(SM.SearchCategory.IN, "{0}: {1} - {3}", new Type[] { typeof(SOShipment.shipmentType), typeof(SOShipment.shipmentNbr), typeof(SOShipment.customerID), typeof(Customer.acctName) },
		   new Type[] { typeof(SOShipment.shipVia) },
		   NumberFields = new Type[] { typeof(SOShipment.shipmentNbr) },
		   Line1Format = "{0:d}{1}{2}{3}", Line1Fields = new Type[] { typeof(SOShipment.shipDate), typeof(SOShipment.status), typeof(SOShipment.shipmentQty), typeof(SOShipment.shipVia) },
		   Line2Format = "{1}{2}{3}{4}", Line2Fields = new Type[] { typeof(SOShipment.shipAddressID), typeof(SOAddress.addressLine1), typeof(SOAddress.addressLine2), typeof(SOAddress.city), typeof(SOAddress.state) },
		   MatchWithJoin = typeof(InnerJoin<Customer, On<Customer.bAccountID, Equal<SOShipment.customerID>>>),
		   SelectForFastIndexing = typeof(Select2<SOShipment, InnerJoin<Customer, On<SOShipment.customerID, Equal<Customer.bAccountID>>>>)
	   )]
		[PXNote(new Type[0], ShowInReferenceSelector = true)]
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
		#region Hold
		public abstract class hold : PX.Data.IBqlField
		{
		}
		protected Boolean? _Hold;
		[PXDBBool()]
		[PXDefault(typeof(SOSetup.holdShipments))]
		[PXUIField(DisplayName = "Hold")]
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
		#region Confirmed
		public abstract class confirmed : PX.Data.IBqlField
		{
		}
		protected Boolean? _Confirmed;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Confirmed")]
		public virtual Boolean? Confirmed
		{
			get
			{
				return this._Confirmed;
			}
			set
			{
				this._Confirmed = value;
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
		[PXUIField(DisplayName = "Released")]
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
		#region Status
		public abstract class status : PX.Data.IBqlField
		{
		}
		protected string _Status;
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[SOShipmentStatus.List()]
		[PXDefault()]
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
		#region LineCntr
		public abstract class lineCntr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineCntr;
		[PXDBInt()]
		[PXDefault(0)]
		public virtual Int32? LineCntr
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
		#region BilledOrderCntr
		public abstract class billedOrderCntr : PX.Data.IBqlField
		{
		}
		protected Int32? _BilledOrderCntr;
		[PXDBInt()]
		[PXDefault(0)]
		public virtual Int32? BilledOrderCntr
		{
			get
			{
				return this._BilledOrderCntr;
			}
			set
			{
				this._BilledOrderCntr = value;
			}
		}
		#endregion
		#region UnbilledOrderCntr
		public abstract class unbilledOrderCntr : PX.Data.IBqlField
		{
		}
		protected Int32? _UnbilledOrderCntr;
		[PXDBInt()]
		[PXDefault(0)]
		public virtual Int32? UnbilledOrderCntr
		{
			get
			{
				return this._UnbilledOrderCntr;
			}
			set
			{
				this._UnbilledOrderCntr = value;
			}
		}
		#endregion
		#region ReleasedOrderCntr
		public abstract class releasedOrderCntr : PX.Data.IBqlField
		{
		}
		protected Int32? _ReleasedOrderCntr;
		[PXDBInt()]
		[PXDefault(0)]
		public virtual Int32? ReleasedOrderCntr
		{
			get
			{
				return this._ReleasedOrderCntr;
			}
			set
			{
				this._ReleasedOrderCntr = value;
			}
		}
		#endregion
		#region ControlQty
		public abstract class controlQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _ControlQty;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName="Control Quantity")]
		public virtual Decimal? ControlQty
		{
			get
			{
				return this._ControlQty;
			}
			set
			{
				this._ControlQty = value;
			}
		}
		#endregion
		#region ShipmentQty
		public abstract class shipmentQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _ShipmentQty;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Shipped Quantity", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? ShipmentQty
		{
			get
			{
				return this._ShipmentQty;
			}
			set
			{
				this._ShipmentQty = value;
			}
		}
		#endregion
		#region ShipmentWeight
		public abstract class shipmentWeight : PX.Data.IBqlField
		{
		}
		protected Decimal? _ShipmentWeight;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Shipped Weight", Enabled = false)]
		public virtual Decimal? ShipmentWeight
		{
			get
			{
				return this._ShipmentWeight;
			}
			set
			{
				this._ShipmentWeight = value;
			}
		}
		public virtual Decimal? OrderWeight
		{
			get
			{
				return this._ShipmentWeight;
			}
			set
			{
				this._ShipmentWeight = value;
			}
		}
		#endregion
		#region ShipmentVolume
		public abstract class shipmentVolume : PX.Data.IBqlField
		{
		}
		protected Decimal? _ShipmentVolume;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Shipped Volume", Enabled = false)]
		public virtual Decimal? ShipmentVolume
		{
			get
			{
				return this._ShipmentVolume;
			}
			set
			{
				this._ShipmentVolume = value;
			}
		}
		public virtual Decimal? OrderVolume
		{
			get
			{
				return this._ShipmentVolume;
			}
			set
			{
				this._ShipmentVolume = value;
			}
		}
		#endregion
		#region PackageLineCntr
		public abstract class packageLineCntr : PX.Data.IBqlField
		{
		}
		protected Int32? _PackageLineCntr;
		[PXDBInt()]
		[PXDefault(0)]
		public virtual Int32? PackageLineCntr
		{
			get
			{
				return this._PackageLineCntr;
			}
			set
			{
				this._PackageLineCntr = value;
			}
		}
		#endregion
		#region PackageWeight
		public abstract class packageWeight : PX.Data.IBqlField
		{
		}
		protected Decimal? _PackageWeight;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Package Weight", Enabled = false)]
		public virtual Decimal? PackageWeight
		{
			get
			{
				return this._PackageWeight;
			}
			set
			{
				this._PackageWeight = value;
			}
		}
		#endregion
		#region IsPackageValid
		public abstract class isPackageValid : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsPackageValid;
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? IsPackageValid
		{
			get
			{
				return this._IsPackageValid;
			}
			set
			{
				this._IsPackageValid = value;
			}
		}
		#endregion
		#region PackageCount
		public abstract class packageCount : PX.Data.IBqlField
		{
		}
		protected int? _PackageCount;
		[PXInt]
		[PXUIField(DisplayName = "Packages", Enabled = false)]
		public virtual int? PackageCount
		{
			get
			{
				return this._PackageCount;
			}
			set
			{
				this._PackageCount = value;
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
		#region Hidden
		public abstract class hidden : PX.Data.IBqlField
		{
		}
		protected Boolean? _Hidden = false;
		[PXBool()]
		public virtual Boolean? Hidden
		{
			get
			{
				return this._Hidden;
			}
			set
			{
				this._Hidden = value;
			}
		}
		#endregion
		#region WorkgroupID
		public abstract class workgroupID : PX.Data.IBqlField
		{
		}
		protected int? _WorkgroupID;
		[PXDBInt]
		[PX.TM.PXCompanyTreeSelector]
		[PXFormula(typeof(Selector<SOShipment.customerID, Selector<Customer.workgroupID, TM.EPCompanyTree.description>>))]
		[PXUIField(DisplayName = "Workgroup", Visibility = PXUIVisibility.Visible)]
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
		[PX.TM.PXOwnerSelector(typeof(SOShipment.workgroupID))]
        [PXFormula(typeof(Selector<SOShipment.customerID, Selector<Customer.ownerID, PX.TM.PXOwnerSelectorAttribute.EPEmployee.acctCD>>))]
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
    #region FreeItemQtyTot
    public abstract class freeItemQtyTot : PX.Data.IBqlField
    {
    }
    protected Decimal? _FreeItemQtyTot;
    [PXDBQuantity]
    [PXUIField(DisplayName = "Free Items Quantity Total")]
    public virtual Decimal? FreeItemQtyTot
    {
        get
        {
            return this._FreeItemQtyTot;
        }
        set
        {
            this._FreeItemQtyTot = value;
        }
    }
    #endregion
		#region SiteID_INSite_descr
		public abstract class siteID_INSite_descr : PX.Data.IBqlField
		{
		}
		#endregion
		#region ShipVia_Carrier_description
		public abstract class shipVia_Carrier_description : PX.Data.IBqlField
		{
		}
		#endregion
		#region StatusIsNull
		public abstract class statusIsNull : PX.Data.IBqlField
		{
		}
		protected string _StatusIsNull;
		[PXString()]
		[PXDBCalced(typeof(Switch<Case<Where<SOShipment.status, IsNull>, SOShipmentStatus.autoGenerated>, SOShipment.status>), typeof(string))]
		[SOShipmentStatus.List()]
		[PXUIField(DisplayName = "Status", Enabled = false)]
		public virtual string StatusIsNull
		{
			get
			{
				return this._StatusIsNull;
			}
			set
			{
				this._StatusIsNull = value;
			}
		}
		#endregion
		#region Methods
		public static implicit operator SOShipment(PXResult<POReceipt, SOOrderShipment, SOOrder> item)
		{
			SOShipment ret = new SOShipment();
			ret.ShipmentNbr = ((POReceipt)item).ReceiptNbr;
			ret.ShipmentType = INDocType.DropShip;
			ret.CustomerID = ((SOOrder)item).CustomerID;
			ret.CustomerLocationID = ((SOOrder)item).CustomerLocationID;
			ret.CustomerOrderNbr = ((SOOrder)item).CustomerOrderNbr;
			ret.Confirmed = true;
			ret.Hold = false;
			ret.CreatedByID = ((POReceipt)item).CreatedByID;
			ret.CreatedByScreenID = ((POReceipt)item).CreatedByScreenID;
			ret.CreatedDateTime = ((POReceipt)item).CreatedDateTime;
			ret.LastModifiedByID = ((POReceipt)item).LastModifiedByID;
			ret.LastModifiedByScreenID = ((POReceipt)item).LastModifiedByScreenID;
			ret.LastModifiedDateTime = ((POReceipt)item).LastModifiedDateTime;
			ret.ShipDate = ((POReceipt)item).ReceiptDate;
			ret.ShipmentQty = ((POReceipt)item).OrderQty;
			ret.ShipmentVolume = ((POReceipt)item).ReceiptVolume;
			ret.ShipmentWeight = ((POReceipt)item).ReceiptWeight;
			ret.Status = SOShipmentStatus.Receipted;
			ret.WorkgroupID = ((POReceipt)item).WorkgroupID;
			ret.OwnerID = ((POReceipt)item).OwnerID;
			ret.SiteID = null;
			ret.ShipVia = null;
			ret.FOBPoint = null;

			return ret;
		}
		#endregion
	}

	public class SOShipmentStatus
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Open, Hold, Completed, Cancelled, Confirmed, Invoiced, PartiallyInvoiced, Receipted, AutoGenerated  },
				new string[] { Messages.Open, Messages.Hold, Messages.Completed, Messages.Cancelled, Messages.Confirmed, Messages.Invoiced, Messages.PartiallyInvoiced, Messages.Receipted, Messages.AutoGenerated }) { ; }
		}

		public const string Open = "N";
		public const string Hold = "H";
		public const string Completed = "C";
		public const string Cancelled = "L";
		public const string Confirmed = "F";
		public const string Invoiced = "I";
		public const string Receipted = "R";
		public const string AutoGenerated = "A";
		public const string PartiallyInvoiced = "Y";

		public class open : Constant<string>
		{
			public open() : base(Open) { ;}
		}

		public class hold : Constant<string>
		{
			public hold() : base(Hold) { ;}
		}

		public class completed : Constant<string>
		{
			public completed() : base(Completed) { ;}
		}

		public class cancelled : Constant<string>
		{
			public cancelled() : base(Cancelled) { ;}
		}

		public class confirmed : Constant<string>
		{
			public confirmed() : base(Confirmed) { ;}
		}

		public class invoiced : Constant<string>
		{
			public invoiced() : base(Invoiced) { ;}
		}

		public class partiallyInvoiced : Constant<string>
		{
			public partiallyInvoiced() : base(PartiallyInvoiced) { ;}
		}

		public class receipted : Constant<string>
		{
			public receipted() : base(Receipted) { ;}
		}

		public class autoGenerated : Constant<string>
		{
			public autoGenerated() : base(AutoGenerated) { ;}
		}
	}
}
