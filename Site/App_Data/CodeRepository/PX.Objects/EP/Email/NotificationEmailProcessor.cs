using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using PX.Common;
using PX.Common.Mail;
using PX.Data;
using PX.Data.EP;
using PX.Objects.CR;
using PX.SM;

namespace PX.Objects.EP
{
	public class NotificationEmailProcessor : BasicEmailProcessor
	{
		protected override bool Process(Package package)
		{
			var message = package.Message;
			if (message.IsIncome != true) return false;

			var fromInternal = message.ClassID == CRActivityClass.EmailRouting;

			var targetAddresses = fromInternal ? GetFromInternal(package) : GetFromExternal(package);

			var email = ConvertAddressesToString(targetAddresses);
			if (string.IsNullOrEmpty(email)) return false;

			SendCopyMessage(package.Graph, package.Account, message, email);
			return true;
		}

		private IEnumerable<Mailbox> GetFromInternal(Package package)
		{
			var message = package.Message;
			var graph = package.Graph;

			var routingMessage = GetRoutingMessage(graph, message);
			if (routingMessage == null) return null;

			var refOwner = FindOwnerAddress(graph, routingMessage.RefNoteID, message.OwnerID);			
			var targetAddresses = GenerateAddresses(refOwner);

			targetAddresses = RemoveAddress(targetAddresses, message.MailFrom);
			targetAddresses = RemoveAddress(targetAddresses, message.MailTo);
			targetAddresses = RemoveAddress(targetAddresses, message.MailCc);
			targetAddresses = RemoveAddress(targetAddresses, message.MailBcc);

			targetAddresses = RemoveAddress(targetAddresses, routingMessage.MailFrom);
			targetAddresses = RemoveAddress(targetAddresses, routingMessage.MailTo);
			targetAddresses = RemoveAddress(targetAddresses, routingMessage.MailCc);
			targetAddresses = RemoveAddress(targetAddresses, routingMessage.MailBcc);
			return targetAddresses;
		}

		private IEnumerable<Mailbox> GetFromExternal(Package package)
		{
			var message = package.Message;
			var graph = package.Graph;

			var refOwner = FindOwnerAddress(graph, message.RefNoteID, message.OwnerID);			
			var targetAddresses = GenerateAddresses(refOwner);

			targetAddresses = RemoveMailbox(targetAddresses, message.MailFrom);
			targetAddresses = RemoveAddress(targetAddresses, message.MailTo);
			targetAddresses = RemoveAddress(targetAddresses, message.MailCc);
			targetAddresses = RemoveAddress(targetAddresses, message.MailBcc);

			var routingMessage = GetRoutingMessage(graph, message);
			if (routingMessage != null)
			{
				targetAddresses = RemoveMailbox(targetAddresses, routingMessage.MailFrom);
				targetAddresses = RemoveAddress(targetAddresses, routingMessage.MailTo);
				targetAddresses = RemoveAddress(targetAddresses, routingMessage.MailCc);
				targetAddresses = RemoveAddress(targetAddresses, routingMessage.MailBcc);
			}
			return targetAddresses;
		}

		private IEnumerable<Mailbox> RemoveMailbox(IEnumerable<Mailbox> source, string email)
		{
			email = email.With(_ => _.Trim());
			var addresses = new HybridDictionary();
			if (!string.IsNullOrEmpty(email))
				foreach (Mailbox mailbox in MailboxList.Parse(email))
					if (!string.IsNullOrEmpty(mailbox.Address))
						addresses.Add(mailbox.Address.ToLower(), mailbox);
			foreach (Mailbox item in source)
				if (addresses.Count == 0 || !addresses.Contains(item.Address.ToLower()))
					yield return item;
		}

		private IEnumerable<Mailbox> RemoveAddress(IEnumerable<Mailbox> source, string email)
		{
			email = email.With(_ => _.Trim());
			HybridDictionary addresses = new HybridDictionary();
			if (!string.IsNullOrEmpty(email))
				foreach (PX.Common.Mail.Address address in AddressList.Parse(email))
				{
					Group group = address as Group;
					if (group != null)
					{
						foreach (Mailbox mailbox in @group.Members.Where(mailbox => !string.IsNullOrEmpty(mailbox.Address) && !addresses.Contains(mailbox.Address.ToLower())))
							addresses.Add(mailbox.Address.ToLower(), mailbox);
					}
					else
					{
						Mailbox mailbox = (Mailbox)address;
						if (!string.IsNullOrEmpty(mailbox.Address) && !addresses.Contains(mailbox.Address.ToLower()))
							addresses.Add(mailbox.Address.ToLower(), mailbox);
					}
				}
			return source.Where(item => addresses.Count == 0 || !addresses.Contains(item.Address.ToLower()));
		}

		private string ConvertAddressesToString(IEnumerable<Mailbox> addresses)
		{
			MailboxList list = new MailboxList();
			if (addresses != null)
				foreach (Mailbox address in addresses)
					list.Add(address);
			return list.ToString();
		}

