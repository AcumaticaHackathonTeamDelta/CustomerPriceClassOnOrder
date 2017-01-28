<?xml version="1.0" encoding="UTF-8"?>
<sm:Screen DisplayName="Expense Receipts" Id="EP301010" Type="FilterListScreen" Visible="true" xmlns:sm="http://acumatica.com/mobilesitemap" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <sm:Container Name="Selection">
    <sm:Field Name="Employee">
      <sm:SelectorContainer FieldsToShow="1" PickerType="Detached">
        <sm:Field ForceType="String" Name="AcctName"/>
        <sm:Field ForceType="String" Name="AcctCD"/>
      </sm:SelectorContainer>
    </sm:Field>
  </sm:Container>
  <sm:Container ContainerActionsToExpand="1" FieldsToShow="5" ListActionsToExpand="1" Name="ExpenseReceipts">
    <sm:Field Name="ReceiptID"/>
    <sm:Field Name="Date"/>
    <sm:Field Name="DescriptionTranDesc"/>
    <sm:Field Name="ClaimAmount"/>
    <sm:Field Name="Currency"/>

    <sm:Action Behavior="Open" Context="Container" Name="editDetail" Redirect="true"/>

    <sm:Action Behavior="Create" Context="Container" Icon="system://Plus" Name="addNew" Redirect="true"/>

    <sm:Action Behavior="Delete" Context="Selection" Icon="system://Trash" DisplayName="Delete" Name="Delete"/>

    <sm:Action Behavior="Void" Context="List" Name="Process" Redirect="true" SyncLongOperation="true"/>
    <sm:Action Behavior="Void" Context="Container" Name="ProcessAll" Redirect="true" SyncLongOperation="true"/>
  </sm:Container>
</sm:Screen>