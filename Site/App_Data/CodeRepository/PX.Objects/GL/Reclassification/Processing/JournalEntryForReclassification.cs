using PX.Data;

namespace PX.Objects.GL.Reclassification.Processing
{
	public class JournalEntryForReclassification : JournalEntry
	{
		protected override void Batch_DateEntered_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
		}

		protected override void Batch_FinPeriodID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
		}

		protected override void GLTran_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
		}

		protected virtual void GLSetup_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
		{
			e.Cancel = true;
		}

		protected override void Batch_DateEntered_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
		}
	}
}
