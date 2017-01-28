using System;
using PX.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Common;
using PX.Objects.GL;
using PX.Objects.PM;
using PX.Objects.IN;
using PX.Reports.ARm;
using PX.Reports.ARm.Data;
using PX.Reports.Drawing;
using PX.CS;
using System.Threading.Tasks;

namespace PX.Objects.CS
{
    public class RMReportReaderPM : PXGraphExtension<RMReportReaderGL, RMReportReader>
    {
        #region report

        private HashSet<int> _historyLoaded;
        private HashSet<PMHistoryKeyTuple> _historySegments;
        private Dictionary<PMHistoryKeyTuple, Dictionary<string, PMHistory>> _pmhistoryPeriodsByKey;

        private RMReportPeriods<PMHistory> _reportPeriods;

        private RMReportRange<PMAccountGroup> _accountGroupsRangeCache;
        private RMReportRange<PMProject> _projectsRangeCache;
        private RMReportRange<PMTask> _tasksRangeCache;
        private RMReportRange<InventoryItem> _itemRangeCache;

        [PXOverride]
        public void Clear(Action del)
        {
            del();

            _historyLoaded = null;
            _accountGroupsRangeCache = null;
            _projectsRangeCache = null;
            _tasksRangeCache = null;
            _itemRangeCache = null;
        }

        public virtual void PMEnsureInitialized()
        {
            if (_historyLoaded == null)
            {
                _reportPeriods = new RMReportPeriods<PMHistory>(this.Base);

                var accountGroupSelect = new PXSelect<PMAccountGroup>(this.Base);
                accountGroupSelect.View.Clear();
                accountGroupSelect.Cache.Clear();
               
                var projectSelect = new PXSelect<PMProject, Where<PMProject.isTemplate, Equal<False>, And<PMProject.nonProject, Equal<False>, And<PMProject.baseType, Equal<PMProject.ProjectBaseType>>>>>(this.Base);
                projectSelect.View.Clear();
                projectSelect.Cache.Clear();

                var taskSelect = new PXSelectJoin<PMTask, InnerJoin<PMProject, On<PMTask.projectID, Equal<PMProject.contractID>>>, Where<PMProject.isTemplate, Equal<False>, And<PMProject.nonProject, Equal<False>>>>(this.Base);
                taskSelect.View.Clear();
                taskSelect.Cache.Clear();
                
                var itemSelect = new PXSelectJoinGroupBy<InventoryItem, InnerJoin<PMHistory, On<InventoryItem.inventoryID, Equal<PMHistory.inventoryID>>>, Aggregate<GroupBy<InventoryItem.inventoryID>>>(this.Base);
                itemSelect.View.Clear();
                itemSelect.Cache.Clear();

                if (Base.Report.Current.ApplyRestrictionGroups == true)
                {
                    projectSelect.WhereAnd<Where<Match<Current<AccessInfo.userName>>>>();
                    taskSelect.WhereAnd<Where<Match<Current<AccessInfo.userName>>>>();
                    itemSelect.WhereAnd<Where<Match<Current<AccessInfo.userName>>>>();
                }

                accountGroupSelect.Select();
                projectSelect.Select();
                taskSelect.Select();

                foreach (InventoryItem item in itemSelect.Select())
                {
                    //The PXSelectJoinGroupBy is read-only, and Inventory items won't be added to the cache. Add them manually.
                    itemSelect.Cache.SetStatus(item, PXEntryStatus.Notchanged);
                }

                _historySegments = new HashSet<PMHistoryKeyTuple>();
                _pmhistoryPeriodsByKey = new Dictionary<PMHistoryKeyTuple, Dictionary<string, PMHistory>>();
                _historyLoaded = new HashSet<int>();

                _accountGroupsRangeCache = new RMReportRange<PMAccountGroup>(Base, PM.AccountGroupAttribute.DimensionName, RMReportConstants.WildcardMode.Fixed, RMReportConstants.BetweenMode.Fixed);
                _projectsRangeCache = new RMReportRange<PMProject>(Base, PM.ProjectAttribute.DimensionName, RMReportConstants.WildcardMode.Fixed, RMReportConstants.BetweenMode.Fixed);
                _tasksRangeCache = new RMReportRange<PMTask>(Base, PM.ProjectTaskAttribute.DimensionName, RMReportConstants.WildcardMode.Normal, RMReportConstants.BetweenMode.Fixed);
                _itemRangeCache = new RMReportRange<InventoryItem>(Base, IN.InventoryAttribute.DimensionName, RMReportConstants.WildcardMode.Fixed, RMReportConstants.BetweenMode.Fixed);

                //Add Inventory <OTHER> with InventoryID=0
                InventoryItem other = new InventoryItem
                {
                    InventoryCD = RMReportWildcard.EnsureWildcardForFixed(Messages.OtherItem, _itemRangeCache.Wildcard),
                    InventoryID = PMInventorySelectorAttribute.EmptyInventoryID,
                    Descr = Messages.OtherItemDescription
                };
                itemSelect.Cache.SetStatus(other, PXEntryStatus.Notchanged);
            }
        }

