<?xml version="1.0" encoding="UTF-8"?>
<sm:Screen DisplayName="Opportunity" Id="CR304000" OpenAs="Form" Type="SimpleScreen" Visible="false" xmlns:sm="http://acumatica.com/mobilesitemap" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">

    <sm:Container Name="OpportunitySummary">
        <sm:Field ForceIsDisabled="true" ForceType="String" Name="OpportunityID"/>
        <sm:Field Name="Status"/>
        <sm:Field Name="Reason"/>
        <sm:Field Name="Stage"/>
        <sm:Field Name="Source"/>
        <sm:Field Name="Subject"/>
        <sm:Field Name="BusinessAccount"/>
        <sm:Field Name="Contact"/>
        <sm:Field Name="ManualAmount"/>
        <sm:Field Name="Discount"/>
        <sm:Field Name="Total"/>

        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Attributes">
            <sm:Attributes From="Attributes"/>
        </sm:Group>

        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Details">
            <sm:Field Container="Details" Name="Branch"/>
            <sm:Field Container="Details" Name="Project"/>
            <sm:Field Container="Details" ForceIsDisabled="true" ForceType="String" Name="Workgroup"/>
            <sm:Field Container="Details" Name="WorkgroupDescription"/>
            <sm:Field Container="Details" Name="Owner"/>
            <sm:Field Container="Details" Name="OwnerEmployeeName"/>
            <sm:Field Container="Details" Name="ParentAccount" />
            <sm:Field Container="Details" Name="Campaign"/>
            <sm:Field Container="Details" Name="TaxZone"/>
            <sm:Field Container="Details" Name="Estimation" />
            <sm:Field Container="Details" Name="WgtTotal"/>
            <sm:Field Container="Details" Name="ConvertedLead"/>
            <sm:Field Container="Details" Name="ConvertedLeadDisplayName"/>
            <sm:Field Container="Details" Name="Details" TextType="HTML"/>
        </sm:Group>

        <sm:Action Behavior="Save" Context="Record" Name="Save"/>
        <sm:Action Behavior="Cancel" Context="Record" Name="Cancel"/>

    </sm:Container>

    <sm:Container FieldsToShow="3" Name="Activities">
        <sm:Field Name="Type"/>
        <sm:Field Name="StartDate"/>
        <sm:Field ForceIsDisabled="true" ForceType="String" Name="Summary"/>
        <sm:Field Name="NoteText" TextType="PlainMultiLine"/>

        <sm:Action Behavior="Open" Context="Container" Name="ViewActivity" Redirect="true"/>

        <sm:Action Behavior="Record" Context="Container" Name="NewTask" Redirect="true"/>
        <sm:Action Behavior="Record" Context="Container" Name="NewEvent" Redirect="true"/>
        <sm:Action Behavior="Record" Context="Container" Name="CNewActivity" Redirect="true"/>
        <sm:Action Behavior="Record" Context="Container" Name="ENewActivity" Redirect="true"/>
        <sm:Action Behavior="Record" Context="Container" Name="EVCRNewActivity" Redirect="true"/>
        <sm:Action Behavior="Record" Context="Container" Name="EVPRNewActivity" Redirect="true"/>
        <sm:Action Behavior="Record" Context="Container" Name="MNewActivity" Redirect="true"/>
        <sm:Action Behavior="Record" Context="Container" Name="NNewActivity" Redirect="true"/>
        <sm:Action Behavior="Record" Context="Container" Name="PNewActivity" Redirect="true"/>
        <sm:Action Behavior="Record" Context="Container" Name="RNewActivity" Redirect="true"/>
        <sm:Action Behavior="Record" Context="Container" Name="SRNewActivity" Redirect="true"/>
        <sm:Action Behavior="Record" Context="Container" Name="WNewActivity" Redirect="true"/>
    </sm:Container>

    <sm:Container FieldsToShow="6" Name="Relations">
        <sm:Field Name="Role"/>
        <sm:Field Name="AccountEmployee"/>
        <sm:Field Name="Name"/>
        <sm:Field Name="Contact"/>
        <sm:Field Name="Email"/>
        <sm:Field Name="AddToCC"/>

        <sm:Action Behavior="Create" Context="Container" Name="Insert"/>
        <sm:Action Behavior="Delete" Context="Selection" Name="Delete"/>
    </sm:Container>

    <sm:Container FieldsToShow="13" Name="Products">
        <sm:Field Name="InventoryID"/>
        <sm:Field Name="FreeItem"/>
        <sm:Field Name="Quantity"/>
        <sm:Field ForceIsDisabled="true" ForceType="String" Name="UOM"/>
        <sm:Field Name="UnitPrice"/>
        <sm:Field Name="ExtPrice"/>
        <sm:Field Name="Discount"/>
        <sm:Field Name="DiscountAmount"/>
        <sm:Field Name="ManualDiscount"/>
        <sm:Field Name="Amount"/>
        <sm:Field ForceIsDisabled="true" ForceType="String" Name="TaxCategory"/>
        <sm:Field Name="TransactionDescription"/>
        <sm:Field ForceIsDisabled="true" ForceType="String" Name="Warehouse"/>

        <sm:Action Behavior="Create" Context="Container" Name="Insert"/>
        <sm:Action Behavior="Delete" Context="Selection" Name="Delete"/>
    </sm:Container>

    <sm:Container FieldsToShow="4" Name="TaxDetails">
        <sm:Field ForceIsDisabled="true" ForceType="String" Name="TaxID"/>
        <sm:Field Name="TaxRate"/>
        <sm:Field Name="TaxableAmount"/>
        <sm:Field Name="TaxAmount"/>

        <sm:Action Behavior="Create" Context="Container" Name="Insert"/>
        <sm:Action Behavior="Delete" Context="Selection" Name="Delete"/>
    </sm:Container>

    <sm:Container FieldsToShow="9" Name="DiscountDetails">
        <sm:Field Name="DiscountID"/>
        <sm:Field Name="SequenceID"/>
        <sm:Field Name="Type"/>
        <sm:Field Name="DiscountableAmt"/>
        <sm:Field Name="DiscountableQty"/>
        <sm:Field Name="DiscountAmt"/>
        <sm:Field Name="Discount"/>
        <sm:Field ForceIsDisabled="true" ForceType="String" Name="FreeItem"/>
        <sm:Field Name="FreeItemQty"/>
    </sm:Container>

</sm:Screen>
