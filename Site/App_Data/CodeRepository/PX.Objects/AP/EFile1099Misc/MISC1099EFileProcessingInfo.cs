using System;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.GL;
using PX.Objects.CM;
using Branch = PX.Objects.GL.Branch;

namespace PX.Objects.AP
{
	[Serializable]
	[PXPrimaryGraph(typeof(MISC1099EFileProcessing))]
	[PXProjection(typeof(Select5<AP1099History,
		InnerJoin<AP1099Box, On<AP1099Box.boxNbr, Equal<AP1099History.boxNbr>>,
		InnerJoin<Vendor, On<Vendor.bAccountID, Equal<AP1099History.vendorID>>,
		InnerJoin<Contact, On<Contact.bAccountID, Equal<Vendor.bAccountID>,
			And<Contact.contactID, Equal<Vendor.defContactID>>>,
		InnerJoin<Location, On<Location.bAccountID, Equal<Vendor.bAccountID>,
			And<Location.locationID, Equal<Vendor.defLocationID>>>,
		InnerJoin<Branch, On<AP1099History.branchID, Equal<Branch.branchID>>,
		InnerJoin<Ledger, On<Branch.ledgerID, Equal<Ledger.ledgerID>>,
		LeftJoin<BranchAlias, On<Ledger.defBranchID, Equal<BranchAlias.branchID>>>>>>>>>,
		Aggregate<
			GroupBy<Vendor.bAccountID,
			GroupBy<AP1099History.branchID,
			GroupBy<AP1099History.finYear,
			Sum<AP1099History.histAmt>>>>>>), Persistent = false)]
	public class MISC1099EFileProcessingInfoRaw : AP1099History
	{
		#region VAcctCD
		public abstract class vAcctCD : IBqlField {}

		[PXUIField(DisplayName = "Vendor")]
		[PXDBString(30, IsUnicode = true, InputMask = "", BqlField = typeof(Vendor.acctCD))]
		public virtual string VAcctCD { get; set; }
		#endregion

		#region VAcctName
		public abstract class vAcctName : IBqlField {}

		[PXDBString(60, IsUnicode = true, BqlField = typeof(Vendor.acctName))]
		[PXUIField(DisplayName = "Vendor Name")]
		public virtual string VAcctName { get; set; }
		#endregion

		#region LTaxRegistrationID
		public abstract class lTaxRegistrationID : IBqlField {}

		[PXDBString(50, IsUnicode = true, BqlField = typeof(Location.taxRegistrationID))]
		[PXUIField(DisplayName = "Tax Registration ID")]
		public virtual string LTaxRegistrationID { get; set; }
		#endregion

		#region ConsolidatedBranchID
		public abstract class consolidatedBranchID : IBqlField {}

		[Branch(null, BqlField = typeof(BranchAlias.branchID), DisplayName = "Consolidated Branch")]
		public virtual int? ConsolidatedBranchID { get; set; }
		#endregion

		#region PayerBranchID
		public abstract class payerBranchID : IBqlField {}

		[PXInt]
		[PXDimensionSelector(BranchAttribute._DimensionName, typeof(Search<Branch.branchID>), typeof(Branch.branchCD))]
		[PXUIField(DisplayName = "Payer")]
		[PXFormula(typeof(IsNull<MISC1099EFileProcessingInfoRaw.consolidatedBranchID, MISC1099EFileProcessingInfoRaw.branchID>))]
		public virtual int? PayerBranchID { get; set; }
		#endregion

		public new abstract class finYear : IBqlField { }
	}

	// TODO: Workaround awaiting AC-64107
	[Serializable]
	[PXPrimaryGraph(typeof(MISC1099EFileProcessing))]
	public class MISC1099EFileProcessingInfo : IBqlTable
	{
		#region Selected
		public abstract class selected : IBqlField {}

		[PXBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Selected")]
		public virtual bool? Selected { get; set; }
		#endregion

		#region BranchID
		public abstract class branchID : IBqlField {}

		[Branch(null)]
		public virtual int? BranchID { get; set; }
		#endregion

		#region VendorID
		public abstract class vendorID : IBqlField {}

		[PXInt(IsKey = true)]
		public virtual int? VendorID { get; set; }
		#endregion

		#region FinYear
		public abstract class finYear : IBqlField {}

		[PXString(4, IsKey = true, IsFixed = true)]
		public virtual string FinYear { get; set; }
		#endregion

		#region BoxNbr
		public abstract class boxNbr : IBqlField {}

		[PXShort(IsKey = true)]
		public virtual short? BoxNbr { get; set; }
		#endregion

		#region HistAmt
		public abstract class histAmt : IBqlField {}

		[PXBaseCury]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public virtual decimal? HistAmt { get; set; }
		#endregion

		#region VAcctCD
		public abstract class vAcctCD : IBqlField {}

		[PXUIField(DisplayName = "Vendor")]
		[PXString(30, IsUnicode = true, InputMask = "")]
		public virtual string VAcctCD { get; set; }
		#endregion

		#region VAcctName
		public abstract class vAcctName : IBqlField {}

		[PXString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Vendor Name")]
		public virtual string VAcctName { get; set; }
		#endregion

		#region LTaxRegistrationID
		public abstract class lTaxRegistrationID : IBqlField { }

		[PXString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "Tax Registration ID")]
		public virtual string LTaxRegistrationID { get; set; }
		#endregion

		#region PayerBranchID
		public abstract class payerBranchID : IBqlField { }

		[PXInt(IsKey = true)]
		[PXDimensionSelector(BranchAttribute._DimensionName, typeof(Search<Branch.branchID>), typeof(Branch.branchCD))]
		[PXUIField(DisplayName = "Payer")]
		public virtual int? PayerBranchID { get; set; }
		#endregion
	}
}
