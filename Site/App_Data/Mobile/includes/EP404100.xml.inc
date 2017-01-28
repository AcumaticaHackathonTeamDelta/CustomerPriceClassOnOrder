<?xml version="1.0" encoding="UTF-8"?>
<sm:Screen DisplayName="Events" Id="EP404100" Type="FilterListScreen" xmlns:sm="http://acumatica.com/mobilesitemap" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    <sm:Container Name="Selection">
        <sm:Field Name="Employee" ForceRequired="True">
            <sm:SelectorContainer>
                <sm:Field Name="EmployeeID"/>
                <sm:Field Name="EmployeeName"/>
                <sm:Field Name="Position"/>
                <sm:Field Name="Department"/>
                <sm:Field Name="Location"/>
                <sm:Field Name="Owner"/>
                <sm:Field ForceIsVisible="false" Name="ExtKey"/>
            </sm:SelectorContainer>
        </sm:Field>
    </sm:Container>

    <sm:Container ContainerActionsToExpand="1" FieldsToShow="6" ListActionsToExpand="1" Name="Events">
        <sm:Field Name="Summary"/>
        <sm:Field Name="Status"/>
        <sm:Field Name="DayOfWeek"/>
        <sm:Field Name="Date"/>
        <sm:Field Name="StartTime"/>
        <sm:Field Name="EndTime"/>

        <sm:Action Behavior="Open" Context="Container" Name="EventsViewDetails" Redirect="true"/>
        <sm:Action Behavior="Create" Context="Container" Icon="system://Plus" Name="AddNew" Redirect="true"/>
        <sm:Action Behavior="Void" Context="Selection" Name="cancelActivity" SyncLongOperation="true"/>
        <sm:Action Behavior="Void" Context="Selection" Name="Complete" Priority="99" SyncLongOperation="true"/>
    </sm:Container>
</sm:Screen>

