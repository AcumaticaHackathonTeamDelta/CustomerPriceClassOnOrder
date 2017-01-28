// This File is Distributed as Part of Acumatica Shared Source Code 
/* ---------------------------------------------------------------------*
*                               Acumatica Inc.                          *
*              Copyright (c) 1994-2011 All rights reserved.             *
*                                                                       *
*                                                                       *
* This file and its contents are protected by United States and         *
* International copyright laws.  Unauthorized reproduction and/or       *
* distribution of all or any portion of the code contained herein       *
* is strictly prohibited and will result in severe civil and criminal   *
* penalties.  Any violations of this copyright will be prosecuted       *
* to the fullest extent possible under law.                             *
*                                                                       *
* UNDER NO CIRCUMSTANCES MAY THE SOURCE CODE BE USED IN WHOLE OR IN     *
* PART, AS THE BASIS FOR CREATING A PRODUCT THAT PROVIDES THE SAME, OR  *
* SUBSTANTIALLY THE SAME, FUNCTIONALITY AS ANY ProjectX PRODUCT.        *
*                                                                       *
* THIS COPYRIGHT NOTICE MAY NOT BE REMOVED FROM THIS FILE.              *
* ---------------------------------------------------------------------*/
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PX.Api;
using PX.Common;

namespace PX.Data
{
    /// <summary>
    /// Untyped interface to access <see cref="PXCache{T}">PXCache&lt;TNode&gt;</see>
    /// without knowledge of the <tt>TNode</tt> type.</summary>
    /// <seealso cref="PXCache{T}"/>
	public abstract partial class PXCache
	{
		#region Ctors

		public PXCache()
		{
			var att = Attribute.GetCustomAttributes(GetItemType(), typeof(PXCacheNameAttribute), true).FirstOrDefault() as PXCacheNameAttribute;
			if (att != null)
				DisplayName = att.Name;
		}

		//static PXCache()
		//{
		//	var ret = FindExtensionTypes();
		//	_AvailableExtensions = ret;
		//}

	    internal static Type[] FindExtensionTypes(List<Exception> errorLog = null)
	    {
		    List<Type> ret = new List<Type>();
		    foreach (System.Reflection.Assembly a in AppDomain.CurrentDomain.GetAssemblies())
		    {
			    // ignore some assemblies including dynamic ones
			    if (!PXSubstManager.IsSuitableTypeExportAssembly(a, false))
				    continue;
			    try
			    {
				    Type[] types = null;
				    try
				    {
					    if (!a.IsDynamic)
						    types = a.GetExportedTypes();
				    }
				    catch (System.Reflection.ReflectionTypeLoadException te)
				    {
					    types = te.Types;
				    }
				    if (types == null)
				    {
					    continue;
				    }
				    // iterate through assembly types
				    foreach (Type t in types)
				    {
					    // processing extension objects
					    if (typeof (PXCacheExtension).IsAssignableFrom(t))
					    {
						    ret.Add(t);
						    if (t.IsDefined(typeof (PXDBInterceptorAttribute), true)
						        && t.BaseType.IsGenericType)
						    {
							    PXDBInterceptorAttribute inter =
								    (PXDBInterceptorAttribute) t.GetCustomAttributes(typeof (PXDBInterceptorAttribute), true)[0];
							    Type table = t.BaseType.GetGenericArguments()[t.BaseType.GetGenericArguments().Length - 1];
							    while (table != typeof (object))
							    {
								    if ((table.BaseType == typeof (object) || !typeof (IBqlTable).IsAssignableFrom(table.BaseType)) &&
								        typeof (IBqlTable).IsAssignableFrom(table) || table.IsDefined(typeof (PXTableAttribute), false))
								    {
									    List<KeyValuePair<string, List<string>>> extensions;
									    if (!_ExtensionTables.TryGetValue(table.Name, out extensions))
									    {
										    _ExtensionTables[table.Name] = extensions = new List<KeyValuePair<string, List<string>>>();
									    }
									    extensions.Add(new KeyValuePair<string, List<string>>(t.Name, inter.Keys));
								    }
								    table = table.BaseType;
							    }
						    }
					    }
				    }
			    }
			    catch(Exception ex)
			    {
					if(errorLog!=null)
						errorLog.Add(new Exception("Failed to load types from the assembly: " + a.FullName + "\nWith message: " + ex.Message, ex));
			    }
		    }
			ret.Sort((a, b) => String.Compare(a.FullName, b.FullName));
		    return ret.ToArray();
	    }

	    private static Dictionary<string, List<KeyValuePair<string, List<string>>>> _ExtensionTables = new Dictionary<string, List<KeyValuePair<string, List<string>>>>(StringComparer.OrdinalIgnoreCase);

		internal static List<KeyValuePair<string, List<string>>> GetExtensionTables(string table)
		{
			List<KeyValuePair<string, List<string>>> ret;
			_ExtensionTables.TryGetValue(table, out ret);
			return ret;
		}

		#endregion

		#region Attributes

		/// <summary>
		/// The flag controlled by the
		/// <see cref="PXDisableCloneAttributesAttribute">PXDisableCloneAttributes</see>
		/// attribute.
		/// </summary>
		public bool DisableCloneAttributes;


		internal bool NeverCloneAttributes;

		internal bool DisableCacheClear;
		public bool DisableReadItem;

		protected List<PXEventSubscriberAttribute> _CacheAttributes;
		protected Dictionary<object, DirtyItemState> _ItemAttributes;
		protected PXEventSubscriberAttribute[] _CacheAttributesWithEmbedded;
		protected PXEventSubscriberAttribute[] _ReusableItemAttributes;
		ILookup<Type, PXEventSubscriberAttribute> _AttributesByType;
		ILookup<string, PXEventSubscriberAttribute> _AttributesByName;
		Func<object, object, bool>[] _AttributeComparers;





		/// <summary>
		/// Returns the cach-level instances of attributes placed on the
		/// specified field and all item-level instances currently stored
		/// in the cache.
		/// </summary>
		/// <param name="name">The name of the field whose attributes are returned.
		/// If <tt>null</tt>, the method returns attributes from all fields.</param>
		public abstract List<PXEventSubscriberAttribute> GetAttributes(string name);
		public abstract List<PXEventSubscriberAttribute> GetAttributesReadonly(string name);
		public abstract List<PXEventSubscriberAttribute> GetAttributesReadonly(string name, bool extractEmmbeddedAttr);

		public abstract IEnumerable<PXEventSubscriberAttribute> GetAttributesReadonly(object data, string name);
		/// <summary>
		/// Returns collection of row specific attributes
		/// This method creates a copy of all cache level attributes 
		/// and stores this clones to internal collection that contains row specific attributes
		/// To avoid cloning cache level attributes, use GetAttributesReadonly method
		/// </summary>
		public abstract IEnumerable<PXEventSubscriberAttribute> GetAttributes(object data, string name);

	    public virtual IEnumerable<T> GetAttributesOfType<T>(object data, string name)
		    where T : PXEventSubscriberAttribute
	    {
		    return GetAttributes(data, name).OfType<T>();
	    }

	    public abstract bool HasAttributes(object data);
        /// <summary>
        /// Returns the cach-level instances of attributes placed on the
        /// specified field and all item-level instances currently stored
        /// in the cache.
        /// </summary>
        /// <typeparam name="Field">The DAC field.</typeparam>
		public List<PXEventSubscriberAttribute> GetAttributes<Field>()
			where Field : IBqlField
		{
			return GetAttributes(typeof(Field).Name);
		}
        /// <summary>
        /// Returns the cache-level instances of attributes placed on the specified
        /// field in the DAC. The field is specified as the type parameter.
        /// </summary>
        /// <typeparam name="Field">The DAC field.</typeparam>
		public List<PXEventSubscriberAttribute> GetAttributesReadonly<Field>()
			where Field : IBqlField
		{
			return GetAttributesReadonly(typeof(Field).Name);
		}
        /// <summary>
        /// Returns the item-level instances of attributes placed on the
        /// specified field if such instances exist for the provided data
        /// record or the cache-level instances otherwise. The field is
        /// specified as the type parameter.
        /// </summary>
        /// <typeparam name="Field">The DAC field.</typeparam>
        /// <param name="data">The data record.</param>
		public IEnumerable<PXEventSubscriberAttribute> GetAttributesReadonly<Field>(object data)
			where Field : IBqlField
		{
			return GetAttributesReadonly(data, typeof(Field).Name);
		}
        /// <summary>
        /// Returns the item-level instances of attributes placed on the
        /// specified field. If such instances are not exist for the
        /// provided data record, the method creates them by copying all
        /// cache-level attributes and storing them in the internal collection
        /// that contains the data record specific attributes.</summary>
        /// <remarks>
        /// To avoid cloning cache-level attributes, use the
        /// <see cref="PXCache{T}.GetAttributesReadonly(object,string)">GetAttributesReadonly(object, string)</see>
        /// method. The field is specified as the type parameter.
        /// </remarks>
        /// <typeparam name="Field">The DAC field.</typeparam>
        /// <param name="data">The data record.</param>
        /// <example>
        /// <code>
        /// foreach (PXEventSubscriberAttribute attr in sender.GetAttributes&lt;Field&gt;(data))
        /// {
        ///     if (attr is PXUIFieldAttribute)
        ///     {
        ///         // Doing something
        ///     }
        /// }
        /// </code>
        /// </example>
		public IEnumerable<PXEventSubscriberAttribute> GetAttributes<Field>(object data)
			where Field : IBqlField
		{
			return GetAttributes(data, typeof(Field).Name);
		}
		public abstract List<Type> GetExtensionTables();

		internal abstract PXCacheExtension[] GetCacheExtensions(IBqlTable item);





