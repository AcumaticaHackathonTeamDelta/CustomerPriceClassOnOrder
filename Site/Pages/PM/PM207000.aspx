<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="PM207000.aspx.cs"
    Inherits="Page_PM207000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.PM.BillingMaint" PrimaryView="Billing">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="Insert" PostData="Self" />
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
            <px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="true" />
            <px:PXDSCallbackCommand Name="Last" PostData="Self" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" Caption="Rule Summary" DataMember="Billing"
        EmailingGraph="" LinkPage="">
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="SM" />
            <px:PXSelector ID="edBillingID" runat="server" DataField="BillingID" DataSourceID="ds" DisplayMode="Value" />
            <px:PXLayoutRule runat="server" ColumnSpan="2" />
            <px:PXTextEdit ID="edDescription" runat="server" DataField="Description" />
            <px:PXLayoutRule runat="server" StartColumn="True" />
            <px:PXCheckBox ID="chkIsActive" runat="server" DataField="IsActive" />
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
   <px:PXGrid ID="gridBillingRules" runat="server" DataSourceID="ds" Style="z-index: 100; height: 395px;" Width="100%" SkinID="Details"
                        Height="395px">
                        <Levels>
                            <px:PXGridLevel DataMember="BillingRules" DataKeyNames="BillingID,AccountGroupID">
                                <RowTemplate>
                                    <px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                                    <px:PXLayoutRule ID="PXLayoutRule2" runat="server" Merge="True" />
                                    <px:PXSegmentMask ID="edAccountGroupID" runat="server" DataField="AccountGroupID" />
                                    <px:PXSegmentMask ID="edAccountID" runat="server" DataField="AccountID" />
                                    <px:PXSegmentMask ID="edCapsAccountGroupID" runat="server" DataField="CapsAccountGroupID"  />
                                    <px:PXSegmentMask Size="s" ID="edSubMask" runat="server" DataField="SubMask" DataMember="_PMBILL_Segments_" />
                                    <px:PXSegmentMask ID="edWipAccountGroupID" runat="server" DataField="WipAccountGroupID" DataMember="_PMAccountGroup_" />
                                    <px:PXSegmentMask ID="PXSegmentMask1" runat="server" DataField="OverflowAccountGroupID" DataMember="_PMAccountGroup_" />
                                    <px:PXCheckBox ID="chkCopyNotesBilling" runat="server" DataField="CopyNotes" />
                                    <px:PXLayoutRule ID="PXLayoutRule3" runat="server" />
                                    <px:PXLayoutRule ID="PXLayoutRule4" runat="server" Merge="True" />
                                    <px:PXTextEdit Size="xxl" ID="edInvoiceDescription" runat="server" DataField="InvoiceDescription" />
                                    <px:PXSegmentMask Size="s" ID="edSubID" runat="server" DataField="SubID" />
                                    <px:PXLayoutRule ID="PXLayoutRule5" runat="server" />
                                    <px:PXCheckBox ID="chkIncludeNonBillable" runat="server" Checked="True" DataField="IncludeNonBillable" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="AccountGroupID" Label="Account Group" Width="81px" />
                                    <px:PXGridColumn DataField="InvoiceDescription" Label="Invoice Description" Width="200px" />
                                    <px:PXGridColumn DataField="AccountSource" RenderEditorText="True" Width="90px" AutoCallBack="True" />
                                    <px:PXGridColumn DataField="SubMask" Width="108px" RenderEditorText="True" />
                                    <px:PXGridColumn AutoCallBack="True" DataField="AccountID" Label="Account" Width="63px" />
                                    <px:PXGridColumn DataField="SubID" Label="Subaccount" Width="108px" />
                                    <px:PXGridColumn DataField="IncludeNonBillable" Label="Include Non-Billable" TextAlign="Center" Type="CheckBox" />
                                    <px:PXGridColumn DataField="LimitQty" Label="Limit Qty." TextAlign="Center" Type="CheckBox" AutoCallBack="True" />
                                    <px:PXGridColumn DataField="LimitAmt" Label="Limit Amount" TextAlign="Center" Type="CheckBox" AutoCallBack="True" />
                                    <px:PXGridColumn DataField="CapsAccountGroupID" Label="Max. Limits Account Group" Width="81px" />
                                    <px:PXGridColumn DataField="OverflowAccountGroupID" Width="81px" />
                                    <px:PXGridColumn DataField="WipAccountGroupID" Width="81px" />
                                    <px:PXGridColumn DataField="CopyNotes" Label="Copy Notes" TextAlign="Center" Type="CheckBox" />
                                </Columns>
                                <Layout FormViewHeight="" />
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" Container="Window" MinHeight="200" />
                    </px:PXGrid>
</asp:Content>
