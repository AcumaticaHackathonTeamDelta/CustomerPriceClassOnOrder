using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Data;

namespace PX.Objects.GL
{
	public class FiscalPeriodMaint : PXGraph<FiscalPeriodMaint, FinYear>
	{
		#region Ctor  + Buttons
		public FiscalPeriodMaint()
		{
			FinYearSetup setup = this.YearSetup.Current;
			if (setup == null)
				throw new PXSetPropertyException(Messages.ConfigDataNotEnteredCMSetupAndFinancialYear);

			this.FiscalYear.Cache.AllowInsert = true;
			this.FiscalYear.Cache.AllowUpdate = true;
			this.FiscalYear.Cache.AllowDelete = true;
		}

		public PXAction<FinYear> AutoFill;
		[PXButton(Tooltip = Messages.GeneratePeriodsToolTip)]
		[PXUIField(DisplayName = Messages.GeneratePeriods, MapEnableRights = PXCacheRights.Select)]
		public virtual IEnumerable autoFill(PXAdapter adapter)
		{
			FinYear year = (FinYear)this.FiscalYear.Current;
			this.isAutoInsert = true;
			int count = 0;
			const int maxCount = 365;
			try
			{
				do
				{
					FinPeriod period = new FinPeriod();
					period = this.Periods.Insert(period);
					count++;
				}
				while (!this.isLastInsertingPeriod && count < maxCount);
			}
			finally
			{
				this.isAutoInsert = false;
			}
			//FiscalPeriodCreator<FinPeriodSetup, FinPeriod, FinYearSetup>.CreatePeriods(this.Periods.Cache, this.PeriodsSetup.Select(), year.StartDate,this.YearSetup.Current);
			return adapter.Get();
		}

		public PXAction<FinYear> addYearToTheFront;
		[PXUIField(DisplayName = "Add Year Before the First", Visible = true)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.AddNew)]
		// [PXUIField(DisplayName = Messages.GeneratePeriods, MapEnableRights = PXCacheRights.Select)]
		public virtual IEnumerable AddYearToTheFront(PXAdapter adapter)
		{

			WebDialogResult answer = this.FiscalYear.Ask(FiscalYear.Current,
					  Messages.ImportantConfirmation, Messages.InsertFinYearBeforeFirstConfirmation, MessageButtons.YesNo, MessageIcon.Question);
			if (answer == WebDialogResult.Yes)
			{
				FinYear year = new FinYear();
				this.insertToFront = true;
				year = this.FiscalYear.Insert(year);
				List<FinYear> result = new List<FinYear>(1);
				result.Add(year);
				this.insertToFront = false;
				return result;
			}
			return adapter.Get();
		}

