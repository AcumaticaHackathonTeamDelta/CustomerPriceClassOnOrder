using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Common;
using PX.Data;
using PX.Objects.GL;

namespace PX.Objects.WZ
{
    #region WizardScenarioStatusesAttribute

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class WizardScenarioStatusesAttribute : PXStringListAttribute
    {
        public const string _PENDING = "PN";
        public const string _ACTIVE = "AC";
        public const string _SUSPEND = "SU";
        public const string _COMPLETED = "CP";

        public WizardScenarioStatusesAttribute()
            : base(new[] { _PENDING, _ACTIVE, _SUSPEND, _COMPLETED },
                    new[] { Messages.Pending, Messages.Active, Messages.Suspend, Messages.Completed })
        { }

        public sealed class Pending : Constant<string>
        {
            public Pending() : base(_PENDING) { }
        }

        public sealed class Active : Constant<string>
        {
            public Active() : base(_ACTIVE) { }
        }

        public sealed class Suspend : Constant<string>
        {
            public Suspend() : base(_SUSPEND) { }
        }

        public sealed class Completed : Constant<string>
        {
            public Completed() : base(_COMPLETED) { }
        }
    }

    #endregion

    #region WizardTaskTypesAttribute

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class WizardTaskTypesAttribute : PXStringListAttribute
    {
        public const string _ARTICLE = "AR";
        public const string _SCREEN = "SC";

        public WizardTaskTypesAttribute()
            : base(new[] { _ARTICLE, _SCREEN },
                    new[] { Messages.Article, Messages.Screen })
        { }

        public sealed class Article : Constant<string>
        {
            public Article() : base(_ARTICLE) { }
        }

        public sealed class Screen : Constant<string>
        {
            public Screen() : base(_SCREEN) { }
        }
    }

    #endregion

    #region WizardTaskStatusesAttribute

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class WizardTaskStatusesAttribute : PXStringListAttribute
    {
        public const string _PENDING = "PN";
        public const string _SKIPPED = "SK";
        public const string _ACTIVE = "AC";
        public const string _DISABLED = "DS";
        public const string _COMPLETED = "CP";
        public const string _OPEN = "OP";

        public WizardTaskStatusesAttribute()
            : base(new[] { _PENDING, _SKIPPED, _ACTIVE, _DISABLED, _COMPLETED, _OPEN },
                    new[] { Messages.Pending, Messages.Skipped, Messages.Active, Messages.Disabled, Messages.Completed, Messages.Open })
        { }

        public sealed class Pending : Constant<string>
        {
            public Pending() : base(_PENDING) { }
        }

        public sealed class Skipped : Constant<string>
        {
            public Skipped() : base(_SKIPPED) { }
        }

        public sealed class Active : Constant<string>
        {
            public Active() : base(_ACTIVE) { }
        }

        public sealed class Disabled : Constant<string>
        {
            public Disabled() : base(_DISABLED) { }
        }

        public sealed class Completed : Constant<string>
        {
            public Completed() : base(_COMPLETED) { }
        }

        public sealed class Open : Constant<string>
        {
            public Open() : base(_OPEN) { }
        }

    }

    #endregion

    #region WZFinPeriod
    public class WZFinPeriodAttribute : FinPeriodIDAttribute
    {

        #region Ctor

        public WZFinPeriodAttribute(Type SourceType, Type SearchType)
		{
			_Attributes.Clear();// = new List<PXEventSubscriberAttribute>();
			PXDBStringAttribute dbstr = new PXDBStringAttribute(FiscalPeriodUtils.FULL_LENGHT);
			dbstr.IsFixed = true;
			_Attributes.Add(dbstr);
            _Attributes.Add(new WZFinPeriodIDFormattingAttribute(SourceType, SearchType));    
            
			_SearchType = SearchType;
		    _SourceType = SourceType;
		}
        #endregion

    }
    #endregion

    public class WZFinPeriodIDFormattingAttribute : PXEventSubscriberAttribute, IPXFieldSelectingSubscriber, IPXFieldUpdatingSubscriber, IPXFieldDefaultingSubscriber
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
        public WZFinPeriodIDFormattingAttribute()
            : base()
        {
        }

        public WZFinPeriodIDFormattingAttribute(Type sourceType, Type searchType)
            : this()
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
            }
        }
        #endregion

        #region Implementation
        private void SourceFieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            object sourcedate = cache.GetValue(e.Row, _sourceField);
            if (sourcedate != null)
            {
                PeriodResult result = GetPeriod(cache, _searchType, (DateTime)sourcedate, new object[] { e.Row }, true);
                string newValue = result;
                bool haspending = false;
                try
                {
                    object pendingValue = cache.GetValuePending(e.Row, _FieldName);
                    if (pendingValue != null && pendingValue != PXCache.NotSetValue)
                    {
                        pendingValue = UnFormatPeriod((string)pendingValue);
                        cache.RaiseFieldVerifying(_FieldName, e.Row, ref pendingValue);
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
                        cache.SetValueExt(e.Row, _FieldName, newValue);
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
                e.NewValue = (string)GetPeriod(sender, _searchType, _sourceDate, new[] { e.Row }, true);
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
}
