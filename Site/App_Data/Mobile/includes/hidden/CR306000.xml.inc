<?xml version="1.0" encoding="UTF-8"?>
<sm:Screen DisplayName="Case" Id="CR306000" OpenAs="Form" Type="SimpleScreen" Visible="false" xmlns:sm="http://acumatica.com/mobilesitemap" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    <sm:Container FormActionsToExpand="1" Name="CaseSummary">
        <sm:Field ForceIsDisabled="true" Name="CaseID"/>
        <sm:Field Name="DateReported"/>
        <sm:Field Name="LastActivityDate"/>
        <sm:Field Name="ClassID" PickerType="Attached"/>
        <sm:Field Name="BusinessAccount">
            <sm:SelectorContainer FieldsToShow="9">
                <sm:Field Name="AccountID"/>
                <sm:Field Name="AccountName"/>
                <sm:Field Name="Type"/>
                <sm:Field Name="ClassID"/>
                <sm:Field Name="Status"/>
                <sm:Field Name="Phone1"/>
                <sm:Field Name="City"/>
                <sm:Field Name="Country"/>
                <sm:Field Name="Email"/>
            </sm:SelectorContainer>
        </sm:Field>
        <sm:Field Name="Contact">
            <sm:SelectorContainer FieldsToShow="4">
                <sm:Field Name="DisplayName_"/>
                <sm:Field Name="Position"/>
                <sm:Field Name="Email"/>
                <sm:Field Name="ContactID"/>
            </sm:SelectorContainer>
        </sm:Field>
		<sm:Field Name="Owner"/>
        <sm:Field Container="AdditionalInfoBusinessAccountDetails" Name="Contract"/>
        <sm:Field Name="Subject"/>
        <sm:Field Name="Status"/>
        <sm:Field Name="Reason"/>
        <sm:Field Name="Severity"/>
        <sm:Field Name="Priority"/>
        <sm:Field Container="Details" Name="Description" TextType="HTML"/>

        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Attributes">
            <sm:Attributes From="Attributes"/>
        </sm:Group>

        <sm:Action Behavior="Record" Context="Record" Name="TakeCase"/>

        <sm:Action After="Close" Behavior="Save" Context="Record" Name="Save"/>
        <sm:Action Behavior="Cancel" Context="Record" Name="Cancel"/>
    </sm:Container>

    <sm:Container FieldsToShow="6" ListActionsToExpand="0" Name="Activities">
        <sm:Field Name="Summary"/>
        <sm:Field Name="Status"/>
        <sm:Field Name="Type"/>
        <sm:Field Name="StartDate"/>
        <sm:Field Name="Category"/>
        <sm:Field Name="Owner"/>
        <sm:Field Name="Billable"/>
        <sm:Field Name="TimeSpent"/>
        <sm:Field Name="Overtime"/>
        <sm:Field Name="BillableTime"/>
        <sm:Field Name="BillableOvertime"/>
        <sm:Field Name="Workgroup"/>

        <sm:Action Behavior="Open" Context="Container" Name="ViewActivity" Redirect="true"/>

        <sm:Action Behavior="Create" Context="Container" Name="NewTask" Redirect="true"/>
        <sm:Action Behavior="Create" Context="Container" Name="NewEvent" Redirect="true"/>
        <sm:Action Behavior="Create" Context="Container" Name="NewMailActivity" Redirect="true"/>
        <sm:Action Behavior="Create" Context="Container" Name="CNewActivity" Redirect="true"/>
        <sm:Action Behavior="Create" Context="Container" Name="ENewActivity" Redirect="true"/>
        <sm:Action Behavior="Create" Context="Container" Name="MNewActivity" Redirect="true"/>
        <sm:Action Behavior="Create" Context="Container" Name="NNewActivity" Redirect="true"/>
        <sm:Action Behavior="Create" Context="Container" Name="PNewActivity" Redirect="true"/>
        <sm:Action Behavior="Create" Context="Container" Name="RNewActivity" Redirect="true"/>
        <sm:Action Behavior="Create" Context="Container" Name="WNewActivity" Redirect="true"/>
    </sm:Container>

</sm:Screen>
