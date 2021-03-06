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

namespace PX.Data
{
	/// <summary>This enumeration specifies the type of a T-SQL statement
	/// generated by the framework.</summary>
	/// <remarks>The enumeration is used to indicate the type of the operation
	/// and the option set for the operation. <tt>PXDBOperation</tt> supports
	/// the <tt>FlagsAttribute</tt> attribute, which allows
	/// <tt>PXDBOperation</tt> members to be represented as bit fields in the
	/// enumeration value.</remarks>
	/// <example>
	/// <para></para>The code below shows how to get the type of an operation.
	/// <code>
	/// protected virtual void DACName_FieldName_CommandPreparing(
	///     PXCache sender, PXCommandPreparingEventArgs e)
	/// {
	///     PXDBOperation operationKind = e.Operation &amp; PXDBOperation.Command;
	/// }</code>
	/// </example>
	[Flags]
	public enum PXDBOperation
	{
		// Command uses the lowest 2 bits
		/// <summary>SELECT operation</summary>
		Select = 0,
		/// <summary>UPDATE operation</summary>
		Update = 1,
		/// <summary>INSERT operation</summary>
		Insert = 2,
		/// <summary>DELETE operation</summary>
		Delete = 3,
		/// <exclude/>
		Command = 0x03,

		// Option uses bits from third to seventh
		/// <summary>The operation has no options set.</summary>
		Normal = 0 << 2,
		/// <summary>This specifies an aggregate operation.</summary>
		GroupBy = 1 << 2,
		/// <summary>The result of the operation cannot be used to prepare
		/// the external representation.</summary>
		Internal = 2 << 2,
		/// <summary>The operation contains a sorting, filter, or search query across
		/// any DAC field visible in the UI.</summary>
		External = 4 << 2,
		/// <summary>The operation is changing system data visibility and transferring
		/// it from the system data segment to the customer data segment.</summary>
		Second = 8 << 2,
		/// <exclude/>
		ReadOnly = 16 << 2,
		/// <exclude/>
		Option = 31 << 2,

		// Place uses bits from eigth to eleventh
		SelectClause = 1 << 7,
		WhereClause = 2 << 7,
		OrderByClause = 4 << 7,
		GroupByClause = 8 << 7,
		Place = 15 << 7

	}

	//public enum PXDBAggregate
	//{
	//    None,
	//    Sum,
	//    Min,
	//    Max,
	//    Avg,
	//    GroupBy
	//}

	public enum PXAttributeLevel
	{
		Type,
		Cache,
		Item
	}

    /// <summary>Maps the user role's access rights for a specific
    /// <tt>PXCache&lt;&gt;</tt> object.</summary>
    /// <example>
    /// Using the enumeration value to confiuge access rights for the button
    /// representing a graph action in the user interface:
    /// <code>
    /// public PXAction&lt;ApproveBillsFilter&gt; ViewDocument;
    /// [PXUIField(DisplayName = "View Document",
    ///     MapEnableRights = PXCacheRights.Update,
    ///     MapViewRights = PXCacheRights.Select)]
    /// [PXButton]
    /// public virtual IEnumerable viewDocument(PXAdapter adapter)
    /// {
    /// ...
    /// }</code>
    /// </example>
	public enum PXCacheRights : byte
	{
        /// <summary>Matches the roles for whom access to a <tt>PXCache</tt>
        /// object is denied.</summary>
		Denied,
        /// <summary>Matches the roles that are allowed to read data records of
        /// the DAC type corresponding to the <tt>PXCache&lt;&gt;</tt> object.</summary>
		Select,
        /// <summary>Matches the roles that are allowed to update data records of
        /// the DAC type corresponding to the <tt>PXCache&lt;&gt;</tt> object.</summary>
		Update,
        /// <summary>Matches the roles that are allowed to insert data records of
        /// the DAC type corresponding to the <tt>PXCache&lt;&gt;</tt> object.</summary>
		Insert,
        /// <summary>Matches the roles that are allowed to delete data records of
        /// the DAC type corresponding to the <tt>PXCache&lt;&gt;</tt> object.</summary>
		Delete
	}

