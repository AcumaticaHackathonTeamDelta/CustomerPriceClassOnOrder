using System;
using PX.Data;
using PX.Common;

namespace PX.Objects.PO
{
	[PXLocalizable(Messages.Prefix)]
	public static class Messages
	{
		// Add your messages here as follows (see line below):
		// public const string YourMessage = "Your message here.";
		#region Validation and Processing Messages
		public const string Prefix = "PO Error";
		public const string DocumentOutOfBalance = "Document is out of balance.";
		public const string DuplicateVendorDoc = "Document with reference number '{0}' dated '{1}' already exists for this Vendor."; //Used
        public const string AssignNotSetup = "Default Purchase Order Assignment Map is not entered in PO setup";
        public const string AssignNotSetup_Receipt = "Default Purchase Order Receipt Assignment Map is not entered in PO setup";
		public const string Document_Status_Invalid = "Document Status is invalid for processing.";
        public const string POOrderLineHasReceiptsAndCannotBeDeleted = "The line cannot be deleted because some quantity of an item in this line have been received.";
        public const string POOrderHasReceiptsAndCannotBeDeleted = "The order cannot be deleted because some quantity of items for this purchase order have been received.";
        public const string NotApprover = "You are not approver for this document.";
		public const string ReceiptLineQtyGoNegative = "Receipt quantity will go negative source PO Order line";	
		public const string ReceiptLineQtyDoesNotMatchMinPOQuantityRules = "Receipt quantity is below then minimum quantity defined in PO Order for this item";
		public const string ReceiptLineQtyDoesNotMatchMaxPOQuantityRules = "Receipted quantity is above the maximum quantity defined in PO Order for this item";
		public const string LandedCostReceiptTotalCostIsZero = "This Landed Cost is allocated by Amount , but Receipt total Amount is equal to zero";
		public const string LandedCostReceiptTotalVolumeIsZero = "This Landed Cost is  allocated by Volume, but Receipt total Volume is equal to zero";
		public const string LandedCostReceiptTotalWeightIsZero = "This Landed Cost is allocated by Weight , but Receipt total Weight is equal to zero";
		public const string LandedCostReceiptTotalQuantityIsZero = "This Landed Cost is allocated by Quantity , but Receipt total Quantity is equal to zero";
		public const string LandedCostReceiptNoReceiptLines = "This Receipt does not have detail lines";
		public const string LandedCostUnknownAllocationType = "Unknown Landed Cost alloction type";
		public const string LandedCostAmountRemainderCannotBeDistributed = "Amount {0} cannot be distributed for the Landed Cost Trans {1} - there is no applicable PO Receipt Lines";
		public const string LandedCostProcessingForOneOrMorePOReceiptsFailed = "At least one of the Document selected has failed to process";
		public const string SourcePOOrderExpiresBeforeTheDateOfDocument = "Originating Blanket Order# {1} expires before the date of current Document - on {0:d}";
		public const string POBlanketOrderExpiresBeforeTheDateOfDocument = "Blanket Order# {1} expires before the date of currend Document- on {0:d}";
		public const string LCVarianceAccountsOrSubAreNtFound = "Landed Cost Variance account/sub for the Inventory Item '{0}' cannot be found";
		public const string PostingClassIsNotDefinedForTheItemInReceiptRow = "Posting class is not defined for the Inventory Item '{0}' in PO Receipt# '{1}' line {2}";
        public const string ShippingContactMayNotBeEmpty = "The document cannot be saved, shipping contact is not specified  for the warehouse selected  on the *Shipping Instructions* tab.";
        public const string ShippingAddressMayNotBeEmpty = "The document cannot be saved, shipping address is not specified  for the warehouse selected  on the *Shipping Instructions* tab.";
        public const string VendorAddressMayNotBeEmpty = "Shipping Contact";
        public const string VendorContactMayNotBeEmpty = "Shipping Contact";
        public const string LCVarianceAccountCanNotBeFoundForItemInReceiptRow = "Landed Cost Variance Account can't be found for the Inventory Item '{0}' in PO Receipt# '{1}' line {2}. Please, check the settings in the IN Post Class '{3}', Inventory Item and Warehouse '{4}'";
		public const string LCVarianceSubAccountCanNotBeFoundForItemInReceiptRow = "Landed Cost Variance Subaccount can't be found for the Inventory Item '{0}' in PO Receipt# '{1}' line {2}. Please, check the settings in the IN Post Class '{3}', Inventory Item and Warehouse '{4}'";
        public const string COGSAccountCanNotBeFoundForItemInReceiptRow = "GOGS Account can't be found for the Inventory Item '{0}' in PO Receipt# '{1}' line {2}. Please, check the settings in the IN Post Class '{3}', Inventory Item and Warehouse '{4}'";
        public const string COGSSubAccountCanNotBeFoundForItemInReceiptRow = "COGS Subaccount can't be found for the Inventory Item '{0}' in PO Receipt# '{1}' line {2}. Please, check the settings in the IN Post Class '{3}', Inventory Item and Warehouse '{4}'";
		public const string POAccrualSubAccountCanNotBeFoundForItemInReceiptRow = "PO Accrual Subaccount can't be found for the Inventory Item '{0}' in PO Receipt# '{1}' line {2}. Please, check the settings in the IN Post Class '{3}', Inventory Item and Warehouse '{4}'";
		public const string LCInventoryItemInReceiptRowIsNotFound = "Inventory Item '{0}' used in PO Receipt# '{1}' line {2} is not found in the system";
		public const string UnknownLCAllocationMethod = "Unknown Landed Cost Allocation Method for Landed Cost Code '{0}'";
		public const string POReceiptLineDoesNotReferencePOOrder = "This line does not reference PO Order";
		public const string INReceiptMustBeReleasedBeforeLCProcessing = "IN Receipt# '{0}' created from PO Receipt# '{1}' must be released before the Landed Cost may be processed";
		public const string OrderLineQtyExceedsQuantityInBlanketOrder = "Order Quantity is above the  quantity {0} defined in the Blanket Order# '{1}' for this item";
		public const string QuantityReceivedForOrderLineExceedsOrdersQuatity = "Quantity received is already above the order's quantity for this line";
		public const string ReleaseOfOneOrMoreReceiptsHasFailed = "Release of one or more PO Receipts  has failed";
		public const string VendorPriceRecordMustBeUniqueForVendorLocationInventorySubUOMCombination = "Price must be unique for the specific Vendor Location,Inventory Item, Sub Item and UOM combination. Such a record already exists.";
		public const string VendorPriceRecordMustBeUniqueForVendorLocationInventoryUOMCombination = "Price must be unique for the specific Vendor Location,Inventory Item and UOM combination. Such a record already exists.";
		public const string VendorPriceRecordMustBeUniqueForVendorLocationInventoryUOMCombinationCheckUOM = "Price must be unique for the specific Vendor Location,Inventory Item and UOM combination. Please, check the UOM or Inventory Item.";
		public const string VendorPriceRecordMustBeUniqueForVendorLocationInventorySubItemUOMCombinationCheckUOM = "Price must be unique for the specific Vendor Location,Inventory Item, Sub Item and UOM combination. Please, check the UOM, Inventory Item or SubItem.";
		public const string PurchaseOrderHasTypeDifferentFromOthersInPOReceipt = "Purchase Order {0} {1} cannot be added - it's has type other then other orders in this Receipt";
		public const string PurchaseOrderOnHoldWithReceipt = "Unable to hold Purchase Order {0} {1} - it's has not released Receipt {2}";
        public const string PurchaseOrderHasShipDestinationDifferentFromOthersInPOReceipt = "Purchase Order {0} {1} cannot be added because it has a different shipping destination than other orders in this receipt.";
		public const string SomeOrdersMayNotBeAddedTypeOrShippingDestIsDifferent = "Selected Purchase Orders cannot be added. Selected Orders must have same Currency, Type and Shipping Destinations.";
		public const string PurchaseOrderHasCurrencyDifferentFromPOReceipt = "Purchase Order {0} {1} has currency other then one of current Purchase Receipt";
		public const string DuplicatePOVendorRefDoc = "PO Receipt Document with this Vendor Ref. already exists for this Vendor - see document '{0}' with reference number '{1}' dated '{2}' .";
		public const string DuplicateAPVendorRefDoc = "AP Document with this Vendor Ref. already exists for this Vendor - see document '{0}' with reference number  '{1}' dated '{2}' .";
		public const string DuplicateLCVendorRefDocInSamePOReceipt = "Value for the 'Invoice Nbr.' is already used in another Landed Cost record in this document. This will lead to the duplication of the 'Vendor Ref.' for AP Documents created from these records";
		public const string DifferentInvoiceDateInLCVendorRefDocInSamePOReceipt = "Two or more Landed Cost rows have the same Invoice Number, but different Invoice dates. This will lead to creation of the several AP Documents  with the same 'Vendor Ref.'";
		public const string DifferentCurrencyInLCVendorRefDocInSamePOReceipt = "Two or more Landed Cost rows have the same Invoice Number, but different currency. This will lead to different AP Documents  with the same 'Vendor Ref.'";
		public const string DifferentTermsInLCVendorRefDocInSamePOReceipt = "Two or more Landed Cost rows have the same Invoice Number, but different Terms. This will lead to different AP Documents  with the same 'Vendor Ref.'";
		public const string DuplicateLCVendorRefDocInAnotherPOReceipt = "Value for the Invoice Nbr. is already used in another Landed Cost record - see PO Receipt with reference number '{0}'. This will lead to the duplication of the 'Vendor Ref.' for AP Documents created from these records";
		public const string LandedCostTranUsesTheSameInvoiceNbrThenItsPOReceiptDocument = "The value '{0}' is also used as 'Vendor Ref.' in this PO Receipt Document. This will lead to the duplication of 'Vendor Ref.' in the resulting AP Documents";
		public const string LandedCostTranIsReferenced = "There are AP or IN documents created from this row. It cannot be deleted";
        public const string LandedCostCannotBeDistributed = "Landed Cost is not applicable for selected transfers";
        public const string LandedCostAmountRemainderCannotBeDistributedMultyLines = "Landed Cost Amount cannot be distributed for the one or more Landed Cost Trans - there is no applicable PO Receipt Lines";
		public const string NoApplicableLinesForLCTransOnlyAPDocumentIsCreated = "No applicable lines has been detected for one or more LC transactions. Only AP Bills were created for these LC lines";
		
