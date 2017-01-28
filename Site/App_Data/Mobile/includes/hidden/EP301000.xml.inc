<?xml version="1.0" encoding="UTF-8"?>
<sm:Screen DisplayName="Expense Claim" Id="EP301000" OpenAs="Form" Type="SimpleScreen" Visible="false" xmlns:sm="http://acumatica.com/mobilesitemap" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">

    <sm:Container FormActionsToExpand="1" Name="DocumentSummary">
        <sm:Attachments Disabled="true"/>
        <sm:Field ForceIsDisabled="true" Name="ReferenceNbr"/>
        <sm:Field Name="Date"/>
        <sm:Field Name="Status"/>
        <sm:Field Name="Description"/>
        <sm:Field Name="ClaimTotal"/>
        <sm:Field Name="Currency" PickerType="Attached"/>
        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Note">
            <sm:Field Name="NoteText" TextType="PlainMultiLine"/>
        </sm:Group>

        <sm:Action Behavior="Void" Context="Record" Name="ShowSubmitReceipt" Redirect="true" RedirectToContainer="AddReceipts$List"/>
        <sm:Action Behavior="Record" Context="Record" Name="ApproveAction"/>
        <sm:Action Behavior="Record" Context="Record" Name="RejectAction"/>
        <sm:Action Behavior="Record" Context="Record" Name="SubmitAction"/>
        <sm:Action Behavior="Record" Context="Record" Name="EditAction"/>

        <sm:Action After="Close" Behavior="Save" Context="Record" Name="Save"/>
        <sm:Action Behavior="Cancel" Context="Record" Name="Cancel"/>
    </sm:Container>

    <sm:Container ContainerActionsToExpand="1" ListActionsToExpand="1" FieldsToShow="4" Name="ExpenseClaimDetails">
        <sm:Field ListPriority="99" Name="Date"/>
        <sm:Field ListPriority="97" Name="Description"/>
        <sm:Field Name="ExpenseItem" PickerType="Attached"/>
        <sm:Field ListPriority="96" Name="Currency" PickerType="Attached"/>
        <sm:Field ListPriority="98" Name="TotalAmount"/>
        <sm:Field Name="ProjectContract" SelectorDisplayFormat="KeyDescription">
            <sm:SelectorContainer>
                <sm:Field Name="ContractID"/>
                <sm:Field Name="Description"/>
                <sm:Field Name="Customer"/>
                <sm:Field Name="Status"/>
            </sm:SelectorContainer>
        </sm:Field>
        <sm:Field Name="ProjectTask" SelectorDisplayFormat="KeyDescription">
            <sm:SelectorContainer>
                <sm:Field Name="TaskID"/>
                <sm:Field Name="Description"/>
                <sm:Field Name="Status"/>
            </sm:SelectorContainer>
        </sm:Field>
        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Details">
            <sm:Field Name="EmployeePart"/>
            <sm:Field Name="Billable"/>
            <sm:Field Name="Customer" SelectorDisplayFormat="KeyDescription">
                <sm:SelectorContainer>
                    <sm:Field Name="CustomerID"/>
                    <sm:Field Name="CustomerName"/>
                    <sm:Field Name="AddressLine1"/>
                    <sm:Field Name="AddressLine2"/>
                    <sm:Field Name="PostalCode"/>
                    <sm:Field Name="Phone1"/>
                    <sm:Field Name="City"/>
                    <sm:Field Name="Country"/>
                    <sm:Field Name="TaxRegistrationID"/>
                    <sm:Field Name="CurrencyID"/>
                    <sm:Field Name="Attention"/>
                    <sm:Field Name="CustomerClass"/>
                    <sm:Field Name="Status"/>
                </sm:SelectorContainer>
            </sm:Field>
            <sm:Field Name="Location">
                <sm:SelectorContainer>
                    <sm:Field Name="LocationID"/>
                    <sm:Field Name="LocationName"/>
                </sm:SelectorContainer>
            </sm:Field>
            <sm:Field Name="TaxCategory" PickerType="Attached" SelectorDisplayFormat="KeyDescription"/>
        </sm:Group>
        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Note">
            <sm:Field Name="NoteText" TextType="PlainMultiLine"/>
        </sm:Group>

        <sm:Action Behavior="Create" Context="Container" Icon="system://Plus" Name="Insert"/>
        <sm:Action Behavior="Delete" Context="Selection" Icon="system://Trash" Name="Delete"/>
    </sm:Container>

    <sm:Container ListActionsToExpand="1" Name="TaxDetails">
        <sm:Field Name="TaxID"/>
        <sm:Field Name="TaxRate"/>
        <sm:Field Name="TaxableAmount"/>
        <sm:Field Name="TaxAmount"/>

        <sm:Action Behavior="Create" Context="Container" Icon="system://Plus" Name="Insert"/>
        <sm:Action Behavior="Delete" Context="Selection" Icon="system://Trash" Name="Delete"/>
    </sm:Container>

    <sm:Container ListActionsToExpand="1" Name="AddReceipts" Type="SelectionActionList" Visible="false">
        <sm:Field Name="Description"/>
        <sm:Field Name="Date"/>
        <sm:Field Name="ClaimAmount"/>

        <sm:Action Behavior="Void" Context="List" Icon="system://Plus" Name="SubmitReceipt"/>
    </sm:Container>

</sm:Screen>