using System;
using PX.Data;
using PX.Data.EP;
using PX.Objects.AP.Standalone;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.CA;
using PX.Objects.CR;
using PX.Common;

namespace PX.Objects.AP
{
	public class APPaymentType : APDocType
	{
        /// <summary>
        /// Specialized selector for APPayment RefNbr.<br/>
        /// By default, defines the following set of columns for the selector:<br/>
        /// APPayment.refNbr,APPayment.docDate, APPayment.finPeriodID, APPayment.vendorID,<br/>
        /// APPayment.vendorID_Vendor_acctName, APPayment.vendorLocationID, APPayment.curyID,<br/>
        /// APPayment.curyOrigDocAmt, APPayment.curyDocBal, APPayment.status, <br/>
        /// APPayment.cashAccountID, APPayment.paymentMethodID, APPayment.extRefNbr <br/>
        /// </summary>
		public class RefNbrAttribute : PXSelectorAttribute
		{
			public RefNbrAttribute(Type SearchType)
				: base(SearchType,
                typeof(APRegister.refNbr),
                typeof(APPayment.extRefNbr), 
				typeof(APRegister.docDate),
				typeof(APRegister.finPeriodID),
				typeof(APRegister.vendorID),
				typeof(APRegister.vendorID_Vendor_acctName),
				typeof(APRegister.vendorLocationID),
				typeof(APRegister.curyID),
				typeof(APRegister.curyOrigDocAmt),
				typeof(APRegister.curyDocBal),
				typeof(APRegister.status),
				typeof(APPayment.cashAccountID),
				typeof(APPayment.paymentMethodID))
			{
			}
		}

		public class AdjgRefNbrAttribute : PXSelectorAttribute
		{
			public AdjgRefNbrAttribute(Type SearchType)
				: base(SearchType,
				typeof(APPayment.refNbr),
				typeof(APPayment.docDate),
				typeof(APPayment.finPeriodID),
				typeof(APPayment.vendorID),
				typeof(APPayment.vendorLocationID),
				typeof(APPayment.curyID),
				typeof(APPayment.curyOrigDocAmt),
				typeof(APPayment.curyDocBal),
				typeof(APPayment.status),
				typeof(APPayment.cashAccountID),
				typeof(APPayment.paymentMethodID),
				typeof(APPayment.extRefNbr),
				typeof(APPayment.docDesc))
			{
			}
		}

		/// <summary>
		/// Specialized for APPayments version of the <see cref="AutoNumberAttribute"/><br/>
		/// It defines how the new numbers are generated for the AP Payment. <br/>
		/// References APPayment.docType and APPayment.docDate fields of the document,<br/>
		/// and also define a link between  numbering ID's defined in AP Setup and APPayment types:<br/>
		/// namely - APSetup.checkNumberingID for all the types<br/>
		/// </summary>		
		public class NumberingAttribute : AutoNumberAttribute
		{
			public NumberingAttribute()
				: base(typeof(APPayment.docType), typeof(APPayment.docDate),
					new string[] { Check, DebitAdj, Prepayment, Refund, VoidCheck },
					new Type[] { typeof(APSetup.checkNumberingID), null, typeof(APSetup.checkNumberingID), typeof(APSetup.checkNumberingID), null })
			{; }

			private static string[] _DocTypes
			{
				get
				{
					return new string[] { Check, DebitAdj, Prepayment, Refund, VoidCheck };
				}
			}

			private static Type[] _SetupFields
			{
				get
				{
					return new Type[]
					{
					 typeof(APSetup.checkNumberingID), null, typeof(APSetup.checkNumberingID), typeof(APSetup.checkNumberingID), null
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
		}

		public new class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Check, DebitAdj, Prepayment, Refund, VoidCheck },
				new string[] { Messages.Check, Messages.DebitAdj, Messages.Prepayment, Messages.Refund, Messages.VoidCheck }) { ; }
		}

		public static bool VoidAppl(string DocType)
		{
			//VoidQuickCheck is excluded
			return (DocType == VoidCheck);
		}

		public static bool CanHaveBalance(string DocType)
		{
			return (DocType == DebitAdj || DocType == Prepayment || DocType == QuickCheck || DocType == VoidCheck);
		}

		public static string DrCr(string DocType)
		{
			switch (DocType)
			{
				case Check:
				case VoidCheck:
				case DebitAdj:
				case Prepayment:
				case QuickCheck:
					return GL.DrCr.Credit;
				case Refund:
				case VoidQuickCheck:
					return GL.DrCr.Debit;
				default:
					return null;
			}
		}
	}


