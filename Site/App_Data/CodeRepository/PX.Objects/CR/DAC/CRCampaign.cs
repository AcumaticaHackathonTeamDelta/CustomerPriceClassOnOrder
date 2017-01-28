namespace PX.Objects.CR
{
	using System;
	using PX.Data;
	using PX.Objects.CM;
	using PX.Objects.CS;
    using PX.Data.EP;

	[PXCacheName(Messages.Campaign)]
	[System.SerializableAttribute()]
	[PXPrimaryGraph(typeof(CampaignMaint))]
	public partial class CRCampaign : PX.Data.IBqlTable
	{
		#region Selected
		public abstract class selected : IBqlField
		{
		}
		protected bool? _Selected = false;
		[PXBool]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Selected")]
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
		#region CampaignID
		public abstract class campaignID : PX.Data.IBqlField
		{
		}
		protected String _CampaignID;
		[PXDBString(10, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXDefault()]
		[PXUIField(DisplayName = Messages.CampaignID, Visibility = PXUIVisibility.SelectorVisible)]
		[AutoNumber(typeof(CRSetup.campaignNumberingID), typeof(AccessInfo.businessDate))]
		[PXSelector(typeof(CRCampaign.campaignID), DescriptionField = typeof(CRCampaign.campaignName))]
        [PXFieldDescription]
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
		#endregion
		#region CampaignName
		public abstract class campaignName : PX.Data.IBqlField
		{
		}
		protected String _CampaignName;
		[PXDBString(60, IsUnicode = true)]
		[PXDefault("")]
		[PXUIField(DisplayName = "Campaign Name", Visibility = PXUIVisibility.SelectorVisible)]
		[PXNavigateSelector(typeof(CRCampaign.campaignName))]
        [PXFieldDescription]
		public virtual String CampaignName
		{
			get
			{
				return this._CampaignName;
			}
			set
			{
				this._CampaignName = value;
			}
		}
		#endregion
		#region CampaignType
		public abstract class campaignType : PX.Data.IBqlField
		{
		}
		protected String _CampaignType;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault("")]
		[PXUIField(DisplayName = "Campaign Type", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(CRCampaignType.typeID), DescriptionField = typeof(CRCampaignType.description))]
		public virtual String CampaignType
		{
			get
			{
				return this._CampaignType;
			}
			set
			{
				this._CampaignType = value;
			}
		}
		#endregion
		#region Status
		public abstract class status : PX.Data.IBqlField
		{
		}
		protected String _Status;
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(CampaignStatusesAttribute._PREPARED)]
		[CampaignStatuses]
		public virtual String Status
		{
			get
			{
				return this._Status;
			}
			set
			{
				this._Status = value;
			}
		}
		#endregion
		#region IsActive
		public abstract class isActive : PX.Data.IBqlField
		{
		}
		protected bool? _IsActive;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Active")]
		public virtual bool? IsActive
		{
			get
			{
				return this._IsActive;
			}
			set
			{
				this._IsActive = value;
			}
		}
		#endregion
		#region DefaultMemberStatus
		public abstract class defaultMemberStatus : PX.Data.IBqlField
		{
		}
		protected String _DefaultMemberStatus;
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Default Member Status")]
		[PXStringList(new string[] { "S", "P", "R" },
			new string[] { "Selected", "Processed", "Responded" })]
		[PXDefault("S")]
		public virtual String DefaultMemberStatus
		{
			get
			{
				return this._DefaultMemberStatus;
			}
			set
			{
				this._DefaultMemberStatus = value;
			}
		}
		#endregion
		#region StartDate
		public abstract class startDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _StartDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Start Date")]
		public virtual DateTime? StartDate
		{
			get
			{
				return this._StartDate;
			}
			set
			{
				this._StartDate = value;
			}
		}
		#endregion
		#region EndDate
		public abstract class endDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _EndDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "End Date")]
		public virtual DateTime? EndDate
		{
			get
			{
				return this._EndDate;
			}
			set
			{
				this._EndDate = value;
			}
		}
		#endregion
		#region ExpectedRevenue
		public abstract class expectedRevenue : PX.Data.IBqlField
		{
		}
		protected Decimal? _ExpectedRevenue;
		[PXDBBaseCury()]
		[PXUIField(DisplayName = "Expected Revenue")]
		public virtual Decimal? ExpectedRevenue
		{
			get
			{
				return this._ExpectedRevenue;
			}
			set
			{
				this._ExpectedRevenue = value;
			}
		}
		#endregion
		#region PlannedBudget
		public abstract class plannedBudget : PX.Data.IBqlField
		{
		}
		protected Decimal? _PlannedBudget;
		[PXDBBaseCury()]
		[PXUIField(DisplayName = "Planned Budget")]
		public virtual Decimal? PlannedBudget
		{
			get
			{
				return this._PlannedBudget;
			}
			set
			{
				this._PlannedBudget = value;
			}
		}
		#endregion
		#region ActualCost
		public abstract class actualCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _ActualCost;
		[PXDBBaseCury()]
		[PXUIField(DisplayName = "Actual Cost")]
		public virtual Decimal? ActualCost
		{
			get
			{
				return this._ActualCost;
			}
			set
			{
				this._ActualCost = value;
			}
		}
		#endregion
		#region ExpectedResponse
		public abstract class expectedResponse : PX.Data.IBqlField
		{
		}
		protected Int32? _ExpectedResponse;
		[PXDBInt()]
		[PXUIField(DisplayName = "Expected Response")]
		public virtual Int32? ExpectedResponse
		{
			get
			{
				return this._ExpectedResponse;
			}
			set
			{
				this._ExpectedResponse = value;
			}
		}
		#endregion
		#region MailsSent
		public abstract class mailsSent : PX.Data.IBqlField
		{
		}
		protected Int32? _MailsSent;
		[PXDBInt()]
		[PXUIField(DisplayName = "Mails Sent")]
		public virtual Int32? MailsSent
		{
			get
			{
				return this._MailsSent;
			}
			set
			{
				this._MailsSent = value;
			}
		}
		#endregion
		#region LeadsGenerated
		public abstract class leadsGenerated : PX.Data.IBqlField
		{
		}
		protected Int32? _LeadsGenerated;
		[PXInt()]
		[PXUIField(DisplayName = "Leads Generated", Enabled = false)]
		[CRCountCalculationAttribute(typeof(Select5<CRCampaignMembers,
			InnerJoin<Contact, On<Contact.contactID, Equal<CRCampaignMembers.contactID>>>,
			Where<CRCampaignMembers.campaignID, Equal<Current<CRCampaign.campaignID>>>,
			Aggregate<Count>>))]
		public virtual Int32? LeadsGenerated
		{
			get
			{
				return this._LeadsGenerated;
			}
			set
			{
				this._LeadsGenerated = value;
			}
		}
		#endregion
		#region LeadsConverted
		public abstract class leadsConverted : PX.Data.IBqlField
		{
		}
		protected Int32? _LeadsConverted;
		[PXInt()]
		[PXUIField(DisplayName = "Leads Converted", Enabled = false)]
		[CRCountCalculationAttribute(typeof(Select5<CRCampaignMembers,
			InnerJoin<Contact, On<Contact.contactID, Equal<CRCampaignMembers.contactID>>>,
			Where<CRCampaignMembers.campaignID, Equal<Current<CRCampaign.campaignID>>,
			And<Contact.status, Equal<LeadStatusesAttribute.closed>, And<Contact.resolution, Equal<LeadResolutionsAttribute.converted>>>>,
			Aggregate<Count>>))]
		public virtual Int32? LeadsConverted
		{
			get
			{
				return this._LeadsConverted;
			}
			set
			{
				this._LeadsConverted = value;
			}
		}
		#endregion
		#region Contacts
		public abstract class contacts : PX.Data.IBqlField
		{
		}
		protected Int32? _Contacts;
		[PXInt()]
		[PXUIField(DisplayName = "Contacts", Enabled = false)]
		[CRCountCalculationAttribute(typeof(Select4<CRCampaignMembers,
			Where<CRCampaignMembers.campaignID, Equal<Current<CRCampaign.campaignID>>>,
			Aggregate<Count>>))]
		public virtual Int32? Contacts
		{
			get
			{
				return this._Contacts;
			}
			set
			{
				this._Contacts = value;
			}
		}
		#endregion
		#region Responses
		public abstract class responses : PX.Data.IBqlField
		{
		}
		protected Int32? _Responses;
		[PXInt()]
		[PXUIField(DisplayName = "Responses", Enabled = false)]
		[CRCountCalculationAttribute(typeof(Select4<CRCampaignMembers,
		   Where<CRCampaignMembers.campaignID, Equal<Current<CRCampaign.campaignID>>,
		   And<CRCampaignMembers.status, Equal<CRSystemCampaignMemberStatusCodes.Responsed>>>,
			Aggregate<Count>>))]
		public virtual Int32? Responses
		{
			get
			{
				return this._Responses;
			}
			set
			{
				this._Responses = value;
			}
		}
		#endregion
		#region Opportunities
		public abstract class opportunities : PX.Data.IBqlField
		{
		}
		protected Int32? _Opportunities;
		[PXInt()]
		[PXUIField(DisplayName = "Opportunities", Enabled = false)]
		[CRCountCalculationAttribute(typeof(Select4<CROpportunity,
			Where<CROpportunity.campaignSourceID, Equal<Current<CRCampaign.campaignID>>>,
			Aggregate<Count>>))]
		public virtual Int32? Opportunities
		{
			get
			{
				return this._Opportunities;
			}
			set
			{
				this._Opportunities = value;
			}
		}
		#endregion
		#region ClosedOpportunities
		public abstract class closedOpportunities : PX.Data.IBqlField
		{
		}
		protected Int32? _ClosedOpportunities;
		[PXInt()]
		[PXUIField(DisplayName = "Won Opportunities", Enabled = false)]
		[CRCountCalculationAttribute(typeof(Select4<CROpportunity,
			Where<CROpportunity.campaignSourceID, Equal<Current<CRCampaign.campaignID>>,
		   And<CROpportunity.status, Equal<OpportunityStatusAttribute.Won>>>,
			Aggregate<Count>>))]
		public virtual Int32? ClosedOpportunities
		{
			get
			{
				return this._ClosedOpportunities;
			}
			set
			{
				this._ClosedOpportunities = value;
			}
		}
		#endregion
		#region OpportunitiesValue
		public abstract class opportunitiesValue : PX.Data.IBqlField
		{
		}
		protected Decimal? _OpportunitiesValue;
		[PXDecimal()]
		[PXUIField(DisplayName = "Opportunities Value", Enabled = false)]
		[CRSummCalculationAttribute(typeof(Select4<CROpportunity,
			Where<CROpportunity.campaignSourceID, Equal<Current<CRCampaign.campaignID>>>,
		   Aggregate<Sum<CROpportunity.rawAmount>>>), typeof(CROpportunity.rawAmount))]
		public virtual Decimal? OpportunitiesValue
		{
			get
			{
				return this._OpportunitiesValue;
			}
			set
			{
				this._OpportunitiesValue = value;
			}
		}
		#endregion
		#region ClosedOpportunitiesValue
		public abstract class closedOpportunitiesValue : PX.Data.IBqlField
		{
		}
		protected Decimal? _ClosedOpportunitiesValue;
		[PXDecimal()]
		[PXUIField(DisplayName = "Won Opportunities Value", Enabled = false)]
		[CRSummCalculationAttribute(typeof(Select4<CROpportunity,
		   Where<CROpportunity.campaignSourceID, Equal<Current<CRCampaign.campaignID>>,
			And<CROpportunity.status, Equal<OpportunityStatusAttribute.Won>>>,
		 Aggregate<Sum<CROpportunity.rawAmount>>>), typeof(CROpportunity.rawAmount))]
		public virtual Decimal? ClosedOpportunitiesValue
		{
			get
			{
				return this._ClosedOpportunitiesValue;
			}
			set
			{
				this._ClosedOpportunitiesValue = value;
			}
		}
		#endregion
		#region PromoCodeID
		public abstract class promoCodeID : PX.Data.IBqlField
		{
		}
		protected String _PromoCodeID;
		[PXDBString(30, IsUnicode = true)]
		[PXDefault("")]
		[PXUIField(DisplayName = "Promo Code", Required = false)]
		public virtual String PromoCodeID
		{
			get
			{
				return this._PromoCodeID;
			}
			set
			{
				this._PromoCodeID = value;
			}
		}
		#endregion
		#region tstamp
		public abstract class Tstamp : PX.Data.IBqlField
		{
		}
		protected Byte[] _tstamp;
		[PXDBTimestamp()]
		public virtual Byte[] tstamp
		{
			get
			{
				return this._tstamp;
			}
			set
			{
				this._tstamp = value;
			}
		}
		#endregion
		#region CreatedDateTime
		public abstract class createdDateTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _CreatedDateTime;
		[PXDBCreatedDateTime()]
		public virtual DateTime? CreatedDateTime
		{
			get
			{
				return this._CreatedDateTime;
			}
			set
			{
				this._CreatedDateTime = value;
			}
		}
		#endregion
		#region CreatedByScreenID
		public abstract class createdByScreenID : PX.Data.IBqlField
		{
		}
		protected String _CreatedByScreenID;
		[PXDBCreatedByScreenID()]
		public virtual String CreatedByScreenID
		{
			get
			{
				return this._CreatedByScreenID;
			}
			set
			{
				this._CreatedByScreenID = value;
			}
		}
		#endregion
		#region CreatedByID
		public abstract class createdByID : PX.Data.IBqlField
		{
		}
		protected Guid? _CreatedByID;
		[PXDBCreatedByID()]
		public virtual Guid? CreatedByID
		{
			get
			{
				return this._CreatedByID;
			}
			set
			{
				this._CreatedByID = value;
			}
		}
		#endregion
		#region LastModifiedByID
		public abstract class lastModifiedByID : PX.Data.IBqlField
		{
		}
		protected Guid? _LastModifiedByID;
		[PXDBLastModifiedByID()]
		public virtual Guid? LastModifiedByID
		{
			get
			{
				return this._LastModifiedByID;
			}
			set
			{
				this._LastModifiedByID = value;
			}
		}
		#endregion
		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : PX.Data.IBqlField
		{
		}
		protected String _LastModifiedByScreenID;
		[PXDBLastModifiedByScreenID()]
		public virtual String LastModifiedByScreenID
		{
			get
			{
				return this._LastModifiedByScreenID;
			}
			set
			{
				this._LastModifiedByScreenID = value;
			}
		}
		#endregion
		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _LastModifiedDateTime;
		[PXDBLastModifiedDateTime()]
		public virtual DateTime? LastModifiedDateTime
		{
			get
			{
				return this._LastModifiedDateTime;
			}
			set
			{
				this._LastModifiedDateTime = value;
			}
		}
		#endregion
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXNote()]
		public virtual Guid? NoteID
		{
			get
			{
				return this._NoteID;
			}
			set
			{
				this._NoteID = value;
			}
		}
		#endregion

		#region TargetStatus
		public abstract class targetStatus : PX.Data.IBqlField
		{
		}
		protected String _TargetStatus = "S";
		[PXString(1, IsFixed = true)]
		[PXDefault("S")]
		[PXUIField(DisplayName = "Target Status")]
		[PXStringList(new string[] { "S", "P", "R" },
			new string[] { "Selected", "Processed", "Responded" },
			BqlField = typeof(CRCampaign.defaultMemberStatus))]
		public virtual String TargetStatus
		{
			get
			{
				return this._TargetStatus;
			}
			set
			{
				this._TargetStatus = value;
			}
		}
		#endregion

		#region DestinationStatus
		public abstract class destinationStatus : PX.Data.IBqlField
		{
		}
		protected String _DestinationStatus = "P";
		[PXString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Destination Status")]
		[PXDefault("P")]
		[PXStringList(new string[] { "S", "P", "R" },
			new string[] { "Selected", "Processed", "Responded" },
			BqlField = typeof(CRCampaign.defaultMemberStatus))]
		public virtual String DestinationStatus
		{
			get
			{
				return this._DestinationStatus;
			}
			set
			{
				this._DestinationStatus = value;
			}
		}
		#endregion
	}
	public class CRSystemCampaignMemberStatusCodes
	{
		public class Sent : Constant<string>
		{
			public Sent()
				: base("S")
			{
			}
		}
		public class Responsed : Constant<string>
		{
			public Responsed()
				: base("P")
			{
			}
		}
	}
	public class CRSystemLeadStatusCodes
	{
		public class Converted : Constant<string>
		{
			public Converted()
				: base("C")
			{
			}
		}
	}
}
