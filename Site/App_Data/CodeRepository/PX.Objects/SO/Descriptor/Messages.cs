using System;
using System.Collections.Generic;
using System.Text;
using PX.Data;
using PX.Common;

namespace PX.Objects.SO
{
	[PXLocalizable(Messages.Prefix)]
	public static class Messages
	{
		public const string Prefix = "SO Error";
        public const string Approval = "Approval";

		#region Captions
		public const string ViewDocument = "View Document";
		#endregion

		#region Graph Names
		public const string SODiscountMaint = "Discount Maintenance";
		public const string SODiscountSequenceMaint = "Discount Sequence Maintenance";
		public const string SOOrderEntry = "Sales Order Entry";
		public const string SOShipmentEntry = "Shipment Entry";
		public const string SOInvoiceEntry = "Invoice Entry";
		public const string SOSalesPriceMaintenance = "Sales Price Maintenance";
		public const string SOUpdateDiscounts = "Update Discounts";
		public const string SOUpdateSalesPrice = "Update Sales Price";
		public const string SOSalesPriceImport = "Sales Price Import";
		public const string SOOrder = "Sales Order";
		public const string SOLine = "Sales Order Line";
		public const string BillingAddress = "Billing Address";
		public const string BillingContact = "Billing Contact";
		public const string ShippingAddress = "Shipping Address";
		public const string ShippingContact = "Shipping Contact";
		public const string SOCarrierRates = "Carrier Rates";
		public const string SOPaymentEntry = "SO Payment Entry";
		public const string SOCreate = "Create Transfer Orders";
		public const string SOExternalTaxCalcProcess = "SO avalara Tax Calculation Process";
        public const string SOSetup = "Sales Orders Preferences";
        #endregion

        #region View Names
        public const string CustomerCredit = "CustomerCredit";
        #endregion

		#region DAC Names
		public const string SOAdjust = "Sales Order Adjust";
		public const string SOLineSplit = "Sales Order Line Split";
		public const string SOOrderDiscountDetail = "Sales Order Discount Detail";
		public const string SOSalesPerTran = "SO Salesperson Commission";
		public const string SOTaxTran = "Sales Order Tax";
		public const string SOMiscLine = "Sales Order Misc. Charge";
		public const string SOOrderShipment = "Sales Order Shipment";
		public const string SOShipmentDiscountDetail = "Shipment Discount Detail";
		public const string SOShipLineSplit = "Shipment Line Split";
		public const string SOPackageDetail = "SO Package Detail";
		public const string SOShipment = "Shipment";
		public const string SOShipLine = "Shipment Line";
		public const string ShipmentContact = "Shipment Contact";
		public const string ShipmentAddress = "Shipment Address";
		public const string SOInvoiceDiscountDetail = "SO Invoice Discount Detail";
		public const string SOFreightDetail = "SO Freight Detail";
		public const string SOInvoice = "SO Invoice";
		public const string SOAddress = "SO Address";
		public const string SOContact = "SO Contact";
		public const string Carrier = "Carrier";
		#endregion

		#region Field Names
		public const string OrigOrderQty = "Ordered Qty.";
		public const string OpenOrderQty = "Open Qty.";
		public const string SiteDescr = "Warehouse Description";
		public const string CarrierDescr = "Ship Via Description";
		public const string CustomerID = "Customer ID";
        #endregion

        #region Validation and Processing Messages

