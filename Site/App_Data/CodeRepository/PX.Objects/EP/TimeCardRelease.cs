using System.Collections;
using System.Collections.Generic;
using PX.Data;
using System;
using PX.Objects.CR;
using PX.Objects.PM;

namespace PX.Objects.EP
{
	[GL.TableDashboardType]
	public class TimeCardRelease : PXGraph<TimeCardRelease>
	{
		public PXCancel<EPTimeCardRow> Cancel;
		public PXAction<EPTimeCardRow> viewDetails;

		#region Selects

		[PXViewName(Messages.Timecards)]
		[PXFilterable]
		public PXProcessingJoin<EPTimeCardRow,
			LeftJoin<EPEmployee, On<EPEmployee.bAccountID, Equal<EPTimeCardRow.employeeID>>,
			LeftJoin<EPApprovalEx, On<EPApprovalEx.refNoteID, Equal<EPTimeCardRow.noteID>>,
			LeftJoin<EPEmployeeEx, On<EPEmployeeEx.userID, Equal<EPApprovalEx.approvedByID>>>>>,
			Where<EPTimeCardRow.isApproved, Equal<True>,
				And2<Where<EPTimeCardRow.isReleased, NotEqual<True>, Or<EPTimeCardRow.isReleased, IsNull>>,
				And2<Where<EPTimeCardRow.isHold, NotEqual<True>, Or<EPTimeCardRow.isHold, IsNull>>,
				And<Where<EPTimeCardRow.isRejected, NotEqual<True>, Or<EPTimeCardRow.isRejected, IsNull>>>>>>,
			OrderBy<Asc<EPTimeCardRow.timeCardCD>>> FilteredItems;

		public IEnumerable filteredItems()
		{
			//due to 2-level approval JOIN to EPApprovalEx can produce duplicate records.
			//timecards hashset maintains the list of unique items.
			HashSet<string> timeCards = new HashSet<string>();

			bool found = false;
			foreach (EPTimeCardRow item in FilteredItems.Cache.Inserted)
			{
				timeCards.Add(item.TimeCardCD);

				found = true;
				yield return item;
			}
			if (found)
				yield break;

			PXSelectBase<EPTimeCardRow> select = new PXSelectJoin<EPTimeCardRow,
			LeftJoin<EPEmployee, On<EPEmployee.bAccountID, Equal<EPTimeCardRow.employeeID>>,
			LeftJoin<EPApprovalEx, On<EPApprovalEx.refNoteID, Equal<EPTimeCardRow.noteID>>,
			LeftJoin<EPEmployeeEx, On<EPEmployeeEx.userID, Equal<EPApprovalEx.approvedByID>>>>>,
			Where<EPTimeCardRow.isApproved, Equal<True>,
				And2<Where<EPTimeCardRow.isReleased, NotEqual<True>, Or<EPTimeCardRow.isReleased, IsNull>>,
				And2<Where<EPTimeCardRow.isHold, NotEqual<True>, Or<EPTimeCardRow.isHold, IsNull>>,
				And<Where<EPTimeCardRow.isRejected, NotEqual<True>, Or<EPTimeCardRow.isRejected, IsNull>>>>>>,
			OrderBy<Asc<EPTimeCardRow.timeCardCD,
					Desc<EPTimeCardRow.approveDate>>>>(this);//Sort by approveDate to display last approver (for 2-level approval)


			foreach (PXResult<EPTimeCardRow, EPEmployee, EPApprovalEx, EPEmployeeEx> res in select.Select())
			{
				EPTimeCardRow rec = (EPTimeCardRow)res;
				EPApprovalEx approval = (EPApprovalEx)res;
				EPEmployeeEx approver = (EPEmployeeEx)res[3];
				if (timeCards.Add(rec.TimeCardCD))
				{
					rec.ApproveDate = approval.ApproveDate;
					rec.ApprovedByID = approver.AcctCD;
					rec.ApprovedByName = approver.AcctName;
					FilteredItems.Cache.SetStatus(rec, PXEntryStatus.Inserted);
					yield return rec;
				}
			}

			FilteredItems.Cache.IsDirty = false;
		}

		public PXSetup<EPSetup> Setup;

		#endregion

		#region Ctors

		public TimeCardRelease()
		{
			FilteredItems.SetProcessCaption(Messages.Release);
			FilteredItems.SetProcessAllCaption(Messages.ReleaseAll);
			FilteredItems.SetSelected<EPTimeCard.selected>();

			FilteredItems.SetProcessDelegate(TimeCardRelease.Release);

		}

		#endregion
		#region Actions



