<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="PM301000.aspx.cs" Inherits="Page_PM301000"
    Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.PM.ProjectEntry" PrimaryView="Project" BorderStyle="NotSet">
        <CallbackCommands>
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
            <px:PXDSCallbackCommand Name="Insert" PostData="Self" />
            <px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="true" />
            <px:PXDSCallbackCommand Name="Last" PostData="Self" />
			<px:PXDSCallbackCommand StartNewGroup="True" Name="Action" />
	        <px:PXDSCallbackCommand Name="Bill" Visible="False" CommitChanges="True"/>
			 <px:PXDSCallbackCommand Name="CreateTemplate" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="AutoBudget" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="Hold" Visible="False" CommitChanges="True" />

            <px:PXDSCallbackCommand DependOnGrid="TaskGrid" Name="ViewTask" Visible="False" CommitChanges="True"/>
            <px:PXDSCallbackCommand DependOnGrid="ProjectStatusGrid" Name="ViewBalance" Visible="False" />
            <px:PXDSCallbackCommand Name="ViewInvoice" Visible="False" DependOnGrid="InvoicesGrid" />
            
            <px:PXDSCallbackCommand Name="AddTasks" Visible="False" CommitChanges="True" PostData="Page"></px:PXDSCallbackCommand>
            <px:PXDSCallbackCommand Name="NewTask" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="NewEvent" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="NewActivity" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="NewMailActivity" Visible="False" CommitChanges="True" PopupCommand="Cancel" PopupCommandTarget="ds" />
           
            
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXSmartPanel ID="PanelTemplateSettings" runat="server" AcceptButtonID="PXButtonOK" AutoReload="true" CancelButtonID="PXButtonCancel" Caption="New Project Template"
        CaptionVisible="True" AutoSaveChanges="True" HideAfterAction="true" Key="TemplateSettings" LoadOnDemand="true">
        <px:PXFormView ID="formTemplateSettings" runat="server" CaptionVisible="False" DataMember="TemplateSettings" DataSourceID="ds" Style="z-index: 100" Width="100%"
            DefaultControlID="edTemplateID">
            <ContentStyle BackColor="Transparent" BorderStyle="None" />
            <Template>
                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                <px:PXTextEdit ID="edTemplateID" runat="server" DataField="TemplateID" CommitChanges="True" />
            </Template>
        </px:PXFormView>
        <px:PXPanel ID="PXPanel1" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButtonOK" runat="server" DialogResult="OK" Text="OK" />
            <px:PXButton ID="PXButtonCancel" runat="server" DialogResult="Cancel" Text="Cancel" />
        </px:PXPanel>
    </px:PXSmartPanel>
    <px:PXSmartPanel ID="pnlChangeID" runat="server"  Caption="Specify New ID"
        CaptionVisible="true" DesignView="Hidden" LoadOnDemand="true" Key="ChangeIDDialog" CreateOnDemand="false" AutoCallBack-Enabled="true"
        AutoCallBack-Target="formChangeID" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True" CallBackMode-PostData="Page"
        AcceptButtonID="btnOK">
            <px:PXFormView ID="formChangeID" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" CaptionVisible="False"
                DataMember="ChangeIDDialog">
                <ContentStyle BackColor="Transparent" BorderStyle="None" />
                <Template>
                    <px:PXLayoutRule ID="rlAcctCD" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                    <px:PXSegmentMask ID="edAcctCD" runat="server" DataField="CD" />
                </Template>
            </px:PXFormView>
            <px:PXPanel ID="pnlChangeIDButton" runat="server" SkinID="Buttons">
                <px:PXButton ID="btnOK" runat="server" DialogResult="OK" Text="OK" >
                    <AutoCallBack Target="formChangeID" Command="Save" />
                </px:PXButton>
				<px:PXButton ID="btnCancel" runat="server" DialogResult="Cancel" Text="Cancel" />						
            </px:PXPanel>
    </px:PXSmartPanel>
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Project" Caption="Project Summary" FilesIndicator="True"
        NoteIndicator="True" NotifyIndicator="true" LinkPage="">
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
            <px:PXSegmentMask ID="edContractCD" runat="server" DataField="ContractCD" DataSourceID="ds">
                <GridProperties FastFilterFields="Description" />
            </px:PXSegmentMask>
            <px:PXSegmentMask CommitChanges="True" ID="edCustomerID" runat="server" DataField="CustomerID" DataSourceID="ds" />
            <px:PXSegmentMask CommitChanges="True" ID="edTemplateID" runat="server" DataField="TemplateID" DataSourceID="ds" />
            <px:PXLayoutRule runat="server" ColumnSpan="2" />
            <px:PXTextEdit ID="edDescription" runat="server" DataField="Description" />
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="S" />
			 <px:PXCheckBox ID="chkHold" runat="server" DataField="Hold">
                <AutoCallBack Command="Hold" Target="ds">
                </AutoCallBack>
            </px:PXCheckBox>
            <px:PXDropDown CommitChanges="True" ID="edStatus" runat="server" DataField="Status" />
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
            <px:PXNumberEdit ID="edAsset" runat="server" DataField="Asset" Enabled="False" />
            <px:PXNumberEdit ID="edLiability" runat="server" DataField="Liability" Enabled="False" />
            <px:PXNumberEdit ID="edIncome" runat="server" DataField="Income" Enabled="False" />
            <px:PXNumberEdit ID="edExpense" runat="server" DataField="Expense" Enabled="False" />
        </Template>
    </px:PXFormView>
    <px:PXSmartPanel ID="PanelAddTasks" runat="server" Height="396px" Width="873px" Caption="Add Tasks" CaptionVisible="True" Key="TasksForAddition" AutoCallBack-Command="Refresh"
        AutoCallBack-Enabled="True" AutoCallBack-Target="gridAddTasks">
        <px:PXGrid ID="gridAddTasks" runat="server" Height="240px" Width="100%" DataSourceID="ds" SkinID="Inquire" NoteIndicator="false" FilesIndicator="false">
            <AutoSize Enabled="true" />
            <Levels>
                <px:PXGridLevel DataMember="TasksForAddition" >
                    <Columns>
                        <px:PXGridColumn AllowCheckAll="True" DataField="Selected" Label="Selected" TextAlign="Center" Type="CheckBox" Width="60px" />
                        <px:PXGridColumn DataField="TaskCD" Label="Task ID" />
                        <px:PXGridColumn DataField="Description" Label="Description" Width="200px" />
                        <px:PXGridColumn DataField="ApproverID" />
                        <px:PXGridColumn DataField="PMProject__NonProject" Label="IS Global" TextAlign="Center" Type="CheckBox" Width="60px" />
                    </Columns>
                </px:PXGridLevel>
            </Levels>
            <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
        </px:PXGrid>
        <px:PXPanel ID="PXPanelBtn" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton2" runat="server" DialogResult="OK" Text="Add " CommandName="AddTasks" CommandSourceID="ds" />
            <px:PXButton ID="PXButton3" runat="server" DialogResult="Cancel" Text="Cancel" />
        </px:PXPanel>
    </px:PXSmartPanel>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXTab ID="tab" runat="server" Width="100%" Height="511px" DataSourceID="ds" DataMember="ProjectProperties">
        <Activity HighlightColor="" SelectedColor="" Width="" Height=""></Activity>
        <Items>
            <px:PXTabItem Text="General Info">
                <Template>
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                    <px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="General Settings" />
                    <px:PXDateTimeEdit ID="edStartDate0" runat="server" DataField="StartDate" />
                    <px:PXDateTimeEdit ID="edExpireDate" runat="server" DataField="ExpireDate" />
                    <px:PXSelector ID="edApprover" runat="server" DataField="ApproverID" />

                    <px:PXCheckBox ID="chkRestrictToEmployeeList" runat="server" DataField="RestrictToEmployeeList" />
                    <px:PXCheckBox ID="chkRestrictToResourceList" runat="server" DataField="RestrictToResourceList" />

                    <px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartGroup="True" GroupCaption="Visibility Settings" />
                    <px:PXLayoutRule ID="PXLayoutRule2" runat="server" Merge="True" />
                    <px:PXCheckBox ID="chkVisibleInGL" runat="server" DataField="VisibleInGL" />
                    <px:PXCheckBox ID="chkVisibleInAP" runat="server" DataField="VisibleInAP" />
                    <px:PXCheckBox ID="chkVisibleInAR" runat="server" DataField="VisibleInAR" />
                    <px:PXCheckBox ID="chkVisibleInSO" runat="server" DataField="VisibleInSO" />
                    <px:PXCheckBox ID="chkVisibleInPO" runat="server" DataField="VisibleInPO" />
                    <px:PXLayoutRule ID="PXLayoutRule5" runat="server" />
                    <px:PXLayoutRule ID="PXLayoutRule4" runat="server" Merge="True" />
                    <px:PXCheckBox ID="chkVisibleInEP" runat="server" DataField="VisibleInEP" />
                    <px:PXCheckBox ID="chkVisibleInIN" runat="server" DataField="VisibleInIN" />
                    <px:PXCheckBox ID="chkVisibleInCA" runat="server" DataField="VisibleInCA" />
                    <px:PXCheckBox ID="chkVisibleInCR" runat="server" DataField="VisibleInCR" />
                    <px:PXLayoutRule ID="PXLayoutRule3" runat="server" />
                    
                    <px:PXLayoutRule ID="PXLayoutRule6" runat="server" StartGroup="True" GroupCaption="Defaults" />
                     <px:PXSegmentMask ID="edDefaultAccountID" runat="server" DataField="DefaultAccountID" />
                    <px:PXSegmentMask ID="edDefaultSubID" runat="server" DataField="DefaultSubID" />
                    <px:PXSegmentMask ID="edDefaultAccrualAccountID" runat="server" DataField="DefaultAccrualAccountID"/>
                    <px:PXSegmentMask ID="edDefaultAccrualSubID" runat="server" DataField="DefaultAccrualSubID" />

                    <px:PXLayoutRule runat="server" StartColumn="True" GroupCaption="Billing Settings" StartGroup="True" LabelsWidth="SM" ControlSize="XM" />
                    <px:PXFormView ID="billingForm" runat="server" DataMember="Billing" DataSourceID="ds" RenderStyle="Simple">
                        <Template>
                            <px:PXLayoutRule ID="PXLayoutRule7" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                            <px:PXDropDown CommitChanges="True" ID="edType" runat="server" DataField="Type" Size="S" />
                            <px:PXDateTimeEdit ID="edNextDate" runat="server" DataField="NextDate" />
                            <px:PXDateTimeEdit ID="edLastDate" runat="server" DataField="LastDate" />
                        </Template>
                    </px:PXFormView>
                    <px:PXSelector ID="edLocation" runat="server" DataField="LocationID"/>
                    <px:PXSelector ID="edAllocationID" runat="server" DataField="AllocationID" AllowEdit="True" AllowAddNew="True" />
                    <px:PXSelector ID="edBillingID" runat="server" DataField="BillingID" AllowEdit="True" AllowAddNew="True" />
                    <px:PXSelector ID="edRateTable" runat="server" DataField="RateTableID" AllowEdit="True" AllowAddNew="True" />
                    <px:PXCheckBox ID="chkAutoAllocate" runat="server" DataField="AutoAllocate" />
                    <px:PXCheckBox ID="chkAutomaticReleaseAR" runat="server" DataField="AutomaticReleaseAR" />
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Tasks">
                <Template>
                    <px:PXGrid runat="server" ID="TaskGrid" Width="100%" DataSourceID="ds" Height="100%" SkinID="DetailsInTab">
                        <Levels>
                            <px:PXGridLevel DataMember="Tasks" DataKeyNames="ProjectID,TaskCD">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                                    <px:PXSegmentMask ID="edProjectTaskCD" runat="server" DataField="TaskCD" />
                                    <px:PXSegmentMask Size="s" ID="edDefaultSubID" runat="server" DataField="DefaultSubID" />
                                    <px:PXSelector ID="edRateTableID" runat="server" DataField="RateTableID" />
                                    <px:PXSelector Size="s" ID="edAllocationID" runat="server" DataField="AllocationID" AllowAddNew="True" AllowEdit="True" />
                                    <px:PXSelector Size="s" ID="edBillingID" runat="server" DataField="BillingID" AllowAddNew="True" AllowEdit="True" />
                                    <px:PXDropDown Size="m" ID="edBillingOption" runat="server" DataField="BillingOption" />
                                    <px:PXSegmentMask ID="edDefaultAccountID" runat="server" DataField="DefaultAccountID"  />
                                    <px:PXTextEdit ID="edDescription" runat="server" DataField="Description" />
                                    <px:PXSegmentMask ID="edLocationID" runat="server" DataField="LocationID" />
                                    <px:PXDropDown ID="edStatus" runat="server" DataField="Status" Enabled="False" />
                                    <px:PXDateTimeEdit ID="edPlannedStartDate" runat="server" DataField="PlannedStartDate" />
                                    <px:PXDateTimeEdit ID="edPlannedEndDate" runat="server" DataField="PlannedEndDate" />
                                    <px:PXDateTimeEdit ID="edStartDate" runat="server" DataField="StartDate" />
                                    <px:PXDateTimeEdit ID="edEndDate" runat="server" DataField="EndDate" />
                                    <px:PXSelector ID="edApproverID" runat="server" DataField="ApproverID" AutoRefresh="True" />
                                    <px:PXSelector ID="edTaxCategoryID" runat="server" DataField="TaxCategoryID" AutoRefresh="True"/>
                                    <px:PXCheckBox ID="chkVisibleInGL" runat="server" Checked="True" DataField="VisibleInGL" />
                                    <px:PXCheckBox ID="chkVisibleInAP" runat="server" Checked="True" DataField="VisibleInAP" />
                                    <px:PXCheckBox ID="chkVisibleInAR" runat="server" Checked="True" DataField="VisibleInAR" />
                                    <px:PXCheckBox ID="chkVisibleInSO" runat="server" Checked="True" DataField="VisibleInSO" />
                                    <px:PXCheckBox ID="chkVisibleInPO" runat="server" Checked="True" DataField="VisibleInPO" />
                                    <px:PXCheckBox ID="chkVisibleInEP" runat="server" Checked="True" DataField="VisibleInEP" />
                                    <px:PXCheckBox ID="chkVisibleInIN" runat="server" Checked="True" DataField="VisibleInIN" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="TaskCD" Label="Task ID" Width="81px" AutoCallBack="True"/>
                                    <px:PXGridColumn DataField="Description" Label="Description" Width="200px" />
                                    <px:PXGridColumn DataField="LocationID" Label="Location" Width="54px" AutoCallBack="True" />
                                    <px:PXGridColumn DataField="RateTableID" Label="RateTable" Width="93px" />
                                    <px:PXGridColumn DataField="AllocationID" Label="Allocation Rule" Width="117px" />
                                    <px:PXGridColumn DataField="BillingID" Label="Billing Rule" Width="117px" />
                                    <px:PXGridColumn DataField="Status" Label="Status" RenderEditorText="True" Width="81px" AutoCallBack="True" />
                                    <px:PXGridColumn DataField="CompletedPct" Label="Completed (%)" TextAlign="Right" />
                                    <px:PXGridColumn DataField="PlannedStartDate" Label="Planned Start Date" Width="90px" SortDirection="Ascending" />
                                    <px:PXGridColumn DataField="PlannedEndDate" Label="Planned End Date" Width="90px" />
                                    <px:PXGridColumn DataField="StartDate" Label="Start Date" Width="90px" />
                                    <px:PXGridColumn DataField="EndDate" Label="End Date" Width="90px" />
                                    <px:PXGridColumn DataField="ApproverID" Width="108px" />
                                    <px:PXGridColumn DataField="BillingOption" Label="Billing Option" RenderEditorText="True" Width="144px" />
                                    <px:PXGridColumn DataField="DefaultAccountID" Width="108px" />
                                    <px:PXGridColumn DataField="DefaultSubID" Label="Default Sub." Width="108px" />
                                    <px:PXGridColumn DataField="PMTaskTotal__Asset" Label="Asset" Width="63px" />
                                    <px:PXGridColumn DataField="PMTaskTotal__Liability" Label="Liability" Width="63px" />
                                    <px:PXGridColumn DataField="PMTaskTotal__Income" Label="Income" Width="63px" />
                                    <px:PXGridColumn DataField="PMTaskTotal__Expense" Label="Expense" Width="63px" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" />
                        <ActionBar>
                            <CustomItems>
                                <px:PXToolBarButton Text="Task Details" Key="cmdViewTask">
                                    <AutoCallBack Command="ViewTask" Target="ds" />
                                    <PopupCommand Command="Refresh" Target="TaskGrid" />
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Text="Add Common Tasks" PopupPanel="PanelAddTasks" />
                            </CustomItems>
                        </ActionBar>
                        <Mode InitNewRow="True" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Balances">
                <Template>
                    <px:PXGrid runat="server" ID="ProjectStatusGrid" Width="100%" DataSourceID="ds" Height="100%" SkinID="DetailsInTab" AdjustPageSize="Auto" AllowPaging="False" AllowSearch="False"
                        AllowFilter="False" OnRowDataBound="ProjectStatusGrid_RowDataBound">
                        <Levels>
                            <px:PXGridLevel DataMember="BalanceRecords" DataKeyNames="RecordID">
                                <Columns>
                                    <px:PXGridColumn DataField="AccountGroup" Label="Account Group" Width="108px" />
                                    <px:PXGridColumn DataField="Description" Label="Description" Width="500px" />
                                    <px:PXGridColumn DataField="Amount" Label="Amount" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn DataField="RevisedAmount" Label="Revised Amount" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn DataField="ActualAmount" Label="Actual Amount" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn DataField="Performance" TextAlign="Right" Width="100px" />
                                </Columns>
                                <Mode AllowAddNew="False" AllowColMoving="False" AllowDelete="False" AllowFormEdit="False" AllowSort="False" AllowUpdate="False" />
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" />
                        <ActionBar DefaultAction="cmdViewBalance">
                            <Actions>
                                <AddNew ToolBarVisible="False" />
                                <Delete ToolBarVisible="False" />
                            </Actions>
                            <CustomItems>
                                <px:PXToolBarButton Text="Balance Details" Key="cmdViewBalance" Visible="False">
                                    <AutoCallBack Command="ViewBalance" Target="ds" />
                                </px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
                        <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Employees">
                <Template>
                    <px:PXSplitContainer runat="server" ID="sp1" SplitterPosition="200" SkinID="Horizontal" Height="400px">
                        <AutoSize Enabled="true" />
                        <Template1>
                            <px:PXGrid ID="gridEmployeeContract" runat="server" DataSourceID="ds" MatrixMode="False" SyncPosition="True" Height="400px" Width="100%"
                                SkinID="DetailsInTab">
                                <Levels>
                                    <px:PXGridLevel DataMember="EmployeeContract">
	                                     <RowTemplate>
		                                     <px:PXSegmentMask ID="edEmployeeID" runat="server" DataField="EmployeeID"/>
			                            </RowTemplate>
                                        <Columns>
                                            <px:PXGridColumn DataField="EmployeeID" Width="150px" AutoCallBack="True" MatrixMode="false" />
                                            <px:PXGridColumn DataField="EPEmployee__AcctName" Label="Employee Name" Width="216px" />
                                            <px:PXGridColumn DataField="EPEmployee__DepartmentID" Label="Department ID" Width="90px" />
                                        </Columns>
                                    </px:PXGridLevel>
                                </Levels>
                                <AutoCallBack Target="gridContractRates" Command="Refresh" />
                                <Mode InitNewRow="True" />
                                <AutoSize Enabled="True" />
                            </px:PXGrid>
                        </Template1>
                        <Template2>
                            <px:PXGrid ID="gridContractRates" runat="server" DataSourceID="ds" Height="400px" Width="100%"
                                SkinID="DetailsInTab" Caption="Overrides" AllowPaging="False">
                                <CallbackCommands>
                                    <Refresh SelectControlsIDs="gridEmployeeContract" />
                                </CallbackCommands>
                                <Levels>
                                    <px:PXGridLevel DataMember="ContractRates" DataKeyNames="RecordID">
	                                    <Columns>
                                            <px:PXGridColumn DataField="EarningType" Width="110px" CommitChanges="True" />
                                            <px:PXGridColumn DataField="EPEarningType__Description" Width="110px" />
                                            <px:PXGridColumn DataField="LabourItemID" Width="150px" Label="Labor Item" />
                                            <px:PXGridColumn DataField="InventoryItem__BasePrice" Width="200px" />
                                        </Columns>
                                        <Layout FormViewHeight="" />
                                    </px:PXGridLevel>
                                </Levels>
                                <AutoSize Enabled="True" />
                                <Mode InitNewRow="True" />
                            </px:PXGrid>
                        </Template2>
                    </px:PXSplitContainer>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Equipment">
                <Template>
                    <px:PXGrid runat="server" ID="ProjectRatesGrid" Width="100%" DataSourceID="ds" Height="100%" SkinID="DetailsInTab" AdjustPageSize="Auto" AllowPaging="True">
                        <Levels>
                            <px:PXGridLevel DataMember="EquipmentRates" DataKeyNames="EquipmentID,ProjectID">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                                    <px:PXSelector ID="edEquipmentID" runat="server" DataField="EquipmentID" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="IsActive" TextAlign="Center" Type="CheckBox" Width="60px" />
                                    <px:PXGridColumn DataField="EquipmentID" Width="108px" AutoCallBack="True" />
                                    <px:PXGridColumn DataField="EPEquipment__Description" Width="200px" />
                                    <px:PXGridColumn DataField="EPEquipment__RunRateItemID" Width="90px" />
                                    <px:PXGridColumn DataField="RunRate" Width="90px" />
                                    <px:PXGridColumn DataField="EPEquipment__SetupRateItemID" Width="90px" />
                                    <px:PXGridColumn DataField="SetupRate" Width="90px" />
                                    <px:PXGridColumn DataField="EPEquipment__SuspendRateItemID" Width="105px" />
                                    <px:PXGridColumn DataField="SuspendRate" Width="90px" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Activity History" LoadOnDemand="true">
                <Template>
                    <pxa:PXGridWithPreview ID="gridActivities" runat="server" DataSourceID="ds" Width="100%" AllowSearch="True" DataMember="Activities" AllowPaging="true" NoteField="NoteText"
                        FilesField="NoteFiles" BorderWidth="0px" GridSkinID="Details" SplitterStyle="z-index: 100; border-top: solid 1px Gray;  border-bottom: solid 1px Gray" PreviewPanelStyle="z-index: 100; background-color: Window"
                        PreviewPanelSkinID="Preview" BlankFilterHeader="All Activities" MatrixMode="true" PrimaryViewControlID="form">
                        <ActionBar DefaultAction="cmdViewActivity" PagerVisible="False">
                            <Actions>
                                <AddNew Enabled="False" />
                                <Delete Enabled="False" />
                            </Actions>
                            <CustomItems>
                                <px:PXToolBarButton Key="cmdAddTask">
                                    <AutoCallBack Command="NewTask" Target="ds" />
                                    <PopupCommand Command="Cancel" Target="ds" />
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Key="cmdAddEvent">
                                    <AutoCallBack Command="NewEvent" Target="ds" />
                                    <PopupCommand Command="Cancel" Target="ds" />
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Key="cmdAddEmail">
                                    <AutoCallBack Command="NewMailActivity" Target="ds" />
                                    <PopupCommand Command="Cancel" Target="ds" />
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Key="cmdAddActivity">
                                    <AutoCallBack Command="NewActivity" Target="ds" />
                                    <PopupCommand Command="Cancel" Target="ds" />
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Key="cmdViewActivity" Visible="false">
                                    <ActionBar MenuVisible="false" />
                                    <AutoCallBack Command="ViewActivity" Target="ds" />
                                </px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
                        <Levels>
                            <px:PXGridLevel DataMember="Activities">
                                <RowTemplate>
                					<px:PXTimeSpan TimeMode="True" ID="edTimeSpent" runat="server" DataField="TimeSpent" InputMask="hh:mm" MaxHours="99" />
                					<px:PXTimeSpan TimeMode="True" ID="edOvertimeSpent" runat="server" DataField="OvertimeSpent" InputMask="hh:mm" MaxHours="99" />
                					<px:PXTimeSpan TimeMode="True" ID="edTimeBillable" runat="server" DataField="TimeBillable" InputMask="hh:mm" MaxHours="99" />
                					<px:PXTimeSpan TimeMode="True" ID="edOvertimeBillable" runat="server" DataField="OvertimeBillable" InputMask="hh:mm" MaxHours="99" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="IsCompleteIcon" Width="21px" AllowShowHide="False" ForceExport="True" />
                                    <px:PXGridColumn DataField="PriorityIcon" Width="21px" AllowShowHide="False" AllowResize="False" ForceExport="True" />
                                    <px:PXGridColumn DataField="ClassIcon" Width="31px" AllowShowHide="False" ForceExport="True" />
                                    <px:PXGridColumn DataField="ClassInfo" />
                                    <px:PXGridColumn DataField="RefNoteID" Visible="false" AllowShowHide="False" />
                                    <px:PXGridColumn DataField="Subject" LinkCommand="ViewActivity" Width="297px" />
                                    <px:PXGridColumn DataField="UIStatus" />
                                    <px:PXGridColumn DataField="Released" Width="80px" />
                                    <px:PXGridColumn DataField="StartDate" Width="108px" />
                                    <px:PXGridColumn DataField="CategoryID" />
                                    <px:PXGridColumn DataField="IsBillable" TextAlign="Center" Type="CheckBox" Width="60px" />
                                    <px:PXGridColumn DataField="TimeSpent" Width="100px" RenderEditorText="True" />
                                    <px:PXGridColumn DataField="OvertimeSpent" Width="100px" RenderEditorText="True" />
                                    <px:PXGridColumn DataField="TimeBillable" Width="100px" RenderEditorText="True" />
                                    <px:PXGridColumn DataField="OvertimeBillable" Width="100px" RenderEditorText="True" />
                                    <px:PXGridColumn DataField="CreatedByID_Creator_Username" Visible="false" SyncVisible="False" SyncVisibility="False" Width="108px" />
                                    <px:PXGridColumn DataField="WorkgroupID" Width="90px" />
                                    <px:PXGridColumn DataField="OwnerID" LinkCommand="OpenActivityOwner" Width="150px" DisplayMode="Text"/>
                                    <px:PXGridColumn DataField="ProjectID" Width="80px" AllowShowHide="true" Visible="false" SyncVisible="false" />
                                    <px:PXGridColumn DataField="ProjectTaskID" Width="80px" AllowShowHide="true" Visible="false" SyncVisible="false"/>
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <CallbackCommands>
                            <Refresh CommitChanges="True" PostData="Page" />
                        </CallbackCommands>
                        <AutoSize Enabled="True" />
                        <GridMode AllowAddNew="False" AllowUpdate="False" />
                        <PreviewPanelTemplate>
                            <px:PXHtmlView ID="edBody" runat="server" DataField="body" TextMode="MultiLine" MaxLength="50" Width="100%" Height="100%" SkinID="Label" >
                                      <AutoSize Container="Parent" Enabled="true" />
                                </px:PXHtmlView>
                        </PreviewPanelTemplate>
                    </pxa:PXGridWithPreview>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Attributes">
                <Template>
                    <px:PXGrid ID="PXGridAnswers" runat="server" DataSourceID="ds" Width="100%" Height="100%" SkinID="DetailsInTab" MatrixMode="True">
                        <Levels>
                            <px:PXGridLevel DataMember="Answers">
                                <Columns>
                                    <px:PXGridColumn DataField="AttributeID" TextAlign="Left" Width="220px" AllowShowHide="False" TextField="AttributeID_description"/>
    								<px:PXGridColumn DataField="isRequired" TextAlign="Center" Type="CheckBox" Width="75px" />
                                    <px:PXGridColumn DataField="Value" Width="148px" RenderEditorText="True" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="true" />
                        <Mode AllowAddNew="False" AllowColMoving="False" AllowDelete="False" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Account Task Mapping">
                <Template>
                    <px:PXGrid runat="server" ID="AccountTaskGrid" Width="100%" DataSourceID="ds" Height="100%" SkinID="DetailsInTab" AdjustPageSize="Auto" AllowPaging="True">
                        <Levels>
                            <px:PXGridLevel DataMember="Accounts">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                                    <px:PXSegmentMask ID="edAccountID" runat="server" DataField="AccountID"  />
                                    <px:PXSegmentMask ID="edTaskID" runat="server" DataField="TaskID" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="AccountID" Width="108px" CommitChanges="True" />
                                    <px:PXGridColumn DataField="TaskID" Width="108px" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Invoices">
                <Template>
                    <px:PXGrid ID="InvoicesGrid" runat="server" Height="350px" Width="100%" Style="z-index: 100" AllowPaging="True" AdjustPageSize="Auto" AllowSearch="true" DataSourceID="ds"
                        SkinID="DetailsInTab">
                        <Levels>
                            <px:PXGridLevel DataMember="Invoices">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                                    <px:PXDropDown ID="edDocType" runat="server" DataField="DocType" />
                                    <px:PXTextEdit ID="edDocDesc" runat="server" DataField="DocDesc" />
                                    <px:PXSelector ID="edRefNbr" runat="server" DataField="RefNbr" />
                                    <px:PXDateTimeEdit ID="edDocDate" runat="server" DataField="DocDate" />
                                    <px:PXSelector ID="edFinPeriodID" runat="server" DataField="FinPeriodID" />
                                    <px:PXNumberEdit ID="edCuryDocBal" runat="server" DataField="CuryDocBal" Enabled="False" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="DocType" Label="Type" RenderEditorText="True" Width="99px" />
                                    <px:PXGridColumn DataField="RefNbr" Label="Reference Nbr." Width="117px" />
                                    <px:PXGridColumn DataField="DocDesc" Label="Description" Width="200px" />
                                    <px:PXGridColumn DataField="DocDate" Label="Date" Width="90px" />
                                    <px:PXGridColumn DataField="FinPeriodID" Label="Post Period" Width="81px" />
                                    <px:PXGridColumn DataField="CuryOrigDocAmt" TextAlign="Right" Width="100px" />
                                    <px:PXGridColumn DataField="CuryDocBal" TextAlign="Right" Width="100px" />
                                    <px:PXGridColumn DataField="CuryOrigDiscAmt" TextAlign="Right" Width="100px" />
                                    <px:PXGridColumn DataField="CuryDiscBal" TextAlign="Right" Width="100px" />
                                    <px:PXGridColumn DataField="CuryTaxTotal" TextAlign="Right" Width="100px" />
                                    <px:PXGridColumn DataField="Printed" TextAlign="Center" Type="CheckBox" Width="60px" />
                                    <px:PXGridColumn DataField="Status" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" />
                        <ActionBar DefaultAction="cmdViewInvoice">
                            <Actions>
                                <AddNew ToolBarVisible="False" />
                                <Delete ToolBarVisible="False" />
                            </Actions>
                            <CustomItems>
                                <px:PXToolBarButton Text="View Details" Key="cmdViewInvoice" CommandName="ViewInvoice" CommandSourceID="ds"></px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
                        <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
			<px:PXTabItem Text="Approval" RepaintOnDemand="false">
                <Template>
                    <px:PXGrid ID="gridApproval" runat="server" DataSourceID="ds" Width="100%" SkinID="DetailsInTab" NoteIndicator="True">
                        <AutoSize Enabled="true" />
                        <Mode AllowAddNew="false" AllowDelete="false" AllowUpdate="false" />
                        <ActionBar>
                            <Actions>
                                <AddNew Enabled="false" />
                                <EditRecord Enabled="false" />
                                <Delete Enabled="false" />
                            </Actions>
                        </ActionBar>
                        <Levels>
                            <px:PXGridLevel DataMember="Approval" DataKeyNames="ApprovalID,AssignmentMapID">
                                <Columns>
                                    <px:PXGridColumn DataField="WorkgroupID" Width="80px" />
                                    <px:PXGridColumn DataField="ApproverEmployee__AcctCD" Width="160px" />
                                    <px:PXGridColumn DataField="ApproverEmployee__AcctName" Width="160px" />
                                    <px:PXGridColumn DataField="ApprovedByEmployee__AcctCD" Width="100px" />
                                    <px:PXGridColumn DataField="ApprovedByEmployee__AcctName" Width="160px" />
                                    <px:PXGridColumn DataField="ApproveDate" Width="90px" />
                                    <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="Status" RenderEditorText="True" />
                                </Columns>
                                <Layout FormViewHeight="" />
                            </px:PXGridLevel>
                        </Levels>
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
        </Items>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
    </px:PXTab>
</asp:Content>
