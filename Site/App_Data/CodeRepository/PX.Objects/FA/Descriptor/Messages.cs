using PX.Common;

namespace PX.Objects.FA
{
	[PXLocalizable(Messages.Prefix)]
	public static class Messages
	{
		public const string Prefix = "FA Error";

		#region Graph/Cache Names
		public const string AssetClassMaint = "Asset Class Maintenance";
		public const string AssetMaint = "Asset Maintenance";
		public const string DepreciationMethodMaint = "Depreciation Method Definition Maintenance";
		public const string CalcDepreciation = "Calculation of Depreciation";
		public const string ServiceScheduleMaint = "Service Schedule Maintenance";
		public const string UsageScheduleMaint = "Usage Schedule Maintenance";
		public const string FATransactionEntry = "Fixed Asset Transactions";
		public const string BookMaint = "Book Maintenance";
		public const string SetupMaint = "Setup Maintenance";
		public const string FABookYearSetupMaint = "Book Calendar";
		public const string GenerationPeriods = "Generate FA Calendars";
		public const string FATranRelease = "Release FA Transactions";
		public const string CalcDeprProcess = "Run Depreciation";
		public const string AssetGLTransactions = "Asset GL Transactions";
		public const string DisposalMethodMaint = "Disposal Methods";
		public const string AssetSummary = "Asset Summary";
		public const string BonusMaint = "Bonuses";
		public const string FixedAsset = "Fixed Asset";
		public const string Register = "Fixed Asset Transaction";
		public const string AssetClass = "Asset Class";
		public const string DepreciationMethod = "Depreciation Method";
		public const string BookCalendar = "Book Calendar";
		public const string DisposalProcess = "Run Disposal";
		public const string SplitProcess = "Split Asset";
		public const string TransferProcess = "Run Transfer";
		public const string FixedAssetCostEnq = "Asset Cost Summary";
		public const string FACostDetailsInq = "Asset Cost Details";
		public const string FASplitsInq = "Asset Splits";
		public const string DeleteDocsProcess = "Delete FA Documents";
		public const string DisposeParams = "Disposal Info";
		public const string ReverseDisposalInfo = "Reverse Disposal Info";
		public const string FAComponent = "FA Component";
		public const string FAHistoryByPeriod = "FA History by Period";
		public const string FATransaction = "Fixed Asset Transaction";
		public const string FALocationHistory = "FA Location History";
		public const string FADetails = "FA Details";
		public const string FABook = "FA Book";
		public const string FABookBalance = "FA Book Balance";
		public const string FABookHistory = "FA Book History";
		public const string FAAccrualTran = "FA Accrual Transaction";
		public const string FABookHistoryRecon = "FA Book History for Reconciliation";
		public const string FALocationHistoryByPeriod = "FA Location History by Period";
		public const string FABonusDetails = "FA Bonus Details";
		public const string FABookPeriod = "FA Book Period";
		public const string FABookPeriodSetup = "FA Book Period Template";
		public const string FABookSettings = "FA Book Preferences";
		public const string FABookYear = "FA Book Year";
		public const string FADepreciationMethodLines = "FA Depreciation Method Lines";
		public const string FADisposalMethod = "FA Disposal Method";
		public const string FAService = "FA Service";
		public const string FAServiceSchedule = "FA Service Schedule";
		public const string FAType = "FA Type";
		public const string FAUsage = "FA Usage";
		public const string FAUsageSchedule = "FA Usage Schedule";
		#endregion

		#region Combo Values
		public const string ClassType = "Class";
		public const string AssetType = "Asset";
		public const string ElementType = "Component";
		public const string BothType = "Both";

		public const string Building = "Building";
		public const string Ground = "Land";
		public const string Vehicle = "Vehicle";
		public const string Equipment = "Equipment";
		public const string Computers = "Computers";
		public const string Furniture = "Furniture";
		public const string Machinery = "Machinery";

		public const string Active = "Active";
		//public const string Hold = "On Hold";
		public const string Suspend = "Suspend";
		public const string Unsuspend = "Unsuspend";
		public const string FullyDepreciated = "Fully Depreciated";
		public const string Disposed = "Disposed";
		public const string UnderConstruction = "Under construction";
		public const string Dekitting = "Dekitting";

