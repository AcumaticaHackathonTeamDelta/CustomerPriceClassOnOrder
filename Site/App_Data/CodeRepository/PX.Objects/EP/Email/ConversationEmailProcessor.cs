using PX.Data;
using PX.SM;
using PX.Objects.CR;
using System;

namespace PX.Objects.EP
{
	public class ConversationEmailProcessor : BasicEmailProcessor
	{
		protected override bool Process(Package package)
		{
			CRSMEmail activity = package.Message;
			EMailAccount account = package.Account;
			PXGraph graph = package.Graph;

		    if (activity.Ticket != null
		        || activity.ParentNoteID != null
		        || account.EmailAccountType != EmailAccountTypesAttribute.Exchange
		        || !(account.IncomingProcessing ?? false))
		    {
		        return false;
		    }

			if (String.IsNullOrEmpty(activity.MessageId) || String.IsNullOrEmpty(activity.MessageReference)) return false;

			foreach (String id in activity.MessageReference.Split(' '))
			{
				Boolean found = false;
				foreach (CRSMEmail parent in PXSelect<CRSMEmail, Where<CRSMEmail.messageId, Like<Required<CRSMEmail.messageId>>>>.SelectSingleBound(graph, null, id))
				{
					activity.Ticket = parent.ID;
					found = true;
					break;
				}
				if (found) return true;
			}

			return false;
		}
	}
}
