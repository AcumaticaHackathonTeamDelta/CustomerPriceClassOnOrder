using System;
using System.Collections;
using System.Collections.Generic;
using PX.Common;
using PX.Data;
using PX.Data.EP;
using PX.Objects.CS;
using PX.Objects.EP;

namespace PX.Objects.CR
{
	public class CRMarketingListMaint : PXGraph<CRMarketingListMaint, CRMarketingList>
	{
		#region MailSubscriptionInfo

		[Serializable]
		[PXHidden]
		public partial class MailSubscriptionInfo : IBqlTable
		{
			#region Email

			public abstract class email : IBqlField { }

			[PXString]
			[PXDefault]
			[PXUIField(DisplayName = "Email")]
			public virtual String Email { get; set; }

			#endregion

			#region FirstName

			public abstract class firstName : IBqlField { }

			[PXString]
			[PXDefault]
			[PXUIField(DisplayName = "First Name")]
			public virtual String FirstName { get; set; }

			#endregion

			#region LastName

			public abstract class lastName : IBqlField { }

			[PXString]
			[PXDefault]
			[PXUIField(DisplayName = "Last Name")]
			public virtual String LastName { get; set; }

			#endregion

			#region CompanyName

			public abstract class companyName : IBqlField { }

			[PXString]
			[PXUIField(DisplayName = "Company Name")]
			public virtual String CompanyName { get; set; }

			#endregion

			#region ActivationID

			public abstract class activationID : IBqlField { }

			[PXString]
			[PXUIField(DisplayName = "Activation ID")]
			public virtual String ActivationID { get; set; }

			#endregion

			#region Salutation

			public abstract class salutation : IBqlField { }

			[PXDBString(255, IsUnicode = true)]
			[PXUIField(DisplayName = Messages.Position, Visibility = PXUIVisibility.SelectorVisible)]
			public virtual String Salutation { get; set; }
			#endregion

			#region Source
			public abstract class source : IBqlField { }

			[PXDBString(1, IsFixed = true)]
			[PXUIField(DisplayName = "Lead Source", Visibility = PXUIVisibility.SelectorVisible)]
			[CRMSources]
			[PXDefault(CRMSourcesAttribute._WEB)]
			public virtual String Source { get; set; }
			#endregion

			#region Status
			public abstract class status : IBqlField { }

			[PXDBString(1)]
			[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible)]
			[LeadStatuses]
			[PXDefault(LeadStatusesAttribute.New)]
			public virtual String Status { get; set; }
			#endregion

			#region Phone
			public abstract class phone : IBqlField { }

			[PXDBString(50)]
			[PXUIField(DisplayName = "Phone", Visibility = PXUIVisibility.SelectorVisible)]
			[PhoneValidation]
			public virtual String Phone { get; set; }
			#endregion

			#region ClassID
			public abstract class classID : IBqlField { }

			[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
			[PXUIField(DisplayName = "Class ID")]
			[PXSelector(typeof(CRContactClass.classID), DescriptionField = typeof(CRContactClass.description))]
			public virtual String ClassID { get; set; }
			#endregion

			#region NoCall
			public abstract class noCall : IBqlField { }

			[PXDBBool]
			[PXDefault(false)]
			[PXUIField(DisplayName = "Do Not Call")]
			public virtual bool? NoCall { get; set; }
			#endregion
		}

		#endregion

		#region MailUnsubscriptionInfo

		[Serializable]
		[PXHidden]
		public partial class MailUnsubscriptionInfo : IBqlTable
		{
			#region Email

			public abstract class email : IBqlField { }

			[PXString]
			[PXDefault]
			[PXUIField(DisplayName = "Email")]
			public virtual String Email { get; set; }

			#endregion
		}

		#endregion

		#region MailListInfo

		[Serializable]
        [PXHidden]
		[PXProjection(typeof(Select<CRMarketingList, Where<CRMarketingList.isActive, Equal<True>>>), Persistent = true)]
		public partial class MarketingListInfo : CRMarketingList
		{
		}

		#endregion

		#region Constants

		public const string REMOVE_ACTION = "Remove Members";
		public const string ADD_ACTION = "Add Members";

		#endregion

		#region Selects

		[PXViewName(Messages.MarketingList)]
		public PXSelect<CRMarketingList>
			MailLists;

		[PXHidden]
		public PXSelect<Contact>
			Leads;