        protected virtual void NormalizeDataSource(RMDataSourcePM dsPM)
        {
            if (dsPM.StartAccountGroup != null && dsPM.StartAccountGroup.TrimEnd() == "")
            {
                dsPM.StartAccountGroup = null;
            }
            if (dsPM.EndAccountGroup != null && dsPM.EndAccountGroup.TrimEnd() == "")
            {
                dsPM.EndAccountGroup = null;
            }
            if (dsPM.StartProject != null && dsPM.StartProject.TrimEnd() == "")
            {
                dsPM.StartProject = null;
            }
            if (dsPM.EndProject != null && dsPM.EndProject.TrimEnd() == "")
            {
                dsPM.EndProject = null;
            }
            if (dsPM.StartProjectTask != null && dsPM.StartProjectTask.TrimEnd() == "")
            {
                dsPM.StartProjectTask = null;
            }
            if (dsPM.EndProjectTask != null && dsPM.EndProjectTask.TrimEnd() == "")
            {
                dsPM.EndProjectTask = null;
            }
            if (dsPM.StartInventory != null && dsPM.StartInventory.TrimEnd() == "")
            {
                dsPM.StartInventory = null;
            }
            if (dsPM.EndInventory != null && dsPM.EndInventory.TrimEnd() == "")
            {
                dsPM.EndInventory = null;
            }
        }

        private void ProcessPMResultset(PXResultset<PMHistory> resultset)
        {
            foreach (PXResult<PMHistory, PMTask> result in resultset)
            {
                var hist = (PMHistory) result;
                var task = (PMTask) result;

                var key = new PMHistoryKeyTuple(hist.ProjectID.Value, task.TaskCD, hist.AccountGroupID.Value, hist.InventoryID.Value);
                Dictionary<string, PMHistory> keyData;
                if (_pmhistoryPeriodsByKey.TryGetValue(key, out keyData))
                {
                    keyData.Add(hist.PeriodID, hist);
                }
                else
                {
                    _pmhistoryPeriodsByKey.Add(key, new Dictionary<string, PMHistory> { { hist.PeriodID, hist } });
                }

                _historySegments.Add(new PMHistoryKeyTuple(0, String.Empty, hist.AccountGroupID.Value, 0));
                _historySegments.Add(new PMHistoryKeyTuple(hist.ProjectID.Value, String.Empty, hist.AccountGroupID.Value, 0));
                _historySegments.Add(new PMHistoryKeyTuple(hist.ProjectID.Value, task.TaskCD, hist.AccountGroupID.Value, 0));
            }
        }

