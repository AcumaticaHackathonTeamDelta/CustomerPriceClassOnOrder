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
using System.ComponentModel;


namespace PX.Data
{
	public delegate void PXRowUpdating(PXCache sender, PXRowUpdatingEventArgs e);

    /// <summary>Provides data for the <a
    /// href="RowUpdating.html">RowUpdating</a> event.</summary>
	public sealed class PXRowUpdatingEventArgs : CancelEventArgs
	{
		private readonly object _Row;
		private readonly object _NewRow;
		private readonly bool _ExternalCall;

        /// <summary>
        /// Initializes a new instance of the class with the provided values.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="newrow"></param>
        /// <param name="externalCall"></param>
		public PXRowUpdatingEventArgs(object row, object newrow, bool externalCall)
		{
			_Row = row;
			_NewRow = newrow;
			_ExternalCall = externalCall;
		}

        /// <summary>Gets the original DAC object that is being updated.</summary>
		public object Row
		{
			get
			{
				return _Row;
			}
		}
        /// <summary>Gets the updated copy of the DAC object that is going to be
        /// merged with the original one.</summary>
		public object NewRow
		{
			get
			{
				return _NewRow;
			}
		}

        /// <summary>
        /// Means that transaction has been initiated from the UI.<br/>
        /// More precisely, the method Update(IDictionary, IDictionary) has been invoked.<br/>
        /// </summary>
		public bool ExternalCall
		{
			get
			{
				return _ExternalCall;
			}
		}
	}

	public delegate void PXRowUpdated(PXCache sender, PXRowUpdatedEventArgs e);

    /// <summary>Provides data for the <a
    /// href="RowUpdated.html">RowUpdated</a> event.</summary>
	public sealed class PXRowUpdatedEventArgs : EventArgs
	{
		private readonly object _Row;
		private readonly object _OldRow;
		private readonly bool _ExternalCall;

        /// <summary>
        /// Initializes a new instance of the class with the provided values.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="oldRow"></param>
        /// <param name="externalCall"></param>
		public PXRowUpdatedEventArgs(object row, object oldRow, bool externalCall)
		{
			_Row = row;
			_OldRow = oldRow;
			_ExternalCall = externalCall;
		}

        /// <summary>Gets the DAC object that has been updated</summary>
		public object Row
		{
			get
			{
				return _Row;
			}
		}
        /// <summary>Gets the copy of the original DAC object before the Update
        /// operation</summary>
		public object OldRow
		{
			get
			{
				return _OldRow;
			}
		}

        /// <summary>
        /// Means that this event has been initiated from the UI.<br/>
        /// More precisely, the method Update(IDictionary, IDictionary) has been invoked.<br/>
        /// </summary>
		public bool ExternalCall
		{
			get
			{
				return _ExternalCall;
			}
		}
	}

	public delegate void PXRowInserting(PXCache sender, PXRowInsertingEventArgs e);

    /// <summary>Provides data for the <a
    /// href="RowInserting.html">RowInserting</a> event.</summary>
	public sealed class PXRowInsertingEventArgs : CancelEventArgs
	{
		private readonly object _Row;
		private readonly bool _ExternalCall;

        /// <summary>
        /// Initializes a new instance of the class with the provided values.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="externalCall"></param>
		public PXRowInsertingEventArgs(object row, bool externalCall)
		{
			_Row = row;
			_ExternalCall = externalCall;
		}

        /// <summary>Gets the DAC object that is being inserted.</summary>
		public object Row
		{
			get
			{
				return _Row;
			}
		}

        /// <summary>Gets the value indicating, if it equals <tt>true</tt>, that
        /// the DAC object is being inserted from the UI or through the Web
        /// Service API.</summary>
		public bool ExternalCall
		{
			get
			{
				return _ExternalCall;
			}
		}
	}

	public delegate void PXRowInserted(PXCache sender, PXRowInsertedEventArgs e);

    /// <summary>Provides data for the <a
    /// href="RowInserted.html">RowInserted</a> event.</summary>
	public sealed class PXRowInsertedEventArgs : EventArgs
	{
		private readonly object _Row;
		private readonly object _NewRow;
		private readonly bool _ExternalCall;

        /// <summary>
        /// Initializes a new instance of the class with the provided values.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="externalCall"></param>
		public PXRowInsertedEventArgs(object row, bool externalCall)
		{
			_Row = row;
			_ExternalCall = externalCall;
		}

