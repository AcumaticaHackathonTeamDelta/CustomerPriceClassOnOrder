using System;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.EP;
using PX.Objects.CR;
using PX.Objects.CS;

namespace PX.Objects.AR
{
    public class ARSetupMaint : PXGraph<ARSetupMaint>
    {
        #region Public members
        public PXSave<ARSetup> Save;
        public PXCancel<ARSetup> Cancel;
        public PXSelect<ARSetup> ARSetupRecord;
        public CRNotificationSetupList<ARNotification> Notifications;
        public PXSelect<NotificationSetupRecipient,
            Where<NotificationSetupRecipient.setupID, Equal<Current<ARNotification.setupID>>>> Recipients;

        public PXSelect<ARDunningSetup> DunningSetup;   //MMK Dunnuing Letter parameters Setup 

        public CM.CMSetupSelect CMSetup;
        public PXSetup<GL.Company> Company;
		public PXSetup<GLSetup> GLSetup;

		#endregion

        #region CacheAttached
        [PXDBString(10)]
        [PXDefault]
        [CustomerContactType.ClassList]
        [PXUIField(DisplayName = "Contact Type")]
        [PXCheckUnique(typeof(NotificationSetupRecipient.contactID),
            Where = typeof(Where<NotificationSetupRecipient.setupID, Equal<Current<NotificationSetupRecipient.setupID>>>))]
        public virtual void NotificationSetupRecipient_ContactType_CacheAttached(PXCache sender)
        {
        }
        [PXDBInt]
        [PXUIField(DisplayName = "Contact ID")]
        [PXNotificationContactSelector(typeof(NotificationSetupRecipient.contactType),
            typeof(Search2<Contact.contactID,
                LeftJoin<EPEmployee,
                            On<EPEmployee.parentBAccountID, Equal<Contact.bAccountID>,
                            And<EPEmployee.defContactID, Equal<Contact.contactID>>>>,
                Where<Current<NotificationSetupRecipient.contactType>, Equal<NotificationContactType.employee>,
                            And<EPEmployee.acctCD, IsNotNull>>>))]
        public virtual void NotificationSetupRecipient_ContactID_CacheAttached(PXCache sender)
        {
        }

        #endregion

        #region Events
        protected virtual void ARSetup_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            ARSetup row = e.Row as ARSetup;
            if (row != null)
            {
                bool useMultipleBranches = this.ShowBranches();
                PXUIFieldAttribute.SetEnabled<ARSetup.invoicePrecision>(sender, row, (row.InvoiceRounding != RoundingType.Currency));
                PXUIFieldAttribute.SetEnabled<ARSetup.statementBranchID>(sender, row, row.ConsolidatedStatement ?? false);
                PXUIFieldAttribute.SetEnabled<ARSetup.dunningLetterBranchID>(sender, row, row.ConsolidatedDunningLetter ?? false);
                if (row.DunningLetterProcessType == 1)
                {
                    PXUIFieldAttribute.SetEnabled<ARSetup.autoReleaseDunningLetter>(sender, row, true);
                }

                PXUIFieldAttribute.SetVisible<ARSetup.consolidatedStatement>(sender, row, useMultipleBranches || row.StatementBranchID.HasValue);
                PXUIFieldAttribute.SetVisible<ARSetup.consolidatedDunningLetter>(sender, row, useMultipleBranches || row.DunningLetterBranchID.HasValue);
                PXUIFieldAttribute.SetVisible<ARSetup.statementBranchID>(sender, row, useMultipleBranches || row.StatementBranchID.HasValue);
                PXUIFieldAttribute.SetVisible<ARSetup.dunningLetterBranchID>(sender, row, useMultipleBranches || row.DunningLetterBranchID.HasValue);

                PXUIFieldAttribute.SetEnabled<ARSetup.numberOfMonths>(sender, row, row.RetentionType == RetentionTypeList.FixedNumOfMonths);

	            VerifyInvoiceRounding(sender, row);
            }
        }

