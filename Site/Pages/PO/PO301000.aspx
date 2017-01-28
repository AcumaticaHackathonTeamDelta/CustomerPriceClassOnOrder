<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="PO301000.aspx.cs"
    Inherits="Page_PO301000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" TypeName="PX.Objects.PO.POOrderEntry" PrimaryView="Document" Width="100%">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Insert" PostData="Self" />
			<px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
			<px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="True" />
			<px:PXDSCallbackCommand Name="Last" PostData="Self" />
			<px:PXDSCallbackCommand StartNewGroup="True" Name="Action" />
			<px:PXDSCallbackCommand Visible="false" Name="Hold" CommitChanges="true" />
			<px:PXDSCallbackCommand Visible="False" Name="NewVendor" />
			<px:PXDSCallbackCommand Visible="False" Name="EditVendor" />
			<px:PXDSCallbackCommand Visible="false" Name="AddPOOrder" CommitChanges="true" />
			<px:PXDSCallbackCommand Visible="false" Name="AddPOOrderLine" CommitChanges="true" />
			<px:PXDSCallbackCommand Visible="false" Name="CreatePOReceipt" CommitChanges="true" />
			<px:PXDSCallbackCommand Visible="False" Name="CurrencyView" />
			<px:PXDSCallbackCommand Visible="False" Name="CurrencyView" />
			<px:PXDSCallbackCommand Name="AddInvBySite" Visible="False" CommitChanges="true" />
			<px:PXDSCallbackCommand Name="AddInvSelBySite" Visible="False" CommitChanges="true" />
			<px:PXDSCallbackCommand Name="ViewDemand" Visible="false" DependOnGrid="grid" />
			<px:PXDSCallbackCommand Name="NewTask" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="NewEvent" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="NewActivity" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="NewMailActivity" Visible="False" CommitChanges="True" PopupCommand="Cancel" PopupCommandTarget="ds" />
			<px:PXDSCallbackCommand StartNewGroup="True" Name="ValidateAddresses" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="RecalculateDiscountsAction" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="RecalcOk" PopupCommand="" PopupCommandTarget="" PopupPanel="" Text="" Visible="False" />       
            <px:PXDSCallbackCommand Name="CreatePrepayment" Visible="False" CommitChanges="True" PopupCommand="Cancel" PopupCommandTarget="ds" />   
		</CallbackCommands>
		<DataTrees>
			<px:PXTreeDataMember TreeView="_EPCompanyTree_Tree_" TreeKeys="WorkgroupID" />
		</DataTrees>
	</px:PXDataSource>
    <px:PXSmartPanel ID="PanelAddPOLine" runat="server" Height="370px" Style="z-index: 108; position: absolute; left: 660px;
        top: 99px" Width="960px" Key="poLinesSelection" Caption="Add Purchase Order Line" CaptionVisible="True" LoadOnDemand="true"
        ShowAfterLoad="true">
        <px:PXFormView ID="frmPOFilter" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="filter" Caption="PO Selection"
            CaptionVisible="false" SkinID="Transparent">
            <Template>
                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                <px:PXDropDown CommitChanges="True" ID="edOrderType" runat="server" AllowNull="False" DataField="OrderType" SelectedIndex="2" />
                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                <px:PXSelector CommitChanges="True" ID="edOrderNbr" runat="server" DataField="OrderNbr" AutoRefresh="True" />
            </Template>
        </px:PXFormView>
        <px:PXGrid ID="gridOL" runat="server" Height="240px" Width="100%" DataSourceID="ds" Style="border-width: 1px 0px" AutoAdjustColumns="true">
            <Levels>
                <px:PXGridLevel DataMember="poLinesSelection">
                    <Columns>
                        <px:PXGridColumn DataField="Selected" Width="60px" Type="CheckBox" AllowCheckAll="True" />
                        <px:PXGridColumn AllowNull="False" DataField="LineType" Width="90px" />
                        <px:PXGridColumn DataField="InventoryID" DisplayFormat="&gt;AAAAAAAAAA" />
                        <px:PXGridColumn DataField="SubItemID" DisplayFormat="&gt;AA-A-A" Width="60px" />
                        <px:PXGridColumn DataField="UOM" Width="63px" DisplayFormat="&gt;aaaaaa" />
                        <px:PXGridColumn DataField="OrderQty" Width="100px" TextAlign="Right" />
                        <px:PXGridColumn DataField="LeftToReceiveQty" Width="108px" TextAlign="Right" />
                        <px:PXGridColumn DataField="TranDesc" Width="200px" />
                        <px:PXGridColumn AllowNull="False" DataField="RcptQtyMin" TextAlign="Right" Width="100px" />
                        <px:PXGridColumn AllowNull="False" DataField="RcptQtyMax" TextAlign="Right" Width="100px" />
                        <px:PXGridColumn AllowNull="False" DataField="RcptQtyAction" Type="DropDownList" Width="100px" />
                    </Columns>
                    <Mode AllowAddNew="false" AllowUpdate="false" AllowDelete="false" />
                </px:PXGridLevel>
            </Levels>
            <AutoSize Enabled="true" />
        </px:PXGrid>
        <px:PXPanel ID="PXPanel1" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton1" runat="server" DialogResult="OK" Text="Save" />
            <px:PXButton ID="PXButton2" runat="server" DialogResult="No" Text="Cancel" />
        </px:PXPanel>
    </px:PXSmartPanel>
    <px:PXSmartPanel ID="PanelAddPO" runat="server" Height="415px" Style="z-index: 108; left: 486px; position: absolute; top: 99px"
        Width="960px" Caption="Add Purchase Order" CaptionVisible="True" LoadOnDemand="true" Key="openOrders" ShowAfterLoad="true"
        AutoCallBack-Enabled="True" AutoCallBack-Target="grdOpenOrders" AutoCallBack-Command="Refresh">
        <px:PXGrid ID="grdOpenOrders" runat="server" Height="340px" Width="100%" DataSourceID="ds" Style="border-width: 1px 0px;
            left: 0px; top: 2px;" AutoAdjustColumns="true">
            <AutoSize Enabled="true" />
            <Levels>
                <px:PXGridLevel DataMember="openOrders">
                    <Columns>
                        <px:PXGridColumn AllowCheckAll="True" AllowNull="False" AutoCallBack="True" DataField="Selected" TextAlign="Center" Type="CheckBox"
                            Width="60px" />
                        <px:PXGridColumn DataField="OrderType" />
                        <px:PXGridColumn DataField="OrderNbr" />
                        <px:PXGridColumn DataField="OrderDate" Width="90px" />
                        <px:PXGridColumn DataField="ExpirationDate" Width="90px" />
                        <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="Status" />
                        <px:PXGridColumn DataField="CuryID" />
                        <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="CuryOrderTotal" Width="100px" TextAlign="Right" />
                        <px:PXGridColumn DataField="VendorRefNbr" />
                        <px:PXGridColumn DataField="TermsID" />
                        <px:PXGridColumn DataField="OrderDesc" Width="200px" />
                        <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="LeftToReceiveQty" TextAlign="Right" Width="100px" />
                        <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="CuryLeftToReceiveCost" TextAlign="Right" Width="100px" />
                    </Columns>
                </px:PXGridLevel>
            </Levels>
        </px:PXGrid>
        <px:PXPanel ID="PXPanel2" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton3" runat="server" DialogResult="OK" Text="Save" />
            <px:PXButton ID="PXButton4" runat="server" DialogResult="No" Text="Cancel" />
        </px:PXPanel>
    </px:PXSmartPanel>
    <px:PXSmartPanel ID="PanelFixedDemand" runat="server" Height="415px" Style="z-index: 108; left: 486px; position: absolute;
        top: 99px" Width="960px" Caption="Demand" CaptionVisible="True" LoadOnDemand="true" Key="FixedDemand" ShowAfterLoad="true"
        AutoCallBack-Enabled="True" AutoCallBack-Target="gridFixedDemand" AutoCallBack-Command="Refresh">
        <px:PXGrid ID="gridFixedDemand" runat="server" Height="340px" Width="100%" DataSourceID="ds" Style="border-width: 1px 0px;
            left: 0px; top: 2px;" AutoAdjustColumns="true">
            <AutoSize Enabled="true" />
            <Levels>
                <px:PXGridLevel DataMember="FixedDemand">
                    <Columns>
                        <px:PXGridColumn DataField="OrderType" />
                        <px:PXGridColumn DataField="OrderNbr" />
                        <px:PXGridColumn DataField="RequestDate" Width="90px" />
                        <px:PXGridColumn DataField="CustomerID" Width="90px" />
                        <px:PXGridColumn DataField="SiteID" Width="90px" />
                        <px:PXGridColumn AllowUpdate="False" DataField="UOM" DisplayFormat="&gt;aaaaaa" Label="Orig. UOM" />
                        <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="OrderQty" Label="Orig. Quantity" TextAlign="Right" Width="100px" />
                        <px:PXGridColumn AllowUpdate="False" DataField="POUOM" DisplayFormat="&gt;aaaaaa" Label="UOM" />
                        <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="POUOMOrderQty" Label="Quantity" TextAlign="Right" Width="100px" />
                        <px:PXGridColumn DataField="INItemPlan__Active" Width="60px" AllowNull="False" TextAlign="Center" Type="CheckBox" />
                    </Columns>
					<RowTemplate>
						<px:PXSelector ID="edOrderNbr" DataField="OrderNbr" runat="server" AllowEdit="True" />
					</RowTemplate>
                </px:PXGridLevel>
            </Levels>
        </px:PXGrid>
        <px:PXPanel ID="PXPanel3" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton5" runat="server" DialogResult="Cancel" Text="Close" />
        </px:PXPanel>
    </px:PXSmartPanel>
    <px:PXSmartPanel ID="PanelReplenishment" runat="server" Height="415px" Style="z-index: 108; left: 486px; position: absolute;
        top: 99px" Width="960px" Caption="Demand" CaptionVisible="True" LoadOnDemand="True" Key="ReplenishmentLines" ShowAfterLoad="True"
        AutoCallBack-Target="gridReplenishmentLines" AutoCallBack-Command="Refresh">
        <px:PXGrid ID="gridReplenishmentLines" runat="server" Height="340px" Width="100%" DataSourceID="ds" Style="border-width: 1px 0px;
            left: 0px; top: 2px;" AutoAdjustColumns="true">
            <AutoSize Enabled="true" />
            <Levels>
                <px:PXGridLevel DataMember="ReplenishmentLines">
                    <Columns>
                        <px:PXGridColumn DataField="RefNbr" />
                        <px:PXGridColumn DataField="OrderDate" Width="90px" />
                        <px:PXGridColumn DataField="UOM" Width="90px" />
                        <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="Qty" TextAlign="Right" Width="100px" />
                    </Columns>
                </px:PXGridLevel>
            </Levels>
        </px:PXGrid>
        <px:PXPanel ID="PXPanel4" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton9" runat="server" DialogResult="Cancel" Text="Close"/>
        </px:PXPanel>
    </px:PXSmartPanel>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Document" Caption="Document Summary"
        NoteIndicator="True" FilesIndicator="True" LinkIndicator="true" ActivityIndicator="true" ActivityField="NoteActivity"
        emailinggraph="PX.Objects.CR.CREmailActivityMaint,PX.Objects" DefaultControlID="edOrderType" NotifyIndicator="True">
        <CallbackCommands>
            <Save PostData="Self" />
        </CallbackCommands>
        <Activity HighlightColor="" SelectedColor="" Width="" Height=""></Activity>
        <Parameters>
            <px:PXControlParam ControlID="form" Name="POOrder.orderType" PropertyName="NewDataKey[&quot;OrderType&quot;]" Type="String" />
        </Parameters>
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="S" />
            <px:PXDropDown ID="edOrderType" runat="server" DataField="OrderType" SelectedIndex="-1"/>
            <px:PXSelector ID="edOrderNbr" runat="server" DataField="OrderNbr" AutoRefresh="true">
                <GridProperties FastFilterFields="VendorRefNbr">
                    <PagerSettings Mode="NextPrevFirstLast" />
                </GridProperties>
            </px:PXSelector>
            <px:PXCheckBox ID="chkHold" runat="server" DataField="Hold">
                <AutoCallBack Command="Hold" Target="ds">
                </AutoCallBack>
            </px:PXCheckBox>
            <px:PXDropDown ID="edStatus" runat="server" AllowNull="False" DataField="Status" Enabled="False" />
            <px:PXCheckBox ID="chkRequestApproval" runat="server" DataField="RequestApproval" />                    
            <px:PXCheckBox ID="chkApproved" runat="server" DataField="Approved" />
            <px:PXDateTimeEdit CommitChanges="True" ID="edOrderDate" runat="server" DataField="OrderDate" />
            <px:PXDateTimeEdit CommitChanges="True" ID="edExpectedDate" runat="server" DataField="ExpectedDate" />
            <px:PXDateTimeEdit ID="edExpirationDate" runat="server" DataField="ExpirationDate" />
            <px:PXLayoutRule runat="server" ColumnSpan="2" />
            <px:PXTextEdit ID="edOrderDesc" runat="server" DataField="OrderDesc" />
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
            <px:PXSegmentMask CommitChanges="True" ID="edVendorID" runat="server" DataField="VendorID" AllowAddNew="True" AllowEdit="True" AutoRefresh="True" />
            <px:PXSegmentMask CommitChanges="True" ID="edVendorLocationID" runat="server" AutoRefresh="True" DataField="VendorLocationID" />
            <px:PXSegmentMask CommitChanges="True" ID="edEmployeeID" runat="server" DataField="EmployeeID" />
            <pxa:PXCurrencyRate ID="edCury" DataField="CuryID" runat="server" DataSourceID="ds" RateTypeView="_POOrder_CurrencyInfo_"
                DataMember="_Currency_" />
            <px:PXTextEdit CommitChanges="True" ID="edVendorRefNbr" runat="server" DataField="VendorRefNbr" />
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
            <px:PXNumberEdit ID="edCuryLineTotal" runat="server" DataField="CuryLineTotal" Enabled="False" />
            <px:PXNumberEdit ID="edCuryDiscTot" runat="server" Enabled="False" DataField="CuryDiscTot" />
            <px:PXNumberEdit ID="edCuryVatExemptTotal" runat="server" DataField="CuryVatExemptTotal" Enabled="False" />
            <px:PXNumberEdit ID="edCuryVatTaxableTotal" runat="server" DataField="CuryVatTaxableTotal" Enabled="False" />
            <px:PXNumberEdit ID="edCuryTaxTotal" runat="server" DataField="CuryTaxTotal" Enabled="False" />
            <px:PXNumberEdit ID="edCuryOrderTotal" runat="server" DataField="CuryOrderTotal" Enabled="False" />
            <px:PXNumberEdit CommitChanges="True" ID="edCuryControlTotal" runat="server" DataField="CuryControlTotal" />
        </Template>
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
                    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100; left: 0px; top: 0px; height: 384px;" Width="100%"
                        BorderWidth="0px" SkinID="Details" SyncPosition="True" Height="384px" TabIndex="2500">
                        <Levels>
                            <px:PXGridLevel DataMember="Transactions">
                                <Columns>
                                    <px:PXGridColumn DataField="BranchID" DisplayFormat="&gt;AAAAAAAAAA" Width="81px" RenderEditorText="True"
                                        AllowShowHide="Server" CommitChanges="True" />
                                    <px:PXGridColumn DataField="InventoryID" DisplayFormat="&gt;AAAAAAAAAA" Width="81px" CommitChanges="True" />
                                    <px:PXGridColumn DataField="SubItemID" DisplayFormat="&gt;AA-A-A" CommitChanges="True" />
                                    <px:PXGridColumn AllowNull="False" DataField="LineType" Type="DropDownList" CommitChanges="True" />
                                    <px:PXGridColumn DataField="SiteID" DisplayFormat="&gt;AAAAAAAAAA" CommitChanges="True" />
                                    <px:PXGridColumn DataField="TranDesc" Width="180px" />
                                    <px:PXGridColumn DataField="UOM" Width="54px" DisplayFormat="&gt;aaaaaa" CommitChanges="True" />
                                    <px:PXGridColumn AllowNull="False" DataField="OrderQty" TextAlign="Right" Width="81px" CommitChanges="True" />
                                    <px:PXGridColumn AllowNull="False" DataField="ReceivedQty" TextAlign="Right" Width="100px" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryUnitCost" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryLineAmt" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn DataField="DiscPct" TextAlign="Right" />
									<px:PXGridColumn DataField="CuryDiscAmt" TextAlign="Right" />
                                    <px:PXGridColumn DataField="CuryDiscCost" TextAlign="Right" />
									<px:PXGridColumn DataField="ManualDisc" TextAlign="Center" Type="CheckBox" />
                                    <px:PXGridColumn DataField="DiscountID" RenderEditorText="True" TextAlign="Left" AllowShowHide="Server" Width="90px" CommitChanges="True" />
                                    <px:PXGridColumn DataField="DiscountSequenceID" TextAlign="Left" Width="90px" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryExtCost" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="CuryReceivedCost" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn DataField="AlternateID" Width="180px" />
                                    <px:PXGridColumn DataField="LotSerialNbr" AllowShowHide="Server" Width="120px" />
                                    <px:PXGridColumn AllowNull="False" DataField="RcptQtyMin" TextAlign="Right" Width="100px" />
                                    <px:PXGridColumn AllowNull="False" DataField="RcptQtyMax" TextAlign="Right" Width="100px" />
                                    <px:PXGridColumn DataField="RcptQtyThreshold" TextAlign="Right" Width="120px" />
                                    <px:PXGridColumn AllowNull="False" DataField="RcptQtyAction" Width="108px" Type="DropDownList" />
                                    <px:PXGridColumn DataField="TaxCategoryID" DisplayFormat="&gt;aaaaaaaaaa" Width="54px" />
                                    <px:PXGridColumn DataField="ExpenseAcctID" DisplayFormat="&gt;######" CommitChanges="True" />
                                    <px:PXGridColumn DataField="ExpenseAcctID_Account_description" Width="120px" />
                                    <px:PXGridColumn DataField="ExpenseSubID" DisplayFormat="&gt;AA-AA-AA-AA-AAA" />
                                    <px:PXGridColumn DataField="ProjectID" Label="Project" Width="108px" CommitChanges="True" />
                                    <px:PXGridColumn DataField="TaskID" DisplayFormat="&gt;AAAAAAAAAA" Label="Task" Width="81px" CommitChanges="True" />
                                    <px:PXGridColumn DataField="OrderType" Visible="False" />
                                    <px:PXGridColumn DataField="LineNbr" TextAlign="Right" Visible="False" />
                                    <px:PXGridColumn DataField="RequestedDate" Width="90px" />
                                    <px:PXGridColumn DataField="PromisedDate" Width="90px" />
                                    <px:PXGridColumn AllowNull="False" DataField="Completed" TextAlign="Center" Type="CheckBox" Width="60px" />
                                    <px:PXGridColumn AllowNull="False" DataField="Cancelled" TextAlign="Center" Type="CheckBox" Width="60px" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="POType" RenderEditorText="True" Width="72px" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="PONbr" DisplayFormat="&gt;CCCCCCCCCCCCCCC" Width="117px" />
                                </Columns>
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                                    <px:PXSegmentMask CommitChanges="True" ID="edInventoryID" runat="server" DataField="InventoryID" AllowEdit="True" 
                                        AutoRefresh="True">
                                        <Parameters>
                                            <px:PXControlParam ControlID="grid" Name="POLine.lineType" PropertyName="DataValues[&quot;LineType&quot;]" Type="String" />
                                        </Parameters>
                                    </px:PXSegmentMask>
                                    <px:PXSegmentMask CommitChanges="True" ID="edSubItemID" runat="server" DataField="SubItemID">
                                        <Parameters>
                                            <px:PXControlParam ControlID="grid" Name="POLine.inventoryID" PropertyName="DataValues[&quot;InventoryID&quot;]" Type="String" />
                                        </Parameters>
                                    </px:PXSegmentMask>
                                    <px:PXSegmentMask CommitChanges="True" ID="edSiteID" runat="server" DataField="SiteID" AutoRefresh="True" />
                                    <px:PXDropDown CommitChanges="True" ID="edLineType" runat="server" AllowNull="False" DataField="LineType" />
                                    <px:PXSelector CommitChanges="True" ID="edUOM" runat="server" DataField="UOM" AutoRefresh="True">
                                        <Parameters>
                                            <px:PXSyncGridParam ControlID="grid" />
                                        </Parameters>
                                    </px:PXSelector>
                                    <px:PXNumberEdit ID="edOrderQty" runat="server" DataField="OrderQty" />
                                    <px:PXNumberEdit ID="edReceivedQty" runat="server" DataField="ReceivedQty" Enabled="False" />
                                    <px:PXNumberEdit ID="edCuryUnitCost" runat="server" DataField="CuryUnitCost" />
                                    <px:PXSelector ID="edDiscountCode" runat="server" DataField="DiscountID" CommitChanges="True" AllowEdit="True" edit="1" />
                                    <px:PXNumberEdit ID="edDiscPct" runat="server" DataField="DiscPct" />
									<px:PXNumberEdit ID="edCuryDiscAmt" runat="server" DataField="CuryDiscAmt" />
									<px:PXCheckBox ID="chkManualDisc" runat="server" DataField="ManualDisc" CommitChanges="True" />
                                    <px:PXNumberEdit ID="edCuryLineAmt" runat="server" DataField="CuryLineAmt" />
                                    <px:PXNumberEdit ID="edCuryExtCost" runat="server" DataField="CuryExtCost" />
                                    <px:PXNumberEdit ID="edCuryReceivedCost" runat="server" DataField="CuryReceivedCost" Enabled="False" />
                                    <px:PXLayoutRule runat="server" ColumnSpan="2" />
                                    <px:PXTextEdit ID="edTranDesc" runat="server" DataField="TranDesc" />

                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="S" />
                                    <px:PXNumberEdit ID="edRcptQtyMin" runat="server" DataField="RcptQtyMin" />
                                    <px:PXNumberEdit ID="edRcptQtyMax" runat="server" DataField="RcptQtyMax" />
                                    <px:PXNumberEdit ID="edRcptQtyThreshold" runat="server" DataField="RcptQtyThreshold" />
                                    <px:PXDropDown ID="edRcptQtyAction" runat="server" DataField="RcptQtyAction" />
                                    <px:PXCheckBox ID="chkCompleted" runat="server" DataField="Completed" />
                                    <px:PXCheckBox ID="chkCancelled" runat="server" DataField="Cancelled" />

                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
                                    <px:PXSegmentMask CommitChanges="True" Height="19px" ID="edBranchID" runat="server" DataField="BranchID" />
                                    <px:PXSegmentMask CommitChanges="True" ID="edExpenseAcctID" runat="server" DataField="ExpenseAcctID" 
                                        AutoRefresh="True" />
                                    <px:PXSegmentMask ID="edExpenseSubID" runat="server" DataField="ExpenseSubID" AutoRefresh="True">
                                        <Parameters>
                                            <px:PXSyncGridParam ControlID="grid" />
                                        </Parameters>
                                    </px:PXSegmentMask>
                                    <px:PXDateTimeEdit ID="edRequestedDate" runat="server" DataField="RequestedDate" />
                                    <px:PXDateTimeEdit ID="edPromisedDate" runat="server" DataField="PromisedDate" />
                                    <px:PXSelector ID="edTaxCategoryID" runat="server" DataField="TaxCategoryID" AutoRefresh="True"/>
                                    <px:PXTextEdit ID="edAlternateID" runat="server" DataField="AlternateID" />
                                    <px:PXSegmentMask CommitChanges="True" ID="edProjectID" runat="server" DataField="ProjectID" />
                                    <px:PXSegmentMask CommitChanges="True" ID="edTaskID" runat="server" DataField="TaskID" AutoRefresh="True">
                                        <Parameters>
                                            <px:PXSyncGridParam ControlID="grid" />
                                        </Parameters>
                                    </px:PXSegmentMask>
                                    <px:PXDropDown ID="edPOType" runat="server" DataField="POType" />
                                    <px:PXSelector ID="edPONbr" runat="server" DataField="PONbr" AllowEdit="True" />
                                </RowTemplate>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" MinHeight="150" />
                        <Mode InitNewRow="True" AllowFormEdit="True" AllowUpload="True" />
                        <ActionBar>
                            <CustomItems>
                                <px:PXToolBarButton Text="Add Item" Key="cmdASI">
                                    <AutoCallBack Command="AddInvBySite" Target="ds">
                                        <Behavior PostData="Page" CommitChanges="True" />
                                    </AutoCallBack>
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Text="View Demand">
                                    <AutoCallBack Command="ViewDemand" Target="ds">
                                        <Behavior PostData="Page" CommitChanges="True" />
                                    </AutoCallBack>
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Text="Add Order" Key="cmdPO" CommandSourceID="ds" CommandName="AddPOOrder" />
                                <px:PXToolBarButton Text="Add Order Line" Key="cmdAddPOLine" CommandSourceID="ds" CommandName="AddPOOrderLine" />                                                                
                            </CustomItems>
                        </ActionBar>
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Tax Details">
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
                                    <px:PXGridColumn DataField="TaxID" Width="81px" AllowUpdate="False" />
                                    <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="TaxRate" TextAlign="Right" Width="81px" />
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
            <px:PXTabItem Text="Shipping Instructions">
                <Template>
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                    <px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Ship To:" />
                    <px:PXDropDown CommitChanges="True" ID="edShipDestType" runat="server" AllowNull="False" DataField="ShipDestType" 
                        SuppressLabel="False" />
                    <px:PXSelector CommitChanges="True" ID="edShipToBAccountID" runat="server" DataField="ShipToBAccountID" 
                        AutoRefresh="True"  SuppressLabel="False"/>

                    <px:PXSegmentMask CommitChanges="True" SuppressLabel="False" ID="edSiteID" runat="server" DataField="SiteID" />

                    <px:PXSegmentMask CommitChanges="True" ID="edShipToLocationID" runat="server" AutoRefresh="True" DataField="ShipToLocationID" />
                    <px:PXFormView ID="formSC" runat="server" CaptionVisible="False" DataMember="Shipping_Contact" DataSourceID="ds" 
                        RenderStyle="Simple">
                        <Template>
                            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                            <px:PXCheckBox CommitChanges="True" SuppressLabel="True" ID="chkOverrideContact" runat="server" DataField="OverrideContact" />
                            <px:PXTextEdit ID="edSalutation" runat="server" DataField="Salutation" />
                            <px:PXTextEdit ID="edFullName" runat="server" DataField="FullName" />
                            <px:PXTextEdit ID="edPhone1" runat="server" DataField="Phone1" />
                            <px:PXMailEdit ID="edEmail" runat="server" DataField="Email" CommitChanges="True"/>
                        </Template>
                        <ContentStyle BackColor="Transparent" BorderStyle="None">
                        </ContentStyle>
                        <AutoSize MinWidth="100" />
                    </px:PXFormView>
                    <px:PXFormView ID="formSA" DataMember="Shipping_Address" runat="server" DataSourceID="ds" CaptionVisible="False" 
                        SyncPosition="True" RenderStyle="Simple">
                        <AutoSize MinHeight="100" MinWidth="100" />
                        <Template>
                            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                            <px:PXLayoutRule runat="server" Merge="True" />
                            <px:PXCheckBox CommitChanges="True" SuppressLabel="True" ID="chkOverrideAddress" runat="server" DataField="OverrideAddress"
                                Height="18px" />
                            <px:PXCheckBox ID="chkIsValidated" runat="server" DataField="IsValidated" Enabled="False"/>
                            <px:PXLayoutRule runat="server" />
                            <px:PXTextEdit ID="edAddressLine1" runat="server" DataField="AddressLine1" />
                            <px:PXTextEdit ID="edAddressLine2" runat="server" DataField="AddressLine2" />
                            <px:PXTextEdit ID="edCity" runat="server" DataField="City" />
                            <px:PXSelector ID="edCountryID" runat="server" DataField="CountryID" AutoRefresh="True" DataSourceID="ds" CommitChanges="true" />
                            <px:PXSelector ID="edState" runat="server" DataField="State" AutoRefresh="True" DataSourceID="ds">
                                <CallBackMode PostData="Container" />
                                <Parameters>
                                    <px:PXControlParam ControlID="formSA" Name="POShipAddress.countryID" PropertyName="DataControls[&quot;edCountryID&quot;].Value"
                                        Type="String" />
                                </Parameters>
                            </px:PXSelector>
                            <px:PXMaskEdit ID="edPostalCode" runat="server" DataField="PostalCode" CommitChanges="true" />
                        </Template>
                        <ContentStyle BackColor="Transparent" BorderStyle="None" />
                    </px:PXFormView>
                    <px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" StartColumn="True" />
                    <px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Ship Via:" />
                    <px:PXSelector ID="edFOBPoint" runat="server" DataField="FOBPoint" />
                    <px:PXSelector ID="edShipVia" runat="server" DataField="ShipVia" />
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Vendor Info">
                <Template>
                    <px:PXLayoutRule runat="server" StartColumn="True" SuppressLabel="True" />
                    <px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Vendor Address" />
                    <px:PXFormView ID="formVC" runat="server" CaptionVisible="False" DataMember="Remit_Contact" DataSourceID="ds" 
                                   RenderStyle="Simple">
                        <Template>
                            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                            <px:PXCheckBox CommitChanges="True" SuppressLabel="True" ID="chkOverrideContact" runat="server" DataField="OverrideContact" />
                            <px:PXTextEdit ID="edFullName" runat="server" DataField="FullName" />
                            <px:PXTextEdit ID="edSalutation" runat="server" DataField="Salutation" />
                            <px:PXTextEdit ID="edPhone1" runat="server" DataField="Phone1" />
                            <px:PXMailEdit ID="edEmail" runat="server" DataField="Email" CommitChanges="True"/>
                        </Template>
                        <ContentStyle BackColor="Transparent" BorderStyle="None" />
                    </px:PXFormView>
                    <px:PXFormView ID="formVA" DataMember="Remit_Address" runat="server" DataSourceID="ds" SyncPosition="True" RenderStyle="Simple">
                        <Template>
                            <px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="XM" />
                            <px:PXLayoutRule runat="server" Merge="True" />
                            <px:PXCheckBox CommitChanges="True" SuppressLabel="True" ID="chkOverrideAddress" runat="server" DataField="OverrideAddress" />
                            <px:PXCheckBox ID="chkIsValidated" runat="server" DataField="IsValidated" Enabled="False"/>
                            <px:PXLayoutRule runat="server" />
                            <px:PXTextEdit ID="edAddressLine1" runat="server" DataField="AddressLine1" />
                            <px:PXTextEdit ID="edAddressLine2" runat="server" DataField="AddressLine2" />
                            <px:PXTextEdit ID="edCity" runat="server" DataField="City" />
                            <px:PXSelector ID="edCountryID" runat="server" DataField="CountryID" AutoRefresh="True" DataSourceID="ds" CommitChanges="true" />
                            <px:PXSelector ID="edState" runat="server" DataField="State" AutoRefresh="True" DataSourceID="ds">
                                <CallBackMode PostData="Container" />
                                <Parameters>
                                    <px:PXControlParam ControlID="formVA" Name="PORemitAddress.countryID" PropertyName="DataControls[&quot;edCountryID&quot;].Value"
                                                       Type="String" />
                                </Parameters>
                            </px:PXSelector>
                            <px:PXMaskEdit ID="edPostalCode" runat="server" DataField="PostalCode" CommitChanges="true" />
                        </Template>
                        <ContentStyle BackColor="Transparent" BorderStyle="None" />
                    </px:PXFormView>
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                    <px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Info" />
                    <px:PXSelector CommitChanges="True" ID="edTermsID" runat="server" DataField="TermsID" />
                    <px:PXSelector CommitChanges="True" ID="edTaxZoneID" runat="server" DataField="TaxZoneID" Text="ZONE1" />
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Approval Details" BindingContext="form" VisibleExp="DataControls[&quot;chkRequestApproval&quot;].Value = 1">
                <Template>
                    <px:PXGrid ID="gridApproval" runat="server" DataSourceID="ds" Width="100%" SkinID="DetailsInTab" NoteIndicator="True" Style="left: 0px;
                        top: 0px;">
                        <AutoSize Enabled="True" />
                        <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
                        <Levels>
                            <px:PXGridLevel DataMember="Approval">
                                <Columns>
                                    <px:PXGridColumn DataField="ApproverEmployee__AcctCD" Width="160px" />
                                    <px:PXGridColumn DataField="ApproverEmployee__AcctName" Width="160px" />
                                    <px:PXGridColumn DataField="ApprovedByEmployee__AcctCD" Width="100px" />
                                    <px:PXGridColumn DataField="ApprovedByEmployee__AcctName" Width="160px" />
                                    <px:PXGridColumn DataField="ApproveDate" Width="90px" />
                                    <px:PXGridColumn DataField="Status" AllowNull="False" AllowUpdate="False" RenderEditorText="True" />
                                    <px:PXGridColumn DataField="WorkgroupID" Width="150px" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Discount Details">
                <Template>
                    <px:PXGrid ID="formDiscountDetail" runat="server" DataSourceID="ds" Width="100%" SkinID="Details" BorderStyle="None">
                        <Levels>
                            <px:PXGridLevel DataMember="DiscountDetails">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                                    <px:PXCheckBox ID="chkSkipDiscount" runat="server" DataField="SkipDiscount" />
                                    <px:PXSelector ID="edDiscountID" runat="server" DataField="DiscountID" 
                                        AllowEdit="True" edit="1" />
                                    <px:PXDropDown ID="edType" runat="server" DataField="Type" Enabled="False" />
                                    <px:PXCheckBox ID="chkIsManual" runat="server" DataField="IsManual" />
                                    <px:PXSelector ID="edDiscountSequenceID" runat="server" DataField="DiscountSequenceID" AllowEdit="True" AutoRefresh="True" edit="1" />
                                    <px:PXNumberEdit ID="edCuryDiscountableAmt" runat="server" DataField="CuryDiscountableAmt" />
                                    <px:PXNumberEdit ID="edDiscountableQty" runat="server" DataField="DiscountableQty" />
                                    <px:PXNumberEdit ID="edCuryDiscountAmt" runat="server" DataField="CuryDiscountAmt" />
                                    <px:PXNumberEdit ID="edDiscountPct" runat="server" DataField="DiscountPct" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="SkipDiscount" Width="75px" Type="CheckBox" TextAlign="Center" />
                                    <px:PXGridColumn DataField="DiscountID" Width="90px" CommitChanges="True" />
                                    <px:PXGridColumn DataField="DiscountSequenceID" Width="90px" CommitChanges="True" />
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
              <px:PXTabItem Text="Receipts">
                <Template>
                    <px:PXGrid ID="formReceipts" runat="server" DataSourceID="ds" Width="100%" SkinID="Details" BorderStyle="None">
                        <Levels>
                            <px:PXGridLevel DataMember="Receipts" >
                                <RowTemplate>
                                 <px:PXSelector SuppressLabel="True" Size="s" ID="edReceiptNbr" runat="server" 
                                        DataField="ReceiptNbr" AutoRefresh="True"
                                        AllowEdit="True" edit="1" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="ReceiptType" Width="90px" CommitChanges="True" />
                                    <px:PXGridColumn DataField="ReceiptNbr" Width="90px" CommitChanges="True" />
                                    <px:PXGridColumn DataField="Status" RenderEditorText="True" Width="90px" />
                                    <px:PXGridColumn DataField="ReceiptDate" RenderEditorText="True" Width="90px" />
                                    <px:PXGridColumn DataField="OrderQty" RenderEditorText="True" Width="90px" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" MinHeight="150" />
                         <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Other Information">
                <Template>
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM"  />
                    <px:PXSegmentMask CommitChanges="True" ID="edBranchID" runat="server" DataField="BranchID" />
                    <px:PXSelector CommitChanges="True" ID="edSOOrderType" runat="server" DataField="SOOrderType" />
                    <px:PXSelector ID="edSOOrderNbr" runat="server" AllowEdit="True" DataField="SOOrderNbr" edit="1" />
                    <px:PXSelector ID="edRQReqNbr" runat="server" AllowEdit="True" DataField="RQReqNbr" Enabled="False" />
                    <px:PXSelector ID="edPrepaymentRefNbr" runat="server" DataField="PrepaymentRefNbr" AllowEdit="True" Enabled="False" />
                    <px:PXSelector ID="edOwnerWorkgroupID" runat="server" DataField="OwnerWorkgroupID" />
                    <px:PXLayoutRule runat="server" Merge="True" />
                    <px:PXCheckBox ID="chkDontPrint" runat="server" Checked="True" DataField="DontPrint" Size="SM" />
                    <px:PXCheckBox ID="chkPrinted" runat="server" DataField="Printed" Enabled="False" Size="SM" />
                    <px:PXLayoutRule runat="server" />
                    <px:PXLayoutRule runat="server" Merge="True" />
                    <px:PXCheckBox ID="chkDontEmail" runat="server" Checked="True" DataField="DontEmail" Size="SM" />
                    <px:PXCheckBox ID="chkEmailed" runat="server" DataField="Emailed" Enabled="False" Size="SM" />
                    <px:PXLayoutRule runat="server" />
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                    <px:PXNumberEdit ID="edOpenOrderQty" runat="server" DataField="OpenOrderQty" />
                    <px:PXNumberEdit ID="edCuryOpenOrderTotal" runat="server" DataField="CuryOpenOrderTotal" />                                                            
                </Template>
            </px:PXTabItem>
        </Items>
        <AutoSize Container="Window" Enabled="True" MinHeight="180" />
    </px:PXTab>
    <px:PXSmartPanel ID="PanelAddSiteStatus" runat="server" Key="sitestatus" LoadOnDemand="true" Width="1100px" Height="500px"
        Caption="Inventory Lookup" CaptionVisible="true" AutoCallBack-Command='Refresh' AutoCallBack-Enabled="True" AutoCallBack-Target="formSitesStatus"
        DesignView="Hidden">
        <px:PXFormView ID="formSitesStatus" runat="server" CaptionVisible="False" DataMember="sitestatusfilter" DataSourceID="ds"
            Width="100%" SkinID="Transparent">
            <Template>
                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
                <px:PXTextEdit CommitChanges="True" ID="edInventory" runat="server" DataField="Inventory" />
                <px:PXTextEdit CommitChanges="True" ID="edBarCode" runat="server" DataField="BarCode" />
                <px:PXCheckBox CommitChanges="True" ID="chkOnlyAvailable" runat="server" Checked="True" DataField="OnlyAvailable" AutoCallBack="true"  />
                <px:PXLayoutRule  runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                <px:PXSegmentMask CommitChanges="True" ID="edSiteID" runat="server" DataField="SiteID" />
                <px:PXSelector CommitChanges="True" ID="edItemClassID" runat="server" DataField="ItemClassID" />
                <px:PXSegmentMask CommitChanges="True" ID="edSubItem" runat="server" DataField="SubItem" AutoRefresh="true" /></Template>
        </px:PXFormView>
        <px:PXGrid ID="gripSiteStatus" runat="server" DataSourceID="ds" Style="border-width: 1px 0px; top: 0px; left: 0px;" AutoAdjustColumns="true"
            Width="100%" SkinID="Details" AdjustPageSize="Auto" Height="135px" AllowSearch="True" BatchUpdate="true" FastFilterID="edInventory"
            FastFilterFields="InventoryCD,Descr,AlternateID">
            <CallbackCommands>
                <Refresh CommitChanges="true"></Refresh>
            </CallbackCommands>
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
                        <px:PXGridColumn AllowNull="False" DataField="AlternateID" Width="120px" />
                        <px:PXGridColumn AllowNull="False" DataField="AlternateType" Width="120px" />
                        <px:PXGridColumn AllowNull="False" DataField="AlternateDescr" Width="120px" />
                    </Columns>
                </px:PXGridLevel>
            </Levels>
            <AutoSize Enabled="true" />
        </px:PXGrid>
        <px:PXPanel ID="PXPanel5" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton6" runat="server" CommandName="AddInvSelBySite" CommandSourceID="ds" Text="Add" SyncVisible="false"/>
            <px:PXButton ID="PXButton7" runat="server" Text="Add & Close" DialogResult="OK"/>
            <px:PXButton ID="PXButton8" runat="server" DialogResult="Cancel" Text="Cancel" />
        </px:PXPanel>
    </px:PXSmartPanel>
    <%-- Recalculate Prices and Discounts --%>
    <px:PXSmartPanel ID="PanelRecalcDiscounts" runat="server" Caption="Recalculate Prices" CaptionVisible="true" LoadOnDemand="true" Key="recalcdiscountsfilter"
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
        <px:PXPanel ID="PXPanel6" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton10" runat="server" DialogResult="OK" Text="OK" CommandName="RecalcOk" CommandSourceID="ds" />
        </px:PXPanel>
    </px:PXSmartPanel>
</asp:Content>
