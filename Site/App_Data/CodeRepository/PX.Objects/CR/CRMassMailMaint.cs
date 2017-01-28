using System;
using System.Collections;
using System.Collections.Generic;
using PX.Common;
using PX.Data;
using PX.Data.EP;
using PX.Objects.EP;
using PX.SM;
using PX.TM;
using PX.Objects.CS;

namespace PX.Objects.CR
{
	#region CRMassMailPreview

	[PXCacheName(Messages.PreviewSettings)]
	[Serializable]
	[PXHidden]
	public partial class CRMassMailPreview : IBqlTable
	{
		#region MailAccountID
		public abstract class mailAccountID : PX.Data.IBqlField { }

		[PXDBInt]
		[PXUIField(DisplayName = "From")]
		[PXEMailAccountIDSelectorAttribute]
		[PXDefault]
		public virtual int? MailAccountID { get; set; }
		#endregion

		#region MailTo
		public abstract class mailTo : IBqlField
		{
		}
		protected string _MailTo;
		[PXString(255)]
		[PXUIField(DisplayName = "To")]
		[PXDefault]
		public virtual string MailTo
		{
			get
			{
				return this._MailTo;
			}
			set
			{
				this._MailTo = value;
			}
		}
		#endregion
	}

	#endregion

	public class CRMassMailMaint : PXGraph<CRMassMailMaint, CRMassMail>
	{
		#region Recipient

		private class Recipient
		{
			private readonly object _entity;
			private readonly string _format;

			public Recipient(object entity, string format)
			{
				_entity = entity;
				_format = format;
			}

			public object Entity
			{
				get { return _entity; }
			}

			public string Format
			{
				get { return _format; }
			}
		}

		#endregion

		#region CampaignRecipient

		private class CampaignRecipient : Recipient
		{
			private readonly CRCampaignMembers _member;
			private readonly string _distinationStatus;

			public CampaignRecipient(object entity, string format, CRCampaignMembers member, string distinationStatus) 
				: base(entity, format)
			{
				_member = member;
				_distinationStatus = distinationStatus;
			}

			public CRCampaignMembers Member
			{
				get { return _member; }
			}

			public string DistinationStatus
			{
				get { return _distinationStatus; }
			}
		}

		#endregion

		#region Constants

		private const string _MAILTO_DEFAULT = "((Email))";

		#endregion

		#region Selects

		[PXHidden]
		[PXCopyPasteHiddenView]
		public PXSetup<CRSetup>
			Setup;

		[PXHidden]
		[PXCopyPasteHiddenView]
		public PXSelect<Contact>
			BaseContacts;

		[PXViewName(Messages.MassMailSummary)]
		[CRMassEmailLoadTemplate(typeof(CRMassMail),
			ContentField = typeof(CRMassMail.mailContent))]
		[PXCopyPasteHiddenFields(typeof(CRMassMail.mailContent))]
		public PXSelect<CRMassMail>
			MassMails;

		[PXViewName(Messages.History)]
		[PXFilterable]
		[PXCopyPasteHiddenView]
        [PXViewDetailsButton]
		public PXSelectJoin<CRSMEmail,
			InnerJoin<CRMassMailMessage,
				On<CRMassMailMessage.messageID, Equal<CRSMEmail.imcUID>>>,
			Where<CRMassMailMessage.massMailID, Equal<Optional<CRMassMail.massMailID>>>>
			History;

		[PXHidden]
		[PXCopyPasteHiddenView]
		public PXSelect<CRMassMailMessage, 
			Where<CRMassMailMessage.massMailID, Equal<Current<CRMassMail.massMailID>>>>
			SendedMessages;

		[PXViewName(Messages.EntityFields)]
		[PXCopyPasteHiddenView]
		public PXSelectOrderBy<CacheEntityItem,
			OrderBy<Asc<CacheEntityItem.number>>> 
			EntityItems;

		[PXViewName(Messages.MailLists)]
		[PXFilterable]
		[PXCopyPasteHiddenView]
		public PXSelectJoin<CRMarketingList,
			LeftJoin<CRMassMailMarketingList,
				On<CRMassMailMarketingList.mailListID, Equal<CRMarketingList.marketingListID>,
					And<CRMassMailMarketingList.massMailID, Equal<Current<CRMassMail.massMailID>>>>>,
            Where<CRMarketingList.isActive, Equal<boolTrue>>,
			OrderBy<Asc<CRMarketingList.name>>> 
			MailLists;