        [PXOverride]
        public virtual object GetHistoryValue(ARmDataSet dataSet, bool drilldown, Func<ARmDataSet, bool, object> del)
        {
            string rmType = Base.Report.Current.Type;
            if (rmType == ARmReport.PM)
            {
                RMDataSource ds = Base.DataSourceByID.Current;
                RMDataSourcePM dsPM = Base.Caches[typeof(RMDataSource)].GetExtension<RMDataSourcePM>(ds);

                ds.AmountType = (short?)dataSet[RMReportReaderGL.Keys.AmountType];
                dsPM.StartAccountGroup = dataSet[Keys.StartAccountGroup] as string ?? "";
                dsPM.EndAccountGroup = dataSet[Keys.EndAccountGroup] as string ?? "";
                dsPM.StartProject = dataSet[Keys.StartProject] as string ?? "";
                dsPM.EndProject = dataSet[Keys.EndProject] as string ?? "";
                dsPM.StartProjectTask = dataSet[Keys.StartProjectTask] as string ?? "";
                dsPM.EndProjectTask = dataSet[Keys.EndProjectTask] as string ?? "";
                dsPM.StartInventory = dataSet[Keys.StartInventory] as string ?? "";
                dsPM.EndInventory = dataSet[Keys.EndInventory] as string ?? "";

                RMDataSourceGL dsGL = Base.Caches[typeof(RMDataSource)].GetExtension<RMDataSourceGL>(ds);
                dsGL.StartBranch = dataSet[RMReportReaderGL.Keys.StartBranch] as string ?? "";
                dsGL.EndBranch = dataSet[RMReportReaderGL.Keys.EndBranch] as string ?? "";
                dsGL.EndPeriod = ((dataSet[RMReportReaderGL.Keys.EndPeriod] as string ?? "").Length > 2 ? ((dataSet[RMReportReaderGL.Keys.EndPeriod] as string ?? "").Substring(2) + "    ").Substring(0, 4) : "    ") + ((dataSet[RMReportReaderGL.Keys.EndPeriod] as string ?? "").Length > 2 ? (dataSet[RMReportReaderGL.Keys.EndPeriod] as string ?? "").Substring(0, 2) : dataSet[RMReportReaderGL.Keys.EndPeriod] as string ?? "");
                dsGL.EndPeriodOffset = (short?)(int?)dataSet[RMReportReaderGL.Keys.EndOffset];
                dsGL.EndPeriodYearOffset = (short?)(int?)dataSet[RMReportReaderGL.Keys.EndYearOffset];
                dsGL.StartPeriod = ((dataSet[RMReportReaderGL.Keys.StartPeriod] as string ?? "").Length > 2 ? ((dataSet[RMReportReaderGL.Keys.StartPeriod] as string ?? "").Substring(2) + "    ").Substring(0, 4) : "    ") + ((dataSet[RMReportReaderGL.Keys.StartPeriod] as string ?? "").Length > 2 ? (dataSet[RMReportReaderGL.Keys.StartPeriod] as string ?? "").Substring(0, 2) : dataSet[RMReportReaderGL.Keys.StartPeriod] as string ?? "");
                dsGL.StartPeriodOffset = (short?)(int?)dataSet[RMReportReaderGL.Keys.StartOffset];
                dsGL.StartPeriodYearOffset = (short?)(int?)dataSet[RMReportReaderGL.Keys.StartYearOffset];

                List<object[]> splitret = null;

                if (ds.Expand != ExpandType.Nothing)
                {
                    splitret = new List<object[]>();
                }

                if (ds.AmountType == null || ds.AmountType == 0)
                {
                    return 0m;
                }

                PMEnsureInitialized();
                EnsureHistoryLoaded(dsPM);
                NormalizeDataSource(dsPM);

                List<PMAccountGroup> accountGroups = _accountGroupsRangeCache.GetItemsInRange(dataSet[Keys.StartAccountGroup] as string,
                        group => group.GroupCD,
                        (group, code) => group.GroupCD = code);
                List<PMProject> projects = _projectsRangeCache.GetItemsInRange(dataSet[Keys.StartProject] as string,
                        project => project.ContractCD,
						(project, code) => project.ContractCD = code);
                List<PMTask> tasks = _tasksRangeCache.GetItemsInRange(dataSet[Keys.StartProjectTask] as string,
                        task => task.TaskCD,
						(task, code) => task.TaskCD = code);
                List<InventoryItem> items = _itemRangeCache.GetItemsInRange(dataSet[Keys.StartInventory] as string,
                        item => item.InventoryCD,
						(item, code) => item.InventoryCD = code);

                if (ds.Expand == ExpandType.AccountGroup)
                {
                    accountGroups.ForEach(a => splitret.Add(new object[] { a.GroupCD, a.Description, 0m, null, string.Empty }));
                }
                else if (ds.Expand == ExpandType.Project)
                {
                    projects.ForEach(p => splitret.Add(new object[] { p.ContractCD, p.Description, 0m, null, string.Empty }));
                }
                else if (ds.Expand == ExpandType.ProjectTask)
                {
                    //To avoid useless expansion, first restrict list of tasks to those tasks which point to current projects
                    tasks = tasks.Where(t => projects.Any(p => t.ProjectID == p.ContractID)).ToList<PMTask>();
                    tasks = tasks.GroupBy(t => t.TaskCD).Select(g => new PMTask() { TaskCD = g.Key, TaskID =  g.Min(t => t.TaskID), Description = g.Min(t => t.Description) }).ToList<PMTask>();
                    tasks.ForEach(pt => splitret.Add(new object[] { pt.TaskCD, pt.Description, 0m, null, string.Empty }));
                }
                else if (ds.Expand == ExpandType.Inventory)
                {
                    items.ForEach(i => splitret.Add(new object[] { i.InventoryCD, i.Descr, 0m, null, string.Empty }));
                }

                return CalculateAndExpandValue(drilldown, ds, dsGL, dsPM, dataSet, accountGroups, projects, tasks, items, splitret);
            }
            else
            {
                return del(dataSet, drilldown);
            }
        }

        private object CalculateAndExpandValue(bool drilldown, RMDataSource ds, RMDataSourceGL dsGL, RMDataSourcePM dsPM, ARmDataSet dataSet, List<PMAccountGroup> accountGroups, List<PMProject> projects, List<PMTask> tasks, List<InventoryItem> items, List<object[]> splitret)
        {
            decimal totalAmount = 0;
			object locker = new object();
			Dictionary<PMHistoryKeyTuple, PXResult<PMHistory, PMProject, PMTask, PMAccountGroup, InventoryItem>> drilldownData = null;

            if (drilldown)
            {
				drilldownData = new Dictionary<PMHistoryKeyTuple, PXResult<PMHistory, PMProject, PMTask, PMAccountGroup, InventoryItem>>();
            }

