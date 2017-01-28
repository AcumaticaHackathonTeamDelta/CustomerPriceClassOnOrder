using System;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.TX;
using PX.Objects.SO;
using PX.Objects.PM;
using PX.Objects.GL;

namespace PX.Objects.CR
{
    [PXCacheName(Messages.OpportunityProducts)]
    [Serializable]
    public partial class CROpportunityProducts : IBqlTable, IDiscountable
    {
        #region CROpportunityProductID
        public abstract class cROpportunityProductID : IBqlField { }

        [PXDBInt(IsKey = true)]
        [PXLineNbr(typeof(CROpportunity.productCntr))]
        public virtual Int32? CROpportunityProductID { get; set; }
        #endregion
        #region CROpportunityID
        public abstract class cROpportunityID : IBqlField { }

        [PXDBString(10, IsUnicode = true, IsKey = true)]
        [PXDBDefault(typeof(CROpportunity.opportunityID))]
        [PXParent(typeof(Select<CROpportunity,
            Where<CROpportunity.opportunityID, Equal<Current<CROpportunityProducts.cROpportunityID>>>>))]
        public virtual String CROpportunityID { get; set; }

        #endregion

        #region CuryInfoID
        public abstract class curyInfoID : IBqlField { }
        [PXDBLong]
        [CurrencyInfo(typeof(CROpportunity.curyInfoID))]
        public virtual Int64? CuryInfoID { get; set; }
        #endregion

        #region InventoryID
        public abstract class inventoryID : IBqlField { }

        [Inventory(Filterable = true)]
        public virtual Int32? InventoryID { get; set; }
        #endregion

        #region Quantity
        public abstract class quantity : IBqlField { }
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXDBQuantity]
        [PXUIField(DisplayName = "Quantity", Visibility = PXUIVisibility.Visible)]
        public virtual Decimal? Quantity { get; set; }
        #endregion

