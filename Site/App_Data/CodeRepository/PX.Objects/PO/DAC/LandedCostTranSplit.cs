namespace PX.Objects.PO
{
	using System;
	using PX.Data;
	using PX.Objects.CM;
	using PX.Objects.AP;
	using PX.Objects.CS;
	using PX.Objects.CR;
	using PX.Objects.TX;
	using PX.Objects.GL;

	[System.SerializableAttribute()]
	public partial class LandedCostTranSplit : PX.Data.IBqlTable
	{
		#region LCTranID
		public abstract class lCTranID : PX.Data.IBqlField
		{
		}
		protected Int32? _LCTranID;
		[PXDBInt(IsKey = true)]
		[PXDBLiteDefault(typeof(LandedCostTran.lCTranID))]
		[PXParent(typeof(Select<LandedCostTran, Where<LandedCostTran.lCTranID, Equal<Current<LandedCostTranSplit.lCTranID>>>>))]
		[PXUIField(Enabled = false, Visible = false)]
		public virtual Int32? LCTranID
		{
			get
			{
				return this._LCTranID;
			}
			set
			{
				this._LCTranID = value;
			}
		}
		#endregion
		#region SplitLineNbr
		public abstract class splitLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _SplitLineNbr;
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		[PXLineNbr(typeof(LandedCostTran.lineCntr))]
		public virtual Int32? SplitLineNbr
		{
			get
			{
				return this._SplitLineNbr;
			}
			set
			{
				this._SplitLineNbr = value;
			}
		}
		#endregion
		#region POReceiptNbr
		public abstract class pOReceiptNbr : PX.Data.IBqlField
		{
		}
		protected String _POReceiptNbr;
		[PXDBString(15, IsUnicode = true)]
		[PXSelector(typeof(Search5<POReceipt.receiptNbr, InnerJoin<POReceiptLine, On<POReceiptLine.receiptNbr, Equal<POReceipt.receiptNbr>>>, Where<POReceipt.released, Equal<boolTrue>,
                        And<POReceipt.receiptType, NotEqual<POReceiptType.poreturn>,
                        And2<Where<POReceiptLine.siteID, Equal<Current<LandedCostTran.siteID>>, Or<Current<LandedCostTran.siteID>, IsNull>>,
                        And<Where<POReceiptLine.locationID, Equal<Current<LandedCostTran.locationID>>, Or<Current<LandedCostTran.locationID>, IsNull>>>
						>>>, Aggregate<GroupBy<POReceipt.receiptNbr>>>))]

		[PXUIField(DisplayName = "PO Receipt Nbr.")]

		public virtual String POReceiptNbr
		{
			get
			{
				return this._POReceiptNbr;
			}
			set
			{
				this._POReceiptNbr = value;
			}
		}
		#endregion
		#region POReceiptLineNbr
		public abstract class pOReceiptLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _POReceiptLineNbr;
		[PXDBInt()]
		[PXUIField(DisplayName = "PO Receipt Line Nbr.")]
		public virtual Int32? POReceiptLineNbr
		{
			get
			{
				return this._POReceiptLineNbr;
			}
			set
			{
				this._POReceiptLineNbr = value;
			}
		}
		#endregion
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;

		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description")]
		[PXDefault(typeof(Search<LandedCostCode.descr, Where<LandedCostCode.landedCostCodeID, Equal<Current<LandedCostTran.landedCostCodeID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
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
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[IN.Inventory(typeof(Search2<IN.InventoryItem.inventoryID, 
			InnerJoin<POReceiptLine, On<POReceiptLine.inventoryID, Equal<IN.InventoryItem.inventoryID>>>, 
			Where2<Match<Current<AccessInfo.userName>>, And<POReceiptLine.receiptNbr, Equal<Current<LandedCostTranSplit.pOReceiptNbr>>>>>), typeof(IN.InventoryItem.inventoryCD), typeof(IN.InventoryItem.descr), DisplayName = "Inventory ID")]
		[PXUIField(DisplayName = "Inventory ID")]
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
