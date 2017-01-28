namespace PX.Objects.PM
{
    using System;
    using PX.Data;
    using PX.Objects.CM;
    using PX.Objects.IN;
    using PX.Objects.CS;
    using PX.Objects.DR;
    using PX.Objects.AR;
    using PX.Objects.GL;
    using PX.Objects.CT;

    [System.SerializableAttribute()]
    //[PXCacheName(Messages.PMDetail)]
    [PXProjection(typeof(Select2<PMDetail,
        InnerJoin<Contract, On<PMDetail.contractID, Equal<Contract.contractID>, And<PMDetail.revID, Equal<Contract.revID>>>,
        LeftJoin<PMDetailExt, On<PMDetailExt.contractID, Equal<PMDetail.contractID>, And<PMDetailExt.lineNbr, Equal<PMDetail.lineNbr>, And<PMDetailExt.revID, Equal<Contract.lastActiveRevID>>>>>>>), new Type[] { typeof(PMDetail) },
        Persistent = true)]
    public partial class PMDetail : PX.Data.IBqlTable
    {
        #region PMDetailID
        public abstract class pMDetailID : PX.Data.IBqlField
        {
        }
        protected Int32? _PMDetailID;
        [PXDBIdentity()]
        public virtual Int32? PMDetailID
        {
            get
            {
                return this._PMDetailID;
            }
            set
            {
                this._PMDetailID = value;
            }
        }
        #endregion
        #region ContractID
        public abstract class contractID : PX.Data.IBqlField
        {
        }
        protected Int32? _ContractID;
        [PXDBInt(IsKey = true)]
        [PXDBDefault(typeof(Contract.contractID))]
        [PXParent(typeof(Select<Contract, Where<Contract.contractID, Equal<Current<PMDetail.contractID>>>>))]
        [PXParent(typeof(Select<ContractBillingSchedule, Where<ContractBillingSchedule.contractID, Equal<Current<PMDetail.contractID>>>>))]
        public virtual Int32? ContractID
        {
            get
            {
                return this._ContractID;
            }
            set
            {
                this._ContractID = value;
            }
        }
        #endregion
        #region LineNbr
        public abstract class lineNbr : PX.Data.IBqlField
        {
        }
        protected Int32? _LineNbr;
        [PXDBInt(IsKey = true)]
        [ContractLineNbr(typeof(Contract.lineCtr))]
        [PXUIField(DisplayName = "Line Nbr.", Visible = false)]
        public virtual Int32? LineNbr
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
        #region RevID
        public abstract class revID : PX.Data.IBqlField
        {
        }
        [PXDBInt(MinValue = 1)]
        [PXDefault(typeof(Contract.revID), PersistingCheck = PXPersistingCheck.Null)]
        public virtual int? RevID { get; set; }
        #endregion
        #region TaskID
        public abstract class taskID : PX.Data.IBqlField
        {
        }
        protected Int32? _TaskID;
        [PXDefault(0)]
        [PXDBInt]
        public virtual Int32? TaskID
        {
            get
            {
                return this._TaskID;
            }
            set
            {
                this._TaskID = value;
            }
        }
        #endregion
        #region InventoryID
        public abstract class inventoryID : PX.Data.IBqlField
        {
        }
        protected Int32? _InventoryID;
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXDBInt()]
        [PXUIField(DisplayName = "Non-Stock Item")]
        [PXDimensionSelector(InventoryAttribute.DimensionName, typeof(Search<InventoryItem.inventoryID, Where<InventoryItem.stkItem, Equal<False>, And<Match<Current<AccessInfo.userName>>>>>), typeof(InventoryItem.inventoryCD))]
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
        #region ContractItemID
        public abstract class contractItemID : PX.Data.IBqlField
        {
        }
        protected Int32? _ContractItemID;
        [PXDBInt()]
        [PXDefault()]
        [PXDimensionSelector(ContractItemAttribute.DimensionName, typeof(Search<ContractItem.contractItemID>),
                                                                    typeof(ContractItem.contractItemCD),
                                                                    typeof(ContractItem.contractItemCD), typeof(ContractItem.descr))]
        [PXUIField(DisplayName = "Item Code")]
        public virtual Int32? ContractItemID
        {
            get
            {
                return this._ContractItemID;
            }
            set
            {
                this._ContractItemID = value;
            }
        }
        #endregion
        #region CuryID
        public abstract class curyID : PX.Data.IBqlField
        {
        }
        #endregion
        #region CuryInfoID
        public abstract class curyInfoID : PX.Data.IBqlField
        {
        }
        protected Int64? _CuryInfoID;
        [PXDBLong()]
        [CurrencyInfo(ModuleCode = "CT")]
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
        #region ItemFee
        public abstract class itemFee : PX.Data.IBqlField
        {
        }
        protected Decimal? _ItemFee;
        [PXDBDecimal(typeof(Search<CommonSetup.decPlPrcCst>))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? ItemFee
        {
            get
            {
                return this._ItemFee;
            }
            set
            {
                this._ItemFee = value;
            }
        }
        #endregion
        #region CuryItemFee
        public abstract class curyItemFee : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryItemFee;
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXDBCurrency(typeof(PMDetail.curyInfoID), typeof(PMDetail.itemFee))]
        [PXUIField(DisplayName = "Item Fee")]
        public virtual Decimal? CuryItemFee
        {
            get
            {
                return this._CuryItemFee;
            }
            set
            {
                this._CuryItemFee = value;
            }
        }
        #endregion
        #region AccountSource
        public abstract class accountSource : PX.Data.IBqlField
        {
        }
        protected String _AccountSource;
        [PXDBString(1, IsFixed = true)]
        public virtual String AccountSource
        {
            get
            {
                return this._AccountSource;
            }
            set
            {
                this._AccountSource = value;
            }
        }
        #endregion
        #region AccountID
        public abstract class accountID : PX.Data.IBqlField
        {
        }
        protected Int32? _AccountID;
        [PXDBInt()]
        public virtual Int32? AccountID
        {
            get
            {
                return this._AccountID;
            }
            set
            {
                this._AccountID = value;
            }
        }
        #endregion
        #region SubMask
        public abstract class subMask : PX.Data.IBqlField
        {
        }
        protected String _SubMask;
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        public virtual String SubMask
        {
            get
            {
                return this._SubMask;
            }
            set
            {
                this._SubMask = value;
            }
        }
        #endregion
        #region SubID
        public abstract class subID : PX.Data.IBqlField
        {
        }
        protected Int32? _SubID;
        [PXDBInt()]
        public virtual Int32? SubID
        {
            get
            {
                return this._SubID;
            }
            set
            {
                this._SubID = value;
            }
        }
        #endregion
        #region Description
        public abstract class description : PX.Data.IBqlField
        {
        }
        protected String _Description;
        [PXDBString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
        [PXFormula(typeof(Selector<PMDetail.contractItemID, ContractItem.descr>))]
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
        #region ResetUsage
        public abstract class resetUsage : PX.Data.IBqlField
        {
        }
        protected string _ResetUsage;
        [PXDefault(ResetUsageOption.Never)]
        [PXUIField(DisplayName = "Reset Usage", Required = true)]
        [PXDBString(1, IsFixed = true)]
        [ResetUsageOption.List()]
        public virtual string ResetUsage
        {
            get
            {
                return this._ResetUsage;
            }
            set
            {
                this._ResetUsage = value;
            }
        }
        #endregion
        #region Included
        public abstract class included : PX.Data.IBqlField
        {
        }
        protected Decimal? _Included;
        [PXDBQuantity]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Included", Visibility = PXUIVisibility.Visible)]
        public virtual Decimal? Included
        {
            get
            {
                return this._Included;
            }
            set
            {
                this._Included = value;
            }
        }
        #endregion
        #region Used
        public abstract class used : PX.Data.IBqlField
        {
        }
        protected Decimal? _Used;
        [PXDBQuantity]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Used", Visibility = PXUIVisibility.Visible, Enabled = false)]
        public virtual Decimal? Used
        {
            get
            {
                return this._Used;
            }
            set
            {
                this._Used = value;
            }
        }
        #endregion
        #region UsedTotal
        public abstract class usedTotal : PX.Data.IBqlField
        {
        }
        protected Decimal? _UsedTotal;
        [PXDBQuantity]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Used Total", Visibility = PXUIVisibility.Visible, Enabled = false)]
        public virtual Decimal? UsedTotal
        {
            get
            {
                return this._UsedTotal;
            }
            set
            {
                this._UsedTotal = value;
            }
        }
        #endregion
        #region UOM
        public abstract class uOM : PX.Data.IBqlField
        {
        }
        protected String _UOM;
        [PXDefault(typeof(Search<InventoryItem.salesUnit, Where<InventoryItem.inventoryID, Equal<Current<PMDetail.inventoryID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [INUnit(typeof(PMDetail.inventoryID))]
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
        #region LastBilledDate
        public abstract class lastBilledDate : PX.Data.IBqlField
        {
        }
        protected DateTime? _LastBilledDate;
        [PXDBDate()]
        [PXUIField(DisplayName = "Last Billed Date", Enabled = false)]
        public virtual DateTime? LastBilledDate
        {
            get
            {
                return this._LastBilledDate;
            }
            set
            {
                this._LastBilledDate = value;
            }
        }
        #endregion
        #region LastBilledQty
        public abstract class lastBilledQty : PX.Data.IBqlField
        {
        }
        protected Decimal? _LastBilledQty;
        [PXDBQuantity]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Last Billed Qty.", Enabled = false)]
        public virtual Decimal? LastBilledQty
        {
            get
            {
                return this._LastBilledQty;
            }
            set
            {
                this._LastBilledQty = value;
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

    [System.SerializableAttribute()]
    [PXProjection(typeof(Select<PMDetailExt>), Persistent = true)]
    [PXBreakInheritance()]
    [PXHidden]
    public partial class PMDetailExt : PMDetail
    {
        #region PMDetailID
        new public abstract class pMDetailID : PX.Data.IBqlField
        {
        }
        new protected Int32? _PMDetailID;
        [PXDBIdentity(IsKey = true)]
        public override Int32? PMDetailID
        {
            get
            {
                return this._PMDetailID;
            }
            set
            {
                this._PMDetailID = value;
            }
        }
        #endregion
        #region ContractID
        new public abstract class contractID : PX.Data.IBqlField
        {
        }
        new protected Int32? _ContractID;
        #endregion
        #region LineNbr
        new public abstract class lineNbr : PX.Data.IBqlField
        {
        }
        new protected Int32? _LineNbr;
        [PXDBInt()]
        [PXLineNbr(typeof(Contract.lineCtr))]
        [PXUIField(DisplayName = "Line Nbr.", Visible = false)]
        public override Int32? LineNbr
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
        #region RevID
        new public abstract class revID : PX.Data.IBqlField
        {
        }
        [PXDBInt(MinValue = 1, IsKey = true)]
        [PXDefault(typeof(Contract.revID), PersistingCheck = PXPersistingCheck.Null)]
        public override int? RevID { get; set; }
        #endregion
        #region ContractItemID
        new public abstract class contractItemID : PX.Data.IBqlField
        {
        }
        #endregion
    }

    [System.SerializableAttribute()]
    [PMDetailAccumAttribute]
    [PXBreakInheritance()]
    public partial class PMDetailAcum : PMDetail
    {
        #region PMDetailID
        public new abstract class pMDetailID : PX.Data.IBqlField
        {
        }
        [PXDBInt(IsKey = true)]
        public override Int32? PMDetailID
        {
            get
            {
                return this._PMDetailID;
            }
            set
            {
                this._PMDetailID = value;
            }
        }
        #endregion
        #region ContractID
        public new abstract class contractID : PX.Data.IBqlField
        {
        }
        [PXDBInt()]
        public override Int32? ContractID
        {
            get
            {
                return this._ContractID;
            }
            set
            {
                this._ContractID = value;
            }
        }
        #endregion
        #region LineNbr
        public new abstract class lineNbr : PX.Data.IBqlField
        {
        }
        [PXDBInt()]
        public override Int32? LineNbr
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
        #region RevID
        public new abstract class revID : PX.Data.IBqlField
        {
        }
        [PXDBInt()]
        public override int? RevID { get; set; }
        #endregion
        #region TaskID
        public new abstract class taskID : PX.Data.IBqlField
        {
        }
        [PXDBInt()]
        public override Int32? TaskID
        {
            get
            {
                return this._TaskID;
            }
            set
            {
                this._TaskID = value;
            }
        }
        #endregion
        #region InventoryID
        public new abstract class inventoryID : PX.Data.IBqlField
        {
        }

        [PXDBInt()]
        public override Int32? InventoryID
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
        #region CuryInfoID
        public new abstract class curyInfoID : PX.Data.IBqlField
        {
        }
        [PXDBLong()]
        [CurrencyInfo(ModuleCode = "CT")]
        public override Int64? CuryInfoID
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
        #region ItemFee
        public new abstract class itemFee : PX.Data.IBqlField
        {
        }
        [PXDBDecimal(4)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public override Decimal? ItemFee
        {
            get
            {
                return this._ItemFee;
            }
            set
            {
                this._ItemFee = value;
            }
        }
        #endregion
        #region CuryItemFee
        public new abstract class curyItemFee : PX.Data.IBqlField
        {
        }
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXDBCurrency(typeof(PMDetail.curyInfoID), typeof(PMDetail.itemFee))]
        [PXUIField(DisplayName = "Item Fee")]
        public override Decimal? CuryItemFee
        {
            get
            {
                return this._CuryItemFee;
            }
            set
            {
                this._CuryItemFee = value;
            }
        }
        #endregion
        #region AccountID
        public new abstract class accountID : PX.Data.IBqlField
        {
        }
        [PXDBInt()]
        public override Int32? AccountID
        {
            get
            {
                return this._AccountID;
            }
            set
            {
                this._AccountID = value;
            }
        }
        #endregion
        #region SubID
        public new abstract class subID : PX.Data.IBqlField
        {
        }
        [PXDBInt()]
        public override Int32? SubID
        {
            get
            {
                return this._SubID;
            }
            set
            {
                this._SubID = value;
            }
        }
        #endregion
        #region Description
        public new abstract class description : PX.Data.IBqlField
        {
        }
        [PXDBString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
        public override String Description
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
        #region ResetUsage
        public new abstract class resetUsage : PX.Data.IBqlField
        {
        }
        [PXDefault(ResetUsageOption.Never)]
        [PXUIField(DisplayName = "Reset Usage")]
        [PXDBString(1, IsFixed = true)]
        [ResetUsageOption.List()]
        public override string ResetUsage
        {
            get
            {
                return this._ResetUsage;
            }
            set
            {
                this._ResetUsage = value;
            }
        }
        #endregion
        #region Included
        public new abstract class included : PX.Data.IBqlField
        {
        }
        [PXDBQuantity]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Included", Visibility = PXUIVisibility.Visible)]
        public override Decimal? Included
        {
            get
            {
                return this._Included;
            }
            set
            {
                this._Included = value;
            }
        }
        #endregion
        #region Used
        public new abstract class used : PX.Data.IBqlField
        {
        }
        [PXDBQuantity]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Used", Visibility = PXUIVisibility.Visible, Enabled = false)]
        public override Decimal? Used
        {
            get
            {
                return this._Used;
            }
            set
            {
                this._Used = value;
            }
        }
        #endregion
        #region UsedTotal
        public new abstract class usedTotal : PX.Data.IBqlField
        {
        }
        [PXDBQuantity]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Used Total", Visibility = PXUIVisibility.Visible, Enabled = false)]
        public override Decimal? UsedTotal
        {
            get
            {
                return this._UsedTotal;
            }
            set
            {
                this._UsedTotal = value;
            }
        }
        #endregion
        #region UOM
        public new abstract class uOM : PX.Data.IBqlField
        {
        }
        [PXDefault(typeof(Search<InventoryItem.salesUnit, Where<InventoryItem.inventoryID, Equal<Current<PMDetail.inventoryID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [INUnit(typeof(PMDetail.inventoryID))]
        public override String UOM
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
        #region LastBilledDate
        public new abstract class lastBilledDate : PX.Data.IBqlField
        {
        }
        [PXDBDate()]
        [PXUIField(DisplayName = "Last Billed Date", Enabled = false)]
        public override DateTime? LastBilledDate
        {
            get
            {
                return this._LastBilledDate;
            }
            set
            {
                this._LastBilledDate = value;
            }
        }
        #endregion
        #region LastBilledQty
        public new abstract class lastBilledQty : PX.Data.IBqlField
        {
        }
        [PXDBDecimal(6)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Last Billed Qty.", Enabled = false)]
        public override Decimal? LastBilledQty
        {
            get
            {
                return this._LastBilledQty;
            }
            set
            {
                this._LastBilledQty = value;
            }
        }
        #endregion

        #region System Columns
        #region tstamp
        public new abstract class Tstamp : PX.Data.IBqlField
        {
        }
        [PXDBTimestamp()]
        public override Byte[] tstamp
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
        public new abstract class createdByID : PX.Data.IBqlField
        {
        }
        [PXDBCreatedByID()]
        public override Guid? CreatedByID
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
        public new abstract class createdByScreenID : PX.Data.IBqlField
        {
        }
        [PXDBCreatedByScreenID()]
        public override String CreatedByScreenID
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
        public new abstract class createdDateTime : PX.Data.IBqlField
        {
        }
        [PXDBCreatedDateTime()]
        public override DateTime? CreatedDateTime
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
        public new abstract class lastModifiedByID : PX.Data.IBqlField
        {
        }
        [PXDBLastModifiedByID()]
        public override Guid? LastModifiedByID
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
        public new abstract class lastModifiedByScreenID : PX.Data.IBqlField
        {
        }
        [PXDBLastModifiedByScreenID()]
        public override String LastModifiedByScreenID
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
        public new abstract class lastModifiedDateTime : PX.Data.IBqlField
        {
        }
        [PXDBLastModifiedDateTime()]
        public override DateTime? LastModifiedDateTime
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
