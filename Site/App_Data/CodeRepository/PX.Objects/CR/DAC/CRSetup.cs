using System;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.CM;
using PX.Objects.EP;

namespace PX.Objects.CR
{
	[SerializableAttribute]
    [PXPrimaryGraph(typeof(CRSetupMaint))]
    [PXCacheName(Messages.CRSetup)]
    public partial class CRSetup : IBqlTable, PXNoteAttribute.IPXCopySettings
	{
		#region CampaignNumberingID
		public abstract class campaignNumberingID : PX.Data.IBqlField
		{
		}
		protected String _CampaignNumberingID;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault("CAMPAIGN")]
		[PXUIField(DisplayName = "Campaign Numbering Sequence")]
        [PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
		public virtual String CampaignNumberingID
		{
			get
			{
				return this._CampaignNumberingID;
			}
			set
			{
				this._CampaignNumberingID = value;
			}
		}
		#endregion
		#region OpportunityNumberingID
		public abstract class opportunityNumberingID : PX.Data.IBqlField
		{
		}
		protected String _OpportunityNumberingID;
		[PXDBString(10, IsUnicode = true)]
        [PXDefault("OPPORTUNTY")]
		[PXUIField(DisplayName = "Opportunity Numbering Sequence")]
		[PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
		public virtual String OpportunityNumberingID
		{
			get
			{
				return this._OpportunityNumberingID;
			}
			set
			{
				this._OpportunityNumberingID = value;
			}
		}
		#endregion
		#region CaseNumberingID
		public abstract class caseNumberingID : PX.Data.IBqlField
		{
		}
		protected String _CaseNumberingID;
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXDefault("CASE")]
		[PXUIField(DisplayName = "Case Numbering Sequence")]
		[PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
		public virtual String CaseNumberingID
		{
			get
			{
				return this._CaseNumberingID;
			}
			set
			{
				this._CaseNumberingID = value;
			}
		}
		#endregion
		#region MassMailNumberingID
		public abstract class massMailNumberingID : PX.Data.IBqlField
		{
		}
		protected String _MassMailNumberingID;
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXDefault("MMAIL")]
		[PXUIField(DisplayName = "Mass Mail Numbering Sequence")]
		[PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
		public virtual String MassMailNumberingID
		{
			get
			{
				return this._MassMailNumberingID;
			}
			set
			{
				this._MassMailNumberingID = value;
			}
		}
		#endregion
		#region DefaultCaseClassID
		public abstract class defaultCaseClassID : PX.Data.IBqlField
		{
		}
		protected String _DefaultCaseClassID;
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXUIField(DisplayName = "Default Case Class")]
		[PXSelector(typeof(CRCaseClass.caseClassID))]
		public virtual String DefaultCaseClassID
		{
			get
			{
				return this._DefaultCaseClassID;
			}
			set
			{
				this._DefaultCaseClassID = value;
			}
		}
		#endregion
		#region DefaultOpportunityClassID
		public abstract class defaultOpportunityClassID : PX.Data.IBqlField
		{
		}
		protected String _DefaultOpportunityClassID;
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXUIField(DisplayName = "Default Opportunity Class")]
		[PXSelector(typeof(CROpportunityClass.cROpportunityClassID))]
		public virtual String DefaultOpportunityClassID
		{
			get
			{
				return this._DefaultOpportunityClassID;
			}
			set
			{
				this._DefaultOpportunityClassID = value;
			}
		}
		#endregion
		#region DefaultRateTypeID
		public abstract class defaultRateTypeID : PX.Data.IBqlField
		{
		}
		protected String _DefaultRateTypeID;
		[PXDBString(6, IsUnicode = true)]
		[PXSelector(typeof(PX.Objects.CM.CurrencyRateType.curyRateTypeID))]
		[PXUIField(DisplayName = "Default Rate Type ")]
		public virtual String DefaultRateTypeID
		{
			get
			{
				return this._DefaultRateTypeID;
			}
			set
			{
				this._DefaultRateTypeID = value;
			}
		}
		#endregion
		#region DefaultCuryID
		public abstract class defaultCuryID : PX.Data.IBqlField
		{
		}
		protected String _DefaultCuryID;
		[PXDBString(5, IsUnicode = true)]
		[PXSelector(typeof(Currency.curyID), CacheGlobal = true)]
		[PXUIField(DisplayName = "Default Currency")]
		public virtual String DefaultCuryID
		{
			get
			{
				return this._DefaultCuryID;
			}
			set
			{
				this._DefaultCuryID = value;
			}
		}
		#endregion
		#region AllowOverrideCury
		public abstract class allowOverrideCury : PX.Data.IBqlField
		{
		}
		protected Boolean? _AllowOverrideCury;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Enable Currency Override")]
		public virtual Boolean? AllowOverrideCury
		{
			get
			{
				return this._AllowOverrideCury;
			}
			set
			{
				this._AllowOverrideCury = value;
			}
		}
		#endregion
		#region AllowOverrideRate
		public abstract class allowOverrideRate : PX.Data.IBqlField
		{
		}
		protected Boolean? _AllowOverrideRate;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Enable Rate Override")]
		public virtual Boolean? AllowOverrideRate
		{
			get
			{
				return this._AllowOverrideRate;
			}
			set
			{
				this._AllowOverrideRate = value;
			}
		}
		#endregion
		#region LeadDefaultAssignmentMapID
		public abstract class leaddefaultAssignmentMapID : PX.Data.IBqlField
		{
		}
		protected int? _LeadDefaultAssignmentMapID;
		[PXDBInt]
		[PXSelector(typeof(Search<EPAssignmentMap.assignmentMapID,
			Where<EPAssignmentMap.entityType, Equal<AssignmentMapType.AssignmentMapTypeLead>>>))]
		[PXUIField(DisplayName = "Lead Assignment Map")]
		public virtual int? LeadDefaultAssignmentMapID
		{
			get
			{
				return this._LeadDefaultAssignmentMapID;
			}
			set
			{
				this._LeadDefaultAssignmentMapID = value;
			}
		}
		#endregion

