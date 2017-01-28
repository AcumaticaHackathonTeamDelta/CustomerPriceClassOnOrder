<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="CA206000.aspx.cs" Inherits="Page_CA206000"
    Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="PaymentTypeInstance" TypeName="PX.Objects.CA.PaymentTypeInstanceMaint">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="Insert" PostData="Self" />
            <px:PXDSCallbackCommand Name="Cancel" PopupVisible="true" PostData="Self" />
            <px:PXDSCallbackCommand CommitChanges="True" PopupVisible="true" ClosePopup="true" Name="Save" />
            <px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="True" />
            <px:PXDSCallbackCommand Name="Last" PostData="Self" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Width="100%" DataMember="PaymentTypeInstance" Caption="Corporate Card" FilesIndicator="True" NoteIndicator="True">
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="m" />
            <px:PXSegmentMask ID="edCashAccountID" runat="server" DataField="CashAccountID" />
            <px:PXSelector Size="l" ID="edPTInstanceID" runat="server" DataField="PTInstanceID" TextField="Descr">
                <Parameters>
                    <px:PXControlParam ControlID="form" Name="PaymentTypeInstance.CashAccountID" PropertyName="DataControls[&quot;edCashAccountID&quot;].Value" />
                </Parameters>
            </px:PXSelector>
            <px:PXCheckBox ID="chkIsActive" runat="server" DataField="IsActive" />
            <px:PXSelector CommitChanges="True" ID="edPaymentTypeID" runat="server" DataField="PaymentMethodID" />
            <px:PXTextEdit Size="l" ID="edDescr" runat="server" AllowNull="False" DataField="Descr" />
            <px:PXSegmentMask ID="edBAccountID" runat="server" DataField="BAccountID" />
        </Template>
        <Parameters>
            <px:PXControlParam ControlID="form" Name="PaymentTypeInstance.cashAccountID" PropertyName="NewDataKey[&quot;CashAccountID&quot;]" Type="String" />
        </Parameters>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Height="150px" Width="100%" MatrixMode="True" Caption="Details">
        <Levels>
            <px:PXGridLevel DataMember="Details">
                <Columns>
                    <px:PXGridColumn AllowUpdate="False" DataField="DetailID_PaymentMethodDetail_descr" Width="120px"></px:PXGridColumn>
                    <px:PXGridColumn DataField="Value" Width="200px" RenderEditorText="True"></px:PXGridColumn>
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
    </px:PXGrid>
</asp:Content>
