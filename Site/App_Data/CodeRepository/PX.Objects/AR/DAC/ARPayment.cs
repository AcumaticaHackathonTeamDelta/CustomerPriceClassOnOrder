using System;
using PX.Data;
using PX.Data.EP;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.CA;
using PX.Objects.CR;
using PX.Objects.PM;
using PX.Common;
using PX.Objects.AR.CCPaymentProcessing;

namespace PX.Objects.AR
{
	public class ARPaymentType : ARDocType
	{
        /// <summary>
        /// Specialized selector for ARPayment RefNbr.<br/>
        /// By default, defines the following set of columns for the selector:<br/>
        /// ARPayment.refNbr,ARPayment.docDate, ARPayment.finPeriodID,<br/>
        /// ARPayment.customerID, ARPayment.customerID_Customer_acctName,<br/>
        /// ARPayment.customerLocationID, ARPayment.curyID, ARPayment.curyOrigDocAmt,<br/>
        /// ARPayment.curyDocBal,ARPayment.status, ARPayment.cashAccountID, ARPayment.pMInstanceID, ARPayment.extRefNbr<br/>
        /// </summary>		
		public class RefNbrAttribute : PXSelectorAttribute
		{
            /// <summary>
            /// Ctor
            /// </summary>
            /// <param name="SearchType">Must be IBqlSearch, returning ARPayment.refNbr</param>
			public RefNbrAttribute(Type SearchType)
				: base(SearchType,
                typeof(ARRegister.refNbr),
                typeof(ARPayment.extRefNbr),
				typeof(ARRegister.docDate),
				typeof(ARRegister.finPeriodID),
				typeof(ARRegister.customerID),
				typeof(ARRegister.customerID_Customer_acctName),
				typeof(ARRegister.customerLocationID),
				typeof(ARRegister.curyID),
				typeof(ARRegister.curyOrigDocAmt),
				typeof(ARRegister.curyDocBal),
				typeof(ARRegister.status),
				typeof(ARPayment.cashAccountID),
				typeof(ARPayment.pMInstanceID_CustomerPaymentMethod_descr))
			{
			}

			public RefNbrAttribute(Type SearchType, params Type[] fieldList)
				: base(SearchType, fieldList)
			{
			}
		}

        /// <summary>
        /// Specialized selector for ARPayment RefNbr.<br/>
        /// By default, defines the following set of columns for the selector:<br/>
        /// ARPayment.refNbr,ARPayment.docDate, ARPayment.finPeriodID,<br/>
        /// ARPayment.customerID, ARPayment.customerID_Customer_acctName,<br/>
        /// ARPayment.customerLocationID, ARPayment.curyID, ARPayment.curyOrigDocAmt,<br/>
        /// ARPayment.curyDocBal,ARPayment.status, ARPayment.cashAccountID, ARPayment.pMInstanceID, ARPayment.extRefNbr<br/>
        /// </summary>	
		public class AdjgRefNbrAttribute : PXSelectorAttribute
		{
            /// <summary>
            /// Ctor
            /// </summary>
            /// <param name="SearchType">Must be IBqlSearch, returning ARPayment.refNbr</param>
			public AdjgRefNbrAttribute(Type SearchType)
				: base(SearchType,
				typeof(ARPayment.refNbr),
				typeof(ARPayment.docDate),
				typeof(ARPayment.finPeriodID),
				typeof(ARPayment.customerID),
				typeof(ARPayment.customerLocationID),
				typeof(ARPayment.curyID),
				typeof(ARPayment.curyOrigDocAmt),
				typeof(ARPayment.curyDocBal),
				typeof(ARPayment.status),
				typeof(ARPayment.cashAccountID),
				typeof(ARPayment.pMInstanceID),
				typeof(ARPayment.extRefNbr),
				typeof(ARPayment.docDesc))
			{
			}
		}

        /// <summary>
        /// Specialized for ARPayments version of the <see cref="AutoNumberAttribute"/><br/>
        /// It defines how the new numbers are generated for the AR Payment. <br/>
        /// References ARInvoice.docType and ARInvoice.docDate fields of the document,<br/>
        /// and also define a link between  numbering ID's defined in AR Setup and ARInvoice types:<br/>
        /// namely ARSetup.paymentNumberingID - for ARPayment, ARPrepayment, AR Refund 
        /// and null for others.
        /// </summary>		
		public class NumberingAttribute : AutoNumberAttribute
		{
			public NumberingAttribute()
				: base(typeof(ARPayment.docType), typeof(ARPayment.docDate),
					_DocTypes,
					_SetupFields) { ; }

			private static string[] _DocTypes
			{
				get
				{
					return new string[] { Payment, CreditMemo, Prepayment, Refund, VoidPayment, SmallBalanceWO };
				}
			}

			private static Type[] _SetupFields
			{
				get
				{
					return new Type[]
					{
						typeof(ARSetup.paymentNumberingID), null, typeof(ARSetup.paymentNumberingID), typeof(ARSetup.paymentNumberingID), null, null 
					};
				}
			}

			public static Type GetNumberingIDField(string docType)
			{
				foreach (var pair in _DocTypes.Zip(_SetupFields))
					if (pair.Item1 == docType)
						return pair.Item2;

				return null;
			}

