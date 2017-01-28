using PX.Common;
using PX.Data;
using PX.Objects.EP;

namespace PX.Objects.CR
{
	public class OpportunityCommonEmailProcessor : BasicEmailProcessor
	{
		protected override bool Process(Package package)
		{
			var account = package.Account;
			if (account.IncomingProcessing != true)
			{
				return false;
			}

			var message = package.Message;
			if (message.IsIncome != true) return false;
			if (message.RefNoteID == null) return false;


			var graph = package.Graph;

			PXSelect<CROpportunity,
				Where<CROpportunity.noteID, Equal<Required<CROpportunity.noteID>>>>.
				Clear(graph);

			var opportunity = (CROpportunity)PXSelect<CROpportunity,
				Where<CROpportunity.noteID, Equal<Required<CROpportunity.noteID>>>>.
				Select(graph, message.RefNoteID);

			if (opportunity == null || opportunity.OpportunityID == null) return false;			

			return true;
		}
	}
}