        protected virtual void ARSetup_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            ARSetup row = e.Row as ARSetup;
            if (row != null)
            {
                bool useMultipleBranches = PXSelect<GL.Branch>.Select(this).Count > 0;
                PXDefaultAttribute.SetPersistingCheck<ARSetup.statementBranchID>(sender, row, (useMultipleBranches && row.ConsolidatedStatement == true) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
                PXDefaultAttribute.SetPersistingCheck<ARSetup.dunningLetterBranchID>(sender, row, (useMultipleBranches && row.ConsolidatedDunningLetter == true) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
                bool existDunningFee = false;
                foreach (ARDunningSetup item in DunningSetup.Select())
                {
                    if (item.DunningFee.HasValue && item.DunningFee != 0m)
                    {
                        existDunningFee = true;
                        break;
                    }
                }
                PXDefaultAttribute.SetPersistingCheck<ARSetup.dunningFeeInventoryID>(sender, row, existDunningFee ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
            }
        }

        protected virtual void ARSetup_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            ARSetup row = e.Row as ARSetup;
            if (row != null && row.ConsolidatedDunningLetter == false)
            {
                row.DunningLetterBranchID = null;
            }
            if (row != null && row.ConsolidatedStatement == false)
            {
                row.StatementBranchID = null;
            }

            if (row != null && (!sender.ObjectsEqual<ARSetup.retentionType>(e.Row, e.OldRow) || !sender.ObjectsEqual<ARSetup.numberOfMonths>(e.Row, e.OldRow)))
            {
                if (row.RetentionType == RetentionTypeList.LastPrice)
                    sender.RaiseExceptionHandling<ARSetup.retentionType>(e.Row, ((ARSetup)e.Row).RetentionType, new PXSetPropertyException(Messages.LastPriceWarning, PXErrorLevel.Warning));
                if (row.RetentionType == RetentionTypeList.FixedNumOfMonths)
                {
                    if (row.NumberOfMonths != 0) sender.RaiseExceptionHandling<ARSetup.retentionType>(e.Row, ((ARSetup)e.Row).RetentionType, new PXSetPropertyException(Messages.HistoricalPricesWarning, PXErrorLevel.Warning, row.NumberOfMonths));
                    if (row.NumberOfMonths == 0) sender.RaiseExceptionHandling<ARSetup.retentionType>(e.Row, ((ARSetup)e.Row).RetentionType, new PXSetPropertyException(Messages.HistoricalPricesUnlimitedWarning, PXErrorLevel.Warning, row.NumberOfMonths));
                }
            }
        }

        protected virtual void ARSetup_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            ARSetup row = e.Row as ARSetup;
            if (row != null && row.ConsolidatedDunningLetter == false)
            {
                row.DunningLetterBranchID = null;
            }
            if (row != null && row.ConsolidatedStatement == false)
            {
                row.StatementBranchID = null;
            }
        }