			public override void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
			{
				base.RowPersisting(sender, e);

				// Prevent saving Payments and Prepayments with the same RefNbr.
				// -
				string currentDocumentType = sender.GetValue<ARPayment.docType>(e.Row) as string;
				string otherDocumentType = null;

				if (currentDocumentType == ARDocType.Payment)
				{
					otherDocumentType = ARDocType.Prepayment;
				}
				else if (currentDocumentType == ARDocType.Prepayment)
				{
					otherDocumentType = ARDocType.Payment;
				}

				var otherDocumentView = new PXSelect<ARRegister,
					Where<ARRegister.refNbr, Equal<Required<ARRegister.refNbr>>,
					And<Where<ARRegister.docType, Equal<Required<ARRegister.docType>>>>>>(sender.Graph);

				bool isManualNumbering = this.UserNumbering == true;

				string newNumber = isManualNumbering ? (e.Row as ARRegister).RefNbr : this.NewNumber;

				if (otherDocumentType != null && 
					otherDocumentView.Select(newNumber, otherDocumentType).Count > 0)
				{
					if (isManualNumbering)
					{
						sender.RaiseExceptionHandling<ARRegister.refNbr>(
							e.Row,
							newNumber,
							new PXSetPropertyException(
								Messages.DocumentAlreadyExistsWithTheSameReferenceNumber,
								PXErrorLevel.Error,
								otherDocumentType == ARDocType.Payment ? Messages.Payment : Messages.Prepayment));
					}
					else
				{
					// Hack. This exception type is thrown to force the system to retry the transaction
					// with the next available Ref Number.
					// -
					throw new PXLockViolationException(typeof(ARRegister), PXDBOperation.Insert, null);
				}
			}
		}
		}

