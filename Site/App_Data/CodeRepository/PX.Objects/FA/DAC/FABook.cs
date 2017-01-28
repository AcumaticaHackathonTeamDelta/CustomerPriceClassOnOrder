using System;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.CM;

namespace PX.Objects.FA
{
	[Serializable]
	[PXPrimaryGraph(typeof(BookMaint))]
	[PXCacheName(Messages.FABook)]
	public partial class FABook : PX.Data.IBqlTable
	{
		#region Selected
		public abstract class selected : IBqlField
		{
		}
		protected bool? _Selected = false;
		[PXBool()]
		[PXUIField(DisplayName = "Selected")]
		public bool? Selected
		{
			get
			{
				return _Selected;
			}
			set
			{
				_Selected = value;
			}
		}
		#endregion
		#region BookID
		public abstract class bookID : PX.Data.IBqlField
		{
		}
		protected Int32? _BookID;
		[PXDBIdentity()]
		public virtual Int32? BookID
		{
			get
			{
				return this._BookID;
			}
			set
			{
				this._BookID = value;
			}
		}
		#endregion
		#region BookCode
		public abstract class bookCode : PX.Data.IBqlField
		{
		}
		protected String _BookCode;
        [PXDefault]
		[PXDBString(10, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCC")]
		[PXUIField(DisplayName = "Book ID", Visibility = PXUIVisibility.SelectorVisible, TabOrder = 0)]
		public virtual String BookCode
		{
			get
			{
				return this._BookCode;
			}
			set
			{
				this._BookCode = value;
			}
		}
		#endregion
		#region Description
		public abstract class description : PX.Data.IBqlField
		{
		}
		protected String _Description;
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible, TabOrder = 1)]
		public virtual String Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				this._Description = value;
			}
		}
		#endregion
		#region UpdateGL
		public abstract class updateGL : PX.Data.IBqlField
		{
		}
		protected Boolean? _UpdateGL;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Update GL")]
		public virtual Boolean? UpdateGL
		{
			get
			{
				return this._UpdateGL;
			}
			set
			{
				this._UpdateGL = value;
			}
		}
		#endregion
		#region MidMonthType
		public abstract class midMonthType : PX.Data.IBqlField
		{
			#region List
			public class ListAttribute : PXStringListAttribute
			{
				public ListAttribute()
					: base(
					new string[] { FixedDay, NumberOfDays },
					new string[] { Messages.FixedDay, Messages.NumberOfDays }) { }
			}

			public const string FixedDay = "F";
			public const string NumberOfDays = "N";
			public const string PeriodDaysHalve = "H";

			public class fixedDay : Constant<string>
			{
				public fixedDay() : base(FixedDay) { ;}
			}
			public class numberOfDays : Constant<string>
			{
				public numberOfDays() : base(NumberOfDays) { ;}
			}
			public class periodDaysHalve : Constant<string>
			{
				public periodDaysHalve() : base(PeriodDaysHalve) { ;}
			}
			#endregion
		}
		protected String _MidMonthType;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(midMonthType.FixedDay)]
		[PXUIField(DisplayName = "Mid-Period Type")]
		[midMonthType.List]
		public virtual String MidMonthType
		{
			get
			{
				return this._MidMonthType;
			}
			set
			{
				this._MidMonthType = value;
			}
		}
		#endregion
		#region MidMonthDay
		public abstract class midMonthDay : PX.Data.IBqlField
		{
		}
		protected Int16? _MidMonthDay;
		[PXDBShort()]
		[PXUIField(DisplayName = "Mid-Period Day")]
		public virtual Int16? MidMonthDay
		{
			get
			{
				return this._MidMonthDay;
			}
			set
			{
				this._MidMonthDay = value;
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

        #region FirstCalendarYear
        public abstract class firstCalendarYear : IBqlField
        {
        }
        protected String _FirstCalendarYear;
        [PXString(4, IsFixed = true)]
        [PXUIField(DisplayName = "First Calendar Year", Enabled = false)]
        [PXDBScalar(typeof(Search4<FABookYear.year, Where<FABookYear.bookID, Equal<FABook.bookID>>, Aggregate<Min<FABookYear.year>>>))]
        public virtual String FirstCalendarYear
        {
            get
            {
                return _FirstCalendarYear;
            }
            set
            {
                _FirstCalendarYear = value;
            }
        }
        #endregion
        #region LastCalendarYear
		public abstract class lastCalendarYear : IBqlField
		{
		}
		protected String _LastCalendarYear;
		[PXString(4, IsFixed = true)]
		[PXUIField(DisplayName = "Last Calendar Year", Enabled = false)]
		[PXDBScalar(typeof(Search4<FABookYear.year, Where<FABookYear.bookID, Equal<FABook.bookID>>, Aggregate<Max<FABookYear.year>>>))]
		public virtual String LastCalendarYear
		{
			get
			{
				return _LastCalendarYear;
			}
			set
			{
				_LastCalendarYear = value;
			}
		}
		#endregion

	}
}
