using System;
using PX.Data;
using PX.Objects.CS;

namespace PX.Objects.FA
{
	[Serializable]
	[PXCacheName(Messages.FADepreciationMethodLines)]
	public partial class FADepreciationMethodLines : PX.Data.IBqlTable
	{
		#region MethodID
		public abstract class methodID : PX.Data.IBqlField
		{
		}
		protected Int32? _MethodID;
		[PXDBInt(IsKey = true)]
		[PXDBLiteDefault(typeof(FADepreciationMethod.methodID))]
		[PXParent(typeof(Select<FADepreciationMethod, Where<FADepreciationMethod.methodID, Equal<Current<FADepreciationMethodLines.methodID>>>>), UseCurrent = true, LeaveChildren = false)]
		[PXUIField(Visible = false, Visibility = PXUIVisibility.Invisible)]
		public virtual Int32? MethodID
		{
			get
			{
				return this._MethodID;
			}
			set
			{
				this._MethodID = value;
			}
		}
		#endregion
		#region Year
		public abstract class year : PX.Data.IBqlField
		{
		}
		protected Int32? _Year;
		[PXDBInt(IsKey = true, MaxValue = 500, MinValue = 0)]
		[PXUIField(DisplayName = "Recovery Year", Enabled = false)]
		public virtual Int32? Year
		{
			get
			{
				return this._Year;
			}
			set
			{
				this._Year = value;
			}
		}
		#endregion
		#region DisplayRatioPerYear
		public abstract class displayRatioPerYear : IBqlField {}
		[PXDecimal(3, MinValue = 0, MaxValue = 100)]
		[PXFormula(typeof(Mult<Current<ratioPerYear>, decimal100>))]
		[PXUIField(DisplayName = "Percent per Year")]
		public virtual decimal? DisplayRatioPerYear { get; set; }
		#endregion
		#region RatioPerYear
		public abstract class ratioPerYear : PX.Data.IBqlField
		{
		}
		protected Decimal? _RatioPerYear;
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBDecimal(5)]
        [PXFormula(typeof(Div<displayRatioPerYear, decimal100>), typeof(SumCalc<FADepreciationMethod.totalPercents>))]
		public virtual Decimal? RatioPerYear
		{
			[PXDependsOnFields(typeof(setYearRatioByUser),typeof(ratioPerPeriod1),typeof(ratioPerPeriod2),typeof(ratioPerPeriod3),typeof(ratioPerPeriod4),typeof(ratioPerPeriod5),typeof(ratioPerPeriod6),typeof(ratioPerPeriod7),typeof(ratioPerPeriod8),typeof(ratioPerPeriod9),typeof(ratioPerPeriod10),typeof(ratioPerPeriod11),typeof(ratioPerPeriod12))]
			get
			{
				if (this._SetYearRatioByUser != true)
				{
					this._RatioPerYear = (this.RatioPerPeriod1 ?? 0m) + (this.RatioPerPeriod2 ?? 0m) + (this.RatioPerPeriod3 ?? 0m) + (this.RatioPerPeriod4  ?? 0m) + (this.RatioPerPeriod5  ?? 0m) + (this.RatioPerPeriod6  ?? 0m) + 
										   (this.RatioPerPeriod7 ?? 0m) + (this.RatioPerPeriod8 ?? 0m) + (this.RatioPerPeriod9 ?? 0m) + (this.RatioPerPeriod10 ?? 0m) + (this.RatioPerPeriod11 ?? 0m) + (this.RatioPerPeriod12 ?? 0m);
				}
				return this._RatioPerYear;
			}
			set
			{
				this._RatioPerYear = value;
			}
		}
		#endregion
		#region RatioPerPeriod1
		public abstract class ratioPerPeriod1 : PX.Data.IBqlField
		{
		}
		protected Decimal? _RatioPerPeriod1;
		[PercentDBDecimal]
		[PXUIField(DisplayName = "Percent per Period 1")]
		public virtual Decimal? RatioPerPeriod1
		{
			get
			{
				return this._RatioPerPeriod1;
			}
			set
			{
				this._RatioPerPeriod1 = value;
			}
		}
		#endregion
		#region RatioPerPeriod2
		public abstract class ratioPerPeriod2 : PX.Data.IBqlField
		{
		}
		protected Decimal? _RatioPerPeriod2;
		[PercentDBDecimal]
		[PXUIField(DisplayName = "Percent per Period 2")]
		public virtual Decimal? RatioPerPeriod2
		{
			get
			{
				return this._RatioPerPeriod2;
			}
			set
			{
				this._RatioPerPeriod2 = value;
			}
		}
		#endregion
		#region RatioPerPeriod3
		public abstract class ratioPerPeriod3 : PX.Data.IBqlField
		{
		}
		protected Decimal? _RatioPerPeriod3;
		[PercentDBDecimal]
		[PXUIField(DisplayName = "Percent per Period 3")]
		public virtual Decimal? RatioPerPeriod3
		{
			get
			{
				return this._RatioPerPeriod3;
			}
			set
			{
				this._RatioPerPeriod3 = value;
			}
		}
		#endregion
		#region RatioPerPeriod4
		public abstract class ratioPerPeriod4 : PX.Data.IBqlField
		{
		}
		protected Decimal? _RatioPerPeriod4;
		[PercentDBDecimal]
		[PXUIField(DisplayName = "Percent per Period 4")]
		public virtual Decimal? RatioPerPeriod4
		{
			get
			{
				return this._RatioPerPeriod4;
			}
			set
			{
				this._RatioPerPeriod4 = value;
			}
		}
		#endregion
		#region RatioPerPeriod5
		public abstract class ratioPerPeriod5 : PX.Data.IBqlField
		{
		}
		protected Decimal? _RatioPerPeriod5;
		[PercentDBDecimal]
		[PXUIField(DisplayName = "Percent per Period 5")]
		public virtual Decimal? RatioPerPeriod5
		{
			get
			{
				return this._RatioPerPeriod5;
			}
			set
			{
				this._RatioPerPeriod5 = value;
			}
		}
		#endregion
		#region RatioPerPeriod6
		public abstract class ratioPerPeriod6 : PX.Data.IBqlField
		{
		}
		protected Decimal? _RatioPerPeriod6;
		[PercentDBDecimal]
		[PXUIField(DisplayName = "Percent per Period 6")]
		public virtual Decimal? RatioPerPeriod6
		{
			get
			{
				return this._RatioPerPeriod6;
			}
			set
			{
				this._RatioPerPeriod6 = value;
			}
		}
		#endregion
		#region RatioPerPeriod7
		public abstract class ratioPerPeriod7 : PX.Data.IBqlField
		{
		}
		protected Decimal? _RatioPerPeriod7;
		[PercentDBDecimal]
		[PXUIField(DisplayName = "Percent per Period 7")]
		public virtual Decimal? RatioPerPeriod7
		{
			get
			{
				return this._RatioPerPeriod7;
			}
			set
			{
				this._RatioPerPeriod7 = value;
			}
		}
		#endregion
		#region RatioPerPeriod8
		public abstract class ratioPerPeriod8 : PX.Data.IBqlField
		{
		}
		protected Decimal? _RatioPerPeriod8;
		[PercentDBDecimal]
		[PXUIField(DisplayName = "Percent per Period 8")]
		public virtual Decimal? RatioPerPeriod8
		{
			get
			{
				return this._RatioPerPeriod8;
			}
			set
			{
				this._RatioPerPeriod8 = value;
			}
		}
		#endregion
		#region RatioPerPeriod9
		public abstract class ratioPerPeriod9 : PX.Data.IBqlField
		{
		}
		protected Decimal? _RatioPerPeriod9;
		[PercentDBDecimal]
		[PXUIField(DisplayName = "Percent per Period 9")]
		public virtual Decimal? RatioPerPeriod9
		{
			get
			{
				return this._RatioPerPeriod9;
			}
			set
			{
				this._RatioPerPeriod9 = value;
			}
		}
		#endregion
		#region RatioPerPeriod10
		public abstract class ratioPerPeriod10 : PX.Data.IBqlField
		{
		}
		protected Decimal? _RatioPerPeriod10;
		[PercentDBDecimal]
		[PXUIField(DisplayName = "Percent per Period 10")]
		public virtual Decimal? RatioPerPeriod10
		{
			get
			{
				return this._RatioPerPeriod10;
			}
			set
			{
				this._RatioPerPeriod10 = value;
			}
		}
		#endregion
		#region RatioPerPeriod11
		public abstract class ratioPerPeriod11 : PX.Data.IBqlField
		{
		}
		protected Decimal? _RatioPerPeriod11;
		[PercentDBDecimal]
		[PXUIField(DisplayName = "Percent per Period 11")]
		public virtual Decimal? RatioPerPeriod11
		{
			get
			{
				return this._RatioPerPeriod11;
			}
			set
			{
				this._RatioPerPeriod11 = value;
			}
		}
		#endregion
		#region RatioPerPeriod12
		public abstract class ratioPerPeriod12 : PX.Data.IBqlField
		{
		}
		protected Decimal? _RatioPerPeriod12;
		[PercentDBDecimal]
		[PXUIField(DisplayName = "Percent per Period 12")]
		public virtual Decimal? RatioPerPeriod12
		{
			get
			{
				return this._RatioPerPeriod12;
			}
			set
			{
				this._RatioPerPeriod12 = value;
			}
		}
		#endregion		
		#region SetYearRatioByUser
		public abstract class setYearRatioByUser : PX.Data.IBqlField
		{
		}
		protected Boolean? _SetYearRatioByUser;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Set Year Ratio", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Boolean? SetYearRatioByUser
		{
			get
			{
				return this._SetYearRatioByUser;
			}
			set
			{
				this._SetYearRatioByUser = value;
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
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXNote()]
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
	}
}
