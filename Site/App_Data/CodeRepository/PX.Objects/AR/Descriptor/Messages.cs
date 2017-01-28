using PX.Common;

namespace PX.Objects.AR
{
	[PXLocalizable(Messages.Prefix)]
	public static class Messages
	{
		// Add your messages here as follows (see line below):
		#region Validation and Processing Messages
		public const string Prefix = "AR Error";
		public const string Document_Status_Invalid = AP.Messages.Document_Status_Invalid;
		public const string Entry_LE = AP.Messages.Entry_LE;
		public const string Entry_GE = AP.Messages.Entry_GE; 
		public const string UnknownDocumentType = AP.Messages.UnknownDocumentType;
		public const string Only_Open_Documents_MayBe_Processed = AP.Messages.Only_Open_Documents_MayBe_Processed;
		public const string Document_OnHold_CannotRelease = AP.Messages.Document_OnHold_CannotRelease;
		public const string ApplDate_Less_DocDate = AP.Messages.ApplDate_Less_DocDate;
		public const string WriteOff_ApplDate_Less_DocDate = "Write-Off {0} cannot be less than Document Date.";
		public const string ApplPeriod_Less_DocPeriod = AP.Messages.ApplPeriod_Less_DocPeriod;
		public const string ApplDate_Greater_DocDate = AP.Messages.ApplDate_Greater_DocDate;
		public const string ApplPeriod_Greater_DocPeriod = AP.Messages.ApplPeriod_Greater_DocPeriod;
		public const string DocumentBalanceNegative = AP.Messages.DocumentBalanceNegative;
		public const string DocumentApplicationAlreadyVoided = AP.Messages.DocumentApplicationAlreadyVoided;
		public const string DocumentOutOfBalance = AP.Messages.DocumentOutOfBalance;
		public const string CashSaleOutOfBalance = AP.Messages.QuickCheckOutOfBalance;
		public const string SheduleNextExecutionDateExceeded = GL.Messages.SheduleNextExecutionDateExceeded;
		public const string SheduleExecutionLimitExceeded = GL.Messages.SheduleExecutionLimitExceeded;
		public const string SheduleHasExpired = GL.Messages.SheduleHasExpired;
		public const string MultipleApplicationError = AP.Messages.MultipleApplicationError;
		public const string Cannot_Change_Details_Exists = "This field cannot be modified if one or more application is entered for the document.";
		public const string Only_Invoices_MayBe_Payed = "Only Invoices and Debit Adjustments can be selected for payment.";
		public const string VoidAppl_CheckNbr_NotMatchOrigPayment = "Void Payment must have the same Reference Number as the voided payment.";
		public const string FinChargeCanNotBeDeleted = "Financial charges cannot be entered directly. Please use Overdue Charges calculation process.";
		public const string CreditLimitWasExceeded = "The customer's credit limit has been exceeded.";
		public const string CreditDaysPastDueWereExceeded = "The customer's Days Past Due number of days has been exceeded!";
		public const string CustomerIsOnCreditHold = "The customer status is 'Credit Hold'.";
		public const string CustomerIsOnHold = "The customer status is 'On Hold'.";
		public const string CustomerIsInactive = "The customer status is 'Inactive'.";
		public const string SalesPersonIsInactive = "The sales person status is 'Inactive'.";
		public const string CustomerSmallBalanceAllowOff = "Write-Offs are not allowed for the customer.";
		public const string CreditHoldEntry = "Document status is 'On Credit Hold'.";
		public const string AdminHoldEntry = "Document status is 'On Hold'.";
		public const string SPCommissionCalcFailure = "Commission calculation process failed with one or more error.";
		public const string SPFuturePeriodIsInvalidToProcess = "Processing date is less than the start date for the selected commission period.";
		public const string SPOpenPeriodProcessingConfirmation = "If new documents for this period arrive, you will need to repeat the process of calculating commission or they will be included into the next commission period. Do you want to continue?";
		public const string SalesPersonAddedForAllLocations = "This Sales Person is added for all the Customer locations already.";
		public const string SalesPersonWithHistoryMayNotBeDeleted = "One or more AR transactions exists for the selected Sales Person. This record cannot be deleted.";
		public const string AllCustomerLocationsAreAdded = "All Customer locations has been added already.";
		public const string ErrorAutoApply = "Unexpected error occurred while applying {0} {1} to {2} {3}.";
		public const string CustomerClassChangeWarning = "Please confirm if you want to update current customer settings with the customer class defaults. Otherwise, original settings will be preserved.";
		public const string TempCrLimitInvalidDate = "Start date must be less or equal to the end date.";
		public const string TempCrLimitPeriodsCrossed = "Credit limit for this customer has already been exceeded.";
		public const string DuplicateCustomerPayment = "Payment with Payment Ref. '{0}' dated '{1}' already exists for this Customer and have the same Payment Method. It's Reference Number - {2} {3}.";
		public const string PaymentMethodIsAlreadyDefined = "You cannot add more than one Payment Method of this type for the Customer.";
		public const string ERR_UnreleasedFinChargesForDocument = "At least one unreleased overdue charge document has been found for this document. Processing has been aborted.";
		public const string WRN_FinChargeCustomerHasOpenPayments = "One or more unapplied or unreleased payments has been found for this Customer. Calculation of Overdue Charges can be affected by these documents. It is recommended to release and apply these documents prior to the processing.";
		public const string ERR_EmailIsRequiredForSendByEmailOptions = "Email address must be specified if any of the following options is activated: {0}.";
		public const string ERR_EmailIsRequiredForOption = "Email address must be specified if '{0}' option is activated.";
		public const string WRN_ProcessStatementDetectsUnappliedPayments = "One or more Customers with unapplied payment documents has been found. It is recommended to run Auto Apply Payments process prior to this Statement Cycle closure.";
		public const string WRN_ProcessStatementDetectsOverdueInvoices = "One or more Customers with overdue documents has been found. It is recommended to run Calculate Overdue Charges process prior to this Statement Cycle closure.";
		public const string WRN_ProcessStatementDetectsOverdueInvoicesAndUnappliedPayments = "One or more Customers with unapplied payments and overdue documents has been found. It's recommened to run Auto Apply Payments and Calculate Overdue Charges process prior to this Statement Cycle closure.";
		public const string Invoice_NotPrinted_CannotRelease = "Invoice/Memo document was not printed and cannot be released.";
		public const string Invoice_NotEmailed_CannotRelease = "Invoice/Memo document was not emailed and cannot be released.";
		public const string ERR_IncorrectFormatOfPMExpiryDate = "An invalid expiration date has been provided.";
		public const string ERR_ProcessingCenterForCardNotConfigured = "Processing center for this card type is not configured properly.";
		public const string ERR_ProcessingCenterTypeIsInvalid = "Type {0} defined for the processing center {1} cannot is not located within processing object.";
		public const string ERR_ProcessingCenterTypeInstanceCreationFailed = "Cannot instantiate processing object of {0} type for the processing center {1}.";
		public const string ERR_CCPaymentProcessingInternalError = "Error during request processing. Transaction ID:{0}, Error:{1}";
		public const string ERR_CCProcessingReferensedTransactionNotAuthorized = "Transaction {0} failed authorization";
		public const string ERR_CCProcessingTransactionMayNotBeVoided = "Transaction of {0} type cannot be voided";
		public const string ERR_CCProcessingTransactionMayNotBeCredited = "Transaction {0} type cannot not be credited";
		public const string ERR_CCTransactionCurrentlyInProgress = "This document has one or more transaction under processing.";
		public const string ERR_CCAuthorizedPaymentAlreadyCaptured = "This payment has been captured already.";
		public const string ERR_CCPaymentAlreadyAuthorized = "This payment has been pre-authorized already.";
		public const string ERR_CCPaymentIsAlreadyRefunded = "This payment has been refunded already.";
		public const string ERR_CCNoTransactionToVoid = "There is no successful transaction to void.";
		public const string ERR_CCTransactionOfThisTypeInvalidToVoid = "This type of transaction cannot be voided";
		public const string ERR_CCTransactionWasNotAuthorizedByProcCenter = "Authorization for transaction {0} failed. See transaction description for details.";
		public const string ERR_DuplicatedSalesPersonAdded = "This Sales Person is already added";
		public const string OverdueChargeDateAndFinPeriodAreRequired = "Overdue Charge Date and Fin. Period are required";
		public const string RecordAlreadyExists = "Record already exists";
		public const string TransactionIsAlreadyExpired = "Transaction is already expired";
		public const string UnknownPrepareOnType = "Unknown PrepareOn type";
		public const string CreditCardWithID_0_IsNotDefined = "Credit Card with ID {0} is not defined";
		public const string Cash_Sale_Cannot_Have_Multiply_Installments = "Multiple Installments are not allowed for Cash Sale.";
		public const string Multiply_Installments_Cannot_be_Reversed = "Multiple installments invoice cannot be reversed, Please reverse original invoice '{0}'.";
		public const string Application_Amount_Cannot_Exceed_Document_Amount = "Total application amount cannot exceed document amount.";
		public const string CustomerPMInstanceHasDuplicatedDescription = "The Payment Method with such description already exists for this customer.";
		public const string ERR_CCTransactionMustBeAuthorizedBeforeCapturing = "Transaction must be authorized before it may be captured";
		public const string ERR_CCOriginalTransactionNumberIsRequiredForVoiding = "Original transaction is required to may be voided";
		public const string ERR_CCOriginalTransactionNumberIsRequiredForVoidingOrCrediting = "Original transaction is required to may be voided/credited";
		public const string ERR_CCOriginalTransactionNumberIsRequiredForCrediting = "Original transaction is required to may be credited";
		public const string ERR_CCUnknownOperationType = "This operation is not implemented yet";
		public const string ERR_CCCreditCardHasExpired = "Credit card for the customer {1} is expired on {0}";
		public const string ERR_CCAuthorizationTransactionIsNotFound = "Priorly Authorized Transaction {0} is not found";
		public const string ERR_CCAuthorizationTransactionHasExpired = "Authorizing Transaction {0} has already expired. Authorization must be redone";
		public const string ERR_CCProcessingCenterUsedForAuthIsNotValid = "Processing center {0}, specified in authorizing transaction {1} can't be found";
		public const string ERR_CCProcessingCenterIsNotSpecified = "Processing center for payment method {0} is not specified";
		public const string ERR_CCProcessingCenterUsedForAuthIsNotActive = "Processing center {0}, specified in authorizing transaction {1} is inactive";
		public const string ERR_CCProcessingIsInactive = "Processing center {0} is inactive";
		public const string ERR_CCProcessingCenterIsNotActive = "Processing center {0} is inactive";
		public const string ERR_CCTransactionToVoidIsNotFound = "Transaction to be Void {0} is not found";
		public const string ERR_CCProcessingCenterUsedInReferencedTranNotFound = "Processing center {0}, specified in referenced transaction {1} can't be found";
		public const string ERR_CCProcessingCenterNotFound = "Processing center can't be found";
		public const string ERR_CCProcessingCenterUsedInReferencedTranNotActive = "Processing center {0}, specified in referenced transaction {1} is inactive";
		public const string ERR_CCTransactionToCreditIsNotFound = "Transaction to be Credited {0} is not found";
		public const string ERR_CCMultiplyPreauthCombined = "Multiply preauthorized orders combined in one invoice.";
		public const string ERR_CCTransactionMustBeVoided = "CC Payment must be voided.";
		public const string ERR_CCExternalAuthorizationNumberIsRequiredForCaptureOnlyTrans = "Authorization Number, received from Processing Center is required for this type of transaction.";
		public const string ARPaymentIsCreatedProcessingINProgress = "Payment {0} has been created";
		public const string ARPaymentIsCreatedButProcessingFailed = "Payments has been saccessfully created for several invoices, but their processing has failed. Please, check error in the specific row or check payment settings for the customer";
		public const string ARPaymentIsCreatedButProcessingFailedAndSomePaymentsFailed = "Payments was not success created for one or more selected documnets. For others, Payments has been saccessfully created, but their processing has failed. Please, check  an error in the specific rows";
		public const string CreationOfARPaymentFailedForSomeInvoices = "Creation of the Payment document has failed for one or more selected documents. Please, check specific error in each row";
		public const string PaymentProcessingFailedErrorReported= "Processing of Payment {0} {1} has failed. Reported error is: '{2}'";
		public const string ReservedWord = "'{0}' is a reserved word and cannot be used here.";
		public const string PrepaymentAppliedToMultiplyInstallments = "Prepayments cannot be applied to multiple-installment invoices.";
		public const string InvalidCashReceiptDeferredCode = "On Cash Receipt Deferred Code is not valid for the given document.";
		public const string SPCommissionPeriodMayNotBeProcessedThereArePeriodsOpenBeforeIt = "This Commission Period cannot be processed - all the previous commission periods must be closed first";
		public const string SPCommissionPeriodMayNotBeClosedThereArePeriodsOpenBeforeIt = "This Commission Period cannot be closed - all the previous commission periods must be closed first";
		public const string SPCommissionPeriodMayNotBeReopendThereAreClosedPeriodsAfterIt = "This Commission Period cannot be reopened - there are closed commission periods after it";
		public const string DuplicateInvoiceNbr = "Document with this Invoice Nbr. already exists.";
		public const string EntityDuplicateInvoiceNbr = "This vendor ref. number \"{0}\" has already been used for the document \"{1}\".";
        public const string SubEntityDuplicateInvoiceNbr = "This vendor ref. number \"{0}\" has already been used for the landed cost document (line number \"{1}\") linked to the purchase receipt \"{2}\".";
		public const string CannotSaveNotes = "Cannot save notes.";
		public const string CreditCardExpirationNotificationByEMailFailedCheckConfiguration = "E-mail notification for the Customer {0} has failed. Please, check Customer's e-mail or notification configuration";
		public const string CreditCardExpirationNotificationException = "Notification by E-mail failed: {0}";
		public const string InvoiceNotificationFailed = "Recipients not found to process email invoice.";
		public const string ARPaymentIsIncludedIntoCADepositAndCannotBeVoided = "This payment is included into Payment Deposit document. It can't be voided until deposit is released or the payment is excluded from it.";
		public const string PaymentIsRefunded = "Payment has been refunded with Refund {0}.";
		public const string AccountIsSameAsDeferred = "Transaction Account is same as Deferral Account specified in Deferred Code.";
		public const string DocumentNotFound = "Document {0} {1} cannot be found in the system.";
		public const string OriginalDocumentIsNotSet = "Original document is not set.";
		public const string DiscountOutOfDate = "Discount is out of date {0}.";
		public const string ERR_PCTransactionNumberOfTheOriginalPaymentIsRequired = "A valid PC Transaction number of the original payment is required";
		public const string DocsDepositAsBatchSettingDoesNotMatchClearingAccountFlag = "'Batch deposit' setting does not match 'Clearing Account' flag of the Cash Account";
		public const string PMDeltaOptionRequired = "Billing Option is required for ARTran when Amount is less than original PM Transaction";
        public const string PMDeltaOptionNotValid = "Bill Later Option is not valid under current settings. The Amount of the given transaction is set to zero. Delete this line from the invoice so that it can be billed next time.";
		public const string DefaultPaymentForTheCustomerMethodMayNotBeDeleted = "This Payment Method is set as default for the Customer. It may not be deleted.";
		public const string PaymentMethodInstanceIsUsedInDocumentsAndMayNotBeDeleted = "This Payment Method was used in one or more documents. It may not be deleted - instead, you can make it inactive.";
		public const string CreditCardHasUncapturedTransactions = "This card holds authorized transactions that were not captured.";
        public const string CustomerIsInStatus = "Customer is {0}.";
		public const string CashAccountIsNotConfiguredForPaymentMethodInAR = "The Cash Account specified is not configuered for usage in AR for the Payment Method {0}";
        public const string CustomerClassCanNotBeDeletedBecauseItIsUsed = "This Customer Class can not be deleted because it is used in Accounts Receivable Preferences.";
		public const string InactiveCreditCardMayNotBeProcessed = "The credit card with ID {0} is inactive and may not be processed";
		public const string InactiveCustomerPaymentMethodIsUsedInTheScheduledInvoices = "This Customer Payment method is inactive, but there are scheduled invoices using it. You need to correct them in order to avoid invoice processing interruptions.";
		public const string OnlyLastRowCanBeDeleted = "Only last row can be deleted";
		public const string ThisValueMUSTExceed = "This value MUST exceed {0}";
		public const string ThisValueCanNotExceed = "This value can not exceed {0}";
		public const string TaxIsNotUptodate = "Tax is not up-to-date.";
		public const string FreightTaxIsNotUptodate = "Freight Tax is not up-to-date.";
		public const string NoPaymentInstance = "There is no Customer Payment Method associated with the given record. This Payment method does not require specific information for the given customer.";
		public const string AskConfirmation = "Confirmation";
		public const string AskUpdateLastRefNbr = "Do you want to update Last Reference Number with entered number?";
		public const string GroupUpdateConfirm = "Restriction Groups will be reset for all Customer that belongs to this customer class.  This might override the custom settings. Please confirm your action";
		public const string CardProcessingError = "Credit card processing error. {0} : {1}";
		public const string TokenizationNotSupported = "Tokenization feature is not supported by processing";
		public const string HostedFormNotSupoorted = "Hosted form feature is not supported by processing";
		public const string FeatureNotSupportedByProcessing = "{0} feature is not supported by processing";
		public const string NOCCPID = "No Payment Profile ID in detials for payment method {0}!";
		public const string CantEnterAllDetailsAtOnce = "You can't enter Payment Profileg ID and card details at the same time." +
														" Enter only Payment Profile ID to sync it with processing center." +
														" Enter only details to create new payment method instance and sync it with processing center.";
		public const string AutoSyncImpossible = "More than one unsynchronized method found at processing center! Automatic syncronization is not possible. Please syncronize manually.";
		public const string CouldntGetPMIDetails = "Couldn't get details from processing center for payment method instance {0}";
		public const string DocumentAmountBelowMin = "The overdue charge document cannot be generated. The amount of overdue charges does not exceed the threshold amount required for generating the document.";
		public const string FixedAmountBelowMin = "With 0.00 amount specified, the system will not calculate charges for overdue documents. To initiate calculation of overdue charges, specify the fixed amount greater than 0.00.";
		public const string LineAmountBelowMin = "This line will not be added to the Overdue Charge document because calculated charge amount is less than the threshold amount specified in the overdue charge code on the Overdue Charges (AR204500) form.";
		public const string ZeroOverdueCharge = "The overdue charge cannot be calculate, the Use Percent Rate and Use Line Minimum Amount checkboxes are both cleared.";
        public const string PercentListEmpty = "For the selected charging method, at least one percent rate must be specified in the table below.";
        public const string PercentForDateNotFound = "Effective percent rate is not found.";
		public const string DateToSettleCrossDunningLetterOfNextLevel = "'{0}'+'{1}' should not exceed the '{0}' of the next level Dunning Letter.";
		public const string NoStatementToRegenerate = "There is no Statement available to regenerate.  Go to Prepare Statement to create a Statement.";
		public const string StatementCycleNotSpecified = "Statement Cycle not specified for the Customer.";
		public const string StatementCycleDayEmpty = "Day Of Month must be number between 1 and 31.";
		public const string StatementCycleDayIncorrect = "Please note that with Day Of Month set to {0}, statements will be generated earlier, on the last day of the month, if the month is shorter than {0} days.";
		public const string NoStatementsForCustomer = "There is no Statement available for the Customer.  Go to Prepare Statement to create a Statement.";
		public const string ReasonCodeIsRequired = "Reason Code must be specified before running the process.";
		public const string GroupDiscountExceedLimit = "Total group discount exceeds limit configured for this customer class. Document Discount was not calculated.";
        public const string OnlyGroupDiscountExceedLimit = "Total group discount exceeds limit configured for this customer class.";
		public const string DocDiscountExceedLimit = "Total Group and Document discount exceeds limit configured for this customer class ({0:F2}%).";
        public const string OnlyOneDocumentDiscountAllowed = "Only one Document Discount allowed.";
        public const string OneOrMoreItemsAreNotReleased = "One or more items are not released";
		public const string OneOrMoreItemsAreNotProcessed = "One or more items are not processed";
        public const string DuplicateGroupDiscount = "Duplicate Group Discount.";
		public const string AccountMappingNotConfigured = "Account Task Mapping is not configured for the following Project: {0}, Account: {1}";
		public const string LineDiscountAmtMayNotBeGreaterExtPrice = "Discount Amount may not be greater than Ext. Price.";
		public const string WriteOffIsDisabled = "Write-Off is disabled for the given customer. Set non zero write-off limit on the Customer screen and try again.";
		public const string WriteOffIsOutOfLimit = "Document balance exceeds the configured write-off limit for the given customer (Limit = {0}). Change the write-off limit on the Customer screen and try again.";
		public const string EffectiveDateExpirationDate = "The Expiration Date should not be earlier than the Effective Date.";
		public const string FreeItemMayNotBeEmpty = "Free Item may not be empty. Please select Free Item before activating discount.";
		public const string FreeItemMayNotBeEmptyPending = "Free Item may not be empty. Please select Pending Free Item and update discount before activating it.";
		public const string CarriersCannotBeMixed = "Common carrier and Local carrier cannot be mixed in one invoice. Tax calculation will be invalid.";
		public const string MultipleShipAddressOnInvoice = "Invoice references multiple shipments that were shipped to different locations. Tax calculation will be invalid.";
		public const string PaymentMethodNotConfigured = "To create tokenized payment methods you must first configure 'Payment Profile ID' in Payment Method's 'Settings for Use in AR'";
		public const string PostingToAvalaraFailed = "Document was released succesfully but failed to post tax to avalara with the following message: {0}";
		public const string NotAllCardsShown = "Some cards for {0} payment method(s) are not shown here because their data is stored at processing center";
		public const string ApplicationDateChanged = "Application date was changed to date of card transaction";
		public const string PaymentAndCaptureDatesDifferent = "Payment date is different than date of capture transaction";
		public const string ExpirationLessThanEffective = "Expiration Date may not be less than Effective Date.";
        public const string DuplicateSalesPrice = "Duplicate Sales Price. This line overlaps with another Sales Price (Price: {0}, Effective Date: {1}, Expiration Date: {2})";
        public const string DuplicateSalesPriceWS = "Duplicate Sales Price.";
		public const string ProcessingCenterCurrencyDoesNotMatch = "Currency of transacation ({0}) does not match with currency of processing center ({1})";
        public const string LastPriceWarning = "The system retains the last price and the current price for each item.";
        public const string HistoricalPricesWarning = "The system retains changes of the price records during {0} months.";
        public const string HistoricalPricesUnlimitedWarning = "The system retains changes of the price records for an unlimited period.";
		public const string AccounTaskMappingNotFound = "AR Account is mapped to an AccountGroup however there is no Account-DefaultTask mapping setup in the Project. Please correct and try again.";
        public const string UniqueItemConstraint = "Same Item cannot be listed more than once. Same item cannot belong to two or more active discount sequences of the same discount code";
        public const string UniqueBranchConstraint = "Same Branch cannot be listed more than once. Same branch cannot belong to two or more active discount sequences of the same discount code";
        public const string UniqueWarehouseConstraint = "Same Warehouse cannot be listed more than once. Same warehouse cannot belong to two or more active discount sequences of the same discount code";
        public const string UnconditionalDiscUniqueConstraint = "Unconditional discounts cannot have active overlapping sequences.";
        public const string DiscSeqCannotActivate = "Cannot activate the sequence because one or more constraints failed to validate.";
        public const string NoDiscountFound = "The Discount Code {0} has no matching Discount Sequence to apply.";
        public const string DiscountGreaterLineTotal = "Discount Total may not be greater than Line Total.";
        public const string DiscountGreaterLineMiscTotal = "Discount Total may not be greater than Line Total + Misc. Total.";
        public const string NoApplicableSequenceFound = "No applicable discount sequence found.";
        public const string MultipleApplicableSequencesFound = "Two or more applicable discount sequences found. Please select discount sequence manually.";
        public const string UnapplicableSequence = "Discount Sequence {0} cannot be applied to this document.";
        public const string DocumentDicountCanNotBeAdded = "Skip Document Discounts option is set for one or more group discounts. Document discount can not be added.";
        public const string SequenceExists = "Cannot delete a Discount if there already exist one or more Discount Sequences associated with the given Discount";
        public const string SequenceExistsApplicableTo = "Cannot change Applicable To for the Discount if there already exist one or more Discount Sequences associated with the given Discount";
        public const string NoLineSelected = "No Document Details line selected. Please select Document Details line.";
        public const string DiscountTypeCannotChanged = "Discount Type can not be changed if Discount Code has Discount Sequence";
        public const string DiscountsNotvalid = "One or more validations failed for the given discount sequence. Please fix the errors and try again.";
        public const string MultiplePriceRecords = "There are multiple price records that are effective on different dates. Please specify Effective As Of date.";
		public const string RequiredField = "This field is required!";
        public const string ShippedNotInvoicedINtranNotReleased = "Please release inventory Issue {0} before releasing Invoice.";
		public const string ReversingDocumentExists = "A reversing document {0} {1} already exists. Do you want to continue?";
		public const string ScheduledDocumentAlreadyReleased = "One of the scheduled documents is already released. Cannot generate new documents.";
		public const string TaxAccountTaskMappingNotFound = "Tax Account {0} is mapped to an AccountGroup however there is no Account-DefaultTask mapping setup in the Project. Please correct and try again.";
		public const string ChildCustomerShouldConsolidateBBFStatements = "Child accounts that consolidate balance to parent and use Balance Brought Forward statement type must consolidate statements as well.";
		public const string RelatedFieldChangeOnParentWarning = "Do you wish to update the {0} setting for all child accounts of this customer?";
		public const string StatementCycleShouldBeTheSameOnParentAndChildCustomer = "We recommend setting the same statement cycle as for the parent account.";
		public const string StatementTypeShouldBeOpenItemForParentChildAfterSeparation = "We recommend switching to Open Item statement type for both parent and child accounts.";
		public const string CustomerCantHaveSeparateStatement = "This customer can't have separate statement. Please view the statement for the parent customer {0}";
		public const string ConsolidatingCustomersParentMustBeCustomer = "If either or both of the Consolidate Balance and Consolidate Statements check boxes are selected, only an account of the Customer or Customer & Vendor type can be used as a parent account.";
		public const string ConsolidatingCustomersParentMustNotBeChild = "If either or both of the Consolidate Balance and Consolidate Statements check boxes are selected, only a customer account that has no parent account assigned can be specified as a parent account.";
		public const string CannotCaptureInInvalidPeriod = "Cannot record CC transaction on {0} : {1}";
		public const string CustomerRelationshipCannotBeBroken = "Unreleased parent-child applications exist for this customer. Neither Parent Account nor the Consolidate Balance option can be changed until these are released or deleted. Check the following documents: {0}";
		public const string ShouldSpecifyRoundingLimit = "To use this rounding rule, you should specify Rounding Limit on General Ledger Preferences (GL.10.20.00) form.";
		public const string UnprocessedPPDExists = "Documents with unprocessed cash discounts exist. Before you proceed, process these documents by generating and releasing credit memos on the \"Generate VAT Credit Memos\" (AR.50.45.00) form.";
		public const string PartialPPD = "Cash discount can be applied only on final payment.";
		public const string PaidPPD = "This document has been paid in full. To close the document, you need to apply the cash discount by generating a credit memo on the \"Generate VAT Credit Memos\" (AR.50.45.00) form.";
		public const string PPDApplicationExists = "To proceed, you have to reverse application of the final payment {0} with cash discount given.";
		public const string SelfVoidingDocPartialReverse = "The document should be voided in full. The reversing applications cannot be deleted partially.";		
		public const string SharedChildCreditHoldChange = "The status can not be changed. This is a child account that shares the credit policy with its parent account. Change the status of a parent account and the value will propagate to all child accounts.";
		public const string AdjustRefersNonExistentDocument = "Failed to process an application between documents {0} {1} and {2} {3} -	one of these documents cannot be found. Check whether both documents exist in the system.";
		public const string DuplicateCCProcessingID = "The Token ID {0} cannot be added to the selected payment method because it is already used in another customer payment method ({1})";
		public const string CreditCardTokenIDNotFound = "Card token ID cannot be found.";
		public const string CreditCardNotFoundInProcCenter = "No card with token ID {0} is found in the payment processing center {1}.";
		public const string TransactionHasExpired = "Transaction has already expired.";
		public const string DocumentAlreadyExistsWithTheSameReferenceNumber = "A {0} with this Reference Nbr. already exists in the system. Please, specify another reference number.";
		public const string ApplicationStateInvalid = "The application cannot be processed and your changes are not saved. Cancel the changes and start over. If the error persists, please contact support.";
		public const string AnotherChargeInvoiceRunning = "Another 'Generate Payments' process is already running. Please wait until it is finished.";
		public const string UnableToApplyDocumentApplicationDateEarlierThanDocumentDate = "Unable to apply the document because the application date is earlier than the document date.";
		public const string UnableToApplyDocumentApplicationPeriodPrecedesDocumentPeriod = "Unable to apply the document because the application post period precedes the financial period of the document.";
		public const string AmountEnteredExceedsRemainingCashDiscountBalance = "The amount entered exceeds the remaining cash discount balance {0}.";
		public const string FailedToSyncCC = "Credit card data cannot be synchronized. Please process the synchronization manually.";
		#endregion

