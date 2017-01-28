<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="DR503000.aspx.cs"
    Inherits="Page_DR503000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="Filter" TypeName="PX.Objects.DR.DRDraftScheduleProc"/>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" Caption="Selection" DataMember="Filter">
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
            <px:PXDropDown CommitChanges="True" ID="edAccountType" runat="server" AllowNull="False" DataField="AccountType" />
            <px:PXSegmentMask CommitChanges="True" ID="edAccountID" runat="server" DataField="AccountID" />
            <px:PXSegmentMask CommitChanges="True" ID="edSubID" runat="server" DataField="SubID" />
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
            <px:PXSelector CommitChanges="True" ID="edDeferredCode" runat="server" DataField="DeferredCode" />
            <px:PXSelector CommitChanges="True" ID="edBAccountID" runat="server" DataField="BAccountID" />
            <px:PXSegmentMask CommitChanges="True" ID="edComponentID" runat="server" DataField="ComponentID" AllowEdit="true" />
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" Height="150px" SkinID="PrimaryInquire" Caption="Deferral Schedules"
        AllowPaging="True" TabIndex="300" SyncPosition="True">
        <Levels>
            <px:PXGridLevel DataMember="Items">
                <RowTemplate>
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                    <px:PXSelector ID="edScheduleID" runat="server" DataField="ScheduleID" Enabled="False" />
                    <px:PXSelector ID="edRefNbr" runat="server" DataField="RefNbr" Enabled="False" />
                    <px:PXNumberEdit ID="edTotalAmt" runat="server" DataField="TotalAmt" Enabled="False" ValidateRequestMode="Inherit" />
                    <px:PXNumberEdit ID="edDefAmt" runat="server" DataField="DefAmt" Enabled="False" ValidateRequestMode="Inherit" />
                    <px:PXSelector ID="edDefCode" runat="server" DataField="DefCode" Enabled="False" />
                    <px:PXSegmentMask ID="edDefAcctID" runat="server" DataField="DefAcctID" Enabled="False" />
                    <px:PXSegmentMask ID="edDefSubID" runat="server" DataField="DefSubID" Enabled="False" />
                    <px:PXNumberEdit ID="edLineNbr" runat="server" DataField="LineNbr" Enabled="False" ValidateRequestMode="Inherit" />
                    <px:PXDateTimeEdit ID="edDocDate" runat="server" AllowNull="False" DataField="DocDate" Enabled="False" ValidateRequestMode="Inherit" />
                    <px:PXSelector ID="edFinPeriodID" runat="server" DataField="FinPeriodID" Enabled="False" />
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                    <px:PXSelector ID="edBAccountID" runat="server" DataField="BAccountID" Enabled="False" />
                    <px:PXDropDown ID="edDocumentType" runat="server" DataField="DocumentType" Enabled="False" />
                    <px:PXCheckBox ID="chkSelected" runat="server" DataField="Selected" ValidateRequestMode="Inherit" />
                </RowTemplate>
                <Columns>
                    <px:PXGridColumn AllowCheckAll="True" AllowNull="False" DataField="Selected" TextAlign="Center" Type="CheckBox" Width="30px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="ScheduleID" TextAlign="Right" Width="108px" LinkCommand="ViewSchedule" />
                    <px:PXGridColumn AllowUpdate="False" DataField="DefCode" Width="81px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="DocumentType" Width="81px" RenderEditorText="True" />
                    <px:PXGridColumn AllowUpdate="False" DataField="RefNbr" Width="108px" LinkCommand="ViewDocument" />
                    <px:PXGridColumn AllowUpdate="False" DataField="ComponentID" Width="108px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="LineNbr" TextAlign="Right" Width="54px" />
                    <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="TotalAmt" TextAlign="Right" Width="81px" />
                    <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="DefAmt" TextAlign="Right" Width="81px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="DefAcctID" Width="81px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="DefSubID" Width="108px" />
                    <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="DocDate" Width="90px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="FinPeriodID" Width="63px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="BAccountID" Width="108px" />
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="400" />
        <ActionBar DefaultAction="ViewSchedule"/>
    </px:PXGrid>
</asp:Content>