		public new class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Payment, CreditMemo, Prepayment, Refund, VoidPayment, SmallBalanceWO },
				new string[] { Messages.Payment, Messages.CreditMemo, Messages.Prepayment, Messages.Refund, Messages.VoidPayment, Messages.SmallBalanceWO }) { ; }
		}

		public new class SOListAttribute : PXStringListAttribute
		{
			public SOListAttribute()
				: base(
				new string[] { Payment, CreditMemo, Prepayment },
				new string[] { Messages.Payment, Messages.CreditMemo, Messages.Prepayment }) { ; }
		}

		public static bool VoidAppl(string DocType)
		{
			//CashReturn is excluded
			return (DocType == VoidPayment);
		}

		public static bool CanHaveBalance(string DocType)
		{
			return (DocType == CreditMemo || DocType == Payment || DocType == VoidPayment || DocType == Prepayment);
		}

		public static string DrCr(string DocType)
		{
			switch (DocType)
			{
				case Payment:
				case VoidPayment:
				case CreditMemo:
				case SmallBalanceWO:
				case CashSale:
				case Prepayment:
					return GL.DrCr.Debit;
				case Refund:
				case CashReturn:
					return GL.DrCr.Credit;
				default:
					return null;
			}
		}
	}


	[PXCacheName(Messages.ARPayment)]
	[System.SerializableAttribute()]
	[PXTable()]
	[PXSubstitute(GraphType = typeof(ARPaymentEntry))]
	//[PXSubstitute(GraphType = typeof(PX.Objects.SO.SOPaymentEntry))]
	[PXPrimaryGraph(
		new Type[] {
			typeof(ARCashSaleEntry ),
			typeof(ARPaymentEntry)},
		new Type[] {
    typeof(Select<AR.Standalone.ARCashSale, 
			Where2<Where<AR.Standalone.ARCashSale.docType, Equal<ARDocType.cashSale>, Or<ARRegister.docType, Equal<ARDocType.cashReturn>>>, 
			And<AR.Standalone.ARCashSale.refNbr, Equal<Current<ARPayment.refNbr>>>>>),
	typeof(Select<ARPayment, 
			Where<ARPayment.docType, Equal<Current<ARPayment.docType>>, 
			And<ARPayment.refNbr, Equal<Current<ARPayment.refNbr>>>>>)})]
	public partial class ARPayment : ARRegister, IInvoice, ICCPayment
	{
		#region Selected
		public new abstract class selected : IBqlField
		{
		}
		#endregion
		#region DocType
		public new abstract class docType : PX.Data.IBqlField
		{
		}
		[PXDBString(3, IsKey = true, IsFixed = true)]
		[PXDefault()]
		[ARPaymentType.List()]
		[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = true, TabOrder = 0)]
		[PXFieldDescription]
		public override String DocType
		{
			get
			{
				return this._DocType;
			}
			set
			{
				this._DocType = value;
			}
		}
		#endregion
		#region RefNbr
		public new abstract class refNbr : PX.Data.IBqlField
		{
		}
		[PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXDefault()]
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible, TabOrder = 1)]
		[ARPaymentType.RefNbr(typeof(Search2<Standalone.ARRegisterAlias.refNbr,
			InnerJoinSingleTable<ARPayment, On<ARPayment.docType, Equal<Standalone.ARRegisterAlias.docType>,
				And<ARPayment.refNbr, Equal<Standalone.ARRegisterAlias.refNbr>>>,
			InnerJoinSingleTable<Customer, On<Standalone.ARRegisterAlias.customerID, Equal<Customer.bAccountID>>>>,
			Where<Standalone.ARRegisterAlias.docType, Equal<Current<ARPayment.docType>>,
				And<Match<Customer, Current<AccessInfo.userName>>>>, 
			OrderBy<Desc<Standalone.ARRegisterAlias.refNbr>>>), Filterable = true, IsPrimaryViewCompatible = true)]
		[ARPaymentType.Numbering()]
		[PXFieldDescription]
		public override String RefNbr
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
		#region CustomerID
		public new abstract class customerID : PX.Data.IBqlField
		{
        }
        [CustomerActive(Visibility = PXUIVisibility.SelectorVisible, Filterable = true, TabOrder = 2)]
        [PXRestrictor(typeof(Where<Customer.status, Equal<CR.BAccount.status.active>, 
					Or<Customer.status, Equal<CR.BAccount.status.oneTime>, 
					Or<Customer.status, Equal<CR.BAccount.status.hold>, 
					Or<Customer.status, Equal<CR.BAccount.status.creditHold>>>>>), Messages.CustomerIsInStatus, typeof(Customer.status), ReplaceInherited = true)]
		[PXUIField(DisplayName = "Customer", TabOrder = 2)]
		[PXDefault()]
		public override Int32? CustomerID
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
		#region Status
		public new abstract class status : PX.Data.IBqlField
		{
		}
		#endregion
        #region PaymentMethodID
        public abstract class paymentMethodID : PX.Data.IBqlField
        {
        }
        protected String _PaymentMethodID;
        [PXDBString(10, IsUnicode = true)]
        [PXDefault(typeof(Coalesce<Search2<CustomerPaymentMethod.paymentMethodID, InnerJoin<Customer,On<CustomerPaymentMethod.bAccountID,Equal<Customer.bAccountID>>>,
                                        Where<Customer.bAccountID, Equal<Current<ARPayment.customerID>>, 
                                              And<CustomerPaymentMethod.pMInstanceID, Equal<Customer.defPMInstanceID>>>>,
                                   Search<Customer.defPaymentMethodID,
                                         Where<Customer.bAccountID, Equal<Current<ARPayment.customerID>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [PXSelector(typeof(Search5<PaymentMethod.paymentMethodID, LeftJoin<CustomerPaymentMethod,On<CustomerPaymentMethod.paymentMethodID, Equal<PaymentMethod.paymentMethodID>,
                                    And<CustomerPaymentMethod.bAccountID,Equal<Current<ARPayment.customerID>>>>>, 
                                Where<PaymentMethod.isActive, Equal<True>,
                                And<PaymentMethod.useForAR, Equal<True>,
                                And<Where<PaymentMethod.aRIsOnePerCustomer,Equal<True>,
									Or<Where<CustomerPaymentMethod.pMInstanceID, IsNotNull>>>>>>, Aggregate<GroupBy<PaymentMethod.paymentMethodID, GroupBy<PaymentMethod.useForAR, GroupBy<PaymentMethod.useForAP>>>>>), DescriptionField = typeof(PaymentMethod.descr))]
        [PXUIField(DisplayName = "Payment Method", Enabled = false)]
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
		#region BranchID
		public new abstract class branchID : PX.Data.IBqlField
		{
		}
		[Branch(typeof(Coalesce<
			Search<Location.cBranchID, Where<Location.bAccountID, Equal<Current<ARRegister.customerID>>, And<Location.locationID, Equal<Current<ARRegister.customerLocationID>>>>>,
			Search<Branch.branchID, Where<Branch.branchID, Equal<Current<AccessInfo.branchID>>>>>), IsDetail = false)]
		public override Int32? BranchID
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
        #region PMInstanceID
        public abstract class pMInstanceID : PX.Data.IBqlField
        {
        }
        protected Int32? _PMInstanceID;
        [PXDBInt()]
        [PXUIField(DisplayName = "Card/Account No")]
        [PXDefault(typeof(Coalesce< 
                        Search2<Customer.defPMInstanceID, InnerJoin<CustomerPaymentMethod,On<CustomerPaymentMethod.pMInstanceID,Equal<Customer.defPMInstanceID>,
                                And<CustomerPaymentMethod.bAccountID,Equal<Customer.bAccountID>>>>, 
                                Where<Customer.bAccountID, Equal<Current<ARPayment.customerID>>,
                                  And<CustomerPaymentMethod.isActive,Equal<True>,
                                  And<CustomerPaymentMethod.paymentMethodID,Equal<Current2<ARPayment.paymentMethodID>>>>>>,
                        Search<CustomerPaymentMethod.pMInstanceID, 
                                Where<CustomerPaymentMethod.bAccountID, Equal<Current<ARPayment.customerID>>,
                                    And<CustomerPaymentMethod.paymentMethodID,Equal<Current2<ARPayment.paymentMethodID>>,
									And<CustomerPaymentMethod.isActive, Equal<True>>>>, OrderBy<Desc<CustomerPaymentMethod.expirationDate, Desc<CustomerPaymentMethod.pMInstanceID>>>>>)
                        , PersistingCheck = PXPersistingCheck.Nothing)]
        [PXSelector(typeof(Search<CustomerPaymentMethod.pMInstanceID, Where<CustomerPaymentMethod.bAccountID, Equal<Current<ARPayment.customerID>>,
            And<CustomerPaymentMethod.paymentMethodID, Equal<Current2<ARPayment.paymentMethodID>>,
            And<Where<CustomerPaymentMethod.isActive, Equal<True>, Or<CustomerPaymentMethod.pMInstanceID,
                    Equal<Current<ARPayment.pMInstanceID>>>>>>>>), DescriptionField = typeof(CustomerPaymentMethod.descr))]
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
		#region PMInstanceID_CustomerPaymentMethod_descr
		public abstract class pMInstanceID_CustomerPaymentMethod_descr : PX.Data.IBqlField
		{
		}
		#endregion
        #region CashAccountID
		public abstract class cashAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _CashAccountID;
		[PXDefault(typeof(Coalesce<Search2<CustomerPaymentMethod.cashAccountID,
									InnerJoin<PaymentMethodAccount, On<PaymentMethodAccount.cashAccountID,Equal<CustomerPaymentMethod.cashAccountID>,
										And<PaymentMethodAccount.paymentMethodID,Equal<CustomerPaymentMethod.paymentMethodID>,
										And<PaymentMethodAccount.useForAR, Equal<True>>>>>, 
								Where<CustomerPaymentMethod.bAccountID, Equal<Current<ARPayment.customerID>>,
								And<Current<ARPayment.docType>, NotEqual<ARDocType.refund>,
								And<CustomerPaymentMethod.pMInstanceID, Equal<Current2<ARPayment.pMInstanceID>>>>>>, 
                            Search2<CashAccount.cashAccountID, 
                                InnerJoin<PaymentMethodAccount, On<PaymentMethodAccount.cashAccountID, Equal<CashAccount.cashAccountID>, 
									And<PaymentMethodAccount.useForAR,Equal<True>,
                                    And2<Where2<Where<Current<ARPayment.docType>,NotEqual<ARDocType.refund>,
										And<PaymentMethodAccount.aRIsDefault,Equal<True>>>,
										Or<Where<Current<ARPayment.docType>,Equal<ARDocType.refund>,
										And<PaymentMethodAccount.aRIsDefaultForRefund,Equal<True>>>>>,
                                    And<PaymentMethodAccount.paymentMethodID, Equal<Current2<ARPayment.paymentMethodID>>>>>>>,
                                    Where<CashAccount.branchID,Equal<Current<ARPayment.branchID>>,
										And<Match<Current<AccessInfo.userName>>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [CashAccount(typeof(ARPayment.branchID), typeof(Search2<CashAccount.cashAccountID, 
                InnerJoin<PaymentMethodAccount, On<PaymentMethodAccount.cashAccountID, Equal<CashAccount.cashAccountID>,
                    And<PaymentMethodAccount.paymentMethodID, Equal<Current2<ARPayment.paymentMethodID>>,
                    And<PaymentMethodAccount.useForAR,Equal<True>>>>>, 
					Where<Match<Current<AccessInfo.userName>>>>), Visibility = PXUIVisibility.Visible)]
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
		#region ProjectID
		public abstract class projectID : PX.Data.IBqlField
		{
		}
		protected Int32? _ProjectID;
		[ProjectDefault(BatchModule.AR, typeof(Search<Location.cDefProjectID, Where<Location.bAccountID, Equal<Current<ARPayment.customerID>>, And<Location.locationID, Equal<Current<ARPayment.customerLocationID>>>>>), typeof(ARPayment.cashAccountID))]
		[ActiveProjectForModule(BatchModule.AR, false)]
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
		[ActiveProjectTask(typeof(ARPayment.projectID), BatchModule.AR, DisplayName = "Project Task")]
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
		#region UpdateNextNumber
		public abstract class updateNextNumber : PX.Data.IBqlField
		{
		}
		protected Boolean? _UpdateNextNumber;

		[PXBool()]
		[PXUIField(DisplayName = "Update Next Number", Visibility = PXUIVisibility.Invisible)]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Boolean? UpdateNextNumber
		{
			get
			{
				return this._UpdateNextNumber;
			}
			set
			{
				this._UpdateNextNumber = value;
			}
		}
		#endregion
		#region ExtRefNbr
		public abstract class extRefNbr : PX.Data.IBqlField
		{
		}
		protected String _ExtRefNbr;
		[PXDBString(40, IsUnicode = true)]
		[PXUIField(DisplayName = "Payment Ref.", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PaymentRef(typeof(ARPayment.cashAccountID), typeof(ARPayment.paymentMethodID), typeof(ARPayment.updateNextNumber))]
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
		#region CuryID
		public new abstract class curyID : PX.Data.IBqlField
		{
		}
		#endregion
		#region AdjDate
		public abstract class adjDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _AdjDate;
		[PXDBDate()]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Application Date", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual DateTime? AdjDate
		{
			get
			{
				return this._AdjDate;
			}
			set
			{
				this._AdjDate = value;
			}
		}
		#endregion
		#region AdjFinPeriodID
		public abstract class adjFinPeriodID : PX.Data.IBqlField
		{
		}
		protected String _AdjFinPeriodID;
		[AROpenPeriod(typeof(ARPayment.adjDate))]
		[PXUIField(DisplayName = "Application Period", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String AdjFinPeriodID
		{
			get
			{
				return this._AdjFinPeriodID;
			}
			set
			{
				this._AdjFinPeriodID = value;
			}
		}
		#endregion
		#region AdjTranPeriodID
		public abstract class adjTranPeriodID : PX.Data.IBqlField
		{
		}
		protected String _AdjTranPeriodID;
		[TranPeriodID(typeof(ARPayment.adjDate))]
		public virtual String AdjTranPeriodID
		{
			get
			{
				return this._AdjTranPeriodID;
			}
			set
			{
				this._AdjTranPeriodID = value;
			}
		}
		#endregion
		#region DocDate
		public new abstract class docDate : PX.Data.IBqlField
		{
		}
		[PXDBDate()]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Payment Date", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public override DateTime? DocDate
		{
			get
			{
				return this._DocDate;
			}
			set
			{
				this._DocDate = value;
			}
		}
		#endregion
		#region TranPeriodID
		public new abstract class tranPeriodID : PX.Data.IBqlField
		{
		}
		[FinPeriodID(typeof(ARRegister.docDate))]
		public override String TranPeriodID
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
		#region FinPeriodID
		public new abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		[AROpenPeriod(typeof(ARRegister.docDate))]
        [PXDefault()]
		[PXUIField(DisplayName = "Payment Period", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public override String FinPeriodID
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
		#region CuryDocBal
		public new abstract class curyDocBal : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryOrigDocAmt
		public new abstract class curyOrigDocAmt : PX.Data.IBqlField
		{
		}
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(ARPayment.curyInfoID), typeof(ARPayment.origDocAmt))]
		[PXUIField(DisplayName = "Payment Amount", Visibility = PXUIVisibility.SelectorVisible)]
		public override Decimal? CuryOrigDocAmt
		{
			get
			{
				return this._CuryOrigDocAmt;
			}
			set
			{
				this._CuryOrigDocAmt = value;
			}
		}
		#endregion
		#region OrigDocAmt
		public new abstract class origDocAmt : PX.Data.IBqlField
		{
		}
		#endregion
        #region CuryConsolidateChargeTotal
        public abstract class curyConsolidateChargeTotal : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryConsolidateChargeTotal;
        [PXDBCurrency(typeof(ARPayment.curyInfoID), typeof(ARPayment.consolidateChargeTotal))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Deducted Charges", Visibility = PXUIVisibility.Visible, Enabled = false)]
        public virtual Decimal? CuryConsolidateChargeTotal
        {
            get
            {
                return this._CuryConsolidateChargeTotal;
            }
            set
            {
                this._CuryConsolidateChargeTotal = value;
            }
        }
        #endregion
        #region ConsolidateChargeTotal
        public abstract class consolidateChargeTotal : PX.Data.IBqlField
        {
        }
        protected Decimal? _ConsolidateChargeTotal;
        [PXDBDecimal(4)]
        public virtual Decimal? ConsolidateChargeTotal
        {
            get
            {
                return this._ConsolidateChargeTotal;
            }
            set
            {
                this._ConsolidateChargeTotal = value;
            }
        }
        #endregion
		#region AdjCntr
		public new abstract class adjCntr : PX.Data.IBqlField
		{
		}
		#endregion
        #region ChargeCntr
        public abstract class chargeCntr : PX.Data.IBqlField
        {
        }
        protected Int32? _ChargeCntr;
        [PXDBInt()]
        [PXDefault(0)]
        public virtual Int32? ChargeCntr
        {
            get
            {
                return this._ChargeCntr;
            }
            set
            {
                this._ChargeCntr = value;
            }
        }
        #endregion
		#region CuryInfoID
		public new abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryUnappliedBal
		public abstract class curyUnappliedBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryUnappliedBal;
		[PXCurrency(typeof(ARPayment.curyInfoID), typeof(ARPayment.unappliedBal))]
		[PXUIField(DisplayName = "Available Balance", Visibility = PXUIVisibility.Visible, Enabled = false)]
		[PXFormula(typeof(Sub<ARPayment.curyDocBal, Add<ARPayment.curyApplAmt, ARPayment.curySOApplAmt>>))]
		public virtual Decimal? CuryUnappliedBal
		{
			get
			{
				return this._CuryUnappliedBal;
			}
			set
			{
				this._CuryUnappliedBal = value;
			}
		}
		#endregion
		#region UnappliedBal
		public abstract class unappliedBal : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnappliedBal;
		[PXDecimal(4)]
		public virtual Decimal? UnappliedBal
		{
			get
			{
				return this._UnappliedBal;
			}
			set
			{
				this._UnappliedBal = value;
			}
		}
		#endregion
		#region CuryApplAmt
		public abstract class curyApplAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryApplAmt;
		[PXCurrency(typeof(ARPayment.curyInfoID), typeof(ARPayment.applAmt))]
		[PXUIField(DisplayName = "Applied to Documents", Enabled = false)]
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
		#region CurySOApplAmt
		public abstract class curySOApplAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CurySOApplAmt;
		[PXCurrency(typeof(ARPayment.curyInfoID), typeof(ARPayment.sOApplAmt))]
		[PXUIField(DisplayName = "Applied to Orders", Enabled = false)]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Decimal? CurySOApplAmt
		{
			get
			{
				return this._CurySOApplAmt;
			}
			set
			{
				this._CurySOApplAmt = value;
			}
		}
		#endregion
		#region SOApplAmt
		public abstract class sOApplAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _SOApplAmt;
		[PXDecimal(4)]
		public virtual Decimal? SOApplAmt
		{
			get
			{
				return this._SOApplAmt;
			}
			set
			{
				this._SOApplAmt = value;
			}
		}
		#endregion
		#region ApplAmt
		public abstract class applAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _ApplAmt;
		[PXDecimal(4)]
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
		#region CuryWOAmt
		public abstract class curyWOAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryWOAmt;
		[PXCurrency(typeof(ARPayment.curyInfoID), typeof(ARPayment.wOAmt))]
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
		#region Cleared
		public abstract class cleared : PX.Data.IBqlField
		{
		}
		protected Boolean? _Cleared;
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Cleared")]
		public virtual Boolean? Cleared
		{
			get
			{
				return this._Cleared;
			}
			set
			{
				this._Cleared = value;
			}
		}
		#endregion
		#region ClearDate
		public abstract class clearDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ClearDate;
		[PXDBDate]
		[PXUIField(DisplayName = "Clear Date", Visibility = PXUIVisibility.Visible)]
		public virtual DateTime? ClearDate
		{
			get
			{
				return this._ClearDate;
			}
			set
			{
				this._ClearDate = value;
			}
		}
		#endregion
		#region BatchNbr
		public new abstract class batchNbr : PX.Data.IBqlField
		{
		}
		#endregion
		#region Voided
		public new abstract class voided : PX.Data.IBqlField
		{
		}
		[PXDBBool()]
		[PXUIField(DisplayName = "Voided", Visibility = PXUIVisibility.Visible)]
		[PXDefault(false)]
		public override Boolean? Voided
		{
			get
			{
				return this._Voided;
			}
			set
			{
				this._Voided = value;
			}
		}
		#endregion
		#region VoidAppl
		public abstract class voidAppl : PX.Data.IBqlField
		{
		}
		[PXBool()]
		[PXUIField(DisplayName = "Void Application", Visibility = PXUIVisibility.Visible)]
		[PXDefault(false)]
		public virtual Boolean? VoidAppl
		{
			[PXDependsOnFields(typeof(docType))]
			get
			{
				return ARPaymentType.VoidAppl(this._DocType);
			}
			set
			{
				if ((bool)value)
				{
					this._DocType = ARPaymentType.VoidPayment;
				}
			}
		}
		#endregion
		#region CustomerID_Customer_acctName
		public new abstract class customerID_Customer_acctName : PX.Data.IBqlField
		{
		}
		#endregion
		#region CanHaveBalance
		public abstract class canHaveBalance : PX.Data.IBqlField
		{
		}
		[PXBool()]
		[PXUIField(DisplayName = "Can Have Balance", Visibility = PXUIVisibility.Visible)]
		[PXDefault(false)]
		public virtual Boolean? CanHaveBalance
		{
			[PXDependsOnFields(typeof(docType))]
			get
			{
				return ARPaymentType.CanHaveBalance(this._DocType);
			}
			set
			{
			}
		}
		#endregion
		#region Released
		public new abstract class released : PX.Data.IBqlField
		{
		}
		#endregion
		#region Hold
		public new abstract class hold : PX.Data.IBqlField
		{
		}
		#endregion
		#region OpenDoc
		public new abstract class openDoc : PX.Data.IBqlField
		{
		}
		#endregion
		#region DrCr
		public abstract class drCr : PX.Data.IBqlField
		{
		}
		protected string _DrCr;
		[PXString(1, IsFixed = true)]
		public virtual string DrCr
		{
			[PXDependsOnFields(typeof(docType))]
			get
			{
				return ARPaymentType.DrCr(this._DocType);
			}
			set
			{
			}
		}
		#endregion
		#region CATranID
		public abstract class cATranID : PX.Data.IBqlField
		{
		}
		protected Int64? _CATranID;
		[PXDBLong()]
		[ARCashTranID()]
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
		#region DiscDate
		public virtual DateTime? DiscDate
		{
			get
			{
				return new DateTime();
			}
			set
			{
				;
			}
		}
		#endregion
		#region CuryWhTaxBal
		public virtual Decimal? CuryWhTaxBal
		{
			get
			{
				return 0m;
			}
			set
			{
				;
			}
		}
		#endregion
		#region WhTaxBal
		public virtual Decimal? WhTaxBal
		{
			get
			{
				return 0m;
			}
			set
			{
				;
			}
		}
		#endregion
		#region CustomerPaymentMethod_descr
		public abstract class CustomerPaymentMethod_descr : PX.Data.IBqlField
		{
		}
		#endregion
		#region IsCCPayment
		public abstract class isCCPayment : PX.Data.IBqlField
		{
		}

		protected bool? _IsCCPayment;
		[PXBool()]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(Visible = false, Enabled = false)]
		public virtual bool? IsCCPayment
		{
			get
			{
				return this._IsCCPayment;
			}
			set
			{
				this._IsCCPayment = value;
			}
		}
		#endregion
		#region CCPaymentStateDescr
		public abstract class cCPaymentStateDescr : PX.Data.IBqlField
		{
		}
		protected String _CCPaymentStateDescr;
		[PXString(255)]
		[PXUIField(DisplayName = "Processing Status", Enabled = false)]
		public virtual String CCPaymentStateDescr
		{
			get
			{
				return this._CCPaymentStateDescr;
			}
			set
			{
				this._CCPaymentStateDescr = value;
			}
		}
		#endregion
		#region ARDepositAsBatch
		public abstract class depositAsBatch : PX.Data.IBqlField
		{
		}
		protected Boolean? _DepositAsBatch;
		[PXDBBool()]
		[PXDefault(false, typeof(Search<CashAccount.clearingAccount, Where<CashAccount.cashAccountID, Equal<Current<ARPayment.cashAccountID>>>>))]
		[PXUIField(DisplayName = "Batch Deposit", Enabled = false)]
		public virtual Boolean? DepositAsBatch
		{
			get
			{
				return this._DepositAsBatch;
			}
			set
			{
				this._DepositAsBatch = value;
			}
		}
		#endregion
		#region DepositAfter
		public abstract class depositAfter : PX.Data.IBqlField
		{
		}
		protected DateTime? _DepositAfter;
		[PXDBDate()]
		[PXDefault(PersistingCheck =PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Deposit After", Enabled=false,Visible=false)]
		public virtual DateTime? DepositAfter
		{
			get
			{
				return this._DepositAfter;
			}
			set
			{
				this._DepositAfter = value;
			}
		}
		#endregion
		#region DepositDate
		public abstract class depositDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _DepositDate;
		[PXDBDate()]		
		[PXUIField(DisplayName = "Batch Deposit Date", Enabled=false)]
		public virtual DateTime? DepositDate
		{
			get
			{
				return this._DepositDate;
			}
			set
			{
				this._DepositDate = value;
			}
		}
		#endregion
		#region Deposited
		public abstract class deposited : PX.Data.IBqlField
		{
		}
		protected Boolean? _Deposited;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Deposited",Enabled=false)]
		public virtual Boolean? Deposited
		{
			get
			{
				return this._Deposited;
			}
			set
			{
				this._Deposited = value;
			}
		}
		#endregion
		#region DepositType
		public abstract class depositType : PX.Data.IBqlField
		{
		}
		protected String _DepositType;
		[PXDBString(3, IsFixed=true)]
		
		public virtual String DepositType
		{
			get
			{
				return this._DepositType;
			}
			set
			{
				this._DepositType = value;
			}
		}
		#endregion
		#region DepositNbr
		public abstract class depositNbr : PX.Data.IBqlField
		{
		}
		protected String _DepositNbr;
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Batch Deposit Nbr.", Enabled=false)]
		[PXSelector(typeof(Search<CADeposit.refNbr,Where<CADeposit.tranType,Equal<Current<ARPayment.depositType>>>>))]
		public virtual String DepositNbr
		{
			get
			{
				return this._DepositNbr;
			}
			set
			{
				this._DepositNbr = value;
			}
		}
		#endregion
		#region CARefTranAccountID
		protected Int32? _CARefTranAccountID;
		public virtual Int32? CARefTranAccountID
		{
			get
			{
				return this._CARefTranAccountID;
			}
			set
			{
				this._CARefTranAccountID = value;
			}
		}
		#endregion
		#region CARefTranID
		protected Int64? _CARefTranID;
		public virtual Int64? CARefTranID
		{
			get
			{
				return this._CARefTranID;
			}
			set
			{
				this._CARefTranID = value;
			}
		}
		#endregion
        #region CARefSplitLineNbr
        protected Int32? _CARefSplitLineNbr;
        public virtual Int32? CARefSplitLineNbr
        {
            get
            {
                return this._CARefSplitLineNbr;
            }
            set
            {
                this._CARefSplitLineNbr = value;
            }
        }
        #endregion
		#region RefTranExtNbr
		public abstract class refTranExtNbr : PX.Data.IBqlField
		{
		}
		protected String _RefTranExtNbr;
		[PXDBString(50, IsUnicode = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXSelector(typeof(Search<CCProcTran.pCTranNumber,Where<CCProcTran.pMInstanceID,Equal<Current<ARPayment.pMInstanceID>>,
								And<CCProcTran.procStatus,Equal<CCProcStatus.finalized>,
								And<CCProcTran.tranStatus,Equal<CCTranStatusCode.approved>,
								And<Where<CCProcTran.tranType,Equal<CCTranTypeCode.authorizeAndCapture>,
								Or<CCProcTran.tranType,Equal<CCTranTypeCode.priorAuthorizedCapture>>>>>>>,OrderBy<Desc<CCProcTran.tranNbr>>>))]
		[PXUIField(DisplayName = "Orig. PC Ref. Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String RefTranExtNbr
		{
			get
			{
				return this._RefTranExtNbr;
			}
			set
			{
				this._RefTranExtNbr = value;
			}
		}
		#endregion
        #region NoteID
        public new abstract class noteID : PX.Data.IBqlField
        {
        }
		[PXSearchable(SM.SearchCategory.AR, "AR {0}: {1} - {3}", new Type[] { typeof(ARPayment.docType), typeof(ARPayment.refNbr), typeof(ARPayment.customerID), typeof(Customer.acctName) },
			new Type[] { typeof(ARPayment.extRefNbr), typeof(ARPayment.docDesc) },
			NumberFields = new Type[] { typeof(ARPayment.refNbr) },
			Line1Format = "{0:d}{1}{2}", Line1Fields = new Type[] { typeof(ARPayment.docDate), typeof(ARPayment.status), typeof(ARPayment.extRefNbr) },
			Line2Format = "{0}", Line2Fields = new Type[] { typeof(ARPayment.docDesc) },
			MatchWithJoin = typeof(InnerJoin<Customer, On<Customer.bAccountID, Equal<ARPayment.customerID>>>),
			SelectForFastIndexing = typeof(Select2<ARPayment, InnerJoin<Customer, On<ARPayment.customerID, Equal<Customer.bAccountID>>>>)
		)]
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

		#region ICCPayment Members

		
		
		
		string ICCPayment.OrigDocType
		{
			get
			{
				return null;
			}			
		}

		string ICCPayment.OrigRefNbr
		{
			get
			{
				return null;
			}
			
		}

		#endregion
	}
}
namespace PX.Objects.AR.Standalone
{
	[Serializable()]
	[PXHidden(ServiceVisible = false)]
	[PXCacheName(Messages.ARPayment)]
	public partial class ARPayment : PX.Data.IBqlTable
	{

		#region DocType
		public abstract class docType : PX.Data.IBqlField
		{
		}
		protected string _DocType;
		[PXDBString(3, IsKey = true, IsFixed = true)]
		[PXDefault()]
		public virtual String DocType
		{
			get
			{
				return this._DocType;
			}
			set
			{
				this._DocType = value;
			}
		}
		#endregion
		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
		}
		protected string _RefNbr;
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "")]
		[PXDefault()]
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
		#region PMInstanceID
		public abstract class pMInstanceID : PX.Data.IBqlField
		{
		}
		protected Int32? _PMInstanceID;
		[PXDBInt()]
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
		#region PaymentMethodID
		public abstract class paymentMethodID : PX.Data.IBqlField
		{
		}
		protected String _PaymentMethodID;
		[PXDBString(10, IsUnicode = true)]
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
		#region CashAccountID
		public abstract class cashAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _CashAccountID;
		[PXDBInt()]
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
		#region ExtRefNbr
		public abstract class extRefNbr : PX.Data.IBqlField
		{
		}
		protected String _ExtRefNbr;
		[PXDBString(40, IsUnicode = true)]
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
		#region AdjDate
		public abstract class adjDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _AdjDate;
		[PXDBDate()]
		[PXDefault(typeof(AccessInfo.businessDate))]
		public virtual DateTime? AdjDate
		{
			get
			{
				return this._AdjDate;
			}
			set
			{
				this._AdjDate = value;
			}
		}
		#endregion
		#region AdjFinPeriodID
		public abstract class adjFinPeriodID : PX.Data.IBqlField
		{
		}
		protected String _AdjFinPeriodID;
		[PXDBString()]
		public virtual String AdjFinPeriodID
		{
			get
			{
				return this._AdjFinPeriodID;
			}
			set
			{
				this._AdjFinPeriodID = value;
			}
		}
		#endregion
		#region AdjTranPeriodID
		public abstract class adjTranPeriodID : PX.Data.IBqlField
		{
		}
		protected String _AdjTranPeriodID;
		[PXDBString()]
		public virtual String AdjTranPeriodID
		{
			get
			{
				return this._AdjTranPeriodID;
			}
			set
			{
				this._AdjTranPeriodID = value;
			}
		}
		#endregion
		#region Cleared
		public abstract class cleared : PX.Data.IBqlField
		{
		}
		protected Boolean? _Cleared;
		[PXDBBool()]
		public virtual Boolean? Cleared
		{
			get
			{
				return this._Cleared;
			}
			set
			{
				this._Cleared = value;
			}
		}
		#endregion
		#region ClearDate
		public abstract class clearDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ClearDate;
		[PXDBDate()]
		public virtual DateTime? ClearDate
		{
			get
			{
				return this._ClearDate;
			}
			set
			{
				this._ClearDate = value;
			}
		}
		#endregion
		#region CATranID
		public abstract class cATranID : PX.Data.IBqlField
		{
		}
		protected Int64? _CATranID;
		[PXDBLong()]
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
		#region ARDepositAsBatch
		public abstract class depositAsBatch : PX.Data.IBqlField
		{
		}
		protected Boolean? _DepositAsBatch;
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? DepositAsBatch
		{
			get
			{
				return this._DepositAsBatch;
			}
			set
			{
				this._DepositAsBatch = value;
			}
		}
		#endregion
		#region DepositAfter
		public abstract class depositAfter : PX.Data.IBqlField
		{
		}
		protected DateTime? _DepositAfter;
		[PXDBDate()]
		[PXDefault(PersistingCheck =PXPersistingCheck.Nothing)]		
		public virtual DateTime? DepositAfter
		{
			get
			{
				return this._DepositAfter;
			}
			set
			{
				this._DepositAfter = value;
			}
		}
		#endregion
		#region DepositDate
		public abstract class depositDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _DepositDate;
		[PXDBDate()]				
		public virtual DateTime? DepositDate
		{
			get
			{
				return this._DepositDate;
			}
			set
			{
				this._DepositDate = value;
			}
		}
		#endregion
		#region Deposited
		public abstract class deposited : PX.Data.IBqlField
		{
		}
		protected Boolean? _Deposited;
		[PXDBBool()]
		[PXDefault(false)]		
		public virtual Boolean? Deposited
		{
			get
			{
				return this._Deposited;
			}
			set
			{
				this._Deposited = value;
			}
		}
		#endregion
		#region DepositType
		public abstract class depositType : PX.Data.IBqlField
		{
		}
		protected String _DepositType;
		[PXDBString(3, IsFixed=true)]		
		public virtual String DepositType
		{
			get
			{
				return this._DepositType;
			}
			set
			{
				this._DepositType = value;
			}
		}
		#endregion
		#region DepositNbr
		public abstract class depositNbr : PX.Data.IBqlField
		{
		}
		protected String _DepositNbr;
		[PXDBString(15, IsUnicode = true)]
		public virtual String DepositNbr
		{
			get
			{
				return this._DepositNbr;
			}
			set
			{
				this._DepositNbr = value;
			}
		}
		#endregion
		#region ProjectID
		public abstract class projectID : PX.Data.IBqlField
		{
		}
		protected Int32? _ProjectID;
		[ProjectDefault(BatchModule.AR)]
		[ActiveProjectForModule(BatchModule.AR, false)]
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
		[ActiveProjectTask(typeof(ARPayment.projectID), BatchModule.AR, DisplayName = "Project Task")]
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
        #region ChargeCntr
        public abstract class chargeCntr : PX.Data.IBqlField
        {
        }
        protected Int32? _ChargeCntr;
        [PXDBInt()]
        [PXDefault(0)]
        public virtual Int32? ChargeCntr
        {
            get
            {
                return this._ChargeCntr;
            }
            set
            {
                this._ChargeCntr = value;
            }
        }
        #endregion
        #region CuryConsolidateChargeTotal
        public abstract class curyConsolidateChargeTotal : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryConsolidateChargeTotal;
        [PXDBDecimal(4)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? CuryConsolidateChargeTotal
        {
            get
            {
                return this._CuryConsolidateChargeTotal;
            }
            set
            {
                this._CuryConsolidateChargeTotal = value;
            }
        }
        #endregion
        #region ConsolidateChargeTotal
        public abstract class consolidateChargeTotal : PX.Data.IBqlField
        {
        }
        protected Decimal? _ConsolidateChargeTotal;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]        
        public virtual Decimal? ConsolidateChargeTotal
        {
            get
            {
                return this._ConsolidateChargeTotal;
            }
            set
            {
                this._ConsolidateChargeTotal = value;
            }
        }
        #endregion
		#region RefTranExtNbr
		public abstract class refTranExtNbr : PX.Data.IBqlField
		{
		}
		protected String _RefTranExtNbr;
		[PXDBString(50, IsUnicode = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual String RefTranExtNbr
		{
			get
			{
				return this._RefTranExtNbr;
			}
			set
			{
				this._RefTranExtNbr = value;
			}
		}
		#endregion
	}
}
