using System;
using System.Collections.Generic;
using System.Linq;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.Common.Extensions;
using PX.Objects.Common.Tools;
using PX.Objects.GL.Reclassification.Common;

namespace PX.Objects.GL.Reclassification.Processing
{
	public class ReclassifyTransactionsProcessor : ReclassifyTransactionsBase<ReclassifyTransactionsProcessor>
	{
		public PXSelect<GLTranForReclassification> GLTranForReclass;

		protected JournalEntryForReclassification JournalEntryInstance;

		public void ProcessTransForReclassification(List<GLTranForReclassification> origTransForReclass, ReclassGraphState state)
		{
			State = state;

			TransGroupKey? transGroupKeyToPutToExistingBatch = null;
			JournalEntryInstance = CreateInstance<JournalEntryForReclassification>();
			var hasError = false;
			Batch batchForEditing = null;
			IReadOnlyCollection<GLTranForReclassTriple> transForReclassTripleGroupsToBatchForEditing = null;

			var workOrigTranMap = origTransForReclass.ToDictionary(origTran => PXCache<GLTranForReclassification>.CreateCopy(origTran),
					origTran => origTran);

			var transForReclass = workOrigTranMap.Keys.ToList();

			if (State.ReclassScreenMode == ReclassScreenMode.Editing)
			{
				transGroupKeyToPutToExistingBatch = DefineTransGroupKeyToPutToExistingBatch(transForReclass);
			}

			PrepareJournalEntryGraph();

			var transForReclassTriples = BuildTranForReclasTriples(transForReclass);

			var transForReclassTripleGroups = transForReclassTriples.GroupBy(tranWithCury =>
				new TransGroupKey()
				{
					FinPeriodID = tranWithCury.Tran.FinPeriodID,
					CuryID = tranWithCury.CuryInfo.CuryID
				});

			foreach (var tranCuryPairsGroup in transForReclassTripleGroups)
			{
				var tranForReclassTriples = tranCuryPairsGroup.ToArray();

				if (State.ReclassScreenMode == ReclassScreenMode.Editing
					&& tranCuryPairsGroup.Key.Equals(transGroupKeyToPutToExistingBatch.Value))
				{
					batchForEditing = JournalEntry.FindBatch(JournalEntryInstance, State.EditingBatchModule, State.EditingBatchNbr);
					transForReclassTripleGroupsToBatchForEditing = tranForReclassTriples;

					//existing batch must be processed in the end
					continue;
				}

				hasError |= !ProcessTranForReclassGroup(tranForReclassTriples,
														workOrigTranMap,
														origTransForReclass);
			}

			if (State.ReclassScreenMode == ReclassScreenMode.Editing)
			{
				hasError |= !ProcessTranForReclassGroup(transForReclassTripleGroupsToBatchForEditing,
														workOrigTranMap,
														origTransForReclass,
														batchForEditing);
			}

			if (hasError)
				throw new PXException(ErrorMessages.SeveralItemsFailed);
		}

		private void PrepareJournalEntryGraph()
		{
			if (State.ReclassScreenMode == ReclassScreenMode.Editing)
			{
				var transForEditing = JournalEntry.GetTrans(JournalEntryInstance, State.EditingBatchModule, State.EditingBatchNbr);

				foreach (var tran in transForEditing)
				{
					JournalEntryInstance.GLTranModuleBatNbr.Cache.SetStatus(tran, PXEntryStatus.Held);
				}
			}

			JournalEntryInstance.glsetup.Current.RequireControlTotal = false;
		}


