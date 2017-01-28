namespace PX.Objects.GL
{
	using System;
	using PX.Data;
	
    /// <summary>
    /// Represents financial period setup records used to define the templates for actual <see cref="FinPeriod">Financial Periods</see>.
    /// The records of this type are edited through the Financial Year (GL.10.10.00) screen.
    /// </summary>
	[System.SerializableAttribute()]
	[PXCacheName(Messages.FinPeriodSetup)]
	public partial class FinPeriodSetup : PX.Data.IBqlTable, IPeriodSetup
	{
		#region PeriodNbr
		public abstract class periodNbr : PX.Data.IBqlField
		{
		}
		protected String _PeriodNbr;

        /// <summary>
        /// The number of the period in a year.
        /// </summary>
        /// <value>
        /// Used to determine the <see cref="FinPeriod.PeriodNbr"/> field when the real period is created from the template.
        /// </value>
		[PXDBString(2, IsKey = true, IsFixed = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Period Nbr.", Enabled=false)]
		[PXParent(typeof(Select<FinYearSetup>))]
		public virtual String PeriodNbr
		{
			get
			{
				return this._PeriodNbr;
			}
			set
			{
				this._PeriodNbr = value;
			}
		}
		#endregion
		#region StartDate
		public abstract class startDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _StartDate;

        /// <summary>
        /// The start date of the period (inclusive).
        /// </summary>
        /// <value>
        /// Used to determine the <see cref="FinPeriod.StartDate"/> field when the real period is created from the template.
        /// </value>
		[PXDBDate()]
		[PXDefault()]
		[PXUIField(DisplayName = "Start Date", Enabled =false)]
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

        /// <summary>
        /// The end date of the period (exclusive).
        /// </summary>
        /// <value>
        /// Used to determine the <see cref="FinPeriod.EndDate"/> field when the real period is created from the template.
        /// </value>
		[PXDBDate()]
		[PXDefault()]
		[PXUIField(DisplayName = "End Date", Enabled=false)]
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
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;

        /// <summary>
        /// The description of the period.
        /// </summary>
        /// <value>
        /// Used to determine the <see cref="FinPeriod.Descr"/> field when the real period is created from the template.
        /// </value>
		[PXDBLocalizableString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description")]
		public virtual String Descr
		{
			get
			{
				return this._Descr;
			}
			set
			{
				this._Descr = value;
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
		#region NoteID
		public abstract class noteID : IBqlField { }

		[PXNote]
		public virtual Guid? NoteID { get; set; }
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
		#region EndDateUI
		public abstract class endDateUI : PX.Data.IBqlField
		{
		}

        /// <summary>
        /// The field used to display and edit the <see cref="EndDate"/> of the template period (inclusive) in the UI.
        /// </summary>
        /// <value>
        /// Depends on and changes the value of the <see cref="EndDate"/> field, performing additional transformations.
        /// </value>
		[PXDate()]
		[PXUIField(DisplayName = "End Date", Visibility = PXUIVisibility.Visible, Visible = true, Enabled = false)]
		public virtual DateTime? EndDateUI
		{
			[PXDependsOnFields(typeof(endDate),typeof(startDate))]
			get
			{
				bool isEmpty = (this.EndDate.HasValue && this.EndDate.HasValue && this.StartDate == this.EndDate);
				return ((this._EndDate.HasValue && !isEmpty) ? this._EndDate.Value.AddDays(-1): this._EndDate);			
				//return this._EndDate.HasValue ? this._EndDate.Value.AddDays(-1) : this._EndDate;
			}
			set
			{
				bool isEmpty = (this.EndDate.HasValue && this.StartDate.HasValue && this.StartDate == this.EndDate);
				this._EndDate = (value.HasValue && !isEmpty) ? value.Value.AddDays(1) : value;				
			}
		}
		#endregion
		#region Custom
		public abstract class custom : PX.Data.IBqlField
		{
		}
		protected Boolean? _Custom;

        /// <summary>
        /// Indicates whether the start and end dates of the Financial Period are defined by user.
        /// </summary>
		public virtual Boolean? Custom
		{
			get
			{
				return this._Custom;
			}
			set
			{
				this._Custom = value;
			}
		}
		#endregion
	}
}