		private IEnumerable<Mailbox> GenerateAddresses(params Mailbox[] addresses)
		{
			if (addresses != null)
				foreach (Mailbox address in addresses.Where(address => address != null))
					yield return address;
		}

		private Mailbox FindOwnerAddress(PXGraph graph, Guid? noteId, Guid? mainOwner)
		{
			if (noteId == null) return null;

			var source = FindSource(graph, noteId.Value);
			if (source == null) return null;

			Contact owner;
			Users user;
			FindOwner(graph, source as IAssign, out owner, out user);
			if (user == null) return null;
			if (mainOwner == user.PKID) return null;

			return GenerateAddress(owner, user);
		}

		private CRSMEmail GetRoutingMessage(PXGraph graph, CRSMEmail message)
		{
			return (CRSMEmail)PXSelect<CRSMEmail,
									Where<CRSMEmail.parentNoteID, Equal<Required<CRSMEmail.parentNoteID>>>>.
									SelectWindowed(graph, 0, 1, message.NoteID);
		}

		private Mailbox GenerateAddress(Contact employee, Users user)
		{
			string displayName = null;
			string address = null;
			if (user != null && user.PKID != null)
			{
				var userDisplayName = user.FullName.With(_ => _.Trim());
				if (!string.IsNullOrEmpty(userDisplayName))
					displayName = userDisplayName;
				var userAddress = user.Email.With(_ => _.Trim());
				if (!string.IsNullOrEmpty(userAddress))
					address = userAddress;
			}
			if (employee != null && employee.BAccountID != null)
			{
				var employeeDisplayName = employee.DisplayName.With(_ => _.Trim());
				if (!string.IsNullOrEmpty(employeeDisplayName))
					displayName = employeeDisplayName;
				var employeeAddress = employee.EMail.With(_ => _.Trim());
				if (!string.IsNullOrEmpty(employeeAddress))
					address = employeeAddress;
			}
			return string.IsNullOrEmpty(address) ? null : new Mailbox(displayName, address);
		}

		private void SendCopyMessage(PXGraph graph, EMailAccount account, CRSMEmail message, string email)
		{
			var cache = graph.Caches[message.GetType()];
			var copy = (CRSMEmail)cache.CreateCopy(message);
			copy.NoteID = null;
			copy.EmailNoteID = null;
			copy.IsIncome = false;
			copy.ParentNoteID = message.NoteID;
			copy.MailTo = email; //TODO: need add address description
			copy.MailCc = null;
			copy.MailBcc = null;
			copy.MPStatus = MailStatusListAttribute.PreProcess;
			copy.ClassID = CRActivityClass.EmailRouting;
			new AddInfoEmailProcessor().Process(new EmailProcessEventArgs(graph, account, copy));
			copy.RefNoteID = null;
			copy.BAccountID = null;
			copy.ContactID = null;
			copy.Pop3UID = null;
			copy.ImapUID = null;
			var imcUid = Guid.NewGuid();
			copy.ImcUID = imcUid;
			copy.MessageId = this.GetType().Name + "_" + imcUid.ToString().Replace("-", string.Empty);
			copy.OwnerID = null;
			copy.WorkgroupID = null;

			copy = (CRSMEmail)cache.CreateCopy(cache.Insert(copy));

			//Update owner and reset owner if employee not found
			copy.OwnerID = message.OwnerID;
			try
			{
				cache.Update(copy);
			}
			catch (PXSetPropertyException)
			{
				copy.OwnerID = null;
				copy =  (CRSMEmail)cache.CreateCopy(cache.Update(copy));
			}

			copy.WorkgroupID = message.WorkgroupID;

			var noteFiles = PXNoteAttribute.GetFileNotes(cache, message);
			if (noteFiles != null)
				PXNoteAttribute.SetFileNotes(cache, copy, noteFiles);
			graph.EnsureCachePersistence(copy.GetType());
		}

		private object FindSource(PXGraph graph, Guid noteId)
		{
			return new EntityHelper(graph).GetEntityRow(noteId);
		}

		private void FindOwner(PXGraph graph, IAssign source, out Contact employee, out Users user)
		{
			employee = null;
			user = null;
			if (source == null || source.OwnerID == null) return;

			PXSelectJoin<Users,
				LeftJoin<EPEmployee, On<EPEmployee.userID, Equal<Users.pKID>>,
				LeftJoin<Contact, On<Contact.contactID, Equal<EPEmployee.defContactID>>>>, 
				Where<Users.pKID, Equal<Required<Users.pKID>>>>.
				Clear(graph);
			var row = (PXResult<Users, EPEmployee, Contact>)PXSelectJoin<Users,
				LeftJoin<EPEmployee, On<EPEmployee.userID, Equal<Users.pKID>>,
				LeftJoin<Contact, On<Contact.contactID, Equal<EPEmployee.defContactID>>>>,
				Where<Users.pKID, Equal<Required<Users.pKID>>>>.
				Select(graph, source.OwnerID);

			employee = (Contact)row;
			user = (Users)row;
		}
	}
}
