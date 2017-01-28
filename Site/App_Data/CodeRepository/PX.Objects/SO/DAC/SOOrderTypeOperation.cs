namespace PX.Objects.SO
{
	using System;
	using PX.Data;
	using PX.Objects.AR;
	using PX.Objects.IN;
	using PX.Objects.CS;
	using PX.Objects.GL;	

	[System.SerializableAttribute()]	
	public partial class SOOrderTypeOperation : PX.Data.IBqlTable
	{
		#region OrderType
		public abstract class orderType : PX.Data.IBqlField
		{
		}
		protected String _OrderType;
		[PXDBString(2, IsKey = true, IsFixed = true, InputMask = ">aa")]
		[PXUIField(DisplayName = "Order Type", Visible = false)]
		[PXDefault(typeof(SOOrderType.orderType))]
		[PXParent(typeof(Select<SOOrderType, Where<SOOrderType.orderType, Equal<Current<SOOrderTypeOperation.orderType>>>>))]
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
		#region Operation
		public abstract class operation : PX.Data.IBqlField
		{
		}
		protected String _Operation;
		[PXDBString(1, IsKey = true, IsFixed = true, InputMask = "")]
		[PXUIField(DisplayName = "Operation", Enabled = false)]
		[PXDefault(typeof(SOOrderType.defaultOperation))]
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
							
		#region INDocType
		public abstract class iNDocType : PX.Data.IBqlField
		{
			public const int Length = 3;
		}
		protected String _INDocType;
		[PXDBString(iNDocType.Length, IsFixed = true)]
		[PXDefault()]
		[INTranType.SOList()]
		[PXUIField(DisplayName = "Inventory Transaction Type")]
		public virtual String INDocType
		{
			get
			{
				return this._INDocType;
			}
			set
			{
				this._INDocType = value;
			}
		}
		#endregion
		#region OrderPlanType
		public abstract class orderPlanType : PX.Data.IBqlField
		{
			public const int Length = 2;
		}
		protected String _OrderPlanType;
		[PXDBString(orderPlanType.Length, IsFixed = true, InputMask = ">aa")]
		[PXUIField(DisplayName = "Order Plan Type")]
		[PXSelector(typeof(Search<INPlanType.planType>), DescriptionField = typeof(INPlanType.descr))]
		public virtual String OrderPlanType
		{
			get
			{
				return this._OrderPlanType;
			}
			set
			{
				this._OrderPlanType = value;
			}
		}
		#endregion
		#region ShipmentPlanType
		public abstract class shipmentPlanType : PX.Data.IBqlField
		{
		}
		protected String _ShipmentPlanType;
		[PXDBString(2, IsFixed = true, InputMask = ">aa")]
		[PXUIField(DisplayName = "Shipment Plan Type")]
		[PXSelector(typeof(Search<INPlanType.planType>), DescriptionField = typeof(INPlanType.descr))]
		public virtual String ShipmentPlanType
		{
			get
			{
				return this._ShipmentPlanType;
			}
			set
			{
				this._ShipmentPlanType = value;
			}
		}
		#endregion
		#region AutoCreateIssueLine
		public abstract class autoCreateIssueLine : PX.Data.IBqlField
		{
		}
		protected bool? _AutoCreateIssueLine;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Auto Create Issue Line")]
		public virtual bool? AutoCreateIssueLine
		{
			get
			{
				return this._AutoCreateIssueLine;
			}
			set
			{
				this._AutoCreateIssueLine = value;
			}
		}
		#endregion
		#region Active
		public abstract class active : PX.Data.IBqlField
		{
		}
		protected Boolean? _Active;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Active")]
		public virtual Boolean? Active
		{
			get
			{
				return this._Active;
			}
			set
			{
				this._Active = value;
			}
		}
		#endregion	
		#region RequireReasonCode
		public abstract class requireReasonCode : PX.Data.IBqlField
		{
		}
		protected Boolean? _RequireReasonCode;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Require Reason Code")]
		public virtual Boolean? RequireReasonCode
		{
			get
			{
				return this._RequireReasonCode;
			}
			set
			{
				this._RequireReasonCode = value;
			}
		}
		#endregion	
		#region InvtMult
		public abstract class invtMult : PX.Data.IBqlField
		{
		}
		protected Int16? _InvtMult;
		[PXDBShort()]
		[PXDefault()]
		[PXFormula(typeof(Default<SOOrderTypeOperation.iNDocType>))]
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

	public class SOOperation
	{
		public const string Issue = "I";
		public const string Receipt = "R";

		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(new string[] { Issue, Receipt },
					new string[] { Messages.Issue, Messages.Receipt })
			{				
			}
		}
		public class issue: Constant<string> { public issue():base(Issue){}}
		public class receipt : Constant<string> { public receipt() : base(Receipt) { } }
	}
}
