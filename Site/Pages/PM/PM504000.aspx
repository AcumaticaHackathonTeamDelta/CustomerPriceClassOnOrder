<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true"
    ValidateRequest="false" CodeFile="PM504000.aspx.cs" Inherits="Page_PM504000"
    Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" Width="100%" runat="server" TypeName="PX.Objects.PM.ProjectBalanceValidation"
        PrimaryView="Filter" PageLoadBehavior="PopulateSavedValues" Visible="true" />
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds"
		Style="z-index: 100" Width="100%" DataMember="Filter" Caption="Parameters" 
        DefaultControlID="edSiteID" NoteField="">
        <Activity HighlightColor="" SelectedColor="" Width="" Height=""></Activity>
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartColumn="True" />
            <px:PXCheckBox ID="chkRecalculateUnbilledSummary" runat="server" DataField="RecalculateUnbilledSummary" CommitChanges="true" AlignLeft="True" />
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid ID="grid" runat="server" Height="400px" Width="100%" Style="z-index: 100"
        AllowPaging="True" AllowSearch="true" AdjustPageSize="Auto" DataSourceID="ds" SyncPosition="True"
        SkinID="PrimaryInquire" Caption="Projects" FastFilterFields="ContractCD,Description,CustomerID">
        <Levels>
            <px:PXGridLevel DataKeyNames="ContractCD" DataMember="Items">
                <Columns>
                    <px:PXGridColumn AllowCheckAll="true" DataField="Selected" Label="Selected" TextAlign="Center" Type="CheckBox" Width="60px" />
                    <px:PXGridColumn DataField="ContractCD" Width="108px" />
                    <px:PXGridColumn DataField="Description" Label="Description" Width="400px" />
                    <px:PXGridColumn DataField="CustomerID" Label="Customer" Width="108px" />
                    <px:PXGridColumn DataField="Status" Label="Status" RenderEditorText="True" Width="108px" />
                    <px:PXGridColumn DataField="StartDate" Label="Start Date" Width="90px" />
                    <px:PXGridColumn DataField="ExpireDate" Label="End Date" Width="90px" />
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="200" />
        <ActionBar PagerVisible="False" />
    </px:PXGrid>
</asp:Content>

