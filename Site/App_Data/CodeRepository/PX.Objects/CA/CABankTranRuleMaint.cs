using PX.Data;
using System;

namespace PX.Objects.CA
{
	public class CABankTranRuleMaint : PXGraph<CABankTranRuleMaint, CABankTranRule>
	{
		public PXSelect<CABankTranRule> Rule;

		protected virtual void CABankTranRule_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
		{
			var rule = e.Row as CABankTranRule;
			if (String.IsNullOrWhiteSpace(rule.BankTranDescription) && rule.CuryTranAmt == null && String.IsNullOrWhiteSpace(rule.TranCode))
			{
				throw new PXException(Messages.BankRuleTooLoose);
			}

			if (rule.BankTranCashAccountID != null && rule.DocumentEntryTypeID != null)
			{
				CashAccountETDetail entryTypeForAcct = PXSelect<CashAccountETDetail,
					Where<CashAccountETDetail.accountID, Equal<Required<CashAccountETDetail.accountID>>,
						And<CashAccountETDetail.entryTypeID, Equal<Required<CashAccountETDetail.entryTypeID>>>>>
						.Select(this, rule.BankTranCashAccountID, rule.DocumentEntryTypeID);

				if (entryTypeForAcct == null)
				{
					cache.RaiseExceptionHandling<CABankTranRule.documentEntryTypeID>(rule, rule.DocumentEntryTypeID,
						new PXSetPropertyException<CABankTranRule.documentEntryTypeID>(Messages.BankRuleEntryTypeDoesntSuitCashAccount));
				}
			}
		}

		protected virtual void CABankTranRule_RowDeleting(PXCache cache, PXRowDeletingEventArgs e)
		{
			var rule = e.Row as CABankTranRule;
			CABankTran referer = PXSelect<CABankTran, Where<CABankTran.ruleID, Equal<Required<CABankTran.ruleID>>>>.Select(this, rule.RuleID);

			if (referer != null)
				throw new PXException(Messages.BankRuleInUseCantDelete);
		}

		protected virtual void CABankTranRule_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			var rule = e.Row as CABankTranRule;
			if (rule == null)
				return;

			bool isCreateDoc = rule.Action == RuleAction.CreateDocument;

			PXDefaultAttribute.SetPersistingCheck<CABankTranRule.documentModule>(cache, rule, isCreateDoc ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
			PXDefaultAttribute.SetPersistingCheck<CABankTranRule.documentEntryTypeID>(cache, rule, isCreateDoc ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);

			PXUIFieldAttribute.SetVisible<CABankTranRule.documentModule>(cache, rule, isCreateDoc);
			PXUIFieldAttribute.SetVisible<CABankTranRule.documentEntryTypeID>(cache, rule, isCreateDoc);

			var amtNeeded = rule.AmountMatchingMode != MatchingMode.None;
			var maxAmtNeeded = rule.AmountMatchingMode == MatchingMode.Between;

			PXUIFieldAttribute.SetVisible<CABankTranRule.curyTranAmt>(cache, rule, amtNeeded);
			PXUIFieldAttribute.SetVisible<CABankTranRule.maxCuryTranAmt>(cache, rule, maxAmtNeeded);

			PXDefaultAttribute.SetPersistingCheck<CABankTranRule.curyTranAmt>(cache, rule, amtNeeded ? PXPersistingCheck.Null : PXPersistingCheck.Nothing);
			PXDefaultAttribute.SetPersistingCheck<CABankTranRule.maxCuryTranAmt>(cache, rule, maxAmtNeeded ? PXPersistingCheck.Null : PXPersistingCheck.Nothing);

			bool explicitCurrencyAllowed = rule.BankTranCashAccountID == null;
			PXUIFieldAttribute.SetEnabled<CABankTranRule.tranCuryID>(cache, rule, explicitCurrencyAllowed);
		}

		protected virtual void CABankTranRule_BankTranCashAccountID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			var rule = e.Row as CABankTranRule;
			if (rule == null)
				return;

			if (rule.BankTranCashAccountID != null)
			{
				var cashAcct = (CashAccount)PXSelectorAttribute.Select<CABankTranRule.bankTranCashAccountID>(cache, rule);
				cache.SetValueExt<CABankTranRule.tranCuryID>(rule, cashAcct.CuryID);
			}
			else
			{
				cache.SetDefaultExt<CABankTranRule.tranCuryID>(rule);
			}
		}

		protected virtual void CABankTranRule_AmountMatchingMode_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			var rule = e.Row as CABankTranRule;
			if (rule == null)
				return;

			switch (rule.AmountMatchingMode)
			{
				case MatchingMode.None:
					cache.SetValueExt<CABankTranRule.curyTranAmt>(rule, null);
					cache.SetValueExt<CABankTranRule.maxCuryTranAmt>(rule, null);
					break;
				case MatchingMode.Equal:
					cache.SetValueExt<CABankTranRule.maxCuryTranAmt>(rule, null);
					break;
				case MatchingMode.Between:
					if (rule.MaxCuryTranAmt == null)
					{
						cache.SetValueExt<CABankTranRule.maxCuryTranAmt>(rule, rule.CuryTranAmt);
					}
					break;
			}
		}

		protected virtual void CABankTranRule_Action_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			var rule = e.Row as CABankTranRule;

			if (rule.Action == RuleAction.CreateDocument)
			{
				cache.SetDefaultExt<CABankTranRule.documentModule>(rule);
				cache.SetDefaultExt<CABankTranRule.documentEntryTypeID>(rule);
			}
			else
			{
				rule.DocumentModule = null;
				rule.DocumentEntryTypeID = null;
			}
		}
	}
	public class CABankTranRuleMaintPopup : CABankTranRuleMaint
	{
		[PXDBString(30, IsUnicode = true)]
		[PXDefault]
		[PXUIField(DisplayName = "Rule", Visibility = PXUIVisibility.SelectorVisible)]
		[PXCheckUnique()]
		protected virtual void CABankTranRule_RuleCD_CacheAttached(PXCache sender) { }

		[PXDBIdentity(IsKey = true)]
		protected virtual void CABankTranRule_RuleID_CacheAttached(PXCache sender) { }

	}
}
