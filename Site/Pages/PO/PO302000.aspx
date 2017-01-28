<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="PO302000.aspx.cs"
    Inherits="Page_PO302000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" TypeName="PX.Objects.PO.POReceiptEntry" PrimaryView="Document" BorderStyle="NotSet" Width="100%">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="Insert" PostData="Self" />
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
            <px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="True" />
            <px:PXDSCallbackCommand Name="Last" PostData="Self" />
            <px:PXDSCallbackCommand Name="Cancel" PopupVisible="true" />
            <px:PXDSCallbackCommand Name="Release" CommitChanges="True" StartNewGroup="true" />
            <px:PXDSCallbackCommand Name="Action" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="Inquiry" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="Report" CommitChanges="True" />
            <px:PXDSCallbackCommand Visible="false" StartNewGroup="true" Name="AddPOOrder" CommitChanges="true" />
            <px:PXDSCallbackCommand Visible="false" StartNewGroup="true" Name="AddTransfer" CommitChanges="true" />
            <px:PXDSCallbackCommand Visible="false" Name="AddPOOrderLine" CommitChanges="true" />
            <px:PXDSCallbackCommand Visible="false" Name="AddPOOrderLine2" CommitChanges="true" />
            <px:PXDSCallbackCommand Visible="false" Name="AddPOReceiptLine" CommitChanges="true" />
            <px:PXDSCallbackCommand Visible="false" Name="AddPOReceiptLine2" CommitChanges="true" />
            <px:PXDSCallbackCommand Name="Assign" Visible="false" RepaintControls="All" />
            <px:PXDSCallbackCommand Visible="false" DependOnGrid="grid" Name="ViewPOOrder" />
            <px:PXDSCallbackCommand Name="ViewINDocument" StartNewGroup="true" CommitChanges="True" Visible="false" />
            <px:PXDSCallbackCommand Name="ViewAPDocument" StartNewGroup="true" CommitChanges="True" Visible="false" />
            <px:PXDSCallbackCommand Name="CreateAPDocument" CommitChanges="True" Visible="false" />            
            <px:PXDSCallbackCommand Name="ViewLCINDocument" DependOnGrid="gridLCTran" StartNewGroup="true" Visible="false" />
            <px:PXDSCallbackCommand Name="ViewLCAPInvoice" DependOnGrid="gridLCTran" StartNewGroup="true" Visible="false" />
            <px:PXDSCallbackCommand Visible="False" Name="CurrencyView" />
            <px:PXDSCallbackCommand CommitChanges="True" Visible="False" Name="LSPOReceiptLine_generateLotSerial" />
            <px:PXDSCallbackCommand CommitChanges="True" Visible="False" Name="LSPOReceiptLine_binLotSerial" DependOnGrid="grid" />
			<px:PXDSCallbackCommand Name="NewTask" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="NewEvent" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="NewActivity" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="NewMailActivity" Visible="False" CommitChanges="True" PopupCommand="Cancel" PopupCommandTarget="ds" />
            <px:PXDSCallbackCommand Name="RecalculateDiscountsAction" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="RecalcOk" PopupCommand="" PopupCommandTarget="" PopupPanel="" Text="" Visible="False" />
        </CallbackCommands>
        <DataTrees>
            <px:PXTreeDataMember TreeView="_EPCompanyTree_Tree_" TreeKeys="WorkgroupID" />
        </DataTrees>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Document" Caption="Document Summary"
        NoteIndicator="True" FilesIndicator="True" LinkIndicator="true" NotifyIndicator="true" EmailingGraph="PX.Objects.CR.CREmailActivityMaint,PX.Objects"
        ActivityIndicator="true" ActivityField="NoteActivity" DefaultControlID="edReceiptType">
        <CallbackCommands>
            <Save PostData="Self" />
        </CallbackCommands>
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="S" />
            <px:PXDropDown  ID="edReceiptType" runat="server" SelectedIndex="-1" DataField="ReceiptType" />
            <px:PXSelector ID="edReceiptNbr" runat="server" DataField="ReceiptNbr" AutoRefresh="true">
                <GridProperties FastFilterFields="InvoiceNbr">
                </GridProperties>
            </px:PXSelector>
            <px:PXDropDown ID="edStatus" runat="server" AllowNull="False" DataField="Status" Enabled="False" />
            <px:PXCheckBox CommitChanges="True" ID="chkHold" runat="server" DataField="Hold" />
            <px:PXDateTimeEdit CommitChanges="True" ID="edReceiptDate" runat="server" DataField="ReceiptDate" />
            <px:PXSelector ID="edFinPeriodID" runat="server" DataField="FinPeriodID" />
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
            <px:PXSegmentMask CommitChanges="True" ID="edVendorID" runat="server" DataField="VendorID" AllowAddNew="True" AllowEdit="True">
                <GridProperties>
                    <Layout ColumnsMenu="False" />
                </GridProperties>
            </px:PXSegmentMask>
            <px:PXSegmentMask CommitChanges="True" ID="edVendorLocationID" runat="server" AutoRefresh="True" DataField="VendorLocationID">
                <GridProperties>
                    <Layout ColumnsMenu="False" />
                </GridProperties>
            </px:PXSegmentMask>
            <px:PXSegmentMask CommitChanges="True" ID="edSiteID" runat="server" DataField="SiteID" AllowAddNew="True" AllowEdit="True">
                <GridProperties>
                    <Layout ColumnsMenu="False" />
                </GridProperties>
            </px:PXSegmentMask>
            <pxa:PXCurrencyRate ID="edCury" DataField="CuryID" runat="server" DataSourceID="ds" RateTypeView="_POReceipt_CurrencyInfo_"
                DataMember="_Currency_"></pxa:PXCurrencyRate>
            <px:PXCheckBox CommitChanges="True" SuppressLabel="True" ID="chkAutoCreateInvoice" runat="server" Checked="True" DataField="AutoCreateInvoice" />
            <px:PXTextEdit CommitChanges="True" ID="edInvoiceNbr" runat="server" DataField="InvoiceNbr" />
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
            <px:PXNumberEdit ID="edOrderQty" runat="server" DataField="OrderQty" />
            <px:PXNumberEdit ID="edControlQty" runat="server" DataField="ControlQty" />
            <px:PXNumberEdit ID="edCuryVatExemptTotal" runat="server" DataField="CuryVatExemptTotal" Enabled="False" />
            <px:PXNumberEdit ID="edCuryVatTaxableTotal" runat="server" DataField="CuryVatTaxableTotal" Enabled="False" />
            <px:PXNumberEdit ID="edCuryDiscTot" runat="server" Enabled="False" DataField="CuryDiscTot" />
            <px:PXNumberEdit ID="edCuryOrderTotal" runat="server" DataField="CuryOrderTotal" Enabled="False" />
            <px:PXNumberEdit CommitChanges="True" ID="edCuryControlTotal" runat="server" DataField="CuryControlTotal" /></Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXTab ID="tab" runat="server" Height="353px" Style="z-index: 100;" Width="100%" DataMember="CurrentDocument" DataSourceID="ds">
        <Activity HighlightColor="" SelectedColor="" Width="" Height=""></Activity>
        <Items>
            <px:PXTabItem Text="Document Details">
                <Template>
                    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100; left: 0px; top: 0px; height: 455px;" Width="100%"
                        ActionsPosition="Top" SkinID="DetailsInTab" StatusField="Availability" SyncPosition="true">
                        <Levels>
                            <px:PXGridLevel DataMember="transactions">
                                <Columns>
                                    <px:PXGridColumn DataField="Availability" Width="1px" Label="Availability" />
                                    <px:PXGridColumn DataField="BranchID" DisplayFormat="&gt;AAAAAAAAAA" Width="81px" AutoCallBack="True" RenderEditorText="True"
                                        AllowShowHide="Server" />
                                    <px:PXGridColumn DataField="InventoryID" DisplayFormat="&gt;AAAAAAAAAA" Width="81px" AutoCallBack="True" Label="Inventory ID" />
                                    <px:PXGridColumn DataField="LineType" Type="DropDownList" AutoCallBack="True" Label="Line Type" />
                                    <px:PXGridColumn DataField="SubItemID" DisplayFormat="&gt;AA-A-A" NullText="&lt;SPLIT&gt;" AutoCallBack="True" Label="Subitem ID" />
                                    <px:PXGridColumn DataField="SiteID" DisplayFormat="&gt;AAAAAAAAAA" AutoCallBack="True" Label="Warehouse ID" />
                                    <px:PXGridColumn DataField="LocationID" DisplayFormat="&gt;AAAAAAAAAA" Width="81px" NullText="&lt;SPLIT&gt;" Label="Location ID" AutoCallBack="True"/>
                                    <px:PXGridColumn DataField="TranDesc" Width="180px" Label="Transaction Descr." />
                                    <px:PXGridColumn DataField="UOM" Width="54px" AutoCallBack="True" DisplayFormat="&gt;aaaaaa" Label="Unit of Measure" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="OrigOrderQty" Label="Ordered Qty." TextAlign="Right" Width="100px" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="OpenOrderQty" Label="Open Qty." TextAlign="Right" Width="100px" />
                                    <px:PXGridColumn AllowNull="False" DataField="ReceiptQty" TextAlign="Right" Width="81px" AutoCallBack="True" Label="Order Qty" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryUnitCost" TextAlign="Right" Width="81px" Label="Unit Cost" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryLineAmt" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn DataField="DiscPct" TextAlign="Right" />
									<px:PXGridColumn DataField="CuryDiscAmt" TextAlign="Right" />
                                    <px:PXGridColumn DataField="CuryDiscCost" TextAlign="Right" />
									<px:PXGridColumn DataField="ManualDisc" TextAlign="Center" Type="CheckBox" />
                                    <px:PXGridColumn DataField="DiscountID" RenderEditorText="True" TextAlign="Left" AllowShowHide="Server" Width="90px" AutoCallBack="True" />
                                    <px:PXGridColumn DataField="DiscountSequenceID" TextAlign="Left" Width="90px" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryExtCost" TextAlign="Right" Width="81px" Label="Unit Cost" />
                                    <px:PXGridColumn DataField="TaxCategoryID" DisplayFormat="&gt;aaaaaaaaaa" Width="54px" Label="Tax Category" />
                                    <px:PXGridColumn DataField="ExpenseAcctID" DisplayFormat="&gt;######" AutoCallBack="True" Label="Account" />
                                    <px:PXGridColumn DataField="ExpenseAcctID_Account_description" Width="120px" Label="Account Description" />
                                    <px:PXGridColumn DataField="ExpenseSubID" DisplayFormat="&gt;AA-AA-AA-AA-AAA" Label="Sub" />
									<px:PXGridColumn DataField="POAccrualAcctID" DisplayFormat="&gt;######" AutoCallBack="True"/>
                                    <px:PXGridColumn DataField="POAccrualSubID" DisplayFormat="&gt;AA-AA-AA-AA-AAA"/>
                                    <px:PXGridColumn AutoCallBack="True" DataField="ProjectID" Label="Project" Width="108px" />
                                    <px:PXGridColumn AutoCallBack="True" DataField="TaskID" DisplayFormat="&gt;AAAAAAAAAA" Label="Task" Width="81px" />
                                    <px:PXGridColumn DataField="ExpireDate" Width="90px" Label="Expire Date" />
                                    <px:PXGridColumn DataField="LineNbr" TextAlign="Right" Visible="False" Label="Line Nbr" />
                                    <px:PXGridColumn DataField="LotSerialNbr" Width="198px" NullText="&lt;SPLIT&gt;" Label="Lot/Serial Nbr" />
                                    <px:PXGridColumn DataField="POType" RenderEditorText="True" Width="72px" Label="Order Type" />
                                    <px:PXGridColumn DataField="PONbr" Label="Order Nbr" />
                                    <px:PXGridColumn DataField="POLineNbr" TextAlign="Right" Label="PO Line Nbr" />
                                    <px:PXGridColumn DataField="SOOrderType" Label="Transfer Type" />
                                    <px:PXGridColumn DataField="SOOrderNbr" Label="Transfer Nbr" />
                                    <px:PXGridColumn DataField="SOOrderLineNbr" TextAlign="Right" Label="Transfer Line Nbr" />
                                    <px:PXGridColumn DataField="SOShipmentNbr" />
                                    <px:PXGridColumn AllowNull="False" DataField="AllowComplete" TextAlign="Center" Type="CheckBox" Width="60px" AutoCallBack="True"
                                        Label="Allow Complete Line" />
                                    <px:PXGridColumn AllowNull="False" DataField="AllowOpen" TextAlign="Center" Type="CheckBox" Width="60px" AutoCallBack="True"
                                        Label="Allow Open Line" />
                                    <px:PXGridColumn AutoCallBack="True" DataField="ReasonCode" DisplayFormat="&gt;aaaaaaaaaa" Width="81px" Label="Reason Code" />
                                </Columns>
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                                    <px:PXDropDown CommitChanges="True" ID="edLineType" runat="server" AllowNull="False" DataField="LineType" />
                                    <px:PXSegmentMask CommitChanges="True" ID="edInventoryID" runat="server" DataField="InventoryID" AllowEdit="True" AutoRefresh="true">
                                        <Parameters>
                                            <px:PXControlParam ControlID="grid" Name="POReceiptLine.lineType" PropertyName="DataValues[&quot;LineType&quot;]" Type="String" />
                                        </Parameters>
                                    </px:PXSegmentMask>
                                    <px:PXSegmentMask CommitChanges="True" ID="edSubItemID" runat="server" DataField="SubItemID" AutoRefresh="True">
                                        <Parameters>
                                            <px:PXControlParam ControlID="grid" Name="POReceiptLine.inventoryID" PropertyName="DataValues[&quot;InventoryID&quot;]" Type="String" />
                                        </Parameters>
                                    </px:PXSegmentMask>
                                    <px:PXSegmentMask CommitChanges="True" ID="edSiteID" runat="server" DataField="SiteID" AutoRefresh="True" />
                                    <px:PXSegmentMask ID="edLocationID" runat="server" DataField="LocationID" AutoRefresh="true">
                                        <Parameters>
                                            <px:PXControlParam ControlID="grid" Name="POReceiptLine.siteID" PropertyName="DataValues[&quot;SiteID&quot;]" Type="String" />
                                            <px:PXControlParam ControlID="grid" Name="POReceiptLine.inventoryID" PropertyName="DataValues[&quot;InventoryID&quot;]" Type="String" />
                                            <px:PXControlParam ControlID="grid" Name="POReceiptLine.subItemID" PropertyName="DataValues[&quot;SubItemID&quot;]" Type="String" />
                                        </Parameters>
                                    </px:PXSegmentMask>
                                    <px:PXSelector CommitChanges="True" ID="edUOM" runat="server" DataField="UOM">
                                        <Parameters>
                                            <px:PXControlParam ControlID="grid" Name="POReceiptLine.inventoryID" PropertyName="DataValues[&quot;InventoryID&quot;]" Type="String" />
                                        </Parameters>
                                    </px:PXSelector>
                                    <px:PXNumberEdit ID="edOrigOrderQty" runat="server" DataField="OrigOrderQty" Enabled="False" />
                                    <px:PXNumberEdit ID="edOperOrderQty" runat="server" DataField="OpenOrderQty" Enabled="False" />
                                    <px:PXNumberEdit CommitChanges="True" ID="edReceiptQty" runat="server" DataField="ReceiptQty" />
                                    <px:PXNumberEdit ID="edCuryUnitCost" runat="server" DataField="CuryUnitCost" />
                                    <px:PXSelector ID="edDiscountCode" runat="server" DataField="DiscountID" CommitChanges="True" AllowEdit="True" />
                                    <px:PXNumberEdit ID="edDiscPct" runat="server" DataField="DiscPct" />
									<px:PXNumberEdit ID="edCuryDiscAmt" runat="server" DataField="CuryDiscAmt" />
									<px:PXCheckBox ID="chkManualDisc" runat="server" DataField="ManualDisc" CommitChanges="true" />
                                    <px:PXNumberEdit ID="edCuryLineAmt" runat="server" DataField="CuryLineAmt" />
                                    <px:PXNumberEdit ID="edCuryExtCost" runat="server" DataField="CuryExtCost" />
                                    <px:PXDropDown ID="edPOType" runat="server" DataField="POType" />
                                    <px:PXSelector ID="edPONbr" runat="server" DataField="PONbr" AllowEdit="True" />
                                    <px:PXSelector ID="edSOOrderNbr" runat="server" DataField="SOOrderNbr" AllowEdit="True" />
                                    <px:PXSelector ID="edSOShipmentNbr" runat="server" DataField="SOShipmentNbr" AllowEdit="True" />
                                    <px:PXLayoutRule runat="server" Merge="True" />
                                    <px:PXNumberEdit Size="xxs" ID="edPOLineNbr" runat="server" DataField="POLineNbr" />
                                    <px:PXCheckBox ID="chkAllowComplete" runat="server" DataField="AllowComplete" />
                                    <px:PXLayoutRule runat="server" Merge="False" />
                                    <px:PXLayoutRule ID="PXLayoutRule11" runat="server" ColumnSpan="2" />
                                    <px:PXTextEdit ID="edTranDesc" runat="server" DataField="TranDesc" />
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                                    <px:PXSegmentMask CommitChanges="True" Height="19px" ID="edBranchID" runat="server" DataField="BranchID" />
                                    <px:PXSelector ID="edTaxCategoryID" runat="server" DataField="TaxCategoryID" AutoRefresh="True"/>
                                    <px:PXSegmentMask CommitChanges="True" ID="edExpenseAcctID" runat="server" DataField="ExpenseAcctID" AutoRefresh="true" />
                                    <px:PXSegmentMask ID="edExpenseSubID" runat="server" DataField="ExpenseSubID" AutoRefresh="true">
                                        <Parameters>
                                            <px:PXSyncGridParam ControlID="grid" />
                                        </Parameters>
                                    </px:PXSegmentMask>
									<px:PXSegmentMask CommitChanges="True" ID="edPOAccrualAcctID" runat="server" DataField="POAccrualAcctID" AutoRefresh="true" />
									<px:PXSegmentMask CommitChanges="True" ID="edPOAccrualSubID" runat="server" DataField="POAccrualSubID" AutoRefresh="true" />
                                    <px:PXSelector ID="edLotSerialNbr" runat="server" DataField="LotSerialNbr" AutoRefresh="true">
                                        <Parameters>
                                            <px:PXSyncGridParam ControlID="grid" Name="SyncGrid" />
                                        </Parameters>
                                    </px:PXSelector>
                                    <px:PXDateTimeEdit ID="edExpireDate" runat="server" DataField="ExpireDate" DisplayFormat="d" />
                                    <px:PXSelector ID="edReasonCode" runat="server" DataField="ReasonCode" />
                                    <px:PXSegmentMask CommitChanges="True" ID="edProjectID" runat="server" DataField="ProjectID" />
                                    <px:PXSegmentMask CommitChanges="True" ID="edTaskID" runat="server" AutoRefresh="True" DataField="TaskID" /></RowTemplate>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" MinHeight="150" />
                        <Mode InitNewRow="True" AllowFormEdit="True" />
                        <ActionBar>
                            <CustomItems>
                                <px:PXToolBarButton Text="Allocations" Key="cmdLS" CommandName="LSPOReceiptLine_binLotSerial" CommandSourceID="ds" DependOnGrid="grid" />
                                <px:PXToolBarSeperator />
                                <px:PXToolBarButton Text="Add Line" Key="cmdAddReceiptLine" CommandSourceID="ds" CommandName="AddPOReceiptLine" />
                                <px:PXToolBarButton Text="Add Order" Key="cmdPO" CommandSourceID="ds" CommandName="AddPOOrder" />
                                <px:PXToolBarButton Text="Add Order Line" Key="cmdAddPOLine" CommandSourceID="ds" CommandName="AddPOOrderLine" />
                                <px:PXToolBarButton Text="Add Transfer" Key="cmdAddTransfer" CommandSourceID="ds" CommandName="AddTransfer" />
                                <px:PXToolBarSeperator />
                                <px:PXToolBarButton Text="View PO Order" Key="cmdViewPOOrder">
                                    <AutoCallBack Command="ViewPOOrder" Target="ds" />
                                </px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Tax Details" VisibleExp="DataControls[&quot;edReceiptType&quot;].Value!=RX" BindingContext="form">
                <Template>
                    <px:PXGrid ID="gridTaxes" runat="server" DataSourceID="ds" Height="150px" Style="z-index: 100" Width="100%" ActionsPosition="Top"
                        BorderWidth="0px" SkinID="Details">
                        <AutoSize Enabled="True" MinHeight="150" />
                        <ActionBar>
                            <Actions>
                                <Search Enabled="False" />
                                <Save Enabled="False" />
                                <EditRecord Enabled="False" />
                            </Actions>
                        </ActionBar>
                        <Levels>
                            <px:PXGridLevel DataMember="Taxes">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                                    <px:PXSelector SuppressLabel="True" ID="edTaxID" runat="server" DataField="TaxID" />
                                    <px:PXNumberEdit SuppressLabel="True" ID="edTaxRate" runat="server" DataField="TaxRate" Enabled="False" />
                                    <px:PXNumberEdit SuppressLabel="True" ID="edCuryTaxableAmt" runat="server" DataField="CuryTaxableAmt" />
                                    <px:PXNumberEdit SuppressLabel="True" ID="edCuryTaxAmt" runat="server" DataField="CuryTaxAmt" /></RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="TaxID" Width="81px" />
                                    <px:PXGridColumn AllowNull="False" DataField="TaxRate" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryTaxableAmt" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryTaxAmt" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn AllowNull="False" DataField="Tax__TaxType" Label="Tax Type" RenderEditorText="True" />
                                    <px:PXGridColumn AllowNull="False" DataField="Tax__PendingTax" Label="Pending VAT" TextAlign="Center" Type="CheckBox" Width="60px" />
                                    <px:PXGridColumn AllowNull="False" DataField="Tax__ReverseTax" Label="Reverse VAT" TextAlign="Center" Type="CheckBox" Width="60px" />
                                    <px:PXGridColumn AllowNull="False" DataField="Tax__ExemptTax" Label="Exempt From VAT" TextAlign="Center" Type="CheckBox"
                                        Width="60px" />
                                    <px:PXGridColumn AllowNull="False" DataField="Tax__StatisticalTax" Label="Statistical VAT" TextAlign="Center" Type="CheckBox"
                                        Width="60px" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Financial Details">
                <Template>
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                    <px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Billing Settings " />
                    <px:PXSegmentMask CommitChanges="True" ID="edBranchID" runat="server" DataField="BranchID" />
                    <px:PXSelector CommitChanges="True" ID="edTermsID" runat="server" DataField="TermsID" />
                    <px:PXDateTimeEdit CommitChanges="True" ID="edInvoiceDate" runat="server" DataField="InvoiceDate" />
                    <px:PXDateTimeEdit ID="edDueDate" runat="server" DataField="DueDate" />
                    <px:PXDateTimeEdit ID="edDiscDate" runat="server" DataField="DiscDate" />
                    <px:PXNumberEdit ID="edCuryDiscAmt" runat="server" DataField="CuryDiscAmt" CommitChanges="True" />
                    <px:PXSelector CommitChanges="True" ID="edTaxZoneID" runat="server" DataField="TaxZoneID" Text="ZONE1" />
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                    <px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Totals" />
                    <px:PXNumberEdit ID="edCuryUnbilledTotal" runat="server" DataField="CuryUnbilledTotal" Enabled="False" />
                    <px:PXNumberEdit ID="edUnbilledQty" runat="server" DataField="UnbilledQty" />
                    <px:PXNumberEdit ID="edCuryUnbilledLineTotal" runat="server" DataField="CuryUnbilledLineTotal" Enabled="False" />
                    <px:PXNumberEdit ID="edCuryUnbilledTaxTotal" runat="server" DataField="CuryUnbilledTaxTotal" Enabled="False" />
                    <px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartGroup="True" GroupCaption="Assign To" LabelsWidth="SM" ControlSize="XM" />
                    <px:PXSelector ID="trsWorkgroupID" runat="server" DataField="WorkgroupID" CommitChanges="True" />
                    <px:PXSelector ID="edOwnerID" runat="server" DataField="OwnerID" AutoRefresh="true" />

                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Landed Costs" BindingContext="form">
                <Template>
                    <px:PXGrid ID="gridLCTran" runat="server" DataSourceID="ds" Style="z-index: 100; left: 0px; top: 0px; height: 317px;" Width="100%"
                        ActionsPosition="Top" BorderWidth="0px" SkinID="Details">
                        <Levels>
                            <px:PXGridLevel DataMember="landedCostTrans" SortOrder="GroupTranID">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                                    <px:PXSelector CommitChanges="True" ID="edLandedCostCodeID" runat="server" DataField="LandedCostCodeID" />
                                    <px:PXTextEdit ID="edInvoiceNbr" runat="server" DataField="InvoiceNbr" CommitChanges="true" />
                                    <px:PXSegmentMask CommitChanges="True" ID="edVendorID" runat="server" DataField="VendorID" Enabled="False" />
                                    <px:PXSegmentMask ID="edVendorLocationID" runat="server" DataField="VendorLocationID" Enabled="False">
                                        <Parameters>
                                            <px:PXSyncGridParam ControlID="gridLCTran" />
                                        </Parameters>
                                    </px:PXSegmentMask>
                                    <px:PXDateTimeEdit ID="edInvoiceDate" runat="server" DataField="InvoiceDate" />
                                    <pxa:CurrencyEditor ID="CurrencyEditor1" DataField="CuryInfoID" runat="server" Hidden="true"
                                        DataSourceID="ds" DataMember="_LandedCostTran_CurrencyInfo_" />
                                    <pxa:PXCurrencyRate ID="edCury" DataField="CuryID" runat="server" DataSourceID="ds" RateTypeView="_POReceipt_CurrencyInfo_"
                                        DataMember="_Currency_"></pxa:PXCurrencyRate>
                                    <px:PXSelector ID="edCury2" DataField="CuryID" runat="server" /> 
                                    <px:PXNumberEdit ID="edCuryLCAmount" runat="server" DataField="CuryLCAmount" />
                                    <px:PXLayoutRule runat="server" ColumnSpan="2" />
                                    <px:PXTextEdit ID="edDescr" runat="server" DataField="Descr" />
                                    <px:PXSegmentMask ID="edInventoryID1" runat="server" DataField="InventoryID" AutoRefresh="true" />
                                    <px:PXLayoutRule ID="PXLayoutRule25" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                                    <px:PXSelector ID="edTaxCategory1ID" runat="server" DataField="TaxCategoryID" AutoRefresh="True"/>
                                    <px:PXSelector ID="edTermsID" runat="server" DataField="TermsID" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn AllowUpdate="False" DataField="LCTranID" TextAlign="Right" Visible="False" />
                                    <px:PXGridColumn DataField="LandedCostCodeID" DisplayFormat="&gt;aaaaaaaaaaaaaaa" Width="117px" AutoCallBack="True" />
                                    <px:PXGridColumn DataField="POReceiptType" Width="18px" />
                                    <px:PXGridColumn DataField="POReceiptNbr" Width="90px" />
                                    <px:PXGridColumn DataField="LineNbr"/>
                                    <px:PXGridColumn DataField="Descr" Width="351px" />
                                    <px:PXGridColumn DataField="InvoiceNbr" Width="90px" />
                                    <px:PXGridColumn DataField="VendorID" DisplayFormat="&gt;AAAAAAAAAA" Width="81px" AutoCallBack="True" />
                                    <px:PXGridColumn DataField="VendorLocationID" DisplayFormat="&gt;AAAAAA" Width="54px" AutoCallBack="True" />
                                    <px:PXGridColumn AllowNull="False" DataField="InvoiceDate" Width="90px" />
                                    <px:PXGridColumn DataField="CuryID" Width="54px" Visible="False" AllowShowHide="False" />
                                    <px:PXGridColumn DataField="CuryInfoID" Width="54px" TextField="CuryID" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryLCAmount" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn DataField="TaxCategoryID" DisplayFormat="&gt;aaaaaaaaaa" Label="Tax Category" />
                                    <px:PXGridColumn DataField="TermsID" DisplayFormat="&gt;aaaaaaaaaa" Label="Terms" Width="81px" />
                                    <px:PXGridColumn DataField="InventoryID" DisplayFormat="&gt;AAAAAAAAAA" Width="115px" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="APDocType" Label="AP Doc. Type" RenderEditorText="True" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="APRefNbr" DisplayFormat="&gt;CCCCCCCCCCCCCCC" Label="AP Ref. Nbr." />
                                    <px:PXGridColumn AllowUpdate="False" DataField="INDocType" Label="IN Doc. Type" RenderEditorText="True" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="INRefNbr" DisplayFormat="&gt;CCCCCCCCCCCCCCC" Label="IN Ref. Nbr." />
                                    <px:PXGridColumn AllowNull="False" DataField="PostponeAP" TextAlign="Center" Type="CheckBox" Width="60px" CommitChanges="true" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" MinHeight="150" />
                        <Mode InitNewRow="True" AllowFormEdit="True" />
                        <ActionBar>
                            <CustomItems>
                                <px:PXToolBarButton Text="View LC APInvoice">
                                    <AutoCallBack Command="ViewLCAPInvoice" Target="ds" />
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Text="View LC IN Adjustment" Key="cmdViewLCTran">
                                    <AutoCallBack Command="ViewLCINDocument" Target="ds" />
                                </px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
                        <AutoSize Enabled="True" MinHeight="150" />
                        <Mode InitNewRow="True" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Discount Details">
                <Template>
                    <px:PXGrid ID="formDiscountDetail" runat="server" DataSourceID="ds" Width="100%" SkinID="Details" BorderStyle="None">
                        <Levels>
                            <px:PXGridLevel DataMember="DiscountDetails" DataKeyNames="ReceiptType,ReceiptNbr,DiscountID,DiscountSequenceID,Type">
                                <RowTemplate>
                                    <px:PXLayoutRule ID="PXLayoutRule3" runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                                    <px:PXCheckBox ID="chkSkipDiscount" runat="server" DataField="SkipDiscount" />
                                    <px:PXSelector ID="edDiscountID" runat="server" DataField="DiscountID" 
                                        AllowEdit="True" edit="1" />
                                    <px:PXDropDown ID="edType" runat="server" DataField="Type" Enabled="False" />
                                    <px:PXCheckBox ID="chkIsManual" runat="server" DataField="IsManual" />
                                    <px:PXSelector ID="edDiscountSequenceID" runat="server" DataField="DiscountSequenceID" AutoRefresh="true" AllowEdit="true"/>
                                    <px:PXNumberEdit ID="edCuryDiscountableAmt" runat="server" DataField="CuryDiscountableAmt" />
                                    <px:PXNumberEdit ID="edDiscountableQty" runat="server" DataField="DiscountableQty" />
                                    <px:PXNumberEdit ID="edCuryDiscountAmt" runat="server" DataField="CuryDiscountAmt" />
                                    <px:PXNumberEdit ID="edDiscountPct" runat="server" DataField="DiscountPct" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="SkipDiscount" Width="75px" Type="CheckBox" TextAlign="Center" />
                                    <px:PXGridColumn DataField="DiscountID" Width="90px" CommitChanges="true" />
                                    <px:PXGridColumn DataField="DiscountSequenceID" Width="90px" CommitChanges="true" />
                                    <px:PXGridColumn DataField="Type" RenderEditorText="True" Width="90px" />
                                    <px:PXGridColumn DataField="IsManual" Width="75px" Type="CheckBox" TextAlign="Center" />
                                    <px:PXGridColumn DataField="CuryDiscountableAmt" TextAlign="Right" Width="90px" />
                                    <px:PXGridColumn DataField="DiscountableQty" TextAlign="Right" Width="90px" />
                                    <px:PXGridColumn DataField="CuryDiscountAmt" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn DataField="DiscountPct" TextAlign="Right" Width="81px" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" MinHeight="150" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
        </Items>
        <AutoSize Enabled="True" Container="Window" MinHeight="180"></AutoSize>
        <CallbackCommands>
            <Refresh CommitChanges="True" PostData="Page" />
            <Search CommitChanges="True" PostData="Page" />
        </CallbackCommands>
        <AutoSize Container="Window" Enabled="True" MinHeight="180" />
    </px:PXTab>
    <script type="text/javascript">
        function PanelAdd_Load() {
            PXCallback.addHandler(_receiptHhandleCallback);
            var barcode = px_alls["edBarCodePnl"];
            var subitem = px_alls["edSubItemIDPnl"];
            var lotSerial = px_alls["edLotSerialNbrPnl"];
            var location = px_alls["edLocationIDPnl"];
            barcode.focus();
            barcode.events.addEventHandler("keyPress", _keypress);
            subitem.events.addEventHandler("keyPress", _keypress);
            lotSerial.events.addEventHandler("keyPress", _keypress);
            location.events.addEventHandler("keyPress", _keypress);
        }

        function _keypress(ctrl, ev) {
            var me = this, timeout = this._enterTimeoutID;
            if (timeout) clearTimeout(timeout);
            this._enterTimeoutID = setTimeout(function () {
                var autoAdd = px_alls["chkAutoAddLine"];
                if (autoAdd != null && autoAdd.getValue() == true)
                    ctrl.updateValue();
            }, 500);
        }

        function _receiptHhandleCallback(context, error) {
            var barcode = px_alls["edBarCodePnl"];
            if (context != null && context.info != null && context.info.name == "AddPOReceiptLine2" && barcode != null) {
                barcode.focus();
                return;
            }

            if (context == null || context.info == null || context.info.name != "Save" || !context.controlID.endsWith("_frmReceipt"))
                return;

            var item = px_alls["edInventoryIDPnl"];
            var subitem = px_alls["edSubItemIDPnl"];
            var lotSerial = px_alls["edLotSerialNbrPnl"];
            var location = px_alls["edLocationIDPnl"];
            var description = px_alls["edDescriptionPnl"];

            if (description != null && description.getValue() != null)
                document.getElementById("audioDing").play();

            if (barcode != null && barcode.getValue() == null && barcode.getEnabled())
            { barcode.focus(); return; }

            if (item != null && item.getValue() == null && barcode != null)
            { barcode.elemText.select(); barcode.focus(); return; }

            if (subitem != null && subitem.getValue() == null && subitem.getEnabled())
            { subitem.focus(); return; }

            if (lotSerial != null && lotSerial.getValue() == null && lotSerial.getEnabled())
            { lotSerial.focus(); return; }

            if (location != null && location.getValue() == null && location.getEnabled())
            { location.focus(); return; }
        }

    </script>
    <px:PXSmartPanel ID="PanelAddRL" runat="server" Style="z-index: 108;" Width="708px" Key="addReceipt" Caption="Add Receipt Line"
        CaptionVisible="True" LoadOnDemand="true" AutoCallBack-Command="Refresh" AutoCallBack-Enabled="True" AutoCallBack-Target="frmReceipt" ClientEvents-AfterLoad="PanelAdd_Load">
        <px:PXFormView ID="frmReceipt" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="addReceipt"
            SkinID="Transparent" CaptionVisible="False">
            <Activity Height="" HighlightColor="" SelectedColor="" Width="" />
            <Template>
                <audio id="audioDing" src="../../Sounds/Ding.wav" preload="auto" style="visibility: hidden">
                </audio>
                <px:PXLayoutRule runat="server" StartColumn="true" ControlSize="SM" />      
                <px:PXTextEdit CommitChanges="True" ID="edBarCodePnl" runat="server" DataField="BarCode" />
                <px:PXSegmentMask CommitChanges="True" ID="edInventoryIDPnl" runat="server" DataField="InventoryID" />
                <px:PXSegmentMask CommitChanges="True" ID="edSubItemIDPnl" runat="server" DataField="SubItemID" />

                <px:PXTextEdit CommitChanges="True" ID="edLotSerialNbrPnl" runat="server" DataField="LotSerialNbr"  />
                <px:PXSegmentMask CommitChanges="True" ID="edSiteID" runat="server" DataField="SiteID"  />
                <px:PXSegmentMask CommitChanges="True" ID="edLocationIDPnl" runat="server" DataField="LocationID" />
                
                <px:PXDateTimeEdit ID="edExpireDate" runat="server" DataField="ExpireDate" DisplayFormat="d" />
                <px:PXNumberEdit CommitChanges="True" ID="edReceiptQty" runat="server" DataField="ReceiptQty" Size="XS" />
                <px:PXSelector CommitChanges="True" ID="edUOM" runat="server" DataField="UOM" Size="XS" />

                <px:PXLayoutRule runat="server" StartColumn="true" ControlSize="S" />
                <px:PXSegmentMask CommitChanges="True" ID="edVendorID" runat="server" DataField="VendorID" Size="M" />
                <px:PXSegmentMask ID="edVendorLocationID" runat="server" DataField="VendorLocationID" Size="SM" />
                <px:PXDropDown CommitChanges="True" ID="edPOType" runat="server" DataField="POType" />

                <px:PXSelector ID="edPONbr" runat="server" DataField="PONbr" Enabled="False" />
                <px:PXNumberEdit ID="edPOLineNbr" runat="server" DataField="POLineNbr" Enabled="False" />
                <px:PXSegmentMask ID="edShipFromSiteID" runat="server"  DataField="ShipFromSiteID" Enabled="False" />
                <px:PXSelector ID="edSOOrderType" runat="server" DataField="SOOrderType" Enabled="False" />
                <px:PXSelector ID="edSOOrderNbr" runat="server" DataField="SOOrderNbr" Enabled="False" />
                <px:PXNumberEdit ID="edSOOrderLineNbr" runat="server" DataField="SOOrderLineNbr" Enabled="False" />
                <px:PXSelector ID="edSOShipmentNbr" runat="server" DataField="SOShipmentNbr" Enabled="False" />
                <px:PXNumberEdit CommitChanges="True" ID="edCuryUnitCost" runat="server" DataField="CuryUnitCost" />
                <px:PXNumberEdit ID="edCuryExtCost" runat="server" DataField="CuryExtCost"  />              
                
                <px:PXCheckBox CommitChanges="True" ID="chkByOne" runat="server" Checked="True" DataField="ByOne" />
                <px:PXCheckBox CommitChanges="True" ID="chkAutoAddLine" runat="server" DataField="AutoAddLine" />
                <px:PXTextEdit ID="edDescriptionPnl" runat="server" DataField="Description" SkinID="Label" Size="M" />
            </Template>
        </px:PXFormView>
        <px:PXPanel ID="PXPanel1" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton7" runat="server" Text="Add" CommandSourceID="ds" CommandName="AddPOReceiptLine2" />
            <px:PXButton ID="PXButton5" runat="server" DialogResult="OK" Text="Add & Close" />
            <px:PXButton ID="PXButton6" runat="server" DialogResult="No" Text="Close" />
        </px:PXPanel>
    </px:PXSmartPanel>
    <px:PXSmartPanel ID="PanelLS" runat="server" Style="z-index: 108;" Width="764px" Height="360px" Caption="Allocations" CaptionVisible="True"
        Key="lsselect" AutoCallBack-Command="Refresh" AutoCallBack-Enabled="True" AutoCallBack-Target="optform">
        <px:PXFormView ID="optform" runat="server" Width="100%" CaptionVisible="False" DataMember="LSPOReceiptLine_lotseropts" DataSourceID="ds"
            SkinID="Transparent">
            <Parameters>
                <px:PXSyncGridParam ControlID="grid" />
            </Parameters>
            <Template>
                <px:PXLayoutRule ID="PXLayoutRule26" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                <px:PXNumberEdit ID="edUnassignedQty" runat="server" DataField="UnassignedQty" Enabled="False" />
                <px:PXNumberEdit ID="edQty" runat="server" DataField="Qty">
                    <AutoCallBack>
                        <Behavior CommitChanges="True" />
                    </AutoCallBack>
                </px:PXNumberEdit>
                <px:PXLayoutRule ID="PXLayoutRule27" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                <px:PXMaskEdit ID="edStartNumVal" runat="server" DataField="StartNumVal" />
                <px:PXButton ID="btnGenerate" runat="server" Text="Generate" Height="20px" CommandName="LSPOReceiptLine_generateLotSerial"
                    CommandSourceID="ds">
                </px:PXButton>
            </Template>
        </px:PXFormView>
        <px:PXGrid ID="grid2" runat="server" Width="100%" AutoAdjustColumns="True" DataSourceID="ds" Style="border-width: 1px 0px;
            left: 0px; top: 0px; height: 192px;" SyncPosition="true">
            <Mode InitNewRow="True" />
            <Parameters>
                <px:PXSyncGridParam ControlID="grid" />
            </Parameters>
            <Levels>
                <px:PXGridLevel DataMember="splits">
                    <Columns>
                        <px:PXGridColumn DataField="InventoryID" Width="108px" />
                        <px:PXGridColumn DataField="SubItemID" Width="108px" />
                        <px:PXGridColumn DataField="LocationID" AllowShowHide="Server" Width="108px" />
                        <px:PXGridColumn DataField="LotSerialNbr" AllowShowHide="Server" Width="108px" />
                        <px:PXGridColumn DataField="Qty" Width="108px" TextAlign="Right" />
                        <px:PXGridColumn DataField="UOM" Width="108px" />
                        <px:PXGridColumn DataField="ExpireDate" Width="90px" />
                    </Columns>
                    <RowTemplate>
                        <px:PXLayoutRule ID="PXLayoutRule28" runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                        <px:PXSegmentMask ID="edSubItemID2" runat="server" DataField="SubItemID" AutoRefresh="true" />
                        <px:PXSegmentMask ID="edLocationID2" runat="server" DataField="LocationID" AutoRefresh="true">
                            <Parameters>
                                <px:PXControlParam ControlID="grid2" Name="POReceiptLineSplit.siteID" PropertyName="DataValues[&quot;SiteID&quot;]" Type="String" />
                                <px:PXControlParam ControlID="grid2" Name="POReceiptLineSplit.inventoryID" PropertyName="DataValues[&quot;InventoryID&quot;]"
                                    Type="String" />
                                <px:PXControlParam ControlID="grid2" Name="POReceiptLineSplit.subItemID" PropertyName="DataValues[&quot;SubItemID&quot;]"
                                    Type="String" />
                            </Parameters>
                        </px:PXSegmentMask>
                        <px:PXNumberEdit ID="edQty2" runat="server" DataField="Qty" />
                        <px:PXSelector ID="edUOM2" runat="server" DataField="UOM" AutoRefresh="true">
                            <Parameters>
                                <px:PXControlParam ControlID="grid" Name="POReceiptLineSplit.inventoryID" PropertyName="DataValues[&quot;InventoryID&quot;]"
                                    Type="String" />
                            </Parameters>
                        </px:PXSelector>
                        <px:PXSelector ID="edLotSerialNbr2" runat="server" DataField="LotSerialNbr" AutoRefresh="true">
                            <Parameters>
                                <px:PXControlParam ControlID="grid2" Name="POReceiptLineSplit.inventoryID" PropertyName="DataValues[&quot;InventoryID&quot;]"
                                    Type="String" />
                                <px:PXControlParam ControlID="grid2" Name="POReceiptLineSplit.subItemID" PropertyName="DataValues[&quot;SubItemID&quot;]"
                                    Type="String" />
                                <px:PXControlParam ControlID="grid2" Name="POReceiptLineSplit.locationID" PropertyName="DataValues[&quot;LocationID&quot;]"
                                    Type="String" />
                            </Parameters>
                        </px:PXSelector>
                        <px:PXDateTimeEdit ID="edExpireDate2" runat="server" DataField="ExpireDate" />
                    </RowTemplate>
                </px:PXGridLevel>
            </Levels>
            <AutoSize Enabled="true" />
        </px:PXGrid>
        <px:PXPanel ID="PXPanel2" runat="server" SkinID="Buttons">
            <px:PXButton ID="btnSave" runat="server" DialogResult="OK" Text="OK" />
        </px:PXPanel>
    </px:PXSmartPanel>
    <px:PXSmartPanel ID="PanelAddPOLine" runat="server" Height="370px" HideAfterAction="false" Style="z-index: 108;" Width="1020px"
        Key="poLinesSelection" Caption="Add Purchase Order Line" CaptionVisible="True" LoadOnDemand="true" ShowAfterLoad="true"
        AutoCallBack-Command="Refresh" AutoCallBack-Enabled="True" AutoCallBack-Target="frmPOFilter">
        <px:PXFormView ID="frmPOFilter" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="filter" Caption="PO Selection"
            SkinID="Transparent" CaptionVisible="false">
            <CallbackCommands>
                <Refresh RepaintControls="None" RepaintControlsIDs="gridOL" />
                <Save RepaintControls="None" RepaintControlsIDs="gridOL" />
            </CallbackCommands>
            <Template>
                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                <px:PXDropDown CommitChanges="True" ID="edOrderType" runat="server" AllowNull="False" DataField="OrderType" SelectedIndex="2" />
                <px:PXLayoutRule ID="PXLayoutRule30" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                <px:PXSelector CommitChanges="True" ID="edOrderNbr" runat="server" DataField="OrderNbr" AutoRefresh="True" />
            </Template>
        </px:PXFormView>
        <px:PXGrid ID="gridOL" runat="server" Height="50px" Width="100%" DataSourceID="ds" AutoAdjustColumns="true" AdjustPageSize="Auto" SkinID="Inquire">
            <AutoSize Enabled="true" />
            <Levels>
                <px:PXGridLevel DataMember="poLinesSelection">
                    <Columns>
                        <px:PXGridColumn DataField="Selected" Width="60px" Type="CheckBox" AllowCheckAll="True" AutoCallBack="True" TextAlign="Center" />
                        <px:PXGridColumn AllowUpdate="False" DataField="OrderNbr" DisplayFormat="&gt;CCCCCCCCCCCCCCC" Label="Order Nbr." Width="100px" />
                        <px:PXGridColumn DataField="VendorID" DisplayFormat="&gt;AAAAAAAAAA" Label="Vendor" Width="100px" />
                        <px:PXGridColumn AllowNull="False" DataField="LineType" Width="90px" />
                        <px:PXGridColumn DataField="InventoryID" DisplayFormat="&gt;AAAAAAAAAA" Width="100px" />
                        <px:PXGridColumn DataField="SubItemID" DisplayFormat="&gt;AA-A-A" Width="60px" />
                        <px:PXGridColumn DataField="UOM" Width="63px" DisplayFormat="&gt;aaaaaa" />
                        <px:PXGridColumn DataField="OrderQty" Width="100px" TextAlign="Right" />
                        <px:PXGridColumn DataField="ReceivedQty" Width="108px" TextAlign="Right" />
                        <px:PXGridColumn DataField="LeftToReceiveQty" Width="108px" TextAlign="Right" />
                        <px:PXGridColumn DataField="TranDesc" Width="200px" />
                        <px:PXGridColumn DataField="PromisedDate" TextAlign="Left" Width="80px" />
                        <px:PXGridColumn AllowNull="False" DataField="RcptQtyMin" TextAlign="Right" Width="100px" />
                        <px:PXGridColumn AllowNull="False" DataField="RcptQtyMax" TextAlign="Right" Width="100px" />
                        <px:PXGridColumn AllowNull="False" DataField="RcptQtyAction" Type="DropDownList" Width="100px" />
                        <px:PXGridColumn DataField="LineNbr" TextAlign="Right" Visible="False" />
                    </Columns>
                </px:PXGridLevel>
            </Levels>
            <AutoSize Enabled="true" />
            <CallbackCommands>
                <Save RepaintControls="None" />
            </CallbackCommands>
        </px:PXGrid>
        <px:PXPanel ID="PXPanel3" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton1" runat="server" Text="Add" SyncVisible="false">
                <AutoCallBack Command="AddPOOrderLine2" Target="ds" />
            </px:PXButton>
            <px:PXButton ID="PXButton2" runat="server" DialogResult="OK" Text="Add & Close" />
            <px:PXButton ID="PXButton9" runat="server" DialogResult="No" Text="Cancel" />
        </px:PXPanel>
    </px:PXSmartPanel>
    <px:PXSmartPanel ID="PanelAddPO" runat="server" Style="z-index: 108; height: 396px;" Width="960px" Caption="Add Purchase Order"
        CaptionVisible="True" LoadOnDemand="true" Key="openOrders" AutoCallBack-Command="Refresh" AutoCallBack-Enabled="True"
        AutoCallBack-Target="frmOrderFilter1" AutoRepaint="True">
        <px:PXFormView ID="frmOrderFilter1" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="filter"
            Caption="PO Selection" SkinID="Transparent" CaptionVisible="False">
            <Template>
                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                <px:PXDropDown CommitChanges="True" ID="edOrderType" runat="server" DataField="OrderType" SelectedIndex="2" />
                <px:PXCheckBox CommitChanges="True" ID="chkAnyCurrency" runat="server" Checked="True" DataField="AnyCurrency" />
                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                <px:PXSelector CommitChanges="True" ID="edShipToBAccountID" runat="server" DataField="ShipToBAccountID" AutoRefresh="true" />
                <px:PXSegmentMask CommitChanges="True" ID="edShipToLocationID" runat="server" DataField="ShipToLocationID" AutoRefresh="true" /></Template>
        </px:PXFormView>
        <px:PXGrid ID="grdOpenOrders" runat="server" Height="304px" Width="100%" DataSourceID="ds" Style="border-width: 1px 0px;"
            AutoAdjustColumns="true">
            <AutoSize Enabled="true" />
            <Levels>
                <px:PXGridLevel DataMember="openOrders">
                    <Columns>
                        <px:PXGridColumn AllowCheckAll="True" AllowNull="False" AutoCallBack="True" DataField="Selected" TextAlign="Center" Type="CheckBox"
                            Width="60px" />
                        <px:PXGridColumn DataField="OrderType" />
                        <px:PXGridColumn DataField="OrderNbr" />
                        <px:PXGridColumn DataField="OrderDate" Width="90px" />
                        <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="Status" />
                        <px:PXGridColumn DataField="CuryID" />
                        <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="CuryOrderTotal" Width="100px" TextAlign="Right" />
                        <px:PXGridColumn DataField="VendorRefNbr" />
                        <px:PXGridColumn DataField="TermsID" />
                        <px:PXGridColumn DataField="OrderDesc" Width="200px" />
                        <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="receivedQty" TextAlign="Right" Width="100px" />
                        <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="CuryReceivedCost" TextAlign="Right" Width="100px" />
                        <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="LeftToReceiveQty" TextAlign="Right" Width="100px" />
                        <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="CuryLeftToReceiveCost" TextAlign="Right" Width="100px" />
                    </Columns>
                </px:PXGridLevel>
            </Levels>
        </px:PXGrid>
        <px:PXPanel ID="PXPanel4" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton8" runat="server" Text="Add" SyncVisible="false">
                <AutoCallBack Command="AddPOOrder2" Target="ds" />
            </px:PXButton>
            <px:PXButton ID="PXButton3" runat="server" DialogResult="OK" Text="Add & Close" />
            <px:PXButton ID="PXButton4" runat="server" DialogResult="No" Text="Cancel" />
        </px:PXPanel>
    </px:PXSmartPanel>
    <%-- Recalculate Prices and Discounts --%>
    <px:PXSmartPanel ID="PanelRecalcDiscounts" runat="server" Caption="Recalculate Prices and Discounts" CaptionVisible="true" LoadOnDemand="true" Key="recalcdiscountsfilter"
        AutoCallBack-Enabled="true" AutoCallBack-Target="formRecalcDiscounts" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True"
        CallBackMode-PostData="Page">
        <div style="padding: 5px">
            <px:PXFormView ID="formRecalcDiscounts" runat="server" DataSourceID="ds" CaptionVisible="False" DataMember="recalcdiscountsfilter">
                <Activity Height="" HighlightColor="" SelectedColor="" Width="" />
                <ContentStyle BackColor="Transparent" BorderStyle="None" />
                <Template>
                    <px:PXLayoutRule ID="PXLayoutRule3" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="SM" />
                    <px:PXDropDown ID="edRecalcTerget" runat="server" DataField="RecalcTarget" CommitChanges ="true" />
                    <px:PXCheckBox CommitChanges="True" ID="chkRecalcUnitPrices" runat="server" DataField="RecalcUnitPrices" />
                    <px:PXCheckBox CommitChanges="True" ID="chkOverrideManualPrices" runat="server" DataField="OverrideManualPrices" />
                    <px:PXCheckBox CommitChanges="True" ID="chkRecalcDiscounts" runat="server" DataField="RecalcDiscounts" />
                    <px:PXCheckBox CommitChanges="True" ID="chkOverrideManualDiscounts" runat="server" DataField="OverrideManualDiscounts" />
                </Template>
            </px:PXFormView>
        </div>
        <px:PXPanel ID="PXPanel5" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton10" runat="server" DialogResult="OK" Text="OK" CommandName="RecalcOk" CommandSourceID="ds" />
        </px:PXPanel>
    </px:PXSmartPanel>
    <px:PXSmartPanel ID="PanelAddTransfer" runat="server" Style="z-index: 108; height: 396px;" Width="960px" Caption="Add Transfer Order"
        CaptionVisible="True" LoadOnDemand="true" Key="openTransfers" AutoCallBack-Command="Refresh" AutoCallBack-Enabled="True"
        AutoCallBack-Target="formTransferFilter1" AutoRepaint="True">
        <px:PXFormView ID="formTransferFilter1" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="filter"
            Caption="PO Selection" SkinID="Transparent" CaptionVisible="False">
            <Template>
                <px:PXLayoutRule ID="PXLayoutRule2" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                <px:PXSegmentMask CommitChanges="True" ID="edShipFromSiteID" runat="server" DataField="ShipFromSiteID" AutoRefresh="true" />
			</Template>
        </px:PXFormView>
        <px:PXGrid ID="gridOpenTransfers" runat="server" Height="304px" Width="100%" DataSourceID="ds" Style="border-width: 1px 0px;"
            AutoAdjustColumns="true">
            <AutoSize Enabled="true" />
            <Levels>
                <px:PXGridLevel DataMember="openTransfers">
                    <Columns>
                        <px:PXGridColumn AllowCheckAll="True" AllowNull="False" AutoCallBack="True" DataField="Selected" TextAlign="Center" Type="CheckBox" AllowResize="False" Width="20px" />
                        <px:PXGridColumn DataField="OrderType" Width="90px" />
                        <px:PXGridColumn DataField="OrderNbr" Width="100px" />
                        <px:PXGridColumn DataField="ShipmentNbr" Width="110px" />
						<px:PXGridColumn DataField="INRegister__SiteID" Width="120px" />
						<px:PXGridColumn DataField="INRegister__ToSiteID" Width="110px" />
						<px:PXGridColumn DataField="INRegister__TranDate" Width="80px" />
						<px:PXGridColumn DataField="INRegister__TranDesc" Width="200px" />
                    </Columns>
                </px:PXGridLevel>
            </Levels>
        </px:PXGrid>
        <px:PXPanel ID="PXPanel6" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton11" runat="server" Text="Add" >
                <AutoCallBack Command="AddTransfer2" Target="ds" />
            </px:PXButton>
            <px:PXButton ID="PXButton12" runat="server" DialogResult="OK" Text="Add & Close" />
            <px:PXButton ID="PXButton13" runat="server" DialogResult="No" Text="Cancel" />
        </px:PXPanel>
    </px:PXSmartPanel>
    <px:PXSmartPanel ID="PanelAddINTran" runat="server" Height="370px" HideAfterAction="false" Style="z-index: 108;" Width="1020px"
        Key="intranSelection" Caption="Add Transfer Line" CaptionVisible="True" LoadOnDemand="true" ShowAfterLoad="true"
        AutoCallBack-Command="Refresh" AutoCallBack-Enabled="True" AutoCallBack-Target="frmTransferFilter">
        <px:PXFormView ID="frmTransferFilter" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="filter" Caption="Transfer Selection"
            SkinID="Transparent" CaptionVisible="false">
            <CallbackCommands>
                <Refresh RepaintControls="None" RepaintControlsIDs="gridOL" />
                <Save RepaintControls="None" RepaintControlsIDs="gridOL" />
            </CallbackCommands>
            <Template>
                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                <px:PXSegmentMask CommitChanges="True" ID="edShipFromSiteID" runat="server"  AutoRefresh="True" DataField="ShipFromSiteID" SelectedIndex="2" />
                <px:PXLayoutRule ID="PXLayoutRule30" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                <px:PXSelector CommitChanges="True" ID="edSOOrderNbr" runat="server" DataField="SOOrderNbr" AutoRefresh="True" />
            </Template>
        </px:PXFormView>
        <px:PXGrid ID="PXGrid1" runat="server" Height="240px" Width="100%" DataSourceID="ds" Style="border-width: 1px 0px" AutoAdjustColumns="true">
            <AutoSize Enabled="true" />
            <Levels>
                <px:PXGridLevel DataMember="intranSelection">
                    <Columns>
                        <px:PXGridColumn DataField="Selected" Width="60px" Type="CheckBox" AllowCheckAll="True" AutoCallBack="True" TextAlign="Center" />
                        <px:PXGridColumn AllowUpdate="False" DataField="RefNbr" DisplayFormat="&gt;CCCCCCCCCCCCCCC" Label="Order Nbr." Width="100px" />
                        <px:PXGridColumn DataField="InventoryID" DisplayFormat="&gt;AAAAAAAAAA" Width="100px" />
                        <px:PXGridColumn DataField="SubItemID" DisplayFormat="&gt;AA-A-A" Width="60px" />
                        <px:PXGridColumn DataField="UOM" Width="63px" DisplayFormat="&gt;aaaaaa" />
                        <px:PXGridColumn DataField="Qty" Width="100px" TextAlign="Right" />
                        <px:PXGridColumn DataField="TranDesc" Width="200px" />
                        <px:PXGridColumn DataField="LineNbr" TextAlign="Right" Visible="False" />
                    </Columns>
                </px:PXGridLevel>
            </Levels>
            <AutoSize Enabled="true" />
            <CallbackCommands>
                <Save RepaintControls="None" />
            </CallbackCommands>
        </px:PXGrid>
        <px:PXPanel ID="PXPanel7" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton15" runat="server" DialogResult="OK" Text="Add & Close" />
            <px:PXButton ID="PXButton16" runat="server" DialogResult="No" Text="Cancel" />
        </px:PXPanel>
    </px:PXSmartPanel>
</asp:Content>
