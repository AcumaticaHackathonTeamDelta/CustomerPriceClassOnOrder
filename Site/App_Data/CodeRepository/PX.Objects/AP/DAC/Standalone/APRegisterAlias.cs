using System;

using PX.Data;

namespace PX.Objects.AP.Standalone
{
	/// <summary>
	/// An alias descendant version of <see cref="APRegister"/>. Can be 
	/// used e.g. to avoid behaviour when <see cref="PXSubstituteAttribute"/> 
	/// substitutes <see cref="APRegister"/> for derived classes. Can also be used
	/// as a table alias if <see cref="APRegister"/> is joined multiple times in BQL.
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
			typeof(APQuickCheckEntry),
			typeof(TX.TXInvoiceEntry),
			typeof(APInvoiceEntry),
			typeof(APPaymentEntry)
		},
		new Type[] 
		{
			typeof(Select<
				APQuickCheck,
					Where<APQuickCheck.docType, Equal<Current<APQuickCheck.docType>>,
					And<APQuickCheck.refNbr, Equal<Current<APQuickCheck.refNbr>>>>>),
			typeof(Select<
				AP.APInvoice,
				Where<
					AP.APInvoice.docType, Equal<Current<AP.APInvoice.docType>>,
					And<AP.APInvoice.refNbr, Equal<Current<AP.APInvoice.refNbr>>,
					And<Where<AP.APInvoice.released, Equal<False>, 
					And<AP.APInvoice.origModule, Equal<GL.BatchModule.moduleTX>>>>>>>),
			typeof(Select<
				AP.APInvoice,
				Where<
					AP.APInvoice.docType, Equal<Current<AP.APInvoice.docType>>,
					And<AP.APInvoice.refNbr, Equal<Current<AP.APInvoice.refNbr>>>>>),
			typeof(Select<
				AP.APPayment,
				Where<
					AP.APPayment.docType, Equal<Current<AP.APPayment.docType>>,
					And<AP.APPayment.refNbr, Equal<Current<AP.APPayment.refNbr>>>>>)
		})]
	public partial class APRegisterAlias : AP.APRegister
	{
		public new abstract class selected : IBqlField { }
		public new abstract class hidden : IBqlField { }
		public new abstract class branchID : IBqlField { }
		public new abstract class docType : IBqlField { }
		public new abstract class printDocType : IBqlField { }
		public new abstract class refNbr : IBqlField { }
		public new abstract class origModule : IBqlField { }
		public new abstract class docDate : IBqlField { }
		public new abstract class origDocDate : IBqlField { }
		public new abstract class tranPeriodID : IBqlField { }
		public new abstract class finPeriodID : IBqlField { }
		public new abstract class vendorID : IBqlField { }
		public new abstract class vendorID_Vendor_acctName : IBqlField { }
		public new abstract class vendorLocationID : IBqlField { }
		public new abstract class curyID : IBqlField { }
		public new abstract class aPAccountID : IBqlField { }
		public new abstract class aPSubID : IBqlField { }
		public new abstract class lineCntr : IBqlField { }
		public new abstract class curyInfoID : IBqlField { }
		public new abstract class curyOrigDocAmt : IBqlField { }
		public new abstract class origDocAmt : IBqlField { }
		public new abstract class curyDocBal : IBqlField { }
		public new abstract class docBal : IBqlField { }
		public new abstract class discTot : IBqlField { }
		public new abstract class curyDiscTot : IBqlField { }
		public new abstract class docDisc : IBqlField { }
		public new abstract class curyDocDisc : IBqlField { }
		public new abstract class curyOrigDiscAmt : IBqlField { }
		public new abstract class origDiscAmt : IBqlField { }
		public new abstract class curyDiscTaken : IBqlField { }
		public new abstract class discTaken : IBqlField { }
		public new abstract class curyDiscBal : IBqlField { }
		public new abstract class discBal : IBqlField { }
		public new abstract class curyOrigWhTaxAmt : IBqlField { }
		public new abstract class origWhTaxAmt : IBqlField { }
		public new abstract class curyWhTaxBal : IBqlField { }
		public new abstract class whTaxBal : IBqlField { }
		public new abstract class curyTaxWheld : IBqlField { }
		public new abstract class taxWheld : IBqlField { }
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
		public new abstract class prebookBatchNbr : IBqlField { }
		public new abstract class voidBatchNbr : IBqlField { }
		public new abstract class released : IBqlField { }
		public new abstract class openDoc : IBqlField { }
		public new abstract class hold : IBqlField { }
		public new abstract class scheduled : IBqlField { }
		public new abstract class voided : IBqlField { }
		public new abstract class printed : IBqlField { }
		public new abstract class prebooked : IBqlField { }
		public new abstract class noteID : IBqlField { }
		public new abstract class refNoteID : IBqlField { }
		public new abstract class closedFinPeriodID : IBqlField { }
		public new abstract class closedTranPeriodID : IBqlField { }
		public new abstract class rGOLAmt : IBqlField { }
		public new abstract class curyRoundDiff : IBqlField { }
		public new abstract class roundDiff : IBqlField { }
		public new abstract class curyTaxRoundDiff : IBqlField { }
		public new abstract class taxRoundDiff : IBqlField { }
		public new abstract class status : IBqlField { }
		public new abstract class scheduleID : IBqlField { }
		public new abstract class impRefNbr : IBqlField { }
		public new abstract class isTaxValid : IBqlField { }
		public new abstract class isTaxPosted : IBqlField { }
		public new abstract class isTaxSaved : IBqlField { }
		public new abstract class origDocType : IBqlField { }
		public new abstract class origRefNbr : IBqlField { }
		public new abstract class releasedOrPrebooked : IBqlField { }
		public new abstract class taxCalcMode : IBqlField { }
		public new abstract class approved : IBqlField { }
		public new abstract class rejected : IBqlField { }
		public new abstract class requestApproval : IBqlField { }
		public new abstract class dontApprove : IBqlField { }
		public new abstract class employeeWorkgroupID : IBqlField { }
		public new abstract class employeeID : IBqlField { }
		public new abstract class workgroupID : IBqlField { }
		public new abstract class ownerID : IBqlField { }
	}
}
