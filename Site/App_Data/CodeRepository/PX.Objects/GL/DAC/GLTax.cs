namespace PX.Objects.GL
{
	using System;
	using PX.Data;
	using PX.Objects.TX;
	using PX.Objects.CM;
	using PX.Objects.CS;
	using PX.Objects.GL;

	[System.SerializableAttribute()]
	[PXCacheName(Messages.GLTax)]
	public partial class GLTax : TaxDetail, PX.Data.IBqlTable
	{
		#region Module
		public abstract class module : PX.Data.IBqlField
		{
		}
		protected String _Module;
		[PXDBString(2, IsKey = true, IsFixed = true)]
		[PXDBDefault(typeof(GLDocBatch), DefaultForUpdate = false)]
		[PXUIField(DisplayName = "Module", Visibility = PXUIVisibility.Visible, Visible = false)]
		public virtual String Module
		{
			get
			{
				return this._Module;
			}
			set
			{
				this._Module = value;
			}
		}
		#endregion
		#region BatchNbr
		public abstract class batchNbr : PX.Data.IBqlField
		{
		}
		protected String _BatchNbr;
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "")]
		[PXDBDefault(typeof(GLDocBatch), DefaultForUpdate = false)]
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.Visible, Visible = false)]
		public virtual String BatchNbr
		{
			get
			{
				return this._BatchNbr;
			}
			set
			{
				this._BatchNbr = value;
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
		[PXParent(typeof(Select<GLTranDoc, Where<GLTranDoc.module, Equal<Current<GLTax.module>>,
								  And<GLTranDoc.batchNbr, Equal<Current<GLTax.batchNbr>>,
								  And<GLTranDoc.lineNbr, Equal<Current<GLTax.lineNbr>>,
								  And<Current<GLTax.detailType>,Equal<GLTaxDetailType.lineTax>>>>>>))]
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
		#region DetailType
		public abstract class detailType : PX.Data.IBqlField
		{
		}
		protected Int16? _DetailType;
		[PXDBShort(IsKey = true)]
		[PXDefault(GLTaxDetailType.LineTax)]
		[PXUIField(DisplayName = "Tax Detail Type", Visibility = PXUIVisibility.Visible, Visible = false)]
		public virtual Int16? DetailType
		{
			get
			{
				return this._DetailType;
			}
			set
			{
				this._DetailType = value;
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
		public abstract class curyInfoID : PX.Data.IBqlField
		{
		}

		[PXDBLong()]
		[CurrencyInfo(typeof(GLTranDoc.curyInfoID))]
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
		[PXDBCurrency(typeof(GLTax.curyInfoID), typeof(GLTax.taxableAmt))]
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
		#region CuryTaxAmt
		public abstract class curyTaxAmt : PX.Data.IBqlField
		{
		}
		protected decimal? _CuryTaxAmt;
		[PXDBCurrency(typeof(GLTax.curyInfoID), typeof(GLTax.taxAmt))]
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
		#region ExpenseAmt
		public abstract class expenseAmt : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryExpenseAmt
		public abstract class curyExpenseAmt : PX.Data.IBqlField
		{
		}
		[PXDBCurrency(typeof(GLTax.curyInfoID), typeof(GLTax.expenseAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Expense Amount", Visibility = PXUIVisibility.Visible)]
		public override Decimal? CuryExpenseAmt { get; set; }
		#endregion
	}

	[PXProjection(typeof(Select<GLTax, Where<GLTax.detailType, Equal<GLTaxDetailType.docTax>>>), Persistent = true)]
    [Serializable]
	[PXCacheName(Messages.GLTaxTran)]
	public class GLTaxTran : GLTax
	{
		#region Module
		public new abstract class module : PX.Data.IBqlField
		{
		}
		[PXDBString(2, IsKey = true, IsFixed = true)]		
		[PXDBDefault(typeof(GLDocBatch), DefaultForUpdate = false)]
		[PXUIField(DisplayName = "Order Type", Visible = false)]
		public override String Module
		{
			get
			{
				return this._Module;
			}
			set
			{
				this._Module = value;
			}
		}
		#endregion
		#region BatchNbr
		public new abstract class batchNbr : PX.Data.IBqlField
		{
		}
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "")]
		[PXDBDefault(typeof(GLDocBatch), DefaultForUpdate = false)]
		[PXUIField(DisplayName = "Order Nbr.", Visible = false)]
		public override String BatchNbr
		{
			get
			{
				return this._BatchNbr;
			}
			set
			{
				this._BatchNbr = value;
			}
		}
		#endregion
		#region Line Nbr
		public new abstract class lineNbr : PX.Data.IBqlField
		{
		}

		[PXDBInt(IsKey = true)]
		[PXDefault(typeof(Coalesce<Search<GLTranDoc.lineNbr, Where<GLTranDoc.module, Equal<Current<GLTranDoc.module>>,
																And<GLTranDoc.batchNbr, Equal<Current<GLTranDoc.batchNbr>>,
																And<GLTranDoc.lineNbr, Equal<Current<GLTranDoc.parentLineNbr>>>>>>, 
								   Search<GLTranDoc.lineNbr, Where<GLTranDoc.module, Equal<Current<GLTranDoc.module>>,
																And<GLTranDoc.batchNbr, Equal<Current<GLTranDoc.batchNbr>>,
																And<GLTranDoc.lineNbr, Equal<Current<GLTranDoc.lineNbr>>>>>>>))]
		[PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false)]
		[PXParent(typeof(Select<GLTranDoc, Where<GLTranDoc.module, Equal<Current<GLTaxTran.module>>,
								And<GLTranDoc.batchNbr, Equal<Current<GLTaxTran.batchNbr>>,
								And<GLTranDoc.lineNbr, Equal<Current<GLTaxTran.lineNbr>>>>>>))]
		public override Int32? LineNbr
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
		#region DetailType
		public new abstract class detailType : PX.Data.IBqlField
		{
		}

		[PXDBShort(IsKey = true)]
		[PXDefault(GLTaxDetailType.DocTax)]
		[PXUIField(DisplayName = "Tax Detail Type", Visibility = PXUIVisibility.Visible, Visible = false)]
		public override Int16? DetailType
		{
			get
			{
				return this._DetailType;
			}
			set
			{
				this._DetailType = value;
			}
		}
		#endregion
		#region TaxID
		public new abstract class taxID : PX.Data.IBqlField
		{
		}
		[PXDBString(Tax.taxID.Length, IsUnicode = true, IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Tax ID", Visibility = PXUIVisibility.Visible)]
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
		#region CuryInfoID
		public new abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		[PXDBLong()]
		[CurrencyInfo(typeof(GLTranDoc.curyInfoID))]
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
		#region CuryTaxAmt
		public new abstract class curyTaxAmt : PX.Data.IBqlField
		{
		}
		[PXDBCurrency(typeof(GLTaxTran.curyInfoID), typeof(GLTaxTran.taxAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]		
        [PXFormula(typeof(Switch<Case<Where<GLTaxTran.curyExpenseAmt,NotEqual<decimal0>>,
                Sub<Mult<GLTaxTran.curyTaxableAmt, Div<GLTaxTran.taxRate, decimal100>>, GLTaxTran.curyExpenseAmt>>,GLTaxTran.curyTaxAmt>), null)]
		[PXUIField(DisplayName = "Tax Amount", Visibility = PXUIVisibility.Visible)]
		public override Decimal? CuryTaxAmt
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
		#region TaxAmt
		public new abstract class taxAmt : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryTaxableAmt
		public new abstract class curyTaxableAmt : PX.Data.IBqlField
		{
		}
		[PXDBCurrency(typeof(GLTaxTran.curyInfoID), typeof(GLTaxTran.taxableAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Taxable Amount", Visibility = PXUIVisibility.Visible)]
		//[PXUnboundFormula(typeof(Switch<Case<Where2<WhereExempt<GLTaxTran.taxID>, And<GLTaxTran.detailType, Equal<short1>>>, Row<GLTaxTran.curyTaxableAmt>>, Const<decimal0>>), typeof(SumCalc<GLTranDoc.curyVatExemptTotal>))]
		//[PXUnboundFormula(typeof(Switch<Case<Where2<WhereTaxable<GLTaxTran.taxID>, And<GLTaxTran.detailType, Equal<short1>>>, Row<GLTaxTran.curyTaxableAmt>>, Const<decimal0>>), typeof(SumCalc<GLTranDoc.curyVatTaxableTotal>))]
		public override Decimal? CuryTaxableAmt
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
		#region TaxableAmt
		public new abstract class taxableAmt : PX.Data.IBqlField
		{
		}
		#endregion
        #region NonDeductibleTaxRate
        public abstract class nonDeductibleTaxRate : PX.Data.IBqlField
        {
        }
        #endregion
        #region ExpenseAmt
        public new abstract class expenseAmt : PX.Data.IBqlField
        {
        }
        #endregion        
        #region CuryExpenseAmt
        public new abstract class curyExpenseAmt : PX.Data.IBqlField
        {
        }
        [PXDBCurrency(typeof(GLTaxTran.curyInfoID), typeof(GLTaxTran.expenseAmt))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXFormula(typeof(Mult<Mult<GLTaxTran.curyTaxableAmt, Div<GLTaxTran.taxRate, decimal100>>, Sub<decimal1, Div<GLTaxTran.nonDeductibleTaxRate, decimal100>>>), null)]
        [PXUIField(DisplayName = "Expense Amount", Visibility = PXUIVisibility.Visible)]
        public override Decimal? CuryExpenseAmt { get; set; }
        #endregion        
        
	}


	public class GLTaxDetailType
	{
		public const short LineTax = 0;
		public const short DocTax = 1;

		public class lineTax : PX.Objects.CS.short0
		{
		}
		public class docTax : PX.Objects.CS.short1
		{
		}
	}

}
