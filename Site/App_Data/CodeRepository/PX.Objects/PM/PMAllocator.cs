using System;
using System.Collections.Generic;
using System.Linq;
using PX.Common.Parser;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Reports.Parser;

namespace PX.Objects.PM
{
	/// <summary>
	/// Creates allocation transaction based on the allocation rules for the given project task(s).
	/// </summary>
    [Serializable]
	public class PMAllocator : PXGraph<PMAllocator>
	{
		#region DAC Attributes override
		
		[PXDefault]
		[PXDBInt]
		protected virtual void PMTran_ProjectID_CacheAttached(PXCache sender) { }

		[PXDBInt]
		protected virtual void PMTran_TaskID_CacheAttached(PXCache sender) { }

		#endregion

		#region Selects/Views
		
		public PXSelect<PMRegister> Document;
		public PXSelect<PMTran, Where<PMTran.tranType, Equal<Current<PMRegister.module>>, And<PMTran.refNbr, Equal<Current<PMRegister.refNbr>>>>> Transactions;
		public PXSelect<PMAllocationSourceTran> SourceTran;
		public PXSelect<PMAllocationAuditTran> AuditTran;
		public PXSelect<PMTaskAllocTotalAccum> TaskTotals;
		public PXSetupOptional<INSetup> Insetup;  //used in INUnit conversions and rounding. 
		public PXSetupOptional<CommonSetup> commonsetup;

		#endregion

		//stores allocated transactions for the step.
		protected Dictionary<int, List<PMTranWithTrace>> stepResults;
		
		protected Dictionary<int, Dictionary<int, List<PXResult<PMTran>>>> transactions; //Transactions stored by TaskID and then By AccountGroupID

		protected RateEngineV2 rateEngine;

		protected Dictionary<string, AllocationInfo> allocationInfo; //Cached set of allocation properties for a given AllocationID;
		protected Dictionary<int, AccountGroup> accountGroups; //Cached set of AccountGroups;
		protected Dictionary<string, List<PMRateDefinition>> rateDefinitions; //Cached set of RateDefinitions; 

		
		/// <summary>
		/// Gets or sets the Allocation Date from the Transactions. Used to override the default allocation date for the document.
		/// </summary>
		public DateTime? OverrideAllocationDate { get; set; }

		/// <summary>
		/// Gets or sets Source Transactions for the allocation. If null Transactions are selected from the database.
		/// </summary>
		public List<PMTran> SourceTransactions { get; set; }


        /// <summary>
        /// Get or sets Restriction on Transactions that can be allocated. 
        /// If set only those transactions get allocated that have the Date greater or equal to FilterStartDate.
        /// </summary>
	    public DateTime? FilterStartDate { get; set; }

        /// <summary>
        /// Get or sets Restriction on Transactions that can be allocated. 
        /// If set only those transactions get allocated that have the Date less then or equal to FilterEndDate.
        /// </summary>
        public DateTime? FilterEndDate { get; set; }

		public PMAllocator()
		{
			Caches[typeof(PMAllocationStep)].AllowInsert = false;
			Caches[typeof(PMAllocationStep)].AllowUpdate = false;
			Caches[typeof(PMAllocationStep)].AllowDelete = false;

			stepResults = new Dictionary<int, List<PMTranWithTrace>>();
			transactions = new Dictionary<int, Dictionary<int, List<PXResult<PMTran>>>>();
			allocationInfo = new Dictionary<string, AllocationInfo>();
			rateDefinitions = new Dictionary<string, List<PMRateDefinition>>();
		}

		/// <summary>
		/// Executes Allocation for the list of tasks.
		/// </summary>
		/// <param name="tasks"></param>
		public virtual void Execute(List<PMTask> tasks)
		{
			PreselectAccountGroups();
			if (PreSelectTasksTransactions(tasks))
			{
				foreach (PMTask task in tasks)
				{
					Execute(task, false);
				}
			}
		}

		/// <summary>
		/// Executes Allocation for the given Task.
		/// </summary>
		/// <param name="task">Task</param>
		public virtual void Execute(PMTask task)
		{
			Execute(task, true);
		}

		public virtual void Execute(PMTask task, bool preselectTransactions)
		{
			stepResults.Clear();

			PreselectAccountGroups();

			if (preselectTransactions)
				PreSelectTaskTransactions(task);

			foreach (PMAllocationStep step in PXSelect<PMAllocationStep, Where<PMAllocationStep.allocationID, Equal<Required<PMAllocationStep.allocationID>>>, OrderBy<Asc<PMAllocationStep.stepID>>>.Select(this, task.AllocationID))
			{
				try
				{
					ProcessStep(task, step);
				}
				catch (PXException ex)
				{
					throw new PXException(ex, Messages.AllocationStepFailed, step.StepID, task.TaskCD);
				}
			}

			if (stepResults.Count > 0)
			{
				foreach (KeyValuePair<int, List<PMTranWithTrace>> kv in stepResults)
				{
					foreach (PMTranWithTrace twt in kv.Value)
					{
						AddAuditTran(task.AllocationID, twt.Tran.TranID, twt.OriginalTrans);
					}
				}
			}
		}

		protected virtual void PreselectAccountGroups()
		{
			if (accountGroups == null)
			{
				accountGroups = new Dictionary<int, AccountGroup>();
				PXSelectBase<PMAccountGroup> select = new PXSelect<PMAccountGroup, Where<PMAccountGroup.isActive, Equal<True>>>(this);

				using (new PXFieldScope(select.View, new Type[] { typeof(PMAccountGroup.groupID), typeof(PMAccountGroup.groupCD) }))
				{
					foreach (PMAccountGroup ag in select.Select())
					{
						accountGroups.Add(ag.GroupID.Value, new AccountGroup(ag.GroupID.Value, ag.GroupCD));
					}
				}
			}
		}

		protected virtual void PreSelectTaskTransactions(PMTask task)
		{
			if (task.AllocationID == null)
				return;

			transactions.Clear();
			Dictionary<int, List<PXResult<PMTran>>> transactionsByAccountGroup = new Dictionary<int, List<PXResult<PMTran>>>();
			HashSet<int> distinctAccountGroups = new HashSet<int>();
			HashSet<string> distinctRateTypes = new HashSet<string>();

			AllocationInfo info;
			if (!allocationInfo.TryGetValue(task.AllocationID, out info))
			{
				List<PMAllocationStep> steps = new List<PMAllocationStep>();
				info = new AllocationInfo(distinctAccountGroups, distinctRateTypes, steps);
				allocationInfo.Add(task.AllocationID, info);

				foreach (PMAllocationStep step in PXSelect<PMAllocationStep, Where<PMAllocationStep.allocationID, Equal<Required<PMAllocationStep.allocationID>>>,
						OrderBy<Asc<PMAllocationStep.stepID>>>.Select(this, task.AllocationID))
				{
					if (step.Method == PMMethod.Budget)
						info.ContainsBudgetStep = true;
					foreach (int ag in GetAccountGroups(step))
					{
						distinctAccountGroups.Add(ag);
					}
					distinctRateTypes.Add(step.RateTypeID);
					steps.Add(step);
				}
			}
			else
			{
				distinctAccountGroups = info.AccountGroups;
				distinctRateTypes = info.RateTypes;
			}
			
			
			foreach (int distinctAccountGroup in distinctAccountGroups)
			{
				transactionsByAccountGroup.Add(distinctAccountGroup, new List<PXResult<PMTran>>(GetTranFromDatabase(task.ProjectID, task.TaskID, distinctAccountGroup)));
			}

			transactions.Add(task.TaskID.Value, transactionsByAccountGroup);

			rateEngine = CreateRateEngineV2( new string[1] {task.RateTableID}, distinctRateTypes.ToList());
		}

		protected virtual bool PreSelectTasksTransactions(List<PMTask> tasks)
		{
			transactions.Clear();

			if (tasks.Count == 0) return false;

			bool tryToAllocate = false;
			
			HashSet<string> distinctAllocationRules = new HashSet<string>();
			HashSet<string> distinctRateTables = new HashSet<string>();
			HashSet<int> distinctProjects = new HashSet<int>();
			foreach (PMTask task in tasks)
			{
				if (task.AllocationID == null)
					continue;

				distinctAllocationRules.Add(task.AllocationID);
				transactions.Add(task.TaskID.Value, new Dictionary<int, List<PXResult<PMTran>>>());
				distinctRateTables.Add(task.RateTableID);
				distinctProjects.Add(task.ProjectID.Value);
			}

			HashSet<int> distinctAccountGroups = new HashSet<int>();
			HashSet<string> distinctRateTypes = new HashSet<string>(); 

			foreach(string allocationID in distinctAllocationRules)
			{
				AllocationInfo info;
				if (!allocationInfo.TryGetValue(allocationID, out info))
				{
					List<PMAllocationStep> steps = new List<PMAllocationStep>();
					info = new AllocationInfo(distinctAccountGroups, distinctRateTypes, steps);
					allocationInfo.Add(allocationID, info);

					foreach (PMAllocationStep step in PXSelect<PMAllocationStep, Where<PMAllocationStep.allocationID, Equal<Required<PMAllocationStep.allocationID>>>,
							OrderBy<Asc<PMAllocationStep.stepID>>>.Select(this, allocationID))
					{
						if (step.Method == PMMethod.Budget)
							info.ContainsBudgetStep = true;
						foreach (int ag in GetAccountGroups(step))
						{
							distinctAccountGroups.Add(ag);
						}
						distinctRateTypes.Add(step.RateTypeID);
						steps.Add(step);
					}
				}
				else
				{
					distinctAccountGroups = info.AccountGroups;
					distinctRateTypes = info.RateTypes;
				}

				tryToAllocate = info.ContainsBudgetStep;
			}

			foreach (int distinctAccountGroup in distinctAccountGroups)
			{
				foreach (int projectID in distinctProjects)
				{
					foreach (PXResult<PMTran> tran in GetTranFromDatabase(projectID, distinctAccountGroup))
					{
						tryToAllocate = true;

						Dictionary<int, List<PXResult<PMTran>>> transactionsByAccountGroup;
						
						if (transactions.TryGetValue(((PMTran) tran).TaskID.Value, out transactionsByAccountGroup))
						{
							List<PXResult<PMTran>> trans;
							if (!transactionsByAccountGroup.TryGetValue(distinctAccountGroup, out trans))
							{
								trans = new List<PXResult<PMTran>>();
								transactionsByAccountGroup.Add(distinctAccountGroup, trans);
							}
							
							trans.Add(tran);
						}
					}
				}
			}

			if (!tryToAllocate)
				return false;

			rateEngine = CreateRateEngineV2(distinctRateTables.ToList(), distinctRateTypes.ToList());

			return true;
		}

		/// <summary>
		/// When overriding in customization return null in order for system to use RateEngineV1
		/// </summary>
		public virtual RateEngineV2 CreateRateEngineV2(IList<string> rateTables, IList<string> rateTypes)
		{
			return new RateEngineV2(this, rateTables, rateTypes);
		}

		/// <summary>
		/// Returns distinct account groups for the given step. 
		/// Method relies on pre-selected data stored in allocationInfo and accountGroups collections.
		/// Method do not query database.
		/// </summary>
		/// <param name="step">Allocation step</param>
		/// <returns></returns>
		protected virtual HashSet<int> GetAccountGroups(PMAllocationStep step)
		{
			HashSet<int> set = new HashSet<int>();
			if (step.SelectOption == PMSelectOption.Step)
			{
				foreach (PMAllocationStep innerStep in GetSteps(step.AllocationID, step.RangeStart, step.RangeEnd))
				{
					foreach (int ag in GetAccountGroups(innerStep))
					{
						set.Add(ag);
					}
				}
			}
			else
			{
				foreach (int groupID in GetAccountGroupsRange(step))
					set.Add(groupID);
			}

			return set;
		}

		/// <summary>
		/// Returns distinct account groups for the Range defined in the given step. 
		/// Method relies on pre-selected data stored in accountGroups collection.
		/// Method do not query database.
		/// </summary>
		protected virtual IList<int> GetAccountGroupsRange(PMAllocationStep step)
		{
			List<int> list = new List<int>();

			if ((step.AccountGroupFrom != null && step.AccountGroupTo == null) ||
				 (step.AccountGroupFrom != null && step.AccountGroupTo == step.AccountGroupFrom))
			{
				list.Add(step.AccountGroupFrom.Value);
			}

			if (step.AccountGroupFrom == null && step.AccountGroupTo != null)
			{
				list.Add(step.AccountGroupFrom.Value);
			}

			if (step.AccountGroupTo != null && step.AccountGroupTo != step.AccountGroupFrom)
			{
				AccountGroup fromGroup;
				AccountGroup toGroup;

				if (!accountGroups.TryGetValue(step.AccountGroupFrom.Value, out fromGroup))
				{
					throw new PXException(Messages.AccountGroupInAllocationStepFromNotFound, step.AllocationID, step.StepID, step.AccountGroupFrom);
				}

				if (!accountGroups.TryGetValue(step.AccountGroupTo.Value, out toGroup))
				{
					throw new PXException(Messages.AccountGroupInAllocationStepToNotFound, step.AllocationID, step.StepID, step.AccountGroupTo);
				}

				foreach (AccountGroup ag in accountGroups.Values)
				{
					if (string.Compare(ag.GroupCD, fromGroup.GroupCD, StringComparison.InvariantCultureIgnoreCase) >= 0 && string.Compare(ag.GroupCD, toGroup.GroupCD, StringComparison.InvariantCultureIgnoreCase) <= 0)
					{
						list.Add(ag.GroupID);
					}
				}
			}

			return list;
		}