		public const string VendorPriceRecordMustBeUniqueForVendorVendorLocationInventorySubCombination = "Price must be unique for the specific Vendor, Vendor Location,Inventory Item and Sub Item combination. Such a record already exists.";
		public const string POReceiptFromOrderCreation_NoApplicableLinesFound = "There are no lines in this document that may be entered in PO Receipt Document";
		public const string APInvoicePOOrderCreation_NoApplicableLinesFound = "There are no lines in this document that may be entered in AP Bill Document directly";
		public const string ReceiptNumberBelongsToDocumentHavingDifferentType = "Document with number {0} already exists but it has a type {1}. Document Number must be unique for all the PO Receipts regardless of the type";
		public const string UnreasedAPDocumentExistsForPOReceipt = "There is one or more unreleased AP documents created for this document. They must be released before another AP Document may be created";
	    public const string AllTheLinesOfPOReceiptAreAlreadyBilled = "AP documents are already created for all the lines of this document.";
		public const string OrderQtyShouldBeNonZeroForStockItems = "Order Qty should not be 0 for the stock items";
		public const string UnitCostShouldBeNonZeroForStockItems = "Unit Cost should not be 0 for the stock items";
		public const string QuantityReducedToBlanketOpen = "Order Quantity reduced to Blanket Order: '{0}' Open Qty. for this item";
		public const string LCCodeUsesNonLCVendor = "This Code uses a vendor which has 'Landed Cost Vendor' set to 'off'. You should correct vendor or use another one";
		public const string POOrderHasUnreleaseReceiptsAndCantBeCancelled = "This Order can not be cancelled - there is one or more unreleased PO Receipts referencing it";
		public const string POLineHasUnreleaseReceiptsAndCantBeCompletedOrCancelled = "This line can not be completed or cancelled - there is one or more unreleased PO Receipts referencing it";
		public const string POReceiptLinePOLineHasUnreleaseReceiptsAndCantBeCompleted= "PO line can not be completed - there are other unreleased PO Receipts referencing it";
		public const string EmailPurchaseOrderError = "Email purchase order has failed. Please, check Vendor's email or notification configuration";
        public const string POSourceNotFound = "There are no lines in open purchase orders that match specified criteria.";
        public const string INSourceNotFound = "There are no unreceived lines in transfer orders that match specified criteria.";
		public const string ItemNotFountByBarCode = "There is no inventory item for entered barcode.";
		public const string BarCodeAddToItem = "Barcode will be added to inventory item.";
		public const string POOrderPromisedDateChangeConfirmation = "Changing of the purchase order 'Promised on' date will reset 'Promised' dates for all it's details to their default values. Continue?";
        public const string POOrderOrderDateChangeConfirmation = "Changing of the purchase order date will reset the 'Requested' and 'Promised' dates for all order lines to new values. Do you want to continue?";
		public const string INDocumentFailedToReleaseDuringPOReceiptRelease = "IN Document has been created but failed to release with the following error: '{0}'. Please, validate a created IN Document";
        public const string APDocumentFailedToReleaseDuringPOReceiptRelease = "Release of AP document failed with the following error: '{0}'. Please validate the AP document";
		public const string POOrderQtyValidation = "Insufficient quantity requested. Line quantity was changed to match.";		
		public const string POLineQuantityMustBeGreaterThanZero = "Quantity must be greater than 0";
		public const string BinLotSerialNotAssigned = IN.Messages.BinLotSerialNotAssigned;
		public const string ReceiptAddedForPO = "Item {0}{1} receipted {2} {3} for Purchase Order {4}";
		public const string ReceiptAdded = "Item {0}{1} receipted {2} {3}";
		public const string Availability_Info = IN.Messages.Availability_Info;
		public const string VendorIsNotLandedCostVendor = "Vendor is not landed cost vendor.";
		public const string UnsupportedLineType = "Selected line type is not allowed for current inventory item.";
		public const string POVendorInventoryDuplicate = "Unable to propagate selected item for all locations there's another one.";
		public const string UnitCostMustBeNonNegativeForStockItem = "A value for the Unit Cost must not be negaive for Stock Items";
		public const string POOrderTotalAmountMustBeNonNegative = "'{0}' may not be negative";
		public const string POReceiptTotalAmountMustBeNonNegative = "'{0}' may not be negative";
        public const string LineCannotBeReceiptedTwicePO = "The item in this transfer line is already received according to the '{1}' line of the '{0}' PO receipt.";
        public const string LineCannotBeReceiptedTwiceIN = "The item in this transfer line is already received according to the '{1}' line of the '{0}' IN transaction.";
        public const string DiscountGreaterLineTotal = "Discount Total may not be greater than Line Total";
		public const string ValueNoAccessible = "Value '{0}' is no More Accessible";
		public const string PrepaymentNotUpdated = "Prepayment was not updated: ";

