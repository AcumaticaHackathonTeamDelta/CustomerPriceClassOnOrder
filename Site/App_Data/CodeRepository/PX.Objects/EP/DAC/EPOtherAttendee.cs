using System;
using System.Diagnostics;
using PX.Data;
using PX.Objects.CR;

namespace PX.Objects.EP
{
	[Serializable]
	[DebuggerDisplay("EventNoteID = {EventNoteID}, Email = {Email}")]
    [PXHidden]
	public partial class EPOtherAttendee : IBqlTable
	{
		#region EventNoteID
		public abstract class eventNoteID : IBqlField { }
		
		[PXDBGuid(IsKey = true)]
		[PXDBDefault(typeof(CRActivity.noteID))]
		public virtual Guid? EventNoteID { get; set; }
		#endregion

		#region AttendeeID
		public abstract class attendeeID : IBqlField { }
		
		[PXDBIdentity(IsKey = true)]
		[PXUIField(Visible = false)]
		public virtual int? AttendeeID { get; set; }
		#endregion

		#region Email
		public abstract class email : IBqlField { }
		
		[PXDBEmail]
        [PXDefault]
		public virtual string Email { get; set; }
		#endregion

		#region Name
		public abstract class name : IBqlField { }
		
		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "Name")]
		public virtual string Name { get; set; }
		#endregion

		#region Comment
		public abstract class comment : IBqlField { }
		
		[PXDBString(255, IsUnicode = true)]
		[PXUIField(DisplayName = "Comment")]
		public virtual string Comment { get; set; }
		#endregion

		#region Invitation
		public abstract class invitation : IBqlField { }
		
		[PXDBInt]
		[PXUIField(DisplayName = "Invitation", Enabled = false)]
		[PXDefault(PXInvitationStatusAttribute.NOTINVITED)]
		[PXInvitationStatus]
		public virtual int? Invitation { get; set; }
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
