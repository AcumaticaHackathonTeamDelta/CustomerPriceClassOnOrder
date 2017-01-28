<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="FA509000.aspx.cs"
    Inherits="Page_FA509000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" Width="100%" runat="server" Visible="True" PrimaryView="PeriodList" TypeName="PX.Objects.FA.FAClosing" />
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
    <px:PXGrid ID="grid" runat="server" Height="400px" Width="100%" Style="z-index: 100" DataSourceID="ds" AllowSearch="true"
        SkinID="PrimaryInquire" Caption="Financial Periods">
        <Levels>
            <px:PXGridLevel DataMember="PeriodList">
                <Columns>
                    <px:PXGridColumn AllowNull="False" DataField="Selected" TextAlign="Center" Type="CheckBox" AllowCheckAll="True" AllowSort="False"
                        AllowMove="False" Width="30px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="FinPeriodID" DisplayFormat="##-####" Width="100px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="Descr" Width="120px" />
                    <px:PXGridColumn AllowNull="False" DataField="Active" TextAlign="Center" Type="CheckBox" AllowSort="False" AllowMove="False"
                        Width="60px" />
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="200" />
    </px:PXGrid>
</asp:Content>
