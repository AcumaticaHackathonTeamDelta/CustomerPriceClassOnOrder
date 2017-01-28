using System;

using PX.Data;

using PX.Objects.CR;
using PX.Objects.GL;

namespace PX.Objects.AR.Standalone
{
	/// <summary>
	/// An alias descendant version of <see cref="ARRegister"/>. Can be 
	/// used e.g. to avoid behaviour when <see cref="PXSubstituteAttribute"/> 
	/// substitutes <see cref="ARRegister"/> for derived classes. Can also be used
	/// as a table alias if <see cref="ARRegister"/> is joined multiple times in BQL.
	/// </summary>
	/// <remarks>
	/// Contains all BQL fields from the base class, which is enforced by unit tests.
	/// This class should NOT override any properties. If you need such behaviour,
	/// derive from this class, do not alter it.
	/// </remarks>
	[Serializable]
	[PXHidden]
	[PXPrimaryGraph(new Type[] 
		{
			typeof(SO.SOInvoiceEntry),
			typeof(ARCashSaleEntry),
			typeof(ARInvoiceEntry),
			typeof(ARPaymentEntry)
		},
		new Type[] 
		{
			typeof(Select<
				AR.ARInvoice,
				Where<
					AR.ARInvoice.docType, Equal<Current<AR.ARInvoice.docType>>,
					And<AR.ARInvoice.refNbr, Equal<Current<AR.ARInvoice.refNbr>>,
					And<AR.ARInvoice.origModule, Equal<BatchModule.moduleSO>,
					And<AR.ARInvoice.released, Equal<False>>>>>>),
			typeof(Select<
				ARCashSale,
				Where<
					ARCashSale.docType, Equal<Current<ARCashSale.docType>>,
					And<ARCashSale.refNbr, Equal<Current<ARCashSale.refNbr>>>>>),
			typeof(Select<
				AR.ARInvoice,
				Where<
					AR.ARInvoice.docType, Equal<Current<AR.ARInvoice.docType>>,
					And<AR.ARInvoice.refNbr, Equal<Current<AR.ARInvoice.refNbr>>>>>),
			typeof(Select<
				AR.ARPayment,
				Where<
					AR.ARPayment.docType, Equal<Current<AR.ARPayment.docType>>,
					And<AR.ARPayment.refNbr, Equal<Current<AR.ARPayment.refNbr>>>>>)
		})]
	public partial class ARRegisterAlias : AR.ARRegister
	{
		public new abstract class selected : IBqlField { }
		public new abstract class branchID : IBqlField { }
		public new abstract class docType : IBqlField { }
		public new abstract class printDocType : IBqlField { }
		public new abstract class refNbr : IBqlField { }
		public new abstract class origModule : IBqlField { }
		public new abstract class docDate : IBqlField { }
		public new abstract class origDocDate : IBqlField { }
		public new abstract class dueDate : IBqlField { }
		public new abstract class tranPeriodID : IBqlField { }
		public new abstract class finPeriodID : IBqlField { }
		public new abstract class customerID : IBqlField { }
		public new abstract class customerID_Customer_acctName : IBqlField { }
		public new abstract class customerLocationID : IBqlField { }
		public new abstract class curyID : IBqlField { }
		public new abstract class aRAccountID : IBqlField { }
		public new abstract class aRSubID : IBqlField { }
		public new abstract class lineCntr : IBqlField { }
		public new abstract class adjCntr : IBqlField { }
		public new abstract class curyInfoID : IBqlField { }
		public new abstract class curyOrigDocAmt : IBqlField { }
		public new abstract class origDocAmt : IBqlField { }
		public new abstract class curyDocBal : IBqlField { }
		public new abstract class docBal : IBqlField { }
		public new abstract class curyOrigDiscAmt : IBqlField { }
		public new abstract class origDiscAmt : IBqlField { }
		public new abstract class curyDiscTaken : IBqlField { }
		public new abstract class discTaken : IBqlField { }
		public new abstract class curyDiscBal : IBqlField { }
		public new abstract class discBal : IBqlField { }
		public new abstract class discTot : IBqlField { }
		public new abstract class curyDiscTot : IBqlField { }
		public new abstract class docDisc : IBqlField { }
		public new abstract class curyDocDisc : IBqlField { }
		public new abstract class curyChargeAmt : IBqlField { }
		public new abstract class chargeAmt : IBqlField { }
		public new abstract class docDesc : IBqlField { }
		public new abstract class createdByID : IBqlField { }
		public new abstract class createdByScreenID : IBqlField { }
		public new abstract class createdDateTime : IBqlField { }
		public new abstract class lastModifiedByID : IBqlField { }
		public new abstract class lastModifiedByScreenID : IBqlField { }
		public new abstract class lastModifiedDateTime : IBqlField { }
		public new abstract class Tstamp : IBqlField { }
		public new abstract class docClass : IBqlField { }
		public new abstract class batchNbr : IBqlField { }
		public new abstract class batchSeq : IBqlField { }
		public new abstract class released : IBqlField { }
		public new abstract class openDoc : IBqlField { }
		public new abstract class hold : IBqlField { }
		public new abstract class scheduled : IBqlField { }
		public new abstract class voided : IBqlField { }
		public new abstract class selfVoidingDoc : IBqlField { }
		public new abstract class noteID : IBqlField { }
		public new abstract class closedFinPeriodID : IBqlField { }
		public new abstract class closedTranPeriodID : IBqlField { }
		public new abstract class rGOLAmt : IBqlField { }
		public new abstract class curyRoundDiff : IBqlField { }
        public new abstract class roundDiff : IBqlField { }
		public new abstract class scheduleID : IBqlField { }
		public new abstract class impRefNbr : IBqlField { }
		public new abstract class statementDate : IBqlField { }
		public new abstract class salesPersonID : IBqlField { }
		public new abstract class isTaxValid : IBqlField { }
		public new abstract class isTaxPosted : IBqlField { }
		public new abstract class isTaxSaved : IBqlField { }
		public new abstract class origDocType : IBqlField { }
		public new abstract class origRefNbr : IBqlField { }
		public new abstract class status : IBqlField { }
		public new abstract class curyDiscountedDocTotal : IBqlField { }
		public new abstract class discountedDocTotal : IBqlField { }
		public new abstract class curyDiscountedTaxableTotal : IBqlField { }
		public new abstract class discountedTaxableTotal : IBqlField { }
		public new abstract class curyDiscountedPrice : IBqlField { }
		public new abstract class discountedPrice : IBqlField { }
		public new abstract class hasPPDTaxes : IBqlField { }
		public new abstract class pendingPPD : IBqlField { }
	}
}
