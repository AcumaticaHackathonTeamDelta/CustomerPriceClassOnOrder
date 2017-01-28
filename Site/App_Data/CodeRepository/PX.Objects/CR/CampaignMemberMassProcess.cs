using System;
using System.Collections.Specialized;
using PX.Data;
using PX.Objects.GL;
using System.Collections.Generic;
using PX.SM;

namespace PX.Objects.CR
{
	public class CampaignMemberMassProcess : PXGraph<CampaignMemberMassProcess>
	{
		#region DACs

		#region CampaignOperationParam

		[Serializable]
		public partial class CampaignOperationParam : OperationParam
		{
			public abstract class campaignID : IBqlField { }
			protected String _CampaignID;
			[PXDBString]
			[PXDefault]
			[PXUIField(DisplayName = "Campaign ID")]
			[PXSelector(typeof(CRCampaign.campaignID), DescriptionField = typeof(CRCampaign.campaignName))]
			public virtual String CampaignID
			{
				get
				{
					return this._CampaignID;
				}
				set
				{
					this._CampaignID = value;
				}
			}			

			public abstract class status : IBqlField { }
			protected String _Status;
			[PXDBString(1, IsFixed = true)]
			[PXUIField(DisplayName = "Members' Status")]
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

			public new class action : IBqlField
			{
			}
			[PXDBString(10)]
			[PXUIField(DisplayName = "Action", Visibility = PXUIVisibility.SelectorVisible)]
			[ActionList]
			[PXDefault(ActionList.Add)]
			public new string Action
			{
				get
				{
					return _Action;
				}
				set
				{
					_Action = value;
				}
			}

			public class ActionList : PXStringListAttribute
			{
				public ActionList()
					: base(new[] {Add, Update, Remove}, new[] {Messages.Add, Messages.Update, Messages.Remove })
				{
					
				}
				public const string Add = "Add";
				public const string Update = "Update";
				public const string Remove = "Remove";

				public class add : Constant<string>{public add() : base(Add){}}
				public class update : Constant<string>{public update() : base(Update){}}
				public class remove : Constant<string>{public remove() : base(Remove){}}
			}
		}
		
		#endregion				

		#endregion

		#region Actions
		public PXCancel<CampaignOperationParam> Cancel;
		#endregion

		#region Selects
		[PXHidden]
		public PXFilter<CampaignOperationParam>
			Operations;

		[PXHidden]
		public PXSetup<Company> 
			company;

		[PXHidden]
		public PXSelect<Contact>
			BaseContacts;

		[PXHidden]
		public PXSelect<BAccount>
			BAccount;

		[PXHidden]
		public PXFilter<CampaignOperationParam>
			ProcessParameters;		


		[PXFilterable]
		[PXViewDetailsButton(typeof(CampaignOperationParam))]
		[PXViewDetailsButton(typeof(CampaignOperationParam),
			typeof(Select<BAccount,
				Where<BAccount.bAccountID, Equal<Current<Contact.bAccountID>>>>))]
		[PXViewDetailsButton(typeof(CampaignOperationParam),
			typeof(Select<BAccountParent,
				Where<BAccountParent.bAccountID, Equal<Current<Contact.parentBAccountID>>>>))]
		public PXFilteredProcessingJoin<Contact, CampaignOperationParam,
			LeftJoin<CRCampaignMembers, On<CRCampaignMembers.contactID, Equal<Contact.contactID>,
				And<CRCampaignMembers.campaignID, Equal<Current<CampaignOperationParam.campaignID>>>>,
			LeftJoin<CRCampaign, On<CRCampaign.campaignID, Equal<CRCampaignMembers.campaignID>>,
			LeftJoin<BAccount, On<BAccount.bAccountID, Equal<Contact.bAccountID>>,
			LeftJoin<Address, On<Address.addressID, Equal<Contact.defAddressID>>,
			LeftJoin<BAccountParent, On<BAccountParent.bAccountID, Equal<Contact.parentBAccountID>>>>>>>,		
			Where<Current<CampaignOperationParam.campaignID>, IsNotNull, 
					And2< 
					Where<Contact.contactType, Equal<ContactTypesAttribute.lead>,
						Or<Contact.contactType, Equal<ContactTypesAttribute.person>>>,
					And<Where2<Where<Not2<Where<Current<CampaignOperationParam.action>, NotEqual<CampaignOperationParam.ActionList.add>,
								  Or<CRCampaignMembers.contactID, IsNotNull>>,
							 And<Where<Current<CampaignOperationParam.action>, Equal<CampaignOperationParam.ActionList.add>,
							    Or<CRCampaignMembers.contactID, IsNull>>>>>,
					Or<Where2<Where<Current<CampaignOperationParam.action>, Equal<CampaignOperationParam.ActionList.add>,
								  And<CRCampaignMembers.contactID, IsNull>>,
							 Or<Where<Current<CampaignOperationParam.action>, NotEqual<CampaignOperationParam.ActionList.add>,
								And<CRCampaignMembers.contactID, IsNotNull>>>>>>>>>,
				OrderBy<Asc<Contact.displayName>>>
			Items;