	    /// <summary>
	    ///initializes internal collections: _ItemAttributes, _CacheAttributesWithEmbedded, _ReusableItemAttributes, _AttributesByType
	    /// </summary>
	    protected void InitItemAttributesCollection()
	    {
		    if (_ItemAttributes != null)
			    return;

		    _ItemAttributes = new Dictionary<object, DirtyItemState>();

		    if (_CacheAttributesWithEmbedded == null) //_ItemAttributes cleared sometimes
			{

			    _CacheAttributesWithEmbedded = GetAttributesReadonly(null, true).ToArray();
			    for (int i = 0; i < _CacheAttributesWithEmbedded.Length; i++)
			    {
				    _CacheAttributesWithEmbedded[i].IndexInClonesArray = i;
			    }
			    _ReusableItemAttributes = new PXEventSubscriberAttribute[_CacheAttributesWithEmbedded.Length];

			    var q = from a in _CacheAttributesWithEmbedded
				    from t in a.GetType().CreateList(_ => _.BaseType)
				    where typeof (PXEventSubscriberAttribute).IsAssignableFrom(t) && t != typeof (PXEventSubscriberAttribute)
				    select new {t, a};

			    this._AttributesByType = q.ToLookup(_ => _.t, _ => _.a);


			    this._AttributesByName = _CacheAttributesWithEmbedded.ToLookup(_ => _.FieldName, _ => _,
				    StringComparer.OrdinalIgnoreCase);



			    _AttributeComparers = _CacheAttributesWithEmbedded.Select(GetAttributeComparer).ToArray();

		    }
	    }

	    /// <summary>
		/// Ensures cleanup of item attributes after foreach statement
		/// </summary>
		protected class DisposableAttributesList : IPXAttributeList
		{
			public DisposableAttributesList(PXCache c, object data)
			{
				this.cache = c;

				this.Buffer = c.GetItemState(data);
				if (this.Buffer.RefCnt > 1)
				{
					Buffer.RefCnt--;
					_disposed = true;
				}
			}

			public string NameFilter;
			public PXCache cache;

			public DirtyItemState Buffer;

			private bool _disposed;
			internal Type TypeFilter;


			public void Dispose()
			{
				if (_disposed)
					return;
				_disposed = true;
				cache.CompressItemState(Buffer);

			}



			public IEnumerator<PXEventSubscriberAttribute> GetEnumerator()
			{
				return EnumItems().GetEnumerator();

			}



			bool IsMatchFilters(PXEventSubscriberAttribute a)
			{
				if (NameFilter != null && !NameFilter.Equals(a.FieldName, StringComparison.OrdinalIgnoreCase))
					return false;

				if (TypeFilter != null && !TypeFilter.IsInstanceOfType(a))
					return false;

				return true;
			}
			IEnumerable<PXEventSubscriberAttribute> EnumItems()
			{
				using (this)
				{
					var d = Buffer.DirtyAttributes;
					var p = cache._CacheAttributesWithEmbedded;
					if (NameFilter != null)
					{

						if (this.cache._AttributesByName.Contains(NameFilter))
						{
							foreach (var a in this.cache._AttributesByName[NameFilter])
							{
								if (!IsMatchFilters(a))
									continue;
								var i = a.IndexInClonesArray;
								var result = cache.CheckoutItemAttribute(Buffer, i);

								yield return result;
							}

						}

					}
					else if (TypeFilter != null)
					{
						if (this.cache._AttributesByType.Contains(TypeFilter))
						{
							foreach (var a in this.cache._AttributesByType[TypeFilter])
							{
								if (!IsMatchFilters(a))
									continue;
								var i = a.IndexInClonesArray;
								var result = cache.CheckoutItemAttribute(Buffer, i);

								yield return result;
							}

						}

					}
					else
					{
						for (int i = 0; i < d.Length; i++)
						{
							var result = cache.CheckoutItemAttribute(Buffer, i);
							yield return result;
						}
					}
				}

			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				//return new Enumerator(this);
				return this.EnumItems().GetEnumerator();

			}



		}


		/// <summary>
		/// Compares attributes by field values
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		bool AttributesEqual(PXEventSubscriberAttribute a, PXEventSubscriberAttribute b)
		{
			if (object.ReferenceEquals(a, b))
				return true;

			bool eq = _AttributeComparers[a.IndexInClonesArray](a, b);

			return eq;
		}
		static Func<object, object, bool> GetAttributeComparer(PXEventSubscriberAttribute a)
		{
			var r = _Comparers.GetOrAdd(a.GetType(), type => new CompiledAttributeComparer(type));
			return r.CompiledMethod;
		}
		static readonly ConcurrentDictionary<Type, CompiledAttributeComparer> _Comparers = new ConcurrentDictionary<Type, CompiledAttributeComparer>();

		class CompiledAttributeComparer
		{
			public CompiledAttributeComparer(Type t)
			{
				T = t;
			}
			readonly Type T;
			Func<object, object, bool> _CompiledMethod;

			public Func<object, object, bool> CompiledMethod
			{
				get
				{
					if (_CompiledMethod == null)
					{
						lock (this)
						{
							if (_CompiledMethod == null)
								_CompiledMethod = CompileAttributeComparer();
						}

					}

					return _CompiledMethod;
				}
			}

			Func<object, object, bool> CompileAttributeComparer()
			{
				var flist = new List<FieldInfo>();
				for (var t = T; t != typeof(object); t = t.BaseType)
				{

					if (t == typeof(PXEventSubscriberAttribute))
						break;

					var flds = t.GetFields(BindingFlags.Instance
										   | BindingFlags.Public
										   | BindingFlags.NonPublic
										   | BindingFlags.DeclaredOnly
						)
						.OrderBy(_ => _.Name).ToArray();

					if (t == typeof(PXAggregateAttribute))
						flds = flds.Where(_ => _.Name != "_Attributes").ToArray();

					flist.AddRange(flds);
				}

				var aParam = Expression.Parameter(typeof(object), "a");
				var bParam = Expression.Parameter(typeof(object), "b");

				//var bodyList = new List<Expression>();
				Expression ret = Expression.Constant(true);

				foreach (var fieldInfo in flist)
				{

					var cmp = Expression.Equal(
						Expression.Field(Expression.Convert(aParam, fieldInfo.DeclaringType), fieldInfo),
						Expression.Field(Expression.Convert(bParam, fieldInfo.DeclaringType), fieldInfo)

						);

					ret = Expression.And(cmp, ret);

				}

				var result = Expression.Lambda<Func<object, object, bool>>(ret, aParam, bParam).Compile();
				return result;

			}

		}


		/// <summary>
		/// Container for dirty attributes in _ItemAttributes collection
		/// </summary>
		protected class DirtyItemState
		{
			public PXEventSubscriberAttribute[] DirtyAttributes;
			//public Dictionary<int, PXEventSubscriberAttribute> DirtyAttributes;
			public int RefCnt;
			public object BoundItem;


		}




		public static void TryDispose(object obj)
		{
			var d = obj as IDisposable;
			d?.Dispose();
		}

		/// <summary>
		/// Clones attribute to per item collection
		/// </summary>
		/// <param name="state"></param>
		/// <param name="idx"></param>
		/// <returns></returns>
		PXEventSubscriberAttribute CheckoutItemAttribute(DirtyItemState state, int idx)
		{
			var result = state.DirtyAttributes[idx];
			if (result == null)
			{

				result = _ReusableItemAttributes[idx];
				if (result != null)
				{
					_ReusableItemAttributes[idx] = null;

					//sanity check
					if (!AttributesEqual(result, result.Prototype))
						throw new PXException("NewItemAttribute is dirty.");

					if (!AttributesEqual(result, _CacheAttributesWithEmbedded[idx]))
						result = null;
				}


				if (result == null)
				{
					result = _CacheAttributesWithEmbedded[idx].Clone(PXAttributeLevel.Item);
					result.Prototype = _CacheAttributesWithEmbedded[idx].Clone(PXAttributeLevel.Item);



				}

				state.DirtyAttributes[idx] = result;

			}
			var aggr = result as PXAggregateAttribute;
			if (aggr != null)
			{
				var list = aggr.InternalAttributesAccessor;
				for (int i = 0; i < list.Count; i++)
				{
					var a = list[i];
					//if (a == null || a.AttributeLevel == PXAttributeLevel.Item)
					//	continue;
					list[i] = CheckoutItemAttribute(state, a.IndexInClonesArray);
				}
			}


			return result;
		}

	    private PXEventSubscriberAttribute[] _reusableContainer;
		protected DirtyItemState GetItemState(object row)
		{
			if (row == null || _ItemAttributes == null)
				return null;



			DirtyItemState state;
			if (_ItemAttributes.TryGetValue(row, out state))
			{
				state.RefCnt++;

				if (state.DirtyAttributes == null)
				{
					if (_reusableContainer != null)
					{
						state.DirtyAttributes = _reusableContainer;
						_reusableContainer = null;
					}
					else
					{
						state.DirtyAttributes = new PXEventSubscriberAttribute[_ReusableItemAttributes.Length];
					}
				}
			}



			return state;

		}


		/// <summary>
		/// Check if a subscriber attribute has an item related dirty clone
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="buffer"></param>
		/// <param name="subscriber"></param>
		/// <returns></returns>
		protected T GetDirtyAttribute<T>(DirtyItemState buffer, T subscriber)
		where T : class
		{
			var a = subscriber as PXEventSubscriberAttribute;

			if (buffer == null)
			{
				if (a.AttributeLevel != PXAttributeLevel.Cache)
					throw new PXException("GetDirtyAttribute level");
				//a.Version++;
				return subscriber;
			}


			var bound = CheckoutItemAttribute(buffer, a.IndexInClonesArray);


			return bound as T;
		}


	    /// <summary>
	    /// Cleanup unmodified attributes from the item state
	    /// </summary>
	    /// <param name="buffer"></param>
	    protected void CompressItemState(DirtyItemState buffer)
	    {
		    if (buffer == null)
			    return;
		    buffer.RefCnt--;
		    if (buffer.RefCnt > 0)
			    return;

		    if (buffer.RefCnt < 0)
			    throw new PXException("ReleaseItemState RefCnt<0");

		    bool hasDirty = false;
		    for (int index = 0; index < buffer.DirtyAttributes.Length; index++)
		    {
			    var attribute = buffer.DirtyAttributes[index];
			    if (attribute == null)
				    continue;

			    bool isDirty = attribute.IsDirty;
			    if (!isDirty)
			    {
				    isDirty = attribute.IsDirty = !AttributesEqual(attribute, attribute.Prototype);
			    }
			    if (isDirty)
			    {
				    hasDirty = true;
				    attribute.Prototype = null;
				    continue;
			    }

			    _ReusableItemAttributes[index] = attribute;
			    buffer.DirtyAttributes[index] = null;

		    }
		    if (!hasDirty)
		    {
			    _reusableContainer = buffer.DirtyAttributes;
			    buffer.DirtyAttributes = null;
		    }

	    }







	    #endregion

		#region Field Manipulation
		internal Dictionary<string, int> _FieldsMap;
		protected HashSet<string> _BypassAuditFields;

