using System;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.GL;
using PX.Objects.TX;
using PX.Objects.CS;
using PX.Objects.PO;
using PX.Objects.IN;
using PX.Objects.SO;
using PX.Objects.CA;
using CRLocation = PX.Objects.CR.Standalone.Location;


namespace PX.Objects.CR
{

	[System.SerializableAttribute()]
	[PXPrimaryGraph(
		new [] { typeof(LocationMaint) }, 
		new [] { typeof(Select<Location, Where<Location.bAccountID, Equal<Current<LocationExtAddress.bAccountID>>, And<Location.locationID, Equal<Current<LocationExtAddress.locationID>>>>>)})]
	[PXProjection(typeof(Select2<CRLocation,
	LeftJoin<Address, On<CRLocation.bAccountID,
			Equal<Address.bAccountID>,
			And<CRLocation.defAddressID, Equal<Address.addressID>>>,
	LeftJoin<BAccountR, On<BAccountR.bAccountID, Equal<CRLocation.bAccountID>>>>>), Persistent = true)]
	[PXCacheName(Messages.LocationExtAddress)]
	public partial class LocationExtAddress : Address, IDefAddressAccessor, ILocation
	{
		#region LocationBAccountID
		public abstract class locationBAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _LocationBAccountID;
		[PXDBInt(IsKey = true, BqlField = typeof(CRLocation.bAccountID))]
		[PXDBLiteDefault(typeof(BAccount.bAccountID))]
		[PXUIField(DisplayName = "Business Account ID", Visible = false, Enabled = false, Visibility = PXUIVisibility.Invisible)]
		[PXParent(typeof(Select<BAccount,
			Where<BAccount.bAccountID,
				Equal<Current<LocationExtAddress.bAccountID>>,
			And<BAccount.type, NotEqual<BAccountType.combinedType>>>>),
				LeaveChildren = true)]
		[PXNavigateSelector(typeof(LocationExtAddress.locationBAccountID))]
		public virtual Int32? LocationBAccountID
		{
			get
			{
				return this._LocationBAccountID;
			}
			set
			{
				this._LocationBAccountID = value;
			}
		}
		#endregion

