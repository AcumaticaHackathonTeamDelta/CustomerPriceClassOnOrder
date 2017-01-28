<%@ Page Language="C#" MasterPageFile="~/MasterPages/Workspace.master" AutoEventWireup="true"
  CodeFile="Default.aspx.cs" Inherits="Frames_Default" Title="Acumatica" %>

<%@ MasterType VirtualPath="~/MasterPages/Workspace.master" %>

<asp:Content ID="c1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource runat="server" ID="ds" TypeName="PX.Dashboards.LayoutMaint" PrimaryView="Dashboard" PageLoadBehavior="SearchSavedKeys">
	    <CallbackCommands>
	        <px:PXDSCallbackCommand Name="designMode" />
	        <px:PXDSCallbackCommand Name="resetToDefault" SelectControlsIDs="dashSet" BlockPage="True" />
	        <px:PXDSCallbackCommand Name="editDashboard" SelectControlsIDs="dashSet" BlockPage="True" />
	    </CallbackCommands>
	</px:PXDataSource>
	
	<px:PXDashboardContainer runat="server" ID="dashSet" DataSourceID="ds" DataMember="Widgets" 
		WizardWidgetViewName="Widget" WizardControlID="pnlWidget"
        CallbackUpdatable="True">
	    <CallBackMode RepaintControls="Unbound" />
		<%--<AutoSize Enabled="true" Container="Window" />--%>
		<Fields DashboardID="DashboardID" WidgetID="WidgetID" Type="Type" Settings="Settings" 
			Height="Height" Width="Width" Column="Column" Row="Row" Caption="Caption" Workspace="Workspace" />
	</px:PXDashboardContainer>

	<px:PXDashboardWizard runat="server" ID="pnlWidget" GraphName="PX.Dashboards.LayoutMaint" ViewName="Dashboard" 
		WidgetViewName="Widgets" WizardWidgetViewName="Widget">
		<Fields DashboardID="DashboardID" WidgetID="WidgetID" Type="Type" Settings="Settings" 
			Height="Height" Width="Width" Column="Column" Row="Row" Caption="Caption" Workspace="Workspace" />
	</px:PXDashboardWizard>
</asp:Content>