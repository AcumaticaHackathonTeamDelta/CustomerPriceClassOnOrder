namespace PX.Objects.RQ
{
	using System;
	using PX.Data;
	using PX.Objects.GL;
	using PX.Objects.AP;
	using PX.Objects.IN;
	using PX.Objects.CM;
	using PX.Objects.TX;
	using PX.Objects.CR;
	using PX.Objects.CS;
	using PX.Objects.EP;

	[System.SerializableAttribute()]
	public partial class RQRequisitionContent : PX.Data.IBqlTable
	{
		#region ReqNbr
		public abstract class reqNbr : PX.Data.IBqlField
		{
		}
		protected String _ReqNbr;
		[PXDBDefault(typeof(RQRequisition.reqNbr))]
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "")]		
		[PXUIField(DisplayName = "Req. Nbr.", Visibility = PXUIVisibility.Invisible, Visible = false)]
		public virtual String ReqNbr
		{
			get
			{
				return this._ReqNbr;
			}
			set
			{
				this._ReqNbr = value;
			}
		}
		#endregion
		#region ReqLineNbr
		public abstract class reqLineNbr : PX.Data.IBqlField
		{
		}
		protected int? _ReqLineNbr;
		[PXDBInt(IsKey = true)]
		[PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false)]
		[PXParent(typeof(Select<RQRequisitionLine,
			Where<RQRequisitionLine.reqNbr, Equal<Current<RQRequisitionContent.reqNbr>>,
			And<RQRequisitionLine.lineNbr, Equal<Current<RQRequisitionContent.reqLineNbr>>>>>))]
		public virtual int? ReqLineNbr
		{
			get
			{
				return this._ReqLineNbr;
			}
			set
			{
				this._ReqLineNbr = value;
			}
		}
		#endregion
		#region OrderNbr
		public abstract class orderNbr : PX.Data.IBqlField
		{
		}
		protected String _OrderNbr;

		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "")]		
		[PXUIField(DisplayName = "Order Nbr.", Visibility = PXUIVisibility.Invisible, Visible = false)]
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
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected int? _LineNbr;
		[PXDBInt(IsKey = true)]
		[PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false)]
		[PXParent(typeof(Select<RQRequestLine,
							Where<RQRequestLine.orderNbr, Equal<Current<RQRequisitionContent.orderNbr>>,
							And<RQRequestLine.lineNbr, Equal<Current<RQRequisitionContent.lineNbr>>>>>))]
		public virtual int? LineNbr
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
		#region ItemQty
		public abstract class itemQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _ItemQty;
		[PXDBDecimal(2)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty.", Visibility = PXUIVisibility.Visible)]
		[PXFormula(null, typeof(AddCalc<RQRequestLine.reqQty>))]
		[PXFormula(null, typeof(SubCalc<RQRequestLine.openQty>))]
		public virtual Decimal? ItemQty
		{
			get
			{
				return this._ItemQty;
			}
			set
			{
				this._ItemQty = value;
			}
		}
		#endregion
		#region BaseItemQty
		public abstract class baseItemQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseItemQty;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? BaseItemQty
		{
			get
			{
				return this._BaseItemQty;
			}
			set
			{
				this._BaseItemQty = value;
			}
		}
		#endregion
		#region ReqQty
		public abstract class reqQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _ReqQty;
		[PXDBDecimal(2)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty.", Visibility = PXUIVisibility.Visible)]
		[PXFormula(null, typeof(AddCalc<RQRequisitionLine.orderQty>))]
		public virtual Decimal? ReqQty
		{
			get
			{
				return this._ReqQty;
			}
			set
			{
				this._ReqQty = value;
			}
		}
		#endregion
		#region BaseReqQty
		public abstract class baseReqQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BaseReqQty;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? BaseReqQty
		{
			get
			{
				return this._BaseReqQty;
			}
			set
			{
				this._BaseReqQty = value;
			}
		}
		#endregion
		#region RecalcOnly
		public abstract class recalcOnly : PX.Data.IBqlField
		{
		}
		protected bool? _RecalcOnly;
		[PXBool]		
		public virtual bool? RecalcOnly
		{
			get
			{
				return this._RecalcOnly;
			}
			set
			{
				this._RecalcOnly = value;
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