            Parallel.For(0, accountGroups.Count, accountGroup =>
            {
                PMAccountGroup currentAccountGroup = accountGroups[accountGroup];
                if (!_historySegments.Contains(new PMHistoryKeyTuple(0, String.Empty, currentAccountGroup.GroupID.Value, 0))) return;

                Parallel.For(0, projects.Count, project =>
                {
                    PMProject currentProject = projects[project];
                    if (!_historySegments.Contains(new PMHistoryKeyTuple(currentProject.ContractID.Value, String.Empty, currentAccountGroup.GroupID.Value, 0))) return;

                    Parallel.For(0, tasks.Count, task =>
                    {
                        PMTask currentTask = tasks[task];
                        if (!_historySegments.Contains(new PMHistoryKeyTuple(currentProject.ContractID.Value, currentTask.TaskCD, currentAccountGroup.GroupID.Value, 0))) return;
						if (ds.Expand != ExpandType.ProjectTask && currentTask.ProjectID != currentProject.ContractID) return;

                        Parallel.For(0, items.Count, item =>
                        {
                            InventoryItem currentItem = items[item];
                            List<PMHistory> periods = GetPeriodsToCalculate(currentProject, currentTask, currentAccountGroup, currentItem, ds, dsGL);
                            if (periods == null) { return; }

                            foreach (var hist in periods)
                            {
                                decimal amount = GetAmountFromPMHistory(ds, hist);
                                lock (locker)
                                {
                                    totalAmount += amount;
                                }

                                if (drilldown)
                                {
									var key = new PMHistoryKeyTuple(currentProject.ContractID.Value, currentTask.TaskCD, currentAccountGroup.GroupID.Value, currentItem.InventoryID.Value);
	                                PXResult<PMHistory, PMProject, PMTask, PMAccountGroup, InventoryItem> drilldownRow = null;

									lock (drilldownData)
                                    {
	                                    if (!drilldownData.TryGetValue(key, out drilldownRow))
	                                    {
		                                    drilldownRow = new PXResult<PMHistory, PMProject, PMTask, PMAccountGroup, InventoryItem>(new PMHistory(), currentProject, currentTask, currentAccountGroup, currentItem);
		                                    drilldownData.Add(key, drilldownRow);
	                                    }
                                    }
									
		                            lock (drilldownRow)
		                            {
			                            AggregatePMHistoryForDrilldown(drilldownRow, hist);
		                            }
                                }

                                if (ds.Expand == ExpandType.AccountGroup)
                                {
                                    lock (currentAccountGroup)
                                    {
                                        splitret[accountGroup][2] = (decimal)splitret[accountGroup][2] + amount;
                                        if (splitret[accountGroup][3] == null)
                                        {
                                            var dataSetCopy = new ARmDataSet(dataSet);
                                            dataSetCopy[PX.Objects.CS.RMReportReaderPM.Keys.StartAccountGroup] = dataSetCopy[PX.Objects.CS.RMReportReaderPM.Keys.EndAccountGroup] = currentAccountGroup.GroupCD;
                                            splitret[accountGroup][3] = dataSetCopy;
                                        }
                                    }
                                }
                                else if (ds.Expand == ExpandType.Project)
                                {
                                    lock (currentProject)
                                    {
                                        splitret[project][2] = (decimal)splitret[project][2] + amount;
                                        if (splitret[project][3] == null)
                                        {
                                            var dataSetCopy = new ARmDataSet(dataSet);
                                            dataSetCopy[PX.Objects.CS.RMReportReaderPM.Keys.StartProject] = dataSetCopy[PX.Objects.CS.RMReportReaderPM.Keys.EndProject] = currentProject.ContractCD;
                                            splitret[project][3] = dataSetCopy;
                                        }
                                    }
                                }
                                else if (ds.Expand == ExpandType.ProjectTask)
                                {
                                    lock (currentItem)
                                    {
                                        splitret[task][2] = (decimal)splitret[task][2] + amount;
                                        if (splitret[task][3] == null)
                                        {
                                            var dataSetCopy = new ARmDataSet(dataSet);
                                            dataSetCopy[PX.Objects.CS.RMReportReaderPM.Keys.StartProjectTask] = dataSetCopy[PX.Objects.CS.RMReportReaderPM.Keys.EndProjectTask] = currentTask.TaskCD;
                                            splitret[task][3] = dataSetCopy;
                                        }
                                    }
                                }
                                else if (ds.Expand == ExpandType.Inventory)
                                {
                                    lock (currentItem)
                                    {
                                        splitret[item][2] = (decimal)splitret[item][2] + amount;
                                        if (splitret[item][3] == null)
                                        {
                                            var dataSetCopy = new ARmDataSet(dataSet);
                                            dataSetCopy[PX.Objects.CS.RMReportReaderPM.Keys.StartInventory] = dataSetCopy[PX.Objects.CS.RMReportReaderPM.Keys.EndInventory] = currentItem.InventoryCD;
                                            splitret[item][3] = dataSetCopy;
                                        }
                                    }
                                }
                            }
                        });
                    });
                });
            });

            if (drilldown)
            {
				var resultset = new PXResultset<PMHistory, PMProject, PMTask, PMAccountGroup, InventoryItem>();
				foreach (var r in
					from row in drilldownData.Values
					let projectCD = ((PMProject)row[typeof(PMProject)]).With(_ => _.ContractCD)
					let taskCD = ((PMTask)row[typeof(Sub)]).With(_ => _.TaskCD)
					let accGroupCD = ((PMAccountGroup)row[typeof(PMAccountGroup)]).With(_ => _.GroupCD)
					let inventoryCD = ((InventoryItem)row[typeof(InventoryItem)]).With(_ => _.InventoryCD)
					orderby projectCD, taskCD, accGroupCD, inventoryCD
					select row)
				{
					resultset.Add(r);
				}
                return resultset;
            }
            else if (ds.Expand != ExpandType.Nothing)
            {
                return splitret;
            }
            else
            {
                return totalAmount;
            }
        }

