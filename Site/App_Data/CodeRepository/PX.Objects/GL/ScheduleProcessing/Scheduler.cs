using System;
using System.Collections.Generic;
using System.Linq;

using PX.Data;
using PX.Objects.CS;
using PX.Objects.DR.Descriptor;

namespace PX.Objects.GL
{
	/// <summary>
	/// A specialized utility class responsible for generation
	/// of schedules given the schedule parameters.
	/// </summary>
	public class Scheduler
	{
		private PXGraph _graph;
		private IFinancialPeriodProvider _financialPeriodProvider;

		public Scheduler(PXGraph graph, IFinancialPeriodProvider financialPeriodProvider = null)
		{
			_graph = graph;
			_financialPeriodProvider = financialPeriodProvider ?? FinancialPeriodProvider.Default;
		}

		public virtual IEnumerable<ScheduleDet> MakeSchedule(
			Schedule scheduleParameters, 
			short numberOccurrences, 
			DateTime? runDate = null)
		{
			runDate = runDate ?? _graph.Accessinfo.BusinessDate;

			List<ScheduleDet> scheduleOccurrences = new List<ScheduleDet>();

			if (scheduleParameters.NextRunDate == null && scheduleParameters.LastRunDate == null)
			{
				scheduleParameters.NextRunDate = scheduleParameters.StartDate;
			}
			else if (scheduleParameters.NextRunDate == null)
			{
				scheduleParameters.NextRunDate = scheduleParameters.LastRunDate;
			}

			int occurrencesCreated = 0;
			short oldRunCounter = scheduleParameters.RunCntr ?? 0;

			do
			{
				ScheduleDet occurrence = new ScheduleDet();
				do
				{
					switch (scheduleParameters.ScheduleType)
					{
						case GLScheduleType.Daily:
							occurrence.ScheduledDate = scheduleParameters.NextRunDate;
							scheduleParameters.NextRunDate = ((DateTime)scheduleParameters.NextRunDate).AddDays((short)scheduleParameters.DailyFrequency);
							break;
						case GLScheduleType.Weekly:
							CalcRunDatesWeekly(scheduleParameters, occurrence);
							break;
						case GLScheduleType.Periodically:
							try
							{
								switch (scheduleParameters.PeriodDateSel)
								{
									case PeriodDateSelOption.PeriodStart:
										occurrence.ScheduledDate = _financialPeriodProvider.PeriodStartDate(_graph, _financialPeriodProvider.PeriodFromDate(_graph, scheduleParameters.NextRunDate));
										scheduleParameters.NextRunDate = _financialPeriodProvider.PeriodStartDate(_graph, _financialPeriodProvider.PeriodPlusPeriod(_graph, _financialPeriodProvider.PeriodFromDate(_graph, scheduleParameters.NextRunDate), (short)scheduleParameters.PeriodFrequency));
										break;
									case PeriodDateSelOption.PeriodEnd:
										occurrence.ScheduledDate = _financialPeriodProvider.PeriodEndDate(_graph, _financialPeriodProvider.PeriodFromDate(_graph, scheduleParameters.NextRunDate));
										scheduleParameters.NextRunDate = _financialPeriodProvider.PeriodEndDate(_graph, _financialPeriodProvider.PeriodPlusPeriod(_graph, _financialPeriodProvider.PeriodFromDate(_graph, scheduleParameters.NextRunDate), (short)scheduleParameters.PeriodFrequency));
										break;
									case PeriodDateSelOption.PeriodFixedDate:
										occurrence.ScheduledDate = _financialPeriodProvider.PeriodStartDate(_graph, _financialPeriodProvider.PeriodFromDate(_graph, scheduleParameters.NextRunDate)).AddDays((short)scheduleParameters.PeriodFixedDay - 1);
										if (((DateTime)occurrence.ScheduledDate).CompareTo(_financialPeriodProvider.PeriodEndDate(_graph, _financialPeriodProvider.PeriodFromDate(_graph, scheduleParameters.NextRunDate))) > 0)
										{
											occurrence.ScheduledDate = _financialPeriodProvider.PeriodEndDate(_graph, _financialPeriodProvider.PeriodFromDate(_graph, scheduleParameters.NextRunDate));
										}

										DateTime? OldNextRunDate = scheduleParameters.NextRunDate;
										scheduleParameters.NextRunDate = _financialPeriodProvider.PeriodStartDate(_graph, _financialPeriodProvider.PeriodPlusPeriod(_graph, _financialPeriodProvider.PeriodFromDate(_graph, scheduleParameters.NextRunDate), (short)scheduleParameters.PeriodFrequency)).AddDays((short)scheduleParameters.PeriodFixedDay - 1);
										if (((DateTime)scheduleParameters.NextRunDate).CompareTo(_financialPeriodProvider.PeriodEndDate(_graph, _financialPeriodProvider.PeriodPlusPeriod(_graph, _financialPeriodProvider.PeriodFromDate(_graph, OldNextRunDate), (short)scheduleParameters.PeriodFrequency))) > 0)
										{
											scheduleParameters.NextRunDate = _financialPeriodProvider.PeriodEndDate(_graph, _financialPeriodProvider.PeriodPlusPeriod(_graph, _financialPeriodProvider.PeriodFromDate(_graph, OldNextRunDate), (short)scheduleParameters.PeriodFrequency));
										}
										break;
								}
							}
							catch (PXFinPeriodException financialPeriodException)
							{
								if (occurrence.ScheduledDate != null 
									&& (bool)scheduleParameters.NoRunLimit == false 
									&& scheduleParameters.RunCntr + 1 == scheduleParameters.RunLimit
									&& scheduleParameters.NextRunDate != null)
								{
									scheduleParameters.NextRunDate = null;
								}
								else
								{
									scheduleParameters.RunCntr = oldRunCounter;
									throw new PXFinPeriodException(
										financialPeriodException,
										Messages.NoFinancialPeriodForNextExecutionDate);
								}
							}
							break;
						case GLScheduleType.Monthly:
							switch (scheduleParameters.MonthlyDaySel)
							{
								case "D":
									occurrence.ScheduledDate = new PXDateTime((short)((DateTime)scheduleParameters.NextRunDate).Year, (short)((DateTime)scheduleParameters.NextRunDate).Month, (short)scheduleParameters.MonthlyOnDay);
									scheduleParameters.NextRunDate = PXDateTime.DatePlusMonthSetDay((DateTime)occurrence.ScheduledDate, (short)scheduleParameters.MonthlyFrequency, (short)scheduleParameters.MonthlyOnDay);
									break;
								case "W":
									occurrence.ScheduledDate = PXDateTime.MakeDayOfWeek((short)((DateTime)scheduleParameters.NextRunDate).Year, (short)((DateTime)scheduleParameters.NextRunDate).Month, (short)scheduleParameters.MonthlyOnWeek, (short)scheduleParameters.MonthlyOnDayOfWeek);

									scheduleParameters.NextRunDate = new PXDateTime(((DateTime)occurrence.ScheduledDate).Year, ((DateTime)occurrence.ScheduledDate).Month, 1).AddMonths((short)scheduleParameters.MonthlyFrequency);
									scheduleParameters.NextRunDate = PXDateTime.MakeDayOfWeek((short)((DateTime)scheduleParameters.NextRunDate).Year, (short)((DateTime)scheduleParameters.NextRunDate).Month, (short)scheduleParameters.MonthlyOnWeek, (short)scheduleParameters.MonthlyOnDayOfWeek);
									break;
							}
							break;
						default:
							throw new PXException();
					}
				} while (occurrence.ScheduledDate == scheduleParameters.LastRunDate);

				if (occurrence.ScheduledDate != null
					&& ((DateTime)occurrence.ScheduledDate).CompareTo(scheduleParameters.StartDate) >= 0
					&& ((bool)scheduleParameters.NoEndDate || ((DateTime)occurrence.ScheduledDate).CompareTo(scheduleParameters.EndDate) <= 0)
					&& (((DateTime)occurrence.ScheduledDate).CompareTo(runDate) <= 0)
					&& ((bool)scheduleParameters.NoRunLimit || scheduleParameters.RunCntr < scheduleParameters.RunLimit))
				{
					try
					{
						occurrence.ScheduledPeriod = _financialPeriodProvider.PeriodFromDate(_graph, occurrence.ScheduledDate);
					}
					catch (PXFinPeriodException financialPeriodException)
					{
						scheduleParameters.RunCntr = oldRunCounter;
						throw new PXFinPeriodException(
							financialPeriodException,
							Messages.NoFinancialPeriodForNextExecutionDate);
					}

					scheduleOccurrences.Add(occurrence);
					scheduleParameters.RunCntr++;

					if ((bool)scheduleParameters.NoRunLimit == false && scheduleParameters.RunCntr >= scheduleParameters.RunLimit
						|| (scheduleParameters.NextRunDate != null) && (bool)scheduleParameters.NoEndDate == false && ((DateTime)scheduleParameters.NextRunDate).CompareTo(scheduleParameters.EndDate) > 0)
					{
						scheduleParameters.Active = false;
						scheduleParameters.NextRunDate = null;
					}

					if (scheduleParameters.NextRunDate == null)
					{
						break;
					}

					if (++occurrencesCreated >= numberOccurrences)
					{
						return scheduleOccurrences;
					}
				}
				else
				{
					if (((DateTime)occurrence.ScheduledDate).CompareTo(scheduleParameters.StartDate) < 0)
					{
						continue;
					}
					if (occurrence.ScheduledDate != null)
					{
						scheduleParameters.NextRunDate = occurrence.ScheduledDate;
					}
					break;
				}
			} while (true);

			return scheduleOccurrences;
		}

