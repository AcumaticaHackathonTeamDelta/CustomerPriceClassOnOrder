using System;
using System.Collections;
using PX.Data;
using PX.Objects.CS;
using PX.SM;


namespace PX.Objects.CR
{
	[DashboardType(PX.TM.OwnedFilter.DASHBOARD_TYPE, GL.TableAndChartDashboardTypeAttribute._AMCHARTS_DASHBOART_TYPE)]
	public class CampaignEnq : PXGraph<CampaignEnq>
	{
		#region CampaignFilter

		[Serializable]
		public partial class CampaignFilter : OwnedFilter
		{
			#region CampaignID
			public abstract class campaignID : IBqlField { }
			protected String _CampaignID;
			[PXString]
			[PXUIField(DisplayName = "Campaign")]
			[PXSelector(typeof(CRCampaign.campaignID), DescriptionField = typeof(CRCampaign.campaignName))]
			public virtual String CampaignID
			{
				get
				{
					return _CampaignID;
				}
				set
				{
					_CampaignID = value;
				}
			}
			#endregion

			#region Status
			public abstract class status : IBqlField { }
			protected String _Status;
			[PXString]
			[PXUIField(DisplayName = "Status")]
			[PXStringList(new string[] { "S", "P", "R" },
				new string[] { "Selected", "Processed", "Responded" },
				BqlField = typeof(CRCampaign.defaultMemberStatus))]
			public virtual String Status
			{
				get
				{
					return _Status;
				}
				set
				{
					_Status = value;
				}
			}
			#endregion
		}
		#endregion

		#region Selects

		[PXHidden]
		public PXSelect<BAccount>
			BaseAccounts;

		[PXHidden]
		public PXSelect<Contact>
			BaseContacts;

		[PXViewName(Messages.Selection)]
		public PXFilter<CampaignFilter>
			Filter;

		[PXViewName(Messages.CampaignMembers)]
		[PXFilterable]
        [PXViewDetailsButton(typeof(CampaignFilter), OnClosingPopup = PXSpecialButtonType.Refresh)]
		public PXOwnerFilteredSelectReadonly<CampaignFilter,
			Select2<Contact,
			InnerJoin<CRCampaignMembers, On<CRCampaignMembers.contactID, Equal<Contact.contactID>>,
			LeftJoin<CRCampaign, On<CRCampaign.campaignID, Equal<CRCampaignMembers.campaignID>>,
			LeftJoin<BAccount, On<BAccount.bAccountID, Equal<Contact.bAccountID>>,
			LeftJoin<Address, On<Address.addressID, Equal<Contact.defAddressID>>,
			LeftJoin<State,
					On<State.countryID, Equal<Address.countryID>,
						And<State.stateID, Equal<Address.state>>>>>>>>,
			Where2<
				Where<Current<CampaignFilter.campaignID>, IsNull,
					Or<Current<CampaignFilter.campaignID>, Equal<CRCampaign.campaignID>>>,
				And<
					Where<Current<CampaignFilter.status>, IsNull,
						Or<Current<CampaignFilter.status>, Equal<CRCampaignMembers.status>>>>>>,
			Contact.workgroupID, Contact.ownerID>
			FilteredItems;

		#endregion

		#region Ctors

		public CampaignEnq()
		{
			FilteredItems.NewRecordTarget = typeof(ContactMaint);
			Actions.Move("FilteredItems_AddNew", "AddNewLead");
			Actions["FilteredItems_AddNew"].SetCaption("New Contact");

			PXUIFieldAttribute.SetRequired<Contact.displayName>(FilteredItems.Cache, false);

			var bAccountCache = Caches[typeof(BAccount)];
			bAccountCache.DisplayName = Messages.Customer;
			PXUIFieldAttribute.SetDisplayName<BAccount.acctCD>(bAccountCache, Messages.BAccountCD);
			PXUIFieldAttribute.SetDisplayName<BAccount.acctName>(bAccountCache, Messages.BAccountName);


			var stateCache = Caches[typeof(State)];
			PXUIFieldAttribute.SetDisplayName<State.name>(stateCache, Messages.State);

			var campaignCache = Caches[typeof(CRCampaign)];
			PXUIFieldAttribute.SetRequired<CRCampaign.campaignID>(campaignCache, false);
			PXUIFieldAttribute.SetRequired<CRCampaign.campaignName>(campaignCache, false);
			PXUIFieldAttribute.SetDisplayName<CRCampaign.status>(campaignCache, Messages.CampaignStatus);

		}

		#endregion

		#region Actions

		public PXCancel<CampaignFilter> Cancel;

		public PXAction<CampaignFilter> addNewLead;
		[PXUIField(DisplayName = Messages.AddNewLead)]
        [PXButton(Tooltip = "Add New Record", CommitChanges = true, OnClosingPopup = PXSpecialButtonType.Cancel)]
		public virtual void AddNewLead()
		{
			var target = PXGraph.CreateInstance<LeadMaint>();
			var targetCache = target.Lead.Cache;
			var row = targetCache.Insert();
			var newRow = (Contact)targetCache.CreateCopy(row);
			newRow.WorkgroupID = Filter.Current.WorkGroupID;
			newRow.OwnerID = Filter.Current.OwnerID;
			targetCache.Update(newRow);
			PXRedirectHelper.TryRedirect(target, PXRedirectHelper.WindowMode.NewWindow);
		}

		#endregion

		#region Event Handlers

		[PXUIField(DisplayName = "Company Name")]
		[CRLeadFullName(typeof(Contact.bAccountID))]
		public virtual void Contact_FullName_CacheAttached(PXCache sender)
		{

		}

		[PXDBString(2, IsFixed = true)]
		[PXDefault(ContactTypesAttribute.Person, PersistingCheck = PXPersistingCheck.Nothing)]
		[ContactTypes]
		[PXUIField(DisplayName = "Type", Enabled = false)]
		protected virtual void Contact_ContactType_CacheAttached(PXCache sender)
		{

		}

		#endregion

		#region Overrides

		public override bool IsDirty
		{
			get
			{
				return false;
			}
		}

		#endregion
	}
}
