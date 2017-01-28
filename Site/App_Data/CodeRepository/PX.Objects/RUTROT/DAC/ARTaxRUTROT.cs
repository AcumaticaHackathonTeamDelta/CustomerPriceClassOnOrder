
using System;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.TX;
using PX.Objects.AR;


namespace PX.Objects.RUTROT
{
	[System.SerializableAttribute()]
	public partial class ARTaxRUTROT : PXCacheExtension<ARTax>
	{
		#region CuryRUTROTTaxAmt
		public abstract class curyRUTROTTaxAmt : IBqlField { }

		[PXCurrency(typeof(ARTax.curyInfoID), typeof(rUTROTTaxAmt))]
		[PXUnboundFormula(typeof(Switch<Case<Where<Selector<ARTax.taxID, Tax.taxCalcLevel>, NotEqual<CSTaxCalcLevel.inclusive>>, ARTax.curyTaxAmt>, CS.decimal0>),
				   typeof(SumCalc<ARTranRUTROT.curyRUTROTTaxAmountDeductible>), FieldClass = RUTROTMessages.FieldClass)]
		public virtual decimal? CuryRUTROTTaxAmt { get; set; }
		#endregion
		#region RUTROTTaxAmt
		public abstract class rUTROTTaxAmt : IBqlField { }

		[PXBaseCury]
		public virtual decimal? RUTROTTaxAmt { get; set; }
		#endregion
	}
}
