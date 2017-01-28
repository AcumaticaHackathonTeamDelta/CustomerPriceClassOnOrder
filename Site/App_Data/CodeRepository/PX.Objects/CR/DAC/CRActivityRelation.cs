using System;
using PX.Data;

namespace PX.Objects.CR
{
    [Serializable]
    [PXHidden]
	[PXCacheName(Messages.ActivityRelation)]
	public partial class CRActivityRelation : IBqlTable
	{
		#region RecordID

		public abstract class recordID : IBqlField { }

		[PXDBIdentity(IsKey = true)]
		[PXUIField(Visible = false)]
		public virtual int? RecordID { get; set; }

		#endregion

		#region NoteID

		public abstract class noteID : IBqlField { }

		[PXDBGuid]
		[PXUIField(Visible = false)]
		[PXDBDefault(typeof(CRActivity.noteID))]
		public virtual Guid? NoteID { get; set; }

		#endregion

		#region RefNoteID

		public abstract class refNoteID : IBqlField { }

		[PXDBGuid]
		[PXUIField(DisplayName = "Task ID")]
        [PXDBDefault(typeof(CRActivity.noteID))]
		[RefTaskSelector(typeof(CRActivity.noteID))]
		public virtual Guid? RefNoteID { get; set; }

		#endregion

		#region Subject

		public abstract class subject : IBqlField { }

		[PXString]
		[PXUIField(DisplayName = "Subject", Enabled = false)]
		public virtual string Subject { get; set; }

		#endregion

		#region StartDate
		public abstract class startDate : IBqlField { }

		[PXDate(InputMask = "g", DisplayMask = "g")]
		[PXUIField(DisplayName = "Start Date", Enabled = false)]
		public virtual DateTime? StartDate { get; set; }
		#endregion

		#region EndDate
		public abstract class endDate : IBqlField { }

		[PXDate(InputMask = "g", DisplayMask = "g")]
		[PXUIField(DisplayName = "Due Date", Enabled = false)]
		public virtual DateTime? EndDate { get; set; }
		#endregion

		#region CompletedDateTime
		public abstract class completedDateTime : IBqlField { }

		[PXDate(InputMask = "g", DisplayMask = "g")]
		[PXUIField(DisplayName = "Completed At", Enabled = false)]
		public virtual DateTime? CompletedDateTime { get; set; }
		#endregion

		#region Status
		public abstract class status : IBqlField { }

		[PXString]
		[ActivityStatusList]
		[PXUIField(DisplayName = "Status", Enabled = false)]
		public virtual string Status { get; set; }
		#endregion


		#region CreatedByID
		public abstract class createdByID : IBqlField
		{
		}
		[PXDBCreatedByID()]
		[PXUIField(DisplayName = "Created By")]
		public virtual Guid? CreatedByID { get; set; }
		#endregion
		#region CreatedByScreenID
		public abstract class createdByScreenID : IBqlField
		{
		}
		[PXDBCreatedByScreenID()]
		public virtual string CreatedByScreenID { get; set; }
		#endregion
		#region CreatedDateTime
		public abstract class createdDateTime : IBqlField
		{
		}
		[PXDBCreatedDateTime()]
		[PXUIField(DisplayName = "Created Date")]
		public virtual DateTime? CreatedDateTime { get; set; }
		#endregion
		#region LastModifiedByID
		public abstract class lastModifiedByID : IBqlField
		{
		}
		[PXDBLastModifiedByID()]
		[PXUIField(DisplayName = "Last Modified By")]
		public virtual Guid? LastModifiedByID { get; set; }
		#endregion
		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : IBqlField
		{
		}
		[PXDBLastModifiedByScreenID()]
		public virtual string LastModifiedByScreenID { get; set; }
		#endregion
		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : IBqlField
		{
		}
		[PXDBLastModifiedDateTime()]
		[PXUIField(DisplayName = "Last Modified Date")]
		public virtual DateTime? LastModifiedDateTime { get; set; }
		#endregion
		#region tstamp
		public abstract class Tstamp : IBqlField
		{
		}
		[PXDBTimestamp()]
		public virtual Byte[] tstamp { get; set; }
		#endregion
	}
}
