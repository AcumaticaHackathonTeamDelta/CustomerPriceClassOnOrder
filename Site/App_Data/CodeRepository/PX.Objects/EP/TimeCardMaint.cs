using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Common;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.CT;
using PX.Objects.PM;
using PX.Objects.GL;
using System.Diagnostics;
using System.Globalization;
using PX.SM;
using PX.TM;
using PX.Web.UI;
using Branch = PX.Objects.GL.Branch;

namespace PX.Objects.EP
{   
    [Serializable]
	public class TimeCardMaint : PXGraph<TimeCardMaint, EPTimeCard>, PXImportAttribute.IPXPrepareItems
	{
		#region DAC Overrides

		[PXDBDate()]
		[PXDefault(typeof(EPTimeCard.createdDateTime), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void EPApproval_DocDate_CacheAttached(PXCache sender)
		{
		}

		[PXDBInt()]
		[PXDefault(typeof(EPTimeCard.employeeID), PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void EPApproval_BAccountID_CacheAttached(PXCache sender)
		{
		}

		[PXDBLong()]
		protected virtual void EPApproval_CuryInfoID_CacheAttached(PXCache sender)
		{
		}

		#endregion

		#region Selects

		//This view is required for 2 purposes - 1 so that correct cache is initialized ApproverEmployee -> EPEmployee
		//										 2 Employee object with all its properties can be used in the Assignment Rules.

		[PXViewName(Messages.Employee)]
        public PXSelect<EPEmployee, Where<EPEmployee.bAccountID, Equal<Current<EPTimeCard.employeeID>>>> Employee;
            
        [PXHidden]
		public PXSetup<EPSetup> EpSetup;
		
		[PXHidden]
        public PXSelect<EPEarningType> EarningTypes;
            
        [PXHidden]
		public PXSelect<BAccount> AccountBase;

		[PXViewName(Messages.TimeCardDocument)]
		public PXSelectJoin<EPTimeCard,
						InnerJoin<EPEmployee, On<EPEmployee.bAccountID, Equal<EPTimeCard.employeeID>>>,
						Where<EPTimeCard.createdByID, Equal<Current<AccessInfo.userID>>,
										Or<EPEmployee.userID, Equal<Current<AccessInfo.userID>>,
										Or<EPEmployee.userID, OwnedUser<Current<AccessInfo.userID>>,
										Or<EPTimeCard.noteID, Approver<Current<AccessInfo.userID>>,
										Or<EPTimeCard.employeeID, WingmanUser<Current<AccessInfo.userID>>>>>>>> Document;

        [PXHidden]
		public PXFilter<EP.CRActivityMaint.EPTempData> TempData;

		[PXImport(typeof(EPTimeCard))]
        [PXViewName(Messages.TimeCardSummary)]
		public PXSelect<EPTimeCardSummary, Where<EPTimeCardSummary.timeCardCD, Equal<Current<EPTimeCard.timeCardCD>>>, OrderBy<Asc<EPTimeCardSummary.lineNbr>>> Summary;

		protected Dictionary<string, SummaryRecord> sumList = new Dictionary<string, SummaryRecord>();
		protected Dictionary<string, EPTimeCardSummary> existingSummaryRowsByKeys = new Dictionary<string, EPTimeCardSummary>();
		protected Dictionary<int, EPTimeCardSummary> existingSummaryRowsByLineNbr = new Dictionary<int, EPTimeCardSummary>();


		public virtual IEnumerable summary()
		{
			Stopwatch sw = new Stopwatch();
			Stopwatch sw2 = new Stopwatch();
			sw2.Start();
			

            EPTimeCard document = Document.Current;

            if (PXView.Currents != null)
                foreach (object current in PXView.Currents)
                {
                    var currentDocument = current as EPTimeCard;
                    if (currentDocument != null)
                        document = currentDocument;
                }

            if (document == null)
                return new List<EPTimeCardSummary>();

            if (document.WeekID == null)
                return new List<EPTimeCardSummary>();

            /*
             * List all summary records that are in tthe database.
             * Add new ones based on the new activities added.
             * Remove the ones that are empty - no activities associated.
             * Update existing ones - new activities must be aggregated              
             */

            existingSummaryRowsByKeys.Clear();
			existingSummaryRowsByLineNbr.Clear();
			PXSelectBase<EPTimeCardSummary> select = new PXSelect<EPTimeCardSummary, Where<EPTimeCardSummary.timeCardCD, Equal<Required<EPTimeCard.timeCardCD>>>, OrderBy<Asc<EPTimeCardSummary.lineNbr>>>(this);
	        foreach (EPTimeCardSummary item in select.Select(document.TimeCardCD))
	        {
		        string key = GetSummaryKey(item);
				if (!existingSummaryRowsByKeys.ContainsKey(key))
					existingSummaryRowsByKeys.Add(key, item);
				existingSummaryRowsByLineNbr.Add(item.LineNbr.Value, item);
			}

			
            sumList.Clear();

			//sw.Start();
	        PXResultset<EPTimecardDetail> res = Activities.Select();
			//sw.Stop();
			//Debug.Print("Activities.Select() in {0} ms.", sw.ElapsedMilliseconds);
			sw.Restart();

			foreach (EPTimecardDetail activity in res)
            {
	            if (string.IsNullOrEmpty(activity.EarningTypeID))
		            continue;
                string key = GetSummaryKey(activity);

                SummaryRecord record = null;
	            EPTimeCardSummary summary = null;

	            if (activity.SummaryLineNbr != null)
	            {
		            if (!existingSummaryRowsByLineNbr.TryGetValue(activity.SummaryLineNbr.Value, out summary))
		            {
						existingSummaryRowsByKeys.TryGetValue(key, out summary);
		            }
	            }
	            else
	            {
					existingSummaryRowsByKeys.TryGetValue(key, out summary);
	            }
	            
                if (summary != null)
                {
                    if (sumList.ContainsKey(summary.LineNbr.ToString()))
                    {
                        record = sumList[summary.LineNbr.ToString()];
                    }
                }
                else
                {
                    if (sumList.ContainsKey(key))
                    {
                        record = sumList[key];
                    }
                }
                
                if (activity.SummaryLineNbr != null)
                {
                    if (summary != null)
                    {
                        if (record == null)
                        {
                            record = new SummaryRecord(summary);
                            sumList.Add(record.Summary.LineNbr.ToString(), record);
                        }
                        record.LinkedDetails.Add(activity);
                    }
                    else
                    {
                        Activities.Cache.SetValue<EPTimecardDetail.summaryLineNbr>(activity, null);
                        if (Activities.Cache.GetStatus(activity) == PXEntryStatus.Notchanged)
                            Activities.Cache.SetStatus(activity, PXEntryStatus.Updated);
                        if (record == null)
                        {
                            record = new SummaryRecord(null);
                            sumList.Add(key, record);
                        }
                        record.SummaryKey = key;
                        record.NotLinkedDetails.Add(activity);
                    }
                }
                else
                {
                    if (summary != null)
                    {
                        if (record == null)
                        {
                            record = new SummaryRecord(summary);
                            sumList.Add(record.Summary.LineNbr.ToString(), record);
                        }
                       
                    }
                    else
                    {
                        Activities.Cache.SetValue<EPTimecardDetail.summaryLineNbr>(activity, null);
                        if (Activities.Cache.GetStatus(activity) == PXEntryStatus.Notchanged)
                            Activities.Cache.SetStatus(activity, PXEntryStatus.Updated);
                        if (record == null)
                        {
                            record = new SummaryRecord(null);
                            sumList.Add(key, record);
                        }
                        record.SummaryKey = key;
                    }
                    record.NotLinkedDetails.Add(activity);
                }
            }

			sw.Stop();
			Debug.Print("Summary first block in {0} ms.", sw.ElapsedMilliseconds);
			sw.Restart();


            List<EPTimeCardSummary> list = new List<EPTimeCardSummary>();

			foreach (EPTimeCardSummary item in existingSummaryRowsByLineNbr.Values)
			{
			    if (sumList.ContainsKey(item.LineNbr.ToString()))
			    {
				    bool hasChanges = false;

				    EPTimeCardSummary summary = sumList[item.LineNbr.ToString()].Summary;
				    EPTimeCardSummary keyItem = sumList[item.LineNbr.ToString()].SummariseDetails();
				    if (summary.Mon.GetValueOrDefault() != keyItem.Mon.GetValueOrDefault())
				    {
					    hasChanges = true;
					    summary.Mon = keyItem.Mon;
				    }
				    if (summary.Tue.GetValueOrDefault() != keyItem.Tue.GetValueOrDefault())
				    {
					    hasChanges = true;
					    summary.Tue = keyItem.Tue;
				    }
				    if (summary.Wed.GetValueOrDefault() != keyItem.Wed.GetValueOrDefault())
				    {
					    hasChanges = true;
					    summary.Wed = keyItem.Wed;
				    }
				    if (summary.Thu.GetValueOrDefault() != keyItem.Thu.GetValueOrDefault())
				    {
					    hasChanges = true;
					    summary.Thu = keyItem.Thu;
				    }
				    if (summary.Fri.GetValueOrDefault() != keyItem.Fri.GetValueOrDefault())
				    {
					    hasChanges = true;
					    summary.Fri = keyItem.Fri;
				    }
				    if (summary.Sat.GetValueOrDefault() != keyItem.Sat.GetValueOrDefault())
				    {
					    hasChanges = true;
					    summary.Sat = keyItem.Sat;
				    }
				    if (summary.Sun.GetValueOrDefault() != keyItem.Sun.GetValueOrDefault())
				    {
					    hasChanges = true;
					    summary.Sun = keyItem.Sun;
				    }

				    dontSyncDetails = true;
				    try
				    {
						skipValidation = true;
					    if (hasChanges)
						    Summary.Update(summary);
				    }
				    finally
				    {
					    dontSyncDetails = false;
						skipValidation = false;
				    }
				    list.Add(summary);
			    }
			    else
			    {
				    if (Summary.Cache.GetStatus(item) == PXEntryStatus.Notchanged)
				    {
					    bool hasChanges = false;
						if (item.Mon.GetValueOrDefault() != 0)
						{
							item.Mon = 0;
							hasChanges = true;
						}
						if (item.Tue.GetValueOrDefault() != 0)
						{
							item.Tue = 0;
							hasChanges = true;
						}
						if (item.Wed.GetValueOrDefault() != 0)
						{
							item.Wed = 0;
							hasChanges = true;
						}
						if (item.Thu.GetValueOrDefault() != 0)
						{
							item.Thu = 0;
							hasChanges = true;
						}
						if (item.Fri.GetValueOrDefault() != 0)
						{
							item.Fri = 0;
							hasChanges = true;
						}
						if (item.Sat.GetValueOrDefault() != 0)
						{
							item.Sat = 0;
							hasChanges = true;
						}
						if (item.Sun.GetValueOrDefault() != 0)
						{
							item.Sun = 0;
							hasChanges = true;
						}
					    if (hasChanges)
					    {
							dontSyncDetails = true;
							skipValidation = true;
							try
							{
								Summary.Update(item);
							}
							finally
							{
								dontSyncDetails = false;
								skipValidation = false;
							}
						}
				    }
					list.Add(item);
			    }
            }

			sw.Stop();
			Debug.Print("Summary second block in {0} ms.", sw.ElapsedMilliseconds);
			sw.Restart();

			//Stopwatch sw3 = new Stopwatch();
            foreach (SummaryRecord record in sumList.Values)
            {
                if (record.SummaryKey != null)
                {
					List<EPTimecardDetail> details = record.NotLinkedDetails;
					EPTimeCardSummary summary = null;
					//sw3.Restart();
                    foreach (EPTimecardDetail detail in details)
                    {
	                    if (detail.TimeSpent.GetValueOrDefault() != 0)
	                    {
		                    try
		                    {
			                    skipValidation = true;
			                    summary = AddToSummary(summary, detail);
		                    }
                            catch (PXFieldProcessingException ex)
                            {
                                summary = null;
                                if (ex.FieldName == "ParentNoteID")
                                {                                                                        
                                    Activities.Cache.RaiseExceptionHandling<EPTimecardDetail.parentTaskNoteID>
                                        (detail,
                                        detail.Summary, new PXSetPropertyException(Messages.SummaryTaskNotFound,
                                            PXErrorLevel.RowError));
                                }
                                else
                                {
                                    Activities.Cache.RaiseExceptionHandling<EPTimecardDetail.summary>(detail, detail.Summary, new PXSetPropertyException(Messages.SummarySyncFailed
                                    + Environment.NewLine + ex.MessageNoPrefix, PXErrorLevel.RowError));
                                }
                            }
                            catch (PXSetPropertyException ex)
							{
								summary = null;                                
								Activities.Cache.RaiseExceptionHandling<EPTimecardDetail.summary>(detail, detail.Summary, new PXSetPropertyException(Messages.SummarySyncFailed
									+ Environment.NewLine + ex.MessageNoPrefix, PXErrorLevel.RowError));
							}
		                    finally
		                    {
								skipValidation = false;
		                    }
	                    }
                    }
					//sw3.Stop();
					//Debug.Print("AddToSummary for {0} details in {1} ms.", details.Count, sw3.ElapsedMilliseconds);

                    if (summary != null)
                        list.Add(summary);
                }
            }

			sw.Stop();
			Debug.Print("Summary third block in {0} ms.", sw.ElapsedMilliseconds);
			sw.Restart();

		    foreach (EPTimeCardSummary sr in list)
		    {
                //Debug.WriteLine(sr.ToString());
		    }

			sw2.Stop();
			Debug.Print("summary() in {0} ms.", sw2.ElapsedMilliseconds);

			return list;
		}

		
		[PXViewName(Messages.TimeCardDetail)]
        public PXSelectJoin<EPTimecardDetail,
            InnerJoin<CREmployee, 
				On<CREmployee.userID, Equal<EPTimecardDetail.ownerID>>,
		    LeftJoin<CRActivityLink, 
				On<CRActivityLink.noteID, Equal<EPTimecardDetail.refNoteID>>,
			LeftJoin<CRCase, 
				On<CRCase.noteID, Equal<CRActivityLink.refNoteID>>,
			LeftJoin<AR.Customer, 
				On<AR.Customer.bAccountID, Equal<CRCase.customerID>>,
			LeftJoin<ContractEx, 
				On<ContractEx.contractID, Equal<CRCase.contractID>>,
			LeftJoin<PMProject, 
				On<PMProject.contractID, Equal<EPTimecardDetail.projectID>>>>>>>>,
			Where<CREmployee.bAccountID, Equal<Current<EPTimeCard.employeeID>>,
                And<EPTimecardDetail.weekID, Equal<Current<EPTimeCard.weekId>>,
                And<EPTimecardDetail.trackTime, Equal<True>,
				And<EPTimecardDetail.approvalStatus, NotEqual<ActivityStatusListAttribute.canceled>,
                And<Where<EPTimecardDetail.timeCardCD, IsNull, Or<EPTimecardDetail.timeCardCD, Equal<Current<EPTimeCard.timeCardCD>>>>>>>>>,
            OrderBy<Asc<EPTimecardDetail.date>>> Activities;

		public virtual IEnumerable activities()
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			EPTimeCard document = Document.Current;
			
            if (PXView.Currents != null)
                foreach (object current in PXView.Currents)
                {
                    var currentDocument = current as EPTimeCard;
                    if (currentDocument != null)
                        document = currentDocument;
                }

            if (document == null) 
                yield break;

            if (document.WeekID == null)
                yield break;


			bool showAllActivities = document.IsHold == true && document.IsApproved != true &&
				document.IsRejected != true && document.IsReleased != true;

           if (showAllActivities)
			{
				foreach (PXResult<EPTimecardDetail, CREmployee> row in QSelect(this, Activities.View.BqlSelect))
			    {
                    yield return row;
			    }
			}
			else
			{
				foreach (PXResult<EPTimecardDetail, CREmployee, CRActivityLink, CRCase, AR.Customer, ContractEx, PMProject> row in
                    PXSelectJoin<EPTimecardDetail,
                    InnerJoin<CREmployee, 
						On<CREmployee.userID, Equal<EPTimecardDetail.ownerID>>,
					LeftJoin<CRActivityLink, 
						On<CRActivityLink.noteID, Equal<EPTimecardDetail.refNoteID>>,
					LeftJoin<CRCase, 
						On<CRCase.noteID, Equal<CRActivityLink.refNoteID>>,
					LeftJoin<AR.Customer, 
						On<AR.Customer.bAccountID, Equal<CRCase.customerID>>,
					LeftJoin<ContractEx, 
						On<ContractEx.contractID, Equal<CRCase.contractID>>,
					LeftJoin<PMProject, 
						On<PMProject.contractID, Equal<EPTimecardDetail.projectID>>>>>>>>,
                    Where<EPTimecardDetail.timeCardCD, Equal<Required<EPTimeCard.timeCardCD>>>,
                    OrderBy<Asc<EPTimecardDetail.date>>>.
					Select(this, document.TimeCardCD))
				{
                    yield return row;
				}
			}

			sw.Stop();
			Debug.Print("activities() in {0} ms.", sw.ElapsedMilliseconds);
		}

        [PXCopyPasteHiddenFields(typeof(EPTimeCardItem.noteID), typeof(Note.noteText))]
        public PXSelect<EPTimeCardItem,
            Where<EPTimeCardItem.timeCardCD, Equal<Current<EPTimeCard.timeCardCD>>>,
            OrderBy<Asc<EPTimeCardItem.lineNbr>>> Items;

        public PXSelectJoin<EPTimecardTask,
            InnerJoin<CREmployee, On<CREmployee.userID, Equal<EPTimecardTask.ownerID>>>,
			Where<CREmployee.bAccountID, Equal<Current<EPTimeCard.employeeID>>, And<EPTimecardTask.classID, Equal<CRActivityClass.task>>>> Tasks;

        [PXViewName(Messages.Approval)]
		public EPApprovalAction<EPTimeCard, EPTimeCard.isApproved, EPTimeCard.isRejected> Approval;

        private BqlCommand _approvalCommand;

        protected IEnumerable approval()
		{
			return this.QuickSelect(_approvalCommand);
		}

		#endregion
		
		#region Flags
		/// <summary>
		/// When True activities billable field is not updated when created TC correction.
		/// </summary>
		protected bool isCreateCorrectionFlag = false;
		/// <summary>
		/// When True Summary records are not updated as a result of a detail row update.
		/// </summary>
		protected bool dontSyncSummary = false;
		/// <summary>
		/// When True detail row is not updated when a summary record is modified.
		/// </summary>
		protected bool dontSyncDetails = false;

		#endregion

        

        public TimeCardMaint()
		{
			if (EpSetup.Current.TimeCardNumberingID == null)
				throw new PXSetupNotEnteredException(ErrorMessages.SetupNotEntered, typeof(EPSetup), typeof(EPSetup).Name);
			
            PXUIFieldAttribute.SetEnabled<EPTimecardDetail.overtimeSpent>(Activities.Cache, null, !ShowActivityTime);

            PXDBDateAndTimeAttribute.SetTimeVisible<EPTimecardDetail.date>(Activities.Cache, null, ShowActivityTime);
			
		    bool timeReportingInstalled = PXAccess.FeatureInstalled<CS.FeaturesSet.timeReportingModule>();
            if (!timeReportingInstalled)
            {
                preloadFromTasks.SetVisible(false);
				PXUIFieldAttribute.SetVisible<EPTimeCardSummary.parentNoteID>(Summary.Cache, null, false);
				PXUIFieldAttribute.SetVisible<EPTimecardDetail.approvalStatus>(Activities.Cache, null, false);
                createActivity.SetVisible(false);
            }
			bool projectInstalled = PXAccess.FeatureInstalled<CS.FeaturesSet.projectModule>();
			if (!projectInstalled)
			{
				PXUIFieldAttribute.SetVisible<EPTimecardDetail.approvalStatus>(Activities.Cache, null, false);
			}
			bool contractInstalled = PXAccess.FeatureInstalled<CS.FeaturesSet.contractManagement>();
			if (!contractInstalled)
			{
				PXUIFieldAttribute.SetVisible<ContractEx.contractCD>(Caches[typeof(ContractEx)], null, false);
			}
			bool crmInstalled = PXAccess.FeatureInstalled<CS.FeaturesSet.customerModule>();
			if (!crmInstalled)
			{
				PXUIFieldAttribute.SetVisible<CRCase.caseCD>(Caches[typeof(CRCase)], null, false);
			}
            
            //InitializeApproval:
			var command = Approval.View.BqlSelect.GetType();
			command = BqlCommand.AppendJoin(command,
				typeof(LeftJoin<ApproverEmployee, On<ApproverEmployee.userID, Equal<EPApproval.ownerID>>>));
			command = BqlCommand.AppendJoin(command,
				typeof(LeftJoin<ApprovedByEmployee, On<ApprovedByEmployee.userID, Equal<EPApproval.approvedByID>>>));
			_approvalCommand = BqlCommand.CreateInstance(command);

            var startDate_date_fieldName = typeof(EPTimecardDetail.date).Name + PXDBDateAndTimeAttribute.DATE_FIELD_POSTFIX;
            FieldUpdating.AddHandler(typeof(EPTimecardDetail), startDate_date_fieldName, EPTimecardDetail_StartDate_Date_FieldUpdating);
            FieldUpdated.AddHandler(typeof(EPTimecardDetail), startDate_date_fieldName, EPTimecardDetail_StartDate_Date_FieldUpdated);
            FieldVerifying.AddHandler(typeof(EPTimecardDetail), startDate_date_fieldName, EPTimecardDetail_StartDate_Date_FieldVerifying);
            var startDate_time_fieldName = typeof(EPTimecardDetail.date).Name + PXDBDateAndTimeAttribute.TIME_FIELD_POSTFIX;
            FieldUpdating.AddHandler(typeof(EPTimecardDetail), startDate_time_fieldName, EPTimecardDetail_StartDate_Time_FieldUpdating);
		}

		public override bool CanClipboardCopyPaste() { return false; }

		public void CheckAllowedUser()
		{
			EPEmployee employeeByUserID = PXSelect<EPEmployee, Where<EP.EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>.Select(this);
			if (employeeByUserID == null && System.Web.HttpContext.Current != null && !IsImport && !IsExport && !IsCopyPasteContext && !ProxyIsActive)
			{
					throw new PXException(Messages.MustBeEmployee);
			}
		}

		public static IEnumerable QSelect(PXGraph graph, BqlCommand bqlCommand)
		{
			var view = new PXView(graph, false, bqlCommand);
			var startRow = PXView.StartRow;
			int totalRows = 0;

			Stopwatch sw = new Stopwatch();
			sw.Start();

			var list = view.Select(PXView.Currents, PXView.Parameters, PXView.Searches, PXView.SortColumns, PXView.Descendings, PXView.Filters,
								   ref startRow, PXView.MaximumRows, ref totalRows);

			sw.Stop();
			Debug.Print("activities.QSelect in {0} ms.", sw.ElapsedMilliseconds);

			PXView.StartRow = 0;
			return list;
		}

	    public virtual string GetSummaryKey(EPTimecardDetail activity)
	    {
			return string.Format("{0}.{1}.{2}.{3}.{4}.{5}.{6}", activity.EarningTypeID.ToUpper(),
													   activity.ProjectID.GetValueOrDefault(),
													   activity.ProjectTaskID.GetValueOrDefault(-1),
													   activity.IsBillable.GetValueOrDefault(),
													   activity.ParentTaskNoteID.GetValueOrDefault(Guid.Empty),
													   activity.JobID.GetValueOrDefault(-1),
													   activity.ShiftID.GetValueOrDefault(-1));
	    }

		public virtual string GetSummaryKey(EPTimeCardSummary summary)
		{
			return string.Format("{0}.{1}.{2}.{3}.{4}.{5}.{6}", summary.EarningType.ToUpper(),
													   summary.ProjectID.GetValueOrDefault(),
													   summary.ProjectTaskID.GetValueOrDefault(-1),
													   summary.IsBillable.GetValueOrDefault(),
													   summary.ParentNoteID.GetValueOrDefault(Guid.Empty),
													   summary.JobID.GetValueOrDefault(-1),
													   summary.ShiftID.GetValueOrDefault(-1));
		}

        #region Setup Settings

        /// <summary>
        /// Gets the source for the generated PMTran.AccountID
        /// </summary>
        public string ExpenseAccountSource
        {
            get
            {
                string result = PM.PMExpenseAccountSource.InventoryItem;

                PMSetup setup = PXSelect<PMSetup>.Select(this);
                if (setup != null && !string.IsNullOrEmpty(setup.ExpenseAccountSource))
                {
                    result = setup.ExpenseAccountSource;
                }

                return result;
            }
        }

        public string ExpenseSubMask
        {
            get
            {
                string result = null;

                PMSetup setup = PXSelect<PMSetup>.Select(this);
                if (setup != null && !string.IsNullOrEmpty(setup.ExpenseSubMask))
                {
                    result = setup.ExpenseSubMask;
                }

                return result;
            }
        }

        public string ExpenseAccrualAccountSource
        {
            get
            {
                string result = PM.PMExpenseAccountSource.InventoryItem;

                PMSetup setup = PXSelect<PMSetup>.Select(this);
                if (setup != null && !string.IsNullOrEmpty(setup.ExpenseAccountSource))
                {
                    result = setup.ExpenseAccrualAccountSource;
                }

                return result;
            }
        }

        public string ExpenseAccrualSubMask
        {
            get
            {
                string result = null;

                PMSetup setup = PXSelect<PMSetup>.Select(this);
                if (setup != null && !string.IsNullOrEmpty(setup.ExpenseAccrualSubMask))
                {
                    result = setup.ExpenseAccrualSubMask;
                }

                return result;
            }
        }

        public string ActivityTimeUnit
        {
            get
            {
                string result = EPSetup.Minute;

                EPSetup setup = PXSelect<EPSetup>.Select(this);
                if (setup != null && !string.IsNullOrEmpty(setup.ActivityTimeUnit))
                {
                    result = setup.ActivityTimeUnit;
                }

                return result;
            }
        }

        public string EmployeeRateUnit
        {
            get
            {
                string result = EPSetup.Hour;

                EPSetup setup = PXSelect<EPSetup>.Select(this);
                if (setup != null && !string.IsNullOrEmpty(setup.EmployeeRateUnit))
                {
                    result = setup.EmployeeRateUnit;
                }

                return result;
            }
        }

        public bool ShowActivityTime
        {
            get
            {
                bool result = false;

                EPSetup setup = PXSelect<EPSetup>.Select(this);
                if (setup != null)
                {
                    result = setup.RequireTimes == true;
                }

                return result;
            }
        }
        #endregion

		#region Actions

		public PXAction<EPTimeCard> viewActivity;
		[PXUIField(Visible = false)]
		[PXButton]
		public virtual IEnumerable ViewActivity(PXAdapter adapter)
		{
			if (Activities.Current != null)
			{
				this.Save.Press();

				PXRedirectHelper.TryRedirect(this, Activities.Current, PXRedirectHelper.WindowMode.NewWindow);
			}
			return adapter.Get();
		}

		public PXAction<EPTimeCard> createActivity;
		[PXUIField(DisplayName = Messages.AddActivity)]
		[PXButton(Tooltip = Messages.AddActivityTooltip,
			ImageKey = PX.Web.UI.Sprite.Main.DataEntry)]
		public virtual IEnumerable CreateActivity(PXAdapter adapter)
		{
			var doc = Document.Current;
			if (doc != null)
			{
				Guid? owner = null;
				EPEmployee emp = Employee.Select();
				if (emp != null)
					owner = emp.UserID;

				new ActivityService().CreateActivity(null, EpSetup.Current.DefaultActivityType, owner);
			}
			return adapter.Get();
		}

		public PXAction<EPTimeCard> action;
		[PXUIField(DisplayName = Messages.Actions)]
		[PXButton]
		public virtual IEnumerable Action(PXAdapter adapter)
		{
			return adapter.Get();
		}

		public PXAction<EPTimeCard> submit;
		[PXUIField(DisplayName = Messages.Submit)]
		[PXButton]
		public virtual void Submit()
		{
			if (Document.Current != null)
			{
				List<EPTimecardDetail> details = new List<EPTimecardDetail>();
				PXView detailsView = new PXView(this, false, Activities.View.BqlSelect);
				List<int?> details_laborItemID = new List<int?>();
				bool hasError = false;
				bool hasInactiveProject = false;
				bool laborItemNotSpecified = false;
				EmployeeCostEngine engine = CreateEmployeeCostEngine();
				EPEmployee employee = PXSelect<EPEmployee, Where<EPEmployee.bAccountID, Equal<Required<EPEmployee.bAccountID>>>>.Select(this, Document.Current.EmployeeID);
				foreach (PXResult<EPTimecardDetail, CREmployee, CRActivityLink, CRCase, AR.Customer, ContractEx, PMProject> res in detailsView.SelectMultiBound(new object[] { Document.Current }))
				{
					EPTimecardDetail act = (EPTimecardDetail)res;
					PMProject pr = (PMProject)res;
					CRCase refCase = (CRCase)res;

					int? laborClassID = engine.GetLaborClass(act, employee, (refCase.CaseID != null ? refCase : null) );

					if (laborClassID == null)
					{
						if (act.IsOvertimeCalc == true)
						{
							Activities.Cache.RaiseExceptionHandling<EPTimecardDetail.labourItemID>(act, null, new PXSetPropertyException(Messages.OvertimeLaborClassNotSpecified, PXErrorLevel.RowError));
						}
						else
						{
							Activities.Cache.RaiseExceptionHandling<EPTimecardDetail.labourItemID>(act, null, new PXSetPropertyException(Messages.LaborClassNotSpecified, PXErrorLevel.RowError));
						}

						laborItemNotSpecified = true;
					}
					
					if (act.ApprovalStatus != ActivityStatusAttribute.Completed &&
					    act.ApprovalStatus != ActivityStatusAttribute.Canceled &&
					    act.ApprovalStatus != ActivityStatusAttribute.Approved &&
                        act.ApprovalStatus != ActivityStatusAttribute.Released)
					{
						Activities.Cache.RaiseExceptionHandling<EPTimecardDetail.date>(act, null, new PXSetPropertyException(Messages.ActivityIsNotCompleted, PXErrorLevel.RowError));

						hasError = true;
					}
					
					if (pr.IsActive != true)
					{
						Activities.Cache.RaiseExceptionHandling<EPTimecardDetail.date>(act, null,
							new PXSetPropertyException(Messages.ProjectIsNotActive, PXErrorLevel.RowError, pr.ContractCD));

						hasInactiveProject = true;
					}

					details.Add(act);
					details_laborItemID.Add(laborClassID);
				}

				if (hasError || hasInactiveProject || laborItemNotSpecified)
				{
					throw new PXException((hasError ? PXMessages.LocalizeNoPrefix(Messages.HasOpenActivity) : " ") + (hasInactiveProject ? PXMessages.LocalizeNoPrefix(Messages.HasInactiveProject) : " ") + (laborItemNotSpecified ? PXMessages.LocalizeNoPrefix(Messages.EmptyLabourOrOvertimeItem) : " "));
				}

				RecalculateTotals(Document.Current, details);
				string errorMsg;
				if (!ValidateTotals(Document.Current, out errorMsg))
				{
					if (employee.HoursValidation == HoursValidationOption.Validate)
					{
						throw new PXException(PXMessages.LocalizeNoPrefix(Messages.TimecardIsNotValid) + " " + errorMsg);
					}
				}
				
				for( int i = 0; i < details.Count ; i++)
				{
					bool updated = false;
					if (details[i].LabourItemID != details_laborItemID[i])
					{
						Activities.Cache.SetValue<EPTimecardDetail.labourItemID>(details[i], details_laborItemID[i]);
						updated = true;
					}

					if (details[i].TimeCardCD != Document.Current.TimeCardCD)
					{
						Activities.Cache.SetValue<EPTimecardDetail.timeCardCD>(details[i], Document.Current.TimeCardCD);
						updated = true;
					}

					if (updated && Activities.Cache.GetStatus(details[i]) == PXEntryStatus.Notchanged)
					{
						Activities.Cache.SetStatus(details[i], PXEntryStatus.Updated);
					}
				}

				if (EpSetup.Current.TimeCardAssignmentMapID != null)
				{
					if (Approval.Assign(Document.Current, EpSetup.Current.TimeCardAssignmentMapID, EpSetup.Current.TimeCardAssignmentNotificationID))
						Document.Current.IsApproved = false;
					else
					{
						Document.Current.IsApproved = true;
						PXTrace.WriteWarning("Approval map could not assign the approver for the document. The document is pre-approved.");
					}
				}
				else
				{
					Document.Current.IsApproved = true;
				}
				Document.Current.IsHold = false;
				Document.Cache.SetStatus(Document.Current, PXEntryStatus.Updated);
			}
		}

		public PXAction<EPTimeCard> edit;
		[PXUIField(DisplayName = Messages.PutOnHold)]
		[PXButton]
		public virtual void Edit()
		{
			if (Document.Current != null)
			{
				Document.Current.IsApproved = false;
				Document.Current.IsRejected = false;
				Document.Current.IsHold = true;
				Document.Cache.SetStatus(Document.Current, PXEntryStatus.Updated);

				PXSelectBase<EPApproval> select = new PXSelect<EPApproval, Where<EPApproval.refNoteID, Equal<Required<EPApproval.refNoteID>>>>(this);
				foreach (EPApproval approval in select.Select((Document.Current.NoteID)))
				{
					this.Caches[typeof(EPApproval)].Delete(approval);
				}

				foreach (EPTimecardDetail item in Activities.Select())
				{
					Activities.Cache.SetValue<EPTimecardDetail.timeCardCD>(item, null);
					Activities.Cache.SetStatus(item, PXEntryStatus.Updated);
				}
			}
		}

		public PXAction<EPTimeCard> release;
		[PXUIField(DisplayName = Messages.Release)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.Release)]
		public virtual IEnumerable Release(PXAdapter adapter)
		{
			List<EPTimeCard> list = new List<EPTimeCard>();
			foreach (var res in adapter.Get())
			{
				EPTimeCard doc = PXResult.Unwrap<EPTimeCard>(res);

				if (doc != null && doc.IsReleased != true)
					list.Add(doc);
			}
            
			if (list.Any())
			{
				Save.Press();
				PXLongOperation.StartOperation(this, delegate()
				{
					var releaseGraph = (PM.RegisterEntry)PXGraph.CreateInstance(typeof(PM.RegisterEntry));
					var timecardGraph = PXGraph.CreateInstance<TimeCardMaint>();
					releaseGraph.Clear();

					foreach (EPTimeCard item in list)
					{						
				        releaseGraph.Clear();
                        
			            PMSetup setup = PXSelect<PMSetup>.Select(releaseGraph);
                        if (setup == null)
                        {
                            //Setup may be null because the PM module is not enabled. and yet we need to be able to generate PMTran for the Contract billing.
                            releaseGraph.Setup.Insert();
                        }
                
				        using (PXTransactionScope ts = new PXTransactionScope())
				        {
							timecardGraph.Clear();

                            //as setcurrent doesn't place the item into the cache, set status is raised
                            timecardGraph.Document.Current = item;
                            timecardGraph.Document.Cache.SetStatus(item, PXEntryStatus.Notchanged);

					        if (string.IsNullOrEmpty(item.OrigTimeCardCD))
					        {
								timecardGraph.ProcessRegularTimecard(releaseGraph, item);
					        }
					        else
					        {
								timecardGraph.ProcessCorrectingTimecard(releaseGraph, item);
					        }

					        releaseGraph.Save.Press();

                            item.Status = EPTimeCard.ReleasedStatus;
					        item.IsReleased = true;
							timecardGraph.Document.Update(item);

							timecardGraph.Save.Press();

					        ts.Complete();
				        }

				        if (EpSetup.Current.AutomaticReleasePM == true)
				        {
					        PX.Objects.PM.RegisterRelease.Release(releaseGraph.Document.Current);
				        }
			        }
				});
			}
			else throw new PXException(Messages.AlreadyReleased);

			return list;
		}

        public PXAction<EPTimeCard> correct;
		[PXUIField(DisplayName = Messages.Correct)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.Release)]
		public virtual IEnumerable Correct(PXAdapter adapter)
		{
            if (Document.Current != null)
            {
				EPTimeCard source = GetLastCorrection(Document.Current);

	            if (source.IsReleased != true)
					return new EPTimeCard[] { source };


				EPTimeCard newCard = (EPTimeCard)Document.Cache.CreateInstance();
                newCard.WeekID = source.WeekID;
                newCard.OrigTimeCardCD = source.TimeCardCD;
				newCard = Document.Insert(newCard);
	            newCard.EmployeeID = source.EmployeeID;

				bool failed = false;
				foreach (PXResult<EPTimecardDetail> actRow in Activities.View.SelectMultiBound(new object[] { source }))
                {
                    EPTimecardDetail act = (EPTimecardDetail)actRow;

                    EPTimecardDetail newActivity = PXCache<EPTimecardDetail>.CreateCopy(act);
                    newActivity.Released = false;
					newActivity.Billed = false;
					newActivity.NoteID = null;
                    newActivity.TimeCardCD = null;
                    newActivity.TimeSheetCD = null;
					newActivity.OrigNoteID = act.NoteID; //relation between the original activity and the corrected one.
                    newActivity.Date = act.Date;
                    newActivity.Billed = false;
                    newActivity.SummaryLineNbr = null;
	                newActivity.NoteID = null;
	                isCreateCorrectionFlag = true;
					try
					{
						newActivity = Activities.Insert(newActivity);
					}
					catch (PXSetPropertyException ex)
					{
						failed = true;
						Activities.Cache.RaiseExceptionHandling<EPTimecardDetail.summary>(act, act.Summary, new PXSetPropertyException(ex.MessageNoPrefix, PXErrorLevel.RowError));
						continue;
					}
	                newActivity.TrackTime = act.TrackTime; //copy as is.
					isCreateCorrectionFlag = false;
	                newActivity.ApprovalStatus = ActivityStatusAttribute.Completed;
                    newActivity.RefNoteID = act.RefNoteID;

					PXNoteAttribute.CopyNoteAndFiles(Activities.Cache, act, Activities.Cache, newActivity);

                    Activities.Cache.SetValue<EPTimecardDetail.isCorrected>(act, true);
                    Activities.Cache.SetStatus(act, PXEntryStatus.Updated);
                }

				if (failed)
				{
					throw new PXException(Messages.FailedToCreateCorrectionTC);
				}

                foreach (EPTimeCardItem item in Items.View.SelectMultiBound(new object[] { source }))
                {
                    EPTimeCardItem record = Items.Insert();
                    record.ProjectID = item.ProjectID;
                    record.TaskID = item.TaskID;
                    record.Description = item.Description;
                    record.InventoryID = item.InventoryID;
                    record.UOM = item.UOM;
                    record.Mon = item.Mon;
                    record.Tue = item.Tue;
                    record.Wed = item.Wed;
                    record.Thu = item.Thu;
                    record.Fri = item.Fri;
                    record.Sat = item.Sat;
                    record.Sun = item.Sun;
                    record.OrigLineNbr = item.LineNbr;//relation between the original activity and the corrected one.
                }


                return new EPTimeCard[] { newCard };
            }

		    return adapter.Get();
		}

        
        public PXAction<EPTimeCard> preloadFromTasks;
        [PXUIField(DisplayName = Messages.PreloadFromTasks)]
        [PXButton(Tooltip = Messages.PreloadFromTasksTooltip)]
		public virtual void PreloadFromTasks()
        {
            if (Tasks.AskExt() == WebDialogResult.OK)
            {
                foreach (EPTimecardTask task in Tasks.Cache.Updated)
                {
                    if (task.Selected == true)
                    {
						bool alreadyExists = false;
						foreach (EPTimeCardSummary existing in Summary.Select())
						{
							if (existing.ProjectID == task.ProjectID && existing.ProjectTaskID == task.ProjectTaskID 
								&& existing.JobID == task.JobID && existing.ShiftID == task.ShiftID)
							{
								alreadyExists = true;
								break;
							}
						}

						if (alreadyExists)
							continue;

						EPTimeCardSummary summary = (EPTimeCardSummary)Summary.Cache.CreateInstance();
						summary.ParentNoteID = task.NoteID;
                        summary.ProjectID = task.ProjectID;
                        summary.ProjectTaskID = task.ProjectTaskID;
                        summary.IsBillable = task.IsBillable;
                        summary.Description = task.Subject;
						summary.JobID = task.JobID;
						summary.ShiftID = task.ShiftID;

                        summary = Summary.Insert(summary);
                    }
                }

				if (Document.Cache.GetStatus(Document.Current) != PXEntryStatus.Inserted)
					this.Save.Press();
            }
        }

        public PXAction<EPTimeCard> preloadFromPreviousTimecard;
        [PXUIField(DisplayName = Messages.PreloadFromPreviousTimecard)]
		[PXButton(Tooltip = Messages.PreloadFromPreviousTimecardTooltip)]
		public virtual void PreloadFromPreviousTimecard()
        {
            if (Document.Current == null)
                return;

            if (Document.Current.WeekID == null)
                return;

            EPEmployee employee =
                PXSelect<EPEmployee, Where<EPEmployee.bAccountID, Equal<Current<EPTimeCard.employeeID>>>>.Select(this);

            if (employee == null)
                return;

            EPTimeCard previous = PXSelect<EPTimeCard, Where<EPTimeCard.employeeID, Equal<Current<EPTimeCard.employeeID>>, And<EPTimeCard.weekId, Equal<Required<EPTimeCard.weekId>>>>>.Select(this, Document.Current.WeekID.Value - 1);
            if (previous == null)
                return;

            PXSelectBase<EPTimeCardSummary> summarySelect = new PXSelect<EPTimeCardSummary, Where<EPTimeCardSummary.timeCardCD, Equal<Required<EPTimeCardSummary.timeCardCD>>>>(this);
            foreach (EPTimeCardSummary item in summarySelect.Select(previous.TimeCardCD))
            {
                if (item.EarningType != EpSetup.Current.HolidaysType &&
                    item.EarningType != EpSetup.Current.VacationsType)
                {

					bool alreadyExists = false;
					foreach (EPTimeCardSummary existing in Summary.Select())
					{
						if (existing.ProjectID == item.ProjectID
							&& existing.ProjectTaskID == item.ProjectTaskID 
							&& existing.JobID == item.JobID
							&& existing.ShiftID == item.ShiftID
							&& existing.EarningType == item.EarningType)
						{
							alreadyExists = true;
							break;
						}
					}

					if (alreadyExists)
						continue;

					EPTimeCardSummary summary = (EPTimeCardSummary)PXCache<EPTimeCardSummary>.CreateCopy(item);
                    summary.TimeCardCD = null;
                    summary.Description = null;
                    summary.Mon = null;
                    summary.Tue = null;
                    summary.Wed = null;
                    summary.Thu = null;
                    summary.Fri = null;
                    summary.Sat = null;
                    summary.Sun = null;

                    Summary.Insert(summary);
                }
            }

			if (Document.Cache.GetStatus(Document.Current) != PXEntryStatus.Inserted)
				this.Save.Press();
        }

        public PXAction<EPTimeCard> preloadHolidays;
        [PXUIField(DisplayName = Messages.PreloadHolidays)]
        [PXButton(Tooltip = Messages.PreloadHolidays)]
		public virtual void PreloadHolidays()
        {
            if (Document.Current == null)
                return;

            if (Document.Current.WeekStartDate == null)
                return;

            EPEmployee employee =
                PXSelect<EPEmployee, Where<EPEmployee.bAccountID, Equal<Current<EPTimeCard.employeeID>>>>.Select(this);

            if (employee == null)
                return;

			EPEmployeeRate rate = CreateEmployeeCostEngine().GetEmployeeRate(Document.Current.EmployeeID, Document.Current.WeekStartDate);
			if (rate == null || rate.RegularHours == null)
                return;

			int minutPerDay = (int)(rate.RegularHours.Value * 60 / 5);

            DateTime firstDay = Document.Current.WeekStartDate.Value;
            if (firstDay.DayOfWeek == DayOfWeek.Sunday)
                firstDay = firstDay.AddDays(1);
            
            EPTimeCardSummary summary = new EPTimeCardSummary();
            summary.EarningType = EpSetup.Current.HolidaysType;
            if (!CalendarHelper.IsWorkDay(this, employee.CalendarID, firstDay))
            {
                summary.Mon = minutPerDay;
            }
            if (!CalendarHelper.IsWorkDay(this, employee.CalendarID, firstDay.AddDays(1)))
            {
                summary.Tue = minutPerDay;
            }
            if (!CalendarHelper.IsWorkDay(this, employee.CalendarID, firstDay.AddDays(2)))
            {
                summary.Wed = minutPerDay;
            }
            if (!CalendarHelper.IsWorkDay(this, employee.CalendarID, firstDay.AddDays(3)))
            {
                summary.Thu = minutPerDay;
            }
            if (!CalendarHelper.IsWorkDay(this, employee.CalendarID, firstDay.AddDays(4)))
            {
				summary.Fri = minutPerDay;
            }
           
            summary.IsBillable = false;
            summary.Description = "Holiday";

			if (summary.GetTimeTotal() > 0)
                Summary.Insert(summary);

			if (Document.Cache.GetStatus(Document.Current) != PXEntryStatus.Inserted)
				this.Save.Press();
        }

        public PXAction<EPTimeCard> normalizeTimecard;
        [PXUIField(DisplayName = Messages.NormalizeTimecard)]
		[PXButton(Tooltip = Messages.NormalizeTimecard)]
		public virtual void NormalizeTimecard()
        {
            if (Document.Current == null)
                return;
            
            if (!Summary.Cache.AllowInsert)
                return;

			EPEmployeeRate rate = CreateEmployeeCostEngine().GetEmployeeRate(Document.Current.EmployeeID, Document.Current.WeekStartDate);
			if (rate == null || rate.RegularHours == null)
                return;

			int minutesPerday = (int)(rate.RegularHours.Value * 60 / 5);

            int monMinutes = 0;
			int tueMinutes = 0;
			int wedMinutes = 0;
			int thuMinutes = 0;
			int friMinutes = 0;
			int satMinutes = 0;
			int sunMinutes = 0;

	        int? nonProjectID = PM.ProjectDefaultAttribute.NonProject(this);
	        foreach (EPTimeCardSummary item in Summary.Select())
	        {
		        if (item.IsBillable != true && item.ProjectID == nonProjectID &&
		            item.EarningType == EpSetup.Current.RegularHoursType)
			        continue;

		        monMinutes += item.Mon.GetValueOrDefault();
				tueMinutes += item.Tue.GetValueOrDefault();
				wedMinutes += item.Wed.GetValueOrDefault();
				thuMinutes += item.Thu.GetValueOrDefault();
				friMinutes += item.Fri.GetValueOrDefault();
				satMinutes += item.Sat.GetValueOrDefault();
				sunMinutes += item.Sun.GetValueOrDefault();
	        }

			int delta = (int)(rate.RegularHours.GetValueOrDefault() * 60) - (monMinutes + tueMinutes + wedMinutes + thuMinutes + friMinutes + satMinutes + sunMinutes);

			if (delta <= 0)
				return;


            EPTimeCardSummary summary = PXSelect<EPTimeCardSummary, Where<EPTimeCardSummary.timeCardCD, Equal<Current<EPTimeCard.timeCardCD>>,
                And<EPTimeCardSummary.earningType, Equal<Required<EPTimeCardSummary.earningType>>,
                And<EPTimeCardSummary.projectID, Equal<Required<EPTimeCardSummary.projectID>>,
				And<EPTimeCardSummary.isBillable, Equal<False>>>>>>.Select(this, EpSetup.Current.RegularHoursType, nonProjectID);

            if (summary == null)
            {
				summary = (EPTimeCardSummary)Summary.Cache.CreateInstance();
                summary.ProjectID = PM.ProjectDefaultAttribute.NonProject(this);
	            summary.ProjectTaskID = null;
                summary.IsBillable = false;
                summary.Description = "Normalization";
                summary = Summary.Insert(summary);
            }

			int monQuota = minutesPerday - monMinutes;
	        if (monQuota > 0)
	        {
		        summary.Mon = summary.Mon.GetValueOrDefault() + delta < monQuota ? delta : monQuota;
		        delta = delta - monQuota;
	        }

	        if (delta >= 0)
            {
				int tueQuota = minutesPerday - tueMinutes;
	            if (tueQuota > 0)
	            {
		            summary.Tue = summary.Tue.GetValueOrDefault() + delta < tueQuota ? delta : tueQuota;
		            delta = delta - tueQuota;
	            }
            }

            if (delta >= 0)
            {
				int wedQuota = minutesPerday - wedMinutes;
	            if (wedQuota > 0)
	            {
		            summary.Wed = summary.Wed.GetValueOrDefault() + delta < wedQuota ? delta : wedQuota;
		            delta = delta - wedQuota;
	            }
            }

            if (delta >= 0)
            {
				int thuQuota = minutesPerday - thuMinutes;
	            if (thuQuota > 0)
	            {
		            summary.Thu = summary.Thu.GetValueOrDefault() + delta < thuQuota ? delta : thuQuota;
		            delta = delta - thuQuota;
	            }
            }

            if (delta >= 0)
            {
				int friQuota = minutesPerday - friMinutes;
	            if (friQuota > 0)
	            {
		            summary.Fri = summary.Fri.GetValueOrDefault() + delta < friQuota ? delta : friQuota;
		            delta = delta - friQuota;
	            }
            }

			
            //by this time delta should be less then equal to zero
			Debug.Assert(delta <= 0);

            Summary.Update(summary);
	        
			if (Document.Cache.GetStatus(Document.Current) != PXEntryStatus.Inserted)
				this.Save.Press();
        }

        public PXAction<EPTimeCard> View;
        [PXUIField(DisplayName = Messages.View)]
        [PXButton()]
        public virtual IEnumerable view(PXAdapter adapter)
        {
            var row = Activities.Current;
            if (row != null)
            {
				if (Document.Cache.GetStatus(Document.Current) == PXEntryStatus.Inserted)
				{
					throw new PXException(Messages.TimecardMustBeSaved);
				}
	            PXAutomation.GetStep(this,
													  new object[] { Views[AutomationView].Cache.Current },
													  BqlCommand.CreateInstance(typeof(Select<>), Views[AutomationView].Cache.GetItemType())
										            );

				var result = new List<object>(Save.Press(adapter).Cast<object>());
                PXRedirectHelper.TryRedirect(this, row, PXRedirectHelper.WindowMode.NewWindow);
				return result;

            }
            return adapter.Get();
        }

		public PXAction<EPTimeCard> viewPMTran;
		[PXUIField(DisplayName = PM.Messages.ViewTransactions, FieldClass = ProjectAttribute.DimensionName)]
	    [PXButton()]
		public virtual IEnumerable ViewPMTran(PXAdapter adapter)
	    {
			if (Document.Current != null)
			{
				RegisterEntry graph = PXGraph.CreateInstance<RegisterEntry>();
				graph.Clear();
				PMRegister registr = PXSelect<PMRegister
					, Where<PMRegister.origDocType, Equal<PMOrigDocType.timeCard>
						, And<PMRegister.origDocNbr, Equal<Current<EPTimeCard.timeCardCD>>>>>.SelectSingleBound(this, new object[] { Document });
				if (registr == null)
				{
					adapter.View.Ask(Messages.TransactionNotExists, MessageButtons.OK);
				}
				else
				{
					graph.Document.Current = registr;
					throw new PXRedirectRequiredException(graph, PM.Messages.ViewTransactions);
            }
			} 
            return adapter.Get();
        }

        #endregion

        #region Event Handlers

        protected virtual void EPTimecardDetail_EarningTypeID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
	    {
			EPTimecardDetail row = (EPTimecardDetail)e.Row;
			if (row != null && e.NewValue == null)
		    {
			    e.Cancel = true;
				throw new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(EPTimecardDetail.earningTypeID).Name);
		    }
	    }