        [PXHidden]
        public PXSelect<BAccount> BAccount;

        [PXHidden]
        public PXSelect<BAccountR> BAccountR;

		[PXHidden]
		[PXCopyPasteHiddenFields(typeof(CRMarketingList.description))]
		public PXSelect<CRMarketingList, Where<CRMarketingList.marketingListID, Equal<Current<CRMarketingList.marketingListID>>>>
			MailListsCurrent;

		[PXImport(typeof(CRMarketingList))]
		[PXFilterable]
		public PXSelectJoin<CRMarketingListMember,
			InnerJoin<ContactBAccount, On<ContactBAccount.contactID, Equal<CRMarketingListMember.contactID>>,            
            LeftJoin<BAccountR, On<
                Where2<Where<ContactBAccount.contactType, NotEqual<ContactTypesAttribute.employee>, And<BAccountR.bAccountID, Equal<ContactBAccount.bAccountID>>>,
                    Or<Where<ContactBAccount.contactType, Equal<ContactTypesAttribute.employee>, 
                        And<ContactBAccount.contactID, Equal<BAccountR.defContactID>, 
                        And<ContactBAccount.bAccountID, Equal<BAccountR.parentBAccountID>>>>>>>>>,
			Where<CRMarketingListMember.marketingListID, Equal<Optional<CRMarketingList.marketingListID>>>>
			MailRecipients;

		[PXViewName(Messages.Filter)]
		public PXFilter<OperationParam>
			Operations;

		public PXAction<CRMarketingList> process;
		[PXProcessButton]
		[PXUIField(DisplayName = "Process")]
		protected virtual IEnumerable Process(PXAdapter adapter)
		{
			var list = new List<Contact>();
			foreach (Contact item in FilteredItems.Cache.Cached)
			{
				bool? sel = (bool?)FilteredItems.Cache.GetValue<Contact.selected>(item);
				if (sel == true)
				{
					PXEntryStatus status = FilteredItems.Cache.GetStatus(item);
					if (status == PXEntryStatus.Inserted
						|| status == PXEntryStatus.Updated)
					{
						list.Add(item);
					}
				}
			}
			if (list.Count > 0)
			{
				PerformAction(list);
			}
			return adapter.Get();
		}

		bool _ProcessPending;
		public PXAction<CRMarketingList> processAll;
		[PXProcessButton]
		[PXUIField(DisplayName = "Process All")]
		protected virtual IEnumerable ProcessAll(PXAdapter adapter)
		{
			_ProcessPending = true;
			return adapter.Get();
		}

		[PXViewName(Messages.SelectionPreview)]
		[CRFixedFilterable(typeof(CRMarketingList.noteID))]
		public PXSelectJoin<Contact,
			LeftJoin<CRMarketingListMember,
				On<CRMarketingListMember.contactID, Equal<Contact.contactID>,
					And<CRMarketingListMember.marketingListID, Equal<Current<CRMarketingList.marketingListID>>>>,
			LeftJoin<BAccount,
				On<BAccount.bAccountID, Equal<Contact.bAccountID>>,
			LeftJoin<BAccountParent,
				On<BAccountParent.bAccountID, Equal<Contact.parentBAccountID>>,
			LeftJoin<Address,
				On<Address.addressID, Equal<Contact.defAddressID>>,
			LeftJoin<State,
				On<State.countryID, Equal<Address.countryID>,
					And<State.stateID, Equal<Address.state>>>>>>>>,
			Where<CRMarketingListMember.contactID, IsNull>>
			FilteredItems;

		[PXHidden]
		public PXSelectJoin<Contact,
			InnerJoin<CRMarketingListMember, On<CRMarketingListMember.contactID, Equal<Contact.contactID>>,
			LeftJoin<BAccount, On<BAccount.bAccountID, Equal<Contact.bAccountID>>,
			LeftJoin<BAccountParent, On<BAccountParent.bAccountID, Equal<Contact.parentBAccountID>>,
			LeftJoin<Address, On<Address.addressID, Equal<Contact.defAddressID>>,
			LeftJoin<State,
				On<State.countryID, Equal<Address.countryID>,
					And<State.stateID, Equal<Address.state>>>>>>>>,
			Where<CRMarketingListMember.marketingListID, Equal<Current<CRMarketingList.marketingListID>>>>
			RemoveItems;

