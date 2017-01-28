<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="CA503000.aspx.cs" Inherits="Page_CA503000"
    Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" Width="100%" runat="server" Visible="True" PrimaryView="CABalValidateList" TypeName="PX.Objects.CA.CABalValidate" />
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
    <px:PXGrid ID="grid" runat="server" Height="400px" Width="100%" AllowPaging="True" ActionsPosition="Top" AllowSearch="true" DataSourceID="ds" SkinID="PrimaryInquire" Caption="Cash Accounts"
        FastFilterFields="CashAccountCD, Descr">
        <Levels>
            <px:PXGridLevel DataMember="CABalValidateList">
                <Columns>
                    <px:PXGridColumn AllowNull="False" DataField="Selected" TextAlign="Center" AllowCheckAll="True" AllowSort="False" Type="CheckBox" Width="30px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="CashAccountCD" DisplayFormat="&gt;AAAA" />
                    <px:PXGridColumn AllowUpdate="False" DataField="Descr" Width="200px" />
                </Columns>
            </px:PXGridLevel>
        </Levels> 
        <AutoSize Container="Window" Enabled="True" MinHeight="400" />
    </px:PXGrid>
</asp:Content>
