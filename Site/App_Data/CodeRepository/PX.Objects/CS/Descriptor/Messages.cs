using System;
using PX.Data;
using PX.Common;

namespace PX.Objects.CS
{
	[PXLocalizable(Messages.Prefix)]
	public static class Messages
	{
		// Add your messages here as follows (see line below):
		// public const string YourMessage = "Your message here.";

		#region Validation and Processing Messages
		public const string Prefix = "CS Error";
		public const string Entry_LE = "Incorrect value. The value to be entered must be less than or equal to {0}.";
		public const string Entry_GE = "Incorrect value. The value to be entered must be greater than or equal to {0}.";
		public const string Entry_LT = "Incorrect value. The value to be entered must be less than {0}.";
		public const string Entry_GT = "Incorrect value. The value to be entered must be greater than {0}.";
		public const string Entry_EQ = "Incorrect value. The value to be entered must be equal to {0}.";
		public const string Entry_NE = "Incorrect value. The value to be entered must not be equal to {0}.";
		public const string Entry_Multiple = "{0}, {1}, {2} etc.";
		public const string FieldReadOnly = "You are not allowed to change this field.";
		public const string StateIDMustBeUnique = "State/Region ID must be unique.";
		public const string StateNameCantBeEmpty = "State/Region Name cannot be empty.";
		public const string StateIDCantBeEmpty = "State/Region ID cannot be empty.";
		public const string SegmentHasChilds = "Segment '{0}' cannot be deleted as it has one or more override segments.";
		public const string SegmentIsNotLast = "Segment '{0}' cannot be deleted as it is not last segment.";
		public const string SegmentHasValues = "Segment '{0}' cannot be deleted as it has one or more segment values defined.";
        public const string SegmentNotOverridden = "Segment '{0}' is not overridden and cannot be deleted.";
		public const string DimensionIsEmpty = "Segmented key must have at least one segment.";
		public const string SameNumberingMask = "Start, End Number, Last and Warning Numbers must have the identical length and numbering mask.";
		public const string StartNumMustBeGreaterEndNum = "Start Number must be greater than the End Number.";
		public const string WarnNumMustBeLessEndNum = "Warning Number must be less than  the End Number.";
		public const string WarnNumMustBeGreaterStartNum = "Warning Number must be greater than the Start Number.";
		public const string LastNumMustBeLessEndNum = "Last Number must be less than the End Number.";
		public const string LastNumMustBeGreaterOrEqualStartNum = "Last Number must be greater than or equal to the Start Number-1.";
		public const string NewSymbolLength = "New Symbol length must not exceed Start or End Number length.";
		public const string ListOfInstallmentsComplete = "Installments Schedule is completed. The total percentage equal to 100%.";
		public const string NumberOfInstallmentsDoesntMatch = "The number of records for the installment schedule must match the value in the Number of Installments field.";
		public const string SumOfInstallmentsMustBe100 = "The total percentage for Installments Schedule must be equal to 100%.";
		public const string NumberingIDNull = "Numbering ID is null.";
		public const string CantAutoNumber = "Cannot generate the next number for the sequence.";
		public const string CantAutoNumberSpecific = "Cannot generate the next number for the {0} sequence.";
		public const string CantManualNumber = "Cannot generate the next number. Manual Numbering is activated for '{0}'";
		public const string WarningNumReached = "The numbering sequence is expiring.";
		public const string EndOfNumberingReached = "Cannot generate the next number for the {0} sequence because it is expired.";
		public const string CountryNameCantBeEmpty = "Country Name cannot be empty.";
		public const string CountryIDCantBeEmpty = "CountryID cannot be empty.";
		public const string NumberingIDRequired = "Numbering ID must be specified if Auto Numbering is enabled for one of the Segments.";
		public const string NumberingIDRequiredCustom = "{0} - Numbering ID must be specified if Auto Numbering is enabled for one of the Segments.";
		public const string NumberingViolatesSegmentDef = "Auto Numbering format violates the segment format. Segmented Key: '{0}' Segment: '{1}'.";
		public const string NumberingIsUsedFailedDelete = "This numbering sequence cannot be deleted. It is used by the '{0}' segmented key, in the '{1}' segment.";
		public const string NumberingIsUsedFailedDeleteSetup = "This numbering sequence cannot be deleted. It is used on the '{0}' form, in the '{1}' box.";
		public const string NumberingIsUsedFailedDeleteWorkBook = "This numbering sequence cannot be deleted. It is used by the '{0}' workbook.";
		public const string TermsIDEmpty = "TermsID cannot not be empty.";
		public const string TermsDiscountGreater = "Discount entered exceeds the discount specified for this terms.";
		public const string DescrEmpty = "Description cannot be empty.";
		public const string CashAccountsForBranch = "This branch cannot be deleted unless all its cash accounts are deleted. Please, delete the following cash account first: {0}.";
		public const string ValueNotInRange = "Value must be in the range of [1-31].";
		public const string DayToDayFrom = "Day To must be greater or equal then the Day From.";
		public const string DayFromNotNull = "Day From must be greater than or equal to 1.";
		public const string ValueMustBeLessEqualDueDay = "Value must less or equal then the Due Day.";
		public const string ValueMustBeGreaterEqualDiscDay = "Value must greater or equal then a discount day.";
		public const string NumberOfInstalments0 = "Number of installments must be greater than 0.";
		public const string OptionCantBeSelected = "This option cannot be used with the selected Due Date Type.";
		public const string OptionFixedNumberOfDays = "Only Fixed Number of Days option can be used with the selected Due Date Type.";
		public const string OptionEndOfMonth = "Only End of Month, Day of the Month and Day of Next Month options can be used with the selected Due Date Type.";
		public const string OptionDayOfTheMonth = "Only Day of the Month option can be used with the selected Due Date Type.";
		public const string DiscDateWillBeReset = "If calculated discount date is greater than due date, it will be reset to due date.";
		public const string MappedSegValueLength = "Total mapped segment value length should have {0} symbols.";
        public const string NumberingIDCannotBeUsedWithSegment = "Numbering ID cannot be used with '{0}' segment";
		public const string NumberingIDCannotBeUsedWithSegmentCustom = "{0} - Numbering ID '{1}' cannot be used with '{2}' segment";
        public const string EnsureSegmentLength = ": Ensure the segment's length matches the length of the numbering.";
        public const string EnsureSegmentMask = ": Ensure the segment's mask allows numerics.";
        public const string SegmentLengthLimit = "Total segment length cannot exceed 30 symbols.";
		public const string SegmentHasValuesFailedUpdate = "Segment '{0}' has values and cannot be updated.";
		public const string SegmentHasChildsFailedUpdate = "Segment '{0}' cannot be updated because it has override segments in the following keys: {1}.";
		public const string SegmentValueExistsType = "Segment value already exists.";
        public const string DimensionLengthOutOfRange = "Total key length is greater then the maximum allowed value.";
        public const string SegmentLengthLessThenZero = "Segment length must have positive value.";
		public const string AccountTypeWarn = "Account Type is not {0}.";
		public const string ItemExistsReenter = "Item exists, please enter it again.";
		public const string MaskSourceMissing = "{0} {1} is missing.";
		public const string MaskSourceMissing2 = "{0} '{2}' {1} is missing.";
		public const string FieldShouldBePositive = "'{0}' should be positive.";
		public const string FieldShouldNotBeNegative = "'{0}' should not be negative.";
		public const string ReferencedByEmployeeClass = "This record cannot be deleted because it is referenced by Employee Class - {0}";
		public const string ReferencedByCaseClass = "This record cannot be deleted because it is referenced by Case Class - {0}";
		public const string ReferencedByCarrier = "This record cannot be deleted because it is referenced by Carrier - {0}";
		public const string ReferencedByContract = "This record cannot be deleted because it is referenced by Contract - {0}";
		public const string ReferencedByEmployee = "This record cannot be deleted because it is referenced by Employee - {0}";
		public const string InvalidMask = "Invalid mask specified in segment {0} for {1}.";
		public const string ImportSettings = "Import Settings from Plug-in";
		public const string FailedToCreateCarrierPlugin = "Failed to Create Carrier Plug-in. Please check that Plug-In(Type) is valid Type Name. {0}";
		public const string FailedToFindCarrierPlugin = "Failed to Find Carrier Plug-in with the given ID - {0}";
		public const string Warning = "Warning";
		public const string SegmentHasValuesQuestion = "Segment '{0}' has one or more segment values defined. Would you like to delete them?";
		public const string ConversionForCarrierUOMNotFound = "Unit Conversion is not setup , typeof(Select<PM.PMSetup>)for the Carrier UOM. Please setup Unit Conversion FROM {0} TO {1}.";
		public const string GroupUpdateConfirm = "Restriction Groups will be reset for all Items that belongs to this item class.  This might override the custom settings. Please confirm your action";
		public const string StartDateNotUnique = "Start Date is not unique";
		public const string OneFieldMustBeFilled = "Either New Number Symbol or Manual Numbering should be set";
		public const string IncorrectFromDate = "'From' date cannot be less than 'To' date.";
		public const string CannotBeEmpty = "Cannot be empty.";
		public const string BranchNotEnoughRights = "You don't have enough rights on branch '{0}'";
		public const string BranchCantBeDeactivated = "Branch cannot be deactivated.";
		public const string NotificationSourceArgument = "Template and report must be defined in same class.";
		public const string ErrorFormat = "Specified format isn't supported for this type notification.";
		public const string ValidateAddress = "Validate Address";
		public const string ValidateAddresses = "Validate Addresses";
		public const string UnknownErrorOnAddressValidation = "An unknown error has happen during address validation";
		public const string AddressVerificationServiceCreationErrorHTTP = "Address verification Service {0} is failed to be created";
		public const string AddressVerificationServiceCreationError = "Address verification Service {0} is failed to be created";
		public const string AddressVerificationServiceIsNotActive = "The Address Verification Service (AVS) configured for the country '{0}' is not active";
		public const string AddressVerificationServiceIsNotSetup = "The Address Verification Service (AVS) is not configured for the country '{0}'";
		public const string AddressVerificationServiceReturnsField = "AVS Returns: '{0}'";
		public const string ShipViaFK = "Carrier cannot be deleted. One or more Ship Via is depends on this Carrier.";
		public const string AvalaraAVSUnknownError = "Avalara AVS: Unknown Error";
		public const string RefreshSettings = "Refresh Component List";
		public const string ThereAreSubaccountIdentifiersWhoseLengthIsGreaterThanLengthOfSegmentedKey = "There are subaccount identifiers {0} whose length is greater than the length of the Subaccount segmented key. Before the length of the segmented key is decreased, the length of the existing subaccount identifiers should be decreased.";
		public const string FeaturesUsageWarning = "This feature is in use, disabling it may cause unexpected results.";
		public const string FeaturesUsageUncheck = "This feature is in use and cannot be deactivated.";
        public const string FeaturesDependency = "To enable this feature, enable {0}  first.";
        public const string FeaturesDependencies = "To enable this feature, enable one of {0} first.";
		public const string TheSegmentWithConsolOrderValueEqualToAlreadyExists = "The segment '{0}' with Consol Order value equal to '{1}' already exists.";
        public const string SegmentedKeyAddingNewSegmentRequiresUpdating = "Adding a new segment will require the update of the identifiers of all related records in the system.";
        public const string DimensionDuplicate = "Length of segmented key cannot be decreased. Decreasing length will result to duplication of records in {0} table.";
		public const string FieldClassRestricted = "Segmented Key is restricted by features configuration.";
		public const string WeightOfEmptyBoxMustBeLessThenMaxWeight = "Weight of an empty Box should be less then the Max. Weight";
		public const string SelectSubaccountSegmentValidateCheckBox = "Select the Validate check box if the length of the subaccount segment in the consolidating company (Number of characters) differs from the length of the subaccount segment to be mapped (Length).";
		public const string ConsolidatedBranchEFilingSetting = "Please note that the system will use the 1099-MISC e-filing settings of the consolidation branch {0} to prepare e-file for the branch {1}.";
        public const string AddressOrContactIsMissing = "An address or a contact is not specified.";
		public const string InvalidWildcardMode = "Invalid WildcardMode.";
		public const string InvalidBetweenMode = "Invalid BetweenMode.";
		#endregion //Validation and Processing Messages

