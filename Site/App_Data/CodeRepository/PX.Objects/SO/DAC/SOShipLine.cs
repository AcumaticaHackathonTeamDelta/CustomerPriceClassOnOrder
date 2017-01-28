namespace PX.Objects.SO
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using PX.Data;
	using PX.Objects.CM;
	using PX.Objects.IN;
	using PX.Objects.CS;
	using PX.Objects.GL;
	using PX.Objects.TX;
	using PX.Objects.AR;
	using POReceiptLine = PX.Objects.PO.POReceiptLine;

	[System.SerializableAttribute()]
	[PXCacheName(Messages.SOShipLine)]
	public partial class SOShipLine : PX.Data.IBqlTable, ILSPrimary, IDiscountable
	{
		#region ShipmentNbr
		public abstract class shipmentNbr : PX.Data.IBqlField
		{
		}
		protected String _ShipmentNbr;
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "")]
		[PXDBDefault(typeof(SOShipment.shipmentNbr))]
		[PXUIField(DisplayName = "Shipment Nbr.", Visible = false, Enabled = false)]
		[PXParent(typeof(Select<SOShipment, Where<SOShipment.shipmentNbr, Equal<Current<SOShipLine.shipmentNbr>>>>))]
		[PXParent(typeof(Select<SOOrderShipment, Where<SOOrderShipment.shipmentNbr, Equal<Current<SOShipLine.shipmentNbr>>, And<SOOrderShipment.shipmentType, Equal<Current<SOShipLine.shipmentType>>, And<SOOrderShipment.orderType, Equal<Current<SOShipLine.origOrderType>>, And<SOOrderShipment.orderNbr, Equal<Current<SOShipLine.origOrderNbr>>>>>>>), LeaveChildren = true)]
		[PXFormula(null, typeof(CountCalc<SOOrderShipment.lineCntr>))]
		public virtual String ShipmentNbr
		{
			get
			{
				return this._ShipmentNbr;
			}
			set
			{
				this._ShipmentNbr = value;
			}
		}
		#endregion
		#region ShipmentType
		public abstract class shipmentType : PX.Data.IBqlField
		{
		}
		protected String _ShipmentType;
		[PXDBString(1, IsFixed = true, IsKey = true)]
		[PXDefault(typeof(SOShipment.shipmentType))]
		public virtual String ShipmentType
		{
			get
			{
				return this._ShipmentType;
			}
			set
			{
				this._ShipmentType = value;
			}
		}
		#endregion
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineNbr;
		[PXDBInt(IsKey = true)]
		[PXLineNbr(typeof(SOShipment.lineCntr))]
		[PXUIField(DisplayName = "Line Nbr.", Visible = false)]
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
		#region CustomerID
		public abstract class customerID : PX.Data.IBqlField
		{
		}
		protected Int32? _CustomerID;
		[PXDBInt()]
		[PXDefault(typeof(SOShipment.customerID))]
		public virtual Int32? CustomerID
		{
			get
			{
				return this._CustomerID;
			}
			set
			{
				this._CustomerID = value;
			}
		}
		#endregion
		#region ShipDate
		public abstract class shipDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ShipDate;
		[PXDBDate()]
		[PXDBDefault(typeof(SOShipment.shipDate))]
		public virtual DateTime? ShipDate
		{
			get
			{
				return this._ShipDate;
			}
			set
			{
				this._ShipDate = value;
			}
		}
		#endregion
		#region Confirmed
		public abstract class confirmed : PX.Data.IBqlField
		{
		}
		protected Boolean? _Confirmed;
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? Confirmed
		{
			get
			{
				return this._Confirmed;
			}
			set
			{
				this._Confirmed = value;
			}
		}
		#endregion
		#region Released
		public abstract class released : PX.Data.IBqlField
		{
		}
		protected Boolean? _Released;
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? Released
		{
			get
			{
				return this._Released;
			}
			set
			{
				this._Released = value;
			}
		}
		#endregion
		#region LineType
		public abstract class lineType : PX.Data.IBqlField
		{
		}
		protected String _LineType;
		[PXDBString(2, IsFixed = true)]
		[PXDefault()]
		public virtual String LineType
		{
			get
			{
				return this._LineType;
			}
			set
			{
				this._LineType = value;
			}
		}
		#endregion
		#region OrigOrderType
		public abstract class origOrderType : PX.Data.IBqlField
		{
		}
		protected String _OrigOrderType;
		[PXDBString(2, IsFixed = true)]
		[PXUIField(DisplayName = "Order Type", Enabled = false)]
		public virtual String OrigOrderType
		{
			get
			{
				return this._OrigOrderType;
			}
			set
			{
				this._OrigOrderType = value;
			}
		}
		#endregion
		#region OrigOrderNbr
		public abstract class origOrderNbr : PX.Data.IBqlField
		{
		}
		protected String _OrigOrderNbr;
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Order Nbr.", Enabled = false)]
		[PXParent(typeof(Select<SOOrder, Where<SOOrder.orderType, Equal<Current<SOShipLine.origOrderType>>, And<SOOrder.orderNbr, Equal<Current<SOShipLine.origOrderNbr>>>>>))]
		public virtual String OrigOrderNbr
		{
			get
			{
				return this._OrigOrderNbr;
			}
			set
			{
				this._OrigOrderNbr = value;
			}
		}
		#endregion
		#region OrigLineNbr
		public abstract class origLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _OrigLineNbr;
		[PXDBInt()]
		[PXUIField(DisplayName = "Order Line Nbr.", Visible = false, Enabled = false)]
		[PXParent(typeof(Select<SOLine2, Where<SOLine2.orderType, Equal<Current<SOShipLine.origOrderType>>, And<SOLine2.orderNbr, Equal<Current<SOShipLine.origOrderNbr>>, And<SOLine2.lineNbr, Equal<Current<SOShipLine.origLineNbr>>>>>>))]
		public virtual Int32? OrigLineNbr
		{
			get
			{
				return this._OrigLineNbr;
			}
			set
			{
				this._OrigLineNbr = value;
			}
		}
		#endregion
		#region OrigSplitLineNbr
		public abstract class origSplitLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _OrigSplitLineNbr;
		[PXDBInt()]
		[PXUIField(DisplayName = "Split Line Nbr.", Visible = false, Enabled = false)]
		[PXParent(typeof(Select<SOLineSplit2, Where<SOLineSplit2.orderType, Equal<Current<SOShipLine.origOrderType>>, And<SOLineSplit2.orderNbr, Equal<Current<SOShipLine.origOrderNbr>>, And<SOLineSplit2.lineNbr, Equal<Current<SOShipLine.origLineNbr>>, And<SOLineSplit2.splitLineNbr, Equal<Current<SOShipLine.origSplitLineNbr>>>>>>>))]
		public virtual Int32? OrigSplitLineNbr
		{
			get
			{
				return this._OrigSplitLineNbr;
			}
			set
			{
				this._OrigSplitLineNbr = value;
			}
		}
		#endregion
		#region Operation
		public abstract class operation : PX.Data.IBqlField
		{
		}
		protected String _Operation;
		[PXDBString(1, IsFixed = true, InputMask = ">a")]
		[PXDefault()]
		[PXSelector(typeof(Search<SOOrderTypeOperation.operation, Where<SOOrderTypeOperation.orderType, Equal<Current<SOShipLine.origOrderType>>>>))]
		public virtual String Operation
		{
			get
			{
				return this._Operation;
			}
			set
			{
				this._Operation = value;
			}
		}
		#endregion
		#region OrigPlanType
		public abstract class origPlanType : PX.Data.IBqlField
		{
		}
		[PXDBString(2, IsFixed = true)]
		[PXDefault()]
		[PXSelector(typeof(Search<INPlanType.planType>), CacheGlobal = true)]
		public virtual String OrigPlanType
		{
			get;
			set;
		}
		#endregion
		#region InvtMult
		public abstract class invtMult : PX.Data.IBqlField
		{
		}
		protected Int16? _InvtMult;
		[PXDBShort()]
		[PXDefault((short)-1)]
		[PXUIField(DisplayName = "Inventory Multiplier")]
		public virtual Int16? InvtMult
		{
			get
			{
				return this._InvtMult;
			}
			set
			{
				this._InvtMult = value;
			}
		}
		#endregion
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[Inventory( Enabled = false)]
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
		#region TranType
		public abstract class tranType : PX.Data.IBqlField
		{
		}
		protected String _TranType;
		[PXFormula(typeof(Selector<SOShipLine.operation, SOOrderTypeOperation.iNDocType>))]
		[PXString(SOOrderTypeOperation.iNDocType.Length, IsFixed = true)]
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
		#region TranDate
		public virtual DateTime? TranDate
		{
			get { return this._ShipDate; }
		}
		#endregion
		#region PlanType
		public abstract class planType : PX.Data.IBqlField
		{
		}
		protected String _PlanType;
		[PXDBString(2, IsFixed = true)]
		[PXDefault(typeof(Search<SOOrderTypeOperation.shipmentPlanType, Where<SOOrderTypeOperation.orderType, Equal<Current<SOShipLine.origOrderType>>, And<SOOrderTypeOperation.operation, Equal<Current<SOShipLine.operation>>>>>))]
		public virtual String PlanType
		{
			get
			{
				return this._PlanType;
			}
			set
			{
				this._PlanType = value;
			}
		}
		#endregion
		#region SubItemID
		public abstract class subItemID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubItemID;
		[SubItem(typeof(SOShipLine.inventoryID))]
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
		[SiteAvail(typeof(SOShipLine.inventoryID), typeof(SOShipLine.subItemID))]
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
		#region LocationID
		public abstract class locationID : PX.Data.IBqlField
		{
		}
		protected Int32? _LocationID;
		[SOLocationAvail(typeof(SOShipLine.inventoryID), typeof(SOShipLine.subItemID), typeof(SOShipLine.siteID), typeof(SOShipLine.tranType), typeof(SOShipLine.invtMult))]
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
		[INLotSerialNbr(typeof(SOShipLine.inventoryID), typeof(SOShipLine.subItemID), typeof(SOShipLine.locationID), PersistingCheck = PXPersistingCheck.Nothing)]
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
		#region ExpireDate
		public abstract class expireDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ExpireDate;
		[INExpireDate(typeof(SOShipLine.inventoryID), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual DateTime? ExpireDate
		{
			get
			{
				return this._ExpireDate;
			}
			set
			{
				this._ExpireDate = value;
			}
		}
		#endregion
		#region OrderUOM
		public abstract class orderUOM : PX.Data.IBqlField
		{
		}
		protected string _OrderUOM;
		[PXDBString(6, IsUnicode = true, InputMask = ">aaaaaa")]
		[PXDefault(typeof(Select<SOLine, Where<SOLine.orderType, Equal<Current<SOShipLine.origOrderType>>, And<SOLine.orderNbr, Equal<Current<SOShipLine.origOrderNbr>>, And<SOLine.lineNbr, Equal<Current<SOShipLine.origLineNbr>>>>>>), SourceField = typeof(SOLine.uOM))]
		public virtual string OrderUOM
		{
			get
			{
				return this._OrderUOM;
			}
			set
			{
				this._OrderUOM = value;
			}
		}
		#endregion
		#region UOM
		public abstract class uOM : PX.Data.IBqlField
		{
		}
		protected String _UOM;
		[INUnit(typeof(SOShipLine.inventoryID), DisplayName = "UOM")]
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
		#region ShippedQty
		public abstract class shippedQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _ShippedQty;
		[PXDBQuantity(typeof(SOShipLine.uOM), typeof(SOShipLine.baseShippedQty))]
		[PXUIField(DisplayName = "Shipped Qty.")]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXFormula(null, typeof(SumCalc<SOShipment.shipmentQty>))]
		[PXFormula(null, typeof(SumCalc<SOOrderShipment.shipmentQty>))]
		public virtual Decimal? ShippedQty
		{
			get
			{
				return this._ShippedQty;
			}
			set
			{
				this._ShippedQty = value;
			}
		}
		public virtual Decimal? Qty
		{
			get
			{
				return this._ShippedQty;
			}
			set
			{
				this._ShippedQty = value;
			}
		}
		#endregion
		#region BaseShippedQty
		public abstract class baseShippedQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseShippedQty;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXFormula(null, typeof(SumCalc<SOLine2.baseShippedQty>))]
		[PXFormula(null, typeof(SumCalc<SOLineSplit2.baseShippedQty>))]
		public virtual Decimal? BaseShippedQty
		{
			get
			{
				return this._BaseShippedQty;
			}
			set
			{
				this._BaseShippedQty = value;
			}
		}
		public virtual Decimal? BaseQty
		{
			get
			{
				return this._BaseShippedQty;
			}
			set
			{
				this._BaseShippedQty = value;
			}
		}
		#endregion
		#region OriginalShippedQty
		public abstract class originalShippedQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _OriginalShippedQty;
		[PXCalcQuantity(typeof(SOShipLine.uOM), typeof(SOShipLine.baseOriginalShippedQty))]
		[PXUIField(DisplayName = "Original Qty.", Visibility = PXUIVisibility.Visible, Visible = false, Enabled = false)]		
		public virtual Decimal? OriginalShippedQty
		{
			get
			{
				return this._OriginalShippedQty;
			}
			set
			{
				this._OriginalShippedQty = value;
			}
		}		
		#endregion
		#region BaseOriginalShippedQty
		public abstract class baseOriginalShippedQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseOriginalShippedQty;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? BaseOriginalShippedQty
		{
			get
			{
				return this._BaseOriginalShippedQty;
			}
			set
			{
				this._BaseOriginalShippedQty = value;
			}
		}		
		#endregion
		#region UnassignedQty
		public abstract class unassignedQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnassignedQty;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? UnassignedQty
		{
			get
			{
				return this._UnassignedQty;
			}
			set
			{
				this._UnassignedQty = value;
			}
		}
		#endregion
		#region CompleteQtyMin
		public abstract class completeQtyMin : PX.Data.IBqlField
		{
		}
		protected Decimal? _CompleteQtyMin;
		[PXDBDecimal(2, MinValue = 0.0, MaxValue = 100.0)]
		[PXDefault(TypeCode.Decimal, "100.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Undership Threshold (%)", Visible = false, IsReadOnly = true)]
		public virtual Decimal? CompleteQtyMin
		{
			get
			{
				return this._CompleteQtyMin;
			}
			set
			{
				this._CompleteQtyMin = value;
			}
		}
		#endregion
		#region BaseOrigOrderQty
		public abstract class baseOrigOrderQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseOrigOrderQty;
		[PXDBBaseQuantity(typeof(SOShipLine.uOM), typeof(SOShipLine.origOrderQty))]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Decimal? BaseOrigOrderQty
		{
			get
			{
				return this._BaseOrigOrderQty;
			}
			set
			{
				this._BaseOrigOrderQty = value;
			}
		}
		#endregion
		#region OrigOrderQty
		public abstract class origOrderQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _OrigOrderQty;
		[PXDBQuantity()]
		[PXUIField(DisplayName = "Ordered Qty.", Enabled = false, Visible = true)]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Decimal? OrigOrderQty
		{
			get
			{
				return this._OrigOrderQty;
			}
			set
			{
				this._OrigOrderQty = value;
			}
		}
		#endregion
		#region OpenOrderQty
		public abstract class openOrderQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _OpenOrderQty;
		[PXQuantity()]
		[PXFormula(typeof(Switch<Case<Where<SOShipLine.confirmed, Equal<boolTrue>, And<Where<SOShipLine.shipComplete, Equal<SOShipComplete.cancelRemainder>,
			Or<Sub<Mult<SOShipLine.origOrderQty, Div<SOShipLine.completeQtyMin, decimal100>>, SOShipLine.shippedQty>, LessEqual<decimal0>>>>>, decimal0>, 
			Sub<SOShipLine.origOrderQty, SOShipLine.shippedQty>>))]
		[PXUIField(DisplayName = "Open Qty.", Enabled = false, Visible = true)]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Decimal? OpenOrderQty
		{
			get
			{
				return this._OpenOrderQty;
			}
			set
			{
				this._OpenOrderQty = value;
			}
		}
		#endregion
		#region UnitCost
		public abstract class unitCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnitCost;
		[PXDBPriceCost()]
		[PXDefault(typeof(Select<SOLine, Where<SOLine.orderType, Equal<Current<SOShipLine.origOrderType>>, And<SOLine.orderNbr, Equal<Current<SOShipLine.origOrderNbr>>, And<SOLine.lineNbr, Equal<Current<SOShipLine.origLineNbr>>>>>>), SourceField = typeof(SOLine.unitCost))]
		public virtual Decimal? UnitCost
		{
			get
			{
				return this._UnitCost;
			}
			set
			{
				this._UnitCost = value;
			}
		}
		#endregion
		#region ExtCost
		public abstract class extCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _ExtCost;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXFormula(typeof(Mult<SOShipLine.shippedQty, SOShipLine.unitCost>))]
		public virtual Decimal? ExtCost
		{
			get
			{
				return this._ExtCost;
			}
			set
			{
				this._ExtCost = value;
			}
		}
		#endregion
		#region UnitPrice
		public abstract class unitPrice : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnitPrice;
		[PXDBPriceCost()]
		[PXDefault(typeof(Select<SOLine, Where<SOLine.orderType, Equal<Current<SOShipLine.origOrderType>>, And<SOLine.orderNbr, Equal<Current<SOShipLine.origOrderNbr>>, And<SOLine.lineNbr, Equal<Current<SOShipLine.origLineNbr>>>>>>), SourceField = typeof(SOLine.unitPrice))]
		public virtual Decimal? UnitPrice
		{
			get
			{
				return this._UnitPrice;
			}
			set
			{
				this._UnitPrice = value;
			}
		}
		#endregion
		#region DiscPct
		public abstract class discPct : PX.Data.IBqlField
		{
		}
		protected Decimal? _DiscPct;
		[PXDBDecimal(6)]
		[PXDefault(typeof(Search<SOLine.discPct, Where<SOLine.orderType, Equal<Current<SOShipLine.origOrderType>>, And<SOLine.orderNbr, Equal<Current<SOShipLine.origOrderNbr>>, And<SOLine.lineNbr, Equal<Current<SOShipLine.origLineNbr>>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Decimal? DiscPct
		{
			get
			{
				return this._DiscPct;
			}
			set
			{
				this._DiscPct = value;
			}
		}
		#endregion
		#region LineAmt
		public abstract class lineAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _LineAmt;
		[PXDecimal(6)]
		[PXFormula(typeof(Mult<SOShipLine.shippedQty, Mult<SOShipLine.unitPrice, Sub<decimal1, Div<SOShipLine.discPct, decimal100>>>>), typeof(SumCalc<SOShipment.lineTotal>))]
		[PXFormula(null, typeof(SumCalc<SOOrderShipment.lineTotal>))]
		public virtual Decimal? LineAmt
		{
			get
			{
				return this._LineAmt;
			}
			set
			{
				this._LineAmt = value;
			}
		}
		#endregion
		#region AlternateID
		public abstract class alternateID : PX.Data.IBqlField
		{
		}
		protected String _AlternateID;
		[PXDBString(50, IsUnicode = true, InputMask = "")]
		public virtual String AlternateID
		{
			get
			{
				return this._AlternateID;
			}
			set
			{
				this._AlternateID = value;
			}
		}
		#endregion
		#region TranDesc
		public abstract class tranDesc : PX.Data.IBqlField
		{
		}
		protected String _TranDesc;
		[PXDBString(256, IsUnicode = true)]
		[PXUIField(DisplayName = "Description")]
		public virtual String TranDesc
		{
			get
			{
				return this._TranDesc;
			}
			set
			{
				this._TranDesc = value;
			}
		}
		#endregion
		#region UnitWeigth
		public abstract class unitWeigth : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnitWeigth;
		[PXUIField(DisplayName = "Unit Weight")]
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0", typeof(Search<InventoryItem.baseWeight, Where<InventoryItem.inventoryID, Equal<Current<SOShipLine.inventoryID>>>>), CacheGlobal = true)]
		public virtual Decimal? UnitWeigth
		{
			get
			{
				return this._UnitWeigth;
			}
			set
			{
				this._UnitWeigth = value;
			}
		}
		#endregion
		#region UnitVolume
		public abstract class unitVolume : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnitVolume;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0", typeof(Search<InventoryItem.baseVolume, Where<InventoryItem.inventoryID, Equal<Current<SOShipLine.inventoryID>>>>), CacheGlobal=true)]
		public virtual Decimal? UnitVolume
		{
			get
			{
				return this._UnitVolume;
			}
			set
			{
				this._UnitVolume = value;
			}
		}
		#endregion
		#region ExtWeight
		public abstract class extWeight : PX.Data.IBqlField
		{
		}
		protected Decimal? _ExtWeight;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXFormula(typeof(Mult<Row<SOShipLine.baseShippedQty, SOShipLine.shippedQty>, SOShipLine.unitWeigth>), typeof(SumCalc<SOShipment.shipmentWeight>))]
		[PXFormula(null, typeof(SumCalc<SOOrderShipment.shipmentWeight>))]
		public virtual Decimal? ExtWeight
		{
			get
			{
				return this._ExtWeight;
			}
			set
			{
				this._ExtWeight = value;
			}
		}
		#endregion
		#region ExtVolume
		public abstract class extVolume : PX.Data.IBqlField
		{
		}
		protected Decimal? _ExtVolume;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXFormula(typeof(Mult<Row<SOShipLine.baseShippedQty, SOShipLine.shippedQty>, SOShipLine.unitVolume>), typeof(SumCalc<SOShipment.shipmentVolume>))]
		[PXFormula(null, typeof(SumCalc<SOOrderShipment.shipmentVolume>))]
		public virtual Decimal? ExtVolume
		{
			get
			{
				return this._ExtVolume;
			}
			set
			{
				this._ExtVolume = value;
			}
		}
		#endregion
		#region ProjectID
		public abstract class projectID : PX.Data.IBqlField
		{
		}
		protected Int32? _ProjectID;
		[PXDBInt()]
		public virtual Int32? ProjectID
		{
			get
			{
				return this._ProjectID;
			}
			set
			{
				this._ProjectID = value;
			}
		}
		#endregion
		#region TaskID
		public abstract class taskID : PX.Data.IBqlField
		{
		}
		protected Int32? _TaskID;
		[PXDBInt()]
		public virtual Int32? TaskID
		{
			get
			{
				return this._TaskID;
			}
			set
			{
				this._TaskID = value;
			}
		}
		#endregion
		#region ReasonCode
		public abstract class reasonCode : PX.Data.IBqlField
		{
		}
		protected String _ReasonCode;
		[PXDBString(CS.ReasonCode.reasonCodeID.Length, IsUnicode = true)]
		[PXSelector(typeof(Search<ReasonCode.reasonCodeID,
			Where<ReasonCode.usage, Equal<ReasonCodeUsages.sales>, Or<ReasonCode.usage, Equal<ReasonCodeUsages.issue>>>>), DescriptionField = typeof(ReasonCode.descr))]
		[PXUIField(DisplayName = "Reason Code")]
		public virtual String ReasonCode
		{
			get
			{
				return this._ReasonCode;
			}
			set
			{
				this._ReasonCode = value;
			}
		}
		#endregion
		#region IsFree
		public abstract class isFree : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsFree;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Free Item", Enabled = false)]
		public virtual Boolean? IsFree
		{
			get
			{
				return this._IsFree;
			}
			set
			{
				this._IsFree = value;
			}
		}
		#endregion
        #region ManualPrice
        public abstract class manualPrice : PX.Data.IBqlField
        {
        }
        protected Boolean? _ManualPrice;
        [PXDBBool()]
        [PXDefault(false)]
        public virtual Boolean? ManualPrice
        {
            get
            {
                return this._ManualPrice;
            }
            set
            {
                this._ManualPrice = value;
            }
        }
        #endregion
		#region ManualDisc
		public abstract class manualDisc : PX.Data.IBqlField
		{
		}
		protected Boolean? _ManualDisc;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Manual Cash Discount", Visibility = PXUIVisibility.Invisible)]
		public virtual Boolean? ManualDisc
		{
			get
			{
				return this._ManualDisc;
			}
			set
			{
				this._ManualDisc = value;
			}
		}
		#endregion
		#region IsUnassigned
		public abstract class isUnassigned : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsUnassigned;
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? IsUnassigned
		{
			get
			{
				return this._IsUnassigned;
			}
			set
			{
				this._IsUnassigned = value;
			}
		}
		#endregion

        #region DiscountID
        public abstract class discountID : PX.Data.IBqlField
        {
        }
        protected String _DiscountID;
        [PXDBString(10, IsUnicode = true)]
        [PXSelector(typeof(Search<ARDiscount.discountID, Where<ARDiscount.type, Equal<DiscountType.LineDiscount>>>))]
        [PXUIField(DisplayName = "Discount Code", Visible = true, Enabled = false)]
        public virtual String DiscountID
        {
            get
            {
                return this._DiscountID;
            }
            set
            {
                this._DiscountID = value;
            }
        }
        #endregion
        #region DiscountSequenceID
        public abstract class discountSequenceID : PX.Data.IBqlField
        {
        }
        protected String _DiscountSequenceID;
        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "Discount Sequence", Visible = false, Enabled = false)]
        public virtual String DiscountSequenceID
        {
            get
            {
                return this._DiscountSequenceID;
            }
            set
            {
                this._DiscountSequenceID = value;
            }
        }
        #endregion

		#region DetDiscIDC1
		public abstract class detDiscIDC1 : PX.Data.IBqlField
		{
		}
		protected String _DetDiscIDC1;
		[PXDBString(10, IsUnicode = true)]
		public virtual String DetDiscIDC1
		{
			get
			{
				return this._DetDiscIDC1;
			}
			set
			{
				this._DetDiscIDC1 = value;
			}
		}
		#endregion
		#region DetDiscSeqIDC1
		public abstract class detDiscSeqIDC1 : PX.Data.IBqlField
		{
		}
		protected String _DetDiscSeqIDC1;
		[PXDBString(10, IsUnicode = true)]
		public virtual String DetDiscSeqIDC1
		{
			get
			{
				return this._DetDiscSeqIDC1;
			}
			set
			{
				this._DetDiscSeqIDC1 = value;
			}
		}
		#endregion
		#region DetDiscIDC2
		public abstract class detDiscIDC2 : PX.Data.IBqlField
		{
		}
		protected String _DetDiscIDC2;
		[PXDBString(10, IsUnicode = true)]
		public virtual String DetDiscIDC2
		{
			get
			{
				return this._DetDiscIDC2;
			}
			set
			{
				this._DetDiscIDC2 = value;
			}
		}
		#endregion
		#region DetDiscSeqIDC2
		public abstract class detDiscSeqIDC2 : PX.Data.IBqlField
		{
		}
		protected String _DetDiscSeqIDC2;
		[PXDBString(10, IsUnicode = true)]
		public virtual String DetDiscSeqIDC2
		{
			get
			{
				return this._DetDiscSeqIDC2;
			}
			set
			{
				this._DetDiscSeqIDC2 = value;
			}
		}
		#endregion
		#region DetDiscApp
		public abstract class detDiscApp : PX.Data.IBqlField
		{
		}
		protected Boolean? _DetDiscApp;
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? DetDiscApp
		{
			get
			{
				return this._DetDiscApp;
			}
			set
			{
				this._DetDiscApp = value;
			}
		}
		#endregion
		#region PromoDiscID
		public virtual string PromoDiscID
		{
			get
			{
				return null;
			}
			set
			{ ;}
		}
		#endregion
		#region DocDiscIDC1
		public abstract class docDiscIDC1 : PX.Data.IBqlField
		{
		}
		protected String _DocDiscIDC1;
		[PXDBString(10, IsUnicode = true)]
		public virtual String DocDiscIDC1
		{
			get
			{
				return this._DocDiscIDC1;
			}
			set
			{
				this._DocDiscIDC1 = value;
			}
		}
		#endregion
		#region DocDiscSeqIDC1
		public abstract class docDiscSeqIDC1 : PX.Data.IBqlField
		{
		}
		protected String _DocDiscSeqIDC1;
		[PXDBString(10, IsUnicode = true)]
		public virtual String DocDiscSeqIDC1
		{
			get
			{
				return this._DocDiscSeqIDC1;
			}
			set
			{
				this._DocDiscSeqIDC1 = value;
			}
		}
		#endregion
		#region DocDiscIDC2
		public abstract class docDiscIDC2 : PX.Data.IBqlField
		{
		}
		protected String _DocDiscIDC2;
		[PXDBString(10, IsUnicode = true)]
		public virtual String DocDiscIDC2
		{
			get
			{
				return this._DocDiscIDC2;
			}
			set
			{
				this._DocDiscIDC2 = value;
			}
		}
		#endregion
		#region DocDiscSeqIDC2
		public abstract class docDiscSeqIDC2 : PX.Data.IBqlField
		{
		}
		protected String _DocDiscSeqIDC2;
		[PXDBString(10, IsUnicode = true)]
		public virtual String DocDiscSeqIDC2
		{
			get
			{
				return this._DocDiscSeqIDC2;
			}
			set
			{
				this._DocDiscSeqIDC2 = value;
			}
		}
		#endregion

		#region KeepManualFreight
		public abstract class keepManualFreight : PX.Data.IBqlField
		{
		}
		protected Boolean? _KeepManualFreight = false;
		[PXBool()]
		public virtual Boolean? KeepManualFreight
		{
			get
			{
				return this._KeepManualFreight;
			}
			set
			{
				this._KeepManualFreight = value;
			}
		}
		#endregion

		#region ShipComplete
		public abstract class shipComplete : PX.Data.IBqlField
		{
		}
		protected String _ShipComplete;
		[PXDBString(1, IsFixed = true)]
		[SOShipComplete.List()]
		[PXUIField(DisplayName = "Shipping Rule", Enabled = false, Visible = false)]
		public virtual String ShipComplete
		{
			get
			{
				return this._ShipComplete;
			}
			set
			{
				this._ShipComplete = value;
			}
		}
		#endregion

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
		#region Methods
		public static implicit operator SOShipLineSplit(SOShipLine item)
		{
			SOShipLineSplit ret = new SOShipLineSplit();
			ret.ShipmentNbr = item.ShipmentNbr;
			ret.LineNbr = item.LineNbr;
			ret.OrigOrderType = item.OrigOrderType;
			ret.Operation = item.Operation;
			ret.SplitLineNbr = 1;
			ret.InventoryID = item.InventoryID;
			ret.SiteID = item.SiteID;
			ret.SubItemID = item.SubItemID;
			ret.LocationID = item.LocationID;
			ret.LotSerialNbr = item.LotSerialNbr;
			ret.ExpireDate = item.ExpireDate;
			ret.Qty = item.Qty;
			ret.UOM = item.UOM;
			ret.ShipDate = item.ShipDate;
			ret.BaseQty = item.BaseQty;
			ret.InvtMult = item.InvtMult;
			ret.PlanType = item.PlanType;
			ret.Released = item.Released;

			return ret;
		}
		public static implicit operator SOShipLine(SOShipLineSplit item)
		{
			SOShipLine ret = new SOShipLine();
			ret.ShipmentNbr = item.ShipmentNbr;
			ret.LineNbr = item.LineNbr;
			ret.OrigOrderType = item.OrigOrderType;
			ret.LineType = "GI";
			ret.Operation = item.Operation;
			ret.InventoryID = item.InventoryID;
			ret.SiteID = item.SiteID;
			ret.SubItemID = item.SubItemID;
			ret.LocationID = item.LocationID;
			ret.LotSerialNbr = item.LotSerialNbr;
			ret.Qty = item.Qty;
			ret.UOM = item.UOM;
			ret.ShipDate = item.ShipDate;
			ret.BaseQty = item.BaseQty;
			ret.InvtMult = item.InvtMult;
			ret.PlanType = item.PlanType;

			return ret;
		}
		public static implicit operator SOShipLine(SOLine item)
		{
			SOShipLine ret = new SOShipLine();
			ret.OrigOrderType = item.OrderType;
			ret.OrigOrderNbr = item.OrderNbr;
			ret.OrigLineNbr = item.LineNbr;
			ret.ShipmentType = INTranType.DocType(item.TranType);
			ret.ShipmentNbr = Constants.NoShipmentNbr;
			ret.LineType = item.LineType;
			ret.LineNbr = item.LineNbr;
			ret.InventoryID = item.InventoryID;
			ret.UOM = item.UOM;
			ret.ShippedQty = item.OrderQty;
			ret.ShipComplete = item.ShipComplete;
			return ret;
		}
		public static implicit operator SOShipLine(PXResult<POReceiptLine, SOLineSplit, SOLine> item)
		{
			SOShipLine ret = new SOShipLine();
			ret.OrigOrderType = ((SOLine)item).OrderType;
			ret.OrigOrderNbr = ((SOLine)item).OrderNbr;
			ret.OrigLineNbr = ((SOLine)item).LineNbr;
			ret.ShipmentNbr = ((POReceiptLine)item).ReceiptNbr;
			ret.ShipmentType = SOShipmentType.DropShip;
			ret.LineType = ((SOLine)item).LineType;
			ret.LineNbr = ((POReceiptLine)item).LineNbr;
			ret.InventoryID = ((POReceiptLine)item).InventoryID;
			ret.UOM = ((POReceiptLine)item).UOM;
			ret.ShippedQty = ((POReceiptLine)item).ReceiptQty;
			return ret;
		}
		#endregion

		#region IDiscountable Members

        public decimal? CuryExtPrice
        {
            get
            {
                return null;
            }
        }

		public decimal? CuryLineAmt
		{
			get
			{
				return null;
			}
			set
			{

			}
		}

		public decimal? CuryDiscAmt
		{
			get
			{
				return null;
			}
			set
			{

			}
		}

		public decimal? CuryUnitPrice
		{
			get { return null; }
		}

		public long? CuryInfoID
		{
			get { return null; }
		}

		#endregion
		#region HasKitComponents
		protected bool? _HasKitComponents = false;
		public bool? HasKitComponents
		{
			get
			{
				return _HasKitComponents;
			}
			set
			{
				this._HasKitComponents = value;
			}
		}
		#endregion
		#region HasSerialComponents
		protected bool? _HasSerialComponents = false;
		public bool? HasSerialComponents
		{
			get
			{
				return _HasSerialComponents;
			}
			set
			{
				this._HasSerialComponents = value;
			}
		}
		#endregion
		#region Components
		public class KitComponentKey
		{
			public readonly Int32 ItemID;
            public readonly Int32 SubItemID;
			protected Int32 _HashCode;

            public KitComponentKey(Int32? ItemID, Int32? SubItemID)
			{
				this.ItemID = (Int32)ItemID;
				this.SubItemID = (Int32)SubItemID;
				_HashCode = this.ItemID.GetHashCode() * 397 ^ this.SubItemID.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				KitComponentKey that = obj as KitComponentKey;

				if (that == null)
				{
					return false;
				}

				return object.Equals(that.ItemID, this.ItemID) && object.Equals(that.SubItemID, this.SubItemID);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return _HashCode;
				}
			} 
		}

		public Dictionary<KitComponentKey, decimal?> Unshipped = new Dictionary<KitComponentKey,decimal?>();
		public Dictionary<KitComponentKey, decimal?> Planned = new Dictionary<KitComponentKey, decimal?>();
		#endregion
		#region HasMixedProjectTasks
		public abstract class hasMixedProjectTasks : PX.Data.IBqlField
		{
		}
		protected bool? _HasMixedProjectTasks;
		/// <summary>
		/// Returns true if the splits associated with the line has mixed ProjectTask values.
		/// This field is used to validate the record on persist. 
		/// </summary>
		[PXBool]
		[PXFormula(typeof(False))]
		public virtual bool? HasMixedProjectTasks
		{
			get
			{
				return _HasMixedProjectTasks;
			}
			set
			{
				_HasMixedProjectTasks = value;
			}
		}
		#endregion
		#region IsClone
		protected bool? _IsClone = false;
		public bool? IsClone
		{
			get
			{
				return _IsClone;
			}
			set
			{
				this._IsClone = value;
			}
		}
		#endregion
	}
}