		internal PXRowInsertedEventArgs(object row, object newRow, bool externalCall)
		{
			_Row = row;
			_NewRow = newRow;
			_ExternalCall = externalCall;
		}

        /// <summary>Gets the DAC object that has been inserted</summary>
		public object Row
		{
			get
			{
				return _Row;
			}
		}

		internal object NewRow
		{
			get
			{
				return _NewRow;
			}
		}

        /// <summary>
        /// Means that transaction has been initiated from the UI.<br/>
        /// More precisely, the method Insert(IDictionary) has been invoked.<br/>
        /// </summary>
        /// <summary>Gets the value indicating, if it equals <tt>true</tt>, that
        /// the DAC object has been inserted in the UI or through the Web Service
        /// API</summary>
		public bool ExternalCall
		{
			get
			{
				return _ExternalCall;
			}
		}
	}

	public delegate void PXRowDeleting(PXCache sender, PXRowDeletingEventArgs e);

    /// <summary>Provides data for the <a
    /// href="RowDeleting.html">RowDeleting</a> event.</summary>
	public sealed class PXRowDeletingEventArgs : CancelEventArgs
	{
		private readonly object _Row;
		private readonly bool _ExternalCall;

        /// <summary>
        /// Initializes a new instance of the class with the provided values.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="externalCall"></param>
		public PXRowDeletingEventArgs(object row, bool externalCall)
		{
			_Row = row;
			_ExternalCall = externalCall;
		}

        /// <summary>Gets the DAC object that has been marked as
        /// <tt>Deleted</tt>.</summary>
		public object Row
		{
			get
			{
				return _Row;
			}
		}

        /// <summary>Gets the value indicating, if it equals <tt>true</tt>, that
        /// the DAC object has been marked as <tt>Deleted</tt> in the UI or
        /// through the Web Service API.</summary>
		public bool ExternalCall
		{
			get
			{
				return _ExternalCall;
			}
		}
	}

	public delegate void PXRowDeleted(PXCache sender, PXRowDeletedEventArgs e);

    /// <summary>Provides data for the <a
    /// href="RowDeleted.html">RowDeleted</a> event.</summary>
	public sealed class PXRowDeletedEventArgs : EventArgs
	{
		private readonly object _Row;
		private readonly bool _ExternalCall;

        /// <summary>
        /// Initializes a new instance of the class with the provided values.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="externalCall"></param>
		public PXRowDeletedEventArgs(object row, bool externalCall)
		{
			_Row = row;
			_ExternalCall = externalCall;
		}

        /// <summary>Gets the DAC object that has been marked as
        /// <b><tt>Deleted</tt></b></summary>
		public object Row
		{
			get
			{
				return _Row;
			}
		}

        /// <summary>Gets the value indicating, if it equals <tt>true</tt>, that
        /// the DAC object has been marked as <b><tt>Deleted</tt></b> in the UI or
        /// through the Web Services API</summary>
		public bool ExternalCall
		{
			get
			{
				return _ExternalCall;
			}
		}
	}

	public delegate void PXRowSelected(PXCache sender, PXRowSelectedEventArgs e);
    /// <summary>Provides data for the <a
    /// href="RowSelected.html">RowSelected</a> event.</summary>
	public sealed class PXRowSelectedEventArgs : EventArgs
	{
		private readonly object _Row;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="row"></param>
		public PXRowSelectedEventArgs(object row)
		{
			_Row = row;
		}

        /// <summary>Gets the DAC object that is being processed</summary>
		public object Row
		{
			get
			{
				return _Row;
			}
		}
	}

	public delegate void PXCommandPreparing(PXCache sender, PXCommandPreparingEventArgs e);

