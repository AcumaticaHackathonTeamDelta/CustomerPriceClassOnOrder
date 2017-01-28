using System;
using PX.Data;
using System.Collections.Generic;
using System.Collections;

namespace PX.Objects.CR
{
	#region CampaignMaint
    public class CampaignMaint : PXGraph<CampaignMaint, CRCampaign>, PXImportAttribute.IPXPrepareItems
	{
		[PXHidden]
		public PXSetup<CRSetup>
			crSetup;

		[PXHidden]
		public PXSelect<Contact>
			BaseContacts;

		[PXViewName(Messages.Campaign)]
		public PXSelect<CRCampaign>
			Campaign;

		[PXHidden]
		public PXSelect<CRCampaign,
			Where<CRCampaign.campaignID, Equal<Current<CRCampaign.campaignID>>>>
			CampaignCurrent;

		[PXViewName(Messages.CampaignMembers)]
        [PXImport(typeof(CRCampaign))]
		public PXSelectJoin<SelCampaignMembers,
				LeftJoin<Contact, On<SelCampaignMembers.contactID, Equal<Contact.contactID>,
					And<SelCampaignMembers.campaignID, Equal<Current<CRCampaign.campaignID>>>>,
				LeftJoin<Address, On<Address.addressID, Equal<Contact.defAddressID>>>>,
				Where2<
					Where<Contact.contactType, Equal<ContactTypesAttribute.lead>,
						Or<Contact.contactType, Equal<ContactTypesAttribute.person>>>,
					And<SelCampaignMembers.campaignID, Equal<Current<CRCampaign.campaignID>>>>>
				CampaignMembers;

