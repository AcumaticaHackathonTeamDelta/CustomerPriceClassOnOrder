using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Compilation;
using PX.Data;
using PX.Objects.CR;
using PX.SM;
using PX.TM;
using PX.Objects.CM;

namespace PX.Objects.EP
{

	[PX.Objects.GL.TableAndChartDashboardType]
	public class EPApprovalProcess : PXGraph<EPApprovalProcess>
	{
		public PXSelect<BAccount> bAccount;

		[Serializable]
        [PXProjection(typeof(Select5<EPApproval, InnerJoin<Note,
										On<Note.noteID, Equal<EPApproval.refNoteID>,
									 And<EPApproval.status, Equal<EPApprovalStatus.pending>>>>,
                                     Where2<Where<EPApproval.ownerID, IsNotNull, And<EPApproval.ownerID, Equal<CurrentValue<AccessInfo.userID>>>>, 
                                            Or2<Where<EPApproval.workgroupID, InMember<CurrentValue<AccessInfo.userID>>,
                                            Or<EPApproval.workgroupID, IsNull>>,
                                            Or<EPApproval.workgroupID, Owned<CurrentValue<AccessInfo.userID>>>>>,
                                     Aggregate<GroupBy<EPApproval.refNoteID, 
			GroupBy<EPApproval.curyInfoID, 
			GroupBy<EPApproval.bAccountID, 
			GroupBy<EPApproval.ownerID, 
			GroupBy<EPApproval.approvedByID,
			GroupBy<EPApproval.curyTotalAmount>>>>>>>>))]
		public partial class EPOwned : EPApproval
		{
			#region RefNoteID
			public new abstract class refNoteID : PX.Data.IBqlField
			{
			}
			[PXRefNote(BqlTable = typeof(EPApproval), LastKeyOnly=true)]
			[PXUIField(DisplayName = "Reference Nbr.")]
			[PXNoUpdate]
			public override Guid? RefNoteID
			{
				get
				{
					return this._RefNoteID;
				}
				set
				{
					this._RefNoteID = value;
				}
			}
			#endregion
			#region Selected
			public abstract class selected : PX.Data.IBqlField
			{
			}
			protected bool? _Selected = false;
			[PXBool]
			[PXDefault(false)]
			[PXUIField(DisplayName = "Selected")]
			public virtual bool? Selected
			{
				get
				{
					return _Selected;
				}
				set
				{
					_Selected = value;
				}
			}
			#endregion
			#region Escalated
			public abstract class escalated : PX.Data.IBqlField
			{
			}
			protected bool? _Escalated;

			[PXBool]
            [PXUIField(DisplayName = "Escalated")]
			public virtual bool? Escalated
			{
				get
				{
					return this._Escalated;
				}
				set
				{
					this._Escalated = value;
				}
			}
			#endregion

