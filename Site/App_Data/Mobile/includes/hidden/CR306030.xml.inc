<?xml version="1.0" encoding="UTF-8"?>
<sm:Screen DisplayName="Event" Id="CR306030" OpenAs="Form" Type="SimpleScreen" Visible="false" xmlns:sm="http://acumatica.com/mobilesitemap" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">

  <sm:Container FormActionsToExpand="2" Name="Details">
    <sm:Field Name="Status"/>
    <sm:Field Name="Summary"/>
    <sm:Field Name="Location"/>
    <sm:Field DisplayName="Start Date" Name="Date"/>
    <sm:Field DisplayName="Start Time " Name="StartTime"/>
    <sm:Field DisplayName="End Date" Name="EndTimeEndDateDate"/>
    <sm:Field DisplayName="End Time" Name="EndTimeEndDateTime"/>
    <sm:Field Name="AllDay"/>
    <sm:Field Name="Category" PickerType="Attached"/>
    <sm:Field Name="Priority"/>
    <sm:Field Name="ActivityDetails" TextType="HTML"/>

    <sm:Action Behavior="Record" Context="Record" Name="AcceptInvitation"/>
    <sm:Action Behavior="Record" Context="Record" Name="RejectInvitation"/>
    <sm:Action Behavior="Record" Context="Record" Name="Complete"/>
    <sm:Action Behavior="Record" Context="Record" Name="CancelActivity"/>
    <sm:Action Behavior="Delete" Context="Record" Name="Delete" After="Close" />

    <sm:Action Behavior="Save" Context="Record" Name="Save" SyncLongOperation="true"/>
    <sm:Action Behavior="Cancel" Context="Record" Name="Cancel"/>
  </sm:Container>

  <sm:Container ContainerActionsToExpand="2" ListActionsToExpand="2" FieldsToShow="4" FormActionsToExpand="2" Name="Attendees">
    <sm:Attachments Disabled="true"/>
    <sm:Field DisplayName="Name" Name="Name">
      <sm:SelectorContainer FieldsToShow="2">
        <sm:Field Name="DisplayName_" ListPriority="99"/>
        <sm:Field Name="Email" ListPriority="98"/>
        <sm:Field Name="PKID" ForceIsVisible="false"/>
      </sm:SelectorContainer>
    </sm:Field>
    <sm:Field Name="Email"/>
    <sm:Field Name="Comment"/>
    <sm:Field Name="Invitation"/>

    <sm:Action Behavior="Void" Context="Container" Name="SendInvitations" SyncLongOperation="true"/>
    <sm:Action Behavior="Record" Context="Selection" Name="SendPersonalInvitation" SyncLongOperation="true"/>
    <sm:Action Behavior="Record" Context="Record" Name="SendPersonalInvitation" SyncLongOperation="true" After="Close" />
    <sm:Action Behavior="Create" Context="Container" Icon="system://Plus" Name="Insert"/>
    <sm:Action Behavior="Delete" Context="Selection" Icon="system://Trash" Name="Delete"/>
  </sm:Container>

</sm:Screen>
