using System;
using PX.Data;
using System.Collections;
using System.Collections.Generic;
using PX.Objects.CM;
using PX.Objects.CS;

namespace PX.Objects.GL
{
	public class GeneralLedgerMaint : PXGraph<GeneralLedgerMaint>
    {
        public PXSavePerRow<Ledger, Ledger.ledgerID> Save;
		public PXCancel<Ledger> Cancel;		
		[PXImport(typeof(Ledger))]
		public PXSelect<Ledger> LedgerRecords;
		public PXSetup<Company> company;

		public GeneralLedgerMaint()
		{
			var mcFeatureInstalled = PXAccess.FeatureInstalled<FeaturesSet.multicurrency>();

			PXUIFieldAttribute.SetVisible<Ledger.baseCuryID>(LedgerRecords.Cache, null, mcFeatureInstalled);
			PXUIFieldAttribute.SetEnabled<Ledger.baseCuryID>(LedgerRecords.Cache, null, mcFeatureInstalled);
		}

		protected virtual void Ledger_BaseCuryID_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			Ledger ledger = e.Row as Ledger;
			if (ledger != null && ledger.LedgerID != null && ledger.BaseCuryID != null)
			{
				if (GLUtility.IsLedgerHistoryExist(this, (int)ledger.LedgerID))
				{
					throw new PXSetPropertyException(Messages.CantChangeField, "BaseCuryID");
				}

                if (ledger.BalanceType == LedgerBalanceType.Actual && company.Current.BaseCuryID != (string)e.NewValue)
                {
                    throw new PXSetPropertyException(Messages.ActualLedgerInBaseCurrency, ledger.LedgerCD, company.Current.BaseCuryID);
                }
			}
		}