        public const string DontApprovedDocumentsCannotBeSelected = "No payment can be created for a sales order with any of the following statuses: Voided, Cancelled, or Pending approval.";
		public const string MustBeUserNumbering = "Invoice Number is specified, Manual Numbering should be activated for '{0}'.";
		public const string UnappliedBalanceIncludesAR = "Includes Unreleased AR Applications";
		public const string MissingShipmentControlTotal = "Control Total is required for shipment confirmation.";
		public const string UnableConfirmZeroShipment = "Unable to confirm zero shipment {0}.";
		public const string UnableConfirmShipment = "Unable to confirm empty shipment {0}.";
		public const string UnableConfirmZeroOrderShipment = "Unable to confirm shipment {0} with zero Shipped Qty. for Order {1} {2}.";
		public const string MissingMassProcessWorkFlow = "Work Flow is not set up for this screen.";
		public const string DocumentOutOfBalance = AR.Messages.DocumentOutOfBalance;
		public const string DocumentBalanceNegative = AR.Messages.DocumentBalanceNegative;
		public const string DocumentBalanceNegativePremiumFreight = "Negative Premium Freight is greater than Document Balance. Document Balance will go negative.";
		public const string AssignNotSetup = "Default Sales Order Assignment Map is not entered in Sales Orders Preferences";
        public const string AssignNotSetup_Shipment = "Default Sales Order Shipment Assignment Map is not entered in Sales Orders Preferences";
		public const string CannotShipComplete_Line = "Shipment cannot be confirmed because the quantity of the item for the line '{0}' with the Ship Complete setting is less than the ordered quantity.";
		public const string CannotShipComplete_Order = "Shipment cannot be confirmed for the order with the order-level Ship Complete setting, because the line '{0}' is not included in the shipment.";
        public const string CannotShipCompleteTraced = "Order {0} {1} cannot be shipped in full. Check Trace for more details.";
        public const string CannotShipTraced = "Order {0} {1} does not contain any available items. Check previous warnings.";
        public const string NothingToShipTraced = "Order {0} {1} does not contain any items planned for shipment on '{2}'.";
        public const string NothingToReceiveTraced = "Order {0} {1} does not contain any items planned for receipt on '{2}'.";
		public const string InvoiceCheck_QtyNegative = "Item '{0} {1}' in invoice '{2}' quantity returned is greater than quantity invoiced.";
		public const string InvoiceCheck_QtyLotSerialNegative = "Item '{0} {1}' in invoice '{2}' lot/serial number '{3}' quantity returned is greater than quantity invoiced.";
		public const string InvoiceCheck_SerialAlreadyReturned = "Item '{0} {1}' in invoice '{2}' serial number '{3}' is already returned.";
		public const string InvoiceCheck_LotSerialInvalid = "Item '{0} {1}' in invoice '{2}' lot/serial number '{3}' is missing from invoice.";
		public const string OrderCheck_QtyNegative = "Item '{0} {1}' in order '{2} {3}' quantity shipped is greater than quantity ordered.";
        public const string OrderSplitCheck_QtyNegative = "For item '{0} {1}' in order '{2} {3}', the quantity shipped is greater than the quantity allocated.";
		public const string BinLotSerialInvalid = "Shipment Scheduling and Bin/Lot/Serial assignment are not possible for non-stock items.";
		public const string BackOrderCannotBeDeleted = "Back Order cannot be deleted.";
        public const string WarehouseWithoutAddressAndContact = "The Multiple Warehouses feature and the Transfer order type are activated in the system, in this case an address and a contact must be configured for the '{1}' warehouse.";
        public const string DestinationSiteContactMayNotBeEmpty = "The document cannot be saved, contact is not specified for selected destination warehouse ";
        public const string DestinationSiteAddressMayNotBeEmpty = "The document cannot be saved, address is not specified for selected destination warehouse ";
        public const string CustomerContactMayNotBeEmpty = "Shipping Contact";
        public const string CustomerAddressMayNotBeEmpty = "Shipping Contact";

		public const string Availability_Info = IN.Messages.Availability_Info;
		public const string Availability_AllocatedInfo = "On Hand {1} {0}, Available {2} {0}, Available for Shipping {3} {0}, Allocated {4} {0}";

