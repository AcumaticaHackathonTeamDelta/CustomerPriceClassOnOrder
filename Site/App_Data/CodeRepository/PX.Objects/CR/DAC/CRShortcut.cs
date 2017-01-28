using System;
using PX.Data;
using PX.SM;

namespace PX.Objects.CR
{
	[Serializable]
    [PXHidden]
	[Obsolete("Will be removed in 7.0 version")]
	public partial class CRShortcut : IBqlTable
	{
		#region Command

		public abstract class command : IBqlField
		{
		}

		[PXDBString(IsKey = true, IsUnicode = true)]
		[PXUIField(DisplayName = "Command")]
		public virtual String Command { get; set; }

		#endregion

		#region Ctrl

		public abstract class ctrl : IBqlField
		{
		}

		[PXDBBool]
		[PXUIField(DisplayName = "Ctrl")]
		public virtual Boolean? Ctrl { get; set; }

		#endregion

		#region Alt

		public abstract class alt : IBqlField
		{
		}

		[PXDBBool]
		[PXUIField(DisplayName = "Alt")]
		public virtual Boolean? Alt { get; set; }

		#endregion

		#region Shift

		public abstract class shift : IBqlField
		{
		}

		[PXDBBool]
		[PXUIField(DisplayName = "Shift")]
		public virtual Boolean? Shift { get; set; }

		#endregion

		#region Shortcut

		public abstract class shortcut : IBqlField
		{
		}

		[PXDBString(10)]
		[PXUIField(DisplayName = "Char Codes")]
		public virtual String CharCodes { get; set; }

		#endregion


		#region KeysDescription

		public abstract class keysDescription : IBqlField
		{
		}

		[PXString]
		[PXUIField(DisplayName = "Shortcuts")]
		public virtual String KeysDescription { get; set; }

		#endregion

		#region DisplayName

		public abstract class displayName : IBqlField
		{
		}

		[PXString]
		[PXUIField(DisplayName = "Command", IsReadOnly = true)]
		public virtual string DisplayName { get; set; }

		#endregion


		#region CreatedByID
		public abstract class createdByID : PX.Data.IBqlField
		{
		}
		protected Guid? _CreatedByID;
		[PXDBCreatedByID()]
		[PXUIField(DisplayName = "Created By")]
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
		[PXUIField(DisplayName = "Created Date")]
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
		[PXUIField(DisplayName = "Last Modified By")]
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
		[PXUIField(DisplayName = "Last Modified Date")]
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
	}
}
