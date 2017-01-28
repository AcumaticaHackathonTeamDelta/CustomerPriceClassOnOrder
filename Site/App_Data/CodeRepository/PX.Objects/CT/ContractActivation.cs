using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.IN;
using PX.Objects.CM;
using System.Diagnostics;
using PX.Objects.CR;
using PX.Objects.CS;

namespace PX.Objects.CT
{
	[Serializable]
	[PXCacheName(Messages.ContractUsage)]
	[PXHidden]
	public partial class UsageData:IBqlTable
	{
		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		protected Int32? _BranchID;
		[PXDBInt]
		[PXDefault()]
		public virtual Int32? BranchID
		{
			get
			{
				return this._BranchID;
			}
			set
			{
				this._BranchID = value;
			}
		}
		#endregion
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[PXDBInt]
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
		#region InventoryCD
		public abstract class inventoryCD : PX.Data.IBqlField
		{
		}
		protected String _InventoryCD;
		[PXDBString]
		[PXDefault()]
		[PXUIField(DisplayName = "Item Code")]
		public virtual String InventoryCD
		{
			get
			{
				return this._InventoryCD;
			}
			set
			{
				this._InventoryCD = value;
			}
		}
		#endregion
		#region Description
		public abstract class description : PX.Data.IBqlField
		{
		}
		protected String _Description;
		[PXDBString()]
		[PXDefault()]
		[PXUIField(DisplayName = "Description")]
		public virtual String Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				this._Description = value;
			}
		}
		#endregion
		#region UOM
		public abstract class uOM : PX.Data.IBqlField
		{
		}
		protected String _UOM;
		[PXDBString()]
		[PXDefault()]
		[PXUIField(DisplayName = "UOM")]
		public virtual String UOM
		{
			get
			{
				return this._UOM;
			}
			set
			{
				this._UOM = value;
			}
		}
		#endregion
		#region EmployeeID
		public abstract class employeeID : PX.Data.IBqlField
		{
		}
		protected Int32? _EmployeeID;
		[PXDBInt]
		public virtual Int32? EmployeeID
		{
			get
			{
				return this._EmployeeID;
			}
			set
			{
				this._EmployeeID = value;
			}
		}
		#endregion
		#region Prefix
		public abstract class prefix : PX.Data.IBqlField
		{
		}
		protected String _Prefix;
		[PXDBString()]
		[PXDefault()]
		[PXUIField(DisplayName = "Billing Type")]
		public virtual String Prefix
		{
			get
			{
				return this._Prefix;
			}
			set
			{
				this._Prefix = value;
			}
		}
		#endregion
		#region Qty
		public abstract class qty : PX.Data.IBqlField
		{
		}
		protected Decimal? _Qty;
		[PXDBQuantity]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Included Qty")]
		public virtual Decimal? Qty
		{
			get
			{
				return this._Qty;
			}
			set
			{
				this._Qty = value;
			}
		}
		#endregion
		#region PreciseQty
		public abstract class preciseQty : IBqlField {}
		protected decimal? _PreciseQty;
		[PXDBQuantity]
		public virtual decimal? PreciseQty
		{
			get
			{
				_PreciseQty = _PreciseQty ?? _Qty;
				return this._PreciseQty;
			}
			set
			{
				this._PreciseQty = value;
			}
		}
		#endregion
		#region Proportion
		public abstract class proportion : PX.Data.IBqlField
		{
		}
		protected Decimal? _Proportion;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? Proportion
		{
			get
			{
				return this._Proportion;
			}
			set
			{
				this._Proportion = value;
			}
		}
		#endregion
		#region ExtPrice
		public abstract class extPrice : PX.Data.IBqlField
		{
		}
		protected Decimal? _ExtPrice;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? ExtPrice
		{
			get
			{
				return this._ExtPrice;
			}
			set
			{
				this._ExtPrice = value;
			}
		}
		#endregion
		#region IsFree
		public abstract class isFree : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsFree;
		[PXDBBool]
		[PXDefault]
		public virtual Boolean? IsFree
		{
			get
			{
				return this._IsFree;
			}
			set
			{
				this._IsFree = value;
			}
		}
		#endregion
		#region IsTranData
		public abstract class isTranData : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsTranData;
		[PXDBBool]
		[PXDefault]
		public virtual Boolean? IsTranData
		{
			get
			{
				return this._IsTranData;
			}
			set
			{
				this._IsTranData = value;
			}
		}
		#endregion
		#region IsDollarUsage
		public abstract class isDollarUsage : PX.Data.IBqlField
		{
		}
		[PXDBBool]
		[PXDefault]
		public virtual Boolean? IsDollarUsage
		{
			get;
			set;
		}
		#endregion
		#region PriceOverride
		public abstract class priceOverride : PX.Data.IBqlField
		{
		}
		protected Decimal? _PriceOverride;
		[PXDBDecimal(6)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? PriceOverride
		{
			get
			{
				return this._PriceOverride;
			}
			set
			{
				this._PriceOverride = value;
			}
		}
		#endregion
		#region RefLineNbr
		public abstract class refLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _RefLineNbr;
		[PXDBInt]
		[PXDefault()]
		public virtual Int32? RefLineNbr
		{
			get
			{
				return this._RefLineNbr;
			}
			set
			{
				this._RefLineNbr = value;
			}
		}
		#endregion
		#region TranIDs
		public abstract class tranIDs : PX.Data.IBqlField
		{
		}
		protected List<long?> _TranIDs;
		public virtual List<long?> TranIDs
		{
			get
			{
				return this._TranIDs;
			}
			set
			{
				this._TranIDs = value;
			}
		}
		#endregion
		#region TranDate
		public abstract class tranDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _TranDate;
		[PXDBDate()]
		[PXDefault()]
		public virtual DateTime? TranDate
		{
			get
			{
				return this._TranDate;
			}
			set
			{
				this._TranDate = value;
			}
		}
		#endregion
		#region DiscountID
		public abstract class discountID : PX.Data.IBqlField
		{
		}
		[PXDBString(10, IsUnicode = true)]
		public virtual String DiscountID
		{
			get;
			set;
		}
		#endregion
		#region DiscountSeq
		public abstract class discountSeq : PX.Data.IBqlField
		{
		}
		[PXDBString(10, IsUnicode = true)]
		public virtual String DiscountSeq
		{
			get;
			set;
		}
		#endregion
		#region CaseID
		public abstract class caseID : PX.Data.IBqlField
		{
		}
		protected Int32? _CaseID;
		[PXDBInt]
		public virtual Int32? CaseID
		{
			get
			{
				return this._CaseID;
			}
			set
			{
				this._CaseID = value;
			}
		}
		#endregion

		public UsageData()
		{
			_TranIDs = new List<long?>();
		}
	}
}