    /// <summary>Provides data for the <a href="CommandPreparing.html">CommandPreparing</a>
    /// event.</summary>
	public sealed class PXCommandPreparingEventArgs : CancelEventArgs
	{
    /// <summary>
        /// The nested class that provides information about the field
        /// required for the SQL statement generation.
    /// </summary>
		public sealed class FieldDescription : ICloneable, IEquatable<FieldDescription>
	{
            /// <summary>
            /// The type of DAC objects placed in the cache.
            /// </summary>
			public readonly Type BqlTable;
            /// <summary>
            /// The name of the DAC field
            /// </summary>
			public string FieldName;
            /// <summary>The <tt>PXDbType</tt> of the DAC field being
            /// used during the current operation.</summary>
			public PXDbType DataType;
            /// <summary>
            /// The storage size of the DAC field.
            /// </summary>
			public readonly int? DataLength;
            /// <summary>
            /// The value stored in the DAC field.
            /// </summary>
			public readonly object DataValue;
            /// <summary>
            /// The value indicating that the DAC field being used during the
            /// UPDATE or DELETE operation is placed in the WHERE clause.
            /// </summary>
			public readonly bool IsRestriction;
			internal FieldDescription(Type bqlTable, string fieldName, PXDbType dataType, int? dataLength, object dataValue, bool isRestriction)
			{
				BqlTable = bqlTable;
				FieldName = fieldName;
				DataType = dataType;
				DataLength = dataLength;
				DataValue = dataValue;
				IsRestriction = isRestriction;
			}

			public object Clone()
			{
				return new FieldDescription(BqlTable, FieldName, DataType, DataLength, DataValue, IsRestriction);
		}

			public bool Equals(FieldDescription other)
			{
				if (other == null)
					return false;
				if (Object.ReferenceEquals(this, other))
					return true;
				return BqlTable == other.BqlTable
				       && FieldName == other.FieldName
				       && DataType == other.DataType
				       && DataLength == other.DataLength
				       && (ReferenceEquals(DataValue, other.DataValue) || DataValue != null && DataValue.Equals(other.DataValue))
				       && IsRestriction == other.IsRestriction;
			}
		}
        /// <summary>
        /// Initializes and returns an object containing the description of
        /// the DAC field being used during the current operation.
        /// </summary>
		public FieldDescription GetFieldDescription()
		{
			return new FieldDescription(_BqlTable, _FieldName, _DataType, _DataLength, _DataValue, _IsRestriction);
		}
	    public void FillFromFieldDescription(FieldDescription fDescr)
	    {
			if (fDescr == null)
				throw new ArgumentNullException("fDescr");
			_BqlTable = fDescr.BqlTable;
			_DataLength = fDescr.DataLength;
			_DataType = fDescr.DataType;
			_DataValue = fDescr.DataValue;
			_FieldName = fDescr.FieldName;
			_IsRestriction = fDescr.IsRestriction;
	    }
		private readonly object _Row;
		private object _Value;
		private readonly PXDBOperation _Operation;
		private readonly Type _Table;
		private Type _BqlTable;
		private string _FieldName;
		private PXDbType _DataType = PXDbType.Unspecified;
		private int? _DataLength;
		private object _DataValue;
		private bool _IsRestriction;
		public readonly ISqlDialect SqlDialect;

        /// <summary>
        /// Initializes an instance of the <tt>PXCommandPreparingEventArgs</tt> class.
        /// </summary>
        /// <param name="row">The data record.</param>
        /// <param name="value">The field value.</param>
        /// <param name="operation">The type of the database operation</param>
        /// <param name="table">The DAC type of the data record.</param>
		public PXCommandPreparingEventArgs(object row, object value, PXDBOperation operation, Type table, ISqlDialect dialect = null)
		{
			_Row = row;
			_Value = value;
			_Operation = operation;
			_Table = table;
			SqlDialect = dialect;
		}

		public bool IsSelect()
		{
			return (_Operation & PXDBOperation.Command) == PXDBOperation.Select;
		}

        /// <summary>Gets the current DAC object.</summary>
		public object Row
		{
			get
			{
				return _Row;
			}
		}
        /// <summary>Gets or sets the current DAC field value.</summary>
		public object Value
		{
			get
			{
				return _Value;
			}
			set
			{
				_Value = value;
			}
		}
        /// <summary>Gets the type of the current database
        /// operation.</summary>
		public PXDBOperation Operation
		{
			get
			{
				return _Operation;
			}
		}
        /// <summary>Gets the type of DAC objects placed in the cache.</summary>
		public Type Table
		{
			get
			{
				return _Table;
			}
		}
        /// <summary>Gets or sets the type of the DAC being used during the
        /// current operation.</summary>
		public Type BqlTable
		{
			get
			{
				return _BqlTable;
			}
			set
			{
				_BqlTable = value;
			}
		}
        /// <summary>Gets or sets the name of the DAC field being used during the
        /// current operation.</summary>
		public string FieldName
		{
			get
			{
				return _FieldName;
			}
			set
			{
				_FieldName = value;
			}
		}
        /// <summary>Gets or sets the <tt>PXDbType</tt> of the DAC field being
        /// used during the current operation.</summary>
		public PXDbType DataType
		{
			get
			{
				return _DataType;
			}
			set
			{
				_DataType = value;
			}
		}
        /// <summary>Gets or sets the number of characters in the DAC field being
        /// used during the current operation.</summary>
		public int? DataLength
		{
			get
			{
				return _DataLength;
			}
			set
			{
				_DataLength = value;
			}
		}
        /// <summary>Gets or sets the DAC field value being used during the
        /// current operation.</summary>
		public object DataValue
		{
			get
			{
				return _DataValue;
			}
			set
			{
				_DataValue = value;
			}
		}
        /// <summary>Gets or sets the value indicating that the DAC field being
        /// used during the UPDATE or DELETE operation is placed in the WHERE
        /// clause.</summary>
		public bool IsRestriction
		{
			get
			{
				return _IsRestriction;
			}
			set
			{
				_IsRestriction = value;
			}
		}
	}

