namespace PX.Objects.TX
{
	using System;
	using PX.Data;
	using PX.Objects.GL;
	using PX.Objects.AP;
	using PX.Objects.BQLConstants;
	using PX.Objects.CS;

	public static class CSTaxType
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Sales, Use, VAT, Withholding },
				new string[] { Messages.Sales, Messages.Use, Messages.VAT, Messages.Withholding }) { }
		}

		public class ListSimpleAttribute : PXStringListAttribute
		{
			public ListSimpleAttribute()
				: base(
				new string[] { Sales, Use, Withholding },
				new string[] { Messages.Sales, Messages.Use, Messages.Withholding }) { }
		}

		public const string Sales = CSTaxBucketType.Sales;
		public const string Use = CSTaxBucketType.Purchase;
		public const string VAT = "V";
		public const string Withholding = "W";

		public class sales : Constant<string>
		{
			public sales() : base(Sales) { ;}
		}

		public class use : Constant<string>
		{
			public use() : base(Use) { ;}
		}

		public class vat : Constant<string>
		{
			public vat() : base(VAT) { ;}
		}

		public class withholding : Constant<string>
		{
			public withholding() : base(Withholding) { ;}
		}
	}
	
	public static class CSTaxCalcType
	{
		public const string Item = "I";
		public const string Doc = "D";

		public class item : Constant<string>
		{
			public item() : base(Item) { }
		}

		public class doc : Constant<string>
		{
			public doc() : base(Doc) { }
		}
	}

	public static class CSTaxCalcLevel
	{
		public const string Inclusive = "0";
		public const string CalcOnItemAmt = "1";
		public const string CalcOnItemAmtPlusTaxAmt = "2";

		public class inclusive : Constant<string>
		{
			public inclusive() : base(Inclusive) { ;}
		}

		public class calcOnItemAmt : Constant<string>
		{
			public calcOnItemAmt() : base(CalcOnItemAmt) { ;}
		}

		public class calcOnItemAmtPlusTaxAmt : Constant<string>
		{
			public calcOnItemAmtPlusTaxAmt() : base(CalcOnItemAmtPlusTaxAmt) { ;}
		}
	}

	public static class CSTaxTermsDiscount
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				:base(new string[] { ToTaxableAmount, ToPromtPayment, NoAdjust },
							new string[] { Messages.DiscountToTaxableAmount, Messages.DiscountToPromtPayment, Messages.DiscountToTotalAmount }) 
			{
			}
		}

		public const string ToTaxableAmount = "X";
		public const string ToPromtPayment = "P";
		public const string ToTaxAmount = "T";
		public const string AdjustTax = "A";
		public const string NoAdjust = "N";

		public class toPromtPayment : Constant<string>
		{
			public toPromtPayment() : base(ToPromtPayment) { ;}
		}
	}

	[System.SerializableAttribute()]
	[PXPrimaryGraph(typeof(SalesTaxMaint))]
	[PXCacheName(Messages.Tax)]
	public partial class Tax : PX.Data.IBqlTable
	{
		#region TaxID
		public abstract class taxID : PX.Data.IBqlField
		{
			/// <summary>
			/// 30
			/// </summary>
			public const int Length = 30;
		}
		protected String _TaxID;
		[PXDBString(Tax.taxID.Length, IsUnicode = true, IsKey = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
		[PXUIField(DisplayName = "Tax ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Tax.taxID), DescriptionField = typeof(Tax.descr), CacheGlobal = true)]
		[PX.Data.EP.PXFieldDescription]
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
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;
		[PXDBLocalizableString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		[PX.Data.EP.PXFieldDescription]
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
		#region TaxType
		public abstract class taxType : PX.Data.IBqlField
		{
		}
		protected String _TaxType;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(CSTaxType.Sales)]
		[CSTaxType.List()]
		[PXUIField(DisplayName = "Tax Type", Visibility=PXUIVisibility.Visible)]
		public virtual String TaxType
		{
			get
			{
				return this._TaxType;
			}
			set
			{
				this._TaxType = value;
			}
		}
		#endregion
		#region TaxCalcType
		public abstract class taxCalcType : PX.Data.IBqlField
		{
		}
		protected String _TaxCalcType;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(CSTaxCalcType.Doc)]
		[PXStringList(new string[] { CSTaxCalcType.Doc, CSTaxCalcType.Item }, new string[] { "Document", "Item" })]
		public virtual String TaxCalcType
		{
			get
			{
				return this._TaxCalcType;
			}
			set
			{
				this._TaxCalcType = value;
			}
		}
		#endregion
		#region TaxCalcLevel
		public abstract class taxCalcLevel : PX.Data.IBqlField
		{
		}
		protected String _TaxCalcLevel;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(CSTaxCalcLevel.CalcOnItemAmt)]
		[PXStringList(new string[] { CSTaxCalcLevel.Inclusive, CSTaxCalcLevel.CalcOnItemAmt, CSTaxCalcLevel.CalcOnItemAmtPlusTaxAmt },
					new string[] { "Inclusive", "Calc. On Item Amount", "Calc. On Item Amt + Tax Amt" })]
		public virtual String TaxCalcLevel
		{
			get
			{
				return this._TaxCalcLevel;
			}
			set
			{
				this._TaxCalcLevel = value;
			}
		}
		#endregion
		#region TaxCalcRule
		public abstract class taxCalcRule : PX.Data.IBqlField
		{
		}
		[PXString(2, IsFixed = true)]
		[PXStringList(new string[] 
			{ 
				CSTaxCalcType.Item + CSTaxCalcLevel.Inclusive, 
				CSTaxCalcType.Item + CSTaxCalcLevel.CalcOnItemAmt, 
				CSTaxCalcType.Item + CSTaxCalcLevel.CalcOnItemAmtPlusTaxAmt, 
				CSTaxCalcType.Doc + CSTaxCalcLevel.CalcOnItemAmt, 
				CSTaxCalcType.Doc + CSTaxCalcLevel.CalcOnItemAmtPlusTaxAmt
			}, new string[] 
			{ 
				"Extract From Item Amount", 
				"Calc. On Item Amount" ,
				"Calc. On Item + Tax Amount",
				"Calc. On Document Amount",
				"Calc. On Document + Tax Amount"
			})]
		[PXUIField(DisplayName = "Calculate On")]
		public virtual String TaxCalcRule
		{
			[PXDependsOnFields(typeof(taxCalcType),typeof(taxCalcLevel))]
			get
			{
				return this._TaxCalcType + this._TaxCalcLevel;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this._TaxCalcType = null;
					this._TaxCalcLevel = null;
				}
				else
				{
					this._TaxCalcType = value.Substring(0, 1);
					this._TaxCalcLevel = value.Substring(1, 1);
				}
			}
		}
		#endregion
		#region TaxCalcLevel2Exclude
		public abstract class taxCalcLevel2Exclude : PX.Data.IBqlField
		{
		}
		protected Boolean? _TaxCalcLevel2Exclude;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Exclude from Tax-on-Tax Calculation")]
		public virtual Boolean? TaxCalcLevel2Exclude
		{
			get
			{
				return this._TaxCalcLevel2Exclude;
			}
			set
			{
				this._TaxCalcLevel2Exclude = value;
			}
		}
		#endregion
		#region TaxApplyTermsDisc
		public abstract class taxApplyTermsDisc : PX.Data.IBqlField
		{
		}
		protected String _TaxApplyTermsDisc;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(CSTaxTermsDiscount.ToTaxableAmount)]
		[CSTaxTermsDiscount.List()]
		[PXUIField(DisplayName = "Cash Discount")]
		public virtual String TaxApplyTermsDisc
		{
			get
			{
				return this._TaxApplyTermsDisc;
			}
			set
			{
				this._TaxApplyTermsDisc = value;
			}
		}
		#endregion
		#region PendingTax
		public abstract class pendingTax : PX.Data.IBqlField
		{
		}
		protected Boolean? _PendingTax;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Pending VAT")]
		public virtual Boolean? PendingTax
		{
			get
			{
				return this._PendingTax;
			}
			set
			{
				this._PendingTax = value;
			}
		}
		#endregion
		#region ReverseTax
		public abstract class reverseTax : PX.Data.IBqlField
		{
		}
		protected Boolean? _ReverseTax;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Reverse VAT")]
		public virtual Boolean? ReverseTax
		{
			get
			{
				return this._ReverseTax;
			}
			set
			{
				this._ReverseTax = value;
			}
		}
		#endregion
		#region IncludeInTaxable
		public abstract class includeInTaxable : PX.Data.IBqlField
		{
		}
		protected Boolean? _IncludeInTaxable;
		[PXDBBool()]
		[PXDefault(false)]
        [PXUIField(DisplayName = "Include in VAT Taxable Total")]
		public virtual Boolean? IncludeInTaxable
		{
			get
			{
				return this._IncludeInTaxable;
			}
			set
			{
				this._IncludeInTaxable = value;
			}
		}
		#endregion
		#region ExemptTax
		public abstract class exemptTax : PX.Data.IBqlField
        {
        }
        protected Boolean? _ExemptTax;
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Include in VAT Exempt Total")]
        public virtual Boolean? ExemptTax
        {
            get
            {
                return this._ExemptTax;
            }
            set
            {
                this._ExemptTax = value;
            }
        }
        #endregion

        #region StatisticalTax
        public abstract class statisticalTax : PX.Data.IBqlField
        {
        }
        protected Boolean? _StatisticalTax;
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Statistical VAT")]
        public virtual Boolean? StatisticalTax
        {
            get
            {
                return this._StatisticalTax;
            }
            set
            {
                this._StatisticalTax = value;
            }
        }
        #endregion
		#region DirectTax
		public abstract class directTax : PX.Data.IBqlField
		{
		}
		protected Boolean? _DirectTax;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Enter from Tax Bill")]
		public virtual Boolean? DirectTax
		{
			get
			{
				return this._DirectTax;
			}
			set
			{
				this._DirectTax = value;
			}
		}
		#endregion

		#region DeductibleVAT
		public abstract class deductibleVAT : PX.Data.IBqlField
		{
		}
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Deductible VAT")]
		public virtual Boolean? DeductibleVAT { get; set; }
		#endregion

		#region ReportExpenseToSingleAccount
		public abstract class reportExpenseToSingleAccount : PX.Data.IBqlField
		{
		}
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Use Tax Expense Account", Visible = false)]
		public virtual bool? ReportExpenseToSingleAccount
		{
			get; set;
		}
		#endregion

		#region TaxVendorID
		public abstract class taxVendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _TaxVendorID;
		[Vendor(typeof(Search<Vendor.bAccountID, Where<Vendor.taxAgency, Equal<True>>>), DisplayName = "Tax Agency ID", DescriptionField=typeof(Vendor.acctName))]
		public virtual Int32? TaxVendorID
		{
			get
			{
				return this._TaxVendorID;
			}
			set
			{
				this._TaxVendorID = value;
			}
		}
		#endregion
		#region SalesTaxAcctID
		public abstract class salesTaxAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _SalesTaxAcctID;
		[PXDefault(typeof(Search<Vendor.salesTaxAcctID, Where<Vendor.bAccountID, Equal<Current<Tax.taxVendorID>>>>))]
		[Account(DisplayName = "Tax Payable Account", DescriptionField = typeof(Account.description))]
		public virtual Int32? SalesTaxAcctID
		{
			get
			{
				return this._SalesTaxAcctID;
			}
			set
			{
				this._SalesTaxAcctID = value;
			}
		}
		#endregion
		#region SalesTaxSubID
		public abstract class salesTaxSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _SalesTaxSubID;
		[PXDefault(typeof(Search<Vendor.salesTaxSubID, Where<Vendor.bAccountID, Equal<Current<Tax.taxVendorID>>>>))]
		[SubAccount(typeof(Tax.salesTaxAcctID), DisplayName = "Tax Payable Subaccount", DescriptionField = typeof(Sub.description))]
		public virtual Int32? SalesTaxSubID
		{
			get
			{
				return this._SalesTaxSubID;
			}
			set
			{
				this._SalesTaxSubID = value;
			}
		}
		#endregion
		#region PurchTaxAcctID
		public abstract class purchTaxAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _PurchTaxAcctID;
		[PXDefault(typeof(Search<Vendor.purchTaxAcctID, Where<Vendor.bAccountID, Equal<Current<Tax.taxVendorID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[Account(DisplayName = "Tax Claimable Account", DescriptionField = typeof(Account.description))]
		public virtual Int32? PurchTaxAcctID
		{
			get
			{
				return this._PurchTaxAcctID;
			}
			set
			{
				this._PurchTaxAcctID = value;
			}
		}
		#endregion
		#region PurchTaxSubID
		public abstract class purchTaxSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _PurchTaxSubID;
		[PXDefault(typeof(Search<Vendor.purchTaxSubID, Where<Vendor.bAccountID, Equal<Current<Tax.taxVendorID>>>>), PersistingCheck=PXPersistingCheck.Nothing)]
		[SubAccount(typeof(Tax.purchTaxAcctID), DisplayName = "Tax Claimable Subaccount", DescriptionField = typeof(Sub.description))]
		public virtual Int32? PurchTaxSubID
		{
			get
			{
				return this._PurchTaxSubID;
			}
			set
			{
				this._PurchTaxSubID = value;
			}
		}
		#endregion
		#region PendingSalesTaxAcctID
		public abstract class pendingSalesTaxAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _PendingSalesTaxAcctID;
		[PXDefault(typeof(Search<Vendor.salesTaxAcctID, Where<Vendor.bAccountID, Equal<Current<Tax.taxVendorID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[Account(DisplayName = "Pending Tax Payable Account", DescriptionField = typeof(Account.description))]
		public virtual Int32? PendingSalesTaxAcctID
		{
			get
			{
				return this._PendingSalesTaxAcctID;
			}
			set
			{
				this._PendingSalesTaxAcctID = value;
			}
		}
		#endregion
		#region PendingSalesTaxSubID
		public abstract class pendingSalesTaxSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _PendingSalesTaxSubID;
		[PXDefault(typeof(Search<Vendor.salesTaxSubID, Where<Vendor.bAccountID, Equal<Current<Tax.taxVendorID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[SubAccount(typeof(Tax.pendingSalesTaxAcctID), DisplayName = "Pending Tax Payable Subaccount", DescriptionField = typeof(Sub.description))]
		public virtual Int32? PendingSalesTaxSubID
		{
			get
			{
				return this._PendingSalesTaxSubID;
			}
			set
			{
				this._PendingSalesTaxSubID = value;
			}
		}
		#endregion
		#region PendingPurchTaxAcctID
		public abstract class pendingPurchTaxAcctID : PX.Data.IBqlField
		{
		}
		protected Int32? _PendingPurchTaxAcctID;
		[PXDefault(typeof(Search<Vendor.purchTaxAcctID, Where<Vendor.bAccountID, Equal<Current<Tax.taxVendorID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[Account(DisplayName = "Pending Tax Claimable Account", DescriptionField = typeof(Account.description))]
		public virtual Int32? PendingPurchTaxAcctID
		{
			get
			{
				return this._PendingPurchTaxAcctID;
			}
			set
			{
				this._PendingPurchTaxAcctID = value;
			}
		}
		#endregion
		#region PendingPurchTaxSubID
		public abstract class pendingPurchTaxSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _PendingPurchTaxSubID;
		[PXDefault(typeof(Search<Vendor.purchTaxSubID, Where<Vendor.bAccountID, Equal<Current<Tax.taxVendorID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[SubAccount(typeof(Tax.pendingPurchTaxAcctID), DisplayName = "Pending Tax Claimable Subaccount", DescriptionField = typeof(Sub.description))]
		public virtual Int32? PendingPurchTaxSubID
		{
			get
			{
				return this._PendingPurchTaxSubID;
			}
			set
			{
				this._PendingPurchTaxSubID = value;
			}
		}
		#endregion
		#region ExpenseAccountID
		public abstract class expenseAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _ExpenseAccountID;
		[PXDefault(typeof(Search<Vendor.taxExpenseAcctID, Where<Vendor.bAccountID, Equal<Current<Tax.taxVendorID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[Account(DisplayName = "Tax Expense Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		public virtual Int32? ExpenseAccountID
		{
			get
			{
				return this._ExpenseAccountID;
			}
			set
			{
				this._ExpenseAccountID = value;
			}
		}
		#endregion
		#region ExpenseSubID
		public abstract class expenseSubID : PX.Data.IBqlField
		{
		}
		protected Int32? _ExpenseSubID;
		[PXDefault(typeof(Search<Vendor.taxExpenseSubID, Where<Vendor.bAccountID, Equal<Current<Tax.taxVendorID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[SubAccount(typeof(Tax.expenseAccountID), DisplayName = "Tax Expense Subaccount", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
		public virtual Int32? ExpenseSubID
		{
			get
			{
				return this._ExpenseSubID;
			}
			set
			{
				this._ExpenseSubID = value;
			}
		}
		#endregion
		#region Outdated
		public abstract class outdated : PX.Data.IBqlField
		{
		}
		protected Boolean? _Outdated;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Is Blocked", Enabled=false, Visibility=PXUIVisibility.SelectorVisible, Visible=false)]
		public virtual Boolean? Outdated
		{
			get
			{
				return this._Outdated;
			}
			set
			{
				this._Outdated = value;
			}
		}
		#endregion
		#region OutDate
		public abstract class outDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _OutDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Not Valid After", Visibility = PXUIVisibility.Visible)]
		public virtual DateTime? OutDate
		{
			get
			{
				return this._OutDate;
			}
			set
			{
				this._OutDate = value;
			}
		}
		#endregion
		#region IsImported
		public abstract class isImported : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsImported;
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? IsImported
		{
			get
			{
				return this._IsImported;
			}
			set
			{
				this._IsImported = value;
			}
		}
		#endregion
		#region IsExternal
		public abstract class isExternal : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsExternal;
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? IsExternal
		{
			get
			{
				return this._IsExternal;
			}
			set
			{
				this._IsExternal = value;
			}
		}
		#endregion

		#region NoteID
		public abstract class noteID : IBqlField { }

		[PXNote(DescriptionField = typeof(Tax.taxID),
			Selector = typeof(Search<Tax.taxID>))]
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
	}
}
