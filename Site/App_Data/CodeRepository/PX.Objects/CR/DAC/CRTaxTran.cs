using System;
using PX.Data;
using PX.Objects.TX;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.AP;

namespace PX.Objects.CR
{
	[PXProjection(typeof(Select<CROpportunityTax, Where<CROpportunityTax.opportunityProductID, Equal<intMax>>>), Persistent = true)]
    [Serializable]
	public partial class CRTaxTran : CROpportunityTax
	{
		#region OpportunityID
		public new abstract class opportunityID : PX.Data.IBqlField
		{
		}
		[PXDBString(10, IsUnicode = true, IsKey = true)]
		[PXDBDefault(typeof(CROpportunity.opportunityID))]
		[PXUIField(DisplayName = "Opportunity ID", Enabled = false, Visible = false)]
		public override String OpportunityID
		{
			get
			{
				return this._OpportunityID;
			}
			set
			{
				this._OpportunityID = value;
			}
		}
		#endregion
		#region OpportunityProductID
		public new abstract class opportunityProductID : PX.Data.IBqlField
		{
		}
		[PXDBInt(IsKey = true)]
		[PXDefault(int.MaxValue)]
		[PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false)]
        [PXParent(typeof(Select<CROpportunity, Where<CROpportunity.opportunityID, Equal<Current<CRTaxTran.opportunityID>>>>))]
		public override Int32? OpportunityProductID
		{
			get
			{
				return this._OpportunityProductID;
			}
			set
			{
				this._OpportunityProductID = value;
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
		[CurrencyInfo(typeof(CROpportunity.curyInfoID))]
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
		[PXDBCurrency(typeof(CRTaxTran.curyInfoID), typeof(CRTaxTran.taxableAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Taxable Amount", Visibility = PXUIVisibility.Visible)]
        [PXUnboundFormula(typeof(Switch<Case<WhereExempt<CRTaxTran.taxID>, CRTaxTran.curyTaxableAmt>, decimal0>), typeof(SumCalc<CROpportunity.curyVatExemptTotal>))]
        [PXUnboundFormula(typeof(Switch<Case<WhereTaxable<CRTaxTran.taxID>, CRTaxTran.curyTaxableAmt>, decimal0>), typeof(SumCalc<CROpportunity.curyVatTaxableTotal>))]        
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
		[PXDBCurrency(typeof(CRTaxTran.curyInfoID), typeof(CRTaxTran.taxAmt))]
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
	}
}
