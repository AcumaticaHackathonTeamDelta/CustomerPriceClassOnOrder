<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="CR208000.aspx.cs" Inherits="Page_CR208000"
	Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="CustomerClass"
		TypeName="PX.Objects.CR.CRCustomerClassMaint">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Insert" PostData="Self" />
			<px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
			<px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="True" />
			<px:PXDSCallbackCommand Name="Last" PostData="Self" />
			<px:PXDSCallbackCommand DependOnGrid="grid" Name="ShowDetails" Visible="False" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%"
		Caption="Business Account Class Summary" DataMember="CustomerClass" FilesIndicator="True"
		NoteIndicator="True" ActivityIndicator="true" ActivityField="NoteActivity" DefaultControlID="edCRCustomerClassID">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartColumn="True" LabelsWidth="SM"
				ControlSize="M" />
            <px:PXLayoutRule ID="PXLayoutRule2" runat="server" Merge="True"/>
			<px:PXSelector ID="edCRCustomerClassID" runat="server" DataField="CRCustomerClassID"
				Size="SM" FilterByAllFields="True" />
            <px:PXCheckBox ID="chkInternal" runat="server" DataField="IsInternal"/>
            <px:PXLayoutRule ID="PXLayoutRule3" runat="server"/>
			<px:PXTextEdit ID="edDescription" runat="server" DataField="Description" Size="XL" />
			<px:PXSelector ID="edDefaultEMailAccount" runat="server" DataField="DefaultEMailAccountID"
				DisplayMode="Text" />
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" SkinID="Details" Height="150px"
		Style="z-index: 100" Width="100%" ActionsPosition="Top" Caption="Attributes" MatrixMode="True">
		<Levels>
			<px:PXGridLevel DataMember="Mapping">
				<Columns>
					<px:PXGridColumn DataField="AttributeID" DisplayFormat="&gt;aaaaaaaaaa" Width="81px"
						AutoCallBack="true" LinkCommand="ShowDetails" />
					<px:PXGridColumn AllowNull="False" DataField="Description" Width="351px" />
					<px:PXGridColumn DataField="SortOrder" TextAlign="Right" Width="81px" />
					<px:PXGridColumn AllowNull="False" DataField="Required" TextAlign="Center" Type="CheckBox" />
					<px:PXGridColumn AllowNull="True" DataField="CSAttribute__IsInternal" TextAlign="Center" Type="CheckBox" />
					<px:PXGridColumn AllowNull="False" DataField="ControlType" Type="DropDownList" Width="81px" />
                    <px:PXGridColumn AllowNull="True" DataField="DefaultValue" Width="100px" RenderEditorText="True" />
				</Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXGrid>
</asp:Content>