		[PXHidden]
		public PXSelect<CRMarketingListMember>
			NewMailSubscriptions;

		[PXHidden]
		public PXFilter<MailSubscriptionInfo>
			Subscriptions;

		[PXHidden]
		public PXFilter<MailUnsubscriptionInfo>
			Unsubscriptions;

		[PXHidden]
		public PXSelect<MarketingListInfo>
			MailListsSubscription;

		#endregion

		#region Ctors

		public CRMarketingListMaint()
		{
			var contactCache = Caches[typeof(Contact)];
			PXUIFieldAttribute.SetDisplayName<Contact.fullName>(contactCache, PXLocalizer.Localize(Messages.ContactFullName, typeof(Messages).ToString()));
			PXDBAttributeAttribute.Activate(FilteredItems.Cache);
			
			var parentBAccountCache = Caches[typeof(BAccount)];
			parentBAccountCache.DisplayName = Messages.ParentAccount;
			PXUIFieldAttribute.SetDisplayName<BAccount.acctCD>(parentBAccountCache, PXLocalizer.Localize(Messages.BAccountAcctCD, typeof(Messages).ToString()));
			PXUIFieldAttribute.SetDisplayName<BAccount.acctName>(parentBAccountCache, Messages.ParentAccountNameShort);
			PXDBAttributeAttribute.Activate(parentBAccountCache);

			PXUIFieldAttribute.SetVisible<CRMarketingListMember.marketingListID>(MailRecipients.Cache, null, false);
		}

		#endregion

		#region Actions

		public PXAction<Contact> details;
		[PXButton(Tooltip = Messages.ViewDetailsTooltip)]
		[PXUIField(DisplayName = Messages.ViewDetails, Visible = false)]
		public virtual IEnumerable Details(PXAdapter adapter)
		{
			if (FilteredItems.Current != null)
			{
				var recipient = (Contact)PXSelect<Contact>.
					Search<Contact.contactID>(this, FilteredItems.Current.ContactID);
				if (recipient != null)
					PXRedirectHelper.TryOpenPopup(Caches[typeof(Contact)], recipient, string.Empty);
			}
			return adapter.Get();
		}

		public PXAction<Contact> generateActivation;

		[PXUIField(Visible = false)]
		[PXButton]
		public virtual IEnumerable GenerateActivation(PXAdapter adapter)
		{
			var recipient = Leads.Current;
			var minutes = 10;
			if (adapter.Parameters != null && adapter.Parameters.Length != 0)
				int.TryParse(adapter.Parameters[0].ToString(), out minutes);

			if (recipient != null)
			{
				var wasUpdated = false;
				var cache = Caches[typeof(CRMarketingListMember)];
				foreach (MarketingListInfo listInfo in MailListsSubscription.Select())
					if (listInfo.MarketingListID != null && listInfo.Selected == true)
						foreach (CRMarketingListMember item in PXSelect<CRMarketingListMember,
							Where<CRMarketingListMember.contactID, Equal<Required<CRMarketingListMember.contactID>>,
								And<CRMarketingListMember.marketingListID, Equal<Required<CRMarketingListMember.marketingListID>>>>>.
							Select(this, recipient.ContactID, listInfo.MarketingListID))
						{
							item.Activated = false;
							cache.Update(item);
							wasUpdated = true;
						}
				if (wasUpdated) Save.Press();

				//recipient.ActivationID = System.Web.HttpUtility.UrlEncode(recipient.ContactID.ToString());//TODO: need remove
			}
			return adapter.Get();
		}

		public PXAction<CRMarketingList> removeRecipient;

		[PXUIField(Visible = false)]
		[PXButton]
		public virtual IEnumerable RemoveRecipient(PXAdapter adapter)
		{
			foreach (MailUnsubscriptionInfo item in Unsubscriptions.Select())
			{
				foreach (MarketingListInfo list in MailListsSubscription.Select())
					foreach (PXResult<CRMarketingListMember> row in
						PXSelectJoin<CRMarketingListMember,
							InnerJoin<Contact, On<Contact.contactID, Equal<CRMarketingListMember.contactID>>>,
							Where<CRMarketingListMember.marketingListID, Equal<Required<CRMarketingList.marketingListID>>,
								And<Contact.eMail, Equal<Required<Contact.eMail>>>>>.
							Select(this, list.MarketingListID, item.Email))
					{
						var sub = (CRMarketingListMember)row;
						sub.Activated = false;
						MailRecipients.Update(sub);
					}
			}
			Save.Press();
			SelectTimeStamp();
			return adapter.Get();
		}

