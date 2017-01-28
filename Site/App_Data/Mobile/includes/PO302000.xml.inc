<?xml version="1.0" encoding="UTF-8"?>
<sm:Screen DisplayName="Purchase Receipts" Id="PO302000" Type="SimpleScreen" ExpandSelector="Type" Icon="system://Box1" xmlns:sm="http://acumatica.com/mobilesitemap" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">

    <sm:Container Name="DocumentSummary" FieldsToShow="9" ContainerActionsToExpand="1" ListActionsToExpand="1" FormActionsToExpand="1">
        <sm:Attachments Disabled="true"/>
        <sm:Field Name="Vendor" FormPriority="193" ListPriority="99"/>      
        <sm:Field Name="ReceiptNbr" FormPriority="198" ListPriority="98" ForceIsDisabled="true"/>
        <sm:Field Name="Type" FormPriority="199" ListPriority="97" ForceIsDisabled="true"/>
        <sm:Field Name="Status" FormPriority="197" ListPriority="96"/>
        <sm:Field Name="Hold" FormPriority="196"/>
        <sm:Field Name="Date" FormPriority="195" ListPriority="95"/>
        
        <sm:Field Name="PostPeriod" FormPriority="194"/>
        <sm:Field Name="Warehouse" FormPriority="192"/>
        <sm:Field Name="Location" FormPriority="191" ListPriority="94"/>
        <sm:Field Name="Currency" FormPriority="190" ListPriority="93"/>
        <sm:Field Name="CreateBill" FormPriority="189"/>
        <sm:Field Name="VendorRef" FormPriority="188"/>
        <sm:Field Name="TotalQty" FormPriority="187" ListPriority="92"/>
        <sm:Field Name="ControlQty" FormPriority="186"/>
        <sm:Field Name="VATExemptTotal" FormPriority="185"/>
        <sm:Field Name="VATTaxableTotal" FormPriority="184"/>
        <sm:Field Name="DiscountTotal" FormPriority="183"/>
        <sm:Field Name="TotalAmt" FormPriority="182" ListPriority="91"/>
        <sm:Field Name="ControlAmt" FormPriority="181"/>

        <sm:ContainerLink Container="DocumentDetails" Control="ListItem" Priority="99"/>
        <sm:ContainerLink Container="TaxDetails" Control="ListItem" Priority="98"/>

        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Financial Details" FormPriority="97">
            <sm:Field Container="FinancialDetailsBillingSettings" Name="Branch"/>
            <sm:Field Container="FinancialDetailsBillingSettings" Name="Terms"/>
            <sm:Field Container="FinancialDetailsBillingSettings" Name="BillDate"/>
            <sm:Field Container="FinancialDetailsBillingSettings" Name="DueDate"/>
            <sm:Field Container="FinancialDetailsBillingSettings" Name="CashDiscountDate"/>
            <sm:Field Container="FinancialDetailsBillingSettings" Name="CashDiscount"/>
            <sm:Field Container="FinancialDetailsBillingSettings" Name="VendorTaxZone"/>
            <sm:Field Container="FinancialDetailsTotals" Name="UnbilledBalance"/>
            <sm:Field Container="FinancialDetailsTotals" Name="UnbilledQuantity"/>
            <sm:Field Container="FinancialDetailsTotals" Name="UnbilledLineTotal"/>
            <sm:Field Container="FinancialDetailsTotals" Name="UnbilledTaxTotal"/>
            <sm:Field Container="FinancialDetailsAssignTo" Name="Workgroup"/>
            <sm:Field Container="FinancialDetailsAssignTo" Name="Owner"/>
        </sm:Group>

        <sm:ContainerLink Container="LandedCosts" Control="ListItem" Priority="96"/>
        <sm:ContainerLink Container="DiscountDetails" Control="ListItem" Priority="95"/>

        <sm:Action Behavior="Create" Context="Container" Name="Insert" Icon="system://Plus"/>

        <sm:Action Behavior="Delete" Context="Selection" Icon="system://Trash" Name="Delete" DisplayName="Delete" SyncLongOperation="true"/>

        <sm:Action Behavior="Save" Context="Record" Name="Save"/>
        <sm:Action Behavior="Cancel" Context="Record" Name="Cancel"/>
        <sm:Action Behavior="Record" Context="Record" Name="AssignAction" DisplayName="Assign"/>
    </sm:Container>

    <sm:Container Name="DocumentDetails" FieldsToShow="8" ContainerActionsToExpand="1" ListActionsToExpand="0" FormActionsToExpand="1">
        <sm:Attachments Disabled="true"/>
        <sm:Field Name="InventoryID" ListPriority="99"/>
        <sm:Field Name="Branch" ListPriority="98"/>
        <sm:Field Name="Subitem" ListPriority="97"/>
        <sm:Field Name="LineType"/>
        <sm:Field Name="Warehouse" ListPriority="96"/>
        <sm:Field Name="Location" ListPriority="95"/>
        <sm:Field Name="TransactionDescr"/>
        <sm:Field Name="UOM" ListPriority="93"/>
        <sm:Field Name="OrderedQty"/>
        <sm:Field Name="OpenQty"/>
        <sm:Field Name="ReceiptQty" ListPriority="94"/>
        <sm:Field Name="UnitCost"/>
        <sm:Field Name="ExtCost" ListPriority="92"/>
        <sm:Field Name="DiscountPercent"/>
        <sm:Field Name="ManualDiscount"/>
        <sm:Field Name="DiscountAmount"/>
        <sm:Field Name="DiscountCode"/>
        <sm:Field Name="Amount"/>
        <sm:Field Name="TaxCategory"/>
        <sm:Field Name="Account"/>
        <sm:Field Name="Description"/>
        <sm:Field Name="Sub"/>
        <sm:Field Name="AccrualAccount"/>
        <sm:Field Name="AccrualSub"/>
        <sm:Field Name="Project"/>
        <sm:Field Name="ProjectTask"/>
        <sm:Field Name="ExpirationDate"/>
        <sm:Field Name="LotSerialNbr"/>
        <sm:Field Name="POOrderType"/>
        <sm:Field Name="POOrderNbr"/>
        <sm:Field Name="POLineNbr"/>
        <sm:Field Name="CompletePOLine"/>
        <sm:Field Name="ReasonCode"/>
        <sm:Field Name="TransferLineNbr"/>
        <sm:Field Name="TransferOrderNbr"/>
        <sm:Field Name="TransferOrderType"/>

        <sm:Action Behavior="Create" Context="Container" Name="Insert" Icon="system://Plus"/>
        <sm:Action Behavior="Delete" Context="Selection" Name="Delete" Icon="system://Trash"/>
        <sm:Action Behavior="Void" Context="Record" Name="LSPOReceiptLineBinLotSerial" Redirect="true" RedirectToContainer="Allocations$List"/>
    </sm:Container>

    <sm:Container Name="Allocations" FieldsToShow="6" ContainerActionsToExpand="1" ListActionsToExpand="0" Visible="false">
        <sm:Attachments Disabled="true"/>

        <sm:Field Name="Subitem"/>
        <sm:Field Name="Location"/>
        <sm:Field Name="LotSerialNbr"/>
        <sm:Field Name="Quantity"/>
        <sm:Field Name="UOM"/>
        <sm:Field Name="ExpirationDate"/>

        <sm:Action Behavior="Create" Context="Container" Name="Insert" Icon="system://Plus"/>
        <sm:Action Behavior="Delete" Context="Selection" Name="Delete" Icon="system://Trash"/>
    </sm:Container>

    <sm:Container Name="TaxDetails" FieldsToShow="4" ContainerActionsToExpand="1" ListActionsToExpand="0">
        <sm:Attachments Disabled="true"/>

        <sm:Field Name="TaxID"/>
        <sm:Field Name="TaxRate"/>
        <sm:Field Name="TaxableAmount"/>
        <sm:Field Name="TaxAmount"/>

        <sm:Action Behavior="Create" Context="Container" Name="Insert" Icon="system://Plus"/>
        <sm:Action Behavior="Delete" Context="Selection" Name="Delete" Icon="system://Trash"/>
    </sm:Container>

    <sm:Container Name="LandedCosts" FieldsToShow="5" ContainerActionsToExpand="1" ListActionsToExpand="0">
        <sm:Attachments Disabled="true"/>

        <sm:Field Name="LandedCostCode" ListPriority="99"/>
        <sm:Field Name="Description" ListPriority="98"/>
        <sm:Field Name="VendorRef"/>
        <sm:Field Name="Vendor" ListPriority="97"/>
        <sm:Field Name="Location"/>
        <sm:Field Name="APBillDate"/>
        <sm:Field Name="CurrencySimpleSelector" DisplayName="Currency" ListPriority="95"/>
        <sm:Field Name="Amount" ListPriority="96"/>
        <sm:Field Name="TaxCategory"/>
        <sm:Field Name="Terms"/>
        <sm:Field Name="InventoryID"/>
        <sm:Field Name="PostponeAPBillCreation"/>

        <sm:Action Behavior="Create" Context="Container" Name="Insert" Icon="system://Plus"/>
        <sm:Action Behavior="Delete" Context="Selection" Name="Delete" Icon="system://Trash"/>
    </sm:Container>

    <sm:Container Name="DiscountDetails" FieldsToShow="5" ContainerActionsToExpand="1" ListActionsToExpand="0">
        <sm:Attachments Disabled="true"/>

        <sm:Field Name="SkipDiscount" ListPriority="99"/>
        <sm:Field Name="DiscountCode" ListPriority="98"/>
        <sm:Field Name="SequenceID" ListPriority="97"/>
        <sm:Field Name="Type" ListPriority="96"/>
        <sm:Field Name="ManualDiscount"/>
        <sm:Field Name="DiscountableAmt"/>
        <sm:Field Name="DiscountableQty"/>
        <sm:Field Name="DiscountAmt" ListPriority="95"/>
        <sm:Field Name="Discount"/>

        <sm:Action Behavior="Create" Context="Container" Name="Insert" Icon="system://Plus"/>
        <sm:Action Behavior="Delete" Context="Selection" Name="Delete" Icon="system://Trash"/>
    </sm:Container>

</sm:Screen>
