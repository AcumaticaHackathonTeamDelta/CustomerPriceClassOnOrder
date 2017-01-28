namespace PX.Objects.IN
{
	using System;
	using PX.Data;

	[System.SerializableAttribute()]
    [PXHidden]
	public partial class INPlanType : PX.Data.IBqlTable
	{
		#region PlanType
		public abstract class planType : PX.Data.IBqlField
		{
		}
		protected String _PlanType;
		[PXDBString(2, IsKey = true, IsFixed = true, InputMask=">aa")]
		[PXDefault()]
		[PXUIField(DisplayName="Plan Type", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String Descr
		{
			get
			{
				return this._Descr;
			}
			set
			{
				this._Descr = value;
			}
		}
		#endregion
		#region IsFixed
		public abstract class isFixed : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsFixed;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName="Is Fixed", Enabled = false)]
		public virtual Boolean? IsFixed
		{
			get
			{
				return this._IsFixed;
			}
			set
			{
				this._IsFixed = value;
			}
		}
		#endregion
		#region IsSupply
		public abstract class isSupply : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsSupply;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Is Supply", Enabled = false)]
		public virtual Boolean? IsSupply
		{
			get
			{
				return this._IsSupply;
			}
			set
			{
				this._IsSupply = value;
			}
		}
		#endregion
		#region IsDemand
		public abstract class isDemand : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsDemand;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Is Demand", Enabled = false)]
		public virtual Boolean? IsDemand
		{
			get
			{
				return this._IsDemand;
			}
			set
			{
				this._IsDemand = value;
			}
		}
		#endregion
		#region IsForDate
		public abstract class isForDate : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsForDate;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Planned for Date", Enabled = false)]
		public virtual Boolean? IsForDate
		{
			get
			{
				return this._IsForDate;
			}
			set
			{
				this._IsForDate = value;
			}
		}
		#endregion		
		#region InclQtySOBackOrdered
		public abstract class inclQtySOBackOrdered : PX.Data.IBqlField
		{
		}
		protected Int16? _InclQtySOBackOrdered;
		[PXDBShort()]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "SO Back Ordered", Enabled = false)]
		public virtual Int16? InclQtySOBackOrdered
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
		#region InclQtySOPrepared
		public abstract class inclQtySOPrepared : PX.Data.IBqlField
		{
		}
		protected Int16? _InclQtySOPrepared;
		[PXDBShort()]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "SO Prepared", Enabled = false)]
		public virtual Int16? InclQtySOPrepared
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
		#region InclQtySOBooked
		public abstract class inclQtySOBooked : PX.Data.IBqlField
		{
		}
		protected Int16? _InclQtySOBooked;
		[PXDBShort()]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "SO Booked", Enabled = false)]
		public virtual Int16? InclQtySOBooked
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
		#region InclQtySOShipped
		public abstract class inclQtySOShipped : PX.Data.IBqlField
		{
		}
		protected Int16? _InclQtySOShipped;
		[PXDBShort()]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "SO Shipped", Enabled = false)]
		public virtual Int16? InclQtySOShipped
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
		#region InclQtySOShipping
		public abstract class inclQtySOShipping : PX.Data.IBqlField
		{
		}
		protected Int16? _InclQtySOShipping;
		[PXDBShort()]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "SO Allocated", Enabled = false)]
		public virtual Int16? InclQtySOShipping
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
		#region InclQtyInTransit
		public abstract class inclQtyInTransit : PX.Data.IBqlField
		{
		}
		protected Int16? _InclQtyInTransit;
		[PXDBShort()]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "In Transit", Enabled = false)]
		public virtual Int16? InclQtyInTransit
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
        #region InclQtyInTransitToSO
        public abstract class inclQtyInTransitToSO : PX.Data.IBqlField
        {
        }
        protected Int16? _InclQtyInTransitToSO;
        [PXDBShort()]
        [PXDefault((short)0)]
        [PXUIField(DisplayName = "In Transit to SO", Enabled = false)]
        public virtual Int16? InclQtyInTransitToSO
        {
            get
            {
                return this._InclQtyInTransitToSO;
            }
            set
            {
                this._InclQtyInTransitToSO = value;
            }
        }
        #endregion
		#region InclQtyPOReceipts
		public abstract class inclQtyPOReceipts : PX.Data.IBqlField
		{
		}
		protected Int16? _InclQtyPOReceipts;
		[PXDBShort()]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "PO Receipt", Enabled = false)]
		public virtual Int16? InclQtyPOReceipts
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
		#region InclQtyPOPrepared
		public abstract class inclQtyPOPrepared : PX.Data.IBqlField
		{
		}
		protected Int16? _InclQtyPOPrepared;
		[PXDBShort()]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "PO Prepared", Enabled = false)]
		public virtual Int16? InclQtyPOPrepared
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
		#region InclQtyPOOrders
		public abstract class inclQtyPOOrders : PX.Data.IBqlField
		{
		}
		protected Int16? _InclQtyPOOrders;
		[PXDBShort()]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "PO Order", Enabled = false)]
		public virtual Int16? InclQtyPOOrders
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
		#region InclQtyINIssues
		public abstract class inclQtyINIssues : PX.Data.IBqlField
		{
		}
		protected Int16? _InclQtyINIssues;
		[PXDBShort()]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "IN Issues", Enabled = false)]
		public virtual Int16? InclQtyINIssues
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
		#region InclQtyINReceipts
		public abstract class inclQtyINReceipts : PX.Data.IBqlField
		{
		}
		protected Int16? _InclQtyINReceipts;
		[PXDBShort()]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "IN Receipts", Enabled = false)]
		public virtual Int16? InclQtyINReceipts
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
		#region InclQtyINAssemblyDemand
		public abstract class inclQtyINAssemblyDemand : PX.Data.IBqlField
		{
		}
		protected Int16? _InclQtyINAssemblyDemand;
		[PXDBShort()]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "IN Assembly Demand", Enabled = false)]
		public virtual Int16? InclQtyINAssemblyDemand
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
		#region InclQtyINAssemblySupply
		public abstract class inclQtyINAssemblySupply : PX.Data.IBqlField
		{
		}
		protected Int16? _InclQtyINAssemblySupply;
		[PXDBShort()]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "IN Assembly Supply", Enabled = false)]
		public virtual Int16? InclQtyINAssemblySupply
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
		#region InclQtyINReplaned
		public abstract class inclQtyINReplaned : PX.Data.IBqlField
		{
		}
		protected Int16? _InclQtyINReplaned;
		[PXDBShort()]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "IN Replaned", Enabled = false)]
		public virtual Int16? InclQtyINReplaned
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
		#region InclQtySOFixed
		public abstract class inclQtySOFixed : IBqlField { }
		protected short? _InclQtySOFixed;
		[PXDBShort()]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "SO to Purchase", Enabled = false)]
		public virtual short? InclQtySOFixed
		{
			get
			{
				return this._InclQtySOFixed;
			}
			set
			{
				this._InclQtySOFixed = value;
			}
		}
		#endregion
		#region InclQtyPOFixedOrders
		public abstract class inclQtyPOFixedOrders : IBqlField { }
		protected short? _InclQtyPOFixedOrders;
		[PXDBShort()]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "Purchase for SO", Enabled = false)]
		public virtual short? InclQtyPOFixedOrders
		{
			get
			{
				return this._InclQtyPOFixedOrders;
			}
			set
			{
				this._InclQtyPOFixedOrders = value;
			}
		}
		#endregion
		#region InclQtyPOFixedPrepared
		public abstract class inclQtyPOFixedPrepared : IBqlField { }
		protected short? _InclQtyPOFixedPrepared;
		[PXDBShort()]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "Purchase for SO Prepared", Enabled = false)]
		public virtual short? InclQtyPOFixedPrepared
		{
			get
			{
				return this._InclQtyPOFixedPrepared;
			}
			set
			{
				this._InclQtyPOFixedPrepared = value;
			}
		}
		#endregion
		#region InclQtyPOFixedReceipts
		public abstract class inclQtyPOFixedReceipts : IBqlField { }
		protected short? _InclQtyPOFixedReceipts;
		[PXDBShort()]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "Receipts for SO", Enabled = false)]
		public virtual short? InclQtyPOFixedReceipts
		{
			get
			{
				return this._InclQtyPOFixedReceipts;
			}
			set
			{
				this._InclQtyPOFixedReceipts = value;
			}
		}
		#endregion
		#region InclQtySODropShip
		public abstract class inclQtySODropShip : IBqlField { }
		protected short? _InclQtySODropShip;
		[PXDBShort()]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "SO to Drop-Ship", Enabled = false)]
		public virtual short? InclQtySODropShip
		{
			get
			{
				return this._InclQtySODropShip;
			}
			set
			{
				this._InclQtySODropShip = value;
			}
		}
		#endregion
		#region InclQtyPODropShipOrders
		public abstract class inclQtyPODropShipOrders : IBqlField { }
		protected short? _InclQtyPODropShipOrders;
		[PXDBShort()]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "Drop-Ship for SO", Enabled = false)]
		public virtual short? InclQtyPODropShipOrders
		{
			get
			{
				return this._InclQtyPODropShipOrders;
			}
			set
			{
				this._InclQtyPODropShipOrders = value;
			}
		}
		#endregion
		#region InclQtyPODropShipPrepared
		public abstract class inclQtyPODropShipPrepared : IBqlField { }
		protected short? _InclQtyPODropShipPrepared;
		[PXDBShort()]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "Drop-Ship for SO Prepared", Enabled = false)]
		public virtual short? InclQtyPODropShipPrepared
		{
			get
			{
				return this._InclQtyPODropShipPrepared;
			}
			set
			{
				this._InclQtyPODropShipPrepared = value;
			}
		}
		#endregion
		#region InclQtyPODropShipReceipts
		public abstract class inclQtyPODropShipReceipts : IBqlField { }
		protected short? _InclQtyPODropShipReceipts;
		[PXDBShort()]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "Drop-Ship for SO Receipts", Enabled = false)]
		public virtual short? InclQtyPODropShipReceipts
		{
			get
			{
				return this._InclQtyPODropShipReceipts;
			}
			set
			{
				this._InclQtyPODropShipReceipts = value;
			}
		}
        #endregion
        #region DeleteOnEvent
        public abstract class deleteOnEvent : PX.Data.IBqlField
		{
		}
		protected Boolean? _DeleteOnEvent;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Delete On Event", Enabled = false)]
		public virtual Boolean? DeleteOnEvent
		{
			get
			{
				return this._DeleteOnEvent;
			}
			set
			{
				this._DeleteOnEvent = value;
			}
		}
		#endregion
		#region ReplanOnEvent
		public abstract class replanOnEvent : PX.Data.IBqlField
		{
		}
		protected String _ReplanOnEvent;
		[PXDBString(2, IsFixed = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Replan On Event")]
		public virtual String ReplanOnEvent
		{
			get
			{
				return this._ReplanOnEvent;
			}
			set
			{
				this._ReplanOnEvent = value;
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
		public static INPlanType operator -(INPlanType t1)
		{
			INPlanType ret = PXCache<INPlanType>.CreateCopy(t1);
			ret.InclQtyINIssues = (short)-ret.InclQtyINIssues;
			ret.InclQtyINReceipts = (short)-ret.InclQtyINReceipts;
			ret.InclQtyInTransit = (short)-ret.InclQtyInTransit;
            ret.InclQtyInTransitToSO = (short)-ret.InclQtyInTransitToSO;
            ret.InclQtyPOReceipts = (short)-ret.InclQtyPOReceipts;
			ret.InclQtyPOPrepared = (short)-ret.InclQtyPOPrepared;
			ret.InclQtyPOOrders = (short)-ret.InclQtyPOOrders;
			ret.InclQtySOBackOrdered = (short)-ret.InclQtySOBackOrdered;
			ret.InclQtySOPrepared = (short)-ret.InclQtySOPrepared;
			ret.InclQtySOBooked = (short)-ret.InclQtySOBooked;
			ret.InclQtySOShipped = (short)-ret.InclQtySOShipped;
			ret.InclQtySOShipping = (short)-ret.InclQtySOShipping;
			ret.InclQtyINAssemblySupply = (short)-ret.InclQtyINAssemblySupply;
			ret.InclQtyINAssemblyDemand = (short)-ret.InclQtyINAssemblyDemand;
			ret.InclQtyINReplaned = (short)-ret.InclQtyINReplaned;
			ret.InclQtySOFixed = (short)-ret.InclQtySOFixed;
			ret.InclQtyPOFixedOrders = (short)-ret.InclQtyPOFixedOrders;
			ret.InclQtyPOFixedPrepared = (short)-ret.InclQtyPOFixedPrepared;
			ret.InclQtyPOFixedReceipts = (short)-ret.InclQtyPOFixedReceipts;
			ret.InclQtySODropShip = (short)-ret.InclQtySODropShip;
			ret.InclQtyPODropShipOrders = (short)-ret.InclQtyPODropShipOrders;
			ret.InclQtyPODropShipPrepared = (short)-ret.InclQtyPODropShipPrepared;
			ret.InclQtyPODropShipReceipts = (short)-ret.InclQtyPODropShipReceipts;
            
            return ret;
		}

		public static INPlanType operator -(INPlanType t1, INPlanType t2)
		{
			INPlanType ret = PXCache<INPlanType>.CreateCopy(t1);
			ret.InclQtyINIssues = (short)(t1.InclQtyINIssues - t2.InclQtyINIssues);
			ret.InclQtyINReceipts = (short)(t1.InclQtyINReceipts - t2.InclQtyINReceipts);
			ret.InclQtyInTransit = (short)(t1.InclQtyInTransit - t2.InclQtyInTransit);
            ret.InclQtyInTransitToSO = (short)(t1.InclQtyInTransitToSO - t2.InclQtyInTransitToSO);
            ret.InclQtyPOReceipts = (short)(t1.InclQtyPOReceipts - t2.InclQtyPOReceipts);
			ret.InclQtyPOPrepared = (short)(t1.InclQtyPOPrepared - t2.InclQtyPOPrepared);
			ret.InclQtyPOOrders = (short)(t1.InclQtyPOOrders - t2.InclQtyPOOrders);
			ret.InclQtySOBackOrdered = (short)(t1.InclQtySOBackOrdered - t2.InclQtySOBackOrdered);
			ret.InclQtySOPrepared = (short)(t1.InclQtySOPrepared - t2.InclQtySOPrepared);
			ret.InclQtySOBooked = (short)(t1.InclQtySOBooked - t2.InclQtySOBooked);
			ret.InclQtySOShipped = (short)(t1.InclQtySOShipped - t2.InclQtySOShipped);
			ret.InclQtySOShipping = (short)(t1.InclQtySOShipping - t2.InclQtySOShipping);
			ret.InclQtyINAssemblySupply = (short)(t1.InclQtyINAssemblySupply - t2.InclQtyINAssemblySupply);
			ret.InclQtyINAssemblyDemand = (short)(t1.InclQtyINAssemblyDemand - t2.InclQtyINAssemblyDemand);
			ret.InclQtyINReplaned = (short)(t1.InclQtyINReplaned - t2.InclQtyINReplaned);
			ret.InclQtySOFixed = (short)(t1.InclQtySOFixed - t2.InclQtySOFixed);
			ret.InclQtyPOFixedOrders = (short)(t1.InclQtyPOFixedOrders - t2.InclQtyPOFixedOrders);
			ret.InclQtyPOFixedPrepared = (short)(t1.InclQtyPOFixedPrepared - t2.InclQtyPOFixedPrepared);
			ret.InclQtyPOFixedReceipts = (short)(t1.InclQtyPOFixedReceipts - t2.InclQtyPOFixedReceipts);
			ret.InclQtySODropShip = (short)(t1.InclQtySODropShip - t2.InclQtySODropShip);
			ret.InclQtyPODropShipOrders = (short)(t1.InclQtyPODropShipOrders - t2.InclQtyPODropShipOrders);
			ret.InclQtyPODropShipPrepared = (short)(t1.InclQtyPODropShipPrepared - t2.InclQtyPODropShipPrepared);
			ret.InclQtyPODropShipReceipts = (short)(t1.InclQtyPODropShipReceipts - t2.InclQtyPODropShipReceipts);

            return ret;
		}

		public static INPlanType operator +(INPlanType t1, INPlanType t2)
		{
			return t1 - (-t2);
		}

		/*
		public static INPlanType operator ^(INPlanType t1, INPlanType t2)
		{
			INPlanType ret = new INPlanType();

			ret.InclQtyINIssues = (short)(t1.InclQtyINIssues != 0 && t2.InclQtyINIssues != 0 ? 0 : 1);
			ret.InclQtyINReceipts = (short)(t1.InclQtyINReceipts != 0 && t2.InclQtyINReceipts != 0 ? 0 : 1);
			ret.InclQtyInTransit = (short)(t1.InclQtyInTransit != 0 && t2.InclQtyInTransit != 0 ? 0 : 1);
			ret.InclQtyPOReceipts = (short)(t1.InclQtyPOReceipts != 0 && t2.InclQtyPOReceipts != 0 ? 0 : 1);
			ret.InclQtyPOPrepared = (short)(t1.InclQtyPOPrepared != 0 && t2.InclQtyPOPrepared != 0 ? 0 : 1);
			ret.InclQtyPOOrders = (short)(t1.InclQtyPOOrders != 0 && t2.InclQtyPOOrders != 0 ? 0 : 1);
			ret.InclQtySOBackOrdered = (short)(t1.InclQtySOBackOrdered != 0 && t2.InclQtySOBackOrdered != 0 ? 0 : 1);
			ret.InclQtySOPrepared = (short)(t1.InclQtySOPrepared != 0 && t2.InclQtySOPrepared != 0 ? 0 : 1);
			ret.InclQtySOBooked = (short)(t1.InclQtySOBooked != 0 && t2.InclQtySOBooked != 0 ? 0 : 1);
			ret.InclQtySOShipped = (short)(t1.InclQtySOShipped != 0 && t2.InclQtySOShipped != 0 ? 0 : 1);
			ret.InclQtySOShipping = (short)(t1.InclQtySOShipping != 0 && t2.InclQtySOShipping != 0 ? 0 : 1);
			ret.InclQtyINAssemblySupply = (short)(t1.InclQtyINAssemblySupply != 0 && t2.InclQtyINAssemblySupply != 0 ? 0 : 1);
			ret.InclQtyINAssemblyDemand = (short)(t1.InclQtyINAssemblyDemand != 0 && t2.InclQtyINAssemblyDemand != 0 ? 0 : 1);
			ret.InclQtyINReplaned = (short)(t1.InclQtyINReplaned != 0 && t2.InclQtyINReplaned != 0 ? 0 : 1);
			ret.InclQtySOFixed = (short)(t1.InclQtySOFixed != 0 && t2.InclQtySOFixed != 0 ? 0 : 1);
			ret.InclQtyPOFixedOrders = (short)(t1.InclQtyPOFixedOrders != 0 && t2.InclQtyPOFixedOrders != 0 ? 0 : 1);
			ret.InclQtyPOFixedPrepared = (short)(t1.InclQtyPOFixedPrepared != 0 && t2.InclQtyPOFixedPrepared != 0 ? 0 : 1);
			ret.InclQtyPOFixedReceipts = (short)(t1.InclQtyPOFixedReceipts != 0 && t2.InclQtyPOFixedReceipts != 0 ? 0 : 1);
			ret.InclQtySODropShip = (short)(t1.InclQtySODropShip != 0 && t2.InclQtySODropShip != 0 ? 0 : 1);
			ret.InclQtyPODropShipOrders = (short)(t1.InclQtyPODropShipOrders != 0 && t2.InclQtyPODropShipOrders != 0 ? 0 : 1);
			ret.InclQtyPODropShipPrepared = (short)(t1.InclQtyPODropShipPrepared != 0 && t2.InclQtyPODropShipPrepared != 0 ? 0 : 1);
			ret.InclQtyPODropShipReceipts = (short)(t1.InclQtyPODropShipReceipts != 0 && t2.InclQtyPODropShipReceipts != 0 ? 0 : 1);
			return ret;
		}
		*/

		/*
		public static INPlanType operator *(INPlanType t1, INPlanType t2)
		{
			INPlanType ret = new INPlanType();
			ret.InclQtyINIssues = (short)(t1.InclQtyINIssues * t2.InclQtyINIssues);
			ret.InclQtyINReceipts = (short)(t1.InclQtyINReceipts * t2.InclQtyINReceipts);
			ret.InclQtyInTransit = (short)(t1.InclQtyInTransit * t2.InclQtyInTransit);
			ret.InclQtyPOReceipts = (short)(t1.InclQtyPOReceipts * t2.InclQtyPOReceipts);
			ret.InclQtyPOPrepared = (short)(t1.InclQtyPOPrepared * t2.InclQtyPOPrepared);
			ret.InclQtyPOOrders = (short)(t1.InclQtyPOOrders * t2.InclQtyPOOrders);
			ret.InclQtySOBackOrdered = (short)(t1.InclQtySOBackOrdered * t2.InclQtySOBackOrdered);
			ret.InclQtySOPrepared = (short)(t1.InclQtySOPrepared * t2.InclQtySOPrepared);
			ret.InclQtySOBooked = (short)(t1.InclQtySOBooked * t2.InclQtySOBooked);
			ret.InclQtySOShipped = (short)(t1.InclQtySOShipped * t2.InclQtySOShipped);
			ret.InclQtySOShipping = (short)(t1.InclQtySOShipping * t2.InclQtySOShipping);
			ret.InclQtyINAssemblySupply = (short)(t1.InclQtyINAssemblySupply * t2.InclQtyINAssemblySupply);
			ret.InclQtyINAssemblyDemand = (short)(t1.InclQtyINAssemblyDemand * t2.InclQtyINAssemblyDemand);
			ret.InclQtyINReplaned = (short)(t1.InclQtyINReplaned * t2.InclQtyINReplaned);
			ret.InclQtySOFixed = (short)(t1.InclQtySOFixed * t2.InclQtySOFixed);
			ret.InclQtyPOFixedOrders = (short)(t1.InclQtyPOFixedOrders * t2.InclQtyPOFixedOrders);
			ret.InclQtyPOFixedPrepared = (short)(t1.InclQtyPOFixedPrepared * t2.InclQtyPOFixedPrepared);
			ret.InclQtyPOFixedReceipts = (short)(t1.InclQtyPOFixedReceipts * t2.InclQtyPOFixedReceipts);
			ret.InclQtySODropShip = (short)(t1.InclQtySODropShip * t2.InclQtySODropShip);
			ret.InclQtyPODropShipOrders = (short)(t1.InclQtyPODropShipOrders * t2.InclQtyPODropShipOrders);
			ret.InclQtyPODropShipPrepared = (short)(t1.InclQtyPODropShipPrepared * t2.InclQtyPODropShipPrepared);
			ret.InclQtyPODropShipReceipts = (short)(t1.InclQtyPODropShipReceipts * t2.InclQtyPODropShipReceipts);
			return ret;
		}
		*/

		public static implicit operator INPlanType(int n)
		{
			return new INPlanType
			{
				InclQtyINIssues = (short)n,
				InclQtyINReceipts = (short)n,
				InclQtyInTransit = (short)n,
                InclQtyInTransitToSO = (short)n,
				InclQtyPOReceipts = (short)n,
				InclQtyPOPrepared = (short)n,
				InclQtyPOOrders = (short)n,
				InclQtySOBackOrdered = (short)n,
				InclQtySOPrepared = (short)n,
				InclQtySOBooked = (short)n,
				InclQtySOShipped = (short)n,
				InclQtySOShipping = (short)n,
				InclQtyINAssemblySupply = (short)n,
				InclQtyINAssemblyDemand = (short)n,
				InclQtyINReplaned = (short)n,
				InclQtySOFixed = (short)n,
				InclQtyPOFixedOrders = (short)n,
				InclQtyPOFixedPrepared = (short)n,
				InclQtyPOFixedReceipts = (short)n,
				InclQtySODropShip = (short)n,
				InclQtyPODropShipOrders = (short)n,
				InclQtyPODropShipPrepared = (short)n,
				InclQtyPODropShipReceipts = (short)n
			};
		}

		public static implicit operator int(INPlanType t)
		{
			return 
				t.InclQtyINIssues > 0 ||
				t.InclQtyINReceipts > 0 ||
				t.InclQtyInTransit > 0 ||
                t.InclQtyInTransitToSO > 0 ||
				t.InclQtyPOReceipts > 0 ||
				t.InclQtyPOPrepared > 0 ||
				t.InclQtyPOOrders > 0 ||
				t.InclQtySOBackOrdered > 0 ||
				t.InclQtySOPrepared > 0 ||
				t.InclQtySOBooked > 0 ||
				t.InclQtySOShipped > 0 ||
				t.InclQtySOShipping > 0 ||
				t.InclQtyINAssemblySupply > 0 ||
				t.InclQtyINAssemblyDemand > 0 ||
				t.InclQtyINReplaned > 0 ||
				t.InclQtySOFixed > 0 ||
				t.InclQtyPOFixedOrders > 0 ||
				t.InclQtyPOFixedPrepared > 0 ||
				t.InclQtyPOFixedReceipts > 0 ||
				t.InclQtySODropShip > 0 ||
				t.InclQtyPODropShipOrders > 0 ||
				t.InclQtyPODropShipPrepared > 0 ||
				t.InclQtyPODropShipReceipts > 0 ? 1 : 
				t.InclQtyINIssues < 0 ||
				t.InclQtyINReceipts < 0 ||
				t.InclQtyInTransit < 0 ||
                t.InclQtyInTransitToSO < 0 ||
                t.InclQtyPOReceipts < 0 ||
				t.InclQtyPOPrepared < 0 ||
				t.InclQtyPOOrders < 0 ||
				t.InclQtySOBackOrdered < 0 ||
				t.InclQtySOPrepared < 0 ||
				t.InclQtySOBooked < 0 ||
				t.InclQtySOShipped < 0 ||
				t.InclQtySOShipping < 0 ||
				t.InclQtyINAssemblySupply < 0 ||
				t.InclQtyINAssemblyDemand < 0 ||
				t.InclQtyINReplaned < 0 ||
				t.InclQtySOFixed < 0 ||
				t.InclQtyPOFixedOrders < 0 ||
				t.InclQtyPOFixedPrepared < 0 ||
				t.InclQtyPOFixedReceipts < 0 ||
				t.InclQtySODropShip < 0 ||
				t.InclQtyPODropShipOrders < 0 ||
				t.InclQtyPODropShipPrepared < 0 ||
				t.InclQtyPODropShipReceipts < 0 ? -1 : 0;
		}

		public static bool operator ==(INPlanType t1, INPlanType t2)
		{
			if (Object.Equals(t1,null) || Object.Equals(t2,null))
			{
				return (Object.Equals(t1, null) && Object.Equals(t2,null));
			}
			else
			{
				return
				(t1.InclQtyINIssues == t2.InclQtyINIssues) &&
				(t1.InclQtyINReceipts == t2.InclQtyINReceipts) &&
				(t1.InclQtyInTransit == t2.InclQtyInTransit) &&
                (t1.InclQtyInTransitToSO == t2.InclQtyInTransitToSO) &&
                (t1.InclQtyPOReceipts == t2.InclQtyPOReceipts) &&
				(t1.InclQtyPOPrepared == t2.InclQtyPOPrepared) &&
				(t1.InclQtyPOOrders == t2.InclQtyPOOrders) &&
				(t1.InclQtySOBackOrdered == t2.InclQtySOBackOrdered) &&
				(t1.InclQtySOPrepared == t2.InclQtySOPrepared) &&
				(t1.InclQtySOBooked == t2.InclQtySOBooked) &&
				(t1.InclQtySOShipped == t2.InclQtySOShipped) &&
				(t1.InclQtySOShipping == t2.InclQtySOShipping) &&
				(t1.InclQtyINAssemblySupply == t2.InclQtyINAssemblySupply) &&
				(t1.InclQtyINAssemblyDemand == t2.InclQtyINAssemblyDemand) &&
				(t1.InclQtySOFixed == t2.InclQtySOFixed) &&
				(t1.InclQtyPOFixedOrders == t2.InclQtyPOFixedOrders) &&
				(t1.InclQtyPOFixedPrepared == t2.InclQtyPOFixedPrepared) &&
				(t1.InclQtyPOFixedReceipts == t2.InclQtyPOFixedReceipts) &&
				(t1.InclQtySODropShip == t2.InclQtySODropShip) &&
				(t1.InclQtyPODropShipOrders == t2.InclQtyPODropShipOrders) &&
				(t1.InclQtyPODropShipPrepared == t2.InclQtyPODropShipPrepared) &&
				(t1.InclQtyPODropShipReceipts == t2.InclQtyPODropShipReceipts);
			}	
		}

		public static bool operator !=(INPlanType t1, INPlanType t2)
		{
			return !(t1 == t2);
		}

		public override bool Equals(object obj)
		{
			return this == (INPlanType)obj;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		#endregion
	}

	public class INPlanConstants
	{
		public const string Plan10 = "10";
		public const string Plan20 = "20";
		public const string Plan40 = "40";
		public const string Plan41 = "41";
		public const string Plan42 = "42";
		public const string Plan43 = "43";
        public const string Plan44 = "44";
        public const string Plan45 = "45";
		public const string Plan50 = "50";
		public const string Plan51 = "51";
		public const string Plan60 = "60";
		public const string Plan61 = "61";
		public const string Plan62 = "62";
		public const string Plan63 = "63";
		public const string Plan64 = "64";
		public const string Plan65 = "65";
		public const string Plan66 = "66";
		public const string Plan68 = "68";
		public const string Plan69 = "69";
		public const string Plan6B = "6B";
		//public const string Plan6T = "6T";
		public const string Plan6D = "6D";
		public const string Plan6E = "6E";
		public const string Plan70 = "70";
		public const string Plan71 = "71";
		public const string Plan72 = "72";
		public const string Plan73 = "73";
		public const string Plan74 = "74";
		public const string Plan75 = "75";
		public const string Plan76 = "76";
		public const string Plan77 = "77";
		public const string Plan78 = "78";
		public const string Plan79 = "79";
		public const string Plan7B = "7B";
		public const string Plan90 = "90";
        public const string Plan93 = "93";
		public const string Plan94 = "94";
		public const string Plan95 = "95";
        public const string Plan96 = "96";


		public class plan10 : Constant<string>
		{
			public plan10() : base(Plan10) { ;}
		}

		public class plan20 : Constant<string>
		{
			public plan20() : base(Plan20) { ;}
		}
		
		public class plan40 : Constant<string>
		{
			public plan40() : base(Plan40) { ;}
		}

		public class plan41 : Constant<string>
		{
			public plan41() : base(Plan41) { ;}
		}

		public class plan42 : Constant<string>
		{
			public plan42() : base(Plan42) { ;}
		}

		public class plan43 : Constant<string>
		{
			public plan43() : base(Plan43) { ;}
		}

        public class plan44 : Constant<string>
        {
            public plan44() : base(Plan44) { ;}
        }

        public class plan45 : Constant<string>
        {
            public plan45() : base(Plan45) { ;}
        }

		public class plan50 : Constant<string>
		{
			public plan50() : base(Plan50) { ;}
		}

		public class plan51 : Constant<string>
		{
			public plan51() : base(Plan51) { ;}
		}

		public class plan60 : Constant<string>
		{
			public plan60() : base(Plan60) { ;}
		}

		public class plan61 : Constant<string>
		{
			public plan61() : base(Plan61) { ;}
		}

		public class plan62 : Constant<string>
		{
			public plan62() : base(Plan62) { ;}
		}

		public class plan63 : Constant<string>
		{
			public plan63() : base(Plan63) { ;}
		}

		public class plan64 : Constant<string>
		{
			public plan64() : base(Plan64) { ;}
		}

		/*
		public class plan65 : Constant<string>
		{
			public plan65() : base(Plan65) { ;}
		}
		*/

		public class plan66 : Constant<string>
		{
			public plan66() : base(Plan66) { ;}
		}

		public class plan68 : Constant<string>
		{
			public plan68() : base(Plan68) { ;}
		}
		public class plan69 : Constant<string>
		{
			public plan69() : base(Plan69) {; }
		}
		public class plan6B : Constant<string>
		{
			public plan6B() : base(Plan6B) { ;}
		}
		public class plan6D : Constant<string>
		{
			public plan6D() : base(Plan6D) { ;}
		}
		public class plan6E : Constant<string>
		{
			public plan6E() : base(Plan6E) { ;}
		}
        /*
		public class plan6T : Constant<string>
		{
			public plan6T() : base(Plan6T) { ;}
		}
        */
		public class plan70 : Constant<string>
		{
			public plan70() : base(Plan70) { ;}
		}

		public class plan71 : Constant<string>
		{
			public plan71() : base(Plan71) { ;}
		}

		public class plan72 : Constant<string>
		{
			public plan72() : base(Plan72) { ;}
		}
		public class plan73 : Constant<string>
		{
			public plan73() : base(Plan73) { ;}
		}
		public class plan74 : Constant<string>
		{
			public plan74() : base(Plan74) { ;}
		}
		public class plan75 : Constant<string>
		{
			public plan75() : base(Plan75) { ;}
		}
		public class plan76 : Constant<string>
		{
			public plan76() : base(Plan76) { ;}
		}
		public class plan77 : Constant<string>
		{
			public plan77() : base(Plan77) { ;}
		}
		public class plan78 : Constant<string>
		{
			public plan78() : base(Plan78) { ;}
		}
		public class plan79 : Constant<string>
		{
			public plan79() : base(Plan79) { ;}
		}
		public class plan7B : Constant<string>
		{
			public plan7B() : base(Plan7B) { ;}
		}		
		public class plan90 : Constant<string>
		{
			public plan90() : base(Plan90) { ;}
		}

        public class plan93 : Constant<string>
        {
            public plan93() : base(Plan93) { ;}
        }
		
		public class plan94 : Constant<string>
		{
			public plan94() : base(Plan94) { ;}
		}

		public class plan95 : Constant<string>
		{
			public plan95() : base(Plan95) { ;}
		}

        public class plan96 : Constant<string>
        {
            public plan96() : base(Plan96) { ;}
        }

        public static bool IsFixed(string PlanType)
		{
			return
				PlanType == Plan64 ||
				PlanType == Plan66 ||
                PlanType == Plan6B ||
                PlanType == Plan6E ||
				PlanType == Plan6D ||
				PlanType == Plan93;
        }

        public static bool IsAllocated(string PlanType)
        {
            return 
                PlanType == Plan61 || 
                PlanType == Plan63;
        }
	}	
}