    /// <summary>
    /// Represents Checks, Debit Adjustments, Prepayments, Refunds and Void Checks in Accounts Payable module.
    /// This DAC extends <see cref="APRegister"/> with the fields specific to the documents of the above types.
    /// </summary>
	[System.SerializableAttribute()]
	[PXTable()]
	[PXSubstitute(GraphType = typeof(APPaymentEntry))]
	[PXPrimaryGraph(new Type[] {
		typeof(APQuickCheckEntry),
		typeof(APPaymentEntry)
	},
		new Type[] {
		typeof(Select<APQuickCheck, 
			Where<APQuickCheck.docType, Equal<Current<APPayment.docType>>, 
			And<APQuickCheck.refNbr, Equal<Current<APPayment.refNbr>>>>>),
		typeof(Select<APPayment, 
			Where<APPayment.docType, Equal<Current<APPayment.docType>>, 
			And<APPayment.refNbr, Equal<Current<APPayment.refNbr>>>>>)
		})]
	[PXEMailSource]
	[PXCacheName(Messages.APPayment)]
	public partial class APPayment : APRegister, IInvoice
	{
		#region Selected
		public new abstract class selected : PX.Data.IBqlField
		{
		}
		#endregion
		#region DocType
		public new abstract class docType : PX.Data.IBqlField
		{
		}

        /// <summary>
        /// [key] Type of the document.
        /// </summary>
        /// <value>
        /// Possible values are: "CHK" - Check, "ADR" - Debit Adjustment,
        /// "PPM" - Prepayment, "REF" - Refund, "VCK" - Void Check
        /// </value>
		[PXDBString(3, IsKey = true, IsFixed = true)]
		[PXDefault()]
		[APPaymentType.List()]
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

