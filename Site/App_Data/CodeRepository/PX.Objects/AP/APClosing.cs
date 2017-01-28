using System.Collections;
using System.Collections.Generic;

using PX.Data;

using PX.Objects.Common;
using PX.Objects.GL;
using PX.Objects.PO;

namespace PX.Objects.AP
{
	[TableAndChartDashboardType]
	public class APClosing : Closing
	{
		protected override IEnumerable periodList()
		{
			string fiscalYear = null;
			APSetup.Current = APSetup.Select();

			if (APSetup.Current == null) yield break;

			foreach (FinPeriod per in PXSelect<FinPeriod, Where<FinPeriod.aPClosed, Equal<False>>>.Select(this))
			{
				if (fiscalYear == null)
				{
					fiscalYear = per.FinYear;
				}
				if (per.FinYear == fiscalYear)
				{
					yield return per;
				}
			}
		}

		protected override void StartClosePeriod(List<FinPeriod> list)
		{
			PXLongOperation.StartOperation(this, delegate { ClosePeriod(list); });
		}

		public new static void ClosePeriod(List<FinPeriod> list)
		{
			APClosing pg = CreateInstance<APClosing>();
			foreach (FinPeriod period in list)
			{
				pg.Clear();
				pg.ClosePeriodProc(period);
			}
		}

		protected override void RunShowReport(Dictionary<string, string> d )
		{
			string reportID = "AP656000"; 
			if (d.Count > 0)
			{
				d[ReportMessages.CheckReportFlag] = ReportMessages.CheckReportFlagValue;
				throw new PXReportRequiredException(d, reportID, PXBaseRedirectException.WindowMode.New, "Open AP Documents");
			}
		}

		protected override void ClosePeriodProc(FinPeriod financialPeriod)
        {
			PXSelectBase<APRegister> prebookedDocuments = new PXSelect<
				APRegister, 
				Where<
					APRegister.voided, Equal<False>,
					And<APRegister.prebooked, Equal<True>,
					And<APRegister.released, Equal<False>,
					And<APRegister.finPeriodID, Equal<Required<APRegister.finPeriodID>>>>>>>(this);

			if (prebookedDocuments.Any(financialPeriod.FinPeriodID))
			{
				throw new PXException(Messages.PeriodHasPrebookedDocs);
			}

			PXSelectBase<APRegister> unreleasedDocuments = new PXSelectJoin<
				APRegister,
					LeftJoin<APAdjust, 
						On<APAdjust.adjgDocType, Equal<APRegister.docType>, 
						And<APAdjust.adjgRefNbr, Equal<APRegister.refNbr>, 
						And<APAdjust.released, Equal<False>>>>>,
				Where<
					APRegister.voided, Equal<False>,
					And<APRegister.scheduled, Equal<False>,
					And<APRegister.rejected, Equal<False>,
					And<Where<
						APAdjust.adjgFinPeriodID, IsNull, 
						And<APRegister.released, Equal<False>, 
						And<APRegister.finPeriodID, Equal<Required<APRegister.finPeriodID>>, 
						Or<APAdjust.adjgFinPeriodID, Equal<Required<APAdjust.adjgFinPeriodID>>>>>>>>>>>(this);

			if (unreleasedDocuments.Any(financialPeriod.FinPeriodID, financialPeriod.FinPeriodID))
			{
				throw new PXException(Messages.PeriodHasUnreleasedDocs);
			}

			LandedCostTran landedCostTransactions = PXSelectJoin<
				LandedCostTran,
					InnerJoin<POReceipt, 
						On<LandedCostTran.pOReceiptNbr, Equal<POReceipt.receiptNbr>>>,
					Where<
						LandedCostTran.source, Equal<LandedCostTranSource.fromPO>,
						And<POReceipt.released, Equal<True>,
						And<LandedCostTran.postponeAP, Equal<False>,
						And<LandedCostTran.processed, Equal<False>,
						And<LandedCostTran.invoiceDate, GreaterEqual<Required<LandedCostTran.invoiceDate>>,
						And<LandedCostTran.invoiceDate, Less<Required<LandedCostTran.invoiceDate>>>>>>>>>
				.SelectWindowed(this, 0, 1, financialPeriod.StartDate, financialPeriod.EndDate);

			if (landedCostTransactions?.LCTranID != null) 
			{
				throw new PXException(Messages.PeriodHasAPDocsFromPO_LCToBeCreated);
			}
			
			financialPeriod.APClosed = true;
			Caches[typeof(FinPeriod)].Update(financialPeriod);

			Actions.PressSave();
		}
	}
}
