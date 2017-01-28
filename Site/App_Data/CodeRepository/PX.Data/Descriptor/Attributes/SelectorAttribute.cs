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

using PX.Api;
using PX.Api.Soap.Screen;
using PX.Common;
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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using PX.Data.Update.ExchangeService;

namespace PX.Data
{
	#region PXSelectorAttribute
    /// <summary>Configures the lookup control for a DAC field that references
    /// a data record from a particular table by holding its key
    /// field.See</summary>
    /// <remarks>
    /// <para>The attribute configures the input control for a DAC field that
    /// references a data record from a particular table. Such field holds a
    /// key value that identifies the data record in this table.</para>
    /// <para>The input control will be of "lookup" type (may also be called a
    /// "selector"). A user can either input the value for the field manually
    /// or select from the list of the data records. If a value is inserted
    /// manually, the attribute checks if it is included in the list. You can
    /// specify a complex BQL query to define the set of data records that
    /// appear in the list.</para>
    /// <para>The key field usually represents a database identity column that
    /// may not be user-friendly (surrogate key). It is possible to substitute
    /// its value with the value of another field from the same data record
    /// (natural key). This field should be specified in the
    /// <tt>SubstituteKey</tt> property. In this case, the table, and the DAC,
    /// have two fields that uniquely identify a data record from this table.
    /// For example, the <tt>Account</tt> table may have the numeric
    /// <tt>AccountID</tt> field and the user-friendly string
    /// <tt>AccountCD</tt> field. On a field that references <tt>Account</tt>
    /// data records in another DAC, you should place the <tt>PXSelector</tt>
    /// attribute as follows.</para>
    /// <code>
    /// [PXSelector(typeof(Search&lt;Account.accountID&gt;),
    ///             SubstituteKey = typeof(Account.accountCD))]</code>
    /// <para>The attribute will automatically convert the stored numeric
    /// value to the displayed string value and back. Note that only the
    /// <tt>AccountCD</tt> property should be marked with <tt>IsKey</tt>
    /// property set to <tt>true</tt>.</para>
    /// <para>It is also possible to define the list of columns to display.
    /// You can use an appropriated constructor and specify the types of the
    /// fields. By default, all fields that have the <tt>PXUIField</tt>
    /// attribute's <tt>Visibility</tt> property set to
    /// <tt>PXUIVisibility.SelectorVisible</tt>.</para>
    /// <para>Along with a key, some other field can be displayed as the
    /// description of the key. This field should be specified in the
    /// <tt>DescriptionField</tt> property. The way the description is
    /// displayed in the lookup control is configured in the webpage layout
    /// through the <tt>DisplayMode</tt> property of the <tt>PXSelector</tt>
    /// control. The default display format is <i>ValueField &#8211;
    /// DescriptionField</i>. It can be changed to display the description
    /// only.</para>
    /// <para>To achieve better performance, the attribute can be configured
    /// to cache the displayed data records.</para>
    /// </remarks>
    /// <example>
    /// <para>The first example below shows the simplest <tt>PXSelector</tt> attribute
    /// declaration. All <tt>Category</tt> data records will be available for
    /// selection. Their <tt>CategoryCD</tt> field values will be inserted
    /// without conversion.</para>
    /// <para>Other code samples show DAC field definitions with more
    /// complex configurations of lookup controls.</para>
    /// <code>
    /// [PXSelector(typeof(Category.categoryCD))]
    /// public virtual string CategoryCD { get; set; }
    /// </code>
    /// <code>
    /// [PXDBInt(IsKey = true, BqlField = typeof(CuryARHistory.branchID))]
    /// [PXSelector(typeof(Branch.branchID), SubstituteKey = typeof(Branch.branchCD))]
    /// public virtual Int32? BranchID { get; set; }
    /// </code>
    /// <code>
    /// [PXDBString(15, IsUnicode = true, InputMask = "&gt;CCCCCCCCCCCCCCC")]
    /// [PXUIField(DisplayName = "Order Nbr.", Enabled = false)]
    /// [PXSelector(
    ///     typeof(Search&lt;SOOrder.orderNbr,
    ///         Where&lt;SOOrder.orderType, Equal&lt;Optional&lt;ARAdjust.adjdOrderType&gt;&gt;&gt;&gt;),
    ///     Filterable = true)]
    /// public virtual String AdjdOrderNbr { get; set; }
    /// </code>
    /// <code>
    /// [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = "&gt;CCCCCCCCCCCCCCC")]
    /// [PXDefault()]
    /// [PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible, TabOrder = 1)]
    /// [PXSelector(
    ///     typeof(Search&lt;ARPriceWorksheet.refNbr&gt;),
    ///     new Type[] {
    ///         typeof(ARPriceWorksheet.refNbr),
    ///         typeof(ARPriceWorksheet.status),
    ///         typeof(ARPriceWorksheet.descr)
    ///     })]
    /// public virtual String RefNbr { get; set; }
    /// </code>
    /// </example>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Parameter | AttributeTargets.Method)]
	[PXAttributeFamily(typeof(PXSelectorAttribute))]
	public class PXSelectorAttribute : PXEventSubscriberAttribute, IPXFieldVerifyingSubscriber, IPXFieldSelectingSubscriber, IPXDependsOnFields
	{
		#region State
        public bool IsPrimaryViewCompatible { get; set; }
		protected Type _Type;
		protected Type _BqlType;
	    protected Type _FilterEntity;
		protected Type _CacheType;
		protected BqlCommand _Select;
		/// <summary>
		/// Returns Bql command used for selection of referenced records.
		/// </summary>
        /// <exclude/>
		public virtual BqlCommand GetSelect()
		{
			return _Select;
		}
		protected int _ParsCount;
		protected BqlCommand _PrimarySelect;
		protected BqlCommand _OriginalSelect;
		protected BqlCommand _NaturalSelect;
		protected BqlCommand _UnconditionalSelect;
		protected string[] _FieldList;
		protected string[] _HeaderList;
		protected string _ViewName;
		protected Type _DescriptionField;
		protected Type _SubstituteKey;
		protected bool _DirtyRead;
		protected bool _Filterable;
		protected bool _CacheGlobal;
		protected bool _ViewCreated;
		protected bool _IsOwnView;
		protected UIFieldRef _UIFieldRef;

		protected Delegate _ViewHandler;

        /// <summary>Gets or sets the value that indicates whether the attribute
        /// should cache the data records retrieved from the database to show in
        /// the lookup control. By default, the attribute does not cache the data
        /// records.</summary>
		public virtual bool CacheGlobal
		{
			get
			{
				return _CacheGlobal;
			}
			set
			{
				if (_NaturalSelect != null && _CacheGlobal != value) {
					IBqlParameter[] pars = _NaturalSelect.GetParameters();
					Type surrogate = pars[pars.Length - 1].GetReferencedType();
					if (!value) {
						_NaturalSelect = _Select.WhereAnd(BqlCommand.Compose(typeof(Where<,>), surrogate, typeof(Equal<>), typeof(Required<>), surrogate));
					}
					else {
						Type field = ((IBqlSearch)_Select).GetField();
						_NaturalSelect = BqlCommand.CreateInstance(typeof(Search<,>), field, typeof(Where<,>), surrogate, typeof(Equal<>), typeof(Required<>), surrogate);
					}
				}
				_CacheGlobal = value;
			}
		}
        /// <summary>Gets or sets the field from the referenced table that
        /// contains the description.</summary>
        /// <example>
        /// In the code below, the <apiname>PXSelector</apiname> attribute configures
        /// a lookup field that will let a user select from the data set defined
        /// by the <tt>Search</tt> query. The lookup control will display descriptions
        /// of the data records taken from <tt>CRLeadClass.description</tt> field.
        /// <code>
        /// [PXDBString(10, IsUnicode = true, InputMask = "&gt;aaaaaaaaaa")]
        /// [PXUIField(DisplayName = "Class ID")]
        /// [PXSelector(
        ///     typeof(Search&lt;CRLeadClass.cRLeadClassID,
        ///                Where&lt;CRLeadClass.isActive, Equal&lt;True&gt;&gt;&gt;),
        ///     DescriptionField = typeof(CRLeadClass.description))]
        /// public virtual string ClassID { get; set; }
        /// </code>
        /// </example>
		public virtual Type DescriptionField
		{
			get
			{
				return _DescriptionField;
			}
			set
			{
				if (value == null || typeof(IBqlField).IsAssignableFrom(value) && value.IsNested) {
					_DescriptionField = value;
				}
				else {
					throw new PXException(ErrorMessages.CantSetDescriptionField, value);
				}
			}
		}

		/// <summary>
		/// Gets or sets the type that is used as a key for saved filters.
		/// </summary>
	    public virtual Type FilterEntity
	    {
			get { return _FilterEntity; }
		    set
		    {
			    if (value == null || typeof (IBqlTable).IsAssignableFrom(value))
				    _FilterEntity = value;
			    else
				    throw new PXException(ErrorMessages.CantSetFilterEntity, value);
		    }
	    }

        /// <summary>Gets or sets the field from the referenced table that
        /// substitutes the key field used as internal value and is displayed as a
        /// value in the user interface (natural key).</summary>
        /// <example>
        /// The attribute below let the user select from the <tt>Branch</tt> data records.
        /// The attribute displays the <tt>Branch.BranchCD</tt> field value in the user
        /// interface, but actually assigns the <tt>Branch.BranchID</tt> field value to the
        /// field.
        /// <code>
        /// [PXSelector(typeof(Branch.branchID),
        ///             SubstituteKey = typeof(Branch.branchCD))]
        /// public virtual int? BranchID { get; set; }
        /// </code>
        /// </example>
		public virtual Type SubstituteKey
		{
			get
			{
				return _SubstituteKey;
			}
			set
			{
				if (value != null && typeof(IBqlField).IsAssignableFrom(value) && value.IsNested) {
					_SubstituteKey = value;
					if (!_CacheGlobal) {
						_NaturalSelect = _Select.WhereAnd(BqlCommand.Compose(typeof(Where<,>), value, typeof(Equal<>), typeof(Required<>), value));
					}
					else {
						Type field = ((IBqlSearch)_Select).GetField();
						_NaturalSelect = BqlCommand.CreateInstance(typeof(Search<,>), field, typeof(Where<,>), value, typeof(Equal<>), typeof(Required<>), value);
					}
				}
				else {
					throw new PXException(ErrorMessages.CantSubstituteKey, value);
				}
			}
		}
        /// <summary>Gets the field that identifies a referenced data record
        /// (surrogate key) and is assigned to the field annotated with the
        /// <tt>PXSelector</tt> attribute. Typically, it is the first parameter of
        /// the BQL query passed to the attribute constructor.</summary>
		public virtual Type Field
		{
			get
			{
				return ((IBqlSearch)_Select).GetField();
			}
		}
        /// <summary>Gets or sets a value that indicates whether the attribute
        /// should take into account the unsaved modifications when displaying
        /// data records in control. If <tt>false</tt>, the data records are taken
        /// from the database and not merged with the cache object. If
        /// <tt>true</tt>, the data records are merged with the modification
        /// stored in the cache object.</summary>
		public virtual bool DirtyRead
		{
			get
			{
				return _DirtyRead;
			}
			set
			{
				_DirtyRead = value;
			}
		}

		/// <summary>
		/// Allows to control validation process.
		/// </summary>
		public bool ValidateValue = true;

        /// <summary>Gets or sets the value that indicates whether the filters
        /// defined by the user should be stored in the database.</summary>
		public virtual bool Filterable
		{
			get
			{
				return _Filterable;
			}
			set
			{
				_Filterable = value;
			}
		}
        /// <summary>Gets or sets the list of labels for column headers that are
        /// displayed in the lookup control. By default, the attribute uses
        /// display names of the fields.</summary>
		public virtual string[] Headers
		{
			get
			{
				return _HeaderList;
			}
			set
			{
				if (_FieldList == null || value != null && value.Length != _FieldList.Length) {
					throw new PXArgumentException("Headers", ErrorMessages.HeadersNotMeetColList);
				}
				_HeaderList = value;
			}
		}

		protected Type _ValueField;
        /// <summary>
        ///  Gets the referenced data record field whose value is
        ///  assigned to the current field (marked with the <tt>PXSelector</tt>
        ///  attribute).
        /// </summary>
		public Type ValueField
		{
			get
			{
				return _ValueField;
			}
		}

		protected PXSelectorMode _SelectorMode;
        /// <summary>
        /// Gets or sets the value that determines the value displayed by
        /// the selector control in the UI and some aspects of
        /// attribute's behavior. You can assign a combination of
        /// <see cref="PXSelectorMode">PXSelectorMode</see> values joined
        /// by bitwise or ("|").
        /// </summary>
        /// <example>
        /// In the following example, the <tt>SelectorMode</tt> property
        /// is used to disable autocompletion in the selector control.
        /// <code>
        /// ...
        /// [PXSelector(
        ///     typeof(FinPeriod.finPeriodID), 
        ///     DescriptionField = typeof(FinPeriod.descr),
        ///     SelectorMode = PXSelectorMode.NoAutocomplete)]
        /// public virtual String FinPeriodID { get; set; }
        /// </code>
        /// </example>
		public virtual PXSelectorMode SelectorMode
		{
			get
			{
				string key = _CacheType.FullName + "_AutoNumber";
				HashSet<string> fields;
				if ((fields = PXContext.GetSlot<HashSet<string>>(key)) == null || !fields.Contains(_FieldName)) {
					return _SelectorMode;
				}
				return _SelectorMode & ~PXSelectorMode.NoAutocomplete;
			}
			set
			{
				this._SelectorMode = value;
			}
		}

        /// <summary>Gets the BQL query that is used to retrieve data records to
        /// show to the user.</summary>
		public BqlCommand PrimarySelect
		{
			get { return _PrimarySelect; }
		}

        /// <exclude/>
		public int ParsCount
		{
			get { return _ParsCount; }
		}
		/// <exclude/>
		internal Type Type
		{
			get { return _Type; }
		}
		#endregion

