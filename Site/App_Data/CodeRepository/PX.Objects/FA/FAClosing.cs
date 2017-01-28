using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.GL;

namespace PX.Objects.FA
{
	[TableAndChartDashboardType]
	public class FAClosing : Closing
	{
		protected override IEnumerable periodList()
		{
			string fiscalYear = null;
			FASetup.Current = FASetup.Select();
			if (FASetup.Current == null)
				yield break;
			foreach (FinPeriod per in PXSelect<FinPeriod, Where<FinPeriod.fAClosed, Equal<False>>>.Select(this))
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
			FAClosing pg = CreateInstance<FAClosing>();
			foreach (FinPeriod p in list)
			{
				pg.Clear();
				pg.ClosePeriodProc(p);
			}
		}

		protected override void RunShowReport(Dictionary<string, string> d)
		{
			string ReportID = "FA651000";
			if (d.Count > 0)
			{
				throw new PXReportRequiredException(d, ReportID, PXBaseRedirectException.WindowMode.New, "Open FA Documents");
			}
		}

		protected override void ClosePeriodProc(FinPeriod p)
		{
			PXSelectBase select = new PXSelect<FATran,
				Where<FATran.finPeriodID, Equal<Required<FATran.finPeriodID>>,
				And<FATran.released, Equal<False>>>>(this);
			FATran tran = (FATran)select.View.SelectSingle(p.FinPeriodID);
			if (tran != null)
			{
				throw new PXException(AP.Messages.PeriodHasUnreleasedDocs);
			}

			PXResult<FABookBalance, FixedAsset, FABook> res = (PXResult<FABookBalance, FixedAsset, FABook>)PXSelectJoin<FABookBalance, 
				LeftJoin<FixedAsset, On<FixedAsset.assetID, Equal<FABookBalance.assetID>>, 
				LeftJoin<FABook, On<FABookBalance.bookID, Equal<FABook.bookID>>, 
				LeftJoin<FADetails, On<FADetails.assetID, Equal<FABookBalance.assetID>>>>>,
				Where<FABookBalance.deprFromPeriod, LessEqual<Current<FinPeriod.finPeriodID>>, 
					And<FABookBalance.deprToPeriod, GreaterEqual<Current<FinPeriod.finPeriodID>>, 
					And<FABookBalance.updateGL, Equal<True>,
					And<FixedAsset.suspended, NotEqual<True>, 
					And<FADetails.hold, NotEqual<True>, 
					And<FABookBalance.initPeriod, IsNotNull,
					And<Where<FABookBalance.currDeprPeriod, IsNull, 
							And<FABookBalance.status, Equal<FixedAssetStatus.active>,
							Or<FABookBalance.currDeprPeriod, LessEqual<Current<FinPeriod.finPeriodID>>>>>>>>>>>>>.SelectSingleBound(this, new object[]{p});
			if(res != null)
			{
				FixedAsset asset = res;
				FABook book = res;
				throw new PXException(Messages.AssetNotDepreciatedInPeriod, asset.AssetCD, book.BookCode, FinPeriodIDAttribute.FormatForError(p.FinPeriodID));
			}

			p.FAClosed = true;
			Caches[typeof(FinPeriod)].Update(p);

			Actions.PressSave();
		}

		protected override void FinPeriod_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			base.FinPeriod_RowSelected(sender, e);
			FinPeriod per = (FinPeriod)e.Row;
			if (per == null) return;

			FinPeriod p = PeriodList.Select();
			ShowAssets.SetEnabled(p.Selected == true);
		}


		public PXAction<FinPeriod> ShowAssets;
		[PXUIField(DisplayName = Messages.ShowAssets, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		//[PXLookupButton]
		[PXButton]
		public virtual IEnumerable showAssets(PXAdapter adapter)
		{
			string minPeriod = null, maxPeriod = null;
			foreach (FinPeriod period in adapter.Get())
			{
				if (period.Selected == true)
				{
					if(string.IsNullOrEmpty(minPeriod) || string.Compare(period.FinPeriodID, minPeriod) < 0)
					{
						minPeriod = period.FinPeriodID;
					}
					if (string.IsNullOrEmpty(maxPeriod) || string.Compare(period.FinPeriodID, maxPeriod) > 0)
					{
						maxPeriod = period.FinPeriodID;
					}
				}
			}

			Dictionary<string, string> d = new Dictionary<string, string>();

			d["PeriodFrom"] = minPeriod.Substring(4, 2) + minPeriod.Substring(0, 4);
			d["PeriodTo"] = maxPeriod.Substring(4, 2) + maxPeriod.Substring(0, 4);
			const string ReportID = "FA652000";
			throw new PXReportRequiredException(d, ReportID, PXBaseRedirectException.WindowMode.New, "Active Fixed Assets");
		}

	}
}

