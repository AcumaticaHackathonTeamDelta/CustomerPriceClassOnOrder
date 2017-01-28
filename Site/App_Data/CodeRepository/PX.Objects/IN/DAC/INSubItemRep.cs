using System;
using PX.Data;

namespace PX.Objects.IN
{
    [PXCacheName(Messages.SubitemReplenishmentSettings)]
    [Serializable]
	public partial class INSubItemRep : IBqlTable
	{
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[StockItem(IsKey = true, DirtyRead = true, DisplayName = "Inventory ID", Visible = false)]
		[PXParent(typeof(Select<INItemRep, 
			Where<INItemRep.inventoryID, Equal<Current<INSubItemRep.inventoryID>>,
			And<INItemRep.replenishmentClassID, Equal<Current<INSubItemRep.replenishmentClassID>>>>>))]
		[PXDBDefault(typeof(InventoryItem.inventoryID))]
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
		#region ReplenishmentClassID
		public abstract class replenishmentClassID : PX.Data.IBqlField
		{
		}
		protected String _ReplenishmentClassID;
		[PXDefault(typeof(INItemRep.replenishmentClassID))]
		[PXDBString(10, IsUnicode = true, IsKey = true, InputMask = ">aaaaaaaaaa")]
		[PXUIField(DisplayName = "Replenishment Class ID", Visible = false)]		
		public virtual String ReplenishmentClassID
		{
			get
			{
				return this._ReplenishmentClassID;
			}
			set
			{
				this._ReplenishmentClassID = value;
			}
		}
		#endregion
		#region SubItemID
		public abstract class subItemID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubItemID;
		[SubItem(typeof(INSubItemRep.inventoryID), DisplayName = "Subitem", IsKey = true)]
		[PXDefault]
		public virtual Int32? SubItemID
		{
			get
			{
				return this._SubItemID;
			}
			set
			{
				this._SubItemID = value;
			}
		}
		#endregion
		#region SafetyStock
		public abstract class safetyStock : PX.Data.IBqlField
		{
		}
		protected Decimal? _SafetyStock;
		[PXDBQuantity]
		[PXUIField(DisplayName = "Safety Stock")]
		[PXDefault(TypeCode.Decimal, "0.0",
			typeof(Select<INItemRep,
			Where<INItemRep.inventoryID, Equal<Current<INSubItemRep.inventoryID>>,
			And<INItemRep.replenishmentClassID, Equal<Current<INSubItemRep.replenishmentClassID>>>>>),
			SourceField = typeof(INItemRep.safetyStock))]
		public virtual Decimal? SafetyStock
		{
			get
			{
				return this._SafetyStock;
			}
			set
			{
				this._SafetyStock = value;
			}
		}
		#endregion
		#region MinQty
		public abstract class minQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _MinQty;
		[PXDBQuantity]
		[PXUIField(DisplayName = "Reorder Point")]
		[PXDefault(TypeCode.Decimal, "0.0",
				typeof(Select<INItemRep,
				Where<INItemRep.inventoryID, Equal<Current<INSubItemRep.inventoryID>>,
				And<INItemRep.replenishmentClassID, Equal<Current<INSubItemRep.replenishmentClassID>>>>>),
				SourceField = typeof(INItemRep.minQty))]
		public virtual Decimal? MinQty
		{
			get
			{
				return this._MinQty;
			}
			set
			{
				this._MinQty = value;
			}
		}
		#endregion
		#region MaxQty
		public abstract class maxQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _MaxQty;
		[PXDBQuantity]
		[PXUIField(DisplayName = "Max Qty.")]
		[PXDefault(TypeCode.Decimal, "0.0",
			typeof(Select<INItemRep,
			Where<INItemRep.inventoryID, Equal<Current<INSubItemRep.inventoryID>>,
			And<INItemRep.replenishmentClassID, Equal<Current<INSubItemRep.replenishmentClassID>>>>>),
			SourceField = typeof(INItemRep.maxQty))]
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
		#region TransferERQ
		public abstract class transferERQ : PX.Data.IBqlField
		{
		}
		protected Decimal? _TransferERQ;
		[PXDBQuantity]		
		[PXUIField(DisplayName = "Transfer ERQ")]
		[PXDefault(TypeCode.Decimal, "0.0",
				typeof(Select<INItemRep,
				Where<INItemRep.inventoryID, Equal<Current<INSubItemRep.inventoryID>>,
				And<INItemRep.replenishmentClassID, Equal<Current<INSubItemRep.replenishmentClassID>>>>>),
				SourceField = typeof(INItemRep.transferERQ), PersistingCheck = PXPersistingCheck.Nothing )]
		public virtual Decimal? TransferERQ
		{
			get
			{
				return this._TransferERQ;
			}
			set
			{
				this._TransferERQ = value;
			}
		}
		#endregion
		#region ItemStatus
		public abstract class itemStatus : PX.Data.IBqlField
		{
		}
		protected String _ItemStatus;
		[PXDBString(2, IsFixed = true)]		
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible)]
		[InventoryItemStatus.SubItemList]
		[PXDefault(InventoryItemStatus.Active)]
		public virtual String ItemStatus
		{
			get
			{
				return this._ItemStatus;
			}
			set
			{
				this._ItemStatus = value;
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
