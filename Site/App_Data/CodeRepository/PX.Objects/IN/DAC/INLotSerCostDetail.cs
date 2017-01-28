namespace PX.Objects.IN
{
	using PX.Data;
	using System;
	/*
	select 
	lss.InventoryID,
	lss.SubItemID,
	lss.SiteID,
	lss.LocationID,
	lss.LotSerialNbr,
	QtyOnHand = MAX (lss.QtyOnHand),
	UnitCost = SUM (cs.totalCost) / SUM (cs.QtyOnHand),
	TotalCost = MAX (lss.QtyOnHand) * SUM (cs.totalCost) / SUM (cs.QtyOnHand)

	from 
	INLotSerialStatus lss

	join INLocation l 
	on l.LocationId = lss.LocationId 
	and l.SiteId = lss.SiteId 

	join INCostSubItemXRef csixr
	on csixr.SubItemId = lss.SubItemId

	join INCostStatus cs
	on
	cs.CostID = lss.CostID
	or 
	(
	lss.CostID is null
	and cs.InventoryID = lss.InventoryID
	and cs.CostSubItemID = csixr.CostSubItemID
	and 
	(
	(cs.CostSiteID = lss.SiteID and l.IsCosted = 0)
	or (cs.CostSiteID = lss.LocationID and l.IsCosted <> 0) -- ����� ��������� �������� 
	)
	and 
	(
	cs.LotSerialNbr = lss.LotSerialNbr 
	or cs.LotSerialNbr is null
	) 
	)
	where lss.QtyOnHand <> 0 -- ����� �������� div0
	group by 
	lss.InventoryID, 
	lss.SubItemID,
	lss.SiteID,
	lss.LocationID,
	lss.LotSerialNbr*/

    [Obsolete()]
	[PXProjection(typeof(Select5<INLotSerialStatus,
		InnerJoin<INLocation,
			On<INLocation.locationID, Equal<INLotSerialStatus.locationID>,
			And<INLocation.siteID, Equal<INLotSerialStatus.siteID>>>,
		InnerJoin<INCostSubItemXRef,
			On<INCostSubItemXRef.subItemID, Equal<INLotSerialStatus.subItemID>>,
		InnerJoin<INCostStatus,
			On<INCostStatus.costID, Equal<INLotSerialStatus.costID>,
			Or<Where<INLotSerialStatus.costID, IsNull,
			And<INCostStatus.inventoryID, Equal<INLotSerialStatus.inventoryID>,
			And<INCostStatus.costSubItemID, Equal<INCostSubItemXRef.costSubItemID>,
			And2<Where<INCostStatus.costSiteID, Equal<INLotSerialStatus.siteID>, 
							And<INLocation.isCosted, Equal<CS.boolFalse>,
					Or<INCostStatus.costSiteID, Equal<INLotSerialStatus.locationID>, 
							And<INLocation.isCosted, NotEqual<CS.boolFalse>>>>>,
			And<Where<INCostStatus.lotSerialNbr, Equal<INLotSerialStatus.lotSerialNbr>,
				Or<INCostStatus.lotSerialNbr, IsNull>>>>>>>>>>>>,
		Where<INLotSerialStatus.qtyOnHand, NotEqual<CS.decimal0>>,
		Aggregate<GroupBy<INLotSerialStatus.inventoryID, 
			GroupBy<INLotSerialStatus.subItemID, 
			GroupBy<INLotSerialStatus.siteID,
			GroupBy<INLotSerialStatus.locationID,
			GroupBy<INLotSerialStatus.lotSerialNbr,
            Max<INLotSerialStatus.qtyOnHand,
            Sum<INCostStatus.totalCost,
			Sum<INCostStatus.qtyOnHand>>>>>>>>>>))]
    [Serializable]
    [PXHidden]
	public partial class INLotSerCostDetail : PX.Data.IBqlTable
	{
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[StockItem(IsKey = true, BqlField = typeof(INLotSerialStatus.inventoryID))]
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
		[SubItem(IsKey = true, BqlField = typeof(INLotSerialStatus.subItemID))]
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
		[Site(IsKey = true, BqlField = typeof(INLotSerialStatus.siteID))]
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
		#region LocationID
		public abstract class locationID : PX.Data.IBqlField
		{
		}
		protected Int32? _LocationID;
		[Location(typeof(INLotSerCostDetail.siteID), IsKey = true, BqlField = typeof(INLotSerialStatus.locationID))]
		public virtual Int32? LocationID
		{
			get
			{
				return this._LocationID;
			}
			set
			{
				this._LocationID = value;
			}
		}
		#endregion
		#region LotSerialNbr
		public abstract class lotSerialNbr : PX.Data.IBqlField
		{
		}
		protected String _LotSerialNbr;
		[PXDBString(100, IsUnicode = true, IsKey = true, BqlField = typeof(INLotSerialStatus.lotSerialNbr))]
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

		#region QtyOnHand
		public abstract class qtyOnHand : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyOnHand;
		[PXDBQuantity(BqlField = typeof(INLotSerialStatus.qtyOnHand))]
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
		#region TotalCost
		public abstract class totalCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _TotalCost;
		[PXDBDecimal(4, BqlField = typeof(INCostStatus.totalCost))]
		public virtual Decimal? TotalCost
		{
			get
			{
				return this._TotalCost;
			}
			set
			{
				this._TotalCost = value;
			}
		}
		#endregion
		#region QtyOnHand_CostStatus
		public abstract class qtyOnHand_CostStatus : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyOnHand_CostStatus;
		[PXDBQuantity(BqlField = typeof(INCostStatus.qtyOnHand))]
		public virtual Decimal? QtyOnHand_CostStatus
		{
			get
			{
				return this._QtyOnHand_CostStatus;
			}
			set
			{
				this._QtyOnHand_CostStatus = value;
			}
		}
		#endregion

	}
}
