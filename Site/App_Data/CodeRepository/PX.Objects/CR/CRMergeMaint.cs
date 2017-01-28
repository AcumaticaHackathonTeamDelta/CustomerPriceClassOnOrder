using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using PX.Data;
using System.Reflection;

namespace PX.Objects.CR
{
	#region MergeProcessor
	public abstract class MergeProcessor
	{
		public class State : IBqlTable
		{
			public abstract class onlySelected : PX.Data.IBqlField
			{
			}
			protected Boolean? _OnlySelected;
			[PXDBBool]
			public virtual Boolean? OnlySelected
			{
				get
				{
					return this._OnlySelected;
				}
				set
				{
					this._OnlySelected = value;
				}
			}
		}

		protected const string _PROCESSMERGE_ACTION_NAME = "$processMerge";

		private bool _showDetailsAfterMerge;
		private Type _primaryViewItemType;

		public bool ShowDetailsAfterMerge
		{
			get { return _showDetailsAfterMerge; }
			set { _showDetailsAfterMerge = value; }
		}

		public Type PrimaryViewItemType
		{
			get { return _primaryViewItemType; }
			set { _primaryViewItemType = value; }
		}

		public abstract void Initialize();
		public abstract void PerformMerge(bool onlySelected);

		public static bool IsMergeAction(string actionName)
		{
			return !string.IsNullOrEmpty(actionName) && actionName.EndsWith(_PROCESSMERGE_ACTION_NAME);
		}

		public static string ActionName
		{
			get { return _PROCESSMERGE_ACTION_NAME; }
		}
	}

