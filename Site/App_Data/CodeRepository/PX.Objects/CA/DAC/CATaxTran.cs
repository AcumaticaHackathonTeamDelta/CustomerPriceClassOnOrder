using System;
using PX.Data;
using PX.Objects.TX;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.AP;

namespace PX.Objects.CA
{
	[PXProjection(typeof(Select<TaxTran, Where<TaxTran.module, Equal<BatchModule.moduleCA>>>), Persistent = true)]
    [Serializable]
	[PXCacheName(Messages.CATaxTran)]
	public partial class CATaxTran : TaxTran
	{
		#region Module
		public new abstract class module : PX.Data.IBqlField
		{
		}
        [PXDBString(2, IsKey = true, IsFixed = true)]
		[PXDefault((string)BatchModule.CA)]
		[PXUIField(DisplayName = "Module", Enabled = false, Visible = false)]
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
		#region TranType
		public new abstract class tranType : PX.Data.IBqlField
		{
		}
        [PXDBString(3, IsKey = true, IsFixed = true)]
		[PXDBDefault(typeof(CAAdj.adjTranType))]
		[PXParent(typeof(Select<CAAdj, Where<CAAdj.adjTranType, Equal<Current<TaxTran.tranType>>, And<CAAdj.adjRefNbr, Equal<Current<TaxTran.refNbr>>>>>))]
		[PXUIField(DisplayName = "Tran. Type", Enabled = false, Visible = false)]
		public override String TranType
		{
			get
			{
				return this._TranType;
			}
			set
			{
				this._TranType = value;
			}
		}
		#endregion
		#region RefNbr
		public new abstract class refNbr : PX.Data.IBqlField
		{
		}
        [PXDBString(15, IsKey = true, IsUnicode = true)]
		[PXDBDefault(typeof(CAAdj.adjRefNbr))]
		[PXUIField(DisplayName = "Ref. Nbr.", Enabled = false, Visible = false)]
		public override String RefNbr
		{
			get
			{
				return this._RefNbr;
			}
			set
			{
				this._RefNbr = value;
			}
		}
		#endregion
		#region BranchID
		public new abstract class branchID : PX.Data.IBqlField
		{
		}
		[Branch(typeof(Search<CashAccount.branchID, Where<CashAccount.cashAccountID, Equal<Current<CAAdj.cashAccountID>>>>), Enabled = false)]
		public override Int32? BranchID
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
		#region Released
		public new abstract class released : PX.Data.IBqlField
		{
		}
		#endregion
		#region Voided
		public new abstract class voided : PX.Data.IBqlField
		{
		}
		#endregion
		#region TaxPeriodID
		public new abstract class taxPeriodID : PX.Data.IBqlField
		{
		}
		[GL.FinPeriodID()]
		public override String TaxPeriodID
		{
			get
			{
				return this._TaxPeriodID;
			}
			set
			{
				this._TaxPeriodID = value;
			}
		}
		#endregion
		#region FinPeriodID
		public new abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		[GL.FinPeriodID()]
		[PXDBDefault(typeof(CAAdj.finPeriodID))]
		public override String FinPeriodID
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
		#region VendorID
		public new abstract class vendorID : PX.Data.IBqlField
		{
		}
		[PXDBInt()]
		[PXDefault(typeof(Search<Tax.taxVendorID, Where<Tax.taxID, Equal<Current<CATaxTran.taxID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public override Int32? VendorID
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
		#region TaxZoneID
		public new abstract class taxZoneID : PX.Data.IBqlField
		{
		}
		[PXDBString(10, IsUnicode = true)]
		[PXDBDefault(typeof(CAAdj.taxZoneID))]
		public override String TaxZoneID
		{
			get
			{
				return this._TaxZoneID;
			}
			set
			{
				this._TaxZoneID = value;
			}
		}
		#endregion
		#region AccountID
		public new abstract class accountID : PX.Data.IBqlField
		{
		}
		[Account()]
		[PXDefault()]
		public override Int32? AccountID
		{
			get
			{
				return this._AccountID;
			}
			set
			{
				this._AccountID = value;
			}
		}
		#endregion
		#region SubID
		public new abstract class subID : PX.Data.IBqlField
		{
		}
		[SubAccount()]
		[PXDefault()]
		public override Int32? SubID
		{
			get
			{
				return this._SubID;
			}
			set
			{
				this._SubID = value;
			}
		}
		#endregion
		#region TranDate
		public new abstract class tranDate : PX.Data.IBqlField
		{
		}
		[PXDBDate()]
		[PXDBDefault(typeof(CAAdj.tranDate))]
		public override DateTime? TranDate
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
		#region TaxType
		public new abstract class taxType : PX.Data.IBqlField
		{
		}
		[PXDBString(1, IsFixed = true)]
		[PXDefault()]
		public override String TaxType
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
		#region TaxBucketID
		public new abstract class taxBucketID : PX.Data.IBqlField
		{
		}
		[PXDBInt()]
		[PXDefault(typeof(Search<TaxRev.taxBucketID, Where<TaxRev.taxID, Equal<Current<CATaxTran.taxID>>, And<Current<CATaxTran.tranDate>, Between<TaxRev.startDate, TaxRev.endDate>, And2<Where<TaxRev.taxType, Equal<Current<CATaxTran.taxType>>, Or<TaxRev.taxType, Equal<TaxType.sales>, And<Current<CATaxTran.taxType>, Equal<TaxType.pendingSales>, Or<TaxRev.taxType, Equal<TaxType.purchase>, And<Current<CATaxTran.taxType>, Equal<TaxType.pendingPurchase>>>>>>, And<TaxRev.outdated, Equal<boolFalse>>>>>>))]
		public override Int32? TaxBucketID
		{
			get
			{
				return this._TaxBucketID;
			}
			set
			{
				this._TaxBucketID = value;
			}
		}
		#endregion
		#region CuryInfoID
		public new abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		[PXDBLong()]
		[CurrencyInfo(typeof(CAAdj.curyInfoID))]
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
		public new abstract class curyTaxableAmt : PX.Data.IBqlField
		{
		}
		[PXDBCurrency(typeof(CATaxTran.curyInfoID), typeof(CATaxTran.taxableAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Taxable Amount", Visibility = PXUIVisibility.Visible)]
        [PXUnboundFormula(typeof(Switch<Case<WhereExempt<CATaxTran.taxID>, CATaxTran.curyTaxableAmt>, decimal0>), typeof(SumCalc<CAAdj.curyVatExemptTotal>))]
        [PXUnboundFormula(typeof(Switch<Case<WhereTaxable<CATaxTran.taxID>, CATaxTran.curyTaxableAmt>, decimal0>), typeof(SumCalc<CAAdj.curyVatTaxableTotal>))]
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
		#region CuryTaxAmt
		public new abstract class curyTaxAmt : PX.Data.IBqlField
		{
		}
		[PXDBCurrency(typeof(CATaxTran.curyInfoID), typeof(CATaxTran.taxAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
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

        #region NonDeductibleTaxRate
        public new abstract class nonDeductibleTaxRate : PX.Data.IBqlField
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
		[PXDBCurrency(typeof(CATaxTran.curyInfoID), typeof(CATaxTran.expenseAmt))]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Expense Amount", Visibility = PXUIVisibility.Visible)]
		public override Decimal? CuryExpenseAmt
		{
			get; set;
		}
		#endregion
	}
}