	public delegate void PXRowSelecting(PXCache sender, PXRowSelectingEventArgs e);
    /// <summary>Provides data for the <a
    /// href="RowSelecting.html">RowSelecting</a> event.</summary>
	public sealed class PXRowSelectingEventArgs : CancelEventArgs
	{
		private object _Row;
		private readonly PXDataRecord _Record;
		private int _Position;
		private readonly bool _IsReadOnly;

        /// <summary>
        /// Initializes a new instance of the class with the provided values.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="record"></param>
        /// <param name="position"></param>
        /// <param name="isReadOnly"></param>
		public PXRowSelectingEventArgs(object row, PXDataRecord record, int position, bool isReadOnly)
		{
			_Row = row;
			_Record = record;
			_Position = position;
			_IsReadOnly = isReadOnly;
		}

        /// <summary>Gets the DAC object that is being processed.</summary>
		public object Row
		{
			get
			{
				return _Row;
			}
			internal set
			{
				_Row = value;
			}
		}
        /// <summary>Gets the proceeded data record in the result set.</summary>
		public PXDataRecord Record
		{
			get
			{
				return _Record;
			}
		}
		public int Position
		{
			get
			{
				return _Position;
			}
			set
			{
				_Position = value;
			}
		}
		public bool IsReadOnly
		{
			get
			{
				return _IsReadOnly;
			}
		}
	}

	public delegate void PXRowPersisting(PXCache sender, PXRowPersistingEventArgs e);

    /// <summary>Provides data for the <a
    /// href="RowPersisting.html">RowPersisting</a>
    /// event.</summary>
	public sealed class PXRowPersistingEventArgs : CancelEventArgs
	{
		private readonly object _Row;
		private readonly PXDBOperation _Operation;

        /// <summary>
        /// Initializes a new instance of the class with the provided values.
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="row"></param>
		public PXRowPersistingEventArgs(PXDBOperation operation, object row)
		{
			_Row = row;
			_Operation = operation;
		}

        /// <summary>Gets the DAC object that is being committed to the
        /// database.</summary>
		public object Row
		{
			get
			{
				return _Row;
			}
		}
        /// <summary>Gets the <tt>PXDBOperation</tt> of the current commit
        /// operation</summary>
		public PXDBOperation Operation
		{
			get
			{
				return _Operation;
			}
		}
	}

	public delegate void PXRowPersisted(PXCache sender, PXRowPersistedEventArgs e);

    /// <summary>Provides data for the <a
    /// href="RowPersisted.html">RowPersisted</a> event.</summary>
	public sealed class PXRowPersistedEventArgs : EventArgs
	{
		private readonly object _Row;
		private readonly PXDBOperation _Operation;
		private readonly PXTranStatus _TranStatus;
		private readonly Exception _Exception;

        /// <summary>
        /// Initializes a new instance of the class with the provided values.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="operation"></param>
        /// <param name="tranStatus"></param>
        /// <param name="exception"></param>
		public PXRowPersistedEventArgs(object row, PXDBOperation operation, PXTranStatus tranStatus, Exception exception)
		{
			_Row = row;
			_Operation = operation;
			_TranStatus = tranStatus;
			_Exception = exception;
		}

