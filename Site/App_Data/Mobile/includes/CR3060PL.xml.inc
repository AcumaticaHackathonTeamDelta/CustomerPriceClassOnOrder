<?xml version="1.0" encoding="UTF-8"?>
<sm:Screen DisplayName="Cases" Id="CR3060PL" Type="SimpleScreen" xmlns:sm="http://acumatica.com/mobilesitemap" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">

    <sm:Container FieldsToShow="6" Name="Result">
        <sm:Field ForceIsDisabled="true" ForceType="String" Name="CaseID"/>
        <sm:Field Name="Status"/>
        <sm:Field Name="Subject"/>
        <sm:Field Name="Priority"/>
        <sm:Field ForceIsDisabled="true" Name="BusinessAccount"/>
        <sm:Field ForceIsDisabled="true" Name="DateReported"/>

        <sm:Action Behavior="Create" Context="Container" DisplayName="New" Name="Insert" Redirect="true"/>
        <sm:Action Behavior="Open" Context="Container" Name="EditDetail" Redirect="true"/>

    </sm:Container>
</sm:Screen>