		#endregion

		#region Data Handlers        
        
        [PXDimensionSelector("BIZACCT",
            typeof(Search2<BAccountR.acctCD,
                    LeftJoin<Contact, On<Contact.bAccountID, Equal<BAccountR.bAccountID>, And<Contact.contactID, Equal<BAccountR.defContactID>>>,
                    LeftJoin<Address, On<Address.bAccountID, Equal<BAccountR.bAccountID>, And<Address.addressID, Equal<BAccountR.defAddressID>>>>>,
                Where2<Where<BAccountR.type, Equal<BAccountType.customerType>,
                    Or<BAccountR.type, Equal<BAccountType.prospectType>,
                    Or<BAccountR.type, Equal<BAccountType.combinedType>,
                    Or<BAccountR.type, Equal<BAccountType.vendorType>>>>>,
                    And<Match<Current<AccessInfo.userName>>>>>),
            typeof(BAccountR.acctCD),
            typeof(BAccountR.acctCD), typeof(BAccountR.acctName), typeof(BAccountR.type), typeof(BAccountR.classID), typeof(BAccountR.status), typeof(Contact.phone1), typeof(Address.city), typeof(Address.countryID), typeof(Contact.eMail))]
        [PXDBString(30, IsUnicode = true, IsKey = true, InputMask = "")]
        [PXUIField(DisplayName = "Business Account", Enabled = false)]
        public virtual void BAccountR_AcctCD_CacheAttached(PXCache sender)
	    {
	        
	    }
		public override void Persist()
		{
			PersistSubscription();
			PersistUnsubscription();

			base.Persist();
		}

		private void PersistSubscription()
		{
			var current = Subscriptions.Current;
			if (string.IsNullOrEmpty(current.Email)) return;

			var lead = (Contact)Leads.Search<Contact.eMail>(current.Email);
			if (lead == null)
			{
				lead = (Contact)Leads.Cache.Insert();
				lead.ContactType = ContactTypesAttribute.Lead;
				lead.FirstName = current.FirstName;
				lead.LastName = current.LastName;
				lead.EMail = current.Email;
				lead.FullName = current.CompanyName;
				lead.Salutation = current.Salutation;
				lead.Source = current.Source;
				lead.Status = current.Status;
				lead.Phone1 = current.Phone;
				lead.ClassID = current.ClassID;
				lead.NoCall = current.NoCall;
				Leads.Cache.Update(lead);
			}

			foreach (MarketingListInfo list in MailListsSubscription.Select())
			{
				if (list.Selected != true) continue;

				if (Leads.Cache.GetStatus(lead) != PXEntryStatus.Inserted)
				{
					var existSubscriptions = NewMailSubscriptions.
						Search<CRMarketingListMember.contactID, CRMarketingListMember.marketingListID>(lead.ContactID, list.MarketingListID);
					if (existSubscriptions != null && existSubscriptions.Count > 0) continue;
				}

				var subscr = (CRMarketingListMember)NewMailSubscriptions.Cache.CreateInstance();
				subscr.ContactID = lead.ContactID;
				subscr.MarketingListID = list.MarketingListID;
				subscr.Activated = false;
				NewMailSubscriptions.Cache.Update(subscr);
			}
		}

		private void PersistUnsubscription()
		{
			Contact lead;
			if (string.IsNullOrEmpty(Unsubscriptions.Current.Email) ||
				(lead = (Contact)Leads.Search<Contact.eMail>(Unsubscriptions.Current.Email)) == null ||
				Leads.Cache.GetStatus(lead) == PXEntryStatus.Inserted)
			{
				return;
			}

			foreach (MarketingListInfo list in MailListsSubscription.Select())
			{
				if (list.Selected == true) continue;

				var existSubscriptions = NewMailSubscriptions.
					Search<CRMarketingListMember.contactID, CRMarketingListMember.marketingListID>(lead.ContactID, list.MarketingListID);
				if (existSubscriptions == null || existSubscriptions.Count == 0) continue;

				var subscr = (CRMarketingListMember)existSubscriptions;
				subscr.Activated = false;
				NewMailSubscriptions.Cache.Update(subscr);
			}
		}

