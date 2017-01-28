using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using PX.Api;
using PX.Common;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.GL.Reclassification.Common;
using PX.Objects.GL.Reclassification.Processing;
using PX.Objects.PM;
using PX.Web.UI;

namespace PX.Objects.GL.Reclassification.UI
{
	public class ReclassifyTransactionsProcess : ReclassifyTransactionsBase<ReclassifyTransactionsProcess>
	{
		public PXCancel<GLTranForReclassification> Cancel;
		public PXDelete<GLTranForReclassification> Delete;
		public PXAction<GLTranForReclassification> showLoadTransPopup;
		public PXAction<GLTranForReclassification> reloadTrans;
		public PXAction<GLTranForReclassification> loadTrans;
		public PXAction<GLTranForReclassification> replace;

		public PXProcessing<GLTranForReclassification> GLTranForReclass;

		public PXSelectJoin<GLTranForReclassification,
						InnerJoin<CurrencyInfo,
							On<GLTran.curyInfoID, Equal<CurrencyInfo.curyInfoID>>>,
						Where<GLTranForReclassification.module, Equal<Required<GLTranForReclassification.module>>,
							And<GLTranForReclassification.batchNbr, Equal<Required<GLTranForReclassification.batchNbr>>,
							And<GLTranForReclassification.lineNbr, Equal<Required<GLTranForReclassification.lineNbr>>,
							And<GLTranForReclassification.reclassBatchNbr, IsNull>>>>> 
						GLTranForReclassWithCuryInfo;

		public PXSelect<GLTranForReclassification,
						Where<GLTranForReclassification.module, Equal<Required<GLTranForReclassification.module>>,
							And<GLTranForReclassification.batchNbr, Equal<Required<GLTranForReclassification.batchNbr>>,
							And<GLTranForReclassification.lineNbr, Equal<Required<GLTranForReclassification.lineNbr>>>>>>
						GLTranForReclass_Module_BatchNbr_LineNbr;

		public PXSelect<GLTranForReclassification,
							Where<GLTranForReclassification.module, Equal<Required<GLTranForReclassification.module>>,
									And<GLTranForReclassification.batchNbr, Equal<Required<GLTranForReclassification.batchNbr>>,
									And<GLTranForReclassification.isReclassReverse, Equal<False>,
									And<GLTranForReclassification.isInterCompany, Equal<False>>>>>>
							GLTransForReclassForReverseView;

		public PXSelect<GLTran,
							Where<GLTran.module, Equal<Required<GLTran.module>>,
									And<GLTran.batchNbr, Equal<Required<GLTran.batchNbr>>,
									And<GLTran.isReclassReverse, Equal<True>>>>>
							ReclassReverseGLTransView;

		public PXFilter<LoadOptions> LoadOptionsView;
		public PXFilter<ReplaceOptions> ReplaceOptionsView;
		
		public PXSelect<BAccountR> BAccountRView;

		private const string ScheduleActionKey = "Schedule";

		private static string[] _emptyStringReplaceWildCards = {"\"\"", "''"};		

		protected IEnumerable<GLTranForReclassification> GetUpdatedTranForReclass()
		{
			return GLTranForReclass.Cache.Updated.OfType<GLTranForReclassification>();
		}

		protected static Type[] EditableFields = new[]
		{
			typeof (GLTranForReclassification.newBranchID),
			typeof (GLTranForReclassification.newAccountID),
			typeof (GLTranForReclassification.newSubID),
			typeof (GLTranForReclassification.newTranDate),
		};

		public ReclassifyTransactionsProcess()
		{
			Actions[ScheduleActionKey].SetVisible(false);

			GLTranForReclass.SetSelected<GLTranForReclassification.selected>();
			GLTranForReclass.SetProcessCaption(Messages.Process);
			GLTranForReclass.SetProcessAllVisible(false);

			PXUIFieldAttribute.SetVisible<GLTranForReclassification.refNbr>(GLTranForReclass.Cache, null, false);
			PXUIFieldAttribute.SetVisible<GLTranForReclassification.selected>(GLTranForReclass.Cache, null, true);

			PXUIFieldAttribute.SetVisibility<BAccountR.acctReferenceNbr>(BAccountRView.Cache, null, PXUIVisibility.Visible);
			PXUIFieldAttribute.SetVisibility<BAccountR.parentBAccountID>(BAccountRView.Cache, null, PXUIVisibility.Visible);
			PXUIFieldAttribute.SetVisibility<BAccountR.ownerID>(BAccountRView.Cache, null, PXUIVisibility.Visible);

			var opt = LoadOptionsView.Current;

			showLoadTransPopup.StateSelectingEvents += ButtonsFieldSelectingHandlerForDisableAfterProcess;
			loadTrans.StateSelectingEvents += ButtonsFieldSelectingHandlerForDisableAfterProcess;
			replace.StateSelectingEvents += DependingOnRowExistanceButtonsSelectingHandler;
			Delete.StateSelectingEvents += DependingOnRowExistanceButtonsSelectingHandler;
			reloadTrans.StateSelectingEvents += ReloadTransButtonStateSelectingHandler;
		}

		protected virtual IEnumerable glTranForReclass()
		{
			return GetUpdatedTranForReclass();
		}

		protected virtual void GLTranForReclassification_ReclassBatchNbr_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			if (e.Row == null)
				PXUIFieldAttribute.SetVisible<GLTranForReclassification.reclassBatchNbr>(
					GLTranForReclass.Cache, null,
					GetUpdatedTranForReclass().Any(row => row.ReclassBatchNbr != null));
		}
		#region Event Handlers

		#region GLTranForReclassification

