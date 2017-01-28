<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="SM204030.aspx.cs" Inherits="Page_SM204030"
	Title="Synchronization Processing" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="Filter" TypeName="PX.Objects.CS.Email.EmailsSyncMaint">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Cancel" />
			<px:PXDSCallbackCommand Name="Process" CommitChanges="true" StartNewGroup="True" />
			<px:PXDSCallbackCommand Name="ProcessAll" CommitChanges="true" />
			<px:PXDSCallbackCommand Name="Reset" CommitChanges="true" DependOnGrid="grid" RepaintControls="All" Visible="False" />
			<px:PXDSCallbackCommand Name="Status" CommitChanges="true" DependOnGrid="grid" RepaintControls="All" Visible="False" PostData="Page" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXSmartPanel ID="pnlStatus" runat="server" CaptionVisible="True" Caption="Synchronization Status" DesignView="Content" Key="CurrentItem" AutoRepaint="true" Height="500px" Width="850px" 
		AutoCallBack-ActiveBehavior="true" AutoCallBack-Behavior-RepaintControls="All">
		<px:PXFormView ID="frmStatus" runat="server" SkinID="Transparent" DataMember="CurrentItem" DataSourceID="ds">
			<Template>
				<px:PXLayoutRule ID="PXLayoutRule1" runat="server"  StartColumn="True" LabelsWidth="SM" ControlSize="M" />
				<px:PXSelector ID="edServer" runat="server" AllowNull="False" DataField="ServerID" Enabled="false"  />
				<px:PXTextEdit ID="edContactsExportedDate" runat="server" AllowNull="False" DataField="ContactsExportDate" Enabled="false" />
				<px:PXTextEdit ID="edEmailsExportedDate" runat="server" AllowNull="False" DataField="EmailsExportDate" Enabled="false" />
				<px:PXTextEdit ID="edTasksExportedDate" runat="server" AllowNull="False" DataField="TasksExportDate" Enabled="false" />
				<px:PXTextEdit ID="edEventsExportedDate" runat="server" AllowNull="False" DataField="EventsExportDate" Enabled="false" />
			
				<px:PXLayoutRule ID="PXLayoutRule2" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
				<px:PXTextEdit ID="edAddress" runat="server" AllowNull="False" DataField="Address" Enabled="false"  />
				<px:PXTextEdit ID="edContactsImportedDate" runat="server" AllowNull="False" DataField="ContactsImportDate" Enabled="false" />
				<px:PXTextEdit ID="edEmailsImportedDate" runat="server" AllowNull="False" DataField="EmailsImportDate" Enabled="false" />
				<px:PXTextEdit ID="edTasksImportedDate" runat="server" AllowNull="False" DataField="TasksImportDate" Enabled="false" />
				<px:PXTextEdit ID="edEventsImportedDate" runat="server" AllowNull="False" DataField="EventsImportDate" Enabled="false" />
			</Template>
		</px:PXFormView>
		<px:PXGrid ID="gridLog" runat="server" DataSourceID="ds" Height="200px" SkinID="Inquire" Caption="Log" Width="100%" AutoAdjustColumns="true" AllowEdit="True" AdjustPageSize="Auto" AllowFilter="true" >
			<Levels>
				<px:PXGridLevel DataMember="OperationLog" >
					<RowTemplate>
						<px:PXSelector ID="selServerID" runat="server" DataField="ServerID" AllowEdit="True" />
					</RowTemplate>
					<Columns>
						<px:PXGridColumn AllowUpdate="False" DataField="ServerID" RenderEditorText="True" Width="100px" />
						<px:PXGridColumn AllowUpdate="False" DataField="Address" RenderEditorText="True" Width="150px" />
						<px:PXGridColumn AllowUpdate="False" DataField="Level" Width="100px" />
						<px:PXGridColumn AllowUpdate="False" DataField="Date" Width="150px" DisplayFormat="g" />
						<px:PXGridColumn AllowUpdate="False" DataField="Message" Width="500px" />
					</Columns>
				</px:PXGridLevel>
			</Levels>
			<AutoSize Enabled="True" MinHeight="150" />
		</px:PXGrid>
		<px:PXFormView ID="frmBtn" runat="server" SkinID="Transparent" DataMember="CurrentItem" DataSourceID="ds">
			<Template>
				<px:PXPanel ID="PXPanel1" runat="server" SkinID="Buttons">
					<px:PXButton ID="PXButton1" runat="server" DialogResult="Cancel" Text="Reset Synchronization" AlignLeft="true">
						<AutoCallBack Command="Reset" Target="ds" ActiveBehavior="false"  />
					</px:PXButton>
					<px:PXButton ID="btnCopyCompanyCancel" runat="server" DialogResult="Cancel" Text="Close" />
				</px:PXPanel>
			</Template>
		</px:PXFormView>
	</px:PXSmartPanel>
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Caption="Operation" Width="100%" DataMember="Filter">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
			<px:PXSelector ID="edServer" runat="server" AllowNull="True" DataField="ServerID" CommitChanges="True" />
			<px:PXLayoutRule ID="PXLayoutRule2" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
			<px:PXSelector ID="edPolicyName" runat="server" AllowNull="True" DataField="PolicyName" CommitChanges="True" />
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100; height: 283px;" Width="100%" SkinID="Details" Caption="Accounts" AutoAdjustColumns="true" AllowEdit="True" SyncPosition="True">
		<Levels>
			<px:PXGridLevel DataMember="SelectedItems" >
				<RowTemplate>
					<px:PXSelector ID="edServerID" runat="server" DataField="ServerID" AllowEdit="True" />
					<px:PXSelector ID="edEmployeeID" runat="server" DataField="EmployeeID" AllowEdit="True" />
					<px:PXSelector ID="edEmailAccountID" runat="server" DataField="EmailAccountID" AllowEdit="True" />
					<px:PXSelector ID="edPolicyName" runat="server" DataField="PolicyName" AllowEdit="True"  />
				</RowTemplate>
				<Columns>
					<px:PXGridColumn AllowNull="False" DataField="Selected" TextAlign="Center"	Type="CheckBox" Width="70" />
					<px:PXGridColumn AllowUpdate="False" DataField="ServerID" RenderEditorText="True" Width="150px" />
					<px:PXGridColumn AllowUpdate="False" DataField="Address" RenderEditorText="True" Width="200px" />
					<px:PXGridColumn AllowUpdate="False" DataField="EmailAccountID" Width="200px" TextAlign="Right" DisplayMode="Text"  />
					<px:PXGridColumn AllowUpdate="False" DataField="EmployeeID" Width="100px" DisplayMode="Value"  />
					<px:PXGridColumn AllowUpdate="False" DataField="EmployeeCD" Width="150px" />		
					<px:PXGridColumn AllowUpdate="False" DataField="PolicyName" Width="200px" DisplayMode="Value"  />
				</Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
		<ActionBar>
			<Actions>
				<Delete MenuVisible="false" ToolBarVisible="false" />
				<AddNew MenuVisible="false" ToolBarVisible="false" />
			</Actions>
			<CustomItems>
				<px:PXToolBarButton Text="Synchronization Status" Key="cmdStatus">
				    <AutoCallBack Command="Status" Target="ds" />
				</px:PXToolBarButton>
			</CustomItems>
		</ActionBar>
	</px:PXGrid>
</asp:Content>
