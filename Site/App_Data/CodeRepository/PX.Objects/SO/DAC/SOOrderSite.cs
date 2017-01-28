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

	[System.SerializableAttribute()]
	public partial class SOOrderSite : PX.Data.IBqlTable
	{
		#region OrderType
		public abstract class orderType : PX.Data.IBqlField
		{
		}
		protected String _OrderType;
		[PXDBString(2, IsKey = true, IsFixed = true)]
		[PXDBLiteDefault(typeof(SOOrder.orderType))]
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
		[PXDBString(15, IsKey = true, InputMask = "")]
		[PXDBLiteDefault(typeof(SOOrder.orderNbr))]
		[PXParent(typeof(Select<SOOrder, Where<SOOrder.orderType, Equal<Current<SOOrderSite.orderType>>, And<SOOrder.orderNbr, Equal<Current<SOOrderSite.orderNbr>>>>>))]
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
		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SiteID;
		[IN.Site(DisplayName = "Warehouse ID", IsKey = true, DescriptionField = typeof(INSite.descr))]
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
		#region OpenLineCntr
		public abstract class openLineCntr : PX.Data.IBqlField
		{
		}
		protected Int32? _OpenLineCntr;
		[PXDBInt()]
		[PXDefault(0)]
		public virtual Int32? OpenLineCntr
		{
			get
			{
				return this._OpenLineCntr;
			}
			set
			{
				this._OpenLineCntr = value;
			}
		}
		#endregion
		#region ShipmentCntr
		public abstract class shipmentCntr : PX.Data.IBqlField
		{
		}
		protected Int32? _ShipmentCntr;
		[PXDBInt()]
		[PXDefault(0)]
		public virtual Int32? ShipmentCntr
		{
			get
			{
				return this._ShipmentCntr;
			}
			set
			{
				this._ShipmentCntr = value;
			}
		}
		#endregion
		#region OpenShipmentCntr
		public abstract class openShipmentCntr : PX.Data.IBqlField
		{
		}
		protected Int32? _OpenShipmentCntr;
		[PXDBInt()]
		[PXDefault(0)]
		public virtual Int32? OpenShipmentCntr
		{
			get
			{
				return this._OpenShipmentCntr;
			}
			set
			{
				this._OpenShipmentCntr = value;
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
