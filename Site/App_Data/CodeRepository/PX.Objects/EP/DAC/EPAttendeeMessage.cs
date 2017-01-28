using System;
using System.Diagnostics;
using PX.Data;
using PX.Objects.CR;

namespace PX.Objects.EP
{
	[Serializable]
	[DebuggerDisplay("EventNoteID = {EventNoteID}, UserID = {UserID}, Type = {Type}")]
	[PXCacheName(Messages.AttendeeMessage)]
	public partial class EPAttendeeMessage : IBqlTable
	{
		#region EventNoteID
		public abstract class eventNoteID : IBqlField { }
		
		[PXDBGuid(IsKey = true)]
		[PXDBDefault(typeof(CRActivity.noteID))]
		public virtual Guid? EventNoteID { get; set; }
		#endregion

		#region UserID
		public abstract class userID : IBqlField { }
		
		[PXDBGuid(IsKey = true)]
		[PXUIField(Visible = false)]
		public virtual Guid? UserID { get; set; }
		#endregion

		#region MessageID
		public abstract class messageID : IBqlField { }
		
		[PXDBGuid(IsKey = true)]
		[PXUIField(Visible = false)]
		public virtual Guid? MessageID { get; set; }
		#endregion

		#region Type
		public abstract class type : IBqlField { }
		
		[EPMessageType.EPMessageTypeList]
		[PXUIField(DisplayName = "Type")]
		public virtual bool? Type { get; set; }
		#endregion

		#region CreatedByID
		public abstract class createdByID : IBqlField { }
		
		[PXDBCreatedByID]
		public virtual Guid? CreatedByID { get; set; }
		#endregion

		#region CreatedByScreenID
		public abstract class createdByScreenID : IBqlField { }
		
		[PXDBCreatedByScreenID]
		public virtual string CreatedByScreenID { get; set; }
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
		
		[PXDBLastModifiedByScreenID]
		public virtual string LastModifiedByScreenID { get; set; }
		#endregion

		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : IBqlField { }
		
		[PXDBLastModifiedDateTime]
		public virtual DateTime? LastModifiedDateTime { get; set; }
		#endregion
	}
}
