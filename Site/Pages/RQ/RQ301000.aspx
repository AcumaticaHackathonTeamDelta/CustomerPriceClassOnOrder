<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="RQ301000.aspx.cs"
    Inherits="Page_RQ301000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" TypeName="PX.Objects.RQ.RQRequestEntry" PrimaryView="Document" BorderStyle="NotSet" Width="100%">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="Cancel" PopupVisible="true" />
            <px:PXDSCallbackCommand Name="Insert" PostData="Self" />
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" ClosePopup="true" PopupVisible="true" />
            <px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="True" />
            <px:PXDSCallbackCommand Name="Last" PostData="Self" />
            <px:PXDSCallbackCommand Name="Action" StartNewGroup="True" />
            <px:PXDSCallbackCommand Visible="false" Name="Hold" CommitChanges="true" />
            <px:PXDSCallbackCommand Name="viewDetails" DependOnGrid="grid" Visible="false" />
            <px:PXDSCallbackCommand Name="viewRequisition" DependOnGrid="gridContent" Visible="false" />
            <px:PXDSCallbackCommand Visible="False" Name="CurrencyView" />
            <px:PXDSCallbackCommand Name="AddInvBySite" Visible="False" CommitChanges="true" />
            <px:PXDSCallbackCommand Name="AddInvSelBySite" Visible="False" CommitChanges="true" />
            <px:PXDSCallbackCommand Name="NewTask" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="NewEvent" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="NewActivity" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="NewMailActivity" Visible="False" CommitChanges="True" PopupCommand="Cancel" PopupCommandTarget="ds" />
            <px:PXDSCallbackCommand StartNewGroup="True" Name="ValidateAddresses" Visible="False" CommitChanges="True" />
        </CallbackCommands>
    </px:PXDataSource>
    <px:PXSmartPanel ID="pnlContent" runat="server" Key="Contents" DesignView="Content" CaptionVisible="true" Caption="Requisition Details"
        Width="1000px" Height="400px" AutoCallBack-Command="Refresh" AutoCallBack-Enabled="True" AutoCallBack-Target="gridContent"
        CallBackMode-CommitChanges="True" CallBackMode-PostData="Page" LoadOnDemand="true" ShowAfterLoad="true">
        <px:PXGrid ID="gridContent" runat="server" DataSourceID="ds" Width="100%" BorderWidth="0px" SkinID="Details">
            <Mode AllowAddNew="false" AllowDelete="false" />
            <Levels>
                <px:PXGridLevel DataMember="Contents">
                    <RowTemplate>
                        <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
                        <px:PXSelector ID="edRQRequisition__ReqNbr" runat="server" AllowEdit="True" DataField="RQRequisition__ReqNbr" />
                    </RowTemplate>
                    <Columns>
                        <px:PXGridColumn DataField="RQRequisition__Priority" />
                        <px:PXGridColumn DataField="RQRequisition__ReqNbr" />
                        <px:PXGridColumn DataField="RQRequisition__OrderDate" Width="90px" />
                        <px:PXGridColumn DataField="RQRequisition__Status" Width="90px" />
                        <px:PXGridColumn DataField="RQRequisitionLineReceived__InventoryID" DisplayFormat="&gt;AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"
                            AllowUpdate="False" />
                        <px:PXGridColumn DataField="RQRequisitionLineReceived__UOM" DisplayFormat="&gt;aaaaaa" AllowUpdate="False" />
                        <px:PXGridColumn DataField="RQRequisitionLineReceived__Description" Width="150px" AllowUpdate="False" />
                        <px:PXGridColumn DataField="ItemQty" TextAlign="Right" AllowNull="False" Width="80px" />
                        <px:PXGridColumn DataField="RQRequisitionLineReceived__OrderQty" TextAlign="Right" AllowNull="False" Width="80px" />
                        <px:PXGridColumn DataField="RQRequisitionLineReceived__POOrderQty" TextAlign="Right" AllowNull="False" Width="80px" />
                        <px:PXGridColumn DataField="RQRequisitionLineReceived__POReceivedQty" TextAlign="Right" AllowNull="False" Width="80px" />
                    </Columns>
                </px:PXGridLevel>
            </Levels>
            <AutoSize Enabled="true" />
            <ActionBar DefaultAction="View">
                <CustomItems>
                    <px:PXToolBarButton Text="Details" Tooltip="Show requisition details" Key="View">
                        <AutoCallBack Command="viewRequisition" Target="ds">
                            <Behavior CommitChanges="True" />
                        </AutoCallBack>
                    </px:PXToolBarButton>
                </CustomItems>
            </ActionBar>
        </px:PXGrid>
        <px:PXPanel ID="PXPanel2" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton2" runat="server" DialogResult="Cancel" Text="OK" Width="63px" Height="20px" Style="margin-left: 5px" />
        </px:PXPanel>
    </px:PXSmartPanel>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Document" Caption="Document Summary"
        NoteIndicator="True" FilesIndicator="True" LinkIndicator="true" NotifyIndicator="true" ActivityIndicator="true" ActivityField="NoteActivity"
        EmailingGraph="PX.Objects.CR.CREmailActivityMaint,PX.Objects" DefaultControlID="edOrderNbr">
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="S" />
            <px:PXSelector ID="edOrderNbr" runat="server" DataField="OrderNbr" />
            <px:PXSelector CommitChanges="True" ID="edReqClassID" runat="server" DataField="ReqClassID" />
            <px:PXDropDown Size="s" ID="edStatus" runat="server" AllowNull="False" DataField="Status" Enabled="False" />
            <px:PXCheckBox ID="chkHold" runat="server" Checked="True" DataField="Hold">
                <AutoCallBack Command="Hold" Target="ds" />
            </px:PXCheckBox>
            <px:PXDateTimeEdit Size="s" ID="edOrderDate" runat="server" DataField="OrderDate" />
            <px:PXCheckBox ID="chkApproved" runat="server" DataField="Approved" />
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
            <px:PXDropDown ID="edPriority" runat="server" AllowNull="False" DataField="Priority" SelectedIndex="1" />
            <px:PXSegmentMask CommitChanges="True" ID="edEmployeeID" runat="server" DataField="EmployeeID" AutoRefresh="true" ValueField="AcctCD" />
            <px:PXSegmentMask CommitChanges="True" ID="edLocationID" runat="server" DataField="LocationID" AutoRefresh="true" />
            <px:PXSelector CommitChanges="True" ID="edDepartmentID" runat="server" DataField="DepartmentID" />
            <pxa:PXCurrencyRate ID="edCury" runat="server" DataField="CuryID" DataMember="_Currency_" DataSourceID="ds" RateTypeView="_RQRequest_CurrencyInfo_" />
            <px:PXLayoutRule runat="server" ColumnSpan="2" />
            <px:PXTextEdit ID="edDescription" runat="server" DataField="Description" />
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
            <px:PXNumberEdit ID="edCuryEstExtCostTotal" runat="server" DataField="CuryEstExtCostTotal" Enabled="False" />
            <px:PXNumberEdit ID="edOpenOrderQty" runat="server" DataField="OpenOrderQty" /></Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <script type="text/javascript">
        function UpdateItemSiteCell(n, c) {
            var activeRow = c.cell.row;
            var sCell = activeRow.getCell("Selected");
            var qCell = activeRow.getCell("QtySelected");
            if (sCell == c.cell) {
                if (sCell.getValue() == true)
                    qCell.setValue("1");
                else
                    qCell.setValue("0");
            }
            if (qCell == c.cell) {
                if (qCell.getValue() == "0")
                    sCell.setValue(false);
                else
                    sCell.setValue(true);
            }
        }
    </script>
    <px:PXTab ID="tab" runat="server" Height="504px" Style="z-index: 100;" Width="100%" DataSourceID="ds" DataMember="CurrentDocument">
        <Activity HighlightColor="" SelectedColor="" Width="" Height=""></Activity>
        <Items>
            <px:PXTabItem Text="Document Details">
                <Template>
                    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100; height: 423px; top: 0px; left: 0px; margin-bottom: 0px;"
                        Width="100%" BorderWidth="0px" SkinID="Details" Height="423px" TabIndex="1900">
                        <Levels>
                            <px:PXGridLevel DataMember="Lines" DataKeyNames="OrderNbr,LineNbr">
                                <Columns>
                                    <px:PXGridColumn DataField="InventoryID" DisplayFormat="&gt;AAAAAAAAAA" Width="100px" AutoCallBack="True" AllowShowHide="Server" />
                                    <px:PXGridColumn DataField="SubItemID" DisplayFormat="&gt;A" Width="100px" AutoCallBack="True" />
                                    <px:PXGridColumn DataField="Description" Width="200px" />
                                    <px:PXGridColumn DataField="UOM" DisplayFormat="&gt;aaaaaa" AutoCallBack="True" />
                                    <px:PXGridColumn AllowNull="False" DataField="OrderQty" TextAlign="Right" Width="100px" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryEstUnitCost" MatrixMode="True" TextAlign="Right" Width="100px" AutoCallBack="True" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryEstExtCost" MatrixMode="True" TextAlign="Right" Width="100px" />
                                    <px:PXGridColumn DataField="ExpenseAcctID" DisplayFormat="&gt;######" AutoCallBack="True" />
                                    <px:PXGridColumn DataField="ExpenseSubID" DisplayFormat="&gt;AA-AA-AA-AA-AAA" />
                                    <px:PXGridColumn DataField="VendorID" DisplayFormat="&gt;AAAAAAAAAA" AutoCallBack="True" />
                                    <px:PXGridColumn DataField="VendorLocationID" DisplayFormat="&gt;AAAAAA" AutoCallBack="True" />
                                    <px:PXGridColumn DataField="VendorName" Width="200px" />
                                    <px:PXGridColumn DataField="VendorRefNbr" />
                                    <px:PXGridColumn DataField="VendorDescription" Width="200px" />
                                    <px:PXGridColumn DataField="AlternateID" Width="120px" />
                                    <px:PXGridColumn DataField="RequestedDate" Width="90px" />
                                    <px:PXGridColumn DataField="PromisedDate" Width="90px" />
                                    <px:PXGridColumn DataField="IssueStatus" RenderEditorText="True" />
                                    <px:PXGridColumn AllowNull="False" DataField="Cancelled" TextAlign="Center" Type="CheckBox" Width="60px" />
                                </Columns>
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
                                    <px:PXCheckBox ID="chkCancelled" runat="server" DataField="Cancelled" />
                                    <px:PXSegmentMask CommitChanges="True" ID="edInventoryID" runat="server" DataField="InventoryID" AllowEdit="True" AutoRefresh="True" />
                                    <px:PXSegmentMask CommitChanges="True" ID="edSubItemID" runat="server" DataField="SubItemID" AutoRefresh="True" />
                                    <px:PXLayoutRule runat="server" ColumnSpan="2" />
                                    <px:PXTextEdit ID="edDescription" runat="server" DataField="Description" />
                                    <px:PXSegmentMask CommitChanges="True" ID="edVendorID" runat="server" DataField="VendorID" />
                                    <px:PXSegmentMask ID="edVendorLocationID" runat="server" DataField="VendorLocationID" AutoRefresh="True">
                                        <Parameters>
                                            <px:PXSyncGridParam ControlID="grid" />
                                        </Parameters>
                                    </px:PXSegmentMask>
                                    <px:PXLayoutRule runat="server" ColumnSpan="2" />
                                    <px:PXTextEdit ID="edVendorName" runat="server" DataField="VendorName" />
                                    <px:PXTextEdit ID="edAlternateID" runat="server" DataField="AlternateID" />
                                    <px:PXLayoutRule runat="server" ColumnSpan="3" />
                                    <px:PXTextEdit ID="edVendorDescription" runat="server" DataField="VendorDescription" />
                                    <px:PXSegmentMask CommitChanges="True" ID="edExpenseAcctID" runat="server" DataField="ExpenseAcctID" />
                                    <px:PXSegmentMask ID="edExpenseSubID" runat="server" DataField="ExpenseSubID" AutoRefresh="True" />
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
                                    <px:PXTextEdit ID="edVendorRefNbr" runat="server" DataField="VendorRefNbr" />
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
                                    <px:PXSelector ID="edUOM" runat="server" DataField="UOM" AutoRefresh="True">
                                        <Parameters>
                                            <px:PXSyncGridParam ControlID="grid" />
                                        </Parameters>
                                    </px:PXSelector>
                                    <px:PXNumberEdit ID="edOrderQty" runat="server" DataField="OrderQty" />
                                    <px:PXNumberEdit ID="edCuryEstUnitCost" runat="server" DataField="CuryEstUnitCost" />
                                    <px:PXNumberEdit ID="edCuryEstExtCost" runat="server" DataField="CuryEstExtCost" />
                                    <px:PXDateTimeEdit ID="edRequestedDate" runat="server" DataField="RequestedDate" />
                                    <px:PXDateTimeEdit ID="edPromisedDate" runat="server" DataField="PromisedDate" />
                                    <px:PXDropDown ID="edIssueStatus" runat="server" DataField="IssueStatus" Enabled="False" />
                                    <px:PXLayoutRule runat="server" ColumnSpan="2" />
                                    <px:PXTextEdit SuppressLabel="True" ID="orderNbr" runat="server" DataField="OrderNbr" />
                                    <px:PXLayoutRule runat="server" ColumnSpan="2" />
                                    <px:PXTextEdit SuppressLabel="True" ID="lineNbr" runat="server" DataField="LineNbr" />
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
                                    <px:PXNumberEdit ID="edOriginQty" runat="server" DataField="OriginQty" Enabled="False" />
                                    <px:PXNumberEdit ID="edIssuedQty" runat="server" DataField="IssuedQty" Enabled="False" />
                                    <px:PXNumberEdit ID="edOpenQty" runat="server" DataField="OpenQty" Enabled="False" /></RowTemplate>
                                <Layout FormViewHeight="" />
                            </px:PXGridLevel>
                        </Levels>
                        <Layout FormViewHeight="400px" />
                        <AutoSize Enabled="True" MinHeight="150" />
                        <Mode InitNewRow="True" AllowFormEdit="True" />
                        <ActionBar DefaultAction="Content">
                            <CustomItems>
                                <px:PXToolBarButton Text="Add Item" Key="cmdASI">
                                    <AutoCallBack Command="AddInvBySite" Target="ds">
                                        <Behavior PostData="Page" CommitChanges="True" />
                                    </AutoCallBack>
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Text="Details" Tooltip="Show requisition details" Key="Content">
                                    <AutoCallBack Command="viewDetails" Target="ds">
                                        <Behavior CommitChanges="True" />
                                    </AutoCallBack>
                                </px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Shipping Instructions">
                <Template>
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
                    <px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Ship To:" />
                    <px:PXDropDown CommitChanges="True" Size="s" ID="edShipDestType" runat="server" AllowNull="False" DataField="ShipDestType" />
                    <px:PXSelector CommitChanges="True" Size="m" ID="edShipToBAccountID" runat="server" DataField="ShipToBAccountID" AutoRefresh="True" />
                    <px:PXSegmentMask ID="edShipToLocationID" runat="server" AutoRefresh="True" DataField="ShipToLocationID" />
                    <px:PXFormView runat="server" DataMember="Shipping_Contact" ID="fShipping_Contact" DataSourceID="ds" RenderStyle="Simple">
                        <Template>
                            <px:PXLayoutRule runat="server" StartColumn="True"  LabelsWidth="SM" ControlSize="M" />
                            <px:PXCheckBox CommitChanges="True" SuppressLabel="True" ID="chkOverrideContact" runat="server" DataField="OverrideContact" />
                            <px:PXTextEdit ID="edSalutation" runat="server" DataField="Salutation" />
                            <px:PXTextEdit ID="edFullName" runat="server" DataField="FullName" />
                            <px:PXTextEdit ID="edPhone1" runat="server" DataField="Phone1" />
                            <px:PXMailEdit ID="edEmail" runat="server" DataField="Email" />
                        </Template>
                    </px:PXFormView>
                    <px:PXFormView ID="fShipping_Address" runat="server" DataMember="Shipping_Address" DataSourceID="ds" SyncPosition="true" RenderStyle="Simple">
                        <Template>
                            <px:PXLayoutRule runat="server" StartColumn="True"  LabelsWidth="SM" ControlSize="M" />
                            <px:PXCheckBox CommitChanges="True" SuppressLabel="True" ID="chkOverrideAddress" runat="server" DataField="OverrideAddress"
                                Height="18px" />
                            <px:PXCheckBox ID="chkIsValidated" runat="server" DataField="IsValidated" Enabled="False" />
                            <px:PXTextEdit ID="edAddressLine1" runat="server" DataField="AddressLine1" />
                            <px:PXTextEdit ID="edAddressLine2" runat="server" DataField="AddressLine2" />
                            <px:PXTextEdit ID="edCity" runat="server" DataField="City" />
                            <px:PXSelector ID="edCountryID" runat="server" DataField="CountryID" AutoRefresh="True" CommitChanges="true" />
                            <px:PXSelector ID="edState" runat="server" DataField="State" AutoRefresh="True">
                                <CallBackMode PostData="Container" />
                                <Parameters>
                                    <px:PXControlParam ControlID="fShipping_Address" Name="POShipAddress.countryID" PropertyName="DataControls[&quot;edCountryID&quot;].Value"
                                        Type="String" />
                                </Parameters>
                            </px:PXSelector>
                            <px:PXMaskEdit ID="edPostalCode" runat="server" DataField="PostalCode" CommitChanges="true" />
                        </Template>
                    </px:PXFormView>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Vendor Info" VisibleExp="DataControls[&quot;chkVendorHidden&quot;].Checked != true" RepaintOnDemand="false">
                <Template>
                    <px:PXLayoutRule runat="server" StartColumn="True" SuppressLabel="True" />
                    <px:PXPanel RenderStyle="Fieldset" ID="panelVC" runat="server" Caption="Vendor Address">
                        <px:PXFormView ID="formVC" runat="server" CaptionVisible="False" DataMember="Remit_Contact" DataSourceID="ds" Width="100%">
                            <Template>
                                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
                                <px:PXCheckBox CommitChanges="True" ID="chkOverrideContact" runat="server" DataField="OverrideContact" />
                                <px:PXTextEdit ID="edFullName" runat="server" DataField="FullName" />
                                <px:PXTextEdit ID="edSalutation" runat="server" DataField="Salutation" />
                                <px:PXTextEdit ID="edPhone1" runat="server" DataField="Phone1" />
                                <px:PXMailEdit ID="edEmail" runat="server" DataField="Email" />
                            </Template>
                            <ContentStyle BackColor="Transparent" BorderStyle="None">
                            </ContentStyle>
                        </px:PXFormView>
                        <px:PXFormView ID="formVA" DataMember="Remit_Address" runat="server" DataSourceID="ds" Width="100%" SyncPosition="true" CaptionVisible="False"
                            Style="left: 0px; position: absolute; top: 144px">
                            <AutoSize Enabled="True" MinHeight="100" MinWidth="100" />
                            <Template>
                                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
                                <px:PXCheckBox CommitChanges="True" ID="chkOverrideAddress" runat="server" DataField="OverrideAddress" />
                                <px:PXCheckBox ID="chkIsValidated" runat="server" DataField="IsValidated" Enabled="False" />
                                <px:PXTextEdit ID="edAddressLine1" runat="server" DataField="AddressLine1" />
                                <px:PXTextEdit ID="edAddressLine2" runat="server" DataField="AddressLine2" />
                                <px:PXTextEdit ID="edCity" runat="server" DataField="City" />
                                <px:PXSelector ID="edCountryID" runat="server" DataField="CountryID" AutoRefresh="True" CommitChanges="true" />
                                <px:PXSelector ID="edState" runat="server" DataField="State" AutoRefresh="True">
                                    <CallBackMode PostData="Container" />
                                    <Parameters>
                                        <px:PXControlParam ControlID="formVA" Name="PORemitAddress.countryID" PropertyName="DataControls[&quot;edCountryID&quot;].Value"
                                            Type="String" />
                                    </Parameters>
                                </px:PXSelector>
                                <px:PXMaskEdit ID="edPostalCode" runat="server" DataField="PostalCode" CommitChanges="true" />
                            </Template>
                            <ContentStyle BackColor="Transparent" BorderStyle="None">
                            </ContentStyle>
                        </px:PXFormView>
                    </px:PXPanel>
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
                    <px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Vendor" />
                    <px:PXSegmentMask CommitChanges="True" ID="edVendorID" runat="server" DataField="VendorID" />
                    <px:PXSegmentMask ID="edVendorLocationID" runat="server" AutoRefresh="True" DataField="VendorLocationID" />
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
                    <px:PXCheckBox SuppressLabel="True" ID="chkVendorHidden" runat="server" DataField="VendorHidden" />
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Approval Details" RepaintOnDemand="false">
                <Template>
                    <px:PXGrid ID="gridApproval" runat="server" DataSourceID="ds" Width="100%" SkinID="Details" NoteIndicator="True" BorderWidth="0px">
                        <AutoSize Enabled="True" />
                        <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
                        <Levels>
                            <px:PXGridLevel DataMember="Approval" DataKeyNames="ApprovalID,AssignmentMapID">
                                <RowTemplate>
                                    <px:PXSelector ID="edWorkgroupID" runat="server" DataField="WorkgroupID" AutoRefresh="True" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="ApproverEmployee__AcctCD" Width="160px" />
                                    <px:PXGridColumn DataField="ApproverEmployee__AcctName" Width="160px" />
                                    <px:PXGridColumn DataField="ApprovedByEmployee__AcctCD" Width="100px" />
                                    <px:PXGridColumn DataField="ApprovedByEmployee__AcctName" Width="160px" />
                                    <px:PXGridColumn DataField="ApproveDate" Width="90px" />
                                    <px:PXGridColumn DataField="Status" AllowNull="False" AllowUpdate="False" RenderEditorText="True" />
                                    <px:PXGridColumn DataField="WorkgroupID" Width="150px" />
                                </Columns>
                                <Layout FormViewHeight="" />
                            </px:PXGridLevel>
                        </Levels>
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Budget Details" VisibleExp="DataControls[&quot;chkBudgetValidation&quot;].Checked == true" RepaintOnDemand="false">
                <Template>
                    <div style="position: relative; height: 50px; top: 0px; left: 0px;">
                        <px:PXLabel ID="lblFinPeriodID" runat="server" Style="z-index: 100; position: absolute; left: 14px; top: 14px; right: 719px;">Fin Period:</px:PXLabel>
                        <px:PXSelector CommitChanges="True" ID="edFinPeriodID" runat="server" DataField="FinPeriodID" LabelID="lblFinPeriodID" Style="z-index: 101;
                            position: absolute; left: 95px; top: 14px; right: 626px;" Width="83px" />
                        <px:PXCheckBox ID="chkBudgetValidation" runat="server" DataField="BudgetValidation" Style="z-index: 100; position: absolute;
                            left: 185px; top: 14px; visibility: hidden;" />
                    </div>
                    <px:PXGrid runat="server" ID="gridBudget" Width="100%" Height="400px" Caption="Details" DataSourceID="ds" SkinID="DetailsWithFilter"
                        Style="position: static">
                        <Mode AllowAddNew="False" AllowUpdate="False" AllowDelete="False" />
                        <Levels>
                            <px:PXGridLevel DataMember="Budget" DataKeyNames="ExpenseAcctID,ExpenseSubID">
                                <Columns>
                                    <px:PXGridColumn DataField="ExpenseAcctID" DisplayFormat="&gt;######" />
                                    <px:PXGridColumn DataField="ExpenseSubID" DisplayFormat="&gt;AA-AA-AA-AA-AAA" Width="120px" />
                                    <px:PXGridColumn DataField="CuryID" />
                                    <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="DocRequestAmt" TextAlign="Right" Width="160px" />
                                    <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="RequestAmt" TextAlign="Right" Width="160px" />
                                    <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="BudgetAmt" TextAlign="Right" Width="100px" />
                                    <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="UsageAmt" TextAlign="Right" Width="100px" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="AprovedAmt" TextAlign="Right" Width="160px" />
                                    <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="UnaprovedAmt" TextAlign="Right" Width="160px" />
                                </Columns>
                                <Layout FormViewHeight="" />
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Other Information">
                <Template>
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
                    <px:PXSelector ID="edWorkgroupID" runat="server" DataField="WorkgroupID" />
                    <px:PXSelector ID="edOwnerID" runat="server" DataField="OwnerID" />
                </Template>
            </px:PXTabItem>
        </Items>
        <CallbackCommands>
            <Search CommitChanges="True" PostData="Page" />
            <Refresh CommitChanges="True" PostData="Page" />
        </CallbackCommands>
        <AutoSize Container="Window" Enabled="True" MinHeight="180" />
    </px:PXTab>
    <px:PXSmartPanel ID="PanelAddSiteStatus" runat="server" Key="sitestatus" LoadOnDemand="true" Width="800px" Height="500px"
        Caption="Inventory Lookup" CaptionVisible="true" AutoCallBack-Command='Refresh' AutoCallBack-Enabled="True" AutoCallBack-Target="formSitesStatus"
        DesignView="Hidden">
        <px:PXFormView ID="formSitesStatus" runat="server" CaptionVisible="False" DataMember="sitestatusfilter" DataSourceID="ds"
            Width="100%" SkinID="Transparent">
            <Template>
                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
                <px:PXTextEdit CommitChanges="True" ID="edInventory" runat="server" DataField="Inventory" />
                <px:PXTextEdit CommitChanges="True" ID="edBarCode" runat="server" DataField="BarCode" />
                <px:PXCheckBox CommitChanges="True" ID="chkOnlyAvailable" runat="server" Checked="True" DataField="OnlyAvailable" />
                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
                <px:PXSegmentMask CommitChanges="True" ID="edSiteID" runat="server" DataField="SiteID" />
                <px:PXSelector CommitChanges="True" ID="edItemClassID" runat="server" DataField="ItemClassID" />
                <px:PXSegmentMask CommitChanges="True" ID="edSubItem" runat="server" DataField="SubItem" AutoRefresh="true" /></Template>
        </px:PXFormView>
        <px:PXGrid ID="gripSiteStatus" runat="server" DataSourceID="ds" Style="border-width: 1px 0px; top: 0px; left: 0px;" AutoAdjustColumns="true"
            Width="100%" SkinID="Details" AdjustPageSize="Auto" Height="135px" AllowSearch="True" BatchUpdate="true" FastFilterID="edInventory"
            FastFilterFields="InventoryCD,Descr">
            <ClientEvents AfterCellUpdate="UpdateItemSiteCell" />
            <ActionBar PagerVisible="False">
                <PagerSettings Mode="NextPrevFirstLast" />
            </ActionBar>
            <Levels>
                <px:PXGridLevel DataMember="siteStatus">
                    <Mode AllowAddNew="false" AllowDelete="false" />
                    <Columns>
                        <px:PXGridColumn AllowNull="False" DataField="Selected" TextAlign="Center" Type="CheckBox" Width="80px" AutoCallBack="true"
                            AllowCheckAll="true" />
                        <px:PXGridColumn AllowNull="False" DataField="QtySelected" TextAlign="Right" Width="200px" />
                        <px:PXGridColumn DataField="SiteID" Width="160px" />
                        <px:PXGridColumn DataField="ItemClassID" Width="140px" />
                        <px:PXGridColumn DataField="ItemClassDescription" Width="140px" />
                        <px:PXGridColumn DataField="PriceClassID" Width="140px" />
                        <px:PXGridColumn DataField="PriceClassDescription" Width="140px" />
                        <px:PXGridColumn DataField="PreferredVendorID" Width="160px" />
                        <px:PXGridColumn DataField="PreferredVendorDescription" Width="140px" />
                        <px:PXGridColumn DataField="InventoryCD" DisplayFormat="&gt;AAAAAAAAAA" Width="260px" />
                        <px:PXGridColumn DataField="SubItemID" DisplayFormat="&gt;AA-A-A" Width="150px" />
                        <px:PXGridColumn DataField="Descr" Width="220px" />
                        <px:PXGridColumn DataField="PurchaseUnit" DisplayFormat="&gt;aaaaaa" Width="200px" />
                        <px:PXGridColumn AllowNull="False" DataField="QtyAvailExt" TextAlign="Right" Width="180px" />
                        <px:PXGridColumn AllowNull="False" DataField="QtyOnHandExt" TextAlign="Right" Width="180px" />
                        <px:PXGridColumn AllowNull="False" DataField="QtyPOOrdersExt" TextAlign="Right" Width="180px" />
                        <px:PXGridColumn AllowNull="False" DataField="QtyPOReceiptsExt" TextAlign="Right" Width="180px" />
                    </Columns>
                </px:PXGridLevel>
            </Levels>
            <AutoSize Enabled="true" />
        </px:PXGrid>
        <px:PXPanel ID="PXPanel1" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton6" runat="server" CommandName="AddInvSelBySite" CommandSourceID="ds" Text="Add" SyncVisible="false"/>
            <px:PXButton ID="PXButton7" runat="server" Text="Add & Close" DialogResult="OK" />
            <px:PXButton ID="PXButton8" runat="server" DialogResult="Cancel" Text="Cancel" />
        </px:PXPanel>
    </px:PXSmartPanel>
</asp:Content>
