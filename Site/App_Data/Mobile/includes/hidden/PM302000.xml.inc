<?xml version="1.0" encoding="UTF-8"?>
<sm:Screen Id="PM302000" Type="SimpleScreen" DisplayName="Project Tasks" Visible="false" Icon="system://Network" xmlns:sm="http://acumatica.com/mobilesitemap" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    
	<sm:Container Name="TaskSummary" FieldsToShow="5" ContainerActionsToExpand="1" ListActionsToExpand="0" FormActionsToExpand="0">
        <sm:Attachments Disabled="true"/>
        
		<sm:Field Name="ProjectID" FormPriority="299" ListPriority="99" SelectorDisplayFormat="Key"/>
		<sm:Field Name="TaskID" ForceType="String" FormPriority="298" ListPriority="97"/>
		<sm:Field Name="Customer" FormPriority="297" ListPriority="98" SelectorDisplayFormat="Key"/>
		<sm:Field Name="Location" FormPriority="296"/>
		<sm:Field Name="RateTable" FormPriority="295"/>
		<sm:Field Name="AllocationRule" FormPriority="294"/>
		<sm:Field Name="BillingRule" FormPriority="293"/>
		<sm:Field Name="Status" FormPriority="292" ListPriority="94"/>
		<sm:Field Name="Description" FormPriority="291" ListPriority="96"/>
		
        <sm:Group Collapsable="true" Collapsed="true" DisplayName="General Settings" FormPriority="199">
            <sm:Field Container="GeneralSettingsGeneralSettings" Name="PlannedStart"/>
            <sm:Field Container="GeneralSettingsGeneralSettings" Name="PlannedEnd"/>
            <sm:Field Container="GeneralSettingsGeneralSettings" Name="StartDate" ListPriority="95"/>
            <sm:Field Container="GeneralSettingsGeneralSettings" Name="EndDate"/>
            <sm:Field Container="GeneralSettingsGeneralSettings" Name="CompleteCalculation"/>
            <sm:Field Container="GeneralSettingsGeneralSettings" Name="Completed"/>
            <sm:Field Container="GeneralSettingsGeneralSettings" Name="DefaultAccount" SelectorDisplayFormat="Key"/>
            <sm:Field Container="GeneralSettingsGeneralSettings" Name="DefaultSubaccount"/>
            <sm:Field Container="GeneralSettingsGeneralSettings" Name="DefaultAccrualAccount" SelectorDisplayFormat="Key"/>
            <sm:Field Container="GeneralSettingsGeneralSettings" Name="DefaultAccrualSub"/>
            <sm:Field Container="GeneralSettingsGeneralSettings" Name="BillingOption"/>
            <sm:Field Container="GeneralSettingsGeneralSettings" Name="Approver"/>
        </sm:Group>
        
        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Visibility Settings" FormPriority="198">
            <sm:Field Container="GeneralSettingsVisibilitySettings" Name="GL"/>
            <sm:Field Container="GeneralSettingsVisibilitySettings" Name="AP"/>
            <sm:Field Container="GeneralSettingsVisibilitySettings" Name="AR"/>
            <sm:Field Container="GeneralSettingsVisibilitySettings" Name="SO"/>
            <sm:Field Container="GeneralSettingsVisibilitySettings" Name="PO"/>
            <sm:Field Container="GeneralSettingsVisibilitySettings" Name="TE"/>
            <sm:Field Container="GeneralSettingsVisibilitySettings" Name="IN"/>
            <sm:Field Container="GeneralSettingsVisibilitySettings" Name="CA"/>
            <sm:Field Container="GeneralSettingsVisibilitySettings" Name="CR"/>
        </sm:Group>
        
        <sm:ContainerLink Container="Budget" Control="ListItem" Priority="99"/>
        <sm:ContainerLink Container="Attributes" Control="ListItem" Priority="98"/>
        
        <sm:Action Behavior="Create" Context="Container" Name="Insert" Icon="system://Plus"/>
        <sm:Action Behavior="Delete" Context="Selection" Name="Delete" Icon="system://Trash"/>
        
        <sm:Action Behavior="Save" Context="Record" Name="Save" After="Close"/>
        <sm:Action Behavior="Cancel" Context="Record" Name="Cancel"/>
	</sm:Container>
    
	<sm:Container Name="Attributes" Attributes="true"/>
    
	<sm:Container Name="Budget" FieldsToShow="8" ContainerActionsToExpand="1" ListActionsToExpand="0">
        <sm:Attachments Disabled="true"/>
        
		<sm:Field Name="Type" ListPriority="99"/>
		<sm:Field Name="AccountGroup" ListPriority="98" SelectorDisplayFormat="Key"/>
		<sm:Field Name="InventoryID" ListPriority="97"/>
		<sm:Field Name="Description" ListPriority="96"/>
		<sm:Field Name="BudgetedQty"/>
		<sm:Field Name="UOM"/>
		<sm:Field Name="Rate"/>
		<sm:Field Name="BudgetedAmount" ListPriority="95"/>
		<sm:Field Name="RevisedQty"/>
		<sm:Field Name="RevisedAmount" ListPriority="94"/>
		<sm:Field Name="ActualQty"/>
		<sm:Field Name="ActualAmount" ListPriority="93"/>
		<sm:Field Name="Performance" ForceIsVisible="false" ListPriority="92"/>
		
        <sm:Action Behavior="Create" Context="Container" Name="Insert" Icon="system://Plus"/>
        <sm:Action Behavior="Delete" Context="Selection" Name="Delete" Icon="system://Trash"/>
	</sm:Container>
    
</sm:Screen>
