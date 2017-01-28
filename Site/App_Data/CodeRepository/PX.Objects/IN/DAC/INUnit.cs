namespace PX.Objects.IN
{
	using System;
	using PX.Data;
	using PX.Objects.CS;
	
	[System.SerializableAttribute()]
	[PXPrimaryGraph(typeof(INUnitMaint))]
    [PXCacheName(Messages.InventoryUnitConversions)]
	public partial class INUnit : PX.Data.IBqlTable
	{
		#region UnitType
		public abstract class unitType : PX.Data.IBqlField
		{
		}
		protected Int16? _UnitType;
		[PXDBShort(IsKey = true)]
		[PXDefault((short)3)]
		[PXUIField(DisplayName="Unit Type", Visibility=PXUIVisibility.Invisible, Visible=false)]
		[PXIntList(new int[] {1,2,3}, new string[] {"Inventory Item","Item Class","Global"})]
		public virtual Int16? UnitType
		{
			get
			{
				return this._UnitType;
			}
			set
			{
				this._UnitType = value;
			}
		}
		#endregion
		#region ItemClassID
		public abstract class itemClassID : PX.Data.IBqlField
		{
		}
		protected String _ItemClassID;
		[PXDBString(10, IsUnicode = true, IsKey = true)]
		[PXDefault("")]
		[PXUIField(DisplayName = "Item Class ID", Visibility = PXUIVisibility.Invisible, Visible = false)]
		[PXParent(typeof(Select<INItemClass, Where<INItemClass.itemClassID, Equal<Current<INUnit.itemClassID>>>>))]
		public virtual String ItemClassID
		{
			get
			{
				return this._ItemClassID;
			}
			set
			{
				this._ItemClassID = value;
			}
		}
		#endregion
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[PXDBInt(IsKey = true)]
		[PXDefault(0)]
		[PXUIField(DisplayName = "Inventory ID", Visibility = PXUIVisibility.Invisible, Visible = false)]
		public virtual Int32? InventoryID
		{
			get
			{
				return this._InventoryID;
			}
			set
			{
				this._InventoryID = value;
			}
		}
		#endregion
		#region ToUnit
		public abstract class toUnit : PX.Data.IBqlField
		{
		}
		protected String _ToUnit;
		[PXDefault()]
		[INUnit(IsKey=true, DisplayName = "To Unit", Visibility = PXUIVisibility.Visible)]
		public virtual String ToUnit
		{
			get
			{
				return this._ToUnit;
			}
			set
			{
				this._ToUnit = value;
			}
		}
		#endregion
		#region SampleToUnit
		public abstract class sampleToUnit : PX.Data.IBqlField
		{
		}
		protected String _SampleToUnit;
		[PXString(6, IsUnicode = true, InputMask=">aaaaaa")]
		[PXUIField(DisplayName="To Unit", Visible=false)]
		public virtual String SampleToUnit
		{
			[PXDependsOnFields(typeof(toUnit))]
			get
			{
				return this._SampleToUnit ?? this._ToUnit;
			}
			set
			{
				this._SampleToUnit = value;
			}
		}
		#endregion
		#region FromUnit
		public abstract class fromUnit : PX.Data.IBqlField
		{
		}
		protected String _FromUnit;
		[PXDefault()]
		[INUnit(IsKey = true, DisplayName = "From Unit", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String FromUnit
		{
			get
			{
				return this._FromUnit;
			}
			set
			{
				this._FromUnit = value;
			}
		}
		#endregion
		#region UnitMultDiv
		public abstract class unitMultDiv : PX.Data.IBqlField
		{
		}
		protected String _UnitMultDiv;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(MultDiv.Multiply)]
		[PXUIField(DisplayName="Multiply/Divide", Visibility=PXUIVisibility.Visible)]
		[MultDiv.List()]
		public virtual String UnitMultDiv
		{
			get
			{
				return this._UnitMultDiv;
			}
			set
			{
				this._UnitMultDiv = value;
			}
		}
		#endregion
		#region UnitRate
		public abstract class unitRate : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnitRate;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal,"1.0")]
		[PXUIField(DisplayName="Conversion Factor", Visibility=PXUIVisibility.Visible)]
        [PXFormula(typeof(Validate<INUnit.unitMultDiv>))]
		public virtual Decimal? UnitRate
		{
			get
			{
				return this._UnitRate;
			}
			set
			{
				this._UnitRate = value;
			}
		}
		#endregion
		#region RecordID
		public abstract class recordID : PX.Data.IBqlField
		{
		}
		protected long? _RecordID;
		[PXDBLongIdentity()]
		public virtual long? RecordID
		{
			get
			{
				return this._RecordID;
			}
			set
			{
				this._RecordID = value;
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
	}
}