		#region Graph Names
		public const string ReasonCodeMaint = "Reason Codes Maintenance";
		public const string RMReportMaint = "ARM Reports Maintenance";
		public const string RMColumnSetMaint = "ARM Column Sets Maintenance";
		public const string RMRowSetMaint = "ARM Row Sets Maintenance";
		public const string RMUnitSetMaint = "ARM Unit Sets Maintenance";
		public const string CSCalendarMaint = "Calendars Maintenance";
		public const string CSFilterMaint = "System Filters Maintenance";
		public const string CSDeferredCodeMaint = "Deferred Codes Maintenance";
		public const string CompanyLocationMaint = "Company Locations Maintenance";
		public const string CompanySetup = "Company Setup";
		public const string BranchMaint = "Company Branches";
		public const string DimesionMaint = "Segmented Keys Maintenance";
		public const string SegmentMaint = "Segmented Values Maintenance";
		public const string SalesTaxMaint = "Sales Tax Maintenance";
		public const string TaxCategoryMaint = "Tax Category Maintenance";
		public const string TaxZoneMaint = "Tax Zone Maintenance";
		public const string CarrierMaint = "Carriers Maintenance";
		public const string CarrierPluginMaint = "Carrier Plug-in Maintenance";
		public const string CountryMaint = "Countries Maintenance";
		public const string NumberingMaint = "Numbering Sequences Maintenance";
		public const string FOBPointMaint = "FOB Points Maintenance";
		public const string ShipTermsMaint = "Shipment Terms Maintenance";
		public const string ShippingZoneMaint = "Shipping Zone Maintenance";
		public const string TermsMaint = "Terms Maintenance";
		public const string PreferencesMaint = "Preferences Maintenance";
		public const string Country = "Country";
		public const string RMReportReader = "ARM Reports Reader";
		public const string CSAttributeMaint = "Attribute Maintenance";
		public const string CSBoxMaint = "Box Maintenance";
		public const string DaylightShiftMaint = "Daylight Saving Time Calendar Maintenance";
		#endregion //Graph Names