		public const string Sale = "Sale";
		public const string Retirement = "Retirement";
		public const string Sectionalize = "Sectionalize";
		public const string TakenApart = "TakenApart";
		public const string Disappear = "Disappear";

		public const string Displacement = "Displacement";
		public const string ChangeResponsible = "Change Responsible";

		public const string ComponentDisplacementsWithAssets = "Component displacements with Assets";
		public const string ComponentBecomeAsset = "Component become Asset";
		public const string AssetBecomeComponent = "Asset become Component";
		public const string AssetBecomeInventoryItem = "Asset become Inventory Item";
		public const string BecomeAsset = "Inventory Item become Asset";
		public const string ComponentBecomeInventoryItem = "Component become Inventory Item";
		public const string BecomeComponent = "Inventory Item become Component";

		public const string Mileage = "Mileage";
		public const string Operating = "Operating";

		public const string Day = "Day";
		public const string Week = "Week";
		public const string Month = "Month";
		public const string Year = "Year";

		public const string Property = "Property";
		public const string GrantProperty = "Grant Property";
		public const string Leased = "Leased";
		public const string LeasedtoOthers = "Leased to Others";
		public const string Rented = "Rented";
		public const string RentedtoOthers = "Rented to Others";
		public const string Credit = "To the Credit of";

		public const string Software = "Software";
		public const string Goodwill = "Goodwill";
		public const string Patents = "Patents";
		public const string Copyrights = "Copyrights";

		public const string Good = "Good";
		public const string Avg = "Average";
		public const string Poor = "Poor";

		public const string NotAplicable = "Not Applicable";
		public const string Line10 = "10. Office Furniture & Machines & Library";
		public const string Line11 = "11. EDP Equipment/Computers/Word Processors";
		public const string Line12 = "12. Store Bar & Lounge and Restaurant Furniture & Equipment";
		public const string Line13 = "13. Machinery and Manufacturing Equipment";
		public const string Line14 = "14. Farm Grove and Dairy Equipment";
		public const string Line15 = "15. Professional Medical Dental & Laboratory Equipment";
		public const string Line16 = "16. Hotel Motel & Apartment Complex";
		public const string Line16a = "16a. Rental Units - Stove Refrig. Furniture Drapes & Appliances";
		public const string Line17 = "17. Mobile Home Attachments";
		public const string Line18 = "18. Service Station & Bulk Plant Equipment";
		public const string Line19 = "19. Sings - Billboard Pole Wall Portable Directional Etc.";
		public const string Line20 = "20. Leasehold improvements";
		public const string Line21 = "21. Pollution Control Equipment";
		public const string Line22 = "22. Equipment owned by you but rented leased or held by others";
		public const string Line23 = "23. Supplies - Not Held for Resale";
		public const string Others = "24. Other";

		public const string Acquisition = "Acquisition";
		public const string AccumulatedDepreciation = "Accumulated Depreciation";
		public const string Disposal = "Disposal";
		public const string CostReappraisal = "Cost Reappraisal";
		public const string AccumulatedDepreciationReappraisal = "Accumulated Depreciation Reappraisal";

		public const string InvoiceService = "Service Invoice";
		public const string InvoiceInsurance = "Insurance Invoice";
		public const string InvoiceLease = "Lease Invoice";
		public const string InvoiceRent = "Rent Invoice";
		public const string InvoiceCredit = "Credit Invoice";

		public const string PaymentService = "Service Payment";
		public const string PaymentInsurance = "Insurance Payment";
		public const string PaymentLease = "Lease Payment";
		public const string PaymentRent = "Rent Payment";
		public const string PaymentCredit = "Credit Payment";

		public const string Balanced = "Balanced";
		public const string Released = "Released";
		public const string Hold = "On Hold";
		public const string Voided = "Voided";
		public const string Calculated = "Calculated";
		public const string Unposted = "Unposted";
		public const string Posted = "Posted";
		public const string Completed = "Completed";


		public const string AssetInventory = "Asset Inventory";
		public const string ComponentInventory = "Component Inventory";