        /// <summary>Gets the DAC object that has been committed to the
        /// database</summary>
		public object Row
		{
			get
			{
				return _Row;
			}
		}
        /// <summary>Gets the status of the transation scope associated with the
        /// current committing operation</summary>
		public PXTranStatus TranStatus
		{
			get
			{
				return _TranStatus;
			}
		}
        /// <summary>Gets the <tt>PXDBOperation</tt> value indicating the type of
        /// the current commit operation</summary>
		public PXDBOperation Operation
		{
			get
			{
				return _Operation;
			}
		}
        /// <summary>Gets the <tt>Exception</tt> object thrown while changes are
        /// committed to the database</summary>
		public Exception Exception
		{
			get
			{
				return _Exception;
			}
		}
	}

	public delegate void PXFieldSelecting(PXCache sender, PXFieldSelectingEventArgs args);

    /// <summary>Provides data for the <a
    /// href="FieldSelecting.html">FieldSelecting</a>
    /// event.</summary>
	public sealed class PXFieldSelectingEventArgs : CancelEventArgs
	{
		private readonly object _Row;
		private object _ReturnValue;
		private bool _IsAltered;
		private readonly bool _ExternalCall;

        /// <summary>
        /// Initializes a new instance of the class with the provided values.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="returnValue"></param>
        /// <param name="isAltered"></param>
        /// <param name="externalCall"></param>
		public PXFieldSelectingEventArgs(object row, object returnValue, bool isAltered, bool externalCall)
		{
			_Row = row;
			_ReturnValue = returnValue;
			_IsAltered = isAltered;
			_ExternalCall = externalCall;
		}
        /// <summary>Gets the current DAC object.</summary>
		public object Row
		{
			get
			{
				return _Row;
			}
		}
        /// <summary>Gets or sets the data used to set up DAC field input control
        /// or cell presentation.</summary>
		public object ReturnState
		{
			get
			{
				return _ReturnValue;
			}
			set
			{
				_ReturnValue = value;
			}
		}
        /// <summary>Gets or sets the value indicating whether the
        /// <tt>ReturnState</tt> property should be created for each data
        /// record.</summary>
		public bool IsAltered
		{
			get
			{
				return _IsAltered;
			}
			set
			{
				_IsAltered = value;
			}
		}
        /// <summary>Gets or sets the external presentation of the value of the
        /// DAC field.</summary>
		public object ReturnValue
		{
			get
			{
				PXFieldState state = _ReturnValue as PXFieldState;
				if (state == null)
				{
					return _ReturnValue;
				}
				else
				{
					return state.Value;
				}
			}
			set
			{
				PXFieldState state = _ReturnValue as PXFieldState;
				if (state == null)
				{
					_ReturnValue = value;
				}
				else
				{
					state.Value = value;
				}
			}
		}
        /// <summary>Gets the value specifying if the current DAC field has been
        /// selected in the UI or through the Web Service API.</summary>
		public bool ExternalCall
		{
			get
			{
				return _ExternalCall;
			}
		}
	}