	public struct PXCacheRightsPrioritized
	{
		public readonly bool Prioritized;
		public readonly PXCacheRights Rights;
		public PXCacheRightsPrioritized(bool prioritized, PXCacheRights rights)
		{
			Prioritized = prioritized;
			Rights = rights;
		}

		private bool EqualsIntern(PXCacheRightsPrioritized other)
		{
			return Prioritized == other.Prioritized && Rights == other.Rights;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is PXCacheRightsPrioritized))
				return false;

			//if (ReferenceEquals(null, obj)) return false;
			//if (ReferenceEquals(this, obj)) return true;
			//if (obj.GetType() != this.GetType()) return false;
			return EqualsIntern((PXCacheRightsPrioritized)obj);
		}


		public override int GetHashCode()
		{
			return (Prioritized.GetHashCode() * 397) ^ Rights.GetHashCode();
		}

	}

	public enum PXMemberRights : byte
	{
		Denied,
		Visible,
		Enabled
	}

	public struct PXMemberRightsPrioritized
	{
		public readonly bool Prioritized;
		public readonly PXMemberRights Rights;
		public PXMemberRightsPrioritized(bool prioritized, PXMemberRights rights)
		{
			Prioritized = prioritized;
			Rights = rights;
		}

		private bool EqualsIntern(PXMemberRightsPrioritized other)
		{
			return Prioritized == other.Prioritized && Rights == other.Rights;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is PXMemberRightsPrioritized))
				return false;

			//if (ReferenceEquals(null, obj)) return false;
			//if (ReferenceEquals(this, obj)) return true;
			//if (obj.GetType() != this.GetType()) return false;
			return EqualsIntern((PXMemberRightsPrioritized)obj);
		}


		public override int GetHashCode()
		{
			return (Prioritized.GetHashCode() * 397) ^ Rights.GetHashCode();
		}

	}

	internal enum PXCacheOperation
	{
		Insert,
		Update,
		Delete
	}

    /// <summary>Describes the current status of a transaction
    /// scope.</summary>
	public enum PXTranStatus
	{
        /// <summary>The status of the transaction is unknown, because some
        /// participants still have to be polled.</summary>
		Open,
        /// <summary>The changes associated with the transaction scope have been
        /// successfully committed to the database.</summary>
		Completed,
        /// <summary>The changes within the transaction scope have been dropped
        /// because of an error.</summary>
		Aborted
	}

    /// <summary>Defines possible options of clearing the graph data through
    /// the <see cref="PXGraph.Clear(PXClearOption)">Clear(PXClearOption)</see>
    /// method.</summary>
	public enum PXClearOption
	{
        /// <summary>Data records are preserved.</summary>
		PreserveData,
        /// <summary>The timestamp is preserved.</summary>
		PreserveTimeStamp,
        /// <summary>The query cache is preserved.</summary>
		PreserveQueries,
        /// <summary>Everything is removed.</summary>
		ClearAll,
        /// <summary>Only the query cache is cleared.</summary>
		ClearQueriesOnly
	}

    /// <summary>Defines possible special types of a button. The enumeration
    /// is used to set <tt>PXButton</tt> attribute properties.</summary>
	public enum PXSpecialButtonType
	{
        /// <summary>The button does not have a special type.</summary>
		Default,
        /// <summary>The button has the <b>Save</b> button type. In particular, a
        /// graph searches buttons of this type when the
        /// graph's<tt>Actions.PressSave()</tt> method is invoked.</summary>
		Save,
		SaveNotClose,
        /// <summary>The button has the <b>Cancel</b> button type. In particular,
        /// a graph searches buttons of this type when the graph's
        /// <tt>Actions.PressCancel()</tt> method is invoked.</summary>
		Cancel,
        /// <summary>The button has the <b>Refresh</b> button type.</summary>
		Refresh,
		Report,
		First,
		Next,
		Prev,
		Last,
		Insert,
		Delete,
		Approve,
		ApproveAll,
		Process,
		ProcessAll,
		EditDetail
	}
}
