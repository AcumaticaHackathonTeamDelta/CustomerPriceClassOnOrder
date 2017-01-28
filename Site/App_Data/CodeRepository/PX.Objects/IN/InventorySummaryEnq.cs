using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using PX.SM;
using PX.Data;
using PX.Objects.BQLConstants;
using PX.Objects.CS;
using PX.Objects.CM;
using PX.Objects.GL;
using SiteStatus = PX.Objects.IN.Overrides.INDocumentRelease.SiteStatus;
using LocationStatus = PX.Objects.IN.Overrides.INDocumentRelease.LocationStatus;


namespace PX.Objects.IN
{

	#region Additional DAC

	

	#endregion Additional DACs

	#region Filter

	[Serializable]
	public partial class InventorySummaryEnqFilter : PX.Data.IBqlTable
	{

		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[Inventory(typeof(Search<InventoryItem.inventoryID, Where<InventoryItem.stkItem, NotEqual<boolFalse>, And<Where<Match<Current<AccessInfo.userName>>>>>>),
			typeof(InventoryItem.inventoryCD), typeof(InventoryItem.descr),
			Required = true)]
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
		[SubItemRawExt(typeof(InventorySummaryEnqFilter.inventoryID), DisplayName = "Subitem")]
		//[SubItemRaw(DisplayName = "Subitem")]
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

		#region SubItemCD Wildcard
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
		[Location(typeof(InventorySummaryEnqFilter.siteID), Visibility = PXUIVisibility.Visible, KeepEntry = false, DescriptionField = typeof(INLocation.descr), DisplayName = "Location")]
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

		#region ExpandByLotSerialNbr
		public abstract class expandByLotSerialNbr : PX.Data.IBqlField
		{
		}
		protected bool? _ExpandByLotSerialNbr;
		[PXDBBool()]
		[PXDefault()]
		[PXUIField(DisplayName = "Expand By Lot/Serial Number", Visibility = PXUIVisibility.Visible)]
		public virtual bool? ExpandByLotSerialNbr
		{
			get
			{
				return this._ExpandByLotSerialNbr;
			}
			set
			{
				this._ExpandByLotSerialNbr = value;
			}
		}
		#endregion

	}
	#endregion

