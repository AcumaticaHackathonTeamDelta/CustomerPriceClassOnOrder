<?xml version="1.0" encoding="UTF-8"?>
<sm:Screen DisplayName="Purchase Orders" Icon="system://Cart" Id="PO301000" Type="SimpleScreen" xmlns:sm="http://acumatica.com/mobilesitemap" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" ExpandSelector="Type">

    <sm:Container Name="DocumentSummary" FieldsToShow="8" ContainerActionsToExpand="1" ListActionsToExpand="2" FormActionsToExpand="0">
        <sm:Attachments Disabled="true"/>

        <sm:Field Name="Type" ListPriority="99" FormPriority="199" ForceIsDisabled="true"/>
        <sm:Field Name="OrderNbr" ListPriority="98" FormPriority="198" ForceIsDisabled="true"/>
        <sm:Field Name="Hold" FormPriority="197"/>
        <sm:Field Name="Status" ListPriority="97" FormPriority="196"/>
        <sm:Field Name="Approved" FormPriority="195"/>
        <sm:Field Name="Date" ListPriority="96" FormPriority="194"/>
        <sm:Field Name="PromisedOn" FormPriority="193"/>
        <sm:Field Name="Description" FormPriority="192"/>
        <sm:Field Name="Vendor" ListPriority="95" FormPriority="191"/>
        <sm:Field Name="Location" ListPriority="94" FormPriority="190"/>
        <sm:Field Name="Owner" FormPriority="189"/>
        <sm:Field Name="Currency" ListPriority="93" FormPriority="188"/>
        <sm:Field Name="VendorRef" FormPriority="187"/>
        <sm:Field Name="LineTotal" FormPriority="186"/>
        <sm:Field Name="DiscountTotal" FormPriority="185"/>
        <sm:Field Name="VATExemptTotal" FormPriority="184"/>
        <sm:Field Name="VATTaxableTotal" FormPriority="183"/>
        <sm:Field Name="TaxTotal" FormPriority="182"/>
        <sm:Field Name="OrderTotal" ListPriority="92" FormPriority="181"/>
        <sm:Field Name="ControlTotal" FormPriority="180"/>

        <sm:ContainerLink Container="DocumentDetails" Control="ListItem" Priority="99"/>
        <sm:ContainerLink Container="ApprovalDetails" Control="ListItem" Priority="98"/>
        <sm:ContainerLink Container="TaxDetails" Control="ListItem" Priority="97"/>

        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Shipping Instructions" FormPriority="96">
            <sm:Field Container="ShippingInstructionsShipToShippingDestinationType" Name="ShippingDestinationType"/>
            <sm:Field Container="ShippingInstructionsShipToShippingDestinationType" Name="ShipTo"/>
            <sm:Field Container="ShippingInstructionsShipToShippingDestinationType" Name="ShippingLocation"/>
            <sm:Field Container="ShippingInstructionsShipToOverride" Name="Override"/>
            <sm:Field Container="ShippingInstructionsShipToOverride" Name="Attention"/>
            <sm:Field Container="ShippingInstructionsShipToOverride" Name="BusinessName"/>
            <sm:Field Container="ShippingInstructionsShipToOverride" Name="Phone1"/>
            <sm:Field Container="ShippingInstructionsShipToOverride" Name="Email"/>
            <sm:Field Container="ShippingInstructionsShipTo" Name="Override"/>
            <sm:Field Container="ShippingInstructionsShipTo" Name="AddressLine1"/>
            <sm:Field Container="ShippingInstructionsShipTo" Name="AddressLine2"/>
            <sm:Field Container="ShippingInstructionsShipTo" Name="City"/>
            <sm:Field Container="ShippingInstructionsShipTo" Name="Country"/>
            <sm:Field Container="ShippingInstructionsShipTo" Name="State"/>
            <sm:Field Container="ShippingInstructionsShipTo" Name="PostalCode"/>
            <sm:Field Container="ShippingInstructionsShipVia" Name="FOBPoint" PickerType="Attached"/>
            <sm:Field Container="ShippingInstructionsShipVia" Name="ShipVia" PickerType="Attached"/>
        </sm:Group>

        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Vendor Info" FormPriority="95">
            <sm:Field Container="VendorInfoVendorAddressOverride" Name="Override"/>
            <sm:Field Container="VendorInfoVendorAddressOverride" Name="BusinessName"/>
            <sm:Field Container="VendorInfoVendorAddressOverride" Name="Attention"/>
            <sm:Field Container="VendorInfoVendorAddressOverride" Name="Phone1"/>
            <sm:Field Container="VendorInfoVendorAddressOverride" Name="Email"/>
            <sm:Field Container="VendorInfoVendorAddress" Name="Override"/>
            <sm:Field Container="VendorInfoVendorAddress" Name="AddressLine1"/>
            <sm:Field Container="VendorInfoVendorAddress" Name="AddressLine2"/>
            <sm:Field Container="VendorInfoVendorAddress" Name="City"/>
            <sm:Field Container="VendorInfoVendorAddress" Name="Country"/>
            <sm:Field Container="VendorInfoVendorAddress" Name="State"/>
            <sm:Field Container="VendorInfoVendorAddress" Name="PostalCode"/>
            <sm:Field Container="VendorInfoInfo" Name="Terms"/>
            <sm:Field Container="VendorInfoInfo" Name="VendorTaxZone"/>
        </sm:Group>

        <sm:ContainerLink Container="DiscountDetails" Control="ListItem" Priority="94"/>
        <sm:ContainerLink Container="Receipts" Control="ListItem" Priority="93"/>

        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Other Information" FormPriority="92">
            <sm:Field Container="OtherInformation" Name="Branch"/>
            <sm:Field Container="OtherInformation" Name="SalesOrderType"/>
            <sm:Field Container="OtherInformation" Name="SalesOrderNbr"/>
            <sm:Field Container="OtherInformation" Name="RequisitionRefNbr"/>
            <sm:Field Container="OtherInformation" Name="PrepaymentRefNbr"/>
            <sm:Field Container="OtherInformation" Name="WorkgroupID"/>
            <sm:Field Container="OtherInformation" Name="OpenQuantity"/>
            <sm:Field Container="OtherInformation" Name="OpenBalance"/>
        </sm:Group>

        <sm:Action Behavior="Create" Context="Container" Name="Insert" Icon="system://Plus"/>

        <sm:Action Behavior="Delete" Context="Selection" Icon="system://Trash" Name="Delete" DisplayName="Delete" SyncLongOperation="true"/>

        <sm:Action Behavior="Save" Context="Record" Name="Save"/>
        <sm:Action Behavior="Cancel" Context="Record" Name="Cancel"/>
        
        <sm:Action Behavior="Record" Context="Record" Name="ApproveAction" SyncLongOperation="true"/>
        <sm:Action Behavior="Record" Context="Record" Name="RejectAction" SyncLongOperation="true"/>
    </sm:Container>

    <sm:Container Name="DocumentDetails" FieldsToShow="9" ContainerActionsToExpand="1" ListActionsToExpand="0">
        <sm:Attachments Disabled="true"/>

        <sm:Field Name="Branch" ListPriority="99"/>
        <sm:Field Name="InventoryID" ListPriority="98">
            <sm:SelectorContainer FieldsToShow="2">
                <sm:Field ForceType="String" Name="InventoryID"/>
                <sm:Field ForceType="String" Name="Description"/>
                <sm:Field ForceType="String" Name="ItemClass"/>
                <sm:Field ForceType="String" Name="ItemStatus"/>
            </sm:SelectorContainer>
        </sm:Field>
        <sm:Field Name="AlternateID"/>
        <sm:Field Name="Subitem" ListPriority="97"/>
        <sm:Field Name="LineType"/>
        <sm:Field Name="Warehouse" ListPriority="95"/>
        <sm:Field Name="LineDescription"/>
        <sm:Field Name="UOM" ListPriority="93"/>
        <sm:Field Name="OrderQty" ListPriority="94"/>
        <sm:Field Name="QtyOnReceipts"/>
        <sm:Field Name="UnitCost"/>
        <sm:Field Name="ExtCost" ListPriority="92"/>
        <sm:Field Name="DiscountPercent"/>
        <sm:Field Name="DiscountAmount"/>
        <sm:Field Name="DiscountCode"/>
        <sm:Field Name="Amount"/>
        <sm:Field Name="ReceivedAmt"/>
        <sm:Field Name="MinReceipt"/>
        <sm:Field Name="MaxReceipt"/>
        <sm:Field Name="CompleteOn"/>
        <sm:Field Name="ReceiptAction"/>
        <sm:Field Name="TaxCategory"/>
        <sm:Field Name="Account"/>
        <sm:Field Name="Description"/>
        <sm:Field Name="Sub"/>
        <sm:Field Name="Project"/>
        <sm:Field Name="ProjectTask"/>
        <sm:Field Name="Requested"/>
        <sm:Field Name="Promised" ListPriority="96"/>
        <sm:Field Name="Completed" ListPriority="91"/>
        <sm:Field Name="Cancelled"/>

        <sm:Action Behavior="Create" Context="Container" Name="Insert" Icon="system://Plus"/>
        <sm:Action Behavior="Delete" Context="Selection" Name="Delete" Icon="system://Trash"/>
    </sm:Container>

    <sm:Container Name="ApprovalDetails" FieldsToShow="4">
        <sm:Attachments Disabled="true"/>

        <sm:Field Name="Approver" ListPriority="99"/>
        <sm:Field Name="ApproverName" ListPriority="98"/>
        <sm:Field Name="ApprovedBy"/>
        <sm:Field Name="ApprovedByName"/>
        <sm:Field Name="Date" ListPriority="97"/>
        <sm:Field Name="Status" ListPriority="96"/>
        <sm:Field Name="Workgroup"/>
    </sm:Container>

    <sm:Container Name="TaxDetails" FieldsToShow="4">
        <sm:Attachments Disabled="true"/>

        <sm:Field Name="TaxID"/>
        <sm:Field Name="TaxRate"/>
        <sm:Field Name="TaxableAmount"/>
        <sm:Field Name="TaxAmount"/>

        <sm:Action Behavior="Create" Context="Container" Name="Insert" Icon="system://Plus"/>
        <sm:Action Behavior="Delete" Context="Selection" Name="Delete" Icon="system://Trash"/>
    </sm:Container>

    <sm:Container Name="DiscountDetails" FieldsToShow="5">
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

    <sm:Container Name="Receipts" FieldsToShow="5">
        <sm:Attachments Disabled="true"/>

        <sm:Field Name="Type"/>
        <sm:Field Name="ReceiptNbr"/>
        <sm:Field Name="Status"/>
        <sm:Field Name="Date" ForceIsDisabled="true"/>
        <sm:Field Name="TotalQty" ForceIsDisabled="true"/>
    </sm:Container>

</sm:Screen>
