using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Linq;

using PX.Api;
using PX.Data;
using PX.Common;

using PX.Objects.Common;
using PX.Objects.CS;
using PX.Objects.CM;
using PX.Objects.CA;
using PX.Objects.FA;
using PX.Objects.GL.JournalEntryState;
using PX.Objects.GL.JournalEntryState.PartiallyEditable;
using PX.Objects.GL.Overrides.PostGraph;
using PX.Objects.GL.Reclassification.UI;
using PX.Objects.PM;
using PX.Objects.TX;

namespace PX.Objects.GL
{
	public class JournalEntry : PXGraph<JournalEntry, Batch>, PXImportAttribute.IPXPrepareItems, PX.Objects.GL.IVoucherEntry
	{
		public PXAction DeleteButton
		{
			get
			{
				return this.Delete;
			}
		}
		#region Cache Attached Events
		#region PMTran
		#region TranID

		[PXDBLongIdentity(IsKey = true)]
		protected virtual void PMTran_TranID_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region RefNbr
		[PXDBString(15, IsUnicode = true)]
		protected virtual void PMTran_RefNbr_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region BatchNbr
		[PXDBLiteDefault(typeof(Batch.batchNbr), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "BatchNbr")]
		protected virtual void PMTran_BatchNbr_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region Date
		[PXDBDate()]
		[PXDefault(typeof(PMRegister.date))]
		public virtual void PMTran_Date_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region FinPeriodID
		[FinPeriodID(typeof(PMRegister.date))]
		[PXUIField(DisplayName = "Fin. Period", Visibility = PXUIVisibility.SelectorVisible, Required = true)]
		public virtual void PMTran_FinPeriodID_CacheAttached(PXCache sender)
		{
		}
		#endregion
		
		#endregion

		#region GLTran
		#region LedgerID
		[PXDBInt]
		[PXFormula(typeof(Switch<Case<Where<Selector<Current<Batch.ledgerID>, Ledger.balanceType>, Equal<LedgerBalanceType.actual>>, 
			Selector<GLTran.branchID, Selector<Branch.ledgerID, Ledger.ledgerCD>>>,
			Selector<Current<Batch.ledgerID>, Ledger.ledgerCD>>))]
		[PXUIField(DisplayName = "Ledger", Enabled = false)]
		[PXSelector(typeof(Ledger.ledgerID), SubstituteKey = typeof(Ledger.ledgerCD))]
		public virtual void GLTran_LedgerID_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#endregion
		#endregion

		public ToggleCurrency<Batch> CurrencyView;

		public PXSelect<Batch, Where<Batch.module, Equal<Optional<Batch.module>>, And<Batch.draft, Equal<False>>>> BatchModule;
		public PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<Batch.curyInfoID>>>> currencyinfo;
		[PXImport(typeof(Batch))]

		[PXCopyPasteHiddenFields(typeof(GLTran.reclassified), typeof(GLTran.reclassifiedWithTranDate), typeof(GLTran.isReclassReverse), typeof(GLTran.reclassificationProhibited),
			typeof(GLTran.reclassBatchModule),typeof(GLTran.reclassBatchNbr), typeof(GLTran.reclassSourceTranModule),typeof(GLTran.reclassSourceTranBatchNbr),
			typeof(GLTran.reclassSourceTranLineNbr), typeof(GLTran.origModule), typeof(GLTran.origBatchNbr), typeof(GLTran.origLineNbr))]
		public PXSelect<GLTran, Where<GLTran.module, Equal<Current<Batch.module>>, And<GLTran.batchNbr, Equal<Current<Batch.batchNbr>>>>> GLTranModuleBatNbr;
		public PXSelect<GLAllocationHistory, Where<GLAllocationHistory.batchNbr, Equal<Current<Batch.batchNbr>>, And<GLAllocationHistory.module, Equal<Current<Batch.module>>>>> AllocationHistory;
		public PXSelect<GLAllocationAccountHistory, Where<GLAllocationAccountHistory.batchNbr, Equal<Current<Batch.batchNbr>>, And<GLAllocationAccountHistory.module, Equal<Current<Batch.module>>>>> AllocationAccountHistory;
		public PXSelect<CATran> catran;
		public PXSelectReadonly<FinPeriod, Where<FinPeriod.finPeriodID, Equal<Current<Batch.finPeriodID>>>> finperiod;
		public PXSelect<PMRegister> ProjectDocs;
		public PXSelect<PMTran> ProjectTrans;
		public PXSelect<PMTaskTotal> ProjectTaskTotals;
		public PXSetup<Branch, Where<Branch.branchID, Equal<Optional<Batch.branchID>>>> branch;
		public PXSetup<Ledger, Where<Ledger.ledgerID, Equal<Optional<Batch.ledgerID>>>> ledger;
		public PXSetup<Company> company;
		public PXSelect<Sub> ViewSub;

		public PXSelect<GLVoucher, Where<True, Equal<False>>> Voucher;

		public PXSelect<CASetup> CASetup;

		protected SummaryPostingController SummaryPostingController;

		#region Repo Methods

		public static IEnumerable<Batch> GetReversingBatches(PXGraph graph, string module, string batchNbr)
		{
			var reversingBatches = PXSelectReadonly<Batch,
										Where<Batch.origModule, Equal<Required<Batch.module>>,
											And<Batch.origBatchNbr, Equal<Required<Batch.batchNbr>>,
											And<Batch.autoReverseCopy, Equal<True>>>>>
										.Select(graph, module, batchNbr)
										.RowCast<Batch>();

			return reversingBatches;
		}

		public static IEnumerable<Batch> GetReleasedReversingBatches(PXGraph graph, string module, string batchNbr)
		{
			var reversingBatches = PXSelectReadonly<Batch,
										Where<Batch.origModule, Equal<Required<Batch.module>>,
											And<Batch.origBatchNbr, Equal<Required<Batch.batchNbr>>,
											And<Batch.autoReverseCopy, Equal<True>,
											And<Batch.released,Equal<True>>>>>>
										.Select(graph, module, batchNbr)
										.RowCast<Batch>();

			return reversingBatches;
		}

		public static Batch FindBatch(PXGraph graph, GLTran tran)
		{
			return FindBatch(graph, tran.Module, tran.BatchNbr);
		}

		public static Batch FindBatch(PXGraph graph, string module, string batchNbr)
		{
			var query = new PXSelect<Batch, Where<Batch.module, Equal<Required<Batch.module>>, And<Batch.batchNbr, Equal<Required<Batch.batchNbr>>>>>(graph);

			return query.Select(module, batchNbr);
		}


		#region Tran

		public static GLTran FindTran(PXGraph graph, GLTranKey key)
		{
			return FindTran(graph, key.Module, key.BatchNbr, key.LineNbr.Value);
		}

		public static GLTran GetTran(PXGraph graph, string module, string batchNbr, int lineNbr)
		{
			var tran = FindTran(graph, module, batchNbr, lineNbr);

			if (tran == null)
				throw new PXException(ErrorMessages.ElementDoesntExist, GLTran.GetImage(module, batchNbr, lineNbr));

			return null;
		}

		public static GLTran FindTran(PXGraph graph, string module, string batchNbr, int lineNbr)
		{
			return PXSelect<GLTran,
							Where<GLTran.module, Equal<Required<GLTran.module>>,
								And<GLTran.batchNbr, Equal<Required<GLTran.batchNbr>>,
								And<GLTran.lineNbr, Equal<Required<GLTran.lineNbr>>>>>>
							.Select(graph, module, batchNbr, lineNbr);
		}

		public static IEnumerable<GLTran> GetTrans(PXGraph graph, string module, string batchNbr)
		{
			return PXSelect<GLTran,
							Where<GLTran.module, Equal<Required<GLTran.module>>,
								And<GLTran.batchNbr, Equal<Required<GLTran.batchNbr>>>>>
							.Select(graph, module, batchNbr)
							.RowCast<GLTran>();
		}

		protected IReadOnlyCollection<PXResult<GLTran, CurrencyInfo>> GetTranCuryInfoNotInterCompany(string module, string batchNbr)
		{
			return PXSelectJoin<GLTran,
								InnerJoin<CurrencyInfo,
									On<CurrencyInfo.curyInfoID, Equal<GLTran.curyInfoID>>>,
								Where<GLTran.module, Equal<Required<GLTran.module>>,
									And<GLTran.batchNbr, Equal<Required<GLTran.batchNbr>>,
									And<GLTran.isInterCompany, Equal<False>>>>> 
								.Select(this, module, batchNbr)
								.ToArray<PXResult<GLTran, CurrencyInfo>>();
		}

		#endregion

		#endregion

		public static void OpenDocumentByTran(GLTran tran)
		{
			IDocGraphCreator creator = null;
			switch (tran.Module)
			{
				case PX.Objects.GL.BatchModule.AP:
					creator = new APDocGraphCreator();
					break;
				case PX.Objects.GL.BatchModule.AR:
					creator = new ARDocGraphCreator();
					break;
				case PX.Objects.GL.BatchModule.CA:
					creator = new CADocGraphCreator();
					break;
				case PX.Objects.GL.BatchModule.DR:
					creator = new DRDocGraphCreator();
					break;
				case PX.Objects.GL.BatchModule.IN:
					creator = new INDocGraphCreator();
					break;
				case PX.Objects.GL.BatchModule.PM:
					creator = new PMDocGraphCreator();
					break;
			}
			if (creator != null)
			{
				PXGraph graph = creator.Create(tran);
				if (graph != null)
				{
					throw new PXRedirectRequiredException(graph, true, "ViewDocument")
					{
						Mode = PXBaseRedirectException.WindowMode.NewWindow
					};
				}
			}
			throw new PXException(Messages.SourceDocumentCanNotBeFound);
		}

		#region Properties

		public PXSetup<GLSetup> glsetup;

		public bool AutoRevEntry
		{
			get
			{
				return glsetup.Current.AutoRevEntry == true;
			}
		}

		public CMSetupSelect CMSetup;

		protected CurrencyInfo _CurrencyInfo;
		public CurrencyInfo currencyInfo
		{
			get
			{
				return currencyinfo.Select();
			}
		}
		public FinPeriod FINPERIOD
		{
			get
			{
				return finperiod.Select();
			}
		}

		#endregion

		#region Buttons

