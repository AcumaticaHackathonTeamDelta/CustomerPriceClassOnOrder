using System;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.TX;

namespace PX.Objects.CA
{
    [Serializable]
	[PXCacheName(Messages.CABankTranDetail)]
	public partial class CABankTranDetail : IBqlTable
    {
        #region BranchID
        public abstract class branchID : PX.Data.IBqlField
        {
        }
        protected Int32? _BranchID;
        [Branch()]//we can take this field from cashAcct
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
        #region BankTranID
        public abstract class bankTranID : PX.Data.IBqlField
        {
        }
        protected Int32? _BankTranID;
        [PXDBInt(IsKey = true)]
        [PXDefault(typeof(CABankTran.tranID))]
        [PXParent(typeof(Select<CABankTran, Where<CABankTran.tranType, Equal<Current<CABankTranDetail.bankTranType>>, And<CABankTran.tranID, Equal<Current<CABankTranDetail.bankTranID>>>>>))]
        [PXUIField(Visible = false)]
        public virtual Int32? BankTranID
        {
            get
            {
                return this._BankTranID;
            }
            set
            {
                this._BankTranID = value;
            }
        }
        #endregion
        #region BankTranType
        public abstract class bankTranType : PX.Data.IBqlField
        {
        }
        protected String _BankTranType;
        [PXDBString(1, IsKey = true, IsFixed = true)]
        [PXDefault(typeof(CABankTran.tranType))]
        [PXUIField(DisplayName = "Type", Visible = false)]
        public virtual String BankTranType
        {
            get
            {
                return this._BankTranType;
            }
            set
            {
                this._BankTranType = value;
            }
        }
        #endregion
        #region LineNbr
        public abstract class lineNbr : PX.Data.IBqlField
        {
        }
        protected Int32? _LineNbr;
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false)]
        [PXLineNbr(typeof(CABankTran.lineCntrCA))]
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
        #region AccountID
        public abstract class accountID : PX.Data.IBqlField
        {
        }
        protected Int32? _AccountID;
        [Account(typeof(CABankTranDetail.branchID), typeof(Search2<Account.accountID, LeftJoin<CashAccount, On<CashAccount.accountID, Equal<Account.accountID>>,
                                            InnerJoin<CAEntryType, On<CAEntryType.entryTypeId, Equal<Current<CABankTran.entryTypeID>>>>>,
                                        Where<CAEntryType.useToReclassifyPayments, Equal<False>,
                                            Or<Where<CashAccount.cashAccountID, IsNotNull,
                                                And<CashAccount.curyID, Equal<Current<CABankTran.curyID>>,
                                                And<CashAccount.cashAccountID, NotEqual<Current<CABankTran.cashAccountID>>>>>>>>),
                                                DisplayName = "Offset Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description), CacheGlobal = false, Required=true,PersistingCheck=PXPersistingCheck.Nothing)]
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
        #region SubID
        public abstract class subID : PX.Data.IBqlField
        {
        }
        protected Int32? _SubID;
        [SubAccount(typeof(CABankTranDetail.accountID), typeof(CABankTranDetail.branchID), DisplayName = "Offset Subaccount", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description), Required=true, PersistingCheck=PXPersistingCheck.Nothing)]
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
        #region CashAccountID
        public abstract class cashAccountID : PX.Data.IBqlField
        {
        }
        protected Int32? _CashAccountID;

		[PXRestrictor(typeof(Where<CashAccount.branchID, Equal<Current<CABankTranDetail.branchID>>>), Messages.CashAccountNotMatchBranch)]
		[PXRestrictor(typeof(Where<CashAccount.curyID, Equal<Current<CABankTran.curyID>>>), Messages.OffsetAccountForThisEntryTypeMustBeInSameCurrency)]
		[CashAccountScalar(DisplayName = "Offset Cash Account", Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(CashAccount.descr))]
		[PXDBScalar(typeof(Search<CashAccount.cashAccountID, Where<CashAccount.accountID, Equal<CABankTranDetail.accountID>,
								 And<CashAccount.subID, Equal<CABankTranDetail.subID>, And<CashAccount.branchID, Equal<CABankTranDetail.branchID>>>>>))]
		public virtual Int32? CashAccountID
        {
            get
            {
                return this._CashAccountID;
            }
            set
            {
                this._CashAccountID = value;
            }
        }
        #endregion
        #region TaxCategoryID
        public abstract class taxCategoryID : PX.Data.IBqlField
        {
        }
        protected String _TaxCategoryID;
        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "Tax Category", Visible = false, Visibility = PXUIVisibility.Invisible)]
		[PXSelector(typeof(TaxCategory.taxCategoryID), DescriptionField = typeof(TaxCategory.descr))]
        [PXRestrictor(typeof(Where<TaxCategory.active, Equal<True>>), TX.Messages.InactiveTaxCategory, typeof(TaxCategory.taxCategoryID))]
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
        #region ReferenceID
        public abstract class referenceID : PX.Data.IBqlField
        {
        }
        protected Int32? _ReferenceID;
        [PXDBInt()]
        public virtual Int32? ReferenceID
        {
            get
            {
                return this._ReferenceID;
            }
            set
            {
                this._ReferenceID = value;
            }
        }
        #endregion
        #region TranDesc
        public abstract class tranDesc : PX.Data.IBqlField
        {
        }
        protected String _TranDesc;
        [PXDBString(256, IsUnicode = true)]
        [PXUIField(DisplayName = "Description")]
        public virtual String TranDesc
        {
            get
            {
                return this._TranDesc;
            }
            set
            {
                this._TranDesc = value;
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
        #region CuryTranAmt
        public abstract class curyTranAmt : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryTranAmt;
        [PXDBCurrency(typeof(CABankTranDetail.curyInfoID), typeof(CABankTranDetail.tranAmt))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Amount")]
		[PXUnboundFormula(typeof(CABankTranDetail.curyTranAmt), typeof(SumCalc<CABankTran.curyApplAmtCA>))]		
        public virtual Decimal? CuryTranAmt
        {
            get
            {
                return this._CuryTranAmt;
            }
            set
            {
                this._CuryTranAmt = value;
            }
        }
        #endregion
        #region TranAmt
        public abstract class tranAmt : PX.Data.IBqlField
        {
        }
        protected Decimal? _TranAmt;
        [PXDBDecimal(4)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Tran. Amount")]
        //[PXFormula(null, typeof(SumCalc<CABankTran.tranAmt>))]
        //[PXFormula(null, typeof(SumCalc<CABankTran.splitTotal>))]
        public virtual Decimal? TranAmt
        {
            get
            {
                return this._TranAmt;
            }
            set
            {
                this._TranAmt = value;
            }
        }
        #endregion
        #region CuryTaxableAmt
        public abstract class curyTaxableAmt : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryTaxableAmt;
        [PXCurrency(typeof(CABankTranDetail.curyInfoID), typeof(CABankTranDetail.taxableAmt), BaseCalc = false)]
        [PXDBScalar(typeof(Search2<CATax.curyTaxableAmt, InnerJoin<Tax, On<Tax.taxID, Equal<CATax.taxID>>>,
         Where<CATax.adjTranType, Equal<CABankTranDetail.bankTranType>,
            And<CATax.adjRefNbr, Equal<CABankTranDetail.bankTranID>,
            And<CATax.lineNbr, Equal<CABankTranDetail.lineNbr>,
            And<Tax.taxCalcLevel, Equal<CSTaxCalcLevel.inclusive>,
            And<Tax.taxType, NotEqual<CSTaxType.withholding>>>>>>, OrderBy<Asc<CATax.taxID>>>))]
        public virtual Decimal? CuryTaxableAmt
        {
            get
            {
                return this._CuryTaxableAmt;
            }
            set
            {
                this._CuryTaxableAmt = value;
            }
        }
        #endregion
        #region TaxableAmt
        public abstract class taxableAmt : PX.Data.IBqlField
        {
        }
        protected Decimal? _TaxableAmt;
        [PXBaseCury()]
        [PXDBScalar(typeof(Search2<CATax.taxableAmt, InnerJoin<Tax, On<Tax.taxID, Equal<CATax.taxID>>>,
         Where<CATax.adjTranType, Equal<CABankTranDetail.bankTranType>,
            And<CATax.adjRefNbr, Equal<CABankTranDetail.bankTranID>,
            And<CATax.lineNbr, Equal<CABankTranDetail.lineNbr>,
            And<Tax.taxCalcLevel, Equal<CSTaxCalcLevel.inclusive>,
            And<Tax.taxType, NotEqual<CSTaxType.withholding>>>>>>, OrderBy<Asc<CATax.taxID>>>))]
        public virtual Decimal? TaxableAmt
        {
            get
            {
                return this._TaxableAmt;
            }
            set
            {
                this._TaxableAmt = value;
            }
        }
        #endregion
        #region InventoryID
        public abstract class inventoryID : PX.Data.IBqlField
        {
        }
        protected Int32? _InventoryID;
        [NonStockItem()]
        [PXUIField(DisplayName = "Item ID", Visible = false, Visibility = PXUIVisibility.SelectorVisible)]
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
        #region Qty
        public abstract class qty : PX.Data.IBqlField
        {
        }
        protected Decimal? _Qty;
        [PXDBDecimal(6)]
        [PXDefault(TypeCode.Decimal, "1.0")]
        [PXUIField(DisplayName = "Quantity")]
        public virtual Decimal? Qty
        {
            get
            {
                return this._Qty;
            }
            set
            {
                this._Qty = value;
            }
        }
        #endregion
        #region UnitPrice
        public abstract class unitPrice : PX.Data.IBqlField
        {
        }
        protected Decimal? _UnitPrice;
        [PXDBDecimal(6)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? UnitPrice
        {
            get
            {
                return this._UnitPrice;
            }
            set
            {
                this._UnitPrice = value;
            }
        }
        #endregion
        #region CuryUnitPrice
        public abstract class curyUnitPrice : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryUnitPrice;
        [PXDBDecimal(6)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Price")]
        public virtual Decimal? CuryUnitPrice
        {
            get
            {
                return this._CuryUnitPrice;
            }
            set
            {
                this._CuryUnitPrice = value;
            }
        }
        #endregion
        #region ProjectID
        public abstract class projectID : PX.Data.IBqlField
        {
        }
        protected Int32? _ProjectID;
        [ProjectDefault(BatchModule.CA)]
        [ActiveProjectForModule(PX.Objects.GL.BatchModule.CA, Visible = false, Visibility = PXUIVisibility.SelectorVisible)]
        public virtual Int32? ProjectID
        {
            get
            {
                return this._ProjectID;
            }
            set
            {
                this._ProjectID = value;
            }
        }
        #endregion
        #region TaskID
        public abstract class taskID : PX.Data.IBqlField
        {
        }
        protected Int32? _TaskID;
        [ActiveProjectTask(typeof(CABankTranDetail.projectID), BatchModule.CA, DisplayName = "Project Task", Visible = false, Visibility = PXUIVisibility.SelectorVisible)]
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
        #region NonBillable
        public abstract class nonBillable : PX.Data.IBqlField
        {
        }
        protected Boolean? _NonBillable;
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Non Billable", Visible = false, Visibility = PXUIVisibility.SelectorVisible, FieldClass = ProjectAttribute.DimensionName)]
        public virtual Boolean? NonBillable
        {
            get
            {
                return this._NonBillable;
            }
            set
            {
                this._NonBillable = value;
            }
        }
        #endregion
        #region NoteID
        public abstract class noteID : PX.Data.IBqlField
        {
        }
        protected Guid? _NoteID;
        [PXNote()]
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
    }
        
}
