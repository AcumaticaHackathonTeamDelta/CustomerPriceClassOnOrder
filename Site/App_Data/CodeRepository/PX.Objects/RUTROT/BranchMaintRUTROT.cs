using System;
using PX.Data;
using PX.Objects.CS;

namespace PX.Objects.RUTROT
{
	[Serializable]
	public class BranchMaintRUTROT : PXGraphExtension<BranchMaint>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.rutRotDeduction>();
		}

		#region Events Handlers

		public virtual void BranchBAccount_AllowsRUTROT_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			var branch = (BranchMaint.BranchBAccount)e.Row;
			if (branch == null)
				return;
			var branchRR = PXCache<BranchMaint.BranchBAccount>.GetExtension<BranchBAccountRUTROT>(branch);
			if (branchRR.AllowsRUTROT == true)
			{
				branchRR.RUTROTCuryID = Base.Company.Current.BaseCuryID;
				branchRR.RUTPersonalAllowanceLimit = 25000.0m;
				branchRR.RUTExtraAllowanceLimit = 50000.0m;
				branchRR.RUTDeductionPct = 50.0m;

				branchRR.ROTPersonalAllowanceLimit = 50000.0m;
				branchRR.ROTExtraAllowanceLimit = 50000.0m;
				branchRR.ROTDeductionPct = 30.0m;
				branchRR.RUTROTOrgNbrValidRegEx = "^(\\d{10})$";


				Base.BAccount.Cache.RaiseFieldUpdated<BranchBAccountRUTROT.rOTPersonalAllowanceLimit>(branch, 0.0m);
				Base.BAccount.Cache.RaiseFieldUpdated<BranchBAccountRUTROT.rOTExtraAllowanceLimit>(branch, 0.0m);
				Base.BAccount.Cache.RaiseFieldUpdated<BranchBAccountRUTROT.rOTDeductionPct>(branch, 0.0m);
				Base.BAccount.Cache.RaiseFieldUpdated<BranchBAccountRUTROT.rUTPersonalAllowanceLimit>(branch, 0.0m);
				Base.BAccount.Cache.RaiseFieldUpdated<BranchBAccountRUTROT.rUTExtraAllowanceLimit>(branch, 0.0m);
				Base.BAccount.Cache.RaiseFieldUpdated<BranchBAccountRUTROT.rUTDeductionPct>(branch, 0.0m);
			}
			else
			{
				branchRR.ROTDeductionPct = 0.0m;
				branchRR.ROTPersonalAllowanceLimit = 0.0m;
				branchRR.ROTExtraAllowanceLimit = 0.0m;
				branchRR.RUTDeductionPct = 0.0m;
				branchRR.RUTPersonalAllowanceLimit = 0.0m;
				branchRR.RUTExtraAllowanceLimit = 0.0m;
				branchRR.RUTROTCuryID = null;
			}
		}

		public virtual void BranchBAccount_RUTROTClaimNextRefNbr_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
		{
			var branch = (BranchMaint.BranchBAccount)e.Row;

			if (branch == null || PXAccess.FeatureInstalled<FeaturesSet.rutRotDeduction>() == false || PXCache<BranchMaint.BranchBAccount>.GetExtension<BranchBAccountRUTROT>(branch).AllowsRUTROT != true)
				return;

			int newValue = (int?)e.NewValue ?? 0;
			int oldValue = PXCache<BranchMaint.BranchBAccount>.GetExtension<BranchBAccountRUTROT>(branch).RUTROTClaimNextRefNbr ?? 0;

			if (newValue < oldValue)
			{
				PXUIFieldAttribute.SetWarning<BranchBAccountRUTROT.rUTROTClaimNextRefNbr>(Base.BAccount.Cache, branch, RUTROTMessages.ClaimNextRefDecreased);
			}
		}

		public virtual void BranchBAccount_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			UpdateRUTROTControlsState();
		}

		private void UpdateRUTROTControlsState()
		{
			var branchBAcct = Base.CurrentBAccount.Current;
			var branchBAcctCache = Base.CurrentBAccount.Cache;

			var showRUTROTFields = PXCache<BranchMaint.BranchBAccount>.GetExtension<BranchBAccountRUTROT>(branchBAcct).AllowsRUTROT == true;

			PXUIFieldAttribute.SetEnabled<BranchBAccountRUTROT.rUTROTCuryID>(branchBAcctCache, branchBAcct, showRUTROTFields);
			PXUIFieldAttribute.SetEnabled<BranchBAccountRUTROT.rOTPersonalAllowanceLimit>(branchBAcctCache, branchBAcct, showRUTROTFields);
			PXUIFieldAttribute.SetEnabled<BranchBAccountRUTROT.rOTExtraAllowanceLimit>(branchBAcctCache, branchBAcct, showRUTROTFields);
			PXUIFieldAttribute.SetEnabled<BranchBAccountRUTROT.rOTDeductionPct>(branchBAcctCache, branchBAcct, showRUTROTFields);
			PXUIFieldAttribute.SetEnabled<BranchBAccountRUTROT.rUTPersonalAllowanceLimit>(branchBAcctCache, branchBAcct, showRUTROTFields);
			PXUIFieldAttribute.SetEnabled<BranchBAccountRUTROT.rUTExtraAllowanceLimit>(branchBAcctCache, branchBAcct, showRUTROTFields);
			PXUIFieldAttribute.SetEnabled<BranchBAccountRUTROT.rUTDeductionPct>(branchBAcctCache, branchBAcct, showRUTROTFields);
			PXUIFieldAttribute.SetEnabled<BranchBAccountRUTROT.rUTROTClaimNextRefNbr>(branchBAcctCache, branchBAcct, showRUTROTFields);

			var persistingCheck = showRUTROTFields ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing;

			PXDefaultAttribute.SetPersistingCheck<BranchBAccountRUTROT.rUTROTCuryID>(branchBAcctCache, branchBAcct, persistingCheck);
			PXDefaultAttribute.SetPersistingCheck<BranchBAccountRUTROT.rOTPersonalAllowanceLimit>(branchBAcctCache, branchBAcct, persistingCheck);
			PXDefaultAttribute.SetPersistingCheck<BranchBAccountRUTROT.rOTExtraAllowanceLimit>(branchBAcctCache, branchBAcct, persistingCheck);
			PXDefaultAttribute.SetPersistingCheck<BranchBAccountRUTROT.rOTDeductionPct>(branchBAcctCache, branchBAcct, persistingCheck);
			PXDefaultAttribute.SetPersistingCheck<BranchBAccountRUTROT.rUTPersonalAllowanceLimit>(branchBAcctCache, branchBAcct, persistingCheck);
			PXDefaultAttribute.SetPersistingCheck<BranchBAccountRUTROT.rUTExtraAllowanceLimit>(branchBAcctCache, branchBAcct, persistingCheck);
			PXDefaultAttribute.SetPersistingCheck<BranchBAccountRUTROT.rUTDeductionPct>(branchBAcctCache, branchBAcct, persistingCheck);
			PXDefaultAttribute.SetPersistingCheck<BranchBAccountRUTROT.rUTROTCuryID>(branchBAcctCache, branchBAcct, persistingCheck);
		}
		#endregion
	}
}
