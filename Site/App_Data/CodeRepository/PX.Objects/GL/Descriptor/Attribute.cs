using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using PX.Common;
using PX.Data;

using PX.Objects.Common;
using PX.Objects.AR;
using PX.Objects.AP;
using PX.Objects.CS;
using PX.Objects.CM;
using PX.Objects.CA;
using PX.Objects.TX;

namespace PX.Objects.GL
{
	public class DrCr
	{
		public const string Debit = "D";
		public const string Credit = "C";

		public class debit : Constant<string>
		{
			public debit()
				: base(Debit) { ; }
		}

		public class credit : Constant<string>
		{
			public credit()
				: base(Credit) { ; }
		}
	}

	public class TranDateOutOfRangeException : PXSetPropertyException
	{
		public TranDateOutOfRangeException(string FieldName)
			: base(Messages.TranDateOutOfRange, FieldName)
		{
		}


		public TranDateOutOfRangeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			PXReflectionSerializer.RestoreObjectProps(this, info);
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			PXReflectionSerializer.GetObjectData(this, info);
			base.GetObjectData(info, context);
		}
	}

	public class FiscalPeriodClosedException : PXSetPropertyException
	{
		public FiscalPeriodClosedException(string FinPeriodID)
			: this(FinPeriodID, PXErrorLevel.Error)
		{
		}

		public FiscalPeriodClosedException(string FinPeriodID, PXErrorLevel errorLevel)
			: base(Messages.FiscalPeriodClosed, errorLevel, FinPeriodIDAttribute.FormatForError(FinPeriodID))
		{ 
		}

		
		public FiscalPeriodClosedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			PXReflectionSerializer.RestoreObjectProps(this, info);
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			PXReflectionSerializer.GetObjectData(this, info);
			base.GetObjectData(info, context);
		}
	}

	public class FiscalPeriodInactiveException : PXSetPropertyException
	{
		public FiscalPeriodInactiveException(string FinPeriodID)
			: this(FinPeriodID, PXErrorLevel.Error)
		{
		}

		public FiscalPeriodInactiveException(string FinPeriodID, PXErrorLevel errorLevel)
			: base(Messages.FiscalPeriodInactive, errorLevel, FinPeriodIDAttribute.FormatForError(FinPeriodID))
		{
		}


		public FiscalPeriodInactiveException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			PXReflectionSerializer.RestoreObjectProps(this, info);
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			PXReflectionSerializer.GetObjectData(this, info);
			base.GetObjectData(info, context);
		}
	}

	public class FiscalPeriodInvalidException : PXSetPropertyException
	{
		public FiscalPeriodInvalidException(string FinPeriodID)
			: this(FinPeriodID, PXErrorLevel.Error)
		{
		}

		public FiscalPeriodInvalidException(string FinPeriodID, PXErrorLevel errorLevel)
			: base(Messages.FiscalPeriodInvalid, errorLevel, FinPeriodIDAttribute.FormatForError(FinPeriodID))
		{
		}


		public FiscalPeriodInvalidException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			PXReflectionSerializer.RestoreObjectProps(this, info);
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			PXReflectionSerializer.GetObjectData(this, info);
			base.GetObjectData(info, context);
		}
	}

	/// <summary>
	/// Implements Formatting for FinPeriod field.
	/// FinPeriod is stored and dispalyed as a string but in different formats. 
	/// This Attribute handles the conversion of one format into another.
	/// </summary>
	public class FinPeriodIDFormattingAttribute : PXEventSubscriberAttribute, IPXFieldSelectingSubscriber, IPXFieldUpdatingSubscriber, IPXFieldDefaultingSubscriber
	{
		#region State
		private Type _sourceType;
		private Type _searchType;
		private string _sourceField;
		private DateTime _sourceDate;
		protected const string CS_DISPLAY_MASK = "##-####";

		public Type SearchType
		{
			get
			{
				return _searchType;
			}
			set
			{
				_searchType = value;
			}
		}

		public Type SourceType
		{
			get
			{
				return _sourceType;
			}
			set
			{
				_sourceType = value;
			}
		}
		#endregion

		#region Ctor
		public FinPeriodIDFormattingAttribute()
			: base()			
		{
		}

		public FinPeriodIDFormattingAttribute(Type sourceType, Type searchType)
			:this()
		{
			if (sourceType != null)
			{
				_sourceType = BqlCommand.GetItemType(sourceType);
				_searchType = searchType;
				_sourceField = sourceType.Name;
			}
		}
		#endregion

		#region Initialization
		public override void CacheAttached(PXCache sender)
		{
			if (_sourceType != null)
			{
				sender.Graph.FieldUpdated.AddHandler(_sourceType, _sourceField, SourceFieldUpdated);
				sender.Graph.FieldVerifying.AddHandler(_sourceType, _sourceField, SourceFieldVerifying);
			}
		}
		#endregion

		#region Implementation
		private void SourceFieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			string newValue = null;
			if (e.NewValue != null)
			{
				DateTime sourcedate = (DateTime)e.NewValue;
				newValue = GetPeriod(cache, _searchType, sourcedate, new object[] { e.Row }, false);

				if (string.IsNullOrEmpty(newValue))
				{
					throw new TranDateOutOfRangeException(_sourceField);
				}
			}
		}

		internal void SourceFieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			object sourcedate = cache.GetValue(e.Row, _sourceField);
			if (sourcedate != null)
			{
				PeriodResult result = GetPeriod(cache, _searchType, (DateTime)sourcedate, new object[] { e.Row }, true);
				object newValue = (string)result;
				bool haspending = false;
				try
				{
					object pendingValue = cache.GetValuePending(e.Row, _FieldName);
					if (pendingValue != null && pendingValue!= PXCache.NotSetValue)
					{
						object period = UnFormatPeriod((string)pendingValue);
						cache.RaiseFieldVerifying(_FieldName, e.Row, ref period);
						haspending = true;
					}
				}
				catch (PXSetPropertyException)
				{
					cache.SetValuePending(e.Row, _FieldName, newValue);
				}
				finally
				{
					if (cache.HasAttributes(e.Row))
					{
						cache.RaiseExceptionHandling(_FieldName, e.Row, null, null);
					}

					//this will happen only if FirstOpenPeriod is set from OpenPeriod
					if (!haspending && result.StartDate > (DateTime?)sourcedate)
					{
						cache.SetValueExt(e.Row, _sourceField, result.StartDate);
					}
					else
					{
						cache.SetDefaultExt(e.Row, _FieldName, newValue);
					}
				}
			}
			else
			{
				cache.SetValue(e.Row, _FieldName, null);
			}
		}

		public virtual void FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
		{
			e.NewValue = FormatForStoring(e.NewValue as string);
		}

		public virtual void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			string perPost = e.ReturnValue as string;
			if (perPost != null && (e.Row == null || object.Equals(e.ReturnValue, sender.GetValue(e.Row, _FieldOrdinal))))
			{
				e.ReturnValue = FormatForDisplay(perPost);
			}
			if (_AttributeLevel == PXAttributeLevel.Item || e.IsAltered)
			{
				e.ReturnState = PXStringState.CreateInstance(e.ReturnState, FiscalPeriodUtils.FULL_LENGHT, null, _FieldName, null, null, CS_DISPLAY_MASK, null, null, null, null);
			}
		}

		public virtual void FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			GetFields(sender, e.Row);

			if (_sourceDate != DateTime.MinValue)
			{
				e.NewValue = (string)GetPeriod(sender, _searchType, _sourceDate, new [] {e.Row}, true);
			}
		}

		public static string FormatForError(string period)
		{ 
			return Mask.Format(CS_DISPLAY_MASK, FormatForDisplay(period));
		}

		protected static string FixedLength(string period)
		{
			if (period == null)
			{
				return null;
			}
			else if (period.Length >= FiscalPeriodUtils.FULL_LENGHT)
			{
				return period.Substring(0, FiscalPeriodUtils.FULL_LENGHT);
			}
			else
			{
				return period.PadRight(FiscalPeriodUtils.FULL_LENGHT);
			}
		}

		public static string FormatForDisplay(string period)
		{
			return FormatForDisplayInt(FixedLength(period));
		}

		protected static string FormatForDisplayInt(string period)
		{
			return string.IsNullOrEmpty(period) ? null : FiscalPeriodUtils.PeriodInYear(period) + FiscalPeriodUtils.FiscalYear(period);
		}

		public static string FormatForStoring(string period)
		{
			return FormatForStoringInt(FixedLength(period));
		}

		protected static string FormatForStoringInt(string period)
		{
			return string.IsNullOrEmpty(period) ? null : period.Substring(FiscalPeriodUtils.PERIOD_LENGTH, FiscalPeriodUtils.YEAR_LENGTH) + period.Substring(0, FiscalPeriodUtils.PERIOD_LENGTH);
		}

		protected static string FormatPeriod(string period)
		{
			return FormatForDisplay(period);
		}

		protected static string UnFormatPeriod(string period)
		{
			return FormatForStoring(period);
		}

		public class PeriodResult
		{
			public string PeriodID;
			public DateTime? StartDate;
			public DateTime? EndDate;
			public static implicit operator string(PeriodResult p)
			{
				return p.PeriodID;
			}
		}

		public static PeriodResult GetPeriod(PXCache sender, Type searchType, DateTime fromdate, Boolean applyformat)
		{
			return GetPeriod(sender, searchType, fromdate, null, applyformat);
		}

		public static PeriodResult GetPeriod(PXCache sender, Type searchType, DateTime fromdate, object[] currents, Boolean applyformat)
		{
			BqlCommand Select = BqlCommand.CreateInstance(searchType);

			Type sourceType = BqlCommand.GetItemType(((IBqlSearch)Select).GetField());
			string sourceField = ((IBqlSearch)Select).GetField().Name;

			PXView view = sender.Graph.TypedViews.GetView(Select, false);
			int startRow = 0;
			int totalRows = 0;
			List<object> source = view.Select(
				currents,
				new object[] { fromdate, fromdate },
				null,
				null,
				null,
				null,
				ref startRow,
				1,
				ref totalRows);
			if (source != null && source.Count > 0)
			{
				object item = source[source.Count - 1];
				if (item != null && item is PXResult)
				{
					item = ((PXResult)item)[sourceType];
				}

				string result = (string)sender.Graph.Caches[sourceType].GetValue(item, sourceField);
				if (applyformat && result != null && result.Length == 6)
				{
					result = FormatPeriod(result);
				}

				if (item as IPeriod != null)
				{
					return new PeriodResult { PeriodID = result, StartDate = (item as IPeriod).StartDate, EndDate = (item as IPeriod).EndDate };
				}
			}
			return new PeriodResult();
		}

		private void GetFields(PXCache sender, object row)
		{
			_sourceDate = DateTime.MinValue;
			if (_sourceType != null)
			{
				_sourceDate = (DateTime)(PXView.FieldGetValue(sender, row, _sourceType, _sourceField) ?? DateTime.MinValue);
			}
		}

		public virtual DateTime GetDate(PXCache sender, object row)
		{
			GetFields(sender, row);
			return _sourceDate;
		}

		#endregion
	}

	public class PXFinPeriodException : PXException
	{
		public PXFinPeriodException()
			: base(Messages.NoPeriodsDefined)
		{
		}

		public PXFinPeriodException(string message)
			: base(message)
		{
		}

		public PXFinPeriodException(string format, params object[] args)
			: base(format, args)
		{
		}

		public PXFinPeriodException(Exception innerException, string format, params object[] args)
			: base(innerException, format, args)
		{ }

		public PXFinPeriodException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			PXReflectionSerializer.RestoreObjectProps(this, info);
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			PXReflectionSerializer.GetObjectData(this, info);
			base.GetObjectData(info, context);
		}
	}

	public class PeriodIDAttribute : PXAggregateAttribute, IPXCommandPreparingSubscriber, IPXRowSelectingSubscriber
	{
		#region State
		protected Type _SearchType;
		protected Type _SourceType;
		protected bool _IsDBField = true;
		public bool IsDBField
		{
			get
			{
				return this._IsDBField;
			}
			set
			{
				this._IsDBField = value;
			}
		}
		public bool IsKey
		{
			get
			{
				return ((PXDBStringAttribute)_Attributes[0]).IsKey;
			}
			set
			{
				((PXDBStringAttribute)_Attributes[0]).IsKey = value;
			}
		}

		public new string FieldName
		{
			get
			{
				return ((PXDBStringAttribute)_Attributes[0]).FieldName;
			}
			set
			{
				((PXDBStringAttribute)_Attributes[0]).FieldName = value;
			}
		}

		public Type BqlField
		{
			get
			{
				return ((PXDBFieldAttribute)_Attributes[0]).BqlField;
			}
			set
			{
				((PXDBFieldAttribute)_Attributes[0]).BqlField = value;
				BqlTable = ((PXDBFieldAttribute)_Attributes[0]).BqlTable;
			}
		}

		public Type SearchType
		{
			get
			{
				return _SearchType;
			}
			set
			{
				_Attributes.OfType<FinPeriodIDFormattingAttribute>().First().SearchType = _SearchType = value;
			}
		}

		public Type SourceType
		{
			get
			{
				return _SourceType;
			}
			set
			{
				_Attributes.OfType<FinPeriodIDFormattingAttribute>().First().SourceType = _SourceType = value;
			}
		}

		#endregion

		#region Ctor
		public PeriodIDAttribute()
			: this(null, null)
		{
		}
		public PeriodIDAttribute(Type SourceType, Type SearchType)
		{
			_Attributes.Clear();// = new List<PXEventSubscriberAttribute>();
			PXDBStringAttribute dbstr = new PXDBStringAttribute(FiscalPeriodUtils.FULL_LENGHT);
			dbstr.IsFixed = true;
			_Attributes.Add(dbstr);
			_Attributes.Add(new FinPeriodIDFormattingAttribute(SourceType, SearchType));
			_SearchType = SearchType;
		}
		#endregion

		#region Initialization
		public override void GetSubscriber<ISubscriber>(List<ISubscriber> subscibers)
		{
			if ((typeof(ISubscriber) == typeof(IPXCommandPreparingSubscriber)
				|| typeof(ISubscriber) == typeof(IPXRowSelectingSubscriber)))
			{
				if (_IsDBField == false)
				{
					subscibers.Add(this as ISubscriber);
				}
				else
				{
					base.GetSubscriber<ISubscriber>(subscibers);
					subscibers.Remove(this as ISubscriber);
				}
			}
			else
			{
				base.GetSubscriber<ISubscriber>(subscibers);
			}
		}
		#endregion

		#region Implementation
		public virtual void CommandPreparing(PXCache sender, PXCommandPreparingEventArgs e)
		{
			e.FieldName = string.Empty;
			e.Cancel = true;
		}
		public virtual void RowSelecting(PXCache sender, PXRowSelectingEventArgs e)
		{
			sender.SetValue(e.Row, _FieldOrdinal, null);
		}
		#endregion

		#region Static
		protected static string OffsetPeriod(PXGraph graph, string FiscalPeriodID, int counter, int periodsInYear)
		{
			int years = Convert.ToInt32(FiscalPeriodID.Substring(0, 4));
			int periods = Convert.ToInt32(FiscalPeriodID.Substring(4, 2));

			int endYear = years - 1 + (int)decimal.Ceiling((periods + counter) / (decimal)periodsInYear);
			int endPeriod;
			if (periods + counter > 0)
			{
				endPeriod = (endPeriod = (periods + counter) % periodsInYear) == 0 ? periodsInYear : endPeriod;
			}
			else
			{
				endPeriod = (endPeriod = (periods + counter) % periodsInYear) == 0 ? periodsInYear : periodsInYear + endPeriod;
			}

			return $"{endYear:0000}{endPeriod:00}";
		}

		public static string Min(string period1, string period2)
		{
			return string.CompareOrdinal(period1, period2) > 0 ? period2 : period1;
		}

		public static string Max(string period1, string period2)
		{
			return string.CompareOrdinal(period1, period2) > 0 ? period1 : period2;
		}
		#endregion

	}


	/// <summary>
	/// Attribute describes FinPeriod Field.
	/// This attribute contains static Util functions.
	/// </summary>
	public class FinPeriodIDAttribute : PeriodIDAttribute
	{
		#region Ctor
		public FinPeriodIDAttribute()
			:this(null)
		{
		}
		public FinPeriodIDAttribute(Type SourceType)
			: base(SourceType, typeof(Search<FinPeriod.finPeriodID, Where<FinPeriod.startDate, LessEqual<Required<FinPeriod.startDate>>, And<FinPeriod.endDate, Greater<Required<FinPeriod.endDate>>>>>))
		{
		}
		#endregion

		#region Period Manipulation

		/// <summary>
		/// Returns FinPeriod form the given date.
		/// </summary>
		public static string PeriodFromDate(DateTime? fromdate)
		{
			return GetPeriod(fromdate.Value);
		}

		/// <summary>
		/// Returns FinPeriod form the given date.
		/// </summary>
		public static string GetPeriod(DateTime fromDate)
		{
			using (PXDataRecord record = PXDatabase.SelectSingle<FinPeriod>(
				new PXDataField(nameof(FinPeriod.FinPeriodID)),
				new PXDataFieldValue(nameof(FinPeriod.StartDate), PXDbType.SmallDateTime, 4, fromDate, PXComp.LE),
				new PXDataFieldValue(nameof(FinPeriod.EndDate), PXDbType.SmallDateTime, 4, fromDate, PXComp.GT)))
			{
				return record?.GetString(0);
			}
		}

		public static FinPeriod FindMaxFinPeriodWithEndDataBelongToInterval(PXGraph graph, DateTime? startDate, DateTime? endDate)
		{
			return PXSelect<FinPeriod,
								Where<FinPeriod.finDate, GreaterEqual<Required<FinPeriod.finDate>>,
									And<FinPeriod.finDate, Less<Required<FinPeriod.finDate>>>>,
								OrderBy<Desc<FinPeriod.finPeriodID>>>
								.Select(graph, startDate, endDate);
		}

		/// <summary>
		/// Returns PeriodID from the given date.
		/// </summary>
		public static string PeriodFromDate(PXGraph graph, DateTime? date)
		{
			var period = GetFinPeriodByDate(graph, date);

			if (period == null)
			{
				throw new PXFinPeriodException(
					Messages.NoFinancialPeriodForDate,
					date?.ToShortDateString());
			}

			return period.FinPeriodID;
		}
		
		public static FinPeriod GetFinPeriodByDate(PXGraph graph, DateTime? date)
		{
			FinPeriod finPeriod = PXSelect<
				FinPeriod, 
				Where<
					FinPeriod.startDate, LessEqual<Required<FinPeriod.startDate>>, 
					And<FinPeriod.endDate, Greater<Required<FinPeriod.endDate>>>>>
				.Select(graph, date, date);

			if (finPeriod == null)
			{
				throw new PXFinPeriodException(
					Messages.NoFinancialPeriodForDate,
					date?.ToShortDateString());
			}

			return finPeriod;
		}
		
		/// <summary>
		/// Returns n'th PeriodID from the given Period. 
		/// </summary>
		/// <param name="fiscalPeriodID">Given Period.</param>
		/// <param name="periodsToSkip">Number of periods to skip over</param>
		public static string PeriodPlusPeriod(PXGraph graph, string fiscalPeriodID, short periodsToSkip)
		{
			FinYearSetup setup = PXSelect<FinYearSetup>.Select(graph);

			//TODO: Need to refactor, duplicates the part of function FABookPeriodIDAttribute.GetBookPeriodsInYear
			FinPeriodSetup periodsInYear = PXSelectGroupBy<FinPeriodSetup, Where<FinPeriodSetup.endDate, Greater<FinPeriodSetup.startDate>>,
																Aggregate<Max<FinPeriodSetup.periodNbr>>>.Select(graph);
			if (setup != null && FiscalPeriodSetupCreator.IsFixedLengthPeriod(setup.FPType) && 
				periodsInYear != null && periodsInYear.PeriodNbr != null)
			{
				return OffsetPeriod(graph, fiscalPeriodID, periodsToSkip, Convert.ToInt32(periodsInYear.PeriodNbr));
			}
			else if (periodsToSkip > 0)
			{
				PXResultset<FinPeriod> res = PXSelect<FinPeriod, Where<FinPeriod.finPeriodID, Greater<Required<FinPeriod.finPeriodID>>, And<FinPeriod.startDate, NotEqual<FinPeriod.endDate>>>, OrderBy<Asc<FinPeriod.finPeriodID>>>.SelectWindowed(graph, 0, periodsToSkip, fiscalPeriodID);

				if (res.Count < periodsToSkip)
				{
					throw new PXFinPeriodException();
				}

				return ((FinPeriod)res[res.Count - 1]).FinPeriodID;
			}
			else if (periodsToSkip < 0)
			{
				PXResultset<FinPeriod> res = PXSelect<FinPeriod, Where<FinPeriod.finPeriodID, Less<Required<FinPeriod.finPeriodID>>, And<FinPeriod.startDate, NotEqual<FinPeriod.endDate>>>, OrderBy<Desc<FinPeriod.finPeriodID>>>.SelectWindowed(graph, 0, -periodsToSkip, fiscalPeriodID);

				if (res.Count < -periodsToSkip)
				{
					throw new PXFinPeriodException();
				}

				return ((FinPeriod)res[res.Count - 1]).FinPeriodID;
			}
			else
			{
				return fiscalPeriodID;
			}
		}
		
		/// <summary>
		/// Returns Next Period from the given.
		/// </summary>
		public static string NextPeriod(PXGraph graph, string FiscalPeriodID)
		{
			return PeriodPlusPeriod(graph, FiscalPeriodID, 1);
		}

		/// <summary>
		/// Returns Start date for the given Period
		/// </summary>
		public static DateTime PeriodStartDate(PXGraph graph, string FiscalPeriodID)
		{
			FinPeriod p = (FinPeriod)PXSelect<FinPeriod, Where<FinPeriod.finPeriodID, Equal<Required<FinPeriod.finPeriodID>>>>.Select(graph, FiscalPeriodID);

			if (p == null)
			{
				throw new PXFinPeriodException();
			}

			return (DateTime)p.StartDate;
		}

		/// <summary>
		/// Returns End date for the given period
		/// </summary>
		public static DateTime PeriodEndDate(PXGraph graph, string FiscalPeriodID)
		{
			FinPeriod p = (FinPeriod)PXSelect<FinPeriod, Where<FinPeriod.finPeriodID, Equal<Required<FinPeriod.finPeriodID>>>>.Select(graph, FiscalPeriodID);

			if (p == null)
			{
				throw new PXFinPeriodException();
			}

			return ((DateTime)p.EndDate).AddDays(-1);
		}

		public static IEnumerable<FinPeriod> GetFinPeriodsInInterval(PXGraph graph,  DateTime? fromDate, DateTime? tillDate)
		{
			return PXSelect<FinPeriod,
								Where<FinPeriod.startDate, GreaterEqual<Required<FinPeriod.startDate>>,
										And<FinPeriod.endDate, LessEqual<Required<FinPeriod.endDate>>,
										And<FinPeriod.startDate, NotEqual<FinPeriod.endDate>>>>>
								.Select(graph, fromDate, tillDate)
								.RowCast<FinPeriod>();
		}

		public static IEnumerable<FinPeriod> GetAdjustmentFinPeriods(PXGraph graph, string finYear)
		{
			return PXSelect<FinPeriod,
								Where<FinPeriod.finYear, Equal<Required<FinPeriod.finYear>>,
										And<FinPeriod.startDate, Equal<FinPeriod.endDate>>>>
								.Select(graph, finYear)
								.RowCast<FinPeriod>();
		}

		public static FinPeriod FindLastYearNotAdjustmentPeriod(PXGraph graph, string finYear)
		{
			return (FinPeriod)PXSelect<FinPeriod,
											Where<FinPeriod.finYear, Equal<Required<FinPeriod.finYear>>,
													And<FinPeriod.startDate, NotEqual<FinPeriod.endDate>>>,
											OrderBy<Desc<FinPeriod.finPeriodID>>>
											.SelectWindowed(graph, 0, 1, finYear);
		}

		/// <summary>
		/// Format Period to string that can be used in an error message.
		/// </summary>
		public static string FormatForError(string period)
		{
			return FinPeriodIDFormattingAttribute.FormatForError(period);
		}

		/// <summary>
		/// Format Period to string that can be displayed in the control.
		/// </summary>
		/// <param name="period">Period in database format</param>
		public static string FormatForDisplay(string period)
		{
			return FinPeriodIDFormattingAttribute.FormatForDisplay(period);
		}

		/// <summary>
		/// Format period to database format
		/// </summary>
		/// <param name="period">Period in display format</param>
		/// <returns></returns>
		public static string UnFormatPeriod(string period)
		{
			return FinPeriodIDFormattingAttribute.FormatForStoring(period);
		}

		/// <summary>
		/// Returns a minimal set of financial periods that contain a given date interval
		/// within them, excluding any adjustment periods.
		/// </summary>
		/// <param name="graph">The graph which will be used when performing a select DB query.</param>
		/// <param name="startDate">The starting date of the date interval.</param>
		/// <param name="endDate">The ending date of the date interval.</param>
		public static IEnumerable<FinPeriod> PeriodsBetweenInclusive(PXGraph graph, DateTime startDate, DateTime endDate)
		{
			if (startDate > endDate)
			{
				throw new PXArgumentException(nameof(startDate));
			}

			return PXSelect<
				FinPeriod,
				Where<
					FinPeriod.endDate, Greater<Required<FinPeriod.endDate>>,
					And<FinPeriod.startDate, LessEqual<Required<FinPeriod.startDate>>,
					And<FinPeriod.startDate, NotEqual<FinPeriod.endDate>>>>>
				.Select(graph, startDate, endDate)
				.RowCast<FinPeriod>();
		}

		public enum FinPeriodComparison
		{
			Full,
			Year,
			Month
		};

		public static bool FinPeriodEqual(string period1, string period2, FinPeriodComparison comparison)
		{
			if (period1 != null && period2 != null && period1.Length >= FiscalPeriodUtils.FULL_LENGHT && period2.Length >= FiscalPeriodUtils.FULL_LENGHT)
			{
				switch (comparison)
				{
					case FinPeriodComparison.Full:
						break;
					case FinPeriodComparison.Year:
						period1 = period1.Substring(0, FiscalPeriodUtils.YEAR_LENGTH);
						period2 = period2.Substring(0, FiscalPeriodUtils.YEAR_LENGTH);
						break;
					case FinPeriodComparison.Month:
						period1 = period1.Substring(FiscalPeriodUtils.YEAR_LENGTH, FiscalPeriodUtils.PERIOD_LENGTH);
						period2 = period2.Substring(FiscalPeriodUtils.YEAR_LENGTH, FiscalPeriodUtils.PERIOD_LENGTH);
						break;
				}
			}

			return string.Equals(period1, period2);
		}
      public static void CheckIsDateWithinPeriod(PXGraph graph, string finPeriodID, DateTime date, string errorMessage)
      {
          var tranFinPeriod = (FinPeriod)PXSelect<FinPeriod,
              Where<FinPeriod.finPeriodID, Equal<Required<FinPeriod.finPeriodID>>>>
              .Select(graph, finPeriodID);

          if (date < tranFinPeriod.StartDate || date >= tranFinPeriod.EndDate)
          {
              throw new PXSetPropertyException(errorMessage);
          }
      }
		
	public static string GetFirstFinPeriodIDOfYear(string finPeriodID)
		{
			return string.Concat(finPeriodID.Substring(0, 4), "01");
		}

		public static string GetNextYearID(string finID)
		{
			return (Convert.ToInt32(finID.Substring(0, 4)) + 1).ToString();
		}

		public static bool PeriodExists(PXGraph graph, string finPeriodID)
		{
			PXSelectBase<FinPeriod> financialPeriodView = new PXSelect<
				FinPeriod,
				Where<
					FinPeriod.finPeriodID, Equal<Required<FinPeriod.finPeriodID>>>>(graph);

			return financialPeriodView.Any(finPeriodID);
		}

		/// <summary>
		/// Gets the ID of the financial period with the same <see cref="FinPeriod.PeriodNbr"/> 
		/// as the one specified, but residing in the previous financial year. If no such financial 
		/// period exists, an exception is thrown.
		/// </summary>
		public static string GetSamePeriodInPreviousYear(PXGraph graph, string finPeriodID)
		{
			string yearPart = finPeriodID.Substring(0, 4);
			string periodNumber = finPeriodID.Substring(4, 2);

			string previousYear = (int.Parse(yearPart) - 1).ToString();
			string suggestedPeriodID = string.Concat(previousYear, periodNumber);

			if (!PeriodExists(graph, suggestedPeriodID))
			{
				throw new PXFinPeriodException();
			}

			return suggestedPeriodID;
		}

		public static FinPeriod GetFinPeriodByID(PXGraph graph, string finPeriodID)
		{
			var finPeriod = (FinPeriod)PXSelect<FinPeriod,
								Where<FinPeriod.finPeriodID, Equal<Required<FinPeriod.finPeriodID>>>>
								.Select(graph, finPeriodID);
			if (finPeriod == null)
			{
				throw new PXException(Common.Messages.EntityWithIDDoesNotExist, EntityHelper.GetFriendlyEntityName(typeof(FinPeriod)),
					finPeriodID);
			}

			return finPeriod;
		}

		public static FinPeriod FindFinPeriodWithStartDate(PXGraph graph, DateTime? startDate)
		{
			return (FinPeriod)PXSelect<FinPeriod,
							Where<FinPeriod.startDate, Equal<Required<FinPeriod.startDate>>>>
							.Select(graph, startDate);
		}

		public static FinPeriod FindFinPeriodWithEndDate(PXGraph graph, DateTime? endDate)
		{
			return (FinPeriod)PXSelect<FinPeriod,
							Where<FinPeriod.endDate, Equal<Required<FinPeriod.endDate>>>>
							.Select(graph, endDate);
		}

		#endregion
	}

	public class TranPeriodIDAttribute : FinPeriodIDAttribute
	{
		private string _sourceField;

		#region Ctor
		public TranPeriodIDAttribute(Type sourceType)
			: base(sourceType)
		{
			_sourceField = sourceType.Name;
		}

		private FinPeriodIDFormattingAttribute FormattingAttribute
		{
			get
			{
				return _Attributes.OfType<FinPeriodIDFormattingAttribute>().First();
			}
		}

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);

			var sourceItemType = FormattingAttribute.SourceType;

			if(sourceItemType != null)
			{
				sender.Graph.FieldUpdated.RemoveHandler(sourceItemType, _sourceField, FormattingAttribute.SourceFieldUpdated);
				sender.Graph.FieldUpdated.AddHandler(sourceItemType, _sourceField, SourceFieldUpdated);
			}
		}

		private void SourceFieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			object sourceDate = cache.GetValue(e.Row, _sourceField);
			if(sourceDate != null)
			{
				var period = GetPeriod((DateTime)sourceDate);
				var formattedPeriod = FinPeriodIDAttribute.FormatForDisplay(period);

				cache.SetValuePending(e.Row, _FieldName, formattedPeriod);
				cache.SetValueExt(e.Row, _FieldName, formattedPeriod);
			}
		}
		#endregion
	}

	/// <summary>
	/// Selector for FinPeriod. Extends <see cref="FinPeriodIDAttribute"/>.
	/// Displays all available fin Periods.
	/// </summary>
	public class FinPeriodSelectorAttribute : FinPeriodIDAttribute, IPXFieldVerifyingSubscriber
	{
		#region Ctor
		public FinPeriodSelectorAttribute()
			: this(null)
		{
		}

		public FinPeriodSelectorAttribute(Type SourceType, params Type[] fieldList)
			: this(typeof(Search3<FinPeriod.finPeriodID, OrderBy<Desc<FinPeriod.finPeriodID>>>), SourceType, fieldList)
		{
		}

		public FinPeriodSelectorAttribute(Type SearchType, Type SourceType, params Type[] fieldList)
			:base(SourceType)
		{
			Type[] fields = new Type[] { typeof(FinPeriod.finPeriodID),typeof(FinPeriod.descr) };
			if (fieldList != null && fieldList.Length > 0)
			{				
				fields = fieldList;
			}

			if (SearchType == null || !typeof(IBqlSearch).IsAssignableFrom(SearchType))
			{
				throw new PXArgumentException("search", ErrorMessages.ArgumentException);
			}

			Type searchType = SearchType.GetGenericTypeDefinition();
			Type[] searchArgs = SearchType.GetGenericArguments();

			Type cmd;
			//compatibility: if WhereType = null then Desc<> else Asc<>
			if (searchType == typeof(Search<>))
			{
				cmd = BqlCommand.Compose(
					typeof(Search3<,>),
					typeof(FinPeriod.finPeriodID),
					typeof(OrderBy<Desc<FinPeriod.finPeriodID>>));
			}
			else if (searchType == typeof(Search<,>))
			{
				cmd = BqlCommand.Compose(
					typeof(Search<,,>),
					typeof(FinPeriod.finPeriodID),
					searchArgs[1],
					typeof(OrderBy<Asc<FinPeriod.finPeriodID>>));
			}
			else if (searchType == typeof(Search<,,>))
			{
				cmd = SearchType;
			}
			
			else if (searchType == typeof(Search2<,>))
			{
				cmd = BqlCommand.Compose(
					typeof(Search3<,,>),
					typeof(FinPeriod.finPeriodID),
					searchArgs[1],
					typeof(OrderBy<Desc<FinPeriod.finPeriodID>>));
			}
			else if (searchType == typeof(Search2<,,>))
			{
				cmd = BqlCommand.Compose(
					typeof(Search2<,,,>),
					typeof(FinPeriod.finPeriodID),
					searchArgs[1],
					searchArgs[2],
					typeof(OrderBy<Asc<FinPeriod.finPeriodID>>));
			}
			else if (searchType == typeof(Search2<,,,>))
			{
				cmd = SearchType;
			}
			else if (searchType == typeof(Search3<,>))
			{
				cmd = SearchType;
			}
			else if (searchType == typeof(Search3<,,>))
			{
				cmd = SearchType;
			}
			else
			{
				throw new PXArgumentException("search", ErrorMessages.ArgumentException);
			}

			_Attributes.Add(new PXSelectorAttribute(cmd, fields));
		}
		#endregion

		#region Initialization
		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);
			Selector.SelectorMode |= PXSelectorMode.NoAutocomplete;
		}

		public override void GetSubscriber<ISubscriber>(List<ISubscriber> subscribers)
		{
			if (typeof(ISubscriber) == typeof(IPXFieldVerifyingSubscriber))
			{
				subscribers.Add(this as ISubscriber);
			}
			else
			{
				base.GetSubscriber<ISubscriber>(subscribers);
			}
		}
		#endregion

		#region Implementation
		public virtual void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			try
			{
				((IPXFieldVerifyingSubscriber)_Attributes[2]).FieldVerifying(sender, e);
			}
			catch (PXSetPropertyException)
			{
				e.NewValue = FormatForDisplay((string)e.NewValue);
				throw;
			}
		}
				
		#endregion

		protected PXSelectorAttribute Selector
		{
			get { return _Attributes[_Attributes.Count - 1] as PXSelectorAttribute; }
		}

		public Type DescriptionField
		{
			get { return Selector.DescriptionField; }
			set { Selector.DescriptionField = value; }
		}

		public PXSelectorMode SelectorMode
		{
			get { return Selector.SelectorMode; }
			set { Selector.SelectorMode = value; }
		}
	}

	public enum PeriodValidation
	{
		Nothing,
		DefaultUpdate,
		DefaultSelectUpdate,
	}

	/// <summary>
	/// FinPeriod selector that extends <see cref="FinPeriodSelectorAttribute"/>. 
	/// Displays and accepts only Open Fin Periods. 
	/// When Date is supplied through SourceType parameter FinPeriod is defaulted with the FinPeriod for the given date.
	/// </summary>
	public class OpenPeriodAttribute : FinPeriodSelectorAttribute, IPXFieldVerifyingSubscriber, IPXFieldDefaultingSubscriber, IPXFieldSelectingSubscriber, IPXRowPersistingSubscriber, IPXRowSelectedSubscriber, IPXFieldUpdatedSubscriber 
	{
		#region State
		protected PeriodValidation _ValidatePeriod = PeriodValidation.DefaultUpdate;

		/// <summary>
		/// Gets or sets how the Period validation logic is handled.
		/// </summary>
		public PeriodValidation ValidatePeriod
		{
			get
			{
				return _ValidatePeriod;
			}
			set
			{
				_ValidatePeriod = value;
			}
		}

		public bool RaiseErrorOnInactivePeriod { get; set; }

		protected DateTime _sourceDate;
		#endregion

		#region Ctor
		public OpenPeriodAttribute(Type SourceType)
			: base(typeof(Search<FinPeriod.finPeriodID, Where<FinPeriod.closed, Equal<False>, And<FinPeriod.active, Equal<True>>>>), SourceType)
		{
		}

		public OpenPeriodAttribute()
			: this(null)
		{
		}

		public OpenPeriodAttribute(Type SearchType, Type SourceType)
			: base(SearchType, SourceType)
		{
		}
		#endregion

		#region Runtime
		public static void SetValidatePeriod<Field>(PXCache cache, object data, PeriodValidation isValidatePeriod)
			where Field : IBqlField
		{
			if (data == null)
			{
				cache.SetAltered<Field>(true);
			}
			foreach (PXEventSubscriberAttribute attr in cache.GetAttributes<Field>(data))
			{
				if (attr is OpenPeriodAttribute)
				{
					((OpenPeriodAttribute)attr).ValidatePeriod = isValidatePeriod;
				}
			}
		}

		public static void SetValidatePeriod(PXCache cache, object data, string name, PeriodValidation isValidatePeriod)
		{
			if (data == null)
			{
				cache.SetAltered(name, true);
			}
			foreach (PXEventSubscriberAttribute attr in cache.GetAttributes(data, name))
			{
				if (attr is OpenPeriodAttribute)
				{
					((OpenPeriodAttribute)attr).ValidatePeriod = isValidatePeriod;
				}
			}
		}
		#endregion

		#region Initialization
		public override void GetSubscriber<ISubscriber>(List<ISubscriber> subscribers)
		{
			if (typeof(ISubscriber) == typeof(IPXFieldVerifyingSubscriber)
				|| typeof(ISubscriber) == typeof(IPXFieldDefaultingSubscriber)
				|| typeof(ISubscriber) == typeof(IPXFieldSelectingSubscriber)
				)
			{
				subscribers.Add(this as ISubscriber);
			}
			else
			{
				base.GetSubscriber<ISubscriber>(subscribers);
			}
		}
		#endregion

		#region Implementation

		public override void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (_ValidatePeriod != PeriodValidation.Nothing)
			{
				OpenPeriodVerifying(sender, e);
			}
			else
			{
				base.FieldVerifying(sender, e);
			}
		}

		public virtual void FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (_ValidatePeriod != PeriodValidation.Nothing)
			{
				OpenPeriodDefaulting(sender, e);
			}
			else
			{
				((IPXFieldDefaultingSubscriber)_Attributes[1]).FieldDefaulting(sender, e);
			}
		}

		public virtual void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			((IPXFieldSelectingSubscriber)_Attributes[1]).FieldSelecting(sender, e);
			((IPXFieldSelectingSubscriber)_Attributes[2]).FieldSelecting(sender, e);

			if (e.ReturnState != null && e.ReturnState is PXStringState)
			{
				e.ReturnState = PXStringState.CreateInstance(e.ReturnState, ((PXStringState)e.ReturnState).Length, null, _FieldName, null, 1, ((PXStringState)e.ReturnState).InputMask, null, null, null, null);
			}
		}

		public virtual void FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			object val = sender.GetValueExt(e.Row, _FieldName);
			if (val is PXFieldState)
			{
				val = ((PXFieldState)val).Value;
			}

			string errval = UnFormatPeriod((string)val);
			if (errval != null && !errval.Equals(sender.GetValue(e.Row, _FieldName)))
			{
				PXUIFieldAttribute.SetError(sender, e.Row, _FieldName, null, null);
			}

		}

		public virtual void GetFields(PXCache sender, object row)
		{
			_sourceDate = ((FinPeriodIDFormattingAttribute)_Attributes[1]).GetDate(sender, row);
		}

		public virtual void OpenPeriodDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			GetFields(sender, e.Row);

			if (_sourceDate != DateTime.MinValue)
			{
				e.NewValue = (string)FinPeriodIDFormattingAttribute.GetPeriod(sender, _SearchType, _sourceDate, true);

				if (e.NewValue == null)
				{
					return;
				}

				try
				{
					IsValidPeriod(sender, e.Row, UnFormatPeriod((string)e.NewValue));
				}
				catch (PXSetPropertyException ex)
				{
					if (e.Row != null)
					{
						sender.RaiseExceptionHandling(_FieldName, e.Row, e.NewValue, ex);
					}
					e.NewValue = null;
				}
			}
		}

		public virtual void OpenPeriodVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			string NewValue = e.NewValue as string;
			string OldValue = (string)sender.GetValue(e.Row, _FieldName);

			try
			{
				IsValidPeriod(sender, e.Row, e.NewValue);
			}
			catch (PXSetPropertyException)
			{
				e.NewValue = FormatForDisplay((string)e.NewValue);
				throw;
			}
			IsCurrentPeriod(sender, e.Row, e.NewValue);

		}

		protected virtual void IsCurrentPeriod(PXCache sender, object row, object value)
		{
			GetFields(sender, row);
			if (_ValidatePeriod != PeriodValidation.Nothing && _sourceDate != DateTime.MinValue)
			{
				string PeriodFromDate = FinPeriodIDFormattingAttribute.GetPeriod(sender, _SearchType, _sourceDate, false);
				string UserPeriod = (string)value;
				if (!object.Equals(PeriodFromDate, UserPeriod))
				{
					//check if user entered offsetting period
					foreach (PXResult<FinPeriod, FinYear> r in PXSelectJoin<FinPeriod, InnerJoin<FinYear, On<FinYear.year, Equal<FinPeriod.finYear>>>, Where<FinPeriod.finPeriodID, Equal<Required<FinPeriod.finPeriodID>>>, OrderBy<Asc<FinPeriod.finPeriodID>>>.Select(sender.Graph, UserPeriod))
					{
						FinPeriod p = (FinPeriod)r;
						FinYear y = (FinYear)r;
						if (p != null)
						{
							if (object.Equals(p.StartDate, p.EndDate) && DateTime.Compare((DateTime)_sourceDate, (DateTime)y.StartDate) >= 0 && DateTime.Compare((DateTime)_sourceDate, (DateTime)p.EndDate) < 0)
							{
								return;
							}
						}
					}

					if (PXUIFieldAttribute.GetError(sender, row, _FieldName) == null)
					{
						PXUIFieldAttribute.SetWarning(sender, row, _FieldName, Messages.FiscalPeriodNotCurrent);
					}
				}
			}
		}

		public virtual void RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			if (sender.Graph.GetType() != typeof(PXGraph) && sender.Graph.GetType() != typeof(PXGenericInqGrph) && !sender.Graph.IsImport)
			{
				//will be called after graph event
				sender.Graph.RowSelected.AddHandler(sender.GetItemType(), new DynamicRowSelected(Document_RowSelected).RowSelected);
			}
		}

		private class DynamicRowSelected
		{
			private PXRowSelected _del;

			public DynamicRowSelected(PXRowSelected del)
			{
				_del = del;
			}
			public void RowSelected(PXCache sender, PXRowSelectedEventArgs e)
			{
				try
				{

					using (sender.GetAttributesReadonly(e.Row, null) as IDisposable)
						_del(sender, e);
				}
				finally
				{
					sender.Graph.RowSelected.RemoveHandler(sender.GetItemType(), RowSelected);
				}
			}
		}

		private PeriodValidation GetValidatePeriod(PXCache cache, object Row)
		{
			foreach (OpenPeriodAttribute attr in cache.GetAttributesReadonly(Row, _FieldName).OfType<OpenPeriodAttribute>())
			{
				return attr._ValidatePeriod;
			}
			return PeriodValidation.Nothing;
		}

		public virtual void Document_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			if (GetValidatePeriod(sender, e.Row) == PeriodValidation.DefaultSelectUpdate)
			{
				object oldState = sender.GetValueExt(e.Row, _FieldName);
				if (oldState is PXFieldState)
				{
					oldState = ((PXFieldState)oldState).Value;
				}
				string OldValue = UnFormatPeriod(oldState as string);

				try
				{
					if (sender.AllowDelete && !string.IsNullOrEmpty(OldValue))
					{
						IsValidPeriod(sender, e.Row, OldValue);
						IsCurrentPeriod(sender, e.Row, OldValue);
					}
				}
				catch (FiscalPeriodInactiveException ex)
				{
					PXUIFieldAttribute.SetWarning(sender, e.Row, _FieldName, ex.Message);
				}
				catch (PXSetPropertyException ex)
				{
					PXUIFieldAttribute.SetError(sender, e.Row, _FieldName, ex.Message, FormatForDisplay(OldValue));
				}
			}
		}

		public virtual void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			string NewValue = (string)sender.GetValue(e.Row, _FieldName);

			try
			{
				if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Insert && _ValidatePeriod != PeriodValidation.Nothing || (e.Operation & PXDBOperation.Command) == PXDBOperation.Update && sender.AllowDelete && _ValidatePeriod == PeriodValidation.DefaultSelectUpdate)
				{
					IsValidPeriod(sender, e.Row, NewValue);

					GetFields(sender, e.Row);
					if (_sourceDate != DateTime.MinValue && string.IsNullOrEmpty(NewValue))
					{
						if (sender.RaiseExceptionHandling(_FieldName, e.Row, null, new PXSetPropertyKeepPreviousException(PXMessages.LocalizeFormat(ErrorMessages.FieldIsEmpty, _FieldName))))
						{
							throw new PXRowPersistingException(_FieldName, null, ErrorMessages.FieldIsEmpty, _FieldName);
						}
					}
				}
			}
			catch (FiscalPeriodInactiveException ex)
			{
				if(RaiseErrorOnInactivePeriod && sender.RaiseExceptionHandling(_FieldName, e.Row, FormatForDisplay(NewValue), ex))
				{
					throw new PXRowPersistingException(_FieldName, FormatForDisplay(NewValue), ex.Message);
				}
			}
			catch (PXSetPropertyException ex)
			{
				if (sender.RaiseExceptionHandling(_FieldName, e.Row, FormatForDisplay(NewValue), ex))
				{
					throw new PXRowPersistingException(_FieldName, FormatForDisplay(NewValue), ex.Message);
				}
			}
		}

		protected virtual bool PostClosedPeriods(PXGraph graph)
		{
			GLSetup setup = (GLSetup)PXSelect<GLSetup>.Select(graph);
			if (setup != null)
			{
				return (bool)setup.PostClosedPeriods;
			}
			return false;
		}

		public virtual void IsValidPeriod(PXCache sender, object Row, object NewValue)
		{
			if (NewValue != null && _ValidatePeriod != PeriodValidation.Nothing)
			{
				FinPeriod p = (FinPeriod)PXSelect<FinPeriod, Where<FinPeriod.finPeriodID, Equal<Required<FinPeriod.finPeriodID>>>>.Select(sender.Graph, (string)NewValue);

				if (p == null)
				{
					throw new PXSetPropertyException(Messages.NoPeriodsDefined);
				}

				if (p.Active == false)
				{
					throw new FiscalPeriodInactiveException(p.FinPeriodID);
				}

				if (p.Closed == true)
				{
					if (PostClosedPeriods(sender.Graph))
					{
						sender.RaiseExceptionHandling(_FieldName, Row, null, new FiscalPeriodClosedException(p.FinPeriodID, PXErrorLevel.Warning));
						return;
					}
					else
					{
						throw new FiscalPeriodClosedException(p.FinPeriodID);
					}
				}
			}
		}
		#endregion
	}

	/// <summary>
	/// FinPeriod selector that extends <see cref="FinPeriodSelectorAttribute"/>. 
	/// Displays and accepts only Closed Fin Periods. 
	/// When Date is supplied through aSourceType parameter FinPeriod is defaulted with the FinPeriod for the given date.
	/// </summary>
	public class ClosedPeriodAttribute : FinPeriodSelectorAttribute
	{
		public ClosedPeriodAttribute(Type aSourceType)
			: base(typeof(Search<FinPeriod.finPeriodID, Where<FinPeriod.closed, Equal<True>, Or<FinPeriod.active, Equal<True>>>, OrderBy<Desc<FinPeriod.finPeriodID>>>), aSourceType)
		{
		}

		public ClosedPeriodAttribute()
			: this(null)
		{
		}
	}

	/// <summary>
	/// FinPeriod selector that extends <see cref="FinPeriodSelectorAttribute"/>. 
	/// Displays any periods (active, closed, etc), maybe filtered. 
	/// When Date is supplied through aSourceType parameter FinPeriod is defaulted with the FinPeriod for the given date.
	/// Default columns list includes 'Active' and  'Closed in GL' columns
	/// </summary>
	public class AnyPeriodFilterableAttribute : FinPeriodSelectorAttribute
	{
		protected int _SelAttrIndex = -1;
		public AnyPeriodFilterableAttribute(Type aSourceType)
			: base(typeof(Search3<FinPeriod.finPeriodID, OrderBy<Desc<FinPeriod.finPeriodID>>>), aSourceType, typeof(FinPeriod.finPeriodID),typeof(FinPeriod.descr),typeof(FinPeriod.active),typeof(FinPeriod.closed))
		{
			this.Initialize();
			this.Filterable = true;
		}

		public AnyPeriodFilterableAttribute(Type aSourceType, params Type[] fieldList)
			: base(typeof(Search3<FinPeriod.finPeriodID, OrderBy<Desc<FinPeriod.finPeriodID>>>), aSourceType, fieldList)
		{
			this.Initialize();
			this.Filterable = true;
		}

		public AnyPeriodFilterableAttribute(Type aSearchType, Type aSourceType, params Type[] fieldList)
			: base((aSearchType!=null?aSearchType: typeof(Search3<FinPeriod.finPeriodID, OrderBy<Desc<FinPeriod.finPeriodID>>>)),
				aSourceType, fieldList)
		{
			this.Initialize();
			this.Filterable = true;
		}

		public AnyPeriodFilterableAttribute()
			: this(null)
		{
			this.Initialize();
			this.Filterable = true;
		
		}

		public virtual bool Filterable
		{
			get
			{
				return (_SelAttrIndex == -1) ? false : ((PXSelectorAttribute)_Attributes[_SelAttrIndex]).Filterable;
			}
			set
			{
				if (_SelAttrIndex != -1)
					((PXSelectorAttribute)_Attributes[_SelAttrIndex]).Filterable = value;
			}
		}
		protected virtual void Initialize()
		{
			this._SelAttrIndex = -1;
			foreach (PXEventSubscriberAttribute attr in _Attributes)
			{
				if (attr is PXSelectorAttribute && _SelAttrIndex < 0)
				{
					_SelAttrIndex = _Attributes.IndexOf(attr);
				}				
			}
		}
	}

	/// <summary>
	/// This is a Generic Attribute that Aggregates other attributes and exposes there public properties.
	/// The Attributes aggregated can be of the following types:
	/// - DBFieldAttribute such as PXBDInt, PXDBString, etc.
	/// - PXUIFieldAttribute
	/// - PXSelectorAttribute
	/// - PXDefaultAttribute
	/// </summary>
	public class AcctSubAttribute : PXAggregateAttribute, IPXInterfaceField, IPXCommandPreparingSubscriber, IPXRowSelectingSubscriber
	{
		protected int _DBAttrIndex = -1;
		protected int _NonDBAttrIndex = -1;
		protected int _UIAttrIndex = -1;
		protected int _SelAttrIndex = -1;
		protected int _DefAttrIndex = -1;

		protected virtual void Initialize()
		{
			_DBAttrIndex = -1;
			_NonDBAttrIndex = -1;
			_UIAttrIndex = -1;
			_SelAttrIndex = -1;
			_DefAttrIndex = -1;

			foreach (PXEventSubscriberAttribute attr in _Attributes)
			{
				if (attr is PXDBFieldAttribute)
				{
					_DBAttrIndex = _Attributes.IndexOf(attr);
					foreach (PXEventSubscriberAttribute sibling in _Attributes)
					{
						if (!object.ReferenceEquals(attr, sibling) && PXAttributeFamilyAttribute.IsSameFamily(attr.GetType(), sibling.GetType()))
						{
							_NonDBAttrIndex = _Attributes.IndexOf(sibling);
							break;
						}
					}
				}
				if (attr is PXUIFieldAttribute)
				{
					_UIAttrIndex = _Attributes.IndexOf(attr);
				}
				if (attr is PXDimensionSelectorAttribute)
				{
					_SelAttrIndex = _Attributes.IndexOf(attr);
				}
				if (attr is PXSelectorAttribute && _SelAttrIndex < 0)
				{
					_SelAttrIndex = _Attributes.IndexOf(attr);
				}
				if (attr is PXDefaultAttribute)
				{
					_DefAttrIndex = _Attributes.IndexOf(attr);
				}
			} 
		}

		public AcctSubAttribute()
			: base()
		{
			Initialize();
			this.Filterable = true;
		}

		protected bool _IsDBField = true;
		public bool IsDBField
		{
			get
			{ 
				return this._IsDBField;
			}
			set
			{
				this._IsDBField = value;
			}
		}

		public new string FieldName
		{
			get
			{
				return (_DBAttrIndex == -1) ? null : ((PXDBFieldAttribute)_Attributes[_DBAttrIndex]).FieldName;
			}
			set
			{
				((PXDBFieldAttribute)_Attributes[_DBAttrIndex]).FieldName = value;
			}
		}

		public bool IsKey
		{
			get
			{
				return (_DBAttrIndex==-1) ? false : ((PXDBFieldAttribute)_Attributes[_DBAttrIndex]).IsKey;
			}
			set
			{
				((PXDBFieldAttribute)_Attributes[_DBAttrIndex]).IsKey = value;
			}
		}

		public bool IsFixed
		{
			get
			{
				return (_DBAttrIndex == -1) ? false : ((PXDBStringAttribute)_Attributes[_DBAttrIndex]).IsFixed;
			}
			set
			{
				((PXDBStringAttribute)_Attributes[_DBAttrIndex]).IsFixed = value;
				if (_NonDBAttrIndex != -1)
				{
					((PXStringAttribute)_Attributes[_NonDBAttrIndex]).IsFixed = value;
				}
			}
		}

		public PXUIVisibility Visibility
		{
			get
			{
				return (_UIAttrIndex==-1) ? PXUIVisibility.Undefined :((PXUIFieldAttribute)_Attributes[_UIAttrIndex]).Visibility;
			}
			set
			{
				if (_UIAttrIndex != -1)
					((PXUIFieldAttribute)_Attributes[_UIAttrIndex]).Visibility = value;
			}
		}

		public bool Visible
		{
			get
			{
				return (_UIAttrIndex == -1) ? true : ((PXUIFieldAttribute)_Attributes[_UIAttrIndex]).Visible;
			}
			set
			{
				if (_UIAttrIndex != -1)
					((PXUIFieldAttribute)_Attributes[_UIAttrIndex]).Visible = value;
			}
		}
		public bool Enabled
		{
			get
			{
				return (_UIAttrIndex == -1) ? true : ((PXUIFieldAttribute)_Attributes[_UIAttrIndex]).Enabled;
			}
			set
			{
				if (_UIAttrIndex != -1)
					((PXUIFieldAttribute)_Attributes[_UIAttrIndex]).Enabled = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return (_UIAttrIndex == -1) ? null : ((PXUIFieldAttribute)_Attributes[_UIAttrIndex]).DisplayName;
			}
			set
			{
				if (_UIAttrIndex != -1)
					((PXUIFieldAttribute)_Attributes[_UIAttrIndex]).DisplayName = value;
			}
		}

		public string FieldClass
		{
			get
			{
				return (_UIAttrIndex == -1) ? null : ((PXUIFieldAttribute)_Attributes[_UIAttrIndex]).FieldClass;
			}
			set
			{
				if (_UIAttrIndex != -1)
					((PXUIFieldAttribute)_Attributes[_UIAttrIndex]).FieldClass = value;
			}
		}

		public bool Required
		{
			get
			{
				return (_UIAttrIndex == -1) ? false : ((PXUIFieldAttribute)_Attributes[_UIAttrIndex]).Required;
			}
			set
			{
				if (_UIAttrIndex != -1)
					((PXUIFieldAttribute)_Attributes[_UIAttrIndex]).Required = value;
			}
		}

		public virtual Type DescriptionField
		{
			get
			{
				return (_SelAttrIndex == -1) ? null : ((PXDimensionSelectorAttribute)_Attributes[_SelAttrIndex]).DescriptionField;
			}
			set
			{
				if (_SelAttrIndex != -1)
					((PXDimensionSelectorAttribute)_Attributes[_SelAttrIndex]).DescriptionField = value;
			}
		}

		public virtual bool DirtyRead
		{
			get
			{
				return (_SelAttrIndex == -1) ? false : ((PXDimensionSelectorAttribute)_Attributes[_SelAttrIndex]).DirtyRead;
			}
			set
			{
				if (_SelAttrIndex != -1)
					((PXDimensionSelectorAttribute)_Attributes[_SelAttrIndex]).DirtyRead = value;
			}
		}

		public virtual bool CacheGlobal
		{
			get
			{
				return (_SelAttrIndex == -1) ? false : ((PXDimensionSelectorAttribute)_Attributes[_SelAttrIndex]).CacheGlobal;
			}
			set
			{
				if (_SelAttrIndex != -1)
					((PXDimensionSelectorAttribute)_Attributes[_SelAttrIndex]).CacheGlobal = value;
			}
		}

		public virtual bool ValidComboRequired
		{
			get
			{
				return (_SelAttrIndex == -1) ? false : ((PXDimensionSelectorAttribute)_Attributes[_SelAttrIndex]).ValidComboRequired;
			}
			set
			{
				if (_SelAttrIndex != -1)
					((PXDimensionSelectorAttribute)_Attributes[_SelAttrIndex]).ValidComboRequired = value;
			}
		}

		public Type BqlField 
		{
			get
			{
				return (_DBAttrIndex == -1) ? null : ((PXDBFieldAttribute)_Attributes[_DBAttrIndex]).BqlField;
			}
			set
			{
				((PXDBFieldAttribute)_Attributes[_DBAttrIndex]).BqlField = value;
				BqlTable = ((PXDBFieldAttribute)_Attributes[_DBAttrIndex]).BqlTable;
			}
		}

		public virtual bool Filterable
		{
			get
			{
				return (_SelAttrIndex == -1) ? false : ((PXDimensionSelectorAttribute)_Attributes[_SelAttrIndex]).Filterable;
			}
			set
			{
				if (_SelAttrIndex != -1)
					((PXDimensionSelectorAttribute)_Attributes[_SelAttrIndex]).Filterable = value;
			}
		}

		public virtual int TabOrder
		{
			get
			{
				return (_UIAttrIndex == -1) ? _FieldOrdinal : ((PXUIFieldAttribute)_Attributes[_UIAttrIndex]).TabOrder;
			}
			set
			{
				if (_UIAttrIndex != -1)
					((PXUIFieldAttribute)_Attributes[_UIAttrIndex]).TabOrder = value;
			}
		}

		public PXEventSubscriberAttribute SelectorAttr
		{
			get
			{
				return (_SelAttrIndex == -1) ? null : _Attributes[_SelAttrIndex];
			}
		}

		public virtual PXErrorHandling ErrorHandling
		{
			get
			{
				return (_UIAttrIndex == -1) ? PXErrorHandling.WhenVisible : ((PXUIFieldAttribute)_Attributes[_UIAttrIndex]).ErrorHandling;
			}
			set
			{
				if (_UIAttrIndex != -1)
					((PXUIFieldAttribute)_Attributes[_UIAttrIndex]).ErrorHandling = value;
			}
		}

		public virtual PXPersistingCheck PersistingCheck
		{
			get
			{
				return (_DefAttrIndex == -1) ? PXPersistingCheck.Nothing : ((PXDefaultAttribute)_Attributes[_DefAttrIndex]).PersistingCheck;
			}
			set
			{
				if (_DefAttrIndex != -1)
					((PXDefaultAttribute)_Attributes[_DefAttrIndex]).PersistingCheck = value;
			}
		}

		#region Implementation

		public virtual void CommandPreparing(PXCache sender, PXCommandPreparingEventArgs e)
		{
			e.FieldName = string.Empty;
			e.Cancel = true;
		}

	

		public virtual void RowSelecting(PXCache sender, PXRowSelectingEventArgs e)
		{
			sender.SetValue(e.Row, _FieldOrdinal, null);
		}

		
		#endregion

		#region Initialization
		public override void GetSubscriber<ISubscriber>(List<ISubscriber> subscribers)
		{
			if ((typeof(ISubscriber) == typeof(IPXCommandPreparingSubscriber)
				|| typeof(ISubscriber) == typeof(IPXRowSelectingSubscriber)))
			{
				if (_IsDBField == false)
				{
					if (_NonDBAttrIndex == -1)
					{
					subscribers.Add(this as ISubscriber);
					}
					else
					{
						if (typeof(ISubscriber) == typeof(IPXRowSelectingSubscriber))
						{
							subscribers.Add(this as ISubscriber);
						}
						else
						{
							_Attributes[_NonDBAttrIndex].GetSubscriber<ISubscriber>(subscribers);
						}
					}
					for (int i = 0; i < _Attributes.Count; i++)
					{
						if (i != _DBAttrIndex && i != _NonDBAttrIndex)
						{
							_Attributes[i].GetSubscriber<ISubscriber>(subscribers);
						}
					}
				}
				else
				{
					base.GetSubscriber<ISubscriber>(subscribers);

					if (_NonDBAttrIndex != -1)
					{
						subscribers.Remove(_Attributes[_NonDBAttrIndex] as ISubscriber);
					}

					subscribers.Remove(this as ISubscriber);
				}
			}
			else
			{
				base.GetSubscriber<ISubscriber>(subscribers);

				if (_NonDBAttrIndex != -1)
				{
					subscribers.Remove(_Attributes[_NonDBAttrIndex] as ISubscriber);
				}
			}
		}
		#endregion

		#region IPXInterfaceField Members


		public string ErrorText
		{
			get
			{
				return (_UIAttrIndex != -1) ? ((IPXInterfaceField)_Attributes[_UIAttrIndex]).ErrorText: null;
			}
			set
			{
				if (_UIAttrIndex != -1) 
					((IPXInterfaceField)_Attributes[_UIAttrIndex]).ErrorText = value;				
			}
		}

		public object ErrorValue
		{
			get
			{
				return (_UIAttrIndex != -1) ? ((IPXInterfaceField)_Attributes[_UIAttrIndex]).ErrorValue : null;
			}
			set
			{
				if (_UIAttrIndex != -1) 
					((IPXInterfaceField)_Attributes[_UIAttrIndex]).ErrorValue = value;				
			}			
		}

		public PXErrorLevel ErrorLevel
		{
			get
			{
				return (_UIAttrIndex != -1) ? ((IPXInterfaceField)_Attributes[_UIAttrIndex]).ErrorLevel : PXErrorLevel.Undefined;
			}
			set
			{
				if (_UIAttrIndex != -1)
					((IPXInterfaceField)_Attributes[_UIAttrIndex]).ErrorLevel = value;
			}
		}

		public void ForceEnabled()
		{
			if(_UIAttrIndex != -1) 
			((IPXInterfaceField)_Attributes[_UIAttrIndex]).ForceEnabled();
		}

		public PXCacheRights MapEnableRights
		{
			get
			{
				return (_UIAttrIndex != -1) ? ((IPXInterfaceField)_Attributes[_UIAttrIndex]).MapEnableRights : PXCacheRights.Select;
			}
			set
			{
				if (_UIAttrIndex != -1)
					((IPXInterfaceField)_Attributes[_UIAttrIndex]).MapEnableRights = value;	
			}
		}

		public PXCacheRights MapViewRights
		{
			get
			{
				return (_UIAttrIndex != -1) ? ((IPXInterfaceField)_Attributes[_UIAttrIndex]).MapViewRights : PXCacheRights.Select;
			}
			set
			{
				if (_UIAttrIndex != -1)
					((IPXInterfaceField)_Attributes[_UIAttrIndex]).MapViewRights = value;	
			}
		}

		public bool ViewRights
		{
			get
			{
				return (_UIAttrIndex != -1) ? ((IPXInterfaceField)_Attributes[_UIAttrIndex]).ViewRights : true;
			}
		}

		#endregion

	}

	/// <summary>
	/// Consol. Branch Field.
	/// </summary>
	/// <remarks></remarks>
	public class ConsolBranchAttribute : BranchAttribute
	{
		public ConsolBranchAttribute()
			:base(null, typeof(Search<Branch.branchID, Where<Branch.ledgerID, IsNull, Or<Branch.branchID, Equal<Current<Ledger.defBranchID>>>>>))
		{
			this.IsDetail = false;
		}
	}

	/// <summary>
	/// Branch Field.
	/// </summary>
	/// <remarks>In case your DAC  supports multiple branches add this attribute to the Branch field of your DAC.</remarks>
	[PXDBInt()]
	[PXInt]
	[PXUIField(DisplayName = "Branch", FieldClass = _FieldClass)]
	[PXRestrictor(typeof(Where<Branch.active, Equal<True>>), Messages.BranchInactive)]
	public class BranchAttribute : AcctSubAttribute, IPXFieldSelectingSubscriber
	{
		public const string _FieldClass = "BRANCH";
		public const string _DimensionName = "BIZACCT";
		private bool _IsDetail = true;
		private bool _Suppress = false;

		public bool IsDetail
		{
			get
			{
				return this._IsDetail;
			}
			set
			{
				this._IsDetail = value;
			}
		}

		public bool IsEnabledWhenOneBranchIsAccessible { get; set; }

		public BranchAttribute(Type sourceType)
			: this(sourceType, typeof(Search2<Branch.branchID, LeftJoin<Ledger, On<Ledger.defBranchID, Equal<Branch.branchID>>>, Where2<MatchWithBranch<Branch.branchID>, And<Ledger.ledgerID, IsNull>>>))
		{
			_Attributes.Add(sourceType != null ? new PXDefaultAttribute(sourceType) : new PXDefaultAttribute());

			Initialize();
		}

		protected BranchAttribute(Type sourceType, Type searchType)
			: base()
		{
			if (sourceType == null || !typeof(IBqlField).IsAssignableFrom(sourceType) || sourceType == typeof(AccessInfo.branchID))
			{
				IsDetail = false;
			}

			if (IsDetail)
			{
				_Attributes.Add(new PXRestrictorAttribute(BqlCommand.Compose(
						typeof(Where2<,>), 
						typeof(SameLedgerBranch<,>), 
						typeof(Branch.branchID), 
						typeof(Current<>),sourceType,
						typeof(Or<>),typeof(FeatureInstalled<FeaturesSet.interBranch>)),
						Messages.InterBranchFeatureIsDisabled));
			}

			PXDimensionSelectorAttribute attr = new PXDimensionSelectorAttribute(_DimensionName, searchType, typeof(Branch.branchCD));
			attr.ValidComboRequired = true;
			attr.DescriptionField = typeof(Branch.acctName);
			_Attributes.Add(attr);
			_SelAttrIndex = _Attributes.Count - 1;

			Initialize();
		}

		public BranchAttribute()
			:this(typeof(AccessInfo.branchID))
		{
		}

		public bool Suppress()
		{
			object[] ids = PXAccess.GetBranches();

			return (ids == null || ids.Length <= 1) && !IsEnabledWhenOneBranchIsAccessible;
		}

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);

			_Suppress = Suppress();
		}

		public override void GetSubscriber<ISubscriber>(List<ISubscriber> subscribers)
		{
			base.GetSubscriber<ISubscriber>(subscribers);
			if (typeof(ISubscriber) == typeof(IPXFieldSelectingSubscriber))
			{
				subscribers.Remove(this as ISubscriber);
				subscribers.Add(this as ISubscriber);
			}
		}

		public virtual void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			if (_Suppress && e.ReturnState is PXFieldState)
			{
				PXFieldState state = (PXFieldState)e.ReturnState;

				state.Enabled = false;
				if (_IsDetail)
				{
					state.Visible = false;
					state.Visibility = PXUIVisibility.Invisible;
				}
			}
		}
	}


	/// <summary>
	/// Super-Branch Attribute for Consolidated reporting.
	/// </summary>
	/// <remarks>Use in report parameters when you need Super-Branches only.</remarks>
	public class MasterBranchAttribute : MasterBranchBaseAttribute
	{
		public MasterBranchAttribute() : base(typeof(Search2<Branch.branchID,
				InnerJoin<Ledger, On<Ledger.ledgerID, Equal<Branch.ledgerID>>>,
				Where2<
					Where2<Not<FeatureInstalled<FeaturesSet.branch>>,
						Or2<IsSingleBranchCompany,
						Or<Ledger.postInterCompany, Equal<True>, And<Ledger.defBranchID, NotEqual<Branch.branchID>,
						Or<Ledger.postInterCompany, Equal<True>, And<Ledger.defBranchID, IsNull,
						Or<Ledger.postInterCompany, Equal<False>, And<Ledger.defBranchID, Equal<Branch.branchID>>>>>>>>>,
					And<MatchWithBranch<Branch.branchID>>>>)) {}
	}

	public class MasterBranch1099Attribute : MasterBranchBaseAttribute, IPXFieldDefaultingSubscriber
	{
		public MasterBranch1099Attribute() : base(typeof(Search2<Branch.branchID,
				InnerJoin<Ledger, On<Ledger.ledgerID, Equal<Branch.ledgerID>>>,
				Where2<
					Where2<Not<FeatureInstalled<FeaturesSet.branch>>,
						Or2<IsSingleBranchCompany,
						Or<Ledger.defBranchID, IsNull,
						Or<Ledger.defBranchID, Equal<Branch.branchID>>>>>,
					And<MatchWithBranch<Branch.branchID>>>>))
		{
			_Attributes.RemoveAt(_DefAttrIndex);
			_DefAttrIndex = -1;
		}


		public void FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			PXResultset<Ledger> results = PXSelectJoin<Ledger,
				LeftJoin<Branch, On<Branch.ledgerID, Equal<Ledger.ledgerID>>>,
				Where<Branch.branchID, Equal<Required<AccessInfo.branchID>>>>
				.SelectWindowed(sender.Graph, 0, 1, sender.Graph.Accessinfo.BranchID);
			if (results.Count > 0)
			{
				e.NewValue = ((Ledger)results[0]).DefBranchID ?? sender.Graph.Accessinfo.BranchID;
			}
		}
	}

	public class MasterBranch1099EFileAttribute : MasterBranchBaseAttribute, IPXFieldDefaultingSubscriber
	{
		public MasterBranch1099EFileAttribute() : base(typeof(Search2<Branch.branchID,
			InnerJoin<Ledger, On<Ledger.ledgerID, Equal<Branch.ledgerID>>>,
			Where2<
				Where2<Not<FeatureInstalled<FeaturesSet.branch>>,
					Or2<IsSingleBranchCompany,
					Or<Ledger.defBranchID, IsNull,
					Or<Ledger.defBranchID, Equal<Branch.branchID>>>>>,
				And<Branch.reporting1099, Equal<True>,
				And<MatchWithBranch<Branch.branchID>>>>>))
		{
			_Attributes.RemoveAt(_DefAttrIndex);
			_DefAttrIndex = -1;
		}

		public void FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			foreach (PXResult<Ledger, Branch, BranchAlias> res in PXSelectJoin<Ledger,
				LeftJoin<Branch, On<Branch.ledgerID, Equal<Ledger.ledgerID>>,
					LeftJoin<BranchAlias, On<Ledger.defBranchID, Equal<BranchAlias.branchID>>>>,
				Where<Branch.branchID, Equal<Required<AccessInfo.branchID>>>>
				.SelectSingleBound(sender.Graph, new object[]{}, sender.Graph.Accessinfo.BranchID))
			{
				Branch currentBranch = res;
				BranchAlias consolidationBranch = res;
                e.NewValue = (consolidationBranch.Reporting1099 == true ? consolidationBranch.BranchID : null) ?? (currentBranch.Reporting1099 == true ? currentBranch.BranchID : null);
				break;
			}
		}
	}

	[PXDBInt]
	[PXInt]
	[PXUIField(DisplayName = "Branch", FieldClass = _FieldClass, Required = true)]
	[PXRestrictor(typeof(Where<Branch.active, Equal<True>>), Messages.BranchInactive)]
	public class MasterBranchBaseAttribute : AcctSubAttribute, IPXFieldSelectingSubscriber
	{
		public const string _FieldClass = "BRANCH";
		public const string _DimensionName = "BIZACCT";
		private bool _Suppress;
		private bool _isDetail = true;

		public bool IsDetail
		{
			get { return _isDetail; }
			set { _isDetail = value; }
		}

		public MasterBranchBaseAttribute(Type search)
		{
			PXDimensionSelectorAttribute attr = new PXDimensionSelectorAttribute(_DimensionName, search, typeof(Branch.branchCD))
			{
				ValidComboRequired = true,
				DescriptionField = typeof(Branch.acctName)
			};
			_Attributes.Add(attr);
			_SelAttrIndex = _Attributes.Count - 1;

			_Attributes.Add(new PXDefaultAttribute(search));
			_DefAttrIndex = _Attributes.Count - 1;

			Initialize();
		}

		public static bool Suppress()
		{
			int[] ids = PXAccess.GetBranchIDs();

			return (ids == null || ids.Length <= 1);
		}

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);

			_Suppress = false;
		}

		public override void GetSubscriber<ISubscriber>(List<ISubscriber> subscribers)
		{
			base.GetSubscriber<ISubscriber>(subscribers);
			if (typeof(ISubscriber) == typeof(IPXFieldSelectingSubscriber))
			{
				subscribers.Remove(this as ISubscriber);
				subscribers.Add(this as ISubscriber);
			}
		}

		public virtual void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			if (_Suppress && e.ReturnState is PXFieldState)
			{
				PXFieldState state = (PXFieldState)e.ReturnState;

				state.Enabled = false;
				if (IsDetail)
				{
					state.Visible = false;
					state.Visibility = PXUIVisibility.Invisible;
				}
			}
		}
	}

	/// <summary>
	/// Base Attribute for AccountCD field. Aggregates PXFieldAttribute, PXUIFieldAttribute and PXDimensionAttribute.
	/// PXDimensionAttribute selector has no restrictions and returns all records.
	/// </summary>
	[PXDBString(10, IsUnicode = true, InputMask = "")]
	[PXUIField(DisplayName = "Account", Visibility = PXUIVisibility.Visible)]
	public sealed class AccountRawAttribute : AcctSubAttribute
	{
		private string _DimensionName = "ACCOUNT";

		public AccountRawAttribute()
			: base()
		{
			PXDimensionAttribute attr = new PXDimensionAttribute(_DimensionName);
			attr.ValidComboRequired = false;
			_Attributes.Add(attr);
			_SelAttrIndex = _Attributes.Count - 1;
		}
	}

	/// <summary>
	/// Base Attribute for SubCD field. Aggregates PXFieldAttribute, PXUIFieldAttribute and PXDimensionAttribute.
	/// PXDimensionAttribute selector has no restrictions and returns all records.
	/// </summary>
	[PXDBString(30, IsUnicode = true, InputMask = "")]
	[PXUIField(DisplayName = "Subaccount", Visibility = PXUIVisibility.Visible, FieldClass = _DimensionName)]
	public class SubAccountRawAttribute : AcctSubAttribute
	{
		protected const string _DimensionName = "SUBACCOUNT";

		public SubAccountRawAttribute()
			: base()
		{
			PXDimensionAttribute attr = new PXDimensionAttribute(_DimensionName);
			attr.ValidComboRequired = false;
			_Attributes.Add(attr);
			_SelAttrIndex = _Attributes.Count - 1;			
		}

		protected bool _SuppressValidation = false;
		public bool SuppressValidation
		{
			get
			{
				return this._SuppressValidation;
			}
			set
			{
				this._SuppressValidation = value;
			}
		}
		public override void GetSubscriber<ISubscriber>(List<ISubscriber> subscribers) 
		{
			base.GetSubscriber<ISubscriber>(subscribers);
			if (this._SuppressValidation)
			{
				if (typeof(ISubscriber) == typeof(IPXFieldVerifyingSubscriber) && (_SelAttrIndex >= 0))
				{
					subscribers.Remove(_Attributes[_SelAttrIndex] as ISubscriber);
				}
			}
		} 
	}

	/// <summary>
	/// Base Attribute for SubCD field. Aggregates PXFieldAttribute, PXUIFieldAttribute and PXDimensionAttribute.
	/// PXDimensionAttribute selector returns only records that are visible for the current user.
	/// </summary>
	[PXDBString(30, InputMask = "")]
	[PXUIField(DisplayName = "Subaccount", Visibility = PXUIVisibility.Visible, FieldClass = SubAccountAttribute.DimensionName)]
	public sealed class SubAccountRestrictedRawAttribute : SubAccountRawAttribute, IPXFieldSelectingSubscriber
	{
		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);
			PX.SM.Users user = PXSelect<PX.SM.Users,
				Where<PX.SM.Users.username, Equal<Current<AccessInfo.userName>>>>
				.Select(sender.Graph);
			if (user != null && user.GroupMask != null)
			{
				((PXDimensionAttribute)_Attributes[_SelAttrIndex]).Restrictions = new GroupHelper.ParamsPair[][] { GroupHelper.GetParams(typeof(PX.SM.Users), typeof(CS.SegmentValue), user.GroupMask) };
			}
			sender.Graph.Views["_" + _DimensionName + "_RestrictedSegments_"] =
				new PXView(sender.Graph, true, new Select<PXDimensionAttribute.SegmentValue>(), (PXSelectDelegate<short?>)((PXDimensionAttribute)_Attributes[_SelAttrIndex]).SegmentSelect);
		}
		public void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			if (e.ReturnState is PXFieldState)
			{
				((PXFieldState)e.ReturnState).ViewName = "_" + _DimensionName + "_RestrictedSegments_";
			}
		}
		public override void GetSubscriber<ISubscriber>(List<ISubscriber> subscribers)
		{
			base.GetSubscriber<ISubscriber>(subscribers);
			if (typeof(ISubscriber) == typeof(IPXFieldSelectingSubscriber))
			{
				subscribers.Remove(this as ISubscriber);
				subscribers.Add(this as ISubscriber);
			}			
		}
		
	}

	/// <summary>
	/// Represents CashAccount Field with Selector that shows all Cash Accounts.
	/// </summary>
	[PXDBInt()]
		[PXUIField(DisplayName = "Cash Account", Visibility = PXUIVisibility.Visible, FieldClass = DimensionName)]
	public class CashAccountAttribute : AcctSubAttribute
	{			
		private Type[] _selectorCols = new Type[] 
			{
				typeof(CashAccount.cashAccountCD),
				typeof(CashAccount.accountID),
				typeof(CashAccount.descr),
				typeof(CashAccount.curyID),
				typeof(CashAccount.subID),
				typeof(CashAccount.branchID)
			};

		private Type _branchID;
		private Type _search;
		private bool? _filterBranch;
		
		[Flags]
		private enum searchParamTypes
		{
			Join = 0x01,
			Where = 0x02,
			Aggregate = 0x04,
			OrderBy = 0x08
		}

		private enum searchTypes
		{
			Search23, Search24, Search54, Search55
		}

		private Type generateNewSearch(Type[] searchArgs, Type branchID, bool? filterBranch)
		{
			if (branchID == null)
			{
				branchID = typeof(AccessInfo.branchID);
			}
			searchParamTypes argsTypes = 0;
			searchTypes searchType = 0;
			var typesDict = new Dictionary<Type, Type>();
			foreach (Type arg in searchArgs)
			{
				if (typeof(IBqlJoin).IsAssignableFrom(arg))
				{
					typesDict[typeof(IBqlJoin)] = arg;
					argsTypes |= searchParamTypes.Join;
				} else if (typeof(IBqlWhere).IsAssignableFrom(arg))
				{
					typesDict[typeof(IBqlWhere)] = arg;
					argsTypes |= searchParamTypes.Where;
				} else if (typeof(IBqlAggregate).IsAssignableFrom(arg))
				{
					typesDict[typeof(IBqlAggregate)] = arg;
					argsTypes |= searchParamTypes.Aggregate;
				} else if (typeof(IBqlOrderBy).IsAssignableFrom(arg))
				{
					typesDict[typeof(IBqlOrderBy)] = arg;
					argsTypes |= searchParamTypes.OrderBy;
				}
			}
			Type newSearch = null;
			Type newSearchType = null;
			switch (argsTypes & (searchParamTypes.Aggregate | searchParamTypes.OrderBy))
			{
				case searchParamTypes.Aggregate:
					newSearchType = typeof (Search5<,,,>);
					searchType = searchTypes.Search54;
					break;
				case searchParamTypes.OrderBy:
					newSearchType = typeof (Search2<,,,>);
					searchType = searchTypes.Search24;
					break;
				case (searchParamTypes.Aggregate | searchParamTypes.OrderBy):
					newSearchType = typeof (Search5<,,,,>);
					searchType = searchTypes.Search55;
					break;
				case 0:
					newSearchType = typeof (Search2<,,>);
					searchType = searchTypes.Search23;
					break;
			}
			if (newSearchType != null)
			{
				if (typesDict.ContainsKey(typeof(IBqlJoin)))
				{
					typesDict[typeof(IBqlJoin)] = BqlCommand.Compose(
						typeof(InnerJoin<,,>),
						typeof(Account),
						typeof(On<,,>),
						typeof(Account.accountID), typeof(Equal<CashAccount.accountID>),
						typeof(And2<,>),
						typeof(Match<Account, Current<AccessInfo.userName>>),
						typeof(And<>),
						typeof(Match<,>),
						typeof(Account),
						typeof(Optional<>), branchID,
						typeof(InnerJoin<,,>),
						typeof(Sub),
						typeof(On<Sub.subID, Equal<CashAccount.subID>, And<Match<Sub, Current<AccessInfo.userName>>>>),
						typesDict[typeof(IBqlJoin)]);
				}
				else
				{
					typesDict[typeof(IBqlJoin)] = typeof(InnerJoin<Account,
						On<Account.accountID, Equal<CashAccount.accountID>, And<Match<Account, Current<AccessInfo.userName>>>>,
						InnerJoin<Sub, On<Sub.subID, Equal<CashAccount.subID>, And<Match<Sub, Current<AccessInfo.userName>>>>>>);
				}
				List<Type> args = new List<Type>();
				args.AddRange(new Type[] {typeof(Where2<,>),
						typeof(Not<>),
						typeof(FeatureInstalled<FeaturesSet.interBranch>),
						typeof(Or<,,>), typeof(CashAccount.restrictVisibilityWithBranch), typeof(Equal<>), typeof(False),
						typeof(Or<,>), typeof(CashAccount.branchID)});
				args.AddRange(filterBranch == true ? new Type[] {typeof (Equal<>), typeof (Current<>), branchID} : new Type[] {typeof (IsNotNull)});
				if (typesDict.ContainsKey(typeof (IBqlWhere)))
				{
					args.Insert(0, typeof(Where2<,>));
					args.AddRange(new Type[] {typeof (And<>), typesDict[typeof (IBqlWhere)]});
				}
				typesDict[typeof(IBqlWhere)] = BqlCommand.Compose(args.ToArray());
				var newSearchList = new List<Type> { newSearchType, typeof(CashAccount.cashAccountID), typesDict[typeof(IBqlJoin)], typesDict[typeof(IBqlWhere)] };
				if (searchType == searchTypes.Search54 || searchType == searchTypes.Search55)
				{
					newSearchList.Add(typesDict[typeof(IBqlAggregate)]);
				}
				if (searchType == searchTypes.Search24 || searchType == searchTypes.Search55)
				{
					newSearchList.Add(typesDict[typeof(IBqlOrderBy)]);
				}
				newSearch = BqlCommand.Compose(newSearchList.ToArray());
			}
			return newSearch;
		}

		public const string DimensionName = "CASHACCOUNT";

		public bool SuppressCurrencyValidation
		{
			get { return _SuppressCurrencyValidation; }
			set { _SuppressCurrencyValidation = value; }
		}

		private bool _SuppressCurrencyValidation;


		public CashAccountAttribute()
		{
			_filterBranch = false;
			_branchID = null;
			_search = typeof(Search<CashAccount.cashAccountID>);
			InitAttribute();
		}

		public CashAccountAttribute(Type search)
		{
			_filterBranch = false;
			_branchID = null;
			_search = search;
			InitAttribute();
		}

		public CashAccountAttribute(Type branchID, Type search)
		{
			_filterBranch = true;
			_branchID = branchID;
			_search = search;
			InitAttribute();
		}

		private void InitAttribute()
		{
			_search = _search ?? typeof(Search<CashAccount.cashAccountID>);

			Type searchCommand = generateNewSearch(_search.GetGenericArguments(), _branchID, _filterBranch);

			if (searchCommand == null)
			{
				throw new PXArgumentException("search", ErrorMessages.ArgumentException);
			}

			PXDimensionSelectorAttribute dimensionSelectorAttribute = new PXDimensionSelectorAttribute(
				DimensionName, 
				searchCommand, 
				typeof(CashAccount.cashAccountCD),
				_selectorCols);

			dimensionSelectorAttribute.CacheGlobal = true;
			dimensionSelectorAttribute.DescriptionField = typeof(CashAccount.descr);

			_Attributes.Add(dimensionSelectorAttribute);
			_SelAttrIndex = _Attributes.Count - 1;

			this.Filterable = true;
		}

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);
			if (_branchID != null)
				sender.Graph.FieldUpdated.AddHandler(BqlCommand.GetItemType(_branchID), _branchID.Name, BranchFieldUpdated);
		}

		protected virtual void BranchFieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			PXFieldState state = (PXFieldState)sender.GetValueExt(e.Row, _FieldName);
			if (state != null && state.Value != null)
			{
				sender.SetValue(e.Row, _FieldName, null);
				sender.SetValueExt(e.Row, _FieldName, state.Value);
			}
		}
	}

	[PXDBString(10, IsUnicode = true, InputMask = "")]
		[PXUIField(DisplayName = "Cash Account", Visibility = PXUIVisibility.Visible, FieldClass = _DimensionName)]
	public sealed class CashAccountRawAttribute : AcctSubAttribute
	{
		private const string _DimensionName = "CASHACCOUNT";

		public CashAccountRawAttribute()
			: base()
		{
			Type SearchType = typeof(Search2<CashAccount.cashAccountCD,
		InnerJoin<Account, On<Account.accountID, Equal<CashAccount.accountID>,
		And2<Match<Account, Current<AccessInfo.userName>>, And<Match<Account, Current<AccessInfo.branchID>>>>>,
		InnerJoin<Sub, On<Sub.subID, Equal<CashAccount.subID>, And<Match<Sub, Current<AccessInfo.userName>>>>>>>); 
			PXDimensionSelectorAttribute attr = new PXDimensionSelectorAttribute(_DimensionName, SearchType);
			attr.CacheGlobal = true;
			attr.DescriptionField = typeof(CashAccount.descr);
			_Attributes.Add(attr);
			_SelAttrIndex = _Attributes.Count - 1;
		}
	}

		[PXUIField(DisplayName = "Account", Visibility = PXUIVisibility.Visible, FieldClass = DimensionName)]
	public class UnboundAccountAttribute : AcctSubAttribute
	{
		public const string DimensionName = "ACCOUNT";

		public UnboundAccountAttribute()
			: base()
		{
			Type SearchType = typeof(Search<Account.accountID,
				Where2<Match<Current<AccessInfo.userName>>,
				And<Where<Current<GLSetup.ytdNetIncAccountID>, IsNull,
				Or<Account.accountID, NotEqual<Current<GLSetup.ytdNetIncAccountID>>>>>>>);
			PXDimensionSelectorAttribute attr = new PXDimensionSelectorAttribute(DimensionName, SearchType, typeof(Account.accountCD));
			attr.CacheGlobal = true;
			_Attributes.Add(attr);
			_SelAttrIndex = _Attributes.Count - 1;
			DescriptionField = typeof(Account.description);
			this.Filterable = true;
		}
	}	
	

	/// <summary>
	/// Represents Account Field
	/// The Selector will return all accounts.
	/// This attribute also tracks currency (which is supplied as curyField parameter) for the Account and
	/// raises an error in case Denominated GL Account currency is different from transaction currency.
	/// </summary>
	[PXRestrictor(typeof(Where<True, Equal<True>>), Messages.AccountInactive, ReplaceInherited = true)]
	public class AccountAnyAttribute : AccountAttribute
	{		
		public AccountAnyAttribute()
			: base(null) 
		{
			this.Filterable = true;
		}

		public override void Verify(PXCache sender, Account item, object row)
		{
		}
	}

	/// <summary>
	/// Represents Account Field.
	/// The Selector will return all accounts except 'YTD Net Income' account.
	/// This attribute also tracks currency (which is supplied as curyField parameter) for the Account and
	/// raises an error in case Denominated GL Account currency is different from transaction currency.
	/// </summary>
	[PXUIField(DisplayName = "Account", Visibility = PXUIVisibility.Visible, FieldClass = DimensionName)]	
	[PXDBInt]		
	[PXInt]
	[PXRestrictor(typeof(Where<Account.active, Equal<True>>), Messages.AccountInactive)]
	[PXRestrictor(typeof(Where<Where<Current<GLSetup.ytdNetIncAccountID>, IsNull,
							Or<Account.accountID, NotEqual<Current<GLSetup.ytdNetIncAccountID>>>>>), Messages.YTDNetIncomeSelected)]
	public class AccountAttribute : AcctSubAttribute, IPXFieldVerifyingSubscriber, IPXRowPersistingSubscriber
	{	
		public class CuryException : PXSetPropertyException
		{
			public CuryException()
				: base(Messages.AccountCuryNotTransactionCury)
			{ 
			}

			public CuryException(SerializationInfo info, StreamingContext context)
				: base(info, context)
			{
				PXReflectionSerializer.RestoreObjectProps(this, info);
			}

			public override void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				PXReflectionSerializer.GetObjectData(this, info);
				base.GetObjectData(info, context);
			}


		}


		public const string DimensionName = "ACCOUNT";

		private const string glSetup = "_GLSetup";

		public class dimensionName : Constant<string>
		{
			public dimensionName() : base(DimensionName) { ;}
		}

		public bool SuppressCurrencyValidation
		{
			get { return _SuppressCurrencyValidation; }
			set { _SuppressCurrencyValidation = value; }
		}

		private bool _SuppressCurrencyValidation;

		public Type  LedgerID
		{
			get { return _ledgerID; }
			set { _ledgerID = value; }
		}

		private Type _ledgerID;
		private Type _branchID;
		
		private Type _curyKeyField;
		
		public AccountAttribute()
			: this(null)
		{			
		}

		public AccountAttribute(Type branchID)
			: this(branchID, typeof(Search<Account.accountID, Where<Match<Current<AccessInfo.userName>>>>))
		{			
		}

		public AccountAttribute(Type branchID, Type SearchType)
		{
			if(SearchType == null)
			{
				throw new PXArgumentException("SearchType", ErrorMessages.ArgumentNullException);
			}

			if (branchID != null)
			{
				_branchID = branchID; 

				List<Type> items = new List<Type> { SearchType.GetGenericTypeDefinition() };
				items.AddRange(SearchType.GetGenericArguments());
				for (int i = 0; i < items.Count; i++)
				{
					if (typeof(IBqlWhere).IsAssignableFrom(items[i]))
					{
						items[i] = BqlCommand.Compose(
							typeof(Where2<,>),
                            typeof(Where<,,>),
                            typeof(Account.isCashAccount), typeof(Equal<True>), typeof(Or<>),
							typeof(Match<>), typeof(Optional<>), branchID,
							typeof(And<>),
							items[i]);
						SearchType = BqlCommand.Compose(items.ToArray());
					}
				}
			}

			PXDimensionSelectorAttribute attr = new PXDimensionSelectorAttribute(DimensionName, SearchType, typeof(Account.accountCD));
			attr.CacheGlobal = true;
			attr.DescriptionField = typeof(Account.description);
			_Attributes.Add(attr);
			_SelAttrIndex = _Attributes.Count - 1;
			this.Filterable = true;
		}

		private static Type SearchKeyField(PXCache sender)
		{
			for (int i = 0; i < sender.BqlFields.Count; i++)
				if (String.Compare(sender.BqlFields[i].Name, "CuryInfoID", true) == 0)
					return sender.BqlFields[i];
			return null;
		}

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);
			sender.Graph.FieldSelecting.AddHandler(sender.GetItemType(), _FieldName, FieldSelecting);				
			_curyKeyField = SuppressCurrencyValidation ? null : SearchKeyField(sender);

			if (!PXAccess.FeatureInstalled<FeaturesSet.financialModule>())
			{
				PXDimensionSelectorAttribute.SetValidCombo(sender, _FieldName, false);
				if (!sender.Graph.Views.Caches.Contains(typeof(Account)))
					sender.Graph.Views.Caches.Add(typeof(Account));

				sender.Graph.FieldDefaulting.AddHandler(sender.GetItemType(), _FieldName, Feature_FieldDefaulting);
			}
			else
			{
				if (_curyKeyField != null)
				{
					sender.Graph.RowUpdated.AddHandler<CurrencyInfo>(CurrencyInfoRowUpdated);
				}

				if (_branchID != null)
					sender.Graph.FieldUpdated.AddHandler(BqlCommand.GetItemType(_branchID), _branchID.Name, BranchFieldUpdated);


				if (((PXDimensionSelectorAttribute) _Attributes[_SelAttrIndex]).CacheGlobal == false)
				{
					sender.Graph.FieldUpdating.AddHandler(sender.GetItemType(), _FieldName, FieldUpdating);
					sender.Graph.FieldUpdating.RemoveHandler(sender.GetItemType(), _FieldName, ((PXDimensionSelectorAttribute) _Attributes[_SelAttrIndex]).FieldUpdating);
				}
			}
		}

		public virtual void Feature_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (!e.Cancel)
			{
				if (this.Definitions.DefaultAccountID == null)
				{					
					object newValue = Account.Default;
					sender.RaiseFieldUpdating(_FieldName, e.Row, ref newValue);
					e.NewValue = newValue;										
				}
				else
				{
					e.NewValue = this.Definitions.DefaultAccountID;
				}

				e.Cancel = true;
			}
		}

		public virtual void Verify(PXCache sender, Account item, object row)
		{
			if (_curyKeyField != null)
			{
				if (item != null && item.CuryID != null)
				{
					string CuryID;
					using (new PXCuryViewStateScope(sender.Graph))
					{
						object value = sender.GetValueExt(row, typeof(CurrencyInfo.curyID).Name);
						CuryID = value is PXFieldState ? ((PXFieldState)value).Value.ToString().TrimEnd() : (string)value;
					}

					if (!object.Equals(item.CuryID, CuryID))
					{
						string Ledger_BalanceType = "A";

						if (_ledgerID != null)
						{
							int ledgerID = (int)PXView.FieldGetValue(sender, row, BqlCommand.GetItemType(_ledgerID), _ledgerID.Name);
							Ledger ledger = PXSelect<Ledger, Where<Ledger.ledgerID, Equal<Required<Ledger.ledgerID>>>>.Select(sender.Graph, ledgerID);

							if (ledger != null)
							{
								Ledger_BalanceType = ledger.BalanceType;
							}
						}

						if (Ledger_BalanceType != "R")
						{
							throw new CuryException();
						}
					}
				}
			}
		}

		public virtual void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			PXView glSetupView = new PXView(sender.Graph, false, BqlCommand.CreateInstance(typeof(Select<GLSetup>)));
			if (!sender.Graph.Views.ContainsKey(glSetup))
				sender.Graph.Views.Add(glSetup, glSetupView);
			if (glSetupView.Cache.Current == null)
				glSetupView.Cache.Current = (GLSetup)PXSelect<GLSetup>.SelectWindowed(sender.Graph, 0, 1);
		}

		public virtual void FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
		{
			if (!e.Cancel && e.NewValue != null)
			{
				PXFieldUpdating fu = ((PXDimensionSelectorAttribute)_Attributes[_SelAttrIndex]).GetAttribute<PXDimensionAttribute>().FieldUpdating;
				fu(sender, e);
				e.Cancel = false;

				Account item = PXSelect<Account, Where2<Match<Current<AccessInfo.userName>>, And<Account.accountCD, Equal<Required<Account.accountCD>>>>>.Select(sender.Graph, e.NewValue);

				if (item == null)
				{
					fu = ((PXDimensionSelectorAttribute)_Attributes[_SelAttrIndex]).GetAttribute<PXSelectorAttribute>().SubstituteKeyFieldUpdating;
					fu(sender, e);
				}
				else
				{
					e.NewValue = item.AccountID;
				}
			}
		}

		private void CheckData(PXCache cache, object data)
		{
			object accountID = cache.GetValue(data, _FieldName);
			if (accountID != null)
			{
				try
				{
					Account item = (Account)PXSelectorAttribute.Select(cache, data, _FieldName);
					if (item != null)
					{
						Verify(cache, item, data);
					}
				}
				catch (CuryException ee)
				{
					object val = cache.GetValueExt(data, FieldName);
					if (val is PXFieldState)
					{
						val = ((PXFieldState)val).Value;
					}
					cache.RaiseExceptionHandling(_FieldName, data, val, ee);
				}
				catch (PXSetPropertyException)
				{ }
			}
		}

		private void CurrencyInfoRowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			if (!sender.ObjectsEqual<CurrencyInfo.curyID>(e.Row, e.OldRow))
		{
			PXView siblings = CurrencyInfoAttribute.GetView(sender.Graph, BqlCommand.GetItemType(_curyKeyField), _curyKeyField);
			if (siblings != null)
			{
				PXCache cache = siblings.Cache;
				foreach (object data in siblings.SelectMultiBound(new object[] { e.Row }))
				{
					CheckData(cache, data);
				}
			}			
		}
		}

		protected virtual void BranchFieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			PXFieldState state = (PXFieldState)sender.GetValueExt(e.Row, _FieldName);
			if (state != null && state.Value != null)
			{
				Account item = (Account)PXSelectorAttribute.Select(sender, e.Row, _FieldName);
				if (item == null || item.IsCashAccount == false)
				{
					sender.SetValue(e.Row, _FieldName, null);
					sender.SetValueExt(e.Row, _FieldName, state.Value);
				}
			}
		}

		public virtual void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{		
			if (!e.Cancel && e.NewValue != null)
			{
				//Account item = PXSelect<Account, Where2<Match<Current<AccessInfo.userName>>, And<Account.accountID, Equal<Required<Account.accountID>>>>>.Select(sender.Graph, e.NewValue);
				Account item = (Account)PXSelectorAttribute.Select(sender, e.Row, _FieldName, e.NewValue);

				if (item != null && !sender.Graph.UnattendedMode)
				{
					try
					{
						Verify(sender, item, e.Row);

					}
					catch (PXSetPropertyException)
					{
						e.NewValue = item.AccountCD;
						throw;
					}
				}
			}

			if (_UIAttrIndex != -1)
			{
				((IPXFieldVerifyingSubscriber)_Attributes[_UIAttrIndex]).FieldVerifying(sender, e);
			}
		}

		void IPXRowPersistingSubscriber.RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if (_curyKeyField != null && ((e.Operation & PXDBOperation.Command) == PXDBOperation.Insert || (e.Operation & PXDBOperation.Command) == PXDBOperation.Update))
			{
				CheckData(sender, e.Row);
			}
		}

		#region Default SiteID
		protected virtual Definition Definitions
		{
			get
			{
				Definition defs = PX.Common.PXContext.GetSlot<Definition>();
				if (defs == null)
				{
					defs = PX.Common.PXContext.SetSlot<Definition>(PXDatabase.GetSlot<Definition>("Account.Definition", typeof(Account)));
				}
				return defs;
			}
		}

		protected class Definition : IPrefetchable
		{
			private int? _DefaultAccountID;
			public int? DefaultAccountID
			{
				get { return _DefaultAccountID; }
			}

			public void Prefetch()
			{
				using (PXDataRecord record = PXDatabase.SelectSingle<Account>(
					new PXDataField<Account.accountID>(),
					new PXDataFieldOrder<Account.accountID>()))
				{
					_DefaultAccountID = null;
					if (record != null)
						_DefaultAccountID = record.GetInt32(0);
				}
			}
		}
		#endregion

		public static Account GetAccount(PXGraph graph, int? accountID)
		{
			if (accountID == null)
				throw new ArgumentNullException("accountID");

			var account = (Account)PXSelect<Account,
								Where<Account.accountID, Equal<Required<Account.accountID>>>>
								.Select(graph, accountID);

			if (account == null)
			{
				throw new PXException(Messages._0_WithID_1_DoesNotExists, EntityHelper.GetFriendlyEntityName(typeof(Account)),
					accountID);
			}

			return account;
		}
	}
	[PXRestrictor(typeof(Where<Account.curyID, IsNull, And<Account.isCashAccount, Equal<False>>>), Messages.AccountCanNotBeDenominated)]
	public class PXNonCashAccountAttribute : AccountAttribute 
	{}

	/// <summary>
	/// Represents Subaccount Field
	/// Subaccount field usually exists in pair with Account field. Use AccountType argument to specify the respective Account field.
	/// </summary>
	[PXDBInt]
	[PXInt]
	[PXUIField(DisplayName = "Subaccount", Visibility = PXUIVisibility.Visible, FieldClass = DimensionName)]
	[PXRestrictor(typeof(Where<Sub.active, Equal<True>>), Messages.SubaccountInactive)]
	public class SubAccountAttribute : AcctSubAttribute, IPXRowInsertingSubscriber
	{
		#region State

		public const string DimensionName = "SUBACCOUNT";

		public class dimensionName : Constant<string>
		{
			public dimensionName() : base(DimensionName) {}
		}
		private readonly Type _branchID;
		private readonly Type _accounType;
		#endregion

		#region Ctor

		public SubAccountAttribute() : this(null) {}

		public SubAccountAttribute(Type AccountType) : this(AccountType, null) {}

		public SubAccountAttribute(Type AccountType, Type BranchType, bool AccountAndBranchRequired=false)
		{
			_branchID = BranchType;
			_accounType = AccountType;

			BqlCommand search = BqlCommand.CreateInstance(typeof(Search<Sub.subID, Where<Match<Current<AccessInfo.userName>>>>));
			if (AccountType != null)
			{
				search = search.WhereAnd(GetIsNullAndMatchWhere(AccountType, AccountAndBranchRequired));

				if (BranchType != null)
				{
					search = search.WhereAnd(GetIsNullAndMatchWhere(BranchType, AccountAndBranchRequired));
				}
			}

			PXDimensionSelectorAttribute attr = new PXDimensionSelectorAttribute(DimensionName, search.GetType(), typeof (Sub.subCD))
			{
				CacheGlobal = true,
				DescriptionField = typeof (Sub.description)
			};
			_Attributes.Add(attr);
			_SelAttrIndex = _Attributes.Count - 1;
			Filterable = true;
		}

		private static Type GetIsNullAndMatchWhere(Type entityType, bool IsRequired)
		{
			Type where = BqlCommand.Compose(typeof(Where<>), typeof(Match<>), typeof(Optional<>), entityType);
			if (!IsRequired)
			{
				where = BqlCommand.Compose(typeof(Where<,,>), typeof(Optional<>), entityType, typeof(IsNull), typeof(Or<>), where);
			}
			return where;
		}

		#endregion

		#region Runtime

		public override void CacheAttached(PXCache sender)
		{
			if (!PXAccess.FeatureInstalled<FeaturesSet.subAccount>())
			{
				((PXDimensionSelectorAttribute)this._Attributes[_Attributes.Count - 1]).ValidComboRequired = false;
				sender.Graph.FieldDefaulting.AddHandler(sender.GetItemType(), _FieldName, FieldDefaulting);
			}

			base.CacheAttached(sender);
			if (_branchID != null)
				sender.Graph.FieldUpdated.AddHandler(BqlCommand.GetItemType(_branchID), _branchID.Name, RelatedFieldUpdated);
			if (_accounType != null)
				sender.Graph.FieldUpdated.AddHandler(BqlCommand.GetItemType(_accounType), _accounType.Name, RelatedFieldUpdated);
			//should execute before PXDimensionSelector RowPersisting()
			sender.Graph.RowPersisting.AddHandler(sender.GetItemType(), RowPersisting);
		}

		#endregion

		#region Implementation

		public virtual void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			int? KeyToAbort = (int?)sender.GetValue(e.Row, _FieldOrdinal);

			if (((e.Operation & PXDBOperation.Command) == PXDBOperation.Insert || (e.Operation & PXDBOperation.Command) == PXDBOperation.Update) &&
					KeyToAbort == null && !PXAccess.FeatureInstalled<FeaturesSet.subAccount>())
			{
				KeyToAbort = GetDefaultSubID(sender, e.Row);
				sender.SetValue(e.Row, _FieldName, KeyToAbort);
				PXUIFieldAttribute.SetError(sender, e.Row, _FieldName, null);
			}

			if (((e.Operation & PXDBOperation.Command) == PXDBOperation.Insert ||
				 (e.Operation & PXDBOperation.Command) == PXDBOperation.Update) && KeyToAbort < 0)
			{
				PXCache cache = sender.Graph.Caches[typeof(Sub)];
				PXSelectBase<Sub> cmd = new PXSelectReadonly<Sub, Where<Sub.subCD, Equal<Current<Sub.subCD>>>>(sender.Graph);
				Sub persisteditem = null;

				foreach (Sub item in cache.Inserted)
				{
					if (object.Equals(item.SubID, KeyToAbort))
					{
						if ((persisteditem = (Sub)cmd.View.SelectSingleBound(new object[] { item })) != null)
						{
							//place in _JustPersisted dictionary
							cache.RaiseRowPersisting(item, PXDBOperation.Insert);
							cache.RaiseRowPersisted(persisteditem, PXDBOperation.Insert, PXTranStatus.Open, null);

							persisteditem = item;
							break;
						}
					}
				}

				if (persisteditem != null)
				{
					cache.SetStatus(persisteditem, PXEntryStatus.Notchanged);
					cache.Remove(persisteditem);
				}
			}
		}

		protected virtual void RelatedFieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			SetSubAccount(sender, e.Row);
		}

		public virtual void RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			SetSubAccount(sender, e.Row);
		}

		public virtual void FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (!e.Cancel)
			{
				e.NewValue = GetDefaultSubID(sender, e.Row);
				e.Cancel = true;
			}
		}

		#endregion

		private void SetSubAccount(PXCache sender, object row)
		{
			PXFieldState state = (PXFieldState)sender.GetValueExt(row, _FieldName);
			if (state != null && state.Value != null)
			{
				sender.SetValue(row, _FieldName, null);
				sender.SetValueExt(row, _FieldName, state.Value);
			}
		}

		#region Default SubID
		private int? GetDefaultSubID(PXCache sender, object row)
		{
			if (Definitions.DefaultSubID == null)
			{
				object newValue = "0";
				sender.RaiseFieldUpdating(_FieldName, row, ref newValue);
				return (int?)newValue;
			}
			return Definitions.DefaultSubID;
		}

		protected static Definition Definitions
		{
			get
			{
				Definition defs = PX.Common.PXContext.GetSlot<Definition>();
				if (defs == null)
				{
					defs = PX.Common.PXContext.SetSlot<Definition>(PXDatabase.GetSlot<Definition>(typeof(Definition).FullName, typeof(Sub)));
				}
				return defs;
			}
		}

		protected class Definition : IPrefetchable
		{
			private int? _DefaultSubID;
			public int? DefaultSubID
			{
				get { return _DefaultSubID; }
			}

			public void Prefetch()
			{
				_DefaultSubID = null;

				using (PXDataRecord record = PXDatabase.SelectSingle<Sub>(
					new PXDataField<Sub.subID>(),
					new PXDataFieldOrder<Sub.subID>()))
				{
					if (record != null)
						_DefaultSubID = record.GetInt32(0);
				}
			}
		}

		#endregion

		public static Sub GetSubaccount(PXGraph graph, int? subID)
		{
			if (subID == null)
				throw new ArgumentNullException("subID");

			var subaccount = (Sub)PXSelect<Sub,
								Where<Sub.subID, Equal<Required<Sub.subID>>>>
								.Select(graph, subID);

			if (subaccount == null)
			{
				throw new PXException(Messages._0_WithID_1_DoesNotExists, EntityHelper.GetFriendlyEntityName(typeof(Sub)),
					subID);
			}

			return subaccount;
		}

		/// <summary>
		/// Returns deafult subID if default subaccount exists, else returns null.
		/// </summary>
		public static int? TryGetDefaultSubID()
		{
			return Definitions.DefaultSubID;
		}
	}

	[PXDBString(10, IsUnicode = true, InputMask = "")]
	[PXUIField(DisplayName = "Account", Visibility = PXUIVisibility.Visible)]
	public sealed class AccountCDWildcardAttribute : PXDimensionWildcardAttribute 
	{        
		private int _UIAttrIndex = -1;
		
		private void Initialize()
		{            
			_UIAttrIndex = -1;
			foreach (PXEventSubscriberAttribute attr in _Attributes)
			{                
				if (attr is PXUIFieldAttribute)
				{
					_UIAttrIndex = _Attributes.IndexOf(attr);
				}               
			}
		}
		private const string _DimensionName = "ACCOUNT";
		
		public AccountCDWildcardAttribute()
			: base(_DimensionName,typeof(Account.accountCD))
		{
			Initialize();	
		}
		public AccountCDWildcardAttribute(Type aSearchType)
			: base(_DimensionName, aSearchType)
		{
			Initialize();
		}

		public string DisplayName
		{
			get
			{
				return (_UIAttrIndex == -1) ? null : ((PXUIFieldAttribute)_Attributes[_UIAttrIndex]).DisplayName;
			}
			set
			{
				if (_UIAttrIndex != -1)
					((PXUIFieldAttribute)_Attributes[_UIAttrIndex]).DisplayName = value;
			}
		}

	}


	[PXDBString(30, IsUnicode = true, InputMask = "")]
	[PXUIField(DisplayName = "Subaccount", Visibility = PXUIVisibility.Visible, FieldClass = _DimensionName)]
	public sealed class SubCDWildcardAttribute : PXDimensionWildcardAttribute
	{
		private int _UIAttrIndex = -1;

		private void Initialize()
		{
			_UIAttrIndex = -1;
			foreach (PXEventSubscriberAttribute attr in _Attributes)
			{
				if (attr is PXUIFieldAttribute)
				{
					_UIAttrIndex = _Attributes.IndexOf(attr);
				}
			}
		}
		private const string _DimensionName = "SUBACCOUNT";
		public SubCDWildcardAttribute()
			: base(_DimensionName, typeof(Sub.subCD))
		{
			int dimensionSelIndex = this._Attributes.Count - 2;
			PXDimensionAttribute attr = this._Attributes[dimensionSelIndex] as PXDimensionAttribute; 
			attr.ValidComboRequired = false;
			Initialize();				
		}
		public string DisplayName
		{
			get
			{
				return (_UIAttrIndex == -1) ? null : ((PXUIFieldAttribute)_Attributes[_UIAttrIndex]).DisplayName;
			}
			set
			{
				if (_UIAttrIndex != -1)
					((PXUIFieldAttribute)_Attributes[_UIAttrIndex]).DisplayName = value;
			}
		}

		public override void CacheAttached(PXCache sender)
		{
			if (!PXAccess.FeatureInstalled<FeaturesSet.subAccount>())
			{
				sender.Graph.FieldDefaulting.AddHandler(sender.GetItemType(), _FieldName, FieldDefaulting);
			}
			base.CacheAttached(sender);
		}
		
		public void FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (!e.Cancel)
			{
				e.NewValue = GetDefaultSubID(sender, e.Row);
				e.Cancel = true;
			}
		}


		#region Default SubID
		private string GetDefaultSubID(PXCache sender, object row)
		{
			if (this.Definitions.DefaultSubCD == null)
			{
				object newValue = "0";
				sender.RaiseFieldUpdating(_FieldName, row, ref newValue);
				return (string)newValue;
			}
			return this.Definitions.DefaultSubCD;
		}

		private Definition Definitions
		{
			get
			{
				Definition defs = PX.Common.PXContext.GetSlot<Definition>();
				if (defs == null)
				{
					defs = PX.Common.PXContext.SetSlot<Definition>(PXDatabase.GetSlot<Definition>(typeof(Definition).FullName, typeof(Sub)));
				}
				return defs;
			}
		}

		private class Definition : IPrefetchable
		{
			private string _DefaultSubCD;
			public string DefaultSubCD
			{
				get { return _DefaultSubCD; }
			}

			public void Prefetch()
			{
				_DefaultSubCD = null;

				using (PXDataRecord record = PXDatabase.SelectSingle<Sub>(
					new PXDataField<Sub.subCD>(),
					new PXDataFieldOrder<Sub.subCD>()))
				{
					if (record != null)
						_DefaultSubCD = record.GetString(0);
				}
			}
		}

		#endregion
	}

	/// <summary>
	/// Specialized for the GLTran version of the <see cref="CashTranIDAttribute"/><br/>
	/// Since CATran created from the source row, it may be used only the fields <br/>
	/// of GLTran compatible DAC. <br/>
	/// The main purpuse of the attribute - to create CATran <br/>
	/// for the source row and provide CATran and source synchronization on persisting.<br/>
	/// CATran cache must exists in the calling Graph.<br/>
	/// </summary>
	public class GLCashTranIDAttribute : CashTranIDAttribute, IPXRowInsertingSubscriber, IPXRowUpdatingSubscriber  
	{

		protected bool	 _IsIntegrityCheck = false;
		protected bool _CATranValidation = false;

		protected string _LedgerNotActual = "N";
		protected string _LedgerActual = "A";

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);
			Type ChildType = sender.GetItemType();

			if (sender.Graph.Views.Caches.Contains(ChildType) == false)
			{
				sender.Graph.Views.Caches.Add(ChildType);
			}

			if (sender.Graph.Views.Caches.Contains(typeof(CATran)) == false)
			{
				sender.Graph.Views.Caches.Add(typeof(CATran));
			}
		}

		public static CATran DefaultValues<Field>(PXCache sender, object data)
			where Field : IBqlField
		{
			foreach (PXEventSubscriberAttribute attr in sender.GetAttributes<Field>(data))
			{
				if (attr is GLCashTranIDAttribute)
				{
					((GLCashTranIDAttribute)attr)._IsIntegrityCheck = true;
					((GLCashTranIDAttribute) attr)._CATranValidation = true;
					CATran result = ((GLCashTranIDAttribute)attr).DefaultValues(sender, new CATran(), data);
					((GLCashTranIDAttribute) attr)._CATranValidation = false;
					return result;
				}
			}
			return null;
		}

		public static CATran DefaultValues(PXCache sender, object data)
		{
			GLCashTranIDAttribute attr = new GLCashTranIDAttribute();
			attr._IsIntegrityCheck = true;
			return attr.DefaultValues(sender, new CATran(), data);
		}

		public override void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if (_IsIntegrityCheck == false)
			{
				base.RowPersisting(sender, e);
			}
		}

		public override void RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
		{
			if (_IsIntegrityCheck == false)
			{
				base.RowPersisted(sender, e);
			}
		}

		public virtual void RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
		{
			GLTran tran = (GLTran)e.NewRow;
			int? cashAccountID;
			if (CheckGLTranCashAcc(sender.Graph, tran, out cashAccountID) == false)
			{
				Branch branch = (Branch)PXSelectorAttribute.Select<GLTran.branchID>(sender, tran);
				Account account = (Account)PXSelectorAttribute.Select<GLTran.accountID>(sender, tran);
				Sub sub = (Sub)PXSelectorAttribute.Select<GLTran.subID>(sender, tran);				
				if (e.ExternalCall)
				{
					sender.RaiseExceptionHandling<GLTran.subID>(tran, sub.SubCD,
						new PXSetPropertyException(Messages.CashAccountDoesNotExist, branch.BranchCD, account.AccountCD, sub.SubCD));
					e.Cancel = true;
				}
				else
				{
					throw new PXSetPropertyException(Messages.CashAccountDoesNotExist, branch.BranchCD, account.AccountCD, sub.SubCD);
				}
			} 
		}

		public virtual void RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			GLTran tran = (GLTran)e.Row;
			int? cashAccountID;
			if (CheckGLTranCashAcc(sender.Graph, tran, out cashAccountID) == false)
			{
				Branch branch = (Branch)PXSelectorAttribute.Select<GLTran.branchID>(sender, tran);
				Account account = (Account)PXSelectorAttribute.Select<GLTran.accountID>(sender, tran);
				Sub sub = (Sub)PXSelectorAttribute.Select<GLTran.subID>(sender, tran);
				if (e.ExternalCall)
				{
					sender.RaiseExceptionHandling<GLTran.subID>(tran, sub.SubCD,
						new PXSetPropertyException(Messages.CashAccountDoesNotExist, branch.BranchCD, account.AccountCD, sub.SubCD));
					e.Cancel = true;
				}
				else
				{
					throw new PXSetPropertyException(Messages.CashAccountDoesNotExist, branch.BranchCD, account.AccountCD, sub.SubCD);
				}
			}
		}

		public static bool? CheckGLTranCashAcc(PXGraph graph, GLTran tran, out int? cashAccountID)
		{
			cashAccountID = null;
			Account acc = PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(graph, tran.AccountID);
			if (acc != null && acc.IsCashAccount == true && tran.SubID != null && tran.BranchID != null)
			{
				CashAccount cashAcc = PXSelect<CashAccount, Where<CashAccount.accountID, Equal<Required<CashAccount.accountID>>,
					And<CashAccount.subID, Equal<Required<CashAccount.subID>>, And<CashAccount.branchID,
					Equal<Required<CashAccount.branchID>>>>>>.Select(graph, tran.AccountID, tran.SubID, tran.BranchID);
				
				if (cashAcc == null)
				{
					return false;
				}
				else
				{
					cashAccountID = cashAcc.CashAccountID;
					return true;
				}
			}
			return null;
		}

		public static bool CheckDocTypeAmounts(string Module, string DocType, decimal CuryDocAmount, decimal CuryDebitAmount, decimal CuryCreditAmount)
		{
			switch (Module)
			{
				case BatchModule.AR:
					switch (DocType)
					{
						case ARDocType.Payment:
						case ARDocType.CashSale:
						case ARDocType.Prepayment:
							//Debit
							return Math.Abs(CuryDocAmount) == CuryDebitAmount;
						case ARDocType.VoidPayment:
						case ARDocType.Refund:
						case ARDocType.CashReturn:
							//Credit
							return Math.Abs(CuryDocAmount) == CuryCreditAmount;
						default:
							return false;
					}
				case BatchModule.AP:
					switch (DocType)
					{
						case APDocType.Check:
						case APDocType.Prepayment:
						case APDocType.QuickCheck:
							//Credit
							return Math.Abs(CuryDocAmount) == CuryCreditAmount;
						case APDocType.VoidCheck:
						case APDocType.Refund:
						case APDocType.VoidQuickCheck:
							//Debit
							return Math.Abs(CuryDocAmount) == CuryDebitAmount;
						default:
							return false;
					}
			}
			return false;
		}	

		public override CATran DefaultValues(PXCache sender, CATran catran_Row, object orig_Row)
		{
			GLTran parentTran = (GLTran)orig_Row;
			bool LedgerNotActual = false;
			if (String.IsNullOrWhiteSpace(parentTran.LedgerBalanceType))
			{
				Ledger setup = (Ledger)PXSelect<Ledger, Where<Ledger.ledgerID, Equal<Required<Ledger.ledgerID>>,
					And<Ledger.balanceType, Equal<LedgerBalanceType.actual>>>>.Select(sender.Graph, parentTran.LedgerID);
				LedgerNotActual = (setup == null);
			}
			else
			{
				LedgerNotActual = (parentTran.LedgerBalanceType == _LedgerNotActual);
			}
			int? cashAccountID;
			bool? CashAccChecked = CheckGLTranCashAcc(sender.Graph, parentTran, out cashAccountID);

			if (CashAccChecked == null || LedgerNotActual || parentTran.Module == BatchModule.CM)
			{
				return null;
			}
			else
			{
				if (CashAccChecked == false)
				{
					Branch branch = (Branch)PXSelectorAttribute.Select<GLTran.branchID>(sender, parentTran);
					Account account = (Account)PXSelectorAttribute.Select<GLTran.accountID>(sender, parentTran);
					Sub sub = (Sub)PXSelectorAttribute.Select<GLTran.subID>(sender, parentTran);
					sender.RaiseExceptionHandling<GLTran.subID>(parentTran, sub.SubCD, new PXSetPropertyException(Messages.CashAccountDoesNotExist,
						branch.BranchCD, account.AccountCD, sub.SubCD));
					return null;
				}
			}
			
			if (parentTran.Module == BatchModule.GL || catran_Row.TranID == null)
			{
				if (catran_Row.TranID == null)
				{
					string origTranType = CAAPARTranType.GLEntry;
					string origRefNbr = parentTran.BatchNbr;
					string extRefNbr = parentTran.RefNbr;
					long? caTranId = null;
					CashAccount cashAcct;
					if (_CATranValidation)
					{
						switch (parentTran.Module)
						{
							case BatchModule.AR:
								ARPayment arPayment = PXSelect<ARPayment, Where<ARPayment.docType, Equal<Required<ARPayment.docType>>,
									And<ARPayment.refNbr, Equal<Required<ARPayment.refNbr>>>>>.Select(sender.Graph, parentTran.TranType, parentTran.RefNbr);
								cashAcct = PXSelectorAttribute.Select<ARPayment.cashAccountID>(sender.Graph.Caches[typeof(ARPayment)], arPayment) as CashAccount;
								if (arPayment != null && cashAcct != null && cashAcct.AccountID == parentTran.AccountID
									&& cashAcct.SubID == parentTran.SubID 
									&& CheckDocTypeAmounts(parentTran.Module, parentTran.TranType, arPayment.CuryOrigDocAmt.Value,
										parentTran.CuryDebitAmt.Value, parentTran.CuryCreditAmt.Value))
								{
									origTranType = arPayment.DocType;
									origRefNbr = arPayment.RefNbr;
									extRefNbr = arPayment.ExtRefNbr;
									caTranId = arPayment.CATranID;
								}
								break;
							case BatchModule.AP:
								APPayment apPayment = PXSelect<APPayment, Where<APPayment.docType, Equal<Required<APPayment.docType>>,
									And<APPayment.refNbr, Equal<Required<APPayment.refNbr>>>>>.Select(sender.Graph, parentTran.TranType, parentTran.RefNbr);
								cashAcct = PXSelectorAttribute.Select<APPayment.cashAccountID>(sender.Graph.Caches[typeof(APPayment)], apPayment) as CashAccount;
								if (apPayment != null && cashAcct != null && cashAcct.AccountID == parentTran.AccountID
									&& cashAcct.SubID == parentTran.SubID
									&& CheckDocTypeAmounts(parentTran.Module, parentTran.TranType, apPayment.CuryOrigDocAmt.Value,
										parentTran.CuryDebitAmt.Value, parentTran.CuryCreditAmt.Value))
								{
									origTranType = apPayment.DocType;
									origRefNbr = apPayment.RefNbr;
									extRefNbr = apPayment.ExtRefNbr;
									caTranId = apPayment.CATranID;
								}
								break;
						}

						if (caTranId != null)
						{
							CATran catran = PXSelect<CATran, Where<CATran.tranID, Equal<Required<CATran.tranID>>>>.Select(sender.Graph, caTranId);
							if (catran != null)
							{
								catran_Row = catran;
							}
							else
							{
								caTranId = null;
							}
						}

						if (caTranId == null)
						{
							decimal? amt;
							string DrCr;
							if (parentTran.CuryDebitAmt - parentTran.CuryCreditAmt >= 0)
							{
								amt = parentTran.CuryDebitAmt - parentTran.CuryCreditAmt;
								DrCr = GL.DrCr.Debit;
							}
							else
							{
								amt = parentTran.CuryDebitAmt - parentTran.CuryCreditAmt;
								DrCr = GL.DrCr.Credit;
							}
							PXResultset<CATran> catranList = PXSelect<CATran, Where<CATran.origModule, Equal<Required<CATran.origModule>>,
									And<CATran.origTranType, Equal<Required<CATran.origTranType>>,
									And<CATran.origRefNbr, Equal<Required<CATran.origRefNbr>>,
									And<CATran.cashAccountID, Equal<Required<CATran.cashAccountID>>,
									And<CATran.curyTranAmt, Equal<Required<CATran.curyTranAmt>>,
									And<CATran.drCr, Equal<Required<CATran.drCr>>>>>>>>>.Select(sender.Graph, parentTran.Module,
										origTranType, origRefNbr, cashAccountID, amt, DrCr);
							foreach (CATran catran in catranList)
							{
								GLTran linkedTran = PXSelect<GLTran, Where<GLTran.module, Equal<Required<GLTran.module>>,
									And<GLTran.batchNbr, Equal<Required<GLTran.batchNbr>>,
										And<GLTran.cATranID, Equal<Required<GLTran.cATranID>>>>>>.Select(sender.Graph, parentTran.Module, parentTran.BatchNbr, catran.TranID);
								if (linkedTran == null)
								{
									catran_Row = catran;
									break;
								}
							}
						}
					}

					if (catran_Row.TranID == null)
					{
						catran_Row.OrigModule = parentTran.Module;
						catran_Row.OrigTranType = origTranType;
						catran_Row.OrigRefNbr = origRefNbr;
						catran_Row.ExtRefNbr = extRefNbr;
					}
					else
					{
						//make a copy for accumulator attribute
						catran_Row = PXCache<CATran>.CreateCopy(catran_Row);
					}
				}
				else if (catran_Row.TranID < 0)
				{
					catran_Row.OrigModule = parentTran.Module;
					catran_Row.OrigTranType = CAAPARTranType.GLEntry;
					catran_Row.OrigRefNbr = parentTran.BatchNbr;
					catran_Row.ExtRefNbr = parentTran.RefNbr;
				}
				
				if (!_CATranValidation || catran_Row.TranID == null || catran_Row.TranID < 0)
				{

					catran_Row.CuryTranAmt = parentTran.CuryDebitAmt - parentTran.CuryCreditAmt;
					catran_Row.TranAmt = parentTran.DebitAmt - parentTran.CreditAmt;
					if (parentTran.CuryDebitAmt - parentTran.CuryCreditAmt >= 0)
				{
					catran_Row.DrCr = DrCr.Debit;
				}
				else
				{
					//cash credits have minus sign
					catran_Row.DrCr = DrCr.Credit;
				}
				CashAccount cashacc = PXSelect<CashAccount, Where<CashAccount.cashAccountID, Equal<Required<CashAccount.cashAccountID>>>>.Select(sender.Graph, cashAccountID);

				catran_Row.CashAccountID = cashacc.CashAccountID;
				catran_Row.CuryInfoID = parentTran.CuryInfoID;
				catran_Row.TranDate = parentTran.TranDate;
				catran_Row.TranDesc = parentTran.TranDesc;
				catran_Row.FinPeriodID = parentTran.FinPeriodID;
				catran_Row.ReferenceID = parentTran.ReferenceID;
				catran_Row.Released = parentTran.Released;
				catran_Row.Posted = parentTran.Posted;
				catran_Row.Hold = false;
				catran_Row.BatchNbr = parentTran.BatchNbr;


				if (cashacc != null && cashacc.Reconcile == false)
				{
					catran_Row.Cleared = true;
					catran_Row.ClearDate = catran_Row.TranDate;
				}
			}
			}
			else
			{
				if ((parentTran.Released == true) && (catran_Row != null) && (catran_Row.TranID != null))
				{
					catran_Row.Released	= parentTran.Released;
					catran_Row.Posted	= parentTran.Posted;
					catran_Row.Hold		= false;
					catran_Row.BatchNbr = parentTran.BatchNbr;
					catran_Row.CuryTranAmt = (parentTran.CuryDebitAmt - parentTran.CuryCreditAmt);
					catran_Row.TranAmt = (parentTran.DebitAmt - parentTran.CreditAmt);

					PXSelectBase<CashAccount> selectStatement = new PXSelectReadonly<CashAccount, Where<CashAccount.cashAccountID, Equal<Required<CashAccount.cashAccountID>>>>(sender.Graph);
					CashAccount cashacc						  = (CashAccount) selectStatement.View.SelectSingle(catran_Row.CashAccountID);
					if (cashacc != null && cashacc.Reconcile == false && (catran_Row.Cleared != true || catran_Row.TranDate == null))
					{
						catran_Row.Cleared   = true;
						catran_Row.ClearDate = catran_Row.TranDate;
					}
				}
				else
				{
					return null;
				}
			}
			return catran_Row;
		}

		/// <returns>
		/// <c>true</c>, if the <see cref="Batch"/> record, to which the current <see cref="GLTran"/>
		/// row belongs, is part of a recurring schedule, and a default value otherwise.
		/// </returns>
		protected override bool NeedPreventCashTransactionCreation(PXCache sender, object row)
		{
			GLTran transaction = row as GLTran;

			if (transaction?.Module == null || 
				transaction.BatchNbr == null)
			{
				return base.NeedPreventCashTransactionCreation(sender, row);
			}

			PXCache batchCache = sender.Graph.Caches[typeof(Batch)];

			// Try searching for the referenced Batch record in the 
			// Current property first.
			// -
			Batch parentBatch = batchCache.Current as Batch;

			if (parentBatch?.Module == transaction.Module &&
				parentBatch?.BatchNbr == transaction.BatchNbr)
			{
				return parentBatch?.Scheduled == true;
			}

			// Failing that, try to locate the Batch record in the cache.
			// -
			parentBatch = batchCache.CreateInstance() as Batch;

			if (parentBatch != null)
			{
				parentBatch.Module = transaction.Module;
				parentBatch.BatchNbr = transaction.BatchNbr;

				parentBatch = batchCache.Locate(parentBatch) as Batch;

				if (parentBatch != null)
				{
					return parentBatch.Scheduled == true;
				}
			}

			// Failing that, read the Batch record from the database.
			// -
			parentBatch = PXSelect<
				Batch,
				Where<
					Batch.module, Equal<Required<Batch.module>>,
					And<Batch.batchNbr, Equal<Required<Batch.batchNbr>>>>>
				.Select(sender.Graph, transaction.Module, transaction.BatchNbr);

			if (parentBatch != null)
			{
				return parentBatch.Scheduled == true;
			}

			return base.NeedPreventCashTransactionCreation(sender, row);
		}
	}

	#region TrialBalanceImportStatusAttribute

	/// <summary>
	/// List Attrubute with the following values : New, Valid, Duplicate, Error.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class TrialBalanceImportStatusAttribute : PXIntListAttribute
	{
		public const int NEW = 0;
		public const int VALID = 1;
		public const int DUPLICATE = 2;
		public const int ERROR = 3;

		public TrialBalanceImportStatusAttribute()
			: base(new int[] { NEW, VALID, DUPLICATE, ERROR }, 
				   new string[] { Messages.New, Messages.Valid, Messages.Duplicate, Messages.Error })
		{
		}
	}

	#endregion

	#region TrialBalanceImportMapStatusAttribute

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class TrialBalanceImportMapStatusAttribute : PXIntListAttribute
	{
		public const int HOLD = 0;
		public const int BALANCED = 1;
		public const int RELEASED = 2;

		public TrialBalanceImportMapStatusAttribute()
			: base(new int[] { HOLD, BALANCED, RELEASED }, 
				   new string[] { Messages.Hold, Messages.Balanced, Messages.Released })
		{
		}
	}

	#endregion

	#region PersistErrorAttribute

	/// <summary>
	/// Maps the errors from one field to another. Whenever an error is raised on the Source field it is transfered to
	/// the target field and gets displayed on the targer field.
	/// To use this attribute decorate the Source field with this attribute and pass Taget field an argument in the constructor of this attrubute.
	/// </summary>
	/// <example>
	/// [PersistError(typeof(GLTrialBalanceImportDetails.importAccountCDError))]
	/// </example>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class PersistErrorAttribute : PXEventSubscriberAttribute, IPXFieldSelectingSubscriber
	{
		private readonly string _errorFieldName;

		public PersistErrorAttribute(Type errorField)
		{
			_errorFieldName = errorField.Name;
		}

		public void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			if (e.Row != null)
			{
				string error = sender.GetValue(e.Row, _errorFieldName) as string;
				if (!string.IsNullOrEmpty(error))
				{
					SetError(sender, e.Row, error);
					if (e.ReturnState != null) 
						e.ReturnState = PXFieldState.CreateInstance(e.ReturnState, null, null, null,
							null, null, null, null, _FieldName, null, null, error, PXErrorLevel.Error,
							null, null, null, PXUIVisibility.Undefined, null, null, null);
				}
			}
		}

		public static void SetError(PXCache sender, object data, string fieldName, string error)
		{
			PersistErrorAttribute attribute = GetAttribute(sender, fieldName);
			if (attribute != null)
			{
				sender.SetValue(data, attribute._errorFieldName, error);
				attribute.SetError(sender, data, error);
			}
		}

		public static void ClearError(PXCache sender, object data, string fieldName)
		{
			PersistErrorAttribute attribute = GetAttribute(sender, fieldName);
			if (attribute != null) sender.SetValue(data, attribute._errorFieldName, null);
		}

		private void SetError(PXCache sender, object data, string error)
		{
			object value = sender.GetValue(data, _FieldOrdinal);
			PXUIFieldAttribute.SetError(sender, data, _FieldName, error,
										value == null ? null : value.ToString());
		}

		private static PersistErrorAttribute GetAttribute(PXCache sender, string fieldName)
		{
			foreach (PXEventSubscriberAttribute attribute in sender.GetAttributes(fieldName))
			{
				PersistErrorAttribute castAttribute = attribute as PersistErrorAttribute;
				if (castAttribute != null) return castAttribute;
			}
			return null;
		}
	}

	#endregion

	#region TableAndChartDashboardTypeAttribute

	/// <summary>
	/// Graphs decorated with the given attribute will expose there primary View as a source for both Dashboard Table and Dashbprd Chart Controls.
	/// Usually an Inquiry Graph is decorated with this attribute.
	/// </summary>
	/// <example>
	/// [DashboardType(PX.TM.OwnedFilter.DASHBOARD_TYPE, GL.TableAndChartDashboardTypeAttribute._AMCHARTS_DASHBOART_TYPE)]
	/// </example>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class TableAndChartDashboardTypeAttribute : DashboardTypeAttribute
	{
		public const int _AMCHARTS_DASHBOART_TYPE = 20;

		public TableAndChartDashboardTypeAttribute()
			: base((int)Type.Default, (int)Type.Chart, _AMCHARTS_DASHBOART_TYPE)
		{
			
		}
	}

	#endregion

	#region TableDashboardTypeAttribute

	/// <summary>
	/// Graphs decorated with the given attribute will expose there primary View as a source for Dashboard Table Controls.
	/// Usually an Inquiry Graph is decorated with this attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class TableDashboardTypeAttribute : DashboardTypeAttribute
	{
		public TableDashboardTypeAttribute()
			: base((int)Type.Default)
		{
			
		}
	}

	#endregion

	public class GLTaxAttribute : TX.TaxAttribute
	{
		#region CuryInclTaxTotal
		protected string _CuryInclTaxTotal = "CuryTaxTotal";
		protected string _CuryTranTotal = "CuryTranTotal";
		public Type CuryInclTaxTotal
		{
			set
			{
				_CuryInclTaxTotal = value.Name;
			}
			get
			{
				return null;
			}
		}

		public Type CuryTranTotal
		{
			set
			{
				_CuryTranTotal = value.Name;
			}
			get
			{
				return null;
			}
		}
		#endregion

		public GLTaxAttribute(Type ParentType, Type TaxType, Type TaxSumType)
			: base(ParentType, TaxType, TaxSumType)
		{
			this.CuryDocBal = null;
			this.CuryLineTotal = typeof(GLTranDoc.curyTranAmt); //Used only for reading
			this.CuryTaxTotal = typeof(GLTranDoc.curyTaxAmt);
			this.CuryInclTaxTotal = typeof(GLTranDoc.curyInclTaxAmt);
			this.DocDate = typeof(GLTranDoc.tranDate);
			this.CuryTranAmt = typeof(GLTranDoc.curyTranAmt);

			//this._Attributes.Add(new PXUnboundFormulaAttribute(typeof(Switch<Case<Where<GLTranDoc.debitAccountID, IsNotNull>, Add<GLTranDoc.curyTranAmt, Sub<GLTranDoc.curyTaxAmt, GLTranDoc.curyInclTaxAmt>>>, Sub<GLTranDoc.curyTaxAmt, GLTranDoc.curyInclTaxAmt>>), typeof(SumCalc<GLDocBatch.curyDebitTotal>)));
			//this._Attributes.Add(new PXUnboundFormulaAttribute(typeof(Switch<Case<Where<GLTranDoc.creditAccountID, IsNotNull>, Add<GLTranDoc.curyTranAmt, Sub<GLTranDoc.curyTaxAmt, GLTranDoc.curyInclTaxAmt>>>, Sub<GLTranDoc.curyTaxAmt, GLTranDoc.curyInclTaxAmt>>), typeof(SumCalc<GLDocBatch.curyCreditTotal>)));

			this._Attributes.Add(new PXUnboundFormulaAttribute(typeof(Switch<Case<Where<GLTranDoc.debitAccountID, IsNotNull>, GLTranDoc.curyTranTotal>, Sub<GLTranDoc.curyTaxAmt, GLTranDoc.curyInclTaxAmt>>), typeof(SumCalc<GLDocBatch.curyDebitTotal>)));
			this._Attributes.Add(new PXUnboundFormulaAttribute(typeof(Switch<Case<Where<GLTranDoc.creditAccountID, IsNotNull>, GLTranDoc.curyTranTotal>, Sub<GLTranDoc.curyTaxAmt, GLTranDoc.curyInclTaxAmt>>), typeof(SumCalc<GLDocBatch.curyCreditTotal>)));


			//this._Attributes.Add(new PXUnboundFormulaAttribute(typeof(Switch<Case<Where<GLTranDoc.debitAccountID, IsNotNull>, Switch<Case<Where<GLTranDoc.parentLineNbr, IsNull>, GLTranDoc.curyTranTotal>, GLTranDoc.curyTranAmt>>, Sub<GLTranDoc.curyTaxAmt, GLTranDoc.curyInclTaxAmt>>), typeof(SumCalc<GLDocBatch.curyDebitTotal>)));
			//this._Attributes.Add(new PXUnboundFormulaAttribute(typeof(Switch<Case<Where<GLTranDoc.creditAccountID, IsNotNull>, Switch<Case<Where<GLTranDoc.parentLineNbr, IsNull>, GLTranDoc.curyTranTotal>, GLTranDoc.curyTranAmt>>, Sub<GLTranDoc.curyTaxAmt, GLTranDoc.curyInclTaxAmt>>), typeof(SumCalc<GLDocBatch.curyCreditTotal>)));

			//this._Attributes.Add(new PXUnboundFormulaAttribute(typeof(Switch<Case<Where<GLTranDoc.debitAccountID, IsNotNull, And<GLTranDoc.parentLineNbr, IsNull>>, GLTranDoc.curyTranTotal,
			//                                    Case<Where<GLTranDoc.debitAccountID, IsNull, And<GLTranDoc.parentLineNbr, IsNull>>,Sub<GLTranDoc.curyTaxAmt, GLTranDoc.curyInclTaxAmt>,
			//                                    Case<Where<GLTranDoc.debitAccountID, IsNotNull, And<GLTranDoc.parentLineNbr, IsNotNull>>, GLTranDoc.curyTranAmt>>>, decimal0>), typeof(SumCalc<GLDocBatch.curyDebitTotal>)));
			//this._Attributes.Add(new PXUnboundFormulaAttribute(typeof(Switch<Case<Where<GLTranDoc.creditAccountID, IsNotNull, And<GLTranDoc.parentLineNbr, IsNull>>, GLTranDoc.curyTranTotal,
			//                                    Case<Where<GLTranDoc.creditAccountID, IsNull, And<GLTranDoc.parentLineNbr, IsNull>>, Sub<GLTranDoc.curyTaxAmt, GLTranDoc.curyInclTaxAmt>,
			//                                    Case<Where<GLTranDoc.creditAccountID, IsNotNull, And<GLTranDoc.parentLineNbr, IsNotNull>>, GLTranDoc.curyTranAmt>>>, decimal0>), typeof(SumCalc<GLDocBatch.curyCreditTotal>)));
			////this._Attributes.Add(new PXUnboundFormulaAttribute(typeof(Switch<Case<Where<GLTranDoc.creditAccountID, IsNotNull, And<GLTranDoc.parentLineNbr, IsNull>>, GLTranDoc.curyTranTotal, Case<Where<GLTranDoc.creditAccountID, IsNotNull, And<GLTranDoc.parentLineNbr, IsNotNull>>, GLTranDoc.curyTranAmt>>, Sub<GLTranDoc.curyTaxAmt, GLTranDoc.curyInclTaxAmt>>), typeof(SumCalc<GLDocBatch.curyCreditTotal>)));
			
			//this._Attributes.Add(new PXUnboundFormulaAttribute(typeof(Switch<Case<Where<GLTranDoc.parentLineNbr, IsNotNull>, Switch<Case<Where<GLTranDoc.debitAccountID, IsNotNull>, Add<GLTranDoc.curyTranAmt, Sub<GLTranDoc.curyTaxAmt, GLTranDoc.curyInclTaxAmt>>>, Sub<GLTranDoc.curyTaxAmt, GLTranDoc.curyInclTaxAmt>>>,
			//							Switch<Case<Where<GLTranDoc.debitAccountID, IsNotNull>, GLTranDoc.curyTranTotal>, Sub<GLTranDoc.curyTaxAmt, GLTranDoc.curyInclTaxAmt>>>), typeof(SumCalc<GLDocBatch.curyDebitTotal>)));

			//this._Attributes.Add(new PXUnboundFormulaAttribute(typeof(Switch<Case<Where<GLTranDoc.parentLineNbr, IsNotNull>, Switch<Case<Where<GLTranDoc.creditAccountID, IsNotNull>, Add<GLTranDoc.curyTranAmt, Sub<GLTranDoc.curyTaxAmt, GLTranDoc.curyInclTaxAmt>>>, Sub<GLTranDoc.curyTaxAmt, GLTranDoc.curyInclTaxAmt>>>,
			//							Switch<Case<Where<GLTranDoc.creditAccountID, IsNotNull>, GLTranDoc.curyTranTotal>, Sub<GLTranDoc.curyTaxAmt, GLTranDoc.curyInclTaxAmt>>>), typeof(SumCalc<GLDocBatch.curyCreditTotal>)));
			
			
		}

		protected override decimal CalcLineTotal(PXCache sender, object row)
		{
			return (decimal?)ParentGetValue(sender.Graph, _CuryLineTotal) ?? 0m; //It's only readed, not updated
		}

		protected override List<object> SelectTaxes<Where>(PXGraph graph, object row, PXTaxCheck taxchk, params object[] parameters)
		{
			Dictionary<string, PXResult<Tax, TaxRev>> tail = new Dictionary<string, PXResult<Tax, TaxRev>>();
			PXSelectBase<Tax> taxRevSelect = null;
			GLTranDoc parentrow =(GLTranDoc)(_ParentRow ?? row);
			if (parentrow != null && (parentrow.TranModule == GL.BatchModule.AP))
			{
				taxRevSelect = new PXSelectReadonly2<Tax,
											LeftJoin<TaxRev, On<TaxRev.taxID, Equal<Tax.taxID>,
												And<TaxRev.outdated, Equal<False>,
												And2<Where<TaxRev.taxType, Equal<TaxType.purchase>, And<Tax.reverseTax, Equal<False>,
													Or<TaxRev.taxType, Equal<TaxType.sales>, And<Where<Tax.reverseTax, Equal<True>,
													Or<Tax.taxType, Equal<CSTaxType.use>, Or<Tax.taxType, Equal<CSTaxType.withholding>>>>>>>>,
													And<Current<GLTranDoc.tranDate>, Between<TaxRev.startDate, TaxRev.endDate>>>>>>,Where>(graph);
			}
			if(parentrow != null && (parentrow.TranModule == GL.BatchModule.AR))
			{
				taxRevSelect = new PXSelectReadonly2<Tax,
				LeftJoin<TaxRev, On<TaxRev.taxID, Equal<Tax.taxID>,
					And<TaxRev.outdated, Equal<False>,
					And<TaxRev.taxType, Equal<TaxType.sales>,
					And<Tax.taxType, NotEqual<CSTaxType.withholding>,
					And<Tax.taxType, NotEqual<CSTaxType.use>,
					And<Tax.reverseTax, Equal<False>,
					And<Current<GLTranDoc.tranDate>, Between<TaxRev.startDate, TaxRev.endDate>>>>>>>>>,
						Where>(graph);
			}
			if (parentrow != null && (parentrow.TranModule == GL.BatchModule.CA))
			{
				taxRevSelect = new PXSelectReadonly2<Tax,
				LeftJoin<TaxRev, On<TaxRev.taxID, Equal<Tax.taxID>,
					And<TaxRev.outdated, Equal<False>,
					And2<
						Where<Current<GLTranDoc.cADrCr>, Equal<CADrCr.cACredit>, And<TaxRev.taxType, Equal<TaxType.purchase>, And<Tax.reverseTax, Equal<False>,
						Or<Current<GLTranDoc.cADrCr>, Equal<CADrCr.cACredit>, And<TaxRev.taxType, Equal<TaxType.sales>, And<Tax.reverseTax, Equal<True>,
						Or<Current<GLTranDoc.cADrCr>, Equal<CADrCr.cADebit>, And<TaxRev.taxType, Equal<TaxType.sales>, And<Tax.reverseTax, Equal<False>,
							And<Tax.taxType, NotEqual<CSTaxType.withholding>, And<Tax.taxType, NotEqual<CSTaxType.use>
						>>>>>>>>>>>,
					And<Current<GLTranDoc.tranDate>, Between<TaxRev.startDate, TaxRev.endDate>>>>>>,
					Where>(graph);
			}

			if (taxRevSelect != null)
			{
				foreach (PXResult<Tax, TaxRev> record in taxRevSelect.View.SelectMultiBound(new object[] { row }, parameters))
				{
					tail[((Tax)record).TaxID] = record;
				}
			}
			List<object> ret = new List<object>();
			
			switch (taxchk)
			{
				case PXTaxCheck.Line:
					foreach (GLTax record in PXSelect<GLTax,
						Where<GLTax.module, Equal<Current<GLTranDoc.module>>,
							And<GLTax.batchNbr, Equal<Current<GLTranDoc.batchNbr>>,
							And<GLTax.detailType, Equal<GLTaxDetailType.lineTax>,
							And<GLTax.lineNbr, Equal<Current<GLTranDoc.lineNbr>>>>>>>
						.SelectMultiBound(graph, new object[] { row }))
					{
						PXResult<Tax, TaxRev> line;
						if (tail.TryGetValue(record.TaxID, out line))
						{
							int idx;
							for (idx = ret.Count;
								(idx > 0)
								&& String.Compare(((Tax)(PXResult<GLTax, Tax, TaxRev>)ret[idx - 1]).TaxCalcLevel, ((Tax)line).TaxCalcLevel) > 0;
								idx--) ;
							ret.Insert(idx, new PXResult<GLTax, Tax, TaxRev>(record, (Tax)line, (TaxRev)line));
						}
					}
					return ret;
				case PXTaxCheck.RecalcLine:
					{
						object[] details = PXSelect<GLTranDoc, Where<GLTranDoc.module, Equal<Current<GLTranDoc.module>>,
														And<GLTranDoc.batchNbr, Equal<Current<GLTranDoc.batchNbr>>,
														And<GLTranDoc.parentLineNbr, Equal<Current<GLTranDoc.lineNbr>>>>>>.SelectMultiBound(graph, new object[] { _ParentRow }).ToArray();
						HashSet<Int32?> lines = new HashSet<Int32?>(Array.ConvertAll(details, a => PXResult.Unwrap<GLTranDoc>(a).LineNbr));
						if (row == null && _ParentRow == null) return ret;
						lines.Add(((GLTranDoc)row ?? (GLTranDoc)_ParentRow).LineNbr);

						foreach (GLTax record in PXSelect<GLTax,
							Where<GLTax.module, Equal<Current<GLTranDoc.module>>,
							And<GLTax.batchNbr, Equal<Current<GLTranDoc.batchNbr>>,
							And<GLTax.detailType, Equal<GLTaxDetailType.lineTax>>>>>
							.SelectMultiBound(graph, new object[] { row }))
						{
							PXResult<Tax, TaxRev> line;
							if (lines.Contains(record.LineNbr) && tail.TryGetValue(record.TaxID, out line))
							{
								int idx;
								for (idx = ret.Count;
									(idx > 0)
								&& ((GLTax)(PXResult<GLTax, Tax, TaxRev>)ret[idx - 1]).LineNbr == record.LineNbr
								&& String.Compare(((Tax)(PXResult<GLTax, Tax, TaxRev>)ret[idx - 1]).TaxCalcLevel, ((Tax)line).TaxCalcLevel) > 0;
								idx--) ;
								ret.Insert(idx, new PXResult<GLTax, Tax, TaxRev>(record, (Tax)line, (TaxRev)line));
							}
						}

						return ret;
					}
					case PXTaxCheck.RecalcTotals:					
					{
						GLTranDoc parent = (GLTranDoc)_ParentRow;
						if (parent == null) return ret;
						foreach (GLTaxTran record in PXSelect<GLTaxTran,
							Where<GLTaxTran.module, Equal<Current<GLTranDoc.module>>,
								And<GLTaxTran.batchNbr, Equal<Current<GLTranDoc.batchNbr>>,
								And<GLTaxTran.lineNbr, Equal<Current<GLTranDoc.lineNbr>>>>>>
							.SelectMultiBound(graph, new object[] { parent }))
						{
							PXResult<Tax, TaxRev> line;
							if (record.TaxID != null && tail.TryGetValue(record.TaxID, out line))
							{
								int idx;
								for (idx = ret.Count;
									(idx > 0)
									&& String.Compare(((Tax)(PXResult<GLTaxTran, Tax, TaxRev>)ret[idx - 1]).TaxCalcLevel, ((Tax)line).TaxCalcLevel) > 0;
									idx--) ;
								ret.Insert(idx, new PXResult<GLTaxTran, Tax, TaxRev>(record, (Tax)line, (TaxRev)line));
							}
						}
						return ret;
					}				
				default:
					return ret;
			}
		}

		protected override IEnumerable<ITaxDetail> ManualTaxes(PXCache sender, object row)
		{
			List<ITaxDetail> ret = new List<ITaxDetail>();

			foreach (PXResult r in SelectTaxes(sender, row, PXTaxCheck.RecalcTotals))
			{
				ret.Add((ITaxDetail)r[0]);
			}
			return ret;
		}

		public override void CacheAttached(PXCache sender)
		{
			if (this.EnableTaxCalcOn(sender.Graph))
			{
				base.CacheAttached(sender);
				sender.Graph.RowInserting.AddHandler(_TaxSumType, Tax_RowInserting);
			}
			else
			{
				this.TaxCalc = TaxCalc.NoCalc;
			}
		}

		virtual protected bool EnableTaxCalcOn(PXGraph aGraph)
		{
			return (aGraph is JournalWithSubEntry);
		}

		protected override void Tax_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			if ((_TaxCalc != TaxCalc.NoCalc && e.ExternalCall || _TaxCalc == TaxCalc.ManualCalc))
			{
				if (!sender.ObjectsEqual<GLTaxTran.curyTaxAmt>(e.OldRow,e.Row))
				{
					_ParentRow = TaxParentAttribute.SelectParent(sender, e.Row, typeof(GLTranDoc));

					if (_ParentRow != null)
					{
						_ParentRow = PXCache<GLTranDoc>.CreateCopy((GLTranDoc)_ParentRow);
					}

					CalcTotals(sender.Graph.Caches[_ChildType], null, false);

					sender.Graph.Caches[_ParentType].Update(_ParentRow);
				}
			}
		}

		protected virtual void Tax_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			if (e.ExternalCall)
			{
				throw new PXSetPropertyException(Messages.TaxDetailCanNotBeInsertedManully);
			}
		}

		protected override void Tax_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			bool atleastonematched = false;
			if ((_TaxCalc != TaxCalc.NoCalc && e.ExternalCall || _TaxCalc == TaxCalc.ManualCalc))
			{
				PXCache cache = sender.Graph.Caches[_ChildType];
				PXCache taxcache = sender.Graph.Caches[_TaxType];

				List<object> details = TaxParentAttribute.ChildSelect(cache, e.Row, _ParentType);
				foreach (object det in details)
				{
					ITaxDetail taxzonedet = MatchesCategory(cache, det, (ITaxDetail)e.Row);
					AddOneTax(taxcache, det, taxzonedet);
					if (taxzonedet != null)
					{
						atleastonematched = true;
					}
				}
				_NoSumTotals = (_TaxCalc == TaxCalc.ManualCalc && e.ExternalCall == false);
				CalcTaxes(cache, null);
				_NoSumTotals = false;

				if (!atleastonematched)
				{
					sender.RaiseExceptionHandling("TaxID", e.Row, ((TaxDetail)e.Row).TaxID, new PXSetPropertyException(TX.Messages.NoLinesMatchTax, PXErrorLevel.RowError));
				}
			}
		}

		protected override void Tax_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			if ((_TaxCalc != TaxCalc.NoCalc && e.ExternalCall || _TaxCalc == TaxCalc.ManualCalc))
			{
				PXCache cache = sender.Graph.Caches[_ChildType];
				PXCache taxcache = sender.Graph.Caches[_TaxType];

				List<object> details = TaxParentAttribute.ChildSelect(cache, e.Row, _ParentType);
				foreach (object det in details)
				{
					DelOneTax(taxcache, det, e.Row);
				}
				CalcTaxes(cache, null);
			}
		}

		protected override void CalcDocTotals(PXCache sender, object row, decimal CuryTaxTotal, decimal CuryInclTaxTotal, decimal CuryWhTaxTotal)
		{
			base.CalcDocTotals(sender, row, CuryTaxTotal, CuryInclTaxTotal, CuryWhTaxTotal);

			if (ParentGetStatus(sender.Graph) != PXEntryStatus.Deleted)
			{
				decimal doc_CuryWhTaxTotal = (decimal)(ParentGetValue(sender.Graph, _CuryWhTaxTotal) ?? 0m);

				if (object.Equals(CuryWhTaxTotal, doc_CuryWhTaxTotal) == false)
				{
					ParentSetValue(sender.Graph, _CuryWhTaxTotal, CuryWhTaxTotal);
				}
				decimal doc_CuryInclTaxTotal = (decimal)(ParentGetValue(sender.Graph, _CuryInclTaxTotal) ?? Decimal.Zero);
				if (object.Equals(CuryInclTaxTotal, doc_CuryInclTaxTotal) == false)
				{
					ParentSetValue(sender.Graph, _CuryInclTaxTotal, CuryInclTaxTotal);
				}
			}
		}

		protected bool _InternalCall = false;

		public override void RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			GLTranDoc doc = (GLTranDoc)e.Row;
			if (doc.ParentLineNbr != null)
			{
				_ParentRow = PXParentAttribute.SelectParent(sender, e.Row, typeof(GLTranDoc));

				if (_ParentRow != null)
				{
					_ParentRow = PXCache<GLTranDoc>.CreateCopy((GLTranDoc)_ParentRow);
				}
				if (doc.CuryTranTotal != doc.CuryTranAmt)
				{
					doc.CuryTranAmt = doc.CuryTranTotal;
				}
				base.RowInserted(sender, e);
				Object current = sender.Current;
				sender.Update(_ParentRow);
				sender.Current = current;
			}
			else
			{
				_ParentRow = doc;
				base.RowInserted(sender, e);
				if (doc.IsChildTran==true)
				{
					doc.CuryTranAmt = this.CalcTaxableFromTotal(sender, doc, doc.CuryTranTotal.Value); 
				}
				else if(doc.Split==false)
				{
					doc.CuryTranAmt = this.CalcTaxableFromTotal(sender, doc, doc.CuryTranTotal.Value);
					CalcTaxes(sender, doc, PXTaxCheck.Line);
				}
			}
		}

		public override void RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			GLTranDoc doc = (GLTranDoc)e.Row;
			if (doc.ParentLineNbr != null)
			{
				_ParentRow = PXParentAttribute.SelectParent(sender, e.Row, typeof(GLTranDoc));
				if (_ParentRow != null)
				{
					_ParentRow = PXCache<GLTranDoc>.CreateCopy((GLTranDoc)_ParentRow);
				}

				if (doc.CuryTranTotal != ((GLTranDoc)e.OldRow).CuryTranTotal)
				{
					doc.CuryTranAmt = doc.CuryTranTotal;
				}
				base.RowUpdated(sender, e);
				Object current = sender.Current;
				sender.Update(_ParentRow);
				sender.Current = current;
			}
			else
			{
				_ParentRow = doc;
				GLTranDoc oldRow = ((GLTranDoc)e.OldRow);
				if (doc.Split == false && doc.CuryTranTotal != oldRow.CuryTranTotal) 
				{
					doc.CuryTranAmt = this.CalcTaxableFromTotal(sender, doc, doc.CuryTranTotal.Value);
				}				
				GLTranDoc copy = PXCache<GLTranDoc>.CreateCopy((GLTranDoc)_ParentRow);
				base.RowUpdated(sender, e);
				if ((doc.TaxCategoryID != oldRow.TaxCategoryID || doc.TaxZoneID != oldRow.TaxZoneID || doc.TranDate != oldRow.TranDate || doc.Split != oldRow.Split || doc.TermsID != oldRow.TermsID) && (doc.Split == false)) 
				{
					
					doc.CuryTranAmt = this.CalcTaxableFromTotal(sender, doc, doc.CuryTranTotal.Value);
					CalcTaxes(sender, doc, PXTaxCheck.Line);
					
				}
				if (_TaxCalc != TaxCalc.NoCalc && _TaxCalc != TaxCalc.ManualLineCalc)
				{
					PXRowUpdatedEventArgs e1 = new PXRowUpdatedEventArgs(e.Row, copy, e.ExternalCall);
					for (int i = 0; i < _Attributes.Count; i++)
					{
						if (_Attributes[i] is IPXRowUpdatedSubscriber)
						{
							((IPXRowUpdatedSubscriber)_Attributes[i]).RowUpdated(sender, e1);
						}
					}
				}				
			}
		}

		public override void RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			GLTranDoc doc = (GLTranDoc)e.Row;
			if (doc.ParentLineNbr != null)
			{
				_ParentRow = PXParentAttribute.SelectParent(sender, e.Row, typeof(GLTranDoc));

				if (_ParentRow != null)
				{
					_ParentRow = PXCache<GLTranDoc>.CreateCopy((GLTranDoc)_ParentRow);
				}
				base.RowDeleted(sender, e);
				Object current = sender.Current;
				sender.Update(_ParentRow);
				sender.Current = current;
			}
			else
			{
				_ParentRow = doc;
				base.RowDeleted(sender, e);
			}
		}

		protected override List<object> ChildSelect(PXCache cache, object data)
		{
			GLTranDoc doc = (GLTranDoc)data;
			if (doc.Split == false && doc.ParentLineNbr == null)
			{
				return new List<object> { data };
			}
			else
			{
				return base.ChildSelect(cache, data);
			}
		}

		public Decimal CalcTaxableFromTotal(PXCache cache, object row, Decimal aCuryTotal) 
		{
			Decimal result = decimal.Zero;
			List<object> taxes = SelectTaxes<Where<True,Equal<True>>>(cache.Graph, row, PXTaxCheck.Line);
			Terms terms = this.SelectTerms(cache.Graph);
			Decimal rate = Decimal.Zero;
			bool haveTaxes = false;
			bool haveTaxOnTax = false;
			foreach (PXResult<GLTax, Tax, TaxRev> iRes in taxes)
			{
				Tax tax = iRes;
				TaxRev taxRev = iRes;
				Decimal multiplier = tax.ReverseTax == true ? Decimal.MinusOne : Decimal.One;
				if (tax.TaxCalcLevel == CSTaxCalcLevel.Inclusive) continue;
				if (tax.TaxCalcLevel == CSTaxCalcLevel.CalcOnItemAmt)
				{
					haveTaxes = true;
					decimal termsFactor = Decimal.Zero;
					if(tax.TaxApplyTermsDisc == CSTaxTermsDiscount.ToTaxableAmount) 
						termsFactor = ((decimal)(terms.DiscPercent ?? 0m) / 100m);
					rate += multiplier * taxRev.TaxRate.Value * (1m - termsFactor);
				}
				if (tax.TaxCalcLevel == CSTaxCalcLevel.CalcOnItemAmtPlusTaxAmt && haveTaxes)
				{
					haveTaxOnTax = true;
					break;
				}
			}

			if (haveTaxOnTax)
				throw new PXException(Messages.TaxableAmountCalculatedError);

			result = PXDBCurrencyAttribute.RoundCury(cache, row, aCuryTotal / (1 + rate / 100));
			Decimal curyTaxTotal = decimal.Zero;
			foreach (PXResult<GLTax, Tax, TaxRev> iRes in taxes)			
			{
				Tax tax = iRes;
				TaxRev taxRev = iRes;
				Decimal multiplier = tax.ReverseTax == true ? Decimal.MinusOne : Decimal.One;
				if (tax.TaxCalcLevel == CSTaxCalcLevel.Inclusive) continue;
				if (tax.TaxCalcLevel == CSTaxCalcLevel.CalcOnItemAmt)
				{
					decimal termsFactor = Decimal.Zero;
					if (tax.TaxApplyTermsDisc == CSTaxTermsDiscount.ToTaxableAmount)
						termsFactor = ((decimal)(terms.DiscPercent ?? 0m) / 100m);					
					Decimal curyTaxAmt = multiplier * (result *(1 - termsFactor) * taxRev.TaxRate / 100m)?? Decimal.Zero;
					if (tax.TaxApplyTermsDisc == CSTaxTermsDiscount.ToTaxAmount) 
					{
						curyTaxAmt *= (1 - (decimal)(terms.DiscPercent ?? 0m) / 100m);
					}
					curyTaxTotal += PXDBCurrencyAttribute.RoundCury(cache, row, curyTaxAmt);
				}
			}
			result = aCuryTotal - curyTaxTotal;
			return result;		
		}
	}

	#region generalLedgerModule

	public sealed class generalLedgerModule : Constant<string>
	{
		public generalLedgerModule() : base(typeof(generalLedgerModule).Namespace){}
	}

	#endregion

	#region accountType
	public sealed class accountType : Constant<string>
	{
		public accountType()
			: base(typeof(PX.Objects.GL.Account).FullName)
		{
		}
	}
	#endregion
	#region subType
	public sealed class subType : Constant<string>
	{
		public subType()
			: base(typeof(PX.Objects.GL.Sub).FullName)
		{
		}
	}
	#endregion

	#region budgetType
	public sealed class budgetType : Constant<string>
	{
		public budgetType()
			: base(typeof(PX.Objects.GL.GLBudgetLine).FullName)
		{
		}
	}
	#endregion

	#region branchType
	public sealed class branchType : Constant<string>
	{
		public branchType()
			: base(typeof(PX.Objects.GL.Branch).FullName)
		{
		}
	}
	#endregion

	public sealed class RunningFlagScope<KeyGraphType> : IDisposable where KeyGraphType : PXGraph
	{
		private class BoolWrapper
		{
			public bool Value { get; set; }
			public BoolWrapper () { this.Value = false; }
		}

		private static string _SLOT_KEY = string.Format("{0}_Running", typeof(KeyGraphType).Name);

		public RunningFlagScope()
		{
			BoolWrapper val = PXDatabase.GetSlot<BoolWrapper>(_SLOT_KEY);
			val.Value = true;
		}

		public void Dispose()
		{
			PXDatabase.ResetSlot<BoolWrapper>(_SLOT_KEY);
		}

		public static bool IsRunning
		{
			get
			{
				return PXDatabase.GetSlot<BoolWrapper>(_SLOT_KEY).Value;
			}
		}
	}

	public class PXRequiredExprAttribute : PXEventSubscriberAttribute
	{
		protected IBqlWhere _Where;

		public PXRequiredExprAttribute(Type where)
		{
			if (where == null)
			{
				throw new PXArgumentException("where", ErrorMessages.ArgumentNullException);
			}
			else if (typeof(IBqlWhere).IsAssignableFrom(where))
			{
				_Where = (IBqlWhere)Activator.CreateInstance(where);
			}
			else
			{
				throw new PXArgumentException("where", ErrorMessages.ArgumentException);
			}

		}

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);
			sender.Graph.RowPersisting.AddHandler(sender.GetItemType(), RowPersisting);
		}

		public void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if (sender.GetStatus(e.Row) == PXEntryStatus.Updated || sender.GetStatus(e.Row) == PXEntryStatus.Inserted)
			{
				bool? result = null;
				object value = null;
				_Where.Verify(sender, e.Row, new List<object>(), ref result, ref value);
				PXDefaultAttribute.SetPersistingCheck(sender, FieldName, e.Row, result.HasValue && result.Value ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
			}
		}
	}
}
