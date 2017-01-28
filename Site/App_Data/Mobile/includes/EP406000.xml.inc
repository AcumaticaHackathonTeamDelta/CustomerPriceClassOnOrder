<?xml version="1.0" encoding="UTF-8"?>
<sm:Screen DisplayName="Employee Time Cards" Id="EP406000" Type="FilterListScreen" xmlns:sm="http://acumatica.com/mobilesitemap" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  
  <sm:Container Name="Selection">
    <sm:Field Name="Employee">
      <sm:SelectorContainer FieldsToShow="1" PickerType="Detached">
        <sm:Field ForceType="String" Name="AcctName"/>
        <sm:Field ForceType="String" Name="AcctCD"/>
      </sm:SelectorContainer>
    </sm:Field>
  </sm:Container>
  
  <sm:Container ContainerActionsToExpand="1" FieldsToShow="5" ListActionsToExpand="1" Name="TimeCards">
    <sm:Attachments Disabled="true"/>
    <sm:Field Name="RefNbr"/>
    <sm:Field Name="Status"/>
    <sm:Field Name="WeekStartDate"/>
    <sm:Field Name="WeekDescription"/>
    <sm:Field Name="EmployeeName"/>

    <sm:Action Behavior="Create" Context="Container" DisplayName="Add" Icon="system://Plus" Name="Create" Redirect="true"/>
    <sm:Action Behavior="Open" Context="Container" Name="Update" Redirect="true"/>
    <sm:Action Behavior="Void" Context="Record" Name="Update" Redirect="true"/>
    <sm:Action Behavior="Delete" Context="Selection" Icon="system://Trash" Name="Delete"/>
  </sm:Container>
</sm:Screen>