	    private static void AggregatePMHistoryForDrilldown(PMHistory resulthist, PMHistory hist)
	    {
			resulthist.ProjectID = hist.ProjectID;
			resulthist.ProjectTaskID = hist.ProjectTaskID;
			resulthist.AccountGroupID = hist.AccountGroupID;
			resulthist.InventoryID = hist.InventoryID;
			resulthist.PeriodID = hist.PeriodID;

		    resulthist.BudgetQty = resulthist.BudgetQty.GetValueOrDefault() + hist.BudgetQty.GetValueOrDefault();
		    resulthist.BudgetAmount = resulthist.BudgetAmount.GetValueOrDefault() + hist.BudgetAmount.GetValueOrDefault();
		    resulthist.RevisedQty = resulthist.RevisedQty.GetValueOrDefault() + hist.RevisedQty.GetValueOrDefault();
			resulthist.RevisedAmount = resulthist.RevisedAmount.GetValueOrDefault() + hist.RevisedAmount.GetValueOrDefault();

			resulthist.FinPTDAmount = resulthist.FinPTDAmount.GetValueOrDefault() + hist.FinPTDAmount.GetValueOrDefault();
			resulthist.FinYTDAmount = resulthist.FinYTDAmount.GetValueOrDefault() + hist.FinYTDAmount.GetValueOrDefault();
			resulthist.FinPTDQty = resulthist.FinPTDQty.GetValueOrDefault() + hist.FinPTDQty.GetValueOrDefault();
			resulthist.FinYTDQty = resulthist.FinYTDQty.GetValueOrDefault() + hist.FinYTDQty.GetValueOrDefault();
			resulthist.TranPTDAmount = resulthist.TranPTDAmount.GetValueOrDefault() + hist.TranPTDAmount.GetValueOrDefault();
			resulthist.TranYTDAmount = resulthist.TranYTDAmount.GetValueOrDefault() + hist.TranYTDAmount.GetValueOrDefault();
			resulthist.TranPTDQty = resulthist.TranPTDQty.GetValueOrDefault() + hist.TranPTDQty.GetValueOrDefault();
			resulthist.TranYTDQty = resulthist.TranYTDQty.GetValueOrDefault() + hist.TranYTDQty.GetValueOrDefault();
		}

		private List<PMHistory> GetPeriodsToCalculate(PMProject project, PMTask task, PMAccountGroup accountGroup, InventoryItem item, RMDataSource ds, RMDataSourceGL dsGL)
        {
            Dictionary<string, PMHistory> periodsForKey = null;

            var key = new PMHistoryKeyTuple(project.ContractID.Value, task.TaskCD, accountGroup.GroupID.Value, item.InventoryID.Value);
            if (!_pmhistoryPeriodsByKey.TryGetValue(key, out periodsForKey))
            {
                return null;
            }

            if (ds.AmountType == BalanceType.Amount || ds.AmountType == BalanceType.Quantity ||
                ds.AmountType == BalanceType.BudgetAmount || ds.AmountType == BalanceType.BudgetQuantity ||
                ds.AmountType == BalanceType.RevisedAmount || ds.AmountType == BalanceType.RevisedQuantity)
            {
                //These amounts are calculated against start of project
                dsGL.StartPeriod = _reportPeriods.PerWildcard;
                dsGL.StartPeriodOffset = 0;
                dsGL.StartPeriodYearOffset = 0;
            }

            return _reportPeriods.GetPeriodsForRegularAmount(dsGL, periodsForKey);
        }

        private static decimal GetAmountFromPMHistory(RMDataSource ds, PMHistory hist)
        {
            //We always use the PTD amounts; depending on BalanceType we will expand our calculation to more periods (GetPeriodsToCalculate)
            switch (ds.AmountType.Value)
            {
                case BalanceType.Amount:
                case BalanceType.TurnoverAmount:
                    return hist.FinPTDAmount.Value;
                case BalanceType.Quantity:
                case BalanceType.TurnoverQuantity:
                    return hist.FinPTDQty.Value;
                case BalanceType.BudgetAmount:
                case BalanceType.BudgetPTDAmount:
                    return hist.PTDBudgetAmount.Value;
                case BalanceType.BudgetQuantity:
                case BalanceType.BudgetPTDQuantity:
                    return hist.PTDBudgetQty.Value;
                case BalanceType.RevisedAmount:
                case BalanceType.RevisedPTDAmount:
                    return hist.PTDRevisedAmount.Value;
                case BalanceType.RevisedQuantity:
                case BalanceType.RevisedPTDQuantity:
                    return hist.PTDRevisedQty.Value;
                default:
                    System.Diagnostics.Debug.Assert(false, "Unknown amount type: " + ds.AmountType.Value);
                    return 0;
            }
        }

        [PXOverride]
        public string GetUrl(Func<string> del)
        {
            string rmType = Base.Report.Current.Type;
            if (rmType == ARmReport.PM)
            {
                PXSiteMapNode node = PXSiteMap.Provider.FindSiteMapNodeByScreenID("CS600010");
                if (node != null)
                {
                    return PX.Common.PXUrl.TrimUrl(node.Url);
                }
                throw new PXException(ErrorMessages.NotEnoughRightsToAccessObject, "CS600010");
            }
            else
            {
                return del();
            }
        }

        private void EnsureHistoryLoaded(RMDataSourcePM dsPM)
        {
            //Unlike RMReportReaderGL, there is no lazy loading for now, given the way PMHistory is structured we need to load whole project history to get balances for a given project
            //We could do lazy loading by project, but that would be slower if report including a large number of projects (ex: project profitability list)
            var key = 1;
            if (!_historyLoaded.Contains(key))
            {
                ProcessPMResultset(PXSelectReadonly2<PMHistory, 
                    InnerJoin<PMTask, On<PMHistory.projectTaskID, Equal<PMTask.taskID>>>>.Select(this.Base));
                _historyLoaded.Add(key);
            }
        }

        #endregion

        #region IARmDataSource

