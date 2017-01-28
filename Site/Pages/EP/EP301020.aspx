<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true"
    ValidateRequest="false" CodeFile="EP301020.aspx.cs" Inherits="Page_EP301020" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.EP.ExpenseClaimDetailEntry" PrimaryView="ClaimDetails">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="Cancel" PopupVisible="true"/>
            <px:PXDSCallbackCommand Name="Delete" PopupVisible="true" ClosePopup="True" />
            <px:PXDSCallbackCommand Name="Action" Visible="True" CommitChanges="true" StartNewGroup="true" />
            <px:PXDSCallbackCommand Visible="False" Name="CurrencyView" />
            <px:PXDSCallbackCommand Name="NewTask" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="NewEvent" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="NewActivity" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="NewMailActivity" Visible="False" CommitChanges="True" PopupCommand="Cancel" PopupCommandTarget="ds" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="ClaimDetails"
        CaptionVisible="False" TabIndex="5200" ActivityIndicator="True" NoteIndicator="True" FilesIndicator="True">
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True">
            </px:PXLayoutRule>
            <px:PXSelector ID="SelectorClaimDetailID" runat="server" DataField="ClaimDetailID" TextField="ClaimDetailID" NullText="<NEW>" Size="s" />
            <px:PXLayoutRule runat="server" StartColumn="True">
            </px:PXLayoutRule>
            <px:PXDropDown ID="PXDropDown1" runat="server" DataField="Status" Enabled="False" />
            <contentstyle borderstyle="None" />
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXTab ID="Tabs" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" Height="100%" DataMember="CurrentClaimDetails" DefaultControlID="ExpenseDate">
        <Items>
            <px:PXTabItem Text="Receipt Details">
                <Template>
                    <px:PXLayoutRule ID="PXLayoutRule2" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" GroupCaption="Expense Details" />
                    <px:PXDateTimeEdit ID="edExpenseDate" runat="server" DataField="ExpenseDate" CommitChanges="True" />
                    <pxa:PXCurrencyRate DataField="CuryID" ID="edCuryID" runat="server" DataSourceID="ds" RateTypeView="_EPExpenseClaimDetails_CurrencyInfo_" DataMember="CurrencyList" />
                    <px:PXTextEdit ID="PXTextEdit1" runat="server" DataField="ExpenseRefNbr" />
                    <px:PXSegmentMask ID="edInventoryID" runat="server" DataField="InventoryID" CommitChanges="True" AllowEdit="True" />
                    <px:PXTextEdit ID="edTranDesc" runat="server" DataField="TranDesc" CommitChanges="True" TextMode="MultiLine" Width="395px" Height="96" />
                    <px:PXSelector ID="edUOM" runat="server" DataField="UOM" CommitChanges="True" Size="S" />
                    <px:PXNumberEdit ID="PXNumberEdit1" runat="server" DataField="Qty" CommitChanges="True" />
                    <px:PXNumberEdit ID="PXNumberEdit2" runat="server" DataField="CuryUnitCost" CommitChanges="True" />
                    <px:PXNumberEdit ID="PXNumberEdit3" runat="server" DataField="CuryExtCost" CommitChanges="True" />
                    <px:PXNumberEdit ID="PXNumberEdit4" runat="server" DataField="CuryEmployeePart" CommitChanges="True" />
                    <px:PXNumberEdit ID="PXNumberEdit5" runat="server" DataField="CuryTranAmt" Enabled="False" />
                    <px:PXLayoutRule ID="PXLayoutRule1" runat="server" GroupCaption="Image" />
                    <px:PXImageUploader Width="395px" Height="275" ID="imgUploader" runat="server" AllowUpload="True" ViewOnly="True" ArrowsOutside="True" LabelText="&nbsp;" />
                    <px:PXLayoutRule ID="PXLayoutInfo" runat="server" StartGroup="True" GroupCaption="Expense Classification" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                    <px:PXCheckBox ID="PXHold" runat="server" DataField="Hold" Enabled="False" />
                    <px:PXCheckBox ID="PXApproved" runat="server" DataField="Approved" Enabled="False" />
                    <px:PXCheckBox ID="PXCheckBox1" runat="server" DataField="Rejected" Enabled="False" />
                    <px:PXCheckBox ID="PXReleased" runat="server" DataField="Released" Enabled="False" />
                    <px:PXCheckBox ID="PXClaimHold" runat="server" DataField="HoldClaim" Enabled="False" />
                    <px:PXSegmentMask CommitChanges="True" ID="edEmployeeID" runat="server" DataField="EmployeeID" DataSourceID="ds" />
                    <px:PXSegmentMask CommitChanges="True" ID="edBranchID" runat="server" DataField="BranchID" DataSourceID="ds" />
                    <px:PXSelector CommitChanges="True" ID="edRefNbr" runat="server" DataField="RefNbr" AutoRefresh="True" AllowEdit="True"/>
                    <px:PXDropDown ID="PXStatusClaim" runat="server" DataField="StatusClaim" Enabled="False" />
                    <px:PXCheckBox ID="chkBillable" runat="server" DataField="Billable" CommitChanges="True" />
                    <px:PXSegmentMask CommitChanges="True" ID="edContract" runat="server" DataField="ContractID" Size="XM" />
                    <px:PXSegmentMask CommitChanges="True" ID="edTaskID" runat="server" DataField="TaskID" Size="XM" AutoRefresh="True" />
                    <px:PXSegmentMask CommitChanges="True" ID="edCustomerID" runat="server" DataField="CustomerID" Size="XM" />
                    <px:PXSegmentMask CommitChanges="True" Size="XM" ID="edCustomerLocationID" runat="server" DataField="CustomerLocationID" AutoRefresh="True" />
                    <px:PXLayoutRule ID="PXLayoutFinencial" runat="server" StartGroup="True" GroupCaption="Financial Details" />
                    <px:PXSegmentMask ID="edExpenseAccountID" runat="server" DataField="ExpenseAccountID" CommitChanges="true" />
                    <px:PXSegmentMask ID="edExpenseSubID" runat="server" DataField="ExpenseSubID" AutoRefresh="True" />
                    <px:PXSegmentMask ID="edSalesAccountID" runat="server" DataField="SalesAccountID" />
                    <px:PXSegmentMask ID="edSalesSubID" runat="server" DataField="SalesSubID" AutoRefresh="True" />
                    <px:PXSelector ID="edTaxCategoryID" runat="server" DataField="TaxCategoryID" AutoRefresh="True" />
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Approval Details" BindingContext="form" RepaintOnDemand="false">
                <Template>
                    <px:PXGrid ID="gridApproval" runat="server" DataSourceID="ds" Width="100%" SkinID="DetailsInTab" Style="left: 0px; top: 0px;">
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
        </Items>
        <AutoSize Container="Window" Enabled="True" />
    </px:PXTab>
</asp:Content>