	public class MergeProcessor<BatchTable> : MergeProcessor
		where BatchTable : class, IBqlTable, IPXSelectable, new()
	{
		#region IMergeable
		public interface IMergeable
		{
			IsBatchsEqual EqualHandler { get; }
			CreateGraphForDetails CreateGraphForDetails { get; }
			MergeHandler OnPrePersistHandler { get; }
			MergeHandler OnPostPersistHandler { get; }
		}
		#endregion

		#region Constants
		private const string _MERGING_VIEW_SUFIX = "$Merging";
		private const string _MERGENEWVALUES_VIEW_SUFIX = "$mergeNewValues";
		private const string _MERGESTATE_VIEW_SUFIX = "$mergeState";
		private const string _VALUE_BATCH_FIELD = "Value";

		#endregion

		#region Fields
		private PXGraph _graph;
		private PXCache<BatchTable> _cache;
		private IsBatchsEqual _equalHandler;
		private PXView _newValuesView;
		private CreateGraphForDetails _createGraphForDetails;
		private CRMergeValuesListAttribute _valuesListAttribute;
		private readonly Dictionary<string, string> _mergingViewFieldDic = new Dictionary<string, string>();
		private readonly Dictionary<string, string> _mergingViewValueFieldDic = new Dictionary<string, string>();
		private PXView _onlySelectedState;
		private PXView _view;

		#endregion

		#region Events
		public delegate void MergeHandler(BatchTable newEntity, ICollection<BatchTable> delete);
		public delegate bool IsBatchsEqual(BatchTable x, BatchTable y);
		public delegate PXGraph CreateGraphForDetails(BatchTable selectedItem);

		public event MergeHandler OnPrePersist;
		public event MergeHandler OnPostPersist;
		#endregion

		#region Ctors

		public MergeProcessor(PXGraph graph, PXView view)
		{
			CRHelper.AssertNull(graph, "graph");
			CRHelper.AssertNull(view, "view");
			if (view.GetItemType() != typeof(BatchTable))
				throw new ArgumentException("item type of view must be " + typeof(BatchTable).Name, "view");

			IMergeable mergeGraph = graph as IMergeable;
			if (mergeGraph == null) throw new ArgumentException("graph doesn't implement IMergeable interface.", "graph");

			CRHelper.AssertNull(mergeGraph.EqualHandler, "equalHandler");
			CRHelper.AssertNull(mergeGraph.CreateGraphForDetails, "createGraphForDetails");

			Initialize(graph, view, mergeGraph.EqualHandler, mergeGraph.CreateGraphForDetails, mergeGraph.OnPrePersistHandler, mergeGraph.OnPostPersistHandler);
		}

		public MergeProcessor(PXGraph graph, PXView view, IsBatchsEqual equalHandler, CreateGraphForDetails createGraphForDetails, MergeHandler prePersistHandler, MergeHandler postPersistHandler)
		{
			CRHelper.AssertNull(graph, "graph");
			CRHelper.AssertNull(view, "view");
			CRHelper.AssertNull(equalHandler, "equalHandler");
			CRHelper.AssertNull(createGraphForDetails, "createGraphForDetails");

			Initialize(graph, view, equalHandler, createGraphForDetails, prePersistHandler, postPersistHandler);
		}

		private void Initialize(PXGraph graph, PXView view, IsBatchsEqual equalHandler, CreateGraphForDetails createGraphForDetails, MergeHandler prePersistHandler, MergeHandler postPersistHandler)
		{
			if (view.GetItemType() != typeof(BatchTable))
				throw new ArgumentException("item type of view must be " + typeof(BatchTable).Name, "view");

			_graph = graph;
			_view = view;
			_createGraphForDetails = createGraphForDetails;
			_cache = view.Cache as PXCache<BatchTable>;
			_equalHandler = equalHandler;

			_onlySelectedState = _graph.Views[typeof(BatchTable).Name + _MERGESTATE_VIEW_SUFIX] =
				new PXView(_graph, false, new Select<State>(), new PXSelectDelegate(getState));
			_newValuesView = _graph.Views[typeof(BatchTable).Name + _MERGENEWVALUES_VIEW_SUFIX] =
							   new PXView(_graph, false, new Select<CRMergeNewValues<BatchTable>>(), new PXSelectDelegate(newValues));
			_valuesListAttribute = new CRMergeValuesListAttribute(MergeTypes);
			_valuesListAttribute.OnlySelected = OnlySelectedState;
			OnValuesListPreInit(graph);
			_valuesListAttribute.CacheAttached(_newValuesView.Cache);
			_graph.FieldSelecting.AddHandler(typeof(CRMergeNewValues<BatchTable>), _VALUE_BATCH_FIELD, _valuesListAttribute.FieldSelecting);
			_graph.FieldUpdating.AddHandler(typeof(CRMergeNewValues<BatchTable>), _VALUE_BATCH_FIELD, _valuesListAttribute.FieldUpdating);
			_graph.FieldVerifying.AddHandler(typeof(CRMergeNewValues<BatchTable>), _VALUE_BATCH_FIELD, _valuesListAttribute.FieldVerifying);

			if (prePersistHandler != null) OnPrePersist += prePersistHandler;
			if (postPersistHandler != null) OnPostPersist += postPersistHandler;
		}

		protected virtual void OnValuesListPreInit(PXGraph graph)
		{
			if (!OnlySelectedState) _view.SelectMulti();
		}

		protected virtual Type[] MergeTypes
		{
			get { return new Type[] { typeof(BatchTable) }; }
		}

		#endregion

		#region Public Methods
		public override void Initialize()
		{
			AddMergingViews();

			_graph.FieldUpdating.AddHandler(typeof(CRMergeNewValues<BatchTable>), _VALUE_BATCH_FIELD, TNewValues_Value_FieldUpdating);
			CRHelper.AddNamedAction(_graph, typeof(BatchTable).Name + _PROCESSMERGE_ACTION_NAME,
				PrimaryViewItemType ?? typeof(BatchTable), Process);
		}

		public override void PerformMerge(bool onlySelected)
		{
			if (_newValuesView.Answer == WebDialogResult.None)
				_valuesListAttribute.OnlySelected = OnlySelectedState = onlySelected;
			if (CanPerformMerge(_view.Cache) && _newValuesView.Ask(null, MessageButtons.Panel) == WebDialogResult.OK)
			{
				BatchTable newLead = null;
				List<BatchTable> deletingItems = new List<BatchTable>();
				foreach (BatchTable lead in GetItemsByState(_cache))
					if (!_equalHandler(lead, newLead ?? (newLead = lead)))
					{
						_cache.Delete(lead);
						deletingItems.Add(lead);
					}
				if (newLead != null)
				{
					IDictionary values = GetValues(_newValuesView.Cache.Inserted as IEnumerable<CRMergeNewValues<BatchTable>>);
					_cache.Update(values, values);
					if (OnPrePersist != null) OnPrePersist(newLead, deletingItems);
					CRHelper.SafetyPersist(_cache, PXDBOperation.Update, PXDBOperation.Delete);
					_cache.IsDirty = false;
					if (OnPostPersist != null) OnPostPersist(newLead, deletingItems);
					if (ShowDetailsAfterMerge) ShowDetails(newLead);
					else
					{
						_cache.Clear();
						_view.RequestRefresh();
					}
				}
			}
		}

		public static void ShowDetails(BatchTable item, CreateGraphForDetails createGraphForDetails)
		{
			if (item != null && createGraphForDetails != null)
				throw new PXRedirectRequiredException(createGraphForDetails(item), true, Messages.Details);
		}
		#endregion

		#region Private Methods

		[PXUIField(DisplayName = Messages.Merge)]
		[CRButtonGroup(CRMassProcessingAttribute.MASSPROCESSING_GROUPNAME, ImageUrl = "~/Icons/Menu/process_16.gif")]
		private IEnumerable Process(PXAdapter adapter)
		{
			PerformMerge(CRMassProcessingAttribute.GetOnlySelectedParameter(adapter.Parameters));
			return adapter.Get();
		}

		private void TNewValues_Value_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
		{
			CRMergeNewValues<BatchTable> row = e.Row as CRMergeNewValues<BatchTable>;
			if (row != null && !string.IsNullOrEmpty(row.SelectorViewName))
				e.NewValue = GetBindedValue(row.SelectorViewName,
					GetViewValueFieldName(row.SelectorViewName), row.SelectorDescriptionName, e.NewValue);
		}

		private IEnumerable getState()
		{
			foreach (State item in _onlySelectedState.Cache.Inserted)
			{
				yield return item;
				yield break;
			}
			yield return _onlySelectedState.Cache.Insert();
			_onlySelectedState.Cache.IsDirty = false;
		}

		private IEnumerable newValues()
		{
			if (!OnlySelectedState) _view.SelectMulti();
			BatchTable someSelectedItem = GetSomeItem();
			List<string> avaliableProperties = GetDiffItemsProperties();
			if (someSelectedItem != null)
			{
				SortedList<string, CRFieldNewValues> records = new SortedList<string, CRFieldNewValues>(StringComparer.CurrentCultureIgnoreCase);
				// fill cache
				foreach (CRMergeNewValues<BatchTable> item in _newValuesView.Cache.Inserted)
					if (avaliableProperties.Contains(item.DataField))
					{
						records.Add(item.DataFieldName + item.DataField, item);
						avaliableProperties.Remove(item.DataField);
					}
				foreach (CRFieldNewValues item in GetFields(avaliableProperties))
				{
					item.Value = CRHelper.ObjectToString(GetValueExt(someSelectedItem, item));
					_newValuesView.Cache.Insert(item);
					records.Add(item.DataFieldName + item.DataField, item);
				}
				if (avaliableProperties.Count > 0)
					SetNewItemValues(_cache, someSelectedItem, avaliableProperties, _newValuesView.Cache.Inserted);

				// select rows
				foreach (KeyValuePair<string, CRFieldNewValues> pair in records)
					yield return pair.Value;
			}
			_newValuesView.Cache.IsDirty = false;
			yield break;
		}

		protected virtual object GetValueExt(object row, CRFieldNewValues item)
		{
			return _cache.GetValueExt(row, item.DataField);
		}

		private void ShowDetails(BatchTable item)
		{
			ShowDetails(item, _createGraphForDetails);
		}

		private string GetViewFieldName(string viewName)
		{
			return _mergingViewFieldDic.ContainsKey(viewName) ? _mergingViewFieldDic[viewName] : viewName;
		}

		private string GetViewValueFieldName(string viewName)
		{
			return _mergingViewValueFieldDic.ContainsKey(viewName) ? _mergingViewValueFieldDic[viewName] : null;
		}

		private static IDictionary GetValues(IEnumerable<CRMergeNewValues<BatchTable>> source)
		{
			OrderedDictionary values = new OrderedDictionary();
			foreach (CRMergeNewValues<BatchTable> item in source)
				if (values.Contains(item.DataField)) values[item.DataField] = item.Value;
				else values.Add(item.DataField, item.Value);
			return values;
		}

		private object GetNewValue(IEnumerable<CRMergeNewValues<BatchTable>> values, string fieldName)
		{
			foreach (CRMergeNewValues<BatchTable> value in values)
				if (value.DataField == fieldName) return value.Value;
			return null;
		}

		private List<string> GetDiffItemsProperties()
		{
			List<string> result = new List<string>();
			foreach (KeyValuePair<string, List<object>> pair in GetDiffItems())
				if (pair.Value.Count > 1) result.Add(pair.Key);
			return result;
		}

		private bool CanPerformMerge(PXCache cache)
		{
			return !OnlySelectedState || CRMassProcessingAttribute.ExistSelectedItems(cache, 2);
		}

		private List<object> GetDiffItemsPropertyValues(string fieldName)
		{
			foreach (KeyValuePair<string, List<object>> pair in GetDiffItems())
				if (pair.Key == fieldName) return pair.Value;
			return new List<object>(0);
		}

		private BatchTable GetSomeItem()
		{
			IEnumerator iterator = GetItemsByState(_cache).GetEnumerator();
			if (iterator.MoveNext()) return iterator.Current as BatchTable;
			return null;
		}

		private string GetBindedValue(string viewName, string bindedFieldName, string valueField, object value)
		{
			PXView originalView = _graph.Views[viewName];
			if (originalView != null)
			{
				PXCache originalCache = originalView.Cache;

				Type key = GetKey(originalCache, valueField);

				BqlCommand select = BqlCommand.CreateInstance(typeof(Select<,>), originalCache.BqlTable,
					typeof(Where<,>), key, typeof(Equal<>), typeof(Required<>), key);

				object selectedItem = (new PXView(_graph, true, select)).SelectSingle(value);
				if (selectedItem != null)
				{
					object descriptionValue = PXView.FieldGetValue(originalCache, selectedItem,
						originalCache.BqlTable, bindedFieldName);
					return descriptionValue == null ? null : descriptionValue.ToString();
				}
			}
			return null;
		}

		private static Type GetKey(PXCache originalCache, string keyField)
		{
			return originalCache.GetBqlField(keyField);
		}

		protected static string GetMergingViewName(string key)
		{
			if (key == null) return null;
			return key + _MERGING_VIEW_SUFIX;
		}
		#endregion

		#region Protected Methods
		protected string GetViewName(PXCache cache, string field)
		{
			return ((PXFieldState)cache.GetStateExt(null, field)).ViewName;
		}

		protected bool OnlySelectedState
		{
			get
			{
				IEnumerator iterator = _onlySelectedState.SelectMulti().GetEnumerator();
				if (iterator.MoveNext())
				{
					State current = (State)iterator.Current;
					return current.OnlySelected.HasValue ? current.OnlySelected.Value : true;
				}
				return true;
			}
			set
			{
				foreach (State item in _onlySelectedState.SelectMulti())
					item.OnlySelected = value;
			}
		}

		protected virtual void AddMergingViews()
		{
			foreach (string field in _cache.Fields)
				if (CRFieldNewValues.GetSelectorAttribute(_cache, field) != null || CRFieldNewValues.GetDimensionAttribute(_cache, field) != null)
					AddMergingView(_cache, GetViewName(_cache, field), field);
		}

		protected virtual IEnumerable<CRFieldNewValues> GetFields(List<string> avaliableProperties)
		{
			return CRFieldNewValues.GetFields(typeof(CRMergeNewValues<BatchTable>), _cache, avaliableProperties, GetMergingViewName);
		}

		protected virtual Dictionary<string, List<object>> GetDiffItems()
		{
			return CRMergeValuesListAttribute.GetPropertyValuesPairs(_cache, GetItemsByState(_cache));
		}

		protected IEnumerable GetItemsByState(PXCache cache)
		{
			if (OnlySelectedState) return CRMassProcessingAttribute.GetSelectedItems(cache);
			return CRMassProcessingAttribute.GetAllItems(cache);
		}

		protected void AddMergingView(PXCache cache, string viewName, string field)
		{
			if (viewName == null) return;
			string mergingViewName = GetMergingViewName(field);
			if (!_graph.Views.ContainsKey(mergingViewName))
			{
				PXView originalView = _graph.Views[viewName];
				BqlCommand originalBqlSelect = originalView.BqlSelect;
				PXSelectDelegate handler = new PXSelectDelegate(delegate()
					{
						BqlCommand select = originalBqlSelect;
						List<object> values = GetDiffItemsPropertyValues(GetViewFieldName(viewName));
						if (values.Count > 0)
						{
							Type valueField = GetKey(originalView.Cache, GetViewValueFieldName(viewName));
							select = originalBqlSelect.WhereAnd(In.Create(valueField, values.Count));

						}
						return new PXView(_graph, true, select).SelectMulti(values.ToArray());
					});
				PXView newView = new PXView(_graph, true, originalBqlSelect, handler);
				_mergingViewFieldDic.Add(mergingViewName, field);
				_mergingViewValueFieldDic.Add(mergingViewName,
					(CRFieldNewValues.GetSelectorValueField(cache, field) ?? CRFieldNewValues.GetDimensionValueField(cache, field)) ?? field);
				_graph.Views.Add(mergingViewName, newView);
			}
		}

		protected void SetNewItemValues(PXCache cache, object source, List<string> fields, IEnumerable target)
		{
			foreach (CRFieldNewValues item in target)
				if (fields.Contains(item.DataField))
				{
					item.Value = CRHelper.ObjectToString(cache.GetValueExt(source, item.DataField));
					if (!string.IsNullOrEmpty(item.SelectorViewName))
						item.SelectorDescription = GetBindedValue(item.SelectorViewName,
							item.SelectorDescriptionName, item.SelectorValueName, item.Value);
				}
		}
		#endregion
	}
	#endregion