			#region BAccountID
			public new abstract class bAccountID : PX.Data.IBqlField
			{
			}
			[PXDBInt(BqlTable = typeof(EPApproval))]
			[PXUIField(DisplayName = "Business Account")]
			[PXSelector(typeof(BAccount.bAccountID), SubstituteKey = typeof(BAccount.acctCD), DescriptionField = typeof(BAccount.acctName))]
			public override Int32? BAccountID
			{
				get
				{
					return this._BAccountID;
				}
				set
				{
					this._BAccountID = value;
				}
			}
			#endregion
			#region CuryInfoID
			public new abstract class curyInfoID : PX.Data.IBqlField
			{
			}
			[PXDBLong(BqlTable = typeof(EPApproval))]
			[CM.CurrencyInfo()]
			public override Int64? CuryInfoID
			{
				get
				{
					return this._CuryInfoID;
				}
				set
				{
					this._CuryInfoID = value;
				}
			}
			#endregion			
			#region CuryTotalAmount
			public new abstract class curyTotalAmount : PX.Data.IBqlField
			{
			}
            [PXDBCurrency(typeof(EPOwned.curyInfoID), typeof(EPOwned.totalAmount), BqlTable = typeof(EPApproval))]
			[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
			[PXUIField(DisplayName = "Total Amount")]
			public override Decimal? CuryTotalAmount
			{
				get
				{
					return this._CuryTotalAmount;
				}
				set
				{
					this._CuryTotalAmount = value;
				}
			}
			#endregion
			#region TotalAmount
			public new abstract class totalAmount : PX.Data.IBqlField
			{
			}
			
			[PXDBDecimal(4, BqlTable = typeof(EPApproval))]
			[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
			public override Decimal? TotalAmount
			{
				get
				{
					return this._TotalAmount;
				}
				set
				{
					this._TotalAmount = value;
				}
			}
			#endregion                        
			#region CreatedDateTime
			public new abstract class createdDateTime : PX.Data.IBqlField
			{
			}
			[PXDBDate(PreserveTime = true, DisplayMask = "g", BqlTable = typeof(EPApproval))]
			[PXUIField(DisplayName = "Requested Time")]
			public override DateTime? CreatedDateTime
			{
				get
				{
					return this._CreatedDateTime;
				}
				set
				{
					this._CreatedDateTime = value;
				}
			}
			#endregion
			#region EntityType
			public abstract class entityType : IBqlField
			{
			}
			private string _EntityType;
			[PXDBString(BqlTable = typeof(Note))]
			public string EntityType
			{
				get
				{
					return _EntityType;
				}
				set
				{
					_EntityType = value;
				}
			}
			#endregion
			#region DocType
			public new abstract class docType : IBqlField
			{
			}
			private string _DocType;
			[PXString()]
			[PXUIField(DisplayName = "Type")]
			[PXFormula(typeof(ApprovalDocType<EPOwned.entityType, EPOwned.sourceItemType>))]
			public override string DocType
			{
				get
				{
					return _DocType;
				}
				set
				{
					_DocType = value;
				}
			}
			#endregion
			public override Guid? NoteID
			{
				get
				{
					return base.NoteID;
				}
				set
				{
					base.NoteID = value;
				}
			}
		}

		public PXCancel<EPOwned> Cancel;

        public PXAction<EPOwned> EditDetail;
        [PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXEditDetailButton]
        public virtual IEnumerable editDetail(PXAdapter adapter)
        {
            if (Records.Current != null && Records.Current.RefNoteID != null)
            {
				bool navigate = true;

                EntityHelper helper = new EntityHelper(this);
				Note note = helper.SelectNote(Records.Current.RefNoteID);

				if (note != null && note.EntityType == typeof(EPExpenseClaim).FullName)
				{
					EPExpenseClaim claim = PXSelect<EPExpenseClaim, Where<EPExpenseClaim.noteID, Equal<Required<EPExpenseClaim.noteID>>>>.Select(this, note.NoteID);

					if (claim != null)
					{
						ExpenseClaimEntry graph = PXGraph.CreateInstance<ExpenseClaimEntry>();
						EPExpenseClaim target = graph.ExpenseClaim.Search<EPExpenseClaim.refNbr>(claim.RefNbr);
						if (target == null)
						{
							navigate = false;
						}

					}
				}

				if (note != null && note.EntityType == typeof(EPTimeCard).FullName)
				{
					EPTimeCard timecard = PXSelect<EPTimeCard, Where<EPTimeCard.noteID, Equal<Required<EPTimeCard.noteID>>>>.Select(this, note.NoteID);

					if (timecard != null)
					{
						TimeCardMaint graph = PXGraph.CreateInstance<TimeCardMaint>();
						EPTimeCard target = graph.Document.Search<EPTimeCard.timeCardCD>(timecard.TimeCardCD);
						if (target == null)
						{
							navigate = false;
						}
						
					}
				}
						
				if (navigate)
                helper.NavigateToRow(Records.Current.RefNoteID.Value, PXRedirectHelper.WindowMode.InlineWindow);
            }
            return adapter.Get();
        }

		[PXFilterable]
		public PXProcessing<EPOwned,Where<True, Equal<True>>, OrderBy<Desc<EPOwned.docDate, Asc<EPOwned.approvalID>>>> Records;

		public PXSetup<EPSetup> EPSetup;

		[PXHidden]
		public PXSelect<PM.PMProject> Projects;

		public EPApprovalProcess()
		{
			Records.SetProcessCaption(EP.Messages.Approve);
			Records.SetProcessAllCaption(EP.Messages.ApproveAll);
			Records.SetSelected<EPOwned.selected>();
			Records.SetProcessDelegate(Approve);
		}

	    public virtual IEnumerable records()
	    {
	        Records.Cache.AllowInsert = false;
	        Records.Cache.AllowDelete = false;

	        PXSelectBase<EPOwned> select =
	            new PXSelect<EPOwned,
	                Where<True, Equal<True>>,
	                OrderBy<Desc<EPOwned.docDate, Asc<EPOwned.approvalID>>>>(this);
	        select.View.Clear();

			int startRow = PXView.StartRow;
			int totalRows = 0;
			foreach (EPOwned doc in select.View.Select(PXView.Currents, null, PXView.Searches, PXView.SortColumns, PXView.Descendings, PXView.Filters, ref startRow, PXView.MaximumRows, ref totalRows))
	        {
	            doc.Escalated = GetEscalated(doc.RefNoteID);
	            yield return doc;
	        }
			PXView.StartRow = 0;
	    }

	    private bool GetEscalated(Guid? refNoteID)
		{
			EPOwned item =
			PXSelect<EPOwned,
			Where<EPOwned.refNoteID, Equal<Required<EPOwned.refNoteID>>,
				And<EPOwned.status, Equal<EPApprovalStatus.pending>,
				And<EPOwned.workgroupID, Escalated<Current<AccessInfo.userID>, EPApproval.workgroupID, EPApproval.ownerID, EPApproval.createdDateTime>>>>>
			.Select(this, refNoteID);

			return item != null;
		}

		protected virtual void EPOwned_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			EPOwned row = e.Row as EPOwned;
			if (row != null && row.Selected != true)
				sender.SetStatus(row, PXEntryStatus.Notchanged);
		}