        // Initializing IDictionary with Keys and their string represantations in static constructor for perfomance
        // (Enum.GetNames(), Enum.TryParse(), Enum.GetValues(), Enum.IsDefined() are very slow because of using Reflection)

        static RMReportReaderPM()
        {
            _keysDictionary = Enum.GetValues(typeof(Keys)).Cast<Keys>().ToDictionary(@e => @e.ToString(), @e => @e);
        }

        private static readonly IDictionary<string, Keys> _keysDictionary;

        public enum Keys
        {
            StartAccountGroup,
            EndAccountGroup,
            StartProject,
            EndProject,
            StartProjectTask,
            EndProjectTask,
            StartInventory,
            EndInventory,
        }

        [PXOverride]
        public bool IsParameter(ARmDataSet ds, string name, ValueBucket value, Func<ARmDataSet, string, ValueBucket, bool> del)
        {
            bool flag = del(ds, name, value);
            if (!flag)
            {
                Keys key;
                if (_keysDictionary.TryGetValue(name, out key))
                {
                    value.Value = ds[key];
                    return true;
                }
                return false;
            }
            return flag;
        }

        [PXOverride]
        public ARmDataSet MergeDataSet(IEnumerable<ARmDataSet> list, string expand, MergingMode mode, Func<IEnumerable<ARmDataSet>, string, MergingMode, ARmDataSet> del)
        {
            ARmDataSet dataSet = del(list, expand, mode);

            string rmType = Base.Report.Current.Type;
            if (rmType == ARmReport.PM)
            {
                foreach (ARmDataSet set in list)
                {
                    if (set == null) continue;

					RMReportWildcard.ConcatenateRangeWithDataSet(dataSet, set, Keys.StartAccountGroup, Keys.EndAccountGroup, mode);
					RMReportWildcard.ConcatenateRangeWithDataSet(dataSet, set, Keys.StartProject, Keys.EndProject, mode);
					RMReportWildcard.ConcatenateRangeWithDataSet(dataSet, set, Keys.StartProjectTask, Keys.EndProjectTask, mode);
					RMReportWildcard.ConcatenateRangeWithDataSet(dataSet, set, Keys.StartInventory, Keys.EndInventory, mode);
                }

                dataSet.Expand = list.First().Expand;
            }

            return dataSet;
        }