	#region Correct referencing instances
	public abstract class RefItemsCorrectorBase<TInstance, TRefItem>
		where TInstance : class, IBqlTable, new()
		where TRefItem : class, IBqlTable, new()
	{
		protected readonly bool DeleteReferences = false;
		protected readonly PXGraph Graph;

		protected RefItemsCorrectorBase(PXGraph graph, bool deleteReferences)
		{
			Graph = graph;
			DeleteReferences = deleteReferences;
		}

		public void Correct(TInstance leaveInstance, ICollection<TInstance> deletingInstances)
		{
			PXCache cache = Graph.Caches[typeof(TRefItem)];
			// Move
			foreach (TRefItem item in RefItemsForAction(leaveInstance, deletingInstances, false))
			{
				UpdateItem(cache, leaveInstance, item);
				cache.Update(item);
			}
			// Delete
			foreach (TRefItem item in RefItemsForAction(leaveInstance, deletingInstances, true))
				cache.Delete(item);
		}

		public PXDBOperation[] SaveOperations
		{
			get
			{
				return new PXDBOperation[] { PXDBOperation.Delete, PXDBOperation.Update, PXDBOperation.Insert };
			}
		}

		protected IEnumerable<TRefItem> RefItemsForAction(TInstance leaveInstance, ICollection<TInstance> deletingInstances, bool forDeleting)
		{
			// Constraint
			if (deletingInstances.Count < 1) yield break;

			// Compose command
			BqlCommand command = GetCommand(leaveInstance, deletingInstances, forDeleting);
			if (command == null) yield break;

			// Compose parameters
			object[] deletingIDs = GetInstancesKeys(leaveInstance, deletingInstances, forDeleting);

			// Select
			PXView view = new PXView(Graph, false, command);
			foreach (object item in view.SelectMulti(deletingIDs))
			{
				PXResult record = item as PXResult;
				if (record != null) yield return record[typeof(TRefItem)] as TRefItem;
				else yield return item as TRefItem;
			}
		}

		protected abstract void UpdateItem(PXCache cache, TInstance leaveInstance, TRefItem item);

		protected abstract BqlCommand GetCommand(TInstance leaveInstance, ICollection<TInstance> deletingInstances, bool forDeleting);

		protected abstract object[] GetInstancesKeys(TInstance leaveInstance, ICollection<TInstance> deletingInstances, bool forDeleting);
	}

