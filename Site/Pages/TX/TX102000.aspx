<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="TX102000.aspx.cs" Inherits="Page_TX102000"
    Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="Setup" TypeName="PX.Objects.TX.AvalaraMaint" BorderStyle="NotSet" SuspendUnloading="False">
        <CallbackCommands>
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
            <px:PXDSCallbackCommand CommitChanges="True" Name="Test" Visible="false" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" Width="100%" DataMember="Setup" DataSourceID="ds">
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" GroupCaption="Connection Settings" />
            <px:PXCheckBox ID="chkIsActive" runat="server" DataField="IsActive" />
            <px:PXTextEdit ID="edAccount" runat="server" DataField="Account" />
            <px:PXTextEdit ID="edLicence" runat="server" DataField="Licence" TextMode="Password" />
            <px:PXTextEdit ID="edUrl" runat="server" DataField="Url" />
            <px:PXTextEdit ID="edTimeout" runat="server" DataField="Timeout" Size="XXS" />
            <px:PXButton ID="shopRates" runat="server" Text="Test Connection" CommandName="Test" CommandSourceID="ds" />
            <px:PXLayoutRule ID="taxCalculation" runat="server"  LabelsWidth="SM" ControlSize="M" GroupCaption="Tax Calculation" />
            <px:PXCheckBox ID="chkSendRevenueAccount" runat="server" DataField="SendRevenueAccount" />
            <px:PXCheckBox ID="chkAlwaysCheckAddress" runat="server" DataField="AlwaysCheckAddress" />
			<px:PXCheckBox ID="PXCheckBox1" runat="server" DataField="IsInclusiveTax" />
            <px:PXGrid ID="PXGridMapping" runat="server" DataSourceID="ds" Width="400" AllowFilter="False" Caption="Branch - Company Code Mapping" Height="144px" SkinID="ShortList">
                <Levels>
                    <px:PXGridLevel DataMember="Mapping">
                        <Columns>
                            <px:PXGridColumn DataField="BranchID" Width="91px" />
                            <px:PXGridColumn DataField="CompanyCode" Width="160px" />
                        </Columns>
                        <Layout FormViewHeight="" />
                        
                    </px:PXGridLevel>
                </Levels>
            </px:PXGrid>
        </Template>
        <AutoSize Container="Window" Enabled="True" MinHeight="210" />
    </px:PXFormView>
</asp:Content>
