namespace PX.Objects.IN
{
	using System;
	using PX.Data;
	using PX.Objects.TX;
	using PX.Objects.CS;
	using PX.Objects.GL;
	using PX.Objects.CM;
	
	[System.SerializableAttribute()]
	[PXCacheName(Messages.INPIDetail)]
	public partial class INPIDetail : PX.Data.IBqlTable
	{

		#region PIID
		public abstract class pIID : PX.Data.IBqlField
		{
		}
		protected String _PIID;
		[PXDBDefault(typeof(INPIHeader.pIID))]
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "")]
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[PXParent(typeof(Select<INPIHeader, Where<INPIHeader.pIID, Equal<Current<INPIDetail.pIID>>>>))]
		public virtual String PIID
		{
			get
			{
				return this._PIID;
			}
			set
			{
				this._PIID = value;
			}
		}
		#endregion
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected int? _LineNbr;
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		[PXLineNbr(typeof(INPIHeader.lineCntr))]
		[PXUIField(DisplayName = "Line Nbr.", Enabled = false)]
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
		#region TagNumber
		public abstract class tagNumber : PX.Data.IBqlField
		{
		}
		protected Int32? _TagNumber;
		[PXDBInt(MinValue = 0)]
		[PXUIField(DisplayName = "Tag Nbr.", Enabled = false)]
		public virtual Int32? TagNumber
		{
			get
			{
				return this._TagNumber;
			}
			set
			{
				this._TagNumber = value;
			}
		}
		#endregion

		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[StockItem(DisplayName="Inventory ID")]		
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
		[PXDefault(typeof(Search<InventoryItem.defaultSubItemID,
			Where<InventoryItem.inventoryID, Equal<Current<INPIDetail.inventoryID>>,
			And<InventoryItem.defaultSubItemOnEntry, Equal<boolTrue>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXFormula(typeof(Default<INTranSplit.inventoryID>))]
		[SubItem(typeof(INPIDetail.inventoryID))]		
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
		#region SiteID
		public abstract class siteID : IBqlField { }
		[PXDefault(typeof(INPIHeader.siteID))]
		[Site()]
		public Int32? SiteID
		{
			get;
			set;
		}
		#endregion
		#region LocationID
		public abstract class locationID : PX.Data.IBqlField
		{
		}
		protected Int32? _LocationID;
		[Location(typeof(INPIDetail.siteID), Visibility = PXUIVisibility.SelectorVisible)]		
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
		#region LotSerialNbr
		public abstract class lotSerialNbr : PX.Data.IBqlField
		{
		}
		protected String _LotSerialNbr;
		//[INLotSerialNbr(typeof(INPIDetail.inventoryID), typeof(INPIDetail.subItemID), typeof(INPIDetail.locationID))]
		[PXDBString(100, IsUnicode = true)]
		[PXUIField(DisplayName = "Lot/Serial Number", Visibility = PXUIVisibility.SelectorVisible)]		
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
		[PXUIField(DisplayName = "Expiration Date")]		
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

		#region BookQty
		public abstract class bookQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BookQty;
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Book Quantity" , Enabled = false)]
		public virtual Decimal? BookQty
		{
			get
			{
				return this._BookQty;
			}
			set
			{
				this._BookQty = value;
			}
		}
		#endregion

		#region PhysicalQty
		public abstract class physicalQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _PhysicalQty;
		[PXDBQuantity(MinValue = 0)]
		[PXUIField(DisplayName = "Physical Quantity")]

		// [PXFormula(null, typeof(SumCalc<INPIHeader.totalPhysicalQty>))]
		//  manually , not via PXFormula because of the problems in INPIReview with double-counting during cost recalculation called from FieldUpdated event

		public virtual Decimal? PhysicalQty
		{
			get
			{
				return this._PhysicalQty;
			}
			set
			{
				this._PhysicalQty = value;
			}
		}
		#endregion
		#region VarQty
		public abstract class varQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _VarQty;
		[PXDBQuantity()]
		[PXUIField(DisplayName = "Variance Quantity", Enabled = false)]

		//[PXFormula(null, typeof(SumCalc<INPIHeader.totalVarQty>))]
		//  manually , not via PXFormula because of the problems in INPIReview with double-counting during cost recalculation called from FieldUpdated event

		public virtual Decimal? VarQty
		{
			get
			{
				return this._VarQty;
			}
			set
			{
				this._VarQty = value;
			}
		}
		#endregion

		#region UnitCost
		public abstract class unitCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnitCost;
		[PXDBPriceCost()]
		//[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Unit Cost", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
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
        #region ReasonCode
        public abstract class reasonCode : PX.Data.IBqlField
        {
        }
        protected String _ReasonCode;
		[PXDBString(CS.ReasonCode.reasonCodeID.Length, IsUnicode = true)]
		[PXSelector(typeof(Search<ReasonCode.reasonCodeID, Where<ReasonCode.usage, Equal<ReasonCodeUsages.adjustment>>>), DescriptionField = typeof(ReasonCode.descr))]
        [PXDefault(typeof(Coalesce<Search2<INPostClass.pIReasonCode, InnerJoin<InventoryItem, On<InventoryItem.postClassID, Equal<INPostClass.postClassID>>>, Where<InventoryItem.inventoryID, Equal<Current<INPIDetail.inventoryID>>, And<INPostClass.pIReasonCode, IsNotNull>>>, Search<INSetup.pIReasonCode>>))]
        [PXFormula(typeof(Default<INPIDetail.inventoryID>))]
        [PXUIField(DisplayName = "Reason Code")]
        public virtual String ReasonCode
        {
            get
            {
                return this._ReasonCode;
            }
            set
            {
                this._ReasonCode = value;
            }
        }
        #endregion
/*
		#region ExtBookCost
		public abstract class extBookCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _ExtBookCost;
		[PXDBBaseCury()]
		//[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Ext. Book Cost", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual Decimal? ExtBookCost
		{
			get
			{
				return this._ExtBookCost;
			}
			set
			{
				this._ExtBookCost = value;
			}
		}
		#endregion
*/
		#region ExtVarCost
		public abstract class extVarCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _ExtVarCost;
		[PXDBBaseCury()]
		[PXUIField(DisplayName = "Ext. Variance Cost", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]

		//[PXFormula(null, typeof(SumCalc<INPIHeader.totalVarCost>))]
		//  manually , not via PXFormula because of the problems in INPIReview with double-counting during cost recalculation called from FieldUpdated event

		public virtual Decimal? ExtVarCost
		{
			get
			{
				return this._ExtVarCost;
			}
			set
			{
				this._ExtVarCost = value;
			}
		}
		#endregion

		#region Status
		public abstract class status : PX.Data.IBqlField
		{
		}
		protected String _Status;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(INPIDetStatus.NotEntered)]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[INPIDetStatus.List()]
		public virtual String Status
		{
			get
			{
				return this._Status;
			}
			set
			{
				this._Status = value;
			}
		}
		#endregion

		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXNote()]
		public virtual Guid? NoteID
		{
			get
			{
				return this._NoteID;
			}
			set
			{
				this._NoteID = value;
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

		#region LineType
		public abstract class lineType : PX.Data.IBqlField
		{
		}
		protected String _LineType;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(INPIDetLineType.UserEntered)]
		[PXUIField(DisplayName = "Line Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[INPIDetLineType.List()]
		public virtual String LineType
		{
			get
			{
				return this._LineType;
			}
			set
			{
				this._LineType = value;
			}
		}
		#endregion

		#region //CSQtyOnHand
		/*
		public abstract class cSQtyOnHand : PX.Data.IBqlField
		{
		}
		protected Decimal? _CSQtyOnHand;
		[PXDBQuantity()]
		[PXUIField(Visibility = PXUIVisibility.Invisible)]
		public virtual Decimal? CSQtyOnHand
		{
			get
			{
				return this._CSQtyOnHand;
			}
			set
			{
				this._CSQtyOnHand = value;
			}
		}
		*/
		#endregion

		#region //CSTotalCost
		/*
		public abstract class cSTotalCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _CSTotalCost;
		[PXDBBaseCury()]
		[PXUIField(Visibility = PXUIVisibility.Invisible)]
		public virtual Decimal? CSTotalCost
		{
			get
			{
				return this._CSTotalCost;
			}
			set
			{
				this._CSTotalCost = value;
			}
		}
		*/
		#endregion

	}



	public class INPIDetStatus
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { NotEntered, Entered, Skipped },
				new string[] { Messages.NotEntered, Messages.Entered, Messages.Skipped }) { ; }
		}

		public const string NotEntered = "N";
		public const string Entered = "E";
		public const string Skipped = "S";

		public class notEntered : Constant<string>
		{
			public notEntered() : base(NotEntered) { ;}
		}

		public class entered : Constant<string>
		{
			public entered() : base(Entered) { ;}
		}

		public class skipped : Constant<string>
		{
			public skipped() : base(Skipped) { ;}
		}
	}


	public class INPIDetLineType
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Normal, Blank, UserEntered },
				new string[] { Messages.Normal, Messages.Blank, Messages.UserEntered }) { ; }
		}

		public const string Normal = "N";
		public const string Blank = "B";
		public const string UserEntered = "U";  // probably this functionality will be implemented in the future

		public class normal : Constant<string>
		{
			public normal() : base(Normal) { ;}
		}

		public class blank : Constant<string>
		{
			public blank() : base(Blank) { ;}
		}

		public class userEntered : Constant<string>
		{
			public userEntered() : base(UserEntered) { ;}
		}
	}




}