		#region Cache Names
		public const string Branch = "Branch";
		public const string State = "State";
		public const string CSAnswers = "Answers";
		public const string DaylightShift = "Daylight Shift";
		public const string CalendarYear = "Calendar Year";
		public const string SegmentValue = "Segment Value";
		public const string FeaturesSet = "Features Set";
		public const string CommonSetup = "Common Setup";
        public const string Dimension = "Dimension";
        
		public const string FOBPoint = "FOB Point";
		public const string ShipTerms = "Shipping Terms";
		public const string Terms = "Terms";
		public const string BranchBAccount = "Branch with Business Account";
		public const string CarrierPackage = "Carrier Package";
		public const string CarrierPlugin = "Carrier Plugin";
		public const string CarrierPluginCustomer = "Carrier Plugin Customer";
		public const string CarrierPluginDetail = "Carrier Plugin Detail";
		public const string CompanyBAccount = "Company with Busienss Account";
		public const string FreightRate = "Freight Rate";
		public const string Numbering = "Numbering Sequence";
		public const string NumberingSequence = "Numbering Sequence Detail";
		public const string Segment = "Segment";
		public const string ShippingZone = "Shipping Zone";
		public const string ShipTermsDetail = "Shiping Terms Detail";
		public const string TermsInstallments = "Terms Installments Detail";
		public const string NotificationSource = "Notification Source";
		public const string NotificationSetupRecipient = "Default Notification Recipient";
		public const string NotificationSetup = "Default Notification setup";
		public const string NotificationRecipient = "Notification Recipient";
		public const string Calendar = "Calendar";
		public const string CalendarException = "Calendar Exception";
		public const string AttributeGroup = "Attribute Group";
		public const string AttributeDetail = "Attribute Detail";
		public const string Attribute = "Attribute";
		#endregion

