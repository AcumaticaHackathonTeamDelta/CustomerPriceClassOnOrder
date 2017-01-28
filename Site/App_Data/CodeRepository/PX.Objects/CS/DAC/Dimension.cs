using PX.Objects.CR;

namespace PX.Objects.CS
{
	using System;
	using PX.Data;

	[Serializable]
    [PXCacheName(Messages.Dimension)]
	[PXPrimaryGraph(
		new Type[] { typeof(DimensionMaint)},
		new Type[] { typeof(Select<Dimension, 
			Where<Dimension.dimensionID, Equal<Current<Dimension.dimensionID>>>>)
		})]
	public partial class Dimension : PX.Data.IBqlTable
	{
		#region DimensionID
		public abstract class dimensionID : PX.Data.IBqlField
		{
		}
		protected String _DimensionID;
		[PXDBString(15, IsUnicode = true, IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Segmented Key ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<Dimension.dimensionID, Where<Dimension.dimensionID, InFieldClassActivated>>))]
        public virtual String DimensionID
		{
			get
			{
				return this._DimensionID;
			}
			set
			{
				this._DimensionID = value;
			}
		}
		#endregion
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;
		[PXDBString(60, IsUnicode = true)]
		[PXDefault("")]
		[PXUIField(DisplayName = "Description", Visibility =PXUIVisibility.SelectorVisible)]
		public virtual String Descr
		{
			get
			{
				return this._Descr;
			}
			set
			{
				this._Descr = value;
			}
		}
		#endregion
		#region Length
		public abstract class length : PX.Data.IBqlField
		{
		}
		protected Int16? _Length;
		[PXDBShort()]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "Length", Visibility = PXUIVisibility.Visible)]
		public virtual Int16? Length
		{
			get
			{
				return this._Length;
			}
			set
			{
				this._Length = value;
			}
		}
		#endregion
        #region MaxLength
        public abstract class maxLength : PX.Data.IBqlField
        {
        }
        protected int? _maxLength;
        [PXInt]
        [PXUIField(DisplayName = "Max Length", Enabled = false)]
        public virtual int? MaxLength
        {
            get { return _maxLength; }
            set { _maxLength = value; }
        }
        #endregion
        #region Segments
        public abstract class segments : PX.Data.IBqlField
		{
		}
		protected Int16? _Segments;
		[PXDBShort()]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "Segments", Visibility = PXUIVisibility.Visible)]
		public virtual Int16? Segments
		{
			get
			{
				return this._Segments;
			}
			set
			{
				this._Segments = value;
			}
		}
		#endregion
		#region Internal
		public abstract class @internal : PX.Data.IBqlField
		{
		}
		protected Boolean? _Internal;
		[PXDBBool()]
		[PXDefault((bool) false)]
		public virtual Boolean? Internal
		{
			get
			{
				return this._Internal;
			}
			set
			{
				this._Internal = value;
			}
		}
		#endregion
		#region NumberingID
		public abstract class numberingID : PX.Data.IBqlField
		{
		}
		protected String _NumberingID;
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName="Numbering ID", Visibility = PXUIVisibility .Visible)]
		[PXSelector(typeof(Numbering.numberingID))]
		public virtual String NumberingID
		{
			get
			{
				return this._NumberingID;
			}
			set
			{
				this._NumberingID = value;
			}
		}
		#endregion
		#region Validate
		public abstract class validate : PX.Data.IBqlField
		{
		}
		protected Boolean? _Validate;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "On-The-Fly Entry")]
		public virtual Boolean? Validate
		{
			get
			{
				return this._Validate;
			}
			set
			{
				this._Validate = value;
			}
		}
		#endregion
		#region SpecificModule
		public abstract class specificModule : PX.Data.IBqlField
		{
		}
		protected String _SpecificModule;
		[PXDBString(255)]
		[PX.SM.RelationGroup.ModuleAll]
		[PXUIField(DisplayName = "Specific Module")]
		public virtual String SpecificModule
		{
			get
			{
				return this._SpecificModule;
			}
			set
			{
				this._SpecificModule = value;
			}
		}
		#endregion

		#region ParentDimensionID

		public abstract class parentDimensionID : IBqlField
		{
		}

		[PXDBString]
		[PXUIField(DisplayName = "Parent", Enabled = false)]
        [PXNavigateSelector(typeof(Dimension.dimensionID))]
		public virtual String ParentDimensionID { get; set; }

		#endregion

        /*
        #region TableName
        public abstract class tableName : PX.Data.IBqlField
        {
        }
        protected String _TableName;
        [PXDBString(128, IsUnicode = true)]
        [PXUIField(DisplayName = "Table Name", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual String TableName
        {
            get
            {
                return this._TableName;
            }
            set
            {
                this._TableName = value;
            }
        }
        #endregion
        #region FieldName
        public abstract class fieldName : PX.Data.IBqlField
        {
        }
        protected String _FieldName;
        [PXDBString(128, IsUnicode = true)]
        [PXUIField(DisplayName = "Field Name", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual String FieldName
        {
            get
            {
                return this._FieldName;
            }
            set
            {
                this._FieldName = value;
            }
        }
        #endregion
        #region DividerField
        public abstract class dividerField : PX.Data.IBqlField
        {
        }
        protected String _DividerField;
        [PXDBString(128, IsUnicode = true)]
        [PXUIField(DisplayName = "Divider Field Name", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual String DividerField
        {
            get
            {
                return this._DividerField;
            }
            set
            {
                this._DividerField = value;
            }
        }
        #endregion
        #region DividerValue
        public abstract class dividerValue : PX.Data.IBqlField
        {
        }
        protected bool? _DividerValue;
        [PXDBBool()]
        [PXUIField(DisplayName = "Divider Value", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual bool? DividerValue
        {
            get
            {
                return this._DividerValue;
            }
            set
            {
                this._DividerValue = value;
            }
        }
        #endregion*/

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
	}
}
