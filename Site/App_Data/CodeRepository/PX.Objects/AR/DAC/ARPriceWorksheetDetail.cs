namespace PX.Objects.AR
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
	[PXCacheName(Messages.ARPriceWorksheetDetail)]
	public partial class ARPriceWorksheetDetail : PX.Data.IBqlTable
    {
        #region RefNbr
        public abstract class refNbr : PX.Data.IBqlField
        {
        }
        protected String _RefNbr;
        [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXDBDefault(typeof(ARPriceWorksheet.refNbr))]
        [PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible, TabOrder = 1)]
        [PXParent(typeof(Select<ARPriceWorksheet, Where<ARPriceWorksheet.refNbr, Equal<Current<ARPriceWorksheetDetail.refNbr>>>>))]
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
        #region PriceType
        public abstract class priceType : PX.Data.IBqlField
        {
        }
        protected String _PriceType;
        [PXDBString(1, IsFixed = true)]
        [PXDefault(PriceTypeList.CustomerPriceClass)]
        [PriceTypeList.List()]
        [PXUIField(DisplayName = "Price Type", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual String PriceType
        {
            get
            {
                return this._PriceType;
            }
            set
            {
                this._PriceType = value;
            }
        }
        #endregion
        #region PriceCode
        public abstract class priceCode : PX.Data.IBqlField
        {
        }
        protected String _PriceCode;
        [PXString(30, InputMask = ">CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Price Code", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(ARPriceWorksheetDetail.priceCode), new Type[] {typeof(ARPriceWorksheetDetail.priceCode), typeof(ARPriceWorksheetDetail.description)}, ValidateValue=false )]
        public virtual String PriceCode
        {
            get
            {
                return this._PriceCode;
            }
            set
            {
                this._PriceCode = value;
            }
        }
        #endregion
        #region CustPriceClassID
        public abstract class custPriceClassID : PX.Data.IBqlField
        {
        }
        protected String _CustPriceClassID;
        [PXDBString(10, InputMask = ">aaaaaaaaaa")]
        [PXDefault(PersistingCheck=PXPersistingCheck.Nothing)]
        //[PXUIField(DisplayName = "Customer Price Class", Visibility = PXUIVisibility.SelectorVisible)]
        [CustomerPriceClass]
        public virtual String CustPriceClassID
        {
            get
            {
                return this._CustPriceClassID;
            }
            set
            {
                this._CustPriceClassID = value;
            }
        }
        #endregion
        #region CustomerID
        public abstract class customerID : PX.Data.IBqlField
        {
        }
        protected Int32? _CustomerID;
        [Customer(Visible=false, PersistingCheck=PXPersistingCheck.Nothing)]
        [PXParent(typeof(Select<Customer, Where<Customer.bAccountID, Equal<Current<ARSalesPrice.customerID>>>>))]
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
        #region InventoryID
        public abstract class inventoryID : PX.Data.IBqlField
        {
        }
        protected Int32? _InventoryID;
        [Inventory(DisplayName = "Inventory ID")]
        [PXParent(typeof(Select<InventoryItem, Where<InventoryItem.inventoryID, Equal<Current<ARPriceWorksheetDetail.inventoryID>>>>))]
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
            Where<InventoryItem.inventoryID, Equal<Current<ARPriceWorksheetDetail.inventoryID>>,
            And<InventoryItem.defaultSubItemOnEntry, Equal<True>>>>),
            PersistingCheck = PXPersistingCheck.Nothing)]
        [SubItem(typeof(ARPriceWorksheetDetail.inventoryID))]
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
        [PXDefault(typeof(Search<InventoryItem.salesUnit, Where<InventoryItem.inventoryID, Equal<Current<ARPriceWorksheetDetail.inventoryID>>>>))]
        [INUnit(typeof(ARPriceWorksheetDetail.inventoryID))]
        [PXFormula(typeof(Selector<ARPriceWorksheetDetail.inventoryID, InventoryItem.salesUnit>))]
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
    public static class PriceTypeList
    {
        public class ListAttribute : PXStringListAttribute
        {
            public ListAttribute()
                : base(
                new string[] { BasePrice, CustomerPriceClass, Customer },
                new string[] { Messages.BasePrice, Messages.CustomerPriceClass, Messages.Customer }) { ; }
        }
        public const string CustomerPriceClass = "P";
        public const string Customer = "C";
        public const string Vendor = "V";
        public const string BasePrice = "B";

        public class customerPriceClass : Constant<string>
        {
            public customerPriceClass() : base(PriceTypeList.CustomerPriceClass) { ;}
        }

        public class customer : Constant<string>
        {
            public customer() : base(PriceTypeList.Customer) { ;}
        }

        public class vendor : Constant<string>
        {
            public vendor() : base(PriceTypeList.Vendor) { ;}
        }

        public class basePrice : Constant<string>
        {
            public basePrice() : base(PriceTypeList.BasePrice) { ;}
        }
    }
}