		#region Custom Actions
		public const string ViewEmployee = "Employee Details";
		public const string NewEmployee = "New Employee";

		public const string ttipRenumber = "Renumber rows.";

		public const string AddToSiteMap = "Add To Site Map";
		public const string ttipAddToSiteMap = "Add this report to site map.";
		public const string ViewSegment = "View Segment";
		public const string CopyUnitSet = "Copy Unit Set";
		//public const string Up = "Up";
		//public const string Down = "Down";
		//public const string Left = "Left";
		//public const string Right = "Right";

		#endregion

		#region Translatable Strings used in the codes
		public const string All = "All";
		public const string Vendor = "Vendors";
		public const string Customer = "Customers";
		public const string Disabled = "Disabled";
		public const string FixedNumberOfDays = "Fixed Number of Days";
		public const string DayOfNextMonth = "Day of Next Month";
		public const string EndOfMonth = "End of Month";
		public const string EndOfNextMonth = "End of Next Month";
		public const string DayOfTheMonth = "Day of the Month";
		public const string Prox = "Fixed Number of Days starting Next Month"; 
		public const string Custom = "Custom";
		public const string Weekly = "Weekly";
		public const string Monthly = "Monthly";
		public const string SemiMonthly = "Semi-Monthly";
		public const string Single = "Single";
		public const string Multiple = "Multiple";
		public const string EqualParts = "Equal Parts";
		public const string AllTaxInFirst = "Tax in First Installment";
		public const string SplitByPercents = "Split by Percent in Table";
		public const string NoNumberNewSymbol = "<SELECT>";
		public const string WorkDay = "Work Day:";
		public const string WorkingHours = "Working Hours:";
		public const string GoodsAreMoved = "Goods Are Moved:";