	public sealed class RefItemsCorrector<TInstance, TRefItem, TRefField> : RefItemsCorrectorBase<TInstance, TRefItem>
		where TInstance : class, IBqlTable, new()
		where TRefItem : class, IBqlTable, new()
		where TRefField : IBqlField
	{
		public delegate object GetInstanceKey(TInstance instance);

		private readonly GetInstanceKey _getKeyDelegate;

		public RefItemsCorrector(PXGraph graph, GetInstanceKey getKeyDelegate)
			: this(graph, getKeyDelegate, false)
		{
		}

		public RefItemsCorrector(PXGraph graph, GetInstanceKey getKeyDelegate, bool deleteReferences)
			: base(graph, deleteReferences)
		{
			_getKeyDelegate = getKeyDelegate;
		}

		protected override void UpdateItem(PXCache cache, TInstance leaveInstance, TRefItem item)
		{
			cache.SetValue(item, typeof(TRefField).Name, _getKeyDelegate(leaveInstance));
		}

		protected override BqlCommand GetCommand(TInstance leaveInstance, ICollection<TInstance> deletingInstances, bool forDeleting)
		{
			if (DeleteReferences ^ forDeleting) return null;

			BqlCommand command = new Select<TRefItem>();
			command = command.WhereNew(In<TRefField>.Create(deletingInstances.Count));
			return command;
		}

		protected override object[] GetInstancesKeys(TInstance leaveInstance, ICollection<TInstance> deletingInstances, bool forDeleting)
		{
			if (DeleteReferences ^ forDeleting) return null;

			object[] deletingIDs = new object[deletingInstances.Count];
			SetInstancesKeys(deletingIDs, 0, deletingInstances);
			return deletingIDs;
		}

		private void SetInstancesKeys(object[] keys, int startIndex, ICollection<TInstance> deletingInstances)
		{
			foreach (TInstance item in deletingInstances)
				keys[startIndex++] = _getKeyDelegate(item);
		}
	}