		#region Ctor
		static PXSelectorAttribute()
		{
			Type t = System.Web.Compilation.PXBuildManager.GetType("PX.Objects.CS.FeaturesSet", false);
			if (t != null)
			{
				PXDatabase.Subscribe(t, () =>
					{
						lock (_flock)
						{
							_fields.Clear();
							_headers.Clear();
						}
					});
			}
		}
        /// <summary>Initializes a new instance that will use the specified BQL
        /// query to retrieve the data records to select from. The list of
        /// displayed columns is created automatically and consists of all columns
        /// from the referenced table with the <tt>Visibility</tt> property of the
        /// <see cref="PXUIFieldAttribute">PXUIField</see> attribute set to
        /// <tt>PXUIVisibility.SelectorVisible</tt>.</summary>
        /// <param name="type">A BQL query that defines the data set that is shown
        /// to the user along with the key field that is used as a value. Set to a
        /// field (type part of a DAC field) to select all data records from the
        /// referenced table. Set to a BQL command of <tt>Search</tt> type to
        /// specify a complex select statement.</param>
		public PXSelectorAttribute(Type type)
		{
			if (type == null) {
				throw new PXArgumentException("type", ErrorMessages.ArgumentNullException);
			}
			if (typeof(IBqlSearch).IsAssignableFrom(type)) {
				_Select = BqlCommand.CreateInstance(type);
				_Type = BqlCommand.GetItemType(((IBqlSearch)_Select).GetField());
			}
			else if (type.IsNested && typeof(IBqlField).IsAssignableFrom(type)) {
				_Type = BqlCommand.GetItemType(type);
				_Select = BqlCommand.CreateInstance(typeof(Search<>), type);
			}
			else {
				throw new PXArgumentException("type", ErrorMessages.CantCreateForeignKeyReference, type);
			}
			_BqlType = _Type;
			while (typeof(IBqlTable).IsAssignableFrom(_BqlType.BaseType)
				&& !_BqlType.IsDefined(typeof(PXTableAttribute), false)
				&& !_BqlType.IsDefined(typeof(PXTableNameAttribute), false)) {
				_BqlType = _BqlType.BaseType;
			}
			Type field = ((IBqlSearch)_Select).GetField();
			_ValueField = field;
			_ViewName = GenerateViewName();
			_PrimarySelect = _Select.WhereAnd(BqlCommand.Compose(typeof(Where<,>), field, typeof(Equal<>), typeof(Required<>), field));
			_OriginalSelect = BqlCommand.CreateInstance(_Select.GetSelectType());
			_UnconditionalSelect = BqlCommand.CreateInstance(typeof(Search<,>), field, typeof(Where<,>), field, typeof(Equal<>), typeof(Required<>), field);
		}
        /// <summary>Initializes a new instance that will use the specified BQL
        /// query to retrieve the data records to select from, and display the
        /// provided set of columns.</summary>
        /// <param name="type">A BQL query that defines the data set that is shown
        /// to the user along with the key field that is used as a value. Set to a
        /// field (type part of a DAC field) to select all data records from the
        /// referenced table. Set to a BQL command of <tt>Search</tt> type to
        /// specify a complex select statement.</param>
        /// <param name="fieldList">Fields to display in the control.</param>
        /// <example>
        /// The attribute below configures the lookup control to let the user select from the
        /// <tt>Customer</tt> data records retrieved by the <tt>Search</tt> BQL
        /// query. The displayed columns are specified explicitly: <tt>AccountCD</tt> and
        /// <tt>CompanyName</tt>. The <tt>Customer.accountCD</tt> field data will be
        /// inserted as a value without conversion.
        /// <code>
        /// [PXSelector(
        ///     typeof(Search&lt;Customer.accountCD, 
        ///                Where&lt;Customer.companyType, Equal&lt;CompanyType.customer&gt;&gt;&gt;),
        ///     new Type[] 
        ///     {
        ///         typeof(Customer.accountCD),
        ///         typeof(Customer.companyName)
        ///     })]
        /// public virtual string AccountCD { get; set; }
        /// </code>
        /// </example>
		public PXSelectorAttribute(Type type, params Type[] fieldList)
			: this(type)
        {
	        SetFieldList(fieldList);
        }

	    internal void SetFieldList(Type[] fieldList)
	    {
		    _FieldList = new string[fieldList.Length];
		    Type[] tables = _Select.GetTables();
		    for (int i = 0; i < fieldList.Length; i++)
		    {
			    if (!fieldList[i].IsNested || !typeof (IBqlField).IsAssignableFrom(fieldList[i]))
			    {
				    throw new PXArgumentException("fieldList", ErrorMessages.InvalidSelectorColumn);
			    }
			    if (tables.Length <= 1 || BqlCommand.GetItemType(fieldList[i]).IsAssignableFrom(tables[0]))
			    {
				    _FieldList[i] = fieldList[i].Name;
			    }
			    else
			    {
				    _FieldList[i] = BqlCommand.GetItemType(fieldList[i]).Name + "__" + fieldList[i].Name;
			    }
		    }
	    }

	    public interface IPXAdjustableView { }

		public class PXAdjustableView : PXView, IPXAdjustableView
		{
			public PXAdjustableView(PXGraph graph, bool isReadOnly, BqlCommand @select, Delegate handler)
				: base(graph, isReadOnly, @select, handler)
			{
			}
		}

        /// <exclude/>
		public BqlCommand WhereAnd(PXCache sender, Type whr)
		{
			if (!typeof(IBqlWhere).IsAssignableFrom(whr)) return _PrimarySelect;

			_Select = _Select.WhereAnd(whr);

			if (_ViewHandler == null) {
				_ViewHandler = new PXSelectDelegate(
					delegate {
						int startRow = PXView.StartRow;
						int totalRows = 0;

						if (PXView.MaximumRows == 1) {
							IBqlParameter[] selpars = _Select.GetParameters();
							object[] parameters = PXView.Parameters;
							List<object> pars = new List<object>();

							for (int i = 0; i < selpars.Length && i < parameters.Length; i++) {
								if (selpars[i].MaskedType != null) {
									break;
								}
								if (selpars[i].IsVisible) {
									pars.Add(parameters[i]);
								}
							}

							return new PXView(sender.Graph, !_DirtyRead, _OriginalSelect).Select(PXView.Currents, pars.ToArray(), PXView.Searches, PXView.SortColumns, PXView.Descendings, PXView.Filters, ref startRow, PXView.MaximumRows, ref totalRows);
						}

						return null;
					});
			}

			if (_ViewCreated) {
				// recreate selector view
				CreateView(sender);
			}

			return _PrimarySelect.WhereAnd(whr);
		}

		/// <summary>
		/// Generates default view name. View name is used by UI controls when selecting list of records available for selection.
		/// </summary>
		/// <returns>A string that references a PXView instance which will be used to retrive a list of records.</returns>
		protected virtual string GenerateViewName()
		{
			if (!typeof(IBqlSearch).IsAssignableFrom(_Select.GetType())) return null;

			var parameters = _Select.GetParameters();
			if (parameters != null) _ParsCount = parameters.Count(p => p.IsVisible);

			//			var parameters = _Select.GetParameters();
			//			var bld = new StringBuilder("_");
			//			bld.Append(_Type.Name);
			//			if (parameters != null)
			//				foreach (var par in parameters)
			//				{
			//					if (!par.HasDefault) throw new PXArgumentException("sourceType", ErrorMessages.NotCurrentOrOptionalParameter);
			//					if (par.IsVisible) _ParsCount++;
			//					var t = par.GetReferencedType();
			//					bld.Append('_');
			//					bld.Append(BqlCommand.GetItemType(t).Name);
			//					bld.Append('.');
			//					bld.Append(t.Name);
			//				}
			//			bld.Append('_');
			//			return bld.ToString();
			return string.Format("_{0}_", ((IBqlSearch)_Select).GetField().FullName);
		}

		#endregion

		#region Runtime
		/// <summary>
		/// A wrapper to PXView.SelectMultiBound() method, extracts the first table in a row if a result of a join is returned.<br/>
		/// While we are looking for a single record here, we still call SelectMulti() for performance reason, to hit cache and get the result of previously executed queries if any.<br/>
		/// 'Bound' means we will take omitted parameters from explicitly defined array of rows, not from current records set in the graph.
		/// </summary>
		/// <param name="view">PXView instance to be called for a selection result</param>
		/// <param name="currents">List of rows used as a source for omitted parameter values</param>
		/// <param name="pars">List of parameters to be passed to the query</param>
		/// <returns>Foreign record retrieved</returns>
		internal static object SelectSingleBound(PXView view, object[] currents, params object[] pars)
		{
			List<object> ret = view.SelectMultiBound(currents, pars);
			if (ret.Count > 0) {
				if (ret[0] is PXResult) {
					return ((PXResult)ret[0])[0];
				}
				return ret[0];
			}

			return null;
		}
		/// <summary>
		/// A wrapper to PXView.SelectSingleBound() method, extracts the first table in a row if a result of a join is returned.<br/>
		/// </summary>
		/// <param name="view">PXView instance to be called for a selection result</param>
		/// <param name="pars">List of parameters to be passed to the query</param>
		/// <returns>Foreign record retrieved</returns>
		internal static object SelectSingle(PXView view, params object[] pars)
		{
			List<object> ret = view.SelectMulti(pars);
			if (ret.Count > 0) {
				if (ret[0] is PXResult) {
					return ((PXResult)ret[0])[0];
				}
				return ret[0];
			}
			return null;
		}

        internal static object SelectSingle(PXCache cache, object data, string field, object value)
        {
            foreach (PXEventSubscriberAttribute attr in cache.GetAttributesReadonly(field))
            {
                if (attr is PXSelectorAttribute)
                {
                    PXView select = ((PXSelectorAttribute)attr).GetView(cache, ((PXSelectorAttribute)attr)._PrimarySelect, !((PXSelectorAttribute)attr)._DirtyRead);
                    object[] pars = new object[((PXSelectorAttribute)attr)._ParsCount + 1];
                    pars[pars.Length - 1] = value;

                    List<object> ret = select.SelectMultiBound(new object[] { data }, pars);
                    if (ret.Count > 0)
                    {
                        return ret[0];
                    }

                    return null;
                }
            }
            return null;
        }

		internal static object SelectSingle(PXCache cache, object data, string field)
		{
            object value = cache.GetValue(data, field);
            return SelectSingle(cache, data, field, value);
		}
		/// <summary>
		/// Returns cached typed view, can be ovirriden to substitute a view with a delegate instead.
		/// </summary>
		/// <param name="cache">PXCache instance, used to retrive a graph object</param>
		/// <param name="select">Bql command to be searched</param>
		/// <param name="dirtyRead">Flag to separate result sets either merged with not saved changes or not</param>
		/// <returns></returns>
		protected virtual PXView GetView(PXCache cache, BqlCommand select, bool isReadOnly)
		{
			return cache.Graph.TypedViews.GetView(select, isReadOnly);
		}
        /// <summary>Returns the data record referenced by the attribute instance
        /// that marks the field with the specified name in a particular data
        /// record.</summary>
        /// <param name="cache">The cache object to search for the attributes of
        /// <tt>PXSelector</tt> type.</param>
        /// <param name="data">The data record the method is applied to.</param>
        /// <param name="field">The name of the field that is be marked with the
        /// attribute.</param>
		public static object Select(PXCache cache, object data, string field)
		{
			object value = cache.GetValue(data, field);
			return Select(cache, data, field, value);
		}

        /// <summary>Returns the first data record retrieved by the attribute
        /// instance that marks the specified field in a particular data
        /// record.</summary>
        /// <param name="cache">The cache object to search for the attributes of
        /// <tt>PXSelector</tt> type.</param>
        /// <param name="data">The data record the method is applied to.</param>
		public static object SelectFirst<Field>(PXCache cache, object data)
			where Field : IBqlField
		{
			return SelectFirst(cache, data, typeof(Field).Name);
		}
        /// <summary>Returns the first data record retrieved by the attribute
        /// instance that marks the field with the specified name in a particular
        /// data record.</summary>
        /// <param name="cache">The cache object to search for the attributes of
        /// <tt>PXSelector</tt> type.</param>
        /// <param name="data">The data record the method is applied to.</param>
        /// <param name="field">The name of the field that is be marked with the
        /// attribute.</param>
		public static object SelectFirst(PXCache cache, object data, string field)
		{
			foreach (PXEventSubscriberAttribute attr in cache.GetAttributesReadonly(field)) {
				if (attr is PXSelectorAttribute) {
					PXView view = cache.Graph.TypedViews.GetView(((PXSelectorAttribute)attr)._Select, !((PXSelectorAttribute)attr)._DirtyRead);
					int startRow = 0;
					int totalRows = 0;
					List<object> source = view.Select(new object[] { data }, null, null, null, null, null, ref startRow, 1, ref totalRows);
					if (source != null && source.Count > 0) {
						object item = source[source.Count - 1];
						if (item != null && item is PXResult) {
							item = ((PXResult)item)[0];
						}
						return item;
					}
					return null;
				}
			}
			return null;
		}

        /// <summary>Returns the last data record retrieved by the attribute
        /// instance that marks the specified field in a particular data
        /// record.</summary>
        /// <param name="cache">The cache object to search for the attributes of
        /// <tt>PXSelector</tt> type.</param>
        /// <param name="data">The data record the method is applied to.</param>
		public static object SelectLast<Field>(PXCache cache, object data)
			where Field : IBqlField
		{
			return SelectLast(cache, data, typeof(Field).Name);
		}
        /// <summary>Returns the last data record retrieved by the attribute
        /// instance that marks the field with the specified name in a particular
        /// data record.</summary>
        /// <param name="cache">The cache object to search for the attributes of
        /// <tt>PXSelector</tt> type.</param>
        /// <param name="data">The data record the method is applied to.</param>
        /// <param name="field">The name of the field that is be marked with the
        /// attribute.</param>
		public static object SelectLast(PXCache cache, object data, string field)
		{
			foreach (PXEventSubscriberAttribute attr in cache.GetAttributesReadonly(field)) {
				if (attr is PXSelectorAttribute) {
					PXView view = cache.Graph.TypedViews.GetView(((PXSelectorAttribute)attr)._Select, !((PXSelectorAttribute)attr)._DirtyRead);
					int startRow = -1;
					int totalRows = 0;
					List<object> source = view.Select(new object[] { data }, null, null, null, null, null, ref startRow, 1, ref totalRows);
					if (source != null && source.Count > 0) {
						object item = source[source.Count - 1];
						if (item != null && item is PXResult) {
							item = ((PXResult)item)[0];
						}
						return item;
					}
					return null;
				}
			}
			return null;
		}

        /// <summary>Returns the referenced data record that holds the specified
        /// value. The data record should be referenced by the attribute instance
        /// that marks the field with the specified in a particular data
        /// record.</summary>
        /// <param name="cache">The cache object to search for the attributes of
        /// <tt>PXSelector</tt> type.</param>
        /// <param name="data">The data record the method is applied to.</param>
        /// <param name="field">The name of the field that is be marked with the
        /// attribute.</param>
        /// <param name="value">The value to search the referenced table
        /// for.</param>
        /// <returns>Foreign record.</returns>
		public static object Select(PXCache cache, object data, string field, object value)
		{
			foreach (PXEventSubscriberAttribute attr in cache.GetAttributesReadonly(field)) {
				if (attr is PXSelectorAttribute) {
					return GetItem(cache, (PXSelectorAttribute)attr, data, value);
				}
			}
			return null;
		}

		internal sealed class GlobalDictionary :IPXCompanyDependent
		{
			struct CacheValue
			{
				public KeyValuePair<object, bool> Value;
				public PXCacheExtension[] Extensions;

			}
			readonly Dictionary<object, CacheValue> Items = new Dictionary<object, CacheValue>();

