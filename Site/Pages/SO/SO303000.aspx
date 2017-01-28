<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SO303000.aspx.cs"
    Inherits="Page_SO303000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.SO.SOInvoiceEntry" PrimaryView="Document">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="Insert" PostData="Self" />
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
            <px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="True" />
            <px:PXDSCallbackCommand Name="Last" PostData="Self" />
            <px:PXDSCallbackCommand Visible="false" Name="Release" />
            <px:PXDSCallbackCommand Visible="False" Name="ReverseInvoice" />
            <px:PXDSCallbackCommand Visible="False" Name="PayInvoice" />
            <px:PXDSCallbackCommand Visible="False" Name="CreateSchedule" />
            <px:PXDSCallbackCommand Visible="False" Name="ViewSchedule" CommitChanges="true" DependOnGrid="grid" />
            <px:PXDSCallbackCommand Visible="False" Name="ViewBatch" />
            <px:PXDSCallbackCommand Visible="False" Name="NewCustomer" />
            <px:PXDSCallbackCommand Visible="False" Name="SendARInvoiceMemo" />
            <px:PXDSCallbackCommand Visible="False" Name="EditCustomer" />
            <px:PXDSCallbackCommand Visible="False" Name="CustomerDocuments" />
            <px:PXDSCallbackCommand Visible="False" Name="SOInvoice" />
            <px:PXDSCallbackCommand Name="SelectShipment" Visible="False" CommitChanges="true" />
            <px:PXDSCallbackCommand Name="AddShipment" Visible="False" CommitChanges="true" />
            <px:PXDSCallbackCommand Name="AddShipmentCancel" Visible="False" CommitChanges="true" />
            <px:PXDSCallbackCommand Name="Action" Visible="True" CommitChanges="true" StartNewGroup="true" />
            <px:PXDSCallbackCommand Name="Report" Visible="True" CommitChanges="true" />
            <px:PXDSCallbackCommand Name="Inquiry" Visible="True" CommitChanges="true" />
            <px:PXDSCallbackCommand Name="Hold" Visible="false" CommitChanges="true" />
            <px:PXDSCallbackCommand Name="CreditHold" Visible="false" CommitChanges="true" />
            <px:PXDSCallbackCommand Name="Flow" Visible="false" CommitChanges="true" />
            <px:PXDSCallbackCommand Visible="false" Name="AutoApply" CommitChanges="true" />
            <px:PXDSCallbackCommand Visible="false" Name="ViewPayment" DependOnGrid="detgrid" CommitChanges="true" />
            <px:PXDSCallbackCommand Visible="false" Name="ViewInvoice" />
            <px:PXDSCallbackCommand Visible="false" Name="ViewInvoice2" />
            <px:PXDSCallbackCommand Visible="false" Name="ViewItem" DependOnGrid="grid" />
            <px:PXDSCallbackCommand Visible="False" Name="CurrencyView" />
            <px:PXDSCallbackCommand Visible="False" CommitChanges="true" Name="CaptureCCPayment" />
            <px:PXDSCallbackCommand Visible="False" CommitChanges="true" Name="AuthorizeCCPayment" />
            <px:PXDSCallbackCommand Visible="False" CommitChanges="true" Name="VoidCCPayment" />
			<px:PXDSCallbackCommand Visible="False" CommitChanges="true" Name="CreditCCPayment" />
            <px:PXDSCallbackCommand Name="NewTask" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="NewEvent" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="NewActivity" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="NewMailActivity" Visible="False" CommitChanges="True" PopupCommand="Cancel" PopupCommandTarget="ds" />
            <px:PXDSCallbackCommand StartNewGroup="True" Name="ValidateAddresses" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="RecalculateDiscountsAction" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="RecalcOk" PopupCommand="" PopupCommandTarget="" PopupPanel="" Text="" Visible="False" />
			<px:PXDSCallbackCommand Name="WriteOff" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Visible="false" Name="ReclassifyBatch" />
        </CallbackCommands>
        <DataTrees>
            <px:PXTreeDataMember TreeView="_EPCompanyTree_Tree_" TreeKeys="WorkgroupID" />
        </DataTrees>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Document" Caption="Invoice Summary"
        NoteIndicator="True" FilesIndicator="True" LinkIndicator="True" NotifyIndicator="True" EmailingGraph="PX.Objects.CR.CREmailActivityMaint,PX.Objects"
        ActivityIndicator="True" ActivityField="NoteActivity" DefaultControlID="edDocType" TabIndex="100">
        <CallbackCommands>
            <Save PostData="Self" />
        </CallbackCommands>
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="S" />
            <px:PXDropDown ID="edDocType" runat="server" DataField="DocType" SelectedIndex="-1" />
            <px:PXSelector ID="edRefNbr" runat="server" DataField="RefNbr" AutoRefresh="True" DataSourceID="ds" />
            <px:PXDropDown ID="edStatus" runat="server" DataField="Status" Enabled="False" />
            <px:PXCheckBox ID="chkHold" runat="server" DataField="Hold">
                <AutoCallBack Command="Hold" Target="ds">
                    <Behavior CommitChanges="True" />
                </AutoCallBack>
            </px:PXCheckBox>
            <px:PXCheckBox ID="chkCreditHold" runat="server" DataField="CreditHold">
                <AutoCallBack Command="CreditHold" Target="ds">
                    <Behavior CommitChanges="True" />
                </AutoCallBack>
            </px:PXCheckBox>
            <px:PXDateTimeEdit CommitChanges="True" ID="edDocDate" runat="server" DataField="DocDate" />
            <px:PXSelector CommitChanges="True" ID="edFinPeriodID" runat="server" DataField="FinPeriodID" DataSourceID="ds" />
            <px:PXTextEdit ID="edInvoiceNbr" runat="server" DataField="InvoiceNbr" />
            <px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="S" StartColumn="True" />
            <px:PXSegmentMask CommitChanges="True" ID="edCustomerID" runat="server" DataField="CustomerID" AllowAddNew="True" AllowEdit="True"
                DataSourceID="ds" />
            <px:PXSegmentMask CommitChanges="True" ID="edCustomerLocationID" runat="server" AutoRefresh="True" DataField="CustomerLocationID"
                DataSourceID="ds" />
            <pxa:PXCurrencyRate DataField="CuryID" ID="edCury" runat="server" DataSourceID="ds" RateTypeView="_ARInvoice_CurrencyInfo_"
                DataMember="_Currency_" />
            <px:PXSelector CommitChanges="True" ID="edTermsID" runat="server" DataField="TermsID" DataSourceID="ds" />
            <px:PXDateTimeEdit ID="edDueDate" runat="server" DataField="DueDate" />
            <px:PXDateTimeEdit ID="edDiscDate" runat="server" DataField="DiscDate" />
            <px:PXSegmentMask ID="edProjectID" runat="server" DataField="ProjectID" DataSourceID="ds" />
            <px:PXLayoutRule runat="server" ColumnSpan="2" />
            <px:PXTextEdit ID="edDocDesc" runat="server" DataField="DocDesc" />
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
            <px:PXNumberEdit ID="edCuryVatExemptTotal" runat="server" DataField="CuryVatExemptTotal" Enabled="False" />
            <px:PXNumberEdit ID="edCuryVatTaxableTotal" runat="server" DataField="CuryVatTaxableTotal" Enabled="False" />
            <px:PXNumberEdit ID="edCuryDocBal" runat="server" DataField="CuryDocBal" Enabled="False" />
            <px:PXNumberEdit CommitChanges="True" ID="edCuryOrigDocAmt" runat="server" DataField="CuryOrigDocAmt" />
            <px:PXNumberEdit CommitChanges="True" ID="edCuryOrigDiscAmt" runat="server" DataField="CuryOrigDiscAmt" />
            <px:PXCheckBox runat="server" DataField="IsRUTROTDeductible" CommitChanges="True" ID="chkRUTROT" AlignLeft="true" />
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXTab ID="tab" runat="server" Height="363px" Style="z-index: 100;" Width="100%" TabIndex="23">
        <Activity HighlightColor="" SelectedColor="" Width="" Height=""></Activity>
        <Items>
            <px:PXTabItem Text="Document Details">
                <Template>
                    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100; left: 0px; top: 0px; height: 385px;" Width="100%"
                        BorderWidth="0px" SkinID="Details" SyncPosition="True" Height="385px" TabIndex="7700">
                        <Levels>
                            <px:PXGridLevel DataMember="Transactions" DataKeyNames="RefNbr,LineNbr">
                                <Columns>
                                    <px:PXGridColumn DataField="BranchID" DisplayFormat="&gt;AAAAAAAAAA" Width="81px" AutoCallBack="True" RenderEditorText="True"
                                        AllowShowHide="Server" />
                                    <px:PXGridColumn DataField="TranType" Width="30px" />
                                    <px:PXGridColumn DataField="RefNbr" Width="90px" />
                                    <px:PXGridColumn DataField="LineNbr" TextAlign="Right" Width="54px" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="LineType" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="SOShipmentNbr" Width="108px">
                                        <NavigateParams>
                                            <px:PXSyncGridParam ControlID="grid" />
                                        </NavigateParams>
                                    </px:PXGridColumn>
                                    <px:PXGridColumn AllowUpdate="False" DataField="SOOrderType" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="SOOrderNbr" Width="108px" />
                                    <px:PXGridColumn DataField="InventoryID" DisplayFormat="&gt;AAAAAAAAAA" Width="81px" AutoCallBack="True" RenderEditorText="True"
                                        LinkCommand="ViewItem" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="SOLine__SubItemID" DisplayFormat="#" Label="Subitem" />
                                    <px:PXGridColumn DataField="TranDesc" Width="180px" />
                                    <px:PXGridColumn AllowNull="False" DataField="Qty" TextAlign="Right" Width="81px" CommitChanges="True" />
                                    <px:PXGridColumn DataField="UOM" Width="54px" AutoCallBack="True" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryUnitPrice" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn DataField="ManualPrice" TextAlign="Center" Type="CheckBox" />
                                    <px:PXGridColumn AllowNull="False" DataField="DiscPct" TextAlign="Right" Width="108px" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryDiscAmt" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn AllowNull="False" DataField="ManualDisc" TextAlign="Center" Type="CheckBox" />
                                    <px:PXGridColumn DataField="DiscountID" RenderEditorText="True" TextAlign="Left" AllowShowHide="Server" Width="90px" AutoCallBack="True" />
                                    <px:PXGridColumn DataField="DiscountSequenceID" TextAlign="Left" Width="90px" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryTranAmt" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn DataField="AccountID" DisplayFormat="&gt;#####" Width="108px" AutoCallBack="True" />
                                    <px:PXGridColumn DataField="AccountID_Account_description" Width="120px" />
                                    <px:PXGridColumn DataField="SubID" DisplayFormat="&gt;AA-AA-AA-AA-AA-AA" Width="180px" />
                                    <px:PXGridColumn DataField="TaskID" DisplayFormat="&gt;AAAAAAAAAA" Label="Task" Width="81px" />
                                    <px:PXGridColumn DataField="SalesPersonID" DisplayFormat="&gt;AAAAAAAAAA" Width="81px" RenderEditorText="True" AutoCallBack="True" />
                                    <px:PXGridColumn DataField="DeferredCode" DisplayFormat="&gt;aaaaaaaaaa" Label="Deferral Code" Width="81px" CommitChanges="true" />
									<px:PXGridColumn DataField="DRTermStartDate" Width="80px" CommitChanges="true" />
									<px:PXGridColumn DataField="DRTermEndDate" Width="80px" CommitChanges="true" />
                                    <px:PXGridColumn DataField="DefScheduleID" Label="Deferral Schedule" TextAlign="Right" Width="108px" />
                                    <px:PXGridColumn DataField="TaxCategoryID" />
                                    <px:PXGridColumn AllowNull="False" DataField="Commissionable" TextAlign="Center" Type="CheckBox" Width="80px" AutoCallBack="True" />
									<px:PXGridColumn AllowUpdate="False" DataField="SOOrderLineNbr" Width="90px" />   
                                    <px:PXGridColumn DataField="IsRUTROTDeductible" Width="100px" Type="Checkbox" AutoCallBack="True" CommitChanges="true" />
                                    <px:PXGridColumn DataField="RUTROTItemType" Width="100px" AutoCallBack="True" CommitChanges="true" />
                                    <px:PXGridColumn DataField="RUTROTWorkTypeID" Width="100px" AutoCallBack="True" CommitChanges="true" />     
                                    <px:PXGridColumn DataField="CuryRUTROTAvailableAmt" Width="100px" />                      
                                </Columns>
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="S" />
                                    <px:PXSegmentMask CommitChanges="True" ID="edInventoryID" runat="server" DataField="InventoryID" AllowEdit="True" Size="XM" />
                                    <px:PXSegmentMask ID="edSOLine__SubItemID" runat="server" DataField="SOLine__SubItemID" Enabled="False" />
                                    <px:PXSelector ID="edSOShipmentNbr" runat="server" DataField="SOShipmentNbr" Enabled="False" AllowEdit="True" />
                                    <px:PXTextEdit ID="edSOOrderType1" runat="server" DataField="SOOrderType" Enabled="False" />
                                    <px:PXSelector ID="edSOOrderNbr1" runat="server" DataField="SOOrderNbr" Enabled="False" AllowEdit="True"/>
                                    <px:PXSelector CommitChanges="True" ID="edUOM" runat="server" DataField="UOM">
                                        <Parameters>
                                            <px:PXControlParam ControlID="grid" Name="ARTran.inventoryID" PropertyName="DataValues[&quot;InventoryID&quot;]" Type="String" />
                                        </Parameters>
                                    </px:PXSelector>
                                    <px:PXNumberEdit ID="edQty" runat="server" DataField="Qty" />
                                    <px:PXNumberEdit ID="edCuryUnitPrice" runat="server" DataField="CuryUnitPrice" />
                                    <px:PXSelector ID="edDiscountCode" runat="server" DataField="DiscountID" AllowEdit="True" edit="1" />
                                    <px:PXNumberEdit ID="edDiscPct" runat="server" DataField="DiscPct" />
                                    <px:PXNumberEdit ID="edCuryDiscAmt" runat="server" DataField="CuryDiscAmt" />
                                    <px:PXCheckBox ID="chkManualDisc" runat="server" DataField="ManualDisc" CommitChanges="true" />
                                    <px:PXLayoutRule runat="server" ColumnSpan="2" />
                                    <px:PXTextEdit ID="edTranDesc1" runat="server" DataField="TranDesc" />
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                                    <px:PXSegmentMask CommitChanges="True" ID="edBranchID" runat="server" DataField="BranchID" />
                                    <px:PXSegmentMask ID="edTaskID" runat="server" DataField="TaskID" />
                                    <px:PXNumberEdit ID="edCuryTranAmt1" runat="server" DataField="CuryTranAmt" Enabled="False" />
                                    <px:PXSegmentMask ID="edAccountID1" runat="server" DataField="AccountID" AutoRefresh="True" />
                                    <px:PXSegmentMask ID="edSubID1" runat="server" DataField="SubID" AutoRefresh="True">
                                        <Parameters>
                                            <px:PXControlParam ControlID="grid" Name="ARTran.accountID" PropertyName="DataValues[&quot;AccountID&quot;]" Type="String" />
                                        </Parameters>
                                    </px:PXSegmentMask>
                                    <px:PXSegmentMask ID="edSalesPersonID1" runat="server" DataField="SalesPersonID" />
                                    <px:PXSelector ID="edTaxCategoryID1" runat="server" DataField="TaxCategoryID" AutoRefresh="True"/>
                                    <px:PXCheckBox CommitChanges="True" ID="chkCommissionable" runat="server" Checked="True" DataField="Commissionable" />
                                    <px:PXSelector ID="edDeferredCode" runat="server" DataField="DeferredCode" CommitChanges="true" />
									<px:PXDateTimeEdit ID="edDRTermStartDate" runat="server" DataField="DRTermStartDate" CommitChanges="true" />
									<px:PXDateTimeEdit ID="edDRTermEndDate" runat="server" DataField="DRTermEndDate" CommitChanges="true" />
                                    <px:PXSelector ID="edDefScheduleID" runat="server" DataField="DefScheduleID" />
                                    <px:PXCheckBox runat="server" DataField="IsRUTROTDeductible" CommitChanges="True" ID="chkRRDeductibleTran" />
                                    <px:PXDropDown runat="server" DataField="RUTROTItemType" CommitChanges="True" ID="cmbRRItemType" />
                                    <px:PXSelector runat="server" DataField="RUTROTWorkTypeID" CommitChanges="True" ID="cmbRRWorkType"  AutoRefresh="true"  />
                                    <px:PXNumberEdit runat="server" DataField="CuryRUTROTAvailableAmt" ID="edRRAvailable" />
                                </RowTemplate>
                                <Layout FormViewHeight="" />
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" MinHeight="150" />
                        <Mode InitNewRow="True" AllowFormEdit="True" />
                        <ActionBar>
                            <CustomItems>
                                <px:PXToolBarButton Text="Add Order" Key="cmdShipmentList" >
                                    <AutoCallBack Command="SelectShipment" Target="ds">
                                        <Behavior PostData="Page" CommitChanges="True" />
                                    </AutoCallBack>
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Text="View Schedule" Key="cmdViewSchedule">
                                    <AutoCallBack Command="ViewSchedule" Target="ds"/>
                                    <PopupCommand Command="Cancel" Target="ds" />
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Text="View Item" Key="ViewItem">
                                    <AutoCallBack Command="ViewItem" Target="ds"/>
                                </px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Tax Details">
                <Template>
                    <px:PXGrid ID="grid1" runat="server" DataSourceID="ds" Height="150px" Style="z-index: 100" Width="100%" SkinID="Details"
                        BorderWidth="0px">
                        <AutoSize Enabled="True" MinHeight="150" />
                        <Levels>
                            <px:PXGridLevel DataMember="Taxes">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                                    <px:PXSelector SuppressLabel="True" ID="edTaxID" runat="server" DataField="TaxID" />
                                    <px:PXNumberEdit SuppressLabel="True" ID="edTaxRate" runat="server" DataField="TaxRate" Enabled="False" />
                                    <px:PXNumberEdit SuppressLabel="True" ID="edCuryTaxableAmt" runat="server" DataField="CuryTaxableAmt" />
                                    <px:PXNumberEdit SuppressLabel="True" ID="edCuryTaxAmt" runat="server" DataField="CuryTaxAmt" />
                                    <px:PXNumberEdit ID="edCuryDiscountedTaxableAmt" runat="server" DataField="CuryDiscountedTaxableAmt" Enabled="False" />
                                    <px:PXNumberEdit ID="edCuryDiscountedPrice" runat="server" DataField="CuryDiscountedPrice" Enabled="False" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="TaxID" Width="81px" AllowUpdate="False" />
                                    <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="TaxRate" TextAlign="Right" Width="90px" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryTaxableAmt" TextAlign="Right" Width="90px" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryTaxAmt" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn AllowNull="False" DataField="Tax__TaxType" Label="Tax Type" RenderEditorText="True" />
                                    <px:PXGridColumn AllowNull="False" DataField="Tax__PendingTax" Label="Pending VAT" TextAlign="Center" Type="CheckBox" Width="60px" />
                                    <px:PXGridColumn AllowNull="False" DataField="Tax__ReverseTax" Label="Reverse VAT" TextAlign="Center" Type="CheckBox" Width="60px" />
                                    <px:PXGridColumn AllowNull="False" DataField="Tax__ExemptTax" Label="Exempt From VAT" TextAlign="Center" Type="CheckBox"
                                        Width="60px" />
                                    <px:PXGridColumn AllowNull="False" DataField="Tax__StatisticalTax" Label="Statistical VAT" TextAlign="Center" Type="CheckBox"
                                        Width="60px" />
                                    <px:PXGridColumn DataField="CuryDiscountedTaxableAmt" Width="100px"/>
                                    <px:PXGridColumn DataField="CuryDiscountedPrice" Width="100px" />
                                </Columns>
                                <Layout FormViewHeight="" />
                            </px:PXGridLevel>
                        </Levels>
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Commissions" RepaintOnDemand="false">
                <Template>
                     <px:PXFormView ID="Commission" runat="server" DataMember="CurrentDocument" RenderStyle="Simple" SkinID="Transparent">
						<Template>
						    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
							<px:PXSegmentMask CommitChanges="True" ID="edSalesPersonID" runat="server" DataField="SalesPersonID" />
                            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
					        <px:PXNumberEdit ID="edCuryCommnblAmt" runat="server" DataField="CuryCommnblAmt" Enabled="False" />
					        <px:PXNumberEdit ID="edCuryCommnAmt" runat="server" DataField="CuryCommnAmt" Enabled="False" />
						</Template>
					</px:PXFormView>
                    <px:PXGrid ID="gridSalesPerTran" runat="server" Height="200px" Width="100%" DataSourceID="ds" BorderWidth="0px" SkinID="Details">
                        <Levels>
                            <px:PXGridLevel DataMember="SalesPerTrans">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                                    <px:PXNumberEdit ID="edCommnPct" runat="server" DataField="CommnPct" AllowNull="True" />
                                    <px:PXNumberEdit ID="edCommnAmt" runat="server" DataField="CommnAmt" />
                                    <px:PXNumberEdit ID="edCuryCommnAmt" runat="server" DataField="CuryCommnAmt" />
                                    <px:PXNumberEdit ID="edCommnblAmt" runat="server" DataField="CommnblAmt" />
                                    <px:PXNumberEdit ID="edCuryCommnblAmt" runat="server" DataField="CuryCommnblAmt" AllowNull="True" />
                                    <px:PXSegmentMask ID="edSalesPersonID_1" runat="server" DataField="SalespersonID" AutoRefresh="True" /></RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="SalespersonID" Width="108px" AutoCallBack="True" />
                                    <px:PXGridColumn DataField="CommnPct" TextAlign="Right" Width="108px" />
                                    <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="CuryCommnAmt" TextAlign="Right" Width="99px" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="CuryCommnblAmt" TextAlign="Right" Width="99px" />
                                </Columns>
                                <Layout FormViewHeight="" />
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" MinHeight="100" MinWidth="100" />
                        <Mode AllowAddNew="False" AllowDelete="False" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Freight Details">
                <Template>
                    <px:PXGrid ID="gridFreightDetails" runat="server" DataSourceID="ds" Style="z-index: 100; left: 0px; top: 0px; height: 265px;"
                        Width="100%" BorderWidth="0px" SkinID="Details" Height="265px">
                        <Levels>
                            <px:PXGridLevel DataMember="FreightDetails">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                                    <px:PXTextEdit ID="edShipmentNbr" runat="server" DataField="ShipmentNbr" Enabled="False" />
                                    <px:PXSelector ID="edShipTermsID" runat="server" DataField="ShipTermsID" Enabled="False" />
                                    <px:PXSelector ID="edShipZoneID" runat="server" DataField="ShipZoneID" Enabled="False" />
                                    <px:PXSelector ID="edShipVia" runat="server" DataField="ShipVia" Enabled="False" />
                                    <px:PXNumberEdit ID="edWeight" runat="server" DataField="Weight" Enabled="False" />
                                    <px:PXNumberEdit ID="edVolume" runat="server" DataField="Volume" Enabled="False" />
                                    <px:PXNumberEdit ID="edCuryLineTotal" runat="server" DataField="CuryLineTotal" Enabled="False" />
                                    <px:PXNumberEdit ID="edCuryFreightAmt" runat="server" DataField="CuryFreightAmt" Enabled="False" />
                                    <px:PXNumberEdit ID="edCuryPremiumFreightAmt" runat="server" DataField="CuryPremiumFreightAmt" />
                                    <px:PXNumberEdit ID="edCuryTotalFreightAmt" runat="server" DataField="CuryTotalFreightAmt" Enabled="False" />
                                    <px:PXSegmentMask CommitChanges="True" ID="edAccountID2" runat="server" DataField="AccountID" />
                                    <px:PXSegmentMask ID="edSubID2" runat="server" DataField="SubID" />
                                    <px:PXSelector ID="edTaxCategoryID2" runat="server" DataField="TaxCategoryID" AutoRefresh="True"/>
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn AllowUpdate="False" DataField="ShipmentNbr" DisplayFormat="&gt;aaaaaaaaaaaaaaa" Width="90px" />
                                    <px:PXGridColumn DataField="ShipTermsID" DisplayFormat="&gt;aaaaaaaaaa" Width="90px" />
                                    <px:PXGridColumn DataField="ShipZoneID" DisplayFormat="&gt;aaaaaaaaaaaaaaa" Width="90px" />
                                    <px:PXGridColumn DataField="ShipVia" DisplayFormat="&gt;aaaaaaaaaaaaaaa" Width="90px" />
                                    <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="Weight" TextAlign="Right" Width="72px" />
                                    <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="Volume" TextAlign="Right" Width="72px" />
                                    <px:PXGridColumn DataField="CuryLineTotal" TextAlign="Right" Width="72px" />
                                    <px:PXGridColumn DataField="CuryFreightCost" TextAlign="Right" Width="72px" />
                                    <px:PXGridColumn DataField="CuryFreightAmt" TextAlign="Right" Width="72px" />
                                    <px:PXGridColumn DataField="CuryPremiumFreightAmt" TextAlign="Right" Width="118px" />
                                    <px:PXGridColumn DataField="CuryTotalFreightAmt" TextAlign="Right" Width="90px" />
                                    <px:PXGridColumn DataField="AccountID" DisplayFormat="&gt;AAAAAAAAAA" AutoCallBack="True" Width="81px" />
                                    <px:PXGridColumn DataField="AccountID_Account_description" Width="120px" />
                                    <px:PXGridColumn DataField="SubID" DisplayFormat="&gt;AAAA.AA.AA.AAAA" Width="108px" />
                                    <px:PXGridColumn DataField="TaskID" AutoCallBack="True" Width="81px" />
                                    <px:PXGridColumn DataField="TaxCategoryID" Width="90px" />
                                </Columns>
                                <Layout FormViewHeight="" />
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" MinHeight="150" />
                        <Mode AllowAddNew="False" AllowDelete="False" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Financial Details">
                <Template>
                    <px:PXFormView ID="form2" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="CurrentDocument"
                        CaptionVisible="False" SkinID="Transparent">
                        <Template>
                            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                            <px:PXSelector ID="edBatchNbr" runat="server" DataField="BatchNbr" Enabled="False" AllowEdit="True" DataSourceID="ds" />
                            <px:PXSegmentMask CommitChanges="True" ID="edBranchID" runat="server" DataField="BranchID" DataSourceID="ds" />
                            <px:PXSegmentMask ID="edARAccountID" runat="server" DataField="ARAccountID" DataSourceID="ds" />
                            <px:PXSegmentMask ID="edARSubID" runat="server" DataField="ARSubID" AutoRefresh="True" DataSourceID="ds">
                                <Parameters>
                                    <px:PXControlParam ControlID="form2" Name="ARRegister.aRAccountID" PropertyName="DataControls[&quot;edARAccountID&quot;].Value" />
                                </Parameters>
                            </px:PXSegmentMask>
                            <px:PXSelector CommitChanges="True" ID="edTaxZoneID" runat="server" DataField="TaxZoneID" Text="ZONE1" DataSourceID="ds" />
							<px:PXDropDown ID="edAvalaraCustomerUsageTypeID" runat="server" CommitChanges="True" DataField="AvalaraCustomerUsageType" />
                            <px:PXLayoutRule runat="server" Merge="True" />
                            <px:PXCheckBox ID="chkDontPrint" runat="server" Checked="True" DataField="DontPrint" Size="SM" />
                            <px:PXCheckBox ID="chkPrinted" runat="server" DataField="Printed" Enabled="False" Size="SM" />
                            <px:PXLayoutRule runat="server" />
                            <px:PXLayoutRule runat="server" Merge="True" />
                            <px:PXCheckBox ID="chkDontEmail" runat="server" Checked="True" DataField="DontEmail" Size="SM" />
                            <px:PXCheckBox ID="chkEmailed" runat="server" DataField="Emailed" Enabled="False" Size="SM" />
                            <px:PXLayoutRule runat="server" />
                            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                            <px:PXTreeSelector CommitChanges="True" ID="edWorkgroupID" runat="server" DataField="WorkgroupID" TreeDataMember="_EPCompanyTree_Tree_"
                                TreeDataSourceID="ds" PopulateOnDemand="True" InitialExpandLevel="0" ShowRootNode="False">
                                <DataBindings>
                                    <px:PXTreeItemBinding TextField="Description" ValueField="Description" />
                                </DataBindings>
                            </px:PXTreeSelector>
                            <px:PXSelector CommitChanges="True" ID="edOwnerID" runat="server" AutoRefresh="True" DataField="OwnerID" DataSourceID="ds" />
                        </Template>
                        <AutoSize Enabled="True" />
                    </px:PXFormView>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Payment Information">
                <Template>
                    <px:PXFormView ID="formP" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="SODocument" CaptionVisible="False"
                        SkinID="Transparent">
                        <Template>
                            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                            <px:PXSelector CommitChanges="True" ID="edPaymentMethodID" runat="server" DataField="PaymentMethodID"
                                AutoRefresh="True" DataSourceID="ds" />
                            <px:PXSelector CommitChanges="True" Size="xm" ID="edPMInstanceID" runat="server" DataField="PMInstanceID" TextField="Descr"
                                AutoRefresh="True" AutoGenerateColumns="True" DataSourceID="ds" />
                            <px:PXSegmentMask CommitChanges="True" ID="edCashAccountID" runat="server" DataField="CashAccountID" DataSourceID="ds" />
                            <px:PXTextEdit ID="edExtRefNbr" runat="server" DataField="ExtRefNbr" />
                            <px:PXLayoutRule runat="server" Merge="True" />
                            <px:PXDateTimeEdit CommitChanges="True" Size="s" ID="edClearDate" runat="server" DataField="ClearDate" />
                            <px:PXCheckBox CommitChanges="True" ID="edCleared" runat="server" DataField="Cleared" />
                            <px:PXLayoutRule runat="server" />
                            <px:PXTextEdit ID="edCCPaymentStateDescr" runat="server" DataField="CCPaymentStateDescr" Enabled="False" />
                            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="XXS" ControlSize="M" SuppressLabel="True" />
                            <px:PXButton ID="pbAutorizeCCPayment" runat="server" Text="Authorize Payment" CommandName="AuthorizeCCPayment" CommandSourceID="ds" />
                            <px:PXButton ID="pbCaptureCCPayment" runat="server" Text="Capture Payment" CommandName="CaptureCCPayment" CommandSourceID="ds" />
                            <px:PXButton ID="pbVoidCCPayment" runat="server" Text="Void Authorization" CommandName="VoidCCPayment" CommandSourceID="ds" />
							<px:PXButton ID="pbCreditCCPayment" runat="server" Text="Refund Payment" CommandName="CreditCCPayment" CommandSourceID="ds" />
                            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                            <px:PXNumberEdit ID="edCuryPaymentTotal" runat="server" DataField="CuryPaymentTotal" Enabled="False" />
                            <px:PXNumberEdit ID="edCuryAmtToCapture" runat="server" DataField="CuryAmtToCapture" Enabled="False" />
                            <px:PXNumberEdit ID="edCuryCCCapturedAmt" runat="server" DataField="CuryCCCapturedAmt" Enabled="False" />
							<px:PXSelector ID="edRefTranExtNbr" runat="server" DataField="RefTranExtNbr" DataSourceID="ds" AutoRefresh="True" CommitChanges="True"/>
                        </Template>
                    </px:PXFormView>
                    <px:PXGrid ID="grdCCProcTran" runat="server" DataSourceID="ds" Height="100%" Width="100%" BorderWidth="0px" Style="left: 0px;
                        top: 0px;" SkinID="DetailsInTab" Caption="Credit Card Processing Info">
                        <ActionBar>
                            <Actions>
                                <Save Enabled="False" />
                                <AddNew Enabled="False" />
                                <Delete Enabled="False" />
                            </Actions>
                        </ActionBar>
                        <Levels>
                            <px:PXGridLevel DataMember="ccProcTran">
                                <Mode AllowAddNew="True" AllowDelete="True" AllowUpdate="True" />
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                                    <px:PXNumberEdit ID="edTranNbr2" runat="server" DataField="TranNbr" />
                                    <px:PXDropDown ID="edProcStatus" runat="server" AllowNull="False" DataField="ProcStatus" />
                                    <px:PXTextEdit ID="edProcessingCenterID" runat="server" AllowNull="False" DataField="ProcessingCenterID" />
                                    <px:PXDropDown ID="edCVVVerificationStatus" runat="server" DataField="CVVVerificationStatus" />
                                    <px:PXDropDown ID="edCCTranType" runat="server" DataField="TranType" />
                                    <px:PXDropDown ID="edTranStatus" runat="server" AllowNull="False" DataField="TranStatus" />
                                    <px:PXNumberEdit ID="edAmount" runat="server" DataField="Amount" />
                                    <px:PXNumberEdit ID="edRefTranNbr" runat="server" DataField="RefTranNbr" />
                                    <px:PXTextEdit ID="edPCTranNumber" runat="server" DataField="PCTranNumber" />
                                    <px:PXTextEdit ID="edAuthNumber" runat="server" DataField="AuthNumber" />
                                    <px:PXTextEdit ID="edPCResponseReasonText" runat="server" DataField="PCResponseReasonText" />
                                    <px:PXDateTimeEdit ID="edStartTime" runat="server" DataField="StartTime" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="TranNbr" TextAlign="Right" Width="54px" />
                                    <px:PXGridColumn AllowNull="False" DataField="ProcessingCenterID" Width="85px" />
                                    <px:PXGridColumn DataField="TranType" RenderEditorText="True" Width="140px" />
                                    <px:PXGridColumn AllowNull="False" DataField="TranStatus" RenderEditorText="True" Width="75px" />
                                    <px:PXGridColumn AllowNull="False" DataField="Amount" TextAlign="Right" Width="80px" />
                                    <px:PXGridColumn DataField="RefTranNbr" TextAlign="Right" Width="54px" />
                                    <px:PXGridColumn DataField="PCTranNumber" Width="90px" />
                                    <px:PXGridColumn DataField="AuthNumber" Width="75px" />
                                    <px:PXGridColumn DataField="PCResponseReasonText" Width="240px" />
                                    <px:PXGridColumn DataField="StartTime" />
                                    <px:PXGridColumn AllowNull="False" DataField="ProcStatus" RenderEditorText="True" Width="72px" />
                                    <px:PXGridColumn DataField="CVVVerificationStatus" RenderEditorText="True" Width="171px" />
                                    <px:PXGridColumn DataField="ErrorSource" Visible="False" />
                                    <px:PXGridColumn DataField="ErrorText" Visible="False" Width="200px" />
                                </Columns>
                                <Layout FormViewHeight="" />
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" MinHeight="200" MinWidth="50" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Billing Address">
                <Template>
                    <px:PXLayoutRule runat="server" StartColumn="True" SuppressLabel="True" />
                    <px:PXPanel RenderStyle="Fieldset" ID="panelC" runat="server" Caption="Bill-To Info">
                        <px:PXFormView ID="formA" runat="server" CaptionVisible="False" DataMember="Billing_Address" DataSourceID="ds" AllowCollapse="false">
                            <Template>
                                <px:PXLayoutRule runat="server" StartColumn="True" ControlSize="XM" LabelsWidth="SM" />
                                <px:PXCheckBox ID="chkOverrideAddress" runat="server" CommitChanges="True" DataField="OverrideAddress" Height="18px" />
                                <px:PXTextEdit ID="edAddressLine1" runat="server" DataField="AddressLine1" />
                                <px:PXTextEdit ID="edAddressLine2" runat="server" DataField="AddressLine2" />
                                <px:PXTextEdit ID="edCity" runat="server" DataField="City" />
                                <px:PXSelector ID="edCountryID" runat="server" AutoRefresh="True" DataField="CountryID" DataSourceID="ds" CommitChanges="true" />
                                <px:PXSelector ID="edState" runat="server" AutoRefresh="True" DataField="State" DataSourceID="ds">
                                    <CallBackMode PostData="Container" />
                                    <Parameters>
                                        <px:PXControlParam ControlID="formA" Name="ARAddress.countryID" PropertyName="DataControls[&quot;edCountryID&quot;].Value"
                                            Type="String" />
                                    </Parameters>
                                </px:PXSelector>
                                <px:PXMaskEdit ID="edPostalCode" runat="server" CommitChanges="True" DataField="PostalCode" />
                            </Template>
                            <ContentStyle BackColor="Transparent" BorderStyle="None" />
                        </px:PXFormView>
                        <px:PXFormView ID="formC" runat="server" CaptionVisible="False" DataMember="Billing_Contact" DataSourceID="ds" AllowCollapse="false">
                            <Template>
                                <px:PXLayoutRule runat="server" StartColumn="True" ControlSize="XM" LabelsWidth="SM" />
                                <px:PXCheckBox ID="chkOverrideContact" runat="server" CommitChanges="True" DataField="OverrideContact" />
                                <px:PXTextEdit ID="edFullName" runat="server" DataField="FullName" />
                                <px:PXTextEdit ID="edSalutation" runat="server" DataField="Salutation" />
                                <px:PXMaskEdit ID="edPhone1" runat="server" DataField="Phone1" />
                                <px:PXMailEdit ID="edEmail" runat="server" DataField="Email" CommitChanges="True"/>
                            </Template>
                            <ContentStyle BackColor="Transparent" BorderStyle="None" />
                        </px:PXFormView>
                    </px:PXPanel>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Discount Details" RepaintOnDemand="false">
                <Template>
                    <px:PXGrid ID="freeItemsGrid" runat="server" DataSourceID="ds" Height="150px" Width="100%" SkinID="Details" BorderWidth="0px"
                        Style="left: 0px; top: 0px;">
                        <Levels>
                            <px:PXGridLevel DataMember="DiscountDetails">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                                    <px:PXCheckBox ID="chkSkipDiscount" runat="server" DataField="SkipDiscount" />
                                    <px:PXTextEdit ID="edDiscountID" runat="server" DataField="DiscountID" />
                                    <px:PXTextEdit ID="edDiscountSequenceID" runat="server" DataField="DiscountSequenceID" />
                                    <px:PXMaskEdit ID="edType" runat="server" DataField="Type" InputMask="&gt;a" />
                                    <px:PXCheckBox ID="chkIsManual" runat="server" DataField="IsManual" />
                                    <px:PXNumberEdit ID="edCuryDiscountableAmt" runat="server" DataField="CuryDiscountableAmt" />
                                    <px:PXNumberEdit ID="edDiscountableQty" runat="server" DataField="DiscountableQty" />
                                    <px:PXNumberEdit ID="edCuryDiscountAmt" runat="server" DataField="CuryDiscountAmt" />
                                    <px:PXNumberEdit ID="edDiscountPct" runat="server" DataField="DiscountPct" />
                                    <px:PXSegmentMask ID="edFreeItemID" runat="server" DataField="FreeItemID" AllowEdit="True" />
                                    <px:PXNumberEdit ID="edFreeItemQty" runat="server" DataField="FreeItemQty" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="SkipDiscount" Width="75px" Type="CheckBox" TextAlign="Center" />
                                    <px:PXGridColumn DataField="OrderType" Width="63px" />
                                    <px:PXGridColumn DataField="OrderNbr" Width="90px" />
                                    <px:PXGridColumn DataField="DiscountID" Width="90px" />
                                    <px:PXGridColumn DataField="DiscountSequenceID" Width="90px" />
                                    <px:PXGridColumn DataField="Type" DisplayFormat="&gt;a" Width="90px" />
                                    <px:PXGridColumn DataField="IsManual" Width="75px" Type="CheckBox" TextAlign="Center" />
                                    <px:PXGridColumn DataField="CuryDiscountableAmt" TextAlign="Right" Width="99px" />
                                    <px:PXGridColumn DataField="DiscountableQty" TextAlign="Right" Width="99px" />
                                    <px:PXGridColumn DataField="CuryDiscountAmt" TextAlign="Right" Width="90px" />
                                    <px:PXGridColumn DataField="DiscountPct" TextAlign="Right" Width="90px" />
                                    <px:PXGridColumn DataField="FreeItemID" DisplayFormat="&gt;CCCCC-CCCCCCCCCCCCCCC" Width="144px" />
                                    <px:PXGridColumn DataField="FreeItemQty" TextAlign="Right" Width="81px" />
                                </Columns>
                                <Layout FormViewHeight="" />
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" MinHeight="150" />
                        <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Applications" RepaintOnDemand="false">
                <Template>
                    <px:PXGrid ID="detgrid" runat="server" DataSourceID="ds" Style="z-index: 100; left: 0px; top: 0px; height: 332px;" Width="100%"
                        BorderWidth="0px" SkinID="Details" Height="332px">
                        <Levels>
                            <px:PXGridLevel DataMember="Adjustments">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                                    <px:PXDropDown ID="edAdjgDocType" runat="server" DataField="AdjgDocType" Enabled="False" />
                                    <px:PXSelector CommitChanges="True" ID="edAdjgRefNbr" runat="server" DataField="AdjgRefNbr" Enabled="False">
                                        <Parameters>
                                            <px:PXControlParam ControlID="form" Name="ARInvoice.customerID" PropertyName="DataControls[&quot;edCustomerID&quot;].Value" />
                                            <px:PXControlParam ControlID="detgrid" Name="ARAdjust.adjgDocType" PropertyName="DataValues[&quot;AdjgDocType&quot;]" />
                                        </Parameters>
                                    </px:PXSelector>
                                    <px:PXNumberEdit CommitChanges="True" ID="edCuryAdjdAmt" runat="server" DataField="CuryAdjdAmt" />
                                    <px:PXDateTimeEdit ID="edARPayment__DocDate" runat="server" DataField="ARPayment__DocDate" Enabled="False" />
                                    <px:PXNumberEdit ID="edCuryDocBal" runat="server" DataField="CuryDocBal" Enabled="False" />
                                    <px:PXSelector ID="edARPayment__CuryID" runat="server" DataField="ARPayment__CuryID" />
                                    <px:PXSelector ID="edARPayment__FinPeriodID" runat="server" DataField="ARPayment__FinPeriodID" Enabled="False" />
                                    <px:PXTextEdit ID="edARPayment__ExtRefNbr" runat="server" DataField="ARPayment__ExtRefNbr" />
                                    <px:PXDropDown ID="edARPayment__Status" runat="server" AllowNull="False" DataField="ARPayment__Status" Enabled="False" />
                                    <px:PXTextEdit ID="edARPayment__DocDesc" runat="server" DataField="ARPayment__DocDesc" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="AdjgDocType" Width="117px" RenderEditorText="True" />
                                    <px:PXGridColumn DataField="AdjgRefNbr" Width="108px" AutoCallBack="True" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryAdjdAmt" AutoCallBack="True" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn DataField="CuryAdjgDiscAmt" TextAlign="Right" Width="90px" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="ARPayment__DocDate" Width="90px" />
                                    <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="CuryDocBal" TextAlign="Right" Width="81px" />
                                    <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="ARPayment__DocDesc" Width="180px" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="ARPayment__CuryID" DisplayFormat="&gt;LLLLL" Width="54px" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="ARPayment__FinPeriodID" DisplayFormat="##-####" />
                                    <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="ARPayment__ExtRefNbr" Width="90px" />
                                    <px:PXGridColumn DataField="CustomerID" TextAlign="Right" Width="54px" />
                                    <px:PXGridColumn DataField="AdjdDocType" Width="18px" />
                                    <px:PXGridColumn DataField="AdjdRefNbr" Width="90px" />
                                    <px:PXGridColumn DataField="AdjNbr" TextAlign="Right" Width="54px" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="ARPayment__Status" RenderEditorText="True" />
                                </Columns>
                                <Layout FormViewHeight="" />
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" MinHeight="150" />
                        <ActionBar>
                            <CustomItems>
                                <px:PXToolBarButton Text="View Payment" Tooltip="View Payment">
                                    <AutoCallBack Command="ViewPayment" Target="ds">
                                        <Behavior CommitChanges="True" />
                                    </AutoCallBack>
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Text="Auto Apply" Tooltip="Auto Apply">
                                    <AutoCallBack Command="AutoApply" Target="ds">
                                        <Behavior CommitChanges="True" />
                                    </AutoCallBack>
                                </px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
                        <Mode AllowFormEdit="True" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Totals">
                <Template>
                    <px:PXFormView ID="formG" runat="server" DataSourceID="ds" Style="z-index: 100; left: 18px; top: 36px;" Width="100%" DataMember="SODocument"
                        CaptionVisible="False" SkinID="Transparent">
                        <AutoSize Enabled="True" />
                        <Template>
                            <px:PXLayoutRule runat="server" ControlSize="XL" LabelsWidth="XL" StartColumn="True"/>
                            <px:PXPanel ID="PXPanel1" runat="server" RenderStyle="Simple">
                            <px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="S" StartColumn="True"/>
                                <px:PXNumberEdit ID="edCuryLineTotal" runat="server" DataField="CuryLineTotal" Enabled="False"/>
                                <px:PXNumberEdit ID="edCuryMiscTot" runat="server" DataField="CuryMiscTot"/>
                                <px:PXNumberEdit ID="edCuryDiscTot" runat="server" DataField="CuryDiscTot" Enabled="False"/>
                                <px:PXNumberEdit ID="edCuryTaxTotal2" runat="server" DataField="CuryTaxTotal" Enabled="False"/>
                                <px:PXNumberEdit ID="edCuryFreightAmt" runat="server" CommitChanges="True" DataField="CuryFreightAmt"/>
                                <px:PXNumberEdit ID="edCuryPremiumFreightAmt" runat="server" CommitChanges="True" DataField="CuryPremiumFreightAmt"/>
                                <px:PXNumberEdit ID="edCuryPaymentTotal1" runat="server" DataField="CuryPaymentTotal" Enabled="False"/>
                                <px:PXNumberEdit ID="edCuryCCCapturedAmt1" runat="server" DataField="CuryCCCapturedAmt" Enabled="False"/>
                            </px:PXPanel>
                            <px:PXLayoutRule runat="server" StartColumn="True" />
                            <px:PXFormView ID="formT" runat="server" DataMember="CurrentDocument" DataSourceID="ds" RenderStyle="Simple">
                                <Template>
                                    <px:PXLayoutRule runat="server" StartGroup="True" LabelsWidth="M" ControlSize="M" GroupCaption="Cash Discount Info"/>
                                    <px:PXNumberEdit runat="server" DataField="CuryDiscountedDocTotal" ID="edCuryDiscountedDocTotal" Enabled="false" />
                                    <px:PXNumberEdit runat="server" DataField="CuryDiscountedTaxableTotal" ID="edCuryDiscountedTaxableTotal" Enabled="false" />
                                    <px:PXNumberEdit runat="server" DataField="CuryDiscountedPrice" ID="edCuryDiscountedPrice" Enabled="false" />
                                </Template>
                            </px:PXFormView>
                        </Template>
                    </px:PXFormView>
                </Template>
            </px:PXTabItem>
               <px:PXTabItem Text="ROT/RUT Details" VisibleExp="DataControls[&quot;chkRUTROT&quot;].Value == 1" BindingContext="form" RepaintOnDemand="false">
                <Template>
                    <px:PXFormView runat="server" SkinID="Transparent" ID="RUTROTForm" DataSourceID="ds" DataMember="Rutrots">
                        <Template>
                            <px:PXLayoutRule runat="server" ControlSize="M" LabelsWidth="SM" StartColumn="True" GroupCaption="RUT and ROT Settings" />
                            <px:PXCheckBox runat="server" DataField="AutoDistribution" CommitChanges="True" ID="chkRRAutoDistribution" AlignLeft="true" />
                            <px:PXGroupBox runat="server" DataField="RUTROTType" CommitChanges="True" RenderStyle="Simple" ID="gbRRType">
                                <ContentLayout Layout="Stack" Orientation="Horizontal" />
                                <Template>
                                    <px:PXRadioButton runat="server" Value="O" ID="gbRRType_opO" GroupName="gbRRType" Text="ROT" />
                                    <px:PXRadioButton runat="server" Value="U" ID="gbRRType_opU" GroupName="gbRRType" Text="RUT" />
                                </Template>
                            </px:PXGroupBox>
                            <px:PXTextEdit runat="server" DataField="ROTAppartment" ID="edRAppartment" />
                            <px:PXTextEdit runat="server" DataField="ROTEstate" ID="edRREstate" />
                            <px:PXTextEdit runat="server" DataField="ROTOrganizationNbr" ID="edRROrganizationNbr" CommitChanges="true" />
                            <px:PXLayoutRule runat="server" GroupCaption="RUT and ROT Distribution" />
                            <px:PXGrid runat="server" DataSourceID="ds" Width="350px" AllowFilter="false" AllowSearch="false" Height="200px" SkinID="DetailsInTab" ID="gridDistribution"
                                Caption="RUT and ROT Distribution" CaptionVisible="false">
                                <Mode InitNewRow="true" />
                                <Levels>
                                    <px:PXGridLevel DataMember="RRDistribution">
                                        <RowTemplate>
                                            <px:PXTextEdit runat="server" DataField="PersonalID" ID="edPersonalID" />
                                            <px:PXNumberEdit runat="server" DataField="CuryAmount" ID="edAmountRR" />
                                        </RowTemplate>
                                        <Columns>
                                            <px:PXGridColumn DataField="PersonalID" Width="160px" />
                                            <px:PXGridColumn DataField="CuryAmount" Width="100px" TextAlign="Right" />
                                            <px:PXGridColumn DataField="Extra" Width="60px" Type="CheckBox" />
                                        </Columns>

                                    </px:PXGridLevel>
                                </Levels>
                                <ActionBar>
                                    <Actions>
                                        <ExportExcel Enabled="false" />
                                        <AddNew Enabled="true" />
                                        <Delete Enabled="true" />
                                        <AdjustColumns Enabled="true" />
                                    </Actions>
                                </ActionBar>
                            </px:PXGrid>
                            <px:PXLayoutRule runat="server" ControlSize="M" LabelsWidth="SM" StartColumn="True" GroupCaption="RUT and ROT Totals" />
                            <px:PXNumberEdit runat="server" DataField="DeductionPct" CommitChanges="True" ID="edRRDeduction" />
                            <px:PXNumberEdit runat="server" DataField="CuryTotalAmt" ID="edRRTotalAmt" Enabled="false" />
                            <px:PXNumberEdit runat="server" DataField="CuryOtherCost" ID="edRUTROTOtherCost" Enabled="false" />
                            <px:PXNumberEdit runat="server" DataField="CuryMaterialCost" ID="edRUTROTMaterialCost" Enabled="false" />
                            <px:PXNumberEdit runat="server" DataField="CuryWorkPrice" ID="edRUTROTWorkPrice" Enabled="false" />
                            <px:PXNumberEdit runat="server" DataField="CuryDistributedAmt" ID="edRRAvailAmt" Enabled="false" />
                            <px:PXNumberEdit runat="server" DataField="CuryUndistributedAmt" ID="edRRUndsitributedAmt" Enabled="false" />
                        </Template>
                    </px:PXFormView>
                </Template>
            </px:PXTabItem>
        </Items>
        <CallbackCommands>
            <Search CommitChanges="True" PostData="Page" />
            <Refresh CommitChanges="True" PostData="Page" RepaintControlsIDs="ds" />
        </CallbackCommands>
        <AutoSize Container="Window" Enabled="True" MinHeight="180" />
    </px:PXTab>

    <px:PXSmartPanel ID="PanelAddShipment" runat="server" Height="396px" Style="z-index: 108; left: 216px; position: absolute;
        top: 171px" Width="873px" CommandName="AddShipment" CommandSourceID="ds" Caption="Add Order" CaptionVisible="True" LoadOnDemand="true"
        CallBackMode-CommitChanges="True" CallBackMode-PostData="Page" AutoCallBack-Command="Refresh" AutoCallBack-Enabled="True"
        AutoCallBack-Target="grid4" Key="ShipmentList" >
        <px:PXGrid ID="grid4" runat="server" Height="240px" Width="100%" DataSourceID="ds" BatchUpdate="true" Style="border-width: 1px 0px"
            AutoAdjustColumns="true" SkinID="Inquire" FilesIndicator="false" NoteIndicator="false">
            <AutoSize Enabled="true" />
            <Levels>
                <px:PXGridLevel DataMember="shipmentlist">
                    <Columns>
                        <px:PXGridColumn AllowNull="False" DataField="Selected" DataType="Boolean" DefValueText="False" TextAlign="Center" Type="CheckBox"
                            AllowCheckAll="True" AllowSort="False" AllowMove="False" Width="20px" />
                        <px:PXGridColumn AllowUpdate="False" DataField="OrderType" Width="70px" />
                        <px:PXGridColumn AllowUpdate="False" DataField="OrderNbr" Width="108px" />
                        <px:PXGridColumn AllowUpdate="False" DataField="ShipmentNbr" Width="108px" />
                        <px:PXGridColumn AllowUpdate="False" DataField="CustomerID" DisplayFormat="AAAAAAAAAA" Width="81px" />
                        <px:PXGridColumn AllowUpdate="False" DataField="CustomerLocationID" DisplayFormat="&gt;AAAAAAA" Width="63px" />
                        <px:PXGridColumn AllowUpdate="False" DataField="ShipDate" DataType="DateTime" Width="90px" />
                        <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="ShipmentQty" DataType="Decimal" Decimals="2" DefValueText="0.0"
                            TextAlign="Right" Width="81px" />
                    </Columns>
                    <Layout ColumnsMenu="False" />
                </px:PXGridLevel>
            </Levels>
        </px:PXGrid>
        <px:PXPanel ID="PXPanel1" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton1" runat="server" CommandName="AddShipment" CommandSourceID="ds" Text="Add" SyncVisible="false"/>
            <px:PXButton ID="PXButton2" runat="server" DialogResult="OK" Text="Add &amp; Close" />
            <px:PXButton ID="PXButton3" runat="server" CommandName="AddShipmentCancel" CommandSourceID="ds" DialogResult="Cancel" Text="Cancel" SyncVisible="false"/>
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
                    <px:PXDropDown ID="edRecalcTerget" runat="server" DataField="RecalcTarget" />
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
