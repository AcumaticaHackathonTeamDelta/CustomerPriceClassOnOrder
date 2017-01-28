<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="PM208000.aspx.cs" Inherits="Page_PM208000"
    Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.PM.TemplateMaint" PrimaryView="Project" BorderStyle="NotSet">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="Insert" PostData="Self" />
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
            <px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="true" />
            <px:PXDSCallbackCommand Name="Last" PostData="Self" />
            <px:PXDSCallbackCommand DependOnGrid="TaskGrid" Name="ViewTask" Visible="False" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Project" Caption="Template Summary" FilesIndicator="True"
        NoteIndicator="True" LinkPage="">
        <Template>
            <px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" StartColumn="True" />
            <px:PXSegmentMask ID="edContractCD" runat="server" DataField="ContractCD" DataSourceID="ds" />
            <px:PXDropDown CommitChanges="True" ID="edStatus" runat="server" DataField="Status" />
            <px:PXLayoutRule runat="server" />
            <px:PXTextEdit ID="edDescription" runat="server" DataField="Description" />
            <px:PXLayoutRule runat="server" StartColumn="True" />
            <px:PXCheckBox ID="chkNonProject" runat="server" DataField="NonProject" />
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXTab ID="tab" runat="server" Width="100%" Height="511px" DataSourceID="ds" DataMember="ProjectProperties">
        <Items>
            <px:PXTabItem Text="General Info">
                <Template>
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
					<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="General Settings" />
                    <px:PXSelector ID="edApproverID" runat="server" DataField="ApproverID" AutoRefresh="True" />
                    <px:PXCheckBox ID="chkRestrictToEmployeeList" runat="server" DataField="RestrictToEmployeeList" />
                    <px:PXCheckBox ID="chkRestrictToResourceList" runat="server" DataField="RestrictToResourceList" />
                     
					<px:PXLayoutRule ID="PXLayoutRule6" runat="server" StartGroup="True" GroupCaption="Defaults" />
                    <px:PXSegmentMask ID="PXSegmentMask1" runat="server" DataField="DefaultAccountID" />
                    <px:PXSegmentMask ID="PXSegmentMask2" runat="server" DataField="DefaultSubID" />
                    <px:PXSegmentMask ID="edDefaultAccrualAccountID" runat="server" DataField="DefaultAccrualAccountID"/>
                    <px:PXSegmentMask ID="edDefaultAccrualSubID" runat="server" DataField="DefaultAccrualSubID" />
					
					<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                    <px:PXLayoutRule runat="server" GroupCaption="Billing Settings" StartGroup="True" />
					<px:PXSelector ID="PXSelector1" runat="server" DataField="AllocationID"  AllowAddNew="True" AllowEdit="True"/>
                    <px:PXSelector ID="edBillingID" runat="server" DataField="BillingID"  AllowAddNew="True" AllowEdit="True"/>
					<px:PXSelector ID="PXSelector2" runat="server" DataField="RateTableID"  AllowAddNew="True" AllowEdit="True"/>
                    <px:PXFormView ID="billingForm" runat="server" DataMember="Billing" DataSourceID="ds" RenderStyle="Simple">
                        <Template>
                            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                            <px:PXDropDown CommitChanges="True" ID="edType" runat="server" DataField="Type" />
                        </Template>
                    </px:PXFormView>
                    <px:PXCheckBox ID="chkAutomaticReleaseAR" runat="server" DataField="AutomaticReleaseAR" />
                    <px:PXLayoutRule runat="server" GroupCaption="Visibility Settings" StartGroup="True" />
                    <px:PXLayoutRule runat="server" Merge="True" />
                    <px:PXCheckBox SuppressLabel="True" ID="chkVisibleInGL" runat="server" DataField="VisibleInGL" />
                    <px:PXCheckBox ID="chkVisibleInAP" runat="server" DataField="VisibleInAP" />
                    <px:PXCheckBox ID="chkVisibleInAR" runat="server" DataField="VisibleInAR" />
                    <px:PXCheckBox ID="chkVisibleInSO" runat="server" DataField="VisibleInSO" />
                    <px:PXCheckBox ID="chkVisibleInPO" runat="server" DataField="VisibleInPO" />
                    <px:PXLayoutRule runat="server" />
                    <px:PXLayoutRule runat="server" Merge="True" />
                    <px:PXCheckBox ID="chkVisibleInEP" runat="server" DataField="VisibleInEP" />
                    <px:PXCheckBox ID="chkVisibleInIN" runat="server" DataField="VisibleInIN" />
                    <px:PXCheckBox ID="chkVisibleInCA" runat="server" DataField="VisibleInCA" />
                    <px:PXCheckBox ID="chkVisibleInCR" runat="server" DataField="VisibleInCR" />
                    <px:PXLayoutRule runat="server" />
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Tasks">
                <Template>
                    <px:PXGrid runat="server" ID="TaskGrid" Width="100%" DataSourceID="ds" Height="100%" SkinID="DetailsInTab">
                        <Levels>
                            <px:PXGridLevel DataMember="Tasks" DataKeyNames="ProjectID,TaskCD">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM"  />
                                    <px:PXSegmentMask ID="edProjectTaskCD" runat="server" DataField="TaskCD" />
                                    <px:PXSegmentMask Size="s" ID="edDefaultSubID" runat="server" DataField="DefaultSubID" />
                                    <px:PXSelector Size="s" ID="edAllocationID" runat="server" DataField="AllocationID" AllowAddNew="True" AllowEdit="True" />
                                    <px:PXSelector Size="s" ID="edBillingID" runat="server" DataField="BillingID" AllowAddNew="True" AllowEdit="True" />
                                    <px:PXDropDown Size="m" ID="edBillingOption" runat="server" DataField="BillingOption" />
                                    <px:PXSegmentMask ID="edDefaultAccountID" runat="server" DataField="DefaultAccountID" />
                                    <px:PXTextEdit ID="edDescription" runat="server" DataField="Description" />
                                    <px:PXSegmentMask ID="edLocationID" runat="server" DataField="LocationID" />
                                    <px:PXDropDown ID="edStatus" runat="server" DataField="Status" Enabled="False" />
                                    <px:PXSelector ID="edApproverID" runat="server" DataField="ApproverID" AutoRefresh="True" />
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
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
                                    <px:PXGridColumn DataField="TaskCD" Label="Task ID" Width="81px" />
                                    <px:PXGridColumn DataField="Description" Label="Description" Width="200px" />
                                    <px:PXGridColumn DataField="AutoIncludeInPrj" Label="Automatic Include in Project" TextAlign="Center" Type="CheckBox" Width="60px" />
                                    <px:PXGridColumn DataField="RateTableID" Label="Rate Table" Width="117px" />
									<px:PXGridColumn DataField="AllocationID" Label="Billing Rule" Width="117px" />
									<px:PXGridColumn DataField="BillingID" Label="Billing Rule" Width="117px" />
                                    <px:PXGridColumn DataField="Status" Label="Status" RenderEditorText="True" Width="81px" />
                                    <px:PXGridColumn DataField="BillingOption" Label="Billing Option" RenderEditorText="True" Width="144px" />
                                    <px:PXGridColumn DataField="ApproverID" Width="108px" />
                                    <px:PXGridColumn AutoCallBack="True" DataField="DefaultAccountID" Width="108px" />
                                    <px:PXGridColumn DataField="DefaultSubID" Label="Default Sub." Width="108px" />
                                </Columns>
                                <Layout FormViewHeight="" />
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" />
                        <ActionBar DefaultAction="cmdViewTask">
                            <CustomItems>
                                <px:PXToolBarButton Text="Task Details" Key="cmdViewTask">
                                    <AutoCallBack Command="ViewTask" Target="ds" />
                                    <PopupCommand Command="Refresh" Target="TaskGrid" />
                                </px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
                        <Mode InitNewRow="True" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Employees" BindingContext="form" VisibleExp="DataControls[&quot;chkNonProject&quot;].Value != true">
                <Template>
                    <px:PXSplitContainer runat="server" ID="sp1" SplitterPosition="200" SkinID="Horizontal" Height="500px">
                        <AutoSize Enabled="true" />
                        <Template1>
                            <px:PXGrid ID="gridEmployeeContract" runat="server" DataSourceID="ds" SyncPosition="True" Height="300px" Width="100%"
                                SkinID="DetailsInTab">
                                <Levels>
                                    <px:PXGridLevel DataMember="EmployeeContract">
                                        <RowTemplate>
		                                     <px:PXSegmentMask ID="edEmployeeID" runat="server" DataField="EmployeeID"/>
			                            </RowTemplate>
                                        <Columns>
                                            <px:PXGridColumn DataField="EmployeeID" Width="150px" AutoCallBack="True" />
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
                                SkinID="DetailsInTab" Caption="Overrides">
                                <CallbackCommands>
                                    <Refresh SelectControlsIDs="gridEmployeeContract" />
                                </CallbackCommands>
                                <Levels>
                                    <px:PXGridLevel DataMember="ContractRates" DataKeyNames="RecordID">
                                        <Columns>
                                            <px:PXGridColumn DataField="EarningType" Width="110px" CommitChanges="True" />
                                            <px:PXGridColumn DataField="EPEarningType__Description" Width="110px" />
                                            <px:PXGridColumn DataField="LabourItemID" Width="100px" Label="Labor Item" />
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
            <px:PXTabItem Text="Equipment" BindingContext="form" VisibleExp="DataControls[&quot;chkNonProject&quot;].Value != true">
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
                                    <px:PXGridColumn DataField="EquipmentID" Width="108px" />
                                    <px:PXGridColumn DataField="EPEquipment__Description" Width="200px" />
                                    <px:PXGridColumn DataField="EPEquipment__RunRateItemID" Width="90px" />
                                    <px:PXGridColumn DataField="RunRate" Width="90px" />
                                    <px:PXGridColumn DataField="EPEquipment__SetupRateItemID" Width="90px" />
                                    <px:PXGridColumn DataField="SetupRate" Width="90px" />
                                    <px:PXGridColumn DataField="EPEquipment__SuspendRateItemID" Width="90px" />
                                    <px:PXGridColumn DataField="SuspendRate" Width="90px" />
                                </Columns>
                                <Layout FormViewHeight="" />
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Account Task Mapping">
                <Template>
                    <px:PXGrid runat="server" ID="AccountTaskGrid" Width="100%" DataSourceID="ds" Height="100%" SkinID="DetailsInTab" AdjustPageSize="Auto" AllowPaging="True">
                        <Levels>
                            <px:PXGridLevel DataMember="Accounts" DataKeyNames="ProjectID,RecordID">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                                    <px:PXSegmentMask ID="edAccountID" runat="server" DataField="AccountID" />
                                    <px:PXSegmentMask ID="edTaskID" runat="server" DataField="TaskID" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="AccountID" Width="108px" />
                                    <px:PXGridColumn DataField="TaskID" Width="108px" />
                                </Columns>
                                <Layout FormViewHeight="" />
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
        </Items>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
    </px:PXTab>
</asp:Content>
