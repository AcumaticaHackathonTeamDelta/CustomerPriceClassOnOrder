using System;
using System.Collections;
using System.Collections.Generic;

using PX.Data;

using PX.Objects.Common;

namespace PX.Objects.GL
{
	public class ScheduleRunBase
	{
		public static void SetProcessDelegate<ProcessGraph>(PXGraph graph, ScheduleRun.Parameters filter, PXProcessing<Schedule> view)
			where ProcessGraph : PXGraph<ProcessGraph>, IScheduleProcessing, new()
		{
			if (filter == null) return;

			short times = ScheduleRunLimitType.RunMultipleTimes == filter.LimitTypeSel
				? filter.RunLimit ?? 1
				: short.MaxValue;

			DateTime date = ScheduleRunLimitType.RunTillDate == filter.LimitTypeSel
				? filter.EndDate ?? graph.Accessinfo.BusinessDate.Value
				: DateTime.MaxValue;

			view.SetProcessDelegate((ProcessGraph processGraph, Schedule schedule) =>
			{
				processGraph.Clear();
				processGraph.GenerateProc(schedule, times, date);
			});
		}
	}

	/// <summary>
	/// Encapsulates the common logic for generating transactions according to
	/// a schedule. Module-specific schedule running graphs derive from it.
	/// </summary>
	/// <typeparam name="TGraph">The specific graph type.</typeparam>
	/// <typeparam name="TMaintenanceGraph">The type of the graph used to create and edit schedules for relevant entity type.</typeparam>
	/// <typeparam name="TProcessGraph">The type of the graph used to run schedules for relevant entity type.</typeparam>
	public class ScheduleRunBase<TGraph, TMaintenanceGraph, TProcessGraph> : PXGraph<TGraph>
		where TGraph : PXGraph
		where TMaintenanceGraph: ScheduleMaintBase<TMaintenanceGraph, TProcessGraph>, new()
		where TProcessGraph : PXGraph<TProcessGraph>, IScheduleProcessing, new()
	{
		public PXFilter<ScheduleRun.Parameters> Filter;
		public PXCancel<ScheduleRun.Parameters> Cancel;

		[PXFilterable]
		public PXFilteredProcessing<
			Schedule,
			ScheduleRun.Parameters,
			Where<
				Schedule.active, Equal<True>,
				And2<Where<
					Current<ScheduleRun.Parameters.limitTypeSel>, Equal<ScheduleRunLimitType.runMultipleTimes>,
					Or<Schedule.nextRunDate, LessEqual<Current<ScheduleRun.Parameters.endDate>>>>,
				And<Where<
					Current<ScheduleRun.Parameters.startDate>, IsNull,
					Or<Schedule.nextRunDate, GreaterEqual<Current<ScheduleRun.Parameters.startDate>>>>>>>>
			Schedule_List;

		protected virtual IEnumerable schedule_List()
		{
			IEnumerable<Schedule> schedules = new PXView(this, false, Schedule_List.View.BqlSelect)
				.SelectMulti()
				.RowCast<Schedule>();

			// Checking for the presence of schedule details is delegated to
			// the concrete maintenance graph.
			// -
			ScheduleMaintBase<TMaintenanceGraph, TProcessGraph> maintenanceGraph =
				PXGraph.CreateInstance<TMaintenanceGraph>();

			foreach (Schedule schedule in schedules)
			{
				maintenanceGraph.Schedule_Header.Current = schedule;

				if (maintenanceGraph.AnyScheduleDetails())
				{
					yield return schedule;
				}
			}
		}

		public ScheduleRunBase()
		{
			Schedule_List.SetProcessCaption(Messages.ProcRunSelected);
			Schedule_List.SetProcessAllCaption(Messages.ProcRunAll);
		}

		protected IEnumerable ViewScheduleAction(PXAdapter adapter)
		{
			if (Schedule_List.Current == null) return adapter.Get();

			PXRedirectHelper.TryRedirect(
				Schedule_List.Cache,
				Schedule_List.Current,
				nameof(Schedule),
				PXRedirectHelper.WindowMode.NewWindow);

			return adapter.Get();
		}

		#region Cache Attached
		[PXDBString(10, IsUnicode = true, IsKey = true)]
		[PXUIField(DisplayName = Common.Messages.ScheduleID, Visibility = PXUIVisibility.Visible, Enabled = false)]
		protected virtual void Schedule_ScheduleID_CacheAttached(PXCache sender)
		{
		}
		#endregion

		#region Event Handlers
		protected virtual void Schedule_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			Schedule task = (Schedule)e.Row;

			if (task == null || PXLongOperation.Exists(UID)) return;

			if ((task.NoRunLimit ?? false) == false && task.RunCntr == task.RunLimit)
			{
				cache.RaiseExceptionHandling<Schedule.scheduleID>(e.Row, task.ScheduleID, new PXSetPropertyException(Messages.SheduleExecutionLimitExceeded, PXErrorLevel.RowError));
			}
			else if ((task.NoEndDate ?? false) == false && task.EndDate < Accessinfo.BusinessDate)
			{
				cache.RaiseExceptionHandling<Schedule.scheduleID>(e.Row, task.ScheduleID, new PXSetPropertyException(Messages.SheduleHasExpired, PXErrorLevel.RowError));
			}
			else if (task.NextRunDate > Accessinfo.BusinessDate)
			{
				cache.RaiseExceptionHandling<Schedule.scheduleID>(e.Row, task.ScheduleID, new PXSetPropertyException(Messages.SheduleNextExecutionDateExceeded, PXErrorLevel.RowWarning));
			}
		}

		protected virtual void Parameters_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			ScheduleRunBase.SetProcessDelegate<TProcessGraph>(
				this,
				(ScheduleRun.Parameters)e.Row,
				Schedule_List);
		}

		protected virtual void Parameters_StartDate_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			ScheduleRun.Parameters filterData = (ScheduleRun.Parameters)e.Row;

			if (filterData.EndDate != null && filterData.StartDate > filterData.EndDate)
			{
				filterData.EndDate = filterData.StartDate;
			}
		}
		#endregion
	}
}
