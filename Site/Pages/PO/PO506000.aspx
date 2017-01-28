<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="PO506000.aspx.cs"
    Inherits="Page_PO506000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" Width="100%" runat="server" Visible="True" PrimaryView="receiptsList" TypeName="PX.Objects.PO.POLandedCostProcess"/>
       
    
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
    <px:PXGrid ID="grid" runat="server" Height="400px" Width="100%" Style="z-index: 100" AllowPaging="True" ActionsPosition="Top"
        AutoAdjustColumns="True" AllowSearch="true" DataSourceID="ds" Caption="Documents" FastFilterFields="ReceiptType,ReceiptNbr,VendorID,CuryID"
        BatchUpdate="True" SkinID="PrimaryInquire" SyncPosition="True">
        <Levels>
            <px:PXGridLevel DataMember="receiptsList">
                <RowTemplate>
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                    <px:PXCheckBox ID="chkSelected" runat="server" DataField="Selected" />
                    <px:PXDropDown ID="edReceiptType" runat="server" DataField="ReceiptType" Enabled="False" />
                    <px:PXTextEdit ID="edReceiptNbr" runat="server" DataField="ReceiptNbr" Enabled="False" />
                    <px:PXSegmentMask ID="edVendorID" runat="server" DataField="VendorID" Enabled="False" />
                    <px:PXSegmentMask ID="edVendorLocationID" runat="server" DataField="VendorLocationID" Enabled="False" />
                    <px:PXDateTimeEdit ID="edReceiptDate" runat="server" DataField="ReceiptDate" Enabled="False" />
                    <px:PXMaskEdit ID="edCuryID" runat="server" DataField="CuryID" Enabled="False" InputMask="&gt;LLLLL" />
                    <px:PXNumberEdit ID="edCuryOrderTotal" runat="server" DataField="CuryOrderTotal" Enabled="False" />
                    <px:PXNumberEdit ID="edLCTranTotal" runat="server" DataField="LCTranTotal" Enabled="False" />
                </RowTemplate>
                <Columns>
                    <px:PXGridColumn AllowCheckAll="True" AllowNull="False" AllowShowHide="False" DataField="Selected" TextAlign="Center" Type="CheckBox"
                        Width="30px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="ReceiptType" RenderEditorText="True" Width="60px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="ReceiptNbr" Width="80px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="VendorID" DisplayFormat="&gt;AAAAAAAAAA" Width="81px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="VendorLocationID" DisplayFormat="&gt;AAAAAA" Width="54px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="ReceiptDate" Width="40px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="CuryID" DisplayFormat="&gt;LLLLL" Width="36px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="CuryOrderTotal" TextAlign="Right" Width="60px" />
                    <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="LCTranTotal" TextAlign="Right" Width="81px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="LCTranCount" TextAlign="Right" />
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <ActionBar DefaultAction="viewDocument"/>
           
        
        <AutoSize Container="Window" Enabled="True" MinHeight="200" />
    </px:PXGrid>
</asp:Content>