			public object SyncRoot
			{
				get { return ((ICollection) Items).SyncRoot; }
			}
			public bool TryGetValue(object key, out KeyValuePair<object, bool> pair)
			{
				CacheValue ret;
				if (!Items.TryGetValue(key, out ret))
				{
					pair = default(KeyValuePair<object, bool>);
					return false;
				}

				pair = ret.Value;

				if (ret.Extensions != null)
				{
					var r = pair.Key as IBqlTable;
					
					var ext = PXCacheExtensionCollection.GetSlot(true);
					lock (ext.SyncRoot)
					{
						ext[r] = ret.Extensions;
					}
				}

				return true;
			}

			public void Set(object key, object row, bool deleted)
			{
				var store = new CacheValue();
				store.Value = new KeyValuePair<object, bool>(row, deleted);
				var r = row as IBqlTable;
				if (r != null)
				{
					store.Extensions = r.GetExtensions();
				}
				

				Items[key] = store;
			}

		}

        /// <summary>Returns the foreign data record by the specified
        /// key.</summary>
        /// <param name="cache">The cache object to search for the attributes of
        /// <tt>PXSelector</tt> type.</param>
        /// <param name="attr">The instance of the <tt>PXSelector</tt> attribute
        /// to query for a data record.</param>
        /// <param name="data">The data record that contains a reference to the
        /// foreign data record.</param>
        /// <param name="key">The key value of the referenced data record.</param>
		public static object GetItem(PXCache cache, PXSelectorAttribute attr, object data, object key)
		{
			return GetItem(cache, attr, data, key, false);
		}
		internal static object GetItem(PXCache cache, PXSelectorAttribute attr, object data, object key, bool unconditionally)
		{
			object row = null;
			GlobalDictionary dict = null;
			if (attr._CacheGlobal && key != null) {
				dict = PXContext.GetSlot<GlobalDictionary>(_GetSlotName(attr._Type));
				if (dict == null)
				{
					PXContext.SetSlot<GlobalDictionary>(attr._Type.FullName, (dict = PXDatabase.GetSlot<GlobalDictionary>(_GetSlotName(attr._Type), attr._BqlType)));
				}
				lock ((dict).SyncRoot) {
					KeyValuePair<object, bool> pair;
					if (dict.TryGetValue(key, out pair) && !pair.Value && !(pair.Key is IDictionary))
					{
						row = pair.Key;
					}
				}
			}
			if (row == null && (key == null || key.GetType() == cache.GetFieldType(attr._FieldName)))
			{
				PXView select = attr.GetView(cache, attr._PrimarySelect, !attr._DirtyRead);
				object[] pars = new object[attr._ParsCount + 1];
				pars[pars.Length - 1] = key;
				row = SelectSingleBound(select, new object[] { data }, pars);
				if (row == null)
				{
					if (!unconditionally)
					{
					return null;
				}
					select = attr.GetView(cache, attr._UnconditionalSelect, !attr._DirtyRead);
					row = SelectSingleBound(select, new object[] { data }, key);
					return row;
				}
				if (attr._CacheGlobal && key != null && select.Cache.GetStatus(row) == PXEntryStatus.Notchanged && !PXDatabase.ReadDeleted && select.Cache.Keys.Count <= 1)
				{
					CheckIntegrityAndPutGlobal(cache, select.Cache, ((IBqlSearch)attr._Select).GetField().Name, dict, row, false, key, attr._FieldName);
				}
			}
			return row;
		}
        /// <summary>Clears the internal cache of the <tt>PXSelector</tt>
        /// attribute, removing the data records retrieved from the specified
        /// table. Typically, you don't need to call this method, because the
        /// attribute subscribes on the change notifications related to the table
        /// and drops the cache automatically.</summary>
		public static void ClearGlobalCache<Table>()
			where Table : IBqlTable
		{
			PXDatabase.ResetSlot<GlobalDictionary>(typeof(Table).FullName, typeof(Table));
		}
        /// <summary>Clears the internal cache of the <tt>PXSelector</tt>
        /// attribute, removing the data records retrieved from the specified
        /// table. Typically, you don't need to call this method, because the
        /// attribute subscribes on the change notifications related to the table
        /// and drops the cache automatically.</summary>
        /// <param name="table">The DAC to drop from the attribute's
        /// cache.</param>
		public static void ClearGlobalCache(Type table)
		{
			if (table == null) {
				throw new PXArgumentException(ErrorMessages.ArgumentNullException, "table");
			}
			PXDatabase.ResetSlot<GlobalDictionary>(table.FullName, table);
		}
        /// <summary>Returns a value of the field from a foreign data
        /// record.</summary>
        /// <param name="cache">The cache object to search for the attributes of
        /// <tt>PXSelector</tt> type.</param>
        /// <param name="data">The data record that contains a reference to the
        /// foreign data record.</param>
        /// <param name="field">The name of the field holding the referenced data
        /// record key value.</param>
        /// <param name="value">The key value of the referenced data
        /// record.</param>
        /// <param name="foreignField">The name of the referenced data record
        /// field whose value is returned by the method.</param>
		public static object GetField(PXCache cache, object data, string field, object value, string foreignField)
		{
			foreach (PXEventSubscriberAttribute attr in cache.GetAttributesReadonly(data, field)) {
				if (attr is PXSelectorAttribute) {
					object row = null;
					GlobalDictionary dict = null;
					if (((PXSelectorAttribute)attr)._CacheGlobal && value != null) {
						dict = PXDatabase.GetSlot<GlobalDictionary>(_GetSlotName(((PXSelectorAttribute)attr)._Type), cache.Graph.Caches[((PXSelectorAttribute)attr)._Type].BqlTable);
						lock ((dict).SyncRoot) {
							KeyValuePair<object, bool> pair;
							if (dict.TryGetValue(value, out pair) && !pair.Value) {
								row = pair.Key;
							}
						}
					}
					if (row == null) {
						PXView select = ((PXSelectorAttribute)attr).GetView(cache, ((PXSelectorAttribute)attr)._PrimarySelect, !((PXSelectorAttribute)attr)._DirtyRead);
						object[] pars = new object[((PXSelectorAttribute)attr)._ParsCount + 1];
						pars[pars.Length - 1] = value;
						row = SelectSingleBound(select, new object[] { data }, pars);
						if (row == null) {
							return null;
						}
						if (((PXSelectorAttribute)attr)._CacheGlobal && value != null && !PXDatabase.ReadDeleted && select.Cache.Keys.Count <= 1)
						{
							CheckIntegrityAndPutGlobal(cache, select.Cache, ((IBqlSearch)((PXSelectorAttribute)attr)._Select).GetField().Name, dict, row, false, value, ((PXSelectorAttribute)attr)._FieldName);
						}
					}
					return cache.Graph.Caches[((PXSelectorAttribute)attr)._Type].GetValue(row, foreignField) ?? new byte[0];
				}
			}
			return null;
		}

		internal static void CheckIntegrityAndPutGlobal(PXCache sender, PXCache cache, string referencedField, GlobalDictionary dict, object row, bool deleted, object key, string fieldName, bool putsingle = false)
		{
			object val;
			bool putboth = false;
			if ((object.Equals(key, (val = cache.GetValue(row, referencedField)))
				|| (putboth = (!putsingle && key is string && val is string && string.Equals(((string)key).TrimEnd(), ((string)val).TrimEnd()))))
				&& (cache.Keys.Count == 0
				|| cache.Keys.Contains(referencedField)
				|| cache.Identity == null
				|| String.Equals(cache.Identity, referencedField, StringComparison.OrdinalIgnoreCase)))
			{
				lock ((dict).SyncRoot)
				{
					dict.Set(key, row, deleted);
					if (putboth)
					{
						dict.Set(val, row, deleted);

						//dict[val] = new KeyValuePair<object, bool>(row, deleted);
					}
				}
			}
			//else
			//{
			//	PXFirstChanceExceptionLogger.LogMessage(String.Format("Global cache failure: {0}, {1}, {2}", cache.GetItemType().FullName, fieldName, referencedField));
			//}
		}

        /// <summary>Returns the data access class referenced by the attribute
        /// instance that marks the field with specified name.</summary>
        /// <param name="cache">The cache object to search for the attributes of
        /// <tt>PXSelector</tt> type.</param>
        /// <param name="field">The name of the field that marked with the
        /// attribute.</param>
		public static Type GetItemType(PXCache cache, string field)
		{
			foreach (PXEventSubscriberAttribute attr in cache.GetAttributesReadonly(field)) {
				if (attr is PXSelectorAttribute) {
					return ((PXSelectorAttribute)attr)._Type;
				}
			}
			return null;
		}
        /// <summary>Returns all data records kept by the attribute instance the
        /// marks the specified field in a particular data record.</summary>
        /// <param name="cache">The cache object to search for the attributes of
        /// <tt>PXSelector</tt> type.</param>
        /// <param name="data">The data record the method is applied to.</param>
		public static List<object> SelectAll<Field>(PXCache cache, object data)
			where Field : IBqlField
		{
			return SelectAll(cache, typeof(Field).Name, data);
		}
        /// <summary>Returns all data records kept by the attribute instance the
        /// marks the field with the specified name in a particular data
        /// record.</summary>
        /// <param name="cache">The cache object to search for the attributes of
        /// <tt>PXSelector</tt> type.</param>
        /// <param name="fieldname">The name of the field that should be marked
        /// with the attribute.</param>
        /// <param name="data">The data record the method is applied to.</param>
		public static List<object> SelectAll(PXCache cache, string fieldname, object data)
		{
			foreach (PXEventSubscriberAttribute attr in cache.GetAttributesReadonly(fieldname)) {
				if (attr is PXSelectorAttribute) {
					PXView select = ((PXSelectorAttribute)attr).GetView(cache, ((PXSelectorAttribute)attr)._Select, !((PXSelectorAttribute)attr)._DirtyRead);
					return select.SelectMultiBound(new object[] { data });
				}
			}
			return null;
		}
        /// <summary>Returns the data record referenced by the attribute instance
        /// that marks the specified field in a particular data record.</summary>
        /// <param name="cache">The cache object to search for the attributes of
        /// <tt>PXSelector</tt> type.</param>
        /// <param name="data">The data record the method is applied to.</param>
		public static object Select<Field>(PXCache cache, object data)
			where Field : IBqlField
		{
			return Select(cache, data, typeof(Field).Name);
		}
        /// <summary>Returns the referenced data record that holds the specified
        /// value. The data record is searched among the ones referenced by the
        /// attribute instance that marks the specified field in a particular data
        /// record.</summary>
        /// <param name="cache">The cache object to search for the attributes of
        /// <tt>PXSelector</tt> type.</param>
        /// <param name="data">The data record the method is applied to.</param>
        /// <param name="value">The value to search the referenced table
        /// for.</param>
		public static object Select<Field>(PXCache cache, object data, object value)
			where Field : IBqlField
		{
			return Select(cache, data, typeof(Field).Name, value);
		}
        /// <summary>Sets the list of columns and column headers to display for
        /// the attribute instance that marks the field with the specified name in
        /// a particular data record.</summary>
        /// <param name="cache">The cache object to search for the attributes of
        /// <tt>PXSelector</tt> type.</param>
        /// <param name="data">The data record the method is applied to. If
        /// <tt>null</tt>, the method is applied to all data records kept in the
        /// cache object.</param>
        /// <param name="field">The name of the field marked with the
        /// attribute.</param>
        /// <param name="fieldList">The new list of field names.</param>
        /// <param name="headerList">The new list of column headers.</param>
		public static void SetColumns(PXCache cache, object data, string field, string[] fieldList, string[] headerList)
		{
			if (data == null) {
				cache.SetAltered(field, true);
			}
			foreach (PXEventSubscriberAttribute attr in cache.GetAttributes(data, field)) {
				if (attr is PXSelectorAttribute) {
					((PXSelectorAttribute)attr)._FieldList = fieldList;
					((PXSelectorAttribute)attr)._HeaderList = headerList;
				}
			}
		}
        /// <summary>Sets the list of columns and column headers for all attribute
        /// instances that mark the field with the specified name in all data
        /// records in the cache object.</summary>
        /// <param name="cache">The cache object to search for the attributes of
        /// <tt>PXSelector</tt> type.</param>
        /// <param name="field">The name of the field marked with the
        /// attribute.</param>
        /// <param name="fieldList">The new list of field names.</param>
        /// <param name="headerList">The new list of column headers.</param>
		public static void SetColumns(PXCache cache, string field, string[] fieldList, string[] headerList)
		{
			cache.SetAltered(field, true);
			foreach (PXEventSubscriberAttribute attr in cache.GetAttributes(field)) {
				if (attr is PXSelectorAttribute) {
					((PXSelectorAttribute)attr)._FieldList = fieldList;
					((PXSelectorAttribute)attr)._HeaderList = headerList;
				}
			}
		}
		/// <summary>Sets the list of columns and column headers for an attribute
		/// instance.</summary>
		/// <param name="fieldList">The new list of field names.</param>
		/// <param name="headerList">The new list of column headers.</param>
		public virtual void SetColumns(string[] fieldList, string[] headerList)
		{
			this._FieldList = fieldList;
			this._HeaderList = headerList;
		}
        /// <summary>Sets the list of columns and column headers to display for
        /// the attribute instance that marks the specified field in a particular
        /// data record.</summary>
        /// <param name="cache">The cache object to search for the attributes of
        /// <tt>PXSelector</tt> type.</param>
        /// <param name="data">The data record the method is applied to.</param>
        /// <param name="fieldList">The new list of field names.</param>
        /// <param name="headerList">The new list of column headers.</param>
		public static void SetColumns<Field>(PXCache cache, object data, Type[] fieldList, string[] headerList)
			where Field : IBqlField
		{
			if (data == null) {
				cache.SetAltered<Field>(true);
			}
			foreach (PXEventSubscriberAttribute attr in cache.GetAttributes<Field>(data)) {
				if (attr is PXSelectorAttribute) {
					((PXSelectorAttribute)attr)._FieldList = new string[fieldList.Length];
					Type[] tables = ((PXSelectorAttribute)attr)._Select.GetTables();
					for (int i = 0; i < fieldList.Length; i++) {
						if (!fieldList[i].IsNested || !typeof(IBqlField).IsAssignableFrom(fieldList[i])) {
							throw new PXArgumentException("fieldList", ErrorMessages.InvalidSelectorColumn);
						}
						if (tables.Length <= 1 || BqlCommand.GetItemType(fieldList[i]).IsAssignableFrom(tables[0])) {
							((PXSelectorAttribute)attr)._FieldList[i] = fieldList[i].Name;
						}
						else {
							((PXSelectorAttribute)attr)._FieldList[i] = BqlCommand.GetItemType(fieldList[i]).Name + "__" + fieldList[i].Name;
						}
					}
					((PXSelectorAttribute)attr)._HeaderList = headerList;
				}
			}
		}
        /// <summary>Sets the list of columns and column headers for all attribute
        /// instances that mark the specified field in all data records in the
        /// cache object.</summary>
        /// <param name="cache">The cache object to search for the attributes of
        /// <tt>PXSelector</tt> type.</param>
        /// <param name="fieldList">The new list of field names.</param>
        /// <param name="headerList">The new list of column headers.</param>
		public static void SetColumns<Field>(PXCache cache, Type[] fieldList, string[] headerList)
			where Field : IBqlField
		{
			cache.SetAltered<Field>(true);
			foreach (PXEventSubscriberAttribute attr in cache.GetAttributes<Field>()) {
				if (attr is PXSelectorAttribute) {
					((PXSelectorAttribute)attr)._FieldList = new string[fieldList.Length];
					Type[] tables = ((PXSelectorAttribute)attr)._Select.GetTables();
					for (int i = 0; i < fieldList.Length; i++) {
						if (!fieldList[i].IsNested || !typeof(IBqlField).IsAssignableFrom(fieldList[i])) {
							throw new PXArgumentException("fieldList", ErrorMessages.InvalidSelectorColumn);
						}
						if (tables.Length <= 1 || BqlCommand.GetItemType(fieldList[i]).IsAssignableFrom(tables[0])) {
							((PXSelectorAttribute)attr)._FieldList[i] = fieldList[i].Name;
						}
						else {
							((PXSelectorAttribute)attr)._FieldList[i] = BqlCommand.GetItemType(fieldList[i]).Name + "__" + fieldList[i].Name;
						}
					}
					((PXSelectorAttribute)attr)._HeaderList = headerList;
				}
			}
		}

