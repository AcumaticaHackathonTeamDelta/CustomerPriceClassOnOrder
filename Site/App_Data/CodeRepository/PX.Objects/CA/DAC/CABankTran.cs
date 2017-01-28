using System;
using PX.Data;
using PX.Data.EP;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CA.BankStatementHelpers;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Web.UI;

namespace PX.Objects.CA
{
	[System.SerializableAttribute()]
    [PXCacheName(Messages.BankTransaction)]
	public partial class CABankTran : PX.Data.IBqlTable, ICADocSource
	{
		#region CashAccountID
		public abstract class cashAccountID : PX.Data.IBqlField { }
		protected Int32? _CashAccountID;
		[PXDBInt(IsKey = true)]
		[PXDefault(typeof(CABankTranHeader.cashAccountID))]
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
		#region TranID
		public abstract class tranID : PX.Data.IBqlField
		{
		}
		protected Int32? _TranID;
		[PXDBIdentity(IsKey = true)]
		public virtual Int32? TranID
		{
			get
			{
				return this._TranID;
			}
			set
			{
				this._TranID = value;
			}
		}
		#endregion
        #region TranType
        public abstract class tranType : PX.Data.IBqlField
        {
        }
        protected String _TranType;
        [PXDBString(1, IsFixed = true)]
        [PXDefault(typeof(CABankTranHeader.tranType))]
        [CABankTranType.List()]
        [PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = true, TabOrder = 0)]
        public virtual String TranType
        {
            get
            {
                return this._TranType;
            }
            set
            {
                this._TranType = value;
            }
        }
        #endregion
		#region HeaderRefNbr
		public abstract class headerRefNbr : PX.Data.IBqlField
		{
		}
		protected String _HeaderRefNbr;
		[PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXDBDefault(typeof(CABankTranHeader.refNbr))]
        [PXUIField(DisplayName="Statement Nbr.")]
		[PXParent(typeof(Select<CABankTranHeader, Where<CABankTranHeader.refNbr, Equal<Current<CABankTran.headerRefNbr>>, And<CABankTranHeader.tranType,Equal<Current<CABankTran.tranType>>>>>))]
		public virtual String HeaderRefNbr
		{
			get
			{
				return this._HeaderRefNbr;
			}
			set
			{
				this._HeaderRefNbr = value;
			}
		}
		#endregion
		#region ExtTranID
		public abstract class extTranID : PX.Data.IBqlField
		{
		}
		protected String _ExtTranID;
		[PXDBString(255, IsUnicode = true)]
		[PXUIField(DisplayName = "Ext. Tran. ID", Visible = false)]
		public virtual String ExtTranID
		{
			get
			{
				return this._ExtTranID;
			}
			set
			{
				this._ExtTranID = value;
			}
		}
		#endregion
		#region DrCr
		public abstract class drCr : PX.Data.IBqlField
		{
		}
		protected String _DrCr;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(CADrCr.CACredit)]
		[CADrCr.List()]
		[PXUIField(DisplayName = "DrCr")]
		public virtual String DrCr
		{
			get
			{
				return this._DrCr;
			}
			set
			{
				this._DrCr = value;
			}
		}
		#endregion
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXDBString(5, IsUnicode = true)]
		[PXDefault(/*typeof(CashAccount.curyID)*/)]
		[PXSelector(typeof(Currency.curyID), CacheGlobal = true)]
		[PXUIField(DisplayName = "Currency")]
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
		[CurrencyInfoConditional(typeof(CABankTran.createDocument), ModuleCode = GL.BatchModule.CA)]
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
		#region TranDate
		public abstract class tranDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _TranDate;
		[PXDBDate()]
		[PXDefault()]
		[PXUIField(DisplayName = "Tran. Date")]
		public virtual DateTime? TranDate
		{
			get
			{
				return this._TranDate;
			}
			set
			{
				this._TranDate = value;
			}
		}
		#endregion
		#region TranEntryDate
		public abstract class tranEntryDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _TranEntryDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Tran. Entry Date", Visible = false)]
		public virtual DateTime? TranEntryDate
		{
			get
			{
				return this._TranEntryDate;
			}
			set
			{
				this._TranEntryDate = value;
			}
		}
		#endregion
		#region CuryTranAmt
		public abstract class curyTranAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryTranAmt;
		[PXDBCury(typeof(CABankTran.curyID))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "CuryTranAmt")]
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
		#region OrigCuryID
		public abstract class origCuryID : PX.Data.IBqlField
		{
		}
		protected String _OrigCuryID;
		[PXDBString(5, IsUnicode = true)]
		[PXSelector(typeof(Currency.curyID), CacheGlobal = true)]
		[PXUIField(DisplayName = "Orig. Currency", Visible = false)]
		public virtual String OrigCuryID
		{
			get
			{
				return this._OrigCuryID;
			}
			set
			{
				this._OrigCuryID = value;
			}
		}
		#endregion
		#region CuryOrigAmt
		public abstract class curyOrigAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryOrigAmt;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Cury. Orig. Amt.", Visible = false)]
		public virtual Decimal? CuryOrigAmt
		{
			get
			{
				return this._CuryOrigAmt;
			}
			set
			{
				this._CuryOrigAmt = value;
			}
		}
		#endregion
		#region ExtRefNbr
		public abstract class extRefNbr : PX.Data.IBqlField
		{
		}
		protected String _ExtRefNbr;
		[PXDBString(40, IsUnicode = true)]
		[PXUIField(DisplayName = "Ext. Ref. Nbr.")]
		public virtual String ExtRefNbr
		{
			get
			{
				return this._ExtRefNbr;
			}
			set
			{
				this._ExtRefNbr = value;
			}
		}
		#endregion
		#region TranDesc
		public abstract class tranDesc : PX.Data.IBqlField
		{
		}
		protected String _TranDesc;
		[PXDBString(256, IsUnicode = true)]
		[PXUIField(DisplayName = "Tran. Desc")]
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
        #region UserDesc
        public abstract class userDesc : PX.Data.IBqlField
        {
        }
        protected String _UserDesc;
        [PXDBString(256, IsUnicode = true)]
        [PXUIField(DisplayName = "Tran. Desc",Enabled=true)]
        public virtual String UserDesc
        {
            get
            {
                return this._UserDesc;
            }
            set
            {
                this._UserDesc = value;
            }
        }
        #endregion
		#region PayeeName
		public abstract class payeeName : PX.Data.IBqlField
		{
		}
		protected String _PayeeName;
		[PXDBString(256, IsUnicode = true)]
		[PXUIField(DisplayName = "Payee Name", Visible = false)]
		public virtual String PayeeName
		{
			get
			{
				return this._PayeeName;
			}
			set
			{
				this._PayeeName = value;
			}
		}
		#endregion
		#region PayeeAddress1
		public abstract class payeeAddress1 : PX.Data.IBqlField
		{
		}
		protected String _PayeeAddress1;
		[PXDBString(256, IsUnicode = true)]
		[PXUIField(DisplayName = "Payee Address1", Visible = false)]
		public virtual String PayeeAddress1
		{
			get
			{
				return this._PayeeAddress1;
			}
			set
			{
				this._PayeeAddress1 = value;
			}
		}
		#endregion
		#region PayeeCity
		public abstract class payeeCity : PX.Data.IBqlField
		{
		}
		protected String _PayeeCity;
		[PXDBString(256, IsUnicode = true)]
		[PXUIField(DisplayName = "Payee City", Visible = false)]
		public virtual String PayeeCity
		{
			get
			{
				return this._PayeeCity;
			}
			set
			{
				this._PayeeCity = value;
			}
		}
		#endregion
		#region PayeeState
		public abstract class payeeState : PX.Data.IBqlField
		{
		}
		protected String _PayeeState;
		[PXDBString(256, IsUnicode = true)]
		[PXUIField(DisplayName = "Payee State", Visible = false)]
		public virtual String PayeeState
		{
			get
			{
				return this._PayeeState;
			}
			set
			{
				this._PayeeState = value;
			}
		}
		#endregion
		#region PayeePostalCode
		public abstract class payeePostalCode : PX.Data.IBqlField
		{
		}
		protected String _PayeePostalCode;
		[PXDBString(256, IsUnicode = true)]
		[PXUIField(DisplayName = "Payee Postal Code", Visible = false)]
		public virtual String PayeePostalCode
		{
			get
			{
				return this._PayeePostalCode;
			}
			set
			{
				this._PayeePostalCode = value;
			}
		}
		#endregion
		#region PayeePhone
		public abstract class payeePhone : PX.Data.IBqlField
		{
		}
		protected String _PayeePhone;
		[PXDBString(256, IsUnicode = true)]
		[PXUIField(DisplayName = "Payee Phone", Visible = false)]
		public virtual String PayeePhone
		{
			get
			{
				return this._PayeePhone;
			}
			set
			{
				this._PayeePhone = value;
			}
		}
		#endregion
		#region TranCode
		public abstract class tranCode : PX.Data.IBqlField
		{
		}
		protected String _TranCode;
		[PXDBString(35, IsUnicode = true)]
		[PXUIField(DisplayName = "Tran. Code", Visible = false)]
		public virtual String TranCode
		{
			get
			{
				return this._TranCode;
			}
			set
			{
				this._TranCode = value;
			}
		}
		#endregion
		#region OrigModule
		public abstract class origModule : PX.Data.IBqlField
		{
		}
		protected String _OrigModule;
		[PXDBString(2, IsFixed = true)]
		[PXStringList(new string[] { GL.BatchModule.AP, GL.BatchModule.AR, GL.BatchModule.CA, }, new string[] { GL.Messages.ModuleAP, GL.Messages.ModuleAR, GL.Messages.ModuleCA })]
		[PXUIField(DisplayName = "Module", Enabled = false)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual String OrigModule
		{
			get
			{
				return this._OrigModule;
			}
			set
			{
				this._OrigModule = value;
			}
		}
		#endregion
		#region PayeeBAccountID
		public abstract class payeeBAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _PayeeBAccountID;
		[PXDBInt()]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXVendorCustomerSelector(typeof(CABankTran.origModule))]
		[PXUIField(DisplayName = "Business Account", Visible=false)]
		public virtual Int32? PayeeBAccountID
		{
			get
			{
				return this._PayeeBAccountID;
			}
			set
			{
				this._PayeeBAccountID = value;
			}
		}
		#endregion
		#region PayeeLocationID
		public abstract class payeeLocationID : PX.Data.IBqlField
		{
		}
		protected Int32? _PayeeLocationID;
		[LocationID(typeof(Where<Location.bAccountID, Equal<Current<CABankTran.payeeBAccountID>>>), DisplayName = "Location", DescriptionField = typeof(Location.descr),Visible=false)]
		[PXDefault(typeof(Search<BAccountR.defLocationID, Where<BAccountR.bAccountID, Equal<Current<CABankTran.payeeBAccountID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Int32? PayeeLocationID
		{
			get
			{
				return this._PayeeLocationID;
			}
			set
			{
				this._PayeeLocationID = value;
			}
		}
		#endregion
		#region PaymentMethodID
		public abstract class paymentMethodID : PX.Data.IBqlField
		{
		}
		protected String _PaymentMethodID;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault(typeof(Coalesce<
							Coalesce<
								Search2<Customer.defPaymentMethodID, 
									InnerJoin<PaymentMethod,
										On<PaymentMethod.paymentMethodID, Equal<Customer.defPaymentMethodID>,
										And<PaymentMethod.useForAR, Equal<True>>>,
									InnerJoin<PaymentMethodAccount,
										On<PaymentMethodAccount.paymentMethodID, Equal<Customer.defPaymentMethodID>,
										And<PaymentMethodAccount.useForAR, Equal<True>,
										And<PaymentMethodAccount.cashAccountID, Equal<Current<CABankTran.cashAccountID>>>>>>>,
									Where<Current<CABankTran.origModule>, Equal<GL.BatchModule.moduleAR>,
										And<Customer.bAccountID, Equal<Current<CABankTran.payeeBAccountID>>>>>,
								Search2<PaymentMethodAccount.paymentMethodID, 
									InnerJoin<PaymentMethod, On<PaymentMethodAccount.paymentMethodID,Equal<PaymentMethod.paymentMethodID>,
										And<PaymentMethodAccount.cashAccountID, Equal<Current<CABankTran.cashAccountID>>,
										And<PaymentMethodAccount.useForAR, Equal<True>>>>>,
									Where<Current<CABankTran.origModule>, Equal<GL.BatchModule.moduleAR>,
										And<PaymentMethod.useForAR, Equal<True>,
										And<PaymentMethod.isActive, Equal<boolTrue>>>>, OrderBy<Asc<PaymentMethodAccount.aRIsDefault, Desc<PaymentMethodAccount.paymentMethodID>>>>>,
							Coalesce<
								Search2<Location.vPaymentMethodID, 
									InnerJoin<Vendor, On<Location.bAccountID, Equal<Vendor.bAccountID>, And<Location.locationID, Equal<Vendor.defLocationID>>>,
									InnerJoin<PaymentMethodAccount, On<PaymentMethodAccount.paymentMethodID, Equal<Location.vPaymentMethodID>,
										And<PaymentMethodAccount.useForAP, Equal<True>, 
										And<PaymentMethodAccount.cashAccountID, Equal<Current<CABankTran.cashAccountID>>>>>>>, 
									Where<Current<CABankTran.origModule>, Equal<GL.BatchModule.moduleAP>,
										And<Vendor.bAccountID, Equal<Current<CABankTran.payeeBAccountID>>>>>,
								Search2<PaymentMethodAccount.paymentMethodID, 
									InnerJoin<PaymentMethod, On<PaymentMethodAccount.paymentMethodID,Equal<PaymentMethod.paymentMethodID>,
										And<PaymentMethodAccount.cashAccountID, Equal<Current<CABankTran.cashAccountID>>,
										And<PaymentMethodAccount.useForAP, Equal<True>>>>>,
									Where<Current<CABankTran.origModule>, Equal<GL.BatchModule.moduleAP>,
										And<PaymentMethod.useForAP, Equal<True>,
										And<PaymentMethod.isActive, Equal<boolTrue>>>>, OrderBy<Asc<PaymentMethodAccount.aPIsDefault, Desc<PaymentMethodAccount.paymentMethodID>>>>>>),
					PersistingCheck = PXPersistingCheck.Nothing)]
		[PXSelector(typeof(Search2<PaymentMethod.paymentMethodID,
				InnerJoin<PaymentMethodAccount, On<PaymentMethodAccount.paymentMethodID,
				Equal<PaymentMethod.paymentMethodID>,
				And<PaymentMethodAccount.cashAccountID, Equal<Current<CABankTran.cashAccountID>>,
					And<Where2<Where<Current<CABankTran.origModule>, Equal<GL.BatchModule.moduleAP>, And<PaymentMethodAccount.useForAP, Equal<True>>>,
						Or<Where<Current<CABankTran.origModule>, Equal<GL.BatchModule.moduleAR>, And<PaymentMethodAccount.useForAR, Equal<True>>>>>>>>>,
				Where<PaymentMethod.isActive, Equal<boolTrue>,
					And<Where2<Where<Current<CABankTran.origModule>, Equal<GL.BatchModule.moduleAP>, And<PaymentMethod.useForAP, Equal<True>>>,
						Or<Where<Current<CABankTran.origModule>, Equal<GL.BatchModule.moduleAR>, And<PaymentMethod.useForAR, Equal<True>>>>>>>>), DescriptionField = typeof(PaymentMethod.descr))]
		[PXUIField(DisplayName = "Payment Method", Visible = false)]
		public virtual String PaymentMethodID
		{
			get
			{
				return this._PaymentMethodID;
			}
			set
			{
				this._PaymentMethodID = value;
			}
		}
		#endregion
		#region PMInstanceID
		public abstract class pMInstanceID : PX.Data.IBqlField
		{
		}
		protected Int32? _PMInstanceID;
		[PXDBInt()]
		[PXUIField(DisplayName = "Card/Account No", Visible=false)]
		[PXDefault(typeof(Coalesce<
								Search2<Customer.defPMInstanceID, InnerJoin<CustomerPaymentMethod, On<CustomerPaymentMethod.pMInstanceID, Equal<Customer.defPMInstanceID>,
								And<CustomerPaymentMethod.bAccountID, Equal<Customer.bAccountID>>>>,
								Where<Current<CABankTran.origModule>, Equal<GL.BatchModule.moduleAR>,
								And<Customer.bAccountID, Equal<Current<CABankTran.payeeBAccountID>>,
								And<CustomerPaymentMethod.isActive, Equal<True>,
								And<CustomerPaymentMethod.paymentMethodID, Equal<Current<CABankTran.paymentMethodID>>>>>>>,
							  Search<CustomerPaymentMethod.pMInstanceID,
									Where<Current<CABankTran.origModule>, Equal<GL.BatchModule.moduleAR>,
								   And<CustomerPaymentMethod.bAccountID, Equal<Current<CABankTran.payeeBAccountID>>,
									And<CustomerPaymentMethod.paymentMethodID, Equal<Current<CABankTran.paymentMethodID>>,
									And<CustomerPaymentMethod.isActive, Equal<True>>>>>,
								OrderBy<Desc<CustomerPaymentMethod.expirationDate,
								Desc<CustomerPaymentMethod.pMInstanceID>>>>>),
									PersistingCheck = PXPersistingCheck.Nothing)]
		[PXSelector(typeof(Search<CustomerPaymentMethod.pMInstanceID,
									Where<CustomerPaymentMethod.bAccountID, Equal<Current<CABankTran.payeeBAccountID>>,
									  And<CustomerPaymentMethod.paymentMethodID, Equal<Current<CABankTran.paymentMethodID>>,
									  And<CustomerPaymentMethod.isActive, Equal<boolTrue>>>>>),
									  DescriptionField = typeof(CustomerPaymentMethod.descr))]
		public virtual Int32? PMInstanceID
		{
			get
			{
				return this._PMInstanceID;
			}
			set
			{
				this._PMInstanceID = value;
			}
		}
		#endregion
		#region InvoiceInfo
		public abstract class invoiceInfo : PX.Data.IBqlField
		{
		}
		protected String _InvoiceInfo;
		[PXDBString(256, IsUnicode = true)]
		[PXUIField(DisplayName = "Invoice Nbr.", Visible=false)]
		public virtual String InvoiceInfo
		{
			get
			{
				return this._InvoiceInfo;
			}
			set
			{
				this._InvoiceInfo = value;
			}
		}
		#endregion
		#region PayeeMatched
		public abstract class payeeMatched : PX.Data.IBqlField
		{
		}
		protected Boolean? _PayeeMatched;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "PayeeMatched")]
		public virtual Boolean? PayeeMatched
		{
			get
			{
				return this._PayeeMatched;
			}
			set
			{
				this._PayeeMatched = value;
			}
		}
		#endregion
		#region DocumentMatched
		public abstract class documentMatched : PX.Data.IBqlField
		{
		}
		protected Boolean? _DocumentMatched;
		[PXDBBool()]
		[PXHeaderImage(Sprite.AliasControl + "@" + Sprite.Control.CompleteHead)]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Matched", Visible = true, Enabled = false)]
		public virtual Boolean? DocumentMatched
		{
			get
			{
				return this._DocumentMatched;
			}
			set
			{
				this._DocumentMatched = value;
			}
		}
		#endregion
		#region Validated
		public abstract class validated : PX.Data.IBqlField
		{
		}
		protected Boolean? _Validated;
		[PXBool()]
		[PXDefault(false,PersistingCheck=PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Validated", Visible = false,Visibility=PXUIVisibility.Invisible, Enabled = false)]
		public virtual Boolean? Validated
		{
			get
			{
				return this._Validated;
			}
			set
			{
				this._Validated = value;
			}
		}
		#endregion
		#region RuleApplied
		public abstract class ruleApplied : PX.Data.IBqlField
		{
		}
		[PXBool()]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Rule Applied", Visible = false, Visibility = PXUIVisibility.Invisible, Enabled = false)]
		public virtual Boolean? RuleApplied
		{
			[PXDependsOnFields(typeof(ruleID), typeof(createDocument))]
			get
			{
				return this._CreateDocument == true && this._RuleID!=null;
			}
		}
		#endregion	
		#region ApplyRuleEnabled
		public abstract class applyRuleEnabled : PX.Data.IBqlField
		{
		}
		[PXBool()]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Create Rule Enabled", Visible = false, Visibility = PXUIVisibility.Invisible, Enabled = false)]
		public virtual Boolean? ApplyRuleEnabled
		{
			[PXDependsOnFields(typeof(ruleID), typeof(createDocument))]
			get
			{
				return this._CreateDocument == true && this._RuleID == null;
			}
		}
		#endregion
		#region MatchedToExisting
		public abstract class matchedToExisting : PX.Data.IBqlField
		{
		}
		protected Boolean? _MatchedToExisting;
		[PXBool()]
		[PXUIField(DisplayName = "Matched", Visible = true, Enabled = false)]
		public virtual Boolean? MatchedToExisting
		{
			get
			{
				return this._MatchedToExisting;
			}
			set
			{
				this._MatchedToExisting = value;
			}
		}
		#endregion
		#region MatchedToInvoice
		public abstract class matchedToInvoice : PX.Data.IBqlField
		{
		}
		protected Boolean? _MatchedToInvoice;
		[PXBool()]
		[PXUIField(DisplayName = "Matched to Invoice", Visible = false, Enabled = false)]
		public virtual Boolean? MatchedToInvoice
		{
			get
			{
				return this._MatchedToInvoice;
			}
			set
			{
				this._MatchedToInvoice = value;
			}
		}
		#endregion
		#region CreateDocument
		public abstract class createDocument : PX.Data.IBqlField
		{
		}
		protected Boolean? _CreateDocument;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Create")]
		public virtual Boolean? CreateDocument
		{
			get
			{
				return this._CreateDocument;
			}
			set
			{
				this._CreateDocument = value;
			}
		}
		#endregion
		#region Status
		public abstract class status : PX.Data.IBqlField
		{
		}
		[PXString(1, IsFixed = true)]
		[CABankTranStatus.List()]
		[PXUIField(DisplayName = "Match Type", Visibility = PXUIVisibility.SelectorVisible, Visible = false, Enabled = false)]
		public virtual String Status
		{
			[PXDependsOnFields(typeof(hidden), typeof(createDocument), typeof(matchedToInvoice), typeof(documentMatched))]
			get
			{
				if (this._Hidden == true)
				{
					return CABankTranStatus.Hidden;
				}
				else if (this._CreateDocument == true)
				{
					return CABankTranStatus.Created;
				}
				else if (this._MatchedToInvoice == true)
				{
					return CABankTranStatus.InvoiceMatched;
				}
				else if (this._DocumentMatched == true)
				{
					return CABankTranStatus.Matched;
				}
				else return String.Empty;
			}
		}
		#endregion
		#region Processed
		public abstract class processed : PX.Data.IBqlField
		{
		}
		protected Boolean? _Processed;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Processed")]
		public virtual Boolean? Processed
		{
			get
			{
				return this._Processed;
			}
			set
			{
				this._Processed = value;
			}
		}
		#endregion
		#region EntryTypeID
		public abstract class entryTypeID : PX.Data.IBqlField
		{
		}
		protected String _EntryTypeID;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXSelector(typeof(Search2<CAEntryType.entryTypeId,
							  InnerJoin<CashAccountETDetail, On<CashAccountETDetail.entryTypeID, Equal<CAEntryType.entryTypeId>>>,
							  Where<CashAccountETDetail.accountID, Equal<Current<CABankTran.cashAccountID>>,
								And<CAEntryType.module, Equal<GL.BatchModule.moduleCA>,
								And<Where<CAEntryType.drCr, Equal<Current<CABankTran.drCr>>>>>>>),
					  DescriptionField = typeof(CAEntryType.descr))]
		[PXUIField(DisplayName = "Entry Type ID", Visibility = PXUIVisibility.SelectorVisible, Visible=false)]
		public virtual String EntryTypeID
		{
			get
			{
				return this._EntryTypeID;
			}
			set
			{
				this._EntryTypeID = value;
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

		#region CuryDebitAmt
		public abstract class curyDebitAmt : PX.Data.IBqlField
		{
		}
		[PXCury(typeof(CABankTran.curyID))]
		[PXUIField(DisplayName = "Receipt")]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXFormula(null, typeof(SumCalc<CABankTranHeader.curyDebitsTotal>))]
		public virtual Decimal? CuryDebitAmt
		{
			[PXDependsOnFields(typeof(drCr), typeof(curyTranAmt))]
			get
			{
				return (this._DrCr == CADrCr.CADebit) ? this._CuryTranAmt : Decimal.Zero;
			}
			set
			{
				if (value != 0m)
				{
					this._CuryTranAmt = value;
					this._DrCr = CADrCr.CADebit;
				}
				else if (this._DrCr == CADrCr.CADebit)
				{
					this._CuryTranAmt = 0m;
				}
			}
		}
		#endregion
		#region CuryCreditAmt
		public abstract class curyCreditAmt : PX.Data.IBqlField
		{
		}
		[PXCury(typeof(CABankTran.curyID))]
		[PXUIField(DisplayName = "Disbursement")]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXFormula(null, typeof(SumCalc<CABankTranHeader.curyCreditsTotal>))]
		public virtual Decimal? CuryCreditAmt
		{
			[PXDependsOnFields(typeof(drCr), typeof(curyTranAmt))]
			get
			{
				return (this._DrCr == CADrCr.CACredit) ? -this._CuryTranAmt : Decimal.Zero;
			}
			set
			{
				if (value != 0m)
				{
					this._CuryTranAmt = -value;
					this._DrCr = CADrCr.CACredit;
				}
				else if (this._DrCr == CADrCr.CACredit)
				{
					this._CuryTranAmt = 0m;
				}
			}
		}
		#endregion

		#region CuryReconciledDebit
		public abstract class curyReconciledDebit : PX.Data.IBqlField
		{
		}
		[PXCury(typeof(CABankTran.curyID))]
//		[PXFormula(null, typeof(SumCalc<CABankStatement.curyReconciledDebits>))]
		public virtual Decimal? CuryReconciledDebit
		{
			[PXDependsOnFields(typeof(documentMatched), typeof(curyDebitAmt))]
			get
			{
				return (this._DocumentMatched == true ? this.CuryDebitAmt : Decimal.Zero);
			}
			set
			{
			}
		}
		#endregion
		#region CuryReconciledCredit
		public abstract class curyReconciledCredit : PX.Data.IBqlField
		{
		}
		[PXCury(typeof(CABankTran.curyID))]
//		[PXFormula(null, typeof(SumCalc<CABankStatement.curyReconciledCredits>))]
		public virtual Decimal? CuryReconciledCredit
		{
			[PXDependsOnFields(typeof(documentMatched), typeof(curyCreditAmt))]
			get
			{
				return (this.DocumentMatched == true) ? this.CuryCreditAmt : Decimal.Zero;
			}
			set
			{
			}
		}
		#endregion

		#region CountDebit
		public abstract class countDebit : PX.Data.IBqlField
		{
		}
		[PXInt()]
//		[PXFormula(null, typeof(SumCalc<CABankStatement.countDebit>))]
		public virtual Int32? CountDebit
		{
			[PXDependsOnFields(typeof(drCr))]
			get
			{
				return (this._DrCr == CADrCr.CADebit) ? (int)1 : (int)0;
			}
			set
			{
			}
		}
		#endregion
		#region CountCredit
		public abstract class countCredit : PX.Data.IBqlField
		{
		}
		[PXInt()]
//		[PXFormula(null, typeof(SumCalc<CABankStatement.countCredit>))]
		public virtual Int32? CountCredit
		{
			[PXDependsOnFields(typeof(drCr))]
			get
			{
				return (this._DrCr == CADrCr.CACredit ? (int)1 : (int)0);
			}
			set
			{
			}
		}
		#endregion
		#region ReconciledCountDebit
		public abstract class reconciledCountDebit : PX.Data.IBqlField
		{
		}
		[PXInt()]
//		[PXFormula(null, typeof(SumCalc<CABankStatement.reconciledCountDebit>))]
		public virtual Int32? ReconciledCountDebit
		{
			[PXDependsOnFields(typeof(documentMatched), typeof(countDebit))]
			get
			{
				return (this.DocumentMatched == true) ? this.CountDebit : 0;
			}
			set
			{
			}
		}
		#endregion
		#region ReconciledCountCredit
		public abstract class reconciledCountCredit : PX.Data.IBqlField
		{
		}
		[PXInt()]
//		[PXFormula(null, typeof(SumCalc<CABankStatement.reconciledCountCredit>))]
		public virtual Int32? ReconciledCountCredit
		{
			[PXDependsOnFields(typeof(documentMatched), typeof(countCredit))]
			get
			{
				return (this.DocumentMatched == true) ? this.CountCredit : 0;
			}
			set
			{
			}
		}
		#endregion
		#region ICADocSource Members

		public int? BAccountID
		{
			get
			{
				return this._PayeeBAccountID;
			}
			set
			{
				this._PayeeBAccountID = value;
			}
		}

		public int? LocationID
		{
			get
			{
				return this._PayeeLocationID;
			}
			set
			{
				this._PayeeLocationID = value;
			}
		}


		public bool? Cleared
		{
			get
			{
				return false;
			}
			set
			{
				;
			}
		}

		public int? CARefTranAccountID
		{
			get
			{
				return null;
			}
			set
			{
				;
			}
		}
		public long? CARefTranID
		{
			get
			{
				return null;
			}
			set
			{
				;
			}
		}
		public int? CARefSplitLineNbr
		{
			get
			{
				return null;
			}
			set
			{
				;
			}
		}

		public decimal? CuryOrigDocAmt
		{
			[PXDependsOnFields(typeof(curyTranAmt))]
			get
			{
				return this.CuryTranAmt.HasValue ? (this.CuryTranAmt.Value != Decimal.Zero ? this.CuryTranAmt * Math.Sign(this.CuryTranAmt.Value) : Decimal.Zero) : null; //Document sign is inverted compared to the CATran's
			}
			set
			{

			}
		}

		long? ICADocSource.CuryInfoID
		{
			get { return null; }
			set { ;}
		}

		string ICADocSource.FinPeriodID
		{
			get { return null; }
			set { ;}
		}

		string ICADocSource.InvoiceNbr
		{
			get
			{
				return InvoiceInfo;
			}
			set
			{
				;
			}
		}
        string ICADocSource.TranDesc
        {
            get
            {
                return UserDesc;
            }
            set
            {
                ;
            }
        }
		#endregion
		#region CuryTotalAmt
		public abstract class curyTotalAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryTotalAmt;
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Total Amount")]
		[PXCury(typeof(CABankTran.curyID))]
		public virtual Decimal? CuryTotalAmt
		{
			get
			{
				this._CuryTotalAmt = (this._DrCr == CADrCr.CACredit) ? (-1 * this._CuryTranAmt) : this._CuryTranAmt;
				return this._CuryTotalAmt;
			}
			set
			{
				//this._CuryTotalAmt = value;
			}
		}
		#endregion

        #region CuryApplAmtCA
        public abstract class curyApplAmtCA : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryApplAmtCA;
        [PXDBCury(typeof(CABankTran.curyID))]
        [PXUIField(DisplayName = "Amount Used", Visibility = PXUIVisibility.Visible, Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? CuryApplAmtCA
        {
            get
            {
                return this._CuryApplAmtCA;
            }
            set
            {
                this._CuryApplAmtCA = value;
            }
        }
        #endregion
        #region CuryUnappliedBalCA
        public abstract class curyUnappliedBalCA : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryUnappliedBalCA;
        [PXCury(typeof(CABankTran.curyID))]
        [PXUIField(DisplayName = "Balance Left", Visibility = PXUIVisibility.Visible, Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? CuryUnappliedBalCA
        {
            [PXDependsOnFields(typeof(curyTotalAmt), typeof(curyApplAmtCA))]
            get
            {
                return ((this.CuryTotalAmt ?? Decimal.Zero) - (this.CuryApplAmtCA ?? Decimal.Zero));
            }
            set { }
        }
        #endregion

		#region CuryApplAmt
		public abstract class curyApplAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryApplAmt;
		[PXDBCury(typeof(CABankTran.curyID))]
		[PXUIField(DisplayName = "Application Amount", Visibility = PXUIVisibility.Visible, Enabled = false)]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Decimal? CuryApplAmt
		{
			get
			{
				return this._CuryApplAmt;
			}
			set
			{
				this._CuryApplAmt = value;
			}
		}
		#endregion
		#region CuryUnappliedBal
		public abstract class curyUnappliedBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryUnappliedBal;
		[PXCury(typeof(CABankTran.curyID))]
		[PXUIField(DisplayName = "Unapplied Balance", Visibility = PXUIVisibility.Visible, Enabled = false)]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Decimal? CuryUnappliedBal
		{
			[PXDependsOnFields(typeof(curyTotalAmt), typeof(curyApplAmt))]
			get
			{
				return ((this.CuryTotalAmt ?? Decimal.Zero) - (this.CuryApplAmt ?? Decimal.Zero));
			}
			set
			{
				//this._CuryApplAmt = value;
			}
		}
		#endregion
		#region DocType
		public abstract class docType : PX.Data.IBqlField
		{
		}
		protected String _DocType;
		[PXString(3, IsFixed = true)]
		[PXDefault()]
		[APPaymentType.List()]
		[PXFieldDescription]
		public String DocType
		{
			get
			{
				if (this.OrigModule == GL.BatchModule.AP)
				{
					if (this.DrCr == CADrCr.CACredit)
					{
						_DocType = APDocType.Check;
					}
					else
					{
						_DocType = APDocType.Refund;
					}
				}
				else
				{
					if (this.DrCr == CADrCr.CACredit)
					{
						_DocType = ARDocType.Refund;
					}
					else
					{
						_DocType = ARDocType.Payment;
					}
				}
				return _DocType;
			}
			set
			{
				this._DocType = value;
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
        #region LineCntrCA
        public abstract class lineCntrCA : PX.Data.IBqlField
        {
        }
        protected Int32? _LineCntrCA;
        [PXDBInt()]
        [PXDefault(0)]
        public virtual Int32? LineCntrCA
        {
            get
            {
                return this._LineCntrCA;
            }
            set
            {
                this._LineCntrCA = value;
            }
        }
        #endregion
		#region PayeeBAccountIDCopy
		public abstract class payeeBAccountIDCopy : PX.Data.IBqlField
		{
		}
		[PXInt()]
		[PXSelector(typeof(Search<BAccountR.bAccountID>), SubstituteKey = typeof(BAccountR.acctCD), DescriptionField = typeof(BAccountR.acctName))]
		[PXUIField(DisplayName = "Business Account")]
		public virtual Int32? PayeeBAccountIDCopy
		{
			get
			{
				return this._PayeeBAccountID;
			}
			set
			{
				this._PayeeBAccountID = value;
			}
		}
		#endregion
		#region PayeeLocationIDCopy
		public abstract class payeeLocationIDCopy : PX.Data.IBqlField
		{
		}
		[PXInt()]
		[PXUIField(DisplayName = "Location", Visibility = PXUIVisibility.Visible, FieldClass = "LOCATION")]
		[LocationIDBase(typeof(Where<Location.bAccountID, Equal<Current<CABankTran.payeeBAccountID>>>), DisplayName = "Location", DescriptionField = typeof(Location.descr))]
		public virtual Int32? PayeeLocationIDCopy
		{
			get
			{
				return this._PayeeLocationID;
			}
			set
			{
				this._PayeeLocationID = value;
			}
		}
		#endregion
		#region PaymentMethodIDCopy
		public abstract class paymentMethodIDCopy : PX.Data.IBqlField
		{
		}
		[PXString(10, IsUnicode = true)]
		[PXSelector(typeof(Search2<PaymentMethod.paymentMethodID,
				InnerJoin<PaymentMethodAccount, On<PaymentMethodAccount.paymentMethodID,
				Equal<PaymentMethod.paymentMethodID>,
				And<PaymentMethodAccount.cashAccountID, Equal<Current<CABankTran.cashAccountID>>,
					And<Where2<Where<Current<CABankTran.origModule>, Equal<GL.BatchModule.moduleAP>, And<PaymentMethodAccount.useForAP, Equal<True>>>,
						Or<Where<Current<CABankTran.origModule>, Equal<GL.BatchModule.moduleAR>, And<PaymentMethodAccount.useForAR, Equal<True>>>>>>>>>,
				Where<PaymentMethod.isActive, Equal<boolTrue>,
					And<Where2<Where<Current<CABankTran.origModule>, Equal<GL.BatchModule.moduleAP>, And<PaymentMethod.useForAP, Equal<True>>>,
						Or<Where<Current<CABankTran.origModule>, Equal<GL.BatchModule.moduleAR>, And<PaymentMethod.useForAR, Equal<True>>>>>>>>), DescriptionField = typeof(PaymentMethod.descr))]
		[PXUIField(DisplayName = "Payment Method", Visible = true)]
		public virtual String PaymentMethodIDCopy
		{
			get
			{
				return this._PaymentMethodID;
			}
			set
			{
				this._PaymentMethodID = value;
			}
		}
		#endregion
		#region PMInstanceIDCopy
		public abstract class pMInstanceIDCopy : PX.Data.IBqlField
		{
		}
		[PXInt()]
		[PXUIField(DisplayName = "Card/Account No")]
		[PXSelector(typeof(Search<CustomerPaymentMethod.pMInstanceID,
									Where<CustomerPaymentMethod.bAccountID, Equal<Current<CABankTran.payeeBAccountID>>,
									  And<CustomerPaymentMethod.paymentMethodID, Equal<Current<CABankTran.paymentMethodID>>,
									  And<CustomerPaymentMethod.isActive, Equal<boolTrue>>>>>),
									  DescriptionField = typeof(CustomerPaymentMethod.descr))]
		public virtual Int32? PMInstanceIDCopy
		{
			get
			{
				return this._PMInstanceID;
			}
			set
			{
				this._PMInstanceID = value;
			}
		}
		#endregion
        #region RuleID
        public abstract class ruleID : IBqlField { }

        protected int? _RuleID;

        /// <summary>
        /// Identifier of the <see cref="CABankTranRule">Rule</see> applied to the bank transaction
        /// to create a document in the system.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="CABankTranRule.RuleID"/> field.
        /// </value>
        [PXDBInt]
        [PXSelector(typeof(CABankTranRule.ruleID), SubstituteKey = typeof(CABankTranRule.ruleCD))]
        [PXUIField(DisplayName = "Applied Rule", Enabled = false)]
        public int? RuleID
        {
            get { return _RuleID; }
            set { _RuleID = value; }
        }
        #endregion
		#region Hidden
		public abstract class hidden : PX.Data.IBqlField
		{
		}
		protected Boolean? _Hidden;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Hidden", Enabled = false)]
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
		#region InvoiceNotFound
		public abstract class invoiceNotFound : PX.Data.IBqlField
		{
		}
		protected bool? _InvoiceNotFound;
		[PXDBBool()]
		public bool? InvoiceNotFound
		{
			set
			{
				_InvoiceNotFound = value;
			}
			get
			{
				return _InvoiceNotFound;
			}
		}
		#endregion

        #region CountMatches
        public abstract class countMatches : PX.Data.IBqlField
        {
        }
        protected Int32? _CountMatches;
        [PXInt()]
		[PXUIField(Visible = false, Enabled = false)]
        public virtual Int32? CountMatches
        {

            get
            {
                return _CountMatches;
            }
            set
            {
                this._CountMatches = value;
            }
        }
        #endregion
        #region CountInvoiceMatches
        public abstract class countInvoiceMatches : PX.Data.IBqlField
        {
        }
        protected Int32? _CountInvoiceMatches;
        [PXInt()]
        [PXUIField(Visible = false, Enabled = false)]
        public virtual Int32? CountInvoiceMatches
        {

            get
            {
                return _CountInvoiceMatches;
            }
            set
            {
                this._CountInvoiceMatches = value;
            }
        }
        #endregion
        #region ShowCreateDocument
        public abstract class showCreateDocument : PX.Data.IBqlField
        {
        }

        [PXBool()]
        [PXUIField(Visible = false, Enabled = false)]
        public virtual Boolean? ShowCreateDocument
        {

            get
            {
                return (this.CreateDocument == true || this.DocumentMatched == false);
            }
            set
            {

            }
        }
        #endregion
        #region ShowMatches
        public abstract class showMatches : PX.Data.IBqlField
        {
        }

        [PXBool()]
        [PXUIField(Visible = false, Enabled = false)]
        public virtual Boolean? ShowMatches
        {

            get
            {
                return (this.MatchedToExisting== true || (this.CountMatches.HasValue && this.CountMatches > 0));
            }
            set
            {

            }
        }
        #endregion
        #region ShowInvoiceMatches
        public abstract class showInvoiceMatches : PX.Data.IBqlField
        {
        }

        [PXBool()]
        [PXUIField(Visible = false, Enabled = false)]
        public virtual Boolean? ShowInvoiceMatches
        {

            get
            {
                return (this.CountInvoiceMatches.HasValue && this.CountInvoiceMatches > 0);
            }
            set
            {

            }
        }
        #endregion
        #region MatchStatsInfo
        public abstract class matchStatsInfo : PX.Data.IBqlField
        {
        }
        protected String _MatchStatsInfo;
        [PXString]
        [PXUIField(DisplayName = "MatchStatsInfo", Enabled = false, Visibility = PXUIVisibility.Invisible, Visible = false)]
        public virtual String MatchStatsInfo
        {
            get
            {
                return this._MatchStatsInfo;
            }
            set
            {
                this._MatchStatsInfo = value;
            }
        }
        #endregion

        #region AcctName
        public abstract class acctName : PX.Data.IBqlField
        {
        }
        [PXString()]
        [PXSelector(typeof(Search<BAccountR.bAccountID>), SubstituteKey = typeof(BAccountR.acctName))]
        [PXUIField(DisplayName = CR.Messages.BAccountName, Visibility = PXUIVisibility.SelectorVisible, Visible = false)]
        public virtual Int32? AcctName
        {
            get
            {
                return this._PayeeBAccountID;
            }
            set
            {
                this._PayeeBAccountID = value;
            }
        }
        #endregion
        #region PayeeBAccountID1
        public abstract class payeeBAccountID1 : PX.Data.IBqlField
        {
        }
        [PXInt()]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXSelector(typeof(Search<BAccountR.bAccountID>), SubstituteKey = typeof(BAccountR.acctCD), DescriptionField = typeof(BAccountR.acctName))]
        [PXUIField(DisplayName = "Business Account", Visibility = PXUIVisibility.SelectorVisible, Visible = false)]
        public virtual Int32? PayeeBAccountID1
        {
            get
            {
                return this._PayeeBAccountID;
            }
            set
            {
                this._PayeeBAccountID = value;
            }
        }
        #endregion
        #region PayeeLocationID1
        public abstract class payeeLocationID1 : PX.Data.IBqlField
        {
        }
        [PXInt()]
        [PXSelector(typeof(Search<Location.locationID,Where<Location.bAccountID, Equal<Current<CABankTran.payeeBAccountID>>>>),SubstituteKey = typeof(Location.locationCD), DescriptionField = typeof(Location.descr))]
        [PXUIField(DisplayName="Location", Visible = false, Visibility = PXUIVisibility.SelectorVisible)]
        [PXDefault(typeof(Search<BAccountR.defLocationID, Where<BAccountR.bAccountID, Equal<Current<CABankTran.payeeBAccountID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Int32? PayeeLocationID1
        {
            get
            {
                return this._PayeeLocationID;
            }
            set
            {
                this._PayeeLocationID = value;
            }
        }
        #endregion
        #region PaymentMethodID1
        public abstract class paymentMethodID1 : PX.Data.IBqlField
        {
        }
        [PXString(10, IsUnicode = true)]
        [PXDefault(typeof(Coalesce<
                            Coalesce<
                                Search2<Customer.defPaymentMethodID,
                                    InnerJoin<PaymentMethod,
                                        On<PaymentMethod.paymentMethodID, Equal<Customer.defPaymentMethodID>,
                                        And<PaymentMethod.useForAR, Equal<True>>>,
                                    InnerJoin<PaymentMethodAccount,
                                        On<PaymentMethodAccount.paymentMethodID, Equal<Customer.defPaymentMethodID>,
                                        And<PaymentMethodAccount.useForAR, Equal<True>,
                                        And<PaymentMethodAccount.cashAccountID, Equal<Current<CABankTran.cashAccountID>>>>>>>,
                                    Where<Current<CABankTran.origModule>, Equal<GL.BatchModule.moduleAR>,
                                        And<Customer.bAccountID, Equal<Current<CABankTran.payeeBAccountID>>>>>,
                                Search2<PaymentMethodAccount.paymentMethodID,
                                    InnerJoin<PaymentMethod, On<PaymentMethodAccount.paymentMethodID, Equal<PaymentMethod.paymentMethodID>,
                                        And<PaymentMethodAccount.cashAccountID, Equal<Current<CABankTran.cashAccountID>>,
                                        And<PaymentMethodAccount.useForAR, Equal<True>>>>>,
                                    Where<Current<CABankTran.origModule>, Equal<GL.BatchModule.moduleAR>,
                                        And<PaymentMethod.useForAR, Equal<True>,
                                        And<PaymentMethod.isActive, Equal<boolTrue>>>>, OrderBy<Asc<PaymentMethodAccount.aRIsDefault, Desc<PaymentMethodAccount.paymentMethodID>>>>>,
                            Coalesce<
                                Search2<Location.vPaymentMethodID,
                                    InnerJoin<Vendor, On<Location.bAccountID, Equal<Vendor.bAccountID>, And<Location.locationID, Equal<Vendor.defLocationID>>>,
                                    InnerJoin<PaymentMethodAccount, On<PaymentMethodAccount.paymentMethodID, Equal<Location.vPaymentMethodID>,
                                        And<PaymentMethodAccount.useForAP, Equal<True>,
                                        And<PaymentMethodAccount.cashAccountID, Equal<Current<CABankTran.cashAccountID>>>>>>>,
                                    Where<Current<CABankTran.origModule>, Equal<GL.BatchModule.moduleAP>,
                                        And<Vendor.bAccountID, Equal<Current<CABankTran.payeeBAccountID>>>>>,
                                Search2<PaymentMethodAccount.paymentMethodID,
                                    InnerJoin<PaymentMethod, On<PaymentMethodAccount.paymentMethodID, Equal<PaymentMethod.paymentMethodID>,
                                        And<PaymentMethodAccount.cashAccountID, Equal<Current<CABankTran.cashAccountID>>,
                                        And<PaymentMethodAccount.useForAP, Equal<True>>>>>,
                                    Where<Current<CABankTran.origModule>, Equal<GL.BatchModule.moduleAP>,
                                        And<PaymentMethod.useForAP, Equal<True>,
                                        And<PaymentMethod.isActive, Equal<boolTrue>>>>, OrderBy<Asc<PaymentMethodAccount.aPIsDefault, Desc<PaymentMethodAccount.paymentMethodID>>>>>>),
                    PersistingCheck = PXPersistingCheck.Nothing)]
        [PXSelector(typeof(Search2<PaymentMethod.paymentMethodID,
                InnerJoin<PaymentMethodAccount, On<PaymentMethodAccount.paymentMethodID,
                Equal<PaymentMethod.paymentMethodID>,
                And<PaymentMethodAccount.cashAccountID, Equal<Current<CABankTran.cashAccountID>>,
                    And<Where2<Where<Current<CABankTran.origModule>, Equal<GL.BatchModule.moduleAP>, And<PaymentMethodAccount.useForAP, Equal<True>>>,
                        Or<Where<Current<CABankTran.origModule>, Equal<GL.BatchModule.moduleAR>, And<PaymentMethodAccount.useForAR, Equal<True>>>>>>>>>,
                Where<PaymentMethod.isActive, Equal<boolTrue>,
                    And<Where2<Where<Current<CABankTran.origModule>, Equal<GL.BatchModule.moduleAP>, And<PaymentMethod.useForAP, Equal<True>>>,
                        Or<Where<Current<CABankTran.origModule>, Equal<GL.BatchModule.moduleAR>, And<PaymentMethod.useForAR, Equal<True>>>>>>>>), DescriptionField = typeof(PaymentMethod.descr))]
        [PXUIField(DisplayName = "Payment Method", Visible = false)]
        public virtual String PaymentMethodID1
        {
            get
            {
                return this._PaymentMethodID;
            }
            set
            {
                this._PaymentMethodID = value;
            }
        }
        #endregion
        #region InvoiceInfo1
        public abstract class invoiceInfo1 : PX.Data.IBqlField
        {
        }
        [PXString(256, IsUnicode = true)]
        [PXUIField(DisplayName = "Invoice Nbr.", Visibility = PXUIVisibility.SelectorVisible, Visible = false)]
        public virtual String InvoiceInfo1
        {
            get
            {
                return this._InvoiceInfo;
            }
            set
            {
                this._InvoiceInfo = value;
            }
        }
        #endregion
        #region EntryTypeID1
        public abstract class entryTypeID1 : PX.Data.IBqlField
        {
        }
        [PXString(10, IsUnicode = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXSelector(typeof(Search2<CAEntryType.entryTypeId,
                              InnerJoin<CashAccountETDetail, On<CashAccountETDetail.entryTypeID, Equal<CAEntryType.entryTypeId>>>,
                              Where<CashAccountETDetail.accountID, Equal<Current<CABankTran.cashAccountID>>,
                                And<CAEntryType.module, Equal<GL.BatchModule.moduleCA>,
                                And<Where<CAEntryType.drCr, Equal<Current<CABankTran.drCr>>>>>>>),
                      DescriptionField = typeof(CAEntryType.descr))]
        [PXUIField(DisplayName = "Entry Type ID", Visibility = PXUIVisibility.SelectorVisible, Visible = false)]
        public virtual String EntryTypeID1
        {
            get
            {
                return this._EntryTypeID;
            }
            set
            {
                this._EntryTypeID = value;
            }
        }
        #endregion
        #region OrigModule1
        public abstract class origModule1 : PX.Data.IBqlField
        {
        }
        [PXString(2, IsFixed = true)]
        [PXStringList(new string[] { GL.BatchModule.AP, GL.BatchModule.AR, GL.BatchModule.CA, }, new string[] { GL.Messages.ModuleAP, GL.Messages.ModuleAR, GL.Messages.ModuleCA })]
        [PXUIField(DisplayName = "Module", Enabled = false, Visibility = PXUIVisibility.SelectorVisible, Visible = false)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual String OrigModule1
        {
            get
            {
                return this._OrigModule;
            }
            set
            {
                this._OrigModule = value;
            }
        }
        #endregion

		#region CuryWOAmt
		public abstract class curyWOAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryWOAmt;
		[PXCurrency(typeof(CABankTran.curyInfoID), typeof(CABankTran.wOAmt))]
		[PXUIField(DisplayName = "Write-Off Amount", Visibility = PXUIVisibility.Visible, Enabled = false)]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Decimal? CuryWOAmt
		{
			get
			{
				return this._CuryWOAmt;
			}
			set
			{
				this._CuryWOAmt = value;
			}
		}
		#endregion
		#region WOAmt
		public abstract class wOAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _WOAmt;
		[PXDecimal(4)]
		public virtual Decimal? WOAmt
		{
			get
			{
				return this._WOAmt;
			}
			set
			{
				this._WOAmt = value;
			}
		}
		#endregion
    }

	public class CABankTranType
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Statement, PaymentImport },
				new string[] { Messages.Statement, Messages.PaymentImport }) { ; }
		}

		public const string Statement = "S";
        public const string PaymentImport = "I";
		
		public class statement : Constant<string>
		{
			public statement() : base(Statement) { ;}
		}

		public class paymentImport : Constant<string>
		{
			public paymentImport() : base(PaymentImport) { ;}
		}
	}
	public class CABankTranStatus
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
			new string[] { Matched, InvoiceMatched, Created, Hidden },
			new string[] { Messages.Matched, Messages.InvoiceMatched, Messages.Created, Messages.Hidden }) { }
		}
		public class ImagesListAttribute : PXImagesListAttribute
		{
			public ImagesListAttribute()
				: base(
			new string[] { Matched, InvoiceMatched, Created, Hidden },
			new string[] { Messages.Matched, Messages.InvoiceMatched, Messages.Created, Messages.Hidden },
			new string[] { Sprite.AliasMain + "@" + Sprite.Main.Link, Sprite.AliasMain + "@" + Sprite.Main.LinkWB, Sprite.AliasMain + "@" + Sprite.Main.RecordAdd, Sprite.AliasMain + "@" + Sprite.Main.Preview }) { }
		}
		public static bool IsMatchedToInvoice(CABankTran tran, CABankTranMatch match)
		{
			return !(match != null && (match.CATranID != null || (match.DocType == CATranType.CABatch && match.DocModule == GL.BatchModule.AP)));
		}
		public const string Matched = "M";
		public const string InvoiceMatched = "I";
		public const string Created = "C";
		public const string Hidden = "H";

		public class hold : Constant<string>
		{
			public hold() : base(Matched) { ;}
		}

		public class balanced : Constant<string>
		{
			public balanced() : base(InvoiceMatched) { ;}
		}

		public class unposted : Constant<string>
		{
			public unposted() : base(Created) { ;}
		}

		public class posted : Constant<string>
		{
			public posted() : base(Hidden) { ;}
		}
	}
}
