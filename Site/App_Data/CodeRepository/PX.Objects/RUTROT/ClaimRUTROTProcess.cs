using System;
using System.Collections.Generic;
using System.Linq;

using PX.SM;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.GL;

namespace PX.Objects.RUTROT
{
	public class ClaimRUTROTProcess : PXGraph<ClaimRUTROTProcess>
	{
		#region Selects

		public PXSave<ARInvoice> Save;
		public PXSelect<ARInvoice> Documents;
		public PXSelect<RUTROT, Where<RUTROT.docType, Equal<Required<ARInvoice.docType>>,
				And<RUTROT.refNbr, Equal<Required<ARInvoice.refNbr>>>>> Rutrots;
		public PXSelect<ARTran,
				Where<ARTran.tranType, Equal<Required<ARInvoice.docType>>,
				And<ARTran.refNbr, Equal<Required<ARInvoice.refNbr>>>>> Lines;
		public PXSelect<RUTROTDistribution,
				Where<RUTROTDistribution.docType, Equal<Required<ARInvoice.docType>>,
				And<RUTROTDistribution.refNbr, Equal<Required<ARInvoice.refNbr>>>>> Distribution;
		public PXSelectJoin<ARPayment,
							InnerJoin<ARAdjust, On<ARPayment.docType, Equal<ARAdjust.adjgDocType>, And<ARPayment.refNbr, Equal<ARAdjust.adjgRefNbr>>>,
							InnerJoin<ARInvoice, On<ARAdjust.adjdDocType, Equal<ARInvoice.docType>, And<ARAdjust.adjdRefNbr, Equal<ARInvoice.refNbr>>>>>,
							Where<ARInvoice.docType, Equal<Required<ARInvoice.docType>>,
							And<ARInvoice.refNbr, Equal<Required<ARInvoice.refNbr>>,
							And<ARPayment.released, Equal<True>>>>> Payments;
		public PXSelect<GL.Branch, Where<GL.Branch.branchID, Equal<Required<GL.Branch.branchID>>>> CurrentBranch;

		#endregion

		#region Internal Types

		public class DocumentDetails
		{
			public int ClaimNbr { get; set; }

			public ARInvoice Document { get; set; }

			public RUTROT Rutrot { get; set; }

			public IEnumerable<RUTROTDistribution> Distribution { get; set; }

			public IEnumerable<ARTran> Lines { get; set; }
			public IEnumerable<Tuple<ARPayment, ARAdjust>> Payments { get; set; }

			public DistributionRounding Distributor { get; set; }

			private List<decimal> _payDistribution;
			public IEnumerable<decimal> PaymentDistribution
			{
				get
				{
					if (_payDistribution == null)
					{
						_payDistribution = DistributePayment();
					}
					return _payDistribution;
				}
			}

			private List<decimal> DistributePayment()
			{
				decimal totalClaim = Rutrot.CuryDistributedAmt ?? 1.0m;
				var shares = Distribution.Select(d => (d.CuryAmount ?? 0.0m) / totalClaim);
				decimal pct = 0.01m * (Rutrot.DeductionPct.Value);

				decimal rrToPay = (1.0m - pct) * (Rutrot.CuryTotalAmt ?? 0.0m) / pct;

				decimal relevantPay = Math.Min(TotalPay, rrToPay);

				return Distributor.DistributeInShares(relevantPay, shares).ToList();
			}

			public decimal TotalPay
			{
				get
				{
					if (Payments.Select(p => p.Item1.CuryID).All(c => c == Document.CuryID))
						return Payments.Sum(p => p.Item2.CuryAdjgAmt ?? 0.0m);
					else
						return Payments.Sum(p => p.Item2.CuryAdjdAmt ?? 0.0m);
				}
			}
		}
		#endregion

		public void ClaimDocuments(IEnumerable<ARInvoice> documents)
		{
			var extDocuments = documents.Select(GetExtendedDoc);
			ValidateExported(extDocuments);

			string filename = CreateFilename();

			foreach (var d in extDocuments)
			{
				ClaimDocument(d.Rutrot, filename);
			}

			this.Save.Press();

			var file = ExportDocuments(extDocuments, filename);

			throw new PXRedirectToFileException(file, true);
		}

		public void ExportDocuments(IEnumerable<ARInvoice> documents)
		{
			var extDocuments = documents.Select(GetExtendedDoc);
			ValidateExported(extDocuments);

			string filename = CreateFilename();
			var file = ExportDocuments(extDocuments, filename);

			throw new PXRedirectToFileException(file, true);
		}

		public void ClaimDocument(RUTROT rutrot, string filename)
		{
			rutrot.IsClaimed = true;
			Rutrots.Cache.Update(rutrot);
		}

