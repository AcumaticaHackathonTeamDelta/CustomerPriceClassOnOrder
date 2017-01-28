using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using PX.Objects.CR;
using PX.SM;
using PX.Data;
using PX.Objects.BQLConstants;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.SO;
using PX.Objects.PO;

namespace PX.Objects.IN
{
	using IQtyAllocated = PX.Objects.IN.Overrides.INDocumentRelease.IQtyAllocated;

	#region QtyAllocType

	public static class QtyAllocType // = buckets
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base
						(
				new string[] { "undefined" },
				new string[] { "undefined" }
						)
			{ }

			public override void CacheAttached(PXCache sender)
			{
				PXCache cache = sender.Graph.Caches[typeof(INPlanType)];

				List<string> values = new List<string>();
				List<string> labels = new List<string>();

				foreach (string fieldName in cache.Fields)
				{
					object val = cache.GetStateExt(null, fieldName);

					if (val is PXIntState)
					{
						values.Add(char.ToLower(fieldName[0]) + fieldName.Substring(1));
						labels.Add(((PXIntState)val).DisplayName);
					}
				}

				this._AllowedValues = values.ToArray();
				this._AllowedLabels = labels.ToArray();
			}
		}
	}
	#endregion

	#region QtyAllocDocType
	public static class QtyAllocDocType
	{
		public const string INTransfer = "INTransferEntry";
		public const string INReceipt = "INReceiptEntry";
		public const string INIssue = "INIssueEntry";

		public const string POOrder = "POOrderEntry";
		public const string POReceipt = "POReceiptEntry";

		public const string SOOrder = "SOOrderEntry";
		public const string SOShipment = "SOShipmentEntry";
		public const string SOInvoice = "SOInvoiceEntry";
		public const string INReplenishmentOrder = "INReplenishmentMaint";

		// .. possibly to be extended

		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base
						(
		new string[] { QtyAllocDocType.INTransfer, QtyAllocDocType.INReceipt, QtyAllocDocType.INIssue, QtyAllocDocType.POOrder, QtyAllocDocType.POReceipt, QtyAllocDocType.SOOrder, QtyAllocDocType.SOShipment, QtyAllocDocType.SOInvoice, QtyAllocDocType.INReplenishmentOrder },
								new string[] { Messages.qadINTransfer, Messages.qadINReceipt, Messages.qadINIssue, Messages.qadPOOrder, Messages.qadPOReceipt, Messages.qadSOOrder, Messages.qadSOShipment, Messages.qadSOInvoice, Messages.qadSOReplenishment }
						)
			{ }
		}
	}
	#endregion

	#region Filter

	[Serializable]
	public partial class InventoryAllocDetEnqFilter : PX.Data.IBqlTable, IQtyAllocated
	{

		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[PXDefault()]
		[Inventory(typeof(Search<InventoryItem.inventoryID, Where<InventoryItem.stkItem, NotEqual<boolFalse>, And<Where<Match<Current<AccessInfo.userName>>>>>>), typeof(InventoryItem.inventoryCD), typeof(InventoryItem.descr))]
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

		#region SubItemCD
		public abstract class subItemCD : PX.Data.IBqlField
		{
		}
		protected String _SubItemCD;
		[SubItemRawExt(typeof(InventoryAllocDetEnqFilter.inventoryID), DisplayName = "Subitem")]
		public virtual String SubItemCD
		{
			get
			{
				return this._SubItemCD;
			}
			set
			{
				this._SubItemCD = value;
			}
		}
		#endregion
		#region SubItemCDWildcard
		public abstract class subItemCDWildcard : PX.Data.IBqlField { };
		[PXDBString(30, IsUnicode = true)]
		public virtual String SubItemCDWildcard
		{
			get
			{
				//return SubItemCDUtils.CreateSubItemCDWildcard(this._SubItemCD);
				return SubCDUtils.CreateSubCDWildcard(this._SubItemCD, SubItemAttribute.DimensionName);
			}
		}
		#endregion

		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SiteID;
		//        [Site(Visibility = PXUIVisibility.Visible)]
		[Site(DescriptionField = typeof(INSite.descr), DisplayName = "Warehouse")]
		//        [PXDefault()]
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
		[Location(typeof(InventoryAllocDetEnqFilter.siteID), KeepEntry = false, DescriptionField = typeof(INLocation.descr), DisplayName = "Location")]
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
		[LotSerialNbr]
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
		#region LotSerialNbrWildcard
		public abstract class lotSerialNbrWildcard : PX.Data.IBqlField { };
		[PXDBString(100, IsUnicode = true)]
		public virtual String LotSerialNbrWildcard
		{
			get
			{
				var sql = PXDatabase.Provider.SqlDialect;
				return sql.WildcardAnything + this._LotSerialNbr + sql.WildcardAnything;
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
		[PXUIField(DisplayName = "On Hand", Enabled = false)]
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

		#region QtyTotalAddition
		public abstract class qtyTotalAddition : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyTotalAddition;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Total Addition", Enabled = false)]
		public virtual Decimal? QtyTotalAddition
		{
			get
			{
				return this._QtyTotalAddition;
			}
			set
			{
				this._QtyTotalAddition = value;
			}
		}
		#endregion

		#region QtyPOPrepared
		public abstract class qtyPOPrepared : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyPOPrepared;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Purchase Prepared", Enabled = false)]
		public virtual Decimal? QtyPOPrepared
		{
			get
			{
				return this._QtyPOPrepared;
			}
			set
			{
				this._QtyPOPrepared = value;
			}
		}
		#endregion
		#region InclQtyPOPrepared
		public abstract class inclQtyPOPrepared : PX.Data.IBqlField
		{
		}
		protected bool? _InclQtyPOPrepared;
		[PXDBBool()]
		[PXUIField(DisplayName = " ", Enabled = false)]
		public virtual bool? InclQtyPOPrepared
		{
			get
			{
				return this._InclQtyPOPrepared;
			}
			set
			{
				this._InclQtyPOPrepared = value;
			}
		}
		#endregion


		#region QtyPOOrders
		public abstract class qtyPOOrders : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyPOOrders;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Purchase Orders", Enabled = false)]
		public virtual Decimal? QtyPOOrders
		{
			get
			{
				return this._QtyPOOrders;
			}
			set
			{
				this._QtyPOOrders = value;
			}
		}
		#endregion
		#region InclQtyPOOrders
		public abstract class inclQtyPOOrders : PX.Data.IBqlField
		{
		}
		protected bool? _InclQtyPOOrders;
		[PXDBBool()]
		[PXUIField(DisplayName = " ", Enabled = false)]
		public virtual bool? InclQtyPOOrders
		{
			get
			{
				return this._InclQtyPOOrders;
			}
			set
			{
				this._InclQtyPOOrders = value;
			}
		}
		#endregion

		#region QtyPOReceipts
		public abstract class qtyPOReceipts : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyPOReceipts;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "PO Receipts", Enabled = false)]
		public virtual Decimal? QtyPOReceipts
		{
			get
			{
				return this._QtyPOReceipts;
			}
			set
			{
				this._QtyPOReceipts = value;
			}
		}
		#endregion
		#region InclQtyPOReceipts
		public abstract class inclQtyPOReceipts : PX.Data.IBqlField
		{
		}
		protected bool? _InclQtyPOReceipts;
		[PXDBBool()]
		[PXUIField(DisplayName = " ", Enabled = false)]
		public virtual bool? InclQtyPOReceipts
		{
			get
			{
				return this._InclQtyPOReceipts;
			}
			set
			{
				this._InclQtyPOReceipts = value;
			}
		}
		#endregion

		#region QtyINReceipts
		public abstract class qtyINReceipts : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyINReceipts;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "IN Receipts [*]", Enabled = false)]
		public virtual Decimal? QtyINReceipts
		{
			get
			{
				return this._QtyINReceipts;
			}
			set
			{
				this._QtyINReceipts = value;
			}
		}
		#endregion
		#region InclQtyINReceipts
		public abstract class inclQtyINReceipts : PX.Data.IBqlField
		{
		}
		protected bool? _InclQtyINReceipts;
		[PXDBBool()]
		[PXUIField(DisplayName = " ", Enabled = false)]
		public virtual bool? InclQtyINReceipts
		{
			get
			{
				return this._InclQtyINReceipts;
			}
			set
			{
				this._InclQtyINReceipts = value;
			}
		}
		#endregion

		#region QtyINIssue
		public abstract class qtyINIssues : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyINIssues;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "IN Issues [**]", Enabled = false)]
		public virtual Decimal? QtyINIssues
		{
			get
			{
				return this._QtyINIssues;
			}
			set
			{
				this._QtyINIssues = value;
			}
		}
		#endregion
		#region InclQtyINIssues
		public abstract class inclQtyINIssues : PX.Data.IBqlField
		{
		}
		protected bool? _InclQtyINIssues;
		[PXDBBool()]
		[PXUIField(DisplayName = " ", Enabled = false)]
		public virtual bool? InclQtyINIssues
		{
			get
			{
				return this._InclQtyINIssues;
			}
			set
			{
				this._InclQtyINIssues = value;
			}
		}
		#endregion

		#region QtyInTransit
		public abstract class qtyInTransit : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyInTransit;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "In Transit [**]", Enabled = false)]
		public virtual Decimal? QtyInTransit
		{
			get
			{
				return this._QtyInTransit;
			}
			set
			{
				this._QtyInTransit = value;
			}
		}
		#endregion
		#region InclQtyInTransit
		public abstract class inclQtyInTransit : PX.Data.IBqlField
		{
		}
		protected bool? _InclQtyInTransit;
		[PXDBBool()]
		[PXUIField(DisplayName = " ", Enabled = false)]
		public virtual bool? InclQtyInTransit
		{
			get
			{
				return this._InclQtyInTransit;
			}
			set
			{
				this._InclQtyInTransit = value;
			}
		}
		#endregion

        #region QtyInTransitToSO
        public abstract class qtyInTransitToSO : PX.Data.IBqlField
        {
        }
        protected Decimal? _QtyInTransitToSO;
        [PXDBQuantity()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "In Transit to SO", Enabled = false)]
        public virtual Decimal? QtyInTransitToSO
        {
            get
            {
                return this._QtyInTransitToSO;
            }
            set
            {
                this._QtyInTransitToSO = value;
            }
        }
        #endregion

		#region QtyTotalDeduction
		public abstract class qtyTotalDeduction : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyTotalDeduction;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Total Deduction", Enabled = false)]
		public virtual Decimal? QtyTotalDeduction
		{
			get
			{
				return this._QtyTotalDeduction;
			}
			set
			{
				this._QtyTotalDeduction = value;
			}
		}
		#endregion

		#region QtyNotAvail
		public abstract class qtyNotAvail : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyNotAvail;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "On Loc. Not Available", Enabled = false)]
		public virtual Decimal? QtyNotAvail
		{
			get
			{
				return this._QtyNotAvail;
			}
			set
			{
				this._QtyNotAvail = value;
			}
		}
		#endregion
		#region QtyExpired
		public abstract class qtyExpired : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyExpired;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Expired [*]", Enabled = false)]
		public virtual Decimal? QtyExpired
		{
			get
			{
				return this._QtyExpired;
			}
			set
			{
				this._QtyExpired = value;
			}
		}
		#endregion

		#region QtySOPrepared
		public abstract class qtySOPrepared : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtySOPrepared;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "SO Prepared", Enabled = false)]
		public virtual Decimal? QtySOPrepared
		{
			get
			{
				return this._QtySOPrepared;
			}
			set
			{
				this._QtySOPrepared = value;
			}
		}
		#endregion
		#region InclQtySOPrepared
		public abstract class inclQtySOPrepared : PX.Data.IBqlField
		{
		}
		protected bool? _InclQtySOPrepared;
		[PXDBBool()]
		[PXUIField(DisplayName = " ", Enabled = false)]
		public virtual bool? InclQtySOPrepared
		{
			get
			{
				return this._InclQtySOPrepared;
			}
			set
			{
				this._InclQtySOPrepared = value;
			}
		}
		#endregion

		#region QtySOBooked
		public abstract class qtySOBooked : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtySOBooked;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "SO Booked [**]", Enabled = false)]
		public virtual Decimal? QtySOBooked
		{
			get
			{
				return this._QtySOBooked;
			}
			set
			{
				this._QtySOBooked = value;
			}
		}
		#endregion
		#region InclQtySOBooked
		public abstract class inclQtySOBooked : PX.Data.IBqlField
		{
		}
		protected bool? _InclQtySOBooked;
		[PXDBBool()]
		[PXUIField(DisplayName = " ", Enabled = false)]
		public virtual bool? InclQtySOBooked
		{
			get
			{
				return this._InclQtySOBooked;
			}
			set
			{
				this._InclQtySOBooked = value;
			}
		}
		#endregion

		#region QtySOShipping
		public abstract class qtySOShipping : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtySOShipping;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "SO Allocated [**]", Enabled = false)]
		public virtual Decimal? QtySOShipping
		{
			get
			{
				return this._QtySOShipping;
			}
			set
			{
				this._QtySOShipping = value;
			}
		}
		#endregion
		#region InclQtySOShipping
		public abstract class inclQtySOShipping : PX.Data.IBqlField
		{
		}
		protected bool? _InclQtySOShipping;
		[PXDBBool()]
		[PXUIField(DisplayName = " ", Enabled = false)]
		public virtual bool? InclQtySOShipping
		{
			get
			{
				return this._InclQtySOShipping;
			}
			set
			{
				this._InclQtySOShipping = value;
			}
		}
		#endregion

		#region QtySOShipped
		public abstract class qtySOShipped : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtySOShipped;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "SO Shipped [**]", Enabled = false)]
		public virtual Decimal? QtySOShipped
		{
			get
			{
				return this._QtySOShipped;
			}
			set
			{
				this._QtySOShipped = value;
			}
		}
		#endregion
		#region InclQtySOShipped
		public abstract class inclQtySOShipped : PX.Data.IBqlField
		{
		}
		protected bool? _InclQtySOShipped;
		[PXDBBool()]
		[PXUIField(DisplayName = " ", Enabled = false)]
		public virtual bool? InclQtySOShipped
		{
			get
			{
				return this._InclQtySOShipped;
			}
			set
			{
				this._InclQtySOShipped = value;
			}
		}
		#endregion

		#region QtyINAssemblySupply
		public abstract class qtyINAssemblySupply : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyINAssemblySupply;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Kit Assembly Supply", Enabled = false)]
		public virtual Decimal? QtyINAssemblySupply
		{
			get
			{
				return this._QtyINAssemblySupply;
			}
			set
			{
				this._QtyINAssemblySupply = value;
			}
		}
		#endregion
		#region InclQtyINAssemblySupply
		public abstract class inclQtyINAssemblySupply : PX.Data.IBqlField
		{
		}
		protected bool? _InclQtyINAssemblySupply;
		[PXDBBool()]
		[PXUIField(DisplayName = " ", Enabled = false)]
		public virtual bool? InclQtyINAssemblySupply
		{
			get
			{
				return this._InclQtyINAssemblySupply;
			}
			set
			{
				this._InclQtyINAssemblySupply = value;
			}
		}
		#endregion

		#region QtyINAssemblyDemand
		public abstract class qtyINAssemblyDemand : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyINAssemblyDemand;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Kit Assembly Demand", Enabled = false)]
		public virtual Decimal? QtyINAssemblyDemand
		{
			get
			{
				return this._QtyINAssemblyDemand;
			}
			set
			{
				this._QtyINAssemblyDemand = value;
			}
		}
		#endregion
		#region InclQtyINAssemblyDemand
		public abstract class inclQtyINAssemblyDemand : PX.Data.IBqlField
		{
		}
		protected bool? _InclQtyINAssemblyDemand;
		[PXDBBool()]
		[PXUIField(DisplayName = " ", Enabled = false)]
		public virtual bool? InclQtyINAssemblyDemand
		{
			get
			{
				return this._InclQtyINAssemblyDemand;
			}
			set
			{
				this._InclQtyINAssemblyDemand = value;
			}
		}
		#endregion
		
		#region QtyINReplaned
		public abstract class qtyINReplaned : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyINReplaned;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "In Replaned", Enabled = false)]
		public virtual Decimal? QtyINReplaned
		{
			get
			{
				return this._QtyINReplaned;
			}
			set
			{
				this._QtyINReplaned = value;
			}
		}
		#endregion
		#region InclQtyINReplaned
		public abstract class inclQtyINReplaned : PX.Data.IBqlField
		{
		}
		protected bool? _InclQtyINReplaned;
		[PXDBBool()]
		[PXUIField(DisplayName = " ", Enabled = false)]
		public virtual bool? InclQtyINReplaned
		{
			get
			{
				return this._InclQtyINReplaned;
			}
			set
			{
				this._InclQtyINReplaned = value;
			}
		}
        #endregion

        #region InclQtyPOFixedReceipt
        public abstract class inclQtyPOFixedReceipt : PX.Data.IBqlField
        {
        }
        protected Boolean? _InclQtyPOFixedReceipt;
        [PXBool()]
        [PXDefault(typeof(False), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Boolean? InclQtyPOFixedReceipt
        {
            get
            {
                return this._InclQtyPOFixedReceipt;
            }
            set
            {
                this._InclQtyPOFixedReceipt = value;
            }
        }
        #endregion

        #region InclQtySOReverse
        public abstract class inclQtySOReverse : PX.Data.IBqlField
		{
		}
		protected Boolean? _InclQtySOReverse;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = " ", Enabled = false)]
		public virtual Boolean? InclQtySOReverse
		{
			get
			{
				return this._InclQtySOReverse;
			}
			set
			{
				this._InclQtySOReverse = value;
			}
		}
		#endregion
		#region QtySOBackOrdered
		public abstract class qtySOBackOrdered : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtySOBackOrdered;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "SO Back Ordered [**]", Enabled = false)]
		public virtual Decimal? QtySOBackOrdered
		{
			get
			{
				return this._QtySOBackOrdered;
			}
			set
			{
				this._QtySOBackOrdered = value;
			}
		}
		#endregion
		#region InclQtySOBackOrdered
		public abstract class inclQtySOBackOrdered : PX.Data.IBqlField
		{
		}
		protected Boolean? _InclQtySOBackOrdered;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = " ", Enabled = false)]
		public virtual Boolean? InclQtySOBackOrdered
		{
			get
			{
				return this._InclQtySOBackOrdered;
			}
			set
			{
				this._InclQtySOBackOrdered = value;
			}
		}
		#endregion
		#region QtyAvail
		public abstract class qtyAvail : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyAvail;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Available", Enabled = false)]
		public virtual Decimal? QtyAvail
		{
			get
			{
				return this._QtyAvail;
			}
			set
			{
				this._QtyAvail = value;
			}
		}
		#endregion
		#region QtyHardAvail
		public abstract class qtyHardAvail : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyHardAvail;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Available for Shipping", Enabled = false)]
		public virtual Decimal? QtyHardAvail
		{
			get
			{
				return this._QtyHardAvail;
			}
			set
			{
				this._QtyHardAvail = value;
			}
		}
		#endregion
		#region QtySOFixed
		public abstract class qtySOFixed : IBqlField { }
		protected decimal? _QtySOFixed;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "SO to Purchase", Enabled = false)]
		public virtual decimal? QtySOFixed
		{
			get
			{
				return this._QtySOFixed;
			}
			set
			{
				this._QtySOFixed = value;
			}
		}
		#endregion
		#region QtyPOFixedOrders
		public abstract class qtyPOFixedOrders : IBqlField { }
		protected decimal? _QtyPOFixedOrders;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Purchase for SO", Enabled = false)]
		public virtual decimal? QtyPOFixedOrders
		{
			get
			{
				return this._QtyPOFixedOrders;
			}
			set
			{
				this._QtyPOFixedOrders = value;
			}
		}
		#endregion
		#region QtyPOFixedPrepared
		public abstract class qtyPOFixedPrepared : IBqlField { }
		protected decimal? _QtyPOFixedPrepared;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Purchase for SO Prepared", Enabled = false)]
		public virtual decimal? QtyPOFixedPrepared
		{
			get
			{
				return this._QtyPOFixedPrepared;
			}
			set
			{
				this._QtyPOFixedPrepared = value;
			}
		}
		#endregion
		#region QtyPOFixedReceipts
		public abstract class qtyPOFixedReceipts : IBqlField { }
		protected decimal? _QtyPOFixedReceipts;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Receipts for SO", Enabled = false)]
		public virtual decimal? QtyPOFixedReceipts
		{
			get
			{
				return this._QtyPOFixedReceipts;
			}
			set
			{
				this._QtyPOFixedReceipts = value;
			}
		}
		#endregion
		#region QtySODropShip
		public abstract class qtySODropShip : IBqlField { }
		protected decimal? _QtySODropShip;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "SO to Drop-Ship", Enabled = false)]
		public virtual decimal? QtySODropShip
		{
			get
			{
				return this._QtySODropShip;
			}
			set
			{
				this._QtySODropShip = value;
			}
		}
		#endregion
		#region QtyPODropShipOrders
		public abstract class qtyPODropShipOrders : IBqlField { }
		protected decimal? _QtyPODropShipOrders;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Drop-Ship for SO", Enabled = false)]
		public virtual decimal? QtyPODropShipOrders
		{
			get
			{
				return this._QtyPODropShipOrders;
			}
			set
			{
				this._QtyPODropShipOrders = value;
			}
		}
		#endregion
		#region QtyPODropShipPrepared
		public abstract class qtyPODropShipPrepared : IBqlField { }
		protected decimal? _QtyPODropShipPrepared;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Drop-Ship for SO Prepared", Enabled = false)]
		public virtual decimal? QtyPODropShipPrepared
		{
			get
			{
				return this._QtyPODropShipPrepared;
			}
			set
			{
				this._QtyPODropShipPrepared = value;
			}
		}
		#endregion
		#region QtyPODropShipReceipts
		public abstract class qtyPODropShipReceipts : IBqlField { }
		protected decimal? _QtyPODropShipReceipts;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Drop-Ship for SO Receipts", Enabled = false)]
		public virtual decimal? QtyPODropShipReceipts
		{
			get
			{
				return this._QtyPODropShipReceipts;
			}
			set
			{
				this._QtyPODropShipReceipts = value;
			}
		}
		#endregion

		#region BaseUnit
		public abstract class baseUnit : PX.Data.IBqlField
		{
		}
		protected String _BaseUnit;
		[PXDefault("")]
		[INUnit(DisplayName = "Base Unit", Enabled = false)]
		public virtual String BaseUnit
		{
			get
			{
				return this._BaseUnit;
			}
			set
			{
				this._BaseUnit = value;
			}
		}

        #endregion
        #region NegQty
        public abstract class negQty : PX.Data.IBqlField
		{
		}
		protected bool? _NegQty;
		[PXDBBool()]
		[PXDefault(false)]
		public virtual bool? NegQty
		{
			get
			{
				return this._NegQty;
			}
			set
			{
				this._NegQty = value;
			}
		}
		#endregion
		#region InclQtyAvail
		public abstract class inclQtyAvail : PX.Data.IBqlField
		{
		}
		protected Boolean? _InclQtyAvail;
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? InclQtyAvail
		{
			get
			{
				return this._InclQtyAvail;
			}
			set
			{
				this._InclQtyAvail = value;
			}
		}
		#endregion


		#region Label
		public abstract class label : PX.Data.IBqlField
		{
		}
		protected String _Label;
		[PXString]
		[PXUIField]
		[PXDefault(Messages.ExceptLocationNotAvailable, PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual string Label
		{
			get
			{
				return this._Label;
			}
			set
			{
				this._Label = value;
			}
		}
		#endregion
		#region Label2
		public abstract class label2 : PX.Data.IBqlField
		{
		}
		protected String _Label2;
		[PXString]
		[PXUIField]
		[PXDefault(Messages.ExceptExpiredNotAvailable, PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual string Label2
		{
			get
			{
				return this._Label2;
			}
			set
			{
				this._Label2 = value;
			}
		}
		#endregion
	}

	#endregion

	#region //Additional DAC
	/*
    // INTran alias (to ensure there are no released positive part of transfer)
    public class INTranAlias : INTran
    {
        #region DocType
        public new abstract class docType : PX.Data.IBqlField { }
        #endregion
        #region OrigTranType
        public new abstract class origTranType : PX.Data.IBqlField { }
        #endregion
        #region OrigRefNbr
        public new abstract class origRefNbr : PX.Data.IBqlField { }
        #endregion
        #region OrigLineNbr
        public new abstract class origLineNbr : PX.Data.IBqlField { }
        #endregion
        #region InvtMult
        public new abstract class invtMult : PX.Data.IBqlField { }
        #endregion
        #region Released
        public new abstract class released : PX.Data.IBqlField { }
        #endregion        
    };
*/
	#endregion

	#region Resultset

    [Serializable]
	public partial class InventoryAllocDetEnqResult : PX.Data.IBqlTable, IQtyPlanned
	{
		public InventoryAllocDetEnqResult() { }

		#region Module
		public abstract class module : PX.Data.IBqlField { }
		protected String _Module;
		//[BatchModule.List()] // other list ???
		[PXString(2)]
		[PXUIField(DisplayName = "Module", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String Module
		{
			get
			{
				return this._Module;
			}
			set
			{
				this._Module = value;
			}
		}
		#endregion
		#region QADocType
		// QA means QtyAllocDocType , to not confuse with original DocType which is module-specific ...
		public abstract class qADocType : PX.Data.IBqlField { }
		protected String _QADocType;
		[PXString(15, IsUnicode = true)] // ???
		[PXUIField(DisplayName = "Document Type", Visibility = PXUIVisibility.SelectorVisible)]
		[QtyAllocDocType.List()]
		public virtual String QADocType
		{
			get
			{
				return this._QADocType;
			}
			set
			{
				this._QADocType = value;
			}
		}
		#endregion

		#region RefNoteID
		public abstract class refNoteID : PX.Data.IBqlField
		{
		}
		protected Guid? _RefNoteID;

		[PXRefNote]
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Guid? RefNoteID
		{
			get
			{
				return this._RefNoteID;
			}
			set
			{
				this._RefNoteID = value;
			}
		}
		#endregion

		#region SubItemID
		public abstract class subItemID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubItemID;
		[SubItem(Visibility = PXUIVisibility.SelectorVisible, DisplayName = "Subitem")]
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

		#region LocationID
		public abstract class locationID : PX.Data.IBqlField { }
		protected Int32? _LocationID;
		//            [PXDBInt(IsKey = true)] //???
		[Location(Visibility = PXUIVisibility.SelectorVisible, DisplayName = "Location")]
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

		#region LocNotAvailable
		public abstract class locNotAvailable : PX.Data.IBqlField { }
		protected Boolean? _LocNotAvailable;
		[PXBool]
		[PXUIField(DisplayName = "Loc. Not Available", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Boolean? LocNotAvailable
		{
			get
			{
				return this._LocNotAvailable;
			}
			set
			{
				this._LocNotAvailable = value;
			}
		}
		#endregion

		#region SiteId
		public abstract class siteID : PX.Data.IBqlField { }
		protected Int32? _SiteID;
		//[PXDBInt(IsKey = true)] //???
		[Site(Visibility = PXUIVisibility.SelectorVisible, DisplayName = "Warehouse")]
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


		#region LotSerialNbr
		public abstract class lotSerialNbr : PX.Data.IBqlField
		{
		}
		protected String _LotSerialNbr;
		[PXDBString(100, IsUnicode = true, InputMask = "")]
		[PXUIField(DisplayName = "Lot/Serial Number", Visibility = PXUIVisibility.SelectorVisible)]
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

		#region Expired
		public abstract class expired : PX.Data.IBqlField { }
		protected Boolean? _Expired;
		[PXBool]
		[PXUIField(DisplayName = "Expired", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Boolean? Expired
		{
			get
			{
				return this._Expired;
			}
			set
			{
				this._Expired = value;
			}
		}
		#endregion

		#region AllocationType
		public abstract class allocationType : PX.Data.IBqlField
		{
		}
		protected String _AllocationType;
		[PXString(50)]
		[PXUIField(DisplayName = "Allocation Type", Visibility = PXUIVisibility.SelectorVisible)]
		[QtyAllocType.List()]
		public virtual String AllocationType
		{
			get
			{
				return this._AllocationType;
			}
			set
			{
				this._AllocationType = value;
			}
		}
		#endregion
		#region PlanDate
		public abstract class planDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _PlanDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Allocation Date")]
		public virtual DateTime? PlanDate
		{
			get
			{
				return this._PlanDate;
			}
			set
			{
				this._PlanDate = value;
			}
		}
		#endregion
		#region PlanQty
		public abstract class planQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _PlanQty;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty.", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? PlanQty
		{
			get
			{
				return this._PlanQty;
			}
			set
			{
				this._PlanQty = value;
			}
		}
		#endregion
		#region Reverse
		public abstract class reverse : PX.Data.IBqlField
		{
		}
		protected Boolean? _Reverse;
		[PXDBBool()]
		[PXDefault()]
		[PXUIField(DisplayName = "Reverse")]
		public virtual Boolean? Reverse
		{
			get
			{
				return this._Reverse;
			}
			set
			{
				this._Reverse = value;
			}
		}
		#endregion
		#region //BaseUnit
		/*
			public abstract class baseUnit : PX.Data.IBqlField
			{
			}
			protected String _BaseUnit;
			[INUnit(DisplayName = "Base Unit")]
			public virtual String BaseUnit
			{
				get
				{
					return this._BaseUnit;
				}
				set
				{
					this._BaseUnit = value;
				}
			}
*/
		#endregion
		#region PlanID
		public abstract class planID : PX.Data.IBqlField
		{
		}
		protected Int64? _PlanID;
		[PXDBLong()]
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


		#region BAccountID
		public abstract class bAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _BAccountID;
		[PXInt()]
		[PXSelector(typeof(BAccount.bAccountID), SubstituteKey = typeof(BAccount.acctCD), DescriptionField = typeof(BAccount.acctName))]
		[PXUIField(DisplayName = "Account ID")]
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
		#region AcctName
		public abstract class acctName : PX.Data.IBqlField
		{
		}
		protected String _AcctName;
		[PXString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Account Name")]
		public virtual String AcctName
		{
			get
			{
				return this._AcctName;
			}
			set
			{
				this._AcctName = value;
			}
		}
		#endregion
	
		#region GridLineNbr
		// to be grid key
		public abstract class gridLineNbr : PX.Data.IBqlField { }
		protected Int32? _GridLineNbr;
		[PXDBInt(IsKey = true)]
		[PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Invisible, Visible = false)]
		public virtual Int32? GridLineNbr
		{
			get
			{
				return this._GridLineNbr;
			}
			set
			{
				this._GridLineNbr = value;
			}
		}
		#endregion

		public class EntityHelper : PX.Data.EntityHelper
		{
			public EntityHelper(PXGraph graph)
				: base(graph)
			{
			}
			public override object GetEntityRow(Type entityType, Guid? noteID)
			{
				return base.GetEntityRow(entityType == typeof(PX.Objects.SO.SOOrderShipment) ? typeof(PX.Objects.SO.SOOrder) : entityType, noteID);
			}
			protected override void NavigateToRow(Type cachetype, object row, PXRedirectHelper.WindowMode mode)
			{
				base.NavigateToRow(cachetype == typeof(PX.Objects.SO.SOOrderShipment) ? typeof(PX.Objects.SO.SOOrder) : cachetype, row, mode);
			}
		}
		
		public class PXRefNoteAttribute : PX.Data.PXRefNoteAttribute
		{
			public PXRefNoteAttribute()
			{ 
			}

			public override void CacheAttached(PXCache sender)
			{
				base.CacheAttached(sender);
				helper = new EntityHelper(sender.Graph);
			}
		}
	}

	#endregion


	[PX.Objects.GL.TableAndChartDashboardType]
	public class InventoryAllocDetEnq : PXGraph<InventoryAllocDetEnq>
    {
		public PXFilter<BAccount> Dummy_bAccount;
		public PXAction<InventoryAllocDetEnqFilter> RefreshTotal;
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.Refresh, Tooltip = Messages.ttipRefresh, SpecialType = PXSpecialButtonType.Refresh)]
		[PXUIField(DisplayName = "")]
		protected virtual IEnumerable refreshTotal(PXAdapter a)
		{
			Filter.Cache.Current = (InventoryAllocDetEnqFilter)Filter.Select();
			ResultRecords.View.RequestRefresh();
			return a.Get();
		}

		
		public PXCancel<InventoryAllocDetEnqFilter> Cancel;
		public PXAction<InventoryAllocDetEnqFilter> viewDocument;
		public PXFilter<InventoryAllocDetEnqFilter> Filter;
		[PXFilterable]
		public PXSelectOrderBy<InventoryAllocDetEnqResult, OrderBy<Asc<InventoryAllocDetEnqResult.module, Asc<InventoryAllocDetEnqResult.qADocType, Asc<InventoryAllocDetEnqResult.refNoteID>>>>> ResultRecords;

		

		bool WarehouseLocationFeature;
		bool LotSerialFeature;

		public InventoryAllocDetEnq()
		{
			ResultRecords.Cache.AllowInsert = false;
			ResultRecords.Cache.AllowDelete = false;
			ResultRecords.Cache.AllowUpdate = false;

			WarehouseLocationFeature = PXAccess.FeatureInstalled<FeaturesSet.warehouseLocation>();
			LotSerialFeature = PXAccess.FeatureInstalled<FeaturesSet.lotSerialTracking>();

			if (!WarehouseLocationFeature && !LotSerialFeature)
			{
				PXUIFieldAttribute.SetDisplayName<InventoryAllocDetEnqFilter.qtyInTransit>(Filter.Cache, Messages.InTransit);
				PXUIFieldAttribute.SetDisplayName<InventoryAllocDetEnqFilter.qtySOBooked>(Filter.Cache, Messages.SOBooked);
				PXUIFieldAttribute.SetDisplayName<InventoryAllocDetEnqFilter.qtySOShipping>(Filter.Cache, Messages.SOAllocated);
				PXUIFieldAttribute.SetDisplayName<InventoryAllocDetEnqFilter.qtySOShipped>(Filter.Cache, Messages.SOShipped);
				PXUIFieldAttribute.SetDisplayName<InventoryAllocDetEnqFilter.qtyINIssues>(Filter.Cache, Messages.INIssues);
				PXUIFieldAttribute.SetDisplayName<InventoryAllocDetEnqFilter.qtyINReceipts>(Filter.Cache, Messages.INReceipts);
			}
			else
			{
				if (WarehouseLocationFeature && LotSerialFeature)
				{
					PXUIFieldAttribute.SetDisplayName<InventoryAllocDetEnqFilter.qtyInTransit>(Filter.Cache, Messages.InTransit2S);
					PXUIFieldAttribute.SetDisplayName<InventoryAllocDetEnqFilter.qtySOBooked>(Filter.Cache, Messages.SOBooked2S);
					PXUIFieldAttribute.SetDisplayName<InventoryAllocDetEnqFilter.qtySOShipping>(Filter.Cache, Messages.SOAllocated2S);
					PXUIFieldAttribute.SetDisplayName<InventoryAllocDetEnqFilter.qtySOShipped>(Filter.Cache, Messages.SOShipped2S);
					PXUIFieldAttribute.SetDisplayName<InventoryAllocDetEnqFilter.qtyINIssues>(Filter.Cache, Messages.INIssues2S);
					PXUIFieldAttribute.SetDisplayName<InventoryAllocDetEnqFilter.qtyINReceipts>(Filter.Cache, Messages.INReceiptsS);
					PXUIFieldAttribute.SetDisplayName<InventoryAllocDetEnqFilter.qtyExpired>(Filter.Cache, Messages.ExpiredS);
				}
				else
				{
					PXUIFieldAttribute.SetDisplayName<InventoryAllocDetEnqFilter.qtyInTransit>(Filter.Cache, Messages.InTransitS);
					PXUIFieldAttribute.SetDisplayName<InventoryAllocDetEnqFilter.qtySOBooked>(Filter.Cache, Messages.SOBookedS);
					PXUIFieldAttribute.SetDisplayName<InventoryAllocDetEnqFilter.qtySOShipping>(Filter.Cache, Messages.SOAllocatedS);
					PXUIFieldAttribute.SetDisplayName<InventoryAllocDetEnqFilter.qtySOShipped>(Filter.Cache, Messages.SOShippedS);
					PXUIFieldAttribute.SetDisplayName<InventoryAllocDetEnqFilter.qtyINIssues>(Filter.Cache, Messages.INIssuesS);
					if (!WarehouseLocationFeature && LotSerialFeature)
					{
						PXUIFieldAttribute.SetDisplayName<InventoryAllocDetEnqFilter.qtyINReceipts>(Filter.Cache, Messages.INReceipts);
						PXUIFieldAttribute.SetDisplayName<InventoryAllocDetEnqFilter.qtyExpired>(Filter.Cache, Messages.Expired);
					}
				}
			}
		}

		protected virtual void InventoryAllocDetEnqFilter_LocationID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = null;
			e.Cancel = true;
		}

		protected virtual void InventoryAllocDetEnqFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			if (e.Row == null) return;
			InventoryAllocDetEnqFilter row = (InventoryAllocDetEnqFilter)e.Row;
			if (!WarehouseLocationFeature && !LotSerialFeature)
			{
				PXUIFieldAttribute.SetVisible<InventoryAllocDetEnqFilter.label2>(sender, row, false);
				PXUIFieldAttribute.SetVisible<InventoryAllocDetEnqFilter.label>(sender, row, false);
			}
			else
			{
				PXUIFieldAttribute.SetVisible<InventoryAllocDetEnqFilter.label2>(sender, row, true);
				PXUIFieldAttribute.SetVisible<InventoryAllocDetEnqFilter.label>(sender, row, true);
				row.Label2 = PXMessages.LocalizeNoPrefix(Messages.ExceptExpired2S);
				if (!WarehouseLocationFeature)
				{
					PXUIFieldAttribute.SetVisible<InventoryAllocDetEnqFilter.label>(sender, row, false);
					row.Label2 = PXMessages.LocalizeNoPrefix(Messages.ExceptExpiredS);
				}
				if (!LotSerialFeature)
				{
					PXUIFieldAttribute.SetVisible<InventoryAllocDetEnqFilter.label2>(sender, row, false);
				}
			}
			if (string.Equals(row.Label, Messages.ExceptLocationNotAvailable, StringComparison.Ordinal))
			{
				row.Label = PXMessages.LocalizeNoPrefix(Messages.ExceptLocationNotAvailable);
			}
		}

		protected virtual void InventoryAllocDetEnqResult_LocationID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = null;
			e.Cancel = true;
		}

		protected virtual IEnumerable filter()
		{

			PXCache cache = this.Caches[typeof(InventoryAllocDetEnqFilter)];
			if (cache != null)
			{
				InventoryAllocDetEnqFilter filter = cache.Current as InventoryAllocDetEnqFilter;
				if (filter != null)
				{
					filter.QtyOnHand = 0m;

					filter.QtyTotalAddition = 0m;

					filter.QtyPOPrepared = 0m;
					filter.QtyPOOrders = 0m;
					filter.QtyPOReceipts = 0m;
					filter.QtyINReceipts = 0m;
					filter.QtyInTransit = 0m;
                    filter.QtyInTransitToSO = 0m;
					filter.QtyINAssemblySupply = 0m;

					filter.QtyTotalDeduction = 0m;

					filter.QtyHardAvail = 0m;
					filter.QtyNotAvail = 0m;
					filter.QtyExpired = 0m;
					filter.QtySOPrepared = 0m;
					filter.QtySOBooked = 0m;
					filter.QtySOShipping = 0m;
					filter.QtySOShipped = 0m;
					filter.QtyINIssues = 0m;
					filter.QtyINAssemblyDemand = 0m;
					filter.QtySOBackOrdered = 0m;					
					filter.QtySOFixed = 0m;
					filter.QtyPOFixedOrders = 0m;
					filter.QtyPOFixedPrepared = 0m;
					filter.QtyPOFixedReceipts = 0m;
					filter.QtySODropShip = 0m;
					filter.QtyPODropShipOrders = 0m;
					filter.QtyPODropShipPrepared = 0m;
					filter.QtyPODropShipReceipts = 0m;

					filter.QtyAvail = 0m;


					// InventoryId is required field 
					if (filter.InventoryID != null)
					{

						// 'included' checkboxes
						InventoryItem inventoryItemRec = (InventoryItem)PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Current<InventoryAllocDetEnqFilter.inventoryID>>>>.Select(this);
						INItemClass itemClassRec = (INItemClass)PXSelect<INItemClass, Where<INItemClass.itemClassID, Equal<Required<INItemClass.itemClassID>>>>.Select(this, inventoryItemRec.ItemClassID);
						filter.InclQtyPOPrepared = itemClassRec.InclQtyPOPrepared;
						filter.InclQtyPOOrders = itemClassRec.InclQtyPOOrders;
						filter.InclQtyPOReceipts = itemClassRec.InclQtyPOReceipts;
						filter.InclQtyINReceipts = itemClassRec.InclQtyINReceipts;
						filter.InclQtyInTransit = itemClassRec.InclQtyInTransit;
						filter.InclQtySOPrepared = itemClassRec.InclQtySOPrepared;
						filter.InclQtySOBooked = itemClassRec.InclQtySOBooked;
						filter.InclQtySOShipping = itemClassRec.InclQtySOShipping;
						filter.InclQtySOShipped = itemClassRec.InclQtySOShipped;
						filter.InclQtyINIssues = itemClassRec.InclQtyINIssues;
						filter.InclQtyINAssemblyDemand = itemClassRec.InclQtyINAssemblyDemand;
						filter.InclQtyINAssemblySupply = itemClassRec.InclQtyINAssemblySupply;
						filter.InclQtySOBackOrdered = itemClassRec.InclQtySOBackOrdered;
						filter.InclQtySOReverse = itemClassRec.InclQtySOReverse;
						filter.BaseUnit = inventoryItemRec.BaseUnit;


						// QtyOnHand , QtyExpired , QtyLocNotAvail calculation :
						// simplified (without cost) version of code from IN401000
						PXSelectBase<INLocationStatus> calcStatusCmd = new PXSelectReadonly3<INLocationStatus,							
							/*
										InnerJoin<INSiteStatus,
												On<INSiteStatus.inventoryID,Equal<INLocationStatus.inventoryID>,
														And<INSiteStatus.subItemID, Equal<INLocationStatus.subItemID>,
																													And<INSiteStatus.siteID, Equal<INLocationStatus.siteID>>>>>,
							*/
										InnerJoin<InventoryItem,
												On<InventoryItem.inventoryID, Equal<INLocationStatus.inventoryID>>,

										InnerJoin<INLocation,
												On<INLocation.siteID, Equal<INLocationStatus.siteID>,
														And<INLocation.locationID, Equal<INLocationStatus.locationID>>>,

										InnerJoin<INSubItem,
												On<INSubItem.subItemID, Equal<INLocationStatus.subItemID>>,

		LeftJoin<INLotSerClass,
			On<INLotSerClass.lotSerClassID, Equal<InventoryItem.lotSerClassID>>,

										LeftJoin<INLotSerialStatus,
												On<INLotSerialStatus.inventoryID, Equal<INLocationStatus.inventoryID>,
				And<INLotSerClass.lotSerAssign, Equal<INLotSerAssign.whenReceived>,
														And<INLotSerialStatus.subItemID, Equal<INLocationStatus.subItemID>,
														And<INLotSerialStatus.siteID, Equal<INLocationStatus.siteID>,
														And<INLotSerialStatus.locationID, Equal<INLocationStatus.locationID>>>>>>,
										InnerJoin<INSite,
												On<INSite.siteID, Equal<INLocationStatus.siteID>,
												And<Match<IN.INSite, Current<AccessInfo.userName>>>>>>>>>>,


								OrderBy<Asc<InventoryItem.inventoryCD, Asc<INLocationStatus.siteID, Asc<INSubItem.subItemCD, Asc<INLocationStatus.locationID, Asc<INLotSerialStatus.lotSerialNbr>>>>>>>(this);


						calcStatusCmd.WhereAnd<Where<INLocationStatus.inventoryID, Equal<Current<InventoryAllocDetEnqFilter.inventoryID>>>>();
						// condition (filter.InventoryID != null) : checked above

						if (!SubCDUtils.IsSubCDEmpty(filter.SubItemCD))
						{
							calcStatusCmd.WhereAnd<Where<INSubItem.subItemCD, Like<Current<InventoryAllocDetEnqFilter.subItemCDWildcard>>>>();
						}

						if (filter.SiteID != null)
						{
							calcStatusCmd.WhereAnd<Where<INLocationStatus.siteID, Equal<Current<InventoryAllocDetEnqFilter.siteID>>>>();
						}

						if (filter.LocationID != null)
						{
							calcStatusCmd.WhereAnd<Where<INLocationStatus.locationID, Equal<Current<InventoryAllocDetEnqFilter.locationID>>>>();
						}

						if ((filter.LotSerialNbr ?? "") != "")
						{
							calcStatusCmd.WhereAnd<Where<INLotSerialStatus.lotSerialNbr, Like<Current<InventoryAllocDetEnqFilter.lotSerialNbrWildcard>>>>();
						}

						PXResultset<INLocationStatus> calcStatusRecs = (PXResultset<INLocationStatus>)calcStatusCmd.Select();

						// only 3 values here : QtyOnHand, QtyOnLocNotAvail, QtyExpired
						foreach (PXResult<INLocationStatus, /*INSiteStatus,*/ InventoryItem, INLocation, INSubItem, INLotSerClass, INLotSerialStatus> it in calcStatusRecs)
						{
							INLocationStatus ls_rec = (INLocationStatus)it;
							//INSiteStatus ss_rec = (INSiteStatus)it;
							InventoryItem ii_rec = (InventoryItem)it;
							//INSubItem si_rec = (INSubItem)it;  
							INLocation l_rec = (INLocation)it;
							INLotSerialStatus lss_rec = (INLotSerialStatus)it;

							filter.QtyOnHand += (lss_rec.QtyOnHand ?? ls_rec.QtyOnHand);
							filter.QtyHardAvail += (lss_rec.QtyHardAvail ?? ls_rec.QtyHardAvail);

							if (!(l_rec.InclQtyAvail ?? true))
							{
								filter.QtyNotAvail += lss_rec.QtyAvail ?? ls_rec.QtyAvail;
							}
							else
							{
								if ((lss_rec.ExpireDate != null) && (DateTime.Compare((DateTime)this.Accessinfo.BusinessDate, (DateTime)lss_rec.ExpireDate) > 0))
								{
									filter.QtyExpired += lss_rec.QtyOnHand;
								}
							}
						}

						foreach (InventoryAllocDetEnqResult it in this.ResultRecords.Select()) //???
						{
							Aggregate(filter, it);
						}

						filter.QtyTotalAddition =
							((filter.InclQtyPOPrepared ?? false) ? filter.QtyPOPrepared : 0m)
							+ ((filter.InclQtyPOOrders ?? false) ? filter.QtyPOOrders : 0m)
							+ ((filter.InclQtyPOReceipts ?? false) ? filter.QtyPOReceipts : 0m)
							+ ((filter.InclQtyINReceipts ?? false) ? filter.QtyINReceipts : 0m)
							+ ((filter.InclQtyInTransit ?? false) ? filter.QtyInTransit : 0m)
							+ ((filter.InclQtyINAssemblySupply ?? false) ? filter.QtyINAssemblySupply : 0m);
							
						filter.QtyTotalDeduction =
                            filter.QtyExpired
							+ ((filter.InclQtySOPrepared ?? false) ? filter.QtySOPrepared : 0m)
							+ ((filter.InclQtySOBooked ?? false) ? filter.QtySOBooked : 0m)
							+ ((filter.InclQtySOShipping ?? false) ? filter.QtySOShipping : 0m)
							+ ((filter.InclQtySOShipped ?? false) ? filter.QtySOShipped : 0m)
							+ ((filter.InclQtyINIssues ?? false) ? filter.QtyINIssues : 0m)
							+ ((filter.InclQtyINAssemblyDemand ?? false) ? filter.QtyINAssemblyDemand : 0m)
							+ ((filter.InclQtySOBackOrdered ?? false) ? filter.QtySOBackOrdered : 0m);

                        filter.QtyAvail = filter.QtyOnHand + filter.QtyTotalAddition - (filter.QtyTotalDeduction + filter.QtyNotAvail);
						filter.QtyHardAvail = filter.QtyOnHand - filter.QtyNotAvail - filter.QtySOShipping - filter.QtySOShipped - filter.QtyINIssues;
					}
				}
			}
			yield return cache.Current;
			cache.IsDirty = false;
		}

		private bool AdjustPrevResult<Field>(List<InventoryAllocDetEnqResult> resultList, INItemPlan planrec, Guid? PrevNoteID)
			where Field : IBqlField
		{
			InventoryAllocDetEnqResult parent;

			decimal? PlanQty = planrec.Reverse == true ? -planrec.PlanQty : planrec.PlanQty;

			bool found = false;

			while ((parent = resultList.Find(delegate(InventoryAllocDetEnqResult node)
			{
				return object.Equals(node.RefNoteID, PrevNoteID) && 
							string.Equals(node.AllocationType, typeof(Field).Name) &&
                            object.Equals(node.Reverse, planrec.Reverse) && 
							object.Equals(node.SubItemID, planrec.SubItemID) && 
							object.Equals(node.SiteID, planrec.SiteID) && 
							(object.Equals(node.LocationID, planrec.LocationID) || node.LocationID == null) && 
							(string.Equals(node.LotSerialNbr, planrec.LotSerialNbr) || string.IsNullOrEmpty(node.LotSerialNbr));
			})) != null)
			{
				found = true;

                if (parent.PlanQty >= 0 && parent.PlanQty > PlanQty || parent.PlanQty < 0 && parent.PlanQty < PlanQty)
				{
					parent.PlanQty -= PlanQty;
					PlanQty = 0m;
				}
				else
				{
					PlanQty -= parent.PlanQty;
					parent.PlanQty = 0m;

					resultList.Remove(parent);
				}

				if (PlanQty == 0m)
				{
					break;
				}
			}

			return found;
		}
			
		private void ProcessItemPlanRecAs<Field>(List<InventoryAllocDetEnqResult> resultList, PXResult<INItemPlan, INPlanType, Note, INLocation, INLotSerialStatus,BAccountR, SOShipLineSplit, POReceiptLineSplit> widerec)
			where Field : IBqlField 
		{
			INPlanType pt_rec = (INPlanType)widerec;
			PXCache cache = Caches[typeof(INPlanType)];

			short? qtyMult = (short?)cache.GetValue<Field>(pt_rec);

			if (qtyMult == 0) { return; }

            if (typeof(Field) == typeof(INPlanType.inclQtyINReceipts))
            {
                // similar to SO Order-Shipping processing
                INItemPlan planrec = widerec;

                if (planrec.OrigPlanType == INPlanConstants.Plan42) AdjustPrevResult<INPlanType.inclQtyInTransit>(resultList, planrec, planrec.OrigNoteID);
                if (planrec.OrigPlanType == INPlanConstants.Plan44) AdjustPrevResult<INPlanType.inclQtyInTransitToSO>(resultList, planrec, planrec.OrigNoteID);
            }
			else if (typeof(Field) == typeof(INPlanType.inclQtyPOReceipts))
			{
				// similar to SO Order-Shipping processing
				INItemPlan planrec = widerec;

				foreach (POOrder order in PXSelectJoin<POOrder,
					InnerJoin<POOrderReceipt, On<POOrder.orderType, Equal<POOrderReceipt.pOType>, And<POOrder.orderNbr, Equal<POOrderReceipt.pONbr>>>,
					InnerJoin<POReceipt, On<POOrderReceipt.receiptNbr, Equal<POReceipt.receiptNbr>>>>,
					Where<POReceipt.noteID, Equal<Required<POReceipt.noteID>>>>.Select(this, planrec.RefNoteID))
				{
                    if (planrec.OrigPlanType == INPlanConstants.Plan70 && AdjustPrevResult<INPlanType.inclQtyPOOrders>(resultList, planrec, order.NoteID)) break;
                }

                if (planrec.OrigPlanType == INPlanConstants.Plan42) AdjustPrevResult<INPlanType.inclQtyInTransit>(resultList, planrec, planrec.OrigNoteID);
                if (planrec.OrigPlanType == INPlanConstants.Plan44) AdjustPrevResult<INPlanType.inclQtyInTransitToSO>(resultList, planrec, planrec.OrigNoteID);
			}
			else if (typeof(Field) == typeof(INPlanType.inclQtyPOFixedReceipts))
			{
				// similar to SO Order-Shipping processing
				INItemPlan planrec = widerec;

				foreach (POOrder order in PXSelectJoin<POOrder,
					InnerJoin<POOrderReceipt, On<POOrder.orderType, Equal<POOrderReceipt.pOType>, And<POOrder.orderNbr, Equal<POOrderReceipt.pONbr>>>,
					InnerJoin<POReceipt, On<POOrderReceipt.receiptNbr, Equal<POReceipt.receiptNbr>>>>,
					Where<POReceipt.noteID, Equal<Required<POReceipt.noteID>>>>.Select(this, planrec.RefNoteID))
				{
                    if (planrec.OrigPlanType == INPlanConstants.Plan76 && AdjustPrevResult<INPlanType.inclQtyPOFixedOrders>(resultList, planrec, order.NoteID)) break;
			    }

                if (planrec.OrigPlanType == INPlanConstants.Plan42) AdjustPrevResult<INPlanType.inclQtyInTransit>(resultList, planrec, planrec.OrigNoteID);
                if (planrec.OrigPlanType == INPlanConstants.Plan44) AdjustPrevResult<INPlanType.inclQtyInTransitToSO>(resultList, planrec, planrec.OrigNoteID);
			}
			else if (typeof(Field) == typeof(INPlanType.inclQtyPODropShipReceipts))
			{
				// similar to SO Order-Shipping processing
				INItemPlan planrec = widerec;

				foreach (POOrder order in PXSelectJoin<POOrder,
					InnerJoin<POOrderReceipt, On<POOrder.orderType, Equal<POOrderReceipt.pOType>, And<POOrder.orderNbr, Equal<POOrderReceipt.pONbr>>>,
					InnerJoin<POReceipt, On<POOrderReceipt.receiptNbr, Equal<POReceipt.receiptNbr>>>>,
					Where<POReceipt.noteID, Equal<Required<POReceipt.noteID>>>>.Select(this, planrec.RefNoteID))
				{
                    if (planrec.OrigPlanType == INPlanConstants.Plan74 && AdjustPrevResult<INPlanType.inclQtyPODropShipOrders>(resultList, planrec, order.NoteID)) break;
				}
			}
			else if (typeof(Field) == typeof(INPlanType.inclQtySOShipping))
			{
				INItemPlan planrec = widerec;

				foreach (SOOrder order in PXSelectJoin<SOOrder,
					InnerJoin<SOOrderShipment, On<SOOrder.orderType, Equal<SOOrderShipment.orderType>, And<SOOrder.orderNbr, Equal<SOOrderShipment.orderNbr>>>,
					InnerJoin<SOShipment, On<SOOrderShipment.shipmentNbr, Equal<SOShipment.shipmentNbr>, And<SOOrderShipment.shipmentType, Equal<SOShipment.shipmentType>>>>>,
					Where<SOShipment.noteID, Equal<Required<SOShipment.noteID>>>>.Select(this, planrec.RefNoteID))
				{
					if (planrec.OrigPlanType == INPlanConstants.Plan60 && AdjustPrevResult<INPlanType.inclQtySOBooked>(resultList, planrec, order.NoteID)) break;
					if ((planrec.OrigPlanType == INPlanConstants.Plan61 || planrec.OrigPlanType == INPlanConstants.Plan63) && AdjustPrevResult<Field>(resultList, planrec, order.NoteID)) break;
				}
			}

			if (qtyMult == 0) { return; }

			INItemPlan ip_rec = (INItemPlan)widerec;
			Note n_rec = (Note)widerec;
			INLocation l_rec = (INLocation)widerec;
			INLotSerialStatus lss_rec = (INLotSerialStatus)widerec;
			BAccountR bAcc = widerec;

			InventoryAllocDetEnqResult item = new InventoryAllocDetEnqResult();

			item.AllocationType = typeof(Field).Name;

			item.Expired = (lss_rec.ExpireDate != null && (DateTime.Compare((DateTime)this.Accessinfo.BusinessDate, (DateTime)lss_rec.ExpireDate) > 0));
			item.LocationID = ip_rec.LocationID;
			item.LocNotAvailable = !(l_rec.InclQtyAvail ?? true);
			item.LotSerialNbr = ip_rec.LotSerialNbr;
			item.Module = n_rec.GraphType.Substring(11, 2);
			item.QADocType = n_rec.GraphType.Substring(14);
			item.PlanQty = ((decimal)qtyMult) * (ip_rec.PlanQty) ?? 0m;
			if (ip_rec.Reverse == true)
				item.PlanQty = -item.PlanQty;
			item.Reverse = ip_rec.Reverse;
			item.RefNoteID = ip_rec.RefNoteID;
			item.SiteID = ip_rec.SiteID;
			item.SubItemID = ip_rec.SubItemID;
			item.PlanDate = ip_rec.PlanDate;
			item.BAccountID = bAcc.BAccountID;
			item.AcctName = bAcc.AcctName;
			item.PlanID = ip_rec.PlanID;

			resultList.Add(item);
		}

		protected virtual IEnumerable resultRecords()
		{

			InventoryAllocDetEnqFilter filter = Filter.Current;

			List<InventoryAllocDetEnqResult> resultList = new List<InventoryAllocDetEnqResult>();

			if (filter.InventoryID == null)
			{
				return resultList;  //empty
			}

			PXSelectBase<INItemPlan> cmd = new PXSelectJoin<INItemPlan,

				InnerJoin<INPlanType,
					On<INPlanType.planType, Equal<INItemPlan.planType>>,

				InnerJoin<Note,
					On<Note.noteID, Equal<INItemPlan.refNoteID>>,

				LeftJoin<INLocation,
					On<INLocation.siteID, Equal<INItemPlan.siteID>,
						And<INLocation.locationID, Equal<INItemPlan.locationID>>>,

				LeftJoin<INLotSerialStatus,
										On<INLotSerialStatus.inventoryID, Equal<INItemPlan.inventoryID>,
												And<INLotSerialStatus.subItemID, Equal<INItemPlan.subItemID>,
														And<INLotSerialStatus.siteID, Equal<INItemPlan.siteID>,
																And<INLotSerialStatus.locationID, Equal<INItemPlan.locationID>,
									And<INLotSerialStatus.lotSerialNbr, Equal<INItemPlan.lotSerialNbr>>>>>>,
				LeftJoin<BAccountR,
				 On<BAccountR.bAccountID, Equal<INItemPlan.bAccountID>>,

				LeftJoin<SOShipLineSplit, On<SOShipLineSplit.planID, Equal<INItemPlan.planID>>, 
				LeftJoin<POReceiptLineSplit, On<POReceiptLineSplit.planID, Equal<INItemPlan.planID>>, 

				LeftJoin<INSubItem,
						On<INSubItem.subItemID, Equal<INItemPlan.subItemID>>,

				InnerJoin<INSite,
					On<INSite.siteID, Equal<INItemPlan.siteID>>>>>>>>>>>,
				

			Where<INItemPlan.planQty, NotEqual<decimal0>, And<Match<INSite, Current<AccessInfo.userName>>>>,
			OrderBy<Asc<INSubItem.subItemCD, Asc<INSite.siteCD, Asc<INItemPlan.origPlanType, Asc<INItemPlan.planType, Asc<INLocation.locationCD>>>>>>>(this);
			//sorting must be done with PlanType preceeding location

			//InventoryID is required
			if (filter.InventoryID == null)
			{
				return resultList;  //empty
			}

			cmd.WhereAnd<Where<INItemPlan.inventoryID, Equal<Current<InventoryAllocDetEnqFilter.inventoryID>>>>();

			if (!SubCDUtils.IsSubCDEmpty(filter.SubItemCD) && PXAccess.FeatureInstalled<FeaturesSet.subItem>())
			{
				cmd.WhereAnd<Where<INSubItem.subItemCD, Like<Current<InventoryAllocDetEnqFilter.subItemCDWildcard>>>>();
			}

			if (filter.SiteID != null)
			{
				cmd.WhereAnd<Where<INItemPlan.siteID, Equal<Current<InventoryAllocDetEnqFilter.siteID>>>>();
			}

			//if (filter.LocationID != null)
			if ((filter.LocationID ?? -1) != -1) // there are cases when filter.LocationID = -1
			{
				cmd.WhereAnd<Where<INItemPlan.locationID, Equal<Current<InventoryAllocDetEnqFilter.locationID>>>>();
			}

			if ((filter.LotSerialNbr ?? "") != "")
			{
				cmd.WhereAnd<Where<INItemPlan.lotSerialNbr, Like<Current<InventoryAllocDetEnqFilter.lotSerialNbrWildcard>>>>();
			}

			PXResultset<INItemPlan> selectResult = (PXResultset<INItemPlan>)cmd.Select();

			foreach (PXResult<INItemPlan, INPlanType, Note, INLocation, INLotSerialStatus, BAccountR, SOShipLineSplit, POReceiptLineSplit> it in selectResult)
			{
				ProcessItemPlanRecAs<INPlanType.inclQtyINReceipts>(resultList, it);
				ProcessItemPlanRecAs<INPlanType.inclQtyINIssues>(resultList, it);
				ProcessItemPlanRecAs<INPlanType.inclQtyInTransit>(resultList, it);
                ProcessItemPlanRecAs<INPlanType.inclQtyInTransitToSO>(resultList, it);
                ProcessItemPlanRecAs<INPlanType.inclQtyPOOrders>(resultList, it);
				ProcessItemPlanRecAs<INPlanType.inclQtyPOPrepared>(resultList, it);
				ProcessItemPlanRecAs<INPlanType.inclQtyPOReceipts>(resultList, it);
				ProcessItemPlanRecAs<INPlanType.inclQtySOPrepared>(resultList, it);
				ProcessItemPlanRecAs<INPlanType.inclQtySOBooked>(resultList, it);
				ProcessItemPlanRecAs<INPlanType.inclQtySOShipping>(resultList, it);
				ProcessItemPlanRecAs<INPlanType.inclQtySOShipped>(resultList, it);
				ProcessItemPlanRecAs<INPlanType.inclQtySOBackOrdered>(resultList, it);
				ProcessItemPlanRecAs<INPlanType.inclQtyINAssemblyDemand>(resultList, it);
				ProcessItemPlanRecAs<INPlanType.inclQtyINAssemblySupply>(resultList, it);
				ProcessItemPlanRecAs<INPlanType.inclQtySOFixed>(resultList, it);
				ProcessItemPlanRecAs<INPlanType.inclQtyPOFixedOrders>(resultList, it);
				ProcessItemPlanRecAs<INPlanType.inclQtyPOFixedPrepared>(resultList, it);
				ProcessItemPlanRecAs<INPlanType.inclQtyPOFixedReceipts>(resultList, it);
				ProcessItemPlanRecAs<INPlanType.inclQtySODropShip>(resultList, it);
				ProcessItemPlanRecAs<INPlanType.inclQtyPODropShipOrders>(resultList, it);
				ProcessItemPlanRecAs<INPlanType.inclQtyPODropShipPrepared>(resultList, it);
				ProcessItemPlanRecAs<INPlanType.inclQtyPODropShipReceipts>(resultList, it);
			}

			// numerate grid lines (key column) to let ViewDocument button work
			int nextLineNbr = 1;
			DateTime minPlanDate = new DateTime(1900, 1, 1);
			foreach (InventoryAllocDetEnqResult it in resultList)
			{
				if (it.PlanDate == minPlanDate) it.PlanDate = null;
				it.GridLineNbr = nextLineNbr++;
			}

			return resultList;


		}

		public override bool IsDirty
		{
			get { return false; }
		}


		protected virtual void Aggregate(InventoryAllocDetEnqFilter aDest, InventoryAllocDetEnqResult aSrc)
		{
			
			if (aDest.InclQtySOReverse != true && aSrc.Reverse == true) return;

			switch (aSrc.AllocationType)
			{
				case "inclQtyINReceipts":
					aDest.QtyINReceipts += (aSrc.LocNotAvailable == false) ? aSrc.PlanQty : 0m;
					break;
				case "inclQtyInTransit":
					aDest.QtyInTransit += (aSrc.LocNotAvailable == false && aSrc.Expired == false) ? aSrc.PlanQty : 0m;
					break;
				case "inclQtySOPrepared":
					aDest.QtySOPrepared += (aSrc.LocNotAvailable == false && aSrc.Expired == false) ? aSrc.PlanQty : 0m;
					break;
				case "inclQtySOBooked":
					aDest.QtySOBooked += (aSrc.LocNotAvailable == false && aSrc.Expired == false) ? aSrc.PlanQty : 0m;
					break;
				case "inclQtySOShipping":
					aDest.QtySOShipping += (aSrc.LocNotAvailable == false && aSrc.Expired == false) ? aSrc.PlanQty : 0m;
					break;
				case "inclQtySOShipped":
					aDest.QtySOShipped += (aSrc.LocNotAvailable == false && aSrc.Expired == false) ? aSrc.PlanQty : 0m;
					break;
				case "inclQtyINIssues":
					aDest.QtyINIssues += (aSrc.LocNotAvailable == false && aSrc.Expired == false) ? aSrc.PlanQty : 0m;
					break;
				case "inclQtySOBackOrdered":
					aDest.QtySOBackOrdered += (aSrc.LocNotAvailable == false && aSrc.Expired == false) ? aSrc.PlanQty : 0m;
					break;									
				default:
					INPlanType plantype = new INPlanType();
					PXCache cache = Caches[typeof(INPlanType)];
					cache.SetValue(plantype, aSrc.AllocationType, (short)1);

					INItemPlanIDAttribute.UpdateAllocatedQuantitiesBase<InventoryAllocDetEnqFilter>(this, aDest, aSrc, plantype, null);
					break;
			}
		}

		//public PXAction<InventoryAllocDetEnqFilter> viewItem;
		//[PXLookupButton()]
		//[PXUIField(DisplayName = Messages.InventoryItem)]
		//protected virtual IEnumerable ViewItem(PXAdapter a)
		//{
		//	InventoryItemMaint.Redirect(this.Filter.Current.InventoryID, true);
		//	return a.Get();
		//}

		public PXAction<InventoryAllocDetEnqFilter> viewSummary;
		[PXButton()]
		[PXUIField(DisplayName = Messages.InventorySummary)]
		protected virtual IEnumerable ViewSummary(PXAdapter a)
		{
			if (this.ResultRecords.Current != null)
			{
				object subItem =
					this.ResultRecords.Cache.GetValueExt<InventorySummaryEnquiryResult.subItemID>(this.ResultRecords.Current);

				if (subItem is PXSegmentedState)
					subItem = ((PXSegmentedState)subItem).Value;

				InventorySummaryEnq.Redirect(this.Filter.Current.InventoryID,
					subItem != null ? (string)subItem : null,
					this.ResultRecords.Current.SiteID,
					this.ResultRecords.Current.LocationID, false);
			}
			return a.Get();
		}

		[PXUIField(DisplayName = "")]
		[PXEditDetailButton]
		protected virtual IEnumerable ViewDocument(PXAdapter adapter)
		{
			if (this.ResultRecords.Current != null)
			{
				InventoryAllocDetEnqResult res = this.ResultRecords.Current;

				EntityHelper entity = new InventoryAllocDetEnqResult.EntityHelper(this);
				INReplenishmentOrder replenishmentOrder = entity.GetEntityRow(res.RefNoteID) as INReplenishmentOrder;
				if (replenishmentOrder != null && res.PlanID != null)
				{
					SOOrder order = (SOOrder)PXSelectJoin<SOOrder, InnerJoin<INReplenishmentLine, On<SOOrder.orderType, Equal<INReplenishmentLine.sOType>, And<SOOrder.orderNbr, Equal<INReplenishmentLine.sONbr>>>>, Where<INReplenishmentLine.planID, Equal<Required<InventoryAllocDetEnqResult.planID>>>>.Select(this, res.PlanID);
					PXRedirectHelper.TryRedirect(this, order, PXRedirectHelper.WindowMode.NewWindow);
				}
				else
					entity.NavigateToRow((Guid)res.RefNoteID, PXRedirectHelper.WindowMode.NewWindow);
				throw new PXException(Messages.UnableNavigateDocument);
			}
			return adapter.Get();
		}

		public static void Redirect(int? inventoryID, string subItemCD, string lotSerNum, int? siteID, int? locationID)
		{
			InventoryAllocDetEnq graph = PXGraph.CreateInstance<InventoryAllocDetEnq>();
			graph.Filter.Current.InventoryID = inventoryID;
			graph.Filter.Current.SubItemCD = subItemCD;
			graph.Filter.Current.SiteID = siteID;
			graph.Filter.Current.LocationID = locationID;
			graph.Filter.Current.LotSerialNbr = lotSerNum;
			graph.Filter.Select();

			throw new PXRedirectRequiredException(graph, Messages.InventoryAllocDet);
		}
	}

}

