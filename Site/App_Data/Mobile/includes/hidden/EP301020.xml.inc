<?xml version="1.0" encoding="UTF-8"?>
<sm:Screen DisplayName="Expense Receipt" Id="EP301020" OpenAs="Form" Type="SimpleScreen" Visible="false" xmlns:sm="http://acumatica.com/mobilesitemap" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    <sm:Container FormActionsToExpand="1" Name="ClaimDetails">
        <sm:Field ForceIsDisabled="true" Name="ReceiptID"/>
        <sm:Field Name="Status"/>
      
        <sm:Field Name="Date" Container="ReceiptDetailsExpenseDetails" />
        <sm:Field Name="Description" Container="ReceiptDetailsExpenseDetails" />
        <sm:Field Name="ExpenseItem" PickerType="Attached" Container="ReceiptDetailsExpenseDetails"/>
        <sm:Field Name="Currency" PickerType="Attached" Container="ReceiptDetailsExpenseDetails" />
        <sm:Field Name="TotalAmount" Container="ReceiptDetailsExpenseDetails" />
      
        <sm:Field Container="ReceiptDetailsExpenseClassification" Name="ProjectContract" SelectorDisplayFormat="KeyDescription">
            <sm:SelectorContainer>
                <sm:Field Name="ContractID"/>
                <sm:Field Name="Description"/>
                <sm:Field Name="Customer"/>
                <sm:Field Name="Status"/>
            </sm:SelectorContainer>
        </sm:Field>
        <sm:Field Container="ReceiptDetailsExpenseClassification" Name="ProjectTask" SelectorDisplayFormat="KeyDescription">
            <sm:SelectorContainer>
                <sm:Field Name="TaskID"/>
                <sm:Field Name="Description"/>
                <sm:Field Name="Status"/>
            </sm:SelectorContainer>
        </sm:Field>
        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Details" Field="EmployeePart">
            <sm:Field Container="ReceiptDetailsExpenseClassification" Name="ExpenseClaim" SelectorDisplayFormat="KeyDescription">
                <sm:SelectorContainer>
                    <sm:Field Name="ReferenceNbr"/>
                    <sm:Field Name="Description"/>
                    <sm:Field Name="Date"/>
                    <sm:Field Name="ClaimTotal"/>
                </sm:SelectorContainer>
            </sm:Field>
            <sm:Field Name="EmployeePart" Container="ReceiptDetailsExpenseDetails"/>
            <sm:Field Container="ReceiptDetailsExpenseClassification" Name="Billable"/>
            <sm:Field Container="ReceiptDetailsExpenseClassification" Name="Customer" SelectorDisplayFormat="KeyDescription">
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
            <sm:Field Container="ReceiptDetailsExpenseClassification" Name="Location">
                <sm:SelectorContainer>
                    <sm:Field Name="LocationID"/>
                    <sm:Field Name="LocationName"/>
                </sm:SelectorContainer>
            </sm:Field>
            <sm:Field Container="ReceiptDetailsFinancialDetails" Name="TaxCategory" PickerType="Attached" SelectorDisplayFormat="KeyDescription"/>
        </sm:Group>
        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Note">
            <sm:Field Name="NoteText" TextType="PlainMultiLine"/>
        </sm:Group>
        <sm:Action After="Close" Behavior="Save" Context="Record" Name="Save"/>
        <sm:Action Behavior="Cancel" Context="Record" Name="Cancel"/>
        <sm:Attachments ImageAdjustmentPreset="Receipt" />
    </sm:Container>
</sm:Screen>