	public delegate void PXFieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs args);

    /// <summary>Provides data for the <a
    /// href="FieldDefaulting.html">FieldDefaulting</a>
    /// event.</summary>
	public sealed class PXFieldDefaultingEventArgs : CancelEventArgs
	{
		private readonly object _Row;
		private object _NewValue;

        /// <summary>
        /// Initializes a new instance of the class with the provided values.
        /// </summary>
        /// <param name="row"></param>
		public PXFieldDefaultingEventArgs(object row)
		{
			_Row = row;
		}
        /// <summary>Gets the current DAC object.</summary>
		public object Row
		{
			get
			{
				return _Row;
			}
		}
        /// <summary>Gets or sets the default value for the DAC field.</summary>
		public object NewValue
		{
			get
			{
				return _NewValue;
			}
			set
			{
				_NewValue = value;
			}
		}
	}

	public delegate void PXFieldUpdating(PXCache sender, PXFieldUpdatingEventArgs args);

    /// <summary>Provides data for the <a
    /// href="FieldUpdating.html">FieldUpdating</a>
    /// event.</summary>
	public sealed class PXFieldUpdatingEventArgs : CancelEventArgs
	{
		private readonly object _Row;
		private object _NewValue;

        /// <summary>
        /// Initializes a new instance of the class with the provided values.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="newValue"></param>
		public PXFieldUpdatingEventArgs(object row, object newValue)
		{
			_Row = row;
			_NewValue = newValue;
		}
        /// <summary>Gets the current DAC object.</summary>
		public object Row
		{
			get
			{
				return _Row;
			}
		}
        /// <summary>Gets or sets the internal DAC field value.</summary>
		public object NewValue
		{
			get
			{
				return _NewValue;
			}
			set
			{
				_NewValue = value;
			}
		}
	}

	public delegate void PXFieldVerifying(PXCache sender, PXFieldVerifyingEventArgs args);

    /// <summary>Provides data for the <a
    /// href="FieldVerifying.html">FieldVerifying</a>
    /// event.</summary>
	public sealed class PXFieldVerifyingEventArgs : CancelEventArgs
	{
		private readonly object _Row;
		private object _NewValue;
		private readonly bool _ExternalCall;

        /// <summary>
        /// Initializes a new instance of the class with the provided values.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="newValue"></param>
        /// <param name="externalCall"></param>
		public PXFieldVerifyingEventArgs(object row, object newValue, bool externalCall)
		{
			_Row = row;
			_NewValue = newValue;
			_ExternalCall = externalCall;
		}
        /// <summary>Gets the current DAC object.</summary>
		public object Row
		{
			get
			{
				return _Row;
			}
		}
        /// <summary>Gets or sets the new value of the current DAC
        /// field.</summary>
		public object NewValue
		{
			get
			{
				return _NewValue;
			}
			set
			{
				_NewValue = value;
			}
		}
        /// <summary>Gets the value specifying if the new value of the current DAC
        /// field has been received from the UI or through the Web Service
        /// API.</summary>
		public bool ExternalCall
		{
			get
			{
				return _ExternalCall;
			}
		}
	}

	public delegate void PXFieldUpdated(PXCache sender, PXFieldUpdatedEventArgs args);

    /// <summary>Provides data for the <a
    /// href="FieldUpdated.html">FieldUpdated</a> event.</summary>
	public sealed class PXFieldUpdatedEventArgs : EventArgs
	{
		private readonly object _Row;
		private readonly object _OldValue;
		private readonly bool _ExternalCall;

        /// <summary>
        /// Initializes a new instance of the class with the provided values.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="oldValue"></param>
        /// <param name="externalCall"></param>
		public PXFieldUpdatedEventArgs(object row, object oldValue, bool externalCall)
		{
			_Row = row;
			_OldValue = oldValue;
			_ExternalCall = externalCall;
		}

        /// <summary>Gets the current DAC object</summary>
		public object Row
		{
			get
			{
				return _Row;
			}
		}
        /// <summary>Gets the previous value of the current DAC field</summary>
		public object OldValue
		{
			get
			{
				return _OldValue;
			}
		}
        /// <summary>Gets the value specifying whether the new value of the
        /// current DAC field has been changed in the UI or through the Web
        /// Service API</summary>
		public bool ExternalCall
		{
			get
			{
				return _ExternalCall;
			}
		}
	}

	public delegate void PXExceptionHandling(PXCache sender, PXExceptionHandlingEventArgs args);

    /// <summary>Provides data for the <a
    /// href="ExceptionHandling.html">ExceptionHandling</a>
    /// event.</summary>
	public sealed class PXExceptionHandlingEventArgs : CancelEventArgs
	{
		private readonly object _Row;
		private object _NewValue;
		private readonly Exception _Exception;

        /// <summary>
        /// Initializes a new instance of the class with the provided values.
        /// </summary>
        /// <param name="row">The data record.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="exception">The exception instance.</param>
		public PXExceptionHandlingEventArgs(object row, object newValue, Exception exception)
		{
			_Row = row;
			_NewValue = newValue;
			_Exception = exception;
		}
        /// <summary>Gets the current DAC object.</summary>
		public object Row
		{
			get
			{
				return _Row;
			}
		}
        /// <summary>Gets or sets the values of the DAC field. By default,
        /// contains the values that are: <ul>
        /// <li>Generated in the process of assigning a DAC field its default value.</li>
        /// <li>Passed as new values when a field is updated.</li>
        /// <li>Entered in the UI or through the Web Service API.</li>
        /// <li>Received with the <tt>PXCommandPreparingException</tt>,
        /// <tt>PXRowPersistingException</tt>, or <tt>PXRowPersistedException</tt>
        /// exception.</li>
        /// </ul></summary>
		public object NewValue
		{
			get
			{
				return _NewValue;
			}
			set
			{
				_NewValue = value;
			}
		}
        /// <summary>Gets the initial exception that caused the event to be
        /// raised.</summary>
		public Exception Exception
		{
			get
			{
				return _Exception;
			}
		}
	}
}
