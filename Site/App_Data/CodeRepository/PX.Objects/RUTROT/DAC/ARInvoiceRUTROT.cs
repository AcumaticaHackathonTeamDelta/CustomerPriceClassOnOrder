using System;
using PX.Common;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.CS;
using PX.Objects.AR;

namespace PX.Objects.RUTROT
{
	public class ARInvoiceRUTROT : PXCacheExtension<ARInvoice>, IRUTROTable
	{
		#region IsRUTROTDeductible
		public abstract class isRUTROTDeductible : IBqlField { }

		/// <summary>
		/// When set to <c>true</c> indicates that the document is subjected to ROT and RUT deductions.
		/// This field is relevant only if the <see cref="FeaturesSet.RutRotDeduction">ROT and RUT Deduction</see> feature is enabled,
		/// the value of the <see cref="Branch.AllowsRUTROT"/> field is <c>true</c> for the <see cref="DocumentBranchID">Branch of the document</see>
		/// and the document is an invoice (see <see cref="DocumentType"/>).
		/// </summary>
		/// <value>
		/// Defaults to <c>false</c>.
		/// </value>
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "ROT and RUT deductible document", FieldClass = RUTROTMessages.FieldClass, Visible = false)]
		public virtual bool? IsRUTROTDeductible { get; set; }
		#endregion
		public bool? GetRUTROTCompleted() { return Base.Released; }
		public string GetDocumentNbr() { return Base.RefNbr; }
		public string GetDocumentType() { return Base.DocType; }
		public int? GetDocumentBranchID() { return Base.BranchID; }
		public string GetDocumentCuryID() { return Base.CuryID; }
		public bool? GetDocumentHold() { return Base.Hold; }
		public IBqlTable GetBaseDocument() { return Base; }
	}
}