        [PXOverride]
        public ARmReport GetReport(Func<ARmReport> del)
        {
            ARmReport ar = del();

            int? id = Base.Report.Current.StyleID;
            if (id != null)
            {
                RMStyle st = Base.StyleByID.SelectSingle(id);
                Base.fillStyle(st, ar.Style);
            }

            id = Base.Report.Current.DataSourceID;
            if (id != null)
            {
                RMDataSource ds = Base.DataSourceByID.SelectSingle(id);
                FillDataSourceInternal(ds, ar.DataSet, ar.Type);
            }

            List<ARmReport.ARmReportParameter> aRp = ar.ARmParams;
            PXFieldState state;
            RMReportPM rPM = Base.Report.Cache.GetExtension<RMReportPM>(Base.Report.Current);

            if (ar.Type == ARmReport.PM)
            {
                string sViewName = string.Empty;
                string sInputMask = string.Empty;

                // StartAccountGroup, EndAccountGroup
                bool RequestEndAccountGroup = rPM.RequestEndAccountGroup ?? false;
                //int colSpan = RequestEndAccountGroup ? 1 : 2;
                int colSpan = 2;
                sViewName = sInputMask = string.Empty;
                state = Base.DataSourceByID.Cache.GetStateExt<RMDataSourcePM.startAccountGroup>(null) as PXFieldState;
                if (state != null && !String.IsNullOrEmpty(state.ViewName))
                {
                    sViewName = state.ViewName;
                    if (state is PXStringState)
                    {
                        sInputMask = ((PXStringState)state).InputMask;
                    }
                }
                Base.CreateParameter(Keys.StartAccountGroup, "StartAccountGroup", Messages.GetLocal(Messages.StartAccTitle), ar.DataSet[Keys.StartAccountGroup] as string, rPM.RequestStartAccountGroup ?? false, colSpan, sViewName, sInputMask, aRp);

                sViewName = sInputMask = string.Empty;
                state = Base.DataSourceByID.Cache.GetStateExt<RMDataSourcePM.endAccountGroup>(null) as PXFieldState;
                if (state != null && !String.IsNullOrEmpty(state.ViewName))
                {
                    sViewName = state.ViewName;
                    if (state is PXStringState)
                    {
                        sInputMask = ((PXStringState)state).InputMask;
                    }
                }
                Base.CreateParameter(Keys.EndAccountGroup, "EndAccountGroup", Messages.GetLocal(Messages.EndAccTitle), ar.DataSet[Keys.EndAccountGroup] as string, RequestEndAccountGroup, colSpan, sViewName, sInputMask, aRp);

                // StartProject, EndProject
                bool bRequestEndProject = rPM.RequestEndProject ?? false;
                //colSpan = bRequestEndProject ? 1 : 2;
                colSpan = 2;
                sViewName = sInputMask = string.Empty;
                state = Base.DataSourceByID.Cache.GetStateExt<RMDataSourcePM.startProject>(null) as PXFieldState;
                if (state != null && !String.IsNullOrEmpty(state.ViewName))
                {
                    sViewName = state.ViewName;
                    if (state is PXStringState)
                    {
                        sInputMask = ((PXStringState)state).InputMask;
                    }
                }
                Base.CreateParameter(Keys.StartProject, "StartProject", Messages.GetLocal(Messages.StartProjectTitle), ar.DataSet[Keys.StartProject] as string, rPM.RequestStartProject ?? false, colSpan, sViewName, sInputMask, aRp);

                sViewName = sInputMask = string.Empty;
                state = Base.DataSourceByID.Cache.GetStateExt<RMDataSourcePM.endProject>(null) as PXFieldState;
                if (state != null && !String.IsNullOrEmpty(state.ViewName))
                {
                    sViewName = state.ViewName;
                    if (state is PXStringState)
                    {
                        sInputMask = ((PXStringState)state).InputMask;
                    }
                }
                Base.CreateParameter(Keys.EndProject, "EndProject", Messages.GetLocal(Messages.EndProjectTitle), ar.DataSet[Keys.EndProject] as string, bRequestEndProject, colSpan, sViewName, sInputMask, aRp);

                // StartTask, EndTask
                bool RequestEndProjectTask = rPM.RequestEndProjectTask ?? false;

                //colSpan = RequestEndProjectTask ? 1 : 2;
                colSpan = 2;
                sViewName = sInputMask = string.Empty;
                state = Base.DataSourceByID.Cache.GetStateExt<RMDataSourcePM.startProjectTask>(null) as PXFieldState;
                if (state != null && !String.IsNullOrEmpty(state.ViewName))
                {
                    sViewName = state.ViewName;
                    if (state is PXStringState)
                    {
                        sInputMask = ((PXStringState)state).InputMask;
                    }
                }
                Base.CreateParameter(Keys.StartProjectTask, "StartTask", Messages.GetLocal(Messages.StartTaskTitle), ar.DataSet[Keys.StartProjectTask] as string, rPM.RequestStartProjectTask ?? false, colSpan, sViewName, sInputMask, aRp);

                sViewName = sInputMask = string.Empty;
                state = Base.DataSourceByID.Cache.GetStateExt<RMDataSourcePM.endProjectTask>(null) as PXFieldState;
                if (state != null && !String.IsNullOrEmpty(state.ViewName))
                {
                    sViewName = state.ViewName;
                    if (state is PXStringState)
                    {
                        sInputMask = ((PXStringState)state).InputMask;
                    }
                }
                Base.CreateParameter(Keys.EndProjectTask, "EndTask", Messages.GetLocal(Messages.EndTaskTitle), ar.DataSet[Keys.EndProjectTask] as string, RequestEndProjectTask, colSpan, sViewName, sInputMask, aRp);

                // StartInventory, EndInventory
                bool bRequestEndInventory = rPM.RequestEndInventory ?? false;
                //colSpan = bRequestEndInventory ? 1 : 2;
                colSpan = 2;

                sViewName = sInputMask = string.Empty;
                state = Base.DataSourceByID.Cache.GetStateExt<RMDataSourcePM.startInventory>(null) as PXFieldState;
                if (state != null && !String.IsNullOrEmpty(state.ViewName))
                {
                    sViewName = state.ViewName;
                    if (state is PXStringState)
                    {
                        sInputMask = ((PXStringState)state).InputMask;
                    }
                }
                Base.CreateParameter(Keys.StartInventory, "StartInventory", Messages.GetLocal(Messages.StartInventoryTitle), ar.DataSet[Keys.StartInventory] as string, rPM.RequestStartInventory ?? false, colSpan, sViewName, sInputMask, aRp);

                sViewName = sInputMask = string.Empty;
                state = Base.DataSourceByID.Cache.GetStateExt<RMDataSourcePM.endInventory>(null) as PXFieldState;
                if (state != null && !String.IsNullOrEmpty(state.ViewName))
                {
                    sViewName = state.ViewName;
                    if (state is PXStringState)
                    {
                        sInputMask = ((PXStringState)state).InputMask;
                    }
                }
                Base.CreateParameter(Keys.EndInventory, "EndInventory", Messages.GetLocal(Messages.EndInventoryTitle), ar.DataSet[Keys.EndInventory] as string, bRequestEndInventory, colSpan, sViewName, sInputMask, aRp);
            }

            return ar;
        }

