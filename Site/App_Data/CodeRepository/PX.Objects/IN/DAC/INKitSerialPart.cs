namespace PX.Objects.IN
{
	using System;
	using PX.Data;

	[System.SerializableAttribute()]
	public partial class INKitSerialPart : PX.Data.IBqlTable
	{
		#region DocType
		public abstract class docType : PX.Data.IBqlField
		{
		}
		protected String _DocType;
		[PXDBString(1, IsFixed = true, IsKey = true)]
		[PXDefault(typeof(INRegister.docType))]
		public virtual String DocType
		{
			get
			{
				return this._DocType;
			}
			set
			{
				this._DocType = value;
			}
		}
		#endregion
		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
		}
		protected String _RefNbr;
		[PXDBString(15, IsUnicode = true, IsKey = true)]
		[PXDBDefault(typeof(INRegister.refNbr))]
		[PXParent(typeof(Select<INRegister, Where<INRegister.docType, Equal<Current<INKitSerialPart.docType>>, And<INRegister.refNbr, Equal<Current<INKitSerialPart.refNbr>>>>>))]
		public virtual String RefNbr
		{
			get
			{
				return this._RefNbr;
			}
			set
			{
				this._RefNbr = value;
			}
		}
		#endregion
		#region KitLineNbr
		public abstract class kitLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _KitLineNbr;
		[PXDBInt(IsKey = true)]
		public virtual Int32? KitLineNbr
		{
			get
			{
				return this._KitLineNbr;
			}
			set
			{
				this._KitLineNbr = value;
			}
		}
		#endregion
		#region KitSplitLineNbr
		public abstract class kitSplitLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _KitSplitLineNbr;
		[PXDBInt(IsKey=true)]
		public virtual Int32? KitSplitLineNbr
		{
			get
			{
				return this._KitSplitLineNbr;
			}
			set
			{
				this._KitSplitLineNbr = value;
			}
		}
		#endregion
		#region PartLineNbr
		public abstract class partLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _PartLineNbr;
		[PXDBInt(IsKey = true)]
		public virtual Int32? PartLineNbr
		{
			get
			{
				return this._PartLineNbr;
			}
			set
			{
				this._PartLineNbr = value;
			}
		}
		#endregion
		#region PartSplitLineNbr
		public abstract class partSplitLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _PartSplitLineNbr;
		[PXDBInt(IsKey = true)]
		public virtual Int32? PartSplitLineNbr
		{
			get
			{
				return this._PartSplitLineNbr;
			}
			set
			{
				this._PartSplitLineNbr = value;
			}
		}
		#endregion

		#region System Columns
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
		#endregion
	}
}