		public override bool IsDirty
		{
			get
			{
				return false;
			}
		}

		protected static void Approve(List<EPOwned> items)
		{
			EntityHelper helper = new EntityHelper(new PXGraph());
			var graphs = new Dictionary<Type, PXGraph>();

			bool errorOccured = false;
			foreach (EPOwned item in items)
			{
				try
				{
					PXProcessing<EPApproval>.SetCurrentItem(item);
					if (item.RefNoteID == null) throw new PXException(Messages.ApprovalRefNoteIDNull);
					object row = helper.GetEntityRow(item.RefNoteID.Value, true);

					if (row == null) throw new PXException(Messages.ApprovalRecordNotFound);

					Type cahceType = row.GetType();
					Type graphType = helper.GetPrimaryGraphType(row, false);
					PXGraph graph;
					if(!graphs.TryGetValue(graphType, out graph))
					{
						graphs.Add(graphType, graph = PXGraph.CreateInstance(graphType));
					}
					graph.Clear();
					graph.Caches[cahceType].Current = row;
					graph.Caches[cahceType].SetStatus(row, PXEntryStatus.Notchanged);
					PXAutomation.GetView(graph);
					string approved = typeof (EPExpenseClaim.approved).Name;
					if (graph.AutomationView != null)
					{
						PXAutomation.GetStep(graph,
						                                            new object[] {graph.Views[graph.AutomationView].Cache.Current},
						                                            BqlCommand.CreateInstance(
						                                            	typeof (Select<>),
						                                            	graph.Views[graph.AutomationView].Cache.GetItemType())
							);
					}

					if(graph.Actions.Contains("Approve"))
						graph.Actions["Approve"].Press();
					else if (graph.AutomationView != null)
					{
						PXView view = graph.Views[graph.AutomationView];
						BqlCommand select = view.BqlSelect;																				
						PXAdapter adapter = new PXAdapter(new DummyView(graph, select, new List<object> { row }));
						adapter.Menu = "Approve";
                        if (graph.Actions.Contains("Action"))
                        {
                            if (!CheckRights(graphType, cahceType))
                                throw new PXException(Messages.DontHaveAppoveRights);
                            foreach (var i in graph.Actions["Action"].Press(adapter)) ;
                        }
                        else
                        {
                            throw new PXException("Automation for screen/graph {0} exists but is not configured properly. Failed to find action - 'Action'", graph);
                        }
						//PXAutomation.ApplyAction(graph, graph.Actions["Action"], "Approve", row, out rollback);							
					}
					else if (graph.Caches[cahceType].Fields.Contains(approved))
					{
						object upd = graph.Caches[cahceType].CreateCopy(row);
						graph.Caches[cahceType].SetValue(upd, approved, true);
						graph.Caches[cahceType].Update(upd);
					}
					graph.Persist();
                    PXProcessing<EPApproval>.SetInfo(ActionsMessages.RecordProcessed);
				}
				catch (Exception ex)
				{
					errorOccured = true;
					PXProcessing<EPApproval>.SetError(ex);
				}
			}
			if(errorOccured)
				throw new PXOperationCompletedWithErrorException(ErrorMessages.SeveralItemsFailed);
		}

        private static bool CheckRights(Type graphType, Type cacheType)
        {
            PXCacheRights rights;
            List<string> invisible = null;
            List<string> disabled = null;

            PXAccess.Provider.GetRights(PXSiteMap.Provider.FindSiteMapNode(graphType).ScreenID, graphType.Name,
                            cacheType, out rights, out invisible, out disabled);

            var actionName = "Approve@Action";
            if (disabled != null && CompareIgnoreCase.IsInList(disabled, actionName))
                return false;

            return true;
        }

		private sealed class DummyView : PXView
		{
			private readonly List<object> _records;
			internal DummyView(PXGraph graph, BqlCommand command, List<object> records)
				: base(graph, true, command)
			{
				_records = records;
			}
			public override List<object> Select(object[] currents, object[] parameters, object[] searches, string[] sortcolumns, bool[] descendings, PXFilterRow[] filters, ref int startRow, int maximumRows, ref int totalRows)
			{
				return _records;
			}
		}
	}
}