		/// <summary>
		/// Bypass collecting values for audit. May be used for passwords ans so on.
		/// </summary>
        /// <exclude/>
		public virtual HashSet<string> BypassAuditFields
		{
			get
			{
				if (_BypassAuditFields == null)
				{
					_BypassAuditFields = new HashSet<string>();
				}
				return _BypassAuditFields;
			}
		}

		protected HashSet<string> _AlteredFields;
		protected HashSet<string> _SecuredFields;
		protected HashSet<string> _SelectingFields;
		protected HashSet<int> _KeyValueStoredOrdinals;
		protected internal Dictionary<string, int> _KeyValueStoredNames;
		protected KeyValuePair<string, int>? _FirstKeyValueStored;
		internal virtual HashSet<string> SelectingFields
		{
			get
			{
				if (_SelectingFields == null)
				{
					_SelectingFields = new HashSet<string>();
				}
				return _SelectingFields;
			}
		}
		internal virtual HashSet<string> SecuredFields
		{
			get
			{
				if (_SecuredFields == null)
				{
					_SecuredFields = new HashSet<string>();
				}
				return _SecuredFields;
			}
		}

        /// <summary>
        /// Force calculating PXFieldState in methods GetValueExt, GetValueInt. <br/>
        /// This methods internally calls OnFieldSelecting event with flag IsAltered
        /// </summary>
		public virtual ISet<string> AlteredFields
		{
			get
			{
				if (_AlteredFields == null)
				{
					_AlteredFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				}
				return _AlteredFields;
			}
		}
        /// <summary>
        /// Adds or removes the specified field to/from the <tt>AlteredFields</tt> list.
        /// </summary>
        /// <typeparam name="Field">The DAC field.</typeparam>
        /// <param name="isAltered">The value indicating whether the field is added
        /// (<tt>true</tt>) or removed (<tt>false</tt>) from the <tt>AlteredFields</tt> list.</param>
        /// <example>
        /// <code>
        /// Items.Cache.SetAltered&lt;FlatPriceItem.inventoryID&gt;(true);
        /// </code>
        /// </example>
		public virtual void SetAltered<Field>(bool isAltered)
			where Field : IBqlField
		{
			SetAltered(typeof(Field).Name, isAltered);
		}
        /// <summary>
        /// Adds or removes the specified field to/from the <tt>AlteredFields</tt> list.
        /// </summary>
        /// <param name="field">The name of the DAC field.</param>
        /// <param name="isAltered">The value indicating whether the field is added
        /// (<tt>true</tt>) or removed (<tt>false</tt>) from the <tt>AlteredFields</tt> list.</param>
		public virtual void SetAltered(string field, bool isAltered)
		{
			if (!String.IsNullOrEmpty(field))
			{
				if (!isAltered)
				{
					if (_AlteredFields != null)
					{
						_AlteredFields.Remove(field.ToLower());
					}
				}
				else 
				{
					AlteredFields.Add(field.ToLower());
				}
			}
		}

        /// <summary>
        /// Sets value without any validation or event handling
        /// </summary>
        /// <param name="data">IBqlTable or IDictionary</param>
        /// <param name="ordinal"></param>
        /// <param name="value"></param>
		public abstract void SetValue(object data, int ordinal, object value);
		public abstract object GetValue(object data, int ordinal);
        /// <summary>
        /// sets value without any validation or event handling
        /// </summary>
        /// <param name="data">IBqlTable or IDictionary</param>
		public abstract void SetValue(object data, string fieldName, object value);

        /// <summary>
        /// Gets field value by field name without raising any events
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
		public abstract object GetValue(object data, string fieldName);
        /// <summary>
        /// Set field value. <br/>
        /// Raises events: OnFieldUpdating, OnFieldVerifying, OnFieldUpdated. <br/>
        /// If exception - OnExceptionHandling. <br/>
        /// </summary>
        /// <param name="data">IBqlTable or IDictionary</param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
		public abstract void SetValueExt(object data, string fieldName, object value);

		internal abstract object GetCopy(object data);
        /// <summary>
        /// Raises OnFieldUpdating event to set field value
        /// </summary>
        /// <param name="data">IBqlTable or IDictionary</param>
        /// <param name="fieldName"></param>
		public abstract void SetDefaultExt(object data, string fieldName, object value = null);
        /// <summary>
        /// Sets the value of the field in the provided data record
        /// without raising events.
        /// </summary>
        /// <typeparam name="Field">The field that is set to the
        /// value.</typeparam>
        /// <param name="data">The data record.</param>
        /// <param name="value">The value to set to the field.</param>
        /// <example>
        /// The code below shows an event handler that sets values to
        /// three fields of the <tt>ARTran</tt> data record on update
        /// of the <tt>UOM</tt> field: two fields are assigned their
        /// default values and the <tt>UnitCost</tt> field is set
        /// directly.
        /// <code>
        /// protected virtual void APTran_UOM_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        /// {
        ///     APTran tran = (APTran)e.Row;
        ///     sender.SetDefaultExt&lt;APTran.unitCost&gt;(tran);
        ///     sender.SetDefaultExt&lt;APTran.curyUnitCost&gt;(tran);
        ///     sender.SetValue&lt;APTran.unitCost&gt;(tran, null);
        /// }
        /// </code>
        /// </example>
		public void SetValue<Field>(object data, object value)
			where Field : IBqlField
		{
			SetValue(data, typeof(Field).Name, value);
		}
		public class ExternalCallMarker
		{
			public readonly object Value;
			public ExternalCallMarker(object value)
			{
				Value = value;
			}
			public static object Unwrap(object value)
			{
				if (value is ExternalCallMarker)
				{
					return ((ExternalCallMarker)value).Value;
				}
				return value;
			}
		}
        /// <summary>Sets the value of the field in the provided data
        /// record.</summary>
        /// <remarks>The method raises the <tt>FieldUpdating</tt>,
        /// <tt>FieldVerifying</tt>, and <tt>FieldUpdated</tt> events. To set the
        /// value to the field without raising events, use the <see
        /// cref="PXCache{T}.SetValue(object,string,object)">SetValue(object,
        /// string, object)</see> method.</remarks>
        /// <typeparam name="Field">The field that is set to the
        /// value.</typeparam>
        /// <param name="data">The data record.</param>
        /// <param name="value">The value to set to the field.</param>
        /// <example>
        /// <code>
        /// protected virtual void APInvoice_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        /// {
        ///     APInvoice doc = e.Row as APInvoice;
        /// 
        ///     if (doc.Released != true &amp;&amp; doc.Prebooked != true)
        ///     {
        ///         if (doc.CuryDocBal != doc.CuryOrigDocAmt)
        ///         {
        ///             if (doc.CuryDocBal != null &amp;&amp; doc.CuryDocBal != 0)
        ///                 sender.SetValueExt&lt;APInvoice.curyOrigDocAmt&gt;(doc, doc.CuryDocBal);
        ///             else
        ///                 sender.SetValueExt&lt;APInvoice.curyOrigDocAmt&gt;(doc, 0m);
        ///         }
        ///         ...
        ///     }
        /// }
        /// </code>
        /// </example>
		public void SetValueExt<Field>(object data, object value)
			where Field : IBqlField
		{
			SetValueExt(data, typeof(Field).Name, value);
		}
        /// <summary>Sets the default value to the field in the provided data
        /// record.</summary>
        /// <remarks>The method raises <tt>FieldDefaulting</tt>,
        /// <tt>FieldUpdating</tt>, <tt>FieldVerifying</tt>, and
        /// <tt>FieldUpdated</tt>.</remarks>
        /// <typeparam name="Field">The field to set.</typeparam>
        /// <param name="data">The data record.</param>
        /// <example>
        /// The code below sets default values to the fields of a
        /// <tt>Location</tt> data record.
        /// <code>
        /// // The data view to select Location data records
        /// public PXSelect&lt;Location,
        ///     Where&lt;Location.bAccountID, Equal&lt;Current&lt;BAccount.bAccountID&gt;&gt;&gt;,
        ///     OrderBy&lt;Asc&lt;Location.locationID&gt;&gt;&gt; IntLocations;
        /// ...
        /// public virtual void InitVendorLocation(Location aLoc, string aLocationType)
        /// {
        ///     this.IntLocations.Cache.SetDefaultExt&lt;Location.vCarrierID&gt;(aLoc);
        ///     this.IntLocations.Cache.SetDefaultExt&lt;Location.vFOBPointID&gt;(aLoc);
        ///     this.IntLocations.Cache.SetDefaultExt&lt;Location.vLeadTime&gt;(aLoc);
        ///     this.IntLocations.Cache.SetDefaultExt&lt;Location.vShipTermsID&gt;(aLoc);
        ///     ...
        /// }
        /// </code>
        /// </example>
		public void SetDefaultExt<Field>(object data)
			where Field : IBqlField
		{
			SetDefaultExt(data, typeof(Field).Name);
		}

        /// <summary>
        /// Gets value by field name.<br/>
        /// Raises OnFieldSelecting event.<br/>
        /// Returns pure value or PXFieldState( if field is in AlteredFields List ).
        /// </summary>
        /// <param name="data">IBqlTable or IDictionary</param>
        /// <param name="fieldName"></param>
        /// <returns>Pure value or PXFieldState</returns>
		public abstract object GetValueExt(object data, string fieldName);


        /// <summary>
        /// The same as GetValueExt
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        /// <exclude/>
		public abstract object GetValueInt(object data, string fieldName);

	    /// <exclude/>
	    internal abstract object GetStateInt(object data, string fieldName);

