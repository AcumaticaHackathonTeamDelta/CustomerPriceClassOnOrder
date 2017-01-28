namespace PX.Objects.CS
{
	using System;
	using PX.Data;
	using System.Diagnostics;

	[DebuggerDisplay("[{AttributeID}]: {Description}")]
	[System.SerializableAttribute()]
	[PXPrimaryGraph(
		new Type[] { typeof(CSAttributeMaint)},
		new Type[] { typeof(Select<CSAttribute, 
			Where<CSAttribute.attributeID, Equal<Current<CSAttribute.attributeID>>>>)
		})]
	[PXCacheName(Messages.Attribute)]
	public partial class CSAttribute : PX.Data.IBqlTable
	{
		public const int Text = 1;
		public const int Combo = 2;
		public const int CheckBox = 4;
		public const int Datetime = 5;
		public const int MultiSelectCombo = 6;

		#region AttributeID
		public abstract class attributeID : PX.Data.IBqlField
		{
		}
		protected String _AttributeID;
		[PXDBString(10, IsUnicode = true, IsKey = true, InputMask = ">aaaaaaaaaa")]
		[PXDefault()]
		[PXUIField(DisplayName = "Attribute ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(CSAttribute.attributeID))]
		public virtual String AttributeID
		{
			get
			{
				return this._AttributeID;
			}
			set
			{
				this._AttributeID = value;
			}
		}
		#endregion
		#region Description
		public abstract class description : PX.Data.IBqlField
		{
		}
		protected String _Description;
		[PXDBLocalizableString(60, IsUnicode = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				this._Description = value;
			}
		}
		#endregion
		#region ControlType
		public abstract class controlType : PX.Data.IBqlField
		{
		}
		protected Int32? _ControlType;
		[PXDBInt()]
		[PXDefault(1)]
		[PXUIField(DisplayName = "Control Type", Visibility = PXUIVisibility.SelectorVisible)]
		[PXIntList(new int[] { Text, Combo, MultiSelectCombo, CheckBox, Datetime }, new string[] { "Text", "Combo", "Multi Select Combo", "Checkbox", "Datetime" })]
		public virtual Int32? ControlType
		{
			get
			{
				return this._ControlType;
			}
			set
			{
				this._ControlType = value;
			}
		}
		#endregion
		#region EntryMask
		public abstract class entryMask : PX.Data.IBqlField
		{
		}
		protected String _EntryMask;
		[PXDBString(60)]
		[PXUIField(DisplayName = "Entry Mask")]
		public virtual String EntryMask
		{
			get
			{
				return this._EntryMask;
			}
			set
			{
				this._EntryMask = value;
			}
		}
		#endregion
		#region RegExp
		public abstract class regExp : PX.Data.IBqlField
		{
		}
		protected String _RegExp;
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Reg. Exp.")]
		public virtual String RegExp
		{
			get
			{
				return this._RegExp;
			}
			set
			{
				this._RegExp = value;
			}
		}
		#endregion
		#region List
		public abstract class list : PX.Data.IBqlField
		{
		}
		protected String _List;
		[PXDBLocalizableString(IsUnicode = true)]
		public virtual String List
		{
			get
			{
				return this._List;
			}
			set
			{
				this._List = value;
			}
		}
		#endregion
        #region IsInternal

        public abstract class isInternal : PX.Data.IBqlField { }
        protected Boolean? _IsInternal;
        [PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Internal", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual Boolean? IsInternal
        {
            get
            {
                return this._IsInternal;
            }
            set
            {
                this._IsInternal = value;
            }
        }

		#endregion

		#region NoteID
		public abstract class noteID : IBqlField { }

		[PXNote]
		public virtual Guid? NoteID { get; set; }
		#endregion

		#region System Columns
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
		#endregion
	}
}