		public const string ttipCopyStyle = "Copies current style record";
		public const string ttipPasteStyle = "Pastes copied style in current style record";
		public const string MoveToExternalNode = "Move to external node.";
		public const string MoveToInternalNode = "Move to internal node.";
		public const string MoveUp = "Move Up";
		public const string MoveDown = "Move Down";

		public const string CodeNotInt = "The 'Code' column contains one or more non-integer values";
		public const string RowCodeExists = "The record with the same 'Code' field value already exists.";
		public const string SIUnits = "SI Units (Kilogram)";
		public const string USUnits = "US Units (Pound)";
		public const string GroupMultiple = "<MULTIPLE>";
		public const string CannotStartWithDigit = "Cannot start with digit.";
		public const string CannotContainEmptyChars = "Cannot contain empty chars.";
		public const string Date = "Date";

		public const string RecordAlreadyExists = AR.Messages.RecordAlreadyExists;
		public const string CountryNameEmptyError = "Country Name can't be empty";
		public const string CountryIDEmptyError = "CountryID can't be empty";
		public const string WildcardSmallerMasked = "Wildcard string is smaller than string to be masked.";
		public const string WrongSyncDirection = "Wrong sync direction";
		public const string ConnectionCarrierAskSuccessHeader = "Settings";
		public const string ConnectionCarrierAskSuccess = "Connection to Carrier was successful.";
		#endregion //Translatable Strings used in the codes	

		#region Deferred Code Type
		public const string Income = "Revenue";
		public const string Expense = "Expense";
		#endregion

		#region Deferred Method Type
		public const string EvenPeriods = "Even Periods";
		public const string ProrateDays = "Even Periods, Prorate days";
		public const string ExactDays = "Exact days per period";
		#endregion

		#region MultDiv Type
		public const string Multiply = "Multiply";
		public const string Divide = "Divide";
		#endregion
		
		#region ARmReport parameters
		//Title 
		public const string StartBranchTitle = "Start Branch :";
		public const string EndBranchTitle = "End Branch :";
		public const string BookCodeTitle = "Ledger :";
		public const string PeriodTitle = "Financial Period :";
		public const string StartPeriodTitle = "Start Financial Period :";
		public const string EndPeriodTitle = "End Financial Period :";
		public const string StartAccTitle = "Start Account :";
		public const string EndAccTitle = "End Account :";
		public const string StartSubTitle = "Start Sub :";
		public const string EndSubTitle = "End Sub :";
		public const string AccountClassTitle = "Account Class :";

		public const string StartAccountGroupTitle = "Start Account Group :";
		public const string EndAccountGroupTitle = "End Account Group :";
		public const string StartProjectTitle = "Start Project :";
		public const string EndProjectTitle = "End Project :";
		public const string StartTaskTitle = "Start Task :";
		public const string EndTaskTitle = "End Task :";
		public const string StartInventoryTitle = "Start Inventory :";
		public const string EndInventoryTitle = "End Inventory :";		
		#endregion

		#region RMReportPM&GL
		public const string NotSet = "Not Set";
		public const string Amount = "Amount";
		public const string Quantity = "Quantity";
		public const string AmountTurnover = "Amount Turnover";
		public const string QuantityTurnover = "Quantity Turnover";
		public const string BudgetAmount = "Budget Amount";
		public const string BudgetQuantity = "Budget Quantity";
		public const string RevisedAmount = "Revised Amount";
		public const string RevisedQuantity = "Revised Quantity";
		public const string BudgetPTDAmount = "Budget PTD Amount";
		public const string BudgetPTDQuantity = "Budget PTD Quantity";
		public const string RevisedPTDAmount = "Revised PTD Amount";
		public const string RevisedPTDQuantity = "Revised PTD Quantity";
		public const string Turnover = "Turnover";
		public const string Credit = "Credit";
		public const string Debit = "Debit";
		public const string BegBalance = "Beg. Balance";
		public const string EndingBalance = "Ending Balance";
        public const string AmountType = "Amount Type";
        public const string CuryTurnover = "Curr. Turnover";
        public const string CuryCredit = "Curr. Credit";
        public const string CuryDebit = "Curr. Debit";
        public const string CuryBegBalance = "Curr. Beg. Balance";
        public const string CuryEndingBalance = "Curr. Ending Balance";
		public const string CuryAmountType = "Curr. Amount Type";
		public const string Code = "Code";
		public const string Description = "Description";
		public const string CodeDescription = "Code-Description";
		public const string DescriptionCode = "Description-Code";
		public const string Nothing = "Nothing";
		public const string Account = "Account";
        public const string AccountGroup = "Account Group";
        public const string Project = "Project";
        public const string ProjectTask = "Project Task";
        public const string Inventory = "Inventory";
        public const string Sub = "Sub";
        public const string OtherItem = "<OTHER>";
        public const string OtherItemDescription = "Unspecified";

