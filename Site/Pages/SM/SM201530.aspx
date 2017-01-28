<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/FormDetail.master"
	AutoEventWireup="true" CodeFile="SM201530.aspx.cs" Inherits="Pages_SM_SM201530"
	EnableViewState="False" EnableViewStateMac="False" %>

<asp:Content ID="Content1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="Filter"
		TypeName="PX.SM.TaskManager" PageLoadBehavior="PopulateSavedValues">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="actionStop" BlockPage="true" DependOnGrid="grid" CommitChanges="False"
				RepaintControls="All">
			</px:PXDSCallbackCommand>
			<px:PXDSCallbackCommand Name="actionShow" DependOnGrid="grid">
			</px:PXDSCallbackCommand>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView runat="server" ID="form" Width="100%" DataSourceID="ds"
		DataMember="Filter" Caption="Filter" TemplateContainer="">
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" ControlSize="M" 
				LabelsWidth="M">
			</px:PXLayoutRule>
			<px:PXCheckBox ID="ShowAllUsers" runat="server" AlignLeft="True" DataField="ShowAllUsers"
				SuppressLabel="True" Text="Show All Users" CommitChanges="True">
			</px:PXCheckBox>
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXGrid runat="server" ID="grid" Width="100%" Height="100%" DataSourceID="ds"
		AutoGenerateColumns="Recreate" SkinID="Details" Caption="Operations">
		<AutoSize Enabled="true" Container="Window" />
		<Levels>
			<px:PXGridLevel DataMember="Items">
				<Columns>
					<px:PXGridColumn DataField="User" Width="200px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="Screen">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="Title" Width="150px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="Processed" TextAlign="Right" Width="100px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="Total" TextAlign="Right" Width="100px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="Errors" TextAlign="Right" Width="200px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="WorkTime" Width="100px">
					</px:PXGridColumn>
				</Columns>
				<Layout FormViewHeight=""></Layout>
			</px:PXGridLevel>
		</Levels>
	</px:PXGrid>
</asp:Content>
