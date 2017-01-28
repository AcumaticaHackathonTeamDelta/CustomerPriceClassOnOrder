using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using PX.Common;
using PX.Common.Collection;
using PX.Common.Mail;
using PX.Data;
using PX.Data.EP;
using PX.Data.MassProcess;
using PX.Data.Reports;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.Common.Extensions;
using PX.Objects.CR.MassProcess;
using PX.Objects.EP;
using PX.Objects.PM;
using PX.SM;
using PX.TM;
using PX.Objects.CS;
using PX.Objects.SO;
using ActivityService = PX.Data.EP.ActivityService;
using PX.Reports;
using PX.Reports.Data;

namespace PX.Objects.CR
{
	#region ActivityContactFilter

	[Serializable]
	[PXHidden]
	public partial class ActivityContactFilter : IBqlTable
	{
		#region ContactID

		public abstract class contactID : IBqlField { }
		
		[PXInt]
		[PXUIField(DisplayName = "Select Contact")]
		[PXSelector(typeof(Search<Contact.contactID,
			Where<Contact.bAccountID, Equal<Current<BAccount.bAccountID>>,
			And<Contact.contactType, NotEqual<ContactTypesAttribute.bAccountProperty>>>>),
			DescriptionField = typeof(Contact.displayName),
			Filterable = true)]
		public virtual Int32? ContactID { get; set; }

		#endregion

		#region NoteID

		public abstract class noteID : IBqlField { }

		[PXGuid]
		public virtual Guid? NoteID { get; set; }

		#endregion
	}

	#endregion

	#region CRActivityList

	public abstract class CRActivityListBaseAttribute : PXViewExtensionAttribute
	{
		private readonly BqlCommand _command;

		private PXView _view;

		private string _hostViewName;
		private PXSelectBase _select;

		protected CRActivityListBaseAttribute() { }

		protected CRActivityListBaseAttribute(Type select)
		{
			if (select == null) throw new ArgumentNullException("select");

			if (typeof(IBqlSelect).IsAssignableFrom(select))
			{
				_command = BqlCommand.CreateInstance(select);
			}
			else
			{
				var error = string.Format("Incorrect select expression '{0}'", select.Name);
				throw new PXArgumentException("@select", error);
			}
		}

		public override void ViewCreated(PXGraph graph, string viewName)
		{
			Initialize(graph, viewName);

			AttachHandlers(graph);
		}

		private void Initialize(PXGraph graph, string viewName)
		{
			_hostViewName = viewName;
			_select = GetSelectView(graph);

			if (_command != null)
				_view = new PXView(graph, true, _command);
		}

		protected PXSelectBase GraphSelector
		{
			get { return _select; }
		}

		protected abstract void AttachHandlers(PXGraph graph);

		protected static void CorrectView(PXSelectBase select, BqlCommand command)
		{
			var graph = select.View.Graph;
			var newView = new PXView(graph, select.View.IsReadOnly, command);
			var oldView = select.View;
			select.View = newView;
			string viewName;
			if (graph.ViewNames.TryGetValue(oldView, out viewName))
			{
				graph.Views[viewName] = newView;
				graph.ViewNames.Remove(oldView);
				graph.ViewNames[newView] = viewName;
			}
		}

		protected object SelectRecord()
		{
			if (_view == null) 
				throw new InvalidOperationException(Messages.CommandNotSpecified);

			var dataRecord = _view.SelectSingle();
			if (dataRecord == null) return null;

			var res = dataRecord as PXResult;
			if (res == null) return dataRecord;

			return res[0];
		}

		private PXSelectBase GetSelectView(PXGraph graph)
		{
			var selectView = graph.GetType().GetField(_hostViewName).GetValue(graph);
			var selectViewType = selectView.GetType();
			Type typeDefinition = selectViewType;
			if (selectViewType.IsGenericType)
				typeDefinition = selectViewType.GetGenericTypeDefinition();
			if (!typeof(CRActivityList<>).IsAssignableFrom(typeDefinition))
			{
				var attributeTypeName = GetType().Name;
				var error = string.Format("Attribute '{0}' can only be used on '{1}' view or its childs.", 
					attributeTypeName, selectViewType.Name);
				throw new PXArgumentException(error);
			}
			return (PXSelectBase)selectView;
		}
	}
	
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class CRReferenceAttribute : PXViewExtensionAttribute
	{
		private readonly BqlCommand _bAccountCommand;
		private PXView _bAccountView;
		
		private string BAccountRefFieldName
		{
			get { return BAccountRefField != null ? BAccountRefField.Name : EntityHelper.GetIDField(_bAccountView.Cache.GetItemType()); }
		}

		public Type BAccountRefField { get; set; }



		private readonly BqlCommand _contactCommand;
		private PXView _contactView;

		private string ContactRefFieldName
		{
			get { return ContactRefField != null ? ContactRefField.Name : EntityHelper.GetIDField(_contactView.Cache.GetItemType()); }
		}

		public Type ContactRefField { get; set; }

		public bool Persistent { get; set; }

		public CRReferenceAttribute(Type bAccountSelect, Type contactSelect = null)
		{
			Persistent = false;

			if (bAccountSelect == null) throw new ArgumentNullException("bAccountSelect");

			if (typeof(IBqlSelect).IsAssignableFrom(bAccountSelect))
			{
				_bAccountCommand = BqlCommand.CreateInstance(bAccountSelect);

				if (contactSelect != null && typeof(IBqlSelect).IsAssignableFrom(contactSelect))
				{
					_contactCommand = BqlCommand.CreateInstance(contactSelect);
				}
			}
			else
			{
				string error = string.Format("Incorrect select expression '{0}'", bAccountSelect.Name);
				throw new PXArgumentException(error, "sel");
			}
		}

		public override void ViewCreated(PXGraph graph, string viewName)
		{
			_bAccountView = new PXView(graph, true, _bAccountCommand);
			
			graph.FieldDefaulting.AddHandler<CRActivity.bAccountID>(BAccountID_FieldDefaulting);
			graph.FieldDefaulting.AddHandler<CRPMTimeActivity.bAccountID>(BAccountID_FieldDefaulting);
			graph.FieldDefaulting.AddHandler<CRSMEmail.bAccountID>(BAccountID_FieldDefaulting);

			if (_contactCommand != null)
			{
				_contactView = new PXView(graph, true, _contactCommand);

				graph.FieldDefaulting.AddHandler<CRActivity.contactID>(ContactID_FieldDefaulting);
				graph.FieldDefaulting.AddHandler<CRPMTimeActivity.contactID>(ContactID_FieldDefaulting);
				graph.FieldDefaulting.AddHandler<CRSMEmail.contactID>(ContactID_FieldDefaulting);
			}

			if (Persistent)
			{
				graph.Views.Caches.Remove(typeof(CRActivity));
				graph.Views.Caches.Add(typeof(CRActivity));
				graph.RowPersisting.AddHandler(typeof(CRActivity), RowPersisting);

				graph.Views.Caches.Remove(typeof(CRPMTimeActivity));
				graph.Views.Caches.Add(typeof(CRPMTimeActivity));
				graph.RowPersisting.AddHandler(typeof(CRPMTimeActivity), RowPersisting);

				graph.Views.Caches.Remove(typeof(CRSMEmail));
				graph.Views.Caches.Add(typeof(CRSMEmail));
				graph.RowPersisting.AddHandler(typeof(CRSMEmail), RowPersisting);
			}
		}

		private int? GetBAccIDRef(PXCache sender)
		{
			object record = _bAccountView.SelectSingle();
			return GetBAccIDRef(sender, record);
		}

		private int? GetContactIDRef(PXCache sender)
		{
			object record = _contactView.SelectSingle();
			return GetBAccIDRef(sender, record);
		}

		private int? GetBAccIDRef(PXCache sender, object record)
		{
			return record != null ? (int?)sender.Graph.Caches[record.GetType()].GetValue(record, BAccountRefFieldName) : null;
		}

		private int? GetContactIDRef(PXCache sender, object record)
		{
			return record != null ? (int?)sender.Graph.Caches[record.GetType()].GetValue(record, ContactRefFieldName) : null;
		}

		private void BAccountID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			object record = _bAccountView.SelectSingle();
			if (record != null)
				e.NewValue = GetBAccIDRef(sender, record);
		}