		public const string MACRS = "MACRS";
		public const string ACRS = "ACRS";
		public const string StraightLine = "Straight-Line";
		public const string DecliningBalance = "Declining-Balance";
		public const string SumOfTheYearsDigits = "Sum-of-the-Years’-Digits";
		public const string RemainingValue = "Remaining Value";
		public const string CustomTable = "Custom Table";
		public const string Dutch1 = "Dutch Method 1";
		public const string Dutch2 = "Dutch Method 2";

		public const string MethodDescBonus = "BONUS";
		public const string MethodDescTax179 = "TAX179";

		public const string FullPeriod = "Full Period";
		public const string HalfPeriod = "Mid Period";
		public const string ModifiedPeriod = "Modified Half Period";
		public const string ModifiedPeriod2 = "Modified Half Period 2";
		public const string NextPeriod = "Next Period";
		public const string FullQuarter = "Full Quarter";
		public const string HalfQuarter = "Mid Quarter";
		public const string FullYear = "Full Year";
		public const string HalfYear = "Mid Year";
		public const string FullDay = "Full Day";

		public const string PurchasingPlus = "Purchasing+";
		public const string PurchasingMinus = "Purchasing-";
		public const string DepreciationPlus = "Depreciation+";
		public const string DepreciationMinus = "Depreciation-";
		public const string CalculatedPlus = "Calculated+";
		public const string CalculatedMinus = "Calculated-";
		public const string SalePlus = "Sale/Dispose+";
		public const string SaleMinus = "Sale/Dispose-";
		public const string TransferPurchasing = "Transfer Purchasing";
		public const string TransferDepreciation = "Transfer Depreciation";
		public const string ReconciliationPlus = "Reconciliation+";
		public const string ReconciliationMinus = "Reconciliation-";
		public const string PurchasingDisposal = "Purchasing Disposal";
		public const string PurchasingReversal = "Purchasing Reversal";
		public const string AdjustingDeprPlus = "Depreciation Adjusting+";
		public const string AdjustingDeprMinus = "Depreciation Adjusting-";

		public const string QuarterConv = "Quarter";
		public const string MonthConv = "Month";
		public const string HalfYearConv = "Half Year";
		public const string YearConv = "Year";
		public const string HalfMonth = "Half Month";

		public const string FixedDay = "Fixed Day";
		public const string NumberOfDays = "Number of Days";
		public const string PeriodDaysHalve = "Half Period";

		public const string Calculate = "Calculate Only";
		public const string Depreciate = "Depreciate";

		public const string OriginAdjustment = "Adjustment";
		public const string OriginPurchasing = "Purchasing";
		public const string OriginDepreciation = "Depreciation";
		public const string OriginDisposal = "Disposal";
		public const string OriginTransfer = "Transfer";
		public const string OriginReconcilliation = "Reconcilliation";
		public const string OriginSplit = "Split";
		public const string OriginReversal = "Reversal";
		public const string OriginDisposalReversal = "Disposal Reversal";

		public const string SideBySide = "Side by Side";
		public const string BookSheet = "By Book";

		public const string MaskAsset = "Fixed Asset";
		public const string MaskLocation = "Fixed Asset Branch";
		public const string MaskDepartment = "Fixed Asset Department";
		public const string MaskClass = "Fixed Asset Class";

		public const string NetValue = "Net Value";
		public const string Acquired = "Acquisitions";
		public const string Depreciated = "Accumulated Depreciation";
		public const string DepreciationBase = "Bepreciation Base";
		public const string Bonus = "Bonus";
		public const string BonusRecap = "Bonus Recapture";
		public const string BonusTaken = "Bonus Taken";
		public const string Tax179 = "Tax 179";
		public const string Tax179Recap = "Tax 179 Recapture";
		public const string Tax179Taken = "Tax 179 Taken";
		public const string Revalue = "Revalue";
		public const string RGOL = "RGOL";
		public const string Suspended = "Suspended";
		public const string Reversed = "Reversed";

		public const string Addition = "Addition";
		public const string Deduction = "Deduction";

