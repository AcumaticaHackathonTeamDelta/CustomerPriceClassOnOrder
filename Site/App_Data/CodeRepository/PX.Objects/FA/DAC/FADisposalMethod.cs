using System;
using PX.Data;
using PX.Objects.GL;

namespace PX.Objects.FA
{
	[Serializable]
	[PXCacheName(Messages.FADisposalMethod)]
	public partial class FADisposalMethod : IBqlTable
	{
		#region DisposalMethodID
		public abstract class disposalMethodID : IBqlField
		{
		}
		protected Int32? _DisposalMethodID;
		[PXDBIdentity]
        [PXUIField(Visibility = PXUIVisibility.Invisible, Visible = false)]
        public virtual Int32? DisposalMethodID
		{
			get
			{
				return _DisposalMethodID;
			}
			set
			{
				_DisposalMethodID = value;
			}
		}
		#endregion
		#region DisposalMethodCD
		public abstract class disposalMethodCD : IBqlField
		{
		}
		protected String _DisposalMethodCD;
		[PXDBString(10, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCC")]
		[PXUIField(DisplayName = "Disposal Method ID", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String DisposalMethodCD
		{
			get
			{
				return _DisposalMethodCD;
			}
			set
			{
				_DisposalMethodCD = value;
			}
		}
		#endregion
		#region Description
		public abstract class description : IBqlField
		{
		}
		protected String _Description;
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String Description
		{
			get
			{
				return _Description;
			}
			set
			{
				_Description = value;
			}
		}
		#endregion
		#region ProceedsAcctID
		public abstract class proceedsAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _ProceedsAcctID;
		[Account(null,
			DisplayName = "Proceeds Account",
			DescriptionField = typeof(Account.description))]
        [PXDefault(typeof(FASetup.proceedsAcctID), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Int32? ProceedsAcctID
		{
			get
			{
				return this._ProceedsAcctID;
			}
			set
			{
				this._ProceedsAcctID = value;
			}
		}
		#endregion
		#region ProceedsSubID
		public abstract class proceedsSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _ProceedsSubID;
		[SubAccount(typeof(proceedsAcctID),
			DescriptionField = typeof(Sub.description),
			DisplayName = "Proceeds Subaccount")]
        [PXDefault(typeof(FASetup.proceedsSubID), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Int32? ProceedsSubID
		{
			get
			{
				return this._ProceedsSubID;
			}
			set
			{
				this._ProceedsSubID = value;
			}
		}
		#endregion

		#region tstamp
		public abstract class Tstamp : IBqlField
		{
		}
		protected Byte[] _tstamp;
		[PXDBTimestamp]
		public virtual Byte[] tstamp
		{
			get
			{
				return _tstamp;
			}
			set
			{
				_tstamp = value;
			}
		}
		#endregion
		#region CreatedByID
		public abstract class createdByID : IBqlField
		{
		}
		protected Guid? _CreatedByID;
		[PXDBCreatedByID]
		public virtual Guid? CreatedByID
		{
			get
			{
				return _CreatedByID;
			}
			set
			{
				_CreatedByID = value;
			}
		}
		#endregion
		#region CreatedByScreenID
		public abstract class createdByScreenID : IBqlField
		{
		}
		protected String _CreatedByScreenID;
		[PXDBCreatedByScreenID]
		public virtual String CreatedByScreenID
		{
			get
			{
				return _CreatedByScreenID;
			}
			set
			{
				_CreatedByScreenID = value;
			}
		}
		#endregion
		#region CreatedDateTime
		public abstract class createdDateTime : IBqlField
		{
		}
		protected DateTime? _CreatedDateTime;
		[PXDBCreatedDateTime]
		public virtual DateTime? CreatedDateTime
		{
			get
			{
				return _CreatedDateTime;
			}
			set
			{
				_CreatedDateTime = value;
			}
		}
		#endregion
		#region LastModifiedByID
		public abstract class lastModifiedByID : IBqlField
		{
		}
		protected Guid? _LastModifiedByID;
		[PXDBLastModifiedByID]
		public virtual Guid? LastModifiedByID
		{
			get
			{
				return _LastModifiedByID;
			}
			set
			{
				_LastModifiedByID = value;
			}
		}
		#endregion
		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : IBqlField
		{
		}
		protected String _LastModifiedByScreenID;
		[PXDBLastModifiedByScreenID]
		public virtual String LastModifiedByScreenID
		{
			get
			{
				return _LastModifiedByScreenID;
			}
			set
			{
				_LastModifiedByScreenID = value;
			}
		}
		#endregion
		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : IBqlField
		{
		}
		protected DateTime? _LastModifiedDateTime;
		[PXDBLastModifiedDateTime]
		public virtual DateTime? LastModifiedDateTime
		{
			get
			{
				return _LastModifiedDateTime;
			}
			set
			{
				_LastModifiedDateTime = value;
			}
		}
		#endregion
	}
}
