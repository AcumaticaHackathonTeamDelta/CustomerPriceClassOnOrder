<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SO305000.aspx.cs"
    Inherits="Page_SO305000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.SO.SOPaymentEntry" PrimaryView="Document">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="Cancel" PopupVisible="true" Visible="true" />
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" PopupVisible="true" />
            <px:PXDSCallbackCommand Name="Insert" PostData="Self" />
            <px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="True" />
            <px:PXDSCallbackCommand Name="Last" PostData="Self" />
            <px:PXDSCallbackCommand StartNewGroup="True" Name="Release" PopupVisible="true" CommitChanges="true" />
            <px:PXDSCallbackCommand Visible="false" Name="ViewBatch" />
            <px:PXDSCallbackCommand Name="Action" CommitChanges="True" StartNewGroup="True" />
            <px:PXDSCallbackCommand Name="Inquiry" RepaintControls="All" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="Report" Visible="True" CommitChanges="true" />
            <px:PXDSCallbackCommand Name="CurrencyView" Visible="False" />
            <px:PXDSCallbackCommand Name="NewCustomer" Visible="False" />
            <px:PXDSCallbackCommand Visible="false" Name="EditCustomer" />
            <px:PXDSCallbackCommand Visible="false" Name="CustomerDocuments" />
            <px:PXDSCallbackCommand Visible="false" CommitChanges="true" Name="LoadInvoices" />
            <px:PXDSCallbackCommand Visible="false" Name="ReverseApplication" CommitChanges="True" DependOnGrid="detgrid2" />
            <px:PXDSCallbackCommand Name="ViewDocumentToApply" DependOnGrid="detgrid" Visible="False" />
            <px:PXDSCallbackCommand Visible="false" Name="ViewApplicationDocument" DependOnGrid="detgrid2" />
            <px:PXDSCallbackCommand Visible="false" Name="ViewCurrentBatch" DependOnGrid="detgrid2" />
            <px:PXDSCallbackCommand Visible="false" CommitChanges="true" Name="CaptureCCPayment" />
            <px:PXDSCallbackCommand Visible="false" CommitChanges="true" Name="AuthorizeCCPayment" />
            <px:PXDSCallbackCommand Visible="false" CommitChanges="true" Name="VoidCCPayment" />
            <px:PXDSCallbackCommand Visible="false" CommitChanges="true" Name="CreditCCPayment" />
            <px:PXDSCallbackCommand Visible="false" CommitChanges="true" Name="RecordCCPayment" />
            <px:PXDSCallbackCommand Visible="false" CommitChanges="true" Name="CaptureOnlyCCPayment" />
        </CallbackCommands>
    </px:PXDataSource>
    <px:PXSmartPanel ID="pnlRecordCCPayment" runat="server" Caption="Record CC Payment" CaptionVisible="True" Height="170px"
        Key="ccPaymentInfo1" LoadOnDemand="True" CommandSourceID="ds" CommandName="RecordCCPayment" ShowAfterLoad="true" Style="z-index: 108;
        left: 495px; position: absolute;" Width="600px" AutoCallBack-Command="Refresh" AutoCallBack-Enabled="true" AutoCallBack-Target="frmCCPaymentInfo">
        <px:PXFormView ID="frmCCPaymentInfo" runat="server" Caption="CC Payment Data" DataMember="ccPaymentInfo" DataSourceID="ds"
            Style="z-index: 100; border: none" Width="100%" CaptionVisible="false" SkinID="Transparent">
            <Template>
                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                <px:PXTextEdit ID="edPCTranNumber" runat="server" DataField="PCTranNumber" />
                <px:PXTextEdit ID="edAuthNumber" runat="server" DataField="AuthNumber" />
            </Template>
        </px:PXFormView>
        <px:PXPanel ID="PXPanel1" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton7" runat="server" DialogResult="OK" Text="Save" />
            <px:PXButton ID="PXButton8" runat="server" DialogResult="Cancel" Text="Cancel" />
        </px:PXPanel>
    </px:PXSmartPanel>
    <px:PXSmartPanel ID="pnlCaptureCCOnly" runat="server" Caption="CC Payment with External Authorization" CaptionVisible="True"
        Height="170px" Key="ccPaymentInfo" LoadOnDemand="True" CommandSourceID="ds" ShowAfterLoad="True" Style="z-index: 108;
        left: 495px; position: absolute;" Width="600px" AutoCallBack-Command="Refresh" 
        AutoCallBack-Target="frmCCPaymentInfo1">
        <px:PXFormView ID="frmCCPaymentInfo1" runat="server" Caption="CC Payment Data" DataMember="ccPaymentInfo" DataSourceID="ds"
            Style="z-index: 100; border: none" Width="100%" CaptionVisible="false" SkinID="Transparent">
            <Template>
                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                <px:PXTextEdit ID="edAuthNumber" runat="server" DataField="AuthNumber"/>
            </Template>
        </px:PXFormView>
        <px:PXPanel ID="PXPanel2" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton1" runat="server" DialogResult="OK" Text="Save" />
            <px:PXButton ID="PXButton2" runat="server" DialogResult="Cancel" Text="Cancel" />
        </px:PXPanel>
    </px:PXSmartPanel>
    <px:PXSmartPanel ID="pnlLoadOpts" runat="server"  Caption="Load Documents" DesignView="Content" CaptionVisible="True" Key="loadOpts">
        <px:PXFormView ID="loform" runat="server" DataSourceID="ds" Width="100%" DataMember="loadOpts" CaptionVisible="False"  SkinID="Transparent">
            <Template>
                <px:PXLayoutRule runat="server" ControlSize="SM" LabelsWidth="SM" StartColumn="True"/>
                <px:PXDateTimeEdit ID="edFromDate" runat="server" DataField="FromDate"/>
                <px:PXDateTimeEdit ID="edTillDate" runat="server" DataField="TillDate"/>
                <px:PXNumberEdit ID="edMaxDocs" runat="server" DataField="MaxDocs"/>
                <px:PXGroupBox ID="gbOrderBy" runat="server" Caption="Order By" DataField="OrderBy" RenderStyle="Fieldset">
                    <Template>
                        <px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" StartRow="True"/>
                        <px:PXRadioButton ID="rbDueDateRefNbr" runat="server" Text="Due Date, Order Nbr." Value="DUE" GroupName="gbOrderBy" />
                        <px:PXRadioButton ID="rbDocDateRefNbr" runat="server" Text="Doc. Date, Order Nbr." Value="DOC" GroupName="gbOrderBy" />
                        <px:PXRadioButton ID="rbRefNbr" runat="server" Text="Order Nbr." Value="REF" GroupName="gbOrderBy" />
                    </Template>
                </px:PXGroupBox>
            </Template>
            <ContentStyle BorderWidth="0px"/>
        </px:PXFormView>
        <px:PXPanel ID="PXPanel3" runat="server" SkinID="Buttons">
            <px:PXButton ID="btnOK" runat="server" DialogResult="OK" Text="OK"/>
            <px:PXButton ID="btnCancel" runat="server" DialogResult="Cancel" Text="Cancel"/>
        </px:PXPanel>
    </px:PXSmartPanel>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Document" Caption="Payment Summary"
        NoteIndicator="True" FilesIndicator="True" ActivityIndicator="true" ActivityField="NoteActivity" LinkIndicator="true"
        NotifyIndicator="true" DefaultControlID="edDocType">
        <Activity HighlightColor="" SelectedColor="" Width="" Height=""></Activity>
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="S" />
            <px:PXDropDown ID="edDocType" runat="server" DataField="DocType" SelectedIndex="-1">
             </px:PXDropDown>
            <px:PXSelector ID="edRefNbr" runat="server" DataField="RefNbr" AutoRefresh="True">
             </px:PXSelector>
            <px:PXDropDown ID="edStatus" runat="server" AllowNull="False" DataField="Status" Enabled="False" SelectedIndex="2" />
            <px:PXCheckBox CommitChanges="True" ID="chkHold" runat="server" DataField="Hold" />
            <px:PXDateTimeEdit CommitChanges="True" ID="edAdjDate" runat="server" DataField="AdjDate" />
            <px:PXSelector CommitChanges="True" ID="edAdjFinPeriodID" runat="server" DataField="AdjFinPeriodID" />
            <px:PXTextEdit CommitChanges="True" ID="edExtRefNbr" runat="server" DataField="ExtRefNbr" />
            <px:PXLayoutRule runat="server" ColumnSpan="2" />
            <px:PXTextEdit ID="edDocDesc" runat="server" DataField="DocDesc" />
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
            <px:PXSegmentMask CommitChanges="True" ID="edCustomerID" runat="server" DataField="CustomerID" AllowEdit="True" />
            <px:PXSegmentMask CommitChanges="True" ID="edCustomerLocationID" runat="server" DataField="CustomerLocationID" AutoRefresh="True" />
            <px:PXSelector CommitChanges="True" ID="edPaymentMethodID" runat="server" DataField="PaymentMethodID"
                           AutoRefresh="true" />
            <px:PXSelector CommitChanges="True" ID="edPMInstanceID" runat="server" DataField="PMInstanceID" TextField="Descr" AutoRefresh="True"
                AutoGenerateColumns="True" />
            <px:PXSegmentMask CommitChanges="True" ID="edCashAccountID" runat="server" DataField="CashAccountID" />
            <pxa:PXCurrencyRate ID="edCuryID" runat="server" DataField="CuryID" DataKeyNames="CuryID" DataMember="_Currency_" DataSourceID="ds"
                InputMask="&gt;LLLLL" MaxLength="5" RateTypeView="_ARPayment_CurrencyInfo_" />
            <px:PXDateTimeEdit ID="edDepositAfter" runat="server" DataField="DepositAfter" Enabled="False" />
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
            <px:PXCheckBox ID="chkIsCCPayment" runat="server" DataField="IsCCPayment" Enabled="False" />
            <px:PXSelector ID="edRefTranExtNbr" runat="server" DataField="RefTranExtNbr" ValueField="PCTranNumber">
                <Parameters>
                    <px:PXControlParam ControlID="form" Name="ARPayment.pMInstanceID" PropertyName="DataControls[&quot;edPMInstanceID&quot;].Value" />
                </Parameters>
            </px:PXSelector>
            <px:PXTextEdit ID="edCCPaymentStateDescr" runat="server" DataField="CCPaymentStateDescr" Enabled="False" />
            <px:PXNumberEdit CommitChanges="True" ID="edCuryOrigDocAmt" runat="server" DataField="CuryOrigDocAmt" />
            <px:PXNumberEdit ID="edCuryUnappliedBal" runat="server" DataField="CuryUnappliedBal" Enabled="False" />
            <px:PXNumberEdit ID="edCurySOApplAmt" runat="server" DataField="CurySOApplAmt" Enabled="False" />
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXTab ID="tab" runat="server" Width="100%" Height="175px">
        <Items>
            <px:PXTabItem Text="Document to Apply">
                <Template>
                    <px:PXGrid ID="detgrid" runat="server" DataSourceID="ds" Style="z-index: 100; left: 0px; top: 0px; height: 382px;" Width="100%"
                        BorderWidth="0px" SkinID="Details" AdjustPageSize="Auto" AllowPaging="True">
                        <Levels>
                            <px:PXGridLevel DataMember="SOAdjustments">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                                    <px:PXSelector ID="edAdjdOrderType" runat="server" DataField="AdjdOrderType" AutoRefresh="true" />
                                    <px:PXSelector CommitChanges="True" ID="edAdjdOrderNbr" runat="server" DataField="AdjdOrderNbr" AutoRefresh="true">
                                        <Parameters>
                                            <px:PXControlParam ControlID="detgrid" Name="SOAdjust.adjdOrderType" PropertyName="DataValues[&quot;AdjdOrderType&quot;]" />
                                        </Parameters>
                                    </px:PXSelector>
                                    <px:PXNumberEdit ID="edCuryAdjgAmt" runat="server" DataField="CuryAdjgAmt" />
                                    <px:PXNumberEdit ID="edCuryAdjgBilledAmt" runat="server" DataField="CuryAdjgBilledAmt" />
                                    <px:PXDateTimeEdit ID="edAdjdOrderDate" runat="server" DataField="AdjdOrderDate" Enabled="False" />
                                    <px:PXDateTimeEdit ID="edSOOrder__DueDate" runat="server" DataField="SOOrder__DueDate" />
                                    <px:PXDateTimeEdit ID="edSOOrder__DiscDate" runat="server" DataField="SOOrder__DiscDate" />
                                    <px:PXLayoutRule runat="server" ColumnSpan="2" />
                                    <px:PXTextEdit ID="edSOOrder__OrderDesc" runat="server" DataField="SOOrder__OrderDesc" />
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                                    <px:PXNumberEdit ID="edCuryDocBal" runat="server" DataField="CuryDocBal" Enabled="False" />
                                    <px:PXTextEdit ID="edAdjgCuryID" runat="server" DataField="AdjgCuryID" />
                                    <px:PXMaskEdit ID="edSOOrder__InvoiceNbr" runat="server" DataField="SOOrder__InvoiceNbr" InputMask="&gt;CCCCCCCCCCCCCCC" />
                                    <px:PXDateTimeEdit ID="edSOOrder__InvoiceDate" runat="server" DataField="SOOrder__InvoiceDate" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="AdjdOrderType" Label="Order Type" RenderEditorText="True" />
                                    <px:PXGridColumn DataField="AdjdOrderNbr" Label="Order Nbr." AutoCallBack="True" LinkCommand="ViewDocumentToApply" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="AdjgDocType" Label="Doc. Type" RenderEditorText="True" Visible="False" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="AdjgRefNbr" DisplayFormat="&gt;aaaaaaaaaaaaaaa" Label="Reference Nbr." Visible="False" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryAdjgAmt" Label="Applied To Order" TextAlign="Right" Width="100px" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryAdjgBilledAmt" Label="Transferred to Invoice" TextAlign="Right" Width="100px" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="AdjdOrderDate" Label="Date" Width="90px" />
                                    <px:PXGridColumn DataField="SOOrder__DueDate" Label="Due Date" Width="90px" />
                                    <px:PXGridColumn DataField="SOOrder__DiscDate" Label="Cash Discount Date" Width="90px" />
                                    <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="CuryDocBal" Label="Balance" TextAlign="Right" Width="100px" />
                                    <px:PXGridColumn DataField="SOOrder__OrderDesc" Label="Sales Order-Description" Width="200px" />
                                    <px:PXGridColumn DataField="AdjdCuryID" Label="Currency" />
                                    <px:PXGridColumn DataField="SOOrder__InvoiceNbr" DisplayFormat="&gt;CCCCCCCCCCCCCCC" Label="Invoice Nbr." />
                                    <px:PXGridColumn DataField="SOOrder__InvoiceDate" Label="Invoice Date" Width="90px" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" MinHeight="150" />
                        <ActionBar>
                            <CustomItems>
                                <px:PXToolBarButton Text="Load Documents" Tooltip="Load Documents">
                                    <AutoCallBack Command="LoadInvoices" Target="ds">
                                        <Behavior CommitChanges="True" />
                                    </AutoCallBack>
                                </px:PXToolBarButton>
                                <px:PXToolBarButton CommandName="ViewDocumentToApply" CommandSourceID="ds" Text="View Document" />
                            </CustomItems>
                        </ActionBar>
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Financial Detail">
                <Template>
                    <px:PXFormView ID="form2" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="CurrentDocument"
                        CaptionVisible="False" SkinID="Transparent">
                        <Template>
                            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                            <px:PXSelector ID="edBatchNbr" runat="server" DataField="BatchNbr" Enabled="False" AllowEdit="True" />
                            <px:PXSegmentMask CommitChanges="True" ID="edBranchID" runat="server" DataField="BranchID" />
                            <px:PXSegmentMask CommitChanges="True" ID="edARAccountID" runat="server" DataField="ARAccountID" AutoGenerateColumns="true" />
                            <px:PXSegmentMask ID="edARSubID" runat="server" DataField="ARSubID" AutoRefresh="true" AutoGenerateColumns="true">
                                <Parameters>
                                    <px:PXControlParam ControlID="form2" Name="ARRegister.aRAccountID" PropertyName="DataControls[&quot;edARAccountID&quot;].Value" />
                                </Parameters>
                            </px:PXSegmentMask>
                            <px:PXDateTimeEdit CommitChanges="True" ID="edDocDate" runat="server" DataField="DocDate" />
                            <px:PXSelector CommitChanges="True" ID="edFinPeriodID" runat="server" DataField="FinPeriodID" />
                            <px:PXLayoutRule runat="server" Merge="True" />
                            <px:PXDateTimeEdit CommitChanges="True" Size="s" ID="edClearDate" runat="server" DataField="ClearDate" />
                            <px:PXCheckBox CommitChanges="True" ID="edCleared" runat="server" DataField="Cleared" />
                            <px:PXLayoutRule runat="server" Merge="False" />
                            <px:PXCheckBox CommitChanges="True" ID="chkDepositAsBatch" runat="server" DataField="DepositAsBatch" />
                            <px:PXLayoutRule runat="server" Merge="True" />
                            <px:PXDateTimeEdit Size="s" ID="edDepositDate" runat="server" DataField="DepositDate" Enabled="False" />
                            <px:PXCheckBox ID="chkDeposited" runat="server" DataField="Deposited" />
                            <px:PXLayoutRule runat="server" Merge="False" />
                            <px:PXTextEdit ID="edDepositNbr" runat="server" DataField="DepositNbr" />
                        </Template>
                        <AutoSize Enabled="True" />
                    </px:PXFormView>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Credit Card Processing Info" BindingContext="form" VisibleExp="DataControls[&quot;chkIsCCPayment&quot;].Value = 1">
                <Template>
                    <px:PXGrid ID="grdCCProcTran" runat="server" DataSourceID="ds" Height="120px" Width="100%" BorderWidth="0px" Style="left: 0px;
                        top: 0px;" SkinID="DetailsInTab">
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
                                    <px:PXLayoutRule ID="PXLayoutRule2" runat="server" Merge="True" />
                                    <px:PXNumberEdit Size="xxs" ID="edTranNbr" runat="server" DataField="TranNbr" />
                                    <px:PXDropDown Size="xs" ID="edProcStatus" runat="server" AllowNull="False" DataField="ProcStatus" />
                                    <px:PXLayoutRule ID="PXLayoutRule3" runat="server" Merge="False" />
                                    <px:PXLayoutRule ID="PXLayoutRule6" runat="server" Merge="True" />
                                    <px:PXTextEdit Size="xs" ID="edProcessingCenterID" runat="server" AllowNull="False" DataField="ProcessingCenterID" />
                                    <px:PXDropDown Size="m" ID="edCVVVerificationStatus" runat="server" DataField="CVVVerificationStatus" />
                                    <px:PXLayoutRule ID="PXLayoutRule7" runat="server" Merge="False" />
                                    <px:PXDropDown ID="edTranType" runat="server" DataField="TranType" />
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
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" MinHeight="50" MinWidth="50" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
        </Items>
        <AutoSize Container="Window" Enabled="True" MinHeight="180" />
    </px:PXTab>
</asp:Content>
