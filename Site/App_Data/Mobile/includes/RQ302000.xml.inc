<?xml version="1.0" encoding="UTF-8"?>
<sm:Screen DisplayName="Requisitions" Id="RQ302000" Type="SimpleScreen" Icon="system://Portfolio" xmlns:sm="http://acumatica.com/mobilesitemap" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    
	<sm:Container Name="DocumentSummary" FieldsToShow="8" ContainerActionsToExpand="1" ListActionsToExpand="2">
        <sm:Attachments Disabled="true"/>
        
		<sm:Field Name="RefNbr" ListPriority="99" FormPriority="199"/>
		<sm:Field Name="Status" ListPriority="98" FormPriority="198"/>
		<sm:Field Name="Hold" FormPriority="197"/>
		<sm:Field Name="Date" ListPriority="97" FormPriority="196"/>
		<sm:Field Name="Approved" FormPriority="195"/>
		<sm:Field Name="Description" FormPriority="194"/>
		<sm:Field Name="Priority" ListPriority="96" FormPriority="193"/>
		<sm:Field Name="Creator" FormPriority="192"/>
		<sm:Field Name="Customer" ListPriority="95" FormPriority="191"/>
		<sm:Field Name="Location" ListPriority="94" FormPriority="190"/>
		<sm:Field Name="Currency" ListPriority="93" FormPriority="189"/>
		<sm:Field Name="EstExtCost" ListPriority="92" FormPriority="188"/>
		
        <sm:ContainerLink Container="DocumentDetails" Control="ListItem" Priority="99"/>
        <sm:ContainerLink Container="ApprovalDetails" Control="ListItem" Priority="98"/>
        
        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Shipping Instructions" FormPriority="97">
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
            <sm:Field Container="ShippingInstructionsShipVia" Name="FOBPoint"/>
            <sm:Field Container="ShippingInstructionsShipVia" Name="ShipVia"/>
        </sm:Group>
        
        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Bidding" FormPriority="96">
            <sm:Field Container="Bidding" Name="Vendor"/>
            <sm:Field Container="Bidding" Name="Location"/>
            <sm:Field Container="Bidding" Name="VendorRef"/>
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
            <sm:Field Container="VendorInfoInfo" Name="POType"/>
            <sm:Field Container="VendorInfoInfo" Name="Splittable"/>
        </sm:Group>
        
        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Other Information" FormPriority="94">
            <sm:Field Container="OtherInformation" Name="Workgroup"/>
            <sm:Field Container="OtherInformation" Name="Owner"/>
        </sm:Group>
		
        <sm:ContainerLink Container="PurchaseOrders" Control="ListItem" Priority="93"/>
        <sm:ContainerLink Container="SalesOrders" Control="ListItem" Priority="92"/>
        
        
        <sm:Action Behavior="Create" Context="Container" Name="Insert" Icon="system://Plus"/>
        
        <sm:Action Behavior="Void" Context="Selection" Name="ApproveAction" After="Refresh" SyncLongOperation="true"/>
        <sm:Action Behavior="Void" Context="Selection" Name="RejectAction" After="Refresh" SyncLongOperation="true"/>
        <sm:Action Behavior="Delete" Context="Selection" Icon="system://Trash" Name="Delete" DisplayName="Delete" SyncLongOperation="true"/>
        
        <sm:Action Behavior="Save" Context="Record" Name="Save"/>
        <sm:Action Behavior="Cancel" Context="Record" Name="Cancel"/>
	</sm:Container>
	
	<sm:Container Name="DocumentDetails" FieldsToShow="7" ContainerActionsToExpand="1" ListActionsToExpand="0">
        <sm:Attachments Disabled="true"/>
        
		<sm:Field Name="InventoryID" ListPriority="99"/>
		<sm:Field Name="AlternateID"/>
		<sm:Field Name="Subitem" ListPriority="98"/>
		<sm:Field Name="LineSource"/>
		<sm:Field Name="LineType"/>
		<sm:Field Name="Warehouse"/>
		<sm:Field Name="Description" ListPriority="97"/>
		<sm:Field Name="UOM" ListPriority="95"/>
		<sm:Field Name="OrderQty" ListPriority="96"/>
		<sm:Field Name="EstUnitCost"/>
		<sm:Field Name="EstExtCost" ListPriority="94"/>
		<sm:Field Name="Account"/>
		<sm:Field Name="Sub"/>
		<sm:Field Name="UseMarkup"/>
		<sm:Field Name="Markup"/>
		<sm:Field Name="MinReceipt"/>
		<sm:Field Name="MaxReceipt"/>
		<sm:Field Name="CompleteOn"/>
		<sm:Field Name="ReceiptAction"/>
		<sm:Field Name="RequiredDate"/>
		<sm:Field Name="PromisedDate" ListPriority="93"/>
		<sm:Field Name="Canceled"/>
		<sm:Field Name="LineNbr"/>
        
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
		<sm:Field Name="Workgroup" ForceIsDisabled="true"/>
	</sm:Container>
	
	<sm:Container Name="PurchaseOrders" FieldsToShow="6">
        <sm:Attachments Disabled="true"/>
        
		<sm:Field Name="TypePOOrderOrderType" DisplayName="Type"/>
		<sm:Field Name="OrderNbr" ListPriority="99"/>
		<sm:Field Name="Status" ListPriority="98"/>
		<sm:Field Name="Date"/>
		<sm:Field Name="Vendor" ListPriority="97"/>
		<sm:Field Name="Location" ListPriority="96"/>
		<sm:Field Name="VendorRef"/>
		<sm:Field Name="Currency" ListPriority="95"/>
		<sm:Field Name="LineTotal"/>
		<sm:Field Name="TaxTotal"/>
		<sm:Field Name="OrderTotal" ListPriority="94"/>
		<sm:Field Name="Owner"/>
	</sm:Container>
    
	<sm:Container Name="SalesOrders" FieldsToShow="6">
        <sm:Attachments Disabled="true"/>
        
		<sm:Field Name="OrderType"/>
		<sm:Field Name="OrderNbr" ListPriority="99"/>
		<sm:Field Name="Date"/>
		<sm:Field Name="Status" ListPriority="98"/>
		<sm:Field Name="Customer" ListPriority="97"/>
		<sm:Field Name="Location" ListPriority="96"/>
		<sm:Field Name="Currency" ListPriority="95"/>
		<sm:Field Name="LineTotal"/>
		<sm:Field Name="TaxTotal"/>
		<sm:Field Name="OrderTotal" ListPriority="94"/>
	</sm:Container>
	
</sm:Screen>