		protected virtual IEnumerable filteredItems([PXString] string action)
		{
			if (!String.Equals(Operations.Current.Action, action, StringComparison.OrdinalIgnoreCase))
				FilteredItems.Cache.Clear();
			Operations.Current.Action = action;

			var mailListID = MailLists.Current.With(ml => ml.MarketingListID);

			//Dinamic)
			if (MailLists.Current.With(ml => ml.IsDynamic == true))
				return CRSubscriptionsSelect.Select(this, mailListID);

			CRSubscriptionsSelect.MergeFilters(this, mailListID);			

			if (_ProcessPending)
			{
				_ProcessPending = false;
				Contact saved = FilteredItems.Current;
				var list = new List<Contact>();
				int start = 0;
				int total = 0;
				foreach (PXResult<Contact> item in FilteredItems.View.Select(null, new object[] { action }, null, null, null, PXView.Filters, ref start, 0, ref total))
				{
					((Contact)item).Selected = true;
					FilteredItems.Update(item);
					list.Add(((Contact)item));
				}
				FilteredItems.Current = saved;
				FilteredItems.Cache.IsDirty = false;
				PerformAction(list);
                PXRedirectHelper.TryRedirect(this, PXRedirectHelper.WindowMode.Same);
			}

			//Remove
			if (string.Equals(Operations.Current.Action, REMOVE_ACTION, StringComparison.OrdinalIgnoreCase))
				return this.QuickSelect(RemoveItems.View.BqlSelect, PXView.Filters);

			//Add
			var command = FilteredItems.View.BqlSelect;
			if (MailLists.Current.With(ml => ml.NoCall == true))
				command = command.WhereAnd(
					typeof(Where<Contact.noCall, IsNull,
								Or<Contact.noCall, NotEqual<True>>>));
			if (MailLists.Current.With(ml => ml.NoEMail == true))
				command = command.WhereAnd(
					typeof(Where<Contact.noEMail, IsNull,
								Or<Contact.noEMail, NotEqual<True>>>));
			if (MailLists.Current.With(ml => ml.NoFax == true))
				command = command.WhereAnd(
					typeof(Where<Contact.noFax, IsNull,
								Or<Contact.noFax, NotEqual<True>>>));
			if (MailLists.Current.With(ml => ml.NoMail == true))
				command = command.WhereAnd(
					typeof(Where<Contact.noMail, IsNull,
								Or<Contact.noMail, NotEqual<True>>>));
			if (MailLists.Current.With(ml => ml.NoMarketing == true))
				command = command.WhereAnd(
					typeof(Where<Contact.noMarketing, IsNull,
								Or<Contact.noMarketing, NotEqual<True>>>));
			if (MailLists.Current.With(ml => ml.NoMassMail == true))
				command = command.WhereAnd(
					typeof(Where<Contact.noMassMail, IsNull,
								Or<Contact.noMassMail, NotEqual<True>>>));

			return this.QuickSelect(command, PXView.Filters);
		}

		protected virtual void PerformAction(List<Contact> list)
		{
			if (Operations.Current.Action == REMOVE_ACTION)
			{
				if (MailLists.Current != null
					&& Operations.Ask(
							Messages.AskConfirmation,
							string.Format(PXLocalizer.Localize(Messages.ConfirmRemoving, typeof(Messages).ToString()), list.Count),
							MessageButtons.YesNoCancel) == WebDialogResult.Yes)
				{
					try
					{
						foreach (Contact item in list)
						{
							if (item.Selected == true)
							{
								var newItem = (CRMarketingListMember)MailRecipients.Cache.CreateInstance();
								newItem.MarketingListID = MailLists.Current.MarketingListID;
								newItem.ContactID = item.ContactID;
								MailRecipients.Cache.Delete(newItem);
							}
						}
						Save.Press();
						MailRecipients.Cache.Clear();
					}
					finally
					{
						MailRecipients.Cache.Clear();
					}
				}
			}
			else
			{
				if (MailLists.Current != null)
				{
					try
					{
						foreach (Contact item in list)
						{
							if (item.Selected == true)
							{
								var newItem = (CRMarketingListMember)MailRecipients.Cache.CreateInstance();
								newItem.MarketingListID = MailLists.Current.MarketingListID;
								newItem.ContactID = item.ContactID;
								MailRecipients.Cache.Insert(newItem);
							}
						}
						Save.Press();
						FilteredItems.Cache.Clear();
					}
					finally
					{
						MailRecipients.Cache.Clear();
					}
				}
			}
            this.MailRecipients.View.RequestRefresh();
		}