		private bool ProcessTranForReclassGroup(IReadOnlyCollection<GLTranForReclassTriple> tranForReclassTriples,
												Dictionary<GLTranForReclassification, GLTranForReclassification> workOrigTranMap,
												List<GLTranForReclassification> origTransForReclass,
												Batch batchForEdit = null)
		{
			Batch reclassBatch;
			List<GLTran> transMovedFromExistingBatch = new List<GLTran>();
			try
			{
				using (PXTransactionScope ts = new PXTransactionScope())
				{
					reclassBatch = BuildReclassificationBatch(tranForReclassTriples, transMovedFromExistingBatch, batchForEdit);

					JournalEntryInstance.Actions.PressSave();

					foreach (var tran in transMovedFromExistingBatch)
					{
						JournalEntryInstance.GLTranModuleBatNbr.Delete(tran);
					}

					JournalEntryInstance.Actions.PressSave();

					foreach (var tranForReclassTriple in tranForReclassTriples)
					{
						var srcTran = tranForReclassTriple.Tran;

						PXUpdate<
							Set<GLTran.reclassBatchNbr, Required<GLTran.reclassBatchNbr>,
								Set<GLTran.reclassBatchModule, Required<GLTran.reclassBatchModule>>>,
								GLTran,
							Where<GLTran.module, Equal<Required<GLTran.module>>,
								And<GLTran.batchNbr, Equal<Required<GLTran.batchNbr>>,
								And<GLTran.lineNbr, Equal<Required<GLTran.lineNbr>>>>>>
							.Update(this, reclassBatch.BatchNbr, reclassBatch.Module,
											srcTran.Module, srcTran.BatchNbr, srcTran.LineNbr);
					}

					ts.Complete();

					foreach (var tranCuryPair in tranForReclassTriples)
					{
						var origTran = workOrigTranMap[tranCuryPair.Tran];

						origTran.ReclassBatchModule = reclassBatch.Module;
						origTran.ReclassBatchNbr = reclassBatch.BatchNbr;
					}

					if (batchForEdit != null)
					{
						State.GLTranForReclassToDelete.Clear();
					}
				}
			}
			catch (Exception ex)
			{
				var exMessage = GetExceptionMessage(ex);

				var message =
					string.Concat(PXMessages.LocalizeNoPrefix(Messages.ReclassificationBatchHasNotBeenCreatedForTheTransaction),
						Environment.NewLine,
						exMessage);

				foreach (var tranCuryPair in tranForReclassTriples)
				{
					GLTranForReclass.Cache.RestoreCopy(tranCuryPair.Tran, workOrigTranMap[tranCuryPair.Tran]);

					PXProcessing<GLTranForReclassification>.SetError(origTransForReclass.IndexOf(workOrigTranMap[tranCuryPair.Tran]),
						message);
				}

				JournalEntryInstance.Clear();
				PrepareJournalEntryGraph();

				return false;
			}

			if (GLSetup.Current.AutoReleaseReclassBatch == true
				&& batchForEdit == null)
			{
				try
				{
					JournalEntry.ReleaseBatch(new[] { reclassBatch }, externalPostList: null, unholdBatch: true);
				}
				catch (Exception ex)
				{
					var exMessage = GetExceptionMessage(ex);

					var message = string.Concat(
						PXMessages.LocalizeNoPrefix(Messages.ReclassificationBatchGeneratedForThisTransactionHasNotBeenReleasedOrPosted),
						Environment.NewLine,
						exMessage);

					foreach (var tranCuryPair in tranForReclassTriples)
					{
						PXProcessing<GLTranForReclassification>.SetError(
							origTransForReclass.IndexOf(workOrigTranMap[tranCuryPair.Tran]), message);
					}

					return false;
				}
			}

			foreach (var tranCuryPair in tranForReclassTriples)
			{
				PXProcessing<GLTranForReclassification>.SetInfo(origTransForReclass.IndexOf(workOrigTranMap[tranCuryPair.Tran]),
					ActionsMessages.RecordProcessed);
			}

			return true;
		}

