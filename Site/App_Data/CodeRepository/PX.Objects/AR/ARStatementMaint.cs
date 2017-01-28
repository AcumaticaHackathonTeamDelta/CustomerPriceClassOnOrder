using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.BQLConstants;
using PX.Objects.AP;

namespace PX.Objects.AR
{
	public class ARStatementMaint : PXGraph<ARStatementMaint, ARStatementCycle>
	{
		public PXSelect<ARStatementCycle> ARStatementCycleRecord;
		public PXAction<ARStatementCycle> RecreateLast;

		[PXUIField(DisplayName = Messages.RegenerateLastStatement, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable recreateLast(PXAdapter adapter)
		{
			ARStatementCycle row = ARStatementCycleRecord.Current;
			if(row != null)
			{
				if (ARStatementProcess.CheckForUnprocessedPPD(this, row.StatementCycleId, row.LastStmtDate, null))
				{
					throw new PXSetPropertyException(Messages.UnprocessedPPDExists);
				}
			
				if (row.LastStmtDate != null)
				{
					PXLongOperation.StartOperation(this, delegate() { StatementCycleProcessBO.RegenerateLastStatement(new StatementCycleProcessBO(), row); });
				}
			}

			return adapter.Get();
		}
		public virtual void ARStatementCycle_Day01_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			ARStatementCycle row = (ARStatementCycle)e.Row;
			if (row != null && row.PrepareOn == PrepareOnType.Custom)
			{
				if (!IsCorrectDayOfMonth((Int16?)e.NewValue))
				{
					throw new PXSetPropertyException<ARStatementCycle.day01>(Messages.StatementCycleDayEmpty);
				} 
				if (IsInCorrectForSomeMonth((Int16?)e.NewValue))
				{
					cache.RaiseExceptionHandling<ARStatementCycle.day01>(e.Row, e.NewValue, new PXSetPropertyException(Messages.StatementCycleDayIncorrect, PXErrorLevel.Warning, e.NewValue));
				}
			}
		}
		public virtual void ARStatementCycle_Day00_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			ARStatementCycle row = (ARStatementCycle)e.Row;
			if (row != null && (row.PrepareOn == PrepareOnType.Custom || row.PrepareOn == PrepareOnType.FixedDayOfMonth))
			{
				if (!IsCorrectDayOfMonth((Int16?)e.NewValue))
				{
					throw new PXSetPropertyException<ARStatementCycle.day00>(Messages.StatementCycleDayEmpty);
				}
				if(IsInCorrectForSomeMonth((Int16?)e.NewValue))
				{
					cache.RaiseExceptionHandling<ARStatementCycle.day00>(e.Row,e.NewValue, new PXSetPropertyException(Messages.StatementCycleDayIncorrect, PXErrorLevel.Warning, e.NewValue));
				}
			}
		}
		static bool IsCorrectDayOfMonth(Int16? day)
		{
			return day != null && day > 0 && day <= 31;
		}
		static bool IsInCorrectForSomeMonth(Int16? day)
		{
			return day != null && day > 28;
		}
		public virtual void ARStatementCycle_PrepareOn_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			ARStatementCycle row = (ARStatementCycle)e.Row;
			if (row.PrepareOn == PrepareOnType.EndOfMonth)
			{
				row.Day00 = null;
				row.Day01 = null;
			}
			else if (row.PrepareOn == PrepareOnType.FixedDayOfMonth)
			{
				row.Day01 = null;
				if (!IsCorrectDayOfMonth(row.Day00))
				{
					row.Day00 = 1;
				}
			}
			else if (row.PrepareOn == PrepareOnType.Custom)
			{
				if (!IsCorrectDayOfMonth(row.Day00))
				{
					row.Day00 = 1;
				}
				if (!IsCorrectDayOfMonth(row.Day01))
				{
					row.Day01 = 1;
				}
			}
		}

