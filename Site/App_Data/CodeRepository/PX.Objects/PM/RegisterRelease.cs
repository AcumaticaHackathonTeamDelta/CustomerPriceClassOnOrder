using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.CS;
using System.Diagnostics;
using PXMassProcessException = PX.Objects.AR.PXMassProcessException;

namespace PX.Objects.PM
{
	[GL.TableDashboardType]
	public class RegisterRelease : PXGraph<RegisterRelease>
	{
		public PXCancel<PMRegister> Cancel;

		[PXFilterable]
		public PXProcessing<PMRegister, Where<PMRegister.released, Equal<False>>> Items;

		public RegisterRelease()
		{
			Items.SetProcessDelegate(
				delegate(List<PMRegister> list)
				{
					List<PMRegister> newlist = new List<PMRegister>(list.Count);
					foreach (PMRegister doc in list)
					{
						newlist.Add(doc);
					}
					Release(newlist, true);
				}
			);
			Items.SetProcessCaption(Messages.Release);
			Items.SetProcessAllCaption(Messages.ReleaseAll);
		}

		public static void Release(PMRegister doc)
		{
            List<PMRegister> list = new List<PMRegister>();
           
			list.Add(doc);
            Release(list, false);
		}

		public static void Release(List<PMRegister> list, bool isMassProcess)
		{
			List<ProcessInfo<Batch>> infoList;
			
			bool releaseSuccess = ReleaseWithoutPost(list, isMassProcess, out infoList);
			if (!releaseSuccess)
			{
				throw new PXException(GL.Messages.DocumentsNotReleased);
			}

			bool postSuccess = Post(infoList, isMassProcess);
			if (!postSuccess)
			{
				throw new PXException(GL.Messages.DocumentsNotPosted);
			}
		}

		
		public static bool ReleaseWithoutPost(List<PMRegister> list, bool isMassProcess, out List<ProcessInfo<Batch>> infoList)
		{

			bool failed = false;

			RegisterReleaseProcess rg = PXGraph.CreateInstance<RegisterReleaseProcess>();
			JournalEntry je = PXGraph.CreateInstance<JournalEntry>();
			PMAllocator allocator = PXGraph.CreateInstance<PMAllocator>();
			//Task may be IsActive=False - it may be completed. User cannot create transactions with this
			//TaskID. But the system has to process the given task - hence override the FieldVerification in the Selector. 
			je.FieldVerifying.AddHandler<GLTran.projectID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });
			je.FieldVerifying.AddHandler<GLTran.taskID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });
			infoList = new List<ProcessInfo<Batch>>();

			for (int i = 0; i < list.Count; i++)
			{
				ProcessInfo<Batch> info = new ProcessInfo<Batch>(i); 
				infoList.Add(info);
				PMRegister doc = list[i];

				try
				{
					List<PMTask> allocTasks;
					info.Batches.AddRange(rg.Release(je, doc, out allocTasks));
					
					allocator.Clear();
					allocator.TimeStamp = je.TimeStamp;
					
					if (allocTasks.Count > 0)
					{
						allocator.Execute(allocTasks);
						allocator.Actions.PressSave();
					}
					if (allocator.Document.Current != null && rg.AutoReleaseAllocation )
					{
						List<PMTask> allocTasks2;
						info.Batches.AddRange(rg.Release(je, allocator.Document.Current, out allocTasks2));
					}

					if (isMassProcess)
					PXProcessing<PMRegister>.SetInfo(i, ActionsMessages.RecordProcessed);
				}
				catch (Exception e)
				{
					if (isMassProcess)
					{
						PXProcessing<PMRegister>.SetError(i, e is PXOuterException ? e.Message + "\r\n" + String.Join("\r\n", ((PXOuterException)e).InnerMessages) : e.Message);
						failed = true;
					}
					else
					{
						throw new PXMassProcessException(i, e);
					}
				}

			}

			
						
			return !failed;
		}

		public static bool Post(List<ProcessInfo<Batch>> infoList, bool isMassProcess)
		{
			PostGraph pg = PXGraph.CreateInstance<PostGraph>();
			PMSetup setup = PXSelect<PMSetup>.Select(pg);
			bool failed = false;

			if (setup != null && setup.AutoPost == true)
			{
				foreach (ProcessInfo<Batch> t in infoList)
				{
					foreach (Batch batch in t.Batches)
					{
						try
						{
							pg.Clear();
							pg.PostBatchProc(batch);
						}
						catch (Exception e)
						{
							if (isMassProcess)
							{
								failed = true;
									PXProcessing<PMRegister>.SetError(t.RecordIndex,
																	  e is PXOuterException
								                                  		? e.Message + "\r\n" +
								                                  		  String.Join("\r\n", ((PXOuterException) e).InnerMessages)
								                                  		: e.Message);
							}
							else
							{
								throw new PXMassProcessException(t.RecordIndex, new PXException(PXMessages.LocalizeNoPrefix(Messages.PostToGLFailed) + "\r\n" + e.Message, e));
							}
						}
					}
				}
			}

			return !failed;
		}
	}

	public class ProcessInfo<T> where T : class
	{
		public int RecordIndex { get; set; }
		public List<T> Batches { get; private set; }
	   
		public ProcessInfo(int index)
		{
			this.RecordIndex = index;
			this.Batches = new List<T>();
		}
	}
	public class RegisterReleaseProcess : PXGraph<RegisterReleaseProcess>
	{
		private PMSetup setup;
				
		public bool AutoReleaseAllocation
		{
			get
			{
				if (setup == null)
				{
					setup = PXSelect<PMSetup>.Select(this);
				}

				return setup.AutoReleaseAllocation == true;
			}
		}

		public List<Batch> Release(JournalEntry je, PMRegister doc, out List<PMTask> allocTasks)
		{
			allocTasks = new List<PMTask>();
			
			List<Batch> batches = new List<Batch>();
			Dictionary<string, PMTask> tasksToAutoAllocate = new Dictionary<string, PMTask>();
			List<PMTran> sourceForAllocation = new List<PMTran>();
			Dictionary<string, List<TranInfo>> transByFinPeriod = GetTransByBranchAndFinPeriod(doc);

			Debug.Assert(transByFinPeriod.Count > 0, "Failed to select transactions by finperiod in PMRegister Release.");

			using (var ts = new PXTransactionScope())
			{

				foreach (KeyValuePair<string, List<TranInfo>> kv in transByFinPeriod)
				{
					string[] parts = kv.Key.Split('.');
					int? branchID = parts[0] == "0" ? null : (int?)int.Parse(parts[0]);

					je.Clear(PXClearOption.ClearAll);

					Batch newbatch = new Batch();
					newbatch.Module = doc.Module;
					newbatch.Status = BatchStatus.Unposted;
					newbatch.Released = true;
					newbatch.Hold = false;
					newbatch.BranchID = branchID;
					newbatch.FinPeriodID = parts[1];
					newbatch.Description = doc.Description;
					je.BatchModule.Insert(newbatch);

					bool tranAdded = false;
					foreach (TranInfo t in kv.Value)
					{
						bool isGL = false;

						if (t.Tran.Released != true && t.Tran.IsNonGL != true && t.Project.BaseType == PMProject.ProjectBaseType.Project
							&& !string.IsNullOrEmpty(t.AccountGroup.Type) && t.AccountGroup.Type != PMAccountType.OffBalance && !ProjectDefaultAttribute.IsNonProject(this, t.Tran.ProjectID))
						{
							GLTran tran1 = new GLTran();
							tran1.TranDate = t.Tran.TranDate;
							tran1.TranPeriodID = t.Tran.TranPeriodID;
							tran1.SummPost = false;
							tran1.BranchID = t.Tran.BranchID;
							tran1.PMTranID = t.Tran.TranID;
							tran1.ProjectID = t.Tran.ProjectID;
							tran1.TaskID = t.Tran.TaskID;
							tran1.TranDesc = t.Tran.Description;
							tran1.ReferenceID = t.Tran.BAccountID;
							tran1.InventoryID = t.Tran.InventoryID == PMProjectStatus.EmptyInventoryID ? null : t.Tran.InventoryID;
							tran1.Qty = t.Tran.Qty;
							tran1.UOM = t.Tran.UOM;
							tran1.TranType = t.Tran.TranType;
							tran1.CuryCreditAmt = 0;
							tran1.CreditAmt = 0;
							tran1.CuryDebitAmt = t.Tran.Amount;
							tran1.DebitAmt = t.Tran.Amount;
							tran1.AccountID = t.Tran.AccountID;
							tran1.SubID = t.Tran.SubID;
							tran1.Released = true;
							je.GLTranModuleBatNbr.Insert(tran1);

							GLTran tran2 = new GLTran();
							tran2.TranDate = t.Tran.TranDate;
							tran2.TranPeriodID = t.Tran.TranPeriodID;
							tran2.SummPost = false;
							tran2.BranchID = t.Tran.BranchID;
							tran2.PMTranID = t.Tran.TranID;
							tran2.ProjectID = t.OffsetAccountGroup.GroupID != null ? t.Tran.ProjectID : ProjectDefaultAttribute.NonProject(this);
							tran2.TaskID = t.OffsetAccountGroup.GroupID != null ? t.Tran.TaskID : null;
							tran2.TranDesc = t.Tran.Description;
							tran2.ReferenceID = t.Tran.BAccountID;
							tran2.InventoryID = t.Tran.InventoryID == PMProjectStatus.EmptyInventoryID ? null : t.Tran.InventoryID;
							tran2.Qty = t.Tran.Qty;
							tran2.UOM = t.Tran.UOM;
							tran2.TranType = t.Tran.TranType;
							tran2.CuryCreditAmt = t.Tran.Amount;
							tran2.CreditAmt = t.Tran.Amount;
							tran2.CuryDebitAmt = 0;
							tran2.DebitAmt = 0;
							tran2.AccountID = t.Tran.OffsetAccountID;
							tran2.SubID = t.Tran.OffsetSubID;
							tran2.Released = true;
							je.GLTranModuleBatNbr.Insert(tran2);

							tranAdded = true;
							isGL = true;
							t.Tran.BatchNbr = je.BatchModule.Current.BatchNbr;
						}

						if (!isGL)
						{
							if (t.Tran.AccountGroupID == null && t.Project.BaseType == PMProject.ProjectBaseType.Project && t.Project.NonProject != true)
							{
								throw new PXException(Messages.AccountGroupIsRequired, doc.RefNbr);
							}
						}

					IList<PMHistory> historyList = UpdateProjectBalance(je, t.Tran, t.Account, t.AccountGroup, t.OffsetAccount, t.OffsetAccountGroup);
						if (!isGL)
						{
							#region History Update

							foreach (PMHistory item in historyList)
							{
								PMHistoryAccum hist = new PMHistoryAccum();
								hist.ProjectID = item.ProjectID;
								hist.ProjectTaskID = item.ProjectTaskID;
								hist.AccountGroupID = item.AccountGroupID;
								hist.InventoryID = item.InventoryID;
								hist.PeriodID = item.PeriodID;

								hist = (PMHistoryAccum)je.Caches[typeof(PMHistoryAccum)].Insert(hist);
								hist.FinPTDAmount += item.FinPTDAmount.GetValueOrDefault();
								hist.FinYTDAmount += item.FinYTDAmount.GetValueOrDefault();
								hist.FinPTDQty += item.FinPTDQty.GetValueOrDefault();
								hist.FinYTDQty += item.FinYTDQty.GetValueOrDefault();
								hist.TranPTDAmount += item.TranPTDAmount.GetValueOrDefault();
								hist.TranYTDAmount += item.TranYTDAmount.GetValueOrDefault();
								hist.TranPTDQty += item.TranPTDQty.GetValueOrDefault();
								hist.TranYTDQty += item.TranYTDQty.GetValueOrDefault();
							}

							#endregion
						}

						AddToUnbilledSummary(je, t.Tran);
						
						t.Tran.Released = true;
						je.Caches[typeof(PMTran)].Update(t.Tran);

						sourceForAllocation.Add(t.Tran);
						if (t.Tran.Allocated != true && t.Project.AutoAllocate == true)
						{
							if (!tasksToAutoAllocate.ContainsKey(string.Format("{0}.{1}", t.Task.ProjectID, t.Task.TaskID)))
							{
								tasksToAutoAllocate.Add(string.Format("{0}.{1}", t.Task.ProjectID, t.Task.TaskID), t.Task);
							}
						}

					}

					if (tranAdded)
					{
						je.Persist(typeof(PMHistoryAccum), PXDBOperation.Insert);//only non-gl balance is updated
						je.Save.Press();
						batches.Add(je.BatchModule.Current);
					}
					else
					{
						je.Persist(typeof(PMTran), PXDBOperation.Update);
						je.Persist(typeof(PMProjectStatusAccum), PXDBOperation.Insert);
						je.Persist(typeof(PMTaskTotal), PXDBOperation.Insert);
						je.Persist(typeof(PMHistoryAccum), PXDBOperation.Insert);//only non-gl balance is updated
						je.Persist(typeof(PMUnbilledDailySummaryAccum), PXDBOperation.Insert);
						je.SelectTimeStamp();
					}
				}

				allocTasks.AddRange(tasksToAutoAllocate.Values);

				doc.Released = true;
				je.Caches[typeof(PMRegister)].Update(doc);

				je.Persist(typeof(PMTran), PXDBOperation.Update);
				je.Persist(typeof(PMRegister), PXDBOperation.Update);
				je.Persist(typeof(PMProjectStatusAccum), PXDBOperation.Insert);
				je.Persist(typeof(PMTaskAllocTotalAccum), PXDBOperation.Insert);
				je.Persist(typeof(PMTaskTotal), PXDBOperation.Insert);
				

				ts.Complete();
			}

			return batches;
		}

		/// <summary>
		/// The key of the dictionary is a BranchID.FinPeriodID key.
		/// </summary>
		private Dictionary<string, List<TranInfo>> GetTransByBranchAndFinPeriod(PMRegister doc)
		{
			Dictionary<string, List<TranInfo>> transByFinPeriod = new Dictionary<string, List<TranInfo>>();
			
			PXSelectBase<PMTran> select = new PXSelectJoin<PMTran,
				LeftJoin<Account, On<PMTran.accountID, Equal<Account.accountID>>,
				InnerJoin<PMProject, On<PMProject.contractID, Equal<PMTran.projectID>>,
				LeftJoin<PMTask, On<PMTask.projectID, Equal<PMTran.projectID>, And<PMTask.taskID, Equal<PMTran.taskID>>>,
                LeftJoin<OffsetAccount, On<PMTran.offsetAccountID, Equal<OffsetAccount.accountID>>,
                LeftJoin<PMAccountGroup, On<PMAccountGroup.groupID, Equal<Account.accountGroupID>>,
                LeftJoin<OffsetPMAccountGroup, On<OffsetPMAccountGroup.groupID, Equal<OffsetAccount.accountGroupID>>>>>>>>,
                Where<PMTran.tranType, Equal<Required<PMTran.tranType>>, 
                And<PMTran.refNbr, Equal<Required<PMTran.refNbr>>>>>(this);

			foreach (PXResult<PMTran, Account, PMProject, PMTask, OffsetAccount, PMAccountGroup, OffsetPMAccountGroup> res in select.Select(doc.Module, doc.RefNbr))
			{
				TranInfo tran = new TranInfo(res);
				string key = string.Format("{0}.{1}", tran.Tran.BranchID.GetValueOrDefault(), tran.Tran.FinPeriodID);


				if (transByFinPeriod.ContainsKey(key))
				{
					transByFinPeriod[key].Add(tran);
				}
				else
				{
					List<TranInfo> list = new List<TranInfo>();
					list.Add(tran);
					transByFinPeriod.Add(key, list);
				}
			}

			return transByFinPeriod;
		}

		public static IList<PMHistory> UpdateProjectBalance(PXGraph graph,  PMTran tran, Account acc, PMAccountGroup ag, Account offsetAcc, PMAccountGroup offsetAg)
		{
			List<PMHistory> list = new List<PMHistory>();
			if (tran.TaskID != null)
			{
				int invert = 1;
				if (tran.TranType == BatchModule.PM && acc.Type != ag.Type)
				{
					//Invert transactions that originated in PM. All other transactions were already inverted when they were transformed from GLTran to PMTran.
					invert = -1;
				}

				bool debitcreditcancelout = false;
				if (offsetAcc != null && acc.AccountID == offsetAcc.AccountID)
				{
					debitcreditcancelout = true;
				}

				if (string.IsNullOrEmpty(ag.Type))
				{
					//offbalance tran.
					PMAccountGroup tranAG = PXSelect<PMAccountGroup, Where<PMAccountGroup.groupID, Equal<Required<PMAccountGroup.groupID>>>>.Select(graph, tran.AccountGroupID);
					if (tranAG != null)
					{
						//DEBIT ONLY
						if (tranAG.Type == AccountType.Income || tranAG.Type == AccountType.Liability)
							list.AddRange(RegisterReleaseProcess.UpdateProjectBalance(graph, tran, tranAG, null, -1 * invert));
						else
							list.AddRange(RegisterReleaseProcess.UpdateProjectBalance(graph, tran, tranAG, null, 1 * invert));
					}

				}
				else
				{
					if (!debitcreditcancelout)
					{
						//DEBIT
						if (acc.Type == AccountType.Income || acc.Type == AccountType.Liability)
							list.AddRange(RegisterReleaseProcess.UpdateProjectBalance(graph, tran, ag, acc, -1*invert));
						else
							list.AddRange(RegisterReleaseProcess.UpdateProjectBalance(graph, tran, ag, acc, 1*invert));
					}
				}

								
				//CREDIT				
				if (offsetAcc != null && offsetAg != null && offsetAcc.AccountID != null && offsetAg.GroupID != null && !debitcreditcancelout)
				{
					int offsetInvert = 1;
					if (offsetAcc.Type != offsetAg.Type)
					{
						offsetInvert = -1;
					}

					if (offsetAcc.Type == AccountType.Income || offsetAcc.Type == AccountType.Liability)
						list.AddRange(RegisterReleaseProcess.UpdateProjectBalance(graph, tran, offsetAg, offsetAcc, 1 * offsetInvert));
					else
						list.AddRange(RegisterReleaseProcess.UpdateProjectBalance(graph, tran, offsetAg, offsetAcc, -1 * offsetInvert));
				}
			}

			return list;
		}

		/// <summary>
		/// Updates Project Status Balance using PMProjectStatusAccum.
		/// </summary>
		public static IList<PMHistory> UpdateProjectBalance(PXGraph graph, PMTran pmt, PMAccountGroup ag, Account acc, int mult)
		{
			List<PMHistory> list = new List<PMHistory>();
			PXSelectBase<PMProjectStatus> selectProjectStatus = new PXSelect<PMProjectStatus,
				Where<PMProjectStatus.accountGroupID, Equal<Required<PMProjectStatus.accountGroupID>>,
				And<PMProjectStatus.projectID, Equal<Required<PMProjectStatus.projectID>>,
				And<PMProjectStatus.projectTaskID, Equal<Required<PMProjectStatus.projectTaskID>>,
				And<PMProjectStatus.inventoryID, Equal<Required<PMProjectStatus.inventoryID>>>>>>>(graph); // select any without restricting by FinPeriod

			int inventoryID = PMProjectStatus.EmptyInventoryID;
			PMProjectStatus status = null;
			if (pmt.InventoryID != null)
			{
				status = selectProjectStatus.Select(ag.GroupID, pmt.ProjectID, pmt.TaskID, pmt.InventoryID);
				if (status == null)
				{
					status = selectProjectStatus.Select(ag.GroupID, pmt.ProjectID, pmt.TaskID, PMProjectStatus.EmptyInventoryID);
				}
				else
				{
					inventoryID = status.InventoryID ?? PMProjectStatus.EmptyInventoryID;
				}
			}
			else
			{
				status = selectProjectStatus.Select(ag.GroupID, pmt.ProjectID, pmt.TaskID, PMProjectStatus.EmptyInventoryID);
			}

			string UOM = null;
			decimal rollupQty = 0;
			if (status == null)
			{
				//Status does not exist for given Inventory and <Other> is not present.
			}
			else
			{
				if (status.InventoryID == PMProjectStatus.EmptyInventoryID)
				{
					//<Other> item is present. Update only if UOMs are same.
					decimal convertedQty;
					if( IN.INUnitAttribute.TryConvertGlobalUnits(graph, pmt.UOM, status.UOM, pmt.Qty.GetValueOrDefault(), IN.INPrecision.QUANTITY, out convertedQty) )
					{
						rollupQty = convertedQty;
						UOM = status.UOM;
					}
				}
				else
				{
					UOM = status.UOM;

					//Item matches. Convert to UOM of ProjectStatus.
					if (status.UOM != pmt.UOM)
					{

						decimal inBase = IN.INUnitAttribute.ConvertToBase(graph.Caches[pmt.GetType()], pmt.InventoryID, pmt.UOM, pmt.Qty ?? 0, IN.INPrecision.QUANTITY);

						try
						{
							rollupQty = IN.INUnitAttribute.ConvertFromBase(graph.Caches[pmt.GetType()], pmt.InventoryID, status.UOM, inBase, IN.INPrecision.QUANTITY);
						}
						catch (PX.Objects.IN.PXUnitConversionException ex)
						{
							IN.InventoryItem item = PXSelect<IN.InventoryItem, Where<IN.InventoryItem.inventoryID, Equal<Required<IN.InventoryItem.inventoryID>>>>.Select(graph, pmt.InventoryID);
							string msg = PXMessages.LocalizeFormatNoPrefixNLA(Messages.UnitConversionNotDefinedForItemOnBudgetUpdate, item.BaseUnit, status.UOM, item.InventoryCD);

							throw new PXException(msg, ex);

						}
					}
					else
					{
						rollupQty = pmt.Qty ?? 0;
					}
				}
			}

            if (pmt.TaskID != null && (rollupQty != 0 || pmt.Amount != 0)) //TaskID will be null for Contract
            {
                PMProjectStatusAccum ps = new PMProjectStatusAccum();
                ps.PeriodID = pmt.FinPeriodID;
                ps.ProjectID = pmt.ProjectID;
                ps.ProjectTaskID = pmt.TaskID;
                ps.AccountGroupID = ag.GroupID;
                ps.InventoryID = inventoryID;
                ps.UOM = UOM;
				if (status != null)
					ps.IsProduction = status.IsProduction;

                ps = (PMProjectStatusAccum)graph.Caches[typeof(PMProjectStatusAccum)].Insert(ps);

				decimal amt = mult * pmt.Amount.GetValueOrDefault();

	            ps.ActualQty += rollupQty *mult;
				ps.ActualAmount += amt;
				
                graph.Views.Caches.Add(typeof(PMProjectStatusAccum));
								
                #region PMTask Totals Update

                PMTaskTotal ta = new PMTaskTotal();
                ta.ProjectID = pmt.ProjectID;
                ta.TaskID = pmt.TaskID;

                ta = (PMTaskTotal)graph.Caches[typeof(PMTaskTotal)].Insert(ta);

                string accType = null;
				int multFix = 1;//flip back the sign if it was changed because of ag.Type<>acc.type
				if (pmt.TranType == BatchModule.PM && acc != null && !string.IsNullOrEmpty(acc.Type))
				{
					//Only transactions that originated in PM were inverted and require to be fixed.
					accType = ag.Type;

					if (acc.Type != ag.Type)
						multFix = -1;

				}
                else
                {
                    accType = ag.Type;
                }

                switch (accType)
                {
                    case AccountType.Asset:
						ta.Asset += amt * multFix;
                        break;
                    case AccountType.Liability:
						ta.Liability += amt * multFix;
                        break;
                    case AccountType.Income:
						ta.Income += amt * multFix;
                        break;
                    case AccountType.Expense:
						ta.Expense += amt * multFix;
                        break;
                }
								
                graph.Views.Caches.Add(typeof(PMTaskTotal));

                #endregion
				#region History
				PMHistory hist = new PMHistory();
				hist.ProjectID = pmt.ProjectID;
				hist.ProjectTaskID = pmt.TaskID;
				hist.AccountGroupID = ag.GroupID;
				hist.InventoryID = pmt.InventoryID ?? PMProjectStatus.EmptyInventoryID;
				hist.PeriodID = pmt.FinPeriodID;
				decimal baseQty = 0;
				list.Add(hist);
				if (pmt.InventoryID != null && pmt.InventoryID != PMInventorySelectorAttribute.EmptyInventoryID && rollupQty != 0)
				{
					baseQty = mult * PX.Objects.IN.INUnitAttribute.ConvertToBase(graph.Caches[typeof(PMHistory)], pmt.InventoryID, UOM, rollupQty, PX.Objects.IN.INPrecision.QUANTITY);
				}
				hist.FinPTDAmount = amt;
				hist.FinYTDAmount = amt;
				hist.FinPTDQty = baseQty;
				hist.FinYTDQty = baseQty;
				if (pmt.FinPeriodID == pmt.TranPeriodID)
				{
					hist.TranPTDAmount = amt;
					hist.TranYTDAmount = amt;
					hist.TranPTDQty = baseQty;
					hist.TranYTDQty = baseQty;
				}
				else
				{
					PMHistory tranHist = new PMHistory();
					tranHist.ProjectID = pmt.ProjectID;
					tranHist.ProjectTaskID = pmt.TaskID;
					tranHist.AccountGroupID = ag.GroupID;
					tranHist.InventoryID = pmt.InventoryID ?? PM.PMProjectStatus.EmptyInventoryID;
					tranHist.PeriodID = pmt.TranPeriodID;
					list.Add(tranHist);
					tranHist.TranPTDAmount = amt;
					tranHist.TranYTDAmount = amt;
					tranHist.TranPTDQty = baseQty;
					tranHist.TranYTDQty = baseQty;
				}
				#endregion
            }
			return list;
		}


		/// <summary>
		/// Increases the counter of unbilled transactions.
		/// Billed and Reversed transactions are ignored.
		/// </summary>
		public static void AddToUnbilledSummary(PXGraph graph, PMTran tran)
		{
			if (tran.Billed == true || tran.Reversed == true)
				return;
			
			UpdateUnbilledSummary(graph, tran, false);
		}

		/// <summary>
		/// Decreases the counter of unbilled transactions.
		/// Only Billed or Reversed transactions are processed.
		/// </summary>
		public static void SubtractFromUnbilledSummary(PXGraph graph, PMTran tran)
		{
			if (tran.Billed != true && tran.Reversed != true)
				return;

			UpdateUnbilledSummary(graph, tran, true);
		}

		private static void UpdateUnbilledSummary(PXGraph graph, PMTran tran, bool reverse)
		{
			if (tran.ProjectID == null || tran.TaskID == null || tran.AccountGroupID == null || tran.Date == null)
			{
				return;
			}
			
			graph.Views.Caches.Add(typeof(PMUnbilledDailySummaryAccum));

			int counter = reverse ? -1 : 1;

			PMUnbilledDailySummaryAccum unbilled = new PMUnbilledDailySummaryAccum();
			unbilled.ProjectID = tran.ProjectID;
			unbilled.TaskID = tran.TaskID;
			unbilled.AccountGroupID = tran.AccountGroupID;
			unbilled.Date = tran.Date;
			unbilled = (PMUnbilledDailySummaryAccum)graph.Caches[typeof(PMUnbilledDailySummaryAccum)].Insert(unbilled);

			unbilled.Billable += tran.Billable == true ? counter : 0;
			unbilled.NonBillable += tran.Billable == true ? 0 : counter;
		}


		private class TranInfo
		{
			public PMTran Tran { get; private set; }
			public Account Account { get; private set; }
			public PMAccountGroup AccountGroup { get; private set; }
			public OffsetAccount OffsetAccount { get; private set; }
			public OffsetPMAccountGroup OffsetAccountGroup { get; private set; }
			public PMProject Project { get; private set; }
			public PMTask Task { get; private set; }

			public TranInfo(PXResult<PMTran, Account, PMProject, PMTask, OffsetAccount, PMAccountGroup, OffsetPMAccountGroup> res)
			{
				this.Tran = (PMTran)res;
				this.Account = (Account)res;
				this.AccountGroup = (PMAccountGroup)res;
				this.OffsetAccount = (OffsetAccount)res;
				this.OffsetAccountGroup = (OffsetPMAccountGroup)res;
				this.Project = (PMProject)res;
				this.Task = (PMTask)res;
			}
		}
		
		[PXHidden]
        [Serializable]
        public partial class OffsetAccount : Account
        {
            #region AccountID
            public new abstract class accountID : PX.Data.IBqlField
            {
            }
            #endregion
            #region AccountCD
            public new abstract class accountCD : PX.Data.IBqlField
            {
            }
            #endregion
            #region AccountGroupID
            public new abstract class accountGroupID : PX.Data.IBqlField
            {
            }
            #endregion
        }

		[PXHidden]
        [Serializable]
        public partial class OffsetPMAccountGroup : PMAccountGroup
        {
            #region GroupID
            public new abstract class groupID : PX.Data.IBqlField
            {
            }
           
            #endregion
            #region GroupCD
            public new abstract class groupCD : PX.Data.IBqlField
            {
            }
            #endregion
            #region Type
            public new abstract class type : PX.Data.IBqlField
            {
            }
            #endregion
        }
	}
		
}
