using System.Diagnostics;
using PX.Objects.CA;

namespace PX.Objects.CS
{
	using System;
	using PX.Data;
	
	[System.SerializableAttribute()]
	[DebuggerDisplay("[{AttributeID}, {EntityClassID}]: {Description}")]
	[PXPrimaryGraph(typeof(CSAttributeMaint))]
	[PXCacheName(Messages.AttributeGroup)]
	public partial class CSAttributeGroup : PX.Data.IBqlTable
	{
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
		#region EntityClassID
		public abstract class entityClassID : PX.Data.IBqlField
		{
		}
		protected String _EntityClassID;
		[PXDBString(30, IsUnicode = true, IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Entity Class ID", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String EntityClassID
		{
			get
			{
				return this._EntityClassID;
			}
			set
			{
				this._EntityClassID = value;
			}
		}
        #endregion
        #region EntityType
        public abstract class entityType : PX.Data.IBqlField
		{
		}
		[PXDBString(200, IsKey = true)]
		[PXUIField(DisplayName = "Type")]
		[PXDefault()]
		public virtual String EntityType { get; set; }
		#endregion
		#region SortOrder
		public abstract class sortOrder : PX.Data.IBqlField
		{
		}
		protected Int16? _SortOrder;
		[PXDBShort()]
		[PXUIField(DisplayName = "Sort Order")]
		public virtual Int16? SortOrder
		{
			get
			{
				return this._SortOrder;
			}
			set
			{
				this._SortOrder = value;
			}
		}
		#endregion

        #region Description
        public abstract class description : PX.Data.IBqlField
        {
        }
        protected String _Description;
        [PXString(60, IsUnicode = true)]
		[PXFormula(typeof(Selector<attributeID, CSAttribute.description>))]
        [PXUIField(DisplayName = "Description", Enabled = false, Visibility = PXUIVisibility.SelectorVisible)]
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
		#region Required
		public abstract class required : PX.Data.IBqlField
		{
		}
		protected Boolean? _Required;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Required", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Boolean? Required
		{
			get
			{
				return this._Required;
			}
			set
			{
				this._Required = value;
			}
		}
		#endregion
		#region ControlType
		public abstract class controlType : PX.Data.IBqlField
		{
		}
		protected Int32? _ControlType;
		[PXInt()]
		[PXFormula(typeof(Selector<CSAttributeGroup.attributeID, CSAttribute.controlType>))]
		[PXUIField(DisplayName = "Control Type", Enabled = false)]
		[PXIntList(new int[] { 1, 2, 6, 3, 4, 5 }, new string[] { "Text", "Combo", "Multi Select Combo", "Lookup", "Checkbox", "Datetime" })]
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
        #region DefaultValue
        public abstract class defaultValue: PX.Data.IBqlField
        {
        }
	    protected String _DefaultValue;
	    [PXDBString(255, IsUnicode = true)]
	    [PXUIField(DisplayName = "Default Value")]
        [DynamicValueValidation(typeof(Search<CSAttribute.regExp, Where<CSAttribute.attributeID, Equal<Current<CSAttributeGroup.attributeID>>>>))]
	    public virtual String DefaultValue
	    {
	        get
	        {
	            return this._DefaultValue;
	        }
            set
            {
                this._DefaultValue = value;
            }
	    }
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