		#endregion

		#region Reason Codes related
		public const string ReasonCode = "Reason Code";
		public const string Sales = "Sales";
		public const string CreditWriteOff = "Credit Write-Off";
		public const string BalanceWriteOff = "Balance Write-Off";
		public const string Employee = "Employee";
		public const string Salesperson = "Salesperson";
        public const string ReasonCodeInUse = "This reason code is specified as the default reason code on the Inventory Preferences form and cannot be deleted.";
        #endregion

        #region Echange
        public const string EmailExchangeEmptySubject = "Empty subject";
		public const string EmailExchangePrivateEvent = "Private event";
		public const string EmailExchangeNoteIsNull = "Note id cannot be null.";
		public const string EmailExchangeMethodNotSupported = "Exchange provider does not support this operation.";
		public const string EmailExchangeProviderNotFound = "Exchange provider does not found in the system.";
		public const string EmailExchangePolicyNotFound = "Synchronization policy could not be found for account '{0}'.";
		public const string EmailExchangeSyncSuccessful = "{0} {1}, mailbox '{2}'. {3} of item '{4}' has been completed successfully.";
		public const string EmailExchangeSyncError = "{0} {1}, mailbox '{2}'. {3} of item '{4}' has been failed.";
		public const string EmailExchangeSyncItemError = "An error has occured during {0} sync. {1} of item '{2}' failed.";
		public const string EmailExchangeSyncOperationError = "An fatal error has occured during {0} sync.";
		public const string EmailExchangeSyncMailboxError = "An error has occured during processing '{0}' mailbox.";
		public const string EmailExchangeSyncFolderError = "An error has occurred during folder '{0}' initialization for '{1}' mailbox.";
		public const string EmailExchangeSyncInitError = "It seems that '{0}' mailbox hasn't been initialized. You must initialize this mailbox on Outlook Web Access.";
		public const string EmailExchangeSyncFailed = "Synchronization with Exchange has been completed with errors. For details on each error, see the error messages in the grid below.";
		public const string EmailExchangeOperationStarted = "{0} operation for {1} have been started.";
		public const string EmailExchangeOperationFinished = "{0} operation for {1} have been finished.";
		public const string EmailExchangeSyncResult = "{0} {1} for {2} mailboxes have been completed. Total {3} have been processed, including {4} failed.";
		public const string EmailExchangeProviderInitialised = "Exchange sync provider for policy '{0}' has been initialised.";
		public const string EmailExchangeHashIsnotChanged = "Skipping import of item '{0}' due to item hash is not changed since last import.";
		public const string EmailExchangeCategorizing = "Marking prepared items with category.";
		public const string EmailExchangeItemCategorizing = "Item '{0}' has been processed and will be marked with category {1}.";
		public const string EmailExchangeMoving = "Moving prepared items to the parent folder.";
		public const string EmailExchangeItemMoving = "Item '{0}' is processed and will be moved to the parend folder.";

		public const string EmailExchangeAccountNotFound = "An appropriate email account could not be found in the system.";
		public const string EmailExchangeAccountNotEnabled = "Specified email account is not enabled for sending/receiving emails.";
		public const string EmailExchangeBrokenLink = "This link is broken. Please check attached files.";

		public const string EmailExchangeContactClassWarning = "The selected class does not allow creating contacts with the specified filter. You can adjust the class settings, select a different class, or leave the box blank.";
		#endregion

		public static string GetLocal(string message)
		{
			return PXLocalizer.Localize(message, typeof(Messages).FullName);
		}
	}
}