		public PXAction<Batch> batchRegisterDetails;
		[PXUIField(DisplayName = Messages.BatchRegisterDetails, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable BatchRegisterDetails(PXAdapter adapter)
		{
			if (BatchModule.Current != null && BatchModule.Current.Released == true)
			{
				throw new PXReportRequiredException(CreateBatchReportParams(), "GL621000", "Batch Register Details");
			}
			return adapter.Get();
		}

		public PXAction<Batch> glEditDetails;
		[PXUIField(DisplayName = Messages.GLEditDetails, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable GLEditDetails(PXAdapter adapter)
		{
			if (BatchModule.Current != null && (bool)BatchModule.Current.Released == false && (bool)BatchModule.Current.Posted == false)
			{
				throw new PXReportRequiredException(CreateBatchReportParams(), "GL610500", "GL Edit Details");
			}
			return adapter.Get();
		}

		private Dictionary<string, string> CreateBatchReportParams()
		{
			var parameters = new Dictionary<string, string>();
			parameters["LedgerID"] = ledger.Current.LedgerCD;
			parameters["BranchID"] = branch.Current.BranchCD;
			parameters["Batch.BatchNbr"] = BatchModule.Current.BatchNbr;

			var period = BatchModule.Current.FinPeriodID.Substring(4, 2) + BatchModule.Current.FinPeriodID.Substring(0, 4);
			parameters["PeriodFrom"] = period;
			parameters["PeriodTo"] = period;

			return parameters;
		}

		public PXAction<Batch> glReversingBatches;
		[PXUIField(DisplayName = "GL Reversing Batches", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable GLReversingBatches(PXAdapter adapter)
		{
			if (BatchModule.Current != null)
			{
				var reportParams = new Dictionary<string, string>();
				reportParams["Module"] = BatchModule.Current.Module;
				reportParams["OrigBatchNbr"] = BatchModule.Current.BatchNbr;

				throw new PXReportRequiredException(reportParams, "GL690010", "GL Reversing Batches");
			}
			return adapter.Get();
		}

		public PXAction<Batch> editReclassBatch;
		[PXUIField(DisplayName = Messages.Edit, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXButton]
		public virtual IEnumerable EditReclassBatch(PXAdapter adapter)
		{
			var batch = BatchModule.Current;

			if (batch != null)
			{
				ReclassifyTransactionsProcess.OpenForReclassBatchEditing(batch);
			}

			return adapter.Get();
		}

		public PXAction<Batch> release;
		[PXUIField(DisplayName = Messages.Release, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXButton]
		public virtual IEnumerable Release(PXAdapter adapter)
		{
			PXCache cache = Caches[typeof(Batch)];
			List<Batch> list = new List<Batch>();
			
			foreach (object obj in adapter.Get())
			{
				Batch batch=null;
				if (obj is Batch)
					batch = obj as Batch;
				else if (obj is PXResult)
					batch = (obj as PXResult<Batch>);
				else
				{
					batch = (Batch)obj;
				}
				if (batch.Status == BatchStatus.Balanced)
				{
					cache.Update(batch);
					list.Add(batch);
				}
			}
			if (list.Count == 0)
			{
				throw new PXException(Messages.BatchStatusInvalid);
			}
			Save.Press();
			if (list.Count > 0)
			{
				PXLongOperation.StartOperation(this, delegate() { ReleaseBatch(list); });
			}
			return list;
		}

		#region MyButtons (MMK)
		public PXAction<Batch> action;
		[PXUIField(DisplayName = "Actions", MapEnableRights = PXCacheRights.Select)]
		[PXButton]
		protected virtual IEnumerable Action(PXAdapter adapter)
		{
			return adapter.Get();
		}

		public PXAction<Batch> report;
		[PXUIField(DisplayName = "Reports", MapEnableRights = PXCacheRights.Select)]
		[PXButton(SpecialType = PXSpecialButtonType.Report)]
		protected virtual IEnumerable Report(PXAdapter adapter)
		{
			return adapter.Get();
		}
		#endregion

		public PXAction<Batch> createSchedule;
		[PXUIField(DisplayName = Messages.AddToRepeatingTasks, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton(ImageKey = PX.Web.UI.Sprite.Main.Shedule)]
		public virtual IEnumerable CreateSchedule(PXAdapter adapter)
		{
			this.Save.Press();
			if (BatchModule.Current != null && (bool)BatchModule.Current.Released == false && (bool)BatchModule.Current.Hold == false)
			{
				ScheduleMaint sm = PXGraph.CreateInstance<ScheduleMaint>();
				if ((bool)BatchModule.Current.Scheduled && BatchModule.Current.ScheduleID != null)
				{
					sm.Schedule_Header.Current = PXSelect<Schedule,
										Where<Schedule.scheduleID, Equal<Required<Schedule.scheduleID>>>>
										.Select(this, BatchModule.Current.ScheduleID);
				}
				else
				{
					sm.Schedule_Header.Cache.Insert(new Schedule());
					Batch doc = (Batch)sm.Batch_Detail.Cache.CreateInstance();
					PXCache<Batch>.RestoreCopy(doc, BatchModule.Current);
					doc = (Batch)sm.Batch_Detail.Cache.Update(doc);
				}
				throw new PXRedirectRequiredException(sm, "Schedule");
			}
			return adapter.Get();
		}

		public PXAction<Batch> reverseBatch;
		[PXUIField(DisplayName = Messages.ReverseBatch, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable ReverseBatch(PXAdapter adapter)
		{
			if (BatchModule.Current != null)
			{
				if (BatchModule.Current.BatchType == BatchTypeCode.Reclassification)
				{
					var batch = BatchModule.Current;

					ReclassifyTransactionsProcess.OpenForReclassBatchReversing(batch);
				}
				{
				this.Save.Press();
				Batch batch = PXCache<Batch>.CreateCopy(BatchModule.Current);
					FiscalPeriodUtils.VerifyFinPeriod<Batch.finPeriodID, FinPeriod.closed>(this, BatchModule.Cache, batch,
						finperiod);
				
				try
				{
					this.ReverseBatchProc(batch);
				}
				catch(PXException)
				{
					Clear();
					throw;
				}

				if (BatchModule.Current != null)
				{
					object newValue = BatchModule.Current.FinPeriodID;
					BatchModule.Cache.RaiseExceptionHandling<Batch.finPeriodID>(BatchModule.Current, newValue, null);
					BatchModule.Cache.RaiseFieldVerifying<Batch.finPeriodID>(BatchModule.Current, ref newValue);

					List<Batch> rs = new List<Batch>();
					rs.Add(BatchModule.Current);
					return rs;
				}
			}
			}
			return adapter.Get();
		}

		public PXAction<Batch> reclassify;
		[PXUIField(DisplayName = Messages.Reclassify, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXButton]
		public virtual IEnumerable Reclassify(PXAdapter adapter)
		{
			var trans = GetFilteredTrans().ToArray();

			ReclassifyTransactionsProcess.TryOpenForReclassification<GLTran>(this,
				trans,
				ledger.Current,
				tran => BatchModule.Current.BatchType,
				BatchModule.View,
				InfoMessages.SomeTransactionsOfTheBatchCannotBeReclassified,
				InfoMessages.NoReclassifiableTransactionsHaveBeenFoundInTheBatch,
				PXBaseRedirectException.WindowMode.Same);

			return adapter.Get();
		}

		public PXAction<Batch> reclassificationHistory;
		[PXUIField(DisplayName = Messages.ReclassificationHistory, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Enabled = false)]
		[PXButton]
		public virtual IEnumerable ReclassificationHistory(PXAdapter adapter)
		{
			if (BatchModule.Current != null && GLTranModuleBatNbr.Current != null)
			{
				ReclassificationHistoryInq.OpenForTransaction(GLTranModuleBatNbr.Current);
			}

			return adapter.Get();
		}

		public PXAction<Batch> viewDocument;
		[PXUIField(DisplayName = Messages.ViewSourceDocument, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton()]
		public virtual IEnumerable ViewDocument(PXAdapter adapter)
		{
			if (this.GLTranModuleBatNbr.Current != null)
			{
				GLTran tran = (GLTran)this.GLTranModuleBatNbr.Current;

				OpenDocumentByTran(tran);
			}

			return adapter.Get();
				}

		public PXAction<Batch> viewOrigBatch;
		[PXUIField(Visible = false, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable ViewOrigBatch(PXAdapter adapter)
				{
			var tran = GLTranModuleBatNbr.Current;

			if (tran != null)
					{
				RedirectToBatch(this, tran.OrigModule, tran.OrigBatchNbr);
					}

			return adapter.Get();
				}

		public PXAction<Batch> ViewReclassBatch;
		[PXUIField(Visible = false, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable viewReclassBatch(PXAdapter adapter)
		{
			var tran = GLTranModuleBatNbr.Current;

			if (tran != null)
			{
				RedirectToBatch(this, tran.ReclassBatchModule, tran.ReclassBatchNbr);
			}

			return adapter.Get();
		}

		#endregion

		#region Functions

		public void SetZeroPostIfUndefined(GLTran tran, IReadOnlyCollection<string> transClassesWithoutZeroPost)
		{
			if (tran.ZeroPost == null)
			{
				tran.ZeroPost = !transClassesWithoutZeroPost.Contains(tran.TranClass);
			}
		}

		public int GetReversingBatchesCount(Batch batch)
		{
			var reversingBatches = GetReversingBatches(this, batch.Module, batch.BatchNbr);

			return reversingBatches.Count();
		}

		protected IEnumerable<GLTran> GetFilteredTrans()
		{
			int start = 0;
			int total = 0;

			return GLTranModuleBatNbr.View.Select(PXView.Currents, PXView.Parameters, PXView.Searches, PXView.SortColumns,
												PXView.Descendings, GLTranModuleBatNbr.View.GetExternalFilters(), ref start, PXView.MaximumRows, ref total)
										  .RowCast<GLTran>();
		}

		public void ReverseDocumentBatch(Batch batch)
		{
			var transWithCuryInfoForReverse = GetTranCuryInfoNotInterCompany(batch.Module, batch.BatchNbr);

			var countDocuments = transWithCuryInfoForReverse.Select(row=>(GLTran)row)
															.GroupBy(tran => new {tran.RefNbr, tran.TranType})
															.Count();

			if (countDocuments > 1)
			{
				throw new PXException(Messages.BatchCannotBeReversedBecauseItContainsTransactionsForMoreThanOneDocument);
			}

			Func<PXGraph, GLTran, CurrencyInfo, GLTran> buildTranDelegate =
				(graph, srcTran, curyInfo) => BuildReverseTran(graph, srcTran, TranBuildingModes.None, curyInfo);

			ReverseBatchProc(batch, transWithCuryInfoForReverse, BuildBatchHeaderBase, buildTranDelegate);
		}

		public virtual void ReverseBatchProc(Batch batch)
		{
			var transWithCuryInfo = GetTranCuryInfoNotInterCompany(batch.Module, batch.BatchNbr);

			Func<PXGraph, GLTran, CurrencyInfo, GLTran> buildTranDelegate =
				(graph, srcTran, curyInfo) => BuildReverseTran(graph, srcTran, TranBuildingModes.SetLinkToOriginal, curyInfo);

			ReverseBatchProc(batch, transWithCuryInfo, BuildReverseBatchHeader, buildTranDelegate);
		}

		public virtual Batch ReverseBatchProc(Batch srcBatch,
												IReadOnlyCollection<PXResult<GLTran, CurrencyInfo>> transWithCuryInfoForReverse,
												Func<Batch, CurrencyInfo, Batch> buildBatchHeader,
												Func<PXGraph, GLTran, CurrencyInfo, GLTran> buildTran)
		{
			Clear(PXClearOption.PreserveTimeStamp);

			if (srcBatch.Module == GL.BatchModule.CM)
			{
				throw new Exception(Messages.BatchFromCMCantBeReversed);
			}

			CurrencyInfo prev_info = null;
			CurrencyInfo traninfo = (CurrencyInfo) transWithCuryInfoForReverse.First();

			var info = PXCache<CurrencyInfo>.CreateCopy(traninfo);
			info.CuryInfoID = null;
			info.IsReadOnly = false;
			info.BaseCalc = true;
			info = PXCache<CurrencyInfo>.CreateCopy(currencyinfo.Insert(info));

			var batch = buildBatchHeader(srcBatch, info);

			batch = BatchModule.Insert(batch);

			PXNoteAttribute.CopyNoteAndFiles(BatchModule.Cache, srcBatch, BatchModule.Cache, batch);

			if (info != null)
			{
				CurrencyInfo b_info =
					(CurrencyInfo)
						PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<Batch.curyInfoID>>>>.Select(this, null);
				b_info.CuryID = info.CuryID;
				b_info.CuryEffDate = info.CuryEffDate;
				b_info.CuryRateTypeID = info.CuryRateTypeID;
				b_info.CuryRate = info.CuryRate;
				b_info.RecipRate = info.RecipRate;
				b_info.CuryMultDiv = info.CuryMultDiv;
				var copy = (CurrencyInfo)this.currencyinfo.Cache.CreateCopy(b_info);
				this.currencyinfo.Update(copy);
			}

			#region GLAllocation

			foreach (GLAllocationAccountHistory alloc in PXSelect<GLAllocationAccountHistory,
																Where<GLAllocationAccountHistory.module, Equal<Required<Batch.module>>,
																		And<GLAllocationAccountHistory.batchNbr, Equal<Required<Batch.batchNbr>>>>>
																.Select(this, srcBatch.Module,srcBatch.BatchNbr))
			{
				GLAllocationAccountHistory alloccopy = PXCache<GLAllocationAccountHistory>.CreateCopy(alloc);
				alloccopy.BatchNbr = null;
				batch.ReverseCount = 0;
				alloccopy.AllocatedAmount = -1m * alloccopy.AllocatedAmount;
				alloccopy.PriorPeriodsAllocAmount = -1m * alloccopy.PriorPeriodsAllocAmount;
				AllocationAccountHistory.Insert(alloccopy);
			}

			foreach (GLAllocationHistory alloc in PXSelect<GLAllocationHistory,
														Where<GLAllocationHistory.module, Equal<Required<Batch.module>>,
															And<GLAllocationHistory.batchNbr, Equal<Required<Batch.batchNbr>>>>>
														.Select(this, srcBatch.Module, srcBatch.BatchNbr))
			{
				GLAllocationHistory alloccopy = PXCache<GLAllocationHistory>.CreateCopy(alloc);
				alloccopy.BatchNbr = null;
				AllocationHistory.Insert(alloccopy);
			}

			#endregion


			foreach (PXResult<GLTran, CurrencyInfo> res in transWithCuryInfoForReverse)
			{
				traninfo = (CurrencyInfo) res;
				if (prev_info != null && traninfo.CuryInfoID != prev_info.CuryInfoID &&
					(!string.Equals(traninfo.CuryID, traninfo.BaseCuryID) ||
						!string.Equals(prev_info.CuryID, prev_info.BaseCuryID)))
				{
					BatchModule.Cache.RaiseExceptionHandling<Batch.origBatchNbr>(batch, null,
						new PXSetPropertyException(Messages.MultipleCurrencyInfo, PXErrorLevel.Warning));
				}
				prev_info = traninfo;


				GLTran reverseTran = buildTran(this, res, info);

				if (reverseTran.CuryDebitAmt != 0m || reverseTran.CuryCreditAmt != 0m || reverseTran.TaxID != null)
				{
					reverseTran = GLTranModuleBatNbr.Insert(reverseTran);
					PXNoteAttribute.CopyNoteAndFiles(GLTranModuleBatNbr.Cache, (GLTran)res, GLTranModuleBatNbr.Cache, reverseTran);
				}
			}

			return batch;
		}

		#region Batch Header Building

		protected Batch BuildBatchHeaderBase(Batch srcBatch, CurrencyInfo curyInfo)
		{
			var batch = PXCache<Batch>.CreateCopy(srcBatch);

			batch.BatchNbr = null;
			batch.NoteID = null;
			batch.ReverseCount = 0;
			batch.CuryInfoID = curyInfo.CuryInfoID;
			batch.Posted = false;
			batch.Voided = false;
			batch.Scheduled = false;
			batch.CuryDebitTotal = 0m;
			batch.CuryCreditTotal = 0m;
			batch.CuryControlTotal = 0m;
			batch.OrigBatchNbr = null;
			batch.OrigModule = null;
			batch.AutoReverseCopy = false;

			BatchModule.Cache.SetDefaultExt<Batch.hold>(batch);

			return batch;
		}

		protected Batch BuildReverseBatchHeader(Batch srcBatch, CurrencyInfo curyInfo)
		{
			var batch = BuildBatchHeaderBase(srcBatch, curyInfo);

			batch.Module = GL.BatchModule.GL;
			batch.Released = false;
			batch.Status = BatchStatus.Balanced;
			batch.OrigBatchNbr = srcBatch.BatchNbr;
			batch.OrigModule = srcBatch.Module;
			batch.AutoReverseCopy = true;

			return batch;
		}

		#endregion


		#region Transaction Building

		public static GLTran BuildReleasableTransaction(PXGraph graph, GLTran srcTran, TranBuildingModes buildingMode,
			CurrencyInfo curyInfo = null)
		{
			GLTran tran = PXCache<GLTran>.CreateCopy(srcTran);

			tran.Module = null;
			tran.BatchNbr = null;
			tran.LineNbr = null;

			if (buildingMode.HasFlag(TranBuildingModes.SetLinkToOriginal))
			{
				tran.OrigBatchNbr = srcTran.BatchNbr;
				tran.OrigModule = srcTran.Module;
				tran.OrigLineNbr = srcTran.LineNbr;
			}
			else
			{
				tran.OrigBatchNbr = null;
				tran.OrigModule = null;
				tran.OrigLineNbr = null;
			}

			tran.LedgerID = null;
			tran.CATranID = null;
			tran.TranID = null;
			tran.TranDate = null;
			tran.FinPeriodID = null;
			tran.TranPeriodID = null;
			tran.Released = false;
			tran.Posted = false;
			tran.ReclassSourceTranModule = null;
			tran.ReclassSourceTranBatchNbr = null;
			tran.ReclassSourceTranLineNbr = null;
			tran.ReclassBatchNbr = null;
			tran.ReclassBatchModule = null;
			tran.ReclassifiedWithTranDate = null;
			tran.Reclassified = false;
			tran.ReclassSeqNbr = null;
			tran.IsReclassReverse = false;
			tran.ReclassificationProhibited = false;
			tran.NoteID = null;

			if (curyInfo != null)
			{
				tran.CuryInfoID = curyInfo.CuryInfoID;
			}

			return tran;
		}

		[Flags]
		public enum TranBuildingModes
		{
			None = 0,
			SetLinkToOriginal = 1
		}

		public static GLTran BuildReverseTran(PXGraph graph, GLTran srcTran, TranBuildingModes buildingMode,
			CurrencyInfo curyInfo = null)
		{
			var tran = BuildReleasableTransaction(graph, srcTran, buildingMode, curyInfo);

			tran.Qty = -1m*tran.Qty;

			Decimal? curyAmount = tran.CuryCreditAmt;
			tran.CuryCreditAmt = tran.CuryDebitAmt;
			tran.CuryDebitAmt = curyAmount;

			Decimal? amount = tran.CreditAmt;
			tran.CreditAmt = tran.DebitAmt;
			tran.DebitAmt = amount;

			if (tran.ProjectID != null && PM.ProjectDefaultAttribute.IsNonProject(graph, tran.ProjectID))
			{
				tran.TaskID = null;
			}

			return tran;
		}

		#endregion

		protected bool _IsOffline = false;
		protected DocumentList<Batch> _created = null;
		public DocumentList<Batch> created
		{
			get
			{
				return _created;
			}
		}

		public bool IsOffLine
		{
			get
			{
				return _IsOffline;
			}
		}

		public JournalEntry()
		{
			GLSetup setup = glsetup.Current;

			PXUIFieldAttribute.SetVisible<GLTran.projectID>(GLTranModuleBatNbr.Cache, null, PM.ProjectAttribute.IsPMVisible(this, GL.BatchModule.GL));
			PXUIFieldAttribute.SetVisible<GLTran.taskID>(GLTranModuleBatNbr.Cache, null, PM.ProjectAttribute.IsPMVisible(this, GL.BatchModule.GL));
			PXUIFieldAttribute.SetVisible<GLTran.nonBillable>(GLTranModuleBatNbr.Cache, null, PM.ProjectAttribute.IsPMVisible(this, GL.BatchModule.GL));

			_created = new DocumentList<Batch>(this);

			var caSetup = (CASetup)CASetup.Select();

			SummaryPostingController = new SummaryPostingController(this, caSetup);
		}

		/// <summary>
		/// Removes the last elements of the <see cref="JournalEntry.created"/> collection 
		/// corresponding to GL batches that failed to persist. The absence of the batch index 
		/// in the <see cref="persistedBatchIndices"/> dictionary is used as a criterion.
		/// This method should be called each time the transaction scope is rolled back,
		/// immediately after it is rolled back.
		/// </summary>
		public void CleanupCreated(ICollection<int> persistedBatchIndices)
		{
			for (int batchIndex = this.created.Count - 1; batchIndex >= 0; --batchIndex)
			{
				if (!persistedBatchIndices.Contains(batchIndex))
				{
					this.created.RemoveAt(batchIndex);
				}
				else
				{
					break;
				}
			}
		}

		public override void SetOffline()
		{
			base.SetOffline();
			this._IsOffline = true;
		}

		public static void SegregateBatch(JournalEntry graph, string module, int? branchID, string curyID, DateTime? docDate, string finPeriodID, string description, CurrencyInfo curyInfo, Batch consolidatingBatch)
		{
			bool consolidatedPosting = graph.glsetup.Current.ConsolidatedPosting ?? false;

			graph.created.Consolidate = consolidatedPosting;
			var effectiveDate = docDate ?? (curyInfo != null ? curyInfo.CuryEffDate : null);
			var rate = consolidatedPosting == false && curyInfo != null ? curyInfo.SampleCuryRate : null;
			var rateType = curyInfo == null ? null : curyInfo.CuryRateTypeID;
			graph.Segregate(module, branchID, curyID, effectiveDate, finPeriodID, description, rate, rateType, consolidatingBatch);
		}

		public virtual void Segregate(string Module, int? BranchID, string CuryID, DateTime? DocDate, 
			string FinPeriodID, string Descr, decimal? curyRate, string curyRateType, Batch consolidatingBatch)
		{
			Segregate(Module, BranchID, CuryID, DocDate, DocDate, FinPeriodID, Descr, curyRate, curyRateType, consolidatingBatch);
		}

		public virtual void Segregate(string module, int? branchID, string curyID, DateTime? docDate, DateTime? dateEntered,
			string finPeriodID, string descr, decimal? curyRate, string curyRateType, Batch consolidatingBatch)
		{
			if (this.IsOffLine == false && this.GLTranModuleBatNbr.Cache.IsInsertedUpdatedDeleted)
			{
				//Save.Press() was for the first implementation of Quick Checks and Cash Sales when segregate was called twice.
				//under current implementation only failed release operations will move execution here
				this.Clear();
			}

			Batch existing = null;

			if (consolidatingBatch != null
				&& consolidatingBatch.Module == module
				&& consolidatingBatch.BranchID == branchID
				&& consolidatingBatch.CuryID == curyID
				&& consolidatingBatch.FinPeriodID == finPeriodID)
			{
				existing = consolidatingBatch;
			}
			else
			{
				existing = created.Find<Batch.module, Batch.branchID, Batch.curyID, Batch.finPeriodID>(module, branchID, curyID, finPeriodID);
			}

			if (existing != null)
			{
				if (!BatchModule.Cache.ObjectsEqual(BatchModule.Current, existing))
				{
					this.Clear();
				}

				Batch newbatch = this.BatchModule.Search<Batch.batchNbr>(existing.BatchNbr, existing.Module);
				if (newbatch != null)
				{
					if (newbatch.Description != descr)
					{
						newbatch.Description = "";
						this.BatchModule.Update(newbatch);
					}

					this.BatchModule.Current = newbatch;
				}
				else
				{
					created.Remove(existing);
					existing = null;
				}
			}

			if (existing == null)
			{
				this.Clear();

				Ledger ledger = PXSelectJoin<Ledger, LeftJoin<Branch, On<Ledger.ledgerID, Equal<Branch.ledgerID>>>, 
					Where<Branch.branchID, Equal<Optional<Branch.branchID>>>>.Select(this, branchID);
				if (ledger != null && 
					module != GL.BatchModule.GL && 
					module != GL.BatchModule.CM && 
					company.Current.BaseCuryID != ledger.BaseCuryID)
				{
					throw new PXException(Messages.ActualLedgerInBaseCurrency, ledger.LedgerCD, company.Current.BaseCuryID);
				}

				CurrencyInfo info = new CurrencyInfo();
				info.CuryID = curyID;
				info.CuryEffDate = docDate;
				info.CuryRateTypeID = curyRateType ?? info.CuryRateTypeID;
				currencyinfo.Cache.SetValuePending<CurrencyInfo.sampleCuryRate>(info, curyRate ?? info.SampleCuryRate);
				info = this.currencyinfo.Insert(info) ?? info;

				Batch newbatch = new Batch();
				newbatch.BranchID = branchID;
				newbatch.Module = module;
				newbatch.Status = "U";
				newbatch.Released = true;
				newbatch.Hold = false;
				newbatch.DateEntered = dateEntered;
				newbatch.FinPeriodID = finPeriodID;
				newbatch.TranPeriodID = finPeriodID;
				newbatch.CuryID = curyID;
				newbatch.CuryInfoID = info.CuryInfoID;
				newbatch.CuryDebitTotal = 0m;
				newbatch.CuryCreditTotal = 0m;
				newbatch.DebitTotal = 0m;
				newbatch.CreditTotal = 0m;
				newbatch.Description = descr;
				this.BatchModule.Insert(newbatch);

				CurrencyInfo b_info = this.currencyinfo.Select();
				if (b_info != null)
				{
					b_info.CuryID = curyID;
					this.currencyinfo.SetValueExt<CurrencyInfo.curyEffDate>(b_info, docDate);
					b_info.SampleCuryRate = curyRate ?? b_info.SampleCuryRate;
					b_info.CuryRateTypeID = curyRateType ?? b_info.CuryRateTypeID;
					this.currencyinfo.Update(b_info);
				}
			}
		}

		public static void ReleaseBatch(IList<Batch> list)
		{
			ReleaseBatch(list, null);
		}

		public static void ReleaseBatch(IList<Batch> list, IList<Batch> externalPostList)
		{
			ReleaseBatch(list, externalPostList, false);
		}
		public static void ReleaseBatch(IList<Batch> list, IList<Batch> externalPostList, bool unholdBatch)
		{
			PostGraph pg = PXGraph.CreateInstance<PostGraph>();

			bool doPost = (externalPostList == null);
			Batch batch = null;
			for (int i = 0; i < list.Count; i++)
			{
				pg.Clear(PXClearOption.PreserveData);

				batch = list[i];
				pg.ReleaseBatchProc(batch, unholdBatch);

				if ((bool)batch.AutoReverse && pg.glsetup.Current.AutoRevOption == "R")
				{
					Batch copy = pg.ReverseBatchProc(batch);
					list.Add(copy);
				}

				if (pg.AutoPost)
				{
					if (doPost)
					{
						pg.PostBatchProc(batch);
					}
					else 
					{
						externalPostList.Add(batch);
					}
				}
			}
		}

		public static void PostBatch(List<Batch> list)
		{
			PostGraph pg = PXGraph.CreateInstance<PostGraph>();

			for (int i = 0; i < list.Count; i++)
			{
				pg.Clear(PXClearOption.PreserveData);

				Batch batch = list[i];
				pg.PostBatchProc(batch);
			}
		}

		private void SetTransactionsChanged()
		{
			foreach (GLTran tran in GLTranModuleBatNbr.Select())
			{
				if (Caches[typeof(GLTran)].GetStatus(tran) == PXEntryStatus.Notchanged)
				{
					Caches[typeof(GLTran)].SetStatus(tran, PXEntryStatus.Updated);
				}
			}
		}

		private void SetTransactionsChanged<Field>()
				where Field : class, IBqlField
		{
			foreach (GLTran tran in GLTranModuleBatNbr.Select())
			{
				GLTranModuleBatNbr.Cache.SetDefaultExt<Field>(tran);
				if (Caches[typeof(GLTran)].GetStatus(tran) == PXEntryStatus.Notchanged)
				{
					Caches[typeof(GLTran)].SetStatus(tran, PXEntryStatus.Updated);
				}
			}
		}

		protected virtual void PopulateSubDescr(PXCache sender, GLTran Row, bool ExternalCall)
		{
			GLTran prevTran = null;
			foreach (GLTran tran in GLTranModuleBatNbr.Select())
			{
				if (Row == tran)
				{
					break;
				}
				prevTran = tran;
			}

			PXResultset<CashAccount> cashAccSet = PXSelect<CashAccount, Where<CashAccount.branchID, Equal<Required<CashAccount.branchID>>,
				And<CashAccount.accountID, Equal<Required<CashAccount.accountID>>>>>.SelectWindowed(this, 0, 2, Row.BranchID, Row.AccountID);
			if (cashAccSet != null && cashAccSet.Count == 1)
			{
				CashAccount cashacc = (CashAccount)cashAccSet;
				sender.SetValue<GLTran.subID>(Row, cashacc.SubID);
			}
			else if (cashAccSet.Count == 0)
				{
				if (prevTran != null && prevTran.SubID != null && Row.SubID == null)
				{
					Sub sub = (Sub)PXSelectorAttribute.Select<GLTran.subID>(sender, prevTran);
					if (sub != null)
					{
						sender.SetValueExt<GLTran.subID>(Row, sub.SubCD);
						PXUIFieldAttribute.SetError<GLTran.subID>(sender, Row, null);
					}
				}

				Account account = (Account)PXSelectorAttribute.Select<GLTran.accountID>(sender, Row);
				if (account != null && (bool)account.NoSubDetail && glsetup.Current.DefaultSubID != null)
				{
					Row.SubID = glsetup.Current.DefaultSubID;
				}
			}

			if (prevTran != null)
			{
				Row.TranDesc = prevTran.TranDesc;
				Row.RefNbr = prevTran.RefNbr;
			}
			else
			{
				Row.TranDesc = BatchModule.Current.Description;
			}

			decimal difference = (BatchModule.Current.CuryCreditTotal ?? decimal.Zero) -
				(BatchModule.Current.CuryDebitTotal ?? decimal.Zero);
			if (PXCurrencyAttribute.IsNullOrEmpty(Row.CuryDebitAmt) && PXCurrencyAttribute.IsNullOrEmpty(Row.CuryCreditAmt))
			{
				if (difference < decimal.Zero)
				{
					Row.CuryCreditAmt = Math.Abs(difference);
				}
				else
				{
					Row.CuryDebitAmt = Math.Abs(difference);
				}
			}
		}

		public override void Persist()
		{
			List<PMTask> autoAllocateTasks = CreateProjectTrans();

			if (BatchModule.Current != null && BatchModule.Cache.GetStatus(BatchModule.Current) == PXEntryStatus.Inserted)
			{
				foreach (GLTran tran in GLTranModuleBatNbr.Cache.Inserted)
				{
					if (string.Equals(tran.RefNbr, BatchModule.Current.RefNbr))
					{
						PXDBDefaultAttribute.SetDefaultForInsert<GLTran.refNbr>(GLTranModuleBatNbr.Cache, tran, true);
					}
				}
			}

			base.Persist();

			SummaryPostingController.ShouldBeNormalized();

			if (BatchModule.Current != null)
			{
				Batch existing = created.Find(BatchModule.Current);
				if (existing == null)
				{
					created.Add(BatchModule.Current);
				}
				else
				{
					BatchModule.Cache.RestoreCopy(existing, BatchModule.Current);
				}
			}

			if (autoAllocateTasks.Count > 0)
			{
				try
				{
					AutoAllocateTasks(autoAllocateTasks);
				}
				catch (Exception ex)
				{
					throw new PXException(ex, PM.Messages.AutoAllocationFailed);
				}
			}
		}

		public override void Clear()
		{
			base.Clear();

			SummaryPostingController.ResetState();
		}

		protected virtual void AutoAllocateTasks(List<PMTask> tasks)
		{
			PMSetup setup = PXSelect<PMSetup>.Select(this);
			bool autoreleaseAllocation = setup.AutoReleaseAllocation == true;

			PMAllocator allocator = PXGraph.CreateInstance<PMAllocator>();
			allocator.Clear();
			allocator.TimeStamp = this.TimeStamp;
			allocator.Execute(tasks);
			allocator.Actions.PressSave();

			if (allocator.Document.Current != null && autoreleaseAllocation)
			{
				List<PMRegister> list = new List<PMRegister>();
				list.Add(allocator.Document.Current);
				List<ProcessInfo<Batch>> batchList;
				bool releaseSuccess = RegisterRelease.ReleaseWithoutPost(list, false, out batchList);
				if (!releaseSuccess)
				{
					throw new PXException(PM.Messages.AutoReleaseFailed);
				}
			}
		}
		
		protected virtual List<PMTask> CreateProjectTrans()
		{
			List<PMTask> autoAllocateTasks = new List<PMTask>();

			if (BatchModule.Current != null && BatchModule.Current.Module != GL.BatchModule.GL)
			{
					PXSelectBase<GLTran> select = new PXSelectJoin<GLTran,
						InnerJoin<Account, On<GLTran.accountID, Equal<Account.accountID>>,
						InnerJoin<PMAccountGroup, On<PMAccountGroup.groupID, Equal<Account.accountGroupID>>,
						InnerJoin<PMProject, On<PMProject.contractID, Equal<GLTran.projectID>, And<PMProject.nonProject, Equal<False>>>,
						InnerJoin<PMTask, On<PMTask.projectID, Equal<GLTran.projectID>, And<PMTask.taskID, Equal<GLTran.taskID>>>,
						LeftJoin<PMTran, On<GLTran.origPMTranID, Equal<PMTran.tranID>>>>>>>,
						Where<GLTran.module, Equal<Current<Batch.module>>,
						And<GLTran.batchNbr, Equal<Current<Batch.batchNbr>>,
						And<Account.accountGroupID, IsNotNull,
						And<GLTran.pMTranID, IsNull,
						And<PMAccountGroup.type, NotEqual<PMAccountType.offBalance>>>>>>>(this);

				if (BatchModule.Current.Module == GL.BatchModule.AP)
				{
					select.Join<LeftJoin<AP.APTran, On<AP.APTran.refNbr, Equal<GLTran.refNbr>, And<AP.APTran.lineNbr, Equal<GLTran.tranLineNbr>, And<AP.APTran.tranType, Equal<GLTran.tranType>>>>>>();
				}
				else if (BatchModule.Current.Module == GL.BatchModule.AR)
				{
					select.Join<LeftJoin<AR.ARTran, On<AR.ARTran.refNbr, Equal<GLTran.refNbr>, And<AR.ARTran.lineNbr, Equal<GLTran.tranLineNbr>, And<AR.ARTran.tranType, Equal<GLTran.tranType>>>>>>();
				}


				
				PXResultset<GLTran> resultset = select.Select();

				PMRegister doc = null;
				if (resultset.Count > 0)
				{
					doc = new PMRegister();
					doc.Module = BatchModule.Current.Module;
					doc.Date = BatchModule.Current.DateEntered;
					doc.Description = BatchModule.Current.Description;
					doc.Released = true;
					ProjectDocs.Insert(doc);
				}


				Dictionary<string, PMTask> tasksToAutoAllocate = new Dictionary<string, PMTask>();
				List<PMTran> sourceForAllocation = new List<PMTran>();

				foreach (PXResult res in resultset)
				{
					GLTran tran = (GLTran)res[0];
					Account acc = (Account)res[1];
					PMAccountGroup ag = (PMAccountGroup)res[2];
					PMProject project = (PMProject)res[3];
					PMTask task = (PMTask)res[4];
					PMTran origTran = (PMTran)res[5];
					AP.APTran apTran = null;
					AR.ARTran arTran = null;

					if (BatchModule.Current.Module == GL.BatchModule.AP)
					{
						apTran = (AP.APTran)res[6];
					}
					else if (BatchModule.Current.Module == GL.BatchModule.AR)
					{
						arTran = (AR.ARTran)res[6];
					}



					PMTran pmt = (PMTran)ProjectTrans.Cache.Insert();
					pmt.BranchID = tran.BranchID;
					pmt.AccountGroupID = acc.AccountGroupID;
					pmt.AccountID = tran.AccountID;
					pmt.SubID = tran.SubID;
					pmt.BAccountID = tran.ReferenceID; //CustomerLocation is lost.
					//pmt.BatchNbr = tran.BatchNbr;
					pmt.Date = tran.TranDate;
					pmt.TranDate = tran.TranDate;
					pmt.Description = tran.TranDesc;
					pmt.FinPeriodID = tran.FinPeriodID;
					pmt.TranPeriodID = tran.TranPeriodID;
					pmt.InventoryID = tran.InventoryID ?? PM.PMInventorySelectorAttribute.EmptyInventoryID;
					pmt.OrigLineNbr = tran.LineNbr;
					pmt.OrigModule = tran.Module;
					pmt.OrigRefNbr = tran.RefNbr;
					pmt.OrigTranType = tran.TranType;
					pmt.ProjectID = tran.ProjectID;
					pmt.TaskID = tran.TaskID;
					if (arTran != null)
					{
						//if this is an invoice transaction force it to be non-billable 
						//so that it is not billed again even if the billing rule is configured to bill this account group.
						pmt.Billable = false;
						
					}
					else
						pmt.Billable = tran.NonBillable != true;

					if (apTran != null && apTran.Date != null)
					{
						pmt.Date = apTran.Date;
					}

					pmt.UseBillableQty = true;
					pmt.UOM = tran.UOM;
					pmt.Amount = tran.DebitAmt - tran.CreditAmt;
					pmt.Qty = tran.Qty;//pmt.Amount >= 0 ? tran.Qty : (tran.Qty * -1);
					int sign = 1;
					if (acc.Type == AccountType.Income || acc.Type == AccountType.Liability)
					{
						sign = -1;
					}
					
					if (acc.Type != ag.Type)
					{
						pmt.Amount = -pmt.Amount;
						pmt.Qty = -pmt.Qty;
					}
					pmt.BillableQty = pmt.Qty;
					pmt.Released = true;
					pmt.Allocated = tran.OrigPMTranID != null;// do not allocate or bill reverse transaction
					pmt.Billed = tran.OrigPMTranID != null;// do not allocate or bill reverse transaction

					GLTranModuleBatNbr.SetValueExt<GLTran.pMTranID>(tran, pmt.TranID);

					if (apTran != null && apTran.NoteID != null)
					{
						PXNoteAttribute.CopyNoteAndFiles(Caches[typeof(AP.APTran)], apTran, ProjectTrans.Cache, pmt);
					}
					else if (arTran != null && arTran.NoteID != null)
					{
						PXNoteAttribute.CopyNoteAndFiles(Caches[typeof(AR.ARTran)], arTran, ProjectTrans.Cache, pmt);
					}

					PM.RegisterReleaseProcess.UpdateProjectBalance(this, pmt, ag, acc, sign);
					PM.RegisterReleaseProcess.AddToUnbilledSummary(this, pmt);
										
					//Create unbilled remainder if any.
					if (origTran != null && origTran.AccountID == tran.AccountID && origTran.Amount > tran.CreditAmt)
					{
						PMTran remainder = (PMTran)ProjectTrans.Cache.CreateCopy(origTran);
						remainder.TranID = null;
						remainder.Billed = false;
						remainder.TranType = null;
						remainder.RefNbr = null;
						remainder.BatchNbr = null;
						remainder.RemainderOfTranID = origTran.TranID;
						remainder.Amount = origTran.Amount - tran.CreditAmt;
						remainder.Qty = origTran.Qty - tran.Qty;
						remainder.BillableQty = remainder.Qty; 
						ProjectTrans.Insert(remainder);
						PM.RegisterReleaseProcess.AddToUnbilledSummary(this, pmt);
					}

					sourceForAllocation.Add(pmt);
					if (pmt.Allocated != true && project.AutoAllocate == true)
					{
						if (!tasksToAutoAllocate.ContainsKey(string.Format("{0}.{1}", task.ProjectID, task.TaskID)))
						{
							tasksToAutoAllocate.Add(string.Format("{0}.{1}", task.ProjectID, task.TaskID), task);
						}
					}
				}

				autoAllocateTasks.AddRange(tasksToAutoAllocate.Values);
			}

			return autoAllocateTasks;
		}
		
		#endregion

		#region CATran Events

		protected virtual void CATran_CashAccountID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			e.Cancel = true;
		}

		protected virtual void CATran_FinPeriodID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			e.Cancel = true;
		}

		protected virtual void CATran_TranPeriodID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			e.Cancel = true;
		}

		protected virtual void CATran_ReferenceID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			e.Cancel = true;
		}

		protected virtual void CATran_CuryID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			e.Cancel = true;
		}
		#endregion

		#region CurrencyInfo Events
		protected virtual void CurrencyInfo_CuryID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (ledger.Current != null)
			{
				e.NewValue = ledger.Current.BaseCuryID;
				e.Cancel = true;
			}
		}

		protected virtual void CurrencyInfo_BaseCuryID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (ledger.Current != null)
			{
				e.NewValue = ledger.Current.BaseCuryID;
				e.Cancel = true;
			}
		}

		protected virtual void CurrencyInfo_CuryRateTypeID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				e.NewValue = CMSetup.Current.GLRateTypeDflt;
			}
		}

		protected virtual void CurrencyInfo_CuryEffDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			CurrencyInfo currencyInfo = (CurrencyInfo)e.Row;
			if (currencyInfo == null || BatchModule.Current == null || BatchModule.Current.DateEntered == null) return;
			e.NewValue = BatchModule.Current.DateEntered;
			e.Cancel = true;
		}

		protected virtual void CurrencyInfo_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			CurrencyInfo info = (CurrencyInfo)e.Row;
			object CuryID = info.CuryID;
			object CuryRateTypeID = info.CuryRateTypeID;
			object CuryMultDiv = info.CuryMultDiv;
			object CuryRate = info.CuryRate;

			if (BatchModule.Current == null || BatchModule.Current.Module != GL.BatchModule.GL)
			{
				BqlCommand sel = new Select<CurrencyInfo, Where<CurrencyInfo.curyID, Equal<Required<CurrencyInfo.curyID>>, And<CurrencyInfo.curyRateTypeID, Equal<Required<CurrencyInfo.curyRateTypeID>>, And<CurrencyInfo.curyMultDiv, Equal<Required<CurrencyInfo.curyMultDiv>>, And<CurrencyInfo.curyRate, Equal<Required<CurrencyInfo.curyRate>>>>>>>();
				foreach (CurrencyInfo summ_info in sender.Cached)
				{
					if (sender.GetStatus(summ_info) != PXEntryStatus.Deleted &&
						sender.GetStatus(summ_info) != PXEntryStatus.InsertedDeleted)
					{
						if (sel.Meet(sender, summ_info, CuryID, CuryRateTypeID, CuryMultDiv, CuryRate))
						{
							sender.SetValue(e.Row, "CuryInfoID", summ_info.CuryInfoID);
							sender.Delete(summ_info);
							return;
						}
					}
				}
			}
		}
		#endregion

		#region Batch Events

		protected virtual void Batch_LedgerID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			Batch batch = (Batch)e.Row;
			ledger.RaiseFieldUpdated(sender, e.Row);

			currencyinfo.Cache.SetDefaultExt<CurrencyInfo.baseCuryID>(currencyinfo.Current);
			currencyinfo.Cache.SetDefaultExt<CurrencyInfo.curyID>(currencyinfo.Current);
			sender.SetDefaultExt<Batch.curyID>(batch);

			foreach (GLTran tran in GLTranModuleBatNbr.Select())
			{
				GLTran newtran = PXCache<GLTran>.CreateCopy(tran);
				newtran.LedgerID = batch.LedgerID;

				GLTranModuleBatNbr.Cache.Update(newtran);
			}
		}

		protected virtual void Batch_CreateTaxTrans_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			var batch = e.Row as Batch;

			if (batch == null)
				return;

			GetStateController(batch)
				.Batch_CreateTaxTrans_FieldUpdated(sender, e);
		}

		protected virtual void Batch_CuryID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (ledger.Current != null)
			{
				e.NewValue = ledger.Current.BaseCuryID;
				e.Cancel = true;
			}
		}

		protected virtual void Batch_RowUpdated(PXCache cache, PXRowUpdatedEventArgs e)
		{
			Batch batch = (Batch)e.Row;
			Batch oldBatch = (Batch)e.OldRow;
			if ((bool)glsetup.Current.RequireControlTotal == false || batch.Status == BatchStatus.Unposted)
			{
				if (batch.CuryCreditTotal != null && batch.CuryCreditTotal != 0)
					cache.SetValue<Batch.curyControlTotal>(batch, batch.CuryCreditTotal);
				else if (batch.CuryDebitTotal != null && batch.CuryDebitTotal != 0)
					cache.SetValue<Batch.curyControlTotal>(batch, batch.CuryDebitTotal);
				else
					cache.SetValue<Batch.curyControlTotal>(batch, 0m);

				//set control total explicitly
				if (batch.CreditTotal != null && batch.CreditTotal != 0)
					cache.SetValue<Batch.controlTotal>(batch, batch.CreditTotal);
				else if (batch.DebitTotal != null && batch.DebitTotal != 0)
					cache.SetValue<Batch.controlTotal>(batch, batch.DebitTotal);
				else
					cache.SetValue<Batch.controlTotal>(batch, 0m);
			}

			bool isOutOfBalance = false;
			
			if (batch.Status == BatchStatus.Balanced || batch.Status == BatchStatus.Scheduled || batch.Status == BatchStatus.Unposted)
			{
				if (batch.CuryDebitTotal != batch.CuryCreditTotal && batch.BatchType != BatchTypeCode.TrialBalance)
				{
					isOutOfBalance = true;
				}

				if (glsetup.Current.RequireControlTotal == true)
				{
					if (batch.CuryCreditTotal != batch.CuryControlTotal && ledger.Current?.BalanceType != LedgerBalanceType.Statistical && batch.BatchType != BatchTypeCode.TrialBalance)
					{
						cache.RaiseExceptionHandling<Batch.curyControlTotal>(batch, batch.CuryControlTotal, new PXSetPropertyException(Messages.BatchOutOfBalance));
					}
					else
					{
						cache.RaiseExceptionHandling<Batch.curyControlTotal>(batch, batch.CuryControlTotal, null);
					}
				}
			}

			if (batch.DebitTotal != batch.CreditTotal && batch.Status == BatchStatus.Unposted)
			{
				isOutOfBalance = true;
			}

			if (isOutOfBalance && ledger.Current?.BalanceType != LedgerBalanceType.Statistical)
			{
				cache.RaiseExceptionHandling<Batch.curyDebitTotal>(batch, batch.CuryDebitTotal, new PXSetPropertyException(Messages.BatchOutOfBalance));
			}
			else
			{
				cache.RaiseExceptionHandling<Batch.curyDebitTotal>(batch, batch.CuryDebitTotal, null);
			}

			if (batch.Status == BatchStatus.Balanced || batch.Status == BatchStatus.Hold)
			{
				if ((batch.Module == GL.BatchModule.GL && PXAccess.FeatureInstalled<FeaturesSet.taxEntryFromGL>()) 
						&& batch.CreateTaxTrans != oldBatch.CreateTaxTrans)
				{
					if (batch.CreateTaxTrans == false)
					{
						foreach (GLTran iTran in this.GLTranModuleBatNbr.Select())
						{
							bool needUpdate = false;
							if (String.IsNullOrEmpty(iTran.TaxID) == false)
							{
								iTran.TaxID = null;
								needUpdate = true;
							}
							if (String.IsNullOrEmpty(iTran.TaxCategoryID) == false)
							{
								iTran.TaxCategoryID = null;
								needUpdate = true;
							}
							if (needUpdate)
								this.GLTranModuleBatNbr.Update(iTran);
						}
					}
					else
					{
						foreach (GLTran iTran in this.GLTranModuleBatNbr.Select())
						{
							GLTranModuleBatNbr.Cache.SetDefaultExt<GLTran.taxID>(iTran);
							GLTranModuleBatNbr.Cache.SetDefaultExt<GLTran.taxCategoryID>(iTran);
							this.GLTranModuleBatNbr.Update(iTran);
						}
					}
				}
			}
		}

		protected virtual void Batch_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			Batch batch = e.Row as Batch;

			if (batch == null)
				return;

			if (currencyinfo.Current != null && object.Equals(currencyinfo.Current.CuryInfoID, batch.CuryInfoID) == false)
			{
				currencyinfo.Current = null;
			}

			if (finperiod.Current != null && object.Equals(finperiod.Current.FinPeriodID, batch.FinPeriodID) == false)
			{
				finperiod.Current = null;
			}

			bool batchNotReleased = (batch.Released != true);

			PXDBCurrencyAttribute.SetBaseCalc<Batch.curyCreditTotal>(cache, batch, batchNotReleased);
			PXDBCurrencyAttribute.SetBaseCalc<Batch.curyDebitTotal>(cache, batch, batchNotReleased);
			PXDBCurrencyAttribute.SetBaseCalc<Batch.curyControlTotal>(cache, batch, batchNotReleased);
			PXDBCurrencyAttribute.SetBaseCalc<GLTran.curyCreditAmt>(GLTranModuleBatNbr.Cache, null, batchNotReleased);
			PXDBCurrencyAttribute.SetBaseCalc<GLTran.curyDebitAmt>(GLTranModuleBatNbr.Cache, null, batchNotReleased);

			if (batch.Released == true && batch.ReverseCount == null)
			{
				batch.ReverseCount = GetReversingBatchesCount(batch);
			}

			GetStateController(batch)
				.Batch_RowSelected(cache, e);
		}

		protected virtual void Batch_RowSelecting(PXCache cache, PXRowSelectingEventArgs e)
		{
			var batch = (Batch)e.Row;
			if (batch == null)
				return;

			if (batch.Released == true)
			{
				using (var scope = new PXConnectionScope())
				{
					batch.ReverseCount = GetReversingBatchesCount(batch);
				}
			}

		}

  protected virtual void Batch_RowInserting(PXCache cache, PXRowInsertingEventArgs e)
		{
			var batch = (Batch)e.Row;


			if (batch.BatchType == BatchTypeCode.Reclassification)
			{
				if (e.ExternalCall)
				{
					batch.BatchType = BatchTypeCode.Normal;
				}
			} 
		}

		private bool IsBatchReadonly(Batch batch)
		{
			return (batch.Module != GL.BatchModule.GL && BatchModule.Cache.GetStatus(batch) == PXEntryStatus.Inserted)
				   || batch.Voided == true || batch.Released == true;
		}

		protected virtual void Batch_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			var batch = e.Row as Batch;
			if (batch == null)
				return;

			CheckBatchBranchHasLedger(sender, batch);
		}

		internal static void CheckBatchBranchHasLedger(PXCache cache, Batch batch)
		{
			if (batch.Module != GL.BatchModule.GL && batch.LedgerID == null && batch.BranchID != null)
			{
				var branch = (Branch)PXSelectorAttribute.Select<Batch.branchID>(cache, batch);

				if(branch != null)
					throw new PXException(
						PXAccess.FeatureInstalled<FeaturesSet.branch>() ?
							Messages.LedgerMissingForBranchInMultiBranch : Messages.LedgerMissingForBranchInSingleBranch,
						(branch.BranchCD ?? "").TrimEnd());
			}
		}

		protected virtual void Batch_DateEntered_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			var newDate = (DateTime)e.NewValue;
			var batch = (Batch)e.Row;

			if (batch.BatchType == BatchTypeCode.Reclassification)
		{
				FinPeriodIDAttribute.CheckIsDateWithinPeriod(this, batch.FinPeriodID, newDate, Messages.TheDateIsOutsideOfTheFinancialPeriodOfTheBatchTransactions);
			}
		}

