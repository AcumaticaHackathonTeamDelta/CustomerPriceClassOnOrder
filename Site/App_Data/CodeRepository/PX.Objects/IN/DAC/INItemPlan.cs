namespace PX.Objects.IN
{
	using System;
	using PX.Data;
	using SiteStatus = PX.Objects.IN.Overrides.INDocumentRelease.SiteStatus;
	using LocationStatus = PX.Objects.IN.Overrides.INDocumentRelease.LocationStatus;
	using LotSerialStatus = PX.Objects.IN.Overrides.INDocumentRelease.LotSerialStatus;
	using ItemLotSerial = PX.Objects.IN.Overrides.INDocumentRelease.ItemLotSerial;
    using SiteLotSerial = PX.Objects.IN.Overrides.INDocumentRelease.SiteLotSerial;
    using PX.Objects.CS;
	
	[System.SerializableAttribute()]
	public partial class INItemPlan : PX.Data.IBqlTable, IQtyPlanned
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
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[Inventory(IsKey = true)]
		[PXDefault()]
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
		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SiteID;
		[Site()]
		[PXDefault()]
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
		#region PlanDate
		public abstract class planDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _PlanDate;
		[PXDBDate()]
		[PXDefault()]
		[PXUIField(DisplayName = "Planned On")]
		public virtual DateTime? PlanDate
		{
			get
			{
				return this._PlanDate;
			}
			set
			{
				this._PlanDate = value;
			}
		}
		#endregion
		#region PlanID
		public abstract class planID : PX.Data.IBqlField
		{
		}
		protected Int64? _PlanID;
		[PXDBLongIdentity(IsKey = true)]
		public virtual Int64? PlanID
		{
			get
			{
				return this._PlanID;
			}
			set
			{
				this._PlanID = value;
			}
		}
		#endregion
		#region FixedSource
		public abstract class fixedSource : PX.Data.IBqlField
		{
		}
		protected String _FixedSource;
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Fixed Source")]
		[PXDefault(INReplenishmentSource.Purchased, PersistingCheck = PXPersistingCheck.Nothing)]
		[INReplenishmentSource.INPlanList]
		public virtual String FixedSource
		{
			get
			{
				return this._FixedSource;
			}
			set
			{
				this._FixedSource = value;
			}
		}
		#endregion
		#region Active
		public abstract class active : PX.Data.IBqlField
		{
		}
		protected bool? _Active = false;
		[PXExistance()]
		[PXUIField(DisplayName = "Active")]
		public virtual bool? Active
		{
			get
			{
				return this._Active;
			}
			set
			{
				this._Active = value;
			}
		}
		#endregion
		#region PlanType
		public abstract class planType : PX.Data.IBqlField
		{
		}
		protected String _PlanType;
		[PXDBString(2, IsFixed = true)]
		[PXDefault()]
		[PXUIField(DisplayName="Plan Type")]
		[PXSelector(typeof(Search<INPlanType.planType>), CacheGlobal = true, DescriptionField = typeof(INPlanType.descr))]
		public virtual String PlanType
		{
			get
			{
				return this._PlanType;
			}
			set
			{
				this._PlanType = value;
			}
		}
		#endregion
        #region OrigPlanID
        public abstract class origPlanID : PX.Data.IBqlField
        {
        }
        protected Int64? _OrigPlanID;
        [PXDBLong()]
        public virtual Int64? OrigPlanID
        {
            get
            {
                return this._OrigPlanID;
            }
            set
            {
                this._OrigPlanID = value;
            }
        }
        #endregion
		#region OrigPlanType
		public abstract class origPlanType : PX.Data.IBqlField
		{
		}
		protected String _OrigPlanType;
		[PXDBString(2, IsFixed = true)]
		[PXUIField(DisplayName = "Orig. Plan Type")]
		[PXSelector(typeof(Search<INPlanType.planType>), CacheGlobal = true)]
		public virtual String OrigPlanType
		{
			get
			{
				return this._OrigPlanType;
			}
			set
			{
				this._OrigPlanType = value;
			}
		}
		#endregion
        #region OrigNoteID
        public abstract class origNoteID : PX.Data.IBqlField
        {
        }
        protected Guid? _OrigNoteID;
        [PXDBGuid()]
        public virtual Guid? OrigNoteID
        {
            get
            {
                return this._OrigNoteID;
            }
            set
            {
                this._OrigNoteID = value;
            }
        }
        #endregion
        #region OrigPlanLevel
        public abstract class origPlanLevel : IBqlField { }
        [PXDBInt()]
        public int? OrigPlanLevel
        {
            get;
            set;
        }
        #endregion
        #region SubItemID
        public abstract class subItemID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubItemID;
		[SubItem()]
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
		#region LocationID
		public abstract class locationID : PX.Data.IBqlField
		{
		}
		protected Int32? _LocationID;
		[Location(typeof(INItemPlan.siteID), ValidComboRequired = false)]
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
		#region LotSerialNbr
		public abstract class lotSerialNbr : PX.Data.IBqlField
		{
		}
		protected String _LotSerialNbr;
		[LotSerialNbr]
		public virtual String LotSerialNbr
		{
			get
			{
				return this._LotSerialNbr;
			}
			set
			{
				this._LotSerialNbr = value;
			}
		}
		#endregion
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[PXDBInt()]
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
		[PXDBInt()]
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
		#region SourceSiteID
		public abstract class sourceSiteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SourceSiteID;
		[Site]		
		public virtual Int32? SourceSiteID
		{
			get
			{
				return this._SourceSiteID;
			}
			set
			{
				this._SourceSiteID = value;
			}
		}
		#endregion
		#region SupplyPlanID
		public abstract class supplyPlanID : PX.Data.IBqlField
		{
		}
		protected Int64? _SupplyPlanID;
		[PXDBLong()]
		[PXSelector(typeof(Search<INItemPlan.planID>), DirtyRead = true)]
		public virtual Int64? SupplyPlanID
		{
			get
			{
				return this._SupplyPlanID;
			}
			set
			{
				this._SupplyPlanID = value;
			}
		}
		#endregion
		#region DemandPlanID
		public abstract class demandPlanID : PX.Data.IBqlField
		{
		}
		protected Int64? _DemandPlanID;
		[PXDBLong()]
		[PXSelector(typeof(Search<INItemPlan.planID>))]
		public virtual Int64? DemandPlanID
		{
			get
			{
				return this._DemandPlanID;
			}
			set
			{
				this._DemandPlanID = value;
			}
		}
		#endregion
		#region PlanQty
		public abstract class planQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _PlanQty;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName="Planned Qty.")]
		public virtual Decimal? PlanQty
		{
			get
			{
				return this._PlanQty;
			}
			set
			{
				this._PlanQty = value;
			}
		}
		#endregion
		#region RefNoteID
		public abstract class refNoteID : PX.Data.IBqlField
		{
		}
		protected Guid? _RefNoteID;
		[PXDBGuid()]
		[PXDefault()]
		public virtual Guid? RefNoteID
		{
			get
			{
				return this._RefNoteID;
			}
			set
			{
				this._RefNoteID = value;
			}
		}
		#endregion
		#region Hold
		public abstract class hold : PX.Data.IBqlField
		{
		}
		protected Boolean? _Hold;
		[PXDBBool()]
		[PXDefault()]
		[PXUIField(DisplayName = "On Hold")]
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
		#region Reverse
		public abstract class reverse : PX.Data.IBqlField
		{
		}
		protected Boolean? _Reverse;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Reverse")]
		public virtual Boolean? Reverse
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
		#region Methods
		public static implicit operator SiteStatus(INItemPlan item)
		{
			SiteStatus ret = new SiteStatus();
			ret.InventoryID = item.InventoryID;
			ret.SubItemID = item.SubItemID;
			ret.SiteID = item.SiteID;

			return ret;
		}

		public static implicit operator LocationStatus(INItemPlan item)
		{
			LocationStatus ret = new LocationStatus();
			ret.InventoryID = item.InventoryID;
			ret.SubItemID = item.SubItemID;
			ret.SiteID = item.SiteID;
			ret.LocationID = item.LocationID;

			return ret;
		}

		public static implicit operator LotSerialStatus(INItemPlan item)
		{
			LotSerialStatus ret = new LotSerialStatus();
			ret.InventoryID = item.InventoryID;
			ret.SubItemID = item.SubItemID;
			ret.SiteID = item.SiteID;
			ret.LocationID = item.LocationID;
			ret.LotSerialNbr = item.LotSerialNbr;

			return ret;
		}

        public static implicit operator ItemLotSerial(INItemPlan item)
        {
            ItemLotSerial ret = new ItemLotSerial();
            ret.InventoryID = item.InventoryID;
            ret.LotSerialNbr = item.LotSerialNbr;

			return ret;
        }

        public static implicit operator SiteLotSerial(INItemPlan item)
        {
            SiteLotSerial ret = new SiteLotSerial();
            ret.InventoryID = item.InventoryID;
            ret.SiteID = item.SiteID;
            ret.LotSerialNbr = item.LotSerialNbr;

            return ret;
        }
		#endregion
		#region BAccountID
		public abstract class bAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _BAccountID;
		[PXDBInt()]
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
	}
}