		public const string DiscountEngine_InventoryIDIsNull = "Given line is not properly initialized before calling this method. InventoryID property should be set to a valid value.";
		public const string DiscountEngine_QtyIsNull = "Given line is not properly initialized before calling this method. Qty property should be set to a valid value.";
		public const string DiscountEngine_DateIsNull = "Given line is not properly initialized before calling this method. Date property should be set to a valid value.";
		public const string DiscountEngine_CuryExtPriceIsNull = "Given line is not properly initialized before calling this method. CuryExtPrice property should be set to a valid value.";
		public const string ShipmentExistsForSiteCannotReopen = "Another shipment already created for order {0} {1}, current shipment cannot be reopened.";
		public const string ShipmentInvoicedCannotReopen = "Shipment already posted to inventory or invoiced for order {0} {1}, shipment cannot be reopened.";
		public const string ShipmentCancelledCannotReopen = "Order {0} {1} is cancelled, shipment cannot be reopened.";
		public const string PromptReplenishment = "Transfer from Sales-not-allowed location or Replenishment is required for item '{0}'.";
        public const string ItemNotAvailableTraced = "There is no stock available at the {1} warehouse for the {0} item with the {2} subitem.";
		public const string ItemNotAvailable = "There is no stock available for this item.";
		public const string GrossProfitValidationFailed = "Minimum Gross Profit requirement is not satisfied.";
		public const string GrossProfitValidationFailedAndFixed = "Minimum Gross Profit requirement is not satisfied. Discount was reduced to maximum valid value.";
		public const string GrossProfitValidationFailedAndPriceFixed = "Minimum Gross Profit requirement is not satisfied. Sales price was set to minimum valid value.";
		public const string GrossProfitValidationFailedAndUnitPriceFixed = "Minimum Gross Profit requirement is not satisfied. Unit price was set to minimum valid value.";
		public const string GrossProfitValidationFailedNoCost = "No Average or Last cost found to determine minimum valid Unit price.";
		public const string DefaultRateNotSetup = "Default Rate Type is not configured in Accounts Receivable Preferences.";
		public const string FreightAccountIsRequired = "Freight Account is required. Order Type is not properly setup.";
		public const string NoDropShipLocation = "Drop-Ship Location is not configured for warehouse {0}";
		public const string NoRMALocation = "RMA Location is not configured for warehouse {0}";
		public const string FailToInsertSalesPrice = "Failed to preload {0} records from the Inventory.";
		public const string OrderTypeShipmentOrLocation = "Process Shipment or Require Location should be defined when Inventory Transaction Type is specified.";
        public const string OrderTypeShipmentNotLocation = "Require Location should be off if Process Shipment is specified.";
        public const string OrderTypeUnsupportedCombination = "Selected combination of Inventory Transaction Type and AR Document Type is not supported.";
		public const string OrderTypeUnsupportedOperation = "Selected Inventory Transaction Type is not supported for this type operation.";		
		public const string FailedToConvertToBaseUnits = "Failed to convet to Base Units and Check for Minimum Gross Profit requirement";
		public const string SalesPriceSourceDestEquals = "Source and Destination Currency and Price Class are the same.";
		public const string OnlyOneFlatAllowed = "Only one Flat Price discount is allowed for every \"Applicable To\" target.";
		public const string PromotionDateOverlap = "There is already a promotion for item {0} that overlaps the interval from {1} to {2}.";
		public const string MultipleFlatDiscount = "There is already a regular discount for item {0} defined in another discount sequence.";
		public const string CarrierServiceError = "Carrier Service returned error. {0}";
		public const string DefaultWarehouseIsRequired = "Default Warehouse is required to calculate Freight. It's address is used as shipment origin.";
		public const string WarehouseIsRequired = "Warehouse is required. It's address is used as shipment origin.";
		public const string InvalidWeightUOM = "Weight UOM in INSetup is not valid. Expecting KG or POUND.";
		public const string RateTypeNotSpecified = "Carrier returned rates in currency that differs from the Order currency. RateType that is used to convert from one currency to another is not specified in AR Preferences.";
		public const string ReprintLabels = "Labels can be used only once. Are you sure you want to reprint labels?";
		public const string PackagesShipped = "Packages already shipped via Carrier. Labels are generaned. To regenerate labels first you have to Cancel previous shipment.";
		public const string NoPackagesOrder = "Order does not contain any packages.";
        public const string WeightExceedsBoxSpecs = "The weight specified exceeds the max. weight of the box. Choose a bigger box or use multiple boxes.";
		public const string TransferOrderCreated = "Transfer Order '{0}' created.";
		public const string OrderApplyAmount_Cannot_Exceed_OrderTotal = "Amount Applied to Order cannot exceed Order Total";
		public const string AuthorizationMayNotBeEnteredForTheOrderWithZeroBalance = "Authorization Number may not be entered if the order has 0 total or is already payed in full";
		public const string PreAutorizationAmountShouldBeEntered  = "Pre-authorized Amount must be entered if pre-authorization number is provided";
		public const string CannotCancelCCProcessed = "Cannot cancel order. Credit Card pre-authorised or captured.";
		public const string BinLotSerialNotAssigned = IN.Messages.BinLotSerialNotAssigned;
		public const string CarrierUOMIsRequired = "Carrier UOM is required. Please setup UOM for Carrier {0} and retry.";
		public const string ConfirmationIsRequired = "Confirmation for each and every Package is required. Please confirm and retry.";
		public const string MixedFormat = "Your selection contains mixed label format: {0} Thermal Labels and {1} Laser labels. Please select records of same kind and try again.";
		public const string FailedToSaveMergedFile = "Failed to Save Merged file.";
		public const string NoLinesMatchPromo = "No lines match Promo Code.";
		public const string MissingSourceSite = "Missing Source Site to create transfer.";
		public const string EqualSourceDestinationSite = "Source and destination sites should not be the same.";
		public const string EmailSalesOrderError = "Email sales order has failed. Please, check Customer's email or notification configuration";
		public const string PartialInvoice = "Sales Order/Shipment cannot be invoiced partially.";
		public const string TransfertNonStock = "Transfer isn't supported for non stock item";
		public const string ShippedLineDeleting = "Cannot delete line with shipment.";
		public const string ConfirmShipDateRecalc = "Do you want to update order lines with changed requested date and recalculate scheduled shipment date?";
		public const string PlanDateGreaterShipDate = "Scheduled Shipment Date greater than Shipment Date";
		public const string CannotDeleteTemplateOrderType = "Order type cannot be deleted. It is in use as template for order type '{0}'.";
		public const string CannotDeleteOrderType = "Order type cannot be deleted. It is used in transactions.";
		public const string CustomeCarrierAccountIsNotSetup = "Customer Account is not configured. Please setup the Carrier Account on the Carrier Plug-in screen.";
		public const string TaskWasNotAssigned = "Failed to automatically assign Project Task to the Transaction. Please check that the Account-Task mapping exists for the given account '{0}' and Task is Active and Visible in the given Module.";
		public const string PackageIsRequired = "At least one confirmed package is required to confirm this shipment.";
		public const string InvalidInventory = "Invalid Inventory Item. You can only select Inventory Item that exists in the Order.";
		public const string UnbilledBalanceWithoutTaxTaxIsNotUptodate = "Balance does not include Tax. Unbilled Tax is not up-to-date.";
		public const string UseInvoiceDateFromShipmentDateWarning = "Shipment Date will be used for Invoice Date.";
		public const string InvalidShipmentCounters = "Shipment counters are corrupted.";
		public const string SalesOrderWillBeDeleted = "The Sales Order has a payment applied. Deleting the Sales Order will delete the payment reservation. Continue?";