		protected virtual void EPTimeCardSummary_EarningType_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
	    {
			EPTimeCardSummary row = (EPTimeCardSummary)e.Row;
			if (row != null && e.NewValue == null)
		    {
			    e.Cancel = true;
				throw new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(EPTimeCardSummary.earningType).Name);
		    }
	    }

		
	    protected virtual void EPTimeCard_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
            EPTimeCard row = e.Row as EPTimeCard;
			if (row == null) return;

		    if (row.WeekID == null)
		    {
			    try
			    {
				    row.WeekID = GetNextWeekID(row.EmployeeID);
			    }
			    catch (PXException exception)
			    {
				    row.WeekID = null;
					sender.RaiseExceptionHandling<EPTimeCard.weekId>(row, null, exception);
			    }
		    }
        }

		protected virtual void EPTimeCard_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			var row = e.Row as EPTimeCard;
			var oldRow = e.OldRow as EPTimeCard;
			if (row == null || oldRow == null) return;

			var employeeChanged = row.EmployeeID != oldRow.EmployeeID;
			if (row.WeekID == null || employeeChanged)
			{
				try
				{
					row.WeekID = GetNextWeekID(row.EmployeeID);
				}
				catch (PXException exception)
				{
					row.WeekID = null;
					sender.RaiseExceptionHandling<EPTimeCard.weekId>(row, null, exception);
				}
			}

			if (employeeChanged)
			{
				Items.Cache.Clear();
				Activities.Cache.Clear();
				Summary.Cache.Clear();
				this.Clear(PXClearOption.ClearQueriesOnly);
			}
        }

	    protected virtual void EPTimeCard_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
	    {
			var row = e.Row as EPTimeCard;
			if (row == null) return;

			foreach (EPTimeCardSummary summary in Summary.Select())
			{
				try
				{
					dontSyncDetails = true;
					Summary.Delete(summary);
				}
				finally
				{
					dontSyncDetails = false;
				}
			}

			try
			{
				dontSyncSummary = true;
				PXSelectBase<EPTimecardDetail> selectDetail = new PXSelectJoin<EPTimecardDetail,
					InnerJoin<CREmployee, On<CREmployee.userID, Equal<EPTimecardDetail.ownerID>>>,
					Where<CREmployee.bAccountID, Equal<Current<EPTimeCard.employeeID>>, 
					And<EPTimecardDetail.weekID, Equal<Current<EPTimeCard.weekId>>, 
					And<EPTimecardDetail.released, NotEqual<True>, 
					And<EPTimecardDetail.isCorrected, NotEqual<True>, 
					And2<Where<EPTimecardDetail.summaryLineNbr, IsNotNull, Or<EPTimecardDetail.origNoteID, IsNotNull>>
									, And<Where<EPTimecardDetail.timeCardCD, IsNull, Or<EPTimecardDetail.timeCardCD, Equal<Current<EPTimeCard.timeCardCD>>>>>>>>>>>(this);

				foreach (PXResult<EPTimecardDetail, CREmployee> res in selectDetail.Select())
				{
					EPTimecardDetail activity = (EPTimecardDetail)res;
					if (activity.OrigNoteID != null)
					{
						EPTimecardDetail originActivity = PXSelect<EPTimecardDetail>.Search<EPTimecardDetail.noteID>(this, activity.OrigNoteID);
						if (originActivity != null)
						{
							originActivity.IsCorrected = false;
							Activities.Update(originActivity);
						}
					}
					Activities.Delete(activity);
				}

			}
            finally
            {
				dontSyncSummary = false;
			}

		}
		
	    protected virtual void EPTimeCard_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			EPTimeCard row = (EPTimeCard)e.Row;
            EPTimeCard timeCard = PXSelect<EPTimeCard
								, Where<
									EPTimeCard.employeeID, Equal<Current<EPTimeCard.employeeID>>
										, And<EPTimeCard.weekId, Greater<Current<EPTimeCard.weekId>>
											, And<Where<EPTimeCard.timeCardCD, Equal<Current<EPTimeCard.origTimeCardCD>>, Or<Current<EPTimeCard.origTimeCardCD>, IsNull>>>
										>
									>
								>.SelectWindowed(this, 0, 1);
			if (timeCard != null && timeCard.TimeCardCD != row.OrigTimeCardCD)
            {
                throw new PXException(Messages.TimeCardNoDelete);
            }

		}

        protected virtual void EPTimeCard_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			var row = e.Row as EPTimeCard;
			if (row == null) return;

			var isSubordinatedEmployee = row.EmployeeID == null ||
				PXSubordinateSelectorAttribute.IsSubordinated(this, row.EmployeeID);

			if (!isSubordinatedEmployee)
			{
				EPEmployee wingmen = PXSelect<EPEmployee, Where<EPEmployee.bAccountID, WingmanUser<Current<AccessInfo.userID>>>>.SelectSingleBound(this, null);
				isSubordinatedEmployee = wingmen != null;
	        }

	        if (!isSubordinatedEmployee)
			{
				PXUIFieldAttribute.SetEnabled(sender, row, false);
				PXUIFieldAttribute.SetEnabled<EPTimeCard.timeCardCD>(sender, row, true);
			}

	        EPEmployee employee = PXSelect<EPEmployee, Where<EPEmployee.bAccountID, Equal<Current<EPTimeCard.employeeID>>>>.Select(this);
			if (employee != null)
			{
				PXTimeZoneInfo currentTimeZone = LocaleInfo.GetTimeZone();

				UserPreferences pref = PXSelect<UserPreferences, Where<UserPreferences.userID, Equal<Required<UserPreferences.userID>>>>.Select(sender.Graph, employee.UserID);
				if (pref != null && !string.IsNullOrEmpty(pref.TimeZone) && currentTimeZone.Id != pref.TimeZone)
				{
					PXTimeZoneInfo empTimeZone = PXTimeZoneInfo.FindSystemTimeZoneById(pref.TimeZone);
					PXUIFieldAttribute.SetWarning<EPTimeCard.employeeID>(sender, e.Row, Messages.TimeInTimezoneOfEmployee + empTimeZone.DisplayName);
				}
				else if (employee.CalendarID != null)
				{
					CSCalendar cal = PXSelect<CSCalendar, Where<CSCalendar.calendarID, Equal<Required<CSCalendar.calendarID>>>>.Select(this, employee.CalendarID);
					if (cal != null && !string.IsNullOrEmpty(cal.TimeZone) && currentTimeZone.Id != cal.TimeZone)
					{
						PXTimeZoneInfo empTimeZone = PXTimeZoneInfo.FindSystemTimeZoneById(cal.TimeZone);
							PXUIFieldAttribute.SetWarning<EPTimeCard.employeeID>(sender, e.Row, Messages.TimeInTimezoneOfEmployee + empTimeZone.DisplayName);
					}
				}
			}
			

			Document.Cache.AllowDelete = isSubordinatedEmployee && row.IsReleased != true && row.Status == EPTimeCard.HoldStatus;

			var allowEdit = isSubordinatedEmployee &&
				row.IsHold == true && row.IsApproved != true &&
				row.IsRejected != true && row.IsReleased != true &&
				row.WeekID != null;

		    Activities.Cache.AllowInsert = allowEdit;
			Activities.Cache.AllowUpdate = allowEdit;
			Activities.Cache.AllowDelete = allowEdit;
            Summary.Cache.AllowInsert = allowEdit;
            Summary.Cache.AllowUpdate = allowEdit;
            Summary.Cache.AllowDelete = allowEdit;
            Items.Cache.AllowInsert = allowEdit;
            Items.Cache.AllowUpdate = allowEdit;
            Items.Cache.AllowDelete = allowEdit;
            preloadFromTasks.SetEnabled(allowEdit);
            preloadFromPreviousTimecard.SetEnabled(allowEdit);
            preloadHolidays.SetEnabled(allowEdit);
            normalizeTimecard.SetEnabled(allowEdit);
			viewPMTran.SetEnabled(row.IsReleased == true);

			var isInserted = sender.GetStatus(row) == PXEntryStatus.Inserted;
			createActivity.SetEnabled(allowEdit && !isInserted);
			submit.SetEnabled(!isInserted);
			
            
            if (row.EmployeeID != null)
            {
				if (Summary.Select().Count > 0 && Document.Cache.GetStatus(row) != PXEntryStatus.Inserted)
                {
                    PXUIFieldAttribute.SetEnabled<EPTimeCard.employeeID>(sender, row, false);
                }
                else
                {
					PXUIFieldAttribute.SetEnabled<EPTimeCard.employeeID>(sender, row, allowEdit && row.OrigTimeCardCD == null);
                }
            }

			var isFirst = IsFirstTimeCard(row.EmployeeID);
			PXUIFieldAttribute.SetEnabled<EPTimeCard.weekId>(sender, row, isFirst && allowEdit);

		    RecalculateTotals(row);
            string errorMsg;
		    ValidateTotals(row, out errorMsg);

            row.TimecardType = string.IsNullOrEmpty(row.OrigTimeCardCD) ? "N" : "C";
            if (row.IsReleased == true)
            {
                EPTimeCard correction = PXSelect<EPTimeCard, Where<EPTimeCard.origTimeCardCD, Equal<Required<EPTimeCard.origTimeCardCD>>>>.Select(this, row.TimeCardCD);
                if (correction != null)
                {
                    row.TimecardType = "D";
                }
            }

	        if (row.WeekID != null)
	        {
				PX.Objects.EP.PXWeekSelector2Attribute.WeekInfo weekInfo = PXWeekSelector2Attribute.GetWeekInfo(this, row.WeekID.Value);
				PXUIFieldAttribute.SetEnabled<EPTimeCardSummary.mon>(Summary.Cache, null, weekInfo.Mon.Enabled);
		        PXUIFieldAttribute.SetEnabled<EPTimeCardSummary.tue>(Summary.Cache, null, weekInfo.Tue.Enabled);
		        PXUIFieldAttribute.SetEnabled<EPTimeCardSummary.wed>(Summary.Cache, null, weekInfo.Wed.Enabled);
		        PXUIFieldAttribute.SetEnabled<EPTimeCardSummary.thu>(Summary.Cache, null, weekInfo.Thu.Enabled);
		        PXUIFieldAttribute.SetEnabled<EPTimeCardSummary.fri>(Summary.Cache, null, weekInfo.Fri.Enabled);
		        PXUIFieldAttribute.SetEnabled<EPTimeCardSummary.sat>(Summary.Cache, null, weekInfo.Sat.Enabled);
		        PXUIFieldAttribute.SetEnabled<EPTimeCardSummary.sun>(Summary.Cache, null, weekInfo.Sun.Enabled);
		        PXUIFieldAttribute.SetEnabled<EPTimeCardItem.mon>(Items.Cache, null, weekInfo.Mon.Enabled);
		        PXUIFieldAttribute.SetEnabled<EPTimeCardItem.tue>(Items.Cache, null, weekInfo.Tue.Enabled);
		        PXUIFieldAttribute.SetEnabled<EPTimeCardItem.wed>(Items.Cache, null, weekInfo.Wed.Enabled);
		        PXUIFieldAttribute.SetEnabled<EPTimeCardItem.thu>(Items.Cache, null, weekInfo.Thu.Enabled);
		        PXUIFieldAttribute.SetEnabled<EPTimeCardItem.fri>(Items.Cache, null, weekInfo.Fri.Enabled);
		        PXUIFieldAttribute.SetEnabled<EPTimeCardItem.sat>(Items.Cache, null, weekInfo.Sat.Enabled);
		        PXUIFieldAttribute.SetEnabled<EPTimeCardItem.sun>(Items.Cache, null, weekInfo.Sun.Enabled);
	        }
		}

		protected virtual void EPTimeCard_EmployeeID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			EPTimeCard row = e.Row as EPTimeCard;
			if (row != null && row.WeekID != null)
			{
				EPTimeCard futureTimeCard = PXSelect<EPTimeCard, Where<EPTimeCard.employeeID, Equal<Current<EPTimeCard.employeeID>>, And<EPTimeCard.weekId, Greater<Current<EPTimeCard.weekId>>>>>.Select(this);
				if (futureTimeCard != null)
				{
					throw new PXSetPropertyException(Messages.TimeCardInFutureExists);
				}

			}
		}

        protected virtual void EPTimeCard_IsApproved_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            EPTimeCard row = e.Row as EPTimeCard;
            if (row == null) return;

			if ((bool?)e.NewValue == true)
            {
                //all non zero activities must be approved before timecard can be approved.
                PXView detailsView = new PXView(this, false, Activities.View.BqlSelect);
				foreach (PXResult<EPTimecardDetail, CREmployee> res in detailsView.SelectMultiBound(new object[] { row }))
                {
					EPTimecardDetail act = (EPTimecardDetail)res;

                    if (act.Released == true)
                        continue;

					if ((act.TimeSpent.GetValueOrDefault() != 0 || act.TimeBillable.GetValueOrDefault() != 0) && 
						act.ApprovalStatus != ActivityStatusListAttribute.Completed &&
						act.ApprovalStatus != ActivityStatusListAttribute.Approved &&
						act.ApprovalStatus != ActivityStatusListAttribute.Canceled)
                    {
                        e.Cancel = true;

                        Activities.Cache.RaiseExceptionHandling<EPTimecardDetail.timeSpent>(act, act.TimeSpent,
                                                                                            new PXSetPropertyException(
                                                                                                Messages
                                                                                                    .ActivityIsNotApproved,
                                                                                                PXErrorLevel.RowError));

                        throw new PXException(Messages.OneOrMoreActivitiesAreNotApproved);
                    }
                }
            }
        }


		protected virtual void EPTimeCardSummary_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			EPTimeCardSummary row = e.Row as EPTimeCardSummary;
			if (row == null) return;

			if (dontSyncDetails) return;

			string key = GetSummaryKey(row);
			SummaryRecord rec;
			SummaryRecord.SummaryRecordInfo info = new SummaryRecord.SummaryRecordInfo();
			if (sumList.TryGetValue(row.LineNbr.ToString(), out rec))
			{
				info = rec.GetInfo();
			}
			else if (sumList.TryGetValue(key, out rec))
			{
				info = rec.GetInfo();
			}
			
			bool isEnabled = true;

			if (row.ProjectID != null)
			{
				isEnabled = !info.HasManualDetails;
			}
			
			PXUIFieldAttribute.SetEnabled<EPTimeCardSummary.earningType>(sender, row, isEnabled);
            PXUIFieldAttribute.SetEnabled<EPTimeCardSummary.parentNoteID>(sender, row, isEnabled);
            PXUIFieldAttribute.SetEnabled<EPTimeCardSummary.projectID>(sender, row, isEnabled);
			PXUIFieldAttribute.SetEnabled<EPTimeCardSummary.projectTaskID>(sender, row, isEnabled);
			PXUIFieldAttribute.SetEnabled<EPTimeCardSummary.isBillable>(sender, row, isEnabled && !ProjectDefaultAttribute.IsNonProject(this, row.ProjectID));

			if (info.ApprovalRequired)
			{
				if (info.HasCompleted || info.HasOpen)
					row.ApprovalStatus = ApprovalStatusListAttribute.PendingApproval;
				else if (info.HasApproved && info.HasRejected)
					row.ApprovalStatus = ApprovalStatusListAttribute.PartiallyApprove;
				else if (info.HasApproved)
					row.ApprovalStatus = ApprovalStatusListAttribute.Approved;
				else if (info.HasRejected)
					row.ApprovalStatus = ApprovalStatusListAttribute.Rejected;
				else
					row.ApprovalStatus = ApprovalStatusListAttribute.NotRequired;
			}
			else
				row.ApprovalStatus = ApprovalStatusListAttribute.NotRequired;

		}

        protected virtual void EPTimeCardSummary_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
        {
            EPTimeCardSummary row = e.Row as EPTimeCardSummary;
            if (row == null) return;

            EPEarningType earningType = PXSelect<EPEarningType, Where<EPEarningType.typeCD, Equal<Required<EPEarningType.typeCD>>>>.Select(this, row.EarningType);
            if (earningType != null)
            {
                row.EarningType = earningType.TypeCD;
                if (row.IsBillable == null)
                    row.IsBillable = earningType.isBillable == true;

                if (earningType.ProjectID != null && row.ProjectID == null &&
					PXSelectorAttribute.Select(sender, e.Row, sender.GetField(typeof(EPTimeCardSummary.projectID)), earningType.ProjectID) != null //available in selector
					)
                {
	               row.ProjectID = earningType.ProjectID;
                }
				
				if (earningType.TaskID != null && row.ProjectTaskID == null && !ProjectDefaultAttribute.IsNonProject(this, row.ProjectID))
				{
					PMTask defTask = PXSelectorAttribute.Select(sender, e.Row, sender.GetField(typeof(EPTimeCardSummary.projectTaskID)), earningType.TaskID) as PMTask;
					if (defTask != null && defTask.VisibleInEP == true)
					{
						row.ProjectTaskID = earningType.TaskID;
					}
				}

				
            }

	        if (row.ProjectID == null && e.ExternalCall)
	        {
		        EPTimeCardSummary previousRecord = PXSelect<EPTimeCardSummary,
			        Where<EPTimeCardSummary.lineNbr, NotEqual<Current<EPTimeCardSummary.lineNbr>>,
						And<EPTimeCardSummary.timeCardCD, Equal<Current<EPTimeCardSummary.timeCardCD>>>>, OrderBy<Desc<EPTimeCardSummary.lineNbr>>>.SelectSingleBound(this, new object[] { row });

				if (previousRecord != null && !string.Equals(previousRecord.EarningType, row.EarningType, StringComparison.InvariantCultureIgnoreCase) && previousRecord.ProjectID != null)
		        {
			        row.ProjectID = previousRecord.ProjectID;
			        row.ProjectTaskID = previousRecord.ProjectTaskID;
		        }
	        }

	        if (row.ProjectID == null && earningType != null && earningType.TypeCD == EpSetup.Current.HolidaysType)
	        {
				row.ProjectID = PM.ProjectDefaultAttribute.NonProject(this);
	        }
        }
        
        protected virtual void EPTimeCardSummary_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            //Debug.Print("EPTimeCardSummary_RowInserted Start");
            Debug.Indent();
            try
            {
                EPTimeCardSummary row = e.Row as EPTimeCardSummary;
                if (row == null) return;
                if (dontSyncDetails) return;

                UpdateAdjustingActivities(row);
            }
            finally
            {
                Debug.Unindent();
                //Debug.Print("EPTimeCardSummary_RowInserted End");
            }
        }

		protected virtual void EPTimeCardSummary_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
		{
			EPTimeCardSummary row = e.NewRow as EPTimeCardSummary;
			EPTimeCardSummary oldRow = e.Row as EPTimeCardSummary;

			EPEarningType earningType = PXSelect<EPEarningType, Where<EPEarningType.typeCD, Equal<Required<EPEarningType.typeCD>>>>.Select(this, row.EarningType);
			if (earningType != null)
			{
				row.EarningType = earningType.TypeCD;
				if (row.IsBillable == null)
					row.IsBillable = earningType.isBillable == true;

				if (earningType.ProjectID != null)
				{
					if ((row.ProjectID == null || row.EarningType != oldRow.EarningType) &&
						PXSelectorAttribute.Select(sender, e.Row, sender.GetField(typeof(EPTimeCardSummary.projectID)), earningType.ProjectID) != null //available in selector
						)
					{
						row.ProjectID = earningType.ProjectID;
					}
				}

				if (earningType.TaskID != null)
				{
					if (row.ProjectTaskID == null || row.EarningType != oldRow.EarningType)
					{
						if (row.ProjectID == earningType.ProjectID)
						{
							PMTask defTask = PXSelectorAttribute.Select(sender, e.Row, sender.GetField(typeof(EPTimeCardSummary.projectTaskID)), earningType.TaskID) as PMTask;
							if (defTask != null && defTask.VisibleInEP == true)
							{
								row.ProjectTaskID = earningType.TaskID;
							}
						}
					}
				}
			}
		}

        protected virtual void EPTimeCardSummary_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            //Debug.Print("EPTimeCardSummary_RowUpdated Start");
            Debug.Indent();
            try
            {
                EPTimeCardSummary row = e.Row as EPTimeCardSummary;
                EPTimeCardSummary oldRow = e.OldRow as EPTimeCardSummary;

                if (row == null) return;

                //Debug.Print("Old: {0}", oldRow);
                //Debug.Print("New: {0}", row);

                if (dontSyncDetails) return;

                if (oldRow.ProjectID != row.ProjectID || oldRow.ProjectTaskID != row.ProjectTaskID ||
					oldRow.IsBillable != row.IsBillable || oldRow.ParentNoteID != row.ParentNoteID || 
					oldRow.EarningType != row.EarningType || oldRow.JobID != row.JobID || oldRow.ShiftID != row.ShiftID)
                {
					if (!ProjectDefaultAttribute.IsBillableProject(this, row.ProjectID))
                    {
                        row.IsBillable = false;
                    }

                    //It is assumed that this fields can be changed only for the auto activities. If atleast one manual activity is present these fields are not editable.
                    //hence here we will just delete the existing auto details and recreate new one:
                    
                    List<EPTimecardDetail> list = GetDetails(oldRow, Document.Current, false);

                    try
                    {
                        dontSyncSummary = true;
                        foreach (EPTimecardDetail item in list)
                        {
							if (item.SummaryLineNbr == row.LineNbr)
                                Activities.Delete(item);
                        }
                    }
                    finally
                    {
                        dontSyncSummary = false;
                    }
                }
               
                UpdateAdjustingActivities(row, oldRow.Description == row.Description);
            }
            finally
            {
                Debug.Unindent();
                //Debug.Print("EPTimeCardSummary_RowUpdated End");
            }
        }

        protected virtual void EPTimeCardSummary_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
        {
            //Debug.Print("EPTimeCardSummary_RowDeleting Start");
            Debug.Indent();
            try
            {
                EPTimeCardSummary row = e.Row as EPTimeCardSummary;
                if (row == null) return;

                if (dontSyncDetails)
                    return;

                if (row.Mon == null && row.Tue == null && row.Wed == null && row.Thu == null && row.Fri == null &&
                    row.Sat == null && row.Sun == null)
                    return; //Grid on pressing <Enter> at the last field automatically insert new row then delets it. Allow to delete such a row without checking. 

                if (GetDetails(row, Document.Current, true).Count > 0)//exists a detail record
                {
                    if (FindDuplicate(row) == null)//this is the last row that the detail record is referencing
                    {
                        e.Cancel = true;
                        throw new PXException(Messages.RecordCannotDeleted);
                    }
                }

            }
            finally
            {
                Debug.Unindent();
                //Debug.Print("EPTimeCardSummary_RowDeleting End");
            }
        }
        
        protected virtual void EPTimeCardSummary_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
        {
            //Debug.Print("EPTimeCardSummary_RowDeleted Start");
            Debug.Indent();
            try
            {
                EPTimeCardSummary row = e.Row as EPTimeCardSummary;
                if (row == null) return;
                if (dontSyncDetails) return;

                UpdateAdjustingActivities(row);
            }
            finally
            {
                Debug.Unindent();
                //Debug.Print("EPTimeCardSummary_RowDeleted End");
            }
        }
        protected virtual void EPTimeCardSummary_ProjectID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            EPTimeCardSummary row = e.Row as EPTimeCardSummary;
            if (row == null) return;
            
            PMTask currentTask = PXSelect<PMTask, Where<PMTask.taskID, Equal<Required<PMTask.taskID>>>>.Select(this, row.ProjectTaskID);
            if (currentTask != null)
            {
                PMTask newTask = PXSelect<PMTask, Where<PMTask.projectID, Equal<Required<PMTask.projectID>>, And<PMTask.taskCD, Equal<Required<PMTask.taskCD>>>>>.Select(this, row.ProjectID, currentTask.TaskCD);
                if (newTask != null)
                {
                    row.ProjectTaskID = newTask.TaskID;
                }
                else
                {
                    row.ProjectTaskID = null;
                }
                
            }

        }

		protected bool skipValidation = false;
	    protected virtual void EPTimeCardSummary_ProjectTaskID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
	    {
			EPTimeCardSummary row = (EPTimeCardSummary)e.Row;
		    if (row == null) return;

			if (skipValidation)
				return;

			if (e.NewValue != null && e.NewValue is int)
			{
				PMTask task = PXSelect<PMTask>.Search<PMTask.taskID>(sender.Graph, e.NewValue);
				if (task != null)
				{
					if (task.IsCompleted == true)
					{
						var ex = new PXSetPropertyException(PM.Messages.ProjectTaskIsCompleted, PXErrorLevel.Error);
						ex.ErrorValue = task.TaskCD;

						sender.RaiseExceptionHandling<EPTimeCardSummary.projectTaskID>(row, e.NewValue, ex);
					}

					if (task.IsCancelled == true)
					{
						var ex = new PXSetPropertyException(PM.Messages.ProjectTaskIsCanceled, PXErrorLevel.Error);
						ex.ErrorValue = task.TaskCD;

						sender.RaiseExceptionHandling<EPTimeCardSummary.projectTaskID>(row, e.NewValue, ex);
					}
				}
			}
	    }

        protected virtual void EPTimecardDetail_RowSelecting(PXCache sender, PXRowSelectingEventArgs e)
		{
			EPTimecardDetail row = e.Row as EPTimecardDetail;
			if (row != null)
			{
                using (new PXConnectionScope())
                {
                    InitEarningType(row);
                }

			    RecalculateFields(row);

                if (row.Date != null)
                    row.Day = ((int)row.Date.Value.DayOfWeek).ToString();
			}
		}
        
        protected virtual void EPTimecardDetail_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			EPTimecardDetail row = e.Row as EPTimecardDetail;
			if (row != null)
			{
                if (row.Released == true)
                    PXUIFieldAttribute.SetEnabled(sender, e.Row, false);
                else
                {
                    PXUIFieldAttribute.SetEnabled<EPTimecardDetail.billableTimeCalc>(sender, e.Row, row.IsOvertimeCalc != true && row.IsBillable == true);
                    PXUIFieldAttribute.SetEnabled<EPTimecardDetail.billableOvertimeCalc>(sender, e.Row, row.IsOvertimeCalc == true && row.IsBillable == true);

                    EPEarningType EarningTypes = (EPEarningType)PXSelectorAttribute.Select<EPTimecardDetail.earningTypeID>(sender, row);
					PXUIFieldAttribute.SetEnabled<EPTimecardDetail.isBillable>(sender, row, !ProjectDefaultAttribute.IsNonProject(this, row.ProjectID));
                }

				if (row.ApprovalStatus == ActivityStatusAttribute.Open && row.ApproverID != null)
				{
					PXUIFieldAttribute.SetWarning<EPTimecardDetail.approvalStatus>(sender, row, Messages.ActivityIsOpenAndCannotbeApproved);
				}
			}
		}

		protected virtual void EPTimecardDetail_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
            EPTimecardDetail row = e.Row as EPTimecardDetail;
			if (row == null) return;

			if (row.OrigNoteID != null) return;//inserting correction transaction - do not init just insert it as is.

			EPTimeCard doc = Document.Current;
			
			if (doc == null || doc.WeekID == null || doc.EmployeeID == null)
			{
				e.Cancel = true;
				return;
			}

            
            EPEarningType earningType = InitEarningType(row);
			if (earningType != null)
			{
				row.EarningTypeID = earningType.TypeCD;
			}

			if (row.ProjectID == null || !ProjectDefaultAttribute.IsBillableProject(this, row.ProjectID) && !isCreateCorrectionFlag)
            {
                row.IsBillable = false;
            }
            else
            {
				if (earningType != null && !isCreateCorrectionFlag)
					row.IsBillable = earningType.isBillable == true;
            }

            RecalculateFields(row);
			
			PXSelectReadonly<EPEmployee,
					Where<EPEmployee.bAccountID, Equal<Required<EPEmployee.bAccountID>>>>.Clear(this);

            EPEmployee employee = PXSelectReadonly<EPEmployee,
					Where<EPEmployee.bAccountID, Equal<Required<EPEmployee.bAccountID>>>>.
					Select(this, doc.EmployeeID);
			row.OwnerID = employee.With(_ => _.UserID);

			row.Billed = false;
		    row.TrackTime = true;

			if (earningType != null && earningType.ProjectID != null && row.ProjectID == null &&
					PXSelectorAttribute.Select(sender, e.Row, sender.GetField(typeof(EPTimecardDetail.projectID)), earningType.ProjectID) != null //available in selector
					)
            {
                row.ProjectID = earningType.ProjectID;
            }

			if (earningType != null && earningType.TaskID != null && row.ProjectTaskID == null && !ProjectDefaultAttribute.IsNonProject(this, row.ProjectID))
            {
				PMTask defTask = PXSelectorAttribute.Select(sender, e.Row, sender.GetField(typeof(EPTimecardDetail.projectTaskID)), earningType.TaskID) as PMTask;
				if (defTask != null && defTask.VisibleInEP == true)
				{
					row.ProjectTaskID = earningType.TaskID;
				}
            }

		    if (row.IsBillable == true)
		        row.TimeBillable = row.TimeSpent;
			
			
			row.ApprovalStatus = ActivityStatusListAttribute.Completed;

			if (row.Date == null)
			{
				row.Date = CRActivityMaint.GetNextActivityStartDate<EPTimecardDetail>(this, Activities.Select(), row, doc.WeekID, doc.WeekID, TempData.Cache, typeof(EP.CRActivityMaint.EPTempData.lastEnteredDate));
			}

			row.WeekID = doc.WeekID;

			row.Day = ((int)row.Date.Value.DayOfWeek).ToString();

            //autodefault to non-project if project module is off:
            if (row.ProjectID == null && !ProjectAttribute.IsPMVisible(this, BatchModule.EP))
                row.ProjectID = ProjectDefaultAttribute.NonProject(this);
		}

		protected virtual void EPTimecardDetail_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
            //Debug.Print("EPTimecardDetail_RowInserted Start");
            Debug.Indent();
		    try
		    {
                EPTimecardDetail row = e.Row as EPTimecardDetail;
                if (row == null) return;
                if (dontSyncSummary) return;

                EPTimeCardSummary summary = GetSummaryRecord(row);
                AddToSummary(summary, row);
		    }
		    finally
		    {
                Debug.Unindent();
                //Debug.Print("EPTimecardDetail_RowInserted End");
		    }

		   
		}

		protected virtual void EPTimecardDetail_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
		{
            EPTimecardDetail row = e.NewRow as EPTimecardDetail;
            EPTimecardDetail oldRow = e.Row as EPTimecardDetail;
			if (row == null || row.Billed == true) return;

			EPTimeCard doc = Document.Current;
			if (doc == null || doc.WeekID == null)
			{
				e.Cancel = true;
				return;
			}

			if (!dontSyncSummary)
			{
				EPEarningType earningType = InitEarningType(row);
				row.EarningTypeID = earningType.TypeCD;

				if (earningType.ProjectID != null)
				{
					if ((row.ProjectID == null || row.EarningTypeID != oldRow.EarningTypeID) &&
						PXSelectorAttribute.Select(sender, e.Row, sender.GetField(typeof(EPTimecardDetail.projectID)), earningType.ProjectID) != null //available in selector
						)
					{
						row.ProjectID = earningType.ProjectID;
					}
				}

				if (earningType.TaskID != null)
				{
					if (row.ProjectTaskID == null || row.EarningTypeID != oldRow.EarningTypeID)
					{
						if (row.ProjectID == earningType.ProjectID)
						{
							PMTask defTask = PXSelectorAttribute.Select(sender, e.Row, sender.GetField(typeof(EPTimecardDetail.projectTaskID)), earningType.TaskID) as PMTask;
							if (defTask != null && defTask.VisibleInEP == true)
							{
								row.ProjectTaskID = earningType.TaskID;
							}
						}
					}
				}

				if (row.EarningTypeID != oldRow.EarningTypeID || row.ProjectID != oldRow.ProjectID)
				{
					if (!ProjectDefaultAttribute.IsBillableProject(this, row.ProjectID))
					{
						row.IsBillable = false;
					}
					else
					{
						row.IsBillable = earningType.isBillable == true;
					}
					sender.RaiseFieldUpdated<EPTimecardDetail.isBillable>(row, null);
				}
			}

			if (row.ProjectID == null && row.TimeSpent.GetValueOrDefault() != 0)
                sender.RaiseExceptionHandling<EPTimecardDetail.projectID>(row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(EPTimecardDetail.projectID).Name));

			if (row.Date == null || PXWeekSelector2Attribute.GetWeekID(this, (DateTime)row.Date) != doc.WeekID)
			{
                row.Date = PXWeekSelector2Attribute.GetWeekStartDate(this, (int)doc.WeekID);
			}

			row.WeekID = doc.WeekID;
		}

		protected virtual void EPTimecardDetail_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
            //Debug.Print("EPTimecardDetail_RowUpdated Start");
            Debug.Indent();
		    try
		    {
			    EPTimecardDetail row = e.Row as EPTimecardDetail;
			    EPTimecardDetail oldRow = e.OldRow as EPTimecardDetail;
                if (row == null) return;

                if (row.SummaryLineNbr != null)
                {
                    EPTimeCardSummary summary = GetSummaryRecord(row);
                    if (summary != null && (summary.IsBillable.GetValueOrDefault() != row.IsBillable.GetValueOrDefault() ||
                        summary.EarningType != row.EarningTypeID ||
                        summary.ProjectID != row.ProjectID ||
                        summary.ProjectTaskID != row.ProjectTaskID ||
						summary.ParentNoteID != row.ParentTaskNoteID ||
                        summary.JobID != row.JobID ||
                        summary.ShiftID != row.ShiftID))
                    {
                        row.SummaryLineNbr = null;//summary and line are no longer linked with each other.
                    }
                }

		        if (row.EarningTypeID != oldRow.EarningTypeID)
		            InitEarningType(row);

                if (row.EarningTypeID != oldRow.EarningTypeID || row.TimeSpent.GetValueOrDefault() != oldRow.TimeSpent.GetValueOrDefault() ||
                    row.IsBillable.GetValueOrDefault() != oldRow.IsBillable.GetValueOrDefault())
		        {
		            RecalculateFields(row);
		        }
                
                if (row.Date.GetValueOrDefault() != oldRow.Date.GetValueOrDefault()
					|| row.EarningTypeID != oldRow.EarningTypeID 
					|| row.ProjectID.GetValueOrDefault() != oldRow.ProjectID.GetValueOrDefault() 
					|| row.ProjectTaskID.GetValueOrDefault() != oldRow.ProjectTaskID.GetValueOrDefault() 
					|| row.TimeSpent.GetValueOrDefault() != oldRow.TimeSpent.GetValueOrDefault() 
					|| row.IsBillable.GetValueOrDefault() != oldRow.IsBillable.GetValueOrDefault() 
					|| row.TimeBillable.GetValueOrDefault() != oldRow.TimeBillable.GetValueOrDefault() 
					|| row.JobID != oldRow.JobID 
					|| row.ShiftID != oldRow.ShiftID)
		        {
					if (row.ApproverID != null)
		            {
						row.ApprovalStatus = ActivityStatusListAttribute.PendingApproval;
		            }
		        }

		        if (!dontSyncSummary)
		        {
		            EPTimeCardSummary summaryOld = GetSummaryRecord(oldRow);
                    SubtractFromSummary(summaryOld, oldRow);

                    EPTimeCardSummary summary = GetSummaryRecord(row);
		            AddToSummary(summary, row);
                   
		        }
		    }
            finally
            {
                Debug.Unindent();
                //Debug.Print("EPTimecardDetail_RowUpdated End");
            }

		}

        protected virtual void EPTimecardDetail_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
        {
            //Debug.Print("EPTimecardDetail_RowDeleting Start");
            Debug.Indent();
            try
            {
                EPTimecardDetail row = e.Row as EPTimecardDetail;

                if (row != null)
                {
	                if (row.Released == true)
	                {
						throw new PXException(Messages.ActivityIsReleased);
					}
					if (Document.Current != null && !string.IsNullOrEmpty(Document.Current.OrigTimeCardCD) && ((EPTimecardDetail)e.Row).OrigNoteID != null
                        && Document.Cache.GetStatus(Document.Current) != PXEntryStatus.Deleted)
                    {
                        throw new PXException(Messages.CannotDeleteCorrectionActivity);
                    }

                    if (row.SummaryLineNbr == null)
                    {
                        if (!dontSyncSummary)
                        {
                            EPTimeCardSummary summary = GetSummaryRecord(row);
                            SubtractFromSummary(summary, row);
                        }
                    }
                    
                }
            }
            finally
            {
                Debug.Unindent();
                //Debug.Print("EPTimecardDetail_RowDeleting End");
            }
        }

        protected virtual void EPTimecardDetail_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
        {
            //Debug.Print("EPTimecardDetail_RowDeleted Start");
            Debug.Indent();
            try
            {
                EPTimecardDetail row = e.Row as EPTimecardDetail;

                if (row != null && row.SummaryLineNbr != null)//adjust activity is deleted
                {
                    if (!dontSyncSummary)
                    {
                        EPTimeCardSummary summary = GetSummaryRecord(row);
                        SubtractFromSummary(summary, row);
                    }
                }
            }
            finally
            {
                Debug.Unindent();
                //Debug.Print("EPTimecardDetail_RowDeleted End");
            }
        }
        
		protected virtual void EPTimecardDetail_StartDate_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
		{
            EPTimecardDetail row = e.Row as EPTimecardDetail;
			if (row == null) return;

			EPTimeCard doc = Document.Current;
			if (row.Date == null && doc != null && doc.WeekID != null && doc.EmployeeID != null)
			{
				row.Date = CRActivityMaint.GetNextActivityStartDate<EPTimecardDetail>(this, Activities.Select(), row, doc.WeekID, doc.WeekID, TempData.Cache, typeof(EP.CRActivityMaint.EPTempData.lastEnteredDate));
			}
		}
        
        protected virtual void EPTimecardDetail_Day_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
        {
			if (Document.Current == null || Document.Current.WeekID == null) return;
            PXWeekSelector2Attribute.WeekInfo weekInfo = PXWeekSelector2Attribute.GetWeekInfo(this, Document.Current.WeekID.Value);

            List<string> allowedValues = new List<string>();
            List<string> allowedLabels = new List<string>();

            if (weekInfo.Mon.Enabled)
            {
                allowedValues.Add("1");
                allowedLabels.Add(PXMessages.LocalizeNoPrefix(Messages.Monday));
            }

            if (weekInfo.Tue.Enabled)
            {
                allowedValues.Add("2");
                allowedLabels.Add(PXMessages.LocalizeNoPrefix(Messages.Tuesday));
            }

            if (weekInfo.Wed.Enabled)
            {
                allowedValues.Add("3");
                allowedLabels.Add(PXMessages.LocalizeNoPrefix(Messages.Wednesday));
            }

            if (weekInfo.Thu.Enabled)
            {
                allowedValues.Add("4");
                allowedLabels.Add(PXMessages.LocalizeNoPrefix(Messages.Thursday));
            }

            if (weekInfo.Fri.Enabled)
            {
                allowedValues.Add("5");
                allowedLabels.Add(PXMessages.LocalizeNoPrefix(Messages.Friday));
            }

            if (weekInfo.Sat.Enabled)
            {
                allowedValues.Add("6");
                allowedLabels.Add(PXMessages.LocalizeNoPrefix(Messages.Saturday));
            }

            if (weekInfo.Sun.Enabled)
            {
                allowedValues.Add("0");
                allowedLabels.Add(PXMessages.LocalizeNoPrefix(Messages.Sunday));
            }

            e.ReturnState = PXStringState.CreateInstance(e.ReturnState, 1, false, typeof(EPTimecardDetail.day).Name, false, 1, null,
                                                    allowedValues.ToArray(), allowedLabels.ToArray(), true, allowedValues[0]);
        } 

        protected virtual void EPTimecardDetail_Day_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            EPTimecardDetail row = e.Row as EPTimecardDetail;
            if (row == null || e.ExternalCall != true) return;

            if (Document.Current != null)
            {
                PXWeekSelector2Attribute.WeekInfo weekInfo = PXWeekSelector2Attribute.GetWeekInfo(this, Document.Current.WeekID.Value);

				DayOfWeek val = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), row.Day);

                switch (val)
                {
                    case DayOfWeek.Monday:
						sender.SetValueExt<EPTimecardDetail.date>(row, weekInfo.Mon.Date);
						break;
                    case DayOfWeek.Tuesday:
						sender.SetValueExt<EPTimecardDetail.date>(row, weekInfo.Tue.Date);
						break;
                    case DayOfWeek.Wednesday:
						sender.SetValueExt<EPTimecardDetail.date>(row, weekInfo.Wed.Date);
						break;
                    case DayOfWeek.Thursday:
						sender.SetValueExt<EPTimecardDetail.date>(row, weekInfo.Thu.Date);
						break;
                    case DayOfWeek.Friday:
						sender.SetValueExt<EPTimecardDetail.date>(row, weekInfo.Fri.Date);
						break;
                    case DayOfWeek.Saturday:
						sender.SetValueExt<EPTimecardDetail.date>(row, weekInfo.Sat.Date);
						break;
                    case DayOfWeek.Sunday:
						sender.SetValueExt<EPTimecardDetail.date>(row, weekInfo.Sun.Date);
						break;

                }
            }
        }

		protected virtual void EPTimecardDetail_StartDate_Date_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
            EPTimecardDetail row = e.Row as EPTimecardDetail;
			if (row == null) return;

			int? weekId = row.WeekID ?? Document.Current.With(_ => _.WeekID);
			if (weekId == null) return;

			DateTime? newValue = null;
			DateTime valFromString;
			if (e.NewValue is string &&
				DateTime.TryParse((string)e.NewValue, sender.Graph.Culture, System.Globalization.DateTimeStyles.None, out valFromString))
			{
				newValue = valFromString;
			}
			if (e.NewValue is DateTime)
				newValue = (DateTime)e.NewValue;

            PXWeekSelector2Attribute.WeekInfo weekInfo = PXWeekSelector2Attribute.GetWeekInfo(this, Document.Current.WeekID.Value);
            if (newValue != null && !weekInfo.IsValid(((DateTime)newValue).Date))
			{
				throw new PXSetPropertyException(Messages.DateNotInWeek);
			}
		}

		protected virtual void EPTimecardDetail_OwnerID_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
		{
            EPTimecardDetail row = e.Row as EPTimecardDetail;
			if (row == null) return;

			if (row.OwnerID == null)
			{
                EPEmployee employee = Document.Current.
					With(_ => PXSelectReadonly<EPEmployee,
					Where<EPEmployee.bAccountID, Equal<Required<EPEmployee.bAccountID>>>>.
					Select(this, _.EmployeeID));
				row.OwnerID = employee.With(_ => _.UserID);
			}
		}

		protected virtual void EPTimecardDetail_StartDate_Date_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
		{
			var row = e.Row as EPTimecardDetail;
			if (row == null) return;

			DateTime? newValue = null;
			DateTime valFromString;
			if (e.NewValue is string &&
				DateTime.TryParse((string)e.NewValue, sender.Graph.Culture, System.Globalization.DateTimeStyles.None, out valFromString))
			{
				newValue = valFromString;
			}
			if (e.NewValue is DateTime)
				newValue = (DateTime)e.NewValue;

			sender.SetValuePending(e.Row, typeof(EPTimecardDetail.date).Name + "_oldValue", newValue);
		}

		protected virtual void EPTimecardDetail_StartDate_Time_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
		{
			var row = e.Row as EPTimecardDetail;
			if (row == null) return;

			DateTime? newValue = null;
			DateTime valFromString;
			if (e.NewValue is string &&
				DateTime.TryParse((string)e.NewValue, sender.Graph.Culture, System.Globalization.DateTimeStyles.None, out valFromString))
			{
				newValue = valFromString;
			}
			if (e.NewValue is DateTime)
				newValue = (DateTime)e.NewValue;

			var oldValue = (DateTime?)sender.GetValuePending(e.Row, typeof(EPTimecardDetail.date).Name + "_oldValue");
			if (oldValue != null && newValue != null &&
				(int)oldValue.Value.TimeOfDay.TotalMinutes == (int)newValue.Value.TimeOfDay.TotalMinutes)
			{
				e.Cancel = true;
			}
		}

		protected virtual void EPTimecardDetail_StartDate_Date_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			var row = e.Row as EPTimecardDetail;
			if (row == null) return;

			var oldValue = (DateTime?)sender.GetValuePending(e.Row, typeof(EPTimecardDetail.date).Name + "_oldValue");
			var newValue = row.Date;
			if (newValue != null &&
				(oldValue == null ||
					oldValue != newValue && ((int)oldValue.Value.TimeOfDay.TotalMinutes == (int)newValue.Value.TimeOfDay.TotalMinutes)))
			{
				var calendarId = CRActivityMaint.GetCalendarID(this, row);
				if (!string.IsNullOrEmpty(calendarId))
				{
					DateTime start;
					DateTime end;
					CalendarHelper.CalculateStartEndTime(this, calendarId, newValue.Value.Date, out start, out end);
					row.Date = start;
				}
			}

            row.Day = ((int)row.Date.Value.DayOfWeek).ToString();

			if (row != null && row.Date != null)
				TempData.Cache.SetValue<CRActivityMaint.EPTempData.lastEnteredDate>(TempData.Current, row.Date);
		}

        protected virtual void EPTimecardDetail_BillableTimeCalc_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            EPTimecardDetail row = e.Row as EPTimecardDetail;
            if (row == null) return;
			int? newBillableTime = (int?)e.NewValue;

            if (row.TimeSpent < newBillableTime)
            {
                throw new PXSetPropertyException(CR.Messages.BillableTimeCannotBeGreaterThanTimeSpent);
            }
        }
        
		protected virtual void EPTimecardDetail_UIStatus_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            e.Cancel = true;
        }
        
		protected virtual void EPTimecardDetail_BillableTimeCalc_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            EPTimecardDetail row = e.Row as EPTimecardDetail;
            if (row == null) return;
            row.TimeBillable = row.BillableTimeCalc;

        }

        protected virtual void EPTimecardDetail_BillableOvertimeCalc_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            EPTimecardDetail row = e.Row as EPTimecardDetail;
            if (row == null) return;
            int? newBillableTime = (int?)e.NewValue;

            if (row.TimeSpent < newBillableTime)
            {
                throw new PXSetPropertyException(CR.Messages.OvertimeBillableCannotBeGreaterThanOvertimeSpent);
            }
        }

        protected virtual void EPTimecardDetail_BillableOvertimeCalc_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            EPTimecardDetail row = e.Row as EPTimecardDetail;
            if (row == null) return;
            row.TimeBillable = row.BillableOvertimeCalc;
            
        }

        protected virtual void EPTimecardDetail_IsBillable_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            EPTimecardDetail row = e.Row as EPTimecardDetail;
            if (row == null) return;
            
            if (row.IsBillable == true && row.TimeBillable.GetValueOrDefault() == 0)
            {
                row.TimeBillable = row.TimeSpent;
            }
        }

        protected virtual void EPTimeCardItem_InventoryID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            sender.SetDefaultExt<EPTimeCardItem.uOM>(e.Row);
        }

        protected virtual void EPApproval_Details_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            if (this.Document.Current == null) return;

            e.NewValue = null;
            Type[] fields = new Type[]
			{
				typeof(EPTimeCard.weekDescription), typeof(EPTimeCard.timeSpentCalc), typeof(EPTimeCard.overtimeSpentCalc),typeof(EPTimeCard.timeBillableCalc), typeof(EPTimeCard.overtimeBillableCalc)
			};
            foreach (Type t in fields)
            {
                PXStringState strState = this.Document.Cache.GetValueExt(this.Document.Current, t.Name) as PXStringState;
                if (strState != null)
                {
                    string value =
                        strState.InputMask != null ? Mask.Format(strState.InputMask, strState) :
                        strState.Value != null ? strState.Value.ToString() : null;

                    if (!string.IsNullOrEmpty(value))
                        e.NewValue += (e.NewValue != null ? ", " : string.Empty) + strState.DisplayName + "=" + value.Trim();
                }
            }
        }
        
		#endregion
        
		#region Protected Methods

		public virtual void ProcessRegularTimecard(PM.RegisterEntry releaseGraph, EPTimeCard timecard)
        {
            PXCache registerCache = releaseGraph.Document.Cache;
            PXCache tranCache = releaseGraph.Transactions.Cache;
            PM.PMRegister doc = (PM.PMRegister)registerCache.Insert();
            doc.OrigDocType = PMOrigDocType.Timecard;
            doc.OrigDocNbr = timecard.TimeCardCD;
            releaseGraph.FieldVerifying.AddHandler<PMTran.inventoryID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });//restriction should be applicable only for budgeting.
            
            EP.EPEmployee emp = PXSelect<EP.EPEmployee, Where<EP.EPEmployee.bAccountID, Equal<Required<EP.EPEmployee.bAccountID>>>>.Select(this, timecard.EmployeeID);
            if (emp != null)
            {
                doc.Description = string.Format("{0} - {1}", emp.AcctName, timecard.WeekID);
            }

            List<ReleasedActivity> released = new List<ReleasedActivity>();
            foreach (EPTimecardDetail act in Activities.Select())
            {
                if (act.Released == true)//activities can be released throught employee activity release screen (if TC is not required for an employee)
                    continue;

				bool isBilled = false;

				if (act.ContractID != null)
				{
					//Contract without CRM mode
					EmployeeActivitiesRelease.CreateContractUsage(releaseGraph, act.ContractID, act, act.TimeBillable.GetValueOrDefault());
				}
				else if (act.RefNoteID != null && PXSelectJoin<CRCase,
						InnerJoin<CRActivityLink,
							On<CRActivityLink.refNoteID, Equal<CRCase.noteID>>>, 
						Where<CRActivityLink.noteID, Equal<Required<EPTimecardDetail.refNoteID>>>>.Select(releaseGraph, act.RefNoteID).Count == 1)
				{
					 //Add Contract-Usage
						PMTran contractUsageTran = releaseGraph.CreateContractUsage(act, act.TimeBillable.GetValueOrDefault());
					if (contractUsageTran != null)
                        isBilled = true;
                }
			    
				decimal? cost = CreateEmployeeCostEngine().CalculateEmployeeCost(act, timecard.EmployeeID, act.Date.Value);
                releaseGraph.CreateTransaction(act, timecard.EmployeeID, act.Date.Value, act.TimeSpent, act.TimeBillable, cost);
				released.Add(new ReleasedActivity(act, cost.GetValueOrDefault(), isBilled));
            }


            PXWeekSelector2Attribute.WeekInfo week = PXWeekSelector2Attribute.GetWeekInfo(this, timecard.WeekID.Value);

            foreach (EPTimeCardItem item in Items.View.SelectMultiBound(new object[] { timecard }))
            {
                #region Create Transactions By Days

                if (item.Sun.GetValueOrDefault() > 0)
                {
                    CreateItemTransaction(item, timecard.EmployeeID, week.Sun.Date, item.Sun, tranCache);
                }

                if (item.Mon.GetValueOrDefault() > 0)
                {
                    CreateItemTransaction(item, timecard.EmployeeID, week.Mon.Date, item.Mon, tranCache);
                }

                if (item.Tue.GetValueOrDefault() > 0)
                {
                    CreateItemTransaction(item, timecard.EmployeeID, week.Tue.Date, item.Tue, tranCache);
                }

                if (item.Wed.GetValueOrDefault() > 0)
                {
                    CreateItemTransaction(item, timecard.EmployeeID, week.Wed.Date, item.Wed, tranCache);
                }

                if (item.Thu.GetValueOrDefault() > 0)
                {
                    CreateItemTransaction(item, timecard.EmployeeID, week.Thu.Date, item.Thu, tranCache);
                }

                if (item.Fri.GetValueOrDefault() > 0)
                {
                    CreateItemTransaction(item, timecard.EmployeeID, week.Fri.Date, item.Fri, tranCache);
                }

                if (item.Sat.GetValueOrDefault() > 0)
                {
                    CreateItemTransaction(item, timecard.EmployeeID, week.Sat.Date, item.Sat, tranCache);
                }

                #endregion
            }

			//Setting the released/billed status for all activities if and only if all the transaction were processed without an exception in the above loop.
			foreach (ReleasedActivity act in released)
			{
				Activities.Cache.SetValueExt<EPTimecardDetail.released>(act.Activity, true);
				Activities.Cache.SetValue<EPTimecardDetail.employeeRate>(act.Activity, act.Cost);
				if (act.IsBilled)
					Activities.Cache.SetValue<EPTimecardDetail.billed>(act.Activity, true);

				Activities.Cache.SetStatus(act.Activity, PXEntryStatus.Updated);
			}
        }

		public virtual void ProcessCorrectingTimecard(PM.RegisterEntry releaseGraph, EPTimeCard timecard)
        {
            PXCache registerCache = releaseGraph.Document.Cache;
            PXCache tranCache = releaseGraph.Transactions.Cache;
            PM.PMRegister doc = (PM.PMRegister)registerCache.Insert();
            doc.OrigDocType = PMOrigDocType.Timecard;
            doc.OrigDocNbr = timecard.TimeCardCD;
            releaseGraph.FieldVerifying.AddHandler<PMTran.inventoryID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });//restriction should be applicable only for budgeting.

            EP.EPEmployee emp = PXSelect<EP.EPEmployee, Where<EP.EPEmployee.bAccountID, Equal<Required<EP.EPEmployee.bAccountID>>>>.Select(this, timecard.EmployeeID);
            if (emp != null)
            {
                doc.Description = string.Format("{0} - {1} correction", emp.AcctName, timecard.WeekID);
            }

			List<ReleasedActivity> released = new List<ReleasedActivity>();

            //process deleted activities:
			PXSelectBase<EPTimecardDetailOrig> selectDeletedActivities = new PXSelectJoin<
				EPTimecardDetailOrig,
			InnerJoin<CREmployee, 
				On<CREmployee.userID, Equal<EPTimecardDetailOrig.ownerID>>,
			LeftJoin<EPTimecardDetailEx, 
				On<EPTimecardDetailOrig.noteID, Equal<EPTimecardDetailEx.origNoteID>>>>,
                    Where<CREmployee.bAccountID, Equal<Required<EPTimeCard.employeeID>>,
                        And<EPTimecardDetailOrig.weekID, Equal<Required<EPTimeCard.weekId>>,
                        And<EPTimecardDetailOrig.timeSheetCD, IsNull,
				And<EPTimecardDetailEx.noteID, IsNull,
				And<Where<EPTimecardDetailOrig.timeCardCD, IsNull, 
					Or<EPTimecardDetailOrig.timeCardCD, Equal<Required<EPTimeCard.timeCardCD>>>>>>>>>,
                    OrderBy<Asc<EPTimecardDetailOrig.date>>>(this);

            foreach (PXResult<EPTimecardDetailOrig, CREmployee, EPTimecardDetailEx> res in selectDeletedActivities.Select(timecard.EmployeeID, timecard.WeekID, timecard.OrigTimeCardCD))
            {
                EPTimecardDetailOrig orig = (EPTimecardDetailOrig)res;
                bool isBilled = false;
                decimal? origCost = orig.EmployeeRate ?? CreateEmployeeCostEngine().CalculateEmployeeCost(orig, timecard.EmployeeID, orig.Date.Value);

                releaseGraph.CreateTransaction(orig, timecard.EmployeeID, orig.Date.Value, -orig.TimeSpent, -orig.TimeBillable, origCost);
                Activities.Cache.SetValueExt<EPTimecardDetail.released>(orig, true);
                Activities.Cache.SetValue<EPTimecardDetail.employeeRate>(orig, IN.PXDBPriceCostAttribute.Round((decimal)origCost));
                Activities.Cache.SetStatus(orig, PXEntryStatus.Updated);

                if (orig.RefNoteID != null)
                {
                    CRCase crCase = PXSelectJoin<CRCase, 
						InnerJoin<CRActivityLink,
							On<CRActivityLink.refNoteID, Equal<CRCase.noteID>>>,
						Where<CRActivityLink.noteID, Equal<Required<EPTimecardDetailOrig.refNoteID>>>>.Select(this, orig.RefNoteID);
                    if (crCase != null && crCase.ContractID != null)
                    {
                        PMTran contractUsageTran = null;
                        if (orig.IsBillable == true)
                        {
                            //Subtract Contract-Usage
                            contractUsageTran = releaseGraph.CreateContractUsage(orig, -orig.TimeBillable.GetValueOrDefault());
                        }

                        if (contractUsageTran != null)
                            isBilled = true;
                    }
                }

                released.Add(new ReleasedActivity(orig, origCost.GetValueOrDefault(), isBilled));
            }

            //process new and modified activities:
			PXSelectBase<EPTimecardDetail> selectActivities = new PXSelectJoin<
				EPTimecardDetail,
			InnerJoin<CREmployee, 
				On<CREmployee.userID, Equal<EPTimecardDetailOrig.ownerID>>,
			LeftJoin<EPTimecardDetailOrig, 
				On<EPTimecardDetailOrig.noteID, Equal<EPTimecardDetail.origNoteID>>>>,
                    Where<CREmployee.bAccountID, Equal<Required<EPTimeCard.employeeID>>,
                        And<EPTimecardDetail.weekID, Equal<Required<EPTimeCard.weekId>>,
                        And<EPTimecardDetail.timeSheetCD, IsNull,
				And<Where<EPTimecardDetail.timeCardCD, IsNull, 
					Or<EPTimecardDetail.timeCardCD, Equal<Required<EPTimeCard.timeCardCD>>>>>>>>,
                    OrderBy<Asc<EPTimecardDetail.date>>>(this);

            foreach (PXResult<EPTimecardDetail, CREmployee, EPTimecardDetailOrig> res in selectActivities.Select(timecard.EmployeeID, timecard.WeekID, timecard.TimeCardCD))
            {
                EPTimecardDetail act = (EPTimecardDetail)res;

				if (act.Released == true)//new added activities can be released throught employee activity release screen (if TC is not required for an employee)
                    continue;
				bool isBilled = false;

                EPTimecardDetailOrig orig = (EPTimecardDetailOrig)res;
                Activities.Cache.RaiseRowSelected(act);

                decimal? origCost = null;
				decimal? cost = CreateEmployeeCostEngine().CalculateEmployeeCost(act, timecard.EmployeeID, act.Date.Value);
				if (orig.NoteID != null)
					origCost = orig.EmployeeRate ?? CreateEmployeeCostEngine().CalculateEmployeeCost(orig, timecard.EmployeeID, orig.Date.Value);


				int? time = act.TimeSpent;
				if (orig.NoteID != null)
				{
					time = ((act.TimeSpent ?? 0) - (orig.TimeSpent ?? 0));
				}

				int? timeBillable = act.TimeBillable;
				if (orig.NoteID != null)
				{
					timeBillable = ((act.TimeBillable ?? 0) - (orig.TimeBillable ?? 0));
				}


				if (act.RefNoteID != null)
				{
					CRCase crCase = PXSelectJoin<CRCase,
						InnerJoin<CRActivityLink,
							On<CRActivityLink.refNoteID, Equal<CRCase.noteID>>>, 
						Where<CRActivityLink.noteID, Equal<Required<EPTimecardDetail.refNoteID>>>>.Select(this, act.RefNoteID);
					if (crCase != null && crCase.ContractID != null)
					{
						PMTran contractUsageTran = null;
						if (act.IsBillable == orig.IsBillable)
						{
							//Correct Contract-Usage
							contractUsageTran = releaseGraph.CreateContractUsage(act, timeBillable.GetValueOrDefault());
						}
						else
						{
							if (act.IsBillable == true)
							{
								//Add Usage
								contractUsageTran = releaseGraph.CreateContractUsage(act, timeBillable.GetValueOrDefault());
							}
							else
							{
								//Subtract Usage
								contractUsageTran = releaseGraph.CreateContractUsage(act, -timeBillable.GetValueOrDefault());
							}
						}

						if (contractUsageTran != null)
							isBilled = false;
					}
				}


				if (orig.NoteID == null || (act.ProjectID == orig.ProjectID && act.ProjectTaskID == orig.ProjectTaskID && origCost == cost && act.IsBillable == orig.IsBillable))
                {
                    releaseGraph.CreateTransaction(act, timecard.EmployeeID, act.Date.Value, time, timeBillable, cost);
                }
                else
                {
                    //delete previous:
                    releaseGraph.CreateTransaction(orig, timecard.EmployeeID, orig.Date.Value, -orig.TimeSpent, -orig.TimeBillable, origCost);
                   
                    //add new:
                    releaseGraph.CreateTransaction(act, timecard.EmployeeID, act.Date.Value, act.TimeSpent, act.TimeBillable, cost);
                }
				
				released.Add(new ReleasedActivity(act, IN.PXDBPriceCostAttribute.Round((decimal)cost), isBilled));
            }
            

            //Non-Stock Items:
            PXWeekSelector2Attribute.WeekInfo week = PXWeekSelector2Attribute.GetWeekInfo(this, timecard.WeekID.Value);
            //process deleted items
            PXSelectBase<EPTimeCardItemOrig> selectDeletedItems = new PXSelectJoin<EPTimeCardItemOrig,
                LeftJoin<EPTimeCardItemEx, On<EPTimeCardItemEx.timeCardCD, Equal<Current<EPTimeCard.timeCardCD>>,
                    And<EPTimeCardItemEx.origLineNbr, Equal<EPTimeCardItemOrig.lineNbr>>>>,
                Where<EPTimeCardItemOrig.timeCardCD, Equal<Current<EPTimeCard.origTimeCardCD>>,
                    And<EPTimeCardItemEx.timeCardCD, IsNull>>>(this);

            foreach (PXResult<EPTimeCardItemOrig, EPTimeCardItemEx> res in selectDeletedItems.View.SelectMultiBound(new object[] { timecard }))
            {
                EPTimeCardItemOrig orig = (EPTimeCardItemOrig)res;

                #region Create Transactions By Days

                if (orig.Sun.GetValueOrDefault() > 0)
                {
                    CreateItemTransaction(orig, timecard.EmployeeID, week.Sun.Date, -orig.Sun, tranCache);
                    
                }

                if (orig.Mon.GetValueOrDefault() > 0)
                {
                    CreateItemTransaction(orig, timecard.EmployeeID, week.Mon.Date, -orig.Mon, tranCache);
                }

                if (orig.Tue.GetValueOrDefault() > 0)
                {
                    CreateItemTransaction(orig, timecard.EmployeeID, week.Tue.Date, -orig.Tue, tranCache);
                }

                if (orig.Wed.GetValueOrDefault() > 0)
                {
                    CreateItemTransaction(orig, timecard.EmployeeID, week.Wed.Date, -orig.Wed, tranCache);
                }

                if (orig.Thu.GetValueOrDefault() > 0)
                {
                    CreateItemTransaction(orig, timecard.EmployeeID, week.Thu.Date, -orig.Thu, tranCache);
                }

                if (orig.Fri.GetValueOrDefault() > 0)
                {
                    CreateItemTransaction(orig, timecard.EmployeeID, week.Fri.Date, -orig.Fri, tranCache);
                }

                if (orig.Sat.GetValueOrDefault() > 0)
                {
                    CreateItemTransaction(orig, timecard.EmployeeID, week.Sat.Date, -orig.Sat, tranCache);
                }

                #endregion

            }

            //process added items
            PXSelectBase<EPTimeCardItem> selectAddedItems = new PXSelect<EPTimeCardItem,
                Where<EPTimeCardItem.timeCardCD, Equal<Current<EPTimeCard.timeCardCD>>,
                    And<EPTimeCardItem.origLineNbr, IsNull>>>(this);
            foreach (EPTimeCardItem item in selectAddedItems.View.SelectMultiBound(new object[] { timecard }))
            {
                #region Create Transactions By Days

                if (item.Sun.GetValueOrDefault() > 0)
                {
                    CreateItemTransaction(item, timecard.EmployeeID, week.Sun.Date, item.Sun, tranCache);
                }

                if (item.Mon.GetValueOrDefault() > 0)
                {
                    CreateItemTransaction(item, timecard.EmployeeID, week.Mon.Date, item.Mon, tranCache);
                }

                if (item.Tue.GetValueOrDefault() > 0)
                {
                    CreateItemTransaction(item, timecard.EmployeeID, week.Tue.Date, item.Tue, tranCache);
                }

                if (item.Wed.GetValueOrDefault() > 0)
                {
                    CreateItemTransaction(item, timecard.EmployeeID, week.Wed.Date, item.Wed, tranCache);
                }

                if (item.Thu.GetValueOrDefault() > 0)
                {
                    CreateItemTransaction(item, timecard.EmployeeID, week.Thu.Date, item.Thu, tranCache);
                }

                if (item.Fri.GetValueOrDefault() > 0)
                {
                    CreateItemTransaction(item, timecard.EmployeeID, week.Fri.Date, item.Fri, tranCache);
                }

                if (item.Sat.GetValueOrDefault() > 0)
                {
                    CreateItemTransaction(item, timecard.EmployeeID, week.Sat.Date, item.Sat, tranCache);
                }

                #endregion
            }

            //process modified records
            PXSelectBase<EPTimeCardItemOrig> selectModifiedItems = new PXSelectJoin<EPTimeCardItemOrig,
                LeftJoin<EPTimeCardItemEx, On<EPTimeCardItemEx.timeCardCD, Equal<Current<EPTimeCard.timeCardCD>>,
                    And<EPTimeCardItemEx.origLineNbr, Equal<EPTimeCardItemOrig.lineNbr>>>>,
                Where<EPTimeCardItemOrig.timeCardCD, Equal<Current<EPTimeCard.origTimeCardCD>>,
                    And<EPTimeCardItemEx.timeCardCD, IsNotNull>>>(this);
            foreach (PXResult<EPTimeCardItemOrig, EPTimeCardItemEx> res in selectModifiedItems.View.SelectMultiBound(new object[] { timecard }))
            {
                EPTimeCardItemOrig orig = (EPTimeCardItemOrig)res;
                EPTimeCardItemEx item = (EPTimeCardItemEx)res;

                #region Create Transactions By Days

	            if (orig.InventoryID == item.InventoryID)
	            {
                if (item.Sun.GetValueOrDefault() - orig.Sun.GetValueOrDefault() != 0)
                {
                    CreateItemTransaction(item, timecard.EmployeeID, week.Sun.Date, item.Sun.GetValueOrDefault() - orig.Sun.GetValueOrDefault(), tranCache);
                }
	            }
	            else
	            {
					if (orig.Sun.GetValueOrDefault() != 0)
						CreateItemTransaction(orig, timecard.EmployeeID, week.Sun.Date, -orig.Sun.GetValueOrDefault(), tranCache);
					if (item.Sun.GetValueOrDefault() != 0)
						CreateItemTransaction(item, timecard.EmployeeID, week.Sun.Date, item.Sun.GetValueOrDefault(), tranCache);
	            }

	            if (orig.InventoryID == item.InventoryID)
	            {
                if (item.Mon.GetValueOrDefault() - orig.Mon.GetValueOrDefault() != 0)
                {
			            CreateItemTransaction(item, timecard.EmployeeID, week.Mon.Date,
				            item.Mon.GetValueOrDefault() - orig.Mon.GetValueOrDefault(), tranCache);
		            }
	            }
				else
				{
					if (orig.Mon.GetValueOrDefault() != 0)
						CreateItemTransaction(orig, timecard.EmployeeID, week.Mon.Date, -orig.Mon.GetValueOrDefault(), tranCache);
					if (item.Mon.GetValueOrDefault() != 0)
						CreateItemTransaction(item, timecard.EmployeeID, week.Mon.Date, item.Mon.GetValueOrDefault(), tranCache);
                }

	            if (orig.InventoryID == item.InventoryID)
	            {
                if (item.Tue.GetValueOrDefault() - orig.Tue.GetValueOrDefault() != 0)
                {
			            CreateItemTransaction(item, timecard.EmployeeID, week.Tue.Date,
				            item.Tue.GetValueOrDefault() - orig.Tue.GetValueOrDefault(), tranCache);
		            }
	            }
				else
				{
					if (orig.Tue.GetValueOrDefault() != 0)
						CreateItemTransaction(orig, timecard.EmployeeID, week.Tue.Date, -orig.Tue.GetValueOrDefault(), tranCache);
					if (item.Tue.GetValueOrDefault() != 0) 
						CreateItemTransaction(item, timecard.EmployeeID, week.Tue.Date, item.Tue.GetValueOrDefault(), tranCache);
                }

	            if (orig.InventoryID == item.InventoryID)
	            {
                if (item.Wed.GetValueOrDefault() - orig.Wed.GetValueOrDefault() != 0)
                {
			            CreateItemTransaction(item, timecard.EmployeeID, week.Wed.Date,
				            item.Wed.GetValueOrDefault() - orig.Wed.GetValueOrDefault(), tranCache);
		            }
	            }
				else
				{
					if (orig.Wed.GetValueOrDefault() != 0)
						CreateItemTransaction(orig, timecard.EmployeeID, week.Wed.Date, -orig.Wed.GetValueOrDefault(), tranCache);
					if (item.Wed.GetValueOrDefault() != 0) 
						CreateItemTransaction(item, timecard.EmployeeID, week.Wed.Date, item.Wed.GetValueOrDefault(), tranCache);
                }

	            if (orig.InventoryID == item.InventoryID)
	            {
                if (item.Thu.GetValueOrDefault() - orig.Thu.GetValueOrDefault() != 0)
                {
			            CreateItemTransaction(item, timecard.EmployeeID, week.Thu.Date,
				            item.Thu.GetValueOrDefault() - orig.Thu.GetValueOrDefault(), tranCache);
		            }
	            }
				else
				{
					if (orig.Thu.GetValueOrDefault() != 0) 
						CreateItemTransaction(orig, timecard.EmployeeID, week.Thu.Date, -orig.Thu.GetValueOrDefault(), tranCache);
					if (item.Thu.GetValueOrDefault() != 0) 
						CreateItemTransaction(item, timecard.EmployeeID, week.Thu.Date, item.Thu.GetValueOrDefault(), tranCache);
                }

	            if (orig.InventoryID == item.InventoryID)
	            {
                if (item.Fri.GetValueOrDefault() - orig.Fri.GetValueOrDefault() != 0)
                {
			            CreateItemTransaction(item, timecard.EmployeeID, week.Fri.Date,
				            item.Fri.GetValueOrDefault() - orig.Fri.GetValueOrDefault(), tranCache);
		            }
	            }
				else
				{
					if (orig.Fri.GetValueOrDefault() != 0) 
						CreateItemTransaction(orig, timecard.EmployeeID, week.Fri.Date, -orig.Fri.GetValueOrDefault(), tranCache);
					if (item.Fri.GetValueOrDefault() != 0) 
						CreateItemTransaction(item, timecard.EmployeeID, week.Fri.Date, item.Fri.GetValueOrDefault(), tranCache);
                }

	            if (orig.InventoryID == item.InventoryID)
	            {
                if (item.Sat.GetValueOrDefault() - orig.Sat.GetValueOrDefault() != 0)
                {
			            CreateItemTransaction(item, timecard.EmployeeID, week.Sat.Date,
				            item.Sat.GetValueOrDefault() - orig.Sat.GetValueOrDefault(), tranCache);
		            }
	            }
				else
				{
					if (orig.Sat.GetValueOrDefault() != 0) 
						CreateItemTransaction(orig, timecard.EmployeeID, week.Sat.Date, -orig.Sat.GetValueOrDefault(), tranCache);
					if (item.Sat.GetValueOrDefault() != 0) 
						CreateItemTransaction(item, timecard.EmployeeID, week.Sat.Date, item.Sat.GetValueOrDefault(), tranCache);
                }

                #endregion

            }

			//Setting the released/billed status for all activities if and only if all the transaction were processed without an exception in the above loop.
			foreach (ReleasedActivity act in released)
			{
				Activities.Cache.SetValueExt<EPTimecardDetail.released>(act.Activity, true);
				Activities.Cache.SetValue<EPTimecardDetail.employeeRate>(act.Activity, act.Cost);
				if (act.IsBilled)
					Activities.Cache.SetValue<EPTimecardDetail.billed>(act.Activity, true);

				Activities.Cache.SetStatus(act.Activity, PXEntryStatus.Updated);
			}
        }
        
		public virtual EPEarningType InitEarningType(EPTimecardDetail row)
        {
            if (row == null)
                throw new ArgumentNullException();

            EPEarningType earningType = PXSelect<EPEarningType, Where<EPEarningType.typeCD, Equal<Required<EPEarningType.typeCD>>>>.Select(this, row.EarningTypeID);

			if (earningType != null && row.EarningTypeID != null && Document.Current != null && Document.Current.EmployeeID != null && row.Date != null)
            {
                row.IsOvertimeCalc = earningType.IsOvertime;
				row.OvertimeMultiplierCalc = CreateEmployeeCostEngine().GetOvertimeMultiplier((string)row.EarningTypeID, (int)Document.Current.EmployeeID, (DateTime)row.Date);
            }

            return earningType;
        }

		public virtual void RecalculateFields(EPTimecardDetail row)
        {
            if (row == null)
                throw new ArgumentNullException();

            row.BillableTimeCalc = null;
            row.BillableOvertimeCalc = null;
            row.RegularTimeCalc = null;
            row.OverTimeCalc = null;

            if (row.IsOvertimeCalc == true)
            {
                row.OverTimeCalc = row.TimeSpent;
	            row.OvertimeSpent = row.TimeSpent;
                if (row.IsBillable == true)
                {
                    row.BillableOvertimeCalc = row.TimeBillable;
					row.OvertimeBillable = row.TimeBillable;
                }
            }
            else
            {
                row.RegularTimeCalc = row.TimeSpent;
                if (row.IsBillable == true)
                {
                    row.BillableTimeCalc = row.TimeBillable;
                }
            }
        }

		public virtual EPTimeCardSummary GetSummaryRecord(EPTimecardDetail activity)
		{
            if (activity == null)
                throw new ArgumentNullException();

	        if (activity.SummaryLineNbr != null)
			{
				return PXSelect<EPTimeCardSummary, Where<EPTimeCardSummary.timeCardCD, Equal<Current<EPTimeCard.timeCardCD>>, And<EPTimeCardSummary.lineNbr, Equal<Required<EPTimeCardSummary.lineNbr>>>>>.Select(this, activity.SummaryLineNbr);
			}

			PXSelectBase<EPTimeCardSummary> select = new PXSelect<
					EPTimeCardSummary, 
				Where<EPTimeCardSummary.timeCardCD, Equal<Current<EPTimeCard.timeCardCD>>,
					And<EPTimeCardSummary.earningType, Equal<Current<EPTimecardDetail.earningTypeID>>,
					And<EPTimeCardSummary.projectID, Equal<Required<EPTimecardDetail.projectID>>,
					And<EPTimeCardSummary.isBillable, Equal<Current<EPTimecardDetail.isBillable>>>>>>>(this);

			if (activity.ParentTaskNoteID != null)
			{
				select.WhereAnd<Where<EPTimeCardSummary.parentNoteID, Equal<Current<EPTimecardDetail.parentTaskNoteID>>>>();
			}
			else
			{
				select.WhereAnd<Where<EPTimeCardSummary.parentNoteID, IsNull>>();
			}

			if (activity.JobID != null)
			{
				select.WhereAnd<Where<EPTimeCardSummary.jobID, Equal<Current<EPTimecardDetail.jobID>>>>();
			}
			else
			{
				select.WhereAnd<Where<EPTimeCardSummary.jobID, IsNull>>();
			}

			if (activity.ShiftID != null)
			{
				select.WhereAnd<Where<EPTimeCardSummary.shiftID, Equal<Current<EPTimecardDetail.shiftID>>>>();
			}
			else
			{
				select.WhereAnd<Where<EPTimeCardSummary.shiftID, IsNull>>();
			}

			if (activity.ProjectTaskID != null)
			{
                select.WhereAnd<Where<EPTimeCardSummary.projectTaskID, Equal<Current<EPTimecardDetail.projectTaskID>>>>();
			}
			int projectID = activity.ProjectID.GetValueOrDefault(ProjectDefaultAttribute.NonProject(this).GetValueOrDefault(0));
			return (EPTimeCardSummary)select.View.SelectSingleBound(new object[] { Document.Current, activity }, projectID);
		}

		public virtual EPTimeCardSummary AddToSummary(EPTimeCardSummary summary, PMTimeActivity activity)
		{
			return AddToSummary(summary, activity, 1);
		}

		public virtual void SubtractFromSummary(EPTimeCardSummary summary, PMTimeActivity activity)
		{
            summary = AddToSummary(summary, activity, -1);

            try
            {
                dontSyncDetails = true;
                if (summary != null && summary.TimeSpent.Value == 0)
                {
                    Summary.Delete(summary);//cascadly will delete detail record through PXParent.
                }

            }
            finally
            {
                dontSyncDetails = false;
            }
			
		}

		public virtual EPTimeCardSummary AddToSummary(EPTimeCardSummary summary, PMTimeActivity activity, int mult)
		{
            if (activity == null)
                throw new ArgumentNullException();

            //Debug.Print("AddToSummary Mult:{0} Start", mult);
            //Debug.Indent();

            if (activity.TimeSpent.GetValueOrDefault() == 0)
            {
                Debug.Unindent();
                //Debug.Print("Activity is empty. Exiting AddToSummary");
                return null;
            }

            if (activity.ProjectID == null)
            {
                Debug.Unindent();
                //Debug.Print("Activity.ProjectID is empty. Exiting AddToSummary");
                return null;
            }
            
			if (activity.ProjectTaskID == null && !ProjectDefaultAttribute.IsNonProject(this, activity.ProjectID))
			{
				//do not add to summary with empty TaskID
				return null;
			}

			if (summary == null)
			{
				summary = (EPTimeCardSummary)Summary.Cache.CreateInstance();
				summary.EarningType = activity.EarningTypeID;
				summary.ParentNoteID = activity.ParentTaskNoteID;
				summary.ProjectID = activity.ProjectID;
				summary.ProjectTaskID = activity.ProjectTaskID;
				summary.IsBillable = activity.IsBillable;
				summary.JobID = activity.JobID;
				summary.ShiftID = activity.ShiftID;


				if (activity.ParentTaskNoteID != null)
				{
					CRActivity parentTask = PXSelect<CRActivity,
						Where<CRActivity.noteID, Equal<Required<PMTimeActivity.parentTaskNoteID>>>>
						.Select(this, activity.ParentTaskNoteID);

					if (parentTask != null)
						summary.Description = parentTask.Subject;
				}
				else
				{
					PMTask pmTask = PXSelect<PMTask,
						Where<PMTask.projectID, Equal<Required<PMTask.projectID>>,
							And<PMTask.noteID, Equal<Required<PMTask.noteID>>>>>
						.Select(this, activity.ProjectID, activity.ParentTaskNoteID);

					if (pmTask != null)
						summary.Description = pmTask.Description;
				}

				dontSyncDetails = true;
				try
				{
					summary = Summary.Insert(summary);
				    if (summary == null) return null;				   
				    summary.ProjectID = activity.ProjectID; //may have been overriden in rowInserting()
				    summary.ProjectTaskID = activity.ProjectTaskID; //may have been overriden in rowInserting()				    
				}
				finally
				{
					dontSyncDetails = false;
				}
			}

            AddActivityTimeToSummary(summary, activity, mult);

		    try
			{
				dontSyncDetails = true;
				summary = Summary.Update(summary);
			}
			finally
			{
				dontSyncDetails = false;
			}

            //Debug.Unindent();
            //Debug.Print("AddToSummary End");
            

			return summary;
		}

		public virtual void AddActivityTimeToSummary(EPTimeCardSummary summary, PMTimeActivity activity, int mult)
        {
			if (activity.TimeSpent != null && activity.Date != null)
            {
                switch (activity.Date.Value.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        summary.Mon = summary.Mon.GetValueOrDefault() + (mult * activity.TimeSpent.Value);
                        break;
                    case DayOfWeek.Tuesday:
                        summary.Tue = summary.Tue.GetValueOrDefault() + (mult * activity.TimeSpent.Value);
                        break;
                    case DayOfWeek.Wednesday:
                        summary.Wed = summary.Wed.GetValueOrDefault() + (mult * activity.TimeSpent.Value);
                        break;
                    case DayOfWeek.Thursday:
                        summary.Thu = summary.Thu.GetValueOrDefault() + (mult * activity.TimeSpent.Value);
                        break;
                    case DayOfWeek.Friday:
                        summary.Fri = summary.Fri.GetValueOrDefault() + (mult * activity.TimeSpent.Value);
                        break;
                    case DayOfWeek.Saturday:
                        summary.Sat = summary.Sat.GetValueOrDefault() + (mult * activity.TimeSpent.Value);
                        break;
                    case DayOfWeek.Sunday:
                        summary.Sun = summary.Sun.GetValueOrDefault() + (mult * activity.TimeSpent.Value);
                        break;
                }
            }
        }

		public virtual void UpdateAdjustingActivities(EPTimeCardSummary summary)
	    {
		    UpdateAdjustingActivities(summary, false);
	    }
		public virtual void UpdateAdjustingActivities(EPTimeCardSummary summary, bool skipDescriptionUpdate)
		{
            if (summary == null)
                throw new ArgumentNullException();

            EPTimeCard doc = PXSelect<EPTimeCard, Where<EPTimeCard.timeCardCD, Equal<Required<EPTimeCard.timeCardCD>>>>.Select(this, summary.TimeCardCD);
            if (doc == null)
                return;

            Dictionary<DayOfWeek, DayActivities> dict = GetActivities(summary, doc);
            
            PX.Objects.EP.PXWeekSelector2Attribute.WeekInfo weekInfo = PXWeekSelector2Attribute.GetWeekInfo(this, doc.WeekID.Value);

			if (weekInfo.Mon.Enabled)
				UpdateAdjustingActivities(summary, dict, DayOfWeek.Monday, SetEmployeeTime(weekInfo.Mon.Date).Value, skipDescriptionUpdate);
            if (weekInfo.Tue.Enabled)
				UpdateAdjustingActivities(summary, dict, DayOfWeek.Tuesday, SetEmployeeTime(weekInfo.Tue.Date).Value, skipDescriptionUpdate);
            if (weekInfo.Wed.Enabled)
				UpdateAdjustingActivities(summary, dict, DayOfWeek.Wednesday, SetEmployeeTime(weekInfo.Wed.Date).Value, skipDescriptionUpdate);
            if (weekInfo.Thu.Enabled)
				UpdateAdjustingActivities(summary, dict, DayOfWeek.Thursday, SetEmployeeTime(weekInfo.Thu.Date).Value, skipDescriptionUpdate);
            if (weekInfo.Fri.Enabled)
				UpdateAdjustingActivities(summary, dict, DayOfWeek.Friday, SetEmployeeTime(weekInfo.Fri.Date).Value, skipDescriptionUpdate);
            if (weekInfo.Sat.Enabled)
				UpdateAdjustingActivities(summary, dict, DayOfWeek.Saturday, SetEmployeeTime(weekInfo.Sat.Date).Value, skipDescriptionUpdate);
            if (weekInfo.Sun.Enabled)
				UpdateAdjustingActivities(summary, dict, DayOfWeek.Sunday, SetEmployeeTime(weekInfo.Sun.Date).Value, skipDescriptionUpdate);
		}

		public virtual DateTime? SetEmployeeTime(DateTime? date)
        {
            EPEmployee emp = Employee.Select();
            if (emp != null)
            {
				CSCalendar cal = PXSelect<CSCalendar, Where<CSCalendar.calendarID, Equal<Required<CSCalendar.calendarID>>>>.Select(this, emp.CalendarID);
                if (cal != null)
                {
                    switch (date.Value.DayOfWeek)
                    {
                       case DayOfWeek.Monday: return PXDBDateAndTimeAttribute.CombineDateTime(date, cal.MonStartTime);
                       case DayOfWeek.Tuesday: return PXDBDateAndTimeAttribute.CombineDateTime(date, cal.TueStartTime);
                       case DayOfWeek.Wednesday: return PXDBDateAndTimeAttribute.CombineDateTime(date, cal.WedStartTime);
                       case DayOfWeek.Thursday: return PXDBDateAndTimeAttribute.CombineDateTime(date, cal.ThuStartTime);
                       case DayOfWeek.Friday: return PXDBDateAndTimeAttribute.CombineDateTime(date, cal.FriStartTime);
                       case DayOfWeek.Saturday: return PXDBDateAndTimeAttribute.CombineDateTime(date, cal.SatStartTime);
                       case DayOfWeek.Sunday: return PXDBDateAndTimeAttribute.CombineDateTime(date, cal.SunStartTime);
                    }
                }
            }

			return PXDateAndTimeAttribute.CombineDateTime(date, new DateTime(2008, 1, 1, 9, 0, 0));//use default 9:00
        }


		public virtual void UpdateAdjustingActivities(EPTimeCardSummary summary, Dictionary<DayOfWeek, DayActivities> dict, DayOfWeek dayOfWeek, DateTime startDate, bool skipDescriptionUpdate)
        {
            if (summary == null)
                throw new ArgumentNullException("summary");

            if (dict == null)
                throw new ArgumentNullException("dict");

            //Debug.Print("UpdateAdjustingActivities for {0} Start", dayOfWeek);
            Debug.Indent();

            int summaryTimeTotal = 0;

            if (Summary.Cache.GetStatus(summary) != PXEntryStatus.Deleted && Summary.Cache.GetStatus(summary) != PXEntryStatus.InsertedDeleted)
                summaryTimeTotal = summary.GetTimeTotal(dayOfWeek).GetValueOrDefault();

            int dayTotal = 0;
            if (dict.ContainsKey(dayOfWeek))
            {
                dayTotal = dict[dayOfWeek].GetTotalTime(summary.LineNbr.Value);
            }

            EPTimecardDetail adjust = null;
            if (dict.ContainsKey(dayOfWeek))
            {
                adjust = dict[dayOfWeek].GetAdjustingActivity(summary.LineNbr.Value);
            }

            if (summaryTimeTotal != dayTotal)
            {
                if (adjust == null && summaryTimeTotal - dayTotal != 0)
                {
                    dontSyncSummary = true;
                    try
                    {
                        adjust = Activities.Insert();

						// CR
                        adjust.Date = startDate;
                        if (!string.IsNullOrEmpty(summary.Description))
	                    {
                            adjust.Summary = summary.Description;
	                    }
                        else
                        {
							adjust.Summary = string.Format("Summary {0} Activities", dayOfWeek);
                        }
						adjust.ParentTaskNoteID = summary.ParentNoteID;
						adjust.ApprovalStatus = ActivityStatusAttribute.Completed;

						// PM
						adjust.EarningTypeID = summary.EarningType;
						adjust.JobID = summary.JobID;
						adjust.ShiftID = summary.ShiftID;
						adjust.SummaryLineNbr = summary.LineNbr;
						adjust.Day = ((int)adjust.Date.Value.DayOfWeek).ToString();
						adjust.IsBillable = summary.IsBillable;
                        adjust.TimeSpent = (summaryTimeTotal - dayTotal);
                        if (adjust.IsBillable == true)
                            adjust.TimeBillable = adjust.TimeSpent;
                        adjust.ProjectID = summary.ProjectID;
                        adjust.ProjectTaskID = summary.ProjectTaskID;
						Activities.Cache.SetDefaultExt<EPTimecardDetail.approverID>(adjust);
						Activities.Cache.SetDefaultExt<EPTimecardDetail.approvalStatus>(adjust);

                        InitEarningType(adjust);
                        RecalculateFields(adjust);
                    }
                    finally
                    {
                        dontSyncSummary = false;
                    }
                }
                else if (adjust != null && summaryTimeTotal == 0 && adjust.SummaryLineNbr != null)//delete only adjusting activity that was added automatically on summary update.
                {
                    dontSyncSummary = true;
                    try
                    {
                        Activities.Delete(adjust);
                    }
                    finally
                    {
                        dontSyncSummary = false;
                    }
                }
                else if (adjust != null)
                {
                    adjust.TimeSpent = adjust.TimeSpent + summaryTimeTotal - dayTotal;
                    if (!string.IsNullOrEmpty(summary.Description))
	                {
						if (!skipDescriptionUpdate || string.IsNullOrEmpty(adjust.Summary))
                        adjust.Summary = summary.Description;
	                }
                    adjust.IsBillable = summary.IsBillable;
					if (adjust.IsBillable == true)
						adjust.TimeBillable = adjust.TimeSpent;
					RecalculateFields(adjust);

                    dontSyncSummary = true;
                    try
                    {
                        adjust = Activities.Update(adjust);
                    }
                    finally
                    {
                        dontSyncSummary = false;
                    }

                }
            }
            else
            {
                if (adjust != null)
                {
					if (!string.IsNullOrEmpty(summary.Description) && !skipDescriptionUpdate)
                    {
                        Activities.Cache.SetValue<EPTimecardDetail.summary>(adjust, summary.Description);
                        if (Activities.Cache.GetStatus(adjust) == PXEntryStatus.Notchanged)
                            Activities.Cache.SetStatus(adjust, PXEntryStatus.Updated);
                    }
                }
            }

            Debug.Unindent();
            //Debug.Print("UpdateAdjustingActivities for {0} End", dayOfWeek);
        }

		public virtual Dictionary<DayOfWeek, DayActivities> GetActivities(EPTimeCardSummary summary, EPTimeCard doc)
		{
            if (summary == null)
                throw new ArgumentNullException("summary");
            if (doc == null)
                throw new ArgumentNullException("doc");

			Dictionary<DayOfWeek, DayActivities> dict = new Dictionary<DayOfWeek, DayActivities>();

            EPTimeCardSummary duplicate = FindDuplicate(summary);

            foreach (EPTimecardDetail activity in GetDetails(summary, doc, false))
			{
				if (duplicate != null && AreTheseConnected(duplicate, activity))
                    continue;

                DayOfWeek day = activity.Date.Value.DayOfWeek;

				if (dict.ContainsKey(activity.Date.Value.DayOfWeek))
				{
					dict[activity.Date.Value.DayOfWeek].Activities.Add(activity);
				}
				else
				{
					DayActivities d = new DayActivities();
					d.Day = day;
					d.Activities.Add(activity);
					dict.Add(activity.Date.Value.DayOfWeek, d);
				}
			}

			return dict;
		}
		public virtual bool AreTheseConnected(EPTimeCardSummary summary, EPTimecardDetail detail)
	    {
		    if (detail.SummaryLineNbr != null)
		    {
			    return detail.SummaryLineNbr == summary.LineNbr;
		    }
		    if (summary.EarningType != detail.EarningTypeID)
			    return false;
			if (summary.JobID != detail.JobID)
				return false;
			if (summary.ShiftID != detail.ShiftID)
				return false;
		    if (summary.ProjectID != detail.ProjectID)
			    return false;
		    if (summary.ProjectTaskID != detail.ProjectTaskID)
			    return false;
			if (summary.ParentNoteID != null)
			{
				return summary.ParentNoteID == detail.ParentTaskNoteID;
			}
		    return true;
		}

		public virtual EPTimeCardSummary FindDuplicate(EPTimeCardSummary summary)
        {
            EPTimeCardSummary duplicate;
            PXSelectBase<EPTimeCardSummary> select = new PXSelect<EPTimeCardSummary,
                Where<EPTimeCardSummary.timeCardCD, Equal<Current<EPTimeCardSummary.timeCardCD>>,
                    And<EPTimeCardSummary.earningType, Equal<Current<EPTimeCardSummary.earningType>>,
                    And<EPTimeCardSummary.isBillable, Equal<Current<EPTimeCardSummary.isBillable>>,
                    And<EPTimeCardSummary.lineNbr, NotEqual<Current<EPTimeCardSummary.lineNbr>>>>>>>(this);

			if (summary.ParentNoteID != null)
            {
				select.WhereAnd<Where<EPTimeCardSummary.parentNoteID, Equal<Current<EPTimeCardSummary.parentNoteID>>>>();
            }
			
			if (summary.JobID != null)
			{
				select.WhereAnd<Where<EPTimeCardSummary.jobID, Equal<Current<EPTimeCardSummary.jobID>>>>();
			}
			else
			{
				select.WhereAnd<Where<EPTimeCardSummary.jobID, IsNull>>();
			}

			if (summary.ShiftID != null)
			{
				select.WhereAnd<Where<EPTimeCardSummary.shiftID, Equal<Current<EPTimeCardSummary.shiftID>>>>();
			}
			else
			{
				select.WhereAnd<Where<EPTimeCardSummary.shiftID, IsNull>>();
			}


            if (summary.ProjectID != null)
            {
                select.WhereAnd<Where<EPTimeCardSummary.projectID, Equal<Current<EPTimeCardSummary.projectID>>>>();
                if (summary.ProjectTaskID != null)
                {
                    select.WhereAnd<Where<EPTimeCardSummary.projectTaskID, Equal<Current<EPTimeCardSummary.projectTaskID>>>>();
                }
                duplicate = (EPTimeCardSummary)select.View.SelectSingleBound(new object[] { summary });
            }
            else
            {
                select.WhereAnd<Where<EPTimeCardSummary.projectID, IsNull, Or<EPTimeCardSummary.projectID, Equal<Required<EPTimeCardSummary.projectID>>>>>();
				duplicate = (EPTimeCardSummary)select.View.SelectSingleBound(new object[] { summary }, new object[] { PM.ProjectDefaultAttribute.NonProject(this) });
            }

            return duplicate;
        }

		public virtual List<EPTimecardDetail> GetDetails(EPTimeCardSummary summary, EPTimeCard doc, bool onlyManual)
        {
			//Stopwatch sw = new Stopwatch();
			//sw.Start();

            if (summary == null)
                throw new ArgumentNullException("summary");
            if (doc == null)
                throw new ArgumentNullException("doc");

            PXSelectBase<EPTimecardDetail> select = new PXSelectJoin<EPTimecardDetail,
            InnerJoin<CREmployee, 
				On<CREmployee.userID, Equal<EPTimecardDetail.ownerID>>>,
			Where<CREmployee.bAccountID, Equal<Current<EPTimeCard.employeeID>>,
				And<EPTimecardDetail.earningTypeID, Equal<Current<EPTimeCardSummary.earningType>>,
				And<EPTimecardDetail.isBillable, Equal<Current<EPTimeCardSummary.isBillable>>,
				And<EPTimecardDetail.weekID, Equal<Current<EPTimeCard.weekId>>,
				And<EPTimecardDetail.trackTime, Equal<True>,
				And<Where<EPTimecardDetail.timeCardCD, IsNull, Or<EPTimecardDetail.timeCardCD, Equal<Current<EPTimeCard.timeCardCD>>>>>>>>>>>(this);

            if (onlyManual == true)
            {
                select.WhereAnd<Where<EPTimecardDetail.summaryLineNbr, IsNull>>();
            }

            List<object> resultset;
			if (summary.ParentNoteID != null)
            {
				select.WhereAnd<Where<EPTimecardDetail.parentTaskNoteID, Equal<Current<EPTimeCardSummary.parentNoteID>>>>();
            }
            else
            {
				select.WhereAnd<Where<EPTimecardDetail.parentTaskNoteID, IsNull>>();
            }

			if (summary.JobID != null)
			{
				select.WhereAnd<Where<EPTimecardDetail.jobID, Equal<Current<EPTimeCardSummary.jobID>>>>();
			}
			else
			{
				select.WhereAnd<Where<EPTimecardDetail.jobID, IsNull>>();
			}

			if (summary.ShiftID != null)
			{
				select.WhereAnd<Where<EPTimecardDetail.shiftID, Equal<Current<EPTimeCardSummary.shiftID>>>>();
			}
			else
			{
				select.WhereAnd<Where<EPTimecardDetail.shiftID, IsNull>>();
			}

            if (summary.ProjectID != null)
            {
                select.WhereAnd<Where<EPTimecardDetail.projectID, Equal<Current<EPTimeCardSummary.projectID>>>>();
                if (summary.ProjectTaskID != null)
                {
                    select.WhereAnd<Where<EPTimecardDetail.projectTaskID, Equal<Current<EPTimeCardSummary.projectTaskID>>>>();
                }
                else
                {
                    select.WhereAnd<Where<EPTimecardDetail.projectTaskID, IsNull>>();
                }
                resultset = select.View.SelectMultiBound(new object[] { summary, doc });
            }
            else
            {
                select.WhereAnd<Where<EPTimecardDetail.projectID, IsNull, Or<EPTimecardDetail.projectID, Equal<Required<EPTimecardDetail.projectID>>>>>();
                resultset = select.View.SelectMultiBound(new object[] { summary, doc }, new object[] { PM.ProjectDefaultAttribute.NonProject(this) });
            }

            List<EPTimecardDetail> result = new List<EPTimecardDetail>(resultset.Count);
            foreach (PXResult<EPTimecardDetail, CREmployee> item in resultset)
            {
                result.Add((EPTimecardDetail)item);
            }

			//sw.Stop();
			//Debug.Print("GetDetails() in {0} ms.", sw.ElapsedMilliseconds);

            return result;
        }

		public virtual void RecalculateTotals(EPTimeCard timecard)
        {
            if (timecard == null)
                throw new ArgumentNullException();

            List<EPTimecardDetail> list = new List<EPTimecardDetail>();

            foreach (EPTimecardDetail detail in Activities.Select())
            {
               list.Add(detail);
            }

            RecalculateTotals(timecard, list);
        }

		public virtual void RecalculateTotals(EPTimeCard timecard, List<EPTimecardDetail> details)
        {
            if (timecard == null)
                throw new ArgumentNullException("timecard");

            if (details == null)
                throw new ArgumentNullException("details");

            int timeSpent = 0;
            int regularTime = 0;
            int overtimeSpent = 0;
            int timeBillable = 0;
            int overtimeBillable = 0;

            foreach (EPTimecardDetail detail in details)
            {
                timeSpent += detail.TimeSpent.GetValueOrDefault();
                regularTime += detail.RegularTimeCalc.GetValueOrDefault();
                timeBillable += detail.BillableTimeCalc.GetValueOrDefault();
                overtimeSpent += detail.OverTimeCalc.GetValueOrDefault();
                overtimeBillable += detail.BillableOvertimeCalc.GetValueOrDefault();
            }


            timecard.TimeSpentCalc = regularTime;
            timecard.OvertimeSpentCalc = overtimeSpent;
            timecard.TotalSpentCalc = timeSpent;

            timecard.TimeBillableCalc = timeBillable;
            timecard.OvertimeBillableCalc = overtimeBillable;
            timecard.TotalBillableCalc = timeBillable + overtimeBillable;
        }



		public virtual bool ValidateTotals(EPTimeCard timecard, out string errorMsg)
        {
            if (timecard == null)
                throw new ArgumentNullException();

            bool valid = true;
            errorMsg = null;

            EPEmployee employee = PXSelect<EPEmployee, Where<EPEmployee.bAccountID, Equal<Required<EPEmployee.bAccountID>>>>.Select(this, timecard.EmployeeID);
            if (employee == null || employee.HoursValidation == HoursValidationOption.None)
            {
                return true;//always valid
            }

            PXUIFieldAttribute.SetError<EPTimeCard.timeSpentCalc>(Document.Cache, timecard, null);
            PXUIFieldAttribute.SetError<EPTimeCard.overtimeSpentCalc>(Document.Cache, timecard, null);
            PXUIFieldAttribute.SetError<EPTimeCard.totalSpentCalc>(Document.Cache, timecard, null);

            DateTime date = Accessinfo.BusinessDate ?? DateTime.Now;
	        bool isFullWeek = true;
			if (timecard.WeekID != null)
			{
				EPWeekRaw week = PXSelectorAttribute.Select<EPTimeCard.weekId>(Document.Cache, timecard) as EPWeekRaw;
				if (week != null)
				{
					date = week.StartDate.Value;
					isFullWeek = week.IsFullWeek.Value;
				}
			}

			EPEmployeeRate rate = CreateEmployeeCostEngine().GetEmployeeRate(timecard.EmployeeID, date);
			if (isFullWeek && (rate == null || rate.RegularHours == null))
            {
                valid = false;
                errorMsg = PXMessages.LocalizeNoPrefix(Messages.RGIsNotDefinedForEmployee);
                Document.Cache.RaiseExceptionHandling<EPTimeCard.employeeID>(timecard, timecard.EmployeeID, new PXSetPropertyException(errorMsg, PXErrorLevel.Warning));
            }
            else if (isFullWeek && (timecard.TimeSpentCalc > (rate.RegularHours * 60)))
            {
                valid = false;
                errorMsg = PXMessages.LocalizeFormatNoPrefixNLA(Messages.TotalTimeForWeekCannotExceedHours, rate.RegularHours);
                Document.Cache.RaiseExceptionHandling<EPTimeCard.timeSpentCalc>(timecard, timecard.TimeSpentCalc, new PXSetPropertyException(errorMsg, PXErrorLevel.Warning));
            }
			else if (isFullWeek && (timecard.TimeSpentCalc < (rate.RegularHours * 60)))
            {
                valid = false;

                if (timecard.OvertimeSpentCalc > 0)
                {
                    errorMsg = PXMessages.LocalizeFormatNoPrefixNLA(Messages.OvertimeNotAllowed, rate.RegularHours);
                    Document.Cache.RaiseExceptionHandling<EPTimeCard.overtimeSpentCalc>(timecard, timecard.OvertimeSpentCalc, new PXSetPropertyException(errorMsg, PXErrorLevel.Warning));
                }
                else
                {
                    errorMsg = PXMessages.LocalizeFormatNoPrefixNLA(Messages.TimecradIsNotNormalized, rate.RegularHours);
                    Document.Cache.RaiseExceptionHandling<EPTimeCard.totalSpentCalc>(timecard, timecard.TotalSpentCalc, new PXSetPropertyException(errorMsg, PXErrorLevel.Warning));
                }
                
            }

            return valid;
        }
					
		public virtual PM.PMTran CreateItemTransaction(EPTimeCardItem record, int? employeeID, DateTime? date, decimal? qty, PXCache tranCache)
        {
            InventoryItem nsItem = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, record.InventoryID);
            if (nsItem == null)
            {
                throw new PXException(Messages.InventoryItemIsEmpty);
            }

            if (nsItem.InvtAcctID == null)
            {
                throw new PXException(Messages.ExpenseAccrualIsRequired, nsItem.InventoryCD.Trim());
            }

            if (nsItem.InvtSubID == null)
            {
                throw new PXException(Messages.ExpenseAccrualSubIsRequired, nsItem.InventoryCD.Trim());
            }

            Contract contract = PXSelect<Contract, Where<Contract.contractID, Equal<Required<Contract.contractID>>>>.Select(this, record.ProjectID);

            decimal? cost = 0m;

            int? accountID = nsItem.COGSAcctID;
            int? offsetaccountID = nsItem.InvtAcctID;
            int? accountGroupID = null;
            string subCD = null;
            string offsetSubCD = null;

            int? branchID = null;
            EP.EPEmployee emp = PXSelect<EP.EPEmployee, Where<EP.EPEmployee.bAccountID, Equal<Required<EP.EPEmployee.bAccountID>>>>.Select(this, employeeID);
            if (emp != null)
            {
                Branch branch = PXSelect<Branch, Where<Branch.bAccountID, Equal<Required<EPEmployee.parentBAccountID>>>>.Select(this, emp.ParentBAccountID);
                if (branch != null)
                {
                    branchID = branch.BranchID;
                }
            }

            if (contract.BaseType == PMProject.ProjectBaseType.Project)//contract do not record money only usage.
            {
                cost = nsItem.StdCost;

                if (contract.NonProject != true)
                {
                    PMTask task = PXSelect<PMTask, Where<PMTask.projectID, Equal<Required<PMTask.projectID>>, And<PMTask.taskID, Equal<Required<PMTask.taskID>>>>>.Select(this, record.ProjectID, record.TaskID);

                    #region Combine Account and Subaccount

                    if (ExpenseAccountSource == PMAccountSource.Project)
                    {
                        if (contract.DefaultAccountID != null)
                        {
                            accountID = contract.DefaultAccountID;
                            Account account = PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this, accountID);
                            if (account.AccountGroupID == null)
                            {
                                throw new PXException(Messages.NoAccountGroupOnProject, account.AccountCD.Trim(), contract.ContractCD.Trim());
                            }
                            accountGroupID = account.AccountGroupID;
                        }
                        else
                        {
                            PXTrace.WriteWarning(Messages.NoDefualtAccountOnProject, contract.ContractCD.Trim());
                        }
                    }
                    else if (ExpenseAccountSource == PMAccountSource.Task)
                    {

                        if (task.DefaultAccountID != null)
                        {
                            accountID = task.DefaultAccountID;
                            Account account = PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this, accountID);
                            if (account.AccountGroupID == null)
                            {
                                throw new PXException(Messages.NoAccountGroupOnTask, account.AccountCD.Trim(), contract.ContractCD.Trim(), task.TaskCD.Trim());
                            }
                            accountGroupID = account.AccountGroupID;
                        }
                        else
                        {
                            PXTrace.WriteWarning(Messages.NoDefualtAccountOnTask, contract.ContractCD.Trim(), task.TaskCD.Trim());
                        }
                    }
                    else if (ExpenseAccountSource == PMAccountSource.Resource)
                    {
                        if (emp.ExpenseAcctID != null)
                        {
                            accountID = emp.ExpenseAcctID;
                            Account account = PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this, accountID);
                            if (account.AccountGroupID == null)
                            {
                                throw new PXException(Messages.NoAccountGroupOnEmployee, account.AccountCD, emp.AcctCD.Trim());
                            }
                            accountGroupID = account.AccountGroupID;
                        }
                        else
                        {
                            PXTrace.WriteWarning(Messages.NoExpenseAccountOnEmployee, emp.AcctCD.Trim());
                        }
                    }
                    else
                    {
                        if (accountID == null)
                        {
                            throw new PXException(Messages.NoExpenseAccountOnInventory, nsItem.InventoryCD.Trim());
                        }

                        //defaults to InventoryItem.COGSAcctID
                        Account account = PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this, accountID);
                        if (account.AccountGroupID == null)
                        {
                            throw new PXException(Messages.NoAccountGroupOnInventory, account.AccountCD.Trim(), nsItem.InventoryCD.Trim());
                        }
                        accountGroupID = account.AccountGroupID;
                    }


                    if (accountGroupID == null)
                    {
                        //defaults to InventoryItem.COGSAcctID
                        Account account = PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this, accountID);
                        if (account.AccountGroupID == null)
                        {
                            throw new PXException(Messages.AccountGroupIsNotAssignedForAccount, account.AccountCD.Trim());
                        }
                        accountGroupID = account.AccountGroupID;
                    }


                    if (!string.IsNullOrEmpty(ExpenseSubMask))
                    {
                        if (ExpenseSubMask.Contains(PMAccountSource.InventoryItem) && nsItem.COGSSubID == null)
                        {
                            throw new PXException(Messages.NoExpenseSubOnInventory, nsItem.InventoryCD.Trim());
                        }
                        if (ExpenseSubMask.Contains(PMAccountSource.Project) && contract.DefaultSubID == null)
                        {
                            throw new PXException(Messages.NoExpenseSubOnProject, contract.ContractCD.Trim());
                        }
                        if (ExpenseSubMask.Contains(PMAccountSource.Task) && task.DefaultSubID == null)
                        {
                            throw new PXException(Messages.NoExpenseSubOnTask, contract.ContractCD.Trim(), task.TaskCD.Trim());
                        }
                        if (ExpenseSubMask.Contains(PMAccountSource.Resource) && emp.ExpenseSubID == null)
                        {
                            throw new PXException(Messages.NoExpenseSubOnEmployee, emp.AcctCD.Trim());
                        }


                        subCD = PM.SubAccountMaskAttribute.MakeSub<PMSetup.expenseSubMask>(this, ExpenseSubMask,
                            new object[] { nsItem.COGSSubID, contract.DefaultSubID, task.DefaultSubID, emp.ExpenseSubID },
                            new Type[] { typeof(InventoryItem.cOGSSubID), typeof(Contract.defaultSubID), typeof(PMTask.defaultSubID), typeof(EPEmployee.expenseSubID) });
                    }

                    #endregion

                    #region Combine Accrual Account and Subaccount

                    if (ExpenseAccrualAccountSource == PMAccountSource.Project)
                    {
                        if (contract.DefaultAccrualAccountID != null)
                        {
                            offsetaccountID = contract.DefaultAccrualAccountID;
                        }
                        else
                        {
                            PXTrace.WriteWarning(EP.Messages.NoDefualtAccrualAccountOnProject, contract.ContractCD.Trim());
                        }
                    }
                    else if (ExpenseAccrualAccountSource == PMAccountSource.Task)
                    {
                        if (task.DefaultAccrualAccountID != null)
                        {
                            offsetaccountID = task.DefaultAccrualAccountID;
                        }
                        else
                        {
                            PXTrace.WriteWarning(EP.Messages.NoDefualtAccountOnTask, contract.ContractCD.Trim(), task.TaskCD.Trim());
                        }
                    }
                    else
                    {
                        if (offsetaccountID == null)
                        {
                            throw new PXException(EP.Messages.NoAccrualExpenseAccountOnInventory, nsItem.InventoryCD.Trim());
                        }
                    }

                    if (!string.IsNullOrEmpty(ExpenseAccrualSubMask))
                    {
                        if (ExpenseAccrualSubMask.Contains(PMAccountSource.InventoryItem) && nsItem.InvtSubID == null)
                        {
                            throw new PXException(EP.Messages.NoExpenseAccrualSubOnInventory, nsItem.InventoryCD.Trim());
                        }
                        if (ExpenseAccrualSubMask.Contains(PMAccountSource.Project) && contract.DefaultAccrualSubID == null)
                        {
                            throw new PXException(EP.Messages.NoExpenseAccrualSubOnProject, contract.ContractCD.Trim());
                        }
                        if (ExpenseAccrualSubMask.Contains(PMAccountSource.Task) && task.DefaultAccrualSubID == null)
                        {
                            throw new PXException(EP.Messages.NoExpenseAccrualSubOnTask, contract.ContractCD.Trim(), task.TaskCD.Trim());
                        }
						if (ExpenseAccrualSubMask.Contains(PMAccountSource.Resource) && emp.ExpenseSubID == null)
						{
							throw new PXException(Messages.NoExpenseSubOnEmployee, emp.AcctCD.Trim());
						}

                        offsetSubCD = PM.SubAccountMaskAttribute.MakeSub<PMSetup.expenseAccrualSubMask>(this, ExpenseAccrualSubMask,
							new object[] { nsItem.InvtSubID, contract.DefaultAccrualSubID, task.DefaultAccrualSubID, emp.ExpenseSubID },
							new Type[] { typeof(InventoryItem.invtSubID), typeof(Contract.defaultAccrualSubID), typeof(PMTask.defaultAccrualSubID), typeof(EPEmployee.expenseSubID) });
                    }

                    #endregion
                }
                else
                {
                    //defaults to InventoryItem.COGSAcctID
                    Account account = PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this, accountID);
                    if (account.AccountGroupID != null)
                    {
						accountGroupID = account.AccountGroupID;
					}
                }
            }

            int? subID = nsItem.COGSSubID;
            int? offsetSubID = nsItem.InvtSubID;
            EPSetup epsetup = PXSelect<EPSetup>.Select(this);
            if (epsetup != null && epsetup.PostToOffBalance == true)
            {
                accountGroupID = epsetup.OffBalanceAccountGroupID;
                accountID = null;
                offsetaccountID = null;
                offsetSubID = null;
                subCD = null;
                subID = null;
            }

            PMTran tran = (PMTran)tranCache.Insert();
            tran.BranchID = branchID;
            tran.AccountID = accountID;
            if (string.IsNullOrEmpty(subCD))
                tran.SubID = subID;
            if (string.IsNullOrEmpty(offsetSubCD))
                tran.OffsetSubID = offsetSubID;
            tran.AccountGroupID = accountGroupID;
            tran.ProjectID = record.ProjectID;
            tran.TaskID = record.TaskID;
            tran.InventoryID = record.InventoryID;
            tran.ResourceID = employeeID;
            tran.Date = date;
            tran.FinPeriodID = FinPeriodIDAttribute.GetPeriod(tran.Date.Value);
            tran.Qty = qty;
            tran.UOM = record.UOM;
            tran.Billable = true;
            tran.BillableQty = tran.Qty;
			tran.UnitRate = PXDBPriceCostAttribute.Round((decimal)cost);
            tran.Amount = null;
            tran.Description = record.Description;
            tran.OffsetAccountID = offsetaccountID;
            tran.IsQtyOnly = contract.BaseType == Contract.ContractBaseType.Contract;
            
            tran = (PMTran)tranCache.Update(tran);

            if (!string.IsNullOrEmpty(subCD))
                tranCache.SetValueExt<PMTran.subID>(tran, subCD);

            if (!string.IsNullOrEmpty(offsetSubCD))
                tranCache.SetValueExt<PMTran.offsetSubID>(tran, offsetSubCD);

			PXNoteAttribute.CopyNoteAndFiles(Items.Cache, record, tranCache, tran);

            return tran;
        }

		public virtual EmployeeCostEngine CreateEmployeeCostEngine()
        {
            return new EmployeeCostEngine(this);
        }

		protected PXCache CreateInstanceCache<TNode>(Type graphType)
			where TNode : IBqlTable
		{
			if (graphType != null)
			{
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

		public virtual bool IsFirstTimeCard(int? employeeID)
		{
			return employeeID == null ||
				PXSelectReadonly<EPTimeCard,
					Where<EPTimeCard.employeeID, Equal<Required<EPTimeCard.employeeID>>>>.
				SelectWindowed(this, 0, 1, employeeID).Count == 0;
		}

        public virtual int? GetNextWeekID(int? employeeID)
		{
			var isFist = IsFirstTimeCard(employeeID);
			if (!isFist)
			{
				var lastCard = (EPTimeCard)PXSelectReadonly<EPTimeCard,
					Where<EPTimeCard.employeeID, Equal<Required<EPTimeCard.employeeID>>>,
					OrderBy<Desc<EPTimeCard.weekId>>>.
					SelectWindowed(this, 0, 1, employeeID);
				if (lastCard != null && lastCard.WeekID != null)
				{
					return PXWeekSelector2Attribute.GetNextWeekID(this, (int)lastCard.WeekID);
				}
			}
			return Accessinfo.BusinessDate.With(_ => PXWeekSelector2Attribute.GetWeekID(this, _));
		}

        
		public virtual EPTimeCard GetLastCorrection(EPTimeCard source)
	    {
			if (source.IsReleased == true)
			{
				EPTimeCard res = Document.Search<EPTimeCard.origTimeCardCD>(source.TimeCardCD);
				if (res != null)
					return GetLastCorrection(res);
			}
			return source;
	    }

		public bool PrepareImportRow(string viewName, IDictionary keys, IDictionary values)
		{
			return true;
		}

		public bool RowImporting(string viewName, object row)
		{
			return row == null;
		}

		public bool RowImported(string viewName, object row, object oldRow)
		{
			return oldRow == null;
		}

		public void PrepareItems(string viewName, IEnumerable items) { }

	    #endregion

		#region Local Types

		public class DayActivities
		{
            public List<EPTimecardDetail> Activities;
			public DayOfWeek Day;

			public DayActivities()
			{
                Activities = new List<EPTimecardDetail>();
			}

            public int GetTotalTime(int summaryLineNbr)
			{
				int total = 0;
                foreach (EPTimecardDetail activity in Activities)
				{
					if (activity.SummaryLineNbr == null || activity.SummaryLineNbr == summaryLineNbr)
                        total += activity.TimeSpent.GetValueOrDefault();
				}

				return total;
			}

            public EPTimecardDetail GetAdjustingActivity(int summaryLineNbr)
			{
                foreach (EPTimecardDetail activity in Activities)
				{
                    if (activity.SummaryLineNbr == summaryLineNbr && activity.Released != true)
						return activity;
				}

				if (Activities.Count == 1 && Activities[0].SummaryLineNbr == null && Activities[0].Released != true)
                {
                    return Activities[0];
                }

				return null;
			}

		}

		protected class SummaryRecord
		{
			public SummaryRecord(EPTimeCardSummary summary)
			{
				this.Summary = summary;
				LinkedDetails = new List<EPTimecardDetail>();
				NotLinkedDetails = new List<EPTimecardDetail>();
		}

			public EPTimeCardSummary Summary { get; private set; }
			public string SummaryKey { get; set; }
			public List<EPTimecardDetail> LinkedDetails { get; private set; }
			public List<EPTimecardDetail> NotLinkedDetails { get; private set; }

			public EPTimeCardSummary SummariseDetails()
		{
				EPTimeCardSummary keyItem = new EPTimeCardSummary();
            
				if (Summary != null)
				{
					keyItem.TimeCardCD = Summary.TimeCardCD;
					keyItem.LineNbr = Summary.LineNbr;
		}

				foreach (EPTimecardDetail detail in LinkedDetails)
        {
					AddActivityTimeToSummary(keyItem, detail, 1);
				}

				foreach (EPTimecardDetail detail in NotLinkedDetails)
            {
					AddActivityTimeToSummary(keyItem, detail, 1);
            }

				return keyItem;
            }

			public SummaryRecordInfo GetInfo()
			{
				SummaryRecordInfo info = new SummaryRecordInfo();
				foreach (EPTimecardDetail detail in LinkedDetails.Union(NotLinkedDetails))
				{
					if (detail.SummaryLineNbr == null)
					{
						info.HasManualDetails = true;
					}

					if (detail.ApprovalStatus == ActivityStatusListAttribute.Open)
					{
						info.HasOpen = true;
					}

					if (detail.ApprovalStatus == ActivityStatusListAttribute.PendingApproval)
					{
						info.HasCompleted = true;
					}

					if (detail.ApprovalStatus == ActivityStatusListAttribute.Approved)
					{
						info.HasApproved = true;
					}

					if (detail.ApprovalStatus == ActivityStatusListAttribute.Rejected)
					{
						info.HasRejected = true;
					}
					

					if (detail.ApproverID != null)
					{
						info.ApprovalRequired = true;
					}

				}

				return info;
			}

			private void AddActivityTimeToSummary(EPTimeCardSummary summary, PMTimeActivity activity, int mult)
            {
				if (activity.TimeSpent != null)
				{
					switch (activity.Date?.DayOfWeek)
					{
						case DayOfWeek.Monday:
							summary.Mon = summary.Mon.GetValueOrDefault() + (mult*activity.TimeSpent.Value);
							break;
						case DayOfWeek.Tuesday:
							summary.Tue = summary.Tue.GetValueOrDefault() + (mult*activity.TimeSpent.Value);
							break;
						case DayOfWeek.Wednesday:
							summary.Wed = summary.Wed.GetValueOrDefault() + (mult*activity.TimeSpent.Value);
							break;
						case DayOfWeek.Thursday:
							summary.Thu = summary.Thu.GetValueOrDefault() + (mult*activity.TimeSpent.Value);
							break;
						case DayOfWeek.Friday:
							summary.Fri = summary.Fri.GetValueOrDefault() + (mult*activity.TimeSpent.Value);
							break;
						case DayOfWeek.Saturday:
							summary.Sat = summary.Sat.GetValueOrDefault() + (mult*activity.TimeSpent.Value);
							break;
						case DayOfWeek.Sunday:
							summary.Sun = summary.Sun.GetValueOrDefault() + (mult*activity.TimeSpent.Value);
							break;
					}
				}
            }

			public struct SummaryRecordInfo
			{
				public bool HasManualDetails { get; set; }
				public bool HasOpen { get; set; }
				public bool HasApproved { get; set; }
				public bool HasRejected { get; set; }
				public bool HasCompleted { get; set; }
				public bool ApprovalRequired { get; set; }

			}
		}
			
		private class ReleasedActivity
			{
			public EPTimecardDetail Activity { get; private set; }
			public decimal Cost { get; private set; }
			public bool IsBilled { get; private set; }

			public ReleasedActivity(EPTimecardDetail activity, decimal cost, bool isBilled)
				{
				this.Activity = activity;
				this.Cost = cost;
				this.IsBilled = isBilled;
				}
		}

	    /// <summary>
	    /// Required for correct join
	    /// </summary>
	    [PXHidden]
	    [Serializable]
	    public class EPTimecardDetailOrig : EPTimecardDetail
	    {
		    public new abstract class noteID : IBqlField { }
			public new abstract class refNoteID : IBqlField { }
			public new abstract class origNoteID : IBqlField {}
	    }

	    /// <summary>
		/// Required for correct join
		/// </summary>
		[PXHidden]
		[Serializable]
		public class EPTimecardDetailEx : EPTimecardDetail
		{
			public new abstract class noteID : IBqlField { }
			public new abstract class origNoteID : IBqlField { }
		}

		[PXHidden]
		[Serializable]
		public class EPTimecardDetail : PMTimeActivity
		{
			#region ParentTaskNoteID
			public new abstract class parentTaskNoteID : IBqlField { }

			[PXDBGuid(BqlField = typeof(PMTimeActivity.parentTaskNoteID))]
			[PXDBDefault(null, PersistingCheck = PXPersistingCheck.Nothing)]
			[CRTaskSelector]
			[PXRestrictor(typeof(Where<CRActivity.classID, Equal<CRActivityClass.task>>), null)]
			[PXUIField(DisplayName = "Task")]
			public override Guid? ParentTaskNoteID { get; set; }
			#endregion

			#region TimeCardCD
			public new abstract class timeCardCD : IBqlField { }

			[PXDBString(10, BqlField = typeof(PMTimeActivity.timeCardCD))]
			[PXUIField(Visible = false)]
			public override string TimeCardCD { get; set; }
			#endregion

			#region ProjectID
			public new abstract class projectID : IBqlField { }

			[EPTimeCardProject(BqlField = typeof(PMTimeActivity.projectID))]
			public override int? ProjectID { get; set; }
			#endregion

			#region ProjectTaskID
			public new abstract class projectTaskID : IBqlField { }

			[ProjectTask(typeof(projectID), BatchModule.EP, DisplayName = "Project Task", BqlField = typeof(PMTimeActivity.projectTaskID))]
			public override int? ProjectTaskID { get; set; }
			#endregion

			#region WeekID
			public new abstract class weekID : IBqlField { }

			[PXDBInt(BqlField = typeof(PMTimeActivity.weekID))]
			[PXUIField(DisplayName = "Time Card Week")]
			public override int? WeekID { get; set; }
			#endregion
			
			#region TimeSpent
			public new abstract class timeSpent : IBqlField { }

			[PXTimeList]
            [PXDefault(0)]
            [PXDBInt(BqlField = typeof(PMTimeActivity.timeSpent))]
			[PXUIField(DisplayName = "Time Spent")]
			public override int? TimeSpent { get; set; }
			#endregion

			#region IsBillable
			public new abstract class isBillable : IBqlField { }

			[PXDBBool(BqlField = typeof(PMTimeActivity.isBillable))]
			[PXUIField(DisplayName = "Billable")]
			public override Boolean? IsBillable { get; set; }
			#endregion

			#region TimeBillable
			public new abstract class timeBillable : IBqlField { }

			[PXTimeList]
			[PXDefault(0)]
			[PXDBInt(BqlField = typeof(PMTimeActivity.timeBillable))]
			[PXUIEnabled(typeof(isBillable))]
			[PXUIVerify(typeof(Where<timeSpent, IsNull, Or<timeBillable, IsNull, Or<timeSpent, GreaterEqual<timeBillable>, Or<isBillable, Equal<False>>>>>), PXErrorLevel.Error, CR.Messages.BillableTimeCannotBeGreaterThanTimeSpent)]
			[PXFormula(typeof(
				Switch<Case<Where<isBillable, Equal<True>>, timeSpent,
							 Case<Where<isBillable, Equal<False>>, int0>>,
							 timeBillable>))]
			[PXUIField(DisplayName = "Billable Time", FieldClass = "BILLABLE")]
			public override int? TimeBillable { get; set; }
			#endregion

			#region tstamp
			public new abstract class Tstamp : IBqlField { }

			[PXDBTimestamp(RecordComesFirst = true)]
			public override byte[] tstamp { get; set; }
			#endregion

			#region simple BQL Fields override

			public new abstract class noteID : IBqlField { }
			public new abstract class origNoteID : IBqlField { }
			public new abstract class overtimeSpent : IBqlField { }
			public new abstract class summaryLineNbr : IBqlField { }
			public new abstract class isCorrected : IBqlField { }
			public new abstract class jobID : IBqlField { }
			public new abstract class shiftID : IBqlField { }
			public new abstract class ownerID : IBqlField { }

			#endregion			

			#region Unbound Fields (Calculated in the TimecardMaint graph)

			public abstract class day : IBqlField { }
            [PXString()]
            [PXUIField(DisplayName = "Day")]
            //[PXStringList(new string[] { "1", "2", "3", "4", "5", "6", "0" }, new string[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" })]
            public virtual string Day { get; set; }


            public abstract class isOvertimeCalc : IBqlField { }
            [PXBool()]
			[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
			public virtual bool? IsOvertimeCalc { get; set; }


            public abstract class overtimeMultiplierCalc : IBqlField { }
            [PXDecimal(2)]
            [PXUIField(DisplayName = "OT Mult", Enabled = false)]
			public virtual decimal? OvertimeMultiplierCalc { get; set; }


            public abstract class billableTimeCalc : IBqlField { }
            [PXTimeList]
            [PXInt]
            [PXUIField(DisplayName = "Billable Time")]
			public virtual int? BillableTimeCalc { get; set; }


            public abstract class billableOvertimeCalc : IBqlField { }
            [PXTimeList]
            [PXInt]
            [PXUIField(DisplayName = "Billable OT")]
			public virtual int? BillableOvertimeCalc { get; set; }


            public abstract class regularTimeCalc : IBqlField { }
            [PXTimeList]
            [PXInt]
            [PXUIField(DisplayName = "RH", Enabled = false)]
			public virtual int? RegularTimeCalc { get; set; }


            public abstract class overTimeCalc : IBqlField { }
            [PXTimeList]
            [PXInt]
            [PXUIField(DisplayName = "OT", Enabled = false)]
			public virtual int? OverTimeCalc { get; set; }
			#endregion

		}

		[PXHidden]
		[Serializable]
		[PXBreakInheritance]
		public class EPTimecardTask : CRPMTimeActivity
		{
			#region CRActivity

			#region StartDate
			public new abstract class startDate : IBqlField { }

			[PXDefault]
			[EPDBDateAndTime(typeof(ownerID), WithoutDisplayNames = true, BqlField = typeof(CRActivity.startDate))]
			[PXUIField(DisplayName = "Date")]
			public override DateTime? StartDate { get; set; }
            #endregion

            #region simple BQL Fields override

			public new abstract class noteID : IBqlField { }
			public new abstract class parentNoteID : IBqlField { }
			public new abstract class ownerID : IBqlField { }
			public new abstract class classID : IBqlField { }
			public new abstract class type : IBqlField { }
			public new abstract class uistatus : IBqlField { }
			public new abstract class endDate : IBqlField { }

			#endregion

            #endregion
			
			#region PMTimeActivity

            #region TimeCardCD
            public new abstract class timeCardCD : IBqlField { }

			[PXDBString(10, BqlField = typeof(PMTimeActivity.timeCardCD))]
            [PXUIField(Visible = false)]
			public override string TimeCardCD { get; set; }
			#endregion

            #region ProjectID
            public new abstract class projectID : IBqlField { }

			[Project(BqlField = typeof(PMTimeActivity.projectID))]
			public override int? ProjectID { get; set; }
            #endregion

            #region ProjectTaskID
            public new abstract class projectTaskID : IBqlField { }

			[ProjectTask(typeof(projectID), BatchModule.EP, DisplayName = "Project Task", BqlField = typeof(PMTimeActivity.projectTaskID))]
			public override int? ProjectTaskID { get; set; }
            #endregion

            #region WeekID
            public new abstract class weekID : IBqlField { }

			[PXDBInt(BqlField = typeof(PMTimeActivity.weekID))]
            [PXUIField(DisplayName = "Time Card Week")]
			public override int? WeekID { get; set; }
			#endregion

            #region TimeSpent
			public new abstract class timeSpent : IBqlField { }

            [PXTimeList]
			[PXDBInt(BqlField = typeof(PMTimeActivity.timeSpent))]
            [PXUIField(DisplayName = "Time Spent")]
			public override int? TimeSpent { get; set; }
			#endregion

			#region simple BQL Fields override

			public new abstract class origNoteID : IBqlField { }
			public new abstract class overtimeSpent : IBqlField { }
			public new abstract class summaryLineNbr : IBqlField { }

			#endregion
			
            #endregion



            #region Unbound Fields (Calculated in the TimecardMaint graph)

            public abstract class isOvertimeCalc : IBqlField { }
            [PXBool()]
            [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
			public virtual bool? IsOvertimeCalc { get; set; }

            public abstract class overtimeMultiplierCalc : IBqlField { }
            [PXDecimal(1)]
            [PXUIField(DisplayName = "OT Mult")]
			public virtual decimal? OvertimeMultiplierCalc { get; set; }

            public abstract class billableTimeCalc : IBqlField { }
            [PXInt]
            [PXUIField(DisplayName = "Billable Time")]
			public virtual int? BillableTimeCalc { get; set; }

            public abstract class billableOvertimeCalc : IBqlField { }
            [PXInt]
            [PXUIField(DisplayName = "Billable OT")]
			public virtual int? BillableOvertimeCalc { get; set; }

            public abstract class regularTimeCalc : IBqlField { }
            [PXInt]
            [PXUIField(DisplayName = "RH", Enabled = false)]
			public virtual int? RegularTimeCalc { get; set; }


            public abstract class overTimeCalc : IBqlField { }
            [PXInt]
            [PXUIField(DisplayName = "OT", Enabled = false)]
			public virtual int? OverTimeCalc { get; set; }

            #endregion
        }

        [Serializable]
        [PXHidden]
        public class EPTimeCardItemOrig : EPTimeCardItem
        {
            public new abstract class timeCardCD : IBqlField { }
            public new abstract class lineNbr : IBqlField { }
            public new abstract class origLineNbr : IBqlField { }
        }

        [Serializable]
        [PXHidden]
        public class EPTimeCardItemEx : EPTimeCardItem
        {
            public new abstract class timeCardCD : IBqlField { }
            public new abstract class lineNbr : IBqlField { }
            public new abstract class origLineNbr : IBqlField { }
        }

		[Serializable]
	    public class ContractEx : Contract
	    {
			public new abstract class contractID : IBqlField { }
			public new abstract class isTemplate : IBqlField { }
			public new abstract class baseType : IBqlField { }
			public new abstract class description : IBqlField { }
			public new abstract class customerID : IBqlField { }
			public new abstract class status : IBqlField { }
			public new abstract class expireDate : IBqlField { }

			#region ContractCD
			public new abstract class contractCD : IBqlField { } 

			[PXDimensionSelector(ContractAttribute.DimensionName,
				typeof(Search2<contractCD, InnerJoin<ContractBillingSchedule, On<contractID, Equal<ContractBillingSchedule.contractID>>, LeftJoin<AR.Customer, On<AR.Customer.bAccountID, Equal<customerID>>>>
				, Where<isTemplate, Equal<boolFalse>, And<baseType, Equal<ContractBaseType>>>>),
				typeof(contractCD),
				typeof(contractCD), typeof(customerID), typeof(AR.Customer.acctName), typeof(Contract.locationID), typeof(description), typeof(status), typeof(expireDate), typeof(ContractBillingSchedule.lastDate), typeof(ContractBillingSchedule.nextDate), DescriptionField = typeof(description), Filterable = true)]
			[PXDBString(IsUnicode = true, IsKey = true, InputMask = "")]
			[PXUIField(DisplayName = "Contract ID", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
			public override string ContractCD { get; set; }
			#endregion
	    }

		#endregion
	}
}
