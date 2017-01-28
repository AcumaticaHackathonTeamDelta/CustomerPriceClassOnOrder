namespace PX.Objects.IN
{
	using System;
	using PX.Data;
	using PX.Objects.CS;

	[System.SerializableAttribute()]
    [PXHidden]
	public partial class INItemBox : PX.Data.IBqlTable
	{
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[PXDBDefault(typeof(InventoryItem.inventoryID))]
		[PXParent(typeof(Select<InventoryItem, Where<InventoryItem.inventoryID, Equal<Current<INItemBox.inventoryID>>>>))]
		[PXDBInt(IsKey = true)]
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
		#region BoxID
		public abstract class boxID : PX.Data.IBqlField
		{
		}
		protected String _BoxID;
		[PXSelector(typeof(CSBox.boxID))]
		[PXDBString(15, IsKey=true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXUIField(DisplayName = "Box ID", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String BoxID
		{
			get
			{
				return this._BoxID;
			}
			set
			{
				this._BoxID = value;
			}
		}
		#endregion
		#region UOM
		public abstract class uOM : PX.Data.IBqlField
		{
		}
		protected String _UOM;
		[INUnit(typeof(INItemBox.inventoryID), DisplayName = "UOM")]
		[PXDefault(typeof(Search<InventoryItem.salesUnit, Where<InventoryItem.inventoryID, Equal<Current<INItemBox.inventoryID>>>>))]
		public virtual String UOM
		{
			get
			{
				return this._UOM;
			}
			set
			{
				this._UOM = value;
			}
		}
		#endregion
		#region Qty
		public abstract class qty : PX.Data.IBqlField
		{
		}
		protected Decimal? _Qty;
		[PXDBQuantity(typeof(INItemBox.uOM), typeof(INItemBox.baseQty))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty.")]
		public virtual Decimal? Qty
		{
			get
			{
				return this._Qty;
			}
			set
			{
				this._Qty = value;
			}
		}
		#endregion
		#region BaseQty
		public abstract class baseQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseQty;
		[PXDBDecimal(6, MinValue = 0)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? BaseQty
		{
			get
			{
				return this._BaseQty;
			}
			set
			{
				this._BaseQty = value;
			}
		}
		#endregion
		#region MaxQty
		public abstract class maxQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _MaxQty;
		[PXDecimal(6, MinValue = 0)]
		[PXUIField(DisplayName = "Max. Qty", Enabled=false)]
		public virtual Decimal? MaxQty
		{
			get
			{
				return this._MaxQty;
			}
			set
			{
				this._MaxQty = value;
			}
		}
		#endregion

		
		#region System Columns
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXNote()]
		public virtual Guid? NoteID
		{
			get
			{
				return this._NoteID;
			}
			set
			{
				this._NoteID = value;
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

	[PXProjection(typeof(Select2<INItemBox,
			InnerJoin<CSBox, On<CSBox.boxID, Equal<INItemBox.boxID>>>>), new Type[] { typeof(INItemBox) } )]
	[Serializable]
	public partial class INItemBoxEx : INItemBox
	{
		public new abstract class inventoryID : PX.Data.IBqlField{}
		public new abstract class boxID : PX.Data.IBqlField{}
		public new abstract class uOM : PX.Data.IBqlField { }
		public new abstract class qty : PX.Data.IBqlField { }
		public new abstract class baseQty : PX.Data.IBqlField { }
		public new abstract class maxQty : PX.Data.IBqlField { }

		#region Description
		public abstract class description : PX.Data.IBqlField
		{
		}
		protected String _Description;
		[PXDBString(255, IsUnicode = true, BqlField = typeof(CSBox.description))]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
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
		#region CarrierBox
		public abstract class carrierBox : PX.Data.IBqlField
		{
		}
		protected String _CarrierBox;
		[PXDBString(60, IsUnicode = true, BqlField = typeof(CSBox.carrierBox))]
		[PXUIField(DisplayName = "Carrier's Package", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual String CarrierBox
		{
			get
			{
				return this._CarrierBox;
			}
			set
			{
				this._CarrierBox = value;
			}
		}
		#endregion
		#region MaxWeight
		public abstract class maxWeight : PX.Data.IBqlField
		{
		}
		protected Decimal? _MaxWeight;

		[PXDBDecimal(4, MinValue = 0, BqlField = typeof(CSBox.maxWeight))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Max. Weight", Visibility = PXUIVisibility.SelectorVisible, Enabled=false)]
		public virtual Decimal? MaxWeight
		{
			get
			{
				return this._MaxWeight;
			}
			set
			{
				this._MaxWeight = value;
			}
		}
		#endregion
		#region BoxWeight
		public abstract class boxWeight : PX.Data.IBqlField
		{
		}
		protected Decimal? _BoxWeight;

		[PXDBDecimal(4, MinValue = 0, BqlField = typeof(CSBox.boxWeight))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Box Weight", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual Decimal? BoxWeight
		{
			get
			{
				return this._BoxWeight;
			}
			set
			{
				this._BoxWeight = value;
			}
		}
		#endregion
		#region MaxVolume
		public abstract class maxVolume : PX.Data.IBqlField
		{
		}
		protected Decimal? _MaxVolume;

		[PXDBDecimal(4, MinValue = 0, BqlField = typeof(CSBox.maxVolume))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Max Volume", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual Decimal? MaxVolume
		{
			get
			{
				return this._MaxVolume;
			}
			set
			{
				this._MaxVolume = value;
			}
		}
		#endregion
		#region Length
		public abstract class length : PX.Data.IBqlField
		{
		}
		protected int? _Length;
		[PXDBInt(MinValue = 0, BqlField = typeof(CSBox.length))]
		[PXUIField(DisplayName = "Length", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual int? Length
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
		#region Width
		public abstract class width : PX.Data.IBqlField
		{
		}
		protected int? _Width;
		[PXDBInt(MinValue = 0, BqlField = typeof(CSBox.width))]
		[PXUIField(DisplayName = "Width", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual int? Width
		{
			get
			{
				return this._Width;
			}
			set
			{
				this._Width = value;
			}
		}
		#endregion
		#region Height
		public abstract class height : PX.Data.IBqlField
		{
		}
		protected int? _Height;
		[PXDBInt(MinValue = 0, BqlField = typeof(CSBox.height))]
		[PXUIField(DisplayName = "Height", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual int? Height
		{
			get
			{
				return this._Height;
			}
			set
			{
				this._Height = value;
			}
		}
		#endregion

		#region MaxNetWeight
		public virtual Decimal MaxNetWeight
		{
			get
			{
				return MaxWeight.GetValueOrDefault() - BoxWeight.GetValueOrDefault();
			}
		}
		#endregion
	}
}