        protected virtual void ARSetup_ConsolidatedDunningLetter_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            ARSetup row = e.Row as ARSetup;
            if (row != null)
            {
                if (row.ConsolidatedDunningLetter == true)
                {
                    sender.SetDefaultExt<ARSetup.dunningLetterBranchID>(e.Row);
                }
                else
                {
                    sender.SetValueExt<ARSetup.dunningLetterBranchID>(e.Row, null);
                    sender.SetValuePending<ARSetup.dunningLetterBranchID>(e.Row, null);
                }
            }
        }

        protected virtual void ARSetup_ConsolidatedStatement_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            ARSetup row = e.Row as ARSetup;
            if (row != null)
            {
                if (row.ConsolidatedStatement == true)
                {
                    sender.SetDefaultExt<ARSetup.statementBranchID>(e.Row);
                }
                else
                {
                    sender.SetValueExt<ARSetup.statementBranchID>(e.Row, null);
                    sender.SetValuePending<ARSetup.statementBranchID>(e.Row, null);
                }
            }
        }

		protected virtual void ARSetup_DunningFeeInventoryID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			ARSetup setup = e.Row as ARSetup;
			if (setup == null || e.NewValue == null) return;

			IN.InventoryItem item = (IN.InventoryItem)PXSelectorAttribute.Select<ARSetup.dunningFeeInventoryID>(sender, setup, e.NewValue);
			if (item != null && item.SalesAcctID == null)
			{
				e.NewValue = item.InventoryCD;
				throw new PXSetPropertyException(Messages.DunningLetterFeeEmptySalesAccount);
			}
		}

		protected bool ShowBranches()
        {
            int[] ids = PXAccess.GetBranchIDs();
            return (ids != null && ids.Length > 1);
        }
        protected virtual void ARSetup_DunningLetterProcessType_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            ARSetup row = (ARSetup)e.Row;
            int? oldValue = (int?)e.OldValue;
            if (row != null)
            {

                if (row.DunningLetterProcessType == 0)
                {
                    PXUIFieldAttribute.SetEnabled<ARSetup.autoReleaseDunningLetter>(cache, row, false);
                    row.AutoReleaseDunningLetter = true;

                    if (oldValue != null && oldValue == 1)
                    {
                        cache.RaiseExceptionHandling<ARSetup.dunningLetterProcessType>(row, row.DunningLetterProcessType, new PXSetPropertyException(Messages.DunningLetterProcessSwithcedToCustomer, PXErrorLevel.Warning));
                    }
                }
                else
                {
                    PXUIFieldAttribute.SetEnabled<ARSetup.autoReleaseDunningLetter>(cache, row, true);
                }
            }
        }

        #region DunningSetup event handling
        // Deleting order control. Prevents break of consecutive enumeration.
        protected virtual void ARDunningSetup_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
        {
            int MaxRN = 0;
            foreach (PXResult<ARDunningSetup> ii in PXSelect<ARDunningSetup>.Select(this))
            {
                ARDunningSetup v = ii;
                int MaxR = v.DunningLetterLevel.Value;
                MaxRN = MaxRN < MaxR ? MaxR : MaxRN;
            }

            ARDunningSetup row = e.Row as ARDunningSetup;
            if (row != null)
            {
                if (row.DunningLetterLevel.Value < MaxRN)
                {
                    throw new PXException(Messages.OnlyLastRowCanBeDeleted);
                }
            }
        }

        // Prevents break of monotonically increasing values
        protected virtual void ARDunningSetup_DueDays_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            ARDunningSetup row = e.Row as ARDunningSetup;
            if (row != null)
            {
                int llevel = row.DunningLetterLevel.Value;
                int nv = Convert.ToInt32(e.NewValue);
                if (llevel == 1 && nv <= 0)
                {
                    throw new PXSetPropertyException(Messages.ThisValueMUSTExceed, 0);
                }
                else
                {
                    int NextValue = 0;
                    int PrevValue = 0;
                    foreach (PXResult<ARDunningSetup> ii in PXSelect<ARDunningSetup>.Select(this))
                    {
                        ARDunningSetup v = ii;
                        if (v.DunningLetterLevel.Value == llevel - 1)
                        {
                            PrevValue = v.DueDays.Value;
                        }
                        if (v.DunningLetterLevel.Value == llevel + 1)
                        {
                            NextValue = v.DueDays.Value;
                        }
                    }
                    if (nv <= PrevValue)
                    {
                        throw new PXSetPropertyException(Messages.ThisValueMUSTExceed, PrevValue);
                    }
                    if (nv >= NextValue && NextValue > 0)
                    {
                        throw new PXSetPropertyException(Messages.ThisValueCanNotExceed, NextValue);
                    }
                }
            }
        }

        // Computing default value on the basis of the previous values
        protected virtual void ARDunningSetup_DueDays_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            ARDunningSetup row = e.Row as ARDunningSetup;

            if (row != null)
            {
                int llevel = row.DunningLetterLevel.Value;

                if (llevel == 1)
                {
                    e.NewValue = 30;
                }
                else
                {
                    int PrevValue = 0;
                    foreach (PXResult<ARDunningSetup> ii in PXSelect<ARDunningSetup>.Select(this))
                    {
                        ARDunningSetup v = ii;
                        if (v.DunningLetterLevel.Value == llevel - 1)
                        {
                            PrevValue += v.DueDays.Value;
                        }
                        if (v.DunningLetterLevel.Value == 1 && llevel > 1)
                        {
                            PrevValue += v.DueDays.Value;
                        }
                    }
                    e.NewValue = PrevValue;
                }
            }
        }

        protected virtual void ARDunningSetup_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            ARDunningSetup row = e.Row as ARDunningSetup;
            if (row != null)
            {
                int llevel = row.DunningLetterLevel.Value;
                ARDunningSetup nextDL = PXSelect<ARDunningSetup, Where<ARDunningSetup.dunningLetterLevel, Greater<Required<ARDunningSetup.dunningLetterLevel>>>>.Select(this, row.DunningLetterLevel);
                bool clear = true;
                if (nextDL != null && nextDL.DueDays.HasValue)
                {
                    if (row.DueDays.HasValue && row.DaysToSettle.HasValue)
                    {
                        int delay = row.DueDays.Value + row.DaysToSettle.Value;
                        if (delay > nextDL.DueDays)
                        {
                            string dueDaysLabel = PXUIFieldAttribute.GetDisplayName<ARDunningSetup.dueDays>(sender);
                            string daysToSettleLabel = PXUIFieldAttribute.GetDisplayName<ARDunningSetup.daysToSettle>(sender);
                            sender.RaiseExceptionHandling<ARDunningSetup.daysToSettle>(row, row.DaysToSettle, new PXSetPropertyException(Messages.DateToSettleCrossDunningLetterOfNextLevel, PXErrorLevel.Warning, dueDaysLabel, daysToSettleLabel));
                            //PXUIFieldAttribute.SetWarning<ARDunningSetup.daysToSettle>(sender, row, Messages.DateToSettleCrossDunningLetterOfNextLevel);
                            clear = false;
                        }
                    }
                }
                if (clear)
                {
                    //PXUIFieldAttribute.SetWarning<ARDunningSetup.daysToSettle>(sender, row, null);
                    sender.RaiseExceptionHandling<ARDunningSetup.daysToSettle>(row, row.DaysToSettle, null);
                }
            }
        }

        #endregion
        #endregion

		private void VerifyInvoiceRounding(PXCache sender, ARSetup row)
		{
			var hasError = false;
			if (row.InvoiceRounding != RoundingType.Currency)
			{
				var glSetup = GLSetup.Current;

				if (glSetup.RoundingLimit == 0m)
				{
					hasError = true;
					sender.RaiseExceptionHandling<ARSetup.invoiceRounding>(row, null, new PXSetPropertyException(Messages.ShouldSpecifyRoundingLimit, PXErrorLevel.Warning));
				}
			}

			if (!hasError)
			{
				sender.RaiseExceptionHandling<ARSetup.invoiceRounding>(row, null, null);
			}
	    }
    }
}