		[PXViewName(Messages.Campaigns)]
		[PXFilterable]
		[PXCopyPasteHiddenView]
		public PXSelectJoin<CRCampaign,
			LeftJoin<CRMassMailCampaign,
				On<CRMassMailCampaign.campaignID, Equal<CRCampaign.campaignID>, 
					And<CRMassMailCampaign.massMailID, Equal<Current<CRMassMail.massMailID>>>>>,
            Where<CRCampaign.isActive, Equal<boolTrue>>, 
			OrderBy<Asc<CRCampaign.campaignName>>>
			Campaigns;

		[PXViewName(Messages.LeadsAndContacts)]
		[PXFilterable]
		[PXCopyPasteHiddenView]
		public PXSelectJoin<Contact,
			LeftJoin<BAccount, On<BAccount.bAccountID, Equal<Contact.bAccountID>>,
			LeftJoin<Address, On<Address.addressID, Equal<Contact.defAddressID>>, 
			LeftJoin<CRMassMailMember, 
				On<CRMassMailMember.contactID, Equal<Contact.contactID>,
					And<CRMassMailMember.massMailID, Equal<Current<CRMassMail.massMailID>>>>>>>,
			Where2<Where<Contact.noMassMail, IsNull, Or<Contact.noMassMail, NotEqual<True>>>, 
				 And<Where<Contact.noEMail, IsNull, Or<Contact.noEMail, NotEqual<True>>>>>,
			OrderBy<Asc<Contact.displayName, Asc<Contact.contactID>>>>
			Leads;

		[PXViewName(Messages.Preview)]
		[PXCopyPasteHiddenView]
		public PXFilter<CRMassMailPreview> 
			Preview;

		[PXHidden]
		[PXCopyPasteHiddenView]
		public PXSelect<CRMassMailMarketingList,
			Where<CRMassMailMarketingList.massMailID, Equal<Required<CRMassMail.massMailID>>>> 
			selectedMailList;

		[PXHidden]
		[PXCopyPasteHiddenView]
		public PXSelect<CRMassMailCampaign,
			Where<CRMassMailCampaign.massMailID, Equal<Required<CRMassMail.massMailID>>>>
			selectedCampaigns;

		[PXHidden]
		[PXCopyPasteHiddenView]
		public PXSelect<CRMassMailMember,
			Where<CRMassMailMember.massMailID, Equal<Required<CRMassMail.massMailID>>>>
			selectedLeads;

		[PXHidden]
		[PXCopyPasteHiddenView]
		public PXSelectJoin<Contact,
			InnerJoin<CRMarketingListMember,
				On<CRMarketingListMember.contactID, Equal<Contact.contactID>>>> DynamicSourceList;
									
		#endregion

		#region Ctors

