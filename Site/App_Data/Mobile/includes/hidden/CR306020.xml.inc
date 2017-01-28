<?xml version="1.0" encoding="UTF-8"?>
<sm:Screen DisplayName="Task" Id="CR306020" OpenAs="Form" Type="SimpleScreen" Visible="false" xmlns:sm="http://acumatica.com/mobilesitemap" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">

    <sm:Container FormActionsToExpand="1" Name="Details">
        <sm:Field Name="Summary"/>
        <sm:Field Name="Status"/>
        <sm:Field Name="StartDate"/>
        <sm:Field Name="DueDate"/>
        <sm:Field Name="Completion"/>
        <sm:Field Name="Owner"/>
        <sm:Field Name="Priority"/>
        <sm:Field Name="Category" PickerType="Attached"/>
        <sm:Field Name="CompletedAt"/>

        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Remind at">
            <sm:Field Name="Reminder" Container="DetailsReminder"/>
            <sm:Field DisplayName="Date" Name="RemindAtDate" Container="DetailsReminder"/>
            <sm:Field DisplayName="Time" Name="RemindAtTime" Container="DetailsReminder"/>
        </sm:Group>

        <sm:Field Name="ActivityDetails" TextType="HTML"/>

        <sm:Action Behavior="Record" Context="Record" Name="Complete"/>
        <sm:Action Behavior="Record" Context="Record" Name="CompleteAndFollowUp" Redirect="true" After="Close" />
        <sm:Action Behavior="Record" Context="Record" Name="CancelActivity"/>

        <sm:Action Behavior="Delete" Context="Record" Name="Delete" After="Close" />
        <sm:Action After="Close" Behavior="Save" Context="Record" Name="Save"/>
        <sm:Action Behavior="Cancel" Context="Record" Name="Cancel"/>
    </sm:Container>

    <sm:Container FieldsToShow="4" Name="RelatedActivities">
        <sm:Field Name="Type"/>
        <sm:Field Name="Status"/>
        <sm:Field Name="Summary"/>
        <sm:Field Name="StartDate"/>
        <sm:Field Name="CreatedAt"/>
        <sm:Field Name="Category"/>
        <sm:Field Name="Billable"/>
        <sm:Field Name="TimeSpent"/>
        <sm:Field Name="Overtime"/>
        <sm:Field Name="BillableTime"/>
        <sm:Field Name="BillableOvertime"/>
        <sm:Field Name="Workgroup"/>
        <sm:Field Name="Owner"/>

        <sm:Action Behavior="Open" Context="Container" Name="ChildActivitiesViewDetails" Redirect="true"/>

        <sm:Action Behavior="Delete" Context="Selection" Name="Delete"/>

        <sm:Action Behavior="Create" Context="Container" Name="NewMailActivity" Redirect="true"/>
        <sm:Action Behavior="Create" Context="Container" Name="CNewActivity" Redirect="true"/>
        <sm:Action Behavior="Create" Context="Container" Name="ENewActivity" Redirect="true"/>
        <sm:Action Behavior="Create" Context="Container" Name="MNewActivity" Redirect="true"/>
        <sm:Action Behavior="Create" Context="Container" Name="NNewActivity" Redirect="true"/>
        <sm:Action Behavior="Create" Context="Container" Name="PNewActivity" Redirect="true"/>
        <sm:Action Behavior="Create" Context="Container" Name="RNewActivity" Redirect="true"/>
        <sm:Action Behavior="Create" Context="Container" Name="WNewActivity" Redirect="true"/>

    </sm:Container>

  <sm:Container FieldsToShow="4" Name="RelatedTasks">
    <sm:Field Name="TaskID"/>
    <sm:Field Name="Subject"/>
    <sm:Field Name="Status"/>
    <sm:Field Name="StartDate"/>
    <sm:Field Name="DueDate"/>
    <sm:Field Name="CompletedAt"/>

    <sm:Action Behavior="Open" Context="Container" Name="ViewTask" Redirect="true"/>

    <sm:Action Behavior="Delete" Context="Selection" Name="Delete"/>
    <sm:Action Behavior="Create" Context="Container" Name="AddNewRelatedTask" Redirect="true"/>
  </sm:Container>

</sm:Screen>
