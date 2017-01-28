namespace PX.Objects.IN
{
	using System;
	using PX.Data;
	using PX.Objects.GL;

	[Serializable()]
    [PXHidden]
	public partial class INReceiptStatus : PX.Data.IBqlTable
	{
		#region ReceiptID
		public abstract class receiptID : PX.Data.IBqlField
		{
		}
		protected Int64? _ReceiptID;
		[PXDBLongIdentity(IsKey = true)]
		[PXDefault()]
		public virtual Int64? ReceiptID
		{
			get
			{
				return this._ReceiptID;
			}
			set
			{
				this._ReceiptID = value;
			}
		}
		#endregion
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[StockItem()]
		[PXDefault()]
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
		#region CostSubItemID
		public abstract class costSubItemID : PX.Data.IBqlField
		{
		}
		protected Int32? _CostSubItemID;
		[SubItem()]
		[PXDefault()]
		public virtual Int32? CostSubItemID
		{
			get
			{
				return this._CostSubItemID;
			}
			set
			{
				this._CostSubItemID = value;
			}
		}
		#endregion
		#region CostSiteID
		public abstract class costSiteID : PX.Data.IBqlField
		{
		}
		protected Int32? _CostSiteID;
		[PXDBInt()]
		[PXDefault()]
		public virtual Int32? CostSiteID
		{
			get
			{
				return this._CostSiteID;
			}
			set
			{
				this._CostSiteID = value;
			}
		}
        #endregion
        #region AccountID
        public abstract class accountID : PX.Data.IBqlField
        {
        }
        protected Int32? _AccountID;

        [Account(Visibility = PXUIVisibility.Invisible)]
        [PXDefault()]
        public virtual Int32? AccountID
        {
            get
            {
                return this._AccountID;
            }
            set
            {
                this._AccountID = value;
            }
        }
        #endregion
        #region SubID
        public abstract class subID : PX.Data.IBqlField
        {
        }
        protected Int32? _SubID;

        [SubAccount(typeof(INReceiptStatus.accountID), Visibility = PXUIVisibility.Invisible)]
        [PXDefault()]
        public virtual Int32? SubID
        {
            get
            {
                return this._SubID;
            }
            set
            {
                this._SubID = value;
            }
        }
        #endregion
        #region LayerType
        public abstract class layerType : PX.Data.IBqlField
		{
		}
		protected String _LayerType;
		[PXDBString(1, IsFixed = true)]
		[PXDefault()]
		public virtual String LayerType
		{
			get
			{
				return this._LayerType;
			}
			set
			{
				this._LayerType = value;
			}
		}
		#endregion
		#region ValMethod
		public abstract class valMethod : PX.Data.IBqlField
		{
		}
		protected String _ValMethod;
		[PXDBString(1, IsFixed = true)]
		[PXDefault()]
		public virtual String ValMethod
		{
			get
			{
				return this._ValMethod;
			}
			set
			{
				this._ValMethod = value;
			}
		}
		#endregion
		#region ReceiptNbr
		public abstract class receiptNbr : PX.Data.IBqlField
		{
		}
		protected String _ReceiptNbr;
		[PXDBString(15, IsUnicode = true)]
		[PXDefault()]
		public virtual String ReceiptNbr
		{
			get
			{
				return this._ReceiptNbr;
			}
			set
			{
				this._ReceiptNbr = value;
			}
		}
		#endregion
        #region LotSerialNbr
        public abstract class lotSerialNbr : PX.Data.IBqlField
        {
        }
        protected String _LotSerialNbr;
        [PXDBString(100, IsUnicode = true)]
        [PXDefault("")]
        public virtual String LotSerialNbr
        {
            get
            {
                return this._LotSerialNbr;
            }
            set
            {
                this._LotSerialNbr = value;
            }
        }
        #endregion
		#region ReceiptDate
		public abstract class receiptDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ReceiptDate;
		[PXDBDate()]
		[PXDefault()]
		public virtual DateTime? ReceiptDate
		{
			get
			{
				return this._ReceiptDate;
			}
			set
			{
				this._ReceiptDate = value;
			}
		}
		#endregion
		#region OrigQty
		public abstract class origQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _OrigQty;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? OrigQty
		{
			get
			{
				return this._OrigQty;
			}
			set
			{
				this._OrigQty = value;
			}
		}
        #endregion
        #region QtyOnHand
        public abstract class qtyOnHand : PX.Data.IBqlField
        {
        }
        protected Decimal? _QtyOnHand;
        [PXDBQuantity()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? QtyOnHand
        {
            get
            {
                return this._QtyOnHand;
            }
            set
            {
                this._QtyOnHand = value;
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