		[PXHidden]
		public PXSelect<CRCampaignMembers,
			Where<CRCampaignMembers.campaignID, Equal<Current<CampaignOperationParam.campaignID>>>> 
			CampaignMembers;

		#endregion

		#region Ctors
		public CampaignMemberMassProcess()
		{
			Items.SetParametersDelegate(delegate(List<Contact> list)
			{
				bool result = AskProcess(list);
				CampaignMemberMassProcess process = this.Clone();
				Items.SetProcessDelegate(process.Process);
				return result;
			});


			PXDBAttributeAttribute.Activate(Items.Cache);			
			PXUIFieldAttribute.SetVisible(Items.Cache, null, false);
			PXUIFieldAttribute.SetVisible<Contact.fullName>(Items.Cache, null);
			PXUIFieldAttribute.SetVisible<Contact.salutation>(Items.Cache, null);
			PXUIFieldAttribute.SetVisible<Contact.displayName>(Items.Cache, null);
			PXUIFieldAttribute.SetVisible<Contact.bAccountID>(Items.Cache, null);
			PXUIFieldAttribute.SetVisible<Contact.eMail>(Items.Cache, null);
			PXUIFieldAttribute.SetVisible<Contact.phone1>(Items.Cache, null);
			PXUIFieldAttribute.SetVisible<Contact.status>(Items.Cache, null);
			PXUIFieldAttribute.SetRequired<Contact.displayName>(Items.Cache, false);

			var bAccountCache = Caches[typeof(BAccount)];
			bAccountCache.DisplayName = Messages.Customer;
			PXDBAttributeAttribute.Activate(bAccountCache);			
			PXUIFieldAttribute.SetVisible(bAccountCache, null, false);
			PXUIFieldAttribute.SetVisible<BAccount.acctCD>(bAccountCache, null, false);
			PXUIFieldAttribute.SetDisplayName<BAccount.acctCD>(bAccountCache, Messages.BAccountCD);
			PXUIFieldAttribute.SetDisplayName<BAccount.acctName>(bAccountCache, Messages.BAccountName);

			var parentBAccountCache = Caches[typeof(BAccountParent)];
			parentBAccountCache.DisplayName = Messages.ParentAccount;
			PXUIFieldAttribute.SetVisible(parentBAccountCache, null, false);
			PXUIFieldAttribute.SetDisplayName<BAccountParent.acctCD>(parentBAccountCache, Messages.ParentAccountID);
			PXUIFieldAttribute.SetDisplayName<BAccountParent.acctName>(parentBAccountCache, Messages.ParentAccountName);

			PXUIFieldAttribute.SetVisible(Caches[typeof(Address)], null, false);

			PXUIFieldAttribute.SetDisplayName<CRCampaignMembers.status>(Caches[typeof(CRCampaignMembers)], Messages.CampaignStatus);

			Actions["Schedule"].SetVisible(false);
		}
		#endregion

