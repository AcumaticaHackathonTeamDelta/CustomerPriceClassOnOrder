using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using PX.Common;
using PX.CS;
using PX.Data.EP;
using PX.Objects.CR;
using PX.Data;
using System;
using System.Collections;
using PX.Objects.GL;
using PX.Objects.PM;

namespace PX.Objects.EP
{
	public class CRActivityMaint : CRBaseActivityMaint<CRActivityMaint, CRActivity>
	{
		#region Constants

		private static readonly EPSetup EmptyEpSetup = new EPSetup();

		#endregion

		#region Selects

		[PXHidden]
		public PXSelect<CT.Contract>
			BaseContract;

		[PXRefNoteSelector(typeof(CRActivity), typeof(CRActivity.refNoteID))]
		[PXCopyPasteHiddenFields(typeof(CRActivity.body))]
		public PXSelect<CRActivity,
			Where<CRActivity.classID, Equal<CRActivityClass.activity>>>
			Activities;
                
        [PXHidden]
        [PXCopyPasteHiddenView]
        public PXSelect<CRActivity,
            Where<CRActivity.noteID, Equal<Current<CRActivity.noteID>>>>
            CurrentActivity;

        public PXSelect<PMTimeActivity,
			Where<PMTimeActivity.refNoteID, Equal<Current<CRActivity.noteID>>>>
			TimeActivitiesOld;

		public PMTimeActivityList<CRActivity>
			TimeActivity;

		#endregion

		#region Ctors

		public CRActivityMaint()
		{
			CRCaseActivityHelper.Attach(this);

			PXView relEntityView = new PXView(this, true, new Select<CRSMEmail>(), new PXSelectDelegate(GetRelatedEntity));
			Views.Add("RelatedEntity", relEntityView);            
		}

		#endregion

		#region Actions

		public PXDelete<CRActivity> Delete;

		public PXAction<CRActivity> MarkAsCompleted;
		[PXUIField(DisplayName = Messages.Complete)]
		[PXButton(Tooltip = Messages.MarkAsCompletedTooltip,
			ShortcutCtrl = true, ShortcutChar = (char)75)] //Ctrl + K
		public virtual void markAsCompleted()
		{
			CRActivity row = Activities.Current;
			if (row == null) return;

			CompleteActivity(row);
		}

		public PXAction<CRActivity> MarkAsCompletedAndFollowUp;

		[PXUIField(DisplayName = Messages.CompleteAndFollowUp)]
		[PXButton(Tooltip = Messages.CompleteAndFollowUpTooltip,
			ShortcutCtrl = true, ShortcutShift = true, ShortcutChar = (char)75)] //Ctrl + Shift + K
		public virtual void markAsCompletedAndFollowUp()
		{
			CRActivity row = Activities.Current;
			if (row == null) return;

			CompleteActivity(row);

			CRActivityMaint graph = CreateInstance<CRActivityMaint>();

			CRActivity followUpActivity = (CRActivity)graph.Activities.Cache.CreateCopy(row);
			followUpActivity.NoteID = null;
			followUpActivity.ParentNoteID = row.ParentNoteID;
			followUpActivity.UIStatus = null;
			followUpActivity.PercentCompletion = null;

			if (followUpActivity.StartDate != null)
			{
				followUpActivity.StartDate = ((DateTime)followUpActivity.StartDate).AddDays(1D);
			}
			if (followUpActivity.EndDate != null)
				followUpActivity.EndDate = ((DateTime)followUpActivity.EndDate).AddDays(1D);

			graph.Activities.Insert(followUpActivity);

			PXRedirectHelper.TryRedirect(graph, PXRedirectHelper.WindowMode.NewWindow);
		}

		#endregion

		#region Data Handlers

		public IEnumerable GetRelatedEntity()
		{
			var current = Activities.Current;
			if (current != null && current.RefNoteID != null)
			{
				var row = new EntityHelper(this).GetEntityRow(current.RefNoteID);
				if (row != null) yield return row;
			}
		}

		#endregion

		#region Event Handlers

		[PXUIField(DisplayName = "Task")]
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		protected virtual void CRActivity_ParentNoteID_CacheAttached(PXCache cache) { }

		protected virtual void CRActivity_RowInserted(PXCache cache, PXRowInsertedEventArgs e)
		{
			//TODO redesign by task #32833
			var row = (CRActivity)e.Row;
			if (row == null) return;
			row.ClassID = CRActivityClass.Activity;
		}

		protected virtual void CRActivity_RowUpdated(PXCache cache, PXRowUpdatedEventArgs e)
		{
			var row = (CRActivity)e.Row;
			var oldRow = (CRActivity)e.OldRow;
			if (row == null || oldRow == null) return;
			row.ClassID = CRActivityClass.Activity;
		}