		/// <summary>
		/// Returns list of inner steps for the given Range.
		/// Method relies on pre-selected data stored in allocationInfo collection.
		/// Method do not query database.
		/// </summary>
		protected virtual IList<PMAllocationStep> GetSteps(string allocationID, int? rangeStart, int? rangeEnd)
		{
			List<PMAllocationStep> list = new List<PMAllocationStep>();
			foreach (PMAllocationStep innerStep in allocationInfo[allocationID].Steps)
			{
				if (innerStep.StepID >= rangeStart && innerStep.StepID <= rangeEnd)
				{
					list.Add(innerStep);
				}
			}

			return list;
		}

		/// <summary>
		/// Returns RateDefinitions from Cached rateDefinitions collection or from database if not found.
		/// </summary>
		public virtual IList<PMRateDefinition> GetRateDefinitions(string rateTable)
		{
			List<PMRateDefinition> result;
			if (!rateDefinitions.TryGetValue(rateTable, out result))
			{
				PXSelectBase<PMRateDefinition> select = new PXSelect<PMRateDefinition,
			   Where<PMRateDefinition.rateTableID, Equal<Required<PMRateDefinition.rateTableID>>>,
			   OrderBy<Asc<PMRateDefinition.rateTypeID, Asc<PMRateDefinition.sequence>>>>(this);

				result = new List<PMRateDefinition>(select.Select(rateTable).RowCast<PMRateDefinition>());
				rateDefinitions.Add(rateTable, result);
			}

			return result;
		} 

		public virtual void ReverseCreditMemo(string refNbr, List<PXResult<ARTran, PMTran>> list)
		{
			PMRegister doc = Document.Insert();
			doc.OrigDocType = PMOrigDocType.CreditMemo;
			doc.OrigRefNbr = refNbr;

			foreach (PXResult<ARTran, PMTran> item in list)
			{
				ARTran ar = (ARTran)item;
				PMTran pm = (PMTran)item;

				PMTran newTran = PXCache<PMTran>.CreateCopy(pm);
				newTran.TranID = null;
				newTran.TranType = null;
				newTran.RefNbr = null;
				newTran.RefLineNbr = null;
				newTran.BatchNbr = null;
				newTran.TranDate = null;
				newTran.TranPeriodID = null;
				newTran.Released = null;
				newTran.Billed = null;
				newTran.BilledDate = null;
				newTran.Description = ar.TranDesc;
				newTran.UOM = ar.UOM;
				newTran.Qty = ar.Qty;
				newTran.BillableQty = ar.Qty;
				newTran.Amount = ar.TranAmt;

				Transactions.Insert(newTran);
			}
        }

		public virtual void NonGlBillLaterAndReverse(string refNbr, List<PXResult<ARTran, PMTran>> billLater, List<PXResult<ARTran, PMTran>> reverseList)
		{
			if (billLater == null) throw new ArgumentNullException("billLater");
			if (reverseList == null) throw new ArgumentNullException("reverseList");

			PMRegister doc = Document.Insert();
			doc.OrigDocType = reverseList.Count > 0 ? PMOrigDocType.AllocationReversal : PMOrigDocType.UnbilledRemainder;
			doc.OrigRefNbr = refNbr;

			//Reversal on NON-GL if any:
			foreach (PXResult<ARTran, PMTran> item in reverseList)
			{
				ARTran ar = (ARTran)item;
				PMTran pm = (PMTran)item;

				//debit
				PMTran newTran = PXCache<PMTran>.CreateCopy(pm);
				newTran.AccountGroupID = pm.AccountGroupID;
				newTran.Date = ar.TranDate;
				newTran.FinPeriodID = ar.FinPeriodID;
				newTran.OffsetAccountGroupID = null;
				newTran.TranID = null;
				newTran.TranType = null;
				newTran.RefNbr = null;
				newTran.RefLineNbr = null;
				newTran.BatchNbr = null;
				newTran.TranDate = null;
				newTran.TranPeriodID = null;
				newTran.Released = null;
				newTran.Allocated = true;
				newTran.Billed = true;//Must be excluded from billing base
				newTran.BilledDate = null;
				newTran.Qty = -pm.Qty;
				newTran.BillableQty = -pm.BillableQty;
				newTran.Amount = -pm.Amount;

				newTran = Transactions.Insert(newTran);


				//credit
				if (pm.OffsetAccountGroupID != null)
				{
					PMTran revTran = PXCache<PMTran>.CreateCopy(pm);
					revTran.Date = ar.TranDate;
					revTran.FinPeriodID = ar.FinPeriodID;
					revTran.AccountGroupID = pm.OffsetAccountGroupID;
					revTran.OffsetAccountGroupID = null;
					revTran.TranID = null;
					revTran.TranType = null;
					revTran.RefNbr = null;
					revTran.RefLineNbr = null;
					revTran.BatchNbr = null;
					revTran.TranDate = null;
					revTran.TranPeriodID = null;
					revTran.Released = null;
					revTran.Allocated = true;
					revTran.Billed = true;//Must be excluded from billing base
					revTran.BilledDate = null;
					Transactions.Insert(revTran);
				}
			}

			//Bill later Remainder if any:
			foreach (PXResult<ARTran, PMTran> item in billLater)
			{
				ARTran ar = (ARTran)item;
				PMTran pm = (PMTran)item;

				//debit
				PMTran newTran = PXCache<PMTran>.CreateCopy(pm);
				newTran.AccountGroupID = pm.OffsetAccountGroupID ?? pm.AccountGroupID;
				newTran.TranID = null;
				newTran.TranType = null;
				newTran.RefNbr = null;
				newTran.RefLineNbr = null;
				newTran.BatchNbr = null;
				newTran.TranDate = null;
				newTran.TranPeriodID = null;
				newTran.Released = null;
				newTran.Allocated = true;
				newTran.Billed = null;
				newTran.BilledDate = null;
				newTran.Description = ar.TranDesc;
				newTran.UOM = ar.UOM;
				newTran.Qty = ar.Qty;
				newTran.BillableQty = ar.Qty;
				newTran.Amount = pm.Amount - ar.TranAmt;

				newTran = Transactions.Insert(newTran);


				//credit
				if (pm.OffsetAccountGroupID != null)
				{
					PMTran revTran = PXCache<PMTran>.CreateCopy(newTran); 
					newTran.AccountGroupID = pm.AccountGroupID;
					revTran.TranID = null;
					revTran.TranType = null;
					revTran.RefNbr = null;
					revTran.RefLineNbr = null;
					revTran.BatchNbr = null;
					revTran.TranDate = null;
					revTran.TranPeriodID = null;
					revTran.Released = null;
					revTran.Allocated = true;
					revTran.Billed = null;
					revTran.BilledDate = null;
					revTran.Amount = -newTran.Amount;
					Transactions.Insert(revTran);
				}
			}
		}
		
		protected virtual bool ProcessStep(PMTask task, PMAllocationStep step)
		{
			if (step.Post == true)
			{
				if (step.Method == PMMethod.Transaction)
				{
					List<PMTran> sourceList = Select(step, task.ProjectID, task.TaskID);
					if (sourceList.Count > 0)
					{
						List<PMTran> allocatedList = new List<PMTran>();
						Post(step, task, sourceList, allocatedList);
						AddSourceTrans(step, allocatedList);

						foreach (PMTran allocated in allocatedList)
						{
							allocated.Allocated = true;
							Transactions.Update(allocated);
						}
					}
				}
				else
				{
					if (ProcessBudgetStep(step, task))
						return true;
				}
			}
			return false;
		}

		protected virtual void Post(PMAllocationStep step, PMTask task, List<PMTran> sourceList, List<PMTran> allocatedList)
		{
			CapService capService = new CapService(this, step, task);
			AllocatedService allocService = new AllocatedService(this, task);

			List<PMTranWithTrace> allocated = null;
			if (step.FullDetail == true)
			{
				allocated = PostFullDetail(step, task, sourceList, allocatedList, capService, allocService);
			}
			else
			{
				allocated = PostSummary(step, task, sourceList, allocatedList, capService, allocService);
			}

			PostAllocatedTrans(step, allocated);
		}

		protected virtual List<PMTranWithTrace> PostFullDetail(PMAllocationStep step, PMTask task, List<PMTran> list, List<PMTran> allocatedList, CapService cap, AllocatedService allocService)
		{
			List<PMTranWithTrace> result = new List<PMTranWithTrace>(list.Count);

			foreach (PMTran original in list)
			{
				
				string note = null;
				Guid[] files = null;
				if (step.AllocateNonBillable == true || original.Billable == true)
				{
					original.Rate = GetRate(step, original, task.RateTableID);

					if (original.Rate == null)
					{
						//do not allocate option is selected.
						continue;
					}

					IList<PMTran> allocateCandidates = Transform(step, task, original, true, cap.IsWipStep);

					foreach (PMTran allocateCandidate in allocateCandidates)
					{
						List<PMTranWithTrace> allocTrans = ApplyLimits(step, allocateCandidate, original.AccountGroupID.Value, new List<long>(new long[] { original.TranID.Value }), cap, allocService);
						if (allocTrans.Count > 0)
						{
							note = PXNoteAttribute.GetNote(Transactions.Cache, original);
							files = PXNoteAttribute.GetFileNotes(Transactions.Cache, original);

							foreach (PMTranWithTrace item in allocTrans)
							{
								item.NoteText = note;
								item.Files = files;
								result.Add(item);
							}

							allocatedList.Add(original);
						}
						else
						{
							break;//exit foreach; Do not process the second (credit) transaction if the debit was not allocated 
						}

					}
				}
			}

			return result;
		}

		protected virtual List<PMTranWithTrace> PostSummary(PMAllocationStep step, PMTask task, List<PMTran> fullList, List<PMTran> allocatedList, CapService cap, AllocatedService allocService)
		{
			List<Group> groups = BreakIntoGroups(step, fullList);
			List<PMTranWithTrace> result = new List<PMTranWithTrace>();

			foreach (Group group in groups)
			{
				PMDataNavigator navigator = new PMDataNavigator(this, group.List);
				List<long> originalTrans = new List<long>();
				
				decimal sumQty = 0;
				decimal sumBillableQty = 0;
				decimal sumAmt = 0;
				string lastDesc = null;
				DateTime? startDate = null;
				DateTime? endDate = null;
				foreach (PMTran tr in group.List)
				{
					if (startDate == null)
					{
						startDate = tr.StartDate;
					}
					else if (startDate > tr.StartDate)
					{
						startDate = tr.StartDate;
					}

					if (endDate == null)
					{
						endDate = tr.EndDate;
					}
					else if (endDate < tr.EndDate)
					{
						endDate = tr.EndDate;
					}

					decimal? qty = 0, billableQty = 0, amt = 0;
					string desc = null;

					tr.Rate = GetRate(step, tr, task.RateTableID);

					if (tr.Rate == null)
					{
						//do not allocate option is selected.
						break;//exit foreach; Do not process the second (credit) transaction if the debit was not allocated 
					}


					CalculateFormulas(navigator, step, tr, out qty, out billableQty, out amt, out desc);
					lastDesc = desc;

					decimal qtyInBase = qty ?? 0;
					if(tr.InventoryID != PMInventorySelectorAttribute.EmptyInventoryID && !string.IsNullOrEmpty(tr.UOM))
					{
						qtyInBase = ConvertQtyToBase(tr.InventoryID, tr.UOM, qty ?? 0);
					}


					originalTrans.Add(tr.TranID.Value);

					sumQty += qty.GetValueOrDefault();
					sumBillableQty += billableQty.GetValueOrDefault();
					sumAmt += amt.GetValueOrDefault();
				}

				if (group.List.Count > 0)
				{
					IList<PMTran> allocs = Transform(step, task, group.List[0], false, cap.IsWipStep);

					foreach (PMTran alloc in allocs)
					{
						alloc.Qty = alloc.IsInverted == true ? -sumQty : sumQty;
						alloc.BillableQty = alloc.IsInverted == true ? -sumBillableQty : sumBillableQty;
						alloc.Amount = alloc.IsInverted == true ? -CM.PXCurrencyAttribute.BaseRound(this, sumAmt) : CM.PXCurrencyAttribute.BaseRound(this, sumAmt);
						alloc.StartDate = startDate;
						alloc.EndDate = endDate;

						if (alloc.BillableQty != 0)
						{
							decimal amt = PX.Objects.CM.PXCurrencyAttribute.PXCurrencyHelper.BaseRound(this, alloc.Amount.GetValueOrDefault());
							decimal qty = PXDBQuantityAttribute.Round(alloc.BillableQty.Value);
							decimal unitRate = PXDBPriceCostAttribute.Round(amt / qty);

							alloc.UnitRate = unitRate;
						}
						else if (alloc.Qty != 0)
						{
							decimal amt = PX.Objects.CM.PXCurrencyAttribute.PXCurrencyHelper.BaseRound(this, alloc.Amount.GetValueOrDefault());
							decimal qty = PXDBQuantityAttribute.Round(alloc.Qty.Value);
							decimal unitRate = PXDBPriceCostAttribute.Round(amt / qty);

							alloc.UnitRate = unitRate;
						}

						if (group.HasMixedInventory)
						{
							alloc.InventoryID = PMInventorySelectorAttribute.EmptyInventoryID; //mixed inventory in components
						}
						if (group.HasMixedUOM)
						{
							alloc.Qty = 0;
							alloc.BillableQty = 0;
							alloc.UOM = null;
							alloc.UnitRate = 0;
						}
						if (group.HasMixedBAccount)
						{
							alloc.BAccountID = null;
							alloc.LocationID = null;
						}

						if (group.HasMixedBAccountLoc)
						{
							alloc.LocationID = null;
						}
						if (lastDesc == null && group.HasMixedDescription)
						{
							alloc.Description = GetConcatenatedDescription(step, alloc, task);
						}
						else
						{
							alloc.Description = lastDesc;
						}


						List<PMTranWithTrace> allocTrans = ApplyLimits(step, alloc, group.List[0].AccountGroupID.Value, originalTrans, cap, allocService);
						if (allocTrans.Count > 0)
						{
							foreach (PMTranWithTrace item in allocTrans)
							{
                                var files = new List<Guid>();
                                foreach (PMTran original in group.List)
                                {
                                    files.AddRange(PXNoteAttribute.GetFileNotes(Transactions.Cache, original));
                                }
                                item.Files = files.ToArray();
								result.Add(item);
							}

							foreach (PMTran tr in group.List)
							{
								allocatedList.Add(tr);
							}
						}

						
					}
				}
			}

			return result;
		}
		