		#region Translatable Strings used in the code
		public const string OrigDocument = "Orig. Document";
		public const string ViewLastCharge = "View Last Charge";
		public const string NewSchedule = "New Schedule";
		public const string ViewSchedule = "View Schedule";
		public const string MultiplyInstallmentsTranDesc = AP.Messages.MultiplyInstallmentsTranDesc;
		public const string DeferredRevenueTranDesc = "Deferred Revenue Recognition.";
		public const string NewInvoice = "Enter New Invoice";
		public const string NewPayment = "Enter New Payment";
		public const string CustomerPriceClass = "Customer Price Class";
		public const string AllPrices = "All Prices";
        public const string BasePrice = "Base";
		public const string ARAccess = "Customer Access";
		public const string ARAccessDetail = "Customer Access Detail";
		public const string Warning = "Warning";
		public const string FinChargeDocDescr = "Overdue charge";
		public const string SalesPerson = "Sales Person";
		public const string PrintInvoiceMemo = "Print Invoice/Memo";
		public const string VoidCommissions = "Void Commissions";
		public const string ClosePeriod = "Close Period";
		public const string ReopenPeriod = "Reopen Period";
		public const string Days = "Days";
		public const string MessageDescription = "Message Description";
		public const string OverDays = "Over Days";
		public const string DocumentDateSelection = "Document Date Selection";
		public const string Shipping = "Shipping";
		public const string Billing = "Billing";
		public const string ARAutoPaymentRefNbrFormat = "ACCB-{0:yyyy-MM-dd}";
		public const string ReviewSPComissionPeriod = "Review Commission Period";
		public const string Attention = "Attention!";
        public const string Process = "Process";
		public const string ProcessAll = "Process All";
		public const string CustomerPaymentMethodView = "Customer Payment Method";
		public const string CustomerView = "Customer";
		public const string BillingContactView = "Billing Contact";
		public const string CashSaleInvoice = "Cash Sale Invoice";
		public const string CashReturnInvoice = "Cash Return Invoice";
		public const string Calculate = "Calculate";
		public const string AllTransactions = "All Transactions";
		public const string FailedOnlyTransactions =  "Failed Only";
		public const string CreditCardIsExpired = "CC Expired";
		public const string ViewVendor = "View Vendor";
		public const string ViewBusnessAccount = "View Business Account";
		public const string ExtendToVendor = "Extend To Vendor";
		public const string ImportedExternalCCTransaction = "Imported External Transaction";
		public const string LostExpiredTranVoided = "Lost or Expired Transaction was Voided";
		public const string ARBalanceByCustomerReport = "AR Balance by Customer";
		public const string CustomerHistoryReport = "Customer History";
		public const string ARAgedPastDueReport = "AR Aged Past Due";
		public const string ARAgedOutstandingReport = "AR Aged Outstanding";
		public const string ARRegisterReport = "AR Register";
		public const string DocDiscDescr = "Group and Document Discount";
		public const string BasePriceClassDescription = "Base Price Class";
		public const string ViewARDiscountSequence = "View Discount Sequence";
		public const string SearchableTitleCustomer = "Customer: {0}";