		public const string AtleastOnePackageIsRequired = "When using 'Manual Packaging' option at least one package must be defined before a Rate Quote can be requested from the Carriers.";
		public const string AtleastOnePackageIsInvalid = "Warehouse must be defined for all packages before a Rate Quote can be requested from the Carriers.";
		public const string AutoPackagingZeroPackWarning = "Autopackaging for {0} resulted in zero packages. Please check your settings. Make sure that boxes used for packing are configured for a carrier.";
		public const string AutoPackagingIssuesCheckTrace = "There were some issues with autopackaging please check the Trace information for details.";

        public const string NoOrder = "<NEW>";
        public const string NoOrderType = "IN";
        public const string AccountMappingNotConfigured = "Account Task Mapping is not configured for the following Project: {0}, Account: {1}";
		public const string NoBoxForItem = "Packages is not configured properly for item {0}. Please correct in on Stock Items screen and try again. Given item do not fit in any of the box configured for it.";
		public const string OrderTypeInactive = "Order Type is inactive.";
		public const string CarrierBoxesAreNotSetup = "Carrier {0} do not have any boxes setup for it. Please correct this and try again.";
		public const string CarrierBoxesAreNotSetupForInventory = "Carrier \"{0}\" do not have any boxes setup for \"{1}\"";
		public const string ItemBoxesAreNotSetup = "\"{0}\" do not have any boxes";
		public const string PurchaseOrderCannotBeDeselected = "Purchase Order is fully or partially received and cannot be deselected.";
        public const string DocumentOnHoldCannotBeCompleted = "Sales Order {0} is on Hold and cannot be completed. Operation aborted.";
        public const string LotSerialSelectionForOnReceiptOnly = "Lot/Serial Nbr. can be selected for items with When Received assignment method only.";
        public const string NonStockShipReceiptIsOff = "Require Ship/Receipt is OFF in the Non-Stock settings.";
		public const string CouldNotFindCCTranPayment = "The Payment associated with the credit card transaction could not be found";
		public const string CouldNotVoidCCTranPayment = "The Payment {0} associated with the credit card transaction could not be voided";
		public const string LineQtyNotEqualSplitQty = "Quantity in Line is not equal to Quantity in Split. The Shipment cannot be confirmed.";
        public const string ShipmentPlanTypeNotSetup = "For order type '{0}' and operation '{1}', no shipment plan type is specified on the Order Types (SO.20.20.00) form.";
		public const string SOPOLinkIsIvalidInPOOrder = "In the purchase order '{0}', a drop-ship line with the item '{1}' is not linked to any sales order line.";
		public const string SOPOLinkIsIvalid = "Some of drop-ship lines in the purchase orders are not linked to any sales order lines. Check Trace for more details.";
		public const string SOPOLinkNotForNonStockKit = "Non-Stock kit items cannot be linked with purchase order.";
		public const string ReceiptShipmentRequiredForDropshipNonstock = "Only items for which both 'Require Receipt' and 'Require Shipment' are selected can be drop shipped.";
		public const string ValidationFailed = "One or more rows failed to validate. Please correct and try again.";
		public const string POLinkedToSOCancelled = "Purchase Order '{0}' is cancelled.";
		public const string InventoryIDCannotBeChanged = "You cannot change Inventory ID for the allocation line {0}, because the line has been already completed.";
		public const string InvalidUOM = "The UOM can be changed only to a base one or to UOM originally used in the sales order.";
		public const string OrderHasSubsequentShipments = "The '{0} {1}' sales order has one ('{2}') or more subsequent shipments. The '{3}' shipment cannot be reopened.";
		public const string CustomerChangedOnOrderWithRestrictedCurrency = "Customer cannot be changed. Currency is not allowed for the new customer.";
		public const string CustomerChangedOnOrderWithRestrictedRateType = "Customer cannot be changed. Currency rate type is not allowed for the new customer.";
		public const string CustomerChangedOnShippedOrder = "Customer cannot be changed. Either linked documents or transactions exist for the order.";
		public const string CustomerChangedOnOrderWithCCProcessed = "Customer cannot be changed. Credit card payment is authorized or captured for the order.";
		public const string CustomerChangedOnOrderWithARPayments = "Customer cannot be changed. Payment is applied to the order.";
		public const string CustomerChangedOnOrderWithDropShip = "Customer cannot be changed. Drop-ship purchase order is linked to the order.";
		public const string CustomerChangedOnOrderWithInvoices = "Customer cannot be changed. Order line refers to an invoice.";
		public const string CustomerChangedOnQuoteWithOrder = "Customer cannot be changed.The quote is referred from an order.";
        public const string CannotAddNonStockKitDirectly = "A non-stock kit cannot be added to a document manually. Use the Sales Orders (SO301000) form to prepare an invoice for the corresponding sales order.";
		public const string KitComponentIsInactive = "The '{0}' component of the kit is inactive.";
		public const string NotKitsComponent = "The item cannot be added to the shipment. It is not a component of the kit.";
		#endregion