		public CRMassMailMaint()
		{
			if (string.IsNullOrEmpty(Setup.Current.MassMailNumberingID))
				throw new PXSetPropertyException(Messages.NumberingIDIsNull, Messages.CRSetup);

			PXUIFieldAttribute.SetEnabled(Campaigns.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<CRCampaign.selected>(Campaigns.Cache, null, true);
			PXUIFieldAttribute.SetEnabled<CRCampaign.targetStatus>(Campaigns.Cache, null, true);
			PXUIFieldAttribute.SetEnabled<CRCampaign.destinationStatus>(Campaigns.Cache, null, true);

			PXUIFieldAttribute.SetDisplayName<Contact.fullName>(Leads.Cache, Messages.CompanyName);
			PXUIFieldAttribute.SetDisplayName<BAccount.classID>(Caches[typeof(BAccount)], Messages.CompanyClass);
			PXDBAttributeAttribute.Activate(BaseContacts.Cache);
		}

		#endregion

		#region Data Handlers

		protected virtual IEnumerable dynamicSourceList([PXInt] int mailListID)
		{
			return CRSubscriptionsSelect.Select(this, mailListID);
		}
		protected virtual IEnumerable entityItems([PXString] string parent)
		{
			if (MassMails.Current == null) return new CacheEntityItem[0];
            
            return EMailSourceHelper.TemplateEntity(this, parent, typeof(Contact).FullName, null);
		}

		protected virtual IEnumerable mailLists()
		{
			foreach (PXResult row in MailLists.View.QuickSelect())
			{
				var list = (CRMarketingList)row[typeof(CRMarketingList)];
				var mailList = (CRMassMailMarketingList)row[typeof(CRMassMailMarketingList)];
				if (list.Selected != true && mailList.MailListID != null &&
					MailLists.Cache.GetStatus(list) != PXEntryStatus.Updated)
				{
					list.Selected = true;
				}
				yield return new PXResult<CRMarketingList>(list);
			}
		}

		protected virtual IEnumerable campaigns()
		{
			foreach (PXResult row in Campaigns.View.QuickSelect())
			{
				var campaign = (CRCampaign)row[typeof(CRCampaign)];
				var mailCampaign = (CRMassMailCampaign)row[typeof(CRMassMailCampaign)];
				if (campaign.Selected != true && mailCampaign.CampaignID != null &&
					Campaigns.Cache.GetStatus(campaign) != PXEntryStatus.Updated)
				{
					campaign.Selected = true;
					campaign.TargetStatus = mailCampaign.TargetStatus;
					campaign.DestinationStatus = mailCampaign.DestinationStatus;
				}
				yield return new PXResult<CRCampaign>(campaign);
			}
		}

		protected virtual IEnumerable leads()
		{
			foreach(PXResult row in Leads.View.QuickSelect())
			{
				var contact = (Contact)row[typeof(Contact)];
				var mailLead = (CRMassMailMember)row[typeof(CRMassMailMember)];
				if (contact.Selected != true && mailLead.ContactID != null && 
					Leads.Cache.GetStatus(contact) != PXEntryStatus.Updated)
				{
					contact.Selected = true;
				}
				var bAccount = (BAccount)row[typeof(BAccount)];
				var address = (Address)row[typeof(Address)];
				yield return new PXResult<Contact, BAccount, Address>(contact, bAccount, address);
			}
		}

		#endregion

		#region Event Handlers

		[PXUIField(DisplayName = "Display Name", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[PXDBString(255, IsUnicode = true)]
		[PXFieldDescription]
		[PXNavigateSelector(typeof(Search<Contact.displayName>))]
		protected virtual void Contact_DisplayName_CacheAttached(PXCache sender)
		{

		}

		[PXDBGuid]
		[PXOwnerSelector(typeof(Contact.workgroupID))]
		[PXChildUpdatable(AutoRefresh = true, TextField = "AcctName", ShowHint = false)]
		[PXUIField(DisplayName = "Owner", Visibility = PXUIVisibility.SelectorVisible)]
		protected virtual void Contact_OwnerID_CacheAttached(PXCache sender)
		{

		}

		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXUIField(DisplayName = "Class ID")]
		[PXNavigateSelector(typeof(CRContactClass.classID), DescriptionField = typeof(CRContactClass.description), CacheGlobal = true)]
		protected virtual void Contact_ClassID_CacheAttached(PXCache sender)
		{

		}

		[PXDBString(1)]
		[PXUIField(DisplayName = "Source")]
		[CRMSources]
		protected virtual void Contact_Source_CacheAttached(PXCache sender)
		{

		}

		[PXDBString(1)]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible)]
		[LeadStatuses]
		protected virtual void Contact_Status_CacheAttached(PXCache sender)
		{
			
		}

		[PXDBString(2, IsFixed = true)]
		[PXUIField(DisplayName = "Reason")]
		[LeadResolutions]
		protected virtual void Contact_Resolution_CacheAttached(PXCache sender)
		{

		}


		protected virtual void CRMassMail_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			var row = (CRMassMail)e.Row;
			if (row == null) return;

			CorrectUI(cache, row);
		}

		protected virtual void CRMassMail_RowInserted(PXCache cache, PXRowInsertedEventArgs e)
		{
			((CRMassMail)e.Row).MailTo = _MAILTO_DEFAULT;
		}

		protected virtual void CRMarketingList_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
		{
			e.Cancel = true;
		}

