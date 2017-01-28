namespace PX.Objects.IN
{
	using System;
	using PX.Data;
	using PX.Objects.CS;
	
	[System.SerializableAttribute()]

	public partial class INKitSpecNonStkDet : PX.Data.IBqlTable
	{

		#region KitInventoryID
		public abstract class kitInventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _KitInventoryID;
        [Inventory(IsKey = true, DisplayName = "Inventory ID")]
		[PXRestrictor(typeof(Where<InventoryItem.kitItem, Equal<boolTrue>>), Messages.InventoryItemIsNotaKit)]
		[PXDefault(typeof(INKitSpecHdr.kitInventoryID))]
		public virtual Int32? KitInventoryID
		{
			get
			{
				return this._KitInventoryID;
			}
			set
			{
				this._KitInventoryID = value;
			}
		}
		#endregion
		#region RevisionID
		public abstract class revisionID : PX.Data.IBqlField
		{
		}
		protected String _RevisionID;
		[PXDBString(10, IsUnicode = true, IsKey = true)]
		[PXDefault(typeof(INKitSpecHdr.revisionID))]
		[PXParent(typeof(Select<INKitSpecHdr, Where<INKitSpecHdr.kitInventoryID, Equal<Current<INKitSpecNonStkDet.kitInventoryID>>,
			And<INKitSpecHdr.revisionID, Equal<Current<INKitSpecNonStkDet.revisionID>>>>>))]
		public virtual String RevisionID
		{
			get
			{
				return this._RevisionID;
			}
			set
			{
				this._RevisionID = value;
			}
		}
		#endregion
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineNbr;
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		[PXLineNbr(typeof(INKitSpecHdr))]
		public virtual Int32? LineNbr
		{
			get
			{
				return this._LineNbr;
			}
			set
			{
				this._LineNbr = value;
			}
		}
		#endregion
		#region CompInventoryID
		public abstract class compInventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _CompInventoryID;
		[NonStockItem(DisplayName = "Component ID")]
		[PXRestrictor(typeof(Where<InventoryItem.kitItem, Equal<boolFalse>>), Messages.NonStockKitInKit)]
		[PXDefault()]
		public virtual Int32? CompInventoryID
		{
			get
			{
				return this._CompInventoryID;
			}
			set
			{
				this._CompInventoryID = value;
			}
		}
		#endregion
			

		#region DfltCompQty
		public abstract class dfltCompQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _DfltCompQty;
		[PXDBQuantity(MinValue = 0)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Component Qty.")]
		public virtual Decimal? DfltCompQty
		{
			get
			{
				return this._DfltCompQty;
			}
			set
			{
				this._DfltCompQty = value;
			}
		}
		#endregion
		#region UOM
		public abstract class uOM : PX.Data.IBqlField
		{
		}
		protected String _UOM;
		[PXDefault(typeof(Search<InventoryItem.baseUnit, Where<InventoryItem.inventoryID, Equal<Current<INKitSpecNonStkDet.compInventoryID>>>>))]
		[INUnit(typeof(INKitSpecNonStkDet.compInventoryID))]
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
		#region AllowQtyVariation
		public abstract class allowQtyVariation : PX.Data.IBqlField
		{
		}
		protected Boolean? _AllowQtyVariation;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Allow Component Qty. Variance")]
		public virtual Boolean? AllowQtyVariation
		{
			get
			{
				return this._AllowQtyVariation;
			}
			set
			{
				this._AllowQtyVariation = value;
			}
		}
		#endregion
		#region MinCompQty
		public abstract class minCompQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _MinCompQty;
		[PXDBQuantity(MinValue = 0)]
		//[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Min. Component Qty.")]
		public virtual Decimal? MinCompQty
		{
			get
			{
				return this._MinCompQty;
			}
			set
			{
				this._MinCompQty = value;
			}
		}
		#endregion
		#region MaxCompQty
		public abstract class maxCompQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _MaxCompQty;
		[PXDBQuantity(MinValue = 0)]
		//[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Max. Component Qty.")]
		public virtual Decimal? MaxCompQty
		{
			get
			{
				return this._MaxCompQty;
			}
			set
			{
				this._MaxCompQty = value;
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