        #region CuryUnitPrice
        public abstract class curyUnitPrice : IBqlField { }

        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXDBCurrency(typeof(CommonSetup.decPlPrcCst), typeof(CROpportunityProducts.curyInfoID), typeof(CROpportunityProducts.unitPrice))]
        [PXUIField(DisplayName = "Unit Price", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual Decimal? CuryUnitPrice { get; set; }
        #endregion

        #region UnitPrice
        public abstract class unitPrice : IBqlField { }

        [PXDBDecimal(6)]
        [PXDefault(typeof(Search<InventoryItem.basePrice, Where<InventoryItem.inventoryID, Equal<Current<CROpportunityProducts.inventoryID>>>>))]
        public virtual Decimal? UnitPrice { get; set; }
        #endregion

        #region CuryExtPrice
        public abstract class curyExtPrice : IBqlField { }

        [PXDBCurrency(typeof(CROpportunityProducts.curyInfoID), typeof(CROpportunityProducts.extPrice))]
        [PXUIField(DisplayName = "Ext. Price")]
        [PXFormula(typeof(Mult<CROpportunityProducts.quantity, CROpportunityProducts.curyUnitPrice>))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? CuryExtPrice { get; set; }
        #endregion

        #region ExtPrice
        public abstract class extPrice : IBqlField { }

        [PXDBDecimal(4)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? ExtPrice { get; set; }
        #endregion

        #region CuryAmount
        public abstract class curyAmount : IBqlField { }

        [PXDBCurrency(typeof(CROpportunityProducts.curyInfoID), typeof(CROpportunityProducts.amount))]
        [PXUIField(DisplayName = "Amount", Enabled = false)]
        [PXFormula(typeof(Sub<CROpportunityProducts.curyExtPrice, CROpportunityProducts.curyDiscAmt>), null)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? CuryAmount { get; set; }
        #endregion

        #region Amount
        public abstract class amount : IBqlField { }

        [PXDBDecimal(4)]
        public virtual Decimal? Amount { get; set; }
        #endregion

        #region CustomerID
        public abstract class customerID : IBqlField { }

        [PXDBInt]
        [PXDBDefault(typeof(CROpportunity.bAccountID), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Int32? CustomerID { get; set; }
        #endregion

        #region TransactionDescription
        public abstract class transactionDescription : IBqlField { }

        [PXDBLocalizableString(60, IsUnicode = true)]
        [PXUIField(DisplayName = "Transaction Description", Visibility = PXUIVisibility.Visible)]
        public virtual String TransactionDescription { get; set; }
        #endregion

        #region SubItemID
        public abstract class subItemID : IBqlField { }

        [SubItem(typeof(CROpportunityProducts.inventoryID))]
        public virtual Int32? SubItemID { get; set; }
        #endregion

        #region UOM
        public abstract class uOM : IBqlField { }

        [PXDefault(typeof(Search<InventoryItem.salesUnit, Where<InventoryItem.inventoryID, Equal<Current<CROpportunityProducts.inventoryID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [INUnit(typeof(CROpportunityProducts.inventoryID))]
        public virtual String UOM { get; set; }
        #endregion

        #region TaxCategoryID
        public abstract class taxCategoryID : PX.Data.IBqlField
        {
        }
        protected String _TaxCategoryID;
        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "Tax Category", Visibility = PXUIVisibility.Visible)]
        [PXSelector(typeof(TaxCategory.taxCategoryID), DescriptionField = typeof(TaxCategory.descr))]
        [PXDefault(typeof(Search<InventoryItem.taxCategoryID,
            Where<InventoryItem.inventoryID, Equal<Current<CROpportunityProducts.inventoryID>>>>),
            PersistingCheck = PXPersistingCheck.Nothing)]
        [CRTax(typeof(CROpportunity), typeof(CROpportunityTax), typeof(CRTaxTran), TaxCalc = TaxCalc.ManualLineCalc)]
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

        #region ProjectID
        public abstract class projectID : IBqlField { }
        [PXDBInt]
        [PXDefault(typeof(CROpportunity.projectID), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Int32? ProjectID { get; set; }
        #endregion

        #region TaskID
        public abstract class taskID : IBqlField { }

        [ActiveProjectTask(typeof(CROpportunityProducts.projectID), BatchModule.CR, DisplayName = "Project Task")]
        public virtual Int32? TaskID { get; set; }
        #endregion

        #region DiscPct
        public abstract class discPct : IBqlField { }

        [PXDBDecimal(6, MinValue = -100, MaxValue = 100)]
        [PXUIField(DisplayName = "Discount, %")]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? DiscPct { get; set; }

        #endregion

        #region CuryDiscAmt
        public abstract class curyDiscAmt : IBqlField { }

        [PXDBCurrency(typeof(CommonSetup.decPlPrcCst), typeof(CROpportunityProducts.curyInfoID), typeof(CROpportunityProducts.discAmt), MinValue = 0)]
        [PXUIField(DisplayName = "Discount Amount")]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? CuryDiscAmt { get; set; }
        #endregion

        #region DiscAmt
        public abstract class discAmt : IBqlField { }

        [PXDBDecimal(4)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? DiscAmt { get; set; }
        #endregion

        #region ManualDisc
        public abstract class manualDisc : IBqlField { }

        [SOManualDiscMode(typeof(CROpportunityProducts.curyDiscAmt), typeof(CROpportunityProducts.discPct))]
        [PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Manual Discount", Visibility = PXUIVisibility.Visible)]
        public virtual Boolean? ManualDisc { get; set; }
        #endregion

        #region IsFree
        public abstract class isFree : IBqlField { }

        [PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Free Item")]
        public virtual Boolean? IsFree { get; set; }
        #endregion

        #region ManualPrice
        public abstract class manualPrice : PX.Data.IBqlField
        {
        }
        protected Boolean? _ManualPrice;
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Manual Price")]
        public virtual Boolean? ManualPrice
        {
            get
            {
                return this._ManualPrice;
            }
            set
            {
                this._ManualPrice = value;
            }
        }
        #endregion

        #region DetDiscIDC1
        public abstract class detDiscIDC1 : IBqlField { }

        [PXDBString(10, IsUnicode = true)]
        public virtual String DetDiscIDC1 { get; set; }
        #endregion

        #region DetDiscSeqIDC1
        public abstract class detDiscSeqIDC1 : IBqlField { }

        [PXDBString(10, IsUnicode = true)]
        public virtual String DetDiscSeqIDC1 { get; set; }
        #endregion

        #region DetDiscIDC2
        public abstract class detDiscIDC2 : IBqlField { }

        [PXDBString(10, IsUnicode = true)]
        public virtual String DetDiscIDC2 { get; set; }
        #endregion

        #region DetDiscSeqIDC2
        public abstract class detDiscSeqIDC2 : IBqlField { }

        [PXDBString(10, IsUnicode = true)]
        public virtual String DetDiscSeqIDC2 { get; set; }
        #endregion

        #region DetDiscApp
        public abstract class detDiscApp : IBqlField { }

        [PXDBBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Boolean? DetDiscApp { get; set; }
        #endregion

        #region PromoDiscID
        public virtual string PromoDiscID
        {
            get
            {
                return null;
            }
            set { }
        }
        #endregion

        #region DocDiscIDC1
        public abstract class docDiscIDC1 : IBqlField { }

        [PXDBString(10, IsUnicode = true)]
        public virtual String DocDiscIDC1 { get; set; }
        #endregion

        #region DocDiscSeqIDC1
        public abstract class docDiscSeqIDC1 : IBqlField { }

        [PXDBString(10, IsUnicode = true)]
        public virtual String DocDiscSeqIDC1 { get; set; }
        #endregion

        #region DocDiscIDC2
        public abstract class docDiscIDC2 : IBqlField { }

        [PXDBString(10, IsUnicode = true)]
        public virtual String DocDiscIDC2 { get; set; }
        #endregion

        #region DocDiscSeqIDC2
        public abstract class docDiscSeqIDC2 : IBqlField { }

        [PXDBString(10, IsUnicode = true)]
        public virtual String DocDiscSeqIDC2 { get; set; }
        #endregion

        #region SiteID
        public abstract class siteID : IBqlField { }

        [POSiteAvail(typeof(CROpportunityProducts.inventoryID), typeof(CROpportunityProducts.subItemID))]
        [PXDefault(typeof(
            Coalesce<
            Search<Location.cSiteID, Where<Location.bAccountID, Equal<Current<CROpportunity.bAccountID>>, And<Location.locationID, Equal<Current<CROpportunity.locationID>>>>>,
            Search<InventoryItem.dfltSiteID, Where<InventoryItem.inventoryID, Equal<Current<CROpportunityProducts.inventoryID>>>>>),
            PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Int32? SiteID { get; set; }
        #endregion

        #region NoteID
        public abstract class noteID : IBqlField { }

        [PXNote]
        public virtual Guid? NoteID { get; set; }
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

        #region IDiscountable Members

        public decimal? Qty
        {
            get { return Quantity; }
        }

        public decimal? CuryLineAmt
        {
            get
            {
                return CuryAmount;
            }
            set
            {
                CuryAmount = value; ;
            }
        }

        #endregion

        #region Special Fields
        public virtual bool? OpportunityLocationIDChanged { get; set; }
        public virtual bool? OpportunityCloseDateChanged { get; set; }
        #endregion
    }
}