        /// <exclude/>
		public static void StoreCached<Field>(PXCache cache, object data, object item)
			where Field : IBqlField
		{
			foreach (PXEventSubscriberAttribute attr in cache.GetAttributesReadonly<Field>()) {
				if (attr is PXSelectorAttribute) {
					PXView select = ((PXSelectorAttribute)attr).GetView(cache, ((PXSelectorAttribute)attr)._PrimarySelect, !((PXSelectorAttribute)attr)._DirtyRead);
					object[] pars = new object[((PXSelectorAttribute)attr)._ParsCount + 1];
					pars[pars.Length - 1] = cache.GetValue(data, ((PXSelectorAttribute)attr)._FieldOrdinal);

					pars = select.PrepareParameters(new object[] { data }, pars);
					select.StoreCached(new PXCommandKey(pars), new List<object> { item });

					return;
				}
			}
		}
		/// <summary>
		/// Checks foreign keys and raises exception on violation. Works only if foreing key feild has PXSelectorAttribute
		/// </summary>
		/// <param name="Row">Current record</param>
		/// <param name="fieldType">BQL type of foreing key</param>
		/// <param name="searchType">Optional additional BQL statement to be checked</param>
		/// <param name="customMessage">Optional custom message to be displayed to user. Must either have {0} placeholder for name of current table 
		/// and {1} placeholder for foreign key table name, or no format placeholders at all</param>
		public static void CheckAndRaiseForeignKeyException(PXCache sender, object Row, Type fieldType, Type searchType = null, string customMessage = null)
        {
			ForeignKeyChecker checker = new ForeignKeyChecker(sender, Row, fieldType, searchType);
			if (!string.IsNullOrEmpty(customMessage))
        {
				checker.CustomMessage = customMessage;
            }
			checker.DoCheck();
		}

		public virtual ISet<Type> GetDependencies(PXCache sender)
		{
			var result = new HashSet<Type>();
			Type _CacheType = sender.GetItemType();
			if ((_CacheType == _Type || _CacheType.IsSubclassOf(_Type)) && _DescriptionField != null)
			{
				result.Add(_DescriptionField);
			}

			return result;
		}

		#endregion

        #region Implementation
        /// <exclude/>
		public virtual void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (e.NewValue == null || !ValidateValue || _BypassFieldVerifying.Value) {
				return;
			}

			PXView view = GetView(sender, _PrimarySelect, !_DirtyRead);
			if (sender.Keys.Count == 0 || _FieldName != sender.Keys[sender.Keys.Count - 1]) {
				object[] pars = new object[_ParsCount + 1];
				pars[pars.Length - 1] = e.NewValue;
				object item = null;
				try {
					item = SelectSingleBound(view, new object[] { e.Row }, pars);
				}
				catch (FormatException) {} // thrown by SqlServer
				catch (InvalidCastException) {} // thrown by MySql

				if (item == null) {
					if (_SubstituteKey != null) {
						if (e.ExternalCall) {
							object incoming = sender.GetValuePending(e.Row, _FieldName);
							if (incoming != null) {
								e.NewValue = incoming;
							}
						}
						else if (object.Equals(e.NewValue, sender.GetValue(e.Row, _FieldOrdinal))) {
							try {
								object incoming = sender.GetValueExt(e.Row, _FieldName);
								if (incoming is PXFieldState) {
									e.NewValue = ((PXFieldState)incoming).Value;
								}
								if (incoming != null) {
									e.NewValue = incoming;
								}
							}
							catch {
							}
						}
					}
					throwNoItem(hasRestrictedAccess(sender, _PrimarySelect, e.Row), e.ExternalCall, e.NewValue);
				}
			}
		}

		protected string[] hasRestrictedAccess(PXCache sender, BqlCommand command, object row)
		{
			List<string> descr = new List<string>();
			foreach (IBqlParameter par in command.GetParameters()) {
				if (par.MaskedType != null) {
					Type ft = par.GetReferencedType();
					if (ft.IsNested) {
						Type ct = ft.DeclaringType;
						PXCache cache = sender.Graph.Caches[ct];
						object val = null;
						bool currfound = false;
						if (row != null && (row.GetType() == ct || row.GetType().IsSubclassOf(ct))) {
							val = cache.GetValue(row, ft.Name);
							currfound = true;
						}
						if (!currfound && val == null && cache.Current != null) {
							val = cache.GetValue(cache.Current, ft.Name);
						}
						if (val == null && par.TryDefault) {
							if (cache.RaiseFieldDefaulting(ft.Name, null, out val)) {
								cache.RaiseFieldUpdating(ft.Name, null, ref val);
							}
						}

						if (val != null) {
							descr.Add(string.Format("{0}={1}", char.ToUpper(ft.Name[0]) + ft.Name.Substring(1), val.ToString()));
						}
					}
				}
			}

			return (descr.Count > 0) ? descr.ToArray() : null;
		}