		public const string PPVAccountCanNotBeFoundForItemInReceiptRow = "Purchase Price Variance Account can't be found for the Inventory Item '{0}' in PO Receipt# '{1}' line {2}. Please, check the settings in the Reason Code '{3}'";
		public const string PPVSubAccountCanNotBeFoundForItemInReceiptRow = "Purchase Price Variance Subaccount can't be found for the Inventory Item '{0}' in PO Receipt# '{1}' line {2}. Please, check the settings in the Reason Code '{3}', Inventory Item and Warehouse '{4}'";
		public const string PPVInventoryItemInReceiptRowIsNotFound = "Inventory Item '{0}' used in PO Receipt# '{1}' line {2} is not found in the system";
		public const string ThisEntityNotBeDeletedBecauseItIsUsedIn = "This '{0}' cannot be deleted because it is used in '{1}'.";
		public const string DeductibleVATNotAllowed = "Deductible VAT is not supported for stock items and non-stock items that require receipts";
		public const string ProjectTaskIsNotAssociatedWithAnyInLocation = "Given Project Task is not associated with any Warehouse Location and cannot be used with a Stock-Item. Either select another Project Task or map it to the warehouse location.";
		public const string ProjectTaskIsNotAssociatedWithCurrentInLocation = "Given Project Task is not associated with selected Warehouse Location and cannot be used with a Stock-Item. Either change the warehouse or select another Project Task.";
		public const string ProjectTaskIsAssociatedWithAnotherWarehouse = "Given Project Task is associated with another Warehouse - '{0}'. Either change the warehouse or select another Project Task.";
		public const string LocationIsNotMappedToTaskWarning = "Given location is not associated with Project Task selected on Purchase Order line.";
		public const string LocationIsNotMappedToTaskError = "Given location is not associated with Project Task selected on Purchase Order line. Since current document explicitly requires approval and was approved Project Task cannot be changed on the Receipt.";
		public const string TaskDiffersWarning = "Given Task differs from the Project Task selected on Purchase Order line.";
		public const string TaskDiffersError = "Given Task differs from the Project Task selected on Purchase Order line. Since Purchase Order document explicitly requires approval and was approved Project Task cannot be changed on the Receipt.";
		public const string DefaultVendorDeactivated = "Default Vendor cannot be deactivated.";
		public const string DuplicateSerialNumbers = "One or more records with duplicate serial numbers found in the document.";
		public const string ContainsDuplicateSerialNumbers = "Contains duplicate serial numbers.";
		public const string POLineLinkedToSOLine = "Deletion of the purchase order line will unlink sales order '{0}' from this purchase order. Do you want to continue?";
		public const string ChangingInventoryForLinkedRecord = "This line is linked to a sales order line. To replace the item in this line, you have to replace it in the linked sales order first.";
		public const string DropshipReceiptWithInItem = "There is an invalid line with Line Type = 'Goods for IN' in the 'Drop Ship' Receipt.";
		public const string DropshipReceiptLinePOLinkInvalid = "'Drop Ship' Line with item '{0}' is not linked with a line in Purchase Order.";
		public const string CustomerCannotBeChangedForOrderWithActiveDemand = "A customer to whom the drop-ship order is shipped must match the customer specified in the linked sales order.";
		public const string POLineLinkedToSO = "This line is linked to Sales Order and cannot be reopened.";
		#endregion

