using System;
using PX.Common;
using PX.Data;
using PX.Data.EP;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.AR;
using PX.Objects.CA;


namespace PX.Objects.RUTROT
{
	[Serializable]
	public class RUTROT : IBqlTable
	{
		#region DocType
		public abstract class docType : PX.Data.IBqlField
		{
		}

		protected String _DocType;

		[PXDBString(3, IsKey = true, IsFixed = true)]
		[PXDBDefault(typeof(ARInvoice.docType))]
		[PXParent(typeof(Select<ARInvoice, Where<ARInvoice.docType, Equal<Current<RUTROT.docType>>, 
			And<ARInvoice.refNbr, Equal<Current<RUTROT.refNbr>>>>>))]
		public virtual String DocType
		{
			get
			{
				return this._DocType;
			}
			set
			{
				this._DocType = value;
			}
		}
		#endregion
		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
		}
		protected String _RefNbr;
		[PXDBString(15, IsKey = true, IsUnicode = true)]
		[PXDBDefault(typeof(ARInvoice.refNbr))]
		public virtual String RefNbr
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
		#region RUTROTType
		public abstract class rUTROTType : IBqlField { }

		/// <summary>
		/// The type of deduction for a ROT and RUT deductible document.
		/// This field is relevant only if <see cref="IsRUTROTDeductible"/> is <c>true</c>.
		/// </summary>
		/// <value>
		/// Allowed values are:
		/// <c>"U"</c> - RUT,
		/// <c>"O"</c> - ROT.
		/// Defaults to RUT (<c>"U"</c>).
		/// </value>
		[PXDBString(1)]
		[RUTROTTypes.List]
		[PXDefault(RUTROTTypes.RUT, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Deduction Type", FieldClass = RUTROTMessages.FieldClass, Visible = false)]
		public virtual string RUTROTType { get; set; }
		#endregion

		#region CuryROTPersonalAllowance
		public abstract class curyROTPersonalAllowance : IBqlField { }

		/// <summary>
		/// The personal allowance limit for ROT deductions.
		/// When ROT deduction is distributed between household memebers, the amounts assigned
		/// to each of the members can't exceed the value specified in this field (see <see cref="RUTROTDistribution"/>).
		/// This field is relevant only if <see cref="IsRUTROTDeductible"/> is <c>true</c>.
		/// </summary>
		/// <value>
		/// Defaults to <see cref="Branch.ROTPersonalAllowanceLimit"/>.
		/// Given in the <see cref="CuryID">currency</see> of the document.
		/// </value>
		[PXDBCurrency(typeof(ARInvoice.curyInfoID), typeof(RUTROT.rOTPersonalAllowance))]
		[PXDefault(TypeCode.Decimal, "0.0", typeof(Search<BranchRUTROT.rOTPersonalAllowanceLimit, Where<GL.Branch.branchID, Equal<Current<ARInvoice.branchID>>>>))]
		[PXUIField(DisplayName = RUTROTMessages.AllowanceLimit, FieldClass = RUTROTMessages.FieldClass, Visible = false)]
		public virtual Decimal? CuryROTPersonalAllowance { get; set; }
		#endregion
		#region ROTPersonalAllowance
		public abstract class rOTPersonalAllowance : IBqlField { }

		/// <summary>
		/// The personal allowance limit for ROT deductions.
		/// When ROT deduction is distributed between household memebers, the amounts assigned
		/// to each of the members can't exceed the value specified in this field (see <see cref="RUTROTDistribution"/>).
		/// This field is relevant only if <see cref="IsRUTROTDeductible"/> is <c>true</c>.
		/// </summary>
		/// <value>
		/// Defaults to <see cref="Branch.ROTPersonalAllowanceLimit"/>.
		/// Given in the <see cref="Company.BaseCuryID">base currency</see> of the company.
		/// </value>
		[PXDBBaseCury]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual decimal? ROTPersonalAllowance { get; set; }
		#endregion
		#region CuryROTExtraAllowance
		public abstract class curyROTExtraAllowance : IBqlField { }

		/// <summary>
		/// The personal allowance limit for ROT deductions.
		/// When ROT deduction is distributed between household memebers, the amounts assigned
		/// to each of the members can't exceed the value specified in this field (see <see cref="RUTROTDistribution"/>).
		/// This field is relevant only if <see cref="IsRUTROTDeductible"/> is <c>true</c>.
		/// </summary>
		/// <value>
		/// Defaults to <see cref="Branch.ROTExtraAllowanceLimit"/>.
		/// Given in the <see cref="CuryID">currency</see> of the document.
		/// </value>
		[PXDBCurrency(typeof(ARInvoice.curyInfoID), typeof(RUTROT.rOTExtraAllowance))]
		[PXDefault(TypeCode.Decimal, "0.0", typeof(Search<BranchRUTROT.rOTExtraAllowanceLimit, Where<GL.Branch.branchID, Equal<Current<ARInvoice.branchID>>>>))]
		[PXUIField(DisplayName = RUTROTMessages.AllowanceLimitExtra, FieldClass = RUTROTMessages.FieldClass, Visible = false)]
		public virtual Decimal? CuryROTExtraAllowance { get; set; }
		#endregion
		#region ROTExtraAllowance
		public abstract class rOTExtraAllowance : IBqlField { }

		/// <summary>
		/// The personal allowance limit for ROT deductions.
		/// When ROT deduction is distributed between household memebers, the amounts assigned
		/// to each of the members can't exceed the value specified in this field (see <see cref="RUTROTDistribution"/>).
		/// This field is relevant only if <see cref="IsRUTROTDeductible"/> is <c>true</c>.
		/// </summary>
		/// <value>
		/// Defaults to <see cref="Branch.ROTExtraAllowanceLimit"/>.
		/// Given in the <see cref="Company.BaseCuryID">base currency</see> of the company.
		/// </value>
		[PXDBBaseCury]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual decimal? ROTExtraAllowance { get; set; }
		#endregion
		#region ROTDeductionPct
		public abstract class rOTDeductionPct : IBqlField { }

		/// <summary>
		/// The percentage of ROT deduction for the document.
		/// This field is relevant only if <see cref="IsRUTROTDeductible"/> is <c>true</c>.
		/// </summary>
		/// <value>
		/// Determines the percentage of invoice amount that can be deducted and claimed from government.
		/// Defaults to the <see cref="Branch.ROTDeductionPct">Deduction %</see> specified for the <see cref="BranchID">Branch</see> of the document.
		/// </value>
		[PXDBDecimal(2)]
		[PXDefault(TypeCode.Decimal, "0.0", typeof(Search<BranchRUTROT.rOTDeductionPct, Where<GL.Branch.branchID, Equal<Current<ARInvoice.branchID>>>>))]
		[PXUIField(DisplayName = RUTROTMessages.DeductionPercent, Visible = false, FieldClass = RUTROTMessages.FieldClass)]
		public virtual Decimal? ROTDeductionPct { get; set; }
		#endregion

		#region CuryRUTPersonalAllowance
		public abstract class curyRUTPersonalAllowance : IBqlField { }

		/// <summary>
		/// The personal allowance limit for RUT deductions.
		/// When RUT deduction is distributed between household memebers, the amounts assigned
		/// to each of the members can't exceed the value specified in this field (see <see cref="RUTROTDistribution"/>).
		/// This field is relevant only if <see cref="IsRUTROTDeductible"/> is <c>true</c>.
		/// </summary>
		/// <value>
		/// Defaults to <see cref="Branch.RUTPersonalAllowanceLimit"/>.
		/// Given in the <see cref="CuryID">currency</see> of the document.
		/// </value>
		[PXDBCurrency(typeof(ARInvoice.curyInfoID), typeof(RUTROT.rUTPersonalAllowance))]
		[PXDefault(TypeCode.Decimal, "0.0", typeof(Search<BranchRUTROT.rUTPersonalAllowanceLimit, Where<GL.Branch.branchID, Equal<Current<ARInvoice.branchID>>>>))]
		[PXUIField(DisplayName = RUTROTMessages.AllowanceLimit, FieldClass = RUTROTMessages.FieldClass, Visible = false)]
		public virtual Decimal? CuryRUTPersonalAllowance { get; set; }
		#endregion
		#region RUTPersonalAllowance
		public abstract class rUTPersonalAllowance : IBqlField { }

		/// <summary>
		/// The personal allowance limit for RUT deductions.
		/// When RUT deduction is distributed between household memebers, the amounts assigned
		/// to each of the members can't exceed the value specified in this field (see <see cref="RUTROTDistribution"/>).
		/// This field is relevant only if <see cref="IsRUTROTDeductible"/> is <c>true</c>.
		/// </summary>
		/// <value>
		/// Defaults to <see cref="Branch.RUTPersonalAllowanceLimit"/>.
		/// Given in the <see cref="Company.BaseCuryID">base currency</see> of the company.
		/// </value>
		[PXDBBaseCury]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual decimal? RUTPersonalAllowance { get; set; }
		#endregion
		#region CuryRUTExtraAllowance
		public abstract class curyRUTExtraAllowance : IBqlField { }

		/// <summary>
		/// The personal allowance limit for RUT deductions.
		/// When RUT deduction is distributed between household memebers, the amounts assigned
		/// to each of the members can't exceed the value specified in this field (see <see cref="RUTROTDistribution"/>).
		/// This field is relevant only if <see cref="IsRUTROTDeductible"/> is <c>true</c>.
		/// </summary>
		/// <value>
		/// Defaults to <see cref="Branch.RUTExtraAllowanceLimit"/>.
		/// Given in the <see cref="CuryID">currency</see> of the document.
		/// </value>
		[PXDBCurrency(typeof(ARInvoice.curyInfoID), typeof(RUTROT.rUTExtraAllowance))]
		[PXDefault(TypeCode.Decimal, "0.0", typeof(Search<BranchRUTROT.rUTExtraAllowanceLimit, Where<GL.Branch.branchID, Equal<Current<ARInvoice.branchID>>>>))]
		[PXUIField(DisplayName = RUTROTMessages.AllowanceLimitExtra, FieldClass = RUTROTMessages.FieldClass, Visible = false)]
		public virtual Decimal? CuryRUTExtraAllowance { get; set; }
		#endregion
		#region RUTExtraAllowance
		public abstract class rUTExtraAllowance : IBqlField { }

		/// <summary>
		/// The personal allowance limit for RUT deductions.
		/// When RUT deduction is distributed between household memebers, the amounts assigned
		/// to each of the members can't exceed the value specified in this field (see <see cref="RUTROTDistribution"/>).
		/// This field is relevant only if <see cref="IsRUTROTDeductible"/> is <c>true</c>.
		/// </summary>
		/// <value>
		/// Defaults to <see cref="Branch.RUTExtraAllowanceLimit"/>.
		/// Given in the <see cref="Company.BaseCuryID">base currency</see> of the company.
		/// </value>
		[PXDBBaseCury]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual decimal? RUTExtraAllowance { get; set; }
		#endregion
		#region RUTDeductionPct
		public abstract class rUTDeductionPct : IBqlField { }

		/// <summary>
		/// The percentage of RUT deduction for the document.
		/// This field is relevant only if <see cref="IsRUTROTDeductible"/> is <c>true</c>.
		/// </summary>
		/// <value>
		/// Determines the percentage of invoice amount that can be deducted and claimed from government.
		/// Defaults to the <see cref="Branch.RUTDeductionPct">Deduction %</see> specified for the <see cref="BranchID">Branch</see> of the document.
		/// </value>
		[PXDBDecimal(2)]
		[PXDefault(TypeCode.Decimal, "0.0", typeof(Search<BranchRUTROT.rUTDeductionPct, Where<GL.Branch.branchID, Equal<Current<ARInvoice.branchID>>>>))]
		[PXUIField(DisplayName = RUTROTMessages.DeductionPercent, Visible = false, FieldClass = RUTROTMessages.FieldClass)]
		public virtual Decimal? RUTDeductionPct { get; set; }
		#endregion

		#region DeductionPct
		public abstract class deductionPct : IBqlField { }

		[PXDecimal(2)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = RUTROTMessages.DeductionPercent, Visible = false, FieldClass = RUTROTMessages.FieldClass)]
		[PXFormula(typeof(Switch<Case<Where<RUTROT.rUTROTType, Equal<RUTROTTypes.rut>>, rUTDeductionPct>, rOTDeductionPct>))]
		public virtual Decimal? DeductionPct
		{
			[PXDependsOnFields(typeof(rUTDeductionPct), typeof(rUTDeductionPct), typeof(rUTROTType))]
			get { return RUTROTType == RUTROTTypes.RUT ? RUTDeductionPct.Value : ROTDeductionPct; }
			set { }
		}
		#endregion
		#region AutoDistribution
		public abstract class autoDistribution : IBqlField { }

		/// <summary>
		/// When set to <c>true</c> indicates that the RUT and ROT deduction amount must be distributed between the household members automatically.
		/// Otherwise, the amount assigned to each member is entered manually.
		/// (See <see cref="RUTROTDistribution"/>).
		/// This field is relevant only if <see cref="IsRUTROTDeductible"/> is <c>true</c>.
		/// </summary>
		/// <value>
		/// Defaults to <c>true</c>.
		/// </value>
		[PXDBBool]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Distribute Automatically", FieldClass = RUTROTMessages.FieldClass, Visible = false)]
		public virtual bool? AutoDistribution { get; set; }
		#endregion

		#region IsClaimed
		public abstract class isClaimed : IBqlField { }

		/// <summary>
		/// When set to <c>true</c> indicates that the RUT and ROT deduction amount associated with the document has been claimed from the government.
		/// This field is relevant only if <see cref="IsRUTROTDeductible"/> is <c>true</c>.
		/// </summary>
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "ROT and RUT was claimed", FieldClass = RUTROTMessages.FieldClass, Visible = false)]
		public virtual bool? IsClaimed { get; set; }
		#endregion
		#region CuryAllowedAmt
		public abstract class curyAllowedAmt : IBqlField { }

		/// <summary>
		/// The maximum amount of RUT and ROT deduction allowed for the document.
		/// This field is relevant only if <see cref="IsRUTROTDeductible"/> is <c>true</c>.
		/// Given in the <see cref="CuryID">currency</see> of the document.
		/// </summary>
		/// <value>
		/// The value of this field is calculated automatically based on the <see cref="CuryRUTROTPersonalAllowance"/> and
		/// the number of household members specified for the document. (See <see cref="RUTROTDistribution"/>).
		/// </value>
		[PXCurrency(typeof(ARInvoice.curyInfoID), typeof(allowedAmt))]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual decimal? CuryAllowedAmt { get; set; }
		#endregion
		#region AllowedAmt
		public abstract class allowedAmt : IBqlField { }

		/// <summary>
		/// The maximum amount of RUT and ROT deduction allowed for the document.
		/// This field is relevant only if <see cref="IsRUTROTDeductible"/> is <c>true</c>.
		/// Given in the <see cref="Company.BaseCuryID">base currency</see> of the company.
		/// </summary>
		/// <value>
		/// See <see cref="CuryAllowedAmt"/>.
		/// </value>
		[PXBaseCury]
		public virtual decimal? AllowedAmt { get; set; }
		#endregion
		#region CuryDistributedAmt
		public abstract class curyDistributedAmt : IBqlField { }

		/// <summary>
		/// The amount of RUT and ROT deductions that has been distributed between the household members.
		/// This field is relevant only if <see cref="IsRUTROTDeductible"/> is <c>true</c>.
		/// Given in the <see cref="CuryID">currency</see> of the document.
		/// </summary>
		/// <value>
		/// This field equals the total of <see cref="RUTROTDistribution.CuryAmount"/> for the RUT and ROT distribution records associated with the document.
		/// </value>
		[PXDBCurrency(typeof(ARInvoice.curyInfoID), typeof(distributedAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Distributed Amount", FieldClass = RUTROTMessages.FieldClass, Visible = false)]
		public virtual decimal? CuryDistributedAmt { get; set; }
		#endregion
		#region DistributedAmt
		public abstract class distributedAmt : IBqlField { }

		/// <summary>
		/// The amount of RUT and ROT deductions that has been distributed between the household members.
		/// This field is relevant only if <see cref="IsRUTROTDeductible"/> is <c>true</c>.
		/// Given in the <see cref="Company.BaseCuryID">base currency</see> of the company.
		/// </summary>
		/// <value>
		/// See <see cref="CuryDistributedAmt"/>.
		/// </value>
		[PXDBBaseCury]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual decimal? DistributedAmt { get; set; }
		#endregion
		#region CuryTotalAmt
		public abstract class curyTotalAmt : IBqlField { }

		/// <summary>
		/// The portion of the document amount that is RUT and ROT deductible.
		/// This field is relevant only if <see cref="IsRUTROTDeductible"/> is <c>true</c>.
		/// Given in the <see cref="CuryID">currency</see> of the document.
		/// </summary>
		/// <value>
		/// The value of this field is calculated as the total of the <see cref="ARTran.IsRUTROTDeductible">RUT and ROT deductible lines</see>
		/// with regard to taxes. (See <see cref="ARTran.CuryRUTROTAvailableAmt"/>).
		/// </value>
		[PXDBCurrency(typeof(ARInvoice.curyInfoID), typeof(RUTROT.totalAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Total Deductible Amount", FieldClass = RUTROTMessages.FieldClass, Visible = false)]
		[PXFormula(typeof(Validate<RUTROT.curyAllowedAmt>))]
		public virtual Decimal? CuryTotalAmt { get; set; }
		#endregion
		#region TotalAmt
		public abstract class totalAmt : IBqlField { }

		/// <summary>
		/// The portion of the document amount that is RUT and ROT deductible.
		/// This field is relevant only if <see cref="IsRUTROTDeductible"/> is <c>true</c>.
		/// Given in the <see cref="Company.BaseCuryID">base currency</see> of the company.
		/// </summary>
		/// <value>
		/// See <see cref="CuryTotalAmt"/>.
		/// </value>
		[PXDBBaseCury]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? TotalAmt { get; set; }
		#endregion
		#region CuryUndistributedAmt
		public abstract class curyUndistributedAmt : IBqlField { }

		/// <summary>
		/// The portion of the document amount that is RUT and ROT deductible,
		/// but has not been distributed between household members.
		/// This field is relevant only if <see cref="IsRUTROTDeductible"/> is <c>true</c>.
		/// Given in the <see cref="CuryID">currency</see> of the document.
		/// </summary>
		[PXCurrency(typeof(ARInvoice.curyInfoID), typeof(undistributedAmt))]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		[PXFormula(typeof(IsNull<Sub<RUTROT.curyTotalAmt, RUTROT.curyDistributedAmt>, decimal0>))]
		[PXUIField(DisplayName = "Undistributed Amount", FieldClass = RUTROTMessages.FieldClass, Visible = false)]
		public virtual decimal? CuryUndistributedAmt { get; set; }
		#endregion
		#region UndistributedAmt
		public abstract class undistributedAmt : IBqlField { }

		/// <summary>
		/// The portion of the document amount that is RUT and ROT deductible,
		/// but has not been distributed between household members.
		/// This field is relevant only if <see cref="IsRUTROTDeductible"/> is <c>true</c>.
		/// Given in the <see cref="Company.BaseCuryID">base currency</see> of the company.
		/// </summary>
		[PXBaseCury]
		public virtual decimal? UndistributedAmt { get; set; }
		#endregion
		#region DistributionLineCntr
		public abstract class distributionLineCntr : PX.Data.IBqlField
		{
		}

		/// <summary>
		/// The counter of the <see cref="RUTROTDistribution"/> records associated with the document,
		/// used to assign consistent numbers to the child records.
		/// It is not recommended to rely on this field to determine the exact count of child records, because it might not reflect the latter under various conditions.
		/// This field is relevant only if <see cref="IsRUTROTDeductible"/> is <c>true</c>.
		/// </summary>
		[PXDBInt()]
		[PXDefault(0)]
		public virtual Int32? DistributionLineCntr { get; set; }
		#endregion
		#region ClaimDate
		public abstract class claimDate : IBqlField { }

		/// <summary>
		/// The date when the RUT and ROT claim file that includes this document was generated.
		/// This field is relevant only if <see cref="IsRUTROTDeductible"/> is <c>true</c>.
		/// </summary>
		/// <value>
		/// A value is assigned to this field when the document is exported through the Claim ROT and RUT
		/// (AR.53.10.00) screen (corresponds to the <see cref="ClaimRUTROT"/> graph).
		/// </value>
		[PXDBDate]
		[PXUIField(DisplayName = "Export Date", FieldClass = RUTROTMessages.FieldClass, Visible = false)]
		public DateTime? ClaimDate { get; set; }
		#endregion
		#region ClaimFileName
		public abstract class claimFileName : IBqlField { }

		/// <summary>
		/// The name of the RUT and ROT claim file that includes this document.
		/// This field is relevant only if <see cref="IsRUTROTDeductible"/> is <c>true</c>.
		/// </summary>
		/// <value>
		/// A value is assigned to this field when the document is exported through the Claim ROT and RUT
		/// (AR.53.10.00) screen (corresponds to the <see cref="ClaimRUTROT"/> graph).
		/// </value>
		[PXDBString(40)]
		[PXUIField(DisplayName = "Export File", FieldClass = RUTROTMessages.FieldClass, Visible = false)]
		public virtual string ClaimFileName { get; set; }
		#endregion
		#region ExportRefNbr
		public abstract class exportRefNbr : IBqlField { }

		/// <summary>
		/// The reference number of the RUT or ROT claim that includes the document.
		/// This field is relevant only if <see cref="IsRUTROTDeductible"/> is <c>true</c>.
		/// </summary>
		/// <value>
		/// The system uses the <see cref="Branch.RUTROTClaimNextRefNbr"/> field to generate the claim number.
		/// A value is assigned to this field when the document is exported through the Claim ROT and RUT
		/// (AR.53.10.00) screen (corresponds to the <see cref="ClaimRUTROT"/> graph).
		/// </value>
		[PXDBInt]
		[PXUIField(DisplayName = "Export Ref Nbr.", FieldClass = RUTROTMessages.FieldClass, Visible = false)]
		public virtual int? ExportRefNbr { get; set; }
		#endregion

		#region ROTAppartment
		public abstract class rOTAppartment : IBqlField { }

		/// <summary>
		/// The <see cref="CustomerID">Customer's</see> real estate or appartment number for a ROT deductible document.
		/// This field is relevant only if <see cref="IsRUTROTDeductible"/> is <c>true</c> and <see cref="RUTROTType"/> is ROT (<c>"O"</c>).
		/// </summary>
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "Apartment", FieldClass = RUTROTMessages.FieldClass)]
		public virtual string ROTAppartment { get; set; }
		#endregion
		#region ROTEstate
		public abstract class rOTEstate : IBqlField { }

		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "Real estate", FieldClass = RUTROTMessages.FieldClass)]
		public virtual string ROTEstate { get; set; }
		#endregion
		#region ROTOrganizationNbr
		public abstract class rOTOrganizationNbr : IBqlField { }

		/// <summary>
		/// The organization number for a ROT deductible document.
		/// This field is relevant only if <see cref="IsRUTROTDeductible"/> is <c>true</c> and <see cref="RUTROTType"/> is ROT (<c>"O"</c>).
		/// </summary>
		/// <value>
		/// The system validates the organization number according to the <see cref="Branch.RUTROTOrgNbrValidRegEx">regular expression</see>
		/// specified for the <see cref="BranchID">Branch</see> of the document.
		/// </value>
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXDBString(20)]
		[PXUIField(DisplayName = "Organization nbr.", FieldClass = RUTROTMessages.FieldClass, Visible = false)]
		[DynamicValueValidation(typeof(Search<BranchRUTROT.rUTROTOrgNbrValidRegEx,
											Where<Current<ARInvoiceRUTROT.isRUTROTDeductible>, Equal<boolTrue>,
											And<Current<RUTROT.rUTROTType>, Equal<RUTROTTypes.rot>,
											And<GL.Branch.branchID, Equal<Current<ARInvoice.branchID>>>>>>))]
		public virtual string ROTOrganizationNbr { get; set; }
		#endregion

		#region CuryOtherCost
		public abstract class curyOtherCost : IBqlField { }
		[PXUIField(DisplayName = "Other Cost", FieldClass = RUTROTMessages.FieldClass)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(ARInvoice.curyInfoID), typeof(RUTROT.otherCost))]
		public virtual Decimal? CuryOtherCost { get; set; }
		#endregion
		#region CuryMaterialCost
		public abstract class curyMaterialCost : IBqlField { }
		[PXUIField(DisplayName = "Material Cost", FieldClass = RUTROTMessages.FieldClass)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(ARInvoice.curyInfoID), typeof(RUTROT.materialCost))]
		public virtual Decimal? CuryMaterialCost { get; set; }
		#endregion
		#region CuryWorkPrice
		public abstract class curyWorkPrice : IBqlField { }
		[PXUIField(DisplayName = "Work Price", FieldClass = RUTROTMessages.FieldClass)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(ARInvoice.curyInfoID), typeof(RUTROT.workPrice))]
		public virtual Decimal? CuryWorkPrice { get; set; }
		#endregion
		#region OtherCost
		public abstract class otherCost : IBqlField { }
		[PXBaseCury()]
		public virtual Decimal? OtherCost { get; set; }
		#endregion
		#region MaterialCost
		public abstract class materialCost : IBqlField { }
		[PXBaseCury()]
		public virtual Decimal? MaterialCost { get; set; }
		#endregion
		#region WorkPrice
		public abstract class workPrice : IBqlField { }
		[PXBaseCury()]
		public virtual Decimal? WorkPrice { get; set; }
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
		#region tstamp
		public abstract class Tstamp : PX.Data.IBqlField
		{
		}
		protected Byte[] _tstamp;
		[PXDBTimestamp()]
		public virtual Byte[] tstamp
		{
			get
			{
				return this._tstamp;
			}
			set
			{
				this._tstamp = value;
			}
		}
		#endregion
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;

		/// <summary>
		/// Identifier of the <see cref="PX.Data.Note">Note</see> object, associated with the document.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="PX.Data.Note.NoteID">Note.NoteID</see> field. 
		/// </value>
		[PXNote()]
		public virtual Guid? NoteID
		{
			get
			{
				return this._NoteID;
			}
			set
			{
				this._NoteID = value;
			}
		}
		#endregion
	}
}
