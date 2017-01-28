namespace PX.Objects.AR
{
	using System;
	using PX.Data;

	public class ARSPCommissionPeriodStatus
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Prepared, Open, Closed },
				new string[] { Messages.PeriodPrepared, Messages.PeriodOpen, Messages.PeriodClosed }) { ;}
		}

		public const string Prepared = "P";
		public const string Open = "N";
		public const string Closed = "C";

		public class prepared : Constant<string>
		{
			public prepared() : base(Prepared) { ;}
		}

		public class open : Constant<string>
		{
			public open() : base(Open) { ;}
		}

		public class closed : Constant<string>
		{
			public closed() : base(Closed) { ;}
		}
	}

	[System.SerializableAttribute()]
	[PXCacheName(Messages.ARSPCommissionPeriod)]
	public partial class ARSPCommissionPeriod : PX.Data.IBqlTable
	{
		#region CommnPeriodID
		public abstract class commnPeriodID : PX.Data.IBqlField
		{
		}
		protected String _CommnPeriodID;
		[PXDefault()]
		[GL.FinPeriodID(IsKey=true)]
		[PXUIField(DisplayName = "Commission Period", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(ARSPCommissionPeriod.commnPeriodID))]
		public virtual String CommnPeriodID
		{
			get
			{
				return this._CommnPeriodID;
			}
			set
			{
				this._CommnPeriodID = value;
			}
		}
		#endregion
		#region Year
		public abstract class year : PX.Data.IBqlField
		{
		}
		protected String _Year;
		[PXDBString(4, IsFixed = true)]
		[PXDefault()]
		public virtual String Year
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
		#region StartDate
		public abstract class startDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _StartDate;
		[PXDBDate()]
		[PXDefault()]
		[PXUIField(DisplayName = "From", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual DateTime? StartDate
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
		protected DateTime? _EndDate;
		[PXDBDate()]
		[PXDefault()]
		//[PXUIField(DisplayName = "To", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual DateTime? EndDate
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
		}
		protected String _Status;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(ARSPCommissionPeriodStatus.Open)]
		[ARSPCommissionPeriodStatus.List()]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
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
		#region Filed
		public abstract class filed : PX.Data.IBqlField
		{
		}
		protected Boolean? _Filed;
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? Filed
		{
			get
			{
				return this._Filed;
			}
			set
			{
				this._Filed = value;
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
		#region EndDateUI
		public abstract class endDateUI : PX.Data.IBqlField
		{
		}

		[PXDate()]
		[PXUIField(DisplayName = "To", Visibility = PXUIVisibility.Visible, Visible = true, Enabled = false)]
		public virtual DateTime? EndDateUI
		{
			[PXDependsOnFields(typeof(endDate),typeof(startDate))]
			get
			{
				bool isEmpty = (this.EndDate.HasValue && this.EndDate.HasValue && this.StartDate == this.EndDate);
				return ((this._EndDate.HasValue && !isEmpty) ? this._EndDate.Value.AddDays(-1) : this._EndDate);			
			}
			set
			{
				bool isEmpty = (value.HasValue && this.StartDate.HasValue && this.StartDate == value);
				this._EndDate = (value.HasValue && !isEmpty) ? value.Value.AddDays(1) : value;		
			}
		}
		#endregion
	}
}