	    protected virtual List<PMTranWithTrace> ApplyLimits(PMAllocationStep step, PMTran allocateCandidate, int originalAccountGroupID, List<long> originalTranIDs, CapService cap, AllocatedService allocService)
	    {
			List<PMTranWithTrace> result = new List<PMTranWithTrace>();

			bool allocate = false;
			bool split = false;
			decimal availableAmt = 0;

			if (CanAllocate(step, allocateCandidate))
			{
				allocate = true;
				split = false;

				decimal allocateCandidateAmt = allocateCandidate.IsInverted == true ? -allocateCandidate.Amount.Value : allocateCandidate.Amount.Value;

				if (cap.LimitAmt && allocateCandidate.IsCreditPair != true)
				{
					if (cap.IsCapsDefined(allocateCandidate.InventoryID.Value, allocateCandidate.AccountGroupID))
					{
						availableAmt = cap.GetAmtCap(allocateCandidate.InventoryID.Value, allocateCandidate.AccountGroupID) -
											   allocService.GetAllocatedAmt(originalAccountGroupID, allocateCandidate.InventoryID.Value);

					}
					else
					{
						availableAmt = cap.CommomnCapAmt - allocService.GetAllocatedAmt(originalAccountGroupID);
					}

					if (cap.LimitAmt)
					{
						if (availableAmt <= 0)
						{
							allocate = false;
						}
						else
						{
							if (availableAmt < allocateCandidateAmt)
							{
								split = true;
							}
						}
					}
				}


				if (allocate)
				{
					decimal qty = allocateCandidate.IsInverted == true ? -(allocateCandidate.Qty ?? 0) : (allocateCandidate.Qty ?? 0);
					decimal qtyInBase = qty;
					if (allocateCandidate.InventoryID != PMInventorySelectorAttribute.EmptyInventoryID)
					{
						qtyInBase = ConvertQtyToBase(allocateCandidate.InventoryID, allocateCandidate.UOM, qty);
					}
					if (cap.LimitQty && allocateCandidate.IsCreditPair != true)
					{
						if (cap.IsCapsDefined(allocateCandidate.InventoryID.Value, allocateCandidate.AccountGroupID))
						{
							if (cap.LimitQty && cap.GetQtyCap(allocateCandidate.InventoryID.Value, allocateCandidate.AccountGroupID) < allocService.GetAllocatedQty(originalAccountGroupID, allocateCandidate.InventoryID.Value) + qtyInBase)
							{
								allocate = false;
							}
							else
							{
								allocService.AddToAllocatedQty(originalAccountGroupID, allocateCandidate.InventoryID.Value, qtyInBase);
							}
						}

						if (allocate)
							allocService.AddToAllocatedAmt(originalAccountGroupID, allocateCandidate.InventoryID.Value, allocateCandidateAmt);
					}
					else if (!cap.LimitQty && cap.LimitAmt && allocateCandidate.IsCreditPair != true)//only by amount
					{
						allocService.AddToAllocatedAmt(originalAccountGroupID, allocateCandidate.InventoryID.Value, split ? availableAmt : allocateCandidateAmt);
					}

				}
			}

			if (allocate)
			{
				PMTran nonBillable = null;

				if (split)
				{
					decimal nonBillableAmt = allocateCandidate.Amount.GetValueOrDefault() - availableAmt;
					allocateCandidate.Amount = availableAmt;

					if (nonBillableAmt > 0 && cap.OverflowAccountGroup != null)
					{
						nonBillable = (PMTran)Transactions.Cache.CreateInstance();
						nonBillable.BranchID = allocateCandidate.BranchID;
						nonBillable.AccountGroupID = cap.OverflowAccountGroup;
						nonBillable.ProjectID = allocateCandidate.ProjectID;
						nonBillable.TaskID = allocateCandidate.TaskID;
						nonBillable.BAccountID = allocateCandidate.BAccountID;
						nonBillable.Description = allocateCandidate.Description;
						nonBillable.LocationID = allocateCandidate.LocationID;
						nonBillable.ResourceID = allocateCandidate.ResourceID;
						nonBillable.InventoryID = allocateCandidate.InventoryID;
						nonBillable.UOM = allocateCandidate.UOM;
						nonBillable.Qty = allocateCandidate.Qty;
						nonBillable.UnitRate = allocateCandidate.UnitRate;
						nonBillable.Date = allocateCandidate.Date;
						nonBillable.FinPeriodID = allocateCandidate.FinPeriodID;
						nonBillable.Allocated = step.MarkAsNotAllocated != true;
						nonBillable.BillableQty = 0;
						nonBillable.Billable = false;
						nonBillable.Reverse = PMReverse.Never;
						nonBillable.Amount = nonBillableAmt;
					}
				}

				PMTranWithTrace ptwt = new PMTranWithTrace(allocateCandidate, originalTranIDs);
				
				result.Add(ptwt);

				if (nonBillable != null)
				{
					PMTranWithTrace nbTran = new PMTranWithTrace(nonBillable, new List<long>());//empty list is passed - audit trail is not recorded against this tran.
					result.Add(nbTran);
				}

			}
			
		    return result;
	    }
		
		protected virtual void PostAllocatedTrans(PMAllocationStep step, List<PMTranWithTrace> allocated)
		{
			foreach (PMTranWithTrace twt in allocated)
			{
				if (Document.Current == null)
					AddAllocationDocument();

				twt.Tran = Transactions.Insert(twt.Tran); //TranID is initialized -1,-2,-3, etc.
				twt.Tran.BatchNbr = null;//allocation can be called from journal entry and BatchNbr will be defaulted.
				if (twt.NoteText != null)
					PXNoteAttribute.SetNote(Transactions.Cache, twt.Tran, twt.NoteText);
				if (twt.Files != null && twt.Files.Length > 0)
					PXNoteAttribute.SetFileNotes(Transactions.Cache, twt.Tran, twt.Files);

				if (twt.Tran.OrigAccountGroupID != null)
				{
					PMTaskAllocTotalAccum ta = new PMTaskAllocTotalAccum();
					ta.ProjectID = twt.Tran.OrigProjectID;
					ta.TaskID = twt.Tran.OrigTaskID;
					ta.AccountGroupID = twt.Tran.OrigAccountGroupID;
					ta.InventoryID = twt.Tran.InventoryID;

					if (ta.ProjectID == null)
						throw new PXException(Messages.ProjectIsNullAfterAllocation, step.StepID);

					if (ta.TaskID == null)
						throw new PXException(Messages.TaskIsNullAfterAllocation, step.StepID);

					if (ta.AccountGroupID == null)
						throw new PXException(Messages.AccountGroupIsNullAfterAllocation, step.StepID);
					
					ta = TaskTotals.Insert(ta);
					ta.Amount += twt.Tran.Amount;
					ta.Quantity += twt.Tran.BillableQty;
				}
			}

			stepResults.Add(step.StepID.Value, allocated);
		}

		protected virtual bool ProcessBudgetStep(PMAllocationStep step, PMTask task)
		{
			bool tranAdded = false;
			decimal? taskCompletedPct = null;

			#region Get Task Completed % from Production item

			if (task.CompletedPctMethod != PMCompletedPctMethod.Manual)
			{
				PXSelectBase<PMProjectStatus> selectProduction = new PXSelectGroupBy<PMProjectStatus,
								  Where<PMProjectStatus.projectID, Equal<Required<PMTask.projectID>>,
								  And<PMProjectStatus.projectTaskID, Equal<Required<PMTask.taskID>>,
								  And<PMProjectStatus.isProduction, Equal<True>>>>,
								  Aggregate<GroupBy<PMProjectStatus.accountGroupID,
								  GroupBy<PMProjectStatus.projectID,
								  GroupBy<PMProjectStatus.projectTaskID,
								  GroupBy<PMProjectStatus.inventoryID,
								  Sum<PMProjectStatus.amount,
								  Sum<PMProjectStatus.qty,
								  Sum<PMProjectStatus.revisedAmount,
								  Sum<PMProjectStatus.revisedQty,
								  Sum<PMProjectStatus.actualAmount,
								  Sum<PMProjectStatus.actualQty>>>>>>>>>>>>(this);

				PXResultset<PMProjectStatus> ps = selectProduction.Select(task.ProjectID, task.TaskID);

				if (ps != null)
				{
					double percentSum = 0;
					Int32 recordCount = 0;
					decimal actualAmount = 0;
					decimal revisedAmount = 0;


					foreach (PMProjectStatus item in ps)
					{

						if (task.CompletedPctMethod == PMCompletedPctMethod.ByQuantity && item.RevisedQty > 0)
						{
							recordCount++;
							percentSum += Convert.ToDouble(100 * item.ActualQty / item.RevisedQty);
						}
						else if (task.CompletedPctMethod == PMCompletedPctMethod.ByAmount)
						{
							recordCount++;
							actualAmount += item.ActualAmount.GetValueOrDefault(0);
							revisedAmount += item.RevisedAmount.GetValueOrDefault(0);
						}

						//Not persisted yet balances (autoallocation for unpersisted transactions/balances):
						foreach (PMProjectStatusAccum psa in this.Caches[typeof(PMProjectStatusAccum)].Inserted)
						{
							if (psa.ProjectID == item.ProjectID &&
									psa.ProjectTaskID == item.ProjectTaskID &&
									psa.AccountGroupID == item.AccountGroupID &&
									psa.InventoryID == item.InventoryID)
							{
								if (task.CompletedPctMethod == PMCompletedPctMethod.ByQuantity && psa.RevisedQty > 0)
								{
									recordCount++;
									percentSum += Convert.ToDouble(100 * psa.ActualQty / psa.RevisedQty);
								}
								else if (task.CompletedPctMethod == PMCompletedPctMethod.ByAmount)
								{
									recordCount++;
									actualAmount += psa.ActualAmount.GetValueOrDefault(0);
									revisedAmount += psa.RevisedAmount.GetValueOrDefault(0);
								}
							}
						}
					}



					if (task.CompletedPctMethod == PMCompletedPctMethod.ByAmount)
						taskCompletedPct = revisedAmount == 0 ? 0 : 100 * actualAmount / revisedAmount;
					else
						taskCompletedPct = Convert.ToInt32(percentSum) == 0 ? 0 : Convert.ToDecimal(percentSum / recordCount);
				}

			}
			else
			{
				//manual task progress:
				taskCompletedPct = task.CompletedPct;
			}
			#endregion


			AllocatedService allocService = new AllocatedService(this, task);

			PXSelectBase<PMProjectStatus> selectBudget = new PXSelectGroupBy<PMProjectStatus,
				   Where<PMProjectStatus.projectID, Equal<Required<PMProjectStatus.projectID>>,
				   And<PMProjectStatus.projectTaskID, Equal<Required<PMProjectStatus.projectTaskID>>,
				   And<PMProjectStatus.accountGroupID, Equal<Required<PMProjectStatus.accountGroupID>>>>>,
				   Aggregate<
				   GroupBy<PMProjectStatus.accountGroupID,
				   GroupBy<PMProjectStatus.projectID,
				   GroupBy<PMProjectStatus.projectTaskID,
				   GroupBy<PMProjectStatus.inventoryID,
				   Sum<PMProjectStatus.amount,
				   Sum<PMProjectStatus.qty,
				   Sum<PMProjectStatus.revisedAmount,
				   Sum<PMProjectStatus.revisedQty,
				   Sum<PMProjectStatus.actualAmount,
				   Sum<PMProjectStatus.actualQty>>>>>>>>>>>>(this);

			List<AllocData> data = new List<AllocData>();
			Dictionary<int, decimal> uncapTotalAmtByItem = new Dictionary<int, decimal>();
			Dictionary<int, decimal> uncapTotalQtyByItem = new Dictionary<int, decimal>();
			decimal uncapTotal = 0;

			foreach (int groupID in GetAccountGroupsRange(step))
			{
				foreach (PMProjectStatus budget in selectBudget.Select(task.ProjectID, task.TaskID, groupID))
				{
					if (taskCompletedPct > 0 && budget.RevisedAmount > 0)
					{
						AllocData ad = new AllocData(budget.AccountGroupID.Value, budget.InventoryID.Value);
						ad.Amount = PX.Objects.CM.PXCurrencyAttribute.BaseRound(this, budget.RevisedAmount.Value * taskCompletedPct.Value * 0.01m);
						ad.Quantity = budget.RevisedQty.Value * taskCompletedPct.Value * 0.01m;
						ad.UOM = budget.UOM;
						data.Add(ad);

						uncapTotal += ad.Amount;

						if (uncapTotalAmtByItem.ContainsKey(budget.InventoryID.Value))
							uncapTotalAmtByItem[budget.InventoryID.Value] += ad.Amount;
						else
							uncapTotalAmtByItem.Add(budget.InventoryID.Value, ad.Amount);



						decimal qtyInBase = ad.Quantity;
						if (ad.InventoryID != PMInventorySelectorAttribute.EmptyInventoryID && ad.UOM != null)
						{
							qtyInBase = ConvertQtyToBase(ad.InventoryID, ad.UOM, ad.Quantity);
						}

						if (uncapTotalQtyByItem.ContainsKey(budget.InventoryID.Value))
						{
							uncapTotalQtyByItem[budget.InventoryID.Value] += qtyInBase;
						}
						else
						{
							uncapTotalQtyByItem.Add(budget.InventoryID.Value, qtyInBase);
						}
					}
				}
			}

			CapService capService = new CapService(this, step, task);

			List<AllocData> listOfCommomCapRecords = new List<AllocData>();
			decimal alreadyAllocatedForCommonRecords = 0;

			foreach (AllocData ad in data)
			{
				decimal coeff = 1;

				//Filter Common(Generic) Caps:
				if (capService.LimitAmt)
				{
					if (!capService.IsCapsDefined(ad.InventoryID, ad.AccountGroupID))
					{
						alreadyAllocatedForCommonRecords += allocService.GetAllocatedAmt(ad.AccountGroupID, ad.InventoryID);
						listOfCommomCapRecords.Add(ad);
						continue;
					}
				}

				//Process Item Specific Caps:
				if (capService.LimitAmt)
				{
					decimal maxAmt = capService.GetAmtCap(ad.InventoryID, ad.AccountGroupID);
					decimal totalByItem = uncapTotalAmtByItem[ad.InventoryID];
					if (maxAmt > 0 && totalByItem > maxAmt)
					{
						coeff = maxAmt / totalByItem;
					}
				}

				if (capService.LimitQty)
				{
					decimal maxQtyInBase = capService.GetQtyCap(ad.InventoryID, ad.AccountGroupID);
					decimal totalByItemInBase = uncapTotalQtyByItem[ad.InventoryID];
					if (maxQtyInBase > 0 && totalByItemInBase > maxQtyInBase)
					{
						coeff = Math.Min(1, maxQtyInBase / totalByItemInBase);
					}
				}

				decimal unallocatedAmt = PX.Objects.CM.PXCurrencyAttribute.BaseRound(this, ad.Amount * coeff - allocService.GetAllocatedAmt(ad.AccountGroupID, ad.InventoryID));
				decimal quantity = 0;

				if (ad.InventoryID != PMInventorySelectorAttribute.EmptyInventoryID)
				{
					quantity = ad.Quantity * coeff - allocService.GetAllocatedQty(ad.AccountGroupID, ad.InventoryID);
				}

				if (unallocatedAmt != 0)
				{
					AllocateBudget(task, step, ad.AccountGroupID, ad.InventoryID, ad.UOM, unallocatedAmt, quantity);
					tranAdded = true;
				}
			}

			//Process Generic Caps (Only Amount is applicable):
			decimal coeffCommon = 1;
			if (uncapTotal > 0 && uncapTotal > capService.CommomnCapAmt)
			{
				coeffCommon = capService.CommomnCapAmt / uncapTotal;
			}

			foreach (AllocData ad in listOfCommomCapRecords)
			{
				decimal unallocatedAmt = PX.Objects.CM.PXCurrencyAttribute.BaseRound(this, ad.Amount * coeffCommon - allocService.GetAllocatedAmt(ad.AccountGroupID, ad.InventoryID));

				if (unallocatedAmt != 0)
				{
					AllocateBudget(task, step, ad.AccountGroupID, ad.InventoryID, ad.UOM, unallocatedAmt, 0);
					tranAdded = true;
				}
			}

			return tranAdded;
		}

