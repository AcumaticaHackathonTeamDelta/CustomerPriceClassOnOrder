<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="AP506000.aspx.cs" Inherits="Page_AP506000"
    Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" Width="100%" runat="server" Visible="True" PrimaryView="PeriodList" TypeName="PX.Objects.AP.APClosing"/>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
    <px:PXGrid ID="grid" runat="server" Height="400px" Width="100%" Style="z-index: 100" DataSourceID="ds" AllowSearch="true" SkinID="PrimaryInquire" Caption="Financial Periods">
        <Levels>
            <px:PXGridLevel DataMember="PeriodList">
                <Columns>
                    <px:PXGridColumn  DataField="Selected" TextAlign="Center" Type="CheckBox" AllowCheckAll="True" AllowSort="False" AllowMove="False" Width="30px" />
                    <px:PXGridColumn  DataField="FinPeriodID" Width="100px" />
                    <px:PXGridColumn  DataField="Descr" Width="100px" />
                    <px:PXGridColumn  DataField="Active" TextAlign="Center" Type="CheckBox" AllowSort="False" AllowMove="False" Width="40px" />
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="400" />
    </px:PXGrid>
</asp:Content>
