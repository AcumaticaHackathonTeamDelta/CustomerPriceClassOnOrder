<?xml version="1.0" encoding="UTF-8"?>
<sm:Screen DisplayName="Timecard" Id="EP305000" OpenAs="Form" Type="SimpleScreen" Visible="false" xmlns:sm="http://acumatica.com/mobilesitemap" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    <sm:Container FormActionsToExpand="0" Name="DocumentSummary">
        <sm:Attachments Disabled="true"/>
        <sm:Field ForceIsDisabled="true" FormPriority="99" Name="RefNbr"/>
        <sm:Field FormPriority="98" Name="Status"/>
        <sm:Field FormPriority="97" Name="Week"/>
           
        <sm:ContainerLink Container="Summary" Control="ListItem" Priority="52"/>
        <sm:ContainerLink Container="Details" Control="ListItem" Priority="51"/>
        <sm:ContainerLink Container="Materials" Control="ListItem" Priority="50"/>
           
        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Time Totals" FormPriority="1">
            <sm:Field Container="DocumentSummaryTotal" Name="TotalTimeSpent"/>
            <sm:Field Container="DocumentSummaryTotal" Name="TotalBillable"/>
            <sm:Field Container="DocumentSummaryRegular" Name="TimeSpent"/>
            <sm:Field Container="DocumentSummaryRegular" Name="Billable"/>
            <sm:Field Container="DocumentSummaryOvertime" Name="OvertimeSpent"/>
            <sm:Field Container="DocumentSummaryOvertime" Name="BillableOvertime"/>
        </sm:Group>

           
        <sm:Action After="Close" Behavior="Save" Context="Record" Name="Save"/>
        <sm:Action Behavior="Cancel" Context="Record" Name="Cancel"/>

        <sm:Action Behavior="Record" Context="Record" Name="ReleaseAction" SyncLongOperation="true" />
        <sm:Action Behavior="Record" Context="Record" Name="ApproveAction"/>
        <sm:Action Behavior="Record" Context="Record" Name="RejectAction"/>
        <sm:Action Behavior="Record" Context="Record" Name="CorrectAction"/>
        <sm:Action Behavior="Record" Context="Record" Name="SubmitAction"/>
        <sm:Action Behavior="Record" Context="Record" Name="EditAction"/>
    </sm:Container>

    <sm:Container ContainerActionsToExpand="1" FieldsToShow="5" ListActionsToExpand="0" Name="Summary">
        <sm:Attachments Disabled="true"/>
        <sm:Field ListPriority="9" Name="EarningType">
            <sm:SelectorContainer FieldsToShow="2">
                <sm:Field ForceType="String" Name="Code"/>
                <sm:Field ForceType="String" Name="Description"/>
            </sm:SelectorContainer>
        </sm:Field>
        <sm:Field ListPriority="7" Name="Project">
            <sm:SelectorContainer FieldsToShow="2">
                <sm:Field ForceType="String" Name="ProjectID"/>
                <sm:Field ForceType="String" Name="Description"/>
                <sm:Field ForceType="String" Name="Status"/>
            </sm:SelectorContainer>
        </sm:Field>
        <sm:Field ListPriority="5" Name="ProjectTask">
            <sm:SelectorContainer FieldsToShow="2">
                <sm:Field ForceType="String" Name="Description"/>
                <sm:Field ForceType="String" Name="TaskID"/>
                <sm:Field ForceType="String" Name="Status"/>
            </sm:SelectorContainer>
        </sm:Field>
        <sm:Field ListPriority="8" Name="TimeSpent"/>
        <sm:Field ListPriority="4" Name="Billable"/>
        <sm:Field ListPriority="6" Name="Description"/>
		<sm:Field ListPriority="0" Name="ApprovalStatus"/>
           
        <sm:Group Collapsable="false" Collapsed="false" DisplayName="Week Days" ListPriority="2">
            <sm:Field Name="Mon"/>
            <sm:Field Name="Tue"/>
            <sm:Field Name="Wed"/>
            <sm:Field Name="Thu"/>
            <sm:Field Name="Fri"/>
            <sm:Field Name="Sat"/>
            <sm:Field Name="Sun"/>
        </sm:Group>
           
        <sm:Action Behavior="Create" Context="Container" DisplayName="Add" Icon="system://Plus" Name="Insert"/>
        <sm:Action Behavior="Void" Context="Container" Name="NormalizeTimecard"/>
        <sm:Action Behavior="Void" Context="Container" Name="PreloadFromTasks" Redirect="true" RedirectToContainer="PreloadFromTasks$List"/>
        <sm:Action Behavior="Void" Context="Container" Name="PreloadFromPreviousTimecard"/>
        <sm:Action Behavior="Void" Context="Container" Name="PreloadHolidays"/>
        <sm:Action Behavior="Delete" Context="Selection" Icon="system://Trash" Name="Delete"/>
    </sm:Container>
     
    <sm:Container ContainerActionsToExpand="1" FieldsToShow="8" FormActionsToExpand="1" ListActionsToExpand="1" Name="Details">
        <sm:Field ListPriority="9" Name="Date"/>
        <sm:Field ListPriority="7" Name="EarningType"/>
        <sm:Field ListPriority="8" Name="Project"/>
        <sm:Field ListPriority="6" Name="ProjectTask"/>
        <sm:Field ListPriority="4" Name="TimeSpent"/>
        <sm:Field ForceType="String" ListPriority="5" Name="Summary"/>
        <sm:Field ListPriority="0" Name="Billable"/>
        <sm:Field ListPriority="0" Name="BillableTime"/>
        <sm:Field ListPriority="0" Name="BillableOT"/>
        <sm:Field ListPriority="0" Name="Task"/>
        <sm:Field ListPriority="0" Name="RH"/>
        <sm:Field ListPriority="0" Name="OT"/>
        <sm:Field ListPriority="0" Name="OTMult"/>
        <sm:Field ListPriority="3" Name="ApprovalStatus"/>
        <sm:Field ListPriority="10" Name="Day"/>
        
        <sm:Action Behavior="Void" Context="Record" DisplayName="View" Name="View" Redirect="true"/>
        <sm:Action Behavior="Create" Context="Container" DisplayName="Add" Icon="system://Plus" Name="Insert"/>
        <sm:Action Behavior="Create" Context="Container" DisplayName="Add Activity" Name="CreateActivity" Redirect="true"/>
        <sm:Action Behavior="Delete" Context="Selection" Icon="system://Trash" Name="Delete"/>
    </sm:Container>

    <sm:Container ContainerActionsToExpand="1" FieldsToShow="4" ListActionsToExpand="1" Name="Materials">
        <sm:Field Name="Project"/>
        <sm:Field Name="ProjectTask"/>
        <sm:Field Name="InventoryID"/>
        <sm:Field Name="Description"/>
        <sm:Field Name="TotalQty"/>
        <sm:Field Name="UOM" PickerType="Attached"/>
           
        <sm:Group Collapsable="false" Collapsed="false" DisplayName="Week Days">
            <sm:Field Name="Mon"/>
            <sm:Field Name="Tue"/>
            <sm:Field Name="Wed"/>
            <sm:Field Name="Thu"/>
            <sm:Field Name="Fri"/>
            <sm:Field Name="Sat"/>
            <sm:Field Name="Sun"/>
        </sm:Group>

        <sm:Action Behavior="Create" Context="Container" DisplayName="Add" Icon="system://Plus" Name="Insert"/>
        <sm:Action Behavior="Delete" Context="Selection" Icon="system://Trash" Name="Delete"/>
    </sm:Container>
     
    <sm:Container ContainerActionsToExpand="1" FieldsToShow="3" ListActionsToExpand="1" Name="PreloadFromTasks" Type="SelectionActionList" Visible="false">
        <sm:Field Name="Summary"/>
        <sm:Field Name="Project"/>
        <sm:Field Name="ProjectTask"/>
        <sm:Action After="Close" Behavior="Void" Context="List" ForceDialog="OK" Icon="system://Plus" Name="PreloadFromTasks"/>
    </sm:Container>

</sm:Screen>
