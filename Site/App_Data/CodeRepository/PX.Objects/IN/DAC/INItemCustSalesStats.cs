namespace PX.Objects.IN
{
	using System;
	using PX.Data;

	[System.SerializableAttribute()]
    [PXHidden]
	public partial class INItemCustSalesStats : PX.Data.IBqlTable
	{
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[PXDBInt(IsKey = true)]
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
		#region SubItemID
		public abstract class subItemID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubItemID;
		[PXDBInt(IsKey = true)]
		[PXDefault()]
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
		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SiteID;
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public virtual Int32? SiteID
		{
			get
			{
				return this._SiteID;
			}
			set
			{
				this._SiteID = value;
			}
		}
		#endregion
		#region BAccountID
		public abstract class bAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _BAccountID;
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public virtual Int32? BAccountID
		{
			get
			{
				return this._BAccountID;
			}
			set
			{
				this._BAccountID = value;
			}
		}
		#endregion

		#region LastDate
		public abstract class lastDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _LastDate;
		[PXDBDate]		
		public virtual DateTime? LastDate
		{
			get
			{
				return this._LastDate;
			}
			set
			{
				this._LastDate = value;
			}
		}
		#endregion		
		#region LastQty
		public abstract class lastQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _LastQty;
		[PXDBQuantity]
		[PXDefault(TypeCode.Decimal, "0.0")]		
		public virtual Decimal? LastQty
		{
			get
			{
				return this._LastQty;
			}
			set
			{
				this._LastQty = value;
			}
		}
		#endregion		
		#region LastUnitPrice
		public abstract class lastUnitPrice : PX.Data.IBqlField
		{
		}
		protected Decimal? _LastUnitPrice;
		[PXDBDecimal(6)]
		public virtual Decimal? LastUnitPrice
		{
			get
			{
				return this._LastUnitPrice;
			}
			set
			{
				this._LastUnitPrice = value;
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