		#region Translatable Strings used in the code


		public const string SOCreateShipment = "Create Shipment";

		public const string ShipComplete = "Ship Complete";
		public const string BackOrderAllowed = "Back Order Allowed";
		public const string CancelRemainder = "Cancel Remainder";

		public const string Inventory = "Goods for Inventory";
		public const string NonInventory = "Non-Inventory Goods";
		public const string MiscCharge = "Misc. Charge";

		public const string SalesOrder = "Sales Order";
		public const string SalesOrderCustomer = "Customer";
		public const string SalesOrderBillingAddress = "Billing Address";
		public const string SalesOrderShippingAddress = "Shipping Address";
		public const string SalesOrderShipment = "Shipment";
		public const string SalesOrderShipmentCustomer = "Customer";
		public const string SalesOrderShipmentAddress = "Address";
		public const string Assign = CR.Messages.Assign;

		public const string ShipVia = "Ship Via";
		public const string DocDiscDescr = "Document Discount";
		public const string LineDiscDescr = "Item Discount";
		public const string FreightDescr = "Freight ShipVia {0}";
		public const string OrderType = "Order Type";
		public const string AddInvoice = "Add Invoice";

		public const string SelectedWillBeProcessed = "Only the selected items will be processed.";
		public const string AllWillBeCopied = "WARNING: Every item listed will be copied.";
		public const string AllWillBeCalced = "WARNING: Every item listed will be updated.";
		public const string RefreshFreight = "Refresh Freight Cost";

