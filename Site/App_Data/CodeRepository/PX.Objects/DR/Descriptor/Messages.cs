using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Common;

namespace PX.Objects.DR
{
	[PXLocalizable(Messages.Prefix)]
	public static class Messages
	{
		#region Validation and Processing Messages
		public const string Prefix = "DR Error";
		public const string TranAlreadyExist = "Transactions for the given schedule already exist in the system.";
		public const string NoFinPeriod = "Deferral transactions cannot be generated for deferral code {0} because the financial periods are not configured yet. Configure financial periods for the range of time that your revenue recognition schedule covers.";
		public const string TermEndDateIsEarlierThanDocDate = "Deferral transactions cannot be generated because the end date of the recognition process (Term End Date) is earlier than the document date, but recognition in previous periods is not enabled in the deferral code {0}.";
		public const string DeferredAmountSumError = "Sum of all open deferred transactions must be equal to Deferred Amount.";
		public const string InventoryIDRequired = "InventoryID for the Tran must be set for Deferred Revenue/Expense";
		public const string AutoPostFailed = "Auto-Posting failed for one or more Batch.";
		public const string InvalidAccountType = "Invalid Deferral Account Type. Only {0} and {1} are supported but {2} was supplied.";
		public const string WrongDefCodeType = "Deferred Code Type does not match the Document Type";
		public const string RegenerateTran = "Transactions already exist. Do you want to recreate them?";
        public const string FixedAmtSumOverload = "The sum of fixed component amounts is greater than the line amount. Please correct this and try again.";
		public const string CannotSplitLineAmountIntoComponents = "Cannot split the line amount into deferral components: ";
		public const string DeferralCodeNotMDA = CannotSplitLineAmountIntoComponents + "for bundle items, the item-level deferral code should be marked as a Multiple Deliverable Arrangement.";
		public const string NoResidual = CannotSplitLineAmountIntoComponents + "the sum of fixed component amounts is not equal to the line amount, and there is no residual component.";
		public const string TooManyResiduals = CannotSplitLineAmountIntoComponents + "the item contains more than one residual component.";
		public const string MDACodeButNoInventoryItem = CannotSplitLineAmountIntoComponents + "an MDA deferral code is specified, but the document line contains no inventory item.";
		public const string SumOfComponentsError = "The sum of total amounts of all components must match the line amount.";
		public const string ValidationFailed = "Can not save changes. Validation failed.";
		public const string GenerateTransactions = "Generate Transactions";
		public const string DuplicateSchedule = "For the selected line, the deferral schedule {0} already exists.";
		public const string DuplicateComponent = "Component ID must be unique within components.";
		public const string NonExistentLineNumber = "The specified line number does not exist.";
		public const string TermCantBeNegative = "Term End Date ({0:d}) must be greater than Term Start Date ({1:d})";
		public const string TermNeedsStartDate = "Term Start Date should be specified explicitly when Term End Date is present.";
		public const string CannotGenerateTransactionsWithoutTerms = "Deferral transactions cannot be generated unless both Term Start Date and Term End Date are specified in the deferral schedule.";
        public const string MDANotAllowedForItem = "An MDA code is allowed only with Multiple-Deliverable Arrangement items";
		public const string DefaultTermMustBeTheSameForAllComponents = "Default term must be the same for all components of the item";
		public const string ComponentsCantUseMDA = "Multi-Deliverable Arrangement codes can't be used on components";
		public const string NoPriceListWithResidual = "There is no price defined for the item. Residual component won't be used when generating a deferral schedule - amount will be allocated between Fixed Amount and Percentage components only.";
		public const string ResidualWillGoNegative = "Can't create document - residual amount will go negative. Please, adjust prices and discounts.";
		public const string ResidualWillGoNegativeWithCustomAmounts = "Can't update the schedule - residual amount will go negative. Please, adjust amounts for components.";
		public const string NegativeAmountForComponent = "Splitting deferred amount between components results in negative amount for component '{0}'. Can't create deferral schedule. Please correct Fixed Amount components.";
		public const string MDAInUse = "The MDA code is used on some items - can't convert to common code.";
		public const string CodeInUseCantDelete = "Can't delete the Deferral Code because it is in use. There is at least one '{0}' record that refers it.";
		public const string ScheduleAlreadyReleased = "The schedule is already released.";
		public const string CantRegeneratePostedTransactions = "The transactions cannot be regenerated because some transactions have been already posted.";
		public const string CannotDeletePostedTransaction = "Posted transactions cannot be deleted from the schedule.";
		public const string NoZeroOccurrencesForRecognitionMethod = "The number of occurrences should not be equal to zero.";
		public const string AccountTypeIsNotLiability = "Account type is not 'Liability'.";
		public const string AccountTypeIsNotAsset = "Account type is not 'Asset'.";
		#endregion