		protected virtual void ARStatementCycle_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			if (e.Row == null) return;

			ARStatementCycle row = (ARStatementCycle)e.Row;
			PXUIFieldAttribute.SetEnabled<ARStatementCycle.day00>(cache, null, (row.PrepareOn == PrepareOnType.FixedDayOfMonth || row.PrepareOn == PrepareOnType.Custom));
			PXUIFieldAttribute.SetEnabled<ARStatementCycle.day01>(cache, null, (row.PrepareOn == PrepareOnType.Custom));
			PXUIFieldAttribute.SetEnabled<ARStatementCycle.finChargeID>(cache, null, (row.FinChargeApply??false));
			
			bool isRequired = row.FinChargeApply ?? false;
			PXDefaultAttribute.SetPersistingCheck<ARStatementCycle.finChargeID>(cache, row, isRequired ? PXPersistingCheck.Null : PXPersistingCheck.Nothing);
			PXUIFieldAttribute.SetRequired<ARStatementCycle.finChargeID>(cache, isRequired);
			PXUIFieldAttribute.SetEnabled<ARStatementCycle.requireFinChargeProcessing>(cache, null, (row.FinChargeApply ?? false));
		
		}
		protected virtual void ARStatementCycle_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
		{
			ARStatementCycle row = (ARStatementCycle)e.Row;
			if (row == null || e.Operation == PXDBOperation.Delete)
				return;
			if (row.PrepareOn == PrepareOnType.Custom || row.PrepareOn == PrepareOnType.FixedDayOfMonth)
			{
				if (!IsCorrectDayOfMonth(row.Day00))
				{
					cache.RaiseExceptionHandling<ARStatementCycle.day00>(e.Row, row.Day00, new PXSetPropertyException(Messages.StatementCycleDayEmpty, PXErrorLevel.Error));
					throw new PXSetPropertyException<ARStatementCycle.day00>(Messages.StatementCycleDayEmpty);
				}
				if (IsInCorrectForSomeMonth(row.Day00))
				{
					cache.RaiseExceptionHandling<ARStatementCycle.day00>(e.Row, row.Day00, new PXSetPropertyException(Messages.StatementCycleDayIncorrect, PXErrorLevel.Warning, row.Day00));
				}
			} 
			if (row.PrepareOn == PrepareOnType.Custom )
			{
				if (!IsCorrectDayOfMonth(row.Day01))
				{
					cache.RaiseExceptionHandling<ARStatementCycle.day01>(e.Row, row.Day01, new PXSetPropertyException(Messages.StatementCycleDayEmpty, PXErrorLevel.Error));
					throw new PXSetPropertyException<ARStatementCycle.day01>(Messages.StatementCycleDayEmpty);
				}
				if (IsInCorrectForSomeMonth(row.Day01))
				{
					cache.RaiseExceptionHandling<ARStatementCycle.day01>(e.Row, row.Day01, new PXSetPropertyException(Messages.StatementCycleDayIncorrect, PXErrorLevel.Warning, row.Day01));
				}
			}
		}
		protected virtual void ARStatementCycle_RowDeleting(PXCache cache, PXRowDeletingEventArgs e)
		{
			if (e.Row == null) return;
			PXSelectorAttribute.CheckAndRaiseForeignKeyException(cache, e.Row, typeof(ARStatement.statementCycleId));
			PXSelectorAttribute.CheckAndRaiseForeignKeyException(cache, e.Row, typeof(Customer.statementCycleId));
			PXSelectorAttribute.CheckAndRaiseForeignKeyException(cache, e.Row, typeof(CustomerClass.statementCycleId));
		}

		protected virtual void ARStatementCycle_FinChargeApply_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			ARStatementCycle row = (ARStatementCycle)e.Row;
			if (!(row.FinChargeApply ?? false))
			{
				row.FinChargeID = null;
				row.RequireFinChargeProcessing = false;
			}
		}
	}
}
