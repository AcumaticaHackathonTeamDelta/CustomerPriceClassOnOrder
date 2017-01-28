namespace PX.Objects.PO
{
	using System;
	using PX.Data;
	using PX.Objects.CS;
	using PX.Objects.EP;
	using PX.Objects.GL;

	[System.SerializableAttribute()]
	[PXPrimaryGraph(typeof(POSetupMaint))]
	[PXCacheName(Messages.POSetupMaint)]
	public partial class POSetup : PX.Data.IBqlTable
	{
		#region StandardPONumberingID
		public abstract class standardPONumberingID : PX.Data.IBqlField
		{
		}
		protected String _StandardPONumberingID;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault("POORDER")]
		[PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
		[PXUIField(DisplayName = "Blanket Order Numbering Sequence", Visibility = PXUIVisibility.Visible)]
		public virtual String StandardPONumberingID
		{
			get
			{
				return this._StandardPONumberingID;
			}
			set
			{
				this._StandardPONumberingID = value;
			}
		}
		#endregion
		#region RegularPONumberingID
		public abstract class regularPONumberingID : PX.Data.IBqlField
		{
		}
		protected String _RegularPONumberingID;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault("POORDER")]
		[PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
		[PXUIField(DisplayName = "Regular Order Numbering Sequence", Visibility = PXUIVisibility.Visible)]
		public virtual String RegularPONumberingID
		{
			get
			{
				return this._RegularPONumberingID;
			}
			set
			{
				this._RegularPONumberingID = value;
			}
		}
		#endregion
		#region ReceiptNumberingID
		public abstract class receiptNumberingID : PX.Data.IBqlField
		{
		}
		protected String _ReceiptNumberingID;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault("PORECEIPT")]
		[PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
		[PXUIField(DisplayName = "Receipt Numbering Sequence", Visibility = PXUIVisibility.Visible)]
		public virtual String ReceiptNumberingID
		{
			get
			{
				return this._ReceiptNumberingID;
			}
			set
			{
				this._ReceiptNumberingID = value;
			}
		}
		#endregion
		#region RequireReceiptControlTotal
		public abstract class requireReceiptControlTotal : PX.Data.IBqlField
		{
		}
		protected Boolean? _RequireReceiptControlTotal;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "For Receipts")]
		public virtual Boolean? RequireReceiptControlTotal
		{
			get
			{
				return this._RequireReceiptControlTotal;
			}
			set
			{
				this._RequireReceiptControlTotal = value;
			}
		}
		#endregion
		#region RequireOrderControlTotal
		public abstract class requireOrderControlTotal : PX.Data.IBqlField
		{
		}
		protected Boolean? _RequireOrderControlTotal;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "For Normal and Standard Orders")]
		public virtual Boolean? RequireOrderControlTotal
		{
			get
			{
				return this._RequireOrderControlTotal;
			}
			set
			{
				this._RequireOrderControlTotal = value;
			}
		}
		#endregion
		#region RequireBlanketControlTotal
		public abstract class requireBlanketControlTotal : PX.Data.IBqlField
		{
		}
		protected Boolean? _RequireBlanketControlTotal;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "For Blanket Orders")]
		public virtual Boolean? RequireBlanketControlTotal
		{
			get
			{
				return this._RequireBlanketControlTotal;
			}
			set
			{
				this._RequireBlanketControlTotal = value;
			}
		}
		#endregion
		#region RequireDropShipControlTotal
		public abstract class requireDropShipControlTotal : PX.Data.IBqlField
		{
		}
		protected Boolean? _RequireDropShipControlTotal;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "For Drop-Ship Orders")]
		public virtual Boolean? RequireDropShipControlTotal
		{
			get
			{
				return this._RequireDropShipControlTotal;
			}
			set
			{
				this._RequireDropShipControlTotal = value;
			}
		}
		#endregion
		#region HoldReceipts
		public abstract class holdReceipts : PX.Data.IBqlField
		{
		}
		protected Boolean? _HoldReceipts;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Hold Receipts on Entry")]
		public virtual Boolean? HoldReceipts
		{
			get
			{
				return this._HoldReceipts;
			}
			set
			{
				this._HoldReceipts = value;
			}
		}
		#endregion
		#region OrderRequestApproval
		public abstract class orderRequestApproval : PX.Data.IBqlField
		{
		}
		protected bool? _OrderRequestApproval;
		[EPRequireApproval]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Null)]
		[PXUIField(DisplayName = "Require Approval")]
		public virtual bool? OrderRequestApproval
		{
			get
			{
				return this._OrderRequestApproval;
			}
			set
			{
				this._OrderRequestApproval = value;
			}
		}
		#endregion
		#region DefaultReceiptAssignmentMapID
		public abstract class defaultReceiptAssignmentMapID : PX.Data.IBqlField
		{
		}
		protected int? _DefaultReceiptAssignmentMapID;
		[PXDBInt]
		[PXSelector(typeof(Search<EPAssignmentMap.assignmentMapID, Where<EPAssignmentMap.entityType, Equal<AssignmentMapType.AssignmentMapTypePurchaseOrderReceipt>>>))]
		[PXUIField(DisplayName = "Receipt Assignment Map")]
		public virtual int? DefaultReceiptAssignmentMapID
		{
			get
			{
				return this._DefaultReceiptAssignmentMapID;
			}
			set
			{
				this._DefaultReceiptAssignmentMapID = value;
			}
		}
		#endregion
		#region ReceiptRequestApproval
		public abstract class receiptRequestApproval : PX.Data.IBqlField
		{
		}
		protected bool? _ReceiptRequestApproval;
		[PXDBBool]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Null)]
		[PXUIField(DisplayName = "Require Approval")]
		public virtual bool? ReceiptRequestApproval
		{
			get
			{
				return this._ReceiptRequestApproval;
			}
			set
			{
				this._ReceiptRequestApproval = value;
			}
		}
		#endregion
		#region AutoCreateInvoiceOnReceipt
		public abstract class autoCreateInvoiceOnReceipt : PX.Data.IBqlField
		{
		}
		protected Boolean? _AutoCreateInvoiceOnReceipt;
		[PXDBBool()]
		[PXUIField(DisplayName = "Create Bill on Receipt Release", Visibility = PXUIVisibility.Visible)]
		[PXDefault(false)]
		public virtual Boolean? AutoCreateInvoiceOnReceipt
		{
			get
			{
				return this._AutoCreateInvoiceOnReceipt;
			}
			set
			{
				this._AutoCreateInvoiceOnReceipt = value;
			}
		}
		#endregion
		#region FreightExpenseAcctID
		public abstract class freightExpenseAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _FreightExpenseAcctID;
		[Account(DisplayName = "Freight Expense Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		public virtual Int32? FreightExpenseAcctID
		{
			get
			{
				return this._FreightExpenseAcctID;
			}
			set
			{
				this._FreightExpenseAcctID = value;
			}
		}
		#endregion
		#region FreightExpenseSubID
		public abstract class freightExpenseSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _FreightExpenseSubID;
		[SubAccount(typeof(POSetup.freightExpenseAcctID), DisplayName = "Freight Expense Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? FreightExpenseSubID
		{
			get
			{
				return this._FreightExpenseSubID;
			}
			set
			{
				this._FreightExpenseSubID = value;
			}
		}
		#endregion
		#region RCReturnReasonCodeID
		public abstract class rCReturnReasonCodeID : PX.Data.IBqlField
		{
		}
		protected String _RCReturnReasonCodeID;
		[PXDBString(ReasonCode.reasonCodeID.Length, IsUnicode = true)]
		[PXSelector(typeof(Search<ReasonCode.reasonCodeID, Where<ReasonCode.usage, Equal<ReasonCodeUsages.issue>>>), DescriptionField = typeof(ReasonCode.descr))]
		[PXUIField(DisplayName = "PO Return Reason Code")]
		[PXDefault()]
		public virtual String RCReturnReasonCodeID
		{
			get
			{
				return this._RCReturnReasonCodeID;
			}
			set
			{
				this._RCReturnReasonCodeID = value;
			}
		}
		#endregion
		#region PPVAllocationMode
		public abstract class pPVAllocationMode : PX.Data.IBqlField
		{
		}
		protected String _PPVAllocationMode;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(PPVMode.PPVAccount)]
		[PXUIField(DisplayName = "Allocation Mode", Visibility = PXUIVisibility.Visible)]
		[PPVMode.List]
		public virtual String PPVAllocationMode
		{
			get
			{
				return _PPVAllocationMode;
			}
			set
			{
				this._PPVAllocationMode = value;
			}
		}
		#endregion
		#region PPVReasonCodeID
		public abstract class pPVReasonCodeID : PX.Data.IBqlField
		{
		}
		protected String _PPVReasonCodeID;
		[PXDBString(ReasonCode.reasonCodeID.Length, IsUnicode = true)]
		[PXSelector(typeof(Search<ReasonCode.reasonCodeID, Where<ReasonCode.usage, Equal<ReasonCodeUsages.adjustment>>>), DescriptionField = typeof(ReasonCode.descr))]
		[PXUIField(DisplayName = "Reason Code")]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual String PPVReasonCodeID
		{
			get
			{
				return this._PPVReasonCodeID;
			}
			set
			{
				this._PPVReasonCodeID = value;
			}
		}
		#endregion
		#region AutoReleaseAP
		public abstract class autoReleaseAP : PX.Data.IBqlField
		{
		}
		protected bool? _AutoReleaseAP;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Release AP Documents Automatically")]
		public virtual bool? AutoReleaseAP
		{
			get
			{
				return this._AutoReleaseAP;
			}
			set
			{
				this._AutoReleaseAP = value;
			}
		}
		#endregion
		#region AutoReleaseIN
		public abstract class autoReleaseIN : PX.Data.IBqlField
		{
		}
		protected bool? _AutoReleaseIN;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Release IN Documents Automatically")]
		public virtual bool? AutoReleaseIN
		{
			get
			{
				return this._AutoReleaseIN;
			}
			set
			{
				this._AutoReleaseIN = value;
			}
		}
		#endregion
		#region AutoReleaseLCIN
		public abstract class autoReleaseLCIN : PX.Data.IBqlField
		{
		}
		protected bool? _AutoReleaseLCIN;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Release LC IN Adjustments Automatically")]
		public virtual bool? AutoReleaseLCIN
		{
			get
			{
				return this._AutoReleaseLCIN;
			}
			set
			{
				this._AutoReleaseLCIN = value;
			}
		}
		#endregion
		#region CopyLineDescrSO
		public abstract class copyLineDescrSO : PX.Data.IBqlField
		{
		}
		protected bool? _CopyLineDescrSO;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Copy Line Descriptions from Sales Orders")]
		public virtual bool? CopyLineDescrSO
		{
			get
			{
				return this._CopyLineDescrSO;
			}
			set
			{
				this._CopyLineDescrSO = value;
			}
		}
		#endregion
		#region CopyLineNoteSO
		public abstract class copyLineNoteSO : PX.Data.IBqlField
		{
		}
		protected bool? _CopyLineNoteSO;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Copy Line Notes from Sales Orders")]
		public virtual bool? CopyLineNoteSO
		{
			get
			{
				return this._CopyLineNoteSO;
			}
			set
			{
				this._CopyLineNoteSO = value;
			}
		}
		#endregion
		#region ShipDestType
		public abstract class shipDestType : PX.Data.IBqlField
		{
		}
		protected string _ShipDestType;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(POShipDestType.Location)]
		[POShipDestType.List]
		[PXUIField(DisplayName = "Default Ship Dest. Type")]
		public virtual string ShipDestType
		{
			get
			{
				return this._ShipDestType;
			}
			set
			{
				this._ShipDestType = value;
			}
		}
		#endregion
		#region UpdateSubOnOwnerChange
		public abstract class updateSubOnOwnerChange : PX.Data.IBqlField
		{
		}
		protected bool? _UpdateSubOnOwnerChange;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Update Sub. on Order Owner Change", FieldClass = SubAccountAttribute.DimensionName)]
		public virtual bool? UpdateSubOnOwnerChange
		{
			get
			{
				return this._UpdateSubOnOwnerChange;
			}
			set
			{
				this._UpdateSubOnOwnerChange = value;
			}
		}
		#endregion
		#region VendorPriceUpdate
		public abstract class vendorPriceUpdate : PX.Data.IBqlField
		{
		}
		protected String _VendorPriceUpdate;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(POVendorPriceUpdateType.Purchase)]
		[PXUIField(DisplayName = "Vendor Price Update", Visibility = PXUIVisibility.Visible)]
		[POVendorPriceUpdateType.List]
		public virtual String VendorPriceUpdate
		{
			get
			{
				return _VendorPriceUpdate;
			}
			set
			{
				this._VendorPriceUpdate = value;
			}
		}
		#endregion
		#region AutoAddLineReceiptBarcode
		public abstract class autoAddLineReceiptBarcode : PX.Data.IBqlField
		{
		}
		protected bool? _AutoAddLineReceiptBarcode;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Automatically Add Receipt Line for Barcode")]
		public virtual bool? AutoAddLineReceiptBarcode
		{
			get
			{
				return this._AutoAddLineReceiptBarcode;
			}
			set
			{
				this._AutoAddLineReceiptBarcode = value;
			}
		}
		#endregion
		#region ReceiptByOneBarcodeReceiptBarcode
		public abstract class receiptByOneBarcodeReceiptBarcode : PX.Data.IBqlField
		{
		}
		protected bool? _ReceiptByOneBarcodeReceiptBarcode;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Add One Unit per Barcode")]
		public virtual bool? ReceiptByOneBarcodeReceiptBarcode
		{
			get
			{
				return this._ReceiptByOneBarcodeReceiptBarcode;
			}
			set
			{
				this._ReceiptByOneBarcodeReceiptBarcode = value;
			}
		}
		#endregion
		#region DefaultReceiptQty
		public abstract class defaultReceiptQty : PX.Data.IBqlField
		{
		}
		protected string _DefaultReceiptQty;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(DefaultReceiptQuantity.OpenQty)]
		[DefaultReceiptQuantity.List]
		[PXUIField(DisplayName = "Default Receipt Quantity")]
		public virtual string DefaultReceiptQty
		{
			get
			{
				return this._DefaultReceiptQty;
			}
			set
			{
				this._DefaultReceiptQty = value;
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
		public virtual bool GetRequireControlTotal(string aOrderType)
		{
			bool result = false;
			switch (aOrderType)
			{
				case POOrderType.Blanket:
				case POOrderType.StandardBlanket:
					result = (bool)this.RequireBlanketControlTotal; break;
				case POOrderType.RegularOrder:
					result = (bool)this.RequireOrderControlTotal; break;
				case POOrderType.DropShip:
					result = (bool)this.RequireDropShipControlTotal; break;
			}
			return result;
		}
	}
	public class POShipDestType
	{
		public class List : PXStringListAttribute
		{
			public List()
				: base(
				new string[] { Location, Site },
				new string[] { Messages.ShipDestCompanyLocation, Messages.ShipDestSite })
			{
			}
		}
		public const string Location = "L";
		public const string Site = "S";
	}

	public class DefaultReceiptQuantity
	{
		public class List : PXStringListAttribute
		{
			public List()
				: base(
				new string[] { OpenQty, Zero },
				new string[] { Messages.OpenQuantity, Messages.ZeroQuantity })
			{
			}
		}
		public const string OpenQty = "O";
		public const string Zero = "Z";
	}

	public class POVendorPriceUpdateType
	{
		public class List : PXStringListAttribute
		{
			public List()
				: base(
				new string[] { None, Purchase, Receipt, ReleaseReceipt, ReleaseAPBill },
				new string[] { Messages.VendorUpdateNone, Messages.VendorUpdatePurchase, Messages.VendorUpdateReceipt, Messages.VendorUpdateReceiptRelease, Messages.VendorUpdateAPBillRelease })
			{
			}
		}
		public const string None = "N";
		public const string Purchase = "P";
		public const string Receipt = "R";
		public const string ReleaseReceipt = "S";
		public const string ReleaseAPBill = "B";

	}

	public class PPVMode
	{
		public class List : PXStringListAttribute
		{
			public List()
				: base(
				new string[] { Inventory, PPVAccount },
				new string[] { Messages.PPVInventory, Messages.PPVAccount })
			{
			}
		}
		public const string Inventory = "I";
		public const string PPVAccount = "A";
	}
}
