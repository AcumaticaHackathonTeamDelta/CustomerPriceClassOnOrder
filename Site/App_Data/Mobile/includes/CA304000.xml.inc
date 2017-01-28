<?xml version="1.0" encoding="UTF-8"?>
<sm:Screen DisplayName="Cash Transactions" Id="CA304000" Type="SimpleScreen" ExpandSelector="TranType" Icon="system://Credit" xmlns:sm="http://acumatica.com/mobilesitemap" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    
    <sm:Container Name="TransactionSummary" FieldsToShow="6" ContainerActionsToExpand="1" ListActionsToExpand="0" FormActionsToExpand="0">
        <sm:Attachments Disabled="true"/>
        
		<sm:Field Name="ReferenceNbr" ListPriority="99"/>
		<sm:Field Name="Status" ListPriority="94"/>
		<sm:Field Name="Hold"/>
		<sm:Field Name="CashAccount"/>
		<sm:Field Name="EntryType"/>
		<sm:Field Name="DisbReceipt"/>
		<sm:Field Name="TranDate"/>
		<sm:Field Name="FinPeriod"/>
		<sm:Field Name="DocumentRef" ListPriority="97"/>
		<sm:Field Name="Amount" ListPriority="98"/>
		<sm:Field Name="ControlTotal"/>
		<sm:Field Name="TaxAmount"/>
		<sm:Field Name="Currency" ListPriority="96"/>
		<sm:Field Name="Description" ListPriority="95"/>
        
        <sm:Group Collapsable="true" Collapsed="true" DisplayName="Notes">
            <sm:Field Name="NoteText" TextType="PlainMultiLine"/>
        </sm:Group>
        
        <sm:Action Behavior="Create" Context="Container" Name="Insert" Icon="system://Plus"/>
        <sm:Action Behavior="Delete" Context="Selection" Name="Delete" Icon="system://Trash"/>
        <sm:Action Behavior="Void" Context="Selection" Name="ReleaseAction" SyncLongOperation="true"/>
        
        <sm:Action Behavior="Save" Context="Record" Name="Save"/>
        <sm:Action Behavior="Cancel" Context="Record" Name="Cancel"/>
        
        <sm:Action Behavior="Record" Context="Record" Name="ApproveAction" SyncLongOperation="true"/>
        <sm:Action Behavior="Record" Context="Record" Name="RejectAction" SyncLongOperation="true"/>
        <sm:Action Behavior="Record" Context="Record" Name="ReleaseAction" SyncLongOperation="true"/>
    </sm:Container>
    
	<sm:Container Name="TransactionDetails" FieldsToShow="2" ContainerActionsToExpand="1" ListActionsToExpand="1">
        <sm:Attachments Disabled="true"/>
        
        <sm:Field Name="Branch"/>
		<sm:Field Name="Description" ListPriority="99"/>
		<sm:Field Name="Amount" ListPriority="98"/>
		<sm:Field Name="OffsetAccount"/>
        <sm:Field Name="OffsetSubaccount" SelectorDisplayFormat="Key"/>
		<sm:Field Name="Project"/>
		<sm:Field Name="ProjectTask"/>
		<sm:Field Name="NonBillable"/>
        <sm:Field Name="TaxCategory"/>
        
        <sm:Action Behavior="Create" Context="Container" Name="Insert" Icon="system://Plus"/>
        <sm:Action Behavior="Delete" Context="Selection" Name="Delete" Icon="system://Trash"/>
	</sm:Container>
    
	<sm:Container Name="TaxDetails" FieldsToShow="2" ContainerActionsToExpand="1" ListActionsToExpand="1">
        <sm:Attachments Disabled="true"/>
        
		<sm:Field Name="TaxID" ListPriority="99"/>
		<sm:Field Name="TaxRate" ListPriority="98"/>
		<sm:Field Name="TaxableAmount"/>
		<sm:Field Name="TaxAmount"/>
        
        <sm:Action Behavior="Create" Context="Container" Name="Insert" Icon="system://Plus"/>
        <sm:Action Behavior="Delete" Context="Selection" Name="Delete" Icon="system://Trash"/>
	</sm:Container>
    
	<sm:Container Name="ApprovalDetails" FieldsToShow="4">
        <sm:Attachments Disabled="true"/>
        
		<sm:Field Name="ApproverName" ListPriority="99"/>
		<sm:Field Name="ApprovedByName"/>
		<sm:Field Name="Date" ListPriority="96"/>
		<sm:Field Name="Status" ListPriority="97"/>
		<sm:Field Name="Workgroup" ListPriority="98"/>
	</sm:Container>
	
</sm:Screen>
