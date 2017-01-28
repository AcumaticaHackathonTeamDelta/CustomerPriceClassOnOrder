using PX.Common;
using PX.Data;

using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.DR.Descriptor;

namespace PX.Objects.DR
{
	public static class ScheduleHelper
	{
		/// <summary>
		/// Checks if deferral code has been changed or removed from the line.
		/// If so, ensures the removal of any associated deferral schedules.
		/// </summary>
		public static void DeleteAssociatedScheduleIfDeferralCodeChanged(
			PXGraph graph,
			IDocumentLine documentLine)
		{
			IDocumentLine oldLine;

			// Obtain the document line last saved into the database
			// to check if the new line's deferral code differs from it.
			// -
			if (documentLine.Module == GL.BatchModule.AR)
			{
				oldLine = (ARTran)PXSelectReadonly<
					ARTran,
					Where<
						ARTran.tranType, Equal<Required<ARTran.tranType>>,
						And<ARTran.refNbr, Equal<Required<ARTran.refNbr>>,
						And<ARTran.lineNbr, Equal<Required<ARTran.lineNbr>>>>>>
					.Select(graph, documentLine.TranType, documentLine.RefNbr, documentLine.LineNbr);
			}
			else if (documentLine.Module == GL.BatchModule.AP)
			{
				oldLine = (APTran)PXSelectReadonly<
					APTran,
					Where<
						APTran.tranType, Equal<Required<APTran.tranType>>,
						And<APTran.refNbr, Equal<Required<APTran.refNbr>>,
							And<APTran.lineNbr, Equal<Required<APTran.lineNbr>>>>>>
					.Select(graph, documentLine.TranType, documentLine.RefNbr, documentLine.LineNbr);
			}
			else
			{
				throw new PXException(Messages.UnexpectedDocumentLineModule);
			}

			DRSchedule correspondingSchedule = PXSelect<
				DRSchedule,
				Where<
					DRSchedule.module, Equal<Required<DRSchedule.module>>,
					And<DRSchedule.docType, Equal<Required<DRSchedule.docType>>,
					And<DRSchedule.refNbr, Equal<Required<DRSchedule.refNbr>>,
					And<DRSchedule.lineNbr, Equal<Required<DRSchedule.lineNbr>>>>>>>
				.Select(
					graph, 
					documentLine.Module, 
					documentLine.TranType, 
					documentLine.RefNbr, 
					documentLine.LineNbr);

			if (correspondingSchedule == null) return;

			bool deferralCodeRemoved = documentLine.DeferredCode == null;

			bool deferralCodeChanged =
				oldLine?.DeferredCode != null &&
				documentLine.DeferredCode != null &&
				oldLine.DeferredCode != documentLine.DeferredCode;

			if (deferralCodeRemoved || deferralCodeChanged)
			{
				DraftScheduleMaint scheduleGraph = PXGraph.CreateInstance<DraftScheduleMaint>();

				scheduleGraph.Schedule.Current = correspondingSchedule;

				scheduleGraph
					.Components
					.Select()
					.ForEach(component => scheduleGraph.Components.Delete(component));

				scheduleGraph.Schedule.Delete(correspondingSchedule);

				scheduleGraph.Save.Press();
			}
		}
	}
}