		#region UI Field Display Names
		public const string ComponentID = "Component ID";
		public const string Date = "Date";
		public const string LineNumber = "Line Nbr.";
		public const string LineAmount = "Line Amount";
		public const string DocumentType = "Doc. Type";
		public const string TransactionNumber = "Tran. Nbr.";
		#endregion

		#region Translatable Strings used in the code
		public const string DocumentDateSelection = "Document Date Selection";
		public const string StartOfFinancialPeriod = "Start of Financial Period";
		public const string EndOfFinancialPeriod = "End of Financial Period";
		public const string FixedDayOfThePeriod = "Fixed Day of the Period";
		public const string AddNewDeferralSchedule = "Add New Deferral Schedule";
		public const string EditDeferralSchedule = "Edit Deferral Schedule";
		public const string CollectionContainsPostedTransactions = "The collection contains posted transactions.";
		public const string CollectionContainsNonPostedTransactions = "The collection contains non-posted transactions.";
		public const string ScheduleID = "Schedule ID";
		public const string EmptyComponentCD = "<NONE>";
		public const string AllFieldsMustBelongToTheSameType = "All fields must belong to the same declaring type.";
		public const string UnexpectedModuleSpecified = "Unexpected module specified. Only AP and AR are supported.";
		public const string UnexpectedDocumentLineModule = "Unexpected document line module specifier, should be AR or AP.";
		#endregion

		#region Graph/Cache Names
		public const string SchedulesInq = "Deferred Schedules Inquiry";
		public const string ScheduleTransInq = "Deferred Transactions Inquiry";
		public const string ScheduleMaint = "Deferred Shedule Maintenance";
		public const string ScheduleMaint2 = "Deferred Shedule Maintenance 2.0";
		public const string SchedulesRecognition = "Deferred Schedule Recognition";
		public const string DeferredCodeMaint = "Deferred Code Maintenance";
		public const string SchedulesProjection = "Update Deferred Schedule Projection";
        public const string DRDraftScheduleProc = "Draft Schedules Release";
        public const string DRBalanceValidation = "Deferred Balance Validation";
		public const string Schedule = "Deferral Schedule";
		public const string DeferredCode = "Deferral Code";

		public const string ViewDocument = "View Document";
		public const string ViewSchedule = "View Schedule";
		public const string ViewGLBatch = "View GL Batch";
		public const string Release = "Release";
		public const string CreateTransactions = "Create Transactions";
        public const string DRScheduleEx = "Associated Schedule";
		public const string DRScheduleDetail = "Deferral Schedule Component";
		public const string DRScheduleTran = "DRScheduleTran";

		public const string DRRevenueBalance = "DR Revenue Balance";
		public const string DRExpenseBalance = "DR Expense Balance";
		public const string DRRevenueBalanceByPeriod = "DR Revenue Balance by Period";
		public const string DRExpenseBalanceByPeriod = "DR Expense Balance by Period";
		public const string DRRevenueProjection = "DR Revenue Projection";
		public const string DRExpenseProjection = "DR Expense Projection";
		#endregion

		#region Combo Values

		public const string Open = "Open";
		public const string Closed = "Closed";
		public const string Posted = "Posted";
		public const string Projected = "Projected";
		public const string Draft = "Draft";
		public const string MaskDeferralCode = "Deferral Code";
		public const string MaskItem = "Item";
		#endregion

		#region Deferred Code Type
		public const string Income = "Revenue";
		public const string Expense = "Expense";

		public const string Shedule = "Schedule";
		public const string CashReceipt = "On Payment";
		#endregion

		#region Deferred Method Type
        public const string EvenPeriods = "Evenly by Periods";
        public const string ProrateDays = "Evenly by Periods, Prorate by Days";
        public const string ExactDays = "Evenly by Days in Period";
		public const string FlexibleProrateDays = "Flexible by Periods, Prorate by Days";
		public const string FlexibleExactDays = "Flexible by Days in Period";
		#endregion

		#region Deferral Term
		public const string Day = "day(s)";
		public const string Week = "week(s)";
		public const string Month = "month(s)";
		public const string Year = "year(s)";
		#endregion

	}
}