		public const string TokenInputMode = "Profile ID";
		public const string DetailsInputMode = "Card Details";

		public const string PriceCode = "Price Code";
		public const string Description = "Description";

		public const string CreatePriceWorksheet = "Create Price Worksheet";
        public const string SODiscountSetupDocument = "Document-Level Discount";
        public const string SODiscountSetupLine = "Item-Level Discount";

        public const string ReversingRGOLTanDescription = "Reverse Deposit RGOL";
		public const string DocType = "Doc. Type";
		public const string DocumentType = "Document Type";
		public const string DocRefNbr = "Doc Ref. Nbr";
		public const string Type = "Type";
		public const string CashDiscountTaken = "Cash Discount Taken";
		public const string AmountPaid = "Amount Paid";
		public const string BalanceWriteOff = "Balance Write-Off";
		public const string CalculateOnOverdueChargeDocuments = "Calculate on Overdue Charge Documents";
		public const string ApplyOverdueCharges = "Apply Overdue Charges";

		public const string FailedGetFrom = CA.Messages.FailedGetFrom;
		public const string FailedGetTo = CA.Messages.FailedGetTo;
		public const string DocTypeNotSupported = AP.Messages.DocTypeNotSupported;
		public const string EmptyValuesFromAvalara = AP.Messages.EmptyValuesFromAvalara;
		public const string InvalidReasonCode = "Invalid Reason Code Usage. Only Balance Write-Off or Credit Write-Off codes are expected.";
		public const string VoidingCommissionsFailed = "Voiding commissions for the selected period has failed";

