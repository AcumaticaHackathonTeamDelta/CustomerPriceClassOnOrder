namespace PX.Objects.CS
{
	using System;
	using PX.Data;
	using PX.Objects.CM;
	
	[System.SerializableAttribute()]
	[PXCacheName(Messages.ShipTermsDetail)]
	public partial class ShipTermsDetail : PX.Data.IBqlTable
	{
		#region ShipTermsID
		public abstract class shipTermsID : PX.Data.IBqlField
		{
		}
		protected String _ShipTermsID;
		[PXDBString(10, IsUnicode = true, IsKey = true, InputMask = ">aaaaaaaaaaaaaaa")]
		[PXDefault(typeof(ShipTerms.shipTermsID))]
		public virtual String ShipTermsID
		{
			get
			{
				return this._ShipTermsID;
			}
			set
			{
				this._ShipTermsID = value;
			}
		}
		#endregion
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineNbr;
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		[PXLineNbr(typeof(ShipTerms))]
		[PXParent(typeof(Select<ShipTerms, Where<ShipTerms.shipTermsID, Equal<Current<ShipTermsDetail.shipTermsID>>>>), LeaveChildren = true)]
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
		#region BreakAmount
		public abstract class breakAmount : PX.Data.IBqlField
		{
		}
		protected Decimal? _BreakAmount;
		[PXDBDecimal(2)]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Break Amount")]
		public virtual Decimal? BreakAmount
		{
			get
			{
				return this._BreakAmount;
			}
			set
			{
				this._BreakAmount = value;
			}
		}
		#endregion
		#region FreightCostPercent
		public abstract class freightCostPercent : PX.Data.IBqlField
		{
		}
		protected Decimal? _FreightCostPercent;
		[PXDBDecimal(2)]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Freight Cost %")]
		public virtual Decimal? FreightCostPercent
		{
			get
			{
				return this._FreightCostPercent;
			}
			set
			{
				this._FreightCostPercent = value;
			}
		}
		#endregion
		#region InvoiceAmountPercent
		public abstract class invoiceAmountPercent : PX.Data.IBqlField
		{
		}
		protected Decimal? _InvoiceAmountPercent;
		[PXDBDecimal(2)]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Invoice Amount %")]
		public virtual Decimal? InvoiceAmountPercent
		{
			get
			{
				return this._InvoiceAmountPercent;
			}
			set
			{
				this._InvoiceAmountPercent = value;
			}
		}
		#endregion
		#region ShippingHandling
		public abstract class shippingHandling : PX.Data.IBqlField
		{
		}
		protected Decimal? _ShippingHandling;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Shipping and Handling")]
		public virtual Decimal? ShippingHandling
		{
			get
			{
				return this._ShippingHandling;
			}
			set
			{
				this._ShippingHandling = value;
			}
		}
		#endregion
		#region LineHandling
		public abstract class lineHandling : PX.Data.IBqlField
		{
		}
		protected Decimal? _LineHandling;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Line Handling")]
		public virtual Decimal? LineHandling
		{
			get
			{
				return this._LineHandling;
			}
			set
			{
				this._LineHandling = value;
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
	}
}
