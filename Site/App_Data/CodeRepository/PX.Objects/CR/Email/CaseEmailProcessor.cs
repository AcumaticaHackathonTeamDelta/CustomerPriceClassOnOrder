using System;
using System.Collections.Specialized;
using PX.Data;
using PX.Data.EP;
using PX.Objects.EP;

namespace PX.Objects.CR
{
	public class NewCaseEmailProcessor : BasicEmailProcessor
	{
		protected override bool Process(Package package)
		{
			var account = package.Account;
			if (account.IncomingProcessing != true ||
			    account.CreateCase != true )
			{
				return false;
			}

			var message = package.Message;

		    if (!string.IsNullOrEmpty(message.Exception) || message.IsIncome != true || message.RefNoteID != null) return false;


            var graph = PXGraph.CreateInstance<CRCaseMaint>();
			SetCRSetup(graph);
			SetAccessInfo(graph);
		    var copy = package.Graph.Caches[typeof (CRSMEmail)].CreateCopy(message);
		    try
		    {
                var caseCache = graph.Caches[typeof(CRCase)];
                var @case = (CRCase)caseCache.Insert();
			    @case = graph.Case.Search<CRCase.caseCD>(@case.CaseCD);
		        @case = PXCache<CRCase>.CreateCopy(@case);
                //@case.EMail = package.Address;
                @case.Subject = message.Subject;
                if (@case.Subject == null || @case.Subject.Trim().Length == 0)
                    @case.Subject = GetFromString(package.Address, package.Description);
                @case.Description = message.Body;

                if (account.CreateCaseClassID != null)
                    @case.CaseClassID = account.CreateCaseClassID;

                @case = PXCache<CRCase>.CreateCopy((CRCase)caseCache.Update(@case));

                var contact = FindContact(graph, package.Address);
		        if (contact != null)
		        {
		            @case.ContactID = contact.ContactID;
		            message.ContactID = contact.ContactID;
		        }
		        else
		        {
                    CRCaseClass caseClass = PXSelect<CRCaseClass, Where<CRCaseClass.caseClassID, Equal<Required<CRCaseClass.caseClassID>>>>.SelectSingleBound(graph, null, @case.CaseClassID);
                    if (caseClass == null || caseClass.RequireContact == true)
                        return false;
		        }

		        var bAccount = FindAccount(graph, contact);
		        if (bAccount != null)
		        {
		            PXCache cache = graph.Caches[typeof (BAccount)];
		            graph.EnsureCachePersistence(cache.GetItemType());
			        message.BAccountID = bAccount.BAccountID;
					@case.CustomerID = bAccount.BAccountID;		           
		        }

                message.RefNoteID = PXNoteAttribute.GetNoteID<CRCase.noteID>(graph.Caches[typeof(CRCase)], @case);               		        
                caseCache.Update(@case);  
		        graph.Activities.Cache.Current = message;
				graph.Save.PressImpl(false);                        
                	                                  
		    }
		    catch (Exception e)
		    {
                package.Graph.Caches[typeof(CRSMEmail)].RestoreCopy(message, copy);
		        throw new PXException(Messages.CreateCaseException, e is PXOuterException ? ("\r\n" + String.Join("\r\n", ((PXOuterException)e).InnerMessages)) : e.Message);
		    }
			return true;
		}

		private static string GetFromString(string address, string description)
		{
			return string.Format("From {0} {1}", description, address);
		}

		private void SetAccessInfo(PXGraph graph)
		{
			graph.Caches[typeof(AccessInfo)].Current = graph.Accessinfo;
		}

		private void SetCRSetup(PXGraph graph)
		{
			var crSetupCache = graph.Caches[typeof(CRSetup)];
			crSetupCache.Current = (CRSetup)PXSelect<CRSetup>.SelectWindowed(graph, 0, 1);
		}

		private BAccount FindAccount(PXGraph graph, Contact contact)
		{
			if (contact == null || contact.BAccountID == null) return null;

			PXSelect<BAccount,
				Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>,
                   And<Where<BAccount.type, Equal<BAccountType.prospectType>,
                           Or<BAccount.type, Equal<BAccountType.customerType>,
                           Or<BAccount.type, Equal<BAccountType.combinedType>>>>>>>.
				Clear(graph);

            var account = (BAccount)
                PXSelect<BAccount,
                Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>,
                   And<Where<BAccount.type, Equal<BAccountType.prospectType>,
                           Or<BAccount.type, Equal<BAccountType.customerType>,
                           Or<BAccount.type, Equal<BAccountType.combinedType>>>>>>>.
										Select(graph, contact.BAccountID);
			return account;
		}

		private Contact FindContact(PXGraph graph, string address)
		{
			PXSelect<Contact,
				Where<Contact.eMail, Equal<Required<Contact.eMail>>, And<Contact.contactType, Equal<ContactTypesAttribute.person>>>>.
				Clear(graph);
            var contact = (Contact)PXSelect<Contact,
                Where<Contact.eMail, Equal<Required<Contact.eMail>>, And<Contact.contactType, Equal<ContactTypesAttribute.person>>>>.
										SelectWindowed(graph, 0, 1, address);
			return contact;
		}
	}

	public class CaseCommonEmailProcessor : BasicEmailProcessor
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

			PXSelect<CRCase,
				Where<CRCase.noteID, Equal<Required<CRCase.noteID>>>>.
				Clear(graph);

			var @case = (CRCase)PXSelect<CRCase,
				Where<CRCase.noteID, Equal<Required<CRCase.noteID>>>>.
				Select(graph, message.RefNoteID);

			if (@case == null || @case.CaseID == null) return false;
			if (@case != null && message.OwnerID == null && account.EmailAccountType == PX.SM.EmailAccountTypesAttribute.Standard)
			{
				try
				{
					message.WorkgroupID = @case.WorkgroupID;
					graph.Caches[typeof(CRSMEmail)].SetValueExt<CRSMEmail.ownerID>(message, @case.OwnerID);
				}
				catch (PXSetPropertyException)
				{
					message.OwnerID = null;
				}
			}

			if (!RouterEmailProcessor.IsOwnerEqualUser(graph, message, @case.OwnerID) &&
				 (@case.MajorStatus != CRCaseMajorStatusesAttribute._CLOSED &&
				 @case.MajorStatus != CRCaseMajorStatusesAttribute._RELEASED &&
				 @case.Released != true && (@case.Status == CRCaseStatusesAttribute._PENDING_CUSTOMER || @case.Status == CRCaseStatusesAttribute._OPEN)))
			{
				var caseCache = graph.Caches[typeof(CRCase)];
				var newCase = (CRCase)caseCache.CreateCopy(@case);
				newCase.MajorStatus = CRCaseMajorStatusesAttribute._OPEN;
				newCase.Status = CRCaseStatusesAttribute._OPEN;
				newCase.Resolution = CRCaseResolutionsAttribute._CUSTOMER_REPLIED;
				newCase = (CRCase)caseCache.Update(newCase);
				PersistRecord(package, newCase);
			}
			
			return true;
		}
	}
}