		[PXUIField(DisplayName = "")]
		[PXEditDetailButton]
		public virtual IEnumerable ViewDetails(PXAdapter adapter)
		{
			var row = FilteredItems.Current;
			if (row != null)
			{

				TimeCardMaint graph = PXGraph.CreateInstance<TimeCardMaint>();
				graph.Document.Current = graph.Document.Search<EPTimeCard.timeCardCD>(row.TimeCardCD);
				throw new PXRedirectRequiredException(graph, true, Messages.ViewDetails)
				{
					Mode = PXBaseRedirectException.WindowMode.NewWindow
				};


			}
			return adapter.Get();
		}

		#endregion

		public override IEnumerable ExecuteSelect(string viewName, object[] parameters, object[] searches, string[] sortcolumns,
			bool[] descendings, PXFilterRow[] filters, ref int startRow, int maximumRows, ref int totalRows)
		{
			if (viewName == "FilteredItems")
			{
				for (int i = 0; i < sortcolumns.Length; i++)
				{
					if (string.Compare(sortcolumns[i], "WeekID_description", true) == 0)
					{
						sortcolumns[i] = "WeekID";
					}
				}
			}

			return base.ExecuteSelect(viewName, parameters, searches, sortcolumns, descendings, filters, ref startRow, maximumRows, ref totalRows);
		}


		public static void Release(List<EPTimeCardRow> timeCards)
		{
			TimeCardMaint timeCardMaint = PXGraph.CreateInstance<TimeCardMaint>();
			for (int i = 0; i < timeCards.Count; i++)
			{
				timeCardMaint.Clear();
				timeCardMaint.Document.Current = timeCardMaint.Document.Search<EPTimeCard.timeCardCD>(timeCards[i].TimeCardCD);

				if (timeCardMaint.Document.Current == null)
				{
					PXProcessing<EPTimeCardRow>.SetError(i, Messages.TimecardCannotBeReleased_NoRights);
				}
				else
				{
					timeCardMaint.release.Press();
					PXProcessing<EPTimeCardRow>.SetInfo(i, ActionsMessages.RecordProcessed);
				}
			}
		}

		[PXHidden]
		[Serializable]
		public partial class EPApprovalEx : EPApproval
		{
			#region RefNoteID
			public new abstract class refNoteID : PX.Data.IBqlField
			{
			}
			#endregion
			#region ApprovedByID
			public new abstract class approvedByID : IBqlField { }
			[PXDBGuid()]
			[PX.TM.PXOwnerSelector]
			[PXUIField(DisplayName = "Approved by", Visibility = PXUIVisibility.Visible, Enabled = false)]
			public override Guid? ApprovedByID
			{
				get
				{
					return this._ApprovedByID;
				}
				set
				{
					this._ApprovedByID = value;
				}
			}
			#endregion
			#region ApproveDate
			public new abstract class approveDate : PX.Data.IBqlField
			{
			}
			[PXDBDate()]
			[PXUIField(DisplayName = "Approve Date", Enabled = false)]
			public override DateTime? ApproveDate
			{
				get
				{
					return this._ApproveDate;
				}
				set
				{
					this._ApproveDate = value;
				}
			}
			#endregion
		}

		[PXHidden]
		[Serializable]
		public class EPTimeCardRow : TimecardWithTotals
		{
			public new abstract class timeCardCD : IBqlField { }
			public new abstract class employeeID : IBqlField { }
			public new abstract class noteID : IBqlField { }
			public new abstract class isApproved : IBqlField { }
			public new abstract class isReleased : IBqlField { }
			public new abstract class isHold : IBqlField { }
			public new abstract class isRejected : IBqlField { }


			#region ApprovedByID
			public abstract class approvedByID : IBqlField
			{
			}
			protected string _ApprovedByID;
			[PXString(30, IsUnicode = true, InputMask = "")]
			[PXUIField(DisplayName = "Approved by", Visibility = PXUIVisibility.SelectorVisible)]
			public virtual string ApprovedByID
			{
				get
				{
					return this._ApprovedByID;
				}
				set
				{
					this._ApprovedByID = value;
				}
			}
			#endregion
			#region ApprovedByName
			public abstract class approvedByName : IBqlField
			{
			}
			protected string _ApprovedByName;
			[PXString(30, IsUnicode = true, InputMask = "")]
			[PXUIField(DisplayName = "Approver Name", Visibility = PXUIVisibility.SelectorVisible)]
			public virtual string ApprovedByName
			{
				get
				{
					return this._ApprovedByName;
				}
				set
				{
					this._ApprovedByName = value;
				}
			}
			#endregion

			#region ApproveDate
			public abstract class approveDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _ApproveDate;
			[PXDate()]
			[PXUIField(DisplayName = "Approve Date", Enabled = false)]
			public virtual DateTime? ApproveDate
			{
				get
				{
					return this._ApproveDate;
				}
				set
				{
					this._ApproveDate = value;
				}
			}
			#endregion
		}

	}
}
