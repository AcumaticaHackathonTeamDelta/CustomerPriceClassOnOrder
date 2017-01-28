using PX.Data.EP;

namespace PX.Objects.GL
{
	using System;
	using PX.Data;

	/// <summary>
	/// Represents Financial Year.
    /// When the year is created, the values of its fields are determined from the
    /// <see cref="FinYearSetup">Financial Year setup record</see>.
	/// </summary>
	[PXCacheName(Messages.FinancialYear)]
	[System.SerializableAttribute()]
	[PXPrimaryGraph(
		new Type[] { typeof(FiscalPeriodMaint)},
		new Type[] { typeof(Select<FinYear, 
			Where<FinYear.year, Equal<Current<FinYear.year>>>>)
		})]
	public partial class FinYear : PX.Data.IBqlTable, IYear
	{
		#region Year
		public abstract class year : PX.Data.IBqlField
		{
		}
		protected String _Year;

        /// <summary>
        /// Key field.
        /// The financial year.
        /// </summary>
		[PXDBString(4, IsKey = true, IsFixed = true)]
		[PXDefault("")]
		[PXUIField(DisplayName = "Financial Year", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search3<FinYear.year, OrderBy<Desc<FinYear.year>>>))]
		[PXFieldDescription]
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

        /// <summary>
        /// The start date of the year.
        /// </summary>
		[PXDBDate()]
		[PXDefault(TypeCode.DateTime, "01/01/1900")]
		[PXUIField(DisplayName = "Start Date", Enabled = false)]
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
        /// The end date of the year (inclusive).
        /// </summary>
		[PXDBDate()]
		[PXDefault()]
		[PXUIField(DisplayName = "EndDate", Visibility = PXUIVisibility.Visible, Visible = false, Enabled = false)]
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
		#region FinPeriods
		public abstract class finPeriods : PX.Data.IBqlField
		{
		}
		protected Int16? _FinPeriods;

        /// <summary>
        /// The number of periods in the year.
        /// </summary>
		[PXDBShort()]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "Number of Periods", Enabled = false)]
		public virtual Int16? FinPeriods
		{
			get
			{
				return this._FinPeriods;
			}
			set
			{
				this._FinPeriods = value;
			}
		}
		#endregion
		#region CustomPeriods
		public abstract class customPeriods : PX.Data.IBqlField
		{
		}
		protected Boolean? _CustomPeriods;

        /// <summary>
        /// Indicates whether the <see cref="FinPeriod">periods</see> of the year can be modified by user.
        /// </summary>
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "User-Defined Periods")]
		public virtual Boolean? CustomPeriods
		{
			get
			{
				return this._CustomPeriods;
			}
			set
			{
				this._CustomPeriods = value;
			}
		}
		#endregion
		#region BegFinYearHist
		public abstract class begFinYearHist : PX.Data.IBqlField
		{
		}
		protected DateTime? _BegFinYearHist;

        /// <summary>
        /// The start date of the financial year.
        /// </summary>
        /// <value>
        /// Defaults to the value of the <see cref="FinYearSetup.BegFinYear"/> field of the financial year setup record.
        /// </value>
		[PXDBDate()]
		[PXDefault(typeof(Search<FinYearSetup.begFinYear>))]
		[PXUIField(DisplayName = "Financial Year Starts On", Visibility = PXUIVisibility.Visible, Visible = false, Enabled = false)]
		public virtual DateTime? BegFinYearHist
		{
			get
			{
				return this._BegFinYearHist;
			}
			set
			{
				this._BegFinYearHist = value;
			}
		}
		#endregion
		#region PeriodsStartDateHist
		public abstract class periodsStartDateHist : PX.Data.IBqlField
		{
		}
		protected DateTime? _PeriodsStartDateHist;

        /// <summary>
        /// The start date of the first period of the year.
        /// </summary>
        /// <value>
        /// Defaults to the value of the <see cref="FinYearSetup.PeriodsStartDate"/> field of the financial year setup record.
        /// </value>
		[PXDBDate()]
		[PXDefault(typeof(Search<FinYearSetup.periodsStartDate>))]
		[PXUIField(DisplayName = "Periods Start Date", Visibility = PXUIVisibility.Visible, Visible = false, Enabled = false)]
		public virtual DateTime? PeriodsStartDateHist
		{
			get
			{
				return this._PeriodsStartDateHist;
			}
			set
			{
				this._PeriodsStartDateHist = value;
			}
		}
		#endregion
		#region NoteID
        public abstract class noteID : PX.Data.IBqlField { }

        /// <summary>
        /// Identifier of the <see cref="PX.Data.Note">Note</see> object, associated with the document.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Data.Note.NoteID">Note.NoteID</see> field. 
        /// </value>
		[PXNote(DescriptionField = typeof(FinYear.year))]
		public virtual Guid? NoteID { get; set; }
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
		#region Methods
		public static implicit operator TX.TaxYear(FinYear item)
		{
			TX.TaxYear ret = new TX.TaxYear();
			ret.Year = item.Year;
			ret.StartDate = item.StartDate;
			return ret;
		}
		#endregion
	}
}