		protected virtual void AllocateBudget(PMTask task, PMAllocationStep step, int origAccountGroupID, int inventoryID, string UOM, decimal amount, decimal quantity)
		{
			/*
				preconditions:
			 * ProjectID both for debit and credit is always = source.
			 * TaskID both for debit and credit is always = source.
			 * Single-sided transactions are allowed for UpdateGL == false.
			 * 
			*/

			bool additionalNonGLCreditTranIsRequired = false;
			int mult = 1;//sets sign for the transaction. since single-sided tran can only be debit, use mult=-1 to credit an account.

			PMTran tran = new PMTran();
			tran.BranchID = step.TargetBranchID ?? this.Accessinfo.BranchID;
			tran.ProjectID = task.ProjectID;
			tran.TaskID = task.TaskID;
			tran.UOM = UOM;
			tran.BAccountID = task.CustomerID;
			tran.Billable = true;
			tran.UseBillableQty = true;
			tran.BillableQty = quantity;
			tran.InventoryID = inventoryID;
			tran.LocationID = task.LocationID;
			tran.Amount = amount;
			tran.Qty = tran.BillableQty;
			if (tran.Qty != null && tran.Qty != 0)
			{
				decimal a = PX.Objects.CM.PXCurrencyAttribute.PXCurrencyHelper.BaseRound(this, tran.Amount.GetValueOrDefault());
				decimal q = PXDBQuantityAttribute.Round(tran.Qty.Value);
				decimal unitRate = PXDBPriceCostAttribute.Round(a / q);

				tran.UnitRate = unitRate;
			}

			tran.AllocationID = step.AllocationID;
			tran.IsNonGL = step.UpdateGL == false;
			tran.AccountGroupID = origAccountGroupID;
			tran.Reverse = step.Reverse;
			tran.CreatedByCurrentAllocation = true;
			if (OverrideAllocationDate != null)
			{
				tran.Date = OverrideAllocationDate;
			}

			if (step.UpdateGL == true)
			{
				#region Debit/Credit

				if (step.AccountGroupOrigin == PMOrigin.Change)
				{
					tran.AccountGroupID = step.AccountGroupID;
				}
				else if (step.AccountGroupOrigin == PMOrigin.FromAccount)
				{
					Account account = PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this, step.AccountID);
					if (account != null)
						tran.AccountGroupID = account.AccountGroupID;
				}

				if (step.ProjectOrigin == PMOrigin.Change)
				{
					tran.ProjectID = step.ProjectID;
				}
				else
				{
					tran.ProjectID = task.ProjectID;
				}

				if (step.TaskOrigin == PMOrigin.Change)
				{
					tran.TaskID = step.TaskID;
				}

				switch (step.AccountOrigin)
				{
					case PMOrigin.Change:
						tran.AccountID = step.AccountID;
						break;
				}

				tran.SubID = step.SubID ?? task.DefaultSubID;

				if (step.OffsetAccountOrigin == PMOrigin.Change)
				{
					tran.OffsetAccountID = step.OffsetAccountID;
				}

				tran.OffsetSubID = step.OffsetSubID;

				if (tran.OffsetAccountID != null && tran.OffsetSubID == null)
				{
					tran.OffsetSubID = tran.SubID;
				}

				//Make Subaccount:
				PMProject project = PXSelect<PMProject, Where<PMProject.contractID, Equal<Required<PMProject.contractID>>>>.Select(this, task.ProjectID);

				if (project == null)
				{
					throw new PXException(Messages.ProjectInTaskNotFound, task.TaskCD, task.ProjectID);
				}

				object value = PMSubAccountMaskAttribute.MakeSub<PMAllocationStep.subMask>(this, step.SubMask,
				new object[] { tran.SubID, step.SubID, project.DefaultSubID, task.DefaultSubID },
				new Type[] { typeof(PMTran.subID), typeof(PMAllocationStep.subID), typeof(PMProject.defaultSubID), typeof(PMTask.defaultSubID) });
				Transactions.Cache.RaiseFieldUpdating<PMTran.subID>(tran, ref value);
				tran.SubID = (int?)value;


				if (tran.OffsetAccountID != null)
				{
					object offsetValue = PMSubAccountMaskAttribute.MakeSub<PMAllocationStep.offsetSubMask>(this, step.OffsetSubMask,
					new object[] { tran.OffsetSubID, step.OffsetSubID, project.DefaultSubID, task.DefaultSubID },
					new Type[] { typeof(PMTran.offsetSubID), typeof(PMAllocationStep.offsetSubID), typeof(PMProject.defaultSubID), typeof(PMTask.defaultSubID) });
					Transactions.Cache.RaiseFieldUpdating<PMTran.offsetSubID>(tran, ref offsetValue);
					tran.OffsetSubID = (int?)offsetValue;
				}

				#endregion
			}
			else
			{
				if (step.AccountGroupOrigin == PMOrigin.None)
				{
					//Single-sided transaction:

					//Take account group from Credit and change the sign of amount and qty.
					if (step.OffsetAccountGroupOrigin == PMOrigin.Change)
					{
						tran.AccountGroupID = step.OffsetAccountGroupID;
					}

					if (step.OffsetProjectOrigin == PMOrigin.Change)
					{
						tran.ProjectID = step.OffsetProjectID;
					}
					else
					{
						tran.ProjectID = task.ProjectID;
					}

					if (step.OffsetTaskOrigin == PMOrigin.Change)
					{
						tran.TaskID = step.OffsetTaskID;
					}

					mult = -1;
				}
				else
				{
					if (step.AccountGroupOrigin == PMOrigin.Change)
					{
						tran.AccountGroupID = step.AccountGroupID;
					}

					if (step.ProjectOrigin == PMOrigin.Change)
					{
						tran.ProjectID = step.ProjectID;
					}
					else
					{
						tran.ProjectID = task.ProjectID;
					}

					if (step.TaskOrigin == PMOrigin.Change)
					{
						tran.TaskID = step.TaskID;
					}

					if (step.OffsetAccountGroupID != null)
					{
						additionalNonGLCreditTranIsRequired = true;
						tran.OffsetAccountGroupID = step.OffsetAccountGroupID;
					}
				}

				//Update: Why do we need  subaccount at all for the non-gl tran???!!!!
				//Make Subaccount:
				//
				//if (step.AccountGroupOrigin != PMOrigin.None)
				//{
				//    PMProject project = PXSelect<PMProject, Where<PMProject.contractID, Equal<Required<PMProject.contractID>>>>.Select(this, task.ProjectID);

				//    if (project == null)
				//    {
				//        throw new PXException(Messages.ProjectInTaskNotFound, task.TaskCD, task.ProjectID);
				//    }

				//    object value = PMSubAccountMaskAttribute.MakeSub<PMAllocationStep.subMask>(this, step.SubMask,
				//    new object[] { tran.SubID, step.SubID, project.DefaultSubID, task.DefaultSubID },
				//    new Type[] { typeof(PMTran.subID), typeof(PMAllocationStep.subID), typeof(PMProject.defaultSubID), typeof(PMTask.defaultSubID) });
				//    Transactions.Cache.RaiseFieldUpdating<PMTran.subID>(tran, ref value);
				//    tran.SubID = (int?)value;
				//}
			}

			PMDataNavigator navigator = new PMDataNavigator(this, new List<PMTran>(new PMTran[1] { tran }));

			decimal? qty, billableQty, amt;
			string desc;
			CalculateFormulas(navigator, step, tran, out qty, out billableQty, out amt, out desc);
			//!!!Amount and Qty should not be calculated by formula for budget allocation!!!  

			tran.Description = desc;
			tran.Allocated = step.MarkAsNotAllocated != true;
			tran.OrigProjectID = task.ProjectID;
			tran.OrigTaskID = task.TaskID;
			tran.OrigAccountGroupID = origAccountGroupID;

			if (Document.Current == null)
				AddAllocationDocument();

			try
			{
				tran = Transactions.Insert(tran);
			}
			catch (PXFieldProcessingException ex)
			{
				if (ex.FieldName.Equals(Transactions.Cache.GetField(typeof(PMTran.locationID)), StringComparison.InvariantCultureIgnoreCase))
				{
					throw new PXException(PXMessages.LocalizeNoPrefix(Messages.LocationNotFound) + " " + ex.Message);
				}
				else
				{
					throw new PXException(PXMessages.LocalizeNoPrefix(Messages.GenericFieldErrorOnAllocation) + " " + ex.Message);
				}
			}

			PMTaskAllocTotalAccum ta = new PMTaskAllocTotalAccum();
			ta.ProjectID = tran.OrigProjectID;
			ta.TaskID = tran.OrigTaskID;
			ta.AccountGroupID = tran.OrigAccountGroupID;
			ta.InventoryID = tran.InventoryID;

			ta = TaskTotals.Insert(ta);
			ta.Amount += tran.Amount;
			ta.Quantity += tran.BillableQty;

			//Apply sign correction 
			tran.Amount *= mult;
			tran.Qty *= mult;
			tran.BillableQty *= mult;

			if (additionalNonGLCreditTranIsRequired)
			{
				PMTran creditTran = PXCache<PMTran>.CreateCopy(tran);
				creditTran.AccountGroupID = step.OffsetAccountGroupID;

				if (step.OffsetProjectOrigin == PMOrigin.Change)
				{
					tran.ProjectID = step.OffsetProjectID;
				}

				if (step.OffsetTaskOrigin == PMOrigin.Change)
				{
					tran.TaskID = step.OffsetTaskID;
				}

				creditTran.OrigProjectID = null;
				creditTran.OrigAccountGroupID = null;
				creditTran.Qty = -creditTran.Qty;
				creditTran.BillableQty = -creditTran.BillableQty;
				creditTran.Amount = -creditTran.Amount;
				if (creditTran.BillableQty != null && creditTran.BillableQty != 0)
				{
					decimal a = PX.Objects.CM.PXCurrencyAttribute.PXCurrencyHelper.BaseRound(this, creditTran.Amount.GetValueOrDefault());
					decimal q = PXDBQuantityAttribute.Round(creditTran.BillableQty.Value);
					decimal unitRate = PXDBPriceCostAttribute.Round(a / q);
					creditTran.UnitRate = unitRate;
					creditTran.Billable = true;
					creditTran.UseBillableQty = true;
				}
				creditTran.TranID = null;
				creditTran.CreatedByCurrentAllocation = true;

				Transactions.Insert(creditTran);
			}
		}