        protected virtual void Ledger_BalanceType_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			Ledger ledger = e.Row as Ledger;
			if (ledger != null && ledger.LedgerID != null && ledger.CreatedByID != null)
			{
				if (GLUtility.IsLedgerHistoryExist(this, (int)ledger.LedgerID))
				{
					throw new PXSetPropertyException(Messages.CantChangeField, "BalanceType");
				}
            
                if ((string)e.NewValue == LedgerBalanceType.Actual && company.Current.BaseCuryID != ledger.BaseCuryID)
                {
                    throw new PXSetPropertyException(Messages.ActualLedgerInBaseCurrency, ledger.LedgerCD, company.Current.BaseCuryID);
                }
            }
		}

		protected virtual void Ledger_BalanceType_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if (((Ledger)e.Row).BalanceType != LedgerBalanceType.Actual)
			{
				sender.SetValue<Ledger.postInterCompany>(e.Row, false);
				sender.SetValue<Ledger.defBranchID>(e.Row, null);
			}
			else
			{
				sender.SetValue<Ledger.postInterCompany>(e.Row, true);
			}
		}

		protected virtual void Ledger_DefBranchID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			Ledger ledger = e.Row as Ledger;

			if (ledger.BalanceType == LedgerBalanceType.Actual)
			{
				if (ledger.DefBranchID == null && e.OldValue != null)
				{
					sender.SetValue<Ledger.postInterCompany>(e.Row, true);
				}
				else if (!PXAccess.FeatureInstalled<CS.FeaturesSet.interBranch>() && ledger.DefBranchID != null && e.OldValue == null)
				{
					sender.SetValue<Ledger.postInterCompany>(e.Row, false);
				}
			}
		}

		protected virtual void Ledger_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			Ledger ledger = e.Row as Ledger;
			if (ledger != null && ledger.LedgerID.HasValue)
			{
				//Type and Currency are forbidden for changses for the used accounts
				bool hasHistory = GLUtility.IsLedgerHistoryExist(this, ledger.LedgerID);
				PXUIFieldAttribute.SetEnabled<Ledger.balanceType>(LedgerRecords.Cache, ledger, !hasHistory);
				PXUIFieldAttribute.SetEnabled<Ledger.baseCuryID>(LedgerRecords.Cache, ledger, !hasHistory && PXAccess.FeatureInstalled<FeaturesSet.multicurrency>());
			}

			if (ledger != null)
			{
				PXUIFieldAttribute.SetEnabled<Ledger.postInterCompany>(cache, e.Row, ledger.BalanceType == LedgerBalanceType.Actual && ledger.DefBranchID!=null);
				PXUIFieldAttribute.SetEnabled<Ledger.defBranchID>(cache, e.Row, ledger.BalanceType == LedgerBalanceType.Actual);

				if (ledger.BalanceType == LedgerBalanceType.Actual && ledger.DefBranchID!=null)
				{
					using (new PXReadBranchRestrictedScope())
					{
						GLHistory hist = PXSelectReadonly<GLHistory, Where<GLHistory.ledgerID, Equal<Current<Ledger.ledgerID>>, And<GLHistory.branchID, Equal<Current<Ledger.defBranchID>>>>>.SelectWindowed(this, 0, 1);
						PXResultset<GLHistory> hist2 = PXSelectGroupBy<GLHistory, Where<GLHistory.ledgerID, Equal<Current<Ledger.ledgerID>>>, Aggregate<GroupBy<GLHistory.branchID>>>.Select(this);

						PXUIFieldAttribute.SetEnabled<Ledger.postInterCompany>(cache, e.Row, (hist2.Count <= 1));
						PXUIFieldAttribute.SetEnabled<Ledger.defBranchID>(cache, e.Row, (hist == null));
					}
				}
			}
		}

		protected virtual void Ledger_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			Ledger ledger = (Ledger)e.Row;
			if (ledger.DefBranchID != null && (e.Operation == PXDBOperation.Insert || e.Operation == PXDBOperation.Update))
			{
				object BranchCD = sender.GetValueExt<Ledger.defBranchID>(e.Row);
				if (BranchCD is PXFieldState)
				{
					BranchCD = ((PXFieldState)BranchCD).Value;
				}

				IN.INSite site;
				if ((site = PXSelect<IN.INSite, Where<IN.INSite.branchID, Equal<Required<IN.INSite.branchID>>>>.SelectWindowed(this, 0, 1, ledger.DefBranchID)) != null)
				{
					sender.RaiseExceptionHandling<Ledger.defBranchID>(e.Row, BranchCD, new PXSetPropertyException(Messages.BranchUsedWithSite, site.SiteCD));
				}

				FA.FixedAsset asset;
				if ((asset = PXSelect<FA.FixedAsset, Where<FA.FixedAsset.branchID, Equal<Required<FA.FixedAsset.branchID>>>>.SelectWindowed(this, 0, 1, ledger.DefBranchID)) != null)
				{
					sender.RaiseExceptionHandling<Ledger.defBranchID>(e.Row, BranchCD, new PXSetPropertyException(Messages.BranchUsedWithFixedAsset, asset.AssetCD));
				}
			}
		}

		protected virtual void Ledger_RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
		{
			Ledger ledger = (Ledger)e.Row;
			if ((e.Operation == PXDBOperation.Insert || e.Operation == PXDBOperation.Update) && e.TranStatus == PXTranStatus.Open)
			{
				if (ledger.BalanceType == LedgerBalanceType.Actual && ledger.DefBranchID != null)
				{
					PXDatabase.Update<Branch>(
						new PXDataFieldAssign<Branch.ledgerID>(ledger.LedgerID),
						new PXDataFieldRestrict<Branch.branchID>(ledger.DefBranchID),
						new PXDataFieldRestrict<Branch.ledgerID>(PXDbType.Int, 4, null, PXComp.ISNULL));
				}

				//if only single branch exists
				if (ledger.BalanceType == LedgerBalanceType.Actual && (CS.BranchMaint.BranchBAccount)PXSelectJoin<CS.BranchMaint.BranchBAccount,
					InnerJoin<GL.Branch, On<GL.Branch.bAccountID, NotEqual<CS.BranchMaint.BranchBAccount.bAccountID>>>>.Select(this) == null)
				{
					PXUpdateJoin<
						Set<Branch.ledgerID, Required<Branch.ledgerID>>,
					Branch, LeftJoin<Ledger, On<Ledger.ledgerID, Equal<Branch.ledgerID>>>,
					Where<Ledger.ledgerID, IsNull>>.Update(this, ledger.LedgerID);
				}
			}
		}
	}
}
