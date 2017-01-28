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
using System.Collections.Generic;
using System.Text;

namespace PX.Data
{
    [Serializable]
	[PXCacheName(PX.Data.EP.Messages.Note)]
	public class Note : IBqlTable
	{
		#region NoteID
		public abstract class noteID : IBqlField
		{
		}
		private Guid? _NoteID;
		[PXDBSequentialGuid(IsKey = true)]
		[PXUIField(Visibility = PXUIVisibility.SelectorVisible)]
		public Guid? NoteID
		{
			get
			{
				return _NoteID;
			}
			set
			{
				_NoteID = value;
			}
		}
		#endregion

		#region NoteText
		public abstract class noteText : IBqlField
		{
		}
		private string _NoteText;
		[PXDBString(IsUnicode = true)]
		[PXNoteTextAttribute]
		[PXUIField(DisplayName = PX.Data.EP.Messages.NoteText)]
		public string NoteText
		{
			get
			{
				return _NoteText;
			}
			set
			{
				_NoteText = value;
			}
		}
		#endregion

		#region EntityType
		public abstract class entityType : IBqlField
		{
		}
		private string _EntityType;
		[PXDBString()]
		[PXUIField(Visibility = PXUIVisibility.SelectorVisible)]
		public string EntityType
		{
			get
			{
				return _EntityType;
			}
			set
			{
				_EntityType = value;
			}
		}
		#endregion

		#region GraphType
		public abstract class graphType : IBqlField
		{
		}
		private string _GraphType;
		[PXDBString()]
		public string GraphType
		{
			get
			{
				return _GraphType;
			}
			set
			{
				_GraphType = value;
			}
		}
		#endregion

		#region ExternalKey
		public abstract class externalKey : IBqlField { }
		[PXDBString(255, IsUnicode = true)]
		[PXUIField(Visible = false)]
		public string ExternalKey { get; set; }
		#endregion

		#region unbound fields
		#region EntityName
		public abstract class entityName : IBqlField
		{
		}
		private string _EntityName;
		[PXString()]
		[PXUIField(Visibility = PXUIVisibility.SelectorVisible)]
		public string EntityName
		{
			get
			{
				return _EntityName;
			}
			set
			{
				_EntityName = value;
			}
		}
		#endregion
		#endregion
	}

    [Serializable]
	public sealed class NoteDoc : IBqlTable
	{
		#region NoteID
		public abstract class noteID : IBqlField
		{
		}
		private Guid? _NoteID;
		[PXDBGuid(IsKey = true)]
		public Guid? NoteID
		{
			get
			{
				return _NoteID;
			}
			set
			{
				_NoteID = value;
			}
		}
		#endregion

		#region FileID
		public abstract class fileID : IBqlField
		{
		}
		private Guid? _FileID;
		[PXDBGuid(IsKey = true)]
		public Guid? FileID
		{
			get
			{
				return _FileID;
			}
			set
			{
				_FileID = value;
			}
		}
		#endregion

		#region unbound fields
		public abstract class entityType : IBqlField
		{
		}
		private string _EntityType;
		[PXString()]
		public string EntityType
		{
			get
			{
				return _EntityType;
			}
			set
			{
				_EntityType = value;
			}
		}
		public abstract class entityName : IBqlField
		{
		}
		private string _EntityName;
		[PXString()]
		[PXUIField(DisplayName = "Entity")]
		public string EntityName
		{
			get
			{
				return _EntityName;
			}
			set
			{
				_EntityName = value;
			}
		}
		public abstract class entityRowValues : IBqlField
		{
		}
		private string _EntityRowValues;
		[PXString()]
		[PXUIField(DisplayName = "Row Values")]
		public string EntityRowValues
		{
			get
			{
				return _EntityRowValues;
			}
			set
			{
				_EntityRowValues = value;
			}
		}
		#endregion
	}
}