	//public class RefItemsCorrector2<TInstance, TRefItem, TRefField, TCrossField, TRefItem1, TRefField1, TCrossField1> :
	//    RefItemsCorrector<TInstance, TRefItem, TRefField>
	//    where TRefItem : class, IBqlTable, new()
	//    where TRefField : IBqlField
	//    where TCrossField : IBqlField
	//    where TRefItem1 : class, IBqlTable, new()
	//    where TRefField1 : IBqlField
	//    where TCrossField1 : IBqlField
	//{
	//    public RefItemsCorrector2(PXGraph graph, GetInstanceKey getKeyDelegate)
	//        : base(graph, getKeyDelegate)
	//    {
	//    }

	//    protected override BqlCommand GetCommand(TInstance leaveInstance, ICollection<TInstance> deletingInstances, bool forDeleting)
	//    {
	//        Type deletingClause = forDeleting ? typeof(IsNotNull) : typeof(IsNull);
	//        BqlCommand command = new Select2<TRefItem,
	//                        LeftJoin<TRefItem1, On<TCrossField1, Equal<TCrossField>,
	//                            And<TRefField1, Equal<Required<TRefField1>>>>>>();
	//        command = command.WhereNew(BqlCommand.Compose(typeof(Where<,,>), typeof(TRefField1), deletingClause,
	//            typeof(And<>), In<TRefField>.Create(deletingInstances.Count)));
	//        return command;
	//    }

