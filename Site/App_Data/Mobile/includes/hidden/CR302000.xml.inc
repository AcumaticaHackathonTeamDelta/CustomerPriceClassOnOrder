<?xml version="1.0" encoding="UTF-8"?>
<sm:Screen DisplayName="Contact Summary" Id="CR302000" OpenAs="Form" Type="SimpleScreen" Visible="false" xmlns:sm="http://acumatica.com/mobilesitemap" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    
    <sm:Container Name="ContactSummary">
        <sm:Field Container="DetailsSummary" Name="FirstName"/>
        <sm:Field Container="DetailsSummary" Name="LastName"/>
        <sm:Field Container="DetailsSummary" Name="Position"/>
        <sm:Field Container="DetailsSummary" Name="BusinessAccount"/>
        <sm:Field Container="DetailsSummary" Name="CompanyName"/>
        <sm:Field Container="DetailsContact" Name="Email"/>
        <sm:Field Container="DetailsContact" Name="Web"/>
        <sm:Field Container="DetailsContact" Name="Phone1"/>
        <sm:Field Container="DetailsContact" Name="Phone2"/>
        <sm:Field Container="DetailsContact" Name="Phone3"/>
        <sm:Field Container="DetailsContact" Name="Fax"/>
        
        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Address">
            <sm:Field Container="DetailsAddressSameAsInAccount" Name="SameAsInAccount"/>
            <sm:Field Container="DetailsAddress" Name="AddressLine1"/>
            <sm:Field Container="DetailsAddress" Name="AddressLine2"/>
            <sm:Field Container="DetailsAddress" Name="City"/>
            <sm:Field Container="DetailsAddress" Name="Country"/>
            <sm:Field Container="DetailsAddress" Name="State"/>
            <sm:Field Container="DetailsAddress" Name="PostalCode"/>
        </sm:Group>
        
        <sm:Group Collapsable="true" Collapsed="true" DisplayName="CRM">
            <sm:Field Name="Active"/>
            <sm:Field Name="Type"/>
            <sm:Field Container="DetailsCRM" Name="ContactClass"/>
            <sm:Field Container="DetailsSummary" Name="ParentBusinessAccount"/>
            <sm:Field Container="DetailsCRM" Name="ContactMethod"/>
            <sm:Field Container="DetailsCRM" Name="DoNotCall"/>
            <sm:Field Container="DetailsCRM" Name="DoNotFax"/>
            <sm:Field Container="DetailsCRM" Name="DoNotEmail"/>
            <sm:Field Container="DetailsCRM" Name="DoNotMail"/>
            <sm:Field Container="DetailsCRM" Name="NoMassMail"/>
            <sm:Field Container="DetailsCRM" Name="NoMarketing"/>
            <sm:Field Container="DetailsCRM" Name="DateOfBirth"/>
            <sm:Field Container="DetailsCRM" Name="LastIncomingActivity" ForceIsDisabled="true"/>
            <sm:Field Container="DetailsCRM" Name="LastOutgoingActivity" ForceIsDisabled="true"/>
            <sm:Field Container="AdditionalInfoCommon" Name="Gender"/>
            <sm:Field Container="AdditionalInfoCommon" Name="MaritalStatus"/>
            <sm:Field Container="AdditionalInfoCommon" Name="SpousePartnerName"/>
        </sm:Group>
        
        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Attributes">
            <sm:Attributes From="Attributes"/>
        </sm:Group>
        
        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Owner Information">
            <sm:Field Name="Workgroup" PickerType="Attached"/>
            <sm:Field Name="Owner" PickerType="Attached"/>
        </sm:Group>
        
        <sm:Action Context="Record" Name="Save" Behavior="Save" After="Close"/>
        <sm:Action Context="Record" Name="Cancel" Behavior="Cancel"/>
    </sm:Container>
    
    <sm:Container FieldsToShow="3" Name="Activities">
	   <sm:Field Name="Type"/>
        <sm:Field Name="StartDate"/>
        <sm:Field ForceType="String" Name="Summary"/>
        
        
        <sm:Action Behavior="Open" Context="Container" Name="ViewActivity" Redirect="true"/>
        
        <sm:Action Behavior="Record" Context="Container" Name="NewTask" Redirect="true"/>
        <sm:Action Behavior="Record" Context="Container" Name="NewEvent" Redirect="true"/>
        <sm:Action Behavior="Record" Context="Container" Name="NewMailActivity" Redirect="true"/>
        <sm:Action Behavior="Record" Context="Container" Name="CNewActivity" Redirect="true"/>
        <sm:Action Behavior="Record" Context="Container" Name="ENewActivity" Redirect="true"/>
        <sm:Action Behavior="Record" Context="Container" Name="MNewActivity" Redirect="true"/>
        <sm:Action Behavior="Record" Context="Container" Name="NNewActivity" Redirect="true"/>
        <sm:Action Behavior="Record" Context="Container" Name="PNewActivity" Redirect="true"/>
        <sm:Action Behavior="Record" Context="Container" Name="RNewActivity" Redirect="true"/>
        <sm:Action Behavior="Record" Context="Container" Name="WNewActivity" Redirect="true"/>
    </sm:Container>
    
    <sm:Container Name="Relations" FieldsToShow="4" ContainerActionsToExpand="1" ListActionsToExpand="1">
        <sm:Field Name="Role" ListPriority="98"/>
        <sm:Field Name="AccountEmployee" ListPriority="99">
            <sm:SelectorContainer FieldsToShow="3">
                <sm:Field Name="BusinessAccount"/>
                <sm:Field Name="Type"/>
                <sm:Field Name="BusinessAccountName"/>
                <sm:Field Name="BAccountID" ForceIsVisible="false"/>
            </sm:SelectorContainer>
        </sm:Field>
        <sm:Field Name="Name"/>
        <sm:Field Name="Contact">
            <sm:SelectorContainer FieldsToShow="2">
                <sm:Field Name="DisplayName_"/>
                <sm:Field Name="Email"/>
                <sm:Field Name="ContactID" ForceIsVisible="false"/>
            </sm:SelectorContainer>
        </sm:Field>
        <sm:Field Name="ContactDisplayName" ListPriority="97"/>
        <sm:Field Name="Email" ListPriority="96"/>
        <sm:Field Name="AddToCC"/>
        
        <sm:Action Behavior="Create" Context="Container" Name="Insert" Icon="system://Plus"/>
    </sm:Container>
    
    <sm:Container Name="Opportunities" FieldsToShow="4" ContainerActionsToExpand="1" ListActionsToExpand="1">
        <sm:Field Name="OpportunityID"/>
        <sm:Field Name="Total"/>
        <sm:Field Name="Subject"/>
        <sm:Field Name="Stage"/>
        
        <sm:Action Behavior="Open" Context="Container" Name="OpportunitiesViewDetails" Redirect="true"/>
        <sm:Action Behavior="Create" Context="Container" Name="AddOpportunityAction" Redirect="true"  Icon="system://Plus"/>
    </sm:Container>
    
    <sm:Container Name="Cases" FieldsToShow="5" ContainerActionsToExpand="1" ListActionsToExpand="1">
        <sm:Field Name="CaseID"/>
        <sm:Field Name="Status"/>
        <sm:Field Name="ClassID"/>
        <sm:Field Name="Reason"/>
        <sm:Field Name="Subject"/>
        
        <sm:Action Behavior="Open" Context="Container" Name="CasesViewDetails" Redirect="true"/>
        <sm:Action Behavior="Create" Context="Container" Name="AddCaseAction" Redirect="true" Icon="system://Plus"/>
    </sm:Container>
    
    <sm:Container Name="Campaigns" FieldsToShow="2" ContainerActionsToExpand="1" ListActionsToExpand="1">
        <sm:Field Name="CampaignID"/>
        <sm:Field Name="Status"/>
        
        <sm:Action Behavior="Delete" Context="Selection" Name="Delete"/>
    </sm:Container>
    
    <sm:Container Name="MarketingLists" FieldsToShow="2" ContainerActionsToExpand="1" ListActionsToExpand="1">
        <sm:Field Name="MarketingListID"/>
        <sm:Field Name="Activated"/>
        
        <sm:Action Behavior="Delete" Context="Selection" Name="Delete"/>
    </sm:Container>
    
</sm:Screen>