		#region LocationID
		public abstract class locationID : PX.Data.IBqlField
		{
		}
		protected Int32? _LocationID;
		//[PXLocationID(IsKey = false, BqlField = typeof(CRLocation.locationID))]
		[PXDBIdentity(IsKey = false, BqlField = typeof(CRLocation.locationID))]
		[PXUIField(DisplayName = "LocationID", Visible = false, Visibility = PXUIVisibility.Invisible)]
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
		#region LocationCD
		public abstract class locationCD : PX.Data.IBqlField
		{
		}
		protected String _LocationCD;

		
		[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
		[CS.LocationRaw(typeof(Where<Location.bAccountID, Equal<Current<LocationExtAddress.bAccountID>>>), IsKey = true, Visibility = PXUIVisibility.SelectorVisible, DisplayName = "Location ID", BqlField=typeof(Location.locationCD), CacheGlobal = false)]		
		public virtual String LocationCD
		{
			get
			{
				return this._LocationCD;
			}
			set
			{
				this._LocationCD = value;
			}
		}
		#endregion
		#region LocType
		public abstract class locType : PX.Data.IBqlField
		{
		}
		protected String _LocType;
		[PXDBString(BqlField = typeof(CRLocation.locType))]
		[LocTypeList.List()]
		[PXUIField(DisplayName = "Location Type", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String LocType
		{
			get
			{
				return this._LocType;
			}
			set
			{
				this._LocType = value;
			}
		}
		#endregion
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;
		[PXDBString(60, IsUnicode = true, BqlField = typeof(CRLocation.descr))]
		[PXUIField(DisplayName = "Location Name", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String Descr
		{
			get
			{
				return this._Descr;
			}
			set
			{
				this._Descr = value;
			}
		}
		#endregion
		#region TaxRegistrationID
		public abstract class taxRegistrationID : PX.Data.IBqlField
		{
		}
		protected String _TaxRegistrationID;
		[PXDBString(50, IsUnicode = true, BqlField = typeof(CRLocation.taxRegistrationID))]
		[PXUIField(DisplayName = "Tax Registration ID")]
		public virtual String TaxRegistrationID
		{
			get
			{
				return this._TaxRegistrationID;
			}
			set
			{
				this._TaxRegistrationID = value;
			}
		}
		#endregion
		#region DefAddressID
		public abstract class defAddressID : PX.Data.IBqlField
		{
		}
		protected Int32? _DefAddressID;
		[PXDBLiteDefault(typeof(Address.addressID))]
		[PXDBInt(BqlField = typeof(CRLocation.defAddressID))]
		[PXUIField(DisplayName = "Default Address", Visibility = PXUIVisibility.Invisible, Visible = false)]
		[PXSelector(typeof(Search<Address.addressID,
				Where<Address.bAccountID,
				Equal<Current<BAccount.bAccountID>>>>),
				DirtyRead = true)]
		public virtual Int32? DefAddressID
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
		#region DefContactID
		public abstract class defContactID : PX.Data.IBqlField
		{
		}
		protected Int32? _DefContactID;
		[PXDBLiteDefault(typeof(Contact.contactID))]
		[PXDBInt(BqlField = typeof(CRLocation.defContactID))]
		public virtual Int32? DefContactID
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
		#region IsActive
		public abstract class isActive : PX.Data.IBqlField
		{
		}
		protected bool? _IsActive;
		[PXDBBool(BqlField = typeof(CRLocation.isActive))]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Active")]
		public virtual bool? IsActive
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
		#region IsDefault
		public abstract class isDefault : PX.Data.IBqlField
		{
		}
		protected bool? _IsDefault;
		[PXBool()]
		[PXUIField(DisplayName = "Is Default")]
		public virtual bool? IsDefault
		{
			get
			{
				return this._IsDefault;
			}
			set
			{
				this._IsDefault = value;
			}
		}
		#endregion
		#region IsRemitAddressSameAsMain
		public abstract class isRemitAddressSameAsMain : PX.Data.IBqlField
		{
		}
		protected bool? _IsRemitAddressSameAsMain;
		[PXBool()]
		[PXUIField(DisplayName = "Same as Main")]
		public virtual bool? IsRemitAddressSameAsMain
		{
			get
			{
				return this._IsRemitAddressSameAsMain;
			}
			set
			{
				this._IsRemitAddressSameAsMain = value;
			}
		}
		#endregion
		#region IsRemitContactSameAsMain
		public abstract class isRemitContactSameAsMain : PX.Data.IBqlField
		{
		}
		protected bool? _IsRemitContactSameAsMain;
		[PXBool()]
		[PXUIField(DisplayName = "Same as Main")]
		public virtual bool? IsRemitContactSameAsMain
		{
			get
			{
				return this._IsRemitContactSameAsMain;
			}
			set
			{
				this._IsRemitContactSameAsMain = value;
			}
		}
		#endregion
		//Customer Locaiton Properties
		#region CTaxZoneID
		public abstract class cTaxZoneID : PX.Data.IBqlField
		{
		}
		protected String _CTaxZoneID;
		[PXDBString(BqlField = typeof(CRLocation.cTaxZoneID))]
		[PXUIField(DisplayName = "Tax Zone")]
		[PXSelector(typeof(Search<TaxZone.taxZoneID>), DescriptionField = typeof(TaxZone.descr), CacheGlobal = true)]
		public virtual String CTaxZoneID
		{
			get
			{
				return this._CTaxZoneID;
			}
			set
			{
				this._CTaxZoneID = value;
			}
		}
		#endregion
		#region CAvalaraCustomerUsageType
		public abstract class cAvalaraCustomerUsageType : PX.Data.IBqlField
		{
		}
		protected String _CAvalaraCustomerUsageType;
		[PXDBString(1, IsFixed = true, BqlField = typeof(CRLocation.cAvalaraCustomerUsageType))]
		[PXUIField(DisplayName = "Entity Usage Type")]
		[TX.TXAvalaraCustomerUsageType.List]
		public virtual String CAvalaraCustomerUsageType
		{
			get
			{
				return this._CAvalaraCustomerUsageType;
			}
			set
			{
				this._CAvalaraCustomerUsageType = value;
			}
		}
		#endregion
		#region CCarrierID
		public abstract class cCarrierID : PX.Data.IBqlField
		{
		}
		protected String _CCarrierID;
		[PXDBString(BqlField = typeof(CRLocation.cCarrierID), InputMask = ">aaaaaaaaaaaaaaa")]
		[PXUIField(DisplayName = "Ship Via")]
		[PXSelector(typeof(Search<Carrier.carrierID>),
			typeof(Carrier.carrierID), typeof(Carrier.description), typeof(Carrier.isExternal), typeof(Carrier.confirmationRequired),
			CacheGlobal = true,
			DescriptionField = typeof(Carrier.description))]
		public virtual String CCarrierID
		{
			get
			{
				return this._CCarrierID;
			}
			set
			{
				this._CCarrierID = value;
			}
		}
		#endregion
		#region CShipTermsID
		public abstract class cShipTermsID : PX.Data.IBqlField
		{
		}
		protected String _CShipTermsID;
		[PXDBString(BqlField = typeof(CRLocation.cShipTermsID))]
		[PXUIField(DisplayName = "Shipping Terms")]
		[PXSelector(typeof(Search<ShipTerms.shipTermsID>), CacheGlobal = true, DescriptionField = typeof(ShipTerms.description))]
		public virtual String CShipTermsID
		{
			get
			{
				return this._CShipTermsID;
			}
			set
			{
				this._CShipTermsID = value;
			}
		}
		#endregion
		#region CShipZoneID
		public abstract class cShipZoneID : PX.Data.IBqlField
		{
		}
		protected String _CShipZoneID;
		[PXDBString(15, IsUnicode = true, InputMask = ">aaaaaaaaaaaaaaa", BqlField = typeof(CRLocation.cShipZoneID))]
		[PXUIField(DisplayName = "Shipping Zone ID")]
		[PXSelector(typeof(ShippingZone.zoneID), CacheGlobal = true, DescriptionField = typeof(ShippingZone.description))]
		public virtual String CShipZoneID
		{
			get
			{
				return this._CShipZoneID;
			}
			set
			{
				this._CShipZoneID = value;
			}
		}
		#endregion
		#region CFOBPointID
		public abstract class cFOBPointID : PX.Data.IBqlField
		{
		}
		protected String _CFOBPointID;
		[PXDBString(BqlField = typeof(CRLocation.cFOBPointID))]
		[PXUIField(DisplayName = "FOB Point")]
		[PXSelector(typeof(FOBPoint.fOBPointID), CacheGlobal = true, DescriptionField = typeof(FOBPoint.description))]
		public virtual String CFOBPointID
		{
			get
			{
				return this._CFOBPointID;
			}
			set
			{
				this._CFOBPointID = value;
			}
		}
		#endregion
		#region CResedential
		public abstract class cResedential : PX.Data.IBqlField
		{
		}
		protected Boolean? _CResedential;
		[PXDBBool(BqlField = typeof(CRLocation.cResedential))]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Residential Delivery")]
		public virtual Boolean? CResedential
		{
			get
			{
				return this._CResedential;
			}
			set
			{
				this._CResedential = value;
			}
		}
		#endregion
		#region CSaturdayDelivery
		public abstract class cSaturdayDelivery : PX.Data.IBqlField
		{
		}
		protected Boolean? _CSaturdayDelivery;
		[PXDBBool(BqlField = typeof(CRLocation.cSaturdayDelivery))]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Saturday Delivery")]
		public virtual Boolean? CSaturdayDelivery
		{
			get
			{
				return this._CSaturdayDelivery;
			}
			set
			{
				this._CSaturdayDelivery = value;
			}
		}
		#endregion
		#region CGroundCollect
		public abstract class cGroundCollect : PX.Data.IBqlField
		{
		}
		protected Boolean? _CGroundCollect;
		[PXDBBool(BqlField = typeof(CRLocation.cGroundCollect))]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Ground Collect")]
		public virtual Boolean? CGroundCollect
		{
			get
			{
				return this._CGroundCollect;
			}
			set
			{
				this._CGroundCollect = value;
			}
		}
		#endregion
		#region CInsurance
		public abstract class cInsurance : PX.Data.IBqlField
		{
		}
		protected Boolean? _CInsurance;
		[PXDBBool(BqlField = typeof(CRLocation.cInsurance))]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Insurance")]
		public virtual Boolean? CInsurance
		{
			get
			{
				return this._CInsurance;
			}
			set
			{
				this._CInsurance = value;
			}
		}
		#endregion
		#region CLeadTime
		public abstract class cLeadTime : PX.Data.IBqlField
		{
		}
		protected Int16? _CLeadTime;
		[PXDBShort(BqlField = typeof(CRLocation.cLeadTime))]
		[PXUIField(DisplayName = "Lead Time (days)")]
		public virtual Int16? CLeadTime
		{
			get
			{
				return this._CLeadTime;
			}
			set
			{
				this._CLeadTime = value;
			}
		}
		#endregion
		#region CBranchID
		public abstract class cBranchID : PX.Data.IBqlField
		{
		}
		protected Int32? _CBranchID;
		[Branch(null, IsDetail = false, DisplayName = "Shipping Branch", BqlField = typeof(CRLocation.cBranchID), PersistingCheck = PXPersistingCheck.Nothing, IsEnabledWhenOneBranchIsAccessible = true)]		
		public virtual Int32? CBranchID
		{
			get
			{
				return this._CBranchID;
			}
			set
			{
				this._CBranchID = value;
			}
		}
		#endregion
		#region CSalesAcctID
		public abstract class cSalesAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _CSalesAcctID;
		[Account(DisplayName = "Sales Account", BqlField = typeof(CRLocation.cSalesAcctID), Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description), Required = true)]
		public virtual Int32? CSalesAcctID
		{
			get
			{
				return this._CSalesAcctID;
			}
			set
			{
				this._CSalesAcctID = value;
			}
		}
		#endregion
		#region CSalesSubID
		public abstract class cSalesSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _CSalesSubID;
		[SubAccount(typeof(LocationExtAddress.cSalesAcctID), BqlField = typeof(CRLocation.cSalesSubID), DisplayName = "Sales Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description), Required = true)]
		public virtual Int32? CSalesSubID
		{
			get
			{
				return this._CSalesSubID;
			}
			set
			{
				this._CSalesSubID = value;
			}
		}
		#endregion
		#region CPriceClassID
		public abstract class cPriceClassID : PX.Data.IBqlField
		{
		}
		protected String _CPriceClassID;
		[PXDBString(BqlField = typeof(CRLocation.cPriceClassID))]
		[PXSelector(typeof(AR.ARPriceClass.priceClassID))]
		[PXUIField(DisplayName = "Price Class ID", Visibility = PXUIVisibility.Visible)]
		public virtual String CPriceClassID
		{
			get
			{
				return this._CPriceClassID;
			}
			set
			{
				this._CPriceClassID = value;
			}
		}
		#endregion
		#region CSiteID
		public abstract class cSiteID : PX.Data.IBqlField
		{
		}
		protected Int32? _CSiteID;
		[PXDBInt(BqlField = typeof(CRLocation.cSiteID))]
		[PXUIField(DisplayName = "Warehouse", Visibility = PXUIVisibility.Visible)]
		[PXDimensionSelector(SiteAttribute.DimensionName, typeof(INSite.siteID), typeof(INSite.siteCD), DescriptionField = typeof(INSite.descr))]
        [PXRestrictor(typeof(Where<INSite.active, Equal<True>>), IN.Messages.InactiveWarehouse, typeof(INSite.siteCD))]
        public virtual Int32? CSiteID
		{
			get
			{
				return this._CSiteID;
			}
			set
			{
				this._CSiteID = value;
			}
		}
		#endregion
		#region CDiscountAcctID
		public abstract class cDiscountAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _CDiscountAcctID;
		[Account(DisplayName = "Discount Account", BqlField = typeof(CRLocation.cDiscountAcctID), Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description), Required = false)]
		public virtual Int32? CDiscountAcctID
		{
			get
			{
				return this._CDiscountAcctID;
			}
			set
			{
				this._CDiscountAcctID = value;
			}
		}
		#endregion
		#region CDiscountSubID
		public abstract class cDiscountSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _CDiscountSubID;
		[SubAccount(typeof(LocationExtAddress.cDiscountAcctID), BqlField = typeof(CRLocation.cDiscountSubID), DisplayName = "Discount Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description), Required = false)]
		public virtual Int32? CDiscountSubID
		{
			get
			{
				return this._CDiscountSubID;
			}
			set
			{
				this._CDiscountSubID = value;
			}
		}
		#endregion
		#region CFreightAcctID
		public abstract class cFreightAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _CFreightAcctID;
		[Account(DisplayName = "Freight Account", BqlField = typeof(CRLocation.cFreightAcctID), Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description), Required = false)]
		public virtual Int32? CFreightAcctID
		{
			get
			{
				return this._CFreightAcctID;
			}
			set
			{
				this._CFreightAcctID = value;
			}
		}
		#endregion
		#region CFreightSubID
		public abstract class cFreightSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _CFreightSubID;
		[SubAccount(typeof(LocationExtAddress.cFreightAcctID), BqlField = typeof(CRLocation.cFreightSubID), DisplayName = "Freight Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description), Required = false)]
		public virtual Int32? CFreightSubID
		{
			get
			{
				return this._CFreightSubID;
			}
			set
			{
				this._CFreightSubID = value;
			}
		}
		#endregion
		#region CShipComplete
		public abstract class cShipComplete : PX.Data.IBqlField
		{
		}
		protected String _CShipComplete;
		[PXDBString(1, IsFixed = true, BqlField = typeof(CRLocation.cShipComplete))]
		[SOShipComplete.List()]
		[PXDefault(SOShipComplete.CancelRemainder)]
		[PXUIField(DisplayName = "Shipping Rule")]
		public virtual String CShipComplete
		{
			get
			{
				return this._CShipComplete;
			}
			set
			{
				this._CShipComplete = value;
			}
		}
		#endregion
		#region COrderPriority
		public abstract class cOrderPriority : PX.Data.IBqlField
		{
		}
		protected Int16? _COrderPriority;
		[PXDBShort(BqlField=typeof(CRLocation.cOrderPriority))]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "Order Priority")]
		public virtual Int16? COrderPriority
		{
			get
			{
				return this._COrderPriority;
			}
			set
			{
				this._COrderPriority = value;
			}
		}
		#endregion
		#region CARAccountLocationID
		public abstract class cARAccountLocationID : PX.Data.IBqlField
		{
		}
		protected Int32? _CARAccountLocationID;
		[PXDBInt(BqlField = typeof(CRLocation.cARAccountLocationID))]
		[PXDefault()]
		public virtual Int32? CARAccountLocationID
		{
			get
			{
				return this._CARAccountLocationID;
			}
			set
			{
				this._CARAccountLocationID = value;
			}
		}
		#endregion
		#region CARAccountID
		public abstract class cARAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _CARAccountID;
        [Account(null, typeof(Search<Account.accountID,
                    Where2<Match<Current<AccessInfo.userName>>,
                         And<Account.active, Equal<True>,
                         And<Account.isCashAccount, Equal<False>,
                         And<Where<Current<GLSetup.ytdNetIncAccountID>, IsNull,
                          Or<Account.accountID, NotEqual<Current<GLSetup.ytdNetIncAccountID>>>>>>>>>), 
				DisplayName = "AR Account", 
				BqlField = typeof(CRLocation.cARAccountID),
				Required = true)]
		public virtual Int32? CARAccountID
		{
			get
			{
				return this._CARAccountID;
			}
			set
			{
				this._CARAccountID = value;
			}
		}
		#endregion
		#region CARSubID
		public abstract class cARSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _CARSubID;

		[SubAccount(typeof(LocationExtAddress.cARAccountID), 
					BqlField = typeof(CRLocation.cARSubID), 
					DisplayName = "AR Sub.", 
					DescriptionField = typeof(Sub.description),
					Required = true)]
		public virtual Int32? CARSubID
		{
			get
			{
				return this._CARSubID;
			}
			set
			{
				this._CARSubID = value;
			}
		}
		#endregion
		#region IsARAccountSameAsMain
		public abstract class isARAccountSameAsMain : PX.Data.IBqlField
		{
		}
		protected bool? _IsARAccountSameAsMain;
		[PXBool()]
		[PXFormula(typeof(Switch<Case<Where<locationID, Equal<cARAccountLocationID>>, False>, True>))]
		public virtual bool? IsARAccountSameAsMain
		{
			get
			{
				return this._IsARAccountSameAsMain;
			}
			set
			{
				this._IsARAccountSameAsMain = value;
			}
		}
		#endregion

		// Vendor Location Properties
		#region VTaxZoneID
		public abstract class vTaxZoneID : PX.Data.IBqlField
		{
		}
		protected String _VTaxZoneID;
		[PXDBString(BqlField = typeof(CRLocation.vTaxZoneID))]
		[PXUIField(DisplayName = "Tax Zone ID")]
		[PXSelector(typeof(Search<TaxZone.taxZoneID>), DescriptionField = typeof(TaxZone.descr), CacheGlobal = true)]
		public virtual String VTaxZoneID
		{
			get
			{
				return this._VTaxZoneID;
			}
			set
			{
				this._VTaxZoneID = value;
			}
		}
		#endregion
		#region VTaxCalcMode
		public abstract class vTaxCalcMode : IBqlField { }
		[PXDBString(BqlField = typeof(CRLocation.vTaxCalcMode))]
		[PXDefault(VendorClass.taxCalcMode.TaxSetting, typeof(Search<VendorClass.taxCalcMode, Where<VendorClass.vendorClassID, Equal<Current<Vendor.vendorClassID>>>>))]
		[VendorClass.taxCalcMode.List]
		[PXUIField(DisplayName = "Tax Calculation Mode")]
		public virtual string VTaxCalcMode { get; set; }
		#endregion
		#region VCarrierID
		public abstract class vCarrierID : PX.Data.IBqlField
		{
		}
		protected String _VCarrierID;
		[PXDBString(BqlField = typeof(CRLocation.vCarrierID), InputMask = ">aaaaaaaaaaaaaaa")]
		[PXUIField(DisplayName = "Ship Via")]
		[PXSelector(typeof(Search<Carrier.carrierID>),
			typeof(Carrier.carrierID), typeof(Carrier.description), typeof(Carrier.isExternal), typeof(Carrier.confirmationRequired),
			CacheGlobal = true,
			DescriptionField = typeof(Carrier.description))]
		public virtual String VCarrierID
		{
			get
			{
				return this._VCarrierID;
			}
			set
			{
				this._VCarrierID = value;
			}
		}
		#endregion
		#region VShipTermsID
		public abstract class vShipTermsID : PX.Data.IBqlField
		{
		}
		protected String _VShipTermsID;
		[PXDBString(BqlField = typeof(CRLocation.vShipTermsID))]
		[PXUIField(DisplayName = "Shipping Terms")]
		[PXSelector(typeof(Search<ShipTerms.shipTermsID>), CacheGlobal = true, DescriptionField = typeof(ShipTerms.description))]
		public virtual String VShipTermsID
		{
			get
			{
				return this._VShipTermsID;
			}
			set
			{
				this._VShipTermsID = value;
			}
		}
		#endregion
		#region VFOBPointID
		public abstract class vFOBPointID : PX.Data.IBqlField
		{
		}
		protected String _VFOBPointID;
		[PXDBString(BqlField = typeof(CRLocation.vFOBPointID))]
		[PXUIField(DisplayName = "FOB Point")]
		[PXSelector(typeof(FOBPoint.fOBPointID), CacheGlobal = true, DescriptionField = typeof(FOBPoint.description))]
		public virtual String VFOBPointID
		{
			get
			{
				return this._VFOBPointID;
			}
			set
			{
				this._VFOBPointID = value;
			}
		}
		#endregion
		#region VLeadTime
		public abstract class vLeadTime : PX.Data.IBqlField
		{
		}
		protected Int16? _VLeadTime;
		[PXDBShort(BqlField = typeof(CRLocation.vLeadTime))]
		[PXUIField(DisplayName = "Lead Time (days)")]
		public virtual Int16? VLeadTime
		{
			get
			{
				return this._VLeadTime;
			}
			set
			{
				this._VLeadTime = value;
			}
		}
		#endregion
		#region VBranchID
		public abstract class vBranchID : PX.Data.IBqlField
		{
		}
		protected Int32? _VBranchID;
		[Branch(null, IsDetail = false, DisplayName = "Receiving Branch", BqlField = typeof(CRLocation.vBranchID), PersistingCheck = PXPersistingCheck.Nothing, IsEnabledWhenOneBranchIsAccessible = true)]
		public virtual Int32? VBranchID
		{
			get
			{
				return this._VBranchID;
			}
			set
			{
				this._VBranchID = value;
			}
		}
		#endregion
		#region VExpenseAcctID
		public abstract class vExpenseAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _VExpenseAcctID;

		[Account(DisplayName = "Expense Account", 
				BqlField = typeof(CRLocation.vExpenseAcctID), 
				Visibility = PXUIVisibility.Visible, 
				DescriptionField = typeof(Account.description))]
		public virtual Int32? VExpenseAcctID
		{
			get
			{
				return this._VExpenseAcctID;
			}
			set
			{
				this._VExpenseAcctID = value;
			}
		}
		#endregion
		#region VExpenseSubID
		public abstract class vExpenseSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _VExpenseSubID;

		[SubAccount(typeof(LocationExtAddress.vExpenseAcctID), 
					BqlField = typeof(CRLocation.vExpenseSubID), 
					DisplayName = "Expense Sub.", 
					Visibility = PXUIVisibility.Visible, 
					DescriptionField = typeof(Sub.description))]
		public virtual Int32? VExpenseSubID
		{
			get
			{
				return this._VExpenseSubID;
			}
			set
			{
				this._VExpenseSubID = value;
			}
		}
		#endregion
		#region VFreightAcctID
		public abstract class vFreightAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _VFreightAcctID;
		[Account(DisplayName = "Freight Account", BqlField = typeof(CRLocation.vFreightAcctID), Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description), Required = false)]
		public virtual Int32? VFreightAcctID
		{
			get
			{
				return this._VFreightAcctID;
			}
			set
			{
				this._VFreightAcctID = value;
			}
		}
		#endregion
		#region VFreightSubID
		public abstract class vFreightSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _VFreightSubID;
		[SubAccount(typeof(LocationExtAddress.vFreightAcctID), BqlField = typeof(CRLocation.vFreightSubID), DisplayName = "Freight Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description), Required = false)]
		public virtual Int32? VFreightSubID
		{
			get
			{
				return this._VFreightSubID;
			}
			set
			{
				this._VFreightSubID = value;
			}
		}
		#endregion
        #region VDiscountAcctID
        public abstract class vDiscountAcctID : PX.Data.IBqlField
        {
        }
        protected Int32? _VDiscountAcctID;
        [Account(DisplayName = "Discount Account", BqlField = typeof(CRLocation.vDiscountAcctID), Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description), Required = false)]
        public virtual Int32? VDiscountAcctID
        {
            get
            {
                return this._VDiscountAcctID;
            }
            set
            {
                this._VDiscountAcctID = value;
            }
        }
        #endregion
        #region VDiscountSubID
        public abstract class vDiscountSubID : PX.Data.IBqlField
        {
        }
        protected Int32? _VDiscountSubID;
        [SubAccount(typeof(LocationExtAddress.vDiscountAcctID), BqlField = typeof(CRLocation.vDiscountSubID), DisplayName = "Discount Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description), Required = false)]
        public virtual Int32? VDiscountSubID
        {
            get
            {
                return this._VDiscountSubID;
            }
            set
            {
                this._VDiscountSubID = value;
            }
        }
        #endregion

		#region VRcptQtyMin
		public abstract class vRcptQtyMin : PX.Data.IBqlField
		{
		}
		protected Decimal? _VRcptQtyMin;
		[PXDBDecimal(2, MinValue = 0.0, MaxValue = 100.0, BqlField = typeof(CRLocation.vRcptQtyMin))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Min. Receipt (%)")]
		public virtual Decimal? VRcptQtyMin
		{
			get
			{
				return this._VRcptQtyMin;
			}
			set
			{
				this._VRcptQtyMin = value;
			}
		}
		#endregion
		#region VRcptQtyMax
		public abstract class vRcptQtyMax : PX.Data.IBqlField
		{
		}
		protected Decimal? _VRcptQtyMax;
		[PXDBDecimal(2, MinValue = 0.0, MaxValue = 999.0, BqlField = typeof(CRLocation.vRcptQtyMax))]
		[PXDefault(TypeCode.Decimal, "100.0")]
		[PXUIField(DisplayName = "Max. Receipt (%)")]
		public virtual Decimal? VRcptQtyMax
		{
			get
			{
				return this._VRcptQtyMax;
			}
			set
			{
				this._VRcptQtyMax = value;
			}
		}
		#endregion
		#region VRcptQtyThreshold
		public abstract class vRcptQtyThreshold : PX.Data.IBqlField
		{
		}
		protected Decimal? _VRcptQtyThreshold;
		[PXDBDecimal(2, MinValue = 0.0, MaxValue = 999.0, BqlField = typeof(CRLocation.vRcptQtyThreshold))]
		[PXDefault(TypeCode.Decimal, "100.0")]
		[PXUIField(DisplayName = "Threshold Receipt (%)")]
		public virtual Decimal? VRcptQtyThreshold
		{
			get
			{
				return this._VRcptQtyThreshold;
			}
			set
			{
				this._VRcptQtyThreshold = value;
			}
		}
		#endregion
		#region VRcptQtyAction
		public abstract class vRcptQtyAction : PX.Data.IBqlField
		{
		}
		protected String _VRcptQtyAction;
		[PXDBString(1, IsFixed = true, BqlField = typeof(CRLocation.vRcptQtyAction))]
		[POReceiptQtyAction.List()]
		[PXDefault(POReceiptQtyAction.AcceptButWarn)]
		[PXUIField(DisplayName = "Receipt Action")]
		public virtual String VRcptQtyAction
		{
			get
			{
				return this._VRcptQtyAction;
			}
			set
			{
				this._VRcptQtyAction = value;
			}
		}
		#endregion
		#region VSiteID
		public abstract class vSiteID : PX.Data.IBqlField
		{
		}
		protected Int32? _VSiteID;
		[PXDBInt(BqlField = typeof(CRLocation.vSiteID))]
		[PXUIField(DisplayName = "Warehouse", Visibility = PXUIVisibility.Visible)]
		[PXDimensionSelector(SiteAttribute.DimensionName, typeof(INSite.siteID), typeof(INSite.siteCD), DescriptionField = typeof(INSite.descr))]
        [PXRestrictor(typeof(Where<INSite.active, Equal<True>>), IN.Messages.InactiveWarehouse, typeof(INSite.siteCD))]
        public virtual Int32? VSiteID
		{
			get
			{
				return this._VSiteID;
			}
			set
			{
				this._VSiteID = value;
			}
		}
		#endregion
		#region VPrintOrder
		public abstract class vPrintOrder : PX.Data.IBqlField
		{
		}
		protected bool? _VPrintOrder;
		[PXDBBool(BqlField=typeof(CRLocation.vPrintOrder))]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Print Orders")]
		public virtual bool? VPrintOrder
		{
			get
			{
				return this._VPrintOrder;
			}
			set
			{
				this._VPrintOrder = value;
			}
		}
		#endregion
		#region VEmailOrder
		public abstract class vEmailOrder : PX.Data.IBqlField
		{
		}
		protected bool? _VEmailOrder;
		[PXDBBool(BqlField=typeof(CRLocation.vEmailOrder))]
		[PXDefault(false)]
        [PXUIField(DisplayName = "Send Orders by Email")]
		public virtual bool? VEmailOrder
		{
			get
			{
				return this._VEmailOrder;
			}
			set
			{
				this._VEmailOrder = value;
			}
		}
		#endregion
		#region VPaymentInfoLocationID
		public abstract class vPaymentInfoLocationID : PX.Data.IBqlField
		{
		}
		protected Int32? _VPaymentInfoLocationID;
		[PXDBInt(BqlField = typeof(CRLocation.vPaymentInfoLocationID))]
		[PXDefault()]
		public virtual Int32? VPaymentInfoLocationID
		{
			get
			{
				return this._VPaymentInfoLocationID;
			}
			set
			{
				this._VPaymentInfoLocationID = value;
			}
		}
		#endregion
		#region VRemitAddressID
		public abstract class vRemitAddressID : PX.Data.IBqlField
		{
		}
		protected Int32? _VRemitAddressID;
		[PXDBInt(BqlField = typeof(CRLocation.vRemitAddressID))]
		[PXDBChildIdentity(typeof(Address.addressID))]
		public virtual Int32? VRemitAddressID
		{
			get
			{
				return this._VRemitAddressID;
			}
			set
			{
				this._VRemitAddressID = value;
			}
		}
		#endregion
		#region VRemitContactID
		public abstract class vRemitContactID : PX.Data.IBqlField
		{
		}
		protected Int32? _VRemitContactID;
		[PXDBInt(BqlField = typeof(CRLocation.vRemitContactID))]
		[PXDBChildIdentity(typeof(Contact.contactID))]
		public virtual Int32? VRemitContactID
		{
			get
			{
				return this._VRemitContactID;
			}
			set
			{
				this._VRemitContactID = value;
			}
		}
		#endregion
		#region VPaymentMethodID
		public abstract class vPaymentMethodID : PX.Data.IBqlField
		{
		}
		protected String _VPaymentMethodID;
		[PXDBString(10, IsUnicode = true, BqlField = typeof(CRLocation.vPaymentMethodID))]
		[PXUIField(DisplayName = "Payment Method")]
		[PXSelector(typeof(Search<PaymentMethod.paymentMethodID,
							Where<PaymentMethod.useForAP, Equal<True>,
							And<PaymentMethod.isActive, Equal<True>>>>),
							DescriptionField = typeof(PaymentMethod.descr))]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual String VPaymentMethodID
		{
			get
			{
				return this._VPaymentMethodID;
			}
			set
			{
				this._VPaymentMethodID = value;
			}
		}
		#endregion
		#region VCashAccountID
		public abstract class vCashAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _VCashAccountID;
		[CashAccount(typeof(Search2<CashAccount.cashAccountID,
						InnerJoin<PaymentMethodAccount, 
							On<PaymentMethodAccount.cashAccountID,Equal<CashAccount.cashAccountID>>>,
						Where2<Match<Current<AccessInfo.userName>>, 
							And<CashAccount.clearingAccount, Equal<False>,
							And<PaymentMethodAccount.paymentMethodID,Equal<Current<LocationExtAddress.vPaymentMethodID>>,
							And<PaymentMethodAccount.useForAP,Equal<True>>>>>>), BqlField=typeof(CRLocation.vCashAccountID), 
							Visibility = PXUIVisibility.Visible)]
		public virtual Int32? VCashAccountID
		{
			get
			{
				return this._VCashAccountID;
			}
			set
			{
				this._VCashAccountID = value;
			}
		}
		#endregion
		
		#region VPaymentLeadTime
		public abstract class vPaymentLeadTime : PX.Data.IBqlField
		{
		}
		protected Int16? _VPaymentLeadTime;
		[PXDBShort(BqlField = typeof(CRLocation.vPaymentLeadTime), MinValue = -3660, MaxValue = 3660)]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "Payment Lead Time (days)")]
		public Int16? VPaymentLeadTime
		{
			get
			{
				return this._VPaymentLeadTime;
			}
			set
			{
				this._VPaymentLeadTime = value;
			}
		}
		#endregion
		#region VPaymentByType
		public abstract class vPaymentByType : PX.Data.IBqlField
		{
		}
		protected int? _VPaymentByType;
		[PXDBInt(BqlField = typeof(CRLocation.vPaymentByType))]
		[PXDefault(APPaymentBy.DueDate)]
		[APPaymentBy.List]
		[PXUIField(DisplayName = "Payment By")]
		public int? VPaymentByType
		{
			get
			{
				return this._VPaymentByType;
			}
			set
			{
				this._VPaymentByType = value;
			}
		}
		#endregion
		#region VSeparateCheck
		public abstract class vSeparateCheck : PX.Data.IBqlField
		{
		}
		protected Boolean? _VSeparateCheck;
		[PXDBBool(BqlField = typeof(CRLocation.vSeparateCheck))]
		[PXUIField(DisplayName = "Pay Separately")]
		[PXDefault(false)]
		public virtual Boolean? VSeparateCheck
		{
			get
			{
				return this._VSeparateCheck;
			}
			set
			{
				this._VSeparateCheck = value;
			}
		}
		#endregion
		#region VAPAccountLocationID
		public abstract class vAPAccountLocationID : PX.Data.IBqlField
		{
		}
		protected Int32? _VAPAccountLocationID;
		[PXDBInt(BqlField = typeof(CRLocation.vAPAccountLocationID))]
		[PXDefault()]
		public virtual Int32? VAPAccountLocationID
		{
			get
			{
				return this._VAPAccountLocationID;
			}
			set
			{
				this._VAPAccountLocationID = value;
			}
		}
		#endregion
		#region VAPAccountID
		public abstract class vAPAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _VAPAccountID;
		[Account(null, typeof(Search<Account.accountID,
                    Where2<Match<Current<AccessInfo.userName>>,
                         And<Account.active, Equal<True>,
                         And<Account.isCashAccount, Equal<False>,
                         And<Where<Current<GLSetup.ytdNetIncAccountID>, IsNull,
                          Or<Account.accountID, NotEqual<Current<GLSetup.ytdNetIncAccountID>>>>>>>>>), 
					DisplayName = "AP Account", 
					BqlField = typeof(CRLocation.vAPAccountID),
					Required = true)]
		public virtual Int32? VAPAccountID
		{
			get
			{
				return this._VAPAccountID;
			}
			set
			{
				this._VAPAccountID = value;
			}
		}
		#endregion
		#region VAPSubID
		public abstract class vAPSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _VAPSubID;

		[SubAccount(typeof(LocationExtAddress.vAPAccountID), 
					BqlField = typeof(CRLocation.vAPSubID), 
					DisplayName = "AP Sub.", 
					DescriptionField = typeof(Sub.description),
					Required = true)]
		public virtual Int32? VAPSubID
		{
			get
			{
				return this._VAPSubID;
			}
			set
			{
				this._VAPSubID = value;
			}
		}
		#endregion
		#region IsAPAccountSameAsMain
		public abstract class isAPAccountSameAsMain : PX.Data.IBqlField
		{
		}
		protected bool? _IsAPAccountSameAsMain;
		[PXBool()]
		[PXFormula(typeof(Switch<Case<Where<locationID, Equal<vAPAccountLocationID>>, False>, True>))]
		public virtual bool? IsAPAccountSameAsMain
		{
			get
			{
				return this._IsAPAccountSameAsMain;
			}
			set
			{
				this._IsAPAccountSameAsMain = value;
			}
		}
		#endregion

		//BAccount fields
		#region BAccountBAccountID
		public abstract class bAccountBAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _BAccountBAccountID;
		//should be BAccount not BAccountR
		[PXDBInt(BqlField = typeof(BAccount.bAccountID))]
		[PXExtraKey()]
		public virtual Int32? BAccountBAccountID
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		#endregion
		#region VDefAddressID
		public abstract class vDefAddressID : PX.Data.IBqlField
		{
		}
		protected Int32? _VDefAddressID;
		[PXDBInt(BqlField = typeof(BAccountR.defAddressID))]
		[PXDefault(typeof(Select<BAccount, Where<BAccount.bAccountID, Equal<Current<LocationExtAddress.bAccountID>>>>), SourceField = typeof(BAccount.defAddressID))]
		public virtual Int32? VDefAddressID
		{
			get
			{
				return this._VDefAddressID;
			}
			set
			{
				this._VDefAddressID = value;
			}
		}
		#endregion
		#region VDefContactID
		public abstract class vDefContactID : PX.Data.IBqlField
		{
		}
		protected Int32? _VDefContactID;
		[PXDBInt(BqlField = typeof(BAccountR.defContactID))]
		[PXDefault(typeof(Select<BAccount, Where<BAccount.bAccountID, Equal<Current<LocationExtAddress.bAccountID>>>>), SourceField = typeof(BAccount.defContactID))]
		public virtual Int32? VDefContactID
		{
			get
			{
				return this._VDefContactID;
			}
			set
			{
				this._VDefContactID = value;
			}
		}
		#endregion

		//Company Location Properies
		#region CMPSalesSubID
		public abstract class cMPSalesSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _CMPSalesSubID;
		[SubAccount(BqlField = typeof(CRLocation.cMPSalesSubID), DisplayName = "Sales Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? CMPSalesSubID
		{
			get
			{
				return this._CMPSalesSubID;
			}
			set
			{
				this._CMPSalesSubID = value;
			}
		}
		#endregion
		#region CMPExpenseSubID
		public abstract class cMPExpenseSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _CMPExpenseSubID;
		[SubAccount(BqlField = typeof(CRLocation.cMPExpenseSubID), DisplayName = "Expense Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? CMPExpenseSubID
		{
			get
			{
				return this._CMPExpenseSubID;
			}
			set
			{
				this._CMPExpenseSubID = value;
			}
		}
		#endregion
		#region CMPFreightSubID
		public abstract class cMPFreightSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _CMPFreightSubID;
		[SubAccount(BqlField = typeof(CRLocation.cMPFreightSubID),  DisplayName = "Freight Sub.", DescriptionField = typeof(Sub.description))]
		public virtual Int32? CMPFreightSubID
		{
			get
			{
				return this._CMPFreightSubID;
			}
			set
			{
				this._CMPFreightSubID = value;
			}
		}
		#endregion
		#region CMPDiscountSubID
		public abstract class cMPDiscountSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _CMPDiscountSubID;
		[SubAccount(BqlField = typeof(CRLocation.cMPDiscountSubID), DisplayName = "Discount Sub.", DescriptionField = typeof(Sub.description))]
		public virtual Int32? CMPDiscountSubID
		{
			get
			{
				return this._CMPDiscountSubID;
			}
			set
			{
				this._CMPDiscountSubID = value;
			}
		}
		#endregion
        #region CMPGainLossSubID
        public abstract class cMPGainLossSubID : PX.Data.IBqlField
        {
        }
        protected Int32? _CMPGainLossSubID;
        [SubAccount(BqlField = typeof(CRLocation.cMPGainLossSubID), DisplayName = "Currency Gain/Loss Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
        public virtual Int32? CMPGainLossSubID
        {
            get
            {
                return this._CMPGainLossSubID;
            }
            set
            {
                this._CMPGainLossSubID = value;
            }
        }
        #endregion
		#region CMPSiteID
		public abstract class cMPSiteID : PX.Data.IBqlField
		{
		}
		protected Int32? _CMPSiteID;
		[PXDBInt(BqlField = typeof(CRLocation.cMPSiteID))]
		[PXUIField(DisplayName = "Warehouse")]
		[PXDimensionSelector(SiteAttribute.DimensionName, typeof(INSite.siteID), typeof(INSite.siteCD), DescriptionField = typeof(INSite.descr))]
        [PXRestrictor(typeof(Where<INSite.active, Equal<True>>), IN.Messages.InactiveWarehouse, typeof(INSite.siteCD))]
        public virtual Int32? CMPSiteID
		{
			get
			{
				return this._CMPSiteID;
			}
			set
			{
				this._CMPSiteID = value;
			}
		}
		#endregion

		//Audit Fields
		#region CreatedByID
		public abstract class locationCreatedByID : PX.Data.IBqlField
		{
		}
		protected Guid? _LocationCreatedByID;
		[PXDBCreatedByID(BqlField = typeof(CRLocation.createdByID))]
		public virtual Guid? LocationCreatedByID
		{
			get
			{
				return this._LocationCreatedByID;
			}
			set
			{
				this._LocationCreatedByID = value;
			}
		}
		#endregion
		#region LocationCreatedByScreenID
		public abstract class locationCreatedByScreenID : PX.Data.IBqlField
		{
		}
		protected String _LocationCreatedByScreenID;
		[PXDBCreatedByScreenID(BqlField = typeof(CRLocation.createdByScreenID))]
		public virtual String LocationCreatedByScreenID
		{
			get
			{
				return this._LocationCreatedByScreenID;
			}
			set
			{
				this._LocationCreatedByScreenID = value;
			}
		}
		#endregion
		#region LocationCreatedDateTime
		public abstract class locationCreatedDateTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _LocationCreatedDateTime;
		[PXDBCreatedDateTime(BqlField = typeof(CRLocation.createdDateTime))]
		public virtual DateTime? LocationCreatedDateTime
		{
			get
			{
				return this._LocationCreatedDateTime;
			}
			set
			{
				this._LocationCreatedDateTime = value;
			}
		}
		#endregion
		#region LocationLastModifiedByID
		public abstract class locationLastModifiedByID : PX.Data.IBqlField
		{
		}
		protected Guid? _LocationLastModifiedByID;
		[PXDBLastModifiedByID(BqlField = typeof(CRLocation.lastModifiedByID))]
		public virtual Guid? LocationLastModifiedByID
		{
			get
			{
				return this._LocationLastModifiedByID;
			}
			set
			{
				this._LocationLastModifiedByID = value;
			}
		}
		#endregion
		#region LocationLastModifiedByScreenID
		public abstract class locationLastModifiedByScreenID : PX.Data.IBqlField
		{
		}
		protected String _LocationLastModifiedByScreenID;
		[PXDBLastModifiedByScreenID(BqlField = typeof(CRLocation.lastModifiedByScreenID))]
		public virtual String LocationLastModifiedByScreenID
		{
			get
			{
				return this._LocationLastModifiedByScreenID;
			}
			set
			{
				this._LocationLastModifiedByScreenID = value;
			}
		}
		#endregion
		#region LocationLastModifiedDateTime
		public abstract class locationLastModifiedDateTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _LocationLastModifiedDateTime;
		[PXDBLastModifiedDateTime(BqlField = typeof(CRLocation.lastModifiedDateTime))]
		public virtual DateTime? LocationLastModifiedDateTime
		{
			get
			{
				return this._LocationLastModifiedDateTime;
			}
			set
			{
				this._LocationLastModifiedDateTime = value;
			}
		}
		#endregion

		#region IsAddressSameAsMain
		public abstract class isAddressSameAsMain : PX.Data.IBqlField
		{
		}
		protected bool? _IsAddressSameAsMain;
		[PXBool()]
		[PXUIField(DisplayName = "Same as Main")]
		public virtual bool? IsAddressSameAsMain
		{
			get
			{
				return this._IsAddressSameAsMain;
			}
			set
			{
				this._IsAddressSameAsMain = value;
			}
		}
		#endregion
		#region IsContactSameAsMain
		public abstract class isContactSameAsMain : PX.Data.IBqlField
		{
		}
		protected bool? _IsContactSameAsMain;
		[PXBool()]
		[PXUIField(DisplayName = "Same as Main")]
		public virtual bool? IsContactSameAsMain
		{
			get
			{
				return this._IsContactSameAsMain;
			}
			set
			{
				this._IsContactSameAsMain = value;
			}
		}
		#endregion
		#region BAccountID
		public new abstract class bAccountID : PX.Data.IBqlField
		{
		}
		[PXExtraKey()]
		[PXDBInt()]
		[PXDBLiteDefault(typeof(BAccount.bAccountID))]
		[PXUIField(DisplayName = "Business Account ID", Visible = false, Enabled = false, Visibility = PXUIVisibility.Invisible)]
		public override Int32? BAccountID
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
		#region AddressID
		public new abstract class addressID : PX.Data.IBqlField
		{
		}
		[PXExtraKey()]
		[PXDBInt()]
		[PXUIField(DisplayName = "Address ID", Visible = false, Enabled = false, Visibility = PXUIVisibility.Invisible)]
		public override Int32? AddressID
		{
			get
			{
				return this._AddressID;
			}
			set
			{
				this._AddressID = value;
			}
		}
		#endregion
		#region CountryID
		public abstract new class countryID : PX.Data.IBqlField
		{
		}
		#endregion
		#region State
		public new abstract class state : PX.Data.IBqlField
		{
		}
		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "State")]
		[State(typeof(LocationExtAddress.countryID), DescriptionField = typeof(State.name))]
		public override String State
		{
			get
			{
				return this._State;
			}
			set
			{
				this._State = value;
			}
		}
		#endregion
		#region PostalCode
		public abstract new class postalCode : PX.Data.IBqlField
		{
		}
		#endregion
		#region NoteID
		public new abstract class noteID : PX.Data.IBqlField
		{
		}
		[PXNote()]
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
	}
}
