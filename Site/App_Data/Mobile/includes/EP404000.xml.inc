<?xml version="1.0" encoding="UTF-8"?>
<sm:Screen DisplayName="Tasks" Id="EP404000" Type="SimpleScreen" xmlns:sm="http://acumatica.com/mobilesitemap" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    <sm:Container ContainerActionsToExpand="1" ListActionsToExpand="1" FieldsToShow="4" Name="Tasks">
        <sm:Field Name="Summary"/>
        <sm:Field Name="Status"/>
        <sm:Field Name="StartDate"/>

        <sm:Action Behavior="Open" Context="Container" Name="TasksViewDetails" Redirect="true"/>
        <sm:Action Behavior="Create" Context="Container" Icon="system://Plus" Name="AddNew" Redirect="true"/>
        <sm:Action Behavior="Void" Context="Selection" Name="Complete" SyncLongOperation="true"/>
        <sm:Action Behavior="Void" Context="Selection" Name="cancelActivity" SyncLongOperation="true"/>
    </sm:Container>
</sm:Screen>