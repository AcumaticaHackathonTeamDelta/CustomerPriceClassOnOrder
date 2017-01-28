#define CalcTaxInTotals 
namespace PX.Objects.GL
{
    using System;
    using PX.Data;
    using PX.Objects.CM;
    using PX.Objects.CS;
    using PX.Objects.PM;
    using PX.Objects.CR;
    using PX.Objects.CA;
    using PX.Objects.AR;
    using PX.Objects.TX;


    /// <summary>
    /// Represents a line of a <see cref="GLDocBatch">GL document batch</see> and contains all information
    /// required to create the corresponding document or transaction.
    /// Records of this type appear in the details grid of the Journal Vouchers (GL.30.40.00) page.
    /// </summary>
    [System.SerializableAttribute()]
    [PXCacheName(Messages.TransactionDoc)]
	public partial class GLTranDoc:		
		PX.Data.IBqlTable, IInvoice
    {
        #region BranchID
        public abstract class branchID : PX.Data.IBqlField
        {
        }
        protected Int32? _BranchID;

        /// <summary>
        /// Identifier of the <see cref="Branch"/>, to which the parent <see cref="GLTranDoc">document batch</see> belongs.
        /// </summary>
        /// <value>
        /// The value of this field is obtained from the <see cref="GLDocBatch.BranchID"/> field of the parent batch.
        /// Corresponds to the <see cref="Branch.BranchID"/> field.
        /// </value>
        [Branch(typeof(GLDocBatch.branchID))]
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
        #region Module
        public abstract class module : PX.Data.IBqlField
        {
        }
        protected String _Module;

        /// <summary>
        /// Key field.
        /// The code of the module, to which the parent <see cref="GLDocBatch">batch</see> belongs.
        /// </summary>
        /// <value>
        /// Defaults to the value of the <see cref="GLDocBatch.Module"/> field of the parent batch.
        /// This field is not supposed to be changed directly.
        /// Possible values are:
        /// "GL", "AP", "AR", "CM", "CA", "IN", "DR", "FA", "PM", "TX", "SO", "PO".
        /// </value>
        [PXDBString(2, IsKey = true, IsFixed = true)]
        [PXDBDefault(typeof(GLDocBatch))]
        [PXUIField(DisplayName = "Batch Module", Visibility = PXUIVisibility.Visible, Visible = false)]
        public virtual String Module
        {
            get
            {
                return this._Module;
            }
            set
            {
                this._Module = value;
            }
        }
        #endregion
        #region BatchNbr
        public abstract class batchNbr : PX.Data.IBqlField
        {
        }
        protected String _BatchNbr;

        /// <summary>
        /// Key field.
        /// Auto-generated unique number of the parent <see cref="GLDocBatch">document batch</see>.
        /// </summary>
        /// <value>
        /// The number is obtained from the <see cref="GLDocBatch.BatchNbr"/> field of the parent batch.
        /// </value>
        [PXDBString(15, IsUnicode = true, IsKey = true)]
        [PXDBDefault(typeof(GLDocBatch))]
        [PXParent(typeof(Select<GLDocBatch, Where<GLDocBatch.module, Equal<Current<GLTranDoc.module>>, And<GLDocBatch.batchNbr, Equal<Current<GLTranDoc.batchNbr>>>>>))]
        [PXUIField(DisplayName = "Batch Number", Visibility = PXUIVisibility.Visible, Visible = false)]
        public virtual String BatchNbr
        {
            get
            {
                return this._BatchNbr;
            }
            set
            {
                this._BatchNbr = value;
            }
        }
        #endregion
        #region LineNbr
        public abstract class lineNbr : PX.Data.IBqlField
        {
        }
        protected Int32? _LineNbr;

        /// <summary>
        /// Key field.
        /// The number of the document/transaction line inside the <see cref="GLDocBatch">document batch</see>.
        /// </summary>
        /// <value>
        /// Note that the sequence of line numbers of the transactions belonging to a single document may include gaps.
        /// </value>
        [PXDBInt(IsKey = true)]
        [PXDefault()]
        [PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false, Enabled = false)]
        [PXLineNbr(typeof(GLDocBatch.lineCntr))]
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
		#region ImportRefNbr
		public abstract class importRefNbr : PX.Data.IBqlField
		{
		}
		protected String _ImportRefNbr;
		[PXString(15, IsUnicode = true)]
		public virtual String ImportRefNbr
		{
			get
			{
				return this._ImportRefNbr;
			}
			set
			{
				this._ImportRefNbr = value;
			}
		}
        #endregion
        #region LedgerID
        public abstract class ledgerID : PX.Data.IBqlField
        {
        }
        protected Int32? _LedgerID;

        /// <summary>
        /// Identifier of the <see cref="Ledger"/>, to which the parent <see cref="GLDocBatch">document batch</see> belongs.
        /// </summary>
        /// <value>
        /// The value of this field is obtained from the <see cref="GLDocBatch.LedgerID"/> field of the parent batch.
        /// Corresponds to the <see cref="Ledger.LedgerID"/> field.
        /// </value>
        [PXDBInt()]
        [PXDBDefault(typeof(GLDocBatch))]
        public virtual Int32? LedgerID
        {
            get
            {
                return this._LedgerID;
            }
            set
            {
                this._LedgerID = value;
            }
        }
        #endregion
		#region ParentLineNbr
		public abstract class parentLineNbr : PX.Data.IBqlField
        {
        }
		protected Int32? _ParentLineNbr;