		public const string Release = PM.Messages.Release;
		public const string ReleaseAll = PM.Messages.ReleaseAll;
		#endregion

		#region Graphs Names
		public const string ARAutoApplyPayments = "Payment Application Process";
		public const string ARCreateWriteOff = "Balance Write Off Process";
		public const string CustomerClassMaint = "Customer Classes Maintenance";
		public const string CustomerMaint = "Customer Maintenance";
		public const string CustomerPaymentMethodMaint = "Customer Payment Methods Maintenance";
		public const string ARInvoiceEntry = "AR Invoice Entry";		
		public const string ARPaymentEntry = "AR Payment Entry";
		public const string ARDocumentRelease = "AR Documents Release Process";
		public const string ARPrintInvoices = "AR Invoice Printing Process";
		public const string ARReleaseProcess = "AR Release Process";
		public const string ARCustomerBalanceEnq = "Customers Balance - Summary Inquiry";
		public const string ARDocumentEnq = "Customer Balance - Detail Inquiry";
		public const string ARStatementProcess = "Customer Statement Preparation Process";
		public const string ARStatementDetails = "Statements History - Details by Date Inquiry";
		public const string ARStatementPrint = "Customer Statement Printing Process";
		public const string ARStatementForCustomer = "Statements History - Details by Customer Inquiry";
		public const string ARStatementHistory = "Statements History - Summary Inquiry";
		public const string ARStatementMaint = "Statement Cycle Maintenance";
		public const string SalesPersonMaint = "Sales Person Maintenance";
		public const string ARSPCommissionProcess = "Sales Person Commission Preparation Process";
		public const string ARSPCommissionDocEnq = "Sales Person Commission - Details Inquiry";
		public const string ARSPCommissionReview = "Sales Person Commission Period Closing Process";
		public const string ARFinChargesApplyMaint = "Overdue Charges Calculation Process";
		public const string ARFinChargesMaint = "Overdue Charge Codes Maintenance";
		public const string ARIntegrityCheck = "Customer Balances Validation Process";
		public const string ARScheduleMaint = "AR Scheduled Tasks Maintenance";
		public const string ARScheduleProcess = "AR Scheduled Tasks Process";
		public const string ARScheduleRun = "AR Sheduled Tasks Processing List";
		public const string ARSetupMaint = "Accounts Receivables Setup";
		public const string ARCCPaymentProcessing = "Credit Card Payments Processing";
		public const string ARExpiringCreditCardsEnq = "Expiring Credit Cards Inquiry";
		public const string ARSmallCreditWriteOffEntry = "Small Credit Write-Off Creation";
		public const string ARSmallBalanceWriteOffEntry = "Small Balance Write-Off Creation";
		public const string StatementCreateBO = "Statement Creation";
		public const string ARSPCommissionUpdate = "Commission History Creation";
		public const string ARTempCrLimitMaint = "Temporary Credit Limit Maintenance";
		public const string CCTransactionsHistoryEnq = "Credit Card Transactions History";
		public const string ARFailedCCPaymentTransEnq = "Payment Processing Log";
		public const string ARPriceClassMaint = "Customer Price Class Maintenance";
		public const string ARInvoice = "AR Invoice/Memo";
		public const string ARTran = "AR Transactions";
		public const string ARAddress = "AR Address";
		public const string ARContact = "AR Contact";
		public const string ChargeARInvoice = "Charge AR Invoices";
		public const string CaptureARPayment = "Capture AR Payments";
		public const string ARCustomerPaymentMethodExpirationProcess = "Credit Card expiration processing";
		public const string ARExpiredCreditCardsProcess = "Expired Credit Card Processing";
		public const string ARExpiringCreditCardsProcess = "Expiring Credit Card Notification Processing";
		public const string CCExpirationNotifyAll = "Notify All";
		public const string CCExpirationNotify = "Notify";
		public const string CCDeactivateAll = "Deactivate all";
		public const string CCDeactivate = "Deactivate";
        public const string DunningLetter = "Dunning Letter";
        public const string DunningLetterLevel = "Dunning Letter Level";
        public const string IncludeNonOverdue = "Include Other Unpaid Documents";
		public const string DunningLetterNotCreated = "One or more dunning letters was not created";
		public const string DunningLetterNotReleased = "Dunning letter was created but failed to release because of following error: ";
        public const string DunningLetterZeroLevel = "The Dunning Letter does not list any overdue documents, therefore it cannot be released.";
        public const string DunningLetterHavePaidFee = "The invoice for the Dunning Letter Fee has already been paid. To void the invoice you should void the payment first.";
        public const string DunningLetterHigherLevelExists = "The Dunning Letter cannot be voided. A Dunning Letter of a higher level exists for one or more documents. You should void the letters of a higher levels first.";
        public const string ViewDunningLetter = "View Dunning Letter";
        public const string DunningLetterStatus = "Dunning Letter Status";
        public const string DunningLetterFee = "Dunning Letter Fee";
		public const string DunningLetterInventoryNotFound = "The invoice for Dunning Letter Fee cannot be generated as a Dunning Fee Item cannot be found.";
		public const string DunningLetterAccountNotFound = "The invoice for Dunning Letter Fee cannot be generated as a Sales Account is not specified in the Dunning Fee Item.";
		public const string DunningLetterEmptyInventory = "The invoice for Dunning Letter Fee cannot be generated as a Dunning Fee Item is not specified in Accounts Receivable Preferences.";
		public const string DunningLetterFeeEmptySalesAccount = "The non-stock item has no sales account specified, thus it cannot be used for recording the dunning fee. Select another item or specify a sales account for this item on Non-Stock Items (IN202000).";
		public const string DunningLetterExcludedCustomer = "The Customer will be excluded from Dunning Letter Process if both Print and Send by Email check boxes are cleared.";
		public const string DunningLetterProcessSwithcedToCustomer = "If you switch the mode to the \"By Customer\" option, the system will assign the highest level found among customer documents to a customer account. The dunning letters will be prepared starting this level and the documents that have lower levels will be included into the first prepared letter.";
        public const string DunningProcessTypeDocument = "By Document";
        public const string DunningProcessTypeCustomer = "By Customer";
		public const string DunningProcessFeeEmptySalesAccount = "The invoice for the dunning fee cannot be generated. The non-stock item specified in the Dunning Fee Item box on Accounts Receivable Preferences (AR101000) has no sales account. Select another item or specify a sales account for this item on Non-Stock Items (IN202000).";
		public const string ProcessDL = "Process Dunning Letter";
        public const string IncludeAllToDL = "All Overdue Documents";
        public const string IncludeLevelsToDL = "Dunning Letter Level";
		public const string CustomerCreditHold = "Customer Credit Hold Processing";
		public const string ARExternalTaxPost = "AR External Tax Posting";
		public const string ARExternalTaxPostProcess = "AR External Tax Post Process";
		public const string ARExternalTaxCalc = "AR External Tax Posting";
		public const string ARExternalTaxCalcProcess = "AR External Tax Post Process";
		public const string ARSetup = "Account Receivable Preferences";