		protected virtual void AddSourceTrans(PMAllocationStep step, List<PMTran> sourceTrans)
		{
			foreach (PMTran sourceTran in sourceTrans)
			{
				PMAllocationSourceTran allocationTran = CreateAllocationTran(step.AllocationID, step.StepID, sourceTran);
				SourceTran.Insert(allocationTran);
			}
		}

		protected virtual void AddAuditTran(string allocationID, long? tranID, List<long> sourceTrans)
		{
			foreach (long sourceTranID in sourceTrans)
			{
				PMAllocationAuditTran at = new PMAllocationAuditTran();
				at.AllocationID = allocationID;
				at.SourceTranID = sourceTranID;
				at.TranID = tranID;

				AuditTran.Insert(at);
			}
		}

		protected virtual void AddAllocationDocument()
		{
			Document.Cache.Insert();
			Document.Current.OrigDocType = PMOrigDocType.Allocation;
			Document.Current.Description = "Allocation";
			Document.Current.IsAllocation = true;

			if (OverrideAllocationDate != null)
				Document.Current.Date = OverrideAllocationDate;
		}
		
		protected virtual string GetConcatenatedDescription(PMAllocationStep step, PMTran tran, PMTask task)
		{
			string result = "";
			if (step.GroupByDate == true)
				result = string.Format("{0}: ", tran.Date);
			if (step.GroupByItem == true)
			{
				Customer customer = null;
				if (task?.CustomerID != null)
				{
					customer = PXSelectReadonly<AR.Customer, Where<AR.Customer.bAccountID, Equal<Required<AR.Customer.bAccountID>>>>.Select(this, task.CustomerID);
				}

				InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, tran.InventoryID);
				if (item != null)
				{
					using (new PXLocaleScope(customer?.LocaleName))
					{
						result += string.Format("{0} {1}", item.InventoryCD, this.Caches[typeof(InventoryItem)].GetValueExt<InventoryItem.descr>(item));
					}
				}
			}
			else if (step.GroupByEmployee == true)
			{
				BAccount employee = PXSelect<BAccount, Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>.Select(this, tran.ResourceID);
				if (employee != null)
				{
					result += string.Format("{0}", employee.AcctCD);
				}
			}

