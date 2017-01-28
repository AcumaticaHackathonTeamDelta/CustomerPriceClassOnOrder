using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.CM;
using PX.Objects.CR;

namespace PX.Objects.RQ
{
    [Serializable]
	public partial class RQBidding : IBqlTable
	{
		#region Selected
		public abstract class selected : PX.Data.IBqlField
		{
		}
		protected bool? _Selected;
		[PXBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Selected")]
		public virtual bool? Selected
		{
			[PXDependsOnFields(typeof(orderQty))]
			get
			{
				return _Selected != null ? _Selected : _OrderQty > 0;
			}
			set
			{
				_Selected = value;
			}
		}
		#endregion
		#region LineID
		public abstract class lineID : PX.Data.IBqlField
		{
		}
		protected int? _LineID;
		[PXDBIdentity(IsKey = true)]
		public virtual int? LineID
		{
			get
			{
				return this._LineID;
			}
			set
			{
				this._LineID = value;
			}
		}
		#endregion
		#region ReqNbr
		public abstract class reqNbr : PX.Data.IBqlField
		{
		}
		protected String _ReqNbr;
		[PXDBString(15, IsUnicode = true, InputMask = "")]
		[PXDefault(typeof(RQRequisitionLine.reqNbr))]
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
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected int? _LineNbr;
		[PXDBInt]
		[PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false)]
		[PXParent(typeof(Select<RQRequisitionLine,
			Where<RQRequisitionLine.reqNbr, Equal<Current<RQBidding.reqNbr>>,
			And<RQRequisitionLine.lineNbr, Equal<Current<RQBidding.lineNbr>>>>>))]
		[PXDefault(typeof(RQBiddingState.lineNbr))]
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
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[PXDefault]
		[VendorNonEmployeeActive(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true)]
		public virtual Int32? VendorID
		{
			get
			{
				return this._VendorID;
			}
			set
			{
				this._VendorID = value;
			}
		}
		#endregion
		#region VendorLocationID
		public abstract class vendorLocationID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorLocationID;
		[LocationID(typeof(Where<Location.bAccountID, Equal<Current<RQBidding.vendorID>>>), DescriptionField = typeof(Location.descr), Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefaultValidate(
			typeof(Search<Vendor.defLocationID,Where<Vendor.bAccountID, Equal<Current<RQBidding.vendorID>>>>),
			typeof(Search<RQBidding.reqNbr, 
			Where<RQBidding.reqNbr, Equal<Current<RQBidding.reqNbr>>,
			And<RQBidding.lineNbr,Equal<Current<RQBidding.lineNbr>>,
			And<RQBidding.vendorID, Equal<Current<RQBidding.vendorID>>,
			And<RQBidding.vendorLocationID, Equal<Required<RQBidding.vendorLocationID>>>>>>>))]
		[PXParent(typeof(Select<RQBiddingVendor,
			Where<RQBiddingVendor.reqNbr, Equal<Current<RQBidding.reqNbr>>,
			And<RQBiddingVendor.vendorID, Equal<Current<RQBidding.vendorID>>,
			And<RQBiddingVendor.vendorLocationID, Equal<Current<RQBidding.vendorLocationID>>>>>>))]
		public virtual Int32? VendorLocationID
		{
			get
			{
				return this._VendorLocationID;
			}
			set
			{
				this._VendorLocationID = value;
			}
		}
		#endregion
		#region QuoteNumber
		public abstract class quoteNumber : PX.Data.IBqlField
		{
		}
		protected String _QuoteNumber;
		[PXDBString(20, IsUnicode = true)]
		[PXUIField(DisplayName = "Bid Number")]
		public virtual String QuoteNumber
		{
			get
			{
				return this._QuoteNumber;
			}
			set
			{
				this._QuoteNumber = value;
			}
		}
		#endregion
		#region QuoteQty
		public abstract class quoteQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _QuoteQty;
		[PXDBDecimal(2)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Bid Qty.", Visibility = PXUIVisibility.Visible)]
		[PXFormula(null, typeof(SumCalc<RQBiddingVendor.totalQuoteQty>))]
		public virtual Decimal? QuoteQty
		{
			get
			{
				return this._QuoteQty;
			}
			set
			{
				this._QuoteQty = value;
			}
		}
		#endregion
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXString(5)]
		[PXDefault(typeof(Search<Vendor.curyID, Where<Vendor.bAccountID, Equal<Current<RQBidding.vendorID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Currency", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[PXSelector(typeof(Currency.curyID))]
		public virtual String CuryID
		{
			get
			{
				return this._CuryID;
			}
			set
			{
				this._CuryID = value;
			}
		}
		#endregion
		#region CuryInfoID
		public abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _CuryInfoID;
		[PXDBLong()]
		[CurrencyInfo(typeof(RQBiddingVendor.curyInfoID), ModuleCode = "PO")]		
		public virtual Int64? CuryInfoID
		{
			get
			{
				return this._CuryInfoID;
			}
			set
			{
				this._CuryInfoID = value;
			}
		}
		#endregion
		#region QuoteUnitCost
		public abstract class quoteUnitCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _QuoteUnitCost;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? QuoteUnitCost
		{
			get
			{
				return this._QuoteUnitCost;
			}
			set
			{
				this._QuoteUnitCost = value;
			}
		}
		#endregion
		#region CuryQuoteUnitCost
		public abstract class curyQuoteUnitCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryQuoteUnitCost;

		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(Search<CommonSetup.decPlPrcCst>), typeof(RQBidding.curyInfoID), typeof(RQBidding.quoteUnitCost))]
		[PXUIField(DisplayName = "Bid Unit Cost", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? CuryQuoteUnitCost
		{
			get
			{
				return this._CuryQuoteUnitCost;
			}
			set
			{
				this._CuryQuoteUnitCost = value;
			}
		}
		#endregion
		#region OrderQty
		public abstract class orderQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _OrderQty;
		[PXDBDecimal(2)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Order Qty.", Visibility = PXUIVisibility.Visible)]
		[PXFormula(null, typeof(AddCalc<RQRequisitionLine.biddingQty>))]		
		public virtual Decimal? OrderQty
		{
			get
			{
				return this._OrderQty;
			}
			set
			{
				this._OrderQty = value;
			}
		}
		#endregion
		#region CuryQuoteExtCost
		public abstract class curyQuoteExtCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryQuoteExtCost;
		[PXDBCurrency(typeof(RQBidding.curyInfoID), typeof(RQBidding.quoteExtCost))]
		[PXUIField(DisplayName = "Bid Extended Cost", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[PXFormula(typeof(Mult<RQBidding.quoteQty, RQBidding.curyQuoteUnitCost>))]
		[PXFormula(null, typeof(SumCalc<RQBiddingVendor.curyTotalQuoteExtCost>))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryQuoteExtCost
		{
			get
			{
				return this._CuryQuoteExtCost;
			}
			set
			{
				this._CuryQuoteExtCost = value;
			}
		}
		#endregion
		#region QuoteExtCost
		public abstract class quoteExtCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _QuoteExtCost;

		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? QuoteExtCost
		{
			get
			{
				return this._QuoteExtCost;
			}
			set
			{
				this._QuoteExtCost = value;
			}
		}
		#endregion
		#region MinQty
		public abstract class minQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _MinQty;
		[PXDBDecimal(2)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Min. Qty.", Visibility = PXUIVisibility.Visible)]		
		public virtual Decimal? MinQty
		{
			get
			{
				return this._MinQty;
			}
			set
			{
				this._MinQty = value;
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
	//Add field
    [Serializable]
    [PXHidden]
	public partial class RQBiddingRequisitionLine : RQRequisitionLine
	{				
		#region QuoteCost
		public abstract class quoteCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _QuoteCost;
		[PXDBCalced(typeof(Mult<RQRequisitionLine.orderQty, RQBidding.quoteUnitCost>), typeof(Decimal))]		
		public virtual Decimal? QuoteCost
		{
			get
			{
				return this._QuoteCost;
			}
			set
			{
				this._QuoteCost = value;
			}
		}
		#endregion		
	}	
}
