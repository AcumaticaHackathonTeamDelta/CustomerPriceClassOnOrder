namespace PX.Objects.PO
{
	using System;
	using PX.Data;
	using PX.Objects.TX;
	using PX.Objects.CM;
    using PX.Objects.CS;
	
	[System.SerializableAttribute()]
	public partial class POTax : TaxDetail, PX.Data.IBqlTable
	{
		#region OrderType
		public abstract class orderType : PX.Data.IBqlField
		{
		}
		protected String _OrderType;
		[PXDBString(2, IsKey = true, IsFixed = true)]
		[PXDBDefault(typeof(POOrder.orderType), DefaultForUpdate = false)]
		[PXUIField(DisplayName = "Order Type", Visibility = PXUIVisibility.Visible, Visible = false)]
		public virtual String OrderType
		{
			get
			{
				return this._OrderType;
			}
			set
			{
				this._OrderType = value;
			}
		}
		#endregion
		#region OrderNbr
		public abstract class orderNbr : PX.Data.IBqlField
		{
		}
		protected String _OrderNbr;
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "")]
		[PXDBDefault(typeof(POOrder.orderNbr),DefaultForUpdate=false)]
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.Visible, Visible = false)]
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
		protected Int32? _LineNbr;
		[PXDBInt(IsKey = true)]
		[PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false)]
		[PXParent(typeof(Select<POLine, Where<POLine.orderType, Equal<Current<POTax.orderType>>,
			                      And<POLine.orderNbr, Equal<Current<POTax.orderNbr>>, 
								  And<POLine.lineNbr, Equal<Current<POTax.lineNbr>>>>>>))]
		[PXParent(typeof(Select<POLineR, Where<POLineR.orderType, Equal<Current<POTax.orderType>>,
								  And<POLineR.orderNbr, Equal<Current<POTax.orderNbr>>,
								  And<POLineR.lineNbr, Equal<Current<POTax.lineNbr>>>>>>))]
		[PXParent(typeof(Select<POLineUOpen, Where<POLineUOpen.orderType, Equal<Current<POTax.orderType>>,
								  And<POLineUOpen.orderNbr, Equal<Current<POTax.orderNbr>>,
								  And<POLineUOpen.lineNbr, Equal<Current<POTax.lineNbr>>>>>>))]
		public virtual Int32? LineNbr
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
		#region TaxID
		public abstract class taxID : PX.Data.IBqlField
		{
		}
		[PXDBString(Tax.taxID.Length, IsUnicode = true, IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Tax ID")]
		[PXSelector(typeof(Tax.taxID), DescriptionField = typeof(Tax.descr))]
		public override String TaxID
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
		#region TaxRate
		public abstract class taxRate : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryInfoID
		public  abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		
		[PXDBLong()]
		[CurrencyInfo(typeof(POOrder.curyInfoID))]
		public override Int64? CuryInfoID
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
		#region CuryTaxableAmt
		public abstract class curyTaxableAmt : PX.Data.IBqlField
		{
		}
		protected decimal? _CuryTaxableAmt;
		[PXDBCurrency(typeof(POTax.curyInfoID), typeof(POTax.taxableAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Taxable Amount", Visibility = PXUIVisibility.Visible)]
		public virtual Decimal? CuryTaxableAmt
		{
			get
			{
				return this._CuryTaxableAmt;
			}
			set
			{
				this._CuryTaxableAmt = value;
			}
		}
		#endregion
		#region CuryOpenTaxableAmt
		public abstract class curyOpenTaxableAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryOpenTaxableAmt;
		[PXDBCurrency(typeof(POTax.curyInfoID), typeof(POTax.openTaxableAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Open Taxable Amount", Visibility = PXUIVisibility.Visible)]
		public virtual Decimal? CuryOpenTaxableAmt
		{
			get
			{
				return this._CuryOpenTaxableAmt;
			}
			set
			{
				this._CuryOpenTaxableAmt = value;
			}
		}
		#endregion
		#region TaxableAmt
		public abstract class taxableAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _TaxableAmt;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Taxable Amount", Visibility = PXUIVisibility.Visible)]
		public virtual Decimal? TaxableAmt
		{
			get
			{
				return this._TaxableAmt;
			}
			set
			{
				this._TaxableAmt = value;
			}
		}
		#endregion
		#region OpenTaxableAmt
		public abstract class openTaxableAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _OpenTaxableAmt;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? OpenTaxableAmt
		{
			get
			{
				return this._OpenTaxableAmt;
			}
			set
			{
				this._OpenTaxableAmt = value;
			}
		}
		#endregion
		#region CuryTaxAmt
		public abstract class curyTaxAmt : PX.Data.IBqlField
		{
		}
		protected decimal? _CuryTaxAmt;
		[PXDBCurrency(typeof(POTax.curyInfoID), typeof(POTax.taxAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Tax Amount", Visibility = PXUIVisibility.Visible)]
		public virtual Decimal? CuryTaxAmt
		{
			get
			{
				return this._CuryTaxAmt;
			}
			set
			{
				this._CuryTaxAmt = value;
			}
		}
		#endregion
		#region CuryOpenTaxAmt
		public abstract class curyOpenTaxAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryOpenTaxAmt;
		[PXDBCurrency(typeof(POTax.curyInfoID), typeof(POTax.openTaxAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Open Tax Amount", Visibility = PXUIVisibility.Visible)]
		public virtual Decimal? CuryOpenTaxAmt
		{
			get
			{
				return this._CuryOpenTaxAmt;
			}
			set
			{
				this._CuryOpenTaxAmt = value;
			}
		}
		#endregion
		#region TaxAmt
		public abstract class taxAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _TaxAmt;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Tax Amount", Visibility = PXUIVisibility.Visible)]
		public virtual Decimal? TaxAmt
		{
			get
			{
				return this._TaxAmt;
			}
			set
			{
				this._TaxAmt = value;
			}
		}
		#endregion
		#region CuryExpenseAmt
		public abstract class curyExpenseAmt : PX.Data.IBqlField
		{
		}
		[PXDBCurrency(typeof(POTax.curyInfoID), typeof(POTax.expenseAmt))]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Expense Amount", Visibility = PXUIVisibility.Visible)]
		public override Decimal? CuryExpenseAmt
		{
			get; set;
		}
		#endregion
		#region ExpenseAmt
		public abstract class expenseAmt : PX.Data.IBqlField
		{
		}
		#endregion
		#region OpenTaxAmt
		public abstract class openTaxAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _OpenTaxAmt;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? OpenTaxAmt
		{
			get
			{
				return this._OpenTaxAmt;
			}
			set
			{
				this._OpenTaxAmt = value;
			}
		}
		#endregion
		#region TaxZoneID
		public abstract class taxZoneID : IBqlField { }
        [PXDBString(10, IsUnicode = true)]
        public virtual string TaxZoneID
        {
            get;
            set;
        }
        #endregion
	}
}