		private TransGroupKey DefineTransGroupKeyToPutToExistingBatch(IReadOnlyCollection<GLTranForReclassification> transForReclass)
		{
			string minFinPeriodIDForEditing = null;
			string minFinPeriodIDForBatchCury = null;

			string minFinPeriodID = null;
			string minCuryIDForMinFinPeriodID = null;

			foreach (var tranForReclass in transForReclass)
			{
				if (tranForReclass.ReclassRowType == ReclassRowTypes.Editing)
				{
					if (minFinPeriodIDForEditing == null ||
						string.CompareOrdinal(tranForReclass.FinPeriodID, minFinPeriodIDForEditing) < 0)
					{
						minFinPeriodIDForEditing = tranForReclass.FinPeriodID;
					}
				}

				if (tranForReclass.CuryID == State.EditingBatchCuryID)
				{
					if (minFinPeriodIDForBatchCury == null ||
						string.CompareOrdinal(tranForReclass.FinPeriodID, minFinPeriodIDForBatchCury) < 0)
					{
						minFinPeriodIDForBatchCury = tranForReclass.FinPeriodID;
					}
				}

				var compareRes = string.CompareOrdinal(tranForReclass.FinPeriodID, minFinPeriodID);
				if (minFinPeriodID == null || compareRes < 0)
				{
					minFinPeriodID = tranForReclass.FinPeriodID;
					minCuryIDForMinFinPeriodID = tranForReclass.CuryID;
				}
				if (compareRes == 0 && string.CompareOrdinal(tranForReclass.CuryID, minCuryIDForMinFinPeriodID) < 0)
				{
					minCuryIDForMinFinPeriodID = tranForReclass.CuryID;
				}
			}

			//Existing batch must be contain old trans with old or min period, or new trans with old curyID
			var groupDiscriminatorsByPriority = new TransGroupKeyDiscriminatorPair[3];

			groupDiscriminatorsByPriority[0] = new TransGroupKeyDiscriminatorPair(tranForReclass => tranForReclass.ReclassRowType == ReclassRowTypes.Editing && tranForReclass.FinPeriodID == State.EditingBatchFinPeriodID);
			groupDiscriminatorsByPriority[1] = new TransGroupKeyDiscriminatorPair(tranForReclass => tranForReclass.ReclassRowType == ReclassRowTypes.Editing && tranForReclass.FinPeriodID == minFinPeriodIDForEditing);
			groupDiscriminatorsByPriority[2] = new TransGroupKeyDiscriminatorPair(tranForReclass => tranForReclass.CuryID == State.EditingBatchCuryID && tranForReclass.FinPeriodID == minFinPeriodIDForBatchCury);

			foreach (var tranForReclass in transForReclass)
			{
				for (int i = 0; i < groupDiscriminatorsByPriority.Length; i++)
				{
					var discriminator = groupDiscriminatorsByPriority[i].Discriminator;
					if (discriminator(tranForReclass))
					{
						if (i == 0)
						{
							return new TransGroupKey()
							{
								FinPeriodID = tranForReclass.FinPeriodID,
								CuryID = tranForReclass.CuryID
							};
						}

						var curKey = groupDiscriminatorsByPriority[i].TransGroupKey;

						curKey.CuryID = tranForReclass.CuryID;
						curKey.FinPeriodID = tranForReclass.FinPeriodID;
					}
				}
			}

			for (int i = 1; i < groupDiscriminatorsByPriority.Length; i++)
			{
				if (groupDiscriminatorsByPriority[i].TransGroupKey.CuryID != null)
					return groupDiscriminatorsByPriority[i].TransGroupKey;
			}

			//All trans are new and with other curyID, take min by finPeriodID, CuryID
			return new TransGroupKey()
			{
				FinPeriodID = minFinPeriodID,
				CuryID = minCuryIDForMinFinPeriodID
			};
		}

		private List<GLTranForReclassTriple> BuildTranForReclasTriples(ICollection<GLTranForReclassification> transForReclass)
		{
			List<GLTranForReclassTriple> transForReclassWithCuryInfo = new List<GLTranForReclassTriple>();

			var transForReclassByCuryInfoID = transForReclass.GroupBy(tran => tran.CuryInfoID);

			foreach (var tranGroup in transForReclassByCuryInfoID)
			{
				var curyInfo = PXSelect<CurrencyInfo,
					Where<CurrencyInfo.curyInfoID, Equal<Required<CurrencyInfo.curyInfoID>>>>
					.Select(this, tranGroup.Key);

				var tranCuryPairs = tranGroup.Select(tran => new GLTranForReclassTriple(tran, curyInfo));

				transForReclassWithCuryInfo.AddRange(tranCuryPairs);
			}

			return transForReclassWithCuryInfo;
		}

