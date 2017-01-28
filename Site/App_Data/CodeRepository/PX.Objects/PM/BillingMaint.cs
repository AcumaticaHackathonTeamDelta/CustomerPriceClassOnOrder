using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.GL;
using PX.Reports.Parser;
using System.Diagnostics;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.EP;
using PX.Objects.AP;
using PX.Objects.IN;
using PX.Objects.CS;

namespace PX.Objects.PM
{
	public class BillingMaint : PXGraph<BillingMaint, PMBilling>
	{
		public PXSelect<PMBilling> Billing;
		public PXSelect<PMBillingRule, Where<PMBillingRule.billingID, Equal<Current<PMBilling.billingID>>>> BillingRules;

		#region Event Handlers
		
		protected virtual void PMBillingRule_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			PMBillingRule row = e.Row as PMBillingRule;
			if (row != null)
			{
				PXUIFieldAttribute.SetEnabled<PMBillingRule.capsAccountGroupID>(sender, e.Row,
				                                                                (row.LimitAmt == true || row.LimitQty == true));
				PXUIFieldAttribute.SetEnabled<PMBillingRule.accountID>(sender, e.Row, row.AccountSource != PMAccountSource.None);
				PXUIFieldAttribute.SetEnabled<PMBillingRule.subID>(sender, e.Row, row.AccountSource != PMAccountSource.None);
				PXUIFieldAttribute.SetEnabled<PMBillingRule.subMask>(sender, e.Row, row.AccountSource != PMAccountSource.None);
			}
		}

		protected virtual void PMBillingRule_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			PMBillingRule row = e.Row as PMBillingRule;
			if (row != null)
			{
				if (row.CapsAccountGroupID == null && (row.LimitAmt == true || row.LimitQty == true))
				{
					sender.RaiseExceptionHandling<PMBillingRule.capsAccountGroupID>(row, null,
					                                                                new PXSetPropertyException(
						                                                                ErrorMessages.FieldIsEmpty,
						                                                                typeof (PMBillingRule.capsAccountGroupID).Name));
				}
			}
		}

		protected virtual void PMBillingRule_LimitQty_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<PMBillingRule.capsAccountGroupID>(e.Row);
		}

		protected virtual void PMBillingRule_LimitAmt_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<PMBillingRule.capsAccountGroupID>(e.Row);
		}

		protected virtual void PMBillingRule_CapsAccountGroupID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			PMBillingRule row = e.Row as PMBillingRule;
			if (row != null && row.CapsAccountGroupID == null && (row.LimitAmt == true || row.LimitQty == true))
			{
				PMAccountGroup ag = PXSelect<PMAccountGroup>.Search<PMAccountGroup.groupID>(this, row.AccountGroupID);
				e.NewValue = ag.GroupCD;
			}
		}

		protected virtual void PMBillingRule_AccountSource_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			PMBillingRule row = e.Row as PMBillingRule;
			if (row == null) return;

			if (row.AccountSource == PMAccountSource.None)
			{
				row.AccountID = null;
				row.SubID = null;
				row.SubMask = null;
			}
		}

		#endregion

	}

}