	#region ResultSet
    [Serializable]
	public partial class InventorySummaryEnquiryResult : PX.Data.IBqlTable, IStatus, ICostStatus 
	{
		public InventorySummaryEnquiryResult() { }

		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField { }
		protected Int32? _InventoryID;
		[Inventory(Visibility = PXUIVisibility.SelectorVisible, DisplayName = "Inventory ID", IsKey = true)]
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
		[SubItem(Visibility = PXUIVisibility.SelectorVisible, DisplayName = "Subitem", IsKey = true)]
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
		#region SiteId
		public abstract class siteID : PX.Data.IBqlField { }
		protected Int32? _SiteID;
		[Site(Visibility = PXUIVisibility.SelectorVisible, DisplayName = "Warehouse", IsKey = true)]
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
		public abstract class locationID : PX.Data.IBqlField { }
		protected Int32? _LocationID;
		[Location(typeof(siteID), Visibility = PXUIVisibility.SelectorVisible, DisplayName = "Location", IsKey = true)]
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
		#region QtyAvail
		public abstract class qtyAvail : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyAvail;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Available", Visibility = PXUIVisibility.SelectorVisible)]
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
		[PXUIField(DisplayName = "Available for Shipment", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region QtyNotAvail
		public abstract class qtyNotAvail : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyNotAvail;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Not Available", Visibility = PXUIVisibility.SelectorVisible)]
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

		#region QtySOPrepared
		public abstract class qtySOPrepared : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtySOPrepared;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "SO Prepared", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region QtySOBooked
		public abstract class qtySOBooked : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtySOBooked;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "SO Booked", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region QtySOShipped
		public abstract class qtySOShipped : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtySOShipped;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "SO Shipped", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region QtySOShipping
		public abstract class qtySOShipping : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtySOShipping;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "SO Allocated", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region QtySOBackOrdered
		public abstract class qtySOBackOrdered : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtySOBackOrdered;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "SO Back Ordered", Visibility = PXUIVisibility.Visible, Enabled = false)]
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
		#region QtyINAssemblyDemand
		public abstract class qtyINAssemblyDemand : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyINAssemblyDemand;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "In Assembly Demand", Visibility = PXUIVisibility.Visible, Enabled = false)]
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

		#region QtyINIssues
		public abstract class qtyINIssues : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyINIssues;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "IN Issues", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region QtyINReceipts
		public abstract class qtyINReceipts : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyINReceipts;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "IN Receipts", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region QtyInTransit
		public abstract class qtyInTransit : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyInTransit;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "In Transit", Visibility = PXUIVisibility.SelectorVisible)]
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
        #region QtyInTransitToSO
        public abstract class qtyInTransitToSO : PX.Data.IBqlField
        {
        }
        protected Decimal? _QtyInTransitToSO;
        [PXDBQuantity()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "In Transit to SO", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region QtyPOPrepared
		public abstract class qtyPOPrepared : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyPOPrepared;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Purchase Prepared", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region QtyPOOrders
		public abstract class qtyPOOrders : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyPOOrders;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Purchase Orders", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region QtyPOReceipts
		public abstract class qtyPOReceipts : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyPOReceipts;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "PO Receipts", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region QtySOFixed
		public abstract class qtySOFixed : IBqlField { }
		protected decimal? _QtySOFixed;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "SO to Purchase")]
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
		[PXUIField(DisplayName = "Purchase for SO")]
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
		[PXUIField(DisplayName = "Purchase for SO Prepared")]
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
        [PXUIField(DisplayName = "Receipts for SO")]
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
		[PXUIField(DisplayName = "SO to Drop-Ship")]
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
		[PXUIField(DisplayName = "Drop-Ship for SO")]
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
		[PXUIField(DisplayName = "Drop-Ship for SO Prepared")]
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
		[PXUIField(DisplayName = "Drop-Ship for SO Receipts")]
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
		#region QtyINAssemblySupply
		public abstract class qtyINAssemblySupply : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyINAssemblySupply;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "In Assembly Supply", Visibility = PXUIVisibility.Visible, Enabled = false)]
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


		#region QtyOnHand
		public abstract class qtyOnHand : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyOnHand;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "On Hand", Visibility = PXUIVisibility.SelectorVisible)]
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

		#region QtyExpired
		public abstract class qtyExpired : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyExpired;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Expired", Visibility = PXUIVisibility.SelectorVisible)]
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

		#region BaseUnit
		public abstract class baseUnit : PX.Data.IBqlField
		{
		}
		protected String _BaseUnit;
		[INUnit(DisplayName = "Base Unit", Visibility = PXUIVisibility.Visible)]
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

		#region Qty		
		protected Decimal? _Qty;
		[PXQuantity()]		
		public virtual Decimal? Qty
		{
			[PXDependsOnFields(typeof(qtyOnHand),typeof(qtyInTransit))]
			get
			{
				return this.QtyOnHand + this.QtyInTransit;
			}			
		}
		#endregion

		#region UnitCost
		public abstract class unitCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnitCost;
		[PXDBPriceCost()]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Estimated Unit Cost")]
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

		#region TotalCost
		public abstract class totalCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _TotalCost;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Estimated Total Cost")]
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

		#region LotSerialNbr
		public abstract class lotSerialNbr : PX.Data.IBqlField
		{
		}
		protected String _LotSerialNbr;
		[PXDBString(100, IsUnicode = true, IsKey = true, InputMask = "")]
		[PXUIField(DisplayName = "Lot/Serial Number", Visible = false)]
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
		[PXDBDate()]
		[PXUIField(DisplayName = "Expiration Date", Visible = false)]
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
	}

	#endregion


	[PX.Objects.GL.TableAndChartDashboardType]    
    [Serializable]
	public class InventorySummaryEnq : PXGraph<InventorySummaryEnq>
	{				
		private decimal Round(decimal? a)
		{
			return Math.Round(a ?? 0m, (int)commonsetup.Current.DecPlQty, MidpointRounding.AwayFromZero);
		}

		private decimal RoundUnitCost(decimal? a)
		{
			return Math.Round(a ?? 0m, (int)commonsetup.Current.DecPlPrcCst, MidpointRounding.AwayFromZero);
		}

		private bool IsZero(IStatus a)
		{
			return (a.QtyAvail == 0m)
					&& (a.QtyHardAvail == 0m)
					&& (a.QtyOnHand == 0m)
					&& (a.QtySOPrepared == 0m)
					&& (a.QtySOBooked == 0m)
					&& (a.QtySOShipping == 0m)
					&& (a.QtySOShipped == 0m)
					&& (a.QtySOBackOrdered == 0m)
					&& (a.QtyINIssues == 0m)
					&& (a.QtyINReceipts == 0m)
					&& (a.QtyInTransit == 0m)
                    && (a.QtyInTransitToSO == 0m)
					&& (a.QtyPOPrepared == 0m)
					&& (a.QtyPOOrders == 0m)
					&& (a.QtyPOReceipts == 0m)
					&& (a.QtyINAssemblyDemand == 0m)
					&& (a.QtyINAssemblySupply == 0m)
					&& (a.QtySOFixed == 0m)
					&& (a.QtyPOFixedOrders == 0m)
					&& (a.QtyPOFixedPrepared == 0m)
					&& (a.QtyPOFixedReceipts == 0m)
					&& (a.QtySODropShip == 0m)
					&& (a.QtyPODropShipOrders == 0m)
					&& (a.QtyPODropShipPrepared == 0m)
					&& (a.QtyPODropShipReceipts == 0m);
		}

		private void Add(IStatus dest, IStatus source)
		{
			dest.QtyAvail += source.QtyAvail ?? 0m;
			dest.QtyHardAvail += source.QtyHardAvail ?? 0m;
			dest.QtyNotAvail += source.QtyNotAvail ?? 0m;
			dest.QtyExpired += source.QtyExpired ?? 0m;
			dest.QtyOnHand += source.QtyOnHand ?? 0m;
			dest.QtySOPrepared += source.QtySOPrepared ?? 0m;
			dest.QtySOBooked += source.QtySOBooked ?? 0m;
			dest.QtySOShipping += source.QtySOShipping ?? 0m;
			dest.QtySOShipped += source.QtySOShipped ?? 0m;
			dest.QtySOBackOrdered += source.QtySOBackOrdered ?? 0m;
			dest.QtyINAssemblyDemand += source.QtyINAssemblyDemand ?? 0m;
			dest.QtyINAssemblySupply += source.QtyINAssemblySupply ?? 0m;
			dest.QtyINIssues += source.QtyINIssues ?? 0m;
			dest.QtyINReceipts += source.QtyINReceipts ?? 0m;
			dest.QtyInTransit += source.QtyInTransit ?? 0m;
            dest.QtyInTransitToSO += source.QtyInTransitToSO ?? 0m;
			dest.QtyPOPrepared += source.QtyPOPrepared ?? 0m;
			dest.QtyPOOrders += source.QtyPOOrders ?? 0m;
			dest.QtyPOReceipts += source.QtyPOReceipts ?? 0m;
			dest.QtySOFixed += source.QtySOFixed ?? 0m;
			dest.QtyPOFixedOrders += source.QtyPOFixedOrders ?? 0m;
			dest.QtyPOFixedPrepared += source.QtyPOFixedPrepared ?? 0m;
			dest.QtyPOFixedReceipts += source.QtyPOFixedReceipts ?? 0m;
			dest.QtySODropShip += source.QtySODropShip ?? 0m;
			dest.QtyPODropShipOrders += source.QtyPODropShipOrders ?? 0m;
			dest.QtyPODropShipPrepared += source.QtyPODropShipPrepared ?? 0m;
			dest.QtyPODropShipReceipts += source.QtyPODropShipReceipts ?? 0m;

			ICostStatus costdest = dest as ICostStatus;
			ICostStatus costsource = source as ICostStatus;
			if (costdest != null && costsource != null)
			{
				costdest.TotalCost += costsource.TotalCost;
			}
		}

		private void Subtract(IStatus dest, IStatus source)
		{
			dest.QtyAvail -= source.QtyAvail ?? 0m;
			dest.QtyHardAvail -= source.QtyHardAvail ?? 0m;
			dest.QtyNotAvail -= source.QtyNotAvail ?? 0m;
			dest.QtyExpired -= source.QtyExpired ?? 0m;
			dest.QtyOnHand -= source.QtyOnHand ?? 0m;
			dest.QtySOPrepared -= source.QtySOPrepared ?? 0m;
			dest.QtySOBooked -= source.QtySOBooked ?? 0m;
			dest.QtySOShipping -= source.QtySOShipping ?? 0m;
			dest.QtySOShipped -= source.QtySOShipped ?? 0m;
			dest.QtySOBackOrdered -= source.QtySOBackOrdered ?? 0m;
			dest.QtyINAssemblyDemand -= source.QtyINAssemblyDemand ?? 0m;
			dest.QtyINAssemblySupply -= source.QtyINAssemblySupply ?? 0m;
			dest.QtyINIssues -= source.QtyINIssues ?? 0m;
			dest.QtyINReceipts -= source.QtyINReceipts ?? 0m;
			dest.QtyInTransit -= source.QtyInTransit ?? 0m;
            dest.QtyInTransitToSO -= source.QtyInTransitToSO ?? 0m;
			dest.QtyPOPrepared -= source.QtyPOPrepared ?? 0m;
			dest.QtyPOOrders -= source.QtyPOOrders ?? 0m;
			dest.QtyPOReceipts -= source.QtyPOReceipts ?? 0m;
			dest.QtySOFixed -= source.QtySOFixed ?? 0m;
			dest.QtyPOFixedOrders -= source.QtyPOFixedOrders ?? 0m;
			dest.QtyPOFixedPrepared -= source.QtyPOFixedPrepared ?? 0m;
			dest.QtyPOFixedReceipts -= source.QtyPOFixedReceipts ?? 0m;
			dest.QtySODropShip -= source.QtySODropShip ?? 0m;
			dest.QtyPODropShipOrders -= source.QtyPODropShipOrders ?? 0m;
			dest.QtyPODropShipPrepared -= source.QtyPODropShipPrepared ?? 0m;
			dest.QtyPODropShipReceipts -= source.QtyPODropShipReceipts ?? 0m;

			ICostStatus costdest = dest as ICostStatus;
			ICostStatus costsource = source as ICostStatus;
			if (costdest != null && costsource != null)
			{
				costdest.TotalCost -= costsource.TotalCost;
			}
		}

		private void Copy(IStatus dest, IStatus source)
		{
			dest.QtyAvail = source.QtyAvail ?? 0m;
			dest.QtyHardAvail = source.QtyHardAvail ?? 0m;
			dest.QtyNotAvail = source.QtyNotAvail ?? 0m;
			dest.QtyOnHand = source.QtyOnHand ?? 0m;
			dest.QtySOPrepared = source.QtySOPrepared ?? 0m;
			dest.QtySOBooked = source.QtySOBooked ?? 0m;
			dest.QtySOShipping = source.QtySOShipping ?? 0m;
			dest.QtySOShipped = source.QtySOShipped ?? 0m;
			dest.QtySOBackOrdered = source.QtySOBackOrdered ?? 0m;
			dest.QtyINAssemblyDemand = source.QtyINAssemblyDemand ?? 0m;
			dest.QtyINAssemblySupply = source.QtyINAssemblySupply ?? 0m;
			dest.QtyINIssues = source.QtyINIssues ?? 0m;
			dest.QtyINReceipts = source.QtyINReceipts ?? 0m;
			dest.QtyInTransit = source.QtyInTransit ?? 0m;
            dest.QtyInTransitToSO = source.QtyInTransitToSO ?? 0m;
			dest.QtyPOPrepared = source.QtyPOPrepared ?? 0m;
			dest.QtyPOOrders = source.QtyPOOrders ?? 0m;
			dest.QtyPOReceipts = source.QtyPOReceipts ?? 0m;
			dest.QtySOFixed = source.QtySOFixed ?? 0m;
			dest.QtyPOFixedOrders = source.QtyPOFixedOrders ?? 0m;
			dest.QtyPOFixedPrepared = source.QtyPOFixedPrepared ?? 0m;
			dest.QtyPOFixedReceipts = source.QtyPOFixedReceipts ?? 0m;
			dest.QtySODropShip = source.QtySODropShip ?? 0m;
			dest.QtyPODropShipOrders = source.QtyPODropShipOrders ?? 0m;
			dest.QtyPODropShipPrepared = source.QtyPODropShipPrepared ?? 0m;
			dest.QtyPODropShipReceipts = source.QtyPODropShipReceipts ?? 0m;
		}

		private T Copy<T>(IStatus source)
			where T : class, IStatus, new()
		{
			T dest = new T();
			Copy(dest, source);
			return dest;
		}

		private T Subtract<T>(IStatus a, IStatus b)
			where T : class, IBqlTable, IStatus, new()
		{
			T c = new T();
			if (typeof(T).IsAssignableFrom(a.GetType()))
			{
				PXCache<T>.RestoreCopy(c, (T)a);
				Subtract(c, b);
				return c;
			}

			c.QtyAvail = Round((a.QtyAvail ?? 0m) - (b.QtyAvail ?? 0m));
			c.QtyHardAvail = Round((a.QtyHardAvail ?? 0m) - (b.QtyHardAvail ?? 0m));
			c.QtyNotAvail = Round((a.QtyNotAvail ?? 0m) - (b.QtyNotAvail ?? 0m));
			c.QtyExpired = Round((a.QtyExpired ?? 0m) - (b.QtyExpired ?? 0m));
			c.QtyOnHand = Round((a.QtyOnHand ?? 0m) - (b.QtyOnHand ?? 0m));
			c.QtySOPrepared = Round((a.QtySOPrepared ?? 0m) - (b.QtySOPrepared ?? 0m));
			c.QtySOBooked = Round((a.QtySOBooked ?? 0m) - (b.QtySOBooked ?? 0m));
			c.QtySOShipping = Round((a.QtySOShipping ?? 0m) - (b.QtySOShipping ?? 0m));
			c.QtySOShipped = Round((a.QtySOShipped ?? 0m) - (b.QtySOShipped ?? 0m));
			c.QtySOBackOrdered = Round((a.QtySOBackOrdered ?? 0m) - (b.QtySOBackOrdered ?? 0m));
			c.QtyINIssues = Round((a.QtyINIssues ?? 0m) - (b.QtyINIssues ?? 0m));
			c.QtyINReceipts = Round((a.QtyINReceipts ?? 0m) - (b.QtyINReceipts ?? 0m));
			c.QtyInTransit = Round((a.QtyInTransit ?? 0m) - (b.QtyInTransit ?? 0m));
            c.QtyInTransitToSO = Round((a.QtyInTransitToSO ?? 0m) - (b.QtyInTransitToSO ?? 0m));
			c.QtyINAssemblyDemand = Round((a.QtyINAssemblyDemand ?? 0m) - (b.QtyINAssemblyDemand ?? 0m));
			c.QtyINAssemblySupply = Round((a.QtyINAssemblySupply ?? 0m) - (b.QtyINAssemblySupply ?? 0m));
			c.QtyPOPrepared = Round((a.QtyPOPrepared ?? 0m) - (b.QtyPOPrepared ?? 0m));
			c.QtyPOOrders = Round((a.QtyPOOrders ?? 0m) - (b.QtyPOOrders ?? 0m));
			c.QtyPOReceipts = Round((a.QtyPOReceipts ?? 0m) - (b.QtyPOReceipts ?? 0m));
			c.QtySOFixed = Round((a.QtySOFixed ?? 0m) - (b.QtySOFixed ?? 0m));
			c.QtyPOFixedOrders = Round((a.QtyPOFixedOrders ?? 0m) - (b.QtyPOFixedOrders ?? 0m));
			c.QtyPOFixedPrepared = Round((a.QtyPOFixedPrepared ?? 0m) - (b.QtyPOFixedPrepared ?? 0m));
			c.QtyPOFixedReceipts = Round((a.QtyPOFixedReceipts ?? 0m) - (b.QtyPOFixedReceipts ?? 0m));
			c.QtySODropShip = Round((a.QtySODropShip ?? 0m) - (b.QtySODropShip ?? 0m));
			c.QtyPODropShipOrders = Round((a.QtyPODropShipOrders ?? 0m) - (b.QtyPODropShipOrders ?? 0m));
			c.QtyPODropShipPrepared = Round((a.QtyPODropShipPrepared ?? 0m) - (b.QtyPODropShipPrepared ?? 0m));
			c.QtyPODropShipReceipts = Round((a.QtyPODropShipReceipts ?? 0m) - (b.QtyPODropShipReceipts ?? 0m));

			ICostStatus cost_a = a as ICostStatus;
			ICostStatus cost_b = b as ICostStatus;
			ICostStatus cost_c = c as ICostStatus;
			if (cost_a != null && cost_b != null && cost_c != null)
			{
				cost_c.TotalCost = (cost_a.TotalCost ?? 0m) - (cost_b.TotalCost ?? 0m);
			}

			return c;
		}

		private T Add<T>(IStatus a, IStatus b)
			where T : class, IBqlTable, IStatus, new()
		{
			T c = new T();
			if (typeof(T).IsAssignableFrom(a.GetType()))
			{
				PXCache<T>.RestoreCopy(c, (T)a);
				Add(c, b);
				return c;
			}

			c.QtyAvail = Round((a.QtyAvail ?? 0m) + (b.QtyAvail ?? 0m));
			c.QtyHardAvail = Round((a.QtyHardAvail ?? 0m) + (b.QtyHardAvail ?? 0m));
			c.QtyNotAvail = Round((a.QtyNotAvail ?? 0m) + (b.QtyNotAvail ?? 0m));
			c.QtyExpired = Round((a.QtyExpired ?? 0m) + (b.QtyExpired ?? 0m));
			c.QtyOnHand = Round((a.QtyOnHand ?? 0m) + (b.QtyOnHand ?? 0m));
			c.QtySOPrepared = Round((a.QtySOPrepared ?? 0m) + (b.QtySOPrepared ?? 0m));
			c.QtySOBooked = Round((a.QtySOBooked ?? 0m) + (b.QtySOBooked ?? 0m));
			c.QtySOShipping = Round((a.QtySOShipping ?? 0m) + (b.QtySOShipping ?? 0m));
			c.QtySOShipped = Round((a.QtySOShipped ?? 0m) + (b.QtySOShipped ?? 0m));
			c.QtySOBackOrdered = Round((a.QtySOBackOrdered ?? 0m) + (b.QtySOBackOrdered ?? 0m));
			c.QtyINIssues = Round((a.QtyINIssues ?? 0m) + (b.QtyINIssues ?? 0m));
			c.QtyINReceipts = Round((a.QtyINReceipts ?? 0m) + (b.QtyINReceipts ?? 0m));
			c.QtyInTransit = Round((a.QtyInTransit ?? 0m) + (b.QtyInTransit ?? 0m));
            c.QtyInTransitToSO = Round((a.QtyInTransitToSO ?? 0m) + (b.QtyInTransitToSO ?? 0m));
			c.QtyINAssemblyDemand = Round((a.QtyINAssemblyDemand ?? 0m) + (b.QtyINAssemblyDemand ?? 0m));
			c.QtyINAssemblySupply = Round((a.QtyINAssemblySupply ?? 0m) + (b.QtyINAssemblySupply ?? 0m));
			c.QtyPOPrepared = Round((a.QtyPOPrepared ?? 0m) + (b.QtyPOPrepared ?? 0m));
			c.QtyPOOrders = Round((a.QtyPOOrders ?? 0m) + (b.QtyPOOrders ?? 0m));
			c.QtyPOReceipts = Round((a.QtyPOReceipts ?? 0m) + (b.QtyPOReceipts ?? 0m));
			c.QtySOFixed = Round((a.QtySOFixed ?? 0m) + (b.QtySOFixed ?? 0m));
			c.QtyPOFixedOrders = Round((a.QtyPOFixedOrders ?? 0m) + (b.QtyPOFixedOrders ?? 0m));
			c.QtyPOFixedPrepared = Round((a.QtyPOFixedPrepared ?? 0m) + (b.QtyPOFixedPrepared ?? 0m));
			c.QtyPOFixedReceipts = Round((a.QtyPOFixedReceipts ?? 0m) + (b.QtyPOFixedReceipts ?? 0m));
			c.QtySODropShip = Round((a.QtySODropShip ?? 0m) + (b.QtySODropShip ?? 0m));
			c.QtyPODropShipOrders = Round((a.QtyPODropShipOrders ?? 0m) + (b.QtyPODropShipOrders ?? 0m));
			c.QtyPODropShipPrepared = Round((a.QtyPODropShipPrepared ?? 0m) + (b.QtyPODropShipPrepared ?? 0m));
			c.QtyPODropShipReceipts = Round((a.QtyPODropShipReceipts ?? 0m) + (b.QtyPODropShipReceipts ?? 0m));

			ICostStatus cost_a = a as ICostStatus;
			ICostStatus cost_b = b as ICostStatus;
			ICostStatus cost_c = c as ICostStatus;
			if (cost_a != null && cost_b != null && cost_c != null)
			{
				cost_c.TotalCost = (cost_a.TotalCost ?? 0m) + (cost_b.TotalCost ?? 0m);
			}

			return c;
		}


		public PXCancel<InventorySummaryEnqFilter> Cancel;
		public PXAction<InventorySummaryEnqFilter> viewAllocDet;
		public PXFilter<InventorySummaryEnqFilter> Filter;
		[PXFilterable]
		public PXSelect<InventorySummaryEnquiryResult> ISERecords; // ISE = Inventory Summary Enquiry
		public PXSetupOptional<CommonSetup> commonsetup;


		public InventorySummaryEnq()
		{
			ISERecords.Cache.AllowInsert = false;
			ISERecords.Cache.AllowDelete = false;
			ISERecords.Cache.AllowUpdate = false;

			object record = commonsetup.Current;
		}

		protected virtual void InventorySummaryEnqFilter_LocationID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = null;
			e.Cancel = true;
		}

		protected virtual void InventorySummaryEnquiryResult_LocationID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = null;
			e.Cancel = true;
		}

		protected virtual void InventorySummaryEnqFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			InventorySummaryEnqFilter filter = (InventorySummaryEnqFilter)e.Row;
			if (filter == null) return;

			bool expandByLotSerialNbr = filter.ExpandByLotSerialNbr ?? false;

			PXUIFieldAttribute.SetVisible<InventorySummaryEnquiryResult.lotSerialNbr>(ISERecords.Cache, null, expandByLotSerialNbr);
			PXUIFieldAttribute.SetVisible<InventorySummaryEnquiryResult.expireDate>(ISERecords.Cache, null, expandByLotSerialNbr);
		}

		protected virtual void AppendFilters<T>(PXSelectBase<T> cmd, InventorySummaryEnqFilter filter)
			where T : class, IBqlTable, new()
		{
			if (filter.InventoryID != null)
			{
				cmd.WhereAnd<Where<InventoryItem.inventoryID, Equal<Current<InventorySummaryEnqFilter.inventoryID>>>>();
			}

			if (!SubCDUtils.IsSubCDEmpty(filter.SubItemCD))
			{
				cmd.WhereAnd<Where<INSubItem.subItemCD, Like<Current<InventorySummaryEnqFilter.subItemCDWildcard>>>>();
			}

			if (filter.SiteID != null)
			{
				cmd.WhereAnd<Where<INSite.siteID, Equal<Current<InventorySummaryEnqFilter.siteID>>>>();
			}

			if (typeof(T) != typeof(INSiteStatus) && filter.LocationID != null)
			{
				cmd.WhereAnd<Where<INLocation.locationID, Equal<Current<InventorySummaryEnqFilter.locationID>>>>();
			}
		}

		protected virtual IEnumerable iSERecords()
		{
			int decPlQty = (commonsetup.Current.DecPlQty ?? 6);
			int decPlPrcCst = (commonsetup.Current.DecPlPrcCst ?? 6);

			InventorySummaryEnqFilter filter = Filter.Current;

			this.Caches[typeof(INSiteStatus)].Clear();
			this.Caches[typeof(INLocationStatus)].Clear();

			this.Caches[typeof(SiteStatus)].Clear();
			this.Caches[typeof(LocationStatus)].Clear();

			PXSelectBase<INLotSerialStatus> cmd_lss = new PXSelectReadonly2<INLotSerialStatus,
				InnerJoin<INLocation,
					On<INLocation.siteID, Equal<INLotSerialStatus.siteID>, And<INLocation.locationID, Equal<INLotSerialStatus.locationID>>>,
				InnerJoin<InventoryItem,
					On<InventoryItem.inventoryID, Equal<INLotSerialStatus.inventoryID>,
					And<Match<InventoryItem, Current<AccessInfo.userName>>>>,
				InnerJoin<INSite,
					On<INSite.siteID, Equal<INLotSerialStatus.siteID>,
					And<Match<INSite, Current<AccessInfo.userName>>>>,
				InnerJoin<INSubItem,
					On<INSubItem.subItemID, Equal<INLotSerialStatus.subItemID>>,
				LeftJoin<INLotSerClass,
					On<INLotSerClass.lotSerClassID, Equal<InventoryItem.lotSerClassID>>,
				LeftJoin<INLotSerialCostStatus,
					On<INLotSerialCostStatus.inventoryID, Equal<INLotSerialStatus.inventoryID>,
						And<INLotSerialCostStatus.subItemID, Equal<INLotSerialStatus.subItemID>,
						And<INLotSerialCostStatus.siteID, Equal<INLotSerialStatus.siteID>,
						And<INLotSerialCostStatus.lotSerialNbr, Equal<INLotSerialStatus.lotSerialNbr>>>>>,
				LeftJoin<INLocationCostStatus,
					On<INLocationCostStatus.inventoryID, Equal<INLotSerialStatus.inventoryID>,
						And<INLocationCostStatus.subItemID, Equal<INLotSerialStatus.subItemID>,
						And<INLocationCostStatus.locationID, Equal<INLotSerialStatus.locationID>>>>,
				LeftJoin<INSiteCostStatus,
					On<INSiteCostStatus.inventoryID, Equal<INLotSerialStatus.inventoryID>,
						And<INSiteCostStatus.subItemID, Equal<INLotSerialStatus.subItemID>,
						And<INSiteCostStatus.siteID, Equal<INLotSerialStatus.siteID>>>>>>>>>>>>,
				Where<INLotSerialStatus.inventoryID, Equal<Current<InventorySummaryEnqFilter.inventoryID>>,
					And<Where<Current<InventorySummaryEnqFilter.expandByLotSerialNbr>, Equal<True>, And<INLotSerClass.lotSerAssign, Equal<INLotSerAssign.whenReceived>,
					Or<InventoryItem.valMethod, Equal<INValMethod.specific>>>>>>>(this);

			AppendFilters<INLotSerialStatus>(cmd_lss, filter);

			foreach (PXResult<INLotSerialStatus, INLocation, InventoryItem> res in cmd_lss.Select())
			{
				INLotSerialStatus lss_rec = res;
				INLocation loc_rec = res;
				InventoryItem item_rec = res;

				InventorySummaryEnquiryResult ret = Copy<InventorySummaryEnquiryResult>(lss_rec);
				ret.InventoryID = lss_rec.InventoryID;
				ret.SubItemID = lss_rec.SubItemID;
				ret.SiteID = lss_rec.SiteID;
				ret.LocationID = lss_rec.LocationID;
				ret.LotSerialNbr = lss_rec.LotSerialNbr;
				ret.ExpireDate = lss_rec.ExpireDate;
				ret.BaseUnit = item_rec.BaseUnit;
				ret.TotalCost = 0m;
				ret.QtyExpired = 0m;

				INLotSerialCostStatus lscs_rec = PXResult.Unwrap<INLotSerialCostStatus>(res);
				INLocationCostStatus lcs_rec = PXResult.Unwrap<INLocationCostStatus>(res);
				INSiteCostStatus scs_rec = PXResult.Unwrap<INSiteCostStatus>(res);

				if (lscs_rec.InventoryID != null)
				{
					ret.UnitCost = lscs_rec.UnitCost;
					ret.TotalCost = PXDBCurrencyAttribute.BaseRound(this, (decimal)(ret.QtyOnHand * lscs_rec.UnitCost));
				}
				else if (lcs_rec.InventoryID != null)
				{
					ret.UnitCost = lcs_rec.UnitCost;
					ret.TotalCost = PXDBCurrencyAttribute.BaseRound(this, (decimal)(ret.QtyOnHand * lcs_rec.UnitCost));
				}
				else if (scs_rec.InventoryID != null)
				{
					ret.UnitCost = scs_rec.UnitCost;
					ret.TotalCost = PXDBCurrencyAttribute.BaseRound(this, (decimal)(ret.QtyOnHand * scs_rec.UnitCost));
				}

				if (loc_rec.InclQtyAvail == false)
				{
					ret.QtyNotAvail = ret.QtyAvail;
					ret.QtyAvail = 0m;
					ret.QtyHardAvail = 0m;
				}
				else
				{
					ret.QtyNotAvail = 0m;
				}

				ret.ExpireDate = lss_rec.ExpireDate;
				if (lss_rec.ExpireDate != null && DateTime.Compare((DateTime)this.Accessinfo.BusinessDate, (DateTime)lss_rec.ExpireDate) > 0)
				{
					ret.QtyExpired = lss_rec.QtyOnHand;
				}

				if (PXAccess.FeatureInstalled<FeaturesSet.warehouseLocation>())
				{
					PXCache cache = this.Caches[typeof(LocationStatus)];
					LocationStatus aggregate = new LocationStatus();
					aggregate.InventoryID = ret.InventoryID;
					aggregate.SubItemID = ret.SubItemID;
					aggregate.SiteID = ret.SiteID;
					aggregate.LocationID = ret.LocationID;

					aggregate = (LocationStatus)cache.Insert(aggregate);
					Add(aggregate, ret);
				}
				else
				{
					PXCache cache = this.Caches[typeof(SiteStatus)];
					SiteStatus aggregate = new SiteStatus();
					aggregate.InventoryID = ret.InventoryID;
					aggregate.SubItemID = ret.SubItemID;
					aggregate.SiteID = ret.SiteID;

					aggregate = (SiteStatus)cache.Insert(aggregate);
					Add(aggregate, ret);
				}

				if (filter.ExpandByLotSerialNbr == true)
				{
					if (!IsZero(ret)) yield return ret;
				}
			}

			PXSelectBase<INLocationStatus> cmd_ls = new PXSelectReadonly2<INLocationStatus,
				InnerJoin<INLocation, 
					On<INLocation.siteID, Equal<INLocationStatus.siteID>, And<INLocation.locationID, Equal<INLocationStatus.locationID>>>,
				InnerJoin<InventoryItem,
					On<InventoryItem.inventoryID, Equal<INLocationStatus.inventoryID>,
					And<Match<InventoryItem, Current<AccessInfo.userName>>>>,
				InnerJoin<INSite,
					On<INSite.siteID, Equal<INLocationStatus.siteID>,
					And<Match<INSite, Current<AccessInfo.userName>>>>,
				InnerJoin<INSubItem,
					On<INSubItem.subItemID, Equal<INLocationStatus.subItemID>>,
				LeftJoin<INLotSerClass,
					On<INLotSerClass.lotSerClassID, Equal<InventoryItem.lotSerClassID>>,
				LeftJoin<INLocationCostStatus,
					On<INLocationCostStatus.inventoryID, Equal<INLocationStatus.inventoryID>,
						And<INLocationCostStatus.subItemID, Equal<INLocationStatus.subItemID>,
						And<INLocationCostStatus.locationID, Equal<INLocationStatus.locationID>>>>,
				LeftJoin<INSiteCostStatus,
					On<INSiteCostStatus.inventoryID, Equal<INLocationStatus.inventoryID>,
						And<INSiteCostStatus.subItemID, Equal<INLocationStatus.subItemID>,
						And<INSiteCostStatus.siteID, Equal<INLocationStatus.siteID>>>>>>>>>>>,
				Where<INLocationStatus.inventoryID, Equal<Current<InventorySummaryEnqFilter.inventoryID>>>>(this);

			AppendFilters<INLocationStatus>(cmd_ls, filter);

			foreach (PXResult<INLocationStatus, INLocation, InventoryItem> res in cmd_ls.Select())
			{
				INLocationStatus ls_rec = res;
				INLocation loc_rec = res;
				InventoryItem item_rec = res;

				InventorySummaryEnquiryResult ret = Copy<InventorySummaryEnquiryResult>(ls_rec);
				ret.InventoryID = ls_rec.InventoryID;
				ret.SubItemID = ls_rec.SubItemID;
				ret.SiteID = ls_rec.SiteID;
				ret.LocationID = ls_rec.LocationID;
				ret.BaseUnit = item_rec.BaseUnit;
				ret.UnitCost = ret.UnitCost ?? 0m;
				ret.TotalCost = 0m;
				ret.QtyExpired = 0m;

				INLocationCostStatus lcs_rec = PXResult.Unwrap<INLocationCostStatus>(res);
				INSiteCostStatus scs_rec = PXResult.Unwrap<INSiteCostStatus>(res);
				if (lcs_rec.InventoryID != null)
				{
					ret.UnitCost = lcs_rec.UnitCost;
					ret.TotalCost = PXDBCurrencyAttribute.BaseRound(this, (decimal)(ret.QtyOnHand * lcs_rec.UnitCost));
				}
				else if (scs_rec.InventoryID != null)
				{
					ret.UnitCost = scs_rec.UnitCost;
					ret.TotalCost = PXDBCurrencyAttribute.BaseRound(this, (decimal)(ret.QtyOnHand * scs_rec.UnitCost));
				}

				if (loc_rec.InclQtyAvail == false)
				{
					ret.QtyNotAvail = ret.QtyAvail;
					ret.QtyAvail = 0m;
					ret.QtyHardAvail = 0m;
				}
				else
				{
					ret.QtyNotAvail = 0m; 
				}

				if (PXAccess.FeatureInstalled<FeaturesSet.warehouseLocation>())
				{
					PXCache cache = this.Caches[typeof(SiteStatus)];
					SiteStatus aggregate = new SiteStatus();
					aggregate.InventoryID = ret.InventoryID;
					aggregate.SubItemID = ret.SubItemID;
					aggregate.SiteID = ret.SiteID;

					aggregate = (SiteStatus)cache.Insert(aggregate);
					Add(aggregate, ret);
				}

				{
					PXCache cache = this.Caches[typeof(LocationStatus)];
					LocationStatus aggregate = new LocationStatus();
					aggregate.InventoryID = ret.InventoryID;
					aggregate.SubItemID = ret.SubItemID;
					aggregate.SiteID = ret.SiteID;
					aggregate.LocationID = ret.LocationID;

					aggregate = (LocationStatus)cache.Insert(aggregate);
					if (filter.ExpandByLotSerialNbr == true)
					{
						ret = Subtract<InventorySummaryEnquiryResult>(ret, aggregate);

						if (PXAccess.FeatureInstalled<FeaturesSet.warehouseLocation>())
						{
							if (!IsZero(ret)) yield return ret;
						}
						continue;
					}
					else if (aggregate.TotalCost != 0m)
					{
						ret.TotalCost = aggregate.TotalCost;
						ret.UnitCost = RoundUnitCost(aggregate.QtyOnHand != 0m ? aggregate.TotalCost / aggregate.QtyOnHand : 0m);
					}
				}

				if (PXAccess.FeatureInstalled<FeaturesSet.warehouseLocation>())
				{
					if (!IsZero(ret)) yield return ret;
				}
			}

			PXSelectBase<INSiteStatus> cmd_ss = new PXSelectReadonly2<INSiteStatus,
				InnerJoin<InventoryItem,
					On<InventoryItem.inventoryID, Equal<INSiteStatus.inventoryID>,
					And<Match<InventoryItem, Current<AccessInfo.userName>>>>,
				InnerJoin<INSite,
					On<INSite.siteID, Equal<INSiteStatus.siteID>,
					And<Match<INSite, Current<AccessInfo.userName>>>>,
				InnerJoin<INSubItem,
					On<INSubItem.subItemID, Equal<INSiteStatus.subItemID>>,
				LeftJoin<INLotSerClass,
					On<INLotSerClass.lotSerClassID, Equal<InventoryItem.lotSerClassID>>>>>>,
				Where<INSiteStatus.inventoryID, Equal<Current<InventorySummaryEnqFilter.inventoryID>>>>(this);

			AppendFilters<INSiteStatus>(cmd_ss, filter);

			foreach (PXResult<INSiteStatus, InventoryItem> res in cmd_ss.Select())
			{
				INSiteStatus ss_rec = res;
				InventoryItem item_rec = res;

				InventorySummaryEnquiryResult ret = Copy<InventorySummaryEnquiryResult>(ss_rec);
				ret.InventoryID = ss_rec.InventoryID;
				ret.SubItemID = ss_rec.SubItemID;
				ret.SiteID = ss_rec.SiteID;
				ret.BaseUnit = item_rec.BaseUnit;
				ret.UnitCost = ret.UnitCost ?? 0m;
				ret.TotalCost = 0m;
				ret.QtyExpired = 0m;

				{
					PXCache cache = this.Caches[typeof(SiteStatus)];
					SiteStatus aggregate = new SiteStatus();
					aggregate.InventoryID = ret.InventoryID;
					aggregate.SubItemID = ret.SubItemID;
					aggregate.SiteID = ret.SiteID;

					aggregate = (SiteStatus)cache.Insert(aggregate);

					ret = Subtract<InventorySummaryEnquiryResult>(ret, aggregate);
				}

				if (!PXAccess.FeatureInstalled<FeaturesSet.warehouseLocation>() || filter.LocationID == null)
				{
					if (!IsZero(ret)) yield return ret;
				}
			}
		}

		public override bool IsDirty
		{
			get
			{
				return false;
			}
		}

		
		[PXUIField(DisplayName = "")]
		[PXEditDetailButton]
		protected virtual IEnumerable ViewAllocDet(PXAdapter a)
		{
			if (this.ISERecords.Current != null)
			{
				object subItem =
						this.ISERecords.Cache.GetValueExt<InventorySummaryEnquiryResult.subItemID>
					(this.ISERecords.Current);

				if (subItem is PXSegmentedState)
					subItem = ((PXSegmentedState)subItem).Value;

				InventoryAllocDetEnq.Redirect(
					this.ISERecords.Current.InventoryID,
					subItem != null ? (string)subItem : null,
					this.Filter.Current.ExpandByLotSerialNbr == true ?
					this.ISERecords.Current.LotSerialNbr : null,
					this.ISERecords.Current.SiteID,
					this.ISERecords.Current.LocationID);
			}
			return a.Get();
		}

		public PXAction<InventorySummaryEnqFilter> viewItem;
		[PXButton()]
		[PXUIField(DisplayName = "")]
		protected virtual IEnumerable ViewItem(PXAdapter a)
		{
			if (this.ISERecords.Current != null)
				InventoryItemMaint.Redirect(this.ISERecords.Current.InventoryID, true);
			return a.Get();
		}

		public static void Redirect(int? inventoryID, string subItemCD, int? siteID, int? locationID)
		{
			Redirect(inventoryID, subItemCD, siteID, locationID, true);
		}

		public static void Redirect(int? inventoryID, string subItemCD, int? siteID, int? locationID, bool newWindow)
		{
			InventorySummaryEnq graph = PXGraph.CreateInstance<InventorySummaryEnq>();
			graph.Filter.Current.InventoryID = inventoryID;
			graph.Filter.Current.SubItemCD = subItemCD;
			graph.Filter.Current.SiteID = siteID;
			graph.Filter.Current.LocationID = locationID;

			InventorySummaryEnqFilter filter = new InventorySummaryEnqFilter();
			if (newWindow)
				throw new PXRedirectRequiredException(graph, true, Messages.InventorySummary) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
			else
				throw new PXRedirectRequiredException(graph, Messages.InventorySummary);
			//throw new PXPopupRedirectException(graph, Messages.InventorySummary, false, false);
		}
	}
}