		public const string Automatic = "Automatic";
		public const string Manual = "Manual";
		#endregion

		#region Error messages
		public const string Document_Status_Invalid = AP.Messages.Document_Status_Invalid;
		public const string FixedAssetClassCannotBeDeactivated = "The fixed asset class cannot be deactivated. There are some fixed assets with the Active status associated with this class.";
		public const string WrongValue = "Value must equal 100%.";
		public const string BookExistsHistory = "Book '{0}' cannot be deleted because it is used by Fixed Asset or Fixed Asset Class.";
		public const string LedgerBookExists = "You cannot set defaul Ledger from GLSetup because there is one more Book with default ledger.";
		public const string ScheduleExistsHistory = "You cannot delete Schedule because transactions for this Schedule exist.";
		public const string NoPeriodsDefined = "Book Period cannot be found in the system.";
		public const string NoCalendarDefined = "Book Calendar cannot be found in the system.";

		public const string ValueCanNotBeEmpty = "Value can not be empty.";
		public const string FinPeriodsNotDefined = "Financial periods are not defined in {1}.";
		public const string FABookPeriodsNotDefinedFrom = "Asset will be depreciated from {0}, book '{2}' does not have financial periods generated from {1}. You need to go to 'Generate FA Calendars' screen and generate financial periods before changing this asset.";
		public const string FABookPeriodsNotDefinedFromTo = "Asset will be depreciated from {0} to {1}, book '{4}' does not have financial periods generated from {2} to {3}. You need to go to 'Generate FA Calendars' screen and generate financial periods before changing this asset.";
		public const string FABookPeriodsNotDefined = "Financial periods are not defined for the book '{0}' in {1}.";
		public const string CanNotUseAveragingConventionWhithDate = "Can not use averaging convention {0} with depreciation start date {1}.";
		public const string CanNotUseAveragingConventionWhithRecoveryPeriods = "Can not use averaging convention {0} with recovery period {1}.";
		public const string CanNotCalculateDepreciation = "Can not calculate depreciation!";
		public const string DepreciationMethodDoesNotExist = "Depreciation method does not exist.";
		public const string DepreciationAdjustmentPostedOpenPeriod = "Depreciation adjustments (D+/D-) can be posted only to closed periods.";
		public const string DepreciationAdjustmentPostedClosedPeriod = "Depreciation adjustments (A+/A-) can be posted only to closed periods.";
		public const string CalculatedDepreciationPostedFuturePeriod = "Calculated depreciation can be posted only to current period.";
		public const string PurchasePostedClosedPeriod = "Purchasing adjustments can be posted only to open periods.";
		public const string BalanceRecordCannotBeDeleted = "Record cannot be deleted.";
		public const string TranDateOutOfRange = "Calendar is not setup for date in Book '{0}'.";
		public const string AssetDisposedInPastPeriod = "Asset cannot be disposed in past periods.";
		public const string AssetDisposedInSuspendedPeriod = "Asset cannot be disposed in suspended periods.";
		public const string TranPostedToSuspendedPeriod = "Transactions cannot be posted to suspended period in Book '{0}'.";
		public const string TranPostedOnHold = "Transactions cannot be posted for asset 'On Hold'.";
		public const string CalendarAlreadyExists = "Calendar for this book already created.";
		public const string AssetIsNotSaved = "Fixed asset must be saved";
		public const string UnholdAssetOutOfBalance = "Asset is out of balance and cannot be removed from 'Hold'. Add Purchasing+ transactions with total amount {0}.";
		public const string GLTranNotSelected = "GL Transaction must be selected.";
		public const string IncorrectDepreciationPeriods = "Incorrect periods beginning and end of depreciation for Book '{0}'";
		public const string CyclicParentRef = "You cannot reference the child asset '{0}' as the parent asset.";
		public const string AssetHasUnreleasedTran = "Fixed asset has unreleased transactions";
		public const string AssetShouldBeDeprToPeriod = "Fixed asset should be depreciated to period '{0}' if \"Automatically Release Depreciation Transactions\" is not set.";
		public const string CalendarSetupNotFound = "FA calendar cannot be generated for the book '{0}' because the calendar structure is not configured for this book on the Book Calendars (FA206000) form.";
		public const string FixedAssetNotSaved = "The fixed asset is not saved.";
		public const string FixedAssetHasUnreleasedPurchasing = "The fixed asset has unreleased purchasing transactions.";
		public const string FixedAssetHasUnreleasedRecon = "The fixed asset has unreleased reconciliation transactions.";
		public const string InvalidReconTran = "Reconcilliation transaction has no reference to the original GL transaction.";
		public const string CanNotDisposeUnreconciledAsset = "Unreconciled asset can not be disposed";
		public const string AcquisitionAfterDisposal = "Disposal date must be greater than acquisition date.";
		public const string SplittedCostGreatherOrigin = "Total cost of splitted assets greater than cost of origin asset.";
		public const string SplittedQtyGreatherOrigin = "Total quantity of splitted assets greater than or equal to quantity of origin asset.";
		public const string CannotChangeUsefulLife = "New Depr. To Period '{0}' less than Last Depreciation Period '{1}'.";
		public const string CannotCreateAsset = "Cannot create Fixed Asset. Asset Numbering is off, Asset ID is empty.";
		public const string FAClassChangeConfirmation = "Warning - Only GL Accounts will be changed, all other parameters remain unchanged. Do you want to continue?";
		public const string DispAmtIsEmpty = "Can not dispose Fixed Assets. Disposal Amount is empty.";
		public const string CannotReleaseInInitializeMode = "Only Purchasing Register can be released in Initialization Mode.";
		public const string AssetNotDepreciatedInPeriod = "Fixed asset '{0}' not depreciated by the book '{1}' in {2} period.";
		public const string NextPeriodNotGenerated = "Next Period after Last Depr. Period isn't generated.";
		public const string CurrentDeprPeriodIsNull = "Fixed asset not acquired or fully depreciated.";
		public const string OperationNotWorkInInitMode = "This operation is not available in initialization mode. To exit the initialization mode, select the '{1}' checkbox on the '{0}' screen.";
		public const string CannotChangeCurrentPeriod = "New Current Period '{0}' less than previous Current Period '{1}'.";
		public const string ReverseDispPeriodNotFound = "Active open period is not found after Disposal Period '{0}'";
		public const string ActiveAssetTransferedPastDeprToPeriod = "Active asset cannot be transferred past Depr. To Period {0}";
		public const string ActiveAssetTransferedBeforePeriod = "Active asset cannot be transferred before Current Period {0}";
		public const string FullyDepreciatedAssetTransferedBeforePeriod = "Fully-depreciated asset cannot be transferred before Period {0}";
		public const string AnotherDeprRunning = "Another depreciation process is already running. Wait for completion.";
		public const string CantReverseDisposedAsset = "Unable Reverse Disposed Fixed Asset '{0}'";
		public const string CantReverseDisposal = "The Status of Fixed Asset '{0}' is '{1}'. Unable Reverse Disposal.";
		public const string TableMethodHasNoLineForYear = "Table depreciation method '{0}' has no line for {1} year.";
		public const string FlagHasValueOnButItMustBeUndefinedForTransactionType = "{0} flag has value on {1}, but it must be undefined for transaction type '{2}'.";
		public const string FlagsOfControlAccountsWereNotDefinedForType = "Flags of control accounts were not defined for type '{0}'.";
		public const string FAClassUsedInAssets = "You cannot delete Fixed Asset Class because this Class used in Fixed Assets.";
		public const string FADeprMethodUsedInAssets = "You cannot delete Depreciation Method because this Method used in Fixed Assets or Classes.";
		public const string FAClassIsParent = "You cannot delete Fixed Asset Class because this Class is parent for another class.";
		public const string FATypeDeleteUsed = "You cannot delete that Asset Type because it is specified for some fixed asset or fixed asset class.";
		public const string FATypeChangeUsed = "Changes will affect only newly created fixed asset or fixed asset class.";
		public const string UsefulLifeNotMatchDeprMethod = "Useful Life does not match the recovery period specified for selected Depreciation Method.";
		public const string PrevLocationRevisionNotFound = "Previous location of Fixed Asset '{0}' is not found for restoring.";
		public const string QuarterIsUndefined = "Quarter is defined only for the 'Month' and 'Quarter' types of period.";
		public const string InactiveFAClass = "Fixed Asset Class '{0}' is inactive.";
		public const string InvalidDeprToDate = "DepreciationToDate cannot be greater than RecoveryEndDate. Please contact support.";
		public const string UpdateGLBookHasFACalendar = "The Update GL check box cannot be selected for the {0} book because the calendar has been configured for the book on Book Calendars (FA206000).";
		#endregion