		private Batch BuildReclassificationBatch(IReadOnlyCollection<GLTranForReclassTriple> transForReclassTriples,
			List<GLTran> transMovedFromExistingBatch,
			Batch batchForEditing = null)
		{
			DateTime earliestNewTranDate = DateTime.MaxValue;
			Batch batch;

			if (batchForEditing == null)
			{
				batch = JournalEntryInstance.BatchModule.Insert(new Batch());
			}
			else
			{
				batch = batchForEditing;
				JournalEntryInstance.BatchModule.Current = batch;
			}

			//adding or editing of transactions
			foreach (var transForReclassTriple in transForReclassTriples)
			{
				var tranForReclass = transForReclassTriple.Tran;

				if (tranForReclass.ReclassRowType == ReclassRowTypes.Editing)
				{
					EditReverseTran(tranForReclass, transMovedFromExistingBatch);
					transForReclassTriple.ReclassifyingTran = EditReclassifyingTran(tranForReclass, transMovedFromExistingBatch);
				}
				else
				{
					var tranCuryInfo = CreateCurrencyInfo(transForReclassTriple.CuryInfo);

					BuildReverseTran(tranForReclass, tranCuryInfo);
					transForReclassTriple.ReclassifyingTran = BuildReclassifyingTran(tranForReclass, tranCuryInfo);
				}

				if (earliestNewTranDate > transForReclassTriple.ReclassifyingTran.TranDate)
				{
					earliestNewTranDate = transForReclassTriple.ReclassifyingTran.TranDate.Value;
				}	
			}

			//remove deleted trans
			if (batchForEditing != null)
			{
				foreach (var tranForReclassToDel in State.GLTranForReclassToDelete)
				{
					var reverseTran = LocateReverseTran(JournalEntryInstance.GLTranModuleBatNbr.Cache, tranForReclassToDel);
					var reclassifyingTran = LocateReclassifyingTran(JournalEntryInstance.GLTranModuleBatNbr.Cache, tranForReclassToDel);

					JournalEntryInstance.GLTranModuleBatNbr.Delete(reverseTran);
					JournalEntryInstance.GLTranModuleBatNbr.Delete(reclassifyingTran);
				}
			}

			var representativeTranForReclass = transForReclassTriples.First().Tran;

			//creating and editing of batch header
			if (batchForEditing == null)
			{
				var batchCuryInfo = PXCache<CurrencyInfo>.CreateCopy(transForReclassTriples.First().CuryInfo);
				batchCuryInfo.CuryInfoID = null;
				batchCuryInfo = JournalEntryInstance.currencyinfo.Insert(batchCuryInfo);

				batch.BatchType = BatchTypeCode.Reclassification;
				batch.BranchID = Accessinfo.BranchID;
				batch.LedgerID = representativeTranForReclass.LedgerID;
				batch.Module = BatchModule.GL;
				batch.CuryInfoID = batchCuryInfo.CuryInfoID;
			}
			else
			{
				var firstCuryInfo = transForReclassTriples.GetItemWithMin(triple => triple.ReclassifyingTran.LineNbr.Value).CuryInfo;

				var batchCuryInfo = PXCache<CurrencyInfo>.CreateCopy(firstCuryInfo);
				batchCuryInfo.CuryInfoID = JournalEntryInstance.currencyInfo.CuryInfoID;
				batchCuryInfo.tstamp = JournalEntryInstance.currencyInfo.tstamp;

				JournalEntryInstance.currencyinfo.Update(batchCuryInfo);
			}

			batch.DateEntered = earliestNewTranDate;
			batch.FinPeriodID = representativeTranForReclass.FinPeriodID;
			batch.CuryID = representativeTranForReclass.CuryID;

			if (State.ReclassScreenMode == ReclassScreenMode.Reversing)
			{
				batch.OrigModule = State.OrigBatchModuleToReverse;
				batch.OrigBatchNbr = State.OrigBatchNbrToReverse;
				batch.AutoReverseCopy = true;
			}

			batch = JournalEntryInstance.BatchModule.Update(batch);

			return batch;
		}

		private GLTran BuildReclassifyingTran(GLTranForReclassification tranForReclass, CurrencyInfo tranCuryInfo)
		{
			var reclassifyingTran = JournalEntry.BuildReleasableTransaction(JournalEntryInstance, tranForReclass, JournalEntry.TranBuildingModes.SetLinkToOriginal, tranCuryInfo);

			SetReclassifyingTranBusinessAttrs(reclassifyingTran, tranForReclass);
			SetReclassificationLinkingAttrs(reclassifyingTran, tranForReclass);

			return JournalEntryInstance.GLTranModuleBatNbr.Insert(reclassifyingTran);
		}

