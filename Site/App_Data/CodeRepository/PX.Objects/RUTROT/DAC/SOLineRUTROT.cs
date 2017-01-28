using System;
using System.Collections;
using System.Collections.Generic;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.SO;

namespace PX.Objects.RUTROT
{
	public class SOLineRUTROT : PXCacheExtension<SOLine>, IRUTROTableLine
	{
		#region IsRUTROTDeductible
		public abstract class isRUTROTDeductible : IBqlField { }

		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "ROT or RUT deductible", FieldClass = RUTROTMessages.FieldClass, Visible = false)]
		public virtual bool? IsRUTROTDeductible { get; set; }
		#endregion
		#region RUTROTItemType
		public abstract class rUTROTItemType : IBqlField { }
		protected String _RUTROTItemType;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(RUTROTItemTypes.OtherCost, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Item Type", FieldClass = RUTROTMessages.FieldClass)]
		[RUTROTItemTypes.List]
		public virtual String RUTROTItemType
		{
			get
			{
				return this._RUTROTItemType;
			}
			set
			{
				this._RUTROTItemType = value;
			}
		}
		#endregion
		#region RUTROTWorkTypeID
		public abstract class rUTROTWorkTypeID : IBqlField { }
		protected Int32? _RUTROTWorkTypeID;
		[PXDBInt]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Type of Work", FieldClass = RUTROTMessages.FieldClass)]
		[PXSelector(typeof(Search<RUTROTWorkType.workTypeID,
			Where2<Where<RUTROTWorkType.endDate, Greater<Current<SOOrder.orderDate>>, Or<RUTROTWorkType.endDate, IsNull>>,
				And<RUTROTWorkType.startDate, LessEqual<Current<SOOrder.orderDate>>,
				And<RUTROTWorkType.rUTROTType, Equal<Current<RUTROT.rUTROTType>>>>>>),
			SubstituteKey = typeof(RUTROTWorkType.description), DescriptionField = typeof(RUTROTWorkType.xmlTag))]
		public virtual Int32? RUTROTWorkTypeID
		{
			get
			{
				return this._RUTROTWorkTypeID;
			}
			set
			{
				this._RUTROTWorkTypeID = value;
			}
		}
		#endregion
		#region CuryRUTROTTaxAmountDeductible
		public abstract class curyRUTROTTaxAmountDeductible : IBqlField { }

		[PXDBCurrency(typeof(SOLine.curyInfoID), typeof(rUTROTTaxAmountDeductible))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual decimal? CuryRUTROTTaxAmountDeductible { get; set; }
		#endregion
		#region RUTROTTaxAmountDeductible
		public abstract class rUTROTTaxAmountDeductible : IBqlField { }

		[PXDBBaseCury]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual decimal? RUTROTTaxAmountDeductible { get; set; }
		#endregion
		#region CuryRUTROTAvailableAmt
		public abstract class curyRUTROTAvailableAmt : IBqlField { }
		[PXParent(typeof(Select<RUTROT, Where<RUTROT.docType, Equal<Current<SOLine.orderType>>, And<RUTROT.refNbr, Equal<Current<SOLine.orderNbr>>>>>), LeaveChildren = true)]
		[PXDBCurrency(typeof(SOLine.curyInfoID), typeof(rUTROTAvailableAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Deductible Amount", Enabled = false, FieldClass = RUTROTMessages.FieldClass, Visible = false)]
		[PXFormula(typeof(Switch<Case<Where<isRUTROTDeductible, Equal<True>>,
							Mult<curyRUTROTTotal, Mult<IsNull<Current<RUTROT.deductionPct>, decimal0>, decimalPct>>>,
						decimal0>),
		  typeof(SumCalc<RUTROT.curyTotalAmt>), FieldClass = RUTROTMessages.FieldClass)]
		public virtual decimal? CuryRUTROTAvailableAmt { get; set; }
		#endregion
		#region RUTROTAvailableAmt
		public abstract class rUTROTAvailableAmt : IBqlField { }

		[PXDBBaseCury]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual decimal? RUTROTAvailableAmt { get; set; }
		#endregion
		#region CuryRUTROTTotal
		public abstract class curyRUTROTTotal : IBqlField { }
		[PXCurrency(typeof(SOLine.curyInfoID), typeof(rUTROTTotal))]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXFormula(typeof(Add<Sub<SOLine.curyExtPrice, SOLine.curyDiscAmt>, IsNull<curyRUTROTTaxAmountDeductible, decimal0>>))]
		[PXUnboundFormula(typeof(Switch<Case<Where<rUTROTItemType, Equal<RUTROTItemTypes.materialCost>>,
			curyRUTROTTotal>>),
			typeof(SumCalc<RUTROT.curyMaterialCost>), FieldClass = RUTROTMessages.FieldClass)]
		[PXUnboundFormula(typeof(Switch<Case<Where<rUTROTItemType, Equal<RUTROTItemTypes.otherCost>>,
			curyRUTROTTotal>>),
			typeof(SumCalc<RUTROT.curyOtherCost>), FieldClass = RUTROTMessages.FieldClass)]
		[PXUnboundFormula(typeof(Switch<Case<Where<rUTROTItemType, Equal<RUTROTItemTypes.service>>,
			curyRUTROTTotal>>),
			typeof(SumCalc<RUTROT.curyWorkPrice>), FieldClass = RUTROTMessages.FieldClass)]
		public virtual decimal? CuryRUTROTTotal { get; set; }
		#endregion
		#region RUTROTTotal
		public abstract class rUTROTTotal : IBqlField { }
		[PXBaseCury]
		public virtual decimal? RUTROTTotal { get; set; }
		#endregion
		public int? GetInventoryID() {  return Base.InventoryID;  }
	}
}
