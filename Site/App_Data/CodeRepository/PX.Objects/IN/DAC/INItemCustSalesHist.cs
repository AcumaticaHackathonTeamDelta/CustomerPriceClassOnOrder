namespace PX.Objects.IN
{
	using System;
	using PX.Data;

	[System.SerializableAttribute()]
    [PXHidden]
	public partial class INItemCustSalesHist : PX.Data.IBqlTable
	{
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[PXDBInt(IsKey = true)]
		[PXDefault()]
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
		#region CostSubItemID
		public abstract class costSubItemID : PX.Data.IBqlField
		{
		}
		protected Int32? _CostSubItemID;
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public virtual Int32? CostSubItemID
		{
			get
			{
				return this._CostSubItemID;
			}
			set
			{
				this._CostSubItemID = value;
			}
		}
		#endregion
		#region CostSiteID
		public abstract class costSiteID : PX.Data.IBqlField
		{
		}
		protected Int32? _CostSiteID;
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public virtual Int32? CostSiteID
		{
			get
			{
				return this._CostSiteID;
			}
			set
			{
				this._CostSiteID = value;
			}
		}
		#endregion
		#region BAccountID
		public abstract class bAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _BAccountID;
		[PXDBInt(IsKey = true)]
		[PXDefault()]
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
		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected String _FinPeriodID;
		[GL.FinPeriodID(IsKey = true)]
		[PXDefault()]
		public virtual String FinPeriodID
		{
			get
			{
				return this._FinPeriodID;
			}
			set
			{
				this._FinPeriodID = value;
			}
		}
		#endregion
		#region FinPtdCOGS
		public abstract class finPtdCOGS : PX.Data.IBqlField
		{
		}
		protected Decimal? _FinPtdCOGS;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? FinPtdCOGS
		{
			get
			{
				return this._FinPtdCOGS;
			}
			set
			{
				this._FinPtdCOGS = value;
			}
		}
		#endregion
		#region FinPtdCOGSCredits
		public abstract class finPtdCOGSCredits : PX.Data.IBqlField
		{
		}
		protected Decimal? _FinPtdCOGSCredits;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? FinPtdCOGSCredits
		{
			get
			{
				return this._FinPtdCOGSCredits;
			}
			set
			{
				this._FinPtdCOGSCredits = value;
			}
		}
		#endregion
		#region FinPtdCOGSDropShips
		public abstract class finPtdCOGSDropShips : PX.Data.IBqlField
		{
		}
		protected Decimal? _FinPtdCOGSDropShips;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? FinPtdCOGSDropShips
		{
			get
			{
				return this._FinPtdCOGSDropShips;
			}
			set
			{
				this._FinPtdCOGSDropShips = value;
			}
		}
		#endregion
		#region FinPtdQtySales
		public abstract class finPtdQtySales : PX.Data.IBqlField
		{
		}
		protected Decimal? _FinPtdQtySales;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? FinPtdQtySales
		{
			get
			{
				return this._FinPtdQtySales;
			}
			set
			{
				this._FinPtdQtySales = value;
			}
		}
		#endregion
		#region FinPtdQtyCreditMemos
		public abstract class finPtdQtyCreditMemos : PX.Data.IBqlField
		{
		}
		protected Decimal? _FinPtdQtyCreditMemos;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? FinPtdQtyCreditMemos
		{
			get
			{
				return this._FinPtdQtyCreditMemos;
			}
			set
			{
				this._FinPtdQtyCreditMemos = value;
			}
		}
		#endregion
		#region FinPtdQtyDropShipSales
		public abstract class finPtdQtyDropShipSales : PX.Data.IBqlField
		{
		}
		protected Decimal? _FinPtdQtyDropShipSales;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? FinPtdQtyDropShipSales
		{
			get
			{
				return this._FinPtdQtyDropShipSales;
			}
			set
			{
				this._FinPtdQtyDropShipSales = value;
			}
		}
		#endregion
		#region FinPtdSales
		public abstract class finPtdSales : PX.Data.IBqlField
		{
		}
		protected Decimal? _FinPtdSales;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? FinPtdSales
		{
			get
			{
				return this._FinPtdSales;
			}
			set
			{
				this._FinPtdSales = value;
			}
		}
		#endregion
		#region FinPtdCreditMemos
		public abstract class finPtdCreditMemos : PX.Data.IBqlField
		{
		}
		protected Decimal? _FinPtdCreditMemos;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? FinPtdCreditMemos
		{
			get
			{
				return this._FinPtdCreditMemos;
			}
			set
			{
				this._FinPtdCreditMemos = value;
			}
		}
		#endregion
		#region FinPtdDropShipSales
		public abstract class finPtdDropShipSales : PX.Data.IBqlField
		{
		}
		protected Decimal? _FinPtdDropShipSales;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? FinPtdDropShipSales
		{
			get
			{
				return this._FinPtdDropShipSales;
			}
			set
			{
				this._FinPtdDropShipSales = value;
			}
		}
		#endregion
		#region FinYtdCOGS
		public abstract class finYtdCOGS : PX.Data.IBqlField
		{
		}
		protected Decimal? _FinYtdCOGS;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? FinYtdCOGS
		{
			get
			{
				return this._FinYtdCOGS;
			}
			set
			{
				this._FinYtdCOGS = value;
			}
		}
		#endregion
		#region FinYtdCOGSCredits
		public abstract class finYtdCOGSCredits : PX.Data.IBqlField
		{
		}
		protected Decimal? _FinYtdCOGSCredits;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? FinYtdCOGSCredits
		{
			get
			{
				return this._FinYtdCOGSCredits;
			}
			set
			{
				this._FinYtdCOGSCredits = value;
			}
		}
		#endregion
		#region FinYtdCOGSDropShips
		public abstract class finYtdCOGSDropShips : PX.Data.IBqlField
		{
		}
		protected Decimal? _FinYtdCOGSDropShips;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? FinYtdCOGSDropShips
		{
			get
			{
				return this._FinYtdCOGSDropShips;
			}
			set
			{
				this._FinYtdCOGSDropShips = value;
			}
		}
		#endregion
		#region FinYtdQtySales
		public abstract class finYtdQtySales : PX.Data.IBqlField
		{
		}
		protected Decimal? _FinYtdQtySales;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? FinYtdQtySales
		{
			get
			{
				return this._FinYtdQtySales;
			}
			set
			{
				this._FinYtdQtySales = value;
			}
		}
		#endregion
		#region FinYtdQtyCreditMemos
		public abstract class finYtdQtyCreditMemos : PX.Data.IBqlField
		{
		}
		protected Decimal? _FinYtdQtyCreditMemos;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? FinYtdQtyCreditMemos
		{
			get
			{
				return this._FinYtdQtyCreditMemos;
			}
			set
			{
				this._FinYtdQtyCreditMemos = value;
			}
		}
		#endregion
		#region FinYtdQtyDropShipSales
		public abstract class finYtdQtyDropShipSales : PX.Data.IBqlField
		{
		}
		protected Decimal? _FinYtdQtyDropShipSales;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? FinYtdQtyDropShipSales
		{
			get
			{
				return this._FinYtdQtyDropShipSales;
			}
			set
			{
				this._FinYtdQtyDropShipSales = value;
			}
		}
		#endregion
		#region FinYtdSales
		public abstract class finYtdSales : PX.Data.IBqlField
		{
		}
		protected Decimal? _FinYtdSales;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? FinYtdSales
		{
			get
			{
				return this._FinYtdSales;
			}
			set
			{
				this._FinYtdSales = value;
			}
		}
		#endregion
		#region FinYtdCreditMemos
		public abstract class finYtdCreditMemos : PX.Data.IBqlField
		{
		}
		protected Decimal? _FinYtdCreditMemos;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? FinYtdCreditMemos
		{
			get
			{
				return this._FinYtdCreditMemos;
			}
			set
			{
				this._FinYtdCreditMemos = value;
			}
		}
		#endregion
		#region FinYtdDropShipSales
		public abstract class finYtdDropShipSales : PX.Data.IBqlField
		{
		}
		protected Decimal? _FinYtdDropShipSales;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? FinYtdDropShipSales
		{
			get
			{
				return this._FinYtdDropShipSales;
			}
			set
			{
				this._FinYtdDropShipSales = value;
			}
		}
		#endregion
		#region TranPtdCOGS
		public abstract class tranPtdCOGS : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranPtdCOGS;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TranPtdCOGS
		{
			get
			{
				return this._TranPtdCOGS;
			}
			set
			{
				this._TranPtdCOGS = value;
			}
		}
		#endregion
		#region TranPtdCOGSCredits
		public abstract class tranPtdCOGSCredits : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranPtdCOGSCredits;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TranPtdCOGSCredits
		{
			get
			{
				return this._TranPtdCOGSCredits;
			}
			set
			{
				this._TranPtdCOGSCredits = value;
			}
		}
		#endregion
		#region TranPtdCOGSDropShips
		public abstract class tranPtdCOGSDropShips : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranPtdCOGSDropShips;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TranPtdCOGSDropShips
		{
			get
			{
				return this._TranPtdCOGSDropShips;
			}
			set
			{
				this._TranPtdCOGSDropShips = value;
			}
		}
		#endregion
		#region TranPtdQtySales
		public abstract class tranPtdQtySales : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranPtdQtySales;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TranPtdQtySales
		{
			get
			{
				return this._TranPtdQtySales;
			}
			set
			{
				this._TranPtdQtySales = value;
			}
		}
		#endregion
		#region TranPtdQtyCreditMemos
		public abstract class tranPtdQtyCreditMemos : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranPtdQtyCreditMemos;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TranPtdQtyCreditMemos
		{
			get
			{
				return this._TranPtdQtyCreditMemos;
			}
			set
			{
				this._TranPtdQtyCreditMemos = value;
			}
		}
		#endregion
		#region TranPtdQtyDropShipSales
		public abstract class tranPtdQtyDropShipSales : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranPtdQtyDropShipSales;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TranPtdQtyDropShipSales
		{
			get
			{
				return this._TranPtdQtyDropShipSales;
			}
			set
			{
				this._TranPtdQtyDropShipSales = value;
			}
		}
		#endregion
		#region TranPtdSales
		public abstract class tranPtdSales : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranPtdSales;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TranPtdSales
		{
			get
			{
				return this._TranPtdSales;
			}
			set
			{
				this._TranPtdSales = value;
			}
		}
		#endregion
		#region TranPtdCreditMemos
		public abstract class tranPtdCreditMemos : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranPtdCreditMemos;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TranPtdCreditMemos
		{
			get
			{
				return this._TranPtdCreditMemos;
			}
			set
			{
				this._TranPtdCreditMemos = value;
			}
		}
		#endregion
		#region TranPtdDropShipSales
		public abstract class tranPtdDropShipSales : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranPtdDropShipSales;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TranPtdDropShipSales
		{
			get
			{
				return this._TranPtdDropShipSales;
			}
			set
			{
				this._TranPtdDropShipSales = value;
			}
		}
		#endregion
		#region TranYtdCOGS
		public abstract class tranYtdCOGS : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranYtdCOGS;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TranYtdCOGS
		{
			get
			{
				return this._TranYtdCOGS;
			}
			set
			{
				this._TranYtdCOGS = value;
			}
		}
		#endregion
		#region TranYtdCOGSCredits
		public abstract class tranYtdCOGSCredits : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranYtdCOGSCredits;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TranYtdCOGSCredits
		{
			get
			{
				return this._TranYtdCOGSCredits;
			}
			set
			{
				this._TranYtdCOGSCredits = value;
			}
		}
		#endregion
		#region TranYtdCOGSDropShips
		public abstract class tranYtdCOGSDropShips : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranYtdCOGSDropShips;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TranYtdCOGSDropShips
		{
			get
			{
				return this._TranYtdCOGSDropShips;
			}
			set
			{
				this._TranYtdCOGSDropShips = value;
			}
		}
		#endregion
		#region TranYtdQtySales
		public abstract class tranYtdQtySales : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranYtdQtySales;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TranYtdQtySales
		{
			get
			{
				return this._TranYtdQtySales;
			}
			set
			{
				this._TranYtdQtySales = value;
			}
		}
		#endregion
		#region TranYtdQtyCreditMemos
		public abstract class tranYtdQtyCreditMemos : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranYtdQtyCreditMemos;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TranYtdQtyCreditMemos
		{
			get
			{
				return this._TranYtdQtyCreditMemos;
			}
			set
			{
				this._TranYtdQtyCreditMemos = value;
			}
		}
		#endregion
		#region TranYtdQtyDropShipSales
		public abstract class tranYtdQtyDropShipSales : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranYtdQtyDropShipSales;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TranYtdQtyDropShipSales
		{
			get
			{
				return this._TranYtdQtyDropShipSales;
			}
			set
			{
				this._TranYtdQtyDropShipSales = value;
			}
		}
		#endregion
		#region TranYtdSales
		public abstract class tranYtdSales : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranYtdSales;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TranYtdSales
		{
			get
			{
				return this._TranYtdSales;
			}
			set
			{
				this._TranYtdSales = value;
			}
		}
		#endregion
		#region TranYtdCreditMemos
		public abstract class tranYtdCreditMemos : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranYtdCreditMemos;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TranYtdCreditMemos
		{
			get
			{
				return this._TranYtdCreditMemos;
			}
			set
			{
				this._TranYtdCreditMemos = value;
			}
		}
		#endregion
		#region TranYtdDropShipSales
		public abstract class tranYtdDropShipSales : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranYtdDropShipSales;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TranYtdDropShipSales
		{
			get
			{
				return this._TranYtdDropShipSales;
			}
			set
			{
				this._TranYtdDropShipSales = value;
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
