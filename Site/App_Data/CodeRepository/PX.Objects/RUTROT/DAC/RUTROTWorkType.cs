using System;
using PX.Data;

namespace PX.Objects.RUTROT
{
	[Serializable]
	public class RUTROTWorkType : IBqlTable
	{
		#region RUTROTType
		public abstract class rUTROTType : IBqlField { }

		/// <summary>
		/// The type of deduction for a ROT and RUT deductible document.
		/// </summary>
		/// <value>
		/// Allowed values are:
		/// <c>"U"</c> - RUT,
		/// <c>"O"</c> - ROT.
		/// Defaults to RUT (<c>"U"</c>).
		/// </value>
		[PXDBString(1)]
		[RUTROTTypes.List]
		[PXDefault(RUTROTTypes.RUT)]
		[PXUIField(DisplayName = "Deduction Type", Enabled = true, Visible = true, Visibility = PXUIVisibility.Visible, FieldClass = RUTROTMessages.FieldClass)]
		public virtual string RUTROTType { get; set; }
		#endregion
		#region WorkTypeID
		public abstract class workTypeID : PX.Data.IBqlField
		{
		}

		/// <summary>
		/// Database identity.
		/// The unique identifier of the RUTROTWorkType.
		/// </summary>
		[PXDBIdentity(IsKey = true)]
		[PXUIField(DisplayName = "Work Type ID", Visibility = PXUIVisibility.Invisible, Visible = false, FieldClass = RUTROTMessages.FieldClass)]
		public virtual Int32? WorkTypeID
		{
			get;
			set;
		}
		#endregion
		#region Description
		public abstract class description : PX.Data.IBqlField
		{
		}

		/// <summary>
		/// The user-friendly description of the Work Type.
		/// </summary>
		[PXDBString(255, IsUnicode = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Title", Enabled = true, Visible = true, Visibility = PXUIVisibility.Visible, FieldClass = RUTROTMessages.FieldClass)]
		[PX.Data.EP.PXFieldDescription]
		public virtual String Description
		{
			get;
			set;
		}
		#endregion
		#region XMLTag
		public abstract class xmlTag : PX.Data.IBqlField
		{
		}

		/// <summary>
		/// The xml tag that will be included to the exported xml file.
		/// </summary>
		[PXDBString(50, IsUnicode = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "XML Tag", Enabled = true, Visible = true, Visibility = PXUIVisibility.Visible, FieldClass = RUTROTMessages.FieldClass)]
		public virtual String XMLTag
		{
			get;
			set;
		}
		#endregion

		#region StartDate
		public abstract class startDate : PX.Data.IBqlField
		{
		}
		[PXDBDate()]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Start Date", Enabled = true, Visible = true, Visibility = PXUIVisibility.Visible, FieldClass = RUTROTMessages.FieldClass)]
		public virtual DateTime? StartDate
		{
			get;
			set;
		}
		#endregion
		#region EndDate
		public abstract class endDate : PX.Data.IBqlField
		{
		}
		[PXDBDate()]
		[PXUIField(DisplayName = "End Date", Enabled = true, Visible = true, Visibility = PXUIVisibility.Visible, FieldClass = RUTROTMessages.FieldClass)]
		public virtual DateTime? EndDate
		{
			get;
			set;
		}
		#endregion
		#region Position
		public abstract class position : PX.Data.IBqlField
		{
		}

		/// <summary>
		/// 
		/// </summary>
		[PXDBInt()]
		[PXCheckUnique(IgnoreNulls = true)]
		[PXDefault]
		[PXUIField(DisplayName = "Position", Visibility = PXUIVisibility.Invisible, Visible = false, FieldClass = RUTROTMessages.FieldClass)]
		public virtual Int32? Position
		{
			get;
			set;
		}
		#endregion

		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}

		/// <summary>
		/// Identifier of the <see cref="PX.Data.Note">Note</see> object, associated with the item.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="PX.Data.Note.NoteID">Note.NoteID</see> field. 
		/// </value>
		[PXNote]
		public virtual Guid? NoteID
		{
			get;
			set;
		}
		#endregion

		#region CreatedByID
		public abstract class createdByID : PX.Data.IBqlField
		{
		}
		[PXDBCreatedByID()]
		public virtual Guid? CreatedByID
		{
			get;
			set;
		}
		#endregion
		#region CreatedByScreenID
		public abstract class createdByScreenID : PX.Data.IBqlField
		{
		}
		[PXDBCreatedByScreenID()]
		public virtual String CreatedByScreenID
		{
			get;
			set;
		}
		#endregion
		#region CreatedDateTime
		public abstract class createdDateTime : PX.Data.IBqlField
		{
		}
		[PXDBCreatedDateTime()]
		public virtual DateTime? CreatedDateTime
		{
			get;
			set;
		}
		#endregion
		#region LastModifiedByID
		public abstract class lastModifiedByID : PX.Data.IBqlField
		{
		}
		[PXDBLastModifiedByID()]
		public virtual Guid? LastModifiedByID
		{
			get;
			set;
		}
		#endregion
		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : PX.Data.IBqlField
		{
		}
		[PXDBLastModifiedByScreenID()]
		public virtual String LastModifiedByScreenID
		{
			get;
			set;
		}
		#endregion
		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : PX.Data.IBqlField
		{
		}
		[PXDBLastModifiedDateTime()]
		public virtual DateTime? LastModifiedDateTime
		{
			get;
			set;
		}
		#endregion
		#region tstamp
		public abstract class Tstamp : PX.Data.IBqlField
		{
		}
		[PXDBTimestamp()]
		public virtual Byte[] tstamp
		{
			get;
			set;
		}
		#endregion
	}
}
