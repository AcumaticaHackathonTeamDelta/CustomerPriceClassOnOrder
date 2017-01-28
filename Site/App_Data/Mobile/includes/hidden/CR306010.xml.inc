<?xml version="1.0" encoding="UTF-8"?>
<sm:Screen DisplayName="Activity" Id="CR306010" OpenAs="Form" Type="SimpleScreen" Visible="false" xmlns:sm="http://acumatica.com/mobilesitemap" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">

    <sm:Container Name="Activities">
        <sm:Field Name="Summary"/>
        <sm:Field Name="Type" PickerType="Attached" ForceIsDisabled="true"/>
        <sm:Field Name="Internal"/>
        <sm:Field Name="Date"/>
        <sm:Field Name="StartTime" DisplayName="Time"/>
        <sm:Field Name="Workgroup" PickerType="Attached"/>
        <sm:Field Name="Owner"/>

        <sm:Field Name="TrackTime" Container="TimeActivity" />
        <sm:Field Name="Status" Container="TimeActivity"/>
        <sm:Field Name="TimeSpent" Container="TimeActivity"/>
        <sm:Field Name="Billable" Container="TimeActivity"/>
        <sm:Field Name="BillableTime" Container="TimeActivity"/>

        <sm:Field Name="Project" Container="TimeActivity"/>
        <sm:Field Name="ProjectTask" Container="TimeActivity"/>

        <sm:Field Name="ActivityDetails" TextType="HTML" Container="ActivityDetails"/>

        <sm:Action Behavior="Delete" Context="Record" Name="Delete" After="Close" />
        <sm:Action Behavior="Save" Context="Record" Name="Save"/>
        <sm:Action Behavior="Cancel" Context="Record" Name="Cancel"/>
    </sm:Container>

</sm:Screen>