		protected virtual void CRActivity_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			CRActivity row = (CRActivity)e.Row;
			if (row == null) return;

			var tAct = (PMTimeActivity)TimeActivity.SelectSingle();
			var tActCache = TimeActivity.Cache;

			PXUIFieldAttribute.SetEnabled<CRActivity.endDate>(cache, row, false);

			bool wasUsed = !string.IsNullOrEmpty(tAct?.TimeCardCD) || tAct?.Billed == true;
			if (wasUsed)
				PXUIFieldAttribute.SetEnabled(cache, row, false);

			bool isPmVisible = ProjectAttribute.IsPMVisible(this, BatchModule.EP);
			PXUIFieldAttribute.SetVisible<PMTimeActivity.projectID>(tActCache, tAct, isPmVisible);
			PXUIFieldAttribute.SetVisible<PMTimeActivity.projectTaskID>(tActCache, tAct, isPmVisible);
			PXUIFieldAttribute.SetVisible<PMTimeActivity.timeSpent>(tActCache, tAct, tAct?.TrackTime == true);
			PXUIFieldAttribute.SetVisible<PMTimeActivity.approvalStatus>(tActCache, tAct, tAct?.TrackTime == true);
			PXUIFieldAttribute.SetVisible<PMTimeActivity.earningTypeID>(tActCache, tAct, tAct?.TrackTime == true);
			PXUIFieldAttribute.SetVisible<PMTimeActivity.isBillable>(tActCache, tAct, tAct?.TrackTime == true);
			PXUIFieldAttribute.SetVisible<PMTimeActivity.released>(tActCache, tAct, tAct?.TrackTime == true);
			PXUIFieldAttribute.SetVisible<PMTimeActivity.timeBillable>(tActCache, tAct, tAct?.IsBillable == true && tAct?.TrackTime == true);
			PXUIFieldAttribute.SetVisible<PMTimeActivity.arRefNbr>(tActCache, tAct, tAct?.IsBillable == true && tAct?.TrackTime == true);
			PXUIFieldAttribute.SetVisible<PMTimeActivity.approverID>(tActCache, tAct, tAct?.TrackTime == true);
			PXUIFieldAttribute.SetVisible<PMTimeActivity.projectID>(tActCache, tAct, tAct?.TrackTime == true);
			PXUIFieldAttribute.SetVisible<PMTimeActivity.projectTaskID>(tActCache, tAct, tAct?.TrackTime == true);
			PXUIFieldAttribute.SetVisible<PMTimeActivity.overtimeSpent>(tActCache, tAct, false);
			PXUIFieldAttribute.SetVisible<PMTimeActivity.overtimeSpent>(tActCache, tAct, false);
			PXUIFieldAttribute.SetVisible<PMTimeActivity.overtimeBillable>(tActCache, tAct, false);

			bool showMinutes = EPSetupCurrent.RequireTimes == true;
			PXDBDateAndTimeAttribute.SetTimeEnabled<CRActivity.startDate>(cache, row, showMinutes && tAct?.TrackTime == true);
			PXDBDateAndTimeAttribute.SetTimeVisible<CRActivity.startDate>(cache, row, showMinutes && tAct?.TrackTime == true);
			PXDBDateAndTimeAttribute.SetTimeVisible<CRActivity.endDate>(cache, row, showMinutes && tAct?.TrackTime == true);			

			string origStatus = 
				(string)this.Activities.Cache.GetValueOriginal<CRActivity.uistatus>(row) 
				?? ActivityStatusListAttribute.Open;

			string origTimeStatus =
				(string)this.TimeActivity.Cache.GetValueOriginal<PMTimeActivity.approvalStatus>(tAct)
				?? ActivityStatusListAttribute.Open;

			bool? origTrackTime =
				(bool?)this.TimeActivity.Cache.GetValueOriginal<PMTimeActivity.trackTime>(tAct) 
				?? false;

			if (origStatus == ActivityStatusListAttribute.Completed && origTrackTime != true)
				origStatus = ActivityStatusListAttribute.Open;

			if (row.IsLocked == true)
				origStatus = ActivityStatusListAttribute.Completed;

			if (origStatus == ActivityStatusListAttribute.Open)
			{
				PXUIFieldAttribute.SetEnabled(cache, row, true);
				Delete.SetEnabled(!wasUsed);
			}
			else
			{
				PXUIFieldAttribute.SetEnabled(cache, row, false);								
				Delete.SetEnabled(false);
			}

			PXUIFieldAttribute.SetEnabled<CRActivity.source>(cache, row, false);
			PXUIFieldAttribute.SetEnabled<CRActivity.noteID>(cache, row);

			MarkAsCompleted.SetEnabled(origStatus == ActivityStatusListAttribute.Open);
			MarkAsCompleted.SetVisible(origStatus == ActivityStatusListAttribute.Open && tAct?.TrackTime == true);
			MarkAsCompletedAndFollowUp.SetVisible(false);

