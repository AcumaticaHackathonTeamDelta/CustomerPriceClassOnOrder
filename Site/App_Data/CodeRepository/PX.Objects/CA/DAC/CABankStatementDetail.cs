namespace PX.Objects.CA
{
	using System;
	using PX.Data;
	using PX.Objects.CM;
	using PX.Objects.CS;
	using PX.Objects.CR;
	using PX.Objects.AR;
	using PX.Objects.AP;
    using PX.Data.EP;
	using PX.Objects.CA.BankStatementHelpers;


	
	[System.SerializableAttribute()]
	public partial class CABankStatementDetail : PX.Data.IBqlTable, ICADocSource
	{
		#region CashAccountID
		public abstract class cashAccountID : PX.Data.IBqlField	{}

		protected Int32? _CashAccountID;
		[PXDBInt()]
		[PXDefault(typeof(CABankStatement.cashAccountID))]
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
		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
		}

		protected String _RefNbr;
		[PXDBString(15, IsKey = true, IsUnicode = true,InputMask = ">CCCCCCCCCCCCCCC")]
		[PXDBDefault(typeof(CABankStatement.refNbr))]
		[PXParent(typeof(Select<CABankStatement, Where<CABankStatement.refNbr, Equal<Current<CABankStatementDetail.refNbr>>>>))]
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
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineNbr;
		[PXDBInt(IsKey = true)]
		[PXLineNbr(typeof(CABankStatement.lineCntr))]
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
		#region ExtTranID
		public abstract class extTranID : PX.Data.IBqlField
		{
		}
		protected String _ExtTranID;
		[PXDBString(255, IsUnicode = true)]		
		[PXUIField(DisplayName = "Ext. Tran. ID")]
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
        [PXDefault(typeof(CABankStatement.curyID))]
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
        //[PXDBDefault(typeof(CABankStatement.curyInfoID))]
        //[CurrencyInfo(ModuleCode = "CA")]
        [CurrencyInfoConditional(typeof(CABankStatementDetail.createDocument), ModuleCode = "CA")]
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
		[PXUIField(DisplayName = "Tran. Entry Date")]
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
#if UseCuryInfo
		[PXDBCurrency(typeof(CABankStatementDetail.curyInfoID), typeof(CABankStatementDetail.tranAmt))]
#else
		[PXDBCury(typeof(CABankStatementDetail.curyID))]
#endif
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

#if UseCuryInfo
		#region TranAmt
		public abstract class tranAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranAmt;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal,"0.0")]		
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
#endif
		#region OrigCuryID
		public abstract class origCuryID : PX.Data.IBqlField
		{
		}
		protected String _OrigCuryID;
		[PXDBString(5, IsUnicode = true)]
		[PXSelector(typeof(Currency.curyID), CacheGlobal = true)]
		[PXUIField(DisplayName = "Orig. Currency",Visible = false)]
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
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Cury. Orig. Amt.",Visible = false)]
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
		#region PayeeName
		public abstract class payeeName : PX.Data.IBqlField
		{
		}
		protected String _PayeeName;
		[PXDBString(256, IsUnicode = true)]
		[PXUIField(DisplayName = "Payee Name",Visible = false)]
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
		[PXUIField(DisplayName = "Payee Address1",Visible = false)]
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
		[PXUIField(DisplayName = "Payee City",Visible = false)]
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
		[PXUIField(DisplayName = "Payee State",Visible = false)]
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
		[PXUIField(DisplayName = "Payee Postal Code",Visible = false)]
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
		[PXUIField(DisplayName = "Payee Phone",Visible = false)]
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
		[PXUIField(DisplayName = "Tran. Code")]
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
		[PXDBString(2,IsFixed = true)]
		[PXStringList(new string[] { GL.BatchModule.AP, GL.BatchModule.AR, GL.BatchModule.CA,}, new string[] { "AP", "AR", "CA" })]
		[PXUIField(DisplayName = "Module", Enabled = true)]
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
		[PXVendorCustomerSelector(typeof(CABankStatementDetail.origModule))]		
		[PXUIField(DisplayName = "Business Account")]
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

		[LocationID(typeof(Where<Location.bAccountID, Equal<Current<CABankStatementDetail.payeeBAccountID>>>), DisplayName = "Location", DescriptionField = typeof(Location.descr))]
		//[PXUIField(DisplayName = "Location")]
		[PXDefault(typeof(Search<BAccountR.defLocationID, Where<BAccountR.bAccountID, Equal<Current<CABankStatementDetail.payeeBAccountID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
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
                                    Search2<Customer.defPaymentMethodID, InnerJoin<PaymentMethodAccount,
                                       On<PaymentMethodAccount.paymentMethodID, Equal<Customer.defPaymentMethodID>,
                                        And<PaymentMethodAccount.useForAR, Equal<True>,
                                        And<PaymentMethodAccount.cashAccountID, Equal<Current<CABankStatementDetail.cashAccountID>>>>>>,
                                      Where<Current<CABankStatementDetail.origModule>, Equal<GL.BatchModule.moduleAR>,
                                        And<Customer.bAccountID, Equal<Current<CABankStatementDetail.payeeBAccountID>>>>>,
                                    Search<PaymentMethodAccount.paymentMethodID,
                                        Where<Current<CABankStatementDetail.origModule>, Equal<GL.BatchModule.moduleAR>,
                                            And<PaymentMethodAccount.useForAR, Equal<True>,
                                            And<PaymentMethodAccount.cashAccountID, Equal<Current<CABankStatementDetail.cashAccountID>>>>>,
                                                        OrderBy<Desc<PaymentMethodAccount.aRIsDefault>>>>,
                               Search2<PaymentMethod.paymentMethodID, InnerJoin<PaymentMethodAccount, On<PaymentMethodAccount.paymentMethodID,
                                           Equal<PaymentMethod.paymentMethodID>,
                                             And<PaymentMethodAccount.cashAccountID, Equal<Current<CABankStatementDetail.cashAccountID>>,
                                             And<PaymentMethodAccount.useForAP, Equal<True>>>>>,
                                           Where<Current<CABankStatementDetail.origModule>, Equal<GL.BatchModule.moduleAP>,
                                           And<PaymentMethod.useForAP, Equal<True>,
                                           And<PaymentMethod.isActive, Equal<boolTrue>>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]

        [PXSelector(typeof(Search2<PaymentMethod.paymentMethodID,
                InnerJoin<PaymentMethodAccount, On<PaymentMethodAccount.paymentMethodID,
                Equal<PaymentMethod.paymentMethodID>,
                And<PaymentMethodAccount.cashAccountID, Equal<Current<CABankStatementDetail.cashAccountID>>,
                    And<Where2<Where<Current<CABankStatementDetail.origModule>, Equal<GL.BatchModule.moduleAP>, And<PaymentMethodAccount.useForAP, Equal<True>>>,
                        Or<Where<Current<CABankStatementDetail.origModule>, Equal<GL.BatchModule.moduleAR>, And<PaymentMethodAccount.useForAR, Equal<True>>>>>>>>>,
                Where<PaymentMethod.isActive, Equal<boolTrue>,
                    And<Where2<Where<Current<CABankStatementDetail.origModule>, Equal<GL.BatchModule.moduleAP>, And<PaymentMethod.useForAP, Equal<True>>>,
                        Or<Where<Current<CABankStatementDetail.origModule>, Equal<GL.BatchModule.moduleAR>, And<PaymentMethod.useForAR, Equal<True>>>>>>>>), DescriptionField = typeof(PaymentMethod.descr))]            
		[PXUIField(DisplayName = "Payment Method", Visible = true)]
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
        [PXUIField(DisplayName = "Card/Account No")]
        [PXDefault(typeof(Coalesce<
                                Search2<Customer.defPMInstanceID, InnerJoin<CustomerPaymentMethod, On<CustomerPaymentMethod.pMInstanceID, Equal<Customer.defPMInstanceID>,
                                And<CustomerPaymentMethod.bAccountID, Equal<Customer.bAccountID>>>>,
                                Where<Current<CABankStatementDetail.origModule>, Equal<GL.BatchModule.moduleAR>,
                                And<Customer.bAccountID, Equal<Current<CABankStatementDetail.payeeBAccountID>>,
								And<CustomerPaymentMethod.isActive, Equal<True>,
                                And<CustomerPaymentMethod.paymentMethodID, Equal<Current<CABankStatementDetail.paymentMethodID>>>>>>>,
                              Search<CustomerPaymentMethod.pMInstanceID,
                                    Where<Current<CABankStatementDetail.origModule>, Equal<GL.BatchModule.moduleAR>,
                                   And<CustomerPaymentMethod.bAccountID, Equal<Current<CABankStatementDetail.payeeBAccountID>>,
                                    And<CustomerPaymentMethod.paymentMethodID, Equal<Current<CABankStatementDetail.paymentMethodID>>,
									And<CustomerPaymentMethod.isActive, Equal<True>>>>>,
								OrderBy<Desc<CustomerPaymentMethod.expirationDate,
								Desc<CustomerPaymentMethod.pMInstanceID>>>>>),
                                    PersistingCheck = PXPersistingCheck.Nothing)]
        [PXSelector(typeof(Search<CustomerPaymentMethod.pMInstanceID,
                                    Where<CustomerPaymentMethod.bAccountID, Equal<Current<CABankStatementDetail.payeeBAccountID>>,
                                      And<CustomerPaymentMethod.paymentMethodID, Equal<Current<CABankStatementDetail.paymentMethodID>>,
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
		[PXUIField(DisplayName = "Invoice Nbr.")]
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
        #region PaymentType
        //public abstract class paymentType : PX.Data.IBqlField
        //{
        //}
        //protected String _paymentType;
        //[PXString(3)]        
        //[APPaymentType.List()]
        //[PXUIField(Visibility = PXUIVisibility.Invisible, Enabled = false, TabOrder = 0)]
        //[PXFieldDescription]
        //public virtual String PaymentType
        //{
        //    get
        //    {
        //        if (this.DrCr == CADrCr.CACredit)
        //        {
        //            return APDocType.Prepayment;
        //        }
        //        else
        //        {
        //            return APDocType.Refund;
        //        }
        //    }
        //    set
        //    {
        //        this._paymentType = value;
        //    }
        //}
        #endregion

		#region CATranID
		public abstract class cATranID : PX.Data.IBqlField
		{
		}
		protected Int64? _CATranID;		

		[PXDBLong()]
		[PXUIField(DisplayName="CA TranID",Visible = false)]
		public virtual Int64? CATranID
		{
			get
			{
				return this._CATranID;
			}
			set
			{
				this._CATranID = value;
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
		#region CreateDocument
		public abstract class createDocument : PX.Data.IBqlField
		{
		}
		protected Boolean? _CreateDocument;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Create Doc.")]
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
		#region EntryTypeID
		public abstract class entryTypeID : PX.Data.IBqlField
		{
		}
		protected String _EntryTypeID;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXSelector(typeof(Search2<CAEntryType.entryTypeId,
							  InnerJoin<CashAccountETDetail, On<CashAccountETDetail.entryTypeID, Equal<CAEntryType.entryTypeId>>>,
							  Where<CashAccountETDetail.accountID, Equal<Current<CABankStatement.cashAccountID>>,
								And<CAEntryType.module, Equal<GL.BatchModule.moduleCA>,
								And<Where<CAEntryType.drCr, Equal<Current<CABankStatementDetail.drCr>>>>>>>),
					  DescriptionField = typeof(CAEntryType.descr))]
		[PXUIField(DisplayName = "Entry Type ID", Visibility = PXUIVisibility.SelectorVisible)]
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

		//[PXDecimal()]
		[PXCury(typeof(CABankStatementDetail.curyID))]
		[PXUIField(DisplayName = "Receipt")]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXFormula(null, typeof(SumCalc<CABankStatement.curyDebitsTotal>))]
		public virtual Decimal? CuryDebitAmt
		{
			[PXDependsOnFields(typeof(drCr),typeof(curyTranAmt))]
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

		//[PXDecimal()]
		[PXCury(typeof(CABankStatementDetail.curyID))]
		[PXUIField(DisplayName = "Disbursement")]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXFormula(null, typeof(SumCalc<CABankStatement.curyCreditsTotal>))]
		public virtual Decimal? CuryCreditAmt
		{
			[PXDependsOnFields(typeof(drCr),typeof(curyTranAmt))]
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
		//[PXDecimal()]
		[PXCury(typeof(CABankStatementDetail.curyID))]
		[PXFormula(null, typeof(SumCalc<CABankStatement.curyReconciledDebits>))]
		public virtual Decimal? CuryReconciledDebit
		{
			[PXDependsOnFields(typeof(documentMatched),typeof(curyDebitAmt))]
			get
			{
				return (this._DocumentMatched == true ? this.CuryDebitAmt: Decimal.Zero);
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
		//[PXDecimal()]
		[PXCury(typeof(CABankStatementDetail.curyID))]
		[PXFormula(null, typeof(SumCalc<CABankStatement.curyReconciledCredits>))]
		public virtual Decimal? CuryReconciledCredit
		{
			[PXDependsOnFields(typeof(documentMatched),typeof(curyCreditAmt))]
			get
			{
				return (this.DocumentMatched == true)? this.CuryCreditAmt : Decimal.Zero;
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
		[PXFormula(null, typeof(SumCalc<CABankStatement.countDebit>))]
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
		[PXFormula(null, typeof(SumCalc<CABankStatement.countCredit>))]
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
		[PXFormula(null, typeof(SumCalc<CABankStatement.reconciledCountDebit>))]
		public virtual Int32? ReconciledCountDebit
		{
			[PXDependsOnFields(typeof(documentMatched),typeof(countDebit))]
			get
			{
				return (this.DocumentMatched == true)? this.CountDebit: 0;
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
		[PXFormula(null, typeof(SumCalc<CABankStatement.reconciledCountCredit>))]
		public virtual Int32? ReconciledCountCredit
		{
			[PXDependsOnFields(typeof(documentMatched),typeof(countCredit))]
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
			{		;
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
                return this.CuryTranAmt.HasValue? (this.CuryTranAmt.Value != Decimal.Zero ? this.CuryTranAmt * Math.Sign(this.CuryTranAmt.Value) : Decimal.Zero):null; //Document sign is inverted compared to the CATran's
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

		

		#endregion

        #region TotalAmt
        //public abstract class totalAmt : PX.Data.IBqlField
        //{
        //}
        //protected Decimal? _TotalAmt;
        ////[PXDBBaseCury(typeof(GLTranDoc.ledgerID))]
        //[PXDecimal(4)]
        //[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        ////[PXFormula(null, typeof(SumCalc<GLDocBatch.debitTotal>))]
        //public virtual Decimal? TotalAmt
        //{
        //    get
        //    {                
        //        return this._TotalAmt;
        //    }
        //    set
        //    {
        //        this._TotalAmt = value;
        //    }
        //}
        #endregion
        #region CuryTotalAmt
        public abstract class curyTotalAmount : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryTotalAmt;
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Total Amount")]
        //[PXCurrency(typeof(CABankStatementDetail.curyInfoID), null)]
        [PXCury(typeof(CABankStatementDetail.curyID))]
        public virtual Decimal? CuryTotalAmt
        {
            get
            {
                this._CuryTotalAmt = (this._DrCr == CADrCr.CACredit) ? (- 1 * this._CuryTranAmt) : this._CuryTranAmt;                               
                return this._CuryTotalAmt;
            }
            set
            {
                //this._CuryTotalAmt = value;
            }
        }
        #endregion    

        #region CuryApplAmt
        public abstract class curyApplAmt : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryApplAmt;        
        //[PXDBCurrency(typeof(CABankStatementDetail.curyInfoID), null)]
        [PXDBCury(typeof(CABankStatementDetail.curyID))]
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
        #region ApplAmt
        //public abstract class applAmt : PX.Data.IBqlField
        //{
        //}
        //protected Decimal? _ApplAmt;
        //[PXDBDecimal(4)]
        //public virtual Decimal? ApplAmt
        //{
        //    get
        //    {               
        //        return this._ApplAmt;
        //    }
        //    set
        //    {
        //        this._ApplAmt = value;
        //    }
        //}
        #endregion

        #region CuryUnappliedBal
        public abstract class curyUnappliedBal : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryUnappliedBal;
        //[PXCurrency(typeof(CABankStatementDetail.curyInfoID))]
        [PXCury(typeof(CABankStatementDetail.curyID))]
        [PXUIField(DisplayName = "Unapplied Balance", Visibility = PXUIVisibility.Visible, Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? CuryUnappliedBal
        {
            [PXDependsOnFields(typeof(curyTotalAmount), typeof(curyApplAmt))]
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
        #region UnappliedBal
        //public abstract class unappliedBal : PX.Data.IBqlField
        //{
        //}
        //protected Decimal? _UnappliedBal;
        //[PXDecimal(4)]
        //public virtual Decimal? UnappliedBal
        //{
        //    [PXDependsOnFields(typeof(totalAmt), typeof(applAmt))]
        //    get
        //    {
        //        return ((this.TotalAmt ?? Decimal.Zero) - (this.ApplAmt ?? Decimal.Zero));
        //    }
        //    set
        //    {
        //        //this._ApplAmt = value;
        //    }
        //}
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
	}	
}


