<?xml version="1.0" encoding="UTF-8"?>
<sm:Screen DisplayName="Equipment Time Card" Icon="system://Clock" Id="EP308000" Type="SimpleScreen" Visible="false" OpenAs="Form" xmlns:sm="http://acumatica.com/mobilesitemap" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    
    <sm:Container Name="DocumentSummary" FormActionsToExpand="0">
        <sm:Attachments Disabled="true"/>
        
        <sm:Field Name="RefNbr" FormPriority="99" ForceIsDisabled="true"/>
        <sm:Field Name="Status" FormPriority="98" ForceIsDisabled="true"/>
        <sm:Field Name="Week" FormPriority="97"/>
        <sm:Field Name="EquipmentID" FormPriority="96" PickerType="Attached"/>
        <sm:Field Name="Type" FormPriority="95"/>
		<sm:Field Name="OrigRefNbr" FormPriority="94"/>
        
        <sm:ContainerLink Container="Summary" Control="ListItem" Priority="93"/>
        <sm:ContainerLink Container="Details" Control="ListItem" Priority="92"/>
        
        <sm:Group Collapsable="true" Collapsed="false" DisplayName="Time Totals" FormPriority="91">
            <sm:Field Container="DocumentSummarySetup" Name="TimeSpent" DisplayName="Setup Time"/>
            <sm:Field Container="DocumentSummarySetup" Name="Billable" DisplayName="Billable Setup"/>
            <sm:Field Container="DocumentSummaryRun" Name="Run" DisplayName="Run Time"/>
            <sm:Field Container="DocumentSummaryRun" Name="BillableRun" DisplayName="Billable Run"/>
            <sm:Field Container="DocumentSummarySuspend" Name="Suspend" DisplayName="Suspend Time"/>
            <sm:Field Container="DocumentSummarySuspend" Name="BillableSuspend" DisplayName="Billable Suspend"/>
            <sm:Field Container="DocumentSummaryTotal" Name="Total"/>
            <sm:Field Container="DocumentSummaryTotal" Name="BillableTotal"/>
        </sm:Group>
        
        <sm:Action Behavior="Save" Context="Record" Name="Save" After="Close"/>
        <sm:Action Behavior="Cancel" Context="Record" Name="Cancel"/>
        
        <sm:Action Behavior="Record" Context="Record" Name="ReleaseAction" SyncLongOperation="true"/>
        <sm:Action Behavior="Record" Context="Record" Name="ApproveAction"/>
        <sm:Action Behavior="Record" Context="Record" Name="RejectAction"/>
        <sm:Action Behavior="Record" Context="Record" Name="SubmitAction"/>
        <sm:Action Behavior="Record" Context="Record" Name="EditAction"/>
    </sm:Container>
    
    <sm:Container Name="Summary" FieldsToShow="6" ContainerActionsToExpand="1" ListActionsToExpand="1">
        <sm:Attachments Disabled="true"/>
        
        <sm:Field ListPriority="99" Name="Project">
            <sm:SelectorContainer FieldsToShow="2">
                <sm:Field ForceType="String" Name="ProjectID"/>
                <sm:Field ForceType="String" Name="Description"/>
                <sm:Field ForceType="String" Name="Status"/>
            </sm:SelectorContainer>
        </sm:Field>
        
        <sm:Field ListPriority="97" Name="ProjectTask">
            <sm:SelectorContainer FieldsToShow="2">
                <sm:Field ForceType="String" Name="Description"/>
                <sm:Field ForceType="String" Name="TaskID"/>
                <sm:Field ForceType="String" Name="Status"/>
            </sm:SelectorContainer>
        </sm:Field>
        
        <sm:Field Name="RateType" ListPriority="98"/>
        <sm:Field Name="TimeSpent" ListPriority="96"/>
        <sm:Field Name="Billable" ListPriority="94"/>
        <sm:Field Name="Description" ListPriority="95"/>
        
        <sm:Group Collapsable="false" Collapsed="false" DisplayName="Week Days">
            <sm:Field Name="Mon"/>
            <sm:Field Name="Tue"/>
            <sm:Field Name="Wed"/>
            <sm:Field Name="Thu"/>
            <sm:Field Name="Fri"/>
            <sm:Field Name="Sat"/>
            <sm:Field Name="Sun"/>
        </sm:Group>
        
        <sm:Action Behavior="Create" Context="Container" Name="Insert" Icon="system://Plus"/>
        <sm:Action Behavior="Delete" Context="Selection" Name="Delete" Icon="system://Trash"/>
        <sm:Action Behavior="Void" Context="Container" Name="PreloadFromPreviousTimecard"/>
    </sm:Container>
    
    <sm:Container Name="Details" FieldsToShow="4" ContainerActionsToExpand="1" ListActionsToExpand="1" FormActionsToExpand="1">
        <sm:Field Name="Date" ListPriority="98"/>
        <sm:Field Name="Project" ListPriority="99"/>
        <sm:Field Name="ProjectTask" ListPriority="97"/>
        <sm:Field Name="SetupTime"/>
        <sm:Field Name="RunTime"/>
        <sm:Field Name="SuspendTime"/>
        <sm:Field Name="Billable"/>
        <sm:Field Name="Description" ListPriority="96"/>
        
        <sm:Action Behavior="Create" Context="Container" Name="Insert" DisplayName="Add" Icon="system://Plus"/>
        <sm:Action Behavior="Delete" Context="Selection" Name="Delete" Icon="system://Trash"/>
    </sm:Container>
    
</sm:Screen>