		#endregion 

        #region View Names
        public const string CustomerCredit = "CustomerCredit";
        #endregion

		#region DAC Names
		public const string CustomerPaymentMethodInfo = "Customer Payment Method";
		public const string CustomerPaymentMethod = "Customer Payment Method";
		public const string CustomerPaymentMethodDetail = "Customer Payment Method Detail";
		public const string ARSalesPerTran = "AR Salesperson Commission";
		public const string ARTaxTran = "AR Tax";
		public const string ARAdjust = "Applications";
		public const string ARPayment = "AR Payment";
		public const string CustSalesPeople = "Customer Salespersons";		
		public const string CustomerNotificationSource = "Customer Notification Source";
		public const string CustomerNotificationRecipient = "Customer Notification Recipient";
		public const string CustomerBalanceSummary = "Balance Summary";
		public const string ARCashSale = "Cash Sale";
		public const string CustomerClass = "Customer Class";
		public const string StatementCycle = "Statement Cycle";
		public const string Statement = "AR Statement";

		public const string ARBalances = "AR Balance";
		public const string ARBalanceByCustomer = "AR Balance by Customer";
		public const string CustomerHistory = "Customer History";
		public const string ARAgedPastDue = "AR Aged Past Due";
		public const string ARAgedOutstanding = "AR Aged Outstanding";
		public const string ARRegister = "AR Register";
		public const string CustomerDetails = "Customer Details";
		public const string DocumentSelection = "AR Document to Process";
		public const string ARDocument = "AR Document";
		public const string ARHistory = "AR History";
		public const string ARHistoryForReport = "AR History for Report";
		public const string ARHistoryByPeriod = "AR History by Period";
		public const string BaseARHistoryByPeriod = "Base AR History by Period";

