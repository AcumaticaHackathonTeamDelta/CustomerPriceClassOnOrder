using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.AP;
using PX.Objects.CM;
using PX.Objects.GL.Overrides.CloseGraph;
using PX.Objects.GL;
using PX.Objects.PO;
using System.Diagnostics;

namespace PX.Objects.IN
{
	[PX.Objects.GL.TableAndChartDashboardType]
	public class INClosing : Closing
	{
		protected override IEnumerable periodList()
		{
			string fiscalYear = null;
			INSetup.Current = INSetup.Select();
			if (INSetup.Current == null)
				yield break;
			foreach (FinPeriod per in PXSelect<FinPeriod, Where<FinPeriod.iNClosed, Equal<boolFalse>>>.Select(this))
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
			PXLongOperation.StartOperation(this, delegate() { ClosePeriod(list); });
		}

		public static new void ClosePeriod(List<FinPeriod> list)
		{
            INClosing pg = CreateInstance<INClosing>();
			for (int i = 0; i < list.Count; i++)
			{
				pg.Clear();
				FinPeriod per = list[i];
				pg.ClosePeriodProc(per);
			}
		}
		protected override void RunShowReport(Dictionary<string, string> d )
		{
			string ReportID = "IN656000"; 
			if (d.Count > 0)
			{
				throw new PXReportRequiredException(d, ReportID, PXBaseRedirectException.WindowMode.New, "Open IN Documents");
			}
		}
		protected override void ClosePeriodProc(FinPeriod p)
		{
			PXSelectBase select = new PXSelect<INRegister,
										  Where<INRegister.finPeriodID, Equal<Required<INRegister.finPeriodID>>,
											And<INRegister.released, Equal<boolFalse>>>>(this);
			INRegister doc = (INRegister)select.View.SelectSingle(p.FinPeriodID);
			if (doc != null)
			{
				throw new PXException(AP.Messages.PeriodHasUnreleasedDocs);
			}

			//MS Landed cost will not be able to create these transactions if the period is closed
			LandedCostTran lcTranFromAP = PXSelectJoin<LandedCostTran,
										InnerJoin<APRegister, On<LandedCostTran.aPDocType, Equal<APRegister.docType>,
											And<LandedCostTran.aPRefNbr, Equal<APRegister.refNbr>>>>,
											Where<LandedCostTran.invoiceDate, GreaterEqual<Required<LandedCostTran.invoiceDate>>,
												And<LandedCostTran.invoiceDate, Less<Required<LandedCostTran.invoiceDate>>,
												And<LandedCostTran.source, Equal<LandedCostTranSource.fromAP>,
												And<APRegister.released, Equal<True>,
												And<LandedCostTran.processed, Equal<False>>>>>>>.Select(this, p.StartDate,p.EndDate);
			if (lcTranFromAP != null && lcTranFromAP.LCTranID.HasValue) 
			{
				throw new PXException(Messages.PeriodHasINDocsFromAP_LCToBeCreated);
			}

			PO.LandedCostTran lcTranFromPO = PXSelectJoin<LandedCostTran,
											InnerJoin<POReceipt, On<LandedCostTran.pOReceiptNbr, Equal<POReceipt.receiptNbr>>>,
											Where<LandedCostTran.invoiceDate, GreaterEqual<Required<LandedCostTran.invoiceDate>>,
												And<LandedCostTran.invoiceDate, Less<Required<LandedCostTran.invoiceDate>>,
											And<LandedCostTran.source, Equal<LandedCostTranSource.fromPO>,
											And<POReceipt.released, Equal<True>,											
											And<LandedCostTran.processed, Equal<False>>>>>>>.Select(this, p.StartDate, p.EndDate);
			if (lcTranFromPO != null && lcTranFromPO.LCTranID.HasValue)
			{
				throw new PXException(Messages.PeriodHasINDocsFromPO_LCToBeCreated);
			}

			p.INClosed = true;
			Caches[typeof(FinPeriod)].Update(p);

			Actions.PressSave();
		}
	}
}

