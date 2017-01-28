<?xml version="1.0" encoding="UTF-8"?>
<sm:Screen DisplayName="Approvals" Id="EP503010" Type="SimpleScreen" xmlns:sm="http://acumatica.com/mobilesitemap" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    <sm:Container ContainerActionsToExpand="1" FieldsToShow="9" ListActionsToExpand="1" Name="Claims">
        <sm:Field Name="Type"/>
        <sm:Field Name="ReferenceNbr"/>
        <sm:Field Name="DocumentDate"/>
        <sm:Field Name="AccountName"/>
        <sm:Field Name="DescriptionDescr"/>
        <sm:Field Name="BusinessAccount"/>
        <sm:Field Name="TotalAmount"/>
        <sm:Field Name="Currency"/>

        <sm:Action Behavior="Void" Context="Container" Name="ProcessAll" SyncLongOperation="true"/>
        <sm:Action Behavior="Void" Context="List" Name="Process" SyncLongOperation="true"/>
        <sm:Action Behavior="Open" Context="Container" Name="EditDetail" Redirect="true"/>
    </sm:Container>
</sm:Screen>