		public const string ShipDate = "Ship Date";
		public const string CancelBy = "Cancel By";
		public const string OrderDate = "Order Date";
		public const string SearchString = "Search String";

		public const string Packages = "Packages";

		public const string EmptyValuesFromAvalara = AP.Messages.EmptyValuesFromAvalara;
		public const string ExternalTaxVendorNotFound = TX.Messages.ExternalTaxVendorNotFound;
		public const string FailedGetFromAddressSO = "Failed to Get 'From' Address from the Sales Order";
		public const string FailedGetToAddressSO = "Failed to Get 'To' Address from the Sales Order";
		public const string CarrierWithIdNotFound = "Carrier with the given ID was not found in the system.";
		public const string CarrierPluginWithIdNotFound = "Carrier Plugin with the given ID was not found in the system";
		public const string ShipViaMustBeSet = "Ship Via must be set before auto packaging.";
		public const string TaxZoneIsNotSet = "TaxZone is not set in the document";
		public const string OrderHaveZeroBalanceError = "Order with authorization may not have zero balance";
		public const string PackagesRecalcErrorReleasedDocument = "Packages cannot be recalculated on a confirmed / released document.";
		public const string PackagesRecalcErrorWarehouseIdNotSpecified = "WarehouseID must be specified before packages can be recalculated.";
		public const string FailedFindSOLine = "Failed to find SOLine associated with the SOShipline when allocating Free Items.";
		public const string CannotMergeFiles = "Cannot merge files with different formats (extensions)";
		#endregion

		#region Order + Shipment Statuses
		public const string Order = "Order";
		public const string Open = "Open";
		public const string Hold = "On Hold";
		public const string CreditHold = "Credit Hold";
		public const string Completed = "Completed";
		public const string Cancelled = "Canceled";
		public const string Confirmed = "Confirmed";
		public const string Invoiced = "Invoiced";
		public const string BackOrder = "Back Order";
		public const string Shipping = "Shipping";
		public const string Receipted = "Receipted";
		public const string AutoGenerated = "Auto-Generated";
		public const string PartiallyInvoiced = "Partially Invoiced";
		#endregion

		#region Shipment Type
		public const string Normal = "Shipment";
		#endregion

		#region Operation Type
		public const string Issue = IN.Messages.Issue;
		public const string Receipt = IN.Messages.Receipt;
		#endregion

		#region Order Behavior
		public const string SOName = "Sales Order";
		public const string INName = "Invoice";
		public const string QTName = "Quote";
		public const string RMName = "RMA Order";
		public const string CMName = "Credit Memo";
		#endregion

		#region AddItemType
		public const string BySite = "All Items";
		public const string ByCustomer = "Sold Since";
		#endregion

		#region Sales Price Update Unit

		public const string BaseUnit = "Base Unit";
		public const string SalesUnit = "Sales Unit";

		#endregion

		#region Custom Actions
		public const string Process = IN.Messages.Process;
		public const string ProcessAll = IN.Messages.ProcessAll;
		#endregion

		#region DiscountAppliedTo
		public const string ExtendedPrice = "Item Extended Price";
		public const string SalesPrice = "Item Price";
		#endregion

		#region FreeItemShipType
		public const string Proportional = "Proportional";
		public const string OnLastShipment = "On Last Shipment";
		#endregion

		#region Freight Allocation
		public const string FullAmount = "Full Amount First Time";
		public const string Prorate = "Allocate Proportionally";
		#endregion

		#region Minimal Gross Profit Validation Allocation
		public const string None = "No Validation";
		public const string Warning = "Warning";
		public const string SetToMin = "Set to minimum";
		#endregion

		#region PackagingType

		public const string PackagingType_Auto = "Auto";
		public const string PackagingType_Manual = "Manual";
		public const string PackagingType_Both = "Auto and Manual";
		#endregion

		public static string GetLocal(string message)
		{
			return PXLocalizer.Localize(message, typeof(Messages).FullName);
		}

	}
}