        /// <summary>
        /// Gets the field state by the field name.
        /// </summary>
        /// <remarks>Raises the <tt>FieldSelecting</tt> event.</remarks>
        /// <param name="data">IBqlTable or IDictionary</param>
        /// <param name="fieldName"></param>
        /// <returns>A <tt>PXFieldState</tt> object generated in the <tt>FieldSelecting</tt> event</returns>
		public abstract object GetStateExt(object data, string fieldName);
        /// <summary>
        /// Gets the value of the specified field in the provided data record.
        /// </summary>
        /// <typeparam name="Field">The field.</typeparam>
        /// <param name="data">The data record.</param>
        /// <returns>The field value.</returns>
        /// <example>
        /// The code below shows a <tt>FieldDefaulting</tt> event handler which
        /// gets the default value for the <tt>LocationExtAddress.VPaymentByType</tt> field
        /// from the current <tt>VendorClass</tt> data record.
        /// <code>
        /// protected virtual void LocationExtAddress_VPaymentByType_FieldDefaulting(
        ///     PXCache sender, PXFieldDefaultingEventArgs e)
        /// {
        ///     if (VendorClass.Current != null)
        ///     {
        ///         e.NewValue = VendorClass.Cache.GetValue&lt;VendorClass.paymentByType&gt;(VendorClass.Current);
        ///         e.Cancel = true;
        ///     }
        /// }
        /// </code>
        /// </example>
		public object GetValue<Field>(object data)
			where Field : IBqlField
		{
			return GetValue(data, typeof(Field).Name);
		}
        /// <summary>Returns the value or the <tt>PXFieldState</tt> object of the
        /// specified field in the given data record. The <tt>PXFieldState</tt>
        /// object is returned if the field is in the <tt>AlteredFields</tt>
        /// collection.</summary>
        /// <remarks>The method raises the <tt>FieldSelecting</tt> event.</remarks>
        /// <param name="data">The data record.</param>
        /// <typeparam name="Field">The field whose value or
        /// <tt>PXFieldState</tt> object is returned.</typeparam>
        /// <returns>The field value or <tt>PXFieldState</tt> object.</returns>
        /// <example>
        /// The code below shows a <tt>RowDeleted</tt> event handler in which you
        /// get the value (or field state) of the <tt>IsDefault</tt> field
        /// of the <tt>POVendorInventory</tt> data record.
        /// <code>
        /// protected virtual void POVendorInventory_RowDeleted(PXCache cache, PXRowDeletedEventArgs e)
        /// {
        ///     POVendorInventory vendor = e.Row as POVendorInventory;
        ///     object isdefault = cache.GetValueExt&lt;POVendorInventory.isDefault&gt;(e.Row);
        ///     if (isdefault is PXFieldState)
        ///     {
        ///         isdefault = ((PXFieldState)isdefault).Value;
        ///     }
        ///     if ((bool?)isdefault == true)
        ///         ...
        /// }
        /// </code>
        /// </example>
		public object GetValueExt<Field>(object data)
			where Field : IBqlField
		{
			return GetValueExt(data, typeof(Field).Name);
		}
        /// <exclude/>
		public object GetValueInt<Field>(object data)
			where Field : IBqlField
		{
			return GetValueInt(data, typeof(Field).Name);
		}
        /// <summary>
        /// Gets the <tt>PXFieldState</tt> object of the specified field
        /// in the given data record.
        /// </summary>
        /// <remarks>The method raises the <tt>FieldSelecting</tt> event.</remarks>
        /// <typeparam name="Field">The field whose
        /// <tt>PXFieldState</tt> object is created.</typeparam>
        /// <param name="data">The data record.</param>
        /// <returns>The field state object.</returns>
        /// <example>
        /// The code below shows a part of the <tt>SOOrderEntry</tt> graph
        /// constructor. The field state is created for the
        /// <tt>SOLine.InventoryID</tt> field.
        /// <code>
        /// public SOOrderEntry()
        /// {
        ///     ...
        ///     PXFieldState state = (PXFieldState)this.Transactions.Cache.GetStateExt&lt;SOLine.inventoryID&gt;(null);
        ///     viewInventoryID = state != null ? state.ViewName : null;
        ///     ...
        /// }
        /// </code>
        /// </example>
		public object GetStateExt<Field>(object data)
			where Field : IBqlField
		{
			return GetStateExt(data, typeof(Field).Name);
		}

        /// <summary>
        /// Specified value stored in internal dictionary associated with data row,  <br/>
        /// and can be retrived later by GetValuePending method.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
		public abstract void SetValuePending(object data, string fieldName, object value);

        /// <summary>
        /// returns value stored by SetValuePending
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
		public abstract object GetValuePending(object data, string fieldName);
		public object GetValuePending<Field>(object data)
			where Field : IBqlField
		{
			string fieldName = typeof(Field).Name;
			return GetValuePending(data, char.ToUpper(fieldName[0]).ToString() + fieldName.Substring(1));
		}
		public void SetValuePending<Field>(object data, object value)
			where Field : IBqlField
		{
			string fieldName = typeof(Field).Name;
			SetValuePending(data, char.ToUpper(fieldName[0]).ToString() + fieldName.Substring(1), value);
		}

		/// <summary>
		/// returns value from instance copy stored in database
		/// </summary>
		/// <param name="data"></param>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public abstract object GetValueOriginal(object data, string fieldName);
		public object GetValueOriginal<Field>(object data)
			where Field : IBqlField
		{
			string fieldName = typeof(Field).Name;
			return GetValueOriginal(data, char.ToUpper(fieldName[0]).ToString() + fieldName.Substring(1));
		}

		internal abstract object GetOriginal(object data);
		private static Dictionary<Type, Dictionary<Type, string[]>> _keys = new Dictionary<Type, Dictionary<Type, string[]>>();
		internal static string[] GetKeyNames(PXGraph graph, Type cacheType)
		{
			Dictionary<Type, string[]> dict;
			string[] ret;
			lock (((ICollection)_keys).SyncRoot)
			{
				if (!_keys.TryGetValue(cacheType, out dict))
				{
					_keys[cacheType] = dict = new Dictionary<Type, string[]>();
				}
				dict.TryGetValue(graph.GetType(), out ret);
			}
			if (ret == null)
			{
				PXCache cache = graph.Caches[cacheType];
				ret = cache.Keys.ToArray();
				lock (((ICollection)_keys).SyncRoot)
				{
					dict[graph.GetType()] = ret;
				}
			}
			return ret;
		}

		protected KeysCollection _Keys;
        /// <summary>
        /// Gets the collection of fied names that form the primary key
        /// of the data record. The collection is usually composed of
        /// the field that have attributes with the <tt>IsKey</tt> property
        /// set to <tt>true</tt>.
        /// </summary>
		public virtual KeysCollection Keys
		{
			get
			{
				if (_Keys == null)
				{
					_Keys = new KeysCollection();
				}
				return _Keys;
			}
		}

		internal protected string _Identity;

		/// <summary>
		/// When set to <c>true</c> makes the cache sort the accumulator records
		/// by keys before executing update/insert commands, which allows to
		/// avoid deadlocks in case of multithreaded update of the same accumulator table.
		/// </summary>
		public bool PreventDeadlock
		{
			get { return _PreventDeadlock; }
			set { _PreventDeadlock = value; }
		}

		internal protected bool _PreventDeadlock;

		/// <summary>
        /// Gets the name of the identity field if the DAC defines it.
		/// </summary>
		public virtual string Identity
		{
			get
			{
				return _Identity;
			}
		}

		internal protected bool _AggregateSelecting;
		internal protected bool _SingleTableSelecting;
		internal protected string _RowId;
		/// <summary>
		/// Gets the name of the Row ID (identity, guid or foreign identity) field if the DAC defines it.
		/// </summary>
		public virtual string RowId
		{
			get
			{
				return _RowId;
			}
		}

		internal protected int? _NoteIDOrdinal;
		internal protected string _NoteIDName;
		internal protected int _TranslationsCount;
		internal protected abstract void _AdjustStorage(int i, PXDataFieldParam assign);
		internal protected abstract void _AdjustStorage(string name, PXDataFieldParam assign);
		internal protected virtual bool _HasKeyValueStored()
		{
			return _KeyValueStoredNames != null;
		}
		internal protected virtual string _ReportGetFirstKeyValueStored(string fieldName)
		{
			return _KeyValueStoredNames != null && _KeyValueStoredNames.ContainsKey(fieldName) ? _FirstKeyValueStored.Value.Key : null;
		}

		protected List<string> _Immutables;
        /// <summary>
        /// Gets the list of names of the fields that are considered as immutable.
        /// Immutable fields are the fields that have attributes with the
        /// <tt>IsImmutable</tt> property set to <tt>true</tt>.
        /// </summary>
        public virtual List<string> Immutables
        {
            get
            {
                if (_Immutables == null)
                {
                    _Immutables = new List<string>();
                }
                return _Immutables;
            }
        }
		protected PXFieldCollection _Fields;

        /// <summary>
        /// Collection of Cache field names. <br/>
        /// By default, contains all DAC class public properties and virtual fields injected by attributes.<br/>
        /// Programmer can add custom fields to this collection.
        /// </summary>
		public virtual PXFieldCollection Fields
		{
			get
			{
				if (_Fields == null)
				{
					_Fields = new PXFieldCollection(new string[0], new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase));
				}
				return _Fields;
			}
		}

        /// <summary>
        /// allow to intercept the database operations(insert, update and delete row in database).
        /// </summary>
		public abstract PXDBInterceptorAttribute Interceptor
		{
			get;
			set;
		}

		protected List<Type> _BqlFields;

        /// <summary>
        /// List of classes that implements IBqlField nested in DAC class and ancestor types.<br/>
        /// This list is distinct of the list, that Fields property returns.
        /// </summary>
		public virtual List<Type> BqlFields
		{
			get
			{
				if (_BqlFields == null)
				{
					_BqlFields = new List<Type>();
				}
				return _BqlFields;
			}
		}

        /// <summary>
        /// search for bqlField in Fields collection 
        /// </summary>
        /// <param name="bqlField"></param>
        /// <returns></returns>
		public string GetField(Type bqlField)
		{
			string bqlFieldName = bqlField.Name;
			foreach (string field in Fields)
				if (string.Equals(field, bqlFieldName, StringComparison.OrdinalIgnoreCase)) return field;
			return null;
		}


        /// <summary>
        /// Searches for the specified field in the <see cref="PXCache{T}.BqlFields">BqlFields</see>
        /// collection.
        /// </summary>
        /// <param name="field">The field to find.</param>
        /// <returns>The abstract type implementing <tt>IBqlField</tt></returns>
		public virtual Type GetBqlField(string field)
		{
			foreach (Type bqlField in BqlFields)
				if (string.Equals(field, bqlField.Name, StringComparison.OrdinalIgnoreCase)) return bqlField;
			return null;
		}

        internal virtual Type GetBaseBqlField(string field)
        {
            return GetBqlField(field);
        }

