using System;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.GL;

namespace PX.Objects.RUTROT
{
	[Serializable]
	public class BranchRUTROT : PXCacheExtension<Branch>
	{
		#region AllowsRUTROT
		public abstract class allowsRUTROT : IBqlField { }

		/// <summary>
		/// When set to <c>true</c>, indicates that the Branch allows for ROT and RUT deduction.
		/// This, in turn, allows to mark the lines of AR invoices as <see cref="AR.ARTran.IsRUTROTDeductible">RUT and ROT deductible</see>.
		/// This field is relevant only if the <see cref="FeaturesSet.RutRotDeduction">ROT and RUT Deduction</see> feature has been activated.
		/// </summary>
		/// <value>
		/// Defaults to <c>false</c>.
		/// </value>
		[PXDBBool]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Uses ROT & RUT deduction", FieldClass = RUTROTMessages.FieldClass)]
		public virtual Boolean? AllowsRUTROT { get; set; }
		#endregion
		#region RUTDeductionPct
		public abstract class rUTDeductionPct : IBqlField { }

		/// <summary>
		/// The percentage of RUT deduction for this Branch.
		/// This field is relevant only for Branches that <see cref="AllowsRUTROT">allow RUT and ROT deduction</see>.
		/// </summary>
		/// <value>
		/// Determines the percentage of invoice amount that can be deducted and claimed from government.
		/// </value>
		[PXDBDecimal(MinValue = 0, MaxValue = 100)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = RUTROTMessages.DeductionPercent, FieldClass = RUTROTMessages.FieldClass)]
		public Decimal? RUTDeductionPct { get; set; }
		#endregion
		#region RUTPersonalAllowanceLimit
		public abstract class rUTPersonalAllowanceLimit : IBqlField { }