		#region Translatable Strings used in the code

		public const string AskConfirmation = "Confirmation";
		public const string Warning = "Warning";
		public const string ReprintCaption = "Reprint";
		public const string PurchaseOrder = "Purchase Order";
		public const string PurchaseOrderVendor = "Vendor";
		public const string PurchaseOrderShippingAddress = "Shipping Address";
		public const string PurchaseOrderReceipt = "Purchase Order Receipt";
		public const string PurchaseOrderReceiptVendor = "Receipt Vendor";
		public const string Assign = CR.Messages.Assign;
		public const string Release = "Release";
		public const string AddPOOrder = "Add PO";
		public const string AddTransfer = "Add Transfer";
        public const string AddTransferLine = "Add Transfer Line";
		public const string AddBlanketOrder = "Add Blanket PO";
		public const string ReceivePOOrder = "Receive PO Order";
		public const string ViewPOOrder = "View PO";
		public const string AddPOOrderLine = "Add PO Line";
		public const string AddBlanketLine = "Add Blanket PO Line";
		public const string AddPOReceipt = "Add PO Receipt";
		public const string AddPOReceiptLine = "Add PO Receipt Line";
		public const string AddLine = "Add Line";
		public const string ViewINDocument = "View IN Document";
		public const string ViewAPDocument = "View AP Document";
		public const string ViewPODocument = "View PO Document";
		public const string NewVendor = "New Vendor";
		public const string ViewDemand = "View SO Demand";
		public const string EditVendor = "Edit Vendor";
		public const string NewItem = "New Item";
		public const string EditItem = "Edit Item";
		public const string ReportPOReceiptBillingDetails = "Purchase Receipt Billing Details";
		public const string ReportPOReceipAllocated = "Purchase Receipt Allocated and Backordered";
		public const string Process = "Process";
		public const string ProcessAll = "Process All";
		public const string Document = "Document";
		public const string CreatePOReceipt = "Enter Receipt";
		public const string CreateAPInvoice= "Enter AP Bill";
		public const string POReceiptRedirection = "Switch to PO Receipt";
		public const string APReceiptRedirection = "Switch to AP Bill";
		public const string AllowComplete = "Allow Complete";
		public const string AllowOpen = "Allow Open";
		public const string EmptyValuesFromAvalara = AP.Messages.EmptyValuesFromAvalara;
		public const string ExternalTaxVendorNotFound = TX.Messages.ExternalTaxVendorNotFound;
		public const string FailedGetFromAddressSO = SO.Messages.FailedGetFromAddressSO;
		public const string FailedGetToAddressSO = SO.Messages.FailedGetToAddressSO;