		public CampaignMaint()
		{
			PXUIFieldAttribute.SetEnabled<CRCampaign.leadsGenerated>(CampaignCurrent.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<CRCampaign.leadsConverted>(CampaignCurrent.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<CRCampaign.contacts>(CampaignCurrent.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<CRCampaign.responses>(CampaignCurrent.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<CRCampaign.opportunities>(CampaignCurrent.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<CRCampaign.closedOpportunities>(CampaignCurrent.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<CRCampaign.opportunitiesValue>(CampaignCurrent.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<CRCampaign.closedOpportunitiesValue>(CampaignCurrent.Cache, null, false);

			PXUIFieldAttribute.SetRequired<CRCampaign.startDate>(CampaignCurrent.Cache, true);
			PXUIFieldAttribute.SetRequired<CRCampaign.status>(CampaignCurrent.Cache, true);

			PXUIFieldAttribute.SetEnabled<SelCampaignMembers.bAccountID>(CampaignMembers.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<SelCampaignMembers.salutation>(CampaignMembers.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<SelCampaignMembers.eMail>(CampaignMembers.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<SelCampaignMembers.phone1>(CampaignMembers.Cache, null, false);

			//PXUIFieldAttribute.SetVisible<SelCampaignMembers.title>(CampaignMembers.Cache, null, false);

			var cache = Caches[typeof(Contact)];
			PXDBAttributeAttribute.Activate(cache);			
			PXUIFieldAttribute.SetVisible<Contact.title>(cache, null, false);
			PXUIFieldAttribute.SetVisible<Contact.workgroupID>(cache, null, false);
			PXUIFieldAttribute.SetVisible<Contact.ownerID>(cache, null, false);
			PXUIFieldAttribute.SetVisible<Contact.firstName>(cache, null, false);
			PXUIFieldAttribute.SetVisible<Contact.midName>(cache, null, false);
			PXUIFieldAttribute.SetVisible<Contact.lastName>(cache, null, false);
			PXUIFieldAttribute.SetVisible<Contact.phone2>(cache, null, false);
			PXUIFieldAttribute.SetVisible<Contact.phone3>(cache, null, false);
			PXUIFieldAttribute.SetVisible<Contact.fax>(cache, null, false);
			PXUIFieldAttribute.SetVisible<Contact.webSite>(cache, null, false);
			PXUIFieldAttribute.SetVisible<Contact.isActive>(cache, null, false);
			PXUIFieldAttribute.SetVisible<Contact.dateOfBirth>(cache, null, false);
			PXUIFieldAttribute.SetVisible<Contact.createdByID>(cache, null, false);
			PXUIFieldAttribute.SetVisible<Contact.createdDateTime>(cache, null, false);
			PXUIFieldAttribute.SetVisible<Contact.lastModifiedByID>(cache, null, false);
			PXUIFieldAttribute.SetVisible<Contact.lastModifiedDateTime>(cache, null, false);

			PXUIFieldAttribute.SetVisible<Address.addressLine1>(Caches[typeof(Address)], null, false);
			PXUIFieldAttribute.SetVisible<Address.addressLine2>(Caches[typeof(Address)], null, false);

			PXUIFieldAttribute.SetVisible<Contact.classID>(cache, null, false);
			PXUIFieldAttribute.SetVisible<Contact.source>(cache, null, false);
			PXUIFieldAttribute.SetVisible<Contact.fullName>(cache, null, false);
			PXUIFieldAttribute.SetVisible<Contact.status>(cache, null, false);

			PXUIFieldAttribute.SetDisplayName<BAccount.acctName>(Caches[typeof(BAccount)], Messages.CustomerName);
			PXUIFieldAttribute.SetDisplayName<Contact.bAccountID>(cache, PXLocalizer.Localize(Messages.ContactBAccountID, typeof(Messages).ToString()));
			PXUIFieldAttribute.SetDisplayName<Contact.status>(cache, PXLocalizer.Localize(Messages.ContactStatus, typeof(Messages).ToString()));
		}

		public virtual IEnumerable campaignMembers()
		{
			foreach (PXResult<SelCampaignMembers, Contact, Address> record in PXSelectJoin<SelCampaignMembers,
				LeftJoin<Contact, On<SelCampaignMembers.contactID, Equal<Contact.contactID>,
					And<SelCampaignMembers.campaignID, Equal<Current<CRCampaign.campaignID>>>>,
				LeftJoin<Address, On<Address.addressID, Equal<Contact.defAddressID>>>>,
				Where2<
					Where<Contact.contactType, Equal<ContactTypesAttribute.lead>,
						Or<Contact.contactType, Equal<ContactTypesAttribute.person>>>,
					And<SelCampaignMembers.campaignID, Equal<Current<CRCampaign.campaignID>>>>>.
				Select(this))
			{
				var contact = (Contact)record;
				if (contact.ContactID == null)
				{
					var member = (SelCampaignMembers)record;
					if (CampaignMembers.Cache.GetStatus(member) != PXEntryStatus.Notchanged)
						contact = (Contact)PXSelect<Contact,
												Where<Contact.contactID, Equal<Required<Contact.contactID>>>>.
												Select(this, member.ContactID);
					yield return new PXResult<SelCampaignMembers, Contact, Address>(record, contact, record);
				}
				else yield return record;
			}
		}
		
		protected virtual void SelCampaignMembers_Status_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
		{
			if (Campaign.Current != null)
				e.NewValue = Campaign.Current.DefaultMemberStatus;
		}

		protected virtual void SelCampaignMembers_CampaignID_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
		{
			if (Campaign.Current != null)
				e.NewValue = Campaign.Current.CampaignID;
		}

		protected virtual void SelCampaignMembers_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			SelCampaignMembers member = (SelCampaignMembers)e.Row;
			if (member.ContactID != null)
			{
				var cont = (Contact)PXSelect<Contact>.Search<Contact.contactID>(this, member.ContactID);
				member.FirstName = cont.FirstName;
				member.MidName = cont.MidName;
				member.LastName = cont.LastName;
				member.Title = cont.Title;
			}
		}


		protected virtual void CRCampaign_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			CRCampaign row = e.Row as CRCampaign;
			if (row != null)
			{
				if (CanBeDeleted(row) == false)
				{
					e.Cancel = true;
					throw new PXException(Messages.CampaignIsReferenced);
				}
			}
		}

		private bool CanBeDeleted(CRCampaign campaign)
		{
			if (campaign.ClosedOpportunities.HasValue && campaign.ClosedOpportunities.Value != 0)
				return false;
			if (campaign.Contacts.HasValue && campaign.Contacts.Value != 0)
				return false;
			if (campaign.LeadsConverted.HasValue && campaign.LeadsConverted.Value != 0)
				return false;
			if (campaign.LeadsGenerated.HasValue && campaign.LeadsGenerated.Value != 0)
				return false;
			if (campaign.MailsSent.HasValue && campaign.MailsSent.Value != 0)
				return false;
			if (campaign.Opportunities.HasValue && campaign.Opportunities.Value != 0)
				return false;
			if (campaign.Responses.HasValue && campaign.Responses.Value != 0)
				return false;


			return true;
		}


		#region Sub-screen Navigation Buttons
		public PXAction<CRCampaign> multipleInsert;
		[PXUIField(DisplayName = Messages.AddNewMembers, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.Process)]
		public virtual IEnumerable MultipleInsert(PXAdapter adapter)
		{
			if (Campaign.Current != null && Campaign.Current.CampaignID != null &&
				Campaign.Cache.GetStatus(Campaign.Current) != PXEntryStatus.Inserted)
			{
				CampaignMemberMassProcess target = PXGraph.CreateInstance<CampaignMemberMassProcess>();
				target.Operations.Current.CampaignID = Campaign.Current.CampaignID;
				target.Operations.Current.Action = CampaignMemberMassProcess.CampaignOperationParam.ActionList.Add;

				throw new PXRedirectRequiredException(target, Messages.AddNewCampaignMembers);
			}
			return adapter.Get();
		}
		public PXAction<CRCampaign> multipleDelete;
		[PXUIField(DisplayName = Messages.DeleteSelected, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.RemoveConfig, Tooltip = Messages.MultiDeleteTooltip)]
		public virtual IEnumerable MultipleDelete(PXAdapter adapter)
		{
			if (Campaign.Current != null && Campaign.Current.CampaignID != null && CampaignMembers.Cache.IsDirty &&
				Campaign.Cache.GetStatus(Campaign.Current) != PXEntryStatus.Inserted)
			{
				WebDialogResult res = this.Campaign.Ask(Messages.Confirmation, Messages.CampaignDeleteMembersQuestion, MessageButtons.YesNo);
				if (res == WebDialogResult.Yes)
				{
					List<SelCampaignMembers> membersToDelete = new List<SelCampaignMembers>();
					foreach (SelCampaignMembers member in CampaignMembers.Cache.Updated)
						if (member.Selected ?? false) membersToDelete.Add(member);
					foreach (SelCampaignMembers member in membersToDelete)
						CampaignMembers.Cache.Delete(member);
					Save.Press();
				}
			}
			return adapter.Get();
		}
		#endregion //Sub-screen Navigation Buttons

		protected virtual void SelCampaignMembers_RowInserting(PXCache cache, PXRowInsertingEventArgs e)
		{
			if (Campaign.Current != null && Campaign.Current.CampaignID != null)
			{
				SelCampaignMembers result = PXSelect<SelCampaignMembers, Where<SelCampaignMembers.campaignID, Equal<Required<SelCampaignMembers.campaignID>>, And<SelCampaignMembers.contactID, Equal<Required<SelCampaignMembers.contactID>>>>>.Search<SelCampaignMembers.contactID>(this, ((SelCampaignMembers)e.Row).ContactID, Campaign.Current.CampaignID, ((SelCampaignMembers)e.Row).ContactID);
				if (result != null)
				{
					cache.RaiseExceptionHandling<SelCampaignMembers.contactID>(e.Row, ((SelCampaignMembers)e.Row).ContactID, new PXException(Messages.RecordAlreadyExists));
					e.Cancel = true;
				}
			}
		}

		protected virtual void SelCampaignMembers_RowSelecting(PXCache cache, PXRowSelectingEventArgs e)
		{
			if (e.Row != null)
			{
				PXUIFieldAttribute.SetEnabled<SelCampaignMembers.contactID>(CampaignMembers.Cache, e.Row, false);
			}
		}

		protected virtual void CRCampaign_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			var canAddAndRemove = Campaign.Cache.GetStatus(Campaign.Current) != PXEntryStatus.Inserted;
			multipleInsert.SetEnabled(canAddAndRemove);
			multipleDelete.SetEnabled(canAddAndRemove);
		}

		protected virtual void CRCampaign_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
		{
			CRCampaign row = (CRCampaign)e.Row;
			if (row != null)
			{
				if (row.StartDate.HasValue == false)
				{
					if (cache.RaiseExceptionHandling<CRCampaign.startDate>(e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(CRCampaign.startDate).Name)))
					{
						throw new PXRowPersistingException(typeof(CRCampaign.startDate).Name, null, ErrorMessages.FieldIsEmpty, typeof(CRCampaign.startDate).Name);
					}
				}
			}
		}

        #region Implementation of IPXPrepareItems

        public virtual bool PrepareImportRow(string viewName, IDictionary keys, IDictionary values)
        {
            if (string.Compare(viewName, "CampaignMembers", true) == 0)
            {
                if (values.Contains("ContactID"))
                {
                    
                    Contact contact;
                    int contactID;
                    if (int.TryParse(values["ContactID"].ToString(), out contactID))
                    {
                        contact = PXSelect<Contact, Where<Contact.contactID, Equal<Required<Contact.contactID>>, And<
                                    Where<Contact.contactType, Equal<ContactTypesAttribute.lead>,
                                    Or<Contact.contactType, Equal<ContactTypesAttribute.person>>>>>>.Select(this, contactID);
                    }
                    else
                    {
                        string contactDisplayName = values["ContactID"].ToString();
                        contact = PXSelect <Contact, Where<Contact.displayName, Equal<Required<Contact.displayName>>, And<
                                    Where<Contact.contactType, Equal<ContactTypesAttribute.lead>,
                                    Or<Contact.contactType, Equal<ContactTypesAttribute.person>>>>>>.Select(this, contactDisplayName);
                    }

                    if (contact != null)
                    {
                        if (values.Contains("CampaignID"))
                            values["CampaignID"]= Campaign.Current.CampaignID;
                        else
                            values.Add("CampaignID", Campaign.Current.CampaignID);
                        keys["CampaignID"] = Campaign.Current.CampaignID;
                        
                        if (values.Contains("ContactID"))
                            values["ContactID"] = contact.ContactID;
                        else
                            values.Add("ContactID", contact.ContactID);
                        keys["ContactID"] = contact.ContactID;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public bool RowImporting(string viewName, object row)
        {
            return row == null;
        }

        public bool RowImported(string viewName, object row, object oldRow)
        {
            return oldRow == null;
        }

        public virtual void PrepareItems(string viewName, IEnumerable items)
        {
        }

        #endregion
	}
	#endregion

	#region DAOs

	[PXProjection(typeof(Select2<CRCampaignMembers, 
		RightJoin<Contact, On<Contact.contactID, Equal<CRCampaignMembers.contactID>>>>), 
		Persistent = true)]
	[PXCacheName(Messages.CampaignMember)]
	[PXBreakInheritance]
    [Serializable]
	public partial class SelCampaignMembers : CRCampaignMembers, IPXSelectable
	{
		#region ContactID
		public new abstract class contactID : PX.Data.IBqlField
		{
		}
		#endregion
		#region CampaignID
		public new abstract class campaignID : PX.Data.IBqlField
		{
		}
		[PXDBString(10, IsUnicode = true, IsKey = true)]
		[PXDefault]
		[PXUIField(DisplayName = Messages.CampaignID)]
		public override String CampaignID { get; set; }
		#endregion
		#region Selected
		public abstract class selected : IBqlField
		{
		}
		protected bool? _Selected = false;
		[PXBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Selected", Visibility = PXUIVisibility.Service)]
		public bool? Selected
		{
			get
			{
				return _Selected;
			}
			set
			{
				_Selected = value;
			}
		}
		#endregion
		#region DisplayName
		public abstract class displayName : PX.Data.IBqlField
		{
		}
		protected String _displayName;
		[PXUIField(DisplayName = "Display Name", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[ContactDisplayName(typeof(SelCampaignMembers.lastName), typeof(SelCampaignMembers.firstName),
			typeof(SelCampaignMembers.midName), typeof(SelCampaignMembers.title), true,
			BqlField = typeof(Contact.displayName))]
		public virtual String DisplayName
		{
			get { return _displayName; }
			set { _displayName = value; }
		}
		#endregion
		#region FirstName
		public abstract class firstName : PX.Data.IBqlField
		{
		}
		protected String _FirstName;
		[PXDBString(50, IsUnicode = true, BqlField = typeof(Contact.firstName))]
		[PXUIField(DisplayName = "First Name")]
		public virtual String FirstName
		{
			get
			{
				return this._FirstName;
			}
			set
			{
				this._FirstName = value;
			}
		}
		#endregion
		#region MidName
		public abstract class midName : PX.Data.IBqlField
		{
		}
		protected String _MidName;
		[PXDBString(50, IsUnicode = true, BqlField = typeof(Contact.midName))]
		[PXUIField(DisplayName = "Middle Name")]
		public virtual String MidName
		{
			get
			{
				return this._MidName;
			}
			set
			{
				this._MidName = value;
			}
		}
		#endregion
		#region LastName
		public abstract class lastName : PX.Data.IBqlField
		{
		}
		protected String _LastName;
		[PXDBString(100, IsUnicode = true, BqlField = typeof(Contact.lastName))]
		[PXUIField(DisplayName = "Last Name")]
		public virtual String LastName
		{
			get
			{
				return this._LastName;
			}
			set
			{
				this._LastName = value;
			}
		}
		#endregion
		#region Title
		public abstract class title : PX.Data.IBqlField
		{
		}
		protected String _Title;
		[PXDBString(50, IsUnicode = true, BqlField = typeof(Contact.title))]
		[Titles]
		[PXUIField(DisplayName = "Title")]
		public virtual String Title
		{
			get
			{
				return this._Title;
			}
			set
			{
				this._Title = value;
			}
		}
		#endregion
		#region Salutation
		public abstract class salutation : PX.Data.IBqlField
		{
		}
		protected String _Salutation;
		[PXDBString(255, IsUnicode = true, BqlField = typeof(Contact.salutation))]
		[PXUIField(DisplayName = Messages.Position, Visibility = PXUIVisibility.SelectorVisible)]
		//[PXParentSearch()]
		public virtual String Salutation
		{
			get
			{
				return this._Salutation;
			}
			set
			{
				this._Salutation = value;
			}
		}
		#endregion
		#region EMail
		public abstract class eMail : PX.Data.IBqlField
		{
		}
		protected String _EMail;
		[PXDBEmail(BqlField = typeof(Contact.eMail))]
		[PXUIField(DisplayName = "Email", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String EMail
		{
			get
			{
				return this._EMail;
			}
			set
			{
				this._EMail = value;
			}
		}
		#endregion
		#region Phone1
		public abstract class phone1 : PX.Data.IBqlField
		{
		}
		protected String _Phone1;
		[PXDBString(50, BqlField = typeof(Contact.phone1))]
		[PXUIField(DisplayName = "Phone 1", Visibility = PXUIVisibility.SelectorVisible)]
		[PhoneValidation()]
		public virtual String Phone1
		{
			get
			{
				return this._Phone1;
			}
			set
			{
				this._Phone1 = value;
			}
		}
		#endregion
		#region BAccountID
		public abstract class bAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _BAccountID;
		[PXDBInt(IsKey = false, BqlField = typeof(Contact.bAccountID))]
		//[PXDBLiteDefault(typeof(BAccount.bAccountID))]
		[PXDimensionSelector("BIZACCT", typeof(Search<BAccount.bAccountID>), typeof(BAccount.acctCD), DescriptionField = typeof(BAccount.acctName), DirtyRead = true)]
		[PXUIField(DisplayName = "Customer ID", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Int32? BAccountID
		{
			get
			{
				return this._BAccountID;
			}
			set
			{
				this._BAccountID = value;
			}
		}
		#endregion
		#region ContactContactID
		public abstract class contactContactID : PX.Data.IBqlField
		{
		}
		protected Int32? _ContactContactID;
		[PXDBInt(BqlField = typeof(Contact.contactID))]
		[PXUIField(Visibility = PXUIVisibility.Invisible)]
		[PXExtraKey]
		public virtual Int32? ContactContactID
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		#endregion
	}

	#endregion
}