		public const string ARInvoiceDiscountDetail = "AR Invoice Discount Detail";
        public const string DiscountSequence = "Discount Sequence";
		public const string ARDunningLetterDetail = "Dunning Letter Detail";

		public const string ARLatestHistory = "AR Latest History";
		public const string ARDiscount = "AR Discount";
		public const string ARDunningSetup = "AR Dunning Setup";
		public const string ARFinCharge = "AR Financial Charge";
		public const string ARFinChargePercent = "AR Financial Charge Percent";
		public const string ARFinChargeTran = "AR Financial Charge Transaction";
		public const string ARInvoiceNbr = "AR Invoice Nbr";
		public const string ARNotification = "AR Notification";
		public const string ARPaymentChargeTran = "AR Payment Charge Transaction";
		public const string ARPriceClass = "AR Price Class";
		public const string ARPriceWorksheet = "AR Price Worksheet";
		public const string ARPriceWorksheetDetail = "AR Price Worksheet Detail";
		public const string ARSalesPrice = "AR Sales Price";
		public const string ARSPCommissionPeriod = "AR Salesperson Commission Period";
		public const string ARSPCommissionYear = "AR Salesperson Commission Year";
		public const string ARSPCommnHistory = "AR Salesperson Commission History";
		public const string ARStatementDetail = "AR Statement Detail";
		public const string ARStatementDetailInfo = "AR Statement Detail Info";
		public const string ARTax = "AR Tax Detail";
		public const string CCProcTran = "Credit Card Processing Transaction";
		public const string CuryARHistory = "Currency AR History";
		public const string DiscountBranch = "Discount for Branch";
		public const string DiscountCustomer = "Discount for Customer";
		public const string DiscountCustomerPriceClass = "Discount for Customer and Price Class";
		public const string DiscountInventoryPriceClass = "Discount for Inventory and Price Class";
		public const string DiscountItem = "Discount Item";
		public const string DiscountSequenceDetail = "Discount Sequence Detail";
		public const string DiscountSite = "Discount for Warehouse";
		#endregion

