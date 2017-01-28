<?xml version="1.0" encoding="UTF-8"?>
<sm:Screen DisplayName="Account Summary" Id="CR303000" OpenAs="Form" Type="SimpleScreen" Visible="false" xmlns:sm="http://acumatica.com/mobilesitemap" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    <sm:Container Name="AccountSummary">
        <sm:Field Name="BusinessAccount" ForceType="String" TextType="PlainSingleLine"/>
        <sm:Field Name="BusinessAccountName"/>
        <sm:Field Name="Status"/>
        <sm:Field Name="Type"/>

        <sm:Group Collapsable="true" Collapsed="false" DisplayName="Main Contact">
            <sm:Field Container="DetailsMainContact" Name="CompanyName"/>
            <sm:Field Container="DetailsMainContact" Name="Email"/>
            <sm:Field Container="DetailsMainContact" Name="Web"/>
            <sm:Field Container="DetailsMainContact" Name="Phone1Type"/>
            <sm:Field Container="DetailsMainContact" Name="Phone1"/>
            <sm:Field Container="DetailsMainContact" Name="Phone2Type"/>
            <sm:Field Container="DetailsMainContact" Name="Phone2"/>
            <sm:Field Container="DetailsMainContact" Name="FaxType"/>
            <sm:Field Container="DetailsMainContact" Name="Fax"/>
        </sm:Group>

        <sm:Group Collapsable="true" Collapsed="false" DisplayName="Main Address">
            <sm:Field Container="DetailsMainAddress" Name="AddressLine1"/>
            <sm:Field Container="DetailsMainAddress" Name="AddressLine2"/>
            <sm:Field Container="DetailsMainAddress" Name="City"/>
            <sm:Field Container="DetailsMainAddress" Name="Country"/>
            <sm:Field Container="DetailsMainAddress" Name="State"/>
            <sm:Field Container="DetailsMainAddress" Name="PostalCode"/>
        </sm:Group>

        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Shipping Contact">
            <sm:Field Container="DeliverySettingsShippingContactSameAsMain" Name="SameAsMain"/>
            <sm:Field Container="DeliverySettingsShippingContact" Name="Email"/>
            <sm:Field Container="DeliverySettingsShippingContact" Name="Phone1"/>
            <sm:Field Container="DeliverySettingsShippingContact" Name="Phone2"/>
            <sm:Field Container="DeliverySettingsShippingContact" Name="Fax"/>
        </sm:Group>

        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Shipping Address">
            <sm:Field Container="DeliverySettingsShippingAddressSameAsMain" Name="SameAsMain"/>
            <sm:Field Container="DeliverySettingsShippingAddress" Name="AddressLine1"/>
            <sm:Field Container="DeliverySettingsShippingAddress" Name="AddressLine2"/>
            <sm:Field Container="DeliverySettingsShippingAddress" Name="City"/>
            <sm:Field Container="DeliverySettingsShippingAddress" Name="Country"/>
            <sm:Field Container="DeliverySettingsShippingAddress" Name="State"/>
            <sm:Field Container="DeliverySettingsShippingAddress" Name="PostalCode"/>
        </sm:Group>

        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Default Location Settings">
            <sm:Field Container="DeliverySettingsDefaultLocationSettings" Name="LocationName"/>
            <sm:Field Container="DeliverySettingsDefaultLocationSettings" Name="TaxRegistrationID"/>
            <sm:Field Container="DeliverySettingsDefaultLocationSettings" Name="TaxZone"/>
            <sm:Field Container="DeliverySettingsDefaultLocationSettings" Name="ShippingBranch"/>
            <sm:Field Container="DeliverySettingsDefaultLocationSettings" Name="PriceClass"/>
        </sm:Group>

        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Shipping Instructions">
            <sm:Field Container="DeliverySettingsShippingInstructions" Name="Warehouse"/>
            <sm:Field Container="DeliverySettingsShippingInstructions" Name="ShipVia"/>
            <sm:Field Container="DeliverySettingsShippingInstructions" Name="ShippingTerms"/>
            <sm:Field Container="DeliverySettingsShippingInstructions" Name="ShippingZone"/>
            <sm:Field Container="DeliverySettingsShippingInstructions" Name="FOBPoint"/>
            <sm:Field Container="DeliverySettingsShippingInstructions" Name="ResidentialDelivery"/>
            <sm:Field Container="DeliverySettingsShippingInstructions" Name="SaturdayDelivery"/>
            <sm:Field Container="DeliverySettingsShippingInstructions" Name="Insurance"/>
            <sm:Field Container="DeliverySettingsShippingInstructions" Name="ShippingRule"/>
            <sm:Field Container="DeliverySettingsShippingInstructions" Name="OrderPriority"/>
            <sm:Field Container="DeliverySettingsShippingInstructions" Name="LeadTimeDays"/>
        </sm:Group>

        <sm:Group Collapsable="true" Collapsed="true" DisplayName="CRM">
            <sm:Field Container="DetailsCRM" Name="ClassID"/>
            <sm:Field Container="DetailsCRM" Name="AccountRef"/>
            <sm:Field Container="DetailsCRM" Name="ParentAccount"/>
            <sm:Field Container="DetailsCRM" Name="LastIncomingActivity"/>
            <sm:Field Container="DetailsCRM" Name="LastOutgoingActivity"/>
        </sm:Group>

        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Attributes">
            <sm:Attributes From="Attributes"/>
        </sm:Group>

        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Owner Information">
            <sm:Field Name="Workgroup"/>
            <sm:Field Name="Owner">
                <sm:SelectorContainer FieldsToShow="1">
                    <sm:Field Name="EmployeeName"/>
                    <sm:Field Name="ExtKey" ForceIsVisible="false"/>
                </sm:SelectorContainer>
            </sm:Field>
        </sm:Group>

        <sm:Action Behavior="Save" Context="Record" Name="Save"/>
        <sm:Action Behavior="Cancel" Context="Record" Name="Cancel"/>
    </sm:Container>

    <sm:Container Name="Activities" FieldsToShow="3" ContainerActionsToExpand="0" ListActionsToExpand="1">
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

    <sm:Container Name="Contacts" FieldsToShow="4" ContainerActionsToExpand="1" ListActionsToExpand="1">
        <sm:Field Name="DisplayName_"/>
        <sm:Field Name="Position"/>
        <sm:Field Name="Email"/>
        <sm:Field Name="Phone1"/>

        <sm:Action Behavior="Create" Context="Container" Name="AddContact" Redirect="true" Icon="system://Plus"/>
        <sm:Action Behavior="Open" Context="Container" Name="ContactsViewDetails" Redirect="true"/>
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
        <sm:Action Behavior="Create" Context="Container" Name="AddOpportunity" Redirect="true" Icon="system://Plus"/>
    </sm:Container>

    <sm:Container Name="Cases" FieldsToShow="5" ContainerActionsToExpand="1" ListActionsToExpand="1">
        <sm:Field Name="CaseID"/>
        <sm:Field Name="Status"/>
        <sm:Field Name="ClassID"/>
        <sm:Field Name="Reason"/>
        <sm:Field Name="Subject"/>

        <sm:Action Behavior="Open" Context="Container" Name="CasesViewDetails" Redirect="true"/>
        <sm:Action Behavior="Create" Context="Container" Name="AddCase" Redirect="true" Icon="system://Plus"/>
    </sm:Container>

    <sm:Container Name="Contracts" FieldsToShow="4" ContainerActionsToExpand="1" ListActionsToExpand="1">
        <sm:Field Name="ContractID" ForceIsDisabled="true"/>
        <sm:Field Name="Status" ForceIsDisabled="true"/>
        <sm:Field Name="Description" ForceIsDisabled="true"/>
        <sm:Field Name="ExpirationDate" ForceIsDisabled="true"/>

        <!--<sm:Action Behavior="Open" Context="Container" Name="ContractsViewDetails" Redirect="true"/>-->
    </sm:Container>

    <sm:Container Name="Orders" FieldsToShow="6" ContainerActionsToExpand="1" ListActionsToExpand="1">
        <sm:Field Name="OrderNbr"/>
        <sm:Field Name="OrderTotal"/>
        <sm:Field Name="RequestedOn"/>
        <sm:Field Name="Currency"/>
        <sm:Field Name="Status"/>
        <sm:Field Name="CustomerOrder"/>

        <sm:Action Behavior="Open" Context="Container" Name="OrdersViewDetails" Redirect="true" RedirectToContainer="OrderSummary$Form"/>
    </sm:Container>

    <sm:Container Name="Campaigns" FieldsToShow="2" ContainerActionsToExpand="1" ListActionsToExpand="1">
        <sm:Field Name="CampaignID"/>
        <sm:Field Name="Status"/>

        <!--<sm:Action Behavior="Open" Context="Container" Name="MembersCRCampaignViewDetails" Redirect="true"/>-->
    </sm:Container>

    <sm:Container Name="MarketingLists" FieldsToShow="2" ContainerActionsToExpand="1" ListActionsToExpand="1">
        <sm:Field Name="MarketingListID" ForceIsVisible="false" />
        <sm:Field Name="MarketingListIDCRMarketingListMailListCode"/>
        <sm:Field Name="Name"/>
        <sm:Field Name="Activated"/>
        <!--<sm:Action Behavior="Open" Context="Container" Name="SubscriptionsCRMarketingListViewDetails" Redirect="true"/>-->
    </sm:Container>
</sm:Screen>
