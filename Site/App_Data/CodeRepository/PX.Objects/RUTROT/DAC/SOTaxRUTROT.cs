using System;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.TX;
using PX.Objects.SO;

namespace PX.Objects.RUTROT
{
	[System.SerializableAttribute()]
	public partial class SOTaxRUTROT : PXCacheExtension<SOTax>
	{
		#region CuryRUTROTTaxAmt
		public abstract class curyRUTROTTaxAmt : IBqlField { }

		[PXCurrency(typeof(SOTax.curyInfoID), typeof(rUTROTTaxAmt))]
		[PXUnboundFormula(typeof(Switch<Case<Where<Selector<SOTax.taxID, Tax.taxCalcLevel>, NotEqual<CSTaxCalcLevel.inclusive>>, SOTax.curyTaxAmt>, CS.decimal0>),
				   typeof(SumCalc<SOLineRUTROT.curyRUTROTTaxAmountDeductible>), FieldClass = RUTROTMessages.FieldClass)]
		public virtual decimal? CuryRUTROTTaxAmt { get; set; }
		#endregion
		#region RUTROTTaxAmt
		public abstract class rUTROTTaxAmt : IBqlField { }

		[PXBaseCury]
		public virtual decimal? RUTROTTaxAmt { get; set; }
		#endregion
	}
}
