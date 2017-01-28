using System;
using PX.Data;

namespace PX.Objects.CR
{
	[Serializable]
	[PXCacheName(Messages.RecordForMerge)]
	[PXHidden]
	public partial class CRMergeResult : IBqlTable
	{
		#region Selected
		public abstract class selected : IBqlField { }

		[PXBool]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Selected")]
		public bool? Selected { get; set; }
		#endregion

		#region MergeID

		public abstract class mergeID : IBqlField { }

		[PXDBInt(IsKey = true)]
		[PXDBLiteDefault(typeof(CRMerge.mergeID))]
		[PXUIField(Visible = false)]
		public virtual Int32? MergeID { get; set; }

		#endregion

		#region LineNbr

		public abstract class lineNbr : IBqlField { }

		[PXDBInt(IsKey = true)]
		[PXUIField(Visible = false)]
		[RowNbr]
		public virtual Int32? LineNbr { get; set; }

		#endregion

		#region Parameters

		public abstract class parameters : IBqlField { }

		[PXDBBinary]
		[PXUIField(Visible = false)]
		public virtual byte[] Parameters { get; set; }

		#endregion

		#region ParamValues

		public abstract class paramValues : IBqlField { }

		[PXDBBinary]
		[PXUIField(Visible = false)]
		public virtual byte[] ParamValues { get; set; }

		#endregion

		#region EstCount

		public abstract class estCount : IBqlField { }

		[PXInt]
		[PXUIField(DisplayName = "Count", Enabled = false)]
		public virtual Int32? EstCount { get; set; }

		#endregion

		#region ShortDescr

		public abstract class shortDescr : IBqlField { }

		[PXString(IsUnicode = true)]
		[PXUIField(DisplayName = "Summary", Enabled = false)]
		public virtual String ShortDescr { get; set; }

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
		[PXDBCreatedDateTimeUtc]
		[PXUIField(DisplayName = "Date Reported", Enabled = false)]
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
		[PXDBLastModifiedDateTimeUtc]
		[PXUIField(DisplayName = "Last Activity", Enabled = false)]
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