		private static void CalcRunDatesWeekly(Schedule schedule, ScheduleDet occurrence)
		{
			DateTime? nextRunDateL = schedule.NextRunDate;
			DateTime? iteratorDate = schedule.NextRunDate;
			DateTime? scheduledDateL = null;

			do
			{
				DayOfWeek dayOfWeek = ((DateTime)iteratorDate).DayOfWeek;

				if (schedule.WeeklyOnDay1 == true && dayOfWeek == DayOfWeek.Sunday
					|| schedule.WeeklyOnDay2 == true && dayOfWeek == DayOfWeek.Monday
					|| schedule.WeeklyOnDay3 == true && dayOfWeek == DayOfWeek.Tuesday
					|| schedule.WeeklyOnDay4 == true && dayOfWeek == DayOfWeek.Wednesday
					|| schedule.WeeklyOnDay5 == true && dayOfWeek == DayOfWeek.Thursday
					|| schedule.WeeklyOnDay6 == true && dayOfWeek == DayOfWeek.Friday)
				{
					if (scheduledDateL == null)
					{
						scheduledDateL = iteratorDate;
					}
					else
					{
						nextRunDateL = iteratorDate;
					}
				}
				else if (dayOfWeek == DayOfWeek.Saturday)
				{
					if (schedule.WeeklyOnDay7 == true)
					{
						if (scheduledDateL == null)
						{
							scheduledDateL = iteratorDate;
						}
						else
						{
							nextRunDateL = iteratorDate;
						}
					}
					if (nextRunDateL == schedule.NextRunDate)
					{
						iteratorDate = ((DateTime)iteratorDate).AddDays(7 * ((short)schedule.WeeklyFrequency - 1) + 1);
						continue;
					}
				}
				iteratorDate = ((DateTime)iteratorDate).AddDays(1);
			}
			while (nextRunDateL == schedule.NextRunDate);

			schedule.NextRunDate = nextRunDateL;
			occurrence.ScheduledDate = scheduledDateL;
		}

		public DateTime? GetNextRunDate(Schedule schedule)
		{
            if (schedule.Active == false)
            {
                return null;
            }
            
            Schedule copy = PXCache<Schedule>.CreateCopy(schedule);

            if (copy.LastRunDate != null && !(copy.Active == true && copy.NextRunDate == null))
            {
                return copy.NextRunDate;
            }

			copy.NextRunDate = null;

			try
			{
				DateTime? businessDate = _graph.Accessinfo.BusinessDate;

				_graph.Accessinfo.BusinessDate = copy.NoEndDate == true 
					? DateTime.MaxValue
					: copy.EndDate;

				try
				{
					IEnumerable<ScheduleDet> occurrences = new Scheduler(_graph).MakeSchedule(copy, 1);

					if (occurrences.Any())
					{
						return occurrences.First().ScheduledDate;
					}
				}
				finally
				{
					_graph.Accessinfo.BusinessDate = businessDate;
				}
			}
			catch (PXFinPeriodException)
			{
                if (schedule.NextRunDate != null)
                {
                    return schedule.NextRunDate;
                }
                else
                {
                    throw;
                }
			}

			return copy.NextRunDate;
		}
	}
}
