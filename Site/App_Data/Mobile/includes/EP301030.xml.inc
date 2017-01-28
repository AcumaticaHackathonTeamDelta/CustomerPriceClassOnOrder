<?xml version="1.0" encoding="UTF-8"?>
<sm:Screen DisplayName="Expense Claims" Id="EP301030" Type="FilterListScreen" Visible="true" xmlns:sm="http://acumatica.com/mobilesitemap" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">

    <sm:Container Name="Selection">
        <sm:Field Name="Employee">
            <sm:SelectorContainer>
                <sm:Field Name="EmployeeID"/>
                <sm:Field Name="EmployeeName"/>
                <sm:Field Name="ClassID"/>
                <sm:Field Name="Position"/>
                <sm:Field Name="Department"/>
                <sm:Field Name="Login"/>
                <sm:Field Name="DisplayName"/>
            </sm:SelectorContainer>
        </sm:Field>
    </sm:Container>

    <sm:Container ContainerActionsToExpand="1" FieldsToShow="5" ListActionsToExpand="1" Name="Claim">
        <sm:Field ForceIsDisabled="true" Name="ReferenceNbr"/>
        <sm:Field Name="Status"/>
        <sm:Field Name="Date"/>
        <sm:Field Name="ClaimTotal"/>
        <sm:Field Name="Description"/>

        <sm:Action Behavior="Open" Context="Container" Name="EditDetail" Redirect="true"/>

        <sm:Action Behavior="Create" Context="Container" Icon="system://Plus" Name="CreateNew" Redirect="true"/>

        <sm:Action Behavior="Delete" Context="Selection" Icon="system://Trash" Name="Delete" DisplayName="Delete" />
        <sm:Action Behavior="Void" Context="Selection" Name="Submit"/>
    </sm:Container>
</sm:Screen>