		#endregion

		#region Graph and Cache Names
		public const string POSetupMaint = "Purchasing Preferences";
		public const string POOrderEntry = "Purchase Order Entry";
		public const string POOrderApproval = "Approve Purchase Order";
		public const string POPrintOrder = "Print Purchase Order";
		public const string POReceiptEntry = "PO Receipt Entry";
		public const string POInvoiceEntry = "PO Invoice Entry";
		public const string POVendorCatalogueMaint = "PO Vendor Catalogue Maintenance";
		public const string POVendorCatalogueEnq = "PO Vendor Catalogue Details";
		public const string LandedCostCodeMaint = "Landed Cost Code Maintenance";
		public const string LandedCostTranProcessing = " Landed Cost Transaction Processing";
		public const string POReceiptLandedCostTranProcessing = "PO Receipt Landed Cost Processing";
		public const string POReceiptRelease = "PO Receipt Processing";

		public const string POOrder = "Purchase Order";
		public const string POReceipt = "Purchase Receipt";
		public const string POReceiptLine = "Purchase Receipt Line";		
		public const string POLine = "Purchase Order Line";
		public const string PORemitAddress = "Remit Address";
		public const string PORemitContact = "Remit Contact";
		public const string POShipAddress = "Ship Address";
		public const string POShipContact = "Ship Contact";
        public const string VendorContact = "Vendor Contact";
        public const string Approval = "Approval";
		public const string LandedCostCode = "Landed Cost Code";
		public const string VendorLocation = "Vendor Location";
        public const string InventoryItemVendorDetails = "Inventory Item Vendor Details";
        public const string LandedCostTran = "Landed Cost";
        public const string PORemitAddressFull = "PO Remittance Address";
        public const string POShipAddressFull = "PO Shipping Address";
        public const string POAddress = "PO Address";
        public const string PORemitContactFull = "PO Remittance Contact";
        public const string POShipContactFull = "PO Shipping Contact";
        public const string POContact = "PO Contact";
        public const string POLineS = "PO Line to Add";
        public const string POLineShort = "PO Line";

