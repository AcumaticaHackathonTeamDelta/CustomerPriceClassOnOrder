<?xml version="1.0" encoding="UTF-8"?>
<sm:Screen DisplayName="Requests" Id="RQ301000" Type="SimpleScreen" Icon="system://Note" xmlns:sm="http://acumatica.com/mobilesitemap" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    
	<sm:Container Name="DocumentSummary" FieldsToShow="7" ContainerActionsToExpand="1" ListActionsToExpand="2">
        <sm:Attachments Disabled="true"/>
        
		<sm:Field Name="RefNbr" ListPriority="99" FormPriority="199"/>
		<sm:Field Name="RequestClass" FormPriority="198"/>
		<sm:Field Name="Status" ListPriority="95" FormPriority="197"/>
		<sm:Field Name="Hold" FormPriority="196"/>
		<sm:Field Name="Date" ListPriority="97" FormPriority="195"/>
		<sm:Field Name="Approved" FormPriority="194"/>
		<sm:Field Name="Priority" ListPriority="96" FormPriority="193"/>
		<sm:Field Name="RequestedBy" ListPriority="98" FormPriority="192"/>
		<sm:Field Name="Location" FormPriority="191"/>
		<sm:Field Name="Department" FormPriority="190"/>
		<sm:Field Name="Currency" ListPriority="94" FormPriority="189"/>
		<sm:Field Name="Description" FormPriority="188"/>
		<sm:Field Name="EstExtCost" ListPriority="93" FormPriority="187"/>
		<sm:Field Name="OpenQty" FormPriority="186"/>
		
        <sm:ContainerLink Container="DocumentDetails" Control="ListItem" Priority="99"/>
        <sm:ContainerLink Container="ApprovalDetails" Control="ListItem" Priority="98"/>
        
        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Shipping Instructions" FormPriority="97">
            <sm:Field Container="ShippingInstructionsShipToShippingDestinationType" Name="ShippingDestinationType"/>
            <sm:Field Container="ShippingInstructionsShipToShippingDestinationType" Name="ShipTo"/>
            <sm:Field Container="ShippingInstructionsShipToShippingDestinationType" Name="Location"/>
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
        </sm:Group>
		
        <sm:ContainerLink Container="BudgetDetailsDetails" Control="ListItem" Priority="96"/>
        
        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Other Information" FormPriority="95">
            <sm:Field Container="OtherInformation" Name="Workgroup"/>
            <sm:Field Container="OtherInformation" Name="Owner"/>
        </sm:Group>
        
        
        <sm:Action Behavior="Create" Context="Container" Name="Insert" Icon="system://Plus"/>
        
        <sm:Action Behavior="Void" Context="Selection" Name="ApproveAction" After="Refresh" SyncLongOperation="true"/>
        <sm:Action Behavior="Void" Context="Selection" Name="RejectAction" After="Refresh" SyncLongOperation="true"/>
        <sm:Action Behavior="Delete" Context="Selection" Icon="system://Trash" Name="Delete" DisplayName="Delete" SyncLongOperation="true"/>
        
        <sm:Action Behavior="Save" Context="Record" Name="Save"/>
        <sm:Action Behavior="Cancel" Context="Record" Name="Cancel"/>
	</sm:Container>
    
    <sm:Container Name="DocumentDetails" FieldsToShow="7" ContainerActionsToExpand="1" ListActionsToExpand="0">
        <sm:Attachments Disabled="true"/>
        
		<sm:Field Name="Inventory" ListPriority="99"/>
		<sm:Field Name="AlternateID"/>
		<sm:Field Name="Subitem" ListPriority="98"/>
		<sm:Field Name="Description" ListPriority="97"/>
		<sm:Field Name="UOM" ListPriority="95"/>
		<sm:Field Name="OrderQty" ListPriority="96"/>
		<sm:Field Name="EstUnitCost"/>
		<sm:Field Name="EstExtCost" ListPriority="94"/>
		<sm:Field Name="RequiredDate"/>
		<sm:Field Name="PromisedDate" ListPriority="93"/>
		<sm:Field Name="IssueStatus"/>
		<sm:Field Name="Canceled"/>
		<sm:Field Name="Location"/>
		<sm:Field Name="Vendor"/>
		<sm:Field Name="VendorDescription"/>
		<sm:Field Name="VendorRef"/>
		<sm:Field Name="VendorName"/>
		<sm:Field Name="Account"/>
		<sm:Field Name="Sub"/>
        
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
		<sm:Field Name="Workgroup" ForceIsDisabled="True"/>
	</sm:Container>
    
    <sm:Container Name="BudgetDetailsDetails" FieldsToShow="4" DisplayName="Budget Details">
        <sm:Attachments Disabled="true"/>
        
		<sm:Field Name="Account" ListPriority="99"/>
		<sm:Field Name="Sub" ListPriority="98"/>
		<sm:Field Name="Currency"/>
		<sm:Field Name="DocumentAmount"/>
		<sm:Field Name="RequestAmount" ListPriority="97"/>
		<sm:Field Name="BudgetAmount" ListPriority="96"/>
		<sm:Field Name="AmountSpent"/>
		<sm:Field Name="ApprovedAmount"/>
		<sm:Field Name="UnapprovedAmount"/>
	</sm:Container>
	
</sm:Screen>