		#region ContactDefaultAssignmentMapID
		public abstract class contactdefaultAssignmentMapID : PX.Data.IBqlField
		{
		}
		protected int? _ContactDefaultAssignmentMapID;
		[PXDBInt]
		[PXSelector(typeof(Search<EPAssignmentMap.assignmentMapID,
			Where<EPAssignmentMap.entityType, Equal<AssignmentMapType.AssignmentMapTypeLead>>>))]
		[PXUIField(DisplayName = "Contact Assignment Map")]
		public virtual int? ContactDefaultAssignmentMapID
		{
			get
			{
				return this._ContactDefaultAssignmentMapID;
			}
			set
			{
				this._ContactDefaultAssignmentMapID = value;
			}
		}
		#endregion

		#region DefaultCaseAssignmentMapID
		public abstract class defaultCaseAssignmentMapID : PX.Data.IBqlField
		{
		}
		protected int? _DefaultCaseAssignmentMapID;
		[PXDBInt]
		[PXSelector(typeof(Search<EPAssignmentMap.assignmentMapID, Where<EPAssignmentMap.entityType, Equal<AssignmentMapType.AssignmentMapTypeCase>>>))]
		[PXUIField(DisplayName = "Case Assignment Map")]
		public virtual int? DefaultCaseAssignmentMapID
		{
			get
			{
				return this._DefaultCaseAssignmentMapID;
			}
			set
			{
				this._DefaultCaseAssignmentMapID = value;
			}
		}
		#endregion
		#region DefaultBAccountAssignmentMapID
		public abstract class defaultBAccountAssignmentMapID : PX.Data.IBqlField
		{
		}
		[PXDBInt]
		[PXSelector(typeof(Search<EPAssignmentMap.assignmentMapID,
			Where<EPAssignmentMap.entityType, Equal<AssignmentMapType.AssignmentMapTypeProspect>>>))]
		[PXUIField(DisplayName = "Business Account Assignment Map")]
		public virtual int? DefaultBAccountAssignmentMapID { get; set; }
		#endregion

		#region DefaultOpportunityAssignmentMapID
		public abstract class defaultOpportunityAssignmentMapID : PX.Data.IBqlField
		{
		}
		[PXDBInt]
		[PXSelector(typeof(Search<EPAssignmentMap.assignmentMapID,
			Where<EPAssignmentMap.entityType, Equal<AssignmentMapType.AssignmentMapTypeOpportunity>>>))]
		[PXUIField(DisplayName = "Opportunity Assignment Map")]
		public virtual int? DefaultOpportunityAssignmentMapID { get; set; }
		#endregion