		#region Transaction Descriptions
		public const string TranDescPurchase = "Purchase for Asset {0}";
		public const string TranDescSale = "Sale of Asset {0}";
		public const string TranDescDepreciation = "Depreciation for Asset {0}";
		public const string TranDescCostDisposal = "Cost Disposal for Asset {0}";
		public const string TranDescDeprDisposal = "Depreciation Disposal for Asset {0}";
		public const string TranDescDisposalAdj = "Depreciation Adjustment on Disposal for Asset {0}";
		public const string TranDescDepreciationRecap = "Depreciation Recap for Asset {0}";
		public const string TranDescTransferPurchasing = "Transfer Purchasing for Asset {0}";
		public const string TranDescTransferDepreciation = "Transfer Depreciation for Asset {0}";
		public const string ReduceUnreconciledCost = "Deduction Unreconciled Cost for Asset {0}";
		public const string TranDescSplit = "Split of Asset {0}";
		public const string SplitAssetDesc = "split from";
		public const string DocDescReversal = "Full Reversal of Asset {0}";
		public const string TranDescReversal = "{0} - reversed";
		public const string DocDescDispReversal = "Disposal Reversal of Asset {0}";
		public const string TranDescDispReversal = "{0} - disposal reversed";
		#endregion

		public const string Calendar = "Calendar";
		public const string PeriodFieldName = "Period";
		public const string DeprValueFieldName = "Depreciated";
		public const string CalcValueFieldName = "Calculated";
		public const string Dispose = "Dispose";
		public const string CalculateDepreciation = "Calculate Depreciation";
		public const string ProcessAdditions = "Process";
		public const string ViewDocument = "View Register";
		public const string Split = "Split";
		public const string Transfer = "Transfer";
		public const string Reverse = "Reverse";
		public const string DispReverse = "Reverse Disposal";