		#region Export
		private string CreateFilename()
		{
			string name = String.Format("ROT & RUT Export {0:000}{1:yy-hhmmss}.xml", DateTime.Now.DayOfYear, DateTime.Now);
			return String.Join("", name.Where(c => !"\\/:".Contains(c)));
		}

		private int GetClaimNumber()
		{
			var branch = CurrentBranch.SelectSingle(PXAccess.GetBranchID());
			if (branch == null)
				return 0;
			return PXCache<GL.Branch>.GetExtension<BranchRUTROT>(branch).RUTROTClaimNextRefNbr ?? 0;
		}

		private void UpdateClaimNumber()
		{
			var branch = CurrentBranch.SelectSingle(PXAccess.GetBranchID());
			if (branch == null)
				return;

			PXCache<GL.Branch>.GetExtension<BranchRUTROT>(branch).RUTROTClaimNextRefNbr += 1;
			CurrentBranch.Cache.Update(branch);
		}

		private void UpdateInvoiceExportNumbers(int exportNbr, IEnumerable<RUTROT> documents)
		{
			foreach (var doc in documents)
			{
				var copy = (RUTROT)Rutrots.Cache.CreateCopy(doc);
				copy.ExportRefNbr = exportNbr;
				Rutrots.Cache.Update(copy);
			}
		}

		private FileInfo ExportDocuments(IEnumerable<DocumentDetails> documents, string filename)
		{
			var file = new FileInfo(Guid.NewGuid(), filename, null, CreateExportContents(documents));

			var saver = PXGraph.CreateInstance<PX.SM.UploadFileMaintenance>();
			if (saver.SaveFile(file) || file.UID == null)
			{
				AttachFileToDocuments(file.UID.Value, documents.Select(d => d.Document));
			}
			else
				throw new PXException(RUTROTMessages.FailedToExport);

			foreach (var d in documents)
			{
				d.Rutrot.ClaimDate = DateTime.Today;
				d.Rutrot.ClaimFileName = filename;

				Documents.Cache.Update(d.Document);
			}

			UpdateInvoiceExportNumbers(documents.First().ClaimNbr, documents.Select(d => d.Rutrot));
			UpdateClaimNumber();

			this.Save.Press();

			return file;
		}

		#region Validation
		public void ValidateExported(IEnumerable<DocumentDetails> documents)
		{
			if (!documents.Any())
				throw new PXException(RUTROTMessages.NoDocumentsSelected);

			int buyersCount = documents.SelectMany(d => d.Distribution.Select(r => r.PersonalID)).Distinct().Count();

			if (buyersCount < 1)
				throw new PXException(RUTROTMessages.AtLeastOneBuyerMustBeMentioned);

			if (buyersCount > 100)
				throw new PXException(RUTROTMessages.NoMoreThan100Buyers);

			ValidateDates(documents);
			ValidateAmounts(documents);
		}

		private void ValidateDates(IEnumerable<DocumentDetails> documents)
		{
			var invoiceDates = documents.Select(d => d.Document.DocDate).Where(date => date != null).Select(date => date.Value);
			int minYear = DateTime.Today.Month > 1 ? DateTime.Today.Year : DateTime.Today.Year - 1;

			bool hasIncorrectDates = false;

			for (int i = 0; i < documents.Count(); i++)
			{
				var inv = documents.ElementAt(i).Document;
				if (inv.DocDate == null)
				{
					hasIncorrectDates = true;
					PXProcessing.SetError(i, new PXException(RUTROTMessages.DateShouldBeSpecifiedOnDocument));
				}

				if (inv.DocDate.Value.Year < minYear)
				{
					hasIncorrectDates = true;
					PXProcessing.SetError(i, new PXException(RUTROTMessages.DocumentDateIsBelowAllowed, new DateTime(minYear, 1, 1)));
				}
			}

			if (hasIncorrectDates)
				throw new PXException(RUTROTMessages.SomeDocumentDatesIncorrect);

			if (invoiceDates.Select(d => d.Year).Distinct().Count() > 1)
				throw new PXException(RUTROTMessages.AllDocumentsMustBeSameYear);

			bool hasTooLatePayments = false;

			for (int i = 0; i < documents.Count(); i++)
			{
				foreach (var pay in documents.ElementAt(i).Payments.Select(p => p.Item2))
				{
					if (pay.AdjgDocDate > DateTime.Today)
					{
						hasTooLatePayments = true;
						PXProcessing.SetError(i, new PXException(RUTROTMessages.PaymentDatesMustNotExceedClaimDate));
						break;
					}
				}
			}

			if (hasTooLatePayments)
				throw new PXException(RUTROTMessages.PaymentDatesMustNotExceedClaimDate);
		}

