namespace PX.Objects.SO
{
	using System;
	using PX.Data;
	using PX.Objects.IN;
	using PX.Objects.CS;
	using PX.Objects.GL;

	[System.SerializableAttribute()]
	[PXCacheName(Messages.SOShipLineSplit)]
	[PXProjection(typeof(Select<SOShipLineSplit, Where<SOShipLineSplit.isUnassigned, Equal<False>>>), Persistent=true)]
	public partial class SOShipLineSplit : PX.Data.IBqlTable, ILSDetail
	{
		#region ShipmentNbr
		public abstract class shipmentNbr : PX.Data.IBqlField
		{
		}
		protected String _ShipmentNbr;
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "")]
		[PXDBDefault(typeof(SOShipment.shipmentNbr))]
		[PXParent(typeof(Select<SOShipLine, Where<SOShipLine.shipmentNbr, Equal<Current<SOShipLineSplit.shipmentNbr>>, And<SOShipLine.lineNbr, Equal<Current<SOShipLineSplit.lineNbr>>>>>))]
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
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineNbr;
		[PXDBInt(IsKey = true)]
		[PXDefault(typeof(SOShipLine.lineNbr))]
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
		#region OrigOrderType
		public abstract class origOrderType : PX.Data.IBqlField
		{
		}
		protected String _OrigOrderType;
		[PXDBString(2, IsFixed = true)]
		[PXDefault(typeof(SOShipLine.origOrderType))]
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
		[PXDefault(typeof(SOShipLine.origOrderNbr))]
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
		[PXDefault(typeof(SOShipLine.origLineNbr))]
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
		[PXDefault(typeof(SOShipLine.origSplitLineNbr), PersistingCheck = PXPersistingCheck.Nothing)]
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
		#region OrigPlanType
		public abstract class origPlanType : PX.Data.IBqlField
		{
		}
		[PXDBString(2, IsFixed = true)]
		[PXDefault(typeof(SOShipLine.origPlanType))]
		[PXSelector(typeof(Search<INPlanType.planType>), CacheGlobal = true)]
		public virtual String OrigPlanType
		{
			get;
			set;
		}
		#endregion
		#region Operation
		public abstract class operation : PX.Data.IBqlField
		{
		}
		protected String _Operation;
		[PXDBString(1, IsFixed = true, InputMask = ">a")]
		[PXDefault(typeof(SOShipLine.operation))]
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
		#region SplitLineNbr
		public abstract class splitLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _SplitLineNbr;
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		[PXLineNbr(typeof(SOShipment.lineCntr))]
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
		#region InvtMult
		public abstract class invtMult : PX.Data.IBqlField
		{
		}
		protected Int16? _InvtMult;
		[PXDBShort()]
		[PXDefault(typeof(INTran.invtMult))]
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
		[Inventory(Enabled = false, Visible = true)]
		[PXDefault(typeof(SOShipLine.inventoryID))]
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
		#region LineType
		public abstract class lineType : PX.Data.IBqlField
		{
		}
		protected String _LineType;
		[PXDBString(2, IsFixed = true)]
		[PXDefault(typeof(SOShipLine.lineType))]
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
		#region IsStockItem
		public abstract class isStockItem : IBqlField { }
		[PXDBBool()]
		[PXFormula(typeof(Selector<SOShipLineSplit.inventoryID, InventoryItem.stkItem>))]
		public bool? IsStockItem
		{
			get;
			set;
		}
		#endregion
		#region IsComponentItem
		public abstract class isComponentItem : IBqlField { }
		[PXDBBool()]
		[PXFormula(typeof(Switch<Case<Where<SOShipLineSplit.inventoryID, Equal<Current<SOShipLine.inventoryID>>>, False>, True>))]
		public bool? IsComponentItem
		{
			get;
			set;
		}
		#endregion
		#region TranType
		public abstract class tranType : PX.Data.IBqlField
		{
		}
		protected String _TranType;
		[PXFormula(typeof(Selector<SOShipLineSplit.operation, SOOrderTypeOperation.iNDocType>))]
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
		[PXDefault(typeof(SOShipLine.planType))]
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
		#region PlanID
		public abstract class planID : PX.Data.IBqlField
		{
		}
		protected Int64? _PlanID;
		[PXDBLong(IsImmutable = true)]
		[SOShipLineSplitPlanID(typeof(SOShipment.noteID), typeof(SOShipment.hold), typeof(SOShipment.shipDate))]
		public virtual Int64? PlanID
		{
			get
			{
				return this._PlanID;
			}
			set
			{
				this._PlanID = value;
			}
		}
		#endregion
		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SiteID;
		[IN.Site()]
		[PXDefault(typeof(SOShipLine.siteID))]
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
		[SOLocationAvail(typeof(SOShipLineSplit.inventoryID), typeof(SOShipLineSplit.subItemID), typeof(SOShipLineSplit.siteID), typeof(SOShipLineSplit.tranType), typeof(SOShipLineSplit.invtMult))]
		[PXDefault()]
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
		#region SubItemID
		public abstract class subItemID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubItemID;
		[IN.SubItem(typeof(SOShipLineSplit.inventoryID))]
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
		#region LotSerialNbr
		public abstract class lotSerialNbr : PX.Data.IBqlField
		{
		}
		protected String _LotSerialNbr;
        [SOShipLotSerialNbr(typeof(SOShipLineSplit.siteID), typeof(SOShipLineSplit.inventoryID), typeof(SOShipLineSplit.subItemID), typeof(SOShipLineSplit.locationID), typeof(SOShipLine.lotSerialNbr), FieldClass = "LotSerial")]
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
        #region LastLotSerialNbr
        public abstract class lastLotSerialNbr : PX.Data.IBqlField
        {
        }
        protected String _LastLotSerialNbr;
        [PXString(100, IsUnicode = true)]
        public virtual String LastLotSerialNbr
        {
            get
            {
                return this._LastLotSerialNbr;
            }
            set
            {
                this._LastLotSerialNbr = value;
            }
        }
        #endregion
		#region LotSerClassID
		public abstract class lotSerClassID : PX.Data.IBqlField
		{
		}
		protected String _LotSerClassID;
		[PXString(10, IsUnicode = true)]
		public virtual String LotSerClassID
		{
			get
			{
				return this._LotSerClassID;
			}
			set
			{
				this._LotSerClassID = value;
			}
		}
		#endregion
		#region AssignedNbr
		public abstract class assignedNbr : PX.Data.IBqlField
		{
		}
		protected String _AssignedNbr;
		[PXString(30, IsUnicode = true)]
		public virtual String AssignedNbr
		{
			get
			{
				return this._AssignedNbr;
			}
			set
			{
				this._AssignedNbr = value;
			}
		}
		#endregion
		#region ExpireDate
		public abstract class expireDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ExpireDate;
		[SOShipExpireDate(typeof(SOShipLineSplit.inventoryID), FieldClass = "LotSerial")]
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
		#region UOM
		public abstract class uOM : PX.Data.IBqlField
		{
		}
		protected String _UOM;
		[INUnit(typeof(SOShipLineSplit.inventoryID), DisplayName = "UOM", Enabled = false)]
		[PXDefault(typeof(SOShipLine.uOM))]
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
		[PXDBQuantity(typeof(SOShipLineSplit.uOM), typeof(SOShipLineSplit.baseQty))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Quantity")]
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
		[PXDBDecimal(6)]
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
		[PXUIField(DisplayName = "Confirmed")]
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
		[PXUIField(DisplayName = "Released")]
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
		#region ProjectID
		public abstract class projectID : PX.Data.IBqlField
		{
		}
		protected Int32? _ProjectID;
		[PXFormula(typeof(Selector<SOShipLineSplit.locationID, INLocation.projectID>))]
		[PXInt]
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
		[PXFormula(typeof(Selector<SOShipLineSplit.locationID, INLocation.taskID>))]
		[PXInt]
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
	namespace Unassigned
	{
		using PX.Objects.IN.Overrides.INDocumentRelease;
		/// <summary>
		/// Is exact copy of SOShipLineSplit except PXProjection Where clause and missing PlanIDAttribute.
		/// </summary>
		[System.SerializableAttribute()]
		[PXProjection(typeof(Select<SOShipLineSplit, Where<SOShipLineSplit.isUnassigned, Equal<True>>>), Persistent = true)]
		public partial class SOShipLineSplit : PX.Data.IBqlTable, ILSDetail
		{
			#region ShipmentNbr
			public abstract class shipmentNbr : PX.Data.IBqlField
			{
			}
			protected String _ShipmentNbr;
			[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "")]
			[PXDBDefault(typeof(SOShipment.shipmentNbr))]
			[PXParent(typeof(Select<SOShipLine, Where<SOShipLine.shipmentNbr, Equal<Current<SOShipLineSplit.shipmentNbr>>, And<SOShipLine.lineNbr, Equal<Current<SOShipLineSplit.lineNbr>>>>>))]
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
			#region LineNbr
			public abstract class lineNbr : PX.Data.IBqlField
			{
			}
			protected Int32? _LineNbr;
			[PXDBInt(IsKey = true)]
			[PXDefault(typeof(SOShipLine.lineNbr))]
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
			#region OrigOrderType
			public abstract class origOrderType : PX.Data.IBqlField
			{
			}
			protected String _OrigOrderType;
			[PXDBString(2, IsFixed = true)]
			[PXDefault(typeof(SOShipLine.origOrderType))]
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
			[PXDefault(typeof(SOShipLine.origOrderNbr))]
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
			[PXDefault(typeof(SOShipLine.origLineNbr))]
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
			[PXDefault(typeof(SOShipLine.origSplitLineNbr), PersistingCheck = PXPersistingCheck.Nothing)]
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
			#region OrigPlanType
			public abstract class origPlanType : PX.Data.IBqlField
			{
			}
			[PXDBString(2, IsFixed = true)]
			[PXDefault(typeof(SOShipLine.origPlanType))]
			[PXSelector(typeof(Search<INPlanType.planType>), CacheGlobal = true)]
			public virtual String OrigPlanType
			{
				get;
				set;
			}
			#endregion
			#region Operation
			public abstract class operation : PX.Data.IBqlField
			{
			}
			protected String _Operation;
			[PXDBString(1, IsFixed = true, InputMask = ">a")]
			[PXDefault(typeof(SOShipLine.operation))]
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
			#region SplitLineNbr
			public abstract class splitLineNbr : PX.Data.IBqlField
			{
			}
			protected Int32? _SplitLineNbr;
			[PXDBInt(IsKey = true)]
			[PXDefault()]
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
			#region InvtMult
			public abstract class invtMult : PX.Data.IBqlField
			{
			}
			protected Int16? _InvtMult;
			[PXDBShort()]
			[PXDefault(typeof(INTran.invtMult))]
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
			[Inventory(Enabled = false, Visible = true)]
			[PXDefault(typeof(SOShipLine.inventoryID))]
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
			#region LineType
			public abstract class lineType : PX.Data.IBqlField
			{
			}
			protected String _LineType;
			[PXDBString(2, IsFixed = true)]
			[PXDefault(typeof(SOShipLine.lineType))]
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
			#region IsStockItem
			public abstract class isStockItem : IBqlField { }
			[PXDBBool()]
			[PXFormula(typeof(Selector<SOShipLineSplit.inventoryID, InventoryItem.stkItem>))]
			public bool? IsStockItem
			{
				get;
				set;
			}
			#endregion
			#region IsComponentItem
			public abstract class isComponentItem : IBqlField { }
			[PXDBBool()]
			[PXFormula(typeof(Switch<Case<Where<SOShipLineSplit.inventoryID, Equal<Current<SOShipLine.inventoryID>>>, False>, True>))]
			public bool? IsComponentItem
			{
				get;
				set;
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
			[PXDefault(typeof(SOShipLine.planType))]
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
			#region PlanID
			public abstract class planID : PX.Data.IBqlField
			{
			}
			protected Int64? _PlanID;
			[PXDBLong(IsImmutable = true)]
			public virtual Int64? PlanID
			{
				get
				{
					return this._PlanID;
				}
				set
				{
					this._PlanID = value;
				}
			}
			#endregion
			#region SiteID
			public abstract class siteID : PX.Data.IBqlField
			{
			}
			protected Int32? _SiteID;
			[IN.Site()]
			[PXDefault(typeof(SOShipLine.siteID))]
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
			[SOLocationAvail(typeof(SOShipLineSplit.inventoryID), typeof(SOShipLineSplit.subItemID), typeof(SOShipLineSplit.siteID), typeof(SOShipLineSplit.tranType), typeof(SOShipLineSplit.invtMult))]
			[PXDefault()]
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
			#region SubItemID
			public abstract class subItemID : PX.Data.IBqlField
			{
			}
			protected Int32? _SubItemID;
			[IN.SubItem(typeof(SOShipLineSplit.inventoryID))]
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
			#region LotSerialNbr
			public abstract class lotSerialNbr : PX.Data.IBqlField
			{
			}
			protected String _LotSerialNbr;
			[SOShipLotSerialNbr(typeof(SOShipLineSplit.siteID), typeof(SOShipLineSplit.inventoryID), typeof(SOShipLineSplit.subItemID), typeof(SOShipLineSplit.locationID), typeof(SOShipLine.lotSerialNbr), FieldClass = "LotSerial")]
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
			#region LastLotSerialNbr
			public abstract class lastLotSerialNbr : PX.Data.IBqlField
			{
			}
			protected String _LastLotSerialNbr;
			[PXString(100, IsUnicode = true)]
			public virtual String LastLotSerialNbr
			{
				get
				{
					return this._LastLotSerialNbr;
				}
				set
				{
					this._LastLotSerialNbr = value;
				}
			}
			#endregion
			#region LotSerClassID
			public abstract class lotSerClassID : PX.Data.IBqlField
			{
			}
			protected String _LotSerClassID;
			[PXString(10, IsUnicode = true)]
			public virtual String LotSerClassID
			{
				get
				{
					return this._LotSerClassID;
				}
				set
				{
					this._LotSerClassID = value;
				}
			}
			#endregion
			#region AssignedNbr
			public abstract class assignedNbr : PX.Data.IBqlField
			{
			}
			protected String _AssignedNbr;
			[PXString(30, IsUnicode = true)]
			public virtual String AssignedNbr
			{
				get
				{
					return this._AssignedNbr;
				}
				set
				{
					this._AssignedNbr = value;
				}
			}
			#endregion
			#region ExpireDate
			public abstract class expireDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _ExpireDate;
			[SOShipExpireDate(typeof(SOShipLineSplit.inventoryID), FieldClass = "LotSerial")]
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
			#region UOM
			public abstract class uOM : PX.Data.IBqlField
			{
			}
			protected String _UOM;
			[INUnit(typeof(SOShipLineSplit.inventoryID), DisplayName = "UOM", Enabled = false)]
			[PXDefault(typeof(SOShipLine.uOM))]
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
			[PXDBQuantity(typeof(SOShipLineSplit.uOM), typeof(SOShipLineSplit.baseQty))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Quantity")]
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
			[PXDBDecimal(6)]
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
			[PXUIField(DisplayName = "Confirmed")]
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
			[PXUIField(DisplayName = "Released")]
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
			#region IsUnassigned
			public abstract class isUnassigned : PX.Data.IBqlField
			{
			}
			protected Boolean? _IsUnassigned;
			[PXDBBool()]
			[PXDefault(typeof(SOShipLine.isUnassigned))]
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
			#region ProjectID
			public abstract class projectID : PX.Data.IBqlField
			{
			}
			protected Int32? _ProjectID;
			[PXFormula(typeof(Selector<SOShipLineSplit.locationID, INLocation.projectID>))]
			[PXInt]
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
			[PXFormula(typeof(Selector<SOShipLineSplit.locationID, INLocation.taskID>))]
			[PXInt]
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
}