        /// <summary>
        /// The <see cref="LineNbr">line number</see> of the line, which represents the parent document for this line of the batch.
        /// This field is used to define the details (lines) of a document included in the <see cref="GLDocBatch"/>.
        /// For more information see <see cref="Split"/> field and the documentation for the Journal Vouchers (GL.30.40.00) page.
        /// </summary>
		[PXDBInt()]
		[PXParent(typeof(Select<GLTranDoc, Where<GLTranDoc.lineNbr, Equal<Current<GLTranDoc.parentLineNbr>>,
		                                    And<GLTranDoc.module,Equal<Current<GLTranDoc.module>>,
		                                    And<GLTranDoc.batchNbr,Equal<Current<GLTranDoc.batchNbr>>>>>>))]		
		[PXUIField(DisplayName = "Parent Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = true, Enabled = false)]		
		public virtual Int32? ParentLineNbr
        {
            get
            {
				return this._ParentLineNbr;
            }
            set
            {
				this._ParentLineNbr = value;
            }
        }
        #endregion

        #region Split
        public abstract class split : IBqlField
        {
        }
        protected bool? _Split;

        /// <summary>
        /// When set to <c>true</c>, indicates that the document or transaction created from this line should include
        /// the details defined by other <see cref="GLTranDoc">lines</see> of the batch.
        /// See also the <see cref="ParentLineNbr"/> field.
        /// </summary>
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Split")]
        public bool? Split
        {
            get
            {
                return _Split;
            }
            set
            {
                _Split = value;
            }
        }
        #endregion
        #region ProjectID
        public abstract class projectID : PX.Data.IBqlField
        {
        }
        protected Int32? _ProjectID;

        /// <summary>
        /// Identifier of the <see cref="PMProject">Project</see> associated with the document or transaction that the system creates from the line,
        /// or the <see cref="PMSetup.NonProjectCode">non-project code</see> indicating that the document or transaction is not associated with any particular project.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PMProject.ProjectID"/> field.
        /// </value>
		[ProjectDefault(null)]
		[ActiveProjectForModule(typeof(GLTranDoc.tranModule), typeof(GLTranDoc.bAccountID), false)]        
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

        /// <summary>
        /// Identifier of the <see cref="PMTask">Task</see> associated with the document or transaction that the system creates from the line.
        /// The field is relevant only if the Projects module has been activated and integrated with the module of the document or transaction.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PMTask.TaskID"/> field.
        /// </value>
		[ActiveProjectTask(typeof(GLTranDoc.projectID), typeof(GLTranDoc.tranModule), typeof(needTaskValidation), DisplayName = "Project Task")]
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
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
        {
        }
		protected String _CuryID;

        /// <summary>
        /// Identifier of the <see cref="Currency"/> of the document or transaction that the system creates from the line of the batch.
        /// </summary>
		[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL")]
		[PXUIField(DisplayName = "Currency", Visible = true, Enabled = false)]
		[PXDefault(typeof(GLDocBatch.curyID))]
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
        #region TranCode
        public abstract class tranCode : PX.Data.IBqlField
        {
        }
        protected String _TranCode;

        /// <summary>
        /// Entry code that defines the <see cref="GLTranCode.Module">Module</see>
        /// and the <see cref="GLTranCode.TranType">Type</see> of the document or transaction that the system creates from this line of the batch.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="GLTranCode.TranCode"/> field.
        /// </value>
        [PXString(5, IsUnicode = true,InputMask=">aaaaa")]
        [PXDBScalar(typeof(Search<GLTranCode.tranCode,Where<GLTranCode.module,Equal<GLTranDoc.tranModule>,
                            And<GLTranCode.tranType,Equal<GLTranDoc.tranType>>>>))]
        [PXSelector(typeof(Search<GLTranCode.tranCode, Where<GLTranCode.active, Equal<True>>>),
				typeof(GLTranCode.tranCode), 
				typeof(GLTranCode.module),
				typeof(GLTranCode.tranType),
				typeof(GLTranCode.descr))]
        [PXUIField(DisplayName = "Tran Code",Required=true)]
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
        #region TranModule
        public abstract class tranModule : PX.Data.IBqlField
        {
        }
        protected String _TranModule;

        /// <summary>
        /// The module of the transaction or document that the system creates from this line of the batch.
        /// </summary>
        /// <value>
        /// The value of this field is defined by the <see cref="GLTranCode">Entry Code</see> specified in the <see cref="TranCode"/> field.
        /// </value>
        [PXDBString(2, IsFixed = true)]
        [PXDefault(typeof(Search<GLTranCode.module, Where<GLTranCode.tranCode, Equal<Current<GLTranDoc.tranCode>>>>))]
        [PXUIField(DisplayName = "Tran. Module", Visibility = PXUIVisibility.Visible, Visible = false)]
        public virtual String TranModule
        {
            get
            {
                return this._TranModule;
            }
            set
            {
                this._TranModule = value;
            }
        }
        #endregion
        #region TranType
        public abstract class tranType : PX.Data.IBqlField
        {
        }
        protected String _TranType;

        /// <summary>
        /// The type of the transaction or document that the system creates from this line of the batch.
        /// </summary>
        /// <value>
        /// The value of this field is defined by the <see cref="GLTranCode">Entry Code</see> specified in the <see cref="TranCode"/> field.
        /// </value>
        [PXDBString(3, IsFixed = true)]
        [PXDefault(typeof(Search<GLTranCode.tranType, Where<GLTranCode.tranCode, Equal<Current<GLTranDoc.tranCode>>>>))]
        [PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.Visible, Visible=false, Enabled = true)]
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
		#region TranDate
		public abstract class tranDate : PX.Data.IBqlField
		{
		}
        protected DateTime? _TranDate;

        /// <summary>
        /// The date of the transaction or document that the system creates from this line of the batch.
        /// </summary>
        /// <value>
        /// Defaults to the <see cref="GLDocBatch.DateEntered">date of the batch</see>.
        /// </value>
		[PXDBDate()]
		[PXDefault(typeof(GLDocBatch.dateEntered))]
		[PXUIField(DisplayName = "Transaction Date")]
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
		#region BAccountID
		public abstract class bAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _BAccountID;

        /// <summary>
        /// Identifier of the <see cref="PX.Objects.AP.Vendor">Vendor</see> or <see cref="PX.Objects.AR.Customer">Customer</see>,
        /// for whom the system will create a document from this line.
        /// </summary>
        /// <value>
        /// The field corresponds to the <see cref="BAccount.BAccountID"/> field and is relevant only for the documents of AP and AR modules.
        /// </value>
		[PXDBInt()]
		//[PXSelector(typeof(Search<BAccountR.bAccountID, Where<BAccountR.type, NotEqual<BAccountType.companyType>,
		//   And<BAccountR.type, NotEqual<BAccountType.prospectType>>>>), SubstituteKey = typeof(BAccountR.acctCD))]
		[PXVendorCustomerSelector(typeof(GLTranDoc.tranModule), typeof(GLTranDoc.curyID), CacheGlobal = true)]
		[PXUIField(DisplayName = "Customer/Vendor", Enabled = true, Visible = true)]
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
		#region LocationID
		public abstract class locationID : PX.Data.IBqlField
		{
		}
		protected Int32? _LocationID;

        /// <summary>
        /// Identifier of the <see cref="Location">Location</see> of the <see cref="BAccountID">Vendor or
        /// Customer</see> associated with this line of the batch.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="Location.LocationID"/> field.
        /// Defaults to vendor's or customer's <see cref="BAccountR.DefLocationID">default location</see>.
        /// </value>
		[LocationID(typeof(Where<Location.bAccountID, Equal<Current<GLTranDoc.bAccountID>>>), DisplayName = "Location", DescriptionField = typeof(Location.descr))]
		//[PXUIField(DisplayName = "Location")]
		[PXDefault(typeof(Search<BAccountR.defLocationID, Where<BAccountR.bAccountID, Equal<Current<GLTranDoc.bAccountID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
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
        
		#region EntryTypeID
		public abstract class entryTypeID : PX.Data.IBqlField
		{
		}
		protected String _EntryTypeID;

        /// <summary>
        /// Identifier of the <see cref="CAEntryType">Entry Type</see> of the CA transaction
        /// that the system creates from this line of the batch.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="CAEntryType.EntryTypeID"/> field.
        /// This field is relevant only for the lines defining the transactions of the CA module.
        /// </value>
		[PXDBString(10, IsUnicode = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXSelector(typeof(Search<CAEntryType.entryTypeId,							  
							  Where<CAEntryType.module, Equal<Current<GLTranDoc.tranModule>>,
							  And<CAEntryType.useToReclassifyPayments,NotEqual<True>>>>),
					  DescriptionField = typeof(CAEntryType.descr))]
		[PXUIField(DisplayName = "Entry Type ID")]
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
		#region CADrCr
		public abstract class cADrCr : PX.Data.IBqlField
		{
		}
		protected string _CADrCr;

        /// <summary>
        /// Indicates whether the CA transaction that the system will create from this line is Receipt or Disbursement.
        /// </summary>
        /// <value>
        /// The value of this field is determined by the <see cref="EntryType"/> of the line.
        /// Possible values are: <c>"D"</c> for Receipt and <c>"C"</c> for Disbursement.
        /// </value>
		[PXString(1, IsFixed = true)]
		[PXDefault(typeof(Search<CAEntryType.drCr, Where<Current<GLTranDoc.tranModule>, Equal<GL.BatchModule.moduleCA>,
							And<CAEntryType.entryTypeId, Equal<Current<GLTranDoc.entryTypeID>>>>>),PersistingCheck = PXPersistingCheck.Nothing)]
		[PXDBScalar(typeof(Search<CAEntryType.drCr, Where<CAEntryType.entryTypeId, Equal<GLTranDoc.entryTypeID>,
								And<GLTranDoc.tranModule, Equal<GL.BatchModule.moduleCA>>>>))]
		public string CADrCr
		{
			get
			{
				return _CADrCr;
			}
			set
			{
				this._CADrCr = value;
			}
		}
		#endregion
        #region PaymentMethodID
        public abstract class paymentMethodID : PX.Data.IBqlField
        {
        }
        protected String _PaymentMethodID;

        /// <summary>
        /// Identifier of the <see cref="PaymentMethod">Payment Method</see> used for the document created from the line.
        /// </summary>
        /// <value>
        /// This field is relevant only for the lines defining documents of AP and AR modules.
        /// Corresponds to the <see cref="PaymentMethod.PaymentMethodID"/> field.
        /// </value>
        [PXDBString(10, IsUnicode = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXSelector(typeof(Search<PaymentMethod.paymentMethodID,
							Where<PaymentMethod.isActive, Equal<True>,
							And<Where2<Where<Current<GLTranDoc.tranModule>, Equal<GL.BatchModule.moduleAP>, 
									And<PaymentMethod.useForAP, Equal<True>,
									And<Where2<Where<PaymentMethod.aPPrintChecks, Equal<False>, 
									And<PaymentMethod.aPCreateBatchPayment, Equal<False>>>,
                                    Or<Current<GLTranDoc.tranType>,Equal<AP.APDocType.invoice>,
                                    Or<Current<GLTranDoc.tranType>,Equal<AP.APDocType.debitAdj>>>>>>>,
								Or<Where<Current<GLTranDoc.tranModule>, Equal<GL.BatchModule.moduleAR>, 
									And<PaymentMethod.useForAR, Equal<True>,
									And<Where<PaymentMethod.aRIsProcessingRequired,Equal<False>,
                                    Or<Current<GLTranDoc.tranType>,Equal<AR.ARDocType.invoice>,
                                    Or<Current<GLTranDoc.tranType>,Equal<AR.ARDocType.debitMemo>>>>>>>>>>>>), 
                                    DescriptionField = typeof(PaymentMethod.descr))]
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
		#region TaxCategoryID
		public abstract class taxCategoryID : PX.Data.IBqlField
		{
		}
        protected String _TaxCategoryID;

        /// <summary>
        /// Identifier of the <see cref="TaxCategory">Tax Category</see> associated with the document or transaction defined by the line.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="TaxCategory.TaxCategoryID"/> field.
        /// </value>
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Tax Category", Visibility = PXUIVisibility.Visible)]
        [PXSelector(typeof(TaxCategory.taxCategoryID), DescriptionField = typeof(TaxCategory.descr))]
        [PXRestrictor(typeof(Where<TaxCategory.active, Equal<True>>), TX.Messages.InactiveTaxCategory, typeof(TaxCategory.taxCategoryID))]
        [GLTax(typeof(GLTranDoc), typeof(GLTax), typeof(GLTaxTran))]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
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
        
        #region DebitAccountID
        public abstract class debitAccountID : PX.Data.IBqlField
        {
        }
        protected Int32? _DebitAccountID;

        /// <summary>
        /// Identifier of the debit <see cref="Account"/> associated with the line of the batch.
        /// </summary>
        /// <value>
        /// For the lines defining GL transactions the value of this field is entered by user.
        /// For the lines used to create AP or AR documents the account is selected automatically once the <see cref="BAccountID">vendor or customer</see> is chosen.
        /// For CA transactions the account is either entered by user (for Receipts) or selected automatically 
        /// based on the <see cref="EntryTypeID">Entry Type</see> (for Disbursements).
        /// Corresponds to the <see cref="Account.AccountID"/> field.
        /// </value>
		[Account(typeof(GLTranDoc.branchID), 
			typeof(Search2<Account.accountID, 
										LeftJoin<CashAccount,On<CashAccount.accountID,Equal<Account.accountID>,
                                            And<CashAccount.branchID, Equal<Current<GLTranDoc.branchID>>,
											And<CashAccount.curyID,Equal<Optional<GLTranDoc.curyID>>>>>,
										LeftJoin<CAEntryType, On<CAEntryType.entryTypeId,Equal<Optional<GLTranDoc.entryTypeID>>>,
										LeftJoin<CashAccountETDetail,On<CashAccountETDetail.accountID, Equal<CashAccount.cashAccountID>,
											And<CashAccountETDetail.entryTypeID,Equal<CAEntryType.entryTypeId>>>>>>, 
										Where<Where2<Where<Optional<GLTranDoc.tranModule>,Equal<GL.BatchModule.moduleCA>,
												And<CAEntryType.entryTypeId,IsNotNull,
												And<Where<CAEntryType.drCr,Equal<CADrCr.cACredit>,
													Or<CashAccountETDetail.accountID,IsNotNull>>>>>,
												Or<Where<Optional<GLTranDoc.tranModule>,NotEqual<GL.BatchModule.moduleCA>,
													And<Where<Optional<GLTranDoc.needsDebitCashAccount>, Equal<False>,
													Or<CashAccount.accountID,IsNotNull>>>>>>>>),
											LedgerID = typeof(GLTranDoc.ledgerID), 
											DescriptionField = typeof(Account.description),
											DisplayName = "Debit Account")]
		
        [PXDefault]
        public virtual Int32? DebitAccountID
        {
            get
            {
                return this._DebitAccountID;
            }
            set
            {
                this._DebitAccountID = value;
            }
        }
        #endregion
        #region DebitSubID
        public abstract class debitSubID : PX.Data.IBqlField
        {
        }
        protected Int32? _DebitSubID;

        /// <summary>
        /// Identifier of the debit <see cref="Sub">Subaccount</see> associated with the line of the batch.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="Sub.SubID"/> field.
        /// For the information related to defaulting of this field see <see cref="DebitAccountID"/>.
        /// </value>
        [SubAccount(typeof(GLTranDoc.debitAccountID), typeof(GLTranDoc.branchID), DisplayName = "Debit Subaccount")]
        [PXDefault]
        public virtual Int32? DebitSubID
        {
            get
            {
                return this._DebitSubID;
            }
            set
            {
                this._DebitSubID = value;
            }
        }
        #endregion
        #region CreditAccountID
        public abstract class creditAccountID : PX.Data.IBqlField
        {
        }
        protected Int32? _CreditAccountID;

        /// <summary>
        /// Identifier of the credit <see cref="Account"/> associated with the line of the batch.
        /// </summary>
        /// <value>
        /// For the lines defining GL transactions the value of this fields is entered by user.
        /// For the lines used to create AP or AR documents the account is selected automatically once the <see cref="BAccountID">vendor or customer</see> is chosen.
        /// For CA transactions the account is either entered by user (for Disbursements) or selected automatically
        /// based on the <see cref="EntryTypeID">Entry Type</see> (for Receipts).
        /// Corresponds to the <see cref="Account.AccountID"/> field.
        /// </value>
		[Account(typeof(GLTranDoc.branchID),
				typeof(Search2<Account.accountID,
										LeftJoin<CashAccount, On<CashAccount.accountID, Equal<Account.accountID>,
                                            And<CashAccount.branchID, Equal<Current<GLTranDoc.branchID>>,
											And<CashAccount.curyID, Equal<Optional<GLTranDoc.curyID>>>>>,
										LeftJoin<CAEntryType, On<CAEntryType.entryTypeId, Equal<Optional<GLTranDoc.entryTypeID>>>,
										LeftJoin<CashAccountETDetail, On<CashAccountETDetail.accountID, Equal<CashAccount.cashAccountID>,
											And<CashAccountETDetail.entryTypeID, Equal<CAEntryType.entryTypeId>>>>>>,
										Where<Where2<Where<Optional<GLTranDoc.tranModule>, Equal<GL.BatchModule.moduleCA>,
												And<CAEntryType.entryTypeId, IsNotNull,
												And<Where<CAEntryType.drCr, Equal<CADrCr.cADebit>,
													Or<CashAccountETDetail.accountID, IsNotNull>>>>>,
												Or<Where<Optional<GLTranDoc.tranModule>, NotEqual<GL.BatchModule.moduleCA>,
													And<Where<Optional<GLTranDoc.needsCreditCashAccount>, Equal<False>,
													Or<CashAccount.accountID, IsNotNull>>>>>>>>),
                                            LedgerID = typeof(GLTranDoc.ledgerID),
                                            DescriptionField = typeof(Account.description),
				    						DisplayName = "Credit Account")]
        [PXDefault]
        public virtual Int32? CreditAccountID
        {
            get
            {
                return this._CreditAccountID;
            }
            set
            {
                this._CreditAccountID = value;
            }
        }
        #endregion
        #region CreditSubID
        public abstract class creditSubID : PX.Data.IBqlField
        {
        }
        protected Int32? _CreditSubID;

        /// <summary>
        /// Identifier of the credit <see cref="Sub">Subaccount</see> associated with the line of the batch.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="Sub.SubID"/> field.
        /// For the information related to defaulting of this field see <see cref="CreditAccountID"/>.
        /// </value>
		[SubAccount(typeof(GLTranDoc.creditAccountID), typeof(GLTranDoc.branchID), DisplayName = "Credit Subaccount")]
        [PXDefault]
        public virtual Int32? CreditSubID
        {
            get
            {
                return this._CreditSubID;
            }
            set
            {
                this._CreditSubID = value;
            }
        }
        #endregion
        #region RefNbr
        public abstract class refNbr : PX.Data.IBqlField
        {
        }
        protected String _RefNbr;

        /// <summary>
        /// Reference number of the document or transaction created from the line.
        /// </summary>
        /// <value>
        /// For the lines defining GL transactions the field corresponds to the <see cref="Batch.BatchNbr"/> field.
        /// For the lines used to create documents in AP and AR modules the field corresponds to the 
        /// <see cref="PX.Objects.AP.APRegister.RefNbr"/> and <see cref="PX.Objects.AR.ARRegister.RefNbr"/> fields, respectively.
        /// For the lines defining CA transactions the field corresponds to the <see cref="CAAdj.AdjRefNbr"/> field.
        /// </value>
        [PXDBString(15, IsUnicode = true)]
        //[PXDefault("", typeof(Search<GLTranDoc.refNbr, Where<GLTranDoc.module, Equal<Current<GLTranDoc.module>>, And<GLTranDoc.batchNbr, Equal<Current<GLTranDoc.batchNbr>>>>>))]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Ref. Number", Visible=true)]
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
		#region DocCreated
		public abstract class docCreated : PX.Data.IBqlField
		{
		}
		protected Boolean? _DocCreated;

        /// <summary>
        /// When <c>true</c>, indicates that the document or transaction defined by the line has been created.
        /// </summary>
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Doc. Created", Enabled = false)]
		public virtual Boolean? DocCreated
		{
			get
			{
				return this._DocCreated;
			}
			set
			{
				this._DocCreated = value;
			}
		}
		#endregion
        #region ExtRefNbr
        public abstract class extRefNbr : PX.Data.IBqlField
        {
        }
        protected String _ExtRefNbr;

        /// <summary>
        /// The reference number of the original vendor or customer document.
        /// </summary>
        [PXDBString(40, IsUnicode = true)]
        [PXDefault()]
        [PXUIField(DisplayName = "Ext. Ref.Number", Visibility = PXUIVisibility.Visible)]
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
#if false
		
        #region InventoryID
        public abstract class inventoryID : PX.Data.IBqlField
        {
        }
        protected Int32? _InventoryID;
        [IN.Inventory(Enabled = false, Visible = false)]
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
        #region UOM
        public abstract class uOM : PX.Data.IBqlField
        {
        }
        protected String _UOM;
        [IN.INUnit(typeof(GLTranDoc.inventoryID), typeof(GLTranDoc.accountID), typeof(GLTranDoc.accountRequireUnits))]
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
        #region Qty
        public abstract class qty : PX.Data.IBqlField
        {
        }
        protected Decimal? _Qty;
        [IN.PXDBQuantity()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Quantity", Visibility = PXUIVisibility.Visible)]
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
  
#endif
        #region TranAmt
        public abstract class tranAmt : PX.Data.IBqlField
        {
        }
        protected Decimal? _TranAmt;

        /// <summary>
        /// The sum of the document or transaction details or lines. Does not include tax amount.
        /// For the batch lines representing details of the document to be created (see <see cref="Split"/> field),
        /// the value of this field defines the line amount.
        /// Given in the <see cref="Company.BaseCuryID">base currency</see> of the company.
        /// See also <see cref="CuryTranAmt"/>.
        /// </summary>
        [PXDBBaseCury(typeof(GLTranDoc.ledgerID))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        //[PXFormula(null, typeof(SumCalc<GLDocBatch.debitTotal>))]
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

		#region TranTotal
		public abstract class tranTotal : PX.Data.IBqlField
        {
        }
        protected Decimal? _TranTotal;

        /// <summary>
        /// The amount of the document or transaction, including the total tax amount.
        /// Given in the <see cref="Company.BaseCuryID">base currency</see> of the company.
        /// See also <see cref="CuryTotalAmt"/>.
        /// </summary>
		[PXDBBaseCury(typeof(GLTranDoc.ledgerID))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		//[PXFormula(null, typeof(SumCalc<GLDocBatch.debitTotal>))]
		public virtual Decimal? TranTotal
        {
            get
            {
				return this._TranTotal;
            }
            set
            {
				this._TranTotal = value;
            }
        }
        #endregion
        
        #region CuryInfoID
        public abstract class curyInfoID : PX.Data.IBqlField
        {
        }
        protected Int64? _CuryInfoID;

        /// <summary>
        /// Identifier of the <see cref="PX.Objects.CM.CurrencyInfo">CurrencyInfo</see> object associated with the line.
        /// </summary>
        /// <value>
        /// Aut-generated. Corresponds to the <see cref="PX.Objects.CM.CurrencyInfo.CurrencyInfoID"/> field.
        /// </value>
		[PXDBLong()]
		[CurrencyInfo(typeof(GLDocBatch.curyInfoID))]
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
		#region CuryTranTotal
		public abstract class curyTranTotal : PX.Data.IBqlField
		{
		}
        protected Decimal? _CuryTranTotal;

        /// <summary>
        /// The amount of the document or transaction, including the total tax amount.
        /// Given in the <see cref="CuryID">currency</see> of the line.
        /// See also <see cref="TotalAmt"/>.
        /// </summary>
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Total Amount", Visibility = PXUIVisibility.Visible)]
		[PXDBCurrency(typeof(GLTranDoc.curyInfoID), typeof(GLTranDoc.tranTotal))]
		public virtual Decimal? CuryTranTotal
		{
			get
			{
				return this._CuryTranTotal;
			}
			set
			{
				this._CuryTranTotal = value;
			}
		}
		#endregion        
        #region CuryTranAmt
        public abstract class curyTranAmt : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryTranAmt;

        /// <summary>
        /// The sum of the document or transaction details or lines. Does not include tax amount.
        /// For the batch lines representing details of the document to be created (see <see cref="Split"/> field),
        /// the value of this field defines the line amount.
        /// Given in the <see cref="CuryID">currency</see> of the line.
        /// See also <see cref="TranAmt"/>.
        /// </summary>
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Subtotal Amount", Visibility = PXUIVisibility.Visible, Enabled =false)]
        [PXDBCurrency(typeof(GLTranDoc.curyInfoID), typeof(GLTranDoc.tranAmt))]
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
        #region Released
        public abstract class released : PX.Data.IBqlField
        {
        }
        protected Boolean? _Released;

        /// <summary>
        /// Indicates whether the document defined by the line has been released.
        /// The system releases the documents once they have been generated.
        /// </summary>
        /// <value>
        /// <c>true</c> if released, otherwise <c>false</c>.
        /// </value>
        [PXDBBool()]
        [PXDefault(false)]
		[PXUIField(DisplayName="Released",Enabled= false)]
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
        
        #region TranClass
        public abstract class tranClass : PX.Data.IBqlField
        {
        }
        protected String _TranClass;

        /// <summary>
        /// Reserved for internal use.
        /// The class of the document or transaction defined by the line.
        /// This field affects posting of documents and transactions to GL.
        /// </summary>
        [PXDBString(1, IsFixed = true)]
        [PXDefault("N")]
        public virtual String TranClass
        {
            get
            {
                return this._TranClass;
            }
            set
            {
                this._TranClass = value;
            }
        }
        #endregion
        #region TranDesc
        public abstract class tranDesc : PX.Data.IBqlField
        {
        }
        protected String _TranDesc;

        /// <summary>
        /// The description of the document or transaction defined by the line.
        /// </summary>
				[PXDBString(256, IsUnicode = true)]
        //[PXDefault(typeof(Search<GLTranDoc.tranDesc, Where<GLTranDoc.module, Equal<Current<GLTranDoc.module>>, And<GLTranDoc.batchNbr, Equal<Current<GLTranDoc.batchNbr>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Transaction Description", Visibility = PXUIVisibility.Visible)]
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
        
        #region TranLineNbr
        public abstract class tranLineNbr : PX.Data.IBqlField
        {
        }
        protected Int32? _TranLineNbr;

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        [PXDBInt()]
        public virtual Int32? TranLineNbr
        {
            get
            {
                return this._TranLineNbr;
            }
            set
            {
                this._TranLineNbr = value;
            }
        }
        #endregion
       
        #region PMInstanceID
        public abstract class pMInstanceID : PX.Data.IBqlField
        {
        }
        protected Int32? _PMInstanceID;

        /// <summary>
        /// Identifier of the <see cref="CustomerPaymentMethod">Customer Payment Method</see> (card or account number) associated with the line.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="CustomerPaymentMethod.PMInstanceID"/> field.
        /// Relevant only for the lines representing AR documents.
        /// </value>
        [PXDBInt()]
        [PXUIField(DisplayName = "Card/Account No")]
        [PXSelector(typeof(Search<CustomerPaymentMethod.pMInstanceID,
                                    Where<CustomerPaymentMethod.bAccountID, Equal<Current<GLTranDoc.bAccountID>>,
                                      And<CustomerPaymentMethod.paymentMethodID, Equal<Current<GLTranDoc.paymentMethodID>>,
                                      And<CustomerPaymentMethod.isActive, Equal<True>>>>>),
                                      DescriptionField = typeof(CustomerPaymentMethod.descr))]        
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
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
        #region FinPeriodID
        public abstract class finPeriodID : PX.Data.IBqlField
        {
        }
        protected String _FinPeriodID;

        /// <summary>
        /// <see cref="FinPeriod">Financial Period</see> of the document or transaction.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="GLDocBatch.FinPeriodID">period of the batch</see>,
        /// which is defined by its <see cref="GLDocBatch.DateEntered">date</see>, but can be overriden by user.
        /// </value>
        [PXDBDefault(typeof(GLDocBatch.finPeriodID))]
        [GL.FinPeriodID()]
        public virtual String FinPeriodID
        {
            get
            {
                return this._FinPeriodID;
            }
            set
            {
                this._FinPeriodID = value;
            }
        }
        #endregion
        #region TranPeriodID
        public abstract class tranPeriodID : PX.Data.IBqlField
        {
        }
        protected String _TranPeriodID;

        /// <summary>
        /// <see cref="FinPeriod">Financial Period</see> of the document or transaction.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="GLDocBatch.TranPeriodID">period of the batch</see>,
        /// which is defined by its <see cref="GLDocBatch.DateEntered">date</see> and can't be changed by user.
        /// </value>
		[TranPeriodID(typeof(GLTranDoc.tranDate))]
        public virtual String TranPeriodID
        {
            get
            {
                return this._TranPeriodID;
            }
            set
            {
                this._TranPeriodID = value;
            }
        }
        #endregion
        #region PMTranID
        public abstract class pMTranID : PX.Data.IBqlField
        {
        }
        protected Int64? _PMTranID;

        /// <summary>
        /// Identifier of the <see cref="PMTran">Project Transactions</see> assoicated with the document or transactions, represented by the line.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PMTran.TranID"/> field.
        /// </value>
        [PXDBChildIdentity(typeof(PMTran.tranID))]
        [PXDBLong()]
        public virtual Int64? PMTranID
        {
            get
            {
                return this._PMTranID;
            }
            set
            {
                this._PMTranID = value;
            }
        }
        #endregion                
        #region LedgerBalanceType
        public abstract class ledgerBalanceType : PX.Data.IBqlField
        {
        }
        protected string _LedgerBalanceType;

        /// <summary>
        /// The type of balance of the <see cref="Ledger"/>, associated with the line.
        /// </summary>
        /// <value>
        /// Possible values are:
        /// <c>"A"</c> - Actual,
        /// <c>"R"</c> - Reporting,
        /// <c>"S"</c> - Statistical,
        /// <c>"B"</c> - Budget.
        /// Corresponds to the <see cref="Ledger.BalanceType"/> field.
        /// </value>
        [PXString(1, IsFixed = true, InputMask = "")]
        public virtual string LedgerBalanceType
        {
            get
            {
                return this._LedgerBalanceType;
            }
            set
            {
                this._LedgerBalanceType = value;
            }
        }
        #endregion
        #region AccountBranchID
		[Obsolete(Common.Messages.FieldIsObsoleteRemoveInAcumatica7)]
        public abstract class accountBranchID : IBqlField { }
        [PXInt]
		[Obsolete(Common.Messages.FieldIsObsoleteRemoveInAcumatica7)]
        public virtual Int32? AccountBranchID
        {
            get;
            set;
        }
        #endregion
        #region TermsID
        public abstract class termsID : PX.Data.IBqlField
        {
        }
        protected String _TermsID;

        /// <summary>
        /// Identifier of the <see cref="Terms">Credit Terms</see> record associated with the line.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="Terms.TermsID"/> field.
        /// </value>
        [PXDBString(10, IsUnicode = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Terms", Visibility = PXUIVisibility.Visible)]
        [PXSelector(typeof(Search<Terms.termsID, Where<Terms.visibleTo, Equal<TermsVisibleTo.all>, Or<Terms.visibleTo, Equal<TermsVisibleTo.customer>>>>), DescriptionField = typeof(Terms.descr), Filterable = true)]
		[Terms(typeof(GLTranDoc.tranDate), typeof(GLTranDoc.dueDate), typeof(GLTranDoc.discDate), typeof(GLTranDoc.curyTranTotal), typeof(GLTranDoc.curyDiscAmt))]
        public virtual String TermsID
        {
            get
            {
                return this._TermsID;
            }
            set
            {
                this._TermsID = value;
            }
        }
        #endregion
        #region DueDate
        public abstract class dueDate : PX.Data.IBqlField
        {
        }
        protected DateTime? _DueDate;

        /// <summary>
        /// The due date of the document defined by the line, if applicable.
        /// </summary>
        [PXDBDate()]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Due Date", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual DateTime? DueDate
        {
            get
            {
                return this._DueDate;
            }
            set
            {
                this._DueDate = value;
            }
        }
        #endregion
        #region DiscDate
        public abstract class discDate : PX.Data.IBqlField
        {
        }
        protected DateTime? _DiscDate;

        /// <summary>
        /// The date of the cash discount for the document, if applicable.
        /// </summary>
        [PXDBDate()]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Cash Discount Date", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual DateTime? DiscDate
        {
            get
            {
                return this._DiscDate;
            }
            set
            {
                this._DiscDate = value;
            }
        }
        #endregion
        #region CuryDiscAmt
        public abstract class curyDiscAmt : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryDiscAmt;
        //[PXDefault(TypeCode.Decimal, "0.0")]

        /// <summary>
        /// The amount of the cash discount for the document (if applicable),
        /// given in the <see cref="CuryID">currency</see> of the line.
        /// See also <see cref="DiscAmt"/>.
        /// </summary>
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXDBCurrency(typeof(GLTranDoc.curyInfoID), typeof(GLTranDoc.discAmt))]
        [PXUIField(DisplayName = "Cash Discount", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual Decimal? CuryDiscAmt
        {
            get
            {
                return this._CuryDiscAmt;
            }
            set
            {
                this._CuryDiscAmt = value;
            }
        }
        #endregion
        #region DiscAmt
        public abstract class discAmt : PX.Data.IBqlField
        {
        }
        protected Decimal? _DiscAmt;

        /// <summary>
        /// The amount of the cash discount for the document (if applicable),
        /// given in the <see cref="Company.BaseCuryID">base currency</see> of the company.
        /// See also <see cref="DiscAmt"/>.
        /// </summary>
        [PXDBBaseCury()]
        //[PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? DiscAmt
        {
            get
            {
                return this._DiscAmt;
            }
            set
            {
                this._DiscAmt = value;
            }
        }
        #endregion		
        #region DrCr
#if false
		
        public abstract class drCr : PX.Data.IBqlField
        {
        }
        protected string _DrCr;
        [PXString(1, IsFixed = true)]
        public string DrCr
        {
            get
            {
                string result = null;
                if (this.TranModule == BatchModule.AP)
                {
                     result = AP.APPaymentType.DrCr(this._TranType);
                     if (String.IsNullOrEmpty(result)) 
                     {
                         result = AP.APInvoiceType.DrCr(this._TranType);
                     }                     
                }
                if (this.TranModule == BatchModule.AR)
                {                    
                    result = AR.ARPaymentType.DrCr(this._TranType);
                    if (String.IsNullOrEmpty(result))
                    {
                        result = AR.ARInvoiceType.DrCr(this._TranType);    
                    }
                }
                return result;
            }
            set
            {
            }
        }
  
	#endif      
        #endregion
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
        {
        }
        protected Guid? _NoteID;

        /// <summary>
        /// Identifier of the <see cref="PX.Data.Note">Note</see> object, associated with the document.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Data.Note.NoteID">Note.NoteID</see> field. 
        /// </value>
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

        #region CuryBalanceAmt

        protected Decimal? _CuryBalanceAmt;

        /// <summary>
        /// The open balance of the parent document (represented by another line of the batch) for the moment when
        /// the child line is inserted.
        /// This field is used to determine the default value of the <see cref="CuryTranAmt"/> for the lines,
        /// representing details of a document to be created from another line of the batch.
        /// Given in the <see cref="CuryID">currency</see> of the line.
        /// </summary>
        [PXDecimal(4)]        
        [PXUIField(DisplayName = "Tran Amount", Visibility = PXUIVisibility.Visible)]
        public virtual Decimal? CuryBalanceAmt
        {
            get
            {
                return this._CuryBalanceAmt;
            }
            set
            {
                this._CuryBalanceAmt = value;
            }
        }

        //protected bool? _NeedsInversion;
        //public bool? NeedsInversion
        //{
        //    set
        //    {
        //        this._NeedsInversion = value;
        //    }
        //    get
        //    {
        //        return this._NeedsInversion;
        //    }
        //}
        //
        #endregion   

        #region GroupTranID
        public abstract class groupTranID : PX.Data.IBqlField
        {
        }

        /// <summary>
        /// Read-only identifier of the group of batch lines.
        /// </summary>
        /// <value>
        /// Batch lines are grouped by document or transaction, so that for the lines describing different documents
        /// the values of this field are different, and for the lines defining header or lines of the same document
        /// (see the <see cref="Split"/> field) the GroupTranID value is the same.
        /// Depends only on the <see cref="LineNbr"/> and <see cref="ParentLineNbr"/> fields.
        /// </value>
        [PXInt()]
        [PXUIField(FieldName = "Group Tran ID", Visible = false, Enabled =false)]
        public virtual Int32? GroupTranID
        {
            //[PXDependsOnFields(typeof(parentTranID),typeof(tranID))]
			[PXDependsOnFields(typeof(parentLineNbr), typeof(lineNbr))]
            get
            {
                //int? id = this.ParentTranID.HasValue? this.ParentTranID.Value: this.TranID;
                //return ((id.HasValue && id <0)? (id - int.MinValue) + (int.MaxValue/2): id);
				int? id = this.ParentLineNbr.HasValue ? this.ParentLineNbr.Value : this.LineNbr;
				return id * 10000;
            }
            set
            {
                
            }
        }
        #endregion
        #region CashAccountID
        public abstract class cashAccountID : PX.Data.IBqlField
        {
        }

        /// <summary>
        /// Read-only identifier of the <see cref="CashAccount">Cash Account</see> associated with the document or transaction defined by the line.
        /// </summary>
        /// <value>
        /// Depending on the <see cref="TranModule"/> and <see cref="TranType"/> returns either <see cref="DebitCashAccountID"/> or <see cref="CreditCashAccountID"/>.
        /// </value>
        [PXInt()]
        public virtual Int32? CashAccountID
        {
            [PXDependsOnFields(typeof(tranModule), typeof(tranType), typeof(creditCashAccountID), typeof(debitCashAccountID), typeof(CADrCr))]
            get
            {
                int? acctID = null;
                if (this._TranModule == BatchModule.AP)
                {
                    if (this._TranType == AP.APPaymentType.Check || this._TranType == AP.APPaymentType.Prepayment
                        || this._TranType == AP.APPaymentType.QuickCheck)
                        acctID = this.CreditCashAccountID;
                    if (this.TranType == AP.APPaymentType.Refund || this.TranType == AP.APPaymentType.VoidCheck
                        || this.TranType == AP.APPaymentType.VoidQuickCheck)
                        acctID = this.DebitCashAccountID;
                }

                if (this._TranModule == BatchModule.AR)
                {
                    if (this._TranType == AR.ARPaymentType.Payment || this._TranType == AR.ARPaymentType.Prepayment
                        || this._TranType == AR.ARPaymentType.CashSale)
                        acctID = this.DebitCashAccountID;
                    if (this.TranType == AR.ARPaymentType.Refund || this.TranType == AR.ARPaymentType.VoidPayment
                        || this.TranType == AR.ARPaymentType.CashReturn)
                        acctID = this.CreditCashAccountID;
                }
                if (this._TranModule == BatchModule.CA && this.IsChildTran == false)
                {
                    if (String.IsNullOrEmpty(this._CADrCr) == false)
                    {
                        acctID = (this.CADrCr == CA.CADrCr.CADebit) ? this.DebitCashAccountID : this.CreditCashAccountID;
                    }
                }
                return acctID;

            }
            set
            {

            }
        }
        #endregion 
        #region TaxZoneID
        public abstract class taxZoneID : PX.Data.IBqlField
        {
        }
        protected String _TaxZoneID;

        /// <summary>
        /// Identifier of the <see cref="TaxZone">Tax Zone</see> associated with the document, if applicable.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="TaxZone.TaxZoneID"/> field.
        /// </value>
        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "Tax Zone", Visibility = PXUIVisibility.Visible)]
        [PXSelector(typeof(TaxZone.taxZoneID), DescriptionField = typeof(TaxZone.descr), Filterable = true)]
        public virtual String TaxZoneID
        {
            get
            {
                return this._TaxZoneID;
            }
            set
            {
                this._TaxZoneID = value;
            }
        }
        #endregion
      
        #region TaxID
        public abstract class taxID : PX.Data.IBqlField
        {
        }
		protected String _TaxID;

        /// <summary>
        /// Identifier of the <see cref="Tax"/> associated with the document, if applicable.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="Tax.TaxID"/> field.
        /// </value>
		[PXDBString(Tax.taxID.Length, IsUnicode = true)]
		[PXUIField(DisplayName = "Tax ID", Visible = false)]
		[PXSelector(typeof(Tax.taxID), DescriptionField = typeof(Tax.descr))]
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
        #region TaxRate
        public abstract class taxRate : PX.Data.IBqlField
        {
        }
		protected Decimal? _TaxRate;
        
        /// <summary>
        /// The tax rate for the document, if applicable.
        /// </summary>
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Tax Rate", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public virtual Decimal? TaxRate
        {
            get
            {
                return this._TaxRate;
            }
            set
            {
                this._TaxRate = value;
            }
        }
        #endregion        
        #region CuryTaxableAmt
        public abstract class curyTaxableAmt : PX.Data.IBqlField
        {
        }
		protected Decimal? _CuryTaxableAmt; 

        /// <summary>
        /// The amount that is subjected to the tax for the line.
        /// Given in the <see cref="CuryID">currency</see> of the line.
        /// See also <see cref="TaxableAmt"/>.
        /// </summary>
		[PXDBCurrency(typeof(GLTranDoc.curyInfoID), typeof(GLTranDoc.taxableAmt))]
		//[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Taxable Amount", Visibility = PXUIVisibility.Visible)]
		public virtual decimal? CuryTaxableAmt		
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

        /// <summary>
        /// The amount that is subjected to the tax for the line.
        /// Given in the <see cref="Company.BaseCuryID">base currency</see> of the company.
        /// See also <see cref="CuryTaxableAmt"/>.
        /// </summary>    
		[PXDBDecimal(4)]
		//[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Taxable Amount", Visibility = PXUIVisibility.Visible)]
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
		#region CuryTaxAmt
        public abstract class curyTaxAmt : PX.Data.IBqlField
        {
        }
        
		
		protected Decimal? _CuryTaxAmt;

        /// <summary>
        /// The resulting amount of the tax associated with the line.
        /// Given in the <see cref="CuryID">currency</see> of the line.
        /// See also <see cref="TaxAmt"/>.
        /// </summary>
		[PXDBCurrency(typeof(GLTranDoc.curyInfoID), typeof(GLTranDoc.taxAmt))]
		//[PXFormula(typeof(Mult<GLTranDoc.curyTaxableAmt, Div<GLTranDoc.taxRate, decimal100>>), typeof(SumCalc<TaxAdjustment.curyDocBal>))]
		//[PXFormula(typeof(Mult<GLTranDoc.curyTaxableAmt, Div<GLTranDoc.taxRate, decimal100>>), null)]
		//[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Tax Amount", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public virtual Decimal? CuryTaxAmt				
		{
			get
			{
				return this._CuryTaxAmt;
			}
			set
			{
				this._CuryTaxAmt = value;				
			}
		}
		#endregion
		#region TaxAmt
        public abstract class taxAmt : PX.Data.IBqlField
        {
        }

        protected Decimal? _TaxAmt;

        /// <summary>
        /// The resulting amount of the tax associated with the line.
        /// Given in the <see cref="Company.BaseCuryID">base currency</see> of the company.
        /// See also <see cref="CuryTaxAmt"/>.
        /// </summary>
		[PXDBDecimal(4)]
		//[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Tax Amount", Visibility = PXUIVisibility.Visible)]
		public virtual Decimal? TaxAmt				
		{
			get
			{
				return this._TaxAmt;
			}
			set
			{
				this._TaxAmt = value;				
			}
		}
        #endregion

		#region CuryInclTaxAmt
		public abstract class curyInclTaxAmt : PX.Data.IBqlField
		{
		}

		protected Decimal? _CuryInclTaxAmt;

        /// <summary>
        /// The amount of tax that is included in the <see cref="CuryTranTotal">document total</see>.
        /// Given in the <see cref="CuryID">currency</see> of the line.
        /// See also <see cref="InclTaxAmt"/>.
        /// </summary>
		[PXDBCurrency(typeof(GLTranDoc.curyInfoID), typeof(GLTranDoc.inclTaxAmt))]
		//[PXFormula(typeof(Mult<GLTranDoc.curyTaxableAmt, Div<GLTranDoc.taxRate, decimal100>>), typeof(SumCalc<TaxAdjustment.curyDocBal>))]
		//[PXFormula(typeof(Mult<GLTranDoc.curyTaxableAmt, Div<GLTranDoc.taxRate, decimal100>>), null)]
		//[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Included Tax Amount", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public virtual Decimal? CuryInclTaxAmt
		{
			get
			{
				return this._CuryInclTaxAmt;
			}
			set
			{
				this._CuryInclTaxAmt = value;
			}
		}
		#endregion
		#region InclTaxAmt
		public abstract class inclTaxAmt : PX.Data.IBqlField
		{
		}
        protected Decimal? _InclTaxAmt;

        /// <summary>
        /// The amount of tax that is included in the <see cref="TranTotal">document total</see>.
        /// Given in the <see cref="Company.BaseCuryID">base currency</see> of the company.
        /// See also <see cref="CuryInclTaxAmt"/>.
        /// </summary>
		[PXDBDecimal(4)]
		//[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Included Tax Amount", Visibility = PXUIVisibility.Visible)]
		public virtual Decimal? InclTaxAmt
		{
			get
			{
				return this._InclTaxAmt;
			}
			set
			{
				this._InclTaxAmt = value;
			}
		}
		#endregion

        #region CuryOrigWhTaxAmt
        public abstract class curyOrigWhTaxAmt : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryOrigWhTaxAmt;

        /// <summary>
        /// The tax amount withheld on the document.
        /// Given in the <see cref="CuryID">currency</see> of the line.
        /// See also <see cref="OrigWhTaxAmt"/>.
        /// </summary>
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXDBCurrency(typeof(GLTranDoc.curyInfoID), typeof(GLTranDoc.origWhTaxAmt))]
        [PXUIField(DisplayName = "With. Tax", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
        public virtual Decimal? CuryOrigWhTaxAmt
        {
            get
            {
                return this._CuryOrigWhTaxAmt;
            }
            set
            {
                this._CuryOrigWhTaxAmt = value;
            }
        }
        #endregion
        #region OrigWhTaxAmt
        public abstract class origWhTaxAmt : PX.Data.IBqlField
        {
        }
        protected Decimal? _OrigWhTaxAmt;

        /// <summary>
        /// The tax amount withheld on the document.
        /// Given in the <see cref="Company.BaseCuryID">base currency</see> of the company.
        /// See also <see cref="OrigWhTaxAmt"/>.
        /// </summary>
        [PXDBBaseCury()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? OrigWhTaxAmt
        {
            get
            {
                return this._OrigWhTaxAmt;
            }
            set
            {
                this._OrigWhTaxAmt = value;
            }
        }
        #endregion

		#region CuryDocAmt -Total of the document
		public abstract class curyDocTotal : PX.Data.IBqlField
		{
		}

        /// <summary>
        /// Read-only document total.
        /// Given in the <see cref="CuryID">currency</see> of the line.
        /// </summary>
        /// <value>
        /// For the lines representing document headers gives the total amount of the document, including taxes amount.
        /// For the lines representing details of a document returns <c>null</c>.
        /// Calculated from <see cref="CuryTaxAmt"/>, <see cref="CuryTranAmt"/> and <see cref="CuryInclTaxAmt"/> fields.
        /// </value>
		[PXCurrency(typeof(GLTranDoc.curyInfoID),typeof(GLTranDoc.docTotal))]
		[PXUIField(DisplayName = "Doc Total", Visibility = PXUIVisibility.Visible, Enabled = false)]		
		public virtual Decimal? CuryDocTotal
		{
			[PXDependsOnFields(typeof(curyTaxAmt), typeof(curyTranAmt), typeof(curyInclTaxAmt))]
			get
			{
				if (this.IsChildTran) return null;
				return  this._CuryTranAmt + (this._CuryTaxAmt??Decimal.Zero) - (this._CuryInclTaxAmt??Decimal.Zero);
			}
			set
			{

			}
		}

		public abstract class docTotal : PX.Data.IBqlField
		{
		}

        /// <summary>
        /// Read-only document total.
        /// Given in the <see cref="Company.BaseCuryID">base currency</see> of the company.
        /// </summary>
        /// <value>
        /// For the lines representing document headers gives the total amount of the document, including taxes amount.
        /// For the lines representing details of a document returns <c>null</c>.
        /// Calculated from <see cref="TaxAmt"/>, <see cref="TranAmt"/> and <see cref="InclTaxAmt"/> fields.
        /// </value>
		public virtual Decimal? DocTotal
		{
			[PXDependsOnFields(typeof(taxAmt), typeof(tranAmt), typeof(inclTaxAmt))]
			get
			{
				if (this.IsChildTran) return null;
				return  this._TranAmt + this._TaxAmt - this._InclTaxAmt;
			}
			set
			{

			}
		}
		#endregion

		#region CuryTaxTotal -Total Tax Total of the document
		public abstract class curyTaxTotal : PX.Data.IBqlField
		{
		}

        /// <summary>
        /// Read-only total amount of tax associated with the document or transaction.
        /// Given in the <see cref="CuryID">currency</see> of the line.
        /// </summary>
        /// <value>
        /// Calculated from the <see cref="CuryTaxAmt"/> and <see cref="CuryInclTaxAmt"/> fields.
        /// </value>
		[PXCury(typeof(GLTranDoc.curyID))]
		//[PXFormula(null, typeof(AddCalc<GLDocBatch.curyCreditTotal>))]
		//[PXFormula(null, typeof(AddCalc<GLDocBatch.curyDebitTotal>))]
		[PXUIField(DisplayName = "Tax Total", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public virtual Decimal? CuryTaxTotal
		{
			[PXDependsOnFields(typeof(curyTaxAmt),  typeof(curyInclTaxAmt))]
			get
			{
				return this._CuryTaxAmt - this._CuryInclTaxAmt;
			}
			set
			{

			}
		}

		//[PXFormula(null, typeof(AddCalc<GLDocBatch.creditTotal>))]
        //[PXFormula(null, typeof(AddCalc<GLDocBatch.debitTotal>))]

        /// <summary>
        /// Read-only total amount of tax associated with the document or transaction.
        /// Given in the <see cref="Company.BaseCuryID">base currency</see> of the company.
        /// </summary>
        /// <value>
        /// Calculated from the <see cref="TaxAmt"/> and <see cref="InclTaxAmt"/> fields.
        /// </value>
		public virtual Decimal? TaxTotal
		{
			[PXDependsOnFields(typeof(taxAmt), typeof(inclTaxAmt))]
			get
			{
				return this._TaxAmt - this._InclTaxAmt;
			}
			set
			{

			}
		}
		#endregion

		#region CuryApplAmt
		public abstract class curyApplAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryApplAmt;

        /// <summary>
        /// The amount of the application.
        /// Given in the <see cref="CuryID">currency</see> of the line.
        /// See also <see cref="ApplAmt"/>.
        /// </summary>
        /// <value>
        /// The value of this field should be set if a line describes an <see cref="PX.Objects.AP.APPayment">APPayment</see> or
        /// an <see cref="ARPayment">ARPayment</see>, which is applied to existing documents.
        /// Journal Vouchers (GL.30.40.00) screen allows to enter application information through the AP Payment Applications 
        /// and AR Payment Applications tabs.
        /// </value>
		[PXDBCurrency(typeof(GLTranDoc.curyInfoID), typeof(GLTranDoc.applAmt))]
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
		public abstract class applAmt : PX.Data.IBqlField
		{
		}
        protected Decimal? _ApplAmt;

        /// <summary>
        /// The amount of the application.
        /// Given in the <see cref="Company.BaseCuryID">base currency</see> of the company.
        /// For more infor see <see cref="CuryApplAmt"/>.
        /// </summary>
		[PXDBDecimal(4)]
		public virtual Decimal? ApplAmt
		{
			get
			{
				return this._ApplAmt;
			}
			set
			{
				this._ApplAmt = value;
			}
		}
		#endregion

        #region CuryDiscTaken
        public abstract class curyDiscTaken : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryDiscTaken;

        /// <summary>
        /// The amount of the cash discount taken on the document.
        /// Given in the <see cref="CuryID">currency</see> of the line.
        /// See also <see cref="DiscTaken"/>.
        /// </summary>
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXDBCurrency(typeof(GLTranDoc.curyInfoID), typeof(GLTranDoc.discTaken))]
        public virtual Decimal? CuryDiscTaken
        {
            get
            {
                return this._CuryDiscTaken;
            }
            set
            {
                this._CuryDiscTaken = value;
            }
        }
        #endregion
        #region DiscTaken
        public abstract class discTaken : PX.Data.IBqlField
        {
        }
        protected Decimal? _DiscTaken;

        /// <summary>
        /// The amount of the cash discount taken on the document.
        /// Given in the <see cref="Company.BaseCuryID">base currency</see> of the company.
        /// See also <see cref="CuryDiscTaken"/>.
        /// </summary>
        [PXDBBaseCury()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? DiscTaken
        {
            get
            {
                return this._DiscTaken;
            }
            set
            {
                this._DiscTaken = value;
            }
        }
        #endregion

        #region CuryTaxWheld
        public abstract class curyTaxWheld : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryTaxWheld;

        /// <summary>
        /// The actual amount of tax withheld on the applications of the document.
        /// Given in the <see cref="CuryID">currency</see> of the line.
        /// See also <see cref="TaxWheld"/>.
        /// </summary>
        /// <value>
        /// The value of this field is calculated from the values of the <see cref="PX.Objects.AP.APAdjust.CuryAdjdWhTaxAmt">APAdjust.CuryAdjdWhTaxAmt</see> or
        /// <see cref="PX.Objects.AR.ARAdjust.CuryAdjdWhTaxAmt">ARAdjust.CuryAdjdWhTaxAmt</see> fields of the applications assoicated with the line.
        /// </value>
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXDBCurrency(typeof(GLTranDoc.curyInfoID), typeof(GLTranDoc.taxWheld))]
        public virtual Decimal? CuryTaxWheld
        {
            get
            {
                return this._CuryTaxWheld;
            }
            set
            {
                this._CuryTaxWheld = value;
            }
        }
        #endregion
        #region TaxWheld
        public abstract class taxWheld : PX.Data.IBqlField
        {
        }
        protected Decimal? _TaxWheld;

        /// <summary>
        /// The actual amount of tax withheld on the applications of the document.
        /// Given in the <see cref="Company.BaseCuryID">base currency</see> of the company.
        /// See also <see cref="CuryTaxWheld"/>.
        /// </summary>
        /// <value>
        /// The value of this field is calculated from the values of the <see cref="PX.Objects.AP.APAdjust.CuryAdjdWhTaxAmt">APAdjust.CuryAdjdWhTaxAmt</see> or
        /// <see cref="PX.Objects.AR.ARAdjust.CuryAdjdWhTaxAmt">ARAdjust.CuryAdjdWhTaxAmt</see> fields of the applications assoicated with the line.
        /// </value>
        [PXDBBaseCury()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? TaxWheld
        {
            get
            {
                return this._TaxWheld;
            }
            set
            {
                this._TaxWheld = value;
            }
        }
        #endregion

        #region ApplCount
        public abstract class applCount : PX.Data.IBqlField
        {
        }
        protected Int32? _ApplCount;
        
        /// <summary>
        /// The number of applications specified for the document.
        /// The applications are represented by <see cref="PX.Objects.AP.APAdjust">APAdjust</see> or
        /// <see cref="PX.Objects.AR.ARAdjust">ARAdjust</see> records.
        /// </summary>
        [PXDBInt]
        [PXDefault(0)]
        public virtual Int32? ApplCount
        {
            get
            {
                return this._ApplCount;
            }
            set
            {
                this._ApplCount = value;
            }
        }
        #endregion

		#region CuryUnappliedBal
		public abstract class curyUnappliedBal : PX.Data.IBqlField
		{
		}
		//protected Decimal? _CuryUnappliedBal;

        /// <summary>
        /// Read-only unapplied balance of the document.
        /// Given in the <see cref="CuryID">currency</see> of the line.
        /// See also <see cref="UnappliedBalance"/>.
        /// </summary>
        /// <value>
        /// The value of this field is calculated from the <see cref="CuryTranTotal"/> and <see cref="CuryApplAmt"/> fields.
        /// </value>
		[PXCurrency(typeof(GLTranDoc.curyInfoID), typeof(GLTranDoc.unappliedBal))]
		[PXUIField(DisplayName = "Unapplied Balance", Visibility = PXUIVisibility.Visible, Enabled = false)]		
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Decimal? CuryUnappliedBal
		{
			[PXDependsOnFields(typeof(curyTranTotal),typeof(curyApplAmt))]
			get
			{
				return ((this.CuryTranTotal??Decimal.Zero) - (this.CuryApplAmt??Decimal.Zero));
			}
			set
			{
				//this._CuryApplAmt = (this.CuryTranTotal - value);
			}
		}
		#endregion
		#region UnappliedBal
		public abstract class unappliedBal : PX.Data.IBqlField
		{
		}
        //protected Decimal? _UnappliedBal;

        /// <summary>
        /// Read-only unapplied balance of the document.
        /// Given in the <see cref="Company.BaseCuryID">base currency</see> of the company.
        /// See also <see cref="CuryUnappliedBalance"/>.
        /// </summary>
        /// <value>
        /// The value of this field is calculated from the <see cref="TranTotal"/> and <see cref="ApplAmt"/> fields.
        /// </value>
		[PXDecimal(4)]
		public virtual Decimal? UnappliedBal
		{
			[PXDependsOnFields(typeof(tranTotal), typeof(applAmt))]
			get
			{
				return ((this.TranTotal??Decimal.Zero) - (this.ApplAmt??Decimal.Zero));				
			}
			set
			{
				//this._ApplAmt = (this.TranTotal - value);
			}
		}
		#endregion

        #region CuryDiscBal
        public abstract class curyDiscBal : PX.Data.IBqlField
        {
        }
        
        /// <summary>
        /// The read-only discount balance of the document - the amount of the cash discount that has not been used.
        /// Given in the <see cref="CuryID">currency</see> of the line.
        /// See also <see cref="DiscBal"/>.
        /// </summary>
        /// <value>
        /// The value of this field is calculated from the <see cref="CuryDiscAmt"/> and <see cref="CuryDiscTaken"/> fields.
        /// </value>
        [PXCury(typeof(GLTranDoc.curyID))]
        [PXUIField(DisplayName = "Disc. Balance", Visibility = PXUIVisibility.Visible, Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? CuryDiscBal
        {
            [PXDependsOnFields(typeof(curyDiscAmt), typeof(curyDiscTaken))]
            get
            {
                return ((this.CuryDiscAmt ?? Decimal.Zero) - (this.CuryDiscTaken ?? Decimal.Zero));
            }
            set
            {
                //this._CuryApplAmt = (this.CuryTranTotal - value);
            }
        }
        #endregion
        #region DiscBal
        public abstract class discBal : PX.Data.IBqlField
        {
        }

        /// <summary>
        /// The read-only discount balance of the document - the amount of the cash discount that has not been used.
        /// Given in the <see cref="Company.BaseCuryID">base currency</see> of the company.
        /// See also <see cref="CuryDiscBal"/>.
        /// </summary>
        /// <value>
        /// The value of this field is calculated from the <see cref="DiscAmt"/> and <see cref="DiscTaken"/> fields.
        /// </value>
        [PXDecimal(4)]
        public virtual Decimal? DiscBal
        {
            [PXDependsOnFields(typeof(discAmt), typeof(discTaken))]
            get
            {
                return ((this.DiscAmt?? Decimal.Zero) - (this.DiscTaken ?? Decimal.Zero));
            }
            set
            {
                //this._ApplAmt = (this.TranTotal - value);
            }
        }
        #endregion

        #region CuryWhTaxBal
        public abstract class curyWhTaxBal : PX.Data.IBqlField
        {
        }

        /// <summary>
        /// The read-only balance of the tax withheld on the document.
        /// Given in the <see cref="CuryID">currency</see> of the line.
        /// See also the <see cref="WhTaxBal"/> field.
        /// </summary>
        /// <value>
        /// The value of this field is calculated from the <see cref="CuryOrigWhTaxAmt"/> and <see cref="CuryTaxWheld"/> fields.
        /// </value>
        [PXCury(typeof(GLTranDoc.curyID))]
        [PXUIField(DisplayName = "Wh. Tax Balance", Visibility = PXUIVisibility.Visible, Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? CuryWhTaxBal
        {
            [PXDependsOnFields(typeof(curyOrigWhTaxAmt), typeof(curyTaxWheld))]
            get
            {
                return ((this.CuryOrigWhTaxAmt ?? Decimal.Zero) - (this.CuryTaxWheld ?? Decimal.Zero));
            }
            set
            {
                
            }
        }
        #endregion
        #region WhTaxBal
        public abstract class whTaxBal : PX.Data.IBqlField
        {
        }

        /// <summary>
        /// The read-only balance of the tax withheld on the document.
        /// Given in the <see cref="Company.BaseCuryID">base currency</see> of the company.
        /// See also the <see cref="CuryWhTaxBal"/> field.
        /// </summary>
        /// <value>
        /// The value of this field is calculated from the <see cref="OrigWhTaxAmt"/> and <see cref="TaxWheld"/> fields.
        /// </value>
        [PXDecimal(4)]
        public virtual Decimal? WhTaxBal
        {
            [PXDependsOnFields(typeof(origWhTaxAmt), typeof(taxWheld))]
            get
            {
                return ((this.OrigWhTaxAmt ?? Decimal.Zero) - (this.TaxWheld ?? Decimal.Zero));
            }
            set
            {
                
            }
        }
        #endregion

		#region NeedsDebitCashAccount
		public abstract class needsDebitCashAccount : PX.Data.IBqlField
		{
		}

        /// <summary>
        /// The read-only field, indicating whether the <see cref="DebitCashAccount">Debit Cash Account</see>
        /// is required to define the document described by the batch line.
        /// </summary>
        /// <value>
        /// The value of this field is determined by the <see cref="TranModule"/>, <see cref="TranType"/>,
        /// <see cref="EntryTypeID"/> and <see cref="CADrCr"/> fields.
        /// </value>
		[PXBool()]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Boolean? NeedsDebitCashAccount
		{			
            [PXDependsOnFields(typeof(tranModule), typeof(tranType), typeof(entryTypeID), typeof(CADrCr))]
			get
			{
				Boolean? result = false;
				if (this._TranModule == BatchModule.AP)
				{
					if (this.TranType == AP.APPaymentType.Refund || this.TranType == AP.APPaymentType.VoidCheck
						|| this.TranType == AP.APPaymentType.VoidQuickCheck)
						result = true;
				}

				if (this._TranModule == BatchModule.AR)
				{
					if (this._TranType == AR.ARPaymentType.Payment || this._TranType == AR.ARPaymentType.Prepayment
						|| this._TranType == AR.ARPaymentType.CashSale)
						result = true;
					
				}

                if(this._TranModule == BatchModule.CA)
                {
                    if (!String.IsNullOrEmpty(this.EntryTypeID))
                    {
                        result = (this.CADrCr == CA.CADrCr.CADebit);
                    }
                }
				return result;
			}
			set
			{

			}
		}
		#endregion
		#region NeedsCreditCashAccount
		public abstract class needsCreditCashAccount : PX.Data.IBqlField
		{
		}

        /// <summary>
        /// The read-only field, indicating whether the <see cref="CreditCashAccount">Credit Cash Account</see>
        /// is required to define the document described by the batch line.
        /// </summary>
        /// <value>
        /// The value of this field is determined by the <see cref="TranModule"/>, <see cref="TranType"/>,
        /// <see cref="EntryTypeID"/> and <see cref="CADrCr"/> fields.
        /// </value>
		[PXBool()]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Boolean? NeedsCreditCashAccount
		{
			[PXDependsOnFields(typeof(tranModule), typeof(tranType), typeof(entryTypeID), typeof(CADrCr))]
			get
			{
				Boolean? result = false;
				if (this._TranModule == BatchModule.AP)
				{
					if (this._TranType == AP.APPaymentType.Check || this._TranType == AP.APPaymentType.Prepayment
						|| this._TranType == AP.APPaymentType.QuickCheck)
						result = true;					
				}

				if (this._TranModule == BatchModule.AR)
				{
					if (this.TranType == AR.ARPaymentType.Refund || this.TranType == AR.ARPaymentType.VoidPayment
						|| this.TranType == AR.ARPaymentType.CashReturn)
						result = true;
				}

				if (this._TranModule == BatchModule.CA)
				{
				    if (!String.IsNullOrEmpty(this.EntryTypeID)) 
                    {
                        result = (this.CADrCr == CA.CADrCr.CACredit);
                    }                        
				}
				return result;
			}
			set
			{

			}
		}
		#endregion

		#region NeedTaskValidation
		public abstract class needTaskValidation : PX.Data.IBqlField
		{
		}

        /// <summary>
        /// Indicates whether validation for the presence of the correct <see cref="TaskID"/> must be performed for the line before it is persisted to the database.
        /// </summary>
		[PXBool()]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Boolean? NeedTaskValidation
		{
			[PXDependsOnFields(typeof(tranModule), typeof(split), typeof(parentLineNbr))]
			get
			{
				if (this.Split == true && this.IsChildTran == false)
					return false;
  				return true;
			}
			set
			{

			}
		}
		#endregion
        internal bool IsBalanced 
        {
            [PXDependsOnFields(typeof(creditAccountID),typeof(debitAccountID))]
            get
            {
                return this.CreditAccountID.HasValue && this.DebitAccountID.HasValue;
            }
        }
        internal bool IsChildTran
        {
            [PXDependsOnFields(typeof(parentLineNbr))]
            get { return this.ParentLineNbr.HasValue; }
        }

        #region DebitCashAccountID
        public abstract class debitCashAccountID : PX.Data.IBqlField {}

        /// <summary>
        /// The identifier of the <see cref="CashAccount">Debit Cash Account</see> associated with the document.
        /// Debit Cash Account is required for some types of the documents, that can be included in a <see cref="GLDocBatch">document batch</see>
        /// - see <see cref="NeedsDebitCashAccount"/>.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="CashAccount.CashAccountID"/> field.
        /// </value>
        [PXDBInt()]
        public virtual Int32? DebitCashAccountID
        {
            get;
            set;            
        }
        #endregion
        #region CreditCashAccountID
        public abstract class creditCashAccountID : PX.Data.IBqlField { }

        /// <summary>
        /// The identifier of the <see cref="CashAccount">Credit Cash Account</see> associated with the document.
        /// Credit Cash Account is required for some types of the documents, that can be included in a <see cref="GLDocBatch">document batch</see>
        /// - see <see cref="NeedsCreditCashAccount"/>.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="CashAccount.CashAccountID"/> field.
        /// </value>
        [PXDBInt()]
        public virtual Int32? CreditCashAccountID
        {
            get;
            set;
        }
        #endregion
        #region IInvoice Members

        decimal? IInvoice.CuryDocBal
		{
			get
			{
				return this.CuryUnappliedBal;
			}
			set
			{
				this.CuryUnappliedBal = value;
			}
		}

		decimal? IInvoice.DocBal
		{
			get
			{
				return UnappliedBal;
			}
			set
			{
				this.UnappliedBal = value;
			}
		}

		decimal? IInvoice.CuryDiscBal
		{
			get
			{
				return this.CuryDiscBal;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		decimal? IInvoice.DiscBal
		{
			get
			{
				return this.DiscBal;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		decimal? IInvoice.CuryWhTaxBal
		{
			get
			{
				return this.CuryWhTaxBal;
			}
			set
			{
				throw new NotImplementedException(); 
			}
		}

		decimal? IInvoice.WhTaxBal
		{
			get
			{
				return this.WhTaxBal;				
			}
			set
			{
				throw new NotImplementedException(); 
			}
		}

		long? IInvoice.CuryInfoID
		{
			get
			{
				return this.CuryInfoID;
			}
			set
			{
				;
			}
		}

		DateTime? IInvoice.DiscDate
		{
			get
			{
				return this.DiscDate;
			}
			set
			{
				;
			}
		}

		public string DocType
		{
			get
			{
				return this.TranType;
			}
			set { }
		}
		public string OrigModule
		{
			get
			{
				return this.Module;
			}
			set {}
		}

	    public DateTime? DocDate
		{
			get { return null; }
			set { }
		}

		public string DocDesc
		{
			get { return null; }
			set { }
		}

		public decimal? CuryOrigDocAmt { get { return null; } set {} }
		public decimal? OrigDocAmt { get { return null; } set {} }

		#endregion
	}

	public interface IRegister 
	{
		Int32? BAccountID { get; set; }
		Int32? LocationID { get; set; }
		Int32? BranchID { get; set; }
		Int32? AccountID { get; set; }
		Int32? SubID { get; set; }
		String FinPeriodID { get; set; }
		long? CuryInfoID { get; set; }
	}
}
