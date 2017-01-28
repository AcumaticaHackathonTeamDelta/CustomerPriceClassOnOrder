<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="AR503000.aspx.cs" Inherits="Page_AR503000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" Width="100%" runat="server" Visible="True" PrimaryView="CyclesList" TypeName="PX.Objects.AR.ARStatementProcess" />
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
    <px:PXGrid ID="grid" runat="server" Height="400px" Width="100%" Style="z-index: 100" AllowPaging="True" DataSourceID="ds" SkinID="PrimaryInquire" FastFilterFields="StatementCycleId, Descr">
        <Levels>
            <px:PXGridLevel DataMember="CyclesList">
                <Columns>
                    <px:PXGridColumn AllowCheckAll="True" AllowMove="False" AllowNull="False" AllowSort="False" DataField="Selected" TextAlign="Center" Type="CheckBox" Width="30px" />
                    <px:PXGridColumn DataField="StatementCycleId" />
                    <px:PXGridColumn DataField="Descr" Width="300px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="LastStmtDate" Width="90px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="LastFinChrgDate" Width="90px" />
                    <px:PXGridColumn AllowNull="False" DataField="PrepareOn" Width="120px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="NextStmtDate" Width="90px" />
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="400" />
    </px:PXGrid>
</asp:Content>
