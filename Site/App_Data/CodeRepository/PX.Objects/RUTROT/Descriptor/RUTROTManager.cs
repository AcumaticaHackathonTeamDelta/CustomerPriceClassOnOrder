using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Common;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.AR;
using PX.Objects.SO;
using PX.Objects.IN;
using PX.Objects.CM;

namespace PX.Objects.RUTROT
{
	public class RUTROTHelper
	{
		public static bool IsRUTROTcompatibleType(string type)
		{
			return (type == ARDocType.Invoice) || (type == ARDocType.DebitMemo) ||
					(type == SOOrderTypeConstants.QuoteOrder) || (type == SOOrderTypeConstants.SalesOrder);
		}
		public static bool CurrenciesMatch(BranchRUTROT branchRR, IRUTROTable document)
		{
			return branchRR?.RUTROTCuryID == document?.GetDocumentCuryID();
		}
		public static bool IsRUTROTAllowed(BranchRUTROT branchRR, IRUTROTable document)
		{
			return branchRR?.AllowsRUTROT == true && IsRUTROTcompatibleType(document?.GetDocumentType()) && CurrenciesMatch(branchRR, document);
		}
		public static bool IsItemMatchRUTROTType(string rutrotType, InventoryItem item, InventoryItemRUTROT itemRR, bool isRUTROTDeductible)
		{
			return !(item != null && !String.IsNullOrEmpty(itemRR.RUTROTType) && !String.IsNullOrEmpty(rutrotType) && itemRR.RUTROTType != rutrotType && isRUTROTDeductible);
		}
		public static bool IsUpToDateWorkType(int? workTypeID, DateTime currentDate, PXGraph graph)
		{
			RUTROTWorkType workType = PXSelect<RUTROTWorkType,
				Where<RUTROTWorkType.workTypeID, Equal<Required<RUTROTWorkType.workTypeID>>>>.Select(graph, workTypeID);
			return IsUpToDateWorkType(workType, currentDate);
		}
		public static bool IsUpToDateWorkType(RUTROTWorkType workType, DateTime currentDate)
		{
			if (workType == null)
				return false;
			return (workType.EndDate > currentDate || workType.EndDate==null) && workType.StartDate <=currentDate;
		}
		public static DocExt GetExtensionNullable<Doc, DocExt>(Doc doc)
			where Doc : class, IBqlTable, new()
			where DocExt : PXCacheExtension<Doc>
		{
			return doc == null ? null : PXCache<Doc>.GetExtension<DocExt>(doc);
		}
	}
	public class RUTROTManager<Doc, Tran, DocExt, TranExt>
		where Doc : class, IBqlTable, IInvoice, new()
		where Tran : class, IBqlTable, new()
		where DocExt : PXCacheExtension<Doc>, IRUTROTable
		where TranExt : PXCacheExtension<Tran>, IRUTROTableLine
	{
		const String IsRUTROTDeductible = "IsRUTROTDeductible";
		PXGraph Base;
		PXSelectBase<RUTROT> Rutrots;
		PXSelectBase<Doc> Document;
		PXSelectBase<Tran> Transactions;
		PXSelectBase<RUTROTDistribution> RRDistribution;
		PXSetup<ARSetup> ARSetup;
		PXSelectBase<CurrencyInfo> currencyinfo;
		public RUTROTManager(PXGraph graph, PXSelectBase<RUTROT> rutrot, PXSelectBase<RUTROTDistribution> distribution, PXSelectBase<Doc> document, PXSelectBase<Tran> transactions, PXSelectBase<CurrencyInfo> curyInfo)
		{
			this.Base = graph;
			Rutrots = rutrot;
			RRDistribution = distribution;
			Document = document;
			Transactions = transactions;
			currencyinfo = curyInfo;
			ARSetup = new PXSetup<AR.ARSetup>(graph);

			graph.RowSelected.AddHandler<RUTROTDistribution>(RUTROTDistribution_RowSelected);
			graph.RowInserted.AddHandler<RUTROT>(RUTROT_RowInserted);
			graph.RowUpdated.AddHandler<RUTROT>(RUTROT_RowUpdated);
			graph.RowPersisting.AddHandler<RUTROT>(RUTROT_RowPersisting);
			graph.FieldVerifying.AddHandler<RUTROT.rUTROTType>(RUTROT_RUTROTType_FieldVerifying);
			graph.FieldUpdated.AddHandler<RUTROT.rUTROTType>(RUTROT_RUTROTType_FieldUpdated);
		}
		#region Methods
		public void Update(Doc document)
		{
			if (document == null)
				return;
			RUTROT rutrot = Rutrots.Current ?? Rutrots.SelectSingle();
			DocExt documentRR = PXCache<Doc>.GetExtension<DocExt>(document);
			Branch branch = PXSelect<Branch, Where<Branch.branchID, Equal<Required<Branch.branchID>>>>.Select(Base, documentRR.GetDocumentBranchID());
			BranchRUTROT branchRR = RUTROTHelper.GetExtensionNullable<Branch, BranchRUTROT>(branch);
			PXErrorLevel errorLevel = documentRR.GetDocumentHold() != true ? PXErrorLevel.Error : PXErrorLevel.Warning;

			bool enableEdit = documentRR.GetRUTROTCompleted() != true && RUTROTHelper.CurrenciesMatch(branchRR, documentRR);
			bool showSection = documentRR.IsRUTROTDeductible == true;
			bool showROTSection = showSection && rutrot?.RUTROTType == RUTROTTypes.ROT;
			bool isAutoDistribution = rutrot?.AutoDistribution == true;

			UpdateRUTROTCheckbox(enableEdit, RUTROTHelper.IsRUTROTAllowed(branchRR, documentRR));
			UpdateRUTROTSection(showSection, enableEdit, showROTSection);
			UpdateDistributionControls(showSection, enableEdit, isAutoDistribution);
			SetPersistingChecks(documentRR, rutrot);
			WarnOnDeductionExceedsAllowance(documentRR, rutrot, errorLevel);
			WarnUndistributedAmount(documentRR, rutrot, errorLevel, currencyinfo.Current ?? currencyinfo.SelectSingle());
		}

		private void UpdateRUTROTCheckbox(bool enableEdit, bool showTick)
		{
			PXUIFieldAttribute.SetVisible(Document.Cache, null, IsRUTROTDeductible, showTick);
			PXUIFieldAttribute.SetEnabled(Document.Cache, null, IsRUTROTDeductible, enableEdit);
		}

		private void UpdateRUTROTSection(bool showSection, bool enableEdit, bool showROTSection)
		{
			PXUIFieldAttribute.SetVisible<RUTROT.rUTROTType>(Rutrots.Cache, null, showSection);
			PXUIFieldAttribute.SetVisible<RUTROT.curyOtherCost>(Rutrots.Cache, null, showSection);
			PXUIFieldAttribute.SetVisible<RUTROT.curyMaterialCost>(Rutrots.Cache, null, showSection);
			PXUIFieldAttribute.SetVisible<RUTROT.curyWorkPrice>(Rutrots.Cache, null, showSection);
			PXUIFieldAttribute.SetVisible<RUTROT.autoDistribution>(Rutrots.Cache, null, showSection);
			PXUIFieldAttribute.SetVisible<RUTROT.curyTotalAmt>(Rutrots.Cache, null, showSection);

			PXUIFieldAttribute.SetVisible<RUTROT.rOTAppartment>(Rutrots.Cache, null, showROTSection);
			PXUIFieldAttribute.SetVisible<RUTROT.rOTEstate>(Rutrots.Cache, null, showROTSection);
			PXUIFieldAttribute.SetVisible<RUTROT.rOTOrganizationNbr>(Rutrots.Cache, null, showROTSection);
			Rutrots.Cache.AllowUpdate = enableEdit;
		}

		private void UpdateDistributionControls(bool showSection, bool enableEdit, bool isAutoDistribution)
		{
			RRDistribution.Cache.AllowSelect = showSection;

			PXUIFieldAttribute.SetVisible<RUTROT.curyUndistributedAmt>(Rutrots.Cache, null, showSection && isAutoDistribution == false);
			PXUIFieldAttribute.SetVisible<RUTROT.curyDistributedAmt>(Rutrots.Cache, null, showSection && PXAccess.FeatureInstalled<PX.Objects.CS.FeaturesSet.invoiceRounding>() && isAutoDistribution);

			PXUIFieldAttribute.SetEnabled<RUTROTDistribution.curyAmount>(RRDistribution.Cache, null, isAutoDistribution == false);

			RRDistribution.Cache.AllowInsert = enableEdit;
			RRDistribution.Cache.AllowDelete = enableEdit;
			RRDistribution.Cache.AllowUpdate = enableEdit;
		}

		private void SetPersistingChecks(IRUTROTable document, RUTROT rutrot)
		{
			PXPersistingCheck check = document.IsRUTROTDeductible == true ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing;
			PXPersistingCheck rotCheck = document.IsRUTROTDeductible == true && rutrot?.RUTROTType == RUTROTTypes.ROT ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing;

			PXDefaultAttribute.SetPersistingCheck<RUTROT.rUTROTType>(Rutrots.Cache, rutrot, check);
			PXDefaultAttribute.SetPersistingCheck<RUTROT.rOTDeductionPct>(Rutrots.Cache, rutrot, check);
			PXDefaultAttribute.SetPersistingCheck<RUTROT.curyROTPersonalAllowance>(Rutrots.Cache, rutrot, check);
			PXDefaultAttribute.SetPersistingCheck<RUTROT.curyROTExtraAllowance>(Rutrots.Cache, rutrot, check);
			PXDefaultAttribute.SetPersistingCheck<RUTROT.rUTDeductionPct>(Rutrots.Cache, rutrot, check);
			PXDefaultAttribute.SetPersistingCheck<RUTROT.curyRUTPersonalAllowance>(Rutrots.Cache, rutrot, check);
			PXDefaultAttribute.SetPersistingCheck<RUTROT.curyRUTExtraAllowance>(Rutrots.Cache, rutrot, check);

			if (!String.IsNullOrWhiteSpace(rutrot?.ROTOrganizationNbr))
			{
				PXDefaultAttribute.SetPersistingCheck<RUTROT.rOTAppartment>(Rutrots.Cache, rutrot, rotCheck);
				PXDefaultAttribute.SetPersistingCheck<RUTROT.rOTEstate>(Rutrots.Cache, rutrot, PXPersistingCheck.Nothing);
			}
			else
			{
				PXDefaultAttribute.SetPersistingCheck<RUTROT.rOTEstate>(Rutrots.Cache, rutrot, rotCheck);
				PXDefaultAttribute.SetPersistingCheck<RUTROT.rOTAppartment>(Rutrots.Cache, rutrot, PXPersistingCheck.Nothing);
			}
		}

		public void RUTROTDeductibleUpdated(IRUTROTable row, IRUTROTable oldRow, RUTROT rutrot)
		{
			if (row != null && oldRow?.IsRUTROTDeductible != row.IsRUTROTDeductible)
			{
				if ((rutrot == null || rutrot.RefNbr == null) && row.IsRUTROTDeductible == true)
				{
					rutrot = (RUTROT)Rutrots.Insert(new RUTROT());
				}
				else if (row.IsRUTROTDeductible != true)
				{
					Rutrots.Delete(rutrot);
				}
			}
		}

		private void RedistributeDeduction(IRUTROTable document, RUTROT rutrot, ARSetup setup, CurrencyInfo curyInfo)
		{
			if (document == null || document.IsRUTROTDeductible != true || rutrot == null)
				return;

			var persons = RRDistribution.Select().ToList();
			int count = persons.Count;

			if (rutrot.AutoDistribution == true && count != 0)
			{
				decimal totalFromTrans = rutrot.CuryTotalAmt ?? 0.0m;


				var distributor = new DistributionRounding(setup, PXAccess.FeatureInstalled<PX.Objects.CS.FeaturesSet.invoiceRounding>())
				{ PreventOverflow = true, CuryPlaces = curyInfo?.CuryPrecision ?? 0 };
				var amts = distributor.DistributeEven(totalFromTrans, count);

				foreach (var p in persons.Zip(amts, (p, a) => new { DistributionItem = p, Amount = a }))
				{
					var item = (RUTROTDistribution)RRDistribution.Cache.CreateCopy((RUTROTDistribution)p.DistributionItem);
					if (item.CuryAmount != p.Amount)
					{
						item.CuryAmount = p.Amount;
						RRDistribution.Cache.Update(item);
					}
				}
			}

			RRDistribution.View.RequestRefresh();
		}

		public void UncheckRUTROTIfProhibited(IRUTROTable document, Branch branch)
		{
			if (branch == null || document == null)
				return;
			BranchRUTROT branchRR = PXCache<Branch>.GetExtension<BranchRUTROT>(branch);

			if (RUTROTHelper.IsRUTROTAllowed(branchRR, document) == false && document.IsRUTROTDeductible == true)
			{
				document.IsRUTROTDeductible = false;
				Document.Cache.Update(document.GetBaseDocument());
			}
		}

		private bool WarnUndistributedAmount(IRUTROTable document, RUTROT rutrot, PXErrorLevel errorLevel, CurrencyInfo currencyInfo)
		{
			if (document == null || rutrot == null || document.IsRUTROTDeductible != true)
				return false;

			decimal maxDiff = 0.0m;

			if (PXAccess.FeatureInstalled<PX.Objects.CS.FeaturesSet.invoiceRounding>() && currencyInfo != null)
			{
				var distributor = new DistributionRounding(ARSetup.Current, true) { CuryPlaces = currencyInfo.CuryPrecision ?? 0 };
				maxDiff = distributor.FinishStep;
			}

			Action<string> setNotification = null;
			if (errorLevel == PXErrorLevel.Error)
				setNotification = m => Rutrots.Cache.RaiseExceptionHandling<RUTROT.curyUndistributedAmt>(rutrot, rutrot.CuryUndistributedAmt, new PXSetPropertyException(m, errorLevel));
			else
				setNotification = m => PXUIFieldAttribute.SetWarning<RUTROT.curyUndistributedAmt>(Rutrots.Cache, rutrot, m);

			if (rutrot.CuryUndistributedAmt > maxDiff)
			{
				if (rutrot.AutoDistribution == true)
				{
					setNotification(RUTROTMessages.PositiveUndistributedAmount);
					return true;
				}
				else
				{
					PXUIFieldAttribute.SetWarning<RUTROT.curyUndistributedAmt>(Rutrots.Cache, rutrot, RUTROTMessages.PositiveUndistributedAmount);
					return false;
				}
			}
			else if (rutrot.CuryUndistributedAmt < 0.0m)
			{
				setNotification(RUTROTMessages.NegativeUndistributedAmount);
				return true;
			}
			else
			{
				Rutrots.Cache.RaiseExceptionHandling<RUTROT.curyUndistributedAmt>(rutrot, rutrot.CuryUndistributedAmt, null);
				return false;
			}
		}

		private void WarnPersonAmount(RUTROTDistribution personalAmount, RUTROT rutrot, IRUTROTable document)
		{
			if (document == null || rutrot == null || document.IsRUTROTDeductible != true || personalAmount == null)
				return;

			bool treatAsError = document.GetDocumentHold() != true;
			PXErrorLevel errorLevel = treatAsError ? PXErrorLevel.Error : PXErrorLevel.Warning;

			PXSetPropertyException<RUTROTDistribution.curyAmount> error = null;

			if (personalAmount.CuryAmount < 0.0m && rutrot.CuryTotalAmt != 0.0m)
			{
				error = new PXSetPropertyException<RUTROTDistribution.curyAmount>(RUTROTMessages.NonpositivePersonAmount, errorLevel);
			}
			else if (personalAmount.CuryAmount > personalAmount.CuryAllowance)
			{
				error = new PXSetPropertyException<RUTROTDistribution.curyAmount>(RUTROTMessages.PersonExceedsAllowance, PXErrorLevel.Error);
			}

			RRDistribution.Cache.RaiseExceptionHandling<RUTROTDistribution.curyAmount>(personalAmount, personalAmount.CuryAmount, error);
		}

		private bool WarnOnDeductionExceedsAllowance(IRUTROTable document, RUTROT rutrot, PXErrorLevel errorLevel)
		{
			if (document == null || rutrot == null || document.IsRUTROTDeductible != true || rutrot.CuryAllowedAmt == null)
				return false;

			Rutrots.Cache.RaiseExceptionHandling<RUTROT.curyTotalAmt>(rutrot, rutrot.CuryTotalAmt, null);

			Action<string> setNotification = null;
			if (errorLevel == PXErrorLevel.Error)
				setNotification = m => Rutrots.Cache.RaiseExceptionHandling<RUTROT.curyTotalAmt>(rutrot, rutrot.CuryTotalAmt, new PXSetPropertyException(m, errorLevel));
			else
				setNotification = m => PXUIFieldAttribute.SetWarning<RUTROT.curyTotalAmt>(Rutrots.Cache, rutrot, m);

			if (rutrot.CuryTotalAmt <= rutrot.CuryAllowedAmt)
			{
				return false;
			}
			else if (rutrot.CuryAllowedAmt > 0.0m)
			{
				PXUIFieldAttribute.SetWarning<RUTROT.curyTotalAmt>(Rutrots.Cache, rutrot, RUTROTMessages.DeductibleExceedsAllowance);
				return false;
			}
			else
			{
				setNotification(RUTROTMessages.PeopleAreRequiredForDeduction);
				return true;
			}
		}

		public void UpdateTranDeductibleFromInventory(IRUTROTableLine line, IRUTROTable document)
		{
			if (line == null)
				return;

			InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(Base, line.GetInventoryID());
			InventoryItemRUTROT itemRR = RUTROTHelper.GetExtensionNullable<InventoryItem, InventoryItemRUTROT>(item);

			if (itemRR == null)
				return;

			if (document?.IsRUTROTDeductible == true)
			{
				line.IsRUTROTDeductible = itemRR.IsRUTROTDeductible == true;
			}
			else
			{
				line.IsRUTROTDeductible = false;
			}

			if (itemRR.RUTROTItemType != null)
			{
				line.RUTROTItemType = itemRR.RUTROTItemType;
			}
			if (itemRR.RUTROTWorkTypeID != null)
			{
				line.RUTROTWorkTypeID = itemRR.RUTROTWorkTypeID;
			}
		}

		private void ClearROTFields(RUTROT rutrot)
		{
			if (rutrot != null && rutrot.RUTROTType == RUTROTTypes.RUT)
			{
				rutrot.ROTAppartment = null;
				rutrot.ROTEstate = null;
				rutrot.ROTOrganizationNbr = null;
			}
		}
		private void ClearWorkTypes(RUTROT rutrot)
		{
			foreach (Tran line in Transactions.Select())
			{
				TranExt lineRR = RUTROTHelper.GetExtensionNullable<Tran, TranExt>(line);
				RUTROTWorkType workType = PXSelect<RUTROTWorkType, Where<RUTROTWorkType.workTypeID, Equal<Required<RUTROTWorkType.workTypeID>>>>.Select(this.Base, lineRR.RUTROTWorkTypeID);
				if (workType?.RUTROTType != rutrot.RUTROTType)
				{
					lineRR.RUTROTWorkTypeID = null;
					Transactions.Update(line);
				}
			}
		}
		private void RecalcFormulas(RUTROT rutrot, DocExt document)
		{
			if (document.IsRUTROTDeductible == true)
			{
				foreach (Tran tran in Transactions.Select())
				{
					if (PXCache<Tran>.GetExtension<TranExt>(tran)?.IsRUTROTDeductible == true)
					{
						Transactions.Cache.RaiseFieldUpdated("IsRUTROTDeductible", tran, false);//this is required for updating formula on ARTranRUTROT.CuryRUTROTAvailableAmt
					}
				}
				foreach (RUTROTDistribution distribution in RRDistribution.Select())
				{
					RRDistribution.Cache.RaiseFieldUpdated<RUTROTDistribution.extra>(distribution, !distribution.Extra);//this is required for updating formula on RUTROTDistribution.curyAllowance
				}

				PXUnboundFormulaAttribute.CalcAggregate<ARTranRUTROT.curyRUTROTTotal>(Transactions.Cache, rutrot);
				PXFormulaAttribute.CalcAggregate<ARTranRUTROT.curyRUTROTAvailableAmt>(Transactions.Cache, rutrot);

				PXUnboundFormulaAttribute.CalcAggregate<SOLineRUTROT.curyRUTROTTotal>(Transactions.Cache, rutrot);
				PXFormulaAttribute.CalcAggregate<SOLineRUTROT.curyRUTROTAvailableAmt>(Transactions.Cache, rutrot);

				PXFormulaAttribute.CalcAggregate<RUTROTDistribution.curyAllowance>(RRDistribution.Cache, rutrot);
			}
		}
		#endregion
		#region Events
		#region RUTROTDistribution
		public void RUTROTDistribution_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			WarnPersonAmount(e.Row as RUTROTDistribution, Rutrots.Current, RUTROTHelper.GetExtensionNullable<Doc, DocExt>(Document.Current));
		}
		#endregion
		#region RUTROT
		protected virtual void RUTROT_RUTROTType_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			DocExt document = RUTROTHelper.GetExtensionNullable<Doc, DocExt>(Document.Current);
			RUTROT rutrot = (RUTROT)e.Row;
			ClearROTFields(rutrot);
			ClearWorkTypes(rutrot);
			RecalcFormulas(rutrot, document);
		
		}
		protected virtual void RUTROT_RUTROTType_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			DocExt document = RUTROTHelper.GetExtensionNullable<Doc, DocExt>(Document.Current);
			RUTROT rutrot = (RUTROT)e.Row;
			if (document != null && document.IsRUTROTDeductible == true)
			{
				string value = (string)e.NewValue;
				foreach (Tran tran in Transactions.Select())
				{
					TranExt tranRR = RUTROTHelper.GetExtensionNullable<Tran, TranExt>(tran);
					if (tranRR.GetInventoryID() != null)
					{
						InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(Base, tranRR.GetInventoryID());
						InventoryItemRUTROT itemRR = PXCache<InventoryItem>.GetExtension<InventoryItemRUTROT>(item);
						if (!RUTROTHelper.IsItemMatchRUTROTType(value, item, itemRR, itemRR?.IsRUTROTDeductible == true))
						{
							sender.RaiseExceptionHandling<RUTROT.rUTROTType>(rutrot, rutrot.RUTROTType, new PXSetPropertyException<RUTROT.rUTROTType>(RUTROTMessages.LineDoesNotMatchDoc));
							e.NewValue = rutrot.RUTROTType;
							break;
						}
					}
				}
			}
		}

		protected virtual void RUTROT_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			RUTROT rutrot = (RUTROT)e.Row;
			rutrot.CuryAllowedAmt = 0m;
			rutrot.CuryMaterialCost = 0m;
			rutrot.CuryOtherCost = 0m;
			rutrot.CuryWorkPrice = 0m;

			RecalcFormulas(rutrot, RUTROTHelper.GetExtensionNullable<Doc, DocExt>(Document.Current));
		}
		protected virtual void RUTROT_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			RedistributeDeduction(RUTROTHelper.GetExtensionNullable<Doc, DocExt>(Document.Current), (RUTROT)e.Row, ARSetup.Current, currencyinfo.Current);
		}
		protected virtual void RUTROT_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			RUTROT rutrot = (RUTROT)e.Row;
			DocExt doc = RUTROTHelper.GetExtensionNullable<Doc, DocExt>(Document.Current);

			bool treatAsError = doc?.GetDocumentHold() != true;
			PXErrorLevel errorLevel = treatAsError ? PXErrorLevel.Error : PXErrorLevel.Warning;
			if (WarnOnDeductionExceedsAllowance(doc, rutrot, errorLevel) && treatAsError)
				throw new PXSetPropertyException(Rutrots.Current.CuryAllowedAmt == 0.0m ? RUTROTMessages.PeopleAreRequiredForDeduction : RUTROTMessages.DeductibleExceedsAllowance, PXErrorLevel.Error);

			if (WarnUndistributedAmount(doc, rutrot, errorLevel, currencyinfo.Current ?? currencyinfo.SelectSingle()) && treatAsError)
				throw new PXSetPropertyException(RUTROTMessages.UndistributedAmount, PXErrorLevel.Error);
		}
		#endregion
		#endregion
	}
}