		#region Document Type
		public const string Register = "Register";		
		public const string Invoice = "Invoice";
		public const string DebitMemo = "Debit Memo";
		public const string CreditMemo = "Credit Memo";
		public const string Payment = "Payment";
		public const string Prepayment = "Prepayment";
		public const string Refund = "Customer Refund";
		public const string VoidPayment = "Void Payment";
		public const string FinCharge = "Overdue Charge";
		public const string SmallBalanceWO = "Balance WO";
		public const string SmallCreditWO = "Credit WO";
		public const string DeferredRevenue = "Deferred Revenue";
		public const string CashSale = "Cash Sale";
		public const string CashReturn = "Cash Return";
		public const string NoUpdate = "No Update";
		public const string SalesOrderPrefix = "SO - ";


        #endregion

        #region ChargingMethod
        public const string FixedAmount = "Fixed Amount";
        public const string PercentWithThreshold = "Percent with Threshold";
        public const string PercentWithMinAmount = "Percent with Min. Amount";
        #endregion

        #region CalculationMethod
        public const string InterestOnBalance = "Interest on Balance";
        public const string InterestOnProratedBalance = "Interest on Prorated Balance";
        public const string InterestOnArrears = "Interest on Arrears";
        #endregion

        #region Recalculate Discounts Options
        public const string CurrentLine = "Current Line";
		public const string AllLines = "All Lines";
		#endregion

		#region Report Document Type
		public const string PrintInvoice = "INVOICE";
		public const string PrintDebitMemo = "DEBIT MEMO";
		public const string PrintCreditMemo = "CREDIT MEMO";
		public const string PrintPayment = "PAYMENT";
		public const string PrintPrepayment = "PREPAYMENT";
		public const string PrintRefund = "REFUND";
		public const string PrintVoidPayment = "VOIDPAY";
        public const string PrintFinCharge = "OVERDUE CHARGES";
		public const string PrintSmallBalanceWO = "BALANCE WO";
		public const string PrintSmallCreditWO = "CREDIT WO";
		public const string PrintCashSale = "CASH SALE";
		public const string PrintCashReturn = "CASH RET";
		#endregion

		#region Document Status
		public const string Hold = "On Hold";
		public const string Balanced = "Balanced";
		public const string Voided = "Voided";
		public const string Scheduled = "Scheduled";
        public const string Open = "Open";
        public const string Draft = "Draft";
		public const string Closed = "Closed";
		public const string PendingPrint = "Pending Print";
		public const string PendingEmail = "Pending Email";
		public const string CCHold = "Pending CC Processing";
		public const string CreditHold = "Credit Hold";
		public const string PendingApproval = "Pending Approval";
		public const string Released = "Released";
		public const string Reserved = "Reserved";
		#endregion