	//    protected override object[] GetInstancesKeys(TInstance leaveInstance, ICollection<TInstance> deletingInstances, bool forDeleting)
	//    {
	//        object[] deletingIDs = new object[deletingInstances.Count + 1];
	//        deletingIDs[0] = InstanceKeyDelegate(leaveInstance);
	//        SetInstancesKeys(deletingIDs, 1, deletingInstances);
	//        return deletingIDs;
	//    }
	//}
	#endregion

	#region CRMergeableAttribute
	public class CRMergeableAttribute : PXEventSubscriberAttribute
	{
		public readonly bool Enabled;

		public CRMergeableAttribute(bool enabled)
		{
			Enabled = enabled;
		}

		public CRMergeableAttribute() : this(true) { }
	}
	#endregion

	#region CRMergeNewValues

	[Serializable]
	public class CRMergeNewValues<BatchTable> : CRFieldNewValues<BatchTable>
		where BatchTable : class, IBqlTable, IPXSelectable, new()
	{ }

	#region CRMergeValuesListAttribute
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
	public class CRMergeValuesListAttribute : PXEventSubscriberAttribute, IPXFieldSelectingSubscriber, IPXFieldUpdatingSubscriber, IPXFieldVerifyingSubscriber
	{
		private readonly Type[] _valuesType;
		private readonly Dictionary<string, object[]> _valuesDic;
		private readonly Dictionary<string, string[]> _labelsDic;

		private bool _onlySelected;

		public CRMergeValuesListAttribute(params Type[] valuesType)
			: base()
		{
			_valuesType = valuesType;
			_valuesDic = new Dictionary<string, object[]>();
			_labelsDic = new Dictionary<string, string[]>();
		}

		public bool OnlySelected
		{
			get { return _onlySelected; }
			set { _onlySelected = value; }
		}