		public PXAction<FinYear> UpdateYearOK;
		[PXUIField(DisplayName = "UpdateYear", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual void updateYearOK()
		{
			FinPeriodSaveDialog saveDialog = SaveDialog.Current;
			FinYear year = this.FiscalYear.Current;
			FinYearSetup setupYear = this.YearSetup.Current;
			FinPeriod lastPeriod = null;
			int count = 0;
			//Scan existing periods
			if (year != null)
			{
				foreach (FinPeriod iPer in this.Periods.Select(year.Year))
				{
					count++;
					PXEntryStatus status = this.Periods.Cache.GetStatus(iPer);
					if (lastPeriod == null)
						lastPeriod = iPer;
					else
						if ((iPer.StartDate.Value > lastPeriod.StartDate.Value || iPer.EndDate.Value > lastPeriod.EndDate.Value) && iPer.StartDate.Value != iPer.EndDate)
							lastPeriod = iPer;
				}
			}

			DateTime lastPeriodCalculatedEndDate = (DateTime)lastPeriod.EndDate;
			if (IsWeekBasedPeriod(setupYear.PeriodType) && setupYear.PeriodsStartDate.Value.DayOfWeek != lastPeriod.EndDate.Value.DayOfWeek && !(bool)saveDialog.MoveDayOfWeek)
			{
				int daysBack = setupYear.PeriodsStartDate.Value.DayOfWeek - lastPeriod.EndDate.Value.DayOfWeek;
				int daysForward = 7 - lastPeriod.EndDate.Value.DayOfWeek - setupYear.PeriodsStartDate.Value.DayOfWeek;

				if (Math.Abs(daysBack) < daysForward && lastPeriod.EndDate.Value.AddDays(daysBack) > lastPeriod.StartDate.Value)
				{
					lastPeriodCalculatedEndDate = lastPeriod.EndDate.Value.AddDays(daysBack);
				}
				else
				{
					lastPeriodCalculatedEndDate = lastPeriod.EndDate.Value.AddDays(daysForward);
				}
				lastPeriod.EndDate = lastPeriodCalculatedEndDate;
				lastPeriod = Periods.Update(lastPeriod);
			}

			switch (saveDialog.Method)
			{
				case FinPeriodSaveDialog.method.UpdateFinYearSetup:
					{
						TimeSpan dateDifference = (TimeSpan)(year.EndDate - lastPeriod.EndDate);

						if (!FiscalPeriodSetupCreator.IsFixedLengthPeriod(((FinYearSetup)YearSetup.Select()).FPType))
						{
							if (IsLeapDayPresent(year.EndDate.Value, lastPeriod.EndDate.Value) && !IsLeapDayPresent(setupYear.BegFinYear.Value, setupYear.BegFinYear.Value - dateDifference))
								dateDifference = (dateDifference.Days > 0) ? dateDifference.Subtract(new TimeSpan(1, 0, 0, 0)) : dateDifference.Add(new TimeSpan(1, 0, 0, 0));
							else if (IsLeapDayPresent(year.EndDate.Value, lastPeriod.EndDate.Value, 28) && IsLeapDayPresent(setupYear.BegFinYear.Value, setupYear.BegFinYear.Value - dateDifference))
								dateDifference = (dateDifference.Days > 0) ? dateDifference.Add(new TimeSpan(1, 0, 0, 0)) : dateDifference.Subtract(new TimeSpan(1, 0, 0, 0));
						}
						setupYear.BegFinYear = setupYear.BegFinYear - dateDifference;
						setupYear.PeriodsStartDate = setupYear.PeriodsStartDate - dateDifference;
						setupYear.EndYearDayOfWeek = (int)setupYear.PeriodsStartDate.Value.DayOfWeek + 1;
						year.BegFinYearHist = setupYear.BegFinYear;
						year.PeriodsStartDateHist = setupYear.PeriodsStartDate;
						YearSetup.Update(setupYear);

						year.EndDate = lastPeriod.EndDate;
						FiscalYear.Update(year);
						base.Persist();
						if (!FiscalPeriodSetupCreator.IsFixedLengthPeriod(((FinYearSetup)YearSetup.Select()).FPType))
						{
							FiscalYearSetupMaint yearSetup = PXGraph.CreateInstance<FiscalYearSetupMaint>();
							yearSetup.FiscalYearSetup.Current = setupYear;
							yearSetup.AutoFill.Press();
							yearSetup.Save.Press();
						}
						base.Persist();
						break;
					}
				case FinPeriodSaveDialog.method.UpdateNextYearStart:
					{
						year.EndDate = lastPeriod.EndDate;
						FiscalYear.Update(year);
						base.Persist();
						break;
					}
				case FinPeriodSaveDialog.method.ExtendLastPeriod:
					{
						lastPeriod.EndDate = year.EndDate;
						Periods.Update(lastPeriod);
						base.Persist();
						break;
					}
			}
		}

		#endregion

		#region Selects
		public PXFilter<FinPeriodSaveDialog> SaveDialog;
		public PXSelect<FinYear> FiscalYear;
		public PXSelect<FinPeriod, Where<FinPeriod.finYear, Equal<Current<FinYear.year>>>, OrderBy<Asc<FinPeriod.periodNbr>>> Periods;
		public PXSelectReadonly3<FinPeriodSetup, OrderBy<Asc<FinPeriodSetup.periodNbr>>> PeriodsSetup;
		public PXSetup<FinYearSetup> YearSetup;

		#endregion

		#region Fin Year Events

		protected virtual void FinYear_RowDeleting(PXCache cache, PXRowDeletingEventArgs e)
		{
			FinYear year = (FinYear)e.Row;
			bool canDelete = true;
			foreach (FinPeriod period in this.Periods.Select(year.Year))
			{
				if ((period.Closed == true) || (period.DateLocked == true))
				{
					canDelete = false;
					break;
				}
			}
			if (canDelete)
				canDelete = !(this.IsYearReferenced(year.Year));

			if (!canDelete)
			{
				throw new PXException(Messages.PeriodAlreadyUsed);
			}
			//Check for the next year
			if (this.GetNextYear(year) != null)
			{
				throw new PXException(Messages.DeleteSubseqYears);
			}

			if (year.StartDate.Value.Month != this.YearSetup.Current.PeriodsStartDate.Value.Month ||
			  year.StartDate.Value.Day != this.YearSetup.Current.PeriodsStartDate.Value.Day)
			{
				FinYearSetup setupYear = this.YearSetup.Current;
				FinYear prevYear = PXSelect<FinYear, Where<FinYear.year, Less<Current<FinYear.year>>>, OrderBy<Desc<FinYear.year>>>.SelectWindowed(this, 0, 1);
				if (prevYear != null)
				{
					setupYear.BegFinYear = prevYear.BegFinYearHist;
					setupYear.PeriodsStartDate = prevYear.PeriodsStartDateHist;
					FiscalYearSetupMaint yearSetup = PXGraph.CreateInstance<FiscalYearSetupMaint>();
					yearSetup.FiscalYearSetup.Update(setupYear);
					if (!FiscalPeriodSetupCreator.IsFixedLengthPeriod(((FinYearSetup)YearSetup.Select()).FPType))
					{
						yearSetup.FiscalYearSetup.Current = setupYear;
						yearSetup.AutoFill.Press();
						yearSetup.Save.Press();
					}
					yearSetup.Save.Press();
				}
			}
		}

		private bool IsYearReferenced(string year)
		{
			PXSelectBase select = new PXSelectJoin<GL.Batch, InnerJoin<FinPeriod, On<FinPeriod.finPeriodID, Equal<GL.Batch.finPeriodID>>>, Where<FinPeriod.finYear, Equal<Required<FinPeriod.finYear>>>>(this);
			Object result = select.View.SelectSingle(year);
			return (result != null);
		}

		protected virtual void FinYear_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			FinYear fy = (FinYear)e.Row;
			if (fy == null) return;

			FinYear nextYear = this.GetNextYear(fy);
			PXUIFieldAttribute.SetEnabled<FinYear.customPeriods>(cache, fy, (!fy.CustomPeriods ?? true) && nextYear == null);

			this.Periods.Cache.AllowInsert = (fy.CustomPeriods ?? false) && nextYear == null;
			this.Periods.Cache.AllowDelete = (fy.CustomPeriods ?? false) && nextYear == null;
			AutoFill.SetEnabled(!fy.CustomPeriods ?? true);

			PXResult<FinYear, FinYearSetup> first = (PXResult<FinYear, FinYearSetup>)(PXSelectOrderBy<FinYear, InnerJoin<FinYearSetup, On<FinYearSetup.firstFinYear, LessEqual<FinYear.year>>>, OrderBy<Asc<FinYear.year>>>.Select(this));

			bool mismatch = (((FinYear)first).Year != ((FinYearSetup)first).FirstFinYear);
			this.addYearToTheFront.SetVisible(mismatch);

			bool customPeriodsEnabled = fy.CustomPeriods ?? false;

			if (customPeriodsEnabled)
			{
				short count = 0;
				foreach (FinPeriod fp in PXSelect<FinPeriod, Where<FinPeriod.finYear, Equal<Required<FinPeriod.finYear>>>>.Select(this, fy.Year))
				{
					count++;
				}
				fy.FinPeriods = (short)count;
			}
			PXUIFieldAttribute.SetVisible(Periods.Cache, typeof(FinPeriod.length).Name, customPeriodsEnabled);
			PXUIFieldAttribute.SetVisible(Periods.Cache, typeof(FinPeriod.isAdjustment).Name, customPeriodsEnabled);
		}

		protected virtual void FinYear_RowInserting(PXCache cache, PXRowInsertingEventArgs e)
		{
			if (this.HasInsertedYear())
			{
				e.Cancel = true;
				return;
			}
			FinYear yr = (FinYear)e.Row;
			bool result = insertToFront ? this.CreatePrevYear(yr) : this.CreateNextYear(yr);
			if (!result)
				e.Cancel = true;
		}
		#endregion

		#region Fin Period Events
		protected virtual void FinPeriod_RowInserting(PXCache cache, PXRowInsertingEventArgs e)
		{
			FinPeriod period = (FinPeriod)e.Row;
			FinYear year = (FinYear)this.FiscalYear.Current;

			FinPeriod lastPeriod = PXSelect<FinPeriod, Where<FinPeriod.finYear, Equal<Current<FinYear.year>>>, OrderBy<Desc<FinPeriod.periodNbr>>>.SelectWindowed(this, 0, 1);

			int yearNbr = year.StartDate.Value.Year;

			if ((bool)FiscalYear.Current.CustomPeriods && e.ExternalCall)
			{
				period.Custom = true;
			}
			else
			{
				period.Custom = false;
			}

			FiscalPeriodCreator<FinYear, FinPeriod, FinPeriodSetup> creator = new FiscalPeriodCreator<FinYear, FinPeriod, FinPeriodSetup>(this.YearSetup.Current, year.Year, year.StartDate.Value, this.PeriodsSetup.Select().RowCast<FinPeriodSetup>());
			creator.Graph = this;
			if (!creator.fillNextPeriod(period, lastPeriod) && !(bool)year.CustomPeriods)
			{
				if (this.isAutoInsert)
					e.Cancel = true;
				else
					throw new PXException(Messages.DataInconsistent);
			}
		}

		protected virtual void FinPeriod_RowInserted(PXCache cache, PXRowInsertedEventArgs e)
		{
			FinPeriod row = (FinPeriod)e.Row;
			FinYear year = this.FiscalYear.Current;
			if (row.EndDate < row.StartDate)
			{
				row.EndDateUI = row.StartDate;
				PXUIFieldAttribute.SetError<FinPeriod.endDateUI>(cache, row, Messages.FiscalPeriodEndDateLessThanStartDate);
			}
		}

		//protected virtual void FinPeriod_RowInserted(PXCache cache, PXRowInsertedEventArgs e)
		//{
		//    FinPeriod row = (FinPeriod)e.Row;
		//    FinYear year = this.FiscalYear.Current;
		//    if (row != null && year != null) 
		//    {
		//        year.FinPeriods++;
		//    }
		//}

		protected virtual void FinPeriod_EndDateUI_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			FinPeriod row = (FinPeriod)e.Row;
			if (row == null) return;
			if ((DateTime)e.NewValue < row.StartDate)
			{
				throw new PXSetPropertyException(Messages.FiscalPeriodEndDateLessThanStartDate);
			}
		}