		protected void throwNoItem(string[] restricted, bool external, object value)
		{
			PXTrace.WriteInformation("The item {0} is not found (restricted:{1},external:{2},value:{3})",
				this.FieldName, restricted != null ? string.Join(",", restricted) : false.ToString(), external, value);

			if (restricted == null) {
				if (external || value == null) {
					throw new PXSetPropertyException(PXMessages.LocalizeFormat(ErrorMessages.ElementDoesntExist, _FieldName));
				}
				else {
					throw new PXSetPropertyException(PXMessages.LocalizeFormat(ErrorMessages.ValueDoesntExist, _FieldName, value));
				}
			}
			else {
				if (external || value == null) {
					throw new PXSetPropertyException(PXMessages.LocalizeFormat(ErrorMessages.ElementDoesntExistOrNoRights, _FieldName));
				}
				else {
					throw new PXSetPropertyException(PXMessages.LocalizeFormat(ErrorMessages.ValueDoesntExistOrNoRights, _FieldName, value));
				}
			}
		}
        /// <exclude/>
		public virtual void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			bool deleted = false;
			if (_SubstituteKey == null && e.ReturnValue != null && IsReadDeletedSupported && sender.Graph.GetType() != typeof(PXGraph) &&
				(!_BqlTable.IsAssignableFrom(_BqlType) || sender.Keys.Count == 0 || String.Compare(sender.Keys[sender.Keys.Count - 1], _FieldName, StringComparison.OrdinalIgnoreCase) != 0)) {
				object key = e.ReturnValue;
				GlobalDictionary dict = null;
				object item = null;
				if (_CacheGlobal) {
					dict = PXContext.GetSlot<GlobalDictionary>(_GetSlotName(_Type));
					if (dict == null) {
						PXContext.SetSlot<GlobalDictionary>(_Type.FullName, (dict = PXDatabase.GetSlot<GlobalDictionary>(_GetSlotName(_Type), _BqlType)));
					}
					lock ((dict).SyncRoot) {
						KeyValuePair<object, bool> pair;
						if (dict.TryGetValue(key, out pair)) {
							item = pair.Key;
							deleted = pair.Value;
						}
					}
				}
				if (item == null)
				{
					PXView select = GetView(sender, _PrimarySelect, !_DirtyRead);
					object[] pars = new object[_ParsCount + 1];
					pars[pars.Length - 1] = key;
					item = SelectSingleBound(select, new object[] { e.Row }, pars);
					if (item == null)
					{
						using (PXReadDeletedScope rds = new PXReadDeletedScope())
						{
							item = SelectSingleBound(select, new object[] { e.Row }, pars);
							deleted = (item != null);
						}
					}
					cacheOnReadItem(sender, select.Cache, dict, item, deleted, key, ((IBqlSearch)_Select).GetField().Name);
				}
			}
			if (_AttributeLevel == PXAttributeLevel.Item || e.IsAltered) {
				if (_HeaderList == null) {
					populateFields(sender, PXContext.GetSlot<bool>(selectorBypassInit));
				}
				e.ReturnState = PXFieldState.CreateInstance(e.ReturnState, null, null, null, null, null, null, null, _FieldName, _DescriptionField != null ? _DescriptionField.Name : null, null, deleted ? ErrorMessages.ForeignRecordDeleted : null, deleted ? PXErrorLevel.Warning : PXErrorLevel.Undefined, null, null, null, PXUIVisibility.Undefined, _ViewName, _FieldList, _HeaderList);
				((PXFieldState)e.ReturnState).ValueField = _SubstituteKey == null ? ValueField.Name : _SubstituteKey.Name;
				((PXFieldState)e.ReturnState).SelectorMode = SelectorMode;
			}
			else if (deleted) {
				e.ReturnState = sender.GetStateExt(e.Row, _FieldName);
			}
			//if (_HeaderList == null || _HeaderList.Length == 0 || _FieldList == null || _FieldList.Length == 0)
			//{
			//	PXFirstChanceExceptionLogger.LogMessage("Empty selector columns detected " +
			//		(_HeaderList == null ? "headers are null " : (_HeaderList.Length == 0 ? "headers are empty " : "")) +
			//		(_FieldList == null ? "fields are null " : (_FieldList.Length == 0 ? "fields are empty " : "")) +
			//		"for the field " + _FieldName +
			//		" flag state " + PXContext.GetSlot<bool>(selectorBypassInit).ToString());
			//}
		}


		protected virtual bool IsReadDeletedSupported
		{
			get { return PXDatabase.IsReadDeletedSupported(_BqlType); }
		}

        /// <exclude/>
		public virtual void DescriptionFieldSelecting(PXCache sender, PXFieldSelectingEventArgs e, string alias)
		{
			bool deleted = false;
			if (e.Row != null) {
				object key = sender.GetValue(e.Row, _FieldOrdinal);
				if (key != null) {
					object item = null;
					GlobalDictionary dict = null;
					if (_CacheGlobal) {
						dict = PXContext.GetSlot<GlobalDictionary>(_GetSlotName(_Type));
						if (dict == null) {
							PXContext.SetSlot<GlobalDictionary>(_Type.FullName, (dict = PXDatabase.GetSlot<GlobalDictionary>(_GetSlotName(_Type), _BqlType)));
						}
						lock ((dict).SyncRoot) {
							KeyValuePair<object, bool> pair;
							if (dict.TryGetValue(key, out pair)) {
								item = pair.Key;
								deleted = pair.Value;
							}
						}
					}
					if (item == null)
					{
						PXCache itemCache = sender;
						readItem(itemCache, e.Row, key, out itemCache, out item, ref deleted);
						cacheOnReadItem(sender, itemCache, dict, item, deleted, key, ((IBqlSearch)_Select).GetField().Name);
					}
					if (item != null)
					{
						PXCache itemCache = sender.Graph.Caches[_Type];
						e.ReturnValue = itemCache.GetValue(item, _DescriptionField.Name);
						if (e.ReturnValue == null)
						{
							readItem(itemCache, e.Row, key, out itemCache, out item, ref deleted);
							cacheOnReadItem(sender, itemCache, dict, item, deleted, key, ((IBqlSearch)_Select).GetField().Name);
							if (item != null)
								e.ReturnValue = itemCache.GetValue(item, _DescriptionField.Name);
						}
					}
				}
			}
			if (e.Row == null || e.IsAltered || deleted) {
				int? length;
				string displayname = getDescriptionName(sender, out length);
				if (_UIFieldRef != null &&_UIFieldRef.UIFieldAttribute == null)
				{
					_UIFieldRef.UIFieldAttribute = sender.GetAttributes(FieldName)
												   .OfType<PXUIFieldAttribute>()
												   .FirstOrDefault();
				}
				bool isVisible = true;
				PXUIVisibility visibility = PXUIVisibility.Visible;
				if (_UIFieldRef != null && _UIFieldRef.UIFieldAttribute != null)
				{
					isVisible = _UIFieldRef.UIFieldAttribute.Visible;
					visibility = _UIFieldRef.UIFieldAttribute.Visibility;
					if (((visibility & PXUIVisibility.SelectorVisible) == PXUIVisibility.SelectorVisible)
						&& (!sender.Keys.Contains(_FieldName) || !String.Equals(alias, _UIFieldRef.UIFieldAttribute.FieldName + "_description", StringComparison.OrdinalIgnoreCase)))
					{
						visibility = PXUIVisibility.Visible;
					}
				}
				e.ReturnState = PXFieldState.CreateInstance(e.ReturnState, typeof(string), false, true, null, null, length, null,
					alias//_FieldName + "_" + _Type.Name + "_" + _DescriptionField.Name
					, null, displayname, deleted ? ErrorMessages.ForeignRecordDeleted : null, deleted ? PXErrorLevel.Warning : PXErrorLevel.Undefined, false, isVisible, null, visibility, null, null, null);
			}
		}

		protected void readItem(PXCache sender, object row, object p, out PXCache itemCache, out object item, ref bool deleted)
		{
			PXView substituteView = null;
			item = null;
			if (_UnconditionalSelect.GetTables()[0] == _PrimarySelect.GetTables()[0])
			{
				object[] pars = new object[_ParsCount + 1];
				pars[pars.Length - 1] = p;
				substituteView = GetView(sender, _PrimarySelect, !_DirtyRead);
				using (new PXReadBranchRestrictedScope())
				{
					try
					{
						item = SelectSingleBound(substituteView, new object[] { row }, pars);
						if (item == null && IsReadDeletedSupported)
						{
							using (PXReadDeletedScope rds = new PXReadDeletedScope())
							{
								item = SelectSingleBound(substituteView, new object[] { row }, pars);
								deleted = (item != null);
							}
						}
					}
					catch
					{
					}
				}
			}
			if (item == null)
			{
				substituteView = GetView(sender, _UnconditionalSelect, !_DirtyRead);
				using (new PXReadBranchRestrictedScope())
				{
					try
					{
						item = SelectSingleBound(substituteView, new object[] { row }, p);
						if (item == null && IsReadDeletedSupported)
						{
							using (PXReadDeletedScope rds = new PXReadDeletedScope())
							{
								item = SelectSingleBound(substituteView, new object[] { row }, p);
								deleted = (item != null);
							}
						}
					}
					catch (FormatException) // thrown by MS SQL
					{
					}
					catch (InvalidCastException) // thrown by MySQL
					{
					}
				}
			}

			itemCache = substituteView?.Cache ?? sender;
		}

	    private void cacheOnReadItem(PXCache sender, PXCache cache, GlobalDictionary dict, object item, bool deleted, object key, string referenceField)
	    {
			if (item != null && dict != null && _CacheGlobal && cache.GetStatus(item) == PXEntryStatus.Notchanged && !PXDatabase.ReadDeleted && cache.Keys.Count <= 1)
			{
				CheckIntegrityAndPutGlobal(sender, cache, referenceField, dict, item, deleted, key, _FieldName);
			}
		}

		protected static Dictionary<Type, KeyValuePair<KeyValuePair<string, int?>, bool?>> _substitutekeys = new Dictionary<Type, KeyValuePair<KeyValuePair<string, int?>, bool?>>();
		protected string getSubstituteKeyMask(PXCache sender, out int? length, out bool? isUnicode)
		{
			length = null;
			isUnicode = null;
			string mask = null;
			if (_SubstituteKey != null) {
				KeyValuePair<KeyValuePair<string, int?>, bool?> pair;
				bool hasValue;
				lock (((ICollection) _substitutekeys).SyncRoot)
				{
					hasValue = _substitutekeys.TryGetValue(_SubstituteKey, out pair);
				}
				if (!hasValue) {
					PXCache cache = sender.Graph._GetReadonlyCache(_Type);
					foreach (PXEventSubscriberAttribute attr in cache.GetAttributes(_SubstituteKey.Name)) {
						if (attr is PXDBStringAttribute) {
							length = ((PXDBStringAttribute)attr).Length;
							isUnicode = ((PXDBStringAttribute)attr).IsUnicode;
							mask = ((PXDBStringAttribute)attr).InputMask;
						}
						else if (attr is PXStringAttribute) {
							length = ((PXStringAttribute)attr).Length;
							isUnicode = ((PXStringAttribute)attr).IsUnicode;
							mask = ((PXStringAttribute)attr).InputMask;
						}
						if (mask != null && length != null) {
							break;
						}
					}
					if (cache.BqlTable.IsAssignableFrom(_Type)) {
						lock (((ICollection)_substitutekeys).SyncRoot) {
							_substitutekeys[_SubstituteKey] = new KeyValuePair<KeyValuePair<string, int?>, bool?>(new KeyValuePair<string, int?>(mask, length), isUnicode);
						}
					}
				}
				else {
					mask = pair.Key.Key;
					length = pair.Key.Value;
					isUnicode = pair.Value;
				}
			}
			return mask;
		}

        protected string getDescriptionName(PXCache sender, out int? length)
		{
			length = null;
			string displayname = null;
			KeyValuePair<string, int?> pair;
			Dictionary<string, KeyValuePair<string, int?>> descriptions = PXContext.GetSlot<Dictionary<string, KeyValuePair<string, int?>>>("_DescriptionFieldFullName$" + System.Threading.Thread.CurrentThread.CurrentCulture.Name);
			if (descriptions == null) {
				PXContext.SetSlot("_DescriptionFieldFullName$" + System.Threading.Thread.CurrentThread.CurrentCulture.Name, 
                    descriptions = PXDatabase.GetSlot<Dictionary<string, KeyValuePair<string, int?>>>("_DescriptionFieldFullName$" + System.Threading.Thread.CurrentThread.CurrentCulture.Name, _BqlType));
			}
            var key = sender.Graph.GetType().FullName + "$" + _DescriptionField.FullName;
            var found = false;
            lock (((ICollection)descriptions).SyncRoot)
            {
                found = descriptions.TryGetValue(key, out pair);
            }
            if (!found) {
				PXCache cache = sender.Graph._GetReadonlyCache(_Type);
				foreach (PXEventSubscriberAttribute attr in cache.GetAttributes(_DescriptionField.Name)) {
					if (attr is PXUIFieldAttribute) {
						displayname = ((PXUIFieldAttribute)attr).DisplayName;
					}
					else if (attr is PXDBStringAttribute) {
						length = ((PXDBStringAttribute)attr).Length;
					}
					else if (attr is PXStringAttribute) {
						length = ((PXStringAttribute)attr).Length;
					}
					if (displayname != null && length != null) {
						break;
					}
				}
				if (displayname == null) {
					displayname = _DescriptionField.Name;
				}
				if (cache.BqlTable.IsAssignableFrom(_Type)) {
					lock (((ICollection)descriptions).SyncRoot) {
						descriptions[key] = new KeyValuePair<string, int?>(displayname, length);
					}
				}
			}
			else {
				displayname = pair.Key;
				length = pair.Value;
			}
			if (_FieldList != null && _HeaderList != null && _FieldList.Length == _HeaderList.Length) {
				for (int i = 0; i < _FieldList.Length; i++) {
					if (_FieldList[i] == _DescriptionField.Name)
						return _HeaderList[i];
				}
			}
			return displayname;
		}

		protected static string _GetSlotName(Type type)
		{
			if (!PXDBLocalizableStringAttribute.IsEnabled)
			{
				return type.FullName;
			}
			string language = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName ?? "";
			return type.FullName + language.ToUpper();
		}
		/// <exclude/>
		public virtual void SelfRowSelecting(PXCache sender, PXRowSelectingEventArgs e)
		{
			if (sender.RowId == null || sender.Keys.Count > 1 || sender._AggregateSelecting || sender._SingleTableSelecting) return;

			var dict = PXContext.GetSlot<GlobalDictionary>(_GetSlotName(_Type));
			if (dict == null)
			{
				PXContext.SetSlot<GlobalDictionary>(_Type.FullName, (dict = PXDatabase.GetSlot<GlobalDictionary>(_GetSlotName(_Type), _BqlType)));
			}
			lock ((dict).SyncRoot)
			{
				object key = sender.GetValue(e.Row, sender.RowId);
				KeyValuePair<object, bool> pair;
				if (key != null && !dict.TryGetValue(key, out pair))
				{
					object row;
					if (sender.Graph.GetType() == typeof(PXGenericInqGrph) || PXView.CurrentRestrictedFields.Any())
					{
						var values = new Dictionary<string, object>();
						values.Add(_FieldName, sender.GetValue(e.Row, _FieldName));
						if (_DescriptionField != null)
							values.Add(_DescriptionField.Name, sender.GetValue(e.Row, _DescriptionField.Name));
						row = values;
					}
					else
					{
						row = e.Row;
					}
					dict.Set(key, row, false);
					//dict[key] = new KeyValuePair<object, bool>(row, false);
				}
			}
		}	
        /// <exclude/>
		public virtual void SubstituteKeyFieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			bool deleted = false;
			if (e.ReturnValue != null) {
				object item = null;
				GlobalDictionary dict = null;
				if (_CacheGlobal) {
					dict = PXContext.GetSlot<GlobalDictionary>(_GetSlotName(_Type));
					if (dict == null) {
						PXContext.SetSlot<GlobalDictionary>(_Type.FullName, (dict = PXDatabase.GetSlot<GlobalDictionary>(_GetSlotName(_Type), _BqlType)));
					}
					lock ((dict).SyncRoot) {
						KeyValuePair<object, bool> pair;
						if (dict.TryGetValue(e.ReturnValue, out pair)) {
							item = pair.Key;
							deleted = pair.Value;
						}
					}
				}
				if (item == null) {
					if (e.ReturnValue == null || e.ReturnValue.GetType() == sender.GetFieldType(_FieldName)) {
						PXCache itemCache = sender;
						readItem(itemCache, e.Row, e.ReturnValue, out itemCache, out item, ref deleted);
						if (item != null) {
							if (_SubstituteKey != null) {
								object ret = itemCache.GetValue(item, _SubstituteKey.Name);
								if (e.ReturnValue != null)
								{
									cacheOnReadItem(sender, itemCache, dict, item, deleted, ret, _SubstituteKey.Name);
									cacheOnReadItem(sender, itemCache, dict, item, deleted, e.ReturnValue, ((IBqlSearch)_Select).GetField().Name);
								}
								e.ReturnValue = ret;
							}
						}
					}
				}
				else {
					PXCache itemCache = sender.Graph.Caches[_Type];
					object p = e.ReturnValue;
					e.ReturnValue = itemCache.GetValue(item, _SubstituteKey.Name);
					if (e.ReturnValue == null)
					{
						readItem(itemCache, e.Row, p, out itemCache, out item, ref deleted);
						if (item != null) {
							e.ReturnValue = itemCache.GetValue(item, _SubstituteKey.Name);
							if (e.ReturnValue != null)
							{
								cacheOnReadItem(sender, itemCache, dict, item, deleted, e.ReturnValue, _SubstituteKey.Name);
								cacheOnReadItem(sender, itemCache, dict, item, deleted, p, ((IBqlSearch)_Select).GetField().Name);
							}
						}
					}
				}
			}
			if (!e.IsAltered) {
				e.IsAltered = deleted || sender.HasAttributes(e.Row);
			}
			if (_AttributeLevel == PXAttributeLevel.Item || e.IsAltered) {
				int? length;
				bool? isUnicode;
				string mask = getSubstituteKeyMask(sender, out length, out isUnicode);
				e.ReturnState = PXStringState.CreateInstance(e.ReturnState, length, null, _FieldName, null, null, mask, null, null, null, null);
				if (e.ReturnValue != null && e.ReturnValue.GetType() != typeof(string))
				{
					e.ReturnValue = e.ReturnValue.ToString();
				}
				if (deleted) {
					((PXFieldState)e.ReturnState).Error = ErrorMessages.ForeignRecordDeleted;
					((PXFieldState)e.ReturnState).ErrorLevel = PXErrorLevel.Warning;
					((PXFieldState)e.ReturnState).SelectorMode = SelectorMode;
				}
			}

			//if (e.ReturnState is PXFieldState )
			//{
			//    var returnState = (PXFieldState)e.ReturnState;
			//    returnState.ValueField = _SubstituteKey;
			//}
		}
        protected internal ObjectRef<bool> _BypassFieldVerifying;
        /// <exclude/>
		public virtual void SubstituteKeyFieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {
	        e.NewValue = PXMacroManager.Default.TryResolveExt(e.NewValue, sender, FieldName, e.Row);
			if (!e.Cancel && e.NewValue != null) {
				object item = null;
				GlobalDictionary dict = null;
				if (_CacheGlobal) {
					dict = PXContext.GetSlot<GlobalDictionary>(_GetSlotName(_Type));
					if (dict == null) {
						PXContext.SetSlot<GlobalDictionary>(_Type.FullName, (dict = PXDatabase.GetSlot<GlobalDictionary>(_GetSlotName(_Type), _BqlType)));
					}
					lock ((dict).SyncRoot) {
						KeyValuePair<object, bool> pair;
						if (dict.TryGetValue(e.NewValue, out pair)) {
							if (pair.Value && !PXDatabase.ReadDeleted) {
								throw new PXForeignRecordDeletedException();
							}
							item = pair.Key;
						}
					}
				}
				if (item == null) {
					PXView select = GetView(sender, _NaturalSelect, !_DirtyRead);
					bool bypass = e.NewValue.GetType() != select.Cache.GetFieldType(_SubstituteKey.Name) && e.NewValue.GetType() == sender.GetFieldType(_FieldName);
					if (!bypass) {
					if (!_CacheGlobal) {
						object[] pars = new object[_ParsCount + 1];
						pars[pars.Length - 1] = e.NewValue;
						item = SelectSingleBound(select, new object[] { e.Row }, pars);
					}
					else {
						item = SelectSingleBound(select, new object[] { e.Row }, e.NewValue);
					}
					}
					if (item != null) {
						object ret = select.Cache.GetValue(item, ((IBqlSearch)_Select).GetField().Name);
						cacheOnReadItem(sender, select.Cache, dict, item, false, e.NewValue, _SubstituteKey.Name);
						cacheOnReadItem(sender, select.Cache, dict, item, false, ret, ((IBqlSearch)_Select).GetField().Name);
						e.NewValue = ret;
					}
					else {
						using (new PXReadBranchRestrictedScope()) {
							if (!bypass) {
							if (!_CacheGlobal) {
								object[] pars = new object[_ParsCount + 1];
								pars[pars.Length - 1] = e.NewValue;
								item = SelectSingleBound(select, new object[] { e.Row }, pars);
							}
							else {
								item = SelectSingleBound(select, new object[] { e.Row }, e.NewValue);
							}
							if (item == null && IsReadDeletedSupported) {
								using (PXReadDeletedScope rds = new PXReadDeletedScope()) {
									if (!_CacheGlobal) {
										object[] pars = new object[_ParsCount + 1];
										pars[pars.Length - 1] = e.NewValue;
										item = SelectSingleBound(select, new object[] { e.Row }, pars);
									}
									else {
										item = SelectSingleBound(select, new object[] { e.Row }, e.NewValue);
									}
									if (item != null) {
										object ret = select.Cache.GetValue(item, ((IBqlSearch)_Select).GetField().Name);
										cacheOnReadItem(sender, select.Cache, dict, item, false, e.NewValue, _SubstituteKey.Name);
										cacheOnReadItem(sender, select.Cache, dict, item, false, ret, ((IBqlSearch)_Select).GetField().Name);
										throw new PXForeignRecordDeletedException();
									}
								}
							}
							}
							if (e.NewValue.GetType() == sender.GetFieldType(_FieldName)) {
								PXView view = GetView(sender, _PrimarySelect, !_DirtyRead);
								object[] pars = new object[_ParsCount + 1];
								pars[pars.Length - 1] = e.NewValue;
								item = null;
								try {
									item = SelectSingleBound(view, new object[] { e.Row }, pars);
								}
								catch (FormatException) {
								}
								if (item != null) {
									return;
								}
							}
							_BypassFieldVerifying.Value = true;
							try {
								object val = e.NewValue;
								sender.OnFieldVerifying(_FieldName, e.Row, ref val, true);

								if (val != null && val.GetType() == sender.GetFieldType(_FieldName)) {
									e.NewValue = val;
									return;
								}
							}
							catch (Exception ex) {
								if (ex is PXSetPropertyException) {
									throw PXException.PreserveStack(ex);
								}
							}
							finally {
								_BypassFieldVerifying.Value = false;
							}
							string[] restricted = (item != null) ? new string[] { true.ToString() } : hasRestrictedAccess(sender, _NaturalSelect, e.Row);
							throwNoItem(restricted, true, e.NewValue);
						}
					}
				}
				else {
					PXCache cache = sender.Graph.Caches[_Type];
					object p = e.NewValue;
					e.NewValue = cache.GetValue(item, ((IBqlSearch)_Select).GetField().Name);
					if (e.NewValue == null) {
						PXView select = GetView(sender, _NaturalSelect, !_DirtyRead);
						item = SelectSingleBound(select, new object[] { e.Row }, p);
						if (item != null) {
							e.NewValue = select.Cache.GetValue(item, ((IBqlSearch)_Select).GetField().Name);
						}
						else {
							throwNoItem(hasRestrictedAccess(sender, _NaturalSelect, e.Row), true, p);
						}
					}
				}
			}
		}
        /// <exclude/>
		public virtual void SubstituteKeyCommandPreparing(PXCache sender, PXCommandPreparingEventArgs e)
		{
			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Select &&
				(e.Operation & PXDBOperation.Option) == PXDBOperation.External &&
				(e.Value == null || e.Value is string)) {
				e.Cancel = true;
				foreach (PXEventSubscriberAttribute attr in sender.GetAttributes(_FieldName)) {
					if (attr is PXDBFieldAttribute) {
						e.BqlTable = _BqlTable;
						string extTable = _Type.Name + "Ext";
						e.FieldName = BqlCommand.SubSelect + _SubstituteKey.Name + " FROM " + BqlCommand.GetTableName(_Type, sender.Graph) + " " + extTable + " "
							+ "WHERE " + e.SqlDialect.quoteTableAndColumn(extTable, ((IBqlSearch)_Select).GetField().Name) + " = " 
							+  e.SqlDialect.quoteTableAndColumn(e.Table == null ? _BqlTable.Name : e.Table.Name, sender.BqlSelect == null ? ((PXDBFieldAttribute)attr).DatabaseFieldName : _FieldName)
							+ ")";
						if (e.Value != null) {
							e.DataValue = e.Value;
							e.DataType = PXDbType.NVarChar;
							e.DataLength = ((string)e.Value).Length;
						}
						break;
					}
				}
			}
		}
        /// <exclude/>
		public virtual void DescriptionFieldCommandPreparing(PXCache sender, PXCommandPreparingEventArgs e)
		{
			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Select &&
				(e.Operation & PXDBOperation.Option) == PXDBOperation.External &&
				(e.Value == null || e.Value is string))
			{
				e.Cancel = true;
				foreach (PXEventSubscriberAttribute attr in sender.GetAttributes(_FieldName))
				{
					if (attr is PXDBFieldAttribute)
					{
						e.BqlTable = _BqlTable;
						string extTable = e.SqlDialect.quoteDbIdentifier(_Type.Name + "Ext");
						string descrFieldName = _DescriptionField.Name;
						string tableName = BqlCommand.GetTableName(_Type, sender.Graph);
						if (!String.IsNullOrEmpty(tableName) && !tableName.Trim().StartsWith(BqlCommand.SubSelect)) // do not expand description field to subselect in case when description field is being selected from another expanded subselect
						{
							PXCommandPreparingEventArgs.FieldDescription descrFieldDescription;
							Type descrTable = BqlCommand.GetItemType(_DescriptionField);
							PXCache descrCache = sender.Graph.Caches[descrTable];
							PXDBOperation operation = PXDBOperation.Select;
							if (PXDBLocalizableStringAttribute.IsEnabled && descrCache.GetAttributes(_DescriptionField.Name).Any(_ => _ is PXDBLocalizableStringAttribute))
							{
								operation |= PXDBOperation.External;
							}
							descrCache.RaiseCommandPreparing(_DescriptionField.Name, null, null, operation, descrTable, out descrFieldDescription);
							if (descrFieldDescription != null && !String.IsNullOrEmpty(descrFieldDescription.FieldName))
							{
								descrFieldName = descrFieldDescription.FieldName
									.Replace(e.SqlDialect.quoteDbIdentifier(_Type.Name), extTable)
									.Replace(e.SqlDialect.quoteDbIdentifier(descrTable.Name), extTable);
							}
						}
						
						e.FieldName = BqlCommand.SubSelect + descrFieldName + " FROM " + tableName + " " + extTable + " "
							+ "WHERE " + e.SqlDialect.quoteTableAndColumn(extTable, ((IBqlSearch)_Select).GetField().Name) + " = "
							+ e.SqlDialect.quoteTableAndColumn(e.Table == null ? _BqlTable.Name : e.Table.Name, sender.BqlSelect == null ? ((PXDBFieldAttribute)attr).DatabaseFieldName : _FieldName)
							+ ")";
						if (e.Value != null)
						{
							e.DataValue = e.Value;
							e.DataType = PXDbType.NVarChar;
							e.DataLength = ((string)e.Value).Length;
						}
						break;
					}
				}
			}
		}
        /// <exclude/>
		public virtual void ForeignTableRowPersisted(PXCache sender, PXRowPersistedEventArgs e)
		{
			if (e.TranStatus == PXTranStatus.Completed) {
				ClearGlobalCache(_Type);
			}
		}
        /// <exclude/>
		public virtual void ReadDeletedFieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (e.NewValue == null ||
				!ValidateValue ||
				_BqlTable.IsAssignableFrom(_BqlType) && sender.Keys.Count > 0 && String.Compare(sender.Keys[sender.Keys.Count - 1], _FieldName, StringComparison.OrdinalIgnoreCase) == 0) {
				return;
			}
			GlobalDictionary dict = null;
			object key = e.NewValue;
			if (_CacheGlobal) {
				dict = PXContext.GetSlot<GlobalDictionary>(_GetSlotName(_Type));
				if (dict == null) {
					PXContext.SetSlot<GlobalDictionary>(_Type.FullName, (dict = PXDatabase.GetSlot<GlobalDictionary>(_GetSlotName(_Type), _BqlType)));
				}
				lock ((dict).SyncRoot) {
					KeyValuePair<object, bool> pair;
					if (dict.TryGetValue(key, out pair)) {
						if (pair.Value && !PXDatabase.ReadDeleted) {
							throw new PXForeignRecordDeletedException();
						}
						else {
							return;
						}
					}
				}
			}
			PXView select = GetView(sender, _PrimarySelect, !_DirtyRead);
			object[] pars = new object[_ParsCount + 1];
			pars[pars.Length - 1] = key;
			bool deleted = false;
			object item = SelectSingleBound(select, new object[] { e.Row }, pars);
			if (item == null) {
				using (PXReadDeletedScope rds = new PXReadDeletedScope()) {
					item = SelectSingleBound(select, new object[] { e.Row }, pars);
					deleted = true;
				}
			}
			if (item != null) {
				if (select.Cache.GetStatus(item) == PXEntryStatus.Notchanged) {
					if (dict != null && !PXDatabase.ReadDeleted && select.Cache.Keys.Count <= 1) {
						CheckIntegrityAndPutGlobal(sender, select.Cache, ((IBqlSearch)_Select).GetField().Name, dict, item, deleted, key, _FieldName);
					}
					if (deleted) {
						throw new PXForeignRecordDeletedException();
					}
				}
			}
		}
		#endregion

		#region Initialization
		protected static Dictionary<Type, List<KeyValuePair<string, Type>>> _SelectorFields = new Dictionary<Type, List<KeyValuePair<string, Type>>>();
		protected internal override void SetBqlTable(Type bqlTable)
		{
			base.SetBqlTable(bqlTable);
			lock (((ICollection)_SelectorFields).SyncRoot) {
				List<KeyValuePair<string, Type>> list;
				if (!_SelectorFields.TryGetValue(bqlTable, out list)) {
					_SelectorFields[bqlTable] = list = new List<KeyValuePair<string, Type>>();
				}
				bool found = false;
				foreach (KeyValuePair<string, Type> pair in list) {
					if (pair.Key == base.FieldName) {
						found = true;
						break;
					}
				}
				if (!found) {
					Type field = ((IBqlSearch)_Select).GetField();
					Type table = BqlCommand.GetItemType(field);
					if (table == null || !bqlTable.IsAssignableFrom(table) || !String.Equals(field.Name, base.FieldName, StringComparison.OrdinalIgnoreCase)) {
						list.Add(new KeyValuePair<string, Type>(base.FieldName, ((IBqlSearch)_Select).GetField()));
					}
				}
			}
		}
        /// <exclude/>
		public static List<KeyValuePair<string, Type>> GetSelectorFields(Type table)
		{
	        if (ServiceManager.EnsureCachesInstatiated(true))
	        {
		        lock (((ICollection) _SelectorFields).SyncRoot)
		        {
			        List<KeyValuePair<string, Type>> list;
			        if (_SelectorFields.TryGetValue(table, out list))
			        {
						HashSet<string> distinct = null;
						List<KeyValuePair<string, Type>> ret = list;
						while ((table = table.BaseType) != typeof(object))
						{
							List<KeyValuePair<string, Type>> toMerge;
							if (_SelectorFields.TryGetValue(table, out toMerge))
							{
								if (distinct == null)
								{
									distinct = new HashSet<string>(list.Select(_ => _.Key));
									ret = new List<KeyValuePair<string, Type>>(list);
								}
								int cnt = ret.Count;
								ret.AddRange(toMerge.Where(_ => !distinct.Contains(_.Key)));
								ret.GetRange(cnt, ret.Count - cnt).ForEach(_ => distinct.Add(_.Key));
							}
						}
						return ret;
			        }
		        }
	        }
	        return new List<KeyValuePair<string, Type>>();
		}

		protected internal const string selectorBypassInit = "selectorBypassInit";
		protected static object _tlock = new object();
		protected static Dictionary<string, string[]> _fields = new Dictionary<string, string[]>();
		protected static Dictionary<string, string[]> _headers = new Dictionary<string, string[]>();
		protected static object _flock = new object();
		protected static Dictionary<Type, string> _descriptions = new Dictionary<Type, string>();

		protected void populateFields(PXCache sender, bool bypassInit)
		{
			string key;
			if (_FieldList == null) {
				PXCache c;
				Type t = _Type;
				if ((c = sender.Graph.Caches[_Type.Name]) != null && c.GetItemType() != _Type
					|| c == null && (t = PXSubstManager.Substitute(_Type, sender.Graph.GetType())) != _Type)
				{
					if (c != null)
					{
						key = c.GetItemType().FullName + "$" + sender.Graph.GetType().FullName;
					}
					else
					{
						key = t.FullName + "$" + sender.Graph.GetType().FullName;
					}
				}
				else
				{
					key = _Type.FullName;
					if (sender.Graph.HasGraphSpecificFields(_Type))
					{
						key = key + "$" + sender.Graph.GetType().FullName;
					}
				}
			}
			else {
				key = sender.GetItemType().FullName + "$" + _FieldName;
				PXCache c;
				if (sender.IsGraphSpecificField(_FieldName)
					|| sender.Graph.HasGraphSpecificFields(_Type)
					|| (c = sender.Graph.Caches[_Type.Name]) != null && c.GetItemType() != _Type
					|| c == null && PXSubstManager.Substitute(_Type, sender.Graph.GetType()) != _Type)
				{
					key = key + "$" + sender.Graph.GetType().FullName;
				}
			}
			string culture = key + "@" + System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
			string[] headerList = null;
			lock (_flock) {
				if (_FieldList == null) {
					_fields.TryGetValue(key, out _FieldList);
				}
				if (_FieldList != null) {
					_headers.TryGetValue(culture + "$" + _FieldList.Length.ToString(CultureInfo.InvariantCulture), out headerList);
				}
			}
			if (_FieldList == null || headerList == null) {
				if (_FieldList != null && _HeaderList != null) {
					for (int i = 0; i < _HeaderList.Length; i++) {
						uint msgnum;
						string msgprefix;
						_HeaderList[i] = PXMessages.Localize(_HeaderList[i], out msgnum, out msgprefix);
					}
					lock (_flock) {
						_fields[key] = _FieldList;
						_headers[culture + "$" + _FieldList.Length.ToString(CultureInfo.InvariantCulture)] = _HeaderList;
					}
				}
				else if (!bypassInit) {
					_HeaderList = new string[0];
					List<string> fields = new List<string>();
					List<string> headers = new List<string>();
					PXCache cache = sender.GetItemType() == _Type || sender.GetItemType().IsSubclassOf(_Type) && !Attribute.IsDefined(sender.GetItemType(), typeof(PXBreakInheritanceAttribute), false) ? sender : sender.Graph._GetReadonlyCache(_Type);
					PXContext.SetSlot<bool>(selectorBypassInit, true);
					try
					{
						if (_FieldList == null)
						{
							foreach (string name in cache.Fields)
							{
								PXFieldState st = cache.GetStateExt(null, name) as PXFieldState;
								if (st != null &&
									((st.Visibility & PXUIVisibility.SelectorVisible) == PXUIVisibility.SelectorVisible ||
									(st.Visibility & PXUIVisibility.Dynamic) == PXUIVisibility.Dynamic))
								{
									fields.Add(st.Name);
									headers.Add(st.DisplayName);
								}
							}
						}
						else
						{
							for (int i = 0; i < _FieldList.Length; i++)
							{
								bool found = false;
								{
									PXFieldState st = cache.GetStateExt(null, _FieldList[i]) as PXFieldState;
									if (st != null)
									{
										fields.Add(_FieldList[i]);
										headers.Add(st.DisplayName);
										found = true;
									}
								}
								int idx;
								if (!found)
								{
									if ((idx = _FieldList[i].IndexOf("__")) > 0)
									{
										if (idx + 2 < _FieldList[i].Length)
										{
											string tname = _FieldList[i].Substring(0, idx);
											foreach (Type table in _Select.GetTables())
											{
												if (table.Name == tname)
												{
													string fname = _FieldList[i].Substring(idx + 2, _FieldList[i].Length - idx - 2);
													PXCache tcache = sender.Graph._GetReadonlyCache(table);
													{
														PXFieldState st = tcache.GetStateExt(null, fname) as PXFieldState;
														if (st != null)
														{
															fields.Add(_FieldList[i]);
															headers.Add(st.DisplayName);
														}
													}
													break;
												}
											}
										}
									}
									else if ((idx = _FieldList[i].IndexOf('_')) > 0)
									{
										string fname = _FieldList[i].Substring(0, idx);
										foreach (PXEventSubscriberAttribute attr in cache.GetAttributes(fname))
										{
											if (attr is PXSelectorAttribute)
											{
												fields.Add(attr.FieldName);
												int? length;
												headers.Add(((PXSelectorAttribute)attr).getDescriptionName(sender, out length));
												break;
											}
										}
									}
								}
							}
						}
					}
					catch (Exception)
					{
						_HeaderList = null;
                        #pragma warning disable CS0618 // Type or member is obsolete
                        PXFirstChanceExceptionLogger.LogMessage("Failed to retrieve selector columns");
                        #pragma warning restore CS0618 // Type or member is obsolete
                        throw;
					}
					finally {
						PXContext.SetSlot<bool>(selectorBypassInit, false);
					}
					_FieldList = fields.ToArray();
					_HeaderList = headers.ToArray();
					lock (_flock) {
						_fields[key] = _FieldList;
						_headers[culture + "$" + _FieldList.Length.ToString(CultureInfo.InvariantCulture)] = _HeaderList;
					}
				}
			}
			else {
				_HeaderList = headerList;
			}
		}

	    protected virtual PXFilterView createFilterView(PXGraph graph)
	    {
			return new PXFilterView(graph, "SELECTOR", "_" + (_FilterEntity ?? _Type).Name + "_");
	    }

        /// <exclude/>
		public void CreateView(PXCache sender)
		{
			_ViewName = string.Format("_{0}{1}{2}", sender.GetItemType().Name, _FieldName, _ViewName);
			PXView view;
			if (sender.Graph.Views.TryGetValue(_ViewName, out view)) {
				if (view.BqlSelect.GetType() != _Select.GetType()) {
                    // as we have field name in viewname we don`t need random viewnames
					//if (!_IsOwnView) {
					//	_ViewName = Guid.NewGuid().ToString();
					//}
					view = null;
				}
			}
			if (view == null) {
				if (_ViewHandler != null) {
					if (_CacheGlobal) view = new adjustableViewGlobal(sender.Graph, true, _Select, _ViewHandler, sender, _FieldName);
					else view = new PXAdjustableView(sender.Graph, true, _Select, _ViewHandler);
				}
				else {
					view = _CacheGlobal ? new viewGlobal(sender.Graph, true, _Select, sender, _FieldName) : new PXView(sender.Graph, true, _Select);
				}
				sender.Graph.Views[_ViewName] = view;
				_IsOwnView = true;
				if (_DirtyRead) {
					view.IsReadOnly = false;
				}
				if (_Filterable)
				{
					sender.Graph.Views[_ViewName + PXFilterableAttribute.FilterHeaderName] = createFilterView(sender.Graph);
					sender.Graph.Views[_ViewName + PXFilterableAttribute.FilterRowName] = new PXView(sender.Graph, false, new Select<FilterRow, Where<FilterRow.filterID, Equal<Required<FilterRow.filterID>>>>());
				}
			}
		}

        /// <exclude/>
		public override void CacheAttached(PXCache sender)
		{
			_UIFieldRef = new UIFieldRef();
			_CacheType = sender.GetItemType();
			_BypassFieldVerifying = new ObjectRef<bool>();
			if (_CacheGlobal && (_CacheType == _Type || _CacheType.IsSubclassOf(_Type))) {
				sender.RowPersisted += ForeignTableRowPersisted;
				GlobalDictionary dict = PXContext.GetSlot<GlobalDictionary>(_GetSlotName(_Type));
				if (dict == null) {
					PXContext.SetSlot<GlobalDictionary>(_Type.FullName, (dict = PXDatabase.GetSlot<GlobalDictionary>(_GetSlotName(_Type), _BqlType)));
				}
			}
			populateFields(sender, true);
			CreateView(sender);
			_ViewCreated = true;

			if (!(_CacheType == _Type || _CacheType.IsSubclassOf(_Type)) || !String.Equals(_FieldName, ((IBqlSearch)_Select).GetField().Name, StringComparison.OrdinalIgnoreCase)) {
				EmitColumnForDescriptionField(sender);
			}
			else {
				SelectorMode |= PXSelectorMode.NoAutocomplete;
			}

			if ((_CacheType == _Type || _CacheType.IsSubclassOf(_Type)) && String.Equals(_FieldName, ((IBqlSearch)_Select).GetField().Name, StringComparison.OrdinalIgnoreCase) && !(sender.Graph.GetType() == typeof(PXGraph)))
			{
				if (!sender.Graph.Views.ToArray().Any(pair => !string.Equals(pair.Key, _ViewName, StringComparison.OrdinalIgnoreCase) && pair.Value is viewGlobal && pair.Value.BqlSelect.GetFirstTable() == _Type))
				{
					sender.RowSelecting += SelfRowSelecting;
				}
			}

			if (_SubstituteKey != null) {
				string name = _FieldName.ToLower();
				sender.FieldSelectingEvents[name] += SubstituteKeyFieldSelecting;
				sender.FieldUpdatingEvents[name] += SubstituteKeyFieldUpdating;
				if (String.Compare(_SubstituteKey.Name, _FieldName, StringComparison.OrdinalIgnoreCase) != 0) {
					sender.CommandPreparingEvents[name] += SubstituteKeyCommandPreparing;
				}
			}
			else if (IsReadDeletedSupported) {
				sender.FieldVerifyingEvents[_FieldName.ToLower()] += ReadDeletedFieldVerifying;
				_CacheGlobal = true;
			}
		}

		protected void EmitColumnForDescriptionField(PXCache sender)
		{
			if (_DescriptionField == null) return;
			if (true) {
				string alias = _FieldName + "_" + _Type.Name + "_" + _DescriptionField.Name;
				if (!sender.Fields.Contains(alias)) {
					sender.Fields.Add(alias);
					sender.FieldSelectingEvents[alias.ToLower()] +=
						delegate(PXCache s, PXFieldSelectingEventArgs e) { DescriptionFieldSelecting(s, e, alias); };
					sender.CommandPreparingEvents[alias.ToLower()] += DescriptionFieldCommandPreparing;
				}
			}

			if (true) {
				string alias = _FieldName + "_description";
				if (!sender.Fields.Contains(alias)) {
					sender.Fields.Add(alias);
					sender.FieldSelectingEvents[alias.ToLower()] += //DescriptionFieldSelecting;
							delegate(PXCache s, PXFieldSelectingEventArgs e) { DescriptionFieldSelecting(s, e, alias); };
					sender.CommandPreparingEvents[alias.ToLower()] += DescriptionFieldCommandPreparing;

				}
			}

		}

		#endregion

		private sealed class adjustableViewGlobal : viewGlobal, IPXAdjustableView
		{
			public adjustableViewGlobal(PXGraph graph, bool isReadOnly, BqlCommand select, Delegate handler, PXCache sender, string fieldName)
				: base(graph, isReadOnly, select, handler, sender, fieldName)
			{
			}
		}

		private class viewGlobal : PXView
		{
			private BqlCommand _select;
			private PXCache _sender;
			private string _fieldName;
			private PXSearchColumn[] _sorts;
			public viewGlobal(PXGraph graph, bool isReadOnly, BqlCommand select, PXCache sender, string fieldName)
				: base(graph, isReadOnly, select)
			{
				_select = select;
				_sender = sender;
				_fieldName = fieldName;
			}
			public viewGlobal(PXGraph graph, bool isReadOnly, BqlCommand select, Delegate handler, PXCache sender, string fieldName)
				: base(graph, isReadOnly, select, handler)
			{
				_select = select;
				_sender = sender;
				_fieldName = fieldName;
			}

			protected override List<object> InvokeDelegate(object[] parameters)
			{
				_sorts = PXView._Executing.Peek().Sorts;
				return base.InvokeDelegate(parameters);
			}

			//protected override PXSearchColumn[] prepareSorts(string[] sortcolumns, bool[] descendings, object[] searches, int topCount, out bool needOverrideSort, out bool anySearch, ref bool resetTopCount)
			//{
			//	_sorts = base.prepareSorts(sortcolumns, descendings, searches, topCount, out needOverrideSort, out anySearch, ref resetTopCount);
			//	return _sorts;
			//}

			public override List<object> Select(object[] currents, object[] parameters, object[] searches, string[] sortcolumns, bool[] descendings, PXFilterRow[] filters, ref int startRow, int maximumRows, ref int totalRows)
			{
				List<object> ret = null;
				object key = null;
				GlobalDictionary dict = null;
				if (startRow == 0 && maximumRows == 1 && searches != null && searches.Length == 1 && searches[0] != null) {
					key = searches[0];
					KeyValuePair<object, bool> item;
					dict = PXContext.GetSlot<GlobalDictionary>(_GetSlotName(Cache.GetItemType()));
					if (dict == null) {
						PXContext.SetSlot<GlobalDictionary>(Cache.GetItemType().FullName, (dict = PXDatabase.GetSlot<GlobalDictionary>(_GetSlotName(Cache.GetItemType()), Cache.BqlTable)));
					}
					lock ((dict).SyncRoot) {
						if (dict.TryGetValue(searches[0], out item)) {
							ret = new List<object>();
							ret.Add(item.Key);
						}
					}
					if (ret == null) {
						ret = base.Select(currents, parameters, searches, sortcolumns, descendings, filters, ref startRow, maximumRows, ref totalRows);
						bool deleted = false;
						if ((ret == null || ret.Count == 0) && PXDatabase.IsReadDeletedSupported(Cache.BqlTable)) {
							using (PXReadDeletedScope rds = new PXReadDeletedScope()) {
								ret = base.Select(currents, parameters, searches, sortcolumns, descendings, filters, ref startRow, maximumRows, ref totalRows);
							}
							deleted = true;
						}
						PXSearchColumn col;
						if (ret != null && ret.Count == 1 && !PXDatabase.ReadDeleted && base.Cache.Keys.Count <= 1) {
							if (sortcolumns.Length == 1 && !String.IsNullOrEmpty(sortcolumns[0])
								&& (col = _sorts.Where(_ => String.Equals(_.Column, sortcolumns[0], StringComparison.OrdinalIgnoreCase)).FirstOrDefault()) != null
								&& col.SearchValue != null)
							{
								object row;
								CheckIntegrityAndPutGlobal(_sender, base.Cache, sortcolumns[0], dict, (row = ret[0] is PXResult ? ((PXResult)ret[0])[0] : ret[0]), deleted, col.SearchValue, _fieldName, true);
								if (col.SearchValue is string && key is string && !String.Equals(((string)col.SearchValue), (string)key) && String.Equals(((string)col.SearchValue).TrimEnd(), ((string)key).TrimEnd()))
								{
									lock ((dict).SyncRoot)
									{
										dict.Set(key, row, deleted);
										//dict[key] = new KeyValuePair<object, bool>(row, deleted);
									}
								}
							}
						}
					}
				}
				if (ret == null) {
					ret = base.Select(currents, parameters, searches, sortcolumns, descendings, filters, ref startRow, maximumRows, ref totalRows);
				}
				return ret;
			}
		}
	}
	#endregion

	#region PXCustomSelectorAttribute
    /// <summary>The base class for custom selector attributes. To create a
    /// custom selector attribute, derive a class from this class and implement
    /// the <tt>GetRecords()</tt> method.</summary>
    /// <remarks>
    /// You can also override the <tt>CacheAttached(...)</tt> method to
    /// add initialization logic and override the <tt>FieldVerifying(...)</tt>
    /// method to redefine the verification logic for the field value.
    /// </remarks>
    /// <example>
    /// <code>
    /// public class MyCustomSelector : PXCustomSelectorAttribute
    /// {
    ///     public MyCustomSelector(Type type)
    ///         : base(type) { }
    ///     
    ///     public virtual IEnumerable GetRecords()
    ///     {
    ///         ...
    ///     }
    /// }
    /// </code>
    /// </example>
	public class PXCustomSelectorAttribute : PXSelectorAttribute
	{
		readonly long hashCode;
		protected PXGraph _Graph;


		#region Ctor
        /// <summary>Initializes a new instance with the specified BQL query for
        /// selecting the data records to show to the user.</summary>
        /// <param name="type">A BQL query that defines the data set that is shown
        /// to the user along with the key field that is used as a value. Set to a
        /// field (type part of a DAC field) to select all data records from the
        /// referenced table. Set to a BQL command of <tt>Search</tt> type to
        /// specify a complex select statement.</param>
		public PXCustomSelectorAttribute(Type type)
			: base(type)
		{
			this.hashCode = DateTime.Now.Ticks;
		}

        /// <summary>Initializes a new instance that will use the specified BQL
        /// query to retrieve the data records to select from, and display the
        /// provided set of columns.</summary>
        /// <param name="type">A BQL query that defines the data set that is shown
        /// to the user along with the key field that is used as a value. Set to a
        /// field (type part of a DAC field) to select all data records from the
        /// referenced table. Set to a BQL command of <tt>Search</tt> type to
        /// specify a complex select statement.</param>
        /// <param name="fieldList">Fields to display in the control.</param>
		public PXCustomSelectorAttribute(Type type, params Type[] fieldList)
			: base(type, fieldList)
		{
			this.hashCode = DateTime.Now.Ticks;
		}

		protected sealed class FilteredView : PXView
		{
			private PXView _OuterView;
			private PXView _TemplateView;
			public FilteredView(PXView outerView, PXView templateView)
				: base(templateView.Graph, templateView.IsReadOnly, templateView.BqlSelect)
			{
				_OuterView = outerView;
				_TemplateView = templateView;
			}
			public override List<object> Select(object[] currents, object[] parameters, object[] searches, string[] sortcolumns, bool[] descendings, PXFilterRow[] filters, ref int startRow, int maximumRows, ref int totalRows)
			{
				if (parameters != null && parameters.Length > 0) {
					string[] names = _TemplateView.GetParameterNames();
					int idx;
					if (names.Length > 0 && !String.IsNullOrEmpty(names[names.Length - 1]) && (idx = names[names.Length - 1].LastIndexOf('.')) != -1 && idx + 1 < names[names.Length - 1].Length) {
						string field = names[names.Length - 1].Substring(idx + 1);
						object val = parameters[parameters.Length - 1];
						try {
							Cache.RaiseFieldSelecting(field, currents != null && currents.Length > 0 ? currents[0] : null, ref val, false);
							val = PXFieldState.UnwrapValue(val);
						}
						catch {
						}
						if (val == null) {
							val = parameters[parameters.Length - 1];
						}
						PXFilterRow filter = new PXFilterRow(field, PXCondition.EQ, val);
						if (filters == null || filters.Length == 0) {
							filters = new PXFilterRow[] { filter };
						}
						else {
							filters = filters.Append(filter).ToArray();
							if (filters.Length > 2) {
								filters[0].OpenBrackets++;
								filters[filters.Length - 2].CloseBrackets++;
							}
							filters[filters.Length - 2].OrOperator = false;
						}
						Array.Resize(ref parameters, parameters.Length - 1);
					}
				}
				return _OuterView.Select(currents, parameters, searches, sortcolumns, descendings, filters, ref startRow, maximumRows, ref totalRows);
			}
		}
		protected override PXView GetView(PXCache cache, BqlCommand select, bool isReadOnly)
		{
			return new FilteredView(cache.Graph.Views[_ViewName], base.GetView(cache, select, isReadOnly));
		}
		#endregion

		#region Implementation
        /// <summary>
        /// The handler of the <tt>FieldVerifying</tt> event.
        /// </summary>
        /// <param name="sender">The cache object that has raised the event.</param>
        /// <param name="e">The event arguments.</param>
		public override void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (!ValidateValue)
				return;

			if (e.NewValue == null) {
				return;
			}

			if (sender.Keys.Count == 0 || _FieldName != sender.Keys[sender.Keys.Count - 1]) {
				List<object> records = sender.Graph.Views[_ViewName].SelectMultiBound(new object[] { e.Row });
				PXCache cache = sender.Graph.Caches[BqlCommand.GetItemType(((IBqlSearch)_Select).GetField())];
				foreach (object rec in records) {
					object item = (rec is PXResult) ? ((PXResult)rec)[0] : rec;
					object val = cache.GetValue(item, ((IBqlSearch)_Select).GetField().Name);
					if (Equals(val, e.NewValue))
						return;
					if (val is Array && e.NewValue is Array
						&& ((Array)val).Length == ((Array)e.NewValue).Length)
					{
						bool meet = true;
						for (int i = 0; i < ((Array)val).Length; i++)
						{
							if (!(meet = meet && Equals(((Array)val).GetValue(i), ((Array)e.NewValue).GetValue(i))))
							{
								break;
							}
						}
						if (meet)
						{
							return;
						}
					}
				}
				throw new PXSetPropertyException(PXMessages.LocalizeFormat(ErrorMessages.ElementDoesntExist, _FieldName));
			}
		}

		private string GetHash()
		{
			return this.GetType().FullName + this.hashCode.ToString();
		}
		#endregion

		#region Initialization
		private delegate PXView CreateViewDelegate(PXCustomSelectorAttribute attr, PXGraph graph, bool IsReadOnly, BqlCommand select);
		// Dictionary of createView delegates for each user-defined class which are derived from PXCustomSelector, dictionary key - is a type of derived class
		private static readonly Dictionary<string, CreateViewDelegate> createView = new Dictionary<string, CreateViewDelegate>();
		private static readonly object _vlock = new object();


		[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, Unrestricted = true)]
		[System.Security.Permissions.ReflectionPermission(System.Security.Permissions.SecurityAction.Assert, Unrestricted = true)]

		private static CreateViewDelegate CreateDelegate(PXCustomSelectorAttribute attr)
		{
			DynamicMethod dm;
			if (!PXGraph.IsRestricted) {
				dm = new DynamicMethod("InitView", typeof(PXView), new Type[] { typeof(PXCustomSelectorAttribute), typeof(PXGraph), typeof(bool), typeof(BqlCommand) }, typeof(PXCustomSelectorAttribute), true);
			}
			else {
				dm = new DynamicMethod("InitView", typeof(PXView), new Type[] { typeof(PXCustomSelectorAttribute), typeof(PXGraph), typeof(bool), typeof(BqlCommand) }, true);
			}
			MethodInfo del = attr.GetType().GetMethod("GetRecords", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (del == null)
				return null;

			Type tdel = null;
			ParameterInfo[] pars = del.GetParameters();
			if (typeof(IEnumerable).IsAssignableFrom(del.ReturnType)) {
				switch (pars.Length) {
					case 0:
						tdel = typeof(PXSelectDelegate);
						break;
					case 1:
						tdel = typeof(PXSelectDelegate<>).MakeGenericType(
							pars[0].ParameterType);
						break;
					case 2:
						tdel = typeof(PXSelectDelegate<,>).MakeGenericType(
							pars[0].ParameterType,
							pars[1].ParameterType);
						break;
					case 3:
						tdel = typeof(PXSelectDelegate<,,>).MakeGenericType(
							pars[0].ParameterType,
							pars[1].ParameterType,
							pars[2].ParameterType);
						break;
					case 4:
						tdel = typeof(PXSelectDelegate<,,,>).MakeGenericType(
							pars[0].ParameterType,
							pars[1].ParameterType,
							pars[2].ParameterType,
							pars[3].ParameterType);
						break;
					case 5:
						tdel = typeof(PXSelectDelegate<,,,,>).MakeGenericType(
							pars[0].ParameterType,
							pars[1].ParameterType,
							pars[2].ParameterType,
							pars[3].ParameterType,
							pars[4].ParameterType);
						break;
					case 6:
						tdel = typeof(PXSelectDelegate<,,,,,>).MakeGenericType(
							pars[0].ParameterType,
							pars[1].ParameterType,
							pars[2].ParameterType,
							pars[3].ParameterType,
							pars[4].ParameterType,
							pars[5].ParameterType);
						break;
					case 7:
						tdel = typeof(PXSelectDelegate<,,,,,,>).MakeGenericType(
							pars[0].ParameterType,
							pars[1].ParameterType,
							pars[2].ParameterType,
							pars[3].ParameterType,
							pars[4].ParameterType,
							pars[5].ParameterType,
							pars[6].ParameterType);
						break;
					case 8:
						tdel = typeof(PXSelectDelegate<,,,,,,,>).MakeGenericType(
							pars[0].ParameterType,
							pars[1].ParameterType,
							pars[2].ParameterType,
							pars[3].ParameterType,
							pars[4].ParameterType,
							pars[5].ParameterType,
							pars[6].ParameterType,
							pars[7].ParameterType);
						break;
					case 9:
						tdel = typeof(PXSelectDelegate<,,,,,,,,>).MakeGenericType(
							pars[0].ParameterType,
							pars[1].ParameterType,
							pars[2].ParameterType,
							pars[3].ParameterType,
							pars[4].ParameterType,
							pars[5].ParameterType,
							pars[6].ParameterType,
							pars[7].ParameterType,
							pars[8].ParameterType);
						break;
					case 10:
						tdel = typeof(PXSelectDelegate<,,,,,,,,,>).MakeGenericType(
							pars[0].ParameterType,
							pars[1].ParameterType,
							pars[2].ParameterType,
							pars[3].ParameterType,
							pars[4].ParameterType,
							pars[5].ParameterType,
							pars[6].ParameterType,
							pars[7].ParameterType,
							pars[8].ParameterType,
							pars[9].ParameterType);
						break;
					case 11:
						tdel = typeof(PXSelectDelegate<,,,,,,,,,,>).MakeGenericType(
							pars[0].ParameterType,
							pars[1].ParameterType,
							pars[2].ParameterType,
							pars[3].ParameterType,
							pars[4].ParameterType,
							pars[5].ParameterType,
							pars[6].ParameterType,
							pars[7].ParameterType,
							pars[8].ParameterType,
							pars[9].ParameterType,
							pars[10].ParameterType);
						break;
				}
			}
			else if (pars.Length == 0)
			{
				tdel = typeof(PXPrepareDelegate);
			}
			if (tdel == null)
				return null;

			ILGenerator ilgen = dm.GetILGenerator();
			LocalBuilder result = ilgen.DeclareLocal(typeof(PXView));
			ilgen.Emit(OpCodes.Nop);
			ilgen.Emit(OpCodes.Ldarg_1);
			ilgen.Emit(OpCodes.Ldarg_2);
			ilgen.Emit(OpCodes.Ldarg_3);
			ilgen.Emit(OpCodes.Ldarg_0);
			ilgen.Emit(OpCodes.Castclass, attr.GetType());
			ilgen.Emit(OpCodes.Ldftn, del);
			ilgen.Emit(OpCodes.Newobj, tdel.GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }));
			ilgen.Emit(OpCodes.Newobj, typeof(PXView).GetConstructor(new Type[] { typeof(PXGraph), typeof(bool), typeof(BqlCommand), typeof(Delegate) }));
			ilgen.Emit(OpCodes.Stloc, result.LocalIndex);
			ilgen.Emit(OpCodes.Ldloc, result.LocalIndex);
			ilgen.Emit(OpCodes.Ret);

			return (CreateViewDelegate)dm.CreateDelegate(typeof(CreateViewDelegate));
		}

		protected bool writeLog = false;
        /// <summary>
        /// The method executed when the attribute is copied to the cache level.
        /// </summary>
        /// <param name="sender">The cache object that has raised the event.</param>
        public override void CacheAttached(PXCache sender)
		{
			_CacheType = sender.GetItemType();
			_Graph = sender.Graph;
			_BypassFieldVerifying = new ObjectRef<bool>();

			lock (_vlock) {
				if (!createView.ContainsKey(this.GetHash()))
					createView.Add(this.GetHash(), CreateDelegate(this));
			}

			populateFields(sender, true);
			PXView view;
			_ViewName = string.Format("_{0}{1}{2}", sender.GetItemType().Name, _FieldName, _ViewName);
			if (!sender.Graph.Views.TryGetValue(_ViewName, out view))
			{
				//if(createView != null)
				lock (_vlock)
				{
					view = createView[this.GetHash()](this, sender.Graph, !_DirtyRead, _Select);
				}
				sender.Graph.Views[_ViewName] = view;
				//else
				//    sender.Graph.Views[_ViewName] = new PXView(sender.Graph, !_DirtyRead, _Select);

				if (_Filterable)
				{
					sender.Graph.Views[_ViewName + PXFilterableAttribute.FilterHeaderName] = createFilterView(sender.Graph);
					sender.Graph.Views[_ViewName + PXFilterableAttribute.FilterRowName] = new PXView(sender.Graph, false, new Select<FilterRow, Where<FilterRow.filterID, Equal<Required<FilterRow.filterID>>>>());
				}
			}
			else if (view.BqlTarget != this.GetType()) {
				//_ViewName = Guid.NewGuid().ToString();
				lock (_vlock)
				{
					view = createView[this.GetHash()](this, sender.Graph, !_DirtyRead, _Select);
				}
				sender.Graph.Views[_ViewName] = view;

				if (_Filterable)
				{
					sender.Graph.Views[_ViewName + PXFilterableAttribute.FilterHeaderName] = createFilterView(sender.Graph);
					sender.Graph.Views[_ViewName + PXFilterableAttribute.FilterRowName] = new PXView(sender.Graph, false, new Select<FilterRow, Where<FilterRow.filterID, Equal<Required<FilterRow.filterID>>>>());
				}
			}

			if (!(_CacheType == _Type || _CacheType.IsSubclassOf(_Type))) {
				EmitColumnForDescriptionField(sender);
			}

			//if (_DescriptionField != null)
			//{
			//    string alias = _FieldName + "_" + _Type.Name + "_" + _DescriptionField.Name;
			//    if (!sender.Fields.Contains(alias))
			//    {
			//        sender.Fields.Add(alias);
			//        sender.FieldSelectingEvents[alias.ToLower()] += DescriptionFieldSelecting;
			//    }
			//}
			if (_SubstituteKey != null) {
				string name = _FieldName.ToLower();
				sender.FieldSelectingEvents[name] += SubstituteKeyFieldSelecting;
				sender.FieldUpdatingEvents[name] += SubstituteKeyFieldUpdating;
			}
		}
		#endregion
	}
	#endregion

	public class UIFieldRef
	{
		public PXUIFieldAttribute UIFieldAttribute;
	}

	public class ForeignKeyChecker
	{
		private PXCache _sender;
		private object _row;
		private Type _fieldType, _searchType;
		public string CustomMessage = null;

		public ForeignKeyChecker(PXCache sender, object row, Type fieldType, Type searchType)
		{
			_sender = sender;
			_row = row;
			_fieldType = fieldType;
			_searchType = searchType;
		}

		public void DoCheck()
		{
			string foreingTableName;
			string currentTableName;
			if (isExists(out currentTableName, out foreingTableName))
			{
				string message = String.IsNullOrEmpty(CustomMessage) ? ErrorMessages.ExtRefError : CustomMessage;
				if (message.Contains("{0}") && message.Contains("{1}"))
				{
					message = PXLocalizer.LocalizeFormat(message, currentTableName, foreingTableName);
				}
				else
				{
					message = PXLocalizer.Localize(message);
				}
				throw new PXException(message);
			}
		}

		private bool isExists(out string currentTableName, out string foreingTableName)
		{
			if (_searchType != null && !typeof(IBqlSearch).IsAssignableFrom(_searchType))
			{
				throw new PXArgumentException("selectType", ErrorMessages.ArgumentException);
			}
			Type currentTableType = _row.GetType();
			Type cmd;
			Type tableType = BqlCommand.GetItemType(_fieldType);
			foreingTableName = getTableName(tableType);
			currentTableName = getTableName(currentTableType);
			Type currentFieldType = getCurrentFieldType(_sender, _fieldType);
			if (currentFieldType == null)
			{
				return false;
			}
			if (_searchType == null)
			{
				cmd = BqlCommand.Compose(
					typeof(Search<,>),
					_fieldType,
					typeof(Where<,>),
					_fieldType,
					typeof(Equal<>), typeof(Current<>), currentFieldType);
			}
			else
			{
				IBqlSearch Select = (IBqlSearch)Activator.CreateInstance(_searchType);
				if (Select.GetType() != _searchType)
				{
					throw new PXArgumentException("selectType", ErrorMessages.ArgumentException);
				}
				List<Type> args = new List<Type> { _searchType.GetGenericTypeDefinition() };
				args.AddRange(_searchType.GetGenericArguments());
				int j = args.FindIndex(arg => typeof(IBqlWhere).IsAssignableFrom(arg));
				if (j == -1)
				{
					throw new PXArgumentException("selectType", ErrorMessages.ArgumentException);
				}
				args[j] = BqlCommand.Compose(
					typeof(Where2<,>),
					typeof(Where<,>),
					_fieldType,
					typeof(Equal<>), typeof(Current<>), currentFieldType,
					typeof(And<>),
					args[j]);
				cmd = BqlCommand.Compose(args.ToArray());
			}
			PXView view = new PXView(_sender.Graph, false, BqlCommand.CreateInstance(cmd));
			object refObject = view.SelectSingleBound(new object[] { _row });
			return refObject != null;
		}

		private string getTableName(Type TableType)
		{
			if (TableType.IsDefined(typeof(PXCacheNameAttribute), true))
			{
				var attr = (PXCacheNameAttribute)(TableType.GetCustomAttributes(typeof(PXCacheNameAttribute), true)[0]);
				return attr.GetName();
			}
			return TableType.Name;
		}

		private Type getCurrentFieldType(PXCache sender, Type foreingFieldType)
		{
			foreach (var attr in sender.Graph.Caches[BqlCommand.GetItemType(foreingFieldType)].GetAttributesReadonly(foreingFieldType.Name))
			{
				if (attr is PXSelectorAttribute)
				{
					return ((PXSelectorAttribute)attr).Field;
				}
			}
			return null;
		}
	}


}
