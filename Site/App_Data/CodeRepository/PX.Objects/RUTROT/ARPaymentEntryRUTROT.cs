using System;
using PX.Common;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.AR;
using PX.Objects.CS;
using System.Collections.Generic;
using System.Linq;


namespace PX.Objects.RUTROT
{
	public class ARPaymentEntryRUTROT : PXGraphExtension<ARPaymentEntry>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.rutRotDeduction>();
		}

		public PXSelectJoin<ARAdjust, LeftJoin<ARInvoice, 
			On<ARInvoice.docType, Equal<ARAdjust.adjdDocType>, And<ARInvoice.refNbr, Equal<ARAdjust.adjdRefNbr>>>,
			LeftJoin<RUTROT, On<RUTROT.docType, Equal<ARInvoice.docType>, And<RUTROT.refNbr, Equal<ARInvoice.refNbr>>>>>, 
			Where<ARAdjust.adjgDocType, Equal<Current<ARPayment.docType>>, 
				And<ARAdjust.adjgRefNbr, Equal<Current<ARPayment.refNbr>>, 
					And<ARAdjust.adjNbr, Equal<Current<ARPayment.adjCntr>>>>>> Adjustments;
		protected void SetPaymentAmount(decimal? amount)
		{
			Base.Document.Current.CuryOrigDocAmt = amount;
			Base.Document.Update(Base.Document.Current);

			var adj = Base.Adjustments.SelectSingle();
			adj.CuryAdjgAmt = amount;
			Base.Adjustments.Cache.Update(adj);
		}
		[PXOverride]
		public virtual void CreatePayment(ARInvoice ardoc)
		{
			Base.CreatePayment(ardoc, null, null, null, true);
			RUTROT rowRR = PXSelect<RUTROT, Where<RUTROT.docType, Equal<Required<ARInvoice.docType>>,
			And<RUTROT.refNbr, Equal<Required<ARInvoice.refNbr>>>>>.Select(this.Base, ardoc.DocType, ardoc.RefNbr);
			if (PXAccess.FeatureInstalled<FeaturesSet.rutRotDeduction>() && ardoc != null && PXCache<ARInvoice>.GetExtension<ARInvoiceRUTROT>(ardoc)?.IsRUTROTDeductible == true)
			{
				if (rowRR.IsClaimed != true)
				{
					SetPaymentAmount(Math.Max((ardoc.CuryDocBal - rowRR.CuryDistributedAmt) ?? 0m, 0m));
				}
				else if (rowRR.IsClaimed == true && ardoc.CuryDocBal == rowRR.CuryDistributedAmt)
				{

					PXCache<ARPayment>.GetExtension<ARPaymentRUTROT>(Base.Document.Current).IsRUTROTPayment = true;
					Base.Document.Update(Base.Document.Current);
				}
			}
		}
		[PXDecimal()]
		protected virtual void ARInvoice_CuryRUTROTUndistributedAmt_CacheAttached(PXCache sender)
		{
		}
		protected virtual void ARPayment_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			ARPayment doc = e.Row as ARPayment;

			Branch branch = Base.CurrentBranch.SelectSingle(doc.BranchID);
			var adjustments = Adjustments.Select();
			List<ARInvoice> invoices = adjustments.Select(d => d.GetItem<ARInvoice>()).ToList();
			List<RUTROT> rutrots= adjustments.Select(d => d.GetItem<RUTROT>()).ToList();
			bool hasRutRotInvoices = invoices.Any(d => (d != null && PXCache<ARInvoice>.GetExtension<ARInvoiceRUTROT>(d)?.IsRUTROTDeductible == true));
			bool someInvoiceClaimed = rutrots.Any(d => (d != null && d.IsClaimed == true));

			PXUIFieldAttribute.SetVisible<ARPaymentRUTROT.isRUTROTPayment>(Base.Document.Cache, e.Row, branch != null && PXCache<Branch>.GetExtension<BranchRUTROT>(branch).AllowsRUTROT == true && DocTypeSuits(doc));
			PXUIFieldAttribute.SetEnabled<ARPaymentRUTROT.isRUTROTPayment>(Base.Document.Cache, e.Row, branch != null && PXCache<Branch>.GetExtension<BranchRUTROT>(branch).RUTROTCuryID == doc.CuryID && doc.Released != true &&
																		(someInvoiceClaimed == true || hasRutRotInvoices == false));
		}
		private bool DocTypeSuits(ARPayment payment)
		{
			if (payment == null)
				return true;

			return payment.DocType == ARDocType.Payment || payment.DocType == ARDocType.VoidPayment || payment.DocType == ARDocType.CreditMemo;
		}
	}
}