		private void ValidateAmounts(IEnumerable<DocumentDetails> documents)
		{
			bool hasAmountProblems = false;

			for (int i = 0; i < documents.Count(); i++)
			{
				decimal paid = documents.ElementAt(i).TotalPay;
				decimal claim = documents.ElementAt(i).Rutrot.CuryDistributedAmt ?? 0.0m;

				decimal maxDiff = GetDistributor(documents.ElementAt(i).Document.CuryInfoID).FinishStep;
				decimal deductionPct = (documents.ElementAt(i).Rutrot.RUTROTType == RUTROTTypes.RUT ?
					(documents.ElementAt(i).Rutrot.RUTDeductionPct) :
					(documents.ElementAt(i).Rutrot.ROTDeductionPct)) ?? 0.0m;
				deductionPct *= 0.01m;

				decimal minPay = claim / deductionPct * (1 - deductionPct);
				if (minPay - paid > maxDiff)
				{
					hasAmountProblems = true;
					PXProcessing.SetError(i, new PXException(RUTROTMessages.PaymentMustCoverDeductible));
				}

				if (claim + paid - (documents.ElementAt(i).Document.CuryOrigDocAmt ?? 0.0m) > maxDiff)
				{
					hasAmountProblems = true;
					PXProcessing.SetError(i, new PXException(RUTROTMessages.ClaimedPaidMustNotExceedTotal));
				}
			}

			if (hasAmountProblems)
				throw new PXException(RUTROTMessages.SomeAmountsIncorrect);

			decimal totalClaimAmt = documents.Sum(d => d.Rutrot.CuryTotalAmt ?? 0.0m);
			if (totalClaimAmt > 50000)
				throw new PXException(RUTROTMessages.ClaimedTotalTooMuch);
		}
		#endregion

		protected virtual byte[] CreateExportContents(IEnumerable<DocumentDetails> documents)
		{
			bool isRut = documents.First().Rutrot.RUTROTType == RUTROTTypes.RUT;
			List<RUTROTWorkType> availableWorkTypes = new List<RUTROTWorkType>();
			foreach (RUTROTWorkType workType in PXSelectOrderBy<RUTROTWorkType, OrderBy<Asc<RUTROTWorkType.rUTROTType, Asc<RUTROTWorkType.position>>>>.Select(this))
			{
				if(RUTROTHelper.IsUpToDateWorkType(workType, DateTime.Now))
					availableWorkTypes.Add(workType);
			}
			var exporter = isRut ? new RUTExport(availableWorkTypes) : new ROTExport(availableWorkTypes);

			return exporter.Export(documents);
		}

		private void AttachFileToDocuments(Guid fileId, IEnumerable<ARInvoice> documents)
		{
			var fcache = this.Caches[typeof(NoteDoc)];
			foreach (var d in documents)
			{
				var fileNote = new NoteDoc { NoteID = d.NoteID, FileID = fileId };
				fcache.Insert(fileNote);

				this.Persist(typeof(NoteDoc), PXDBOperation.Insert);
			}
		}
		#endregion

		#region Getting Doc Details
		private DocumentDetails GetExtendedDoc(ARInvoice inv)
		{
			return new DocumentDetails
			{
				ClaimNbr = GetClaimNumber(),
				Document = inv,
				Rutrot = Rutrots.Select(inv.DocType, inv.RefNbr),
				Distribution = Distribution.Select(inv.DocType, inv.RefNbr).Select(r => (RUTROTDistribution)r).ToList(),
				Lines = GetLines(inv),
				Payments = GetPayments(inv),
				Distributor = GetDistributor(inv.CuryInfoID)
			};
		}

		private IEnumerable<ARTran> GetLines(ARInvoice invoice)
		{
			var trans = Lines.Select(invoice.DocType, invoice.RefNbr).Select(r => (ARTran)r);
			return trans.ToList();
		}

		private IEnumerable<Tuple<ARPayment, ARAdjust>> GetPayments(ARInvoice invoice)
		{
			var payments = Payments.Select(invoice.DocType, invoice.RefNbr);
			return payments.Select(p => new Tuple<ARPayment, ARAdjust>((ARPayment)p, p.GetItem<ARAdjust>())).ToList();
		}
		#endregion

		private DistributionRounding GetDistributor(long? curyInfoID)
		{
			var arsetup = new PXSelect<ARSetup>(this).SelectSingle();
			var curyInfo = new PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Required<ARInvoice.curyInfoID>>>>(this).SelectSingle(curyInfoID);

			if (curyInfo == null)
				return new DistributionRounding(arsetup, PXAccess.FeatureInstalled<PX.Objects.CS.FeaturesSet.invoiceRounding>()) { CuryPlaces = 0, PreventOverflow = true };
			else
				return new DistributionRounding(arsetup, PXAccess.FeatureInstalled<PX.Objects.CS.FeaturesSet.invoiceRounding>()) { CuryPlaces = curyInfo.CuryPrecision ?? 0, PreventOverflow = true };
		}
	}
}
