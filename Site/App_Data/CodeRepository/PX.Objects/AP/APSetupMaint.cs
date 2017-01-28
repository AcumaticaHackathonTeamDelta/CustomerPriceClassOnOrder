using PX.Data;

using PX.Objects.Common;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.CR;
using PX.Objects.CS;

namespace PX.Objects.AP
{
	public class APSetupMaint : PXGraph<APSetupMaint>
	{
		public PXSave<APSetup> Save;
		public PXCancel<APSetup> Cancel;
		public PXSelect<APSetup> Setup;
		public PXSelect<APSetupApproval> SetupApproval;
		//public PXSelect<VendorClass, Where<VendorClass.vendorClassID, Equal<Current<APSetup.dfltVendorClassID>>>> DefaultVendorClass;
		public PXSelect<AP1099Box> Boxes1099;
		public PXSelect<Account, Where<Account.accountID,Equal<Required<Account.accountID>>>> Account_AccountID;
		public PXSelect<Account, Where<Account.box1099, Equal<Required<Account.box1099>>>> Account_Box1099;

		public CRNotificationSetupList<APNotification> Notifications;
		public PXSelect<NotificationSetupRecipient,
			Where<NotificationSetupRecipient.setupID, Equal<Current<APNotification.setupID>>>> Recipients;

		#region CacheAttached
		[PXDBString(10)]
		[PXDefault]
		[VendorContactType.ClassList]
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

		#region Setups
		public CM.CMSetupSelect CMSetup;
		public PXSetup<GL.Company> Company;
		public PXSetup<GLSetup> GLSetup;
		#endregion

		public APSetupMaint()
		{
			GLSetup setup = GLSetup.Current;

            Boxes1099.Cache.AllowDelete = false;
            Boxes1099.Cache.AllowInsert = false;
            Boxes1099.Cache.AllowUpdate = true;
		}

		protected virtual void AP1099Box_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			AP1099Box row = (AP1099Box)e.Row;
            if (row == null || row.OldAccountID != null) return;
            
            Account acct = (Account)Account_Box1099.Select(row.BoxNbr);

			if (acct != null)
			{
                row.AccountID    = acct.AccountID;
                row.OldAccountID = acct.AccountID;
			}
		}

		protected virtual void AP1099Box_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if ((e.Operation & PXDBOperation.Command) != PXDBOperation.Update)
			{
				e.Cancel = true;
				return;
			}