		protected virtual void ReclassGraphState_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			var state = PXCache<ReclassGraphState>.CreateCopy(State);
			GLTranForReclass.SetProcessDelegate(delegate(List<GLTranForReclassification> transForReclass)
			{
				var graph = CreateInstance<ReclassifyTransactionsProcessor>();

				graph.ProcessTransForReclassification(transForReclass, state);
			});
		}

		protected virtual void GLTranForReclassification_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			var tran = e.Row as GLTranForReclassification;

			if (tran == null)
				return;

			var canEdit = !PXLongOperation.Exists(UID);

			PXUIFieldAttribute.SetEnabled<GLTranForReclassification.selected>(cache, tran, tran.Selected == true && canEdit);

			PXUIFieldAttribute.SetEnabled<GLTranForReclassification.newBranchID>(cache, tran, canEdit);
			PXUIFieldAttribute.SetEnabled<GLTranForReclassification.newAccountID>(cache, tran, canEdit);
			PXUIFieldAttribute.SetEnabled<GLTranForReclassification.newSubID>(cache, tran, canEdit);
			PXUIFieldAttribute.SetEnabled<GLTranForReclassification.newFinPeriodID>(cache, tran, canEdit);
			PXUIFieldAttribute.SetEnabled<GLTranForReclassification.newTranDate>(cache, tran, canEdit);
			PXUIFieldAttribute.SetEnabled<GLTranForReclassification.newTranDesc>(cache, tran, canEdit);

			if (!tran.VerifyingForFromValuesInvoked)
			{
				VerifyAndRememberErorrForNewFields(tran);

				tran.VerifyingForFromValuesInvoked = true;
			}

			SetErrorToNewFieldsIfNeed(tran);

			CalcAndSetSelectedFieldState(tran);
		}

		/// <summary>
		/// Setting of the error to "New" fields if they contain values are equal to invalid values from "From" fields.
		/// </summary>
		private void SetErrorToNewFieldsIfNeed(GLTranForReclassification tran)
		{
			foreach (var fieldNameErrorPair in tran.FieldsErrorForInvalidFromValues)
			{
				if (fieldNameErrorPair.Value != null)
				{
					object curValue = null;

					if (fieldNameErrorPair.Key == typeof(GLTranForReclassification.newBranchID).Name)
					{
						curValue = tran.NewBranchID;
					}
					else if (fieldNameErrorPair.Key == typeof(GLTranForReclassification.newAccountID).Name)
					{
						curValue = tran.NewAccountID;
					}
					else if (fieldNameErrorPair.Key == typeof(GLTranForReclassification.newSubID).Name)
					{
						curValue = tran.NewSubID;
					}
					else if (fieldNameErrorPair.Key == typeof(GLTranForReclassification.newTranDate).Name)
					{
						curValue = tran.NewTranDate;
					}
					else if (fieldNameErrorPair.Key == typeof(GLTranForReclassification.newFinPeriodID).Name)
					{
						curValue = tran.NewFinPeriodID;
					}

					if (curValue != null && curValue.Equals(fieldNameErrorPair.Value.ErrorValue)
					    || curValue == null && fieldNameErrorPair.Value.ErrorValue == null)
					{
						GLTranForReclass.Cache.RaiseExceptionHandling(fieldNameErrorPair.Key, tran, fieldNameErrorPair.Value.ErrorUIValue,
							fieldNameErrorPair.Value.Error);
					}
				}
			}
		}

		protected virtual void GLTranForReclassification_Selected_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			var tranForReclass = (GLTranForReclassification) e.Row;

			if (e.ExternalCall && tranForReclass.Selected == false)
			{
				InitTranForReclassEditableFields(tranForReclass);
			}

			if (tranForReclass.ReclassRowType == ReclassRowTypes.Editing)
			{
				if (tranForReclass.Selected == true)
				{
					State.GLTranForReclassToDelete.Remove(tranForReclass);
				}
				else
				{
					State.GLTranForReclassToDelete.Add(tranForReclass);
				}
			}
		}

		protected virtual void GLTranForReclassification_NewTranDate_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{


			var newDate = (DateTime)e.NewValue;
			var tranForReclass = (GLTranForReclassification)e.Row;

			FinPeriodIDAttribute.CheckIsDateWithinPeriod(this, tranForReclass.FinPeriodID, newDate, Messages.TheDateIsOutsideOfTheFinancialPeriodOfTheTransaction);
		}

		protected virtual void GLTranForReclassification_NewSubID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			var tran = (GLTranForReclassification)e.Row;

			var toSubIDState = (PXFieldState)GLTranForReclass.Cache.GetStateExt<GLTranForReclassification.newSubID>(tran);
			tran.NewSubCD = (string)toSubIDState.Value;
		}

		protected virtual void GLTranForReclassification_NewBranchID_FieldUpdating(PXCache cache, PXFieldUpdatingEventArgs e)
		{
			var tran = (GLTranForReclassification)e.Row;

			GLTranForReclass.Cache.RaiseExceptionHandling<GLTranForReclassification.newBranchID>(tran, e.NewValue, null);
		}

		protected virtual void GLTranForReclassification_NewAccountID_FieldUpdating(PXCache cache, PXFieldUpdatingEventArgs e)
		{
			var tran = (GLTranForReclassification)e.Row;

			GLTranForReclass.Cache.RaiseExceptionHandling<GLTranForReclassification.newAccountID>(tran, e.NewValue, null);
		}

		protected virtual void GLTranForReclassification_NewSubID_FieldUpdating(PXCache cache, PXFieldUpdatingEventArgs e)
		{
			var tran = (GLTranForReclassification)e.Row;

			GLTranForReclass.Cache.RaiseExceptionHandling<GLTranForReclassification.newSubID>(tran, e.NewValue, null);
		}

		protected virtual void GLTranForReclassification_NewTranDate_FieldUpdating(PXCache cache, PXFieldUpdatingEventArgs e)
		{
			var tran = (GLTranForReclassification)e.Row;

			GLTranForReclass.Cache.RaiseExceptionHandling<GLTranForReclassification.newTranDate>(tran, e.NewValue, null);
		}

		protected virtual void GLTranForReclassification_NewFinPeriodID_ExceptionHandling(PXCache cache, PXExceptionHandlingEventArgs e)
		{
			var tran = (GLTranForReclassification)e.Row;

			var newTranDatehasError = FieldHasError(tran, typeof(GLTranForReclassification.newTranDate).Name);

			if (!newTranDatehasError)
			{
				cache.RaiseExceptionHandling<GLTranForReclassification.newTranDate>(tran, tran.NewTranDate, e.Exception);
			}
		}

		protected virtual void GLTranForReclassification_RowUpdated(PXCache cache, PXRowUpdatedEventArgs e)
		{
			var tran = (GLTranForReclassification) e.Row;

			if (tran.NewBranchID == null)
			{
				cache.RaiseExceptionHandling<GLTranForReclassification.newBranchID>(e.Row, null,
					new PXSetPropertyException(ErrorMessages.FieldIsEmpty, PXErrorLevel.Error,
						typeof (GLTranForReclassification.newBranchID).Name));
			}

			if (tran.NewAccountID == null)
			{
				cache.RaiseExceptionHandling<GLTranForReclassification.newAccountID>(e.Row, null,
					new PXSetPropertyException(ErrorMessages.FieldIsEmpty, PXErrorLevel.Error,
						typeof (GLTranForReclassification.newAccountID).Name));
			}

			if (tran.NewSubID == null)
			{
				cache.RaiseExceptionHandling<GLTranForReclassification.newSubID>(e.Row, null,
					new PXSetPropertyException(ErrorMessages.FieldIsEmpty, PXErrorLevel.Error,
						typeof (GLTranForReclassification.newSubID).Name));
			}

			if (tran.NewTranDate == null)
			{
				cache.RaiseExceptionHandling<GLTranForReclassification.newTranDate>(e.Row, null,
					new PXSetPropertyException(ErrorMessages.FieldIsEmpty, PXErrorLevel.Error,
						typeof (GLTranForReclassification.newTranDate).Name));
			}
		}

		private void VerifyAndRememberErorrForNewFields(GLTranForReclassification tran)
		{
			VerifyAndRememberErorr<GLTranForReclassification.newBranchID>(tran.NewBranchID, tran);
			VerifyAndRememberErorr<GLTranForReclassification.newAccountID>(tran.NewAccountID, tran);
			VerifyAndRememberErorr<GLTranForReclassification.newSubID>(tran.NewSubID, tran);
			VerifyAndRememberErorr<GLTranForReclassification.newTranDate>( tran.NewTranDate, tran);
			VerifyAndRememberErorr<GLTranForReclassification.newFinPeriodID>( tran.NewFinPeriodID, tran);
		}

		#endregion


		#region LoadOptions

		protected virtual void LoadOptions_FromAccountID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			var options = (LoadOptions)e.Row;

			var fromAccountIDState = (PXFieldState)cache.GetStateExt<LoadOptions.fromAccountID>(options);
			var toAccountIDState = (PXFieldState)cache.GetStateExt<LoadOptions.toAccountID>(options);

			var fromAccountCD = (string) fromAccountIDState.Value;
			var toAccountCD = (string) toAccountIDState.Value;

			if (String.CompareOrdinal(fromAccountCD, toAccountCD) > 0)
			{
				cache.SetValueExt<LoadOptions.toAccountID>(e.Row, fromAccountCD);
			}
		}

		protected virtual void LoadOptions_ToAccountID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			var options = (LoadOptions)e.Row;

			var fromAccountIDState = (PXFieldState)cache.GetStateExt<LoadOptions.fromAccountID>(options);
			var toAccountIDState = (PXFieldState)cache.GetStateExt<LoadOptions.toAccountID>(options);

			var fromAccountCD = (string)fromAccountIDState.Value;
			var toAccountCD = (string)toAccountIDState.Value;

			if (toAccountCD != null && String.CompareOrdinal(fromAccountCD, toAccountCD) > 0)
			{
				cache.SetValueExt<LoadOptions.fromAccountID>(e.Row, toAccountCD);
			}
		}

		protected virtual void LoadOptions_FromSubID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			var options = (LoadOptions)e.Row;

			var fromSubIDState = (PXFieldState)cache.GetStateExt<LoadOptions.fromSubID>(options);
			var toSubIDState = (PXFieldState)cache.GetStateExt<LoadOptions.toSubID>(options);

			var fromSubCD = (string)fromSubIDState.Value;
			var toSubCD = (string)toSubIDState.Value;

			if (String.CompareOrdinal(fromSubCD, toSubCD) > 0)
			{
				cache.SetValue<LoadOptions.toSubID>(e.Row, options.FromSubID);
			}
		}

		protected virtual void LoadOptions_ToSubID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			var options = (LoadOptions)e.Row;

			var fromSubIDState = (PXFieldState)cache.GetStateExt<LoadOptions.fromSubID>(options);
			var toSubIDState = (PXFieldState)cache.GetStateExt<LoadOptions.toSubID>(options);

			var fromSubCD = (string)fromSubIDState.Value;
			var toSubCD = (string)toSubIDState.Value;

			if (toSubCD != null && String.CompareOrdinal(fromSubCD, toSubCD) > 0)
			{
				cache.SetValue<LoadOptions.fromSubID>(e.Row, options.ToSubID);
			}
		}

		protected virtual void LoadOptions_FromFinPeriodID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			var options = (LoadOptions)e.Row;

			if (String.CompareOrdinal(options.FromFinPeriodID, options.ToFinPeriodID) > 0)
			{
				cache.SetValue<LoadOptions.toFinPeriodID>(e.Row, options.FromFinPeriodID);
			}

			SetPeriodDates(options);
		}

		protected virtual void LoadOptions_ToFinPeriodID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			var options = (LoadOptions)e.Row;

			if (options.ToFinPeriodID != null && String.CompareOrdinal(options.FromFinPeriodID, options.ToFinPeriodID) > 0)
			{
				cache.SetValue<LoadOptions.fromFinPeriodID>(e.Row, options.ToFinPeriodID);
			}

			SetPeriodDates(options);
		}

		protected virtual void LoadOptions_FromDate_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			var options = (LoadOptions)e.Row;

			if (options.FromDate > options.ToDate || options.ToDate == null)
			{
				cache.SetValue<LoadOptions.toDate>(e.Row, options.FromDate);
			}
		}

		protected virtual void LoadOptions_ToDate_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			var options = (LoadOptions)e.Row;

			if (options.ToDate != null && options.FromDate > options.ToDate)
			{
				cache.SetValue<LoadOptions.fromDate>(e.Row, options.ToDate);
			}
		}

		protected virtual void LoadOptions_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			var loadOptions = e.Row as LoadOptions;

			if (loadOptions == null)
				return;

			if (loadOptions.ToDate.HasValue && ((loadOptions.PeriodEndDate.HasValue && loadOptions.ToDate > loadOptions.PeriodEndDate) || (loadOptions.PeriodStartDate.HasValue && loadOptions.ToDate < loadOptions.PeriodStartDate)))
			{
				cache.RaiseExceptionHandling<LoadOptions.toDate>(e.Row, loadOptions.ToDate, new PXSetPropertyException(Messages.TheDateIsOutsideOfTheSpecifiedPeriod, PXErrorLevel.Warning));
			}
			else
			{
				cache.RaiseExceptionHandling<LoadOptions.toDate>(e.Row, null, null);
			}

			if (loadOptions.FromDate.HasValue && ((loadOptions.PeriodStartDate.HasValue && loadOptions.FromDate < loadOptions.PeriodStartDate) || (loadOptions.PeriodEndDate.HasValue && loadOptions.FromDate >= loadOptions.PeriodEndDate)))
			{
				cache.RaiseExceptionHandling<LoadOptions.fromDate>(e.Row, loadOptions.FromDate, new PXSetPropertyException(Messages.TheDateIsOutsideOfTheSpecifiedPeriod, PXErrorLevel.Warning));
			}
			else
			{
				cache.RaiseExceptionHandling<LoadOptions.fromDate>(e.Row, null, null);
			}
		}

		#endregion

		private void ButtonsFieldSelectingHandlerForDisableAfterProcess(PXCache sender, PXFieldSelectingEventArgs e)
		{
			e.ReturnState = CreateReturnState(e.ReturnState);

			((PXButtonState)e.ReturnState).Enabled = PXLongOperation.GetStatus(this.UID) == PXLongRunStatus.NotExists;
		}

		private void DependingOnRowExistanceButtonsSelectingHandler(PXCache sender, PXFieldSelectingEventArgs e)
		{
			e.ReturnState = CreateReturnState(e.ReturnState);

			ButtonsFieldSelectingHandlerForDisableAfterProcess(sender, e);

			((PXButtonState)e.ReturnState).Enabled = ((PXButtonState)e.ReturnState).Enabled && sender.Cached.Any_();
		}

		private void ReloadTransButtonStateSelectingHandler(PXCache sender, PXFieldSelectingEventArgs e)
		{
			e.ReturnState = CreateReturnState(e.ReturnState);

			((PXButtonState)e.ReturnState).Enabled = PXLongOperation.GetStatus(this.UID) == PXLongRunStatus.NotExists
													  && State.ReclassScreenMode != ReclassScreenMode.Editing;
		}

		#endregion


		#region ReplaceOptions

		protected virtual void ReplaceOptions_NewFinPeriodID_ExceptionHandling(PXCache cache, PXExceptionHandlingEventArgs e)
		{
			var replaceOptionRow = (ReplaceOptions)e.Row;

			cache.RaiseExceptionHandling<ReplaceOptions.newDate>(replaceOptionRow, replaceOptionRow.NewDate, e.Exception);
		}

		protected virtual void ReplaceOptions_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			var replaceOptionRow = (ReplaceOptions)e.Row;

			if (!replaceOptionRow.Showed)
			{
				replaceOptionRow.Showed = true;

				if (replaceOptionRow.NewFinPeriodID != null)
				{
					object finPeriodID = replaceOptionRow.NewFinPeriodID;

					cache.RaiseFieldVerifying<ReplaceOptions.newFinPeriodID>(replaceOptionRow, ref finPeriodID);
				}
				else if (replaceOptionRow.NewDate != null)
				{
					cache.RaiseFieldUpdated<ReplaceOptions.newDate>(replaceOptionRow, replaceOptionRow.NewDate);
				}
			}
		}

		#endregion


		#region Actions

		[PXDeleteButton(ConfirmationType = PXConfirmationType.Unspecified, ClosePopup = false, ImageKey = Sprite.Main.RecordDel)]
		[PXUIField(DisplayName = ActionsMessages.Delete, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		protected virtual IEnumerable delete(PXAdapter adapter)
		{
			var tran = GLTranForReclass.Current;

			if (tran != null)
			{
				if (tran.ReclassRowType == ReclassRowTypes.Editing)
				{
					State.GLTranForReclassToDelete.Add(tran);
				}

				GLTranForReclass.Cache.Remove(tran);
			}

			return adapter.Get();
		}

		[PXCancelButton]
		[PXUIField(DisplayName = ActionsMessages.Cancel, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		protected virtual IEnumerable cancel(PXAdapter adapter)
		{
			var addedTranForReclass = GetUpdatedTranForReclass().Where(tran => tran.ReclassRowType == ReclassRowTypes.AddingNew);
			var state = State;
			state.GLTranForReclassToDelete.Clear();

			Clear();

			State = state;
			PutTransForReclassToCacheByKey(addedTranForReclass);
			PutReclassificationBatchTransForEditingToCache(State.EditingBatchModule, State.EditingBatchNbr);

			return adapter.Get();
		}

		public PXAction<GLTranForReclassification> ViewReclassBatch;
		[PXUIField(Visible = false, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable viewReclassBatch(PXAdapter adapter)
		{
			var tran = GLTranForReclass.Current;

			if (tran != null)
			{
				JournalEntry.RedirectToBatch(this, tran.ReclassBatchModule, tran.ReclassBatchNbr);
			}

			return adapter.Get();
		}

		[PXUIField(DisplayName = Messages.Load, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXButton]
		public virtual IEnumerable ShowLoadTransPopup(PXAdapter adapter)
		{
			LoadOptionsView.AskExt();

			return adapter.Get();
		}

		[PXUIField(DisplayName = "Reload", Visible = false, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXButton]
		public virtual IEnumerable ReloadTrans(PXAdapter adapter)
		{
			var res = GLTranForReclass.Ask(InfoMessages.TransactionsListedOnTheFormIfAnyWillBeRemoved, MessageButtons.OKCancel);

			if (res != WebDialogResult.OK)
				return adapter.Get();
			
			GLTranForReclass.Cache.Clear();

			LoadTransactionsProc();

			return adapter.Get(); 
		}

		[PXUIField(DisplayName = Messages.Load, Visible = false, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXButton]
		public virtual IEnumerable LoadTrans(PXAdapter adapter)
		{
			LoadTransactionsProc();

			return adapter.Get(); 
		}

		[PXUIField(DisplayName = "Replace", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXButton]
		public virtual IEnumerable Replace(PXAdapter adapter)
		{
			var options = ReplaceOptionsView.Current;

			options.Showed = false;

			if (ReplaceOptionsView.AskExt() == WebDialogResult.OK)
			{
				if (options.NewBranchID == null
				    && options.NewAccountID == null
				    && options.NewSubID == null
				    && options.NewFinPeriodID == null
				    && options.NewDate == null
				    && options.NewTranDesc == null)
				{
					return adapter.Get();
				}

				var trans = GetUpdatedTranForReclass();

				if (options.WithBranchID != null)
				{
					trans = trans.Where(tran => tran.NewBranchID == options.WithBranchID);
				}

				if (options.WithAccountID != null)
				{
					trans = trans.Where(tran => tran.NewAccountID == options.WithAccountID);
				}

				if (options.WithSubID != null)
				{
					trans = trans.Where(tran => tran.NewSubID == options.WithSubID);
				}

				if (options.WithFinPeriodID != null)
				{
					trans = trans.Where(tran => tran.FinPeriodID == options.WithFinPeriodID);
				}

				if (options.WithDate != null)
				{
					trans = trans.Where(tran => tran.NewTranDate == options.WithDate);
				}

				if (options.WithTranDescFilteringValue != null)
				{
					trans = AddTransDescWhereConditionForReplace(trans, options.WithTranDescFilteringValue);
				}

				foreach (var tran in trans)
				{
					var oldTran = PXCache<GLTranForReclassification>.CreateCopy(tran);

					if (options.NewBranchID != null && options.NewBranchID != tran.NewBranchID)
					{
						tran.NewBranchID = options.NewBranchID;
					}

					if (options.NewAccountID != null && options.NewAccountID != tran.NewAccountID)
					{
						tran.NewAccountID = options.NewAccountID;
					}

					if (options.NewSubID != null && options.NewSubID != tran.NewSubID)
					{
						var newSubIDState = (PXFieldState) ReplaceOptionsView.Cache.GetStateExt<ReplaceOptions.newSubID>(options);
						GLTranForReclass.Cache.SetValueExt<GLTranForReclassification.newSubID>(tran, newSubIDState.Value);
					}

					if (options.NewDate != null && options.NewDate != tran.NewTranDate)
					{
						tran.NewTranDate = options.NewDate;

						object newValue = tran.NewTranDate;

						try
						{
							GLTranForReclass.Cache.RaiseFieldVerifying<GLTranForReclassification.newTranDate>(tran, ref newValue);
						}
						catch (PXSetPropertyException ex)
						{
							GLTranForReclass.Cache.RaiseExceptionHandling<GLTranForReclassification.newTranDate>(tran, newValue, ex);
						}
					}

					if (options.NewTranDesc != null)
					{
						tran.NewTranDesc = _emptyStringReplaceWildCards.Contains((options.NewTranDesc))
							? null
							: options.NewTranDesc;
					}

					GLTranForReclass.Cache.RaiseRowUpdated(tran, oldTran);
				}
			}

			return adapter.Get();
		}

		public PXAction<GLTranForReclassification> viewDocument;
		[PXUIField(MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXButton()]
		public virtual IEnumerable ViewDocument(PXAdapter adapter)
		{
			var tranForReclass = GLTranForReclass.Current;
			
			if (tranForReclass != null)
			{
				JournalEntry.OpenDocumentByTran(tranForReclass);
			}

			return adapter.Get();
		}

		#endregion


		#region UI

		private void VerifyAndRememberErorr<TField>(object newValue, GLTranForReclassification tran) 
			where TField : IBqlField
		{
			var origNewValue = newValue;

			try
			{
				GLTranForReclass.Cache.RaiseFieldVerifying<TField>(tran, ref newValue);
			}
			catch (PXSetPropertyException ex)
			{
				var fieldName = typeof(TField).Name;

				tran.FieldsErrorForInvalidFromValues[fieldName] = new GLTranForReclassification.ExceptionAndErrorValuesTriple()
				{
					Error = ex,
					ErrorValue = origNewValue,
					ErrorUIValue = newValue
				};

				GLTranForReclass.Cache.RaiseExceptionHandling<TField>(tran, newValue, ex);
			}
		}

		private PXButtonState CreateReturnState(object returnState)
		{
			return PXButtonState.CreateInstance(returnState, null, null, null, null, null, false,
					   PXConfirmationType.Unspecified, null, null, null, null, null, null, null, null, null, null, null,
					   typeof(GLTranForReclassification));

		}

		protected void LoadTransactionsProc()
		{
			var dataRows = GetTransForReclassByLoadOptions(LoadOptionsView.Current);

			var nonReclassifiableFound = false;
			var reclassifiableFound = false;

			foreach (var row in dataRows)
			{
				var tranForReclass = (GLTranForReclassification) row;
				var curyInfo = (CurrencyInfo) row;
				var batch = (Batch) row;
				var ledger = (Ledger) row;

				if (JournalEntry.IsTransactionReclassifiable(tranForReclass, batch.BatchType, ledger.BalanceType, ProjectDefaultAttribute.NonProject(this)))
				{
					InitTranForReclassAdditionalFields(tranForReclass, curyInfo);
					GLTranForReclass.Cache.SetStatus(tranForReclass, PXEntryStatus.Updated);

					reclassifiableFound = true;
				}
				else if (tranForReclass.ReclassRowType != ReclassRowTypes.Editing)
				{
					nonReclassifiableFound = true;

					GLTranForReclass.Cache.Remove(tranForReclass);
				}
			}

			if (!reclassifiableFound)
			{
				throw new PXException(
					InfoMessages.NoReclassifiableTransactionsHaveBeenFoundToMatchTheCriteria);
			}

			if (nonReclassifiableFound)
			{
				GLTranForReclass.Ask(
					InfoMessages
						.SomeTransactionsCannotBeReclassified,
					MessageButtons.OK);
			}
		}

		protected virtual void SetPeriodDates(LoadOptions loadOptions)
		{
			FinPeriod fromPeriod = PXSelectReadonly<FinPeriod, Where<FinPeriod.finPeriodID, Equal<Required<FinPeriod.finPeriodID>>>>.Select(this, loadOptions.FromFinPeriodID);
			FinPeriod endPeriod = PXSelectReadonly<FinPeriod, Where<FinPeriod.finPeriodID, Equal<Required<FinPeriod.finPeriodID>>>>.Select(this, loadOptions.ToFinPeriodID);

			if (fromPeriod != null && endPeriod != null)
			{
				loadOptions.PeriodStartDate = fromPeriod.StartDate <= endPeriod.StartDate ? fromPeriod.StartDate : endPeriod.StartDate;
				loadOptions.PeriodEndDate = endPeriod.EndDate >= fromPeriod.EndDate ? endPeriod.EndDate : fromPeriod.EndDate;
			}
			else if (fromPeriod != null || endPeriod != null)
			{
				var datesPeriod = fromPeriod ?? endPeriod;
				loadOptions.PeriodStartDate = datesPeriod.StartDate;
				loadOptions.PeriodEndDate = datesPeriod.EndDate;
			}
			else
			{
				loadOptions.PeriodStartDate = null;
				loadOptions.PeriodEndDate = null;
			}
		}

		private void CalcAndSetSelectedFieldState(GLTranForReclassification tranForReclass)
		{
			var anyReclassAttrChanged = tranForReclass.NewBranchID != tranForReclass.BranchID ||
										 tranForReclass.NewAccountID != tranForReclass.AccountID ||
										 tranForReclass.NewSubID != tranForReclass.SubID;

			var allNotNull = tranForReclass.NewBranchID != null &&
							 tranForReclass.NewAccountID != null &&
							 tranForReclass.NewSubID != null &&
							 tranForReclass.NewTranDate != null;

			var hasErrorInFields = false;

			foreach (var editableField in EditableFields)
			{
				hasErrorInFields = FieldHasError(tranForReclass, editableField.Name);

				if (hasErrorInFields)
					break;
			}

			var allowProcess = anyReclassAttrChanged && allNotNull && !hasErrorInFields;

			if (tranForReclass.Selected != allowProcess)
			{
				GLTranForReclass.Cache.SetValueExt<GLTranForReclassification.selected>(tranForReclass, allowProcess);
				PXUIFieldAttribute.SetEnabled<GLTranForReclassification.selected>(GLTranForReclass.Cache, tranForReclass, allowProcess);
			}
		}

		private bool FieldHasError(GLTranForReclassification tranForReclass, string fieldName)
		{
			return GLTranForReclass.Cache.GetAttributesReadonly(tranForReclass, fieldName)
															.OfType<IPXInterfaceField>()
															.Any(attr => attr.ErrorText != null && attr.ErrorLevel == PXErrorLevel.Error);
		}

		private IEnumerable<GLTranForReclassification> AddTransDescWhereConditionForReplace(IEnumerable<GLTranForReclassification> trans, string filteringValue)
		{
			if (_emptyStringReplaceWildCards.Contains(filteringValue))
			{
				return trans.Where(tran => string.IsNullOrEmpty(tran.NewTranDesc));
			}

			var pattern = Regex.Escape(filteringValue);
			pattern = string.Concat("^", pattern.Replace("\\*", ".*").Replace("\\?", "."), "$");

			var regex = new Regex(pattern, RegexOptions.IgnoreCase);

			return trans.Where(tran => regex.IsMatch(tran.TranDesc));
		}

		private IList<PXResult<GLTranForReclassification, CurrencyInfo>> RegetAndKeepInCacheOnlyReclassifiableTransByKey(IEnumerable<GLTran> trans)
		{
			var resultTrans = new List<PXResult<GLTranForReclassification, CurrencyInfo>>();

			foreach (var tran in trans)
			{
				if (tran.ReclassBatchNbr == null)
				{
					var refreshedTran =
						(PXResult<GLTranForReclassification, CurrencyInfo>)
							GLTranForReclassWithCuryInfo.Select(tran.Module, tran.BatchNbr, tran.LineNbr);

					if (refreshedTran != null)
					{
						resultTrans.Add(refreshedTran);
					}
					else
					{
						TryRemoveTranForReclassFromCacheByKey(tran);
					}
				}
				else
				{
					TryRemoveTranForReclassFromCacheByKey(tran);
				}
			}

			return resultTrans;
		}

		private void TryRemoveTranForReclassFromCacheByKey(GLTran tran)
		{
			var tranForReclass = new GLTranForReclassification()
			{
				Module = tran.Module,
				BatchNbr = tran.BatchNbr,
				LineNbr = tran.LineNbr
			};

			tranForReclass = GLTranForReclass.Locate(tranForReclass);

			if (tranForReclass != null)
			{
				GLTranForReclass.Cache.Remove(tranForReclass);
			}
		}

		private void PutTransForReclassToCacheByKey(IEnumerable<GLTran> trans)
		{
			var resultRows = RegetAndKeepInCacheOnlyReclassifiableTransByKey(trans);

			foreach (PXResult<GLTranForReclassification, CurrencyInfo> row in resultRows)
			{
				var tranForReclass = (GLTranForReclassification) row;
				var curyInfo = (CurrencyInfo) row;

				InitTranForReclassAdditionalFields(tranForReclass, curyInfo);

				GLTranForReclass.Cache.SetStatus(tranForReclass, PXEntryStatus.Updated);
			}
		}

		private void PutReclassificationBatchTransForEditingToCache(string module, string batchNbr)
		{
			var transForEditing = JournalEntry.GetTrans(this, module, batchNbr);

			foreach (var tranToEdit in transForEditing)
			{
				if (tranToEdit.IsReclassReverse == false)
				{
					var tranForReclass = (GLTranForReclassification) GLTranForReclass_Module_BatchNbr_LineNbr.Select(tranToEdit.OrigModule,
																														tranToEdit.OrigBatchNbr,
																														tranToEdit.OrigLineNbr);

					GLTranForReclass.Cache.SetStatus(tranForReclass, PXEntryStatus.Updated);

					InitTranForReclassAdditionalFieldsForEditing(tranForReclass, tranToEdit);
					tranForReclass.EditingPairReclassifyingLineNbr = tranToEdit.LineNbr;
					tranForReclass.ReclassRowType = ReclassRowTypes.Editing;
				}
			}
		}

		private void PutReclassificationBatchTransForReversingToCache(string module, string batchNbr, string curyID)
		{
			var transForReclass = GLTransForReclassForReverseView.Select(module, batchNbr)
															.RowCast<GLTranForReclassification>();

			var reverseTrans = ReclassReverseGLTransView.Select(module, batchNbr)
														.RowCast<GLTran>()
														.ToDictionary(tran => tran.LineNbr, tran => tran);

			foreach (var tranForReclass in transForReclass)
			{
				var reverseTran = reverseTrans[tranForReclass.LineNbr.Value - 1];

				InitTranForReclassEditableFieldsFromTran(tranForReclass, reverseTran);
				tranForReclass.CuryID = curyID;

				GLTranForReclass.Cache.SetStatus(tranForReclass, PXEntryStatus.Updated);
			}
		}

		public static void TryOpenForReclassificationOfDocument(PXView askView, string module, string batchNbr, string docType, string refNbr)
		{
			var graph = PXGraph.CreateInstance<ReclassifyTransactionsProcess>();

			var dataRows = PXSelectJoin<GLTranForReclassification,
									InnerJoin<CurrencyInfo,
										On<GLTran.curyInfoID, Equal<CurrencyInfo.curyInfoID>>>,
									Where<GLTranForReclassification.module, Equal<Required<GLTranForReclassification.module>>,
										And<GLTranForReclassification.batchNbr, Equal<Required<GLTranForReclassification.batchNbr>>,
										And<GLTranForReclassification.tranType, Equal<Required<GLTranForReclassification.tranType>>,
										And<GLTranForReclassification.refNbr, Equal<GLTranForReclassification.refNbr> >>>>>
									.Select(graph, module, batchNbr, docType, refNbr)
									.Select<PXResult<GLTranForReclassification, CurrencyInfo>>();

			var nonReclassifiableFound = false;
			var reclassifiableFound = false;

			foreach (var row in dataRows)
			{
				var tran = (GLTranForReclassification) row;
				var curyInfo = (CurrencyInfo) row;


				if (JournalEntry.IsTransactionReclassifiable(tran, null, null, ProjectDefaultAttribute.NonProject(graph)))
				{
					graph.InitTranForReclassAdditionalFields(tran, curyInfo);

					graph.GLTranForReclass.Cache.SetStatus(tran, PXEntryStatus.Updated);

					reclassifiableFound = true;
				}
				else
				{
					nonReclassifiableFound = true;

					graph.GLTranForReclass.Cache.Remove(tran);
				}
			}

			if (!reclassifiableFound)
			{
				throw new PXException(
					InfoMessages.NoReclassifiableTransactionsHaveBeenFoundInTheBatch);
			}

			if (nonReclassifiableFound)
			{
				askView.Ask(
					InfoMessages.SomeTransactionsOfTheBatchCannotBeReclassified,
					MessageButtons.OK);
			}

			throw new PXRedirectRequiredException(graph, true, string.Empty)
			{
				Mode = PXBaseRedirectException.WindowMode.New
			};
		}

		public static void TryOpenForReclassification<TTran>(PXGraph graph, 
																IEnumerable<TTran> trans, 
																Ledger ledger,
																Func<TTran, string> getBatchTypeDelegate,
																PXView askView,
																string someTransactionsCannotBeReclassifiedMessage,
																string noTransactionsForWhichTheReclassificationCanBePerformed,
																PXBaseRedirectException.WindowMode redirectMode = PXBaseRedirectException.WindowMode.New)
			where TTran : GLTran
		{
			var nonReclassifiableFound = false;

			var validTrans = new List<TTran>();

			foreach (var tran in trans)
			{
				if (JournalEntry.IsTransactionReclassifiable(tran, getBatchTypeDelegate(tran), ledger.BalanceType, ProjectDefaultAttribute.NonProject(graph)))
				{
					validTrans.Add(tran);
				}
				else
				{
					nonReclassifiableFound = true;
				}
			}

			if (validTrans.Count == 0)
			{
				throw new PXException(noTransactionsForWhichTheReclassificationCanBePerformed);
			}

			if (nonReclassifiableFound)
			{
				askView.Ask(someTransactionsCannotBeReclassifiedMessage, MessageButtons.OK);
			}

			OpenForReclassification(validTrans, redirectMode);
		}

		public static void OpenForReclassification(IReadOnlyCollection<GLTran> trans, PXBaseRedirectException.WindowMode redirectMode = PXBaseRedirectException.WindowMode.New)
		{
			if (trans == null)
				throw new ArgumentNullException("trans");

			var graph = PXGraph.CreateInstance<ReclassifyTransactionsProcess>();

			graph.State.ReclassScreenMode = ReclassScreenMode.Reclassification;
			graph.PutTransForReclassToCacheByKey(trans);

			throw new PXRedirectRequiredException(graph, true, string.Empty)
			{
				Mode = redirectMode
			};
		}

		public static void OpenForReclassBatchEditing(Batch batch)
		{
			var graph = PXGraph.CreateInstance<ReclassifyTransactionsProcess>();

			graph.State.ReclassScreenMode = ReclassScreenMode.Editing;
			graph.State.EditingBatchModule = batch.Module;
			graph.State.EditingBatchNbr = batch.BatchNbr;
			graph.State.EditingBatchFinPeriodID = batch.FinPeriodID;
			graph.State.EditingBatchCuryID = batch.CuryID;

			graph.PutReclassificationBatchTransForEditingToCache(batch.Module, batch.BatchNbr);

			throw new PXRedirectRequiredException(graph, true, string.Empty)
			{
				Mode = PXBaseRedirectException.WindowMode.Same
			};
		}

		public static void OpenForReclassBatchReversing(Batch batch)
		{
			var graph = PXGraph.CreateInstance<ReclassifyTransactionsProcess>();

			graph.State.ReclassScreenMode = ReclassScreenMode.Reversing;
			graph.State.OrigBatchModuleToReverse = batch.Module;
			graph.State.OrigBatchNbrToReverse = batch.BatchNbr;

			graph.PutReclassificationBatchTransForReversingToCache(batch.Module, batch.BatchNbr, batch.CuryID);

			throw new PXRedirectRequiredException(graph, true, string.Empty)
			{
				Mode = PXBaseRedirectException.WindowMode.Same
			};
		}

		private void InitTranForReclassAdditionalFieldsForEditing(GLTranForReclassification tranForReclass, GLTran tran)
		{
			InitTranForReclassEditableFieldsFromTran(tranForReclass, tran);
			tranForReclass.CuryID = State.EditingBatchCuryID;
		}

		private void InitTranForReclassEditableFieldsFromTran(GLTranForReclassification tranForReclass, GLTran tran)
		{
			tranForReclass.NewBranchID = tran.BranchID;
			tranForReclass.NewAccountID = tran.AccountID;
			tranForReclass.NewSubID = tran.SubID;
			tranForReclass.NewSubCD = null;
			tranForReclass.NewTranDate = tran.TranDate;
			tranForReclass.NewFinPeriodID = tran.FinPeriodID;
			tranForReclass.NewTranDesc = tran.TranDesc;
		}

		private void InitTranForReclassEditableFields(GLTranForReclassification tranForReclass)
		{
			tranForReclass.NewFinPeriodID = tranForReclass.FinPeriodID;
			tranForReclass.NewTranDate = tranForReclass.TranDate;
			tranForReclass.NewBranchID = tranForReclass.BranchID;
			tranForReclass.NewAccountID = tranForReclass.AccountID;
			tranForReclass.NewSubID = tranForReclass.SubID;
			tranForReclass.NewSubCD = null;
			tranForReclass.NewTranDesc = tranForReclass.TranDesc;
		}

		private void InitTranForReclassAdditionalFields(GLTranForReclassification tran, CurrencyInfo curyInfo)
		{
			InitTranForReclassEditableFields(tran);
			tran.CuryID = curyInfo.CuryID;
			tran.VerifyingForFromValuesInvoked = false;
		}

		private IEnumerable<PXResult<GLTranForReclassification, Account, Sub, Batch, CurrencyInfo, Ledger>> GetTransForReclassByLoadOptions(LoadOptions loadOptions)
		{
			PXSelectBase<GLTranForReclassification> query = new PXSelectJoinOrderBy<GLTranForReclassification,
																		InnerJoin<Account,
																			On<GLTranForReclassification.accountID, Equal<Account.accountID>>,
																		InnerJoin<Sub,
																			On<GLTranForReclassification.subID, Equal<Sub.subID>>,
																		InnerJoin<Batch,
																			On<Batch.module, Equal<GLTranForReclassification.module>,
																				And<Batch.batchNbr, Equal<GLTranForReclassification.batchNbr>>>,
																		InnerJoin<CurrencyInfo,
																			On<GLTranForReclassification.curyInfoID, Equal<CurrencyInfo.curyInfoID>>,
																		InnerJoin<Ledger,
																			On<GLTranForReclassification.ledgerID, Equal<Ledger.ledgerID>>>>>>>,
																		OrderBy<Asc<GLTranForReclassification.module, 
																				Asc<GLTranForReclassification.batchNbr, 
																				Asc<GLTranForReclassification.lineNbr>>>>>
																		(this);

			var pars = new List<object>();

			if (loadOptions.BranchID != null)
			{
				query.WhereAnd<Where<GLTranForReclassification.branchID, Equal<Required<GLTranForReclassification.branchID>>>>();
				pars.Add(loadOptions.BranchID);
			}

			if (loadOptions.FromAccountID != null)
			{
				var fromAccount = AccountAttribute.GetAccount(this, loadOptions.FromAccountID);

				query.WhereAnd<Where<Account.accountCD, GreaterEqual<Required<Account.accountCD>>>>();
				pars.Add(fromAccount.AccountCD);
			}
			if (loadOptions.ToAccountID != null)
			{
				var toAccount = AccountAttribute.GetAccount(this, loadOptions.ToAccountID);

				query.WhereAnd<Where<Account.accountCD, LessEqual<Required<Account.accountCD>>>>();
				pars.Add(toAccount.AccountCD);
			}

			if (loadOptions.FromSubID != null)
			{
				var fromSubaccount = SubAccountAttribute.GetSubaccount(this, loadOptions.FromSubID);

				query.WhereAnd<Where<Sub.subCD, GreaterEqual<Required<Sub.subCD>>>>();
				pars.Add(fromSubaccount.SubCD);
			}
			if (loadOptions.ToSubID != null)
			{
				var toSubaccount = SubAccountAttribute.GetSubaccount(this, loadOptions.ToSubID);

				query.WhereAnd<Where<Sub.subCD, LessEqual<Required<Sub.subCD>>>>();
				pars.Add(toSubaccount.SubCD);
			}

			if (loadOptions.FromDate != null)
			{
				query.WhereAnd<Where<GLTranForReclassification.tranDate, GreaterEqual<Current<LoadOptions.fromDate>>>>();
			}
			if (loadOptions.ToDate != null)
			{
				query.WhereAnd<Where<GLTranForReclassification.tranDate, LessEqual<Current<LoadOptions.toDate>>>>();
			}

			if (loadOptions.FromFinPeriodID != null)
			{
				query.WhereAnd<Where<GLTranForReclassification.finPeriodID, GreaterEqual<Current<LoadOptions.fromFinPeriodID>>>>();
			}
			if (loadOptions.ToFinPeriodID != null)
			{
				query.WhereAnd<Where<GLTranForReclassification.finPeriodID, LessEqual<Current<LoadOptions.toFinPeriodID>>>>();
			}

			if (loadOptions.Module != null)
			{
				query.WhereAnd<Where<GLTranForReclassification.module, Equal<Current<LoadOptions.module>>>>();
			}

			if (loadOptions.BatchNbr != null)
			{
				query.WhereAnd<Where<GLTranForReclassification.batchNbr, Equal<Current<LoadOptions.batchNbr>>>>();
			}

			if (loadOptions.RefNbr != null)
			{
				query.WhereAnd<Where<GLTranForReclassification.refNbr, Equal<Current<LoadOptions.refNbr>>>>();
			}

			if (loadOptions.ReferenceID != null)
			{
				query.WhereAnd<Where<GLTranForReclassification.referenceID, Equal<Current<LoadOptions.referenceID>>>>();
			}

			var trans = loadOptions.MaxTrans == null
				? query.Select(pars.ToArray())
				: query.SelectWindowed(0, (int)loadOptions.MaxTrans, pars.ToArray());

			return trans.Select<PXResult<GLTranForReclassification, Account, Sub, Batch, CurrencyInfo, Ledger>>();
		}

		#endregion


		#region DACs & DTOs

		public partial class LoadOptions : IBqlTable
		{
			#region BranchID

			public abstract class branchID : PX.Data.IBqlField
			{
			}

			protected Int32? _BranchID;
			[Branch(IsDBField = false, Required = false)]
			public virtual Int32? BranchID
			{
				get { return this._BranchID; }
				set { this._BranchID = value; }
			}

			#endregion
			#region FromAccountID

			public abstract class fromAccountID : PX.Data.IBqlField
			{
			}

			protected Int32? _FromAccountID;

			[AccountAny(DisplayName = "From Account")]
			public virtual Int32? FromAccountID
			{
				get { return this._FromAccountID; }
				set { this._FromAccountID = value; }
			}

			#endregion
			#region ToAccountID

			public abstract class toAccountID : PX.Data.IBqlField
			{
			}

			protected Int32? _ToAccountID;

			[AccountAny(DisplayName = "To Account")]
			public virtual Int32? ToAccountID
			{
				get { return this._ToAccountID; }
				set { this._ToAccountID = value; }
			}

			#endregion	
			#region FromSubID

			public abstract class fromSubID : PX.Data.IBqlField
			{
			}

			protected Int32? _FromSubID;
			
			[SubAccount(DisplayName = "From Subaccount")]
			public virtual Int32? FromSubID
			{
				get { return this._FromSubID; }
				set { this._FromSubID = value; }
			}

			#endregion
			#region ToSubID

			public abstract class toSubID : PX.Data.IBqlField
			{
			}

			protected Int32? _ToSubID;

			[SubAccount(DisplayName = "To Subaccount")]
			public virtual Int32? ToSubID
			{
				get { return this._ToSubID; }
				set { this._ToSubID = value; }
			}

			#endregion
			#region FromDate
			public abstract class fromDate : IBqlField
			{
			}
			protected DateTime? _FromDate;
			[PXDBDate]
			[PXUIField(DisplayName = "From Date")]
			public virtual DateTime? FromDate
			{
				get
				{
					return _FromDate;
				}
				set
				{
					_FromDate = value;
				}
			}
			#endregion
			#region ToDate
			public abstract class toDate : IBqlField
			{
			}
			protected DateTime? _ToDate;
			[PXDBDate]
			[PXUIField(DisplayName = "To Date")]
			public virtual DateTime? ToDate
			{
				get
				{
					return _ToDate;
				}
				set
				{
					_ToDate = value;
				}
			}
			#endregion
			#region FromPeriodID
			public abstract class fromFinPeriodID : IBqlField
			{
			}
			protected String _FromFinPeriodID;
			[FinPeriodSelector]
			[PXUIField(DisplayName = "From Period", Required = false)]
			public virtual String FromFinPeriodID
			{
				get
				{
					return _FromFinPeriodID;
				}
				set
				{
					_FromFinPeriodID = value;
				}
			}
			#endregion
			#region ToFinPeriodID
			public abstract class toFinPeriodID : IBqlField
			{
			}
			protected String _ToFinPeriodID;
			[FinPeriodSelector]
			[PXUIField(DisplayName = "To Period", Required = false)]
			public virtual String ToFinPeriodID
			{
				get
				{
					return _ToFinPeriodID;
				}
				set
				{
					_ToFinPeriodID = value;
				}
			}
			#endregion
			#region Module
			public abstract class module : PX.Data.IBqlField
			{
			}
			protected String _Module;

			[PXDBString(2, IsFixed = true)]
			[PXUIField(DisplayName = "Module", Visibility = PXUIVisibility.SelectorVisible)]
			[ModuleList()]
			public virtual String Module
			{
				get
				{
					return this._Module;
				}
				set
				{
					this._Module = value;
				}
			}
			#endregion
			#region BatchNbr
			public abstract class batchNbr : PX.Data.IBqlField
			{
			}
			protected String _BatchNbr;

			[PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
			[PXSelector(typeof(Search<Batch.batchNbr, Where<Batch.module, Equal<Current<LoadOptions.module>>, And<Batch.draft, Equal<False>>>, OrderBy<Desc<Batch.batchNbr>>>), Filterable = true)]
			[PXUIField(DisplayName = "Batch Number", Visibility = PXUIVisibility.SelectorVisible)]
			public virtual String BatchNbr
			{
				get
				{
					return this._BatchNbr;
				}
				set
				{
					this._BatchNbr = value;
				}
			}
			#endregion
			#region RefNbr
			public abstract class refNbr : PX.Data.IBqlField
			{
			}
			protected String _RefNbr;

			[PXDBString(15, IsUnicode = true)]
			[PXUIField(DisplayName = "Ref. Number")]
			public virtual String RefNbr
			{
				get
				{
					return this._RefNbr;
				}
				set
				{
					this._RefNbr = value;
				}
			}
			#endregion
			#region ReferenceID
			public abstract class referenceID : PX.Data.IBqlField
			{
			}
			protected Int32? _ReferenceID;

			[PXDBInt()]
			[PXSelector(typeof(Search<BAccountR.bAccountID, Where<BAccountR.type, NotEqual<BAccountType.companyType>,
				And<BAccountR.type, NotEqual<BAccountType.prospectType>>>>), SubstituteKey = typeof(BAccountR.acctCD))]
			[PXUIField(DisplayName = "Customer/Vendor")]
			public virtual Int32? ReferenceID
			{
				get
				{
					return this._ReferenceID;
				}
				set
				{
					this._ReferenceID = value;
				}
			}
			#endregion
			#region MaxTrans
			public abstract class maxDocs : IBqlField
			{
			}
			protected int? _MaxTrans;
			[PXDBInt]
			[PXUIField(DisplayName = "Max. Number of Transactions")]
			[PXDefault(999, PersistingCheck = PXPersistingCheck.Nothing)]
			public virtual int? MaxTrans
			{
				get
				{
					return _MaxTrans;
				}
				set
				{
					_MaxTrans = value;
				}
			}

			#endregion

			#region PeriodStartDate
			public abstract class periodStartDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _PeriodStartDate;
			[PXDBDate()]
			public virtual DateTime? PeriodStartDate
			{
				get
				{
					return this._PeriodStartDate;
				}
				set
				{
					this._PeriodStartDate = value;
				}
			}
			#endregion
			#region PeriodEndDate
			public abstract class periodEndDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _PeriodEndDate;
			
			[PXDBDate()]
			public virtual DateTime? PeriodEndDate
			{
				get
				{
					return this._PeriodEndDate;
				}
				set
				{
					this._PeriodEndDate = value;
				}
			}
			#endregion

			public class ModuleListAttribute : PXStringListAttribute
			{
				public ModuleListAttribute()
					: base(
					new string[] { BatchModule.GL, BatchModule.AP, BatchModule.AR, BatchModule.CA, BatchModule.IN, BatchModule.DR, BatchModule.FA, BatchModule.PM, BatchModule.PR },
					new string[] { Messages.ModuleGL, Messages.ModuleAP, Messages.ModuleAR, Messages.ModuleCA, Messages.ModuleIN, Messages.ModuleDR, Messages.ModeleFA, Messages.ModulePM, Messages.ModulePR }) { }
			}
		}

		public partial class ReplaceOptions : IBqlTable
		{
			public virtual bool Showed { get; set; }

			#region WithBranchID

			public abstract class withBranchID : PX.Data.IBqlField
			{
			}

			protected Int32? _WithBranchID;

			[Branch(null, IsDBField = false, Required = false)]
			public virtual Int32? WithBranchID
			{
				get { return this._WithBranchID; }
				set { this._WithBranchID = value; }
			}

			#endregion
			#region WithAccountID

			public abstract class withAccountID : PX.Data.IBqlField
			{
			}

			protected Int32? _WithAccountID;

			[AccountAny(DisplayName = "Account")]
			public virtual Int32? WithAccountID
			{
				get { return this._WithAccountID; }
				set { this._WithAccountID = value; }
			}

			#endregion
			#region WithSubID

			public abstract class withSubID : PX.Data.IBqlField
			{
			}

			protected Int32? _WithSubID;

			[SubAccount(DisplayName = "Subaccount")]
			public virtual Int32? WithSubID
			{
				get { return this._WithSubID; }
				set { this._WithSubID = value; }
			}

			#endregion
			#region WithDate
			public abstract class withDate : IBqlField
			{
			}
			protected DateTime? _WithDate;
			[PXDBDate]
			[PXUIField(DisplayName = "Date")]
			public virtual DateTime? WithDate
			{
				get
				{
					return _WithDate;
				}
				set
				{
					_WithDate = value;
				}
			}
			#endregion
			#region WithFinPeriodID
			public abstract class withFinPeriodID : IBqlField
			{
			}
			protected String _WithFinPeriodID;
			[OpenPeriod(typeof(ReplaceOptions.withDate), IsDBField = false, RaiseErrorOnInactivePeriod = true)]
			[PXUIField(DisplayName = "Post Period", Visibility = PXUIVisibility.SelectorVisible, Required = false)]
			public virtual String WithFinPeriodID
			{
				get
				{
					return _WithFinPeriodID;
				}
				set
				{
					_WithFinPeriodID = value;
				}
			}
			#endregion
			#region WithTranDesc
			public abstract class withTranDescFilteringValue : PX.Data.IBqlField
			{
			}
			protected String _WithTranDescFilteringValue;

			[PXString(256, IsUnicode = true)]
			[PXUIField(DisplayName = "Transaction Description")]
			public virtual String WithTranDescFilteringValue
			{
				get
				{
					return this._WithTranDescFilteringValue;
				}
				set
				{
					this._WithTranDescFilteringValue = value;
				}
			}
			#endregion

			#region NewBranchID

			public abstract class newBranchID : PX.Data.IBqlField
			{
			}

			protected Int32? _NewBranchID;

			[Branch(null, DisplayName = "Branch", IsDBField = false, Required = false)]
			public virtual Int32? NewBranchID
			{
				get { return this._NewBranchID; }
				set { this._NewBranchID = value; }
			}

			#endregion
			#region NewAccountID

			public abstract class newAccountID : PX.Data.IBqlField
			{
			}

			protected Int32? _NewAccountID;

			[AccountAny(DisplayName = "Account")]
			public virtual Int32? NewAccountID
			{
				get { return this._NewAccountID; }
				set { this._NewAccountID = value; }
			}

			#endregion
			#region NewSubID

			public abstract class newSubID : PX.Data.IBqlField
			{
			}

			protected Int32? _NewSubID;

			[SubAccount(DisplayName = "Subaccount")]
			public virtual Int32? NewSubID
			{
				get { return this._NewSubID; }
				set { this._NewSubID = value; }
			}

			#endregion
			#region NewDate
			public abstract class newDate : IBqlField
			{
			}
			protected DateTime? _NewDate;
			[PXDBDate]
			[PXUIField(DisplayName = "Date")]
			public virtual DateTime? NewDate
			{
				get
				{
					return _NewDate;
				}
				set
				{
					_NewDate = value;
				}
			}
			#endregion
			#region NewFinPeriodID
			public abstract class newFinPeriodID : IBqlField
			{
			}
			protected String _NewFinPeriodID;

			//Used only for validation
			[OpenPeriod(typeof(ReplaceOptions.newDate), IsDBField = false, RaiseErrorOnInactivePeriod = true)]
			public virtual String NewFinPeriodID
			{
				get
				{
					return _NewFinPeriodID;
				}
				set
				{
					_NewFinPeriodID = value;
				}
			}
			#endregion
			#region NewTranDesc
			public abstract class newTranDesc : PX.Data.IBqlField
			{
			}
			protected String _NewTranDesc;

			[PXString(256, IsUnicode = true)]
			[PXUIField(DisplayName = "Transaction Description")]
			public virtual String NewTranDesc
			{
				get
				{
					return this._NewTranDesc;
				}
				set
				{
					this._NewTranDesc = value;
				}
			}
			#endregion
		}

		#endregion
	}
}
