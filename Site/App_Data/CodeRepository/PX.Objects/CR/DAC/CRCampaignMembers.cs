using System;
using PX.Data;

namespace PX.Objects.CR
{
	[Serializable]
	[PXCacheName(Messages.CampaignMember)]
	[PXEMailSource]
	public partial class CRCampaignMembers : IBqlTable
	{
		#region CampaignID
		public abstract class campaignID : IBqlField { }

		[PXDBString(10, IsUnicode = true, IsKey = true)]
		[PXDefault]
		[PXUIField(DisplayName = Messages.CampaignID)]
		[PXSelector(typeof(Search<CRCampaign.campaignID,
			Where<CRCampaign.isActive, Equal<True>>>))]
		public virtual String CampaignID { get; set; }
		#endregion

		#region ContactID
		public abstract class contactID : IBqlField { }

		[PXDBInt(IsKey = true)]
		[PXDefault]
		[PXUIField(DisplayName = "Contact ID")]
		[PXSelector(typeof(Search<Contact.contactID,
			Where<Contact.contactType, Equal<ContactTypesAttribute.lead>,
				Or<Contact.contactType, Equal<ContactTypesAttribute.person>>>>),
			DescriptionField = typeof(Contact.displayName), Filterable = true)]
		[PXParent(typeof(Select<Contact, Where<Contact.contactID, Equal<Current<CRCampaignMembers.contactID>>>>))]
		public virtual Int32? ContactID { get; set; }
		#endregion

		#region Status
		public abstract class status : IBqlField { }

		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Status")]
		[PXStringList(new string[] { "S", "P", "R" },
			new string[] { "Selected", "Processed", "Responded" },
			BqlField = typeof(CRCampaign.defaultMemberStatus))]
		[PXDefault("S")]
		public virtual String Status { get; set; }
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
	}
}
