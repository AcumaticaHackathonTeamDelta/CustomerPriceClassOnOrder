namespace PX.Objects.SO
{
	using System;
	using PX.Data;
	using PX.Objects.IN;
	using PX.Objects.AR;
	using PX.Objects.CR;
	using PX.Objects.CS;
	using PX.Objects.CM;
	using POReceipt = PX.Objects.PO.POReceipt;
	using POReceiptLine = PX.Objects.PO.POReceiptLine;
	using POReceiptEntry = PX.Objects.PO.POReceiptEntry;

	[PXPrimaryGraph(new Type[] {
					typeof(SOShipmentEntry),
					typeof(POReceiptEntry)},
				new Type[] {
					typeof(Select2<SOShipment, LeftJoin<INSite, On<INSite.siteID, Equal<SOShipment.siteID>>>, Where<SOShipment.shipmentNbr, Equal<Current<SOOrderShipment.shipmentNbr>>, And<SOShipmentType.dropShip, NotEqual<Current<SOOrderShipment.shipmentType>>, And<Where<INSite.siteID, IsNull, Or<Match<INSite, Current<AccessInfo.userName>>>>>>>>),
					typeof(Select<POReceipt, Where<POReceipt.receiptNbr, Equal<Current<SOOrderShipment.shipmentNbr>>, And<SOShipmentType.dropShip, Equal<Current<SOOrderShipment.shipmentType>>>>>) 
					},
					UseParent = false)]
	[Serializable()]
	[PXCacheName(Messages.SOOrderShipment)]
	public partial class SOOrderShipment : PX.Data.IBqlTable
	{
		#region Selected
		public abstract class selected : IBqlField
		{
		}
		protected bool? _Selected = false;
		[PXBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Selected")]
		public virtual bool? Selected
		{
			get
			{
				return _Selected;
			}
			set
			{
				_Selected = value;
			}
		}
		#endregion
		#region OrderType
		public abstract class orderType : PX.Data.IBqlField
		{
		}
		protected String _OrderType;
		[PXDBString(2, IsKey = true, IsFixed = true)]
		[PXUIField(DisplayName = "Order Type", Enabled = false)]
		[PXSelector(typeof(Search<SOOrderType.orderType>))]
		[PXDefault()]
		public virtual String OrderType
		{
			get
			{
				return this._OrderType;
			}
			set
			{
				this._OrderType = value;
			}
		}
		#endregion
		#region OrderNbr
		public abstract class orderNbr : PX.Data.IBqlField
		{
		}
		protected String _OrderNbr;
		[PXDBString(15, IsKey = true, InputMask = "", IsUnicode = true)]
		[PXUIField(DisplayName = "Order Nbr.", Enabled = false)]
		[PXSelector(typeof(Search<SOOrder.orderNbr, Where<SOOrder.orderType, Equal<Current<SOOrderShipment.orderType>>>>))]
		[PXParent(typeof(Select<SOOrder, Where<SOOrder.orderType, Equal<Current<SOOrderShipment.orderType>>, And<SOOrder.orderNbr, Equal<Current<SOOrderShipment.orderNbr>>>>>))]
		[PXDefault()]
		public virtual String OrderNbr
		{
			get
			{
				return this._OrderNbr;
			}
			set
			{
				this._OrderNbr = value;
			}
		}
		#endregion
		#region Operation
		public abstract class operation : PX.Data.IBqlField
		{
		}
		protected String _Operation;
		[PXDBString(1, IsFixed = true, InputMask = ">a")]
		[PXUIField(DisplayName = "Operation")]
		[SOOperation.List]
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
		#region ShipmentType
		public abstract class shipmentType : PX.Data.IBqlField
		{
		}
		protected String _ShipmentType;
		[PXDBString(1, IsKey = true, IsFixed = true)]
		[PXDefault(typeof(SOShipment.shipmentType))]
		[SOShipmentType.List()]
		[PXUIField(DisplayName = "Shipment Type")]
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
		#region ShipmentNbr
		public abstract class shipmentNbr : PX.Data.IBqlField
		{
		}
		protected String _ShipmentNbr;
		[PXDBString(15, IsKey = true, InputMask = "", IsUnicode = true)]
		[PXDBLiteDefault(typeof(SOShipment.shipmentNbr), DefaultForInsert = false, DefaultForUpdate = false)]
		[PXUIField(DisplayName = "Shipment Nbr.", Visible = false, Enabled = false)]
		[PXParent(typeof(Select<SOShipment, Where<SOShipment.shipmentNbr, Equal<Current<SOOrderShipment.shipmentNbr>>, And<SOShipment.shipmentType, Equal<Current<SOOrderShipment.shipmentType>>>>>))]
		[PXSelector(typeof(Search<SOOrderShipment.shipmentNbr, Where<SOOrderShipment.orderType, Equal<Current<SOOrderShipment.orderType>>, And<SOOrderShipment.orderNbr, Equal<Current<SOOrderShipment.orderNbr>>, And<SOOrderShipment.shipmentType, Equal<Current<SOOrderShipment.shipmentType>>>>>>), DirtyRead = true)]
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
		#region CustomerID
		public abstract class customerID : PX.Data.IBqlField
		{
		}
		protected Int32? _CustomerID;
		[Customer(typeof(Search<BAccountR.bAccountID,
					Where<Customer.bAccountID, IsNotNull,
					Or<BAccountR.type, Equal<BAccountType.companyType>>>>))]
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
		#region CustomerLocationID
		public abstract class customerLocationID : PX.Data.IBqlField
		{
		}
		protected Int32? _CustomerLocationID;
		[LocationID(typeof(Where<Location.bAccountID, Equal<Current<SOOrderShipment.customerID>>>), DescriptionField = typeof(Location.descr))]
		[PXDefault(typeof(SOShipment.customerLocationID))]
		public virtual Int32? CustomerLocationID
		{
			get
			{
				return this._CustomerLocationID;
			}
			set
			{
				this._CustomerLocationID = value;
			}
		}
		#endregion
		#region ShipAddressID
		public abstract class shipAddressID : PX.Data.IBqlField
		{
		}
		protected Int32? _ShipAddressID;
		[PXDBInt()]
		[PXDBDefault(typeof(SOShipment.shipAddressID), DefaultForInsert = false, DefaultForUpdate = false)]
		public virtual Int32? ShipAddressID
		{
			get
			{
				return this._ShipAddressID;
			}
			set
			{
				this._ShipAddressID = value;
			}
		}
		#endregion
		#region ShipContactID
		public abstract class shipContactID : PX.Data.IBqlField
		{
		}
		protected Int32? _ShipContactID;
		[PXDBInt()]
		[PXDBDefault(typeof(SOShipment.shipContactID), DefaultForInsert = false, DefaultForUpdate = false)]
		public virtual Int32? ShipContactID
		{
			get
			{
				return this._ShipContactID;
			}
			set
			{
				this._ShipContactID = value;
			}
		}
		#endregion
		#region LineCntr
		public abstract class lineCntr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineCntr;
		[PXDBInt()]
		[PXDefault(0)]
		public virtual Int32? LineCntr
		{
			get
			{
				return this._LineCntr;
			}
			set
			{
				this._LineCntr = value;
			}
		}
		#endregion
		#region ShipDate
		public abstract class shipDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ShipDate;
		[PXDBDate()]
		[PXDefault(typeof(SOShipment.shipDate))]
		[PXUIField(DisplayName="Shipment Date")]
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
		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SiteID;
		[IN.Site(DisplayName = "Warehouse ID", DescriptionField = typeof(INSite.descr))]
		[PXDefault(typeof(SOShipment.siteID), PersistingCheck = PXPersistingCheck.Nothing)]
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
		#region Hold
		public abstract class hold : PX.Data.IBqlField
		{
		}
		protected Boolean? _Hold;
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? Hold
		{
			get
			{
				return this._Hold;
			}
			set
			{
				this._Hold = value;
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
		#region ShipComplete
		public abstract class shipComplete : PX.Data.IBqlField
		{
		}
		protected String _ShipComplete;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(typeof(Search<SOOrder.shipComplete, Where<SOOrder.orderType, Equal<Current<SOOrderShipment.orderType>>, And<SOOrder.orderNbr, Equal<Current<SOOrderShipment.orderNbr>>>>>))]
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
		#region ShipmentQty
		public abstract class shipmentQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _ShipmentQty;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName="Shipped Qty.", Enabled = false)]
		public virtual Decimal? ShipmentQty
		{
			get
			{
				return this._ShipmentQty;
			}
			set
			{
				this._ShipmentQty = value;
			}
		}
		#endregion
		#region ShipmentWeight
		public abstract class shipmentWeight : PX.Data.IBqlField
		{
		}
		protected Decimal? _ShipmentWeight;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Shipped Weight", Enabled = false)]
		public virtual Decimal? ShipmentWeight
		{
			get
			{
				return this._ShipmentWeight;
			}
			set
			{
				this._ShipmentWeight = value;
			}
		}
		#endregion
		#region ShipmentVolume
		public abstract class shipmentVolume : PX.Data.IBqlField
		{
		}
		protected Decimal? _ShipmentVolume;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Shipped Volume", Enabled = false)]
		public virtual Decimal? ShipmentVolume
		{
			get
			{
				return this._ShipmentVolume;
			}
			set
			{
				this._ShipmentVolume = value;
			}
		}
		#endregion
		#region LineTotal
		public abstract class lineTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _LineTotal;
		[PXDBBaseCury()]
		[PXUIField(DisplayName = "Line Total")]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? LineTotal
		{
			get
			{
				return this._LineTotal;
			}
			set
			{
				this._LineTotal = value;
			}
		}
		#endregion
		#region InvoiceType
		public abstract class invoiceType : PX.Data.IBqlField
		{
		}
		protected String _InvoiceType;
		[PXDBString(3, IsFixed = true)]
		[PXUIField(DisplayName = "Invoice Type", Enabled = false)]
		[ARDocType.List()]
		public virtual String InvoiceType
		{
			get
			{
				return this._InvoiceType;
			}
			set
			{
				this._InvoiceType = value;
			}
		}
		#endregion
		#region InvoiceNbr
		public abstract class invoiceNbr : PX.Data.IBqlField
		{
		}
		protected String _InvoiceNbr;
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Invoice Nbr.", Enabled = false)]
		[PXParent(typeof(Select<SOInvoice, Where<SOInvoice.docType, Equal<Current<SOOrderShipment.invoiceType>>, And<SOInvoice.refNbr, Equal<Current<SOOrderShipment.invoiceNbr>>>>>))]
		[PXDBLiteDefault(typeof(ARInvoice.refNbr), DefaultForInsert = false, DefaultForUpdate = false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXSelector(typeof(Search<SOInvoice.refNbr, Where<SOInvoice.docType, Equal<Current<SOOrderShipment.invoiceType>>>>), DirtyRead = true)]
		[PXUnboundFormula(typeof(Switch<Case<Where<Current2<SOOrderShipment.invoiceNbr>, IsNull, And<Selector<SOOrderShipment.orderType, SOOrderType.aRDocType>, NotEqual<ARDocType.noUpdate>>>, int1>, int0>), typeof(SumCalc<SOShipment.unbilledOrderCntr>))]
		[PXUnboundFormula(typeof(Switch<Case<Where<Current2<SOOrderShipment.invoiceNbr>, IsNotNull, And<SOOrderShipment.invoiceReleased, Equal<False>>>, int1>, int0>), typeof(SumCalc<SOShipment.billedOrderCntr>))]
		[PXUnboundFormula(typeof(Switch<Case<Where<Current2<SOOrderShipment.invoiceNbr>, IsNotNull, And<SOOrderShipment.invoiceReleased, Equal<False>>>, int1>, int0>), typeof(SumCalc<SOOrder.billedCntr>))]
		[PXUnboundFormula(typeof(Switch<Case<Where<Current2<SOOrderShipment.invoiceNbr>, IsNotNull, And<SOOrderShipment.invoiceReleased, Equal<True>>>, int1>, int0>), typeof(SumCalc<SOShipment.releasedOrderCntr>))]
		[PXUnboundFormula(typeof(Switch<Case<Where<Current2<SOOrderShipment.invoiceNbr>, IsNotNull, And<SOOrderShipment.invoiceReleased, Equal<True>>>, int1>, int0>), typeof(SumCalc<SOOrder.releasedCntr>))]
		public virtual String InvoiceNbr
		{
			get
			{
				return this._InvoiceNbr;
			}
			set
			{
				this._InvoiceNbr = value;
			}
		}
		#endregion
		#region InvoiceReleased
		public abstract class invoiceReleased : PX.Data.IBqlField
		{
		}
		protected Boolean? _InvoiceReleased;
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? InvoiceReleased
		{
			get
			{
				return this._InvoiceReleased;
			}
			set
			{
				this._InvoiceReleased = value;
			}
		}
		#endregion
		#region CreateINDoc
		public abstract class createINDoc : PX.Data.IBqlField
		{
		}
		protected Boolean? _CreateINDoc;
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? CreateINDoc
		{
			get
			{
				return this._CreateINDoc;
			}
			set
			{
				this._CreateINDoc = value;
			}
		}
		#endregion
		#region InvtDocType
		public abstract class invtDocType : PX.Data.IBqlField
		{
		}
		protected String _InvtDocType;
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Inventory Doc. Type", Enabled = false)]
		[INDocType.List()]
		public virtual String InvtDocType
		{
			get
			{
				return this._InvtDocType;
			}
			set
			{
				this._InvtDocType = value;
			}
		}
		#endregion
		#region InvtRefNbr
		public abstract class invtRefNbr : PX.Data.IBqlField
		{
		}
		protected String _InvtRefNbr;
		[PXDBString(15, IsUnicode = true, InputMask = "")]
		[PXUIField(DisplayName = "Inventory Ref. Nbr.", Enabled = false)]
		[PXSelector(typeof(Search<INRegister.refNbr, Where<INRegister.docType, Equal<Current<SOOrderShipment.invtDocType>>>>))]
		public virtual String InvtRefNbr
		{
			get
			{
				return this._InvtRefNbr;
			}
			set
			{
				this._InvtRefNbr = value;
			}
		}
        #endregion
        #region HasDetailDeleted
        public abstract class hasDetailDeleted : PX.Data.IBqlField
        {
        }
        protected Boolean? _HasDetailDeleted;
        [PXBool()]
        public virtual Boolean? HasDetailDeleted
        {
            get
            {
                return this._HasDetailDeleted;
            }
            set
            {
                this._HasDetailDeleted = value;
            }
        }
        #endregion
        //TODO: remove from DB Schema
        /*
		#region OrigPOType
		public abstract class origPOType : PX.Data.IBqlField
		{
		}
		protected String _OrigPOType;
		[PXDBString(2, IsFixed = true)]
		[PO.POOrderType.List()]
		[PXUIField(DisplayName = "Orig. Type", Enabled = false)]
		public virtual String OrigPOType
		{
			get
			{
				return this._OrigPOType;
			}
			set
			{
				this._OrigPOType = value;
			}
		}
		#endregion
		#region OrigPONbr
		public abstract class origPONbr : PX.Data.IBqlField
		{
		}
		protected String _OrigPoNbr;
		[PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXUIField(DisplayName = "Orig. Nbr.", Enabled = false)]
		[PXSelector(typeof(Search<PO.POOrder.orderNbr,
			Where<PO.POOrder.orderType, Equal<PO.POOrderType.transfer>>>), Filterable = true)]
		public virtual String OrigPONbr
		{
			get
			{
				return this._OrigPoNbr;
			}
			set
			{
				this._OrigPoNbr = value;
			}
		}
		#endregion
        */
        #region NoteID
        public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[CopiedNoteID(new Type[0])]
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
		public static implicit operator SOOrderShipment(SOOrder item)
		{
			SOOrderShipment ret = new SOOrderShipment();
			ret.OrderType = item.OrderType;
			ret.OrderNbr = item.OrderNbr;
			ret.ShipAddressID = item.ShipAddressID;
			ret.ShipContactID = item.ShipContactID;
			ret.ShipmentType = null;
			ret.ShipmentNbr = Constants.NoShipmentNbr;
			ret.Operation = item.DefaultOperation;
			ret.ShipDate = item.ShipDate;
			ret.CustomerID = item.CustomerID;
			ret.CustomerLocationID = item.CustomerLocationID;
			ret.SiteID = null;
			ret.ShipmentQty = item.OrderQty;
			ret.ShipmentVolume = item.OrderVolume;
			ret.ShipmentWeight = item.OrderWeight;
			ret.LineTotal = item.LineTotal;
			//ret.OrigPOType = item.OrigPOType;
			//ret.OrigPONbr = item.OrigPONbr;
			//confirmed should be true for non-shipping orders
			ret.Confirmed = true;
			ret.LineTotal = item.LineTotal;

			return ret;
		}
		
		public static implicit operator SOOrderShipment(PXResult<SOOrderReceipt, SOOrder> item)
		{
			SOOrderShipment ret = new SOOrderShipment();
			ret.OrderType = ((SOOrder)item).OrderType;
			ret.OrderNbr = ((SOOrder)item).OrderNbr;
			ret.ShipAddressID = ((SOOrder)item).ShipAddressID;
			ret.ShipContactID = ((SOOrder)item).ShipContactID;
			ret.ShipmentType = INDocType.DropShip;
			ret.ShipmentNbr = ((SOOrderReceipt)item).ReceiptNbr;
			ret.Operation = SOOperation.Issue;
			ret.ShipDate = ((SOOrderReceipt)item).ReceiptDate;
			ret.CustomerID = ((SOOrder)item).CustomerID;
			ret.CustomerLocationID = ((SOOrder)item).CustomerLocationID;
			ret.SiteID = null;
			ret.ShipmentWeight = ((SOOrderReceipt)item).ExtWeight;
			ret.ShipmentVolume = ((SOOrderReceipt)item).ExtVolume;
			ret.ShipmentQty = ((SOOrderReceipt)item).ReceiptQty;
			ret.LineTotal = 0m;
			//ret.OrigPOType = ((SOOrder)item).OrigPOType;
			//ret.OrigPONbr = ((SOOrder)item).OrigPONbr;
			//confirmed should be true for non-shipping orders
			ret.Confirmed = true;

			return ret;
		}

		public static implicit operator SOOrderShipment(PXResult<POReceipt, SOOrder, POReceiptLine> item)
		{
			return new PXResult<SOOrder, POReceiptLine>(item, item);
		}

		public static implicit operator SOOrderShipment(PXResult<SOOrder, POReceiptLine> item)
		{
			SOOrderShipment ret = new SOOrderShipment();
			ret.OrderType = ((SOOrder)item).OrderType;
			ret.OrderNbr = ((SOOrder)item).OrderNbr;
			ret.ShipAddressID = ((SOOrder)item).ShipAddressID;
			ret.ShipContactID = ((SOOrder)item).ShipContactID;
			ret.ShipmentType = INDocType.DropShip;
			ret.ShipmentNbr = ((POReceiptLine)item).ReceiptNbr;
			ret.Operation = SOOperation.Issue;
			ret.ShipDate = ((POReceiptLine)item).ReceiptDate;
			ret.CustomerID = ((SOOrder)item).CustomerID;
			ret.CustomerLocationID = ((SOOrder)item).CustomerLocationID;
			ret.SiteID = null;
			ret.ShipmentWeight = ((POReceiptLine)item).ExtWeight;
			ret.ShipmentVolume = ((POReceiptLine)item).ExtVolume;
			ret.ShipmentQty = ((POReceiptLine)item).ReceiptQty;
			//ret.OrigPOType = ((SOOrder)item).OrigPOType;
			//ret.OrigPONbr = ((SOOrder)item).OrigPONbr;
			ret.LineTotal = 0m;
			//confirmed should be true for non-shipping orders
			ret.Confirmed = true;

			return ret;
		}
		#endregion
		#region Attributes
		public class CopiedNoteIDAttribute : PXNoteAttribute
		{
			public CopiedNoteIDAttribute(params Type[] searches)
				: base(searches)
			{
			}
			public override void RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
			{
			}
		}
		#endregion
	}

	public class SOShipmentType : INDocType
	{
		public new class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
			new string[] { Issue, DropShip, Transfer },
			new string[] { Messages.Normal, IN.Messages.DropShip, IN.Messages.Transfer }) { }
		}
	}
}
