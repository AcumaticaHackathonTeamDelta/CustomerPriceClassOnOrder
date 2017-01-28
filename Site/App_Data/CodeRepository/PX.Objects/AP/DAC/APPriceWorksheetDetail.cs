namespace PX.Objects.AP
{
    using System;
    using PX.Data;
    using PX.Objects.IN;
    using PX.Objects.CM;
    using PX.Objects.TX;
    using PX.Objects.CS;
    using System.Collections;
    using System.Collections.Generic;

    [System.SerializableAttribute()]
	[PXCacheName(Messages.APPriceWorksheetDetail)]
	public partial class APPriceWorksheetDetail : PX.Data.IBqlTable
    {
        #region RefNbr
        public abstract class refNbr : PX.Data.IBqlField
        {
        }
        protected String _RefNbr;
        [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXDBDefault(typeof(APPriceWorksheet.refNbr))]
        [PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible, TabOrder = 1)]
        [PXParent(typeof(Select<APPriceWorksheet, Where<APPriceWorksheet.refNbr, Equal<Current<APPriceWorksheetDetail.refNbr>>>>))]
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
        #region LineID
        public abstract class lineID : PX.Data.IBqlField
        {
        }
        protected Int32? _LineID;
        [PXDBIdentity(IsKey = true)]
        public virtual Int32? LineID
        {
            get
            {
                return this._LineID;
            }
            set
            {
                this._LineID = value;
            }
        }
        #endregion
        #region VendorID
        public abstract class vendorID : PX.Data.IBqlField
        {
        }
        protected Int32? _VendorID;
        [Vendor]
        [PXDefault]
        [PXParent(typeof(Select<Vendor, Where<Vendor.bAccountID, Equal<Current<APVendorPrice.vendorID>>>>))]
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
        #region InventoryID
        public abstract class inventoryID : PX.Data.IBqlField
        {
        }
        protected Int32? _InventoryID;
        [Inventory(DisplayName = "Inventory ID")]
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
        #region SubItemID
        public abstract class subItemID : PX.Data.IBqlField
        {
        }
        protected Int32? _SubItemID;
        [PXDefault(typeof(Search<InventoryItem.defaultSubItemID,
            Where<InventoryItem.inventoryID, Equal<Current<APPriceWorksheetDetail.inventoryID>>,
            And<InventoryItem.defaultSubItemOnEntry, Equal<True>>>>),
            PersistingCheck = PXPersistingCheck.Nothing)]
        [SubItem(typeof(APPriceWorksheetDetail.inventoryID))]
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
        [PXString(256, IsUnicode = true)]
        [PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.Visible)]
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
        [PXDefault(typeof(Search<InventoryItem.purchaseUnit, Where<InventoryItem.inventoryID, Equal<Current<APPriceWorksheetDetail.inventoryID>>>>))]
        [INUnit(typeof(APPriceWorksheetDetail.inventoryID))]
        [PXFormula(typeof(Selector<APPriceWorksheetDetail.inventoryID, InventoryItem.purchaseUnit>))]
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
        #region BreakQty
        public abstract class breakQty : PX.Data.IBqlField
        {
        }
        protected Decimal? _BreakQty;
        [PXDBQuantity(MinValue = 0)]
        [PXUIField(DisplayName = "Break Qty", Visibility = PXUIVisibility.Visible, Enabled = true)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? BreakQty
        {
            get
            {
                return this._BreakQty;
            }
            set
            {
                this._BreakQty = value;
            }
        }
        #endregion
        #region CurrentPrice
        public abstract class currentPrice : PX.Data.IBqlField
        {
        }
        protected Decimal? _CurrentPrice;
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXDBPriceCost]
        [PXUIField(DisplayName = "Source Price", Visibility = PXUIVisibility.Visible, Enabled = false)]
        public virtual Decimal? CurrentPrice
        {
            get
            {
                return this._CurrentPrice;
            }
            set
            {
                this._CurrentPrice = value;
            }
        }
        #endregion
        #region PendingPrice
        public abstract class pendingPrice : PX.Data.IBqlField
        {
        }
        protected Decimal? _PendingPrice;
        [PXDBDecimal(6)]
        [PXUIField(DisplayName = "Pending Price", Visibility = PXUIVisibility.Visible)]
        public virtual Decimal? PendingPrice
        {
            get
            {
                return this._PendingPrice;
            }
            set
            {
                this._PendingPrice = value;
            }
        }
        #endregion
        #region CuryID
        public abstract class curyID : PX.Data.IBqlField
        {
        }
        protected string _CuryID;
        [PXDBString(5)]
        [PXDefault(typeof(Search<GL.Company.baseCuryID>))]
        [PXSelector(typeof(Currency.curyID), CacheGlobal = true)]
        [PXUIField(DisplayName = "Currency")]
        public virtual string CuryID
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
        #region TaxID
        public abstract class taxID : PX.Data.IBqlField
        {
        }
        protected String _TaxID;
        [PXUIField(DisplayName = "Tax", Visibility = PXUIVisibility.Visible, Enabled = true)]
		[PXSelector(typeof(Tax.taxID), DescriptionField = typeof(Tax.descr))]
        [PXDBString(Tax.taxID.Length)]
        public virtual String TaxID
        {
            get
            {
                return this._TaxID;
            }
            set
            {
                this._TaxID = value;
            }
        }
        #endregion
        #region System Columns
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
    }
}