		protected internal string GetFieldName(string name, bool needBql)
		{
			if (needBql)
			{
				for (int j = BqlFields.Count - 1; j >= 0; j--)
				{
					if (String.Compare(BqlFields[j].Name, name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return BqlFields[j].Name;
					}
				}
			}
			else
			{
				for (int j = 0; j < Fields.Count; j++)
				{
					if (String.Compare(Fields[j], name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return Fields[j];
					}
				}
			}
			return null;
		}

		protected List<Type> _BqlKeys;
		public virtual List<Type> BqlKeys
		{
			get
			{
				if (_BqlKeys == null)
				{
					_BqlKeys = new List<Type>();
				}
				return _BqlKeys;
			}
		}

        protected List<Type> _BqlImmutables;
        public virtual List<Type> BqlImmutables
        {
            get
            {
                if (_BqlImmutables == null)
                {
                    _BqlImmutables = new List<Type>();
                }
                return _BqlImmutables;
            }
        }
        /// <summary>
        /// Looks like obsolete property. Value of this property does not used.
        /// </summary>
		public abstract BqlCommand BqlSelect
		{
			get;
			set;
		}
		internal bool BypassCalced;
		internal bool SingleExtended;
		public abstract Type BqlTable
		{
			get;
		}

		public virtual Type GenericParameter { get { return null; } }

		public static Type GetBqlTable(Type dac)
		{
			var result = dac;
			while (result != null && 
				typeof(IBqlTable).IsAssignableFrom(result.BaseType) && 
				!result.IsDefined(typeof(PXTableAttribute), false) &&
				!result.IsDefined(typeof(PXTableNameAttribute), false))
			{
				result = result.BaseType;
			}
			return result;
		}

        /// <summary>
        /// Save row field values to Dictionary 
        /// </summary>
        /// <param name="data">IBqlTable</param>
        /// <returns></returns>
		public abstract Dictionary<string, object> ToDictionary(object data);

		public abstract string ToXml(object data);

		public abstract object FromXml(string xml);

		public abstract string ValueToString(string fieldName, object val);

		public abstract object ValueFromString(string fieldName, string val);

		//protected abstract Dictionary<string, string> FieldAliases
		//{
		//    get;
		//}

		public abstract int GetFieldCount();
		public abstract int GetFieldOrdinal(string field);
		public abstract int GetFieldOrdinal<Field>()
			where Field : IBqlField;

    	internal abstract Type GetFieldType(string fieldName);

        /// <summary>
        /// Repair internal hashtable, if user has changed the key values of stored data rows.
        /// </summary>
		public abstract void Normalize();
		#endregion

		#region External References
		protected PXGraph _Graph;
		public virtual PXGraph Graph
		{
			get
			{
				return _Graph;
			}
			set
			{
				_Graph = value;
			}
		}
		#endregion

		#region Access Rights
		protected internal object _CacheSecurity;

		protected bool _SelectRights = true;
		protected internal virtual bool SelectRights
		{
			get
			{
				return _SelectRights;
			}
			set
			{
				_SelectRights = value;
				if (!value)
				{
					_AllowSelect = false;
				}
			}
		}
		protected internal bool _AllowSelect = true;
		protected internal bool _AllowSelectChanged;
		internal bool AutomationHidden;
		internal bool AutomationInsertDisabled;
		internal bool AutomationUpdateDisabled;
		internal bool AutomationDeleteDisabled;
        /// <summary>
        /// Gets or sets the value that indicates whether the cache
        /// allows selection of data records from the user interface.
        /// </summary>
		public virtual bool AllowSelect
		{
			get
			{
				if (!AutomationHidden)
				{
					return _AllowSelect;
				}
				return false;
			}
			set
			{
				if (_SelectRights)
				{
					if (_AllowSelect != value)
					{
						_AllowSelectChanged = true;
					}
					_AllowSelect = value;
				}
			}
		}
		protected bool _UpdateRights = true;
		protected internal virtual bool UpdateRights
		{
			get
			{
				return _UpdateRights;
			}
			set
			{
				_UpdateRights = value;
				if (!value)
				{
					_AllowUpdate = false;
				}
			}
		}
		protected internal bool _AllowUpdate = true;

        /// <summary>
        /// Gets or sets the value that indicates whether the cache allows
        /// update of data records from the user interface. This value does
        /// not affect the ability to update a data record via the methods.
        /// By default, the property equals true.
        /// </summary>
		public virtual bool AllowUpdate
		{
			get
			{
				if (!AutomationUpdateDisabled)
				{
					return _AllowUpdate;
				}
				return false;
			}
			set
			{
				if (_UpdateRights)
				{
					_AllowUpdate = value;
				}
			}
		}
		protected bool _InsertRights = true;
		protected internal virtual bool InsertRights
		{
			get
			{
				return _InsertRights;
			}
			set
			{
				_InsertRights = value;
				if (!value)
				{
					_AllowInsert = false;
				}
			}
		}
		protected internal bool _AllowInsert = true;

        /// <summary>
        /// Gets or sets the value that indicates whether the cache allows insertion
        /// of data records from the user interface. This value does not affect the
        /// ability to insert a data record via the methods. By default, the property
        /// equals <tt>true</tt>.
        /// </summary>
        public virtual bool AllowInsert
		{
			get
			{
				if (!AutomationInsertDisabled)
				{
					return _AllowInsert;
				}
				return false;
			}
			set
			{
				if (_InsertRights)
				{
					_AllowInsert = value;
				}
			}
		}
		protected bool _DeleteRights = true;
		protected internal virtual bool DeleteRights
		{
			get
			{
				return _DeleteRights;
			}
			set
			{
				_DeleteRights = value;
				if (!value)
				{
					_AllowDelete = false;
				}
			}
		}
		protected internal bool _AllowDelete = true;
		
        /// <summary>
        /// Gets or sets the value that indicates whether the cache allows
        /// deletion of data records from the user interface. This value does
        /// not affect the ability to delete a data record via the methods.
        /// By default, the property equals <tt>true</tt>.
        /// </summary>
        public virtual bool AllowDelete
		{
			get
			{
				if (!AutomationDeleteDisabled)
				{
					return _AllowDelete;
				}
				return false;
			}
			set
			{
				if (_DeleteRights)
				{
					_AllowDelete = value;
				}
			}
		}
		#endregion

		#region Commands
		internal abstract object _Clone(object item);
		//public abstract string TableName
		//{
		//    get;
		//    set;
		//}
		internal bool _ReadonlyCreatedCache;
		protected bool _AutoSave;
		public virtual bool AutoSave
		{
			get
			{
				return _AutoSave;
			}
			set
			{
				_AutoSave = value;
			}
		}
		#endregion

		#region Item manipulation methods

		public static List<Type> getBqlTableAndParents(Type bqlTable)
		{
			List<Type> tables = new List<Type>();
			while (typeof(IBqlTable).IsAssignableFrom(bqlTable))
			{
				tables.Add(bqlTable);
				bqlTable = bqlTable.BaseType;
			}
			return tables;
		}

		public static ExtraFieldsDefinition _GetAttributeDefinition(Type tnode)
		{
			var noteIdProp = tnode.GetProperty("NoteID", BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
			if (noteIdProp == null) return null;

			// learn if they have attributes
			ExtraFieldsDefinition def = ExtraFieldsDefinition.getForType(tnode);
			return def == null || def.AttributeIdByIndex == null || def.AttributeIdByIndex.Length == 0 ? null : def;
		}

		public abstract Type[] GetExtensionTypes();
		static readonly Type[] _AvailableExtensions = FindExtensionTypes();

		static Type[] _DynamicExtensions;
		static bool _initDynamicExtensionsWatcher;

		static IEnumerable<Type> GetAllExtensions()
		{
			if (WebConfig.DisableExtensions)
				return new Type[0];

			if (_DynamicExtensions == null)
			{
				_DynamicExtensions = PXCodeDirectoryCompiler.GetCompiledTypes<PXCacheExtension>().ToArray();
			}

			if (!_initDynamicExtensionsWatcher)
			{
				_initDynamicExtensionsWatcher = true;
				PXCodeDirectoryCompiler.NotifyOnChange(ClearCaches);
				PXCacheExtensionCollection.ClearCacheExtensionsDelegate = ClearCaches;
			}


			return _AvailableExtensions.Concat(_DynamicExtensions);


		}

	    static void ClearCaches()
		{
			_DynamicExtensions = null;
			PXDatabase.ResetSlot<Dictionary<Type, CacheStaticInfoBase>>("CacheStaticInfo");
			lock (PXExtensionManager._StaticInfoLock)
			{
				PXExtensionManager._CacheStaticInfo.Clear();
				
			}
		}

		internal static List<Type> _GetExtensions(Type tnode, bool checkActive)
		{
			List<Type> tables = getBqlTableAndParents(tnode);
			List<Type> ret = new List<Type>();
			for (int i = tables.Count - 1; i >= 0; i--)
			{
				tnode = tables[i];
				List<Type> ext = new List<Type>();
				foreach (Type t in GetAllExtensions())
				{
					// processing extension objects
					if (checkActive)
					{
						try
						{
							System.Reflection.MethodInfo info = t.GetMethod("IsActive", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase, null, new Type[0], null);
							object res;
							if (info != null && info.ReturnType == typeof(bool) && (res = info.Invoke(null, new object[0])) is bool && !(bool)res)
							{
								continue;
							}
						}
						catch
						{
						}
					}
					Type[] args;
					if (typeof(PXCacheExtension).IsAssignableFrom(t) && t.BaseType.IsGenericType
						&& (args = t.BaseType.GetGenericArguments()).Length > 0 && (args[args.Length - 1] == tnode))
					{
						ext.Add(t);
					}
				}
				ret.AddRange(PXExtensionManager.Sort(ext));
			}
			//ExtraFieldsDefinition definition = _GetAttributeDefinition(tnode);
			//if (definition != null) {
			//	Type[] genericParams = definition.GetExtensionGenerics(tnode);
			//	Type cacheExt = ExtraFieldsCacheExtensionHelper.allExtensions[genericParams.Length-1];
			//	ret.Insert(0, cacheExt.MakeGenericType(genericParams));
			//}

			return ret;
		}

		/// <summary>
		/// Gets an extension of appropriate type
		/// </summary>
		/// <typeparam name="Extension">
		/// The type of extension requested
		/// </typeparam>
		/// <param name="item">
		/// Parent standard object
		/// </param>
		/// <returns>
		/// Object of type Extension
		/// </returns>
		public abstract Extension GetExtension<Extension>(object item)
			where Extension : PXCacheExtension;

	    public static readonly object NotSetValue = new object();
		public static readonly string IsNewRow = Guid.NewGuid().ToString();

		/// <summary>
		/// Gets the type of data rows in the cache.
		/// </summary>
		/// <returns>Containing type.</returns>
		public abstract Type GetItemType();

		internal object _Current;
        /// <summary>
        /// Contains value of type IBqlTable.<br/>
        /// Can be used in PXSelect query<br/>
        /// This property may point to last row displayed in the user interface<br/>
        /// If user selects row in PXGrid, this property may point to this row on callback.<br/>
        /// if user inserts, updates or deletes row, Current points to such row.<br/>
        /// API methods Insert and Update also assigned this property.<br/>
        /// When this property assigned, OnRowSelected event is raised. 
        /// 
        /// </summary>
		public abstract object Current
		{
			get;
			set;
		}

		/// <summary>
		/// Allow to pass active row from business logic back to UI
		/// Current can not be used for this because its value can be unpredictable changed
		/// </summary>
	    public IBqlTable ActiveRow;

		public abstract object InternalCurrent
		{
			get;
		}
		/// <summary>
        /// Compares values of key fields. List of key fields is taken from the cache
        /// </summary>
        /// <param name="a">IBqlTable or IDictionary</param>
        /// <param name="b">IBqlTable or IDictionary</param>
        /// <returns></returns>
		public abstract bool ObjectsEqual(object a, object b);

        /// <summary>
        /// Compares values of specified fields.
        /// </summary>
		public bool ObjectsEqual<Field1>(object a, object b)
			where Field1 : IBqlField
		{
			return object.Equals(GetValue<Field1>(a), GetValue<Field1>(b));
		}
        /// <summary>
        /// Compares values of specified fields.
        /// </summary>
		public bool ObjectsEqual<Field1, Field2>(object a, object b)
			where Field1 : IBqlField
			where Field2 : IBqlField
		{
			return ObjectsEqual<Field1>(a, b) && ObjectsEqual<Field2>(a, b);
		}

        /// <summary>
        /// Compares values of specified fields.
        /// </summary>
		public bool ObjectsEqual<Field1, Field2, Field3>(object a, object b)
			where Field1 : IBqlField
			where Field2 : IBqlField
			where Field3 : IBqlField
		{
			return ObjectsEqual<Field1>(a, b) && ObjectsEqual<Field2, Field3>(a, b);
		}
        /// <summary>
        /// Compares values of specified fields.
        /// </summary>
		public bool ObjectsEqual<Field1, Field2, Field3, Field4>(object a, object b)
			where Field1 : IBqlField
			where Field2 : IBqlField
			where Field3 : IBqlField
			where Field4 : IBqlField
		{
			return ObjectsEqual<Field1>(a, b) && ObjectsEqual<Field2, Field3, Field4>(a, b);
		}
        /// <summary>
        /// Compares values of specified fields.
        /// </summary>
		public bool ObjectsEqual<Field1, Field2, Field3, Field4, Field5>(object a, object b)
			where Field1 : IBqlField
			where Field2 : IBqlField
			where Field3 : IBqlField
			where Field4 : IBqlField
			where Field5 : IBqlField
		{
			return ObjectsEqual<Field1>(a, b) && ObjectsEqual<Field2, Field3, Field4, Field5>(a, b);
		}
        /// <summary>
        /// Compares values of specified fields.
        /// </summary>
		public bool ObjectsEqual<Field1, Field2, Field3, Field4, Field5, Field6>(object a, object b)
			where Field1 : IBqlField
			where Field2 : IBqlField
			where Field3 : IBqlField
			where Field4 : IBqlField
			where Field5 : IBqlField
			where Field6 : IBqlField
		{
			return ObjectsEqual<Field1>(a, b) && ObjectsEqual<Field2, Field3, Field4, Field5, Field6>(a, b);
		}
        /// <summary>
        /// Compares values of specified fields.
        /// </summary>
		public bool ObjectsEqual<Field1, Field2, Field3, Field4, Field5, Field6, Field7>(object a, object b)
			where Field1 : IBqlField
			where Field2 : IBqlField
			where Field3 : IBqlField
			where Field4 : IBqlField
			where Field5 : IBqlField
			where Field6 : IBqlField
			where Field7 : IBqlField
		{
			return ObjectsEqual<Field1>(a, b) && ObjectsEqual<Field2, Field3, Field4, Field5, Field6, Field7>(a, b);
		}
        /// <summary>
        /// Compares values of specified fields.
        /// </summary>
		public bool ObjectsEqual<Field1, Field2, Field3, Field4, Field5, Field6, Field7, Field8>(object a, object b)
			where Field1 : IBqlField
			where Field2 : IBqlField
			where Field3 : IBqlField
			where Field4 : IBqlField
			where Field5 : IBqlField
			where Field6 : IBqlField
			where Field7 : IBqlField
			where Field8 : IBqlField
		{
			return ObjectsEqual<Field1>(a, b) && ObjectsEqual<Field2, Field3, Field4, Field5, Field6, Field7, Field8>(a, b);
		}
        /// <summary>
        /// Returns hash code generated from key field values.
        /// </summary>
        /// <param name="data">IBqlTable or IDictionary</param>
        /// <returns></returns>
		public abstract int GetObjectHashCode(object data);

        /// <summary>
        /// Displays key fields in format {k1=v1, k2=v2}
        /// </summary>
        /// <param name="data">IBqlTable or PXResult</param>
        /// <returns></returns>
		public abstract string ObjectToString(object data);

        /// <summary>
        /// Looks for an object in the cache, returns its status.
        /// Item located by key fields. 
        /// </summary>
        /// <param name="item">Cache item to test, IBqlTable</param>
        /// <returns></returns>
		public abstract PXEntryStatus GetStatus(object item);

        /// <summary>
        /// Looks for item in the cache, sets status. 
        /// If item not not in the cache, it will be inserted.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="status"></param>
		public abstract void SetStatus(object item, PXEntryStatus status);


        /// <summary>
        /// Searches data row in the cache.<br/>
        /// Returns located row.
        /// </summary>
        /// <param name="item">IBqlRow</param>
        /// <returns></returns>
		public abstract object Locate(object item);

		protected internal abstract bool IsPresent(object item);

		protected internal abstract bool IsGraphSpecificField(string fieldName);

        /// <summary>
        /// Searches data row in the cache. <br/>
        /// If row not found in cache, <br/>
        /// reads it from the database and places into the cache with status NotChanged.<br/>
        /// </summary>
		public abstract int Locate(IDictionary keys);


        /// <summary>
        /// Remove entry from cache items hashtable.
        /// </summary>
        /// <param name="item"></param>
        public abstract void Remove(object item);

		/// <summary>
        /// Places row into the cache with status Updated.<br/>
        /// If row does not exists in the cache, looks for it in database.<br/>
        /// If row does not exists in database, inserts row with status Inserted.<br/>
        /// Raise events OnRowUpdating, OnRowUpdated and other events.<br/>
        /// This method is used to update row from user interface.<br/>
        /// Flag AllowUpdate may cancel this method.<br/>
        /// returns 1 if updated successfully, otherwise 0.
		/// </summary>
		/// <param name="keys">Primary key of the item.</param>
		/// <param name="values">New field values to update the item with.</param>
		/// <returns>1 if updated successfully, otherwise 0.</returns>
		public abstract int Update(IDictionary keys, IDictionary values);


        /// <summary>
        /// Places row into the cache with status Updated.<br/>
        /// If row does not exists in the cache, looks for it in database.<br/>
        /// If row does not exists in database, inserts row with status Inserted.<br/>
        /// Raise events OnRowUpdating, OnRowUpdated and other events.
        /// Flag AllowUpdate does not affects this method.
        /// </summary>
        /// <param name="item">IBqlTable of type [CacheItemType]</param>
        /// <returns></returns>
		public abstract object Update(object item);

        /// <summary>
        /// Place row into the cache with status Updated.<br/>
        /// If row does not exists in the cache, looks for it in database.<br/>
        /// If row does not exists in database, inserts row with status Inserted.<br/>
        /// Raise events OnRowUpdating, OnRowUpdated and other events.
        /// </summary>
        /// <param name="item">IBqlTable of type [CacheItemType]</param>
        /// <param name="bypassinterceptor">whether to ignore PXDBInterceptorAttribute</param>
        protected internal abstract object Update(object item, bool bypassinterceptor);

		/// <summary>
        /// Updates Last Modified state of the row into the cache. <br />
        /// Called at the end of RowUpdated, RowInserted event. <br />
        /// Last modified is used to track properties changes on RowUpdate. <br />
        /// Has influence on Accumulator Attributes <br />
        /// </summary>
        /// <param name="item">Row updated/inserted</param>
        /// <param name="inserted">Flag insert/update</param>
        protected internal abstract void UpdateLastModified(object item, bool inserted);

        /// <summary>
        /// Inserts  new row into the cache. <br/>
        /// Raises events OnRowInserting, OnRowInserted and other field related events.<br/>
        /// Does not check the database for existing row.<br/>
        /// Values of dictionary are not readonly and can be updated during method call.<br/>
        /// Flag AllowInsert may cancel this method.<br/>
        /// Returns 1 if inserted successfully, otherwise 0.<br/>
		/// </summary>
		/// <param name="values">Field values to populate the item before inserting.</param>
		/// <returns>1 if inserted successfully, otherwise 0.</returns>
		public abstract int Insert(IDictionary values);
		internal abstract object FillItem(IDictionary values);


        /// <summary>
        /// Inserts  new row into the cache. <br/>
        /// Returns inserted row of type [CacheItemType] or null if row was not inserted.<br/>
        /// Raises events OnRowInserting, OnRowInserted and other field related events.<br/>
        /// Does not check the database for existing row.
        /// Flag AllowInsert does not affects this method.
        /// </summary>
        /// <param name="item">IBqlTable of type [CacheItemType]</param>
		public abstract object Insert(object item);

        /// <summary>
        /// Inserts new row into the cache. <br/>
        /// Returns inserted row of type [CacheItemType] or null if row was not inserted.<br/>
        /// Raises events OnRowInserting, OnRowInserted and other field related events.<br/>
        /// Does not check the database for existing row.<br/>
        /// </summary>
        /// <param name="item">IBqlTable of type [CacheItemType]</param>
        /// <param name="bypassinterceptor">whether to ignore PXDBInterceptorAttribute</param>
        /// <returns></returns>
        protected internal abstract object Insert(object item, bool bypassinterceptor);

        /// <summary>
        /// Place new row into the cache.
        /// </summary>
        /// <returns></returns>
        public abstract object Insert();

        /// <summary>
        /// return new TNode();
        /// </summary>
		public abstract object CreateInstance();

		internal abstract object ToChildEntity<Parent>(Parent item)
			where Parent : class, IBqlTable, new();

        /// <summary>
        /// Inserts row into the cache, when row type is distinct from cache item type.<br/>
        /// Returns inserted row.
        /// </summary>
        /// <typeparam name="Parent"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
		public abstract object Extend<Parent>(Parent item)
			where Parent : class, IBqlTable, new();

        /// <summary>
        /// Place item into the cache with status Deleted.
        /// Raise events OnRowDeleting, OnRowDeleted
        /// This method is used to delete row from user interface.<br/>
        /// Flag AllowDelete may cancel this method.<br/>
        /// returns 1 if deleted successfully, otherwise 0.
        /// </summary>
        /// <param name="keys">Primary key of the item.</param>
        /// <param name="values">Not used, can be null</param>
        /// <returns>1 if deleted successfully, otherwise 0.</returns>
        public abstract int Delete(IDictionary keys, IDictionary values);

        /// <summary>
        /// Place item into the cache with status Deleted.
        /// Raise events OnRowDeleting, OnRowDeleted
        /// Flag AllowDelete does not affects this method.
        /// </summary>
		public abstract object Delete(object item);


        protected internal abstract object Delete(object item, bool bypassinterceptor);
		/// <summary>
		/// Looks for row in cache collection. If row has status deleted or insertedDeleted - returns null. 
		/// returns row inserted to cache.
		/// </summary>
		internal protected abstract void PlaceNotChanged(object data);

		/// <summary>
		/// Looks for row in cache collection. If row has status deleted or insertedDeleted - returns null.
		/// If status inserted or updated - wasUpdated flag is true. 
		/// returns row inserted to cache.
		/// </summary>
		internal protected abstract object PlaceNotChanged(object data, out bool wasUpdated);
		
		/// <summary>
        /// Inserts item created from the PXDataRecord into the cache with status NotChanged.
        /// Raise event OnRowSelecting
        /// </summary>
		public abstract object Select(PXDataRecord record, ref int position, bool isReadOnly, out bool wasUpdated);
		
        /// <summary>
        /// Clone field values.
        /// </summary>
        /// <param name="item">IBqlTable</param>
        /// <returns></returns>
        public abstract object CreateCopy(object item);

        /// <summary>
        /// Copy field values from copy to item.
        /// </summary>
        /// <param name="item">IBqlTable</param>
        /// <param name="copy">IBqlTable</param>
		public abstract void RestoreCopy(object item, object copy);
		#endregion

		#region Session state managment methods
		/// <summary>
		/// Loads data rows and other cache state objects from the session.
		/// </summary>
		public abstract void Load();

		/// <summary>
        /// Saves dirty data rows and other cache state objects into the session.
		/// </summary>
		public abstract void Unload();

		/// <summary>
		/// Clear all the info stored in the session previously.
		/// </summary>
		public abstract void Clear();
		public abstract void ClearItemAttributes();

		/// <summary>
		/// Clear all cached query result for a given table.
		/// </summary>
		public abstract void ClearQueryCache();
		#endregion

		#region Dirty items enumerators
        /// <summary>
        /// Dirty items.
        /// </summary>
        public abstract IEnumerable Dirty
        {
            get;
        }

		/// <summary>
		/// Deleted items.
		/// </summary>
		public abstract IEnumerable Deleted
		{
			get;
		}

		/// <summary>
		/// Updated items.
		/// </summary>
		public abstract IEnumerable Updated
		{
			get;
		}

		/// <summary>
		/// Inserted items.
		/// </summary>
		public abstract IEnumerable Inserted
		{
			get;
		}
		/// <summary>
		/// All cached items.
		/// </summary>
		public abstract IEnumerable Cached
		{
			get;
		}
		internal abstract int Version
		{
			get;
		}
		internal PXCacheOriginalCollection _Originals;
		internal Tuple<string, int?, int?> _GetOriginalCounts(object item)
		{
			lock (((ICollection)_Originals).SyncRoot)
			{
				BqlTablePair orig;
				if (item is IBqlTable && _Originals.TryGetValue((IBqlTable)item, out orig))
				{
					return new Tuple<string, int?, int?>(orig.NoteText, orig.FilesCount, orig.ActivitiesCount);
				}
				return new Tuple<string, int?, int?>(null, null, null);
			}
		}
		internal void _SetTranslations(object item, int idx, string[] translations)
		{
			if (item is IBqlTable)
			{
				lock (((ICollection)_Originals).SyncRoot)
				{
					BqlTablePair orig;
					if (!_Originals.TryGetValue((IBqlTable)item, out orig))
					{
						_Originals[(IBqlTable)item] = orig = new BqlTablePair();
					}
					if (orig.Translations == null)
					{
						orig.Translations = new List<string[]>(_TranslationsCount);
					}
					for (int i = orig.Translations.Count; i <= idx; i++)
					{
						orig.Translations.Add(null);
					}
					orig.Translations[idx] = translations;
				}
			}
		}
		internal string[] _GetTranslations(object item, int idx)
		{
			if (item is IBqlTable)
			{
				lock (((ICollection)_Originals).SyncRoot)
				{
					BqlTablePair orig;
					if (_Originals.TryGetValue((IBqlTable)item, out orig))
					{
						if (orig.Translations != null && idx < orig.Translations.Count)
						{
							return orig.Translations[idx];
						}
					}
				}
			}
			return null;
		}
		internal void _SetOriginalCounts(object item, string noteText, int? filesCount, int? activitiesCount)
		{
			if (item is IBqlTable)
			{
				lock (((ICollection)_Originals).SyncRoot)
				{
					BqlTablePair orig;
					if (!_Originals.TryGetValue((IBqlTable)item, out orig))
					{
						_Originals[(IBqlTable)item] = orig = new BqlTablePair();
					}
					if (noteText != null)
					{
						orig.NoteText = noteText;
					}
					if (filesCount != null)
					{
						orig.FilesCount = filesCount;
					}
					if (activitiesCount != null)
					{
						orig.ActivitiesCount = activitiesCount;
					}
				}
			}
		}
		internal void _ResetOriginalCounts(object item, bool resetText, bool resetFiles, bool resetActivities)
		{
			if (item is IBqlTable)
			{
				lock (((ICollection)_Originals).SyncRoot)
				{
					BqlTablePair orig;
					if (!_Originals.TryGetValue((IBqlTable)item, out orig))
					{
						_Originals[(IBqlTable)item] = orig = new BqlTablePair();
					}
					if (resetText)
					{
						orig.NoteText = null;
					}
					if (resetFiles)
					{
						orig.FilesCount = null;
					}
					if (resetActivities)
					{
						orig.ActivitiesCount = null;
					}
				}
			}
		}
		internal PXCacheOriginalCollection _OriginalsRemoved;
		#endregion

		#region Persistance to the database
		/// <summary>
        /// Saves changed data rows to the database.<br/>
        /// Raise events OnRowPersisting, OnCommandPreparing, OnRowPersisted, OnExceptionHandling
		/// </summary>
		/// <returns>The first item saved.</returns>
		public abstract int Persist(PXDBOperation operation);


        /// <summary>
        /// executes PersistUpdated or PersistInserted or PersistDeleted depends on operation
        /// </summary>
        /// <param name="row"></param>
        /// <param name="operation"></param>
		public abstract void Persist(object row, PXDBOperation operation);


        /// <summary>
        /// Saves updated data row to the database.<br/>
        /// Raise events OnRowPersisting, OnCommandPreparing, OnRowPersisted, OnExceptionHandling<br/>
        /// Default behavior can be modified by PXDBInterceptorAttribute
        /// </summary>
		public abstract bool PersistUpdated(object row);
       
        
        /// <summary>
        /// Inserts data row into the database.<br/>
        /// Exception is trown if row with such keys exists in database. <br/>
        /// Raise events OnRowPersisting, OnCommandPreparing, OnRowPersisted, OnExceptionHandling<br/>
        /// Default behavior can be modified by PXDBInterceptorAttribute
        /// </summary>
		public abstract bool PersistInserted(object row);


        /// <summary>
        /// Deletes row from the database by the keys.<br/>
        /// Raise events OnRowPersisting, OnCommandPreparing, OnRowPersisted, OnExceptionHandling<br/>
        /// Default behavior can be modified by PXDBInterceptorAttribute
        /// </summary>
		public abstract bool PersistDeleted(object row);

        /// <summary>
        /// For each persisted row - raise OnRowPersisted, SetStatus(Notchanged)
        /// </summary>
		public abstract void Persisted(bool isAborted);

        /// <summary>
        /// For each persisted row - raise OnRowPersisted, SetStatus(Notchanged)
        /// </summary>
		protected internal abstract void Persisted(bool isAborted, Exception exception);

        /// <summary>
        /// remove row from list of persited items
        /// </summary>
        /// <param name="row"></param>
		public abstract void ResetPersisted(object row);


		protected bool _IsDirty;

		/// <summary>
		/// If cache contains data rows to be saved to database.
		/// </summary>		
		public virtual bool IsDirty
		{
			get
			{
				if (!AutoSave)
				{
					return _IsDirty;
				}
				return false;
			}
			set
			{
				_IsDirty = value;
			}
		}

		/// <summary>
		/// If cache contains data rows to be saved to database.
		/// </summary>		
        public abstract bool IsInsertedUpdatedDeleted
        {
            get;
        }
		#endregion

		#region Events
		internal protected sealed class EventsRow
		{
			internal List<PXRowSelecting> _RowSelectingList = new List<PXRowSelecting>();
			private PXRowSelecting _RowSelectingDelegate;
			public PXRowSelecting RowSelecting
			{
				get
				{
					if (_RowSelectingList != null && _RowSelectingList.Count > 0)
					{
						_RowSelectingDelegate = (PXRowSelecting)Delegate.Combine(_RowSelectingList.ToArray());
						_RowSelectingList = null;
					}
					return _RowSelectingDelegate;
				}
				set
				{
					_RowSelectingDelegate = value;
				}
			}
			internal List<PXRowSelected> _RowSelectedList = new List<PXRowSelected>();
			private PXRowSelected _RowSelectedDelegate;
			public PXRowSelected RowSelected
			{
				get
				{
					if (_RowSelectedList != null && _RowSelectedList.Count > 0)
					{
						_RowSelectedDelegate = (PXRowSelected)Delegate.Combine(_RowSelectedList.ToArray());
						_RowSelectedList = null;
					}
					return _RowSelectedDelegate;
				}
				set
				{
					_RowSelectedDelegate = value;
				}
			}
			internal List<PXRowInserting> _RowInsertingList = new List<PXRowInserting>();
			private PXRowInserting _RowInsertingDelegate;
			public PXRowInserting RowInserting
			{
				get
				{
					if (_RowInsertingList != null && _RowInsertingList.Count > 0)
					{
						_RowInsertingDelegate = (PXRowInserting)Delegate.Combine(_RowInsertingList.ToArray());
						_RowInsertingList = null;
					}
					return _RowInsertingDelegate;
				}
				set
				{
					_RowInsertingDelegate = value;
				}
			}
			internal List<PXRowInserted> _RowInsertedList = new List<PXRowInserted>();
			private PXRowInserted _RowInsertedDelegate;
			public PXRowInserted RowInserted
			{
				get
				{
					if (_RowInsertedList != null && _RowInsertedList.Count > 0)
					{
						_RowInsertedDelegate = (PXRowInserted)Delegate.Combine(_RowInsertedList.ToArray());
						_RowInsertedList = null;
					}
					return _RowInsertedDelegate;
				}
				set
				{
					_RowInsertedDelegate = value;
				}
			}
			internal List<PXRowUpdating> _RowUpdatingList = new List<PXRowUpdating>();
			private PXRowUpdating _RowUpdatingDelegate;
			public PXRowUpdating RowUpdating
			{
				get
				{
					if (_RowUpdatingList != null && _RowUpdatingList.Count > 0)
					{
						_RowUpdatingDelegate = (PXRowUpdating)Delegate.Combine(_RowUpdatingList.ToArray());
						_RowUpdatingList = null;
					}
					return _RowUpdatingDelegate;
				}
				set
				{
					_RowUpdatingDelegate = value;
				}
			}
			internal List<PXRowUpdated> _RowUpdatedList = new List<PXRowUpdated>();
			private PXRowUpdated _RowUpdatedDelegate;
			public PXRowUpdated RowUpdated
			{
				get
				{
					if (_RowUpdatedList != null && _RowUpdatedList.Count > 0)
					{
						_RowUpdatedDelegate = (PXRowUpdated)Delegate.Combine(_RowUpdatedList.ToArray());
						_RowUpdatedList = null;
					}
					return _RowUpdatedDelegate;
				}
				set
				{
					_RowUpdatedDelegate = value;
				}
			}
			internal List<PXRowDeleting> _RowDeletingList = new List<PXRowDeleting>();
			private PXRowDeleting _RowDeletingDelegate;
			public PXRowDeleting RowDeleting
			{
				get
				{
					if (_RowDeletingList != null && _RowDeletingList.Count > 0)
					{
						_RowDeletingDelegate = (PXRowDeleting)Delegate.Combine(_RowDeletingList.ToArray());
						_RowDeletingList = null;
					}
					return _RowDeletingDelegate;
				}
				set
				{
					_RowDeletingDelegate = value;
				}
			}
			internal List<PXRowDeleted> _RowDeletedList = new List<PXRowDeleted>();
			private PXRowDeleted _RowDeletedDelegate;
			public PXRowDeleted RowDeleted
			{
				get
				{
					if (_RowDeletedList != null && _RowDeletedList.Count > 0)
					{
						_RowDeletedDelegate = (PXRowDeleted)Delegate.Combine(_RowDeletedList.ToArray());
						_RowDeletedList = null;
					}
					return _RowDeletedDelegate;
				}
				set
				{
					_RowDeletedDelegate = value;
				}
			}
			internal List<PXRowPersisting> _RowPersistingList = new List<PXRowPersisting>();
			private PXRowPersisting _RowPersistingDelegate;
			public PXRowPersisting RowPersisting
			{
				get
				{
					if (_RowPersistingList != null && _RowPersistingList.Count > 0)
					{
						_RowPersistingDelegate = (PXRowPersisting)Delegate.Combine(_RowPersistingList.ToArray());
						_RowPersistingList = null;
					}
					return _RowPersistingDelegate;
				}
				set
				{
					_RowPersistingDelegate = value;
				}
			}
			internal List<PXRowPersisted> _RowPersistedList = new List<PXRowPersisted>();
			private PXRowPersisted _RowPersistedDelegate;
			public PXRowPersisted RowPersisted
			{
				get
				{
					if (_RowPersistedList != null && _RowPersistedList.Count > 0)
					{
						_RowPersistedDelegate = (PXRowPersisted)Delegate.Combine(_RowPersistedList.ToArray());
						_RowPersistedList = null;
					}
					return _RowPersistedDelegate;
				}
				set
				{
					_RowPersistedDelegate = value;
				}
			}
		}
		//internal protected sealed class EventsField
		//{
		//    public PXCommandPreparing CommandPreparing;
		//    public PXFieldDefaulting FieldDefaulting;
		//    public PXFieldUpdating FieldUpdating;
		//    public PXFieldVerifying FieldVerifying;
		//    public PXFieldUpdated FieldUpdated;
		//    public PXFieldSelecting FieldSelecting;
		//    public PXExceptionHandling ExceptionHandling;
		//}
		protected sealed class EventsRowAttr
		{
			public IPXRowSelectingSubscriber[] RowSelecting;
			public IPXRowSelectedSubscriber[] RowSelected;
			public IPXRowInsertingSubscriber[] RowInserting;
			public IPXRowInsertedSubscriber[] RowInserted;
			public IPXRowUpdatingSubscriber[] RowUpdating;
			public IPXRowUpdatedSubscriber[] RowUpdated;
			public IPXRowDeletingSubscriber[] RowDeleting;
			public IPXRowDeletedSubscriber[] RowDeleted;
			public IPXRowPersistingSubscriber[] RowPersisting;
			public IPXRowPersistedSubscriber[] RowPersisted;
		}
		//protected sealed class EventsFieldAttr
		//{
		//    public IPXCommandPreparingSubscriber[] CommandPreparing;
		//    public IPXFieldDefaultingSubscriber[] FieldDefaulting;
		//    public IPXFieldUpdatingSubscriber[] FieldUpdating;
		//    public IPXFieldVerifyingSubscriber[] FieldVerifying;
		//    public IPXFieldUpdatedSubscriber[] FieldUpdated;
		//    public IPXFieldSelectingSubscriber[] FieldSelecting;
		//    public IPXExceptionHandlingSubscriber[] ExceptionHandling;
		//}
		
		protected internal sealed class EventsFieldMap
		{
			public List<IPXCommandPreparingSubscriber> CommandPreparing = new List<IPXCommandPreparingSubscriber>();
			public List<IPXFieldDefaultingSubscriber> FieldDefaulting = new List<IPXFieldDefaultingSubscriber>();
			public List<IPXFieldUpdatingSubscriber> FieldUpdating = new List<IPXFieldUpdatingSubscriber>();
			public List<IPXFieldVerifyingSubscriber> FieldVerifying = new List<IPXFieldVerifyingSubscriber>();
			public List<IPXFieldUpdatedSubscriber> FieldUpdated = new List<IPXFieldUpdatedSubscriber>();
			public List<IPXFieldSelectingSubscriber> FieldSelecting = new List<IPXFieldSelectingSubscriber>();
			public List<IPXExceptionHandlingSubscriber> ExceptionHandling = new List<IPXExceptionHandlingSubscriber>();
		}
		internal class EventPosition
		{
			public int RowSelectingFirst = -1;
			public int RowSelectingLength;
			public int RowSelectedFirst = -1;
			public int RowSelectedLength;
			public int RowInsertingFirst = -1;
			public int RowInsertingLength;
			public int RowInsertedFirst = -1;
			public int RowInsertedLength;
			public int RowUpdatingFirst = -1;
			public int RowUpdatingLength;
			public int RowUpdatedFirst = -1;
			public int RowUpdatedLength;
			public int RowDeletingFirst = -1;
			public int RowDeletingLength;
			public int RowDeletedFirst = -1;
			public int RowDeletedLength;
			public int RowPersistingFirst = -1;
			public int RowPersistingLength;
			public int RowPersistedFirst = -1;
			public int RowPersistedLength;
			public int CommandPreparingFirst = -1;
			public int CommandPreparingLength;
		}
		internal delegate void _CacheAttachedDelegate(PXGraph graph, PXCache sender);
		internal protected EventsRow _EventsRow = new EventsRow();
		protected EventsRowAttr _EventsRowAttr = new EventsRowAttr();
		//protected Dictionary<object, EventsRowAttr> _EventsRowItem;

		#endregion
		#region Public Properties
		/// <summary>
        /// Gets or sets the user-friendly name set by the
        /// <see cref="PXCacheNameAttribute">PXCacheName</see> attribute.
        /// </summary>
		public string DisplayName { get; set; }

		#endregion

    	internal abstract void ClearSession();

		internal virtual bool HasChangedKeys()
		{
			return false;
		}

	    internal bool isKeysFilled(object item)
	    {
		    foreach (string key in _Keys)
		    {
			    if (GetValue(item, key) == null)
			    {
				    return false;
			    }
		    }
		    return true;
	    }

	    public abstract int LocateByNoteID(Guid noteId);
	}

	public static class PXCacheEx
	{
		public static TExtension GetExtension<TExtension>(this IBqlTable item)
			where TExtension : PXCacheExtension
		{
			var dict = PXContext.GetSlot<PXCacheExtensionCollection>();
			if (dict == null)
			{
				dict = PXContext.SetSlot<PXCacheExtensionCollection>(new PXCacheExtensionCollection());
			}


			lock (((ICollection) dict).SyncRoot)
			{
				PXCacheExtension[] itemextensions;

				if (dict.TryGetValue(item, out itemextensions))
				{
					return (TExtension) itemextensions.FirstOrDefault((x) => x is TExtension && x.GetType() == typeof (TExtension));
				}
			}


			throw new PXException("GetItemExtension failed.");


		}

		public static PXCacheExtension[] GetExtensions(this IBqlTable item)
		{
			var dict = PXContext.GetSlot<PXCacheExtensionCollection>();
			if (dict == null)
			{
				dict = PXContext.SetSlot<PXCacheExtensionCollection>(new PXCacheExtensionCollection());
			}


			lock (((ICollection) dict).SyncRoot)
			{
				PXCacheExtension[] itemextensions;

				if (dict.TryGetValue(item, out itemextensions))
				{
					return itemextensions;
				}
			}


			return null;


		}



	}
}
