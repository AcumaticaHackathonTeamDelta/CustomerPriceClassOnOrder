namespace PX.Objects.IN
{
	using PX.Data;
	using System;

/*	select 
	ts.JrnlType,
	ts.TranType,
	ts.RefNbr,
	ts.LineNbr,
	ts.SplitLineNbr,
	BaseQty = MAX (ts.BaseQty),
	EstimatedTranCost = MAX (ts.BaseQty) * SUM (tc.TranCost) / SUM (tc.Qty) 

	from INTranSplit ts

	join INCostSubItemXRef csixr on 
		csixr.SubItemId = ts.SubItemId

	LEFT join INLotSerialStatus lss ON 
		lss.InventoryId = ts.InventoryId and 
		lss.SubItemId = ts.SubItemId and 
		lss.SiteId = ts.SiteId and 
		lss.LocationId = ts.LocationId and 
		lss.LotSerialNbr = ts.LotSerialNbr

	join INTranCost tc ON 
		tc.CostId = lss.CostId
		OR
		( 
		lss.CostId IS NULL and 
		tc.TranType = ts.TranType and 
		tc.RefNbr = ts.RefNbr and 
		tc.LineNbr = ts.LineNbr and 
		(tc.CostSiteId = ts.SiteId or tc.CostSiteId = ts.LocationId) and 
		tc.CostSubItemId = csixr.CostSubItemId
		)

	-- where [additional conditions]

	group by -- back grouping to INTranSplit
	ts.JrnlType,
	ts.TranType, 
	ts.RefNbr,
	ts.LineNbr,
	ts.SplitLineNbr */

    [Obsolete()]
	[PXProjection(typeof(Select5<INTranSplit, 
		InnerJoin<INCostSubItemXRef,
			On<INCostSubItemXRef.subItemID, Equal<INTranSplit.subItemID>>,
		LeftJoin<INLotSerialStatus,
			On<INLotSerialStatus.inventoryID, Equal<INTranSplit.inventoryID>,
			And<INLotSerialStatus.subItemID, Equal<INTranSplit.subItemID>,
			And<INLotSerialStatus.siteID, Equal<INTranSplit.siteID>,
			And<INLotSerialStatus.locationID, Equal<INTranSplit.locationID>,
			And<INLotSerialStatus.lotSerialNbr, Equal<INTranSplit.lotSerialNbr>>>>>>,
		InnerJoin<INTranCost,
			On<INTranCost.costID, Equal<INLotSerialStatus.costID>,
			Or<Where<INLotSerialStatus.costID , IsNull,
			And<INTranCost.tranType, Equal<INTranSplit.tranType>,
			And<INTranCost.refNbr, Equal<INTranSplit.refNbr>,
			And<INTranCost.lineNbr, Equal<INTranSplit.lineNbr>,
			And<INTranCost.costSubItemID, Equal<INCostSubItemXRef.costSubItemID>,
			And<Where<INTranCost.costSiteID, Equal<INTranSplit.siteID>, 
			Or<INTranCost.costSiteID, Equal<INTranSplit.locationID>>>>>>>>>>>>>>,
		Aggregate<GroupBy<INTranSplit.docType, 
			GroupBy<INTranSplit.tranType, 
			GroupBy<INTranSplit.refNbr,
			GroupBy<INTranSplit.lineNbr,
			GroupBy<INTranSplit.splitLineNbr,
            Max<INTranSplit.tranDate,
            Max<INTranSplit.lastModifiedDateTime,
			Max<INTranSplit.baseQty,
			Sum<INTranCost.tranCost,
			Sum<INTranCost.qty>>>>>>>>>>>>))]
    [Serializable]
	public partial class INTranSplitCostDetail : PX.Data.IBqlTable
	{
		#region DocType
		public abstract class docType : PX.Data.IBqlField
		{
		}
		protected String _DocType;
		[PXDBString(1, IsFixed = true, IsKey = true, BqlField = typeof(INTranSplit.docType))]
		public virtual String DocType
		{
			get
			{
				return this._DocType;
			}
			set
			{
				this._DocType = value;
			}
		}
		#endregion
		#region TranType
		public abstract class tranType : PX.Data.IBqlField
		{
		}
		protected String _TranType;
		[PXDBString(3, IsKey = true, BqlField = typeof(INTranSplit.tranType))]
		public virtual String TranType
		{
			get
			{
				return this._TranType;
			}
			set
			{
				this._TranType = value;
			}
		}
		#endregion
		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
		}
		protected String _RefNbr;
		[PXDBString(15, IsUnicode = true, IsKey = true, BqlField = typeof(INTranSplit.refNbr))]
		public virtual String RefNbr
		{
			get
			{
				return this._RefNbr;
			}
			set
			{
				this._RefNbr = value;
			}
		}
		#endregion
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineNbr;
		[PXDBInt(IsKey = true, BqlField = typeof(INTranSplit.lineNbr))]
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
		#region SplitLineNbr
		public abstract class splitLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _SplitLineNbr;
		[PXDBInt(IsKey = true, BqlField = typeof(INTranSplit.splitLineNbr))]
		public virtual Int32? SplitLineNbr
		{
			get
			{
				return this._SplitLineNbr;
			}
			set
			{
				this._SplitLineNbr = value;
			}
		}
		#endregion

        #region TranDate
        public abstract class tranDate : PX.Data.IBqlField
        {
        }
        protected DateTime? _TranDate;
        [PXDBDate(BqlField = typeof(INTranSplit.tranDate))]
        public virtual DateTime? TranDate
        {
            get
            {
                return this._TranDate;
            }
            set
            {
                this._TranDate = value;
            }
        }
        #endregion

        #region LastModifiedDateTime
        public abstract class lastModifiedDateTime : PX.Data.IBqlField
        {
        }
        protected DateTime? _LastModifiedDateTime;
        [PXDBDate(BqlField = typeof(INTranSplit.lastModifiedDateTime))]
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
        
        #region BaseQty
		public abstract class baseQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseQty;
		[PXDBQuantity(BqlField = typeof(INTranSplit.baseQty))]
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

		#region TranCost
		public abstract class tranCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranCost;
		[PXDBDecimal(4, BqlField = typeof(INTranCost.tranCost))]
		public virtual Decimal? TranCost
		{
			get
			{
				return this._TranCost;
			}
			set
			{
				this._TranCost = value;
			}
		}
		#endregion

		#region Qty
		public abstract class qty : PX.Data.IBqlField
		{
		}
		protected Decimal? _Qty;
		[PXDBQuantity(BqlField = typeof(INTranCost.qty))]
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
	}
	
	/*
select * from INLocationStatus L
   inner join InventoryItem I on L.CompanyID = I.CompanyID 
                         and L.InventoryID = I.InventoryID 
   inner join INCostSubitemxref X on L.SubitemID = X.SubitemID
   inner join INLocation LC on L.SiteID = LC.SiteID
                           and L.LocationID=LC.LocationID
                           and L.QtyOnHand <> 0
   left  join INLotSerialStatus S on L.CompanyID = S.CompanyID 
                                 and L.InventoryID = S.InventoryID 
                                 and L.LocationID = S.LocationID
                                 and L.SiteID = S.SiteID
                                 and L.SubItemID = S.SubItemID
   inner join 
   (Select CompanyID, InventoryID, CostSiteID, CostSubItemID,  LotSerialNbr, 
			QtyOnHand = Sum(QtyOnHand), TotalCost = Sum(TotalCost) from INCostStatus 
   Group By CompanyID, InventoryID, CostSiteID, CostSiteID, LotSerialNbr, CostSubItemID) C 
				on L.CompanyID = C.CompanyID 
                and L.InventoryID = C.InventoryID 
                and X.CostSubItemID = C.CostSubItemID
                and ((I.ValMethod = 'S' and S.LotSerialNbr = C.LotSerialNbr) 
					or I.ValMethod <> 'S' or S.LotSerialNbr is null)
                and ((L.SiteID = C.CostSiteID and LC.Iscosted=0) 
					or (L.LocationID = C.CostSiteID and LC.Iscosted<>0))
	*/
    [Obsolete()]
    [PXProjection(typeof(Select2<INLocationStatus,
		InnerJoin<InventoryItem,
		   On<InventoryItem.inventoryID, Equal<INLocationStatus.inventoryID>>,
		InnerJoin<INCostSubItemXRef,
		   On<INCostSubItemXRef.subItemID, Equal<INLocationStatus.subItemID>>,
		InnerJoin<INLocation,
			On<INLocation.siteID, Equal<INLocationStatus.siteID>,
			And<INLocation.locationID, Equal<INLocationStatus.locationID>,
			And<INLocationStatus.qtyOnHand, NotEqual<CS.decimal0>>>>,
		LeftJoin<INLotSerialStatus,
			On<INLotSerialStatus.inventoryID, Equal<INLocationStatus.inventoryID>,
			And<INLotSerialStatus.subItemID, Equal<INLocationStatus.subItemID>,
			And<INLotSerialStatus.siteID, Equal<INLocationStatus.siteID>,
			And<INLotSerialStatus.locationID, Equal<INLocationStatus.locationID>>>>>,
	   InnerJoin<INCostStatusSubqueryEstimated,
		   On<INCostStatusSubqueryEstimated.inventoryID, Equal<INLocationStatus.inventoryID>,
			And<INCostStatusSubqueryEstimated.costSubItemID, Equal<INCostSubItemXRef.costSubItemID>,
			And2<
				Where<InventoryItem.valMethod, NotEqual<INValMethod.specific>,
				Or<INLotSerialStatus.lotSerialNbr, IsNull, 
				Or<
					Where<InventoryItem.valMethod, Equal<INValMethod.specific>,
					And<INLotSerialStatus.lotSerialNbr, Equal<INCostStatusSubqueryEstimated.lotSerialNbr>>>>>>,
			And<Where2<Where<INCostStatusSubqueryEstimated.costSiteID, Equal<INLocationStatus.siteID>,
					And<INLocation.isCosted, Equal<PX.Objects.CS.boolFalse>>>,
				Or<Where<INCostStatusSubqueryEstimated.costSiteID, Equal<INLocationStatus.locationID>, 
					And<INLocation.isCosted, NotEqual<PX.Objects.CS.boolFalse>>>>>>>>>>>>>>>))]
    [Serializable]
    [PXHidden]
	public partial class INEstimatedCost : PX.Data.IBqlTable
	{
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[StockItem(IsKey = true, BqlField = typeof(INLocationStatus.inventoryID))]
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
		[SubItem(IsKey = true, BqlField = typeof(INLocationStatus.subItemID))]
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
		[Site(IsKey = true, BqlField = typeof(INLocationStatus.siteID))]
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
		[Location(IsKey = true, BqlField = typeof(INLocationStatus.locationID))]
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
		[PXDBString(100, IsUnicode = true, IsKey = true, BqlField = typeof(INCostStatusSubqueryEstimated.lotSerialNbr))]
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

		#region TotalCost
		public abstract class totalCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _TotalCost;
		[PXDBDecimal(4, BqlField = typeof(INCostStatusSubqueryEstimated.totalCost))]
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
		[PXDBQuantity(BqlField = typeof(INCostStatusSubqueryEstimated.qtyOnHand))]
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
		#region QtyOnHand_LocStatus
		public abstract class qtyOnHand_LocStatus : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyOnHand_LocStatus;
		[PXDBQuantity(BqlField = typeof(INLocationStatus.qtyOnHand))]
		public virtual Decimal? QtyOnHand_LocStatus
		{
			get
			{
				return this._QtyOnHand_LocStatus;
			}
			set
			{
				this._QtyOnHand_LocStatus = value;
			}
		}
		#endregion
		#region EstimatedCost
		public abstract class estimatedCost : PX.Data.IBqlField
		{
		}
		[PXDBDecimal(4)]
		public virtual Decimal? EstimatedCost
		{
			[PXDependsOnFields(typeof(totalCost))]
			get
			{
				if (_QtyOnHand_CostStatus.HasValue && _QtyOnHand_CostStatus.Value == decimal.Zero)
					return decimal.Zero;

				return (_TotalCost /** _QtyOnHand_LocStatus*/) / _QtyOnHand_CostStatus;
			}
		}
		#endregion
	}

	/* query (1)
   select * from INLocationStatus L
	   inner join InventoryItem I on L.CompanyID = I.CompanyID 
							 and L.InventoryID = I.InventoryID 
	   inner join INCostSubitemxref X on L.SubitemID = X.SubitemID
	   inner join INLocation LC on L.SiteID = LC.SiteID
							   and L.LocationID=LC.LocationID
							   and L.QtyOnHand <> 0
	   left  join INLotSerialStatus S on L.CompanyID = S.CompanyID 
									 and L.InventoryID = S.InventoryID 
									 and L.LocationID = S.LocationID
									 and L.SiteID = S.SiteID
									 and L.SubItemID = S.SubItemID
	   inner join 
	   (Select CompanyID, InventoryID, CostSiteID, CostSubItemID,  LotSerialNbr, QtyOnHand = Sum(QtyOnHand), TotalCost = Sum(TotalCost) from INCostStatus 
	   Group By CompanyID, InventoryID, CostSiteID, CostSiteID, LotSerialNbr, CostSubItemID) C on L.CompanyID = C.CompanyID 
									 and L.InventoryID = C.InventoryID 
									 and X.CostSubItemID = C.CostSubItemID
									 and ((I.ValMethod = 'S' and S.LotSerialNbr = C.LotSerialNbr) or I.ValMethod <> 'S' or S.LotSerialNbr is null)
									 and ((L.SiteID = C.CostSiteID and LC.Iscosted=0) or (L.LocationID = C.CostSiteID and LC.Iscosted<>0))	
   query (2)
	 Select CompanyID, InventoryID, CostSiteID, CostSubItemID,  LotSerialNbr, 
			QtyOnHand = Sum(QtyOnHand), TotalCost = Sum(TotalCost) from INCostStatus 
	   Group By CompanyID, InventoryID, CostSiteID, LotSerialNbr, CostSubItemID
	 * 
	 * This PXProjection implements query (2) that used in query (1)
	 */
    [Obsolete()]
	[PXProjection(typeof(Select4<INCostStatus,
		Aggregate<GroupBy<INCostStatus.inventoryID, 
			GroupBy<INCostStatus.costSiteID, 
			GroupBy<INCostStatus.costSubItemID,
			GroupBy<INCostStatus.lotSerialNbr,
 			Sum<INCostStatus.totalCost,
			Sum<INCostStatus.qtyOnHand>>>>>>>>))]
    [Serializable]
    [PXHidden]
	public partial class INCostStatusSubqueryEstimated : PX.Data.IBqlTable
	{
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[PXDBInt(IsKey = true, BqlField = typeof(INCostStatus.inventoryID))]
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
		[PXDBInt(IsKey = true, BqlField = typeof(INCostStatus.costSubItemID))]
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
		[PXDBInt(IsKey = true, BqlField = typeof(INCostStatus.costSiteID))]
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
		#region LotSerialNbr
		public abstract class lotSerialNbr : PX.Data.IBqlField
		{
		}
		protected String _LotSerialNbr;
		[PXDBString(100, IsUnicode = true, IsKey = true, BqlField = typeof(INCostStatus.lotSerialNbr))]
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
		[PXDBQuantity(BqlField = typeof(INCostStatus.qtyOnHand))]
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
	}
}