		public const string ViewBatch = "View Batch";
		public const string ViewAsset = "View Fixed Asset";
		public const string ViewBook = "View Book";
		public const string ViewClass = "View Asset Class";
		public const string ReduceUnreconCost = "Reduce Unreconciled Cost";
		public const string Prepare = "Prepare";
		public const string PrepareAll = "Prepare All";
		public const string ViewDetails = "View Details";
		public const string ShowAssets = "Show Fixed Assets";
		public const string DeleteGeneratedPeriods = "Delete Book Periods";
		public const string FASetup = "Fixed Assets Preferences";
		public const string FixedAssetSearchTitle = "Fixed Asset ID:{0}";

		#region Field Display Names

		public const string AcquiredDate = "Acquisition Date";
		public const string FixedAssetsAccountClass = "Fixed Assets Account Class";
		public const string DeprFromDate = "Depr. From";
		public const string PlacedInServiceDate = "Placed-in-Service Date";
		public const string DeprFromPeriod = "Depr. From Period";
		public const string PlacedInServicePeriod = "Placed-in-Service Period";

		#endregion

		public const string DisposalPrepared = "Prepared Disposal FA Registers";

		#region Button Displays

		public const string CalcDeprProc = "Calc. & Depreciate";
		public const string CalcDeprAllProc = "Calc. & Depreciate All";
		public const string CalcProc = "Calculate";
		public const string CalcAllProc = "Calculate All";
		public const string DeleteProc = "Delete";
		public const string DeleteAllProc = "Delete All";
		public const string Release = PM.Messages.Release;
		public const string ReleaseAll = PM.Messages.ReleaseAll;

		#endregion

		public const string Predefined = "Predefined";
		public const string Custom = "Custom";
	}
}