		protected virtual void FinPeriod_EndDateUI_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			FinPeriod row = (FinPeriod)e.Row;
			FinYear year = this.FiscalYear.Current;
			PXSelectBase<FinPeriod> nextPeriodSelect = new PXSelect<FinPeriod, Where<FinPeriod.finYear, Equal<Current<FinYear.year>>, And<FinPeriod.finPeriodID, Equal<Required<FinPeriod.finPeriodID>>>>>(this);
			FinPeriod nextPeriod = nextPeriodSelect.Select((int.Parse(row.FinPeriodID) + 1).ToString());
			if (nextPeriod != null && e.OldValue != null && row.EndDateUI != null && (row.EndDateUI < (DateTime)e.OldValue || (row.EndDateUI > (DateTime)e.OldValue && row.EndDate.Value.AddDays(1) < nextPeriod.EndDate)))
			{
				if (nextPeriod.StartDate == nextPeriod.EndDate)
					nextPeriod.EndDate = row.EndDate;
				nextPeriod.StartDate = row.EndDate;
				nextPeriod.Custom = true;
				Periods.Update(nextPeriod);
			}
			else
			{
				do
				{
					if (nextPeriod != null && nextPeriod.EndDate <= row.EndDate)
					{
						FinPeriod tmpPeriod = nextPeriodSelect.Select((int.Parse(nextPeriod.FinPeriodID) + 1).ToString());
						if (nextPeriod.StartDate != nextPeriod.EndDate || (nextPeriod.StartDate == nextPeriod.EndDate && tmpPeriod != null))
						{
							Periods.Delete(nextPeriod);
						}
						else if (tmpPeriod == null)
						{
							break;
						}
						nextPeriod = tmpPeriod;
					}
				}
				while (nextPeriod != null && nextPeriod.EndDate <= row.EndDate);
				if (nextPeriod != null)
				{
					List<FinPeriod> periodsLeft = new List<FinPeriod>();
					foreach (FinPeriod period in PXSelect<FinPeriod, Where<FinPeriod.finYear, Equal<Current<FinYear.year>>, And<FinPeriod.finPeriodID, Greater<Required<FinPeriod.finPeriodID>>>>>.Select(this, (int.Parse(row.FinPeriodID).ToString())))
					{
						periodsLeft.Add(period);
						Periods.Delete(period);
					}
					if (FiscalPeriodSetupCreator.IsFixedLengthPeriod(this.YearSetup.Current.FPType))
					{
						foreach (FinPeriod period in periodsLeft)
						{
							Periods.Insert(period);
						}
					}
					else
					{
						foreach (FinPeriod period in periodsLeft)
						{
							FinPeriod newPeriod = new FinPeriod();
							newPeriod = Periods.Insert(period);
							if (period.StartDate > row.EndDate)
							{
								newPeriod.StartDate = period.StartDate;
							}
							else
							{
								newPeriod.StartDate = row.EndDate;
							}
							newPeriod.EndDate = period.EndDate;
							if (period.StartDate == period.EndDate)
								newPeriod.EndDate = newPeriod.StartDate;
							newPeriod.Descr = period.Descr;
							Periods.Update(newPeriod);
						}
					}
				}
			}
			Periods.View.RequestRefresh();
			row.Custom = true;
		}

		protected virtual void FinPeriod_IsAdjustment_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
		{
			FinPeriod row = (FinPeriod)e.Row;
			if (e.NewValue != null && (bool)e.NewValue == true)
			{
				row.EndDate = row.StartDate;
				Periods.Update(row);
			}
		}

		protected virtual void FinPeriod_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			FinPeriod fp = (FinPeriod)e.Row;
			if (fp == null) return;
			FinYear year = this.FiscalYear.Current;
			if (year == null) return;
			FinYear nextYear = this.GetNextYear(year);
			bool customPeriodsEnabled = FiscalYear.Current.CustomPeriods ?? false;
			if (customPeriodsEnabled && nextYear == null &&
				PXSelect<FinPeriod, Where<FinPeriod.periodNbr,
				GreaterEqual<Required<FinPeriod.periodNbr>>,
				And<FinPeriod.finYear, Equal<Current<FinYear.year>>,
				And<Where<FinPeriod.dateLocked, Equal<True>,
				Or<FinPeriod.active, Equal<True>>>>>>>.Select(this, fp.PeriodNbr).Count == 0)
			{
				PXUIFieldAttribute.SetEnabled<FinPeriod.endDateUI>(cache, fp, true);
			}

			if (year.FinPeriods != null && fp.PeriodNbr != null && fp.IsAdjustment != null && !(bool)fp.IsAdjustment)
			PXUIFieldAttribute.SetEnabled<FinPeriod.isAdjustment>(cache, fp, year.FinPeriods == Int16.Parse(fp.PeriodNbr));
			
			//if (fp.StartDate == fp.EndDate && this.GetNextPeriod(fp) != null)
			//{
			//	Periods.Cache.RaiseExceptionHandling<FinPeriod.endDateUI>(Periods.Current, Periods.Current.EndDateUI,
			//		new PXSetPropertyException(Messages.FiscalPeriodAdjustmentPeriodError, PXErrorLevel.RowError));
			//}
		}

		protected virtual void FinYear_CustomPeriods_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
		{
			if (Boolean.Parse(e.NewValue.ToString()) == true)
			{
				if (FiscalYear.Ask(Messages.FiscalYearCustomPeriodsMessageTitle, Messages.FiscalYearCustomPeriodsMessage, MessageButtons.YesNo, MessageIcon.Warning) == WebDialogResult.No)
				{
					e.NewValue = false;
				}
			}

		}

		protected virtual void FinPeriod_RowDeleting(PXCache cache, PXRowDeletingEventArgs e)
		{
			FinPeriod fp = (FinPeriod)e.Row;
			PXEntryStatus status = this.Periods.Cache.GetStatus(fp);
			PXEntryStatus yearStatus = this.FiscalYear.Cache.GetStatus(this.FiscalYear.Current);
			bool isYearDeleting = PXEntryStatus.InsertedDeleted == yearStatus || PXEntryStatus.Deleted == yearStatus;
			if (status == PXEntryStatus.Notchanged || status == PXEntryStatus.Updated || status == PXEntryStatus.Deleted)
			{
				if (fp.Closed.Value || fp.DateLocked.Value)
				{
					throw new PXException(Messages.PeriodAlreadyUsed);
				}
			}
			int periodNbr = 0;
			bool isIntialized = int.TryParse(fp.PeriodNbr, out periodNbr);
			if (isIntialized)
			{
				bool isLastPeriod = (this.GetNextPeriod(fp) == null);
				bool hasLaterPeriods = false;
				//Check inserted records
				if (isLastPeriod)
				{
					string year = fp.FinYear;
					foreach (FinPeriod iPer in this.Periods.Select(year))
					{
						if (iPer.StartDate.Value > fp.StartDate.Value)
						{
							hasLaterPeriods = false;
							break;
						}
					}
				}
				if (((!(isYearDeleting || isLastPeriod)) || hasLaterPeriods) && e.ExternalCall)
				{
					throw new PXException(Messages.DeleteSubseqPeriods);
				}
			}
		}
		//protected virtual void FinPeriod_RowDeleted(PXCache cache, PXRowDeletedEventArgs e)
		//{
		//    FinPeriod row = (FinPeriod)e.Row;
		//    FinYear year = this.FiscalYear.Current;
		//    if (row != null && year != null)
		//    {
		//        year.FinPeriods--;
		//    }
		//}

		protected virtual void FinPeriod_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
		{
			FinPeriod fp = (FinPeriod)e.Row;
			//Reset fiscal period id - if it changed in parent row
			string fpID = string.Concat(fp.FinYear, fp.PeriodNbr);
			if (fp.FinPeriodID != fpID) fp.FinPeriodID = fpID; //Do we need additional processing in this case?

			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Delete)
			{
				Batch batch = PXSelect<Batch,
							Where<Batch.finPeriodID, Equal<Required<Batch.finPeriodID>>>>
							.SelectWindowed(this, 0, 1, fp.FinPeriodID);

				if (batch != null)
				{
					throw new PXException(Messages.FinancialPeriodCanNotBeDeleted, fp.FinPeriodID);
				}
			}
		}

		#endregion

		#region FinPeriodSaveDialog events
		protected virtual void FinPeriodSaveDialog_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			FinPeriodSaveDialog row = (FinPeriodSaveDialog)e.Row;
			if (row == null) return;
			FinYear year = this.FiscalYear.Current;
			if (year == null) return;
			FinYearSetup setupYear = this.YearSetup.Current;
			if (setupYear == null) return;
			FinPeriod lastPeriod = null;
			int count = 0;
			//Scan existing periods
			if (year != null)
			{
				foreach (FinPeriod iPer in this.Periods.Select(year.Year))
				{
					count++;
					PXEntryStatus status = this.Periods.Cache.GetStatus(iPer);
					if (lastPeriod == null)
						lastPeriod = iPer;
					else
						if ((iPer.StartDate.Value > lastPeriod.StartDate.Value || iPer.EndDate.Value > lastPeriod.EndDate.Value) && iPer.StartDate.Value != iPer.EndDate.Value)
							lastPeriod = iPer;
				}
			}
			if (lastPeriod == null) return;
			Dictionary<string, string> allowed = new FinPeriodSaveDialog.method.ListAttribute().ValueLabelDic;
			if (!FiscalPeriodSetupCreator.IsFixedLengthPeriod(((FinYearSetup)YearSetup.Select()).FPType))
			{
				allowed.Remove(FinPeriodSaveDialog.method.UpdateNextYearStart);
			}
			if (year.EndDate < lastPeriod.EndDate)
			{
				allowed.Remove(FinPeriodSaveDialog.method.ExtendLastPeriod);
			}

			PXStringListAttribute.SetList<FinPeriodSaveDialog.method>(sender, e.Row,
														   allowed.Keys.ToArray(),
														   allowed.Values.ToArray());

			DateTime lastPeriodCalculatedEndDate = new DateTime();
			if (IsWeekBasedPeriod(setupYear.PeriodType) && setupYear.PeriodsStartDate.Value.DayOfWeek != lastPeriod.EndDate.Value.DayOfWeek)
			{
				PXUIFieldAttribute.SetVisible<FinPeriodSaveDialog.moveDayOfWeek>(sender, row, true);
				int daysBack = setupYear.PeriodsStartDate.Value.DayOfWeek - lastPeriod.EndDate.Value.DayOfWeek;
				int daysForward = 7 - lastPeriod.EndDate.Value.DayOfWeek - setupYear.PeriodsStartDate.Value.DayOfWeek;

				if (Math.Abs(daysBack) < daysForward && lastPeriod.EndDate.Value.AddDays(daysBack) > lastPeriod.StartDate.Value)
				{
					lastPeriodCalculatedEndDate = lastPeriod.EndDate.Value.AddDays(daysBack);
				}
				else
				{
					lastPeriodCalculatedEndDate = lastPeriod.EndDate.Value.AddDays(daysForward);
				}
			}

			switch (row.Method)
			{
				case FinPeriodSaveDialog.method.UpdateNextYearStart:
					{
						row.MethodDescription = PXMessages.LocalizeFormatNoPrefix(Messages.FiscalPeriodMethodModifyNextYear, lastPeriod.EndDate.Value.ToShortDateString());
						if (IsWeekBasedPeriod(setupYear.PeriodType) && setupYear.PeriodsStartDate.Value.DayOfWeek != lastPeriod.EndDate.Value.DayOfWeek)
						{
							if (!(bool)row.MoveDayOfWeek)
							{
								row.MethodDescription = PXMessages.LocalizeFormatNoPrefix(Messages.FiscalPeriodMethodModifyNextYear, lastPeriodCalculatedEndDate.ToShortDateString());
								row.MethodDescription += PXMessages.LocalizeFormatNoPrefix(Messages.FiscalPeriodMethodEndDateMoveWarning, lastPeriodCalculatedEndDate.AddDays(-1).ToShortDateString(), setupYear.PeriodsStartDate.Value.DayOfWeek.ToString());
							}
							else
								row.MethodDescription += PXMessages.LocalizeFormatNoPrefix(Messages.FiscalPeriodMethodWeekStartWarning, setupYear.PeriodsStartDate.Value.DayOfWeek.ToString(), lastPeriod.EndDate.Value.DayOfWeek.ToString());
						}
						break;
					}
				case FinPeriodSaveDialog.method.UpdateFinYearSetup:
					{
						TimeSpan dateDifference = (TimeSpan)(lastPeriod.EndDate - year.EndDate);

						int daysToAdd = 0;
						if (!FiscalPeriodSetupCreator.IsFixedLengthPeriod(((FinYearSetup)YearSetup.Select()).FPType))
						{
							if (IsLeapDayPresent(year.EndDate.Value, lastPeriod.EndDate.Value) && !IsLeapDayPresent(setupYear.BegFinYear.Value, setupYear.BegFinYear.Value + dateDifference))
								daysToAdd = -1;
							else if (IsLeapDayPresent(year.EndDate.Value, lastPeriod.EndDate.Value, 28) && IsLeapDayPresent(setupYear.BegFinYear.Value, setupYear.BegFinYear.Value + dateDifference))
								daysToAdd = 1;
						}

						if ((lastPeriod.EndDate - year.EndDate).Value.Days > 0)
						{
							if (IsCalendarBasedPeriod(setupYear.PeriodType)) row.MethodDescription = PXMessages.LocalizeFormatNoPrefix(Messages.FiscalPeriodMethodModifyNextYearSetupDate, setupYear.BegFinYear.Value.AddDays(Math.Abs((lastPeriod.EndDate - year.EndDate).Value.Days) + daysToAdd).ToString("MMMM dd"));
							else row.MethodDescription = PXMessages.LocalizeFormatNoPrefix(Messages.FiscalPeriodMethodModifyNextYearSetupForward, (lastPeriod.EndDate - year.EndDate).Value.Days + daysToAdd);
						}
						else
						{
							if (IsCalendarBasedPeriod(setupYear.PeriodType)) row.MethodDescription = PXMessages.LocalizeFormatNoPrefix(Messages.FiscalPeriodMethodModifyNextYearSetupDate, setupYear.BegFinYear.Value.AddDays(-Math.Abs((lastPeriod.EndDate - year.EndDate).Value.Days) - daysToAdd).ToString("MMMM dd"));
							else row.MethodDescription = PXMessages.LocalizeFormatNoPrefix(Messages.FiscalPeriodMethodModifyNextYearSetupBack, Math.Abs((lastPeriod.EndDate - year.EndDate).Value.Days) + daysToAdd);
						}
						if (IsWeekBasedPeriod(setupYear.PeriodType) && setupYear.PeriodsStartDate.Value.DayOfWeek != lastPeriod.EndDate.Value.DayOfWeek)
						{
							if (!(bool)row.MoveDayOfWeek)
							{
								if (lastPeriodCalculatedEndDate != year.EndDate && (lastPeriod.EndDate - year.EndDate).Value.Days != 0)
								{
									if ((lastPeriod.EndDate - year.EndDate).Value.Days > 0)
									{
										row.MethodDescription = PXMessages.LocalizeFormatNoPrefix(Messages.FiscalPeriodMethodModifyNextYearSetupForward, (lastPeriodCalculatedEndDate - year.EndDate).Value.Days);
									}
									else
									{
										row.MethodDescription = PXMessages.LocalizeFormatNoPrefix(Messages.FiscalPeriodMethodModifyNextYearSetupBack, Math.Abs((lastPeriodCalculatedEndDate - year.EndDate).Value.Days));
									}
								}
								else
								{
									row.MethodDescription = Messages.FiscalPeriodMethodModifyNextYearNoChange;
								}
								row.MethodDescription += PXMessages.LocalizeFormatNoPrefix(Messages.FiscalPeriodMethodEndDateMoveWarning, lastPeriodCalculatedEndDate.AddDays(-1).ToShortDateString(), setupYear.PeriodsStartDate.Value.DayOfWeek.ToString());
							}
							else
							{
								row.MethodDescription += PXMessages.LocalizeFormatNoPrefix(Messages.FiscalPeriodMethodWeekStartWarning, setupYear.PeriodsStartDate.Value.DayOfWeek.ToString(), lastPeriod.EndDate.Value.DayOfWeek.ToString());
							}
						}
						break;
					}
				case FinPeriodSaveDialog.method.ExtendLastPeriod:
					{
						row.MethodDescription = PXMessages.LocalizeFormatNoPrefix(Messages.FiscalPeriodMethodExtendLastPeriod, year.EndDate.Value.AddDays(-1).ToShortDateString());
						break;
					}
			}
		}

		#endregion

		public override void Persist()
		{
			FinYear year = this.FiscalYear.Current;
			FinPeriod lastPeriod = null;
			int count = 0;

			//Scan existing periods
			if (year != null)
			{
				PXResultset<FinPeriod> allPeriods = this.Periods.Select(year.Year);
				foreach (FinPeriod iPer in allPeriods)
				{
					if (iPer.EndDate < iPer.StartDate)
					{
						throw new PXException(Messages.FiscalPeriodEndDateLessThanStartDate);
					}
					count++;
					PXEntryStatus status = this.Periods.Cache.GetStatus(iPer);
					if (lastPeriod == null)
						lastPeriod = iPer;
					else
						if (iPer.StartDate.Value > lastPeriod.StartDate.Value || iPer.EndDate.Value > lastPeriod.EndDate.Value || Int32.Parse(iPer.FinPeriodID) > Int32.Parse(lastPeriod.FinPeriodID))
							lastPeriod = iPer;
				}
				foreach (FinPeriod iPer in allPeriods)
				{
					if (iPer.StartDate == iPer.EndDate && iPer != lastPeriod)
					{
						Periods.Cache.RaiseExceptionHandling<FinPeriod.endDateUI>(iPer, Periods.Current.EndDateUI,
						new PXSetPropertyException(Messages.FiscalPeriodAdjustmentPeriodError, PXErrorLevel.RowError));
						throw new PXException(Messages.FiscalPeriodAdjustmentPeriodError);
					}
				}

				if (lastPeriod == null)
				{
					throw new PXException(Messages.FiscalPeriodNoPeriods);
				}
				if (year.EndDate != lastPeriod.EndDate)
				{
					SaveDialog.Current.Message = PXMessages.LocalizeFormatNoPrefix(Messages.FiscalPeriodEndDateNotEqualFinYearEndDate, lastPeriod.EndDate.Value.AddDays(-1).ToShortDateString(), year.EndDate.Value.AddDays(-1).ToShortDateString());
					SaveDialog.AskExt();
				}
				else
				{
					var changedPeriods = GetUpdatedPeriods();

					base.Persist();

					UpdateTaxTranFinDate(changedPeriods);
				}
			}
			else
			{
				base.Persist();
			}
		}

		private IEnumerable<FinPeriod> GetUpdatedPeriods()
		{
			var olds = PXSelectReadonly<FinPeriod, Where<FinPeriod.finYear, Equal<Current<FinYear.year>>>>.Select(this).RowCast<FinPeriod>();
			var news = this.Periods.Select().RowCast<FinPeriod>();

			return olds.Join(news, fp => fp.FinPeriodID, fp => fp.FinPeriodID, (o, n) => new Tuple<FinPeriod, FinPeriod>(o, n))
				.Where(pair => pair.Item1.EndDate != pair.Item2.EndDate)
				.Select(pair => pair.Item2);
		}

		private void UpdateTaxTranFinDate(IEnumerable<FinPeriod> periods)
		{
			foreach(var period in periods)
			{
				PXUpdate<Set<TX.TaxTran.finDate, Required<TX.TaxTran.finDate>>,
					TX.TaxTran,
					Where<TX.TaxTran.finPeriodID, Equal<Required<FinPeriod.finPeriodID>>,
						And<TX.TaxTran.taxPeriodID, IsNull>>>.Update(this, period.EndDate.Value.AddDays(-1), period.FinPeriodID);
			}
		}

		#region Year Utility Functions
		private bool CreateNextYear(FinYear newYear)
		{
			PXSelectBase select = new PXSelect<GL.FinYearSetup>(this);
			GL.FinYearSetup setupValue = (GL.FinYearSetup)select.View.SelectSingle();

			FinYear lastYear = FindLatestYear();
			return FiscalYearCreator<FinYear, FinPeriod, FinPeriodSetup>.CreateNextYear(setupValue, lastYear, newYear);
		}

		private bool CreatePrevYear(FinYear newYear)
		{
			PXSelectBase select = new PXSelect<GL.FinYearSetup>(this);
			GL.FinYearSetup setupValue = (GL.FinYearSetup)select.View.SelectSingle();

			FinYear lastYear = FindEarliestYear();

			return FiscalYearCreator<FinYear, FinPeriod, FinPeriodSetup>.CreatePrevYear(setupValue, lastYear, newYear);
		}

		private bool HasInsertedYear()
		{
			bool found = false;
			foreach (Object fp in this.FiscalYear.Cache.Inserted)
			{
				found = true;
				break;
			}
			return found;
		}

		private FinYear FindLatestYear()
		{
			FinYear result = null;
			//foreach (FinYear year in this.FinYear.Cache)
			foreach (FinYear year in PXSelect<FinYear>.Select(this))
			{
				int yr;
				if (!int.TryParse(year.Year, out yr)) { continue; } //Skip invalid records
				if (result != null)
				{
					if (int.Parse(result.Year) < yr) result = year;

				}
				else
					result = year;

			}
			return result;
		}

		private FinYear FindEarliestYear()
		{
			FinYear result = null;
			//foreach (FinYear year in this.FinYear.Cache)
			foreach (FinYear year in PXSelect<FinYear>.Select(this))
			{
				int yr;
				if (!int.TryParse(year.Year, out yr)) { continue; } //Skip invalid records
				if (result != null)
				{
					if (int.Parse(result.Year) > yr) result = year;

				}
				else
					result = year;

			}
			return result;
		}

		//Returns next period (from db) or null if there is none
		private FinYear GetNextYear(FinYear aYear)
		{
			PXSelectBase select = new PXSelect<FinYear, Where<FinYear.startDate, Greater<Required<FinYear.startDate>>>, OrderBy<Asc<FinPeriod.startDate>>>(this);
			FinYear result = (FinYear)select.View.SelectSingle(aYear.StartDate.Value);
			return result;
		}

		private bool IsWeekBasedPeriod(string periodType)
		{
			return (periodType == FinPeriodType.Week
					|| periodType == FinPeriodType.BiWeek
					|| periodType == FinPeriodType.FourWeek
					|| periodType == FinPeriodType.FourFourFive
					|| periodType == FinPeriodType.FourFiveFour
					|| periodType == FinPeriodType.FiveFourFour
					);
		}

		private bool IsCalendarBasedPeriod(string periodType)
		{
			return (periodType == FinPeriodType.BiMonth
					|| periodType == FinPeriodType.Month
					|| periodType == FinPeriodType.Quarter
					);
		}

		private bool IsLeapDayPresent(DateTime date1, DateTime date2, int leapDay = 29)
		{
			bool leapDayPresent = false;
			DateTime leapDate1 = DateTime.MinValue;
			DateTime leapDate2 = DateTime.MinValue;
			if (leapDay == 29)
			{
				if (DateTime.IsLeapYear(date1.Year))
					leapDate1 = new DateTime(date1.Year, 2, leapDay);
				if (DateTime.IsLeapYear(date2.Year))
					leapDate2 = new DateTime(date2.Year, 2, leapDay);
			}
			else
			{
				leapDate1 = new DateTime(date1.Year, 2, leapDay);
				leapDate2 = new DateTime(date2.Year, 2, leapDay);
			}

			if (((date1 > date2 && (leapDate1 > date2 && date1 >= leapDate1)) || (date1 < date2 && (leapDate1 <= date2 && date1 < leapDate1))) || 
				((date1 > date2 && (leapDate2 > date2 && date1 >= leapDate2)) || (date1 < date2 && (leapDate2 <= date2 && date1 < leapDate2))))
			{
				leapDayPresent = true;
			}

			return leapDayPresent;
		}

		#endregion

		#region FinPeriod Utility Functions
		//Returns next period (from db) or null if there is none
		private FinPeriod GetNextPeriod(FinPeriod aPeriod)
		{
			//foreach (FinYear year in this.FinYear.Cache)
			PXSelectBase select = new PXSelect<FinPeriod, Where<FinPeriod.startDate, Greater<Required<FinPeriod.startDate>>>, OrderBy<Asc<FinPeriod.startDate>>>(this);
			FinPeriod result = (FinPeriod)select.View.SelectSingle(aPeriod.StartDate.Value);
			return result;
		}

		#endregion
		#region Internal Variables
		private bool insertToFront = false;
		private bool isAutoInsert = false;
		private bool isLastInsertingPeriod = false;
		#endregion
	}

	public class FiscalYearCreator<TYear, TPeriod, TPeriodSetup> : FiscalPeriodCreator<TYear, TPeriod, TPeriodSetup>
		where TYear : class, GL.IYear, IBqlTable, new()
		where TPeriod : class, GL.IPeriod, IBqlTable, new()
		where TPeriodSetup : class, GL.IPeriodSetup
	{

		public FiscalYearCreator(IYearSetup aSetup, string aYear, DateTime? startDate, IEnumerable<TPeriodSetup> aPeriodSetup)
			: base(aSetup, aYear, startDate, aPeriodSetup)
		{
		}

		public static bool CreateNextYear(IYearSetup setupValue, TYear lastYear, TYear newYear)
		{
			if (setupValue == null)
				throw new PXSetPropertyException(Messages.ConfigDataNotEntered);
			int yearNumber = -1;
			if (lastYear != null)
			{
				if (!lastYear.EndDate.HasValue)
				{
					FiscalPeriodSetupCreator<TPeriod> lastYearCreator = new FiscalPeriodSetupCreator<TPeriod>(lastYear.Year, lastYear.StartDate.Value, setupValue.FPType, setupValue.HasAdjustmentPeriod.Value, setupValue.AdjustToPeriodStart.Value, setupValue.PeriodLength, setupValue.BegFinYear.Value, setupValue.EndYearCalcMethod, setupValue.EndYearDayOfWeek.Value);
					newYear.StartDate = lastYearCreator.YearEnd;
				}
				else
					newYear.StartDate = lastYear.EndDate;
				yearNumber = int.Parse(lastYear.Year);
				yearNumber++;

				string yearNumberAsString = FiscalPeriodSetupCreator.FormatYear(yearNumber);
				DateTime yearStartDate = CalcYearStartDate(setupValue, yearNumberAsString);
				FiscalPeriodSetupCreator<TPeriod> newYearCreator = new FiscalPeriodSetupCreator<TPeriod>(
																										yearNumberAsString,
																										newYear.StartDate.Value,
																										setupValue.FPType,
																										setupValue.HasAdjustmentPeriod.Value,
																										setupValue.AdjustToPeriodStart.Value,
																										setupValue.PeriodLength,
																										yearStartDate,
																										setupValue.EndYearCalcMethod,
																										setupValue.EndYearDayOfWeek.Value);
				newYear.EndDate = newYearCreator.YearEnd;
				newYear.FinPeriods = (short)newYearCreator.ActualNumberOfPeriods;
				newYear.Year = FiscalPeriodSetupCreator.FormatYear(yearNumber);
				////if (FiscalPeriodSetupCreator.IsFixedLengthPeriod((FiscalPeriodSetupCreator.FPType)setupValue.PeriodType))
				////{
				//FiscalPeriodSetupCreator<TPeriod> creator = new FiscalPeriodSetupCreator<TPeriod>(lastYear.Year, lastYear.StartDate.Value, (FiscalPeriodSetupCreator.FPType)setupValue.PeriodType, setupValue.HasAdjustmentPeriod.Value, setupValue.AdjustToPeriodStart.Value, setupValue.PeriodLength);
				//newYear.StartDate = creator.YearEnd;
				//}
				//else
				//{
				//    newYear.StartDate = new DateTime(lastYear.StartDate.Value.Year + 1, setupValue.BegFinYear.Value.Month, setupValue.BegFinYear.Value.Day);
				//}
			}
			else
			{
				CreateFromSetup(setupValue, newYear);
			}
			return true;
		}

		public static bool CreatePrevYear(IYearSetup setupValue, TYear firstYear, TYear newYear)
		{
			if (setupValue == null)
				throw new PXSetPropertyException(Messages.ConfigDataNotEntered);

			int yearNumber = -1;
			int setupFirstYear = string.IsNullOrEmpty(setupValue.FirstFinYear) ? setupValue.BegFinYear.Value.Year : int.Parse(setupValue.FirstFinYear); // No need to increment in this case
			if (firstYear != null)
			{
				yearNumber = int.Parse(firstYear.Year);
				yearNumber--;
				if (yearNumber < setupFirstYear)
					return false;
				//newYear.StartDate = new DateTime(firstYear.StartDate.Value.Year - 1, setupValue.BegFinYear.Value.Month, setupValue.BegFinYear.Value.Day);				
				FiscalPeriodSetupCreator<FinPeriodSetup> creator = new FiscalPeriodSetupCreator<FinPeriodSetup>(setupValue.FirstFinYear, setupValue.PeriodsStartDate.Value, setupValue.FPType, (bool)setupValue.HasAdjustmentPeriod, (bool)setupValue.AdjustToPeriodStart, setupValue.PeriodLength, setupValue.BegFinYear.Value, setupValue.EndYearCalcMethod, setupValue.EndYearDayOfWeek.Value);
				string yearNumberAsString = FiscalPeriodSetupCreator.FormatYear(yearNumber);
				DateTime periodsStartingDate = creator.CalcPeriodsStartDate(yearNumberAsString);
				DateTime yearStartingDate = CalcYearStartDate(setupValue, yearNumberAsString);
				newYear.StartDate = periodsStartingDate;
				FiscalPeriodSetupCreator<FinPeriodSetup> prevYearCreator = new FiscalPeriodSetupCreator<FinPeriodSetup>(yearNumberAsString, newYear.StartDate.Value, setupValue.FPType, (bool)setupValue.HasAdjustmentPeriod, (bool)setupValue.AdjustToPeriodStart, setupValue.PeriodLength, yearStartingDate, setupValue.EndYearCalcMethod, setupValue.EndYearDayOfWeek.Value);
				newYear.EndDate = prevYearCreator.YearEnd;
				newYear.FinPeriods = (short)prevYearCreator.ActualNumberOfPeriods;
				newYear.Year = FiscalPeriodSetupCreator.FormatYear(yearNumber);
			}
			else
			{
				CreateFromSetup(setupValue, newYear);
			}
			return true;
		}

		public static void CreateFromSetup(IYearSetup setupValue, TYear newYear)
		{
			FiscalPeriodSetupCreator<TPeriod> creator = new FiscalPeriodSetupCreator<TPeriod>(setupValue.FirstFinYear, setupValue.PeriodsStartDate.Value, setupValue.FPType, setupValue.HasAdjustmentPeriod.Value, setupValue.AdjustToPeriodStart.Value, setupValue.PeriodLength, setupValue.BegFinYear.Value, setupValue.EndYearCalcMethod, setupValue.EndYearDayOfWeek.Value);
			newYear.StartDate = new DateTime(setupValue.PeriodsStartDate.Value.Year, setupValue.PeriodsStartDate.Value.Month, setupValue.PeriodsStartDate.Value.Day);
			newYear.EndDate = creator.YearEnd;
			newYear.Year = setupValue.FirstFinYear;
			newYear.FinPeriods = FiscalPeriodSetupCreator.IsFixedLengthPeriod(setupValue.FPType) ? (short)creator.ActualNumberOfPeriods : setupValue.FinPeriods;
		}

		public static TYear CreateNextYear(PXGraph graph, IYearSetup setupValue, IEnumerable<TPeriodSetup> periods, TYear lastYear)
		{
			TYear newYear = new TYear();
			CreateNextYear(setupValue, lastYear, newYear);
			if ((newYear = (TYear)graph.Caches[typeof(TYear)].Insert(newYear)) != null)
			{
				CreatePeriods(graph.Caches[typeof(TPeriod)], periods, newYear.Year, newYear.StartDate, setupValue);
			}

			return newYear;
		}

		public static TYear CreatePrevYear(PXGraph graph, IYearSetup setupValue, IEnumerable<TPeriodSetup> periods, TYear firstYear)
		{
			TYear newYear = new TYear();
			CreatePrevYear(setupValue, firstYear, newYear);
			if ((newYear = (TYear)graph.Caches[typeof(TYear)].Insert(newYear)) != null)
			{
				CreatePeriods(graph.Caches[typeof(TPeriod)], periods, newYear.Year, newYear.StartDate, setupValue);
			}

			return newYear;
		}

		public static DateTime CalcYearStartDate(IYearSetup aSetup, string aYearNumber)
		{
			int yearDelta = aSetup.BegFinYear.Value.Year - int.Parse(aSetup.FirstFinYear);
			return new DateTime(int.Parse(aYearNumber) + yearDelta, aSetup.BegFinYear.Value.Month, aSetup.BegFinYear.Value.Day);
		}
	}

	public class FiscalPeriodCreator<TYear, TPeriod, TPeriodSetup>
		where TYear : class, GL.IYear, IBqlTable, new()
		where TPeriod : class, GL.IPeriod, IBqlTable, new()
		where TPeriodSetup : class, GL.IPeriodSetup
	{
		//public FiscalPeriodCreator(int aFiscalYear, IEnumerable aPeriods)
		//{
		//    this.fiscalYear = aFiscalYear;
		//    this.periodsSetup = aPeriods;
		//}

		//public FiscalPeriodCreator(DateTime? YearStartDate, IEnumerable aPeriods)
		//    :this(((DateTime)YearStartDate).Year, aPeriods)
		//{
		//    periodsStartDate = YearStartDate.Value;
		//}

		public FiscalPeriodCreator(IYearSetup aYearSetup, string aFinYear, DateTime? aPeriodsStartDate, IEnumerable<TPeriodSetup> aPeriods)
		{
			this.yearSetup = aYearSetup;
			this.finYear = aFinYear;
			this.periodsStartDate = aPeriodsStartDate.Value;
			this.periodsSetup = aPeriods;
			//Calc internanal variables
			this.yearStartDate = FiscalYearCreator<TYear, TPeriod, TPeriodSetup>.CalcYearStartDate(this.yearSetup, aFinYear);
			this.firstPeriodYear = this.periodsStartDate.Year;
		}

		public PXGraph Graph;

		public virtual void CreatePeriods(PXCache cache)
		{
			TPeriod iPer = null;
			int count = 0;
			const int maxCount = 1000;
			do
			{
				iPer = this.createNextPeriod(iPer);
				if (iPer != null)
				{
					cache.Insert(iPer);
					count++;
				}
			}
			while (iPer != null && count < maxCount);
			if (count >= maxCount)
				throw new PXException(Messages.ERR_InfiniteLoopDetected);
		}

		//public static void CreatePeriods(PXCache cache, IEnumerable periods, DateTime? YearStartDate, Int16? NumberPeriods)
		//{
		//    new FiscalPeriodCreator<TPeriodSetup, TPeriod, TYearSetup>(YearStartDate, periods).CreatePeriods(cache);
		//}		   

		public static void CreatePeriods(PXCache cache, IEnumerable<TPeriodSetup> aPeriods, string aYear, DateTime? YearStartDate, IYearSetup aYearSetup)
		{
			new FiscalPeriodCreator<TYear, TPeriod, TPeriodSetup>(aYearSetup, aYear, YearStartDate, aPeriods).CreatePeriods(cache);
		}


		public virtual bool fillNextPeriod(TPeriod result, TPeriod current)
		{
			if (FiscalPeriodSetupCreator.IsFixedLengthPeriod(yearSetup.FPType)
				|| (current != null && (current.Custom ?? false))
				|| (result != null && (result.Custom ?? false)))
			{
				if (!this.FinPeriodCreator.fillNextPeriod(result, current)) return false;
				result.Closed = false;
				result.DateLocked = false;
				result.FinPeriodID = result.FinYear + result.PeriodNbr;
				return true;
			}
			else
			{
				return fillNextFromTemplate(result, current);
			}
		}

		protected virtual bool fillNextFromTemplate(TPeriod result, TPeriod current)
		{
			IPeriodSetup source = null;
			IPeriodSetup prev = null;
			int yearDelta = 0;
			//This algorithm relies on the fact that FiscalPeriodSetups are sorted by PeriodNbr in accending order;
			foreach (TPeriodSetup res in this.periodsSetup)
			{
				TPeriodSetup fps = res;
				yearDelta += GetYearDelta(fps, prev); //accumulate year change for all previous periods 
				if (IsEqual(prev, current))
				{
					source = fps;
					break;
				}
				prev = fps;
			}
			if (source == null) return false;
			int adjustedYear = this.firstPeriodYear + yearDelta;
			CopyFrom(result, source, adjustedYear, Graph);
			result.Closed = false;
			result.DateLocked = false;
			return true;
		}

		static int GetYearDelta(IPeriodSetup current, IPeriodSetup prev)
		{
			if (prev != null)
				return (current.StartDate.Value.Year - prev.StartDate.Value.Year);
			return 0;
		}

		static int GetYearDelta(IPeriodSetup current)
		{
			return (current.EndDate.Value.Year - current.StartDate.Value.Year);
		}

		static bool IsEqual(IPeriodSetup op1, TPeriod op2)
		{
			if (op1 == null || op2 == null) return (op1 == null && op2 == null);
			return op1.PeriodNbr == op2.PeriodNbr;
		}

		public virtual TPeriod createNextPeriod(TPeriod current)
		{
			TPeriod result = new TPeriod();
			if (!this.fillNextPeriod(result, current)) return null;
			return result;
		}


		/// <summary>
		/// This function fills result based on aSetup (as template) and current year
		/// Start and End Date of result are adjusted using the following alorithm only month/date parts are used, aYear is used as year ;
		/// If there is year break inside aSetup period, Dates are adjusted accordingly
		/// </summary>
		/// <param name="result">Result of operation (must be not null)</param>
		/// <param name="aSetup">Setup - used as template. ReadOnly.</param>
		/// <param name="aYear">Year</param>
		static void CopyFrom(TPeriod result, IPeriodSetup aSetup, int aYear, PXGraph graph)
		{
			result.PeriodNbr = aSetup.PeriodNbr;
			int newYear = aYear + GetYearDelta(aSetup);
			result.StartDate = new DateTime(aYear, aSetup.StartDate.Value.Month, Math.Min(DateTime.DaysInMonth(aYear, aSetup.StartDate.Value.Month), aSetup.StartDate.Value.Day));
			result.EndDate = new DateTime(newYear, aSetup.EndDate.Value.Month, Math.Min(DateTime.DaysInMonth(newYear, aSetup.EndDate.Value.Month), aSetup.EndDate.Value.Day));
			result.Descr = aSetup.Descr;
			result.FinPeriodID = result.FinYear + result.PeriodNbr;
			if (graph != null && PXDBLocalizableStringAttribute.IsEnabled && aSetup is IBqlTable && result is IBqlTable)
			{
				PXCache scache = graph.Caches[aSetup.GetType()];
				PXCache dcache = graph.Caches[result.GetType()];
				string sfield = null;
				string dfield = null;
				foreach (PXEventSubscriberAttribute attr in scache.GetAttributesReadonly(null))
				{
					if (attr is PXDBLocalizableStringAttribute)
					{
						sfield = attr.FieldName;
						break;
					}
				}
				foreach (PXEventSubscriberAttribute attr in dcache.GetAttributesReadonly(null))
				{
					if (attr is PXDBLocalizableStringAttribute)
					{
						dfield = attr.FieldName;
						break;
					}
				}
				if (!String.IsNullOrEmpty(sfield) && !String.IsNullOrEmpty(dfield))
				{
					string[] translations = scache.GetValueExt(aSetup, sfield + "Translations") as string[];
					if (translations != null)
					{
						dcache.SetValueExt(result, dfield + "Translations", translations);
					}
				}
			}
		}

		private int firstPeriodYear;
		private string finYear;
		private DateTime yearStartDate;
		private DateTime periodsStartDate;
		private IYearSetup yearSetup;
		private IEnumerable<TPeriodSetup> periodsSetup;
		private FiscalPeriodSetupCreator<TPeriod> _finPeriodCreator = null;

		private FiscalPeriodSetupCreator<TPeriod> FinPeriodCreator
		{
			get
			{
				if (this._finPeriodCreator == null)
				{
					DateTime yearStartingDate = this.yearStartDate;
					this._finPeriodCreator = new FiscalPeriodSetupCreator<TPeriod>(FiscalPeriodSetupCreator<TPeriod>.FormatYear(firstPeriodYear),
																this.periodsStartDate,
																yearSetup.FPType,
																yearSetup.HasAdjustmentPeriod ?? false,
																false,
																yearSetup.PeriodLength,
																yearStartingDate,
																yearSetup.EndYearCalcMethod,
																yearSetup.EndYearDayOfWeek.Value);

				}
				return this._finPeriodCreator;
			}
		}
	}

	[Serializable]
	public partial class FinPeriodSaveDialog : IBqlTable
	{
		#region Message
		[PXString()]
		[PXUIField(Enabled = false)]
		public virtual String Message { get; set; }
		#endregion
		#region Method
		public abstract class method : IBqlField
		{
			#region List
			public class ListAttribute : PXStringListAttribute
			{
				public ListAttribute()
					: base(
				new[] { UpdateNextYearStart, UpdateFinYearSetup, ExtendLastPeriod },
				new[] { Messages.FinPeriodUpdateNextYearStart, Messages.FinPeriodUpdateFinYearSetup, Messages.FinPeriodExtendLastPeriod }) { }
			}

			public const string UpdateNextYearStart = "N";
			public const string UpdateFinYearSetup = "Y";
			public const string ExtendLastPeriod = "E";
			public const string ShortenLastPeriod = "S";

			public class updateNextYearStart : Constant<string>
			{
				public updateNextYearStart() : base(UpdateNextYearStart) { }
			}
			public class updateFinYearSetup : Constant<string>
			{
				public updateFinYearSetup() : base(UpdateFinYearSetup) { }
			}
			public class extendLastPeriod : Constant<string>
			{
				public extendLastPeriod() : base(ExtendLastPeriod) { }
			}
			public class shortenLastPeriod : Constant<string>
			{
				public shortenLastPeriod() : base(ShortenLastPeriod) { }
			}
			#endregion
		}
		[PXDBString(1, IsFixed = true)]
		[PXDefault(method.UpdateFinYearSetup)]
		[method.List]
		[PXUIField(DisplayName = "Update Method")]
		public virtual String Method { get; set; }
		#endregion
		#region MoveDayOfWeek
		public abstract class moveDayOfWeek : PX.Data.IBqlField
		{
		}
		protected bool? _MoveDayOfWeek;
		[PXBool]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Move start day of financial period ", Enabled = true, Required = false, Visible = false)]
		public virtual bool? MoveDayOfWeek
		{
			get
			{
				return this._MoveDayOfWeek;
			}
			set
			{
				this._MoveDayOfWeek = value;
			}
		}
		#endregion
		#region MethodDescription
		[PXString()]
		[PXDefault(Messages.FiscalPeriodMethodModifyNextYear)]
		[PXUIField(Enabled = false, Required=false)]
		public virtual String MethodDescription { get; set; }
		#endregion
	}
}