		public override void CacheAttached(PXCache sender)
		{
			_valuesDic.Clear();
			_labelsDic.Clear();
			if (sender.Graph != null)
				foreach (Type itemType in _valuesType)
					CacheAttached(sender.Graph, itemType);
			base.CacheAttached(sender);
		}

		private void CacheAttached(PXGraph graph, Type itemType)
		{
			PXCache valuesCache;
			if ((valuesCache = graph.Caches[itemType]) != null)
			{
				Dictionary<string, List<object>> values =
					GetPropertyValuesPairs(valuesCache,
										   OnlySelected
											? CRMassProcessingAttribute.GetSelectedItems(valuesCache)
											: CRMassProcessingAttribute.GetAllItems(valuesCache));
				FillArrayDic<object>(values, _valuesDic);
				FillArrayDic<string>(GetPropertyLabelsPairs(valuesCache, values, itemType), _labelsDic);
			}
		}

		private static void FillArrayDic<TValue>(Dictionary<string, List<TValue>> source, Dictionary<string, TValue[]> target)
		{
			foreach (KeyValuePair<string, List<TValue>> pair in source)
				if (!target.ContainsKey(pair.Key)) target.Add(pair.Key, pair.Value.ToArray());
		}

		public static Dictionary<string, List<object>> GetPropertyValuesPairs(PXCache valuesCache, IEnumerable items)
		{
			return GetPropertyValuesPairs(valuesCache, items, valuesCache.Fields);
		}

		public static Dictionary<string, List<object>> GetPropertyValuesPairs(PXCache valuesCache, IEnumerable items, IEnumerable<string> fields)
		{
			Dictionary<string, List<object>> dinamicValuesDic =
				new Dictionary<string, List<object>>();
			List<string> mergeableFields = new List<string>();
			foreach (string field in fields)
			{
				CRMergeableAttribute att = CRHelper.GetCustomAttribute<CRMergeableAttribute>(valuesCache, field);
				if (att == null || att.Enabled) mergeableFields.Add(field);
			}
			foreach (object row in items)
				foreach (string field in mergeableFields)
				{
					if (!dinamicValuesDic.ContainsKey(field))
						dinamicValuesDic.Add(field, new List<object>());

					List<object> currentValuesList = dinamicValuesDic[field];
					object currentValue = valuesCache.GetValueExt(row, field);
					if (!currentValuesList.Contains(currentValue))
						currentValuesList.Add(currentValue);
				}
			return dinamicValuesDic;
		}

		private static Dictionary<string, List<string>> GetPropertyLabelsPairs(PXCache cache, Dictionary<string, List<object>> valuesDic, Type valuesType)
		{
			PXCache propertiesCache = cache.BqlTable == valuesType ? cache : cache.Graph.Caches[valuesType];
			List<string> properties = propertiesCache.Fields;
			Dictionary<string, List<string>> labelsDic =
				new Dictionary<string, List<string>>(properties.Count);

			CRListAttributesHelper currentListHelper;
			foreach (string field in properties)
			{
				PXFieldState state = propertiesCache.GetStateExt(null, field) as PXFieldState;
				if (state == null || !state.Enabled) continue;

				if (!labelsDic.ContainsKey(field) && valuesDic.ContainsKey(field) &&
					(currentListHelper = CRListAttributesHelper.CreateFrom(cache.Graph, propertiesCache, field)) != null)
				{
					labelsDic.Add(field, currentListHelper.GetLabels(valuesDic[field]));
				}
			}
			return labelsDic;
		}

		public virtual void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			CRFieldNewValues row = e.Row as CRFieldNewValues;
			if (row != null && _valuesDic.ContainsKey(row.DataField))
			{
				if (string.Compare(row.DataFieldType, typeof(bool).Name, true) == 0)
					e.ReturnState = PXFieldState.CreateInstance(e.ReturnState, typeof(Boolean), false, null,
						-1, null, null, null, base._FieldName, null, null, null,
						PXErrorLevel.Undefined, null, null, PXUIVisibility.Undefined, null, null, null);
				else
					if (!string.IsNullOrEmpty(row.SelectorViewName))
					{
						e.ReturnState = PXFieldState.CreateInstance(e.ReturnState, typeof(string), false, null,
							-1, null, null, null, base._FieldName, row.SelectorDescriptionName, row.SelectorDescription, null, null,
							PXErrorLevel.Undefined, null, null, PXUIVisibility.Undefined, row.SelectorViewName, null, null);
					}
					else
					{
						string[] values = Array.ConvertAll<object, string>(_valuesDic[row.DataField], CRHelper.ObjectToString);
						e.ReturnState = PXStringState.CreateInstance(e.ReturnState, null, base._FieldName, null,
							-1, null, values, _labelsDic.ContainsKey(row.DataField) ?
							_labelsDic[row.DataField] : values, (bool?)true);
					}
			}
		}