        public const string POOrderDiscountDetail = "Purchase Order Discount Detail";
        public const string POReceiptDiscountDetail = "Purchase Receipt Discount Detail";
		#endregion

		#region Order Type
		public const string Blanket = "Blanket";
		public const string StandardBlanket = "Standard";
		public const string RegularOrder = "Normal";
		public const string DropShip = "Drop Ship";
		//public const string Transfer = "Transfer Request";

		#endregion

		#region Shipping Destination
        public const string ShipDestCompanyLocation = "Branch";
        public const string ShipDestCustomer = "Customer";
		public const string ShipDestVendor = "Vendor";
		public const string ShipDestSite = "Warehouse";
		#endregion

        #region Default Receipt Quantity
        public const string OpenQuantity = "Open Quantity";
        public const string ZeroQuantity = "Zero";
        #endregion

		#region Order Line Type
		public const string GoodsForInventory = "Goods for IN";
		public const string GoodsForSalesOrder = "Goods for SO";
		public const string GoodsForReplenishment = "Goods for RP";
		public const string GoodsForDropShip = "Goods for Drop-Ship";
		public const string NonStockForDropShip = "Non-Stock for Drop-Ship";
		public const string NonStockForSalesOrder = "Non-Stock for SO";
		public const string NonStockItem = "Non-Stock";
		public const string Service = "Service";
		public const string Freight = "Freight";
		public const string MiscCharges = "Misc. Charges";
		public const string Description = "Description";

