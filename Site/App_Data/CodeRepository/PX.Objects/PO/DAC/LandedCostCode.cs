namespace PX.Objects.PO
{
	using System;
	using PX.Data;
	using PX.Objects.AP;
	using PX.Objects.GL;
	using PX.Objects.CS;
	using PX.Objects.CR;
	using PX.Objects.TX;

	
	[System.SerializableAttribute()]
	[PXCacheName(Messages.LandedCostCode)]
	public partial class LandedCostCode : PX.Data.IBqlTable
	{
		#region LandedCostCodeID
		public abstract class landedCostCodeID : PX.Data.IBqlField
		{
		}
		protected String _LandedCostCodeID;
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = ">aaaaaaaaaaaaaaa")]
		[PXDefault()]
		[PXUIField(DisplayName = "Landed Cost Code",Visibility=PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<LandedCostCode.landedCostCodeID>))]
		[PX.Data.EP.PXFieldDescription]
		public virtual String LandedCostCodeID
		{
			get
			{
				return this._LandedCostCodeID;
			}
			set
			{
				this._LandedCostCodeID = value;
			}
		}
		#endregion
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region LCType
		public abstract class lCType : PX.Data.IBqlField
		{
		}
		protected String _LCType;
		[PXDBString(2, IsFixed = true)]
		[PXDefault(LandedCostType.FreightOriginCharges)]
		[LandedCostType.List()]
		[PXUIField(DisplayName = "Type",Visibility=PXUIVisibility.SelectorVisible)]
		public virtual String LCType
		{
			get
			{
				return this._LCType;
			}
			set
			{
				this._LCType = value;
			}
		}
		#endregion
		#region ApplicationMethod
		public abstract class applicationMethod : PX.Data.IBqlField
		{
		}
		protected String _ApplicationMethod;
		[PXDBString(2, IsFixed = true)]
		[PXDefault(LandedCostApplicationMethod.FromBoth)]
		[LandedCostApplicationMethod.List()]
		[PXUIField(DisplayName = "Application Method",Visibility=PXUIVisibility.SelectorVisible)]
		public virtual String ApplicationMethod
		{
			get
			{
				return this._ApplicationMethod;
			}
			set
			{
				this._ApplicationMethod = value;
			}
		}
		#endregion
		#region AllocationMethod
		public abstract class allocationMethod : PX.Data.IBqlField
		{
		}
		protected String _AllocationMethod;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(LandedCostAllocationMethod.ByQuantity)]
		[LandedCostAllocationMethod.List()]

		[PXUIField(DisplayName = "Allocation Method",Visibility=PXUIVisibility.SelectorVisible)]
		public virtual String AllocationMethod
		{
			get
			{
				return this._AllocationMethod;
			}
			set
			{
				this._AllocationMethod = value;
			}
		}
		#endregion
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		//[AP.Vendor(typeof(Where<Where<Vendor.landedCostVendor,Equal<boolTrue>,Or<Vendor.bAccountID,Equal<Current<LandedCostCode.vendorID>>>>>),
		[AP.Vendor(typeof(Search<Vendor.bAccountID, Where<Vendor.landedCostVendor, Equal<boolTrue>>>),
			DisplayName ="Vendor",Visibility=PXUIVisibility.SelectorVisible,DescriptionField=typeof(Vendor.acctName))]
		//[PXDefault()]
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
		[LocationID(typeof(Where<Location.bAccountID, Equal<Current<LandedCostCode.vendorID>>>), DescriptionField = typeof(Location.descr), Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(typeof(Search<Vendor.defLocationID, Where<Vendor.bAccountID, Equal<Current<LandedCostCode.vendorID>>>>),PersistingCheck = PXPersistingCheck.Nothing)]
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
		#region TermsID
		public abstract class termsID : PX.Data.IBqlField
		{
		}
		protected String _TermsID;
		[PXDBString(10, IsUnicode = true)]		
		[PXDefault(typeof(Search<Vendor.termsID, Where<Vendor.bAccountID, Equal<Current<LandedCostCode.vendorID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Terms", Visibility = PXUIVisibility.Visible)]
		[PXSelector(typeof(Search<Terms.termsID, Where<Terms.visibleTo, Equal<TermsVisibleTo.all>, Or<Terms.visibleTo, Equal<TermsVisibleTo.vendor>>>>), DescriptionField = typeof(Terms.descr), Filterable = true)]
		
		public virtual String TermsID
		{
			get
			{
				return this._TermsID;
			}
			set
			{
				this._TermsID = value;
			}
		}
		#endregion
		#region ReasonCode
		public abstract class reasonCode : PX.Data.IBqlField
		{
		}
		protected String _ReasonCode;
		[PXDBString(CS.ReasonCode.reasonCodeID.Length, IsUnicode = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Reason Code")]
		[PXSelector(typeof(Search<ReasonCode.reasonCodeID, Where<ReasonCode.usage, Equal<ReasonCodeUsages.adjustment>>>), DescriptionField = typeof(ReasonCode.descr))]
		
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
		#region LCAccrualAcct
		public abstract class lCAccrualAcct : PX.Data.IBqlField
		{
		}
		protected Int32? _LCAccrualAcct;

		[Account(DisplayName = "Landed Cost Accrual Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		[PXDefault()]
		public virtual Int32? LCAccrualAcct
		{
			get
			{
				return this._LCAccrualAcct;
			}
			set
			{
				this._LCAccrualAcct = value;
			}
		}
		#endregion
		#region LCAccrualSub
		public abstract class lCAccrualSub : PX.Data.IBqlField
		{
		}
		protected Int32? _LCAccrualSub;

		[SubAccount(typeof(LandedCostCode.lCAccrualAcct), DisplayName = "Landed Cost Accrual Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		[PXDefault()]
		public virtual Int32? LCAccrualSub
		{
			get
			{
				return this._LCAccrualSub;
			}
			set
			{
				this._LCAccrualSub = value;
			}
		}
		#endregion
		#region LCVarianceAcct
		public abstract class lCVarianceAcct : PX.Data.IBqlField
		{
		}
		protected Int32? _LCVarianceAcct;

		[Account(DisplayName = "Landed Cost Variance Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		[PXDefault()]
		public virtual Int32? LCVarianceAcct
		{
			get
			{
				return this._LCVarianceAcct;
			}
			set
			{
				this._LCVarianceAcct = value;
			}
		}
		#endregion
		#region LCVarianceSub
		public abstract class lCVarianceSub : PX.Data.IBqlField
		{
		}
		protected Int32? _LCVarianceSub;

		[SubAccount(typeof(LandedCostCode.lCVarianceAcct), DisplayName = "Landed Cost Variance Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		[PXDefault()]
		public virtual Int32? LCVarianceSub
		{
			get
			{
				return this._LCVarianceSub;
			}
			set
			{
				this._LCVarianceSub = value;
			}
		}
		#endregion
		
		#region TaxCategoryID
		public abstract class taxCategoryID : PX.Data.IBqlField
		{
		}
		protected String _TaxCategoryID;
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Tax Category", Visibility = PXUIVisibility.Visible)]
        [PXSelector(typeof(TaxCategory.taxCategoryID), DescriptionField = typeof(TaxCategory.descr))]
        [PXRestrictor(typeof(Where<TaxCategory.active, Equal<True>>), TX.Messages.InactiveTaxCategory, typeof(TaxCategory.taxCategoryID))]
        public virtual String TaxCategoryID
		{
			get
			{
				return this._TaxCategoryID;
			}
			set
			{
				this._TaxCategoryID = value;
			}
		}
		#endregion

		#region NoteID
		public abstract class noteID : IBqlField { }

		[PXNote(DescriptionField = typeof(LandedCostCode.landedCostCodeID),
			Selector = typeof(Search<LandedCostCode.landedCostCodeID>),
			FieldList = new [] { typeof(LandedCostCode.landedCostCodeID), typeof(LandedCostCode.descr), 
				typeof(LandedCostCode.lCType), typeof(LandedCostCode.applicationMethod), 
				typeof(LandedCostCode.allocationMethod), typeof(LandedCostCode.vendorID) })]
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
		#region TStamp
		public abstract class tStamp : PX.Data.IBqlField
		{
		}
		protected Byte[] _TStamp;
		[PXDBTimestamp()]
		public virtual Byte[] TStamp
		{
			get
			{
				return this._TStamp;
			}
			set
			{
				this._TStamp = value;
			}
		}
		#endregion
	}

	public static class LandedCostApplicationMethod
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { FromAP, FromPO, FromBoth},
				new string[] { Messages.FromAP, Messages.FromPO, Messages.FromBoth }) { }
		}


		public const string FromAP = "AP";
		public const string FromPO = "PO";
		public const string FromBoth = "BH";
		
		public class fromAP : Constant<string>
		{
			public fromAP() : base(FromAP) { ;}
		}

		public class fromPO : Constant<string>
		{
			public fromPO() : base(FromPO) { ;}
		}

		public class fromBoth : Constant<string>
		{
			public fromBoth() : base(FromBoth) { ;}
		}
	}

	public static class LandedCostAllocationMethod
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { ByQuantity, ByCost, ByWeight, ByVolume, None },
				new string[] { Messages.ByQuantity, Messages.ByCost, Messages.ByWeight, Messages.ByVolume,Messages.None }) { }
		}

	
		public const string ByQuantity = "Q";
		public const string ByCost = "C";
		public const string ByWeight = "W";
		public const string ByVolume = "V";
		public const string None = "N";




		public class byQuantity : Constant<string>
		{
			public byQuantity() : base(ByQuantity) { ;}
		}

		public class byCost : Constant<string>
		{
			public byCost() : base(ByCost) { ;}
		}

		public class byWeight : Constant<string>
		{
			public byWeight() : base(ByWeight) { ;}
		}

		public class byVolume : Constant<string>
		{
			public byVolume() : base(ByVolume) { ;}
		}
	}

	public static class LandedCostType
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { FreightOriginCharges, CustomDuties, VATTaxes, MiscDestinationCharges,Other },
				new string[] { Messages.FreightOriginCharges, Messages.CustomDuties, Messages.VATTaxes, Messages.MiscDestinationCharges, Messages.Other }) { }
		}


		public const string FreightOriginCharges = "FO";
		public const string CustomDuties = "CD";
		public const string VATTaxes = "VT";
		public const string MiscDestinationCharges = "DC";
		public const string Other = "OR";




		public class freightOriginCharges : Constant<string>
		{
			public freightOriginCharges() : base(FreightOriginCharges) { ;}
		}

		public class customDuties : Constant<string>
		{
			public customDuties() : base(CustomDuties) { ;}
		}

		public class vATTaxes : Constant<string>
		{
			public vATTaxes() : base(VATTaxes) { ;}
		}

		public class miscDestinationCharges : Constant<string>
		{
			public miscDestinationCharges() : base(MiscDestinationCharges) { ;}
		}

		public class other : Constant<string>
		{
			public other() : base(Other) { ;}
		}

	}

}