			GotoParentActivity.SetEnabled(row.ParentNoteID != null);

			// TimeActivity

			if (tAct?.Released == true)
				origTimeStatus = ActivityStatusAttribute.Completed;

			if (origTimeStatus == ActivityStatusListAttribute.Open)
			{
				PXUIFieldAttribute.SetEnabled(tActCache, tAct, true);

				PXUIFieldAttribute.SetEnabled<PMTimeActivity.timeBillable>(tActCache, tAct, !wasUsed && tAct?.IsBillable == true);
				PXUIFieldAttribute.SetEnabled<PMTimeActivity.overtimeBillable>(tActCache, tAct, !wasUsed && tAct?.IsBillable == true);
			}
			else
			{
				PXUIFieldAttribute.SetEnabled(tActCache, tAct, false);
			}
			PXUIFieldAttribute.SetEnabled<PMTimeActivity.approvalStatus>(tActCache, tAct, tAct != null && tAct.TrackTime == true && !wasUsed);

			PXUIFieldAttribute.SetEnabled<PMTimeActivity.released>(tActCache, tAct, false);

			PXDefaultAttribute.SetPersistingCheck<CRActivity.ownerID>(cache, row, tAct?.TrackTime == true ? PXPersistingCheck.Null : PXPersistingCheck.Nothing);
			PXDefaultAttribute.SetPersistingCheck<CRActivity.type>(cache, row, PXPersistingCheck.Null);
		}

		protected virtual void CRActivity_RefNoteID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			CRActivity activity = (CRActivity)e.Row;
			if (activity == null) return;

