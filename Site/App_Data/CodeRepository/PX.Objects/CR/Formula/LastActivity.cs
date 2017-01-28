using PX.Data;
using System.Linq;

namespace PX.Objects.CR
{
	[PXUnboundFormula(typeof(Switch<Case<Where<CRSMEmail.incoming, Equal<True>>, True>, False>), typeof(LastActivity<CRActivityStatistics.lastIncomingActivityNoteID, CRSMEmail.noteID>))]
	[PXUnboundFormula(typeof(Switch<Case<Where<CRSMEmail.incoming, Equal<True>>, True>, False>), typeof(LastActivity<CRActivityStatistics.lastIncomingActivityDate, CRSMEmail.createdDateTime>))]
	[PXUnboundFormula(typeof(Switch<Case<Where<CRSMEmail.outgoing, Equal<True>>, True>, False>), typeof(LastActivity<CRActivityStatistics.lastOutgoingActivityNoteID, CRSMEmail.noteID>))]
	[PXUnboundFormula(typeof(Switch<Case<Where<CRSMEmail.outgoing, Equal<True>>, True>, False>), typeof(LastActivity<CRActivityStatistics.lastOutgoingActivityDate, CRSMEmail.createdDateTime>))]
	public sealed class CRSMEmailStatisticFormulas : PXAggregateAttribute { }

	[PXUnboundFormula(typeof(Switch<Case<Where<CRPMTimeActivity.incoming, Equal<True>>, True>, False>), typeof(LastActivity<CRActivityStatistics.lastIncomingActivityNoteID, CRPMTimeActivity.noteID>))]
	[PXUnboundFormula(typeof(Switch<Case<Where<CRPMTimeActivity.incoming, Equal<True>>, True>, False>), typeof(LastActivity<CRActivityStatistics.lastIncomingActivityDate, CRPMTimeActivity.createdDateTime>))]
	[PXUnboundFormula(typeof(Switch<Case<Where<CRPMTimeActivity.outgoing, Equal<True>>, True>, False>), typeof(LastActivity<CRActivityStatistics.lastOutgoingActivityNoteID, CRPMTimeActivity.noteID>))]
	[PXUnboundFormula(typeof(Switch<Case<Where<CRPMTimeActivity.outgoing, Equal<True>>, True>, False>), typeof(LastActivity<CRActivityStatistics.lastOutgoingActivityDate, CRPMTimeActivity.createdDateTime>))]
	public sealed class CRPMTimeActivityStatisticFormulas : PXAggregateAttribute { }

	[PXUnboundFormula(typeof(Switch<Case<Where<CRActivity.incoming, Equal<True>>, True>, False>), typeof(LastActivity<CRActivityStatistics.lastIncomingActivityNoteID, CRActivity.noteID>))]
	[PXUnboundFormula(typeof(Switch<Case<Where<CRActivity.incoming, Equal<True>>, True>, False>), typeof(LastActivity<CRActivityStatistics.lastIncomingActivityDate, CRActivity.createdDateTime>))]
	[PXUnboundFormula(typeof(Switch<Case<Where<CRActivity.outgoing, Equal<True>>, True>, False>), typeof(LastActivity<CRActivityStatistics.lastOutgoingActivityNoteID, CRActivity.noteID>))]
	[PXUnboundFormula(typeof(Switch<Case<Where<CRActivity.outgoing, Equal<True>>, True>, False>), typeof(LastActivity<CRActivityStatistics.lastOutgoingActivityDate, CRActivity.createdDateTime>))]
	public sealed class CRActivityStatisticFormulas : PXAggregateAttribute { }

	public sealed class LastActivity<TargetField, ReturnField> : IBqlUnboundAggregateCalculator
		where TargetField : IBqlField	
		where ReturnField : IBqlField
	{
		private static object CalcFormula(IBqlCreator formula, PXCache cache, object item)
		{
			object value = null;
			bool? result = null;

			BqlFormula.Verify(cache, item, formula, ref result, ref value);

			return value;
		}

		public object Calculate(PXCache cache, object row, IBqlCreator formula, object[] records, int digit)
		{
			if (records.Length < 1 || !(records[0] is CRActivity)) return null;

			return
				cache.GetValue<ReturnField>(
					records.Where(a => ((bool?) CalcFormula(formula, cache, a) == true))
						.OrderBy(a => ((CRActivity) a).CreatedDateTime)
						.LastOrDefault());
		}

		public object Calculate(PXCache cache, object row, object oldrow, IBqlCreator formula, int digit)
		{
			return null;
		}
	}
}
