using System;
using PX.Common;
using PX.Data;
using PX.SM;

namespace PX.Objects.CR
{
	[PXPrimaryGraph(typeof(CRAnnouncementMaint))]
    [Serializable]
	[PXHidden]
	public class CRAnnouncement : PX.Data.IBqlTable
	{
		#region AnnouncementsID
		public abstract class announcementsID : PX.Data.IBqlField
		{
		}
		protected Int32? _AnnouncementsID;
		[PXDBIdentity(IsKey = true)]
		[PXUIField(DisplayName = "Announcement", Visibility = PXUIVisibility.Invisible)]
		[PXSelector(typeof(announcementsID), DescriptionField = typeof(subject))]
		public virtual Int32? AnnouncementsID
		{
			get
			{
				return this._AnnouncementsID;
			}
			set
			{
				this._AnnouncementsID = value;
			}
		}
		#endregion
		
		#region Subject
		public abstract class subject : PX.Data.IBqlField
		{
		}
		protected String _Subject;
		[PXDBString(255, InputMask = "", IsUnicode = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Subject", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String Subject
		{
			get
			{
				return _Subject;
			}
			set
			{
				this._Subject = value;
			}
		}
		#endregion

		#region Body
		public abstract class body : PX.Data.IBqlField
		{
		}
		protected String _Body;
		[PXDBText(IsUnicode = true)]
		[PXUIField(DisplayName = "Body")]
		public virtual String Body
		{
			get
			{
				return this._Body;
			}
			set
			{
				this._Body = value;
			}
		}
		#endregion

		#region Order
		public abstract class order : IBqlField
		{
		}
		protected Int32? _Order;
		[PXInt]
		[PXUIField(DisplayName = "Order")]
		public virtual Int32? Order
		{
			get { return this._Order; }
			set { this._Order = value; }
		}
		#endregion

		#region SmallBody
		public abstract class smallbody : PX.Data.IBqlField
		{
		}
		protected String _Smallbody;
		[PXString(IsUnicode = true)]
		[PXUIField(DisplayName = "Body")]
		public virtual String Smallbody
		{
			get
			{
				return _Smallbody;
			}
			set
			{
				this._Smallbody = value;
			}
		}
		#endregion

		#region MajorStatus
		public abstract class majorStatus : IBqlField { }
		[PXDBInt]
		[AnnouncementMajorStatuses]
		[PXDefault(AnnouncementMajorStatusesAttribute._DRAFT, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(Visible = false)]
		public virtual Int32? MajorStatus { get; set; }
		#endregion

		#region StartDate
		public abstract class startDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _StartDate;
		[PXDBDate(InputMask = "g", PreserveTime = true)]
		[PXUIField(DisplayName = "Publication Date")]
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

		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXNote(new Type[0])]
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

		#region Status
		public abstract class status : IBqlField
		{
		}
		protected string _Status;
		[PXDefault(NotificationStatusAttribute.Draft)]
		[PXStringList(new string[] { NotificationStatusAttribute.Draft, NotificationStatusAttribute.Published, NotificationStatusAttribute.Archived },
			new string[] { "Draft", "Published", "Archived" })]
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Status")]
		public virtual string Status { get; set; }
		#endregion

		#region IsPortalVisible
		public abstract class isPortalVisible : IBqlField
		{
		}
		protected bool? _IsPortalVisible;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Visible on Portal")]
		public virtual bool? IsPortalVisible
		{
			get
			{
				return _IsPortalVisible;
			}
			set
			{
				_IsPortalVisible = value;
			}
		}
		#endregion

		#region PublishedDateTime
		public abstract class publishedDateTime : IBqlField { }
		protected DateTime? _PublishedDateTime;
		[PXDBDate(InputMask = "g", PreserveTime = true, UseTimeZone = true)]
		[PXUIField(DisplayName = "Published Date")]
		public virtual DateTime? PublishedDateTime { get; set; }
		#endregion

		#region Category
		public abstract class category : IBqlField{ }
		protected String _Category;
		[PXDBString(255, InputMask = "", IsUnicode = true)]
		[PXSelector(typeof(Search4<category, Where<category, IsNotNull> ,Aggregate<GroupBy<category>>>))]
		[PXUIField(DisplayName = "Category", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String Category
		{
			get
			{
				return this._Category;
			}
			set
			{
				this._Category = value;
			}
		}

		#endregion

		#region CreatedByID
		public abstract class createdByID : PX.Data.IBqlField { }

		[PXDBCreatedByID]
		[PXUIField(DisplayName = "Created By")]
		public virtual Guid? CreatedByID { get; set; }
		#endregion

		#region CreatedByScreenID
		public abstract class createdByScreenID : PX.Data.IBqlField { }

		[PXDBCreatedByScreenID]
		public virtual String CreatedByScreenID { get; set; }
		#endregion

		#region CreatedDateTime
		public abstract class createdDateTime : PX.Data.IBqlField { }

		[PXDBCreatedDateTime]
		[PXUIField(DisplayName = "Created Date")]
		public virtual DateTime? CreatedDateTime { get; set; }
		#endregion

		#region LastModifiedByID
		public abstract class lastModifiedByID : PX.Data.IBqlField { }

		[PXDBLastModifiedByID]
		[PXUIField(DisplayName = "Last Modified By")]
		public virtual Guid? LastModifiedByID { get; set; }
		#endregion

		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : PX.Data.IBqlField { }

		[PXDBLastModifiedByScreenID]
		public virtual String LastModifiedByScreenID { get; set; }
		#endregion

		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : PX.Data.IBqlField { }

		[PXDBLastModifiedDateTime]
		[PXUIField(DisplayName = "Last Modified Date")]
		public virtual DateTime? LastModifiedDateTime { get; set; }
		#endregion

		#region tstamp
		public abstract class Tstamp : PX.Data.IBqlField { }

		[PXDBTimestamp]
		public virtual Byte[] tstamp { get; set; }
		#endregion
	}
}