		#region Process
		protected virtual bool AskProcess(List<Contact> list)
		{
			if (Operations.Current.CampaignID == null) return false;
			CRCampaign campaign = PXSelect<CRCampaign,
					Where<CRCampaign.campaignID, Equal<Current<CampaignOperationParam.campaignID>>>>
					.Select(this);

			if (Operations.Current.Action == CampaignOperationParam.ActionList.Update)
			{
				return PXView.AskExt(this, "UpdateMembers", null, (s, e) =>
					{
						Operations.Current.Status = campaign != null ? campaign.DefaultMemberStatus : null;
					}) == WebDialogResult.OK && Operations.Current.Status != null;
			}

			if (Operations.Current.Action == CampaignOperationParam.ActionList.Remove)
			{
				return (Operations.Current.CampaignID != null
						&& Operations.Ask(Messages.AskConfirmation,
                                          PXMessages.LocalizeFormatNoPrefix(Messages.ConfirmRemoving, list.Count),
										  MessageButtons.YesNoCancel) == WebDialogResult.Yes);
			}

			return PXView.AskExt(this, "AddMembers", null, (s, e) =>
				{
					Operations.Current.Status = campaign != null ? campaign.DefaultMemberStatus : null;
				}) == WebDialogResult.OK && Operations.Current.Status != null;		
		}

		protected virtual void Process(List<Contact> list)
		{
			int result = PXProcessing.ProcessItems(list, item =>
				{
					if (Operations.Current.Action == CampaignOperationParam.ActionList.Update)
					{
						CRCampaignMembers member = PXSelect<CRCampaignMembers,
							Where<CRCampaignMembers.campaignID, Equal<Current<CampaignOperationParam.campaignID>>,
								And<CRCampaignMembers.contactID, Equal<Required<CRCampaignMembers.contactID>>>>>.
							Select(this, item.ContactID);

						if (member != null)
						{
							CRCampaignMembers upd = PXCache<CRCampaignMembers>.CreateCopy(member);
							upd.Status = Operations.Current.Status;
							CampaignMembers.Cache.Update(upd);
						}
					}
					else if (Operations.Current.Action == CampaignOperationParam.ActionList.Remove)
					{
						var member = (CRCampaignMembers)PXSelect<CRCampaignMembers,
									 Where<CRCampaignMembers.campaignID, Equal<Current<CampaignOperationParam.campaignID>>,
									 And<CRCampaignMembers.contactID, Equal<Required<CRCampaignMembers.contactID>>>>>.
									 Select(this, item.ContactID);
						if (member != null)
						{
							CampaignMembers.Cache.Delete(member);
						}
					}
					else if (item.Selected == true)
					{
						OrderedDictionary doc = new OrderedDictionary();
						doc.Add("CampaignID", Operations.Current.CampaignID);
						doc.Add("ContactID", item.ContactID);
						doc.Add("Status", Operations.Current.Status);
						CampaignMembers.Cache.Update(doc, doc);
					}
				});
		
			try
			{
				if(result > 0)
					Actions.PressSave();
				Items.Cache.Clear();
				if (result != list.Count)
					throw new PXOperationCompletedWithErrorException(ErrorMessages.SeveralItemsFailed);
			}
			finally
			{
				CampaignMembers.Cache.Clear();
			}																																				
		}
		#endregion	

		#region Event Handlers
		protected virtual void CampaignOperationParam_Action_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			this.Items.Cache.Clear();
		}
		protected virtual void CampaignOperationParam_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			PXUIFieldAttribute.SetVisible<CRCampaignMembers.status>(Caches[typeof(CRCampaignMembers)], null, Operations.Current.Action != CampaignOperationParam.ActionList.Add);
		}
		
		protected virtual void Contact_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			e.Cancel = true;
		}
		#endregion

	}
}