		protected virtual void CRCampaign_RowUpdating(PXCache cache, PXRowUpdatingEventArgs e)
		{
			var row = (CRCampaign)e.NewRow;
			if (row == null) return;

			if (row.TargetStatus == null)
			{
				cache.RaiseExceptionHandling<CRCampaign.targetStatus>
					(row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(CRCampaign.targetStatus).Name));
			}
			if (row.DestinationStatus == null)
			{
				cache.RaiseExceptionHandling<CRCampaign.destinationStatus>
					(row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(CRCampaign.destinationStatus).Name));
			}
		}

		protected virtual void Contact_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
		{
			e.Cancel = true;
		}

		protected virtual void CRCampaign_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
		{
			e.Cancel = true;
		}

		protected virtual void CRMassMailPreview_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
		{
			e.Cancel = true;
		}

		#endregion

		#region Overrides

		public override void Persist()
		{
			saveMailLists();
			saveCampaigns();
			saveLeads();

			base.Persist();

			CorrectUI(MassMails.Cache, MassMails.Current);
		}

		#endregion

		#region Actions

		public PXAction<CRMassMail> PreviewMail;
		[PXUIField(DisplayName = Messages.PreviewMessage)]
		[PXButton]
		public virtual IEnumerable previewMail(PXAdapter a)
		{
			if (Preview.AskExt(
				(graph, name) =>
					{
						Preview.Current.MailAccountID = MassMails.Current.With(current => current.MailAccountID)
							?? MailAccountManager.DefaultMailAccountID;
						Preview.Current.MailTo = MailAccountManager.GetDefaultEmailAccount(false).With(_ => _.Address);
					}) == WebDialogResult.OK)
			{
				Preview.View.Answer = WebDialogResult.OK;
				CheckFields(Preview.Cache, Preview.Current,
							typeof(CRMassMailPreview.mailAccountID),
							typeof(CRMassMailPreview.mailTo));
				Preview.View.Answer = WebDialogResult.None;

				var mails = GetMailsForSending(true);
				if (mails.Count == 0) throw new PXException(Messages.RecipientsNotFound);

				var mailTo = Preview.Current.MailTo;
				var accountId = Preview.Current.MailAccountID;

				SendMassMail(accountId, mailTo, null, null, mails, false);
				Save.Press();
			}

			yield return MassMails.Current;
		}

		public PXAction<CRMassMail> Send;
		[PXUIField(DisplayName = Messages.Send)]
		[PXSendMailButton]
		public virtual IEnumerable send(PXAdapter a)
		{
			CheckFields(MassMails.Cache, MassMails.Current,
						typeof(CRMassMail.mailAccountID),
						typeof(CRMassMail.mailSubject),
						typeof(CRMassMail.mailTo),
						typeof(CRMassMail.plannedDate));
			if (SendMails())
			{
				MassMails.Current.Status = CRMassMailStatusesAttribute.Send;
				MassMails.Current.SentDateTime = DateTime.Now;
				MassMails.Update(MassMails.Current);
				Save.Press();
			}
			yield return MassMails.Current;
		}

		public PXAction<CRMassMail> MessageDetails;
		[PXUIField(Visible = false)]
		[PXButton]
		public virtual IEnumerable messageDetails(PXAdapter a)
		{
			PXRedirectHelper.TryOpenPopup(History.Cache, History.Current, string.Empty);
			
			yield return MassMails.Current;
		}

		#endregion

		#region Private Methods

		private void CorrectUI(PXCache cache, CRMassMail row)
		{
            if (row == null) return;

			var isEnabled = row.Status != CRMassMailStatusesAttribute.Send;
			PXUIFieldAttribute.SetEnabled<CRMassMail.massMailID>(MassMails.Cache, row);
			PXUIFieldAttribute.SetEnabled<CRMassMail.mailAccountID>(MassMails.Cache, row, isEnabled);
			PXUIFieldAttribute.SetEnabled<CRMassMail.mailSubject>(MassMails.Cache, row, isEnabled);
			PXUIFieldAttribute.SetEnabled<CRMassMail.mailTo>(MassMails.Cache, row, isEnabled);
			PXUIFieldAttribute.SetEnabled<CRMassMail.mailCc>(MassMails.Cache, row, isEnabled);
			PXUIFieldAttribute.SetEnabled<CRMassMail.mailBcc>(MassMails.Cache, row, isEnabled);
			PXUIFieldAttribute.SetEnabled<CRMassMail.mailContent>(MassMails.Cache, row, isEnabled);
			PXUIFieldAttribute.SetEnabled<CRMassMail.source>(MassMails.Cache, row, isEnabled);
			PXUIFieldAttribute.SetEnabled<CRMassMail.sourceType>(MassMails.Cache, row, isEnabled);
			PXUIFieldAttribute.SetEnabled<CRMassMail.plannedDate>(MassMails.Cache, row, isEnabled);
			PXUIFieldAttribute.SetEnabled<CRMassMail.status>(MassMails.Cache, row, isEnabled);
			PXUIFieldAttribute.SetEnabled<CRMassMail.sentDateTime>(MassMails.Cache, row, false);
			MailLists.Cache.AllowUpdate = isEnabled;
			Leads.Cache.AllowUpdate = isEnabled;
			Campaigns.Cache.AllowUpdate = isEnabled;

			var isNotInserted = cache.GetStatus(row) != PXEntryStatus.Inserted;
			Send.SetEnabled(isEnabled && isNotInserted);
			PreviewMail.SetEnabled(isEnabled && isNotInserted);
		}

		private bool SendMails()
		{
			if (MassMails.Current == null || MassMails.Current.Status == CRMassMailStatusesAttribute.Send)
				return false;

			var mails = GetMailsForSending(false);
			if (mails.Count == 0) throw new PXException(Messages.RecipientsNotFound);

			var confirmMessage = PXMessages.LocalizeFormatNoPrefix(Messages.MassMailSend, mails.Count);
			if (MassMails.Ask(Messages.Confirmation, confirmMessage, MessageButtons.YesNo) != WebDialogResult.Yes)
			{
				return false;
			}

			var mailAccountId = MassMails.Current.MailAccountID;
			var mailTo = MassMails.Current.MailTo;
			var mailCc = MassMails.Current.MailCc;
			var mailBcc = MassMails.Current.MailBcc;

			SendMassMail(mailAccountId, mailTo, mailCc, mailBcc, mails, true);

			UpdateCampaignMembers(mails);

			return true;
		}

		private void SendMassMail(int? accountId, string mailTo, string mailCc, string mailBcc, IEnumerable<Recipient> recievers, bool linkToEntity)
		{
			PXGraph proc = new PXGraph();
            EntityHelper entityHelper = new EntityHelper(proc);
			foreach (Recipient item in recievers)
			{
				var entity = item.Entity;
                PXCache cache = proc.Caches[entity.GetType()];
				cache.SetStatus(entity, PXEntryStatus.Notchanged);
			    var id = entityHelper.GetEntityNoteID(entity, true);

				var sender = TemplateNotificationGenerator.Create(entity);
				sender.MailAccountId = accountId ?? MailAccountManager.DefaultMailAccountID;
                sender.To = mailTo;
                sender.Cc = mailCc;
                sender.Bcc = mailBcc;
				sender.Body = MassMails.Current.MailContent ?? string.Empty;
				sender.Subject = MassMails.Current.MailSubject;
				sender.BodyFormat = item.Format;
				sender.AttachmentsID = MassMails.Current.NoteID;
				sender.LinkToEntity = linkToEntity;

				var messages = sender.Send();

				foreach (CRSMEmail message in messages)
				{
					var log = (CRMassMailMessage)SendedMessages.Cache.CreateInstance();
					log.MassMailID = MassMails.Current.MassMailID;
					log.MessageID = message.ImcUID;
					SendedMessages.Insert(log);
				}
			}
		}

		private void UpdateCampaignMembers(IEnumerable<Recipient> recipients)
		{
			var cache = Caches[typeof(CRCampaignMembers)];
			this.EnsureCachePersistence(typeof(CRCampaignMembers));
			CampaignRecipient rec;
			foreach (Recipient item in recipients)
				if ((rec = item as CampaignRecipient) != null)
				{
					rec.Member.Status = rec.DistinationStatus;
					cache.Update(rec.Member);
				}
		}

		private List<Recipient> GetMailsForSending(bool onlyFirst)
		{
			var mailKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			var mails = new List<Recipient>();

			switch (MassMails.Current.Source)
			{
				case CRMassMailSourcesAttribute.MailList:
					foreach (CRMarketingList list in MailLists.Select())
						if (list.Selected == true)
						{
							IEnumerable rows;
							if (list.IsDynamic == true)
							{
								rows = DynamicSourceList.Select(list.MarketingListID);
							}
							else
							{
								rows = PXSelectJoin<Contact,
									InnerJoin<CRMarketingListMember,
										On<CRMarketingListMember.contactID, Equal<Contact.contactID>>>,
									Where2<
										Where<Contact.noMassMail, IsNull, 
											Or<Contact.noMassMail, NotEqual<True>>>,
										And2<
											Where<Contact.noEMail, IsNull, 
												Or<Contact.noEMail, NotEqual<True>>>,
										And<CRMarketingListMember.marketingListID, Equal<Required<CRMarketingListMember.marketingListID>>,
										And<CRMarketingListMember.activated, Equal<True>>>>>>
									.Select(this, list.MarketingListID);
							}
							foreach (PXResult row in rows)
							{
								var contact = row[0] as Contact;
								if (contact.NoMassMail == true && list.NoMassMail == true ||
									contact.NoEMail == true && list.NoEMail == true || 
									contact.NoMail == true && list.NoMail == true || 
									contact.NoCall == true && list.NoCall == true || 
									contact.NoFax == true && list.NoFax == true || 
									contact.NoMarketing == true && list.NoMarketing == true)
								{
									continue;
								}

								var subscription = row[1] as CRMarketingListMember;

								var address = contact.EMail;
								if (address != null && (address = address.Trim()) != string.Empty && !mailKeys.Contains(address))
								{
									mailKeys.Add(address);
									mails.Add(new Recipient(contact, subscription.Return(s => s.Format, NotificationFormat.Html)));
									if (onlyFirst) break;
								}
							}
							if (mails.Count > 0 && onlyFirst) break;
						}
					break;
				case CRMassMailSourcesAttribute.Campaign:
					foreach (CRCampaign list in Campaigns.Select())
						if (list.Selected == true)
						{
							foreach (PXResult<Contact, CRCampaignMembers> item in
								PXSelectJoin<Contact,
								InnerJoin<CRCampaignMembers, On<CRCampaignMembers.contactID, Equal<Contact.contactID>>>,
                                Where2<
                                        Where<Contact.noMassMail, IsNull,
                                            Or<Contact.noMassMail, NotEqual<True>>>,
                                        And2<
                                            Where<Contact.noEMail, IsNull,
                                                Or<Contact.noEMail, NotEqual<True>>>,
                                    And<CRCampaignMembers.campaignID, Equal<Required<CRCampaignMembers.campaignID>>,
									And<CRCampaignMembers.status, Equal<Required<CRCampaignMembers.status>>>>>>>
								.Select(this, list.CampaignID, list.TargetStatus))
							{
								var contact = (Contact)item;
								var member = (CRCampaignMembers)item;

								var address = contact.EMail;
								if (address != null && (address = address.Trim()) != string.Empty && !mailKeys.Contains(address))
								{
									mailKeys.Add(address);
									mails.Add(new CampaignRecipient(contact, NotificationFormat.Html, member, list.DestinationStatus));
									if (onlyFirst) break;
								}
							}
							if (mails.Count > 0 && onlyFirst) break;
						}
					break;
				case CRMassMailSourcesAttribute.Lead:
					foreach (Contact contact in Leads.Select())
						if (contact.Selected == true)
						{
							var address = contact.EMail;
							if (address != null && (address = address.Trim()) != string.Empty && !mailKeys.Contains(address))
							{
								mailKeys.Add(address);
								mails.Add(new Recipient(contact, NotificationFormat.Html));
								if (onlyFirst) break;
							}
						}
					break;
			}
			return mails;
		}

		private void saveLeads()
		{
			if (MassMails.Current != null && MassMails.Current.MassMailID != null)
			{
				var massMailID = (int)MassMails.Current.MassMailID;
				selectedLeads.View.Clear();
				if (MassMails.Current.Source == CRMassMailSourcesAttribute.Lead)
					foreach (Contact batch in Leads.Cache.Updated)
					{
						if (batch == null || batch.ContactID == null) continue;

						var item = (CRMassMailMember)PXSelect<CRMassMailMember>.
							Search<CRMassMailMember.massMailID, CRMassMailMember.contactID>(this, massMailID, batch.ContactID);

						if (batch.Selected != true && item != null)
							selectedLeads.Delete(item);

						if (batch.Selected == true && item == null)
						{
							item = new CRMassMailMember();
							item.MassMailID = massMailID;
							item.ContactID = batch.ContactID;
							selectedLeads.Insert(item);
						}
					}
				else
					foreach (CRMassMailMember item in selectedLeads.Select(massMailID))
						selectedLeads.Delete(item);
			}
		}

		private void saveCampaigns()
		{
			if (MassMails.Current != null && MassMails.Current.MassMailID != null)
			{
				var massMailID = (int)MassMails.Current.MassMailID;
				selectedCampaigns.View.Clear();
				if (MassMails.Current.Source == CRMassMailSourcesAttribute.Campaign)
					foreach (CRCampaign batch in Campaigns.Cache.Updated)
					{
						if (batch == null || batch.CampaignID == null) continue;

						var item = (CRMassMailCampaign)PXSelect<CRMassMailCampaign>.
							Search<CRMassMailCampaign.massMailID, CRMassMailCampaign.campaignID>(this, massMailID, batch.CampaignID);

						if (batch.Selected != true && item != null)
							selectedCampaigns.Delete(item);

						if (batch.Selected == true)
						{
							if (item == null)
							{
								item = new CRMassMailCampaign();
								item.MassMailID = massMailID;
								item.CampaignID = batch.CampaignID;
								item.TargetStatus = batch.TargetStatus;
								item.DestinationStatus = batch.DestinationStatus;
								selectedCampaigns.Insert(item);
							}
							else
							{
								item.TargetStatus = batch.TargetStatus;
								item.DestinationStatus = batch.DestinationStatus;
								selectedCampaigns.Update(item);
							}
						}
					}
				else
					foreach (CRMassMailCampaign item in selectedCampaigns.Select(massMailID))
						selectedCampaigns.Delete(item);
			}
		}

		private void saveMailLists()
		{
			if (MassMails.Current != null && MassMails.Current.MassMailID != null)
			{
				var massMailID = (int)MassMails.Current.MassMailID;
				selectedMailList.View.Clear();
				if (MassMails.Current.Source == CRMassMailSourcesAttribute.MailList)
					foreach (CRMarketingList batch in MailLists.Cache.Updated)
					{
						if (batch == null || !batch.MarketingListID.HasValue) continue;

						var item = (CRMassMailMarketingList)PXSelect<CRMassMailMarketingList>.
							Search<CRMassMailMarketingList.massMailID, CRMassMailMarketingList.mailListID>(this, massMailID, batch.MarketingListID);

						if (batch.Selected != true && item != null)
							selectedMailList.Delete(item);

						if (batch.Selected == true && item == null)
						{
							item = new CRMassMailMarketingList();
							item.MassMailID = massMailID;
							item.MailListID = batch.MarketingListID;
							selectedMailList.Insert(item);
						}
					}
				else
					foreach (CRMassMailMarketingList item in selectedMailList.Select(massMailID))
						selectedMailList.Delete(item);

			}
		}

		private void CheckFields(PXCache cache, object row, params Type[] fields)
		{
			var errors = new Dictionary<string, string>(fields.Length);
			foreach (Type field in fields)
			{
				var value = cache.GetValue(row, field.Name);
				if (value == null || (value is string && string.IsNullOrEmpty(value as string)))
				{
					var state = cache.GetValueExt(row, field.Name) as PXFieldState;
					var fieldDisplayName = state == null || string.IsNullOrEmpty(state.DisplayName) 
						? field.Name 
						: state.DisplayName;
					var errorMessage = PXMessages.LocalizeFormatNoPrefix(Messages.EmptyValueErrorFormat, fieldDisplayName);
					var fieldName = cache.GetField(field);
					errors.Add(fieldName, errorMessage);
					PXUIFieldAttribute.SetError(cache, row, fieldName, errorMessage);
				}
			}
			if (errors.Count > 0)
				throw new PXOuterException(errors, GetType(), row, ErrorMessages.RecordRaisedErrors, null, cache.GetItemType().Name);
		}

		#endregion
	}
}