			if (Caches[typeof(RelatedEntity)].Current != null)
			{
				var related = Views["RelatedEntity"].SelectSingle();
				if (related == null) return;

				var relatedType = related.GetType();

				if (typeof(Contact).IsAssignableFrom(relatedType))
				{
					Contact contact = related as Contact;
					activity.ContactID = contact?.ContactID;
					activity.BAccountID = contact?.BAccountID;
				}
				else if (typeof(BAccount).IsAssignableFrom(relatedType))
				{
					BAccount contact = related as BAccount;
					activity.ContactID = null;
					activity.BAccountID = contact?.BAccountID;
				}
			}
		}

		#endregion

		#region Public Methods

		public override void CompleteRow(CRActivity row)
		{
			CompleteActivity(row);
		}

		public static TimeSpan CalculateOvertime(PXGraph graph, PMTimeActivity act, DateTime start, DateTime end)
		{
			var calendarId = GetCalendarID(graph, act);
			return calendarId == null ? new TimeSpan() : CalendarHelper.CalculateOvertime(graph, start, end, calendarId);
		}

		public static string GetCalendarID(PXGraph graph, PMTimeActivity act)
		{
			var projectCalendar = act.ProjectID.
				With(_ => (CT.Contract)PXSelect<CT.Contract,
					Where<CT.Contract.contractID, Equal<Required<CT.Contract.contractID>>>>.
				Select(graph, _.Value)).
				With(_ => _.CalendarID);
			if (projectCalendar != null) return projectCalendar;

			var projectTaskCalendar = act.ProjectTaskID.
				With(_ => (PXResult<Location, PM.PMTask>)PXSelectJoin<Location,
					InnerJoin<PM.PMTask, On<PM.PMTask.customerID, Equal<Location.bAccountID>, And<PM.PMTask.locationID, Equal<Location.locationID>>>>,
					Where<PM.PMTask.taskID, Equal<Required<PM.PMTask.taskID>>>>.
				Select(graph, _.Value)).
				With(_ => ((Location)_).CCalendarID);
			if (projectTaskCalendar != null) return projectTaskCalendar;

			var caseLocationCalendar = act.RefNoteID.
				With(_ => (PXResult<Location, CRCase>)PXSelectJoin<Location,
					InnerJoin<CRCase, 
						On<CRCase.customerID, Equal<Location.bAccountID>, 
						And<CRCase.locationID, Equal<Location.locationID>>>, 
					InnerJoin<CRActivityLink,
						On<CRActivityLink.refNoteID, Equal<CRCase.noteID>>>>,
					Where<CRActivityLink.noteID, Equal<Required<PMTimeActivity.refNoteID>>>>.
				Select(graph, _.Value)).
				With(_ => ((Location)_).CCalendarID);
			if (caseLocationCalendar != null) return caseLocationCalendar;

			var employeeCalendar = act.OwnerID.
				With(_ => (EPEmployee)PXSelect<EPEmployee,
					Where<EPEmployee.userID, Equal<Required<EPEmployee.userID>>>>.
				Select(graph, _.Value)).
				With(_ => _.CalendarID);
			if (employeeCalendar != null) return employeeCalendar;

			return null;
		}

		public static DateTime? GetNextActivityStartDate<Activity>(PXGraph graph, PXResultset<Activity> res, PMTimeActivity row, int? fromWeekId, int? tillWeekId, PXCache tempDataCache, Type tempDataField)
			where Activity : PMTimeActivity, new()
		{
			DateTime? date;
			if (fromWeekId != null || tillWeekId != null)
				date = PXWeekSelector2Attribute.GetWeekStartDate(graph, (int)(fromWeekId ?? tillWeekId));
			else
				date = graph.Accessinfo.BusinessDate.GetValueOrDefault(DateTime.Now).Date;

			EPEmployee employee = PXSelect<EPEmployee, Where<EPEmployee.userID, Equal<Required<EPEmployee.userID>>>>.Select(graph, row.OwnerID);
			EPEmployeeClass employeeClass = PXSelect<EPEmployeeClass, Where<EPEmployeeClass.vendorClassID, Equal<Required<EPEmployee.vendorClassID>>>>.Select(graph, employee != null ? employee.VendorClassID : null);
			var calendarId = CRActivityMaint.GetCalendarID(graph, row);

			if (employeeClass != null && EPEmployeeClass.defaultDateInActivity.LastDay == employeeClass.DefaultDateInActivity)
			{
				DateTime? val = tempDataCache.GetValue(tempDataCache.Current, tempDataField.Name) as DateTime?;
				if (val != null)
				{
					int week = PXWeekSelector2Attribute.GetWeekID(graph, (DateTime)val);
					if ((fromWeekId == null || week >= fromWeekId) && (tillWeekId == null || tillWeekId >= week))
						date = val;
				}
			}
			else
			{
				DateTime weekDate = (DateTime)date;
				DateTime? newDate = null;
				date = res != null && res.Count > 0 ? res.Max(_ => ((Activity)_).Date) : null ?? date;
				for (int curentWeek = PXWeekSelector2Attribute.GetWeekID(graph, weekDate); tillWeekId == null || curentWeek <= tillWeekId; curentWeek = PXWeekSelector2Attribute.GetWeekID(graph, weekDate))
				{
					PXWeekSelector2Attribute.WeekInfo week1 = PXWeekSelector2Attribute.GetWeekInfo(graph,
						PXWeekSelector2Attribute.GetWeekID(graph, weekDate));
					foreach (KeyValuePair<DayOfWeek, PXWeekSelector2Attribute.DayInfo> pair in week1.Days.OrderBy(_ => _.Value.Date))
					{
						if (pair.Value.Date >= date &&
							(CalendarHelper.IsWorkDay(graph, calendarId, (DateTime)pair.Value.Date) ||
							 string.IsNullOrEmpty(calendarId) && pair.Key != DayOfWeek.Saturday && pair.Key != DayOfWeek.Sunday))
						{
							newDate = (DateTime)pair.Value.Date;
							break;
						}
						weekDate = weekDate.AddDays(1D);
					}
					if (newDate != null)
					{
						date = ((DateTime)newDate).Date;
						break;
					}
				}
			}

			if (!string.IsNullOrEmpty(calendarId) && date != null)
			{
				DateTime startDate;
				DateTime endDate;
				CalendarHelper.CalculateStartEndTime(graph, calendarId, (DateTime)date, out startDate, out endDate);
				date = startDate;
			}

			return date;
		}

		[Serializable]
		[PXHidden]
		public class EPTempData : IBqlTable
		{
			#region LastEnteredDate
			public abstract class lastEnteredDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _LastEnteredDate;
			[PXDate]
			public virtual DateTime? LastEnteredDate
			{
				get
				{
					return this._LastEnteredDate;
				}
				set
				{
					this._LastEnteredDate = value;
				}
			}
			#endregion

		}

		#endregion

		#region Private Methods

		private void CompleteActivity(CRActivity activity)
		{
			string origStatus = (string)this.Activities.Cache.GetValueOriginal<CRActivity.uistatus>(activity) ?? ActivityStatusListAttribute.Open;

			if (activity == null ||
				origStatus == ActivityStatusListAttribute.Completed ||
				origStatus == ActivityStatusListAttribute.Canceled)
			{
				return;
			}

			activity.UIStatus = ActivityStatusListAttribute.Completed;
			Activities.Cache.Update(activity);

			var timeAct = TimeActivity.Current;

			if (timeAct != null)
			{
				timeAct.ApprovalStatus = ActivityStatusListAttribute.Completed;
				TimeActivity.Cache.Update(timeAct);
			}

			Actions.PressSave();
		}

		private EPSetup EPSetupCurrent
		{
			get
			{
				var res = (EPSetup)PXSelect<EPSetup>.
					SelectWindowed(this, 0, 1);
				return res ?? EmptyEpSetup;
			}
		}


		#endregion
	}
}
