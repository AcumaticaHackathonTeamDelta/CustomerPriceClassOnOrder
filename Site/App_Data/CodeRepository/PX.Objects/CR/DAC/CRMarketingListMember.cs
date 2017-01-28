using System;
using PX.Data;
using PX.Objects.CS;

namespace PX.Objects.CR
{
	[Serializable]
	[PXCacheName(Messages.MarketingListMember)]
	public partial class CRMarketingListMember : IBqlTable
	{
		#region ContactID

		public abstract class contactID : IBqlField { }

		[PXDBInt(IsKey = true)]
		[PXUIField(DisplayName = "Contact")]
		[PXSelector(typeof(Search<ContactBAccount.contactID>),
			DescriptionField = typeof(ContactBAccount.contact), 
			Filterable = true, 
			DirtyRead = true)]
		[PXParent(typeof(Select<Contact, Where<Contact.contactID, Equal<Current<CRMarketingListMember.contactID>>>>))]
		public virtual Int32? ContactID { get; set; }

		#endregion

		#region MarketingListID

		public abstract class marketingListID : IBqlField { }

		[PXDBInt(IsKey = true)]
		[PXDBLiteDefault(typeof(CRMarketingList.marketingListID))]
		[PXUIField(DisplayName = "Marketing List ID")]
		[PXSelector(typeof(Search<CRMarketingList.marketingListID>),
		    DescriptionField = typeof(CRMarketingList.mailListCode))]
		public virtual Int32? MarketingListID { get; set; }

		#endregion

		#region Activated

		public abstract class activated : IBqlField { }

		[PXDBBool]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Activated")]
		public virtual Boolean? Activated { get; set; }

		#endregion

		#region Format

		public abstract class format : IBqlField { }
		[PXDBString]
		[PXDefault(NotificationFormat.Html)]
		[PXUIField(DisplayName = "Format")]		
		[NotificationFormat.TemplateList]
		public virtual string Format { get; set; }

		#endregion

		#region CreatedByScreenID

		public abstract class createdByScreenID : IBqlField { }

		[PXDBCreatedByScreenID]
		public virtual String CreatedByScreenID { get; set; }

		#endregion

		#region CreatedByID

		public abstract class createdByID : IBqlField { }

		[PXDBCreatedByID]
		public virtual Guid? CreatedByID { get; set; }

		#endregion

		#region CreatedDateTime

		public abstract class createdDateTime : IBqlField { }

		[PXDBCreatedDateTime]
		public virtual DateTime? CreatedDateTime { get; set; }

		#endregion

		#region LastModifiedByID

		public abstract class lastModifiedByID : IBqlField { }

		[PXDBLastModifiedByID]
		public virtual Guid? LastModifiedByID { get; set; }

		#endregion

		#region LastModifiedByScreenID

		public abstract class lastModifiedByScreenID : IBqlField { }

		[PXDBLastModifiedByScreenID()]
		public virtual String LastModifiedByScreenID { get; set; }

		#endregion

		#region LastModifiedDateTime

		public abstract class lastModifiedDateTime : IBqlField { }

		[PXDBLastModifiedDateTime]
		public virtual DateTime? LastModifiedDateTime { get; set; }

		#endregion

		#region tstamp

		public abstract class Tstamp : IBqlField { }

		[PXDBTimestamp]
		public virtual Byte[] tstamp { get; set; }

		#endregion
	}
}
