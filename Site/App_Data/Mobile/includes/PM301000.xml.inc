<?xml version="1.0" encoding="UTF-8"?>
<sm:Screen Id="PM301000" Type="SimpleScreen" DisplayName="Projects" Icon="system://Network" xmlns:sm="http://acumatica.com/mobilesitemap" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    
    <sm:Container Name="ProjectSummary" FieldsToShow="5" ContainerActionsToExpand="1" ListActionsToExpand="0" FormActionsToExpand="0" UseOptimizedExport="true">
        <sm:Attachments Disabled="true"/>
        <sm:Field Name="ProjectID" ForceType="String" FormPriority="299" ListPriority="99"/>
        <sm:Field Name="TemplateID" FormPriority="297"/>
        <sm:Field Name="Description" FormPriority="296" ListPriority="97"/>
        <sm:Field Name="Hold" FormPriority="295"/>
        <sm:Field Name="Status" FormPriority="294" ListPriority="96"/>
      
        <sm:Field Name="Customer" FormPriority="298" ListPriority="-100" SelectorDisplayFormat="Key"/>  

      <sm:Group Collapsable="true" Collapsed="true" DisplayName="Project Summary" FormPriority="199">
            <sm:Field Name="Assets"/>
            <sm:Field Name="Liabilities"/>
            <sm:Field Name="Income"/>
            <sm:Field Name="Expenses"/>
        </sm:Group>
        
        <sm:Group Collapsable="true" Collapsed="true" DisplayName="General Settings" FormPriority="198">
            <sm:Field Container="GeneralInfoGeneralSettings" Name="StartDate" ListPriority="95"/>
            <sm:Field Container="GeneralInfoGeneralSettings" Name="EndDate" ListPriority="94"/>
            <sm:Field Container="GeneralInfoGeneralSettings" Name="ProjectManager"/>
            <sm:Field Container="GeneralInfoGeneralSettings" Name="RestrictEmployees"/>
            <sm:Field Container="GeneralInfoGeneralSettings" Name="RestrictEquipment"/>
        </sm:Group>
        
        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Visibility Settings" FormPriority="197">
            <sm:Field Container="GeneralInfoVisibilitySettings" Name="GL"/>
            <sm:Field Container="GeneralInfoVisibilitySettings" Name="AP"/>
            <sm:Field Container="GeneralInfoVisibilitySettings" Name="AR"/>
            <sm:Field Container="GeneralInfoVisibilitySettings" Name="SO"/>
            <sm:Field Container="GeneralInfoVisibilitySettings" Name="PO"/>
            <sm:Field Container="GeneralInfoVisibilitySettings" Name="TE"/>
            <sm:Field Container="GeneralInfoVisibilitySettings" Name="IN"/>
            <sm:Field Container="GeneralInfoVisibilitySettings" Name="CA"/>
            <sm:Field Container="GeneralInfoVisibilitySettings" Name="CR"/>
        </sm:Group>
        
        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Defaults" FormPriority="196">
            <sm:Field Container="GeneralInfoDefaults" Name="DefaultAccount" SelectorDisplayFormat="Key"/>
            <sm:Field Container="GeneralInfoDefaults" Name="DefaultSubaccount" />
            <sm:Field Container="GeneralInfoDefaults" Name="DefaultAccrualAccount" SelectorDisplayFormat="Key"/>
            <sm:Field Container="GeneralInfoDefaults" Name="DefaultAccrualSub" />
        </sm:Group>
        
        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Billing Settings" FormPriority="195">
            <sm:Field Container="GeneralInfoBillingSettingsBillingPeriod" Name="BillingPeriod"/>
            <sm:Field Container="GeneralInfoBillingSettingsBillingPeriod" Name="NextBillingDate"/>
            <sm:Field Container="GeneralInfoBillingSettingsBillingPeriod" Name="LastBillingDate"/>
            <sm:Field Container="GeneralInfoBillingSettings" Name="CustomerLocation"/>
            <sm:Field Container="GeneralInfoBillingSettings" Name="AllocationRule"/>
            <sm:Field Container="GeneralInfoBillingSettings" Name="BillingRule"/>
            <sm:Field Container="GeneralInfoBillingSettings" Name="RateTable"/>
            <sm:Field Container="GeneralInfoBillingSettings" Name="RunAllocationOnPMTransactionRelease"/>
            <sm:Field Container="GeneralInfoBillingSettings" Name="AutomaticallyReleaseARDocuments"/>
        </sm:Group>
        
        <sm:ContainerLink Container="Tasks" Control="ListItem" Priority="99"/>
        <sm:ContainerLink Container="Balances" Control="ListItem" Priority="98"/>
        <sm:ContainerLink Container="Employees" Control="ListItem" Priority="97"/>
        <sm:ContainerLink Container="Equipment" Control="ListItem" Priority="96"/>
        <sm:ContainerLink Container="Attributes" Control="ListItem" Priority="95"/>
        <sm:ContainerLink Container="Invoices" Control="ListItem" Priority="94"/>
        
        <sm:Action Behavior="Create" Context="Container" Name="Insert" Icon="system://Plus"/>
        <sm:Action Behavior="Delete" Context="Selection" Name="Delete" Icon="system://Trash"/>
        
        <sm:Action Behavior="Save" Context="Record" Name="Save"/>
        <sm:Action Behavior="Cancel" Context="Record" Name="Cancel"/>

        <sm:Action Behavior="Record" Context="Record" Name="ApproveAction"/>
        <sm:Action Behavior="Record" Context="Record" Name="RejectAction"/>
    </sm:Container>
    
	<sm:Container Name="Attributes" Attributes="true"/>
    
	<sm:Container Name="Tasks" FieldsToShow="6" ContainerActionsToExpand="1" ListActionsToExpand="1" FormActionsToExpand="1">
        <sm:Attachments Disabled="true"/>
        
		<sm:Field Name="TaskID" ForceType="String"/>
		<sm:Field Name="Description"/>
		<sm:Field Name="StartDate"/>
		<sm:Field Name="EndDate"/>
		<sm:Field Name="AllocationRule"/>
		<sm:Field Name="Status"/>

    <sm:Field Name="Location"/>
    <sm:Field Name="RateTable"/>
    <sm:Field Name="BillingRule"/>
    <sm:Field Name="Completed"/>
    <sm:Field Name="PlannedStart"/>
    <sm:Field Name="PlannedEnd"/>
    <sm:Field Name="Approver"/>

    <sm:Field Name="BillingOption"/>
    <sm:Field Name="DefaultAccount"/>
    <sm:Field Name="DefaultSubaccount"/>
    <sm:Field Name="Asset"/>
    <sm:Field Name="Income"/>
    <sm:Field Name="Expense"/>

    <sm:Action Behavior="Create" Context="Container" Name="Insert" Icon="system://Plus"/>
    <sm:Action Behavior="Delete" Context="Selection" Name="Delete" Icon="system://Trash"/>
		
    <sm:Action Behavior="Record" Context="Record" Name="ViewTask" Redirect="true" DisplayName="View Detailed"/>
	</sm:Container>
    
	<sm:Container Name="Balances" FieldsToShow="6" ContainerActionsToExpand="1" ListActionsToExpand="0">
        <sm:Attachments Disabled="true"/>
        
		<sm:Field Name="AccountGroup"/>
		<sm:Field Name="Description"/>
		<sm:Field Name="CurrentBudgetedAmount"/>
		<sm:Field Name="ActualAmount"/>
		<sm:Field Name="OriginalBudgetedAmount"/>
		<sm:Field Name="Performance"/>
	</sm:Container>
    
	<sm:Container Name="Employees" FieldsToShow="3" ContainerActionsToExpand="1" ListActionsToExpand="0">
        <sm:Attachments Disabled="true"/>
        
		<sm:Field Name="EmployeeID" SelectorDisplayFormat="Key"/>
		<sm:Field Name="EmployeeName"/>
		<sm:Field Name="Department"/>
		
        <sm:Action Behavior="Create" Context="Container" Name="Insert" Icon="system://Plus"/>
        <sm:Action Behavior="Delete" Context="Selection" Name="Delete" Icon="system://Trash"/>
	</sm:Container>
    
	<sm:Container Name="Equipment" FieldsToShow="5" ContainerActionsToExpand="1" ListActionsToExpand="0">
        <sm:Attachments Disabled="true"/>
        
		<sm:Field Name="EquipmentID" ListPriority="99" SelectorDisplayFormat="Key"/>
		<sm:Field Name="Description" ListPriority="98"/>
		<sm:Field Name="Active" ListPriority="95"/>
		<sm:Field Name="RunRateItem" ListPriority="97"/>
		<sm:Field Name="RunRate" ListPriority="96"/>
		<sm:Field Name="SetupRateItem"/>
		<sm:Field Name="SetupRate"/>
		<sm:Field Name="SuspendRateItem"/>
		<sm:Field Name="SuspendRate"/>
		
        <sm:Action Behavior="Create" Context="Container" Name="Insert" Icon="system://Plus"/>
        <sm:Action Behavior="Delete" Context="Selection" Name="Delete" Icon="system://Trash"/>
	</sm:Container>
    
	<sm:Container Name="Invoices" FieldsToShow="6" ContainerActionsToExpand="1" ListActionsToExpand="0">
        <sm:Attachments Disabled="true"/>
        
		<sm:Field Name="Type" ListPriority="99"/>
		<sm:Field Name="ReferenceNbr" ListPriority="98"/>
		<sm:Field Name="Description" ListPriority="97"/>
		<sm:Field Name="Date" ListPriority="95"/>
		<sm:Field Name="PostPeriod"/>
		<sm:Field Name="Amount" ListPriority="96"/>
		<sm:Field Name="Balance"/>
		<sm:Field Name="Status" ListPriority="94"/>
		<sm:Field Name="CashDiscount"/>
		<sm:Field Name="CashDiscountBalance"/>
		<sm:Field Name="TaxTotal"/>
		<sm:Field Name="Printed"/>
	</sm:Container>
    
</sm:Screen>