			foreach (AP1099Box box in Boxes1099.Cache.Updated)
			{
				if (box.OldAccountID != null && (box.AccountID == null || box.OldAccountID != box.AccountID))
				{
					Account acct = (Account)Account_AccountID.Select(box.OldAccountID);
					if (acct != null)
					{
						acct.Box1099 = null;
						Account_AccountID.Cache.Update(acct);
					}
				}

				if (box.AccountID != null && (box.OldAccountID == null || box.OldAccountID != box.AccountID))
				{
					Account acct = (Account)Account_AccountID.Select(box.AccountID);
					if (acct != null)
					{
						acct.Box1099 = box.BoxNbr;
						Account_AccountID.Cache.Update(acct);
					}
				}
			}
		}
        protected virtual void APSetup_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
	        var row = (APSetup) e.Row;
            
			if (e.Row == null) 
				return;

            PXUIFieldAttribute.SetEnabled<APSetup.invoicePrecision>(sender, row, (row.InvoiceRounding != RoundingType.Currency));
            PXUIFieldAttribute.SetEnabled<APSetup.numberOfMonths>(sender, row, row.RetentionType == AR.RetentionTypeList.FixedNumOfMonths);

	        VerifyInvoiceRounding(sender, row);
        }

        protected virtual void APSetup_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            APSetup row = e.Row as APSetup;
            if (row == null) return;

            if (row != null && (!sender.ObjectsEqual<APSetup.retentionType>(e.Row, e.OldRow) || !sender.ObjectsEqual<APSetup.numberOfMonths>(e.Row, e.OldRow)))
            {
                if (row.RetentionType == AR.RetentionTypeList.LastPrice)
                    sender.RaiseExceptionHandling<APSetup.retentionType>(e.Row, ((APSetup)e.Row).RetentionType, new PXSetPropertyException(Messages.LastPriceWarning, PXErrorLevel.Warning));
                if (row.RetentionType == AR.RetentionTypeList.FixedNumOfMonths)
                {
					if (row.NumberOfMonths != 0) sender.RaiseExceptionHandling<APSetup.retentionType>(e.Row, ((APSetup)e.Row).RetentionType, new PXSetPropertyException(Messages.HistoricalPricesWarning, PXErrorLevel.Warning, row.NumberOfMonths));
					if (row.NumberOfMonths == 0) sender.RaiseExceptionHandling<APSetup.retentionType>(e.Row, ((APSetup)e.Row).RetentionType, new PXSetPropertyException(Messages.HistoricalPricesUnlimitedWarning, PXErrorLevel.Warning, row.NumberOfMonths));
                }
            }
        }

		private void VerifyInvoiceRounding(PXCache sender, APSetup row)
		{
			var hasError = false;
			if (row.InvoiceRounding != RoundingType.Currency)
			{
				var glSetup = GLSetup.Current;

				if (glSetup.RoundingLimit == 0m)
				{
					hasError = true;
					sender.RaiseExceptionHandling<APSetup.invoiceRounding>(row, null, new PXSetPropertyException(AR.Messages.ShouldSpecifyRoundingLimit, PXErrorLevel.Warning));
				}
			}

			if (!hasError)
			{
				sender.RaiseExceptionHandling<APSetup.invoiceRounding>(row, null, null);
			}
		}

		/*
        protected virtual void APSetup_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            if (DefaultVendorClass.Select().Count == 0)
            {
                DefaultVendorClass.Cache.Insert(new VendorClass());
            }
        }
		
        protected virtual void APSetup_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            PXUIFieldAttribute.SetEnabled<APSetup.dfltVendorClassID>(sender, e.Row, (Setup.Cache.GetStatus(e.Row) != PXEntryStatus.Inserted));
        }
		
        protected virtual void VendorClass_VendorClassID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            e.NewValue = ((APSetup)Setup.Select()).DfltVendorClassID;
        }
		
        protected virtual void VendorClass_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            bool mCActivated = (CMSetup.Current.MCActivated == true);

            PXUIFieldAttribute.SetVisible<VendorClass.vendorClassID>    (sender, e.Row, false);
            PXUIFieldAttribute.SetVisible<VendorClass.curyID>           (sender, null, mCActivated);
            PXUIFieldAttribute.SetVisible<VendorClass.curyRateTypeID>   (sender, null, mCActivated);
            PXUIFieldAttribute.SetVisible<VendorClass.allowOverrideCury>(sender, null, mCActivated);
            PXUIFieldAttribute.SetVisible<VendorClass.allowOverrideRate>(sender, null, mCActivated);
        }

		
        protected virtual void VendorClass_CuryID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            if (CMSetup.Current.MCActivated != true)
            {
                e.NewValue = null;
                e.Cancel = true;
            }
        }

        protected virtual void VendorClass_CuryRateTypeID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            if (CMSetup.Current.MCActivated != true)
            {
                e.NewValue = string.Empty;
                e.Cancel = true;
            }
        }

        protected virtual void VendorClass_CuryRateTypeID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            if (CMSetup.Current.MCActivated != true)
            {
                e.Cancel = true;
            }
        }
        */

		protected virtual void APSetup_InvoiceRequestApproval_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			PXCache cache = this.Caches[typeof(APSetupApproval)];

			PXResultset<APSetupApproval> approvalSetupRecords = PXSelect<APSetupApproval>.Select(sender.Graph, null);

			foreach (APSetupApproval approvalSetup in approvalSetupRecords)
			{
				approvalSetup.IsActive = (bool?)e.NewValue;
				cache.Update(approvalSetup);
			}
		}

		protected virtual void APSetup_HoldEntry_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			APSetup setup = e.Row as APSetup;

			if (setup == null) return;

			bool newHoldOnEntry = (bool?)e.NewValue == true;

			if (!newHoldOnEntry && setup.InvoiceRequestApproval == true)
			{
				e.NewValue = false;
				throw new PXSetPropertyException<APSetup.holdEntry>(
					Messages.CannotDisableHoldBecauseApprovalsRequested, 
					PXErrorLevel.RowWarning);
			}
		}

		protected virtual void APSetup_InvoiceRequestApproval_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			APSetup setup = e.Row as APSetup;
			
			if (setup == null) return;

			if (setup.HoldEntry != true &&
				setup.InvoiceRequestApproval == true)
			{
				// The approval process requires the documents
				// to be created in the hold status.
				// -
				sender.SetValueExt<APSetup.holdEntry>(setup, true);
			}
		}
	}
}
