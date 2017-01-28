<?xml version="1.0" encoding="UTF-8"?>
<sm:Screen DisplayName="Equipment Time Card" Icon="system://Clock" Id="EP407000" Type="SimpleScreen" xmlns:sm="http://acumatica.com/mobilesitemap" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    <sm:Container Name="TimeCards" FieldsToShow="4" ContainerActionsToExpand="1" ListActionsToExpand="1">
		<sm:Field Name="RefNbr"/>
		<sm:Field Name="EquipmentID"/>
		<sm:Field Name="Status"/>
        
        <sm:Action Behavior="Open" Context="Container" Name="Update" Redirect="true"/>
        <sm:Action Behavior="Create" Context="Container" Name="Create" Redirect="true" Icon="system://Plus"/>
        <sm:Action Behavior="Delete" Context="Selection" Name="Delete" Icon="system://Trash"/>
    </sm:Container>
</sm:Screen>