		#region AR Mask Codes
		public const string MaskItem = "Non-Stock Item";
		public const string MaskCustomer = "Customer";
        public const string MaskLocation = "Customer Location";
        public const string MaskEmployee = "Employee";
		public const string MaskCompany = "Branch";
		public const string MaskSalesPerson = "Salesperson";
		#endregion

		#region Commission Period Type
		public const string Monthly = "Monthly";
		public const string Quarterly = "Quarterly";
		public const string Yearly = "Yearly";
		public const string FiscalPeriod = "By Financial Period";
		#endregion

		#region PMInstanceSearchType

		public const string PMInstanceSearchByPartialNumber = "Search by Partial Number";
		public const string PMInstanceSearchByFullNumber = "Search By Full Number";
		
		#endregion

		#region Commission Period Status
		public const string PeriodPrepared = "Prepared";
		public const string PeriodOpen = "Open";
		public const string PeriodClosed = "Closed";
		#endregion

		#region SPCommnCalcTypes
		public const string ByInvoice = "Invoice";
		public const string ByPayment = "Payment";
		#endregion

		#region CCProcessingState
		public const string	CCNone = "None";
		public const string	CCPreAuthorized ="Pre-Authorized";
		public const string	CCPreAuthorizationFailed = "Pre-Authorization Failed";
		public const string	CCCaptured = "Captured";
		public const string	CCCaptureFailed = "Capture Failed";
		public const string	CCVoided = "Voided";
		public const string CCVoidFailed = "Voiding failed";
		public const string	CCRefunded = "Refunded";
		public const string	CCRefundFailed = "Refund Failed";
		public const string	CCPreAuthorizationExpired = "Pre-Authorization Expired";
		#endregion

		#region Load Child Documents Options
		public const string None = "None";
		public const string ExcludeCRM = "Except Credit Memos";
		public const string IncludeCRM = "All Types";
		#endregion

		#region Custom Actions
		public const string	ViewCustomer = "View Customer";
		public const string	ViewPaymentMethod = "View Payment Method";
		public const string ViewPayment= "View Payment";
		public const string ViewDocument = "View Document";
		public const string ViewOrigDocument = "View Orig. Document";
		public const string ProcessPrintInvoice = "Print Invoices";
		public const string ProcessEmailInvoice = "Email Invoices";
		public const string ProcessPrintStatement = "Print Statement";
		public const string ProcessEmailStatement = "Email Statement";
		public const string ProcessMarkDontEmail = "Mark as Do not Email";
		public const string ProcessMarkDontPrint = "Mark as Do not Print";
        public const string ProcessReleaseDunningLetter = "Release Dunning Letter";
        public const string ProcessRelease = "Release";
		public const string RegenerateStatement = "Regenerate Statement";
		public const string CustomerStatementHistory = "Customer Statement History";

		public const string ProcessPrintDL = "Print Dunning Letter";
		public const string ProcessEmailDL = "Email Dunning Letter";

		public const string RegenerateLastStatement = "Regenerate Last Statement";

		public const string Add = "Add";
		public const string AddItem = "Add Item";
		public const string CopyPrices = "Copy Prices";
		public const string CalcPendingPrices = "Calculate Pending Prices";
		#endregion

		#region Report Names
		public const string CustomerStatement = "Customer Statement";
		#endregion

		#region DiscountAppliedTo
		public const string ExtendedPrice = "Item Extended Price";
		public const string SalesPrice = "Item Price";
		#endregion

        #region Discount Type

        public const string Line = "Line";
        public const string Group = "Group";
        public const string Document = "Document";
        public const string Flat = "Flat-Price";
        public const string Unconditional = "Unconditional";

        #endregion

        #region Discount Target
        public const string Customer = "Customer";
		public const string CustomerMaster = "Customer (alias)";
		public const string Discount_Inventory = "Item";
        public const string CustomerPrice = "Customer Price Class";
        public const string InventoryPrice = "Item Price Class";
        public const string CustomerAndInventory = "Customer and Item";
        public const string CustomerPriceAndInventory = "Customer Price Class and Item";
        public const string CustomerAndInventoryPrice = "Customer and Item Price Class";
        public const string CustomerPriceAndInventoryPrice = "Customer Price Class and Item Price Class";

        public const string CustomerAndBranch = "Customer and Branch";
        public const string CustomerPriceAndBranch = "Customer Price Class and Branch";
        public const string Warehouse = "Warehouse";
        public const string WarehouseAndInventory = "Warehouse and Item";
        public const string WarehouseAndCustomer = "Warehouse and Customer";
        public const string WarehouseAndInventoryPrice = "Warehouse and Item Price Class";
        public const string WarehouseAndCustomerPrice = "Warehouse and Customer Price Class";
        public const string Branch = "Branch";
        #endregion

        #region Discount Option
        public const string Percent = "Percent";
        public const string Amount = "Amount";
        public const string FreeItem = "Free Item";
        #endregion

        #region BreakdownType
        public const string Quantity = "Quantity";
        #endregion

		#region Price Option
		public const string PriceClass = "Price Class";
		#endregion

		#region Retention Types
		public const string LastPrice = "Last Price";
		public const string FixedNumberOfMonths = "Fixed Number of Months";
		#endregion

		#region Price Basis
		public const string LastCost = "Last Cost + Markup %";
		public const string StdCost = "Avg./Std. Cost + Markup %";
		public const string CurrentPrice = "Source Price";
		public const string PendingPrice = "Pending Price";
		public const string RecommendedPrice = "MSRP";
		#endregion

		#region Adjustment Type
		public const string Adjusted = "Adjusted";
		public const string Adjusting = "Adjusting";
		#endregion

        #region Credithold Actions
        public const string ApplyCreditHoldMsg = "Apply Credit Hold";
        public const string ReleaseCreditHoldMsg = "Release Credit Hold";
        #endregion

        public const string DueDateRefNbr = "Due Date, Reference Nbr.";
		public const string DocDateRefNbr = "Doc. Date, Reference Nbr.";
		public const string RefNbr = "Reference Nbr.";
		public const string OrderNbr = "Order Nbr.";
		public const string OrderDateOrderNbr = "Order Date, Order Nbr.";
		public const string Revoked = "Revoked";

		public const string CompleteLine = "Complete Line";
		public const string IncompleteLine = "Bill Later";

		public const string MayNeedToUseAdvancedView = "Some settings required for creating document on this screen are missing from the Customer. You may need to use Advanced View to create a document for this Customer.";

		public const string LabelCury = "If copying to a different currency, select the rate type for currency conversion";
	}
}