		/// <summary>
		/// The personal allowance limit for RUT deduction.
		/// When RUT deduction is distributed between household memebers, the amounts assigned
		/// to each of the members can't exceed the value specified in this field.
		/// This field is relevant only for Branches that <see cref="AllowsRUTROT">allow RUT and ROT deduction</see>.
		/// </summary>
		/// <value>
		/// Given in the <see cref="RUTROTCuryID">currency of deduction</see>.
		/// </value>
		[PXDBDecimal(MinValue = 0, MaxValue = 100000000)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = RUTROTMessages.AllowanceLimit, FieldClass = RUTROTMessages.FieldClass)]
		public virtual Decimal? RUTPersonalAllowanceLimit { get; set; }
		#endregion
		#region RUTExtraAllowanceLimit
		public abstract class rUTExtraAllowanceLimit : IBqlField { }

		/// <summary>
		/// The personal allowance limit for RUT deduction.
		/// When RUT deduction is distributed between household memebers, the amounts assigned
		/// to each of the members can't exceed the value specified in this field.
		/// This field is relevant only for Branches that <see cref="AllowsRUTROT">allow RUT and ROT deduction</see>.
		/// </summary>
		/// <value>
		/// Given in the <see cref="RUTROTCuryID">currency of deduction</see>.
		/// </value>
		[PXDBDecimal(MinValue = 0, MaxValue = 100000000)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = RUTROTMessages.AllowanceLimitExtra, FieldClass = RUTROTMessages.FieldClass)]
		public virtual Decimal? RUTExtraAllowanceLimit { get; set; }
		#endregion
		#region ROTDeductionPct
		public abstract class rOTDeductionPct : IBqlField { }

		/// <summary>
		/// The percentage of ROT deduction for this Branch.
		/// This field is relevant only for Branches that <see cref="AllowsRUTROT">allow RUT and ROT deduction</see>.
		/// </summary>
		/// <value>
		/// Determines the percentage of invoice amount that can be deducted and claimed from government.
		/// </value>
		[PXDBDecimal(MinValue = 0, MaxValue = 100)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = RUTROTMessages.DeductionPercent, FieldClass = RUTROTMessages.FieldClass)]
		public Decimal? ROTDeductionPct { get; set; }
		#endregion
		#region ROTPersonalAllowanceLimit
		public abstract class rOTPersonalAllowanceLimit : IBqlField { }

		/// <summary>
		/// The personal allowance limit for ROT deduction.
		/// When ROT deduction is distributed between household memebers, the amounts assigned
		/// to each of the members can't exceed the value specified in this field.
		/// This field is relevant only for Branches that <see cref="AllowsRUTROT">allow RUT and ROT deduction</see>.
		/// </summary>
		/// <value>
		/// Given in the <see cref="RUTROTCuryID">currency of deduction</see>.
		/// </value>
		[PXDBDecimal(MinValue = 0, MaxValue = 100000000)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = RUTROTMessages.AllowanceLimit, FieldClass = RUTROTMessages.FieldClass)]
		public virtual Decimal? ROTPersonalAllowanceLimit { get; set; }
		#endregion
		#region ROTExtraAllowanceLimit
		public abstract class rOTExtraAllowanceLimit : IBqlField { }

		/// <summary>
		/// The personal allowance limit for ROT deduction.
		/// When ROT deduction is distributed between household memebers, the amounts assigned
		/// to each of the members can't exceed the value specified in this field.
		/// This field is relevant only for Branches that <see cref="AllowsRUTROT">allow RUT and ROT deduction</see>.
		/// </summary>
		/// <value>
		/// Given in the <see cref="RUTROTCuryID">currency of deduction</see>.
		/// </value>
		[PXDBDecimal(MinValue = 0, MaxValue = 100000000)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = RUTROTMessages.AllowanceLimitExtra, FieldClass = RUTROTMessages.FieldClass)]
		public virtual Decimal? ROTExtraAllowanceLimit { get; set; }
		#endregion
		#region RUTROTCuryID
		public abstract class rUTROTCuryID : PX.Data.IBqlField
		{
		}

		/// <summary>
		/// The identifier of the <see cref="Currency"/>, in which RUT and ROT are deducted.
		/// This field is relevant only for Branches that <see cref="AllowsRUTROT">allow RUT and ROT deduction</see>.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="Currency.CuryID"/> field.
		/// </value>
		[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL")]
		[PXUIField(DisplayName = "Currency", FieldClass = RUTROTMessages.FieldClass)]
		[PXSelector(typeof(Currency.curyID))]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual String RUTROTCuryID { get; set; }
		#endregion
		#region RUTROTClaimNextRefNbr
		public abstract class rUTROTClaimNextRefNbr : PX.Data.IBqlField { }

		/// <summary>
		/// The number of the next RUT and ROT claim document.
		/// The value of this field is used to assign consistent numbers to the RUT and ROT claims created through the
		/// Claim RUT and ROT (AR.53.10.00) screen (corresponds to the <see cref="ClaimRUTROT"/> graph).
		/// This field is relevant only for Branches that <see cref="AllowsRUTROT">allow RUT and ROT deduction</see>.
		/// </summary>
		[PXDBInt(MinValue = 0, MaxValue = 100000000)]
		[PXDefault(0)]
		[PXUIField(DisplayName = "Next Claim Nbr", FieldClass = RUTROTMessages.FieldClass)]
		public virtual int? RUTROTClaimNextRefNbr { get; set; }
		#endregion
		#region RUTROTOrgNbrValidRegEx
		public abstract class rUTROTOrgNbrValidRegEx : PX.Data.IBqlField
		{
		}

		/// <summary>
		/// The regular expression used to validate <see cref="RUTROT.ROTOrganizationNbr">Organization Number</see>
		/// on ROT-deductible <see cref="ARInvoiceRUTROT">Invoices</see>.
		/// This field is relevant only for Branches that <see cref="AllowsRUTROT">allow RUT and ROT deduction</see>.
		/// </summary>
		[PXDBString(255)]
		[PXUIField(DisplayName = "Org. Nbr. Validation Reg. Exp.", FieldClass = RUTROTMessages.FieldClass)]
		public virtual String RUTROTOrgNbrValidRegEx { get; set; }
		#endregion
	}
}