		#region DefaultLeadClassID
		public abstract class defaultLeadClassID : IBqlField {}
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXUIField(DisplayName = "Default Lead Class")]
		[PXSelector(typeof(CRContactClass.classID))]
		public virtual String DefaultLeadClassID { get; set; }
		#endregion

		#region DefaultContactClassID
		public abstract class defaultContactClassID : IBqlField { }
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXUIField(DisplayName = "Default Contact Class")]
		[PXSelector(typeof(CRContactClass.classID))]
		public virtual String DefaultContactClassID { get; set; }
		#endregion

		#region DefaultCustomerClassID
		public abstract class defaultCustomerClassID : PX.Data.IBqlField
		{
		}
		protected String _DefaultCustomerClassID;
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXUIField(DisplayName = "Default Business Account Class")]
		[PXSelector(typeof(CRCustomerClass.cRCustomerClassID))]
		public virtual String DefaultCustomerClassID
		{
			get
			{
				return this._DefaultCustomerClassID;
			}
			set
			{
				this._DefaultCustomerClassID = value;
			}
		}
		#endregion
		
		#region ShowAuthorForCaseEmail

		public abstract class showAuthorForCaseEmail : IBqlField { }

		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXDBBool]
		[PXUIField(DisplayName = "Show Author's Email Address")]
		public virtual Boolean? ShowAuthorForCaseEmail { get; set; }

		#endregion
	

		#region PurgeAgeOfNotConvertedLeads
		public abstract class purgeAgeOfNotConvertedLeads : IBqlField { }
		[PXDBInt(MinValue = 0)]
		[PXDefault(6, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Purge Unconverted Leads that Are Older than")]
		public virtual int? PurgeAgeOfNotConvertedLeads { get; set; }
		#endregion
		#region PurgePeriodWithoutActivity
		public abstract class purgePeriodWithoutActivity : IBqlField { }
		[PXDBInt(MinValue = 0)]
		[PXDefault(9, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Purge Inactive Contacts with No Activities for More than")]
		public virtual int? PurgePeriodWithoutActivity { get; set; }
		#endregion

		#region CopyNotes
		public abstract class copyNotes : IBqlField
		{
		}
		protected bool? _CopyNotes;
		[PXDBBool]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Copy Notes")]
		public virtual bool? CopyNotes
		{
			get
			{
				return _CopyNotes;
			}
			set
			{
				_CopyNotes = value;
			}
		}
		#endregion
		#region CopyFiles
		public abstract class copyFiles : IBqlField
		{
		}
		protected bool? _CopyFiles;
		[PXDBBool]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Copy Attachments")]
		public virtual bool? CopyFiles
		{
			get
			{
				return _CopyFiles;
			}
			set
			{
				_CopyFiles = value;
			}
		}
		#endregion

		#region CheckUniqueEmails
		public abstract class contactEmailUnique : IBqlField { }

		[PXDBBool]
		[PXUIField(DisplayName = "Require Unique Email Address on Contact Validation")]
		[PXDefault(false)]
		public virtual Boolean? ContactEmailUnique { get; set; }
		#endregion

		#region ValidateContactDuplicatesOnEntry
		public abstract class validateContactDuplicatesOnEntry : IBqlField { }

		[PXDBBool]
		[PXUIField(DisplayName = "Validate Leads and Contacts for Duplicates on Entry")]
		[PXDefault(false)]
		public virtual Boolean? ValidateContactDuplicatesOnEntry { get; set; }
		#endregion

		#region ValidateAccountDuplcatesOnEntry
		public abstract class validateAccountDuplicatesOnEntry : IBqlField { }

		[PXDBBool]
		[PXUIField(DisplayName = "Validate Accounts for Duplicates on Entry")]
		[PXDefault(false)]
		public virtual Boolean? ValidateAccountDuplicatesOnEntry { get; set; }
		#endregion

		#region LeadValidationThreshold
		public abstract class leadValidationThreshold : IBqlField { }

		[PXDBDecimal(2)]
		[PXUIField(DisplayName = "Lead and Contact Validation Score Threshold")]
		[PXDefault(TypeCode.Decimal, "5.0")]
		public virtual decimal? LeadValidationThreshold { get; set; }
		#endregion

		#region AccountValidationThreshold
		public abstract class accountValidationThreshold : IBqlField { }

		[PXDBDecimal(2)]
		[PXUIField(DisplayName = "Account Validation Score Threshold")]
		[PXDefault(TypeCode.Decimal, "5.0")]
		public virtual decimal? AccountValidationThreshold { get; set; }
		#endregion

		#region LeadToAccountValidationThreshold
		public abstract class leadToAccountValidationThreshold : IBqlField { }

		[PXDBDecimal(2)]
		[PXUIField(DisplayName = "Lead and Account Validation Score Threshold")]
		[PXDefault(TypeCode.Decimal, "6.0")]
		public virtual decimal? LeadToAccountValidationThreshold { get; set; }
		#endregion

		#region CloseLeadsWithoutActivitiesScore
		public abstract class closeLeadsWithoutActivitiesScore : IBqlField { }

		[PXDBDecimal(2)]
		[PXUIField(DisplayName = "Close Leads with no Activities with Score Higher Than")]
		[PXDefault(TypeCode.Decimal, "6.0")]
		public virtual decimal? CloseLeadsWithoutActivitiesScore { get; set; }
		#endregion

		#region GrammValidationDateTime
		public abstract class grammValidationDateTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _GrammValidationDateTime;	
		[PXDBDateAndTime]
		public virtual DateTime? GrammValidationDateTime
		{
			get
			{
				return this._GrammValidationDateTime;
			}
			set
			{
				this._GrammValidationDateTime = value;
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
		#region NoteID
		public abstract class noteID : IBqlField { }

		[PXNote]
		public virtual Guid? NoteID { get; set; }
		#endregion
	}
}
