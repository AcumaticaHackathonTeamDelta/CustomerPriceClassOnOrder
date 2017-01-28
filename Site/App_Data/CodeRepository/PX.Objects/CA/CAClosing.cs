using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.CM;
using PX.Objects.GL.Overrides.CloseGraph;
using PX.Objects.GL;
using System.Diagnostics;

namespace PX.Objects.CA
{
	[TableAndChartDashboardType]
	public class CAClosing : Closing
	{
		protected override IEnumerable periodList()
		{
			string fiscalYear = null;
			CASetup.Current = CASetup.Select();
			if (CASetup.Current == null)
				yield break;
			foreach (FinPeriod per in PXSelect<FinPeriod, Where<FinPeriod.cAClosed, Equal<boolFalse>>>.Select(this))
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
			CAClosing pg = PXGraph.CreateInstance<CAClosing>();
			for (int i = 0; i < list.Count; i++)
			{
				pg.Clear();
				FinPeriod per = list[i];
				pg.ClosePeriodProc(per);
			}
		}
		protected override void RunShowReport(Dictionary<string, string> d )
		{
			string ReportID = "CA656000"; 
			if (d.Count > 0)
			{
				throw new PXReportRequiredException(d, ReportID, PXBaseRedirectException.WindowMode.New, "Open CA Documents");
			}
		}
		protected override void ClosePeriodProc(FinPeriod p)
		{
			PXSelectBase select = new PXSelect<CATran,
										  Where<CATran.finPeriodID, Equal<Required<CATran.finPeriodID>>,
											And<CATran.origModule, Equal<GL.BatchModule.moduleCA>,
											And<CATran.released, Equal<boolFalse>>>>>(this);
			CATran doc = (CATran)select.View.SelectSingle(p.FinPeriodID);
			if (doc != null)
			{
				throw new PXException(Messages.PeriodHasUnreleasedDocs);
			}

			p.CAClosed = true;
			Caches[typeof(FinPeriod)].Update(p);

			Actions.PressSave();
		}
	}
}