		private GLTran EditReclassifyingTran(GLTranForReclassification tranForReclass, List<GLTran> transMovedFromExistingBatch)
		{
			var reclassifyingTran = LocateReclassifyingTran(JournalEntryInstance.GLTranModuleBatNbr.Cache, tranForReclass);

			var anyChanged = reclassifyingTran.BranchID != tranForReclass.NewBranchID
							 || reclassifyingTran.AccountID != tranForReclass.NewAccountID
							 || reclassifyingTran.SubID != tranForReclass.NewSubID
							 || reclassifyingTran.TranDate != tranForReclass.NewTranDate
							 || reclassifyingTran.TranDesc != tranForReclass.NewTranDesc;

			if (anyChanged)
			{
				var newTran = CopyTranForMovingIfNeed(reclassifyingTran, transMovedFromExistingBatch);

				if (newTran != null)
				{
					SetReclassifyingTranBusinessAttrs(newTran, tranForReclass);
					return JournalEntryInstance.GLTranModuleBatNbr.Insert(newTran);
				}

				SetReclassifyingTranBusinessAttrs(reclassifyingTran, tranForReclass);
				return JournalEntryInstance.GLTranModuleBatNbr.Update(reclassifyingTran);
			}

			return reclassifyingTran;
		}

		private GLTran CopyTranForMovingIfNeed(GLTran tran, List<GLTran> transMovedFromExistingBatch)
		{
			var batch = JournalEntryInstance.BatchModule.Current;

			if (batch.Module != tran.Module || batch.BatchNbr != tran.BatchNbr)
			{
				var oldTran = tran;
				tran = PXCache<GLTran>.CreateCopy(oldTran);

				tran.Module = null;
				tran.BatchNbr = null;
				tran.LineNbr = null;

				transMovedFromExistingBatch.Add(oldTran);

				return tran;
			}

			return null;
		}

		private void EditReverseTran(GLTranForReclassification tranForReclass, List<GLTran> transMovedFromExistingBatch)
		{
			var reverseTran = LocateReverseTran(JournalEntryInstance.GLTranModuleBatNbr.Cache, tranForReclass);

			var anyChanged = reverseTran.TranDate != tranForReclass.NewTranDate
							 || reverseTran.TranDesc != tranForReclass.NewTranDesc;

			if (anyChanged)
			{
				var newTran = CopyTranForMovingIfNeed(reverseTran, transMovedFromExistingBatch);

				if (newTran != null)
				{
					SetCommonBusinessAttrs(newTran, tranForReclass);
					JournalEntryInstance.GLTranModuleBatNbr.Insert(newTran);
				}
				else
				{
					SetCommonBusinessAttrs(reverseTran, tranForReclass);
					JournalEntryInstance.GLTranModuleBatNbr.Update(reverseTran);
				}
			}
		}

		private void BuildReverseTran(GLTranForReclassification tranForReclass, CurrencyInfo tranCuryInfo)
		{
			var reverseTran = JournalEntry.BuildReverseTran(JournalEntryInstance, tranForReclass, JournalEntry.TranBuildingModes.SetLinkToOriginal, tranCuryInfo);

			reverseTran.IsReclassReverse = true;
			SetCommonBusinessAttrs(reverseTran, tranForReclass);
			SetReclassificationLinkingAttrs(reverseTran, tranForReclass);

			JournalEntryInstance.GLTranModuleBatNbr.Insert(reverseTran);
		}

		private void SetCommonBusinessAttrs(GLTran tran, GLTranForReclassification tranForReclassification)
		{
			tran.TranDate = tranForReclassification.NewTranDate;
			tran.TranDesc = tranForReclassification.NewTranDesc;
		}