		#endregion

		#region Event Handlers

		[PXDBString(1, IsFixed = true)]
		[CRContactMethods]
		[PXUIField(DisplayName = "Contact Method")]
		protected virtual void Contact_Method_CacheAttached(PXCache sender)
		{
			
		}

		[PXDBBool]
		[PXDefault(true, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Active")]
		protected virtual void Contact_IsActive_CacheAttached(PXCache sender)
		{
			
		}

		[PXDBInt]
		[AddressRevisionID]
		protected virtual void Contact_RevisionID_CacheAttached(PXCache sender)
		{
			
		}

		[PXDBString(2, IsFixed = true)]
		[PXUIField(DisplayName = "Type")]
		[ContactTypes]
		[PXDefault(ContactTypesAttribute.Lead, PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void Contact_ContactType_CacheAttached(PXCache sender)
		{
			
		}

		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Source")]
		[CRMSources]
		[PXDefault(CRMSourcesAttribute._PURCHASED_LIST, PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void Contact_Source_CacheAttached(PXCache sender)
		{
			
		}

		[PXDBString(1)]
		[PXUIField(DisplayName = "Status")]
		[LeadStatuses]
		[PXDefault(LeadStatusesAttribute.New, PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void Contact_Status_CacheAttached(PXCache sender)
		{
			
		}

        [PXDBString(2, IsFixed = true)]
		[PXUIField(DisplayName = "Business Account Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[BAccountType.List()]
	    protected virtual void BAccount_Type_CacheAttached(PXCache sender)
	    {
	        
	    }

		protected virtual void Contact_RowPersisted(PXCache cache, PXRowPersistedEventArgs e)
		{
			var row = e.Row as Contact;
			if (row == null || e.TranStatus != PXTranStatus.Open) return;

			foreach (CRMarketingListMember item in NewMailSubscriptions.Cache.Inserted)
				if (item.ContactID < 0) item.ContactID = row.ContactID;
		}

		protected virtual void CRMarketingList_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			var row = e.Row as CRMarketingList;
			if (row == null) return;

			var isStatic = row.IsDynamic != true;
			PXUIFieldAttribute.SetEnabled<CRMarketingList.isSelfManaged>(sender, row, isStatic);
			process.SetEnabled(isStatic);
			processAll.SetEnabled(isStatic);
			processAll.ClearAnswerAfterPress = false;

			PXUIFieldAttribute.SetVisible<Contact.selected>(FilteredItems.Cache, null, isStatic);
			FilteredItems.Cache.AllowUpdate = isStatic;
		}

		protected virtual void CRMarketingList_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			var row = e.Row as CRMarketingList;
			var oldRow = e.OldRow as CRMarketingList;
			if (row == null || oldRow == null) return;

			if (row.IsDynamic == true && row.IsDynamic != oldRow.IsDynamic &&
				MailRecipients.Select(row.MarketingListID).Count > 0)
			{
				row.IsDynamic = false;
				sender.RaiseExceptionHandling<CRMarketingList.isDynamic>(e.Row, row.IsDynamic, 
					new PXSetPropertyException(Messages.ThereAreManualSubscribers, PXErrorLevel.Warning));
			}
		}

		#endregion

		#region Private Methods

		public static void DoActivate(string activationID)
		{
			int contactId;
			if (!int.TryParse(System.Web.HttpUtility.UrlDecode(activationID), out contactId))
			{
				return;
			}

			var graph = new PXGraph();
			var cache = graph.Caches[typeof(CRMarketingListMember)];
			graph.Views.Caches.Add(typeof(CRMarketingListMember));
			var wasUpdated = false;
				foreach (CRMarketingListMember item in PXSelect<CRMarketingListMember,
					Where<CRMarketingListMember.contactID, Equal<Required<CRMarketingListMember.contactID>>>>.
					Select(graph, contactId))
				{
					item.Activated = true;
					cache.Update(item);
					wasUpdated = true;
				}
			if (wasUpdated) graph.Actions.PressSave();
		}

		#endregion
	}
}
