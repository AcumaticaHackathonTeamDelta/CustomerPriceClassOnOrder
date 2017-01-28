namespace PX.Objects.IN
{
	using System;
	using PX.Data;
	using PX.Objects.GL;
	
	[System.SerializableAttribute()]
	public partial class INLotSerSegment : PX.Data.IBqlTable
	{
		#region LotSerClassID
		public abstract class lotSerClassID : PX.Data.IBqlField
		{
		}
		protected String _LotSerClassID;
		[PXDBString(10, IsUnicode = true, IsKey = true)]
		[PXDefault(typeof(INLotSerClass.lotSerClassID))]
		[PXParent(typeof(Select<INLotSerClass, Where<INLotSerClass.lotSerClassID,Equal<Current<INLotSerSegment.lotSerClassID>>>>))]
		public virtual String LotSerClassID
		{
			get
			{
				return this._LotSerClassID;
			}
			set
			{
				this._LotSerClassID = value;
			}
		}
		#endregion
		#region SegmentID
		public abstract class segmentID : PX.Data.IBqlField
		{
		}
		protected Int16? _SegmentID;
		[PXDBShort(IsKey = true)]
		[PXUIField(DisplayName="Segment Number", Enabled=false)]
		[PXLineNbr(typeof(INLotSerClass))]
		[PXDefault()]
		public virtual Int16? SegmentID
		{
			get
			{
				return this._SegmentID;
			}
			set
			{
				this._SegmentID = value;
			}
		}
		#endregion
		#region SegmentType
		public abstract class segmentType : PX.Data.IBqlField
		{
		}
		protected String _SegmentType;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(INLotSerSegmentType.FixedConst)]
		[INLotSerSegmentType.List()]
		[PXUIField(DisplayName="Type")]
		public virtual String SegmentType
		{
			get
			{
				return this._SegmentType;
			}
			set
			{
				this._SegmentType = value;
			}
		}
		#endregion
		#region SegmentValue
		public abstract class segmentValue : PX.Data.IBqlField
		{
		}
		protected String _SegmentValue;
		[PXDBString(30, IsUnicode = true)]
		[PXUIField(DisplayName="Value")]
		public virtual String SegmentValue
		{
			get
			{
				return this._SegmentValue;
			}
			set
			{
				this._SegmentValue = value;
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

	public class INLotSerSegmentType
	{ 
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { NumericVal, FixedConst, DayConst, MonthConst, MonthLongConst, YearConst, YearLongConst, DateConst },
				new string[] { Messages.NumericVal, Messages.FixedConst, Messages.DayConst, Messages.MonthConst, Messages.MonthLongConst, Messages.YearConst, Messages.YearLongConst, Messages.DateConst, }) { }
		}

		public const string NumericVal = "N";
		public const string FixedConst = "C";
		public const string DateConst  = "D";
		public const string DayConst = "U";
		public const string MonthConst = "M";
		public const string MonthLongConst = "A";
		public const string YearConst = "Y";
		public const string YearLongConst = "L";
	
	}
}
