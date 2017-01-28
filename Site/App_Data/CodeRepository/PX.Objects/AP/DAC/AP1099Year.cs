namespace PX.Objects.AP
{
	using System;
	using PX.Data;
	
	[System.SerializableAttribute()]
	[PXCacheName(Messages.AP1099Year)]
	public partial class AP1099Year : PX.Data.IBqlTable
	{
        private class string0101 : Constant<string>
        {
            public string0101()
                : base("0101")
            {
            }
        }
        private class string1231 : Constant<string>
        {
            public string1231()
                : base("1231")
            {
            }
        }

		#region FinYear
		public abstract class finYear : PX.Data.IBqlField
		{
		}
		protected String _FinYear;
		[PXDBString(4, IsKey = true, IsFixed = true)]
		[PXDefault()]
		[PXUIField(DisplayName="1099 Year", Visibility=PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<AP1099Year.finYear>))]
		public virtual String FinYear
		{
			get
			{
				return this._FinYear;
			}
			set
			{
				this._FinYear = value;
			}
		}
		#endregion
        #region StartDate
        public abstract class startDate : PX.Data.IBqlField
        {
        }
        protected String _StartDate;
        [PXDBCalced(typeof(Add<AP1099Year.finYear, string0101>), typeof(string))]
        public virtual String StartDate
        {
            get
            {
                return this._StartDate;
            }
            set
            {
                this._StartDate = value;
            }
        }
        #endregion
        #region EndDate
        public abstract class endDate : PX.Data.IBqlField
        {
        }
        protected String _EndDate;
        [PXDBCalced(typeof(Add<AP1099Year.finYear, string1231>), typeof(string))]
        public virtual String EndDate
        {
            get
            {
                return this._EndDate;
            }
            set
            {
                this._EndDate = value;
            }
        }
        #endregion
        #region Status
		public abstract class status : PX.Data.IBqlField
		{
			public class ListAttribute : PXStringListAttribute
			{
				public ListAttribute()
					: base(
						new string[] { Open, Closed },
						new string[] { "Open", "Closed" }) { }
			}

			public const string Open = "N";
			public const string Closed = "C";

			public class open : Constant<string>
			{
				public open() : base(Open) { ;}
			}

			public class closed : Constant<string>
			{
				public closed() : base(Closed) { ;}
			}
		}
		protected String _Status;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(status.Open)]
		[status.List()]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled=false)]
		public virtual String Status
		{
			get
			{
				return this._Status;
			}
			set
			{
				this._Status = value;
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