        /// <summary>
        /// [key] Reference number of the document.
        /// </summary>
		[PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXDefault()]
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible, TabOrder = 1)]
		[APPaymentType.RefNbr(typeof(Search2<Standalone.APRegisterAlias.refNbr,
			InnerJoinSingleTable<APPayment, On<APPayment.docType, Equal<Standalone.APRegisterAlias.docType>,
				And<APPayment.refNbr, Equal<Standalone.APRegisterAlias.refNbr>>>,
			InnerJoinSingleTable<Vendor, On<Standalone.APRegisterAlias.vendorID, Equal<Vendor.bAccountID>>>>,
			Where<Standalone.APRegisterAlias.docType, Equal<Current<APPayment.docType>>,
				And<Match<Vendor, Current<AccessInfo.userName>>>>, 
			OrderBy<Desc<Standalone.APRegisterAlias.refNbr>>>),Filterable=true)]
		[APPaymentType.Numbering()]
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
		#region VendorID
		public new abstract class vendorID : PX.Data.IBqlField
		{
		}
		#endregion
		#region VendorLocationID
		public new abstract class vendorLocationID : PX.Data.IBqlField
		{
		}
		#endregion
		#region RemitAddressID
		public abstract class remitAddressID : PX.Data.IBqlField
		{
		}
		protected Int32? _RemitAddressID;

        /// <summary>
        /// Remittance address for the document.
        /// </summary>
        /// <value>
        /// Defaults to the <see cref="PX.Objects.CR.Location.VRemitAddressID">remittance address</see> of the vendor.
        /// </value>
		[PXDBInt()]
		[APAddress(typeof(Select2<Location,
			InnerJoin<BAccountR, On<BAccountR.bAccountID, Equal<Location.bAccountID>>,
			InnerJoin<Address, On<Address.addressID, Equal<Location.remitAddressID>, And<Where<Address.bAccountID, Equal<Location.bAccountID>, Or<Address.bAccountID, Equal<BAccountR.parentBAccountID>>>>>,
			LeftJoin<APAddress, On<APAddress.vendorID, Equal<Address.bAccountID>, And<APAddress.vendorAddressID, Equal<Address.addressID>, And<APAddress.revisionID, Equal<Address.revisionID>, And<APAddress.isDefaultAddress, Equal<True>>>>>>>>,
			Where<Location.bAccountID, Equal<Current<APPayment.vendorID>>, And<Location.locationID, Equal<Current<APPayment.vendorLocationID>>>>>))]
		public virtual Int32? RemitAddressID
		{
			get
			{
				return this._RemitAddressID;
			}
			set
			{
				this._RemitAddressID = value;
			}
		}
		#endregion
		#region RemitContactID
		public abstract class remitContactID : PX.Data.IBqlField
		{
		}
		protected Int32? _RemitContactID;

        /// <summary>
        /// Remittance contact for the document.
        /// </summary>
        /// <value>
        /// Defaults to the <see cref="PX.Objects.CR.Location.VRemitContactID">remittance contact</see> of the vendor.
        /// </value>
		[PXDBInt()]
		[APContact(typeof(Select2<Location,
			InnerJoin<BAccountR, On<BAccountR.bAccountID, Equal<Location.bAccountID>>,
			InnerJoin<Contact, On<Contact.contactID, Equal<Location.remitContactID>, And<Where<Contact.bAccountID, Equal<Location.bAccountID>, Or<Contact.bAccountID, Equal<BAccountR.parentBAccountID>>>>>,
			LeftJoin<APContact, On<APContact.vendorID, Equal<Contact.bAccountID>, And<APContact.vendorContactID, Equal<Contact.contactID>, And<APContact.revisionID, Equal<Contact.revisionID>, And<APContact.isDefaultContact, Equal<True>>>>>>>>,
			Where<Location.bAccountID, Equal<Current<APPayment.vendorID>>, And<Location.locationID, Equal<Current<APPayment.vendorLocationID>>>>>))]
		public virtual Int32? RemitContactID
		{
			get
			{
				return this._RemitContactID;
			}
			set
			{
				this._RemitContactID = value;
			}
		}
		#endregion
		#region APAccountID
		public new abstract class aPAccountID : PX.Data.IBqlField
		{
		}
		#endregion
		#region APSubID
		public new abstract class aPSubID : PX.Data.IBqlField
		{
		}
		#endregion
		#region Status
		public new abstract class status : PX.Data.IBqlField
		{
		}
		#endregion
		#region BranchID
		public new abstract class branchID : PX.Data.IBqlField
		{
		}

        /// <summary>
        /// Identifier of the <see cref="PX.Objects.GL.Branch">Branch</see>, to which the document belongs.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Objects.GL.Branch.BranchID">Branch.BranchID</see> field.
        /// </value>
		[Branch(typeof(Coalesce<
			Search<Location.vBranchID, Where<Location.bAccountID, Equal<Current<APRegister.vendorID>>, And<Location.locationID, Equal<Current<APRegister.vendorLocationID>>>>>,
			Search<Branch.branchID, Where<Branch.branchID, Equal<Current<AccessInfo.branchID>>>>>),IsDetail = false)]
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
		#region PaymentMethodID
		public abstract class paymentMethodID : PX.Data.IBqlField
		{
		}
		protected String _PaymentMethodID;

        /// <summary>
        /// The <see cref="PX.Objects.CA.PaymentMethod">payment method</see> used for the document.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Objects.CA.PaymentMethod.PaymentMethodID">PaymentMethod.PaymentMethodID</see> field.
        /// Defaults to the payment method associated with the vendor location.
        /// </value>
		[PXDBString(10, IsUnicode = true)]
		[PXDefault(typeof(Search<Location.paymentMethodID, 
							Where<Location.bAccountID, Equal<Current<APPayment.vendorID>>, And<Location.locationID, Equal<Current<APPayment.vendorLocationID>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Payment Method", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<PaymentMethod.paymentMethodID, 
						  Where<PaymentMethod.useForAP,Equal<True>,
							And<PaymentMethod.isActive, Equal<True>>>>))]
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

        /// <summary>
        /// The <see cref="PX.Objects.CA.CashAccount">cash account</see> associated with the <see cref="PaymentMethodID">payment method</see>.
        /// The field is irrelevant for debit adjustments.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Objects.CA.CashAccount.CashAccountID">CashAccount.CashAccountID</see> field.
        /// Defaults to the cash account associated with the payment method and location. 
        /// </value>
		[PXDefault(typeof(Coalesce<Search2<Location.cashAccountID,
										InnerJoin<PaymentMethodAccount, On<PaymentMethodAccount.cashAccountID, Equal<Location.cashAccountID>,
											And<PaymentMethodAccount.paymentMethodID, Equal<Location.vPaymentMethodID>,
											And<PaymentMethodAccount.useForAP, Equal<True>>>>>,
										Where<Location.bAccountID, Equal<Current<APPayment.vendorID>>, 
											And<Location.locationID, Equal<Current<APPayment.vendorLocationID>>,
											And<Location.vPaymentMethodID,Equal<Current<APPayment.paymentMethodID>>>>>>,
								   Search2<PaymentMethodAccount.cashAccountID,InnerJoin<CashAccount,On<CashAccount.cashAccountID,Equal<PaymentMethodAccount.cashAccountID>>>, 
										Where<PaymentMethodAccount.paymentMethodID, Equal<Current<APPayment.paymentMethodID>>, 
											And<CashAccount.branchID, Equal<Current<APPayment.branchID>>,
											And<PaymentMethodAccount.useForAP,Equal<True>,												
											And<PaymentMethodAccount.aPIsDefault,Equal<True>>>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]

        [CashAccount(typeof(APPayment.branchID), typeof(Search2<CashAccount.cashAccountID,
						InnerJoin<PaymentMethodAccount, 
							On<PaymentMethodAccount.cashAccountID,Equal<CashAccount.cashAccountID>>>,
						Where2<Match<Current<AccessInfo.userName>>, 							
							And<PaymentMethodAccount.paymentMethodID,Equal<Current<APPayment.paymentMethodID>>,
							And<PaymentMethodAccount.useForAP,Equal<True>,
							And<Where<CashAccount.clearingAccount, Equal<False>,
								Or<Current<APPayment.docType>,Equal<APDocType.refund>>>>>>>>), 
											Visibility = PXUIVisibility.Visible, SuppressCurrencyValidation = true)]
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
		#region UpdateNextNumber
		public abstract class updateNextNumber : PX.Data.IBqlField
		{
		}
		protected Boolean? _UpdateNextNumber;

        /// <summary>
        /// When set to <c>true</c> indicates that the <see cref="PX.Objects.CA.PaymentMethodAccount.APLastRefNbr">last reference number</see> f
        /// ield in the associated <see cref="PX.Objects.CA.PaymentMethodAccount">PaymentMethodAccount</see> record should be updated when the 
        /// payment record is persisted.
        /// </summary>
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

        /// <summary>
        /// A payment reference number, which can be a system-generated number or an external reference number 
        /// (such as a wire transfer number or a bank check number) entered manually.
        /// Irrelevant for Debit Adjustments.
        /// </summary>
		[PXDBString(40, IsUnicode=true)]
		[PXUIField(DisplayName = "Payment Ref.", Visibility = PXUIVisibility.SelectorVisible)]
		[PaymentRef(typeof(APPayment.cashAccountID), typeof(APPayment.paymentMethodID),typeof(APPayment.stubCntr), typeof(APPayment.updateNextNumber))]
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

        /// <summary>
        /// The date when the payment is applied.
        /// </summary>
        /// <value>
        /// Defaults to the current <see cref="AccessInfo.BusinessDate">business date</see>.
        /// </value>
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

        /// <summary>
        /// The <see cref="PX.Objects.GL.FinPeriod">financial period</see> of payment application.
        /// </summary>
        /// <value>
        /// The value of this field is determined by the <see cref="AdjDate"/> field, but can be overriden manually.
        /// </value>
		[APOpenPeriod(typeof(APPayment.adjDate))]
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

        /// <summary>
        /// The <see cref="PX.Objects.GL.FinPeriod">financial period</see> of payment application.
        /// </summary>
        /// <value>
        /// Unlike the <see cref="AdjFinPeriodID"/>, the value of this field is determined solely by the <see cref="AdjDate"/> field and can't be overriden.
        /// </value>
		[TranPeriodID(typeof(APPayment.adjDate))]
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

        /// <summary>
        /// The date of the payment document.
        /// </summary>
		[PXDBDate()]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Payment Date", Visibility = PXUIVisibility.SelectorVisible, Enabled=false)]
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

        /// <summary>
        /// The <see cref="PX.Objects.GL.FinPeriod">financial period</see> of the document corresponding to the <see cref="DocDate">payment date</see>.
        /// </summary>
        /// <value>
        /// Unlike the <see cref="FinPeriodID"/>, the value of this field is determined solely by the <see cref="AdjDate"/> field and can't be overriden.
        /// </value>
		[FinPeriodID(typeof(APRegister.docDate))]
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

        /// <summary>
        /// The <see cref="PX.Objects.GL.FinPeriod">financial period</see> of the document.
        /// </summary>
        /// <value>
        /// The value of this field is determined by the <see cref="DocDate"/> field, but can be overriden manually.
        /// </value>
		[APOpenPeriod(typeof(APRegister.docDate))]
        [PXDefault()]
        [PXUIField(DisplayName = "Fin. Period", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
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

        /// <summary>
        /// The total payment amount that should be applied to the documents.
        /// (Presented in the currency of the document, see <see cref="CuryID"/>)
        /// </summary>
		[PXDefault(TypeCode.Decimal, "0.0")]
        [PXDBCurrency(typeof(APPayment.curyInfoID), typeof(APPayment.origDocAmt))]
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
		#region LineCntr
		public new abstract class lineCntr : PX.Data.IBqlField
		{
		}
		#endregion
		#region StubCntr
		public abstract class stubCntr : PX.Data.IBqlField
		{
		}
		protected Int32? _StubCntr;

        /// <summary>
        /// The counter of the related pay stubs.
        /// Note that this field is used internally for numbering purposes and its value may not reflect the actual count of the pay stubs.
        /// </summary>
		[PXDBInt()]
		[PXDefault(0)]
		public virtual Int32? StubCntr
		{
			get
			{
				return this._StubCntr;
			}
			set
			{
				this._StubCntr = value;
			}
		}
		#endregion
		#region BillCntr
		public abstract class billCntr : PX.Data.IBqlField
		{
		}
		protected Int32? _BillCntr;

        /// <summary>
        /// The counter of the related bills.
        /// Note that this field is used internally for numbering purposes and its value may not reflect the actual count of the bills.
        /// </summary>
		[PXDBInt()]
		[PXDefault(0)]
		public virtual Int32? BillCntr
		{
			get
			{
				return this._BillCntr;
			}
			set
			{
				this._BillCntr = value;
			}
		}
		#endregion
        #region ChargeCntr
        public abstract class chargeCntr : PX.Data.IBqlField
        {
        }
        protected Int32? _ChargeCntr;

        /// <summary>
        /// The counter of the related charge entries.
        /// Note that this field is used internally for numbering purposes and its value may not reflect the actual count of the charges.
        /// </summary>
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

        /// <summary>
        /// The balance that has not been applied. This will be a nonzero value if the payment amount is not equal to a document’s total amount.
        /// Checks shall always have a zero unapplied balance.
        /// (Presented in the currency of the document, see <see cref="CuryID"/>)
        /// </summary>
		[PXCurrency(typeof(APPayment.curyInfoID), typeof(APPayment.unappliedBal))]
		[PXUIField(DisplayName = "Unapplied Balance", Visibility = PXUIVisibility.Visible, Enabled = false)]
		[PXFormula(typeof(Sub<APPayment.curyDocBal, APPayment.curyApplAmt>))]
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

        /// <summary>
        /// The balance that has not been applied. This will be a nonzero value if the payment amount is not equal to a document’s total amount.
        /// Checks shall always have a zero unapplied balance.
        /// (Presented in the base currency of the company, see <see cref="Company.BaseCuryID"/>)
        /// </summary>
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

        /// <summary>
        /// The amount to be applied on the application date.
        /// (Presented in the currency of the document, see <see cref="CuryID"/>)
        /// </summary>
		[PXCurrency(typeof(APPayment.curyInfoID), typeof(APPayment.applAmt))]
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
        /// The amount to be applied on the application date.
        /// (Presented in the base currency of the company, see <see cref="Company.BaseCuryID"/>)
        /// </summary>
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
		#region Cleared
        public abstract class cleared : PX.Data.IBqlField
        {
        }
        protected Boolean? _Cleared;

        /// <summary>
        /// When set to <c>true</c> indicates that the check was cleared in the process of reconciliation.
        /// </summary>
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

        /// <summary>
        /// The date when the check was cleared.
        /// </summary>
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
		#region Released
		public new abstract class released : PX.Data.IBqlField
		{
		}
		#endregion
		#region OpenDoc
		public new abstract class openDoc : PX.Data.IBqlField
		{
		}
		#endregion
		#region Hold
		public new abstract class hold : PX.Data.IBqlField
		{
		}
		#endregion
		#region Voided
		public new abstract class voided : PX.Data.IBqlField
		{
		}

        /// <summary>
        /// When set to <c>true</c> indicates that the document was voided. In this case <see cref="VoidBatchNbr"/> field will hold the number of the voiding <see cref="Batch"/>.
        /// </summary>
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
		#region Printed
		public new abstract class printed : PX.Data.IBqlField
		{
		}

        /// <summary>
        /// When set to <c>true</c> indicates that the document was printed.
        /// </summary>
		[PXDBBool()]
		public override Boolean? Printed
		{
			get
			{
				return this._Printed;
			}
			set
			{
				this._Printed = value;
			}
		}
		#endregion
		#region VendorID_Vendor_acctName
		public new abstract class vendorID_Vendor_acctName : PX.Data.IBqlField
		{
		}
		#endregion
		#region PrintCheck
		public abstract class printCheck : PX.Data.IBqlField
		{
		}

        /// <summary>
        /// When set to <c>true</c> indicates that a check must be printed for the payment represented by this record.
        /// </summary>
		[PXBool()]
		[PXDefault(typeof(Search<PaymentMethod.printOrExport, Where<PaymentMethod.paymentMethodID,Equal<Current<APPayment.paymentMethodID>>>>))]
		[PXUIField(DisplayName="Print Check")]
		public virtual Boolean? PrintCheck
		{
			[PXDependsOnFields(typeof(printed))]
			get
			{
				return (this._Printed == null ? (Boolean?)null : (bool)this._Printed == false);
			}
			set
			{
				this._Printed = (value == null ? (Boolean?)null : (bool)value == false);
			}
		}
		#endregion
		#region VoidAppl
		public abstract class voidAppl : PX.Data.IBqlField
		{
		}

        /// <summary>
        /// When <c>true</c> indicates that the document is Void Check.
        /// </summary>
        /// <value>
        /// Setting this field to <c>true</c> will change the <see cref="DocType">type of the document</see> to Void Check (<c>"VCK"</c>).
        /// </value>
		[PXBool()]
		[PXUIField(DisplayName = "Void Application", Visibility = PXUIVisibility.Visible)]
		[PXDefault(false)]
		public virtual Boolean? VoidAppl
		{
			[PXDependsOnFields(typeof(docType))]
			get
			{
				return APPaymentType.VoidAppl(this._DocType);
			}
			set
			{
				if ((bool)value)
				{
					this._DocType = APPaymentType.VoidCheck;
				}
			}
		}
		#endregion
		#region CanHaveBalance
		public abstract class canHaveBalance : PX.Data.IBqlField
		{
		}

        /// <summary>
        /// Read-only field indicating whether the document can have balance.
        /// </summary>
        /// <value>
        /// <c>true</c> for Debit Adjustments, Prepayments, Quick Checks and Void Checks. <c>false</c> for other documents.
        /// </value>
		[PXBool()]
		[PXUIField(DisplayName = "Can Have Balance", Visibility = PXUIVisibility.Visible)]
		[PXDefault(false)]
		public virtual Boolean? CanHaveBalance
		{
			[PXDependsOnFields(typeof(docType))]
			get
			{
				return APPaymentType.CanHaveBalance(this._DocType);
			}
			set	
			{
			}
		}
		#endregion
		#region DrCr
		public abstract class drCr : PX.Data.IBqlField
		{
		}
		protected string _DrCr;

        /// <summary>
        /// Read-only field indicating whether the document is of debit or credit type.
        /// The value of this field is based solely on the <see cref="DocType"/> field.
        /// </summary>
        /// <value>
        /// Possible values are <c>"D"</c> (for Refund and Void Quick Check)
        /// and <c>"C"</c> (for Check, Void Check, Debit Adjustment, Prepayment and Quick Check).
        /// </value>
		[PXString(1, IsFixed = true)]
		public string DrCr
		{
			[PXDependsOnFields(typeof(docType))]
			get
			{
				return APPaymentType.DrCr(this._DocType);
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

        /// <summary>
        /// Identifier of the related <see cref="PX.Objects.CA.CATran">transaction in Cash Management module</see>.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Objects.CA.CATran.CATranID">CATran.CATranID</see> field.
        /// </value>
		[PXDBLong()]
		[APCashTranID()]
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
		#region AmountToWords
		public abstract class amountToWords : IBqlField { }
		protected string _AmountToWords;

        /// <summary>
        /// Returns the word representation of the amount of the document. (English only)
        /// </summary>
		[ToWords(typeof(APPayment.curyOrigDocAmt))]
		public virtual string AmountToWords
		{
			get
			{
				return this._AmountToWords;
			}
			set
			{
				this._AmountToWords = value;
			}
		}
		#endregion
		#region PTInstanceID
		public abstract class pTInstanceID : PX.Data.IBqlField
		{
		}
		protected Int32? _PTInstanceID;

        /// <summary>
        /// Identifier of the related <see cref="PX.Objects.CA.PaymentTypeInstance">PaymentTypeInstance</see> record.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Objects.CA.PaymentTypeInstance.PTInstanceID">PaymentTypeInstance.PTInstanceID</see> field.
        /// </value>
		[PXDBInt()]
		[PXUIField(DisplayName = "Card ID",Enabled = false,Visible = false)]
		[PXSelector(typeof(Search<PaymentTypeInstance.pTInstanceID, Where<PaymentTypeInstance.cashAccountID, Equal<Current<APPayment.cashAccountID>>,
								And<PaymentTypeInstance.paymentMethodID,Equal<Current<APPayment.paymentMethodID>>,
			And<PaymentTypeInstance.isActive,Equal<BQLConstants.BitOn>>>>>), DescriptionField = typeof(PaymentTypeInstance.descr))]
		public virtual Int32? PTInstanceID
		{
			get
			{
				return this._PTInstanceID;
			}
			set
			{
				this._PTInstanceID = value;
			}
		}
		#endregion
		#region CARefTranAccountID
		protected Int32? _CARefTranAccountID;

        /// <summary>
        /// Together with the <see cref="CARefTranID"/> and <see cref="CARefSplitLineNbr"/> the field is used to link the payment
        /// to the appropriate Cash Management entities (See <see cref="CATran"/>, <see cref="CASplit"/>, <see cref="CashAccount"/>)
        /// during the payments reclassification process.
        /// </summary>
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

        /// <summary>
        /// Together with the <see cref="CARefTranAccountID"/> and <see cref="CARefSplitLineNbr"/> the field is used to link the payment
        /// to the appropriate Cash Management entities (See <see cref="CATran"/>, <see cref="CASplit"/>, <see cref="CashAccount"/>)
        /// during the payments reclassification process.
        /// </summary>
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

        /// <summary>
        /// Together with the <see cref="CARefTranID"/> and <see cref="CARefTranAccountID"/> the field is used to link the payment
        /// to the appropriate Cash Management entities (See <see cref="CATran"/>, <see cref="CASplit"/>, <see cref="CashAccount"/>)
        /// during the payments reclassification process.
        /// </summary>
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
		#region DiscDate

        /// <summary>
        /// Doesn't bear any meaning in the context of <see cref="APPayment"/> records.
        /// </summary>
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
		#region DepositAsBatch
		public abstract class depositAsBatch : PX.Data.IBqlField
		{
		}
		protected Boolean? _DepositAsBatch;

        /// <summary>
        /// When set to <c>true</c> indicates that the payment can be included in a deposit.
        /// </summary>
		[PXDBBool()]
		[PXUIField(DisplayName = "Batch Deposit", Enabled = false)]
		[PXDefault(false, typeof(Search<CashAccount.clearingAccount, Where<CashAccount.cashAccountID, Equal<Current<APPayment.cashAccountID>>>>))]
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

        /// <summary>
        /// Informational date specified on the document, which is the source of the deposit.
        /// </summary>
        /// <value>
        /// Normally defaults to <see cref="AdjDate"/> for payments with <see cref="DepositAsBatch"/> set to <c>true</c>.
        /// </value>
		[PXDBDate()]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Deposit After", Enabled = false, Visible = false)]
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
		#region Deposited
		public abstract class deposited : PX.Data.IBqlField
		{
		}
		protected Boolean? _Deposited;

        /// <summary>
        /// When equal to <c>true</c> indicates that the payment was deposited.
        /// </summary>
		[PXDBBool()]
		[PXUIField(DisplayName = "Deposited", Enabled = false)]
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
		#region DepositDate
		public abstract class depositDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _DepositDate;

        /// <summary>
        /// The date of deposit.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Objects.CA.CADeposit.TranDate">CADeposit.TranDate</see> field
        /// </value>
		[PXDBDate()]
		[PXUIField(DisplayName = "Batch Deposit Date", Enabled = false)]
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
		#region DepositType
		public abstract class depositType : PX.Data.IBqlField
		{
		}
		protected String _DepositType;

        /// <summary>
        /// The type of the <see cref="PX.Objects.CA.CADeposit">deposit document</see>.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Objects.CA.CADeposit.TranType">CADeposit.TranType</see> field
        /// </value>
		[PXUIField(Enabled = false)]
		[PXDBString(3, IsFixed = true)]
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

        /// <summary>
        /// The reference number of the <see cref="PX.Objects.CA.CADeposit">deposit document</see>.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Objects.CA.CADeposit.RefNbr">CADeposit.RefNbr</see> field
        /// </value>
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Batch Deposit Nbr.", Enabled = false)]
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
        #region NoteID
        public new abstract class noteID : PX.Data.IBqlField
        {
        }

        /// <summary>
        /// Identifier of the <see cref="PX.Data.Note">Note</see> object, associated with the document.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Data.Note.NoteID">Note.NoteID</see> field. 
        /// </value>
		[PXSearchable(SM.SearchCategory.AP, "AP {0}: {1} - {3}", new Type[] { typeof(APPayment.docType), typeof(APPayment.refNbr), typeof(APPayment.vendorID), typeof(Vendor.acctName) },
            new Type[] { typeof(APPayment.extRefNbr), typeof(APPayment.docDesc)},
		   NumberFields = new Type[] { typeof(APPayment.refNbr) },
		   Line1Format = "{0:d}{1}{2}", Line1Fields = new Type[] { typeof(APPayment.docDate), typeof(APPayment.status), typeof(APPayment.extRefNbr) },
		   Line2Format = "{0}", Line2Fields = new Type[] { typeof(APPayment.docDesc) },
		   WhereConstraint = typeof(Where<APRegister.docType, NotEqual<APDocType.quickCheck>, And<APRegister.docType, NotEqual<APDocType.voidQuickCheck>>>),
		   MatchWithJoin = typeof(InnerJoin<Vendor, On<Vendor.bAccountID, Equal<APPayment.vendorID>>>),
		   SelectForFastIndexing = typeof(Select2<APPayment, InnerJoin<Vendor, On<APPayment.vendorID, Equal<Vendor.bAccountID>>>, Where<APRegister.docType, NotEqual<APDocType.quickCheck>, And<APRegister.docType, NotEqual<APDocType.voidQuickCheck>>>>)
		)]
        [PXNote]
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
		#region Approval Process Overrides
		public new abstract class approved : IBqlField { }

		[PXDBBool]
		[PXDefault(true, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Approved", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public override bool? Approved
		{
			get;
			set;
		}

		public new abstract class dontApprove : IBqlField { }
	    public override bool? DontApprove
	    {
		    get
		    {
				return true;
		    }
		    set
		    {
		    }
	    }
	    #endregion
	}
}

namespace PX.Objects.AP.Standalone
{
	[Serializable()]
	[PXHidden(ServiceVisible = false)]
	public partial class APPayment : PX.Data.IBqlTable
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
		#region RemitAddressID
		public abstract class remitAddressID : PX.Data.IBqlField
		{
		}
		protected Int32? _RemitAddressID;
		[PXDBInt()]
		public virtual Int32? RemitAddressID
		{
			get
			{
				return this._RemitAddressID;
			}
			set
			{
				this._RemitAddressID = value;
			}
		}
		#endregion
		#region RemitContactID
		public abstract class remitContactID : PX.Data.IBqlField
		{
		}
		protected Int32? _RemitContactID;
		[PXDBInt()]
		public virtual Int32? RemitContactID
		{
			get
			{
				return this._RemitContactID;
			}
			set
			{
				this._RemitContactID = value;
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
		[PXDefault()]
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
		[PXDBString(6, IsFixed=true)]
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
		[PXDBString(6, IsFixed = true)]
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
		#region StubCntr
		public abstract class stubCntr : PX.Data.IBqlField
		{
		}
		protected Int32? _StubCntr;
		[PXDBInt()]
		[PXDefault(0)]
		public virtual Int32? StubCntr
		{
			get
			{
				return this._StubCntr;
			}
			set
			{
				this._StubCntr = value;
			}
		}
		#endregion
		#region BillCntr
		public abstract class billCntr : PX.Data.IBqlField
		{
		}
		protected Int32? _BillCntr;
		[PXDBInt()]
		[PXDefault(0)]
		public virtual Int32? BillCntr
		{
			get
			{
				return this._BillCntr;
			}
			set
			{
				this._BillCntr = value;
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
		#region Cleared
		public abstract class cleared : PX.Data.IBqlField
		{
		}
		protected Boolean? _Cleared;
		[PXDBBool()]
		[PXDefault(false)]
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
		#region PTInstanceID
		public abstract class pTInstanceID : PX.Data.IBqlField
		{
		}
		protected Int32? _PTInstanceID;
		[PXDBInt()]
		public virtual Int32? PTInstanceID
		{
			get
			{
				return this._PTInstanceID;
			}
			set
			{
				this._PTInstanceID = value;
			}
		}
		#endregion

		#region DepositAsBatch
		public abstract class depositAsBatch : PX.Data.IBqlField
		{
		}
		protected Boolean? _DepositAsBatch;
		[PXDBBool()]
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
		#region Deposited
		public abstract class deposited : PX.Data.IBqlField
		{
		}
		protected Boolean? _Deposited;
		[PXDBBool()]
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
		#region DepositType
		public abstract class depositType : PX.Data.IBqlField
		{
		}
		protected String _DepositType;
		
		[PXDBString(3, IsFixed = true)]
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
	}
}