        [PXOverride]
        public virtual List<ARmUnit> ExpandUnit(RMDataSource ds, ARmUnit unit, Func<RMDataSource, ARmUnit, List<ARmUnit>> del)
        {
            string rmType = Base.Report.Current.Type;
            if (rmType == ARmReport.PM)
            {
                if (unit.DataSet.Expand != ExpandType.Nothing)
                {
                    PMEnsureInitialized();
                    if (ds.Expand == ExpandType.AccountGroup)
                    {
                        return RMReportUnitExpansion<PMAccountGroup>.ExpandUnit(Base, ds, unit, Keys.StartAccountGroup, Keys.EndAccountGroup,
                            rangeToFetch => _accountGroupsRangeCache.GetItemsInRange(rangeToFetch[Keys.StartAccountGroup] as string,
                                accountGroup => accountGroup.GroupCD,
                                (accountGroup, code) => accountGroup.GroupCD = code),
                            accountGroup => accountGroup.GroupCD, accountGroup => accountGroup.Description,
                            (accountGroup, wildcard) => { accountGroup.GroupCD = wildcard; accountGroup.Description = wildcard; });
                    }
                    else if (ds.Expand == ExpandType.Project)
                    {
                        return RMReportUnitExpansion<PMProject>.ExpandUnit(Base, ds, unit, Keys.StartProject, Keys.EndProject,
                            rangeToFetch => _projectsRangeCache.GetItemsInRange(rangeToFetch[Keys.StartProject] as string,
                                project => project.ContractCD,
                                (project, code) => project.ContractCD = code),
                            project => project.ContractCD, project => project.Description,
                            (project, wildcard) => { project.ContractCD = wildcard; project.Description = wildcard; });
                    }
                    else if (ds.Expand == ExpandType.ProjectTask)
                    {
                        return RMReportUnitExpansion<PMTask>.ExpandUnit(Base, ds, unit, Keys.StartProjectTask, Keys.EndProjectTask,
                            rangeToFetch => {
                                List<PMTask> tasks = _tasksRangeCache.GetItemsInRange(rangeToFetch[Keys.StartProjectTask] as string,
                                    task => task.TaskCD,
                                    (task, code) => task.TaskCD = code);

                                ARmDataSet projectRange = new ARmDataSet();
                                RMReportWildcard.ConcatenateRangeWithDataSet(projectRange, unit.DataSet, Keys.StartProject, Keys.EndProject, MergingMode.Intersection);

                                if (!String.IsNullOrEmpty(projectRange[Keys.StartProject] as string))
                                {
                                    //A project range is specified in the unit; restrict tasks to tasks of this project range.
                                    List<PMProject> projects = _projectsRangeCache.GetItemsInRange(projectRange[Keys.StartProject] as string,
                                        project => project.ContractCD,
                                        (project, code) => project.ContractCD = code);
                                    tasks = tasks.Where(t => projects.Any(p => t.ProjectID == p.ContractID)).ToList<PMTask>();
                                }

                                //Same project TaskCD can be reused in multiple projects; it only makes sense to get distinct values for the purpose of filling the unit tree
                                List<PMTask> groupedTasks = tasks.GroupBy(t => t.TaskCD).Select(g => new PMTask() { TaskCD = g.Key, Description = g.Min(t => t.Description) }).ToList<PMTask>();
                                return groupedTasks;
                            },
                            task => task.TaskCD, project => project.Description,
                            (task, wildcard) => { task.TaskCD = wildcard; task.Description = wildcard; });
                    }
                    else if (ds.Expand == ExpandType.Inventory)
                    {
                        return RMReportUnitExpansion<InventoryItem>.ExpandUnit(Base, ds, unit, Keys.StartInventory, Keys.EndInventory,
                            rangeToFetch => _itemRangeCache.GetItemsInRange(rangeToFetch[Keys.StartInventory] as string,
                                item => item.InventoryCD,
                                (item, code) => item.InventoryCD = code),
                            item => item.InventoryCD, item => item.Descr,
                            (item, wildcard) => { item.InventoryCD = wildcard; item.Descr = wildcard; });
                    }
                }
            }
            else
            {
                return del(ds, unit);
            }

            return null;
        }

        [PXOverride]
        public void FillDataSource(RMDataSource ds, ARmDataSet dst, string rmType, Action<RMDataSource, ARmDataSet, string> del)
        {
            del(ds, dst, rmType);
            
            if (rmType == ARmReport.PM)
            {
                FillDataSourceInternal(ds, dst, rmType);
            }
        }

        private void FillDataSourceInternal(RMDataSource ds, ARmDataSet dst, string rmType)
        {
            if (ds != null && ds.DataSourceID != null)
            {
                RMDataSourcePM dsPM = Base.Caches[typeof(RMDataSource)].GetExtension<RMDataSourcePM>(ds);
                dst[Keys.StartAccountGroup] = dsPM.StartAccountGroup;
                dst[Keys.EndAccountGroup] = dsPM.EndAccountGroup;
                dst[Keys.StartProject] = dsPM.StartProject;
                dst[Keys.EndProject] = dsPM.EndProject;
                dst[Keys.StartProjectTask] = dsPM.StartProjectTask;
                dst[Keys.EndProjectTask] = dsPM.EndProjectTask;
                dst[Keys.StartInventory] = dsPM.StartInventory;
                dst[Keys.EndInventory] = dsPM.EndInventory;
            }
        }

        private ArmDATA _Data;
        [PXOverride]
        public object GetExprContext()
        {
            if (_Data == null)
            {
                _Data = new ArmDATA();
            }
            return _Data;
        }

        #endregion
    }

    public struct PMHistoryKeyTuple
    {
        public readonly int ProjectID;
        public readonly string ProjectTaskCD;
        public readonly int AccountGroupID;
        public readonly int InventoryID;

        public PMHistoryKeyTuple(int projectID, string projectTaskCD, int accountGroupID, int inventoryID)
        {
            ProjectID = projectID;
            ProjectTaskCD = projectTaskCD;
            AccountGroupID = accountGroupID;
            InventoryID = inventoryID;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + ProjectID.GetHashCode();
                hash = hash * 23 + ProjectTaskCD.GetHashCode();
                hash = hash * 23 + AccountGroupID.GetHashCode();
                hash = hash * 23 + InventoryID.GetHashCode();
                return hash;
            }
        }
    }
}
