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
using System.Diagnostics;

namespace PX.Objects.AR
{
	[TableAndChartDashboardType]
	public class ARClosing : Closing
	{
		protected override IEnumerable periodList()
		{
			string fiscalYear = null;
			ARSetup.Current = ARSetup.Select();
			if (ARSetup.Current == null)
				yield break;
			foreach (FinPeriod per in PXSelect<FinPeriod, Where<FinPeriod.aRClosed, Equal<False>>>.Select(this))
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
			ARClosing pg = CreateInstance<ARClosing>();
			for (int i = 0; i < list.Count; i++)
			{
				pg.Clear();
				FinPeriod per = list[i];
				pg.ClosePeriodProc(per);
			}
		}
		protected override void RunShowReport(Dictionary<string, string> d )
		{
			string ReportID = "AR656000"; 
			if (d.Count > 0)
			{
				throw new PXReportRequiredException(d, ReportID, PXBaseRedirectException.WindowMode.New, "Open AR Documents");
			}
		}
		protected override void ClosePeriodProc(FinPeriod p)
		{
			PXSelectBase select = new PXSelectJoin<ARRegister, 
											LeftJoin<ARAdjust, On<ARAdjust.adjgDocType, Equal<ARRegister.docType>, And<ARAdjust.adjgRefNbr, Equal<ARRegister.refNbr>, And<ARAdjust.released, Equal<False>>>>>,
											Where<ARRegister.voided, Equal<False>, 
											And<ARRegister.scheduled, Equal<False>,
											And<Where<ARAdjust.adjgFinPeriodID, IsNull, And<ARRegister.released, Equal<False>, And<ARRegister.finPeriodID, Equal<Required<ARRegister.finPeriodID>>, Or<ARAdjust.adjgFinPeriodID, Equal<Required<ARAdjust.adjgFinPeriodID>>>>>>>>>>(this);
			object doc = select.View.SelectSingle(p.FinPeriodID, p.FinPeriodID);
			if (doc != null)
			{			
				throw new PXException(AP.Messages.PeriodHasUnreleasedDocs);
			}			

			p.ARClosed = true;
			Caches[typeof(FinPeriod)].Update(p);
			Actions.PressSave();
		}

		protected override void FinPeriod_Selected_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			FinPeriod row = (FinPeriod) e.Row;
			if (row != null && row.Selected == true)
			{
				if (PXSelect<PendingPPDCreditMemoApp, Where<ARAdjust.adjgFinPeriodID, Equal<Required<ARAdjust.adjgFinPeriodID>>>>
					.SelectSingleBound(this, null, row.FinPeriodID).Count > 0)
				{
					sender.RaiseExceptionHandling<FinPeriod.selected>(row, true,
						new PXSetPropertyException(Messages.UnprocessedPPDExists, PXErrorLevel.RowWarning));
				}
			}

			base.FinPeriod_Selected_FieldUpdated(sender, e);
		}
	}
}

