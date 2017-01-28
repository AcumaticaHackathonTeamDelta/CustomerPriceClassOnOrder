using System;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.CM;
using PX.Objects.AR;
using PX.Objects.GL;

namespace PX.Objects.CT
{
	[PXPrimaryGraph(typeof(ContractItemMaint))]
	[Serializable]
	[PXCacheName(Messages.ContractItem)]
	public class ContractItem : IBqlTable
	{
		#region ContractItemID
		public abstract class contractItemID : IBqlField { }
		[PXDBIdentity]
		[PXUIField(Visible=false, Visibility=PXUIVisibility.Invisible)]
		public int? ContractItemID
		{
			get;
			set;
		}
		#endregion
		#region ContractItemCD
		public abstract class contractItemCD : IBqlField { }
		[ContractItem(IsKey=true, Visibility=PXUIVisibility.SelectorVisible)]
		public string ContractItemCD
		{
			get;
			set;
		}
		#endregion
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;
		[PXDBString()]
		[PXDefault(PersistingCheck=PXPersistingCheck.NullOrBlank)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible, Required=true)]
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
		#region DefaultQty
		public abstract class defaultQty : IBqlField { }
		[PXDBQuantity(MinValue = 0)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXFormula(typeof(Validate<ContractItem.maxQty, ContractItem.minQty>))]
		[PXUIField(DisplayName = "Default Quantity")]
		public decimal? DefaultQty
		{
			get;
			set;
		}
		#endregion
		#region MinQty
		public abstract class minQty : IBqlField { }
		[PXDBQuantity(MinValue=0)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Minimum Allowed Quantity")]
		public decimal? MinQty
		{
			get;
			set;
		}
		#endregion
		#region MaxQty
		public abstract class maxQty : IBqlField { }
		[PXDBQuantity(MinValue=0)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Maximum Allowed Quantity")]
		public decimal? MaxQty
		{
			get;
			set;
		}
		#endregion
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXDefault()]
		[PXDBString(5, IsUnicode = true)]
		[PXSelector(typeof(Search<Currency.curyID>))]
		[PXUIField(DisplayName = "Currency ID")]
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
		#region BaseItemID
		public abstract class baseItemID : IBqlField { }
		[PXDBInt()]
		[PXDimensionSelector(InventoryAttribute.DimensionName, 
			typeof(Search5<InventoryItem.inventoryID, 
				LeftJoin<ARSalesPrice, On<ARSalesPrice.inventoryID, Equal<InventoryItem.inventoryID>, 
					And<ARSalesPrice.uOM, Equal<InventoryItem.baseUnit>, 
					And<ARSalesPrice.curyID, Equal<Current<ContractItem.curyID>>, 
					And<ARSalesPrice.priceType, Equal<PriceTypes.basePrice>, 
					And<ARSalesPrice.breakQty, Equal<decimal0>, 
					And<ARSalesPrice.isPromotionalPrice, Equal<False>>>>>>>>, 
				Where<InventoryItem.stkItem, Equal<False>>, 
				Aggregate<GroupBy<InventoryItem.inventoryID, 
					GroupBy<InventoryItem.stkItem>>>>), 
			typeof(InventoryItem.inventoryCD))]
		[PXUIField(DisplayName = "Setup Item")]
		public int? BaseItemID { get; set; }
		#endregion
		#region BasePriceOption
		public abstract class basePriceOption : IBqlField { }
		[PXDBString(1, IsFixed = true)]
		[PriceOption.List]
		[PXUIField(DisplayName = "Setup Pricing")]
		[PXDefault(PriceOption.ItemPrice)]
		public string BasePriceOption
		{
			get;
			set;
		}
		#endregion
		#region BasePrice
		public abstract class basePrice : IBqlField { }
		[PXDBDecimal(6)]
		[PXUIField(DisplayName = "Item Price/Percent")]
		[PXDefault(TypeCode.Decimal, "100.0")]
		public decimal? BasePrice
		{
			get;
			set;
		}
		#endregion
		#region ProrateSetup
		public abstract class prorateSetup : IBqlField { }
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Prorate Setup")]
		public bool? ProrateSetup
		{
			get;
			set;
		}
		#endregion
		#region RetainRate
		public abstract class retainRate : IBqlField { }
		[PXDBDecimal(2)]
		[PXUIField(DisplayName = "Retain Rate")]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXFormula(typeof(Switch<Case<Where<deposit, NotEqual<True>>, decimal0>, retainRate>))]
		[PXUIEnabled(typeof(Where<deposit, Equal<True>>))]
		public decimal? RetainRate { get; set; }
		#endregion
		#region Refundable
		public abstract class refundable : IBqlField { }
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIEnabled(typeof(Where<ContractItem.baseItemID, IsNotNull, Or<ContractItem.renewalItemID, IsNotNull>>))]
		[PXUIField(DisplayName = "Refundable")]
		public bool? Refundable
		{
			get;
			set;
		}
		#endregion
		#region Deposit
		public abstract class deposit : IBqlField { }
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Deposit")]
		public bool? Deposit
		{
			get;
			set;
		}
		#endregion
		#region CollectRenewFeeOnActivation
		public abstract class collectRenewFeeOnActivation : IBqlField { }
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Collect Renewal Fee on Activation")]
		public bool? CollectRenewFeeOnActivation { get; set; }
		#endregion
		#region RenewalItemID
		public abstract class renewalItemID : IBqlField { }
		[PXDBInt]
		[PXDimensionSelector(InventoryAttribute.DimensionName, 
			typeof(Search5<InventoryItem.inventoryID, 
				LeftJoin<ARSalesPrice, On<ARSalesPrice.inventoryID, Equal<InventoryItem.inventoryID>, 
					And<ARSalesPrice.uOM, Equal<InventoryItem.baseUnit>, 
					And<ARSalesPrice.curyID, Equal<Current<ContractItem.curyID>>, 
					And<ARSalesPrice.priceType, Equal<PriceTypes.basePrice>, 
					And<ARSalesPrice.breakQty, Equal<decimal0>,
					And<ARSalesPrice.isPromotionalPrice, Equal<False>>>>>>>>, 
				Where<InventoryItem.stkItem, Equal<False>>, 
				Aggregate<GroupBy<InventoryItem.inventoryID, 
					GroupBy<InventoryItem.stkItem>>>>), 
			typeof(InventoryItem.inventoryCD))]
		[PXUIField(DisplayName = "Renewal Item")]
		public int? RenewalItemID { get; set; }
		#endregion
		#region RenewalPriceOption
		public abstract class renewalPriceOption : IBqlField { }
		[PXDBString(1, IsFixed = true)]
		[RenewalOption.List]
		[PXDefault(PriceOption.BasePercent)]
		[PXUIField(DisplayName = "Renewal Pricing")]
		public string RenewalPriceOption
		{
			get;
			set;
		}
		#endregion
		#region RenewalPrice
		public abstract class renewalPrice : IBqlField { }
		[PXDBDecimal(6)]
		[PXUIField(DisplayName = "Item Price/Percent")]
		[PXDefault(TypeCode.Decimal, "100.0")]
		public decimal? RenewalPrice
		{
			get;
			set;
		}
		#endregion
		#region RecurringType
		public abstract class recurringType : IBqlField { }
		[PXDBString(1, IsFixed = true)]
		[RecurringOption.List]
		[PXDefault(RecurringOption.None)]
		[PXUIField(DisplayName = "Billing Type")]
		public string RecurringType
		{
			get;
			set;
		}
		#endregion
		#region RecurringTypeForDeposits
		public abstract class recurringTypeForDeposits : IBqlField { }
		protected String _RecurringTypeForDeposits;
		[PXString(1, IsFixed = true)]
		[RecurringOption.ListForDeposits]
		[PXUIField(DisplayName = "Billing Type")]
		public string RecurringTypeForDeposits
		{            
			get
			{
				return _RecurringTypeForDeposits;
			}
			set
			{
				_RecurringTypeForDeposits = value;
			}
		}
		#endregion
		#region UOMForDeposits
		public abstract class uOMForDeposits : PX.Data.IBqlField
		{
		}
		protected String _UOMForDeposits;
		[PXUIField(DisplayName = "UOM")]
		[PXString(10, IsFixed = true)]
		public virtual String UOMForDeposits
		{
			get
			{
				return this._UOMForDeposits;
			}
			set
			{
				this._UOMForDeposits = value;
			}
		}
		#endregion
		#region RecurringItemID
		public abstract class recurringItemID : IBqlField { }
		//[NonStockItem(DisplayName = "Recurring Item Code")]
		[PXDefault()]
		[PXDBInt()]
		[PXDimensionSelector(InventoryAttribute.DimensionName, 
			typeof(Search5<InventoryItem.inventoryID, 
				LeftJoin<ARSalesPrice, On<ARSalesPrice.inventoryID, Equal<InventoryItem.inventoryID>, 
					And<ARSalesPrice.uOM, Equal<InventoryItem.baseUnit>, 
					And<ARSalesPrice.curyID, Equal<Current<ContractItem.curyID>>, 
					And<ARSalesPrice.priceType, Equal<PriceTypes.basePrice>, 
					And<ARSalesPrice.breakQty, Equal<decimal0>, 
					And<ARSalesPrice.isPromotionalPrice, Equal<False>>>>>>>>, 
				Where<InventoryItem.stkItem, Equal<False>>, 
				Aggregate<GroupBy<InventoryItem.inventoryID, 
					GroupBy<InventoryItem.stkItem>>>>), 
			typeof(InventoryItem.inventoryCD))]
		[PXUIField(DisplayName = "Recurring Item")]
		public int? RecurringItemID { get; set; }
		#endregion
		#region ResetUsageOnBilling
		public abstract class resetUsageOnBilling : IBqlField { }
		[PXDBBool]
		[PXUIField(DisplayName = "Reset Usage on Billing")]
		[PXDefault(false)]
		public bool? ResetUsageOnBilling { get; set; }
		#endregion
		#region FixedRecurringPriceOption
		public abstract class fixedRecurringPriceOption : IBqlField { }
		[PXDBString(1, IsFixed = true)]
		[FixedRecurringOption.List]
		[PXDefault(PriceOption.ItemPercent)]
		[PXUIField(DisplayName = "Recurring Pricing")]
		public string FixedRecurringPriceOption
		{
			get;
			set;
		}
		#endregion
		#region FixedRecurringPrice
		public abstract class fixedRecurringPrice : IBqlField { }
		[PXDBDecimal(6)]
		[PXUIField(DisplayName = "Item Price/Percent")]
		[PXDefault(TypeCode.Decimal, "100.0")]
		public decimal? FixedRecurringPrice
		{
			get;
			set;
		}
		#endregion
		#region UsagePriceOption
		public abstract class usagePriceOption : IBqlField { }
		[PXDBString(1, IsFixed = true)]
		[UsageOption.List]
		[PXDefault(PriceOption.ItemPrice)]
		[PXUIField(DisplayName = "Extra Usage Pricing")]
		public string UsagePriceOption
		{
			get;
			set;
		}
		#endregion
		#region UsagePrice
		public abstract class usagePrice : IBqlField { }
		[PXDBDecimal(6)]
		[PXUIField(DisplayName = "Item Price/Percent")]
		[PXDefault(TypeCode.Decimal, "100.0")]
		public decimal? UsagePrice
		{
			get;
			set;
		}
		#endregion
		#region DiscontinueAfter
		public abstract class discontinueAfter : IBqlField { }
		[PXDBDate()]
		[PXUIField(DisplayName = "Discontinue After")]
		public DateTime? DiscontinueAfter
		{
			get;
			set;
		}
		#endregion
		#region ReplacementItemID
		public abstract class replacementItemID : IBqlField { }
		[PXDBInt()]
		[PXDimensionSelector(ContractItemAttribute.DimensionName, typeof(Search<ContractItem.contractItemID, Where<ContractItem.contractItemID, NotEqual<Current<ContractItem.contractItemID>>>>), typeof(ContractItem.contractItemCD))]
		[PXRestrictor(typeof(Where<ContractItem.deposit, Equal<Current<ContractItem.deposit>>>), Messages.ContractDoesNotMatchDeposit)]
		[PXUIField(DisplayName = "Replacement Item")]
		public int? ReplacementItemID
		{
			get;
			set;
		}
		#endregion
		#region DepositItemID
		public abstract class depositItemID : IBqlField { }
		[PXDBInt()]
		[PXDimensionSelector(ContractItemAttribute.DimensionName, typeof(Search<ContractItem.contractItemID, Where<ContractItem.contractItemID, NotEqual<Current<ContractItem.contractItemID>>>>), typeof(ContractItem.contractItemCD))]
		[PXRestrictor(typeof(Where<ContractItem.deposit, Equal<True>>), Messages.ContractIsNotDeposit)]
		[PXUIField(DisplayName = "Deposit Item")]
		public int? DepositItemID
		{
			get;
			set;
		}
		#endregion

		#region BasePriceVal
		public abstract class basePriceVal : IBqlField { }
		protected decimal? _BasePriceVal;
		[PXDecimal(6)]
		[PXUIField(DisplayName = "Setup Price", Enabled = false)]
		[PXFormula(typeof(Switch<
			Case<Where<ContractItem.baseItemID, IsNotNull>, 
				Switch<
					Case<Where<ContractItem.basePriceOption, Equal<PriceOption.itemPrice>>, IsNull<NullIf<Selector<ContractItem.baseItemID, ARSalesPrice.salesPrice>, decimal0>, 
						Switch<Case<Where<ContractItem.curyID, Equal<Current<Company.baseCuryID>>>, NullIf<Selector<ContractItem.baseItemID, InventoryItem.basePrice>, decimal0>>, Null>>,
					Case<Where<ContractItem.basePriceOption, Equal<PriceOption.itemPercent>>, Div<Mult<ContractItem.basePrice, IsNull<NullIf<Selector<ContractItem.baseItemID, ARSalesPrice.salesPrice>, decimal0>, Switch<Case<Where<ContractItem.curyID, Equal<Current<Company.baseCuryID>>>, NullIf<Selector<ContractItem.baseItemID, InventoryItem.basePrice>, decimal0>>, Null>>>, decimal100>>>,
					ContractItem.basePrice>>,
			decimal0>))]
		public decimal? BasePriceVal
		{
			get
			{
				return this._BasePriceVal;
			}
			set
			{
				this._BasePriceVal = value;
			}
		}
		#endregion
		#region RenewalPriceVal
		public abstract class renewalPriceVal : IBqlField { }
		protected decimal? _RenewalPriceVal;
		[PXDecimal(6)]
		[PXUIField(DisplayName = "Renewal Price", Enabled = false)]
		[PXFormula(typeof(Switch<
			Case<Where<ContractItem.renewalItemID, IsNotNull>,
				Switch<
					Case<Where<ContractItem.renewalPriceOption, Equal<PriceOption.basePercent>>, Div<Mult<ContractItem.renewalPrice, ContractItem.basePriceVal>, decimal100>,
					Case<Where<ContractItem.renewalPriceOption, Equal<PriceOption.itemPrice>>, IsNull<NullIf<Selector<ContractItem.renewalItemID, ARSalesPrice.salesPrice>, decimal0>, 
						Switch<Case<Where<ContractItem.curyID, Equal<Current<Company.baseCuryID>>>, NullIf<Selector<ContractItem.renewalItemID, InventoryItem.basePrice>, decimal0>>, Null>>,
					Case<Where<ContractItem.renewalPriceOption, Equal<PriceOption.itemPercent>>, Div<Mult<ContractItem.renewalPrice, IsNull<NullIf<Selector<ContractItem.renewalItemID, ARSalesPrice.salesPrice>, decimal0>, Switch<Case<Where<ContractItem.curyID, Equal<Current<Company.baseCuryID>>>, NullIf<Selector<ContractItem.renewalItemID, InventoryItem.basePrice>, decimal0>>, Null>>>, decimal100>>>>,
					ContractItem.renewalPrice>>,
			decimal0>))]
		public decimal? RenewalPriceVal
		{
			get
			{
				return this._RenewalPriceVal;
			}
			set
			{
				this._RenewalPriceVal = value;
			}
		}
		#endregion
		#region FixedRecurringPriceVal
		public abstract class fixedRecurringPriceVal : IBqlField { }
		[PXDecimal(6)]
		[PXUIField(DisplayName = "Recurring Price", Enabled = false)]
		[PXFormula(typeof(Switch<
			Case<Where<ContractItem.recurringItemID, IsNotNull>,
				Switch<
					Case<Where<ContractItem.fixedRecurringPriceOption, Equal<PriceOption.basePercent>>, Div<Mult<ContractItem.fixedRecurringPrice, ContractItem.basePriceVal>, decimal100>,
					Case<Where<ContractItem.fixedRecurringPriceOption, Equal<PriceOption.itemPrice>>, IsNull<NullIf<Selector<ContractItem.recurringItemID, ARSalesPrice.salesPrice>, decimal0>, 
						Switch<Case<Where<ContractItem.curyID, Equal<Current<Company.baseCuryID>>>, NullIf<Selector<ContractItem.recurringItemID, InventoryItem.basePrice>, decimal0>>, Null>>,
					Case<Where<ContractItem.fixedRecurringPriceOption, Equal<PriceOption.itemPercent>>, Div<Mult<ContractItem.fixedRecurringPrice, IsNull<NullIf<Selector<ContractItem.recurringItemID, ARSalesPrice.salesPrice>, decimal0>, Switch<Case<Where<ContractItem.curyID, Equal<Current<Company.baseCuryID>>>, NullIf<Selector<ContractItem.recurringItemID, InventoryItem.basePrice>, decimal0>>, Null>>>, decimal100>>>>,
					ContractItem.fixedRecurringPrice>>,
			decimal0>))]
		public decimal? FixedRecurringPriceVal
		{
			get;
			set;
		}
		#endregion
		#region UsagePriceVal
		public abstract class usagePriceVal : IBqlField { }
		[PXDecimal(6)]
		[PXUIField(DisplayName = "Extra Usage Price", Enabled = false)]
		[PXFormula(typeof(Switch<
				Case<Where<ContractItem.recurringItemID, IsNotNull>,
					Switch<
						Case<Where<ContractItem.usagePriceOption, Equal<PriceOption.basePercent>>, Div<Mult<ContractItem.usagePrice, ContractItem.basePriceVal>, decimal100>,
						Case<Where<ContractItem.usagePriceOption, Equal<PriceOption.itemPrice>>, IsNull<NullIf<Selector<ContractItem.recurringItemID, ARSalesPrice.salesPrice>, decimal0>, 
							Switch<Case<Where<ContractItem.curyID, Equal<Current<Company.baseCuryID>>>, NullIf<Selector<ContractItem.recurringItemID, InventoryItem.basePrice>, decimal0>>, Null>>,
						Case<Where<ContractItem.usagePriceOption, Equal<PriceOption.itemPercent>>, Div<Mult<ContractItem.usagePrice, IsNull<NullIf<Selector<ContractItem.recurringItemID, ARSalesPrice.salesPrice>, decimal0>, Switch<Case<Where<ContractItem.curyID, Equal<Current<Company.baseCuryID>>>, NullIf<Selector<ContractItem.recurringItemID, InventoryItem.basePrice>, decimal0>>, Null>>>, decimal100>>>>,
						ContractItem.usagePrice>>,
				decimal0>))]
		public decimal? UsagePriceVal
		{
			get;
			set;
		}
		#endregion
		
		#region IsBaseValid
		public abstract class isBaseValid : IBqlField { }
		[PXBool]
		[PXDBCalced(typeof(Switch<Case<Where<ContractItem.baseItemID, IsNotNull, And<ContractItem.basePriceOption, Equal<PriceOption.manually>, And<ContractItem.basePrice, Equal<decimal0>>>>, False>, True>), typeof(Boolean))]
		public virtual bool? IsBaseValid
		{
			get;
			set;
		}
		#endregion
		#region IsRenewalValid
		public abstract class isRenewalValid : IBqlField { }
		[PXBool]
		[PXDBCalced(typeof(Switch<Case<Where<ContractItem.renewalItemID, IsNotNull, And<ContractItem.renewalPriceOption, Equal<PriceOption.manually>, And<ContractItem.renewalPrice, Equal<decimal0>>>>, False>, True>), typeof(Boolean))]
		public virtual bool? IsRenewalValid
		{
			get;
			set;
		}
		#endregion
		#region IsFixedRecurringValid
		public abstract class isFixedRecurringValid : IBqlField { }
		[PXBool]
		[PXDBCalced(typeof(Switch<Case<Where<ContractItem.recurringItemID, IsNotNull, And<ContractItem.fixedRecurringPriceOption, Equal<PriceOption.manually>, And<ContractItem.fixedRecurringPrice, Equal<decimal0>>>>, False>, True>), typeof(Boolean))]
		public virtual bool? IsFixedRecurringValid
		{
			get;
			set;
		}
		#endregion
		#region IsUsageValid
		public abstract class isUsageValid : IBqlField { }
		[PXBool]
		[PXDBCalced(typeof(Switch<Case<Where<ContractItem.recurringItemID, IsNotNull, And<ContractItem.usagePriceOption, Equal<PriceOption.manually>, And<ContractItem.usagePrice, Equal<decimal0>>>>, False>, True>), typeof(Boolean))]
		public virtual bool? IsUsageValid
		{
			get;
			set;
		}
		#endregion

		#region NoteID
		public abstract class noteID : IBqlField {}
		[PXNote(DescriptionField = typeof(contractItemCD))]
		public virtual Guid? NoteID { get; set; }
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

	public static class PriceOption
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { ItemPrice, ItemPercent,  Manually },
				new string[] { Messages.ItemPrice, Messages.PercentOfItemPrice, Messages.EnterManually }) { ; }
		}

		public const string ItemPrice = "I";
		public const string ItemPercent = "P";
		public const string Manually = "M";
		public const string BasePercent = "B";


		public class itemPrice : Constant<string>
		{
			public itemPrice() : base(ItemPrice) { ;}
		}

		public class itemPercent : Constant<string>
		{
			public itemPercent() : base(ItemPercent) { ;}
		}

		public class manually : Constant<string>
		{
			public manually() : base(Manually) { ;}
		}

		public class basePercent : Constant<string>
		{
			public basePercent() : base(BasePercent) { ;}
		}
	}

	public static class RenewalOption
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { PriceOption.ItemPrice, PriceOption.ItemPercent, PriceOption.BasePercent, PriceOption.Manually },
				new string[] { Messages.ItemPrice, Messages.PercentOfItemPrice, Messages.PercentOfBasePrice, Messages.EnterManually }) { ; }
		}
	}

	public static class RecurringOption
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { None, Prepay, Usage },
				new string[] { Messages.None, Messages.Prepay, Messages.Usage }) { ; }
		}
		public class ListForDepositsAttribute : PXStringListAttribute
		{
			public ListForDepositsAttribute()
				: base(
				new string[] { None, Prepay, Usage,Deposits },
				new string[] { Messages.None, Messages.Prepay, Messages.Usage,Messages.Deposit }) { ; }
		}

		public const string None = "N";
		public const string Prepay = "P";
		public const string Usage = "U";
		public const string Deposits = "D";

		public class none : Constant<string>
		{
			public none() : base(RecurringOption.None) { ;}
		}

		public class prepay : Constant<string>
		{
			public prepay() : base(RecurringOption.Prepay) { ;}
		}

		public class usage : Constant<string>
		{
			public usage() : base(RecurringOption.Usage) { ;}
		}
		public class deposits : Constant<string>
		{
			public deposits() : base(RecurringOption.Deposits) { ;}
		}
	}

	public static class FixedRecurringOption 
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { PriceOption.ItemPrice, PriceOption.ItemPercent, PriceOption.BasePercent, PriceOption.Manually },
				new string[] { Messages.ItemPrice, Messages.PercentOfItemPrice, Messages.PercentOfBasePrice, Messages.EnterManually }) { ; }
		}


	}

	public static class UsageOption 
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { PriceOption.ItemPrice, PriceOption.ItemPercent, PriceOption.BasePercent, PriceOption.Manually },
				new string[] { Messages.ItemPrice, Messages.PercentOfItemPrice, Messages.PercentOfBasePrice, Messages.EnterManually }) { ; }
		}
	}
}