		private void SetReclassifyingTranBusinessAttrs(GLTran reclassifyingTran, GLTranForReclassification tranForReclass)
		{
			reclassifyingTran.BranchID = tranForReclass.NewBranchID;
			reclassifyingTran.AccountID = tranForReclass.NewAccountID;

			if (tranForReclass.NewSubCD != null)
			{
				JournalEntryInstance.GLTranModuleBatNbr.SetValueExt<GLTran.subID>(reclassifyingTran, tranForReclass.NewSubCD);
			}
			else
			{
				reclassifyingTran.SubID = tranForReclass.NewSubID;
			}

			SetCommonBusinessAttrs(reclassifyingTran, tranForReclass);
		}

		private void SetReclassificationLinkingAttrs(GLTran tran, GLTranForReclassification tranForReclassification)
		{
			if (JournalEntry.IsReclassifacationTran(tranForReclassification))
			{
				tran.ReclassSourceTranModule = tranForReclassification.ReclassSourceTranModule;
				tran.ReclassSourceTranBatchNbr = tranForReclassification.ReclassSourceTranBatchNbr;
				tran.ReclassSourceTranLineNbr = tranForReclassification.ReclassSourceTranLineNbr;
				tran.ReclassSeqNbr = tranForReclassification.ReclassSeqNbr + 1;
			}
			else
			{
				tran.ReclassSourceTranModule = tranForReclassification.Module;
				tran.ReclassSourceTranBatchNbr = tranForReclassification.BatchNbr;
				tran.ReclassSourceTranLineNbr = tranForReclassification.LineNbr;
				tran.ReclassSeqNbr = 1;
			}
		}

		private GLTran LocateReclassifyingTran(PXCache cache, GLTranForReclassification tranForReclass)
		{
			var reclassifyingTran = new GLTran()
			{
				Module = State.EditingBatchModule,
				BatchNbr = State.EditingBatchNbr,
				LineNbr = tranForReclass.EditingPairReclassifyingLineNbr
			};

			return (GLTran)cache.Locate(reclassifyingTran);
		}

		private GLTran LocateReverseTran(PXCache cache, GLTranForReclassification tranForReclass)
		{
			var reverseTran = new GLTran()
			{
				Module = State.EditingBatchModule,
				BatchNbr = State.EditingBatchNbr,
				LineNbr = tranForReclass.EditingPairReclassifyingLineNbr - 1
			};

			return (GLTran)cache.Locate(reverseTran);
		}


		#region Service Methods

		private CurrencyInfo CreateCurrencyInfo(CurrencyInfo curyInfo)
		{
			var info = PXCache<CurrencyInfo>.CreateCopy(curyInfo);

			info.CuryInfoID = null;
			info.IsReadOnly = true;
			info.BaseCalc = true;

			info = JournalEntryInstance.currencyinfo.Insert(info);

			return info;
		}

		private string GetExceptionMessage(Exception ex)
		{
			var outerEx = ex as PXOuterException;

			return outerEx != null
				? string.Concat(outerEx.Message, ";", string.Join(";", outerEx.InnerMessages))
				: ex.Message;
		}
		
		#endregion


		#region DTOs

		public struct TransGroupKey
		{
			public string FinPeriodID { get; set; }
			public string CuryID { get; set; }

			public override bool Equals(Object obj)
			{
				if (obj == null || GetType() != obj.GetType())
					return false;

				var p = (TransGroupKey)obj;
				return (FinPeriodID == p.FinPeriodID) && (CuryID == p.CuryID);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					int ret = 17;

					ret = ret * 23 + FinPeriodID.GetHashCode();
					return ret * 23 + CuryID.GetHashCode();
				}
			}
		}

		public class TransGroupKeyDiscriminatorPair
		{
			public TransGroupKey TransGroupKey { get; set; }
			public Func<GLTranForReclassification, bool> Discriminator { get; set; }

			public TransGroupKeyDiscriminatorPair(Func<GLTranForReclassification, bool> discriminator)
			{
				TransGroupKey = new TransGroupKey();
				Discriminator = discriminator;
			}
		}

		public class GLTranForReclassTriple
		{
			public readonly GLTranForReclassification Tran;
			public readonly CurrencyInfo CuryInfo;
			public GLTran ReclassifyingTran;

			public GLTranForReclassTriple(GLTranForReclassification tran, CurrencyInfo curyInfo)
			{
				Tran = tran;
				CuryInfo = curyInfo;
			}
		}
		
		#endregion
	}
}
