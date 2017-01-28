namespace PX.Objects.AR
{
	using System;
	using PX.Data;
	using PX.Objects.IN;
	using PX.Objects.CM;
	using PX.Objects.TX;
	using PX.Objects.CS;
	using PX.Objects.GL;

	[System.SerializableAttribute()]
	[PXCacheName(Messages.ARSalesPrice)]
	public partial class ARSalesPrice : PX.Data.IBqlTable
	{		
		#region RecordID
		public abstract class recordID : PX.Data.IBqlField
		{
		}
		protected Int32? _RecordID;
		[PXDBIdentity(IsKey = true)]
		public virtual Int32? RecordID
		{
			get
			{
				return this._RecordID;
			}
			set
			{
				this._RecordID = value;
			}
		}
		#endregion
		#region PriceType
		public abstract class priceType : PX.Data.IBqlField
		{
		}
		protected String _PriceType;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(PriceTypes.CustomerPriceClass)]
		[PriceTypes.List]
		[PXUIField(DisplayName = "Price Type", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String PriceType
		{
			get
			{
				return this._PriceType;
			}
			set
			{
				this._PriceType = value;
			}
		}
		#endregion
		#region PriceCode
		public abstract class priceCode : PX.Data.IBqlField
		{
		}
		protected String _PriceCode;
		[PXString(30, InputMask = ">CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC")]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Price Code", Visibility = PXUIVisibility.SelectorVisible)]
        [PXPriceCodeSelector(typeof(ARSalesPrice.priceCode), new Type[] { typeof(ARSalesPrice.priceCode), typeof(ARSalesPrice.description) }, ValidateValue = false)]
		public virtual String PriceCode
		{
			get
			{
				return this._PriceCode;
			}
			set
			{
				this._PriceCode = value;
			}
		}
		#endregion
        #region CustPriceClassID
        public abstract class custPriceClassID : PX.Data.IBqlField
        {
        }
        protected String _CustPriceClassID;
        [PXDBString(10, InputMask = ">aaaaaaaaaa")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Customer Price Class", Visibility = PXUIVisibility.SelectorVisible)]
        [CustomerPriceClass]
        public virtual String CustPriceClassID
        {
            get
            {
                return this._CustPriceClassID;
            }
            set
            {
                this._CustPriceClassID = value;
            }
        }
        #endregion
        #region CustomerID
        public abstract class customerID : PX.Data.IBqlField
        {
        }
        protected Int32? _CustomerID;
        [Customer(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXParent(typeof(Select<Customer, Where<Customer.bAccountID, Equal<Current<ARSalesPrice.customerID>>>>))]
        public virtual Int32? CustomerID
        {
            get
            {
                return this._CustomerID;
            }
            set
            {
                this._CustomerID = value;
            }
        }
        #endregion
		#region Description
		public abstract class description : PX.Data.IBqlField
		{
		}
		protected String _Description;
		[PXString(256, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.Visible)]
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
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[Inventory(DisplayName = "Inventory ID")]
		//[PXDefault()]
		[PXParent(typeof(Select<InventoryItem, Where<InventoryItem.inventoryID, Equal<Current<ARSalesPrice.inventoryID>>>>))] 
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
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected string _CuryID;
		[PXDBString(5)]
        [PXDefault(typeof(Search<GL.Company.baseCuryID>))]
		[PXSelector(typeof(Currency.curyID), CacheGlobal = true)]
		[PXUIField(DisplayName = "Currency")]
		public virtual string CuryID
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
		#region UOM
		public abstract class uOM : PX.Data.IBqlField
		{
		}
		protected String _UOM;
		[PXDefault(typeof(Search<InventoryItem.salesUnit, Where<InventoryItem.inventoryID, Equal<Current<ARSalesPrice.inventoryID>>>>))]
		[INUnit(typeof(ARSalesPrice.inventoryID))]
		[PXFormula(typeof(Selector<ARSalesPrice.inventoryID, InventoryItem.salesUnit>))]
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
		#region IsPromotionalPrice
		public abstract class isPromotionalPrice : IBqlField
		{
		}
		protected bool? _IsPromotionalPrice;
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Promotion")]
		public virtual bool? IsPromotionalPrice
		{
			get
			{
				return _IsPromotionalPrice;
			}
			set
			{
				_IsPromotionalPrice = value;
			}
		}
		#endregion
		#region EffectiveDate
		public abstract class effectiveDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _EffectiveDate;
		[PXDefault(typeof(AccessInfo.businessDate), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXDBDate()]
		[PXUIField(DisplayName = "Effective Date", Visibility = PXUIVisibility.Visible)]
		public virtual DateTime? EffectiveDate
		{
			get
			{
				return this._EffectiveDate;
			}
			set
			{
				this._EffectiveDate = value;
			}
		}
		#endregion
		#region SalesPrice
		public abstract class salesPrice : PX.Data.IBqlField
		{
		}
		protected Decimal? _SalesPrice;
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBPriceCost]
		[PXUIField(DisplayName = "Price", Visibility = PXUIVisibility.Visible)]
		public virtual Decimal? SalesPrice
		{
			get
			{
				return this._SalesPrice;
			}
			set
			{
				this._SalesPrice = value;
			}
		}
		#endregion
		#region TaxID
		public abstract class taxID : PX.Data.IBqlField
		{
		}
		protected String _TaxID;
		[PXUIField(DisplayName = "Tax", Visibility = PXUIVisibility.Visible)]
		[PXSelector(typeof(Tax.taxID), DescriptionField = typeof(Tax.descr))]
		[PXDBString(Tax.taxID.Length)]
		public virtual String TaxID
		{
			get
			{
				return this._TaxID;
			}
			set
			{
				this._TaxID = value;
			}
		}
		#endregion
		#region BreakQty
		public abstract class breakQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BreakQty;
		[PXDBQuantity(MinValue=0)]
		[PXUIField(DisplayName = "Break Qty", Visibility = PXUIVisibility.Visible)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? BreakQty
		{
			get
			{
				return this._BreakQty;
			}
			set
			{
				this._BreakQty = value;
			}
		}
		#endregion
		#region ExpirationDate
		public abstract class expirationDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ExpirationDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Expiration Date", Visibility = PXUIVisibility.Visible)]
		//[PXFormula(typeof(Switch<Case<Where<ARSalesPrice.isPromotionalPrice, Equal<False>>, Null>, ARSalesPrice.expirationDate>))]
		public virtual DateTime? ExpirationDate
		{
			get
			{
				return this._ExpirationDate;
			}
			set
			{
				this._ExpirationDate = value;
			}
		}
		#endregion


		#region System Columns
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
		#endregion
	}

    [PXHidden]
	public partial class ARSalesPrice2 : ARSalesPrice
	{
        #region CustPriceClassID
        public new abstract class custPriceClassID : PX.Data.IBqlField
        {
        }
        #endregion
        #region InventoryID
        public new abstract class inventoryID : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryID
		public new abstract class curyID : PX.Data.IBqlField
		{
		}
		#endregion
		#region UOM
		public new abstract class uOM : PX.Data.IBqlField
		{
		}
		#endregion
		#region BreakQty
		public new abstract class breakQty : PX.Data.IBqlField
		{
		}
		#endregion
		#region SalesPrice
		public new abstract class salesPrice : PX.Data.IBqlField
		{
		}
		#endregion
	}

	public static class PriceTypes
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
                new string[] { BasePrice, Customer, CustomerPriceClass },
                new string[] { Messages.BasePrice, Messages.Customer, Messages.CustomerPriceClass }) { ; }
		}

		public class ListWithAllAttribute : PXStringListAttribute
		{
			public ListWithAllAttribute()
				: base(
                new string[] { AllPrices, BasePrice, Customer, CustomerPriceClass },
                new string[] { Messages.AllPrices, Messages.BasePrice, Messages.Customer, Messages.CustomerPriceClass }) { ; }
		}
		public const string Customer = "C";
		public const string CustomerPriceClass = "P";
        public const string BasePrice = "B";
		public const string AllPrices = "A";

		public class customer : Constant<string>
		{
			public customer() : base(PriceTypes.Customer) { ;}
		}

		public class customerPriceClass : Constant<string>
		{
			public customerPriceClass() : base(PriceTypes.CustomerPriceClass) { ;}
		}

        public class basePrice : Constant<string>
        {
            public basePrice() : base(PriceTypes.BasePrice) { ;}
        }

		public class allPrices : Constant<string>
		{
			public allPrices() : base(PriceTypes.AllPrices) { ;}
		}
	}
}