		private void ContactID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			object record = _contactView.SelectSingle();
			if (record != null)
				e.NewValue = GetContactIDRef(sender, record);
		}

		private void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			sender.SetValue(e.Row, typeof(CRActivity.bAccountID).Name, GetBAccIDRef(_bAccountView.Cache));

			if (_contactView != null)
				sender.SetValue(e.Row, typeof(CRActivity.contactID).Name, GetContactIDRef(_contactView.Cache));
		}
	}

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class CRDefaultMailToAttribute : CRActivityListBaseAttribute
	{
		public interface IEmailMessageTarget
		{
			string DisplayName { get; }
			string Address { get; }
		}

		private readonly bool _takeCurrent;

		public CRDefaultMailToAttribute()
			: base()
		{
			_takeCurrent = true;
		}

		public CRDefaultMailToAttribute(Type select)
			: base(select)
		{
		}

		protected override void AttachHandlers(PXGraph graph)
		{
			graph.RowInserting.AddHandler<CRSMEmail>(RowInserting);
		}

		private void RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			var row = e.Row as CRSMEmail;
			if (row == null) return;

			string emailAddress = null;

			IEmailMessageTarget record;
			if (_takeCurrent)
			{
				var primaryDAC = ((IActivityList) GraphSelector).PrimaryViewType;
				var primaryCache = sender.Graph.Caches[primaryDAC];
				record = primaryCache.Current as IEmailMessageTarget;
			}
			else
			{
				record = SelectRecord() as IEmailMessageTarget;
			}

			if (record != null)
			{
				var displayName = record.DisplayName.With(_ => _.Trim());
				var address = record.Address.With(_ => _.Trim());
				if (!string.IsNullOrEmpty(address))
					emailAddress = string.IsNullOrEmpty(displayName) ? address : Mailbox.Create(displayName, address);
			}
			row.MailTo = emailAddress;
		}
	}
	
	public sealed class ProjectTaskActivities: CRActivityList<PMTask>
	{
		public ProjectTaskActivities(PXGraph graph)
			: base(graph)
		{
			_Graph.RowSelected.AddHandler<PMTask>(RowSelected);
			_Graph.RowInserting.AddHandler<CRPMTimeActivity>(RowInserting);
		}

		private void RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			PM.PMTask row = (PM.PMTask)e.Row;
			if (row == null || View == null || View.Cache == null)
				return;

			PMProject project = PXSelect<PMProject, Where<PMProject.contractID, Equal<Current<PMTask.projectID>>>>.Select(_Graph);
			bool userCanAddActivity = true;
			if (project != null && project.RestrictToEmployeeList == true)
			{
				var select = new PXSelectJoin<EPEmployeeContract,
					InnerJoin<EPEmployee, On<EPEmployee.bAccountID, Equal<EPEmployeeContract.employeeID>>>,
					Where<EPEmployeeContract.contractID, Equal<Current<PMTask.projectID>>,
					And<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>>(_Graph);

				EPEmployeeContract record = select.SelectSingle();
				userCanAddActivity = record != null;
			}

			View.Cache.AllowInsert = userCanAddActivity;
		}

		private void RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			var row = (CRPMTimeActivity) e.Row;
			if (row == null) return;

			row.ProjectID = ((PMTask) sender.Graph.Caches[typeof (PMTask)].Current).ProjectID;
			row.ProjectTaskID = ((PMTask)sender.Graph.Caches[typeof(PMTask)].Current).TaskID;
		}

		protected override void SetCommandCondition()
		{
			var command = ProjectTaskActivities.GenerateOriginalCommand();

			var where = typeof(Where<CRPMTimeActivity.projectTaskID, Equal<Current<PMTask.taskID>>>);
			
			command = command.WhereAnd(where);
			View = new PXView(View.Graph, View.IsReadOnly, command);
        }

        protected override void CreateTimeActivity(PXCache cache, int classId)
        {
            PXCache timeCache = cache.Graph.Caches[typeof(PMTimeActivity)];
            if (timeCache == null) return;

            PMTimeActivity timeActivity = (PMTimeActivity)timeCache.Current;
            if (timeActivity == null) return;

            bool withTimeTracking = classId != CRActivityClass.Task && classId != CRActivityClass.Event;
            
            timeCache.SetValue<PMTimeActivity.trackTime>(timeActivity, withTimeTracking);
            timeCache.SetValueExt<PMTimeActivity.projectID>(timeActivity, ((PMTask)_Graph.Caches[typeof(PMTask)].Current)?.ProjectID);
            timeCache.SetValueExt<PMTimeActivity.projectTaskID>(timeActivity, ((PMTask)_Graph.Caches[typeof(PMTask)].Current)?.TaskID);
        }

        protected override IEnumerable NewActivityByType(PXAdapter adapter, string type)
		{
			//DONE redesign by task #32833
			CreateActivity(CRActivityClass.Activity, type);
			return adapter.Get();
		}

		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.MailSend)]
		[PXShortCut(true, false, false, 'A', 'M')]
		public override IEnumerable NewMailActivity(PXAdapter adapter)
		{
			//DONE redesign by task #32833
			CreateActivity(CRActivityClass.Email, null);
			return adapter.Get();
		}

		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.Task)]
		[PXShortCut(true, false, false, 'K', 'C')]
		public override IEnumerable NewTask(PXAdapter adapter)
		{
			//DONE redesign by task #32833
			CreateActivity(CRActivityClass.Task, null);
			return adapter.Get();
		}
	}
	
	public sealed class ProjectActivities : CRActivityList<PMProject>
	{
		public ProjectActivities(PXGraph graph)
			: base(graph)
		{
			_Graph.RowSelected.AddHandler<PMProject>(PMProject_RowSelected);

			_Graph.FieldDefaulting.AddHandler<PMTimeActivity.projectID>(ProjectID_FieldDefaulting);
			_Graph.FieldDefaulting.AddHandler<PMTimeActivity.trackTime>(TrackTime_FieldDefaulting);
		}
		private void ProjectID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			var primaryCache = sender.Graph.Caches[typeof(PMProject)];

			if (primaryCache.Current != null)
		{
				e.NewValue = ((PMProject)primaryCache.Current).ContractID;
			}

		}

		private void TrackTime_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = true;
		}

		private void PMProject_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			PM.PMProject row = (PM.PMProject) e.Row;
			if (row == null || View == null || View.Cache == null)
				return;

			bool userCanAddActivity = row.Status != ProjectStatus.Completed;
			if (row.RestrictToEmployeeList == true && !sender.Graph.IsExport)
			{
				var select = new PXSelectJoin<EPEmployeeContract,
					InnerJoin<EPEmployee, 
						On<EPEmployee.bAccountID, Equal<EPEmployeeContract.employeeID>>>,
					Where<EPEmployeeContract.contractID, Equal<Current<PMProject.contractID>>,
					And<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>>(_Graph);

				EPEmployeeContract record = select.SelectSingle();
				userCanAddActivity = userCanAddActivity && record != null;
		}

			View.Cache.AllowInsert = userCanAddActivity;
		}

		protected override void CreateTimeActivity(PXCache cache, int classId)
		{
			PXCache timeCache = cache.Graph.Caches[typeof (PMTimeActivity)];
			if (timeCache == null) return;

			PMTimeActivity timeActivity = (PMTimeActivity) timeCache.Current;
			if (timeActivity == null) return;

			bool withTimeTracking = classId != CRActivityClass.Task && classId != CRActivityClass.Event;

			timeActivity.TrackTime = withTimeTracking;
			timeActivity.ProjectID = ((PMProject) _Graph.Caches[typeof (PMProject)].Current)?.ContractID;

			timeCache.Update(timeActivity);
		}

		protected override void SetCommandCondition()
		{
			var command = ProjectActivities.GenerateOriginalCommand();

			var where = typeof(Where<CRPMTimeActivity.projectID, Equal<Current<PMProject.contractID>>>);

			command = command.WhereAnd(where);
			View = new PXView(View.Graph, View.IsReadOnly, command);
		}
		protected override IEnumerable NewActivityByType(PXAdapter adapter, string type)
		{
			//DONE redesign by task #32833
			CreateActivity(CRActivityClass.Activity, type);
			return adapter.Get();
		}

		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.MailSend)]
		[PXShortCut(true, false, false, 'A', 'M')]
		public override IEnumerable NewMailActivity(PXAdapter adapter)
		{
			//DONE redesign by task #32833
			CreateActivity(CRActivityClass.Email, null);
			return adapter.Get();
		}

		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.Task)]
		[PXShortCut(true, false, false, 'K', 'C')]
		public override IEnumerable NewTask(PXAdapter adapter)
		{
			//DONE redesign by task #32833
			CreateActivity(CRActivityClass.Task, null);
			return adapter.Get();
		}
		
	}

	public sealed class OpportunityActivities : CRActivityList<CROpportunity>
	{
		public OpportunityActivities(PXGraph graph)
			: base(graph)
		{			
		}
		
		protected override void SetCommandCondition()
		{
			var command = ProjectActivities.GenerateOriginalCommand();
            
			var where = typeof(
				Where<CRPMTimeActivity.refNoteID, Equal<Current<CROpportunity.noteID>>, 
					Or<Where<True, Equal<Current<CROpportunityClass.showContactActivities>>, 
					And<CRPMTimeActivity.refNoteID, Equal<Current<Contact.noteID>>>>>>);

			command = command.WhereAnd(where);
			View = new PXView(View.Graph, View.IsReadOnly, command);
		}	

	}

	public class CRActivityList<TPrimaryView> : CRActivityListBase<TPrimaryView, CRPMTimeActivity>
		where TPrimaryView : class, IBqlTable, new()
	{
		public CRActivityList(PXGraph graph)
			: base(graph)
		{
		}
	}

	public class CRActivityListReadonly<TPrimaryView> : CRActivityListBase<TPrimaryView, CRPMTimeActivity>
			where TPrimaryView : class, IBqlTable, new()
	{
		public CRActivityListReadonly(PXGraph graph)
			: base(graph)
		{
			var cache = _Graph.Caches[typeof(CRPMTimeActivity)];
			PXUIFieldAttribute.SetEnabled(cache, null, typeof(CRPMTimeActivity.subject).Name, false);
			PXUIFieldAttribute.SetEnabled(cache, null, typeof(CRPMTimeActivity.priority).Name, false);
			PXUIFieldAttribute.SetEnabled(cache, null, typeof(CRPMTimeActivity.uistatus).Name, false);
			PXUIFieldAttribute.SetEnabled(cache, null, typeof(CRPMTimeActivity.startDate).Name, false);
			PXUIFieldAttribute.SetEnabled(cache, null, typeof(CRPMTimeActivity.endDate).Name, false);
			PXUIFieldAttribute.SetEnabled(cache, null, typeof(CRPMTimeActivity.noteID).Name, false);
			PXUIFieldAttribute.SetEnabled(cache, null, typeof(CRPMTimeActivity.createdByID).Name, false);
			PXUIFieldAttribute.SetEnabled(cache, null, typeof(CRPMTimeActivity.body).Name, false);
			PXUIFieldAttribute.SetEnabled(cache, null, typeof(CRPMTimeActivity.categoryID).Name, false);

			PXUIFieldAttribute.SetVisible(cache, null, typeof(CRPMTimeActivity.priority).Name, false);
			cache.AllowUpdate = false;
		}
	}

	public class CRChildActivityList<TParentActivity> : CRActivityListBase<TParentActivity, CRChildActivity>
		where TParentActivity : CRActivity, new()
	{
		public CRChildActivityList(PXGraph graph)
			: base(graph)
		{
			_Graph.FieldDefaulting.AddHandler<CRActivity.parentNoteID>(ParentNoteID_FieldDefaulting);
			_Graph.FieldDefaulting.AddHandler<CRChildActivity.parentNoteID>(ParentNoteID_FieldDefaulting);
			_Graph.FieldDefaulting.AddHandler<CRSMEmail.parentNoteID>(ParentNoteID_FieldDefaulting);

			_Graph.FieldDefaulting.AddHandler<PMTimeActivity.projectID>(ProjectID_FieldDefaulting);
			_Graph.FieldDefaulting.AddHandler<PMTimeActivity.projectTaskID>(ProjectTaskID_FieldDefaulting);
		}

		private void ProjectID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			var primaryCache = sender.Graph.Caches[typeof(CRPMTimeActivity)];

			if (primaryCache.Current != null)
			{
				e.NewValue = ((CRPMTimeActivity)primaryCache.Current).ProjectID;
		}

		}

		private void ProjectTaskID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			var primaryCache = sender.Graph.Caches[typeof(CRPMTimeActivity)];

			if (primaryCache.Current != null)
			{
				e.NewValue = ((CRPMTimeActivity)primaryCache.Current).ProjectTaskID;
			}

		}

		private void ParentNoteID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			var parentCache = sender.Graph.Caches[typeof(TParentActivity)];

			if (parentCache.Current != null)
			{
				e.NewValue = ((TParentActivity)parentCache.Current).NoteID;
			}
		}
		
		public IEnumerable SelectByParentNoteID(object parentNoteId)
		{
			return PXSelect<CRChildActivity,
				Where<CRChildActivity.parentNoteID, Equal<Required<CRChildActivity.parentNoteID>>>>.
				Select(_Graph, parentNoteId).RowCast<CRChildActivity>();
		}

		protected override void ReadNoteIDFieldInfo(out string noteField, out Type noteBqlField)
		{
			noteField = typeof(CRActivity.refNoteID).Name;
			noteBqlField = _Graph.Caches[typeof(TParentActivity)].GetBqlField(noteField);
		}

		protected override void SetCommandCondition()
		{
			var newCmd = OriginalCommand.WhereAnd(
				BqlCommand.Compose(typeof(Where<,,>),
					typeof(CRChildActivity.parentNoteID),
					typeof(Equal<>),
					typeof(Optional<>),
					typeof(TParentActivity).GetNestedType(typeof(CRActivity.noteID).Name),
					typeof(And<>),
					typeof(Where<,,>),
					typeof(CRChildActivity.isCorrected),
                    typeof(NotEqual<True>),
					typeof(Or<,>),
					typeof(CRChildActivity.isCorrected),
					typeof(IsNull)));
			View = new PXView(View.Graph, View.IsReadOnly, newCmd);
		}
	}

	public delegate long? CalculateAlternativeNoteIdHandler(object row);

	public interface IActivityList
	{
		bool CheckActivitiesForDelete(params object[] currents);
		Type PrimaryViewType { get; }
	}

	public abstract class CRActivityListBase<TActivity> : PXSelectBase
		where TActivity : CRPMTimeActivity, new()
	{
		protected internal const string _NEWTASK_COMMAND = "NewTask";
		protected internal const string _NEWEVENT_COMMAND = "NewEvent";
		protected internal const string _VIEWACTIVITY_COMMAND = "ViewActivity";
		protected internal const string _VIEWALLACTIVITIES_COMMAND = "ViewAllActivities";
		protected internal const string _NEWACTIVITY_COMMAND = "NewActivity";
		protected internal const string _NEWMAILACTIVITY_COMMAND = "NewMailActivity";
		protected internal const string _REGISTERACTIVITY_COMMAND = "RegisterActivity";
		protected internal const string _OPENACTIVITYOWNER_COMMAND = "OpenActivityOwner";

		public static BqlCommand GenerateOriginalCommand()
		{
			var classIdField = typeof(TActivity).GetNestedType(typeof(CRActivity.classID).Name);
			var createdDateTimeField = typeof(TActivity).GetNestedType(typeof(CRActivity.createdDateTime).Name);

			return BqlCommand.CreateInstance(
				typeof(Select<,,>), typeof(TActivity),			
				typeof(Where<,>), classIdField, typeof(GreaterEqual<>), typeof(Zero), 
				//typeof(And<,>), typeof(CRActivity.mpstatus), typeof(NotEqual<>), typeof(MailStatusListAttribute.deleted),
				typeof(OrderBy<>), 
				typeof(Desc<>), createdDateTimeField);
		}
	}

	[PXDynamicButton(new string[] { "NewTask", "NewEvent", "ViewActivity", "NewMailActivity", "RegisterActivity", "OpenActivityOwner", "ViewAllActivities", "NewActivity" },
					 new string[] { Messages.AddTask, Messages.AddEvent, Messages.Details, Messages.AddEmail, Messages.RegisterActivity, Messages.OpenActivityOwner, Messages.ViewAllActivities, Messages.AddActivity },
					 TranslationKeyType = typeof(Messages))]
	public class CRActivityListBase<TPrimaryView, TActivity> : CRActivityListBase<TActivity>, IActivityList
		where TPrimaryView : class, IBqlTable, new()
		where TActivity : CRPMTimeActivity, new()
	{
		#region Constants

		protected const string _PRIMARY_WORKGROUP_ID = "WorkgroupID";

		#endregion

		#region Fields

		public delegate Email GetEmailHandler();

		private int? _defaultEMailAccountId;

		private readonly BqlCommand _originalCommand;
		private readonly string _refField;
		private readonly Type _refBqlField;

		private bool _enshureTableData = true;

		#endregion

		#region Ctor

		public CRActivityListBase(PXGraph graph)
		{
			_Graph = graph;

			_Graph.EnsureCachePersistence(typeof(TActivity));

			var cache = _Graph.Caches[typeof(TActivity)];			
			
			ReadNoteIDFieldInfo(out _refField, out _refBqlField);

			graph.RowSelected.AddHandler(typeof(TPrimaryView), Table_RowSelected);
			if (typeof(CRActivity).IsAssignableFrom(typeof(TPrimaryView)))
			graph.RowPersisted.AddHandler<TPrimaryView>(Table_RowPersisted);
			graph.RowDeleting.AddHandler<TActivity>(Activity_RowDeleting);
			graph.RowDeleted.AddHandler<TActivity>(Activity_RowDeleted);
			graph.RowPersisting.AddHandler<TActivity>(Activity_RowPersisting);
			graph.RowPersisted.AddHandler<TActivity>(Activity_RowPersisted);
			graph.RowSelected.AddHandler<TActivity>(Activity_RowSelected);
			
			graph.FieldDefaulting.AddHandler(typeof(CRActivity), typeof(CRActivity.refNoteID).Name, Activity_RefNoteID_FieldDefaulting);
			graph.FieldDefaulting.AddHandler(typeof(CRSMEmail), typeof(CRSMEmail.refNoteID).Name, Activity_RefNoteID_FieldDefaulting);
			graph.FieldDefaulting.AddHandler(typeof(TActivity), typeof(CRActivity.refNoteID).Name, Activity_RefNoteID_FieldDefaulting);

			graph.FieldSelecting.AddHandler(typeof(TActivity), typeof(CRActivity.body).Name, Activity_Body_FieldSelecting);

			AddActions(graph);
			AddPreview(graph);

			PXUIFieldAttribute.SetVisible(cache, null, typeof(CRActivity.noteID).Name, false);

			_originalCommand = GenerateOriginalCommand();
			View = new PXView(graph, false, OriginalCommand);

			SetCommandCondition();
		}

		#endregion

		#region Implementation

		#region Preview

		private void AddPreview(PXGraph graph)
		{
			graph.Initialized += sender =>
				{
					string viewName;
					if (graph.ViewNames.TryGetValue(this.View, out viewName))
					{
						var att = new CRPreviewAttribute(typeof(TPrimaryView), typeof(TActivity));
						att.Attach(graph, viewName, null);
					}
				};
		}

		#endregion

		#region Add Actions
		protected void AddActions(PXGraph graph)
		{
			AddAction(graph, _NEWTASK_COMMAND, Messages.AddTask, NewTask);
			AddAction(graph, _NEWEVENT_COMMAND, Messages.AddEvent, NewEvent);
			AddAction(graph, _VIEWACTIVITY_COMMAND, Messages.Details, ViewActivity);
			AddAction(graph, _NEWMAILACTIVITY_COMMAND, Messages.AddEmail, NewMailActivity);
			AddAction(graph, _OPENACTIVITYOWNER_COMMAND, string.Empty, OpenActivityOwner);
			AddAction(graph, _VIEWALLACTIVITIES_COMMAND, Messages.ViewAllActivities, ViewAllActivities);

			AddActivityQuickActionsAsMenu(graph);
		}

		private void AddActivityQuickActionsAsMenu(PXGraph graph)
		{
			List<ActivityService.IActivityType> types = null;
			try
			{
				types = new List<ActivityService.IActivityType>(new EP.ActivityService().GetActivityTypes());				
			}
			catch (Exception) {/* #46997 */}

				PXAction btn = AddAction(graph, _NEWACTIVITY_COMMAND,
									PXMessages.LocalizeNoPrefix(Messages.AddActivity),
									types != null && types.Count > 0,
									NewActivityByType);

			if (types != null && types.Count > 0)
			{
				List<ButtonMenu> menuItems = new List<ButtonMenu>(types.Count);
				foreach (ActivityService.IActivityType type in types)
				{
					ButtonMenu menuItem = new ButtonMenu(type.Type,
						PXMessages.LocalizeFormatNoPrefix(Messages.AddTypedActivityFormat, type.Description), null);
					if (type.IsDefault == true)
						menuItems.Insert(0, menuItem);
					else
						menuItems.Add(menuItem);
				}
				btn.SetMenu(menuItems.ToArray());
			}
		}

		internal void AddAction(PXGraph graph, string name, string displayName, PXButtonDelegate handler)
		{
			AddAction(graph, name, displayName, true, handler, null);
		}

		internal PXAction AddAction(PXGraph graph, string name, string displayName, bool visible, PXButtonDelegate handler, params PXEventSubscriberAttribute[] attrs)
		{
			PXUIFieldAttribute uiAtt = new PXUIFieldAttribute
							{
								DisplayName = PXMessages.LocalizeNoPrefix(displayName),
								MapEnableRights = PXCacheRights.Select
							};
			if (!visible) uiAtt.Visible = false;
			List<PXEventSubscriberAttribute> addAttrs = new List<PXEventSubscriberAttribute> { uiAtt };
			if (attrs != null)
				addAttrs.AddRange(attrs.Where(attr => attr != null));
			PXNamedAction<TPrimaryView> res = new PXNamedAction<TPrimaryView>(graph, name, handler, addAttrs.ToArray());
			graph.Actions[name] = res;
			return res;
		}
		#endregion

		protected PXCache CreateInstanceCache<TNode>(Type graphType)
			where TNode : IBqlTable
		{
			if (graphType != null)
			{
				if (EnshureTableData && _Graph.IsDirty)
				{
                    if (_Graph.AutomationView != null)                    
					    PXAutomation.GetStep(_Graph,
						    new object[] { _Graph.Views[_Graph.AutomationView].Cache.Current },
						    BqlCommand.CreateInstance(typeof(Select<>), _Graph.Views[_Graph.AutomationView].Cache.GetItemType()));
					_Graph.Actions.PressSave();
				}

				var graph = PXGraph.CreateInstance(graphType);
				graph.Clear();
				foreach (Type type in graph.Views.Caches)
				{
					var cache = graph.Caches[type];
					if (typeof(TNode).IsAssignableFrom(cache.GetItemType()))
						return cache;
				}
			}
			return null;
		}

		public Type PrimaryViewType
		{
			get { return typeof (TPrimaryView); }
		}

		#region Delete Record
		public virtual void DeleteActivities(params object[] currents)
		{
			if (!typeof(CRActivity).IsAssignableFrom(typeof(CRActivity))) return;

			foreach (var item in View.SelectMultiBound(currents))
				Cache.Delete(item is PXResult ? ((PXResult)item)[typeof(CRActivity)] : item);
		}

		public virtual bool CheckActivitiesForDelete(params object[] currents)
		{
			return CheckActivitiesForDeleteInternal("act_cannot_delete", Messages.ConfirmDeleteActivities, currents);
		}

		protected virtual bool CheckActivitiesForDeleteInternal(string key, string msg, params object[] currents)
		{
			if (!typeof(CRPMTimeActivity).IsAssignableFrom(typeof(CRPMTimeActivity))) return true;

			foreach (object item in View.SelectMultiBound(currents))
			{
				var row = (CRPMTimeActivity) item;
				
				if (row.Billed == true || !string.IsNullOrEmpty(row.TimeCardCD))
				{
					return View.Ask(key, msg, MessageButtons.YesNoCancel) == WebDialogResult.Yes;
				}
			}
			return true;
		}

		private void DeleteActivity(CRActivity current)
		{
			var graphType = CRActivityPrimaryGraphAttribute.GetGraphType(current);
			var cache = CreateInstanceCache<CRActivity>(graphType);
			if (cache != null)
			{
				if (!cache.AllowDelete)
					throw new PXException(ErrorMessages.CantDeleteRecord);

				var searchView = new PXView(
					cache.Graph,
					false,
					BqlCommand.CreateInstance(typeof(Select<>), cache.GetItemType()));
				var startRow = 0;
				var totalRows = 0;
				var acts = searchView.
					Select(null, null,
						new object[] { current.NoteID },
						new string[] { typeof(CRActivity.noteID).Name },
						null, null, ref startRow, 1, ref totalRows);

				if (acts != null && acts.Count > 0)
				{
					var act = acts[0];
					cache.Current = act;
					cache.Delete(act);
					cache.Graph.Actions.PressSave();
				}
			}
		}
		#endregion

		protected void CreateActivity(int classId, string type)
		{
			Type graphType = CRActivityPrimaryGraphAttribute.GetGraphType(classId);

			// TODO: NEED REFACTOR! 

			if (!PXAccess.VerifyRights(graphType))
			{
				_Graph.Views[_Graph.PrimaryView].Ask(null, Messages.AccessDenied, Messages.FormNoAccessRightsMessage(graphType),
					MessageButtons.OK, MessageIcon.Error);
			}
			else
			{
				PXCache cache;

				if (classId == CRActivityClass.Email)
				{
					cache = CreateInstanceCache<CRSMEmail>(graphType);
					if (cache == null) return;

					var localActivity = (CRSMEmail)_Graph.Caches[typeof(CRSMEmail)].CreateInstance();

					localActivity.ClassID = classId;
					localActivity.Type = type;

					CRSMEmail email = ((PXCache<CRSMEmail>)_Graph.Caches[typeof(CRSMEmail)]).InitNewRow(localActivity);


					Guid? owner = EmployeeMaint.GetCurrentEmployeeID(_Graph);
					int? workgroup = GetParentGroup();
					email.OwnerID = owner;
					if (email.OwnerID != null && PXOwnerSelectorAttribute.BelongsToWorkGroup(_Graph, workgroup, email.OwnerID))
						email.WorkgroupID = workgroup;


					email.MailAccountID = DefaultEMailAccountId;
					FillMailReply(email);
					FillMailTo(email);
					if (email.RefNoteID != null)
						FillMailCC(email, email.RefNoteID);
					FillMailSubject(email);
					email.Body = GenerateMailBody();

					email.ClassID = classId;
					_Graph.Caches[typeof (CRSMEmail)].SetValueExt(email, typeof (CRActivity.type).Name,
						!string.IsNullOrEmpty(type) ? type : email.Type);
					
					if (_Graph.IsDirty)
					{
						if (_Graph.IsMobile) // ensure that row will be persisted with Note when call from mobile
						{
							var rowCache = _Graph.Views[_Graph.PrimaryView].Cache;
							if (rowCache.Current != null)
							{
								rowCache.SetStatus(rowCache.Current, PXEntryStatus.Updated);
							}
						}
						_Graph.Actions.PressSave();
					}

					cache.Insert(email);
				}
				else
				{
					CRActivity activity;
					
					cache = CreateInstanceCache<CRActivity>(graphType);
					if (cache == null) return;

					var localActivity = (CRActivity)_Graph.Caches[typeof(CRActivity)].CreateInstance();

					localActivity.ClassID = classId;
					localActivity.Type = type;

					activity = ((PXCache<CRActivity>)_Graph.Caches[typeof(CRActivity)]).InitNewRow(localActivity);


					Guid? owner = EmployeeMaint.GetCurrentEmployeeID(_Graph);
					int? workgroup = GetParentGroup();
					activity.OwnerID = owner;
					if (activity.OwnerID != null && PXOwnerSelectorAttribute.BelongsToWorkGroup(_Graph, workgroup, activity.OwnerID))
						activity.WorkgroupID = workgroup;


					if (_Graph.IsDirty)
					{
						if (_Graph.IsMobile) // ensure that row will be persisted with Note when call from mobile
						{
							var rowCache = _Graph.Views[_Graph.PrimaryView].Cache;
							if (rowCache.Current != null)
							{
								rowCache.SetStatus(rowCache.Current, PXEntryStatus.Updated);
							}
						}
						_Graph.Actions.PressSave();
					}
					
					cache.Insert(activity);
				}

				CreateTimeActivity(cache, classId);

				foreach (PXCache dirtycache in cache.Graph.Caches.Caches.Where(c => c.IsDirty))
				{
					dirtycache.IsDirty = false;
				}

				PXRedirectHelper.TryRedirect(cache.Graph, PXRedirectHelper.WindowMode.NewWindow);
			}
		}

		protected virtual void CreateTimeActivity(PXCache graphType, int classId)
		{
			
		}

		private int? GetParentGroup()
		{
			PXCache cache = _Graph.Caches[typeof(TPrimaryView)];
			return (int?)cache.GetValue(cache.Current, _PRIMARY_WORKGROUP_ID);
		}

		protected TActivity CurrentActivity
		{
			get
			{
				var tableCache = _Graph.Caches[typeof(TActivity)];
				return tableCache.With(_ => (TActivity)_.Current);
			}
		}

        public virtual void SendNotification(string sourceType, IList<string> notificationCDs, int? branchID, IDictionary<string, string> parameters, IList<Guid?> attachments = null)
	    {
            Guid[] setupIDs = new Guid[notificationCDs.Count];
            if (branchID == null)
                branchID = this._Graph.Accessinfo.BranchID;

            PXCache sourceCache = _Graph.Caches[typeof(TPrimaryView)];
            if (sourceCache.Current == null)
                throw new PXException(Messages.EmailNotificationObjectNotFound);

            if (parameters == null)
            {
                parameters = new Dictionary<string, string>();
                foreach (string key in sourceCache.Keys)
                {
                    object value = sourceCache.GetValueExt(sourceCache.Current, key);
                    parameters[key] = value != null ? value.ToString() : null;
                }
            }

	        for (int i=0; i<notificationCDs.Count; i++)
	        {
	            string notificationCD = notificationCDs[i];
                Guid? SetupID = new NotificationUtility(_Graph).SearchSetupID(sourceType, notificationCD);
                if (SetupID == null)
                    throw new PXException(Messages.EmailNotificationSetupNotFound, notificationCD);
	            setupIDs[i] = SetupID.Value;
	        }
            Send(sourceType, setupIDs, branchID, parameters, attachments);
	    }

	    public void SendNotification(string sourceType, string notifications, int? branchID,
            IDictionary<string, string> parameters, IList<Guid?> attachments = null)
	    {
            if (notifications == null) return;
            IList<string> list = notifications.Split(',')
                .Select(n => n != null ? n.Trim() : null)
                .Where(cd => !string.IsNullOrEmpty(cd)).ToList();
	        SendNotification(sourceType, list, branchID, parameters, attachments);
	    }



        private void Send(string sourceType, IList<Guid> setupIDs, int? branchID, IDictionary<string, string> reportParams, IList<Guid?> attachments = null)
		{
			PXCache cache = _Graph.Caches[typeof(TPrimaryView)];
			TPrimaryView row = (TPrimaryView)cache.Current;

			TActivity activity = ((PXCache<TActivity>)_Graph.Caches[typeof(TActivity)]).InitNewRow();
			Guid? refNoteId = activity.RefNoteID;
			int? bAccountID = activity.BAccountID;

			var sourceRow = bAccountID.With(_ => new EntityHelper(_Graph).GetEntityRowByID(typeof(BAccountR), _.Value));

			if (sourceRow == null)
				sourceRow = refNoteId.With(_ => new EntityHelper(_Graph).GetEntityRow(typeof(BAccountR), _.Value));

			var utility = new NotificationUtility(_Graph);
		    RecipientList recipients = null;
		    TemplateNotificationGenerator sender = null;
		    for(int i=0 ; i < setupIDs.Count; i++)
		    {
		        Guid setupID = setupIDs[i];
                NotificationSource source = utility.GetSource(sourceType, sourceRow, setupID, branchID);

                if (source == null)
                    throw new PXException(PX.SM.Messages.NotificationSourceNotFound);

                if (sender == null)
		        {
                    var accountId = source.EMailAccountID ?? DefaultEMailAccountId;
                    if (accountId == null)
                        throw new PXException(ErrorMessages.EmailNotConfigured);

                    if (recipients == null)
                        recipients = utility.GetRecipients(sourceType, sourceRow, source);

		            sender = TemplateNotificationGenerator.Create(row, source.NotificationID);
                    
		            if (source.EMailAccountID != null)
		            {
		                sender.MailAccountId = accountId;
		            }
		            sender.RefNoteID = refNoteId;
		            sender.BAccountID = bAccountID;
		            sender.Watchers = recipients;
		        }
		        if (source.ReportID != null)
		        {
                    var _report = PXReportTools.LoadReport(source.ReportID, null);

                    if (_report == null) throw new ArgumentException(PXMessages.LocalizeFormatNoPrefixNLA(EP.Messages.ReportCannotBeFound, source.ReportID), "reportId");
                    PXReportTools.InitReportParameters(_report, reportParams, SettingsProvider.Instance.Default);
		            _report.MailSettings.Format = ReportNotificationGenerator.ConvertFormat(source.Format);
                    var reportNode = ReportProcessor.ProcessReport(_report);

                    reportNode.SendMailMode = true;
		            PX.Reports.Mail.Message message = 
                        (from msg in reportNode.Groups.Select(g => g.MailSettings) 
                         where msg != null && msg.ShouldSerialize() 
                         select new PX.Reports.Mail.Message(msg, reportNode, msg)).FirstOrDefault();

		            if (message == null) continue;
		            if (i == 0)
		            {
		                if (sender.Body == null)
		                {
		                    sender.Body = string.IsNullOrEmpty(sender.Body) ? message.Content.Body : sender.Body;
		                    sender.BodyFormat = NotificationFormat.Html;
		                }
		                sender.Subject = string.IsNullOrEmpty(sender.Subject) ? message.Content.Subject : sender.Subject;
                        sender.To = string.IsNullOrEmpty(sender.To) ? message.Addressee.To : sender.To;
                        sender.Cc = string.IsNullOrEmpty(sender.Cc) ? message.Addressee.Cc : sender.Cc;
                        sender.Bcc = string.IsNullOrEmpty(sender.Bcc) ? message.Addressee.Bcc : sender.Bcc;

                        if (!string.IsNullOrEmpty(message.TemplateID))
                        {
                            TemplateNotificationGenerator generator =
                                TemplateNotificationGenerator.Create(row,message.TemplateID);

                            var template = generator.ParseNotification();

                            if (string.IsNullOrEmpty(sender.Body))
                                sender.Body = template.Body;
                            if (string.IsNullOrEmpty(sender.Subject))
                                sender.Subject = template.Subject;
                            if (string.IsNullOrEmpty(sender.To))
                                sender.To = template.To;
                            if (string.IsNullOrEmpty(sender.Cc))
                                sender.Cc = template.Cc;
                            if (string.IsNullOrEmpty(sender.Bcc))
                                sender.Bcc = template.Bcc;
                        }
                        if (string.IsNullOrEmpty(sender.Subject))
                            sender.Subject = reportNode.Report.Name;
		            }
			        foreach (var attachment in message.Attachments)
			        {
				        if (sender.Body == null && sender.BodyFormat == NotificationFormat.Html && attachment.MimeType == "text/html")
				        {
					        sender.Body = attachment.Encoding.GetString(attachment.GetBytes());
				        }
						  else
							sender.AddAttachment(attachment.Name, attachment.GetBytes());
			        }

			        if(attachments != null)
                        foreach (var attachment in attachments)  
                            if(attachment != null)
                                sender.AddAttachmentLink(attachment.Value);                                                   
		        }
		    }
            
			if (sender == null || !sender.Send().Any())
				throw new PXException(Messages.EmailNotificationError);
		}

		private static Guid? GetNoteId(PXGraph graph, object row)
		{
			if (row == null) return null;

			var rowType = row.GetType();
			var noteField = EntityHelper.GetNoteField(rowType);
			var cache = graph.Caches[rowType];
			return PXNoteAttribute.GetNoteID(cache, row, noteField);
		}

		#endregion

		#region Email Methods

		protected virtual void FillMailReply(CRSMEmail message)
		{
			PX.Common.Mail.Mailbox mailAddress = null;
			var isCorrect = message.MailReply != null &&
				PX.Common.Mail.Mailbox.TryParse(message.MailReply, out mailAddress) && 
				!string.IsNullOrEmpty(mailAddress.Address);
			if (isCorrect)
			{
				isCorrect = PXSelect<EMailAccount,
					Where<EMailAccount.address, Equal<Required<EMailAccount.address>>>>.
					Select(_Graph, mailAddress.Address).
					Count > 0;
			}

			var result = message.MailReply;
			if (!isCorrect)
				result = DefaultEMailAccountId.
					With(_ => (EMailAccount)PXSelect<EMailAccount,
						Where<EMailAccount.emailAccountID, Equal<Required<EMailAccount.emailAccountID>>>>.
					Select(_Graph, _.Value)).
					With(_ => _.Address);
			if (string.IsNullOrEmpty(result))
			{
				var firstAcct = (EMailAccount)PXSelect<EMailAccount>.SelectWindowed(_Graph, 0, 1);
				if (firstAcct != null) result = firstAcct.Address;
			}
			message.MailReply = result;
		}
		
		protected virtual void FillMailTo(CRSMEmail message)
		{
			string customMailTo;
			if (GetNewEmailAddress != null && !string.IsNullOrEmpty(customMailTo = this._Graph.IsMobile ? GetNewEmailAddress().Address : GetNewEmailAddress().ToString()))
				message.MailTo = customMailTo.With(_ => _.Trim());
		}

		protected virtual void FillMailCC(CRSMEmail message, Guid? refNoteId)
		{
			if (refNoteId != null)
				foreach (Mailbox email in CRRelationsList<CRRelation.refNoteID>.GetEmailsForCC(_Graph, refNoteId))
					message.MailCc += email + "; ";
		}

		protected virtual void FillMailSubject(CRSMEmail message)
		{
			if (!string.IsNullOrEmpty(DefaultSubject))
				message.Subject = DefaultSubject;
		}

		protected virtual string GenerateMailBody()
		{
			string res = null;
			var signature = ((UserPreferences)PXSelect<UserPreferences>.
				Search<UserPreferences.userID>(_Graph, PXAccess.GetUserID())).
				With(pref => pref.MailSignature);
			if (signature != null && (signature = signature.Trim()) != string.Empty)
				res += "<br />" + signature;
			return res;
		}

		#endregion

		#region Actions

		[PXButton]
		[PXUIField(Visible = false)]
		public virtual IEnumerable OpenActivityOwner(PXAdapter adapter)
		{
			var act = Cache.Current as CRActivity;
			if (act != null)
			{
				var empl = (EPEmployee)PXSelectReadonly<EPEmployee,
					Where<EPEmployee.userID, Equal<Required<EPEmployee.userID>>>>.
					Select(_Graph, act.OwnerID);
				if (empl != null)
					PXRedirectHelper.TryRedirect(_Graph.Caches[typeof(EPEmployee)], empl, string.Empty, PXRedirectHelper.WindowMode.NewWindow);

				var usr = (Users)PXSelectReadonly<Users,
					Where<Users.pKID, Equal<Required<Users.pKID>>>>.
					Select(_Graph, act.OwnerID);
				if (usr != null)
					PXRedirectHelper.TryRedirect(_Graph.Caches[typeof(Users)], usr, string.Empty, PXRedirectHelper.WindowMode.NewWindow);
			}
			return adapter.Get();
		}

		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.Task, OnClosingPopup = PXSpecialButtonType.Refresh)]
		[PXShortCut(true, false, false, 'K', 'C')]		
		public virtual IEnumerable NewTask(PXAdapter adapter)
		{
			CreateActivity(CRActivityClass.Task, null);
			return adapter.Get();
		}

		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.Event, OnClosingPopup = PXSpecialButtonType.Refresh)]
		[PXShortCut(true, false, false, 'E', 'C')]
		public virtual IEnumerable NewEvent(PXAdapter adapter)
		{
			CreateActivity(CRActivityClass.Event, null);
			return adapter.Get();
		}

		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntry, OnClosingPopup = PXSpecialButtonType.Refresh)]
		[PXShortCut(true, false, false, 'A', 'C')]
		public virtual IEnumerable NewActivity(PXAdapter adapter)
		{
			return NewActivityByType(adapter);
		}

		[PXButton(OnClosingPopup = PXSpecialButtonType.Refresh)]
		protected virtual IEnumerable NewActivityByType(PXAdapter adapter)
		{
			return NewActivityByType(adapter, adapter.Menu);
		}

        [PXButton(OnClosingPopup = PXSpecialButtonType.Refresh)]
		protected virtual IEnumerable NewActivityByType(PXAdapter adapter, string type)
		{
			CreateActivity(CRActivityClass.Activity, type);
			return adapter.Get();
		}

		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.MailSend, OnClosingPopup = PXSpecialButtonType.Refresh)]
		[PXShortCut(true, false, false, 'A', 'M')]
		public virtual IEnumerable NewMailActivity(PXAdapter adapter)
		{
			CreateActivity(CRActivityClass.Email, null);
			return adapter.Get();
		}

		[PXUIField(Visible = false, MapEnableRights = PXCacheRights.Select)]
		[PXButton(OnClosingPopup = PXSpecialButtonType.Refresh)]
		public virtual IEnumerable ViewAllActivities(PXAdapter adapter)
		{
			var gr = new ActivitiesMaint();
			gr.Filter.Current.NoteID = ((PXCache<TActivity>)_Graph.Caches[typeof(TActivity)]).InitNewRow().RefNoteID;
			throw new PXPopupRedirectException(gr, string.Empty, true);
		}

		[PXUIField(Visible = false, MapEnableRights = PXCacheRights.Select)]
		[PXButton(OnClosingPopup = PXSpecialButtonType.Refresh)]
		public virtual IEnumerable ViewActivity(PXAdapter adapter)
		{
			PXRedirectHelper.TryRedirect(_Graph.Caches[typeof(TActivity)].Graph, CurrentActivity, PXRedirectHelper.WindowMode.NewWindow);

			return adapter.Get();
		}

		#endregion

		#region Buttons

		public PXAction ButtonViewAllActivities
		{
			get { return _Graph.Actions[_VIEWALLACTIVITIES_COMMAND]; }
		}

		#endregion

		#region Properties

		public bool EnshureTableData
		{
			get { return _enshureTableData; }
			set { _enshureTableData = value; }
		}

		public virtual GetEmailHandler GetNewEmailAddress { get; set; }

		public string DefaultSubject { get; set; }

		public int? DefaultEMailAccountId
		{
			get
			{
				return _defaultEMailAccountId ?? MailAccountManager.DefaultAnyMailAccountID;
			}
			set { _defaultEMailAccountId = value; }
		}

		#endregion

		#region Event Handlers
		protected virtual void Table_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			object row = e.Row;
			if (sender.Graph.GetType() != typeof(PXGraph) && sender.Graph.GetType() != typeof(PXGenericInqGrph))
			{
				Type itemType = sender.GetItemType();
				DynamicRowSelected rs = new DynamicRowSelected(itemType, row, sender, this);
				//will be called after graph event
				sender.Graph.RowSelected.AddHandler(itemType, rs.RowSelected);
			}
			
		}

		private class DynamicRowSelected
		{
			private Type ItemType;
			private object Row;
			private PXCache Cache;
			private CRActivityListBase<TPrimaryView, TActivity> BaseClass;
			public DynamicRowSelected(Type itemType, object row, PXCache cache, CRActivityListBase<TPrimaryView, TActivity> baseClass)
			{
				ItemType = itemType;
				Row = row;
				Cache = cache;
				BaseClass = baseClass;
			}
			public void RowSelected(PXCache sender, PXRowSelectedEventArgs e)
			{
				BaseClass.CorrectButtons(Cache, Row, Cache.GetStatus(Row));
				sender.Graph.RowSelected.RemoveHandler(ItemType, RowSelected);
			}
		}

		internal void CorrectButtons(PXCache sender, object row, PXEntryStatus status)
		{
			if (!EnshureTableData) return;

			row = row ?? sender.Current;
			var viewButtonsEnabled = row != null;

			viewButtonsEnabled = viewButtonsEnabled && Array.IndexOf(NotEditableStatuses, status) < 0;
			var editButtonEnabled = viewButtonsEnabled && this.View.Cache.AllowInsert;
			PXActionCollection actions = sender.Graph.Actions;

			actions[_NEWTASK_COMMAND].SetEnabled(editButtonEnabled);
			actions[_NEWEVENT_COMMAND].SetEnabled(editButtonEnabled);
			actions[_NEWMAILACTIVITY_COMMAND].SetEnabled(editButtonEnabled);
			actions[_NEWACTIVITY_COMMAND].SetEnabled(editButtonEnabled);

			PXButtonState state = actions[_NEWACTIVITY_COMMAND].GetState(row) as PXButtonState;
			if (state != null && state.Menus != null)
				foreach (var button in state.Menus)
				{
					button.Enabled = editButtonEnabled;
				}
		}

		protected virtual void Table_RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
		{
			if (e.Operation == PXDBOperation.Insert && e.TranStatus == PXTranStatus.Completed)
			{
				object row = e.Row;
				CorrectButtons(sender, row, PXEntryStatus.Notchanged);
			}
		}

		protected virtual void Activity_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			if (e.Row != null)
			{
                //Emails with empty recipient should only remove from cache
                if (sender.GetStatus(e.Row) != PXEntryStatus.InsertedDeleted)
                    DeleteActivity((CRActivity)e.Row);				

				sender.SetStatus(e.Row, PXEntryStatus.Notchanged);
				sender.Remove(e.Row);

				sender.SetValuePending(e.Row, "cacheIsDirty", sender.IsDirty);
			}
		}

		protected virtual void Activity_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			if (e.Row != null)
				sender.IsDirty = true.Equals(sender.GetValuePending(e.Row, "cacheIsDirty"));
		}

		protected virtual void Activity_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if ((e.Operation & PXDBOperation.Delete) == PXDBOperation.Delete) e.Cancel = true;

			var row = e.Row as TActivity;
			if (row == null) return;

			if (row.TimeActivityNoteID == null)
			{
				// means no TimeActivity

				foreach (var field in _Graph.Caches<PMTimeActivity>().Fields)
				{
					foreach (PXDefaultAttribute attribute in sender.GetAttributesReadonly(field).OfType<PXDefaultAttribute>())
					{
						if (sender.GetValue(row, field) != null)
							continue;

						sender.SetDefaultExt(row, field);
					}
				}

				PXProjectionAttribute projection = (PXProjectionAttribute) (row.GetType()).GetCustomAttributes(typeof (PXProjectionAttribute), true)[0];

				projection.Persistent = false;
			}
		}

		protected virtual void Activity_RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
		{
			var row = e.Row as TActivity;
			if (row == null) return;

			PXProjectionAttribute projection = (PXProjectionAttribute) (row.GetType()).GetCustomAttributes(typeof (PXProjectionAttribute), true)[0];

			projection.Persistent = true;
		}

		protected virtual void Activity_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			var row = e.Row as TActivity;
			if (row == null) return;

			if (row.ClassID == CRActivityClass.Task || row.ClassID == CRActivityClass.Event)
			{
				int timespent = 0;
				int overtimespent = 0;
				int timebillable = 0;
				int overtimebillable = 0;

				foreach (PXResult<CRChildActivity, PMTimeActivity> child in 
					PXSelectJoin<CRChildActivity,
						InnerJoin<PMTimeActivity, 
							On<PMTimeActivity.refNoteID, Equal<CRChildActivity.noteID>>>,
						Where<CRChildActivity.parentNoteID, Equal<Required<CRChildActivity.parentNoteID>>,
							And<
								Where<PMTimeActivity.isCorrected, NotEqual<True>, Or<PMTimeActivity.isCorrected, IsNull>>>>>.
						Select(_Graph, row.NoteID))
				{
					var childTime = (PMTimeActivity) child;

					timespent += (childTime.TimeSpent ?? 0);
					overtimespent += (childTime.OvertimeSpent ?? 0);
					timebillable += (childTime.TimeBillable ?? 0);
					overtimebillable += (childTime.OvertimeBillable ?? 0);
				}

				row.TimeSpent = timespent;
				row.OvertimeSpent = overtimespent;
				row.TimeBillable = timebillable;
				row.OvertimeBillable = overtimebillable;
			}
		}

		protected virtual void Activity_RefNoteID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			GetNoteId(sender.Graph, sender.Graph.Caches[typeof(TPrimaryView)].Current);
			PXCache cache = sender.Graph.Caches[_refBqlField.DeclaringType];
			e.NewValue = cache.GetValue(cache.Current, _refBqlField.Name);
		}

		protected virtual void Activity_Body_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			var row = e.Row as TActivity;
			if (row == null) return;

			if (row.ClassID == CRActivityClass.Email)
			{
				var entity = (SMEmailBody)PXSelect<SMEmailBody, Where<SMEmailBody.refNoteID, Equal<Required<CRPMTimeActivity.noteID>>>>.Select(sender.Graph, row.NoteID);

				e.ReturnValue = entity.Body;
			}
		}

		#endregion

		public TActivity Current
		{
			get { return (TActivity) View.Cache.Current; }
		}

		public virtual PXResultset<TActivity> Select(params object[] arguments)
		{
			var ret = new PXResultset<TActivity>();
			foreach (object item in View.SelectMulti(arguments))
				if (!(item is PXResult<TActivity>))
				{
					if (item is TActivity)
					{
						ret.Add(new PXResult<TActivity>((TActivity) item));
					}
				}
				else
					ret.Add((PXResult<TActivity>) item);
			return ret;
		}


		protected virtual void ReadNoteIDFieldInfo(out string noteField, out Type noteBqlField)
		{
			var cache = _Graph.Caches[typeof (TPrimaryView)];
			noteField = EntityHelper.GetNoteField(typeof (TPrimaryView));
			if (string.IsNullOrEmpty(_refField))
				throw new ArgumentException(
					string.Format("Type '{0}' must contain field with PX.Data.NoteIDAttribute on it",
								  typeof (TPrimaryView).GetLongName()));
			noteBqlField = cache.GetBqlField(_refField);
		}		

		protected virtual void SetCommandCondition()
		{
			Type refID;
			Type sourceID = _refBqlField;

			if (PrimaryViewType != null && typeof(BAccount).IsAssignableFrom(PrimaryViewType))
			{
				refID = typeof(CRPMTimeActivity.bAccountID);
				sourceID = View.Graph.Caches[PrimaryViewType].GetBqlField(typeof(BAccount.bAccountID).Name);
			}
			else if (PrimaryViewType != null && typeof(Contact).IsAssignableFrom(PrimaryViewType))
			{
				refID = typeof(CRPMTimeActivity.contactID);
				sourceID = View.Graph.Caches[PrimaryViewType].GetBqlField(typeof(Contact.contactID).Name);
			}
			else
			{
				refID = typeof(CRPMTimeActivity.refNoteID);
			}

			var newCmd = OriginalCommand.WhereAnd(
				BqlCommand.Compose(typeof(Where<,>),
					refID,                  // Activity
					typeof(Equal<>),
					typeof(Current<>),
					sourceID));			// Primary

			View = new PXView(View.Graph, View.IsReadOnly, newCmd);
		}

		protected virtual PXEntryStatus[] NotEditableStatuses
		{
			get
			{
				return new[] { PXEntryStatus.Inserted, PXEntryStatus.InsertedDeleted };
			}
		}

		protected BqlCommand OriginalCommand
		{
			get { return _originalCommand; }
		}
	}

	#endregion

	#region PMTimeActivityList

	public class PMTimeActivityList<TMasterActivity> : PXSelectBase
		where TMasterActivity : CRActivity, new()
	{
		#region Constants

		private static readonly EPSetup EmptyEpSetup = new EPSetup();

		private const string _DELETE_ACTION_NAME = "Delete";
		private const string _MARKASCOMPLETED_ACTION_NAME = "MarkAsCompleted";
		#endregion


		#region Ctor

		public PMTimeActivityList(PXGraph graph)
		{
			_Graph = graph;
			
			graph.RowSelected.AddHandler(typeof(PMTimeActivity), PMTimeActivity_RowSelected);
			graph.RowDeleting.AddHandler(typeof(PMTimeActivity), PMTimeActivity_RowDeleting);
			graph.RowInserted.AddHandler(typeof(PMTimeActivity), PMTimeActivity_RowInserted);
			graph.RowInserting.AddHandler(typeof(PMTimeActivity), PMTimeActivity_RowInserting);
			graph.RowPersisting.AddHandler(typeof(PMTimeActivity), PMTimeActivity_RowPersisting);
			graph.RowUpdated.AddHandler(typeof(PMTimeActivity), PMTimeActivity_RowUpdated);

			graph.FieldUpdated.AddHandler(typeof(PMTimeActivity), typeof(PMTimeActivity.timeSpent).Name, PMTimeActivity_TimeSpent_FieldUpdated);
			graph.FieldUpdated.AddHandler(typeof(PMTimeActivity), typeof(PMTimeActivity.trackTime).Name, PMTimeActivity_TrackTime_FieldUpdated);
			graph.FieldUpdated.AddHandler(typeof(PMTimeActivity), typeof(PMTimeActivity.approvalStatus).Name, PMTimeActivity_ApprovalStatus_FieldUpdated);

			graph.RowInserted.AddHandler<TMasterActivity>(Master_RowInserted);
			graph.RowPersisting.AddHandler<TMasterActivity>(Master_RowPersisting);

			graph.FieldUpdated.AddHandler(typeof(TMasterActivity), typeof(CRActivity.type).Name, Master_Type_FieldUpdated);
			graph.FieldUpdated.AddHandler(typeof(TMasterActivity), typeof(CRActivity.ownerID).Name, Master_OwnerID_FieldUpdated);
			graph.FieldUpdated.AddHandler(typeof(TMasterActivity), typeof(CRActivity.startDate).Name, Master_StartDate_FieldUpdated);
			graph.FieldUpdated.AddHandler(typeof(TMasterActivity), typeof(CRActivity.subject).Name, Master_Subject_FieldUpdated);
			graph.FieldUpdated.AddHandler(typeof(TMasterActivity), typeof(CRActivity.parentNoteID).Name, Master_ParentNoteID_FieldUpdated);
			
			View = new PXView(graph, false, GenerateOriginalCommand());
			PXUIFieldAttribute.SetDisplayName<PMTimeActivity.approvalStatus>(View.Cache, Data.EP.Messages.Status);
			ApprovalStatusAttribute.SetRestictedMode<PMTimeActivity.approvalStatus>(View.Cache, true);
		}

		#endregion

		public static BqlCommand GenerateOriginalCommand()
		{
			var createdDateTimeField = typeof(PMTimeActivity).GetNestedType(typeof(PMTimeActivity.createdDateTime).Name);
			var noteIDField = typeof(TMasterActivity).GetNestedType(typeof(CRActivity.noteID).Name);

			return BqlCommand.CreateInstance(
				typeof(Select<,,>), 
					typeof(PMTimeActivity),
				typeof(Where<,,>), 
					typeof(PMTimeActivity.refNoteID), typeof(Equal<>), typeof(Current<>), noteIDField,
                typeof(And<PMTimeActivity.isCorrected, Equal<False>>),
				typeof(OrderBy<>),
					typeof(Desc<>), createdDateTimeField);
		}
		public virtual object SelectSingle(params object[] parameters)
		{
			return View.SelectSingle(parameters);
		}

		#region Event Handlers

		protected virtual void Master_RowInserted(PXCache cache, PXRowInsertedEventArgs e)
		{
            using(var s = new ReadOnlyScope(MainCache))
		    {
		        this.Current = (PMTimeActivity) MainCache.Insert();
		        this.Current.ApprovalStatus = ActivityStatusListAttribute.Open;
		    }
		}
		protected virtual void Master_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
		{
			var row = e.Row as TMasterActivity;
			PMTimeActivity timeActivity = (PMTimeActivity)Current;
			if (row == null || timeActivity == null) return;
			
			if (timeActivity.TrackTime != true && e.Operation != PXDBOperation.Delete)
			{
				if (row.ClassID != CRActivityClass.Email)
				{
					var status = row.ClassID != CRActivityClass.Event && row.ClassID != CRActivityClass.Task
						? ActivityStatusAttribute.Completed
						: row.UIStatus;

					cache.SetValueExt(row, typeof(CRActivity.uistatus).Name, status);
				}
			}
		}
		
		protected virtual void Master_Type_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			var row = e.Row as TMasterActivity;
			PMTimeActivity timeActivity = (PMTimeActivity)(Current ?? MainCache.Insert());
			if (row == null || timeActivity == null) return;

			timeActivity.TrackTime = (bool?) PXFormulaAttribute.Evaluate<PMTimeActivity.trackTime>(MainCache, timeActivity);

			MainCache.Update(timeActivity);
		}

		protected virtual void Master_OwnerID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			var row = e.Row as TMasterActivity;
			PMTimeActivity timeActivity = (PMTimeActivity)Current;
			if (row == null || timeActivity == null) return;

			timeActivity.OwnerID = row.OwnerID;

			MainCache.Update(timeActivity);
		}

		protected virtual void Master_StartDate_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			var row = e.Row as TMasterActivity;
			PMTimeActivity timeActivity = (PMTimeActivity)Current;
			if (row == null || timeActivity == null) return;

			timeActivity.Date = (DateTime?) PXFormulaAttribute.Evaluate<PMTimeActivity.date>(MainCache, timeActivity);
            MainCache.SetDefaultExt<PMTimeActivity.weekID>(timeActivity);
            MainCache.Update(timeActivity);
		}

		protected virtual void Master_Subject_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			PMTimeActivity timeActivity = (PMTimeActivity)Current;
			if (timeActivity == null) return;

			if(MainCache.GetStatus(timeActivity) == PXEntryStatus.Notchanged)
				MainCache.SetStatus(timeActivity, PXEntryStatus.Updated);
		}

		protected virtual void Master_ParentNoteID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			var row = e.Row as TMasterActivity;
			PMTimeActivity timeActivity = (PMTimeActivity) Current;
			if (row == null || timeActivity == null) return;

			var item = (PXResult<CRActivity, PMTimeActivity>)
				PXSelectJoin<CRActivity,
					InnerJoin<PMTimeActivity,
						On<PMTimeActivity.isCorrected, Equal<False>,
						And<CRActivity.noteID, Equal<PMTimeActivity.refNoteID>>>>,
					Where<CRActivity.noteID, Equal<Required<CRActivity.noteID>>>>
					.Select(_Graph, row.ParentNoteID);

			CRActivity parent = item;
			PMTimeActivity timeParent = item;
			if (timeParent != null)
			{
				timeActivity.ProjectID = timeParent.ProjectID;
				timeActivity.ProjectTaskID = timeParent.ProjectTaskID;
			}

			timeActivity.ParentTaskNoteID =
				parent != null && parent.ClassID == CRActivityClass.Task
					? parent.NoteID
					: null;

			MainCache.Update(timeActivity);
		}

		protected virtual void PMTimeActivity_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			PMTimeActivity row = (PMTimeActivity)e.Row;
			TMasterActivity masterAct = (TMasterActivity)MasterCache.Current;

			if (row == null || masterAct == null) return;

            // TimeActivity
            bool wasUsed = !string.IsNullOrEmpty(row.TimeCardCD) || row.Billed == true;

            string origTimeStatus;

			if (masterAct.ClassID == CRActivityClass.Task || masterAct.ClassID == CRActivityClass.Event)
			{
				origTimeStatus =
					(string)MasterCache.GetValueOriginal<CRActivity.uistatus>(masterAct)
					?? ActivityStatusListAttribute.Open;
			}
			else
			{
				origTimeStatus =
					(string)cache.GetValueOriginal<PMTimeActivity.approvalStatus>(row)
					?? ActivityStatusListAttribute.Open;
			}

			if (origTimeStatus == ActivityStatusListAttribute.Open)
            {
                PXUIFieldAttribute.SetEnabled(cache, row, true);

                PXUIFieldAttribute.SetEnabled<PMTimeActivity.timeBillable>(cache, row, !wasUsed && row?.IsBillable == true);
                PXUIFieldAttribute.SetEnabled<PMTimeActivity.overtimeBillable>(cache, row, !wasUsed && row?.IsBillable == true);
				PXUIFieldAttribute.SetEnabled<PMTimeActivity.projectID>(cache, row, !wasUsed);
				PXUIFieldAttribute.SetEnabled<PMTimeActivity.projectTaskID>(cache, row, !wasUsed);
				PXUIFieldAttribute.SetEnabled<PMTimeActivity.trackTime>(cache, row, !wasUsed);
			}
			else
            {
                PXUIFieldAttribute.SetEnabled(cache, row, false);                
            }            
            PXUIFieldAttribute.SetEnabled<PMTimeActivity.approvalStatus>(cache, row, row.TrackTime == true && !wasUsed && row.Released != true);
            PXUIFieldAttribute.SetEnabled<PMTimeActivity.released>(cache, row, false);

            if (row.Released == true && row.ARRefNbr == null)
			{
				CRCase crCase = PXSelect<CRCase,
					Where<CRCase.noteID, Equal<Required<CRActivity.refNoteID>>>>.Select(_Graph, masterAct.RefNoteID);

				if (crCase != null && crCase.ARRefNbr != null)
				{
					ARInvoice invoice = (ARInvoice)PXSelectorAttribute.Select<CRCase.aRRefNbr>(_Graph.Caches<CRCase>(), crCase);
					row.ARRefNbr = invoice.RefNbr;
					row.ARDocType = invoice.DocType;
				}
				if (row.ARRefNbr == null)
				{
					PMTran pmTran = PXSelect<PMTran,
						Where<PMTran.origRefID, Equal<Required<CRActivity.noteID>>>>.Select(_Graph, masterAct.NoteID);

					if (pmTran != null)
					{
						row.ARDocType = pmTran.ARTranType;
						row.ARRefNbr = pmTran.ARRefNbr;
					}
				}
			}
		}
		
		protected virtual void PMTimeActivity_RowDeleting(PXCache cache, PXRowDeletingEventArgs e)
		{
			var row = (PMTimeActivity)e.Row;
			if (row == null) return;

			if (!string.IsNullOrEmpty(row.TimeCardCD) || row.Billed == true)
				throw new PXException(EP.Messages.ActivityIsBilled);
		}

		protected virtual void PMTimeActivity_RowInserted(PXCache cache, PXRowInsertedEventArgs e)
		{
			var row = e.Row as PMTimeActivity;
			TMasterActivity activity = (TMasterActivity)MasterCache.Current;
			if (row == null || activity == null) return;
			
			row.RefNoteID = activity.NoteID;
			row.OwnerID = activity.OwnerID;
			cache.RaiseFieldUpdated<PMTimeActivity.approvalStatus>(row, null);
			if (activity.ParentNoteID != null)
			{
				var item = (PXResult<CRActivity, PMTimeActivity>)
					PXSelectJoin<CRActivity,
						InnerJoin<PMTimeActivity,
							On<PMTimeActivity.isCorrected, Equal<False>,
							And<CRActivity.noteID, Equal<PMTimeActivity.refNoteID>>>>,
						Where<CRActivity.noteID, Equal<Required<CRActivity.noteID>>>>
						.Select(_Graph, activity.ParentNoteID);

				CRActivity parent = item;
				PMTimeActivity timeParent = item;

				if (timeParent != null && timeParent.RefNoteID != null &&
				    (timeParent.ProjectID != null || timeParent.ProjectTaskID != null))
				{
					row.ProjectID = timeParent.ProjectID;
					row.ProjectTaskID = timeParent.ProjectTaskID;

					cache.RaiseFieldUpdated<PMTimeActivity.projectTaskID>(row, null);
				}

				row.ParentTaskNoteID =
					parent != null && parent.ClassID == CRActivityClass.Task
						? parent.NoteID
						: null;
			}
		}

		protected virtual void PMTimeActivity_RowInserting(PXCache cache, PXRowInsertingEventArgs e)
		{
			var row = e.Row as PMTimeActivity;
			if (row == null) return;

            var insEnum = cache.Inserted.GetEnumerator();
            if (insEnum.MoveNext())
            {
                cache.SetStatus(insEnum.Current, PXEntryStatus.InsertedDeleted);                
            }
            else
            {
                var delEnum = cache.Deleted.GetEnumerator();
                if (delEnum.MoveNext())
                {
                    row.NoteID = ((PMTimeActivity)delEnum.Current).NoteID;
                }
            }
        }

		protected virtual void PMTimeActivity_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
		{
			var row = e.Row as PMTimeActivity;
			TMasterActivity activity = (TMasterActivity)MasterCache.Current;
			if (row == null || activity == null) return;
			
			cache.SetValue<PMTimeActivity.summary>(row, activity.Subject);

			if (activity.ClassID == CRActivityClass.Task || activity.ClassID == CRActivityClass.Event)
				cache.SetValue<PMTimeActivity.trackTime>(row, false);

            row.NeedToBeDeleted = (bool?)PXFormulaAttribute.Evaluate<PMTimeActivity.needToBeDeleted>(cache, row);

            if (row.NeedToBeDeleted == true && e.Operation != PXDBOperation.Delete)
			{
				e.Cancel = true;
			}
			else
			{
			    if (row.TrackTime != true)
			        row.ApprovalStatus = ActivityStatusListAttribute.Open;
                else
                    if (row.ApprovalStatus == ActivityStatusListAttribute.Completed && 
                        row.ApproverID != null)
					    row.ApprovalStatus = ActivityStatusListAttribute.PendingApproval;
			}
		}

		protected virtual void PMTimeActivity_RowUpdated(PXCache cache, PXRowUpdatedEventArgs e)
		{
			var row = e.Row as PMTimeActivity;
			if (row == null) return;

			var isInDB = cache.GetValueOriginal<PMTimeActivity.noteID>(row) != null;

            row.NeedToBeDeleted = (bool?)PXFormulaAttribute.Evaluate<PMTimeActivity.needToBeDeleted>(cache, row);

            if (row.NeedToBeDeleted == true)
			{
				if (!isInDB)
					cache.SetStatus(row, PXEntryStatus.InsertedDeleted);
				else
					cache.SetStatus(row, PXEntryStatus.Deleted);
			}
			else if (cache.GetStatus(row) == PXEntryStatus.Updated && !isInDB)
			{
				// means "is not in DB", so move from updated to inserted
				cache.SetStatus(row, PXEntryStatus.Inserted);
			}
		}

		protected virtual void PMTimeActivity_ApprovalStatus_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			var row = e.Row as PMTimeActivity;
			TMasterActivity master = (TMasterActivity)MasterCache.Current;
			if (row == null || master == null) return;
			if (row.TrackTime == true && master.IsLocked != true && master.UIStatus != ActivityStatusListAttribute.Draft)
			{
				TMasterActivity activity = (TMasterActivity)MasterCache.CreateCopy(master);
				switch (row.ApprovalStatus)
				{
					case ActivityStatusListAttribute.Open:
						activity.UIStatus = ActivityStatusListAttribute.Open;
						break;
					case ActivityStatusListAttribute.Canceled:
						activity.UIStatus = ActivityStatusListAttribute.Canceled;
						break;
					default:
						activity.UIStatus = ActivityStatusListAttribute.Completed;
						break;
				}
				if(master.UIStatus != activity.UIStatus)
					MasterCache.Update(activity);
			}
		}
		protected virtual void PMTimeActivity_TimeSpent_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			var row = e.Row as PMTimeActivity;
			TMasterActivity activity = (TMasterActivity)MasterCache.Current;
			if (row == null || activity == null) return;

			if (activity.StartDate != null)
			{
				MasterCache.SetValue(activity, typeof(CRActivity.endDate).Name, row.TimeSpent != null
					? (DateTime?) ((DateTime) activity.StartDate).AddMinutes((int) row.TimeSpent)
					: null);
			}
		}

		protected virtual void PMTimeActivity_TrackTime_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			var row = e.Row as PMTimeActivity;
			TMasterActivity activity = (TMasterActivity)MasterCache.Current;
			if (row == null || activity == null) return;

			if (row.TrackTime != true)
			{
				cache.SetValue<PMTimeActivity.timeSpent>(row, 0);
				cache.SetValue<PMTimeActivity.timeBillable>(row, 0);
				cache.SetValue<PMTimeActivity.overtimeSpent>(row, 0);
				cache.SetValue<PMTimeActivity.overtimeBillable>(row, 0);
				cache.SetValue<PMTimeActivity.approvalStatus>(row, ActivityStatusAttribute.Completed);
				MasterCache.SetValue(activity, typeof(CRActivity.uistatus).Name, ActivityStatusAttribute.Completed);
			}			
		}

		#endregion

		private PXCache MainCache
		{
			get { return _Graph.Caches[typeof(PMTimeActivity)]; }
		}

		private PXCache MasterCache
		{
			get { return _Graph.Caches[typeof(TMasterActivity)]; }
		}

		public PMTimeActivity Current
		{
			get { return (PMTimeActivity)View.Cache.Current; }
			set { View.Cache.Current = value; }
		}

		private EPSetup EPSetupCurrent
		{
			get
			{
				var res = (EPSetup)PXSelect<EPSetup>.
					SelectWindowed(_Graph, 0, 1);
				return res ?? EmptyEpSetup;
			}
		}

	}

	#endregion

	#region CRReminderList

	public class CRReminderList<TMasterActivity> : PXSelectBase
		where TMasterActivity : CRActivity, new()
	{
		#region Ctor

		public CRReminderList(PXGraph graph)
		{
			_Graph = graph;
			
			View = new PXView(graph, false, GenerateOriginalCommand());

			graph.RowInserted.AddHandler<TMasterActivity>(Master_RowInserted);

			graph.FieldUpdated.AddHandler(typeof(TMasterActivity), typeof(CRActivity.ownerID).Name, Master_OwnerID_FieldUpdated);

			graph.RowSelected.AddHandler<CRReminder>(CRReminder_RowSelected);
			graph.RowInserting.AddHandler<CRReminder>(CRReminder_RowInserting);
			graph.RowInserted.AddHandler<CRReminder>(CRReminder_RowInserted);
			graph.RowPersisting.AddHandler<CRReminder>(CRReminder_RowPersisting);
			graph.RowUpdated.AddHandler<CRReminder>(CRReminder_RowUpdated);

			graph.FieldUpdated.AddHandler<CRReminder.isReminderOn>(CRReminder_IsReminderOn_FieldUpdated);

			PXUIFieldAttribute.SetEnabled<CRReminder.reminderDate>(MainCache, null, false);

			graph.EnsureCachePersistence(typeof(CRReminder));
		}

		public virtual object SelectSingle(params object[] parameters)
		{
			return View.SelectSingle(parameters);
		}

		#endregion

		protected virtual void Master_RowInserted(PXCache cache, PXRowInsertedEventArgs e)
		{
            using(var r = new ReadOnlyScope(MainCache))
			    this.Current = (CRReminder)MainCache.Insert();
		}

		protected virtual void Master_OwnerID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			var row = e.Row as TMasterActivity;
			CRReminder reminder = (CRReminder)Current;
			if (row == null || reminder == null) return;

			if (!cache.Graph.UnattendedMode)
			{
				var value = row.CreatedByID != row.OwnerID;

				MainCache.SetValueExt<CRReminder.isReminderOn>(reminder, value);
			}
		}

		protected virtual void CRReminder_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			var row = e.Row as CRReminder;
			TMasterActivity activity = (TMasterActivity)MasterCache.Current;
			if (row == null || activity == null) return;

			if ((string) MasterCache.GetValueOriginal(activity, typeof(CRActivity.uistatus).Name) != ActivityStatusAttribute.Completed)
			{
				PXUIFieldAttribute.SetEnabled<CRReminder.reminderDate>(cache, row, row.IsReminderOn == true);
				return;
			}

			PXUIFieldAttribute.SetEnabled(cache, row, false);
		}

		protected virtual void CRReminder_RowInserting(PXCache cache, PXRowInsertingEventArgs e)
		{
			var row = e.Row as CRReminder;
			TMasterActivity activity = (TMasterActivity)MasterCache.Current;
			if (row == null || activity == null) return;
			
			var delEnum = cache.Deleted.GetEnumerator();
			if (delEnum.MoveNext())
			{
				row.NoteID = ((CRReminder)delEnum.Current).NoteID;
			}
		}

		protected virtual void CRReminder_RowInserted(PXCache cache, PXRowInsertedEventArgs e)
		{
			var row = e.Row as CRReminder;
			TMasterActivity activity = (TMasterActivity)MasterCache.Current;
			if (row == null || activity == null) return;

			row.RefNoteID = activity.NoteID;
			row.Owner = activity.OwnerID;
		}

		protected virtual void CRReminder_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
		{
			var row = e.Row as CRReminder;
			if (row == null) return;
			


			if (row.IsReminderOn != true && e.Operation != PXDBOperation.Delete)
			{
				e.Cancel = true;
			}

			if (row.IsReminderOn == true && row.ReminderDate == null)
			{
				var reminderDateDisplayName = PXUIFieldAttribute.GetDisplayName<CRReminder.reminderDate>(MainCache);
				var exception = new PXSetPropertyException(ErrorMessages.FieldIsEmpty, reminderDateDisplayName);
				if (MainCache.RaiseExceptionHandling<CRReminder.reminderDate>(row, null, exception))
				{
					throw new PXRowPersistingException(typeof(CRReminder.reminderDate).Name, null, ErrorMessages.FieldIsEmpty, reminderDateDisplayName);
				}
			}
		}

		protected virtual void CRReminder_RowUpdated(PXCache cache, PXRowUpdatedEventArgs e)
		{
			var row = e.Row as CRReminder;
			if (row == null) return;

			if (row.IsReminderOn != true)
			{
				if (cache.GetStatus(row) == PXEntryStatus.Inserted)
					cache.SetStatus(row, PXEntryStatus.InsertedDeleted);
				else
					cache.SetStatus(row, PXEntryStatus.Deleted);
			}
		}

		protected virtual void CRReminder_IsReminderOn_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			var row = e.Row as CRReminder;
			TMasterActivity activity = (TMasterActivity)MasterCache.Current;
			if (row == null || activity == null) return;

			if (activity.ClassID == CRActivityClass.Task)
			{
				if (row.IsReminderOn == true)
				{
					cache.SetValue<CRReminder.reminderDate>(row, row.ReminderDate ?? activity.StartDate?.AddMinutes(15) ?? row.LastModifiedDateTime?.AddMinutes(15));
				}
			}
		}

		public static BqlCommand GenerateOriginalCommand()
		{
			var createdDateTimeField = typeof(CRReminder).GetNestedType(typeof(CRReminder.createdDateTime).Name);
			var noteIDField = typeof(TMasterActivity).GetNestedType(typeof(CRActivity.noteID).Name);

			return BqlCommand.CreateInstance(
				typeof(Select<,,>),
					typeof(CRReminder),
				typeof(Where<,>),
					typeof(CRReminder.refNoteID), typeof(Equal<>), typeof(Current<>), noteIDField,
				typeof(OrderBy<>),
					typeof(Desc<>), createdDateTimeField);
		}

		private PXCache MainCache
		{
			get { return _Graph.Caches[typeof(CRReminder)]; }
		}

		private PXCache MasterCache
		{
			get { return _Graph.Caches[typeof(TMasterActivity)]; }
		}

		public CRReminder Current
		{
			get { return (CRReminder)View.Cache.Current; }
			set { View.Cache.Current = value; }
		}
	}

	#endregion

	#region CRReferencedTaskList

	public class CRReferencedTaskList<TPrimaryView> : PXSelectBase
		where TPrimaryView : class, IBqlTable
	{
		private const string _VIEWTASK_COMMAND = "ViewTask";

		public CRReferencedTaskList(PXGraph graph)
		{
			_Graph = graph;

			AddActions();
			AddEventHandlers();

			View = new PXView(graph, false, GetCommand(), new PXSelectDelegate(Handler));
			_Graph.EnsureCachePersistence(typeof(CRActivityRelation));
		}

		private void AddEventHandlers()
		{
			_Graph.FieldUpdating.AddHandler<CRActivityRelation.subject>((sender, e) => e.Cancel = true);
			_Graph.FieldUpdating.AddHandler<CRActivityRelation.startDate>((sender, e) => e.Cancel = true);
			_Graph.FieldUpdating.AddHandler<CRActivityRelation.endDate>((sender, e) => e.Cancel = true);
			_Graph.FieldUpdating.AddHandler<CRActivityRelation.completedDateTime>((sender, e) => e.Cancel = true);
			_Graph.FieldUpdating.AddHandler<CRActivityRelation.status>((sender, e) => e.Cancel = true);
			_Graph.FieldDefaulting.AddHandler<CRActivityRelation.noteID>(
				(sender, e) =>
					{
						var cache = _Graph.Caches[typeof(CRActivity)];
						var noteIDFieldName = cache.GetField(typeof(CRActivity).GetNestedType(typeof(CRActivity.noteID).Name));
						e.NewValue = cache.GetValue(cache.Current, noteIDFieldName);
					});
			_Graph.RowInserted.AddHandler<CRActivityRelation>((sender, e) => FillReadonlyValues(e.Row as CRActivityRelation));
			_Graph.RowUpdated.AddHandler<CRActivityRelation>((sender, e) => FillReadonlyValues(e.Row as CRActivityRelation));
			_Graph.RowPersisting.AddHandler<CRActivityRelation>(
				(sender, e) =>
					{
						if ((e.Row as CRActivityRelation).With(_ => _.RefNoteID) == null) e.Cancel = true;
					});
			_Graph.RowSelected.AddHandler<TPrimaryView>(
				(sender, e) =>
					{
						var tasksCache = sender.Graph.Caches[typeof(CRActivityRelation)];
						var isInserted = sender.GetStatus(e.Row) == PXEntryStatus.Inserted;
						var row = e.Row as CRActivity;
						var isEditable = row != null && row.UIStatus == ActivityStatusAttribute.Open;

						tasksCache.AllowDelete = 
							tasksCache.AllowUpdate = 
								tasksCache.AllowInsert =
									!isInserted && isEditable;
					});
		}

		private void FillReadonlyValues(CRActivityRelation row)
		{
			if (row == null || row.RefNoteID == null) return;

			var act = (CRActivity)PXSelect<CRActivity,
						Where<CRActivity.noteID, Equal<Required<CRActivity.noteID>>>>.
						Select(_Graph, row.RefNoteID);
			if (act != null && act.NoteID != null)
			{
				row.Subject = act.Subject;
				row.StartDate = act.StartDate;
				row.EndDate = act.EndDate;
				row.Status = act.UIStatus;
				row.CompletedDateTime = act.CompletedDate;
			}
		}

		private void AddActions()
		{
			AddAction(_Graph, _VIEWTASK_COMMAND, Messages.ViewActivity, ViewTask);
		}

		private PXAction AddAction(PXGraph graph, string name, string displayName, PXButtonDelegate handler)
		{
			var uiAtt = new PXUIFieldAttribute
			{
				DisplayName = PXMessages.LocalizeNoPrefix(displayName),
				MapEnableRights = PXCacheRights.Select
			};
			var res = (PXAction)Activator.CreateInstance(typeof(PXNamedAction<>).MakeGenericType(
				new[] { typeof(TPrimaryView) }), new object[] { graph, name, handler, new PXEventSubscriberAttribute[] { uiAtt } });
			graph.Actions[name] = res;
			return res;
		}

		private IEnumerable Handler()
		{
			BqlCommand command = View.BqlSelect;
			var startRow = PXView.StartRow;
			int totalRows = 0;
			var list = new PXView(PXView.CurrentGraph, false, command).
				Select(null, null, PXView.Searches, PXView.SortColumns, PXView.Descendings, PXView.Filters,
					   ref startRow, PXView.MaximumRows, ref totalRows);
			PXView.StartRow = 0;
			foreach (PXResult<CRActivityRelation, CRActivity> record in list)
			{
				var row = (CRActivityRelation)record[typeof(CRActivityRelation)];
				var act = (CRActivity) record[typeof(CRActivity)];
				var status = View.Cache.GetStatus(row);

				if (status == PXEntryStatus.Inserted || status == PXEntryStatus.Updated)
				{
					act = PXSelect<CRActivity,
						Where<CRActivity.noteID, Equal<Required<CRActivity.noteID>>>>.
						Select(_Graph, row.RefNoteID);
				}

				if (act != null && act.NoteID != null)
				{
					row.Subject = act.Subject;
					row.StartDate = act.StartDate;
					row.EndDate = act.EndDate;
					row.Status = act.UIStatus;
					row.CompletedDateTime = act.CompletedDate;
				}
				yield return row;
			}
		}

		private static BqlCommand GetCommand()
		{
			Type noteID = typeof (TPrimaryView).GetNestedType(typeof (CRActivity.noteID).Name);
			
			return BqlCommand.CreateInstance(
				typeof(Select2<,,,>),
					typeof(CRActivityRelation),
				typeof(InnerJoin<,>),
					typeof(CRActivity), typeof(On<,>), typeof(CRActivity.noteID), typeof(Equal<>), typeof(CRActivityRelation.refNoteID),
				typeof(Where<,>),
					typeof(CRActivityRelation.noteID), typeof(Equal<>), typeof(Current<>), noteID,
				typeof(OrderBy<>),
					typeof(Asc<>), typeof(CRActivityRelation.refNoteID));
		}

		[PXUIField(Visible = false)]
		[PXButton]
		public virtual IEnumerable ViewTask(PXAdapter adapter)
		{
			var current = (CRActivity)PXSelect<CRActivity,
				Where<CRActivity.noteID, Equal<Current<CRActivityRelation.refNoteID>>>>.
				Select(_Graph);
			if(current != null)
				PXRedirectHelper.TryRedirect(_Graph.Caches[typeof(CRActivity)].Graph, current, PXRedirectHelper.WindowMode.Same);

			return adapter.Get();
		}

		protected PXCache CreateInstanceCache<TNode>(Type graphType)
			where TNode : IBqlTable
		{
			if (graphType != null)
			{
				if (_Graph.IsDirty)
					_Graph.Actions.PressSave();

				var graph = PXGraph.CreateInstance(graphType);
				graph.Clear();
				foreach (Type type in graph.Views.Caches)
				{
					var cache = graph.Caches[type];
					if (typeof(TNode).IsAssignableFrom(cache.GetItemType()))
						return cache;
				}
			}
			return null;
		}
	}

	#endregion

	#region Email

	public struct Email
	{
		private static Regex _emailRegex = new Regex("(?<Name>[^;]+?)\\s*\\((?<Address>[^;]*?)\\)",
			RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

		private readonly string _address;
		private readonly string _displayName;

		private string _toString;

		public static readonly Email Empty = new Email(null, null);

		public Email(string displayName, string address)
		{
			_displayName = displayName;
			if (_displayName != null) _displayName = _displayName.Replace(';', ' '); //.Replace(',', ' ');
			_address = address ?? string.Empty;

			_toString = null;
		}

		public string Address
		{
			get { return _address; }
		}

		public string DisplayName
		{
			get { return _displayName; }
		}

		public override string ToString()
		{
			if (_toString == null)
			{
				var isAddressComplex = _emailRegex.IsMatch(_address);
				_toString = isAddressComplex || string.IsNullOrEmpty(_displayName)
								? CorrectEmailAddress(_address)
								: TextUtils.QuoteString(_displayName )+ " <" + _address + ">";
			}
			return _toString;
		}

		private static string CorrectEmailAddress(string address)
		{
			var sb = new StringBuilder(address.Length * 2);
			foreach (string str in address.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
			{
				var cStr = str.Trim();
				if (string.IsNullOrEmpty(cStr)) continue;
				if (_emailRegex.IsMatch(address))
				{
					sb.Append(cStr);
				}
				else
				{
					var value = cStr.Replace('<', '(').Replace('>', ')');
					sb.AppendFormat("{0} <{0}>", value);
				}
				sb.Append(';');
			}
			return sb.ToString();
		}
	}

	#endregion

	#region NotificationUtility

	public sealed class NotificationUtility
	{
		private readonly PXGraph _graph;

		public NotificationUtility(PXGraph graph)
		{
			_graph = graph;
		}

		public Guid? SearchSetupID(string source, string notificationCD)
		{
			if (source == null || notificationCD == null) return null;
			NotificationSetup setup =
			PXSelect<NotificationSetup,
				Where<NotificationSetup.sourceCD, Equal<Required<NotificationSetup.sourceCD>>,
					And<NotificationSetup.notificationCD, Equal<Required<NotificationSetup.notificationCD>>>>>
					.SelectWindowed(_graph, 0, 1, source, notificationCD);
			return setup == null ? null : setup.SetupID;
		}

		public string SearchReport(string source, object row, string reportID, int? branchID)
		{
			if (source == null) return reportID;
			NotificationSetup setup =
			PXSelect<NotificationSetup,
				Where<NotificationSetup.sourceCD, Equal<Required<NotificationSetup.sourceCD>>,
					And<NotificationSetup.reportID, Equal<Required<NotificationSetup.reportID>>>>>
					.SelectWindowed(_graph, 0, 1, source, reportID);

			if (setup == null) return reportID;

			NotificationSource notification = GetSource(source, row, (Guid)setup.SetupID, branchID);

			return notification == null || notification.ReportID == null
				? reportID :
				notification.ReportID;
		}

	    public NotificationSource GetSource(object row, Guid setupID, int? branchID)
	    {
			return GetSource(null, row, setupID, branchID);
	    }

        public NotificationSource GetSource(string sourceType, object row, Guid setupID, int? branchID)
		{
			if (row == null) return null;
			PXGraph graph = CreatePrimaryGraph(sourceType, row);
			NavigateRow(graph, row);

			PXView notificationView = null;
			graph.Views.TryGetValue("NotificationSources", out notificationView);

			if (notificationView == null)
			{
				foreach (PXView view in graph.GetViewNames().Select(name => graph.Views[name]).Where(view => typeof(NotificationSource).IsAssignableFrom(view.GetItemType())))
				{
					notificationView = view;
					break;
				}
			}
			if (notificationView == null) return null;
			NotificationSource result = null;
			foreach (NotificationSource rec in notificationView.SelectMulti())
			{
				if (rec.SetupID == setupID && rec.NBranchID == branchID)
					return rec;
				if(rec.SetupID == setupID && rec.NBranchID == null)
					result = rec;
			}
			return result;
		}

	    public RecipientList GetRecipients(object row, NotificationSource source)
	    {
	        return GetRecipients(null, row, source);
	    }

        public RecipientList GetRecipients(string type, object row, NotificationSource source)
		{
			if (row == null) return null;
			PXGraph graph = CreatePrimaryGraph(type, row);
			NavigateRow(graph, row);
			NavigateRow(graph, source, false);


			PXView recipientView;
			graph.Views.TryGetValue("NotificationRecipients", out recipientView);

			if (recipientView == null)
			{
				foreach (PXView view in graph.GetViewNames().Select(name => graph.Views[name]).Where(view => typeof (NotificationRecipient).IsAssignableFrom(view.GetItemType())))
				{
					recipientView = view;
					break;
				}
			}
			if (recipientView != null)
			{
				RecipientList recipient = null;
				Dictionary<string, string> errors = new Dictionary<string, string>();
				int count = 0;
				foreach (NotificationRecipient item in recipientView.SelectMulti())
				{
					NavigateRow(graph, item, false);
					if (item.Active == true)
					{
						count++;
						if (string.IsNullOrWhiteSpace(item.Email))
						{
							string currEmail = ((NotificationRecipient) graph.Caches[typeof (NotificationRecipient)].Current).Email;
							if (string.IsNullOrWhiteSpace(currEmail))
							{
								Contact contact = PXSelect<Contact, Where<Contact.contactID, Equal<Current<NotificationRecipient.contactID>>>>.SelectSingleBound(_graph, new object[] {item});
								NotificationContactType.ListAttribute list = new NotificationContactType.ListAttribute();
								StringBuilder display = new StringBuilder(list.ValueLabelDic[item.ContactType]);
								if (contact != null)
								{
									display.Append(" ");
									display.Append(contact.DisplayName);
								}
								errors.Add(count.ToString(CultureInfo.InvariantCulture), PXMessages.LocalizeFormatNoPrefix(Messages.EmptyEmail, display));
							}
							else
							{
								item.Email = currEmail;
							}
						}
						if (!string.IsNullOrWhiteSpace(item.Email))
						{
							if (recipient == null)
								recipient = new RecipientList();
							recipient.Add(item);
						}
					}
				}
				if (errors.Any())
				{
					NotificationSetup nsetup = PXSelect<NotificationSetup, Where<NotificationSetup.setupID, Equal<Current<NotificationSource.setupID>>>>.SelectSingleBound(_graph, new object[] {source});
					throw new PXOuterException(errors, _graph.GetType(), row, Messages.InvalidRecipients, errors.Count, count, nsetup.NotificationCD, nsetup.Module);
				}
				else
				{
					return recipient;
				}
			}
			return null;
		}

		private PXGraph CreatePrimaryGraph(string source, object row)
		{
			Type graphType =  null;
            if(source == ARNotificationSource.Customer)
                graphType = typeof(CustomerMaint);
            else if (source == APNotificationSource.Vendor)
                graphType = typeof(VendorMaint);
            else 
                graphType = new EntityHelper(_graph).GetPrimaryGraphType(row, false);

			if (graphType == null)
				throw new PXException(PX.SM.Messages.NotificationGraphNotFound);

			var res = graphType == _graph.GetType()
				? _graph
				: (PXGraph)PXGraph.CreateInstance(graphType);
			return res;
		}

		private static void NavigateRow(PXGraph graph, object row, bool primaryView = true)
		{
			Type type = row.GetType();
			PXCache primary = graph.Views[graph.PrimaryView].Cache;
			if (primary.GetItemType().IsAssignableFrom(row.GetType()))
			{
				graph.Caches[type].Current = row;
			}
			else if (row.GetType().IsAssignableFrom(primary.GetItemType()))
			{
				object current = primary.CreateInstance();
				PXCache parent = (PXCache)Activator.CreateInstance(typeof(PXCache<>).MakeGenericType(row.GetType()), primary.Graph);
				parent.RestoreCopy(current, row);
				primary.Current = current;
			}
			else
			if (primaryView)
			{
				object[] searches = new object[primary.Keys.Count];
				string[] sortcolumns = new string[primary.Keys.Count];
				for(int i=0 ;i < primary.Keys.Count; i++)
				{
					searches[i] = graph.Caches[type].GetValue(row, primary.Keys[i]);
					sortcolumns[i] = primary.Keys[i];
				}
				int startRow = 0, totalRows = 0;
				graph.Views[graph.PrimaryView].Select(null, null, searches, sortcolumns, null, null, ref startRow, 1, ref totalRows);
			}
			else
			{
				primary = graph.Caches[type];
				object current = primary.CreateInstance();
				PXCache parent = (PXCache)Activator.CreateInstance(typeof(PXCache<>).MakeGenericType(type), primary.Graph);
				parent.RestoreCopy(current, row);
				primary.Current = current;
			}
		}
	}

	#endregion

	#region CRNotificationSetupList

	public class CRNotificationSetupList<Table> : PXSelect<Table>
		where Table : NotificationSetup, new()		
	{
		public CRNotificationSetupList(PXGraph graph)
			: base(graph)
		{
			graph.Views.Caches.Add(typeof(NotificationSource));
			graph.Views.Caches.Add(typeof(NotificationRecipient));
			graph.RowDeleted.AddHandler(typeof(Table), OnRowDeleted);
            graph.RowPersisting.AddHandler(typeof(Table), OnRowPersisting);
		}

		protected virtual void OnRowDeleted(PXCache cache, PXRowDeletedEventArgs e)
		{
			NotificationSetup row = (NotificationSetup)e.Row;

			PXCache source = cache.Graph.Caches[typeof(NotificationSource)];
			foreach (NotificationSource item in
				PXSelect<NotificationSource,
			Where<NotificationSource.setupID, Equal<Required<NotificationSource.setupID>>>>.Select(cache.Graph, row.SetupID))
			{
				source.Delete(item);
			}

			PXCache recipient = cache.Graph.Caches[typeof(NotificationRecipient)];
			foreach (NotificationRecipient item in
				PXSelect<NotificationRecipient,
			Where<NotificationRecipient.setupID, Equal<Required<NotificationRecipient.setupID>>>>.Select(cache.Graph, row.SetupID))
			{
				recipient.Delete(item);
			}
		}

	    protected virtual void OnRowPersisting(PXCache cache, PXRowPersistingEventArgs e)
	    {
	        NotificationSetup row = (NotificationSetup)e.Row;
	        if (row != null && row.NotificationCD == null)
	        {
	            cache.RaiseExceptionHandling<NotificationSetup.notificationCD>(e.Row, row.NotificationCD,
                    new PXSetPropertyException(PXMessages.LocalizeFormatNoPrefix(Messages.EmptyValueErrorFormat, PXUIFieldAttribute.GetDisplayName<NotificationSetup.notificationCD>(cache)), 
                        PXErrorLevel.RowError, typeof(NotificationSetup.notificationCD).Name));
	        }
	    }

	}

	#endregion

	#region CRClassNotificationSourceList

	public class CRClassNotificationSourceList<Table, ClassID, SourceCD> : PXSelect<Table>
		where Table : NotificationSource, new()		
		where ClassID : IBqlField
		where SourceCD : Constant<string>
	{
		public CRClassNotificationSourceList(PXGraph graph)
			: base(graph)
		{						
			this.View = new PXView(graph, false,
				BqlCommand.CreateInstance(
				BqlCommand.Compose(
				typeof(Select2<,,,>), typeof(Table),
				typeof(InnerJoin<,>), typeof(NotificationSetup), 
				typeof(On<,,>), typeof(NotificationSetup.setupID), typeof(Equal<>), typeof(Table).GetNestedType(typeof(NotificationSource.setupID).Name),
				typeof(And<,>), typeof(NotificationSetup.sourceCD), typeof(Equal<>), typeof(SourceCD),
				typeof(Where<,>), typeof(Table).GetNestedType(typeof(NotificationSource.classID).Name), typeof(Equal<>), typeof(Optional<>), typeof(ClassID),
				typeof(OrderBy<>),
				typeof(Asc<>),
				typeof(Table).GetNestedType(typeof(NotificationSource.setupID).Name))));
		
			this.setupNotifications = 
				new PXView(graph, false,
				BqlCommand.CreateInstance(
				BqlCommand.Compose(typeof(Select<,>), typeof(NotificationSetup),
				typeof(Where<,>), typeof(NotificationSetup.sourceCD), typeof(Equal<>), typeof(SourceCD))));

			graph.RowInserted.AddHandler(BqlCommand.GetItemType(typeof(ClassID)), OnClassRowInserted);
            graph.RowUpdated.AddHandler(BqlCommand.GetItemType(typeof(ClassID)), OnClassRowUpdated);
			graph.RowInserted.AddHandler<NotificationSource>(OnSourceRowInseted);						
		}
		private PXView setupNotifications;
		protected virtual void OnClassRowInserted(PXCache cache, PXRowInsertedEventArgs e)
		{			
			if (e.Row == null || cache.GetValue(e.Row, typeof(ClassID).Name) == null) return;

			foreach (NotificationSetup n in	setupNotifications.SelectMulti())
			{
				Table source = new Table();
				source.SetupID = n.SetupID;
				this.Cache.Insert(source);
			}
		}
        public virtual void OnClassRowUpdated(PXCache cache, PXRowUpdatedEventArgs e)
        {
            if (cache.Graph.IsCopyPasteContext)
            {
                foreach (object source in this.Select())
                {
                    this.Cache.Delete(source);
                }
            }
        }
		protected virtual void OnSourceRowInseted(PXCache cache, PXRowInsertedEventArgs e)
		{
            if (cache.Graph.IsCopyPasteContext)
                return;
			NotificationSource source = (NotificationSource)e.Row;
			PXCache rCache = cache.Graph.Caches[typeof(NotificationRecipient)];
			foreach (NotificationSetupRecipient r in
					PXSelect<NotificationSetupRecipient,
					Where<NotificationSetupRecipient.setupID, Equal<Required<NotificationSetupRecipient.setupID>>>>
					.Select(cache.Graph, source.SetupID))
			{
				try
				{
					NotificationRecipient rec = (NotificationRecipient)rCache.CreateInstance();
					rec.SetupID = source.SetupID;
					rec.ContactType = r.ContactType;
					rec.ContactID = r.ContactID;
					rec.Active = r.Active;
					rec.Hidden = r.Hidden;
					rCache.Insert(rec);
				}
				catch(Exception ex)
				{
					PXTrace.WriteError(ex);
				}
			}
		}
	}

	#endregion

	#region CRNotificationSourceList

	public class CRNotificationSourceList<Source, SourceClass, NotificationType> : EPDependNoteList<NotificationSource, NotificationSource.refNoteID, Source>
		where Source : class, IBqlTable	
		where SourceClass : class, IBqlField
		where NotificationType : class, IBqlOperand
	{
		protected readonly PXView _SourceView;
		protected readonly PXView _ClassView;

		public CRNotificationSourceList(PXGraph graph)
			: base(graph)
		{			
			this.View = new PXView(graph, false,
				BqlCommand.CreateInstance(
				BqlCommand.Compose(
				typeof(Select<,,>), typeof(NotificationSource),
				typeof(Where<boolTrue, Equal<boolTrue>>),
				typeof(OrderBy<>),
				typeof(Asc<>),
				typeof(NotificationSource).GetNestedType(typeof(NotificationSource.setupID).Name))),
				new PXSelectDelegate(NotificationSources));

			_SourceView = new PXView(graph, false,
				BqlCommand.CreateInstance(
				BqlCommand.Compose(typeof(Select<,>), typeof(NotificationSource), ComposeWhere)));



			_ClassView = new PXView(graph, true,
									BqlCommand.CreateInstance(BqlCommand.Compose(
										typeof (Select2<,,>),
										typeof (NotificationSource),
										typeof (InnerJoin<NotificationSetup, On<NotificationSetup.setupID, Equal<NotificationSource.setupID>>>),
										typeof (Where<,,>), typeof (NotificationSetup.sourceCD), typeof (Equal<>), typeof (NotificationType),
																typeof(And<,>), typeof(NotificationSource.classID), typeof(Equal<>), typeof(Optional<>), typeof(SourceClass))));

			graph.RowPersisting.AddHandler<NotificationSource>(OnRowPersisting);
			graph.RowDeleting.AddHandler<NotificationSource>(OnRowDeleting);
			graph.RowUpdated.AddHandler<NotificationSource>(OnRowUpdated);
			graph.RowSelected.AddHandler<NotificationSource>(OnRowSelected);
			
		}		
		protected virtual IEnumerable NotificationSources()
		{
			List<NotificationSource> result = new List<NotificationSource>();
			foreach(NotificationSource item in _SourceView.SelectMulti())
			{
				result.Add(item);
			}
			foreach (NotificationSource classItem in GetClassItems())
			{								
				if (result.Find(i => i.SetupID == classItem.SetupID && i.NBranchID == classItem.NBranchID) == null)
					result.Add(classItem);
			}						
			return result;
		}

		protected override void Source_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{			
			sender.Current = e.Row;
			internalDelete = true;
			try
			{
				foreach (NotificationSource item in _SourceView.SelectMulti())
				{
					this._SourceView.Cache.Delete(item);
				}
			}
			finally
			{
				internalDelete = false;
			}
		}
		private bool internalDelete;
		protected virtual void OnRowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			if(!sender.ObjectsEqual<NotificationSource.overrideSource>(e.Row, e.OldRow))
			{
				NotificationSource row = (NotificationSource) e.Row;
				if (row.OverrideSource == false)
				{
					internalDelete = true;
					sender.Delete(row);
					this.View.RequestRefresh();
				}
			}
		}
		protected virtual void OnRowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			if (internalDelete) return;
			NotificationSource row = (NotificationSource)e.Row;
			foreach(NotificationSource classItem in GetClassItems())
				if (classItem.SetupID == row.SetupID && classItem.NBranchID == row.NBranchID && row.OverrideSource == false)
				{
					e.Cancel = true;
					throw new PXRowPersistingException(typeof(NotificationSource).Name, null, Messages.DeleteClassNotification);
				}	
			if(!e.Cancel)
				this.View.RequestRefresh();
		}
		protected virtual void OnRowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{			
			NotificationSource row = (NotificationSource)e.Row;
			if(row.ClassID != null)
			{
				if (e.Operation == PXDBOperation.Delete)
					e.Cancel = true;

				if (e.Operation == PXDBOperation.Update)
				{
					sender.SetStatus(row, PXEntryStatus.Deleted);
					NotificationSource ins = (NotificationSource)sender.CreateInstance();
					ins.SetupID = row.SetupID;
					ins.NBranchID = row.NBranchID;
					ins = (NotificationSource)sender.Insert(ins);

					NotificationSource clone = PXCache<NotificationSource>.CreateCopy(row);
					clone.NBranchID = ins.NBranchID;
					clone.SourceID  = ins.SourceID;
					clone.RefNoteID = ins.RefNoteID;
					clone.ClassID  = null;					
					clone = (NotificationSource)sender.Update(clone);
					if (clone != null)
					{						
						sender.PersistInserted(clone);						
						sender.Normalize();
						sender.SetStatus(clone, PXEntryStatus.Notchanged);
						PXCache source = sender.Graph.Caches[BqlCommand.GetItemType(SourceNoteID)];
						Guid? refNoteID = (Guid?)source.GetValue(source.Current, SourceNoteID.Name);
						if (refNoteID != null)
						{
							foreach (NotificationRecipient r in PXSelect<NotificationRecipient,
							Where<NotificationRecipient.sourceID, Equal<Required<NotificationRecipient.sourceID>>,
							  And<NotificationRecipient.refNoteID, Equal<Required<NotificationRecipient.refNoteID>>,
								And<NotificationRecipient.classID, IsNotNull>>>>
							.Select(sender.Graph, row.SourceID, refNoteID))
							{
                                PXCache cache = sender.Graph.Caches[typeof(NotificationRecipient)];
                                if (cache.GetStatus(r) == PXEntryStatus.Inserted)
                                {
                                    NotificationRecipient u1 = (NotificationRecipient)cache.CreateCopy(r);
                                    u1.SourceID = clone.SourceID;
                                    cache.Update(u1);
                                    cache.PersistInserted(u1);
                                }

                                if (cache.GetStatus(r) == PXEntryStatus.Updated ||
									cache.GetStatus(r) == PXEntryStatus.Inserted) continue;
								NotificationRecipient u = (NotificationRecipient)cache.CreateCopy(r);
								u.SourceID = clone.SourceID;
								u.ClassID  = null;
								cache.Update(u);                                
                            }
						}
					}
					e.Cancel = true;
				}				
			}
		}
		public virtual void OnRowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			if (e.Row == null) return;
			NotificationSource row = (NotificationSource)e.Row;
			bool classitem = GetClassItems().Any(cs => cs.SourceID == row.SourceID);
			PXUIFieldAttribute.SetEnabled<NotificationSource.overrideSource>(sender, row, !classitem);
			PXUIFieldAttribute.SetEnabled<NotificationSource.setupID>(sender, row, !classitem);			
		}

		private IEnumerable<NotificationSource> GetClassItems()
		{
			foreach (object rec in _ClassView.SelectMulti())
			{
				NotificationSource classItem =
					(rec is PXResult && ((PXResult)rec)[0] is NotificationSource) ? (NotificationSource)((PXResult)rec)[0] :
					(rec is NotificationSource ? (NotificationSource)rec : null);
				if (classItem == null) continue;
				yield return classItem;
			}
		}
	}

	#endregion

	#region CRNotificationRecipientList

	public class CRNotificationRecipientList<Source, SourceClassID> : EPDependNoteList<NotificationRecipient, NotificationRecipient.refNoteID, Source> 
		where Source : class, IBqlTable
		where SourceClassID : class, IBqlField
	{
		protected readonly PXView _SourceView;
		protected readonly PXView _ClassView;

		public CRNotificationRecipientList(PXGraph graph)
			: base(graph)
		{
			Type table = typeof(NotificationRecipient);
			Type where = BqlCommand.Compose(
				typeof(Where<,,>),typeof(NotificationRecipient.sourceID), typeof(Equal<Optional<NotificationSource.sourceID>>),
				typeof(And<,>),typeof(NotificationRecipient.refNoteID), typeof(Equal<>), typeof(Current<>), this.SourceNoteID);

			this.View = new PXView(graph, false,
				BqlCommand.CreateInstance(
				BqlCommand.Compose(
				typeof(Select<,,>), table,
				typeof(Where<boolTrue, Equal<boolTrue>>),
				typeof(OrderBy<>),
				typeof(Asc<>),
				table.GetNestedType(typeof(NotificationRecipient.orderID).Name))),
				new PXSelectDelegate(NotificationRecipients));

			_SourceView = new PXView(graph, false,
				BqlCommand.CreateInstance(
				BqlCommand.Compose(typeof(Select<,>), typeof(NotificationRecipient), where)));

	

			_ClassView = new PXView(graph, true,
									BqlCommand.CreateInstance(BqlCommand.Compose(
										typeof (Select<,>), typeof (NotificationRecipient),
										typeof (Where<,,>), typeof (NotificationRecipient.classID), typeof (Equal<>), typeof (Current<>), typeof (SourceClassID),
										typeof (And<,,>), typeof (NotificationRecipient.setupID), typeof (Equal<Current<NotificationSource.setupID>>),
										typeof (And<NotificationRecipient.refNoteID, IsNull>)
																  )));

			graph.RowPersisting.AddHandler<NotificationRecipient>(OnRowPersisting);
			graph.RowSelected.AddHandler<NotificationRecipient>(OnRowSeleted);
			graph.RowDeleting.AddHandler<NotificationRecipient>(OnRowDeleting);
			graph.RowUpdated.AddHandler<NotificationRecipient>(OnRowUpdated);
			graph.RowInserting.AddHandler<NotificationRecipient>(OnRowInserting);
			graph.RowUpdating.AddHandler<NotificationRecipient>(OnRowUpdating);
		}


		protected virtual IEnumerable NotificationRecipients()
		{
			var result = new List<NotificationRecipient>();			
			foreach (NotificationRecipient item in _SourceView.SelectMulti())
			{
				item.OrderID = item.NotificationID;
				result.Add(item);
			}
			
			foreach (NotificationRecipient classItem in GetClassItems())
			{
				NotificationRecipient item = result.Find(i =>
					i.ContactType == classItem.ContactType &&
					i.ContactID == classItem.ContactID);
				if (item == null)
				{
					item = classItem;
					result.Add(item);
				}
				item.OrderID = int.MinValue + classItem.NotificationID;
			}
			return result;
		}

		protected override void Source_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			sender.Current = e.Row;
			internalDelete = true;
			try
			{
				foreach (NotificationRecipient item in _SourceView.SelectMulti())
				{
					this._SourceView.Cache.Delete(item);
				}
			}
			finally
			{
				internalDelete = false;
			}
		}
		private bool internalDelete;

		protected virtual void OnRowSeleted(PXCache sender, PXRowSelectedEventArgs e)
		{
			NotificationRecipient row = (NotificationRecipient)e.Row;
			if (row == null) return;
			bool updatableContractID = 
				(row.ContactType == NotificationContactType.Contact ||
			   row.ContactType == NotificationContactType.Employee);
			bool updatableContactType = !GetClassItems().Any(classItem => row.ContactType == classItem.ContactType &&
																		  row.ContactID == classItem.ContactID);

			PXUIFieldAttribute.SetEnabled(sender, row, typeof(NotificationRecipient.contactID).Name, updatableContactType && updatableContractID);
			PXUIFieldAttribute.SetEnabled(sender, row, typeof(NotificationRecipient.contactType).Name, updatableContactType);
		}
		protected virtual void OnRowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			NotificationRecipient row = (NotificationRecipient)e.Row;

            if (e.Operation == PXDBOperation.Insert)
            {
                NotificationSource source =
                        PXSelectReadonly<NotificationSource,
                        Where<NotificationSource.setupID, Equal<Required<NotificationSource.setupID>>,
                            And<NotificationSource.refNoteID, Equal<Required<NotificationSource.refNoteID>>>>>.
                            Select(sender.Graph, row.SetupID, row.RefNoteID);
                if (source != null)
                {
                    if (sender.Graph.Caches[typeof(NotificationSource)].GetStatus(source) == PXEntryStatus.Updated)
                    {
                        e.Cancel = true;
                    }
                }                
            }


            if (row.ClassID != null)
			{
                if (e.Operation == PXDBOperation.Update)
				{
					sender.SetStatus(row, PXEntryStatus.Deleted);
					NotificationRecipient ins   = (NotificationRecipient)sender.Insert();
					NotificationRecipient clone = PXCache<NotificationRecipient>.CreateCopy(row);
					clone.NotificationID = ins.NotificationID;
					clone.RefNoteID      = ins.RefNoteID;
					clone.ClassID        = null;
					NotificationSource source = 
						PXSelectReadonly<NotificationSource,
						Where<NotificationSource.setupID, Equal<Required<NotificationSource.setupID>>,
							And<NotificationSource.refNoteID, Equal<Required<NotificationSource.refNoteID>>>>>.
							Select(sender.Graph, row.SetupID, row.RefNoteID);
					if (source != null)
						clone.SourceID = source.SourceID;

					clone = (NotificationRecipient)sender.Update(clone);					
					if (clone != null)
					{						
						sender.PersistInserted(clone);						
						sender.Normalize();
						sender.SetStatus(clone, PXEntryStatus.Notchanged);						
					}
					e.Cancel = true;
				}
			}			 
		}
		protected virtual void OnRowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			if (internalDelete) return;
			NotificationRecipient row = (NotificationRecipient)e.Row;
			foreach (NotificationRecipient classItem in GetClassItems())
				if (classItem.SetupID			== row.SetupID && 
					  classItem.ContactType == row.ContactType && 
						classItem.ContactID		== row.ContactID)
				{
					if (row.RefNoteID == null)
					{
						e.Cancel = true;
						throw new PXRowPersistingException(typeof (NotificationRecipient).Name, null, Messages.DeleteClassNotification);
					}
				}
			if(!e.Cancel)	
				this.View.RequestRefresh();
		}
		protected virtual void OnRowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			NotificationRecipient row = (NotificationRecipient)e.Row;
			PXCache source = sender.Graph.Caches[BqlCommand.GetItemType(SourceNoteID)];
			row.RefNoteID = (Guid?)source.GetValue(source.Current, SourceNoteID.Name);
		}
		protected virtual void OnRowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			if (e.Row != null)
			{
				e.Cancel = !sender.Graph.IsImport && !ValidateDuplicates(sender, (NotificationRecipient)e.Row, null);
				if (e.Cancel != true)
				{
					NotificationRecipient r = (NotificationRecipient)e.Row;
					NotificationSource source = (NotificationSource)sender.Graph.Caches[typeof(NotificationSource)].Current;
					r.ClassID = source != null ? source.ClassID : null;
				}
			}
		}

		protected virtual void OnRowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
		{
			if (e.Row != null && e.NewRow != null && CheckUpdated(sender, (NotificationRecipient)e.Row, (NotificationRecipient)e.NewRow))
				e.Cancel = !ValidateDuplicates(sender, (NotificationRecipient)e.NewRow, (NotificationRecipient)e.Row);
		}
		private IEnumerable<NotificationRecipient> GetClassItems()
		{
			foreach (object rec in _ClassView.SelectMulti())
			{
				NotificationRecipient classItem =
					(rec is PXResult && ((PXResult)rec)[0] is NotificationRecipient) ? (NotificationRecipient)((PXResult)rec)[0] :
					(rec is NotificationRecipient ? (NotificationRecipient)rec : null);
				if (classItem == null) continue;
				yield return classItem;
			}
		}
		private bool CheckUpdated(PXCache sender, NotificationRecipient row, NotificationRecipient newRow)
		{
			return row.ContactType != newRow.ContactType || row.ContactID != newRow.ContactID;
		}

		private bool ValidateDuplicates(PXCache sender, NotificationRecipient row, NotificationRecipient oldRow)
		{			
			foreach (NotificationRecipient sibling in this.View.SelectMulti())
			{
				if (!CheckUpdated(sender, sibling, row) && row != sibling)
				{

					if (oldRow == null || row.ContactType != oldRow.ContactType)
						sender.RaiseExceptionHandling<NotificationRecipient.contactType>(
							row, row.ContactType,
							new PXSetPropertyException(ErrorMessages.DuplicateEntryAdded));

					if (oldRow == null || row.ContactID != oldRow.ContactID)
						sender.RaiseExceptionHandling<NotificationRecipient.contactID>(
							row, row.ContactType,
							new PXSetPropertyException(ErrorMessages.DuplicateEntryAdded));
					return false;
				}
			}
			sender.RaiseExceptionHandling<NotificationRecipient.contactType>(
							row, row.ContactType, null);
			sender.RaiseExceptionHandling<NotificationRecipient.contactID>(
							row, row.ContactID, null);
			return true;
		}

	}

	#endregion

	#region EPActivityPreview

	//TODO: move to corresponding control PX.Web.Controls.dll
	/*[Serializable]
	public class EPActivityPreview : EPGenericActivity
	{
		#region TaskID

		public new abstract class taskID : IBqlField { }

		#endregion

		#region Subject

		public new abstract class subject : IBqlField { }

		[PXDBString(255, InputMask = "", IsUnicode = true)]
		[PXUIField(DisplayName = "Summary", IsReadOnly = true)]
		public override string Subject
		{
			get
			{
				return base.Subject;
			}
			set
			{
				base.Subject = value;
			}
		}

		#endregion

		#region ColoredCategory

		public abstract class coloredCategory : IBqlField { }

		[PXString]
		[PXUIField(DisplayName = "Category")]
		public virtual String ColoredCategory { get; set; }

		#endregion

		#region DueDateDescription

		public abstract class dueDateDescription : IBqlField { }

		private string _dueDateDescription;
		[PXString]
		[PXUIField(DisplayName = "Due Date")]
		public virtual String DueDateDescription
		{
			get
			{
				if (_dueDateDescription == null)
				{
					_dueDateDescription = string.Empty;
					if (StartDate != null) _dueDateDescription = "Start " + ((DateTime)StartDate).ToString("g");
					if (EndDate != null)
					{
						if (_dueDateDescription.Length > 0) _dueDateDescription += ", end ";
						else _dueDateDescription += "End ";
						_dueDateDescription += ((DateTime)EndDate).ToString("g");
					}
				}
				return _dueDateDescription;
			}
		}

		#endregion

		public new abstract class owner : IBqlField { }

		[PXDBGuid]
		[PXUIField(DisplayName = "Performed by")]
		public override Guid? Owner
		{
			get
			{
				return base.Owner;
			}
			set
			{
				base.Owner = value;
			}
		}
	}*/

	#endregion

	#region PXActionExt<TNode>
	[Obsolete("Will be removed in 7.0 version")]
	public class PXActionExt<TNode> : PXAction<TNode>
		where TNode : class, IBqlTable, new()
	{
		public delegate void AdapterPrepareHandler(PXAdapter adapter);
		public delegate void AdapterPrepareInsertHandler(PXAdapter adapter, object previousCurrent);

		private PXView _view;
		private AdapterPrepareHandler _selectPrepareHandler;
		private AdapterPrepareInsertHandler _insertPrepareHandler;

		protected PXActionExt(PXGraph graph)
			: base(graph)
		{
		}

		public PXActionExt(PXGraph graph, string name)
			: base(graph, name)
		{
		}

		public PXActionExt(PXGraph graph, Delegate handler)
			: base(graph, handler)
		{
		}

		public virtual void SetPrepareHandlers(AdapterPrepareHandler selectPrepareHandler, AdapterPrepareInsertHandler insertPrepareHandler)
		{
			SetPrepareHandlers(selectPrepareHandler, insertPrepareHandler, null);
		}

		public virtual void SetPrepareHandlers(AdapterPrepareHandler selectPrepareHandler, AdapterPrepareInsertHandler insertPrepareHandler, PXView view)
		{
			if (view != null && !typeof(TNode).IsAssignableFrom(view.GetItemType()))
				throw new ArgumentException(string.Format("Item of view must inherit '{0}'", typeof(TNode).Name), "view");
			_view = view;
			_selectPrepareHandler = selectPrepareHandler;
			_insertPrepareHandler = insertPrepareHandler;
		}

		public override IEnumerable Press(PXAdapter adapter)
		{
			var newAdapter = new PXAdapter(_view ?? adapter.View);
			PXAdapter.Copy(adapter, newAdapter);
			if (_selectPrepareHandler != null) _selectPrepareHandler(adapter);
			var result = new List<object>(base.Press(newAdapter).Cast<object>());
			PXAdapter.Copy(newAdapter, adapter);
			return result;
		}

		protected virtual void InsertExt(PXAdapter adapter, object previousCurrent)
		{
			if (_insertPrepareHandler != null) _insertPrepareHandler(adapter, previousCurrent);
			Insert(adapter);
		}

		protected override void Insert(PXAdapter adapter)
		{
			var vals = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			if (adapter.Searches != null)
				for (int i = 0; i < adapter.Searches.Length && i < adapter.SortColumns.Length; i++)
					vals[adapter.SortColumns[i]] = adapter.Searches[i];
			foreach (string key in adapter.View.Cache.Keys)
				if (!vals.ContainsKey(key)) vals.Add(key, null);
			if (adapter.View.Cache.Insert(vals) == 1)
			{
				if (adapter.SortColumns == null)
					adapter.SortColumns = adapter.View.Cache.Keys.ToArray();
				else
				{
					var cols = new List<string>(adapter.SortColumns);
					foreach (string key in adapter.View.Cache.Keys)
						if (!CompareIgnoreCase.IsInList(cols, key))
							cols.Add(key);
					adapter.SortColumns = cols.ToArray();
				}
				adapter.Searches = new object[adapter.SortColumns.Length];
				for (int i = 0; i < adapter.Searches.Length; i++)
				{
					object val;
					if (vals.TryGetValue(adapter.SortColumns[i], out val))
						adapter.Searches[i] = val is PXFieldState ? ((PXFieldState) val).Value : val;
				}
				adapter.StartRow = 0;
			}
		}
	}

	#endregion

	#region PXFirstExt

	public class PXFirstExt<TNode> : PXActionExt<TNode>
		where TNode : class, IBqlTable, new()
	{
		public PXFirstExt(PXGraph graph, string name)
			: base(graph, name)
		{
		}

		[PXUIField(DisplayName = ActionsMessages.First, MapEnableRights = PXCacheRights.Select)]
		[PXFirstButton]
		protected override IEnumerable Handler(PXAdapter adapter)
		{
			adapter.Currents = new object[] { adapter.View.Cache.Current };
			var previousCurrent = adapter.View.Cache.Current;
			_Graph.SelectTimeStamp();
			adapter.StartRow = 0;
			adapter.Searches = null;
			bool anyFound = false;
			foreach (object item in adapter.Get())
			{
				yield return item;
				anyFound = true;
			}
			if (!anyFound && adapter.MaximumRows == 1 && adapter.View.Cache.AllowInsert)
			{
				InsertExt(adapter, previousCurrent);
				foreach (object ret in adapter.Get())
				{
					yield return ret;
				}
				adapter.View.Cache.IsDirty = false;
			}
		}
	}

	#endregion

	#region PXLastExt
	[Obsolete("Will be removed in 7.0 version")]
	public class PXLastExt<TNode> : PXActionExt<TNode>
		where TNode : class, IBqlTable, new()
	{
		public PXLastExt(PXGraph graph, string name)
			: base(graph, name)
		{
		}
		[PXUIField(DisplayName = ActionsMessages.Last, MapEnableRights = PXCacheRights.Select)]
		[PXLastButton]
		protected override IEnumerable Handler(PXAdapter adapter)
		{
			adapter.Currents = new object[] { adapter.View.Cache.Current };
			var previousCurrent = adapter.View.Cache.Current;
			_Graph.SelectTimeStamp();
			adapter.StartRow = -adapter.MaximumRows;
			adapter.Searches = null;
			bool anyFound = false;
			foreach (object item in adapter.Get())
			{
				yield return item;
				anyFound = true;
			}
			if (!anyFound && adapter.MaximumRows == 1 && adapter.View.Cache.AllowInsert)
			{
				InsertExt(adapter, previousCurrent);
				foreach (object ret in adapter.Get())
				{
					yield return ret;
				}
				adapter.View.Cache.IsDirty = false;
			}
		}
	}

	#endregion

	#region PXPreviousExt

	public class PXPreviousExt<TNode> : PXActionExt<TNode>
		where TNode : class, IBqlTable, new()
	{
		public PXPreviousExt(PXGraph graph, string name)
			: base(graph, name)
		{
		}
		[PXUIField(DisplayName = ActionsMessages.Previous, MapEnableRights = PXCacheRights.Select)]
		[PXPreviousButton]
		protected override IEnumerable Handler(PXAdapter adapter)
		{
			bool inserted = adapter.View.Cache.Current != null && adapter.View.Cache.GetStatus(adapter.View.Cache.Current) == PXEntryStatus.Inserted;
			if (inserted) return MoveToLast(adapter);

			var previousCurrent = adapter.View.Cache.Current;
			adapter.StartRow -= adapter.MaximumRows;
			_Graph.SelectTimeStamp();
			List<object> ret = (List<object>)adapter.Get();
			object curr = adapter.View.Cache.Current;
			adapter.Currents = new object[] { curr };
			_Graph.Clear(PXClearOption.PreserveTimeStamp);
			if (ret.Count == 0)
			{
				if (adapter.View.Cache.AllowInsert && adapter.MaximumRows == 1)
				{
					adapter.Currents = null;
					InsertExt(adapter, previousCurrent);
					ret = (List<object>)adapter.Get();
					adapter.View.Cache.IsDirty = false;
				}
				else
				{
					adapter.StartRow = -adapter.MaximumRows;
					adapter.Searches = null;
					ret = (List<object>) adapter.Get();
					if (ret.Count == 0 && adapter.View.Cache.AllowInsert && adapter.MaximumRows == 1)
					{
						InsertExt(adapter, previousCurrent);
						ret = (List<object>) adapter.Get();
						adapter.View.Cache.IsDirty = false;
					}
				}
			}
			return ret;
		}

		private IEnumerable MoveToLast(PXAdapter adapter)
		{
			adapter.Currents = new object[] { adapter.View.Cache.Current };
			var previousCurrent = adapter.View.Cache.Current;
			_Graph.Clear();
			_Graph.SelectTimeStamp();
			adapter.StartRow = -adapter.MaximumRows;
			adapter.Searches = null;
			bool anyFound = false;
			foreach (object item in adapter.Get())
			{
				yield return item;
				anyFound = true;
			}
			if (!anyFound && adapter.MaximumRows == 1 && adapter.View.Cache.AllowInsert)
			{
				InsertExt(adapter, previousCurrent);
				foreach (object ret in adapter.Get())
				{
					yield return ret;
				}
				adapter.View.Cache.IsDirty = false;
			}
		}
	}

	#endregion

	#region PXNextExt
	[Obsolete("Will be removed in 7.0 version")]
	public class PXNextExt<TNode> : PXActionExt<TNode>
		where TNode : class, IBqlTable, new()
	{
		public PXNextExt(PXGraph graph, string name)
			: base(graph, name)
		{
		}
		[PXUIField(DisplayName = ActionsMessages.Next, MapEnableRights = PXCacheRights.Select)]
		[PXNextButton]
		protected override IEnumerable Handler(PXAdapter adapter)
		{
			bool inserted = adapter.View.Cache.Current != null && adapter.View.Cache.GetStatus(adapter.View.Cache.Current) == PXEntryStatus.Inserted;
			var previousCurrent = adapter.View.Cache.Current;
			adapter.StartRow += adapter.MaximumRows;
			_Graph.SelectTimeStamp();
			List<object> ret = (List<object>)adapter.Get();
			adapter.Currents = new object[] { adapter.View.Cache.Current };
			_Graph.Clear(PXClearOption.PreserveTimeStamp);
			if (ret.Count == 0)
			{
				if (!inserted && adapter.View.Cache.AllowInsert && adapter.MaximumRows == 1)
				{
					InsertExt(adapter, previousCurrent);
					ret = (List<object>)adapter.Get();
					adapter.View.Cache.IsDirty = false;
				}
				else
				{
					adapter.StartRow = 0;
					adapter.Searches = null;
					ret = (List<object>)adapter.Get();
					if (ret.Count == 0 && adapter.View.Cache.AllowInsert && adapter.MaximumRows == 1)
					{
						InsertExt(adapter, previousCurrent);
						ret = (List<object>)adapter.Get();
						adapter.View.Cache.IsDirty = false;
					}
				}
			}
			return ret;
		}
	}

	#endregion

	#region PXInsertExt
	[Obsolete("Will be removed in 7.0 version")]
	public class PXInsertExt<TNode> : PXActionExt<TNode>
		where TNode : class, IBqlTable, new()
	{
		public PXInsertExt(PXGraph graph, string name)
			: base(graph, name)
		{
		}
		[PXUIField(DisplayName = ActionsMessages.Insert, MapEnableRights = PXCacheRights.Insert, MapViewRights = PXCacheRights.Insert)]
		[PXInsertButton]
		protected override IEnumerable Handler(PXAdapter adapter)
		{
			if (!adapter.View.Cache.AllowInsert)
			{
				throw new PXException(ErrorMessages.CantInsertRecord);
			}
			var previousCurrent = adapter.View.Cache.Current;
			_Graph.Clear();
			_Graph.SelectTimeStamp();
			InsertExt(adapter, previousCurrent);
			var newSearches = new ArrayList();
			var newSortColumns = new List<string>();
			for (int i = 0; i < adapter.Searches.Length && i < adapter.SortColumns.Length; i++)
			{
				var sortColumn = adapter.SortColumns[i];
				if (adapter.View.Cache.Keys.Contains(sortColumn, StringComparer.OrdinalIgnoreCase))
				{
					newSearches.Add(adapter.Searches[i]);
					newSortColumns.Add(sortColumn);
				}
			}
			adapter.SortColumns = newSortColumns.ToArray();
			adapter.Searches = newSearches.ToArray();
			foreach (object ret in adapter.Get())
			{
				yield return ret;
			}
			adapter.View.Cache.IsDirty = false;
		}
	}

	#endregion

	#region PXDeleteExt
	[Obsolete("Will be removed in 7.0 version")]
	public class PXDeleteExt<TNode> : PXActionExt<TNode>
		where TNode : class, IBqlTable, new()
	{
		public PXDeleteExt(PXGraph graph, string name)
			: base(graph, name)
		{
		}
		public override object GetState(object row)
		{
			object state = base.GetState(row);
			PXButtonState bs = state as PXButtonState;
			if (bs != null && !String.IsNullOrEmpty(bs.ConfirmationMessage))
			{
				if (typeof(TNode).IsDefined(typeof(PXCacheNameAttribute), true))
				{
					PXCacheNameAttribute attr = (PXCacheNameAttribute)(typeof(TNode).GetCustomAttributes(typeof(PXCacheNameAttribute), true)[0]);
					bs.ConfirmationMessage = String.Format(bs.ConfirmationMessage, attr.GetName());
				}
				else
				{
					bs.ConfirmationMessage = String.Format(bs.ConfirmationMessage, typeof(TNode).Name);
				}
			}
			return state;
		}
		[PXUIField(DisplayName = ActionsMessages.Delete, MapEnableRights = PXCacheRights.Delete, MapViewRights = PXCacheRights.Delete)]
		[PXDeleteButton(ConfirmationMessage = ActionsMessages.ConfirmDeleteExplicit)]
		protected override IEnumerable Handler(PXAdapter adapter)
		{
			if (!adapter.View.Cache.AllowDelete)
			{
				throw new PXException(ErrorMessages.CantDeleteRecord);
			}
			var previousCurrent = adapter.View.Cache.Current;
			int startRow = adapter.StartRow;
			foreach (object item in adapter.Get())
			{
				adapter.View.Cache.Delete(item);
			}
			try
			{
				_Graph.Actions.PressSave();
			}
			catch
			{
				_Graph.Clear();
				throw;
			}
			_Graph.SelectTimeStamp();
			adapter.StartRow = startRow;
			bool anyFound = false;
			foreach (object item in adapter.Get())
			{
				yield return item;
				anyFound = true;
			}
			if (!anyFound && adapter.MaximumRows == 1)
			{
				if (adapter.View.Cache.AllowInsert)
				{
					InsertExt(adapter, previousCurrent);
					foreach (object ret in adapter.Get())
					{
						yield return ret;
					}
					adapter.View.Cache.IsDirty = false;
				}
				else
				{
					adapter.StartRow = 0;
					adapter.Searches = null;
					foreach (object item in adapter.Get())
					{
						yield return item;
					}
				}
			}
		}
	}

	#endregion

	#region PXCancelExt
	[Obsolete("Will be removed in 7.0 version")]
	public class PXCancelExt<TNode> : PXActionExt<TNode>
		where TNode : class, IBqlTable, new()
	{
		public PXCancelExt(PXGraph graph, string name)
			: base(graph, name)
		{
		}
		[PXUIField(DisplayName = ActionsMessages.Cancel, MapEnableRights = PXCacheRights.Select)]
		[PXCancelButton]
		protected override IEnumerable Handler(PXAdapter adapter)
		{
			adapter.Currents = new object[] { adapter.View.Cache.Current };
			var previousCurrent = adapter.View.Cache.Current;
			_Graph.Clear();
			_Graph.SelectTimeStamp();
			bool anyFound = false;
			bool perfromSearch = true;
			if (adapter.MaximumRows == 1)
			{
				perfromSearch = adapter.View.Cache.Keys.Count == 0;
				if (adapter.Searches != null)
				{
					for (int i = 0; i < adapter.Searches.Length; i++)
					{
						if (adapter.Searches[i] != null)
						{
							perfromSearch = true;
							break;
						}
					}
				}
			}
			if (perfromSearch)
			{
				foreach (object item in adapter.Get())
				{
					yield return item;
					anyFound = true;
				}
			}
			if (!anyFound && adapter.MaximumRows == 1)
			{
				if (adapter.View.Cache.AllowInsert)
				{
					_Graph.Clear();
					_Graph.SelectTimeStamp();
					InsertExt(adapter, previousCurrent);
					foreach (object ret in adapter.Get())
					{
						yield return ret;
					}
					adapter.View.Cache.IsDirty = false;
				}
				else
				{
					adapter.Currents = null;
					adapter.StartRow = 0;
					adapter.Searches = null;
					foreach (object item in adapter.Get())
					{
						yield return item;
					}
				}
			}
		}
	}

	#endregion

	#region CRRelationsList<TNoteField>

	public class CRRelationsList<TNoteField> : PXSelect<CRRelation>
		where TNoteField : IBqlField
	{
		public CRRelationsList(PXGraph graph) 
			: base(graph, GetHandler())
		{
			VerifyParameters();
			AttacheEventHandlers(graph);
		}

		private static void VerifyParameters()
		{
			if (BqlCommand.GetItemType(typeof (TNoteField)) == null)
				throw new ArgumentException(PXMessages.LocalizeFormatNoPrefixNLA(Messages.IBqlFieldMustBeNested,
					typeof (TNoteField).GetLongName()), "TNoteField");
			if (!typeof (IBqlTable).IsAssignableFrom(BqlCommand.GetItemType(typeof (TNoteField))))
				throw new ArgumentException(PXMessages.LocalizeFormatNoPrefixNLA(Messages.IBqlTableMustBeInherited,
					BqlCommand.GetItemType(typeof (TNoteField)).GetLongName()), "TNoteField");
		}

		private void AttacheEventHandlers(PXGraph graph)
		{
			PXDBDefaultAttribute.SetSourceType<CRRelation.refNoteID>(graph.Caches[typeof(CRRelation)], typeof(TNoteField));
			graph.FieldDefaulting.AddHandler(typeof(CRRelation), typeof(CRRelation.refNoteID).Name, CRRelation_RefNoteID_FieldDefaulting);
			graph.RowInserted.AddHandler(typeof(CRRelation), CRRelation_RowInserted);
			graph.RowUpdated.AddHandler(typeof(CRRelation), CRRelation_RowUpdated);
			graph.RowSelected.AddHandler(typeof(CRRelation), CRRelation_RowSelected);
			graph.RowPersisting.AddHandler(typeof(CRRelation), CRRelation_RowPersisting);

			graph.Initialized +=
				sender =>
					{
						sender.Views.Caches.Remove(typeof(TNoteField));
						sender.EnsureCachePersistence(typeof(CRRelation));
						AppendActions(graph);
					};
		}

		private void AppendActions(PXGraph graph)
		{
			var viewName = graph.ViewNames[View];
			var primaryDAC = BqlCommand.GetItemType(typeof(TNoteField));
			PXNamedAction.AddAction(graph, primaryDAC, viewName + "_EntityDetails", null, EntityDetailsHandler);
			PXNamedAction.AddAction(graph, primaryDAC, viewName + "_ContactDetails", null, ContactDetailsHandler);
		}

		private IEnumerable EntityDetailsHandler(PXAdapter adapter)
		{
			var graph = adapter.View.Graph;
			var cache = graph.Caches[typeof(CRRelation)];
			var row = (cache.Current as CRRelation).
				With(_ => _.EntityID).
				With(_ => (BAccount)PXSelect<BAccount, 
					Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>.
					Select(graph, _.Value));
            
            if (row != null)
            {
                if (row.Type == BAccountType.EmployeeType)
                    PXRedirectHelper.TryRedirect(PXGraph.CreateInstance<EmployeeMaint>(), row, PXRedirectHelper.WindowMode.NewWindow);
                else
                    PXRedirectHelper.TryRedirect(graph, row, PXRedirectHelper.WindowMode.NewWindow);
            }
			return adapter.Get();
		}

		private IEnumerable ContactDetailsHandler(PXAdapter adapter)
		{
			var graph = adapter.View.Graph;
			var cache = graph.Caches[typeof(CRRelation)];
			var row = (cache.Current as CRRelation).
				With(_ => _.ContactID).
				With(_ => (Contact)PXSelect<Contact,
					Where<Contact.contactID, Equal<Required<Contact.contactID>>>>.
					Select(graph, _.Value));
			if (row != null)
				PXRedirectHelper.TryRedirect(graph, row, PXRedirectHelper.WindowMode.NewWindow);
			return adapter.Get();
		}

		protected virtual void CRRelation_RefNoteID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			var refCache = sender.Graph.Caches[BqlCommand.GetItemType(typeof(TNoteField))];
			e.NewValue = refCache.GetValue(refCache.Current, typeof(TNoteField).Name);
		}

		protected virtual void CRRelation_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			var row = (CRRelation)e.Row;

			FillRow(sender.Graph, row);
		}

		protected virtual void CRRelation_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			var row = (CRRelation)e.Row;
			var oldRow = (CRRelation)e.OldRow;

			if (row.ContactID == oldRow.ContactID && row.EntityID != oldRow.EntityID)
				row.ContactID = null;

			FillRow(sender.Graph, row);
		}

		protected virtual void CRRelation_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			var row = e.Row as CRRelation;
			if (row == null) return;

			var enableContactID = 
				row.EntityID.
				With(id => (BAccount)PXSelect<BAccount>.
					Search<BAccount.bAccountID>(sender.Graph, id.Value)).
				Return(acct => acct.Type != BAccountType.EmployeeType, 
				true);
			PXUIFieldAttribute.SetEnabled(sender, row, typeof(CRRelation.contactID).Name, enableContactID);
		}

		protected virtual void CRRelation_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			CRRelation row = e.Row as CRRelation;
			if (row != null && row.EntityID == null && row.ContactID == null)
			{
				sender.RaiseExceptionHandling<CRRelation.entityID>(row, null, new PXSetPropertyException(ErrorMessages.AllFieldsIsEmpty));
				sender.RaiseExceptionHandling<CRRelation.contactID>(row, null, new PXSetPropertyException(ErrorMessages.AllFieldsIsEmpty));
			}
		}

		private static void FillRow(PXGraph graph, CRRelation row)
		{
			var search = row.EntityID.
					With(id => PXSelect<BAccount,
						Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>.
					Select(graph, id.Value));
			string acctType = null;
			if (search == null || search.Count == 0)
			{
				row.Name = null;
				row.EntityCD = null;
			}
			else
			{
				var account = (BAccount)search[0][typeof(BAccount)];
				row.Name = account.AcctName;
				row.EntityCD = account.AcctCD;
				acctType = account.Type;
			}

			Contact contactSearch;
			PXResultset<EPEmployeeSimple> employeeSearch;
			if (acctType == BAccountType.EmployeeType &&
				(employeeSearch = row.EntityID.
					With(eId => PXSelectJoin<EPEmployeeSimple,
					LeftJoin<Contact, On<Contact.contactID, Equal<EPEmployeeSimple.defContactID>>,
					LeftJoin<Users, On<Users.pKID, Equal<EPEmployeeSimple.userID>>>>,
					Where<EPEmployeeSimple.bAccountID, Equal<Required<EPEmployeeSimple.bAccountID>>>>.
					Select(graph, eId.Value))) != null)
			{
				row.ContactName = null;
				var contact = (Contact)employeeSearch[0][typeof(Contact)];
				row.Email = contact.EMail;
				var user = (Users)employeeSearch[0][typeof(Users)];
				if (string.IsNullOrEmpty(row.Name))
					row.Name = user.FullName;
				if (string.IsNullOrEmpty(row.Email))
					row.Email = user.Email;
			}
			else if ((contactSearch = row.ContactID.
						With(cId => (Contact)PXSelect<Contact>.
							Search<Contact.contactID>(graph, cId.Value))) != null)
			{
				row.ContactName = contactSearch.DisplayName;
				row.Email = contactSearch.EMail;
			}
			else
			{
				row.ContactName = null;
				row.Email = null;
			}
		}

		private static PXSelectDelegate GetHandler()
		{
			return () =>
				{
					var command = new Select2<CRRelation,
						LeftJoin<BAccount, On<BAccount.bAccountID, Equal<CRRelation.entityID>>,							
						LeftJoin<Contact,
								  On<Contact.contactID, Equal<Switch<Case<Where<BAccount.type, Equal<BAccountType.employeeType>>,BAccount.defContactID>,CRRelation.contactID>>>,  
						LeftJoin<Users, On<Users.pKID, Equal<Contact.userID>>>>>,
						Where<CRRelation.refNoteID, Equal<Current<TNoteField>>>>();
					var startRow = PXView.StartRow;
					int totalRows = 0;
					var list = new PXView(PXView.CurrentGraph, false, command).
						Select(null, null, PXView.Searches, PXView.SortColumns, PXView.Descendings, PXView.Filters,
						       ref startRow, PXView.MaximumRows, ref totalRows);
					PXView.StartRow = 0;
					foreach (PXResult<CRRelation, BAccount, Contact, Users> row in list)
					{
						var relation = (CRRelation)row[typeof(CRRelation)];
						var account = (BAccount)row[typeof(BAccount)];
						relation.Name = account.AcctName;
						relation.EntityCD = account.AcctCD;
						var contact = (Contact)row[typeof(Contact)];
						if (contact.ContactID == null && relation.ContactID != null &&
						    account.Type != BAccountType.EmployeeType)
						{
							var directContact = (Contact)PXSelect<Contact>.
								                             Search<Contact.contactID>(PXView.CurrentGraph, relation.ContactID);
							if (directContact != null) contact = directContact;
						}
						relation.Email = contact.EMail;
						var user = (Users)row[typeof(Users)];
						if (account.Type != BAccountType.EmployeeType) 
							relation.ContactName = contact.DisplayName;
						else
						{
							if (string.IsNullOrEmpty(relation.Name))
								relation.Name = user.FullName;
							if (string.IsNullOrEmpty(relation.Email))
								relation.Email = user.Email;
						}
					}
					return list;
				};
		}

		public static IEnumerable<Mailbox> GetEmailsForCC(PXGraph graph, Guid? refNoteID)
		{
			var command = new Select2<CRRelation,
							LeftJoin<BAccount, On<BAccount.bAccountID, Equal<CRRelation.entityID>>,
							LeftJoin<Contact,
								On<Contact.contactID, Equal<Switch<Case<Where<BAccount.type, Equal<BAccountType.employeeType>>,BAccount.defContactID>,CRRelation.contactID>>>,
							LeftJoin<Users, On<Users.pKID, Equal<Contact.userID>>>>>,
							Where<CRRelation.refNoteID, Equal<Required<CRRelation.refNoteID>>>>();
			var list = new PXView(graph, false, command).SelectMulti(refNoteID);
			foreach (PXResult<CRRelation, BAccount, Contact, Users> row in list)
			{
				var relation = (CRRelation)row[typeof(CRRelation)];
				if (relation.AddToCC == true)
				{
					var account = (BAccount)row[typeof(BAccount)];
					var name = account.AcctName;
					var contact = (Contact)row[typeof(Contact)];
					if (contact.ContactID == null && relation.ContactID != null &&
						account.Type != BAccountType.EmployeeType)
					{
						var directContact = (Contact)PXSelect<Contact>.
							Search<Contact.contactID>(PXView.CurrentGraph, relation.ContactID);
						if (directContact != null) contact = directContact;
					}
					var email = contact.EMail;
					var user = (Users)row[typeof(Users)];
					if (account.Type != BAccountType.EmployeeType)
						name = contact.DisplayName;
					else
					{
						if (string.IsNullOrEmpty(name))
							name = user.FullName;
						if (string.IsNullOrEmpty(email))
							email = user.Email;
					}
					if (email != null && (email = email.Trim()) != string.Empty)
						yield return new Mailbox(name, email);
				}
			}
		}
	
	}

	#endregion

	#region CRSubscriptionsSelect

	public sealed class CRSubscriptionsSelect
	{
		public static IEnumerable Select(PXGraph graph, int? mailListID)
		{
			var startRow = PXView.StartRow;
			int totalRows = 0;			
			var list = Select(graph, mailListID, PXView.Searches, PXView.SortColumns, PXView.Descendings,
								   ref startRow, PXView.MaximumRows, ref totalRows);
			PXView.StartRow = 0;
			return list;
		}
	
		public static IEnumerable Select(PXGraph graph, int? mailListID, object[] searches, string[] sortColumns, bool[] descendings, ref int startRow, int maxRows, ref int totalRows)
		{
			CRMarketingList list;
			if (mailListID == null ||
				(list = (CRMarketingList)PXSelect<CRMarketingList>.Search<CRMarketingList.marketingListID>(graph, mailListID)) == null)
			{
				return new PXResultset<Contact, BAccount, BAccountParent, Address, State>();
			}

			MergeFilters(graph, mailListID);

			BqlCommand command = new Select2<Contact,
				LeftJoin<BAccount,
					On<BAccount.bAccountID, Equal<Contact.bAccountID>>,
				LeftJoin<BAccountParent,
					On<BAccountParent.bAccountID, Equal<Contact.parentBAccountID>>,
				LeftJoin<Address,
					On<Address.addressID, Equal<Contact.defAddressID>>,
				LeftJoin<State,
					On<State.countryID, Equal<Address.countryID>,
						And<State.stateID, Equal<Address.state>>>>>>>,
				Where<True,Equal<True>>>();

			if (list.NoCall == true)
				command = command.WhereAnd(
					typeof(Where<Contact.noCall, IsNull,
								Or<Contact.noCall, NotEqual<True>>>));
			if (list.NoEMail == true)
				command = command.WhereAnd(
					typeof(Where<Contact.noEMail, IsNull,
								Or<Contact.noEMail, NotEqual<True>>>));
			if (list.NoFax == true)
				command = command.WhereAnd(
					typeof(Where<Contact.noFax, IsNull,
								Or<Contact.noFax, NotEqual<True>>>));
			if (list.NoMail == true)
				command = command.WhereAnd(
					typeof(Where<Contact.noMail, IsNull,
								Or<Contact.noMail, NotEqual<True>>>));
			if (list.NoMarketing == true)
				command = command.WhereAnd(
					typeof(Where<Contact.noMarketing, IsNull,
								Or<Contact.noMarketing, NotEqual<True>>>));
			if (list.NoMassMail == true)
				command = command.WhereAnd(
					typeof(Where<Contact.noMassMail, IsNull,
								Or<Contact.noMassMail, NotEqual<True>>>));

			var view = new PXView(graph, true, command);
			return view.Select(null, null, searches, sortColumns, descendings, PXView.Filters, ref startRow, maxRows, ref totalRows);
		}

		public static void MergeFilters(PXGraph graph, int? mailListID)
		{
			var filters = PXView.Filters; //new PXView.PXFilterRowCollection(new PXFilterRow[]{});
			CRMarketingList list = PXSelect<CRMarketingList,
				Where<CRMarketingList.marketingListID, Equal<Required<CRMarketingList.marketingListID>>>>.
				Select(graph, mailListID);

			Guid? mailListNoteID = PXNoteAttribute.GetNoteID<CRMarketingList.noteID>(graph.Caches[typeof(CRMarketingList)], list);
				
			PXCache targetCache = graph.Caches[typeof(Contact)];
			var filterRows = new List<PXFilterRow>();
			foreach (CRFixedFilterRow filter in
				PXSelect<CRFixedFilterRow,
					Where<CRFixedFilterRow.refNoteID, Equal<Required<CRFixedFilterRow.refNoteID>>>>.
				Select(graph, mailListNoteID))
			{
				if (filter.IsUsed == true)
				{
					var f = new PXFilterRow
						{
							OpenBrackets = filter.OpenBrackets ?? 0,
							DataField = filter.DataField,
							Condition = (PXCondition) (filter.Condition ?? 0),
							Value = targetCache.ValueFromString(filter.DataField, filter.ValueSt) ?? filter.ValueSt,
							Value2 = targetCache.ValueFromString(filter.DataField, filter.ValueSt2) ?? filter.ValueSt2,
							CloseBrackets = filter.CloseBrackets ?? 0,
							OrOperator = filter.Operator == 1
						};
					filterRows.Add(f);
				}
			}						
			filters.Add(filterRows.ToArray());
		}
	}

	#endregion

	#region _100Percents

	public sealed class _100Percents : Constant<Decimal>
	{
		public _100Percents() : base(100m) { }
	}

	#endregion

	#region BqlSetup<Field>

	public class BqlSetup<Field> : IBqlOperand, IBqlCreator
		where Field : IBqlField
	{
		private static readonly Type _table;
		private static readonly MethodInfo _select;

		static BqlSetup()
		{
			_table = BqlCommand.GetItemType(typeof(Field));
			foreach(MethodInfo method in 
				BqlCommand.Compose(typeof(PXSelectReadonly<>), _table).
				GetMethods(BindingFlags.Public | BindingFlags.Static))
			{
				if (method.Name == "SelectWindowed" && !method.IsGenericMethod)
				{
					_select = method;
					break;
				}
			}
		}

		public virtual void Verify(PXCache cache, object item, List<object> pars, ref bool? result, ref object value)
		{
			value = GetValue(cache.Graph);
		}

		public virtual void Parse(PXGraph graph, List<IBqlParameter> pars, List<Type> tables, List<Type> fields, List<IBqlSortColumn> sortColumns, StringBuilder text, BqlCommand.Selection selection)
		{
			if (graph != null && text != null)
			{
				PXMutableCollection.AddMutableItem(this);
				text.Append(graph.SqlDialect.enquoteValue(GetValue(graph)));
			}
		}

		private static object GetValue(PXGraph graph)
		{
			object res = _select.Invoke(null, new object[] { graph, 0, 1, null });
			if (res is IPXResultset) res = ((IPXResultset)res).GetItem(0, 0);
			if (res == null) return null;
			var cache = graph.Caches[_table];
			var fieldName = cache.GetField(typeof(Field));
			return cache.GetValue(res, fieldName);
		}
	}

	#endregion


	#region CSAttributeSelector<TAnswer, TEntityId, TEntityType, TEntityClassId>
	[Obsolete("Will be removed in 7.0 version")]
	public class CSAttributeSelector<TAnswer, TEntityNoteId, TEntityClassId>
        : PXSelect<TAnswer>
		where TAnswer : class, IBqlTable, new()
	{
		public CSAttributeSelector(PXGraph graph)
			: base(graph)
		{
			Intialize();
		}

		public CSAttributeSelector(PXGraph graph, Delegate handler)
			: base(graph, handler)
		{
			Intialize();
		}

		private void Intialize()
		{
			var graph = View.Graph;
			var isReadonly = View.IsReadOnly;
			var command = CreateCommand();
			View = new PXView(graph, isReadonly, command);
		}

		private BqlCommand CreateCommand()
		{
			var cache = _Graph.Caches[typeof(TAnswer)];
			var attIdField = cache.GetBqlField(typeof(CSAnswers.attributeID).Name);

            var entityNoteIdField = cache.GetBqlField(typeof(CSAnswers.refNoteID).Name);
            var res = BqlCommand.CreateInstance(
				typeof(Select2<,,,>), typeof(TAnswer), 
				typeof(InnerJoin<,,>), typeof(CSAttributeGroup),
					typeof(On<,>), typeof(CSAttributeGroup.attributeID), typeof(Equal<>), attIdField, 
				typeof(LeftJoin<,>), typeof(CSAttribute),
					typeof(On<,>), typeof(CSAttribute.attributeID), typeof(Equal<>), attIdField,
				typeof(Where<,,>), entityNoteIdField, typeof(Equal<>), typeof(TEntityNoteId),
					typeof(And<,,>), typeof(CSAttributeGroup.entityClassID), typeof(Equal<>), typeof(TEntityClassId),
				typeof(OrderBy<Asc<CSAttributeGroup.sortOrder>>));
			res = CorrectCommand(res);
			return res;
		}

		protected virtual BqlCommand CorrectCommand(BqlCommand command)
		{
			return command;
		}
	}

	#endregion

	#region CSAttributeSelector
	[Obsolete("Will be removed in 7.0 version")]
	public class CSAttributeSelector<TAnswer, TEntityNoteId, TEntityClassId, TJoin, TWhere>
        : CSAttributeSelector<TAnswer, TEntityNoteId, TEntityClassId>
		where TAnswer: class, IBqlTable, new()
	{
		public CSAttributeSelector(PXGraph graph)
			: base(graph)
		{
			
		}

		public CSAttributeSelector(PXGraph graph, Delegate handler)
			: base(graph, handler)
		{
		}

		protected override BqlCommand CorrectCommand(BqlCommand command)
		{
			var res = base.CorrectCommand(command);
			res = BqlCommand.CreateInstance(BqlCommand.AppendJoin(res.GetType(), typeof(TJoin)));
			res = res.WhereAnd(typeof(TWhere));
			return res;
		}
	}
	#endregion

	#region CRDBTimeSpanAttribute
	[Obsolete("Will be removed in 7.0 version")]
	public sealed class CRDBTimeSpanAttribute : PXDBTimeSpanAttribute
	{
		private const string _FIELD_POSTFIX = "_byString";
		private const string _INPUTMASK = "##### hrs ## mins";
		private const int _SHARPS_COUNT = 7;
		private const string _DEFAULT_VALUE = "      0";

		private string _byStringFieldName;

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);

			_byStringFieldName = _FieldName + _FIELD_POSTFIX;
			sender.Fields.Add(_byStringFieldName);
			sender.Graph.FieldSelecting.AddHandler(sender.GetItemType(), _byStringFieldName, _FieldName_byString_FieldSelecting);
		}

		private void _FieldName_byString_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs args)
		{
			var mainState = sender.GetStateExt(args.Row, _FieldName) as PXFieldState;
			var displayName = mainState.With(_ => _.DisplayName);
			var visible = mainState.With(_ => _.Visible);
			var val = sender.GetValue(args.Row, _FieldOrdinal);
			var valStr = _DEFAULT_VALUE;
			if (val != null)
			{
				var ts = TimeSpan.FromMinutes((int)val);
				valStr = ((int)ts.TotalHours).ToString() + ((int)ts.Minutes).ToString("00");
				if (valStr.Length < _SHARPS_COUNT)
					valStr = new string(' ', _SHARPS_COUNT - valStr.Length) + valStr;
			}

			var newState = PXStringState.CreateInstance(valStr, null, false, _byStringFieldName, false, null,
															_INPUTMASK, null, null, null, null);
			newState.Enabled = false;
			newState.Visible = visible;
			newState.DisplayName = displayName;

			args.ReturnState = newState;
		}
	}

	#endregion

	#region CRTimeSpanAttribute
	[Obsolete("Will be removed in 7.0 version")]
	public sealed class CRTimeSpanAttribute : PXTimeSpanAttribute
	{
		private const string _FIELD_POSTFIX = "_byString";
		private const string _TIME_FIELD_POSTFIX = "_byTimeString";
		private const string _INPUTMASK = "##### hrs ## mins";
		private const int _SHARPS_COUNT = 7;
		private const string _DEFAULT_VALUE = "      0";

		private string _byStringFieldName;
		private string _byTimeStringFieldName;

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);

			_byStringFieldName = _FieldName + _FIELD_POSTFIX;
			sender.Fields.Add(_byStringFieldName);
			sender.Graph.FieldSelecting.AddHandler(sender.GetItemType(), _byStringFieldName, _FieldName_byString_FieldSelecting);

			_byTimeStringFieldName = _FieldName + _TIME_FIELD_POSTFIX;
			sender.Fields.Add(_byTimeStringFieldName);
			sender.Graph.FieldSelecting.AddHandler(sender.GetItemType(), _byTimeStringFieldName, _FieldName_byTimeString_FieldSelecting);
		}

		private void _FieldName_byString_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs args)
		{
			var mainState = sender.GetStateExt(args.Row, _FieldName) as PXFieldState;
			var displayName = mainState.With(_ => _.DisplayName);
			var visible = mainState.With(_ => _.Visible);
			var val = sender.GetValue(args.Row, _FieldOrdinal);
			var valStr = _DEFAULT_VALUE;
			if (val != null)
			{
				var ts = TimeSpan.FromMinutes((int)val);
				valStr = ((int)ts.TotalHours).ToString() + ((int)ts.Minutes).ToString("00");
				if (valStr.Length < _SHARPS_COUNT)
					valStr = new string(' ', _SHARPS_COUNT - valStr.Length) + valStr;
			}

			var newState = PXStringState.CreateInstance(valStr, null, false, _byStringFieldName, false, null,
															_INPUTMASK, null, null, null, null);
			newState.Enabled = false;
			newState.Visible = visible;
			newState.DisplayName = displayName;

			args.ReturnState = newState;
		}

		private void _FieldName_byTimeString_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs args)
		{
			var mainState = sender.GetStateExt(args.Row, _FieldName) as PXFieldState;
			var displayName = mainState.With(_ => _.DisplayName);
			var visible = mainState.With(_ => _.Visible);
			var val = sender.GetValue(args.Row, _FieldOrdinal);
			var valStr = string.Empty;
			if (val != null)
			{
				var ts = TimeSpan.FromMinutes((int)val);
				valStr = string.Format("{0:00}:{1:00}", (int)ts.TotalHours, ts.Minutes);
			}

			var newState = PXStringState.CreateInstance(valStr, null, false, _byTimeStringFieldName, false, null,
															null, null, null, null, null);
			newState.Enabled = false;
			newState.Visible = visible;
			newState.DisplayName = displayName;

			args.ReturnState = newState;
		}
	}

	#endregion

	#region CRCaseActivityHelper<TableRefNoteID>

	public class CRCaseActivityHelper
	{
		#region Ctor

		public static CRCaseActivityHelper Attach(PXGraph graph)
		{
			var res = new CRCaseActivityHelper();

			graph.RowInserted.AddHandler<PMTimeActivity>(res.ActivityRowInserted);
			graph.RowSelected.AddHandler<PMTimeActivity>(res.ActivityRowSelected);
			graph.RowUpdated.AddHandler<PMTimeActivity>(res.ActivityRowUpdated);

			return res;
		}
		#endregion

		#region Event Handlers

		protected virtual void ActivityRowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			var item = e.Row as PMTimeActivity;
			if (item == null) return;

			var graph = sender.Graph;
			var @case = GetCase(graph, item.RefNoteID);
			if (@case != null && @case.Released == true) item.IsBillable = false;
		}

		protected virtual void ActivityRowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			var item = e.Row as PMTimeActivity;
			var oldItem = e.OldRow as PMTimeActivity;
			if (item == null || oldItem == null) return;

			var graph = sender.Graph;
			var @case = GetCase(graph, item.RefNoteID);
			if (@case != null && @case.Released == true) item.IsBillable = false;
		}

		protected virtual void ActivityRowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			var item = e.Row as PMTimeActivity;
			if (item == null || !string.IsNullOrEmpty(item.TimeCardCD)) return;

			var graph = sender.Graph;
			var @case = GetCase(graph, item.RefNoteID);
			if(@case != null && @case.Released == true )
				PXUIFieldAttribute.SetEnabled<PMTimeActivity.isBillable>(sender, item, false);
		}

		#endregion

		#region Private Methods

		private CRCase GetCase(PXGraph graph, object refNoteID)
		{
			if (refNoteID == null) return null;
			return (CRCase)PXSelectJoin<CRCase,
				InnerJoin<CRActivityLink,
					On<CRActivityLink.refNoteID, Equal<CRCase.noteID>>>, 
				Where<CRActivityLink.noteID, Equal<Required<PMTimeActivity.refNoteID>>>>.
				SelectWindowed(graph, 0, 1, refNoteID);
		}

		#endregion
	}

	#endregion


    public class CRAttribute
    {
        public class Attribute
        {
            public readonly string ID;
            public readonly string Description;
            public readonly int? ControlType;
            public readonly string EntryMask;
            public readonly string RegExp;
            public readonly string List;
            public readonly bool IsInternal;

            public Attribute(PXDataRecord record)
            {
                ID = record.GetString(0);
                Description = record.GetString(1);
                ControlType = record.GetInt32(2);
                EntryMask = record.GetString(3);
                RegExp = record.GetString(4);
                List = record.GetString(5);
                IsInternal = record.GetBoolean(6) == true;
                Values =  new List<AttributeValue>();
            }

            protected Attribute(Attribute clone)
            {
                this.ID = clone.ID;
                this.Description = clone.Description;
                this.ControlType = clone.ControlType;
                this.EntryMask = clone.EntryMask;
                this.RegExp = clone.RegExp;
                this.List = clone.List;
                this.IsInternal = clone.IsInternal;
                this.Values = clone.Values;
            }

            public void AddValue(AttributeValue value)
            {
                Values.Add(value);
            }
            
            public readonly List<AttributeValue> Values;
        }

        public class AttributeValue
        {
            public readonly string ValueID;
            public readonly string Description;            
            public readonly bool Disabled;

            public AttributeValue(PXDataRecord record)
            {
                ValueID = record.GetString(1);
                Description = record.GetString(2);                
                Disabled = record.GetBoolean(3) == true;
            }
        }

        [DebuggerDisplay("ID={ID} Description={Description} Required={Required}")]
        public class AttributeExt : Attribute
        {               
            public readonly string DefaultValue;
            public readonly bool   Required;

            public AttributeExt(Attribute attr, string defaultValue, bool required)
                :base(attr)
            {
                this.DefaultValue = defaultValue;
                this.Required = required;
            }
        }

        public class AttributeList : DList<string, Attribute>
        {
            private readonly bool useDescriptionAsKey;
            public AttributeList(bool useDescriptionAsKey = false)
                : base(StringComparer.InvariantCultureIgnoreCase, DefaultCapacity, DefaultDictionaryCreationThreshold, true)
            {
                this.useDescriptionAsKey = useDescriptionAsKey;
            }
            protected override string GetKeyForItem(Attribute item)
            {
                return useDescriptionAsKey?item.Description:item.ID;
            }

            public override Attribute this[string key]
            {
                get
                {
                    Attribute e;
                    return TryGetValue(key, out e) ? e : null;                                            

                }
            }
        }

        public class ClassAttributeList: DList<string, AttributeExt>
        {
            public ClassAttributeList()
                :base(StringComparer.InvariantCultureIgnoreCase, DefaultCapacity, DefaultDictionaryCreationThreshold, true)
            {
                
            }
            protected override string GetKeyForItem(AttributeExt item)
            {
                return item.ID;
            }
            public override AttributeExt this[string key]
            {
                get
                {
                   AttributeExt e;                        
                   return TryGetValue(key, out e) ? e : null;                                            
                }
            }
        }
        
        private class Definition : IPrefetchable
        {
            public readonly AttributeList Attributes;
            public readonly AttributeList AttributesByDescr;
			public readonly Dictionary<string, AttributeList> EntityAttributes;
            public readonly Dictionary<string, Dictionary<string, ClassAttributeList>> ClassAttributes;

            public Definition()
            {
                Attributes = new AttributeList();
                AttributesByDescr = new AttributeList(true);
                ClassAttributes = new Dictionary<string, Dictionary<string, ClassAttributeList>>(StringComparer.InvariantCultureIgnoreCase);
				EntityAttributes = new Dictionary<string, AttributeList>(StringComparer.InvariantCultureIgnoreCase);
            }
            public void Prefetch()
            {
                using (new PXConnectionScope())
                {
                    Attributes.Clear();
                    AttributesByDescr.Clear();  
                    foreach(PXDataRecord record in  PXDatabase.SelectMulti<CSAttribute>(
                        new PXDataField(typeof(CSAttribute.attributeID).Name),
						PXDBLocalizableStringAttribute.GetValueSelect(typeof(CSAttribute).Name, typeof(CSAttribute.description).Name, false),
                        new PXDataField(typeof(CSAttribute.controlType).Name),
                        new PXDataField(typeof(CSAttribute.entryMask).Name),
                        new PXDataField(typeof(CSAttribute.regExp).Name),
						PXDBLocalizableStringAttribute.GetValueSelect(typeof(CSAttribute).Name, typeof(CSAttribute.list).Name, true),
						new PXDataField(typeof(CSAttribute.isInternal).Name)
                        ))
                    {
                        Attribute attr = new Attribute(record);
                        Attributes.Add(attr);
                        AttributesByDescr.Add(attr);                      
                    }

                    foreach (PXDataRecord record in PXDatabase.SelectMulti<CSAttributeDetail>(
                        new PXDataField(typeof(CSAttributeDetail.attributeID).Name),
                        new PXDataField(typeof(CSAttributeDetail.valueID).Name),
						PXDBLocalizableStringAttribute.GetValueSelect(typeof(CSAttributeDetail).Name, typeof(CSAttributeDetail.description).Name, false),
						new PXDataField(typeof(CSAttributeDetail.disabled).Name),
                        new PXDataFieldOrder(typeof(CSAttributeDetail.attributeID).Name),
                        new PXDataFieldOrder(typeof(CSAttributeDetail.sortOrder).Name)
                        ))
                    {
                        string id = record.GetString(0);
                        Attribute attr;
                        if (Attributes.TryGetValue(id, out attr))
                        {
                            attr.AddValue(new AttributeValue(record));
                        }                        
                    }

                    foreach (PXDataRecord record in PXDatabase.SelectMulti<CSAttributeGroup>(
                       new PXDataField(typeof(CSAttributeGroup.entityType).Name),
                       new PXDataField(typeof(CSAttributeGroup.entityClassID).Name),
                       new PXDataField(typeof(CSAttributeGroup.attributeID).Name),
                       new PXDataField(typeof(CSAttributeGroup.defaultValue).Name),
                       new PXDataField(typeof(CSAttributeGroup.required).Name),
                       new PXDataFieldOrder(typeof(CSAttributeGroup.entityType).Name),
                       new PXDataFieldOrder(typeof(CSAttributeGroup.entityClassID).Name),                       
                       new PXDataFieldOrder(typeof(CSAttributeGroup.sortOrder).Name),
                       new PXDataFieldOrder(typeof(CSAttributeGroup.attributeID).Name)))
                    {
                        string type = record.GetString(0);
                        string classID = record.GetString(1);
                        string id = record.GetString(2);

                        Dictionary<string, ClassAttributeList> dict;
						AttributeList list;

						if (!EntityAttributes.TryGetValue(type, out list))
							EntityAttributes[type] = list = new AttributeList();

                        if (!ClassAttributes.TryGetValue(type, out dict))
													ClassAttributes[type] = dict = new Dictionary<string, ClassAttributeList>(StringComparer.InvariantCultureIgnoreCase);

                        ClassAttributeList group;
                        if (!dict.TryGetValue(classID, out group))
                            dict[classID] = group = new ClassAttributeList();

                        Attribute attr;
                        if (Attributes.TryGetValue(id, out attr))
                        {
							list.Add(attr);
                            group.Add(new AttributeExt(attr, record.GetString(3), record.GetBoolean(4) ?? false));
                        }
                    }
                }
            }
        }

        private static Definition Definitions
        {
            get
            {
                Definition defs = PX.Common.PXContext.GetSlot<Definition>();
                if (defs == null)
                {
					string currentLanguage = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
					if (!PXDBLocalizableStringAttribute.IsEnabled)
					{
						currentLanguage = "";
					}
					defs = PX.Common.PXContext.SetSlot<Definition>(PXDatabase.GetSlot<Definition>("CSAttributes" + currentLanguage, typeof(CSAttribute), typeof(CSAttributeDetail), typeof(CSAttributeGroup)));
                }
                return defs;
            }
        }

        public static AttributeList Attributes
        {
            get
            {             
                return Definitions.Attributes;
            }
        }
        public static AttributeList AttributesByDescr
        {
            get
            {
                return Definitions.AttributesByDescr;
            }
        }

        public static AttributeList EntityAttributes(string type)
		{
			AttributeList list;
			return Definitions.EntityAttributes.TryGetValue(type, out list) ? list : new AttributeList();
		}

        private static ClassAttributeList EntityAttributes(string type, string classID)
        {
            Dictionary<string, ClassAttributeList> typeList;
            ClassAttributeList list;
            if (type != null && classID != null &&
                Definitions.ClassAttributes.TryGetValue(type, out typeList) &&
                typeList.TryGetValue(classID, out list))
                    return list;
            
            return new ClassAttributeList();
        }

        public static ClassAttributeList EntityAttributes(Type entityType, string classID)
        {
            return EntityAttributes(entityType.FullName, classID);
        }
    }

    #region CSAttributeGroupList

    public class CSAttributeGroupList<TEntityClass, TEntity> : PXSelectBase<CSAttributeGroup>
        where TEntityClass : class, IBqlTable, new()
    {
        private readonly string _classIdFieldName;

        public CSAttributeGroupList(PXGraph graph)
        {
            _Graph = graph;

            var command = new Select3<CSAttributeGroup,
                InnerJoin<CSAttribute, On<CSAttributeGroup.attributeID, Equal<CSAttribute.attributeID>>>,
                OrderBy<Asc<CSAttributeGroup.entityClassID,
                    Asc<CSAttributeGroup.entityType, Asc<CSAttributeGroup.sortOrder>>>>>();

            View = new PXView(graph, false, command, new PXSelectDelegate(SelectDelegate));

            _classIdFieldName = _Graph.Caches[typeof (TEntityClass)].BqlKeys.Single().Name;

            _Graph.FieldDefaulting.AddHandler<CSAttributeGroup.entityType>((sender, e) =>
            {
                if (e.Row == null)
                    return;
                e.NewValue = typeof (TEntity).FullName;
            });
            _Graph.FieldDefaulting.AddHandler<CSAttributeGroup.entityClassID>((sender, e) =>
            {
                if (e.Row == null)
                    return;
                var entityClassCache = _Graph.Caches[typeof (TEntityClass)];
                e.NewValue = entityClassCache.GetValue(entityClassCache.Current, _classIdFieldName);
            });


            _Graph.FieldSelecting.AddHandler<CSAttributeGroup.defaultValue>(CSAttributeGroup_DefaultValue_FieldSelecting);

            AttachShowDetailsAction();
        }

        private void CSAttributeGroup_DefaultValue_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
        {
            CSAttributeGroup row = e.Row as CSAttributeGroup;

            if (row == null)
                return;

            const string answerValueField = "DefaultValue";
            const int answerValueLength = 60;

            CSAttribute question = new PXSelect<CSAttribute>(_Graph).Search<CSAttribute.attributeID>(row.AttributeID);

            PXResultset<CSAttributeDetail> options = PXSelect<CSAttributeDetail,
                Where<CSAttributeDetail.attributeID, Equal<Required<CSAttributeGroup.attributeID>>>,
                OrderBy<Asc<CSAttributeDetail.sortOrder>>>.Select(_Graph, row.AttributeID);

            int required = row.Required.GetValueOrDefault() ? 1 : -1;

            //if (options.Count > 0)
            if (options.Count > 0 &&
                (question == null || question.ControlType == CSAttribute.Combo ||
                 question.ControlType == CSAttribute.MultiSelectCombo))
            {
                //ComboBox:

                List<string> allowedValues = new List<string>();
                List<string> allowedLabels = new List<string>();

                foreach (CSAttributeDetail option in options)
                {
                    allowedValues.Add(option.ValueID);
                    allowedLabels.Add(option.Description);
                }

                string mask = question != null ? question.EntryMask : null;

                e.ReturnState = PXStringState.CreateInstance(e.ReturnState, CSAttributeDetail.ParameterIdLength,
                    true, answerValueField, false, required, mask, allowedValues.ToArray(), allowedLabels.ToArray(),
                    true, null);

                if (question.ControlType == CSAttribute.MultiSelectCombo)
                {
                    ((PXStringState) e.ReturnState).MultiSelect = true;
                }
            }
            else if (question != null)
            {
                if (question.ControlType.GetValueOrDefault() == CSAttribute.CheckBox)
                {
                    e.ReturnState = PXFieldState.CreateInstance(e.ReturnState, typeof (bool), false, false, required,
                        null, null, false, answerValueField, null, null, null, PXErrorLevel.Undefined, true, true,
                        null, PXUIVisibility.Visible, null, null, null);
                }
                else if (question.ControlType.GetValueOrDefault() == CSAttribute.Datetime)
                {
                    e.ReturnState = PXDateState.CreateInstance(e.ReturnState, answerValueField, false, required,
                        question.EntryMask, question.EntryMask, null, null);
                }
                else
                {
                    //TextBox:
                    e.ReturnState = PXStringState.CreateInstance(e.ReturnState, answerValueLength, null,
                        answerValueField, false, required, question.EntryMask, null, null, true, null);
                }
            }
        }

        private void AttachShowDetailsAction()
        {
            PXUIFieldAttribute fieldAttribute = new PXUIFieldAttribute
            {
                DisplayName = Messages.Details,
                MapEnableRights = PXCacheRights.Select,
                MapViewRights = PXCacheRights.Select,
                Visible = false
            };
            var addAttrs = new List<PXEventSubscriberAttribute> {fieldAttribute};

            const string actionName = "ShowDetails";

            var res = new PXNamedAction<TEntityClass>(_Graph, actionName, ShowDetails, addAttrs.ToArray());
            _Graph.Actions[actionName] = res;
        }


        [PXUIField(DisplayName = Messages.Details,
            MapEnableRights = PXCacheRights.Select,
            MapViewRights = PXCacheRights.Select,
            Visible = false)]
        [PXLookupButton]
        public virtual IEnumerable ShowDetails(PXAdapter adapter)
        {
            if (Current == null || Current.AttributeID == null)
                yield break;

            var attribute = (CSAttribute) PXSelect<CSAttribute,
                Where<CSAttribute.attributeID, Equal<Required<CSAttribute.attributeID>>>>.
                Select(_Graph, Current.AttributeID);

            if (attribute == null)
                yield break;

            var graph = PXGraph.CreateInstance<CSAttributeMaint>();
            graph.Clear();
            graph.Attributes.Current = attribute;
            throw new PXRedirectRequiredException(graph, Messages.CRAttributeMaint);
        }

        protected virtual IEnumerable SelectDelegate()
        {
            var entityClassCache = _Graph.Caches[typeof (TEntityClass)];
            var row = entityClassCache.Current;

            if (row == null)
                yield break;

            var classIdValue = entityClassCache.GetValue(row, _classIdFieldName);

            if (classIdValue == null)
                yield break;

            var resultSet =
                new PXSelectJoin
                    <CSAttributeGroup,
                        InnerJoin<CSAttribute, On<CSAttributeGroup.attributeID, Equal<CSAttribute.attributeID>>>,
                        Where<CSAttributeGroup.entityClassID, Equal<Required<CSAttributeGroup.entityClassID>>,
                             And<CSAttributeGroup.entityType, Equal<Required<CSAttributeGroup.entityType>>>>>(_Graph)
                    .Select(classIdValue.ToString(), typeof (TEntity).FullName);

            foreach (var record in resultSet)
            {
                var attributeGroup = PXResult.Unwrap<CSAttributeGroup>(record);

                if (attributeGroup != null)
                    yield return record;
            }
        }
    }

    #endregion

    #region CRAttributeList

    public class CRAttributeList<TEntity> : PXSelectBase<CSAnswers>        
    {
        private readonly EntityHelper _helper;

        public CRAttributeList(PXGraph graph)
        {
            _Graph = graph;
            _helper = new EntityHelper(graph);
            View = new PXView(graph, false,
                new Select3<CSAnswers, OrderBy<Asc<CSAnswers.order>>>(),
                new PXSelectDelegate(SelectDelegate));

            _Graph.EnsureCachePersistence(typeof(CSAnswers));
            PXDBAttributeAttribute.Activate(_Graph.Caches[typeof(TEntity)]);
            _Graph.FieldUpdating.AddHandler<CSAnswers.value>(FieldUpdatingHandler);
            _Graph.FieldSelecting.AddHandler<CSAnswers.value>(FieldSelectingHandler);
            _Graph.FieldSelecting.AddHandler<CSAnswers.isRequired>(IsRequiredSelectingHandler);
            _Graph.FieldSelecting.AddHandler<CSAnswers.attributeID>(AttrFieldSelectingHandler);
            _Graph.RowPersisting.AddHandler<CSAnswers>(RowPersistingHandler);
            _Graph.RowPersisting.AddHandler<TEntity>(ReferenceRowPersistingHandler);
            _Graph.RowUpdating.AddHandler<TEntity>(ReferenceRowUpdatingHandler);
			_Graph.RowDeleted.AddHandler<TEntity>(ReferenceRowDeletedHandler);
			_Graph.RowInserted.AddHandler<TEntity>(RowInsertedHandler); 
        }

        protected virtual IEnumerable SelectDelegate()
        {
            var currentObject = _Graph.Caches[typeof (TEntity)].Current;
            return SelectInternal(currentObject);
        }

        protected Guid? GetNoteId(object row)
        {
            return _helper.GetEntityNoteID(row);
        }

        private Type GetClassIdField(object row)
        {
            if (row == null)
                return null;


			var fieldAttribute =
                _Graph.Caches[row.GetType()].GetAttributes(row, null)
                    .OfType<CRAttributesFieldAttribute>()
                    .FirstOrDefault();

            if (fieldAttribute == null)
                return null;

            return fieldAttribute.ClassIdField;
        }

        private Type GetEntityTypeFromAttribute(object row)
        {
            var classIdField = GetClassIdField(row);
            if (classIdField == null)
                return null;

            return classIdField.DeclaringType;
        }

        private string GetClassId(object row)
        {
            var classIdField = GetClassIdField(row);
            if (classIdField == null)
                return null;

            var entityCache = _Graph.Caches[row.GetType()];

            var classIdValue = entityCache.GetValue(row, classIdField.Name);

            return classIdValue == null ? null : classIdValue.ToString();
        }

        protected IEnumerable<CSAnswers> SelectInternal(object row)
        {
            if (row == null)
                yield break;

            var noteId = GetNoteId(row);

            if (!noteId.HasValue)
                yield break;

            var answerCache = _Graph.Caches[typeof (CSAnswers)];
            var entityCache = _Graph.Caches[row.GetType()];

            List<CSAnswers> answerList;

            var status = entityCache.GetStatus(row);

            if (status == PXEntryStatus.Inserted || status == PXEntryStatus.InsertedDeleted)
            {
                answerList = answerCache.Inserted.Cast<CSAnswers>().Where(x => x.RefNoteID == noteId).ToList();
            }
            else
            {
                answerList = PXSelect<CSAnswers, Where<CSAnswers.refNoteID, Equal<Required<CSAnswers.refNoteID>>>>
                    .Select(_Graph, noteId).FirstTableItems.ToList();
            }

            var classId = GetClassId(row);

            CRAttribute.ClassAttributeList classAttributeList = new CRAttribute.ClassAttributeList();

            if (classId != null)
            {
                classAttributeList = CRAttribute.EntityAttributes(GetEntityTypeFromAttribute(row), classId);
            }
            //when coming from Import scenarios there might be attributes which don't belong to entity's current attribute class or the entity might not have any attribute class at all
            if (_Graph.IsImport && PXView.SortColumns.Any() && PXView.Searches.Any())
            {
                var columnIndex = Array.FindIndex(PXView.SortColumns,
                    x => x.Equals(typeof (CSAnswers.attributeID).Name, StringComparison.OrdinalIgnoreCase));

                if (columnIndex >= 0 && columnIndex < PXView.Searches.Length)
                {
                    var searchValue = PXView.Searches[columnIndex];

                    if (searchValue != null)
                    {
                        //searchValue can be either AttributeId or Description
                        var attributeDefinition = CRAttribute.Attributes[searchValue.ToString()] ??
                                             CRAttribute.AttributesByDescr[searchValue.ToString()];
                        
                        if (attributeDefinition == null)
                        {
                            throw new PXSetPropertyException(Messages.AttributeNotValid);
                        }
                        //avoid duplicates
                        else if (classAttributeList[attributeDefinition.ToString()] == null)
                        {
                            classAttributeList.Add(new CRAttribute.AttributeExt(attributeDefinition, null, false));
                        }
                    }
                }
            }

            if (answerList.Count == 0 && classAttributeList.Count == 0)
                yield break;

            //attribute identifiers that are contained in CSAnswers cache/table but not in class attribute list
            List<string> attributeIdListAnswers =
                answerList.Select(x => x.AttributeID)
                    .Except(classAttributeList.Select(x => x.ID))
                    .Distinct()
                    .ToList();

            //attribute identifiers that are contained in class attribute list but not in CSAnswers cache/table
            List<string> attributeIdListClass =
                classAttributeList.Select(x => x.ID)
                    .Except(answerList.Select(x => x.AttributeID))
                    .ToList();

            //attribute identifiers which belong to both lists
            List<string> attributeIdListIntersection =
                classAttributeList.Select(x => x.ID)
                    .Intersect(answerList.Select(x => x.AttributeID))
                    .Distinct()
                    .ToList();


            var cacheIsDirty = answerCache.IsDirty;

            List<CSAnswers> output = new List<CSAnswers>();

            //attributes contained only in CSAnswers cache/table should be added "as is"
            output.AddRange(answerList.Where(x => attributeIdListAnswers.Contains(x.AttributeID)));

                //attributes contained only in class attribute list should be created and initialized with default value
                foreach (var attributeId in attributeIdListClass)
                {
                    var classAttributeDefinition = classAttributeList[attributeId];

                    if (PXSiteMap.IsPortal && classAttributeDefinition.IsInternal)
                        continue;

                    CSAnswers answer = (CSAnswers) answerCache.CreateInstance();
                    answer.AttributeID = classAttributeDefinition.ID;
                    answer.RefNoteID = noteId;
                    answer.Value = GetDefaultAnswerValue(classAttributeDefinition);
                    if (classAttributeDefinition.ControlType == CSAttribute.CheckBox)
                    {
                        bool value;
                        if(bool.TryParse(answer.Value, out value))
                            answer.Value = Convert.ToInt32(value).ToString(CultureInfo.InvariantCulture);
                        else if (answer.Value == null)
                            answer.Value = 0.ToString();
                    }

                    answer.IsRequired = classAttributeDefinition.Required;
                    answer = (CSAnswers) (answerCache.Insert(answer) ?? answerCache.Locate(answer));
                    output.Add(answer);
                }

            //attributes belonging to both lists should be selected from CSAnswers cache/table with and additional IsRequired check against class definition
            foreach (CSAnswers answer in answerList.Where(x => attributeIdListIntersection.Contains(x.AttributeID)).ToList())
            {
                var classAttributeDefinition = classAttributeList[answer.AttributeID];

                if (PXSiteMap.IsPortal && classAttributeDefinition.IsInternal)
                    continue;

                if (answer.Value == null && classAttributeDefinition.ControlType == CSAttribute.CheckBox)
                    answer.Value = bool.FalseString;

                if (answer.IsRequired == null || classAttributeDefinition.Required != answer.IsRequired)
                {
                    answer.IsRequired = classAttributeDefinition.Required;

                    var fieldState = View.Cache.GetValueExt<CSAnswers.isRequired>(answer) as PXFieldState;
                    var fieldValue = fieldState != null && ((bool?)fieldState.Value).GetValueOrDefault();

                    answer.IsRequired = classAttributeDefinition.Required || fieldValue;
                }

                

                output.Add(answer);
            }

            answerCache.IsDirty = cacheIsDirty;

            output =
                output.OrderBy(
                    x =>
                        classAttributeList.Contains(x.AttributeID)
                            ? classAttributeList.IndexOf(x.AttributeID)
                            : (x.Order ?? 0))
                    .ThenBy(x => x.AttributeID)
                    .ToList();

            short attributeOrder = 0;

            foreach (CSAnswers answer in output)
            {
                answer.Order = attributeOrder++;
                yield return answer;
            }
        }

        private void FieldUpdatingHandler(PXCache sender, PXFieldUpdatingEventArgs e)
        {
            var row = e.Row as CSAnswers;


            if (row == null || !(e.NewValue is string) || row.AttributeID == null)
                return;

            var attr = CRAttribute.Attributes[row.AttributeID];
            if (attr == null)
                return;

            var newValue = (string) e.NewValue;
            switch (attr.ControlType)
            {
                case CSAttribute.CheckBox:
                    bool value;
                    if (bool.TryParse(newValue, out value))
                    {
                        e.NewValue = Convert.ToInt32(value).ToString(CultureInfo.InvariantCulture);
                    }
                    break;
                case CSAttribute.Datetime:
                    DateTime dt;
                    if (sender.Graph.IsMobile)
                    {
                        newValue = newValue.Replace("Z", "");
                        if (DateTime.TryParse(newValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            e.NewValue = dt.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        }
                    }
                    else
                    {
                        if (DateTime.TryParse(newValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            e.NewValue = dt.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        }
                    }
                    break;
            }
        }

        private void FieldSelectingHandler(PXCache sender, PXFieldSelectingEventArgs e)
        {
            var row = e.Row as CSAnswers;
            if (row == null) return;

            var question = CRAttribute.Attributes[row.AttributeID];

            var options = question != null ? question.Values : null;

            var required = row.IsRequired == true ? 1 : -1;

            if (options != null && options.Count > 0)
            {
                //ComboBox:
                var allowedValues = new List<string>();
                var allowedLabels = new List<string>();

                foreach (var option in options)
                {
                    if (option.Disabled && row.Value != option.ValueID) continue;

                    allowedValues.Add(option.ValueID);
                    allowedLabels.Add(option.Description);
                }

                e.ReturnState = PXStringState.CreateInstance(e.ReturnState, CSAttributeDetail.ParameterIdLength,
                    true, typeof(CSAnswers.value).Name, false, required, question.EntryMask, allowedValues.ToArray(),
                    allowedLabels.ToArray(), true, null);
                if (question.ControlType == CSAttribute.MultiSelectCombo)
                {
                    ((PXStringState)e.ReturnState).MultiSelect = true;
                }
            }
            else if (question != null)
            {
                if (question.ControlType == CSAttribute.CheckBox)
                {
                    e.ReturnState = PXFieldState.CreateInstance(e.ReturnState, typeof(bool), false, false, required,
                        null, null, false, typeof(CSAnswers.value).Name, null, null, null, PXErrorLevel.Undefined, true,
                        true, null,
                        PXUIVisibility.Visible, null, null, null);
                    if (e.ReturnValue is string)
                    {
                        int value;
                        if (int.TryParse((string)e.ReturnValue, NumberStyles.Integer, CultureInfo.InvariantCulture,
                            out value))
                        {
                            e.ReturnValue = Convert.ToBoolean(value);
                        }
                    }
                }
                else if (question.ControlType == CSAttribute.Datetime)
                {
                    e.ReturnState = PXDateState.CreateInstance(e.ReturnState, typeof(CSAnswers.value).Name, false,
                        required, question.EntryMask, question.EntryMask,
                        null, null);
                }
                else
                {
                    //TextBox:					
                    var vstate = sender.GetStateExt<CSAnswers.value>(null) as PXStringState;
                    e.ReturnState = PXStringState.CreateInstance(e.ReturnState, vstate.With(_ => _.Length), null,
                        typeof(CSAnswers.value).Name,
                        false, required, question.EntryMask, null, null, true, null);
                }
            }
            if (e.ReturnState != null)
            {
                var error = PXUIFieldAttribute.GetError<CSAnswers.value>(sender, row);
                if (error != null)
                {
                    var state = (PXFieldState)e.ReturnState;
                    state.Error = error;
                    state.ErrorLevel = PXErrorLevel.RowError;
                }
            }
        }

        private void AttrFieldSelectingHandler(PXCache sender, PXFieldSelectingEventArgs e)
        {
            PXUIFieldAttribute.SetEnabled<CSAnswers.attributeID>(sender, e.Row, false);
        }

        private void IsRequiredSelectingHandler(PXCache sender, PXFieldSelectingEventArgs e)
        {
            var row = e.Row as CSAnswers;
            var current = sender.Graph.Caches[typeof(TEntity)].Current;

            if (row == null || current == null)
                return;
            var currentNoteId = GetNoteId(current);

            if (e.ReturnValue != null || row.RefNoteID != currentNoteId)
                return;

            //when importing data - make all attributes nonrequired (otherwise import might fail)
            if (sender.Graph.IsImport)
            {
                e.ReturnValue = false;
                return;
            }

            var currentClassId = GetClassId(current);

            var attribute = CRAttribute.EntityAttributes(GetEntityTypeFromAttribute(current), currentClassId)[row.AttributeID];

            if (attribute == null)
            {
                e.ReturnValue = false;
            }
            else
            {
                if (PXSiteMap.IsPortal && attribute.IsInternal)
                    e.ReturnValue = false;
                else
                    e.ReturnValue = attribute.Required;
            }
        }

        private void RowPersistingHandler(PXCache sender, PXRowPersistingEventArgs e)
        {
            if (e.Operation != PXDBOperation.Insert && e.Operation != PXDBOperation.Update) return;

            var row = e.Row as CSAnswers;
            if (row == null) return;

            if (!row.RefNoteID.HasValue)
            {
                e.Cancel = true;
                RowPersistDeleted(sender, row);
            }
            else if (string.IsNullOrEmpty(row.Value))
            {
                var mayNotBeEmpty = PXMessages.LocalizeFormatNoPrefix(ErrorMessages.FieldIsEmpty,
                    sender.GetStateExt<CSAnswers.value>(null).With(_ => _ as PXFieldState).With(_ => _.DisplayName));
                if (row.IsRequired == true &&
                    sender.RaiseExceptionHandling<CSAnswers.value>(e.Row, row.Value,
                        new PXSetPropertyException(mayNotBeEmpty, PXErrorLevel.RowError, typeof(CSAnswers.value).Name)))
                {
                    throw new PXRowPersistingException(typeof(CSAnswers.value).Name, row.Value, mayNotBeEmpty,
                        typeof(CSAnswers.value).Name);
                }
                e.Cancel = true;
                if (sender.GetStatus(row) != PXEntryStatus.Inserted)
                    RowPersistDeleted(sender, row);
            }
        }

        private void RowPersistDeleted(PXCache cache, object row)
        {
            try
            {
                cache.PersistDeleted(row);
                cache.SetStatus(row, PXEntryStatus.InsertedDeleted);
            }
            catch (PXLockViolationException)
            {
            }
            cache.ResetPersisted(row);
        }
		private void ReferenceRowDeletedHandler(PXCache sender, PXRowDeletedEventArgs e)
		{
			object row = e.Row;
			if (row == null) return;

			var noteId = GetNoteId(row);

			if (!noteId.HasValue) return;

			var answerCache = _Graph.Caches[typeof(CSAnswers)];
			var entityCache = _Graph.Caches[row.GetType()];

			List<CSAnswers> answerList;

			var status = entityCache.GetStatus(row);

			if (status == PXEntryStatus.Inserted || status == PXEntryStatus.InsertedDeleted)
			{
				answerList = answerCache.Inserted.Cast<CSAnswers>().Where(x => x.RefNoteID == noteId).ToList();
			}
			else
			{
				answerList = PXSelect<CSAnswers, Where<CSAnswers.refNoteID, Equal<Required<CSAnswers.refNoteID>>>>
					.Select(_Graph, noteId).FirstTableItems.ToList();
			}
			foreach (var answer in answerList)
				this.Cache.Delete(answer);
		}

		private void ReferenceRowPersistingHandler(PXCache sender, PXRowPersistingEventArgs e)
        {
            var row = e.Row;
            
            if (row == null) return;

            var answersCache = _Graph.Caches[typeof(CSAnswers)];

            var emptyRequired = new List<string>();
            foreach (CSAnswers answer in answersCache.Cached)
            {
                if (answer.IsRequired == null)
                {
                    var state = View.Cache.GetValueExt<CSAnswers.isRequired>(answer) as PXFieldState;
                    if (state != null)
                        answer.IsRequired = state.Value as bool?;
                }

                if (e.Operation == PXDBOperation.Delete)
                {
                    answersCache.Delete(answer);
                }
                else if (string.IsNullOrEmpty(answer.Value) && answer.IsRequired == true && !_Graph.UnattendedMode)
                {
                    var displayName = "";

                    var attributeDefinition = CRAttribute.Attributes[answer.AttributeID];
                    if (attributeDefinition != null)
                        displayName = attributeDefinition.Description;

                    emptyRequired.Add(displayName);
                    var mayNotBeEmpty = PXMessages.LocalizeFormatNoPrefix(ErrorMessages.FieldIsEmpty, displayName);
                    answersCache.RaiseExceptionHandling<CSAnswers.value>(answer, answer.Value,
                        new PXSetPropertyException(mayNotBeEmpty, PXErrorLevel.RowError, typeof(CSAnswers.value).Name));
                    PXUIFieldAttribute.SetError<CSAnswers.value>(answersCache, answer, mayNotBeEmpty);
                }
            }
            if (emptyRequired.Count > 0)
                throw new PXException(Messages.RequiredAttributesAreEmpty,
                    string.Join(", ", emptyRequired.Select(s => string.Format("'{0}'", s))));
        }

	    private void ReferenceRowUpdatingHandler(PXCache sender, PXRowUpdatingEventArgs e)
	    {
		    var row = e.Row;
		    var newRow = e.NewRow;

		    if (row == null || newRow == null)
			    return;

		    var rowNoteId = GetNoteId(row);

		    var rowClassId = GetClassId(row);
		    var newRowClassId = GetClassId(newRow);

		    if (string.Equals(rowClassId, newRowClassId, StringComparison.InvariantCultureIgnoreCase))
			    return;

		    var newAttrList = new HashSet<string>();

		    if (newRowClassId != null)
		    {
			    foreach (var attr in CRAttribute.EntityAttributes(GetEntityTypeFromAttribute(newRow), newRowClassId))
			    {
				    newAttrList.Add(attr.ID);
			    }
		    }
		    var relatedEntityTypes =
			    sender.GetAttributesOfType<CRAttributesFieldAttribute>(newRow, null).FirstOrDefault()?.RelatedEntityTypes;

		    PXGraph entityGraph = new PXGraph();
			var entityHelper = new EntityHelper(entityGraph);

			if (relatedEntityTypes != null)
				foreach (var classField in relatedEntityTypes)
				{
					object entity = entityHelper.GetEntityRow(classField.DeclaringType, rowNoteId);
					if(entity == null) continue;
					string entityClass =  (string) entityGraph.Caches[classField.DeclaringType].GetValue(entity, classField.Name);
					if(entityClass == null) continue;
					CRAttribute.EntityAttributes(classField.DeclaringType, entityClass)
						.Where(x => !newAttrList.Contains(x.ID)).Select(x => x.ID)
						.ForEach(x => newAttrList.Add(x));
				}
			
			foreach (CSAnswers answersRow in
			    PXSelect<CSAnswers,
				    Where<CSAnswers.refNoteID, Equal<Required<CSAnswers.refNoteID>>>>
				    .SelectMultiBound(sender.Graph, null, rowNoteId))
		    {
			    var copy = PXCache<CSAnswers>.CreateCopy(answersRow);
			    View.Cache.Delete(answersRow);
			    if (newAttrList.Contains(copy.AttributeID))
			    {
				    View.Cache.Insert(copy);
			    }
		    }

		    if (newRowClassId != null)
			    SelectInternal(newRow).ToList();

		    sender.IsDirty = true;
	    }

	    private void RowInsertedHandler(PXCache sender, PXRowInsertedEventArgs e)
		{
		    if (sender != null && sender.Graph != null && !sender.Graph.IsImport)
		        SelectInternal(e.Row).ToList();
		}

        private void CopyAttributes(object destination, object source, bool copyall)
        {
            if (destination == null || source == null) return;

            var sourceAttributes = SelectInternal(source).RowCast<CSAnswers>().ToList();
            var targetAttributes = SelectInternal(destination).RowCast<CSAnswers>().ToList();

            var answerCache = _Graph.Caches<CSAnswers>();
  

            foreach (var targetAttribute in targetAttributes)
            {
                var sourceAttr = sourceAttributes.SingleOrDefault(x => x.AttributeID == targetAttribute.AttributeID);

                if (sourceAttr == null || string.IsNullOrEmpty(sourceAttr.Value) ||
                    sourceAttr.Value == targetAttribute.Value)
                    continue;

            if (string.IsNullOrEmpty(targetAttribute.Value) || copyall)
                {
                    var answer = PXCache<CSAnswers>.CreateCopy(targetAttribute);
                    answer.Value = sourceAttr.Value;
                    answerCache.Update(answer);
                }
            }
        }

        public void CopyAllAttributes(object row, object src)
        {
            CopyAttributes(row, src, true);
        }

        public void CopyAttributes(object row, object src)
        {
            CopyAttributes(row, src, false);
        }

        protected virtual string GetDefaultAnswerValue(CRAttribute.AttributeExt attr)
        {
            return attr.DefaultValue;
        }
    }

#endregion

    #region CRAttributeSourceList

    public class CRAttributeSourceList<TReference, TSourceField> : CRAttributeList<TReference>
        where TReference : class, IBqlTable, new()
        where TSourceField : IBqlField
    {
        public CRAttributeSourceList(PXGraph graph)
            : base(graph)
        {
            _Graph.FieldUpdated.AddHandler<TSourceField>(ReferenceSourceFieldUpdated);
        }


        private object _AttributeSource;

        protected object AttributeSource
        {
            get
            {
                var cache = _Graph.Caches<TReference>();

                var noteFieldName = EntityHelper.GetNoteField(typeof(TReference));

                if (_AttributeSource == null ||
                    GetNoteId(_AttributeSource) != (Guid?)cache.GetValue(cache.Current, noteFieldName))
                {
                    _AttributeSource = PXSelectorAttribute.Select<TSourceField>(cache, cache.Current);
                }
                return _AttributeSource;
            }
        }

        protected override string GetDefaultAnswerValue(CRAttribute.AttributeExt attr)
        {
            if (AttributeSource == null)
                return base.GetDefaultAnswerValue(attr);

            var sourceNoteId = GetNoteId(AttributeSource);

            var answers =
                PXSelect<CSAnswers, Where<CSAnswers.refNoteID, Equal<Required<CSAnswers.refNoteID>>>>.Select(_Graph, sourceNoteId);

            foreach (CSAnswers answer in answers)
            {
                if (answer.AttributeID == attr.ID && !string.IsNullOrEmpty(answer.Value))
                    return answer.Value;
            }

            return base.GetDefaultAnswerValue(attr);
        }

        protected void ReferenceSourceFieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            CopyAttributes(e.Row, AttributeSource);
        }
    }
    #endregion

    #region AddressSelectBase

    public abstract class AddressSelectBase : PXSelectBase, ICacheType<Address>
	{
		private const string _BUTTON_ACTION = "ValidateAddress"; //TODO: need concat with graph selector name;
		private const string _VIEWONMAP_ACTION = "ViewOnMap";

		protected Type _itemType;

		protected int _addressIdFieldOrdinal;
		protected int _asMainFieldOrdinal;
		private int _accountIdFieldOrdinal;

		private PXAction _action;
		private PXAction _mapAction;

		protected AddressSelectBase(PXGraph graph)
		{
			Initialize(graph);
			CreateView(graph);
			AttacheHandlers(graph);
			AppendButton(graph);
		}

		public bool DoNotCorrectUI { get; set; }

		private void AppendButton(PXGraph graph)
		{
			_action = PXNamedAction.AddAction(graph, _itemType, _BUTTON_ACTION, CS.Messages.ValidateAddress, CS.Messages.ValidateAddress, ValidateAddress);
			_mapAction = PXNamedAction.AddAction(graph, _itemType, _VIEWONMAP_ACTION, Messages.ViewOnMap, ViewOnMap);
		}

		private void Initialize(PXGraph graph)
		{
			_Graph = graph;
			_Graph.EnsureCachePersistence(typeof(Address));
			_Graph.Initialized += sender => sender.Views.Caches.Remove(IncorrectPersistableDAC);

			var addressIdDAC = GetDAC(AddressIdField);
			var asMainDAC = GetDAC(AsMainField);
			var accounDAC = GetDAC(AccountIdField);
			if (addressIdDAC != asMainDAC || asMainDAC != accounDAC)
				throw new Exception(string.Format("Fields '{0}', '{1}' and '{2}' are defined in different DACs",
					addressIdDAC.Name, asMainDAC.Name, accounDAC));
			_itemType = addressIdDAC;

			var cache = _Graph.Caches[_itemType];
			_addressIdFieldOrdinal = cache.GetFieldOrdinal(AddressIdField.Name);
			_asMainFieldOrdinal = cache.GetFieldOrdinal(AsMainField.Name);
			_accountIdFieldOrdinal = cache.GetFieldOrdinal(AccountIdField.Name);
		}

		protected abstract Type AccountIdField { get; }

		protected abstract Type AsMainField { get; }

		protected abstract Type AddressIdField { get; }

		protected abstract Type IncorrectPersistableDAC { get; }

		private static Type GetDAC(Type type)
		{
			var res = type.DeclaringType;
			if (res == null)
				throw new Exception(string.Format("DAC for field '{0}' can not be found", type.Name));
			return res;
		}

		private void CreateView(PXGraph graph)
		{
			var command = new Select<Address>();
			View = new PXView(graph, false, command, new PXSelectDelegate(SelectDelegate));
		}

		private void AttacheHandlers(PXGraph graph)
		{
			graph.RowInserted.AddHandler(_itemType, RowInsertedHandler);
			graph.RowUpdating.AddHandler(_itemType, RowUpdatingHandler);
			graph.RowUpdated.AddHandler(_itemType, RowUpdatedHandler);
			graph.RowSelected.AddHandler(_itemType, RowSelectedHandler);
			graph.RowDeleted.AddHandler(_itemType, RowDeletedHandler);
		}

		private void RowDeletedHandler(PXCache sender, PXRowDeletedEventArgs e)
		{
			var currentAddressId = sender.GetValue(e.Row, _addressIdFieldOrdinal);
			var isMainAddress = sender.GetValue(e.Row, AsMainField.Name) as bool?;
			if (isMainAddress == true) return;

			var currentAddress = currentAddressId.
				With(_ => (Address)PXSelect<Address,
					Where<Address.addressID, Equal<Required<Address.addressID>>>>.
				Select(sender.Graph, _));
			if (currentAddress != null)
			{
				var addressCache = sender.Graph.Caches[typeof(Address)];
				addressCache.Delete(currentAddress);
			}
		}

		private void RowSelectedHandler(PXCache sender, PXRowSelectedEventArgs e)
		{
			var addressCache = sender.Graph.Caches[typeof(Address)];
			var asMain = false;
			var isValidated = false;

			var accountId = sender.GetValue(e.Row, _accountIdFieldOrdinal);
			var account = accountId.
					With(_ => (BAccount)PXSelect<BAccount,
						Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>.
					Select(_Graph, _));
			var accountAddressId = account.With(_ => _.DefAddressID);
			var containsAccount = accountAddressId != null;

			var currentAddressId = sender.GetValue(e.Row, _addressIdFieldOrdinal);
			var currentAddress = currentAddressId.
				With(_ => (Address)PXSelect<Address,
					Where<Address.addressID, Equal<Required<Address.addressID>>>>.
				Select(sender.Graph, _));
			if (currentAddress != null)
			{
				isValidated = currentAddress.IsValidated == true;

				if (currentAddressId.Equals(accountAddressId))
					asMain = true;
			}
			else
			{
				PXEntryStatus status = sender.GetStatus(e.Row);
				if (status != PXEntryStatus.Inserted && status != PXEntryStatus.Deleted && status != PXEntryStatus.InsertedDeleted)
				{
					sender.SetValue(e.Row, _addressIdFieldOrdinal, null);
					RowInsertedHandler(sender, new PXRowInsertedEventArgs(e.Row, true));
					sender.SetStatus(e.Row, PXEntryStatus.Updated);
				}
			}

			sender.SetValue(e.Row, _asMainFieldOrdinal, asMain);
			if (!DoNotCorrectUI)
			{
				PXUIFieldAttribute.SetEnabled(addressCache, currentAddress, !asMain);
				PXUIFieldAttribute.SetEnabled(sender, e.Row, AsMainField.Name, containsAccount);
			}
			_action.SetEnabled(!isValidated);
		}

		protected virtual IEnumerable SelectDelegate()
		{
			var primaryCache = _Graph.Caches[_itemType];
			var primaryRecord = GetPrimaryRow();
			var currentAddressId = primaryCache.GetValue(primaryRecord, _addressIdFieldOrdinal);

			yield return (Address)PXSelect<Address,
				Where<Address.addressID, Equal<Required<Address.addressID>>>>.
				Select(_Graph, currentAddressId);
		}

		protected Type ItemType
		{
			get { return _itemType; }
		}

		protected abstract object GetPrimaryRow();

		private void RowInsertedHandler(PXCache sender, PXRowInsertedEventArgs e)
		{
			var row = e.Row;
			var asMain = sender.GetValue(row, _asMainFieldOrdinal);
			var accountId = sender.GetValue(row, _accountIdFieldOrdinal);
			var account = accountId.
				With(_ => (BAccount)PXSelect<BAccount,
					Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>.
				Select(_Graph, _));
			var accountAddressId = account.With(_ => _.DefAddressID);
			var accountAddress = accountAddressId.
				With<int?, Address>(_ => (Address)PXSelect<Address,
					Where<Address.addressID, Equal<Required<Address.addressID>>>>.
				Select(sender.Graph, _));
			var currentAddressId = sender.GetValue(row, _addressIdFieldOrdinal);

			if (accountAddress == null)
			{
				asMain = false;
				sender.SetValue(row, _asMainFieldOrdinal, false);
			}

			var addressCache = sender.Graph.Caches[typeof(Address)];
			if (accountAddress != null && true.Equals(asMain))
			{
				if (currentAddressId != null && !object.Equals(currentAddressId, accountAddressId))
				{
					var currentAddress = (Address)PXSelect<Address,
						Where<Address.addressID, Equal<Required<Address.addressID>>>>.
						Select(sender.Graph, currentAddressId);
					var oldDirty = addressCache.IsDirty;
					addressCache.Delete(currentAddress);
					addressCache.IsDirty = oldDirty;
				}
				sender.SetValue(row, _addressIdFieldOrdinal, accountAddressId);
			}
			else
			{
				if (currentAddressId == null || object.Equals(currentAddressId, accountAddressId))
				{
					var oldDirty = addressCache.IsDirty;
					Address addr;
					if (accountAddress != null)
					{
						addr = (Address)addressCache.CreateCopy(accountAddress);
					}
					else
					{
						addr = (Address)addressCache.CreateInstance();
					}
					addr.AddressID = null;
					addr.BAccountID = (int?)accountId;
					addr = (Address)addressCache.Insert(addr);

					sender.SetValue(row, _addressIdFieldOrdinal, addr.AddressID);
					addressCache.IsDirty = oldDirty;
				}
			}
		}

		private void RowUpdatingHandler(PXCache sender, PXRowUpdatingEventArgs e)
		{
			var row = e.NewRow;
			var oldRow = e.Row;

			var asMain = sender.GetValue(row, _asMainFieldOrdinal);
			var oldAsMain = sender.GetValue(oldRow, _asMainFieldOrdinal);

			var addressId = sender.GetValue(row, _addressIdFieldOrdinal);

			var accountId = sender.GetValue(row, _accountIdFieldOrdinal);
			var account = accountId.
				With(_ => (BAccount)PXSelect<BAccount,
					Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>.
				Select(_Graph, _));
			var accountAddressId = account.With(_ => _.DefAddressID);
			var accountAddress = accountAddressId.
				With<int?, Address>(_ => (Address)PXSelect<Address,
					Where<Address.addressID, Equal<Required<Address.addressID>>>>.
				Select(sender.Graph, _));

			var oldAccountId = sender.GetValue(oldRow, _accountIdFieldOrdinal);
			if (!object.Equals(accountId, oldAccountId))
			{
				var oldAddressId = sender.GetValue(row, _addressIdFieldOrdinal);
				var oldAccount = accountId.
					With(_ => (BAccount)PXSelect<BAccount,
						Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>.
					Select(_Graph, _));
				var oldAccountAddressId = oldAccount.With(_ => _.DefAddressID);
				var oldAccountAddress = oldAccountAddressId.
					With<int?, Address>(_ => (Address)PXSelect<Address,
						Where<Address.addressID, Equal<Required<Address.addressID>>>>.
					Select(sender.Graph, _));
				oldAsMain = oldAccountAddress != null && object.Equals(oldAddressId, oldAccountAddressId);
				if (true.Equals(oldAsMain))
				{
					asMain = true;
					addressId = accountAddressId;
					sender.SetValue(row, _addressIdFieldOrdinal, accountAddressId);
				} 
			}

			if (true.Equals(asMain))
			{
				
				if (accountAddress == null)
				{
					asMain = false;
					sender.SetValue(row, _asMainFieldOrdinal, false);
				}
			}

			if (!object.Equals(asMain, oldAsMain))
			{
				if (true.Equals(asMain))
				{
					sender.SetValue(row, _addressIdFieldOrdinal, accountAddressId);
				}
				else
				{
					if (object.Equals(accountAddressId, addressId))
						sender.SetValue(row, _addressIdFieldOrdinal, null);
				}
			}
		}

		private void RowUpdatedHandler(PXCache sender, PXRowUpdatedEventArgs e)
		{
			var row = e.Row;
			var oldRow = e.OldRow;

			var accountId = sender.GetValue(row, _accountIdFieldOrdinal);
			var addressId = sender.GetValue(row, _addressIdFieldOrdinal);
			var oldAddressId = sender.GetValue(oldRow, _addressIdFieldOrdinal);
			var addressCache = _Graph.Caches[typeof(Address)];
			if (!object.Equals(addressId, oldAddressId))
			{
				var account = accountId.
					With(_ => (BAccount)PXSelect<BAccount,
						Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>.
					Select(_Graph, _));
				var accountAddressId = account.With(_ => _.DefAddressID);
				var accountWithDefAddress = oldAddressId.
					With(_ => (BAccount)PXSelect<BAccount,
						Where<BAccount.defAddressID, Equal<Required<BAccount.defAddressID>>>>.
					Select(_Graph, _));
				if (accountWithDefAddress == null)
				{
					var oldAddress = oldAddressId.
						With(_ => (Address)PXSelect<Address, 
							Where<Address.addressID, Equal<Required<Address.addressID>>>>.
						Select(_Graph, _));
					if (oldAddress != null)
					{
						var oldIsDirty = addressCache.IsDirty;
						addressCache.Delete(oldAddress);
						addressCache.IsDirty = oldIsDirty;
					}
				}

				if (addressId == null)
				{
					var oldDirty = addressCache.IsDirty;
					Address addr;
					var accountAddress = accountAddressId.
						With<int?, Address>(_ => (Address)PXSelect<Address,
							Where<Address.addressID, Equal<Required<Address.addressID>>>>.
						Select(_Graph, _));
					if (accountAddress != null && object.Equals(accountAddressId, oldAddressId))
					{
						addr = (Address)addressCache.CreateCopy(accountAddress);
					}
					else
					{
						addr = (Address)addressCache.CreateInstance();
					}
					if (addr != null)
					{
						addr.AddressID = null;
						addr.BAccountID = (int?)accountId;
						addr = (Address)addressCache.Insert(addr);

						sender.SetValue(row, _addressIdFieldOrdinal, addr.AddressID);
						sender.SetValue(row, _asMainFieldOrdinal, false);
						addressCache.IsDirty = oldDirty;
						addressId = addr.AddressID;
					}
				}
			}
			if (addressId == null)
			{
				var oldDirty = addressCache.IsDirty;
				var addr = (Address)addressCache.CreateInstance();
				addr.AddressID = null;
				addr.BAccountID = (int?)accountId;
				addr = (Address)addressCache.Insert(addr);

				sender.SetValue(row, _addressIdFieldOrdinal, addr.AddressID);
				sender.SetValue(row, _asMainFieldOrdinal, false);
				addressCache.IsDirty = oldDirty;
			}
		}

		[PXUIField(DisplayName = CS.Messages.ValidateAddress, FieldClass = CS.Messages.ValidateAddress)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.Process)]
		protected virtual IEnumerable ValidateAddress(PXAdapter adapter)
		{
			var graph = adapter.View.Graph;
			var primaryCache = graph.Caches[_itemType];
			var primaryRecord = primaryCache.Current;
			if (primaryRecord != null)
			{
				var addressId = primaryCache.GetValue(primaryRecord, _addressIdFieldOrdinal);
				var address = addressId.With(_ => (Address)PXSelect<Address,
						Where<Address.addressID, Equal<Required<Address.addressID>>>>.
						Select(graph, _));
				if (address != null && address.IsValidated != true)
					PXAddressValidator.Validate<Address>(graph, address, true);
			}
			return adapter.Get();
		}

		[PXUIField(DisplayName = Messages.ViewOnMap)]
		[PXButton()]
		protected virtual IEnumerable ViewOnMap(PXAdapter adapter)
		{
			var graph = adapter.View.Graph;
			var primaryCache = graph.Caches[_itemType];
			var primaryRecord = primaryCache.Current;
			if (primaryRecord != null)
			{
				var currentAddressId = primaryCache.GetValue(primaryRecord, _addressIdFieldOrdinal);
				var currentAddress = currentAddressId.With(_ => (Address)PXSelect<Address,
						Where<Address.addressID, Equal<Required<Address.addressID>>>>.
						Select(graph, _));
				if (currentAddress != null)
					BAccountUtility.ViewOnMap(currentAddress);
			}
			return adapter.Get();
		}
	}

	#endregion

	#region AddressSelect

	public sealed class AddressSelect<TAddressIdField, TAsMainField, TAccountIdField> : AddressSelectBase
		where TAddressIdField : IBqlField
		where TAsMainField : IBqlField
		where TAccountIdField : IBqlField
	{
		public AddressSelect(PXGraph graph)
			: base(graph)
		{
		}

		protected override Type AccountIdField
		{
			get { return typeof(TAccountIdField); }
		}

		protected override Type AsMainField
		{
			get { return typeof(TAsMainField); }
		}

		protected override Type AddressIdField
		{
			get { return typeof(TAddressIdField); }
		}

		protected override Type IncorrectPersistableDAC
		{
			get { return typeof(TAddressIdField); }
		}

		protected override object GetPrimaryRow()
		{
			return _Graph.Caches[ItemType].Current;
		}
	}

	#endregion

	#region AddressSelect2

	public sealed class AddressSelect2<TAddressIdFieldSearch, TAsMainField, TAccountIdField> : AddressSelectBase
		where TAddressIdFieldSearch : IBqlSearch
		where TAsMainField : IBqlField
		where TAccountIdField : IBqlField
	{
		private Type _addressIdField;
		private PXView _select;

		public AddressSelect2(PXGraph graph)
			: base(graph)
		{
		}

		private void Initialize()
		{
			if (_addressIdField != null) return;

			var search = BqlCommand.CreateInstance(typeof(TAddressIdFieldSearch));
			_addressIdField = ((IBqlSearch)search).GetField();
			_select = new PXView(_Graph, false, GetSelectCommand(search));
		}

		private BqlCommand GetSelectCommand(BqlCommand search)
		{
			var arr = BqlCommand.Decompose(search.GetType());
			if (arr.Length < 2)
				throw new Exception("Unsupported search command detected");

			Type oldCommand = arr[0];
			Type newCommand = null;
			if (oldCommand == typeof(Search<,>))
				newCommand = typeof(Select<,>);
			if (oldCommand == typeof(Search2<,>))
				newCommand = typeof(Select2<,>);
			if (oldCommand == typeof(Search2<,,>))
				newCommand = typeof(Select2<,,>);

			if (newCommand == null)
				throw new Exception("Unsupported search command detected");

			arr[0] = newCommand;
			arr[1] = arr[1].DeclaringType;
			return BqlCommand.CreateInstance(arr);
		}

		protected override Type AccountIdField
		{
			get { return typeof(TAccountIdField); }
		}

		protected override Type AsMainField
		{
			get { return typeof(TAsMainField); }
		}

		protected override Type AddressIdField
		{
			get
			{
				Initialize();
				return _addressIdField;
			}
		}

		protected override Type IncorrectPersistableDAC
		{
			get { return typeof(TAddressIdFieldSearch); }
		}

		protected override object GetPrimaryRow()
		{
			var record = _select.SelectSingle();
			if (record is PXResult)
				record = ((PXResult)record)[ItemType];
			return record;
		}

		protected override IEnumerable SelectDelegate()
		{
			PXCache primaryCache = _Graph.Caches[_itemType];
			object primaryRecord = GetPrimaryRow();
			object currentAddressId = primaryCache.GetValue(primaryRecord, _addressIdFieldOrdinal);
			PXCache addrCache = _Graph.Caches<Address>();
			foreach (Address addr in PXSelect<Address, Where<Address.addressID, Equal<Required<Address.addressID>>>>.Select(_Graph, currentAddressId))
			{
				Address a = addr;
				bool? asMain = (bool?)primaryCache.GetValue(primaryRecord, _asMainFieldOrdinal);
				if (asMain == true)
				{
					a = PXCache<Address>.CreateCopy(a);
					PXUIFieldAttribute.SetEnabled(addrCache, a, false);
				}
				else
				{
					PXUIFieldAttribute.SetEnabled(addrCache, a, true);
				}
				yield return a;
			}
		}

	}

	#endregion

	#region ContactSelectBase

	public abstract class ContactSelectBase : PXSelectBase
	{
		protected Type _itemType;

		protected int _contactIdFieldOrdinal;
		protected int _asMainFieldOrdinal;
		private int _accountIdFieldOrdinal;

		protected ContactSelectBase(PXGraph graph)
		{
			Initialize(graph);
			CreateView(graph);
			AttacheHandlers(graph);
		}

		public bool DoNotCorrectUI { get; set; }

		private void Initialize(PXGraph graph)
		{
			_Graph = graph;
			_Graph.EnsureCachePersistence(typeof(Contact));
			_Graph.Initialized += sender => sender.Views.Caches.Remove(IncorrectPersistableDAC);

			var contactIdDAC = GetDAC(ContactIdField);
			var asMainDAC = GetDAC(AsMainField);
			var accounDAC = GetDAC(AccountIdField);
			if (contactIdDAC != asMainDAC || asMainDAC != accounDAC)
				throw new Exception(string.Format("Fields '{0}', '{1}' and '{2}' are defined in different DACs",
					contactIdDAC.Name, asMainDAC.Name, accounDAC));
			_itemType = contactIdDAC;

			var cache = _Graph.Caches[_itemType];
			_contactIdFieldOrdinal = cache.GetFieldOrdinal(ContactIdField.Name);
			_asMainFieldOrdinal = cache.GetFieldOrdinal(AsMainField.Name);
			_accountIdFieldOrdinal = cache.GetFieldOrdinal(AccountIdField.Name);
		}

		protected abstract Type AccountIdField { get; }

		protected abstract Type AsMainField { get; }

		protected abstract Type ContactIdField { get; }

		protected abstract Type IncorrectPersistableDAC { get; }

		private static Type GetDAC(Type type)
		{
			var res = type.DeclaringType;
			if (res == null)
				throw new Exception(string.Format("DAC for field '{0}' can not be found", type.Name));
			return res;
		}

		private void CreateView(PXGraph graph)
		{
			var command = new Select<Contact>();
			View = new PXView(graph, false, command, new PXSelectDelegate(SelectDelegate));
		}

		private void AttacheHandlers(PXGraph graph)
		{
			graph.RowInserted.AddHandler(_itemType, RowInsertedHandler);
			graph.RowUpdating.AddHandler(_itemType, RowUpdatingHandler);
			graph.RowUpdated.AddHandler(_itemType, RowUpdatedHandler);
			graph.RowSelected.AddHandler(_itemType, RowSelectedHandler);
			graph.RowDeleted.AddHandler(_itemType, RowDeletedHandler);
		}

		private void RowDeletedHandler(PXCache sender, PXRowDeletedEventArgs e)
		{
			var currentContactId = sender.GetValue(e.Row, _contactIdFieldOrdinal);
			var isMainContact = sender.GetValue(e.Row, AsMainField.Name) as bool?;
			if (isMainContact == true) return;

			var currentContact = currentContactId.
				With(_ => (Contact)PXSelect<Contact,
					Where<Contact.contactID, Equal<Required<Contact.contactID>>>>.
				Select(sender.Graph, _));
			if (currentContact != null)
			{
				var contactCache = sender.Graph.Caches[typeof(Contact)];
				contactCache.Delete(currentContact);
			}
		}

		private void RowSelectedHandler(PXCache sender, PXRowSelectedEventArgs e)
		{
			var contactCache = sender.Graph.Caches[typeof(Contact)];
			var asMain = false;

			var accountId = sender.GetValue(e.Row, _accountIdFieldOrdinal);

			var currentContactId = sender.GetValue(e.Row, _contactIdFieldOrdinal);
			var currentContact = currentContactId.
				With(_ => (Contact)PXSelect<Contact,
					Where<Contact.contactID, Equal<Required<Contact.contactID>>>>.
				Select(sender.Graph, _));
			if (currentContactId != null && currentContact != null)
			{
				var account = accountId.
					With(_ => (BAccount)PXSelect<BAccount,
						Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>.
					Select(_Graph, _));
				var accountContactId = account.With(_ => _.DefContactID);
				if (currentContactId.Equals(accountContactId))
				{
					asMain = true;
				}
			}

			sender.SetValue(e.Row, _asMainFieldOrdinal, asMain);
			if (!DoNotCorrectUI) PXUIFieldAttribute.SetEnabled(contactCache, currentContact, !asMain);
		}

		protected virtual IEnumerable SelectDelegate()
		{
			var primaryCache = _Graph.Caches[_itemType];
			var primaryRecord = GetPrimaryRow();
			var currentContactId = primaryCache.GetValue(primaryRecord, _contactIdFieldOrdinal);

			yield return (Contact)PXSelect<Contact,
				Where<Contact.contactID, Equal<Required<Contact.contactID>>>>.
				Select(_Graph, currentContactId);
		}

		protected Type ItemType
		{
			get { return _itemType; }
		}

		protected abstract object GetPrimaryRow();

		private void RowInsertedHandler(PXCache sender, PXRowInsertedEventArgs e)
		{
			var row = e.Row;
			var asMain = sender.GetValue(row, _asMainFieldOrdinal);
			var accountId = sender.GetValue(row, _accountIdFieldOrdinal);
			var account = accountId.With(_ => (BAccount)PXSelect<BAccount,
				Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>.
				Select(_Graph, _));
			var accountContactId = account.With(_ => _.DefContactID);
			var accountContact = accountContactId.
				With<int?, Contact>(_ => (Contact)PXSelect<Contact,
					Where<Contact.contactID, Equal<Required<Contact.contactID>>>>.
				Select(sender.Graph, _));
			var currentContactId = sender.GetValue(row, _contactIdFieldOrdinal);

			if (accountContact == null)
			{
				asMain = false;
				sender.SetValue(row, _asMainFieldOrdinal, false);
			}

			var contactCache = sender.Graph.Caches[typeof(Contact)];
			if (accountContact != null && true.Equals(asMain))
			{
				if (currentContactId != null && !object.Equals(currentContactId, accountContactId))
				{
					var currentContact = (Contact)PXSelect<Contact,
						Where<Contact.contactID, Equal<Required<Contact.contactID>>>>.
						Select(sender.Graph, currentContactId);
					var oldDirty = contactCache.IsDirty;
					contactCache.Delete(currentContact);
					contactCache.IsDirty = oldDirty;
				}
				sender.SetValue(row, _contactIdFieldOrdinal, accountContactId);
			}
			else
			{
				if (currentContactId == null || object.Equals(currentContactId, accountContactId))
				{
					var oldDirty = contactCache.IsDirty;
					Contact cnt;
					if (accountContact != null)
					{
						cnt = (Contact)contactCache.CreateCopy(accountContact);
					}
					else
					{
						cnt = (Contact)contactCache.CreateInstance();
					}
					cnt.ContactID = null;
					cnt.BAccountID = (int?)accountId;
					cnt = (Contact)contactCache.Insert(cnt);

					sender.SetValue(row, _contactIdFieldOrdinal, cnt.ContactID);
					contactCache.IsDirty = oldDirty;
				}
			}
		}

		private void RowUpdatingHandler(PXCache sender, PXRowUpdatingEventArgs e)
		{
			var row = e.NewRow;
			var oldRow = e.Row;

			var asMain = sender.GetValue(row, _asMainFieldOrdinal);
			var oldAsMain = sender.GetValue(oldRow, _asMainFieldOrdinal);

			var contactId = sender.GetValue(row, _contactIdFieldOrdinal);

			var accountId = sender.GetValue(row, _accountIdFieldOrdinal);
			var account = accountId.
				With(_ => (BAccount)PXSelect<BAccount,
					Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>.
				Select(_Graph, _));
			var accountContactId = account.With(_ => _.DefContactID);
			var accountContact = accountContactId.
				With<int?, Contact>(_ => (Contact)PXSelect<Contact,
					Where<Contact.contactID, Equal<Required<Contact.contactID>>>>.
				Select(sender.Graph, _));

			var oldAccountId = sender.GetValue(oldRow, _accountIdFieldOrdinal);
			if (!object.Equals(accountId, oldAccountId))
			{
				var oldContactId = sender.GetValue(row, _contactIdFieldOrdinal);
				var oldAccount = accountId.
					With(_ => (BAccount)PXSelect<BAccount,
						Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>.
					Select(_Graph, _));
				var oldAccountContactId = oldAccount.With(_ => _.DefContactID);
				var oldAccountContact = oldAccountContactId.
					With<int?, Contact>(_ => (Contact)PXSelect<Contact,
						Where<Contact.contactID, Equal<Required<Contact.contactID>>>>.
					Select(sender.Graph, _));
				oldAsMain = oldAccountContact != null && object.Equals(oldContactId, oldAccountContactId);
				if (true.Equals(oldAsMain))
				{
					asMain = true;
					contactId = accountContactId;
					sender.SetValue(row, _contactIdFieldOrdinal, accountContactId);
				} 
			}

			if (true.Equals(asMain))
			{
				
				if (accountContact == null)
				{
					asMain = false;
					sender.SetValue(row, _asMainFieldOrdinal, false);
				}
			}

			if (!object.Equals(asMain, oldAsMain))
			{
				if (true.Equals(asMain))
				{
					sender.SetValue(row, _contactIdFieldOrdinal, accountContactId);
				}
				else
				{
					if (object.Equals(accountContactId, contactId))
						sender.SetValue(row, _contactIdFieldOrdinal, null);
				}
			}
		}

		private void RowUpdatedHandler(PXCache sender, PXRowUpdatedEventArgs e)
		{
			var row = e.Row;
			var oldRow = e.OldRow;

			var accountId = sender.GetValue(row, _accountIdFieldOrdinal);
			var contactId = sender.GetValue(row, _contactIdFieldOrdinal);
			var oldContactId = sender.GetValue(oldRow, _contactIdFieldOrdinal);
			var contactCache = _Graph.Caches[typeof(Contact)];
			if (!object.Equals(contactId, oldContactId))
			{
				var account = accountId.
					With(_ => (BAccount)PXSelect<BAccount,
						Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>.
					Select(_Graph, _));
				var accountContactId = account.With(_ => _.DefContactID);
				var accountWithDefContact = oldContactId.
					With(_ => (BAccount)PXSelect<BAccount,
						Where<BAccount.defContactID, Equal<Required<BAccount.defContactID>>>>.
					Select(_Graph, _));
				if (accountWithDefContact == null)
				{
					var oldContact = oldContactId.
						With(_ => (Contact)PXSelect<Contact,
							Where<Contact.contactID, Equal<Required<Contact.contactID>>>>.
						Select(_Graph, _));
					if (oldContact != null)
					{
						var oldIsDirty = contactCache.IsDirty;
						contactCache.Delete(oldContact);
						contactCache.IsDirty = oldIsDirty;
					}
				}

				if (contactId == null)
				{
					var oldDirty = contactCache.IsDirty;
					Contact cnt;
					var accountContact = accountContactId.
						With<int?, Contact>(_ => (Contact)PXSelect<Contact,
							Where<Contact.contactID, Equal<Required<Contact.contactID>>>>.
						Select(_Graph, _));
					if (accountContact != null && object.Equals(accountContactId, oldContactId))
					{
						cnt = (Contact)contactCache.CreateCopy(accountContact);
					}
					else
					{
						cnt = (Contact)contactCache.CreateInstance();
					}
					if (cnt != null)
					{
						cnt.ContactID = null;
						cnt.BAccountID = (int?)accountId;
						cnt = (Contact)contactCache.Insert(cnt);

						sender.SetValue(row, _contactIdFieldOrdinal, cnt.ContactID);
						sender.SetValue(row, _asMainFieldOrdinal, false);
						contactCache.IsDirty = oldDirty;
						contactId = cnt.ContactID;
					}
				}
			}
			if (contactId == null)
			{
				var oldDirty = contactCache.IsDirty;
				var addr = (Contact)contactCache.CreateInstance();
				addr.ContactID = null;
				addr.BAccountID = (int?)accountId;
				addr = (Contact)contactCache.Insert(addr);

				sender.SetValue(row, _contactIdFieldOrdinal, addr.ContactID);
				sender.SetValue(row, _asMainFieldOrdinal, false);
				contactCache.IsDirty = oldDirty;
			}
		}
	}

	#endregion

	#region ContactSelect

	public sealed class ContactSelect<TContactIdField, TAsMainField, TAccountIdField> : ContactSelectBase
		where TContactIdField : IBqlField
		where TAsMainField : IBqlField
		where TAccountIdField : IBqlField
	{
		public ContactSelect(PXGraph graph)
			: base(graph)
		{
		}

		protected override Type AccountIdField
		{
			get { return typeof(TAccountIdField); }
		}

		protected override Type AsMainField
		{
			get { return typeof(TAsMainField); }
		}

		protected override Type ContactIdField
		{
			get { return typeof(TContactIdField); }
		}

		protected override Type IncorrectPersistableDAC
		{
			get { return typeof(TContactIdField); }
		}

		protected override object GetPrimaryRow()
		{
			return _Graph.Caches[ItemType].Current;
		}
	}

	#endregion

	#region ContactSelect2

	public sealed class ContactSelect2<TContactIdFieldSearch, TAsMainField, TAccountIdField> : ContactSelectBase
		where TContactIdFieldSearch : IBqlSearch
		where TAsMainField : IBqlField
		where TAccountIdField : IBqlField
	{
		private Type _addressIdField;
		private PXView _select;

		public ContactSelect2(PXGraph graph)
			: base(graph)
		{
		}

		private void Initialize()
		{
			if (_addressIdField != null) return;

			var search = BqlCommand.CreateInstance(typeof(TContactIdFieldSearch));
			_addressIdField = ((IBqlSearch)search).GetField();
			_select = new PXView(_Graph, false, GetSelectCommand(search));
		}

		private BqlCommand GetSelectCommand(BqlCommand search)
		{
			var arr = BqlCommand.Decompose(search.GetType());
			if (arr.Length < 2)
				throw new Exception("Unsupported search command detected");

			Type oldCommand = arr[0];
			Type newCommand = null;
			if (oldCommand == typeof(Search<,>))
				newCommand = typeof(Select<,>);
			if (oldCommand == typeof(Search2<,>))
				newCommand = typeof(Select2<,>);
			if (oldCommand == typeof(Search2<,,>))
				newCommand = typeof(Select2<,,>);

			if (newCommand == null)
				throw new Exception("Unsupported search command detected");

			arr[0] = newCommand;
			arr[1] = arr[1].DeclaringType;
			return BqlCommand.CreateInstance(arr);
		}

		protected override Type AccountIdField
		{
			get { return typeof(TAccountIdField); }
		}

		protected override Type AsMainField
		{
			get { return typeof(TAsMainField); }
		}

		protected override Type ContactIdField
		{
			get
			{
				Initialize();
				return _addressIdField;
			}
		}

		protected override Type IncorrectPersistableDAC
		{
			get { return typeof(TContactIdFieldSearch); }
		}

		protected override object GetPrimaryRow()
		{
			object record = _select.SelectSingle();
			if (record is PXResult)
				record = ((PXResult)record)[ItemType];
			return record;
		}

		protected override IEnumerable SelectDelegate()
		{
			PXCache primaryCache = _Graph.Caches[_itemType];
			object primaryRecord = GetPrimaryRow();
			object currentContactId = primaryCache.GetValue(primaryRecord, _contactIdFieldOrdinal);
			PXCache contactCache = _Graph.Caches<Contact>();
			foreach (Contact cnt in PXSelect<Contact, Where<Contact.contactID, Equal<Required<Contact.contactID>>>>.Select(_Graph, currentContactId))
			{
				Contact c = cnt;
				bool? asMain = (bool?)primaryCache.GetValue(primaryRecord, _asMainFieldOrdinal);
				if (asMain == true)
				{
					c = PXCache<Contact>.CreateCopy(c);
					PXUIFieldAttribute.SetEnabled(contactCache, c, false);
				}
				else
				{
					PXUIFieldAttribute.SetEnabled(contactCache, c, true);
				}
				yield return c;
			}
		}
	}

	#endregion

	#region PXOwnerFilteredSelect

	public class PXOwnerFilteredSelect<TFilter, TSelect, TGroupID, TOwnerID> : PXSelectBase
		where TFilter : OwnedFilter, new()
		where TSelect : IBqlSelect
		where TGroupID : IBqlField
		where TOwnerID : IBqlField
	{
		private BqlCommand _command;
		private Type _selectTarget;
		private Type _newRecordTarget;

		public PXOwnerFilteredSelect(PXGraph graph)
			: this(graph, false)
		{
			
		}

		protected PXOwnerFilteredSelect(PXGraph graph, bool readOnly)
			: base()
		{
			_Graph = graph;

			InitializeView(readOnly);
			InitializeSelectTarget();
			AppendActions();
			AppendEventHandlers();
		}

		public Type NewRecordTarget
		{
			get { return _newRecordTarget; }
			set
			{
				if (value != null)
				{
					if (!typeof(PXGraph).IsAssignableFrom(value))
						throw new ArgumentException(string.Format("{0} is excpected", typeof(PXGraph).GetLongName()), "value");
					if (value.GetConstructor(new Type[0]) == null)
						throw new ArgumentException("Default constructor is excpected", "value");
				}
				_newRecordTarget = value;
			}
		}

		private void AppendEventHandlers()
		{
			_Graph.RowSelected.AddHandler<TFilter>(RowSelectedHandler);
		}

		private void RowSelectedHandler(PXCache sender, PXRowSelectedEventArgs e)
		{
			var me = true.Equals(sender.GetValue(e.Row, typeof(OwnedFilter.myOwner).Name));
			var myGroup = true.Equals(sender.GetValue(e.Row, typeof(OwnedFilter.myWorkGroup).Name));

			PXUIFieldAttribute.SetEnabled(sender, e.Row, typeof(OwnedFilter.ownerID).Name, !me);
			PXUIFieldAttribute.SetEnabled(sender, e.Row, typeof(OwnedFilter.workGroupID).Name, !myGroup);
		}

		private void AppendActions()
		{
			_Graph.Initialized += sender =>
				{
					var name = _Graph.ViewNames[View] + "_AddNew";
					PXNamedAction.AddAction(_Graph, typeof(TFilter), name, Messages.AddNew, new PXButtonDelegate(AddNewHandler));
				};
		}

		[PXButton(Tooltip = "Add New Record", CommitChanges = true)]
		private IEnumerable AddNewHandler(PXAdapter adapter)
		{
			var filterCache = _Graph.Caches[typeof(TFilter)];
			var currentFilter = filterCache.Current;
			if (NewRecordTarget != null && _selectTarget != null && currentFilter != null)
			{
				var currentOwnerId = filterCache.GetValue(currentFilter, typeof (OwnedFilter.ownerID).Name);
				var currentWorkgroupId = filterCache.GetValue(currentFilter, typeof (OwnedFilter.workGroupID).Name);

				var targetGraph = (PXGraph)PXGraph.CreateInstance(NewRecordTarget);
				var targetCache = targetGraph.Caches[_selectTarget];
				var row = targetCache.Insert();
				var newRow = targetCache.CreateCopy(row);

				EPCompanyTreeMember member = PXSelect<EPCompanyTreeMember,
												Where<EPCompanyTreeMember.userID, Equal<Required<OwnedFilter.ownerID>>,
												  And<EPCompanyTreeMember.workGroupID, Equal<Required<OwnedFilter.workGroupID>>>>>.
				Select(targetGraph, currentOwnerId, currentWorkgroupId);
				if (member == null) currentOwnerId = null;
				
				targetCache.SetValue(newRow, typeof(TGroupID).Name, currentWorkgroupId);
				targetCache.SetValue(newRow, typeof(TOwnerID).Name, currentOwnerId);
				targetCache.Update(newRow);
				PXRedirectHelper.TryRedirect(targetGraph, PXRedirectHelper.WindowMode.NewWindow);
			}
			return adapter.Get();
		}

		private void InitializeSelectTarget()
		{
			var selectTabels = _command.GetTables();
			if (selectTabels == null || selectTabels.Length == 0)
				throw new Exception("Primary table of given select command cannot be found");

			_selectTarget = selectTabels[0];
			_Graph.EnsureCachePersistence(_selectTarget);
		}

		private void InitializeView(bool readOnly)
		{
			_command = CreateCommand();
			View = new PXView(_Graph, readOnly, _command, new PXSelectDelegate(Handler));
		}

		private IEnumerable Handler()
		{
			var filterCache = _Graph.Caches[typeof(TFilter)];
			var currentFilter = filterCache.Current;
			if (filterCache.Current == null) return new object[0];

			var parameters = GetParameters(filterCache, currentFilter);

			return _Graph.QuickSelect(_command, parameters);
		}

		private static object[] GetParameters(PXCache filterCache, object currentFilter)
		{
			var currentOwnerId = filterCache.GetValue(currentFilter, typeof(OwnedFilter.ownerID).Name);
			var currentWorkgroupId = filterCache.GetValue(currentFilter, typeof(OwnedFilter.workGroupID).Name);
			var currentMyWorkgroup = filterCache.GetValue(currentFilter, typeof(OwnedFilter.myWorkGroup).Name);
			var parameters = new object[]
				{
					currentOwnerId, currentOwnerId, 
					currentMyWorkgroup, currentMyWorkgroup, 
					currentWorkgroupId, currentWorkgroupId, currentMyWorkgroup
				};
			return parameters;
		}

		private static BqlCommand CreateCommand()
		{
			var command = BqlCommand.CreateInstance(typeof(TSelect));
			var additionalCondition = BqlCommand.Compose(
				typeof(Where2<Where<Required<OwnedFilter.ownerID>, IsNull,
								Or<Required<OwnedFilter.ownerID>, Equal<TOwnerID>>>,
							And<
								Where2<
									Where<Required<OwnedFilter.myWorkGroup>, IsNull,
										Or<Required<OwnedFilter.myWorkGroup>, Equal<False>>>,
									And2<
										Where<Required<OwnedFilter.workGroupID>, IsNull, 
											Or<TGroupID, Equal<Required<OwnedFilter.workGroupID>>>>,
										Or<Required<OwnedFilter.myWorkGroup>, Equal<True>,
									And<TGroupID, InMember<Current<AccessInfo.userID>>>>>>>>));
			return command.WhereAnd(additionalCondition);
		}
	}

	#endregion

	#region PXOwnerFilteredSelectReadonly

	public class PXOwnerFilteredSelectReadonly<TFilter, TSelect, TGroupID, TOwnerID> 
		: PXOwnerFilteredSelect<TFilter, TSelect, TGroupID, TOwnerID>
		where TFilter : OwnedFilter, new()
		where TSelect : IBqlSelect
		where TGroupID : IBqlField
		where TOwnerID : IBqlField
	{
		public PXOwnerFilteredSelectReadonly(PXGraph graph)
			:base(graph, true)
		{
		}
	}

	#endregion

	#region CRLastNameDefaultAttribute

	internal sealed class CRLastNameDefaultAttribute : PXEventSubscriberAttribute, IPXRowPersistingSubscriber
	{
		public void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			var contactType = sender.GetValue(e.Row, typeof(Contact.contactType).Name);
			var val = sender.GetValue(e.Row, _FieldOrdinal) as string;
			if (contactType != null && (contactType.Equals(ContactTypesAttribute.Lead) || contactType.Equals(ContactTypesAttribute.Person)) && string.IsNullOrWhiteSpace(val))
			{
				if (sender.RaiseExceptionHandling(_FieldName, e.Row, null, new PXSetPropertyKeepPreviousException(PXMessages.LocalizeFormat(ErrorMessages.FieldIsEmpty, _FieldName))))
				{
					throw new PXRowPersistingException(_FieldName, null, ErrorMessages.FieldIsEmpty, _FieldName);
				}
			}
		}
	}

	#endregion

	#region CRContactBAccountDefaultAttribute

	internal sealed class CRContactBAccountDefaultAttribute :  PXEventSubscriberAttribute, IPXRowInsertingSubscriber, IPXRowUpdatingSubscriber, IPXRowPersistingSubscriber, IPXRowPersistedSubscriber
	{
		private Dictionary<object, object> _persistedItems;

		public override void CacheAttached(PXCache sender)
		{
			_persistedItems = new Dictionary<object, object>();
			sender.Graph.RowPersisting.AddHandler(typeof(BAccount), SourceRowPersisting);
		}

		public void RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			SetDefaultValue(sender, e.Row);
		}

		public void RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
		{
			SetDefaultValue(sender, e.Row);
		}

		private void SetDefaultValue(PXCache sender, object row)
		{
			if (IsLeadOrPerson(sender, row)) return;

			var val = sender.GetValue(row, _FieldOrdinal);
			if (val != null) return;

			PXCache cache = sender.Graph.Caches[typeof (BAccount)];
			if (cache.Current != null)
			{
				var newValue = cache.GetValue(cache.Current, typeof (BAccount.bAccountID).Name);
				sender.SetValue(row, _FieldOrdinal, newValue);
			}
		}

		public void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if (IsLeadOrPerson(sender, e.Row)) return;

			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Insert && true ||
				(e.Operation & PXDBOperation.Command) == PXDBOperation.Update && true)
			{
				object key = sender.GetValue(e.Row, _FieldOrdinal);
				if (key != null)
				{
					object parent;
					if (_persistedItems.TryGetValue(key, out parent))
					{
						key = sender.Graph.Caches[typeof(BAccount)].GetValue(parent, typeof(BAccount.bAccountID).Name);
						sender.SetValue(e.Row, _FieldOrdinal, key);
						if (key != null)
						{
							_persistedItems[key] = parent;
						}
					}
				}
			}
			if (((e.Operation & PXDBOperation.Command) == PXDBOperation.Insert && true ||
				 (e.Operation & PXDBOperation.Command) == PXDBOperation.Update && true) &&
				sender.GetValue(e.Row, _FieldOrdinal) == null)
			{
				throw new PXRowPersistingException(_FieldName, null, String.Format("'{0}' may not be empty.", _FieldName));
			}
		}

		public void RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
		{
			if (IsLeadOrPerson(sender, e.Row)) return;

			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Insert && e.TranStatus == PXTranStatus.Aborted)
			{
				object key = sender.GetValue(e.Row, _FieldOrdinal);
				if (key != null)
				{
					object parent;
					if (_persistedItems.TryGetValue(key, out parent))
					{
						var sourceField = typeof(BAccount.bAccountID).Name;
						sender.SetValue(e.Row, _FieldOrdinal, sender.Graph.Caches[typeof(BAccount)].GetValue(parent, sourceField));
					}
				}
			}
		}

		private void SourceRowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if (IsLeadOrPerson(sender, e.Row)) return;

			var sourceField = typeof(BAccount.bAccountID).Name;
			object key = sender.GetValue(e.Row, sourceField);
			if (key != null)
				_persistedItems[key] = e.Row;
		}

		private bool IsLeadOrPerson(PXCache sender, object row)
		{
			var contactType = sender.GetValue(row, typeof(Contact.contactType).Name);
			return contactType != null &&
				(contactType.Equals(ContactTypesAttribute.Lead) ||
					contactType.Equals(ContactTypesAttribute.Person));
		}
	}

	#endregion

	#region BAccountType Attribute
	public class BAccountType
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { VendorType, CustomerType, CombinedType, EmployeeType, EmpCombinedType, ProspectType, CompanyType },
				new string[] { Messages.VendorType, Messages.CustomerType, Messages.CombinedType, Messages.EmployeeType, Messages.EmpCombinedType, Messages.ProspectType, Messages.CompanyType, }) { ; }
		}

		public class SalesPersonTypeListAttribute : PXStringListAttribute
		{
			public SalesPersonTypeListAttribute()
				: base(
				new string[] { VendorType, EmployeeType },
				new string[] { Messages.VendorType, Messages.EmployeeType }) { ; }
		}

		public const string VendorType = "VE";
		public const string CustomerType = "CU";
		public const string CombinedType = "VC";
		public const string EmployeeType = "EP";
		public const string EmpCombinedType = "EC";
		public const string ProspectType = "PR";
		public const string CompanyType = "CP";

		public class vendorType : Constant<string>
		{
			public vendorType() : base(VendorType) { ;}
		}
		public class customerType : Constant<string>
		{
			public customerType() : base(CustomerType) { ;}
		}
		public class combinedType : Constant<string>
		{
			public combinedType() : base(CombinedType) { ;}
		}
		public class employeeType : Constant<string>
		{
			public employeeType() : base(EmployeeType) { ;}
		}
		public class empCombinedType : Constant<string>
		{
			public empCombinedType() : base(EmpCombinedType) { ;}
		}
		public class prospectType : Constant<string>
		{
			public prospectType() : base(ProspectType) { ;}
		}
		public class companyType : Constant<string>
		{
			public companyType() : base(CompanyType) { ;}
		}
	}
	#endregion

	#region IActivityMaint

	public interface IActivityMaint
	{
		void CancelRow(CRActivity row);
		void CompleteRow(CRActivity row);
	}

	#endregion

	#region SelectContactEmailSync

	public class SelectContactEmailSync<TWhere> : PXSelectBase<Contact>
		where TWhere : IBqlWhere, new()
	{
		public SelectContactEmailSync(PXGraph graph, Delegate handler)
			: this(graph)
		{
			View = new PXView(_Graph, false, View.BqlSelect, handler);
		}

		public SelectContactEmailSync(PXGraph graph)
		{
			_Graph = graph;
			View = new PXView(_Graph, false, new Select<Contact>());
			View.WhereAnd<TWhere>();

			_Graph.FieldUpdated.AddHandler(typeof(Contact), typeof(Contact.eMail).Name, FieldUpdated<Contact.eMail, Users.email>);
			_Graph.FieldUpdated.AddHandler(typeof(Contact), typeof(Contact.firstName).Name, FieldUpdated<Contact.firstName, Users.firstName>);
			_Graph.FieldUpdated.AddHandler(typeof(Contact), typeof(Contact.lastName).Name, FieldUpdated<Contact.lastName, Users.lastName>);
		}

		protected virtual void FieldUpdated<TSrcField, TDstField>(PXCache sender, PXFieldUpdatedEventArgs e)
			where TSrcField : IBqlField
			where TDstField : IBqlField
		{
			Contact row = (Contact)e.Row;
			Users user = PXSelect<Users, Where<Users.pKID, Equal<Current<Contact.userID>>>>.SelectSingleBound(_Graph, new object[] { row });
			if (user != null)
			{
				PXCache usercache = _Graph.Caches[typeof(Users)];
				usercache.SetValue<TDstField>(user, sender.GetValue<TSrcField>(row));
				usercache.Update(user);
			}
		}
	}

	#endregion

	#region CRDuplicateContactList
	public class CRDuplicateContactList : PXSelectBase<CRDuplicateRecord>
	{
		#region MergeParams
		[Serializable]
		[PXHidden]
		public class MergeParams : IBqlTable
		{
			#region SourceContactID
			public abstract class sourceContactID : IBqlField { }

			[PXDBInt]
			[PXDefault(typeof(Contact.contactID))]
			public virtual int? SourceContactID { get; set; }
			#endregion

			#region ContactID
			public abstract class contactID : IBqlField { }

			[PXDBInt]
			[PXUIField(DisplayName = "Target", Required = true)]
			[PXDefault(typeof(Contact.contactID))]
			[CRDuplicateContactsSelector(typeof(MergeParams.sourceContactID))]
			public virtual int? ContactID { get; set; }
			#endregion
		}
		#endregion

		public class MergeLead : CRBaseUpdateProcess<MergeLead, Contact, PXMassMergableFieldAttribute, Contact.classID> { }
		public class MergeAddress : CRBaseUpdateProcess<MergeAddress, Address, PXMassMergableFieldAttribute, Contact.classID> { }

		private const string MergeActionName = "merge";
		private const string AttachActionName = "attachToAccount";
		private const string MergeParamsViewName = "mergeParams";
		private const string FieldsViewName = "ValueConflicts";

		private readonly PXVirtualTableView<MergeParams> mergeParam;

		public CRDuplicateContactList(PXGraph graph)
		{
			_Graph = graph;
			View = new PXView(_Graph, false,
				new Select2<CRDuplicateRecord,
				LeftJoin<Contact, On<Contact.contactID, Equal<CRDuplicateRecord.contactID>>,
				LeftJoin<CRLeadContactValidationProcess.Contact2,
							On<CRLeadContactValidationProcess.Contact2.contactID, Equal<CRDuplicateRecord.duplicateContactID>>,
				LeftJoin<BAccountR, On<BAccountR.bAccountID, Equal<CRLeadContactValidationProcess.Contact2.bAccountID>>>>>,
			 Where<CRDuplicateRecord.contactID, Equal<Current<Contact.contactID>>,
				 And<CRDuplicateRecord.duplicateContactID, NotEqual<CRDuplicateRecord.contactID>, 
				 And<CRDuplicateRecord.validationType, Equal<Switch<Case<Where<Contact.contactType, Equal<ContactTypesAttribute.bAccountProperty>,
																																				And<CRLeadContactValidationProcess.Contact2.contactType, Equal<ContactTypesAttribute.bAccountProperty>>>, ValidationTypesAttribute.account,
																																 Case<Where<Contact.contactType, Equal<ContactTypesAttribute.bAccountProperty>,
																																				Or<CRLeadContactValidationProcess.Contact2.contactType, Equal<ContactTypesAttribute.bAccountProperty>>>, ValidationTypesAttribute.leadAccount>>,
																																 ValidationTypesAttribute.leadContact>>,
				 And<CRDuplicateRecord.score, GreaterEqual<
							Switch<Case<Where<CRLeadContactValidationProcess.Contact2.contactType, Equal<ContactTypesAttribute.bAccountProperty>>,
													Current<CRSetup.leadToAccountValidationThreshold>>, Current<CRSetup.leadValidationThreshold>>>,
				 And<Current<Contact.duplicateFound>, Equal<True>,
				 And<CRLeadContactValidationProcess.Contact2.duplicateStatus, NotEqual<DuplicateStatusAttribute.duplicated>,
				 And2<Where<CRLeadContactValidationProcess.Contact2.contactType, NotEqual<ContactTypesAttribute.bAccountProperty>,
								Or<CRLeadContactValidationProcess.Contact2.contactID, Equal<BAccountR.defContactID>>>,
				 And<Where<CRLeadContactValidationProcess.Contact2.contactType, NotEqual<ContactTypesAttribute.bAccountProperty>,								
				 				Or<Contact.bAccountID, IsNull,
 								Or<CRLeadContactValidationProcess.Contact2.bAccountID, IsNull, 
								Or<Contact.bAccountID, NotEqual<CRLeadContactValidationProcess.Contact2.bAccountID>>>>>>>>>>>>>>());

			_Graph.Views.Add(MergeParamsViewName, mergeParam = new PXVirtualTableView<MergeParams>(_Graph));
			_Graph.Views.Add(FieldsViewName, new PXView(_Graph, false, 
				new Select<FieldValue, Where<FieldValue.attributeID, IsNull>, OrderBy<Asc<FieldValue.order>>>(), 
				(PXSelectDelegate)valueConflicts));

			PXDBAttributeAttribute.Activate(_Graph.Caches[typeof(Contact)]);
			//Init PXVirtual Static constructor
			typeof(FieldValue).GetCustomAttributes(typeof(PXVirtualAttribute), false);

			_Graph.FieldSelecting.AddHandler<FieldValue.value>(delegate(PXCache sender, PXFieldSelectingEventArgs e)
				{
					if (e.Row == null) return;
					e.ReturnState = InitValueFieldState(e.Row as FieldValue);
				});
			_Graph.RowSelected.AddHandler<Contact>(delegate(PXCache sender, PXRowSelectedEventArgs e)
				{
					sender.Graph.Actions[MergeActionName].SetEnabled(e.Row != null && sender.GetStatus(e.Row) != PXEntryStatus.Inserted);
				});

			_Graph.FieldDefaulting.AddHandler<MergeParams.contactID>(MergeParams_ContactID_FieldDefaulting);			
			_Graph.RowUpdated.AddHandler<Contact>(Contact_RowUpdated);

			PXNamedAction.AddAction(_Graph, _Graph.Views[_Graph.PrimaryView].Cache.GetItemType(), MergeActionName, Messages.Merge, Merge);
			PXNamedAction.AddAction(_Graph, _Graph.Views[_Graph.PrimaryView].Cache.GetItemType(), AttachActionName, Messages.AttachToAccount, Attach);

			PXUIFieldAttribute.SetDisplayName<BAccountR.type>(_Graph.Caches<BAccountR>(), Messages.BAccountType);

			_Graph.EnsureCachePersistence(typeof(CRActivityStatistics));
		}
		
		protected virtual void MergeParams_ContactID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			Contact current = (Contact) _Graph.Caches<Contact>().Current;
			List<Contact> contacts = PXSelectorAttribute.SelectAll<MergeParams.contactID>(sender, e.Row)
				.RowCast<Contact>()
				.Where(c => c.ContactType == ContactTypesAttribute.Person)
				.ToList();
			e.NewValue = current.ContactType == ContactTypesAttribute.Person && current.IsActive == true || contacts.Count == 0 ? current.ContactID : contacts[0].ContactID;
		}
		
		protected virtual void Contact_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			Contact row = e.Row as Contact;
			if (row == null) return;
			if (!sender.ObjectsEqual<Contact.duplicateStatus>(e.Row, e.OldRow)
				&& sender.Graph.Caches[typeof(CRSetup)].Current != null
				&& ((CRSetup)sender.Graph.Caches[typeof(CRSetup)].Current).ContactEmailUnique == true
				&& (row.ContactType == ContactTypesAttribute.Lead || row.ContactType == ContactTypesAttribute.Person)				 
				&& row.Status != LeadStatusesAttribute.Closed
				&& row.EMail != null
				&& (row.DuplicateStatus == DuplicateStatusAttribute.Validated || row.DuplicateStatus == DuplicateStatusAttribute.PossibleDuplicated))
			{
				Contact duplicate = PXSelect<Contact,
					Where2<Where<Contact.contactType, Equal<ContactTypesAttribute.lead>,
									Or<Contact.contactType, Equal<ContactTypesAttribute.person>>>,
							And<Where<Contact.duplicateStatus, Equal<DuplicateStatusAttribute.validated>,
								And<Contact.eMail, Equal<Current<Contact.eMail>>,
								And<Contact.contactID, NotEqual<Current<Contact.contactID>>>>>>>>.SelectSingleBound(sender.Graph, new object[] { e.Row });
				if (duplicate != null)
				{
					row.DuplicateFound = true;
					row.DuplicateStatus = DuplicateStatusAttribute.PossibleDuplicated;
					sender.RaiseExceptionHandling<Contact.eMail>(row, row.EMail, new PXSetPropertyException(Messages.ContactEmailNotUnique, PXErrorLevel.Warning));
				}
			}
		}
		public IEnumerable valueConflicts()
		{
			return _Graph.Caches[typeof(FieldValue)].Cached.Cast<FieldValue>().Where(fld => fld.Hidden != true);
		}

		public static Tuple<IEnumerable<string>, IEnumerable<string>> GetPossibleValues<T>(PXGraph graph, IEnumerable<T> collection, string propName)
			where T : IBqlTable
		{
			PXCache cache = graph.Caches[typeof(T)];
			return collection.Cast<object>()
							.Select(o => cache.GetStateExt(o, propName))
							.Cast<PXFieldState>()
							.Select(st =>
							{
								string value = null;
								string label = string.Empty;
								if (st != null)
								{
									var stringState = st as PXStringState;
									value = st.Value != null ? st.Value.ToString() : null;

									if (stringState != null && stringState.AllowedValues != null)
									{
										int i = Array.IndexOf(stringState.AllowedValues, value);
										label = (i == -1) ? value : stringState.AllowedLabels[i];
									}
								}
								return new[] { value != null ? new Tuple<string, string>(value, label) : new Tuple<string, string>(null, string.Empty) };
							})
							.SelectMany(z => z.Select(entry => entry))
							.GroupBy(z => z.Item1)
							.Select(g => new Tuple<string, string>(g.Key, g.First(z => z.Item1 == g.Key).Item2))
							.OrderBy(pair => pair.Item1)
							.UnZip();
		}


		protected PXFieldState InitValueFieldState(FieldValue field)
		{
			Tuple<IEnumerable<string>, IEnumerable<string>> possibleValues = new Tuple<IEnumerable<string>, IEnumerable<string>>(new string[0], new string[0]);
			List<Contact> contacts = new List<Contact>(PXSelectorAttribute.SelectAll<MergeParams.contactID>(_Graph.Caches[typeof(MergeParams)], _Graph.Caches[typeof(MergeParams)].Current).RowCast<Contact>());
			if (field.CacheName == typeof(Contact).FullName)
			{
				possibleValues = GetPossibleValues(_Graph, contacts, field.Name);
			}
			else if (field.CacheName == typeof(Address).FullName)
			{
				PXSelectBase<Address> cmd = new PXSelect<Address>(_Graph);
				List<int?> addressIDs = new List<int?>(contacts.Where(c => c.DefAddressID != null).Select(c => c.DefAddressID));
				foreach (int? a in addressIDs)
				{
					cmd.WhereOr<Where<Address.addressID, Equal<Required<Contact.defAddressID>>>>();
				}
				possibleValues = GetPossibleValues(_Graph, cmd.Select(addressIDs.Cast<object>().ToArray()).RowCast<Address>(), field.Name);
			}

			string[] values = possibleValues.Item1.ToArray();
			string[] labels = possibleValues.Item2.ToArray();

			return PXStringState.CreateInstance(field.Value, null, null, typeof(FieldValue.value).Name,
				false, 0, null, values, labels, null, null);
		}

		protected void InsertPropertyValue(FieldValue field, Dictionary<Type, object> targets)
		{
			Type t = Type.GetType(field.CacheName);
			PXCache cache = _Graph.Caches[t];
			object target = targets[t];

			PXStringState state = InitValueFieldState(field) as PXStringState;

			if (state != null)
			{
				if (state.AllowedValues == null || !state.AllowedValues.Any() || state.AllowedValues.Count() == 1 && field.AttributeID == null)
					return;
				if (state.AllowedValues.Count() == 1)
				{
					field.Hidden = true;
					field.Value = state.AllowedValues[0];
				}
				else if (target != null)
				{
					state.Required = true;
					object value = cache.GetValueExt(target, field.Name);
					if (value is PXFieldState) value = ((PXFieldState) value).Value;
					field.Value = value != null ? value.ToString() : null;
				}
			}
			_Graph.Caches[typeof(FieldValue)].Insert(field);
		}

		protected void FillPropertyValue()
		{
			PXCache cache = _Graph.Caches[typeof(FieldValue)];
			cache.Clear();

			PXCache<MergeParams> pcache = _Graph.Caches<MergeParams>();
			pcache.SetDefaultExt<MergeParams.contactID>(pcache.Current);

			int order = 0;
			List<FieldValue> fields = new List<FieldValue>(MergeLead.GetProcessingProperties(_Graph, ref order));
			HashSet<string> fieldNames = new HashSet<string>(fields.Select(f => f.Name));
			fields.AddRange(MergeAddress.GetMarkedProperties(_Graph, ref order).Where(fld => fieldNames.Add(fld.Name)));

			Contact contact = PXSelect<Contact, Where<Contact.contactID, Equal<Required<Contact.contactID>>>>.Select(_Graph, ((MergeParams)pcache.Current).ContactID);
			Address address = PXSelect<Address, Where<Address.addressID, Equal<Required<Address.addressID>>>>.Select(_Graph, contact.DefAddressID);
			Dictionary<Type, object> targets = new Dictionary<Type, object>
			{
				{typeof (Contact), contact},
				{typeof (Address), address}
			};
			foreach (FieldValue fld in fields)
			{
				InsertPropertyValue(fld, targets);
			}
			cache.IsDirty = false;
		}

		[PXUIField(DisplayName = Messages.Merge)]
		[PXButton]
		public virtual IEnumerable Merge(PXAdapter adapter)
		{

			PXCache<MergeParams> mpcache = _Graph.Caches<MergeParams>();
			List<Contact> contacts = null;
			WebDialogResult res = AskExt((graph, name) =>
			{
				mpcache.Clear();
				mpcache.Insert();
				contacts = new List<Contact>(PXSelectorAttribute.SelectAll<MergeParams.contactID>(_Graph.Caches[typeof(MergeParams)], _Graph.Caches[typeof(MergeParams)].Current)
				.RowCast<Contact>()
				.Select(c => _Graph.Caches[typeof(Contact)].CreateCopy(c))
				.Cast<Contact>());

				if (contacts.Count < 2) throw new PXException(Messages.DuplicatesNotSelected);

				FillPropertyValue();
			});
			if (res != WebDialogResult.OK 
				|| !mergeParam.VerifyRequired()) return adapter.Get();

			if (contacts == null)
			{
				contacts = new List<Contact>(PXSelectorAttribute.SelectAll<MergeParams.contactID>(_Graph.Caches[typeof(MergeParams)], _Graph.Caches[typeof(MergeParams)].Current)
					.RowCast<Contact>()
					.Select(c => _Graph.Caches[typeof(Contact)].CreateCopy(c))
					.Cast<Contact>());

			}

			int? targetID = ((MergeParams)_Graph.Caches[typeof(MergeParams)].Current).ContactID;
			List<FieldValue> values = new List<FieldValue>(_Graph.Caches[typeof(FieldValue)].Cached.Cast<FieldValue>()
				.Select(v => _Graph.Caches[typeof(FieldValue)].CreateCopy(v))
				.Cast<FieldValue>());

			_Graph.Actions.PressSave();
			PXLongOperation.StartOperation(this._Graph,
				()=>MergeContacts(
				                               (int) targetID,
				                               contacts,
				                               values
				                               )
				);

			return adapter.Get();
		}
		[PXUIField(DisplayName = Messages.AttachToAccount)]
		[PXButton]
		public virtual IEnumerable Attach(PXAdapter adapter)
		{
			Contact duplicate = PXSelect<Contact, 
				Where<Contact.contactID, Equal<Current<CRDuplicateRecord.duplicateContactID>>>>
				.SelectSingleBound(this._Graph, new object[] { this.View.Cache.Current });
			if(duplicate == null || duplicate.ContactType != ContactTypesAttribute.BAccountProperty)
				throw new PXException(Messages.AttachToAccountNotFound);

			PXCache cache = this._Graph.Caches[typeof (Contact)];
			foreach (var item in adapter.Get())
			{
				Contact contact = PXResult.Unwrap<Contact>(item);
				if (contact != null)
				{
					Contact upd = (Contact)cache.CreateCopy(contact);
					upd.BAccountID = duplicate.BAccountID;
					upd = (Contact)cache.Update(upd);
					if(upd != null)
						cache.RestoreCopy(contact, upd);
				}
				yield return item;
			}
		}
		public static void MergeContacts(int targetID, List<Contact> contacts, List<FieldValue> values)
		{
			PXGraph graph = new PXGraph();
			PXPrimaryGraphCollection primaryGraph = new PXPrimaryGraphCollection(graph);
			Contact target = PXSelect<Contact, Where<Contact.contactID, Equal<Required<Contact.contactID>>>>.Select(graph, targetID);

			graph = primaryGraph[target];
			PXCache cache = graph.Caches[typeof(Contact)];

		    var refNoteIdField = EntityHelper.GetNoteField(cache.GetItemType());

            target = PXCache<Contact>.CreateCopy(target);

			Address targetAddress = PXSelect<Address, Where<Address.addressID, Equal<Required<Address.addressID>>>>.Select(graph, target.DefAddressID);

			Dictionary<Type, object> targets = new Dictionary<Type, object> { { typeof(Contact), target }, { typeof(Address), targetAddress } };


			foreach (FieldValue fld in values)
			{
				if (fld.AttributeID == null)
				{
					Type type = Type.GetType(fld.CacheName);
					PXFieldState state = (PXFieldState)graph.Caches[type].GetStateExt(targets[type], fld.Name);
					if (state == null || !Equals(state.Value, fld.Value))
					{
						graph.Caches[type].SetValueExt(targets[type], fld.Name, fld.Value);
						targets[type] = graph.Caches[type].CreateCopy(graph.Caches[type].Update(targets[type]));
					}
				}
				else
				{
					PXCache attrCache = cache.Graph.Caches[typeof(CSAnswers)];
					CSAnswers attr = new CSAnswers
					{
						AttributeID = fld.AttributeID,
                        RefNoteID = cache.GetValue(target, refNoteIdField) as Guid?,
                        Value = fld.Value,
					};
					attrCache.Update(attr);
				}
			}

			bool needConvert = false;
			PXRedirectRequiredException redirect = null;
			using (PXTransactionScope scope = new PXTransactionScope())
			{
				foreach (Contact contact in contacts.Where(c => c.ContactID != targetID))
				{
					if (contact.ContactType != ContactTypesAttribute.Lead && target.ContactType == ContactTypesAttribute.Lead)
						needConvert = true;

					PXCache Activities = graph.Caches[typeof(CRPMTimeActivity)];
					BAccount baccount = PXSelect<BAccount, Where<BAccount.bAccountID, Equal<Required<Contact.bAccountID>>>>.Select(graph, target.BAccountID);
					foreach (CRPMTimeActivity activity in PXSelect<CRPMTimeActivity, Where<CRPMTimeActivity.contactID, Equal<Current<Contact.contactID>>>>
						.SelectMultiBound(graph, new object[] { contact })
						.RowCast<CRPMTimeActivity>()
						.Select(cas => (CRPMTimeActivity)Activities.CreateCopy(cas)))
					{
						activity.ContactID = target.ContactID;
						activity.BAccountID = baccount.With(a => a.BAccountID);
						Activities.Update(activity);
					}

					PXCache Cases = graph.Caches[typeof(CRCase)];
					foreach (CRCase cas in PXSelect<CRCase,
						Where<CRCase.contactID, Equal<Current<Contact.contactID>>>>.SelectMultiBound(graph, new object[] {contact})
						                                                           .RowCast<CRCase>()
						                                                           .Select(cas => (CRCase) Cases.CreateCopy(cas)))
					{
						if (target.BAccountID != cas.CustomerID)
						{
							throw new PXException(Messages.ContactBAccountCase, contact.DisplayName, cas.CaseCD);
						}
						cas.ContactID = target.ContactID;
						Cases.Update(cas);
					}

					PXCache Opportunities = graph.Caches[typeof (CROpportunity)];
					foreach (CROpportunity opp in PXSelect<CROpportunity,
						Where<CROpportunity.contactID, Equal<Current<Contact.contactID>>>>.SelectMultiBound(graph, new object[] {contact})
						                                                                  .RowCast<CROpportunity>()
						                                                                  .Select(opp => (CROpportunity) Opportunities.CreateCopy(opp)))
					{
						if (target.BAccountID != opp.BAccountID)
						{
							throw new PXException(Messages.ContactBAccountForOpp, contact.DisplayName, contact.ContactID, opp.OpportunityID);
						}
						opp.ContactID = target.ContactID;
						Opportunities.Update(opp);
					}

					PXCache Relations = graph.Caches[typeof (CRRelation)];
					foreach (CRRelation rel in PXSelectJoin<CRRelation,
						LeftJoin<CRRelation2, On<CRRelation.entityID, Equal<CRRelation2.entityID>,
							And<CRRelation.role, Equal<CRRelation2.role>,
								And<CRRelation2.refNoteID, Equal<Required<Contact.noteID>>>>>>,
						Where<CRRelation2.entityID, IsNull,
							And<CRRelation.refNoteID, Equal<Required<Contact.noteID>>>>>.Select(graph, target.NoteID, contact.NoteID)
							                                                            .RowCast<CRRelation>()
							                                                            .Select(rel => (CRRelation) Relations.CreateCopy(rel)))
					{
						rel.RelationID = null;
						rel.RefNoteID = target.NoteID;
						Relations.Insert(rel);
					}

					PXCache Subscriptions = graph.Caches[typeof (CRMarketingListMember)];
					foreach (CRMarketingListMember mmember in PXSelectJoin<CRMarketingListMember,
						LeftJoin<CRMarketingListMember2, On<CRMarketingListMember.marketingListID, Equal<CRMarketingListMember2.marketingListID>,
							And<CRMarketingListMember2.contactID, Equal<Required<Contact.contactID>>>>>,
						Where<CRMarketingListMember.contactID, Equal<Required<Contact.contactID>>,
							And<CRMarketingListMember2.marketingListID, IsNull>>>.Select(graph, target.ContactID, contact.ContactID)
							                                                     .RowCast<CRMarketingListMember>()
							                                                     .Select(mmember => (CRMarketingListMember) Subscriptions.CreateCopy(mmember)))
					{
						mmember.ContactID = target.ContactID;
						Subscriptions.Insert(mmember);
					}

					PXCache Members = graph.Caches[typeof (CRCampaignMembers)];
					foreach (CRCampaignMembers cmember in PXSelectJoin<CRCampaignMembers,
						LeftJoin<CRCampaignMembers2, On<CRCampaignMembers.campaignID, Equal<CRCampaignMembers2.campaignID>,
							And<CRCampaignMembers2.contactID, Equal<Required<Contact.contactID>>>>>,
						Where<CRCampaignMembers2.campaignID, IsNull,
							And<CRCampaignMembers.contactID, Equal<Required<Contact.contactID>>>>>.Select(graph, target.ContactID, contact.ContactID)
							                                                                      .RowCast<CRCampaignMembers>()
							                                                                      .Select(cmember => (CRCampaignMembers) Members.CreateCopy(cmember)))
					{
						cmember.ContactID = target.ContactID;
						Members.Insert(cmember);
					}

					PXCache NWatchers = graph.Caches[typeof (ContactNotification)];
					foreach (ContactNotification watcher in PXSelectJoin<ContactNotification,
						LeftJoin<ContactNotification2, On<ContactNotification.setupID, Equal<ContactNotification2.setupID>,
							And<ContactNotification2.contactID, Equal<Required<Contact.contactID>>>>>,
						Where<ContactNotification2.setupID, IsNull,
							And<ContactNotification.contactID, Equal<Required<Contact.contactID>>>>>.Select(graph, target.ContactID, contact.ContactID)
							                                                                        .RowCast<ContactNotification>()
							                                                                        .Select(watcher => (ContactNotification) NWatchers.CreateCopy(watcher)))
					{
						watcher.NotificationID = null;
						watcher.ContactID = target.ContactID;
						NWatchers.Insert(watcher);
					}

					if (contact.UserID != null)
					{
						Users user = PXSelect<Users, Where<Users.pKID, Equal<Required<Contact.userID>>>>.Select(graph, contact.UserID);
						if (user != null)
						{
							graph.EnsureCachePersistence(typeof(Users));
							user.IsApproved = false;
							graph.Caches[typeof (Users)].Update(user);
						}
					}

					graph.Actions.PressSave();					

					PXGraph operGraph = primaryGraph[contact];					
					RunAction(operGraph, contact, "Close as Duplicate");
					operGraph.Actions.PressSave();
				}
				target.DuplicateFound = false;
				RunAction(graph, target, "Mark As Validated");
				graph.Actions.PressSave();
				if (needConvert)
					try
					{										
						RunAction(graph, target, "ConvertToContact");
					}
					catch (PXRedirectRequiredException r)
					{
						redirect = r;
					}				
				scope.Complete();
			}			
			throw redirect ?? new PXRedirectRequiredException(graph, Messages.Contact);	
		}

		private static void RunAction(PXGraph graph, Contact contact, string menu)
		{
				int startRow = 0;
				int totalRows = 1;
				graph.Views[graph.PrimaryView].Select(null, null, new object[] { contact.ContactID }, new[] { typeof(Contact.contactID).Name }, null, null, ref startRow, 1, ref totalRows);
				PXAdapter a = new PXAdapter(graph.Views[graph.PrimaryView])
					{
						StartRow = 0,
						MaximumRows = 1,
						Searches = new object[] { contact.ContactID },
						Menu = menu,
						SortColumns = new[] {typeof (Contact.contactID).Name}
					};
                if (graph.Actions.Contains("Action"))
				    foreach (var c in graph.Actions["Action"].Press(a)){}
		}

	}
	#endregion

	#region CRDuplicateBAccountList
	public class CRDuplicateBAccountList : PXSelectBase<CRDuplicateRecord>
	{
		#region MergeParams
		[Serializable]
		[PXHidden]
		public class MergeParams : IBqlTable
		{
			#region SourceBAccountID
			public abstract class sourceBAccountID : IBqlField { }
			[PXDBInt]
			[PXDefault(typeof(BAccount.bAccountID))]
			public virtual int? SourceBAccountID { get; set; }
			#endregion

			#region BAccountID
			public abstract class bAccountID : IBqlField { }
			[PXDBInt]
			[PXUIField(DisplayName = "Target", Required = true)]
			[PXDefault(typeof(BAccount.bAccountID))]
			[CRDuplicateBAccountSelector(typeof(MergeParams.sourceBAccountID))]
			public virtual int? BAccountID { get; set; }
			#endregion
		}
		#endregion

		public class MergeBAccount : CRBaseUpdateProcess<MergeBAccount, BAccount, PXMassMergableFieldAttribute, BAccount.classID> { }
		public class MergeContact : CRBaseUpdateProcess<MergeContact, Contact, PXMassMergableFieldAttribute, Contact.classID> { }
		public class MergeAddress : CRBaseUpdateProcess<MergeAddress, Address, PXMassMergableFieldAttribute, Contact.classID> { }

		private const string MergeActionName = "merge";
		private const string MergeParamsViewName = "mergeParams";
		private const string FieldsViewName = "ValueConflicts";

		private readonly PXVirtualTableView<MergeParams> mergeParam;
	    private readonly PXView _intView;

		public CRDuplicateBAccountList(PXGraph graph)
		{
			_Graph = graph;
			_Graph = graph;
			View = new PXView(_Graph, false,
				new Select2<CRDuplicateRecord,
				LeftJoin<Contact, On<Contact.contactID, Equal<CRDuplicateRecord.contactID>>,
				LeftJoin<CRLeadContactValidationProcess.Contact2,
							On<CRLeadContactValidationProcess.Contact2.contactID, Equal<CRDuplicateRecord.duplicateContactID>>,
		                LeftJoin
		                    <BAccountR, On<BAccountR.bAccountID, Equal<CRLeadContactValidationProcess.Contact2.bAccountID>>>>>>(),                
                 new PXSelectDelegate(Handler)
                );

            _intView = new PXView(_Graph, true,
                new Select2<CRDuplicateRecord,
				LeftJoin<Contact, On<Contact.contactID, Equal<CRDuplicateRecord.contactID>>,
				LeftJoin<CRLeadContactValidationProcess.Contact2,
							On<CRLeadContactValidationProcess.Contact2.contactID, Equal<CRDuplicateRecord.duplicateContactID>>,
				LeftJoin<BAccountR, On<BAccountR.bAccountID, Equal<CRLeadContactValidationProcess.Contact2.bAccountID>>>>>,
			 Where<CRDuplicateRecord.contactID, Equal<Required<BAccount.defContactID>>,
				And<CRDuplicateRecord.duplicateContactID, NotEqual<CRDuplicateRecord.contactID>,
				And<CRDuplicateRecord.validationType, Equal<Switch<Case<Where<Contact.contactType, Equal<ContactTypesAttribute.bAccountProperty>,
																											And<CRLeadContactValidationProcess.Contact2.contactType, Equal<ContactTypesAttribute.bAccountProperty>>>, ValidationTypesAttribute.account,
																								Case<Where<Contact.contactType, Equal<ContactTypesAttribute.bAccountProperty>,
																											Or<CRLeadContactValidationProcess.Contact2.contactType, Equal<ContactTypesAttribute.bAccountProperty>>>, ValidationTypesAttribute.leadAccount>>,
																								ValidationTypesAttribute.leadContact>>,
				And<Required<Contact.duplicateFound>, Equal<True>,
				And2<Where<CRLeadContactValidationProcess.Contact2.contactType, NotEqual<ContactTypesAttribute.bAccountProperty>,
								Or<CRLeadContactValidationProcess.Contact2.contactID, Equal<BAccountR.defContactID>>>,
				And<CRDuplicateRecord.score, GreaterEqual<
							Switch<Case<Where<CRLeadContactValidationProcess.Contact2.contactType, Equal<ContactTypesAttribute.bAccountProperty>>,
													Current<CRSetup.accountValidationThreshold>>, 
										Current<CRSetup.leadToAccountValidationThreshold>>>,				 
				 And<Where<Contact.bAccountID, IsNull,
 								Or<CRLeadContactValidationProcess.Contact2.bAccountID, IsNull, 
								Or<Contact.bAccountID, NotEqual<CRLeadContactValidationProcess.Contact2.bAccountID>>>>>>>>>>>>());

			_Graph.Views.Add(MergeParamsViewName, mergeParam = new PXVirtualTableView<MergeParams>(_Graph));
			_Graph.Views.Add(FieldsViewName, new PXView(_Graph, false,
				new Select<FieldValue, Where<FieldValue.attributeID, IsNull>, OrderBy<Asc<FieldValue.order>>>(),
				(PXSelectDelegate)valueConflicts));

			PXDBAttributeAttribute.Activate(_Graph.Caches[typeof(BAccount)]);
			//Init PXVirtual Static constructor
			typeof(FieldValue).GetCustomAttributes(typeof(PXVirtualAttribute), false);

			_Graph.FieldSelecting.AddHandler<FieldValue.value>(delegate(PXCache sender, PXFieldSelectingEventArgs e)
			{
				if (e.Row == null) return;
				e.ReturnState = InitValueFieldState(e.Row as FieldValue);
			});
			_Graph.RowSelected.AddHandler<Contact>(delegate(PXCache sender, PXRowSelectedEventArgs e)
			{
				sender.Graph.Actions[MergeActionName].SetEnabled(e.Row != null && sender.GetStatus(e.Row) != PXEntryStatus.Inserted);
			});
			_Graph.FieldDefaulting.AddHandler<MergeParams.bAccountID>(MergeParams_BAccountID_FieldDefaulting);
			_Graph.FieldVerifying.AddHandler<MergeParams.bAccountID>(MergeParams_BAccountID_FieldVerifying);
            _Graph.RowUpdated.AddHandler<CRDuplicateRecord>(delegate (PXCache sender, PXRowUpdatedEventArgs e)
            {
                sender.IsDirty = false;
            });
			PXNamedAction.AddAction(_Graph, _Graph.Views[_Graph.PrimaryView].Cache.GetItemType(), MergeActionName, Messages.Merge, Merge);
			PXUIFieldAttribute.SetDisplayName<BAccountR.type>(_Graph.Caches<BAccountR>(), Messages.BAccountType);

			_Graph.EnsureCachePersistence(typeof(CRActivityStatistics));
		}

	    protected virtual IEnumerable Handler()
        {
	        //Need select primary conact to sync duplicate status.
	        Contact defContact = 
                PXSelect<Contact, 
                Where<Contact.contactID, Equal<Current<BAccount.defContactID>>>>.SelectSingleBound(PXView.CurrentGraph, null,null);
	        if (defContact == null)
	        {
	            return new List<object>();
	        }
	        var startRow = PXView.StartRow;
            int totalRows = 0;
            var list = _intView.Select(PXView.Currents, new object []{ defContact.ContactID, defContact.DuplicateFound }, PXView.Searches, PXView.SortColumns,
                PXView.Descendings, PXView.Filters, ref startRow, PXView.MaximumRows, ref totalRows);
            PXView.StartRow = 0;
	        foreach (PXResult<CRDuplicateRecord, Contact, CRLeadContactValidationProcess.Contact2, BAccountR> rec in list)
	        {
	            CRDuplicateRecord db = rec;
	            CRDuplicateRecord cached = (CRDuplicateRecord)this.Cache.Locate(db);
	            db.Selected = cached?.Selected;	            
	        }
	        return list;
        }
		protected virtual void MergeParams_BAccountID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			BAccount current = (BAccount)_Graph.Caches<BAccount>().Current;
			List<BAccount> accounts = PXSelectorAttribute.SelectAll<MergeParams.bAccountID>(sender, e.Row)
				.RowCast<BAccount>()
				.Where(a => a.Type == BAccountType.CustomerType || a.Type == BAccountType.VendorType || a.Type == BAccountType.CombinedType)
				.ToList();
			e.NewValue = (current.Type == BAccountType.CustomerType
				|| current.Type == BAccountType.VendorType
				|| current.Type == BAccountType.CombinedType) && current.Status != BAccount.status.Inactive
				|| accounts.Count == 0
				? current.BAccountID : accounts[0].BAccountID;
		}

		protected virtual void MergeParams_BAccountID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			int? targetID = e.NewValue as int?;
			if (PXSelectorAttribute.SelectAll<MergeParams.bAccountID>(sender, e.Row)
				.RowCast<BAccount>()
				.Any(acc => acc.BAccountID != targetID && acc.Type != BAccountType.ProspectType))
			{
				BAccount acct = PXSelect<BAccount, Where<BAccount.bAccountID, Equal<Required<MergeParams.bAccountID>>>>.Select(_Graph, e.NewValue);
				e.NewValue = acct.AcctCD;
				throw new PXSetPropertyException(Messages.OnlyBAccountMergeSources);
			}
		}

		public IEnumerable valueConflicts()
		{
			return _Graph.Caches[typeof(FieldValue)].Cached.Cast<FieldValue>().Where(fld => fld.Hidden != true);
		}

		protected PXFieldState InitValueFieldState(FieldValue field)
		{
			Tuple<IEnumerable<string>, IEnumerable<string>> possibleValues = new Tuple<IEnumerable<string>, IEnumerable<string>>(new string[0], new string[0]);
			List<BAccount> bAccounts = PXSelectorAttribute.SelectAll<MergeParams.bAccountID>(_Graph.Caches[typeof (MergeParams)], _Graph.Caches[typeof(MergeParams)].Current).RowCast<BAccount>().ToList();
			if (field.CacheName == typeof(BAccount).FullName)
			{
				possibleValues = CRDuplicateContactList.GetPossibleValues(_Graph, bAccounts, field.Name);
			}
			else if (field.CacheName == typeof (Contact).FullName)
			{
				PXSelectBase<Contact> cmd = new PXSelect<Contact>(_Graph);
				List<int?> defContactIDs = new List<int?>(bAccounts.Select(acc => acc.DefContactID));
				foreach (int? c in defContactIDs)
				{
					cmd.WhereOr<Where<Contact.contactID, Equal<Required<BAccount.defContactID>>>>();					
				}
				possibleValues = CRDuplicateContactList.GetPossibleValues(_Graph, cmd.Select(defContactIDs.Cast<object>().ToArray()).RowCast<Contact>(), field.Name);
			}
			else if (field.CacheName == typeof(Address).FullName)
			{
				PXSelectBase<Address> cmd = new PXSelect<Address>(_Graph);
				List<int?> defAddressIDs = new List<int?>(bAccounts.Select(acc => acc.DefAddressID));
				foreach (int? a in defAddressIDs)
				{
					cmd.WhereOr<Where<Address.addressID, Equal<Required<BAccount.defAddressID>>>>();
				}
				possibleValues = CRDuplicateContactList.GetPossibleValues(_Graph, cmd.Select(defAddressIDs.Cast<object>().ToArray()).RowCast<Address>(), field.Name);
			}

			string[] values = possibleValues.Item1.ToArray();
			string[] labels = possibleValues.Item2.ToArray();

			return PXStringState.CreateInstance(field.Value, null, null, typeof(FieldValue.value).Name,
				false, 0, null, values, labels, null, null);
		}

		protected void InsertPropertyValue(FieldValue field, Dictionary<Type, object> targets)
		{
			Type t = Type.GetType(field.CacheName);
			PXCache cache = _Graph.Caches[t];
			object target = targets[t];

			PXStringState state = InitValueFieldState(field) as PXStringState;

			if (state != null)
			{
				if (state.AllowedValues == null || !state.AllowedValues.Any() || state.AllowedValues.Count() == 1 && field.AttributeID == null)
					return;
				if (state.AllowedValues.Count() == 1)
				{
					field.Hidden = true;
					field.Value = state.AllowedValues[0];
				}
				else if (target != null)
				{
					state.Required = true;
					object value = cache.GetValueExt(target, field.Name);
					if (value is PXFieldState) value = ((PXFieldState)value).Value;
					field.Value = value != null ? value.ToString() : null;
				}
			}
			_Graph.Caches[typeof(FieldValue)].Insert(field);
		}

		protected void FillPropertyValue(List<BAccount> baccounts)
		{
			PXCache cache = _Graph.Caches[typeof(FieldValue)];
			cache.Clear();
			
			PXCache<MergeParams> pcache = _Graph.Caches<MergeParams>();
			pcache.SetDefaultExt<MergeParams.bAccountID>(pcache.Current);
			
			int order = 0;
			List<FieldValue> fields = new List<FieldValue>(MergeBAccount.GetProcessingProperties(_Graph, ref order));
			if (baccounts.Any(acc => acc.Type != BAccountType.ProspectType))
			{
				fields = fields.Where(fld => string.Compare(fld.Name, typeof(BAccount.classID).Name, StringComparison.OrdinalIgnoreCase) != 0 || fld.CacheName != typeof(BAccount).FullName).ToList();
			}
			HashSet<string> fieldNames = new HashSet<string>(fields.Select(f => f.Name));

			fields.AddRange(MergeContact.GetMarkedProperties(_Graph, ref order).Where(fld => fieldNames.Add(fld.Name)));
			fields.AddRange(MergeAddress.GetMarkedProperties(_Graph, ref order).Where(fld => fieldNames.Add(fld.Name)));

			BAccount account = PXSelect<BAccount, Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>.Select(_Graph, ((MergeParams)pcache.Current).BAccountID);
			Contact contact = PXSelect<Contact, Where<Contact.contactID, Equal<Required<Contact.contactID>>>>.Select(_Graph, account.DefContactID);
			Address address = PXSelect<Address, Where<Address.addressID, Equal<Required<Address.addressID>>>>.Select(_Graph, account.DefAddressID);
			Dictionary<Type, object> targets = new Dictionary<Type, object>
			{
				{typeof (BAccount), account},
				{typeof (Contact), contact},
				{typeof (Address), address}
			};
			foreach (FieldValue fld in fields)
			{
				InsertPropertyValue(fld, targets);
			}
			cache.IsDirty = false;
		}

		[PXUIField(DisplayName = Messages.Merge)]
		[PXButton]
		public virtual IEnumerable Merge(PXAdapter adapter)
		{
			PXCache<MergeParams> mpcache = _Graph.Caches<MergeParams>();
			List<BAccount> baccounts = null;
			WebDialogResult res = AskExt((graph, name) =>
			{
				mpcache.Clear();
				mpcache.Insert();
				baccounts = new List<BAccount>(PXSelectorAttribute.SelectAll<MergeParams.bAccountID>(mpcache, mpcache.Current)
				.RowCast<BAccount>()
				.Select(c => _Graph.Caches[typeof(BAccount)].CreateCopy(c))
				.Cast<BAccount>());

				if (baccounts.Count < 2)
				{
					throw new PXException(Messages.DuplicatesNotSelected);
				}

				FillPropertyValue(baccounts);
			});

			if (res != WebDialogResult.OK || !mergeParam.VerifyRequired()) return adapter.Get();

			int? targetID = ((MergeParams)mpcache.Current).BAccountID;
			List<FieldValue> values = new List<FieldValue>(_Graph.Caches[typeof(FieldValue)].Cached.Cast<FieldValue>()
				.Select(v => _Graph.Caches[typeof(FieldValue)].CreateCopy(v))
				.Cast<FieldValue>());

			if (baccounts == null)
			{
				baccounts = new List<BAccount>(PXSelectorAttribute.SelectAll<MergeParams.bAccountID>(mpcache, mpcache.Current)
					.RowCast<BAccount>()
					.Select(c => _Graph.Caches[typeof(BAccount)].CreateCopy(c))
					.Cast<BAccount>());
			}

			_Graph.Actions.PressSave();
			PXLongOperation.StartOperation(this._Graph, () => MergeBAccounts((int) targetID, baccounts, values));
			
			return adapter.Get();
		}

		public static void MergeBAccounts(int targetID, List<BAccount> accounts, List<FieldValue> values)
		{
			PXGraph graph = new PXGraph();
			PXPrimaryGraphCollection primaryGraph = new PXPrimaryGraphCollection(graph);
			BAccount target = (BAccount)PXSelect<BAccount, Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>.Select(graph, targetID);

			graph = primaryGraph[target];
			PXCache cache = graph.Caches[typeof(BAccount)];

            var refNoteIdField = EntityHelper.GetNoteField(cache.GetItemType());

            PXView view = new PXView(graph, false, BqlCommand.CreateInstance(BqlCommand.Compose(
				typeof(Select<,>), 
					cache.GetItemType(), 
					typeof(Where<,>),
						cache.GetBqlField<BAccount.bAccountID>(),
						typeof(Equal<>), typeof(Required<>), typeof(BAccount.bAccountID)
			)));
			object copy = view.SelectSingle(targetID);

			Contact targetContact = PXSelect<Contact, Where<Contact.contactID, Equal<Required<Contact.contactID>>>>.Select(graph, target.DefContactID);
			Address targetAddress = PXSelect<Address, Where<Address.addressID, Equal<Required<Address.addressID>>>>.Select(graph, target.DefAddressID);

			Dictionary<Type, object> targets = new Dictionary<Type, object> {{typeof (BAccount), copy}, {typeof(Contact), targetContact}, {typeof(Address), targetAddress}};

			PXCache attrCache = graph.Caches[typeof(CSAnswers)];
			foreach (FieldValue fld in values)
			{
				if (fld.AttributeID == null)
				{
					Type type = Type.GetType(fld.CacheName);
					PXFieldState state = (PXFieldState)graph.Caches[type].GetStateExt(targets[type], fld.Name);
					if (state == null || !Equals(state.Value, fld.Value))
					{
						graph.Caches[type].SetValueExt(targets[type], fld.Name, fld.Value);
						targets[type] = graph.Caches[type].CreateCopy(graph.Caches[type].Update(targets[type]));
					}
				}
				else
				{
					CSAnswers attr = new CSAnswers
					{
						AttributeID = fld.AttributeID,
                        RefNoteID = cache.GetValue(target, refNoteIdField) as Guid?,
                        Value = fld.Value
					};
					attrCache.Update(attr);
				}
			}

			if (attrCache.IsDirty) graph.EnsureCachePersistence(attrCache.GetItemType());

			target = (BAccount) targets[typeof (BAccount)];
			using (PXTransactionScope scope = new PXTransactionScope())
			{
				BusinessAccountMaint bagraph = PXGraph.CreateInstance<BusinessAccountMaint>();
				foreach (BAccount baccount in accounts.Where(a => a.BAccountID != targetID))
				{
					if (baccount.Type != BAccountType.ProspectType)
						throw new PXException(Messages.MergeNonProspect);

					int? defContactID = baccount.DefContactID;
					PXCache Contacts = graph.Caches[typeof(Contact)];
					foreach (Contact contact in PXSelect<Contact, Where<Contact.bAccountID, Equal<Current<BAccount.bAccountID>>>>
                        .Select(graph, baccount.BAccountID)
						.RowCast<Contact>()
						.Where(c => c.ContactID != defContactID)
						.Select(c => (Contact)Contacts.CreateCopy(c)))
					{
						contact.BAccountID = target.BAccountID;
						Contacts.Update(contact);
					}

					PXCache Activities = graph.Caches[typeof(CRPMTimeActivity)];
                    foreach (CRPMTimeActivity activity in PXSelect<CRPMTimeActivity, Where<CRPMTimeActivity.bAccountID, Equal<Required<BAccount.bAccountID>>>>
                        .Select(graph, baccount.NoteID)
						.RowCast<CRPMTimeActivity>()
						.Select(cas => (CRPMTimeActivity)Activities.CreateCopy(cas)))
					{
						if (activity.RefNoteID == baccount.NoteID)
						{
							activity.RefNoteID = target.NoteID;
						}
						activity.BAccountID = target.BAccountID;
						Activities.Update(activity);
					}

					PXCache Cases = graph.Caches[typeof(CRCase)];
                    foreach (CRCase cas in PXSelect<CRCase, Where<CRCase.customerID, Equal<Required<BAccount.bAccountID>>>>
                        .Select(graph, baccount.BAccountID)
						.RowCast<CRCase>()
						.Select(cas => (CRCase)Cases.CreateCopy(cas)))
					{
						cas.CustomerID = target.BAccountID;
						Cases.Update(cas);
					}

                    PXCache Opportunities = bagraph.Caches[typeof(CROpportunity)];
                    foreach (CROpportunity opp in PXSelect<CROpportunity, Where<CROpportunity.bAccountID, Equal<Required<BAccount.bAccountID>>>>
						.Select(graph, baccount.BAccountID)
						.RowCast<CROpportunity>()
						.Select(opp => (CROpportunity)Opportunities.CreateCopy(opp)))
					{
						opp.BAccountID = target.BAccountID;
						opp.LocationID = target.DefLocationID;
						Opportunities.Update(opp);
					}

					PXCache Relations = graph.Caches[typeof(CRRelation)];
					foreach (CRRelation rel in PXSelectJoin<CRRelation,
						LeftJoin<CRRelation2, 
							On<CRRelation.entityID, Equal<CRRelation2.entityID>,
							And<CRRelation.role, Equal<CRRelation2.role>,
							And<CRRelation2.refNoteID, Equal<Required<BAccount.noteID>>>>>>,
						Where<CRRelation2.entityID, IsNull,
							And<CRRelation.refNoteID, Equal<Required<BAccount.noteID>>>>>
						.Select(graph, target.NoteID, baccount.NoteID)
						.RowCast<CRRelation>()
						.Select(rel => (CRRelation)Relations.CreateCopy(rel)))
					{
						rel.RelationID = null;
						rel.RefNoteID = target.NoteID;
						Relations.Insert(rel);
					}

                    bagraph.Caches[typeof(BAccount)].Delete(baccount);
                    bagraph.Actions.PressSave();

				}				
				RunAction(graph, target, "MarkAsValidated");
				graph.Actions.PressSave();			
				scope.Complete();
			}
			throw new PXRedirectRequiredException(graph, Messages.BAccount);
		}

		private static void RunAction(PXGraph graph, BAccount baccount, string menu)
		{
			int startRow = 0;
			int totalRows = 1;
			graph.Views[graph.PrimaryView].Select(null, null, new object[] { baccount.BAccountID }, new[] { typeof(BAccount.bAccountID).Name }, null, null, ref startRow, 1, ref totalRows);
			PXAdapter a = new PXAdapter(graph.Views[graph.PrimaryView])
			{
				StartRow = 0,
				MaximumRows = 1,
				Searches = new object[] { baccount.BAccountID },
				Menu = menu,
				SortColumns = new[] { typeof(BAccount.bAccountID).Name }
			};
			foreach (var c in graph.Actions["Action"].Press(a)) { }
		}

	}
	#endregion
	[Serializable]
    [PXHidden]
	public partial class CRRelation2: CRRelation
	{
		public new abstract class refNoteID : IBqlField { }
		public new abstract class entityID : IBqlField { }
		public new abstract class role : IBqlField { }
	}

	[Serializable]
    [PXHidden]
	public partial class CRMarketingListMember2 : CRMarketingListMember
	{
		public new abstract class contactID : IBqlField { }
		public new abstract class marketingListID : IBqlField { }
	}

	[Serializable]
    [PXHidden]
	public partial class CRCampaignMembers2 : CRCampaignMembers
	{
		public new abstract class contactID : IBqlField { }
		public new abstract class campaignID : IBqlField { }
	}

	[Serializable]
    [PXHidden]
	public partial class ContactNotification2 : ContactNotification
	{
		public new abstract class contactID : IBqlField { }
		public new abstract class setupID : IBqlField { }
	}

	public class PXVirtualTableView<TTable> : PXView 
		where TTable : IBqlTable
	{
		public PXVirtualTableView(PXGraph graph)
			: base(graph, false, new Select<TTable>())
		{
			_Delegate = (PXSelectDelegate)Get;
			_Graph.Defaults[_Graph.Caches[typeof(TTable)].GetItemType()] = getFilter;
			_Graph.RowPersisting.AddHandler(typeof(TTable), persisting);
		}
		public IEnumerable Get()
		{
			PXCache cache = _Graph.Caches[typeof(TTable)];
			cache.AllowInsert = true;
			cache.AllowUpdate = true;
			object curr = cache.Current;
			if (curr != null && cache.Locate(curr) == null)
			{
				try
				{
					curr = cache.Insert(curr);
				}
				catch
				{
					cache.SetStatus(curr, PXEntryStatus.Inserted);
				}
			}
			yield return curr;
			cache.IsDirty = false;
		}

		private TTable current;
		private bool _inserting = false;
		private object getFilter()
		{
			PXCache cache = _Graph.Caches[typeof(TTable)];

			if (!_inserting)
			{
				try
				{
					_inserting = true;
					if (current == null)
					{
						current = (TTable)(cache.Insert() ?? cache.Locate(cache.CreateInstance()));
						cache.IsDirty = false;
					}
					else if (cache.Locate(current) == null)
					{
						try
						{
							current = (TTable)cache.Insert(current);
						}
						catch
						{
							cache.SetStatus(current, PXEntryStatus.Inserted);
						}
						cache.IsDirty = false;
					}
				}
				finally
				{
					_inserting = false;
				}
			}
			return current;
		}
		private static void persisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			e.Cancel = true;
		}

		public bool VerifyRequired()
		{
			return VerifyRequired(false);
		}
		public virtual bool VerifyRequired(bool suppressError)
		{
			Cache.RaiseRowSelected(Cache.Current);
			bool result = true;
			PXRowPersistingEventArgs e = new PXRowPersistingEventArgs(PXDBOperation.Insert, Cache.Current);
			foreach (string field in Cache.Fields)
			{
				foreach (PXDefaultAttribute defAttr in Cache.GetAttributes(Cache.Current, field).OfType<PXDefaultAttribute>())
				{
					defAttr.RowPersisting(Cache, e);
					bool error = !string.IsNullOrEmpty(PXUIFieldAttribute.GetError(Cache, Cache.Current, field));
					if (error) result = false;

					if (suppressError && error)
					{
						Cache.RaiseExceptionHandling(field, Cache.Current, null, null);
						return false;
					}
				}
			}
			return result;
		}

	}

}