			return result;
		}

		protected virtual List<Group> BreakIntoGroups(PMAllocationStep step, List<PMTran> fullList)
		{
			List<PMTran> list;
			if (step.AllocateNonBillable != true)
			{
				list = new List<PMTran>(fullList.Count);
				foreach (PMTran tran in fullList)
				{
					if (tran.Billable == true)
					{
						list.Add(tran);
					}
					else
					{
						tran.Allocated = true;
						Transactions.Update(tran);
					}
				}
			}
			else
			{
				list = fullList;
			}

			PMTranComparer comparer = new PMTranComparer(step);
			list.Sort(comparer);

			List<Group> groups = new List<Group>();

			for (int i = 0; i < list.Count; i++)
			{
				if (i > 0)
				{
					if (comparer.Compare(list[i], list[i - 1]) == 0)
					{
						groups[groups.Count - 1].List.Add(list[i]);
					}
					else
					{
						Group nextGroup = new Group();
						nextGroup.List.Add(list[i]);
						groups.Add(nextGroup);
					}
				}
				else
				{
					Group firstGroup = new Group();
					firstGroup.List.Add(list[i]);
					groups.Add(firstGroup);
				}
			}

			return groups;
		}

		protected virtual bool CanAllocate(PMAllocationStep step, PMTran tran)
		{
			bool result = false;

			if (step.AllocateZeroQty == true || tran.Qty != 0)
			{
				result = true;
			}

			if (step.AllocateZeroAmount == true || tran.Amount != 0)
			{
				result = true;
			}

			return result;
		}

		public virtual object Evaluate(PMObjectType objectName, string fieldName, string attribute, PMTran row)
		{
			switch (objectName)
			{
				case PMObjectType.PMTran:
					return ConvertFromExtValue(this.Caches[typeof(PMTran)].GetValueExt(row, fieldName));
				case PMObjectType.PMProject:
					PMProject project = PXSelect<PMProject, Where<PMProject.contractID, Equal<Required<PMProject.contractID>>>>.Select(this, row.ProjectID);
					if (project != null)
					{
						if (attribute != null)
						{
							return EvaluateAttribute(attribute, project.NoteID);
						}
						else
						{
							return ConvertFromExtValue(this.Caches[typeof(PMProject)].GetValueExt(project, fieldName));
						}
					}
					break;
				case PMObjectType.PMTask:
					PMTask task = PXSelect<PMTask, Where<PMTask.projectID, Equal<Required<PMTask.projectID>>, And<PMTask.taskID, Equal<Required<PMTask.taskID>>>>>.Select(this, row.ProjectID, row.TaskID);
					if (task != null)
					{
						if (attribute != null)
						{
							return EvaluateAttribute(attribute, task.NoteID);
						}
						else
							return ConvertFromExtValue(this.Caches[typeof(PMTask)].GetValueExt(task, fieldName));
					}
					break;
				case PMObjectType.PMAccountGroup:
					PMAccountGroup accGroup = PXSelect<PMAccountGroup, Where<PMAccountGroup.groupID, Equal<Required<PMAccountGroup.groupID>>>>.Select(this, row.AccountGroupID);
					if (accGroup != null)
					{
						if (attribute != null)
						{
							return EvaluateAttribute(attribute, accGroup.NoteID);
						}
						else
							return ConvertFromExtValue(this.Caches[typeof(PMAccountGroup)].GetValueExt(accGroup, fieldName));
					}
					break;
				case PMObjectType.EPEmployee:
					EPEmployee employee = PXSelect<EPEmployee, Where<EPEmployee.bAccountID, Equal<Required<EPEmployee.bAccountID>>>>.Select(this, row.ResourceID);
					if (employee != null)
					{
						if (attribute != null)
						{
							return EvaluateAttribute(attribute, employee.NoteID);

						}
						else
							return ConvertFromExtValue(this.Caches[typeof(EPEmployee)].GetValueExt(employee, fieldName));
					}
					break;
				case PMObjectType.Customer:
					Customer customer = PXSelect<Customer, Where<Customer.bAccountID, Equal<Required<Customer.bAccountID>>>>.Select(this, row.BAccountID);
					if (customer != null)
					{
						if (attribute != null)
						{
							return EvaluateAttribute(attribute, customer.NoteID);
						}
						else
							return ConvertFromExtValue(this.Caches[typeof(Customer)].GetValueExt(customer, fieldName));
					}
					break;
				case PMObjectType.Vendor:
					VendorR vendor = PXSelect<VendorR, Where<VendorR.bAccountID, Equal<Required<VendorR.bAccountID>>>>.Select(this, row.BAccountID);
					if (vendor != null)
					{
						if (attribute != null)
						{
							return EvaluateAttribute(attribute, vendor.NoteID);
						}
						else
							return ConvertFromExtValue(this.Caches[typeof(VendorR)].GetValueExt(vendor, fieldName));
					}
					break;
				case PMObjectType.InventoryItem:
					InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, row.InventoryID);
					if (item != null)
					{
						if (attribute != null)
						{
							return EvaluateAttribute(attribute, item.NoteID);
						}
						else
							return ConvertFromExtValue(this.Caches[typeof(InventoryItem)].GetValueExt(item, fieldName));
					}
					break;
				default:
					break;
			}

			return null;
		}

		protected virtual object ConvertFromExtValue(object extValue)
		{
			PXFieldState fs = extValue as PXFieldState;
			if (fs != null)
				return fs.Value;
			else
			{
				return extValue;
			}
		}

		protected virtual object EvaluateAttribute(string attribute, Guid? refNoteID)
		{
		    PXResultset<CSAnswers> res = PXSelectJoin<CSAnswers,
		        InnerJoin<CSAttribute, On<CSAttribute.attributeID, Equal<CSAnswers.attributeID>>>,
		        Where<CSAnswers.refNoteID, Equal<Required<CSAnswers.refNoteID>>,
		            And<CSAnswers.attributeID, Equal<Required<CSAnswers.attributeID>>>>>.Select(this, refNoteID, attribute);

			CSAnswers ans = null;
			CSAttribute attr = null;
			if (res.Count > 0)
			{
				ans = (CSAnswers)res[0][0];
				attr = (CSAttribute)res[0][1];
			}

			if (ans == null || ans.AttributeID == null)
			{
				//answer not found. if attribute exists return the default value.
				attr = PXSelect<CSAttribute, Where<CSAttribute.attributeID, Equal<Required<CSAttribute.attributeID>>>>.Select(this, attribute);

				if (attr != null && attr.ControlType == CSAttribute.CheckBox)
                {
					return false;
                }
            }
			
			if (ans != null)
			{
				if (ans.Value != null)
					return ans.Value;
				else
				{
					if (attr != null && attr.ControlType == CSAttribute.CheckBox)
					{
						return false;
					}
				}
			}
				
			return string.Empty;
		}

		protected virtual IList<PMTran> Transform(PMAllocationStep step, PMTask task, PMTran original, bool calculateFormulas, bool isWipStep)
		{
			List<PMTran> list = new List<PMTran>();

			int? debitAccountGroup = null;
			int? creditAccountGroup = null;
			int? debitProjectID = null;
			int? creditProjectID = null;
			int? debitTaskID = null;
			int? creditTaskID = null;
			decimal? qty = null;
			decimal? billableQty = null;
			decimal? amt = null;
			string desc = null;

			#region Extract parameters

			if (step.AccountGroupOrigin == PMOrigin.Change)
			{
				debitAccountGroup = step.AccountGroupID;
			}
			else if (step.AccountGroupOrigin == PMOrigin.FromAccount)
			{
				if (step.AccountOrigin == PMOrigin.Source)
				{
					Account account = PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this, original.AccountID);
					if (account != null)
						debitAccountGroup = account.AccountGroupID;
				}
				else if (step.AccountOrigin == PMOrigin.OtherSource)
				{
					Account account = PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this, original.OffsetAccountID);
					if (account != null)
						debitAccountGroup = account.AccountGroupID;
				}
				else
				{
					Account account = PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this, step.AccountID);
					if (account != null)
						debitAccountGroup = account.AccountGroupID;
				}
			}
			else
			{
				if (step.AccountID != null)
				{
					PXTrace.WriteWarning("Step {0} is Debit Account Group configured as {1} but an Account is supplied.", step.StepID, step.AccountGroupOrigin);
				}

				debitAccountGroup = original.AccountGroupID;
			}

			if (step.OffsetAccountGroupOrigin == PMOrigin.Change)
			{
				creditAccountGroup = step.OffsetAccountGroupID;
			}
			else if (step.OffsetAccountGroupOrigin == PMOrigin.FromAccount)
			{
				if (step.OffsetAccountOrigin == PMOrigin.Source)
				{
					Account account = PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this, original.OffsetAccountID);
					if (account != null)
						creditAccountGroup = account.AccountGroupID;
				}
				else if (step.OffsetAccountOrigin == PMOrigin.OtherSource)
				{
					Account account = PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this, original.AccountID);
					if (account != null)
						creditAccountGroup = account.AccountGroupID;
				}
				else
				{
					Account account = PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this, step.OffsetAccountID);
					if (account != null)
						creditAccountGroup = account.AccountGroupID;
				}
			}
			else if (step.OffsetAccountGroupOrigin == PMOrigin.Source)
			{
				if (step.OffsetAccountID != null)
				{
					PXTrace.WriteWarning("Step {0} is Credit Account Group configured as {1} but an Account is supplied.", step.StepID, step.OffsetAccountGroupOrigin);
				}

				creditAccountGroup = original.AccountGroupID;
			}


			if (step.ProjectOrigin == PMOrigin.Change)
			{
				debitProjectID = step.ProjectID;
			}
			else
			{
				debitProjectID = original.ProjectID;
			}

			if (step.OffsetProjectOrigin == PMOrigin.Change)
			{
				creditProjectID = step.OffsetProjectID;
			}
			else
			{
				creditProjectID = original.ProjectID;
			}


			if (step.TaskOrigin == PMOrigin.Change)
			{
				debitTaskID = step.TaskID;
			}
			else
			{
				PMTask debitTask = PXSelect<PMTask, Where<PMTask.projectID, Equal<Required<PMTask.projectID>>, And<PMTask.taskCD, Equal<Required<PMTask.taskCD>>>>>.Select(this, debitProjectID, task.TaskCD);

				if (debitTask == null)
				{
					PMProject debitProject = PXSelect<PMProject, Where<PMProject.contractID, Equal<Required<PMProject.contractID>>>>.Select(this, debitProjectID);

					if (debitProject == null)
					{
						throw new PXException(Messages.DebitProjectNotFound, step.StepID);
					}

					throw new PXException(Messages.DebitTaskNotFound, step.StepID, task.TaskCD, debitProject.ContractCD);
				}

				debitTaskID = debitTask.TaskID;
			}

			if (step.OffsetTaskOrigin == PMOrigin.Change)
			{
				creditTaskID = step.OffsetTaskID;
			}
			else
			{
				PMTask creditTask = PXSelect<PMTask, Where<PMTask.projectID, Equal<Required<PMTask.projectID>>, And<PMTask.taskCD, Equal<Required<PMTask.taskCD>>>>>.Select(this, creditProjectID, task.TaskCD);

				if (creditTask == null)
				{
					PMProject creditProject = PXSelect<PMProject, Where<PMProject.contractID, Equal<Required<PMProject.contractID>>>>.Select(this, creditProjectID);

					if (creditProject == null)
					{
						throw new PXException(Messages.CreditProjectNotFound, step.StepID);
					}

					throw new PXException(Messages.CreditTaskNotFound, step.StepID, task.TaskCD, creditProject.ContractCD);
				}

				creditTaskID = creditTask.TaskID;
			}

			#endregion

			if (debitAccountGroup == null)
			{
				PXTrace.WriteError(Messages.InvalidAllocationRule, step.StepID, task.TaskCD);
				throw new PXException(Messages.InvalidAllocationRule, step.StepID, task.TaskCD);
			}
			if (calculateFormulas)
			{
				PMDataNavigator navigator = new PMDataNavigator(this, new List<PMTran>(new PMTran[1] { original }));
				CalculateFormulas(navigator, step, original, out qty, out billableQty, out amt, out desc);
			}

			bool doubleEntry = false;

			if (step.UpdateGL != true)
			{
				doubleEntry = true;
			}
			else
			{
				creditProjectID = debitProjectID;
				creditTaskID = debitTaskID;
			}

			if (doubleEntry)
			{
				#region Debit Tran
				PMTran debitTran = CreateFromTemplate(step, original, isWipStep);
				debitTran.Allocated = step.MarkAsNotAllocated != true;
				debitTran.AccountGroupID = debitAccountGroup;
				if (step.OffsetAccountGroupOrigin != PMOrigin.None)
				debitTran.OffsetAccountGroupID = creditAccountGroup;
				debitTran.ProjectID = debitProjectID;
				debitTran.TaskID = debitTaskID;
				debitTran.CreatedByCurrentAllocation = true;
				
				if (step.UpdateGL == true)
				{
					switch (step.AccountOrigin)
					{
						case PMOrigin.Change:
							debitTran.AccountID = step.AccountID;
							break;
						case PMOrigin.OtherSource:
							debitTran.AccountID = original.OffsetAccountID;
							break;
						default://Source
							debitTran.AccountID = original.AccountID;
							break;
					}

					//Make Subaccount:
					if (original.SubID != null)
					{
						PMProject project = PXSelect<PMProject, Where<PMProject.contractID, Equal<Required<PMProject.contractID>>>>.Select(this, task.ProjectID);

						if (project == null)
						{
							throw new PXException(Messages.ProjectInTaskNotFound, task.TaskCD, task.ProjectID);
						}

						object value = PMSubAccountMaskAttribute.MakeSub<PMAllocationStep.subMask>(this, step.SubMask,
						new object[] { original.SubID, step.SubID, project.DefaultSubID, task.DefaultSubID },
						new Type[] { typeof(PMTran.subID), typeof(PMAllocationStep.subID), typeof(PMProject.defaultSubID), typeof(PMTask.defaultSubID) });
						Transactions.Cache.RaiseFieldUpdating<PMTran.subID>(debitTran, ref value);
						debitTran.SubID = (int?)value;
					}
					else
					{
						debitTran.SubID = step.SubID;
					}
				}

				if (calculateFormulas)
				{
					debitTran.BillableQty = billableQty;
					debitTran.Qty = qty;
					debitTran.Amount = CM.PXCurrencyAttribute.BaseRound(this, amt ?? 0);
					if (billableQty != null && billableQty != 0)
					{
						decimal a = PX.Objects.CM.PXCurrencyAttribute.PXCurrencyHelper.BaseRound(this, debitTran.Amount.GetValueOrDefault());
						decimal q = PXDBQuantityAttribute.Round(billableQty.Value);
						decimal unitRate = PXDBPriceCostAttribute.Round(a / q);
						debitTran.Billable = true;
						debitTran.UseBillableQty = true;
						debitTran.UnitRate = unitRate;
					}
					debitTran.Description = desc;
				}
				list.Add(debitTran);
				#endregion

				#region Credit Tran

				if (creditAccountGroup != null)
				{
                    PMTran creditTran = CreateFromTemplate(step, original, isWipStep);
					creditTran.Allocated = step.MarkAsNotAllocated != true;
					creditTran.AccountGroupID = creditAccountGroup;
					creditTran.ProjectID = creditProjectID;
					creditTran.TaskID = creditTaskID;
					creditTran.IsInverted = true;
					creditTran.OrigAccountGroupID = null; //Skip recording this tran in PMTaskAllocTotal otherwise amount will always be 0.
					creditTran.IsCreditPair = true;//Caps should not be applied to this tran.
					creditTran.CreatedByCurrentAllocation = true;
					
					if (step.UpdateGL == true)
					{
						switch (step.OffsetAccountOrigin)
						{
							case PMOrigin.Change:
								creditTran.AccountID = step.OffsetAccountID;
								break;
							case PMOrigin.OtherSource:
								creditTran.AccountID = original.AccountID;
								break;
							default://Source
								creditTran.AccountID = original.OffsetAccountID;
								break;
						}

						//Make Subaccount:
						if (original.SubID != null)
						{
							PMProject project = PXSelect<PMProject, Where<PMProject.contractID, Equal<Required<PMProject.contractID>>>>.Select(this, task.ProjectID);

							if (project == null)
							{
								throw new PXException(Messages.ProjectInTaskNotFound, task.TaskCD, task.ProjectID);
							}

							object value = PMSubAccountMaskAttribute.MakeSub<PMAllocationStep.offsetSubMask>(this, step.OffsetSubMask,
							new object[] { original.SubID, step.SubID, project.DefaultSubID, task.DefaultSubID },
							new Type[] { typeof(PMTran.subID), typeof(PMAllocationStep.subID), typeof(PMProject.defaultSubID), typeof(PMTask.defaultSubID) });
							Transactions.Cache.RaiseFieldUpdating<PMTran.offsetSubID>(creditTran, ref value);
							creditTran.SubID = (int?)value;
						}
						else
						{
							creditTran.SubID = step.SubID;
						}
					}

					if (calculateFormulas)
					{
						creditTran.BillableQty = -billableQty;
						creditTran.Qty = -qty;
						creditTran.Amount = -CM.PXCurrencyAttribute.BaseRound(this, amt ?? 0);
						if (billableQty != null && billableQty != 0)
						{
							decimal a = PX.Objects.CM.PXCurrencyAttribute.PXCurrencyHelper.BaseRound(this, creditTran.Amount.GetValueOrDefault());
							decimal q = PXDBQuantityAttribute.Round(billableQty.Value);
							decimal unitRate = PXDBPriceCostAttribute.Round(a / q);
							creditTran.Billable = true;
							creditTran.UseBillableQty = true;
							creditTran.UnitRate = unitRate;
						}

						creditTran.Description = desc;
					}
					list.Add(creditTran);
				}
				#endregion

			}
			else
			{
				#region Debit/Credit Tran
                PMTran tran = CreateFromTemplate(step, original, isWipStep);
				tran.Allocated = step.MarkAsNotAllocated != true;
				tran.AccountGroupID = debitAccountGroup;
				tran.ProjectID = debitProjectID;
				tran.TaskID = debitTaskID;
				tran.CreatedByCurrentAllocation = true;
				
				#region Set Debit/Credit Accounts and Subs
				if (step.UpdateGL == true)
				{
					switch (step.AccountOrigin)
					{
						case PMOrigin.Change:
							tran.AccountID = step.AccountID;
							tran.SubID = original.SubID ?? step.SubID;
							break;
						case PMOrigin.OtherSource:
							tran.AccountID = original.OffsetAccountID;
							tran.SubID = original.OffsetSubID;
							break;
						default://Source
							tran.AccountID = original.AccountID;
							tran.SubID = original.SubID;
							break;
					}

                    
					//Make Subaccount:
					PMProject project = PXSelect<PMProject, Where<PMProject.contractID, Equal<Required<PMProject.contractID>>>>.Select(this, task.ProjectID);

					if (project == null)
					{
						throw new PXException(Messages.ProjectInTaskNotFound, task.TaskCD, task.ProjectID);
					}

					if (step.AccountOrigin != PMOrigin.None)
					{
						if (step.SubMask == null)
							throw new PXException(Messages.StepSubMaskSpecified, step.AllocationID, step.StepID);

                        if (step.SubMask.Contains(PMAcctSubDefault.MaskSource) && tran.SubID == null)
                        {
							throw new PXException(Messages.SourceSubNotSpecified, step.AllocationID, step.StepID);
                        }

                        if (step.SubMask.Contains(PMAcctSubDefault.AllocationStep) && step.SubID == null)
                        {
							throw new PXException(Messages.StepSubNotSpecified, step.AllocationID, step.StepID);
                        }

					object value = PMSubAccountMaskAttribute.MakeSub<PMAllocationStep.subMask>(this, step.SubMask,
					new object[] { tran.SubID, step.SubID, project.DefaultSubID, task.DefaultSubID },
					new Type[] { typeof(PMTran.subID), typeof(PMAllocationStep.subID), typeof(PMProject.defaultSubID), typeof(PMTask.defaultSubID) });
					Transactions.Cache.RaiseFieldUpdating<PMTran.subID>(tran, ref value);
					tran.SubID = (int?)value;
					}

					if (step.OffsetAccountOrigin == PMOrigin.Change)
					{
						tran.OffsetAccountID = step.OffsetAccountID;
						tran.OffsetSubID = original.OffsetSubID ?? (step.OffsetSubID ?? original.SubID);
					}
					else if (step.OffsetAccountOrigin == PMOrigin.OtherSource)
					{
                        if (original.AccountID == null)
                        {
							throw new PXException(Messages.OtherSourceIsEmpty, step.AllocationID, step.StepID, original.Description);
                        }

						tran.OffsetAccountID = original.AccountID;
						tran.OffsetSubID = original.SubID;
					}
					else
					{
						tran.OffsetAccountID = original.OffsetAccountID ?? original.AccountID;
						tran.OffsetSubID = original.OffsetSubID ?? original.SubID;

					}


					if (tran.OffsetAccountID != null)
					{
						object offsetValue = PMSubAccountMaskAttribute.MakeSub<PMAllocationStep.offsetSubMask>(this, step.OffsetSubMask,
						new object[] { tran.OffsetSubID, step.OffsetSubID, project.DefaultSubID, task.DefaultSubID },
						new Type[] { typeof(PMTran.offsetSubID), typeof(PMAllocationStep.offsetSubID), typeof(PMProject.defaultSubID), typeof(PMTask.defaultSubID) });
						Transactions.Cache.RaiseFieldUpdating<PMTran.offsetSubID>(tran, ref offsetValue);
						tran.OffsetSubID = (int?)offsetValue;
					}
				}
				#endregion

				if (calculateFormulas)
				{
					tran.BillableQty = billableQty;
					tran.Qty = qty;
					tran.Amount = CM.PXCurrencyAttribute.BaseRound(this, amt ?? 0);
					if (billableQty != null && billableQty != 0)
					{
						decimal a = PX.Objects.CM.PXCurrencyAttribute.PXCurrencyHelper.BaseRound(this, tran.Amount.GetValueOrDefault());
						decimal q = PXDBQuantityAttribute.Round(billableQty.Value);
						decimal unitRate = PXDBPriceCostAttribute.Round(a / q);
						tran.UnitRate = unitRate;
						tran.Billable = original.Billable == true;
						tran.UseBillableQty = original.Billable == true;
					}
					tran.Description = desc;
				}
				list.Add(tran);
				#endregion
			}

			return list;
		}

		protected virtual PMTran CreateFromTemplate(PMAllocationStep step, PMTran original, bool isWipStep)
		{
			PMTran tran = new PMTran();
			tran.BranchID = step.TargetBranchID ?? original.BranchID;
			tran.UOM = original.UOM;
			tran.BAccountID = original.BAccountID;
			tran.Billable = original.Billable;
			tran.BillableQty = original.BillableQty;
			tran.InventoryID = original.InventoryID;
			tran.LocationID = original.LocationID;
			tran.ResourceID = original.ResourceID;
			tran.AllocationID = step.AllocationID;
            if (!isWipStep)
            {
				tran.OrigAccountGroupID = original.AccountGroupID;
				tran.OrigProjectID = original.ProjectID;
				tran.OrigTaskID = original.TaskID;
            }
			tran.IsNonGL = step.UpdateGL == false;
			if (step.DateSource == PMDateSource.Transaction)
				tran.Date = original.Date;
			else if (OverrideAllocationDate != null)
			{
				tran.Date = OverrideAllocationDate;
			}
			tran.FinPeriodID = original.FinPeriodID;
			tran.StartDate = original.StartDate;
			tran.EndDate = original.EndDate;
			tran.OrigRefID = original.OrigRefID;
			tran.UseBillableQty = true;
			tran.Reverse = step.Reverse;

			return tran;
		}

		protected virtual void CalculateFormulas(PMDataNavigator navigator, PMAllocationStep step, PMTran tran, out decimal? qty, out decimal? billableQty, out decimal? amt, out string description)
		{
			qty = null;
			billableQty = null;
			amt = null;
			description = null;

			if (!string.IsNullOrEmpty(step.QtyFormula))
			{
				try
				{
					ExpressionNode qtyNode = PMExpressionParser.Parse(this, step, step.QtyFormula);
					qtyNode.Bind(navigator);
					object val = qtyNode.Eval(tran);
					if (val != null)
					{
						qty = Convert.ToDecimal(val);
					}
				}
				catch (Exception ex)
				{
					throw new PXException(Messages.FailedToCalcQtyFormula, step.AllocationID, step.StepID, step.QtyFormula, ex.Message);
				}
			}
			else
			{
				qty = tran.Qty;
			}

			if (!string.IsNullOrEmpty(step.BillableQtyFormula))
			{
				try
				{
					ExpressionNode billableQtyNode = PMExpressionParser.Parse(this, step, step.BillableQtyFormula);
					billableQtyNode.Bind(navigator);
					object val = billableQtyNode.Eval(tran);

					if (val != null)
					{
						billableQty = Convert.ToDecimal(val);
					}
				}
				catch (Exception ex)
				{
					throw new PXException(Messages.FailedToCalcBillQtyFormula, step.AllocationID, step.StepID, step.BillableQtyFormula, ex.Message);
				}
			}
			else
			{
				billableQty = tran.BillableQty;
			}

			if (!string.IsNullOrEmpty(step.AmountFormula))
			{
				try
				{
					ExpressionNode amtNode = PMExpressionParser.Parse(this, step, step.AmountFormula);
					amtNode.Bind(navigator);
					object val = amtNode.Eval(tran);
					if (val != null)
					{
						amt = Convert.ToDecimal(val);
					}
				}
				catch (Exception ex)
				{
					throw new PXException(Messages.FailedToCalcAmtFormula, step.AllocationID, step.StepID, step.AmountFormula, ex.Message);
				}
			}
			else
			{
				amt = tran.Amount;
			}

			if (!string.IsNullOrEmpty(step.DescriptionFormula))
			{
				try
				{
					ExpressionNode descNode = PMExpressionParser.Parse(this, step, step.DescriptionFormula);
					descNode.Bind(navigator);
					object val = descNode.Eval(tran);
					if (val != null)
						description = val.ToString();
				}
				catch (Exception ex)
				{
					throw new PXException(Messages.FailedToCalcDescFormula, step.AllocationID, step.StepID, step.DescriptionFormula, ex.Message);
				}
			}
			else
			{
				description = tran.Description;
			}

		}
		
		/// <summary>
		/// Selects Source Transactions for the given step. 
		/// Method relies on pre-selected data stored in allocationInfo, accountGroups and transactions collections.
		/// Method do not query database.
		/// </summary>
		protected virtual List<PMTran> Select(PMAllocationStep step, int? projectID, int? taskID)
		{
			List<PMTran> list;

			if (step.Post == true)
			{
				if (stepResults.ContainsKey(step.StepID.Value))
				{
					list = new List<PMTran>(stepResults[step.StepID.Value].Count);
					foreach (PMTranWithTrace twt in stepResults[step.StepID.Value])
					{
						list.Add(twt.Tran);
					}

					return list;
				}
			}

			list = new List<PMTran>();
			if (step.SelectOption == PMSelectOption.Step)
			{
				foreach (PMAllocationStep innerStep in GetSteps(step.AllocationID, step.RangeStart, step.RangeEnd))
				{
					list.AddRange(Select(innerStep, projectID, taskID));
				}
			}
			else
			{
				foreach (PMTran tran in GetTranByStep(step, projectID, taskID))
				{
					list.Add(tran);
				}
			}

			return list;
		}

		/// <summary>
		/// Selects Source Transactions for the given inner step (step with From/To AccountGroups). 
		/// Method relies on pre-selected data stored in allocationInfo, accountGroups and transactions collections.
		/// Method do not query database.
		/// </summary>
		protected virtual IEnumerable<PMTran> GetTranByStep(PMAllocationStep step, int? projectID, int? taskID)
		{
			foreach (int groupID in GetAccountGroupsRange(step))
			{
				if (SourceTransactions != null)
				{
					//use supplied source.
					//Note: do not skip allocated transactions cause the allocated flag might be just set in the previous step within the current allocation process.
					foreach (PMTran tran in SourceTransactions)
					{
						if (tran.Released == true &&
							tran.AccountGroupID == groupID &&
							tran.ProjectID == projectID &&
							tran.TaskID == taskID)
						{
							yield return tran;
						}
					}
				}
				else
				{
					//get from memory (pre-selected/cached)
					Dictionary<int, List<PXResult<PMTran>>> transactionsByTask;
					if (transactions.TryGetValue(taskID.Value, out transactionsByTask))
					{
						List<PXResult<PMTran>> source;
						if (transactionsByTask.TryGetValue(groupID, out source))
						{
							foreach (PMTran tran in source)
							{
								if (step.SourceBranchID != null && step.SourceBranchID != tran.BranchID)
									continue;

								yield return tran;
							}
						}
					}
				}
			}
		}

		protected virtual PXResultset<PMTran> GetTranFromDatabase(int? projectID, int? taskID, int groupID)
		{
		    PXResultset<PMTran> resultset = null;

				PXSelectBase<PMTran> selectTrans = new PXSelectReadonly<PMTran,
					Where<PMTran.allocated, Equal<False>,
					And<PMTran.released, Equal<True>,
					And<PMTran.accountGroupID, Equal<Required<PMTran.accountGroupID>>,
					And<PMTran.projectID, Equal<Required<PMTran.projectID>>,
					And<PMTran.taskID, Equal<Required<PMTran.taskID>>>>>>>>(this);

			if (FilterStartDate != null && FilterEndDate != null)
                {
                    if (FilterStartDate == FilterEndDate)
                    {
                        selectTrans.WhereAnd<Where<PMTran.date, Equal<Required<PMTran.date>>>>();
					resultset = selectTrans.Select(groupID, projectID, taskID, FilterStartDate);
                    }
                    else
                    {
                        selectTrans.WhereAnd<Where<PMTran.date, Between<Required<PMTran.date>, Required<PMTran.date>>>>();
					resultset = selectTrans.Select(groupID, projectID, taskID, FilterStartDate, FilterEndDate);
                    }
                }
                else if (FilterStartDate != null)
                {
                    selectTrans.WhereAnd<Where<PMTran.date, GreaterEqual<Required<PMTran.date>>>>();
				resultset = selectTrans.Select(groupID, projectID, taskID, FilterStartDate);
                }
                else if (FilterEndDate != null)
                {
                    selectTrans.WhereAnd<Where<PMTran.date, LessEqual<Required<PMTran.date>>>>();
				resultset = selectTrans.Select(groupID, projectID, taskID, FilterEndDate);
                }
                else
                {
				resultset = selectTrans.Select(groupID, projectID, taskID);
                }

			return resultset;
            }

		protected virtual PXResultset<PMTran> GetTranFromDatabase(int? projectID, int groupID)
			{
			PXResultset<PMTran> resultset = null;

				PXSelectBase<PMTran> selectTrans = new PXSelectReadonly<PMTran,
					Where<PMTran.allocated, Equal<False>,
					And<PMTran.released, Equal<True>,
					And<PMTran.accountGroupID, Equal<Required<PMTran.accountGroupID>>,
					And<PMTran.projectID, Equal<Required<PMTran.projectID>>>>>>>(this);
				
                if (FilterStartDate != null && FilterEndDate != null)
                {
                    if (FilterStartDate == FilterEndDate)
                    {
                        selectTrans.WhereAnd<Where<PMTran.date, Equal<Required<PMTran.date>>>>();
					resultset = selectTrans.Select(groupID, projectID, FilterStartDate);
                    }
                    else
                    {
                        selectTrans.WhereAnd<Where<PMTran.date, Between<Required<PMTran.date>, Required<PMTran.date>>>>();
					resultset = selectTrans.Select(groupID, projectID, FilterStartDate, FilterEndDate);
                    }
                }
                else if (FilterStartDate != null)
                {
                    selectTrans.WhereAnd<Where<PMTran.date, GreaterEqual<Required<PMTran.date>>>>();
				resultset = selectTrans.Select(groupID, projectID, FilterStartDate);
                }
                else if (FilterEndDate != null)
                {
                    selectTrans.WhereAnd<Where<PMTran.date, LessEqual<Required<PMTran.date>>>>();
				resultset = selectTrans.Select(groupID, projectID, FilterEndDate);
                }
                else
                {
				resultset = selectTrans.Select(groupID, projectID);
			}

			return resultset;
		}

		protected virtual PMAllocationSourceTran CreateAllocationTran(string allocationID, int? stepID, PMTran tran)
		{
			PMAllocationSourceTran at = new PMAllocationSourceTran();
			at.AllocationID = allocationID;
			at.StepID = stepID;
			at.TranID = tran.TranID;
			at.Qty = tran.Qty;
			at.Rate = tran.Rate;
			at.Amount = tran.Amount;

			return at;
		}

		protected virtual decimal? GetRate(PMAllocationStep step, PMTran tran, string rateTableID)
		{
			if (string.IsNullOrEmpty(step.RateTypeID))
			{
				switch (step.NoRateOption)
				{
					case PMNoRateOption.SetZero:
						return 0;
					case PMNoRateOption.RaiseError:
						throw new PXException(Messages.RateTypeNotDefinedForStep, step.StepID);
					case PMNoRateOption.DontAllocate:
						return null;
					default:
						return 1;
				}
			}

			decimal? rate = null;
			string trace = null;

			if (rateEngine != null)
			{
				//use pre-selected rates
				rate = rateEngine.GetRate(rateTableID, step.RateTypeID, tran);
				trace = rateEngine.GetTrace(tran);
			}
			else
			{
			RateEngine engine = CreateRateEngine(step.RateTypeID, tran);

				rate = engine.GetRate(rateTableID);
				trace = engine.GetTrace();
			}
			
			if (rate != null)
				return rate;
			else
			{
				switch (step.NoRateOption)
				{
					case PMNoRateOption.SetZero:
						return 0;
					case PMNoRateOption.RaiseError:
						PXTrace.WriteInformation(trace);
						PXTrace.WriteError(Messages.RateNotDefinedForStep, step.StepID);
						throw new PXException(Messages.RateNotDefinedForStep, step.StepID);
					case PMNoRateOption.DontAllocate:
						PXTrace.WriteInformation(trace);
						return null;
					default:
						return 1;
				}
			}

		}

		public virtual RateEngine CreateRateEngine(string rateTypeID, PMTran tran)
		{
			return new RateEngine(this, rateTypeID, tran);
		}

		protected virtual decimal ConvertQtyToBase(int? inventoryID, string UOM, decimal qty)
		{
			try
			{
				return INUnitAttribute.ConvertToBase(Transactions.Cache, inventoryID, UOM, qty, INPrecision.QUANTITY);
			}
			catch (PXException ex)
			{
				InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>
						.Select(this, inventoryID);

				if (item != null)
				{
					PXTrace.WriteError("Failed to convert the Inventory Item '{0}' FROM '{1}' TO '{2}'. Error: {3}", item.InventoryCD, UOM, item.BaseUnit, ex.Message);
				}

				throw;
			}
		}

		#region Local Types

		protected class Group
		{
			public List<PMTran> List { get; private set; }

			private bool statReady = false;
			private bool hasMixedInventory = false;
			private bool hasMixedUOM = false;
			private bool hasMixedBAccount = false;
			private bool hasMixedBAccountLoc = false;
			private bool hasMixedDescription = false;

			public Group()
			{
				List = new List<PMTran>();
			}

			public bool HasMixedInventory
			{
				get
				{
					if (!statReady)
						InitStatistics();

					return hasMixedInventory;
				}
			}

			public bool HasMixedUOM
			{
				get
				{
					if (!statReady)
						InitStatistics();

					return hasMixedUOM;
				}
			}

			public bool HasMixedBAccount
			{
				get
				{
					if (!statReady)
						InitStatistics();

					return hasMixedBAccount;
				}
			}

			public bool HasMixedBAccountLoc
			{
				get
				{
					if (!statReady)
						InitStatistics();

					return hasMixedBAccountLoc;
				}
			}

			public bool HasMixedDescription
			{
				get
				{
					if (!statReady)
						InitStatistics();

					return hasMixedDescription;
				}
			}

			private void InitStatistics()
			{
				if (List.Count > 0)
				{
					int lastInvetoryID = List[0].InventoryID.Value;
					string lastUOM = List[0].UOM;
					int? lastBAccountID = List[0].BAccountID;
					int? lastLocationID = List[0].LocationID;
					string lastDescription = List[0].Description;

					for (int i = 1; i < List.Count; i++)
					{
						if (lastInvetoryID != List[i].InventoryID)
						{
							hasMixedInventory = true;
						}
						
						if (string.IsNullOrEmpty(lastUOM))
						{
							lastUOM = List[i].UOM;
						}
						else if (!string.IsNullOrEmpty(List[i].UOM))
						{
							if (lastUOM != List[i].UOM)
							{
								hasMixedUOM = true;
							}
						}

						if (lastBAccountID != List[i].BAccountID)
						{
							hasMixedBAccount = true;
						}

						if (lastLocationID != List[i].LocationID)
						{
							hasMixedBAccountLoc = true;
						}

						if (lastDescription != List[i].Description)
						{
							hasMixedDescription = true;
						}
					}

					statReady = true;
				}
			}

		}

		protected class AllocData
		{
			public int AccountGroupID { get; private set; }
			public int InventoryID { get; private set; }
			public decimal Amount { get; set; }
			public decimal Quantity { get; set; }
			public string UOM { get; set; }

			public AllocData(int accountGroupID, int inventoryID)
			{
				this.AccountGroupID = accountGroupID;
				this.InventoryID = inventoryID;
			}
		}

		protected class PMBudgetStat
		{
			public int AccountGroupID { get; private set; }
			public int InventoryID { get; private set; }
			public decimal? Amount { get; set; }
			public decimal? Quantity { get; set; }

			public PMBudgetStat(int accountGroupID, int inventoryID)
			{
				this.AccountGroupID = accountGroupID;
				this.InventoryID = inventoryID;
			}

			public static string GetKey(int accountGroupID, int inventoryID)
			{
				return string.Format("{0}.{1}", accountGroupID, inventoryID);
			}
		}

		public class CapService
		{
			private Dictionary<int, PMBudgetStat> capsByItem = new Dictionary<int, PMBudgetStat>();
			private bool limitAmt = false;
			private bool limitQty = false;
            private bool isWipStep = false;
			private int? debitAccountGroupID = null;
			public int? OverflowAccountGroup { get; private set; }


			public CapService(PXGraph graph, PMAllocationStep step, PMTask task)
			{
				if (step.AccountGroupOrigin == PMOrigin.Change)
				{
					debitAccountGroupID = step.AccountGroupID;
				}
				else if (step.AccountGroupOrigin == PMOrigin.FromAccount)
				{
					Account account = PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(graph, step.AccountID);
					if (account != null)
						debitAccountGroupID = account.AccountGroupID;
				}


                PMBillingRule billingRuleWithWip = PXSelect<PMBillingRule, Where<PMBillingRule.billingID, Equal<Required<PMBillingRule.billingID>>, And<PMBillingRule.wipAccountGroupID, Equal<Required<PMBillingRule.wipAccountGroupID>>>>>.Select(graph, task.BillingID, debitAccountGroupID);
                if (billingRuleWithWip != null)
                    isWipStep = true;
				

				PMBillingRule billingRule = PXSelect<PMBillingRule, Where<PMBillingRule.billingID, Equal<Required<PMBillingRule.billingID>>, And<PMBillingRule.accountGroupID, Equal<Required<PMBillingRule.accountGroupID>>>>>.Select(graph, task.BillingID, debitAccountGroupID);
				if (billingRule != null)
				{
					this.OverflowAccountGroup = billingRule.OverflowAccountGroupID;

					//caps:
					if (billingRule.LimitAmt == true || billingRule.LimitQty == true)
					{
						PXSelectBase<PMProjectStatus> selectBudget = new PXSelectGroupBy<PMProjectStatus,
						Where<PMProjectStatus.projectID, Equal<Required<PMProjectStatus.projectID>>,
						And<PMProjectStatus.projectTaskID, Equal<Required<PMProjectStatus.projectTaskID>>,
						And<PMProjectStatus.accountGroupID, Equal<Required<PMProjectStatus.accountGroupID>>>>>,
						Aggregate<
						GroupBy<PMProjectStatus.accountGroupID,
						GroupBy<PMProjectStatus.projectID,
						GroupBy<PMProjectStatus.projectTaskID,
						GroupBy<PMProjectStatus.inventoryID,
						Sum<PMProjectStatus.amount,
						Sum<PMProjectStatus.qty,
						Sum<PMProjectStatus.revisedAmount,
						Sum<PMProjectStatus.revisedQty,
						Sum<PMProjectStatus.actualAmount,
						Sum<PMProjectStatus.actualQty>>>>>>>>>>>>(graph);

						foreach (PMProjectStatus ps in selectBudget.Select(task.ProjectID, task.TaskID, billingRule.CapsAccountGroupID))
						{
                            PMBudgetStat bs = new PMBudgetStat(debitAccountGroupID.Value, ps.InventoryID.Value);
							if (ps.RevisedAmount > 0)
								bs.Amount = ps.RevisedAmount;

							decimal revisedQtyInBase = ps.RevisedQty ?? 0;

							if (ps.InventoryID.Value != PMInventorySelectorAttribute.EmptyInventoryID)
								revisedQtyInBase = INUnitAttribute.ConvertToBase(graph.Caches[typeof(PMTran)], ps.InventoryID, ps.UOM, ps.RevisedQty ?? 0, INPrecision.QUANTITY);

							if (revisedQtyInBase > 0)
								bs.Quantity = revisedQtyInBase;

							capsByItem.Add(ps.InventoryID.Value, bs);
						}

						//Limit is ON only if there exists a Value.
						if (capsByItem.Count > 0)
						{
							this.limitAmt = billingRule.LimitAmt == true;
							this.limitQty = billingRule.LimitQty == true;
						}
					}
				}
			}

			public bool IsWipStep
            {
                get { return isWipStep;  }
			}

			public bool LimitAmt
			{
				get { return limitAmt; }
			}

			public bool LimitQty
			{
				get { return limitQty; }
			}

			
			public bool IsCapsDefined(int inventoryID, int? targetAccountGroup)
			{
				if (debitAccountGroupID != targetAccountGroup)
					return false;

				if (inventoryID == PMInventorySelectorAttribute.EmptyInventoryID)
					return false;

				return capsByItem.ContainsKey(inventoryID);
			}

			public decimal CommomnCapAmt
			{
				get
				{
					PMBudgetStat result;
					if (capsByItem.TryGetValue(PMInventorySelectorAttribute.EmptyInventoryID, out result))
						return result.Amount ?? 0;
					else
						return 0;
				}
			}

			public decimal GetAmtCap(int inventoryID, int? targetAccountGroup)
			{
				if (debitAccountGroupID != targetAccountGroup)
					return 0;


				if (!limitAmt)
				{
					return 0;
				}

				if (capsByItem.ContainsKey(inventoryID))
				{
					return capsByItem[inventoryID].Amount ?? 0;
				}
				else
				{
					return 0;
				}
			}


			/// <summary>
			/// Returns Maximum Qty for the given Inventory in the Base Units.
			/// </summary>
			public decimal GetQtyCap(int inventoryID, int? targetAccountGroup)
			{
				if (debitAccountGroupID != targetAccountGroup)
					return 0;


				if (!LimitQty)
				{
					return 0;
				}

				if (capsByItem.ContainsKey(inventoryID))
				{
					return capsByItem[inventoryID].Quantity ?? 0;
				}
				else
				{
					return 0;
				}
			}
		}

        [Serializable]
		public class AllocatedService
		{
			private Dictionary<string, PMBudgetStat> list = new Dictionary<string, PMBudgetStat>();

			public AllocatedService(PXGraph graph, PMTask task)
			{
				PXSelectBase<PMTaskAllocTotalEx> select = new PXSelectReadonly<PMTaskAllocTotalEx, Where<PMTaskAllocTotalEx.projectID, Equal<Required<PMTaskAllocTotalEx.projectID>>, And<PMTaskAllocTotalEx.taskID, Equal<Required<PMTaskAllocTotalEx.taskID>>>>>(graph);
				foreach (PMTaskAllocTotalEx tat in select.Select(task.ProjectID, task.TaskID))
				{
					PMBudgetStat bs = new PMBudgetStat(tat.AccountGroupID.Value, tat.InventoryID.Value);
					bs.Amount = tat.Amount;
					bs.Quantity = tat.Quantity;

					list.Add(GetKey(tat.AccountGroupID.Value, tat.InventoryID.Value), bs);
				}

				foreach (PMTaskAllocTotalAccum acum in graph.Caches[typeof(PMTaskAllocTotalAccum)].Inserted)
				{
					string key = GetKey(acum.AccountGroupID.Value, acum.InventoryID.Value);

					if (list.ContainsKey(key) && acum.TaskID == task.TaskID)
					{
						list[key].Amount += acum.Amount;
						list[key].Quantity += acum.Quantity;
					}
				}
			}

			public decimal GetAllocatedAmt(int accountGroupID, int inventoryID)
			{
				PMBudgetStat result;
				if (list.TryGetValue(GetKey(accountGroupID, inventoryID), out result))
					return result.Amount ?? 0;
				else
					return 0;
			}

			public decimal GetAllocatedAmt(int accountGroupID)
			{
				decimal amt = 0;
				foreach (PMBudgetStat record in list.Values)
				{
					if (record.AccountGroupID == accountGroupID)
						amt += record.Amount ?? 0;
				}

				return amt;
			}

			public decimal GetAllocatedQty(int accountGroupID, int inventoryID)
			{
				PMBudgetStat result;
				if (list.TryGetValue(GetKey(accountGroupID, inventoryID), out result))
					return result.Quantity ?? 0;
				else
					return 0;
			}

			public void AddToAllocatedAmt(int accountGroupID, int inventoryID, decimal amount)
			{
				string key = GetKey(accountGroupID, inventoryID);
				if (list.ContainsKey(key))
				{
					list[key].Amount += amount;
				}
				else
				{
					PMBudgetStat bs = new PMBudgetStat(accountGroupID, inventoryID);
					bs.Amount = amount;

					list.Add(key, bs);
				}
			}

			public void AddToAllocatedQty(int accountGroupID, int inventoryID, decimal quantity)
			{
				string key = GetKey(accountGroupID, inventoryID);
				if (list.ContainsKey(key))
				{
					list[key].Quantity += quantity;
				}
				else
				{
					PMBudgetStat bs = new PMBudgetStat(accountGroupID, inventoryID);
					bs.Quantity = quantity;

					list.Add(key, bs);
				}
			}

			private static string GetKey(int accountGroupID, int inventoryID)
			{
				return string.Format("{0}.{1}", accountGroupID, inventoryID);
			}

            [Serializable]
            [PXHidden]
			public class PMTaskAllocTotalEx : PMTaskAllocTotal
			{
				#region ProjectID
				public new abstract class projectID : PX.Data.IBqlField
				{
				}
				#endregion
				#region TaskID
				public new abstract class taskID : PX.Data.IBqlField
				{
				}
				#endregion
				#region AccountGroupID
				public new abstract class accountGroupID : PX.Data.IBqlField
				{
				}
				#endregion
				#region InventoryID
				public new abstract class inventoryID : PX.Data.IBqlField
				{
				}
				#endregion

			}
		}

		protected class PMTranComparer : IComparer<PMTran>
		{
			private bool ByItem;
			private bool ByEmployee;
			private bool ByVendor;
			private bool ByDate;
			private bool ByAccountGroup;

			public PMTranComparer(PMAllocationStep step)
			{
				ByItem = step.GroupByItem == true;
				ByEmployee = step.GroupByEmployee == true;
				ByVendor = step.GroupByVendor == true;
				ByDate = step.GroupByDate == true;

				if (step.AccountGroupOrigin == PMOrigin.Source || step.OffsetAccountGroupOrigin == PMOrigin.Source)
				{
					ByAccountGroup = true;
				}
			}

			public int Compare(PMTran x, PMTran y)
			{
				//always compare by branch and finperiod:

				if (x.BranchID != y.BranchID)
					return x.BranchID.Value.CompareTo(y.BranchID.Value);

				if (x.FinPeriodID != y.FinPeriodID)
					return x.FinPeriodID.CompareTo(y.FinPeriodID);

				if (ByAccountGroup)
				{
					int xAG = x.AccountGroupID ?? 0;
					int yAG = y.AccountGroupID ?? 0;

					if (xAG != yAG)
						return xAG.CompareTo(yAG);
				}

				if (ByItem)
				{
					int xItemID = x.InventoryID ?? 0;
					int yItemID = y.InventoryID ?? 0;

					if (xItemID != yItemID)
						return xItemID.CompareTo(yItemID);
				}

				if (ByEmployee)
				{
					int xEmployeeID = x.ResourceID ?? 0;
					int yEmployeeID = y.ResourceID ?? 0;

					if (xEmployeeID != yEmployeeID)
						return xEmployeeID.CompareTo(yEmployeeID);
				}

				if (ByVendor)
				{
					int xVendorID = x.BAccountID ?? 0;
					int yVendorID = y.BAccountID ?? 0;

					if (xVendorID != yVendorID)
						return xVendorID.CompareTo(yVendorID);
				}

				if (ByDate)
				{
					return x.Date.Value.CompareTo(y.Date.Value);
				}

				bool xBillable = x.Billable == true;
				bool yBillable = y.Billable == true;

				return xBillable.CompareTo(yBillable);
			}
		}

		protected class PMTranWithTrace
		{
			public PMTran Tran;
			public Guid[] Files;
			public string NoteText;
			public List<long> OriginalTrans = new List<long>();

			public PMTranWithTrace(PMTran tran, long? originalTran)
			{
				this.Tran = tran;
				OriginalTrans.Add(originalTran.Value);
			}

			public PMTranWithTrace(PMTran tran, List<long> list)
			{
				this.Tran = tran;
				OriginalTrans.AddRange(list);
			}
		}

		public class PMDataNavigator : PX.Reports.Data.IDataNavigator
		{
			protected List<PMTran> list;
			protected PMAllocator engine;

			public PMDataNavigator(PMAllocator engine, List<PMTran> list)
			{
				this.engine = engine;
				this.list = list;
			}

			#region IDataNavigator Members

			public void Clear()
			{
			}

			public void Refresh()
			{
			}

			public object Current
			{
				get { throw new NotImplementedException(); }
			}

			public PX.Reports.Data.IDataNavigator GetChildNavigator(object record)
			{
				return null;
			}

			public object GetItem(object dataItem, string dataField)
			{
				throw new NotImplementedException();
			}

			public System.Collections.IList GetList()
			{
				return list;
			}

			public object GetValue(object dataItem, string dataField, ref string format)
			{
				PMNameNode nn = new PMNameNode(null, dataField, null);

				if (nn.IsAttribute)
					return engine.Evaluate(nn.ObjectName, null, nn.FieldName, (PMTran)dataItem);
				else
				{
					return engine.Evaluate(nn.ObjectName, nn.FieldName, null, (PMTran)dataItem);

				}
			}

			public bool MoveNext()
			{
				throw new NotImplementedException();
			}

			public void Reset()
			{
				throw new NotImplementedException();
			}

			public PX.Reports.Data.ReportSelectArguments SelectArguments
			{
				get { throw new NotImplementedException(); }
			}

			public object this[string dataField]
			{
				get { throw new NotImplementedException(); }
			}

			public string CurrentlyProcessingParam
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public object Clone()
			{
				return new PMDataNavigator(engine, list == null ? null : new List<PMTran>(list));
			}

			#endregion
		}

		protected class AllocationInfo
		{
			public HashSet<int> AccountGroups { get; private set; }
			public HashSet<string> RateTypes { get; private set; }
			public List<PMAllocationStep> Steps  { get; private set; }
			public bool ContainsBudgetStep { get; set; }

			public AllocationInfo(HashSet<int> accountGroups, HashSet<string> rateTypes, List<PMAllocationStep> steps)
			{
				this.AccountGroups = accountGroups;
				this.RateTypes = rateTypes;
				this.Steps = steps;
	}

		}

		protected class AccountGroup
		{
			public int GroupID { get; private set; }
			public string GroupCD { get; private set; }

			public AccountGroup(int id, string cd)
			{
				this.GroupID = id;
				this.GroupCD = cd;
			}
		}

		#endregion
	}
}
