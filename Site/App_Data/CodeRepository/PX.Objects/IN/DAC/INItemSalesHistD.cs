using System;
using PX.Data;

namespace PX.Objects.IN
{	
    [Serializable]
    [PXHidden]
	public partial class INItemSalesHistD : IBqlTable
	{
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[Inventory(IsKey = true)]
		[PXDefault]
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
		[SubItem(IsKey = true)]
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
		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SiteID;
		[Site(IsKey = true)]
		[PXDefault]
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
		#region DemandType1
		public abstract class demandType1 : PX.Data.IBqlField
		{
		}
		protected int? _DemandType1;
		[PXDBInt]		
		[PXUIField(DisplayName = "Working Day")]
		public virtual int? DemandType1
		{
			get
			{
				return this._DemandType1;
			}
			set
			{
				this._DemandType1 = value;
			}
		}
		#endregion
		#region DemandType2
		public abstract class demandType2 : PX.Data.IBqlField
		{
		}
		protected int? _DemandType2;
		[PXDBInt]
		[PXUIField(DisplayName = "Nont Working Day")]
		public virtual int? DemandType2
		{
			get
			{
				return this._DemandType2;
			}
			set
			{
				this._DemandType2 = value;
			}
		}
		#endregion
		#region SDate
		public abstract class sDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _SDate;
		[PXDBDate(IsKey = true)]
		public virtual DateTime? SDate
		{
			get
			{
				return this._SDate;
			}
			set
			{
				this._SDate = value;
			}
		}
		#endregion
		#region SYear
		public abstract class sYear : PX.Data.IBqlField
		{
		}
		protected int? _SYear;
		[PXDBInt]
		public virtual int? SYear
		{
			get
			{
				return this._SYear;
			}
			set
			{
				this._SYear = value;
			}
		}
		#endregion
		#region SMonth
		public abstract class sMonth : PX.Data.IBqlField
		{
		}
		protected int? _SMonth;
		[PXDBInt]
		public virtual int? SMonth
		{
			get
			{
				return this._SMonth;
			}
			set
			{
				this._SMonth = value;
			}
		}
		#endregion
		#region SQuater
		public abstract class sQuater : PX.Data.IBqlField
		{
		}
		protected int? _SQuater;
		[PXDBInt]
		public virtual int? SQuater
		{
			get
			{
				return this._SQuater;
			}
			set
			{
				this._SQuater = value;
			}
		}
		#endregion		
		#region SDay
		public abstract class sDay : PX.Data.IBqlField
		{
		}
		protected int? _SDay;
		[PXDBInt]
		public virtual int? SDay
		{
			get
			{
				return this._SDay;
			}
			set
			{
				this._SDay = value;
			}
		}
		#endregion
		#region SDayOfWeek
		public abstract class sDayOfWeek : PX.Data.IBqlField
		{
		}
		protected int? _SDayOfWeek;
		[PXDBInt]
		public virtual int? SDayOfWeek
		{
			get
			{
				return this._SDayOfWeek;
			}
			set
			{
				this._SDayOfWeek = value;
			}
		}
		#endregion
		#region QtyIssues
		public abstract class qtyIssues : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyIssues;
		[PXDBQuantity]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. Issues")]
		public virtual Decimal? QtyIssues
		{
			get
			{
				return this._QtyIssues;
			}
			set
			{
				this._QtyIssues = value;
			}
		}
		#endregion
		#region QtyExcluded
		public abstract class qtyExcluded : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyExcluded;
		[PXDBQuantity]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. Excluded")]
		public virtual Decimal? QtyExcluded
		{
			get
			{
				return this._QtyExcluded;
			}
			set
			{
				this._QtyExcluded = value;
			}
		}
		#endregion
		#region QtyLostSales
		public abstract class qtyLostSales : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyLostSales;
		[PXDBQuantity]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. Lost Sales")]
		public virtual Decimal? QtyLostSales
		{
			get
			{
				return this._QtyLostSales;
			}
			set
			{
				this._QtyLostSales = value;
			}
		}
		#endregion
		#region QtyPlanSales
		public abstract class qtyPlanSales : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyPlanSales;
		[PXDBQuantity]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. Plan Sales")]
		public virtual Decimal? QtyPlanSales
		{
			get
			{
				return this._QtyPlanSales;
			}
			set
			{
				this._QtyPlanSales = value;
			}
		}
		#endregion
		#region tstamp
		public abstract class Tstamp : PX.Data.IBqlField
		{
		}
		protected Byte[] _tstamp;
		[PXDBTimestamp]
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