		#endregion

		#region Report Order Type
		public const string PrintBlanket = "Blanket";
		public const string PrintStandardBlanket = "Stadard";
		public const string PrintRegularOrder = "Normal";
		public const string PrintDropShip = "Drop Ship";
		#endregion

		#region Report Order Type
		public const string PrintInvoice = "BILL";
		public const string PrintCreditAdj = "CRADJ";
		public const string PrintDebitAdj = "DRADJ";
		public const string PrintCheck = "CHECK";
		public const string PrintPrepayment = "PREPAY";
		public const string PrintRefund = "REF";
		public const string PrintVoidCheck = "VOIDCK";
		#endregion

		#region Order Status
		public const string Hold = "On Hold";
		public const string Open = "Open";
		public const string Closed = "Closed";
		public const string Cancelled = "Canceled";
		public const string PendingPrint = "Pending Printing";
		public const string PendingEmail = "Pending Email";
		public const string Printed = "Printed";
		public const string Approved = "Approved";
		public const string Rejected = "Rejected";
		public const string PrintOrder = "Print PO Order";
		#endregion

		#region Receipt Status
		public const string Balanced = "Balanced";
		public const string Released = "Released";
		#endregion

		#region Vendor Update Settigs
		public const string VendorUpdateNone = "None";
		public const string VendorUpdatePurchase = "On PO Entry";
		public const string VendorUpdateReceipt = "On Receipt Entry";
		public const string VendorUpdateReceiptRelease = "On Receipt Release";
		public const string VendorUpdateAPBillRelease = "On AP Bill Release";
		
		#endregion

		#region PO Receipt Quantity Action
		public const string Accept = "Accept";
		public const string AcceptButWarn = "Accept but Warn";
		public const string Reject = "Reject";
		#endregion

		#region PO Print Order Action
		public const string Print = "Print";
		public const string PrintAll = "Print All";
		#endregion

		#region PO Vendor Catalogue Actions
        public const string ShowVendorPrices = "Vendor Prices";
		#endregion

		#region AP Mask Codes
		public const string MaskItem = "Non-Stock Item";
		public const string MaskLocation = AP.Messages.MaskLocation;
		public const string MaskEmployee = "Employee";
        public const string MaskCompany = "Branch";
        #endregion

		#region LandedCostAllocationMethods
		public const string ByQuantity = "By Quantity";
		public const string ByCost = "By Cost";
		public const string ByWeight = "By Weight";
		public const string ByVolume = "By Volume";
		public const string None = "None";

		#endregion

		#region LandedCostApplicationMethod
		public const string FromAP = "From AP";
		public const string FromPO = "From PO";
		public const string FromBoth = "From Both";
		#endregion

		#region LandedCostType
		public const string FreightOriginCharges = "Freight & Misc. Origin Charges";
		public const string CustomDuties = "Customs Duties";
		public const string VATTaxes = "VAT Taxes";
		public const string MiscDestinationCharges = "Misc. Destination Charges";
		public const string Other = "Other";
		#endregion

		#region PPV Allocation Mode
		public const string PPVInventory = "Inventory Account";
		public const string PPVAccount = "Purchase Price Variance Account";
		#endregion

		#region Headings
		public const string InventoryItemDescr = "Item Description";
		public const string SiteDescr = "Warehouse Description";
		public const string VendorAcctName = "Vendor Name";
		public const string CustomerAcctName = "Customer Name";
		public const string CustomerLocationID = "Customer Location";
		public const string PlanTypeDescr = "Plan Type";
		public const string VendorPrice = "Vendor Price";
		public const string VendorPriceUOM = "Vendor UOM";
		public const string CustomerPrice = "Customer Price";
		public const string CustomerPriceUOM = "Customer UOM";
		public const string POLineOrderNbr = "PO Nbr.";
		#endregion

		public const string PurchaseOrderCreated = "Purchase Order '{0}' created.";
		public const string MissingVendorOrLocation = "Vendor and vendor location should be defined.";
	}
}