		protected virtual void Batch_DateEntered_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			Batch batch = (Batch)e.Row;

			SetTransactionsChanged<GLTran.tranDate>();

			CurrencyInfoAttribute.SetEffectiveDate<Batch.dateEntered>(cache, e);
		}

		protected virtual void Batch_FinPeriodID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			SetTransactionsChanged();
		}

		protected virtual void Batch_Module_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = GL.BatchModule.GL;
		}

		protected virtual void Batch_AutoReverse_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			Batch row = (Batch) e.Row;
			if(row.AutoReverse == true )
			{
				cache.SetValueExt<Batch.createTaxTrans>(row, false);
			} 
		}

		protected virtual void Batch_AutoReverseCopy_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			Batch row = (Batch)e.Row;
			if (row.AutoReverseCopy == true)
			{
				cache.SetValueExt<Batch.createTaxTrans>(row, false);
			}
		}

		protected virtual void Batch_BatchNbr_FieldSelecting(PXCache cache, PXFieldSelectingEventArgs e)
		{
			Batch batch = e.Row as Batch;

			if (batch == null)
				return;
			bool batchNotReleased = (batch.Released != true);
			PXDBCurrencyAttribute.SetBaseCalc<Batch.curyCreditTotal>(cache, batch, batchNotReleased);
			PXDBCurrencyAttribute.SetBaseCalc<Batch.curyDebitTotal>(cache, batch, batchNotReleased);
		}
		#endregion

		#region GLTran Events
		private bool _importing;

		protected virtual void GLTran_AccountID_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
		{
			if (e.Row != null) _importing = sender.GetValuePending(e.Row, PXImportAttribute.ImportFlag) != null && !IsExport;
		}

		protected virtual void GLTran_LedgerID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<GLTran.projectID>(e.Row);
		}

		protected virtual void GLTran_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			GLTran tran = (GLTran)e.Row;
			if (e.ExternalCall && (e.Row == null || !_importing) && sender.GetStatus(tran) == PXEntryStatus.Inserted && ((GLTran)e.OldRow).AccountID == null && tran.AccountID != null)
			{
				GLTran oldrow = PXCache<GLTran>.CreateCopy(tran);
				PopulateSubDescr(sender, tran, e.ExternalCall);
				sender.RaiseRowUpdated(tran, oldrow);
			}
		}

		protected virtual void GLTran_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			GLTran tran = (GLTran)e.Row;
			if (e.ExternalCall && (e.Row == null || !_importing) && sender.GetStatus(tran) == PXEntryStatus.Inserted && tran.AccountID != null)
			{
				GLTran oldrow = PXCache<GLTran>.CreateCopy(tran);
				PopulateSubDescr(sender, tran, e.ExternalCall);
				sender.RaiseRowUpdated(tran, oldrow);
			}

			if (tran.Module != GL.BatchModule.GL
				&& tran.SummPost == true)
				{
				SummaryPostingController.AddSummaryTransaction(tran);
			}
		}

		protected virtual void GLTran_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			GLTran tran = e.Row as GLTran;

			if (tran.Module != GL.BatchModule.GL)
			{
				SummaryPostingController.RemoveIfNeeded(tran);
			}
		}

		protected virtual void GLTran_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			GLTran tran = (GLTran)e.Row;
			object BranchID = tran.BranchID;
			object AccountID = tran.AccountID;
			object SubID = tran.SubID;
			object CuryInfoID = tran.CuryInfoID;
			object ProjectID = tran.ProjectID;
			object TaskID = tran.TaskID;
			object CATranID = tran.CATranID;

			if (tran.RefNbr == null)
			{
				tran.RefNbr = string.Empty;
			}

			if (tran.TranDesc == null)
			{
				tran.TranDesc = string.Empty;
			}

			object RefNbr = tran.RefNbr;
			object TranDesc = tran.TranDesc;


			if (tran.Module != GL.BatchModule.GL)
			{
				e.Cancel = SummaryPostingController.TryAggregateToSummaryTransaction(tran);

				if (!e.Cancel)
				{
					PostGraph.NormalizeAmounts(tran);
				}

				if (e.Cancel == false)
				{
					e.Cancel = (tran.CuryDebitAmt == 0 &&
								tran.CuryCreditAmt == 0 &&
								tran.DebitAmt == 0 &&
								tran.CreditAmt == 0 &&
								tran.ZeroPost != true);
				}

				if (e.Cancel == false)
				{
					BranchAcctMapFrom mapfrom;
					BranchAcctMapTo mapto;

					if (!PostGraph.GetAccountMapping(this, BatchModule.Current, tran, out mapfrom, out mapto))
					{
						Branch branchfrom = (Branch)PXSelectorAttribute.Select<Batch.branchID>(BatchModule.Cache, BatchModule.Current, BatchModule.Current.BranchID);
						Branch branchto = (Branch)PXSelectorAttribute.Select<GLTran.branchID>(sender, tran, tran.BranchID);

						throw new PXException(Messages.BrachAcctMapMissing, (branchfrom != null ? branchfrom.BranchCD : "Undefined"), (branchto != null ? branchto.BranchCD : "Undefined"));
					}
				}
			}
		}

		protected virtual void Batch_BranchID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<Batch.ledgerID>(e.Row);
		}

		protected virtual void GLTran_AccountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			Account a = (Account)PXSelectorAttribute.Select<GLTran.accountID>(sender, e.Row);
			if (a != null)
			{
				sender.SetDefaultExt<GLTran.projectID>(e.Row);
				sender.SetDefaultExt<GLTran.taxID>(e.Row);
				sender.SetDefaultExt<GLTran.taxCategoryID>(e.Row);
			}
		}

		protected virtual void GLTran_SubID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			GLTran row = (GLTran)e.Row;
			if (row!= null)
			{                
				sender.SetDefaultExt<GLTran.taxID>(e.Row);
			}
		}

		protected virtual void GLTran_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			GLTran row = e.Row as GLTran;
			if (row != null && row.ProjectID == null)
			{
				Account account = PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this, row.AccountID);
				if (account != null && account.AccountGroupID != null)
				{
					sender.RaiseExceptionHandling<GLTran.projectID>(e.Row, row.ProjectID, new PXSetPropertyException(Messages.ProjectIsRequired, account.AccountCD));
				}
			}
		}

		protected virtual void GLTran_RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
		{
			var tran = (GLTran) e.Row;

			if (e.Operation == PXDBOperation.Delete && e.TranStatus == PXTranStatus.Open)
			{
				PXUpdate<
					  Set<GLTran.reclassBatchNbr, Null,
					  Set<GLTran.reclassBatchModule, Null>>,
				  GLTran,
				  Where<GLTran.module, Equal<Required<GLTran.module>>,
					  And<GLTran.batchNbr, Equal<Required<GLTran.batchNbr>>,
					  And<GLTran.lineNbr, Equal<Required<GLTran.lineNbr>>>>>>
				  .Update(this, tran.OrigModule, tran.OrigBatchNbr, tran.OrigLineNbr);
			}
		}

		protected virtual void GLTran_CuryCreditAmt_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			GLTran row = (GLTran)e.Row;

			if (row != null)
			{
				if (row.CuryDebitAmt != null && row.CuryDebitAmt != 0 && row.CuryCreditAmt != null && row.CuryCreditAmt != 0)
				{
					row.CuryDebitAmt = 0.0m;
					row.DebitAmt = 0.0m;
				}
				//row.CreditAmt = row.CuryCreditAmt;
			}
		}

		protected virtual void GLTran_CuryDebitAmt_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			GLTran row = (GLTran)e.Row;

			if (row != null)
			{
				if (row.CuryCreditAmt != null && row.CuryCreditAmt != 0 && row.CuryDebitAmt != null && row.CuryDebitAmt != 0)
				{
					row.CuryCreditAmt = 0.0m;
					row.CreditAmt = 0.0m;
				}
				//row.DebitAmt = row.CuryDebitAmt;
			}
		}

		protected virtual void GLTran_TaxID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			GLTran row = e.Row as GLTran;
			Batch batch = this.BatchModule.Current;
			
			if (batch != null && batch.CreateTaxTrans == true)
			{
				e.NewValue = null;
				if(row.AccountID !=null && row.SubID != null) 
				{
					PXResultset<TX.Tax> taxset = PXSelect<TX.Tax, Where2<Where<TX.Tax.purchTaxAcctID, Equal<Required<GLTran.accountID>>,
								And<TX.Tax.purchTaxSubID, Equal<Required<GLTran.subID>>>>,
								Or<Where<TX.Tax.salesTaxAcctID, Equal<Required<GLTran.accountID>>,
								And<TX.Tax.salesTaxSubID, Equal<Required<GLTran.subID>>>>>>>.Select(this, row.AccountID, row.SubID, row.AccountID, row.SubID);
					if (taxset.Count == 1) 
					{
						e.NewValue = ((Tax)taxset[0]).TaxID;
					}
					else if (taxset.Count > 1
						&& row.TaxID != null && taxset.RowCast<Tax>().Any(t => t.TaxID == row.TaxID))
					{
						e.NewValue = row.TaxID;
					}
					else if (taxset.Count > 1)
					{
						this.GLTranModuleBatNbr.Cache.RaiseExceptionHandling<GLTran.taxID>(row, null, new PXSetPropertyException(Messages.TaxIDMissingForAccountAssociatedWithTaxes, PXErrorLevel.Warning));
						//MS This is needed because of the usage PopulateSubDescr makes last RowSelected call before SubID is initialized - so the warning does not appear for the new records with  "right" default values.
					}
				}
				e.Cancel = true;
			}
			
		}

		protected virtual void GLTran_TaxCategoryID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			var batch = BatchModule.Current;

			if (batch == null || e.Row == null)
				return;

			GetStateController(batch)
							.GLTran_TaxCategoryID_FieldDefaulting(sender, e, batch);
		}

		protected virtual void GLTran_TaxID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			GLTran tran = e.Row as GLTran;
			if (tran == null) return;
			GLTran newtran = sender.CreateCopy(tran) as GLTran;
			newtran.TaxID = e.NewValue as string;
			if (newtran.TaxID != null)
			{
				Tax tax = PXSelectorAttribute.Select<GLTran.taxID>(sender, newtran) as Tax;
				if (tax != null)
				{
					if (tax.PurchTaxAcctID == tax.SalesTaxAcctID && tax.PurchTaxSubID == tax.SalesTaxSubID)
					{
						sender.RaiseExceptionHandling<GLTran.taxID>(tran, tax.TaxID, new PXSetPropertyException(TX.Messages.ClaimableAndPayableAccountsAreTheSame, tax.TaxID));
						e.NewValue = tran.TaxID;
						e.Cancel = true;
						return;
					}

					string taxType = 
						(tax.PurchTaxAcctID == tran.AccountID && tax.PurchTaxSubID == tran.SubID) ? TaxType.Purchase : 
							(tax.SalesTaxAcctID == tran.AccountID && tax.SalesTaxSubID == tran.SubID) ? TaxType.Sales : null;

					if (taxType != null)
					{
						TaxRev taxrev = PXSelectReadonly<TaxRev, Where<TaxRev.taxID, Equal<Required<TaxRev.taxID>>,
							And<TaxRev.outdated, Equal<False>,
								And<TaxRev.taxType, Equal<Required<TaxRev.taxType>>>>>>.SelectWindowed(sender.Graph, 0, 1, tax.TaxID, taxType);

						if (taxrev == null)
						{
							string humanReadableTaxType = PXMessages.LocalizeNoPrefix(GetLabel.For<TaxType>(taxType)).ToLower();

							sender.RaiseExceptionHandling<GLTran.taxID>(
								tran, 
								tax.TaxID, 
								new PXSetPropertyException(TX.Messages.TaxRateNotSpecified, humanReadableTaxType));

							e.NewValue = tran.TaxID;
							e.Cancel = true;
							return;
						}
					}
				}
			}
		}
		
		protected virtual void GLTran_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			var batch = BatchModule.Current;

			if (batch == null || e.Row == null)
				return;

			GetStateController(BatchModule.Current)
						.GLTran_RowSelected(sender, e, batch);
		}

		#endregion

		#region PMRegister Events

		protected virtual void PMRegister_RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
		{
			//CreateProjectTrans() can create more then one PMRegister because of autoallocation
			//hense set the RefNbr Manualy for all child transactios.

			PMRegister row = (PMRegister)e.Row;

			if (e.Operation == PXDBOperation.Insert)
			{
				if (e.TranStatus == PXTranStatus.Open)
				{
					foreach (PMTran tran in ProjectTrans.Cache.Inserted)
					{
						if (tran.TranType == row.Module)
						{
							tran.RefNbr = row.RefNbr;
						}

					}
				}
			}
		}
		
		#endregion

		#region Implementation of IPXPrepareItems

		public virtual bool PrepareImportRow(string viewName, IDictionary keys, IDictionary values)
		{
			if (viewName == "GLTranModuleBatNbr")
			{
				var creditAmt = CorrectImportValue(values, "CreditAmt", "0");
				CorrectImportValue(values, "CuryCreditAmt", creditAmt);
				var debitAmt = CorrectImportValue(values, "DebitAmt", "0");
				CorrectImportValue(values, "CuryDebitAmt", debitAmt);
			}
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

		public virtual void PrepareItems(string viewName, IEnumerable items)
		{
		}

		private static string CorrectImportValue(IDictionary dic, string fieldName, string defValue)
		{
			var result = defValue;
			if (!dic.Contains(fieldName)) dic.Add(fieldName, defValue);
			else
			{
				var val = dic[fieldName];
				Decimal mVal;
				string sVal;
				if (val == null ||
					string.IsNullOrEmpty(sVal = val.ToString()) ||
					!decimal.TryParse(sVal, out mVal))
				{
					dic[fieldName] = defValue;
				}
				else result = sVal;
			}
			return result;
		}

		#endregion

		public IEnumerable<GLTran> CreateTransBySchedule(DR.DRProcess dr, GLTran templateTran)
		{
			decimal drSign = (decimal)templateTran.DebitAmt == 0 ? 0 : 1;
			decimal crSign = (decimal)templateTran.CreditAmt == 0 ? 0 : 1;

			var result = new List<GLTran>();

			foreach (DR.DRScheduleDetail scheduled in dr.GetScheduleDetails(dr.Schedule.Current.ScheduleID))
			{
				var tran = (GLTran)GLTranModuleBatNbr.Cache.CreateCopy(templateTran);

				tran.AccountID = scheduled.DefAcctID;
				tran.SubID = scheduled.DefSubID;
				tran.ReclassificationProhibited = true;

				tran.DebitAmt = drSign * scheduled.TotalAmt;
				tran.CreditAmt = crSign * scheduled.TotalAmt;

				PXCurrencyAttribute.CuryConvCury<GLTran.curyCreditAmt>(GLTranModuleBatNbr.Cache, tran);
				PXCurrencyAttribute.CuryConvCury<GLTran.curyDebitAmt>(GLTranModuleBatNbr.Cache, tran);

				result.Add(tran);
			}

			return result;
		}

		public virtual void CorrectCuryAmountsDueToRounding(IEnumerable<GLTran> transactions, GLTran templateTran, decimal curyExpectedTotal)
		{
			if (transactions.Any() == false)
				return;

			decimal drSign = (decimal)templateTran.DebitAmt == 0 ? 0 : 1;
			decimal crSign = (decimal)templateTran.CreditAmt == 0 ? 0 : 1;

			Func<GLTran, decimal> tranAmount = t => (t.CuryDebitAmt ?? 0m) * drSign + (t.CuryCreditAmt ?? 0m) * crSign;

			var difference = curyExpectedTotal - transactions.Sum(tranAmount);

			var tranToAmend = transactions.OrderByDescending(t => Math.Abs(tranAmount(t))).First();

			tranToAmend.CuryDebitAmt += difference * drSign;
			tranToAmend.CuryCreditAmt += difference * crSign;
		}

		public static void RedirectToBatch(PXGraph graph, string module, string batchNbr)
		{
			var batch = FindBatch(graph, module, batchNbr);

			if (batch != null)
			{
				RedirectToBatch(batch);
			}
		}

		public static void RedirectToBatch(Batch batch)
		{
			if (batch == null)
				throw new ArgumentNullException("batch");

			var graph = PXGraph.CreateInstance<JournalEntry>();
			
			graph.BatchModule.Current = batch;
			
			throw new PXRedirectRequiredException(graph, true, Messages.ViewBatch)
			{
				Mode = PXBaseRedirectException.WindowMode.NewWindow
			};
		}

		public static void SetReclassTranWarningsIfNeed(PXCache cache, GLTran tran)
		{
			string message = null;

			if (tran.Reclassified == true)
			{
				message = Messages.TransHasBeenReclassified;
			}
			else if (tran.ReclassBatchNbr != null)
			{
				message = Messages.UnreleasedReclassificationBatchExists;
			}

			if (message != null)
			{
				cache.RaiseExceptionHandling<GLTran.reclassBatchNbr>(tran, null,
						new PXSetPropertyException(message, PXErrorLevel.RowWarning));
			}
		}

		public static bool IsTransactionReclassifiable(GLTran tran, string batchType, string ledgerBalanceType, int? nonProjectID)
		{
			return tran.ReclassBatchNbr == null
				   && tran.IsReclassReverse != true
				   && tran.Released == true
				   && tran.ReclassificationProhibited != true
				   && tran.IsInterCompany != true
				   && IsModuleReclassifiable(tran.Module)
				   && IsBatchTypeReclassifiable(batchType)
				   && LedgerBalanceTypeAllowReclassification(ledgerBalanceType)
				   && tran.ProjectID == nonProjectID;
		}

		public static bool IsBatchTypeReclassifiable(string batchType)
		{
			return batchType != BatchTypeCode.TrialBalance
				   && batchType != BatchTypeCode.Consolidation
				   && batchType != BatchTypeCode.Allocation;
		}

		public static bool IsModuleReclassifiable(string module)
		{
			return module != GL.BatchModule.CM;
		}

		public static bool LedgerBalanceTypeAllowReclassification(string ledgerType)
		{
			return ledgerType == LedgerBalanceType.Actual || ledgerType == null;
		}

		public static bool IsBatchReclassifiable(Batch batch, Ledger ledger)
		{
			return batch.Released == true
				   && IsBatchTypeReclassifiable(batch.BatchType)
				   && LedgerBalanceTypeAllowReclassification(ledger.BalanceType)
				   && IsModuleReclassifiable(batch.Module);
		}

		public static bool IsReclassifacationTran(GLTran tran)
		{
			return tran.ReclassSourceTranBatchNbr != null;
		}

		public static bool CanShowReclassHistory(GLTran tran, string batchType)
		{
			return batchType == BatchTypeCode.Reclassification ||
										tran.ReclassBatchNbr != null;
		}

		protected virtual StateControllerBase GetStateController(Batch batch)
		{
			if (IsBatchReadonly(batch))
			{
				return new ReadonlyStateController(this);
			}

			if (batch.BatchType == BatchTypeCode.Reclassification)
			{
				return new ReclassStateController(this);
			}

			if (batch.BatchType == BatchTypeCode.TrialBalance)
			{
				return new TrialBalanceStateController(this);
			}

			return new CommonTypeStateController(this);
		}
	}


	[Serializable]
	public class PostGraph : PXGraph<PostGraph>
	{
		#region Cache Attached Events
		#region GLTran
		#region BranchID
		[PXDBInt()]
		[PXSelector(typeof(Search<Branch.branchID>), SubstituteKey = typeof(Branch.branchCD), CacheGlobal = true)]
		protected virtual void GLTran_BranchID_CacheAttached(PXCache cache)
		{
		}
		#endregion
		#region LedgerID
		[PXDBInt()]
		protected virtual void GLTran_LedgerID_CacheAttached(PXCache cache)
		{
		}
		#endregion
		#region AccountID
		[PXDBInt()]
		[PXSelector(typeof(Search<Account.accountID>), SubstituteKey = typeof(Account.accountCD), CacheGlobal = true)]
		protected virtual void GLTran_AccountID_CacheAttached(PXCache cache)
		{
		}
		#endregion
		#region SubID
		[SubAccount]
		protected virtual void GLTran_SubID_CacheAttached(PXCache cache)
		{
		}
		#endregion
		#region OrigAccountID
		[PXDBInt()]
		protected virtual void GLTran_OrigAccountID_CacheAttached(PXCache cache)
		{
		}
		#endregion
		#region OrigSubID
		[PXDBInt()]
		protected virtual void GLTran_OrigSubID_CacheAttached(PXCache cache)
		{
		}
		#endregion
		#region RefNbr
		[PXDBString(15, IsUnicode = true)]
		protected virtual void GLTran_RefNbr_CacheAttached(PXCache cache)
		{
		}
		#endregion
		#region TranDesc
		[PXDBString(255, IsUnicode = true)]
		protected virtual void GLTran_TranDesc_CacheAttached(PXCache cache)
		{
		}
		#endregion
		#region DebitAmt
		[PXDBDecimal(4)]
		protected virtual void GLTran_DebitAmt_CacheAttached(PXCache cache)
		{
		}
		#endregion
		#region CreditAmt
		[PXDBDecimal(4)]
		protected virtual void GLTran_CreditAmt_CacheAttached(PXCache cache)
		{
		}
		#endregion
		#region CuryDebitAmt
		[PXDBDecimal(4)]
		protected virtual void GLTran_CuryDebitAmt_CacheAttached(PXCache cache)
		{
		}
		#endregion
		#region CuryCreditAmt
		[PXDBDecimal(4)]
		protected virtual void GLTran_CuryCreditAmt_CacheAttached(PXCache cache)
		{
		}
		#endregion
		#region CuryInfoID
		[PXDBLong()]
		protected virtual void GLTran_CuryInfoID_CacheAttached(PXCache cache)
		{
		}
		#endregion
		#region CATranID
		[PXDBLong()]
		protected virtual void GLTran_CATranID_CacheAttached(PXCache cache)
		{
		}
		#endregion
		#region ProjectID
		[PXDBInt()]
		protected virtual void GLTran_ProjectID_CacheAttached(PXCache cache)
		{
		}
		#endregion
		#region TaskID
		[PXDBInt()]
		protected virtual void GLTran_TaskID_CacheAttached(PXCache cache)
		{
		}
		#endregion
		#region PMTranID
		[PXDBLong()]
		protected virtual void GLTran_PMTranID_CacheAttached(PXCache cache)
		{
		}
		#endregion
		#endregion
		#region PMRegister
		#region BatchNbr
		[PXDBLiteDefault(typeof(Batch.batchNbr))]
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "BatchNbr")]
		protected virtual void PMRegister_BatchNbr_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#endregion
		#region PMTran
		#region TranID
		[PXDBLongIdentity(IsKey = true)]
		protected virtual void PMTran_TranID_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region RefNbr
		[PXDBLiteDefault(typeof(PMRegister.refNbr))]// is handled by the graph
		[PXDBString(15, IsUnicode = true)]
		protected virtual void PMTran_RefNbr_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region BatchNbr
		[PXDBLiteDefault(typeof(Batch.batchNbr), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "BatchNbr")]
		protected virtual void PMTran_BatchNbr_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region Date
		[PXDBDate()]
		[PXDefault(typeof(PMRegister.date))]
		public virtual void PMTran_Date_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#endregion

		#region TaxTran
		#region FinPeriodID        
		[GL.FinPeriodID()]
		[PXDefault()]		
		protected virtual void TaxTran_FinPeriodID_CacheAttached(PXCache cache)
		{
		}
		#endregion
		#region TaxPeriodID
		[GL.FinPeriodID()]        
		protected virtual void TaxTran_TaxPeriodID_CacheAttached(PXCache cache)
		{
		}
		#endregion
		#region VendorID
		[PXDBInt()]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]        
		protected virtual void TaxTran_VendorID_CacheAttached(PXCache cache)
		{
		}
		#endregion
		#region RefNbr
		[PXDBString(15, IsUnicode = true, IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Ref. Nbr.", Enabled = false, Visible = false)]
		protected virtual void TaxTran_RefNbr_CacheAttached(PXCache cache)
		{
		}
		#endregion
		#region TranDate
		[PXDBDate()]
		[PXDefault()]
		protected virtual void TaxTran_TranDate_CacheAttached(PXCache cache)
		{
		}
		#endregion
		#region TranType
		[PXDBString(3, IsKey = true, IsFixed = true)]
		[PXDefault(" ")]
		protected virtual void TaxTran_TranType_CacheAttached(PXCache cache)
		{
		}
		#endregion
		#region TaxZoneID
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Tax Zone")]		        
		protected virtual void TaxTran_TaxZoneID_CacheAttached(PXCache cache)
		{
		}
		#endregion
		#region TaxID
		[PXDBString(Tax.taxID.Length, IsUnicode = true, IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Tax ID", Visibility = PXUIVisibility.Visible)]
		[PXSelector(typeof(Search<Tax.taxID>))]
		protected virtual void TaxTran_TaxID_CacheAttached(PXCache cache)
		{
		}
		#endregion
		#endregion
		#endregion

		public PXSelectJoin<GLTran,
			LeftJoin<CurrencyInfo, On<GLTran.curyInfoID, Equal<CurrencyInfo.curyInfoID>>,
			LeftJoin<Account, On<GLTran.accountID, Equal<Account.accountID>>,
			LeftJoin<Ledger, On<GLTran.ledgerID, Equal<Ledger.ledgerID>>>>>,
			Where<GLTran.module, Equal<Optional<Batch.module>>,
			And<GLTran.batchNbr, Equal<Optional<Batch.batchNbr>>>>> GLTran_Module_BatNbr;

		[Obsolete("CurrencyInfo table will be removed from the view in the later versions.")]
		public PXSelectJoin<GLTran,
			LeftJoin<CATran, 
				On<CATran.tranID, Equal<GLTran.cATranID>>,
			LeftJoin<CurrencyInfo, 
				On<GLTran.curyInfoID, Equal<CurrencyInfo.curyInfoID>>,
			LeftJoin<Account, 
				On<GLTran.accountID, Equal<Account.accountID>>,
			LeftJoin<Ledger, 
				On<GLTran.ledgerID, Equal<Ledger.ledgerID>>>>>>,
			Where<GLTran.module, Equal<Optional<Batch.module>>,
			And<GLTran.batchNbr, Equal<Optional<Batch.batchNbr>>>>> GLTran_CATran_Module_BatNbr;

		public PXSelect<Batch, Where<Batch.module, Equal<Optional<Batch.module>>>> BatchModule;
		public PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Optional<GLTran.curyInfoID>>>, OrderBy<Asc<CurrencyInfo.curyInfoID>>> CurrencyInfo_ID;
		public PXSelectReadonly<Account, Where<Account.accountID, Equal<Required<GLTran.accountID>>>, OrderBy<Asc<Account.accountCD>>> Account_AccountID;
		public PXSelectReadonly<Ledger, Where<Ledger.ledgerID, Equal<Optional<GLTran.ledgerID>>>, OrderBy<Asc<Ledger.ledgerCD>>> Ledger_LedgerID;

		public PXSelectJoin<GLAllocationAccountHistory,
			InnerJoin<Account, On<Account.accountID, Equal<GLAllocationAccountHistory.accountID>>>,
			Where<GLAllocationAccountHistory.batchNbr, Equal<Required<GLAllocationAccountHistory.batchNbr>>,
			And<GLAllocationAccountHistory.module, Equal<Required<GLAllocationAccountHistory.module>>>>>
			BatchAllocHistory;

		public PXSelect<TaxTran,Where<TaxTran.module,Equal<GL.BatchModule.moduleGL>,
							And<TaxTran.module,Equal<Optional<Batch.module>>,
							And<TaxTran.refNbr,Equal<Optional<Batch.batchNbr>>>>>> GL_GLTran_Taxes;

		public PXSelect<FinPeriod, Where<FinPeriod.finPeriodID, Less<Required<FinPeriod.finPeriodID>>>, OrderBy<Desc<FinPeriod.finPeriodID>>> ClosedCheck;
		public PXSelectReadonly<FinPeriod, Where<FinPeriod.finYear, Greater<Optional<FinPeriod.finYear>>>, OrderBy<Asc<FinPeriod.finPeriodID>>> NextFiscalYear;
		public PXSelect<CATran> catran;
		public PXSelect<PMRegister> ProjectDocs;
		public PXSelect<PMTran> ProjectTrans;

		public PXSetup<GLSetup> glsetup;
		Account niacct = null;
		Account reacct = null;

		public bool AutoPost
		{
			get
			{
				return glsetup.Current.AutoPostOption == true;
			}
		}

		public bool AutoRevEntry
		{
			get
			{
				return glsetup.Current.AutoRevEntry == true;
			}
		}

		protected bool _IsIntegrityCheck = false;

		public PostGraph()
		{
			PXDBCurrencyAttribute.SetBaseCalc<GLTran.curyCreditAmt>(GLTran_Module_BatNbr.Cache, null, false);
			PXDBCurrencyAttribute.SetBaseCalc<GLTran.curyDebitAmt>(GLTran_Module_BatNbr.Cache, null, false);

			PXDBCurrencyAttribute.SetBaseCalc<Batch.curyCreditTotal>(BatchModule.Cache, null, false);
			PXDBCurrencyAttribute.SetBaseCalc<Batch.curyDebitTotal>(BatchModule.Cache, null, false);
			PXDBCurrencyAttribute.SetBaseCalc<Batch.curyControlTotal>(BatchModule.Cache, null, false);
		}

		public static void NormalizeAmounts(GLTran tran)
		{
			if ((tran.CuryDebitAmt - tran.CuryCreditAmt) > 0m && (tran.DebitAmt - tran.CreditAmt) < 0m ||
				(tran.CuryDebitAmt - tran.CuryCreditAmt) < 0m && (tran.DebitAmt - tran.CreditAmt) > 0m)
			{
				return;
			}

			if ((tran.CuryDebitAmt - tran.CuryCreditAmt) != decimal.Zero)
			{
				if ((tran.CuryDebitAmt - tran.CuryCreditAmt) < 0m)
				{
					tran.CuryCreditAmt = Math.Abs((decimal)tran.CuryDebitAmt - (decimal)tran.CuryCreditAmt);
					tran.CreditAmt = Math.Abs((decimal)tran.DebitAmt - (decimal)tran.CreditAmt);
					tran.CuryDebitAmt = 0m;
					tran.DebitAmt = 0m;
				}
				else
				{
					tran.CuryDebitAmt = Math.Abs((decimal)tran.CuryDebitAmt - (decimal)tran.CuryCreditAmt);
					tran.DebitAmt = Math.Abs((decimal)tran.DebitAmt - (decimal)tran.CreditAmt);
					tran.CuryCreditAmt = 0m;
					tran.CreditAmt = 0m;
				}
			}
			else
			{
				if ((tran.DebitAmt - tran.CreditAmt) < 0m)
				{
					tran.CuryCreditAmt = Math.Abs((decimal)tran.CuryDebitAmt - (decimal)tran.CuryCreditAmt);
					tran.CreditAmt = Math.Abs((decimal)tran.DebitAmt - (decimal)tran.CreditAmt);
					tran.CuryDebitAmt = 0m;
					tran.DebitAmt = 0m;
				}
				else
				{
					tran.CuryDebitAmt = Math.Abs((decimal)tran.CuryDebitAmt - (decimal)tran.CuryCreditAmt);
					tran.DebitAmt = Math.Abs((decimal)tran.DebitAmt - (decimal)tran.CreditAmt);
					tran.CuryCreditAmt = 0m;
					tran.CreditAmt = 0m;
				}
			}
		}

		//TODO 111 PAnk clear params
		private void UpdateHistory(GLTran tran, Account acct, string PeriodID, bool FinFlag, bool REFlag)
		{
			AcctHist accthist = new AcctHist();
			accthist.AccountID = acct.AccountID;
			accthist.FinPeriodID = PeriodID;
			accthist.LedgerID = tran.LedgerID;
			accthist.BranchID = tran.BranchID;
			accthist.SubID = tran.SubID;
			accthist.CuryID = acct.CuryID;
			accthist = (AcctHist)Caches[typeof(AcctHist)].Insert(accthist);

			if (accthist != null)
			{
				accthist.FinFlag = FinFlag;

				if (tran.CuryDebitAmt != 0m && tran.CuryCreditAmt != 0m || tran.DebitAmt != 0m && tran.CreditAmt != 0m)
				{
					throw new PXException(Messages.TranAmountsDenormalized);
				}

				if (!REFlag)
				{
					accthist.PtdDebit += (tran.DebitAmt != 0m && tran.CreditAmt == 0m) ? (tran.DebitAmt - tran.CreditAmt) : 0m;
					accthist.PtdCredit += (tran.CreditAmt != 0m && tran.DebitAmt == 0m) ? (tran.CreditAmt - tran.DebitAmt) : 0m;

					if (accthist.CuryID != null)
					{
						accthist.CuryPtdDebit += (tran.CuryDebitAmt != 0m && tran.CuryCreditAmt == 0m) ? (tran.CuryDebitAmt - tran.CuryCreditAmt) : 0m;
						accthist.CuryPtdCredit += (tran.CuryCreditAmt != 0m && tran.CuryDebitAmt == 0m) ? (tran.CuryCreditAmt - tran.CuryDebitAmt) : 0m;
					}
					else
					{
						accthist.CuryPtdDebit = accthist.PtdDebit;
						accthist.CuryPtdCredit = accthist.PtdCredit;
					}
				}

				if (acct.Type == AccountType.Income || acct.Type == AccountType.Liability)
				{
					accthist.YtdBalance += (tran.CreditAmt - tran.DebitAmt);
					if (REFlag)
					{
						accthist.BegBalance += (tran.CreditAmt - tran.DebitAmt);
					}
					if (accthist.CuryID != null)
					{
						if (tran.Module == GL.BatchModule.CM && tran.TranType == "REV" && tran.TranClass == acct.Type)
						{
							accthist.PtdRevalued -= (tran.CreditAmt - tran.DebitAmt);
						}
						accthist.CuryYtdBalance += (tran.CuryCreditAmt - tran.CuryDebitAmt);
						if (REFlag)
						{
							accthist.CuryBegBalance += (tran.CuryCreditAmt - tran.CuryDebitAmt);
						}
					}
					else
					{
						accthist.CuryYtdBalance = accthist.YtdBalance;
						if (REFlag)
						{
							accthist.CuryBegBalance = accthist.BegBalance;
						}
					}
				}
				else
				{
					accthist.YtdBalance += (tran.DebitAmt - tran.CreditAmt);
					if (REFlag)
					{
						accthist.BegBalance += (tran.DebitAmt - tran.CreditAmt);
					}
					if (accthist.CuryID != null)
					{
						if (tran.Module == GL.BatchModule.CM && tran.TranType == "REV" && tran.TranClass == acct.Type)
						{
							accthist.PtdRevalued -= (tran.DebitAmt - tran.CreditAmt);
						}
						accthist.CuryYtdBalance += (tran.CuryDebitAmt - tran.CuryCreditAmt);
						if (REFlag)
						{
							accthist.CuryBegBalance += (tran.CuryDebitAmt - tran.CuryCreditAmt);
						}
					}
					else
					{
						accthist.CuryYtdBalance = accthist.YtdBalance;
						if (REFlag)
						{
							accthist.CuryBegBalance = accthist.BegBalance;
						}
					}
				}
			}
		}

		private void UpdateAllocationBalance(Batch b)
		{
			foreach (PXResult<GLAllocationAccountHistory, Account> res in this.BatchAllocHistory.Select(b.BatchNbr, b.Module))
			{
				GLAllocationAccountHistory iAH = res;
				Account acct = res;
				if (Math.Round(Math.Abs(iAH.AllocatedAmount ?? 0.0m), 4) < 0.00005m) continue;
				AcctHist accthist = new AcctHist();
				accthist.AccountID = iAH.AccountID;
				accthist.FinPeriodID = b.TranPeriodID;
				accthist.LedgerID = b.LedgerID;
				accthist.SubID = iAH.SubID;
				accthist.CuryID = acct.CuryID;
				accthist.BranchID = iAH.BranchID;

				if (Caches[typeof(Ledger)].Current == null)
				{
					Ledger ledger = PXSelectReadonly<Ledger, Where<Ledger.ledgerID, Equal<Required<Batch.ledgerID>>>>.Select(this, b.LedgerID);
					if (ledger != null)
					{
						accthist.BalanceType = ledger.BalanceType;
					}
				}

				accthist = (AcctHist)Caches[typeof(AcctHist)].Insert(accthist);
				if (accthist != null)
				{
					accthist.AllocPtdBalance = (accthist.AllocPtdBalance ?? 0.0m) + iAH.AllocatedAmount;
					accthist.AllocBegBalance = (accthist.AllocBegBalance ?? 0.0m) + iAH.PriorPeriodsAllocAmount;
				}
			}
		}

		private bool UpdateConsolidationBalance(Batch b)
		{
			bool anychanges = false;
			if (b.BatchType == BatchTypeCode.Consolidation)
			{
				GLConsolBatch cb = PXSelect<GLConsolBatch,
					Where<GLConsolBatch.batchNbr, Equal<Required<GLConsolBatch.batchNbr>>>>
					.Select(this, b.BatchNbr);
				if (cb == null && b.AutoReverseCopy == true)
				{
					cb = PXSelect<GLConsolBatch,
						Where<GLConsolBatch.batchNbr, Equal<Required<GLConsolBatch.batchNbr>>>>
						.Select(this, b.OrigBatchNbr);
				}
				if (cb != null)
				{
					PXCache cache = Caches[typeof(ConsolHist)];
					foreach (AcctHist hist in Caches[typeof(AcctHist)].Inserted)
					{
						if (hist.AccountID == glsetup.Current.YtdNetIncAccountID
							|| hist.AccountID == glsetup.Current.RetEarnAccountID
							&& hist.FinPeriodID != b.FinPeriodID)
						{
							continue;
						}
						ConsolHist ch = new ConsolHist();
						ch.SetupID = cb.SetupID;
						ch.BranchID = hist.BranchID;
						ch.LedgerID = hist.LedgerID;
						ch.AccountID = hist.AccountID;
						ch.SubID = hist.SubID;
						ch.FinPeriodID = hist.FinPeriodID;
						ch = (ConsolHist)cache.Insert(ch);
						if (ch != null)
						{
							ch.PtdCredit += hist.FinPtdCredit;
							ch.PtdDebit += hist.FinPtdDebit;
							anychanges = true;
						}
					}
				}
			}
			return anychanges;
		}

		private void DecimalSwap(ref decimal? d1, ref decimal? d2)
		{
			decimal? swap = d1;
			d1 = d2;
			d2 = swap;
		}

		public virtual Batch ReverseBatchProc(Batch b)
		{
			Batch copy = PXCache<Batch>.CreateCopy(b);
			{
				using (PXTransactionScope ts = new PXTransactionScope())
				{
					copy.OrigBatchNbr = copy.BatchNbr;
					copy.OrigModule = copy.Module;
					copy.BatchNbr = null;
					copy.NoteID = null;
					copy.ReverseCount = 0;
					try
					{
						copy.FinPeriodID = FinPeriodIDAttribute.NextPeriod(this, copy.FinPeriodID);
					}
					catch(PXFinPeriodException)
					{
						throw new PXFinPeriodException(Messages.NoOpenPeriodAfter, FinPeriodIDFormattingAttribute.FormatForError(copy.FinPeriodID));
					}
					if (copy.FinPeriodID == null)
					{
						throw new PXException(Messages.NoOpenPeriod);
					}
					copy.DateEntered = FinPeriodIDAttribute.PeriodStartDate(this, copy.FinPeriodID);
					copy.AutoReverse = false;
					copy.AutoReverseCopy = true;
					copy.CuryInfoID = null;

					CurrencyInfo info = (CurrencyInfo)PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Required<CurrencyInfo.curyInfoID>>>>.Select(this, b.CuryInfoID);

					if (info != null)
					{
						CurrencyInfo infocopy = PXCache<CurrencyInfo>.CreateCopy(info);
						infocopy.CuryInfoID = -1;
						copy.CuryInfoID = -1;
						infocopy = (CurrencyInfo)CurrencyInfo_ID.Cache.Insert(infocopy);
					}

					copy.Posted = false;
					copy.Status = BatchStatus.Unposted;
					copy = (Batch)Caches[typeof(Batch)].Insert(copy);
					PXNoteAttribute.CopyNoteAndFiles(Caches[typeof(Batch)], b, Caches[typeof(Batch)], copy);
					foreach (GLTran tran in GLTran_Module_BatNbr.Select(b.Module, b.BatchNbr))
					{
						GLTran trancopy = PXCache<GLTran>.CreateCopy(tran);
						trancopy.OrigBatchNbr = trancopy.BatchNbr;
						trancopy.OrigModule = trancopy.Module;
						trancopy.BatchNbr = null;
						trancopy.CuryInfoID = copy.CuryInfoID;
						trancopy.CATranID = null;
						trancopy.TranID = null;
						trancopy.Posted = false;
						trancopy.TranDate = copy.DateEntered;
						trancopy.FinPeriodID = copy.FinPeriodID;
						trancopy.TranPeriodID = copy.TranPeriodID;

						{
							Decimal? amount = trancopy.CuryDebitAmt;
							trancopy.CuryDebitAmt = trancopy.CuryCreditAmt;
							trancopy.CuryCreditAmt = amount;
						}

						{
							Decimal? amount = trancopy.DebitAmt;
							trancopy.DebitAmt = trancopy.CreditAmt;
							trancopy.CreditAmt = amount;
						}

						GLTran insertedRow = trancopy = (GLTran)Caches[typeof(GLTran)].Insert(trancopy);
						Caches[typeof(GLTran)].SetValueExt<GLTran.taxID>(insertedRow, trancopy.TaxID);
					}

					Caches[typeof(Batch)].Persist(PXDBOperation.Insert);

					foreach (GLTran tran in Caches[typeof(GLTran)].Inserted)
					{
						foreach (Batch batch in Caches[typeof(Batch)].Cached)
						{
							if (object.Equals(tran.OrigBatchNbr, batch.OrigBatchNbr))
							{
								tran.BatchNbr = batch.BatchNbr;
								tran.CuryInfoID = batch.CuryInfoID;
								break;
							}
						}

						CATran catran = GLCashTranIDAttribute.DefaultValues(Caches[typeof(GLTran)], tran);
						if (catran != null)
						{
							catran = (CATran)Caches[typeof(CATran)].Insert(catran);
							Caches[typeof(CATran)].PersistInserted(catran);
							long id = Convert.ToInt64(PXDatabase.SelectIdentity());

							tran.CATranID = id;
							catran.TranID = id;

							Caches[typeof(CATran)].Normalize();
						}
					}

					Caches[typeof(GLTran)].Persist(PXDBOperation.Insert);
					Caches[typeof(CADailySummary)].Persist(PXDBOperation.Insert);

					ts.Complete(this);
				}
				Caches[typeof(Batch)].Persisted(false);
				Caches[typeof(GLTran)].Persisted(false);
				Caches[typeof(CATran)].Persisted(false);
				Caches[typeof(CADailySummary)].Persisted(false);

			}
			return copy;
		}

		private bool IsClosed(string PeriodID)
		{
			FinPeriod fp = ClosedCheck.Select(PeriodID);
			if (fp != null)
			{
				return (bool)fp.Closed;
			}
			else
			{
				return false;
			}
		}

		public virtual void IntegrityCheckProc(Ledger ledger)
		{
			this._IsIntegrityCheck = true;

			string minPeriod = "190001";

			GLHistory maxHist = (GLHistory)PXSelectGroupBy<GLHistory, Where<GLHistory.ledgerID, Equal<Required<GLHistory.ledgerID>>, And<GLHistory.detDeleted, Equal<True>>>, Aggregate<Max<GLHistory.finPeriodID>>>.Select(this, ledger.LedgerID);

			if (maxHist != null && maxHist.FinPeriodID != null)
			{
				minPeriod = maxHist.FinPeriodID;
			}

			this.RowInserting.AddHandler<AcctHist>((sender, e) => { ((AcctHist)e.Row).YearClosed = ((AcctHist)e.Row).AccountID == this.glsetup.Current.RetEarnAccountID && ((AcctHist)e.Row).FinPeriodID.Substring(4, 2) == "01"; });

			//foreach (Batch batch in PXSelect<Batch, Where<Batch.posted, Equal<True>>>.Select(this))
			foreach (FinPeriod period in PXSelectReadonly<FinPeriod, Where<FinPeriod.finPeriodID, Greater<Required<FinPeriod.finPeriodID>>>>.Select(this, minPeriod))
			{
				GLTran_CATran_Module_BatNbr.View.WhereNew<Where<GLTran.finPeriodID, Equal<Required<GLTran.finPeriodID>>, And<GLTran.ledgerID, Equal<Required<GLTran.ledgerID>>, And<GLTran.posted, Equal<True>>>>>();
				UpdateHistoryProc(period.FinPeriodID, ledger.LedgerID);
			}

			using (new PXConnectionScope())
			{
				using (PXTransactionScope ts = new PXTransactionScope())
				{
					PXDatabase.Delete<GLHistory>(
						new PXDataFieldRestrict("LedgerID", PXDbType.Int, 4, ledger.LedgerID, PXComp.EQ),
						new PXDataFieldRestrict("FinPeriodID", PXDbType.Char, 6, minPeriod, PXComp.GT)
						);

					Caches[typeof(AcctHist)].Persist(PXDBOperation.Insert);

					ts.Complete(this);
				}
				Caches[typeof(AcctHist)].Persisted(false);
			}
		}

		public virtual void UpdateHistoryProc(params object[] pars)
		{
			GLTran_CATran_Module_BatNbr.View.Clear();
			foreach (PXResult<GLTran, CATran, CurrencyInfo, Account, Ledger> res in GLTran_CATran_Module_BatNbr.Select(pars))
			{
				GLTran tran = res;
				CATran cashtran = (CATran)res;

				Account acct = Account_AccountID.Current = res;
				Ledger ledger = Ledger_LedgerID.Current = res;

				Account_AccountID.Cache.SetStatus(acct, PXEntryStatus.Held);

				if (acct.AccountID == null)
				{
					throw new PXException(Messages.AccountMissing);
				}

				if (ledger.LedgerID == null)
				{
					throw new PXException(Messages.LedgerMissing);
				}

				if (reacct == null)
				{
					reacct = Account_AccountID.Select(glsetup.Current.RetEarnAccountID);
					if (reacct == null)
					{
						throw new PXException(Messages.InvalidRetEarnings);
					}
				}

				if (niacct == null)
				{
					niacct = Account_AccountID.Select(glsetup.Current.YtdNetIncAccountID);
					if (niacct == null)
					{
						throw new PXException(Messages.InvalidNetIncome);
					}
				}

				//Incomes are treated like Liabilities, Expenses like Assets in statistical ledgers
				if ((acct.Type == AccountType.Income || acct.Type == AccountType.Expense) && ledger.BalanceType != LedgerBalanceType.Statistical)
				{
					if (object.Equals(tran.AccountID, glsetup.Current.YtdNetIncAccountID))
					{
						throw new PXException(Messages.NoPostNetIncome);
					}

					GLTran zeroTran = PXCache<GLTran>.CreateCopy(tran);
					zeroTran.CuryDebitAmt = 0m;
					zeroTran.CuryCreditAmt = 0m;
					zeroTran.DebitAmt = 0m;
					zeroTran.CreditAmt = 0m;


					UpdateHistory(tran, acct, tran.FinPeriodID, true, false);
                    UpdateHistory(tran, acct, tran.TranPeriodID, false, false);
					if (ledger.BalanceType == LedgerBalanceType.Actual || ledger.BalanceType == LedgerBalanceType.Report)
					{
						UpdateHistory(zeroTran, reacct, FinPeriodIDAttribute.GetFirstFinPeriodIDOfYear(tran.FinPeriodID), true, false);
						UpdateHistory(zeroTran, reacct, FinPeriodIDAttribute.GetFirstFinPeriodIDOfYear(tran.TranPeriodID), false, false);

						var firstFinPeriodIDOfNextYear = FinPeriodIDAttribute.GetFirstFinPeriodIDOfYear(FinPeriodIDAttribute.GetNextYearID(tran.FinPeriodID));
						UpdateHistory(tran, reacct, firstFinPeriodIDOfNextYear, true, true);

						var firstTranPeriodIDOfNextYear = FinPeriodIDAttribute.GetFirstFinPeriodIDOfYear(FinPeriodIDAttribute.GetNextYearID(tran.TranPeriodID));
						UpdateHistory(tran, reacct, firstTranPeriodIDOfNextYear, false, true);


						UpdateHistory(tran, niacct, tran.FinPeriodID, true, false);
						UpdateHistory(tran, niacct, tran.TranPeriodID, false, false);

						UpdateHistory(zeroTran, niacct, firstFinPeriodIDOfNextYear, true, false);
						UpdateHistory(zeroTran, niacct, firstTranPeriodIDOfNextYear, false, false);
					}
				}
				else
				{
					UpdateHistory(tran, acct, tran.FinPeriodID, true, false);
					UpdateHistory(tran, acct, tran.TranPeriodID, false, false);

					if ((ledger.BalanceType == LedgerBalanceType.Actual || ledger.BalanceType == LedgerBalanceType.Report) && acct.AccountID == reacct.AccountID)
					{
						GLTran retran = PXCache<GLTran>.CreateCopy(tran);
						retran.CuryDebitAmt = 0m;
						retran.CuryCreditAmt = 0m;
						retran.DebitAmt = 0m;
						retran.CreditAmt = 0m;

						UpdateHistory(retran, niacct, FinPeriodIDAttribute.GetFirstFinPeriodIDOfYear(tran.FinPeriodID), true, false);
						UpdateHistory(retran, reacct, FinPeriodIDAttribute.GetFirstFinPeriodIDOfYear(tran.FinPeriodID), true, false);
					}
				}

				if (_IsIntegrityCheck == false)
				{
					tran.Posted = true;
					GLTran_Module_BatNbr.Cache.SetStatus(tran, PXEntryStatus.Updated);
					if (cashtran.TranID != null)
					{
						cashtran = PXCache<CATran>.CreateCopy(cashtran);
						cashtran.Released = true;
						cashtran.Posted = true;
						cashtran.BatchNbr = tran.BatchNbr;
						catran.Update(cashtran);
					}
				}
			}
		}

		public static bool GetAccountMapping(PXGraph graph, Batch batch, GLTran tran, out BranchAcctMapFrom mapfrom, out BranchAcctMapTo mapto)
		{
			mapfrom = null;
			mapto = null;

			var batchCache = graph.Caches[typeof(Batch)];

			if (batch.BranchID == tran.BranchID || tran.BranchID == null || tran.AccountID == null) return true;

			JournalEntry.CheckBatchBranchHasLedger(batchCache, batch);

			Ledger ledger = (Ledger)PXSelectorAttribute.Select<Batch.ledgerID>(batchCache, batch, batch.LedgerID);

			if (ledger == null)
			{
				throw new PXException(Messages.LedgerMissing);
			}

			if (ledger.BalanceType != LedgerBalanceType.Actual) return true;

			Branch detailbranch = (Branch)PXSelectorAttribute.Select<GLTran.branchID>(graph.Caches[typeof(GLTran)], tran, tran.BranchID);

			if (detailbranch == null)
			{
				throw new PXException(Messages.BranchMissing);
			}

			if (detailbranch.LedgerID == batch.LedgerID && ledger.PostInterCompany == false) return true;
			if (!PXAccess.FeatureInstalled<FeaturesSet.interBranch>()) return false;

			Account account = (Account)PXSelectorAttribute.Select<GLTran.accountID>(graph.Caches[typeof(GLTran)], tran, tran.AccountID);

			if (account == null)
			{
				throw new PXException(Messages.AccountMissing);
			}

			mapfrom = PXSelectReadonly<BranchAcctMapFrom, Where<BranchAcctMapFrom.fromBranchID, Equal<Required<Batch.branchID>>, And<BranchAcctMapFrom.toBranchID, Equal<Required<GLTran.branchID>>, And<Required<Account.accountCD>, Between<BranchAcctMapFrom.fromAccountCD, BranchAcctMapFrom.toAccountCD>>>>>.Select(graph, batch.BranchID, tran.BranchID, account.AccountCD);
			if (mapfrom == null)
			{
				mapfrom = PXSelectReadonly<BranchAcctMapFrom, Where<BranchAcctMapFrom.fromBranchID, Equal<Required<Batch.branchID>>, And<BranchAcctMapFrom.toBranchID, IsNull, And<Required<Account.accountCD>, Between<BranchAcctMapFrom.fromAccountCD, BranchAcctMapFrom.toAccountCD>>>>>.Select(graph, batch.BranchID, account.AccountCD);

				if (mapfrom == null || mapfrom.MapSubID == null)
				{
					return false;
				}
			}

			mapto = PXSelectReadonly<BranchAcctMapTo, Where<BranchAcctMapTo.toBranchID, Equal<Required<Batch.branchID>>, And<BranchAcctMapTo.fromBranchID, Equal<Required<GLTran.branchID>>, And<Required<Account.accountCD>, Between<BranchAcctMapTo.fromAccountCD, BranchAcctMapTo.toAccountCD>>>>>.Select(graph, batch.BranchID, tran.BranchID, account.AccountCD);
			if (mapto == null)
			{
				mapto = PXSelectReadonly<BranchAcctMapTo, Where<BranchAcctMapTo.toBranchID, Equal<Required<Batch.branchID>>, And<BranchAcctMapTo.fromBranchID, IsNull, And<Required<Account.accountCD>, Between<BranchAcctMapTo.fromAccountCD, BranchAcctMapTo.toAccountCD>>>>>.Select(graph, batch.BranchID, account.AccountCD);

				if (mapto == null || mapto.MapSubID == null)
				{
					return false;
				}
			}
			return true;
		}

		protected virtual Batch CreateInterCompany(Batch b)
		{
			this.Caches[typeof(Batch)].Current = b;

			PXRowInserting inserting_handler = new PXRowInserting((sender, e) =>
			{
				GLTran tran = (GLTran)e.Row;
				object BranchID = tran.BranchID;
				object AccountID = tran.AccountID;
				object SubID = tran.SubID;
				object CuryInfoID = tran.CuryInfoID;
				object RefNbr = tran.RefNbr;

				if (tran.IsInterCompany == true)
				{
					e.Cancel = SummaryPostingController.TryAggregateToFirstSuitableTran(sender, (GLTran)e.Row,
						PXSelect<GLTran,
							Where<GLTran.module, Equal<Current<Batch.module>>,
							And<GLTran.batchNbr, Equal<Current<Batch.batchNbr>>,
							And<GLTran.refNbr, Equal<Required<GLTran.refNbr>>,
							And<GLTran.curyInfoID, Equal<Required<GLTran.curyInfoID>>,
							And<GLTran.branchID, Equal<Required<GLTran.branchID>>,
							And<GLTran.accountID, Equal<Required<GLTran.accountID>>,
							And<GLTran.subID, Equal<Required<GLTran.subID>>,
							And<GLTran.isInterCompany, Equal<True>>>>>>>>>>.Select(this, RefNbr, CuryInfoID, BranchID, AccountID, SubID).RowCast<GLTran>());
					if (e.Cancel == false)
					{
						PostGraph.NormalizeAmounts(tran);
					}
				}
				else
				{
					PostGraph.NormalizeAmounts(tran);
				}

				if (e.Cancel == false)
				{
					e.Cancel = (tran.CuryDebitAmt == 0 &&
								tran.CuryCreditAmt == 0 &&
								tran.DebitAmt == 0 &&
								tran.CreditAmt == 0 &&
								tran.ZeroPost != true);
				}
			});

			PXRowInserted inserted_handler = new PXRowInserted((sender, e) =>
			{
				b.CuryCreditTotal += ((GLTran)e.Row).CuryCreditAmt;
				b.CuryDebitTotal += ((GLTran)e.Row).CuryDebitAmt;
				b.CuryControlTotal += ((GLTran)e.Row).CuryDebitAmt;
				b.CreditTotal += ((GLTran)e.Row).CreditAmt;
				b.DebitTotal += ((GLTran)e.Row).DebitAmt;
				b.ControlTotal += ((GLTran)e.Row).DebitAmt;
			});
			PXRowUpdated updated_handler = new PXRowUpdated((sender, e) =>
			{
				b.CuryCreditTotal += ((GLTran)e.Row).CuryCreditAmt;
				b.CuryDebitTotal += ((GLTran)e.Row).CuryDebitAmt;
				b.CuryControlTotal += ((GLTran)e.Row).CuryDebitAmt;
				b.CreditTotal += ((GLTran)e.Row).CreditAmt;
				b.DebitTotal += ((GLTran)e.Row).DebitAmt;
				b.ControlTotal += ((GLTran)e.Row).DebitAmt;

				b.CuryCreditTotal -= ((GLTran)e.OldRow).CuryCreditAmt;
				b.CuryDebitTotal -= ((GLTran)e.OldRow).CuryDebitAmt;
				b.CuryControlTotal -= ((GLTran)e.OldRow).CuryDebitAmt;
				b.CreditTotal -= ((GLTran)e.OldRow).CreditAmt;
				b.DebitTotal -= ((GLTran)e.OldRow).DebitAmt;
				b.ControlTotal -= ((GLTran)e.OldRow).DebitAmt;
			});
			PXRowDeleted deleted_handler = new PXRowDeleted((sender, e) =>
			{
				b.CuryCreditTotal -= ((GLTran)e.Row).CuryCreditAmt;
				b.CuryDebitTotal -= ((GLTran)e.Row).CuryDebitAmt;
				b.CuryControlTotal -= ((GLTran)e.Row).CuryDebitAmt;
				b.CreditTotal -= ((GLTran)e.Row).CreditAmt;
				b.DebitTotal -= ((GLTran)e.Row).DebitAmt;
				b.ControlTotal -= ((GLTran)e.Row).DebitAmt;
			});


			this.RowInserting.AddHandler<GLTran>(inserting_handler);
			this.RowInserted.AddHandler<GLTran>(inserted_handler);
			this.RowUpdated.AddHandler<GLTran>(updated_handler);
			this.RowDeleted.AddHandler<GLTran>(deleted_handler);

			bool isex = false;
			foreach (PXResult<GLTran, Account, Branch, Ledger> res in PXSelectJoin<GLTran, LeftJoin<Account, On<Account.accountID, Equal<GLTran.accountID>>, LeftJoin<Branch, On<Branch.branchID, Equal<GLTran.branchID>>, LeftJoin<Ledger, On<Ledger.ledgerID, Equal<Optional<Batch.ledgerID>>>>>>, Where<GLTran.module, Equal<Optional<Batch.module>>, And<GLTran.batchNbr, Equal<Optional<Batch.batchNbr>>, And<GLTran.branchID, NotEqual<Optional<Batch.branchID>>, And<Ledger.balanceType, Equal<LedgerBalanceType.actual>, And<Where<Branch.ledgerID, NotEqual<Optional<Batch.ledgerID>>, Or<Ledger.postInterCompany, Equal<True>>>>>>>>>.SelectMultiBound(this, new object[] { b }))
			{
				GLTran tran = res;
				Account acct = res;
				Branch branchto = res;
				Ledger ledger = res;

				if (acct.AccountID == null)
				{
					throw new PXException(Messages.AccountMissing);
				}

				if (ledger.LedgerID == null)
				{
					throw new PXException(Messages.LedgerMissing);
				}

				if (branchto.BranchID == null)
				{
					throw new PXException(Messages.BranchMissing);
				}

				PXSelectorAttribute.StoreCached<GLTran.accountID>(this.Caches[typeof(GLTran)], tran, acct);
				PXSelectorAttribute.StoreCached<GLTran.branchID>(this.Caches[typeof(GLTran)], tran, branchto);

				BranchAcctMapFrom mapfrom;
				BranchAcctMapTo mapto;
				if (!GetAccountMapping(this, b, tran, out mapfrom, out mapto))
				{
					Branch branchfrom = PXSelect<Branch, Where<Branch.branchID, Equal<Optional<Batch.branchID>>>>.SelectSingleBound(this, new object[] { b });
					throw new PXException(Messages.BrachAcctMapMissing, (branchfrom != null ? branchfrom.BranchCD : "Undefined"), (branchto != null ? branchto.BranchCD : "Undefined"));
				}

				GLTran copy = PXCache<GLTran>.CreateCopy(tran);
				copy.AccountID = mapfrom.MapAccountID;
				copy.SubID = mapfrom.MapSubID;
				copy.BranchID = b.BranchID;
				copy.LedgerID = b.LedgerID;
				copy.TranLineNbr = null;
				copy.LineNbr = null;
				copy.TranID = null;
				copy.CATranID = null;
				copy.ProjectID = null;
				copy.TaskID = null;
				copy.IsInterCompany = true;
				copy.TranDesc = PXMessages.LocalizeFormatNoPrefix(Messages.InterCoTranDesc, Caches[typeof(Batch)].GetValueExt<Batch.branchID>(b));

				Caches[typeof(GLTran)].Insert(copy);

				copy = PXCache<GLTran>.CreateCopy(tran);
				copy.AccountID = mapto.MapAccountID;
				copy.SubID = mapto.MapSubID;
				copy.LedgerID = branchto.LedgerID;
				copy.TranLineNbr = null;
				copy.LineNbr = null;
				copy.TranID = null;
				copy.CATranID = null;
				copy.ProjectID = null;
				copy.TaskID = null;
				copy.IsInterCompany = true;
				copy.TranDesc = PXMessages.LocalizeFormatNoPrefix(Messages.InterCoTranDesc, branchto.BranchCD);

				decimal? amt = copy.CuryCreditAmt;
				copy.CuryCreditAmt = copy.CuryDebitAmt;
				copy.CuryDebitAmt = amt;

				amt = copy.CreditAmt;
				copy.CreditAmt = copy.DebitAmt;
				copy.DebitAmt = amt;

				Caches[typeof(GLTran)].Insert(copy);

				copy = PXCache<GLTran>.CreateCopy(tran);
				copy.LedgerID = branchto.LedgerID;

				Caches[typeof(GLTran)].Update(copy);

				isex = true;
			}

			this.RowInserting.RemoveHandler<GLTran>(inserting_handler);
			this.RowInserted.RemoveHandler<GLTran>(inserted_handler);
			this.RowUpdated.RemoveHandler<GLTran>(updated_handler);
			this.RowDeleted.RemoveHandler<GLTran>(deleted_handler);

			Caches[typeof(GLTran)].Persist(PXDBOperation.Insert);
			Caches[typeof(GLTran)].Persist(PXDBOperation.Update);

			this.SelectTimeStamp();

			Caches[typeof(GLTran)].Persisted(false);

			return (isex ? b : null);
		}


		public virtual void PostBatchProc(Batch b)
		{
			if (RunningFlagScope<GLHistoryValidate>.IsRunning)
				throw new PXSetPropertyException(Messages.GLHistoryValidationRunning, PXErrorLevel.Warning);

			using (new RunningFlagScope<PostGraph>())
			{
				PostBatchProc(b, true);
			}
		}

		public virtual void PostBatchProc(Batch b, bool createintercompany)
		{
			if (b.Status != BatchStatus.Unposted || b.Released == false)
			{
				throw new PXException(Messages.BatchStatusInvalid);
			}

			ValidateBatchFinPeriod(b);

			if (glsetup.Current.AutoRevOption == "P")
			{
				PostGraph pg = PXGraph.CreateInstance<PostGraph>();

				foreach (PXResult<Batch, BatchCopy> res in PXSelectJoin<Batch, LeftJoin<BatchCopy, On<BatchCopy.origModule, Equal<Batch.module>, And<BatchCopy.origBatchNbr, Equal<Batch.batchNbr>, And<BatchCopy.autoReverseCopy, Equal<True>>>>>, Where<Batch.module, Equal<Required<Batch.module>>, And<Batch.batchNbr, Equal<Required<Batch.batchNbr>>, And<Batch.autoReverse, Equal<True>, And<Batch.released, Equal<True>, And<BatchCopy.origBatchNbr, IsNull>>>>>>.Select(pg, b.Module, b.BatchNbr))
				{
					pg.Clear();

					Batch copy = pg.ReverseBatchProc((Batch)res);
					pg.ReleaseBatchProc(copy);

					if (glsetup.Current.AutoPostOption == true)
					{
						pg.PostBatchProc(copy);
					}
				}
			}

			if (createintercompany)
			{
				PostGraph pg = PXGraph.CreateInstance<PostGraph>();
				pg.Clear();
				using (PXTransactionScope ts = new PXTransactionScope())
				{
					if (pg.CreateInterCompany(b) != null)
					{
						pg.PostBatchProc(b, false);
						ts.Complete();
						return;
					}

					// This ts.Complete() for apparently empty transaction is required to prevent rollback of an outer transaction.
					// Details on how nested PXTransactionScopes work can be found here:
					// https://wiki.acumatica.com/display/AD/Nested+Transaction+Scopes
					ts.Complete();
				}
			}

			this.UpdateHistoryProc(b.Module, b.BatchNbr);
			this.UpdateAllocationBalance(b);
			bool isconsol = this.UpdateConsolidationBalance(b);

			this.UpdateProjectBalance(b);

			b.Posted = true;
			b.Status = BatchStatus.Posted;
			BatchModule.Update(b);

			using (PXTransactionScope ts = new PXTransactionScope())
			{
				BatchModule.Cache.Persist(PXDBOperation.Update);
					GLTran_Module_BatNbr.Cache.Persist(PXDBOperation.Update);

				catran.Cache.Persist(PXDBOperation.Update);
				Caches[typeof(AcctHist)].Persist(PXDBOperation.Insert);
				Caches[typeof(PMHistory)].Persist(PXDBOperation.Insert);
				Caches[typeof(CADailySummary)].Persist(PXDBOperation.Insert);


				if (isconsol)
				{
					Caches[typeof(ConsolHist)].Persist(PXDBOperation.Insert);
				}

				ts.Complete(this);
			}
			BatchModule.Cache.Persisted(false);
			GLTran_Module_BatNbr.Cache.Persisted(false);
			catran.Cache.Persisted(false);
			Caches[typeof(AcctHist)].Persisted(false);
			Caches[typeof(CADailySummary)].Persisted(false);

			if (isconsol)
			{
				Caches[typeof(ConsolHist)].Persisted(false);
			}
			BatchModule.Cache.RestoreCopy(b, BatchModule.Current);
		}

		private void ValidateBatchFinPeriod(Batch batch)
		{
			var cache = Caches[typeof(Batch)];

			foreach (PXEventSubscriberAttribute attr in cache.GetAttributes<Batch.finPeriodID>())
			{
				if (attr is OpenPeriodAttribute)
				{
					((OpenPeriodAttribute) attr).IsValidPeriod(cache, batch, batch.FinPeriodID);
				}
			}
		}

		public virtual void ReleaseBatchProc(Batch b, bool unholdBatch = false)
		{
			Dictionary<string, PMTask> tasksToAutoAllocate = new Dictionary<string, PMTask>();
			using (PXTransactionScope ts = new PXTransactionScope())
			{
				if (unholdBatch)
				{
					b.Hold = false;
				}

				if (string.IsNullOrEmpty(b.OrigBatchNbr) && (b.Status != BatchStatus.Balanced || (bool)b.Released))
				{
					throw new PXException(Messages.BatchStatusInvalid);
				}

				ValidateBatchFinPeriod(b);

				b.CreditTotal = 0.0m;
				b.DebitTotal = 0.0m;
				b.CuryCreditTotal = 0.0m;
				b.CuryDebitTotal = 0.0m;

				CurrencyInfo info = null;

				Ledger ledger = null;
				bool isCurrencyTB = false;
				bool createTaxTrans = (PXAccess.FeatureInstalled<FeaturesSet.taxEntryFromGL>() && b.CreateTaxTrans == true);
				TaxTranCreationProcessor taxCreationProc = null;
				if (createTaxTrans)
					taxCreationProc = new TaxTranCreationProcessor(this.GLTran_CATran_Module_BatNbr.Cache, this.GL_GLTran_Taxes.Cache, (b.SkipTaxValidation ?? false));
				foreach (PXResult<GLTran, CATran, CurrencyInfo, Account, Ledger> res in GLTran_CATran_Module_BatNbr.Select(b.Module, b.BatchNbr))
				{
					GLTran tran = res;
					CATran cashtran = res;
					Account acct = res;
						
					ledger = res;
					info = res;

					if (acct.AccountID == null)
					{
						throw new PXException(Messages.AccountMissing);
					}

					if (ledger.LedgerID == null)
					{
						throw new PXException(Messages.LedgerMissing);
					}

					PXSelectorAttribute.StoreCached<GLTran.accountID>(this.Caches[typeof(GLTran)], tran, acct);

					BranchAcctMapFrom mapfrom;
					BranchAcctMapTo mapto;
					if (!PostGraph.GetAccountMapping(this, b, tran, out mapfrom, out mapto))
					{
						Branch branchfrom = (Branch)PXSelectorAttribute.Select<Batch.branchID>(BatchModule.Cache, b, b.BranchID);
						Branch branchto = (Branch)PXSelectorAttribute.Select<GLTran.branchID>(GLTran_CATran_Module_BatNbr.Cache, tran, tran.BranchID);

						throw new PXException(Messages.BrachAcctMapMissing, (branchfrom != null ? branchfrom.BranchCD : "Undefined"), (branchto != null ? branchto.BranchCD : "Undefined"));
					}

					if (tran.CuryDebitAmt != 0m && tran.CuryCreditAmt != 0m || tran.DebitAmt != 0m && tran.CreditAmt != 0m)
					{
						throw new PXException(Messages.TranAmountsDenormalized);
					}

					if (createTaxTrans) 
					{
						taxCreationProc.AddToDocuments(res);
					}


					if (b.AutoReverseCopy == true && this.AutoRevEntry)
					{
						if (tran.CuryDebitAmt != 0m && tran.CuryCreditAmt == 0m || tran.DebitAmt != 0m && tran.CreditAmt == 0m)
						{
							tran.CuryCreditAmt = -1m * tran.CuryDebitAmt;
							tran.CreditAmt = -1m * tran.DebitAmt;
							tran.CuryDebitAmt = 0m;
							tran.DebitAmt = 0m;
						}
						else if (tran.CuryCreditAmt != 0m && tran.CuryDebitAmt == 0m || tran.CreditAmt != 0m && tran.DebitAmt == 0m)
						{
							tran.CuryDebitAmt = -1m * tran.CuryCreditAmt;
							tran.DebitAmt = -1m * tran.CreditAmt;
							tran.CuryCreditAmt = 0m;
							tran.CreditAmt = 0m;
						}
					}

					b.CreditTotal += tran.CreditAmt;
					b.DebitTotal += tran.DebitAmt;
					b.CuryCreditTotal += tran.CuryCreditAmt;
					b.CuryDebitTotal += tran.CuryDebitAmt;
					b.CuryControlTotal = b.CuryDebitTotal;
					b.ControlTotal = b.DebitTotal;

					tran.Released = true;
					GLTran_Module_BatNbr.Cache.Update(tran);

					if (cashtran.TranID != null)
					{
						cashtran = PXCache<CATran>.CreateCopy(cashtran);
						cashtran.Released = true;
						cashtran.BatchNbr = tran.BatchNbr;
						cashtran.Hold = b.Hold;
						catran.Update(cashtran);
					}

					if (tran.IsReclassReverse == true)
					{
						PXUpdate<Set<GLTran.reclassified, True,
								Set<GLTran.reclassifiedWithTranDate, Required<GLTran.reclassifiedWithTranDate>>>,
							GLTran,
							Where<GLTran.module, Equal<Required<GLTran.module>>,
								And<GLTran.batchNbr, Equal<Required<GLTran.batchNbr>>,
								And<GLTran.lineNbr, Equal<Required<GLTran.lineNbr>>>>>>
							.Update(this,
								tran.TranDate,
								tran.OrigModule,
								tran.OrigBatchNbr,
								tran.OrigLineNbr);
					}

					isCurrencyTB |= (ledger.BalanceType == LedgerBalanceType.Actual && b.BatchType == BatchTypeCode.TrialBalance && info.CuryID != ledger.BaseCuryID);
				}

				GLTran newtran = null;

				if (ledger != null && ledger.BalanceType != LedgerBalanceType.Statistical)
				{
					if (Math.Abs(Math.Round((decimal)(b.CuryDebitTotal - b.CuryCreditTotal), 4)) >= 0.00005m && !isCurrencyTB ||
						Math.Abs(Math.Round((decimal)(b.DebitTotal - b.CreditTotal), 4)) >= 0.00005m && isCurrencyTB)
					{
						throw new PXException(Messages.BatchOutOfBalance);
					}

					if (Math.Abs(Math.Round((decimal)(b.DebitTotal - b.CreditTotal), 4)) >= 0.00005m)
					{
						BatchModule.Current = b;

						GLTran tran = new GLTran();
						Currency c = PXSelect<Currency, Where<Currency.curyID, Equal<Required<CurrencyInfo.curyID>>>>.Select(this, info.CuryID);

						if (Math.Sign((decimal)(b.DebitTotal - b.CreditTotal)) == 1)
						{
							tran.AccountID = c.RoundingGainAcctID;
							tran.SubID = c.RoundingGainSubID;
							tran.CreditAmt = Math.Round((decimal)(b.DebitTotal - b.CreditTotal), 4);
							tran.DebitAmt = 0;

							b.ControlTotal = b.DebitTotal;
							b.CreditTotal = b.DebitTotal;
						}
						else
						{
							tran.AccountID = c.RoundingLossAcctID;
							tran.SubID = c.RoundingLossSubID;
							tran.CreditAmt = 0;
							tran.DebitAmt = Math.Round((decimal)(b.CreditTotal - b.DebitTotal), 4);

							b.ControlTotal = b.CreditTotal;
							b.DebitTotal = b.CreditTotal;
						}
						tran.BranchID = b.BranchID;
						tran.CuryCreditAmt = 0;
						tran.CuryDebitAmt = 0;
						tran.TranDesc = PXMessages.LocalizeNoPrefix(Messages.RoundingDiff);
						tran.LedgerID = b.LedgerID;
						tran.FinPeriodID = b.FinPeriodID;
						tran.TranDate = b.DateEntered;
						tran.Released = true;

						CurrencyInfo infocopy = new CurrencyInfo();
						infocopy.CuryInfoID = -1;
						tran.CuryInfoID = -1;
						infocopy = (CurrencyInfo)CurrencyInfo_ID.Cache.Insert(infocopy);

						tran = (GLTran)GLTran_Module_BatNbr.Cache.Insert(tran);

						newtran = tran;
					}
				}

				b.Released = true;
				b.Status = BatchStatus.Unposted;
				BatchModule.Update(b);

				BatchModule.Cache.Persist(PXDBOperation.Update);

					tasksToAutoAllocate = CreateProjectTransactions(b);

                    #region TaxEntryFromGL
                    bool taxTransCreated = false;
                    if (createTaxTrans && ledger.BalanceType == LedgerBalanceType.Actual) 
                    {
                        taxTransCreated = taxCreationProc.CreateTaxTransactions();                        
                    }

                    #endregion

                    //Caches[typeof(TranslationHistory)].Persist(PXDBOperation.Update);

                    if (newtran != null)
                    {
                        CurrencyInfo_ID.Cache.Persist(PXDBOperation.Insert);
                        newtran.CuryInfoID = Convert.ToInt64(PXDatabase.SelectIdentity());
                        GLTran_Module_BatNbr.Cache.Persist(PXDBOperation.Insert);
                    }

                        GLTran_Module_BatNbr.Cache.Persist(PXDBOperation.Update);

                    catran.Cache.Persist(PXDBOperation.Update);
                    //Caches[typeof(GLTran)].Persist(PXDBOperation.Update);
					Caches[typeof(CADailySummary)].Persist(PXDBOperation.Insert);
                    if (taxTransCreated)
                        this.GL_GLTran_Taxes.Cache.Persist(PXDBOperation.Insert);
                    ts.Complete(this);
                }
                BatchModule.Cache.Persisted(false);
                //Caches[typeof(TranslationHistory)].Persisted(false);
                CurrencyInfo_ID.Cache.Persisted(false);
                GLTran_Module_BatNbr.Cache.Persisted(false);
                catran.Cache.Persisted(false);
				Caches[typeof(CADailySummary)].Persisted(false);
                this.GL_GLTran_Taxes.Cache.Persisted(false);

            BatchModule.Cache.RestoreCopy(b, BatchModule.Current);

			if (tasksToAutoAllocate.Count > 0)
			{
				try
				{
					AutoAllocateTasks(new List<PMTask>(tasksToAutoAllocate.Values));
				}
				catch (Exception ex)
				{
					throw new PXException(ex, PM.Messages.AutoAllocationFailed);
				}
				
			}
        }

		private Dictionary<string, PMTask> CreateProjectTransactions(Batch b)
		{
			Dictionary<string, PMTask> tasksToAutoAllocate = new Dictionary<string, PMTask>();

				if (b.Module == GL.BatchModule.GL)
				{
					PXSelectBase<GLTran> select = new PXSelectJoin<GLTran,
					InnerJoin<Account, On<GLTran.accountID, Equal<Account.accountID>>,
					InnerJoin<PMAccountGroup, On<PMAccountGroup.groupID, Equal<Account.accountGroupID>>,
					InnerJoin<PMProject, On<PMProject.contractID, Equal<GLTran.projectID>, And<PMProject.nonProject, Equal<False>>>,
					InnerJoin<PMTask, On<PMTask.projectID, Equal<GLTran.projectID>, And<PMTask.taskID, Equal<GLTran.taskID>>>>>>>,
					Where<GLTran.module, Equal<BatchModule.moduleGL>,
					And<GLTran.batchNbr, Equal<Required<GLTran.batchNbr>>,
					And<Account.accountGroupID, IsNotNull>>>>(this);

					PXResultset<GLTran> resultset = select.Select(b.BatchNbr);

					if (resultset.Count > 0)
					{
						PMRegister doc = new PMRegister();
						doc.Module = b.Module;
						doc.Date = b.DateEntered;
						doc.Description = b.Description;
						doc.Released = true;
						ProjectDocs.Insert(doc);
						ProjectDocs.Cache.Persist(PXDBOperation.Insert);
					}

					List<PMTran> sourceForAllocation = new List<PMTran>();

					foreach (PXResult<GLTran, Account, PMAccountGroup, PMProject, PMTask> res in resultset)
					{
						GLTran tran = (GLTran)res;
						Account acc = (Account)res;
						PMAccountGroup ag = (PMAccountGroup)res;
						PMProject project = (PMProject)res;
						PMTask task = (PMTask)res;

						PMTran pmt = (PMTran)ProjectTrans.Cache.Insert();
						pmt.BranchID = tran.BranchID;
						pmt.AccountGroupID = acc.AccountGroupID;
						pmt.AccountID = tran.AccountID;
						pmt.SubID = tran.SubID;
						pmt.BAccountID = tran.ReferenceID;
						pmt.BatchNbr = tran.BatchNbr;
						pmt.Date = tran.TranDate;
						pmt.Description = tran.TranDesc;
						pmt.FinPeriodID = tran.FinPeriodID;
						pmt.TranPeriodID = tran.TranPeriodID;
						pmt.InventoryID = tran.InventoryID ?? PM.PMInventorySelectorAttribute.EmptyInventoryID;
						pmt.OrigLineNbr = tran.LineNbr;
						pmt.OrigModule = tran.Module;
						pmt.OrigRefNbr = tran.RefNbr;
						pmt.OrigTranType = tran.TranType;
						pmt.ProjectID = tran.ProjectID;
						pmt.TaskID = tran.TaskID;
						pmt.Billable = tran.NonBillable != true;
						pmt.UseBillableQty = true;
						pmt.UOM = tran.UOM;

						pmt.Amount = tran.DebitAmt - tran.CreditAmt;
						pmt.Qty = tran.Qty;// pmt.Amount >= 0 ? tran.Qty : (tran.Qty * -1);
						int sign = 1;
						if (acc.Type == AccountType.Income || acc.Type == AccountType.Liability)
						{
							sign = -1;
						}

						if (acc.Type != ag.Type)
						{
							pmt.Amount = -pmt.Amount;
							pmt.Qty = -pmt.Qty;
						}
						pmt.BillableQty = tran.Qty;
						pmt.Released = true;

						PXNoteAttribute.CopyNoteAndFiles(GLTran_Module_BatNbr.Cache, tran, ProjectTrans.Cache, pmt);

						ProjectTrans.Update(pmt);
						ProjectTrans.Cache.Persist(pmt, PXDBOperation.Insert);

						tran.PMTranID = pmt.TranID;
						this.Caches[typeof(GLTran)].Update(tran);

						if (pmt.TaskID != null && (pmt.Qty != 0 || pmt.Amount != 0)) //TaskID will be null for Contract
						{
							#region Project Status Update
							//Note: This is a special case of RegisterReleaseProcess.UpdateProjectBalance()
							//Note: GLTran.InventoryID will always be null here and thus can be simplified:  
							PMProjectStatus otherStatus = PXSelect<PMProjectStatus,
								Where<PMProjectStatus.accountGroupID, Equal<Required<PMProjectStatus.accountGroupID>>,
								And<PMProjectStatus.projectID, Equal<Required<PMProjectStatus.projectID>>,
								And<PMProjectStatus.projectTaskID, Equal<Required<PMProjectStatus.projectTaskID>>,
								And<PMProjectStatus.inventoryID, Equal<Required<PMProjectStatus.inventoryID>>>>>>>.Select(this, pmt.AccountGroupID, pmt.ProjectID, pmt.TaskID, PMProjectStatus.EmptyInventoryID);//do not restrict by FinPeriod

							decimal rollupQty = 0;
							string UOM = null;
							if (otherStatus != null && !string.IsNullOrEmpty(otherStatus.UOM))
							{
								UOM = otherStatus.UOM;

								if (otherStatus.UOM == pmt.UOM)
								{
									rollupQty = pmt.Qty.GetValueOrDefault();
								}
								else
								{
									//convert if possible:
									PX.Objects.IN.INUnitAttribute.TryConvertGlobalUnits(this, pmt.UOM, UOM, pmt.Qty.GetValueOrDefault(), PX.Objects.IN.INPrecision.QUANTITY, out rollupQty);
								}
							}

							decimal amount = pmt.Amount.GetValueOrDefault() * sign;

							PMProjectStatusAccum ps = new PMProjectStatusAccum();
							ps.PeriodID = pmt.FinPeriodID;
							ps.ProjectID = pmt.ProjectID;
							ps.ProjectTaskID = pmt.TaskID;
							ps.AccountGroupID = pmt.AccountGroupID;
							ps.InventoryID = PMProjectStatus.EmptyInventoryID;
							ps.UOM = UOM;

							ps = (PMProjectStatusAccum)this.Caches[typeof(PMProjectStatusAccum)].Insert(ps);

							ps.ActualQty += rollupQty;
							ps.ActualAmount += amount;
							this.Views.Caches.Add(typeof(PMProjectStatusAccum));

							#endregion

							#region PMTask Totals Update

							PMTaskTotal ta = new PMTaskTotal();
							ta.ProjectID = pmt.ProjectID;
							ta.TaskID = pmt.TaskID;

							ta = (PMTaskTotal)this.Caches[typeof(PMTaskTotal)].Insert(ta);

							switch (acc.Type)
							{
								case AccountType.Asset:
									ta.Asset += amount;
									break;
								case AccountType.Liability:
									ta.Liability += amount;
									break;
								case AccountType.Income:
									ta.Income += amount;
									break;
								case AccountType.Expense:
									ta.Expense += amount;
									break;
							}
							this.Views.Caches.Add(typeof(PMTaskTotal));

							#endregion

							PM.RegisterReleaseProcess.AddToUnbilledSummary(this, pmt);

							sourceForAllocation.Add(pmt);
							if (pmt.Allocated != true && project.AutoAllocate == true)
							{
								if (!tasksToAutoAllocate.ContainsKey(string.Format("{0}.{1}", task.ProjectID, task.TaskID)))
								{
									tasksToAutoAllocate.Add(string.Format("{0}.{1}", task.ProjectID, task.TaskID), task);
								}
							}
						}
					}


					Caches[typeof(PMProjectStatusAccum)].Persist(PXDBOperation.Insert);
					Caches[typeof(PMTaskTotal)].Persist(PXDBOperation.Insert);
				}

			return tasksToAutoAllocate;
				}

		public class TaxTranCreationProcessor
		{
			protected PXCache GLTranCache = null;
			protected PXCache TaxTranCache = null;
			protected PXGraph graph = null;
			protected Dictionary<string, List<PXResult<GLTran, CATran, CurrencyInfo, Account, Ledger>>> Documents;
			protected bool skipTaxValidation = false;

			public TaxTranCreationProcessor(PXCache aGLTranCache, PXCache aTaxTranCache, bool aSkipTaxValidation)
			{
				this.GLTranCache = aGLTranCache;
				this.TaxTranCache = aTaxTranCache;
				this.graph = this.GLTranCache.Graph;
				this.skipTaxValidation = aSkipTaxValidation;
				this.Documents = new Dictionary<string, List<PXResult<GLTran, CATran, CurrencyInfo, Account, Ledger>>>();
			}
			public void AddToDocuments(PXResult<GLTran, CATran, CurrencyInfo, Account, Ledger> aTran)
			{
				GLTran tran = aTran;
				if (!String.IsNullOrEmpty(tran.RefNbr))
				{
					List<PXResult<GLTran, CATran, CurrencyInfo, Account, Ledger>> lines;
					if (!this.Documents.TryGetValue(tran.RefNbr, out lines))
					{
						lines = new List<PXResult<GLTran, CATran, CurrencyInfo, Account, Ledger>>(1);
						this.Documents.Add(tran.RefNbr, lines);
					}
					lines.Add(aTran);
				}
			}
			public virtual bool CreateTaxTransactions()
			{
				bool taxTransCreated = false;
				if (this.Documents.Count > 0)
				{
					//bool skipTaxValidation = ((Batch) graph.Caches[typeof (Batch)].Current).SkipTaxValidation == true;
					foreach (KeyValuePair<string, List<PXResult<GLTran, CATran, CurrencyInfo, Account, Ledger>>> iDoc in this.Documents)
					{
						List<PXResult<GLTran, CATran, CurrencyInfo, Account, Ledger>> iLines = iDoc.Value;
						string refNumber = iDoc.Key;
						Decimal curyCreditTotal = Decimal.Zero;
						Decimal curyDebitTotal = Decimal.Zero;
						List<GLTran> assetTrans = new List<GLTran>();
						List<GLTran> liabilityTrans = new List<GLTran>();
						List<GLTran> purchaseTaxTrans = new List<GLTran>();                        
						List<GLTran> salesTaxTrans = new List<GLTran>();
						List<GLTran> withHoldingTaxTrans = new List<GLTran>();
						List<GLTran> incomeTrans = new List<GLTran>();
						List<GLTran> expenseTrans = new List<GLTran>();
						CurrencyInfo docInfo = null;
						Dictionary<string, Tax> taxes = new Dictionary<string, Tax>();
						Dictionary<string, List<GLTran>> taxCategories = new Dictionary<string, List<GLTran>>();
						Dictionary<string, HashSet<GLTran>> taxGroups = new Dictionary<string, HashSet<GLTran>>();
						foreach (PXResult<GLTran, CATran, CurrencyInfo, Account, Ledger> iLn in iLines)
						{
							GLTran trn = iLn;
							Account acct = iLn;
							if (docInfo == null) docInfo = iLn;
							curyCreditTotal += trn.CuryCreditAmt.Value;
							curyDebitTotal += trn.CuryDebitAmt.Value;

							if (String.IsNullOrEmpty(trn.TaxID) == false)
							{
								Tax tax;
								if (!taxes.TryGetValue(trn.TaxID, out tax))
								{
									tax = PXSelect<TX.Tax, Where<TX.Tax.taxID, Equal<Required<TX.Tax.taxID>>>>.Select(this.graph, trn.TaxID);
									if (tax == null)
										throw new PXException(Messages.UnrecognizedTaxFoundUsedInDocument, trn.TaxID, refNumber);
									taxes.Add(tax.TaxID, tax);
								}

								if (tax.PurchTaxAcctID == tax.SalesTaxAcctID && tax.PurchTaxSubID == tax.SalesTaxSubID)
								{
									throw new PXException(TX.Messages.ClaimableAndPayableAccountsAreTheSame, trn.TaxID);
								}

								bool isPurchaseTaxTran = (tax.PurchTaxAcctID == trn.AccountID && tax.PurchTaxSubID == trn.SubID && (tax.ReverseTax !=true));
								bool isSalesTaxTran = (tax.SalesTaxAcctID == trn.AccountID && tax.SalesTaxSubID == trn.SubID && (tax.ReverseTax != true) );
								bool isReversedPurchaseTaxTran = (tax.PurchTaxAcctID == trn.AccountID && tax.PurchTaxSubID == trn.SubID && (tax.ReverseTax == true));
								bool isReversedSalesTaxTran = (tax.SalesTaxAcctID == trn.AccountID && tax.SalesTaxSubID == trn.SubID && (tax.ReverseTax == true));
								bool isWithHoldingTaxTran = tax.TaxType == TX.CSTaxType.Withholding && (tax.SalesTaxAcctID == trn.AccountID && tax.SalesTaxSubID == trn.SubID);
								if (isWithHoldingTaxTran)
								{
									withHoldingTaxTrans.Add(trn);
								}
								else if (isSalesTaxTran || isReversedPurchaseTaxTran)
								{
									salesTaxTrans.Add(trn);
								}
								else if (isPurchaseTaxTran || isReversedSalesTaxTran)
								{
									purchaseTaxTrans.Add(trn);
								}								
							}
							else
							{
								if (!String.IsNullOrEmpty(trn.TaxCategoryID))
								{
									List<GLTran> catList;
									if (!taxCategories.TryGetValue(trn.TaxCategoryID, out catList))
									{
										catList = new List<GLTran>();
										taxCategories.Add(trn.TaxCategoryID, catList);
									}
									catList.Add(trn);

									if (acct.Type == AccountType.Asset)
										assetTrans.Add(trn);
									if (acct.Type == AccountType.Liability)
										liabilityTrans.Add(trn);
									if (acct.Type == AccountType.Expense)
										expenseTrans.Add(trn);
									if (acct.Type == AccountType.Income)
										incomeTrans.Add(trn);
								}
							}
						}

						if (salesTaxTrans.Count > 0)
						{
							TaxTranCreationHelper.SegregateTaxGroup(GLTranCache, salesTaxTrans, taxes, taxCategories, taxGroups);
						}
						if (purchaseTaxTrans.Count > 0)
						{
							TaxTranCreationHelper.SegregateTaxGroup(GLTranCache, purchaseTaxTrans, taxes, taxCategories, taxGroups);
						}

						if (withHoldingTaxTrans.Count > 0)
						{
							TaxTranCreationHelper.SegregateTaxGroup(GLTranCache, withHoldingTaxTrans, taxes, taxCategories, taxGroups);
						}

						bool hasTaxes = (salesTaxTrans.Count > 0 || purchaseTaxTrans.Count > 0 || withHoldingTaxTrans.Count >0); //There is no need to analize doc's withount taxes.
						if (hasTaxes)
						{
							if (curyCreditTotal != curyDebitTotal)
							{
								throw new PXException(Messages.DocumentWithTaxIsNotBalanced, refNumber);
							}

							if (salesTaxTrans.Count > 0 && purchaseTaxTrans.Count > 0)
							{
								throw new PXException(Messages.DocumentWithTaxContainsBothSalesAndPurchaseTransactions, refNumber);
							}

							if ( withHoldingTaxTrans.Count > 0 && (purchaseTaxTrans.Count > 0 || salesTaxTrans.Count>0))
							{
								throw new PXException(Messages.DocumentMayNotIncludeWithholdindAndSalesOrPurchanseTaxes, refNumber);
							}


							if (salesTaxTrans.Count > 0 && incomeTrans.Count == 0 && assetTrans.Count == 0)
							{
								throw new PXException(Messages.DocumentContainsSalesTaxTransactionsButNoIncomeAccounts, refNumber);
							}

							if (purchaseTaxTrans.Count > 0 && expenseTrans.Count == 0 && assetTrans.Count == 0)
							{
								throw new PXException(Messages.DocumentContainsPurchaseTaxTransactionsButNoExpenseAccounts, refNumber);
							}

							if (withHoldingTaxTrans.Count > 0 && expenseTrans.Count == 0 && liabilityTrans.Count == 0)
							{
								throw new PXException(Messages.DocumentContainsWithHoldingTaxTransactionsButNoExpenseAccountsOrLiabilityAccount, refNumber);
							}

							List<GLTran> taxTrans = salesTaxTrans.Count > 0 ? salesTaxTrans: (purchaseTaxTrans.Count > 0? purchaseTaxTrans: withHoldingTaxTrans);
							string groupTaxType = salesTaxTrans.Count > 0 || withHoldingTaxTrans.Count >0 ? TaxType.Sales : TaxType.Purchase;
							Dictionary<string, GLTran> taxTransSummary = new Dictionary<string, GLTran>();
							PXCache glTranCache = this.GLTranCache;

							foreach (GLTran iTaxTran in taxTrans)
							{
								GLTran summary;
								if (!taxTransSummary.TryGetValue(iTaxTran.TaxID, out summary))
								{
									summary = (GLTran)glTranCache.CreateCopy(iTaxTran);
									taxTransSummary.Add(summary.TaxID, summary);
								}
								else
									TaxTranCreationHelper.Aggregate(summary, iTaxTran);
							}

							foreach (GLTran iTaxTran in taxTransSummary.Values)
							{
								List<GLTran> taxableTrans = new List<GLTran>();
								HashSet<GLTran> taxgroup;
								string taxID = iTaxTran.TaxID;
								TaxTran taxTran = new TaxTran();
								taxTran.TaxID = taxID;
								Tax tax = taxes[taxID];
								string taxType = groupTaxType;
								if (tax.ReverseTax == true) 
								{
									taxType = (groupTaxType == TaxType.Sales) ? TaxType.Purchase : TaxType.Sales;
								} 
								TaxTranCreationHelper.Copy(taxTran, iTaxTran, tax);

								if (taxGroups.TryGetValue(taxID, out taxgroup))
								{
									taxableTrans.AddRange(taxgroup.ToArray<GLTran>());
								}
								if (taxgroup == null || taxableTrans.Count == 0)
								{
									throw new PXException(Messages.NoTaxableLinesForTaxID, refNumber, taxID);
								}
								taxTran.CuryTaxableAmt = taxTran.TaxableAmt = Decimal.Zero;
								foreach (GLTran iTaxable in taxableTrans)
								{
									Account acct = PXSelectReadonly<Account, Where<Account.accountID,
										Equal<Required<Account.accountID>>>>.Select(glTranCache.Graph, iTaxable.AccountID);
									int accTypeSign = 1;
									if (tax.TaxType == CSTaxType.Withholding)
									{
										accTypeSign = -1;
										if (acct.Type == AccountType.Income || acct.Type == AccountType.Asset)
											throw new PXException(Messages.DocumentContainsIncomeOrAssetAccountdAsTaxableForWithHoldingTax,refNumber);
									}
									else
									{
										accTypeSign = (acct.Type == AccountType.Income || acct.Type == AccountType.Liability) && taxType == TaxType.Sales ||
													   (acct.Type == AccountType.Asset || acct.Type == AccountType.Expense) && taxType == TaxType.Purchase ? 1 : -1;
									}
									taxTran.CuryTaxableAmt += (iTaxable.CuryDebitAmt - iTaxable.CuryCreditAmt) * accTypeSign;
									taxTran.TaxableAmt += (iTaxable.DebitAmt - iTaxable.CreditAmt) * accTypeSign;
								}
								int sign = Math.Sign(taxTran.CuryTaxableAmt.Value);
								taxTran.CuryTaxableAmt *= sign;
								taxTran.TaxableAmt *= sign;
								taxTran.CuryTaxAmt = ((iTaxTran.CuryDebitAmt ?? Decimal.Zero) - (iTaxTran.CuryCreditAmt ?? Decimal.Zero)) * sign;
								taxTran.TaxAmt = ((iTaxTran.DebitAmt ?? Decimal.Zero) - (iTaxTran.CreditAmt ?? Decimal.Zero)) * sign;
								taxTran.TranType = GetTranType(taxType, sign);
								TaxRev taxRev = null;
								foreach (TaxRev iRev in PXSelectReadonly<TaxRev, Where<TaxRev.taxID, Equal<Required<TaxRev.taxID>>,
												And<TaxRev.outdated, Equal<False>,
												And<TaxRev.taxType, Equal<Required<TaxRev.taxType>>,
												And<Required<GLTran.tranDate>, Between<TaxRev.startDate, TaxRev.endDate>>>>>>.Select(this.graph, taxID, taxType, taxTran.TranDate))
								{
									if (taxRev != null && iRev.TaxID == taxID)
									{
										throw new PXException(Messages.SeveralTaxRevisionFoundForTheTax, taxID, taxTran.TranDate);
									}
									taxRev = (TaxRev)this.graph.Caches[typeof(TaxRev)].CreateCopy(iRev);
									if (taxRev != null && tax.DeductibleVAT != true)
									{
										taxRev.NonDeductibleTaxRate = 100m;
									}
								}
								if (taxRev != null)
								{
									TaxTranCreationHelper.Copy(taxTran, taxRev);
									TaxTranCreationHelper.AdjustExpenseAmt(this.TaxTranCache, taxTran);
									if (!this.skipTaxValidation)
									{
										TaxTranCreationHelper.Validate(this.TaxTranCache, taxTran, taxRev, iTaxTran);
									}
									taxTran.Released = true;
									TaxTran copy = (TaxTran)this.TaxTranCache.Insert(taxTran);
									taxTransCreated = true;
								}
								else
								{
									throw new PXException(
										TX.Messages.TaxRateNotSpecified, 
										tax.TaxID,
										PXMessages.LocalizeNoPrefix(GetLabel.For<TaxType>(taxType)).ToLower());
								}
							}
						}
					}
				}
				return taxTransCreated;
			}
			public static string GetTranType(string taxType, decimal sign)
			{
				string result = string.Empty;
				if (taxType == TaxType.Sales)
					result = sign > Decimal.Zero ? TaxTran.tranType.TranReversed : TaxTran.tranType.TranForward;
				if (taxType == TaxType.Purchase)
					result = sign > Decimal.Zero ? TaxTran.tranType.TranForward : TaxTran.tranType.TranReversed;
				if (string.IsNullOrEmpty(result))
				{
					throw new PXException(Messages.TypeForTheTaxTransactionIsNoRegonized, taxType, sign);
				}
				return result;
			}
			public static class TaxTranCreationHelper
			{
				public static void Copy(TaxTran aDest, TaxRev aSrc)
				{
					aDest.TaxRate = aSrc.TaxRate;
					aDest.TaxType = aSrc.TaxType;
					aDest.TaxBucketID = aSrc.TaxBucketID;
					aDest.NonDeductibleTaxRate = aSrc.NonDeductibleTaxRate;
				}
				public static void Copy(TaxTran aDest, GLTran aTaxTran, Tax taxDef)
				{
					aDest.TaxID = aTaxTran.TaxID;
					aDest.AccountID = aTaxTran.AccountID;
					aDest.SubID = aTaxTran.SubID;
					aDest.CuryInfoID = aTaxTran.CuryInfoID;
					aDest.Module = aTaxTran.Module;
					aDest.TranDate = aTaxTran.TranDate;
					aDest.BranchID = aTaxTran.BranchID;
					aDest.TranType = aTaxTran.TranType;
					aDest.RefNbr = aTaxTran.BatchNbr;
					aDest.FinPeriodID = aTaxTran.FinPeriodID;
					aDest.LineRefNbr = aTaxTran.RefNbr;
					aDest.VendorID = taxDef.TaxVendorID;
				}
				public static void Aggregate(GLTran aDest, GLTran aSrc)
				{
					aDest.CuryCreditAmt += aSrc.CuryCreditAmt ?? Decimal.Zero;
					aDest.CuryDebitAmt += aSrc.CuryDebitAmt ?? Decimal.Zero;
					aDest.CreditAmt += aSrc.CreditAmt ?? Decimal.Zero;
					aDest.DebitAmt += aSrc.DebitAmt ?? Decimal.Zero;
				}
				public static void Validate(PXCache cache, TaxTran tran, TaxRev taxRev, GLTran aDocTran)
				{

					if (Math.Sign(tran.CuryTaxAmt.Value) != 0 && Math.Sign(tran.CuryTaxableAmt.Value) != Math.Sign(tran.CuryTaxAmt.Value))
						throw new PXException(Messages.TaxableAndTaxAmountsHaveDifferentSignsForDocument, aDocTran.TaxID, aDocTran.RefNbr);
					if (tran.CuryTaxAmt.Value < 0)
						throw new PXException(Messages.TaxAmountIsNegativeForTheDocument, aDocTran.TaxID, aDocTran.RefNbr);

					TaxTran calculatedTax = CalculateTax(cache, tran, taxRev);
					if (calculatedTax.CuryTaxAmt != tran.CuryTaxAmt)
						if (decimal.Compare((decimal)tran.NonDeductibleTaxRate, 100m) < 0)
						{
							throw new PXException(Messages.DeductedTaxAmountEnteredDoesNotMatchToAmountCalculatedFromTaxableForTheDocument, tran.CuryTaxAmt.Value,
								calculatedTax.CuryTaxAmt, tran.CuryTaxableAmt.Value, tran.TaxRate, tran.NonDeductibleTaxRate, aDocTran.TaxID, aDocTran.RefNbr);
						}
						else
						{
							throw new PXException(Messages.TaxAmountEnteredDoesNotMatchToAmountCalculatedFromTaxableForTheDocument, tran.CuryTaxAmt.Value,
								calculatedTax.CuryTaxAmt, tran.CuryTaxableAmt.Value, tran.TaxRate, aDocTran.TaxID, aDocTran.RefNbr);
						}

					if (tran.CuryTaxableAmt != calculatedTax.CuryTaxableAmt)
					{
						tran.CuryTaxableAmt = calculatedTax.CuryTaxableAmt; //May happen due to min/max taxable limitations.
					}
				}
				public static TaxTran CalculateTax(PXCache cache, TaxTran aTran, TaxRev aTaxRev)
				{
					Decimal curyTaxableAmt = aTran.CuryTaxableAmt ?? Decimal.Zero;
					Decimal taxableAmt = aTran.TaxableAmt ?? Decimal.Zero;
					if (aTaxRev.TaxableMin != 0.0m)
					{
						if (taxableAmt < (decimal)aTaxRev.TaxableMin)
						{
							curyTaxableAmt = 0.0m;
							taxableAmt = 0.0m;
						}
					}
					if (aTaxRev.TaxableMax != 0.0m)
					{
						if (taxableAmt > (decimal)aTaxRev.TaxableMax)
						{
							PXDBCurrencyAttribute.CuryConvCury(cache, aTran, (decimal)aTaxRev.TaxableMax, out curyTaxableAmt);
							taxableAmt = (decimal)aTaxRev.TaxableMax;
						}
					}
					Decimal curyExpenseAmt = 0m;
					Decimal curyTaxAmount = (curyTaxableAmt * (decimal)aTaxRev.TaxRate / 100);
					if ((decimal)aTaxRev.NonDeductibleTaxRate < 100m)
					{
						curyExpenseAmt = curyTaxAmount * (1 - (decimal)aTaxRev.NonDeductibleTaxRate / 100);
						curyTaxAmount -= curyExpenseAmt;
					}
					TaxTran result = (TaxTran)cache.CreateCopy(aTran);
					result.CuryTaxableAmt = PXDBCurrencyAttribute.RoundCury(cache, aTran, curyTaxableAmt);
					result.CuryTaxAmt = PXDBCurrencyAttribute.RoundCury(cache, aTran, curyTaxAmount);//MS TaxAmount and Taxable account will be recalculated on insert anyway;
					result.CuryExpenseAmt = PXDBCurrencyAttribute.RoundCury(cache, aTran, curyExpenseAmt);
					return result;
				}
				public static void AdjustExpenseAmt(PXCache cache, TaxTran tran)
				{
					if (tran != null && (decimal)tran.NonDeductibleTaxRate < 100m)
					{
						decimal ExpenseRate = (decimal)tran.TaxRate * (100 - (decimal)tran.NonDeductibleTaxRate) / 100;
						Decimal curyTaxableAmount = (decimal)tran.CuryTaxableAmt / ((100 + ExpenseRate) / 100);
						Decimal curyExpenseAmt = curyTaxableAmount * ExpenseRate / 100;
						tran.CuryExpenseAmt = PXDBCurrencyAttribute.RoundCury(cache, tran, curyExpenseAmt);
						tran.CuryTaxableAmt = PXDBCurrencyAttribute.RoundCury(cache, tran, curyTaxableAmount);
					}
				}
				public static void SegregateTaxGroup(PXCache cache, List<GLTran> taxLines, Dictionary<string, Tax> taxes,
					Dictionary<string, List<GLTran>> taxCategories, Dictionary<string, HashSet<GLTran>> taxGroups)
				{
					foreach (var taxLine in taxLines)
					{
						if (taxLine.TaxID == null && !taxes.ContainsKey(taxLine.TaxID)) continue;
						PXResultset<TaxCategory> taxCategoryDetails = (new PXSelectJoin<TaxCategory, LeftJoin<TaxCategoryDet, 
							On<TaxCategory.taxCategoryID, Equal<TaxCategoryDet.taxCategoryID>>>,
								Where2<Where<TaxCategory.taxCatFlag, Equal<True>,
								And<Where<TaxCategoryDet.taxID, NotEqual<Required<TaxCategoryDet.taxID>>, Or<TaxCategoryDet.taxID, IsNull>>>>,
								Or<Where<TaxCategory.taxCatFlag, Equal<False>,
								And<TaxCategoryDet.taxID, Equal<Required<TaxCategoryDet.taxID>>>>>>>(cache.Graph)).Select(taxLine.TaxID, taxLine.TaxID);
						if (taxCategoryDetails.Count == 0) continue;
						foreach (TaxCategory tcd in taxCategoryDetails)
						{
							List<GLTran> catgroup;
							if (taxCategories.TryGetValue(tcd.TaxCategoryID, out catgroup))
							{
								HashSet<GLTran> taxgroup;
								if (!taxGroups.TryGetValue(taxLine.TaxID, out taxgroup))
								{
									taxgroup = new HashSet<GLTran>();
									taxGroups.Add(taxLine.TaxID, taxgroup);
								}
								taxgroup.UnionWith(catgroup);
							}
						}
					}
				}
			}
		}
		
		
		protected virtual void UpdateProjectBalance(Batch b)
		{
			PXSelectBase<GLTran> select = new PXSelectJoin<GLTran,
				InnerJoin<PMTran, On<GLTran.pMTranID, Equal<PMTran.tranID>>,
				InnerJoin<PMAccountGroup, On<PMTran.accountGroupID, Equal<PMAccountGroup.groupID>>,
				LeftJoin<Account, On<PMTran.offsetAccountID, Equal<Account.accountID>>,
				LeftJoin<OffsetPMAccountGroup, On<Account.accountGroupID, Equal<OffsetPMAccountGroup.groupID>>>>>>,
				Where<GLTran.module, Equal<Required<GLTran.module>>,
				And<GLTran.batchNbr, Equal<Required<GLTran.batchNbr>>,
				And<GLTran.pMTranID, IsNotNull,
				And<GLTran.projectID, NotEqual<Required<GLTran.projectID>>>>>>>(this);

			foreach (PXResult<GLTran, PMTran, PMAccountGroup, Account, OffsetPMAccountGroup> res in select.Select(b.Module, b.BatchNbr, ProjectDefaultAttribute.NonProject(this)))
			{
				GLTran tran = (GLTran)res;
				PMTran pmt = (PMTran)res;
				PMAccountGroup ag = (PMAccountGroup)res;
				Account offsetAccount = (Account)res;
				OffsetPMAccountGroup offsetAg = (OffsetPMAccountGroup)res;
				
				#region History Update

				bool isOffsetTran = pmt.OffsetAccountID == tran.AccountID;
				decimal amt = tran.DebitAmt.GetValueOrDefault() - tran.CreditAmt.GetValueOrDefault();

				if (isOffsetTran)
				{
					if (offsetAg.Type == AccountType.Income || offsetAg.Type == AccountType.Liability)
					{
						amt *= -1;
					}
				}
				else
				{
					if (ag.Type == AccountType.Income || ag.Type == AccountType.Liability)
					{
						amt *= -1;
					}
				}


				PMHistoryAccum hist = new PMHistoryAccum();
				hist.ProjectID = pmt.ProjectID;
				hist.ProjectTaskID = pmt.TaskID;
				hist.AccountGroupID = isOffsetTran ? offsetAccount.AccountGroupID : pmt.AccountGroupID;
				hist.InventoryID = pmt.InventoryID ?? PMProjectStatus.EmptyInventoryID;
				hist.PeriodID = pmt.FinPeriodID;

				hist = (PMHistoryAccum)this.Caches[typeof(PMHistoryAccum)].Insert(hist);

				decimal baseQty = 0;

				if (pmt.InventoryID != null && pmt.InventoryID != PMInventorySelectorAttribute.EmptyInventoryID && pmt.Qty != null && pmt.Qty != 0 && !string.IsNullOrEmpty(pmt.UOM))
				{
					if (PXAccess.FeatureInstalled<FeaturesSet.multipleUnitMeasure>())
					{
						baseQty = IN.INUnitAttribute.ConvertToBase(this.Caches[typeof(PMHistory)], pmt.InventoryID, pmt.UOM, pmt.Qty ?? 0, IN.INPrecision.QUANTITY);
					}
					else
					{
						IN.InventoryItem initem = PXSelect<IN.InventoryItem, Where<IN.InventoryItem.inventoryID, Equal<Required<IN.InventoryItem.inventoryID>>>>.Select(this, pmt.InventoryID);
						baseQty = IN.INUnitAttribute.ConvertGlobalUnits(this, pmt.UOM, initem.BaseUnit, pmt.Qty ?? 0, IN.INPrecision.QUANTITY);
					}


				}

				hist.FinPTDAmount += amt;
				hist.FinYTDAmount += amt;
				hist.FinPTDQty += baseQty;
				hist.FinYTDQty += baseQty;

				if (pmt.FinPeriodID == pmt.TranPeriodID)
				{
					hist.TranPTDAmount += amt;
					hist.TranYTDAmount += amt;
					hist.TranPTDQty += baseQty;
					hist.TranYTDQty += baseQty;
				}
				else
				{
					PMHistoryAccum tranHist = new PMHistoryAccum();
					tranHist.ProjectID = pmt.ProjectID;
					tranHist.ProjectTaskID = pmt.TaskID;
					tranHist.AccountGroupID = isOffsetTran ? offsetAccount.AccountGroupID : pmt.AccountGroupID;
					tranHist.InventoryID = pmt.InventoryID ?? PM.PMProjectStatus.EmptyInventoryID;
					tranHist.PeriodID = pmt.TranPeriodID;

					tranHist = (PMHistoryAccum)this.Caches[typeof(PMHistoryAccum)].Insert(tranHist);
					tranHist.TranPTDAmount += amt;
					tranHist.TranYTDAmount += amt;
					tranHist.TranPTDQty += baseQty;
					tranHist.TranYTDQty += baseQty;

				}
				this.Views.Caches.Add(typeof(PMHistoryAccum));

				#endregion
			}
		}

		protected virtual void AutoAllocateTasks(List<PMTask> tasks)
		{
			PMSetup setup = PXSelect<PMSetup>.Select(this);
			bool autoreleaseAllocation = setup.AutoReleaseAllocation == true;

			PMAllocator allocator = PXGraph.CreateInstance<PMAllocator>();
			allocator.Clear();
			allocator.TimeStamp = this.TimeStamp;
			allocator.Execute(tasks);
			allocator.Actions.PressSave();

			if (allocator.Document.Current != null && autoreleaseAllocation)
			{
				List<PMRegister> list = new List<PMRegister>();
				list.Add(allocator.Document.Current);
				List<ProcessInfo<Batch>> batchList;
				bool releaseSuccess = RegisterRelease.ReleaseWithoutPost(list, false, out batchList);
				if (!releaseSuccess)
				{
					throw new PXException(PM.Messages.AutoReleaseFailed);
				}
			}
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



namespace PX.Objects.GL.Overrides.PostGraph
{
	[Serializable]
	[PXHidden]
	public partial class BatchCopy : Batch
	{
		public new abstract class origBatchNbr : PX.Data.IBqlField { }
		public new abstract class origModule : PX.Data.IBqlField { }
		public new abstract class autoReverseCopy : PX.Data.IBqlField { }
	}

	public class AHAccumAttribute : PXAccumulatorAttribute
	{
		protected int? reacct;
		protected int? niacct;

		public AHAccumAttribute()
			: base(new Type[] {
				typeof(GLHistory.finYtdBalance),
				typeof(GLHistory.tranYtdBalance),
				typeof(GLHistory.curyFinYtdBalance),
				typeof(GLHistory.curyTranYtdBalance),
				typeof(GLHistory.finYtdBalance),
				typeof(GLHistory.tranYtdBalance),
				typeof(GLHistory.curyFinYtdBalance),
				typeof(GLHistory.curyTranYtdBalance)
				},
					new Type[] {
				typeof(GLHistory.finBegBalance),
				typeof(GLHistory.tranBegBalance),
				typeof(GLHistory.curyFinBegBalance),
				typeof(GLHistory.curyTranBegBalance),
				typeof(GLHistory.finYtdBalance),
				typeof(GLHistory.tranYtdBalance),
				typeof(GLHistory.curyFinYtdBalance),
				typeof(GLHistory.curyTranYtdBalance)
				}
			)
		{
		}

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);
			GLSetup setup = (GLSetup)sender.Graph.Caches[typeof(GLSetup)].Current;
			if (setup == null)
			{
				setup = PXSelect<GLSetup>.Select(sender.Graph);
			}
			if (setup != null)
			{
				reacct = setup.RetEarnAccountID;
				niacct = setup.YtdNetIncAccountID;
			}

			sender.Graph.RowPersisted.AddHandler(sender.GetItemType(), RowPersisted);

			// Makes the inserts/updates of GL History follow
			// the order of inserts into the accumulator cache.
			// This is required to ensure that Net Income account
			// is updated before the Retained Earnings account,
			// so the latter can use the results of the former.
			// -
			sender.PreventDeadlock = false;
		}

		protected override bool PrepareInsert(PXCache sender, object row, PXAccumulatorCollection columns)
		{
			if (!base.PrepareInsert(sender, row, columns))
			{
				return false;
			}

			columns.InitializeFrom<GLHistory.finBegBalance, GLHistory.finYtdBalance>();
			columns.InitializeFrom<GLHistory.tranBegBalance, GLHistory.tranYtdBalance>();
			columns.InitializeFrom<GLHistory.curyFinBegBalance, GLHistory.curyFinYtdBalance>();
			columns.InitializeFrom<GLHistory.curyTranBegBalance, GLHistory.curyTranYtdBalance>();
			columns.InitializeFrom<GLHistory.finYtdBalance, GLHistory.finYtdBalance>();
			columns.InitializeFrom<GLHistory.tranYtdBalance, GLHistory.tranYtdBalance>();
			columns.InitializeFrom<GLHistory.curyFinYtdBalance, GLHistory.curyFinYtdBalance>();
			columns.InitializeFrom<GLHistory.curyTranYtdBalance, GLHistory.curyTranYtdBalance>();

			var hist = (GLHistory)row;

			columns.UpdateFuture<GLHistory.finBegBalance>(hist.FinYtdBalance);
			columns.UpdateFuture<GLHistory.tranBegBalance>(hist.TranYtdBalance);
			columns.UpdateFuture<GLHistory.curyFinBegBalance>(hist.CuryFinYtdBalance);
			columns.UpdateFuture<GLHistory.curyTranBegBalance>(hist.CuryTranYtdBalance);
			columns.UpdateFuture<GLHistory.finYtdBalance>(hist.FinYtdBalance);
			columns.UpdateFuture<GLHistory.tranYtdBalance>(hist.TranYtdBalance);
			columns.UpdateFuture<GLHistory.curyFinYtdBalance>(hist.CuryFinYtdBalance);
			columns.UpdateFuture<GLHistory.curyTranYtdBalance>(hist.CuryTranYtdBalance);

			bool? year = (hist.AccountID == niacct);
			if (year == false && hist.AccountID != reacct)
			{
				PXCache cache = sender.Graph.Caches[typeof(Account)];
				year = null;
				foreach (Account acct in cache.Cached)
				{
					if (acct.AccountID == hist.AccountID)
					{
						year = (acct.Type == AccountType.Income || acct.Type == AccountType.Expense);
						break;
					}
				}
				if (year == null)
				{
					Account acct = PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(sender.Graph, hist.AccountID);
					if (acct != null)
					{
						year = (acct.Type == AccountType.Income || acct.Type == AccountType.Expense);
					}
					else
					{
						year = false;
					}
				}
			}
			if (year == true)
			{
				columns.RestrictPast<GLHistory.finPeriodID>(PXComp.GE, hist.FinPeriodID.Substring(0, 4) + "01");
				columns.RestrictFuture<GLHistory.finPeriodID>(PXComp.LE, hist.FinPeriodID.Substring(0, 4) + "99");
			}

			return true;
		}

		protected string PreviousPeriod(string FinPeriodID)
		{
			using (PXDataRecord rec = PXDatabase.SelectSingle<FinPeriod>(
				new PXDataField<FinPeriod.finPeriodID>(),
				new PXDataFieldValue<FinPeriod.finPeriodID>(PXDbType.Char, 6, FinPeriodID, PXComp.LT),
				new PXDataFieldOrder<FinPeriod.finPeriodID>(true)))
			{
				return rec != null ? rec.GetString(0) : null;
			}
		}

		public virtual void RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
		{
			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Insert && e.TranStatus == PXTranStatus.Open)
			{
				GLHistory hist = e.Row as GLHistory;

				if (hist != null && hist.AccountID == reacct && hist.FinPeriodID.Substring(4, 2) == "01" && hist.YearClosed != true)
				{
					string PrevPeriodID = PreviousPeriod(hist.FinPeriodID);

					PXUpdateJoin<
						Set<AcctHist.yearClosed, True,
						Set<AcctHist.curyFinBegBalance, Add<AcctHist.curyFinBegBalance, IsNull<AcctHist2.curyFinYtdBalance, decimal0>>,
						Set<AcctHist.curyFinYtdBalance, Add<AcctHist.curyFinYtdBalance, IsNull<AcctHist2.curyFinYtdBalance, decimal0>>,
						Set<AcctHist.curyTranBegBalance, Add<AcctHist.curyTranBegBalance, IsNull<AcctHist2.curyTranYtdBalance, decimal0>>,
						Set<AcctHist.curyTranYtdBalance, Add<AcctHist.curyTranYtdBalance, IsNull<AcctHist2.curyTranYtdBalance, decimal0>>,
						Set<AcctHist.finBegBalance, Add<AcctHist.finBegBalance, IsNull<AcctHist2.finYtdBalance, decimal0>>,
						Set<AcctHist.finYtdBalance, Add<AcctHist.finYtdBalance, IsNull<AcctHist2.finYtdBalance, decimal0>>,
						Set<AcctHist.tranBegBalance, Add<AcctHist.tranBegBalance, IsNull<AcctHist2.tranYtdBalance, decimal0>>,
						Set<AcctHist.tranYtdBalance, Add<AcctHist.tranYtdBalance, IsNull<AcctHist2.tranYtdBalance, decimal0>>>>>>>>>>>,
					AcctHist,
					LeftJoin<GLHistoryByPeriod,
						On<GLHistoryByPeriod.ledgerID, Equal<AcctHist.ledgerID>,
						And<GLHistoryByPeriod.branchID, Equal<AcctHist.branchID>,
						And<GLHistoryByPeriod.accountID, Equal<Required<Account.accountID>>,
						And<GLHistoryByPeriod.subID, Equal<AcctHist.subID>,
						And<GLHistoryByPeriod.finPeriodID, Equal<Required<FinPeriod.finPeriodID>>>>>>>,
					LeftJoin<AcctHist3, //Retained Earnings 
						On<AcctHist3.ledgerID, Equal<AcctHist.ledgerID>,
						And<AcctHist3.branchID, Equal<AcctHist.branchID>, 
						And<AcctHist3.accountID, Equal<AcctHist.accountID>,
						And<AcctHist3.subID, Equal<AcctHist.subID>,
						And<Substring<AcctHist3.finPeriodID, int1, int4>, Greater<Substring<GLHistoryByPeriod.lastActivityPeriod,int1, int4>>, 
						And<AcctHist3.finPeriodID, LessEqual<Required<FinPeriod.finPeriodID>>>>>>>>,
					LeftJoin<AcctHist2, //Net Income
						On<AcctHist2.ledgerID, Equal<GLHistoryByPeriod.ledgerID>,
						And<AcctHist2.branchID, Equal<GLHistoryByPeriod.branchID>,
						And<AcctHist2.accountID, Equal<GLHistoryByPeriod.accountID>,
						And<AcctHist2.subID, Equal<GLHistoryByPeriod.subID>,
						And<AcctHist2.finPeriodID, Equal<GLHistoryByPeriod.lastActivityPeriod>,
						And<AcctHist3.accountID, IsNull>>>>>>>>>,
					Where<AcctHist.accountID, Equal<Required<AcctHist.accountID>>,
						And<AcctHist.subID, Equal<Required<AcctHist.subID>>,
						And<AcctHist.ledgerID, Equal<Required<AcctHist.ledgerID>>,
						And<AcctHist.branchID, Equal<Required<AcctHist.branchID>>,
						And<AcctHist.finPeriodID, Equal<Required<AcctHist.finPeriodID>>,
						And<AcctHist.yearClosed, Equal<False>>>>>>>>.Update(sender.Graph, niacct, PrevPeriodID, PrevPeriodID, reacct, hist.SubID, hist.LedgerID, hist.BranchID, hist.FinPeriodID);
				}
			}
		}
	}

	[Serializable]
	[PXAccumulator]
	[PXHidden]
	public partial class ConsolHist : GLConsolHistory
	{
	}

	public class AcctHistDefaultAttribute : PXDefaultAttribute
	{
		public override PXEventSubscriberAttribute Clone(PXAttributeLevel attributeLevel)
		{
			if (attributeLevel == PXAttributeLevel.Item)
			{
				return this;
			}
			return base.Clone(attributeLevel);
		}
		public AcctHistDefaultAttribute(Type sourceType)
			: base(sourceType)
		{
		}
	}

	public class AcctHistDBDecimalAttribute : PXDBDecimalAttribute, IPXFieldDefaultingSubscriber
	{
		public override PXEventSubscriberAttribute Clone(PXAttributeLevel attributeLevel)
		{
			if (attributeLevel == PXAttributeLevel.Item)
			{
				return this;
			}
			return base.Clone(attributeLevel);
		}
		public AcctHistDBDecimalAttribute()
			: base(4)
		{
		}
		public virtual void FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = 0m;
		}
	}

	public class AcctHistDBIntAttribute : PXDBIntAttribute
	{
		public override PXEventSubscriberAttribute Clone(PXAttributeLevel attributeLevel)
		{
			if (attributeLevel == PXAttributeLevel.Item)
			{
				return this;
			}
			return base.Clone(attributeLevel);
		}
		public AcctHistDBIntAttribute()
		{
		}
	}

	public class AcctHistDBStringAttribute : PXDBStringAttribute
	{
		public override PXEventSubscriberAttribute Clone(PXAttributeLevel attributeLevel)
		{
			if (attributeLevel == PXAttributeLevel.Item)
			{
				return this;
			}
			return base.Clone(attributeLevel);
		}
		public AcctHistDBStringAttribute(int length)
			: base(length)
		{
		}
	}

	public class AcctHistDBTimestamp : PXDBTimestampAttribute
	{
		public override PXEventSubscriberAttribute Clone(PXAttributeLevel attributeLevel)
		{
			if (attributeLevel == PXAttributeLevel.Item)
			{
				return this;
			}
			return base.Clone(attributeLevel);
		}
		public AcctHistDBTimestamp()
		{
		}
	}

	[System.SerializableAttribute()]
	[PXHidden]
	[AHAccum]
	[PXDisableCloneAttributes()]
	[PXBreakInheritance()]
	public partial class AcctHist : GLHistory
	{
		#region BranchID
		[AcctHistDBInt(IsKey = true)]
		public override Int32? BranchID
		{
			get
			{
				return this._BranchID;
			}
			set
			{
				this._BranchID = value;
			}
		}
		#endregion
		#region LedgerID
		[AcctHistDBInt(IsKey = true)]
		public override Int32? LedgerID
		{
			get
			{
				return this._LedgerID;
			}
			set
			{
				this._LedgerID = value;
			}
		}
		#endregion
		#region AccountID
		[AcctHistDBInt(IsKey = true)]
		public override Int32? AccountID
		{
			get
			{
				return this._AccountID;
			}
			set
			{
				this._AccountID = value;
			}
		}
		#endregion
		#region SubID
		[AcctHistDBInt(IsKey = true)]
		public override Int32? SubID
		{
			get
			{
				return this._SubID;
			}
			set
			{
				this._SubID = value;
			}
		}
		#endregion
		#region FinPeriod
		[AcctHistDBString(6, IsFixed = true, IsKey = true)]
		public override String FinPeriodID
		{
			get
			{
				return this._FinPeriodID;
			}
			set
			{
				this._FinPeriodID = value;
			}
		}
		#endregion
		#region BalanceType
		[AcctHistDBString(1, IsFixed = true)]
		[AcctHistDefault(typeof(Ledger.balanceType))]
		public override String BalanceType
		{
			get
			{
				return this._BalanceType;
			}
			set
			{
				this._BalanceType = value;
			}
		}
		#endregion
		#region CuryID
		[AcctHistDBString(5, IsUnicode = true)]
		public override String CuryID
		{
			get
			{
				return this._CuryID;
			}
			set
			{
				this._CuryID = value;
			}
		}
		#endregion
		#region FinYear
		public new abstract class finYear : PX.Data.IBqlField
		{
		}
		public override String FinYear
		{
			[PXDependsOnFields(typeof(finPeriodID))]
			get
			{
				return FiscalPeriodUtils.FiscalYear(this._FinPeriodID);
			}
		}
		#endregion
		#region DetDeleted
		public new abstract class detDeleted : PX.Data.IBqlField
		{
		}
		#endregion
		#region YearClosed
		public new abstract class yearClosed : PX.Data.IBqlField { }
		#endregion
		#region FinPtdCredit
		[AcctHistDBDecimal]
		public override Decimal? FinPtdCredit
		{
			get
			{
				return this._FinPtdCredit;
			}
			set
			{
				this._FinPtdCredit = value;
			}
		}
		#endregion
		#region FinPtdDebit
		[AcctHistDBDecimal]
		public override Decimal? FinPtdDebit
		{
			get
			{
				return this._FinPtdDebit;
			}
			set
			{
				this._FinPtdDebit = value;
			}
		}
		#endregion
		#region FinYtdBalance
		public new abstract class finYtdBalance : IBqlField { }
		[AcctHistDBDecimal]
		public override Decimal? FinYtdBalance
		{
			get
			{
				return this._FinYtdBalance;
			}
			set
			{
				this._FinYtdBalance = value;
			}
		}
		#endregion
		#region FinBegBalance
		public new abstract class finBegBalance : IBqlField { }
		[AcctHistDBDecimal]
		public override Decimal? FinBegBalance
		{
			get
			{
				return this._FinBegBalance;
			}
			set
			{
				this._FinBegBalance = value;
			}
		}
		#endregion
		#region FinPtdRevalued
		[AcctHistDBDecimal]
		public override Decimal? FinPtdRevalued
		{
			get
			{
				return this._FinPtdRevalued;
			}
			set
			{
				this._FinPtdRevalued = value;
			}
		}
		#endregion
		#region TranPtdCredit
		[AcctHistDBDecimal]
		public override Decimal? TranPtdCredit
		{
			get
			{
				return this._TranPtdCredit;
			}
			set
			{
				this._TranPtdCredit = value;
			}
		}
		#endregion
		#region TranPtdDebit
		[AcctHistDBDecimal]
		public override Decimal? TranPtdDebit
		{
			get
			{
				return this._TranPtdDebit;
			}
			set
			{
				this._TranPtdDebit = value;
			}
		}
		#endregion
		#region TranYtdBalance
		public new abstract class tranYtdBalance : IBqlField { }
		[AcctHistDBDecimal]
		public override Decimal? TranYtdBalance
		{
			get
			{
				return this._TranYtdBalance;
			}
			set
			{
				this._TranYtdBalance = value;
			}
		}
		#endregion
		#region TranBegBalance
		public new abstract class tranBegBalance : IBqlField { }
		[AcctHistDBDecimal]
		public override Decimal? TranBegBalance
		{
			get
			{
				return this._TranBegBalance;
			}
			set
			{
				this._TranBegBalance = value;
			}
		}
		#endregion
		#region CuryFinPtdCredit
		[AcctHistDBDecimal]
		public override Decimal? CuryFinPtdCredit
		{
			get
			{
				return this._CuryFinPtdCredit;
			}
			set
			{
				this._CuryFinPtdCredit = value;
			}
		}
		#endregion
		#region CuryFinPtdDebit
		[AcctHistDBDecimal]
		public override Decimal? CuryFinPtdDebit
		{
			get
			{
				return this._CuryFinPtdDebit;
			}
			set
			{
				this._CuryFinPtdDebit = value;
			}
		}
		#endregion
		#region CuryFinYtdBalance
		public new abstract class curyFinYtdBalance : IBqlField { }
		[AcctHistDBDecimal]
		public override Decimal? CuryFinYtdBalance
		{
			get
			{
				return this._CuryFinYtdBalance;
			}
			set
			{
				this._CuryFinYtdBalance = value;
			}
		}
		#endregion
		#region CuryFinBegBalance
		public new abstract class curyFinBegBalance : IBqlField { }
		[AcctHistDBDecimal]
		public override Decimal? CuryFinBegBalance
		{
			get
			{
				return this._CuryFinBegBalance;
			}
			set
			{
				this._CuryFinBegBalance = value;
			}
		}
		#endregion
		#region CuryTranPtdCredit
		[AcctHistDBDecimal]
		public override Decimal? CuryTranPtdCredit
		{
			get
			{
				return this._CuryTranPtdCredit;
			}
			set
			{
				this._CuryTranPtdCredit = value;
			}
		}
		#endregion
		#region CuryTranPtdDebit
		[AcctHistDBDecimal]
		public override Decimal? CuryTranPtdDebit
		{
			get
			{
				return this._CuryTranPtdDebit;
			}
			set
			{
				this._CuryTranPtdDebit = value;
			}
		}
		#endregion
		#region CuryTranYtdBalance
		public new abstract class curyTranYtdBalance : IBqlField { }
		[AcctHistDBDecimal]
		public override Decimal? CuryTranYtdBalance
		{
			get
			{
				return this._CuryTranYtdBalance;
			}
			set
			{
				this._CuryTranYtdBalance = value;
			}
		}
		#endregion
		#region CuryTranBegBalance
		public new abstract class curyTranBegBalance : IBqlField { }
		[AcctHistDBDecimal]
		public override Decimal? CuryTranBegBalance
		{
			get
			{
				return this._CuryTranBegBalance;
			}
			set
			{
				this._CuryTranBegBalance = value;
			}
		}
		#endregion
		#region FinFlag
		public override bool? FinFlag
		{
			get
			{
				return this._FinFlag;
			}
			set
			{
				this._FinFlag = value;
			}
		}
		#endregion
		#region PtdCredit
		public override Decimal? PtdCredit
		{
			get
			{
				return ((bool)_FinFlag) ? this._FinPtdCredit : this._TranPtdCredit;
			}
			set
			{
				if ((bool)_FinFlag)
				{
					this._FinPtdCredit = value;
				}
				else
				{
					this._TranPtdCredit = value;
				}
			}
		}
		#endregion
		#region PtdDebit
		public override Decimal? PtdDebit
		{
			get
			{
				return ((bool)_FinFlag) ? this._FinPtdDebit : this._TranPtdDebit;
			}
			set
			{
				if ((bool)_FinFlag)
				{
					this._FinPtdDebit = value;
				}
				else
				{
					this._TranPtdDebit = value;
				}
			}
		}
		#endregion
		#region YtdBalance
		public override Decimal? YtdBalance
		{
			get
			{
				return ((bool)_FinFlag) ? this._FinYtdBalance : this._TranYtdBalance;
			}
			set
			{
				if ((bool)_FinFlag)
				{
					this._FinYtdBalance = value;
				}
				else
				{
					this._TranYtdBalance = value;
				}
			}
		}
		#endregion
		#region BegBalance
		public override Decimal? BegBalance
		{
			get
			{
				return ((bool)_FinFlag) ? this._FinBegBalance : this._TranBegBalance;
			}
			set
			{
				if ((bool)_FinFlag)
				{
					this._FinBegBalance = value;
				}
				else
				{
					this._TranBegBalance = value;
				}
			}
		}
		#endregion
		#region PtdRevalued
		public override Decimal? PtdRevalued
		{
			get
			{
				return ((bool)_FinFlag) ? this._FinPtdRevalued : null;
			}
			set
			{
				if ((bool)_FinFlag)
				{
					this._FinPtdRevalued = value;
				}
			}
		}
		#endregion
		#region CuryPtdCredit
		public override Decimal? CuryPtdCredit
		{
			get
			{
				return ((bool)_FinFlag) ? this._CuryFinPtdCredit : this._CuryTranPtdCredit;
			}
			set
			{
				if ((bool)_FinFlag)
				{
					this._CuryFinPtdCredit = value;
				}
				else
				{
					this._CuryTranPtdCredit = value;
				}
			}
		}
		#endregion
		#region CuryPtdDebit
		public override Decimal? CuryPtdDebit
		{
			get
			{
				return ((bool)_FinFlag) ? this._CuryFinPtdDebit : this._CuryTranPtdDebit;
			}
			set
			{
				if ((bool)_FinFlag)
				{
					this._CuryFinPtdDebit = value;
				}
				else
				{
					this._CuryTranPtdDebit = value;
				}
			}
		}
		#endregion
		#region CuryYtdBalance
		public override Decimal? CuryYtdBalance
		{
			get
			{
				return ((bool)_FinFlag) ? this._CuryFinYtdBalance : this._CuryTranYtdBalance;
			}
			set
			{
				if ((bool)_FinFlag)
				{
					this._CuryFinYtdBalance = value;
				}
				else
				{
					this._CuryTranYtdBalance = value;
				}
			}
		}
		#endregion
		#region CuryBegBalance
		public override Decimal? CuryBegBalance
		{
			get
			{
				return ((bool)_FinFlag) ? this._CuryFinBegBalance : this._CuryTranBegBalance;
			}
			set
			{
				if ((bool)_FinFlag)
				{
					this._CuryFinBegBalance = value;
				}
				else
				{
					this._CuryTranBegBalance = value;
				}
			}
		}
		#endregion
		#region tstamp
		[AcctHistDBTimestamp]
		public override Byte[] tstamp
		{
			get
			{
				return this._tstamp;
			}
			set
			{
				this._tstamp = value;
			}
		}
		#endregion
	}

	[System.SerializableAttribute()]
	[PXHidden]
	[PXBreakInheritance()]
	public partial class AcctHist2 : GLHistory
	{
		#region LedgerID
		public new abstract class ledgerID : PX.Data.IBqlField
		{
		}
		#endregion
		#region BranchID
		public new abstract class branchID : PX.Data.IBqlField
		{
		}
		#endregion
		#region AccountID
		public new abstract class accountID : PX.Data.IBqlField
		{
		}
		#endregion
		#region SubID
		public new abstract class subID : PX.Data.IBqlField
		{
		}
		#endregion
		#region FinPeriod
		public new abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryID
		public new abstract class curyID : PX.Data.IBqlField
		{
		}
		#endregion
		#region DetDeleted
		public new abstract class detDeleted : PX.Data.IBqlField
		{
		}
		#endregion
		#region YearClosed
		public new abstract class yearClosed : PX.Data.IBqlField
		{
		}
		#endregion
		#region FinPtdCredit
		public new abstract class finPtdCredit : PX.Data.IBqlField
		{
		}
		#endregion
		#region FinPtdDebit
		public new abstract class finPtdDebit : PX.Data.IBqlField
		{
		}
		#endregion
		#region FinYtdBalance
		public new abstract class finYtdBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region FinBegBalance
		public new abstract class finBegBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region FinPtdRevalued
		public new abstract class finPtdRevalued : PX.Data.IBqlField
		{
		}
		#endregion
		#region TranPtdCredit
		public new abstract class tranPtdCredit : PX.Data.IBqlField
		{
		}
		#endregion
		#region TranPtdDebit
		public new abstract class tranPtdDebit : PX.Data.IBqlField
		{
		}
		#endregion
		#region TranYtdBalance
		public new abstract class tranYtdBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region TranBegBalance
		public new abstract class tranBegBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryFinPtdCredit
		public new abstract class curyFinPtdCredit : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryFinPtdDebit
		public new abstract class curyFinPtdDebit : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryFinYtdBalance
		public new abstract class curyFinYtdBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryFinBegBalance
		public new abstract class curyFinBegBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryTranPtdCredit
		public new abstract class curyTranPtdCredit : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryTranPtdDebit
		public new abstract class curyTranPtdDebit : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryTranYtdBalance
		public new abstract class curyTranYtdBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryTranBegBalance
		public new abstract class curyTranBegBalance : PX.Data.IBqlField
		{
		}
		#endregion
	}

	[System.SerializableAttribute()]
	[PXHidden]
	[PXBreakInheritance()]
	public partial class AcctHist3 : GLHistory
	{
		#region LedgerID
		public new abstract class ledgerID : PX.Data.IBqlField
		{
		}
		#endregion
		#region BranchID
		public new abstract class branchID : PX.Data.IBqlField
		{
		}
		#endregion
		#region AccountID
		public new abstract class accountID : PX.Data.IBqlField
		{
		}
		#endregion
		#region SubID
		public new abstract class subID : PX.Data.IBqlField
		{
		}
		#endregion
		#region FinPeriod
		public new abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryID
		public new abstract class curyID : PX.Data.IBqlField
		{
		}
		#endregion
		#region DetDeleted
		public new abstract class detDeleted : PX.Data.IBqlField
		{
		}
		#endregion
		#region YearClosed
		public new abstract class yearClosed : PX.Data.IBqlField
		{
		}
		#endregion
		#region FinPtdCredit
		public new abstract class finPtdCredit : PX.Data.IBqlField
		{
		}
		#endregion
		#region FinPtdDebit
		public new abstract class finPtdDebit : PX.Data.IBqlField
		{
		}
		#endregion
		#region FinYtdBalance
		public new abstract class finYtdBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region FinBegBalance
		public new abstract class finBegBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region FinPtdRevalued
		public new abstract class finPtdRevalued : PX.Data.IBqlField
		{
		}
		#endregion
		#region TranPtdCredit
		public new abstract class tranPtdCredit : PX.Data.IBqlField
		{
		}
		#endregion
		#region TranPtdDebit
		public new abstract class tranPtdDebit : PX.Data.IBqlField
		{
		}
		#endregion
		#region TranYtdBalance
		public new abstract class tranYtdBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region TranBegBalance
		public new abstract class tranBegBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryFinPtdCredit
		public new abstract class curyFinPtdCredit : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryFinPtdDebit
		public new abstract class curyFinPtdDebit : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryFinYtdBalance
		public new abstract class curyFinYtdBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryFinBegBalance
		public new abstract class curyFinBegBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryTranPtdCredit
		public new abstract class curyTranPtdCredit : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryTranPtdDebit
		public new abstract class curyTranPtdDebit : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryTranYtdBalance
		public new abstract class curyTranYtdBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryTranBegBalance
		public new abstract class curyTranBegBalance : PX.Data.IBqlField
		{
		}
		#endregion
	}	

	[Obsolete("CuryAcctHist is not used. It will be removed in the later versions.")]
	[System.SerializableAttribute()]
	[PXHidden]
	[AHAccum]
	[PXDisableCloneAttributes()]
	[PXBreakInheritance()]
	public partial class CuryAcctHist : CuryGLHistory
	{
		#region BranchID
		[AcctHistDBInt(IsKey = true)]
		public override Int32? BranchID
		{
			get
			{
				return this._BranchID;
			}
			set
			{
				this._BranchID = value;
			}
		}
		#endregion
		#region LedgerID
		[AcctHistDBInt(IsKey = true)]
		public override Int32? LedgerID
		{
			get
			{
				return this._LedgerID;
			}
			set
			{
				this._LedgerID = value;
			}
		}
		#endregion
		#region AccountID
		[AcctHistDBInt(IsKey = true)]
		public override Int32? AccountID
		{
			get
			{
				return this._AccountID;
			}
			set
			{
				this._AccountID = value;
			}
		}
		#endregion
		#region SubID
		[AcctHistDBInt(IsKey = true)]
		public override Int32? SubID
		{
			get
			{
				return this._SubID;
			}
			set
			{
				this._SubID = value;
			}
		}
		#endregion
		#region CuryID
		[AcctHistDBString(5, IsUnicode = true, IsKey = true)]
		public override String CuryID
		{
			get
			{
				return this._CuryID;
			}
			set
			{
				this._CuryID = value;
			}
		}
		#endregion
		#region FinPeriod
		[AcctHistDBString(6, IsFixed = true, IsKey = true)]
		public override String FinPeriodID
		{
			get
			{
				return this._FinPeriodID;
			}
			set
			{
				this._FinPeriodID = value;
			}
		}
		#endregion
		#region BalanceType
		[AcctHistDBString(1, IsFixed = true)]
		[AcctHistDefault(typeof(Ledger.balanceType))]
		public override String BalanceType
		{
			get
			{
				return this._BalanceType;
			}
			set
			{
				this._BalanceType = value;
			}
		}
		#endregion
		#region BaseCuryID
		[AcctHistDBString(5, IsUnicode = true)]
		public override String BaseCuryID
		{
			get
			{
				return this._BaseCuryID;
			}
			set
			{
				this._BaseCuryID = value;
			}
		}
		#endregion
		#region FinYear
		public new abstract class finYear : PX.Data.IBqlField
		{
		}
		public override String FinYear
		{
			[PXDependsOnFields(typeof(finPeriodID))]
			get
			{
				return (_FinPeriodID == null) ? null : FiscalPeriodUtils.FiscalYear(this._FinPeriodID);
			}
		}
		#endregion
		#region DetDeleted
		public new abstract class detDeleted : PX.Data.IBqlField
		{
		}
		#endregion
		#region FinPtdCredit
		[AcctHistDBDecimal]
		public override Decimal? FinPtdCredit
		{
			get
			{
				return this._FinPtdCredit;
			}
			set
			{
				this._FinPtdCredit = value;
			}
		}
		#endregion
		#region FinPtdDebit
		[AcctHistDBDecimal]
		public override Decimal? FinPtdDebit
		{
			get
			{
				return this._FinPtdDebit;
			}
			set
			{
				this._FinPtdDebit = value;
			}
		}
		#endregion
		#region FinYtdBalance
		[AcctHistDBDecimal]
		public override Decimal? FinYtdBalance
		{
			get
			{
				return this._FinYtdBalance;
			}
			set
			{
				this._FinYtdBalance = value;
			}
		}
		#endregion
		#region FinBegBalance
		[AcctHistDBDecimal]
		public override Decimal? FinBegBalance
		{
			get
			{
				return this._FinBegBalance;
			}
			set
			{
				this._FinBegBalance = value;
			}
		}
		#endregion
		#region FinPtdRevalued
		public override Decimal? FinPtdRevalued
		{
			get
			{
				return this._FinPtdRevalued;
			}
			set
			{
				this._FinPtdRevalued = value;
			}
		}
		#endregion
		#region TranPtdCredit
		[AcctHistDBDecimal]
		public override Decimal? TranPtdCredit
		{
			get
			{
				return this._TranPtdCredit;
			}
			set
			{
				this._TranPtdCredit = value;
			}
		}
		#endregion
		#region TranPtdDebit
		[AcctHistDBDecimal]
		public override Decimal? TranPtdDebit
		{
			get
			{
				return this._TranPtdDebit;
			}
			set
			{
				this._TranPtdDebit = value;
			}
		}
		#endregion
		#region TranYtdBalance
		[AcctHistDBDecimal]
		public override Decimal? TranYtdBalance
		{
			get
			{
				return this._TranYtdBalance;
			}
			set
			{
				this._TranYtdBalance = value;
			}
		}
		#endregion
		#region TranBegBalance
		[AcctHistDBDecimal]
		public override Decimal? TranBegBalance
		{
			get
			{
				return this._TranBegBalance;
			}
			set
			{
				this._TranBegBalance = value;
			}
		}
		#endregion
		#region CuryFinPtdCredit
		[AcctHistDBDecimal]
		public override Decimal? CuryFinPtdCredit
		{
			get
			{
				return this._CuryFinPtdCredit;
			}
			set
			{
				this._CuryFinPtdCredit = value;
			}
		}
		#endregion
		#region CuryFinPtdDebit
		[AcctHistDBDecimal]
		public override Decimal? CuryFinPtdDebit
		{
			get
			{
				return this._CuryFinPtdDebit;
			}
			set
			{
				this._CuryFinPtdDebit = value;
			}
		}
		#endregion
		#region CuryFinYtdBalance
		[AcctHistDBDecimal]
		public override Decimal? CuryFinYtdBalance
		{
			get
			{
				return this._CuryFinYtdBalance;
			}
			set
			{
				this._CuryFinYtdBalance = value;
			}
		}
		#endregion
		#region CuryFinBegBalance
		[AcctHistDBDecimal]
		public override Decimal? CuryFinBegBalance
		{
			get
			{
				return this._CuryFinBegBalance;
			}
			set
			{
				this._CuryFinBegBalance = value;
			}
		}
		#endregion
		#region CuryTranPtdCredit
		[AcctHistDBDecimal]
		public override Decimal? CuryTranPtdCredit
		{
			get
			{
				return this._CuryTranPtdCredit;
			}
			set
			{
				this._CuryTranPtdCredit = value;
			}
		}
		#endregion
		#region CuryTranPtdDebit
		[AcctHistDBDecimal]
		public override Decimal? CuryTranPtdDebit
		{
			get
			{
				return this._CuryTranPtdDebit;
			}
			set
			{
				this._CuryTranPtdDebit = value;
			}
		}
		#endregion
		#region CuryTranYtdBalance
		[AcctHistDBDecimal]
		public override Decimal? CuryTranYtdBalance
		{
			get
			{
				return this._CuryTranYtdBalance;
			}
			set
			{
				this._CuryTranYtdBalance = value;
			}
		}
		#endregion
		#region CuryTranBegBalance
		[AcctHistDBDecimal]
		public override Decimal? CuryTranBegBalance
		{
			get
			{
				return this._CuryTranBegBalance;
			}
			set
			{
				this._CuryTranBegBalance = value;
			}
		}
		#endregion
		#region FinFlag
		public override bool? FinFlag
		{
			get
			{
				return this._FinFlag;
			}
			set
			{
				this._FinFlag = value;
			}
		}
		#endregion
		#region PtdCredit
		public override Decimal? PtdCredit
		{
			get
			{
				return ((bool)_FinFlag) ? this._FinPtdCredit : this._TranPtdCredit;
			}
			set
			{
				if ((bool)_FinFlag)
				{
					this._FinPtdCredit = value;
				}
				else
				{
					this._TranPtdCredit = value;
				}
			}
		}
		#endregion
		#region PtdDebit
		public override Decimal? PtdDebit
		{
			get
			{
				return ((bool)_FinFlag) ? this._FinPtdDebit : this._TranPtdDebit;
			}
			set
			{
				if ((bool)_FinFlag)
				{
					this._FinPtdDebit = value;
				}
				else
				{
					this._TranPtdDebit = value;
				}
			}
		}
		#endregion
		#region YtdBalance
		public override Decimal? YtdBalance
		{
			get
			{
				return ((bool)_FinFlag) ? this._FinYtdBalance : this._TranYtdBalance;
			}
			set
			{
				if ((bool)_FinFlag)
				{
					this._FinYtdBalance = value;
				}
				else
				{
					this._TranYtdBalance = value;
				}
			}
		}
		#endregion
		#region BegBalance
		public override Decimal? BegBalance
		{
			get
			{
				return ((bool)_FinFlag) ? this._FinBegBalance : this._TranBegBalance;
			}
			set
			{
				if ((bool)_FinFlag)
				{
					this._FinBegBalance = value;
				}
				else
				{
					this._TranBegBalance = value;
				}
			}
		}
		#endregion
		#region PtdRevalued
		public override Decimal? PtdRevalued
		{
			get
			{
				return ((bool)_FinFlag) ? this._FinPtdRevalued : null;
			}
			set
			{
				if ((bool)_FinFlag)
				{
					this._FinPtdRevalued = value;
				}
			}
		}
		#endregion
		#region CuryPtdCredit
		public override Decimal? CuryPtdCredit
		{
			get
			{
				return ((bool)_FinFlag) ? this._CuryFinPtdCredit : this._CuryTranPtdCredit;
			}
			set
			{
				if ((bool)_FinFlag)
				{
					this._CuryFinPtdCredit = value;
				}
				else
				{
					this._CuryTranPtdCredit = value;
				}
			}
		}
		#endregion
		#region CuryPtdDebit
		public override Decimal? CuryPtdDebit
		{
			get
			{
				return ((bool)_FinFlag) ? this._CuryFinPtdDebit : this._CuryTranPtdDebit;
			}
			set
			{
				if ((bool)_FinFlag)
				{
					this._CuryFinPtdDebit = value;
				}
				else
				{
					this._CuryTranPtdDebit = value;
				}
			}
		}
		#endregion
		#region CuryYtdBalance
		public override Decimal? CuryYtdBalance
		{
			get
			{
				return ((bool)_FinFlag) ? this._CuryFinYtdBalance : this._CuryTranYtdBalance;
			}
			set
			{
				if ((bool)_FinFlag)
				{
					this._CuryFinYtdBalance = value;
				}
				else
				{
					this._CuryTranYtdBalance = value;
				}
			}
		}
		#endregion
		#region CuryBegBalance
		public override Decimal? CuryBegBalance
		{
			get
			{
				return ((bool)_FinFlag) ? this._CuryFinBegBalance : this._CuryTranBegBalance;
			}
			set
			{
				if ((bool)_FinFlag)
				{
					this._CuryFinBegBalance = value;
				}
				else
				{
					this._CuryTranBegBalance = value;
				}
			}
		}
		#endregion
		#region tstamp
		[AcctHistDBTimestamp]
		public override Byte[] tstamp
		{
			get
			{
				return this._tstamp;
			}
			set
			{
				this._tstamp = value;
			}
		}
		#endregion
	}
}
