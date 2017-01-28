<?xml version="1.0" encoding="UTF-8"?>
<sm:Screen DisplayName="Contacts" Id="CR3020PL" Type="SimpleScreen" xmlns:sm="http://acumatica.com/mobilesitemap" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    
    <sm:Container Name="Result" FieldsToShow="5" ContainerActionsToExpand="1" ListActionsToExpand="1">
        <sm:Field Name="DisplayName_"/>
        <sm:Field Name="BusinessAccount"/>
        <sm:Field Name="Email"/>
        <sm:Field Name="ClassID"/>
        <sm:Field Name="CompanyName"/>
        
        <sm:Action Behavior="Open" Context="Container" Name="editDetail" Redirect="true"/>
        <sm:Action Behavior="Create" Context="Container" Icon="system://Plus" Name="insert" Redirect="true"/>
    </sm:Container>
    
</sm:Screen>