		public virtual void FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
		{
			CRFieldNewValues row = e.Row as CRFieldNewValues;
			if (row != null)
			{
				PXCache fieldCache = GetCacheByField(sender.Graph, row.DataField);
				if (fieldCache != null)
				{
					//TODO: need impelement RaiseFieldUpdating
					//object value = e.NewValue;
					//fieldCache.RaiseFieldUpdating(row.DataField, null, ref value);
					//e.NewValue = value;
				}
			}
		}

		public virtual void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			CRFieldNewValues row = e.Row as CRFieldNewValues;
			if (row != null)
			{
				PXCache fieldCache = GetCacheByField(sender.Graph, row.DataField);
				if (fieldCache != null)
				{
					//TODO: need impelement RaiseFieldVerifying
					//object value = e.NewValue;
					//fieldCache.RaiseFieldVerifying(row.DataField, null, ref value);
					//e.NewValue = value;
				}
			}
		}

		private PXCache GetCacheByField(PXGraph graph, string field)
		{
			foreach (Type cacheType in _valuesType)
			{
				PXCache cache = graph.Caches[cacheType];
				foreach (string cacheField in cache.Fields)
					if (CRHelper.StrEquals(field, cacheField)) return cache;
			}
			return null;
		}
	}
	#endregion
	#endregion

	#region InvalidKeysException
	public class InvalidKeysException : InvalidOperationException
	{
		public InvalidKeysException(Type table)
			: base(string.Format(
			"Merging is imposible, because referenced type {0} contains two or more keys.",
			table))
		{
		}
	}
	#endregion

	#region CRMergeMaint
	public abstract class CRMergeMaint<Graph, BatchTable> :
		PXGraph<Graph, BatchTable>, MergeProcessor<BatchTable>.IMergeable
		where Graph : PXGraph
		where BatchTable : class, IBqlTable, IPXSelectable, new()
	{
		#region Constructors

		protected CRMergeMaint()
			: base()
		{
			Initialize();
		}

		protected CRMergeMaint(PXGraph graph)
			: base(graph)
		{
			Initialize();
		}

		#endregion

		#region Buttons
		public PXAction<BatchTable> details;
		[PXUIField(DisplayName = Messages.Details)]
		[PXButton(ImageUrl = "~/Icons/Menu/entry_16.gif", DisabledImageUrl="~/Icons/Menu/entry_16_NotActive.gif")]
		public IEnumerable Details(PXAdapter adapter)
		{
			ShowDetails();
			return adapter.Get();
		}

		protected void ShowDetails()
		{
			MergeProcessor<BatchTable>.ShowDetails(MargeItems.Current, GetGraphForDetails);
		}

		#endregion

		#region Protected Methods

		protected virtual void Initialize() { }

		protected abstract PXSelectBase<BatchTable> MargeItems { get; }

		protected abstract bool IsBatchsEqual(BatchTable x, BatchTable y);

		protected abstract PXGraph GetGraphForDetails(BatchTable selectedItem);

		protected virtual void PrePersistHandler(BatchTable newInstance, ICollection<BatchTable> deletingInstances)
		{
		}

		protected virtual void PostPersistHandler(BatchTable newInstance, ICollection<BatchTable> deletingInstances)
		{
		}

		protected TGraph CreateGraph<TGraph>()
			where TGraph : PXGraph, new()
		{
			TGraph result = new TGraph();
			result.Clear();
			return result;
		}
		#endregion

		public MergeProcessor<BatchTable>.IsBatchsEqual EqualHandler
		{
			get { return IsBatchsEqual; }
		}

		public MergeProcessor<BatchTable>.CreateGraphForDetails CreateGraphForDetails
		{
			get { return GetGraphForDetails; }
		}

		public MergeProcessor<BatchTable>.MergeHandler OnPrePersistHandler
		{
			get { return PrePersistHandler; }
		}

		public MergeProcessor<BatchTable>.MergeHandler OnPostPersistHandler
		{
			get { return PostPersistHandler; }
		}
	}
	#endregion
}
