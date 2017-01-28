using System;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.CS;

namespace PX.Objects.RUTROT
{
	[Serializable]
	public class BranchBAccountRUTROT : PXCacheExtension<BranchMaint.BranchBAccount>
	{

		#region AllowsRUTROT
		public abstract class allowsRUTROT : IBqlField { }

		[PXDBBool(BqlField = typeof(BranchRUTROT.allowsRUTROT))]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Uses ROT & RUT deduction", FieldClass = RUTROTMessages.FieldClass)]
		public virtual Boolean? AllowsRUTROT { get; set; }
		#endregion
		#region RUTDeductionPct
		public abstract class rUTDeductionPct : IBqlField { }

		[PXDBDecimal(BqlField = typeof(BranchRUTROT.rUTDeductionPct), MinValue = 0, MaxValue = 100)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = RUTROTMessages.DeductionPercent, FieldClass = RUTROTMessages.FieldClass)]
		public Decimal? RUTDeductionPct { get; set; }
		#endregion
		#region RUTPersonalAllowanceLimit
		public abstract class rUTPersonalAllowanceLimit : IBqlField { }

		[PXDBDecimal(BqlField = typeof(BranchRUTROT.rUTPersonalAllowanceLimit), MinValue = 0, MaxValue = 100000000)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = RUTROTMessages.AllowanceLimit, FieldClass = RUTROTMessages.FieldClass)]
		public virtual Decimal? RUTPersonalAllowanceLimit { get; set; }
		#endregion
		#region RUTExtraAllowanceLimit
		public abstract class rUTExtraAllowanceLimit : IBqlField { }

		[PXDBDecimal(BqlField = typeof(BranchRUTROT.rUTExtraAllowanceLimit), MinValue = 0, MaxValue = 100000000)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = RUTROTMessages.AllowanceLimitExtra, FieldClass = RUTROTMessages.FieldClass)]
		public virtual Decimal? RUTExtraAllowanceLimit { get; set; }
		#endregion
		#region ROTDeductionPct
		public abstract class rOTDeductionPct : IBqlField { }

		[PXDBDecimal(BqlField = typeof(BranchRUTROT.rOTDeductionPct), MinValue = 0, MaxValue = 100)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = RUTROTMessages.DeductionPercent, FieldClass = RUTROTMessages.FieldClass)]
		public Decimal? ROTDeductionPct { get; set; }
		#endregion
		#region ROTPersonalAllowanceLimit
		public abstract class rOTPersonalAllowanceLimit : IBqlField { }

		[PXDBDecimal(BqlField = typeof(BranchRUTROT.rOTPersonalAllowanceLimit), MinValue = 0, MaxValue = 100000000)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = RUTROTMessages.AllowanceLimit, FieldClass = RUTROTMessages.FieldClass)]
		public virtual Decimal? ROTPersonalAllowanceLimit { get; set; }
		#endregion
		#region ROTExtraAllowanceLimit
		public abstract class rOTExtraAllowanceLimit : IBqlField { }

		[PXDBDecimal(BqlField = typeof(BranchRUTROT.rOTExtraAllowanceLimit), MinValue = 0, MaxValue = 100000000)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = RUTROTMessages.AllowanceLimitExtra, FieldClass = RUTROTMessages.FieldClass)]
		public virtual Decimal? ROTExtraAllowanceLimit { get; set; }
		#endregion

		#region RUTROTCuryID
		public abstract class rUTROTCuryID : PX.Data.IBqlField
		{
		}

		[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL", BqlField = typeof(BranchRUTROT.rUTROTCuryID))]
		[PXUIField(DisplayName = "Currency", FieldClass = RUTROTMessages.FieldClass)]
		[PXSelector(typeof(Currency.curyID))]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual String RUTROTCuryID { get; set; }
		#endregion
		#region RUTROTClaimNextRefNbr
		public abstract class rUTROTClaimNextRefNbr : PX.Data.IBqlField { }

		[PXDBInt(MinValue = 0, MaxValue = 100000000, BqlField = typeof(BranchRUTROT.rUTROTClaimNextRefNbr))]
		[PXDefault(0)]
		[PXUIField(DisplayName = "Next Export File Ref Nbr", FieldClass = RUTROTMessages.FieldClass)]
		public virtual int? RUTROTClaimNextRefNbr { get; set; }
		#endregion
		#region RUTROTOrgNbrValidRegEx
		public abstract class rUTROTOrgNbrValidRegEx : PX.Data.IBqlField
		{
		}

		[PXDBString(255, BqlField = typeof(BranchRUTROT.rUTROTOrgNbrValidRegEx))]
		[PXUIField(DisplayName = "Org. Nbr. Validation Reg. Exp.", FieldClass = RUTROTMessages.FieldClass)]
		public virtual String RUTROTOrgNbrValidRegEx { get; set; }
		#endregion
	}
}
